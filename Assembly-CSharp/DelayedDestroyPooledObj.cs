using System;
using UnityEngine;

// Token: 0x02000AAF RID: 2735
public class DelayedDestroyPooledObj : MonoBehaviour
{
	// Token: 0x06004234 RID: 16948 RVA: 0x0014D877 File Offset: 0x0014BA77
	protected void OnEnable()
	{
		if (ObjectPools.instance == null || !ObjectPools.instance.initialized)
		{
			return;
		}
		this.timeToDie = Time.time + this.destroyDelay;
	}

	// Token: 0x06004235 RID: 16949 RVA: 0x0014D8A5 File Offset: 0x0014BAA5
	protected void LateUpdate()
	{
		if (Time.time > this.timeToDie)
		{
			ObjectPools.instance.Destroy(base.gameObject);
		}
	}

	// Token: 0x04004D83 RID: 19843
	[Tooltip("Return to the object pool after this many seconds.")]
	public float destroyDelay;

	// Token: 0x04004D84 RID: 19844
	private float timeToDie = -1f;
}
