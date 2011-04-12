
using CodeSoda.Impression.Filters;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests.Filters
{
	public class HtmlFilters
	{

		[Test]
		public void TestHtmlSelectOptions()
		{
			PropertyBag bag = new PropertyBag();
			bag.Add("SelectedValue", "Value2");

			SelectListItem[] items = new[] {
				new SelectListItem { Text = "11", Value = "Value1"},
				new SelectListItem { Text = "6", Value = "6"},
				new SelectListItem { Text = "12", Value = "Value3"}
			};

			string optionHtml = new HtmlOptionsFilter().Run(items, new[] { "Text", "Value", "SelectedValue.Length" }, bag, null) as string;

			int i = 0;
		}

		[Test]
		public void TestHtmlCheckboxChecked()
		{
			Assert.AreEqual("", new HtmlCheckedFilter().Run(false, null, null, null));
			Assert.AreEqual("checked=\"checked\"", new HtmlCheckedFilter().Run(true, null, null, null));
		}

		[Test]
		public void TestHtmlEncodeAcceptsNullandEmpty()
		{
			Assert.AreEqual(null, new HtmlEncodeFilter().Run(null, null, null, null));
			Assert.AreEqual("", new HtmlEncodeFilter().Run("", null, null, null));
		}

	}
}
