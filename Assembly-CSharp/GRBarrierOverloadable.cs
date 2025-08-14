using System;
using UnityEngine;

// Token: 0x02000607 RID: 1543
public class GRBarrierOverloadable : MonoBehaviour
{
	// Token: 0x060025E8 RID: 9704 RVA: 0x000CAEE1 File Offset: 0x000C90E1
	private void OnEnable()
	{
		this.tool.OnEnergyChange += this.OnEnergyChange;
		this.gameEntity.OnStateChanged += this.OnEntityStateChanged;
	}

	// Token: 0x060025E9 RID: 9705 RVA: 0x000CAF14 File Offset: 0x000C9114
	private void OnEnergyChange(GRTool tool, int energyChange, GameEntityId chargingEntityId)
	{
		if (this.state == GRBarrierOverloadable.State.Active && tool.energy >= tool.GetEnergyMax())
		{
			this.SetState(GRBarrierOverloadable.State.Destroyed);
			if (this.gameEntity.IsAuthority())
			{
				this.gameEntity.RequestState(this.gameEntity.id, 1L);
			}
		}
	}

	// Token: 0x060025EA RID: 9706 RVA: 0x000CAF63 File Offset: 0x000C9163
	private void OnEntityStateChanged(long prevState, long nextState)
	{
		if (!this.gameEntity.IsAuthority())
		{
			this.SetState((GRBarrierOverloadable.State)nextState);
		}
	}

	// Token: 0x060025EB RID: 9707 RVA: 0x000CAF7C File Offset: 0x000C917C
	public void SetState(GRBarrierOverloadable.State newState)
	{
		if (this.state != newState)
		{
			this.state = newState;
			GRBarrierOverloadable.State state = this.state;
			if (state == GRBarrierOverloadable.State.Active)
			{
				this.meshRenderer.enabled = true;
				this.collider.enabled = true;
				return;
			}
			if (state != GRBarrierOverloadable.State.Destroyed)
			{
				return;
			}
			this.audioSource.Play();
			this.meshRenderer.enabled = false;
			this.collider.enabled = false;
		}
	}

	// Token: 0x0400300C RID: 12300
	public GRTool tool;

	// Token: 0x0400300D RID: 12301
	public GameEntity gameEntity;

	// Token: 0x0400300E RID: 12302
	public AudioSource audioSource;

	// Token: 0x0400300F RID: 12303
	public MeshRenderer meshRenderer;

	// Token: 0x04003010 RID: 12304
	public Collider collider;

	// Token: 0x04003011 RID: 12305
	private GRBarrierOverloadable.State state;

	// Token: 0x02000608 RID: 1544
	public enum State
	{
		// Token: 0x04003013 RID: 12307
		Active,
		// Token: 0x04003014 RID: 12308
		Destroyed
	}
}
