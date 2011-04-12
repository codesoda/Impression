
using System.Collections;
using IEnumerable=System.Collections.IEnumerable;

namespace CodeSoda.Impression.Filters {

	public class CountFilter: IFilter {

		public string Keyword {
			get { return "count"; }
		}

		public object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup) {
			if (parameters != null && parameters.Length > 0)
				throw new ImpressionInterpretException("Filter " + Keyword + " cannot be used with parameters.", markup);

			return Count(obj);
		}

		public static object Count(object obj) {

			if (obj != null) {

				if (obj.GetType().GetInterface("IEnumerable") != null) {
					IEnumerator en = ((IEnumerable)obj).GetEnumerator();
					int count = 0;
					while (en.MoveNext()) count++;
					obj = count;
				} else
					// if its not enumerable, but its not null, then return 1;
					obj = 1;
			}
			else {
				// if the object is null then return 0
				obj = 0;
			}

			return obj;
		}

	}
}
