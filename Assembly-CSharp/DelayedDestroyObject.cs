using System;
using UnityEngine;

// Token: 0x02000078 RID: 120
public class DelayedDestroyObject : MonoBehaviour
{
	// Token: 0x060002EE RID: 750 RVA: 0x00012701 File Offset: 0x00010901
	private void Start()
	{
		this._timeToDie = Time.time + this.lifetime;
	}

	// Token: 0x060002EF RID: 751 RVA: 0x00012715 File Offset: 0x00010915
	private void LateUpdate()
	{
		if (Time.time >= this._timeToDie)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x0400039B RID: 923
	public float lifetime = 10f;

	// Token: 0x0400039C RID: 924
	private float _timeToDie;
}
