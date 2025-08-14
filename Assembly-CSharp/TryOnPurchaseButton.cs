using System;
using System.Collections;
using GorillaNetworking.Store;
using UnityEngine;

// Token: 0x02000481 RID: 1153
public class TryOnPurchaseButton : GorillaPressableButton
{
	// Token: 0x06001CA9 RID: 7337 RVA: 0x0009A29C File Offset: 0x0009849C
	public void Update()
	{
		if (NetworkSystem.Instance != null && NetworkSystem.Instance.WrongVersion && !this.bError)
		{
			base.enabled = false;
			base.GetComponent<BoxCollider>().enabled = false;
			this.buttonRenderer.material = this.pressedMaterial;
			this.myText.text = "UNAVAILABLE";
		}
	}

	// Token: 0x06001CAA RID: 7338 RVA: 0x0009A2FE File Offset: 0x000984FE
	public override void ButtonActivation()
	{
		if (this.bError)
		{
			return;
		}
		base.ButtonActivation();
		BundleManager.instance.PressPurchaseTryOnBundleButton();
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x06001CAB RID: 7339 RVA: 0x0009A328 File Offset: 0x00098528
	public void AlreadyOwn()
	{
		if (this.bError)
		{
			return;
		}
		base.enabled = false;
		base.GetComponent<BoxCollider>().enabled = false;
		this.buttonRenderer.material = this.pressedMaterial;
		this.myText.text = this.AlreadyOwnText;
	}

	// Token: 0x06001CAC RID: 7340 RVA: 0x0009A368 File Offset: 0x00098568
	public void ResetButton()
	{
		if (this.bError)
		{
			return;
		}
		base.enabled = true;
		base.GetComponent<BoxCollider>().enabled = true;
		this.buttonRenderer.material = this.unpressedMaterial;
		this.myText.text = this.offText;
	}

	// Token: 0x06001CAD RID: 7341 RVA: 0x0009A3A8 File Offset: 0x000985A8
	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.debounceTime);
		this.buttonRenderer.material = (this.isOn ? this.pressedMaterial : this.unpressedMaterial);
		yield break;
	}

	// Token: 0x06001CAE RID: 7342 RVA: 0x0009A3B7 File Offset: 0x000985B7
	public void ErrorHappened()
	{
		this.bError = true;
		this.myText.text = this.ErrorText;
		this.buttonRenderer.material = this.unpressedMaterial;
		base.enabled = false;
		this.isOn = false;
	}

	// Token: 0x04002512 RID: 9490
	public bool bError;

	// Token: 0x04002513 RID: 9491
	public string ErrorText = "ERROR COMPLETING PURCHASE! PLEASE RESTART THE GAME";

	// Token: 0x04002514 RID: 9492
	public string AlreadyOwnText;
}
