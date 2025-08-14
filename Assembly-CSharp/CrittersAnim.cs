using System;
using UnityEngine;

// Token: 0x02000045 RID: 69
[Serializable]
public class CrittersAnim
{
	// Token: 0x06000159 RID: 345 RVA: 0x000091E4 File Offset: 0x000073E4
	public bool IsModified()
	{
		return (this.squashAmount != null && this.squashAmount.length > 1) || (this.forwardOffset != null && this.forwardOffset.length > 1) || (this.horizontalOffset != null && this.horizontalOffset.length > 1) || (this.verticalOffset != null && this.verticalOffset.length > 1);
	}

	// Token: 0x0600015A RID: 346 RVA: 0x0000924D File Offset: 0x0000744D
	public static bool IsModified(CrittersAnim anim)
	{
		return anim != null && anim.IsModified();
	}

	// Token: 0x04000180 RID: 384
	public AnimationCurve squashAmount;

	// Token: 0x04000181 RID: 385
	public AnimationCurve forwardOffset;

	// Token: 0x04000182 RID: 386
	public AnimationCurve horizontalOffset;

	// Token: 0x04000183 RID: 387
	public AnimationCurve verticalOffset;

	// Token: 0x04000184 RID: 388
	public float playSpeed;
}
