using System;
using UnityEngine;

// Token: 0x02000671 RID: 1649
public class GRSummonerEgg : MonoBehaviour
{
	// Token: 0x06002854 RID: 10324 RVA: 0x000D9314 File Offset: 0x000D7514
	private void Awake()
	{
		this.summonedEntity = base.GetComponent<GRSummonedEntity>();
	}

	// Token: 0x06002855 RID: 10325 RVA: 0x000D9324 File Offset: 0x000D7524
	private void Start()
	{
		this.hatchTime = Random.Range(this.minHatchTime, this.maxHatchTime);
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = base.transform.position;
			component.rotation = base.transform.rotation;
			component.velocity = Vector3.up * 2f;
			component.angularVelocity = Vector3.zero;
		}
		base.Invoke("HatchEgg", this.hatchTime);
	}

	// Token: 0x06002856 RID: 10326 RVA: 0x000D93B4 File Offset: 0x000D75B4
	public void HatchEgg()
	{
		GRBreakable component = base.GetComponent<GRBreakable>();
		if (component)
		{
			component.BreakLocal();
		}
		if (this.entity.IsAuthority())
		{
			Vector3 position = this.entity.transform.position + this.spawnOffset;
			Quaternion identity = Quaternion.identity;
			GameEntityManager gameEntityManager = GhostReactorManager.Get(this.entity).gameEntityManager;
			Debug.Log(string.Format("Attempting to spawn {0} from egg at {1}", this.entityPrefabToSpawn.name, position.ToString()), this);
			gameEntityManager.RequestCreateItem(this.entityPrefabToSpawn.name.GetStaticHash(), position, identity, (long)((this.summonedEntity != null) ? this.summonedEntity.GetSummonerNetID() : 0));
		}
		base.Invoke("DestroySelf", 2f);
		this.hatchSound.Play(this.hatchAudio);
	}

	// Token: 0x06002857 RID: 10327 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Update()
	{
	}

	// Token: 0x06002858 RID: 10328 RVA: 0x000D9496 File Offset: 0x000D7696
	public void DestroySelf()
	{
		if (this.entity.IsAuthority())
		{
			this.entity.manager.RequestDestroyItem(this.entity.id);
		}
	}

	// Token: 0x040033CD RID: 13261
	public GameEntity entity;

	// Token: 0x040033CE RID: 13262
	public AudioSource hatchAudio;

	// Token: 0x040033CF RID: 13263
	public AbilitySound hatchSound;

	// Token: 0x040033D0 RID: 13264
	public GameEntity entityPrefabToSpawn;

	// Token: 0x040033D1 RID: 13265
	public Vector3 spawnOffset = new Vector3(0f, 0f, 0.3f);

	// Token: 0x040033D2 RID: 13266
	public float minHatchTime = 3f;

	// Token: 0x040033D3 RID: 13267
	public float maxHatchTime = 6f;

	// Token: 0x040033D4 RID: 13268
	private float hatchTime = 2f;

	// Token: 0x040033D5 RID: 13269
	private GRSummonedEntity summonedEntity;
}
