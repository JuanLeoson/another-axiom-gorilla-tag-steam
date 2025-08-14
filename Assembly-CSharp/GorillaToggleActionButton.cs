using System;
using UnityEngine.Events;

// Token: 0x020007F0 RID: 2032
public class GorillaToggleActionButton : GorillaPressableButton
{
	// Token: 0x060032DE RID: 13022 RVA: 0x00108DD3 File Offset: 0x00106FD3
	public override void Start()
	{
		this.BindToggleAction();
	}

	// Token: 0x060032DF RID: 13023 RVA: 0x00108DDC File Offset: 0x00106FDC
	private void BindToggleAction()
	{
		if (this.ToggleAction == null || !this.ToggleAction.IsValid)
		{
			return;
		}
		this.ToggleAction.Cache();
		this.onPressButton = new UnityEvent();
		this.onPressButton.AddListener(new UnityAction(this.ExecuteToggleAction));
	}

	// Token: 0x060032E0 RID: 13024 RVA: 0x00108E2C File Offset: 0x0010702C
	private void ExecuteToggleAction()
	{
		ComponentFunctionReference<bool> toggleAction = this.ToggleAction;
		this.isOn = (toggleAction != null && toggleAction.Invoke());
		this.UpdateColor();
	}

	// Token: 0x04003FCD RID: 16333
	public ComponentFunctionReference<bool> ToggleAction;

	// Token: 0x04003FCE RID: 16334
	private Func<bool> toggleFunc;
}
