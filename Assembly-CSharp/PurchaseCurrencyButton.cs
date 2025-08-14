using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000470 RID: 1136
public class PurchaseCurrencyButton : GorillaPressableButton
{
	// Token: 0x06001C3D RID: 7229 RVA: 0x00097D64 File Offset: 0x00095F64
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		ATM_Manager.instance.PressCurrencyPurchaseButton(this.purchaseCurrencySize);
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x06001C3E RID: 7230 RVA: 0x00097D8B File Offset: 0x00095F8B
	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.buttonFadeTime);
		this.buttonRenderer.material = this.unpressedMaterial;
		yield break;
	}

	// Token: 0x040024BF RID: 9407
	public string purchaseCurrencySize;

	// Token: 0x040024C0 RID: 9408
	public float buttonFadeTime = 0.25f;
}
