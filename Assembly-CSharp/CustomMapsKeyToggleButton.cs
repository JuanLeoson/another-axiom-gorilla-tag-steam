using System;
using GorillaTagScripts.UI;

// Token: 0x02000842 RID: 2114
public class CustomMapsKeyToggleButton : CustomMapsKeyButton
{
	// Token: 0x060034ED RID: 13549 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void PressButtonColourUpdate()
	{
	}

	// Token: 0x060034EE RID: 13550 RVA: 0x001146D8 File Offset: 0x001128D8
	public void SetButtonStatus(bool newIsPressed)
	{
		if (this.isPressed == newIsPressed)
		{
			return;
		}
		this.isPressed = newIsPressed;
		this.propBlock.SetColor("_BaseColor", this.isPressed ? this.ButtonColorSettings.PressedColor : this.ButtonColorSettings.UnpressedColor);
		this.propBlock.SetColor("_Color", this.isPressed ? this.ButtonColorSettings.PressedColor : this.ButtonColorSettings.UnpressedColor);
		this.ButtonRenderer.SetPropertyBlock(this.propBlock);
	}

	// Token: 0x040041D6 RID: 16854
	private bool isPressed;
}
