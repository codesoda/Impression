using System;
using System.Collections.Generic;
using System.Text;

namespace CodeSoda.Impression
{
	class UpperCaseFilter: IFilter
	{

		public string Keyword {
			get { return "uppercase"; }
		}

		public object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup) {

			if (parameters != null && parameters.Length > 0)
				throw new ImpressionInterpretException("Formatter " + Keyword + " cannot be used with parameters.", markup);
			
			return (obj != null ? obj.ToString().ToUpper() : obj);
		}

	}
}
