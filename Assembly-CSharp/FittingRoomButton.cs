using System;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000425 RID: 1061
public class FittingRoomButton : GorillaPressableButton
{
	// Token: 0x060019AA RID: 6570 RVA: 0x00089EEB File Offset: 0x000880EB
	public override void Start()
	{
		this.currentCosmeticItem = CosmeticsController.instance.nullItem;
	}

	// Token: 0x060019AB RID: 6571 RVA: 0x00089F00 File Offset: 0x00088100
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

	// Token: 0x060019AC RID: 6572 RVA: 0x0008A09C File Offset: 0x0008829C
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivationWithHand(isLeftHand);
		CosmeticsController.instance.PressFittingRoomButton(this, isLeftHand);
	}

	// Token: 0x060019AD RID: 6573 RVA: 0x0008A0B3 File Offset: 0x000882B3
	public void SetItem(CosmeticsController.CosmeticItem item, bool isInTryOnSet)
	{
		this.currentCosmeticItem = item;
		if (this.currentCosmeticSprite.IsNotNull())
		{
			this.currentCosmeticSprite.sprite = this.currentCosmeticItem.itemPicture;
		}
		this.isOn = isInTryOnSet;
		this.UpdateColor();
	}

	// Token: 0x060019AE RID: 6574 RVA: 0x0008A0EC File Offset: 0x000882EC
	public void ClearItem()
	{
		if (this.currentCosmeticItem.isNullItem)
		{
			return;
		}
		this.currentCosmeticItem = CosmeticsController.instance.nullItem;
		if (this.currentCosmeticSprite.IsNotNull())
		{
			this.currentCosmeticSprite.sprite = this.blankSprite;
		}
		this.isOn = false;
		this.UpdateColor();
	}

	// Token: 0x04002207 RID: 8711
	public CosmeticsController.CosmeticItem currentCosmeticItem;

	// Token: 0x04002208 RID: 8712
	[SerializeField]
	private SpriteRenderer currentCosmeticSprite;

	// Token: 0x04002209 RID: 8713
	[SerializeField]
	private Sprite blankSprite;

	// Token: 0x0400220A RID: 8714
	public string noCosmeticText;
}
