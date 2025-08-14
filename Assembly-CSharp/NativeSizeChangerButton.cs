using System;
using UnityEngine;

// Token: 0x020002AC RID: 684
public class NativeSizeChangerButton : GorillaPressableButton
{
	// Token: 0x06000FD7 RID: 4055 RVA: 0x0005C240 File Offset: 0x0005A440
	public override void ButtonActivation()
	{
		this.nativeSizeChanger.Activate(this.settings);
	}

	// Token: 0x04001861 RID: 6241
	[SerializeField]
	private NativeSizeChanger nativeSizeChanger;

	// Token: 0x04001862 RID: 6242
	[SerializeField]
	private NativeSizeChangerSettings settings;
}
