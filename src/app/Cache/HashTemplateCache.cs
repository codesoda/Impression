using System;
using System.Collections;
using CodeSoda.Impression;
namespace CodeSoda.Impression.Cache
{

	public class HashtableTemplateCache : ITemplateCache
	{
		// Fields
		private static Hashtable cacheItems = new Hashtable();

		// Methods
		public HashtableTemplateCache() : this(40) {}

		public HashtableTemplateCache(int maxCacheItems)
		{
			this.MaxCacheItems = maxCacheItems;
		}

		public void Add(string key, ParseList parseList)
		{
			if (string.IsNullOrEmpty(key))
				throw new ArgumentOutOfRangeException("key", "A valid key must be used.");

			lock (cacheItems)
			{
				while ((cacheItems.Count) >= this.MaxCacheItems)
				{
					ParseListCacheItem item = null;
					string lastKey = null;
					foreach (string key2 in cacheItems.Keys)
					{
						ParseListCacheItem item2 = cacheItems[key2] as ParseListCacheItem;

						if ((item == null) || (item2 != null && item2.LastAccessed < item.LastAccessed))
						{
							item = item2;
							lastKey = key2;
						}
					}
					if (lastKey != null)
					{
						cacheItems.Remove(lastKey);
					}
				}
				ParseListCacheItem item3 = new ParseListCacheItem {Item = parseList, LastAccessed = DateTime.Now.Ticks};
				cacheItems.Add(key.ToLower(), item3);
			}
		}

		public void Clear()
		{
			lock (cacheItems)
			{
				cacheItems.Clear();
			}
		}

		public ParseList Get(string key)
		{
			ParseList list = null;
			lock (cacheItems)
			{
				if (string.IsNullOrEmpty(key))
					throw new ArgumentOutOfRangeException("key", "A valid key must be used.");

				string lookupKey = key.ToLower();
				if (cacheItems.ContainsKey(lookupKey))
				{
					var item = cacheItems[lookupKey] as ParseListCacheItem;
					item.LastAccessed = DateTime.Now.Ticks;
					list = item.Item;
				}
			}
			return list;
		}

		// Properties
		public int MaxCacheItems { get; set; }

		private class ParseListCacheItem
		{
			// Fields
			public ParseList Item;
			public long LastAccessed;
		}
	}

}