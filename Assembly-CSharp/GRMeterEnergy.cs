using System;
using UnityEngine;

// Token: 0x02000646 RID: 1606
public class GRMeterEnergy : MonoBehaviour
{
	// Token: 0x06002779 RID: 10105 RVA: 0x000023F5 File Offset: 0x000005F5
	public void Awake()
	{
	}

	// Token: 0x0600277A RID: 10106 RVA: 0x000D4F10 File Offset: 0x000D3110
	public void Refresh()
	{
		float num = 0f;
		if (this.tool != null && this.tool.GetEnergyMax() > 0)
		{
			num = (float)this.tool.energy / (float)this.tool.GetEnergyMax();
		}
		num = Mathf.Clamp(num, 0f, 1f);
		GRMeterEnergy.MeterType meterType = this.meterType;
		if (meterType == GRMeterEnergy.MeterType.Linear || meterType != GRMeterEnergy.MeterType.Radial)
		{
			this.meter.localScale = new Vector3(1f, num, 1f);
			return;
		}
		float value = Mathf.Lerp(this.angularRange.x, this.angularRange.y, num);
		Vector3 zero = Vector3.zero;
		zero[this.rotationAxis] = value;
		this.meter.localRotation = Quaternion.Euler(zero);
	}

	// Token: 0x040032AD RID: 12973
	public GRTool tool;

	// Token: 0x040032AE RID: 12974
	public Transform meter;

	// Token: 0x040032AF RID: 12975
	public Transform chargePoint;

	// Token: 0x040032B0 RID: 12976
	public GRMeterEnergy.MeterType meterType;

	// Token: 0x040032B1 RID: 12977
	public Vector2 angularRange = new Vector2(-45f, 45f);

	// Token: 0x040032B2 RID: 12978
	[Range(0f, 2f)]
	public int rotationAxis;

	// Token: 0x02000647 RID: 1607
	public enum MeterType
	{
		// Token: 0x040032B4 RID: 12980
		Linear,
		// Token: 0x040032B5 RID: 12981
		Radial
	}
}
