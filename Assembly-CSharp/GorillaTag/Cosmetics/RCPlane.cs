using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F16 RID: 3862
	public class RCPlane : RCVehicle
	{
		// Token: 0x06005F96 RID: 24470 RVA: 0x001E4A94 File Offset: 0x001E2C94
		protected override void Awake()
		{
			base.Awake();
			this.pitchAccelMinMax.x = this.pitchVelocityTargetMinMax.x / this.pitchVelocityRampTimeMinMax.x;
			this.pitchAccelMinMax.y = this.pitchVelocityTargetMinMax.y / this.pitchVelocityRampTimeMinMax.y;
			this.rollAccel = this.rollVelocityTarget / this.rollVelocityRampTime;
			this.thrustAccel = this.thrustVelocityTarget / this.thrustAccelTime;
		}

		// Token: 0x06005F97 RID: 24471 RVA: 0x001E4B14 File Offset: 0x001E2D14
		protected override void AuthorityBeginMobilization()
		{
			base.AuthorityBeginMobilization();
			float x = base.transform.lossyScale.x;
			this.rb.velocity = base.transform.forward * this.initialSpeed * x;
		}

		// Token: 0x06005F98 RID: 24472 RVA: 0x001E4B60 File Offset: 0x001E2D60
		protected override void AuthorityUpdate(float dt)
		{
			base.AuthorityUpdate(dt);
			this.motorLevel = 0f;
			if (this.localState == RCVehicle.State.Mobilized)
			{
				this.motorLevel = this.activeInput.trigger;
			}
			this.leftAileronLevel = 0f;
			this.rightAileronLevel = 0f;
			float magnitude = this.activeInput.joystick.magnitude;
			if (magnitude > 0.01f)
			{
				float num = Mathf.Abs(this.activeInput.joystick.x) / magnitude;
				float num2 = Mathf.Abs(this.activeInput.joystick.y) / magnitude;
				this.leftAileronLevel = Mathf.Clamp(num * this.activeInput.joystick.x + num2 * -this.activeInput.joystick.y, -1f, 1f);
				this.rightAileronLevel = Mathf.Clamp(num * this.activeInput.joystick.x + num2 * this.activeInput.joystick.y, -1f, 1f);
			}
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.dataA = (byte)Mathf.Clamp(Mathf.FloorToInt(this.motorLevel * 255f), 0, 255);
				this.networkSync.syncedState.dataB = (byte)Mathf.Clamp(Mathf.FloorToInt(this.leftAileronLevel * 126f), -126, 126);
				this.networkSync.syncedState.dataC = (byte)Mathf.Clamp(Mathf.FloorToInt(this.rightAileronLevel * 126f), -126, 126);
			}
		}

		// Token: 0x06005F99 RID: 24473 RVA: 0x001E4D04 File Offset: 0x001E2F04
		protected override void RemoteUpdate(float dt)
		{
			base.RemoteUpdate(dt);
			if (this.networkSync != null)
			{
				this.motorLevel = Mathf.Clamp01((float)this.networkSync.syncedState.dataA / 255f);
				this.leftAileronLevel = Mathf.Clamp((float)this.networkSync.syncedState.dataB / 126f, -1f, 1f);
				this.rightAileronLevel = Mathf.Clamp((float)this.networkSync.syncedState.dataC / 126f, -1f, 1f);
			}
		}

		// Token: 0x06005F9A RID: 24474 RVA: 0x001E4DA0 File Offset: 0x001E2FA0
		protected override void SharedUpdate(float dt)
		{
			base.SharedUpdate(dt);
			switch (this.localState)
			{
			case RCVehicle.State.DockedLeft:
			case RCVehicle.State.DockedRight:
				this.propellerSpinRate = Mathf.MoveTowards(this.propellerSpinRate, 0.6f, 6.6666665f * dt);
				this.propellerAngle += this.propellerSpinRate * 360f * dt;
				this.propeller.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, this.propellerAngle));
				break;
			case RCVehicle.State.Mobilized:
			{
				if (this.localStatePrev != RCVehicle.State.Mobilized)
				{
					this.audioSource.loop = true;
					this.audioSource.clip = this.motorSound;
					this.audioSource.volume = 0f;
					this.audioSource.GTPlay();
				}
				float target = Mathf.Lerp(this.motorSoundVolumeMinMax.x, this.motorSoundVolumeMinMax.y, this.motorLevel);
				this.audioSource.volume = Mathf.MoveTowards(this.audioSource.volume, target, this.motorSoundVolumeMinMax.y / this.motorVolumeRampTime * dt);
				this.propellerSpinRate = Mathf.MoveTowards(this.propellerSpinRate, 5f, 6.6666665f * dt);
				this.propellerAngle += this.propellerSpinRate * 360f * dt;
				this.propeller.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, this.propellerAngle));
				break;
			}
			case RCVehicle.State.Crashed:
				if (this.localStatePrev != RCVehicle.State.Crashed)
				{
					this.audioSource.GTStop();
					this.audioSource.clip = null;
					this.audioSource.loop = false;
					this.audioSource.volume = this.crashSoundVolume;
					if (this.crashSound != null)
					{
						this.audioSource.GTPlayOneShot(this.crashSound, 1f);
					}
				}
				this.propellerSpinRate = Mathf.MoveTowards(this.propellerSpinRate, 0f, 13.333333f * dt);
				this.propellerAngle += this.propellerSpinRate * 360f * dt;
				this.propeller.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, this.propellerAngle));
				break;
			}
			float target2 = Mathf.Lerp(this.aileronAngularRange.x, this.aileronAngularRange.y, Mathf.InverseLerp(-1f, 1f, this.leftAileronLevel));
			float target3 = Mathf.Lerp(this.aileronAngularRange.x, this.aileronAngularRange.y, Mathf.InverseLerp(-1f, 1f, this.rightAileronLevel));
			this.leftAileronAngle = Mathf.MoveTowards(this.leftAileronAngle, target2, this.aileronAngularAcc * Time.deltaTime);
			this.rightAileronAngle = Mathf.MoveTowards(this.rightAileronAngle, target3, this.aileronAngularAcc * Time.deltaTime);
			Quaternion localRotation = Quaternion.Euler(0f, -90f, 90f + this.leftAileronAngle);
			Quaternion localRotation2 = Quaternion.Euler(0f, 90f, -90f + this.rightAileronAngle);
			this.leftAileronLower.localRotation = localRotation;
			this.leftAileronUpper.localRotation = localRotation;
			this.rightAileronLower.localRotation = localRotation2;
			this.rightAileronUpper.localRotation = localRotation2;
		}

		// Token: 0x06005F9B RID: 24475 RVA: 0x001E5108 File Offset: 0x001E3308
		private void FixedUpdate()
		{
			if (!base.HasLocalAuthority || this.localState != RCVehicle.State.Mobilized)
			{
				return;
			}
			float x = base.transform.lossyScale.x;
			float num = this.thrustVelocityTarget * x;
			float num2 = this.thrustAccel * x;
			float fixedDeltaTime = Time.fixedDeltaTime;
			this.pitch = base.NormalizeAngle180(this.pitch);
			this.roll = base.NormalizeAngle180(this.roll);
			float num3 = this.pitch;
			float num4 = this.roll;
			if (this.activeInput.joystick.y >= 0f)
			{
				float target = this.activeInput.joystick.y * this.pitchVelocityTargetMinMax.y;
				this.pitchVel = Mathf.MoveTowards(this.pitchVel, target, this.pitchAccelMinMax.y * fixedDeltaTime);
				this.pitch += this.pitchVel * fixedDeltaTime;
			}
			else
			{
				float target2 = -this.activeInput.joystick.y * this.pitchVelocityTargetMinMax.x;
				this.pitchVel = Mathf.MoveTowards(this.pitchVel, target2, this.pitchAccelMinMax.x * fixedDeltaTime);
				this.pitch += this.pitchVel * fixedDeltaTime;
			}
			float target3 = -this.activeInput.joystick.x * this.rollVelocityTarget;
			this.rollVel = Mathf.MoveTowards(this.rollVel, target3, this.rollAccel * fixedDeltaTime);
			this.roll += this.rollVel * fixedDeltaTime;
			Quaternion rhs = Quaternion.Euler(new Vector3(this.pitch - num3, 0f, this.roll - num4));
			base.transform.rotation = base.transform.rotation * rhs;
			this.rb.angularVelocity = Vector3.zero;
			Vector3 velocity = this.rb.velocity;
			float magnitude = velocity.magnitude;
			float num5 = Mathf.Max(Vector3.Dot(base.transform.forward, velocity), 0f);
			float num6 = this.activeInput.trigger * num;
			float num7 = 0.1f * x;
			if (num6 > num7 && num6 > num5)
			{
				float num8 = Mathf.MoveTowards(num5, num6, num2 * fixedDeltaTime);
				this.rb.AddForce(base.transform.forward * (num8 - num5), ForceMode.VelocityChange);
			}
			float b = 0.01f * x;
			float time = Vector3.Dot(velocity / Mathf.Max(magnitude, b), base.transform.forward);
			float num9 = this.liftVsAttackCurve.Evaluate(time);
			float num10 = Mathf.Lerp(this.liftVsSpeedOutput.x, this.liftVsSpeedOutput.y, Mathf.InverseLerp(this.liftVsSpeedInput.x, this.liftVsSpeedInput.y, magnitude / x));
			float d = num9 * num10;
			Vector3 a = Vector3.RotateTowards(velocity, base.transform.forward * magnitude, this.pitchVelocityFollowRateAngle * 0.017453292f * fixedDeltaTime, this.pitchVelocityFollowRateMagnitude * fixedDeltaTime) - velocity;
			this.rb.AddForce(a * d, ForceMode.VelocityChange);
			float time2 = Vector3.Dot(velocity.normalized, base.transform.up);
			float d2 = this.dragVsAttackCurve.Evaluate(time2);
			this.rb.AddForce(-velocity * this.maxDrag * d2, ForceMode.Acceleration);
			if (this.rb.useGravity)
			{
				float gravityCompensation = Mathf.Lerp(this.gravityCompensationRange.x, this.gravityCompensationRange.y, Mathf.InverseLerp(0f, num, num5 / x));
				RCVehicle.AddScaledGravityCompensationForce(this.rb, x, gravityCompensation);
			}
		}

		// Token: 0x06005F9C RID: 24476 RVA: 0x001E54C0 File Offset: 0x001E36C0
		private void OnCollisionEnter(Collision collision)
		{
			if (base.HasLocalAuthority && this.localState == RCVehicle.State.Mobilized)
			{
				for (int i = 0; i < collision.contactCount; i++)
				{
					ContactPoint contact = collision.GetContact(i);
					if (!this.nonCrashColliders.Contains(contact.thisCollider))
					{
						this.AuthorityBeginCrash();
					}
				}
				return;
			}
			bool flag = collision.collider.gameObject.IsOnLayer(UnityLayer.GorillaThrowable);
			bool flag2 = collision.collider.gameObject.IsOnLayer(UnityLayer.GorillaHand);
			if ((flag || flag2) && this.localState == RCVehicle.State.Mobilized)
			{
				Vector3 vector = Vector3.zero;
				if (flag2)
				{
					GorillaHandClimber component = collision.collider.gameObject.GetComponent<GorillaHandClimber>();
					if (component != null)
					{
						vector = ((component.xrNode == XRNode.LeftHand) ? GTPlayer.Instance.leftHandCenterVelocityTracker : GTPlayer.Instance.rightHandCenterVelocityTracker).GetAverageVelocity(true, 0.15f, false);
					}
				}
				else if (collision.rigidbody != null)
				{
					vector = collision.rigidbody.velocity;
				}
				if (flag || vector.sqrMagnitude > 0.01f)
				{
					if (base.HasLocalAuthority)
					{
						this.AuthorityApplyImpact(vector, flag);
						return;
					}
					if (this.networkSync != null)
					{
						this.networkSync.photonView.RPC("HitRCVehicleRPC", RpcTarget.Others, new object[]
						{
							vector,
							flag
						});
					}
				}
			}
		}

		// Token: 0x04006AAA RID: 27306
		public Vector2 pitchVelocityTargetMinMax = new Vector2(-180f, 180f);

		// Token: 0x04006AAB RID: 27307
		public Vector2 pitchVelocityRampTimeMinMax = new Vector2(-0.75f, 0.75f);

		// Token: 0x04006AAC RID: 27308
		public float rollVelocityTarget = 180f;

		// Token: 0x04006AAD RID: 27309
		public float rollVelocityRampTime = 0.75f;

		// Token: 0x04006AAE RID: 27310
		public float thrustVelocityTarget = 15f;

		// Token: 0x04006AAF RID: 27311
		public float thrustAccelTime = 2f;

		// Token: 0x04006AB0 RID: 27312
		[SerializeField]
		private float pitchVelocityFollowRateAngle = 60f;

		// Token: 0x04006AB1 RID: 27313
		[SerializeField]
		private float pitchVelocityFollowRateMagnitude = 5f;

		// Token: 0x04006AB2 RID: 27314
		[SerializeField]
		private float maxDrag = 0.1f;

		// Token: 0x04006AB3 RID: 27315
		[SerializeField]
		private Vector2 liftVsSpeedInput = new Vector2(0f, 4f);

		// Token: 0x04006AB4 RID: 27316
		[SerializeField]
		private Vector2 liftVsSpeedOutput = new Vector2(0.5f, 1f);

		// Token: 0x04006AB5 RID: 27317
		[SerializeField]
		private AnimationCurve liftVsAttackCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04006AB6 RID: 27318
		[SerializeField]
		private AnimationCurve dragVsAttackCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04006AB7 RID: 27319
		[SerializeField]
		private Vector2 gravityCompensationRange = new Vector2(0.5f, 1f);

		// Token: 0x04006AB8 RID: 27320
		[SerializeField]
		private List<Collider> nonCrashColliders = new List<Collider>();

		// Token: 0x04006AB9 RID: 27321
		[SerializeField]
		private Transform propeller;

		// Token: 0x04006ABA RID: 27322
		[SerializeField]
		private Transform leftAileronUpper;

		// Token: 0x04006ABB RID: 27323
		[SerializeField]
		private Transform leftAileronLower;

		// Token: 0x04006ABC RID: 27324
		[SerializeField]
		private Transform rightAileronUpper;

		// Token: 0x04006ABD RID: 27325
		[SerializeField]
		private Transform rightAileronLower;

		// Token: 0x04006ABE RID: 27326
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04006ABF RID: 27327
		[SerializeField]
		private AudioClip motorSound;

		// Token: 0x04006AC0 RID: 27328
		[SerializeField]
		private AudioClip crashSound;

		// Token: 0x04006AC1 RID: 27329
		[SerializeField]
		private Vector2 motorSoundVolumeMinMax = new Vector2(0.02f, 0.1f);

		// Token: 0x04006AC2 RID: 27330
		[SerializeField]
		private float crashSoundVolume = 0.12f;

		// Token: 0x04006AC3 RID: 27331
		private float motorVolumeRampTime = 1f;

		// Token: 0x04006AC4 RID: 27332
		private float propellerAngle;

		// Token: 0x04006AC5 RID: 27333
		private float propellerSpinRate;

		// Token: 0x04006AC6 RID: 27334
		private const float propellerIdleAcc = 1f;

		// Token: 0x04006AC7 RID: 27335
		private const float propellerIdleSpinRate = 0.6f;

		// Token: 0x04006AC8 RID: 27336
		private const float propellerMaxAcc = 6.6666665f;

		// Token: 0x04006AC9 RID: 27337
		private const float propellerMaxSpinRate = 5f;

		// Token: 0x04006ACA RID: 27338
		public float initialSpeed = 3f;

		// Token: 0x04006ACB RID: 27339
		private float pitch;

		// Token: 0x04006ACC RID: 27340
		private float pitchVel;

		// Token: 0x04006ACD RID: 27341
		private Vector2 pitchAccelMinMax;

		// Token: 0x04006ACE RID: 27342
		private float roll;

		// Token: 0x04006ACF RID: 27343
		private float rollVel;

		// Token: 0x04006AD0 RID: 27344
		private float rollAccel;

		// Token: 0x04006AD1 RID: 27345
		private float thrustAccel;

		// Token: 0x04006AD2 RID: 27346
		private float motorLevel;

		// Token: 0x04006AD3 RID: 27347
		private float leftAileronLevel;

		// Token: 0x04006AD4 RID: 27348
		private float rightAileronLevel;

		// Token: 0x04006AD5 RID: 27349
		private Vector2 aileronAngularRange = new Vector2(-30f, 45f);

		// Token: 0x04006AD6 RID: 27350
		private float aileronAngularAcc = 120f;

		// Token: 0x04006AD7 RID: 27351
		private float leftAileronAngle;

		// Token: 0x04006AD8 RID: 27352
		private float rightAileronAngle;
	}
}
