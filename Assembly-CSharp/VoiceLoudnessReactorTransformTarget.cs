using System;
using UnityEngine;

// Token: 0x02000B54 RID: 2900
[Serializable]
public class VoiceLoudnessReactorTransformTarget
{
	// Token: 0x17000689 RID: 1673
	// (get) Token: 0x06004579 RID: 17785 RVA: 0x0015B355 File Offset: 0x00159555
	// (set) Token: 0x0600457A RID: 17786 RVA: 0x0015B35D File Offset: 0x0015955D
	public Vector3 Initial
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

	// Token: 0x04005067 RID: 20583
	public Transform transform;

	// Token: 0x04005068 RID: 20584
	private Vector3 initial;

	// Token: 0x04005069 RID: 20585
	public Vector3 Max = Vector3.one;

	// Token: 0x0400506A RID: 20586
	public float Scale = 1f;

	// Token: 0x0400506B RID: 20587
	public bool UseSmoothedLoudness;
}
