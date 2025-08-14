using System;
using GorillaTag;
using GorillaTag.Reactions;
using UnityEngine;

// Token: 0x0200022C RID: 556
public class BasicFireSpawner : MonoBehaviour
{
	// Token: 0x06000D21 RID: 3361 RVA: 0x0004617D File Offset: 0x0004437D
	private void Awake()
	{
		this.scale = this.fireScaleMinMax.y;
	}

	// Token: 0x06000D22 RID: 3362 RVA: 0x00046190 File Offset: 0x00044390
	public void InterpolateScale(float f)
	{
		this.scale = Mathf.Lerp(this.fireScaleMinMax.x, this.fireScaleMinMax.y, f);
	}

	// Token: 0x06000D23 RID: 3363 RVA: 0x000461B4 File Offset: 0x000443B4
	public void Spawn()
	{
		if (this.firePool == null)
		{
			this.firePool = ObjectPools.instance.GetPoolByHash(this.firePrefab);
		}
		FireManager.SpawnFire(this.firePool, base.transform.position, Vector3.up, this.scale);
	}

	// Token: 0x04001007 RID: 4103
	[SerializeField]
	private HashWrapper firePrefab;

	// Token: 0x04001008 RID: 4104
	[SerializeField]
	private Vector2 fireScaleMinMax = Vector2.one;

	// Token: 0x04001009 RID: 4105
	private SinglePool firePool;

	// Token: 0x0400100A RID: 4106
	private float scale;
}
