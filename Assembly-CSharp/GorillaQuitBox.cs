using System;
using UnityEngine;

// Token: 0x020004D9 RID: 1241
public class GorillaQuitBox : GorillaTriggerBox
{
	// Token: 0x06001E63 RID: 7779 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Start()
	{
	}

	// Token: 0x06001E64 RID: 7780 RVA: 0x000A1481 File Offset: 0x0009F681
	public override void OnBoxTriggered()
	{
		Application.Quit();
	}
}
