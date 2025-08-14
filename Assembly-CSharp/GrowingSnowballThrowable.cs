using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020000EA RID: 234
public class GrowingSnowballThrowable : SnowballThrowable
{
	// Token: 0x17000073 RID: 115
	// (get) Token: 0x060005D0 RID: 1488 RVA: 0x000217D6 File Offset: 0x0001F9D6
	public int SizeLevel
	{
		get
		{
			return this.sizeLevel;
		}
	}

	// Token: 0x17000074 RID: 116
	// (get) Token: 0x060005D1 RID: 1489 RVA: 0x000217DE File Offset: 0x0001F9DE
	public int MaxSizeLevel
	{
		get
		{
			return Mathf.Max(this.snowballSizeLevels.Count - 1, 0);
		}
	}

	// Token: 0x17000075 RID: 117
	// (get) Token: 0x060005D2 RID: 1490 RVA: 0x000217F4 File Offset: 0x0001F9F4
	public float CurrentSnowballRadius
	{
		get
		{
			if (this.snowballSizeLevels.Count > 0 && this.sizeLevel > -1 && this.sizeLevel < this.snowballSizeLevels.Count)
			{
				return this.snowballSizeLevels[this.sizeLevel].snowballScale * this.modelRadius * base.transform.lossyScale.x;
			}
			return this.modelRadius * base.transform.lossyScale.x;
		}
	}

	// Token: 0x060005D3 RID: 1491 RVA: 0x00021874 File Offset: 0x0001FA74
	protected override void Awake()
	{
		base.Awake();
		if (NetworkSystem.Instance != null)
		{
			NetworkSystem.Instance.OnMultiplayerStarted += this.StartedMultiplayerSession;
		}
		else
		{
			Debug.LogError("NetworkSystem.Instance was null in SnowballThrowable Awake");
		}
		VRRigCache.OnRigActivated += this.VRRigActivated;
		VRRigCache.OnRigDeactivated += this.VRRigDeactivated;
	}

	// Token: 0x060005D4 RID: 1492 RVA: 0x000218E4 File Offset: 0x0001FAE4
	public override void OnEnable()
	{
		base.OnEnable();
		this.snowballModelParentTransform.localPosition = this.modelParentOffset;
		this.snowballModelTransform.localPosition = this.modelOffset;
		this.otherHandSnowball = (this.isLeftHanded ? (EquipmentInteractor.instance.rightHandHeldEquipment as GrowingSnowballThrowable) : (EquipmentInteractor.instance.leftHandHeldEquipment as GrowingSnowballThrowable));
		if (Time.time > this.maintainSizeLevelUntilLocalTime)
		{
			this.SetSizeLevelLocal(0);
		}
		this.CreatePhotonEventsIfNull();
	}

	// Token: 0x060005D5 RID: 1493 RVA: 0x00021965 File Offset: 0x0001FB65
	protected override void OnDestroy()
	{
		this.DestroyPhotonEvents();
	}

	// Token: 0x060005D6 RID: 1494 RVA: 0x00021970 File Offset: 0x0001FB70
	private void VRRigActivated(RigContainer rigContainer)
	{
		this.targetRig = base.GetComponentInParent<VRRig>(true);
		this.isOfflineRig = (this.targetRig != null && this.targetRig.isOfflineVRRig);
		if (rigContainer.Rig == this.targetRig)
		{
			this.CreatePhotonEventsIfNull();
		}
	}

	// Token: 0x060005D7 RID: 1495 RVA: 0x000219C5 File Offset: 0x0001FBC5
	private void VRRigDeactivated(RigContainer rigContainer)
	{
		if (rigContainer.Rig == this.targetRig)
		{
			this.DestroyPhotonEvents();
		}
	}

	// Token: 0x060005D8 RID: 1496 RVA: 0x000219E0 File Offset: 0x0001FBE0
	private void StartedMultiplayerSession()
	{
		this.targetRig = base.GetComponentInParent<VRRig>(true);
		this.isOfflineRig = (this.targetRig != null && this.targetRig.isOfflineVRRig);
		if (this.isOfflineRig)
		{
			this.DestroyPhotonEvents();
			this.CreatePhotonEventsIfNull();
		}
	}

	// Token: 0x060005D9 RID: 1497 RVA: 0x00021A30 File Offset: 0x0001FC30
	private void CreatePhotonEventsIfNull()
	{
		if (this.targetRig == null)
		{
			this.targetRig = base.GetComponentInParent<VRRig>(true);
			this.isOfflineRig = (this.targetRig != null && this.targetRig.isOfflineVRRig);
		}
		if (this.targetRig == null || this.targetRig.netView == null)
		{
			return;
		}
		if (this.changeSizeEvent == null)
		{
			"SnowballThrowable" + (this.isLeftHanded ? "ChangeSizeEventLeft" : "ChangeSizeEventRight") + this.targetRig.netView.ViewID.ToString();
			int eventId = StaticHash.Compute("SnowballThrowable", this.isLeftHanded ? "ChangeSizeEventLeft" : "ChangeSizeEventRight", this.targetRig.netView.ViewID.ToString());
			this.changeSizeEvent = new PhotonEvent(eventId);
			this.changeSizeEvent.reliable = true;
			this.changeSizeEvent += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.ChangeSizeEventReceiver);
		}
		if (this.snowballThrowEvent == null)
		{
			"SnowballThrowable" + (this.isLeftHanded ? "SnowballThrowEventLeft" : "SnowballThrowEventRight") + this.targetRig.netView.ViewID.ToString();
			int eventId2 = StaticHash.Compute("SnowballThrowable", this.isLeftHanded ? "SnowballThrowEventLeft" : "SnowballThrowEventRight", this.targetRig.netView.ViewID.ToString());
			this.snowballThrowEvent = new PhotonEvent(eventId2);
			this.snowballThrowEvent.reliable = true;
			this.snowballThrowEvent += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.SnowballThrowEventReceiver);
		}
	}

	// Token: 0x060005DA RID: 1498 RVA: 0x00021C04 File Offset: 0x0001FE04
	private void DestroyPhotonEvents()
	{
		if (this.changeSizeEvent != null)
		{
			this.changeSizeEvent -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.ChangeSizeEventReceiver);
			this.changeSizeEvent.Dispose();
			this.changeSizeEvent = null;
		}
		if (this.snowballThrowEvent != null)
		{
			this.snowballThrowEvent -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.SnowballThrowEventReceiver);
			this.snowballThrowEvent.Dispose();
			this.snowballThrowEvent = null;
		}
	}

	// Token: 0x060005DB RID: 1499 RVA: 0x00021C8B File Offset: 0x0001FE8B
	public void IncreaseSize(int increase)
	{
		this.SetSizeLevelAuthority(this.sizeLevel + increase);
	}

	// Token: 0x060005DC RID: 1500 RVA: 0x00021C9C File Offset: 0x0001FE9C
	private void SetSizeLevelAuthority(int sizeLevel)
	{
		if (this.targetRig != null && this.targetRig.creator != null && this.targetRig.creator.IsLocal)
		{
			int validSizeLevel = this.GetValidSizeLevel(sizeLevel);
			if (validSizeLevel > this.sizeLevel)
			{
				this.sizeIncreaseSoundBankPlayer.Play();
			}
			this.SetSizeLevelLocal(validSizeLevel);
			PhotonEvent photonEvent = this.changeSizeEvent;
			if (photonEvent == null)
			{
				return;
			}
			photonEvent.RaiseOthers(new object[]
			{
				validSizeLevel
			});
		}
	}

	// Token: 0x060005DD RID: 1501 RVA: 0x00021D18 File Offset: 0x0001FF18
	private int GetValidSizeLevel(int inputSizeLevel)
	{
		int max = Mathf.Max(this.snowballSizeLevels.Count - 1, 0);
		return Mathf.Clamp(inputSizeLevel, 0, max);
	}

	// Token: 0x060005DE RID: 1502 RVA: 0x00021D44 File Offset: 0x0001FF44
	private void SetSizeLevelLocal(int sizeLevel)
	{
		int validSizeLevel = this.GetValidSizeLevel(sizeLevel);
		if (validSizeLevel >= 0 && validSizeLevel != this.sizeLevel)
		{
			this.sizeLevel = validSizeLevel;
			this.snowballModelParentTransform.localScale = Vector3.one * this.snowballSizeLevels[this.sizeLevel].snowballScale;
		}
	}

	// Token: 0x060005DF RID: 1503 RVA: 0x00021D98 File Offset: 0x0001FF98
	private void ChangeSizeEventReceiver(int sender, int receiver, object[] args, PhotonMessageInfoWrapped info)
	{
		if (sender != receiver)
		{
			return;
		}
		if (args == null || args.Length < 1)
		{
			return;
		}
		int num = (this.targetRig != null && this.targetRig.gameObject.activeInHierarchy && this.targetRig.netView != null && this.targetRig.netView.Owner != null) ? this.targetRig.netView.Owner.ActorNumber : -1;
		if (info.senderID != num)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "ChangeSizeEventReceiver");
		int num2 = (int)args[0];
		if (this.GetValidSizeLevel(num2) > this.sizeLevel)
		{
			this.sizeIncreaseSoundBankPlayer.Play();
		}
		this.SetSizeLevelLocal(num2);
		if (!base.gameObject.activeSelf)
		{
			this.maintainSizeLevelUntilLocalTime = Time.time + 0.1f;
		}
	}

	// Token: 0x060005E0 RID: 1504 RVA: 0x00021E74 File Offset: 0x00020074
	private void SnowballThrowEventReceiver(int sender, int receiver, object[] args, PhotonMessageInfoWrapped info)
	{
		if (sender != receiver)
		{
			return;
		}
		if (args == null || args.Length < 3)
		{
			return;
		}
		if (this.targetRig.IsNull() || !this.targetRig.gameObject.activeSelf)
		{
			return;
		}
		NetPlayer creator = this.targetRig.creator;
		if (info.senderID != this.targetRig.creator.ActorNumber)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "SnowballThrowEventReceiver");
		if (!this.snowballThrowCallLimit.CheckCallTime(Time.time))
		{
			return;
		}
		object obj = args[0];
		if (obj is Vector3)
		{
			Vector3 vector = (Vector3)obj;
			obj = args[1];
			if (obj is Vector3)
			{
				Vector3 inVel = (Vector3)obj;
				obj = args[2];
				if (obj is int)
				{
					int index = (int)obj;
					Vector3 velocity = this.targetRig.ClampVelocityRelativeToPlayerSafe(inVel, 50f);
					float x = this.snowballModelTransform.lossyScale.x;
					float num = 10000f;
					if (!vector.IsValid(num) || !this.targetRig.IsPositionInRange(vector, 4f))
					{
						return;
					}
					this.LaunchSnowballRemote(vector, velocity, x, index, info);
					return;
				}
			}
		}
	}

	// Token: 0x060005E1 RID: 1505 RVA: 0x00021F94 File Offset: 0x00020194
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (GrowingSnowballThrowable.twoHandedSnowballGrowing)
		{
			if (this.otherHandSnowball != null && this.otherHandSnowball.isActiveAndEnabled)
			{
				IHoldableObject holdableObject = this.isLeftHanded ? EquipmentInteractor.instance.rightHandHeldEquipment : EquipmentInteractor.instance.leftHandHeldEquipment;
				if (holdableObject != null && this.otherHandSnowball != (GrowingSnowballThrowable)holdableObject)
				{
					this.otherHandSnowball = null;
					return;
				}
				float num = this.otherHandSnowball.CurrentSnowballRadius + this.CurrentSnowballRadius;
				if (this.SizeLevel < this.MaxSizeLevel && this.otherHandSnowball.SizeLevel < this.otherHandSnowball.MaxSizeLevel && (this.otherHandSnowball.snowballModelTransform.position - this.snowballModelTransform.position).sqrMagnitude < num * num)
				{
					int num2 = this.SizeLevel - this.otherHandSnowball.SizeLevel;
					float magnitude = this.velocityEstimator.linearVelocity.magnitude;
					float magnitude2 = this.otherHandSnowball.velocityEstimator.linearVelocity.magnitude;
					bool flag;
					if (Mathf.Abs(magnitude - magnitude2) > this.combineBasedOnSpeedThreshold || num2 == 0)
					{
						flag = (magnitude > magnitude2);
					}
					else
					{
						flag = (num2 < 0);
					}
					if (flag)
					{
						this.otherHandSnowball.IncreaseSize(this.sizeLevel + 1);
						GorillaTagger.Instance.StartVibration(!this.isLeftHanded, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
						base.SetSnowballActiveLocal(false);
						return;
					}
					this.IncreaseSize(this.otherHandSnowball.SizeLevel + 1);
					GorillaTagger.Instance.StartVibration(this.isLeftHanded, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
					this.otherHandSnowball.SetSnowballActiveLocal(false);
					return;
				}
			}
			else
			{
				this.otherHandSnowball = null;
			}
		}
	}

	// Token: 0x060005E2 RID: 1506 RVA: 0x0002219A File Offset: 0x0002039A
	protected override void OnSnowballRelease()
	{
		if (base.isActiveAndEnabled)
		{
			this.PerformSnowballThrowAuthority();
		}
	}

	// Token: 0x060005E3 RID: 1507 RVA: 0x000221AC File Offset: 0x000203AC
	protected override void PerformSnowballThrowAuthority()
	{
		if (!(this.targetRig != null) || this.targetRig.creator == null || !this.targetRig.creator.IsLocal)
		{
			return;
		}
		Vector3 b = Vector3.zero;
		Rigidbody component = GorillaTagger.Instance.GetComponent<Rigidbody>();
		if (component != null)
		{
			b = component.velocity;
		}
		Vector3 a = this.velocityEstimator.linearVelocity - b;
		float magnitude = a.magnitude;
		if (magnitude > 0.001f)
		{
			float num = Mathf.Clamp(magnitude * this.linSpeedMultiplier, 0f, this.maxLinSpeed);
			a *= num / magnitude;
		}
		Vector3 vector = a + b;
		this.targetRig.GetThrowableProjectileColor(this.isLeftHanded);
		Transform transform = this.snowballModelTransform;
		Vector3 position = transform.position;
		float x = transform.lossyScale.x;
		SlingshotProjectile slingshotProjectile = this.LaunchSnowballLocal(position, vector, x);
		base.SetSnowballActiveLocal(false);
		if (this.randModelIndex > -1 && this.randModelIndex < this.localModels.Count && this.localModels[this.randModelIndex].destroyAfterRelease)
		{
			slingshotProjectile.DestroyAfterRelease();
		}
		PhotonEvent photonEvent = this.snowballThrowEvent;
		if (photonEvent == null)
		{
			return;
		}
		photonEvent.RaiseOthers(new object[]
		{
			position,
			vector,
			slingshotProjectile.myProjectileCount
		});
	}

	// Token: 0x060005E4 RID: 1508 RVA: 0x00022310 File Offset: 0x00020510
	protected virtual SlingshotProjectile LaunchSnowballLocal(Vector3 location, Vector3 velocity, float scale)
	{
		return this.LaunchSnowballLocal(location, velocity, scale, false, Color.white);
	}

	// Token: 0x060005E5 RID: 1509 RVA: 0x00022324 File Offset: 0x00020524
	protected override SlingshotProjectile LaunchSnowballLocal(Vector3 location, Vector3 velocity, float scale, bool randomizeColour, Color colour)
	{
		SlingshotProjectile slingshotProjectile = this.SpawnGrowingSnowball(ref velocity, scale);
		int projectileCount = ProjectileTracker.AddAndIncrementLocalProjectile(slingshotProjectile, velocity, location, scale);
		slingshotProjectile.Launch(location, velocity, NetworkSystem.Instance.LocalPlayer, false, false, projectileCount, scale, randomizeColour, colour);
		if (string.IsNullOrEmpty(this.throwEventName))
		{
			PlayerGameEvents.LaunchedProjectile(this.projectilePrefab.name);
		}
		else
		{
			PlayerGameEvents.LaunchedProjectile(this.throwEventName);
		}
		slingshotProjectile.OnImpact += this.OnProjectileImpact;
		return slingshotProjectile;
	}

	// Token: 0x060005E6 RID: 1510 RVA: 0x0002239B File Offset: 0x0002059B
	protected virtual SlingshotProjectile LaunchSnowballRemote(Vector3 location, Vector3 velocity, float scale, int index, PhotonMessageInfoWrapped info)
	{
		return this.LaunchSnowballRemote(location, velocity, scale, index, false, Color.white, info);
	}

	// Token: 0x060005E7 RID: 1511 RVA: 0x000223B0 File Offset: 0x000205B0
	protected virtual SlingshotProjectile LaunchSnowballRemote(Vector3 location, Vector3 velocity, float scale, int index, bool randomizeColour, Color colour, PhotonMessageInfoWrapped info)
	{
		SlingshotProjectile slingshotProjectile = this.SpawnGrowingSnowball(ref velocity, scale);
		ProjectileTracker.AddRemotePlayerProjectile(info.Sender, slingshotProjectile, index, info.SentServerTime, velocity, location, scale);
		slingshotProjectile.Launch(location, velocity, info.Sender, false, false, index, scale, randomizeColour, Color.white);
		if (string.IsNullOrEmpty(this.throwEventName))
		{
			PlayerGameEvents.LaunchedProjectile(this.projectilePrefab.name);
		}
		else
		{
			PlayerGameEvents.LaunchedProjectile(this.throwEventName);
		}
		slingshotProjectile.OnImpact += this.OnProjectileImpact;
		return slingshotProjectile;
	}

	// Token: 0x060005E8 RID: 1512 RVA: 0x0002243C File Offset: 0x0002063C
	private SlingshotProjectile SpawnGrowingSnowball(ref Vector3 velocity, float scale)
	{
		SlingshotProjectile component = ObjectPools.instance.Instantiate(this.randomModelSelection ? this.localModels[this.randModelIndex].projectilePrefab : this.projectilePrefab, true).GetComponent<SlingshotProjectile>();
		if (this.snowballSizeLevels.Count > 0 && this.sizeLevel >= 0 && this.sizeLevel < this.snowballSizeLevels.Count)
		{
			float num = scale / this.snowballSizeLevels[this.sizeLevel].snowballScale;
			SlingshotProjectile.AOEKnockbackConfig aoeKnockbackConfig = this.snowballSizeLevels[this.sizeLevel].aoeKnockbackConfig;
			aoeKnockbackConfig.aeoInnerRadius *= num;
			aoeKnockbackConfig.aeoOuterRadius *= num;
			aoeKnockbackConfig.knockbackVelocity *= num;
			aoeKnockbackConfig.impactVelocityThreshold *= num;
			velocity *= this.snowballSizeLevels[this.sizeLevel].throwSpeedMultiplier;
			component.gravityMultiplier = this.snowballSizeLevels[this.sizeLevel].gravityMultiplier;
			component.impactEffectScaleMultiplier = this.snowballSizeLevels[this.sizeLevel].impactEffectScale;
			component.aoeKnockbackConfig = new SlingshotProjectile.AOEKnockbackConfig?(aoeKnockbackConfig);
			component.impactSoundVolumeOverride = new float?(this.snowballSizeLevels[this.sizeLevel].impactSoundVolume);
			component.impactSoundPitchOverride = new float?(this.snowballSizeLevels[this.sizeLevel].impactSoundPitch);
		}
		return component;
	}

	// Token: 0x060005E9 RID: 1513 RVA: 0x000225C4 File Offset: 0x000207C4
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!(this.targetRig != null) || this.targetRig.creator == null || !this.targetRig.creator.IsLocal)
		{
			return;
		}
		SnowballThrowable snowballThrowable;
		if (((this.isLeftHanded && grabbingHand == EquipmentInteractor.instance.rightHand && EquipmentInteractor.instance.rightHandHeldEquipment == null) || (!this.isLeftHanded && grabbingHand == EquipmentInteractor.instance.leftHand && EquipmentInteractor.instance.leftHandHeldEquipment == null)) && (this.isLeftHanded ? SnowballMaker.rightHandInstance : SnowballMaker.leftHandInstance).TryCreateSnowball(this.matDataIndexes[0], out snowballThrowable))
		{
			GrowingSnowballThrowable growingSnowballThrowable = snowballThrowable as GrowingSnowballThrowable;
			if (growingSnowballThrowable != null)
			{
				growingSnowballThrowable.IncreaseSize(this.sizeLevel);
				GorillaTagger.Instance.StartVibration(!this.isLeftHanded, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
				base.SetSnowballActiveLocal(false);
			}
		}
	}

	// Token: 0x040006F0 RID: 1776
	public Transform snowballModelParentTransform;

	// Token: 0x040006F1 RID: 1777
	public Transform snowballModelTransform;

	// Token: 0x040006F2 RID: 1778
	public Vector3 modelParentOffset = Vector3.zero;

	// Token: 0x040006F3 RID: 1779
	public Vector3 modelOffset = Vector3.zero;

	// Token: 0x040006F4 RID: 1780
	public float modelRadius = 0.055f;

	// Token: 0x040006F5 RID: 1781
	[Tooltip("Snowballs will combine into the larger snowball unless they are moving faster than this threshold.Then the faster moving snowball will go in to the more stationary hand")]
	public float combineBasedOnSpeedThreshold = 0.5f;

	// Token: 0x040006F6 RID: 1782
	public SoundBankPlayer sizeIncreaseSoundBankPlayer;

	// Token: 0x040006F7 RID: 1783
	public List<GrowingSnowballThrowable.SizeParameters> snowballSizeLevels = new List<GrowingSnowballThrowable.SizeParameters>();

	// Token: 0x040006F8 RID: 1784
	private int sizeLevel;

	// Token: 0x040006F9 RID: 1785
	private float maintainSizeLevelUntilLocalTime;

	// Token: 0x040006FA RID: 1786
	private PhotonEvent changeSizeEvent;

	// Token: 0x040006FB RID: 1787
	private PhotonEvent snowballThrowEvent;

	// Token: 0x040006FC RID: 1788
	private CallLimiterWithCooldown snowballThrowCallLimit = new CallLimiterWithCooldown(10f, 10, 2f);

	// Token: 0x040006FD RID: 1789
	[HideInInspector]
	public static bool debugDrawAOERange = false;

	// Token: 0x040006FE RID: 1790
	[HideInInspector]
	public static bool twoHandedSnowballGrowing = true;

	// Token: 0x040006FF RID: 1791
	private Queue<GrowingSnowballThrowable.AOERangeDebugDraw> aoeRangeDebugDrawQueue = new Queue<GrowingSnowballThrowable.AOERangeDebugDraw>();

	// Token: 0x04000700 RID: 1792
	private GrowingSnowballThrowable otherHandSnowball;

	// Token: 0x04000701 RID: 1793
	private float debugDrawAOERangeTime = 1.5f;

	// Token: 0x020000EB RID: 235
	[Serializable]
	public struct SizeParameters
	{
		// Token: 0x04000702 RID: 1794
		public float snowballScale;

		// Token: 0x04000703 RID: 1795
		public float impactEffectScale;

		// Token: 0x04000704 RID: 1796
		public float impactSoundVolume;

		// Token: 0x04000705 RID: 1797
		public float impactSoundPitch;

		// Token: 0x04000706 RID: 1798
		public float throwSpeedMultiplier;

		// Token: 0x04000707 RID: 1799
		public float gravityMultiplier;

		// Token: 0x04000708 RID: 1800
		public SlingshotProjectile.AOEKnockbackConfig aoeKnockbackConfig;
	}

	// Token: 0x020000EC RID: 236
	private struct AOERangeDebugDraw
	{
		// Token: 0x04000709 RID: 1801
		public float impactTime;

		// Token: 0x0400070A RID: 1802
		public Vector3 position;

		// Token: 0x0400070B RID: 1803
		public float innerRadius;

		// Token: 0x0400070C RID: 1804
		public float outerRadius;
	}
}
