
namespace CodeSoda.Impression.Filters
{
	public class NotFilter: FilterBase
	{
		public override string Keyword
		{
			get { return "not"; }
		}

		public override object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup)
		{
			if (parameters != null && parameters.Length > 0)
				throw new ImpressionInterpretException("Filter " + Keyword + " cannot be used with parameters.", markup);

			return !AsBoolean(obj);

		}

	}
}
