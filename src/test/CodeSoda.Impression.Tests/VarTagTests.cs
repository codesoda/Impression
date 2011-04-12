using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests
{
	[TestFixture]
	public class VarTagTests
	{

		[Test]
		public void CanParseVarTags() {
			IPropertyBag bag = new PropertyBag();
			bag.Add("list", new [] { "Tom", "Dick", "Harry"});
			ImpressionEngine ie = ImpressionEngine.Create(bag);
			var result = ie.RunString("<!-- #VAR {{ list2 = list }} -->{{list2}}");
			Assert.AreEqual(bag["list"].GetType().ToString(), result);
		}

	}
}
