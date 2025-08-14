using System;
using UnityEngine;

// Token: 0x020004E2 RID: 1250
public class GorillaWalkingGrab : MonoBehaviour
{
	// Token: 0x06001E76 RID: 7798 RVA: 0x000A15ED File Offset: 0x0009F7ED
	private void Start()
	{
		this.thisRigidbody = base.gameObject.GetComponent<Rigidbody>();
		this.positionHistory = new Vector3[this.historySteps];
		this.historyIndex = 0;
	}

	// Token: 0x06001E77 RID: 7799 RVA: 0x000A1618 File Offset: 0x0009F818
	private void FixedUpdate()
	{
		this.historyIndex++;
		if (this.historyIndex >= this.historySteps)
		{
			this.historyIndex = 0;
		}
		this.positionHistory[this.historyIndex] = this.handToStickTo.transform.position;
		this.thisRigidbody.MovePosition(this.handToStickTo.transform.position);
		base.transform.rotation = this.handToStickTo.transform.rotation;
	}

	// Token: 0x06001E78 RID: 7800 RVA: 0x00002076 File Offset: 0x00000276
	private bool MakeJump()
	{
		return false;
	}

	// Token: 0x06001E79 RID: 7801 RVA: 0x000A16A0 File Offset: 0x0009F8A0
	private void OnCollisionStay(Collision collision)
	{
		if (!this.MakeJump())
		{
			Vector3 b = Vector3.ProjectOnPlane(this.positionHistory[(this.historyIndex != 0) ? (this.historyIndex - 1) : (this.historySteps - 1)] - this.handToStickTo.transform.position, collision.GetContact(0).normal);
			Vector3 b2 = this.thisRigidbody.transform.position - this.handToStickTo.transform.position;
			this.playspaceRigidbody.MovePosition(this.playspaceRigidbody.transform.position + b - b2);
		}
	}

	// Token: 0x04002724 RID: 10020
	public GameObject handToStickTo;

	// Token: 0x04002725 RID: 10021
	public float ratioToUse;

	// Token: 0x04002726 RID: 10022
	public float forceMultiplier;

	// Token: 0x04002727 RID: 10023
	public int historySteps;

	// Token: 0x04002728 RID: 10024
	public Rigidbody playspaceRigidbody;

	// Token: 0x04002729 RID: 10025
	private Rigidbody thisRigidbody;

	// Token: 0x0400272A RID: 10026
	private Vector3 lastPosition;

	// Token: 0x0400272B RID: 10027
	private Vector3 maybeLastPositionIDK;

	// Token: 0x0400272C RID: 10028
	private Vector3[] positionHistory;

	// Token: 0x0400272D RID: 10029
	private int historyIndex;
}
