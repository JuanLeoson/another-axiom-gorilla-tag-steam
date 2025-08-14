using System;
using GorillaExtensions;
using UnityEngine;

namespace Critters.Scripts
{
	// Token: 0x02000F8B RID: 3979
	public class CrittersActorSpawner : MonoBehaviour
	{
		// Token: 0x06006386 RID: 25478 RVA: 0x001F5B70 File Offset: 0x001F3D70
		private void Awake()
		{
			this.spawnPoint.OnSpawnChanged += this.HandleSpawnedActor;
		}

		// Token: 0x06006387 RID: 25479 RVA: 0x001F5B89 File Offset: 0x001F3D89
		private void OnEnable()
		{
			if (!CrittersManager.instance.actorSpawners.Contains(this))
			{
				CrittersManager.instance.actorSpawners.Add(this);
			}
		}

		// Token: 0x06006388 RID: 25480 RVA: 0x001F5BB1 File Offset: 0x001F3DB1
		private void OnDisable()
		{
			if (CrittersManager.instance.actorSpawners.Contains(this))
			{
				CrittersManager.instance.actorSpawners.Remove(this);
			}
		}

		// Token: 0x06006389 RID: 25481 RVA: 0x001F5BDC File Offset: 0x001F3DDC
		public void ProcessLocal()
		{
			if (!CrittersManager.instance.LocalAuthority())
			{
				return;
			}
			if (this.nextSpawnTime <= (double)Time.time)
			{
				this.nextSpawnTime = (double)(Time.time + (float)this.spawnDelay);
				if (this.currentSpawnedObject == null || !this.currentSpawnedObject.isEnabled)
				{
					this.SpawnActor();
				}
			}
			if (this.currentSpawnedObject.IsNotNull())
			{
				if (!this.currentSpawnedObject.isEnabled)
				{
					this.currentSpawnedObject = null;
					this.spawnPoint.SetSpawnedActor(null);
					return;
				}
				if (!this.insideSpawnerCheck.bounds.Contains(this.currentSpawnedObject.transform.position))
				{
					this.currentSpawnedObject.RemoveDespawnBlock();
					this.currentSpawnedObject = null;
					this.spawnPoint.SetSpawnedActor(null);
					return;
				}
				if (!this.VerifySpawnAttached())
				{
					this.currentSpawnedObject.RemoveDespawnBlock();
					this.currentSpawnedObject = null;
					this.spawnPoint.SetSpawnedActor(null);
				}
			}
		}

		// Token: 0x0600638A RID: 25482 RVA: 0x001F5CD6 File Offset: 0x001F3ED6
		public void DoReset()
		{
			this.currentSpawnedObject = null;
		}

		// Token: 0x0600638B RID: 25483 RVA: 0x001F5CDF File Offset: 0x001F3EDF
		private void HandleSpawnedActor(CrittersActor spawnedActor)
		{
			this.currentSpawnedObject = spawnedActor;
		}

		// Token: 0x0600638C RID: 25484 RVA: 0x001F5CE8 File Offset: 0x001F3EE8
		private void SpawnActor()
		{
			CrittersActor crittersActor = CrittersManager.instance.SpawnActor(this.actorType, this.subActorIndex);
			this.spawnPoint.SetSpawnedActor(crittersActor);
			if (crittersActor.IsNull())
			{
				return;
			}
			if (this.attachSpawnedObjectToSpawnLocation)
			{
				crittersActor.GrabbedBy(this.spawnPoint, true, default(Quaternion), default(Vector3), false);
				return;
			}
			crittersActor.MoveActor(this.spawnPoint.transform.position, this.spawnPoint.transform.rotation, false, true, true);
			crittersActor.rb.velocity = Vector3.zero;
			if (this.applyImpulseOnSpawn)
			{
				crittersActor.SetImpulse();
			}
		}

		// Token: 0x0600638D RID: 25485 RVA: 0x001F5D94 File Offset: 0x001F3F94
		private bool VerifySpawnAttached()
		{
			if (this.attachSpawnedObjectToSpawnLocation)
			{
				CrittersActor crittersActor;
				CrittersManager.instance.actorById.TryGetValue(this.currentSpawnedObject.parentActorId, out crittersActor);
				if (crittersActor.IsNull() || crittersActor != this.spawnPoint)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x04006E7B RID: 28283
		public CrittersActorSpawnerPoint spawnPoint;

		// Token: 0x04006E7C RID: 28284
		public CrittersActor currentSpawnedObject;

		// Token: 0x04006E7D RID: 28285
		public CrittersActor.CrittersActorType actorType;

		// Token: 0x04006E7E RID: 28286
		public int subActorIndex = -1;

		// Token: 0x04006E7F RID: 28287
		public Collider insideSpawnerCheck;

		// Token: 0x04006E80 RID: 28288
		public int spawnDelay = 5;

		// Token: 0x04006E81 RID: 28289
		public bool applyImpulseOnSpawn = true;

		// Token: 0x04006E82 RID: 28290
		public bool attachSpawnedObjectToSpawnLocation;

		// Token: 0x04006E83 RID: 28291
		private double nextSpawnTime;
	}
}
