
using System.Collections;
using System.Collections.Generic;
using ArrayList=System.Collections.ArrayList;
using IEnumerator=System.Collections.IEnumerator;

namespace CodeSoda.Impression.Filters
{
	public class ShuffleFormatter: FilterBase
	{

		public override string Keyword
		{
			get { return "shuffle"; }
		}

		public override object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup)
		{
			if (parameters != null && parameters.Length > 0)
				throw new ImpressionInterpretException("Filter " + Keyword + " cannot be used with parameters.", markup);

			if (obj is string)
				return Shuffle(obj as string);

			//if (obj is IEnumerable && obj.GetType().IsGenericType)
			//{
				
			//}
			//    return Shuffle(obj);

			if (obj is IEnumerable)
				return Shuffle(obj as IEnumerable);

			if (obj is IEnumerable)
				return Shuffle(obj as IEnumerable);

			return Shuffle(obj);
		}


		#region Shuffle

		public static string Shuffle(string s)
		{
			return s;
		}

		// for cases when the parameter is null, or some other data type
		// basically jsut ignore
		public static object Shuffle(object obj)
		{
			return obj;
		}

		public static IEnumerable<T> Shuffle<T>(IEnumerable<T> enumerable)
		{

			if (enumerable == null)
				return null;

			List<T> list = new List<T>(enumerable);
			//list.Sort((a, b) => Guid.NewGuid().CompareTo(Guid.NewGuid()));
			if (list.Count <= 1)
			{
				return list; // nothing to do
			}

			for (int i = 0; i < list.Count; i++)
			{
				int newIndex = random.Next(0, list.Count);

				// swap the two elements over 
				T x = list[i];
				list[i] = list[newIndex];
				list[newIndex] = x;
			}

			return list;
		}

		public static IEnumerable Shuffle(System.Collections.IEnumerable enumerable)
		{

			if (enumerable == null)
				return null;

			ArrayList list = new ArrayList();
			IEnumerator en = enumerable.GetEnumerator();
			while (en.MoveNext())
				list.Add(en.Current);

			for (int i = 0; i < list.Count; i++)
			{
				int r = random.Next(0, list.Count);

				object tmp = list[i];
				list[i] = list[r];
				list[r] = tmp;
			}

			return list;
		}

		#endregion
	}
}
