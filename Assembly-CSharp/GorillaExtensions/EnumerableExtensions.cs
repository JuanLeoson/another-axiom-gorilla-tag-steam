using System;
using System.Collections.Generic;

namespace GorillaExtensions
{
	// Token: 0x02000E3C RID: 3644
	public static class EnumerableExtensions
	{
		// Token: 0x06005A94 RID: 23188 RVA: 0x001C9D58 File Offset: 0x001C7F58
		public static TValue MinBy<TValue, TKey>(this IEnumerable<TValue> ts, Func<TValue, TKey> keyGetter) where TKey : struct, IComparable<TKey>
		{
			TValue result = default(TValue);
			TKey? tkey = null;
			foreach (TValue tvalue in ts)
			{
				TKey value = keyGetter(tvalue);
				if (tkey == null || value.CompareTo(tkey.Value) < 0)
				{
					result = tvalue;
					tkey = new TKey?(value);
				}
			}
			if (tkey == null)
			{
				throw new ArgumentException("Cannot calculate MinBy on an empty IEnumerable.");
			}
			return result;
		}
	}
}
