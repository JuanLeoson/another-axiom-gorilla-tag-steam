using System;
using UnityEngine;

// Token: 0x02000737 RID: 1847
public class GorillaUITransformFollow : MonoBehaviour
{
	// Token: 0x06002E32 RID: 11826 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Start()
	{
	}

	// Token: 0x06002E33 RID: 11827 RVA: 0x000F4EF4 File Offset: 0x000F30F4
	private void LateUpdate()
	{
		if (this.doesMove)
		{
			base.transform.rotation = this.transformToFollow.rotation;
			base.transform.position = this.transformToFollow.position + this.transformToFollow.rotation * this.offset;
		}
	}

	// Token: 0x04003A0F RID: 14863
	public Transform transformToFollow;

	// Token: 0x04003A10 RID: 14864
	public Vector3 offset;

	// Token: 0x04003A11 RID: 14865
	public bool doesMove;
}
