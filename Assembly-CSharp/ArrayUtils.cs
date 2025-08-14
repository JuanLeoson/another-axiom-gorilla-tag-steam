using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

// Token: 0x02000A90 RID: 2704
public static class ArrayUtils
{
	// Token: 0x0600419D RID: 16797 RVA: 0x0014B142 File Offset: 0x00149342
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int BinarySearch<T>(this T[] array, T value) where T : IComparable<T>
	{
		return Array.BinarySearch<T>(array, 0, array.Length, value);
	}

	// Token: 0x0600419E RID: 16798 RVA: 0x0014B14F File Offset: 0x0014934F
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNullOrEmpty<T>(this T[] array)
	{
		return array == null || array.Length == 0;
	}

	// Token: 0x0600419F RID: 16799 RVA: 0x0014B15B File Offset: 0x0014935B
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNullOrEmpty<T>(this List<T> list)
	{
		return list == null || list.Count == 0;
	}

	// Token: 0x060041A0 RID: 16800 RVA: 0x0014B16C File Offset: 0x0014936C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Swap<T>(this T[] array, int from, int to)
	{
		T t = array[from];
		T t2 = array[to];
		array[to] = t;
		array[from] = t2;
	}

	// Token: 0x060041A1 RID: 16801 RVA: 0x0014B1A4 File Offset: 0x001493A4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Swap<T>(this List<T> list, int from, int to)
	{
		T value = list[from];
		T value2 = list[to];
		list[to] = value;
		list[from] = value2;
	}

	// Token: 0x060041A2 RID: 16802 RVA: 0x0014B1E0 File Offset: 0x001493E0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T[] Clone<T>(T[] source)
	{
		if (source == null)
		{
			return null;
		}
		if (source.Length == 0)
		{
			return Array.Empty<T>();
		}
		T[] array = new T[source.Length];
		for (int i = 0; i < source.Length; i++)
		{
			array[i] = source[i];
		}
		return array;
	}

	// Token: 0x060041A3 RID: 16803 RVA: 0x0014B222 File Offset: 0x00149422
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static List<T> Clone<T>(List<T> source)
	{
		if (source == null)
		{
			return null;
		}
		if (source.Count == 0)
		{
			return new List<T>();
		}
		return new List<T>(source);
	}

	// Token: 0x060041A4 RID: 16804 RVA: 0x0014B240 File Offset: 0x00149440
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOfRef<T>(this T[] array, T value) where T : class
	{
		if (array == null || array.Length == 0)
		{
			return -1;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == value)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060041A5 RID: 16805 RVA: 0x0014B27C File Offset: 0x0014947C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOfRef<T>(this List<T> list, T value) where T : class
	{
		if (list == null || list.Count == 0)
		{
			return -1;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == value)
			{
				return i;
			}
		}
		return -1;
	}
}
