using System;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000205 RID: 517
[Serializable]
public struct GorillaPosRotConstraint
{
	// Token: 0x04000F15 RID: 3861
	[Tooltip("Transform that should be moved, rotated, and scaled to match the `source` Transform in world space.")]
	public Transform follower;

	// Token: 0x04000F16 RID: 3862
	[Tooltip("Bone that `follower` should match. Set to `None` to assign a specific Transform within the same prefab.")]
	public GTHardCodedBones.SturdyEBone sourceGorillaBone;

	// Token: 0x04000F17 RID: 3863
	[Tooltip("Transform that `follower` should match. This is overridden at runtime if `sourceGorillaBone` is not `None`. If set in inspector, then it should be only set to a child of the the prefab this component belongs to.")]
	public Transform source;

	// Token: 0x04000F18 RID: 3864
	public string sourceRelativePath;

	// Token: 0x04000F19 RID: 3865
	[Tooltip("Offset to be applied to the follower's position.")]
	public Vector3 positionOffset;

	// Token: 0x04000F1A RID: 3866
	[Tooltip("Offset to be applied to the follower's rotation.")]
	public Quaternion rotationOffset;
}
