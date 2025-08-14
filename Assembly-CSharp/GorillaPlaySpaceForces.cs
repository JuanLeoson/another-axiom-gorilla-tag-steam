using System;
using UnityEngine;

// Token: 0x020004D8 RID: 1240
public class GorillaPlaySpaceForces : MonoBehaviour
{
	// Token: 0x06001E60 RID: 7776 RVA: 0x000A13F0 File Offset: 0x0009F5F0
	private void Start()
	{
		this.playspaceRigidbody = base.GetComponent<Rigidbody>();
		this.leftHandRigidbody = this.leftHand.GetComponent<Rigidbody>();
		this.leftHandCollider = this.leftHand.GetComponent<Collider>();
		this.rightHandRigidbody = this.rightHand.GetComponent<Rigidbody>();
		this.rightHandCollider = this.rightHand.GetComponent<Collider>();
	}

	// Token: 0x06001E61 RID: 7777 RVA: 0x000A144D File Offset: 0x0009F64D
	private void FixedUpdate()
	{
		if (Time.time >= 0.1f)
		{
			this.bodyCollider.transform.position = this.headsetTransform.position + this.bodyColliderOffset;
		}
	}

	// Token: 0x04002704 RID: 9988
	public GameObject rightHand;

	// Token: 0x04002705 RID: 9989
	public GameObject leftHand;

	// Token: 0x04002706 RID: 9990
	public Collider bodyCollider;

	// Token: 0x04002707 RID: 9991
	private Collider leftHandCollider;

	// Token: 0x04002708 RID: 9992
	private Collider rightHandCollider;

	// Token: 0x04002709 RID: 9993
	public Transform rightHandTransform;

	// Token: 0x0400270A RID: 9994
	public Transform leftHandTransform;

	// Token: 0x0400270B RID: 9995
	private Rigidbody leftHandRigidbody;

	// Token: 0x0400270C RID: 9996
	private Rigidbody rightHandRigidbody;

	// Token: 0x0400270D RID: 9997
	public Vector3 bodyColliderOffset;

	// Token: 0x0400270E RID: 9998
	public float forceConstant;

	// Token: 0x0400270F RID: 9999
	private Vector3 lastLeftHandPosition;

	// Token: 0x04002710 RID: 10000
	private Vector3 lastRightHandPosition;

	// Token: 0x04002711 RID: 10001
	private Rigidbody playspaceRigidbody;

	// Token: 0x04002712 RID: 10002
	public Transform headsetTransform;
}
