using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponEnchantments.Common.Utility
{
	public class DuplicateDictionary<TKey, TValue>
	{
		private List<KeyValuePair<TKey, TValue>> list = new();

		private List<TKey> keys;
		public List<TKey> Keys {
			get {
				Contract.Ensures(Contract.Result<ICollection<TKey>>() != null);
				if (keys == null)
					keys = new List<TKey>();

				return keys;
			}
		}

		private List<TValue> values;
		public List<TValue> Values {
			get {
				Contract.Ensures(Contract.Result<ICollection<TValue>>() != null);
				if (values == null)
					values = new List<TValue>();

				return values;
			}
		}

		public int Count => Keys.Count;

		public bool IsReadOnly => false;

		public TValue this[TKey key] { 
			get {
				int i = Keys.IndexOf(key);
				if (i >= 0)
					return Values[i];

				throw new KeyNotFoundException($"Key Not Found: {key}");
			} 
		}

		public void Add(TKey key, TValue value) {
			Keys.Add(key);
			Values.Add(value);
		}

		public bool ContainsKey(TKey key) {
			return Keys.Contains(key);
		}

		public bool Remove(TKey key) {
			int i = Keys.IndexOf(key);
			if (i >= 0) {
				Keys.RemoveAt(i);
				Values.RemoveAt(i);
				return true;
			}

			throw new KeyNotFoundException($"Key Not Found: {key}");
		}

		public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) {
			int i = Keys.IndexOf(key);
			if (i >= 0) {
				value = Values[i];
				return true;
			}

			value = default(TValue);
			return false;
		}

		public void Add(KeyValuePair<TKey, TValue> item) {
			Add(item.Key, item.Value);
		}

		public void Clear() {
			Keys.Clear();
			Values.Clear();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item) {
			for(int i = 0; i < Keys.Count; i++) {
				if (Keys[i].Equals(item.Key) && Values[i].Equals(item.Value))
					return true;
			}

			return false;
		}
	}
}
