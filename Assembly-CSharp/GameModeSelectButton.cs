using System;
using UnityEngine;

// Token: 0x020004CF RID: 1231
public class GameModeSelectButton : GorillaPressableButton
{
	// Token: 0x06001E46 RID: 7750 RVA: 0x000A103B File Offset: 0x0009F23B
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.selector.SelectEntryOnPage(this.buttonIndex);
	}

	// Token: 0x040026C4 RID: 9924
	[SerializeField]
	internal GameModePages selector;

	// Token: 0x040026C5 RID: 9925
	[SerializeField]
	internal int buttonIndex;
}
