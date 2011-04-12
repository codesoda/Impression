
namespace CodeSoda.Impression.Filters
{
	class LowerCaseFilter: IFilter
	{

		public string Keyword {
			get { return "lowercase"; }
		}

		public object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup) {

			if (parameters != null && parameters.Length > 0)
				throw new ImpressionInterpretException("Formatter " + Keyword + " cannot be used with parameters.", markup);
			
			return (obj != null ? obj.ToString().ToLower() : obj);
		}

	}
}
