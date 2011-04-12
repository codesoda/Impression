
using System;
using CodeSoda.Impression.Filters;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests.Filters {

	[TestFixture]
	public class DefaultValueTests: FilterTestBase<DefaultFilter>
	{

		[SetUp]
		public override void Setup() {
			base.Setup();
		}

		[Test]
		[ExpectedException(typeof(ImpressionParseException))]
		public void ThrowsArgumentExceptionWhenNoParameters() {
			Filter.Run(null, null, Bag, Markup);
		}

		[Test]
		[ExpectedException(typeof(ImpressionParseException))]
		public void ThrowsArgumentExceptionWhen2Parameters()
		{
			Filter.Run(null, new [] {"p1", "p2"}, Bag, Markup);
		}

		[Test]
		public void IgnoresDefaultWhenThereIsaValue()
		{
			var result = Filter.Run(5, new[] { "p1" }, Bag, Markup);
			Assert.AreEqual(5, result);
		}

		[Test]
		public void UsesDefaultWhenThereIsNoValue()
		{
			var result = Filter.Run(null, new[] { "'p1'" }, Bag, Markup);
			Assert.AreEqual("p1", result);
		}

		[Test]
		public void UsesLiteralDefaultWhenThereIsNoValue()
		{
			var result = Filter.Run(null, new[] { "'p1'" }, Bag, Markup);
			Assert.AreEqual("p1", result);
		}

		[Test]
		public void UseBagValueWhenThereIsNoValue()
		{
			Bag.Add("p1", "bagvalue");
			var result = Filter.Run(null, new[] { "p1" }, Bag, Markup);
			Assert.AreEqual("bagvalue", result);
		}


	}

}
