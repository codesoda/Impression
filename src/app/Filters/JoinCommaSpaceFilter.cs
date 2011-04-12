
namespace CodeSoda.Impression.Filters
{
	public class JoinCommaSpaceFilter: JoinFilter
	{
		public override string Keyword {
			get { return "join_commaspace"; }
		}

		public override object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup) {
			if (parameters != null && parameters.Length > 0)
				throw new ImpressionInterpretException("Filter " + Keyword + " cannot be used with parameters.", markup);

			if (obj == null)
				return null;

			return Join(obj, ", ");
		}

	}
}
