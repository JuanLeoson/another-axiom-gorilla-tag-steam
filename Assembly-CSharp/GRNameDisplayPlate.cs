using System;
using TMPro;
using UnityEngine;

// Token: 0x02000648 RID: 1608
public class GRNameDisplayPlate : MonoBehaviour
{
	// Token: 0x0600277C RID: 10108 RVA: 0x000D4FF4 File Offset: 0x000D31F4
	public void RefreshPlayerName(VRRig vrRig)
	{
		GRPlayer x = GRPlayer.Get(vrRig);
		if (vrRig != null && x != null)
		{
			if (!this.namePlateLabel.text.Equals(vrRig.playerNameVisible))
			{
				this.namePlateLabel.text = vrRig.playerNameVisible;
				return;
			}
		}
		else
		{
			this.namePlateLabel.text = "";
		}
	}

	// Token: 0x0600277D RID: 10109 RVA: 0x000D5054 File Offset: 0x000D3254
	public void Clear()
	{
		this.namePlateLabel.text = "";
	}

	// Token: 0x040032B6 RID: 12982
	public TMP_Text namePlateLabel;
}
