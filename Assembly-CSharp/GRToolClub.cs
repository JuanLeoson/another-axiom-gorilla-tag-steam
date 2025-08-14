using System;
using System.Collections.Generic;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000677 RID: 1655
public class GRToolClub : MonoBehaviour, IGameHitter
{
	// Token: 0x170003BE RID: 958
	// (get) Token: 0x0600287B RID: 10363 RVA: 0x000D9AE2 File Offset: 0x000D7CE2
	private bool IsExtended
	{
		get
		{
			return this.state == GRToolClub.State.Extended && this.extendedAmount > 0.7f;
		}
	}

	// Token: 0x0600287C RID: 10364 RVA: 0x000D9AFC File Offset: 0x000D7CFC
	private void Awake()
	{
		this.retractableSection.localPosition = new Vector3(0f, 0f, 0f);
	}

	// Token: 0x0600287D RID: 10365 RVA: 0x000D9B1D File Offset: 0x000D7D1D
	public void OnEnable()
	{
		this.SetExtendedAmount(0f);
		this.SetState(GRToolClub.State.Idle);
	}

	// Token: 0x0600287E RID: 10366 RVA: 0x000D9B31 File Offset: 0x000D7D31
	private bool IsHeldLocal()
	{
		return this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x0600287F RID: 10367 RVA: 0x000D9B4A File Offset: 0x000D7D4A
	private bool IsHeld()
	{
		return this.gameEntity.heldByActorNumber != -1;
	}

	// Token: 0x06002880 RID: 10368 RVA: 0x000D9B60 File Offset: 0x000D7D60
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.IsHeld())
		{
			if (this.IsHeldLocal())
			{
				this.OnUpdateAuthority(deltaTime);
			}
			else
			{
				this.OnUpdateRemote(deltaTime);
			}
		}
		else
		{
			this.SetState(GRToolClub.State.Idle);
		}
		this.OnUpdateShared(deltaTime);
	}

	// Token: 0x06002881 RID: 10369 RVA: 0x000D9BA4 File Offset: 0x000D7DA4
	private void OnUpdateAuthority(float dt)
	{
		GRToolClub.State state = this.state;
		if (state != GRToolClub.State.Idle)
		{
			if (state != GRToolClub.State.Extended)
			{
				return;
			}
			if (!this.IsButtonHeld() || !this.tool.HasEnoughEnergy())
			{
				this.SetState(GRToolClub.State.Idle);
			}
		}
		else if (this.IsButtonHeld() && this.tool.HasEnoughEnergy())
		{
			this.SetState(GRToolClub.State.Extended);
			return;
		}
	}

	// Token: 0x06002882 RID: 10370 RVA: 0x000D9C04 File Offset: 0x000D7E04
	private void OnUpdateRemote(float dt)
	{
		GRToolClub.State state = (GRToolClub.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x06002883 RID: 10371 RVA: 0x000D9C30 File Offset: 0x000D7E30
	private void OnUpdateShared(float dt)
	{
		GRToolClub.State state = this.state;
		if (state != GRToolClub.State.Idle)
		{
			if (state != GRToolClub.State.Extended)
			{
				return;
			}
			if (this.extendedAmount < 1f)
			{
				float num = Mathf.MoveTowards(this.extendedAmount, 1f, 1f / this.extensionTime * Time.deltaTime);
				this.SetExtendedAmount(num);
			}
		}
		else if (this.extendedAmount > 0f)
		{
			float num2 = Mathf.MoveTowards(this.extendedAmount, 0f, 1f / this.extensionTime * Time.deltaTime);
			this.SetExtendedAmount(num2);
			return;
		}
	}

	// Token: 0x06002884 RID: 10372 RVA: 0x000D9CBC File Offset: 0x000D7EBC
	private void SetExtendedAmount(float newExtendedAmount)
	{
		this.extendedAmount = newExtendedAmount;
		float y = Mathf.Lerp(this.retractableSectionMin, this.retractableSectionMax, this.extendedAmount);
		this.retractableSection.localPosition = new Vector3(0f, y, 0f);
	}

	// Token: 0x06002885 RID: 10373 RVA: 0x000D9D04 File Offset: 0x000D7F04
	private void SetState(GRToolClub.State newState)
	{
		if (this.state == newState)
		{
			return;
		}
		GRToolClub.State state = this.state;
		if (state != GRToolClub.State.Idle)
		{
		}
		this.state = newState;
		state = this.state;
		if (state != GRToolClub.State.Idle)
		{
			if (state == GRToolClub.State.Extended)
			{
				this.idleCollider.enabled = false;
				this.extendedCollider.enabled = true;
				this.poweredMeshRenderer.material = this.poweredMaterial;
				this.humAudioSource.Play();
				this.dullLight.SetActive(true);
				this.audioSource.PlayOneShot(this.extendAudio, this.extendVolume);
				for (int i = 0; i < this.humParticleEffects.Count; i++)
				{
					this.humParticleEffects[i].gameObject.SetActive(true);
				}
			}
		}
		else
		{
			this.extendedCollider.enabled = false;
			this.idleCollider.enabled = true;
			this.poweredMeshRenderer.material = this.idleMaterial;
			this.humAudioSource.Stop();
			this.dullLight.SetActive(false);
			this.audioSource.PlayOneShot(this.retractAudio, this.retractVolume);
			for (int j = 0; j < this.humParticleEffects.Count; j++)
			{
				this.humParticleEffects[j].gameObject.SetActive(false);
			}
		}
		if (this.IsHeldLocal())
		{
			this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
		}
	}

	// Token: 0x06002886 RID: 10374 RVA: 0x000D9E74 File Offset: 0x000D8074
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

	// Token: 0x06002887 RID: 10375 RVA: 0x000D9ED8 File Offset: 0x000D80D8
	private void OnCollisionEnter(Collision collision)
	{
		if (!this.IsExtended)
		{
			return;
		}
		float num = this.gameEntity.GetVelocity().sqrMagnitude;
		if (this.gameEntity.lastHeldByActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
		{
			return;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(this.gameEntity.heldByActorNumber);
		if (gamePlayer != null)
		{
			float handSpeed = GamePlayerLocal.instance.GetHandSpeed(gamePlayer.FindHandIndex(this.gameEntity.id));
			num = handSpeed * handSpeed;
		}
		if (num < this.minHitSpeed)
		{
			return;
		}
		double timeAsDouble = Time.timeAsDouble;
		if (timeAsDouble < this.hitCooldownEnd)
		{
			return;
		}
		Collider collider = collision.collider;
		GameHittable parentEnemy = this.GetParentEnemy<GameHittable>(collider);
		if (parentEnemy != null)
		{
			if (this.tool.HasEnoughEnergy())
			{
				Vector3 vector = parentEnemy.transform.position - base.transform.position;
				vector.Normalize();
				if (gamePlayer != null)
				{
					vector = GamePlayerLocal.instance.GetHandVelocity(gamePlayer.FindHandIndex(this.gameEntity.id)).normalized;
				}
				vector *= Mathf.Sqrt(num);
				Vector3 position = parentEnemy.transform.position;
				GameHitData hitData = new GameHitData
				{
					hitTypeId = 0,
					hitEntityId = parentEnemy.gameEntity.id,
					hitByEntityId = this.gameEntity.id,
					hitEntityPosition = position,
					hitImpulse = vector,
					hitPosition = collision.GetContact(0).point,
					hitAmount = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.BatonDamage)
				};
				if (parentEnemy.IsHitValid(hitData))
				{
					parentEnemy.RequestHit(hitData);
					this.hitCooldownEnd = timeAsDouble + 0.10000000149011612;
					return;
				}
			}
			else
			{
				this.OnHitFailedOutOfEnergy();
				this.hitCooldownEnd = timeAsDouble + 0.10000000149011612;
			}
		}
	}

	// Token: 0x06002888 RID: 10376 RVA: 0x000DA0C4 File Offset: 0x000D82C4
	private T GetParentEnemy<T>(Collider collider) where T : MonoBehaviour
	{
		Transform transform = collider.transform;
		while (transform != null)
		{
			T component = transform.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			transform = transform.parent;
		}
		return default(T);
	}

	// Token: 0x06002889 RID: 10377 RVA: 0x000DA10C File Offset: 0x000D830C
	private void OnHit()
	{
		this.audioSource.volume = this.hitWithEnergyVolume;
		this.audioSource.PlayOneShot(this.hitWithEnergyAudio);
		this.hitParticleEffect.Play();
		if (this.IsHeldLocal())
		{
			this.PlayVibration(GorillaTagger.Instance.tapHapticStrength, 0.2f);
			GamePlayer gamePlayer = GamePlayer.GetGamePlayer(this.gameEntity.heldByActorNumber);
			if (gamePlayer != null)
			{
				int num = gamePlayer.FindHandIndex(this.gameEntity.id);
				if (num != -1)
				{
					GTPlayer.Instance.TempFreezeHand(GamePlayer.IsLeftHand(num), 0.15f);
				}
			}
		}
	}

	// Token: 0x0600288A RID: 10378 RVA: 0x000DA1A8 File Offset: 0x000D83A8
	private void OnHitFailedOutOfEnergy()
	{
		this.PlayVibration(GorillaTagger.Instance.tapHapticStrength, 0.2f);
		this.audioSource.volume = this.hitEmptyVolume;
		this.audioSource.PlayOneShot(this.hitEmptyAudio);
	}

	// Token: 0x0600288B RID: 10379 RVA: 0x000DA1E4 File Offset: 0x000D83E4
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

	// Token: 0x0600288C RID: 10380 RVA: 0x000DA23E File Offset: 0x000D843E
	public void OnSuccessfulHit(GameHitData hitData)
	{
		this.tool.UseEnergy();
		this.OnHit();
	}

	// Token: 0x040033E5 RID: 13285
	public GameEntity gameEntity;

	// Token: 0x040033E6 RID: 13286
	public GRTool tool;

	// Token: 0x040033E7 RID: 13287
	public Rigidbody rigidBody;

	// Token: 0x040033E8 RID: 13288
	public AudioSource audioSource;

	// Token: 0x040033E9 RID: 13289
	public AudioSource humAudioSource;

	// Token: 0x040033EA RID: 13290
	public List<ParticleSystem> humParticleEffects = new List<ParticleSystem>();

	// Token: 0x040033EB RID: 13291
	public GRAttributes attributes;

	// Token: 0x040033EC RID: 13292
	public ParticleSystem hitParticleEffect;

	// Token: 0x040033ED RID: 13293
	public AudioClip hitWithEnergyAudio;

	// Token: 0x040033EE RID: 13294
	public float hitWithEnergyVolume = 0.5f;

	// Token: 0x040033EF RID: 13295
	public AudioClip hitEmptyAudio;

	// Token: 0x040033F0 RID: 13296
	public float hitEmptyVolume = 0.5f;

	// Token: 0x040033F1 RID: 13297
	public AudioClip extendAudio;

	// Token: 0x040033F2 RID: 13298
	public float extendVolume = 0.5f;

	// Token: 0x040033F3 RID: 13299
	public AudioClip retractAudio;

	// Token: 0x040033F4 RID: 13300
	public float retractVolume = 0.5f;

	// Token: 0x040033F5 RID: 13301
	public float minHitSpeed = 2.25f;

	// Token: 0x040033F6 RID: 13302
	public GameObject dullLight;

	// Token: 0x040033F7 RID: 13303
	public Material idleMaterial;

	// Token: 0x040033F8 RID: 13304
	public Material poweredMaterial;

	// Token: 0x040033F9 RID: 13305
	public MeshRenderer poweredMeshRenderer;

	// Token: 0x040033FA RID: 13306
	public Transform retractableSection;

	// Token: 0x040033FB RID: 13307
	public Collider idleCollider;

	// Token: 0x040033FC RID: 13308
	public Collider extendedCollider;

	// Token: 0x040033FD RID: 13309
	public float retractableSectionMin = -0.31f;

	// Token: 0x040033FE RID: 13310
	public float retractableSectionMax;

	// Token: 0x040033FF RID: 13311
	public float extensionTime = 0.15f;

	// Token: 0x04003400 RID: 13312
	private float extendedAmount;

	// Token: 0x04003401 RID: 13313
	private double hitCooldownEnd;

	// Token: 0x04003402 RID: 13314
	private GRToolClub.State state;

	// Token: 0x02000678 RID: 1656
	private enum State
	{
		// Token: 0x04003404 RID: 13316
		Idle,
		// Token: 0x04003405 RID: 13317
		Extended
	}
}
