using System;
using UnityEngine;

// Token: 0x02000649 RID: 1609
public class GROneTimeEntitySpawner : MonoBehaviour
{
	// Token: 0x0600277F RID: 10111 RVA: 0x000D5066 File Offset: 0x000D3266
	private void Start()
	{
		if (this.EntityPrefab == null)
		{
			Debug.Log("Can't  spawn null entity", this);
		}
		base.Invoke("TrySpawn", this.SpawnDelay);
	}

	// Token: 0x06002780 RID: 10112 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Update()
	{
	}

	// Token: 0x06002781 RID: 10113 RVA: 0x000D5094 File Offset: 0x000D3294
	private void TrySpawn()
	{
		if (!this.bHasSpawned && this.EntityPrefab != null)
		{
			Debug.Log("trying to spawn entity" + this.EntityPrefab.name, this);
			GameEntityManager gameEntityManager = this.reactor.grManager.gameEntityManager;
			if (gameEntityManager.IsAuthority())
			{
				if (!gameEntityManager.IsZoneActive())
				{
					Debug.Log("delaying spawn attempt because zone not active", this);
					base.Invoke("TrySpawn", 0.2f);
					return;
				}
				Debug.Log("trying to spawn entity", this);
				gameEntityManager.RequestCreateItem(this.EntityPrefab.name.GetStaticHash(), base.transform.position + new Vector3(0f, 0f, 0f), base.transform.rotation, 0L);
				this.bHasSpawned = true;
			}
		}
	}

	// Token: 0x040032B7 RID: 12983
	public GhostReactor reactor;

	// Token: 0x040032B8 RID: 12984
	public GameEntity EntityPrefab;

	// Token: 0x040032B9 RID: 12985
	private bool bHasSpawned;

	// Token: 0x040032BA RID: 12986
	private float SpawnDelay = 3f;
}
