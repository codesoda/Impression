
using System.Collections.Generic;
using CodeSoda.Impression.Filters;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests.Filters
{
	[TestFixture]
	public class RandomTests
	{

		[Test]
		public void TestRandomWithSimpleType()
		{
			int[] nums = { 1, 2, 3, 4, 5 };
			int num = (int)new RandomFilter().Run(nums, null, null, null);
			Assert.IsTrue(num > 0 && num <= 5);
		}

		[Test]
		public void TestRandomWithComplexType()
		{
			IList<SimpleObject> list = new[] {
				new SimpleObject {Age = 1, Name = "A"},
				new SimpleObject {Age = 2, Name = "B"},
				new SimpleObject {Age = 3, Name = "C"},
				new SimpleObject {Age = 4, Name = "D"},
				new SimpleObject {Age = 5, Name = "E"},
			};

			SimpleObject item = (SimpleObject)new RandomFilter().Run(list, null, null, null);
			Assert.IsTrue(item.Age > 0 && item.Age <= 5);
		}

	}
}
