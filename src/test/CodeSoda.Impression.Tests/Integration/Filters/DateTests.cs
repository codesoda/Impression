
using System;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests.Integration.Filters
{
	[TestFixture]
	public class DateTests: IntegrationTestBase
	{

		private DateTime _now;

		[SetUp]
		public override void Setup()
		{
			base.Setup();
			_now = DateTime.Now;
			Bag.Add("now", _now);

			Engine = ImpressionEngine.Create(Bag);
		}

		[Test]
		public void IntegrationTestDateFilter() {
			var expected = _now.ToString("HH");
			var result = Engine.RunString("{{now|date('HH')}}");
			Assert.AreEqual(expected, result);
		}
	}
}
