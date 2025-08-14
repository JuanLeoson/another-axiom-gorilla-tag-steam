using System;
using GorillaExtensions;
using GorillaTagScripts.VirtualStumpCustomMaps;
using GT_CustomMapSupportRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GorillaNetworking.Store
{
	// Token: 0x02000DCB RID: 3531
	public class DynamicCosmeticStand : MonoBehaviour, iFlagForBaking
	{
		// Token: 0x0600579C RID: 22428 RVA: 0x001B2680 File Offset: 0x001B0880
		public virtual void SetForBaking()
		{
			this.GorillaHeadModel.SetActive(true);
			this.GorillaTorsoModel.SetActive(true);
			this.GorillaTorsoPostModel.SetActive(true);
			this.GorillaMannequinModel.SetActive(true);
			this.JeweleryBoxModel.SetActive(true);
			this.root.SetActive(true);
			this.DisplayHeadModel.gameObject.SetActive(false);
		}

		// Token: 0x0600579D RID: 22429 RVA: 0x001B26E6 File Offset: 0x001B08E6
		public void OnEnable()
		{
			this.addToCartTextTMP.gameObject.SetActive(true);
			this.slotPriceTextTMP.gameObject.SetActive(true);
		}

		// Token: 0x0600579E RID: 22430 RVA: 0x001B270A File Offset: 0x001B090A
		public void OnDisable()
		{
			this.addToCartTextTMP.gameObject.SetActive(false);
			this.slotPriceTextTMP.gameObject.SetActive(false);
		}

		// Token: 0x0600579F RID: 22431 RVA: 0x001B272E File Offset: 0x001B092E
		public virtual void SetForGame()
		{
			this.DisplayHeadModel.gameObject.SetActive(true);
			this.SetStandType(this.DisplayHeadModel.bustType);
		}

		// Token: 0x17000870 RID: 2160
		// (get) Token: 0x060057A0 RID: 22432 RVA: 0x001B2752 File Offset: 0x001B0952
		// (set) Token: 0x060057A1 RID: 22433 RVA: 0x001B275A File Offset: 0x001B095A
		public string thisCosmeticName
		{
			get
			{
				return this._thisCosmeticName;
			}
			set
			{
				this._thisCosmeticName = value;
			}
		}

		// Token: 0x060057A2 RID: 22434 RVA: 0x001B2764 File Offset: 0x001B0964
		public void InitializeCosmetic()
		{
			this.thisCosmeticItem = CosmeticsController.instance.allCosmetics.Find((CosmeticsController.CosmeticItem x) => this.thisCosmeticName == x.displayName || this.thisCosmeticName == x.overrideDisplayName || this.thisCosmeticName == x.itemName);
			if (this.slotPriceText != null)
			{
				this.slotPriceText.text = this.thisCosmeticItem.itemCategory.ToString().ToUpper() + " " + this.thisCosmeticItem.cost.ToString();
			}
			if (this.slotPriceTextTMP != null)
			{
				this.slotPriceTextTMP.text = this.thisCosmeticItem.itemCategory.ToString().ToUpper() + " " + this.thisCosmeticItem.cost.ToString();
			}
		}

		// Token: 0x060057A3 RID: 22435 RVA: 0x001B2830 File Offset: 0x001B0A30
		public void SpawnItemOntoStand(string PlayFabID)
		{
			this.ClearCosmetics();
			if (PlayFabID.IsNullOrEmpty())
			{
				GTDev.LogWarning<string>("ManuallyInitialize: PlayFabID is null or empty for " + this.StandName, null);
				return;
			}
			if (StoreController.instance.IsNotNull() && Application.isPlaying)
			{
				StoreController.instance.RemoveStandFromPlayFabIDDictionary(this);
			}
			this.thisCosmeticName = PlayFabID;
			if (this.thisCosmeticName.Length == 5)
			{
				this.thisCosmeticName += ".";
			}
			if (Application.isPlaying)
			{
				this.DisplayHeadModel.LoadCosmeticPartsV2(this.thisCosmeticName, false);
			}
			else
			{
				this.DisplayHeadModel.LoadCosmeticParts(StoreController.FindCosmeticInAllCosmeticsArraySO(this.thisCosmeticName), false);
			}
			if (StoreController.instance.IsNotNull() && Application.isPlaying)
			{
				StoreController.instance.AddStandToPlayfabIDDictionary(this);
			}
		}

		// Token: 0x060057A4 RID: 22436 RVA: 0x001B2903 File Offset: 0x001B0B03
		public void ClearCosmetics()
		{
			this.thisCosmeticName = "";
			this.DisplayHeadModel.ClearManuallySpawnedCosmeticParts();
			this.DisplayHeadModel.ClearCosmetics();
		}

		// Token: 0x060057A5 RID: 22437 RVA: 0x001B2928 File Offset: 0x001B0B28
		public void SetStandType(HeadModel_CosmeticStand.BustType newBustType)
		{
			this.DisplayHeadModel.SetStandType(newBustType);
			this.GorillaHeadModel.SetActive(false);
			this.GorillaTorsoModel.SetActive(false);
			this.GorillaTorsoPostModel.SetActive(false);
			this.GorillaMannequinModel.SetActive(false);
			this.GuitarStandModel.SetActive(false);
			this.JeweleryBoxModel.SetActive(false);
			this.AddToCartButton.gameObject.SetActive(true);
			Text text = this.slotPriceText;
			if (text != null)
			{
				text.gameObject.SetActive(true);
			}
			TMP_Text tmp_Text = this.slotPriceTextTMP;
			if (tmp_Text != null)
			{
				tmp_Text.gameObject.SetActive(true);
			}
			Text text2 = this.addToCartText;
			if (text2 != null)
			{
				text2.gameObject.SetActive(true);
			}
			TMP_Text tmp_Text2 = this.addToCartTextTMP;
			if (tmp_Text2 != null)
			{
				tmp_Text2.gameObject.SetActive(true);
			}
			switch (newBustType)
			{
			case HeadModel_CosmeticStand.BustType.Disabled:
			{
				this.ClearCosmetics();
				this.thisCosmeticName = "";
				this.AddToCartButton.gameObject.SetActive(false);
				Text text3 = this.slotPriceText;
				if (text3 != null)
				{
					text3.gameObject.SetActive(false);
				}
				TMP_Text tmp_Text3 = this.slotPriceTextTMP;
				if (tmp_Text3 != null)
				{
					tmp_Text3.gameObject.SetActive(false);
				}
				Text text4 = this.addToCartText;
				if (text4 != null)
				{
					text4.gameObject.SetActive(false);
				}
				TMP_Text tmp_Text4 = this.addToCartTextTMP;
				if (tmp_Text4 != null)
				{
					tmp_Text4.gameObject.SetActive(false);
				}
				this.DisplayHeadModel.transform.localPosition = Vector3.zero;
				this.DisplayHeadModel.transform.localRotation = Quaternion.identity;
				this.root.SetActive(false);
				break;
			}
			case HeadModel_CosmeticStand.BustType.GorillaHead:
				this.root.SetActive(true);
				this.GorillaHeadModel.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.GorillaHeadModel.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.GorillaHeadModel.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.GorillaTorso:
				this.root.SetActive(true);
				this.GorillaTorsoModel.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.GorillaTorsoModel.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.GorillaTorsoModel.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.GorillaTorsoPost:
				this.root.SetActive(true);
				this.GorillaTorsoPostModel.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.GorillaTorsoPostModel.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.GorillaTorsoPostModel.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.GorillaMannequin:
				this.root.SetActive(true);
				this.GorillaMannequinModel.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.GorillaMannequinModel.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.GorillaMannequinModel.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.GuitarStand:
				this.root.SetActive(true);
				this.GuitarStandModel.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.GuitarStandMount.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.GuitarStandMount.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.JewelryBox:
				this.root.SetActive(true);
				this.JeweleryBoxModel.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.JeweleryBoxMount.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.JeweleryBoxMount.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.Table:
				this.root.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.TableMount.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.TableMount.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.PinDisplay:
				this.root.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.PinDisplayMount.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.PinDisplayMount.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.TagEffectDisplay:
				this.root.SetActive(true);
				break;
			default:
				this.root.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = Vector3.zero;
				this.DisplayHeadModel.transform.localRotation = Quaternion.identity;
				break;
			}
			this.SpawnItemOntoStand(this.thisCosmeticName);
		}

		// Token: 0x060057A6 RID: 22438 RVA: 0x001B2E04 File Offset: 0x001B1004
		public void CopyChildsName()
		{
			foreach (DynamicCosmeticStand dynamicCosmeticStand in base.gameObject.GetComponentsInChildren<DynamicCosmeticStand>(true))
			{
				if (dynamicCosmeticStand != this)
				{
					this.StandName = dynamicCosmeticStand.StandName;
				}
			}
		}

		// Token: 0x060057A7 RID: 22439 RVA: 0x001B2E48 File Offset: 0x001B1048
		public void PressCosmeticStandButton()
		{
			this.searchIndex = CosmeticsController.instance.currentCart.IndexOf(this.thisCosmeticItem);
			if (this.searchIndex != -1)
			{
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.cart_item_remove, this.thisCosmeticItem);
				CosmeticsController.instance.currentCart.RemoveAt(this.searchIndex);
				foreach (DynamicCosmeticStand dynamicCosmeticStand in StoreController.instance.StandsByPlayfabID[this.thisCosmeticItem.itemName])
				{
					dynamicCosmeticStand.AddToCartButton.isOn = false;
					dynamicCosmeticStand.AddToCartButton.UpdateColor();
				}
				for (int i = 0; i < 16; i++)
				{
					if (this.thisCosmeticItem.itemName == CosmeticsController.instance.tryOnSet.items[i].itemName)
					{
						CosmeticsController.instance.tryOnSet.items[i] = CosmeticsController.instance.nullItem;
					}
				}
			}
			else
			{
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.cart_item_add, this.thisCosmeticItem);
				CosmeticsController.instance.currentCart.Insert(0, this.thisCosmeticItem);
				foreach (DynamicCosmeticStand dynamicCosmeticStand2 in StoreController.instance.StandsByPlayfabID[this.thisCosmeticName])
				{
					dynamicCosmeticStand2.AddToCartButton.isOn = true;
					dynamicCosmeticStand2.AddToCartButton.UpdateColor();
				}
				if (CosmeticsController.instance.currentCart.Count > CosmeticsController.instance.numFittingRoomButtons)
				{
					foreach (DynamicCosmeticStand dynamicCosmeticStand3 in StoreController.instance.StandsByPlayfabID[CosmeticsController.instance.currentCart[CosmeticsController.instance.numFittingRoomButtons].itemName])
					{
						dynamicCosmeticStand3.AddToCartButton.isOn = false;
						dynamicCosmeticStand3.AddToCartButton.UpdateColor();
					}
					CosmeticsController.instance.currentCart.RemoveAt(CosmeticsController.instance.numFittingRoomButtons);
				}
			}
			CosmeticsController.instance.UpdateShoppingCart();
		}

		// Token: 0x060057A8 RID: 22440 RVA: 0x001B30D0 File Offset: 0x001B12D0
		public void SetStandTypeString(string bustTypeString)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(bustTypeString);
			if (num <= 1590453963U)
			{
				if (num <= 1121133049U)
				{
					if (num != 214514339U)
					{
						if (num == 1121133049U)
						{
							if (bustTypeString == "GuitarStand")
							{
								this.SetStandType(HeadModel_CosmeticStand.BustType.GuitarStand);
								return;
							}
						}
					}
					else if (bustTypeString == "GorillaHead")
					{
						this.SetStandType(HeadModel_CosmeticStand.BustType.GorillaHead);
						return;
					}
				}
				else if (num != 1364530810U)
				{
					if (num != 1520673798U)
					{
						if (num == 1590453963U)
						{
							if (bustTypeString == "GorillaMannequin")
							{
								this.SetStandType(HeadModel_CosmeticStand.BustType.GorillaMannequin);
								return;
							}
						}
					}
					else if (bustTypeString == "JewelryBox")
					{
						this.SetStandType(HeadModel_CosmeticStand.BustType.JewelryBox);
						return;
					}
				}
				else if (bustTypeString == "PinDisplay")
				{
					this.SetStandType(HeadModel_CosmeticStand.BustType.PinDisplay);
					return;
				}
			}
			else if (num <= 2111326094U)
			{
				if (num != 1952506660U)
				{
					if (num == 2111326094U)
					{
						if (bustTypeString == "GorillaTorsoPost")
						{
							this.SetStandType(HeadModel_CosmeticStand.BustType.GorillaTorsoPost);
							return;
						}
					}
				}
				else if (bustTypeString == "GorillaTorso")
				{
					this.SetStandType(HeadModel_CosmeticStand.BustType.GorillaTorso);
					return;
				}
			}
			else if (num != 3217987877U)
			{
				if (num != 3607948159U)
				{
					if (num == 3845287012U)
					{
						if (bustTypeString == "TagEffectDisplay")
						{
							this.SetStandType(HeadModel_CosmeticStand.BustType.TagEffectDisplay);
							return;
						}
					}
				}
				else if (bustTypeString == "Table")
				{
					this.SetStandType(HeadModel_CosmeticStand.BustType.Table);
					return;
				}
			}
			else if (bustTypeString == "Disabled")
			{
				this.SetStandType(HeadModel_CosmeticStand.BustType.Disabled);
				return;
			}
			this.SetStandType(HeadModel_CosmeticStand.BustType.Table);
		}

		// Token: 0x060057A9 RID: 22441 RVA: 0x001B327E File Offset: 0x001B147E
		public void UpdateCosmeticsMountPositions()
		{
			this.DisplayHeadModel.UpdateCosmeticsMountPositions(StoreController.FindCosmeticInAllCosmeticsArraySO(this.thisCosmeticName));
		}

		// Token: 0x060057AA RID: 22442 RVA: 0x001B3298 File Offset: 0x001B1498
		public void InitializeForCustomMapCosmeticItem(GTObjectPlaceholder.ECustomMapCosmeticItem cosmeticItemSlot)
		{
			this.StandName = "CustomMapCosmeticItemStand-" + cosmeticItemSlot.ToString();
			this.ClearCosmetics();
			CustomMapCosmeticItem customMapCosmeticItem;
			if (CosmeticsController.instance.customMapCosmeticsData.TryGetItem(cosmeticItemSlot, out customMapCosmeticItem))
			{
				this.thisCosmeticName = customMapCosmeticItem.playFabID;
				this.SetStandType(customMapCosmeticItem.bustType);
				this.InitializeCosmetic();
			}
		}

		// Token: 0x04006164 RID: 24932
		public HeadModel_CosmeticStand DisplayHeadModel;

		// Token: 0x04006165 RID: 24933
		public GorillaPressableButton AddToCartButton;

		// Token: 0x04006166 RID: 24934
		[HideInInspector]
		public Text slotPriceText;

		// Token: 0x04006167 RID: 24935
		[HideInInspector]
		public Text addToCartText;

		// Token: 0x04006168 RID: 24936
		public TMP_Text slotPriceTextTMP;

		// Token: 0x04006169 RID: 24937
		public TMP_Text addToCartTextTMP;

		// Token: 0x0400616A RID: 24938
		private CosmeticsController.CosmeticItem thisCosmeticItem;

		// Token: 0x0400616B RID: 24939
		[FormerlySerializedAs("StandID")]
		public string StandName;

		// Token: 0x0400616C RID: 24940
		public string _thisCosmeticName = "";

		// Token: 0x0400616D RID: 24941
		public GameObject GorillaHeadModel;

		// Token: 0x0400616E RID: 24942
		public GameObject GorillaTorsoModel;

		// Token: 0x0400616F RID: 24943
		public GameObject GorillaTorsoPostModel;

		// Token: 0x04006170 RID: 24944
		public GameObject GorillaMannequinModel;

		// Token: 0x04006171 RID: 24945
		public GameObject GuitarStandModel;

		// Token: 0x04006172 RID: 24946
		public GameObject GuitarStandMount;

		// Token: 0x04006173 RID: 24947
		public GameObject JeweleryBoxModel;

		// Token: 0x04006174 RID: 24948
		public GameObject JeweleryBoxMount;

		// Token: 0x04006175 RID: 24949
		public GameObject TableMount;

		// Token: 0x04006176 RID: 24950
		[FormerlySerializedAs("PinDisplayMounnt")]
		[FormerlySerializedAs("PinDisplayMountn")]
		public GameObject PinDisplayMount;

		// Token: 0x04006177 RID: 24951
		public GameObject root;

		// Token: 0x04006178 RID: 24952
		public GameObject TagEffectDisplayMount;

		// Token: 0x04006179 RID: 24953
		public GameObject TageEffectDisplayModel;

		// Token: 0x0400617A RID: 24954
		private int searchIndex;
	}
}
