using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CodeSoda.Impression
{
	class FormatterCache
	{
		private Hashtable formatters = null;

		public FormatterCache()
		{
			this.formatters = new Hashtable();
		}

		public void Add(IFilter filter)
		{
			formatters.Add(filter.Keyword.ToLower(), filter);
		}
		public IFilter Get(string keyword)
		{
			return formatters.ContainsKey(keyword.ToLower())
				? formatters[keyword.ToLower()] as IFilter
				: null;
		}

		public static void LoadFormatters(FormatterCache cache)
		{
			if (cache == null)
				return;

			// load all formatters from this assembly
			Type[] types = Assembly.GetExecutingAssembly().GetTypes();
			foreach (Type type in types)
			{
				if (type.GetInterface("IFilter") != null && !type.IsAbstract)
				{
					IFilter filter = Activator.CreateInstance(type) as IFilter;
					if (filter != null)
					{
						cache.Add(filter);
					}
				}
			}
		}
	}

}
