using System;
using System.Collections.Generic;
using System.Text;

namespace CodeSoda.Impression.Tests.Filters
{
	public abstract class FilterTestBase<TFilter> where TFilter: IFilter, new()
	{
		protected TFilter Filter;
		protected IPropertyBag Bag;
		protected string[] Parameters;
		protected IMarkupBase Markup;

		virtual public void Setup() {
			Filter = new TFilter();
			Bag = new PropertyBag();
			Parameters = new string[] {};
		}
	}
}
