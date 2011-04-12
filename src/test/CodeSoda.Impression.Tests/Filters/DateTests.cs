
using System;
using CodeSoda.Impression.Filters;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests.Filters {

	[TestFixture]
	public class DateTests: FilterTestBase<DateFilter>
	{
		private DateTime _date;

		[SetUp]
		override public void Setup()
		{
			base.Setup();
			_date = DateTime.Now;
		}

		[Test]
		public void DateFilter_AcceptsNullsAndEmptyAndInvalidaDates()
		{
			Assert.AreEqual(null, new DateFilter().Run(null, null, null, null));
			Assert.AreEqual("", new DateFilter().Run("", null, null, null));
			Assert.AreEqual("Tom Jones", new DateFilter().Run("Tom Jones", null, null, null));
		}

		[Test]
		public void DateFilter_FormatsCorrectly()
		{
			DateTime now = DateTime.Now;
			Assert.AreEqual(now.ToString("dd-MMM-yyyy"), new DateFilter().Run(now, new[] { "'dd-MMM-yyyy'" }, null, null));
			Assert.AreEqual(now.ToString("d"), new DateFilter().Run(now, new[] { "'d'" }, null, null));
			Assert.AreEqual(now.ToString("MMMM"), new DateFilter().Run(now, new[] { "'MMMM'" }, null, null));
			Assert.AreEqual(now.ToString("yyyy"), new DateFilter().Run(now, new[] { "'yyyy'" }, null, null));
		}

		[Test]
		public void CheckKeyword() {
			Assert.AreEqual("date", Filter.Keyword);
		}

		private void TestFormat(string format)
		{
			Parameters = new[] { format };
			object result = Filter.Run(_date, Parameters, Bag, Markup);
			Assert.AreEqual(_date.ToString(format.Replace("'", "")), result.ToString());
		}

		[Test]
		public void CheckDateFormatsddMMMyyyyCorrectly() {
			TestFormat("'dd-MMM-yyyy'");
		}

		[Test]
		public void CheckDateFormatsddCorrectly() {
			TestFormat("'dd'");
		}

		[Test]
		public void CheckDateFormatsMMMCorrectly()
		{
			TestFormat("'MMM'");
		}

		[Test]
		public void CheckDateFormatsyyyyCorrectly()
		{
			TestFormat("'yyyy'");
		}

		[Test]
		public void CheckDateFormatshhCorrectly()
		{
			TestFormat("'hh'");
		}

		[Test]
		public void CheckDateFormatsmmCorrectly()
		{
			TestFormat("'mm'");
		}

		[Test]
		public void CheckDateFormatsttCorrectly()
		{
			TestFormat("'tt'");
		}


	}

}
