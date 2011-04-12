using System;
using System.Collections.Generic;

namespace CodeSoda.Impression
{
	public interface IPropertyBag
	{
		void Add(string key, object value);
		object this[string key] { get; set; }
		bool ContainsKey(string key);
		void Remove(string key);
		int Count { get; }
		string[] Keys { get; }
	}

	public class PropertyBag : Dictionary<string, object>, IPropertyBag
	{
		public PropertyBag() { }

		public PropertyBag(IPropertyBag bag)
			: base((IDictionary<string, object>)bag) { }

		public new void Add(string key, object value)
		{
			this[key.ToLower()] = value;
		}

		public new object this[string key]
		{
			get { return this.ContainsKey(key.ToLower()) ? base[key.ToLower()] : null; }
			set { base[key.ToLower()] = value; }
		}

		public new bool ContainsKey(string key)
		{
			return base.ContainsKey(key.ToLower());
		}

		public new void Remove(string key) {
			base.Remove(key.ToLower());
		}

		public new string[] Keys {
			get {
				var keys = new string[base.Keys.Count];
				int i = 0;
				foreach(var key in base.Keys) {
					keys[i++] = key;
				}
				return keys;
			}
		}

	}
}
