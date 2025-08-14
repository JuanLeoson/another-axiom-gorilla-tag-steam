using System;
using UnityEngine;

// Token: 0x02000B58 RID: 2904
[Serializable]
public class VoiceLoudnessReactorAnimatorTarget
{
	// Token: 0x04005081 RID: 20609
	public Animator animator;

	// Token: 0x04005082 RID: 20610
	public bool useSmoothedLoudness;

	// Token: 0x04005083 RID: 20611
	public float animatorSpeedToLoudness = 1f;
}
