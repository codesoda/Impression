
using System.Collections;
using System.Collections.Generic;
using CodeSoda.Impression.Filters;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests.Filters
{
	[TestFixture]
	class ShuffleTests
	{

		[Test]
		public void ShuffleTest_IntGenericEnumerable()
		{
			IEnumerable<int> original = new[] { 1, 2, 3, 4, 5 };

			object obj = new ShuffleFormatter().Run(original, null, null, null);

			Assert.IsNotNull(obj);

			//			Assert.IsTrue(
			//			    original[0] != shuffled[0]
			//			    || original[1] != shuffled[1]
			//			    || original[2] != shuffled[2]
			//			    || original[3] != shuffled[3]
			//			    || original[4] != shuffled[4]
			//			);
		}

		[Test]
		public void ShuffleTest_IntEnumerable()
		{
			IEnumerable original = new[] { 1, 2, 3, 4, 5 };

			object obj = new ShuffleFormatter().Run(original, null, null, null);

			Assert.IsNotNull(obj);

		}

		[Test]
		public void ShuffleTest_WithNull()
		{
			object obj = new ShuffleFormatter().Run(null, null, null, null);
			Assert.IsNull(obj);
		}

		[Test]
		public void ShuffleTest_IgnoredForStrings()
		{
			object obj = new ShuffleFormatter().Run("string", null, null, null);
			Assert.AreEqual("string", obj as string);
		}

		[Test]
		public void ShuffleSimpleObject()
		{

			IList<SimpleObject> original = new List<SimpleObject> {
				new SimpleObject {Age = 1, Name = "A"},
				new SimpleObject {Age = 2, Name = "B"},
				new SimpleObject {Age = 3, Name = "C"},
				new SimpleObject {Age = 4, Name = "D"},
				new SimpleObject {Age = 5, Name = "E"}
			};

			IEnumerable shuffled = (IEnumerable)(
				new ShuffleFormatter().Run(original, null, null, null)
			);

			IList<SimpleObject> shuffledList = new List<SimpleObject>();
			foreach (SimpleObject obj in shuffled)
			{
				shuffledList.Add(obj);
			}

			Assert.IsTrue(
				shuffledList[0].Age != original[0].Age
				|| shuffledList[1].Age != original[1].Age
				|| shuffledList[2].Age != original[2].Age
				|| shuffledList[3].Age != original[3].Age
				|| shuffledList[4].Age != original[4].Age
			);

		}

	}
}
