
namespace CodeSoda.Impression.Filters
{
	public class DefaultFilter: FilterBase
	{

		public override string Keyword
		{
			get { return "default"; }
		}

		public override object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup)
		{
			if (parameters == null || parameters.Length != 1)
			{
				throw new ImpressionParseException("default expects 1 parameter", markup);
			}

			bool hasValue = (obj is string && !string.IsNullOrEmpty((string)obj)) || (obj != null);

			return !hasValue
				? (IsLiteral(parameters[0])
					? GetLiteral(parameters[0])
					: bag[parameters[0]])
				: obj;
		}

	}
}
