
using CodeSoda.Impression.Filters;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests.Filters
{
	[TestFixture]
	public class IFTests
	{

		[Test]
		public void TestIIFWithBag()
		{
			PropertyBag bag = new PropertyBag();
			bag.Add("Val1", 1);
			bag.Add("Val2", 2);

			object result = new IFFilter().Run(true, new[] { "Val1", "Val2" }, bag, null);
			Assert.AreEqual(result, 1);

			result = new IFFilter().Run(false, new[] { "Val1", "Val2" }, bag, null);
			Assert.AreEqual(result, 2);

		}

		[Test]
		public void TestIIFWithLiterals()
		{

			object result = new IFFilter().Run(true, new[] { "\"Val1\"", "\"Val2\"" }, null, null);
			Assert.AreEqual(result, "Val1");

			result = new IFFilter().Run(false, new[] { "\"Val1\"", "\"Val2\"" }, null, null);
			Assert.AreEqual(result, "Val2");

		}

	}
}
