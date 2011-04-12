using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using CodeSoda.Impression.Parsers;

namespace CodeSoda.Impression.Tags
{

	public class VarTagParser: ITagParser
	{
		private IReflector reflector;
		private IFilterRunner filterRunner;

		public VarTagParser(IReflector reflector, IFilterRunner filterRunner)
		{
			if (reflector == null)
				throw new ArgumentNullException("reflector");
			this.reflector = reflector;

			if (filterRunner == null)
				throw new ArgumentNullException("filterRunner");
			this.filterRunner = filterRunner;
		}


		public bool CanParseTag(string markup) {
			return VarTagMatcherRegex.IsMatch(markup);
		}

		public TagMarkupBase ParseTag(string markup, int lineNumber, int charPos) {

			VarTagMarkup tagMarkup = null;
			ExpressionMarkup expressionMarkup = null;
			
			Match m = VarTagParserRegex.Match(markup);
			if (m.Success) {

				string tag = m.Groups["Tag"].Success ? m.Groups["Tag"].Value.Trim(): null;
				if (string.IsNullOrEmpty(tag))
					throw new ImpressionParseException("Unsupported Tag Found", markup, lineNumber, charPos);

				string expression = m.Groups["Expression"].Success ? m.Groups["Expression"].Value.Trim() : null;
				if (!string.IsNullOrEmpty(expression)) {
					expressionMarkup = new ExpressionMarkup(reflector, filterRunner, expression, lineNumber, charPos);
				}

				tagMarkup = new VarTagMarkup(
					expressionMarkup,
					markup,
					lineNumber,
					charPos
				);

			}

			if (tagMarkup == null)
				throw new ImpressionParseException("Unsupported Tag Found", markup, lineNumber, charPos);

			return tagMarkup;
		}

		private static Regex _varTagMatcherRegex;
		private static Regex VarTagMatcherRegex {
			get {
				if (_varTagMatcherRegex == null) {
					_varTagMatcherRegex = new Regex(
						@"<!--\s*\#([vV][aA][rR])\s*(?<Expression>.*?)?\s*-->",
						RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.ExplicitCapture
					);
				}
				return _varTagMatcherRegex;
			}
		}

		private static Regex _varTagParserRegex;
		private static Regex VarTagParserRegex {
			get {
				if (_varTagParserRegex == null) {
					_varTagParserRegex = new Regex(
						//@"<!--\s*\#(?<Tag>[fF][oO][rR][eE][aA][cC][hH]|[nN][eE][xX][tT])\s*(?<Expression>{{[\w\s\.|]*}})?\s*-->",
						@"<!--\s*\#(?<Tag>[vV][aA][rR])\s*(?<Expression>.*?)?\s*-->",
						RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.ExplicitCapture
					);
				}
				return _varTagParserRegex;
			}
		}
	}
}
