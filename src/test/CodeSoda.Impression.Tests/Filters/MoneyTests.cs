
using CodeSoda.Impression.Filters;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests.Filters
{
	[TestFixture]
	public class MoneyTests
	{
		[Test]
		public void test_money()
		{

			PropertyBag bag = new PropertyBag();
			bag.Add("Money.Symbol", "$");
			bag.Add("Money.Currency", "AUD");

			string s = new MoneyFilter().Run(1.7568, null, null, null) as string;
			Assert.AreEqual("1.76", s);
			s = new MoneyFilter().Run(1.7435, null, null, null) as string;
			Assert.AreEqual("1.74", s);
			s = new MoneyFilter().Run(1.7568, null, bag, null) as string;
			Assert.AreEqual("$1.76", s);
		}

		[Test]
		public void test_money_without_currency()
		{

			PropertyBag bag = new PropertyBag();
			bag.Add("Money.Symbol", "$");
			bag.Add("Money.Currency", "AUD");

			string s = new MoneyWithoutCurrencyFilter().Run(1.7568, null, bag, null) as string;
			Assert.AreEqual("1.76", s);
		}

		[Test]
		public void test_money_with_currency()
		{

			PropertyBag bag = new PropertyBag();
			bag.Add("Money.Symbol", "$");
			bag.Add("Money.Currency", "AUD");

			string s = new MoneyWithCurrencyFilter().Run(1.7568, null, bag, null) as string;
			Assert.AreEqual("$1.76 AUD", s);
		}

		[Test]
		public void test_money_dollars()
		{
			string s = new MoneyDollars().Run(1.7568, null, null, null) as string;
			Assert.AreEqual("1", s);
			s = new MoneyDollars().Run(0.7568, null, null, null) as string;
			Assert.AreEqual("0", s);
			s = new MoneyDollars().Run(124.7568, null, null, null) as string;
			Assert.AreEqual("124", s);
		}

		[Test]
		public void test_money_cents()
		{
			string s = new MoneyCents().Run(1.01, null, null, null) as string;
			Assert.AreEqual("01", s);
			s = new MoneyCents().Run(0.7568, null, null, null) as string;
			Assert.AreEqual("75", s);
			s = new MoneyCents().Run(124.5, null, null, null) as string;
			Assert.AreEqual("50", s);
			s = new MoneyCents().Run(124, null, null, null) as string;
			Assert.AreEqual("00", s);
		}
	}
}
