using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests
{
	[TestFixture]
	public class ExpressionMarkupTests
	{
		private Mock<IReflector> reflectorMock;
		private Mock<IFilterRunner> filterRunnerMock;
		private Mock<IInterpretContext> interpretContextMock;
		private Mock<IPropertyBag> propertyBagMock;
		
		private ExpressionMarkup markup;
		

		[SetUp]
		public void Setup()
		{
			reflectorMock = new Mock<IReflector>();
			filterRunnerMock = new Mock<IFilterRunner>();
			filterRunnerMock // make runfilters do nothiung
				.Setup(x => x.RunFilters(
					It.IsAny<object>(),
					It.IsAny<string[]>(),
					It.IsAny<PropertyBag>(),
					It.IsAny<IMarkupBase>()
				))
				.Returns((object o) => o);

			propertyBagMock = new Mock<IPropertyBag>();
			//propertyBagMock.Setup(x => x[It.IsAny<string>()]).Returns(null);

			interpretContextMock = new Mock<IInterpretContext>();
			interpretContextMock
				.SetupGet(x => x.Bag)
				.Returns(propertyBagMock.Object);

			markup = new ExpressionMarkup(
				reflectorMock.Object,
				filterRunnerMock.Object,
				"{{markup}}",
				10,
				10
			);
		}

		#region Constructor Tests

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructorThrowsArgumentNullExceptionForNullReflector()
		{
			markup = new ExpressionMarkup(
				null,
				filterRunnerMock.Object,
				"markup",
				10,
				10
			);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructorThrowsArgumentNullExceptionForNullFilterRunner()
		{
			markup = new ExpressionMarkup(
				reflectorMock.Object,
				null,
				"markup",
				10,
				10
			);

		}

		#endregion Constructor Tests

		[Test]
		public void MarkupTypeIsExpression()
		{
			Assert.AreEqual(MarkupType.Expression, markup.MarkupType);
		}



		#region specific parsing checks

		//TODO this should be its own object and have its own tests

		[Test]
		[ExpectedException(typeof(ImpressionParseException))]
		public void ThrowsExceptionForBadmarkup()
		{
			markup = new ExpressionMarkup(
				reflectorMock.Object,
				filterRunnerMock.Object,
				"markup",
				10,
				10
			);
		}

		[Test]
		public void ParsesGoodMarkup()
		{
			markup = new ExpressionMarkup(
				reflectorMock.Object,
				filterRunnerMock.Object,
				"{{markup}}",
				10,
				10
			);

			Assert.AreEqual("markup", markup.ObjectName);
			Assert.IsNull(markup.Properties);
			Assert.IsNull(markup.Within);
		}

		#endregion specific parsing checks

		// ExpressionMarkup should be evaluated by something seperate, like an ExpressionMarkupEvaluator

		[Test]
		public void TestEvaluate()
		{
			propertyBagMock.Setup(x => x.ContainsKey("markup")).Returns(true);
			propertyBagMock.Setup(x => x["markup"]).Returns(new object());
			var obj = markup.Evaluate(interpretContextMock.Object);

			Assert.IsNotNull(obj);
		}

		[Test]
		public void TestEvaluateAsString()
		{
			
		}

		[Test]
		public void TestEvaluateAsBoolean()
		{
			
		}

		[Test]
		public void TestCanParseXEqualsYExpression()
		{
			markup = new ExpressionMarkup(
				reflectorMock.Object,
				filterRunnerMock.Object,
				"{{ abc = xyz.prop1.prop2 }}",
				10,
				10
			);

			Assert.AreEqual("abc", markup.Within);
			Assert.AreEqual("xyz", markup.ObjectName);
			Assert.AreEqual(2, markup.Properties.Length);
			Assert.AreEqual("prop1", markup.Properties[0]);
			Assert.AreEqual("prop2", markup.Properties[1]);
		}
	}
}
