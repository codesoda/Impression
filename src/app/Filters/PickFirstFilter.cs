
using System.Collections;
using IEnumerable=System.Collections.IEnumerable;

namespace CodeSoda.Impression.Filters
{
	class PickFirstFilter: IFilter
	{

		public string Keyword {
			get { return "pick_first"; }
		}

		public object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup)
		{
			if (parameters != null && parameters.Length > 0)
				throw new ImpressionInterpretException("Filter " + Keyword + " cannot be used with parameters.", markup);

			if (obj == null)
				return null;

			return First(obj);
		}

		public static object First(object obj)
		{
			if (obj != null)
			{
				if (obj.GetType().GetInterface("IEnumerable") != null)
				{
					IEnumerator en = ((IEnumerable)obj).GetEnumerator();
					obj = en.MoveNext() ? en.Current : null;
				}

				// if its not enumerable, then leave alone the first item
				// from something which isnt a list or array is itself
			}
			return obj;
		}

	}
}
