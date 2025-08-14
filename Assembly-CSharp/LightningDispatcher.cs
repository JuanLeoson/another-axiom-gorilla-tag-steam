using System;
using UnityEngine;

// Token: 0x02000B22 RID: 2850
public class LightningDispatcher : MonoBehaviour
{
	// Token: 0x1400007C RID: 124
	// (add) Token: 0x060044A2 RID: 17570 RVA: 0x00156D24 File Offset: 0x00154F24
	// (remove) Token: 0x060044A3 RID: 17571 RVA: 0x00156D58 File Offset: 0x00154F58
	public static event LightningDispatcher.DispatchLightningEvent RequestLightningStrike;

	// Token: 0x060044A4 RID: 17572 RVA: 0x00156D8C File Offset: 0x00154F8C
	public void DispatchLightning(Vector3 p1, Vector3 p2)
	{
		if (LightningDispatcher.RequestLightningStrike != null)
		{
			LightningStrike lightningStrike = LightningDispatcher.RequestLightningStrike(p1, p2);
			float num = Mathf.Max(new float[]
			{
				base.transform.lossyScale.x,
				base.transform.lossyScale.y,
				base.transform.lossyScale.z
			});
			lightningStrike.Play(p1, p2, this.beamWidthCM * 0.01f * num, this.soundVolumeMultiplier / num, LightningStrike.rand.NextFloat(this.minDuration, this.maxDuration), this.colorOverLifetime);
		}
	}

	// Token: 0x04004EC5 RID: 20165
	[SerializeField]
	private float beamWidthCM = 1f;

	// Token: 0x04004EC6 RID: 20166
	[SerializeField]
	private float soundVolumeMultiplier = 1f;

	// Token: 0x04004EC7 RID: 20167
	[SerializeField]
	private float minDuration = 0.05f;

	// Token: 0x04004EC8 RID: 20168
	[SerializeField]
	private float maxDuration = 0.12f;

	// Token: 0x04004EC9 RID: 20169
	[SerializeField]
	private Gradient colorOverLifetime;

	// Token: 0x02000B23 RID: 2851
	// (Invoke) Token: 0x060044A7 RID: 17575
	public delegate LightningStrike DispatchLightningEvent(Vector3 p1, Vector3 p2);
}
