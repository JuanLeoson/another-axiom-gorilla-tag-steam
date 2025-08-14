using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000412 RID: 1042
[Serializable]
public class VRMapThumb : VRMap
{
	// Token: 0x06001939 RID: 6457 RVA: 0x000883FC File Offset: 0x000865FC
	public override void Initialize()
	{
		this.closedAngle1Quat = Quaternion.Euler(this.closedAngle1);
		this.closedAngle2Quat = Quaternion.Euler(this.closedAngle2);
		this.startingAngle1Quat = Quaternion.Euler(this.startingAngle1);
		this.startingAngle2Quat = Quaternion.Euler(this.startingAngle2);
	}

	// Token: 0x0600193A RID: 6458 RVA: 0x00088450 File Offset: 0x00086650
	public override void MapMyFinger(float lerpValue)
	{
		this.calcT = 0f;
		if (this.vrTargetNode == XRNode.LeftHand)
		{
			this.primaryButtonPress = ControllerInputPoller.instance.leftControllerPrimaryButton;
			this.primaryButtonTouch = ControllerInputPoller.instance.leftControllerPrimaryButtonTouch;
			this.secondaryButtonPress = ControllerInputPoller.instance.leftControllerSecondaryButton;
			this.secondaryButtonTouch = ControllerInputPoller.instance.leftControllerSecondaryButtonTouch;
		}
		else
		{
			this.primaryButtonPress = ControllerInputPoller.instance.rightControllerPrimaryButton;
			this.primaryButtonTouch = ControllerInputPoller.instance.rightControllerPrimaryButtonTouch;
			this.secondaryButtonPress = ControllerInputPoller.instance.rightControllerSecondaryButton;
			this.secondaryButtonTouch = ControllerInputPoller.instance.rightControllerSecondaryButtonTouch;
		}
		if (this.primaryButtonPress || this.secondaryButtonPress)
		{
			this.calcT = 1f;
		}
		else if (this.primaryButtonTouch || this.secondaryButtonTouch)
		{
			this.calcT = 0.1f;
		}
		this.LerpFinger(lerpValue, false);
	}

	// Token: 0x0600193B RID: 6459 RVA: 0x00088544 File Offset: 0x00086744
	public override void LerpFinger(float lerpValue, bool isOther)
	{
		if (isOther)
		{
			this.currentAngle1 = Mathf.Lerp(this.currentAngle1, this.calcT, lerpValue);
			this.currentAngle2 = Mathf.Lerp(this.currentAngle2, this.calcT, lerpValue);
			this.myTempInt = (int)(this.currentAngle1 * 10.1f);
			if (this.myTempInt != this.lastAngle1)
			{
				this.lastAngle1 = this.myTempInt;
				this.fingerBone1.localRotation = this.angle1Table[this.lastAngle1];
			}
			this.myTempInt = (int)(this.currentAngle2 * 10.1f);
			if (this.myTempInt != this.lastAngle2)
			{
				this.lastAngle2 = this.myTempInt;
				this.fingerBone2.localRotation = this.angle2Table[this.lastAngle2];
				return;
			}
		}
		else
		{
			this.fingerBone1.localRotation = Quaternion.Lerp(this.fingerBone1.localRotation, Quaternion.Lerp(this.startingAngle1Quat, this.closedAngle1Quat, this.calcT), lerpValue);
			this.fingerBone2.localRotation = Quaternion.Lerp(this.fingerBone2.localRotation, Quaternion.Lerp(this.startingAngle2Quat, this.closedAngle2Quat, this.calcT), lerpValue);
		}
	}

	// Token: 0x04002183 RID: 8579
	public InputFeatureUsage inputAxis;

	// Token: 0x04002184 RID: 8580
	public bool primaryButtonTouch;

	// Token: 0x04002185 RID: 8581
	public bool primaryButtonPress;

	// Token: 0x04002186 RID: 8582
	public bool secondaryButtonTouch;

	// Token: 0x04002187 RID: 8583
	public bool secondaryButtonPress;

	// Token: 0x04002188 RID: 8584
	public Transform fingerBone1;

	// Token: 0x04002189 RID: 8585
	public Transform fingerBone2;

	// Token: 0x0400218A RID: 8586
	public Vector3 closedAngle1;

	// Token: 0x0400218B RID: 8587
	public Vector3 closedAngle2;

	// Token: 0x0400218C RID: 8588
	public Vector3 startingAngle1;

	// Token: 0x0400218D RID: 8589
	public Vector3 startingAngle2;

	// Token: 0x0400218E RID: 8590
	public Quaternion closedAngle1Quat;

	// Token: 0x0400218F RID: 8591
	public Quaternion closedAngle2Quat;

	// Token: 0x04002190 RID: 8592
	public Quaternion startingAngle1Quat;

	// Token: 0x04002191 RID: 8593
	public Quaternion startingAngle2Quat;

	// Token: 0x04002192 RID: 8594
	public Quaternion[] angle1Table;

	// Token: 0x04002193 RID: 8595
	public Quaternion[] angle2Table;

	// Token: 0x04002194 RID: 8596
	private float currentAngle1;

	// Token: 0x04002195 RID: 8597
	private float currentAngle2;

	// Token: 0x04002196 RID: 8598
	private int lastAngle1;

	// Token: 0x04002197 RID: 8599
	private int lastAngle2;

	// Token: 0x04002198 RID: 8600
	private InputDevice tempDevice;

	// Token: 0x04002199 RID: 8601
	private int myTempInt;
}
