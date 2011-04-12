
namespace CodeSoda.Impression
{
	public class ImpressionParseException : ImpressionExceptionBase
	{
		public ImpressionParseException(string message, string markup, int lineNumber, int charPos)
			: base(message, markup, lineNumber, charPos) { }

		public ImpressionParseException(string message, IMarkupBase markupBase) : base(message, markupBase) { }

		public ImpressionParseException(string message) : base(message) { }
	}
}
