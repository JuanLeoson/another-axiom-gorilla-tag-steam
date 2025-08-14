using System;
using UnityEngine;

// Token: 0x02000382 RID: 898
public class BouncingBallMgr : MonoBehaviour
{
	// Token: 0x0600152F RID: 5423 RVA: 0x000733EC File Offset: 0x000715EC
	private void Update()
	{
		if (!this.ballGrabbed && OVRInput.GetDown(this.actionBtn, OVRInput.Controller.Active))
		{
			this.currentBall = Object.Instantiate<GameObject>(this.ball, this.rightControllerPivot.transform.position, Quaternion.identity);
			this.currentBall.transform.parent = this.rightControllerPivot.transform;
			this.ballGrabbed = true;
		}
		if (this.ballGrabbed && OVRInput.GetUp(this.actionBtn, OVRInput.Controller.Active))
		{
			this.currentBall.transform.parent = null;
			Vector3 position = this.currentBall.transform.position;
			Vector3 vel = this.trackingspace.rotation * OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
			Vector3 localControllerAngularVelocity = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch);
			this.currentBall.GetComponent<BouncingBallLogic>().Release(position, vel, localControllerAngularVelocity);
			this.ballGrabbed = false;
		}
	}

	// Token: 0x04001CCA RID: 7370
	[SerializeField]
	private Transform trackingspace;

	// Token: 0x04001CCB RID: 7371
	[SerializeField]
	private GameObject rightControllerPivot;

	// Token: 0x04001CCC RID: 7372
	[SerializeField]
	private OVRInput.RawButton actionBtn;

	// Token: 0x04001CCD RID: 7373
	[SerializeField]
	private GameObject ball;

	// Token: 0x04001CCE RID: 7374
	private GameObject currentBall;

	// Token: 0x04001CCF RID: 7375
	private bool ballGrabbed;
}
