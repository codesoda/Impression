
namespace CodeSoda.Impression
{
	public class ImpressionInterpretException : ImpressionExceptionBase
	{
		public ImpressionInterpretException(string message, string markup, int lineNumber, int charPos)
			: base(message, markup, lineNumber, charPos) { }

		public ImpressionInterpretException(string message, IMarkupBase markupBase) : base(message, markupBase) { }

		public ImpressionInterpretException(string message) : base(message) { }
	}
}
