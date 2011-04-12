using System;
using CodeSoda.Impression.Filters;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests.Integration.Filters {

	[TestFixture]
	public class DefaultValueTests
	{

		private DateFilter _filter;
		private IPropertyBag _bag;
		private DateTime _date;
		private string[] _parameters;
		private IMarkupBase _markup;

		[SetUp]
		public void Setup() {
			_filter = new DateFilter();
			_bag = new PropertyBag();
			_date = DateTime.Now;
			_parameters = new [] { "" };
		}

		[Test]
		public void CheckKeyword() {
			Assert.AreEqual("date", _filter.Keyword);
		}

		private void TestFormat(string format)
		{
			_parameters = new[] { format };
			object result = _filter.Run(_date, _parameters, _bag, _markup);
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
