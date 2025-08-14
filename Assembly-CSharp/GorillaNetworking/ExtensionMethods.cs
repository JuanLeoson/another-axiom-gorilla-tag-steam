using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000DB1 RID: 3505
	public static class ExtensionMethods
	{
		// Token: 0x06005705 RID: 22277 RVA: 0x001B0170 File Offset: 0x001AE370
		public static void SafeInvoke<T>(this Action<T> action, T data)
		{
			try
			{
				if (action != null)
				{
					action(data);
				}
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Failure invoking action: {0}", arg));
			}
		}

		// Token: 0x06005706 RID: 22278 RVA: 0x001B01AC File Offset: 0x001AE3AC
		public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
		{
			if (dict.ContainsKey(key))
			{
				dict[key] = value;
				return;
			}
			dict.Add(key, value);
		}
	}
}
