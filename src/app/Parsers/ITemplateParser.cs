using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using CodeSoda.Impression.Properties;

namespace CodeSoda.Impression.Parsers
{
	public interface ITemplateParser {
		string PreProcessTemplate(string templateContent, string templateFolder);
		ParseList ParseTemplate(string template);
	}

	public class TemplateParser : ITemplateParser {

		private ITagFactory tagFactory = null;
		private IReflector reflector = null;
		private IFilterRunner filterRunner = null;

		public TemplateParser(ITagFactory tagFactory, IReflector reflector, IFilterRunner filterRunner) {
			if (tagFactory == null)
				throw new ArgumentNullException("tagFactory", "tagFactory may not be null");
			this.tagFactory = tagFactory;
			
			if (reflector == null)
				throw new ArgumentNullException("reflector");
			this.reflector = reflector;

			if (filterRunner == null)
				throw new ArgumentNullException("filterRunner");
			this.filterRunner = filterRunner;
		}

		public string PreProcessTemplate(string templateContent, string templateFolder) {
			templateContent = PreProcessExtends(templateContent, templateFolder);
			templateContent = PreProcessIncludes(templateContent, templateFolder);

			return templateContent;
		}

		public string PreProcessExtends(string templateContent, string templateFolder) {
			
			// load up any includes
			MatchCollection matches =  ExtendsFinderRegex.Matches(templateContent);

			int matchCount = matches.Count;
			if (matchCount > 1) {
				throw new ImpressionParseException(
					"A template file can only extend one other template file",
					matches[1].ToString(),
					-1, -1
				);
			}

			if (matchCount == 1)
			{
				string masterFileName = matches[0].Groups["File"].ToString();
				string masterFilePath = Path.Combine(templateFolder, masterFileName);

				if (!File.Exists(masterFilePath))
					throw new ImpressionParseException("Could not find tempate file.", matches[0].ToString(), -1, -1);

				// load up the new "master" file
				string masterContent = File.ReadAllText(masterFilePath);

				// parse blocks from master file
				var blockMatches = BlockNameFinderRegex.Matches(masterContent);

				// foreach block in master, grab content from template
				if (blockMatches.Count > 0) {
					int peekIndex = 0;

					// parse blocks from template file into a hashtable for quick lookup
					MatchCollection contentMatches = BlockForFinderRegex.Matches(templateContent);
					Hashtable contentBlocks = new Hashtable(contentMatches.Count);
					foreach(Match content in contentMatches)
					{
						contentBlocks.Add(
							content.Groups["Block"].ToString().ToLower(),
							content.Groups["Content"].ToString()
						);
					}

					// with string builder
					var sb = new StringBuilder(templateContent.Length + masterContent.Length);

					foreach (Match block in blockMatches) {
						Group g = block.Groups[0];
						string blockName = block.Groups["Block"].ToString().ToLower();

						// lookup the content from the extender
						string extendContent = contentBlocks.ContainsKey(blockName)
							? contentBlocks[blockName] as string
							: null;

						// if the index is after the position we have, then copy the content over
						if (g.Index > peekIndex) {
							// add plain content before the extension
							sb.Append(masterContent.Substring(peekIndex, g.Index - peekIndex));
						}

						// if we have some extended content then append it,
						// otherwise keep the original block content
						sb.Append(
							string.IsNullOrEmpty(extendContent)
								? block.Groups["Content"].ToString()
								: extendContent
						);
						

						// update the peekIndex to be the end of the block
						peekIndex = g.Index + g.Length;
					}

					// if theres anything left on the end, then append it also
					if (peekIndex < masterContent.Length) {
						sb.Append(masterContent.Substring(peekIndex));
					}

					templateContent = sb.ToString();
				}
			}

			return templateContent;
		}

		public string PreProcessIncludes(string templateContent, string templateFolder)
		{
			// load up any includes
			MatchCollection matches = IncludeFinderRegex.Matches(templateContent);

			if (matches.Count > 0)
			{
				int peekIndex = 0;
				StringBuilder sb = new StringBuilder();
				foreach (Match match in matches)
				{
					Group g = match.Groups[0];
					string includeFile = Path.Combine(templateFolder, match.Groups["File"].Value);

					// if the index is after the position we have, then copy the content over
					if (g.Index > peekIndex)
					{
						// add plain content before the include
						sb.Append(templateContent.Substring(peekIndex, g.Index - peekIndex));
					}

					// load the file, preprocess it then append it
					if (File.Exists(includeFile))
					{
						string subTemplateContent = File.ReadAllText(includeFile);
						subTemplateContent = PreProcessTemplate(subTemplateContent, templateFolder);
						sb.Append(subTemplateContent);
					}

					// update the peekIndex to be the end of the match
					peekIndex = g.Index + g.Length;
				}

				// if theres anything left on the end, then append it also
				if (peekIndex < templateContent.Length)
				{
					sb.Append(templateContent.Substring(peekIndex));
				}

				templateContent = sb.ToString();
			}

			return templateContent;
		}

		public ParseList ParseTemplate(string template) {

			List<int> newLinePositions = new List<int>();
			for (int i = 0, len = template.Length; i < len; i++)
			{
				if (template[i] == '\n')
				{
					newLinePositions.Add(i);
				}
			}

			ParseList parseList = new ParseList();

			MatchCollection matches = TagFinderRegex.Matches(template);

			int peekIndex = 0;

			foreach (Match match in matches)
			{
				Group g = match.Groups[0];

				if (g.Index > peekIndex)
				{
					parseList.AddRange((ParseExpressions(template.Substring(peekIndex, g.Index - peekIndex))));
				}

				// determine the row number the tag is on and the character position
				int row = 1, pos = 0;
				int len = newLinePositions.Count;
				for (row = 1; row <= len; row++)
				{
					if (g.Index < newLinePositions[row - 1])
					{
						pos = g.Index - ((row <= 1) ? 0 : newLinePositions[row - 2]);
						break;
					}
				}

				parseList.Add(tagFactory.ParseTag(g.Value, row, pos));

				// update the peekIndex to be the end of the match
				peekIndex = g.Index + g.Length;
			}

			if (peekIndex < template.Length)
			{
				parseList.AddRange(ParseExpressions(template.Substring(peekIndex)));
			}

			return parseList;
		}

		public MarkupBase[] ParseExpressions(string markup) {
			List<MarkupBase> fragments = new List<MarkupBase>();


			MatchCollection matches = ExpressionFinderRegex.Matches(markup);

			int peekIndex = 0;

			foreach (Match match in matches) {
				Group g = match.Groups[0];

				// grab the leading content as a literal
				if (g.Index > peekIndex) {
					fragments.Add(new LiteralMarkup(markup.Substring(peekIndex, g.Index - peekIndex)));
				}

				fragments.Add(new ExpressionMarkup(reflector, filterRunner, g.Value));

				// update the peekIndex to be the end of the match
				peekIndex = g.Index + g.Length;
			}

			if (peekIndex < markup.Length) {
				fragments.Add(new LiteralMarkup(markup.Substring(peekIndex)));
			}

			return fragments.ToArray();
		}

		private static Regex includeFinderRegex = null;
		private static Regex IncludeFinderRegex {
			get {
				if (includeFinderRegex == null) {
					includeFinderRegex = new Regex(
						Resources.IncludeFinderPattern,
						RegexOptions.Multiline | RegexOptions.Compiled
					);
				}
				return includeFinderRegex;
			}
		}

		private static Regex extendsFinderRegex = null;
		private static Regex ExtendsFinderRegex {
			get {
				if (extendsFinderRegex == null) {
					extendsFinderRegex = new Regex(
						Resources.ExtendsFinderPattern,
						RegexOptions.Multiline | RegexOptions.Compiled
					);
				}
				return extendsFinderRegex;
			}
		}

		private static Regex blockForFinderRegex = null;
		private static Regex BlockForFinderRegex {
			get {
				if (blockForFinderRegex == null) {
					blockForFinderRegex = new Regex(
						Resources.BlockForFinderPattern,
						RegexOptions.Singleline | RegexOptions.Compiled
					);
				}
				return blockForFinderRegex;
			}
		}

		private static Regex blockNameFinderRegex = null;
		private static Regex BlockNameFinderRegex {
			get {
				if (blockNameFinderRegex == null) {
					blockNameFinderRegex = new Regex(
						Resources.BlockNameFinderPattern,
						RegexOptions.Singleline | RegexOptions.Compiled
					);
				}
				return blockNameFinderRegex;
			}
		}

		private static Regex tagParserRegex;
		private static Regex TagParserRegex {
			get {
				if (tagParserRegex == null) {
					tagParserRegex = new Regex(
						@"<!--\s*\#(?<Tag>[iI][fF]|[eE][lL][sS][eE]|[eE][lL][sS][eE][iI][fF]|[eE][nN][dD][iI][fF]|[fF][oO][rR][eE][aA][cC][hH]|[nN][eE][xX][tT])\s*(?<Expression>{{[\w\s\.|]*}})?\s*-->",
						RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.ExplicitCapture
					);
				}
				return tagParserRegex;
			}
		}

		private static Regex tagFinderRegex;
		private static Regex TagFinderRegex {
			get {
				if (tagFinderRegex == null) {
					tagFinderRegex = new Regex(
						//@"<!--\s*\#(?<Tag>[iI][fF]|[eE][lL][sS][eE]|[eE][lL][sS][eE][iI][fF]|[eE][nN][dD][iI][fF]|[fF][oO][rR][eE][aA][cC][hH]|[nN][eE][xX][tT])\s*(?<Expression>{{[\w\s\.|]*}})?\s*-->",
						Resources.TagFinderPattern,
						RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.ExplicitCapture
					);
				}
				return tagFinderRegex;
			}
		}

		private static Regex expressionFinderRegex;
		public static Regex ExpressionFinderRegex {
			get {
				if (expressionFinderRegex == null) {
					expressionFinderRegex = new Regex(
						Resources.ExpressionFinderPattern,
						RegexOptions.Compiled
					);
				}
				return expressionFinderRegex;
			}
		}

	}
}
