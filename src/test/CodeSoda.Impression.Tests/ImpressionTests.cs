using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests
{
	[TestFixture]
	public class ImpressionTests
	{

		#region Constructor

		[Test]
		public void ConstructorTest_Empty() {
			ImpressionEngine ie = ImpressionEngine.Create(null, null);
		}

		[Test]
		public void ConstructorTest_WithPropertyBag() {
			ImpressionEngine ie = ImpressionEngine.Create(new PropertyBag());
		}

		[Test]
		public void ConstructorTest_WithPropertyBagAndTemplate() {
			string currentFolder = Path.GetDirectoryName(Environment.CurrentDirectory);
			string templatePath = Path.Combine(currentFolder, "../templates/empty.htm");
			ImpressionEngine ie = ImpressionEngine.Create(templatePath, new PropertyBag());
		}

		[Test]
		[ExpectedException(ExceptionName = "System.ArgumentException")]
		public void ConstructorTest_InvalidTemplatePathExpectArgumentException() {
			ImpressionEngine ie = ImpressionEngine.Create("xxxx");
		}

		#endregion

		#region SSI

		[Test]
		public void DummyTemplateTest_SimpleSSI() {
			string currentFolder = Path.GetDirectoryName(Environment.CurrentDirectory);
			string templatePath = Path.Combine(currentFolder, "../templates/simplessi.htm");
			ImpressionEngine ie = ImpressionEngine.Create(templatePath);
			string result = ie.Run();
			Assert.AreEqual("This page will load SSI Content", result);
		}

		#endregion


		#region Parsing Tests 

		[Test]
		public void TestObjectNameIsCaseInsensitive() {
			PropertyBag bag = new PropertyBag();
			bag.Add("UPPERCASE", "UPPERCASE");
			bag.Add("lowercase", "LOWERCASE");
			bag.Add("camelCase", "CAMELCASE");
			bag.Add("PascalCase", "PASCALCASE");
			ImpressionEngine ie = ImpressionEngine.Create(bag);

			string result = ie.RunString("{{uppercase}},{{LOWERCASE}},{{CAMELcase}},{{pascalCASE}}");
			Assert.AreEqual(
				"UPPERCASE,LOWERCASE,CAMELCASE,PASCALCASE",
				result
			);
		}

		[Test]
		public void TestObjectPropertiesAreCaseInsensitive() {
			PropertyBag bag = new PropertyBag();
			bag.Add("UPPERCASE", new SimpleObject() { Name = "Name" });
			bag.Add("lowercase", new SimpleObject() { Field = "Name" });
			bag.Add("camelCase", new SimpleObject() { Property = "Name" });
			bag.Add("PascalCase", new Dictionary<string, string> { { "Key", "Name" }});
			
			ImpressionEngine ie = ImpressionEngine.Create(bag);
			string result = ie.RunString("{{uppercase.NAME}},{{LOWERCASE.field}},{{CAMELcase.PROPERTy}},{{pascalCASE.kEY}}");
			
			Assert.AreEqual(
				"Name,Name,Name,Name",
				result
			);
		}

		[Test]
		public void TestSpacingWithFilters() {
			PropertyBag bag = new PropertyBag();

			ImpressionEngine ie = ImpressionEngine.Create(bag);
			string result = ie.RunString("{{ truevalue | default('true') }}");
			Assert.AreEqual("true", result);

			result = ie.RunString("{{truevalue|default('true')}}");
			Assert.AreEqual("true", result);
		}

		#endregion Parsing tests

		[Test]
		public void DummyTemplateTest_SimpleIfEndIfTrue() {
			string currentFolder = Path.GetDirectoryName(Environment.CurrentDirectory);
			string templatePath = Path.Combine(currentFolder, "../templates/simpleifendiftrue.htm");
			ImpressionEngine ie = ImpressionEngine.Create(templatePath);
			ie.AddItem("TrueValue", true);
			string result = ie.Run();
			Assert.AreEqual("TrueValue", result);
		}

		[Test]
		public void DummyTemplateTest_SimpleIfEndIfFalse() {
			string currentFolder = Path.GetDirectoryName(Environment.CurrentDirectory);
			string templatePath = Path.Combine(currentFolder, "../templates/simpleifendiffalse.htm");
			ImpressionEngine ie = ImpressionEngine.Create(templatePath);
			ie.AddItem("FalseValue", false);
			string result = ie.Run();
			Assert.AreEqual("FalseValue", result);
		}

		[Test]
		public void DummyTemplateTest_SimpleIfElseEndIf() {
			string currentFolder = Path.GetDirectoryName(Environment.CurrentDirectory);
			string templatePath = Path.Combine(currentFolder, "../templates/simpleifelseendif.htm");
			ImpressionEngine ie = ImpressionEngine.Create(templatePath);
			ie.AddItem("TrueValue", true);
			string result = ie.Run();
			Assert.AreEqual("TrueValue", result);
		}

		[Test]
		public void DummyTemplateTest_SimpleIfElseIfElseEndIf()
		{
			string currentFolder = Path.GetDirectoryName(Environment.CurrentDirectory);
			string templatePath = Path.Combine(currentFolder, "../templates/simpleifelseifelseendif.htm");
			ImpressionEngine ie = ImpressionEngine.Create(templatePath);
			ie.AddItem("TrueValue", true);
			string result = ie.Run();
			Assert.AreEqual("TrueValue", result);
		}

		[Test]
		public void DummyTemplateTest_SimpleForEach() {
			string currentFolder = Path.GetDirectoryName(Environment.CurrentDirectory);
			string templatePath = Path.Combine(currentFolder, "../templates/SimpleForEach.htm");
			ImpressionEngine ie = ImpressionEngine.Create(templatePath);
			ie.AddItem("Numbers", new [] { 1, 2, 3, 4, 5 });
			string result = ie.Run();
			Assert.AreEqual("12345", result);
		}

		[Test]
		public void DummyTemplateTest_LineNumberCharPos() {
			string currentFolder = Path.GetDirectoryName(Environment.CurrentDirectory);
			string templatePath = Path.Combine(currentFolder, "../templates/lineNumberCharPos.htm");
			ImpressionEngine ie = ImpressionEngine.Create(templatePath);
		}

		[Test]
		public void ForEachWithPositionTest() {
			PropertyBag bag = new PropertyBag();
			bag.Add("strings", new[] { "One", "Two", "Three" });
			ImpressionEngine ie = ImpressionEngine.Create(bag);
			string s = ie.RunString("<!-- #FOREACH {{string in strings}} -->{{string.Position}}={{string}}<!-- #NEXT -->");
			Assert.AreEqual(s, "1=One2=Two3=Three");
		}

		[Test]
		[ExpectedException(ExceptionName = "CodeSoda.Impression.ImpressionInterpretException")]
		public void DummyTemplateTest_BadlyFormedTemplate() {
			string currentFolder = Path.GetDirectoryName(Environment.CurrentDirectory);
			string templatePath = Path.Combine(currentFolder, "../templates/badlyformed.htm");
			ImpressionEngine ie = ImpressionEngine.Create(templatePath);
			string result = ie.Run();
			Assert.IsNotNull(result);
		}

		[Test]
		public void DummyTemplateTest_DefaultFormatter() {
			PropertyBag bag = new PropertyBag();
			bag.Add("DefaultValue", 1234);

			ImpressionEngine ie = ImpressionEngine.Create(bag);
			string result = ie.RunString("{{ InvalidField | default(DefaultValue) }}");
			Assert.IsNotNull(result);
			Assert.AreEqual("1234", result);
		}

		[Test]
		public void TestHtmlOptions() {
			PropertyBag bag = new PropertyBag();
			bag.Add("SelectedValue", new { Age = new { Value = 2 } });
			bag.Add("Values", new[] {
				new SimpleObject {Age = 1, Name = "One"},
				new SimpleObject {Age = 2, Name = "Two"},
				new SimpleObject {Age = 3, Name = "Three"},
				new SimpleObject {Age = 4, Name = "Four"},
				new SimpleObject {Age = 5, Name = "Five"}
			});

			string select = ImpressionEngine.Create(bag).RunString("<select>{{Values|html_options(Name,Age,SelectedValue.Age.Value)}}</select>");
			Assert.AreEqual("<select><option value=\"1\">One</option><option value=\"2\" selected=\"true\">Two</option><option value=\"3\">Three</option><option value=\"4\">Four</option><option value=\"5\">Five</option></select>", select);
		}

		[Test]
		public void DummyTemplateTest_HtmlCheckedFormatter() {
			PropertyBag bag = new PropertyBag();
			bag.Add("TrueValue", true);
			bag.Add("FalseValue", false);

			ImpressionEngine ie = ImpressionEngine.Create(bag);

			Assert.AreEqual("checked=\"checked\"", ie.RunString("{{ TrueValue | html_checked }}"));
			Assert.AreEqual("", ie.RunString("{{ FalseValue | html_checked }}"));
		}

		[Test]
		public void TestHtmlOptionsWithInvalidData() {
			PropertyBag bag = new PropertyBag();
			bag.Add("SelectedValue", new { Age = new { Value = 2 } });
			bag.Add("Values", new[] {
				new SimpleObject {Age = 1, Name = "One"},
				new SimpleObject {Age = 2, Name = "Two"},
				new SimpleObject {Age = 3, Name = "Three"},
				new SimpleObject {Age = 4, Name = "Four"},
				new SimpleObject {Age = 5, Name = "Five"}
			});

			string select = ImpressionEngine.Create(bag).RunString("<select>{{Valuesx|html_options(xyz,abc,def)}}</select>");
		}

		[Test]
		public void TestHtmlCheckbox() {
			PropertyBag bag = new PropertyBag();
			bag.Add("TrueValue", true);
			bag.Add("FalseValue", false);

			string result = ImpressionEngine.Create(bag).RunString("<input type=\"checkbox\" {{TrueValue|html_checked}} />");
			Assert.AreEqual("<input type=\"checkbox\" checked=\"checked\" />", result);

			result = ImpressionEngine.Create(bag).RunString("<input type=\"checkbox\" {{FalseValue|html_checked}} />");
			Assert.AreEqual("<input type=\"checkbox\"  />", result);

			result = ImpressionEngine.Create(bag).RunString("<input type=\"checkbox\" {{UnknownValue|html_checked}} />");
			Assert.AreEqual("<input type=\"checkbox\"  />", result);

			result = ImpressionEngine.Create(bag).RunString("<input type=\"checkbox\" {{UnknownValue|not|html_checked}} />");
			Assert.AreEqual("<input type=\"checkbox\" checked=\"checked\" />", result);

			result = ImpressionEngine.Create(bag).RunString("<input type=\"checkbox\" {{UnknownValue|default('true')|html_checked}} />");
			Assert.AreEqual("<input type=\"checkbox\" checked=\"checked\" />", result);
		}

		[Test]
		public void TestIFAttribute() {
			PropertyBag bag = new PropertyBag();
			bag.Add("TrueValue", true);
			bag.Add("FalseValue", false);

			string result = ImpressionEngine.Create(bag).RunString("<h1 style=\"{{TrueValue|if('','display:none')}}\">Heading</h1>");
		}



		[Test]
		public void TestPick() {
			PropertyBag bag = new PropertyBag();
			bag.Add("Values", new[] {
				new SimpleObject {Age = 1, Name = "One"},
				new SimpleObject {Age = 2, Name = "Two"},
				new SimpleObject {Age = 3, Name = "Three"},
				new SimpleObject {Age = 4, Name = "Four"},
				new SimpleObject {Age = 5, Name = "Five"}
			});

			string result = ImpressionEngine.Create(bag).RunString("<!-- #FOREACH {{ x in Values | pick(3) }} -->{{x.Name}}<!-- #NEXT -->");
			Assert.AreEqual("OneTwoThree", result);
		}

		[Test]
		public void TestShuffle() {
			PropertyBag bag = new PropertyBag();
			bag.Add("Values", new[] {
				new SimpleObject {Age = 1, Name = "One"},
				new SimpleObject {Age = 2, Name = "Two"},
				new SimpleObject {Age = 3, Name = "Three"},
				new SimpleObject {Age = 4, Name = "Four"},
				new SimpleObject {Age = 5, Name = "Five"}
			});

			string result = ImpressionEngine.Create(bag).RunString("<!-- #FOREACH {{ x in Values | shuffle }} -->{{x.Name}}<!-- #NEXT -->");
			Assert.AreNotEqual("OneTwoThreeFourFive", result);
			string result2 = ImpressionEngine.Create(bag).RunString("<!-- #FOREACH {{ x in Values | shuffle }} -->{{x.Name}}<!-- #NEXT -->");
			Assert.AreNotEqual(result,result2);
		}

		[Test]
		public void Test2DeepExpression()
		{
			PropertyBag bag = new PropertyBag();

			var o1 = new SimpleObject {
				Age = 1,
				Children = new List<SimpleObject> {
					new SimpleObject {
						Age = 2,
						Children = new List<SimpleObject> {
							new SimpleObject {
								Age = 3
							}
						}
					}
				}
			};
			bag.Add("Value", o1);

			var imp = ImpressionEngine.Create(bag);
			var result = imp.RunString("{{value.firstchild.firstchild.age}}");

			Assert.AreEqual("3", result);


		}

	}
}
