using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000873 RID: 2163
[Serializable]
public struct GTSturdyEnum<TEnum> : ISerializationCallbackReceiver where TEnum : struct, Enum
{
	// Token: 0x1700052A RID: 1322
	// (get) Token: 0x06003632 RID: 13874 RVA: 0x0011C6A1 File Offset: 0x0011A8A1
	// (set) Token: 0x06003633 RID: 13875 RVA: 0x0011C6A9 File Offset: 0x0011A8A9
	public TEnum Value { readonly get; private set; }

	// Token: 0x06003634 RID: 13876 RVA: 0x0011C6B4 File Offset: 0x0011A8B4
	public static implicit operator GTSturdyEnum<TEnum>(TEnum value)
	{
		return new GTSturdyEnum<TEnum>
		{
			Value = value
		};
	}

	// Token: 0x06003635 RID: 13877 RVA: 0x0011C6D2 File Offset: 0x0011A8D2
	public static implicit operator TEnum(GTSturdyEnum<TEnum> sturdyEnum)
	{
		return sturdyEnum.Value;
	}

	// Token: 0x06003636 RID: 13878 RVA: 0x0011C6DC File Offset: 0x0011A8DC
	public void OnBeforeSerialize()
	{
		EnumData<TEnum> shared = EnumData<TEnum>.Shared;
		if (!shared.IsBitMaskCompatible)
		{
			this.m_stringValuePairs = new GTSturdyEnum<TEnum>.EnumPair[1];
			GTSturdyEnum<TEnum>.EnumPair[] stringValuePairs = this.m_stringValuePairs;
			int num = 0;
			GTSturdyEnum<TEnum>.EnumPair enumPair = default(GTSturdyEnum<TEnum>.EnumPair);
			TEnum value = this.Value;
			enumPair.Name = value.ToString();
			enumPair.FallbackValue = this.Value;
			stringValuePairs[num] = enumPair;
			return;
		}
		long num2 = Convert.ToInt64(this.Value);
		if (num2 == 0L)
		{
			GTSturdyEnum<TEnum>.EnumPair[] array = new GTSturdyEnum<TEnum>.EnumPair[1];
			int num3 = 0;
			GTSturdyEnum<TEnum>.EnumPair enumPair = default(GTSturdyEnum<TEnum>.EnumPair);
			TEnum value = this.Value;
			enumPair.Name = value.ToString();
			enumPair.FallbackValue = this.Value;
			array[num3] = enumPair;
			this.m_stringValuePairs = array;
			return;
		}
		List<GTSturdyEnum<TEnum>.EnumPair> list = new List<GTSturdyEnum<TEnum>.EnumPair>(shared.Values.Length);
		for (int i = 0; i < shared.Values.Length; i++)
		{
			long num4 = shared.LongValues[i];
			if (num4 != 0L && (num2 & num4) == num4)
			{
				TEnum fallbackValue = shared.Values[i];
				list.Add(new GTSturdyEnum<TEnum>.EnumPair
				{
					Name = fallbackValue.ToString(),
					FallbackValue = fallbackValue
				});
			}
		}
		this.m_stringValuePairs = list.ToArray();
	}

	// Token: 0x06003637 RID: 13879 RVA: 0x0011C824 File Offset: 0x0011AA24
	public void OnAfterDeserialize()
	{
		EnumData<TEnum> shared = EnumData<TEnum>.Shared;
		if (this.m_stringValuePairs == null || this.m_stringValuePairs.Length == 0)
		{
			if (shared.IsBitMaskCompatible)
			{
				this.Value = (TEnum)((object)Enum.ToObject(typeof(TEnum), 0L));
				return;
			}
			this.Value = default(TEnum);
			return;
		}
		else
		{
			if (shared.IsBitMaskCompatible)
			{
				long num = 0L;
				foreach (GTSturdyEnum<TEnum>.EnumPair enumPair in this.m_stringValuePairs)
				{
					TEnum key;
					long num2;
					if (shared.NameToEnum.TryGetValue(enumPair.Name, out key))
					{
						num |= shared.EnumToLong[key];
					}
					else if (shared.EnumToLong.TryGetValue(enumPair.FallbackValue, out num2))
					{
						num |= num2;
					}
				}
				this.Value = (TEnum)((object)Enum.ToObject(typeof(TEnum), num));
				return;
			}
			GTSturdyEnum<TEnum>.EnumPair enumPair2 = this.m_stringValuePairs[0];
			TEnum value;
			if (shared.NameToEnum.TryGetValue(enumPair2.Name, out value))
			{
				this.Value = value;
				return;
			}
			this.Value = enumPair2.FallbackValue;
			return;
		}
	}

	// Token: 0x0400432B RID: 17195
	[SerializeField]
	private GTSturdyEnum<TEnum>.EnumPair[] m_stringValuePairs;

	// Token: 0x02000874 RID: 2164
	[Serializable]
	private struct EnumPair
	{
		// Token: 0x0400432C RID: 17196
		public string Name;

		// Token: 0x0400432D RID: 17197
		public TEnum FallbackValue;
	}
}
