using System;
using UnityEngine;

// Token: 0x02000619 RID: 1561
public class GRDistilleryDeposit : MonoBehaviour
{
	// Token: 0x0600263D RID: 9789 RVA: 0x000CC7F0 File Offset: 0x000CA9F0
	private void Start()
	{
		this._distillery = base.GetComponentInParent<GRDistillery>();
	}

	// Token: 0x0600263E RID: 9790 RVA: 0x000023F5 File Offset: 0x000005F5
	private void OnTriggerEnter(Collider other)
	{
	}

	// Token: 0x04003089 RID: 12425
	public float hapticStrength;

	// Token: 0x0400308A RID: 12426
	public float hapticDuration;

	// Token: 0x0400308B RID: 12427
	private GRDistillery _distillery;
}
