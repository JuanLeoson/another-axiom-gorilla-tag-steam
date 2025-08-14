using System;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTag.Reactions
{
	// Token: 0x02000EAC RID: 3756
	public class SpawnWorldEffects : MonoBehaviour
	{
		// Token: 0x06005DDF RID: 24031 RVA: 0x001D9F00 File Offset: 0x001D8100
		protected void OnEnable()
		{
			if (GorillaComputer.instance == null)
			{
				Debug.LogError("SpawnWorldEffects: Disabling because GorillaComputer not found! Hierarchy path: " + base.transform.GetPath(), this);
				base.enabled = false;
				return;
			}
			if (this._prefabToSpawn != null && !this._isPrefabInPool)
			{
				if (this._prefabToSpawn.CompareTag("Untagged"))
				{
					Debug.LogError("SpawnWorldEffects: Disabling because Spawn Prefab has no tag! Hierarchy path: " + base.transform.GetPath(), this);
					base.enabled = false;
					return;
				}
				this._isPrefabInPool = ObjectPools.instance.DoesPoolExist(this._prefabToSpawn);
				if (!this._isPrefabInPool)
				{
					Debug.LogError("SpawnWorldEffects: Disabling because Spawn Prefab not in pool! Hierarchy path: " + base.transform.GetPath(), this);
					base.enabled = false;
					return;
				}
				this._pool = ObjectPools.instance.GetPoolByObjectType(this._prefabToSpawn);
			}
			this._hasPrefabToSpawn = (this._prefabToSpawn != null && this._isPrefabInPool);
		}

		// Token: 0x06005DE0 RID: 24032 RVA: 0x001DA004 File Offset: 0x001D8204
		public void RequestSpawn(Vector3 worldPosition)
		{
			this.RequestSpawn(worldPosition, Vector3.up);
		}

		// Token: 0x06005DE1 RID: 24033 RVA: 0x001DA014 File Offset: 0x001D8214
		public void RequestSpawn(Vector3 worldPosition, Vector3 normal)
		{
			if (this._maxParticleHitReactionRate < 1E-05f || !FireManager.hasInstance)
			{
				return;
			}
			double num = GTTime.TimeAsDouble();
			if ((float)(num - this._lastCollisionTime) < 1f / this._maxParticleHitReactionRate)
			{
				return;
			}
			if (this._hasPrefabToSpawn && this._isPrefabInPool && this._pool.GetInactiveCount() > 0)
			{
				FireManager.SpawnFire(this._pool, worldPosition, normal, base.transform.lossyScale.x);
			}
			this._lastCollisionTime = num;
		}

		// Token: 0x040067D7 RID: 26583
		[Tooltip("The defaults are numbers for the flamethrower hair dryer.")]
		private readonly float _maxParticleHitReactionRate = 2f;

		// Token: 0x040067D8 RID: 26584
		[Tooltip("Must be in the global object pool and have a tag.")]
		[SerializeField]
		private GameObject _prefabToSpawn;

		// Token: 0x040067D9 RID: 26585
		private bool _hasPrefabToSpawn;

		// Token: 0x040067DA RID: 26586
		private bool _isPrefabInPool;

		// Token: 0x040067DB RID: 26587
		private double _lastCollisionTime;

		// Token: 0x040067DC RID: 26588
		private SinglePool _pool;
	}
}
