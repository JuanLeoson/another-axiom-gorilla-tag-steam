using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000410 RID: 1040
[Serializable]
public class VRMapIndex : VRMap
{
	// Token: 0x06001931 RID: 6449 RVA: 0x00087EBC File Offset: 0x000860BC
	public override void Initialize()
	{
		this.closedAngle1Quat = Quaternion.Euler(this.closedAngle1);
		this.closedAngle2Quat = Quaternion.Euler(this.closedAngle2);
		this.closedAngle3Quat = Quaternion.Euler(this.closedAngle3);
		this.startingAngle1Quat = Quaternion.Euler(this.startingAngle1);
		this.startingAngle2Quat = Quaternion.Euler(this.startingAngle2);
		this.startingAngle3Quat = Quaternion.Euler(this.startingAngle3);
	}

	// Token: 0x06001932 RID: 6450 RVA: 0x00087F30 File Offset: 0x00086130
	public override void MapMyFinger(float lerpValue)
	{
		this.calcT = 0f;
		this.triggerValue = ControllerInputPoller.TriggerFloat(this.vrTargetNode);
		this.triggerTouch = ControllerInputPoller.TriggerTouch(this.vrTargetNode);
		this.calcT = 0.1f * this.triggerTouch;
		this.calcT += 0.9f * this.triggerValue;
		this.LerpFinger(lerpValue, false);
	}

	// Token: 0x06001933 RID: 6451 RVA: 0x00087FA0 File Offset: 0x000861A0
	public override void LerpFinger(float lerpValue, bool isOther)
	{
		if (isOther)
		{
			this.currentAngle1 = Mathf.Lerp(this.currentAngle1, this.calcT, lerpValue);
			this.currentAngle2 = Mathf.Lerp(this.currentAngle2, this.calcT, lerpValue);
			this.currentAngle3 = Mathf.Lerp(this.currentAngle3, this.calcT, lerpValue);
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
			}
			this.myTempInt = (int)(this.currentAngle3 * 10.1f);
			if (this.myTempInt != this.lastAngle3)
			{
				this.lastAngle3 = this.myTempInt;
				this.fingerBone3.localRotation = this.angle3Table[this.lastAngle3];
				return;
			}
		}
		else
		{
			this.fingerBone1.localRotation = Quaternion.Lerp(this.fingerBone1.localRotation, Quaternion.Lerp(this.startingAngle1Quat, this.closedAngle1Quat, this.calcT), lerpValue);
			this.fingerBone2.localRotation = Quaternion.Lerp(this.fingerBone2.localRotation, Quaternion.Lerp(this.startingAngle2Quat, this.closedAngle2Quat, this.calcT), lerpValue);
			this.fingerBone3.localRotation = Quaternion.Lerp(this.fingerBone3.localRotation, Quaternion.Lerp(this.startingAngle3Quat, this.closedAngle3Quat, this.calcT), lerpValue);
		}
	}

	// Token: 0x0400214A RID: 8522
	public InputFeatureUsage inputAxis;

	// Token: 0x0400214B RID: 8523
	public float triggerTouch;

	// Token: 0x0400214C RID: 8524
	public float triggerValue;

	// Token: 0x0400214D RID: 8525
	public Transform fingerBone1;

	// Token: 0x0400214E RID: 8526
	public Transform fingerBone2;

	// Token: 0x0400214F RID: 8527
	public Transform fingerBone3;

	// Token: 0x04002150 RID: 8528
	public Vector3 closedAngle1;

	// Token: 0x04002151 RID: 8529
	public Vector3 closedAngle2;

	// Token: 0x04002152 RID: 8530
	public Vector3 closedAngle3;

	// Token: 0x04002153 RID: 8531
	public Vector3 startingAngle1;

	// Token: 0x04002154 RID: 8532
	public Vector3 startingAngle2;

	// Token: 0x04002155 RID: 8533
	public Vector3 startingAngle3;

	// Token: 0x04002156 RID: 8534
	public Quaternion closedAngle1Quat;

	// Token: 0x04002157 RID: 8535
	public Quaternion closedAngle2Quat;

	// Token: 0x04002158 RID: 8536
	public Quaternion closedAngle3Quat;

	// Token: 0x04002159 RID: 8537
	public Quaternion startingAngle1Quat;

	// Token: 0x0400215A RID: 8538
	public Quaternion startingAngle2Quat;

	// Token: 0x0400215B RID: 8539
	public Quaternion startingAngle3Quat;

	// Token: 0x0400215C RID: 8540
	private int lastAngle1;

	// Token: 0x0400215D RID: 8541
	private int lastAngle2;

	// Token: 0x0400215E RID: 8542
	private int lastAngle3;

	// Token: 0x0400215F RID: 8543
	private InputDevice myInputDevice;

	// Token: 0x04002160 RID: 8544
	public Quaternion[] angle1Table;

	// Token: 0x04002161 RID: 8545
	public Quaternion[] angle2Table;

	// Token: 0x04002162 RID: 8546
	public Quaternion[] angle3Table;

	// Token: 0x04002163 RID: 8547
	private float currentAngle1;

	// Token: 0x04002164 RID: 8548
	private float currentAngle2;

	// Token: 0x04002165 RID: 8549
	private float currentAngle3;

	// Token: 0x04002166 RID: 8550
	private int myTempInt;
}
