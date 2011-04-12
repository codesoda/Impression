namespace CodeSoda.Impression.Tags
{
	/// <summary>
	/// An ImpressionTag represents a functional piece of markup within a template
	/// </summary>
	public class VarTagMarkup : TagMarkupBase {

		public VarTagMarkup(ExpressionMarkup expression, string markup, int lineNumber, int charPos)
			: base(expression, markup, lineNumber, charPos) {}

		internal override void Interpret(IInterpretContext ctx) {

			// lookup the object we are foreach-ing
			object obj = this.Expression.Evaluate(ctx);

			// create the variable by adding the expression value to the bag as "Within"
			ctx.Bag[this.Expression.Within] = obj;

			ctx.MoveNext();
		}
	}
}
