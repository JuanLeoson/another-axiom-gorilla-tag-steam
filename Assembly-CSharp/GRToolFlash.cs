using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x0200067D RID: 1661
public class GRToolFlash : MonoBehaviour
{
	// Token: 0x060028AF RID: 10415 RVA: 0x000DADA3 File Offset: 0x000D8FA3
	private void Awake()
	{
		this.state = GRToolFlash.State.Idle;
		this.stateTimeRemaining = -1f;
	}

	// Token: 0x060028B0 RID: 10416 RVA: 0x000DADB7 File Offset: 0x000D8FB7
	private void OnEnable()
	{
		this.StopFlash();
		this.SetState(GRToolFlash.State.Idle);
	}

	// Token: 0x060028B1 RID: 10417 RVA: 0x000DADC6 File Offset: 0x000D8FC6
	private bool IsHeldLocal()
	{
		return this.item.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x060028B2 RID: 10418 RVA: 0x000DADDF File Offset: 0x000D8FDF
	public void OnUpdate(float dt)
	{
		if (this.IsHeldLocal())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x060028B3 RID: 10419 RVA: 0x000DADF8 File Offset: 0x000D8FF8
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

	// Token: 0x060028B4 RID: 10420 RVA: 0x000DAE2C File Offset: 0x000D902C
	private void OnUpdateAuthority(float dt)
	{
		switch (this.state)
		{
		case GRToolFlash.State.Idle:
			if (this.tool.HasEnoughEnergy() && this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolFlash.State.Charging);
				this.activatedLocally = true;
				return;
			}
			break;
		case GRToolFlash.State.Charging:
		{
			bool flag = this.IsButtonHeld();
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolFlash.State.Flash);
				return;
			}
			if (!flag)
			{
				this.SetStateAuthority(GRToolFlash.State.Idle);
				this.activatedLocally = false;
				return;
			}
			break;
		}
		case GRToolFlash.State.Flash:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolFlash.State.Cooldown);
				return;
			}
			break;
		case GRToolFlash.State.Cooldown:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f && !this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolFlash.State.Idle);
				this.activatedLocally = false;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060028B5 RID: 10421 RVA: 0x000DAF14 File Offset: 0x000D9114
	private void OnUpdateRemote(float dt)
	{
		GRToolFlash.State state = (GRToolFlash.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			if (this.state == GRToolFlash.State.Charging && state == GRToolFlash.State.Cooldown)
			{
				this.SetState(GRToolFlash.State.Flash);
				return;
			}
			if (this.state == GRToolFlash.State.Flash && state == GRToolFlash.State.Cooldown)
			{
				if (Time.time > this.timeLastFlashed + this.flashDuration)
				{
					this.SetState(GRToolFlash.State.Cooldown);
					return;
				}
			}
			else
			{
				this.SetState(state);
			}
		}
	}

	// Token: 0x060028B6 RID: 10422 RVA: 0x000DAF7C File Offset: 0x000D917C
	private void SetStateAuthority(GRToolFlash.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x060028B7 RID: 10423 RVA: 0x000DAFA0 File Offset: 0x000D91A0
	private void SetState(GRToolFlash.State newState)
	{
		if (!this.CanChangeState((long)newState))
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case GRToolFlash.State.Idle:
			this.stateTimeRemaining = -1f;
			return;
		case GRToolFlash.State.Charging:
			this.StartCharge();
			this.stateTimeRemaining = this.chargeDuration;
			return;
		case GRToolFlash.State.Flash:
			this.StartFlash();
			this.stateTimeRemaining = this.flashDuration;
			return;
		case GRToolFlash.State.Cooldown:
			this.StopFlash();
			this.stateTimeRemaining = this.cooldownDuration;
			return;
		default:
			return;
		}
	}

	// Token: 0x060028B8 RID: 10424 RVA: 0x000DB024 File Offset: 0x000D9224
	private void StartCharge()
	{
		this.audioSource.volume = this.chargeSoundVolume;
		this.audioSource.clip = this.chargeSound;
		this.audioSource.Play();
		if (this.IsHeldLocal())
		{
			this.PlayVibration(GorillaTagger.Instance.tapHapticStrength, this.chargeDuration);
		}
	}

	// Token: 0x060028B9 RID: 10425 RVA: 0x000DB07C File Offset: 0x000D927C
	private void StartFlash()
	{
		this.flash.SetActive(true);
		this.audioSource.volume = this.flashSoundVolume;
		this.audioSource.clip = this.flashSound;
		this.audioSource.Play();
		this.timeLastFlashed = Time.time;
		this.tool.UseEnergy();
		if (this.IsHeldLocal())
		{
			int num = Physics.SphereCastNonAlloc(this.shootFrom.position, 1f, this.shootFrom.rotation * Vector3.forward, this.tempHitResults, 5f, this.enemyLayerMask);
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = this.tempHitResults[i];
				Rigidbody attachedRigidbody = raycastHit.collider.attachedRigidbody;
				if (attachedRigidbody != null)
				{
					GameHittable component = attachedRigidbody.GetComponent<GameHittable>();
					if (component != null)
					{
						GameHitData hitData = new GameHitData
						{
							hitTypeId = 1,
							hitEntityId = component.gameEntity.id,
							hitByEntityId = this.gameEntity.id,
							hitEntityPosition = component.gameEntity.transform.position,
							hitPosition = ((raycastHit.distance == 0f) ? this.shootFrom.position : raycastHit.point),
							hitImpulse = Vector3.zero,
							hitAmount = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.FlashDamage)
						};
						component.RequestHit(hitData);
					}
				}
			}
		}
	}

	// Token: 0x060028BA RID: 10426 RVA: 0x000DB213 File Offset: 0x000D9413
	private void StopFlash()
	{
		this.flash.SetActive(false);
	}

	// Token: 0x060028BB RID: 10427 RVA: 0x000DB224 File Offset: 0x000D9424
	private bool IsButtonHeld()
	{
		if (!this.IsHeldLocal())
		{
			return false;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(this.item.heldByActorNumber);
		if (gamePlayer == null)
		{
			return false;
		}
		int num = gamePlayer.FindHandIndex(this.item.id);
		return num != -1 && ControllerInputPoller.TriggerFloat(GamePlayer.IsLeftHand(num) ? XRNode.LeftHand : XRNode.RightHand) > 0.25f;
	}

	// Token: 0x060028BC RID: 10428 RVA: 0x000DB288 File Offset: 0x000D9488
	private void PlayVibration(float strength, float duration)
	{
		if (!this.IsHeldLocal())
		{
			return;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(this.item.heldByActorNumber);
		if (gamePlayer == null)
		{
			return;
		}
		int num = gamePlayer.FindHandIndex(this.item.id);
		if (num == -1)
		{
			return;
		}
		GorillaTagger.Instance.StartVibration(GamePlayer.IsLeftHand(num), strength, duration);
	}

	// Token: 0x060028BD RID: 10429 RVA: 0x000DB2E2 File Offset: 0x000D94E2
	public bool CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 4L && ((int)newStateIndex != 2 || Time.time > this.timeLastFlashed + this.cooldownMinimum);
	}

	// Token: 0x04003438 RID: 13368
	public GameEntity gameEntity;

	// Token: 0x04003439 RID: 13369
	public GRTool tool;

	// Token: 0x0400343A RID: 13370
	public GRAttributes attributes;

	// Token: 0x0400343B RID: 13371
	public GameObject flash;

	// Token: 0x0400343C RID: 13372
	public Transform shootFrom;

	// Token: 0x0400343D RID: 13373
	public LayerMask enemyLayerMask;

	// Token: 0x0400343E RID: 13374
	public AudioSource audioSource;

	// Token: 0x0400343F RID: 13375
	public AudioClip chargeSound;

	// Token: 0x04003440 RID: 13376
	public float chargeSoundVolume = 0.2f;

	// Token: 0x04003441 RID: 13377
	public AudioClip flashSound;

	// Token: 0x04003442 RID: 13378
	public float flashSoundVolume = 1f;

	// Token: 0x04003443 RID: 13379
	public GRToolFlash.UpgradeTypes upgradesApplied;

	// Token: 0x04003444 RID: 13380
	public float chargeDuration = 0.75f;

	// Token: 0x04003445 RID: 13381
	public float flashDuration = 0.1f;

	// Token: 0x04003446 RID: 13382
	public float cooldownDuration;

	// Token: 0x04003447 RID: 13383
	private float timeLastFlashed;

	// Token: 0x04003448 RID: 13384
	private float cooldownMinimum = 0.35f;

	// Token: 0x04003449 RID: 13385
	private bool activatedLocally;

	// Token: 0x0400344A RID: 13386
	public GameEntity item;

	// Token: 0x0400344B RID: 13387
	private GRToolFlash.State state;

	// Token: 0x0400344C RID: 13388
	private float stateTimeRemaining;

	// Token: 0x0400344D RID: 13389
	private RaycastHit[] tempHitResults = new RaycastHit[128];

	// Token: 0x0200067E RID: 1662
	[Flags]
	public enum UpgradeTypes
	{
		// Token: 0x0400344F RID: 13391
		None = 1,
		// Token: 0x04003450 RID: 13392
		UpagredA = 2,
		// Token: 0x04003451 RID: 13393
		UpagredB = 4,
		// Token: 0x04003452 RID: 13394
		UpagredC = 8
	}

	// Token: 0x0200067F RID: 1663
	private enum State
	{
		// Token: 0x04003454 RID: 13396
		Idle,
		// Token: 0x04003455 RID: 13397
		Charging,
		// Token: 0x04003456 RID: 13398
		Flash,
		// Token: 0x04003457 RID: 13399
		Cooldown,
		// Token: 0x04003458 RID: 13400
		Count
	}
}
