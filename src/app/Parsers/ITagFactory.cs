using System;
using System.Collections.Generic;
using System.Text;

namespace CodeSoda.Impression.Parsers
{
	public interface ITagFactory
	{
		TagMarkupBase ParseTag(string markup, int lineNumber, int charPos);
	}
}
