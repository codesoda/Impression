using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CodeSoda.Impression.Filters {

	public class PagedFilter : IFilter {

		public string Keyword {
			get { return "paged"; }
		}

		public object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup) {

			if (parameters == null || parameters.Length != 1)
				throw new ImpressionInterpretException("Filter " + Keyword + " expects 1 parameter.");

			var enumerable = obj as IEnumerable;
			if (enumerable == null)
				return null;

			int pageNumber = 1;
			object pageNumberObject = bag["Page.Number"];
			if (pageNumberObject != null) {
				Int32.TryParse(pageNumberObject.ToString(), out pageNumber);
				if (pageNumber < 1)
					pageNumber = 1;
			}

			int pageSize = 12;
			string pageSizeString = parameters[0];
			object pageSizeObject = bag["Page.Size"];
			if (pageSizeObject != null) {
				pageSizeString = pageSizeObject.ToString();
			}
			Int32.TryParse(pageSizeString, out pageSize);
			if (pageSize < 1)
				pageSize = 12;

			int i = 0, start = (pageNumber - 1)*pageSize, end = (pageNumber*pageSize)-1;
			var list = new ModelListWithPages(pageSize);

			// add the "paged" items
			foreach(var item in enumerable) {
				if (i >= start && i <= end)
						list.Add(item);

				i++;
			}

			// add the pages
			var totalPages = (int)Math.Ceiling(i / (double)pageSize);
			var totalPages0 = totalPages - 1;
			var pageNumber0 = pageNumber - 1;
			for (i = 0; i < totalPages; i++) {
				list.Pages.Add( new PageModel {
					Current = (i == pageNumber0),
					First = (i == 0),
					Last = (i == (totalPages0)),
					Number = (i+1),
					Url = "?page=" + (i+1)
				} );
			}

			list.FirstPage = list.Pages[0];
			list.PrevPage = pageNumber != 1 ? list.Pages[pageNumber0-1]: null;
			list.CurrentPage = list.Pages[pageNumber0];
			list.NextPage = pageNumber != totalPages ? list.Pages[pageNumber]: null;
			list.LastPage = list.Pages[totalPages0];

			return list;
		}

	}

	public class PagerFilter: IFilter {

		public string Keyword {
			get { return "pager"; }
		}

		public object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup) {
			var list = obj as ModelListWithPages;

			if (list == null)
				return null;

			StringBuilder sb = new StringBuilder();

			if (list.CurrentPage.First) {
				sb.Append("<span class=\"page first disabled\">First</span>");
				sb.Append("<span class=\"page prev disabled\">Prev</span>");
			} else {
				sb.AppendFormat("<a href=\"{0}\" class=\"page first\">First</a>", list.FirstPage.Url);
				sb.AppendFormat("<a href=\"{0}\" class=\"page prev\">Prev</a>", list.PrevPage.Url);
			}

			foreach (var page in list.Pages) {
				if (page.Current) {
					sb.AppendFormat("<span class=\"page current\">{0}</span>", page.Number);
				}
				else {
					sb.AppendFormat("<a href=\"{0}\" class=\"page\">{1}</a>", page.Url, page.Number);
				}
			}

			if (list.CurrentPage.Last) {
				sb.Append("<span class=\"page next disabled\">Next</span>");
				sb.Append("<span class=\"page last disabled\">Last</span>");
			} else {
				sb.AppendFormat("<a href=\"{0}\" class=\"page next\">Next</a>", list.NextPage.Url);
				sb.AppendFormat("<a href=\"{0}\" class=\"page last\">Last</a>", list.LastPage.Url);
			}

			return sb.ToString();
		}
	}

	public class PageModel {

		public int Number { get; set; }
		public bool First { get; set; }
		public bool Last { get; set; }
		public bool Current { get; set; }
		public string Url { get; set; }

		public override string ToString() {
			return Number.ToString();
		}
	}

	public class ModelListWithPages : ArrayList
	{
		public IList<PageModel> Pages { get; set; }

		public PageModel FirstPage { get; set; }
		public PageModel PrevPage { get; set; }
		public PageModel CurrentPage { get; set; }
		public PageModel NextPage { get; set; }
		public PageModel LastPage { get; set; }

		public override string ToString() { return ""; }

		public ModelListWithPages(ICollection collection)
			: base(collection) {
			Init();
		}

		public ModelListWithPages(int capacity)
			: base(capacity) { Init(); }

		public ModelListWithPages() { Init(); }

		private void Init()
		{
			this.Pages = new List<PageModel>();
		}
	}

}
