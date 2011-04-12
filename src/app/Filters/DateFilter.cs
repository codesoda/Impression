
using System;

namespace CodeSoda.Impression.Filters
{
	public class DateFilter: FilterBase {

		override public string Keyword {
			get { return "date"; }
		}

		override public object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup) {
			string dateFormat = null;

			if (parameters != null && parameters.Length > 1)
				throw new ImpressionInterpretException("Formatter " + Keyword + " does not accept more than one parameter.");

			if (parameters == null || parameters.Length == 0)
				dateFormat = ImpressionEngine.DateFormat;
			else
				dateFormat = IsLiteral(parameters[0])
					? GetLiteral(parameters[0])
					: bag[parameters[0]].ToString();

			if (obj != null) {
				if (obj is DateTime) {
					obj = ((DateTime)obj).ToString(dateFormat);
				}
				else {
					DateTime val;
					if (DateTime.TryParse(obj.ToString(), out val)) {
						obj = val.ToString(dateFormat);
					}
				}
			}
			return obj;
		}
	}

	public class DateTimeFilter: DateFilter {
		public override string Keyword {
			get {
				return "datetime";
			}
		}

		public override object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup) {
			return base.Run(
				obj,
				new [] {"'" + ImpressionEngine.DateTimeFormat + "'" },
				bag,
				markup
			);
		}

	}

	public class LongDateFormatter : DateFilter
	{
		public override string Keyword {
			get {
				return "datelong";
			}
		}

		public override object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup) {
			return base.Run(
				obj,
				parameters != null && parameters.Length == 0
					? parameters
					: new[] { ImpressionEngine.LongDateFormat },
				bag,
				markup
			);
		}

	}

	public class LongDateTimeFormatter : DateFilter
	{
		public override string Keyword {
			get {
				return "datetimelong";
			}
		}

		public override object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup)
		{
			return base.Run(
				obj,
				parameters != null && parameters.Length == 0
					? parameters
					: new[] { ImpressionEngine.LongDateTimeFormat },
				bag,
				markup
			);
		}

	}
}
