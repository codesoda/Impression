using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace CodeSoda.Impression.Tests
{
	[TestFixture]
	public class EasyTextTests
	{
		private string simpleExample = @"
== This is a Heading ==
This is a word in /italics/
This is a word in *bold*
This word is _underlined_
picture:someimage.png
video:urltovideo
This is a word in *bold*
link:thisisawebsite


";
		// + one or more
		// * zero or more

		[Test]
		public void TestBoldSimple() {
			string text = "This *word* is *bold*";

			string transformed = simpleExample;
			transformed = SwapToken(transformed, @"\*", "b");
			transformed = SwapToken(transformed, "_", "u");
			transformed = SwapToken(transformed, "/", "i");

			transformed = transformed.Replace("\r", "").Replace("\n", "<br />");

			Debug.WriteLine(transformed);

			//MatchCollection matches = boldParser.Matches(simpleExample);
			//if (matches.Count > 0)
			//{
			//    StringBuilder sb = new StringBuilder();

			//    int lastIndex = 0;

			//    for (int i = 0, len = matches.Count; i < len; i++ )
			//    {
			//        Match match = matches[i];

			//        sb.Append(simpleExample.Substring(lastIndex, match.Index - lastIndex));

			//        string matchText = match.Groups["BoldText"].Value;
			//        sb.AppendFormat(
			//            "<{0}>{1}</{0}>",
			//            "b",
			//            matchText
			//        );

			//        // if this is the last item append the standard content after the last match
			//        if (i + 1 >= len)
			//            sb.Append(simpleExample.Substring(match.Index + match.Length));

			//        lastIndex = match.Index;
			//    }

			//    transformed = sb.ToString();
			//} else
			//{
			//    transformed = simpleExample;
			//}

		}

		private string SwapToken(string text, string token, string tagName) {

			string transformed = "";

			string regex = string.Format(
				"{0}(?<MatchedText>.+){0}",
				token
			);

			Regex boldParser = new Regex(
				regex, //<!--\s+\#[\s*=\s*"\s*(?<File>.+)\s*"\s*-->"
				RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.ExplicitCapture
			);

			MatchCollection matches = boldParser.Matches(text);
			if (matches.Count > 0)
			{
				StringBuilder sb = new StringBuilder();

				int lastIndex = 0;

				for (int i = 0, len = matches.Count; i < len; i++)
				{
					Match match = matches[i];

					sb.Append(simpleExample.Substring(lastIndex, match.Index - lastIndex));

					string matchText = match.Groups["MatchedText"].Value;
					sb.AppendFormat(
						"<{0}>{1}</{0}>",
						tagName,
						matchText
					);

					// if this is the last item append the standard content after the last match
					if (i + 1 >= len)
						sb.Append(simpleExample.Substring(match.Index + match.Length));

					lastIndex = match.Index;
				}

				transformed = sb.ToString();
			}
			else
			{
				transformed = text;
			}

			return transformed;
		}

	}
}