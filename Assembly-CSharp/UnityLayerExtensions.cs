using System;
using UnityEngine;

// Token: 0x02000258 RID: 600
public static class UnityLayerExtensions
{
	// Token: 0x06000DFD RID: 3581 RVA: 0x00055706 File Offset: 0x00053906
	public static int ToLayerMask(this UnityLayer self)
	{
		return 1 << (int)self;
	}

	// Token: 0x06000DFE RID: 3582 RVA: 0x0005570E File Offset: 0x0005390E
	public static int ToLayerIndex(this UnityLayer self)
	{
		return (int)self;
	}

	// Token: 0x06000DFF RID: 3583 RVA: 0x00055711 File Offset: 0x00053911
	public static bool IsOnLayer(this GameObject obj, UnityLayer layer)
	{
		return obj.layer == (int)layer;
	}

	// Token: 0x06000E00 RID: 3584 RVA: 0x0005571C File Offset: 0x0005391C
	public static void SetLayer(this GameObject obj, UnityLayer layer)
	{
		obj.layer = (int)layer;
	}
}
