using System;
using UnityEngine;

// Token: 0x02000664 RID: 1636
public class GRReviveStation : MonoBehaviour
{
	// Token: 0x170003BA RID: 954
	// (get) Token: 0x0600281A RID: 10266 RVA: 0x000D8726 File Offset: 0x000D6926
	// (set) Token: 0x0600281B RID: 10267 RVA: 0x000D872E File Offset: 0x000D692E
	public int Index { get; set; }

	// Token: 0x0600281C RID: 10268 RVA: 0x000D8737 File Offset: 0x000D6937
	public void Init(GhostReactor reactor, int index)
	{
		this.reactor = reactor;
		this.Index = index;
	}

	// Token: 0x0600281D RID: 10269 RVA: 0x000D8748 File Offset: 0x000D6948
	public void RevivePlayer(GRPlayer player)
	{
		if (player.State != GRPlayer.GRPlayerState.Alive || player.Hp < player.MaxHp)
		{
			player.OnPlayerRevive(this.reactor.grManager);
			this.audioSource.Play();
			if (this.particleEffects != null)
			{
				for (int i = 0; i < this.particleEffects.Length; i++)
				{
					this.particleEffects[i].Play();
				}
			}
		}
	}

	// Token: 0x0600281E RID: 10270 RVA: 0x000D87B0 File Offset: 0x000D69B0
	private void OnTriggerEnter(Collider collider)
	{
		Rigidbody attachedRigidbody = collider.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			VRRig component = attachedRigidbody.GetComponent<VRRig>();
			if (component != null)
			{
				GRPlayer component2 = component.GetComponent<GRPlayer>();
				if (component2 != null && (component2.State != GRPlayer.GRPlayerState.Alive || component2.Hp < component2.MaxHp))
				{
					if (!NetworkSystem.Instance.InRoom && component == VRRig.LocalRig)
					{
						this.RevivePlayer(component2);
					}
					this.reactor.grManager.RequestPlayerRevive(this, component2);
				}
			}
		}
	}

	// Token: 0x0400338F RID: 13199
	public AudioSource audioSource;

	// Token: 0x04003390 RID: 13200
	public ParticleSystem[] particleEffects;

	// Token: 0x04003392 RID: 13202
	private GhostReactor reactor;
}
