using System;
using System.Collections.Generic;
using System.Text;

namespace CodeSoda.Impression.Parsers
{
	interface IExpressionParser
	{
		MarkupBase[] ParseExpressions(string markup);
	}
}
