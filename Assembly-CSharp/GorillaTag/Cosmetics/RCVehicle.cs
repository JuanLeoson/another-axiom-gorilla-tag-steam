using System;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F19 RID: 3865
	public class RCVehicle : MonoBehaviour, ISpawnable
	{
		// Token: 0x17000943 RID: 2371
		// (get) Token: 0x06005FAD RID: 24493 RVA: 0x001E5EBC File Offset: 0x001E40BC
		public bool HasLocalAuthority
		{
			get
			{
				return !PhotonNetwork.InRoom || (this.networkSync != null && this.networkSync.photonView.IsMine);
			}
		}

		// Token: 0x06005FAE RID: 24494 RVA: 0x001E5EE8 File Offset: 0x001E40E8
		public virtual void WakeUpRemote(RCCosmeticNetworkSync sync)
		{
			this.networkSync = sync;
			this.hasNetworkSync = (sync != null);
			if (this.HasLocalAuthority)
			{
				return;
			}
			if (!base.enabled || !base.gameObject.activeSelf)
			{
				this.localStatePrev = RCVehicle.State.Disabled;
				base.enabled = true;
				base.gameObject.SetActive(true);
				this.RemoteUpdate(Time.deltaTime);
			}
		}

		// Token: 0x06005FAF RID: 24495 RVA: 0x001E5F4C File Offset: 0x001E414C
		public virtual void StartConnection(RCRemoteHoldable remote, RCCosmeticNetworkSync sync)
		{
			this.connectedRemote = remote;
			this.networkSync = sync;
			this.hasNetworkSync = (sync != null);
			base.enabled = true;
			base.gameObject.SetActive(true);
			this.useLeftDock = (remote.XRNode == XRNode.LeftHand);
			if (this.HasLocalAuthority && this.localState != RCVehicle.State.Mobilized)
			{
				this.AuthorityBeginDocked();
			}
		}

		// Token: 0x06005FB0 RID: 24496 RVA: 0x001E5FAD File Offset: 0x001E41AD
		public virtual void EndConnection()
		{
			this.connectedRemote = null;
			this.activeInput = default(RCRemoteHoldable.RCInput);
			this.disconnectionTime = Time.time;
		}

		// Token: 0x06005FB1 RID: 24497 RVA: 0x001E5FD0 File Offset: 0x001E41D0
		protected virtual void ResetToSpawnPosition()
		{
			if (this.rb == null)
			{
				this.rb = base.GetComponent<Rigidbody>();
			}
			if (this.rb != null)
			{
				this.rb.isKinematic = true;
			}
			base.transform.parent = (this.useLeftDock ? this.leftDockParent : this.rightDockParent);
			base.transform.SetLocalPositionAndRotation(this.useLeftDock ? this.dockLeftOffset.pos : this.dockRightOffset.pos, this.useLeftDock ? this.dockLeftOffset.rot : this.dockRightOffset.rot);
			base.transform.localScale = (this.useLeftDock ? this.dockLeftOffset.scale : this.dockRightOffset.scale);
		}

		// Token: 0x06005FB2 RID: 24498 RVA: 0x001E60A8 File Offset: 0x001E42A8
		protected virtual void AuthorityBeginDocked()
		{
			this.localState = (this.useLeftDock ? RCVehicle.State.DockedLeft : RCVehicle.State.DockedRight);
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.state = (byte)this.localState;
			}
			this.stateStartTime = Time.time;
			this.waitingForTriggerRelease = true;
			this.ResetToSpawnPosition();
			if (this.connectedRemote == null)
			{
				this.SetDisabledState();
			}
		}

		// Token: 0x06005FB3 RID: 24499 RVA: 0x001E6118 File Offset: 0x001E4318
		protected virtual void AuthorityBeginMobilization()
		{
			this.localState = RCVehicle.State.Mobilized;
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.state = (byte)this.localState;
			}
			this.stateStartTime = Time.time;
			base.transform.parent = null;
			this.rb.isKinematic = false;
		}

		// Token: 0x06005FB4 RID: 24500 RVA: 0x001E6174 File Offset: 0x001E4374
		protected virtual void AuthorityBeginCrash()
		{
			this.localState = RCVehicle.State.Crashed;
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.state = (byte)this.localState;
			}
			this.stateStartTime = Time.time;
		}

		// Token: 0x06005FB5 RID: 24501 RVA: 0x001E61B0 File Offset: 0x001E43B0
		protected virtual void SetDisabledState()
		{
			this.localState = RCVehicle.State.Disabled;
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.state = (byte)this.localState;
			}
			this.ResetToSpawnPosition();
			base.enabled = false;
			base.gameObject.SetActive(false);
		}

		// Token: 0x06005FB6 RID: 24502 RVA: 0x001E6202 File Offset: 0x001E4402
		protected virtual void Awake()
		{
			this.rb = base.GetComponent<Rigidbody>();
		}

		// Token: 0x06005FB7 RID: 24503 RVA: 0x000023F5 File Offset: 0x000005F5
		protected virtual void OnEnable()
		{
		}

		// Token: 0x17000944 RID: 2372
		// (get) Token: 0x06005FB8 RID: 24504 RVA: 0x001E6210 File Offset: 0x001E4410
		// (set) Token: 0x06005FB9 RID: 24505 RVA: 0x001E6218 File Offset: 0x001E4418
		bool ISpawnable.IsSpawned { get; set; }

		// Token: 0x17000945 RID: 2373
		// (get) Token: 0x06005FBA RID: 24506 RVA: 0x001E6221 File Offset: 0x001E4421
		// (set) Token: 0x06005FBB RID: 24507 RVA: 0x001E6229 File Offset: 0x001E4429
		ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

		// Token: 0x06005FBC RID: 24508 RVA: 0x001E6234 File Offset: 0x001E4434
		void ISpawnable.OnSpawn(VRRig rig)
		{
			if (rig == null)
			{
				GTDev.LogError<string>("RCVehicle: Could not find VRRig in parents. If you are trying to make this a world item rather than a cosmetic then you'll have to refactor how it teleports back to the arms.", this, null);
				return;
			}
			string str;
			if (!GTHardCodedBones.TryGetBoneXforms(rig, out this._vrRigBones, out str))
			{
				Debug.LogError("RCVehicle: " + str, this);
				return;
			}
			if (this.leftDockParent == null && !GTHardCodedBones.TryGetBoneXform(this._vrRigBones, this.dockLeftOffset.bone, out this.leftDockParent))
			{
				GTDev.LogError<string>("RCVehicle: Could not find left dock transform.", this, null);
			}
			if (this.rightDockParent == null && !GTHardCodedBones.TryGetBoneXform(this._vrRigBones, this.dockRightOffset.bone, out this.rightDockParent))
			{
				GTDev.LogError<string>("RCVehicle: Could not find right dock transform.", this, null);
			}
		}

		// Token: 0x06005FBD RID: 24509 RVA: 0x000023F5 File Offset: 0x000005F5
		void ISpawnable.OnDespawn()
		{
		}

		// Token: 0x06005FBE RID: 24510 RVA: 0x001E62EF File Offset: 0x001E44EF
		protected virtual void OnDisable()
		{
			this.localState = RCVehicle.State.Disabled;
			this.localStatePrev = RCVehicle.State.Disabled;
		}

		// Token: 0x06005FBF RID: 24511 RVA: 0x001E6300 File Offset: 0x001E4500
		public void ApplyRemoteControlInput(RCRemoteHoldable.RCInput rcInput)
		{
			this.activeInput.joystick.y = Mathf.Sign(rcInput.joystick.y) * Mathf.Lerp(0f, 1f, Mathf.InverseLerp(this.joystickDeadzone, 1f, Mathf.Abs(rcInput.joystick.y)));
			this.activeInput.joystick.x = Mathf.Sign(rcInput.joystick.x) * Mathf.Lerp(0f, 1f, Mathf.InverseLerp(this.joystickDeadzone, 1f, Mathf.Abs(rcInput.joystick.x)));
			this.activeInput.trigger = Mathf.Clamp(rcInput.trigger, -1f, 1f);
			this.activeInput.buttons = rcInput.buttons;
		}

		// Token: 0x06005FC0 RID: 24512 RVA: 0x001E63E0 File Offset: 0x001E45E0
		private void Update()
		{
			float deltaTime = Time.deltaTime;
			if (this.HasLocalAuthority)
			{
				this.AuthorityUpdate(deltaTime);
			}
			else
			{
				this.RemoteUpdate(deltaTime);
			}
			this.SharedUpdate(deltaTime);
			this.localStatePrev = this.localState;
		}

		// Token: 0x06005FC1 RID: 24513 RVA: 0x001E6420 File Offset: 0x001E4620
		protected virtual void AuthorityUpdate(float dt)
		{
			switch (this.localState)
			{
			default:
				if (this.localState != this.localStatePrev)
				{
					this.ResetToSpawnPosition();
				}
				if (this.connectedRemote == null)
				{
					this.SetDisabledState();
					return;
				}
				if (this.waitingForTriggerRelease && this.activeInput.trigger < 0.25f)
				{
					this.waitingForTriggerRelease = false;
				}
				if (!this.waitingForTriggerRelease && this.activeInput.trigger > 0.25f)
				{
					this.AuthorityBeginMobilization();
					return;
				}
				break;
			case RCVehicle.State.Mobilized:
			{
				if (this.networkSync != null)
				{
					this.networkSync.syncedState.position = base.transform.position;
					this.networkSync.syncedState.rotation = base.transform.rotation;
				}
				bool flag = (base.transform.position - this.leftDockParent.position).sqrMagnitude > this.maxRange * this.maxRange;
				bool flag2 = this.connectedRemote == null && Time.time - this.disconnectionTime > this.maxDisconnectionTime;
				if (flag || flag2)
				{
					this.AuthorityBeginCrash();
					return;
				}
				break;
			}
			case RCVehicle.State.Crashed:
				if (Time.time > this.stateStartTime + this.crashRespawnDelay)
				{
					this.AuthorityBeginDocked();
				}
				break;
			}
		}

		// Token: 0x06005FC2 RID: 24514 RVA: 0x001E6580 File Offset: 0x001E4780
		protected virtual void RemoteUpdate(float dt)
		{
			if (this.networkSync == null)
			{
				this.SetDisabledState();
				return;
			}
			this.localState = (RCVehicle.State)this.networkSync.syncedState.state;
			switch (this.localState)
			{
			case RCVehicle.State.Disabled:
				this.SetDisabledState();
				break;
			default:
				if (this.localStatePrev != RCVehicle.State.DockedLeft)
				{
					this.useLeftDock = true;
					this.ResetToSpawnPosition();
					return;
				}
				break;
			case RCVehicle.State.DockedRight:
				if (this.localStatePrev != RCVehicle.State.DockedRight)
				{
					this.useLeftDock = false;
					this.ResetToSpawnPosition();
					return;
				}
				break;
			case RCVehicle.State.Mobilized:
				if (this.localStatePrev != RCVehicle.State.Mobilized)
				{
					this.rb.isKinematic = true;
					base.transform.parent = null;
				}
				base.transform.position = Vector3.Lerp(this.networkSync.syncedState.position, base.transform.position, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
				base.transform.rotation = Quaternion.Slerp(this.networkSync.syncedState.rotation, base.transform.rotation, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
				return;
			case RCVehicle.State.Crashed:
				if (this.localStatePrev != RCVehicle.State.Crashed)
				{
					this.rb.isKinematic = false;
					base.transform.parent = null;
					if (this.localStatePrev != RCVehicle.State.Mobilized)
					{
						base.transform.position = this.networkSync.syncedState.position;
						base.transform.rotation = this.networkSync.syncedState.rotation;
						return;
					}
				}
				break;
			}
		}

		// Token: 0x06005FC3 RID: 24515 RVA: 0x000023F5 File Offset: 0x000005F5
		protected virtual void SharedUpdate(float dt)
		{
		}

		// Token: 0x06005FC4 RID: 24516 RVA: 0x001E6708 File Offset: 0x001E4908
		public virtual void AuthorityApplyImpact(Vector3 hitVelocity, bool isProjectile)
		{
			if (this.HasLocalAuthority && this.localState == RCVehicle.State.Mobilized)
			{
				float d = isProjectile ? this.projectileVelocityTransfer : this.hitVelocityTransfer;
				this.rb.AddForce(Vector3.ClampMagnitude(hitVelocity * d, this.hitMaxHitSpeed), ForceMode.VelocityChange);
				if (isProjectile || (this.crashOnHit && hitVelocity.sqrMagnitude > this.crashOnHitSpeedThreshold * this.crashOnHitSpeedThreshold))
				{
					this.AuthorityBeginCrash();
				}
			}
		}

		// Token: 0x06005FC5 RID: 24517 RVA: 0x001429B1 File Offset: 0x00140BB1
		protected float NormalizeAngle180(float angle)
		{
			angle = (angle + 180f) % 360f;
			if (angle < 0f)
			{
				angle += 360f;
			}
			return angle - 180f;
		}

		// Token: 0x06005FC6 RID: 24518 RVA: 0x001E6780 File Offset: 0x001E4980
		protected static void AddScaledGravityCompensationForce(Rigidbody rb, float scaleFactor, float gravityCompensation)
		{
			Vector3 gravity = Physics.gravity;
			Vector3 vector = -gravity * gravityCompensation;
			Vector3 vector2 = gravity + vector;
			Vector3 b = vector2 * scaleFactor - vector2;
			rb.AddForce(vector + b, ForceMode.Acceleration);
		}

		// Token: 0x04006AEE RID: 27374
		[SerializeField]
		private Transform leftDockParent;

		// Token: 0x04006AEF RID: 27375
		[SerializeField]
		private Transform rightDockParent;

		// Token: 0x04006AF0 RID: 27376
		[SerializeField]
		private float maxRange = 100f;

		// Token: 0x04006AF1 RID: 27377
		[SerializeField]
		private float maxDisconnectionTime = 10f;

		// Token: 0x04006AF2 RID: 27378
		[SerializeField]
		private float crashRespawnDelay = 3f;

		// Token: 0x04006AF3 RID: 27379
		[SerializeField]
		private bool crashOnHit;

		// Token: 0x04006AF4 RID: 27380
		[SerializeField]
		private float crashOnHitSpeedThreshold = 5f;

		// Token: 0x04006AF5 RID: 27381
		[SerializeField]
		[Range(0f, 1f)]
		private float hitVelocityTransfer = 0.5f;

		// Token: 0x04006AF6 RID: 27382
		[SerializeField]
		[Range(0f, 1f)]
		private float projectileVelocityTransfer = 0.1f;

		// Token: 0x04006AF7 RID: 27383
		[SerializeField]
		private float hitMaxHitSpeed = 4f;

		// Token: 0x04006AF8 RID: 27384
		[SerializeField]
		[Range(0f, 1f)]
		private float joystickDeadzone = 0.1f;

		// Token: 0x04006AF9 RID: 27385
		protected RCVehicle.State localState;

		// Token: 0x04006AFA RID: 27386
		protected RCVehicle.State localStatePrev;

		// Token: 0x04006AFB RID: 27387
		protected float stateStartTime;

		// Token: 0x04006AFC RID: 27388
		protected RCRemoteHoldable connectedRemote;

		// Token: 0x04006AFD RID: 27389
		protected RCCosmeticNetworkSync networkSync;

		// Token: 0x04006AFE RID: 27390
		protected bool hasNetworkSync;

		// Token: 0x04006AFF RID: 27391
		protected RCRemoteHoldable.RCInput activeInput;

		// Token: 0x04006B00 RID: 27392
		protected Rigidbody rb;

		// Token: 0x04006B01 RID: 27393
		private bool waitingForTriggerRelease;

		// Token: 0x04006B02 RID: 27394
		private float disconnectionTime;

		// Token: 0x04006B03 RID: 27395
		private bool useLeftDock;

		// Token: 0x04006B04 RID: 27396
		private BoneOffset dockLeftOffset = new BoneOffset(GTHardCodedBones.EBone.forearm_L, new Vector3(-0.062f, 0.283f, -0.136f), new Vector3(275f, 0f, 25f));

		// Token: 0x04006B05 RID: 27397
		private BoneOffset dockRightOffset = new BoneOffset(GTHardCodedBones.EBone.forearm_R, new Vector3(0.069f, 0.265f, -0.128f), new Vector3(275f, 0f, 335f));

		// Token: 0x04006B06 RID: 27398
		private float networkSyncFollowRateExp = 2f;

		// Token: 0x04006B07 RID: 27399
		private Transform[] _vrRigBones;

		// Token: 0x02000F1A RID: 3866
		protected enum State
		{
			// Token: 0x04006B0B RID: 27403
			Disabled,
			// Token: 0x04006B0C RID: 27404
			DockedLeft,
			// Token: 0x04006B0D RID: 27405
			DockedRight,
			// Token: 0x04006B0E RID: 27406
			Mobilized,
			// Token: 0x04006B0F RID: 27407
			Crashed
		}
	}
}
