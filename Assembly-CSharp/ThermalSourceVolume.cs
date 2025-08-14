using System;
using UnityEngine;

// Token: 0x02000253 RID: 595
public class ThermalSourceVolume : MonoBehaviour
{
	// Token: 0x06000DE6 RID: 3558 RVA: 0x00054B93 File Offset: 0x00052D93
	protected void OnEnable()
	{
		ThermalManager.Register(this);
	}

	// Token: 0x06000DE7 RID: 3559 RVA: 0x00054B9B File Offset: 0x00052D9B
	protected void OnDisable()
	{
		ThermalManager.Unregister(this);
	}

	// Token: 0x040015AD RID: 5549
	[Tooltip("Temperature in celsius. Default is 20 which is room temperature.")]
	public float celsius = 20f;

	// Token: 0x040015AE RID: 5550
	public float innerRadius = 0.1f;

	// Token: 0x040015AF RID: 5551
	public float outerRadius = 1f;
}
