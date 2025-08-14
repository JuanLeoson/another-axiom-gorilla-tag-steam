using System;
using UnityEngine.UI;

// Token: 0x0200052C RID: 1324
public class BuilderKioskButton : GorillaPressableButton
{
	// Token: 0x06002049 RID: 8265 RVA: 0x000AB2E9 File Offset: 0x000A94E9
	public override void Start()
	{
		this.currentPieceSet = BuilderKiosk.nullItem;
	}

	// Token: 0x0600204A RID: 8266 RVA: 0x000AB2F6 File Offset: 0x000A94F6
	public override void UpdateColor()
	{
		if (this.currentPieceSet.isNullItem)
		{
			this.buttonRenderer.material = this.unpressedMaterial;
			this.myText.text = "";
			return;
		}
		base.UpdateColor();
	}

	// Token: 0x0600204B RID: 8267 RVA: 0x000AB32D File Offset: 0x000A952D
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivation();
	}

	// Token: 0x0400291A RID: 10522
	public BuilderSetManager.BuilderSetStoreItem currentPieceSet;

	// Token: 0x0400291B RID: 10523
	public BuilderKiosk kiosk;

	// Token: 0x0400291C RID: 10524
	public Text setNameText;
}
