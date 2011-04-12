using System;
using System.Collections.Generic;
using System.Text;
using CodeSoda.Impression.Tests.Filters;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests
{
	[TestFixture]
	public class ForEachTagTests
	{

		[Test]
		public void TestForEachWithNoItems()
		{
			PropertyBag bag = new PropertyBag();
			bag["y"] = new int[0];
			string result = ImpressionEngine.Create(bag).RunString("<!-- #FOREACH {{ x in y }} -->{{x.Position}}<!-- #NEXT -->");
		}

		[Test]
		public void TestNestedForLoops()
		{
			PropertyBag bag = new PropertyBag();
			var items = new List<SimpleObject> {
				new SimpleObject {Name = "Item 1"},
				new SimpleObject {
					Name = "Item 2",
					Children = new List<SimpleObject> {
						new SimpleObject {Name = "Item 2-1"},
						new SimpleObject {Name = "Item 2-2"}
					}
				}
			};
			bag["Items"] = items;
			string result = ImpressionEngine.Create(bag).RunString(
				"<!-- #FOREACH {{ item in items }} -->{{item.Name}}<!-- #FOREACH {{ subitem in item.children }} -->{{subitem.name}}<!-- #NEXT --><!-- #NEXT -->"
			);

			Assert.IsNotNull(result);
			Assert.AreEqual("Item 1Item 2Item 2-1Item 2-2", result);
		}
	}
}
