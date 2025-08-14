using System;
using System.Collections.Generic;

// Token: 0x02000868 RID: 2152
public static class EnumUtil
{
	// Token: 0x06003604 RID: 13828 RVA: 0x0011BF20 File Offset: 0x0011A120
	public static string[] GetNames<TEnum>() where TEnum : struct, Enum
	{
		return ArrayUtils.Clone<string>(EnumData<TEnum>.Shared.Names);
	}

	// Token: 0x06003605 RID: 13829 RVA: 0x0011BF31 File Offset: 0x0011A131
	public static TEnum[] GetValues<TEnum>() where TEnum : struct, Enum
	{
		return ArrayUtils.Clone<TEnum>(EnumData<TEnum>.Shared.Values);
	}

	// Token: 0x06003606 RID: 13830 RVA: 0x0011BF42 File Offset: 0x0011A142
	public static long[] GetLongValues<TEnum>() where TEnum : struct, Enum
	{
		return ArrayUtils.Clone<long>(EnumData<TEnum>.Shared.LongValues);
	}

	// Token: 0x06003607 RID: 13831 RVA: 0x0011BF53 File Offset: 0x0011A153
	public static string EnumToName<TEnum>(TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToName[e];
	}

	// Token: 0x06003608 RID: 13832 RVA: 0x0011BF65 File Offset: 0x0011A165
	public static TEnum NameToEnum<TEnum>(string n) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.NameToEnum[n];
	}

	// Token: 0x06003609 RID: 13833 RVA: 0x0011BF77 File Offset: 0x0011A177
	public static int EnumToIndex<TEnum>(TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToIndex[e];
	}

	// Token: 0x0600360A RID: 13834 RVA: 0x0011BF89 File Offset: 0x0011A189
	public static TEnum IndexToEnum<TEnum>(int i) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.IndexToEnum[i];
	}

	// Token: 0x0600360B RID: 13835 RVA: 0x0011BF9B File Offset: 0x0011A19B
	public static long EnumToLong<TEnum>(TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToLong[e];
	}

	// Token: 0x0600360C RID: 13836 RVA: 0x0011BFAD File Offset: 0x0011A1AD
	public static TEnum LongToEnum<TEnum>(long l) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.LongToEnum[l];
	}

	// Token: 0x0600360D RID: 13837 RVA: 0x0011BFBF File Offset: 0x0011A1BF
	public static TEnum GetValue<TEnum>(int index) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.Values[index];
	}

	// Token: 0x0600360E RID: 13838 RVA: 0x0011BF77 File Offset: 0x0011A177
	public static int GetIndex<TEnum>(TEnum value) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToIndex[value];
	}

	// Token: 0x0600360F RID: 13839 RVA: 0x0011BF53 File Offset: 0x0011A153
	public static string GetName<TEnum>(TEnum value) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToName[value];
	}

	// Token: 0x06003610 RID: 13840 RVA: 0x0011BF65 File Offset: 0x0011A165
	public static TEnum GetValue<TEnum>(string name) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.NameToEnum[name];
	}

	// Token: 0x06003611 RID: 13841 RVA: 0x0011BF9B File Offset: 0x0011A19B
	public static long GetLongValue<TEnum>(TEnum value) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToLong[value];
	}

	// Token: 0x06003612 RID: 13842 RVA: 0x0011BFAD File Offset: 0x0011A1AD
	public static TEnum GetValue<TEnum>(long longValue) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.LongToEnum[longValue];
	}

	// Token: 0x06003613 RID: 13843 RVA: 0x0011BFD1 File Offset: 0x0011A1D1
	public static TEnum[] SplitBitmask<TEnum>(TEnum bitmask) where TEnum : struct, Enum
	{
		return EnumUtil.SplitBitmask<TEnum>(Convert.ToInt64(bitmask));
	}

	// Token: 0x06003614 RID: 13844 RVA: 0x0011BFE4 File Offset: 0x0011A1E4
	public static TEnum[] SplitBitmask<TEnum>(long bitmaskLong) where TEnum : struct, Enum
	{
		EnumData<TEnum> shared = EnumData<TEnum>.Shared;
		if (!shared.IsBitMaskCompatible)
		{
			throw new ArgumentException("The enum type " + typeof(TEnum).Name + " is not bitmask-compatible.");
		}
		if (bitmaskLong == 0L)
		{
			return new TEnum[]
			{
				(TEnum)((object)Enum.ToObject(typeof(TEnum), 0L))
			};
		}
		List<TEnum> list = new List<TEnum>(shared.Values.Length);
		for (int i = 0; i < shared.Values.Length; i++)
		{
			TEnum item = shared.Values[i];
			long num = shared.LongValues[i];
			if (num != 0L && (bitmaskLong & num) == num)
			{
				list.Add(item);
			}
		}
		return list.ToArray();
	}
}
