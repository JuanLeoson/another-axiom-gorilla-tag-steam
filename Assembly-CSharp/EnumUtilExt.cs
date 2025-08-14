using System;

// Token: 0x02000869 RID: 2153
public static class EnumUtilExt
{
	// Token: 0x06003615 RID: 13845 RVA: 0x0011BF53 File Offset: 0x0011A153
	public static string GetName<TEnum>(this TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToName[e];
	}

	// Token: 0x06003616 RID: 13846 RVA: 0x0011BF77 File Offset: 0x0011A177
	public static int GetIndex<TEnum>(this TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToIndex[e];
	}

	// Token: 0x06003617 RID: 13847 RVA: 0x0011BF9B File Offset: 0x0011A19B
	public static long GetLongValue<TEnum>(this TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToLong[e];
	}

	// Token: 0x06003618 RID: 13848 RVA: 0x0011C09C File Offset: 0x0011A29C
	public static TEnum GetNextValue<TEnum>(this TEnum e) where TEnum : struct, Enum
	{
		EnumData<TEnum> shared = EnumData<TEnum>.Shared;
		return shared.Values[shared.EnumToIndex[e] + 1 % shared.Values.Length];
	}
}
