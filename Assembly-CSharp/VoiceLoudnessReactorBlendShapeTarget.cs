using System;
using UnityEngine;

// Token: 0x02000B53 RID: 2899
[Serializable]
public class VoiceLoudnessReactorBlendShapeTarget
{
	// Token: 0x04005062 RID: 20578
	public SkinnedMeshRenderer SkinnedMeshRenderer;

	// Token: 0x04005063 RID: 20579
	public int BlendShapeIndex;

	// Token: 0x04005064 RID: 20580
	public float minValue;

	// Token: 0x04005065 RID: 20581
	public float maxValue = 1f;

	// Token: 0x04005066 RID: 20582
	public bool UseSmoothedLoudness;
}
