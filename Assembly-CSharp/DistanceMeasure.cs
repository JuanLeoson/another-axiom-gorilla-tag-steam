using System;
using UnityEngine;

// Token: 0x02000864 RID: 2148
public class DistanceMeasure : MonoBehaviour
{
	// Token: 0x060035FD RID: 13821 RVA: 0x0011BD4F File Offset: 0x00119F4F
	private void Awake()
	{
		if (this.from == null)
		{
			this.from = base.transform;
		}
		if (this.to == null)
		{
			this.to = base.transform;
		}
	}

	// Token: 0x040042E5 RID: 17125
	public Transform from;

	// Token: 0x040042E6 RID: 17126
	public Transform to;
}
