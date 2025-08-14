using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GorillaNetworking.Store
{
	// Token: 0x02000DC7 RID: 3527
	public class BundleStand : MonoBehaviour
	{
		// Token: 0x17000868 RID: 2152
		// (get) Token: 0x06005781 RID: 22401 RVA: 0x001B20BB File Offset: 0x001B02BB
		public string playfabBundleID
		{
			get
			{
				return this._bundleDataReference.playfabBundleID;
			}
		}

		// Token: 0x06005782 RID: 22402 RVA: 0x001B20C8 File Offset: 0x001B02C8
		public void Awake()
		{
			this._bundlePurchaseButton.playfabID = this.playfabBundleID;
			if (this._bundleIcon != null && this._bundleDataReference != null && this._bundleDataReference.bundleImage != null)
			{
				this._bundleIcon.sprite = this._bundleDataReference.bundleImage;
			}
		}

		// Token: 0x06005783 RID: 22403 RVA: 0x001B212B File Offset: 0x001B032B
		public void InitializeEventListeners()
		{
			this.AlreadyOwnEvent.AddListener(new UnityAction(this._bundlePurchaseButton.AlreadyOwn));
			this.ErrorHappenedEvent.AddListener(new UnityAction(this._bundlePurchaseButton.ErrorHappened));
		}

		// Token: 0x06005784 RID: 22404 RVA: 0x001B2165 File Offset: 0x001B0365
		public void NotifyAlreadyOwn()
		{
			this.AlreadyOwnEvent.Invoke();
		}

		// Token: 0x06005785 RID: 22405 RVA: 0x001B2172 File Offset: 0x001B0372
		public void ErrorHappened()
		{
			this.ErrorHappenedEvent.Invoke();
		}

		// Token: 0x06005786 RID: 22406 RVA: 0x001B217F File Offset: 0x001B037F
		public void UpdatePurchaseButtonText(string purchaseText)
		{
			if (this._bundlePurchaseButton != null)
			{
				this._bundlePurchaseButton.UpdatePurchaseButtonText(purchaseText);
			}
		}

		// Token: 0x06005787 RID: 22407 RVA: 0x001B219B File Offset: 0x001B039B
		public void UpdateDescriptionText(string descriptionText)
		{
			if (this._bundleDescriptionText != null)
			{
				this._bundleDescriptionText.text = descriptionText;
			}
		}

		// Token: 0x04006150 RID: 24912
		public BundlePurchaseButton _bundlePurchaseButton;

		// Token: 0x04006151 RID: 24913
		[SerializeField]
		public StoreBundleData _bundleDataReference;

		// Token: 0x04006152 RID: 24914
		public GameObject[] EditorOnlyObjects;

		// Token: 0x04006153 RID: 24915
		public Text _bundleDescriptionText;

		// Token: 0x04006154 RID: 24916
		public Image _bundleIcon;

		// Token: 0x04006155 RID: 24917
		public UnityEvent AlreadyOwnEvent;

		// Token: 0x04006156 RID: 24918
		public UnityEvent ErrorHappenedEvent;
	}
}
