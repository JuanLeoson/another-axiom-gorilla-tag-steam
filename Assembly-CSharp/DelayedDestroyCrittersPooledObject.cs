using System;
using UnityEngine;

// Token: 0x02000077 RID: 119
public class DelayedDestroyCrittersPooledObject : MonoBehaviour
{
	// Token: 0x060002EB RID: 747 RVA: 0x0001269B File Offset: 0x0001089B
	protected void OnEnable()
	{
		if (ObjectPools.instance == null || !ObjectPools.instance.initialized)
		{
			return;
		}
		this.timeToDie = Time.time + this.destroyDelay;
	}

	// Token: 0x060002EC RID: 748 RVA: 0x000126C9 File Offset: 0x000108C9
	protected void LateUpdate()
	{
		if (Time.time >= this.timeToDie)
		{
			CrittersPool.Return(base.gameObject);
		}
	}

	// Token: 0x04000399 RID: 921
	public float destroyDelay = 1f;

	// Token: 0x0400039A RID: 922
	private float timeToDie = -1f;
}
