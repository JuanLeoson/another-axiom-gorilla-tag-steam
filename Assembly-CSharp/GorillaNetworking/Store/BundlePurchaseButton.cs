using System;
using System.Collections;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000DC5 RID: 3525
	public class BundlePurchaseButton : GorillaPressableButton, IGorillaSliceableSimple
	{
		// Token: 0x06005770 RID: 22384 RVA: 0x000172AD File Offset: 0x000154AD
		public void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06005771 RID: 22385 RVA: 0x000172B6 File Offset: 0x000154B6
		public void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06005772 RID: 22386 RVA: 0x001B1DE8 File Offset: 0x001AFFE8
		public void SliceUpdate()
		{
			if (NetworkSystem.Instance != null && NetworkSystem.Instance.WrongVersion && !this.bError)
			{
				base.enabled = false;
				base.GetComponent<BoxCollider>().enabled = false;
				this.buttonRenderer.material = this.pressedMaterial;
				this.myText.text = this.UnavailableText;
			}
		}

		// Token: 0x06005773 RID: 22387 RVA: 0x001B1E4B File Offset: 0x001B004B
		public override void ButtonActivation()
		{
			if (this.bError)
			{
				return;
			}
			base.ButtonActivation();
			BundleManager.instance.BundlePurchaseButtonPressed(this.playfabID);
			base.StartCoroutine(this.ButtonColorUpdate());
		}

		// Token: 0x06005774 RID: 22388 RVA: 0x001B1E7C File Offset: 0x001B007C
		public void AlreadyOwn()
		{
			if (this.bError)
			{
				return;
			}
			base.enabled = false;
			base.GetComponent<BoxCollider>().enabled = false;
			this.buttonRenderer.material = this.pressedMaterial;
			this.onText = this.AlreadyOwnText;
			this.myText.text = this.AlreadyOwnText;
			this.isOn = true;
		}

		// Token: 0x06005775 RID: 22389 RVA: 0x001B1EDC File Offset: 0x001B00DC
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
			this.isOn = false;
		}

		// Token: 0x06005776 RID: 22390 RVA: 0x001B1F2E File Offset: 0x001B012E
		private IEnumerator ButtonColorUpdate()
		{
			this.buttonRenderer.material = this.pressedMaterial;
			yield return new WaitForSeconds(this.debounceTime);
			this.buttonRenderer.material = (this.isOn ? this.pressedMaterial : this.unpressedMaterial);
			yield break;
		}

		// Token: 0x06005777 RID: 22391 RVA: 0x001B1F40 File Offset: 0x001B0140
		public void ErrorHappened()
		{
			this.bError = true;
			this.myText.text = this.ErrorText;
			this.buttonRenderer.material = this.unpressedMaterial;
			base.enabled = false;
			this.offText = this.ErrorText;
			this.onText = this.ErrorText;
			this.isOn = false;
		}

		// Token: 0x06005778 RID: 22392 RVA: 0x001B1F9C File Offset: 0x001B019C
		public void InitializeData()
		{
			if (this.bError)
			{
				return;
			}
			this.myText.text = this.offText;
			this.buttonRenderer.material = this.unpressedMaterial;
			base.enabled = true;
			this.isOn = false;
		}

		// Token: 0x06005779 RID: 22393 RVA: 0x001B1FD7 File Offset: 0x001B01D7
		public void UpdatePurchaseButtonText(string purchaseText)
		{
			if (!this.bError)
			{
				this.offText = purchaseText;
				this.UpdateColor();
			}
		}

		// Token: 0x04006148 RID: 24904
		public bool bError;

		// Token: 0x04006149 RID: 24905
		public string ErrorText = "ERROR COMPLETING PURCHASE! PLEASE RESTART THE GAME";

		// Token: 0x0400614A RID: 24906
		public string AlreadyOwnText = "YOU OWN THE BUNDLE ALREADY! THANK YOU!";

		// Token: 0x0400614B RID: 24907
		public string UnavailableText = "UNAVAILABLE";

		// Token: 0x0400614C RID: 24908
		public string playfabID = "";
	}
}
