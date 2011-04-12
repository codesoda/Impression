
using System;
using CodeSoda.Impression.Filters;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests.Filters {

	[TestFixture]
	public class EmptyFalseTests: FilterTestBase<EmptyFalseFilter>
	{

		[SetUp]
		public override void Setup() {
			base.Setup();
		}

		[Test]
		[ExpectedException(typeof(ImpressionInterpretException))]
		public void ThrowsArgumentExceptionWhenAnyParameters()
		{
			Parameters = new[] {"p1", "p2"};
			Filter.Run(null, Parameters, Bag, Markup);
		}

		[Test]
		public void ReturnsFalseForNull()
		{
			var result = Filter.Run(null, null, Bag, Markup);
			Assert.IsFalse((bool)result);
		}

		[Test]
		public void ReturnsTrueForIEnumerableWithItems()
		{
			var result = Filter.Run(new[] { "p1" }, null, Bag, Markup);
			Assert.IsTrue((bool)result);
		}

		[Test]
		public void ReturnsFalseForIEnumerableWithoutItems()
		{
			var result = Filter.Run(new int[0],null, Bag, Markup);
			Assert.IsFalse((bool)result);
		}

		[Test]
		public void ReturnsTrueWhenThereIsaValue()
		{
			var result = Filter.Run(5, null, Bag, Markup);
			Assert.IsTrue((bool)result);
		}

		[Test]
		public void ReturnsFalseForEmptyString()
		{
			var result = Filter.Run("", null, Bag, Markup);
			Assert.IsFalse((bool)result);
		}

		[Test]
		public void ReturnsTrueForNonEmptyString()
		{
			var result = Filter.Run("This is a string", null, Bag, Markup);
			Assert.IsTrue((bool)result);
		}
	}

}
