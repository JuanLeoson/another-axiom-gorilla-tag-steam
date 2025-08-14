using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020002BF RID: 703
public static class NetInput
{
	// Token: 0x1700019F RID: 415
	// (get) Token: 0x06001066 RID: 4198 RVA: 0x0005DEDC File Offset: 0x0005C0DC
	public static VRRig LocalPlayerVRRig
	{
		get
		{
			if (NetInput._localPlayerVRRig == null)
			{
				NetInput._localPlayerVRRig = GameObject.Find("Local VRRig").GetComponentInChildren<VRRig>();
			}
			return NetInput._localPlayerVRRig;
		}
	}

	// Token: 0x06001067 RID: 4199 RVA: 0x0005DF04 File Offset: 0x0005C104
	public static NetworkedInput GetInput()
	{
		NetworkedInput result = default(NetworkedInput);
		if (NetInput.LocalPlayerVRRig == null)
		{
			return result;
		}
		result.headRot_LS = NetInput.LocalPlayerVRRig.head.rigTarget.localRotation;
		result.rightHandPos_LS = NetInput.LocalPlayerVRRig.rightHand.rigTarget.localPosition;
		result.rightHandRot_LS = NetInput.LocalPlayerVRRig.rightHand.rigTarget.localRotation;
		result.leftHandPos_LS = NetInput.LocalPlayerVRRig.leftHand.rigTarget.localPosition;
		result.leftHandRot_LS = NetInput.LocalPlayerVRRig.leftHand.rigTarget.localRotation;
		result.handPoseData = NetInput.LocalPlayerVRRig.ReturnHandPosition();
		result.rootPosition = NetInput.LocalPlayerVRRig.transform.position;
		result.rootRotation = NetInput.LocalPlayerVRRig.transform.rotation;
		result.leftThumbTouch = (ControllerInputPoller.PrimaryButtonTouch(XRNode.LeftHand) || ControllerInputPoller.SecondaryButtonTouch(XRNode.LeftHand));
		result.leftThumbPress = (ControllerInputPoller.PrimaryButtonPress(XRNode.LeftHand) || ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand));
		result.leftIndexValue = ControllerInputPoller.TriggerFloat(XRNode.LeftHand);
		result.leftMiddleValue = ControllerInputPoller.GripFloat(XRNode.LeftHand);
		result.rightThumbTouch = (ControllerInputPoller.PrimaryButtonTouch(XRNode.RightHand) || ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand));
		result.rightThumbPress = (ControllerInputPoller.PrimaryButtonPress(XRNode.RightHand) || ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand));
		result.rightIndexValue = ControllerInputPoller.TriggerFloat(XRNode.RightHand);
		result.rightMiddleValue = ControllerInputPoller.GripFloat(XRNode.RightHand);
		result.scale = NetInput.LocalPlayerVRRig.scaleFactor;
		return result;
	}

	// Token: 0x040018A2 RID: 6306
	private static VRRig _localPlayerVRRig;
}
