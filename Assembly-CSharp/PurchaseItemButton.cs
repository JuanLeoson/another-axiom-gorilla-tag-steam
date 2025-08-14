using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000472 RID: 1138
public class PurchaseItemButton : GorillaPressableButton
{
	// Token: 0x06001C46 RID: 7238 RVA: 0x00097E33 File Offset: 0x00096033
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressPurchaseItemButton(this, isLeftHand);
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x06001C47 RID: 7239 RVA: 0x00097E56 File Offset: 0x00096056
	private IEnumerator ButtonColorUpdate()
	{
		Debug.Log("did this happen?");
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.debounceTime);
		this.buttonRenderer.material = (this.isOn ? this.pressedMaterial : this.unpressedMaterial);
		yield break;
	}

	// Token: 0x040024C4 RID: 9412
	public string buttonSide;
}
