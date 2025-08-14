using System;
using GorillaTag.Reactions;
using UnityEngine;

// Token: 0x02000B0C RID: 2828
[RequireComponent(typeof(SpawnWorldEffects))]
public class SpawnWorldEffectsTrigger : MonoBehaviour
{
	// Token: 0x06004414 RID: 17428 RVA: 0x001558CA File Offset: 0x00153ACA
	private void OnEnable()
	{
		if (this.swe == null)
		{
			this.swe = base.GetComponent<SpawnWorldEffects>();
		}
	}

	// Token: 0x06004415 RID: 17429 RVA: 0x001558E6 File Offset: 0x00153AE6
	private void OnTriggerEnter(Collider other)
	{
		this.spawnTime = Time.time;
		this.swe.RequestSpawn(base.transform.position);
	}

	// Token: 0x06004416 RID: 17430 RVA: 0x00155909 File Offset: 0x00153B09
	private void OnTriggerStay(Collider other)
	{
		if (Time.time - this.spawnTime < this.spawnCooldown)
		{
			return;
		}
		this.swe.RequestSpawn(base.transform.position);
		this.spawnTime = Time.time;
	}

	// Token: 0x04004E67 RID: 20071
	private SpawnWorldEffects swe;

	// Token: 0x04004E68 RID: 20072
	private float spawnTime;

	// Token: 0x04004E69 RID: 20073
	[SerializeField]
	private float spawnCooldown = 1f;
}
