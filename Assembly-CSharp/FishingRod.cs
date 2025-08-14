using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000586 RID: 1414
public class FishingRod : TransferrableObject
{
	// Token: 0x06002270 RID: 8816 RVA: 0x000B9D8C File Offset: 0x000B7F8C
	public override void OnActivate()
	{
		base.OnActivate();
		Transform transform = base.transform;
		Vector3 force = transform.up + transform.forward * 640f;
		this.bobRigidbody.AddForce(force, ForceMode.Impulse);
		this.line.tensionScale = 0.86f;
		this.ReelOut();
	}

	// Token: 0x06002271 RID: 8817 RVA: 0x000B9DE5 File Offset: 0x000B7FE5
	public override void OnDeactivate()
	{
		base.OnDeactivate();
		this.line.tensionScale = 1f;
		this.ReelStop();
	}

	// Token: 0x06002272 RID: 8818 RVA: 0x000B9E03 File Offset: 0x000B8003
	protected override void Start()
	{
		base.Start();
		this.rig = base.GetComponentInParent<VRRig>();
	}

	// Token: 0x06002273 RID: 8819 RVA: 0x000B9E17 File Offset: 0x000B8017
	public void SetBobFloat(bool enable)
	{
		if (!this.bobRigidbody)
		{
			return;
		}
		this._bobFloatPlaneY = this.bobRigidbody.position.y;
		this._bobFloating = enable;
	}

	// Token: 0x06002274 RID: 8820 RVA: 0x000B9E44 File Offset: 0x000B8044
	private void QuickReel()
	{
		if (this._lineResizing)
		{
			return;
		}
		this.bobCollider.enabled = false;
		this.ReelIn();
	}

	// Token: 0x06002275 RID: 8821 RVA: 0x000B9E64 File Offset: 0x000B8064
	public bool IsFreeHandGripping()
	{
		bool flag = base.InLeftHand();
		Transform transform = flag ? this.rig.rightHandTransform : this.rig.leftHandTransform;
		float magnitude = (this.reelToSync.position - transform.position).magnitude;
		bool flag2 = this._grippingHand || magnitude <= 0.16f;
		this.disableStealing = flag2;
		if (!flag2)
		{
			return false;
		}
		VRMapThumb vrmapThumb = flag ? this.rig.rightThumb : this.rig.leftThumb;
		VRMapIndex vrmapIndex = flag ? this.rig.rightIndex : this.rig.leftIndex;
		VRMap vrmap = flag ? this.rig.rightMiddle : this.rig.leftMiddle;
		float calcT = vrmapThumb.calcT;
		float calcT2 = vrmapIndex.calcT;
		float calcT3 = vrmap.calcT;
		bool flag3 = calcT >= 0.1f && calcT2 >= 0.2f && calcT3 >= 0.2f;
		this._grippingHand = (flag3 ? transform : null);
		return flag3;
	}

	// Token: 0x06002276 RID: 8822 RVA: 0x000B9F7D File Offset: 0x000B817D
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (this._grippingHand)
		{
			this._grippingHand = null;
		}
		this.ResetLineLength(this.lineLengthMin * 1.32f);
		return true;
	}

	// Token: 0x06002277 RID: 8823 RVA: 0x000B9FB4 File Offset: 0x000B81B4
	public void ReelIn()
	{
		this._manualReeling = false;
		FishingRod.SetHandleMotorUse(true, this.reelSpinRate, this.handleJoint, true);
		this._lineResizing = true;
		this._lineExpanding = false;
		float num = (float)this.line.segmentNumber + 0.0001f;
		this.line.segmentMinLength = (this._targetSegmentMin = this.lineLengthMin / num);
		this.line.segmentMaxLength = (this._targetSegmentMax = this.lineLengthMax / num);
	}

	// Token: 0x06002278 RID: 8824 RVA: 0x000BA034 File Offset: 0x000B8234
	public void ReelOut()
	{
		this._manualReeling = false;
		FishingRod.SetHandleMotorUse(true, this.reelSpinRate, this.handleJoint, false);
		this._lineResizing = true;
		this._lineExpanding = true;
		float num = (float)this.line.segmentNumber + 0.0001f;
		this.line.segmentMinLength = (this._targetSegmentMin = this.lineLengthMin / num);
		this.line.segmentMaxLength = (this._targetSegmentMax = this.lineLengthMax / num);
	}

	// Token: 0x06002279 RID: 8825 RVA: 0x000BA0B4 File Offset: 0x000B82B4
	public void ReelStop()
	{
		if (this._manualReeling)
		{
			this._localRotDelta = 0f;
		}
		else
		{
			FishingRod.SetHandleMotorUse(false, 0f, this.handleJoint, false);
		}
		this.bobCollider.enabled = true;
		if (this.line)
		{
			this.line.resizeScale = 1f;
		}
		this._lineResizing = false;
		this._lineExpanding = false;
	}

	// Token: 0x0600227A RID: 8826 RVA: 0x000BA120 File Offset: 0x000B8320
	private static void SetHandleMotorUse(bool useMotor, float spinRate, HingeJoint handleJoint, bool reverse)
	{
		JointMotor motor = handleJoint.motor;
		motor.force = (useMotor ? 1f : 0f) * spinRate;
		motor.targetVelocity = 16384f * (reverse ? -1f : 1f);
		handleJoint.motor = motor;
	}

	// Token: 0x0600227B RID: 8827 RVA: 0x000BA170 File Offset: 0x000B8370
	public override void TriggeredLateUpdate()
	{
		base.TriggeredLateUpdate();
		this._manualReeling = (this._isGrippingHandle = this.IsFreeHandGripping());
		if (ControllerInputPoller.instance && ControllerInputPoller.PrimaryButtonPress(base.InLeftHand() ? XRNode.LeftHand : XRNode.RightHand))
		{
			this.QuickReel();
		}
		if (this._lineResetting && this._sinceReset.HasElapsed(this.line.resizeSpeed))
		{
			this.bobCollider.enabled = true;
			this._lineResetting = false;
		}
		this.handleTransform.localPosition = this.reelFreezeLocalPosition;
	}

	// Token: 0x0600227C RID: 8828 RVA: 0x000BA203 File Offset: 0x000B8403
	private void ResetLineLength(float length)
	{
		if (!this.line)
		{
			return;
		}
		this._lineResetting = true;
		this.bobCollider.enabled = false;
		this.line.ForceTotalLength(length);
		this._sinceReset = TimeSince.Now();
	}

	// Token: 0x0600227D RID: 8829 RVA: 0x000BA240 File Offset: 0x000B8440
	private void FixedUpdate()
	{
		Transform transform = base.transform;
		this.handleRigidbody.useGravity = !this._manualReeling;
		if (this._bobFloating && this.bobRigidbody)
		{
			float y = this.bobRigidbody.position.y;
			float num = this.bobFloatForce * this.bobRigidbody.mass;
			float num2 = num * Mathf.Clamp01(this._bobFloatPlaneY - y);
			num += num2;
			if (y <= this._bobFloatPlaneY)
			{
				this.bobRigidbody.AddForce(0f, num, 0f);
			}
		}
		if (this._manualReeling)
		{
			if (this._isGrippingHandle && this._grippingHand)
			{
				this.reelTo.position = this._grippingHand.position;
			}
			Vector3 vector = this.reelFrom.InverseTransformPoint(this.reelTo.position);
			vector.x = 0f;
			vector.Normalize();
			vector *= 2f;
			Quaternion quaternion = Quaternion.FromToRotation(Vector3.forward, vector);
			quaternion = (base.InRightHand() ? quaternion : Quaternion.Inverse(quaternion));
			this._localRotDelta = FishingRod.GetSignedDeltaYZ(ref this._lastLocalRot, ref quaternion);
			this._lastLocalRot = quaternion;
			Quaternion rot = transform.rotation * quaternion;
			this.handleRigidbody.MoveRotation(rot);
		}
		else
		{
			this.reelTo.localPosition = transform.InverseTransformPoint(this.reelToSync.position);
		}
		if (!this.line)
		{
			return;
		}
		if (this._manualReeling)
		{
			this._lineResizing = (Mathf.Abs(this._localRotDelta) >= 0.001f);
			this._lineExpanding = (Mathf.Sign(this._localRotDelta) >= 0f);
		}
		if (!this._lineResizing)
		{
			return;
		}
		float num3 = this._manualReeling ? (Mathf.Abs(this._localRotDelta) * 0.66f * Time.fixedDeltaTime) : (this.lineResizeRate * this.lineCastFactor);
		this.line.resizeScale = this.lineCastFactor;
		float num4 = num3 * Time.fixedDeltaTime;
		float num5 = this.line.segmentTargetLength;
		if (this._manualReeling)
		{
			float num6 = 1f / ((float)this.line.segmentNumber + 0.0001f);
			float num7 = this.lineLengthMin * num6;
			float num8 = this.lineLengthMax * num6;
			num4 *= (this._lineExpanding ? 1f : -1f);
			num4 *= (base.InRightHand() ? -1f : 1f);
			float num9 = num5 + num4;
			if (num9 > num7 && num9 < num8)
			{
				num5 += num4;
			}
		}
		else if (this._lineExpanding)
		{
			if (num5 < this._targetSegmentMax)
			{
				num5 += num4;
			}
			else
			{
				this._lineResizing = false;
			}
		}
		else if (num5 > this._targetSegmentMin)
		{
			num5 -= num4;
		}
		else
		{
			this._lineResizing = false;
		}
		if (this._lineResizing)
		{
			this.line.segmentTargetLength = num5;
			return;
		}
		this.ReelStop();
	}

	// Token: 0x0600227E RID: 8830 RVA: 0x000BA538 File Offset: 0x000B8738
	private static float GetSignedDeltaYZ(ref Quaternion a, ref Quaternion b)
	{
		Vector3 forward = Vector3.forward;
		Vector3 vector = a * forward;
		Vector3 vector2 = b * forward;
		float current = Mathf.Atan2(vector.y, vector.z) * 57.29578f;
		float target = Mathf.Atan2(vector2.y, vector2.z) * 57.29578f;
		return Mathf.DeltaAngle(current, target);
	}

	// Token: 0x04002BF0 RID: 11248
	public Transform handleTransform;

	// Token: 0x04002BF1 RID: 11249
	public HingeJoint handleJoint;

	// Token: 0x04002BF2 RID: 11250
	public Rigidbody handleRigidbody;

	// Token: 0x04002BF3 RID: 11251
	public BoxCollider handleCollider;

	// Token: 0x04002BF4 RID: 11252
	public Rigidbody bobRigidbody;

	// Token: 0x04002BF5 RID: 11253
	public Collider bobCollider;

	// Token: 0x04002BF6 RID: 11254
	public VerletLine line;

	// Token: 0x04002BF7 RID: 11255
	public GorillaVelocityEstimator tipTracker;

	// Token: 0x04002BF8 RID: 11256
	public Rigidbody tipBody;

	// Token: 0x04002BF9 RID: 11257
	[NonSerialized]
	public VRRig rig;

	// Token: 0x04002BFA RID: 11258
	[Space]
	public Vector3 reelFreezeLocalPosition;

	// Token: 0x04002BFB RID: 11259
	public Transform reelFrom;

	// Token: 0x04002BFC RID: 11260
	public Transform reelTo;

	// Token: 0x04002BFD RID: 11261
	public Transform reelToSync;

	// Token: 0x04002BFE RID: 11262
	[Space]
	public float reelSpinRate = 1f;

	// Token: 0x04002BFF RID: 11263
	public float lineResizeRate = 1f;

	// Token: 0x04002C00 RID: 11264
	public float lineCastFactor = 3f;

	// Token: 0x04002C01 RID: 11265
	public float lineLengthMin = 0.1f;

	// Token: 0x04002C02 RID: 11266
	public float lineLengthMax = 8f;

	// Token: 0x04002C03 RID: 11267
	[Space]
	[NonSerialized]
	private bool _bobFloating;

	// Token: 0x04002C04 RID: 11268
	public float bobFloatForce = 8f;

	// Token: 0x04002C05 RID: 11269
	public float bobStaticDrag = 3.2f;

	// Token: 0x04002C06 RID: 11270
	public float bobDynamicDrag = 1.1f;

	// Token: 0x04002C07 RID: 11271
	[NonSerialized]
	private float _bobFloatPlaneY;

	// Token: 0x04002C08 RID: 11272
	[Space]
	[NonSerialized]
	private float _targetSegmentMin;

	// Token: 0x04002C09 RID: 11273
	[NonSerialized]
	private float _targetSegmentMax;

	// Token: 0x04002C0A RID: 11274
	[Space]
	[NonSerialized]
	private bool _manualReeling;

	// Token: 0x04002C0B RID: 11275
	[NonSerialized]
	private bool _lineResizing;

	// Token: 0x04002C0C RID: 11276
	[NonSerialized]
	private bool _lineExpanding;

	// Token: 0x04002C0D RID: 11277
	[NonSerialized]
	private bool _lineResetting;

	// Token: 0x04002C0E RID: 11278
	[NonSerialized]
	private TimeSince _sinceReset;

	// Token: 0x04002C0F RID: 11279
	[Space]
	[NonSerialized]
	private Quaternion _lastLocalRot = Quaternion.identity;

	// Token: 0x04002C10 RID: 11280
	[NonSerialized]
	private float _localRotDelta;

	// Token: 0x04002C11 RID: 11281
	[NonSerialized]
	private bool _isGrippingHandle;

	// Token: 0x04002C12 RID: 11282
	[NonSerialized]
	private Transform _grippingHand;

	// Token: 0x04002C13 RID: 11283
	private TimeSince _sinceGripLoss;
}
