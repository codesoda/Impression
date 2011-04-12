using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using CodeSoda.Impression.Parsers;

namespace CodeSoda.Impression
{

	public class TagFactory : ITagFactory
	{
		private TagParserCache cache = null;

		public TagFactory(IContainer container) {
			cache = new TagParserCache();
			TagParserCache.LoadParsers(cache, container);
		}


		public TagMarkupBase ParseTag(string markup, int lineNumber, int charPos) {

			ITagParser parser = cache.Find(x => x.CanParseTag(markup));

			if (parser == null) {
				throw new ImpressionParseException("Unsupported Tag Found", markup, lineNumber, charPos);
			}

			return parser.ParseTag(markup, lineNumber, charPos);

		}

	}
}
