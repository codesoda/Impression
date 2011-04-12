using System;
using System.Collections.Generic;
using System.Text;

namespace CodeSoda.Impression.Filters
{
	public class TrimFilter: IFilter
	{
		public string Keyword {
			get { return "trim"; }
		}

		public object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup) {

			if (parameters != null && parameters.Length > 0)
				throw new ImpressionInterpretException("Formatter " + Keyword + " cannot be used with parameters.", markup);

			return (obj != null ? obj.ToString().Trim() : obj);
		}
	}
}
