
namespace CodeSoda.Impression.Filters
{
	public class IFFilter: FilterBase
	{
		public override string Keyword
		{
			get { return "if"; }
		}

		public override object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup)
		{

			if (parameters == null || parameters.Length != 2)
			{
				throw new ImpressionInterpretException("Filter " + Keyword + " expects 2 parameters", markup);
			}

			bool isTrue = AsBoolean(obj);

			int index = isTrue ? 0 : 1;

			if (IsLiteral(parameters[index]))
			{
				obj = GetLiteral(parameters[index]);
			}
			else
			{
				// lookup property on the bag
				obj = bag[parameters[index]];
			}

			return obj;

		}
	}
}
