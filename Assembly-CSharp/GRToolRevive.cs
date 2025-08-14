using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000684 RID: 1668
[RequireComponent(typeof(GameEntity))]
public class GRToolRevive : MonoBehaviour
{
	// Token: 0x060028E0 RID: 10464 RVA: 0x000DBECB File Offset: 0x000DA0CB
	private void Awake()
	{
		this.state = GRToolRevive.State.Idle;
	}

	// Token: 0x060028E1 RID: 10465 RVA: 0x000DBED4 File Offset: 0x000DA0D4
	private void OnEnable()
	{
		this.StopRevive();
		this.state = GRToolRevive.State.Idle;
	}

	// Token: 0x060028E2 RID: 10466 RVA: 0x000023F5 File Offset: 0x000005F5
	private void OnDestroy()
	{
	}

	// Token: 0x060028E3 RID: 10467 RVA: 0x000DBEE4 File Offset: 0x000DA0E4
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.IsHeldLocal())
		{
			this.OnUpdateAuthority(deltaTime);
			return;
		}
		this.OnUpdateRemote(deltaTime);
	}

	// Token: 0x060028E4 RID: 10468 RVA: 0x000DBF10 File Offset: 0x000DA110
	private void OnUpdateAuthority(float dt)
	{
		switch (this.state)
		{
		case GRToolRevive.State.Idle:
			if (this.tool.HasEnoughEnergy() && this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolRevive.State.Reviving);
				return;
			}
			break;
		case GRToolRevive.State.Reviving:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolRevive.State.Cooldown);
				return;
			}
			break;
		case GRToolRevive.State.Cooldown:
			if (!this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolRevive.State.Idle);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060028E5 RID: 10469 RVA: 0x000DBF88 File Offset: 0x000DA188
	private void OnUpdateRemote(float dt)
	{
		GRToolRevive.State state = (GRToolRevive.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x060028E6 RID: 10470 RVA: 0x000DBFB2 File Offset: 0x000DA1B2
	private void SetStateAuthority(GRToolRevive.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x060028E7 RID: 10471 RVA: 0x000DBFD4 File Offset: 0x000DA1D4
	private void SetState(GRToolRevive.State newState)
	{
		if (this.state == newState)
		{
			return;
		}
		if (this.state == GRToolRevive.State.Reviving)
		{
			this.StopRevive();
		}
		this.state = newState;
		GRToolRevive.State state = this.state;
		if (state != GRToolRevive.State.Idle)
		{
			if (state == GRToolRevive.State.Reviving)
			{
				this.StartRevive();
				this.stateTimeRemaining = this.reviveDuration;
				return;
			}
		}
		else
		{
			this.stateTimeRemaining = -1f;
		}
	}

	// Token: 0x060028E8 RID: 10472 RVA: 0x000DC030 File Offset: 0x000DA230
	private void StartRevive()
	{
		this.reviveFx.SetActive(true);
		this.audioSource.volume = this.reviveSoundVolume;
		this.audioSource.clip = this.reviveSound;
		this.audioSource.Play();
		this.tool.UseEnergy();
		if (this.gameEntity.IsAuthority())
		{
			int num = Physics.SphereCastNonAlloc(this.shootFrom.position, 0.5f, this.shootFrom.rotation * Vector3.forward, this.tempHitResults, this.reviveDistance, this.playerLayerMask);
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = this.tempHitResults[i];
				Rigidbody attachedRigidbody = raycastHit.collider.attachedRigidbody;
				if (!(attachedRigidbody == null))
				{
					GRPlayer component = attachedRigidbody.GetComponent<GRPlayer>();
					if (component != null && component.State != GRPlayer.GRPlayerState.Alive)
					{
						GhostReactorManager.Get(this.gameEntity).RequestPlayerStateChange(component, GRPlayer.GRPlayerState.Alive);
						return;
					}
				}
			}
		}
	}

	// Token: 0x060028E9 RID: 10473 RVA: 0x000DC131 File Offset: 0x000DA331
	private void StopRevive()
	{
		this.reviveFx.SetActive(false);
		this.audioSource.Stop();
	}

	// Token: 0x060028EA RID: 10474 RVA: 0x000DC14A File Offset: 0x000DA34A
	private bool IsHeldLocal()
	{
		return this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x060028EB RID: 10475 RVA: 0x000DC164 File Offset: 0x000DA364
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

	// Token: 0x040034A1 RID: 13473
	public GameEntity gameEntity;

	// Token: 0x040034A2 RID: 13474
	public GRTool tool;

	// Token: 0x040034A3 RID: 13475
	[SerializeField]
	private Transform shootFrom;

	// Token: 0x040034A4 RID: 13476
	[SerializeField]
	private LayerMask playerLayerMask;

	// Token: 0x040034A5 RID: 13477
	[SerializeField]
	private float reviveDistance = 1.5f;

	// Token: 0x040034A6 RID: 13478
	[SerializeField]
	private GameObject reviveFx;

	// Token: 0x040034A7 RID: 13479
	[SerializeField]
	private float reviveSoundVolume;

	// Token: 0x040034A8 RID: 13480
	[SerializeField]
	private AudioClip reviveSound;

	// Token: 0x040034A9 RID: 13481
	[SerializeField]
	private float reviveDuration = 0.75f;

	// Token: 0x040034AA RID: 13482
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040034AB RID: 13483
	private GRToolRevive.State state;

	// Token: 0x040034AC RID: 13484
	private float stateTimeRemaining;

	// Token: 0x040034AD RID: 13485
	private RaycastHit[] tempHitResults = new RaycastHit[128];

	// Token: 0x02000685 RID: 1669
	private enum State
	{
		// Token: 0x040034AF RID: 13487
		Idle,
		// Token: 0x040034B0 RID: 13488
		Reviving,
		// Token: 0x040034B1 RID: 13489
		Cooldown
	}
}
