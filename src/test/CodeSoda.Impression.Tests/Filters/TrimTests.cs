
using CodeSoda.Impression.Filters;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests.Filters
{
	[TestFixture]
	public class TrimTests
	{
		[Test]
		[ExpectedException("CodeSoda.Impression.ImpressionInterpretException")]
		public void TestTrimHatesParameters()
		{
			new TrimFilter().Run("some text", new[] { "P1" }, null, null);
		}

		[Test]
		public void testTrim()
		{
			string trimmed = (string)new TrimFilter().Run("   TRIM ME  ", null, null, null);
			Assert.IsNotNull(trimmed);
			Assert.AreEqual("TRIM ME", trimmed);
		}
	}
}
