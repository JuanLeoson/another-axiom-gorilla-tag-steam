using System;
using System.Collections;
using System.Collections.Generic;

namespace LitJson
{
	// Token: 0x02000BB2 RID: 2994
	internal class OrderedDictionaryEnumerator : IDictionaryEnumerator, IEnumerator
	{
		// Token: 0x170006FA RID: 1786
		// (get) Token: 0x06004861 RID: 18529 RVA: 0x00160FCA File Offset: 0x0015F1CA
		public object Current
		{
			get
			{
				return this.Entry;
			}
		}

		// Token: 0x170006FB RID: 1787
		// (get) Token: 0x06004862 RID: 18530 RVA: 0x00160FD8 File Offset: 0x0015F1D8
		public DictionaryEntry Entry
		{
			get
			{
				KeyValuePair<string, JsonData> keyValuePair = this.list_enumerator.Current;
				return new DictionaryEntry(keyValuePair.Key, keyValuePair.Value);
			}
		}

		// Token: 0x170006FC RID: 1788
		// (get) Token: 0x06004863 RID: 18531 RVA: 0x00161004 File Offset: 0x0015F204
		public object Key
		{
			get
			{
				KeyValuePair<string, JsonData> keyValuePair = this.list_enumerator.Current;
				return keyValuePair.Key;
			}
		}

		// Token: 0x170006FD RID: 1789
		// (get) Token: 0x06004864 RID: 18532 RVA: 0x00161024 File Offset: 0x0015F224
		public object Value
		{
			get
			{
				KeyValuePair<string, JsonData> keyValuePair = this.list_enumerator.Current;
				return keyValuePair.Value;
			}
		}

		// Token: 0x06004865 RID: 18533 RVA: 0x00161044 File Offset: 0x0015F244
		public OrderedDictionaryEnumerator(IEnumerator<KeyValuePair<string, JsonData>> enumerator)
		{
			this.list_enumerator = enumerator;
		}

		// Token: 0x06004866 RID: 18534 RVA: 0x00161053 File Offset: 0x0015F253
		public bool MoveNext()
		{
			return this.list_enumerator.MoveNext();
		}

		// Token: 0x06004867 RID: 18535 RVA: 0x00161060 File Offset: 0x0015F260
		public void Reset()
		{
			this.list_enumerator.Reset();
		}

		// Token: 0x040051CA RID: 20938
		private IEnumerator<KeyValuePair<string, JsonData>> list_enumerator;
	}
}
