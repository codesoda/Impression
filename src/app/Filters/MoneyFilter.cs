using System;

namespace CodeSoda.Impression.Filters
{
	public class MoneyFilter: IFilter
	{

		public string Keyword
		{
			get { return "money"; }
		}

		public object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup)
		{

			if (parameters != null && parameters.Length > 1)
				throw new ImpressionInterpretException("Filter " + Keyword + " does not accept parameters.");

			if (obj == null)
			{
				obj = 0.0;
			}

			double val = 0;
			if (Double.TryParse(obj.ToString(), out val))
			{
				string moneyFormat = ImpressionEngine.MoneyFormat;

				string symbol = bag != null ? bag["Money.Symbol"] as string : null;
				obj = (symbol ?? "") + val.ToString(moneyFormat);
			}

			return obj;
		}

	}

	public class MoneyWithCurrencyFilter : IFilter
	{

		public string Keyword {
			get { return "money_with_currency"; }
		}

		public object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup) {

			if (parameters != null && parameters.Length > 1)
				throw new ImpressionInterpretException("Filter " + Keyword + " does not accept parameters.");

			if (obj == null) {
				obj = 0.0;
			}

			double val = 0;
			if (Double.TryParse(obj.ToString(), out val)) {

				string moneyFormat = ImpressionEngine.MoneyFormat;

				string symbol = bag != null ? bag["Money.Symbol"] as string : null;
				string currency = bag != null ? bag["Money.Currency"] as string: null;
				obj = (symbol ?? "") + val.ToString(moneyFormat) + (!string.IsNullOrEmpty(currency) ? " " + currency : "");
			}

			return obj;
		}

	}

	public class MoneyWithoutCurrencyFilter : IFilter
	{

		public string Keyword {
			get { return "money_without_currency"; }
		}

		public object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup) {

			if (parameters != null && parameters.Length > 1)
				throw new ImpressionInterpretException("Filter " + Keyword + " does not accept parameters.");

			if (obj == null) {
				obj = 0.0;
			}

			double val = 0;
			if (Double.TryParse(obj.ToString(), out val)) {
				string moneyFormat = ImpressionEngine.MoneyFormat;
				obj = val.ToString(moneyFormat);
			}

			return obj;
		}

	}

	public class MoneyDollars : IFilter
	{

		public string Keyword
		{
			get { return "money_dollars"; }
		}

		public object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup)
		{

			if (parameters != null && parameters.Length > 1)
				throw new ImpressionInterpretException("Filter " + Keyword + " does not accept parameters.");

			if (obj == null)
			{
				obj = 0.0;
			}

			double val = 0;
			string valString = obj.ToString();
			if (Double.TryParse(valString, out val))
			{
				int pointIndex = valString.IndexOf('.');
				obj = pointIndex >= 0 ? valString.Substring(0, pointIndex) : valString;
			}

			return obj;
		}

	}

	public class MoneyCents : IFilter
	{

		public string Keyword
		{
			get { return "money_cents"; }
		}

		public object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup)
		{

			if (parameters != null && parameters.Length > 1)
				throw new ImpressionInterpretException("Filter " + Keyword + " does not accept parameters.");

			if (obj == null)
			{
				obj = 0.0;
			}

			double val = 0;
			string valString = obj.ToString();
			if (Double.TryParse(valString, out val))
			{
				int pointIndex = valString.IndexOf('.');
				if (pointIndex >= 0)
				{
					string subs = valString.Substring(pointIndex + 1, valString.Length - (pointIndex + 1));
					if (subs.Length > 2)
					{
						obj = subs.Substring(0, 2);
					} else
					{
						obj = subs.PadRight(2, '0');
					}
				} else
				{
					obj = "00";
				}
			}

			return obj;
		}

	}
}
