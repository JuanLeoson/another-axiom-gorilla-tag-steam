using System;
using UnityEngine;

// Token: 0x02000264 RID: 612
public class LowEffortZone : GorillaTriggerBox
{
	// Token: 0x06000E31 RID: 3633 RVA: 0x0005707C File Offset: 0x0005527C
	private void Awake()
	{
		if (this.triggerOnAwake)
		{
			this.OnBoxTriggered();
		}
	}

	// Token: 0x06000E32 RID: 3634 RVA: 0x0005708C File Offset: 0x0005528C
	public override void OnBoxTriggered()
	{
		for (int i = 0; i < this.objectsToEnable.Length; i++)
		{
			if (this.objectsToEnable[i] != null)
			{
				this.objectsToEnable[i].SetActive(true);
			}
		}
		for (int j = 0; j < this.objectsToDisable.Length; j++)
		{
			if (this.objectsToDisable[j] != null)
			{
				this.objectsToDisable[j].SetActive(false);
			}
		}
	}

	// Token: 0x040016EE RID: 5870
	public GameObject[] objectsToEnable;

	// Token: 0x040016EF RID: 5871
	public GameObject[] objectsToDisable;

	// Token: 0x040016F0 RID: 5872
	public bool triggerOnAwake;
}
