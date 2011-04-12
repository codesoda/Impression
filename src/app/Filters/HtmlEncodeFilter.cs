using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace CodeSoda.Impression
{
	public class HtmlEncodeFilter: IFilter
	{

		public string Keyword
		{
			get { return "html_encode"; }
		}

		public object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup)
		{
			if (parameters != null && parameters.Length > 0)
				throw new ImpressionInterpretException("Formatter " + Keyword + " cannot be used with parameters.", markup);

			// make sure the object isn't null
			if (obj != null)
				obj = HttpUtility.HtmlEncode(obj.ToString());

			return obj;
		}

	}
}
