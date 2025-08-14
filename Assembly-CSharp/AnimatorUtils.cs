using System;
using UnityEngine;

// Token: 0x02000A8E RID: 2702
public static class AnimatorUtils
{
	// Token: 0x06004195 RID: 16789 RVA: 0x0014B0F8 File Offset: 0x001492F8
	public static void ResetToEntryState(this Animator a)
	{
		if (a == null)
		{
			return;
		}
		a.Rebind();
		a.Update(0f);
	}
}
