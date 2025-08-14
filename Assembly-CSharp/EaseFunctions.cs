using System;
using UnityEngine;

// Token: 0x020005EA RID: 1514
public static class EaseFunctions
{
	// Token: 0x06002536 RID: 9526 RVA: 0x000C845E File Offset: 0x000C665E
	public static float EaseOutPower(float t, float power)
	{
		return 1f - Mathf.Pow(1f - t, power);
	}
}
