using System;
using TMPro;
using UnityEngine;

// Token: 0x02000080 RID: 128
public class MenagerieSlot : MonoBehaviour
{
	// Token: 0x0600033F RID: 831 RVA: 0x00013A4A File Offset: 0x00011C4A
	private void Reset()
	{
		this.critterMountPoint = base.transform;
	}

	// Token: 0x040003CC RID: 972
	public Transform critterMountPoint;

	// Token: 0x040003CD RID: 973
	public TMP_Text label;

	// Token: 0x040003CE RID: 974
	public MenagerieCritter critter;
}
