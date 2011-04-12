
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using CodeSoda.Impression.Parsers;
using CodeSoda.Impression.Properties;

namespace CodeSoda.Impression
{
	public interface IFilterRunner {
		object RunFilters(object obj, string[] filterTexts, IPropertyBag bag, IMarkupBase markup);
	}

	public class FilterRunner : IFilterRunner {
		private readonly FormatterCache formatterCache = null;
		//private readonly ITagParser tagParser = null;

		public FilterRunner() {
			formatterCache = new FormatterCache();
			FormatterCache.LoadFormatters(formatterCache);
		}


		public object RunFilters(object obj, string[] filterTexts, IPropertyBag bag, IMarkupBase markup)
		{
			foreach (string filterText in filterTexts)
			{
				string[] parameters = null;
				string keyword = ParseParameterizedFilter(filterText, out parameters);

				IFilter filter = formatterCache.Get(keyword);
				//if (filter == null)
				//    throw new ImpressionInterpretException("Unsupported filter detected, " + filterText, markup);

				if (filter != null)
					obj = filter.Run(obj, parameters, bag, markup);

			}
			return obj;
		}

		// attribute parser
		// (\s*)(\w+)(\s*)\((\s*)(("?\w+"?)+)(\s*)(((\s*),(\s*)"?\w+"?)+)(\s*)\)(\s*)
		// (?:\s*)(?<method>\w+)(?:\s*)\((?:\s*)(?:(?:(?<param>(?:\w+)|(?:"\w+"))(?:\s*)(?:,?)(?:\s*))+)\)
		private Regex attributeParserRegex;
		private Regex AttributeParserRegex {
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

		private string ParseParameterizedFilter(string filter, out string[] parameters) {

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

}

