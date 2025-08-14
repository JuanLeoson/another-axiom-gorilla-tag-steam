using System;
using UnityEngine;

// Token: 0x0200021E RID: 542
public class PositionVolumeModifier : MonoBehaviour
{
	// Token: 0x06000CB7 RID: 3255 RVA: 0x000445E1 File Offset: 0x000427E1
	public void OnTriggerStay(Collider other)
	{
		this.audioToMod.isModified = true;
	}

	// Token: 0x04000FA8 RID: 4008
	public TimeOfDayDependentAudio audioToMod;
}
