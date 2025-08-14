using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000B2F RID: 2863
public class ThrowableBug : TransferrableObject
{
	// Token: 0x060044D7 RID: 17623 RVA: 0x00157AFC File Offset: 0x00155CFC
	protected override void Start()
	{
		base.Start();
		float f = Random.Range(0f, 6.2831855f);
		this.targetVelocity = new Vector3(Mathf.Sin(f) * this.maxNaturalSpeed, 0f, Mathf.Cos(f) * this.maxNaturalSpeed);
		this.currentState = TransferrableObject.PositionState.Dropped;
		this.rayCastNonAllocColliders = new RaycastHit[5];
		this.rayCastNonAllocColliders2 = new RaycastHit[5];
		this.velocityEstimator = base.GetComponent<GorillaVelocityEstimator>();
	}

	// Token: 0x060044D8 RID: 17624 RVA: 0x00157B78 File Offset: 0x00155D78
	internal override void OnEnable()
	{
		base.OnEnable();
		ThrowableBugBeacon.OnCall += this.ThrowableBugBeacon_OnCall;
		ThrowableBugBeacon.OnDismiss += this.ThrowableBugBeacon_OnDismiss;
		ThrowableBugBeacon.OnLock += this.ThrowableBugBeacon_OnLock;
		ThrowableBugBeacon.OnUnlock += this.ThrowableBugBeacon_OnUnlock;
		ThrowableBugBeacon.OnChangeSpeedMultiplier += this.ThrowableBugBeacon_OnChangeSpeedMultiplier;
	}

	// Token: 0x060044D9 RID: 17625 RVA: 0x00157BE0 File Offset: 0x00155DE0
	internal override void OnDisable()
	{
		base.OnDisable();
		ThrowableBugBeacon.OnCall -= this.ThrowableBugBeacon_OnCall;
		ThrowableBugBeacon.OnDismiss -= this.ThrowableBugBeacon_OnDismiss;
		ThrowableBugBeacon.OnLock -= this.ThrowableBugBeacon_OnLock;
		ThrowableBugBeacon.OnUnlock -= this.ThrowableBugBeacon_OnUnlock;
		ThrowableBugBeacon.OnChangeSpeedMultiplier -= this.ThrowableBugBeacon_OnChangeSpeedMultiplier;
	}

	// Token: 0x060044DA RID: 17626 RVA: 0x00157C48 File Offset: 0x00155E48
	private bool isValid(ThrowableBugBeacon tbb)
	{
		return tbb.BugName == this.bugName && (tbb.Range <= 0f || Vector3.Distance(tbb.transform.position, base.transform.position) <= tbb.Range);
	}

	// Token: 0x060044DB RID: 17627 RVA: 0x00157C9A File Offset: 0x00155E9A
	private void ThrowableBugBeacon_OnCall(ThrowableBugBeacon tbb)
	{
		if (this.isValid(tbb))
		{
			this.reliableState.travelingDirection = tbb.transform.position - base.transform.position;
		}
	}

	// Token: 0x060044DC RID: 17628 RVA: 0x00157CCC File Offset: 0x00155ECC
	private void ThrowableBugBeacon_OnLock(ThrowableBugBeacon tbb)
	{
		if (this.isValid(tbb))
		{
			this.reliableState.travelingDirection = tbb.transform.position - base.transform.position;
			this.lockedTarget = tbb.transform;
			this.locked = true;
		}
	}

	// Token: 0x060044DD RID: 17629 RVA: 0x00157D1B File Offset: 0x00155F1B
	private void ThrowableBugBeacon_OnDismiss(ThrowableBugBeacon tbb)
	{
		if (this.isValid(tbb))
		{
			this.reliableState.travelingDirection = base.transform.position - tbb.transform.position;
			this.locked = false;
		}
	}

	// Token: 0x060044DE RID: 17630 RVA: 0x00157D53 File Offset: 0x00155F53
	private void ThrowableBugBeacon_OnUnlock(ThrowableBugBeacon tbb)
	{
		if (this.isValid(tbb))
		{
			this.locked = false;
		}
	}

	// Token: 0x060044DF RID: 17631 RVA: 0x00157D65 File Offset: 0x00155F65
	private void ThrowableBugBeacon_OnChangeSpeedMultiplier(ThrowableBugBeacon tbb, float f)
	{
		if (this.isValid(tbb))
		{
			this.speedMultiplier = f;
		}
	}

	// Token: 0x060044E0 RID: 17632 RVA: 0x0001D558 File Offset: 0x0001B758
	public override bool ShouldBeKinematic()
	{
		return true;
	}

	// Token: 0x060044E1 RID: 17633 RVA: 0x00157D78 File Offset: 0x00155F78
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		this.raycastFrameCounter = (this.raycastFrameCounter + 1) % this.raycastFramePeriod;
		bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand || this.currentState == TransferrableObject.PositionState.InRightHand;
		if (this.animator.enabled)
		{
			this.animator.SetBool(ThrowableBug._g_IsHeld, flag);
		}
		if (!this.audioSource)
		{
			return;
		}
		switch (this.currentAudioState)
		{
		case ThrowableBug.AudioState.JustGrabbed:
			if (!flag)
			{
				this.currentAudioState = ThrowableBug.AudioState.JustReleased;
				return;
			}
			if (this.grabBugAudioClip && this.audioSource.clip != this.grabBugAudioClip)
			{
				this.audioSource.clip = this.grabBugAudioClip;
				this.audioSource.time = 0f;
				if (this.audioSource.isActiveAndEnabled)
				{
					this.audioSource.GTPlay();
					return;
				}
			}
			else if (!this.audioSource.isPlaying)
			{
				this.currentAudioState = ThrowableBug.AudioState.ContinuallyGrabbed;
				return;
			}
			break;
		case ThrowableBug.AudioState.ContinuallyGrabbed:
			if (!flag)
			{
				this.currentAudioState = ThrowableBug.AudioState.JustReleased;
				return;
			}
			break;
		case ThrowableBug.AudioState.JustReleased:
			if (!flag)
			{
				if (this.releaseBugAudioClip && this.audioSource.clip != this.releaseBugAudioClip)
				{
					this.audioSource.clip = this.releaseBugAudioClip;
					this.audioSource.time = 0f;
					if (this.audioSource.isActiveAndEnabled)
					{
						this.audioSource.GTPlay();
						return;
					}
				}
				else if (!this.audioSource.isPlaying)
				{
					this.currentAudioState = ThrowableBug.AudioState.NotHeld;
					return;
				}
			}
			else
			{
				this.currentAudioState = ThrowableBug.AudioState.JustGrabbed;
			}
			break;
		case ThrowableBug.AudioState.NotHeld:
			if (flag)
			{
				this.currentAudioState = ThrowableBug.AudioState.JustGrabbed;
				return;
			}
			if (this.flyingBugAudioClip && !this.audioSource.isPlaying)
			{
				this.audioSource.clip = this.flyingBugAudioClip;
				this.audioSource.time = 0f;
				if (this.audioSource.isActiveAndEnabled)
				{
					this.audioSource.GTPlay();
					return;
				}
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060044E2 RID: 17634 RVA: 0x00157F7C File Offset: 0x0015617C
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!this.reliableState)
		{
			return;
		}
		if ((this.currentState & TransferrableObject.PositionState.Dropped) == TransferrableObject.PositionState.None)
		{
			return;
		}
		if (this.locked && Vector3.Distance(this.lockedTarget.position, base.transform.position) > 0.1f)
		{
			this.reliableState.travelingDirection = this.lockedTarget.position - base.transform.position;
		}
		if (this.slowingDownProgress < 1f)
		{
			this.slowingDownProgress += this.slowdownAcceleration * Time.deltaTime;
			this.reliableState.travelingDirection = Vector3.Slerp(this.thrownVeloicity, this.targetVelocity, Mathf.SmoothStep(0f, 1f, this.slowingDownProgress));
		}
		else
		{
			this.reliableState.travelingDirection = this.reliableState.travelingDirection.normalized * this.maxNaturalSpeed;
		}
		this.bobingFrequency = (this.shouldRandomizeFrequency ? this.RandomizeBobingFrequency() : this.bobbingDefaultFrequency);
		float num = this.bobingState + this.bobingSpeed * Time.deltaTime;
		float num2 = Mathf.Sin(num / this.bobingFrequency) - Mathf.Sin(this.bobingState / this.bobingFrequency);
		Vector3 vector = Vector3.up * (num2 * this.bobMagnintude);
		this.bobingState = num;
		if (this.bobingState > 6.2831855f)
		{
			this.bobingState -= 6.2831855f;
		}
		vector += this.reliableState.travelingDirection * Time.deltaTime;
		float maxDistance = this.isTooHighTravelingDown ? this.minimumHeightOffOfTheGroundBeforeStoppingDescent : this.maximumHeightOffOfTheGroundBeforeStartingDescent;
		float num3 = this.isTooLowTravelingUp ? this.maximumHeightOffOfTheGroundBeforeStoppingAscent : this.minimumHeightOffOfTheGroundBeforeStartingAscent;
		if (this.raycastFrameCounter == 0)
		{
			if (Physics.RaycastNonAlloc(base.transform.position, Vector3.down, this.rayCastNonAllocColliders2, maxDistance, this.collisionCheckMask) > 0)
			{
				this.isTooHighTravelingDown = false;
				if (this.descentSlerp > 0f)
				{
					this.descentSlerp = Mathf.Clamp01(this.descentSlerp - this.descentSlerpRate * Time.deltaTime);
				}
				RaycastHit raycastHit = this.rayCastNonAllocColliders2[0];
				this.isTooLowTravelingUp = (raycastHit.distance < num3);
				if (this.isTooLowTravelingUp)
				{
					if (this.ascentSlerp < 1f)
					{
						this.ascentSlerp = Mathf.Clamp01(this.ascentSlerp + this.ascentSlerpRate * Time.deltaTime);
					}
				}
				else if (this.ascentSlerp > 0f)
				{
					this.ascentSlerp = Mathf.Clamp01(this.ascentSlerp - this.ascentSlerpRate * Time.deltaTime);
				}
			}
			else
			{
				this.isTooHighTravelingDown = true;
				if (this.descentSlerp < 1f)
				{
					this.descentSlerp = Mathf.Clamp01(this.descentSlerp + this.descentSlerpRate * Time.deltaTime);
				}
			}
		}
		vector += Time.deltaTime * Mathf.SmoothStep(0f, 1f, this.descentSlerp) * this.descentRate * Vector3.down;
		vector += Time.deltaTime * Mathf.SmoothStep(0f, 1f, this.ascentSlerp) * this.ascentRate * Vector3.up;
		float num4;
		Vector3 axis;
		Quaternion.FromToRotation(base.transform.rotation * Vector3.up, Quaternion.identity * Vector3.up).ToAngleAxis(out num4, out axis);
		Quaternion quaternion = Quaternion.AngleAxis(num4 * 0.02f, axis);
		float num5;
		Vector3 axis2;
		Quaternion.FromToRotation(base.transform.rotation * Vector3.forward, this.reliableState.travelingDirection.normalized).ToAngleAxis(out num5, out axis2);
		Quaternion lhs = Quaternion.AngleAxis(num5 * 0.005f, axis2);
		quaternion = lhs * quaternion;
		vector = quaternion * quaternion * quaternion * quaternion * vector;
		vector *= this.speedMultiplier;
		this.speedMultiplier = Mathf.MoveTowards(this.speedMultiplier, 1f, Time.deltaTime);
		if (this.raycastFrameCounter == 0)
		{
			if (Physics.SphereCastNonAlloc(base.transform.position, this.collisionHitRadius, vector.normalized, this.rayCastNonAllocColliders, vector.magnitude, this.collisionCheckMask) > 0)
			{
				Vector3 normal = this.rayCastNonAllocColliders[0].normal;
				this.reliableState.travelingDirection = Vector3.Reflect(this.reliableState.travelingDirection, normal).x0z();
				base.transform.position += Vector3.Reflect(vector, normal);
				this.thrownVeloicity = Vector3.Reflect(this.thrownVeloicity, normal);
				this.targetVelocity = Vector3.Reflect(this.targetVelocity, normal).x0z();
			}
			else
			{
				base.transform.position += vector;
			}
		}
		else
		{
			base.transform.position += vector;
		}
		this.bugRotationalVelocity = quaternion * this.bugRotationalVelocity;
		float num6;
		Vector3 axis3;
		this.bugRotationalVelocity.ToAngleAxis(out num6, out axis3);
		this.bugRotationalVelocity = Quaternion.AngleAxis(num6 * 0.9f, axis3);
		base.transform.rotation = this.bugRotationalVelocity * base.transform.rotation;
	}

	// Token: 0x060044E3 RID: 17635 RVA: 0x00158501 File Offset: 0x00156701
	private float RandomizeBobingFrequency()
	{
		return Random.Range(this.minRandFrequency, this.maxRandFrequency);
	}

	// Token: 0x060044E4 RID: 17636 RVA: 0x00158514 File Offset: 0x00156714
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		this.slowingDownProgress = 0f;
		Vector3 linearVelocity = this.velocityEstimator.linearVelocity;
		this.thrownVeloicity = linearVelocity;
		this.reliableState.travelingDirection = linearVelocity;
		this.bugRotationalVelocity = Quaternion.Euler(this.velocityEstimator.angularVelocity);
		this.startingSpeed = linearVelocity.magnitude;
		Vector3 normalized = this.reliableState.travelingDirection.x0z().normalized;
		this.targetVelocity = normalized * this.maxNaturalSpeed;
		return true;
	}

	// Token: 0x060044E5 RID: 17637 RVA: 0x001585A6 File Offset: 0x001567A6
	public void OnCollisionEnter(Collision collision)
	{
		this.reliableState.travelingDirection *= -1f;
	}

	// Token: 0x060044E6 RID: 17638 RVA: 0x001585C4 File Offset: 0x001567C4
	private void Update()
	{
		if (this.updateMultiplier > 0)
		{
			for (int i = 0; i < this.updateMultiplier; i++)
			{
				this.LateUpdateLocal();
			}
		}
	}

	// Token: 0x04004F07 RID: 20231
	public ThrowableBugReliableState reliableState;

	// Token: 0x04004F08 RID: 20232
	public float slowingDownProgress;

	// Token: 0x04004F09 RID: 20233
	public float startingSpeed;

	// Token: 0x04004F0A RID: 20234
	public int raycastFramePeriod = 5;

	// Token: 0x04004F0B RID: 20235
	private int raycastFrameCounter;

	// Token: 0x04004F0C RID: 20236
	public float bobingSpeed = 1f;

	// Token: 0x04004F0D RID: 20237
	public float bobMagnintude = 0.1f;

	// Token: 0x04004F0E RID: 20238
	public bool shouldRandomizeFrequency;

	// Token: 0x04004F0F RID: 20239
	public float minRandFrequency = 0.008f;

	// Token: 0x04004F10 RID: 20240
	public float maxRandFrequency = 1f;

	// Token: 0x04004F11 RID: 20241
	public float bobingFrequency = 1f;

	// Token: 0x04004F12 RID: 20242
	public float bobingState;

	// Token: 0x04004F13 RID: 20243
	public float thrownYVelocity;

	// Token: 0x04004F14 RID: 20244
	public float collisionHitRadius;

	// Token: 0x04004F15 RID: 20245
	public LayerMask collisionCheckMask;

	// Token: 0x04004F16 RID: 20246
	public Vector3 thrownVeloicity;

	// Token: 0x04004F17 RID: 20247
	public Vector3 targetVelocity;

	// Token: 0x04004F18 RID: 20248
	public Quaternion bugRotationalVelocity;

	// Token: 0x04004F19 RID: 20249
	private RaycastHit[] rayCastNonAllocColliders;

	// Token: 0x04004F1A RID: 20250
	private RaycastHit[] rayCastNonAllocColliders2;

	// Token: 0x04004F1B RID: 20251
	public VRRig followingRig;

	// Token: 0x04004F1C RID: 20252
	public bool isTooHighTravelingDown;

	// Token: 0x04004F1D RID: 20253
	public float descentSlerp;

	// Token: 0x04004F1E RID: 20254
	public float ascentSlerp;

	// Token: 0x04004F1F RID: 20255
	public float maxNaturalSpeed;

	// Token: 0x04004F20 RID: 20256
	public float slowdownAcceleration;

	// Token: 0x04004F21 RID: 20257
	public float maximumHeightOffOfTheGroundBeforeStartingDescent = 5f;

	// Token: 0x04004F22 RID: 20258
	public float minimumHeightOffOfTheGroundBeforeStoppingDescent = 3f;

	// Token: 0x04004F23 RID: 20259
	public float descentRate = 0.2f;

	// Token: 0x04004F24 RID: 20260
	public float descentSlerpRate = 0.2f;

	// Token: 0x04004F25 RID: 20261
	public float minimumHeightOffOfTheGroundBeforeStartingAscent = 0.5f;

	// Token: 0x04004F26 RID: 20262
	public float maximumHeightOffOfTheGroundBeforeStoppingAscent = 0.75f;

	// Token: 0x04004F27 RID: 20263
	public float ascentRate = 0.4f;

	// Token: 0x04004F28 RID: 20264
	public float ascentSlerpRate = 1f;

	// Token: 0x04004F29 RID: 20265
	private bool isTooLowTravelingUp;

	// Token: 0x04004F2A RID: 20266
	public Animator animator;

	// Token: 0x04004F2B RID: 20267
	[FormerlySerializedAs("grabBugAudioSource")]
	public AudioClip grabBugAudioClip;

	// Token: 0x04004F2C RID: 20268
	[FormerlySerializedAs("releaseBugAudioSource")]
	public AudioClip releaseBugAudioClip;

	// Token: 0x04004F2D RID: 20269
	[FormerlySerializedAs("flyingBugAudioSource")]
	public AudioClip flyingBugAudioClip;

	// Token: 0x04004F2E RID: 20270
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04004F2F RID: 20271
	private float bobbingDefaultFrequency = 1f;

	// Token: 0x04004F30 RID: 20272
	public int updateMultiplier;

	// Token: 0x04004F31 RID: 20273
	private ThrowableBug.AudioState currentAudioState;

	// Token: 0x04004F32 RID: 20274
	private float speedMultiplier = 1f;

	// Token: 0x04004F33 RID: 20275
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04004F34 RID: 20276
	[SerializeField]
	private ThrowableBug.BugName bugName;

	// Token: 0x04004F35 RID: 20277
	private Transform lockedTarget;

	// Token: 0x04004F36 RID: 20278
	private bool locked;

	// Token: 0x04004F37 RID: 20279
	private static readonly int _g_IsHeld = Animator.StringToHash("isHeld");

	// Token: 0x02000B30 RID: 2864
	public enum BugName
	{
		// Token: 0x04004F39 RID: 20281
		NONE,
		// Token: 0x04004F3A RID: 20282
		DougTheBug,
		// Token: 0x04004F3B RID: 20283
		MattTheBat
	}

	// Token: 0x02000B31 RID: 2865
	private enum AudioState
	{
		// Token: 0x04004F3D RID: 20285
		JustGrabbed,
		// Token: 0x04004F3E RID: 20286
		ContinuallyGrabbed,
		// Token: 0x04004F3F RID: 20287
		JustReleased,
		// Token: 0x04004F40 RID: 20288
		NotHeld
	}
}
