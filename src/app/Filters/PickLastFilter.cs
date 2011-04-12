
using System.Collections;
using IEnumerable=System.Collections.IEnumerable;

namespace CodeSoda.Impression.Filters
{
	class PickLastFilter: IFilter
	{

		public string Keyword {
			get { return "pick_last"; }
		}

		public object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup)
		{
			if (parameters != null && parameters.Length > 0)
				throw new ImpressionInterpretException("Filter " + Keyword + " cannot be used with parameters.", markup);

			if (obj == null)
				return null;

			return Last(obj);
		}

		public static object Last(object obj)
		{
			if (obj != null)
			{
				if (obj.GetType().GetInterface("IEnumerable") != null)
				{
					IEnumerator en = ((IEnumerable)obj).GetEnumerator();
					object last = null;
					while (en.MoveNext()) last = en.Current;
					obj = last;
				}

				// if its not enumerable, then leave alone the last item
				// from something which isnt a list or array is itself
			}
			return obj;
		}

	}
}
