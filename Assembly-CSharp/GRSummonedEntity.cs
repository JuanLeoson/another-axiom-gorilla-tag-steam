using System;
using UnityEngine;

// Token: 0x02000670 RID: 1648
public class GRSummonedEntity : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x0600284D RID: 10317 RVA: 0x000D9199 File Offset: 0x000D7399
	private void Awake()
	{
		this.entity = base.GetComponent<GameEntity>();
	}

	// Token: 0x0600284E RID: 10318 RVA: 0x000D91A8 File Offset: 0x000D73A8
	public void OnEntityInit()
	{
		this.summonerNetID = (int)this.entity.createData;
		if (this.summonerNetID != 0)
		{
			Debug.Log(string.Format("OnEntityInit GRSummonedEntity {0} has summoner id of {1}", this.entity.ToString(), this.summonerNetID), this);
			this.summoner = this.FindSummoner();
			if (this.summoner != null)
			{
				Debug.Log(string.Format("OnEntityInit GRSummonedEntity found summoner {0}", this.summoner.ToString()), this);
				this.summoner.AddTrackedEntity(this.entity);
			}
		}
	}

	// Token: 0x0600284F RID: 10319 RVA: 0x000D923B File Offset: 0x000D743B
	public int GetSummonerNetID()
	{
		return this.summonerNetID;
	}

	// Token: 0x06002850 RID: 10320 RVA: 0x000D9244 File Offset: 0x000D7444
	public void OnEntityDestroy()
	{
		Debug.Log(string.Format("OnEntityDestroy GRSummonedEntity {0}", this.entity.ToString()), this);
		if (this.summoner != null)
		{
			Debug.Log(string.Format("OnEntityDestroy removing GRSummonedEntity {0} from summoner {1}", this.entity.ToString(), this.summoner.ToString()), this);
			this.summoner.RemoveTrackedEntity(this.entity);
		}
	}

	// Token: 0x06002851 RID: 10321 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002852 RID: 10322 RVA: 0x000D92B4 File Offset: 0x000D74B4
	private GREnemySummoner FindSummoner()
	{
		if (this.summonerNetID != 0)
		{
			GameEntityManager gameEntityManager = GhostReactorManager.Get(this.entity).gameEntityManager;
			GameEntityId entityIdFromNetId = gameEntityManager.GetEntityIdFromNetId(this.summonerNetID);
			GameEntity gameEntity = gameEntityManager.GetGameEntity(entityIdFromNetId);
			if (gameEntity != null)
			{
				Debug.Log(string.Format("GetSummonerScript summoner entity is valid {0} ", gameEntity.ToString()), this);
				return gameEntity.GetComponent<GREnemySummoner>();
			}
		}
		return null;
	}

	// Token: 0x040033CA RID: 13258
	private int summonerNetID;

	// Token: 0x040033CB RID: 13259
	private GameEntity entity;

	// Token: 0x040033CC RID: 13260
	private GREnemySummoner summoner;
}
