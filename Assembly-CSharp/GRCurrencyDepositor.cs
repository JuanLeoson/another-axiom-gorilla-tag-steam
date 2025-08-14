using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000614 RID: 1556
public class GRCurrencyDepositor : MonoBehaviour
{
	// Token: 0x0600261F RID: 9759 RVA: 0x000CBE68 File Offset: 0x000CA068
	public void Init(GhostReactor reactor)
	{
		this.reactor = reactor;
	}

	// Token: 0x06002620 RID: 9760 RVA: 0x000CBE74 File Offset: 0x000CA074
	private void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody != null)
		{
			GRCollectible component = other.attachedRigidbody.GetComponent<GRCollectible>();
			if (component != null)
			{
				if ((component.unlockPointsCollectible && !this.collectSentientCores) || (!component.unlockPointsCollectible && this.collectSentientCores))
				{
					return;
				}
				if (this.reactor.grManager.IsAuthority())
				{
					this.reactor.grManager.RequestDepositCollectible(component.entity.id);
				}
				this.collectibleDepositedEffect.Play();
				this.audioSource.volume = this.collectibleDepositedClipVolume;
				this.audioSource.PlayOneShot(this.collectibleDepositedClip);
				if (component.entity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					if (GamePlayerLocal.instance.gamePlayer.GetGameEntityId(0) == component.entity.id)
					{
						GorillaTagger.Instance.StartVibration(true, 0.5f, 0.15f);
						return;
					}
					if (GamePlayerLocal.instance.gamePlayer.GetGameEntityId(1) == component.entity.id)
					{
						GorillaTagger.Instance.StartVibration(false, 0.5f, 0.15f);
					}
				}
			}
		}
	}

	// Token: 0x0400305D RID: 12381
	public Transform depositingChargePoint;

	// Token: 0x0400305E RID: 12382
	[SerializeField]
	private ParticleSystem collectibleDepositedEffect;

	// Token: 0x0400305F RID: 12383
	[SerializeField]
	private AudioClip collectibleDepositedClip;

	// Token: 0x04003060 RID: 12384
	[SerializeField]
	private float collectibleDepositedClipVolume;

	// Token: 0x04003061 RID: 12385
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04003062 RID: 12386
	[SerializeField]
	private bool collectSentientCores;

	// Token: 0x04003063 RID: 12387
	private const float hapticStrength = 0.5f;

	// Token: 0x04003064 RID: 12388
	private const float hapticDuration = 0.15f;

	// Token: 0x04003065 RID: 12389
	[NonSerialized]
	public GhostReactor reactor;
}
