using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020003F6 RID: 1014
public class TransferrableObjectHoldablePart_Slide : TransferrableObjectHoldablePart
{
	// Token: 0x060017A0 RID: 6048 RVA: 0x0007F748 File Offset: 0x0007D948
	protected override void UpdateHeld(VRRig rig, bool isHeldLeftHand)
	{
		int num = isHeldLeftHand ? 0 : 1;
		GTPlayer instance = GTPlayer.Instance;
		if (!rig.isOfflineVRRig)
		{
			Vector3 b = (isHeldLeftHand ? instance.leftHandOffset : instance.rightHandOffset) * rig.scaleFactor;
			VRMap vrmap = isHeldLeftHand ? rig.leftHand : rig.rightHand;
			this._snapToLine.target.position = vrmap.GetExtrapolatedControllerPosition() - b;
			return;
		}
		Transform transform = (num == 0) ? instance.leftControllerTransform : instance.rightControllerTransform;
		Vector3 position = transform.position;
		Vector3 snappedPoint = this._snapToLine.GetSnappedPoint(position);
		if (this._maxHandSnapDistance > 0f && (transform.position - snappedPoint).IsLongerThan(this._maxHandSnapDistance))
		{
			this.OnRelease(null, isHeldLeftHand ? EquipmentInteractor.instance.leftHand : EquipmentInteractor.instance.rightHand);
			return;
		}
		transform.position = snappedPoint;
		this._snapToLine.target.position = snappedPoint;
	}

	// Token: 0x04001F8C RID: 8076
	[SerializeField]
	private float _maxHandSnapDistance;

	// Token: 0x04001F8D RID: 8077
	[SerializeField]
	private SnapXformToLine _snapToLine;

	// Token: 0x04001F8E RID: 8078
	private const int LEFT = 0;

	// Token: 0x04001F8F RID: 8079
	private const int RIGHT = 1;
}
