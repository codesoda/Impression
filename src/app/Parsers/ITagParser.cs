
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using CodeSoda.Impression.Properties;

namespace CodeSoda.Impression.Parsers
{
	public interface ITagParser
	{
		bool CanParseTag(string markup);
		TagMarkupBase ParseTag(string markup, int lineNumber, int charPos);
	}

	public abstract class TagParserBase: ITagParser {

		public abstract bool CanParseTag(string markup);
		public abstract TagMarkupBase ParseTag(string markup, int lineNumber, int charPos);


		// attribute parser
		// (\s*)(\w+)(\s*)\((\s*)(("?\w+"?)+)(\s*)(((\s*),(\s*)"?\w+"?)+)(\s*)\)(\s*)
		// (?:\s*)(?<method>\w+)(?:\s*)\((?:\s*)(?:(?:(?<param>(?:\w+)|(?:"\w+"))(?:\s*)(?:,?)(?:\s*))+)\)
		private Regex attributeParserRegex;
		public Regex AttributeParserRegex {
			get {
				if (attributeParserRegex == null) {
					attributeParserRegex = new Regex(
						Resources.AttributeParserPattern,
						RegexOptions.Compiled | RegexOptions.ExplicitCapture
					);
				}
				return attributeParserRegex;
			}
		}

		protected string ParseParameterizedFilter(string filter, out string[] parameters) {

			// check if the attribute can be broken into a parameterized attribute call
			// eg. attribute(val1,val2...etc)

			parameters = null;
			string methodName = null;

			Match m = AttributeParserRegex.Match(filter);

			if (m.Success) {
				// attribute method name
				if (m.Groups["method"].Success) {
					methodName = m.Groups["method"].Value.Trim();
				}

				// parameters to the attribute method
				if (m.Groups["param"].Success) {
					CaptureCollection vals = m.Groups["param"].Captures;
					List<string> parms = new List<string>(vals.Count);
					for (int i = 0, len = vals.Count; i < len; i++) {
						parms.Add(vals[i].Value.Trim());
					}
					parameters = parms.ToArray();
				}
			}
			else {
				methodName = filter;
			}

			return methodName;
		}

	}

	public class TagParserCache: List<ITagParser> {

		public static void LoadParsers(TagParserCache cache, IContainer container)
		{
			if (cache == null)
				return;

			// load all formatters from this assembly
			Type[] types = Assembly.GetExecutingAssembly().GetTypes();
			foreach (Type type in types)
			{
				if (type.GetInterface("ITagParser") != null && !type.IsAbstract)
				{
					ITagParser parser = (ITagParser)container.Resolve(type);
					if (parser != null)
					{
						cache.Add(parser);
					}
				}
			}
		}
	}
}
