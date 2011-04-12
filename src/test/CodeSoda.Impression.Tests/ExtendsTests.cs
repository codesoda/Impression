
using System;
using System.IO;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests
{
	[TestFixture]
	public class ExtendsTests
	{
		[Test]
		public void TestBasicExtends() {
			string currentFolder = Path.GetDirectoryName(Environment.CurrentDirectory);
			string templatePath = Path.Combine(currentFolder, "../templates/extends-content.htm");
			ImpressionEngine ie = ImpressionEngine.Create(templatePath);
			string result = ie.Run();
			Assert.AreEqual("\r\nEXTENDED\r\n\r\nLayout Content\r\n\r\n\t<h3>EXTENDED</h3>\r\n\r\nMore Layout Content\r\n\r\n<div id=\"footer\">Layout Footer</div>\r\n\r\n", result);
		}

	}
}