using System;
using System.Collections.Generic;

// Token: 0x02000AB1 RID: 2737
public static class DictValueTypeUtils
{
	// Token: 0x06004238 RID: 16952 RVA: 0x0014D909 File Offset: 0x0014BB09
	public static void TryGetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, out TValue value) where TValue : struct
	{
		if (dict.TryGetValue(key, out value))
		{
			return;
		}
		value = default(TValue);
		dict.Add(key, value);
	}
}
