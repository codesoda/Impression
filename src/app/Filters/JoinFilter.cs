
using System.Collections;
using System.Collections.Generic;
using IEnumerator=System.Collections.IEnumerator;

namespace CodeSoda.Impression.Filters
{
	public class JoinFilter: FilterBase
	{
		public override string Keyword {
			get { return "join"; }
		}

		public override object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup) {
			if (parameters == null || parameters.Length != 1)
				throw new ImpressionInterpretException("Filter " + Keyword + " expects one parameter.");

			if (obj == null)
				return null;

			return Join(obj, parameters[0]);
		}

		protected static object Join(object obj, string seperator)
		{
			if (obj != null && obj.GetType().GetInterface("IEnumerable") != null)
			{
				// if its not a string array
				if (obj.GetType() != typeof(string[]))
				{
					List<string> ar = new List<string>();
					IEnumerator en = ((IEnumerable)obj).GetEnumerator();
					while (en.MoveNext())
						ar.Add(en.Current.ToString());
					obj = ar.ToArray();
				}
				obj = string.Join(seperator, (string[])obj);
			}
			return obj;
		}
	}
}
