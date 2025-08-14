using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

// Token: 0x02000679 RID: 1657
public class GRToolCollector : MonoBehaviour
{
	// Token: 0x0600288E RID: 10382 RVA: 0x000DA2BF File Offset: 0x000D84BF
	private void Awake()
	{
		this.state = GRToolCollector.State.Idle;
		this.stateTimeRemaining = -1f;
	}

	// Token: 0x0600288F RID: 10383 RVA: 0x000DA2D3 File Offset: 0x000D84D3
	private void OnEnable()
	{
		this.SetState(GRToolCollector.State.Idle);
	}

	// Token: 0x06002890 RID: 10384 RVA: 0x000DA2DC File Offset: 0x000D84DC
	private bool IsHeldLocal()
	{
		return this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x06002891 RID: 10385 RVA: 0x000DA2F5 File Offset: 0x000D84F5
	public void OnUpdate(float dt)
	{
		if (this.IsHeldLocal() || this.activatedLocally)
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x06002892 RID: 10386 RVA: 0x000DA318 File Offset: 0x000D8518
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

	// Token: 0x06002893 RID: 10387 RVA: 0x000DA34C File Offset: 0x000D854C
	private void OnUpdateAuthority(float dt)
	{
		switch (this.state)
		{
		case GRToolCollector.State.Idle:
		{
			bool flag = this.IsButtonHeld();
			this.waitingForButtonRelease = (this.waitingForButtonRelease && flag);
			if (flag && !this.waitingForButtonRelease)
			{
				this.SetStateAuthority(GRToolCollector.State.Vacuuming);
				this.activatedLocally = true;
				return;
			}
			break;
		}
		case GRToolCollector.State.Vacuuming:
		{
			bool flag2 = this.IsButtonHeld();
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolCollector.State.Collect);
				return;
			}
			if (!flag2)
			{
				this.SetStateAuthority(GRToolCollector.State.Idle);
				this.activatedLocally = false;
				return;
			}
			break;
		}
		case GRToolCollector.State.Collect:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolCollector.State.Cooldown);
				return;
			}
			break;
		case GRToolCollector.State.Cooldown:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.activatedLocally = false;
				this.waitingForButtonRelease = true;
				this.SetStateAuthority(GRToolCollector.State.Idle);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002894 RID: 10388 RVA: 0x000DA43C File Offset: 0x000D863C
	private void OnUpdateRemote(float dt)
	{
		GRToolCollector.State state = (GRToolCollector.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x06002895 RID: 10389 RVA: 0x000DA466 File Offset: 0x000D8666
	private void SetStateAuthority(GRToolCollector.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06002896 RID: 10390 RVA: 0x000DA488 File Offset: 0x000D8688
	private void SetState(GRToolCollector.State newState)
	{
		this.state = newState;
		switch (this.state)
		{
		case GRToolCollector.State.Idle:
			this.StopVacuum();
			this.stateTimeRemaining = -1f;
			return;
		case GRToolCollector.State.Vacuuming:
			this.StartVacuum();
			this.stateTimeRemaining = this.chargeDuration;
			return;
		case GRToolCollector.State.Collect:
			this.TryCollect();
			this.stateTimeRemaining = this.collectDuration;
			return;
		case GRToolCollector.State.Cooldown:
			this.stateTimeRemaining = this.cooldownDuration;
			return;
		default:
			return;
		}
	}

	// Token: 0x06002897 RID: 10391 RVA: 0x000DA500 File Offset: 0x000D8700
	private void StartVacuum()
	{
		this.vacuumAudioSource.clip = this.vacuumSound;
		this.vacuumAudioSource.volume = this.vacuumSoundVolume;
		this.vacuumAudioSource.loop = true;
		this.vacuumAudioSource.Play();
		this.vacuumParticleEffect.Play();
		if (this.IsHeldLocal())
		{
			this.PlayVibration(GorillaTagger.Instance.tapHapticStrength, this.chargeDuration);
		}
	}

	// Token: 0x06002898 RID: 10392 RVA: 0x000DA56F File Offset: 0x000D876F
	private void StopVacuum()
	{
		this.vacuumAudioSource.loop = false;
		this.vacuumAudioSource.Stop();
		this.vacuumParticleEffect.Stop();
	}

	// Token: 0x06002899 RID: 10393 RVA: 0x000DA594 File Offset: 0x000D8794
	private void TryCollect()
	{
		if (this.IsHeldLocal())
		{
			int num = Physics.SphereCastNonAlloc(this.shootFrom.position, 0.2f, this.shootFrom.rotation * Vector3.forward, this.tempHitResults, 1f, this.collectibleLayerMask);
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = this.tempHitResults[i];
				GameObject gameObject = null;
				Rigidbody attachedRigidbody = raycastHit.collider.attachedRigidbody;
				if (attachedRigidbody != null)
				{
					gameObject = attachedRigidbody.gameObject;
				}
				else
				{
					GameEntity gameEntity = GameEntity.Get(raycastHit.collider);
					if (gameEntity != null)
					{
						gameObject = gameEntity.gameObject;
					}
				}
				if (gameObject != null)
				{
					GRCollectible component = gameObject.GetComponent<GRCollectible>();
					if (component != null && !component.unlockPointsCollectible)
					{
						GhostReactorManager.Get(this.gameEntity).RequestCollectItem(component.entity.id, this.gameEntity.id);
						return;
					}
					if (gameObject.GetComponent<GRCurrencyDepositor>() != null)
					{
						if (this.tool.energy > 0)
						{
							GhostReactorManager.Get(this.gameEntity).RequestDepositCurrency(this.gameEntity.id);
						}
						return;
					}
					GRTool component2 = gameObject.GetComponent<GRTool>();
					if (!(component2 == null) && !(component2 == this.tool))
					{
						GameEntity component3 = gameObject.GetComponent<GameEntity>();
						if (component2 != null && component3 != null)
						{
							GhostReactorManager.Get(this.gameEntity).RequestChargeTool(this.gameEntity.id, component3.id);
							return;
						}
					}
				}
			}
		}
	}

	// Token: 0x0600289A RID: 10394 RVA: 0x000DA738 File Offset: 0x000D8938
	public void PerformCollection(GRCollectible collectible)
	{
		this.tool.RefillEnergy(collectible.energyValue + this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HarvestGain), collectible.entity.id);
		this.collectAudioSource.volume = this.collectSoundVolume;
		this.collectAudioSource.PlayOneShot(this.collectSound);
	}

	// Token: 0x0600289B RID: 10395 RVA: 0x000DA790 File Offset: 0x000D8990
	public void PlayChargeEffect(GRTool targetTool)
	{
		if (targetTool == null)
		{
			return;
		}
		this.collectAudioSource.volume = this.chargeBeamVolume;
		this.collectAudioSource.PlayOneShot(this.chargeBeamSound);
		for (int i = 0; i < targetTool.energyMeters.Count; i++)
		{
			if (targetTool.energyMeters[i].chargePoint != null)
			{
				this.lightningDispatcher.DispatchLightning(this.lightningDispatcher.transform.position, targetTool.energyMeters[i].chargePoint.position);
			}
			else
			{
				this.lightningDispatcher.DispatchLightning(this.lightningDispatcher.transform.position, targetTool.energyMeters[i].transform.position);
			}
		}
	}

	// Token: 0x0600289C RID: 10396 RVA: 0x000DA864 File Offset: 0x000D8A64
	public void PlayChargeEffect(GRCurrencyDepositor targetDepositor)
	{
		if (targetDepositor == null)
		{
			return;
		}
		this.collectAudioSource.volume = this.chargeBeamVolume;
		this.collectAudioSource.PlayOneShot(this.chargeBeamSound);
		this.lightningDispatcher.DispatchLightning(this.lightningDispatcher.transform.position, targetDepositor.depositingChargePoint.position);
	}

	// Token: 0x0600289D RID: 10397 RVA: 0x000DA8C4 File Offset: 0x000D8AC4
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

	// Token: 0x0600289E RID: 10398 RVA: 0x000DA928 File Offset: 0x000D8B28
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

	// Token: 0x04003406 RID: 13318
	public GameEntity gameEntity;

	// Token: 0x04003407 RID: 13319
	public GRTool tool;

	// Token: 0x04003408 RID: 13320
	public GRAttributes attributes;

	// Token: 0x04003409 RID: 13321
	public int energyDepositPerUse = 100;

	// Token: 0x0400340A RID: 13322
	public Transform shootFrom;

	// Token: 0x0400340B RID: 13323
	public LayerMask collectibleLayerMask;

	// Token: 0x0400340C RID: 13324
	public ParticleSystem vacuumParticleEffect;

	// Token: 0x0400340D RID: 13325
	public AudioSource vacuumAudioSource;

	// Token: 0x0400340E RID: 13326
	public AudioClip vacuumSound;

	// Token: 0x0400340F RID: 13327
	public float vacuumSoundVolume = 0.2f;

	// Token: 0x04003410 RID: 13328
	public AudioSource collectAudioSource;

	// Token: 0x04003411 RID: 13329
	[FormerlySerializedAs("flashSound")]
	public AudioClip collectSound;

	// Token: 0x04003412 RID: 13330
	[FormerlySerializedAs("flashSoundVolume")]
	public float collectSoundVolume = 1f;

	// Token: 0x04003413 RID: 13331
	public AudioClip chargeBeamSound;

	// Token: 0x04003414 RID: 13332
	public float chargeBeamVolume = 0.2f;

	// Token: 0x04003415 RID: 13333
	public LightningDispatcher lightningDispatcher;

	// Token: 0x04003416 RID: 13334
	public float chargeDuration = 0.75f;

	// Token: 0x04003417 RID: 13335
	[FormerlySerializedAs("flashDuration")]
	public float collectDuration = 0.1f;

	// Token: 0x04003418 RID: 13336
	public float cooldownDuration;

	// Token: 0x04003419 RID: 13337
	[NonSerialized]
	public GhostReactorManager grManager;

	// Token: 0x0400341A RID: 13338
	private GRToolCollector.State state;

	// Token: 0x0400341B RID: 13339
	private float stateTimeRemaining;

	// Token: 0x0400341C RID: 13340
	private bool activatedLocally;

	// Token: 0x0400341D RID: 13341
	private bool waitingForButtonRelease;

	// Token: 0x0400341E RID: 13342
	private RaycastHit[] tempHitResults = new RaycastHit[128];

	// Token: 0x0200067A RID: 1658
	private enum State
	{
		// Token: 0x04003420 RID: 13344
		Idle,
		// Token: 0x04003421 RID: 13345
		Vacuuming,
		// Token: 0x04003422 RID: 13346
		Collect,
		// Token: 0x04003423 RID: 13347
		Cooldown
	}
}
