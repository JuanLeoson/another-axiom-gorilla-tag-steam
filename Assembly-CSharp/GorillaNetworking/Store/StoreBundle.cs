using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaNetworking.Store
{
	// Token: 0x02000DC9 RID: 3529
	[Serializable]
	public class StoreBundle
	{
		// Token: 0x17000869 RID: 2153
		// (get) Token: 0x0600578A RID: 22410 RVA: 0x001B21B7 File Offset: 0x001B03B7
		public string playfabBundleID
		{
			get
			{
				return this._storeBundleDataReference.playfabBundleID;
			}
		}

		// Token: 0x1700086A RID: 2154
		// (get) Token: 0x0600578B RID: 22411 RVA: 0x001B21C4 File Offset: 0x001B03C4
		public string bundleSKU
		{
			get
			{
				return this._storeBundleDataReference.bundleSKU;
			}
		}

		// Token: 0x1700086B RID: 2155
		// (get) Token: 0x0600578C RID: 22412 RVA: 0x001B21D1 File Offset: 0x001B03D1
		public Sprite bundleImage
		{
			get
			{
				return this._storeBundleDataReference.bundleImage;
			}
		}

		// Token: 0x1700086C RID: 2156
		// (get) Token: 0x0600578D RID: 22413 RVA: 0x001B21DE File Offset: 0x001B03DE
		public string price
		{
			get
			{
				return this._price;
			}
		}

		// Token: 0x1700086D RID: 2157
		// (get) Token: 0x0600578E RID: 22414 RVA: 0x001B21E8 File Offset: 0x001B03E8
		public string bundleName
		{
			get
			{
				if (this._bundleName.IsNullOrEmpty())
				{
					int num = CosmeticsController.instance.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => this.playfabBundleID == x.itemName);
					if (num > -1)
					{
						if (!CosmeticsController.instance.allCosmetics[num].overrideDisplayName.IsNullOrEmpty())
						{
							this._bundleName = CosmeticsController.instance.allCosmetics[num].overrideDisplayName;
						}
						else
						{
							this._bundleName = CosmeticsController.instance.allCosmetics[num].displayName;
						}
					}
					else
					{
						this._bundleName = "NULL_BUNDLE_NAME";
					}
				}
				return this._bundleName;
			}
		}

		// Token: 0x1700086E RID: 2158
		// (get) Token: 0x0600578F RID: 22415 RVA: 0x001B2294 File Offset: 0x001B0494
		public bool HasPrice
		{
			get
			{
				return !string.IsNullOrEmpty(this.price) && this.price != StoreBundle.defaultPrice;
			}
		}

		// Token: 0x1700086F RID: 2159
		// (get) Token: 0x06005790 RID: 22416 RVA: 0x001B22B5 File Offset: 0x001B04B5
		public string bundleDescriptionText
		{
			get
			{
				return this._storeBundleDataReference.bundleDescriptionText;
			}
		}

		// Token: 0x06005791 RID: 22417 RVA: 0x001B22C4 File Offset: 0x001B04C4
		public StoreBundle()
		{
			this.isOwned = false;
			this.bundleStands = new List<BundleStand>();
		}

		// Token: 0x06005792 RID: 22418 RVA: 0x001B2318 File Offset: 0x001B0518
		public StoreBundle(StoreBundleData data)
		{
			this.isOwned = false;
			this.bundleStands = new List<BundleStand>();
			this._storeBundleDataReference = data;
		}

		// Token: 0x06005793 RID: 22419 RVA: 0x001B2370 File Offset: 0x001B0570
		public void InitializebundleStands()
		{
			foreach (BundleStand bundleStand in this.bundleStands)
			{
				bundleStand.UpdateDescriptionText(this.bundleDescriptionText);
				bundleStand.InitializeEventListeners();
			}
		}

		// Token: 0x06005794 RID: 22420 RVA: 0x001B23CC File Offset: 0x001B05CC
		public void TryUpdatePrice(uint bundlePrice)
		{
			this.TryUpdatePrice((bundlePrice / 100m).ToString());
		}

		// Token: 0x06005795 RID: 22421 RVA: 0x001B23FC File Offset: 0x001B05FC
		public void TryUpdatePrice(string bundlePrice = null)
		{
			if (!string.IsNullOrEmpty(bundlePrice))
			{
				decimal num;
				this._price = (decimal.TryParse(bundlePrice, out num) ? (StoreBundle.defaultCurrencySymbol + bundlePrice) : bundlePrice);
			}
			this.UpdatePurchaseButtonText();
		}

		// Token: 0x06005796 RID: 22422 RVA: 0x001B2438 File Offset: 0x001B0638
		public void UpdatePurchaseButtonText()
		{
			this.purchaseButtonText = string.Format(this.purchaseButtonStringFormat, this.bundleName, this.price);
			foreach (BundleStand bundleStand in this.bundleStands)
			{
				bundleStand.UpdatePurchaseButtonText(this.purchaseButtonText);
			}
		}

		// Token: 0x06005797 RID: 22423 RVA: 0x001B24AC File Offset: 0x001B06AC
		public void ValidateBundleData()
		{
			if (this._storeBundleDataReference == null)
			{
				Debug.LogError("StoreBundleData is null");
				foreach (BundleStand bundleStand in this.bundleStands)
				{
					if (bundleStand == null)
					{
						Debug.LogError("BundleStand is null");
					}
					else if (bundleStand._bundleDataReference != null)
					{
						this._storeBundleDataReference = bundleStand._bundleDataReference;
						Debug.LogError("BundleStand StoreBundleData is not equal to StoreBundle StoreBundleData");
					}
				}
			}
			if (this._storeBundleDataReference == null)
			{
				Debug.LogError("StoreBundleData is null");
				return;
			}
			if (this._storeBundleDataReference.playfabBundleID.IsNullOrEmpty())
			{
				Debug.LogError("playfabBundleID is null");
			}
			if (this._storeBundleDataReference.bundleSKU.IsNullOrEmpty())
			{
				Debug.LogError("bundleSKU is null");
			}
			if (this._storeBundleDataReference.bundleImage == null)
			{
				Debug.LogError("bundleImage is null");
			}
			if (this._storeBundleDataReference.bundleDescriptionText.IsNullOrEmpty())
			{
				Debug.LogError("bundleDescriptionText is null");
			}
		}

		// Token: 0x04006157 RID: 24919
		private static readonly string defaultPrice = "$--.--";

		// Token: 0x04006158 RID: 24920
		private static readonly string defaultCurrencySymbol = "$";

		// Token: 0x04006159 RID: 24921
		[NonSerialized]
		public string purchaseButtonStringFormat = "THE {0}\n{1}";

		// Token: 0x0400615A RID: 24922
		[SerializeField]
		public List<BundleStand> bundleStands;

		// Token: 0x0400615B RID: 24923
		public bool isOwned;

		// Token: 0x0400615C RID: 24924
		private string _price = StoreBundle.defaultPrice;

		// Token: 0x0400615D RID: 24925
		private string _bundleName = "";

		// Token: 0x0400615E RID: 24926
		public string purchaseButtonText = "";

		// Token: 0x0400615F RID: 24927
		[FormerlySerializedAs("storeBundleDataReference")]
		[SerializeField]
		[ReadOnly]
		private StoreBundleData _storeBundleDataReference;
	}
}
