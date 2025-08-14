using System;
using System.Collections.Generic;

// Token: 0x02000AB0 RID: 2736
public static class DictRefTypeUtils
{
	// Token: 0x06004237 RID: 16951 RVA: 0x0014D8D7 File Offset: 0x0014BAD7
	public static void TryGetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, out TValue value) where TValue : class, new()
	{
		if (dict.TryGetValue(key, out value) && value != null)
		{
			return;
		}
		value = Activator.CreateInstance<TValue>();
		dict.Add(key, value);
	}
}
