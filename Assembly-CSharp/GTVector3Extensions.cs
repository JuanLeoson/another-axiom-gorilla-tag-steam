using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020001ED RID: 493
public static class GTVector3Extensions
{
	// Token: 0x06000BC3 RID: 3011 RVA: 0x000408AA File Offset: 0x0003EAAA
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 X_Z(this Vector3 vector)
	{
		return new Vector3(vector.x, 0f, vector.z);
	}

	// Token: 0x06000BC4 RID: 3012 RVA: 0x000408C2 File Offset: 0x0003EAC2
	public static Vector3 Sum(this IEnumerable<Vector3> vecs)
	{
		return vecs.Aggregate(Vector3.zero, (Vector3 current, Vector3 vec) => current + vec);
	}

	// Token: 0x06000BC5 RID: 3013 RVA: 0x000408F0 File Offset: 0x0003EAF0
	public static Vector3 Average(this IEnumerable<Vector3> vecs)
	{
		List<Vector3> list = vecs.ToList<Vector3>();
		return list.Sum() / (float)list.Count;
	}
}
