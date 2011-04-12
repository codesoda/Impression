using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using CodeSoda.Impression.Parsers;

namespace CodeSoda.Impression.Tags
{
	public class ForEachTagParser: ITagParser
	{
		private IReflector reflector;
		private IFilterRunner filterRunner;

		public ForEachTagParser(IReflector reflector, IFilterRunner filterRunner)
		{
			if (reflector == null)
				throw new ArgumentNullException("reflector");
			this.reflector = reflector;

			if (filterRunner == null)
				throw new ArgumentNullException("filterRunner");
			this.filterRunner = filterRunner;
		}


		public bool CanParseTag(string markup) {
			return ForEachTagMatcherRegex.IsMatch(markup);
		}

		public TagMarkupBase ParseTag(string markup, int lineNumber, int charPos) {

			ForEachTagMarkup tagMarkup = null;
			ExpressionMarkup expressionMarkup = null;
			
			Match m = ForEachTagParserRegex.Match(markup);
			if (m.Success) {

				string tag = m.Groups["Tag"].Success ? m.Groups["Tag"].Value.Trim(): null;
				if (string.IsNullOrEmpty(tag))
					throw new ImpressionParseException("Unsupported Tag Found", markup, lineNumber, charPos);

				string expression = m.Groups["Expression"].Success ? m.Groups["Expression"].Value.Trim() : null;
				if (!string.IsNullOrEmpty(expression))
				{
					expressionMarkup = new ExpressionMarkup(reflector, filterRunner, expression, lineNumber, charPos);
				}

				switch(tag.ToLower())
				{
					case "foreach":
						if (expressionMarkup == null)
							throw new ImpressionParseException("FOREACH Tag detected without expression", markup, lineNumber, charPos);

						tagMarkup = new ForEachTagMarkup(
							ForEachTagType.ForEach,
							expressionMarkup,
							markup,
							lineNumber,
							charPos
						);

						break;

					case "next":
						tagMarkup = new ForEachTagMarkup(
							ForEachTagType.Next,
							null,
							markup,
							lineNumber,
							charPos
						);
						break;

				}
			}

			if (tagMarkup == null)
				throw new ImpressionParseException("Unsupported Tag Found", markup, lineNumber, charPos);

			return tagMarkup;
		}

		private static Regex forEachTagMatcherRegex;
		private static Regex ForEachTagMatcherRegex {
			get {
				if (forEachTagMatcherRegex == null)
				{
					forEachTagMatcherRegex = new Regex(
						@"<!--\s*\#([fF][oO][rR][eE][aA][cC][hH]|[nN][eE][xX][tT])\s*(?<Expression>.*?)?\s*-->",
						RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.ExplicitCapture
					);
				}
				return forEachTagMatcherRegex;
			}
		}

		private static Regex forEachTagParserRegex;
		private static Regex ForEachTagParserRegex {
			get {
				if (forEachTagParserRegex == null)
				{
					forEachTagParserRegex = new Regex(
						//@"<!--\s*\#(?<Tag>[fF][oO][rR][eE][aA][cC][hH]|[nN][eE][xX][tT])\s*(?<Expression>{{[\w\s\.|]*}})?\s*-->",
						@"<!--\s*\#(?<Tag>[fF][oO][rR][eE][aA][cC][hH]|[nN][eE][xX][tT])\s*(?<Expression>.*?)?\s*-->",
						RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.ExplicitCapture
					);
				}
				return forEachTagParserRegex;
			}
		}
	}
}
