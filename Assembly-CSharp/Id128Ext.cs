using System;
using UnityEngine;

// Token: 0x02000877 RID: 2167
public static class Id128Ext
{
	// Token: 0x06003660 RID: 13920 RVA: 0x0011CE78 File Offset: 0x0011B078
	public static Id128 ToId128(this Hash128 h)
	{
		return new Id128(h);
	}

	// Token: 0x06003661 RID: 13921 RVA: 0x0011CE70 File Offset: 0x0011B070
	public static Id128 ToId128(this Guid g)
	{
		return new Id128(g);
	}
}
