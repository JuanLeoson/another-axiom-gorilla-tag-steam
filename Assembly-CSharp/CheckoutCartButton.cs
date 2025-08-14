using System;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200041A RID: 1050
public class CheckoutCartButton : GorillaPressableButton
{
	// Token: 0x06001984 RID: 6532 RVA: 0x000895EB File Offset: 0x000877EB
	public override void Start()
	{
		this.currentCosmeticItem = CosmeticsController.instance.nullItem;
	}

	// Token: 0x06001985 RID: 6533 RVA: 0x00089600 File Offset: 0x00087800
	public override void UpdateColor()
	{
		if (this.currentCosmeticItem.itemName == "null")
		{
			if (this.buttonRenderer.IsNotNull())
			{
				this.buttonRenderer.material = this.unpressedMaterial;
			}
			if (this.myText.IsNotNull())
			{
				this.myText.text = this.noCosmeticText;
			}
			if (this.myTmpText.IsNotNull())
			{
				this.myTmpText.text = this.noCosmeticText;
			}
			if (this.myTmpText2.IsNotNull())
			{
				this.myTmpText2.text = this.noCosmeticText;
				return;
			}
		}
		else if (this.isOn)
		{
			if (this.buttonRenderer.IsNotNull())
			{
				this.buttonRenderer.material = this.pressedMaterial;
			}
			if (this.myText.IsNotNull())
			{
				this.myText.text = this.onText;
			}
			if (this.myTmpText.IsNotNull())
			{
				this.myTmpText.text = this.onText;
			}
			if (this.myTmpText2.IsNotNull())
			{
				this.myTmpText2.text = this.onText;
				return;
			}
		}
		else
		{
			if (this.buttonRenderer.IsNotNull())
			{
				this.buttonRenderer.material = this.unpressedMaterial;
			}
			if (this.myText.IsNotNull())
			{
				this.myText.text = this.offText;
			}
			if (this.myTmpText.IsNotNull())
			{
				this.myTmpText.text = this.offText;
			}
			if (this.myTmpText2.IsNotNull())
			{
				this.myTmpText2.text = this.offText;
			}
		}
	}

	// Token: 0x06001986 RID: 6534 RVA: 0x0008979C File Offset: 0x0008799C
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressCheckoutCartButton(this, isLeftHand);
	}

	// Token: 0x06001987 RID: 6535 RVA: 0x000897B2 File Offset: 0x000879B2
	public void SetItem(CosmeticsController.CosmeticItem item, bool isCurrentItemToBuy)
	{
		this.currentCosmeticItem = item;
		if (this.currentCosmeticSprite.IsNotNull())
		{
			this.currentCosmeticSprite.sprite = this.currentCosmeticItem.itemPicture;
		}
		this.isOn = isCurrentItemToBuy;
		this.UpdateColor();
	}

	// Token: 0x06001988 RID: 6536 RVA: 0x000897EB File Offset: 0x000879EB
	public void ClearItem()
	{
		this.currentCosmeticItem = CosmeticsController.instance.nullItem;
		if (this.currentCosmeticSprite.IsNotNull())
		{
			this.currentCosmeticSprite.sprite = this.blankSprite;
		}
		this.isOn = false;
		this.UpdateColor();
	}

	// Token: 0x040021E0 RID: 8672
	public CosmeticsController.CosmeticItem currentCosmeticItem;

	// Token: 0x040021E1 RID: 8673
	[SerializeField]
	private SpriteRenderer currentCosmeticSprite;

	// Token: 0x040021E2 RID: 8674
	[SerializeField]
	private Sprite blankSprite;

	// Token: 0x040021E3 RID: 8675
	public string noCosmeticText;
}
