using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000AFE RID: 2814
public static class UnityObjectUtils
{
	// Token: 0x060043D6 RID: 17366 RVA: 0x00154BD4 File Offset: 0x00152DD4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T AsNull<T>(this T obj) where T : Object
	{
		if (obj == null)
		{
			return default(T);
		}
		if (!(obj == null))
		{
			return obj;
		}
		return default(T);
	}

	// Token: 0x060043D7 RID: 17367 RVA: 0x0004092B File Offset: 0x0003EB2B
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SafeDestroy(this Object obj)
	{
		Object.Destroy(obj);
	}
}
