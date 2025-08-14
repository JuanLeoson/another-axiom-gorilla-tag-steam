using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000569 RID: 1385
[Serializable]
internal class HandTransformFollowOffest
{
	// Token: 0x060021C3 RID: 8643 RVA: 0x000B75E0 File Offset: 0x000B57E0
	internal void UpdatePositionRotation()
	{
		if (this.followTransform == null || this.targetTransforms == null)
		{
			return;
		}
		this.position = this.followTransform.position + this.followTransform.rotation * this.positionOffset * GTPlayer.Instance.scale;
		this.rotation = this.followTransform.rotation * this.rotationOffset;
		foreach (Transform transform in this.targetTransforms)
		{
			transform.position = this.position;
			transform.rotation = this.rotation;
		}
	}

	// Token: 0x04002B31 RID: 11057
	internal Transform followTransform;

	// Token: 0x04002B32 RID: 11058
	[SerializeField]
	private Transform[] targetTransforms;

	// Token: 0x04002B33 RID: 11059
	[SerializeField]
	internal Vector3 positionOffset;

	// Token: 0x04002B34 RID: 11060
	[SerializeField]
	internal Quaternion rotationOffset;

	// Token: 0x04002B35 RID: 11061
	private Vector3 position;

	// Token: 0x04002B36 RID: 11062
	private Quaternion rotation;
}
