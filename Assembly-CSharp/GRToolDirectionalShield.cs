using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x0200067B RID: 1659
public class GRToolDirectionalShield : MonoBehaviour, IGameHitter
{
	// Token: 0x060028A0 RID: 10400 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Awake()
	{
	}

	// Token: 0x060028A1 RID: 10401 RVA: 0x000DA9E6 File Offset: 0x000D8BE6
	public void OnEnable()
	{
		this.SetState(GRToolDirectionalShield.State.Closed);
	}

	// Token: 0x060028A2 RID: 10402 RVA: 0x000DA9EF File Offset: 0x000D8BEF
	private bool IsHeldLocal()
	{
		return this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x060028A3 RID: 10403 RVA: 0x000DAA08 File Offset: 0x000D8C08
	private bool IsHeld()
	{
		return this.gameEntity.heldByActorNumber != -1;
	}

	// Token: 0x060028A4 RID: 10404 RVA: 0x000DAA1C File Offset: 0x000D8C1C
	public void BlockHittable(Vector3 enemyPosition, Vector3 enemyAttackDirection, GameHittable hittable, GRShieldCollider shieldCollider)
	{
		if (this.IsHeldLocal())
		{
			Vector3 vector = -hittable.transform.forward;
			vector.y = 0f;
			Vector3 hitImpulse = vector.normalized * shieldCollider.KnockbackVelocity;
			GameHitData hitData = new GameHitData
			{
				hitTypeId = 2,
				hitEntityId = hittable.gameEntity.id,
				hitByEntityId = this.gameEntity.id,
				hitEntityPosition = enemyPosition,
				hitImpulse = hitImpulse,
				hitPosition = enemyPosition,
				hitAmount = 0
			};
			if (hittable.IsHitValid(hitData))
			{
				hittable.RequestHit(hitData);
			}
		}
	}

	// Token: 0x060028A5 RID: 10405 RVA: 0x000DAACB File Offset: 0x000D8CCB
	public void OnEnemyBlocked(Vector3 enemyPosition)
	{
		this.tool.UseEnergy();
		this.PlayBlockEffects(enemyPosition);
	}

	// Token: 0x060028A6 RID: 10406 RVA: 0x000DAAE0 File Offset: 0x000D8CE0
	private void PlayBlockEffects(Vector3 enemyPosition)
	{
		this.audioSource.PlayOneShot(this.deflectAudio, this.deflectVolume);
		this.shieldDeflectVFX.Play();
		Vector3 b = Vector3.ClampMagnitude(enemyPosition - this.shieldArcCenterReferencePoint.position, this.shieldArcCenterRadius);
		Vector3 position = this.shieldArcCenterReferencePoint.position + b;
		this.shieldDeflectImpactPointVFX.transform.position = position;
		this.shieldDeflectImpactPointVFX.Play();
	}

	// Token: 0x060028A7 RID: 10407 RVA: 0x000DAB5A File Offset: 0x000D8D5A
	public void OnSuccessfulHit(GameHitData hitData)
	{
		this.tool.UseEnergy();
		this.PlayBlockEffects(hitData.hitEntityPosition);
	}

	// Token: 0x060028A8 RID: 10408 RVA: 0x000DAB74 File Offset: 0x000D8D74
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (!this.IsHeld())
		{
			this.SetState(GRToolDirectionalShield.State.Closed);
			return;
		}
		if (this.IsHeldLocal())
		{
			this.OnUpdateAuthority(deltaTime);
			return;
		}
		this.OnUpdateRemote(deltaTime);
	}

	// Token: 0x060028A9 RID: 10409 RVA: 0x000DABB0 File Offset: 0x000D8DB0
	private void OnUpdateAuthority(float dt)
	{
		GRToolDirectionalShield.State state = this.state;
		if (state != GRToolDirectionalShield.State.Closed)
		{
			if (state != GRToolDirectionalShield.State.Open)
			{
				return;
			}
			if (!this.IsButtonHeld() || !this.tool.HasEnoughEnergy())
			{
				this.SetStateAuthority(GRToolDirectionalShield.State.Closed);
			}
		}
		else if (this.IsButtonHeld() && this.tool.HasEnoughEnergy())
		{
			this.SetStateAuthority(GRToolDirectionalShield.State.Open);
			return;
		}
	}

	// Token: 0x060028AA RID: 10410 RVA: 0x000DAC10 File Offset: 0x000D8E10
	private void OnUpdateRemote(float dt)
	{
		GRToolDirectionalShield.State state = (GRToolDirectionalShield.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x060028AB RID: 10411 RVA: 0x000DAC3A File Offset: 0x000D8E3A
	private void SetStateAuthority(GRToolDirectionalShield.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x060028AC RID: 10412 RVA: 0x000DAC5C File Offset: 0x000D8E5C
	private void SetState(GRToolDirectionalShield.State newState)
	{
		if (this.state == newState)
		{
			return;
		}
		GRToolDirectionalShield.State state = this.state;
		if (state != GRToolDirectionalShield.State.Closed)
		{
		}
		this.state = newState;
		state = this.state;
		if (state == GRToolDirectionalShield.State.Closed)
		{
			this.openCollidersParent.gameObject.SetActive(false);
			this.shieldAnimator.SetBool("Activated", false);
			this.audioSource.PlayOneShot(this.closeAudio, this.closeVolume);
			return;
		}
		if (state != GRToolDirectionalShield.State.Open)
		{
			return;
		}
		this.openCollidersParent.gameObject.SetActive(true);
		this.shieldAnimator.SetBool("Activated", true);
		this.audioSource.PlayOneShot(this.openAudio, this.openVolume);
	}

	// Token: 0x060028AD RID: 10413 RVA: 0x000DAD0C File Offset: 0x000D8F0C
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

	// Token: 0x04003424 RID: 13348
	[Header("References")]
	public GameEntity gameEntity;

	// Token: 0x04003425 RID: 13349
	public GRTool tool;

	// Token: 0x04003426 RID: 13350
	public Rigidbody rigidBody;

	// Token: 0x04003427 RID: 13351
	public AudioSource audioSource;

	// Token: 0x04003428 RID: 13352
	public Animator shieldAnimator;

	// Token: 0x04003429 RID: 13353
	public Transform openCollidersParent;

	// Token: 0x0400342A RID: 13354
	[Header("Audio")]
	public AudioClip openAudio;

	// Token: 0x0400342B RID: 13355
	public float openVolume = 0.5f;

	// Token: 0x0400342C RID: 13356
	public AudioClip closeAudio;

	// Token: 0x0400342D RID: 13357
	public float closeVolume = 0.5f;

	// Token: 0x0400342E RID: 13358
	public AudioClip deflectAudio;

	// Token: 0x0400342F RID: 13359
	public float deflectVolume = 0.5f;

	// Token: 0x04003430 RID: 13360
	[Header("VFX")]
	public ParticleSystem shieldDeflectVFX;

	// Token: 0x04003431 RID: 13361
	public ParticleSystem shieldDeflectImpactPointVFX;

	// Token: 0x04003432 RID: 13362
	public Transform shieldArcCenterReferencePoint;

	// Token: 0x04003433 RID: 13363
	public float shieldArcCenterRadius = 1f;

	// Token: 0x04003434 RID: 13364
	private GRToolDirectionalShield.State state;

	// Token: 0x0200067C RID: 1660
	private enum State
	{
		// Token: 0x04003436 RID: 13366
		Closed,
		// Token: 0x04003437 RID: 13367
		Open
	}
}
