using System;
using UnityEngine;

// Token: 0x02000B55 RID: 2901
[Serializable]
public class VoiceLoudnessReactorTransformRotationTarget
{
	// Token: 0x1700068A RID: 1674
	// (get) Token: 0x0600457C RID: 17788 RVA: 0x0015B384 File Offset: 0x00159584
	// (set) Token: 0x0600457D RID: 17789 RVA: 0x0015B38C File Offset: 0x0015958C
	public Quaternion Initial
	{
		get
		{
			return this.initial;
		}
		set
		{
			this.initial = value;
		}
	}

	// Token: 0x0400506C RID: 20588
	public Transform transform;

	// Token: 0x0400506D RID: 20589
	private Quaternion initial;

	// Token: 0x0400506E RID: 20590
	public Quaternion Max = Quaternion.identity;

	// Token: 0x0400506F RID: 20591
	public float Scale = 1f;

	// Token: 0x04005070 RID: 20592
	public bool UseSmoothedLoudness;
}
