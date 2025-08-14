using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000612 RID: 1554
public class GRCollectibleDispenser : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x1700039D RID: 925
	// (get) Token: 0x06002613 RID: 9747 RVA: 0x000CBA1E File Offset: 0x000C9C1E
	public bool CollectibleAlreadySpawned
	{
		get
		{
			return this.currentCollectible != null;
		}
	}

	// Token: 0x1700039E RID: 926
	// (get) Token: 0x06002614 RID: 9748 RVA: 0x000CBA2C File Offset: 0x000C9C2C
	public bool ReadyToDispenseNewCollectible
	{
		get
		{
			double num = (double)this.collectibleRespawnTimeMinutes * 60.0;
			bool flag = (ulong)this.collectiblesDispensed < (ulong)((long)this.maxDispenseCount);
			return !this.CollectibleAlreadySpawned && flag && Time.timeAsDouble - this.collectibleDispenseRequestTime > num && Time.timeAsDouble - this.collectibleDispenseTime > num && Time.timeAsDouble - this.collectibleCollectedTime > num;
		}
	}

	// Token: 0x06002615 RID: 9749 RVA: 0x000CBA98 File Offset: 0x000C9C98
	public void OnEntityInit()
	{
		GhostReactor reactor = GhostReactorManager.Get(this.gameEntity).reactor;
		if (reactor != null)
		{
			reactor.collectibleDispensers.Add(this);
		}
	}

	// Token: 0x06002616 RID: 9750 RVA: 0x000CBACC File Offset: 0x000C9CCC
	public void OnEntityDestroy()
	{
		GhostReactorManager ghostReactorManager = GhostReactorManager.Get(this.gameEntity);
		if (ghostReactorManager != null && ghostReactorManager.reactor != null)
		{
			ghostReactorManager.reactor.collectibleDispensers.Remove(this);
		}
	}

	// Token: 0x06002617 RID: 9751 RVA: 0x000CBB10 File Offset: 0x000C9D10
	public void OnEntityStateChange(long prevState, long nextState)
	{
		uint num = this.collectiblesDispensed;
		uint num2 = this.collectiblesCollected;
		this.collectiblesDispensed = (uint)(nextState >> 32);
		this.collectiblesCollected = (uint)(nextState & (long)((ulong)-1));
		if (num != this.collectiblesDispensed)
		{
			this.collectibleDispenseTime = Time.timeAsDouble;
		}
		if (num2 != this.collectiblesCollected)
		{
			this.collectibleCollectedTime = Time.timeAsDouble;
		}
		if ((ulong)this.collectiblesCollected >= (ulong)((long)this.maxDispenseCount))
		{
			this.stillDispensingModel.gameObject.SetActive(false);
			this.fullyConsumedModel.gameObject.SetActive(true);
		}
	}

	// Token: 0x06002618 RID: 9752 RVA: 0x000CBB9C File Offset: 0x000C9D9C
	public void RequestDispenseCollectible()
	{
		if (this.ReadyToDispenseNewCollectible && this.gameEntity.IsAuthority())
		{
			this.gameEntity.manager.RequestCreateItem(this.collectiblePrefab.name.GetStaticHash(), this.spawnLocation.position, this.spawnLocation.rotation, (long)this.gameEntity.manager.GetNetIdFromEntityId(this.gameEntity.id));
			this.collectiblesDispensed += 1U;
			this.collectibleDispenseTime = Time.timeAsDouble;
			long num = (long)((ulong)this.collectiblesDispensed);
			long num2 = (long)((ulong)this.collectiblesCollected);
			long newState = num << 32 | num2;
			this.gameEntity.RequestState(this.gameEntity.id, newState);
		}
	}

	// Token: 0x06002619 RID: 9753 RVA: 0x000CBC5C File Offset: 0x000C9E5C
	public void OnCollectibleConsumed()
	{
		if (this.currentCollectible != null && this.currentCollectible.IsNotNull())
		{
			GRCollectible grcollectible = this.currentCollectible;
			grcollectible.OnCollected = (Action)Delegate.Remove(grcollectible.OnCollected, new Action(this.OnCollectibleConsumed));
			GameEntity entity = this.currentCollectible.entity;
			entity.OnGrabbed = (Action)Delegate.Remove(entity.OnGrabbed, new Action(this.OnCollectibleConsumed));
			this.currentCollectible = null;
		}
		this.collectiblesCollected += 1U;
		this.collectibleCollectedTime = Time.timeAsDouble;
		if (this.gameEntity.IsAuthority())
		{
			long num = (long)((ulong)this.collectiblesDispensed);
			long num2 = (long)((ulong)this.collectiblesCollected);
			long newState = num << 32 | num2;
			this.gameEntity.RequestState(this.gameEntity.id, newState);
		}
		if ((ulong)this.collectiblesCollected >= (ulong)((long)this.maxDispenseCount))
		{
			this.dispenserExhaustedEffect.Play();
			this.audioSource.PlayOneShot(this.dispenserExhaustedClip, this.dispenserExhaustedVolume);
			this.stillDispensingModel.gameObject.SetActive(false);
			this.fullyConsumedModel.gameObject.SetActive(true);
			return;
		}
		this.collectibleTakenEffect.Play();
		this.audioSource.PlayOneShot(this.collectibleTakenClip, this.collectibleTakenVolume);
	}

	// Token: 0x0600261A RID: 9754 RVA: 0x000CBDA8 File Offset: 0x000C9FA8
	public void GetSpawnedCollectible(GRCollectible collectible)
	{
		this.currentCollectible = collectible;
		collectible.OnCollected = (Action)Delegate.Combine(collectible.OnCollected, new Action(this.OnCollectibleConsumed));
		GameEntity entity = collectible.entity;
		entity.OnGrabbed = (Action)Delegate.Combine(entity.OnGrabbed, new Action(this.OnCollectibleConsumed));
	}

	// Token: 0x04003046 RID: 12358
	public GameEntity gameEntity;

	// Token: 0x04003047 RID: 12359
	public GameEntity collectiblePrefab;

	// Token: 0x04003048 RID: 12360
	public Transform spawnLocation;

	// Token: 0x04003049 RID: 12361
	public LayerMask collectibleLayerMask;

	// Token: 0x0400304A RID: 12362
	public float collectibleRespawnTimeMinutes = 1.5f;

	// Token: 0x0400304B RID: 12363
	public int maxDispenseCount = 3;

	// Token: 0x0400304C RID: 12364
	public AudioSource audioSource;

	// Token: 0x0400304D RID: 12365
	public Transform stillDispensingModel;

	// Token: 0x0400304E RID: 12366
	public Transform fullyConsumedModel;

	// Token: 0x0400304F RID: 12367
	public ParticleSystem collectibleTakenEffect;

	// Token: 0x04003050 RID: 12368
	public AudioClip collectibleTakenClip;

	// Token: 0x04003051 RID: 12369
	public float collectibleTakenVolume;

	// Token: 0x04003052 RID: 12370
	public ParticleSystem dispenserExhaustedEffect;

	// Token: 0x04003053 RID: 12371
	public AudioClip dispenserExhaustedClip;

	// Token: 0x04003054 RID: 12372
	public float dispenserExhaustedVolume;

	// Token: 0x04003055 RID: 12373
	private GRCollectible currentCollectible;

	// Token: 0x04003056 RID: 12374
	private Coroutine getSpawnedCollectibleCoroutine;

	// Token: 0x04003057 RID: 12375
	private static Collider[] overlapColliders = new Collider[10];

	// Token: 0x04003058 RID: 12376
	private uint collectiblesDispensed;

	// Token: 0x04003059 RID: 12377
	private uint collectiblesCollected;

	// Token: 0x0400305A RID: 12378
	private double collectibleDispenseRequestTime = -10000.0;

	// Token: 0x0400305B RID: 12379
	private double collectibleDispenseTime = -10000.0;

	// Token: 0x0400305C RID: 12380
	private double collectibleCollectedTime = -10000.0;
}
