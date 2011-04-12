
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

namespace CodeSoda.Impression.Filters
{
	public class StripHtmlFilter: IFilter
	{

		virtual public string Keyword
		{
			get { return "strip_html"; }
		}

		virtual public object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup)
		{
			if (parameters != null && parameters.Length > 0)
				throw new ImpressionInterpretException("Formatter " + Keyword + " does not accept parameters.");

			if (obj != null)
			{
				obj = Regex.Replace(obj.ToString(), @"<(.|\r|\n)*?>", string.Empty);
			}
			return obj;
		}
	}

	public class StripNewLinesFilter : IFilter
	{
		virtual public string Keyword {
			get { return "strip_newlines"; }
		}

		virtual public object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup) {
			if (parameters != null && parameters.Length > 0)
				throw new ImpressionInterpretException("Formatter " + Keyword + " does not accept parameters.");

			if (obj != null) {
				obj = obj.ToString().Replace("\n", "").Replace("\r", "");
			}
			return obj;
		}
	}


	//TODO replace this with the other htmlify method
	public class HtmlifyFilter : IFilter
	{
		virtual public string Keyword {
			get { return "htmlify"; }
		}

		virtual public object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup) {
			if (parameters != null && parameters.Length > 0)
				throw new ImpressionInterpretException("Formatter " + Keyword + " does not accept parameters.");

			if (obj != null) {

				string text = obj.ToString();

				// firstly encode any special characters for html
				string newtext = HttpUtility.HtmlEncode(text);

				// be friendly with newlines from different platforms
				newtext = newtext.Replace("\r\n", "\n");
				newtext = newtext.Replace("\r", "\n");

				// split double \n into paragraphs
				string[] paragraphs = newtext.Split(
					new[] { "\n\n" },
					StringSplitOptions.RemoveEmptyEntries
				);

				// clean up whitespace on each paragraph
				for (int i = 0, len = paragraphs.Length; i < len; i++)
					paragraphs[i] = paragraphs[i].Trim();

				// if there is actually some content
				if (paragraphs.Length > 0) {
					// wrap paragraphs in html p tags
					newtext = "<p>" + string.Join(
						"</p><p>",
						paragraphs
					) + "</p>";

					// replace existing single \n within paragraphs with link breaks
					newtext = newtext.Replace("\n", "<br />");
				}
				else {
					newtext = null;
				}

				return newtext;
			}
			return obj;
		}
	}

	public class TruncateCharactersFilter: IFilter {
		virtual public string Keyword { get { return "truncate"; } }

		virtual public object Run(object obj, string[] paramaters, IPropertyBag bag, IMarkupBase markup) {

			//TODO read the number of characters from the first parameter

			string s = (obj != null) ? obj.ToString() : null;
			if (!string.IsNullOrEmpty(s)) {
				string[] words = s.Split(new[] { " " }, StringSplitOptions.None);

				bool truncated = false;
				if (words.Length > 200) {
					truncated = true;
					words = new List<string>(words).GetRange(0, 200).ToArray();
				}
				obj = string.Join(" ", words) + (truncated ? "..." : "");
			}
			return obj;
		}
	}

	public class TruncateWordsFilter : IFilter
	{
		virtual public string Keyword { get { return "truncate_words"; } }

		virtual public object Run(object obj, string[] paramaters, IPropertyBag bag, IMarkupBase markup) {

			//TODO read the number of characters from the first parameter

			string s = (obj != null) ? obj.ToString() : null;
			if (!string.IsNullOrEmpty(s)) {
				string[] words = s.Split(new[] { " " }, StringSplitOptions.None);

				bool truncated = false;
				string[] subsetWords = new string[200];
				if (words.Length > 200) {
					truncated = true;
					words.CopyTo(subsetWords, 0);
				}
				s = string.Join(" ", words) + (truncated ? "..." : "");
			}
			return s;
		}
	}

}
