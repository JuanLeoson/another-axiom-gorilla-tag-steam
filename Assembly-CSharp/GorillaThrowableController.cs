using System;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

// Token: 0x020007EC RID: 2028
public class GorillaThrowableController : MonoBehaviour
{
	// Token: 0x060032C9 RID: 13001 RVA: 0x00108625 File Offset: 0x00106825
	protected void Awake()
	{
		this.gorillaThrowableLayerMask = LayerMask.GetMask(new string[]
		{
			"GorillaThrowable"
		});
	}

	// Token: 0x060032CA RID: 13002 RVA: 0x00108640 File Offset: 0x00106840
	private void LateUpdate()
	{
		if (this.testCanGrab)
		{
			this.testCanGrab = false;
			this.CanGrabAnObject(this.rightHandController, out this.returnCollider);
			Debug.Log(this.returnCollider.gameObject, this.returnCollider.gameObject);
		}
		if (this.leftHandIsGrabbing)
		{
			if (this.CheckIfHandHasReleased(XRNode.LeftHand))
			{
				if (this.leftHandGrabbedObject != null)
				{
					this.leftHandGrabbedObject.ThrowThisThingo();
					this.leftHandGrabbedObject = null;
				}
				this.leftHandIsGrabbing = false;
			}
		}
		else if (this.CheckIfHandHasGrabbed(XRNode.LeftHand))
		{
			this.leftHandIsGrabbing = true;
			if (this.CanGrabAnObject(this.leftHandController, out this.returnCollider))
			{
				this.leftHandGrabbedObject = this.returnCollider.GetComponent<GorillaThrowable>();
				this.leftHandGrabbedObject.Grabbed(this.leftHandController);
			}
		}
		if (this.rightHandIsGrabbing)
		{
			if (this.CheckIfHandHasReleased(XRNode.RightHand))
			{
				if (this.rightHandGrabbedObject != null)
				{
					this.rightHandGrabbedObject.ThrowThisThingo();
					this.rightHandGrabbedObject = null;
				}
				this.rightHandIsGrabbing = false;
				return;
			}
		}
		else if (this.CheckIfHandHasGrabbed(XRNode.RightHand))
		{
			this.rightHandIsGrabbing = true;
			if (this.CanGrabAnObject(this.rightHandController, out this.returnCollider))
			{
				this.rightHandGrabbedObject = this.returnCollider.GetComponent<GorillaThrowable>();
				this.rightHandGrabbedObject.Grabbed(this.rightHandController);
			}
		}
	}

	// Token: 0x060032CB RID: 13003 RVA: 0x0010878C File Offset: 0x0010698C
	private bool CheckIfHandHasReleased(XRNode node)
	{
		this.inputDevice = InputDevices.GetDeviceAtXRNode(node);
		this.triggerValue = ((node == XRNode.LeftHand) ? SteamVR_Actions.gorillaTag_LeftTriggerFloat.GetAxis(SteamVR_Input_Sources.LeftHand) : SteamVR_Actions.gorillaTag_RightTriggerFloat.GetAxis(SteamVR_Input_Sources.RightHand));
		if (this.triggerValue < 0.75f)
		{
			this.triggerValue = ((node == XRNode.LeftHand) ? SteamVR_Actions.gorillaTag_LeftGripFloat.GetAxis(SteamVR_Input_Sources.LeftHand) : SteamVR_Actions.gorillaTag_RightGripFloat.GetAxis(SteamVR_Input_Sources.RightHand));
			if (this.triggerValue < 0.75f)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060032CC RID: 13004 RVA: 0x00108808 File Offset: 0x00106A08
	private bool CheckIfHandHasGrabbed(XRNode node)
	{
		this.inputDevice = InputDevices.GetDeviceAtXRNode(node);
		this.triggerValue = ((node == XRNode.LeftHand) ? SteamVR_Actions.gorillaTag_LeftTriggerFloat.GetAxis(SteamVR_Input_Sources.LeftHand) : SteamVR_Actions.gorillaTag_RightTriggerFloat.GetAxis(SteamVR_Input_Sources.RightHand));
		if (this.triggerValue > 0.75f)
		{
			return true;
		}
		this.triggerValue = ((node == XRNode.LeftHand) ? SteamVR_Actions.gorillaTag_LeftGripFloat.GetAxis(SteamVR_Input_Sources.LeftHand) : SteamVR_Actions.gorillaTag_RightGripFloat.GetAxis(SteamVR_Input_Sources.RightHand));
		return this.triggerValue > 0.75f;
	}

	// Token: 0x060032CD RID: 13005 RVA: 0x00108884 File Offset: 0x00106A84
	private bool CanGrabAnObject(Transform handTransform, out Collider returnCollider)
	{
		this.magnitude = 100f;
		returnCollider = null;
		Debug.Log("trying:");
		if (Physics.OverlapSphereNonAlloc(handTransform.position, this.handRadius, this.colliders, this.gorillaThrowableLayerMask) > 0)
		{
			Debug.Log("found something!");
			this.minCollider = this.colliders[0];
			foreach (Collider collider in this.colliders)
			{
				if (collider != null)
				{
					Debug.Log("found this", collider);
					if ((collider.transform.position - handTransform.position).magnitude < this.magnitude)
					{
						this.minCollider = collider;
						this.magnitude = (collider.transform.position - handTransform.position).magnitude;
					}
				}
			}
			returnCollider = this.minCollider;
			return true;
		}
		return false;
	}

	// Token: 0x060032CE RID: 13006 RVA: 0x0010896D File Offset: 0x00106B6D
	public void GrabbableObjectHover(bool isLeft)
	{
		GorillaTagger.Instance.StartVibration(isLeft, this.hoverVibrationStrength, this.hoverVibrationDuration);
	}

	// Token: 0x04003FA9 RID: 16297
	public Transform leftHandController;

	// Token: 0x04003FAA RID: 16298
	public Transform rightHandController;

	// Token: 0x04003FAB RID: 16299
	public bool leftHandIsGrabbing;

	// Token: 0x04003FAC RID: 16300
	public bool rightHandIsGrabbing;

	// Token: 0x04003FAD RID: 16301
	public GorillaThrowable leftHandGrabbedObject;

	// Token: 0x04003FAE RID: 16302
	public GorillaThrowable rightHandGrabbedObject;

	// Token: 0x04003FAF RID: 16303
	public float hoverVibrationStrength = 0.25f;

	// Token: 0x04003FB0 RID: 16304
	public float hoverVibrationDuration = 0.05f;

	// Token: 0x04003FB1 RID: 16305
	public float handRadius = 0.05f;

	// Token: 0x04003FB2 RID: 16306
	private InputDevice rightDevice;

	// Token: 0x04003FB3 RID: 16307
	private InputDevice leftDevice;

	// Token: 0x04003FB4 RID: 16308
	private InputDevice inputDevice;

	// Token: 0x04003FB5 RID: 16309
	private float triggerValue;

	// Token: 0x04003FB6 RID: 16310
	private bool boolVar;

	// Token: 0x04003FB7 RID: 16311
	private Collider[] colliders = new Collider[10];

	// Token: 0x04003FB8 RID: 16312
	private Collider minCollider;

	// Token: 0x04003FB9 RID: 16313
	private Collider returnCollider;

	// Token: 0x04003FBA RID: 16314
	private float magnitude;

	// Token: 0x04003FBB RID: 16315
	public bool testCanGrab;

	// Token: 0x04003FBC RID: 16316
	private int gorillaThrowableLayerMask;
}
