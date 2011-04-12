
using CodeSoda.Impression.Filters;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests.Filters
{
	[TestFixture]
	public class StripTests
	{
		[Test]
		public void Test_Striphtml()
		{
			string stripped = (string)new StripHtmlFilter().Run("  <html>This is<img src=\"some image.jpg\" /> html</html>  ", null, null, null);
			Assert.IsNotNull(stripped);
			Assert.AreEqual("  This is html  ", stripped);
		}

		[Test]
		public void Test_StripNewLines()
		{
			string stripped = (string)new StripNewLinesFilter().Run("  Line1\nLine2\rLine3\r\nLine4  ", null, null, null);
			Assert.IsNotNull(stripped);
			Assert.AreEqual("  Line1Line2Line3Line4  ", stripped);
		}

		[Test]
		public void Test_Htmlify()
		{
			string stripped = (string)new HtmlifyFilter().Run("  Line1\nLine2\rLine3\r\nLine4  \r\n\r\nThis is a second paragraph", null, null, null);
			Assert.IsNotNull(stripped);
			Assert.AreEqual("<p>Line1<br />Line2<br />Line3<br />Line4</p><p>This is a second paragraph</p>", stripped);
		}
	}
}
