using System;
using System.Collections.Generic;

// Token: 0x02000AB2 RID: 2738
public static class DictArrayUtils
{
	// Token: 0x06004239 RID: 16953 RVA: 0x0014D92A File Offset: 0x0014BB2A
	public static void TryGetOrAddList<TKey, TValue>(this Dictionary<TKey, List<TValue>> dict, TKey key, out List<TValue> list, int capacity)
	{
		if (dict.TryGetValue(key, out list) && list != null)
		{
			return;
		}
		list = new List<TValue>(capacity);
		dict.Add(key, list);
	}

	// Token: 0x0600423A RID: 16954 RVA: 0x0014D94C File Offset: 0x0014BB4C
	public static void TryGetOrAddArray<TKey, TValue>(this Dictionary<TKey, TValue[]> dict, TKey key, out TValue[] array, int size)
	{
		if (dict.TryGetValue(key, out array) && array != null)
		{
			return;
		}
		array = new TValue[size];
		dict.Add(key, array);
	}
}
