
namespace CodeSoda.Impression
{
	public interface IMarkupBase
	{

		/// <summary>
		/// The string that represents the raw markup for the literal
		/// </summary>
		string Markup { get; }

		int LineNumber { get; }
		int CharPos { get; }
	}

	public abstract class MarkupBase : IMarkupBase
	{

		#region Properties


		/// <summary>
		/// Indicates the type of tag
		/// </summary>
		public abstract MarkupType MarkupType { get; }


		/// <summary>
		/// The string that represents the raw markup for the literal
		/// </summary>
		public string Markup { get; protected set; }

		public int LineNumber { get; set; }
		public int CharPos { get; set; }

		#endregion

		#region Constructors

		protected MarkupBase(string markup, int lineNumber, int charPos) {
			this.Markup = markup;
			this.LineNumber = lineNumber;
			this.CharPos = charPos;
		}

		#endregion

		#region Methods

		internal abstract void Interpret(IInterpretContext ctx);

		public new string ToString() {
			return this.Markup;
		}

		#endregion

	}
}
