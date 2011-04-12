
namespace CodeSoda.Impression.Filters
{
	public class EmptyTrueFormatter: FilterBase
	{
		public override string Keyword
		{
			get { return "empty_true"; }
		}

		public override object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup)
		{
			if (parameters != null && parameters.Length > 0)
				throw new ImpressionInterpretException("Filter " + Keyword + " cannot be used with parameters.", markup);

			bool hasValue = (obj is string && !string.IsNullOrEmpty((string)obj)) || (obj != null);

			if (!hasValue)
			{
				return true;
			}

			return obj;

		}

	}
}
