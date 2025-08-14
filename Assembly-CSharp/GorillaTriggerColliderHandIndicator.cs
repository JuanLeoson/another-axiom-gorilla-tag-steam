using System;
using UnityEngine;

// Token: 0x020007F1 RID: 2033
public class GorillaTriggerColliderHandIndicator : MonoBehaviour
{
	// Token: 0x060032E2 RID: 13026 RVA: 0x00108E4C File Offset: 0x0010704C
	private void Update()
	{
		this.currentVelocity = (base.transform.position - this.lastPosition) / Time.deltaTime;
		this.lastPosition = base.transform.position;
	}

	// Token: 0x060032E3 RID: 13027 RVA: 0x00108E85 File Offset: 0x00107085
	private void OnTriggerEnter(Collider other)
	{
		if (this.throwableController != null)
		{
			this.throwableController.GrabbableObjectHover(this.isLeftHand);
		}
	}

	// Token: 0x04003FCF RID: 16335
	public Vector3 currentVelocity;

	// Token: 0x04003FD0 RID: 16336
	public Vector3 lastPosition = Vector3.zero;

	// Token: 0x04003FD1 RID: 16337
	public bool isLeftHand;

	// Token: 0x04003FD2 RID: 16338
	public GorillaThrowableController throwableController;
}
