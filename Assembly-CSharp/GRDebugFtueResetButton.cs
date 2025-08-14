using System;
using UnityEngine;

// Token: 0x02000615 RID: 1557
public class GRDebugFtueResetButton : GorillaPressableReleaseButton
{
	// Token: 0x06002622 RID: 9762 RVA: 0x000CBFAF File Offset: 0x000CA1AF
	private void Awake()
	{
		if (!this.availableOnLive)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002623 RID: 9763 RVA: 0x000CBFC5 File Offset: 0x000CA1C5
	public void OnPressedButton()
	{
		PlayerPrefs.SetString("spawnInWrongStump", "flagged");
		PlayerPrefs.Save();
	}

	// Token: 0x06002624 RID: 9764 RVA: 0x000CBFDB File Offset: 0x000CA1DB
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.isOn = true;
		this.UpdateColor();
	}

	// Token: 0x06002625 RID: 9765 RVA: 0x000CBFF0 File Offset: 0x000CA1F0
	public override void ButtonDeactivation()
	{
		base.ButtonDeactivation();
		this.isOn = false;
		this.UpdateColor();
	}

	// Token: 0x04003066 RID: 12390
	public bool availableOnLive;
}
