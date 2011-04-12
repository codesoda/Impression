using System;
using System.Collections;
using System.Linq;
using CodeSoda.Impression.Filters;
using Moq;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests.Filters
{
	[TestFixture]
	public class PagedTests: FilterTestBase<PagedFilter>
	{
		[SetUp]
		public override void Setup() {
			base.Setup();
		}

		[Test]
		[ExpectedException(typeof(ImpressionInterpretException))]
		public void ThrowsExceptionWhenPassedNullParameters() {
			object result = Filter.Run(null, null, null, null);
		}

		[Test]
		public void ReturnsNullWhenObjectIsNull() {
			var obj = Filter.Run(null, new [] {"5"}, null, null);
			Assert.IsNull(obj);
		}

		[Test]
		public void ReturnsNullWhenObjectIsNotEnumerable()
		{
			var obj = Filter.Run(5, new[] { "5" }, null, null);
			Assert.IsNull(obj);
		}

		[Test]
		public void LooksUpPageNumberFromBag() {
			var propertyBagMock = new Mock<IPropertyBag>();
			propertyBagMock
				.SetupGet( x=> x["Page.Number"] )
				.Returns("1")
				.Verifiable();

			object result = Filter.Run(new string[10], new string[] {"5"}, propertyBagMock.Object, null);

			propertyBagMock.Verify();
		}

		[Test]
		public void LooksUpPageSizeFromBag() {
			var propertyBagMock = new Mock<IPropertyBag>();
			propertyBagMock
				.SetupGet(x => x["Page.Size"])
				.Returns("3")
				.Verifiable();

			var result = (ModelListWithPages)Filter.Run(new string[10], new string[] { "5" }, propertyBagMock.Object, null);

			propertyBagMock.Verify();
		}

		[Test]
		public void ReturnsPagedData() {
			var propertyBagMock = new Mock<IPropertyBag>();
			//propertyBagMock
			//    .SetupGet(x => x["Page.Size"])
			//    .Returns("3");

			//propertyBagMock
			//    .SetupGet(x => x["Page.Number"])
			//    .Returns("4");

			int[] ints = new int[] { 1,2,3,4,5,6,7,8,9,10,11,12 };

			object result = Filter.Run(ints, new string[] { "3" }, propertyBagMock.Object, null);

			var enumerable = result as IEnumerable;
			Assert.IsNotNull(enumerable);
		}

		[Test]
		public void ReturnsPagedDataForPage2()
		{
			var propertyBagMock = new Mock<IPropertyBag>();
			//propertyBagMock
			//    .SetupGet(x => x["Page.Size"])
			//    .Returns("3");

			propertyBagMock
				.SetupGet(x => x["Page.Number"])
				.Returns("2");

			int[] ints = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

			var result = Filter.Run(ints, new string[] { "3" }, propertyBagMock.Object, null) as ModelListWithPages;

			Assert.IsNotNull(result);

			int i = 0;
			foreach(var item in result) {
				Assert.AreEqual(ints[3 + i], item);
				i++;
			}
		}

		[Test]
		public void HandlesPagingWhenListIsSmallerThanPageSize()
		{
			var propertyBagMock = new Mock<IPropertyBag>();
			propertyBagMock
			    .SetupGet(x => x["Page.Size"])
			    .Returns("20");

			propertyBagMock
			    .SetupGet(x => x["Page.Number"])
			    .Returns("1");

			int[] ints = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

			object result = Filter.Run(ints, new string[] { "3" }, propertyBagMock.Object, null);

			var enumerable = result as IEnumerable;
			Assert.IsNotNull(enumerable);
		}

		[Test]
		public void HandlesPagingWhenListIsEmpty()
		{
			var propertyBagMock = new Mock<IPropertyBag>();
			propertyBagMock
				.SetupGet(x => x["Page.Size"])
				.Returns("20");

			propertyBagMock
				.SetupGet(x => x["Page.Number"])
				.Returns("1");

			int[] ints = new int[0];

			object result = Filter.Run(ints, new string[] { "3" }, propertyBagMock.Object, null);

			var enumerable = result as IEnumerable;
			Assert.IsNotNull(enumerable);
		}

		[Test]
		[ExpectedException(typeof(ImpressionInterpretException))]
		public void ThrowsExceptionWhenPassedEmptyParameters()
		{
			object result = Filter.Run(null, new string[0], null, null);
		}


	}
}
