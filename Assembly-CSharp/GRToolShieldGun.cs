using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000686 RID: 1670
public class GRToolShieldGun : MonoBehaviour
{
	// Token: 0x060028ED RID: 10477 RVA: 0x000DC1F5 File Offset: 0x000DA3F5
	private bool IsHeldLocal()
	{
		return this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x060028EE RID: 10478 RVA: 0x000DC210 File Offset: 0x000DA410
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.IsHeldLocal() || this.activatedLocally)
		{
			this.OnUpdateAuthority(deltaTime);
			return;
		}
		this.OnUpdateRemote(deltaTime);
	}

	// Token: 0x060028EF RID: 10479 RVA: 0x000DC244 File Offset: 0x000DA444
	private void OnUpdateAuthority(float dt)
	{
		switch (this.state)
		{
		case GRToolShieldGun.State.Idle:
			if (this.tool.HasEnoughEnergy() && this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolShieldGun.State.Charging);
				this.activatedLocally = true;
				return;
			}
			break;
		case GRToolShieldGun.State.Charging:
		{
			bool flag = this.IsButtonHeld();
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolShieldGun.State.Firing);
				return;
			}
			if (!flag)
			{
				this.SetStateAuthority(GRToolShieldGun.State.Idle);
				this.activatedLocally = false;
				return;
			}
			break;
		}
		case GRToolShieldGun.State.Firing:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolShieldGun.State.Cooldown);
				return;
			}
			break;
		case GRToolShieldGun.State.Cooldown:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f && !this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolShieldGun.State.Idle);
				this.activatedLocally = false;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060028F0 RID: 10480 RVA: 0x000DC32C File Offset: 0x000DA52C
	private void OnUpdateRemote(float dt)
	{
		GRToolShieldGun.State state = (GRToolShieldGun.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetStateAuthority(state);
		}
	}

	// Token: 0x060028F1 RID: 10481 RVA: 0x000DC356 File Offset: 0x000DA556
	private void SetStateAuthority(GRToolShieldGun.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x060028F2 RID: 10482 RVA: 0x000DC378 File Offset: 0x000DA578
	private void SetState(GRToolShieldGun.State newState)
	{
		if (newState == this.state || !this.CanChangeState((long)newState))
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case GRToolShieldGun.State.Idle:
			this.stateTimeRemaining = -1f;
			return;
		case GRToolShieldGun.State.Charging:
			this.StartCharge();
			this.stateTimeRemaining = this.chargeDuration;
			return;
		case GRToolShieldGun.State.Firing:
			this.StartFiring();
			this.stateTimeRemaining = this.flashDuration;
			return;
		case GRToolShieldGun.State.Cooldown:
			this.stateTimeRemaining = this.cooldownDuration;
			return;
		default:
			return;
		}
	}

	// Token: 0x060028F3 RID: 10483 RVA: 0x000DC3FC File Offset: 0x000DA5FC
	private void StartCharge()
	{
		if (this.chargeSound != null)
		{
			this.audioSource.PlayOneShot(this.chargeSound, this.chargeSoundVolume);
		}
		if (this.IsHeldLocal())
		{
			this.PlayVibration(GorillaTagger.Instance.tapHapticStrength, this.chargeDuration);
		}
	}

	// Token: 0x060028F4 RID: 10484 RVA: 0x000DC44C File Offset: 0x000DA64C
	private void StartFiring()
	{
		if (this.firingSound != null)
		{
			this.audioSource.PlayOneShot(this.firingSound, this.firingSoundVolume);
		}
		this.timeLastFired = Time.time;
		this.tool.UseEnergy();
		Vector3 position = this.firingTransform.position;
		Vector3 velocity = this.firingTransform.forward * this.projectileSpeed;
		float scale = GTPlayer.Instance.scale;
		int hash = PoolUtils.GameObjHashCode(this.projectilePrefab);
		this.firedProjectile = ObjectPools.instance.Instantiate(hash, true).GetComponent<SlingshotProjectile>();
		this.firedProjectile.transform.localScale = Vector3.one * scale;
		if (this.projectileTrailPrefab != null)
		{
			int trailHash = PoolUtils.GameObjHashCode(this.projectileTrailPrefab);
			this.AttachTrail(trailHash, this.firedProjectile.gameObject, position, false, false);
		}
		Collider component = this.firedProjectile.gameObject.GetComponent<Collider>();
		if (component != null)
		{
			for (int i = 0; i < this.colliders.Count; i++)
			{
				Physics.IgnoreCollision(this.colliders[i], component);
			}
		}
		if (this.IsHeldLocal())
		{
			this.firedProjectile.OnImpact += this.OnProjectileImpact;
		}
		this.firedProjectile.Launch(position, velocity, NetworkSystem.Instance.LocalPlayer, false, false, 1, scale, true, this.projectileColor);
	}

	// Token: 0x060028F5 RID: 10485 RVA: 0x000DC5BC File Offset: 0x000DA7BC
	private void AttachTrail(int trailHash, GameObject newProjectile, Vector3 location, bool blueTeam, bool orangeTeam)
	{
		GameObject gameObject = ObjectPools.instance.Instantiate(trailHash, true);
		SlingshotProjectileTrail component = gameObject.GetComponent<SlingshotProjectileTrail>();
		if (component.IsNull())
		{
			ObjectPools.instance.Destroy(gameObject);
		}
		newProjectile.transform.position = location;
		component.AttachTrail(newProjectile, blueTeam, orangeTeam);
	}

	// Token: 0x060028F6 RID: 10486 RVA: 0x000DC608 File Offset: 0x000DA808
	private void OnProjectileImpact(SlingshotProjectile projectile, Vector3 impactPos, NetPlayer hitPlayer)
	{
		projectile.OnImpact -= this.OnProjectileImpact;
		GRPlayer grplayer = null;
		RigContainer rigContainer;
		if (hitPlayer != null && VRRigCache.Instance.TryGetVrrig(hitPlayer, out rigContainer) && rigContainer.Rig != null)
		{
			grplayer = rigContainer.Rig.GetComponent<GRPlayer>();
		}
		else if (this.allowAoeHits)
		{
			GRToolShieldGun.vrRigs.Clear();
			GRToolShieldGun.vrRigs.Add(VRRig.LocalRig);
			VRRigCache.Instance.GetAllUsedRigs(GRToolShieldGun.vrRigs);
			VRRig vrrig = null;
			float num = float.MaxValue;
			for (int i = 0; i < GRToolShieldGun.vrRigs.Count; i++)
			{
				float sqrMagnitude = (GRToolShieldGun.vrRigs[i].bodyTransform.position - impactPos).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					vrrig = GRToolShieldGun.vrRigs[i];
				}
			}
			if (vrrig != null)
			{
				grplayer = vrrig.GetComponent<GRPlayer>();
			}
		}
		if (grplayer != null)
		{
			GhostReactorManager.instance.RequestGrantPlayerShield(grplayer, this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ShieldSize));
		}
	}

	// Token: 0x060028F7 RID: 10487 RVA: 0x000DC71C File Offset: 0x000DA91C
	private bool IsButtonHeld()
	{
		if (!this.IsHeldLocal())
		{
			return false;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(this.gameEntity.heldByActorNumber);
		if (gamePlayer == null)
		{
			return false;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		return num != -1 && ControllerInputPoller.TriggerFloat(GamePlayer.IsLeftHand(num) ? XRNode.LeftHand : XRNode.RightHand) > 0.25f;
	}

	// Token: 0x060028F8 RID: 10488 RVA: 0x000DC780 File Offset: 0x000DA980
	private void PlayVibration(float strength, float duration)
	{
		if (!this.IsHeldLocal())
		{
			return;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(this.gameEntity.heldByActorNumber);
		if (gamePlayer == null)
		{
			return;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		if (num == -1)
		{
			return;
		}
		GorillaTagger.Instance.StartVibration(GamePlayer.IsLeftHand(num), strength, duration);
	}

	// Token: 0x060028F9 RID: 10489 RVA: 0x000DC7DA File Offset: 0x000DA9DA
	public bool CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 4L && ((int)newStateIndex != 2 || Time.time > this.timeLastFired + this.cooldownMinimum);
	}

	// Token: 0x040034B2 RID: 13490
	public GameEntity gameEntity;

	// Token: 0x040034B3 RID: 13491
	public GRTool tool;

	// Token: 0x040034B4 RID: 13492
	public GRAttributes attributes;

	// Token: 0x040034B5 RID: 13493
	public GameObject projectilePrefab;

	// Token: 0x040034B6 RID: 13494
	public GameObject projectileTrailPrefab;

	// Token: 0x040034B7 RID: 13495
	public Transform firingTransform;

	// Token: 0x040034B8 RID: 13496
	public List<Collider> colliders;

	// Token: 0x040034B9 RID: 13497
	public float projectileSpeed = 25f;

	// Token: 0x040034BA RID: 13498
	public Color projectileColor = new Color(0.25f, 0.25f, 1f);

	// Token: 0x040034BB RID: 13499
	public bool allowAoeHits;

	// Token: 0x040034BC RID: 13500
	public float aeoHitRadius = 0.5f;

	// Token: 0x040034BD RID: 13501
	public float chargeDuration = 0.75f;

	// Token: 0x040034BE RID: 13502
	public float flashDuration = 0.1f;

	// Token: 0x040034BF RID: 13503
	public float cooldownDuration;

	// Token: 0x040034C0 RID: 13504
	public AudioSource audioSource;

	// Token: 0x040034C1 RID: 13505
	public AudioClip chargeSound;

	// Token: 0x040034C2 RID: 13506
	public float chargeSoundVolume = 0.5f;

	// Token: 0x040034C3 RID: 13507
	public AudioClip firingSound;

	// Token: 0x040034C4 RID: 13508
	public float firingSoundVolume = 0.5f;

	// Token: 0x040034C5 RID: 13509
	private GRToolShieldGun.State state;

	// Token: 0x040034C6 RID: 13510
	private float stateTimeRemaining;

	// Token: 0x040034C7 RID: 13511
	private bool activatedLocally;

	// Token: 0x040034C8 RID: 13512
	private bool waitingForButtonRelease;

	// Token: 0x040034C9 RID: 13513
	private float timeLastFired;

	// Token: 0x040034CA RID: 13514
	private float cooldownMinimum = 0.35f;

	// Token: 0x040034CB RID: 13515
	private SlingshotProjectile firedProjectile;

	// Token: 0x040034CC RID: 13516
	private static List<VRRig> vrRigs = new List<VRRig>(10);

	// Token: 0x02000687 RID: 1671
	private enum State
	{
		// Token: 0x040034CE RID: 13518
		Idle,
		// Token: 0x040034CF RID: 13519
		Charging,
		// Token: 0x040034D0 RID: 13520
		Firing,
		// Token: 0x040034D1 RID: 13521
		Cooldown,
		// Token: 0x040034D2 RID: 13522
		Count
	}
}
