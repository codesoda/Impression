using System;
using System.Collections;
using System.Collections.Generic;
using ArrayList=System.Collections.ArrayList;
using IEnumerator=System.Collections.IEnumerator;

namespace CodeSoda.Impression.Filters
{
	public class PickFilter: FilterBase
	{

		public override string Keyword {
			get { return "pick"; }
		}

		public override object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup) {
			
			int number;
			
			if (parameters == null || parameters.Length != 1 || !Int32.TryParse(GetLiteral(parameters[0], true), out number))
				throw new ImpressionInterpretException("Filter " + Keyword + " expects one number parameter.");

			if (obj == null) return null;

			if (obj is String)
				return obj;

			if (obj is IEnumerable)
				return Pick(obj as IEnumerable, number);

			return Pick(obj, number);
		}

		#region Pick (take first X elements from a list)

		private static object Pick(object obj, int number)
		{
			return obj;
		}

		private static IEnumerable Pick(IEnumerable obj, int number)
		{

			ArrayList list = new ArrayList();
			System.Collections.IEnumerator en = obj.GetEnumerator();
			for (int i = 0; i < number; i++)
			{
				if (en.MoveNext())
					list.Add(en.Current);
			}

			return list;
		}

		private static IEnumerable<T> Pick<T>(IEnumerable<T> obj, int number)
		{

			List<T> list = new List<T>();
			IEnumerator en = ((IEnumerable)obj).GetEnumerator();
			for (int i = 0; i < number; i++)
			{
				if (en.MoveNext())
					list.Add((T)en.Current);
			}

			return list;
		}

		#endregion

	}
}
