using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000640 RID: 1600
public class GRHazardousMaterial : MonoBehaviour
{
	// Token: 0x06002764 RID: 10084 RVA: 0x000D49AB File Offset: 0x000D2BAB
	public void Init(GhostReactor reactor)
	{
		this.reactor = reactor;
	}

	// Token: 0x06002765 RID: 10085 RVA: 0x000D49B4 File Offset: 0x000D2BB4
	public void OnLocalPlayerOverlap()
	{
		GRPlayer component = VRRig.LocalRig.GetComponent<GRPlayer>();
		if (component != null && component.State == GRPlayer.GRPlayerState.Alive)
		{
			this.reactor.grManager.RequestPlayerStateChange(component, GRPlayer.GRPlayerState.Ghost);
		}
	}

	// Token: 0x06002766 RID: 10086 RVA: 0x000D49EF File Offset: 0x000D2BEF
	private void OnTriggerEnter(Collider collider)
	{
		if (collider == GTPlayer.Instance.headCollider || collider == GTPlayer.Instance.bodyCollider)
		{
			this.OnLocalPlayerOverlap();
		}
	}

	// Token: 0x06002767 RID: 10087 RVA: 0x000D4A1B File Offset: 0x000D2C1B
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider == GTPlayer.Instance.headCollider || collision.collider == GTPlayer.Instance.bodyCollider)
		{
			this.OnLocalPlayerOverlap();
		}
	}

	// Token: 0x04003292 RID: 12946
	private GhostReactor reactor;
}
