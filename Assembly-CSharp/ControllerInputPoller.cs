using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

// Token: 0x0200056D RID: 1389
public class ControllerInputPoller : MonoBehaviour
{
	// Token: 0x17000365 RID: 869
	// (get) Token: 0x060021DC RID: 8668 RVA: 0x000B7C9E File Offset: 0x000B5E9E
	// (set) Token: 0x060021DD RID: 8669 RVA: 0x000B7CA6 File Offset: 0x000B5EA6
	public GorillaControllerType controllerType { get; private set; }

	// Token: 0x060021DE RID: 8670 RVA: 0x000B7CAF File Offset: 0x000B5EAF
	private void Awake()
	{
		if (ControllerInputPoller.instance == null)
		{
			ControllerInputPoller.instance = this;
			return;
		}
		if (ControllerInputPoller.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060021DF RID: 8671 RVA: 0x000B7CE4 File Offset: 0x000B5EE4
	public static void AddUpdateCallback(Action callback)
	{
		if (!ControllerInputPoller.instance.didModifyOnUpdate)
		{
			ControllerInputPoller.instance.onUpdateNext.Clear();
			ControllerInputPoller.instance.onUpdateNext.AddRange(ControllerInputPoller.instance.onUpdate);
			ControllerInputPoller.instance.didModifyOnUpdate = true;
		}
		ControllerInputPoller.instance.onUpdateNext.Add(callback);
	}

	// Token: 0x060021E0 RID: 8672 RVA: 0x000B7D4C File Offset: 0x000B5F4C
	public static void RemoveUpdateCallback(Action callback)
	{
		if (!ControllerInputPoller.instance.didModifyOnUpdate)
		{
			ControllerInputPoller.instance.onUpdateNext.Clear();
			ControllerInputPoller.instance.onUpdateNext.AddRange(ControllerInputPoller.instance.onUpdate);
			ControllerInputPoller.instance.didModifyOnUpdate = true;
		}
		ControllerInputPoller.instance.onUpdateNext.Remove(callback);
	}

	// Token: 0x060021E1 RID: 8673 RVA: 0x000B7DB8 File Offset: 0x000B5FB8
	private void Update()
	{
		InputDevice inputDevice = this.leftControllerDevice;
		if (!this.leftControllerDevice.isValid)
		{
			this.leftControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
			if (this.leftControllerDevice.isValid)
			{
				this.controllerType = GorillaControllerType.OCULUS_DEFAULT;
				if (this.leftControllerDevice.name.ToLower().Contains("knuckles"))
				{
					this.controllerType = GorillaControllerType.INDEX;
				}
				Debug.Log(string.Format("Found left controller: {0} ControllerType: {1}", this.leftControllerDevice.name, this.controllerType));
			}
		}
		InputDevice inputDevice2 = this.rightControllerDevice;
		if (!this.rightControllerDevice.isValid)
		{
			this.rightControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
		}
		InputDevice inputDevice3 = this.headDevice;
		if (!this.headDevice.isValid)
		{
			this.headDevice = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
		}
		InputDevice inputDevice4 = this.leftControllerDevice;
		InputDevice inputDevice5 = this.rightControllerDevice;
		InputDevice inputDevice6 = this.headDevice;
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out this.leftControllerPrimaryButton);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out this.leftControllerSecondaryButton);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out this.leftControllerPrimaryButtonTouch);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.secondaryTouch, out this.leftControllerSecondaryButtonTouch);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.grip, out this.leftControllerGripFloat);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.trigger, out this.leftControllerIndexFloat);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.devicePosition, out this.leftControllerPosition);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out this.leftControllerRotation);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out this.leftControllerPrimary2DAxis);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.triggerButton, out this.leftControllerTriggerButton);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out this.rightControllerPrimaryButton);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out this.rightControllerSecondaryButton);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out this.rightControllerPrimaryButtonTouch);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.secondaryTouch, out this.rightControllerSecondaryButtonTouch);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.grip, out this.rightControllerGripFloat);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.trigger, out this.rightControllerIndexFloat);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.devicePosition, out this.rightControllerPosition);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out this.rightControllerRotation);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out this.rightControllerPrimary2DAxis);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.triggerButton, out this.rightControllerTriggerButton);
		this.leftControllerPrimaryButton = SteamVR_Actions.gorillaTag_LeftPrimaryClick.GetState(SteamVR_Input_Sources.LeftHand);
		this.leftControllerSecondaryButton = SteamVR_Actions.gorillaTag_LeftSecondaryClick.GetState(SteamVR_Input_Sources.LeftHand);
		this.leftControllerPrimaryButtonTouch = SteamVR_Actions.gorillaTag_LeftPrimaryTouch.GetState(SteamVR_Input_Sources.LeftHand);
		this.leftControllerSecondaryButtonTouch = SteamVR_Actions.gorillaTag_LeftSecondaryTouch.GetState(SteamVR_Input_Sources.LeftHand);
		this.leftControllerGripFloat = SteamVR_Actions.gorillaTag_LeftGripFloat.GetAxis(SteamVR_Input_Sources.LeftHand);
		this.leftControllerIndexFloat = SteamVR_Actions.gorillaTag_LeftTriggerFloat.GetAxis(SteamVR_Input_Sources.LeftHand);
		this.leftControllerTriggerButton = SteamVR_Actions.gorillaTag_LeftTriggerClick.GetState(SteamVR_Input_Sources.LeftHand);
		this.rightControllerPrimaryButton = SteamVR_Actions.gorillaTag_RightPrimaryClick.GetState(SteamVR_Input_Sources.RightHand);
		this.rightControllerSecondaryButton = SteamVR_Actions.gorillaTag_RightSecondaryClick.GetState(SteamVR_Input_Sources.RightHand);
		this.rightControllerPrimaryButtonTouch = SteamVR_Actions.gorillaTag_RightPrimaryTouch.GetState(SteamVR_Input_Sources.RightHand);
		this.rightControllerSecondaryButtonTouch = SteamVR_Actions.gorillaTag_RightSecondaryTouch.GetState(SteamVR_Input_Sources.RightHand);
		this.rightControllerGripFloat = SteamVR_Actions.gorillaTag_RightGripFloat.GetAxis(SteamVR_Input_Sources.RightHand);
		this.rightControllerIndexFloat = SteamVR_Actions.gorillaTag_RightTriggerFloat.GetAxis(SteamVR_Input_Sources.RightHand);
		this.rightControllerTriggerButton = SteamVR_Actions.gorillaTag_RightTriggerClick.GetState(SteamVR_Input_Sources.RightHand);
		this.rightControllerPrimary2DAxis = SteamVR_Actions.gorillaTag_RightJoystick2DAxis.GetAxis(SteamVR_Input_Sources.RightHand);
		this.headDevice.TryGetFeatureValue(CommonUsages.devicePosition, out this.headPosition);
		this.headDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out this.headRotation);
		if (this.controllerType == GorillaControllerType.OCULUS_DEFAULT)
		{
			this.CalculateGrabState(this.leftControllerGripFloat, ref this.leftGrab, ref this.leftGrabRelease, ref this.leftGrabMomentary, ref this.leftGrabReleaseMomentary, 0.75f, 0.65f);
			this.CalculateGrabState(this.rightControllerGripFloat, ref this.rightGrab, ref this.rightGrabRelease, ref this.rightGrabMomentary, ref this.rightGrabReleaseMomentary, 0.75f, 0.65f);
		}
		else if (this.controllerType == GorillaControllerType.INDEX)
		{
			this.CalculateGrabState(this.leftControllerGripFloat, ref this.leftGrab, ref this.leftGrabRelease, ref this.leftGrabMomentary, ref this.leftGrabReleaseMomentary, 0.1f, 0.01f);
			this.CalculateGrabState(this.rightControllerGripFloat, ref this.rightGrab, ref this.rightGrabRelease, ref this.rightGrabMomentary, ref this.rightGrabReleaseMomentary, 0.1f, 0.01f);
		}
		if (this.didModifyOnUpdate)
		{
			List<Action> list = this.onUpdateNext;
			List<Action> list2 = this.onUpdate;
			this.onUpdate = list;
			this.onUpdateNext = list2;
			this.didModifyOnUpdate = false;
		}
		foreach (Action action in this.onUpdate)
		{
			action();
		}
	}

	// Token: 0x060021E2 RID: 8674 RVA: 0x000B82C8 File Offset: 0x000B64C8
	private void CalculateGrabState(float grabValue, ref bool grab, ref bool grabRelease, ref bool grabMomentary, ref bool grabReleaseMomentary, float grabThreshold, float grabReleaseThreshold)
	{
		bool flag = grabValue >= grabThreshold;
		bool flag2 = grabValue <= grabReleaseThreshold;
		grabMomentary = (flag && flag != grab);
		grabReleaseMomentary = (flag2 && flag2 != grabRelease);
		grab = flag;
		grabRelease = flag2;
	}

	// Token: 0x060021E3 RID: 8675 RVA: 0x000B830F File Offset: 0x000B650F
	public static bool GetGrab(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftGrab;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightGrab;
	}

	// Token: 0x060021E4 RID: 8676 RVA: 0x000B8334 File Offset: 0x000B6534
	public static bool GetGrabRelease(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftGrabRelease;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightGrabRelease;
	}

	// Token: 0x060021E5 RID: 8677 RVA: 0x000B8359 File Offset: 0x000B6559
	public static bool GetGrabMomentary(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftGrabMomentary;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightGrabMomentary;
	}

	// Token: 0x060021E6 RID: 8678 RVA: 0x000B837E File Offset: 0x000B657E
	public static bool GetGrabReleaseMomentary(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftGrabReleaseMomentary;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightGrabReleaseMomentary;
	}

	// Token: 0x060021E7 RID: 8679 RVA: 0x000B83A3 File Offset: 0x000B65A3
	public static Vector2 Primary2DAxis(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerPrimary2DAxis;
		}
		return ControllerInputPoller.instance.rightControllerPrimary2DAxis;
	}

	// Token: 0x060021E8 RID: 8680 RVA: 0x000B83C2 File Offset: 0x000B65C2
	public static bool PrimaryButtonPress(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerPrimaryButton;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightControllerPrimaryButton;
	}

	// Token: 0x060021E9 RID: 8681 RVA: 0x000B83E7 File Offset: 0x000B65E7
	public static bool SecondaryButtonPress(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerSecondaryButton;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightControllerSecondaryButton;
	}

	// Token: 0x060021EA RID: 8682 RVA: 0x000B840C File Offset: 0x000B660C
	public static bool PrimaryButtonTouch(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerPrimaryButtonTouch;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightControllerPrimaryButtonTouch;
	}

	// Token: 0x060021EB RID: 8683 RVA: 0x000B8431 File Offset: 0x000B6631
	public static bool SecondaryButtonTouch(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerSecondaryButtonTouch;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightControllerSecondaryButtonTouch;
	}

	// Token: 0x060021EC RID: 8684 RVA: 0x000B8456 File Offset: 0x000B6656
	public static float GripFloat(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerGripFloat;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightControllerGripFloat;
		}
		return 0f;
	}

	// Token: 0x060021ED RID: 8685 RVA: 0x000B847F File Offset: 0x000B667F
	public static float TriggerFloat(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerIndexFloat;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightControllerIndexFloat;
		}
		return 0f;
	}

	// Token: 0x060021EE RID: 8686 RVA: 0x000B84A8 File Offset: 0x000B66A8
	public static float TriggerTouch(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerIndexTouch;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightControllerIndexTouch;
		}
		return 0f;
	}

	// Token: 0x060021EF RID: 8687 RVA: 0x000B84D1 File Offset: 0x000B66D1
	public static Vector3 DevicePosition(XRNode node)
	{
		if (node == XRNode.Head)
		{
			return ControllerInputPoller.instance.headPosition;
		}
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerPosition;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightControllerPosition;
		}
		return Vector3.zero;
	}

	// Token: 0x060021F0 RID: 8688 RVA: 0x000B850B File Offset: 0x000B670B
	public static Quaternion DeviceRotation(XRNode node)
	{
		if (node == XRNode.Head)
		{
			return ControllerInputPoller.instance.headRotation;
		}
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerRotation;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightControllerRotation;
		}
		return Quaternion.identity;
	}

	// Token: 0x060021F1 RID: 8689 RVA: 0x000B8548 File Offset: 0x000B6748
	public static bool PositionValid(XRNode node)
	{
		if (node == XRNode.Head)
		{
			InputDevice inputDevice = ControllerInputPoller.instance.headDevice;
			return ControllerInputPoller.instance.headDevice.isValid;
		}
		if (node == XRNode.LeftHand)
		{
			InputDevice inputDevice2 = ControllerInputPoller.instance.leftControllerDevice;
			return ControllerInputPoller.instance.leftControllerDevice.isValid;
		}
		if (node == XRNode.RightHand)
		{
			InputDevice inputDevice3 = ControllerInputPoller.instance.rightControllerDevice;
			return ControllerInputPoller.instance.rightControllerDevice.isValid;
		}
		return false;
	}

	// Token: 0x04002B54 RID: 11092
	[OnEnterPlay_SetNull]
	public static volatile ControllerInputPoller instance;

	// Token: 0x04002B55 RID: 11093
	public float leftControllerIndexFloat;

	// Token: 0x04002B56 RID: 11094
	public float leftControllerGripFloat;

	// Token: 0x04002B57 RID: 11095
	public float rightControllerIndexFloat;

	// Token: 0x04002B58 RID: 11096
	public float rightControllerGripFloat;

	// Token: 0x04002B59 RID: 11097
	public float leftControllerIndexTouch;

	// Token: 0x04002B5A RID: 11098
	public float rightControllerIndexTouch;

	// Token: 0x04002B5B RID: 11099
	public float rightStickLRFloat;

	// Token: 0x04002B5C RID: 11100
	public Vector3 leftControllerPosition;

	// Token: 0x04002B5D RID: 11101
	public Vector3 rightControllerPosition;

	// Token: 0x04002B5E RID: 11102
	public Vector3 headPosition;

	// Token: 0x04002B5F RID: 11103
	public Quaternion leftControllerRotation;

	// Token: 0x04002B60 RID: 11104
	public Quaternion rightControllerRotation;

	// Token: 0x04002B61 RID: 11105
	public Quaternion headRotation;

	// Token: 0x04002B62 RID: 11106
	public InputDevice leftControllerDevice;

	// Token: 0x04002B63 RID: 11107
	public InputDevice rightControllerDevice;

	// Token: 0x04002B64 RID: 11108
	public InputDevice headDevice;

	// Token: 0x04002B65 RID: 11109
	public bool leftControllerPrimaryButton;

	// Token: 0x04002B66 RID: 11110
	public bool leftControllerSecondaryButton;

	// Token: 0x04002B67 RID: 11111
	public bool rightControllerPrimaryButton;

	// Token: 0x04002B68 RID: 11112
	public bool rightControllerSecondaryButton;

	// Token: 0x04002B69 RID: 11113
	public bool leftControllerPrimaryButtonTouch;

	// Token: 0x04002B6A RID: 11114
	public bool leftControllerSecondaryButtonTouch;

	// Token: 0x04002B6B RID: 11115
	public bool rightControllerPrimaryButtonTouch;

	// Token: 0x04002B6C RID: 11116
	public bool rightControllerSecondaryButtonTouch;

	// Token: 0x04002B6D RID: 11117
	public bool leftControllerTriggerButton;

	// Token: 0x04002B6E RID: 11118
	public bool rightControllerTriggerButton;

	// Token: 0x04002B6F RID: 11119
	public bool leftGrab;

	// Token: 0x04002B70 RID: 11120
	public bool leftGrabRelease;

	// Token: 0x04002B71 RID: 11121
	public bool rightGrab;

	// Token: 0x04002B72 RID: 11122
	public bool rightGrabRelease;

	// Token: 0x04002B73 RID: 11123
	public bool leftGrabMomentary;

	// Token: 0x04002B74 RID: 11124
	public bool leftGrabReleaseMomentary;

	// Token: 0x04002B75 RID: 11125
	public bool rightGrabMomentary;

	// Token: 0x04002B76 RID: 11126
	public bool rightGrabReleaseMomentary;

	// Token: 0x04002B78 RID: 11128
	public Vector2 leftControllerPrimary2DAxis;

	// Token: 0x04002B79 RID: 11129
	public Vector2 rightControllerPrimary2DAxis;

	// Token: 0x04002B7A RID: 11130
	private List<Action> onUpdate = new List<Action>();

	// Token: 0x04002B7B RID: 11131
	private List<Action> onUpdateNext = new List<Action>();

	// Token: 0x04002B7C RID: 11132
	private bool didModifyOnUpdate;
}
