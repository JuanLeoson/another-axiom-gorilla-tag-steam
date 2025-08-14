using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005BB RID: 1467
public class GameHittable : MonoBehaviour
{
	// Token: 0x06002403 RID: 9219 RVA: 0x000C1228 File Offset: 0x000BF428
	private void Awake()
	{
		this.components = new List<IGameHittable>(1);
		base.GetComponentsInChildren<IGameHittable>(this.components);
	}

	// Token: 0x06002404 RID: 9220 RVA: 0x000C1242 File Offset: 0x000BF442
	public void RequestHit(GameHitData hitData)
	{
		hitData.hitEntityId = this.gameEntity.id;
		this.gameEntity.manager.RequestHit(hitData);
	}

	// Token: 0x06002405 RID: 9221 RVA: 0x000C1268 File Offset: 0x000BF468
	public void ApplyHit(GameHitData hitData)
	{
		for (int i = 0; i < this.components.Count; i++)
		{
			this.components[i].OnHit(hitData);
		}
		GameEntity gameEntity = this.gameEntity.manager.GetGameEntity(hitData.hitByEntityId);
		if (gameEntity != null)
		{
			List<IGameHitter> list = new List<IGameHitter>(1);
			gameEntity.GetComponents<IGameHitter>(list);
			for (int j = 0; j < list.Count; j++)
			{
				list[j].OnSuccessfulHit(hitData);
			}
		}
	}

	// Token: 0x06002406 RID: 9222 RVA: 0x000C12EC File Offset: 0x000BF4EC
	public bool IsHitValid(GameHitData hitData)
	{
		for (int i = 0; i < this.components.Count; i++)
		{
			if (!this.components[i].IsHitValid(hitData))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x04002D77 RID: 11639
	public GameEntity gameEntity;

	// Token: 0x04002D78 RID: 11640
	private List<IGameHittable> components;
}
