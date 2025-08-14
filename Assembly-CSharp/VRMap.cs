using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x0200040F RID: 1039
[Serializable]
public class VRMap
{
	// Token: 0x170002BD RID: 701
	// (get) Token: 0x06001927 RID: 6439 RVA: 0x00087C62 File Offset: 0x00085E62
	// (set) Token: 0x06001928 RID: 6440 RVA: 0x00087C6F File Offset: 0x00085E6F
	public Vector3 syncPos
	{
		get
		{
			return this.netSyncPos.CurrentSyncTarget;
		}
		set
		{
			this.netSyncPos.SetNewSyncTarget(value);
		}
	}

	// Token: 0x06001929 RID: 6441 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void Initialize()
	{
	}

	// Token: 0x0600192A RID: 6442 RVA: 0x00087C80 File Offset: 0x00085E80
	public void MapOther(float lerpValue)
	{
		Vector3 a;
		Quaternion a2;
		this.rigTarget.GetLocalPositionAndRotation(out a, out a2);
		this.rigTarget.SetLocalPositionAndRotation(Vector3.Lerp(a, this.syncPos, lerpValue), Quaternion.Lerp(a2, this.syncRotation, lerpValue));
	}

	// Token: 0x0600192B RID: 6443 RVA: 0x00087CC4 File Offset: 0x00085EC4
	public void MapMine(float ratio, Transform playerOffsetTransform)
	{
		Vector3 current;
		Quaternion rotation;
		this.rigTarget.GetPositionAndRotation(out current, out rotation);
		if (this.overrideTarget != null)
		{
			Vector3 a;
			Quaternion lhs;
			this.overrideTarget.GetPositionAndRotation(out a, out lhs);
			this.rigTarget.SetPositionAndRotation(a + rotation * this.trackingPositionOffset * ratio, lhs * Quaternion.Euler(this.trackingRotationOffset));
		}
		else
		{
			if (!this.hasInputDevice && ConnectedControllerHandler.Instance.GetValidForXRNode(this.vrTargetNode))
			{
				this.myInputDevice = InputDevices.GetDeviceAtXRNode(this.vrTargetNode);
				this.hasInputDevice = true;
			}
			Quaternion lhs2;
			Vector3 a2;
			if (this.hasInputDevice && this.myInputDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out lhs2) && this.myInputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out a2))
			{
				this.rigTarget.SetPositionAndRotation(a2 + rotation * this.trackingPositionOffset * ratio + playerOffsetTransform.position, lhs2 * Quaternion.Euler(this.trackingRotationOffset));
				this.rigTarget.RotateAround(playerOffsetTransform.position, Vector3.up, playerOffsetTransform.eulerAngles.y);
			}
		}
		if (this.handholdOverrideTarget != null)
		{
			this.rigTarget.position = Vector3.MoveTowards(current, this.handholdOverrideTarget.position - this.handholdOverrideTargetOffset + rotation * this.trackingPositionOffset * ratio, Time.deltaTime * 2f);
		}
	}

	// Token: 0x0600192C RID: 6444 RVA: 0x00087E54 File Offset: 0x00086054
	public Vector3 GetExtrapolatedControllerPosition()
	{
		Vector3 a;
		Quaternion rotation;
		this.rigTarget.GetPositionAndRotation(out a, out rotation);
		return a - rotation * this.trackingPositionOffset * this.rigTarget.lossyScale.x;
	}

	// Token: 0x0600192D RID: 6445 RVA: 0x00087E97 File Offset: 0x00086097
	public virtual void MapOtherFinger(float handSync, float lerpValue)
	{
		this.calcT = handSync;
		this.LerpFinger(lerpValue, true);
	}

	// Token: 0x0600192E RID: 6446 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void MapMyFinger(float lerpValue)
	{
	}

	// Token: 0x0600192F RID: 6447 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void LerpFinger(float lerpValue, bool isOther)
	{
	}

	// Token: 0x0400213D RID: 8509
	public XRNode vrTargetNode;

	// Token: 0x0400213E RID: 8510
	public Transform overrideTarget;

	// Token: 0x0400213F RID: 8511
	public Transform rigTarget;

	// Token: 0x04002140 RID: 8512
	public Vector3 trackingPositionOffset;

	// Token: 0x04002141 RID: 8513
	public Vector3 trackingRotationOffset;

	// Token: 0x04002142 RID: 8514
	public Transform headTransform;

	// Token: 0x04002143 RID: 8515
	internal NetworkVector3 netSyncPos = new NetworkVector3();

	// Token: 0x04002144 RID: 8516
	public Quaternion syncRotation;

	// Token: 0x04002145 RID: 8517
	public float calcT;

	// Token: 0x04002146 RID: 8518
	private InputDevice myInputDevice;

	// Token: 0x04002147 RID: 8519
	private bool hasInputDevice;

	// Token: 0x04002148 RID: 8520
	public Transform handholdOverrideTarget;

	// Token: 0x04002149 RID: 8521
	public Vector3 handholdOverrideTargetOffset;
}
