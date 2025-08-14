using System;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000C88 RID: 3208
	public class BuilderParticleSpawner : MonoBehaviour
	{
		// Token: 0x06004F63 RID: 20323 RVA: 0x0018AD14 File Offset: 0x00188F14
		private void Start()
		{
			this.spawnTrigger.onTriggerFirstEntered += this.OnEnter;
			this.spawnTrigger.onTriggerLastExited += this.OnExit;
		}

		// Token: 0x06004F64 RID: 20324 RVA: 0x0018AD44 File Offset: 0x00188F44
		private void OnDestroy()
		{
			if (this.spawnTrigger != null)
			{
				this.spawnTrigger.onTriggerFirstEntered -= this.OnEnter;
				this.spawnTrigger.onTriggerLastExited -= this.OnExit;
			}
		}

		// Token: 0x06004F65 RID: 20325 RVA: 0x0018AD84 File Offset: 0x00188F84
		public void TrySpawning()
		{
			if (Time.time > this.lastSpawnTime + this.cooldown)
			{
				this.lastSpawnTime = Time.time;
				ObjectPools.instance.Instantiate(this.prefab, this.spawnLocation.position, this.spawnLocation.rotation, this.myPiece.GetScale(), true);
			}
		}

		// Token: 0x06004F66 RID: 20326 RVA: 0x0018ADE3 File Offset: 0x00188FE3
		private void OnEnter()
		{
			if (this.spawnOnEnter)
			{
				this.TrySpawning();
			}
		}

		// Token: 0x06004F67 RID: 20327 RVA: 0x0018ADF3 File Offset: 0x00188FF3
		private void OnExit()
		{
			if (this.spawnOnExit)
			{
				this.TrySpawning();
			}
		}

		// Token: 0x0400584E RID: 22606
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x0400584F RID: 22607
		public GameObject prefab;

		// Token: 0x04005850 RID: 22608
		public float cooldown = 0.1f;

		// Token: 0x04005851 RID: 22609
		private float lastSpawnTime;

		// Token: 0x04005852 RID: 22610
		[SerializeField]
		private BuilderSmallMonkeTrigger spawnTrigger;

		// Token: 0x04005853 RID: 22611
		[SerializeField]
		private bool spawnOnEnter = true;

		// Token: 0x04005854 RID: 22612
		[SerializeField]
		private bool spawnOnExit;

		// Token: 0x04005855 RID: 22613
		[SerializeField]
		private Transform spawnLocation;
	}
}
