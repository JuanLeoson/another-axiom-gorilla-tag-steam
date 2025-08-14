using System;
using System.Collections.Generic;

// Token: 0x02000ABF RID: 2751
public static class LinqUtils
{
	// Token: 0x06004253 RID: 16979 RVA: 0x0014DCC7 File Offset: 0x0014BEC7
	public static IEnumerable<TResult> SelectManyNullSafe<TSource, TResult>(this IEnumerable<TSource> sources, Func<TSource, IEnumerable<TResult>> selector)
	{
		if (sources == null)
		{
			yield break;
		}
		if (selector == null)
		{
			yield break;
		}
		foreach (TSource tsource in sources)
		{
			if (tsource != null)
			{
				IEnumerable<TResult> enumerable = selector(tsource);
				foreach (TResult tresult in enumerable)
				{
					if (tresult != null)
					{
						yield return tresult;
					}
				}
				IEnumerator<TResult> enumerator2 = null;
			}
		}
		IEnumerator<TSource> enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x06004254 RID: 16980 RVA: 0x0014DCDE File Offset: 0x0014BEDE
	public static IEnumerable<TSource> DistinctBy<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
	{
		HashSet<TResult> set = new HashSet<TResult>();
		foreach (TSource tsource in source)
		{
			TResult item = selector(tsource);
			if (set.Add(item))
			{
				yield return tsource;
			}
		}
		IEnumerator<TSource> enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x06004255 RID: 16981 RVA: 0x0014DCF8 File Offset: 0x0014BEF8
	public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
	{
		foreach (T obj in source)
		{
			action(obj);
		}
		return source;
	}

	// Token: 0x06004256 RID: 16982 RVA: 0x0014DD44 File Offset: 0x0014BF44
	public static T[] AsArray<T>(this IEnumerable<T> source)
	{
		return (T[])source;
	}

	// Token: 0x06004257 RID: 16983 RVA: 0x0014DD4C File Offset: 0x0014BF4C
	public static List<T> AsList<T>(this IEnumerable<T> source)
	{
		return (List<T>)source;
	}

	// Token: 0x06004258 RID: 16984 RVA: 0x0014DD54 File Offset: 0x0014BF54
	public static IList<T> Transform<T>(this IList<T> list, Func<T, T> action)
	{
		for (int i = 0; i < list.Count; i++)
		{
			list[i] = action(list[i]);
		}
		return list;
	}

	// Token: 0x06004259 RID: 16985 RVA: 0x0014DD87 File Offset: 0x0014BF87
	public static IEnumerable<T> Self<T>(this T value)
	{
		yield return value;
		yield break;
	}
}
