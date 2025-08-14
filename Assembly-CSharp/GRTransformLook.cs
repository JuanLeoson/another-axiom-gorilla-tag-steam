using System;
using UnityEngine;

// Token: 0x0200068E RID: 1678
public class GRTransformLook : MonoBehaviour
{
	// Token: 0x0600291B RID: 10523 RVA: 0x000DD584 File Offset: 0x000DB784
	private void Awake()
	{
		if (this.followPlayer)
		{
			this.lookTarget = Camera.main.transform;
		}
	}

	// Token: 0x0600291C RID: 10524 RVA: 0x000DD59E File Offset: 0x000DB79E
	private void LateUpdate()
	{
		base.transform.LookAt(this.lookTarget);
		base.transform.rotation *= Quaternion.Euler(this.offsetRotation);
	}

	// Token: 0x04003507 RID: 13575
	public bool followPlayer;

	// Token: 0x04003508 RID: 13576
	public Transform lookTarget;

	// Token: 0x04003509 RID: 13577
	public Vector3 offsetRotation;
}
