using System;
using GorillaTag;
using UnityEngine;

// Token: 0x0200026A RID: 618
public class ZoneRootRegister : MonoBehaviour
{
	// Token: 0x06000E4F RID: 3663 RVA: 0x000578CE File Offset: 0x00055ACE
	private void Awake()
	{
		this.watchableSlot.Value = base.gameObject;
	}

	// Token: 0x06000E50 RID: 3664 RVA: 0x000578E1 File Offset: 0x00055AE1
	private void OnDestroy()
	{
		this.watchableSlot.Value = null;
	}

	// Token: 0x0400172E RID: 5934
	[SerializeField]
	private WatchableGameObjectSO watchableSlot;
}
