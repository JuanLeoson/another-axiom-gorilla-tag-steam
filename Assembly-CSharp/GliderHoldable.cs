using System;
using System.Collections;
using System.Runtime.InteropServices;
using AA;
using Fusion;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000A1B RID: 2587
[RequireComponent(typeof(Rigidbody))]
[NetworkBehaviourWeaved(11)]
public class GliderHoldable : NetworkHoldableObject, IRequestableOwnershipGuardCallbacks
{
	// Token: 0x170005FA RID: 1530
	// (get) Token: 0x06003F0A RID: 16138 RVA: 0x00140188 File Offset: 0x0013E388
	private bool OutOfBounds
	{
		get
		{
			return this.maxDistanceRespawnOrigin != null && (this.maxDistanceRespawnOrigin.position - base.transform.position).sqrMagnitude > this.maxDistanceBeforeRespawn * this.maxDistanceBeforeRespawn;
		}
	}

	// Token: 0x06003F0B RID: 16139 RVA: 0x001401D8 File Offset: 0x0013E3D8
	protected override void Awake()
	{
		base.Awake();
		base.transform.parent = null;
		this.defaultMaxDistanceBeforeRespawn = this.maxDistanceBeforeRespawn;
		this.spawnPosition = (this.skyJungleSpawnPostion = base.transform.position);
		this.spawnRotation = (this.skyJungleSpawnRotation = base.transform.rotation);
		this.skyJungleRespawnOrigin = this.maxDistanceRespawnOrigin;
		this.syncedState.Init(this.spawnPosition, this.spawnRotation);
		this.rb = base.GetComponent<Rigidbody>();
		this.yaw = base.transform.rotation.eulerAngles.y;
		this.oneHandRotationRateExp = Mathf.Exp(this.oneHandHoldRotationRate);
		this.twoHandRotationRateExp = Mathf.Exp(this.twoHandHoldRotationRate);
		this.subtlePlayerPitchRateExp = Mathf.Exp(this.subtlePlayerPitchRate);
		this.subtlePlayerRollRateExp = Mathf.Exp(this.subtlePlayerRollRate);
		this.accelSmoothingFollowRateExp = Mathf.Exp(this.accelSmoothingFollowRate);
		this.networkSyncFollowRateExp = Mathf.Exp(this.networkSyncFollowRate);
		this.ownershipGuard.AddCallbackTarget(this);
		this.calmAudio.volume = 0f;
		this.activeAudio.volume = 0f;
		this.whistlingAudio.volume = 0f;
	}

	// Token: 0x06003F0C RID: 16140 RVA: 0x00140326 File Offset: 0x0013E526
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		if (this.ownershipGuard != null)
		{
			this.ownershipGuard.RemoveCallbackTarget(this);
		}
	}

	// Token: 0x06003F0D RID: 16141 RVA: 0x000C5512 File Offset: 0x000C3712
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
	}

	// Token: 0x06003F0E RID: 16142 RVA: 0x00140348 File Offset: 0x0013E548
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		this.Respawn();
		base.OnDisable();
	}

	// Token: 0x06003F0F RID: 16143 RVA: 0x0014035C File Offset: 0x0013E55C
	public void Respawn()
	{
		if ((base.IsValid && base.IsMine) || !NetworkSystem.Instance.InRoom)
		{
			if (EquipmentInteractor.instance != null)
			{
				if (EquipmentInteractor.instance.leftHandHeldEquipment == this)
				{
					this.OnRelease(null, EquipmentInteractor.instance.leftHand);
				}
				if (EquipmentInteractor.instance.rightHandHeldEquipment == this)
				{
					this.OnRelease(null, EquipmentInteractor.instance.rightHand);
				}
			}
			this.rb.isKinematic = true;
			base.transform.position = this.spawnPosition;
			base.transform.rotation = this.spawnRotation;
			this.lastHeldTime = -1f;
			this.syncedState.Init(this.spawnPosition, this.spawnRotation);
		}
	}

	// Token: 0x06003F10 RID: 16144 RVA: 0x0014042D File Offset: 0x0013E62D
	public void CustomMapLoad(Transform placeholderTransform, float respawnDistance)
	{
		this.maxDistanceRespawnOrigin = placeholderTransform;
		this.spawnPosition = placeholderTransform.position;
		this.spawnRotation = placeholderTransform.rotation;
		this.maxDistanceBeforeRespawn = respawnDistance;
		this.Respawn();
	}

	// Token: 0x06003F11 RID: 16145 RVA: 0x0014045B File Offset: 0x0013E65B
	public void CustomMapUnload()
	{
		this.maxDistanceRespawnOrigin = this.skyJungleRespawnOrigin;
		this.spawnPosition = this.skyJungleSpawnPostion;
		this.spawnRotation = this.skyJungleSpawnRotation;
		this.maxDistanceBeforeRespawn = this.defaultMaxDistanceBeforeRespawn;
		this.Respawn();
	}

	// Token: 0x170005FB RID: 1531
	// (get) Token: 0x06003F12 RID: 16146 RVA: 0x0001D558 File Offset: 0x0001B758
	public override bool TwoHanded
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06003F13 RID: 16147 RVA: 0x00140494 File Offset: 0x0013E694
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
		if (!base.IsMine && NetworkSystem.Instance.InRoom && !this.pendingOwnershipRequest && this.syncedState.riderId == -1)
		{
			this.ownershipGuard.RequestOwnershipImmediately(delegate
			{
				this.pendingOwnershipRequest = false;
			});
			this.pendingOwnershipRequest = true;
			if (this.reenableOwnershipRequestCoroutine != null)
			{
				base.StopCoroutine(this.reenableOwnershipRequestCoroutine);
			}
			this.reenableOwnershipRequestCoroutine = base.StartCoroutine(this.ReenableOwnershipRequest());
		}
	}

	// Token: 0x06003F14 RID: 16148 RVA: 0x00140510 File Offset: 0x0013E710
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (base.IsMine || !NetworkSystem.Instance.InRoom || this.pendingOwnershipRequest)
		{
			this.OnGrabAuthority(pointGrabbed, grabbingHand);
			return;
		}
		if (NetworkSystem.Instance.InRoom && !base.IsMine && !this.pendingOwnershipRequest && this.syncedState.riderId == -1)
		{
			this.ownershipGuard.RequestOwnershipImmediately(delegate
			{
				this.pendingOwnershipRequest = false;
			});
			this.pendingOwnershipRequest = true;
			if (this.reenableOwnershipRequestCoroutine != null)
			{
				base.StopCoroutine(this.reenableOwnershipRequestCoroutine);
			}
			this.reenableOwnershipRequestCoroutine = base.StartCoroutine(this.ReenableOwnershipRequest());
			this.OnGrabAuthority(pointGrabbed, grabbingHand);
		}
	}

	// Token: 0x06003F15 RID: 16149 RVA: 0x001405B8 File Offset: 0x0013E7B8
	public void OnGrabAuthority(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!base.IsMine && NetworkSystem.Instance.InRoom && !this.pendingOwnershipRequest)
		{
			return;
		}
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		if ((flag && !EquipmentInteractor.instance.isLeftGrabbing) || (!flag && !EquipmentInteractor.instance.isRightGrabbing))
		{
			return;
		}
		if (this.riderId != NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.riderId = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			this.cachedRig = this.getNewHolderRig(this.riderId);
		}
		EquipmentInteractor.instance.UpdateHandEquipment(this, flag);
		GorillaTagger.Instance.StartVibration(flag, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		Vector3 worldGrabPoint = this.ClosestPointInHandle(grabbingHand.transform.position, pointGrabbed);
		if (flag)
		{
			this.leftHold.Activate(grabbingHand.transform, base.transform, worldGrabPoint);
		}
		else
		{
			this.rightHold.Activate(grabbingHand.transform, base.transform, worldGrabPoint);
		}
		if (this.leftHold.active && this.rightHold.active)
		{
			Vector3 handsVector = this.GetHandsVector(this.leftHold.transform.position, this.rightHold.transform.position, GTPlayer.Instance.headCollider.transform.position, true);
			this.twoHandRotationOffsetAxis = Vector3.Cross(handsVector, base.transform.right).normalized;
			if ((double)this.twoHandRotationOffsetAxis.sqrMagnitude < 0.001)
			{
				this.twoHandRotationOffsetAxis = base.transform.right;
				this.twoHandRotationOffsetAngle = 0f;
			}
			else
			{
				this.twoHandRotationOffsetAngle = Vector3.SignedAngle(handsVector, base.transform.right, this.twoHandRotationOffsetAxis);
			}
		}
		this.rb.isKinematic = true;
		this.rb.useGravity = false;
		this.ridersMaterialOverideIndex = 0;
		if (this.cosmeticMaterialOverrides.Length != 0)
		{
			VRRig offlineVRRig = this.cachedRig;
			if (offlineVRRig == null)
			{
				offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			}
			if (offlineVRRig != null)
			{
				for (int i = 0; i < this.cosmeticMaterialOverrides.Length; i++)
				{
					if (this.cosmeticMaterialOverrides[i].cosmeticName != null && offlineVRRig.cosmeticSet != null && offlineVRRig.cosmeticSet.HasItem(this.cosmeticMaterialOverrides[i].cosmeticName))
					{
						this.ridersMaterialOverideIndex = i + 1;
						break;
					}
				}
			}
		}
		this.infectedState = false;
		if (GorillaGameManager.instance as GorillaTagManager != null)
		{
			this.infectedState = this.syncedState.tagged;
		}
		if (this.infectedState)
		{
			this.leafMesh.material = this.GetInfectedMaterial();
		}
		else
		{
			this.leafMesh.material = this.GetMaterialFromIndex((byte)this.ridersMaterialOverideIndex);
		}
		if (EquipmentInteractor.instance.rightHandHeldEquipment != null && EquipmentInteractor.instance.rightHandHeldEquipment.GetType() == typeof(GliderHoldable) && EquipmentInteractor.instance.leftHandHeldEquipment != null && EquipmentInteractor.instance.leftHandHeldEquipment.GetType() == typeof(GliderHoldable) && EquipmentInteractor.instance.leftHandHeldEquipment != EquipmentInteractor.instance.rightHandHeldEquipment)
		{
			this.holdingTwoGliders = true;
		}
	}

	// Token: 0x06003F16 RID: 16150 RVA: 0x00140944 File Offset: 0x0013EB44
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		this.holdingTwoGliders = false;
		bool flag = releasingHand == EquipmentInteractor.instance.leftHand;
		if (this.leftHold.active && this.rightHold.active)
		{
			if (flag)
			{
				this.rightHold.Activate(this.rightHold.transform, base.transform, this.ClosestPointInHandle(this.rightHold.transform.position, this.handle));
			}
			else
			{
				this.leftHold.Activate(this.leftHold.transform, base.transform, this.ClosestPointInHandle(this.leftHold.transform.position, this.handle));
			}
		}
		Vector3 velocity = Vector3.zero;
		if (flag)
		{
			this.leftHold.Deactivate();
			velocity = GTPlayer.Instance.leftHandCenterVelocityTracker.GetAverageVelocity(true, 0.15f, false);
			EquipmentInteractor.instance.UpdateHandEquipment(null, true);
		}
		else
		{
			this.rightHold.Deactivate();
			velocity = GTPlayer.Instance.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0.15f, false);
			EquipmentInteractor.instance.UpdateHandEquipment(null, false);
		}
		if (!this.leftHold.active && !this.rightHold.active)
		{
			this.gliderState = GliderHoldable.GliderState.LocallyDropped;
			this.audioLevel = 0f;
			this.riderId = -1;
			this.cachedRig = null;
			this.subtlePlayerPitch = 0f;
			this.subtlePlayerRoll = 0f;
			this.leftHoldPositionLocal = null;
			this.rightHoldPositionLocal = null;
			this.ridersMaterialOverideIndex = 0;
			if (base.IsMine || !NetworkSystem.Instance.InRoom)
			{
				this.rb.isKinematic = false;
				this.rb.useGravity = true;
				this.rb.velocity = velocity;
				this.syncedState.riderId = -1;
				this.syncedState.tagged = false;
				this.syncedState.materialIndex = 0;
				this.syncedState.position = base.transform.position;
				this.syncedState.rotation = base.transform.rotation;
				this.syncedState.audioLevel = 0;
			}
			this.leafMesh.material = this.baseLeafMaterial;
		}
		return true;
	}

	// Token: 0x06003F17 RID: 16151 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void DropItemCleanup()
	{
	}

	// Token: 0x06003F18 RID: 16152 RVA: 0x00140B8C File Offset: 0x0013ED8C
	public void FixedUpdate()
	{
		if (!base.IsMine && NetworkSystem.Instance.InRoom && !this.pendingOwnershipRequest)
		{
			return;
		}
		GTPlayer instance = GTPlayer.Instance;
		if (this.holdingTwoGliders)
		{
			instance.AddForce(Physics.gravity, ForceMode.Acceleration);
			return;
		}
		if (this.leftHold.active || this.rightHold.active)
		{
			float fixedDeltaTime = Time.fixedDeltaTime;
			this.previousVelocity = this.currentVelocity;
			this.currentVelocity = instance.RigidbodyVelocity;
			float magnitude = this.currentVelocity.magnitude;
			this.accelerationAverage.AddSample((this.currentVelocity - this.previousVelocity) / Time.fixedDeltaTime, Time.fixedTime);
			float rollAngle180Wrapping = this.GetRollAngle180Wrapping();
			float angle = this.liftIncreaseVsRoll.Evaluate(Mathf.Clamp01(Mathf.Abs(rollAngle180Wrapping / 180f))) * this.liftIncreaseVsRollMaxAngle;
			Vector3 vector = Vector3.RotateTowards(this.currentVelocity, Quaternion.AngleAxis(angle, -base.transform.right) * base.transform.forward * magnitude, this.pitchVelocityFollowRateAngle * 0.017453292f * fixedDeltaTime, this.pitchVelocityFollowRateMagnitude * fixedDeltaTime);
			Vector3 a = vector - this.currentVelocity;
			float num = this.NormalizeAngle180(Vector3.SignedAngle(Vector3.ProjectOnPlane(this.currentVelocity, base.transform.right), base.transform.forward, base.transform.right));
			if (num > 90f)
			{
				num = Mathf.Lerp(0f, 90f, Mathf.InverseLerp(180f, 90f, num));
			}
			else if (num < -90f)
			{
				num = Mathf.Lerp(0f, -90f, Mathf.InverseLerp(-180f, -90f, num));
			}
			float time = Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(-90f, 90f, num));
			Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(-90f, 90f, this.pitch));
			float d = this.liftVsAttack.Evaluate(time);
			instance.AddForce(a * d, ForceMode.VelocityChange);
			float num2 = this.dragVsAttack.Evaluate(time);
			float num3 = (this.syncedState.riderId != -1 && this.syncedState.materialIndex == 1) ? (this.dragVsSpeedMaxSpeed + this.infectedSpeedIncrease) : this.dragVsSpeedMaxSpeed;
			float num4 = this.dragVsSpeed.Evaluate(Mathf.Clamp01(magnitude / num3));
			float d2 = Mathf.Clamp01(num2 * this.attackDragFactor + num4 * this.dragVsSpeedDragFactor);
			instance.AddForce(-this.currentVelocity * d2, ForceMode.Acceleration);
			if (this.pitch > 0f && this.currentVelocity.y > 0f && (this.currentVelocity - this.previousVelocity).y > 0f)
			{
				float a2 = Mathf.InverseLerp(0f, this.pullUpLiftActivationVelocity, this.currentVelocity.y);
				float b = Mathf.InverseLerp(0f, this.pullUpLiftActivationAcceleration, (this.currentVelocity - this.previousVelocity).y / fixedDeltaTime);
				float d3 = Mathf.Min(a2, b);
				instance.AddForce(-Physics.gravity * this.pullUpLiftBonus * d3, ForceMode.Acceleration);
			}
			if (Vector3.Dot(vector, Physics.gravity) > 0f)
			{
				instance.AddForce(-Physics.gravity * this.gravityCompensation, ForceMode.Acceleration);
				return;
			}
		}
		else
		{
			Vector3 a3 = this.WindResistanceForceOffset(base.transform.up, Vector3.down);
			Vector3 position = base.transform.position - a3 * this.gravityUprightTorqueMultiplier;
			this.rb.AddForceAtPosition(-this.fallingGravityReduction * Physics.gravity, position, ForceMode.Acceleration);
		}
	}

	// Token: 0x06003F19 RID: 16153 RVA: 0x00140F80 File Offset: 0x0013F180
	public void LateUpdate()
	{
		float deltaTime = Time.deltaTime;
		if (base.IsMine || !NetworkSystem.Instance.InRoom || this.pendingOwnershipRequest)
		{
			this.AuthorityUpdate(deltaTime);
			return;
		}
		this.RemoteSyncUpdate(deltaTime);
	}

	// Token: 0x06003F1A RID: 16154 RVA: 0x00140FC0 File Offset: 0x0013F1C0
	private void AuthorityUpdate(float dt)
	{
		if (!this.leftHold.active && !this.rightHold.active)
		{
			this.AuthorityUpdateUnheld(dt);
		}
		else if (this.leftHold.active || this.rightHold.active)
		{
			this.AuthorityUpdateHeld(dt);
		}
		this.syncedState.audioLevel = (byte)Mathf.FloorToInt(255f * this.audioLevel);
	}

	// Token: 0x06003F1B RID: 16155 RVA: 0x00141030 File Offset: 0x0013F230
	private void AuthorityUpdateHeld(float dt)
	{
		if (this.gliderState != GliderHoldable.GliderState.LocallyHeld)
		{
			this.gliderState = GliderHoldable.GliderState.LocallyHeld;
		}
		this.rb.isKinematic = true;
		this.lastHeldTime = Time.time;
		if (this.leftHold.active)
		{
			this.leftHold.holdLocalPos = Vector3.Lerp(Vector3.zero, this.leftHold.holdLocalPos, Mathf.Exp(-5f * dt));
		}
		if (this.rightHold.active)
		{
			this.rightHold.holdLocalPos = Vector3.Lerp(Vector3.zero, this.rightHold.holdLocalPos, Mathf.Exp(-5f * dt));
		}
		Vector3 a = Vector3.zero;
		if (this.leftHold.active && this.rightHold.active)
		{
			a = (this.leftHold.transform.TransformPoint(this.leftHold.holdLocalPos) + this.rightHold.transform.TransformPoint(this.rightHold.holdLocalPos)) * 0.5f;
		}
		else if (this.leftHold.active)
		{
			a = this.leftHold.transform.TransformPoint(this.leftHold.holdLocalPos);
		}
		else if (this.rightHold.active)
		{
			a = this.rightHold.transform.TransformPoint(this.rightHold.holdLocalPos);
		}
		this.UpdateGliderPosition();
		float magnitude = this.currentVelocity.magnitude;
		if (this.setMaxHandSlipDuringFlight && magnitude > this.maxSlipOverrideSpeedThreshold)
		{
			if (this.leftHold.active)
			{
				GTPlayer.Instance.SetLeftMaximumSlipThisFrame();
			}
			if (this.rightHold.active)
			{
				GTPlayer.Instance.SetRightMaximumSlipThisFrame();
			}
		}
		bool flag = false;
		GorillaTagManager gorillaTagManager = GorillaGameManager.instance as GorillaTagManager;
		if (gorillaTagManager != null)
		{
			flag = gorillaTagManager.IsInfected(NetworkSystem.Instance.LocalPlayer);
		}
		bool flag2 = flag != this.infectedState;
		this.infectedState = flag;
		if (flag2)
		{
			if (this.infectedState)
			{
				this.leafMesh.material = this.GetInfectedMaterial();
			}
			else
			{
				this.leafMesh.material = this.GetMaterialFromIndex(this.syncedState.materialIndex);
			}
		}
		Vector3 average = this.accelerationAverage.GetAverage();
		this.accelerationSmoothed = Mathf.Lerp(average.magnitude, this.accelerationSmoothed, Mathf.Exp(-this.accelSmoothingFollowRateExp * dt));
		float num = Mathf.InverseLerp(this.hapticMaxSpeedInputRange.x, this.hapticMaxSpeedInputRange.y, magnitude);
		float num2 = Mathf.InverseLerp(this.hapticAccelInputRange.x, this.hapticAccelInputRange.y, this.accelerationSmoothed);
		float num3 = Mathf.InverseLerp(this.hapticSpeedInputRange.x, this.hapticSpeedInputRange.y, magnitude);
		this.UpdateAudioSource(this.calmAudio, num * this.audioVolumeMultiplier);
		this.UpdateAudioSource(this.activeAudio, num2 * num * this.audioVolumeMultiplier);
		if (this.infectedState)
		{
			this.UpdateAudioSource(this.whistlingAudio, Mathf.InverseLerp(this.whistlingAudioSpeedInputRange.x, this.whistlingAudioSpeedInputRange.y, magnitude) * num2 * num * this.audioVolumeMultiplier);
		}
		else
		{
			this.UpdateAudioSource(this.whistlingAudio, 0f);
		}
		float amplitude = Mathf.Max(num2 * this.hapticAccelOutputMax * num, num3 * this.hapticSpeedOutputMax);
		if (this.rightHold.active)
		{
			GorillaTagger.Instance.DoVibration(XRNode.RightHand, amplitude, dt);
		}
		if (this.leftHold.active)
		{
			GorillaTagger.Instance.DoVibration(XRNode.LeftHand, amplitude, dt);
		}
		Vector3 origin = this.handle.transform.position + this.handle.transform.rotation * new Vector3(0f, 0f, 1f);
		if (Time.frameCount % 2 == 0)
		{
			Vector3 direction = this.handle.transform.rotation * new Vector3(-0.707f, 0f, 0.707f);
			RaycastHit raycastHit;
			if (this.leftWhooshStartTime < Time.time - this.whooshSoundRetriggerThreshold && magnitude > this.whooshSpeedThresholdInput.x && Physics.Raycast(new Ray(origin, direction), out raycastHit, this.whooshCheckDistance, GTPlayer.Instance.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore))
			{
				this.leftWhooshStartTime = Time.time;
				this.leftWhooshHitPoint = raycastHit.point;
				this.leftWhooshAudio.GTStop();
				this.leftWhooshAudio.volume = Mathf.Lerp(this.whooshVolumeOutput.x, this.whooshVolumeOutput.y, Mathf.InverseLerp(this.whooshSpeedThresholdInput.x, this.whooshSpeedThresholdInput.y, magnitude));
				this.leftWhooshAudio.GTPlay();
			}
		}
		else
		{
			Vector3 direction2 = this.handle.transform.rotation * new Vector3(0.707f, 0f, 0.707f);
			RaycastHit raycastHit2;
			if (this.rightWhooshStartTime < Time.time - this.whooshSoundRetriggerThreshold && magnitude > this.whooshSpeedThresholdInput.x && Physics.Raycast(new Ray(origin, direction2), out raycastHit2, this.whooshCheckDistance, GTPlayer.Instance.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore))
			{
				this.rightWhooshStartTime = Time.time;
				this.rightWhooshHitPoint = raycastHit2.point;
				this.rightWhooshAudio.GTStop();
				this.rightWhooshAudio.volume = Mathf.Lerp(this.whooshVolumeOutput.x, this.whooshVolumeOutput.y, Mathf.InverseLerp(this.whooshSpeedThresholdInput.x, this.whooshSpeedThresholdInput.y, magnitude));
				this.rightWhooshAudio.GTPlay();
			}
		}
		Vector3 headCenterPosition = GTPlayer.Instance.HeadCenterPosition;
		if (this.leftWhooshStartTime > Time.time - this.whooshSoundDuration)
		{
			this.leftWhooshAudio.transform.position = this.leftWhooshHitPoint;
		}
		else
		{
			this.leftWhooshAudio.transform.localPosition = new Vector3(-this.whooshAudioPositionOffset.x, this.whooshAudioPositionOffset.y, this.whooshAudioPositionOffset.z);
		}
		if (this.rightWhooshStartTime > Time.time - this.whooshSoundDuration)
		{
			this.rightWhooshAudio.transform.position = this.rightWhooshHitPoint;
		}
		else
		{
			this.rightWhooshAudio.transform.localPosition = new Vector3(this.whooshAudioPositionOffset.x, this.whooshAudioPositionOffset.y, this.whooshAudioPositionOffset.z);
		}
		if (this.extendTagRangeInFlight)
		{
			float tagRadiusOverrideThisFrame = Mathf.Lerp(this.tagRangeOutput.x, this.tagRangeOutput.y, Mathf.InverseLerp(this.tagRangeSpeedInput.x, this.tagRangeSpeedInput.y, magnitude));
			GorillaTagger.Instance.SetTagRadiusOverrideThisFrame(tagRadiusOverrideThisFrame);
			if (this.debugDrawTagRange)
			{
				GorillaTagger.Instance.DebugDrawTagCasts(Color.yellow);
			}
		}
		Vector3 normalized = Vector3.ProjectOnPlane(base.transform.right, Vector3.up).normalized;
		Vector3 normalized2 = Vector3.ProjectOnPlane(base.transform.forward, Vector3.up).normalized;
		float num4 = -Vector3.Dot(a - this.handle.transform.position, normalized2);
		Vector3 b = this.handle.transform.position - normalized2 * (this.riderPosRange.y * 0.5f + this.riderPosRangeOffset + num4);
		float num5 = Vector3.Dot(headCenterPosition - b, normalized);
		float num6 = Vector3.Dot(headCenterPosition - b, normalized2);
		num5 /= this.riderPosRange.x * 0.5f;
		num6 /= this.riderPosRange.y * 0.5f;
		this.riderPosition.x = Mathf.Sign(num5) * Mathf.Lerp(0f, 1f, Mathf.InverseLerp(this.riderPosRangeNormalizedDeadzone.x, 1f, Mathf.Abs(num5)));
		this.riderPosition.y = Mathf.Sign(num6) * Mathf.Lerp(0f, 1f, Mathf.InverseLerp(this.riderPosRangeNormalizedDeadzone.y, 1f, Mathf.Abs(num6)));
		Vector3 vector;
		Vector3 vector2;
		if (this.leftHold.active && this.rightHold.active)
		{
			vector = this.leftHold.transform.position;
			this.leftHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector));
			vector2 = this.rightHold.transform.position;
			this.rightHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector2));
		}
		else if (this.leftHold.active)
		{
			vector = this.leftHold.transform.position;
			this.leftHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector));
			Vector3 vector3 = vector + this.leftHold.transform.up * this.oneHandSimulatedHoldOffset.x;
			if (this.rightHoldPositionLocal != null)
			{
				this.rightHoldPositionLocal = new Vector3?(Vector3.Lerp(GTPlayer.Instance.transform.InverseTransformPoint(vector3), this.rightHoldPositionLocal.Value, Mathf.Exp(-5f * dt)));
				vector2 = GTPlayer.Instance.transform.TransformPoint(this.rightHoldPositionLocal.Value);
			}
			else
			{
				vector2 = vector3;
				this.rightHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector2));
			}
		}
		else
		{
			vector2 = this.rightHold.transform.position;
			this.rightHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector2));
			Vector3 vector4 = vector2 + this.rightHold.transform.up * this.oneHandSimulatedHoldOffset.x;
			if (this.leftHoldPositionLocal != null)
			{
				this.leftHoldPositionLocal = new Vector3?(Vector3.Lerp(GTPlayer.Instance.transform.InverseTransformPoint(vector4), this.leftHoldPositionLocal.Value, Mathf.Exp(-5f * dt)));
				vector = GTPlayer.Instance.transform.TransformPoint(this.leftHoldPositionLocal.Value);
			}
			else
			{
				vector = vector4;
				this.leftHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector));
			}
		}
		Vector3 forward;
		Vector3 vector5;
		this.GetHandsOrientationVectors(vector, vector2, GTPlayer.Instance.headCollider.transform, false, out forward, out vector5);
		float num7 = this.riderPosition.y * this.riderPosDirectPitchMax;
		if (!this.leftHold.active || !this.rightHold.active)
		{
			num7 *= this.oneHandPitchMultiplier;
		}
		Spring.CriticalSpringDamperExact(ref this.pitch, ref this.pitchVel, num7, 0f, this.pitchHalfLife, dt);
		this.pitch = Mathf.Clamp(this.pitch, this.pitchMinMax.x, this.pitchMinMax.y);
		Quaternion rhs = Quaternion.AngleAxis(this.pitch, Vector3.right);
		this.twoHandRotationOffsetAngle = Mathf.Lerp(0f, this.twoHandRotationOffsetAngle, Mathf.Exp(-8f * dt));
		Vector3 upwards = this.twoHandGliderInversionOnYawInsteadOfRoll ? vector5 : Vector3.up;
		Quaternion lhs = Quaternion.AngleAxis(this.twoHandRotationOffsetAngle, this.twoHandRotationOffsetAxis) * Quaternion.LookRotation(forward, upwards) * Quaternion.AngleAxis(-90f, Vector3.up);
		float num8 = (this.leftHold.active && this.rightHold.active) ? this.twoHandRotationRateExp : this.oneHandRotationRateExp;
		base.transform.rotation = Quaternion.Slerp(lhs * rhs, base.transform.rotation, Mathf.Exp(-num8 * dt));
		if (this.subtlePlayerPitchActive || this.subtlePlayerRollActive)
		{
			float a2 = Mathf.InverseLerp(this.subtlePlayerRotationSpeedRampMinMax.x, this.subtlePlayerRotationSpeedRampMinMax.y, this.currentVelocity.magnitude);
			Quaternion rhs2 = Quaternion.identity;
			if (this.subtlePlayerRollActive)
			{
				float num9 = this.GetRollAngle180Wrapping();
				if (num9 > 90f)
				{
					num9 = Mathf.Lerp(0f, 90f, Mathf.InverseLerp(180f, 90f, num9));
				}
				else if (num9 < -90f)
				{
					num9 = Mathf.Lerp(0f, -90f, Mathf.InverseLerp(-180f, -90f, num9));
				}
				Vector3 normalized3 = new Vector3(this.currentVelocity.x, 0f, this.currentVelocity.z).normalized;
				Vector3 vector6 = new Vector3(average.x, 0f, average.z);
				float num10 = Vector3.Dot(vector6 - Vector3.Dot(vector6, normalized3) * normalized3, Vector3.Cross(normalized3, Vector3.up));
				this.turnAccelerationSmoothed = Mathf.Lerp(num10, this.turnAccelerationSmoothed, Mathf.Exp(-this.accelSmoothingFollowRateExp * dt));
				float b2 = 0f;
				if (num10 * num9 > 0f)
				{
					b2 = Mathf.InverseLerp(this.subtlePlayerRollAccelMinMax.x, this.subtlePlayerRollAccelMinMax.y, Mathf.Abs(this.turnAccelerationSmoothed));
				}
				float a3 = num9 * this.subtlePlayerRollFactor * Mathf.Min(a2, b2);
				this.subtlePlayerRoll = Mathf.Lerp(a3, this.subtlePlayerRoll, Mathf.Exp(-this.subtlePlayerRollRateExp * dt));
				rhs2 = Quaternion.AngleAxis(this.subtlePlayerRoll, base.transform.forward);
			}
			Quaternion lhs2 = Quaternion.identity;
			if (this.subtlePlayerPitchActive)
			{
				float a4 = this.pitch * this.subtlePlayerPitchFactor * Mathf.Min(a2, 1f);
				this.subtlePlayerPitch = Mathf.Lerp(a4, this.subtlePlayerPitch, Mathf.Exp(-this.subtlePlayerPitchRateExp * dt));
				lhs2 = Quaternion.AngleAxis(this.subtlePlayerPitch, -base.transform.right);
			}
			GTPlayer.Instance.PlayerRotationOverride = lhs2 * rhs2;
		}
		this.UpdateGliderPosition();
		if (this.syncedState.riderId != NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.riderId = (this.syncedState.riderId = NetworkSystem.Instance.LocalPlayer.ActorNumber);
			this.cachedRig = this.getNewHolderRig(this.riderId);
		}
		this.syncedState.tagged = this.infectedState;
		this.syncedState.materialIndex = (byte)this.ridersMaterialOverideIndex;
		if (this.cachedRig != null)
		{
			this.syncedState.position = this.cachedRig.transform.InverseTransformPoint(base.transform.position);
			this.syncedState.rotation = Quaternion.Inverse(this.cachedRig.transform.rotation) * base.transform.rotation;
		}
		else
		{
			Debug.LogError("Glider failed to get a reference to the local player's VRRig while the player was flying", this);
		}
		this.audioLevel = num2 * num;
		if (this.OutOfBounds)
		{
			this.Respawn();
		}
		if (this.leftHold.active && EquipmentInteractor.instance.leftHandHeldEquipment != this)
		{
			this.OnRelease(null, EquipmentInteractor.instance.leftHand);
		}
		if (this.rightHold.active && EquipmentInteractor.instance.rightHandHeldEquipment != this)
		{
			this.OnRelease(null, EquipmentInteractor.instance.rightHand);
		}
	}

	// Token: 0x06003F1C RID: 16156 RVA: 0x00141FD8 File Offset: 0x001401D8
	private void AuthorityUpdateUnheld(float dt)
	{
		this.syncedState.position = base.transform.position;
		this.syncedState.rotation = base.transform.rotation;
		if (this.gliderState != GliderHoldable.GliderState.LocallyDropped)
		{
			this.gliderState = GliderHoldable.GliderState.LocallyDropped;
			this.syncedState.riderId = -1;
			this.syncedState.materialIndex = 0;
			this.syncedState.tagged = false;
			this.leafMesh.material = this.baseLeafMaterial;
		}
		if (this.audioLevel * this.audioVolumeMultiplier > 0.001f)
		{
			this.audioLevel = Mathf.Lerp(0f, this.audioLevel, Mathf.Exp(-2f * dt));
			this.UpdateAudioSource(this.calmAudio, this.audioLevel * this.audioVolumeMultiplier);
			this.UpdateAudioSource(this.activeAudio, this.audioLevel * this.audioVolumeMultiplier);
			this.UpdateAudioSource(this.whistlingAudio, this.audioLevel * this.audioVolumeMultiplier);
		}
		if (this.OutOfBounds || (this.lastHeldTime > 0f && this.lastHeldTime < Time.time - this.maxDroppedTimeToRespawn))
		{
			this.Respawn();
		}
	}

	// Token: 0x06003F1D RID: 16157 RVA: 0x0014210C File Offset: 0x0014030C
	private void RemoteSyncUpdate(float dt)
	{
		this.rb.isKinematic = true;
		int num = this.syncedState.riderId;
		bool flag = this.riderId != num;
		if (flag)
		{
			this.riderId = num;
			this.cachedRig = this.getNewHolderRig(this.riderId);
		}
		if (this.riderId == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.cachedRig = null;
			this.syncedState.riderId = -1;
			this.syncedState.materialIndex = 0;
			this.syncedState.audioLevel = 0;
		}
		if (this.syncedState.riderId == -1)
		{
			base.transform.position = Vector3.Lerp(this.syncedState.position, base.transform.position, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
			base.transform.rotation = Quaternion.Slerp(this.syncedState.rotation, base.transform.rotation, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
		}
		else if (this.cachedRig != null)
		{
			this.positionLocalToVRRig = Vector3.Lerp(this.syncedState.position, this.positionLocalToVRRig, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
			this.rotationLocalToVRRig = Quaternion.Slerp(this.syncedState.rotation, this.rotationLocalToVRRig, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
			base.transform.position = this.cachedRig.transform.TransformPoint(this.positionLocalToVRRig);
			base.transform.rotation = this.cachedRig.transform.rotation * this.rotationLocalToVRRig;
		}
		bool flag2 = false;
		if (GorillaGameManager.instance as GorillaTagManager != null)
		{
			flag2 = this.syncedState.tagged;
		}
		bool flag3 = flag2 != this.infectedState;
		this.infectedState = flag2;
		if (flag3 || flag)
		{
			if (this.infectedState)
			{
				this.leafMesh.material = this.GetInfectedMaterial();
			}
			else
			{
				this.leafMesh.material = this.GetMaterialFromIndex(this.syncedState.materialIndex);
			}
		}
		float num2 = Mathf.Clamp01((float)this.syncedState.audioLevel / 255f);
		if (this.audioLevel != num2)
		{
			this.audioLevel = num2;
			if (this.syncedState.riderId != -1 && this.syncedState.tagged)
			{
				this.UpdateAudioSource(this.calmAudio, this.audioLevel * this.infectedAudioVolumeMultiplier);
				this.UpdateAudioSource(this.activeAudio, this.audioLevel * this.infectedAudioVolumeMultiplier);
				this.UpdateAudioSource(this.whistlingAudio, this.audioLevel * this.infectedAudioVolumeMultiplier);
				return;
			}
			this.UpdateAudioSource(this.calmAudio, this.audioLevel * this.audioVolumeMultiplier);
			this.UpdateAudioSource(this.activeAudio, this.audioLevel * this.audioVolumeMultiplier);
			this.UpdateAudioSource(this.whistlingAudio, 0f);
		}
	}

	// Token: 0x06003F1E RID: 16158 RVA: 0x00142410 File Offset: 0x00140610
	private VRRig getNewHolderRig(int riderId)
	{
		if (riderId >= 0)
		{
			NetPlayer netPlayer;
			if (riderId == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				netPlayer = NetworkSystem.Instance.LocalPlayer;
			}
			else
			{
				netPlayer = NetworkSystem.Instance.GetPlayer(riderId);
			}
			RigContainer rigContainer;
			if (netPlayer != null && VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer))
			{
				return rigContainer.Rig;
			}
		}
		return null;
	}

	// Token: 0x06003F1F RID: 16159 RVA: 0x00142468 File Offset: 0x00140668
	private Vector3 ClosestPointInHandle(Vector3 startingPoint, InteractionPoint interactionPoint)
	{
		CapsuleCollider component = interactionPoint.GetComponent<CapsuleCollider>();
		Vector3 vector = startingPoint;
		if (component != null)
		{
			Vector3 point = (component.direction == 0) ? Vector3.right : ((component.direction == 1) ? Vector3.up : Vector3.forward);
			Vector3 vector2 = component.transform.rotation * point;
			Vector3 vector3 = component.transform.position + component.transform.rotation * component.center;
			float d = Mathf.Clamp(Vector3.Dot(vector - vector3, vector2), -component.height * 0.5f, component.height * 0.5f);
			vector = vector3 + vector2 * d;
		}
		return vector;
	}

	// Token: 0x06003F20 RID: 16160 RVA: 0x00142528 File Offset: 0x00140728
	private void UpdateGliderPosition()
	{
		if (this.leftHold.active && this.rightHold.active)
		{
			Vector3 a = this.leftHold.transform.TransformPoint(this.leftHold.holdLocalPos) + base.transform.TransformVector(this.leftHold.handleLocalPos);
			Vector3 b = this.rightHold.transform.TransformPoint(this.rightHold.holdLocalPos) + base.transform.TransformVector(this.rightHold.handleLocalPos);
			base.transform.position = (a + b) * 0.5f;
			return;
		}
		if (this.leftHold.active)
		{
			base.transform.position = this.leftHold.transform.TransformPoint(this.leftHold.holdLocalPos) + base.transform.TransformVector(this.leftHold.handleLocalPos);
			return;
		}
		if (this.rightHold.active)
		{
			base.transform.position = this.rightHold.transform.TransformPoint(this.rightHold.holdLocalPos) + base.transform.TransformVector(this.rightHold.handleLocalPos);
		}
	}

	// Token: 0x06003F21 RID: 16161 RVA: 0x00142680 File Offset: 0x00140880
	private Vector3 GetHandsVector(Vector3 leftHandPos, Vector3 rightHandPos, Vector3 headPos, bool flipBasedOnFacingDir)
	{
		Vector3 vector = rightHandPos - leftHandPos;
		Vector3 rhs = (rightHandPos + leftHandPos) * 0.5f - headPos;
		Vector3 normalized = Vector3.Cross(Vector3.up, rhs).normalized;
		if (flipBasedOnFacingDir && Vector3.Dot(vector, normalized) < 0f)
		{
			vector = -vector;
		}
		return vector;
	}

	// Token: 0x06003F22 RID: 16162 RVA: 0x001426DC File Offset: 0x001408DC
	private void GetHandsOrientationVectors(Vector3 leftHandPos, Vector3 rightHandPos, Transform head, bool flipBasedOnFacingDir, out Vector3 handsVector, out Vector3 handsUpVector)
	{
		handsVector = rightHandPos - leftHandPos;
		float magnitude = handsVector.magnitude;
		handsVector /= Mathf.Max(magnitude, 0.001f);
		Vector3 position = head.position;
		float d = 1f;
		Vector3 planeNormal = (Vector3.Dot(head.right, handsVector) < 0f) ? handsVector : (-handsVector);
		Vector3 normalized = Vector3.ProjectOnPlane(-head.forward, planeNormal).normalized;
		Vector3 a = normalized * d + position;
		Vector3 a2 = (leftHandPos + rightHandPos) * 0.5f;
		Vector3 a3 = Vector3.ProjectOnPlane(a2 - head.position, Vector3.up);
		float magnitude2 = a3.magnitude;
		a3 /= Mathf.Max(magnitude2, 0.001f);
		Vector3 normalized2 = Vector3.ProjectOnPlane(-base.transform.forward, Vector3.up).normalized;
		Vector3 a4 = -a3 * d + position;
		float num = Vector3.Dot(normalized2, -a3);
		float num2 = Vector3.Dot(normalized2, normalized);
		if (Vector3.Dot(base.transform.up, Vector3.up) < 0f)
		{
			num = Mathf.Abs(num);
			num2 = Mathf.Abs(num2);
		}
		num = Mathf.Max(num, 0f);
		num2 = Mathf.Max(num2, 0f);
		Vector3 b = (a4 * num + a * num2) / Mathf.Max(num + num2, 0.001f);
		Vector3 vector = a2 - b;
		Vector3 normalized3 = Vector3.Cross(Vector3.up, vector).normalized;
		if (flipBasedOnFacingDir && Vector3.Dot(handsVector, normalized3) < 0f)
		{
			handsVector = -handsVector;
		}
		handsUpVector = Vector3.Cross(Vector3.ProjectOnPlane(vector, Vector3.up), handsVector).normalized;
	}

	// Token: 0x06003F23 RID: 16163 RVA: 0x00142907 File Offset: 0x00140B07
	private Material GetMaterialFromIndex(byte materialIndex)
	{
		if (materialIndex < 1 || (int)materialIndex > this.cosmeticMaterialOverrides.Length)
		{
			return this.baseLeafMaterial;
		}
		return this.cosmeticMaterialOverrides[(int)(materialIndex - 1)].material;
	}

	// Token: 0x06003F24 RID: 16164 RVA: 0x00142934 File Offset: 0x00140B34
	private float GetRollAngle180Wrapping()
	{
		Vector3 normalized = Vector3.ProjectOnPlane(base.transform.forward, Vector3.up).normalized;
		float angle = Vector3.SignedAngle(Vector3.Cross(Vector3.up, normalized).normalized, base.transform.right, base.transform.forward);
		return this.NormalizeAngle180(angle);
	}

	// Token: 0x06003F25 RID: 16165 RVA: 0x00142995 File Offset: 0x00140B95
	private float SignedAngleInPlane(Vector3 from, Vector3 to, Vector3 normal)
	{
		from = Vector3.ProjectOnPlane(from, normal);
		to = Vector3.ProjectOnPlane(to, normal);
		return Vector3.SignedAngle(from, to, normal);
	}

	// Token: 0x06003F26 RID: 16166 RVA: 0x001429B1 File Offset: 0x00140BB1
	private float NormalizeAngle180(float angle)
	{
		angle = (angle + 180f) % 360f;
		if (angle < 0f)
		{
			angle += 360f;
		}
		return angle - 180f;
	}

	// Token: 0x06003F27 RID: 16167 RVA: 0x001429DC File Offset: 0x00140BDC
	private void UpdateAudioSource(AudioSource source, float level)
	{
		source.volume = level;
		if (!source.isPlaying && level > 0.01f)
		{
			source.GTPlay();
			return;
		}
		if (source.isPlaying && level < 0.01f && this.syncedState.riderId == -1)
		{
			source.GTStop();
		}
	}

	// Token: 0x06003F28 RID: 16168 RVA: 0x00142A2B File Offset: 0x00140C2B
	private Material GetInfectedMaterial()
	{
		if (GorillaGameManager.instance is GorillaFreezeTagManager)
		{
			return this.frozenLeafMaterial;
		}
		return this.infectedLeafMaterial;
	}

	// Token: 0x06003F29 RID: 16169 RVA: 0x00142A48 File Offset: 0x00140C48
	public void OnTriggerStay(Collider other)
	{
		GliderWindVolume component = other.GetComponent<GliderWindVolume>();
		if (component == null)
		{
			return;
		}
		if (!base.IsMine && NetworkSystem.Instance.InRoom && !this.pendingOwnershipRequest)
		{
			return;
		}
		if (Time.frameCount == this.windVolumeForceAppliedFrame)
		{
			return;
		}
		if (this.leftHold.active || this.rightHold.active)
		{
			Vector3 accelFromVelocity = component.GetAccelFromVelocity(GTPlayer.Instance.RigidbodyVelocity);
			GTPlayer.Instance.AddForce(accelFromVelocity, ForceMode.Acceleration);
			this.windVolumeForceAppliedFrame = Time.frameCount;
			return;
		}
		Vector3 accelFromVelocity2 = component.GetAccelFromVelocity(this.rb.velocity);
		Vector3 a = this.WindResistanceForceOffset(base.transform.up, component.WindDirection);
		Vector3 position = base.transform.position + a * this.windUprightTorqueMultiplier;
		this.rb.AddForceAtPosition(accelFromVelocity2, position, ForceMode.Acceleration);
		this.windVolumeForceAppliedFrame = Time.frameCount;
	}

	// Token: 0x06003F2A RID: 16170 RVA: 0x00142B36 File Offset: 0x00140D36
	private Vector3 WindResistanceForceOffset(Vector3 upDir, Vector3 windDir)
	{
		if (Vector3.Dot(upDir, windDir) < 0f)
		{
			upDir *= -1f;
		}
		return Vector3.ProjectOnPlane(upDir - windDir, upDir);
	}

	// Token: 0x170005FC RID: 1532
	// (get) Token: 0x06003F2B RID: 16171 RVA: 0x00142B60 File Offset: 0x00140D60
	// (set) Token: 0x06003F2C RID: 16172 RVA: 0x00142B8A File Offset: 0x00140D8A
	[Networked]
	[NetworkedWeaved(0, 11)]
	internal unsafe GliderHoldable.SyncedState Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GliderHoldable.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(GliderHoldable.SyncedState*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GliderHoldable.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(GliderHoldable.SyncedState*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06003F2D RID: 16173 RVA: 0x00142BB8 File Offset: 0x00140DB8
	public override void ReadDataFusion()
	{
		int num = this.syncedState.riderId;
		this.syncedState = this.Data;
		if (num != this.syncedState.riderId)
		{
			this.positionLocalToVRRig = this.syncedState.position;
			this.rotationLocalToVRRig = this.syncedState.rotation;
		}
	}

	// Token: 0x06003F2E RID: 16174 RVA: 0x00142C0B File Offset: 0x00140E0B
	public override void WriteDataFusion()
	{
		this.Data = this.syncedState;
	}

	// Token: 0x06003F2F RID: 16175 RVA: 0x00142C1C File Offset: 0x00140E1C
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		Player sender = info.Sender;
		PunNetPlayer punNetPlayer = (PunNetPlayer)this.ownershipGuard.actualOwner;
		if (sender != ((punNetPlayer != null) ? punNetPlayer.PlayerRef : null))
		{
			return;
		}
		int num = this.syncedState.riderId;
		this.syncedState.riderId = (int)stream.ReceiveNext();
		this.syncedState.tagged = (bool)stream.ReceiveNext();
		this.syncedState.materialIndex = (byte)stream.ReceiveNext();
		this.syncedState.audioLevel = (byte)stream.ReceiveNext();
		Vector3 vector = (Vector3)stream.ReceiveNext();
		ref this.syncedState.position.SetValueSafe(vector);
		Quaternion quaternion = (Quaternion)stream.ReceiveNext();
		ref this.syncedState.rotation.SetValueSafe(quaternion);
		if (num != this.syncedState.riderId)
		{
			this.positionLocalToVRRig = this.syncedState.position;
			this.rotationLocalToVRRig = this.syncedState.rotation;
		}
	}

	// Token: 0x06003F30 RID: 16176 RVA: 0x00142D24 File Offset: 0x00140F24
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		object sender = info.Sender;
		NetPlayer actualOwner = this.ownershipGuard.actualOwner;
		if (!sender.Equals((actualOwner != null) ? actualOwner.GetPlayerRef() : null))
		{
			return;
		}
		stream.SendNext(this.syncedState.riderId);
		stream.SendNext(this.syncedState.tagged);
		stream.SendNext(this.syncedState.materialIndex);
		stream.SendNext(this.syncedState.audioLevel);
		stream.SendNext(this.syncedState.position);
		stream.SendNext(this.syncedState.rotation);
	}

	// Token: 0x06003F31 RID: 16177 RVA: 0x00142DDF File Offset: 0x00140FDF
	private IEnumerator ReenableOwnershipRequest()
	{
		yield return new WaitForSeconds(3f);
		this.pendingOwnershipRequest = false;
		yield break;
	}

	// Token: 0x06003F32 RID: 16178 RVA: 0x00142DF0 File Offset: 0x00140FF0
	public void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		if (toPlayer == NetworkSystem.Instance.LocalPlayer)
		{
			this.pendingOwnershipRequest = false;
			if (!this.leftHold.active && !this.rightHold.active && (this.spawnPosition - base.transform.position).sqrMagnitude > 1f)
			{
				this.rb.isKinematic = false;
				this.rb.WakeUp();
				this.lastHeldTime = Time.time;
			}
		}
	}

	// Token: 0x06003F33 RID: 16179 RVA: 0x00142E72 File Offset: 0x00141072
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		return !base.IsMine || !NetworkSystem.Instance.InRoom || (!this.leftHold.active && !this.rightHold.active);
	}

	// Token: 0x06003F34 RID: 16180 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnMyOwnerLeft()
	{
	}

	// Token: 0x06003F35 RID: 16181 RVA: 0x00002076 File Offset: 0x00000276
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		return false;
	}

	// Token: 0x06003F36 RID: 16182 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnMyCreatorLeft()
	{
	}

	// Token: 0x06003F3A RID: 16186 RVA: 0x0014334B File Offset: 0x0014154B
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06003F3B RID: 16187 RVA: 0x00143363 File Offset: 0x00141563
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04004B08 RID: 19208
	[Header("Flight Settings")]
	[SerializeField]
	private Vector2 pitchMinMax = new Vector2(-80f, 80f);

	// Token: 0x04004B09 RID: 19209
	[SerializeField]
	private Vector2 rollMinMax = new Vector2(-70f, 70f);

	// Token: 0x04004B0A RID: 19210
	[SerializeField]
	private float pitchHalfLife = 0.2f;

	// Token: 0x04004B0B RID: 19211
	public Vector2 pitchVelocityTargetMinMax = new Vector2(-60f, 60f);

	// Token: 0x04004B0C RID: 19212
	public Vector2 pitchVelocityRampTimeMinMax = new Vector2(-1f, 1f);

	// Token: 0x04004B0D RID: 19213
	[SerializeField]
	private float pitchVelocityFollowRateAngle = 60f;

	// Token: 0x04004B0E RID: 19214
	[SerializeField]
	private float pitchVelocityFollowRateMagnitude = 5f;

	// Token: 0x04004B0F RID: 19215
	[SerializeField]
	private AnimationCurve liftVsAttack = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04004B10 RID: 19216
	[SerializeField]
	private AnimationCurve dragVsAttack = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04004B11 RID: 19217
	[SerializeField]
	[Range(0f, 1f)]
	public float attackDragFactor = 0.1f;

	// Token: 0x04004B12 RID: 19218
	[SerializeField]
	private AnimationCurve dragVsSpeed = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04004B13 RID: 19219
	[SerializeField]
	public float dragVsSpeedMaxSpeed = 30f;

	// Token: 0x04004B14 RID: 19220
	[SerializeField]
	[Range(0f, 1f)]
	public float dragVsSpeedDragFactor = 0.2f;

	// Token: 0x04004B15 RID: 19221
	[SerializeField]
	private AnimationCurve liftIncreaseVsRoll = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04004B16 RID: 19222
	[SerializeField]
	private float liftIncreaseVsRollMaxAngle = 20f;

	// Token: 0x04004B17 RID: 19223
	[SerializeField]
	[Range(0f, 1f)]
	private float gravityCompensation = 0.8f;

	// Token: 0x04004B18 RID: 19224
	[Range(0f, 1f)]
	public float pullUpLiftBonus = 0.1f;

	// Token: 0x04004B19 RID: 19225
	public float pullUpLiftActivationVelocity = 1f;

	// Token: 0x04004B1A RID: 19226
	public float pullUpLiftActivationAcceleration = 3f;

	// Token: 0x04004B1B RID: 19227
	[Header("Body Positioning Control")]
	[SerializeField]
	private float riderPosDirectPitchMax = 70f;

	// Token: 0x04004B1C RID: 19228
	[SerializeField]
	private Vector2 riderPosRange = new Vector2(2.2f, 0.75f);

	// Token: 0x04004B1D RID: 19229
	[SerializeField]
	private float riderPosRangeOffset = 0.15f;

	// Token: 0x04004B1E RID: 19230
	[SerializeField]
	private Vector2 riderPosRangeNormalizedDeadzone = new Vector2(0.15f, 0.05f);

	// Token: 0x04004B1F RID: 19231
	[Header("Direct Handle Control")]
	[SerializeField]
	private float oneHandHoldRotationRate = 2f;

	// Token: 0x04004B20 RID: 19232
	private Vector3 oneHandSimulatedHoldOffset = new Vector3(0.5f, -0.35f, 0.25f);

	// Token: 0x04004B21 RID: 19233
	private float oneHandPitchMultiplier = 0.8f;

	// Token: 0x04004B22 RID: 19234
	[SerializeField]
	private float twoHandHoldRotationRate = 4f;

	// Token: 0x04004B23 RID: 19235
	[SerializeField]
	private bool twoHandGliderInversionOnYawInsteadOfRoll;

	// Token: 0x04004B24 RID: 19236
	[Header("Player Settings")]
	[SerializeField]
	private bool setMaxHandSlipDuringFlight = true;

	// Token: 0x04004B25 RID: 19237
	[SerializeField]
	private float maxSlipOverrideSpeedThreshold = 5f;

	// Token: 0x04004B26 RID: 19238
	[Header("Player Camera Rotation")]
	[SerializeField]
	private float subtlePlayerPitchFactor = 0.2f;

	// Token: 0x04004B27 RID: 19239
	[SerializeField]
	private float subtlePlayerPitchRate = 2f;

	// Token: 0x04004B28 RID: 19240
	[SerializeField]
	private float subtlePlayerRollFactor = 0.2f;

	// Token: 0x04004B29 RID: 19241
	[SerializeField]
	private float subtlePlayerRollRate = 2f;

	// Token: 0x04004B2A RID: 19242
	[SerializeField]
	private Vector2 subtlePlayerRotationSpeedRampMinMax = new Vector2(2f, 8f);

	// Token: 0x04004B2B RID: 19243
	[SerializeField]
	private Vector2 subtlePlayerRollAccelMinMax = new Vector2(0f, 30f);

	// Token: 0x04004B2C RID: 19244
	[SerializeField]
	private Vector2 subtlePlayerPitchAccelMinMax = new Vector2(0f, 10f);

	// Token: 0x04004B2D RID: 19245
	[SerializeField]
	private float accelSmoothingFollowRate = 2f;

	// Token: 0x04004B2E RID: 19246
	[Header("Haptics")]
	[SerializeField]
	private Vector2 hapticAccelInputRange = new Vector2(5f, 20f);

	// Token: 0x04004B2F RID: 19247
	[SerializeField]
	private float hapticAccelOutputMax = 0.35f;

	// Token: 0x04004B30 RID: 19248
	[SerializeField]
	private Vector2 hapticMaxSpeedInputRange = new Vector2(5f, 10f);

	// Token: 0x04004B31 RID: 19249
	[SerializeField]
	private Vector2 hapticSpeedInputRange = new Vector2(3f, 30f);

	// Token: 0x04004B32 RID: 19250
	[SerializeField]
	private float hapticSpeedOutputMax = 0.15f;

	// Token: 0x04004B33 RID: 19251
	[SerializeField]
	private Vector2 whistlingAudioSpeedInputRange = new Vector2(15f, 30f);

	// Token: 0x04004B34 RID: 19252
	[Header("Audio")]
	[SerializeField]
	private float audioVolumeMultiplier = 0.25f;

	// Token: 0x04004B35 RID: 19253
	[SerializeField]
	private float infectedAudioVolumeMultiplier = 0.5f;

	// Token: 0x04004B36 RID: 19254
	[SerializeField]
	private Vector2 whooshSpeedThresholdInput = new Vector2(10f, 25f);

	// Token: 0x04004B37 RID: 19255
	[SerializeField]
	private Vector2 whooshVolumeOutput = new Vector2(0.2f, 0.75f);

	// Token: 0x04004B38 RID: 19256
	[SerializeField]
	private float whooshCheckDistance = 2f;

	// Token: 0x04004B39 RID: 19257
	[Header("Tag Adjustment")]
	[SerializeField]
	private bool extendTagRangeInFlight = true;

	// Token: 0x04004B3A RID: 19258
	[SerializeField]
	private Vector2 tagRangeSpeedInput = new Vector2(5f, 20f);

	// Token: 0x04004B3B RID: 19259
	[SerializeField]
	private Vector2 tagRangeOutput = new Vector2(0.03f, 3f);

	// Token: 0x04004B3C RID: 19260
	[SerializeField]
	private bool debugDrawTagRange = true;

	// Token: 0x04004B3D RID: 19261
	[Header("Infected State")]
	[SerializeField]
	private float infectedSpeedIncrease = 5f;

	// Token: 0x04004B3E RID: 19262
	[Header("Glider Materials")]
	[SerializeField]
	private MeshRenderer leafMesh;

	// Token: 0x04004B3F RID: 19263
	[SerializeField]
	private Material baseLeafMaterial;

	// Token: 0x04004B40 RID: 19264
	[SerializeField]
	private Material infectedLeafMaterial;

	// Token: 0x04004B41 RID: 19265
	[SerializeField]
	private Material frozenLeafMaterial;

	// Token: 0x04004B42 RID: 19266
	[SerializeField]
	private GliderHoldable.CosmeticMaterialOverride[] cosmeticMaterialOverrides;

	// Token: 0x04004B43 RID: 19267
	[Header("Network Syncing")]
	[SerializeField]
	private float networkSyncFollowRate = 2f;

	// Token: 0x04004B44 RID: 19268
	[Header("Life Cycle")]
	[SerializeField]
	private Transform maxDistanceRespawnOrigin;

	// Token: 0x04004B45 RID: 19269
	[SerializeField]
	private float maxDistanceBeforeRespawn = 180f;

	// Token: 0x04004B46 RID: 19270
	[SerializeField]
	private float maxDroppedTimeToRespawn = 120f;

	// Token: 0x04004B47 RID: 19271
	[Header("Rigidbody")]
	[SerializeField]
	private float windUprightTorqueMultiplier = 1f;

	// Token: 0x04004B48 RID: 19272
	[SerializeField]
	private float gravityUprightTorqueMultiplier = 0.5f;

	// Token: 0x04004B49 RID: 19273
	[SerializeField]
	private float fallingGravityReduction = 0.1f;

	// Token: 0x04004B4A RID: 19274
	[Header("References")]
	[SerializeField]
	private AudioSource calmAudio;

	// Token: 0x04004B4B RID: 19275
	[SerializeField]
	private AudioSource activeAudio;

	// Token: 0x04004B4C RID: 19276
	[SerializeField]
	private AudioSource whistlingAudio;

	// Token: 0x04004B4D RID: 19277
	[SerializeField]
	private AudioSource leftWhooshAudio;

	// Token: 0x04004B4E RID: 19278
	[SerializeField]
	private AudioSource rightWhooshAudio;

	// Token: 0x04004B4F RID: 19279
	[SerializeField]
	private InteractionPoint handle;

	// Token: 0x04004B50 RID: 19280
	[SerializeField]
	private RequestableOwnershipGuard ownershipGuard;

	// Token: 0x04004B51 RID: 19281
	private bool subtlePlayerPitchActive = true;

	// Token: 0x04004B52 RID: 19282
	private bool subtlePlayerRollActive = true;

	// Token: 0x04004B53 RID: 19283
	private float subtlePlayerPitch;

	// Token: 0x04004B54 RID: 19284
	private float subtlePlayerRoll;

	// Token: 0x04004B55 RID: 19285
	private float subtlePlayerPitchRateExp = 0.75f;

	// Token: 0x04004B56 RID: 19286
	private float subtlePlayerRollRateExp = 0.025f;

	// Token: 0x04004B57 RID: 19287
	private float defaultMaxDistanceBeforeRespawn = 180f;

	// Token: 0x04004B58 RID: 19288
	private GliderHoldable.HoldingHand leftHold;

	// Token: 0x04004B59 RID: 19289
	private GliderHoldable.HoldingHand rightHold;

	// Token: 0x04004B5A RID: 19290
	private GliderHoldable.SyncedState syncedState;

	// Token: 0x04004B5B RID: 19291
	private Vector3 twoHandRotationOffsetAxis = Vector3.forward;

	// Token: 0x04004B5C RID: 19292
	private float twoHandRotationOffsetAngle;

	// Token: 0x04004B5D RID: 19293
	private Rigidbody rb;

	// Token: 0x04004B5E RID: 19294
	private Vector2 riderPosition = Vector2.zero;

	// Token: 0x04004B5F RID: 19295
	private Vector3 previousVelocity;

	// Token: 0x04004B60 RID: 19296
	private Vector3 currentVelocity;

	// Token: 0x04004B61 RID: 19297
	private float pitch;

	// Token: 0x04004B62 RID: 19298
	private float yaw;

	// Token: 0x04004B63 RID: 19299
	private float roll;

	// Token: 0x04004B64 RID: 19300
	private float pitchVel;

	// Token: 0x04004B65 RID: 19301
	private float yawVel;

	// Token: 0x04004B66 RID: 19302
	private float rollVel;

	// Token: 0x04004B67 RID: 19303
	private float oneHandRotationRateExp;

	// Token: 0x04004B68 RID: 19304
	private float twoHandRotationRateExp;

	// Token: 0x04004B69 RID: 19305
	private Quaternion playerFacingRotationOffset = Quaternion.identity;

	// Token: 0x04004B6A RID: 19306
	private const float accelAveragingWindow = 0.1f;

	// Token: 0x04004B6B RID: 19307
	private AverageVector3 accelerationAverage = new AverageVector3(0.1f);

	// Token: 0x04004B6C RID: 19308
	private float accelerationSmoothed;

	// Token: 0x04004B6D RID: 19309
	private float turnAccelerationSmoothed;

	// Token: 0x04004B6E RID: 19310
	private float accelSmoothingFollowRateExp = 1f;

	// Token: 0x04004B6F RID: 19311
	private float networkSyncFollowRateExp = 2f;

	// Token: 0x04004B70 RID: 19312
	private bool pendingOwnershipRequest;

	// Token: 0x04004B71 RID: 19313
	private Vector3 positionLocalToVRRig = Vector3.zero;

	// Token: 0x04004B72 RID: 19314
	private Quaternion rotationLocalToVRRig = Quaternion.identity;

	// Token: 0x04004B73 RID: 19315
	private Coroutine reenableOwnershipRequestCoroutine;

	// Token: 0x04004B74 RID: 19316
	private Vector3 spawnPosition;

	// Token: 0x04004B75 RID: 19317
	private Quaternion spawnRotation;

	// Token: 0x04004B76 RID: 19318
	private Vector3 skyJungleSpawnPostion;

	// Token: 0x04004B77 RID: 19319
	private Quaternion skyJungleSpawnRotation;

	// Token: 0x04004B78 RID: 19320
	private Transform skyJungleRespawnOrigin;

	// Token: 0x04004B79 RID: 19321
	private float lastHeldTime = -1f;

	// Token: 0x04004B7A RID: 19322
	private Vector3? leftHoldPositionLocal;

	// Token: 0x04004B7B RID: 19323
	private Vector3? rightHoldPositionLocal;

	// Token: 0x04004B7C RID: 19324
	private float whooshSoundDuration = 1f;

	// Token: 0x04004B7D RID: 19325
	private float whooshSoundRetriggerThreshold = 0.5f;

	// Token: 0x04004B7E RID: 19326
	private float leftWhooshStartTime = -1f;

	// Token: 0x04004B7F RID: 19327
	private Vector3 leftWhooshHitPoint = Vector3.zero;

	// Token: 0x04004B80 RID: 19328
	private Vector3 whooshAudioPositionOffset = new Vector3(0.5f, -0.25f, 0.5f);

	// Token: 0x04004B81 RID: 19329
	private float rightWhooshStartTime = -1f;

	// Token: 0x04004B82 RID: 19330
	private Vector3 rightWhooshHitPoint = Vector3.zero;

	// Token: 0x04004B83 RID: 19331
	private int ridersMaterialOverideIndex;

	// Token: 0x04004B84 RID: 19332
	private int windVolumeForceAppliedFrame = -1;

	// Token: 0x04004B85 RID: 19333
	private bool holdingTwoGliders;

	// Token: 0x04004B86 RID: 19334
	private GliderHoldable.GliderState gliderState;

	// Token: 0x04004B87 RID: 19335
	private float audioLevel;

	// Token: 0x04004B88 RID: 19336
	private int riderId = -1;

	// Token: 0x04004B89 RID: 19337
	[SerializeField]
	private VRRig cachedRig;

	// Token: 0x04004B8A RID: 19338
	private bool infectedState;

	// Token: 0x04004B8B RID: 19339
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 11)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private GliderHoldable.SyncedState _Data;

	// Token: 0x02000A1C RID: 2588
	private enum GliderState
	{
		// Token: 0x04004B8D RID: 19341
		LocallyHeld,
		// Token: 0x04004B8E RID: 19342
		LocallyDropped,
		// Token: 0x04004B8F RID: 19343
		RemoteSyncing
	}

	// Token: 0x02000A1D RID: 2589
	private struct HoldingHand
	{
		// Token: 0x06003F3C RID: 16188 RVA: 0x00143378 File Offset: 0x00141578
		public void Activate(Transform handTransform, Transform gliderTransform, Vector3 worldGrabPoint)
		{
			this.active = true;
			this.transform = handTransform.transform;
			this.holdLocalPos = handTransform.InverseTransformPoint(worldGrabPoint);
			this.handleLocalPos = gliderTransform.InverseTransformVector(gliderTransform.position - worldGrabPoint);
			this.localHoldRotation = Quaternion.Inverse(handTransform.rotation) * gliderTransform.rotation;
		}

		// Token: 0x06003F3D RID: 16189 RVA: 0x001433D9 File Offset: 0x001415D9
		public void Deactivate()
		{
			this.active = false;
			this.transform = null;
			this.holdLocalPos = Vector3.zero;
			this.handleLocalPos = Vector3.zero;
			this.localHoldRotation = Quaternion.identity;
		}

		// Token: 0x04004B90 RID: 19344
		public bool active;

		// Token: 0x04004B91 RID: 19345
		public Transform transform;

		// Token: 0x04004B92 RID: 19346
		public Vector3 holdLocalPos;

		// Token: 0x04004B93 RID: 19347
		public Vector3 handleLocalPos;

		// Token: 0x04004B94 RID: 19348
		public Quaternion localHoldRotation;
	}

	// Token: 0x02000A1E RID: 2590
	[NetworkStructWeaved(11)]
	[StructLayout(LayoutKind.Explicit, Size = 44)]
	internal struct SyncedState : INetworkStruct
	{
		// Token: 0x06003F3E RID: 16190 RVA: 0x0014340A File Offset: 0x0014160A
		public void Init(Vector3 defaultPosition, Quaternion defaultRotation)
		{
			this.riderId = -1;
			this.materialIndex = 0;
			this.audioLevel = 0;
			this.position = defaultPosition;
			this.rotation = defaultRotation;
		}

		// Token: 0x06003F3F RID: 16191 RVA: 0x0014342F File Offset: 0x0014162F
		public SyncedState(int id = -1)
		{
			this.riderId = id;
			this.materialIndex = 0;
			this.audioLevel = 0;
			this.tagged = default(NetworkBool);
			this.position = default(Vector3);
			this.rotation = default(Quaternion);
		}

		// Token: 0x04004B95 RID: 19349
		[FieldOffset(0)]
		public int riderId;

		// Token: 0x04004B96 RID: 19350
		[FieldOffset(4)]
		public byte materialIndex;

		// Token: 0x04004B97 RID: 19351
		[FieldOffset(8)]
		public byte audioLevel;

		// Token: 0x04004B98 RID: 19352
		[FieldOffset(12)]
		public NetworkBool tagged;

		// Token: 0x04004B99 RID: 19353
		[FieldOffset(16)]
		public Vector3 position;

		// Token: 0x04004B9A RID: 19354
		[FieldOffset(28)]
		public Quaternion rotation;
	}

	// Token: 0x02000A1F RID: 2591
	[Serializable]
	private struct CosmeticMaterialOverride
	{
		// Token: 0x04004B9B RID: 19355
		public string cosmeticName;

		// Token: 0x04004B9C RID: 19356
		public Material material;
	}
}
