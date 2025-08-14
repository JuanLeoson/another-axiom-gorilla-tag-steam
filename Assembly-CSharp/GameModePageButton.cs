using System;
using UnityEngine;

// Token: 0x020004CD RID: 1229
public class GameModePageButton : GorillaPressableButton
{
	// Token: 0x06001E37 RID: 7735 RVA: 0x000A0D64 File Offset: 0x0009EF64
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.selector.ChangePage(this.left);
	}

	// Token: 0x040026BB RID: 9915
	[SerializeField]
	private GameModePages selector;

	// Token: 0x040026BC RID: 9916
	[SerializeField]
	private bool left;
}
