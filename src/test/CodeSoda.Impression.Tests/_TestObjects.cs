
using System.Collections.Generic;

namespace CodeSoda.Impression.Tests
{
	public class SelectListItem
	{
		public string Text;
		public string Value;
	}

	public class SimpleObject
	{
		public string Name;
		public int Age;

		public string Property { get; set; }
		public string Field;

		public IList<SimpleObject> Children;

		public SimpleObject FirstChild
		{
			get { return Children.Count > 0 ? Children[0] : null; }
		}

		public SimpleObject()
		{
			Children = new List<SimpleObject>();
		}

	}
}
