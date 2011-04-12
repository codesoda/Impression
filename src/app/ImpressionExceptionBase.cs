using System;

namespace CodeSoda.Impression
{

	public abstract class ImpressionExceptionBase: ApplicationException {
	   public int LineNumber;
	   public int CharPos;
	   public string Markup;

		protected ImpressionExceptionBase(string message, IMarkupBase markupBase)
			: this(
				message,
				markupBase != null ? markupBase.Markup : null,
				markupBase != null ? markupBase.LineNumber : -1,
			markupBase != null ? markupBase.CharPos : -1) {
			
		}

		protected ImpressionExceptionBase(string message, string markup, int lineNumber, int charPos)
			: base(
				string.IsNullOrEmpty(markup) ?
					(lineNumber > -1 && charPos > -1)
						? string.Format(
								"{0}, {1} ( Line: {2}, Char: {3} )",
								message,
								markup,
								lineNumber,
								charPos
							)
						: string.Format("{0}, {1}", message, markup)
					: message) {}

		protected ImpressionExceptionBase(string message)
			: base(message) { }

	}

}
