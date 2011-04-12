
using System.Collections;
using System.Collections.Generic;

using ArrayList=System.Collections.ArrayList;
using IEnumerator=System.Collections.IEnumerator;

namespace CodeSoda.Impression.Filters
{
	public class RandomFilter: FilterBase
	{

		public override string Keyword {
			get { return "random"; }
		}

		public override object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup) {
			
			if (parameters != null && parameters.Length > 0)
				throw new ImpressionInterpretException("Filter " + Keyword + " cannot be used with parameters.", markup);

			if (obj == null)
				return null;

			if (obj is string)
				return obj;

			if (obj is IEnumerable)
				return Random((IEnumerable) obj);

			return Random(obj);
		}

		public static object Random(object obj)
		{
			return obj;
		}

		public static object Random(IEnumerable obj)
		{
			if (obj == null)
				return null;

			ArrayList list = new ArrayList();
			IEnumerator en = obj.GetEnumerator();
			while (en.MoveNext())
				list.Add(en.Current);

			return list.Count == 0
					? null
					: (list.Count == 1
						? list[0]
						: list[random.Next(0, list.Count)]);
		}

		public static T Random<T>(IEnumerable<T> obj)
		{
			if (obj == null)
				return default(T);

			List<T> list = new List<T>(obj);
			//IEnumerator en = obj.GetEnumerator();
			//while (en.MoveNext())
			//    list.Add((T)en.Current);

			return list.Count == 0
					? default(T)
					: (list.Count == 1
						? list[0]
						: list[random.Next(0, list.Count)]);
		}
	}
}
