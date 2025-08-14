using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using AA;
using BoingKit;
using GorillaExtensions;
using GorillaLocomotion.Climbing;
using GorillaLocomotion.Gameplay;
using GorillaLocomotion.Swimming;
using GorillaTag;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion
{
	// Token: 0x02000E07 RID: 3591
	public class GTPlayer : MonoBehaviour
	{
		// Token: 0x170008A1 RID: 2209
		// (get) Token: 0x060058E6 RID: 22758 RVA: 0x001B9AA2 File Offset: 0x001B7CA2
		public static GTPlayer Instance
		{
			get
			{
				return GTPlayer._instance;
			}
		}

		// Token: 0x170008A2 RID: 2210
		// (get) Token: 0x060058E7 RID: 22759 RVA: 0x001B9AA9 File Offset: 0x001B7CA9
		public Vector3 InstantaneousVelocity
		{
			get
			{
				return this.currentVelocity;
			}
		}

		// Token: 0x170008A3 RID: 2211
		// (get) Token: 0x060058E8 RID: 22760 RVA: 0x001B9AB1 File Offset: 0x001B7CB1
		public Vector3 AveragedVelocity
		{
			get
			{
				return this.averagedVelocity;
			}
		}

		// Token: 0x170008A4 RID: 2212
		// (get) Token: 0x060058E9 RID: 22761 RVA: 0x001B9AB9 File Offset: 0x001B7CB9
		public Transform CosmeticsHeadTarget
		{
			get
			{
				return this.cosmeticsHeadTarget;
			}
		}

		// Token: 0x170008A5 RID: 2213
		// (get) Token: 0x060058EA RID: 22762 RVA: 0x001B9AC1 File Offset: 0x001B7CC1
		public float scale
		{
			get
			{
				return this.scaleMultiplier * this.nativeScale;
			}
		}

		// Token: 0x170008A6 RID: 2214
		// (get) Token: 0x060058EB RID: 22763 RVA: 0x001B9AD0 File Offset: 0x001B7CD0
		public float NativeScale
		{
			get
			{
				return this.nativeScale;
			}
		}

		// Token: 0x170008A7 RID: 2215
		// (get) Token: 0x060058EC RID: 22764 RVA: 0x001B9AD8 File Offset: 0x001B7CD8
		public float ScaleMultiplier
		{
			get
			{
				return this.scaleMultiplier;
			}
		}

		// Token: 0x060058ED RID: 22765 RVA: 0x001B9AE0 File Offset: 0x001B7CE0
		public void SetScaleMultiplier(float s)
		{
			this.scaleMultiplier = s;
		}

		// Token: 0x060058EE RID: 22766 RVA: 0x001B9AEC File Offset: 0x001B7CEC
		public void SetNativeScale(NativeSizeChangerSettings s)
		{
			float num = this.nativeScale;
			if (s != null && s.playerSizeScale > 0f && s.playerSizeScale != 1f)
			{
				this.activeSizeChangerSettings = s;
			}
			else
			{
				this.activeSizeChangerSettings = null;
			}
			if (this.activeSizeChangerSettings == null)
			{
				this.nativeScale = 1f;
			}
			else
			{
				this.nativeScale = this.activeSizeChangerSettings.playerSizeScale;
			}
			if (num != this.nativeScale && NetworkSystem.Instance.InRoom)
			{
				GorillaTagger.Instance.myVRRig != null;
			}
		}

		// Token: 0x170008A8 RID: 2216
		// (get) Token: 0x060058EF RID: 22767 RVA: 0x001B9B77 File Offset: 0x001B7D77
		public bool IsDefaultScale
		{
			get
			{
				return Mathf.Abs(1f - this.scale) < 0.001f;
			}
		}

		// Token: 0x170008A9 RID: 2217
		// (get) Token: 0x060058F0 RID: 22768 RVA: 0x001B9B91 File Offset: 0x001B7D91
		public bool turnedThisFrame
		{
			get
			{
				return this.degreesTurnedThisFrame != 0f;
			}
		}

		// Token: 0x170008AA RID: 2218
		// (get) Token: 0x060058F1 RID: 22769 RVA: 0x001B9BA3 File Offset: 0x001B7DA3
		public List<GTPlayer.MaterialData> materialData
		{
			get
			{
				return this.materialDatasSO.datas;
			}
		}

		// Token: 0x170008AB RID: 2219
		// (get) Token: 0x060058F2 RID: 22770 RVA: 0x001B9BB0 File Offset: 0x001B7DB0
		// (set) Token: 0x060058F3 RID: 22771 RVA: 0x001B9BB8 File Offset: 0x001B7DB8
		protected bool IsFrozen { get; set; }

		// Token: 0x170008AC RID: 2220
		// (get) Token: 0x060058F4 RID: 22772 RVA: 0x001B9BC1 File Offset: 0x001B7DC1
		public List<WaterVolume> HeadOverlappingWaterVolumes
		{
			get
			{
				return this.headOverlappingWaterVolumes;
			}
		}

		// Token: 0x170008AD RID: 2221
		// (get) Token: 0x060058F5 RID: 22773 RVA: 0x001B9BC9 File Offset: 0x001B7DC9
		public bool InWater
		{
			get
			{
				return this.bodyInWater;
			}
		}

		// Token: 0x170008AE RID: 2222
		// (get) Token: 0x060058F6 RID: 22774 RVA: 0x001B9BD1 File Offset: 0x001B7DD1
		public bool HeadInWater
		{
			get
			{
				return this.headInWater;
			}
		}

		// Token: 0x170008AF RID: 2223
		// (get) Token: 0x060058F7 RID: 22775 RVA: 0x001B9BD9 File Offset: 0x001B7DD9
		public WaterVolume CurrentWaterVolume
		{
			get
			{
				if (this.bodyOverlappingWaterVolumes.Count <= 0)
				{
					return null;
				}
				return this.bodyOverlappingWaterVolumes[0];
			}
		}

		// Token: 0x170008B0 RID: 2224
		// (get) Token: 0x060058F8 RID: 22776 RVA: 0x001B9BF7 File Offset: 0x001B7DF7
		public WaterVolume.SurfaceQuery WaterSurfaceForHead
		{
			get
			{
				return this.waterSurfaceForHead;
			}
		}

		// Token: 0x170008B1 RID: 2225
		// (get) Token: 0x060058F9 RID: 22777 RVA: 0x001B9BFF File Offset: 0x001B7DFF
		public WaterVolume LeftHandWaterVolume
		{
			get
			{
				return this.leftHandWaterVolume;
			}
		}

		// Token: 0x170008B2 RID: 2226
		// (get) Token: 0x060058FA RID: 22778 RVA: 0x001B9C07 File Offset: 0x001B7E07
		public WaterVolume RightHandWaterVolume
		{
			get
			{
				return this.rightHandWaterVolume;
			}
		}

		// Token: 0x170008B3 RID: 2227
		// (get) Token: 0x060058FB RID: 22779 RVA: 0x001B9C0F File Offset: 0x001B7E0F
		public WaterVolume.SurfaceQuery LeftHandWaterSurface
		{
			get
			{
				return this.leftHandWaterSurface;
			}
		}

		// Token: 0x170008B4 RID: 2228
		// (get) Token: 0x060058FC RID: 22780 RVA: 0x001B9C17 File Offset: 0x001B7E17
		public WaterVolume.SurfaceQuery RightHandWaterSurface
		{
			get
			{
				return this.rightHandWaterSurface;
			}
		}

		// Token: 0x170008B5 RID: 2229
		// (get) Token: 0x060058FD RID: 22781 RVA: 0x001B9C1F File Offset: 0x001B7E1F
		public Vector3 LastLeftHandPosition
		{
			get
			{
				return this.lastLeftHandPosition;
			}
		}

		// Token: 0x170008B6 RID: 2230
		// (get) Token: 0x060058FE RID: 22782 RVA: 0x001B9C27 File Offset: 0x001B7E27
		public Vector3 LastRightHandPosition
		{
			get
			{
				return this.lastRightHandPosition;
			}
		}

		// Token: 0x170008B7 RID: 2231
		// (get) Token: 0x060058FF RID: 22783 RVA: 0x001B9C2F File Offset: 0x001B7E2F
		public Vector3 RigidbodyVelocity
		{
			get
			{
				return this.playerRigidBody.velocity;
			}
		}

		// Token: 0x170008B8 RID: 2232
		// (get) Token: 0x06005900 RID: 22784 RVA: 0x001B9C3C File Offset: 0x001B7E3C
		public Vector3 HeadCenterPosition
		{
			get
			{
				return this.headCollider.transform.position + this.headCollider.transform.rotation * new Vector3(0f, 0f, -0.11f);
			}
		}

		// Token: 0x170008B9 RID: 2233
		// (get) Token: 0x06005901 RID: 22785 RVA: 0x001B9C7C File Offset: 0x001B7E7C
		public bool HandContactingSurface
		{
			get
			{
				return this.isLeftHandColliding || this.isRightHandColliding;
			}
		}

		// Token: 0x170008BA RID: 2234
		// (get) Token: 0x06005902 RID: 22786 RVA: 0x001B9C8E File Offset: 0x001B7E8E
		public bool BodyOnGround
		{
			get
			{
				return this.bodyGroundContactTime >= Time.time - 0.05f;
			}
		}

		// Token: 0x170008BB RID: 2235
		// (get) Token: 0x06005903 RID: 22787 RVA: 0x001B9CA6 File Offset: 0x001B7EA6
		public bool IsGroundedHand
		{
			get
			{
				return this.HandContactingSurface || this.isClimbing || this.leftHandHolding || this.rightHandHolding;
			}
		}

		// Token: 0x170008BC RID: 2236
		// (get) Token: 0x06005904 RID: 22788 RVA: 0x001B9CC8 File Offset: 0x001B7EC8
		public bool IsGroundedButt
		{
			get
			{
				return this.BodyOnGround;
			}
		}

		// Token: 0x170008BD RID: 2237
		// (set) Token: 0x06005905 RID: 22789 RVA: 0x001B9CD0 File Offset: 0x001B7ED0
		public Quaternion PlayerRotationOverride
		{
			set
			{
				this.playerRotationOverride = value;
				this.playerRotationOverrideFrame = Time.frameCount;
			}
		}

		// Token: 0x170008BE RID: 2238
		// (get) Token: 0x06005906 RID: 22790 RVA: 0x001B9CE4 File Offset: 0x001B7EE4
		// (set) Token: 0x06005907 RID: 22791 RVA: 0x001B9CEC File Offset: 0x001B7EEC
		public bool IsBodySliding { get; set; }

		// Token: 0x170008BF RID: 2239
		// (get) Token: 0x06005908 RID: 22792 RVA: 0x001B9CF5 File Offset: 0x001B7EF5
		public GorillaClimbable CurrentClimbable
		{
			get
			{
				return this.currentClimbable;
			}
		}

		// Token: 0x170008C0 RID: 2240
		// (get) Token: 0x06005909 RID: 22793 RVA: 0x001B9CFD File Offset: 0x001B7EFD
		public GorillaHandClimber CurrentClimber
		{
			get
			{
				return this.currentClimber;
			}
		}

		// Token: 0x170008C1 RID: 2241
		// (get) Token: 0x0600590A RID: 22794 RVA: 0x001B9D05 File Offset: 0x001B7F05
		// (set) Token: 0x0600590B RID: 22795 RVA: 0x001B9D0D File Offset: 0x001B7F0D
		public float jumpMultiplier
		{
			get
			{
				return this._jumpMultiplier;
			}
			set
			{
				this._jumpMultiplier = value;
			}
		}

		// Token: 0x170008C2 RID: 2242
		// (get) Token: 0x0600590C RID: 22796 RVA: 0x001B9D16 File Offset: 0x001B7F16
		// (set) Token: 0x0600590D RID: 22797 RVA: 0x001B9D1E File Offset: 0x001B7F1E
		public float LastTouchedGroundAtNetworkTime { get; private set; }

		// Token: 0x170008C3 RID: 2243
		// (get) Token: 0x0600590E RID: 22798 RVA: 0x001B9D27 File Offset: 0x001B7F27
		// (set) Token: 0x0600590F RID: 22799 RVA: 0x001B9D2F File Offset: 0x001B7F2F
		public float LastHandTouchedGroundAtNetworkTime { get; private set; }

		// Token: 0x06005910 RID: 22800 RVA: 0x001B9D38 File Offset: 0x001B7F38
		private void Awake()
		{
			if (GTPlayer._instance != null && GTPlayer._instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				GTPlayer._instance = this;
				GTPlayer.hasInstance = true;
			}
			this.InitializeValues();
			this.playerRigidBody.maxAngularVelocity = 0f;
			this.bodyOffsetVector = new Vector3(0f, -this.bodyCollider.height / 2f, 0f);
			this.bodyInitialHeight = this.bodyCollider.height;
			this.bodyInitialRadius = this.bodyCollider.radius;
			this.rayCastNonAllocColliders = new RaycastHit[5];
			this.crazyCheckVectors = new Vector3[7];
			this.emptyHit = default(RaycastHit);
			this.crazyCheckVectors[0] = Vector3.up;
			this.crazyCheckVectors[1] = Vector3.down;
			this.crazyCheckVectors[2] = Vector3.left;
			this.crazyCheckVectors[3] = Vector3.right;
			this.crazyCheckVectors[4] = Vector3.forward;
			this.crazyCheckVectors[5] = Vector3.back;
			this.crazyCheckVectors[6] = Vector3.zero;
			if (this.controllerState == null)
			{
				this.controllerState = base.GetComponent<ConnectedControllerHandler>();
			}
			this.layerChanger = base.GetComponent<LayerChanger>();
			this.bodyTouchedSurfaces = new Dictionary<GameObject, PhysicMaterial>();
			Application.onBeforeRender += this.OnBeforeRenderInit;
		}

		// Token: 0x06005911 RID: 22801 RVA: 0x001B9EB4 File Offset: 0x001B80B4
		protected void Start()
		{
			if (this.mainCamera == null)
			{
				this.mainCamera = Camera.main;
			}
			this.mainCamera.farClipPlane = 500f;
			this.lastScale = this.scale;
			this.layerChanger.InitializeLayers(base.transform);
			float degrees = Quaternion.Angle(Quaternion.identity, GorillaTagger.Instance.offlineVRRig.transform.rotation) * Mathf.Sign(Vector3.Dot(Vector3.up, GorillaTagger.Instance.offlineVRRig.transform.right));
			this.Turn(degrees);
		}

		// Token: 0x06005912 RID: 22802 RVA: 0x001B9F51 File Offset: 0x001B8151
		protected void OnDestroy()
		{
			if (GTPlayer._instance == this)
			{
				GTPlayer._instance = null;
				GTPlayer.hasInstance = false;
			}
			if (this.climbHelper)
			{
				Object.Destroy(this.climbHelper.gameObject);
			}
		}

		// Token: 0x06005913 RID: 22803 RVA: 0x001B9F8C File Offset: 0x001B818C
		public void InitializeValues()
		{
			Physics.SyncTransforms();
			this.playerRigidBody = base.GetComponent<Rigidbody>();
			this.velocityHistory = new Vector3[this.velocityHistorySize];
			this.slideAverageHistory = new Vector3[this.velocityHistorySize];
			for (int i = 0; i < this.velocityHistory.Length; i++)
			{
				this.velocityHistory[i] = Vector3.zero;
				this.slideAverageHistory[i] = Vector3.zero;
			}
			this.leftHandFollower.transform.position = this.leftControllerTransform.position;
			this.rightHandFollower.transform.position = this.rightControllerTransform.position;
			this.lastLeftHandPosition = this.leftHandFollower.transform.position;
			this.lastRightHandPosition = this.rightHandFollower.transform.position;
			this.lastLeftHandRotation = this.leftHandFollower.transform.rotation;
			this.lastRightHandRotation = this.rightHandFollower.transform.rotation;
			this.lastHeadPosition = this.headCollider.transform.position;
			this.wasLeftHandColliding = false;
			this.wasRightHandColliding = false;
			this.velocityIndex = 0;
			this.averagedVelocity = Vector3.zero;
			this.slideVelocity = Vector3.zero;
			this.lastPosition = base.transform.position;
			this.lastRealTime = Time.realtimeSinceStartup;
			this.lastOpenHeadPosition = this.headCollider.transform.position;
			this.bodyCollider.transform.position = this.PositionWithOffset(this.headCollider.transform, this.bodyOffset) + this.bodyOffsetVector;
			this.bodyCollider.transform.eulerAngles = new Vector3(0f, this.headCollider.transform.eulerAngles.y, 0f);
		}

		// Token: 0x06005914 RID: 22804 RVA: 0x001BA168 File Offset: 0x001B8368
		public void SetHalloweenLevitation(float levitateStrength, float levitateDuration, float levitateBlendOutDuration, float levitateBonusStrength, float levitateBonusOffAtYSpeed, float levitateBonusFullAtYSpeed)
		{
			this.halloweenLevitationStrength = levitateStrength;
			this.halloweenLevitationFullStrengthDuration = levitateDuration;
			this.halloweenLevitationTotalDuration = levitateDuration + levitateBlendOutDuration;
			this.halloweenLevitateBonusFullAtYSpeed = levitateBonusFullAtYSpeed;
			this.halloweenLevitateBonusOffAtYSpeed = levitateBonusFullAtYSpeed;
			this.halloweenLevitationBonusStrength = levitateBonusStrength;
		}

		// Token: 0x06005915 RID: 22805 RVA: 0x001BA199 File Offset: 0x001B8399
		public void TeleportToTrain(bool enable)
		{
			this.teleportToTrain = enable;
		}

		// Token: 0x06005916 RID: 22806 RVA: 0x001BA1A4 File Offset: 0x001B83A4
		public void TeleportTo(Vector3 position, Quaternion rotation, bool keepVelocity = false)
		{
			Rigidbody component = base.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.isKinematic = true;
				component.position = position;
				component.rotation = rotation;
				component.isKinematic = false;
			}
			base.transform.position = position;
			base.transform.rotation = rotation;
			this.leftHandFollower.position = this.leftControllerTransform.position;
			this.leftHandFollower.rotation = this.leftControllerTransform.rotation;
			this.rightHandFollower.position = this.rightControllerTransform.position;
			this.rightHandFollower.rotation = this.rightControllerTransform.rotation;
			this.lastLeftHandPosition = this.leftHandFollower.transform.position;
			this.lastRightHandPosition = this.rightHandFollower.transform.position;
			this.lastHeadPosition = this.headCollider.transform.position;
			this.lastPosition = position;
			this.lastOpenHeadPosition = this.headCollider.transform.position;
			this.wasLeftHandColliding = false;
			this.wasRightHandColliding = false;
			this.isLeftHandColliding = false;
			this.isRightHandColliding = false;
			this.isLeftHandSliding = false;
			this.wasLeftHandSliding = false;
			this.isRightHandSliding = false;
			this.wasRightHandSliding = false;
			if (!keepVelocity)
			{
				this.playerRigidBody.velocity = Vector3.zero;
			}
			this.bodyCollider.transform.position = this.PositionWithOffset(this.headCollider.transform, this.bodyOffset) + this.bodyOffsetVector;
			this.bodyCollider.transform.eulerAngles = new Vector3(0f, this.headCollider.transform.eulerAngles.y, 0f);
			Physics.SyncTransforms();
			GorillaTagger.Instance.offlineVRRig.transform.position = position;
			GorillaTagger.Instance.offlineVRRig.leftHandLink.BreakLink();
			GorillaTagger.Instance.offlineVRRig.rightHandLink.BreakLink();
		}

		// Token: 0x06005917 RID: 22807 RVA: 0x001BA3A0 File Offset: 0x001B85A0
		public void TeleportTo(Transform destination, bool matchDestinationRotation = true, bool maintainVelocity = true)
		{
			Vector3 position = base.transform.position;
			Vector3 b = this.mainCamera.transform.position - position;
			Vector3 position2 = destination.position - b;
			float num = destination.rotation.eulerAngles.y - this.mainCamera.transform.rotation.eulerAngles.y;
			Vector3 playerVelocity = this.currentVelocity;
			if (!maintainVelocity)
			{
				this.SetPlayerVelocity(Vector3.zero);
			}
			else if (matchDestinationRotation)
			{
				playerVelocity = Quaternion.AngleAxis(num, base.transform.up) * this.currentVelocity;
				this.SetPlayerVelocity(playerVelocity);
			}
			if (matchDestinationRotation)
			{
				this.Turn(num);
			}
			this.TeleportTo(position2, base.transform.rotation, false);
			if (maintainVelocity)
			{
				this.SetPlayerVelocity(playerVelocity);
			}
		}

		// Token: 0x06005918 RID: 22808 RVA: 0x001BA47A File Offset: 0x001B867A
		public void AddForce(Vector3 force, ForceMode mode)
		{
			this.playerRigidBody.AddForce(force, mode);
		}

		// Token: 0x06005919 RID: 22809 RVA: 0x001BA48C File Offset: 0x001B868C
		public void SetPlayerVelocity(Vector3 newVelocity)
		{
			for (int i = 0; i < this.velocityHistory.Length; i++)
			{
				this.velocityHistory[i] = newVelocity;
			}
			this.playerRigidBody.AddForce(newVelocity - this.playerRigidBody.velocity, ForceMode.VelocityChange);
		}

		// Token: 0x0600591A RID: 22810 RVA: 0x001BA4D6 File Offset: 0x001B86D6
		public void SetGravityOverride(Object caller, Action<GTPlayer> gravityFunction)
		{
			if (!this.gravityOverrides.ContainsKey(caller))
			{
				this.gravityOverrides.Add(caller, gravityFunction);
				return;
			}
			this.gravityOverrides[caller] = gravityFunction;
		}

		// Token: 0x0600591B RID: 22811 RVA: 0x001BA501 File Offset: 0x001B8701
		public void UnsetGravityOverride(Object caller)
		{
			this.gravityOverrides.Remove(caller);
		}

		// Token: 0x0600591C RID: 22812 RVA: 0x001BA510 File Offset: 0x001B8710
		private void ApplyGravityOverrides()
		{
			foreach (KeyValuePair<Object, Action<GTPlayer>> keyValuePair in this.gravityOverrides)
			{
				keyValuePair.Value(this);
			}
		}

		// Token: 0x0600591D RID: 22813 RVA: 0x001BA56C File Offset: 0x001B876C
		public void ApplyKnockback(Vector3 direction, float speed, bool forceOffTheGround = false)
		{
			if (forceOffTheGround)
			{
				if (this.wasLeftHandColliding || this.wasRightHandColliding)
				{
					this.wasLeftHandColliding = false;
					this.wasRightHandColliding = false;
					this.playerRigidBody.transform.position += this.minimumRaycastDistance * this.scale * Vector3.up;
				}
				this.didAJump = true;
				this.SetMaximumSlipThisFrame();
			}
			if (speed > 0.01f)
			{
				float num = Vector3.Dot(this.averagedVelocity, direction);
				float d = Mathf.InverseLerp(1.5f, 0.5f, num / speed);
				Vector3 vector = this.averagedVelocity + direction * speed * d;
				this.playerRigidBody.velocity = vector;
				for (int i = 0; i < this.velocityHistory.Length; i++)
				{
					this.velocityHistory[i] = vector;
				}
			}
		}

		// Token: 0x0600591E RID: 22814 RVA: 0x001BA648 File Offset: 0x001B8848
		public void FixedUpdate()
		{
			this.AntiTeleportTechnology();
			this.IsFrozen = (GorillaTagger.Instance.offlineVRRig.IsFrozen || this.debugFreezeTag);
			bool isDefaultScale = this.IsDefaultScale;
			this.playerRigidBody.useGravity = false;
			if (this.gravityOverrides.Count > 0)
			{
				this.ApplyGravityOverrides();
			}
			else
			{
				if (!this.isClimbing)
				{
					this.playerRigidBody.AddForce(Physics.gravity * this.scale, ForceMode.Acceleration);
				}
				if (this.halloweenLevitationBonusStrength > 0f || this.halloweenLevitationStrength > 0f)
				{
					float num = Time.time - this.lastTouchedGroundTimestamp;
					if (num < this.halloweenLevitationTotalDuration)
					{
						this.playerRigidBody.AddForce(Vector3.up * this.halloweenLevitationStrength * Mathf.InverseLerp(this.halloweenLevitationFullStrengthDuration, this.halloweenLevitationTotalDuration, num), ForceMode.Acceleration);
					}
					float y = this.playerRigidBody.velocity.y;
					if (y <= this.halloweenLevitateBonusFullAtYSpeed)
					{
						this.playerRigidBody.AddForce(Vector3.up * this.halloweenLevitationBonusStrength, ForceMode.Acceleration);
					}
					else if (y <= this.halloweenLevitateBonusOffAtYSpeed)
					{
						Mathf.InverseLerp(this.halloweenLevitateBonusOffAtYSpeed, this.halloweenLevitateBonusFullAtYSpeed, this.playerRigidBody.velocity.y);
						this.playerRigidBody.AddForce(Vector3.up * this.halloweenLevitationBonusStrength * Mathf.InverseLerp(this.halloweenLevitateBonusOffAtYSpeed, this.halloweenLevitateBonusFullAtYSpeed, this.playerRigidBody.velocity.y), ForceMode.Acceleration);
					}
				}
			}
			if (this.enableHoverMode)
			{
				this.playerRigidBody.velocity = this.HoverboardFixedUpdate(this.playerRigidBody.velocity);
			}
			else
			{
				this.didHoverLastFrame = false;
			}
			float fixedDeltaTime = Time.fixedDeltaTime;
			this.bodyInWater = false;
			Vector3 lhs = this.swimmingVelocity;
			this.swimmingVelocity = Vector3.MoveTowards(this.swimmingVelocity, Vector3.zero, this.swimmingParams.swimmingVelocityOutOfWaterDrainRate * fixedDeltaTime);
			this.leftHandNonDiveHapticsAmount = 0f;
			this.rightHandNonDiveHapticsAmount = 0f;
			if (this.bodyOverlappingWaterVolumes.Count > 0)
			{
				WaterVolume waterVolume = null;
				float num2 = float.MinValue;
				Vector3 vector = this.headCollider.transform.position + Vector3.down * this.swimmingParams.floatingWaterLevelBelowHead * this.scale;
				this.activeWaterCurrents.Clear();
				for (int i = 0; i < this.bodyOverlappingWaterVolumes.Count; i++)
				{
					WaterVolume.SurfaceQuery surfaceQuery;
					if (this.bodyOverlappingWaterVolumes[i].GetSurfaceQueryForPoint(vector, out surfaceQuery, false))
					{
						float num3 = Vector3.Dot(surfaceQuery.surfacePoint - vector, surfaceQuery.surfaceNormal);
						if (num3 > num2)
						{
							num2 = num3;
							waterVolume = this.bodyOverlappingWaterVolumes[i];
							this.waterSurfaceForHead = surfaceQuery;
						}
						WaterCurrent waterCurrent = this.bodyOverlappingWaterVolumes[i].Current;
						if (waterCurrent != null && num3 > 0f && !this.activeWaterCurrents.Contains(waterCurrent))
						{
							this.activeWaterCurrents.Add(waterCurrent);
						}
					}
				}
				if (waterVolume != null)
				{
					Vector3 velocity = this.playerRigidBody.velocity;
					float magnitude = velocity.magnitude;
					bool flag = this.headInWater;
					this.headInWater = (this.headCollider.transform.position.y < this.waterSurfaceForHead.surfacePoint.y && this.headCollider.transform.position.y > this.waterSurfaceForHead.surfacePoint.y - this.waterSurfaceForHead.maxDepth);
					if (this.headInWater && !flag)
					{
						this.audioSetToUnderwater = true;
						this.audioManager.SetMixerSnapshot(this.audioManager.underwaterSnapshot, 0.1f);
					}
					else if (!this.headInWater && flag)
					{
						this.audioSetToUnderwater = false;
						this.audioManager.UnsetMixerSnapshot(0.1f);
					}
					this.bodyInWater = (vector.y < this.waterSurfaceForHead.surfacePoint.y && vector.y > this.waterSurfaceForHead.surfacePoint.y - this.waterSurfaceForHead.maxDepth);
					if (this.bodyInWater)
					{
						GTPlayer.LiquidProperties liquidProperties = this.liquidPropertiesList[(int)waterVolume.LiquidType];
						if (waterVolume != null)
						{
							float d;
							if (this.swimmingParams.extendBouyancyFromSpeed)
							{
								float time = Mathf.Clamp(Vector3.Dot(velocity / this.scale, this.waterSurfaceForHead.surfaceNormal), this.swimmingParams.speedToBouyancyExtensionMinMax.x, this.swimmingParams.speedToBouyancyExtensionMinMax.y);
								float b = this.swimmingParams.speedToBouyancyExtension.Evaluate(time);
								this.buoyancyExtension = Mathf.Max(this.buoyancyExtension, b);
								float num4 = Mathf.InverseLerp(0f, this.swimmingParams.buoyancyFadeDist + this.buoyancyExtension, num2 / this.scale + this.buoyancyExtension);
								this.buoyancyExtension = Spring.DamperDecayExact(this.buoyancyExtension, this.swimmingParams.buoyancyExtensionDecayHalflife, fixedDeltaTime, 1E-05f);
								d = num4;
							}
							else
							{
								d = Mathf.InverseLerp(0f, this.swimmingParams.buoyancyFadeDist, num2 / this.scale);
							}
							Vector3 a = Physics.gravity * this.scale;
							Vector3 vector2 = liquidProperties.buoyancy * -a * d;
							if (this.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
							{
								vector2 *= this.frozenBodyBuoyancyFactor;
							}
							this.playerRigidBody.AddForce(vector2, ForceMode.Acceleration);
						}
						Vector3 vector3 = Vector3.zero;
						Vector3 vector4 = Vector3.zero;
						for (int j = 0; j < this.activeWaterCurrents.Count; j++)
						{
							WaterCurrent waterCurrent2 = this.activeWaterCurrents[j];
							Vector3 startingVelocity = velocity + vector3;
							Vector3 b2;
							Vector3 b3;
							if (waterCurrent2.GetCurrentAtPoint(this.bodyCollider.transform.position, startingVelocity, fixedDeltaTime, out b2, out b3))
							{
								vector4 += b2;
								vector3 += b3;
							}
						}
						if (magnitude > Mathf.Epsilon)
						{
							float num5 = 0.01f;
							Vector3 vector5 = velocity / magnitude;
							Vector3 right = this.leftHandFollower.right;
							Vector3 dir = -this.rightHandFollower.right;
							Vector3 forward = this.leftHandFollower.forward;
							Vector3 forward2 = this.rightHandFollower.forward;
							Vector3 a2 = vector5;
							float num6 = 0f;
							float num7 = 0f;
							float num8 = 0f;
							if (this.swimmingParams.applyDiveSteering && !this.disableMovement && isDefaultScale)
							{
								float value = Vector3.Dot(velocity - vector4, vector5);
								float time2 = Mathf.Clamp(value, this.swimmingParams.swimSpeedToRedirectAmountMinMax.x, this.swimmingParams.swimSpeedToRedirectAmountMinMax.y);
								float b4 = this.swimmingParams.swimSpeedToRedirectAmount.Evaluate(time2);
								time2 = Mathf.Clamp(value, this.swimmingParams.swimSpeedToMaxRedirectAngleMinMax.x, this.swimmingParams.swimSpeedToMaxRedirectAngleMinMax.y);
								float num9 = this.swimmingParams.swimSpeedToMaxRedirectAngle.Evaluate(time2);
								float value2 = Mathf.Acos(Vector3.Dot(vector5, forward)) / 3.1415927f * -2f + 1f;
								float value3 = Mathf.Acos(Vector3.Dot(vector5, forward2)) / 3.1415927f * -2f + 1f;
								float num10 = Mathf.Clamp(value2, this.swimmingParams.palmFacingToRedirectAmountMinMax.x, this.swimmingParams.palmFacingToRedirectAmountMinMax.y);
								float num11 = Mathf.Clamp(value3, this.swimmingParams.palmFacingToRedirectAmountMinMax.x, this.swimmingParams.palmFacingToRedirectAmountMinMax.y);
								float a3 = (!float.IsNaN(num10)) ? this.swimmingParams.palmFacingToRedirectAmount.Evaluate(num10) : 0f;
								float a4 = (!float.IsNaN(num11)) ? this.swimmingParams.palmFacingToRedirectAmount.Evaluate(num11) : 0f;
								Vector3 a5 = Vector3.ProjectOnPlane(vector5, right);
								Vector3 a6 = Vector3.ProjectOnPlane(vector5, right);
								float num12 = Mathf.Min(a5.magnitude, 1f);
								float num13 = Mathf.Min(a6.magnitude, 1f);
								float magnitude2 = this.leftHandCenterVelocityTracker.GetAverageVelocity(false, this.swimmingParams.diveVelocityAveragingWindow, false).magnitude;
								float magnitude3 = this.rightHandCenterVelocityTracker.GetAverageVelocity(false, this.swimmingParams.diveVelocityAveragingWindow, false).magnitude;
								float time3 = Mathf.Clamp(magnitude2, this.swimmingParams.handSpeedToRedirectAmountMinMax.x, this.swimmingParams.handSpeedToRedirectAmountMinMax.y);
								float time4 = Mathf.Clamp(magnitude3, this.swimmingParams.handSpeedToRedirectAmountMinMax.x, this.swimmingParams.handSpeedToRedirectAmountMinMax.y);
								float a7 = this.swimmingParams.handSpeedToRedirectAmount.Evaluate(time3);
								float a8 = this.swimmingParams.handSpeedToRedirectAmount.Evaluate(time4);
								float averageSpeedChangeMagnitudeInDirection = this.leftHandCenterVelocityTracker.GetAverageSpeedChangeMagnitudeInDirection(right, false, this.swimmingParams.diveVelocityAveragingWindow);
								float averageSpeedChangeMagnitudeInDirection2 = this.rightHandCenterVelocityTracker.GetAverageSpeedChangeMagnitudeInDirection(dir, false, this.swimmingParams.diveVelocityAveragingWindow);
								float time5 = Mathf.Clamp(averageSpeedChangeMagnitudeInDirection, this.swimmingParams.handAccelToRedirectAmountMinMax.x, this.swimmingParams.handAccelToRedirectAmountMinMax.y);
								float time6 = Mathf.Clamp(averageSpeedChangeMagnitudeInDirection2, this.swimmingParams.handAccelToRedirectAmountMinMax.x, this.swimmingParams.handAccelToRedirectAmountMinMax.y);
								float b5 = this.swimmingParams.handAccelToRedirectAmount.Evaluate(time5);
								float b6 = this.swimmingParams.handAccelToRedirectAmount.Evaluate(time6);
								num6 = Mathf.Min(a3, Mathf.Min(a7, b5));
								float num14 = (Vector3.Dot(vector5, forward) > 0f) ? (Mathf.Min(num6, b4) * num12) : 0f;
								num7 = Mathf.Min(a4, Mathf.Min(a8, b6));
								float num15 = (Vector3.Dot(vector5, forward2) > 0f) ? (Mathf.Min(num7, b4) * num13) : 0f;
								if (this.swimmingParams.reduceDiveSteeringBelowVelocityPlane)
								{
									Vector3 rhs;
									if (Vector3.Dot(this.headCollider.transform.up, vector5) > 0.95f)
									{
										rhs = -this.headCollider.transform.forward;
									}
									else
									{
										rhs = Vector3.Cross(Vector3.Cross(vector5, this.headCollider.transform.up), vector5).normalized;
									}
									Vector3 position = this.headCollider.transform.position;
									Vector3 lhs2 = position - this.leftHandFollower.position;
									Vector3 lhs3 = position - this.rightHandFollower.position;
									float reduceDiveSteeringBelowPlaneFadeStartDist = this.swimmingParams.reduceDiveSteeringBelowPlaneFadeStartDist;
									float reduceDiveSteeringBelowPlaneFadeEndDist = this.swimmingParams.reduceDiveSteeringBelowPlaneFadeEndDist;
									float f = Vector3.Dot(lhs2, Vector3.up);
									float f2 = Vector3.Dot(lhs3, Vector3.up);
									float f3 = Vector3.Dot(lhs2, rhs);
									float f4 = Vector3.Dot(lhs3, rhs);
									float num16 = 1f - Mathf.InverseLerp(reduceDiveSteeringBelowPlaneFadeStartDist, reduceDiveSteeringBelowPlaneFadeEndDist, Mathf.Min(Mathf.Abs(f), Mathf.Abs(f3)));
									float num17 = 1f - Mathf.InverseLerp(reduceDiveSteeringBelowPlaneFadeStartDist, reduceDiveSteeringBelowPlaneFadeEndDist, Mathf.Min(Mathf.Abs(f2), Mathf.Abs(f4)));
									num14 *= num16;
									num15 *= num17;
								}
								float num18 = num15 + num14;
								Vector3 vector6 = Vector3.zero;
								if (this.swimmingParams.applyDiveSteering && num18 > num5)
								{
									vector6 = ((num14 * a5 + num15 * a6) / num18).normalized;
									vector6 = Vector3.Lerp(vector5, vector6, num18);
									a2 = Vector3.RotateTowards(vector5, vector6, 0.017453292f * num9 * fixedDeltaTime, 0f);
								}
								else
								{
									a2 = vector5;
								}
								num8 = Mathf.Clamp01((num6 + num7) * 0.5f);
							}
							float num19 = Mathf.Clamp(Vector3.Dot(lhs, vector5), 0f, magnitude);
							float num20 = magnitude - num19;
							if (this.swimmingParams.applyDiveSwimVelocityConversion && !this.disableMovement && num8 > num5 && num19 < this.swimmingParams.diveMaxSwimVelocityConversion)
							{
								float num21 = Mathf.Min(this.swimmingParams.diveSwimVelocityConversionRate * fixedDeltaTime, num20) * num8;
								num19 += num21;
								num20 -= num21;
							}
							float halflife = this.swimmingParams.swimUnderWaterDampingHalfLife * liquidProperties.dampingFactor;
							float halflife2 = this.swimmingParams.baseUnderWaterDampingHalfLife * liquidProperties.dampingFactor;
							float num22 = Spring.DamperDecayExact(num19 / this.scale, halflife, fixedDeltaTime, 1E-05f) * this.scale;
							float num23 = Spring.DamperDecayExact(num20 / this.scale, halflife2, fixedDeltaTime, 1E-05f) * this.scale;
							if (this.swimmingParams.applyDiveDampingMultiplier && !this.disableMovement)
							{
								float t = Mathf.Lerp(1f, this.swimmingParams.diveDampingMultiplier, num8);
								num22 = Mathf.Lerp(num19, num22, t);
								num23 = Mathf.Lerp(num20, num23, t);
								float time7 = Mathf.Clamp((1f - num6) * (num19 + num20), this.swimmingParams.nonDiveDampingHapticsAmountMinMax.x + num5, this.swimmingParams.nonDiveDampingHapticsAmountMinMax.y - num5);
								float time8 = Mathf.Clamp((1f - num7) * (num19 + num20), this.swimmingParams.nonDiveDampingHapticsAmountMinMax.x + num5, this.swimmingParams.nonDiveDampingHapticsAmountMinMax.y - num5);
								this.leftHandNonDiveHapticsAmount = this.swimmingParams.nonDiveDampingHapticsAmount.Evaluate(time7);
								this.rightHandNonDiveHapticsAmount = this.swimmingParams.nonDiveDampingHapticsAmount.Evaluate(time8);
							}
							this.swimmingVelocity = num22 * a2 + vector3 * this.scale;
							this.playerRigidBody.velocity = this.swimmingVelocity + num23 * a2;
						}
					}
				}
			}
			else if (this.audioSetToUnderwater)
			{
				this.audioSetToUnderwater = false;
				this.audioManager.UnsetMixerSnapshot(0.1f);
			}
			this.handleClimbing(Time.fixedDeltaTime);
			this.stuckHandsCheckFixedUpdate();
			this.FixedUpdate_HandHolds(Time.fixedDeltaTime);
		}

		// Token: 0x170008C4 RID: 2244
		// (get) Token: 0x0600591F RID: 22815 RVA: 0x001BB493 File Offset: 0x001B9693
		// (set) Token: 0x06005920 RID: 22816 RVA: 0x001BB49B File Offset: 0x001B969B
		public bool isHoverAllowed { get; private set; }

		// Token: 0x170008C5 RID: 2245
		// (get) Token: 0x06005921 RID: 22817 RVA: 0x001BB4A4 File Offset: 0x001B96A4
		// (set) Token: 0x06005922 RID: 22818 RVA: 0x001BB4AC File Offset: 0x001B96AC
		public bool enableHoverMode { get; private set; }

		// Token: 0x06005923 RID: 22819 RVA: 0x001BB4B5 File Offset: 0x001B96B5
		public void SetHoverboardPosRot(Vector3 worldPos, Quaternion worldRot)
		{
			this.hoverboardPlayerLocalPos = this.headCollider.transform.InverseTransformPoint(worldPos);
			this.hoverboardPlayerLocalRot = this.headCollider.transform.InverseTransformRotation(worldRot);
		}

		// Token: 0x06005924 RID: 22820 RVA: 0x001BB4E8 File Offset: 0x001B96E8
		private void HoverboardLateUpdate()
		{
			Vector3 eulerAngles = this.headCollider.transform.eulerAngles;
			bool flag = false;
			for (int i = 0; i < this.hoverboardCasts.Length; i++)
			{
				GTPlayer.HoverBoardCast hoverBoardCast = this.hoverboardCasts[i];
				RaycastHit raycastHit;
				hoverBoardCast.didHit = Physics.SphereCast(new Ray(this.hoverboardVisual.transform.TransformPoint(hoverBoardCast.localOrigin), this.hoverboardVisual.transform.rotation * hoverBoardCast.localDirection), hoverBoardCast.sphereRadius, out raycastHit, hoverBoardCast.distance, this.locomotionEnabledLayers);
				if (hoverBoardCast.didHit)
				{
					HoverboardCantHover hoverboardCantHover;
					if (raycastHit.collider.TryGetComponent<HoverboardCantHover>(out hoverboardCantHover))
					{
						hoverBoardCast.didHit = false;
					}
					else
					{
						hoverBoardCast.pointHit = raycastHit.point;
						hoverBoardCast.normalHit = raycastHit.normal;
					}
				}
				this.hoverboardCasts[i] = hoverBoardCast;
				if (hoverBoardCast.didHit)
				{
					flag = true;
				}
			}
			this.hasHoverPoint = flag;
			this.bodyCollider.enabled = (this.bodyCollider.transform.position - this.hoverboardVisual.transform.TransformPoint(Vector3.up * this.hoverBodyCollisionRadiusUpOffset)).IsLongerThan(this.hoverBodyHasCollisionsOutsideRadius);
		}

		// Token: 0x06005925 RID: 22821 RVA: 0x001BB630 File Offset: 0x001B9830
		private Vector3 HoverboardFixedUpdate(Vector3 velocity)
		{
			this.hoverboardVisual.transform.position = this.headCollider.transform.TransformPoint(this.hoverboardPlayerLocalPos);
			this.hoverboardVisual.transform.rotation = this.headCollider.transform.TransformRotation(this.hoverboardPlayerLocalRot);
			if (this.didHoverLastFrame)
			{
				velocity += Vector3.up * this.hoverGeneralUpwardForce * Time.fixedDeltaTime;
			}
			Vector3 position = this.hoverboardVisual.transform.position;
			Vector3 a = position + velocity * Time.fixedDeltaTime;
			Vector3 vector = this.hoverboardVisual.transform.forward;
			Vector3 vector2 = this.hoverboardCasts[0].didHit ? this.hoverboardCasts[0].normalHit : Vector3.up;
			bool flag = false;
			for (int i = 0; i < this.hoverboardCasts.Length; i++)
			{
				GTPlayer.HoverBoardCast hoverBoardCast = this.hoverboardCasts[i];
				if (hoverBoardCast.didHit)
				{
					Vector3 b = position + Vector3.Project(hoverBoardCast.pointHit - position, vector);
					Vector3 b2 = a + Vector3.Project(hoverBoardCast.pointHit - position, vector);
					bool flag2 = hoverBoardCast.isSolid || Vector3.Dot(hoverBoardCast.normalHit, hoverBoardCast.pointHit - b2) + this.hoverIdealHeight > 0f;
					float d = hoverBoardCast.isSolid ? (Vector3.Dot(hoverBoardCast.normalHit, hoverBoardCast.pointHit - this.hoverboardVisual.transform.TransformPoint(hoverBoardCast.localOrigin + hoverBoardCast.localDirection * hoverBoardCast.distance)) + hoverBoardCast.sphereRadius) : (Vector3.Dot(hoverBoardCast.normalHit, hoverBoardCast.pointHit - b) + this.hoverIdealHeight);
					if (flag2)
					{
						flag = true;
						this.boostEnabledUntilTimestamp = Time.time + this.hoverboardBoostGracePeriod;
						if (Vector3.Dot(velocity, hoverBoardCast.normalHit) < 0f)
						{
							velocity = Vector3.ProjectOnPlane(velocity, hoverBoardCast.normalHit);
						}
						this.playerRigidBody.transform.position += hoverBoardCast.normalHit * d;
						Vector3 vector3 = this.turnParent.transform.rotation * (this.hoverboardVisual.IsLeftHanded ? this.leftHandCenterVelocityTracker : this.rightHandCenterVelocityTracker).GetAverageVelocity(false, 0.15f, false);
						if (Vector3.Dot(vector3, hoverBoardCast.normalHit) < 0f)
						{
							velocity -= Vector3.Project(vector3, hoverBoardCast.normalHit) * this.hoverSlamJumpStrengthFactor * Time.fixedDeltaTime;
						}
						a = position + velocity * Time.fixedDeltaTime;
					}
				}
			}
			float time = Mathf.Abs(Mathf.DeltaAngle(0f, Mathf.Acos(Vector3.Dot(this.hoverboardVisual.transform.up, Vector3.ProjectOnPlane(vector2, vector).normalized)) * 57.29578f));
			float num = this.hoverCarveAngleResponsiveness.Evaluate(time);
			vector = (vector + Vector3.ProjectOnPlane(this.hoverboardVisual.transform.up, vector2) * this.hoverTiltAdjustsForwardFactor).normalized;
			if (!flag)
			{
				this.didHoverLastFrame = false;
				num = 0f;
			}
			Vector3 b3 = velocity;
			if (this.enableHoverMode && this.hasHoverPoint)
			{
				Vector3 vector4 = Vector3.ProjectOnPlane(velocity, vector2);
				Vector3 b4 = velocity - vector4;
				Vector3 vector5 = Vector3.Project(vector4, vector);
				float num2 = vector4.magnitude;
				if (num2 <= this.hoveringSlowSpeed)
				{
					num2 *= this.hoveringSlowStoppingFactor;
				}
				Vector3 vector6 = vector4 - vector5;
				float num3 = 0f;
				bool flag3 = false;
				if (num > 0f)
				{
					if (vector6.IsLongerThan(vector5))
					{
						num3 = Mathf.Min((vector6.magnitude - vector5.magnitude) * this.hoverCarveSidewaysSpeedLossFactor * num, num2);
						if (num3 > 0f && num2 > this.hoverMinGrindSpeed)
						{
							flag3 = true;
							this.hoverboardVisual.PlayGrindHaptic();
						}
						num2 -= num3;
					}
					vector6 *= 1f - num * this.sidewaysDrag;
					if (!this.isLeftHandColliding && !this.isRightHandColliding)
					{
						velocity = (vector5 + vector6).normalized * num2 + b4;
					}
				}
				else
				{
					velocity = vector4.normalized * num2 + b4;
				}
				float magnitude = (velocity - b3).magnitude;
				this.hoverboardAudio.UpdateAudioLoop(velocity.magnitude, this.bodyVelocityTracker.GetAverageVelocity(true, 0.15f, false).magnitude, magnitude, flag3 ? num3 : 0f);
				if (magnitude > 0f && !flag3)
				{
					this.hoverboardVisual.PlayCarveHaptic(magnitude);
				}
			}
			else
			{
				this.hoverboardAudio.UpdateAudioLoop(0f, this.bodyVelocityTracker.GetAverageVelocity(true, 0.15f, false).magnitude, 0f, 0f);
			}
			return velocity;
		}

		// Token: 0x06005926 RID: 22822 RVA: 0x001BBB8C File Offset: 0x001B9D8C
		public void GrabPersonalHoverboard(bool isLeftHand, Vector3 pos, Quaternion rot, Color col)
		{
			if (this.hoverboardVisual.IsHeld)
			{
				this.hoverboardVisual.DropFreeBoard();
			}
			this.hoverboardVisual.SetIsHeld(isLeftHand, pos, rot, col);
			this.hoverboardVisual.ProxyGrabHandle(isLeftHand);
			FreeHoverboardManager.instance.PreserveMaxHoverboardsConstraint(NetworkSystem.Instance.LocalPlayer.ActorNumber);
		}

		// Token: 0x06005927 RID: 22823 RVA: 0x001BBBE8 File Offset: 0x001B9DE8
		public void SetHoverAllowed(bool allowed, bool force = false)
		{
			if (allowed)
			{
				this.hoverAllowedCount++;
				this.isHoverAllowed = true;
				return;
			}
			this.hoverAllowedCount = ((force || this.hoverAllowedCount == 0) ? 0 : (this.hoverAllowedCount - 1));
			if (this.hoverAllowedCount == 0 && this.isHoverAllowed)
			{
				this.isHoverAllowed = false;
				if (this.enableHoverMode)
				{
					this.SetHoverActive(false);
					VRRig.LocalRig.hoverboardVisual.SetNotHeld();
				}
			}
		}

		// Token: 0x06005928 RID: 22824 RVA: 0x001BBC60 File Offset: 0x001B9E60
		public void SetHoverActive(bool enable)
		{
			if (enable && !this.isHoverAllowed)
			{
				return;
			}
			this.enableHoverMode = enable;
			if (!enable)
			{
				this.bodyCollider.enabled = true;
				this.hasHoverPoint = false;
				this.didHoverLastFrame = false;
				for (int i = 0; i < this.hoverboardCasts.Length; i++)
				{
					this.hoverboardCasts[i].didHit = false;
				}
				this.hoverboardAudio.Stop();
			}
		}

		// Token: 0x06005929 RID: 22825 RVA: 0x001BBCD0 File Offset: 0x001B9ED0
		private void BodyCollider()
		{
			if (this.MaxSphereSizeForNoOverlap(this.bodyInitialRadius * this.scale, this.PositionWithOffset(this.headCollider.transform, this.bodyOffset), false, out this.bodyMaxRadius))
			{
				if (this.scale > 0f)
				{
					this.bodyCollider.radius = this.bodyMaxRadius / this.scale;
				}
				int num = Physics.SphereCastNonAlloc(this.PositionWithOffset(this.headCollider.transform, this.bodyOffset), this.bodyMaxRadius, Vector3.down, this.rayCastNonAllocColliders, this.bodyInitialHeight * this.scale - this.bodyMaxRadius, this.locomotionEnabledLayers);
				if (num > 0)
				{
					this.bodyHitInfo = this.rayCastNonAllocColliders.Take(num).MinBy((RaycastHit col) => col.distance);
					this.bodyCollider.height = (this.bodyHitInfo.distance + this.bodyMaxRadius) / this.scale;
				}
				else
				{
					this.bodyHitInfo = this.emptyHit;
					this.bodyCollider.height = this.bodyInitialHeight;
				}
				if (!this.bodyCollider.gameObject.activeSelf)
				{
					this.bodyCollider.gameObject.SetActive(true);
				}
			}
			else
			{
				this.bodyCollider.gameObject.SetActive(false);
			}
			this.bodyCollider.height = Mathf.Lerp(this.bodyCollider.height, this.bodyInitialHeight, this.bodyLerp);
			this.bodyCollider.radius = Mathf.Lerp(this.bodyCollider.radius, this.bodyInitialRadius, this.bodyLerp);
			this.bodyOffsetVector = Vector3.down * this.bodyCollider.height / 2f;
			this.bodyCollider.transform.position = this.PositionWithOffset(this.headCollider.transform, this.bodyOffset) + this.bodyOffsetVector * this.scale;
			this.bodyCollider.transform.eulerAngles = new Vector3(0f, this.headCollider.transform.eulerAngles.y, 0f);
		}

		// Token: 0x0600592A RID: 22826 RVA: 0x001BBF1C File Offset: 0x001BA11C
		private Vector3 GetCurrentHandPosition(Transform handTransform, Vector3 handOffset)
		{
			if (this.inOverlay)
			{
				return this.headCollider.transform.position + this.headCollider.transform.up * -0.5f * this.scale;
			}
			if ((this.PositionWithOffset(handTransform, handOffset) - this.headCollider.transform.position).magnitude < this.maxArmLength * this.scale)
			{
				return this.PositionWithOffset(handTransform, handOffset);
			}
			return this.headCollider.transform.position + (this.PositionWithOffset(handTransform, handOffset) - this.headCollider.transform.position).normalized * this.maxArmLength * this.scale;
		}

		// Token: 0x0600592B RID: 22827 RVA: 0x001BBFF9 File Offset: 0x001BA1F9
		private Vector3 GetLastLeftHandPosition()
		{
			return this.lastLeftHandPosition + this.MovingSurfaceMovement();
		}

		// Token: 0x0600592C RID: 22828 RVA: 0x001BC00C File Offset: 0x001BA20C
		private Vector3 GetLastRightHandPosition()
		{
			return this.lastRightHandPosition + this.MovingSurfaceMovement();
		}

		// Token: 0x0600592D RID: 22829 RVA: 0x001BC020 File Offset: 0x001BA220
		private Vector3 GetCurrentLeftHandPosition()
		{
			if (this.inOverlay)
			{
				return this.headCollider.transform.position + this.headCollider.transform.up * -0.5f * this.scale;
			}
			if ((this.PositionWithOffset(this.leftControllerTransform, this.leftHandOffset) - this.headCollider.transform.position).magnitude < this.maxArmLength * this.scale)
			{
				return this.PositionWithOffset(this.leftControllerTransform, this.leftHandOffset);
			}
			return this.headCollider.transform.position + (this.PositionWithOffset(this.leftControllerTransform, this.leftHandOffset) - this.headCollider.transform.position).normalized * this.maxArmLength * this.scale;
		}

		// Token: 0x0600592E RID: 22830 RVA: 0x001BC11C File Offset: 0x001BA31C
		private Vector3 GetCurrentRightHandPosition()
		{
			if (this.inOverlay)
			{
				return this.headCollider.transform.position + this.headCollider.transform.up * -0.5f * this.scale;
			}
			if ((this.PositionWithOffset(this.rightControllerTransform, this.rightHandOffset) - this.headCollider.transform.position).magnitude < this.maxArmLength * this.scale)
			{
				return this.PositionWithOffset(this.rightControllerTransform, this.rightHandOffset);
			}
			return this.headCollider.transform.position + (this.PositionWithOffset(this.rightControllerTransform, this.rightHandOffset) - this.headCollider.transform.position).normalized * this.maxArmLength * this.scale;
		}

		// Token: 0x0600592F RID: 22831 RVA: 0x001BC217 File Offset: 0x001BA417
		private Vector3 PositionWithOffset(Transform transformToModify, Vector3 offsetVector)
		{
			return transformToModify.position + transformToModify.rotation * offsetVector * this.scale;
		}

		// Token: 0x06005930 RID: 22832 RVA: 0x001BC23C File Offset: 0x001BA43C
		public void ScaleAwayFromPoint(float oldScale, float newScale, Vector3 scaleCenter)
		{
			if (oldScale < newScale)
			{
				this.lastHeadPosition = GTPlayer.ScalePointAwayFromCenter(this.lastHeadPosition, this.headCollider.radius, oldScale, newScale, scaleCenter);
				this.lastLeftHandPosition = GTPlayer.ScalePointAwayFromCenter(this.lastLeftHandPosition, this.minimumRaycastDistance, oldScale, newScale, scaleCenter);
				this.lastRightHandPosition = GTPlayer.ScalePointAwayFromCenter(this.lastRightHandPosition, this.minimumRaycastDistance, oldScale, newScale, scaleCenter);
			}
		}

		// Token: 0x06005931 RID: 22833 RVA: 0x001BC2A0 File Offset: 0x001BA4A0
		private static Vector3 ScalePointAwayFromCenter(Vector3 point, float baseRadius, float oldScale, float newScale, Vector3 scaleCenter)
		{
			float magnitude = (point - scaleCenter).magnitude;
			float d = magnitude + Mathf.Epsilon + baseRadius * (newScale - oldScale);
			return scaleCenter + (point - scaleCenter) * d / magnitude;
		}

		// Token: 0x06005932 RID: 22834 RVA: 0x001BC2E8 File Offset: 0x001BA4E8
		private void OnBeforeRenderInit()
		{
			if (!this.hasCorrectedForTracking && this.mainCamera.transform.localPosition != Vector3.zero)
			{
				base.transform.position -= this.mainCamera.transform.localPosition;
				this.hasCorrectedForTracking = true;
				Application.onBeforeRender -= this.OnBeforeRenderInit;
			}
		}

		// Token: 0x06005933 RID: 22835 RVA: 0x001BC358 File Offset: 0x001BA558
		private void LateUpdate()
		{
			if (!this.hasCorrectedForTracking && this.mainCamera.transform.localPosition != Vector3.zero)
			{
				base.transform.position -= this.mainCamera.transform.localPosition;
				this.hasCorrectedForTracking = true;
				Application.onBeforeRender -= this.OnBeforeRenderInit;
			}
			if (this.playerRigidBody.isKinematic)
			{
				return;
			}
			float time = Time.time;
			Vector3 position = this.headCollider.transform.position;
			if (this.playerRotationOverrideFrame < Time.frameCount - 1)
			{
				this.playerRotationOverride = Quaternion.Slerp(Quaternion.identity, this.playerRotationOverride, Mathf.Exp(-this.playerRotationOverrideDecayRate * Time.deltaTime));
			}
			base.transform.rotation = this.playerRotationOverride;
			this.turnParent.transform.localScale = VRRig.LocalRig.transform.localScale;
			this.playerRigidBody.MovePosition(this.playerRigidBody.position + position - this.headCollider.transform.position);
			if (Mathf.Abs(this.lastScale - this.scale) > 0.001f)
			{
				if (this.mainCamera == null)
				{
					this.mainCamera = Camera.main;
				}
				this.mainCamera.nearClipPlane = ((this.scale > 0.5f) ? 0.01f : 0.002f);
			}
			this.lastScale = this.scale;
			this.debugLastRightHandPosition = this.lastRightHandPosition;
			this.debugPlatformDeltaPosition = this.MovingSurfaceMovement();
			if (this.debugMovement)
			{
				this.tempRealTime = Time.time;
				this.calcDeltaTime = Time.deltaTime;
				this.lastRealTime = this.tempRealTime;
			}
			else
			{
				this.tempRealTime = Time.realtimeSinceStartup;
				this.calcDeltaTime = this.tempRealTime - this.lastRealTime;
				this.lastRealTime = this.tempRealTime;
				if (this.calcDeltaTime > 0.1f)
				{
					this.calcDeltaTime = 0.05f;
				}
			}
			Vector3 a;
			if (this.lastFrameHasValidTouchPos && this.lastPlatformTouched != null && GTPlayer.ComputeWorldHitPoint(this.lastHitInfoHand, this.lastFrameTouchPosLocal, out a))
			{
				this.refMovement = a - this.lastFrameTouchPosWorld;
			}
			else
			{
				this.refMovement = Vector3.zero;
			}
			Vector3 vector = Vector3.zero;
			Quaternion quaternion = Quaternion.identity;
			Vector3 pivot = this.headCollider.transform.position;
			Vector3 vector2;
			if (this.lastMovingSurfaceContact != GTPlayer.MovingSurfaceContactPoint.NONE && GTPlayer.ComputeWorldHitPoint(this.lastMovingSurfaceHit, this.lastMovingSurfaceTouchLocal, out vector2))
			{
				if (this.wasMovingSurfaceMonkeBlock && (this.lastMonkeBlock == null || this.lastMonkeBlock.state != BuilderPiece.State.AttachedAndPlaced))
				{
					this.movingSurfaceOffset = Vector3.zero;
				}
				else
				{
					this.movingSurfaceOffset = vector2 - this.lastMovingSurfaceTouchWorld;
					vector = this.movingSurfaceOffset / this.calcDeltaTime;
					quaternion = this.lastMovingSurfaceHit.collider.transform.rotation * Quaternion.Inverse(this.lastMovingSurfaceRot);
					pivot = vector2;
				}
			}
			else
			{
				this.movingSurfaceOffset = Vector3.zero;
			}
			float num = 40f * this.scale;
			if (vector.sqrMagnitude >= num * num)
			{
				this.movingSurfaceOffset = Vector3.zero;
				vector = Vector3.zero;
				quaternion = Quaternion.identity;
			}
			if (!this.didAJump && (this.wasLeftHandColliding || this.wasRightHandColliding))
			{
				base.transform.position = base.transform.position + 4.9f * Vector3.down * this.calcDeltaTime * this.calcDeltaTime * this.scale;
				if (Vector3.Dot(this.averagedVelocity, this.slideAverageNormal) <= 0f && Vector3.Dot(Vector3.up, this.slideAverageNormal) > 0f)
				{
					base.transform.position = base.transform.position - Vector3.Project(Mathf.Min(this.stickDepth * this.scale, Vector3.Project(this.averagedVelocity, this.slideAverageNormal).magnitude * this.calcDeltaTime) * this.slideAverageNormal, Vector3.down);
				}
			}
			if (!this.didAJump && (this.wasLeftHandSliding || this.wasRightHandSliding))
			{
				base.transform.position = base.transform.position + this.slideVelocity * this.calcDeltaTime;
				this.slideVelocity += 9.8f * Vector3.down * this.calcDeltaTime * this.scale;
			}
			float d = (Time.time > this.boostEnabledUntilTimestamp) ? 0f : (Time.deltaTime * Mathf.Clamp(this.playerRigidBody.velocity.magnitude * this.hoverboardPaddleBoostMultiplier, 0f, this.hoverboardPaddleBoostMax));
			Vector3 boostVector = this.enableHoverMode ? (this.turnParent.transform.rotation * -this.leftHandCenterVelocityTracker.GetAverageVelocity(false, 0.15f, false) * d) : Vector3.zero;
			Vector3 b;
			this.FirstHandIteration(this.leftControllerTransform, this.leftHandOffset, this.GetLastLeftHandPosition(), boostVector, this.wasLeftHandSliding, this.wasLeftHandColliding, this.LeftSlipOverriddenToMax(), out b, out this.leftHandSlipPercentage, out this.isLeftHandSliding, ref this.leftHandSlideNormal, out this.isLeftHandColliding, out this.leftHandMaterialTouchIndex, out this.leftHandSurfaceOverride, this.leftHandHolding, this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.LEFT);
			this.isLeftHandColliding = (this.isLeftHandColliding && this.controllerState.LeftValid);
			this.isLeftHandSliding = (this.isLeftHandSliding && this.controllerState.LeftValid);
			Vector3 boostVector2 = this.enableHoverMode ? (this.turnParent.transform.rotation * -this.rightHandCenterVelocityTracker.GetAverageVelocity(false, 0.15f, false) * d) : Vector3.zero;
			Vector3 b2;
			this.FirstHandIteration(this.rightControllerTransform, this.rightHandOffset, this.GetLastRightHandPosition(), boostVector2, this.wasRightHandSliding, this.wasRightHandColliding, this.RightSlipOverriddenToMax(), out b2, out this.rightHandSlipPercentage, out this.isRightHandSliding, ref this.rightHandSlideNormal, out this.isRightHandColliding, out this.rightHandMaterialTouchIndex, out this.rightHandSurfaceOverride, this.rightHandHolding, this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.RIGHT);
			this.isRightHandColliding = (this.isRightHandColliding && this.controllerState.RightValid);
			this.isRightHandSliding = (this.isRightHandSliding && this.controllerState.RightValid);
			this.touchPoints = 0;
			Vector3 vector3 = Vector3.zero;
			if (this.isLeftHandColliding || this.wasLeftHandColliding)
			{
				if (this.leftHandSurfaceOverride && this.leftHandSurfaceOverride.disablePushBackEffect)
				{
					vector3 += Vector3.zero;
				}
				else
				{
					vector3 += b;
				}
				this.touchPoints++;
			}
			if (this.isRightHandColliding || this.wasRightHandColliding)
			{
				if (this.rightHandSurfaceOverride && this.rightHandSurfaceOverride.disablePushBackEffect)
				{
					vector3 += Vector3.zero;
				}
				else
				{
					vector3 += b2;
				}
				this.touchPoints++;
			}
			if (this.touchPoints != 0)
			{
				vector3 /= (float)this.touchPoints;
			}
			if (this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.RIGHT || this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.LEFT)
			{
				vector3 += this.movingSurfaceOffset;
			}
			else if (this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.BODY)
			{
				Vector3 b3 = this.lastHeadPosition + this.movingSurfaceOffset - this.headCollider.transform.position;
				vector3 += b3;
			}
			if (!this.MaxSphereSizeForNoOverlap(this.headCollider.radius * 0.9f * this.scale, this.lastHeadPosition, true, out this.maxSphereSize1) && !this.CrazyCheck2(this.headCollider.radius * 0.9f * 0.75f * this.scale, this.lastHeadPosition))
			{
				this.lastHeadPosition = this.lastOpenHeadPosition;
			}
			Vector3 a2;
			float num2;
			if (this.IterativeCollisionSphereCast(this.lastHeadPosition, this.headCollider.radius * 0.9f * this.scale, this.headCollider.transform.position + vector3 - this.lastHeadPosition, Vector3.zero, out a2, false, out num2, out this.junkHit, true))
			{
				vector3 = a2 - this.headCollider.transform.position;
			}
			if (!this.MaxSphereSizeForNoOverlap(this.headCollider.radius * 0.9f * this.scale, this.lastHeadPosition + vector3, true, out this.maxSphereSize1) || !this.CrazyCheck2(this.headCollider.radius * 0.9f * 0.75f * this.scale, this.lastHeadPosition + vector3))
			{
				this.lastHeadPosition = this.lastOpenHeadPosition;
				vector3 = this.lastHeadPosition - this.headCollider.transform.position;
			}
			else if (this.headCollider.radius * 0.9f * 0.825f * this.scale < this.maxSphereSize1)
			{
				this.lastOpenHeadPosition = this.headCollider.transform.position + vector3;
			}
			if (vector3 != Vector3.zero)
			{
				base.transform.position += vector3;
			}
			if (this.lastMovingSurfaceContact != GTPlayer.MovingSurfaceContactPoint.NONE && quaternion != Quaternion.identity && !this.isClimbing && !this.rightHandHolding && !this.leftHandHolding)
			{
				this.RotateWithSurface(quaternion, pivot);
			}
			this.lastHeadPosition = this.headCollider.transform.position;
			this.HandleHandLink();
			this.areBothTouching = ((!this.isLeftHandColliding && !this.wasLeftHandColliding) || (!this.isRightHandColliding && !this.wasRightHandColliding));
			Vector3 vector4 = this.FinalHandPosition(this.leftControllerTransform, this.leftHandOffset, this.GetLastLeftHandPosition(), (float)(this.tempFreezeLeftHandEnableTime - Time.timeAsDouble), boostVector, this.areBothTouching, this.isLeftHandColliding, out this.isLeftHandColliding, this.isLeftHandSliding, out this.isLeftHandSliding, this.leftHandMaterialTouchIndex, out this.leftHandMaterialTouchIndex, this.leftHandSurfaceOverride, out this.leftHandSurfaceOverride, this.leftHandHolding, ref this.leftHandHitInfo);
			this.isLeftHandColliding = (this.isLeftHandColliding && this.controllerState.LeftValid);
			this.isLeftHandSliding = (this.isLeftHandSliding && this.controllerState.LeftValid);
			RaycastHit raycastHit = this.lastHitInfoHand;
			Vector3 vector5 = this.FinalHandPosition(this.rightControllerTransform, this.rightHandOffset, this.GetLastRightHandPosition(), (float)(this.tempFreezeRightHandEnableTime - Time.timeAsDouble), boostVector2, this.areBothTouching, this.isRightHandColliding, out this.isRightHandColliding, this.isRightHandSliding, out this.isRightHandSliding, this.rightHandMaterialTouchIndex, out this.rightHandMaterialTouchIndex, this.rightHandSurfaceOverride, out this.rightHandSurfaceOverride, this.rightHandHolding, ref this.rightHandHitInfo);
			this.isRightHandColliding = (this.isRightHandColliding && this.controllerState.RightValid);
			this.isRightHandSliding = (this.isRightHandSliding && this.controllerState.RightValid);
			Vector3 b4 = this.lastPosition;
			GTPlayer.MovingSurfaceContactPoint movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.NONE;
			int num3 = -1;
			int num4 = -1;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = this.isRightHandColliding && this.IsTouchingMovingSurface(this.GetLastRightHandPosition(), this.lastHitInfoHand, out num3, out flag, out flag2);
			if (flag4 && !flag)
			{
				movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.RIGHT;
				this.lastMovingSurfaceHit = this.lastHitInfoHand;
			}
			else
			{
				bool flag5 = false;
				BuilderPiece builderPiece = flag4 ? this.lastMonkeBlock : null;
				if (this.isLeftHandColliding && this.IsTouchingMovingSurface(this.GetLastLeftHandPosition(), raycastHit, out num4, out flag5, out flag3))
				{
					if (flag5 && flag2 == flag3)
					{
						if (flag && num4.Equals(num3) && (double)Vector3.Dot(raycastHit.point - this.GetLastLeftHandPosition(), this.lastHitInfoHand.point - this.GetLastRightHandPosition()) < 0.3)
						{
							movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.RIGHT;
							this.lastMovingSurfaceHit = this.lastHitInfoHand;
							this.lastMonkeBlock = builderPiece;
						}
					}
					else
					{
						movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.LEFT;
						this.lastMovingSurfaceHit = raycastHit;
					}
				}
			}
			this.StoreVelocities();
			if (this.InWater)
			{
				PlayerGameEvents.PlayerSwam((this.lastPosition - b4).magnitude, this.currentVelocity.magnitude);
			}
			else
			{
				PlayerGameEvents.PlayerMoved((this.lastPosition - b4).magnitude, this.currentVelocity.magnitude);
			}
			this.didAJump = false;
			bool flag6 = this.exitMovingSurface;
			this.exitMovingSurface = false;
			if (this.LeftSlipOverriddenToMax() && this.RightSlipOverriddenToMax())
			{
				this.didAJump = true;
				this.exitMovingSurface = true;
			}
			else if (this.isRightHandSliding || this.isLeftHandSliding)
			{
				this.slideAverageNormal = Vector3.zero;
				this.touchPoints = 0;
				this.averageSlipPercentage = 0f;
				if (this.isLeftHandSliding)
				{
					this.slideAverageNormal += this.leftHandSlideNormal.normalized;
					this.averageSlipPercentage += this.leftHandSlipPercentage;
					this.touchPoints++;
				}
				if (this.isRightHandSliding)
				{
					this.slideAverageNormal += this.rightHandSlideNormal.normalized;
					this.averageSlipPercentage += this.rightHandSlipPercentage;
					this.touchPoints++;
				}
				this.slideAverageNormal = this.slideAverageNormal.normalized;
				this.averageSlipPercentage /= (float)this.touchPoints;
				if (this.touchPoints == 1)
				{
					this.surfaceDirection = (this.isRightHandSliding ? Vector3.ProjectOnPlane(this.rightControllerTransform.forward, this.rightHandSlideNormal) : Vector3.ProjectOnPlane(this.leftControllerTransform.forward, this.leftHandSlideNormal));
					if (Vector3.Dot(this.slideVelocity, this.surfaceDirection) > 0f)
					{
						this.slideVelocity = Vector3.Project(this.slideVelocity, Vector3.Slerp(this.slideVelocity, this.surfaceDirection.normalized * this.slideVelocity.magnitude, this.slideControl));
					}
					else
					{
						this.slideVelocity = Vector3.Project(this.slideVelocity, Vector3.Slerp(this.slideVelocity, -this.surfaceDirection.normalized * this.slideVelocity.magnitude, this.slideControl));
					}
				}
				if (!this.wasLeftHandSliding && !this.wasRightHandSliding)
				{
					this.slideVelocity = ((Vector3.Dot(this.playerRigidBody.velocity, this.slideAverageNormal) <= 0f) ? Vector3.ProjectOnPlane(this.playerRigidBody.velocity, this.slideAverageNormal) : this.playerRigidBody.velocity);
				}
				else
				{
					this.slideVelocity = ((Vector3.Dot(this.slideVelocity, this.slideAverageNormal) <= 0f) ? Vector3.ProjectOnPlane(this.slideVelocity, this.slideAverageNormal) : this.slideVelocity);
				}
				this.slideVelocity = this.slideVelocity.normalized * Mathf.Min(this.slideVelocity.magnitude, Mathf.Max(0.5f, this.averagedVelocity.magnitude * 2f));
				this.playerRigidBody.velocity = Vector3.zero;
			}
			else if (this.isLeftHandColliding || this.isRightHandColliding)
			{
				if (!this.turnedThisFrame)
				{
					this.playerRigidBody.velocity = Vector3.zero;
				}
				else
				{
					this.playerRigidBody.velocity = this.playerRigidBody.velocity.normalized * Mathf.Min(2f, this.playerRigidBody.velocity.magnitude);
				}
			}
			else if (this.wasLeftHandSliding || this.wasRightHandSliding)
			{
				this.playerRigidBody.velocity = ((Vector3.Dot(this.slideVelocity, this.slideAverageNormal) <= 0f) ? Vector3.ProjectOnPlane(this.slideVelocity, this.slideAverageNormal) : this.slideVelocity);
			}
			if ((this.isRightHandColliding || this.isLeftHandColliding) && !this.disableMovement && !this.turnedThisFrame && !this.didAJump)
			{
				if (this.isRightHandSliding || this.isLeftHandSliding)
				{
					if (Vector3.Project(this.averagedVelocity, this.slideAverageNormal).magnitude > this.slideVelocityLimit * this.scale && Vector3.Dot(this.averagedVelocity, this.slideAverageNormal) > 0f && Vector3.Project(this.averagedVelocity, this.slideAverageNormal).magnitude > Vector3.Project(this.slideVelocity, this.slideAverageNormal).magnitude)
					{
						this.isLeftHandSliding = false;
						this.isRightHandSliding = false;
						this.didAJump = true;
						float num5 = this.ApplyNativeScaleAdjustment(Mathf.Min(this.maxJumpSpeed * this.ExtraVelMaxMultiplier(), this.jumpMultiplier * this.ExtraVelMultiplier() * Vector3.Project(this.averagedVelocity, this.slideAverageNormal).magnitude));
						this.playerRigidBody.velocity = num5 * this.slideAverageNormal.normalized + Vector3.ProjectOnPlane(this.slideVelocity, this.slideAverageNormal);
						if (num5 > this.slideVelocityLimit * this.scale * this.exitMovingSurfaceThreshold)
						{
							this.exitMovingSurface = true;
						}
					}
				}
				else if (this.averagedVelocity.magnitude > this.velocityLimit * this.scale)
				{
					float num6 = (this.InWater && this.CurrentWaterVolume != null) ? this.liquidPropertiesList[(int)this.CurrentWaterVolume.LiquidType].surfaceJumpFactor : 1f;
					float num7 = this.ApplyNativeScaleAdjustment(this.enableHoverMode ? Mathf.Min(this.hoverMaxPaddleSpeed, this.averagedVelocity.magnitude) : Mathf.Min(this.maxJumpSpeed * this.ExtraVelMaxMultiplier(), this.jumpMultiplier * this.ExtraVelMultiplier() * num6 * this.averagedVelocity.magnitude));
					Vector3 vector6 = num7 * this.averagedVelocity.normalized;
					this.didAJump = true;
					this.playerRigidBody.velocity = vector6;
					if (this.InWater)
					{
						this.swimmingVelocity += vector6 * this.swimmingParams.underwaterJumpsAsSwimVelocityFactor;
					}
					if (num7 > this.velocityLimit * this.scale * this.exitMovingSurfaceThreshold)
					{
						this.exitMovingSurface = true;
					}
				}
			}
			this.stuckHandsCheckLateUpdate(ref vector4, ref vector5);
			if (this.lastPlatformTouched != null && this.currentPlatform == null)
			{
				if (!this.playerRigidBody.isKinematic)
				{
					this.playerRigidBody.velocity += this.refMovement / this.calcDeltaTime;
				}
				this.refMovement = Vector3.zero;
			}
			if (this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.NONE)
			{
				if (!this.playerRigidBody.isKinematic)
				{
					this.playerRigidBody.velocity += this.lastMovingSurfaceVelocity;
				}
				this.lastMovingSurfaceVelocity = Vector3.zero;
			}
			if (this.enableHoverMode)
			{
				this.HoverboardLateUpdate();
			}
			else
			{
				this.hasHoverPoint = false;
			}
			Vector3 vector7 = Vector3.zero;
			float a3 = 0f;
			float a4 = 0f;
			if (this.bodyInWater)
			{
				Vector3 b5;
				if (this.GetSwimmingVelocityForHand(this.lastLeftHandPosition, vector4, this.leftControllerTransform.right, this.calcDeltaTime, ref this.leftHandWaterVolume, ref this.leftHandWaterSurface, out b5) && !this.turnedThisFrame)
				{
					a3 = Mathf.InverseLerp(0f, 0.2f, b5.magnitude) * this.swimmingParams.swimmingHapticsStrength;
					vector7 += b5;
				}
				Vector3 b6;
				if (this.GetSwimmingVelocityForHand(this.lastRightHandPosition, vector5, -this.rightControllerTransform.right, this.calcDeltaTime, ref this.rightHandWaterVolume, ref this.rightHandWaterSurface, out b6) && !this.turnedThisFrame)
				{
					a4 = Mathf.InverseLerp(0f, 0.15f, b6.magnitude) * this.swimmingParams.swimmingHapticsStrength;
					vector7 += b6;
				}
			}
			Vector3 vector8 = Vector3.zero;
			Vector3 b7;
			if (this.swimmingParams.allowWaterSurfaceJumps && time - this.lastWaterSurfaceJumpTimeLeft > this.waterSurfaceJumpCooldown && this.CheckWaterSurfaceJump(this.lastLeftHandPosition, vector4, this.leftControllerTransform.right, this.leftHandCenterVelocityTracker.GetAverageVelocity(false, 0.1f, false) * this.scale, this.swimmingParams, this.leftHandWaterVolume, this.leftHandWaterSurface, out b7))
			{
				if (time - this.lastWaterSurfaceJumpTimeRight > this.waterSurfaceJumpCooldown)
				{
					vector8 += b7;
				}
				this.lastWaterSurfaceJumpTimeLeft = Time.time;
				GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
			}
			Vector3 b8;
			if (this.swimmingParams.allowWaterSurfaceJumps && time - this.lastWaterSurfaceJumpTimeRight > this.waterSurfaceJumpCooldown && this.CheckWaterSurfaceJump(this.lastRightHandPosition, vector5, -this.rightControllerTransform.right, this.rightHandCenterVelocityTracker.GetAverageVelocity(false, 0.1f, false) * this.scale, this.swimmingParams, this.rightHandWaterVolume, this.rightHandWaterSurface, out b8))
			{
				if (time - this.lastWaterSurfaceJumpTimeLeft > this.waterSurfaceJumpCooldown)
				{
					vector8 += b8;
				}
				this.lastWaterSurfaceJumpTimeRight = Time.time;
				GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
			}
			vector8 = Vector3.ClampMagnitude(vector8, this.swimmingParams.waterSurfaceJumpMaxSpeed * this.scale);
			float num8 = Mathf.Max(a3, this.leftHandNonDiveHapticsAmount);
			if (num8 > 0.001f && time - this.lastWaterSurfaceJumpTimeLeft > GorillaTagger.Instance.tapHapticDuration)
			{
				GorillaTagger.Instance.DoVibration(XRNode.LeftHand, num8, this.calcDeltaTime);
			}
			float num9 = Mathf.Max(a4, this.rightHandNonDiveHapticsAmount);
			if (num9 > 0.001f && time - this.lastWaterSurfaceJumpTimeRight > GorillaTagger.Instance.tapHapticDuration)
			{
				GorillaTagger.Instance.DoVibration(XRNode.RightHand, num9, this.calcDeltaTime);
			}
			if (!this.disableMovement)
			{
				this.swimmingVelocity += vector7;
				if (!this.playerRigidBody.isKinematic)
				{
					this.playerRigidBody.velocity += vector7 + vector8;
				}
			}
			else
			{
				this.swimmingVelocity = Vector3.zero;
			}
			if (GorillaGameManager.instance is GorillaFreezeTagManager)
			{
				if (!this.IsFrozen || !this.primaryButtonPressed)
				{
					this.IsBodySliding = false;
					this.lastSlopeDirection = Vector3.zero;
					if (this.bodyTouchedSurfaces.Count > 0)
					{
						foreach (KeyValuePair<GameObject, PhysicMaterial> keyValuePair in this.bodyTouchedSurfaces)
						{
							MeshCollider meshCollider;
							if (keyValuePair.Key.TryGetComponent<MeshCollider>(out meshCollider))
							{
								meshCollider.material = keyValuePair.Value;
							}
						}
						this.bodyTouchedSurfaces.Clear();
					}
				}
				else if (this.BodyOnGround && this.primaryButtonPressed)
				{
					float y = this.bodyInitialHeight / 2f - this.bodyInitialRadius;
					RaycastHit raycastHit2;
					if (Physics.SphereCast(this.bodyCollider.transform.position - new Vector3(0f, y, 0f), this.bodyInitialRadius - 0.01f, Vector3.down, out raycastHit2, 1f, ~LayerMask.GetMask(new string[]
					{
						"Gorilla Body Collider",
						"GorillaInteractable"
					}), QueryTriggerInteraction.Ignore))
					{
						this.IsBodySliding = true;
						MeshCollider meshCollider2;
						if (!this.bodyTouchedSurfaces.ContainsKey(raycastHit2.transform.gameObject) && raycastHit2.transform.gameObject.TryGetComponent<MeshCollider>(out meshCollider2))
						{
							this.bodyTouchedSurfaces.Add(raycastHit2.transform.gameObject, meshCollider2.material);
							raycastHit2.transform.gameObject.GetComponent<MeshCollider>().material = this.slipperyMaterial;
						}
					}
				}
				else
				{
					this.IsBodySliding = false;
					this.lastSlopeDirection = Vector3.zero;
				}
			}
			else
			{
				this.IsBodySliding = false;
				if (this.bodyTouchedSurfaces.Count > 0)
				{
					foreach (KeyValuePair<GameObject, PhysicMaterial> keyValuePair2 in this.bodyTouchedSurfaces)
					{
						MeshCollider meshCollider3;
						if (keyValuePair2.Key.TryGetComponent<MeshCollider>(out meshCollider3))
						{
							meshCollider3.material = keyValuePair2.Value;
						}
					}
					this.bodyTouchedSurfaces.Clear();
				}
			}
			this.leftHandFollower.position = vector4;
			this.rightHandFollower.position = vector5;
			this.leftHandFollower.rotation = this.leftControllerTransform.rotation * this.leftHandRotOffset;
			this.rightHandFollower.rotation = this.rightControllerTransform.rotation * this.rightHandRotOffset;
			if (this.tempFreezeLeftHandEnableTime - Time.timeAsDouble > 0.0)
			{
				this.leftHandFollower.rotation = this.lastLeftHandRotation;
			}
			if (this.tempFreezeRightHandEnableTime - Time.timeAsDouble > 0.0)
			{
				this.rightHandFollower.rotation = this.lastRightHandRotation;
			}
			this.wasLeftHandColliding = this.isLeftHandColliding;
			this.wasRightHandColliding = this.isRightHandColliding;
			this.wasLeftHandSliding = this.isLeftHandSliding;
			this.wasRightHandSliding = this.isRightHandSliding;
			if ((this.isLeftHandColliding && !this.isLeftHandSliding) || (this.isRightHandColliding && !this.isRightHandSliding))
			{
				this.lastTouchedGroundTimestamp = Time.time;
			}
			if (PhotonNetwork.InRoom)
			{
				if (this.IsGroundedHand)
				{
					this.LastHandTouchedGroundAtNetworkTime = (float)PhotonNetwork.Time;
					this.LastTouchedGroundAtNetworkTime = (float)PhotonNetwork.Time;
				}
				else if (this.IsGroundedButt)
				{
					this.LastTouchedGroundAtNetworkTime = (float)PhotonNetwork.Time;
				}
			}
			else
			{
				this.LastHandTouchedGroundAtNetworkTime = 0f;
				this.LastTouchedGroundAtNetworkTime = 0f;
			}
			this.degreesTurnedThisFrame = 0f;
			this.lastPlatformTouched = this.currentPlatform;
			this.currentPlatform = null;
			this.lastMovingSurfaceVelocity = vector;
			this.lastLeftHandPosition = vector4;
			this.lastRightHandPosition = vector5;
			this.lastLeftHandRotation = this.leftHandFollower.rotation;
			this.lastRightHandRotation = this.rightHandFollower.rotation;
			Vector3 vector9;
			if (GTPlayer.ComputeLocalHitPoint(this.lastHitInfoHand, out vector9))
			{
				this.lastFrameHasValidTouchPos = true;
				this.lastFrameTouchPosLocal = vector9;
				this.lastFrameTouchPosWorld = this.lastHitInfoHand.point;
			}
			else
			{
				this.lastFrameHasValidTouchPos = false;
				this.lastFrameTouchPosLocal = Vector3.zero;
				this.lastFrameTouchPosWorld = Vector3.zero;
			}
			this.lastRigidbodyPosition = this.playerRigidBody.transform.position;
			RaycastHit raycastHit3 = this.emptyHit;
			this.BodyCollider();
			if (this.bodyHitInfo.collider != null)
			{
				this.wasBodyOnGround = true;
				raycastHit3 = this.bodyHitInfo;
			}
			else if (movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.NONE && this.bodyCollider.gameObject.activeSelf)
			{
				bool flag7 = false;
				this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
				Vector3 origin = this.PositionWithOffset(this.headCollider.transform, this.bodyOffset) + (this.bodyInitialHeight * this.scale - this.bodyMaxRadius) * Vector3.down;
				this.bufferCount = Physics.SphereCastNonAlloc(origin, this.bodyMaxRadius, Vector3.down, this.rayCastNonAllocColliders, this.minimumRaycastDistance * this.scale, this.locomotionEnabledLayers.value);
				if (this.bufferCount > 0)
				{
					this.tempHitInfo = this.rayCastNonAllocColliders[0];
					for (int i = 0; i < this.bufferCount; i++)
					{
						if (this.tempHitInfo.distance > 0f && (!flag7 || this.rayCastNonAllocColliders[i].distance < this.tempHitInfo.distance))
						{
							flag7 = true;
							raycastHit3 = this.rayCastNonAllocColliders[i];
						}
					}
				}
				this.wasBodyOnGround = flag7;
			}
			int num10 = -1;
			bool flag8 = false;
			bool flag9;
			if (this.wasBodyOnGround && movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.NONE && this.IsTouchingMovingSurface(this.PositionWithOffset(this.headCollider.transform, this.bodyOffset), raycastHit3, out num10, out flag9, out flag8) && !flag9)
			{
				movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.BODY;
				this.lastMovingSurfaceHit = raycastHit3;
			}
			Vector3 vector10;
			if (movingSurfaceContactPoint != GTPlayer.MovingSurfaceContactPoint.NONE && GTPlayer.ComputeLocalHitPoint(this.lastMovingSurfaceHit, out vector10))
			{
				this.lastMovingSurfaceTouchLocal = vector10;
				this.lastMovingSurfaceTouchWorld = this.lastMovingSurfaceHit.point;
				this.lastMovingSurfaceRot = this.lastMovingSurfaceHit.collider.transform.rotation;
				this.lastAttachedToMovingSurfaceFrame = Time.frameCount;
			}
			else
			{
				movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.NONE;
				this.lastMovingSurfaceTouchLocal = Vector3.zero;
				this.lastMovingSurfaceTouchWorld = Vector3.zero;
				this.lastMovingSurfaceRot = Quaternion.identity;
			}
			Vector3 position2 = this.lastMovingSurfaceTouchWorld;
			int num11 = -1;
			bool flag10 = false;
			switch (movingSurfaceContactPoint)
			{
			case GTPlayer.MovingSurfaceContactPoint.NONE:
				if (flag6)
				{
					this.exitMovingSurface = true;
				}
				num11 = -1;
				break;
			case GTPlayer.MovingSurfaceContactPoint.RIGHT:
				num11 = num3;
				flag10 = flag2;
				position2 = GorillaTagger.Instance.offlineVRRig.rightHandTransform.position;
				break;
			case GTPlayer.MovingSurfaceContactPoint.LEFT:
				num11 = num4;
				flag10 = flag3;
				position2 = GorillaTagger.Instance.offlineVRRig.leftHandTransform.position;
				break;
			case GTPlayer.MovingSurfaceContactPoint.BODY:
				num11 = num10;
				flag10 = flag8;
				position2 = GorillaTagger.Instance.offlineVRRig.bodyTransform.position;
				break;
			}
			if (!flag10)
			{
				this.lastMonkeBlock = null;
			}
			if (num11 != this.lastMovingSurfaceID || this.lastMovingSurfaceContact != movingSurfaceContactPoint || flag10 != this.wasMovingSurfaceMonkeBlock)
			{
				if (num11 == -1)
				{
					if (Time.frameCount - this.lastAttachedToMovingSurfaceFrame > 3)
					{
						VRRig.DetachLocalPlayerFromMovingSurface();
						this.lastMovingSurfaceID = -1;
					}
				}
				else if (flag10)
				{
					if (this.lastMonkeBlock != null)
					{
						VRRig.AttachLocalPlayerToMovingSurface(num11, movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.LEFT, movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.BODY, this.lastMonkeBlock.transform.InverseTransformPoint(position2), flag10);
						this.lastMovingSurfaceID = num11;
					}
					else
					{
						VRRig.DetachLocalPlayerFromMovingSurface();
						this.lastMovingSurfaceID = -1;
					}
				}
				else if (MovingSurfaceManager.instance != null)
				{
					MovingSurface movingSurface;
					if (MovingSurfaceManager.instance.TryGetMovingSurface(num11, out movingSurface))
					{
						VRRig.AttachLocalPlayerToMovingSurface(num11, movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.LEFT, movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.BODY, movingSurface.transform.InverseTransformPoint(position2), flag10);
						this.lastMovingSurfaceID = num11;
					}
					else
					{
						VRRig.DetachLocalPlayerFromMovingSurface();
						this.lastMovingSurfaceID = -1;
					}
				}
				else
				{
					VRRig.DetachLocalPlayerFromMovingSurface();
					this.lastMovingSurfaceID = -1;
				}
			}
			if (this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.NONE && movingSurfaceContactPoint != GTPlayer.MovingSurfaceContactPoint.NONE)
			{
				this.SetPlayerVelocity(Vector3.zero);
			}
			this.lastMovingSurfaceContact = movingSurfaceContactPoint;
			this.wasMovingSurfaceMonkeBlock = flag10;
			if (this.activeSizeChangerSettings != null)
			{
				if (this.activeSizeChangerSettings.ExpireOnDistance > 0f && Vector3.Distance(base.transform.position, this.activeSizeChangerSettings.WorldPosition) > this.activeSizeChangerSettings.ExpireOnDistance)
				{
					this.SetNativeScale(null);
				}
				if (this.activeSizeChangerSettings.ExpireAfterSeconds > 0f && Time.time - this.activeSizeChangerSettings.ActivationTime > this.activeSizeChangerSettings.ExpireAfterSeconds)
				{
					this.SetNativeScale(null);
				}
			}
			HandLink grabbedLink = VRRig.LocalRig.leftHandLink.grabbedLink;
			if (grabbedLink != null)
			{
				double time2 = PhotonNetwork.Time;
				float lastHandTouchedGroundAtNetworkTime = this.LastHandTouchedGroundAtNetworkTime;
				double time3 = PhotonNetwork.Time;
				float lastHandTouchedGroundAtNetworkTime2 = grabbedLink.myRig.LastHandTouchedGroundAtNetworkTime;
			}
		}

		// Token: 0x06005934 RID: 22836 RVA: 0x001BE384 File Offset: 0x001BC584
		private float ApplyNativeScaleAdjustment(float adjustedMagnitude)
		{
			if (this.nativeScale > 0f && this.nativeScale != 1f)
			{
				return adjustedMagnitude *= this.nativeScaleMagnitudeAdjustmentFactor.Evaluate(this.nativeScale);
			}
			return adjustedMagnitude;
		}

		// Token: 0x06005935 RID: 22837 RVA: 0x001BE3B8 File Offset: 0x001BC5B8
		private float RotateWithSurface(Quaternion rotationDelta, Vector3 pivot)
		{
			Quaternion quaternion;
			Quaternion quaternion2;
			QuaternionUtil.DecomposeSwingTwist(rotationDelta, Vector3.up, out quaternion, out quaternion2);
			float num = quaternion2.eulerAngles.y;
			if (num > 270f)
			{
				num -= 360f;
			}
			else if (num > 90f)
			{
				num -= 180f;
			}
			if (Mathf.Abs(num) < 90f * this.calcDeltaTime)
			{
				this.turnParent.transform.RotateAround(pivot, base.transform.up, num);
				return num;
			}
			return 0f;
		}

		// Token: 0x06005936 RID: 22838 RVA: 0x001BE43C File Offset: 0x001BC63C
		private void stuckHandsCheckFixedUpdate()
		{
			this.stuckLeft = (!this.controllerState.LeftValid || (this.isLeftHandColliding && (this.GetCurrentLeftHandPosition() - this.GetLastLeftHandPosition()).magnitude > this.unStickDistance * this.scale && !Physics.Raycast(this.headCollider.transform.position, (this.GetCurrentLeftHandPosition() - this.headCollider.transform.position).normalized, (this.GetCurrentLeftHandPosition() - this.headCollider.transform.position).magnitude, this.locomotionEnabledLayers.value)));
			this.stuckRight = (!this.controllerState.RightValid || (this.isRightHandColliding && (this.GetCurrentRightHandPosition() - this.GetLastRightHandPosition()).magnitude > this.unStickDistance * this.scale && !Physics.Raycast(this.headCollider.transform.position, (this.GetCurrentRightHandPosition() - this.headCollider.transform.position).normalized, (this.GetCurrentRightHandPosition() - this.headCollider.transform.position).magnitude, this.locomotionEnabledLayers.value)));
		}

		// Token: 0x06005937 RID: 22839 RVA: 0x001BE5BC File Offset: 0x001BC7BC
		private void stuckHandsCheckLateUpdate(ref Vector3 finalLeftHandPosition, ref Vector3 finalRightHandPosition)
		{
			if (this.stuckLeft)
			{
				finalLeftHandPosition = this.GetCurrentLeftHandPosition();
				this.stuckLeft = (this.isLeftHandColliding = false);
			}
			if (this.stuckRight)
			{
				finalRightHandPosition = this.GetCurrentRightHandPosition();
				this.stuckRight = (this.isRightHandColliding = false);
			}
		}

		// Token: 0x06005938 RID: 22840 RVA: 0x001BE614 File Offset: 0x001BC814
		private void handleClimbing(float deltaTime)
		{
			if (this.isClimbing && (this.inOverlay || this.climbHelper == null || this.currentClimbable == null || !this.currentClimbable.isActiveAndEnabled))
			{
				this.EndClimbing(this.currentClimber, false, false);
			}
			Vector3 vector = Vector3.zero;
			if (this.isClimbing && (this.currentClimber.transform.position - this.climbHelper.position).magnitude > 1f)
			{
				this.EndClimbing(this.currentClimber, false, false);
			}
			if (this.isClimbing)
			{
				this.playerRigidBody.velocity = Vector3.zero;
				this.climbHelper.localPosition = Vector3.MoveTowards(this.climbHelper.localPosition, this.climbHelperTargetPos, deltaTime * 12f);
				vector = this.currentClimber.transform.position - this.climbHelper.position;
				vector = ((vector.sqrMagnitude > this.maxArmLength * this.maxArmLength) ? (vector.normalized * this.maxArmLength) : vector);
				if (this.isClimbableMoving)
				{
					Quaternion rotationDelta = this.currentClimbable.transform.rotation * Quaternion.Inverse(this.lastClimbableRotation);
					this.RotateWithSurface(rotationDelta, this.currentClimber.handRoot.position);
					this.lastClimbableRotation = this.currentClimbable.transform.rotation;
				}
				this.playerRigidBody.MovePosition(this.playerRigidBody.position - vector);
				if (this.currentSwing)
				{
					this.currentSwing.lastGrabTime = Time.time;
				}
			}
		}

		// Token: 0x06005939 RID: 22841 RVA: 0x001BE7D4 File Offset: 0x001BC9D4
		public HandLinkAuthorityStatus GetSelfHandLinkAuthority()
		{
			int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
			if (this.IsGroundedHand)
			{
				return new HandLinkAuthorityStatus(HandLinkAuthorityType.HandGrounded);
			}
			if ((double)(this.LastHandTouchedGroundAtNetworkTime + 1f) > PhotonNetwork.Time)
			{
				return new HandLinkAuthorityStatus(HandLinkAuthorityType.ResidualHandGrounded, this.LastHandTouchedGroundAtNetworkTime, actorNumber);
			}
			if (this.IsGroundedButt)
			{
				return new HandLinkAuthorityStatus(HandLinkAuthorityType.ButtGrounded);
			}
			return new HandLinkAuthorityStatus(HandLinkAuthorityType.None, this.LastTouchedGroundAtNetworkTime, actorNumber);
		}

		// Token: 0x0600593A RID: 22842 RVA: 0x001BE83C File Offset: 0x001BCA3C
		private void HandleHandLink()
		{
			HandLink leftHandLink = VRRig.LocalRig.leftHandLink;
			HandLink rightHandLink = VRRig.LocalRig.rightHandLink;
			bool flag = leftHandLink.grabbedLink != null;
			bool flag2 = rightHandLink.grabbedLink != null;
			if (!flag && !flag2)
			{
				return;
			}
			HandLinkAuthorityStatus selfHandLinkAuthority = this.GetSelfHandLinkAuthority();
			int num = -1;
			HandLinkAuthorityStatus chainAuthority = new HandLinkAuthorityStatus(HandLinkAuthorityType.None);
			if (flag)
			{
				chainAuthority = leftHandLink.GetChainAuthority(out num);
			}
			int num2 = -1;
			HandLinkAuthorityStatus chainAuthority2 = new HandLinkAuthorityStatus(HandLinkAuthorityType.None);
			if (flag2)
			{
				chainAuthority2 = rightHandLink.GetChainAuthority(out num2);
			}
			if (flag && flag2)
			{
				if (leftHandLink.grabbedPlayer == rightHandLink.grabbedPlayer)
				{
					switch (selfHandLinkAuthority.CompareTo(chainAuthority))
					{
					case -1:
						this.HandLink_PositionChild_LocalPlayer(leftHandLink, rightHandLink);
						return;
					case 0:
						this.HandLink_PositionBoth_BothHands(leftHandLink, rightHandLink);
						return;
					case 1:
						this.HandLink_PositionChild_RemotePlayer_BothHands(leftHandLink, rightHandLink);
						return;
					default:
						return;
					}
				}
				else
				{
					int num3 = selfHandLinkAuthority.CompareTo(chainAuthority);
					int num4 = selfHandLinkAuthority.CompareTo(chainAuthority2);
					switch (num3 * 3 + num4)
					{
					case -3:
					case -2:
						this.HandLink_PositionChild_LocalPlayer(leftHandLink);
						this.HandLink_PositionChild_RemotePlayer(rightHandLink);
						return;
					case -1:
					case 2:
						this.HandLink_PositionChild_LocalPlayer(rightHandLink);
						this.HandLink_PositionChild_RemotePlayer(leftHandLink);
						return;
					case 0:
						this.HandLink_PositionTriple(leftHandLink, rightHandLink);
						return;
					case 1:
						this.HandLink_PositionBoth(leftHandLink);
						this.HandLink_PositionChild_RemotePlayer(rightHandLink);
						return;
					case 3:
						this.HandLink_PositionBoth(rightHandLink);
						this.HandLink_PositionChild_RemotePlayer(leftHandLink);
						return;
					case 4:
						this.HandLink_PositionChild_RemotePlayer(leftHandLink);
						this.HandLink_PositionChild_RemotePlayer(rightHandLink);
						return;
					}
					switch (chainAuthority.CompareTo(chainAuthority2))
					{
					case -1:
						this.HandLink_PositionChild_LocalPlayer(rightHandLink);
						this.HandLink_PositionChild_RemotePlayer(leftHandLink);
						return;
					case 0:
						if (num > num2)
						{
							this.HandLink_PositionChild_LocalPlayer(rightHandLink);
							this.HandLink_PositionChild_RemotePlayer(leftHandLink);
							return;
						}
						if (num < num2)
						{
							this.HandLink_PositionChild_LocalPlayer(leftHandLink);
							this.HandLink_PositionChild_RemotePlayer(rightHandLink);
							return;
						}
						this.HandLink_PositionChild_LocalPlayer(leftHandLink, rightHandLink);
						return;
					case 1:
						this.HandLink_PositionChild_LocalPlayer(leftHandLink);
						this.HandLink_PositionChild_RemotePlayer(rightHandLink);
						return;
					default:
						return;
					}
				}
			}
			else if (flag)
			{
				switch (selfHandLinkAuthority.CompareTo(chainAuthority))
				{
				case -1:
					this.HandLink_PositionChild_LocalPlayer(leftHandLink);
					return;
				case 0:
					this.HandLink_PositionBoth(leftHandLink);
					return;
				case 1:
					this.HandLink_PositionChild_RemotePlayer(leftHandLink);
					return;
				default:
					return;
				}
			}
			else
			{
				switch (selfHandLinkAuthority.CompareTo(chainAuthority2))
				{
				case -1:
					this.HandLink_PositionChild_LocalPlayer(rightHandLink);
					return;
				case 0:
					this.HandLink_PositionBoth(rightHandLink);
					return;
				case 1:
					this.HandLink_PositionChild_RemotePlayer(rightHandLink);
					return;
				default:
					return;
				}
			}
		}

		// Token: 0x0600593B RID: 22843 RVA: 0x001BEA90 File Offset: 0x001BCC90
		private void HandLink_PositionTriple(HandLink linkA, HandLink linkB)
		{
			Vector3 a = linkA.transform.position - linkA.grabbedLink.transform.position;
			Vector3 vector = linkB.transform.position - linkB.grabbedLink.transform.position;
			Vector3 b = (a + vector) * 0.33f;
			bool flag;
			bool flag2;
			linkA.grabbedLink.myRig.TrySweptOffsetMove(a - b, out flag, out flag2);
			bool flag3;
			bool flag4;
			linkB.grabbedLink.myRig.TrySweptOffsetMove(vector - b, out flag3, out flag4);
			this.playerRigidBody.MovePosition(this.playerRigidBody.position - b);
			this.playerRigidBody.velocity = Vector3.zero;
		}

		// Token: 0x0600593C RID: 22844 RVA: 0x001BEB54 File Offset: 0x001BCD54
		private void HandLink_PositionBoth(HandLink link)
		{
			Vector3 vector = (link.grabbedLink.transform.position - link.transform.position) * 0.5f;
			bool flag;
			bool flag2;
			link.grabbedLink.myRig.TrySweptOffsetMove(-vector, out flag, out flag2);
			if (flag || flag2)
			{
				this.HandLink_PositionChild_LocalPlayer(link);
			}
			else
			{
				this.playerRigidBody.transform.position += vector;
			}
			this.playerRigidBody.velocity = Vector3.zero;
		}

		// Token: 0x0600593D RID: 22845 RVA: 0x001BEBE0 File Offset: 0x001BCDE0
		private void HandLink_PositionBoth_BothHands(HandLink link1, HandLink link2)
		{
			Vector3 a = (link1.grabbedLink.transform.position - link1.transform.position) * 0.5f;
			Vector3 b = (link2.grabbedLink.transform.position - link2.transform.position) * 0.5f;
			Vector3 vector = (a + b) * 0.5f;
			bool flag;
			bool flag2;
			link1.grabbedLink.myRig.TrySweptOffsetMove(-vector, out flag, out flag2);
			if (flag || flag2)
			{
				this.HandLink_PositionChild_LocalPlayer(link1, link2);
			}
			else
			{
				this.playerRigidBody.transform.position += vector;
			}
			this.playerRigidBody.velocity = Vector3.zero;
		}

		// Token: 0x0600593E RID: 22846 RVA: 0x001BECA8 File Offset: 0x001BCEA8
		private void HandLink_PositionChild_LocalPlayer(HandLink parentLink)
		{
			Vector3 b = parentLink.grabbedLink.transform.position - parentLink.transform.position;
			this.playerRigidBody.transform.position += b;
			this.playerRigidBody.velocity = Vector3.zero;
		}

		// Token: 0x0600593F RID: 22847 RVA: 0x001BED04 File Offset: 0x001BCF04
		private void HandLink_PositionChild_LocalPlayer(HandLink linkA, HandLink linkB)
		{
			Vector3 a = linkA.grabbedLink.transform.position - linkA.transform.position;
			Vector3 b = linkB.grabbedLink.transform.position - linkB.transform.position;
			this.playerRigidBody.transform.position += (a + b) * 0.5f;
			this.playerRigidBody.velocity = Vector3.zero;
		}

		// Token: 0x06005940 RID: 22848 RVA: 0x001BED90 File Offset: 0x001BCF90
		private void HandLink_PositionChild_RemotePlayer(HandLink childLink)
		{
			Vector3 movement = childLink.transform.position - childLink.grabbedLink.transform.position;
			bool flag;
			bool flag2;
			childLink.grabbedLink.myRig.TrySweptOffsetMove(movement, out flag, out flag2);
			if (flag || flag2)
			{
				this.HandLink_PositionChild_LocalPlayer(childLink);
			}
		}

		// Token: 0x06005941 RID: 22849 RVA: 0x001BEDE0 File Offset: 0x001BCFE0
		private void HandLink_PositionChild_RemotePlayer_BothHands(HandLink childLink1, HandLink childLink2)
		{
			Vector3 a = childLink1.transform.position - childLink1.grabbedLink.transform.position;
			Vector3 b = childLink2.transform.position - childLink2.grabbedLink.transform.position;
			Vector3 movement = (a + b) * 0.5f;
			bool flag;
			bool flag2;
			childLink1.grabbedLink.myRig.TrySweptOffsetMove(movement, out flag, out flag2);
			if (flag || flag2)
			{
				this.HandLink_PositionChild_LocalPlayer(childLink1, childLink2);
			}
		}

		// Token: 0x06005942 RID: 22850 RVA: 0x001BEE64 File Offset: 0x001BD064
		private void FirstHandIteration(Transform handTransform, Vector3 handOffset, Vector3 lastHandPosition, Vector3 boostVector, bool wasHandSlide, bool wasHandTouching, bool fullSlideOverride, out Vector3 pushDisplacement, out float handSlipPercentage, out bool handSlide, ref Vector3 slideNormal, out bool handColliding, out int materialTouchIndex, out GorillaSurfaceOverride touchedOverride, bool skipCollisionChecks, bool hitMovingSurface)
		{
			Vector3 vector = this.GetCurrentHandPosition(handTransform, handOffset) + this.movingSurfaceOffset;
			Vector3 a = vector - lastHandPosition;
			if (!this.didAJump && wasHandSlide && Vector3.Dot(slideNormal, Vector3.up) > 0f)
			{
				a += Vector3.Project(-this.slideAverageNormal * this.stickDepth * this.scale, Vector3.down);
			}
			float num = this.minimumRaycastDistance * this.scale;
			if (this.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
			{
				num = (this.minimumRaycastDistance + VRRig.LocalRig.iceCubeRight.transform.localScale.y / 2f) * this.scale;
			}
			Vector3 vector2 = Vector3.zero;
			if (hitMovingSurface && !this.exitMovingSurface)
			{
				vector2 = Vector3.Project(-this.lastMovingSurfaceHit.normal * (this.stickDepth * this.scale), Vector3.down);
				if (this.scale < 0.5f)
				{
					Vector3 normalized = this.MovingSurfaceMovement().normalized;
					if (normalized != Vector3.zero)
					{
						float num2 = Vector3.Dot(Vector3.up, normalized);
						if ((double)num2 > 0.9 || (double)num2 < -0.9)
						{
							vector2 *= 6f;
							num *= 1.1f;
						}
					}
				}
			}
			Vector3 a2;
			float num3;
			if (this.IterativeCollisionSphereCast(lastHandPosition, num, a + vector2, boostVector, out a2, true, out num3, out this.tempHitInfo, fullSlideOverride) && !skipCollisionChecks && !this.InReportMenu)
			{
				if (wasHandTouching && num3 <= this.defaultSlideFactor && !boostVector.IsLongerThan(0f))
				{
					pushDisplacement = lastHandPosition - vector;
				}
				else
				{
					pushDisplacement = a2 - vector;
				}
				handSlipPercentage = num3;
				handSlide = (num3 > this.iceThreshold);
				slideNormal = this.tempHitInfo.normal;
				handColliding = true;
				materialTouchIndex = this.currentMaterialIndex;
				touchedOverride = this.currentOverride;
				this.lastHitInfoHand = this.tempHitInfo;
				return;
			}
			pushDisplacement = Vector3.zero;
			handSlipPercentage = 0f;
			handSlide = false;
			slideNormal = Vector3.up;
			handColliding = false;
			materialTouchIndex = 0;
			touchedOverride = null;
		}

		// Token: 0x06005943 RID: 22851 RVA: 0x001BF0CC File Offset: 0x001BD2CC
		private Vector3 FinalHandPosition(Transform handTransform, Vector3 handOffset, Vector3 lastHandPosition, float freezeTimeRemaining, Vector3 boostVector, bool bothTouching, bool isHandTouching, out bool handColliding, bool isHandSlide, out bool handSlide, int currentMaterialTouchIndex, out int materialTouchIndex, GorillaSurfaceOverride currentSurface, out GorillaSurfaceOverride touchedOverride, bool skipCollisionChecks, ref RaycastHit hitInfoCopy)
		{
			handColliding = isHandTouching;
			handSlide = isHandSlide;
			materialTouchIndex = currentMaterialTouchIndex;
			touchedOverride = currentSurface;
			if (freezeTimeRemaining > 0f)
			{
				return lastHandPosition;
			}
			Vector3 movementVector = this.GetCurrentHandPosition(handTransform, handOffset) - lastHandPosition;
			float sphereRadius = this.minimumRaycastDistance * this.scale;
			if (this.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
			{
				sphereRadius = (this.minimumRaycastDistance + VRRig.LocalRig.iceCubeRight.transform.localScale.y / 2f) * this.scale;
			}
			Vector3 result;
			float num;
			if (this.IterativeCollisionSphereCast(lastHandPosition, sphereRadius, movementVector, boostVector, out result, bothTouching, out num, out this.junkHit, false) && !skipCollisionChecks)
			{
				handColliding = true;
				handSlide = (num > this.iceThreshold);
				materialTouchIndex = this.currentMaterialIndex;
				touchedOverride = this.currentOverride;
				this.lastHitInfoHand = this.junkHit;
				hitInfoCopy = this.junkHit;
				return result;
			}
			return this.GetCurrentHandPosition(handTransform, handOffset);
		}

		// Token: 0x06005944 RID: 22852 RVA: 0x001BF1BC File Offset: 0x001BD3BC
		private bool IterativeCollisionSphereCast(Vector3 startPosition, float sphereRadius, Vector3 movementVector, Vector3 boostVector, out Vector3 endPosition, bool singleHand, out float slipPercentage, out RaycastHit iterativeHitInfo, bool fullSlide)
		{
			slipPercentage = this.defaultSlideFactor;
			if (!this.CollisionsSphereCast(startPosition, sphereRadius, movementVector, out endPosition, out this.tempIterativeHit))
			{
				iterativeHitInfo = this.tempIterativeHit;
				endPosition = Vector3.zero;
				return false;
			}
			this.firstPosition = endPosition;
			iterativeHitInfo = this.tempIterativeHit;
			this.slideFactor = this.GetSlidePercentage(iterativeHitInfo);
			slipPercentage = ((this.slideFactor != this.defaultSlideFactor) ? this.slideFactor : ((!singleHand) ? this.defaultSlideFactor : 0.001f));
			if (fullSlide)
			{
				slipPercentage = 1f;
			}
			this.movementToProjectedAboveCollisionPlane = Vector3.ProjectOnPlane(startPosition + movementVector - this.firstPosition, iterativeHitInfo.normal) * slipPercentage;
			Vector3 vector = Vector3.zero;
			if (boostVector.IsLongerThan(0f))
			{
				vector = Vector3.ProjectOnPlane(boostVector, iterativeHitInfo.normal);
				this.movementToProjectedAboveCollisionPlane += vector;
				this.CollisionsSphereCast(this.firstPosition, sphereRadius, vector, out endPosition, out this.tempIterativeHit);
				this.firstPosition = endPosition;
			}
			if (this.CollisionsSphereCast(this.firstPosition, sphereRadius, this.movementToProjectedAboveCollisionPlane, out endPosition, out this.tempIterativeHit))
			{
				iterativeHitInfo = this.tempIterativeHit;
				return true;
			}
			if (this.CollisionsSphereCast(this.movementToProjectedAboveCollisionPlane + this.firstPosition, sphereRadius, startPosition + movementVector + vector - (this.movementToProjectedAboveCollisionPlane + this.firstPosition), out endPosition, out this.tempIterativeHit))
			{
				iterativeHitInfo = this.tempIterativeHit;
				return true;
			}
			endPosition = Vector3.zero;
			return false;
		}

		// Token: 0x06005945 RID: 22853 RVA: 0x001BF378 File Offset: 0x001BD578
		private bool CollisionsSphereCast(Vector3 startPosition, float sphereRadius, Vector3 movementVector, out Vector3 finalPosition, out RaycastHit collisionsHitInfo)
		{
			this.MaxSphereSizeForNoOverlap(sphereRadius, startPosition, false, out this.maxSphereSize1);
			bool flag = false;
			this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
			this.bufferCount = Physics.SphereCastNonAlloc(startPosition, this.maxSphereSize1, movementVector.normalized, this.rayCastNonAllocColliders, movementVector.magnitude, this.locomotionEnabledLayers.value);
			if (this.bufferCount > 0)
			{
				this.tempHitInfo = this.rayCastNonAllocColliders[0];
				for (int i = 0; i < this.bufferCount; i++)
				{
					if (this.tempHitInfo.distance > 0f && (!flag || this.rayCastNonAllocColliders[i].distance < this.tempHitInfo.distance))
					{
						flag = true;
						this.tempHitInfo = this.rayCastNonAllocColliders[i];
					}
				}
			}
			if (flag)
			{
				collisionsHitInfo = this.tempHitInfo;
				finalPosition = collisionsHitInfo.point + collisionsHitInfo.normal * sphereRadius;
				this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
				this.bufferCount = Physics.RaycastNonAlloc(startPosition, (finalPosition - startPosition).normalized, this.rayCastNonAllocColliders, (finalPosition - startPosition).magnitude, this.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore);
				List<RaycastHit> list = this.rayCastNonAllocColliders.Take(this.bufferCount).ToList<RaycastHit>();
				if (this.bufferCount > 0)
				{
					this.tempHitInfo = this.rayCastNonAllocColliders[0];
					foreach (RaycastHit raycastHit in list)
					{
						if (!raycastHit.collider.isTrigger && raycastHit.distance < this.tempHitInfo.distance)
						{
							this.tempHitInfo = raycastHit;
						}
					}
					finalPosition = startPosition + movementVector.normalized * this.tempHitInfo.distance;
				}
				this.MaxSphereSizeForNoOverlap(sphereRadius, finalPosition, false, out this.maxSphereSize2);
				this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
				this.bufferCount = Physics.SphereCastNonAlloc(startPosition, Mathf.Min(this.maxSphereSize1, this.maxSphereSize2), (finalPosition - startPosition).normalized, this.rayCastNonAllocColliders, (finalPosition - startPosition).magnitude, this.locomotionEnabledLayers.value);
				if (this.bufferCount > 0)
				{
					this.tempHitInfo = this.rayCastNonAllocColliders[0];
					for (int j = 0; j < this.bufferCount; j++)
					{
						if (this.rayCastNonAllocColliders[j].collider != null && this.rayCastNonAllocColliders[j].distance < this.tempHitInfo.distance)
						{
							this.tempHitInfo = this.rayCastNonAllocColliders[j];
						}
					}
					finalPosition = startPosition + this.tempHitInfo.distance * (finalPosition - startPosition).normalized;
					collisionsHitInfo = this.tempHitInfo;
				}
				if (list.Any<RaycastHit>())
				{
					this.tempHitInfo = list.First<RaycastHit>();
					foreach (RaycastHit raycastHit2 in list)
					{
						if (raycastHit2.distance < this.tempHitInfo.distance)
						{
							this.tempHitInfo = raycastHit2;
						}
					}
					collisionsHitInfo = this.tempHitInfo;
					finalPosition = startPosition;
				}
				return true;
			}
			this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
			this.bufferCount = Physics.RaycastNonAlloc(startPosition, movementVector.normalized, this.rayCastNonAllocColliders, movementVector.magnitude, this.locomotionEnabledLayers.value);
			if (this.bufferCount > 0)
			{
				this.tempHitInfo = this.rayCastNonAllocColliders[0];
				for (int k = 0; k < this.bufferCount; k++)
				{
					if (this.rayCastNonAllocColliders[k].collider != null && this.rayCastNonAllocColliders[k].distance < this.tempHitInfo.distance)
					{
						this.tempHitInfo = this.rayCastNonAllocColliders[k];
					}
				}
				collisionsHitInfo = this.tempHitInfo;
				finalPosition = startPosition;
				return true;
			}
			finalPosition = startPosition + movementVector;
			collisionsHitInfo = default(RaycastHit);
			return false;
		}

		// Token: 0x06005946 RID: 22854 RVA: 0x001BF828 File Offset: 0x001BDA28
		public bool IsHandTouching(bool forLeftHand)
		{
			if (forLeftHand)
			{
				return this.wasLeftHandColliding;
			}
			return this.wasRightHandColliding;
		}

		// Token: 0x06005947 RID: 22855 RVA: 0x001BF83A File Offset: 0x001BDA3A
		public bool IsHandSliding(bool forLeftHand)
		{
			if (forLeftHand)
			{
				return this.wasLeftHandSliding || this.isLeftHandSliding;
			}
			return this.wasRightHandSliding || this.isRightHandSliding;
		}

		// Token: 0x06005948 RID: 22856 RVA: 0x001BF860 File Offset: 0x001BDA60
		public float GetSlidePercentage(RaycastHit raycastHit)
		{
			this.currentOverride = raycastHit.collider.gameObject.GetComponent<GorillaSurfaceOverride>();
			BasePlatform component = raycastHit.collider.gameObject.GetComponent<BasePlatform>();
			if (component != null)
			{
				this.currentPlatform = component;
			}
			if (this.currentOverride != null)
			{
				if (this.currentOverride.slidePercentageOverride >= 0f)
				{
					return this.currentOverride.slidePercentageOverride;
				}
				this.currentMaterialIndex = this.currentOverride.overrideIndex;
				if (this.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
				{
					return this.FreezeTagSlidePercentage();
				}
				if (!this.materialData[this.currentMaterialIndex].overrideSlidePercent)
				{
					return this.defaultSlideFactor;
				}
				return this.materialData[this.currentMaterialIndex].slidePercent;
			}
			else
			{
				this.meshCollider = (raycastHit.collider as MeshCollider);
				if (this.meshCollider == null || this.meshCollider.sharedMesh == null || this.meshCollider.convex)
				{
					return this.defaultSlideFactor;
				}
				this.collidedMesh = this.meshCollider.sharedMesh;
				if (!this.meshTrianglesDict.TryGetValue(this.collidedMesh, out this.sharedMeshTris))
				{
					this.sharedMeshTris = this.collidedMesh.triangles;
					this.meshTrianglesDict.Add(this.collidedMesh, (int[])this.sharedMeshTris.Clone());
				}
				this.vertex1 = this.sharedMeshTris[raycastHit.triangleIndex * 3];
				this.vertex2 = this.sharedMeshTris[raycastHit.triangleIndex * 3 + 1];
				this.vertex3 = this.sharedMeshTris[raycastHit.triangleIndex * 3 + 2];
				this.slideRenderer = raycastHit.collider.GetComponent<Renderer>();
				if (this.slideRenderer != null)
				{
					this.slideRenderer.GetSharedMaterials(this.tempMaterialArray);
				}
				else
				{
					this.tempMaterialArray.Clear();
				}
				if (this.tempMaterialArray.Count > 1)
				{
					for (int i = 0; i < this.tempMaterialArray.Count; i++)
					{
						this.collidedMesh.GetTriangles(this.trianglesList, i);
						int j = 0;
						while (j < this.trianglesList.Count)
						{
							if (this.trianglesList[j] == this.vertex1 && this.trianglesList[j + 1] == this.vertex2 && this.trianglesList[j + 2] == this.vertex3)
							{
								this.findMatName = this.tempMaterialArray[i].name;
								if (this.findMatName.EndsWith("Uber"))
								{
									string text = this.findMatName;
									this.findMatName = text.Substring(0, text.Length - 4);
								}
								this.foundMatData = this.materialData.Find((GTPlayer.MaterialData matData) => matData.matName == this.findMatName);
								this.currentMaterialIndex = this.materialData.FindIndex((GTPlayer.MaterialData matData) => matData.matName == this.findMatName);
								if (this.currentMaterialIndex == -1)
								{
									this.currentMaterialIndex = 0;
								}
								if (this.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
								{
									return this.FreezeTagSlidePercentage();
								}
								if (!this.foundMatData.overrideSlidePercent)
								{
									return this.defaultSlideFactor;
								}
								return this.foundMatData.slidePercent;
							}
							else
							{
								j += 3;
							}
						}
					}
				}
				else if (this.tempMaterialArray.Count > 0)
				{
					this.findMatName = this.tempMaterialArray[0].name;
					if (this.findMatName.EndsWith("Uber"))
					{
						string text = this.findMatName;
						this.findMatName = text.Substring(0, text.Length - 4);
					}
					this.foundMatData = this.materialData.Find((GTPlayer.MaterialData matData) => matData.matName == this.findMatName);
					this.currentMaterialIndex = this.materialData.FindIndex((GTPlayer.MaterialData matData) => matData.matName == this.findMatName);
					if (this.currentMaterialIndex == -1)
					{
						this.currentMaterialIndex = 0;
					}
					if (this.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
					{
						return this.FreezeTagSlidePercentage();
					}
					if (!this.foundMatData.overrideSlidePercent)
					{
						return this.defaultSlideFactor;
					}
					return this.foundMatData.slidePercent;
				}
				this.currentMaterialIndex = 0;
				return this.defaultSlideFactor;
			}
		}

		// Token: 0x06005949 RID: 22857 RVA: 0x001BFCBC File Offset: 0x001BDEBC
		public bool IsTouchingMovingSurface(Vector3 rayOrigin, RaycastHit raycastHit, out int movingSurfaceId, out bool sideTouch, out bool isMonkeBlock)
		{
			movingSurfaceId = -1;
			sideTouch = false;
			isMonkeBlock = false;
			float num = Vector3.Dot(rayOrigin - raycastHit.point, Vector3.up);
			if (num < -0.3f)
			{
				return false;
			}
			if (num < 0f)
			{
				sideTouch = true;
			}
			if (raycastHit.collider == null)
			{
				return false;
			}
			MovingSurface component = raycastHit.collider.GetComponent<MovingSurface>();
			if (component != null)
			{
				isMonkeBlock = false;
				movingSurfaceId = component.GetID();
				return true;
			}
			if (!BuilderTable.IsLocalPlayerInBuilderZone())
			{
				return false;
			}
			BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(raycastHit.collider);
			if (builderPieceFromCollider != null && builderPieceFromCollider.IsPieceMoving())
			{
				isMonkeBlock = true;
				movingSurfaceId = builderPieceFromCollider.pieceId;
				this.lastMonkeBlock = builderPieceFromCollider;
				return true;
			}
			sideTouch = false;
			return false;
		}

		// Token: 0x0600594A RID: 22858 RVA: 0x001BFD78 File Offset: 0x001BDF78
		public void Turn(float degrees)
		{
			Vector3 position = this.headCollider.transform.position;
			bool flag = this.isRightHandColliding || this.rightHandHolding;
			bool flag2 = this.isLeftHandColliding || this.leftHandHolding;
			if (flag != flag2 && flag)
			{
				position = this.rightControllerTransform.position;
			}
			if (flag != flag2 && flag2)
			{
				position = this.leftControllerTransform.position;
			}
			this.turnParent.transform.RotateAround(position, base.transform.up, degrees);
			this.degreesTurnedThisFrame = degrees;
			this.averagedVelocity = Vector3.zero;
			for (int i = 0; i < this.velocityHistory.Length; i++)
			{
				this.velocityHistory[i] = Quaternion.Euler(0f, degrees, 0f) * this.velocityHistory[i];
				this.averagedVelocity += this.velocityHistory[i];
			}
			this.averagedVelocity /= (float)this.velocityHistorySize;
		}

		// Token: 0x0600594B RID: 22859 RVA: 0x001BFE8C File Offset: 0x001BE08C
		public void BeginClimbing(GorillaClimbable climbable, GorillaHandClimber hand, GorillaClimbableRef climbableRef = null)
		{
			if (this.currentClimber != null)
			{
				this.EndClimbing(this.currentClimber, true, false);
			}
			try
			{
				Action<GorillaHandClimber, GorillaClimbableRef> onBeforeClimb = climbable.onBeforeClimb;
				if (onBeforeClimb != null)
				{
					onBeforeClimb(hand, climbableRef);
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			Rigidbody rigidbody;
			climbable.TryGetComponent<Rigidbody>(out rigidbody);
			this.VerifyClimbHelper();
			this.climbHelper.SetParent(climbable.transform);
			this.climbHelper.position = hand.transform.position;
			Vector3 localPosition = this.climbHelper.localPosition;
			if (climbable.snapX)
			{
				GTPlayer.<BeginClimbing>g__SnapAxis|407_0(ref localPosition.x, climbable.maxDistanceSnap);
			}
			if (climbable.snapY)
			{
				GTPlayer.<BeginClimbing>g__SnapAxis|407_0(ref localPosition.y, climbable.maxDistanceSnap);
			}
			if (climbable.snapZ)
			{
				GTPlayer.<BeginClimbing>g__SnapAxis|407_0(ref localPosition.z, climbable.maxDistanceSnap);
			}
			this.climbHelperTargetPos = localPosition;
			climbable.isBeingClimbed = true;
			hand.isClimbing = true;
			this.currentClimbable = climbable;
			this.currentClimber = hand;
			this.isClimbing = true;
			if (climbable.climbOnlyWhileSmall)
			{
				BuilderPiece componentInParent = climbable.GetComponentInParent<BuilderPiece>();
				if (componentInParent != null && componentInParent.IsPieceMoving())
				{
					this.isClimbableMoving = true;
					this.lastClimbableRotation = climbable.transform.rotation;
				}
				else
				{
					this.isClimbableMoving = false;
				}
			}
			else
			{
				this.isClimbableMoving = false;
			}
			GorillaRopeSegment gorillaRopeSegment;
			GorillaZipline gorillaZipline;
			PhotonView view;
			PhotonViewXSceneRef photonViewXSceneRef;
			if (climbable.TryGetComponent<GorillaRopeSegment>(out gorillaRopeSegment) && gorillaRopeSegment.swing)
			{
				this.currentSwing = gorillaRopeSegment.swing;
				this.currentSwing.AttachLocalPlayer(hand.xrNode, climbable.transform, this.climbHelperTargetPos, this.averagedVelocity);
			}
			else if (climbable.transform.parent && climbable.transform.parent.TryGetComponent<GorillaZipline>(out gorillaZipline))
			{
				this.currentZipline = gorillaZipline;
			}
			else if (climbable.TryGetComponent<PhotonView>(out view))
			{
				VRRig.AttachLocalPlayerToPhotonView(view, hand.xrNode, this.climbHelperTargetPos, this.averagedVelocity);
			}
			else if (climbable.TryGetComponent<PhotonViewXSceneRef>(out photonViewXSceneRef))
			{
				VRRig.AttachLocalPlayerToPhotonView(photonViewXSceneRef.photonView, hand.xrNode, this.climbHelperTargetPos, this.averagedVelocity);
			}
			GorillaTagger.Instance.StartVibration(this.currentClimber.xrNode == XRNode.LeftHand, 0.6f, 0.06f);
			if (climbable.clip)
			{
				GorillaTagger.Instance.offlineVRRig.PlayClimbSound(climbable.clip, hand.xrNode == XRNode.LeftHand);
			}
		}

		// Token: 0x0600594C RID: 22860 RVA: 0x001C00F8 File Offset: 0x001BE2F8
		private void VerifyClimbHelper()
		{
			if (this.climbHelper == null || this.climbHelper.gameObject == null)
			{
				this.climbHelper = new GameObject("Climb Helper").transform;
			}
		}

		// Token: 0x0600594D RID: 22861 RVA: 0x001C0130 File Offset: 0x001BE330
		public GorillaVelocityTracker GetInteractPointVelocityTracker(bool isRightHand)
		{
			if (!isRightHand)
			{
				return this.leftInteractPointVelocityTracker;
			}
			return this.rightInteractPointVelocityTracker;
		}

		// Token: 0x0600594E RID: 22862 RVA: 0x001C0144 File Offset: 0x001BE344
		public void EndClimbing(GorillaHandClimber hand, bool startingNewClimb, bool doDontReclimb = false)
		{
			if (hand != this.currentClimber)
			{
				return;
			}
			hand.SetCanRelease(true);
			if (!startingNewClimb)
			{
				this.enablePlayerGravity(true);
			}
			Rigidbody rigidbody = null;
			if (this.currentClimbable)
			{
				this.currentClimbable.TryGetComponent<Rigidbody>(out rigidbody);
				this.currentClimbable.isBeingClimbed = false;
			}
			Vector3 vector = Vector3.zero;
			if (this.currentClimber)
			{
				this.currentClimber.isClimbing = false;
				if (doDontReclimb)
				{
					this.currentClimber.dontReclimbLast = this.currentClimbable;
				}
				else
				{
					this.currentClimber.dontReclimbLast = null;
				}
				this.currentClimber.queuedToBecomeValidToGrabAgain = true;
				this.currentClimber.lastAutoReleasePos = this.currentClimber.handRoot.localPosition;
				if (!startingNewClimb && this.currentClimbable)
				{
					GorillaVelocityTracker gorillaVelocityTracker = (this.currentClimber.xrNode == XRNode.LeftHand) ? this.leftInteractPointVelocityTracker : this.rightInteractPointVelocityTracker;
					if (rigidbody)
					{
						this.playerRigidBody.velocity = rigidbody.velocity;
					}
					else if (this.currentSwing)
					{
						this.playerRigidBody.velocity = this.currentSwing.velocityTracker.GetAverageVelocity(true, 0.25f, false);
					}
					else if (this.currentZipline)
					{
						this.playerRigidBody.velocity = this.currentZipline.GetCurrentDirection() * this.currentZipline.currentSpeed;
					}
					else
					{
						this.playerRigidBody.velocity = Vector3.zero;
					}
					vector = this.turnParent.transform.rotation * -gorillaVelocityTracker.GetAverageVelocity(false, 0.1f, true) * this.scale;
					vector = Vector3.ClampMagnitude(vector, 5.5f * this.scale);
					this.playerRigidBody.AddForce(vector, ForceMode.VelocityChange);
				}
			}
			if (this.currentSwing)
			{
				this.currentSwing.DetachLocalPlayer();
			}
			PhotonView photonView;
			PhotonViewXSceneRef photonViewXSceneRef;
			if (this.currentClimbable.TryGetComponent<PhotonView>(out photonView) || this.currentClimbable.TryGetComponent<PhotonViewXSceneRef>(out photonViewXSceneRef) || this.currentClimbable.IsPlayerAttached)
			{
				VRRig.DetachLocalPlayerFromPhotonView();
			}
			if (!startingNewClimb && vector.magnitude > 2f && this.currentClimbable && this.currentClimbable.clipOnFullRelease)
			{
				GorillaTagger.Instance.offlineVRRig.PlayClimbSound(this.currentClimbable.clipOnFullRelease, hand.xrNode == XRNode.LeftHand);
			}
			this.currentClimbable = null;
			this.currentClimber = null;
			this.currentSwing = null;
			this.currentZipline = null;
			this.isClimbing = false;
		}

		// Token: 0x0600594F RID: 22863 RVA: 0x001C03DC File Offset: 0x001BE5DC
		private void enablePlayerGravity(bool useGravity)
		{
			this.playerRigidBody.useGravity = useGravity;
		}

		// Token: 0x06005950 RID: 22864 RVA: 0x001C03EA File Offset: 0x001BE5EA
		public void SetVelocity(Vector3 velocity)
		{
			this.playerRigidBody.velocity = velocity;
		}

		// Token: 0x06005951 RID: 22865 RVA: 0x001C03F8 File Offset: 0x001BE5F8
		public void TempFreezeHand(bool isLeft, float freezeDuration)
		{
			if (isLeft)
			{
				this.tempFreezeLeftHandEnableTime = Math.Max(this.tempFreezeLeftHandEnableTime, Time.timeAsDouble + (double)freezeDuration);
				return;
			}
			this.tempFreezeRightHandEnableTime = Math.Max(this.tempFreezeRightHandEnableTime, Time.timeAsDouble + (double)freezeDuration);
		}

		// Token: 0x06005952 RID: 22866 RVA: 0x001C0430 File Offset: 0x001BE630
		private void StoreVelocities()
		{
			this.velocityIndex = (this.velocityIndex + 1) % this.velocityHistorySize;
			this.currentVelocity = (base.transform.position - this.lastPosition - this.MovingSurfaceMovement()) / this.calcDeltaTime;
			this.velocityHistory[this.velocityIndex] = this.currentVelocity;
			this.averagedVelocity = this.velocityHistory.Average();
			this.lastPosition = base.transform.position;
		}

		// Token: 0x06005953 RID: 22867 RVA: 0x001C04C0 File Offset: 0x001BE6C0
		private void AntiTeleportTechnology()
		{
			if ((this.headCollider.transform.position - this.lastHeadPosition).magnitude >= this.teleportThresholdNoVel + this.playerRigidBody.velocity.magnitude * this.calcDeltaTime)
			{
				base.transform.position = base.transform.position + this.lastHeadPosition - this.headCollider.transform.position;
			}
		}

		// Token: 0x06005954 RID: 22868 RVA: 0x001C054C File Offset: 0x001BE74C
		private bool MaxSphereSizeForNoOverlap(float testRadius, Vector3 checkPosition, bool ignoreOneWay, out float overlapRadiusTest)
		{
			overlapRadiusTest = testRadius;
			this.overlapAttempts = 0;
			int num = 100;
			while (this.overlapAttempts < num && overlapRadiusTest > testRadius * 0.75f)
			{
				this.ClearColliderBuffer(ref this.overlapColliders);
				this.bufferCount = Physics.OverlapSphereNonAlloc(checkPosition, overlapRadiusTest, this.overlapColliders, this.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore);
				if (ignoreOneWay)
				{
					int num2 = 0;
					for (int i = 0; i < this.bufferCount; i++)
					{
						if (this.overlapColliders[i].CompareTag("NoCrazyCheck"))
						{
							num2++;
						}
					}
					if (num2 == this.bufferCount)
					{
						return true;
					}
				}
				if (this.bufferCount <= 0)
				{
					overlapRadiusTest *= 0.995f;
					return true;
				}
				overlapRadiusTest = Mathf.Lerp(testRadius, 0f, (float)this.overlapAttempts / (float)num);
				this.overlapAttempts++;
			}
			return false;
		}

		// Token: 0x06005955 RID: 22869 RVA: 0x001C062C File Offset: 0x001BE82C
		private bool CrazyCheck2(float sphereSize, Vector3 startPosition)
		{
			for (int i = 0; i < this.crazyCheckVectors.Length; i++)
			{
				if (this.NonAllocRaycast(startPosition, startPosition + this.crazyCheckVectors[i] * sphereSize) > 0)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06005956 RID: 22870 RVA: 0x001C0674 File Offset: 0x001BE874
		private int NonAllocRaycast(Vector3 startPosition, Vector3 endPosition)
		{
			Vector3 direction = endPosition - startPosition;
			int num = Physics.RaycastNonAlloc(startPosition, direction, this.rayCastNonAllocColliders, direction.magnitude, this.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore);
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				if (!this.rayCastNonAllocColliders[i].collider.gameObject.CompareTag("NoCrazyCheck"))
				{
					num2++;
				}
			}
			return num2;
		}

		// Token: 0x06005957 RID: 22871 RVA: 0x001C06E0 File Offset: 0x001BE8E0
		private void ClearColliderBuffer(ref Collider[] colliders)
		{
			for (int i = 0; i < colliders.Length; i++)
			{
				colliders[i] = null;
			}
		}

		// Token: 0x06005958 RID: 22872 RVA: 0x001C0704 File Offset: 0x001BE904
		private void ClearRaycasthitBuffer(ref RaycastHit[] raycastHits)
		{
			for (int i = 0; i < raycastHits.Length; i++)
			{
				raycastHits[i] = this.emptyHit;
			}
		}

		// Token: 0x06005959 RID: 22873 RVA: 0x001C072E File Offset: 0x001BE92E
		private Vector3 MovingSurfaceMovement()
		{
			return this.refMovement + this.movingSurfaceOffset;
		}

		// Token: 0x0600595A RID: 22874 RVA: 0x001C0744 File Offset: 0x001BE944
		private static bool ComputeLocalHitPoint(RaycastHit hit, out Vector3 localHitPoint)
		{
			if (hit.collider == null || hit.point.sqrMagnitude < 0.001f)
			{
				localHitPoint = Vector3.zero;
				return false;
			}
			localHitPoint = hit.collider.transform.InverseTransformPoint(hit.point);
			return true;
		}

		// Token: 0x0600595B RID: 22875 RVA: 0x001C07A2 File Offset: 0x001BE9A2
		private static bool ComputeWorldHitPoint(RaycastHit hit, Vector3 localPoint, out Vector3 worldHitPoint)
		{
			if (hit.collider == null)
			{
				worldHitPoint = Vector3.zero;
				return false;
			}
			worldHitPoint = hit.collider.transform.TransformPoint(localPoint);
			return true;
		}

		// Token: 0x0600595C RID: 22876 RVA: 0x001C07DC File Offset: 0x001BE9DC
		private float ExtraVelMultiplier()
		{
			float num = 1f;
			if (this.leftHandSurfaceOverride != null)
			{
				num = Mathf.Max(num, this.leftHandSurfaceOverride.extraVelMultiplier);
			}
			if (this.rightHandSurfaceOverride != null)
			{
				num = Mathf.Max(num, this.rightHandSurfaceOverride.extraVelMultiplier);
			}
			return num;
		}

		// Token: 0x0600595D RID: 22877 RVA: 0x001C0830 File Offset: 0x001BEA30
		private float ExtraVelMaxMultiplier()
		{
			float num = 1f;
			if (this.leftHandSurfaceOverride != null)
			{
				num = Mathf.Max(num, this.leftHandSurfaceOverride.extraVelMaxMultiplier);
			}
			if (this.rightHandSurfaceOverride != null)
			{
				num = Mathf.Max(num, this.rightHandSurfaceOverride.extraVelMaxMultiplier);
			}
			return num * this.scale;
		}

		// Token: 0x0600595E RID: 22878 RVA: 0x001C088D File Offset: 0x001BEA8D
		public void SetMaximumSlipThisFrame()
		{
			this.leftSlipSetToMaxFrameIdx = Time.frameCount;
			this.rightSlipSetToMaxFrameIdx = Time.frameCount;
		}

		// Token: 0x0600595F RID: 22879 RVA: 0x001C08A5 File Offset: 0x001BEAA5
		public void SetLeftMaximumSlipThisFrame()
		{
			this.leftSlipSetToMaxFrameIdx = Time.frameCount;
		}

		// Token: 0x06005960 RID: 22880 RVA: 0x001C08B2 File Offset: 0x001BEAB2
		public void SetRightMaximumSlipThisFrame()
		{
			this.rightSlipSetToMaxFrameIdx = Time.frameCount;
		}

		// Token: 0x06005961 RID: 22881 RVA: 0x001C08BF File Offset: 0x001BEABF
		public bool LeftSlipOverriddenToMax()
		{
			return this.leftSlipSetToMaxFrameIdx == Time.frameCount;
		}

		// Token: 0x06005962 RID: 22882 RVA: 0x001C08CE File Offset: 0x001BEACE
		public bool RightSlipOverriddenToMax()
		{
			return this.rightSlipSetToMaxFrameIdx == Time.frameCount;
		}

		// Token: 0x06005963 RID: 22883 RVA: 0x001C08DD File Offset: 0x001BEADD
		public void ChangeLayer(string layerName)
		{
			if (this.layerChanger != null)
			{
				this.layerChanger.ChangeLayer(base.transform.parent, layerName);
			}
		}

		// Token: 0x06005964 RID: 22884 RVA: 0x001C0904 File Offset: 0x001BEB04
		public void RestoreLayer()
		{
			if (this.layerChanger != null)
			{
				this.layerChanger.RestoreOriginalLayers();
			}
		}

		// Token: 0x06005965 RID: 22885 RVA: 0x001C0920 File Offset: 0x001BEB20
		public void OnEnterWaterVolume(Collider playerCollider, WaterVolume volume)
		{
			if (this.activeSizeChangerSettings != null && this.activeSizeChangerSettings.ExpireInWater)
			{
				this.SetNativeScale(null);
			}
			if (playerCollider == this.headCollider)
			{
				if (!this.headOverlappingWaterVolumes.Contains(volume))
				{
					this.headOverlappingWaterVolumes.Add(volume);
					return;
				}
			}
			else if (playerCollider == this.bodyCollider && !this.bodyOverlappingWaterVolumes.Contains(volume))
			{
				this.bodyOverlappingWaterVolumes.Add(volume);
			}
		}

		// Token: 0x06005966 RID: 22886 RVA: 0x001C099A File Offset: 0x001BEB9A
		public void OnExitWaterVolume(Collider playerCollider, WaterVolume volume)
		{
			if (playerCollider == this.headCollider)
			{
				this.headOverlappingWaterVolumes.Remove(volume);
				return;
			}
			if (playerCollider == this.bodyCollider)
			{
				this.bodyOverlappingWaterVolumes.Remove(volume);
			}
		}

		// Token: 0x06005967 RID: 22887 RVA: 0x001C09D4 File Offset: 0x001BEBD4
		private bool GetSwimmingVelocityForHand(Vector3 startingHandPosition, Vector3 endingHandPosition, Vector3 palmForwardDirection, float dt, ref WaterVolume contactingWaterVolume, ref WaterVolume.SurfaceQuery waterSurface, out Vector3 swimmingVelocityChange)
		{
			contactingWaterVolume = null;
			this.bufferCount = Physics.OverlapSphereNonAlloc(endingHandPosition, this.minimumRaycastDistance, this.overlapColliders, this.waterLayer.value, QueryTriggerInteraction.Collide);
			if (this.bufferCount > 0)
			{
				float num = float.MinValue;
				for (int i = 0; i < this.bufferCount; i++)
				{
					WaterVolume component = this.overlapColliders[i].GetComponent<WaterVolume>();
					WaterVolume.SurfaceQuery surfaceQuery;
					if (component != null && component.GetSurfaceQueryForPoint(endingHandPosition, out surfaceQuery, false) && surfaceQuery.surfacePoint.y > num)
					{
						num = surfaceQuery.surfacePoint.y;
						contactingWaterVolume = component;
						waterSurface = surfaceQuery;
					}
				}
			}
			if (contactingWaterVolume != null)
			{
				Vector3 a = endingHandPosition - startingHandPosition;
				Vector3 b = Vector3.zero;
				Vector3 b2 = this.playerRigidBody.transform.position - this.lastRigidbodyPosition;
				if (this.turnedThisFrame)
				{
					Vector3 vector = startingHandPosition - this.headCollider.transform.position;
					b = Quaternion.AngleAxis(this.degreesTurnedThisFrame, Vector3.up) * vector - vector;
				}
				float num2 = Vector3.Dot(a - b - b2, palmForwardDirection);
				float num3 = 0f;
				if (num2 > 0f)
				{
					Plane surfacePlane = waterSurface.surfacePlane;
					float distanceToPoint = surfacePlane.GetDistanceToPoint(startingHandPosition);
					float distanceToPoint2 = surfacePlane.GetDistanceToPoint(endingHandPosition);
					if (distanceToPoint <= 0f && distanceToPoint2 <= 0f)
					{
						num3 = 1f;
					}
					else if (distanceToPoint > 0f && distanceToPoint2 <= 0f)
					{
						num3 = -distanceToPoint2 / (distanceToPoint - distanceToPoint2);
					}
					else if (distanceToPoint <= 0f && distanceToPoint2 > 0f)
					{
						num3 = -distanceToPoint / (distanceToPoint2 - distanceToPoint);
					}
					if (num3 > Mathf.Epsilon)
					{
						float resistance = this.liquidPropertiesList[(int)contactingWaterVolume.LiquidType].resistance;
						swimmingVelocityChange = -palmForwardDirection * num2 * 2f * resistance * num3;
						Vector3 forward = this.mainCamera.transform.forward;
						if (forward.y < 0f)
						{
							Vector3 vector2 = forward.x0z();
							float magnitude = vector2.magnitude;
							vector2 /= magnitude;
							float num4 = Vector3.Dot(swimmingVelocityChange, vector2);
							if (num4 > 0f)
							{
								Vector3 vector3 = vector2 * num4;
								swimmingVelocityChange = swimmingVelocityChange - vector3 + vector3 * magnitude + Vector3.up * forward.y * num4;
							}
						}
						return true;
					}
				}
			}
			swimmingVelocityChange = Vector3.zero;
			return false;
		}

		// Token: 0x06005968 RID: 22888 RVA: 0x001C0C90 File Offset: 0x001BEE90
		private bool CheckWaterSurfaceJump(Vector3 startingHandPosition, Vector3 endingHandPosition, Vector3 palmForwardDirection, Vector3 handAvgVelocity, PlayerSwimmingParameters parameters, WaterVolume contactingWaterVolume, WaterVolume.SurfaceQuery waterSurface, out Vector3 jumpVelocity)
		{
			if (contactingWaterVolume != null)
			{
				Plane surfacePlane = waterSurface.surfacePlane;
				bool flag = handAvgVelocity.sqrMagnitude > parameters.waterSurfaceJumpHandSpeedThreshold * parameters.waterSurfaceJumpHandSpeedThreshold;
				if (surfacePlane.GetSide(startingHandPosition) && !surfacePlane.GetSide(endingHandPosition) && flag)
				{
					float value = Vector3.Dot(palmForwardDirection, -waterSurface.surfaceNormal);
					float value2 = Vector3.Dot(handAvgVelocity.normalized, -waterSurface.surfaceNormal);
					float d = parameters.waterSurfaceJumpPalmFacingCurve.Evaluate(Mathf.Clamp(value, 0.01f, 0.99f));
					float d2 = parameters.waterSurfaceJumpHandVelocityFacingCurve.Evaluate(Mathf.Clamp(value2, 0.01f, 0.99f));
					jumpVelocity = -handAvgVelocity * parameters.waterSurfaceJumpAmount * d * d2;
					return true;
				}
			}
			jumpVelocity = Vector3.zero;
			return false;
		}

		// Token: 0x06005969 RID: 22889 RVA: 0x001C0D89 File Offset: 0x001BEF89
		private bool TryNormalize(Vector3 input, out Vector3 normalized, out float magnitude, float eps = 0.0001f)
		{
			magnitude = input.magnitude;
			if (magnitude > eps)
			{
				normalized = input / magnitude;
				return true;
			}
			normalized = Vector3.zero;
			return false;
		}

		// Token: 0x0600596A RID: 22890 RVA: 0x001C0DB6 File Offset: 0x001BEFB6
		private bool TryNormalizeDown(Vector3 input, out Vector3 normalized, out float magnitude, float eps = 0.0001f)
		{
			magnitude = input.magnitude;
			if (magnitude > 1f)
			{
				normalized = input / magnitude;
				return true;
			}
			if (magnitude >= eps)
			{
				normalized = input;
				return true;
			}
			normalized = Vector3.zero;
			return false;
		}

		// Token: 0x0600596B RID: 22891 RVA: 0x001C0DF8 File Offset: 0x001BEFF8
		private float FreezeTagSlidePercentage()
		{
			if (this.materialData[this.currentMaterialIndex].overrideSlidePercent && this.materialData[this.currentMaterialIndex].slidePercent > this.freezeTagHandSlidePercent)
			{
				return this.materialData[this.currentMaterialIndex].slidePercent;
			}
			return this.freezeTagHandSlidePercent;
		}

		// Token: 0x0600596C RID: 22892 RVA: 0x001C0E58 File Offset: 0x001BF058
		private void OnCollisionStay(UnityEngine.Collision collision)
		{
			this.bodyCollisionContactsCount = collision.GetContacts(this.bodyCollisionContacts);
			float num = -1f;
			for (int i = 0; i < this.bodyCollisionContactsCount; i++)
			{
				float num2 = Vector3.Dot(this.bodyCollisionContacts[i].normal, Vector3.up);
				if (num2 > num)
				{
					this.bodyGroundContact = this.bodyCollisionContacts[i];
					num = num2;
				}
			}
			float num3 = 0.5f;
			if (num > num3)
			{
				this.bodyGroundContactTime = Time.time;
			}
		}

		// Token: 0x0600596D RID: 22893 RVA: 0x001C0EE0 File Offset: 0x001BF0E0
		public void DoLaunch(Vector3 velocity)
		{
			GTPlayer.<DoLaunch>d__441 <DoLaunch>d__;
			<DoLaunch>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<DoLaunch>d__.<>4__this = this;
			<DoLaunch>d__.velocity = velocity;
			<DoLaunch>d__.<>1__state = -1;
			<DoLaunch>d__.<>t__builder.Start<GTPlayer.<DoLaunch>d__441>(ref <DoLaunch>d__);
		}

		// Token: 0x0600596E RID: 22894 RVA: 0x001C0F1F File Offset: 0x001BF11F
		private void OnEnable()
		{
			RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
		}

		// Token: 0x0600596F RID: 22895 RVA: 0x001C0F3C File Offset: 0x001BF13C
		private void OnJoinedRoom()
		{
			if (this.activeSizeChangerSettings != null && this.activeSizeChangerSettings.ExpireOnRoomJoin)
			{
				this.SetNativeScale(null);
			}
		}

		// Token: 0x06005970 RID: 22896 RVA: 0x001C0F5A File Offset: 0x001BF15A
		private void OnDisable()
		{
			RoomSystem.JoinedRoomEvent -= new Action(this.OnJoinedRoom);
		}

		// Token: 0x06005971 RID: 22897 RVA: 0x001C0F78 File Offset: 0x001BF178
		internal void AddHandHold(Transform objectHeld, Vector3 localPositionHeld, GorillaGrabber grabber, bool rightHand, bool rotatePlayerWhenHeld, out Vector3 grabbedVelocity)
		{
			if (!this.leftHandHolding && !this.rightHandHolding)
			{
				grabbedVelocity = -this.bodyCollider.attachedRigidbody.velocity;
				this.playerRigidBody.AddForce(grabbedVelocity, ForceMode.VelocityChange);
			}
			else
			{
				grabbedVelocity = Vector3.zero;
			}
			this.secondaryHandHold = this.activeHandHold;
			Vector3 position = grabber.transform.position;
			this.activeHandHold = new GTPlayer.HandHoldState
			{
				grabber = grabber,
				objectHeld = objectHeld,
				localPositionHeld = localPositionHeld,
				localRotationalOffset = grabber.transform.rotation.eulerAngles.y - objectHeld.rotation.eulerAngles.y,
				applyRotation = rotatePlayerWhenHeld
			};
			if (rightHand)
			{
				this.rightHandHolding = true;
			}
			else
			{
				this.leftHandHolding = true;
			}
			this.OnChangeActiveHandhold();
		}

		// Token: 0x06005972 RID: 22898 RVA: 0x001C1068 File Offset: 0x001BF268
		internal void RemoveHandHold(GorillaGrabber grabber, bool rightHand)
		{
			this.activeHandHold.objectHeld == grabber;
			if (this.activeHandHold.grabber == grabber)
			{
				this.activeHandHold = this.secondaryHandHold;
			}
			this.secondaryHandHold = default(GTPlayer.HandHoldState);
			if (rightHand)
			{
				this.rightHandHolding = false;
			}
			else
			{
				this.leftHandHolding = false;
			}
			this.OnChangeActiveHandhold();
		}

		// Token: 0x06005973 RID: 22899 RVA: 0x001C10D0 File Offset: 0x001BF2D0
		private void OnChangeActiveHandhold()
		{
			if (this.activeHandHold.objectHeld != null)
			{
				PhotonView view;
				if (this.activeHandHold.objectHeld.TryGetComponent<PhotonView>(out view))
				{
					VRRig.AttachLocalPlayerToPhotonView(view, this.activeHandHold.grabber.XrNode, this.activeHandHold.localPositionHeld, this.averagedVelocity);
					return;
				}
				PhotonViewXSceneRef photonViewXSceneRef;
				if (this.activeHandHold.objectHeld.TryGetComponent<PhotonViewXSceneRef>(out photonViewXSceneRef))
				{
					PhotonView photonView = photonViewXSceneRef.photonView;
					if (photonView != null)
					{
						VRRig.AttachLocalPlayerToPhotonView(photonView, this.activeHandHold.grabber.XrNode, this.activeHandHold.localPositionHeld, this.averagedVelocity);
						return;
					}
				}
				BuilderPieceHandHold builderPieceHandHold;
				if (this.activeHandHold.objectHeld.TryGetComponent<BuilderPieceHandHold>(out builderPieceHandHold) && builderPieceHandHold.IsHandHoldMoving())
				{
					this.isHandHoldMoving = true;
					this.lastHandHoldRotation = builderPieceHandHold.transform.rotation;
					this.movingHandHoldReleaseVelocity = this.playerRigidBody.velocity;
				}
				else
				{
					this.isHandHoldMoving = false;
					this.lastHandHoldRotation = Quaternion.identity;
					this.movingHandHoldReleaseVelocity = Vector3.zero;
				}
			}
			VRRig.DetachLocalPlayerFromPhotonView();
		}

		// Token: 0x06005974 RID: 22900 RVA: 0x001C11E0 File Offset: 0x001BF3E0
		private void FixedUpdate_HandHolds(float timeDelta)
		{
			if (this.activeHandHold.objectHeld == null)
			{
				if (this.wasHoldingHandhold)
				{
					this.playerRigidBody.velocity = Vector3.ClampMagnitude(this.secondLastPreHandholdVelocity, 5.5f * this.scale);
				}
				this.wasHoldingHandhold = false;
				return;
			}
			Vector3 vector = this.activeHandHold.objectHeld.TransformPoint(this.activeHandHold.localPositionHeld);
			Vector3 position = this.activeHandHold.grabber.transform.position;
			this.secondLastPreHandholdVelocity = this.lastPreHandholdVelocity;
			this.lastPreHandholdVelocity = this.playerRigidBody.velocity;
			this.wasHoldingHandhold = true;
			if (this.isHandHoldMoving)
			{
				this.lastPreHandholdVelocity = this.movingHandHoldReleaseVelocity;
				this.playerRigidBody.velocity = Vector3.zero;
				Vector3 vector2 = vector - position;
				this.playerRigidBody.transform.position += vector2;
				this.movingHandHoldReleaseVelocity = vector2 / timeDelta;
				Quaternion rotationDelta = this.activeHandHold.objectHeld.rotation * Quaternion.Inverse(this.lastHandHoldRotation);
				this.RotateWithSurface(rotationDelta, vector);
				this.lastHandHoldRotation = this.activeHandHold.objectHeld.rotation;
				return;
			}
			this.playerRigidBody.velocity = (vector - position) / timeDelta;
			if (this.activeHandHold.applyRotation)
			{
				this.turnParent.transform.RotateAround(vector, base.transform.up, this.activeHandHold.localRotationalOffset - (this.activeHandHold.grabber.transform.rotation.eulerAngles.y - this.activeHandHold.objectHeld.rotation.eulerAngles.y));
			}
		}

		// Token: 0x0600597A RID: 22906 RVA: 0x001C1799 File Offset: 0x001BF999
		[CompilerGenerated]
		internal static void <BeginClimbing>g__SnapAxis|407_0(ref float val, float maxDist)
		{
			if (val > maxDist)
			{
				val = maxDist;
				return;
			}
			if (val < -maxDist)
			{
				val = -maxDist;
			}
		}

		// Token: 0x040062AD RID: 25261
		private static GTPlayer _instance;

		// Token: 0x040062AE RID: 25262
		public static bool hasInstance;

		// Token: 0x040062AF RID: 25263
		public Camera mainCamera;

		// Token: 0x040062B0 RID: 25264
		public SphereCollider headCollider;

		// Token: 0x040062B1 RID: 25265
		public CapsuleCollider bodyCollider;

		// Token: 0x040062B2 RID: 25266
		private float bodyInitialRadius;

		// Token: 0x040062B3 RID: 25267
		private float bodyInitialHeight;

		// Token: 0x040062B4 RID: 25268
		private RaycastHit bodyHitInfo;

		// Token: 0x040062B5 RID: 25269
		private RaycastHit lastHitInfoHand;

		// Token: 0x040062B6 RID: 25270
		public Transform leftHandFollower;

		// Token: 0x040062B7 RID: 25271
		public Transform rightHandFollower;

		// Token: 0x040062B8 RID: 25272
		public Transform rightControllerTransform;

		// Token: 0x040062B9 RID: 25273
		public Transform leftControllerTransform;

		// Token: 0x040062BA RID: 25274
		public GorillaVelocityTracker rightHandCenterVelocityTracker;

		// Token: 0x040062BB RID: 25275
		public GorillaVelocityTracker leftHandCenterVelocityTracker;

		// Token: 0x040062BC RID: 25276
		public GorillaVelocityTracker rightInteractPointVelocityTracker;

		// Token: 0x040062BD RID: 25277
		public GorillaVelocityTracker leftInteractPointVelocityTracker;

		// Token: 0x040062BE RID: 25278
		public GorillaVelocityTracker bodyVelocityTracker;

		// Token: 0x040062BF RID: 25279
		public PlayerAudioManager audioManager;

		// Token: 0x040062C0 RID: 25280
		private Vector3 lastLeftHandPosition;

		// Token: 0x040062C1 RID: 25281
		private Vector3 lastRightHandPosition;

		// Token: 0x040062C2 RID: 25282
		private Quaternion lastLeftHandRotation;

		// Token: 0x040062C3 RID: 25283
		private Quaternion lastRightHandRotation;

		// Token: 0x040062C4 RID: 25284
		public Vector3 lastHeadPosition;

		// Token: 0x040062C5 RID: 25285
		private Vector3 lastRigidbodyPosition;

		// Token: 0x040062C6 RID: 25286
		private Rigidbody playerRigidBody;

		// Token: 0x040062C7 RID: 25287
		public int velocityHistorySize;

		// Token: 0x040062C8 RID: 25288
		public float maxArmLength = 1f;

		// Token: 0x040062C9 RID: 25289
		public float unStickDistance = 1f;

		// Token: 0x040062CA RID: 25290
		public float velocityLimit;

		// Token: 0x040062CB RID: 25291
		public float slideVelocityLimit;

		// Token: 0x040062CC RID: 25292
		public float maxJumpSpeed;

		// Token: 0x040062CD RID: 25293
		private float _jumpMultiplier;

		// Token: 0x040062CE RID: 25294
		public float minimumRaycastDistance = 0.05f;

		// Token: 0x040062CF RID: 25295
		public float defaultSlideFactor = 0.03f;

		// Token: 0x040062D0 RID: 25296
		public float slidingMinimum = 0.9f;

		// Token: 0x040062D1 RID: 25297
		public float defaultPrecision = 0.995f;

		// Token: 0x040062D2 RID: 25298
		public float teleportThresholdNoVel = 1f;

		// Token: 0x040062D3 RID: 25299
		public float frictionConstant = 1f;

		// Token: 0x040062D4 RID: 25300
		public float slideControl = 0.00425f;

		// Token: 0x040062D5 RID: 25301
		public float stickDepth = 0.01f;

		// Token: 0x040062D6 RID: 25302
		private Vector3[] velocityHistory;

		// Token: 0x040062D7 RID: 25303
		private Vector3[] slideAverageHistory;

		// Token: 0x040062D8 RID: 25304
		private int velocityIndex;

		// Token: 0x040062D9 RID: 25305
		private Vector3 currentVelocity;

		// Token: 0x040062DA RID: 25306
		private Vector3 averagedVelocity;

		// Token: 0x040062DB RID: 25307
		private Vector3 lastPosition;

		// Token: 0x040062DC RID: 25308
		public Vector3 rightHandOffset;

		// Token: 0x040062DD RID: 25309
		public Vector3 leftHandOffset;

		// Token: 0x040062DE RID: 25310
		public Quaternion rightHandRotOffset = Quaternion.identity;

		// Token: 0x040062DF RID: 25311
		public Quaternion leftHandRotOffset = Quaternion.identity;

		// Token: 0x040062E0 RID: 25312
		public Vector3 bodyOffset;

		// Token: 0x040062E1 RID: 25313
		public LayerMask locomotionEnabledLayers;

		// Token: 0x040062E2 RID: 25314
		public LayerMask waterLayer;

		// Token: 0x040062E3 RID: 25315
		public bool wasLeftHandColliding;

		// Token: 0x040062E4 RID: 25316
		public bool wasRightHandColliding;

		// Token: 0x040062E5 RID: 25317
		public bool wasHeadTouching;

		// Token: 0x040062E6 RID: 25318
		public int currentMaterialIndex;

		// Token: 0x040062E7 RID: 25319
		public bool isLeftHandSliding;

		// Token: 0x040062E8 RID: 25320
		public Vector3 leftHandSlideNormal;

		// Token: 0x040062E9 RID: 25321
		public bool isRightHandSliding;

		// Token: 0x040062EA RID: 25322
		public Vector3 rightHandSlideNormal;

		// Token: 0x040062EB RID: 25323
		public Vector3 headSlideNormal;

		// Token: 0x040062EC RID: 25324
		public float rightHandSlipPercentage;

		// Token: 0x040062ED RID: 25325
		public float leftHandSlipPercentage;

		// Token: 0x040062EE RID: 25326
		public float headSlipPercentage;

		// Token: 0x040062EF RID: 25327
		public bool wasLeftHandSliding;

		// Token: 0x040062F0 RID: 25328
		public bool wasRightHandSliding;

		// Token: 0x040062F1 RID: 25329
		public Vector3 rightHandHitPoint;

		// Token: 0x040062F2 RID: 25330
		public Vector3 leftHandHitPoint;

		// Token: 0x040062F3 RID: 25331
		[SerializeField]
		private Transform cosmeticsHeadTarget;

		// Token: 0x040062F4 RID: 25332
		[SerializeField]
		private float nativeScale = 1f;

		// Token: 0x040062F5 RID: 25333
		[SerializeField]
		private float scaleMultiplier = 1f;

		// Token: 0x040062F6 RID: 25334
		private NativeSizeChangerSettings activeSizeChangerSettings;

		// Token: 0x040062F7 RID: 25335
		public bool debugMovement;

		// Token: 0x040062F8 RID: 25336
		public bool disableMovement;

		// Token: 0x040062F9 RID: 25337
		[NonSerialized]
		public bool inOverlay;

		// Token: 0x040062FA RID: 25338
		[NonSerialized]
		public bool isUserPresent;

		// Token: 0x040062FB RID: 25339
		public GameObject turnParent;

		// Token: 0x040062FC RID: 25340
		public int leftHandMaterialTouchIndex;

		// Token: 0x040062FD RID: 25341
		public GorillaSurfaceOverride leftHandSurfaceOverride;

		// Token: 0x040062FE RID: 25342
		public RaycastHit leftHandHitInfo;

		// Token: 0x040062FF RID: 25343
		public int rightHandMaterialTouchIndex;

		// Token: 0x04006300 RID: 25344
		public GorillaSurfaceOverride rightHandSurfaceOverride;

		// Token: 0x04006301 RID: 25345
		public RaycastHit rightHandHitInfo;

		// Token: 0x04006302 RID: 25346
		public GorillaSurfaceOverride currentOverride;

		// Token: 0x04006303 RID: 25347
		public MaterialDatasSO materialDatasSO;

		// Token: 0x04006304 RID: 25348
		private bool isLeftHandColliding;

		// Token: 0x04006305 RID: 25349
		private bool isRightHandColliding;

		// Token: 0x04006306 RID: 25350
		private float degreesTurnedThisFrame;

		// Token: 0x04006307 RID: 25351
		private Vector3 bodyOffsetVector;

		// Token: 0x04006308 RID: 25352
		private Vector3 movementToProjectedAboveCollisionPlane;

		// Token: 0x04006309 RID: 25353
		private MeshCollider meshCollider;

		// Token: 0x0400630A RID: 25354
		private Mesh collidedMesh;

		// Token: 0x0400630B RID: 25355
		private GTPlayer.MaterialData foundMatData;

		// Token: 0x0400630C RID: 25356
		private string findMatName;

		// Token: 0x0400630D RID: 25357
		private int vertex1;

		// Token: 0x0400630E RID: 25358
		private int vertex2;

		// Token: 0x0400630F RID: 25359
		private int vertex3;

		// Token: 0x04006310 RID: 25360
		private List<int> trianglesList = new List<int>(1000000);

		// Token: 0x04006311 RID: 25361
		private Dictionary<Mesh, int[]> meshTrianglesDict = new Dictionary<Mesh, int[]>(128);

		// Token: 0x04006312 RID: 25362
		private int[] sharedMeshTris;

		// Token: 0x04006313 RID: 25363
		private float lastRealTime;

		// Token: 0x04006314 RID: 25364
		private float calcDeltaTime;

		// Token: 0x04006315 RID: 25365
		private float tempRealTime;

		// Token: 0x04006316 RID: 25366
		private Vector3 slideVelocity;

		// Token: 0x04006317 RID: 25367
		private Vector3 slideAverageNormal;

		// Token: 0x04006318 RID: 25368
		private RaycastHit tempHitInfo;

		// Token: 0x04006319 RID: 25369
		private RaycastHit junkHit;

		// Token: 0x0400631A RID: 25370
		private Vector3 firstPosition;

		// Token: 0x0400631B RID: 25371
		private RaycastHit tempIterativeHit;

		// Token: 0x0400631C RID: 25372
		private float maxSphereSize1;

		// Token: 0x0400631D RID: 25373
		private float maxSphereSize2;

		// Token: 0x0400631E RID: 25374
		private Collider[] overlapColliders = new Collider[10];

		// Token: 0x0400631F RID: 25375
		private int overlapAttempts;

		// Token: 0x04006320 RID: 25376
		private int touchPoints;

		// Token: 0x04006321 RID: 25377
		private float averageSlipPercentage;

		// Token: 0x04006322 RID: 25378
		private Vector3 surfaceDirection;

		// Token: 0x04006323 RID: 25379
		public float iceThreshold = 0.9f;

		// Token: 0x04006324 RID: 25380
		private float bodyMaxRadius;

		// Token: 0x04006325 RID: 25381
		public float bodyLerp = 0.17f;

		// Token: 0x04006326 RID: 25382
		private bool areBothTouching;

		// Token: 0x04006327 RID: 25383
		private float slideFactor;

		// Token: 0x04006328 RID: 25384
		[DebugOption]
		public bool didAJump;

		// Token: 0x04006329 RID: 25385
		private Renderer slideRenderer;

		// Token: 0x0400632A RID: 25386
		private RaycastHit[] rayCastNonAllocColliders;

		// Token: 0x0400632B RID: 25387
		private Vector3[] crazyCheckVectors;

		// Token: 0x0400632C RID: 25388
		private RaycastHit emptyHit;

		// Token: 0x0400632D RID: 25389
		private int bufferCount;

		// Token: 0x0400632E RID: 25390
		private Vector3 lastOpenHeadPosition;

		// Token: 0x0400632F RID: 25391
		private List<Material> tempMaterialArray = new List<Material>(16);

		// Token: 0x04006330 RID: 25392
		private int leftSlipSetToMaxFrameIdx = -1;

		// Token: 0x04006331 RID: 25393
		private int rightSlipSetToMaxFrameIdx = -1;

		// Token: 0x04006332 RID: 25394
		private const float CameraFarClipDefault = 500f;

		// Token: 0x04006333 RID: 25395
		private const float CameraNearClipDefault = 0.01f;

		// Token: 0x04006334 RID: 25396
		private const float CameraNearClipTiny = 0.002f;

		// Token: 0x04006335 RID: 25397
		private Dictionary<GameObject, PhysicMaterial> bodyTouchedSurfaces;

		// Token: 0x04006336 RID: 25398
		private bool primaryButtonPressed = true;

		// Token: 0x04006337 RID: 25399
		[Header("Swimming")]
		public PlayerSwimmingParameters swimmingParams;

		// Token: 0x04006338 RID: 25400
		public WaterParameters waterParams;

		// Token: 0x04006339 RID: 25401
		public List<GTPlayer.LiquidProperties> liquidPropertiesList = new List<GTPlayer.LiquidProperties>(16);

		// Token: 0x0400633A RID: 25402
		public bool debugDrawSwimming;

		// Token: 0x0400633B RID: 25403
		[Header("Slam/Hit effects")]
		public GameObject wizardStaffSlamEffects;

		// Token: 0x0400633C RID: 25404
		public GameObject geodeHitEffects;

		// Token: 0x0400633D RID: 25405
		[Header("Freeze Tag")]
		public float freezeTagHandSlidePercent = 0.88f;

		// Token: 0x0400633E RID: 25406
		public bool debugFreezeTag;

		// Token: 0x0400633F RID: 25407
		public float frozenBodyBuoyancyFactor = 1.5f;

		// Token: 0x04006341 RID: 25409
		[Space]
		private WaterVolume leftHandWaterVolume;

		// Token: 0x04006342 RID: 25410
		private WaterVolume rightHandWaterVolume;

		// Token: 0x04006343 RID: 25411
		private WaterVolume.SurfaceQuery leftHandWaterSurface;

		// Token: 0x04006344 RID: 25412
		private WaterVolume.SurfaceQuery rightHandWaterSurface;

		// Token: 0x04006345 RID: 25413
		private Vector3 swimmingVelocity = Vector3.zero;

		// Token: 0x04006346 RID: 25414
		private WaterVolume.SurfaceQuery waterSurfaceForHead;

		// Token: 0x04006347 RID: 25415
		private bool bodyInWater;

		// Token: 0x04006348 RID: 25416
		private bool headInWater;

		// Token: 0x04006349 RID: 25417
		private bool audioSetToUnderwater;

		// Token: 0x0400634A RID: 25418
		private float buoyancyExtension;

		// Token: 0x0400634B RID: 25419
		private float lastWaterSurfaceJumpTimeLeft = -1f;

		// Token: 0x0400634C RID: 25420
		private float lastWaterSurfaceJumpTimeRight = -1f;

		// Token: 0x0400634D RID: 25421
		private float waterSurfaceJumpCooldown = 0.1f;

		// Token: 0x0400634E RID: 25422
		private float leftHandNonDiveHapticsAmount;

		// Token: 0x0400634F RID: 25423
		private float rightHandNonDiveHapticsAmount;

		// Token: 0x04006350 RID: 25424
		private List<WaterVolume> headOverlappingWaterVolumes = new List<WaterVolume>(16);

		// Token: 0x04006351 RID: 25425
		private List<WaterVolume> bodyOverlappingWaterVolumes = new List<WaterVolume>(16);

		// Token: 0x04006352 RID: 25426
		private List<WaterCurrent> activeWaterCurrents = new List<WaterCurrent>(16);

		// Token: 0x04006353 RID: 25427
		private Quaternion playerRotationOverride;

		// Token: 0x04006354 RID: 25428
		private int playerRotationOverrideFrame = -1;

		// Token: 0x04006355 RID: 25429
		private float playerRotationOverrideDecayRate = Mathf.Exp(1.5f);

		// Token: 0x04006357 RID: 25431
		private ContactPoint[] bodyCollisionContacts = new ContactPoint[8];

		// Token: 0x04006358 RID: 25432
		private int bodyCollisionContactsCount;

		// Token: 0x04006359 RID: 25433
		private ContactPoint bodyGroundContact;

		// Token: 0x0400635A RID: 25434
		private float bodyGroundContactTime;

		// Token: 0x0400635B RID: 25435
		private const float movingSurfaceVelocityLimit = 40f;

		// Token: 0x0400635C RID: 25436
		private bool exitMovingSurface;

		// Token: 0x0400635D RID: 25437
		private float exitMovingSurfaceThreshold = 6f;

		// Token: 0x0400635E RID: 25438
		private bool isClimbableMoving;

		// Token: 0x0400635F RID: 25439
		private Quaternion lastClimbableRotation;

		// Token: 0x04006360 RID: 25440
		private int lastAttachedToMovingSurfaceFrame;

		// Token: 0x04006361 RID: 25441
		private const int MIN_FRAMES_OFF_SURFACE_TO_DETACH = 3;

		// Token: 0x04006362 RID: 25442
		private bool isHandHoldMoving;

		// Token: 0x04006363 RID: 25443
		private Quaternion lastHandHoldRotation;

		// Token: 0x04006364 RID: 25444
		private Vector3 movingHandHoldReleaseVelocity;

		// Token: 0x04006365 RID: 25445
		private GTPlayer.MovingSurfaceContactPoint lastMovingSurfaceContact;

		// Token: 0x04006366 RID: 25446
		private int lastMovingSurfaceID = -1;

		// Token: 0x04006367 RID: 25447
		private BuilderPiece lastMonkeBlock;

		// Token: 0x04006368 RID: 25448
		private Quaternion lastMovingSurfaceRot;

		// Token: 0x04006369 RID: 25449
		private RaycastHit lastMovingSurfaceHit;

		// Token: 0x0400636A RID: 25450
		private Vector3 lastMovingSurfaceTouchLocal;

		// Token: 0x0400636B RID: 25451
		private Vector3 lastMovingSurfaceTouchWorld;

		// Token: 0x0400636C RID: 25452
		private Vector3 movingSurfaceOffset;

		// Token: 0x0400636D RID: 25453
		private bool wasMovingSurfaceMonkeBlock;

		// Token: 0x0400636E RID: 25454
		private Vector3 lastMovingSurfaceVelocity;

		// Token: 0x0400636F RID: 25455
		private bool wasBodyOnGround;

		// Token: 0x04006370 RID: 25456
		private BasePlatform currentPlatform;

		// Token: 0x04006371 RID: 25457
		private BasePlatform lastPlatformTouched;

		// Token: 0x04006372 RID: 25458
		private Vector3 lastFrameTouchPosLocal;

		// Token: 0x04006373 RID: 25459
		private Vector3 lastFrameTouchPosWorld;

		// Token: 0x04006374 RID: 25460
		private bool lastFrameHasValidTouchPos;

		// Token: 0x04006375 RID: 25461
		private Vector3 refMovement = Vector3.zero;

		// Token: 0x04006376 RID: 25462
		private Vector3 platformTouchOffset;

		// Token: 0x04006377 RID: 25463
		private Vector3 debugLastRightHandPosition;

		// Token: 0x04006378 RID: 25464
		private Vector3 debugPlatformDeltaPosition;

		// Token: 0x04006379 RID: 25465
		public double tempFreezeRightHandEnableTime;

		// Token: 0x0400637A RID: 25466
		public double tempFreezeLeftHandEnableTime;

		// Token: 0x0400637B RID: 25467
		private const float climbingMaxThrowSpeed = 5.5f;

		// Token: 0x0400637C RID: 25468
		private const float climbHelperSmoothSnapSpeed = 12f;

		// Token: 0x0400637D RID: 25469
		[NonSerialized]
		public bool isClimbing;

		// Token: 0x0400637E RID: 25470
		private GorillaClimbable currentClimbable;

		// Token: 0x0400637F RID: 25471
		private GorillaHandClimber currentClimber;

		// Token: 0x04006380 RID: 25472
		private Vector3 climbHelperTargetPos = Vector3.zero;

		// Token: 0x04006381 RID: 25473
		private Transform climbHelper;

		// Token: 0x04006382 RID: 25474
		private GorillaRopeSwing currentSwing;

		// Token: 0x04006383 RID: 25475
		private GorillaZipline currentZipline;

		// Token: 0x04006384 RID: 25476
		[SerializeField]
		private ConnectedControllerHandler controllerState;

		// Token: 0x04006385 RID: 25477
		public int sizeLayerMask;

		// Token: 0x04006386 RID: 25478
		public bool InReportMenu;

		// Token: 0x04006387 RID: 25479
		private LayerChanger layerChanger;

		// Token: 0x0400638A RID: 25482
		private bool hasCorrectedForTracking;

		// Token: 0x0400638B RID: 25483
		private float halloweenLevitationStrength;

		// Token: 0x0400638C RID: 25484
		private float halloweenLevitationFullStrengthDuration;

		// Token: 0x0400638D RID: 25485
		private float halloweenLevitationTotalDuration = 1f;

		// Token: 0x0400638E RID: 25486
		private float halloweenLevitationBonusStrength;

		// Token: 0x0400638F RID: 25487
		private float halloweenLevitateBonusOffAtYSpeed;

		// Token: 0x04006390 RID: 25488
		private float halloweenLevitateBonusFullAtYSpeed = 1f;

		// Token: 0x04006391 RID: 25489
		private float lastTouchedGroundTimestamp;

		// Token: 0x04006392 RID: 25490
		private bool teleportToTrain;

		// Token: 0x04006393 RID: 25491
		public bool isAttachedToTrain;

		// Token: 0x04006394 RID: 25492
		private bool stuckLeft;

		// Token: 0x04006395 RID: 25493
		private bool stuckRight;

		// Token: 0x04006396 RID: 25494
		private float lastScale;

		// Token: 0x04006397 RID: 25495
		private Vector3 currentSlopDirection;

		// Token: 0x04006398 RID: 25496
		private Vector3 lastSlopeDirection = Vector3.zero;

		// Token: 0x04006399 RID: 25497
		private Dictionary<Object, Action<GTPlayer>> gravityOverrides = new Dictionary<Object, Action<GTPlayer>>();

		// Token: 0x0400639C RID: 25500
		private int hoverAllowedCount;

		// Token: 0x0400639D RID: 25501
		[Header("Hoverboard Proto")]
		[SerializeField]
		private float hoverIdealHeight = 0.5f;

		// Token: 0x0400639E RID: 25502
		[SerializeField]
		private float hoverCarveSidewaysSpeedLossFactor = 1f;

		// Token: 0x0400639F RID: 25503
		[SerializeField]
		private AnimationCurve hoverCarveAngleResponsiveness;

		// Token: 0x040063A0 RID: 25504
		[SerializeField]
		private HoverboardVisual hoverboardVisual;

		// Token: 0x040063A1 RID: 25505
		[SerializeField]
		private float sidewaysDrag = 0.1f;

		// Token: 0x040063A2 RID: 25506
		[SerializeField]
		private float hoveringSlowSpeed = 0.1f;

		// Token: 0x040063A3 RID: 25507
		[SerializeField]
		private float hoveringSlowStoppingFactor = 0.95f;

		// Token: 0x040063A4 RID: 25508
		[SerializeField]
		private float hoverboardPaddleBoostMultiplier = 0.1f;

		// Token: 0x040063A5 RID: 25509
		[SerializeField]
		private float hoverboardPaddleBoostMax = 10f;

		// Token: 0x040063A6 RID: 25510
		[SerializeField]
		private float hoverboardBoostGracePeriod = 1f;

		// Token: 0x040063A7 RID: 25511
		[SerializeField]
		private float hoverBodyHasCollisionsOutsideRadius = 0.5f;

		// Token: 0x040063A8 RID: 25512
		[SerializeField]
		private float hoverBodyCollisionRadiusUpOffset = 0.2f;

		// Token: 0x040063A9 RID: 25513
		[SerializeField]
		private float hoverGeneralUpwardForce = 8f;

		// Token: 0x040063AA RID: 25514
		[SerializeField]
		private float hoverTiltAdjustsForwardFactor = 0.2f;

		// Token: 0x040063AB RID: 25515
		[SerializeField]
		private float hoverMinGrindSpeed = 1f;

		// Token: 0x040063AC RID: 25516
		[SerializeField]
		private float hoverSlamJumpStrengthFactor = 25f;

		// Token: 0x040063AD RID: 25517
		[SerializeField]
		private float hoverMaxPaddleSpeed = 35f;

		// Token: 0x040063AE RID: 25518
		[SerializeField]
		private HoverboardAudio hoverboardAudio;

		// Token: 0x040063AF RID: 25519
		private bool hasHoverPoint;

		// Token: 0x040063B0 RID: 25520
		private float boostEnabledUntilTimestamp;

		// Token: 0x040063B1 RID: 25521
		private GTPlayer.HoverBoardCast[] hoverboardCasts = new GTPlayer.HoverBoardCast[]
		{
			new GTPlayer.HoverBoardCast
			{
				localOrigin = new Vector3(0f, 1f, 0.36f),
				localDirection = Vector3.down,
				distance = 1f,
				sphereRadius = 0.2f,
				intersectToVelocityCap = 0.1f
			},
			new GTPlayer.HoverBoardCast
			{
				localOrigin = new Vector3(0f, 0.05f, 0.36f),
				localDirection = Vector3.forward,
				distance = 0.25f,
				sphereRadius = 0.01f,
				intersectToVelocityCap = 0f,
				isSolid = true
			},
			new GTPlayer.HoverBoardCast
			{
				localOrigin = new Vector3(0f, 0.05f, -0.1f),
				localDirection = -Vector3.forward,
				distance = 0.24f,
				sphereRadius = 0.01f,
				intersectToVelocityCap = 0f,
				isSolid = true
			}
		};

		// Token: 0x040063B2 RID: 25522
		private Vector3 hoverboardPlayerLocalPos;

		// Token: 0x040063B3 RID: 25523
		private Quaternion hoverboardPlayerLocalRot;

		// Token: 0x040063B4 RID: 25524
		private bool didHoverLastFrame;

		// Token: 0x040063B5 RID: 25525
		private GTPlayer.HandHoldState activeHandHold;

		// Token: 0x040063B6 RID: 25526
		private GTPlayer.HandHoldState secondaryHandHold;

		// Token: 0x040063B7 RID: 25527
		private bool rightHandHolding;

		// Token: 0x040063B8 RID: 25528
		private bool leftHandHolding;

		// Token: 0x040063B9 RID: 25529
		public PhysicMaterial slipperyMaterial;

		// Token: 0x040063BA RID: 25530
		private bool wasHoldingHandhold;

		// Token: 0x040063BB RID: 25531
		private Vector3 secondLastPreHandholdVelocity;

		// Token: 0x040063BC RID: 25532
		private Vector3 lastPreHandholdVelocity;

		// Token: 0x040063BD RID: 25533
		[Header("Native Scale Adjustment")]
		[SerializeField]
		private AnimationCurve nativeScaleMagnitudeAdjustmentFactor;

		// Token: 0x02000E08 RID: 3592
		private enum MovingSurfaceContactPoint
		{
			// Token: 0x040063BF RID: 25535
			NONE,
			// Token: 0x040063C0 RID: 25536
			RIGHT,
			// Token: 0x040063C1 RID: 25537
			LEFT,
			// Token: 0x040063C2 RID: 25538
			BODY
		}

		// Token: 0x02000E09 RID: 3593
		[Serializable]
		public struct MaterialData
		{
			// Token: 0x040063C3 RID: 25539
			public string matName;

			// Token: 0x040063C4 RID: 25540
			public bool overrideAudio;

			// Token: 0x040063C5 RID: 25541
			public AudioClip audio;

			// Token: 0x040063C6 RID: 25542
			public bool overrideSlidePercent;

			// Token: 0x040063C7 RID: 25543
			public float slidePercent;

			// Token: 0x040063C8 RID: 25544
			public int surfaceEffectIndex;
		}

		// Token: 0x02000E0A RID: 3594
		[Serializable]
		public struct LiquidProperties
		{
			// Token: 0x040063C9 RID: 25545
			[Range(0f, 2f)]
			[Tooltip("0: no resistance just like air, 1: full resistance like solid geometry")]
			public float resistance;

			// Token: 0x040063CA RID: 25546
			[Range(0f, 3f)]
			[Tooltip("0: no buoyancy. 1: Fully compensates gravity. 2: net force is upwards equal to gravity")]
			public float buoyancy;

			// Token: 0x040063CB RID: 25547
			[Range(0f, 3f)]
			[Tooltip("Damping Half-life Multiplier")]
			public float dampingFactor;

			// Token: 0x040063CC RID: 25548
			[Range(0f, 1f)]
			public float surfaceJumpFactor;
		}

		// Token: 0x02000E0B RID: 3595
		public enum LiquidType
		{
			// Token: 0x040063CE RID: 25550
			Water,
			// Token: 0x040063CF RID: 25551
			Lava
		}

		// Token: 0x02000E0C RID: 3596
		private struct HoverBoardCast
		{
			// Token: 0x040063D0 RID: 25552
			public Vector3 localOrigin;

			// Token: 0x040063D1 RID: 25553
			public Vector3 localDirection;

			// Token: 0x040063D2 RID: 25554
			public float sphereRadius;

			// Token: 0x040063D3 RID: 25555
			public float distance;

			// Token: 0x040063D4 RID: 25556
			public float intersectToVelocityCap;

			// Token: 0x040063D5 RID: 25557
			public bool isSolid;

			// Token: 0x040063D6 RID: 25558
			public bool didHit;

			// Token: 0x040063D7 RID: 25559
			public Vector3 pointHit;

			// Token: 0x040063D8 RID: 25560
			public Vector3 normalHit;
		}

		// Token: 0x02000E0D RID: 3597
		private struct HandHoldState
		{
			// Token: 0x040063D9 RID: 25561
			public GorillaGrabber grabber;

			// Token: 0x040063DA RID: 25562
			public Transform objectHeld;

			// Token: 0x040063DB RID: 25563
			public Vector3 localPositionHeld;

			// Token: 0x040063DC RID: 25564
			public float localRotationalOffset;

			// Token: 0x040063DD RID: 25565
			public bool applyRotation;
		}
	}
}
