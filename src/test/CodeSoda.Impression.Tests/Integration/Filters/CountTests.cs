using System;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests.Integration.Filters
{
	[TestFixture]
	public class CountTests: IntegrationTestBase
	{
		[SetUp]
		public override void Setup() {
			base.Setup();
		}

		[Test]
		public void TestNullReturns0()
		{
			Bag.Add("en", null);
			Engine = ImpressionEngine.Create(Bag);
			var result = Engine.RunString("{{en|count}}");
			Assert.AreEqual("0", result);
		}

		[Test]
		public void TestNotEnumerableReturns1()
		{
			Bag.Add("en", new DateTime());
			Engine = ImpressionEngine.Create(Bag);
			var result = Engine.RunString("{{en|count}}");
			Assert.AreEqual("1", result);

			Bag.Add("en", 5);
			Engine = ImpressionEngine.Create(Bag);
			result = Engine.RunString("{{en|count}}");
			Assert.AreEqual("1", result);
		}

		[Test]
		public void TestEmptyEnumerableReturns0()
		{
			Bag.Add("en", new int[0]);
			Engine = ImpressionEngine.Create(Bag);
			var result = Engine.RunString("{{en|count}}");
			Assert.AreEqual("0", result);
		}

		[Test]
		public void TestEnumerableWith1ItemReturns1()
		{
			Bag.Add("en", new [] { 1 });
			Engine = ImpressionEngine.Create(Bag);
			var result = Engine.RunString("{{en|count}}");
			Assert.AreEqual("1", result);
		}

		[Test]
		public void TestEnumerableWith5ItemsReturns5()
		{
			var obj = new [] { 1,2,3,4,5 };
			Bag.Add("en", obj);
			Engine = ImpressionEngine.Create(Bag);
			var result = Engine.RunString("{{en|count}}");
			Assert.AreEqual("5", result);
		}
	}
}
