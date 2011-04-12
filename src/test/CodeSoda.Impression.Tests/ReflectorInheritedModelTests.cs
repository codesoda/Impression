using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests
{
	[TestFixture]
	public class ReflectorInheritedModelTests
	{
		private SpecialisedModel model = new SpecialisedModel { NewId = Guid.NewGuid(), OverrideId = Guid.NewGuid(), BaseName = "BaseName", NewName = "NewName" };

		[Test]
		public void TestInheritedOverrideIdTest() {
			object obj = new Reflector().Eval(model, new[] {"OverrideId"});
			Assert.AreEqual(model.OverrideId, obj);
		}

		[Test]
		public void TestInheritedNewIdTest() {
			object obj = new Reflector().Eval(model, new[] { "NewId" });
			Assert.AreEqual(model.NewId, obj);
		}

		[Test]
		public void TestInheritedBaseNameTest()
		{
			object obj = new Reflector().Eval(model, new[] { "BaseName" });
			Assert.AreEqual(model.BaseName, obj);
		}

		[Test]
		public void TestInheritedNewNameTest()
		{
			object obj = new Reflector().Eval(model, new[] { "NewName" });
			Assert.AreEqual(model.NewName, obj);
		}
	}

	public class BaseModel
	{
		public virtual Guid OverrideId { get; set; }
		public virtual Guid NewId { get; set; }

		public virtual string BaseName { get; set; }
	}

	public class SpecialisedModel: BaseModel
	{
		public override Guid OverrideId
		{
			get { return base.OverrideId; }
			set { base.OverrideId = value;}
		}

		public new Guid NewId
		{
			get { return base.NewId; }
			set { base.NewId = value; }
		}

		public string NewName { get; set; }
	}
}
