using System;
using UnityEngine;

// Token: 0x02000B57 RID: 2903
[Serializable]
public class VoiceLoudnessReactorGameObjectEnableTarget
{
	// Token: 0x0400507C RID: 20604
	public GameObject GameObject;

	// Token: 0x0400507D RID: 20605
	public float Threshold;

	// Token: 0x0400507E RID: 20606
	public bool TurnOnAtThreshhold = true;

	// Token: 0x0400507F RID: 20607
	public bool UseSmoothedLoudness;

	// Token: 0x04005080 RID: 20608
	public float Scale = 1f;
}
