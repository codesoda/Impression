using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CodeSoda.Impression
{
	public interface IFilter
	{
		string Keyword { get; }
		object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup);
	}

	public abstract class FilterBase: IFilter
	{
		protected static readonly Random random = new Random();

		public abstract string Keyword { get; }
		public abstract object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup);

		protected bool AsBoolean(object obj)
		{
			if (obj == null)
				return false;

			bool retVal = false; // todo check if there is some kinda default value specified in attributes

			// bool
			if (obj is bool)
			{
				retVal = (bool)obj;
			}

			// strings
			else if (obj is string)
			{
				if (!Boolean.TryParse((string)obj, out retVal))
				{
					double tempNumber = 0;
					if (!Double.TryParse((string)obj, out tempNumber))
					{
						retVal = !string.IsNullOrEmpty((string)obj);
					}
					else
					{
						retVal = tempNumber > 0;
					}
				}
			}

			// numbers
			else if (obj is int)
			{
				retVal = (int)obj > 0;
			}
			else if (obj is double)
			{
				retVal = (double)obj > 0;
			}
			else if (obj is float)
			{
				retVal = (float)obj > 0;
			}
			else if (obj is decimal)
			{
				retVal = (decimal)obj > 0;
			}

			// arrays, lists, etc
			else if (obj is IEnumerable)
			{
				retVal = ((IEnumerable)obj).GetEnumerator().MoveNext();
			}

			// convert using dot net
			else if (obj.GetType().GetInterface("IConvertible") != null)
			{
				//try {
				retVal = Convert.ToBoolean(obj);
				//}catch {}
			}

			// catch all, anything not null is true
			else
			{
				retVal = obj != null;
			}
			return retVal;
		}

		protected static bool IsLiteral(string value)
		{
			return
				!string.IsNullOrEmpty(value)
				&& (
					(value.StartsWith("\"") && value.EndsWith("\""))
					|| (value.StartsWith("'") && value.EndsWith("'"))
				);
		}

		protected static string GetLiteral(string value)
		{
			return GetLiteral(value, false);
		}

		protected static string GetLiteral(string value, bool forceLiteral)
		{
			if (forceLiteral || IsLiteral(value))
			{
				return value.Trim('\'', '\"');
			}
			else
				return null;
		}

	}
}
