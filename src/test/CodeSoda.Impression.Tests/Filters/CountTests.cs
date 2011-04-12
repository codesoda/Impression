using System;
using CodeSoda.Impression.Filters;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests.Filters
{
	[TestFixture]
	public class CountTests: FilterTestBase<CountFilter>
	{
		[SetUp]
		public override void Setup() {
			base.Setup();
		}

		[Test]
		public void TestNullReturns0()
		{
			object result = Filter.Run(null, Parameters, Bag, Markup);
			Assert.AreEqual(0, result);
		}

		[Test]
		public void TestNotEnumerableReturns1()
		{
			object result = Filter.Run(new DateTime(), Parameters, Bag, Markup);
			Assert.AreEqual(1, result);

			result = Filter.Run(5, Parameters, Bag, Markup);
			Assert.AreEqual(1, result);
		}

		[Test]
		public void TestEmptyEnumerableReturns0()
		{
			var obj = new int[] {};
			object result = Filter.Run(obj, Parameters, Bag, Markup);
			Assert.AreEqual(0, result);
		}

		[Test]
		public void TestEnumerableWith1ItemReturns1()
		{
			var obj = new [] { 1 };
			object result = Filter.Run(obj, Parameters, Bag, Markup);
			Assert.AreEqual(1, result);
		}

		[Test]
		public void TestEnumerableWith5ItemsReturns5()
		{
			var obj = new [] { 1,2,3,4,5 };
			object result = Filter.Run(obj, Parameters, Bag, Markup);
			Assert.AreEqual(5, result);
		}
	}
}
