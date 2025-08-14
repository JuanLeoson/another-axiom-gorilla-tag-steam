using System;

// Token: 0x02000616 RID: 1558
public class GRDebugGodmodeButton : GorillaPressableReleaseButton
{
	// Token: 0x06002627 RID: 9767 RVA: 0x00020127 File Offset: 0x0001E327
	private void Awake()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06002628 RID: 9768 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnPressedButton()
	{
	}

	// Token: 0x06002629 RID: 9769 RVA: 0x000CC00D File Offset: 0x000CA20D
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.UpdateColor();
	}

	// Token: 0x0600262A RID: 9770 RVA: 0x000CC01B File Offset: 0x000CA21B
	public override void ButtonDeactivation()
	{
		base.ButtonDeactivation();
		this.UpdateColor();
	}
}
