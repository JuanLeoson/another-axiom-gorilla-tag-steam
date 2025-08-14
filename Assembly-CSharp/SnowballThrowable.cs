using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x020000EF RID: 239
public class SnowballThrowable : HoldableObject
{
	// Token: 0x17000079 RID: 121
	// (get) Token: 0x060005FB RID: 1531 RVA: 0x00022BA9 File Offset: 0x00020DA9
	internal int ProjectileHash
	{
		get
		{
			return PoolUtils.GameObjHashCode(this.randomModelSelection ? this.localModels[this.randModelIndex].GetProjectilePrefab() : this.projectilePrefab);
		}
	}

	// Token: 0x060005FC RID: 1532 RVA: 0x00022BD8 File Offset: 0x00020DD8
	protected virtual void Awake()
	{
		if (this.awakeHasBeenCalled)
		{
			return;
		}
		this.awakeHasBeenCalled = true;
		this.targetRig = base.GetComponentInParent<VRRig>(true);
		this.isOfflineRig = (this.targetRig != null && this.targetRig.isOfflineVRRig);
		this.renderers = base.GetComponentsInChildren<Renderer>();
		this.randModelIndex = -1;
		foreach (RandomProjectileThrowable randomProjectileThrowable in this.localModels)
		{
			if (randomProjectileThrowable != null)
			{
				RandomProjectileThrowable randomProjectileThrowable2 = randomProjectileThrowable;
				randomProjectileThrowable2.OnTriggerEntered = (UnityAction<bool>)Delegate.Combine(randomProjectileThrowable2.OnTriggerEntered, new UnityAction<bool>(this.HandleOnGorillaHeadTriggerEntered));
			}
		}
	}

	// Token: 0x060005FD RID: 1533 RVA: 0x00022CA4 File Offset: 0x00020EA4
	public bool IsMine()
	{
		return this.targetRig != null && this.targetRig.isOfflineVRRig;
	}

	// Token: 0x060005FE RID: 1534 RVA: 0x00022CC4 File Offset: 0x00020EC4
	public virtual void OnEnable()
	{
		if (this.targetRig == null)
		{
			Debug.LogError("SnowballThrowable: targetRig is null! Deactivating.");
			base.gameObject.SetActive(false);
			return;
		}
		if (!this.targetRig.isOfflineVRRig)
		{
			if (this.targetRig.netView != null && this.targetRig.netView.IsMine)
			{
				base.gameObject.SetActive(false);
				return;
			}
			Color32 throwableProjectileColor = this.targetRig.GetThrowableProjectileColor(this.isLeftHanded);
			this.ApplyColor(throwableProjectileColor);
			if (this.randomModelSelection)
			{
				foreach (RandomProjectileThrowable randomProjectileThrowable in this.localModels)
				{
					randomProjectileThrowable.gameObject.SetActive(false);
				}
				this.randModelIndex = this.targetRig.GetRandomThrowableModelIndex();
				this.EnableRandomModel(this.randModelIndex, true);
			}
		}
		this.AnchorToHand();
		this.OnEnableHasBeenCalled = true;
	}

	// Token: 0x060005FF RID: 1535 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void OnDisable()
	{
	}

	// Token: 0x06000600 RID: 1536 RVA: 0x000023F5 File Offset: 0x000005F5
	protected new virtual void OnDestroy()
	{
	}

	// Token: 0x06000601 RID: 1537 RVA: 0x00022DD4 File Offset: 0x00020FD4
	public void SetSnowballActiveLocal(bool enabled)
	{
		if (!this.awakeHasBeenCalled)
		{
			this.Awake();
		}
		if (!this.OnEnableHasBeenCalled)
		{
			this.OnEnable();
		}
		if (this.isLeftHanded)
		{
			this.targetRig.LeftThrowableProjectileIndex = (enabled ? this.throwableMakerIndex : -1);
		}
		else
		{
			this.targetRig.RightThrowableProjectileIndex = (enabled ? this.throwableMakerIndex : -1);
		}
		bool flag = !base.gameObject.activeSelf && enabled;
		base.gameObject.SetActive(enabled);
		if (flag && this.pickupSoundBankPlayer != null)
		{
			this.pickupSoundBankPlayer.Play();
		}
		if (this.randomModelSelection)
		{
			if (enabled)
			{
				this.EnableRandomModel(this.GetRandomModelIndex(), true);
			}
			else
			{
				this.EnableRandomModel(this.randModelIndex, false);
			}
			this.targetRig.SetRandomThrowableModelIndex(this.randModelIndex);
		}
		EquipmentInteractor.instance.UpdateHandEquipment(enabled ? this : null, this.isLeftHanded);
		if (this.randomizeColor)
		{
			Color color = enabled ? GTColor.RandomHSV(this.randomColorHSVRanges) : Color.white;
			this.targetRig.SetThrowableProjectileColor(this.isLeftHanded, color);
			this.ApplyColor(color);
		}
	}

	// Token: 0x06000602 RID: 1538 RVA: 0x00022EFC File Offset: 0x000210FC
	private int GetRandomModelIndex()
	{
		if (this.localModels.Count == 0)
		{
			return -1;
		}
		this.randModelIndex = Random.Range(0, this.localModels.Count);
		if ((float)Random.Range(1, 100) <= this.localModels[this.randModelIndex].spawnChance * 100f)
		{
			return this.randModelIndex;
		}
		return this.GetRandomModelIndex();
	}

	// Token: 0x06000603 RID: 1539 RVA: 0x00022F64 File Offset: 0x00021164
	private void EnableRandomModel(int index, bool enable)
	{
		if (this.randModelIndex >= 0 && this.randModelIndex < this.localModels.Count)
		{
			this.localModels[this.randModelIndex].gameObject.SetActive(enable);
			if (enable && this.localModels[this.randModelIndex].autoDestroyAfterSeconds > 0f)
			{
				this.destroyTimer = 0f;
			}
			return;
		}
	}

	// Token: 0x06000604 RID: 1540 RVA: 0x00022FD8 File Offset: 0x000211D8
	protected virtual void LateUpdateLocal()
	{
		if (this.randomModelSelection && this.randModelIndex > -1 && this.localModels[this.randModelIndex].ForceDestroy)
		{
			this.localModels[this.randModelIndex].ForceDestroy = false;
			if (this.localModels[this.randModelIndex].gameObject.activeSelf)
			{
				this.PerformSnowballThrowAuthority();
			}
		}
		if (this.randomModelSelection && this.randModelIndex > -1 && this.localModels[this.randModelIndex].autoDestroyAfterSeconds > 0f)
		{
			this.destroyTimer += Time.deltaTime;
			if (this.destroyTimer > this.localModels[this.randModelIndex].autoDestroyAfterSeconds)
			{
				if (this.localModels[this.randModelIndex].gameObject.activeSelf)
				{
					this.PerformSnowballThrowAuthority();
				}
				this.destroyTimer = -1f;
			}
		}
	}

	// Token: 0x06000605 RID: 1541 RVA: 0x000023F5 File Offset: 0x000005F5
	protected void LateUpdateReplicated()
	{
	}

	// Token: 0x06000606 RID: 1542 RVA: 0x000023F5 File Offset: 0x000005F5
	protected void LateUpdateShared()
	{
	}

	// Token: 0x06000607 RID: 1543 RVA: 0x000230D7 File Offset: 0x000212D7
	private Transform Anchor()
	{
		return base.transform.parent;
	}

	// Token: 0x06000608 RID: 1544 RVA: 0x000230E4 File Offset: 0x000212E4
	private void AnchorToHand()
	{
		BodyDockPositions myBodyDockPositions = this.targetRig.myBodyDockPositions;
		Transform transform = this.Anchor();
		if (this.isLeftHanded)
		{
			transform.parent = myBodyDockPositions.leftHandTransform;
		}
		else
		{
			transform.parent = myBodyDockPositions.rightHandTransform;
		}
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
	}

	// Token: 0x06000609 RID: 1545 RVA: 0x0002313C File Offset: 0x0002133C
	protected void LateUpdate()
	{
		if (this.IsMine())
		{
			this.LateUpdateLocal();
		}
		else
		{
			this.LateUpdateReplicated();
		}
		this.LateUpdateShared();
	}

	// Token: 0x0600060A RID: 1546 RVA: 0x0002315A File Offset: 0x0002135A
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		this.OnSnowballRelease();
		return true;
	}

	// Token: 0x0600060B RID: 1547 RVA: 0x0002316F File Offset: 0x0002136F
	protected virtual void OnSnowballRelease()
	{
		this.PerformSnowballThrowAuthority();
	}

	// Token: 0x0600060C RID: 1548 RVA: 0x00023178 File Offset: 0x00021378
	protected virtual void PerformSnowballThrowAuthority()
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
		Vector3 velocity = a + b;
		Color32 throwableProjectileColor = this.targetRig.GetThrowableProjectileColor(this.isLeftHanded);
		Transform transform = base.transform;
		Vector3 position = transform.position;
		float x = transform.lossyScale.x;
		SlingshotProjectile slingshotProjectile = this.LaunchSnowballLocal(position, velocity, x, this.randomizeColor, throwableProjectileColor);
		this.SetSnowballActiveLocal(false);
		if (this.randModelIndex > -1 && this.randModelIndex < this.localModels.Count)
		{
			if (this.localModels[this.randModelIndex].ForceDestroy || this.localModels[this.randModelIndex].destroyAfterRelease)
			{
				slingshotProjectile.DestroyAfterRelease();
			}
			else if (this.localModels[this.randModelIndex].moveOverPassedLifeTime)
			{
				float num2 = Time.time - this.localModels[this.randModelIndex].TimeEnabled;
				float remainingLifeTime = slingshotProjectile.GetRemainingLifeTime();
				if (remainingLifeTime > num2)
				{
					float newLifeTime = remainingLifeTime - num2;
					slingshotProjectile.UpdateRemainingLifeTime(newLifeTime);
				}
				else
				{
					slingshotProjectile.UpdateRemainingLifeTime(0f);
				}
			}
		}
		if (NetworkSystem.Instance.InRoom)
		{
			RoomSystem.SendLaunchProjectile(position, velocity, this.isLeftHanded ? RoomSystem.ProjectileSource.LeftHand : RoomSystem.ProjectileSource.RightHand, slingshotProjectile.myProjectileCount, this.randomizeColor, throwableProjectileColor.r, throwableProjectileColor.g, throwableProjectileColor.b, throwableProjectileColor.a);
		}
	}

	// Token: 0x0600060D RID: 1549 RVA: 0x00023380 File Offset: 0x00021580
	protected virtual SlingshotProjectile LaunchSnowballLocal(Vector3 location, Vector3 velocity, float scale, bool randomColour, Color colour)
	{
		SlingshotProjectile component = ObjectPools.instance.Instantiate(this.randomModelSelection ? this.localModels[this.randModelIndex].GetProjectilePrefab() : this.projectilePrefab, true).GetComponent<SlingshotProjectile>();
		int projectileCount = ProjectileTracker.AddAndIncrementLocalProjectile(component, velocity, location, scale);
		component.Launch(location, velocity, NetworkSystem.Instance.LocalPlayer, false, false, projectileCount, scale, randomColour, colour);
		if (string.IsNullOrEmpty(this.throwEventName))
		{
			PlayerGameEvents.LaunchedProjectile(this.projectilePrefab.name);
		}
		else
		{
			PlayerGameEvents.LaunchedProjectile(this.throwEventName);
		}
		component.OnImpact += this.OnProjectileImpact;
		return component;
	}

	// Token: 0x0600060E RID: 1550 RVA: 0x00023424 File Offset: 0x00021624
	protected virtual SlingshotProjectile SpawnProjectile()
	{
		return ObjectPools.instance.Instantiate(this.randomModelSelection ? this.localModels[this.randModelIndex].GetProjectilePrefab() : this.projectilePrefab, true).GetComponent<SlingshotProjectile>();
	}

	// Token: 0x0600060F RID: 1551 RVA: 0x0002345C File Offset: 0x0002165C
	protected virtual void OnProjectileImpact(SlingshotProjectile projectile, Vector3 impactPos, NetPlayer hitPlayer)
	{
		if (hitPlayer != null)
		{
			ScienceExperimentManager instance = ScienceExperimentManager.instance;
			if (instance != null && this.projectilePrefab != null && this.projectilePrefab == instance.waterBalloonPrefab)
			{
				instance.OnWaterBalloonHitPlayer(hitPlayer);
			}
		}
	}

	// Token: 0x06000610 RID: 1552 RVA: 0x000234A8 File Offset: 0x000216A8
	private void ApplyColor(Color newColor)
	{
		foreach (Renderer renderer in this.renderers)
		{
			if (renderer)
			{
				foreach (Material material in renderer.materials)
				{
					if (!(material == null))
					{
						if (material.HasProperty(ShaderProps._BaseColor))
						{
							material.SetColor(ShaderProps._BaseColor, newColor);
						}
						if (material.HasProperty(ShaderProps._Color))
						{
							material.SetColor(ShaderProps._Color, newColor);
						}
					}
				}
			}
		}
	}

	// Token: 0x06000611 RID: 1553 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06000612 RID: 1554 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
	}

	// Token: 0x06000613 RID: 1555 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void DropItemCleanup()
	{
	}

	// Token: 0x06000614 RID: 1556 RVA: 0x00023535 File Offset: 0x00021735
	private void HandleOnGorillaHeadTriggerEntered(bool enable)
	{
		this.SetSnowballActiveLocal(enable);
	}

	// Token: 0x04000717 RID: 1815
	[GorillaSoundLookup]
	public List<int> matDataIndexes = new List<int>
	{
		32
	};

	// Token: 0x04000718 RID: 1816
	public GameObject projectilePrefab;

	// Token: 0x04000719 RID: 1817
	[FormerlySerializedAs("shouldColorize")]
	public bool randomizeColor;

	// Token: 0x0400071A RID: 1818
	public GTColor.HSVRanges randomColorHSVRanges = new GTColor.HSVRanges(0f, 1f, 0.7f, 1f, 1f, 1f);

	// Token: 0x0400071B RID: 1819
	public GorillaVelocityEstimator velocityEstimator;

	// Token: 0x0400071C RID: 1820
	public SoundBankPlayer pickupSoundBankPlayer;

	// Token: 0x0400071D RID: 1821
	public float linSpeedMultiplier = 1f;

	// Token: 0x0400071E RID: 1822
	public float maxLinSpeed = 12f;

	// Token: 0x0400071F RID: 1823
	public float maxWristSpeed = 4f;

	// Token: 0x04000720 RID: 1824
	public bool isLeftHanded;

	// Token: 0x04000721 RID: 1825
	[Tooltip("Check this part only if we want to randomize the prefab meshes and projectile")]
	public bool randomModelSelection;

	// Token: 0x04000722 RID: 1826
	public List<RandomProjectileThrowable> localModels;

	// Token: 0x04000723 RID: 1827
	[Tooltip("This needs to match the index of the projectilePrefab in Body Dock Position")]
	public int throwableMakerIndex;

	// Token: 0x04000724 RID: 1828
	public string throwEventName;

	// Token: 0x04000725 RID: 1829
	protected VRRig targetRig;

	// Token: 0x04000726 RID: 1830
	protected bool isOfflineRig;

	// Token: 0x04000727 RID: 1831
	private bool awakeHasBeenCalled;

	// Token: 0x04000728 RID: 1832
	private bool OnEnableHasBeenCalled;

	// Token: 0x04000729 RID: 1833
	private Renderer[] renderers;

	// Token: 0x0400072A RID: 1834
	protected int randModelIndex;

	// Token: 0x0400072B RID: 1835
	private float destroyTimer = -1f;
}
