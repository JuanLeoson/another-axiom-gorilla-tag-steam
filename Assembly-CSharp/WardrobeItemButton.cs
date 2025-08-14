using System;
using GorillaNetworking;

// Token: 0x0200048F RID: 1167
public class WardrobeItemButton : GorillaPressableButton
{
	// Token: 0x06001CF1 RID: 7409 RVA: 0x0009BD59 File Offset: 0x00099F59
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivationWithHand(isLeftHand);
		CosmeticsController.instance.PressWardrobeItemButton(this.currentCosmeticItem, isLeftHand);
	}

	// Token: 0x04002558 RID: 9560
	public HeadModel controlledModel;

	// Token: 0x04002559 RID: 9561
	public CosmeticsController.CosmeticItem currentCosmeticItem;
}
