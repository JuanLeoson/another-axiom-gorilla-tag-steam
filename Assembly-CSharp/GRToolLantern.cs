using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000680 RID: 1664
[RequireComponent(typeof(GameEntity))]
public class GRToolLantern : MonoBehaviour
{
	// Token: 0x060028BF RID: 10431 RVA: 0x000DB366 File Offset: 0x000D9566
	private void Awake()
	{
		this.state = GRToolLantern.State.Off;
		this.gameEntity.OnStateChanged += this.OnStateChanged;
	}

	// Token: 0x060028C0 RID: 10432 RVA: 0x000DB386 File Offset: 0x000D9586
	private void OnEnable()
	{
		this.TurnOff();
		this.state = GRToolLantern.State.Off;
	}

	// Token: 0x060028C1 RID: 10433 RVA: 0x000023F5 File Offset: 0x000005F5
	private void OnDestroy()
	{
	}

	// Token: 0x060028C2 RID: 10434 RVA: 0x000DB398 File Offset: 0x000D9598
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.IsHeldLocal() || this.tool.energy > 0)
		{
			this.OnUpdateAuthority(deltaTime);
			return;
		}
		this.OnUpdateRemote(deltaTime);
	}

	// Token: 0x060028C3 RID: 10435 RVA: 0x000DB3D0 File Offset: 0x000D95D0
	private void OnUpdateAuthority(float dt)
	{
		GRToolLantern.State state = this.state;
		if (state != GRToolLantern.State.Off)
		{
			if (state != GRToolLantern.State.On)
			{
				return;
			}
			this.timeOnSpentEnergy -= dt;
			if ((!this.IsButtonHeld() && this.timeOnSpentEnergy <= 0f) || this.tool.energy <= 0)
			{
				this.SetState(GRToolLantern.State.Off);
				this.gameEntity.RequestState(this.gameEntity.id, 0L);
				return;
			}
			if (this.IsButtonHeld() && this.timeOnSpentEnergy <= 0f)
			{
				this.TryConsumeEnergy();
			}
		}
		else if (this.IsButtonHeld() && this.tool.energy > 0)
		{
			this.SetState(GRToolLantern.State.On);
			this.gameEntity.RequestState(this.gameEntity.id, 1L);
			return;
		}
	}

	// Token: 0x060028C4 RID: 10436 RVA: 0x000DB494 File Offset: 0x000D9694
	private void TryConsumeEnergy()
	{
		int num = Mathf.Min(this.tool.energy, this.minEnergyPerUse);
		if (num > 0)
		{
			this.tool.SetEnergy(this.tool.energy - num);
			this.timeOnSpentEnergy = this.fullChargeDurationSeconds * (float)num / (float)this.tool.GetEnergyMax();
		}
	}

	// Token: 0x060028C5 RID: 10437 RVA: 0x000DB4F0 File Offset: 0x000D96F0
	private void OnUpdateRemote(float dt)
	{
		GRToolLantern.State state = (GRToolLantern.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x060028C6 RID: 10438 RVA: 0x000DB51C File Offset: 0x000D971C
	private void SetState(GRToolLantern.State newState)
	{
		if (this.state == newState)
		{
			return;
		}
		if (!this.CanChangeState((long)newState))
		{
			return;
		}
		this.state = newState;
		GRToolLantern.State state = this.state;
		if (state != GRToolLantern.State.Off)
		{
			if (state == GRToolLantern.State.On)
			{
				this.TurnOn();
				return;
			}
		}
		else
		{
			this.TurnOff();
		}
	}

	// Token: 0x060028C7 RID: 10439 RVA: 0x000DB560 File Offset: 0x000D9760
	private void TurnOn()
	{
		this.gameLight.light.intensity = (float)this.attributes.CalculateFinalValueForAttribute(GRAttributeType.LightIntensity);
		this.gameLight.gameObject.SetActive(true);
		this.audioSource.PlayOneShot(this.turnOnSound, this.turnOnSoundVolume);
		this.meshRenderer.material = this.onMaterial;
		this.timeLastTurnedOn = Time.time;
	}

	// Token: 0x060028C8 RID: 10440 RVA: 0x000DB5CE File Offset: 0x000D97CE
	private void TurnOff()
	{
		this.gameLight.gameObject.SetActive(false);
		this.meshRenderer.material = this.offMaterial;
	}

	// Token: 0x060028C9 RID: 10441 RVA: 0x000DB5F2 File Offset: 0x000D97F2
	private bool IsHeldLocal()
	{
		return this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x060028CA RID: 10442 RVA: 0x000DB60C File Offset: 0x000D980C
	private bool IsButtonHeld()
	{
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(this.gameEntity.heldByActorNumber);
		if (gamePlayer == null)
		{
			return false;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		if (num == -1)
		{
			return false;
		}
		if (!GamePlayer.IsLeftHand(num))
		{
			return gamePlayer.rig.rightIndex.calcT > 0.25f;
		}
		return gamePlayer.rig.leftIndex.calcT > 0.25f;
	}

	// Token: 0x060028CB RID: 10443 RVA: 0x000023F5 File Offset: 0x000005F5
	private void OnStateChanged(long prevState, long nextState)
	{
	}

	// Token: 0x060028CC RID: 10444 RVA: 0x000DB684 File Offset: 0x000D9884
	public bool CanChangeState(long newStateIndex)
	{
		if (newStateIndex < 0L || newStateIndex >= 2L)
		{
			return false;
		}
		GRToolLantern.State state = (GRToolLantern.State)newStateIndex;
		if (state != GRToolLantern.State.Off)
		{
			return state == GRToolLantern.State.On && this.tool.energy > 0;
		}
		return Time.time > this.timeLastTurnedOn + this.minOnDuration || this.tool.energy <= 0;
	}

	// Token: 0x04003459 RID: 13401
	public GameEntity gameEntity;

	// Token: 0x0400345A RID: 13402
	public GRTool tool;

	// Token: 0x0400345B RID: 13403
	public GameLight gameLight;

	// Token: 0x0400345C RID: 13404
	public GRAttributes attributes;

	// Token: 0x0400345D RID: 13405
	[SerializeField]
	private float fullChargeDurationSeconds = 120f;

	// Token: 0x0400345E RID: 13406
	[SerializeField]
	private int minEnergyPerUse = 1;

	// Token: 0x0400345F RID: 13407
	[SerializeField]
	private float turnOnSoundVolume;

	// Token: 0x04003460 RID: 13408
	[SerializeField]
	private AudioClip turnOnSound;

	// Token: 0x04003461 RID: 13409
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04003462 RID: 13410
	[SerializeField]
	private MeshRenderer meshRenderer;

	// Token: 0x04003463 RID: 13411
	[SerializeField]
	private Material offMaterial;

	// Token: 0x04003464 RID: 13412
	[SerializeField]
	private Material onMaterial;

	// Token: 0x04003465 RID: 13413
	private float timeOnSpentEnergy;

	// Token: 0x04003466 RID: 13414
	private float timeLastTurnedOn;

	// Token: 0x04003467 RID: 13415
	private float minOnDuration = 0.5f;

	// Token: 0x04003468 RID: 13416
	private GRToolLantern.State state;

	// Token: 0x02000681 RID: 1665
	private enum State
	{
		// Token: 0x0400346A RID: 13418
		Off,
		// Token: 0x0400346B RID: 13419
		On,
		// Token: 0x0400346C RID: 13420
		Count
	}
}
