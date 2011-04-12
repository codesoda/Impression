
using System.Collections;
using System.Collections.Generic;
using CodeSoda.Impression.Filters;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests.Filters
{
	[TestFixture]
	public class PickTests
	{

		[Test]
		public void TestPickWithComplexType()
		{

			IList<SimpleObject> list = new[] {
				new SimpleObject {Age = 1, Name = "A"},
				new SimpleObject {Age = 2, Name = "B"},
				new SimpleObject {Age = 3, Name = "C"},
				new SimpleObject {Age = 4, Name = "D"},
				new SimpleObject {Age = 5, Name = "E"},
			};

			IEnumerable newEn = (IEnumerable)new PickFilter().Run(list, new[] { "2" }, null, null);

			IList<SimpleObject> newlist = new List<SimpleObject>();
			foreach (SimpleObject obj in newEn)
			{
				newlist.Add(obj);
			}

			Assert.IsNotNull(newlist);
			Assert.AreEqual(2, newlist.Count);
			Assert.AreEqual("A", newlist[0].Name);
			Assert.AreEqual("B", newlist[1].Name);
		}

		[Test]
		public void TestPickWithSimpleType()
		{

			ArrayList list = new ArrayList() { 1, 2, 3, 4, 5 };
			ArrayList newlist = (ArrayList)new PickFilter().Run(list, new[] { "2" }, null, null);

			Assert.IsNotNull(newlist);
			Assert.AreEqual(2, newlist.Count);
			Assert.AreEqual(1, newlist[0]);
			Assert.AreEqual(2, newlist[1]);
		}

	}
}
