using System;
using GorillaNetworking.Store;

// Token: 0x0200047F RID: 1151
public class TryOnBundleButton : GorillaPressableButton
{
	// Token: 0x06001C8D RID: 7309 RVA: 0x000998BB File Offset: 0x00097ABB
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivationWithHand(isLeftHand);
		BundleManager.instance.PressTryOnBundleButton(this, isLeftHand);
	}

	// Token: 0x06001C8E RID: 7310 RVA: 0x000998D4 File Offset: 0x00097AD4
	public override void UpdateColor()
	{
		if (this.playfabBundleID == "NULL")
		{
			this.buttonRenderer.material = this.unpressedMaterial;
			if (this.myText != null)
			{
				this.myText.text = "";
			}
			return;
		}
		base.UpdateColor();
	}

	// Token: 0x040024FF RID: 9471
	public int buttonIndex;

	// Token: 0x04002500 RID: 9472
	public string playfabBundleID = "NULL";
}
