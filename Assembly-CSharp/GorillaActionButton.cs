using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001F8 RID: 504
public class GorillaActionButton : GorillaPressableButton
{
	// Token: 0x06000BE5 RID: 3045 RVA: 0x00040D4F File Offset: 0x0003EF4F
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.onPress.Invoke();
	}

	// Token: 0x04000ED9 RID: 3801
	[SerializeField]
	public UnityEvent onPress;
}
