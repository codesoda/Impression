
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests
{
	public class ReflectObject {
		public string StringField;
		public int IntField;

		public string StringProperty { get; set; }
		public int IntProperty { get; set; }

		public NameValueCollection NameValues;
		public Dictionary<int, string> IntKeyDictionary { get; set; }
		public Dictionary<string, string> StringKeyDictionary { get; set; }
	}

	[TestFixture]
	public class ReflectorTests
	{
		private ReflectObject robj = null;


		[SetUp]
		public void SetUp() {
			robj = new ReflectObject()
			{
				IntField = 1,
				IntProperty = 1,
				StringField = "String",
				StringProperty = "String",
				NameValues = new NameValueCollection {
					{"Key1", "Value1"},
					{"Key2", "Value2"},
					{"Key3", "Value3"}
				},
				IntKeyDictionary = new Dictionary<int, string> {
					{1, "Value1"},
					{2, "Value2"},
					{3, "Value3"}
				},
				StringKeyDictionary = new Dictionary<string, string> {
					{"Key1", "Value1"},
					{"Key2", "Value2"},
					{"Key3", "Value3"}
				}

			};
		}

		[Test]
		public void TestAsBoolean()
		{

			// booleans
			Assert.IsTrue(new Reflector().AsBoolean(true));
			Assert.IsFalse(new Reflector().AsBoolean(false));

			// numbers
			Assert.IsFalse(new Reflector().AsBoolean(-5));
			Assert.IsFalse(new Reflector().AsBoolean(0));
			Assert.IsTrue(new Reflector().AsBoolean(5));

			// strings
			Assert.IsFalse(new Reflector().AsBoolean("0"));
			Assert.IsFalse(new Reflector().AsBoolean("false"));
			Assert.IsFalse(new Reflector().AsBoolean("False"));
			Assert.IsFalse(new Reflector().AsBoolean("FALSE"));
			Assert.IsTrue(new Reflector().AsBoolean("1"));
			Assert.IsTrue(new Reflector().AsBoolean("true"));
			Assert.IsTrue(new Reflector().AsBoolean("True"));
			Assert.IsTrue(new Reflector().AsBoolean("TRUE"));
			Assert.IsTrue(new Reflector().AsBoolean("SomeValue"));

			// enumerable
			Assert.IsFalse(new Reflector().AsBoolean(new int[0]));
			Assert.IsFalse(new Reflector().AsBoolean(new List<int>()));
			Assert.IsFalse(new Reflector().AsBoolean(new Dictionary<int, int>()));
			Assert.IsTrue(new Reflector().AsBoolean(new[] { 1, 2, 3 }));
			Assert.IsTrue(new Reflector().AsBoolean(new List<int> { 1, 2, 3 }));
			Assert.IsTrue(new Reflector().AsBoolean(new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } }));

			// complex object
			Assert.IsTrue(new Reflector().AsBoolean(new SimpleObject { Age = 1, Name = "A" }));

			// null
			Assert.IsFalse(new Reflector().AsBoolean(null));
		}

		[Test]
		public void TestTryGetPropertyInt()
		{
			Reflector rf = new Reflector();
			object value = null;
			bool found = rf.TryGetProperty(robj, "IntProperty", robj.GetType(), out value);

			Assert.IsTrue(found);
			Assert.AreEqual(1, value);

			Assert.IsFalse(rf.TryGetField(robj, "IntProperty", robj.GetType(), out value));
		}

		[Test]
		public void TestTryGetFieldInt()
		{
			Reflector rf = new Reflector();
			object value = null;
			bool found = rf.TryGetField(robj, "IntField", robj.GetType(), out value);

			Assert.IsTrue(found);
			Assert.AreEqual(1, value);

			Assert.IsFalse(rf.TryGetField(robj, "IntProperty", robj.GetType(), out value));
		}

		[Test]
		public void TestTryGetPropertyString()
		{
			Reflector rf = new Reflector();
			object value = null;
			bool found = rf.TryGetProperty(robj, "StringProperty", robj.GetType(), out value);

			Assert.IsTrue(found);
			Assert.AreEqual("String", value);

		}

		[Test]
		public void TestTryGetFieldString()
		{
			Reflector rf = new Reflector();
			object value = null;
			bool found = rf.TryGetField(robj, "StringField", robj.GetType(), out value);

			Assert.IsTrue(found);
			Assert.AreEqual("String", value);

		}

		[Test]
		public void TestTryGetFieldObject()
		{
			Reflector rf = new Reflector();
			object value = null;
			bool found = rf.TryGetField(robj, "NameValues", robj.GetType(), out value);

			Assert.IsTrue(found);
			Assert.AreEqual(typeof(NameValueCollection), value.GetType());

			Assert.IsFalse(rf.TryGetProperty(robj, "NameValues", robj.GetType(), out value));
		}

		[Test]
		public void TestTryGetPropertyObject()
		{
			Reflector rf = new Reflector();
			object value = null;
			bool found = rf.TryGetProperty(robj, "IntKeyDictionary", robj.GetType(), out value);

			Assert.IsTrue(found);
			Assert.AreEqual(typeof(Dictionary<int, string>), value.GetType());

			Assert.IsFalse(rf.TryGetField(robj, "IntKeyDictionary", robj.GetType(), out value));
		}

		[Test]
		public void TestEvalLookup() {
			Reflector rf = new Reflector();

			object value = rf.Eval(robj, "IntField");
			Assert.AreEqual(1, value);

			value = rf.Eval(robj, "IntProperty");
			Assert.AreEqual(1, value);

			value = rf.Eval(robj, "StringField");
			Assert.AreEqual("String", value);

			value = rf.Eval(robj, "StringProperty");
			Assert.AreEqual("String", value);

			value = rf.Eval(robj, "NameValues");
			Assert.IsNotNull(value);

			value = rf.Eval(robj, "IntKeyDictionary");
			Assert.IsNotNull(value);

			value = rf.Eval(robj, "UnknownField");
			Assert.IsNull(value);

			value = rf.Eval(robj, "StringProperty.Length");
			Assert.AreEqual(6, value);

			value = rf.Eval(robj, new [] { "StringProperty", "Length"});
			Assert.AreEqual(6, value);

			value = rf.Eval(robj, "IntKeyDictionary.1");
			Assert.AreEqual("Value1", value);

			value = rf.Eval(robj, "IntKeyDictionary.1.Length");
			Assert.AreEqual(6, value);

			value = rf.Eval(robj, "StringKeyDictionary.Key2");
			Assert.AreEqual("Value2", value);

			value = rf.Eval(robj, "StringKeyDictionary.Key3.Length");
			Assert.AreEqual(6, value);
		}


	}
}
