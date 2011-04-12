
using System.Collections;

namespace CodeSoda.Impression.Filters
{
	public class EmptyFalseFilter : FilterBase
	{
		public override string Keyword
		{
			get { return "empty_false"; }
		}

		public override object Run(object obj, string[] parameters, IPropertyBag bag, IMarkupBase markup)
		{
			if (parameters != null && parameters.Length > 0)
				throw new ImpressionInterpretException("Filter " + Keyword + " cannot be used with parameters.", markup);

			if (obj == null)
				return false;

			bool isEmpty = true;
			if (obj is string && !string.IsNullOrEmpty((string)obj)) {
				isEmpty = false;
			} else {
				if (obj is IEnumerable) {
					var en = (obj as IEnumerable).GetEnumerator();
					isEmpty = !en.MoveNext();
				} else {
					isEmpty = false;
				}
			}

			return !isEmpty;

		}

	}
}
