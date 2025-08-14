using System;
using UnityEngine;

// Token: 0x02000ACB RID: 2763
public static class PoolUtils
{
	// Token: 0x060042B3 RID: 17075 RVA: 0x0014F734 File Offset: 0x0014D934
	public static int GameObjHashCode(GameObject obj)
	{
		return obj.tag.GetHashCode();
	}
}
