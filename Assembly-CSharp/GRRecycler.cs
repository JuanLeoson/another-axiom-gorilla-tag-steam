using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000657 RID: 1623
public class GRRecycler : MonoBehaviour
{
	// Token: 0x060027BE RID: 10174 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Awake()
	{
	}

	// Token: 0x060027BF RID: 10175 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Start()
	{
	}

	// Token: 0x060027C0 RID: 10176 RVA: 0x000023F5 File Offset: 0x000005F5
	private void OnDestroy()
	{
	}

	// Token: 0x060027C1 RID: 10177 RVA: 0x000D6474 File Offset: 0x000D4674
	public void Update()
	{
		if (this.closed && !this.anim.isPlaying)
		{
			if (!this.playedAudio)
			{
				this.audioSource.volume = this.recyclerRunningAudioVolume;
				this.audioSource.PlayOneShot(this.recyclerRunningAudio);
				this.playedAudio = true;
			}
			this.timeRemaining -= Time.deltaTime;
			if (this.timeRemaining <= 0f)
			{
				this.anim.PlayQueued("Recycler_Open", QueueMode.CompleteOthers);
				this.closed = false;
				if (this.closeEffects != null && this.openEffects != null)
				{
					this.closeEffects.Stop();
					this.openEffects.Play();
				}
			}
		}
	}

	// Token: 0x060027C2 RID: 10178 RVA: 0x000D6537 File Offset: 0x000D4737
	public void Init(GhostReactor reactor)
	{
		this.reactor = reactor;
	}

	// Token: 0x060027C3 RID: 10179 RVA: 0x000D6540 File Offset: 0x000D4740
	public int GetRecycleValue(GRRecycler.GRToolType type)
	{
		foreach (GRRecycler.GRRecyclePair grrecyclePair in this.recycleValues)
		{
			if (grrecyclePair.type == type)
			{
				return grrecyclePair.value;
			}
		}
		return 0;
	}

	// Token: 0x060027C4 RID: 10180 RVA: 0x000D65A4 File Offset: 0x000D47A4
	public void ScanItem(GRRecycler.GRToolType toolType)
	{
		this.scanner.ScanItem(toolType);
	}

	// Token: 0x060027C5 RID: 10181 RVA: 0x000D65B4 File Offset: 0x000D47B4
	public void RecycleItem()
	{
		if (this.anim != null)
		{
			this.anim.Play("Recycler_Close");
		}
		if (this.closeEffects != null && this.openEffects != null)
		{
			this.openEffects.Stop();
			this.closeEffects.Play();
		}
		this.closed = true;
		this.playedAudio = false;
		this.timeRemaining = this.closeDuration;
	}

	// Token: 0x060027C6 RID: 10182 RVA: 0x000D662C File Offset: 0x000D482C
	private void OnTriggerEnter(Collider other)
	{
		if (this.reactor == null)
		{
			return;
		}
		if (!this.reactor.grManager.IsAuthority())
		{
			return;
		}
		GRRecycler.GRToolType grtoolType = GRRecycler.GRToolType.None;
		GRTool componentInParent = other.gameObject.GetComponentInParent<GRTool>();
		if (componentInParent == null)
		{
			return;
		}
		if (other.gameObject.GetComponentInParent<GRToolClub>() != null)
		{
			grtoolType = GRRecycler.GRToolType.Club;
		}
		else if (other.gameObject.GetComponentInParent<GRToolCollector>() != null)
		{
			grtoolType = GRRecycler.GRToolType.Collector;
		}
		else if (other.gameObject.GetComponentInParent<GRToolFlash>() != null)
		{
			grtoolType = GRRecycler.GRToolType.Flash;
		}
		else if (other.gameObject.GetComponentInParent<GRToolLantern>() != null)
		{
			grtoolType = GRRecycler.GRToolType.Lantern;
		}
		else if (other.gameObject.GetComponentInParent<GRToolRevive>() != null)
		{
			grtoolType = GRRecycler.GRToolType.Revive;
		}
		else if (other.gameObject.GetComponentInParent<GRToolShieldGun>() != null)
		{
			grtoolType = GRRecycler.GRToolType.ShieldGun;
		}
		else if (other.gameObject.GetComponentInParent<GRToolDirectionalShield>() != null)
		{
			grtoolType = GRRecycler.GRToolType.DirectionalShield;
		}
		this.GetRecycleValue(grtoolType);
		if (GRPlayer.Get(componentInParent.gameEntity.lastHeldByActorNumber) == null)
		{
			return;
		}
		this.reactor.grManager.RequestRecycleItem(componentInParent.gameEntity.lastHeldByActorNumber, componentInParent.gameEntity.id, grtoolType);
	}

	// Token: 0x04003327 RID: 13095
	private GameEntity gameEntity;

	// Token: 0x04003328 RID: 13096
	public ParticleSystem closeEffects;

	// Token: 0x04003329 RID: 13097
	public ParticleSystem openEffects;

	// Token: 0x0400332A RID: 13098
	public List<GRRecycler.GRRecyclePair> recycleValues = new List<GRRecycler.GRRecyclePair>();

	// Token: 0x0400332B RID: 13099
	[NonSerialized]
	public GhostReactor reactor;

	// Token: 0x0400332C RID: 13100
	public GRRecyclerScanner scanner;

	// Token: 0x0400332D RID: 13101
	public Animation anim;

	// Token: 0x0400332E RID: 13102
	public float closeDuration = 1f;

	// Token: 0x0400332F RID: 13103
	private float timeRemaining;

	// Token: 0x04003330 RID: 13104
	private bool closed;

	// Token: 0x04003331 RID: 13105
	private bool playedAudio;

	// Token: 0x04003332 RID: 13106
	public AudioSource audioSource;

	// Token: 0x04003333 RID: 13107
	public AudioClip recyclerRunningAudio;

	// Token: 0x04003334 RID: 13108
	public float recyclerRunningAudioVolume = 0.5f;

	// Token: 0x02000658 RID: 1624
	public enum GRToolType
	{
		// Token: 0x04003336 RID: 13110
		None,
		// Token: 0x04003337 RID: 13111
		Club,
		// Token: 0x04003338 RID: 13112
		Collector,
		// Token: 0x04003339 RID: 13113
		Flash,
		// Token: 0x0400333A RID: 13114
		Lantern,
		// Token: 0x0400333B RID: 13115
		Revive,
		// Token: 0x0400333C RID: 13116
		ShieldGun,
		// Token: 0x0400333D RID: 13117
		DirectionalShield
	}

	// Token: 0x02000659 RID: 1625
	[Serializable]
	public struct GRRecyclePair
	{
		// Token: 0x0400333E RID: 13118
		public GRRecycler.GRToolType type;

		// Token: 0x0400333F RID: 13119
		public int value;
	}
}
