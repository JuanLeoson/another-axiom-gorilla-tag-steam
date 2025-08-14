using System;
using UnityEngine;

// Token: 0x02000611 RID: 1553
public class GRCollectible : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x0600260D RID: 9741 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Awake()
	{
	}

	// Token: 0x0600260E RID: 9742 RVA: 0x000CB9AC File Offset: 0x000C9BAC
	public void OnEntityInit()
	{
		GameEntityManager manager = this.entity.manager;
		GameEntity gameEntity = manager.GetGameEntity(manager.GetEntityIdFromNetId((int)this.entity.createData));
		if (gameEntity != null)
		{
			GRCollectibleDispenser component = gameEntity.GetComponent<GRCollectibleDispenser>();
			if (component != null)
			{
				component.GetSpawnedCollectible(this);
			}
		}
	}

	// Token: 0x0600260F RID: 9743 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06002610 RID: 9744 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002611 RID: 9745 RVA: 0x000CB9FC File Offset: 0x000C9BFC
	public void InvokeOnCollected()
	{
		Action onCollected = this.OnCollected;
		if (onCollected == null)
		{
			return;
		}
		onCollected();
	}

	// Token: 0x04003042 RID: 12354
	public GameEntity entity;

	// Token: 0x04003043 RID: 12355
	public int energyValue = 100;

	// Token: 0x04003044 RID: 12356
	public bool unlockPointsCollectible;

	// Token: 0x04003045 RID: 12357
	public Action OnCollected;
}
