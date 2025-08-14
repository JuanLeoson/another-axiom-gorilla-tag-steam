using System;
using UnityEngine;

// Token: 0x020004D0 RID: 1232
public class GorillaBodyPhysics : MonoBehaviour
{
	// Token: 0x06001E48 RID: 7752 RVA: 0x000A1054 File Offset: 0x0009F254
	private void FixedUpdate()
	{
		this.bodyCollider.transform.position = this.headsetTransform.position + this.bodyColliderOffset;
	}

	// Token: 0x040026C6 RID: 9926
	public GameObject bodyCollider;

	// Token: 0x040026C7 RID: 9927
	public Vector3 bodyColliderOffset;

	// Token: 0x040026C8 RID: 9928
	public Transform headsetTransform;
}
