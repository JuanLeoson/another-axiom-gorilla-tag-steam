using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F14 RID: 3860
	public class RCDragon : RCVehicle
	{
		// Token: 0x06005F82 RID: 24450 RVA: 0x001E3AD8 File Offset: 0x001E1CD8
		protected override void AuthorityBeginDocked()
		{
			base.AuthorityBeginDocked();
			this.turnRate = 0f;
			this.turnAngle = Vector3.SignedAngle(Vector3.forward, Vector3.ProjectOnPlane(base.transform.forward, Vector3.up), Vector3.up);
			this.motorLevel = 0f;
			if (this.connectedRemote == null)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x06005F83 RID: 24451 RVA: 0x001E3B48 File Offset: 0x001E1D48
		protected override void Awake()
		{
			base.Awake();
			this.ascendAccel = this.maxAscendSpeed / this.ascendAccelTime;
			this.turnAccel = this.maxTurnRate / this.turnAccelTime;
			this.horizontalAccel = this.maxHorizontalSpeed / this.horizontalAccelTime;
			this.tiltAccel = this.maxHorizontalTiltAngle / this.horizontalTiltTime;
			this.shouldFlap = false;
			this.isFlapping = false;
			this.StopBreathFire();
			if (this.animation != null)
			{
				this.animation[this.wingFlapAnimName].speed = this.wingFlapAnimSpeed;
				this.animation[this.crashAnimName].speed = this.crashAnimSpeed;
				this.animation[this.mouthClosedAnimName].layer = 1;
				this.animation[this.mouthBreathFireAnimName].layer = 1;
			}
			this.nextFlapEventAnimTime = this.flapAnimEventTime;
		}

		// Token: 0x06005F84 RID: 24452 RVA: 0x001E3C3B File Offset: 0x001E1E3B
		protected override void OnDisable()
		{
			base.OnDisable();
			this.audioSource.GTStop();
		}

		// Token: 0x06005F85 RID: 24453 RVA: 0x001E3C50 File Offset: 0x001E1E50
		public void StartBreathFire()
		{
			if (!string.IsNullOrEmpty(this.mouthBreathFireAnimName))
			{
				this.animation.CrossFade(this.mouthBreathFireAnimName, 0.1f);
			}
			if (this.fireBreath != null)
			{
				this.fireBreath.SetActive(true);
			}
			this.PlayRandomSound(this.breathFireSound, this.breathFireVolume);
			this.fireBreathTimeRemaining = this.fireBreathDuration;
		}

		// Token: 0x06005F86 RID: 24454 RVA: 0x001E3CB8 File Offset: 0x001E1EB8
		public void StopBreathFire()
		{
			if (!string.IsNullOrEmpty(this.mouthClosedAnimName))
			{
				this.animation.CrossFade(this.mouthClosedAnimName, 0.1f);
			}
			if (this.fireBreath != null)
			{
				this.fireBreath.SetActive(false);
			}
			this.fireBreathTimeRemaining = -1f;
		}

		// Token: 0x06005F87 RID: 24455 RVA: 0x001E3D0D File Offset: 0x001E1F0D
		public bool IsBreathingFire()
		{
			return this.fireBreathTimeRemaining >= 0f;
		}

		// Token: 0x06005F88 RID: 24456 RVA: 0x001E3D1F File Offset: 0x001E1F1F
		private void PlayRandomSound(List<AudioClip> clips, float volume)
		{
			if (clips == null || clips.Count == 0)
			{
				return;
			}
			this.PlaySound(clips[Random.Range(0, clips.Count)], volume);
		}

		// Token: 0x06005F89 RID: 24457 RVA: 0x001E3D48 File Offset: 0x001E1F48
		private void PlaySound(AudioClip clip, float volume)
		{
			if (this.audioSource == null || clip == null)
			{
				return;
			}
			this.audioSource.GTStop();
			this.audioSource.clip = null;
			this.audioSource.loop = false;
			this.audioSource.volume = volume;
			this.audioSource.GTPlayOneShot(clip, 1f);
		}

		// Token: 0x06005F8A RID: 24458 RVA: 0x001E3DB0 File Offset: 0x001E1FB0
		protected override void AuthorityUpdate(float dt)
		{
			base.AuthorityUpdate(dt);
			this.motorLevel = 0f;
			if (this.localState == RCVehicle.State.Mobilized)
			{
				this.motorLevel = Mathf.Max(Mathf.Max(Mathf.Abs(this.activeInput.joystick.y), Mathf.Abs(this.activeInput.joystick.x)), this.activeInput.trigger);
				if (!this.IsBreathingFire() && this.activeInput.buttons > 0)
				{
					this.StartBreathFire();
				}
			}
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.dataA = (byte)Mathf.Clamp(Mathf.FloorToInt(this.motorLevel * 255f), 0, 255);
				this.networkSync.syncedState.dataB = this.activeInput.buttons;
				this.networkSync.syncedState.dataC = (this.shouldFlap ? 1 : 0);
			}
		}

		// Token: 0x06005F8B RID: 24459 RVA: 0x001E3EAC File Offset: 0x001E20AC
		protected override void RemoteUpdate(float dt)
		{
			base.RemoteUpdate(dt);
			if (this.localState == RCVehicle.State.Mobilized && this.networkSync != null)
			{
				this.motorLevel = Mathf.Clamp01((float)this.networkSync.syncedState.dataA / 255f);
				if (!this.IsBreathingFire() && this.networkSync.syncedState.dataB > 0)
				{
					this.StartBreathFire();
				}
				this.shouldFlap = (this.networkSync.syncedState.dataC > 0);
			}
		}

		// Token: 0x06005F8C RID: 24460 RVA: 0x001E3F34 File Offset: 0x001E2134
		protected override void SharedUpdate(float dt)
		{
			base.SharedUpdate(dt);
			switch (this.localState)
			{
			case RCVehicle.State.Disabled:
				break;
			case RCVehicle.State.DockedLeft:
			case RCVehicle.State.DockedRight:
				if (this.localStatePrev != RCVehicle.State.DockedLeft && this.localStatePrev != RCVehicle.State.DockedRight)
				{
					this.audioSource.GTStop();
					if (this.crashCollider != null)
					{
						this.crashCollider.enabled = false;
					}
					if (this.animation != null)
					{
						this.animation.Play(this.dockedAnimName);
					}
					if (this.IsBreathingFire())
					{
						this.StopBreathFire();
						return;
					}
				}
				break;
			case RCVehicle.State.Mobilized:
			{
				if (this.localStatePrev != RCVehicle.State.Mobilized && this.crashCollider != null)
				{
					this.crashCollider.enabled = false;
				}
				if (this.animation != null)
				{
					if (!this.isFlapping && this.shouldFlap)
					{
						this.animation.CrossFade(this.wingFlapAnimName, 0.1f);
						this.nextFlapEventAnimTime = this.flapAnimEventTime;
					}
					else if (this.isFlapping && !this.shouldFlap)
					{
						this.animation.CrossFade(this.idleAnimName, 0.15f);
					}
					this.isFlapping = this.shouldFlap;
					if (this.isFlapping && !this.IsBreathingFire())
					{
						AnimationState animationState = this.animation[this.wingFlapAnimName];
						if (animationState.normalizedTime * animationState.length > this.nextFlapEventAnimTime)
						{
							this.PlayRandomSound(this.wingFlapSound, this.wingFlapVolume);
							this.nextFlapEventAnimTime = (Mathf.Floor(animationState.normalizedTime) + 1f) * animationState.length + this.flapAnimEventTime;
						}
					}
				}
				GTTime.TimeAsDouble();
				if (this.IsBreathingFire())
				{
					this.fireBreathTimeRemaining -= dt;
					if (this.fireBreathTimeRemaining <= 0f)
					{
						this.StopBreathFire();
					}
				}
				float target = Mathf.Lerp(this.motorSoundVolumeMinMax.x, this.motorSoundVolumeMinMax.y, this.motorLevel);
				this.audioSource.volume = Mathf.MoveTowards(this.audioSource.volume, target, this.motorSoundVolumeMinMax.y / this.motorVolumeRampTime * dt);
				break;
			}
			case RCVehicle.State.Crashed:
				if (this.localStatePrev != RCVehicle.State.Crashed)
				{
					this.PlaySound(this.crashSound, this.crashSoundVolume);
					if (this.crashCollider != null)
					{
						this.crashCollider.enabled = true;
					}
					if (this.animation != null)
					{
						this.animation.CrossFade(this.crashAnimName, 0.05f);
					}
					if (this.IsBreathingFire())
					{
						this.StopBreathFire();
						return;
					}
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06005F8D RID: 24461 RVA: 0x001E41D0 File Offset: 0x001E23D0
		private void FixedUpdate()
		{
			if (!base.HasLocalAuthority)
			{
				return;
			}
			float x = base.transform.lossyScale.x;
			float fixedDeltaTime = Time.fixedDeltaTime;
			this.shouldFlap = false;
			if (this.localState == RCVehicle.State.Mobilized)
			{
				float num = this.maxAscendSpeed * x;
				float num2 = this.maxHorizontalSpeed * x;
				float d = this.ascendAccel * x;
				float d2 = this.ascendWhileFlyingAccelBoost * x;
				float num3 = 0.5f * x;
				float num4 = 45f;
				Vector3 velocity = this.rb.velocity;
				Vector3 normalized = new Vector3(base.transform.forward.x, 0f, base.transform.forward.z).normalized;
				this.turnAngle = Vector3.SignedAngle(Vector3.forward, normalized, Vector3.up);
				this.tiltAngle = Vector3.SignedAngle(normalized, base.transform.forward, base.transform.right);
				float target = this.activeInput.joystick.x * this.maxTurnRate;
				this.turnRate = Mathf.MoveTowards(this.turnRate, target, this.turnAccel * fixedDeltaTime);
				this.turnAngle += this.turnRate * fixedDeltaTime;
				float num5 = Vector3.Dot(normalized, velocity);
				float t = Mathf.InverseLerp(-num2, num2, num5);
				float target2 = Mathf.Lerp(-this.maxHorizontalTiltAngle, this.maxHorizontalTiltAngle, t);
				this.tiltAngle = Mathf.MoveTowards(this.tiltAngle, target2, this.tiltAccel * fixedDeltaTime);
				base.transform.rotation = Quaternion.Euler(new Vector3(this.tiltAngle, this.turnAngle, 0f));
				Vector3 b = new Vector3(velocity.x, 0f, velocity.z);
				Vector3 a = Vector3.Lerp(normalized * this.activeInput.joystick.y * num2, b, Mathf.Exp(-this.horizontalAccelTime * fixedDeltaTime));
				this.rb.AddForce(a - b, ForceMode.VelocityChange);
				float num6 = this.activeInput.trigger * num;
				if (num6 > 0.01f && velocity.y < num6)
				{
					this.rb.AddForce(Vector3.up * d, ForceMode.Acceleration);
				}
				bool flag = Mathf.Abs(num5) > num3;
				bool flag2 = Mathf.Abs(this.turnRate) > num4;
				if (flag || flag2)
				{
					this.rb.AddForce(Vector3.up * d2, ForceMode.Acceleration);
				}
				this.shouldFlap = (num6 > 0.01f || flag || flag2);
				if (this.rb.useGravity)
				{
					RCVehicle.AddScaledGravityCompensationForce(this.rb, x, this.gravityCompensation);
					return;
				}
			}
			else if (this.localState == RCVehicle.State.Crashed && this.rb.useGravity)
			{
				RCVehicle.AddScaledGravityCompensationForce(this.rb, x, this.crashedGravityCompensation);
			}
		}

		// Token: 0x06005F8E RID: 24462 RVA: 0x001E44B4 File Offset: 0x001E26B4
		private void OnTriggerEnter(Collider other)
		{
			bool flag = other.gameObject.IsOnLayer(UnityLayer.GorillaThrowable);
			bool flag2 = other.gameObject.IsOnLayer(UnityLayer.GorillaHand);
			if (!other.isTrigger && base.HasLocalAuthority && this.localState == RCVehicle.State.Mobilized)
			{
				this.AuthorityBeginCrash();
				return;
			}
			if ((flag || flag2) && this.localState == RCVehicle.State.Mobilized)
			{
				Vector3 vector = Vector3.zero;
				if (flag2)
				{
					GorillaHandClimber component = other.gameObject.GetComponent<GorillaHandClimber>();
					if (component != null)
					{
						vector = ((component.xrNode == XRNode.LeftHand) ? GTPlayer.Instance.leftHandCenterVelocityTracker : GTPlayer.Instance.rightHandCenterVelocityTracker).GetAverageVelocity(true, 0.15f, false);
					}
				}
				else if (other.attachedRigidbody != null)
				{
					vector = other.attachedRigidbody.velocity;
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

		// Token: 0x04006A6B RID: 27243
		[SerializeField]
		private float maxAscendSpeed = 6f;

		// Token: 0x04006A6C RID: 27244
		[SerializeField]
		private float ascendAccelTime = 3f;

		// Token: 0x04006A6D RID: 27245
		[SerializeField]
		private float ascendWhileFlyingAccelBoost;

		// Token: 0x04006A6E RID: 27246
		[SerializeField]
		private float gravityCompensation = 0.9f;

		// Token: 0x04006A6F RID: 27247
		[SerializeField]
		private float crashedGravityCompensation = 0.5f;

		// Token: 0x04006A70 RID: 27248
		[SerializeField]
		private float maxTurnRate = 90f;

		// Token: 0x04006A71 RID: 27249
		[SerializeField]
		private float turnAccelTime = 0.75f;

		// Token: 0x04006A72 RID: 27250
		[SerializeField]
		private float maxHorizontalSpeed = 6f;

		// Token: 0x04006A73 RID: 27251
		[SerializeField]
		private float horizontalAccelTime = 2f;

		// Token: 0x04006A74 RID: 27252
		[SerializeField]
		private float maxHorizontalTiltAngle = 45f;

		// Token: 0x04006A75 RID: 27253
		[SerializeField]
		private float horizontalTiltTime = 2f;

		// Token: 0x04006A76 RID: 27254
		[SerializeField]
		private Vector2 motorSoundVolumeMinMax = new Vector2(0.1f, 0.8f);

		// Token: 0x04006A77 RID: 27255
		[SerializeField]
		private float crashSoundVolume = 0.1f;

		// Token: 0x04006A78 RID: 27256
		[SerializeField]
		private float breathFireVolume = 0.5f;

		// Token: 0x04006A79 RID: 27257
		[SerializeField]
		private float wingFlapVolume = 0.1f;

		// Token: 0x04006A7A RID: 27258
		[SerializeField]
		private Animation animation;

		// Token: 0x04006A7B RID: 27259
		[SerializeField]
		private string wingFlapAnimName;

		// Token: 0x04006A7C RID: 27260
		[SerializeField]
		private float wingFlapAnimSpeed = 1f;

		// Token: 0x04006A7D RID: 27261
		[SerializeField]
		private string dockedAnimName;

		// Token: 0x04006A7E RID: 27262
		[SerializeField]
		private string idleAnimName;

		// Token: 0x04006A7F RID: 27263
		[SerializeField]
		private string crashAnimName;

		// Token: 0x04006A80 RID: 27264
		[SerializeField]
		private float crashAnimSpeed = 1f;

		// Token: 0x04006A81 RID: 27265
		[SerializeField]
		private string mouthClosedAnimName;

		// Token: 0x04006A82 RID: 27266
		[SerializeField]
		private string mouthBreathFireAnimName;

		// Token: 0x04006A83 RID: 27267
		private bool shouldFlap;

		// Token: 0x04006A84 RID: 27268
		private bool isFlapping;

		// Token: 0x04006A85 RID: 27269
		private float nextFlapEventAnimTime;

		// Token: 0x04006A86 RID: 27270
		[SerializeField]
		private float flapAnimEventTime = 0.25f;

		// Token: 0x04006A87 RID: 27271
		[SerializeField]
		private GameObject fireBreath;

		// Token: 0x04006A88 RID: 27272
		[SerializeField]
		private float fireBreathDuration;

		// Token: 0x04006A89 RID: 27273
		private float fireBreathTimeRemaining;

		// Token: 0x04006A8A RID: 27274
		[SerializeField]
		private Collider crashCollider;

		// Token: 0x04006A8B RID: 27275
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04006A8C RID: 27276
		[SerializeField]
		private List<AudioClip> breathFireSound;

		// Token: 0x04006A8D RID: 27277
		[SerializeField]
		private List<AudioClip> wingFlapSound;

		// Token: 0x04006A8E RID: 27278
		[SerializeField]
		private AudioClip crashSound;

		// Token: 0x04006A8F RID: 27279
		private float turnRate;

		// Token: 0x04006A90 RID: 27280
		private float turnAngle;

		// Token: 0x04006A91 RID: 27281
		private float tiltAngle;

		// Token: 0x04006A92 RID: 27282
		private float ascendAccel;

		// Token: 0x04006A93 RID: 27283
		private float turnAccel;

		// Token: 0x04006A94 RID: 27284
		private float tiltAccel;

		// Token: 0x04006A95 RID: 27285
		private float horizontalAccel;

		// Token: 0x04006A96 RID: 27286
		private float motorVolumeRampTime = 1f;

		// Token: 0x04006A97 RID: 27287
		private float motorLevel;
	}
}
