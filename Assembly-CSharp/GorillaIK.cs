using System;
using UnityEngine;

// Token: 0x020006DB RID: 1755
public class GorillaIK : MonoBehaviour
{
	// Token: 0x06002BCA RID: 11210 RVA: 0x000E7EDC File Offset: 0x000E60DC
	private void Awake()
	{
		if (Application.isPlaying && !this.testInEditor)
		{
			this.dU = (this.leftUpperArm.position - this.leftLowerArm.position).magnitude;
			this.dL = (this.leftLowerArm.position - this.leftHand.position).magnitude;
			this.dMax = this.dU + this.dL - this.eps;
			this.initialUpperLeft = this.leftUpperArm.localRotation;
			this.initialLowerLeft = this.leftLowerArm.localRotation;
			this.initialUpperRight = this.rightUpperArm.localRotation;
			this.initialLowerRight = this.rightLowerArm.localRotation;
		}
	}

	// Token: 0x06002BCB RID: 11211 RVA: 0x000E7FAE File Offset: 0x000E61AE
	private void OnEnable()
	{
		GorillaIKMgr.Instance.RegisterIK(this);
	}

	// Token: 0x06002BCC RID: 11212 RVA: 0x000E7FBB File Offset: 0x000E61BB
	private void OnDisable()
	{
		GorillaIKMgr.Instance.DeregisterIK(this);
	}

	// Token: 0x06002BCD RID: 11213 RVA: 0x000E7FC8 File Offset: 0x000E61C8
	public void OverrideTargetPos(bool isLeftHand, Vector3 targetWorldPos)
	{
		if (isLeftHand)
		{
			this.hasLeftOverride = true;
			this.leftOverrideWorldPos = targetWorldPos;
			return;
		}
		this.hasRightOverride = true;
		this.rightOverrideWorldPos = targetWorldPos;
	}

	// Token: 0x06002BCE RID: 11214 RVA: 0x000E7FEA File Offset: 0x000E61EA
	public Vector3 GetShoulderLocalTargetPos_Left()
	{
		return this.leftUpperArm.parent.InverseTransformPoint(this.hasLeftOverride ? this.leftOverrideWorldPos : this.targetLeft.position);
	}

	// Token: 0x06002BCF RID: 11215 RVA: 0x000E8017 File Offset: 0x000E6217
	public Vector3 GetShoulderLocalTargetPos_Right()
	{
		return this.rightUpperArm.parent.InverseTransformPoint(this.hasRightOverride ? this.rightOverrideWorldPos : this.targetRight.position);
	}

	// Token: 0x06002BD0 RID: 11216 RVA: 0x000E8044 File Offset: 0x000E6244
	public void ClearOverrides()
	{
		this.hasLeftOverride = false;
		this.hasRightOverride = false;
	}

	// Token: 0x06002BD1 RID: 11217 RVA: 0x000E8054 File Offset: 0x000E6254
	private void ArmIK(ref Transform upperArm, ref Transform lowerArm, ref Transform hand, Quaternion initRotUpper, Quaternion initRotLower, Transform target)
	{
		upperArm.localRotation = initRotUpper;
		lowerArm.localRotation = initRotLower;
		float num = Mathf.Clamp((target.position - upperArm.position).magnitude, this.eps, this.dMax);
		float num2 = Mathf.Acos(Mathf.Clamp(Vector3.Dot((hand.position - upperArm.position).normalized, (lowerArm.position - upperArm.position).normalized), -1f, 1f));
		float num3 = Mathf.Acos(Mathf.Clamp(Vector3.Dot((upperArm.position - lowerArm.position).normalized, (hand.position - lowerArm.position).normalized), -1f, 1f));
		float num4 = Mathf.Acos(Mathf.Clamp(Vector3.Dot((hand.position - upperArm.position).normalized, (target.position - upperArm.position).normalized), -1f, 1f));
		float num5 = Mathf.Acos(Mathf.Clamp((this.dL * this.dL - this.dU * this.dU - num * num) / (-2f * this.dU * num), -1f, 1f));
		float num6 = Mathf.Acos(Mathf.Clamp((num * num - this.dU * this.dU - this.dL * this.dL) / (-2f * this.dU * this.dL), -1f, 1f));
		Vector3 normalized = Vector3.Cross(hand.position - upperArm.position, lowerArm.position - upperArm.position).normalized;
		Vector3 normalized2 = Vector3.Cross(hand.position - upperArm.position, target.position - upperArm.position).normalized;
		Quaternion rhs = Quaternion.AngleAxis((num5 - num2) * 57.29578f, Quaternion.Inverse(upperArm.rotation) * normalized);
		Quaternion rhs2 = Quaternion.AngleAxis((num6 - num3) * 57.29578f, Quaternion.Inverse(lowerArm.rotation) * normalized);
		Quaternion rhs3 = Quaternion.AngleAxis(num4 * 57.29578f, Quaternion.Inverse(upperArm.rotation) * normalized2);
		this.newRotationUpper = upperArm.localRotation * rhs3 * rhs;
		this.newRotationLower = lowerArm.localRotation * rhs2;
		upperArm.localRotation = this.newRotationUpper;
		lowerArm.localRotation = this.newRotationLower;
		hand.rotation = target.rotation;
	}

	// Token: 0x04003712 RID: 14098
	public Transform headBone;

	// Token: 0x04003713 RID: 14099
	public Transform leftUpperArm;

	// Token: 0x04003714 RID: 14100
	public Transform leftLowerArm;

	// Token: 0x04003715 RID: 14101
	public Transform leftHand;

	// Token: 0x04003716 RID: 14102
	public Transform rightUpperArm;

	// Token: 0x04003717 RID: 14103
	public Transform rightLowerArm;

	// Token: 0x04003718 RID: 14104
	public Transform rightHand;

	// Token: 0x04003719 RID: 14105
	public Transform targetLeft;

	// Token: 0x0400371A RID: 14106
	public Transform targetRight;

	// Token: 0x0400371B RID: 14107
	public Transform targetHead;

	// Token: 0x0400371C RID: 14108
	public Quaternion initialUpperLeft;

	// Token: 0x0400371D RID: 14109
	public Quaternion initialLowerLeft;

	// Token: 0x0400371E RID: 14110
	public Quaternion initialUpperRight;

	// Token: 0x0400371F RID: 14111
	public Quaternion initialLowerRight;

	// Token: 0x04003720 RID: 14112
	public Quaternion newRotationUpper;

	// Token: 0x04003721 RID: 14113
	public Quaternion newRotationLower;

	// Token: 0x04003722 RID: 14114
	public float dU;

	// Token: 0x04003723 RID: 14115
	public float dL;

	// Token: 0x04003724 RID: 14116
	public float dMax;

	// Token: 0x04003725 RID: 14117
	public bool testInEditor;

	// Token: 0x04003726 RID: 14118
	public bool reset;

	// Token: 0x04003727 RID: 14119
	public bool testDefineRot;

	// Token: 0x04003728 RID: 14120
	public bool moveOnce;

	// Token: 0x04003729 RID: 14121
	public float eps;

	// Token: 0x0400372A RID: 14122
	public float upperArmAngle;

	// Token: 0x0400372B RID: 14123
	public float elbowAngle;

	// Token: 0x0400372C RID: 14124
	private bool hasLeftOverride;

	// Token: 0x0400372D RID: 14125
	private Vector3 leftOverrideWorldPos;

	// Token: 0x0400372E RID: 14126
	private bool hasRightOverride;

	// Token: 0x0400372F RID: 14127
	private Vector3 rightOverrideWorldPos;
}
