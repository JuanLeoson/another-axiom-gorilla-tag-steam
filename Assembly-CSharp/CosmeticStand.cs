using System;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000422 RID: 1058
public class CosmeticStand : GorillaPressableButton
{
	// Token: 0x0600199A RID: 6554 RVA: 0x00089CC4 File Offset: 0x00087EC4
	public void InitializeCosmetic()
	{
		this.thisCosmeticItem = CosmeticsController.instance.allCosmetics.Find((CosmeticsController.CosmeticItem x) => this.thisCosmeticName == x.displayName || this.thisCosmeticName == x.overrideDisplayName || this.thisCosmeticName == x.itemName);
		if (this.slotPriceText != null)
		{
			this.slotPriceText.text = this.thisCosmeticItem.itemCategory.ToString().ToUpper() + " " + this.thisCosmeticItem.cost.ToString();
		}
	}

	// Token: 0x0600199B RID: 6555 RVA: 0x00089D42 File Offset: 0x00087F42
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressCosmeticStandButton(this);
	}

	// Token: 0x040021FE RID: 8702
	public CosmeticsController.CosmeticItem thisCosmeticItem;

	// Token: 0x040021FF RID: 8703
	public string thisCosmeticName;

	// Token: 0x04002200 RID: 8704
	public HeadModel thisHeadModel;

	// Token: 0x04002201 RID: 8705
	public Text slotPriceText;

	// Token: 0x04002202 RID: 8706
	public Text addToCartText;

	// Token: 0x04002203 RID: 8707
	[Tooltip("If this is true then this cosmetic stand should have already been updated when the 'Update Cosmetic Stands' button was pressed in the CosmeticsController inspector.")]
	public bool skipMe;
}
