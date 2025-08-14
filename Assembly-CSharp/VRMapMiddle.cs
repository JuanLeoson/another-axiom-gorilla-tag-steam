using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000411 RID: 1041
[Serializable]
public class VRMapMiddle : VRMap
{
	// Token: 0x06001935 RID: 6453 RVA: 0x0008817C File Offset: 0x0008637C
	public override void Initialize()
	{
		this.closedAngle1Quat = Quaternion.Euler(this.closedAngle1);
		this.closedAngle2Quat = Quaternion.Euler(this.closedAngle2);
		this.closedAngle3Quat = Quaternion.Euler(this.closedAngle3);
		this.startingAngle1Quat = Quaternion.Euler(this.startingAngle1);
		this.startingAngle2Quat = Quaternion.Euler(this.startingAngle2);
		this.startingAngle3Quat = Quaternion.Euler(this.startingAngle3);
	}

	// Token: 0x06001936 RID: 6454 RVA: 0x000881EF File Offset: 0x000863EF
	public override void MapMyFinger(float lerpValue)
	{
		this.calcT = 0f;
		this.gripValue = ControllerInputPoller.GripFloat(this.vrTargetNode);
		this.calcT = 1f * this.gripValue;
		this.LerpFinger(lerpValue, false);
	}

	// Token: 0x06001937 RID: 6455 RVA: 0x00088228 File Offset: 0x00086428
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

	// Token: 0x04002167 RID: 8551
	public InputFeatureUsage inputAxis;

	// Token: 0x04002168 RID: 8552
	public float gripValue;

	// Token: 0x04002169 RID: 8553
	public Transform fingerBone1;

	// Token: 0x0400216A RID: 8554
	public Transform fingerBone2;

	// Token: 0x0400216B RID: 8555
	public Transform fingerBone3;

	// Token: 0x0400216C RID: 8556
	public Vector3 closedAngle1;

	// Token: 0x0400216D RID: 8557
	public Vector3 closedAngle2;

	// Token: 0x0400216E RID: 8558
	public Vector3 closedAngle3;

	// Token: 0x0400216F RID: 8559
	public Vector3 startingAngle1;

	// Token: 0x04002170 RID: 8560
	public Vector3 startingAngle2;

	// Token: 0x04002171 RID: 8561
	public Vector3 startingAngle3;

	// Token: 0x04002172 RID: 8562
	public Quaternion closedAngle1Quat;

	// Token: 0x04002173 RID: 8563
	public Quaternion closedAngle2Quat;

	// Token: 0x04002174 RID: 8564
	public Quaternion closedAngle3Quat;

	// Token: 0x04002175 RID: 8565
	public Quaternion startingAngle1Quat;

	// Token: 0x04002176 RID: 8566
	public Quaternion startingAngle2Quat;

	// Token: 0x04002177 RID: 8567
	public Quaternion startingAngle3Quat;

	// Token: 0x04002178 RID: 8568
	public Quaternion[] angle1Table;

	// Token: 0x04002179 RID: 8569
	public Quaternion[] angle2Table;

	// Token: 0x0400217A RID: 8570
	public Quaternion[] angle3Table;

	// Token: 0x0400217B RID: 8571
	private int lastAngle1;

	// Token: 0x0400217C RID: 8572
	private int lastAngle2;

	// Token: 0x0400217D RID: 8573
	private int lastAngle3;

	// Token: 0x0400217E RID: 8574
	private float currentAngle1;

	// Token: 0x0400217F RID: 8575
	private float currentAngle2;

	// Token: 0x04002180 RID: 8576
	private float currentAngle3;

	// Token: 0x04002181 RID: 8577
	private InputDevice tempDevice;

	// Token: 0x04002182 RID: 8578
	private int myTempInt;
}
