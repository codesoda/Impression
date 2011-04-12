using System;
using System.Collections.Generic;
using System.Text;
using CodeSoda.Impression.Tags;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests
{
	[TestFixture]
	public class TagParserTests
	{
		[Test]
		public void TestParsingIfWithDashes()
		{
			const string markup = "<!-- #IF {{snippets.shopfront-callout}} -->";
			var parser = new IfTagParser(new Reflector(), new FilterRunner());
			Assert.IsTrue( parser.CanParseTag(markup) );

			Assert.IsNotNull(parser.ParseTag(markup,1,1));
		}

		//[Test]
		//public void TestParsingSimpleAttributeReturnsNormalFormatter()
		//{
		//    string[] parms = null;
		//    const string attribute = "format_date";
		//    string method = new TagParser().ParseParameterizedAttribute(
		//        attribute,
		//        out parms
		//    );

		//    Assert.AreEqual(attribute, method);
		//    Assert.IsNull(parms);
		//}

		//[Test]
		//public void TestParsingComplexAttributeWith1ParameterNoSpaces()
		//{
		//    string[] parms = null;
		//    const string attribute = "pick(8)";
		//    string method = new TagParser().ParseParameterizedAttribute(
		//        attribute,
		//        out parms
		//    );

		//    Assert.AreEqual("pick", method);
		//    Assert.IsNotNull(parms);
		//    Assert.AreEqual(1, parms.Length);
		//    Assert.AreEqual("8", parms[0]);
		//}

		//[Test]
		//public void TestParsingComplexAttributeWith1ParameterWithSpaces()
		//{
		//    string[] parms = null;
		//    const string attribute = " pick ( 8 ) ";
		//    string method = new TagParser().ParseParameterizedAttribute(
		//        attribute,
		//        out parms
		//    );

		//    Assert.AreEqual("pick", method);
		//    Assert.IsNotNull(parms);
		//    Assert.AreEqual(1, parms.Length);
		//    Assert.AreEqual("8", parms[0]);
		//}

		//[Test]
		//public void TestParsingComplexAttributeWith2ParametersNoSpaces()
		//{
		//    string[] parms = null;
		//    const string attribute = "if(5,10)";
		//    string method = new TagParser().ParseParameterizedAttribute(
		//        attribute,
		//        out parms
		//    );

		//    Assert.AreEqual("if", method);
		//    Assert.IsNotNull(parms);
		//    Assert.AreEqual(2, parms.Length);
		//    Assert.AreEqual("5", parms[0]);
		//    Assert.AreEqual("10", parms[1]);
		//}

		//[Test]
		//public void TestParsingComplexAttributeWith2ParameterWithSpaces()
		//{
		//    string[] parms = null;
		//    const string attribute = " if ( 5 , 10 ) ";
		//    string method = new TagParser().ParseParameterizedAttribute(
		//        attribute,
		//        out parms
		//    );

		//    Assert.AreEqual("if", method);
		//    Assert.IsNotNull(parms);
		//    Assert.AreEqual(2, parms.Length);
		//    Assert.AreEqual("5", parms[0]);
		//    Assert.AreEqual("10", parms[1]);
		//}

		//[Test]
		//public void TestParsingComplexAttributeWith1ParameterWithSpacesAndQuotes()
		//{
		//    string[] parms = null;
		//    const string attribute = " pick ( \"8\" ) ";
		//    string method = new TagParser().ParseParameterizedAttribute(
		//        attribute,
		//        out parms
		//    );

		//    Assert.AreEqual("pick", method);
		//    Assert.IsNotNull(parms);
		//    Assert.AreEqual(1, parms.Length);
		//    Assert.AreEqual("\"8\"", parms[0]);
		//}

		//[Test]
		//public void TestParsingComplexAttributeWith2ParameterWithSpacesAndQuotes()
		//{
		//    string[] parms = null;
		//    const string attribute = " if ( \"5\" , \"10\" ) ";
		//    string method = new TagParser().ParseParameterizedAttribute(
		//        attribute,
		//        out parms
		//    );

		//    Assert.AreEqual("if", method);
		//    Assert.IsNotNull(parms);
		//    Assert.AreEqual(2, parms.Length);
		//    Assert.AreEqual("\"5\"", parms[0]);
		//    Assert.AreEqual("\"10\"", parms[1]);
		//}


	}
}
