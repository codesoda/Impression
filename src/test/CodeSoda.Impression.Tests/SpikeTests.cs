using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests
{
	[TestFixture]
	public class SpikeTests
	{
		[Test]
		public void checkifgenericdictionary()
		{
			var dict = new Dictionary<string, object>();
			//Debug.WriteLine(IsGenericDictionary(dict.GetType(), dict));
			Type contextType = dict.GetType();

			if (contextType.GetInterface("IDictionary`2") != null)
			{
				
			}
		}
	}
}
