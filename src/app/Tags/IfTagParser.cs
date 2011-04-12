using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using CodeSoda.Impression.Parsers;

namespace CodeSoda.Impression.Tags
{
	public class IfTagParser: ITagParser
	{
		private IReflector reflector;
		private IFilterRunner filterRunner;

		public IfTagParser(IReflector reflector, IFilterRunner filterRunner) {
			if (reflector == null)
				throw new ArgumentNullException("reflector");
			this.reflector = reflector;

			if (filterRunner == null)
				throw new ArgumentNullException("filterRunner");
			this.filterRunner = filterRunner;
		}


		public bool CanParseTag(string markup) {
			return IfTagMatcherRegex.IsMatch(markup);
		}

		public TagMarkupBase ParseTag(string markup, int lineNumber, int charPos) {

			IfTagMarkup tagMarkup = null;
			ExpressionMarkup expressionMarkup = null;
			
			Match m = IfTagParserRegex.Match(markup);
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
					case "if":
						if (expressionMarkup == null)
							throw new ImpressionParseException("IF Tag detected without expression", markup, lineNumber, charPos);

						tagMarkup = new IfTagMarkup(
							IfTagType.If,
							expressionMarkup,
							markup,
							lineNumber,
							charPos
						);

						break;

					case "elseif":
						if (expressionMarkup == null)
							throw new ImpressionParseException("ELSEIF Tag detected without expression", markup, lineNumber, charPos);

						tagMarkup = new IfTagMarkup(
							IfTagType.ElseIf,
							expressionMarkup,
							markup,
							lineNumber,
							charPos
						);

						break;

					case "else":
						tagMarkup = new IfTagMarkup(
							IfTagType.Else,
							null,
							markup,
							lineNumber,
							charPos
						);
						break;

					case "endif":
						tagMarkup = new IfTagMarkup(
							IfTagType.EndIf,
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

		private static Regex ifTagMatcherRegex;
		private static Regex IfTagMatcherRegex {
			get {
				if (ifTagMatcherRegex == null) {
					ifTagMatcherRegex = new Regex(
						@"<!--\s*\#([iI][fF]|[eE][lL][sS][eE]|[eE][lL][sS][eE][iI][fF]|[eE][nN][dD][iI][fF])\s*({{[-\w\s\.|]*}})?\s*-->",
						RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.ExplicitCapture
					);
				}
				return ifTagMatcherRegex;
			}
		}

		private static Regex ifTagParserRegex;
		private static Regex IfTagParserRegex {
			get {
				if (ifTagParserRegex == null) {
					ifTagParserRegex = new Regex(
						@"<!--\s*\#(?<Tag>[iI][fF]|[eE][lL][sS][eE]|[eE][lL][sS][eE][iI][fF]|[eE][nN][dD][iI][fF])\s*(?<Expression>{{[-\w\s\.|]*}})?\s*-->",
						RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.ExplicitCapture
					);
				}
				return ifTagParserRegex;
			}
		}
	}
}
