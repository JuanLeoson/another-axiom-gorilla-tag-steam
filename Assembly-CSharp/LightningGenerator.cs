using System;
using UnityEngine;

// Token: 0x02000B25 RID: 2853
public class LightningGenerator : MonoBehaviour
{
	// Token: 0x060044AB RID: 17579 RVA: 0x00156E6C File Offset: 0x0015506C
	private void Awake()
	{
		this.strikes = new LightningStrike[this.maxConcurrentStrikes];
		for (int i = 0; i < this.strikes.Length; i++)
		{
			if (i == 0)
			{
				this.strikes[i] = this.prototype;
			}
			else
			{
				this.strikes[i] = Object.Instantiate<LightningStrike>(this.prototype, base.transform);
			}
			this.strikes[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x060044AC RID: 17580 RVA: 0x00156EDC File Offset: 0x001550DC
	private void OnEnable()
	{
		LightningDispatcher.RequestLightningStrike += this.LightningDispatcher_RequestLightningStrike;
	}

	// Token: 0x060044AD RID: 17581 RVA: 0x00156EEF File Offset: 0x001550EF
	private void OnDisable()
	{
		LightningDispatcher.RequestLightningStrike -= this.LightningDispatcher_RequestLightningStrike;
	}

	// Token: 0x060044AE RID: 17582 RVA: 0x00156F02 File Offset: 0x00155102
	private LightningStrike LightningDispatcher_RequestLightningStrike(Vector3 t1, Vector3 t2)
	{
		this.index = (this.index + 1) % this.strikes.Length;
		return this.strikes[this.index];
	}

	// Token: 0x04004ECB RID: 20171
	[SerializeField]
	private uint maxConcurrentStrikes = 10U;

	// Token: 0x04004ECC RID: 20172
	[SerializeField]
	private LightningStrike prototype;

	// Token: 0x04004ECD RID: 20173
	private LightningStrike[] strikes;

	// Token: 0x04004ECE RID: 20174
	private int index;
}
