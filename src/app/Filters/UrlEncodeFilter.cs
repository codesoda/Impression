using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace CodeSoda.Impression
{
	public class UrlEncodeFilter: IFilter
	{

		public string Keyword
		{
			get { return "url_encode"; }
		}

		public object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup)
		{
			if (parameters != null && parameters.Length > 0)
				throw new ImpressionInterpretException("Formatter " + Keyword + " cannot be used with parameters.", markup);

			// make sure the obj is not null
			if (obj != null)
				obj = HttpUtility.UrlEncode(obj.ToString());

			return obj;
		}

	}
}
