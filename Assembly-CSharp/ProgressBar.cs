using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007A2 RID: 1954
public class ProgressBar : MonoBehaviour
{
	// Token: 0x06003123 RID: 12579 RVA: 0x00100390 File Offset: 0x000FE590
	public void UpdateProgress(float newFill)
	{
		bool flag = newFill > 1f;
		this._fillAmount = Mathf.Clamp(newFill, 0f, 1f);
		this.fillImage.fillAmount = this._fillAmount;
		if (this.useColors)
		{
			if (flag)
			{
				this.fillImage.color = this.overCapacity;
				return;
			}
			if (Mathf.Approximately(this._fillAmount, 1f))
			{
				this.fillImage.color = this.atCapacity;
				return;
			}
			this.fillImage.color = this.underCapacity;
		}
	}

	// Token: 0x04003CC2 RID: 15554
	[SerializeField]
	private Image fillImage;

	// Token: 0x04003CC3 RID: 15555
	[SerializeField]
	private bool useColors;

	// Token: 0x04003CC4 RID: 15556
	[SerializeField]
	private Color underCapacity = Color.green;

	// Token: 0x04003CC5 RID: 15557
	[SerializeField]
	private Color overCapacity = Color.red;

	// Token: 0x04003CC6 RID: 15558
	[SerializeField]
	private Color atCapacity = Color.yellow;

	// Token: 0x04003CC7 RID: 15559
	private float _fillAmount;
}
