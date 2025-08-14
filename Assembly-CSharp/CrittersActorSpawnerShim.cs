using System;
using Critters.Scripts;
using UnityEngine;

// Token: 0x02000044 RID: 68
public class CrittersActorSpawnerShim : MonoBehaviour
{
	// Token: 0x06000156 RID: 342 RVA: 0x00009110 File Offset: 0x00007310
	[ContextMenu("Copy Spawner Data To Shim")]
	private CrittersActorSpawner CopySpawnerDataInPrefab()
	{
		CrittersActorSpawner component = base.gameObject.GetComponent<CrittersActorSpawner>();
		this.spawnerPointTransform = component.spawnPoint.transform;
		this.actorType = component.actorType;
		this.subActorIndex = component.subActorIndex;
		this.insideSpawnerBounds = (BoxCollider)component.insideSpawnerCheck;
		this.spawnDelay = component.spawnDelay;
		this.applyImpulseOnSpawn = component.applyImpulseOnSpawn;
		this.attachSpawnedObjectToSpawnLocation = component.attachSpawnedObjectToSpawnLocation;
		this.colliderTrigger = base.gameObject.GetComponent<BoxCollider>();
		return component;
	}

	// Token: 0x06000157 RID: 343 RVA: 0x0000919C File Offset: 0x0000739C
	[ContextMenu("Replace Spawner With Shim")]
	private void ReplaceSpawnerWithShim()
	{
		CrittersActorSpawner crittersActorSpawner = this.CopySpawnerDataInPrefab();
		if (crittersActorSpawner.spawnPoint.GetComponent<Rigidbody>() != null)
		{
			Object.DestroyImmediate(crittersActorSpawner.spawnPoint.GetComponent<Rigidbody>());
		}
		Object.DestroyImmediate(crittersActorSpawner.spawnPoint);
		Object.DestroyImmediate(crittersActorSpawner);
	}

	// Token: 0x04000178 RID: 376
	public Transform spawnerPointTransform;

	// Token: 0x04000179 RID: 377
	public CrittersActor.CrittersActorType actorType;

	// Token: 0x0400017A RID: 378
	public int subActorIndex;

	// Token: 0x0400017B RID: 379
	public BoxCollider insideSpawnerBounds;

	// Token: 0x0400017C RID: 380
	public int spawnDelay;

	// Token: 0x0400017D RID: 381
	public bool applyImpulseOnSpawn;

	// Token: 0x0400017E RID: 382
	public bool attachSpawnedObjectToSpawnLocation;

	// Token: 0x0400017F RID: 383
	public BoxCollider colliderTrigger;
}
