
using NUnit.Framework;

namespace CodeSoda.Impression.Tests.Filters
{
	[TestFixture]
	public class UrlEncodeTests
	{

		[Test]
		public void TestUrlEncodeAcceptsNullandEmpty()
		{
			Assert.AreEqual(null, new UrlEncodeFilter().Run(null, null, null, null));
			Assert.AreEqual("", new UrlEncodeFilter().Run("", null, null, null));
		}

	}
}
