using System;
using UnityEngine;

// Token: 0x02000484 RID: 1156
public class CosmeticCategoryButton : CosmeticButton
{
	// Token: 0x06001CBA RID: 7354 RVA: 0x0009A6AA File Offset: 0x000988AA
	public void SetIcon(Sprite sprite)
	{
		this.equippedLeftIcon.enabled = false;
		this.equippedRightIcon.enabled = false;
		this.equippedIcon.enabled = (sprite != null);
		this.equippedIcon.sprite = sprite;
	}

	// Token: 0x06001CBB RID: 7355 RVA: 0x0009A6E4 File Offset: 0x000988E4
	public void SetDualIcon(Sprite leftSprite, Sprite rightSprite)
	{
		this.equippedLeftIcon.enabled = (leftSprite != null);
		this.equippedRightIcon.enabled = (rightSprite != null);
		this.equippedIcon.enabled = false;
		this.equippedLeftIcon.sprite = leftSprite;
		this.equippedRightIcon.sprite = rightSprite;
	}

	// Token: 0x06001CBC RID: 7356 RVA: 0x0009A73C File Offset: 0x0009893C
	public override void UpdatePosition()
	{
		base.UpdatePosition();
		if (this.equippedIcon != null)
		{
			this.equippedIcon.transform.position += this.posOffset;
		}
		if (this.equippedLeftIcon != null)
		{
			this.equippedLeftIcon.transform.position += this.posOffset;
		}
		if (this.equippedRightIcon != null)
		{
			this.equippedRightIcon.transform.position += this.posOffset;
		}
	}

	// Token: 0x0400251D RID: 9501
	[SerializeField]
	private SpriteRenderer equippedIcon;

	// Token: 0x0400251E RID: 9502
	[SerializeField]
	private SpriteRenderer equippedLeftIcon;

	// Token: 0x0400251F RID: 9503
	[SerializeField]
	private SpriteRenderer equippedRightIcon;
}
