using System;
using UnityEngine;

// Token: 0x020000E6 RID: 230
public class SpawnOnEnter : MonoBehaviour
{
	// Token: 0x060005B9 RID: 1465 RVA: 0x00021390 File Offset: 0x0001F590
	public void OnTriggerEnter(Collider other)
	{
		if (Time.time > this.lastSpawnTime + this.cooldown)
		{
			this.lastSpawnTime = Time.time;
			ObjectPools.instance.Instantiate(this.prefab, other.transform.position, true);
		}
	}

	// Token: 0x040006DE RID: 1758
	public GameObject prefab;

	// Token: 0x040006DF RID: 1759
	public float cooldown = 0.1f;

	// Token: 0x040006E0 RID: 1760
	private float lastSpawnTime;
}
