using System;
using UnityEngine;

// Token: 0x02000483 RID: 1155
public class CosmeticButton : GorillaPressableButton
{
	// Token: 0x06001CB6 RID: 7350 RVA: 0x0009A49B File Offset: 0x0009869B
	public void Awake()
	{
		this.startingPos = base.transform.localPosition;
	}

	// Token: 0x06001CB7 RID: 7351 RVA: 0x0009A4B0 File Offset: 0x000986B0
	public override void UpdateColor()
	{
		if (!base.enabled)
		{
			this.buttonRenderer.material = this.disabledMaterial;
			if (this.myText != null)
			{
				this.myText.text = this.offText;
			}
		}
		else if (this.isOn)
		{
			this.buttonRenderer.material = this.pressedMaterial;
			if (this.myText != null)
			{
				this.myText.text = this.onText;
			}
		}
		else
		{
			this.buttonRenderer.material = this.unpressedMaterial;
			if (this.myText != null)
			{
				this.myText.text = this.offText;
			}
		}
		this.UpdatePosition();
	}

	// Token: 0x06001CB8 RID: 7352 RVA: 0x0009A568 File Offset: 0x00098768
	public virtual void UpdatePosition()
	{
		Vector3 vector = this.startingPos;
		if (!base.enabled)
		{
			vector += this.disabledOffset;
		}
		else if (this.isOn)
		{
			vector += this.pressedOffset;
		}
		this.posOffset = base.transform.position;
		base.transform.localPosition = vector;
		this.posOffset = base.transform.position - this.posOffset;
		if (this.myText != null)
		{
			this.myText.transform.position += this.posOffset;
		}
		if (this.myTmpText != null)
		{
			this.myTmpText.transform.position += this.posOffset;
		}
		if (this.myTmpText2 != null)
		{
			this.myTmpText2.transform.position += this.posOffset;
		}
	}

	// Token: 0x04002518 RID: 9496
	[SerializeField]
	private Vector3 pressedOffset = new Vector3(0f, 0f, 0.1f);

	// Token: 0x04002519 RID: 9497
	[SerializeField]
	private Material disabledMaterial;

	// Token: 0x0400251A RID: 9498
	[SerializeField]
	private Vector3 disabledOffset = new Vector3(0f, 0f, 0.1f);

	// Token: 0x0400251B RID: 9499
	private Vector3 startingPos;

	// Token: 0x0400251C RID: 9500
	protected Vector3 posOffset;
}
