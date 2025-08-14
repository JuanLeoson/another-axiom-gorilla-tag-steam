using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000656 RID: 1622
public class GRReadyRoom : MonoBehaviour
{
	// Token: 0x060027BC RID: 10172 RVA: 0x000D63F8 File Offset: 0x000D45F8
	public void RefreshRigs(List<VRRig> vrRigs)
	{
		for (int i = 0; i < this.nameDisplayPlates.Count; i++)
		{
			if (this.nameDisplayPlates != null)
			{
				if (i < vrRigs.Count && vrRigs[i] != null && vrRigs[i].OwningNetPlayer != null)
				{
					this.nameDisplayPlates[i].RefreshPlayerName(vrRigs[i]);
				}
				else
				{
					this.nameDisplayPlates[i].Clear();
				}
			}
		}
	}

	// Token: 0x04003326 RID: 13094
	public List<GRNameDisplayPlate> nameDisplayPlates;
}
