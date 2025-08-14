using System;
using GorillaTag;
using UnityEngine;

// Token: 0x02000278 RID: 632
[GTStripGameObjectFromBuild("!GT_AUTOMATED_PERF_TEST")]
public class PerfTestGorillaSlot : MonoBehaviour
{
	// Token: 0x06000E86 RID: 3718 RVA: 0x000583B7 File Offset: 0x000565B7
	private void Start()
	{
		this.localStartPosition = base.transform.localPosition;
	}

	// Token: 0x0400176B RID: 5995
	public PerfTestGorillaSlot.SlotType slotType;

	// Token: 0x0400176C RID: 5996
	public Vector3 localStartPosition;

	// Token: 0x02000279 RID: 633
	public enum SlotType
	{
		// Token: 0x0400176E RID: 5998
		VR_PLAYER,
		// Token: 0x0400176F RID: 5999
		DUMMY
	}
}
