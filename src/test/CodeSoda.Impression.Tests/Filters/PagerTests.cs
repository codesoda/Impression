using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeSoda.Impression.Filters;
using Moq;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests.Filters
{
	[TestFixture]
	public class PagerTests: FilterTestBase<PagerFilter>
	{
		[SetUp]
		public override void Setup() {
			base.Setup();
		}

		[Test]
		public void ReturnsNullWhenObjectIsNull() {
			var obj = Filter.Run(null, new [] {"5"}, null, null);
			Assert.IsNull(obj);
		}

		[Test]
		public void ReturnsNullWhenObjectIsNotModelList()
		{
			var obj = Filter.Run("This is a string", null, null, null);
			Assert.IsNull(obj);

			obj = Filter.Run(5, null, null, null);
			Assert.IsNull(obj);
		}

		[Test]
		public void ReturnsHtml()
		{
			var list = new ModelListWithPages();
			list.Pages = new List<PageModel>(3) {
				new PageModel {Current = true, First = true, Last = false, Number = 1, Url = "?page=1"},
				new PageModel {Current = false, First = false, Last = false, Number = 2, Url = "?page=2"},
				new PageModel {Current = false, First = false, Last = true, Number = 3, Url = "?page=3"}
			};

			list.FirstPage = list.Pages[0];
			list.PrevPage = null;
			list.CurrentPage = list.Pages[0];
			list.NextPage = list.Pages[1];
			list.LastPage = list.Pages[2];

			var result = Filter.Run(list, null, null, null);

			Assert.IsNotNull(result as string);
		}



	}
}
