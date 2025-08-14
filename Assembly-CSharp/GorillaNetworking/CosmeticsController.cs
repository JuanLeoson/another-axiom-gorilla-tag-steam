using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CosmeticRoom;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaNetworking.Store;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using Steamworks;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

namespace GorillaNetworking
{
	// Token: 0x02000D47 RID: 3399
	public class CosmeticsController : MonoBehaviour, IGorillaSliceableSimple, IBuildValidation
	{
		// Token: 0x17000815 RID: 2069
		// (get) Token: 0x06005417 RID: 21527 RVA: 0x0019FE2C File Offset: 0x0019E02C
		// (set) Token: 0x06005418 RID: 21528 RVA: 0x0019FE34 File Offset: 0x0019E034
		public CosmeticInfoV2[] v2_allCosmetics { get; private set; }

		// Token: 0x17000816 RID: 2070
		// (get) Token: 0x06005419 RID: 21529 RVA: 0x0019FE3D File Offset: 0x0019E03D
		// (set) Token: 0x0600541A RID: 21530 RVA: 0x0019FE45 File Offset: 0x0019E045
		public bool v2_allCosmeticsInfoAssetRef_isLoaded { get; private set; }

		// Token: 0x17000817 RID: 2071
		// (get) Token: 0x0600541B RID: 21531 RVA: 0x0019FE4E File Offset: 0x0019E04E
		// (set) Token: 0x0600541C RID: 21532 RVA: 0x0019FE56 File Offset: 0x0019E056
		public bool v2_isGetCosmeticsPlayCatalogDataWaitingForCallback { get; private set; }

		// Token: 0x17000818 RID: 2072
		// (get) Token: 0x0600541D RID: 21533 RVA: 0x0019FE5F File Offset: 0x0019E05F
		// (set) Token: 0x0600541E RID: 21534 RVA: 0x0019FE67 File Offset: 0x0019E067
		public bool v2_isCosmeticPlayFabCatalogDataLoaded { get; private set; }

		// Token: 0x0600541F RID: 21535 RVA: 0x0019FE70 File Offset: 0x0019E070
		private void V2Awake()
		{
			this._allCosmetics = null;
			base.StartCoroutine(this.V2_allCosmeticsInfoAssetRefSO_LoadCoroutine());
		}

		// Token: 0x06005420 RID: 21536 RVA: 0x0019FE86 File Offset: 0x0019E086
		private IEnumerator V2_allCosmeticsInfoAssetRefSO_LoadCoroutine()
		{
			while (!PlayFabAuthenticator.instance)
			{
				yield return new WaitForSeconds(1f);
			}
			float[] retryWaitTimes = new float[]
			{
				1f,
				2f,
				4f,
				4f,
				10f,
				10f,
				10f,
				10f,
				10f,
				10f,
				10f,
				10f,
				10f,
				10f,
				30f
			};
			int retryCount = 0;
			AsyncOperationHandle<AllCosmeticsArraySO> newSysAllCosmeticsAsyncOp;
			for (;;)
			{
				Debug.Log(string.Format("Attempting to load runtime key \"{0}\" ", this.v2_allCosmeticsInfoAssetRef.RuntimeKey) + string.Format("(Attempt: {0})", retryCount + 1));
				newSysAllCosmeticsAsyncOp = this.v2_allCosmeticsInfoAssetRef.LoadAssetAsync();
				yield return newSysAllCosmeticsAsyncOp;
				if (ApplicationQuittingState.IsQuitting)
				{
					break;
				}
				if (!newSysAllCosmeticsAsyncOp.IsValid())
				{
					Debug.LogError("`newSysAllCosmeticsAsyncOp` (should never happen) became invalid some how.");
				}
				if (newSysAllCosmeticsAsyncOp.Status == AsyncOperationStatus.Succeeded)
				{
					goto Block_4;
				}
				Debug.LogError(string.Format("Failed to load \"{0}\". ", this.v2_allCosmeticsInfoAssetRef.RuntimeKey) + "Error: " + newSysAllCosmeticsAsyncOp.OperationException.Message);
				float time = retryWaitTimes[Mathf.Min(retryCount, retryWaitTimes.Length - 1)];
				yield return new WaitForSecondsRealtime(time);
				int num = retryCount;
				retryCount = num + 1;
				newSysAllCosmeticsAsyncOp = default(AsyncOperationHandle<AllCosmeticsArraySO>);
			}
			yield break;
			Block_4:
			this.V2_allCosmeticsInfoAssetRef_LoadSucceeded(newSysAllCosmeticsAsyncOp.Result);
			yield break;
		}

		// Token: 0x06005421 RID: 21537 RVA: 0x0019FE98 File Offset: 0x0019E098
		private void V2_allCosmeticsInfoAssetRef_LoadSucceeded(AllCosmeticsArraySO allCosmeticsSO)
		{
			this.v2_allCosmetics = new CosmeticInfoV2[allCosmeticsSO.sturdyAssetRefs.Length];
			for (int i = 0; i < allCosmeticsSO.sturdyAssetRefs.Length; i++)
			{
				this.v2_allCosmetics[i] = allCosmeticsSO.sturdyAssetRefs[i].obj.info;
			}
			this._allCosmetics = new List<CosmeticsController.CosmeticItem>(allCosmeticsSO.sturdyAssetRefs.Length);
			for (int j = 0; j < this.v2_allCosmetics.Length; j++)
			{
				CosmeticInfoV2 cosmeticInfoV = this.v2_allCosmetics[j];
				string playFabID = cosmeticInfoV.playFabID;
				this._allCosmeticsDictV2[playFabID] = cosmeticInfoV;
				CosmeticsController.CosmeticItem item = new CosmeticsController.CosmeticItem
				{
					itemName = playFabID,
					itemCategory = cosmeticInfoV.category,
					isHoldable = cosmeticInfoV.hasHoldableParts,
					displayName = playFabID,
					itemPicture = cosmeticInfoV.icon,
					overrideDisplayName = cosmeticInfoV.displayName,
					bothHandsHoldable = cosmeticInfoV.usesBothHandSlots,
					isNullItem = false
				};
				this._allCosmetics.Add(item);
			}
			this.v2_allCosmeticsInfoAssetRef_isLoaded = true;
			Action v2_allCosmeticsInfoAssetRef_OnPostLoad = this.V2_allCosmeticsInfoAssetRef_OnPostLoad;
			if (v2_allCosmeticsInfoAssetRef_OnPostLoad == null)
			{
				return;
			}
			v2_allCosmeticsInfoAssetRef_OnPostLoad();
		}

		// Token: 0x06005422 RID: 21538 RVA: 0x0019FFC9 File Offset: 0x0019E1C9
		public bool TryGetCosmeticInfoV2(string playFabId, out CosmeticInfoV2 cosmeticInfo)
		{
			return this._allCosmeticsDictV2.TryGetValue(playFabId, out cosmeticInfo);
		}

		// Token: 0x06005423 RID: 21539 RVA: 0x0019FFD8 File Offset: 0x0019E1D8
		private void V2_ConformCosmeticItemV1DisplayName(ref CosmeticsController.CosmeticItem cosmetic)
		{
			if (cosmetic.itemName == cosmetic.displayName)
			{
				return;
			}
			cosmetic.overrideDisplayName = cosmetic.displayName;
			cosmetic.displayName = cosmetic.itemName;
		}

		// Token: 0x06005424 RID: 21540 RVA: 0x001A0008 File Offset: 0x0019E208
		internal void InitializeCosmeticStands()
		{
			foreach (CosmeticStand cosmeticStand in this.cosmeticStands)
			{
				if (cosmeticStand != null)
				{
					cosmeticStand.InitializeCosmetic();
				}
			}
		}

		// Token: 0x17000819 RID: 2073
		// (get) Token: 0x06005425 RID: 21541 RVA: 0x001A003D File Offset: 0x0019E23D
		// (set) Token: 0x06005426 RID: 21542 RVA: 0x001A0044 File Offset: 0x0019E244
		public static bool hasInstance { get; private set; }

		// Token: 0x1700081A RID: 2074
		// (get) Token: 0x06005427 RID: 21543 RVA: 0x001A004C File Offset: 0x0019E24C
		// (set) Token: 0x06005428 RID: 21544 RVA: 0x001A0054 File Offset: 0x0019E254
		public List<CosmeticsController.CosmeticItem> allCosmetics
		{
			get
			{
				return this._allCosmetics;
			}
			set
			{
				this._allCosmetics = value;
			}
		}

		// Token: 0x1700081B RID: 2075
		// (get) Token: 0x06005429 RID: 21545 RVA: 0x001A005D File Offset: 0x0019E25D
		// (set) Token: 0x0600542A RID: 21546 RVA: 0x001A0065 File Offset: 0x0019E265
		public bool allCosmeticsDict_isInitialized { get; private set; }

		// Token: 0x1700081C RID: 2076
		// (get) Token: 0x0600542B RID: 21547 RVA: 0x001A006E File Offset: 0x0019E26E
		public Dictionary<string, CosmeticsController.CosmeticItem> allCosmeticsDict
		{
			get
			{
				return this._allCosmeticsDict;
			}
		}

		// Token: 0x1700081D RID: 2077
		// (get) Token: 0x0600542C RID: 21548 RVA: 0x001A0076 File Offset: 0x0019E276
		// (set) Token: 0x0600542D RID: 21549 RVA: 0x001A007E File Offset: 0x0019E27E
		public bool allCosmeticsItemIDsfromDisplayNamesDict_isInitialized { get; private set; }

		// Token: 0x1700081E RID: 2078
		// (get) Token: 0x0600542E RID: 21550 RVA: 0x001A0087 File Offset: 0x0019E287
		public Dictionary<string, string> allCosmeticsItemIDsfromDisplayNamesDict
		{
			get
			{
				return this._allCosmeticsItemIDsfromDisplayNamesDict;
			}
		}

		// Token: 0x1700081F RID: 2079
		// (get) Token: 0x0600542F RID: 21551 RVA: 0x001A008F File Offset: 0x0019E28F
		// (set) Token: 0x06005430 RID: 21552 RVA: 0x001A0097 File Offset: 0x0019E297
		public bool isHidingCosmeticsFromRemotePlayers { get; private set; }

		// Token: 0x06005431 RID: 21553 RVA: 0x001A00A0 File Offset: 0x0019E2A0
		public void AddWardrobeInstance(WardrobeInstance instance)
		{
			this.wardrobes.Add(instance);
			if (CosmeticsV2Spawner_Dirty.allPartsInstantiated)
			{
				this.UpdateWardrobeModelsAndButtons();
			}
		}

		// Token: 0x06005432 RID: 21554 RVA: 0x001A00BB File Offset: 0x0019E2BB
		public void RemoveWardrobeInstance(WardrobeInstance instance)
		{
			this.wardrobes.Remove(instance);
		}

		// Token: 0x17000820 RID: 2080
		// (get) Token: 0x06005433 RID: 21555 RVA: 0x001A00CA File Offset: 0x0019E2CA
		public int CurrencyBalance
		{
			get
			{
				return this.currencyBalance;
			}
		}

		// Token: 0x06005434 RID: 21556 RVA: 0x001A00D4 File Offset: 0x0019E2D4
		public void Awake()
		{
			if (CosmeticsController.instance == null)
			{
				CosmeticsController.instance = this;
				CosmeticsController.hasInstance = true;
			}
			else if (CosmeticsController.instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			this.V2Awake();
			if (base.gameObject.activeSelf)
			{
				this.catalog = "DLC";
				this.currencyName = "SR";
				this.nullItem = default(CosmeticsController.CosmeticItem);
				this.nullItem.itemName = "null";
				this.nullItem.displayName = "NOTHING";
				this.nullItem.itemPicture = Resources.Load<Sprite>("CosmeticNull_Icon");
				this.nullItem.itemPictureResourceString = "";
				this.nullItem.overrideDisplayName = "NOTHING";
				this.nullItem.meshAtlasResourceString = "";
				this.nullItem.meshResourceString = "";
				this.nullItem.materialResourceString = "";
				this.nullItem.isNullItem = true;
				this._allCosmeticsDict[this.nullItem.itemName] = this.nullItem;
				this._allCosmeticsItemIDsfromDisplayNamesDict[this.nullItem.displayName] = this.nullItem.itemName;
				for (int i = 0; i < 16; i++)
				{
					this.tryOnSet.items[i] = this.nullItem;
				}
				this.cosmeticsPages[0] = 0;
				this.cosmeticsPages[1] = 0;
				this.cosmeticsPages[2] = 0;
				this.cosmeticsPages[3] = 0;
				this.cosmeticsPages[4] = 0;
				this.cosmeticsPages[5] = 0;
				this.cosmeticsPages[6] = 0;
				this.cosmeticsPages[7] = 0;
				this.cosmeticsPages[8] = 0;
				this.cosmeticsPages[9] = 0;
				this.cosmeticsPages[10] = 0;
				this.itemLists[0] = this.unlockedHats;
				this.itemLists[1] = this.unlockedFaces;
				this.itemLists[2] = this.unlockedBadges;
				this.itemLists[3] = this.unlockedPaws;
				this.itemLists[4] = this.unlockedFurs;
				this.itemLists[5] = this.unlockedShirts;
				this.itemLists[6] = this.unlockedPants;
				this.itemLists[7] = this.unlockedArms;
				this.itemLists[8] = this.unlockedBacks;
				this.itemLists[9] = this.unlockedChests;
				this.itemLists[10] = this.unlockedTagFX;
				this.updateCosmeticsRetries = 0;
				this.maxUpdateCosmeticsRetries = 5;
				this.inventoryStringList.Clear();
				this.inventoryStringList.Add("Inventory");
				base.StartCoroutine(this.CheckCanGetDaily());
			}
		}

		// Token: 0x06005435 RID: 21557 RVA: 0x001A037C File Offset: 0x0019E57C
		public void Start()
		{
			PlayFabTitleDataCache.Instance.GetTitleData("BundleData", delegate(string data)
			{
				this.bundleList.FromJson(data);
			}, delegate(PlayFabError e)
			{
				Debug.LogError(string.Format("Error getting bundle data: {0}", e));
			});
			this.anchorOverrides = GorillaTagger.Instance.offlineVRRig.GetComponent<VRRigAnchorOverrides>();
		}

		// Token: 0x06005436 RID: 21558 RVA: 0x001A03D8 File Offset: 0x0019E5D8
		public void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
			if (SteamManager.Initialized && this._steamMicroTransactionAuthorizationResponse == null)
			{
				this._steamMicroTransactionAuthorizationResponse = Callback<MicroTxnAuthorizationResponse_t>.Create(new Callback<MicroTxnAuthorizationResponse_t>.DispatchDelegate(this.ProcessSteamCallback));
			}
		}

		// Token: 0x06005437 RID: 21559 RVA: 0x001A0407 File Offset: 0x0019E607
		public void OnDisable()
		{
			Callback<MicroTxnAuthorizationResponse_t> steamMicroTransactionAuthorizationResponse = this._steamMicroTransactionAuthorizationResponse;
			if (steamMicroTransactionAuthorizationResponse != null)
			{
				steamMicroTransactionAuthorizationResponse.Unregister();
			}
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06005438 RID: 21560 RVA: 0x000023F5 File Offset: 0x000005F5
		public void SliceUpdate()
		{
		}

		// Token: 0x06005439 RID: 21561 RVA: 0x001A0424 File Offset: 0x0019E624
		public static bool CompareCategoryToSavedCosmeticSlots(CosmeticsController.CosmeticCategory category, CosmeticsController.CosmeticSlots slot)
		{
			switch (category)
			{
			case CosmeticsController.CosmeticCategory.Hat:
				return slot == CosmeticsController.CosmeticSlots.Hat;
			case CosmeticsController.CosmeticCategory.Badge:
				return CosmeticsController.CosmeticSlots.Badge == slot;
			case CosmeticsController.CosmeticCategory.Face:
				return CosmeticsController.CosmeticSlots.Face == slot;
			case CosmeticsController.CosmeticCategory.Paw:
				return slot == CosmeticsController.CosmeticSlots.HandRight || slot == CosmeticsController.CosmeticSlots.HandLeft;
			case CosmeticsController.CosmeticCategory.Chest:
				return CosmeticsController.CosmeticSlots.Chest == slot;
			case CosmeticsController.CosmeticCategory.Fur:
				return CosmeticsController.CosmeticSlots.Fur == slot;
			case CosmeticsController.CosmeticCategory.Shirt:
				return CosmeticsController.CosmeticSlots.Shirt == slot;
			case CosmeticsController.CosmeticCategory.Back:
				return slot == CosmeticsController.CosmeticSlots.BackLeft || slot == CosmeticsController.CosmeticSlots.BackRight;
			case CosmeticsController.CosmeticCategory.Arms:
				return slot == CosmeticsController.CosmeticSlots.ArmLeft || slot == CosmeticsController.CosmeticSlots.ArmRight;
			case CosmeticsController.CosmeticCategory.Pants:
				return CosmeticsController.CosmeticSlots.Pants == slot;
			case CosmeticsController.CosmeticCategory.TagEffect:
				return CosmeticsController.CosmeticSlots.TagEffect == slot;
			default:
				return false;
			}
		}

		// Token: 0x0600543A RID: 21562 RVA: 0x001A04B8 File Offset: 0x0019E6B8
		public static CosmeticsController.CosmeticSlots CategoryToNonTransferrableSlot(CosmeticsController.CosmeticCategory category)
		{
			switch (category)
			{
			case CosmeticsController.CosmeticCategory.Hat:
				return CosmeticsController.CosmeticSlots.Hat;
			case CosmeticsController.CosmeticCategory.Badge:
				return CosmeticsController.CosmeticSlots.Badge;
			case CosmeticsController.CosmeticCategory.Face:
				return CosmeticsController.CosmeticSlots.Face;
			case CosmeticsController.CosmeticCategory.Paw:
				return CosmeticsController.CosmeticSlots.HandRight;
			case CosmeticsController.CosmeticCategory.Chest:
				return CosmeticsController.CosmeticSlots.Chest;
			case CosmeticsController.CosmeticCategory.Fur:
				return CosmeticsController.CosmeticSlots.Fur;
			case CosmeticsController.CosmeticCategory.Shirt:
				return CosmeticsController.CosmeticSlots.Shirt;
			case CosmeticsController.CosmeticCategory.Back:
				return CosmeticsController.CosmeticSlots.Back;
			case CosmeticsController.CosmeticCategory.Arms:
				return CosmeticsController.CosmeticSlots.Arms;
			case CosmeticsController.CosmeticCategory.Pants:
				return CosmeticsController.CosmeticSlots.Pants;
			case CosmeticsController.CosmeticCategory.TagEffect:
				return CosmeticsController.CosmeticSlots.TagEffect;
			default:
				return CosmeticsController.CosmeticSlots.Count;
			}
		}

		// Token: 0x0600543B RID: 21563 RVA: 0x001A051A File Offset: 0x0019E71A
		private CosmeticsController.CosmeticSlots DropPositionToCosmeticSlot(BodyDockPositions.DropPositions pos)
		{
			switch (pos)
			{
			case BodyDockPositions.DropPositions.LeftArm:
				return CosmeticsController.CosmeticSlots.ArmLeft;
			case BodyDockPositions.DropPositions.RightArm:
				return CosmeticsController.CosmeticSlots.ArmRight;
			case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm:
				break;
			case BodyDockPositions.DropPositions.Chest:
				return CosmeticsController.CosmeticSlots.Chest;
			default:
				if (pos == BodyDockPositions.DropPositions.LeftBack)
				{
					return CosmeticsController.CosmeticSlots.BackLeft;
				}
				if (pos == BodyDockPositions.DropPositions.RightBack)
				{
					return CosmeticsController.CosmeticSlots.BackRight;
				}
				break;
			}
			return CosmeticsController.CosmeticSlots.Count;
		}

		// Token: 0x0600543C RID: 21564 RVA: 0x001A054C File Offset: 0x0019E74C
		private static BodyDockPositions.DropPositions CosmeticSlotToDropPosition(CosmeticsController.CosmeticSlots slot)
		{
			switch (slot)
			{
			case CosmeticsController.CosmeticSlots.ArmLeft:
				return BodyDockPositions.DropPositions.LeftArm;
			case CosmeticsController.CosmeticSlots.ArmRight:
				return BodyDockPositions.DropPositions.RightArm;
			case CosmeticsController.CosmeticSlots.BackLeft:
				return BodyDockPositions.DropPositions.LeftBack;
			case CosmeticsController.CosmeticSlots.BackRight:
				return BodyDockPositions.DropPositions.RightBack;
			case CosmeticsController.CosmeticSlots.Chest:
				return BodyDockPositions.DropPositions.Chest;
			}
			return BodyDockPositions.DropPositions.None;
		}

		// Token: 0x0600543D RID: 21565 RVA: 0x001A0580 File Offset: 0x0019E780
		public void AddItemCheckout(ItemCheckout newItemCheckout)
		{
			if (this.itemCheckouts.Contains(newItemCheckout))
			{
				return;
			}
			this.itemCheckouts.Add(newItemCheckout);
			this.UpdateShoppingCart();
			this.FormattedPurchaseText(this.finalLine, this.leftCheckoutPurchaseButtonString, this.rightCheckoutPurchaseButtonString, this.leftCheckoutPurchaseButtonOn, this.rightCheckoutPurchaseButtonOn);
			if (!this.itemToBuy.isNullItem)
			{
				this.RefreshItemToBuyPreview();
			}
		}

		// Token: 0x0600543E RID: 21566 RVA: 0x001A05E5 File Offset: 0x0019E7E5
		public void RemoveItemCheckout(ItemCheckout checkoutToRemove)
		{
			this.itemCheckouts.Remove(checkoutToRemove);
		}

		// Token: 0x0600543F RID: 21567 RVA: 0x001A05F4 File Offset: 0x0019E7F4
		public void AddFittingRoom(FittingRoom newFittingRoom)
		{
			if (this.fittingRooms.Contains(newFittingRoom))
			{
				return;
			}
			this.fittingRooms.Add(newFittingRoom);
			this.UpdateShoppingCart();
		}

		// Token: 0x06005440 RID: 21568 RVA: 0x001A0617 File Offset: 0x0019E817
		public void RemoveFittingRoom(FittingRoom fittingRoomToRemove)
		{
			this.fittingRooms.Remove(fittingRoomToRemove);
		}

		// Token: 0x06005441 RID: 21569 RVA: 0x001A0626 File Offset: 0x0019E826
		private void SaveItemPreference(CosmeticsController.CosmeticSlots slot, int slotIdx, CosmeticsController.CosmeticItem newItem)
		{
			PlayerPrefs.SetString(CosmeticsController.CosmeticSet.SlotPlayerPreferenceName(slot), newItem.itemName);
			PlayerPrefs.Save();
		}

		// Token: 0x06005442 RID: 21570 RVA: 0x001A0640 File Offset: 0x0019E840
		public void SaveCurrentItemPreferences()
		{
			for (int i = 0; i < 16; i++)
			{
				CosmeticsController.CosmeticSlots slot = (CosmeticsController.CosmeticSlots)i;
				this.SaveItemPreference(slot, i, this.currentWornSet.items[i]);
			}
		}

		// Token: 0x06005443 RID: 21571 RVA: 0x001A0678 File Offset: 0x0019E878
		private void ApplyCosmeticToSet(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem newItem, int slotIdx, CosmeticsController.CosmeticSlots slot, bool applyToPlayerPrefs, List<CosmeticsController.CosmeticSlots> appliedSlots)
		{
			CosmeticsController.CosmeticItem cosmeticItem = (set.items[slotIdx].itemName == newItem.itemName) ? this.nullItem : newItem;
			set.items[slotIdx] = cosmeticItem;
			if (applyToPlayerPrefs)
			{
				this.SaveItemPreference(slot, slotIdx, cosmeticItem);
			}
			appliedSlots.Add(slot);
		}

		// Token: 0x06005444 RID: 21572 RVA: 0x001A06D4 File Offset: 0x0019E8D4
		private void PrivApplyCosmeticItemToSet(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem newItem, bool isLeftHand, bool applyToPlayerPrefs, List<CosmeticsController.CosmeticSlots> appliedSlots)
		{
			if (newItem.isNullItem)
			{
				return;
			}
			if (CosmeticsController.CosmeticSet.IsHoldable(newItem))
			{
				BodyDockPositions.DockingResult dockingResult = GorillaTagger.Instance.offlineVRRig.GetComponent<BodyDockPositions>().ToggleWithHandedness(newItem.displayName, isLeftHand, newItem.bothHandsHoldable);
				foreach (BodyDockPositions.DropPositions pos in dockingResult.positionsDisabled)
				{
					CosmeticsController.CosmeticSlots cosmeticSlots = this.DropPositionToCosmeticSlot(pos);
					if (cosmeticSlots != CosmeticsController.CosmeticSlots.Count)
					{
						int num = (int)cosmeticSlots;
						set.items[num] = this.nullItem;
						if (applyToPlayerPrefs)
						{
							this.SaveItemPreference(cosmeticSlots, num, this.nullItem);
						}
					}
				}
				using (List<BodyDockPositions.DropPositions>.Enumerator enumerator = dockingResult.dockedPosition.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BodyDockPositions.DropPositions dropPositions = enumerator.Current;
						if (dropPositions != BodyDockPositions.DropPositions.None)
						{
							CosmeticsController.CosmeticSlots cosmeticSlots2 = this.DropPositionToCosmeticSlot(dropPositions);
							int num2 = (int)cosmeticSlots2;
							set.items[num2] = newItem;
							if (applyToPlayerPrefs)
							{
								this.SaveItemPreference(cosmeticSlots2, num2, newItem);
							}
							appliedSlots.Add(cosmeticSlots2);
						}
					}
					return;
				}
			}
			if (newItem.itemCategory == CosmeticsController.CosmeticCategory.Paw)
			{
				CosmeticsController.CosmeticSlots cosmeticSlots3 = isLeftHand ? CosmeticsController.CosmeticSlots.HandLeft : CosmeticsController.CosmeticSlots.HandRight;
				int slotIdx = (int)cosmeticSlots3;
				this.ApplyCosmeticToSet(set, newItem, slotIdx, cosmeticSlots3, applyToPlayerPrefs, appliedSlots);
				CosmeticsController.CosmeticSlots cosmeticSlots4 = CosmeticsController.CosmeticSet.OppositeSlot(cosmeticSlots3);
				int num3 = (int)cosmeticSlots4;
				if (newItem.bothHandsHoldable)
				{
					this.ApplyCosmeticToSet(set, this.nullItem, num3, cosmeticSlots4, applyToPlayerPrefs, appliedSlots);
					return;
				}
				if (set.items[num3].itemName == newItem.itemName)
				{
					this.ApplyCosmeticToSet(set, this.nullItem, num3, cosmeticSlots4, applyToPlayerPrefs, appliedSlots);
				}
				if (set.items[num3].bothHandsHoldable)
				{
					this.ApplyCosmeticToSet(set, this.nullItem, num3, cosmeticSlots4, applyToPlayerPrefs, appliedSlots);
					return;
				}
			}
			else
			{
				CosmeticsController.CosmeticSlots cosmeticSlots5 = CosmeticsController.CategoryToNonTransferrableSlot(newItem.itemCategory);
				int slotIdx2 = (int)cosmeticSlots5;
				this.ApplyCosmeticToSet(set, newItem, slotIdx2, cosmeticSlots5, applyToPlayerPrefs, appliedSlots);
			}
		}

		// Token: 0x06005445 RID: 21573 RVA: 0x001A08D8 File Offset: 0x0019EAD8
		public void ApplyCosmeticItemToSet(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem newItem, bool isLeftHand, bool applyToPlayerPrefs)
		{
			this.ApplyCosmeticItemToSet(set, newItem, isLeftHand, applyToPlayerPrefs, CosmeticsController._g_default_outAppliedSlotsList_for_applyCosmeticItemToSet);
		}

		// Token: 0x06005446 RID: 21574 RVA: 0x001A08EC File Offset: 0x0019EAEC
		public void ApplyCosmeticItemToSet(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem newItem, bool isLeftHand, bool applyToPlayerPrefs, List<CosmeticsController.CosmeticSlots> outAppliedSlotsList)
		{
			outAppliedSlotsList.Clear();
			if (newItem.itemCategory == CosmeticsController.CosmeticCategory.Set)
			{
				bool flag = false;
				Dictionary<CosmeticsController.CosmeticItem, bool> dictionary = new Dictionary<CosmeticsController.CosmeticItem, bool>();
				foreach (string itemID in newItem.bundledItems)
				{
					CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(itemID);
					if (this.AnyMatch(set, itemFromDict))
					{
						flag = true;
						dictionary.Add(itemFromDict, true);
					}
					else
					{
						dictionary.Add(itemFromDict, false);
					}
				}
				using (Dictionary<CosmeticsController.CosmeticItem, bool>.Enumerator enumerator = dictionary.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<CosmeticsController.CosmeticItem, bool> keyValuePair = enumerator.Current;
						if (flag)
						{
							if (keyValuePair.Value)
							{
								this.PrivApplyCosmeticItemToSet(set, keyValuePair.Key, isLeftHand, applyToPlayerPrefs, outAppliedSlotsList);
							}
						}
						else
						{
							this.PrivApplyCosmeticItemToSet(set, keyValuePair.Key, isLeftHand, applyToPlayerPrefs, outAppliedSlotsList);
						}
					}
					return;
				}
			}
			this.PrivApplyCosmeticItemToSet(set, newItem, isLeftHand, applyToPlayerPrefs, outAppliedSlotsList);
		}

		// Token: 0x06005447 RID: 21575 RVA: 0x001A09D8 File Offset: 0x0019EBD8
		public void RemoveCosmeticItemFromSet(CosmeticsController.CosmeticSet set, string itemName, bool applyToPlayerPrefs)
		{
			this.cachedSet.CopyItems(set);
			for (int i = 0; i < 16; i++)
			{
				if (set.items[i].displayName == itemName)
				{
					set.items[i] = this.nullItem;
					if (applyToPlayerPrefs)
					{
						this.SaveItemPreference((CosmeticsController.CosmeticSlots)i, i, this.nullItem);
					}
				}
			}
			VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			BodyDockPositions component = offlineVRRig.GetComponent<BodyDockPositions>();
			set.ActivateCosmetics(this.cachedSet, offlineVRRig, component, offlineVRRig.cosmeticsObjectRegistry);
		}

		// Token: 0x06005448 RID: 21576 RVA: 0x001A0A60 File Offset: 0x0019EC60
		public void PressFittingRoomButton(FittingRoomButton pressedFittingRoomButton, bool isLeftHand)
		{
			BundleManager.instance._tryOnBundlesStand.ClearSelectedBundle();
			this.ApplyCosmeticItemToSet(this.tryOnSet, pressedFittingRoomButton.currentCosmeticItem, isLeftHand, false);
			this.UpdateShoppingCart();
			this.UpdateWornCosmetics(true);
		}

		// Token: 0x06005449 RID: 21577 RVA: 0x001A0A94 File Offset: 0x0019EC94
		public CosmeticsController.EWearingCosmeticSet CheckIfCosmeticSetMatchesItemSet(CosmeticsController.CosmeticSet set, string itemName)
		{
			CosmeticsController.EWearingCosmeticSet ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.NotASet;
			CosmeticsController.CosmeticItem cosmeticItem = this.allCosmeticsDict[itemName];
			if (cosmeticItem.bundledItems.Length != 0)
			{
				foreach (string key in cosmeticItem.bundledItems)
				{
					if (this.AnyMatch(set, this.allCosmeticsDict[key]))
					{
						if (ewearingCosmeticSet == CosmeticsController.EWearingCosmeticSet.NotASet)
						{
							ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.Complete;
						}
						else if (ewearingCosmeticSet == CosmeticsController.EWearingCosmeticSet.NotWearing)
						{
							ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.Partial;
						}
					}
					else if (ewearingCosmeticSet == CosmeticsController.EWearingCosmeticSet.NotASet)
					{
						ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.NotWearing;
					}
					else if (ewearingCosmeticSet == CosmeticsController.EWearingCosmeticSet.Complete)
					{
						ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.Partial;
					}
				}
			}
			return ewearingCosmeticSet;
		}

		// Token: 0x0600544A RID: 21578 RVA: 0x001A0B08 File Offset: 0x0019ED08
		public void PressCosmeticStandButton(CosmeticStand pressedStand)
		{
			this.searchIndex = this.currentCart.IndexOf(pressedStand.thisCosmeticItem);
			if (this.searchIndex != -1)
			{
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.cart_item_remove, pressedStand.thisCosmeticItem);
				this.currentCart.RemoveAt(this.searchIndex);
				pressedStand.isOn = false;
				for (int i = 0; i < 16; i++)
				{
					if (pressedStand.thisCosmeticItem.itemName == this.tryOnSet.items[i].itemName)
					{
						this.tryOnSet.items[i] = this.nullItem;
					}
				}
			}
			else
			{
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.cart_item_add, pressedStand.thisCosmeticItem);
				this.currentCart.Insert(0, pressedStand.thisCosmeticItem);
				pressedStand.isOn = true;
				if (this.currentCart.Count > this.numFittingRoomButtons)
				{
					foreach (CosmeticStand cosmeticStand in this.cosmeticStands)
					{
						if (!(cosmeticStand == null) && cosmeticStand.thisCosmeticItem.itemName == this.currentCart[this.numFittingRoomButtons].itemName)
						{
							cosmeticStand.isOn = false;
							cosmeticStand.UpdateColor();
							break;
						}
					}
					this.currentCart.RemoveAt(this.numFittingRoomButtons);
				}
			}
			pressedStand.UpdateColor();
			this.UpdateShoppingCart();
		}

		// Token: 0x0600544B RID: 21579 RVA: 0x001A0C6C File Offset: 0x0019EE6C
		public void PressWardrobeItemButton(CosmeticsController.CosmeticItem cosmeticItem, bool isLeftHand)
		{
			if (cosmeticItem.isNullItem)
			{
				return;
			}
			CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(cosmeticItem.itemName);
			List<CosmeticsController.CosmeticSlots> list = CollectionPool<List<CosmeticsController.CosmeticSlots>, CosmeticsController.CosmeticSlots>.Get();
			if (list.Capacity < 16)
			{
				list.Capacity = 16;
			}
			this.ApplyCosmeticItemToSet(this.currentWornSet, itemFromDict, isLeftHand, true, list);
			foreach (CosmeticsController.CosmeticSlots cosmeticSlots in list)
			{
				this.tryOnSet.items[(int)cosmeticSlots] = this.nullItem;
			}
			CollectionPool<List<CosmeticsController.CosmeticSlots>, CosmeticsController.CosmeticSlots>.Release(list);
			this.UpdateShoppingCart();
			this.UpdateWornCosmetics(true);
			Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
			if (onCosmeticsUpdated == null)
			{
				return;
			}
			onCosmeticsUpdated();
		}

		// Token: 0x0600544C RID: 21580 RVA: 0x001A0D2C File Offset: 0x0019EF2C
		public void PressWardrobeFunctionButton(string function)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(function);
			if (num <= 2554875734U)
			{
				if (num <= 895779448U)
				{
					if (num != 292255708U)
					{
						if (num != 306900080U)
						{
							if (num == 895779448U)
							{
								if (function == "badge")
								{
									if (this.wardrobeType == 2)
									{
										return;
									}
									this.wardrobeType = 2;
								}
							}
						}
						else if (function == "left")
						{
							this.cosmeticsPages[this.wardrobeType] = this.cosmeticsPages[this.wardrobeType] - 1;
							if (this.cosmeticsPages[this.wardrobeType] < 0)
							{
								this.cosmeticsPages[this.wardrobeType] = (this.itemLists[this.wardrobeType].Count - 1) / 3;
							}
						}
					}
					else if (function == "face")
					{
						if (this.wardrobeType == 1)
						{
							return;
						}
						this.wardrobeType = 1;
					}
				}
				else if (num != 1538531746U)
				{
					if (num != 2028154341U)
					{
						if (num == 2554875734U)
						{
							if (function == "chest")
							{
								if (this.wardrobeType == 8)
								{
									return;
								}
								this.wardrobeType = 8;
							}
						}
					}
					else if (function == "right")
					{
						this.cosmeticsPages[this.wardrobeType] = this.cosmeticsPages[this.wardrobeType] + 1;
						if (this.cosmeticsPages[this.wardrobeType] > (this.itemLists[this.wardrobeType].Count - 1) / 3)
						{
							this.cosmeticsPages[this.wardrobeType] = 0;
						}
					}
				}
				else if (function == "back")
				{
					if (this.wardrobeType == 7)
					{
						return;
					}
					this.wardrobeType = 7;
				}
			}
			else if (num <= 3034286914U)
			{
				if (num != 2633735346U)
				{
					if (num != 2953262278U)
					{
						if (num == 3034286914U)
						{
							if (function == "fur")
							{
								if (this.wardrobeType == 4)
								{
									return;
								}
								this.wardrobeType = 4;
							}
						}
					}
					else if (function == "outfit")
					{
						if (this.wardrobeType == 5)
						{
							return;
						}
						this.wardrobeType = 5;
					}
				}
				else if (function == "arms")
				{
					if (this.wardrobeType == 6)
					{
						return;
					}
					this.wardrobeType = 6;
				}
			}
			else if (num <= 3300536096U)
			{
				if (num != 3081164502U)
				{
					if (num == 3300536096U)
					{
						if (function == "hand")
						{
							if (this.wardrobeType == 3)
							{
								return;
							}
							this.wardrobeType = 3;
						}
					}
				}
				else if (function == "tagEffect")
				{
					if (this.wardrobeType == 10)
					{
						return;
					}
					this.wardrobeType = 10;
				}
			}
			else if (num != 3568683773U)
			{
				if (num == 4072609730U)
				{
					if (function == "hat")
					{
						if (this.wardrobeType == 0)
						{
							return;
						}
						this.wardrobeType = 0;
					}
				}
			}
			else if (function == "reserved")
			{
				if (this.wardrobeType == 9)
				{
					return;
				}
				this.wardrobeType = 9;
			}
			this.UpdateWardrobeModelsAndButtons();
			Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
			if (onCosmeticsUpdated == null)
			{
				return;
			}
			onCosmeticsUpdated();
		}

		// Token: 0x0600544D RID: 21581 RVA: 0x001A10B8 File Offset: 0x0019F2B8
		public void ClearCheckout(bool sendEvent)
		{
			if (sendEvent)
			{
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.checkout_cancel, this.currentCart);
			}
			this.itemToBuy = this.nullItem;
			this.RefreshItemToBuyPreview();
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
			this.ProcessPurchaseItemState(null, false);
		}

		// Token: 0x0600544E RID: 21582 RVA: 0x001A10F4 File Offset: 0x0019F2F4
		public bool RemoveItemFromCart(CosmeticsController.CosmeticItem cosmeticItem)
		{
			this.searchIndex = this.currentCart.IndexOf(cosmeticItem);
			if (this.searchIndex != -1)
			{
				this.currentCart.RemoveAt(this.searchIndex);
				for (int i = 0; i < 16; i++)
				{
					if (cosmeticItem.itemName == this.tryOnSet.items[i].itemName)
					{
						this.tryOnSet.items[i] = this.nullItem;
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x0600544F RID: 21583 RVA: 0x001A1177 File Offset: 0x0019F377
		public void ClearCheckoutAndCart(bool sendEvent)
		{
			this.currentCart.Clear();
			this.tryOnSet.ClearSet(this.nullItem);
			this.ClearCheckout(sendEvent);
		}

		// Token: 0x06005450 RID: 21584 RVA: 0x001A119C File Offset: 0x0019F39C
		public void PressCheckoutCartButton(CheckoutCartButton pressedCheckoutCartButton, bool isLeftHand)
		{
			if (this.currentPurchaseItemStage != CosmeticsController.PurchaseItemStages.Buying)
			{
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
				this.tryOnSet.ClearSet(this.nullItem);
				if (this.itemToBuy.displayName == pressedCheckoutCartButton.currentCosmeticItem.displayName)
				{
					this.itemToBuy = this.nullItem;
					this.RefreshItemToBuyPreview();
				}
				else
				{
					this.itemToBuy = pressedCheckoutCartButton.currentCosmeticItem;
					this.checkoutCartButtonPressedWithLeft = isLeftHand;
					this.RefreshItemToBuyPreview();
				}
				this.ProcessPurchaseItemState(null, isLeftHand);
				this.UpdateShoppingCart();
			}
		}

		// Token: 0x06005451 RID: 21585 RVA: 0x001A1224 File Offset: 0x0019F424
		private void RefreshItemToBuyPreview()
		{
			if (this.itemToBuy.bundledItems != null && this.itemToBuy.bundledItems.Length != 0)
			{
				List<string> list = new List<string>();
				foreach (string itemID in this.itemToBuy.bundledItems)
				{
					this.tempItem = this.GetItemFromDict(itemID);
					list.Add(this.tempItem.displayName);
				}
				this.iterator = 0;
				while (this.iterator < this.itemCheckouts.Count)
				{
					if (!this.itemCheckouts[this.iterator].IsNull())
					{
						this.itemCheckouts[this.iterator].checkoutHeadModel.SetCosmeticActiveArray(list.ToArray(), new bool[list.Count]);
					}
					this.iterator++;
				}
			}
			else
			{
				this.iterator = 0;
				while (this.iterator < this.itemCheckouts.Count)
				{
					if (!this.itemCheckouts[this.iterator].IsNull())
					{
						this.itemCheckouts[this.iterator].checkoutHeadModel.SetCosmeticActive(this.itemToBuy.displayName, false);
					}
					this.iterator++;
				}
			}
			this.ApplyCosmeticItemToSet(this.tryOnSet, this.itemToBuy, this.checkoutCartButtonPressedWithLeft, false);
			this.UpdateWornCosmetics(true);
		}

		// Token: 0x06005452 RID: 21586 RVA: 0x001A138D File Offset: 0x0019F58D
		public void PressPurchaseItemButton(PurchaseItemButton pressedPurchaseItemButton, bool isLeftHand)
		{
			this.ProcessPurchaseItemState(pressedPurchaseItemButton.buttonSide, isLeftHand);
		}

		// Token: 0x06005453 RID: 21587 RVA: 0x001A139C File Offset: 0x0019F59C
		public void PurchaseBundle(StoreBundle bundleToPurchase)
		{
			if (bundleToPurchase.playfabBundleID != "NULL")
			{
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
				this.ProcessPurchaseItemState("left", false);
				this.buyingBundle = true;
				this.itemToPurchase = bundleToPurchase.playfabBundleID;
				this.SteamPurchase();
			}
		}

		// Token: 0x06005454 RID: 21588 RVA: 0x001A13DC File Offset: 0x0019F5DC
		public void PressEarlyAccessButton()
		{
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
			this.ProcessPurchaseItemState("left", false);
			this.buyingBundle = true;
			this.itemToPurchase = this.BundlePlayfabItemName;
			ATM_Manager.instance.shinyRocksCost = (float)this.BundleShinyRocks;
			this.SteamPurchase();
		}

		// Token: 0x06005455 RID: 21589 RVA: 0x001A1428 File Offset: 0x0019F628
		public void PressPurchaseBundleButton(string PlayFabItemName)
		{
			BundleManager.instance.BundlePurchaseButtonPressed(PlayFabItemName);
		}

		// Token: 0x06005456 RID: 21590 RVA: 0x001A1438 File Offset: 0x0019F638
		public void ProcessPurchaseItemState(string buttonSide, bool isLeftHand)
		{
			switch (this.currentPurchaseItemStage)
			{
			case CosmeticsController.PurchaseItemStages.Start:
				this.itemToBuy = this.nullItem;
				this.FormattedPurchaseText("SELECT AN ITEM FROM YOUR CART TO PURCHASE!", null, null, false, false);
				this.UpdateShoppingCart();
				return;
			case CosmeticsController.PurchaseItemStages.CheckoutButtonPressed:
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.checkout_start, this.currentCart);
				this.searchIndex = this.unlockedCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => this.itemToBuy.itemName == x.itemName);
				if (this.searchIndex > -1)
				{
					this.FormattedPurchaseText("YOU ALREADY OWN THIS ITEM!", "-", "-", true, true);
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.ItemOwned;
					return;
				}
				if (this.itemToBuy.cost <= this.currencyBalance)
				{
					this.FormattedPurchaseText("DO YOU WANT TO BUY THIS ITEM?", "NO!", "YES!", false, false);
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.ItemSelected;
					return;
				}
				this.FormattedPurchaseText("INSUFFICIENT SHINY ROCKS FOR THIS ITEM!", "-", "-", true, true);
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
				return;
			case CosmeticsController.PurchaseItemStages.ItemSelected:
				if (buttonSide == "right")
				{
					GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.item_select, this.itemToBuy);
					this.FormattedPurchaseText("ARE YOU REALLY SURE?", "YES! I NEED IT!", "LET ME THINK ABOUT IT", false, false);
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.FinalPurchaseAcknowledgement;
					return;
				}
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
				this.ProcessPurchaseItemState(null, isLeftHand);
				return;
			case CosmeticsController.PurchaseItemStages.ItemOwned:
			case CosmeticsController.PurchaseItemStages.Buying:
				break;
			case CosmeticsController.PurchaseItemStages.FinalPurchaseAcknowledgement:
				if (buttonSide == "left")
				{
					this.FormattedPurchaseText("PURCHASING ITEM...", "-", "-", true, true);
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Buying;
					this.isLastHandTouchedLeft = isLeftHand;
					this.PurchaseItem();
					return;
				}
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
				this.ProcessPurchaseItemState(null, isLeftHand);
				return;
			case CosmeticsController.PurchaseItemStages.Success:
			{
				this.FormattedPurchaseText("SUCCESS! ENJOY YOUR NEW ITEM!", "-", "-", true, true);
				VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
				offlineVRRig.concatStringOfCosmeticsAllowed += this.itemToBuy.itemName;
				CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(this.itemToBuy.itemName);
				if (itemFromDict.bundledItems != null)
				{
					foreach (string str in itemFromDict.bundledItems)
					{
						VRRig offlineVRRig2 = GorillaTagger.Instance.offlineVRRig;
						offlineVRRig2.concatStringOfCosmeticsAllowed += str;
					}
				}
				this.tryOnSet.ClearSet(this.nullItem);
				this.UpdateShoppingCart();
				this.ApplyCosmeticItemToSet(this.currentWornSet, itemFromDict, isLeftHand, true);
				this.UpdateShoppingCart();
				this.UpdateWornCosmetics(false);
				this.UpdateWardrobeModelsAndButtons();
				Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
				if (onCosmeticsUpdated == null)
				{
					return;
				}
				onCosmeticsUpdated();
				break;
			}
			case CosmeticsController.PurchaseItemStages.Failure:
				this.FormattedPurchaseText("ERROR IN PURCHASING ITEM! NO MONEY WAS SPENT. SELECT ANOTHER ITEM.", "-", "-", true, true);
				return;
			default:
				return;
			}
		}

		// Token: 0x06005457 RID: 21591 RVA: 0x001A16CC File Offset: 0x0019F8CC
		public void FormattedPurchaseText(string finalLineVar, string leftPurchaseButtonText = null, string rightPurchaseButtonText = null, bool leftButtonOn = false, bool rightButtonOn = false)
		{
			this.finalLine = finalLineVar;
			if (leftPurchaseButtonText != null)
			{
				this.leftCheckoutPurchaseButtonString = leftPurchaseButtonText;
				this.leftCheckoutPurchaseButtonOn = leftButtonOn;
			}
			if (rightPurchaseButtonText != null)
			{
				this.rightCheckoutPurchaseButtonString = rightPurchaseButtonText;
				this.rightCheckoutPurchaseButtonOn = rightButtonOn;
			}
			string newText = string.Concat(new string[]
			{
				"SELECTION: ",
				this.GetItemDisplayName(this.itemToBuy),
				"\nITEM COST: ",
				this.itemToBuy.cost.ToString(),
				"\nYOU HAVE: ",
				this.currencyBalance.ToString(),
				"\n\n",
				this.finalLine
			});
			this.iterator = 0;
			while (this.iterator < this.itemCheckouts.Count)
			{
				if (!this.itemCheckouts[this.iterator].IsNull())
				{
					this.itemCheckouts[this.iterator].UpdatePurchaseText(newText, leftPurchaseButtonText, rightPurchaseButtonText, leftButtonOn, rightButtonOn);
				}
				this.iterator++;
			}
		}

		// Token: 0x06005458 RID: 21592 RVA: 0x001A17C8 File Offset: 0x0019F9C8
		public void PurchaseItem()
		{
			PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
			{
				ItemId = this.itemToBuy.itemName,
				Price = this.itemToBuy.cost,
				VirtualCurrency = this.currencyName,
				CatalogVersion = this.catalog
			}, delegate(PurchaseItemResult result)
			{
				if (result.Items.Count > 0)
				{
					foreach (ItemInstance itemInstance in result.Items)
					{
						CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(this.itemToBuy.itemName);
						if (itemFromDict.itemCategory == CosmeticsController.CosmeticCategory.Set)
						{
							this.UnlockItem(itemInstance.ItemId, false);
							foreach (string itemIdToUnlock in itemFromDict.bundledItems)
							{
								this.UnlockItem(itemIdToUnlock, false);
							}
						}
						else
						{
							this.UnlockItem(itemInstance.ItemId, false);
						}
					}
					this.UpdateMyCosmetics();
					if (NetworkSystem.Instance.InRoom)
					{
						base.StartCoroutine(this.CheckIfMyCosmeticsUpdated(this.itemToBuy.itemName));
					}
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Success;
					this.currencyBalance -= this.itemToBuy.cost;
					this.UpdateShoppingCart();
					this.ProcessPurchaseItemState(null, this.isLastHandTouchedLeft);
					return;
				}
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Failure;
				this.ProcessPurchaseItemState(null, false);
			}, delegate(PlayFabError error)
			{
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Failure;
				this.ProcessPurchaseItemState(null, false);
			}, null, null);
		}

		// Token: 0x06005459 RID: 21593 RVA: 0x001A1834 File Offset: 0x0019FA34
		private void UnlockItem(string itemIdToUnlock, bool relock = false)
		{
			int num = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => itemIdToUnlock == x.itemName);
			if (num > -1)
			{
				this.ModifyUnlockList(this.unlockedCosmetics, num, relock);
				if (relock)
				{
					this.concatStringCosmeticsAllowed.Replace(this.allCosmetics[num].itemName, string.Empty);
				}
				else
				{
					this.concatStringCosmeticsAllowed += this.allCosmetics[num].itemName;
				}
				switch (this.allCosmetics[num].itemCategory)
				{
				case CosmeticsController.CosmeticCategory.Hat:
					this.ModifyUnlockList(this.unlockedHats, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Badge:
					this.ModifyUnlockList(this.unlockedBadges, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Face:
					this.ModifyUnlockList(this.unlockedFaces, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Paw:
					if (!this.allCosmetics[num].isThrowable)
					{
						this.ModifyUnlockList(this.unlockedPaws, num, relock);
						return;
					}
					this.ModifyUnlockList(this.unlockedThrowables, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Chest:
					this.ModifyUnlockList(this.unlockedChests, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Fur:
					this.ModifyUnlockList(this.unlockedFurs, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Shirt:
					this.ModifyUnlockList(this.unlockedShirts, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Back:
					this.ModifyUnlockList(this.unlockedBacks, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Arms:
					this.ModifyUnlockList(this.unlockedArms, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Pants:
					this.ModifyUnlockList(this.unlockedPants, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.TagEffect:
					this.ModifyUnlockList(this.unlockedTagFX, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Count:
					break;
				case CosmeticsController.CosmeticCategory.Set:
					foreach (string itemIdToUnlock2 in this.allCosmetics[num].bundledItems)
					{
						this.UnlockItem(itemIdToUnlock2, false);
					}
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x0600545A RID: 21594 RVA: 0x001A1A10 File Offset: 0x0019FC10
		private void ModifyUnlockList(List<CosmeticsController.CosmeticItem> list, int index, bool relock)
		{
			if (!relock && !list.Contains(this.allCosmetics[index]))
			{
				list.Add(this.allCosmetics[index]);
				return;
			}
			if (relock && list.Contains(this.allCosmetics[index]))
			{
				list.Remove(this.allCosmetics[index]);
			}
		}

		// Token: 0x0600545B RID: 21595 RVA: 0x001A1A71 File Offset: 0x0019FC71
		private IEnumerator CheckIfMyCosmeticsUpdated(string itemToBuyID)
		{
			Debug.Log("Cosmetic updated check!");
			yield return new WaitForSeconds(1f);
			this.foundCosmetic = false;
			this.attempts = 0;
			while (!this.foundCosmetic && this.attempts < 10 && NetworkSystem.Instance.InRoom)
			{
				this.playerIDList.Clear();
				if (this.UseNewCosmeticsPath())
				{
					this.playerIDList.Add("Inventory");
					PlayFabClientAPI.GetSharedGroupData(new PlayFab.ClientModels.GetSharedGroupDataRequest
					{
						Keys = this.playerIDList,
						SharedGroupId = NetworkSystem.Instance.LocalPlayer.UserId + "Inventory"
					}, delegate(GetSharedGroupDataResult result)
					{
						this.attempts++;
						foreach (KeyValuePair<string, PlayFab.ClientModels.SharedGroupDataRecord> keyValuePair in result.Data)
						{
							if (keyValuePair.Value.Value.Contains(itemToBuyID))
							{
								PhotonNetwork.RaiseEvent(199, null, new RaiseEventOptions
								{
									Receivers = ReceiverGroup.Others
								}, SendOptions.SendReliable);
								this.foundCosmetic = true;
							}
						}
						if (this.foundCosmetic)
						{
							this.UpdateWornCosmetics(true);
						}
					}, delegate(PlayFabError error)
					{
						this.attempts++;
						this.ReauthOrBan(error);
					}, null, null);
					yield return new WaitForSeconds(1f);
				}
				else
				{
					this.playerIDList.Add(PhotonNetwork.LocalPlayer.ActorNumber.ToString());
					PlayFabClientAPI.GetSharedGroupData(new PlayFab.ClientModels.GetSharedGroupDataRequest
					{
						Keys = this.playerIDList,
						SharedGroupId = NetworkSystem.Instance.RoomName + Regex.Replace(NetworkSystem.Instance.CurrentRegion, "[^a-zA-Z0-9]", "").ToUpper()
					}, delegate(GetSharedGroupDataResult result)
					{
						this.attempts++;
						foreach (KeyValuePair<string, PlayFab.ClientModels.SharedGroupDataRecord> keyValuePair in result.Data)
						{
							if (keyValuePair.Value.Value.Contains(itemToBuyID))
							{
								NetworkSystemRaiseEvent.RaiseEvent(199, null, NetworkSystemRaiseEvent.neoOthers, true);
								this.foundCosmetic = true;
							}
							else
							{
								Debug.Log("didnt find it, updating attempts and trying again in a bit. current attempt is " + this.attempts.ToString());
							}
						}
						if (this.foundCosmetic)
						{
							this.UpdateWornCosmetics(true);
						}
					}, delegate(PlayFabError error)
					{
						this.attempts++;
						if (error.Error == PlayFabErrorCode.NotAuthenticated)
						{
							PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
						}
						else if (error.Error == PlayFabErrorCode.AccountBanned)
						{
							GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
						}
						Debug.Log("Got error retrieving user data, on attempt " + this.attempts.ToString());
						Debug.Log(error.GenerateErrorReport());
					}, null, null);
					yield return new WaitForSeconds(1f);
				}
			}
			Debug.Log("done!");
			yield break;
		}

		// Token: 0x0600545C RID: 21596 RVA: 0x001A1A88 File Offset: 0x0019FC88
		public void UpdateWardrobeModelsAndButtons()
		{
			if (!CosmeticsV2Spawner_Dirty.allPartsInstantiated)
			{
				return;
			}
			foreach (WardrobeInstance wardrobeInstance in this.wardrobes)
			{
				wardrobeInstance.wardrobeItemButtons[0].currentCosmeticItem = ((this.cosmeticsPages[this.wardrobeType] * 3 < this.itemLists[this.wardrobeType].Count) ? this.itemLists[this.wardrobeType][this.cosmeticsPages[this.wardrobeType] * 3] : this.nullItem);
				wardrobeInstance.wardrobeItemButtons[1].currentCosmeticItem = ((this.cosmeticsPages[this.wardrobeType] * 3 + 1 < this.itemLists[this.wardrobeType].Count) ? this.itemLists[this.wardrobeType][this.cosmeticsPages[this.wardrobeType] * 3 + 1] : this.nullItem);
				wardrobeInstance.wardrobeItemButtons[2].currentCosmeticItem = ((this.cosmeticsPages[this.wardrobeType] * 3 + 2 < this.itemLists[this.wardrobeType].Count) ? this.itemLists[this.wardrobeType][this.cosmeticsPages[this.wardrobeType] * 3 + 2] : this.nullItem);
				this.iterator = 0;
				while (this.iterator < wardrobeInstance.wardrobeItemButtons.Length)
				{
					CosmeticsController.CosmeticItem currentCosmeticItem = wardrobeInstance.wardrobeItemButtons[this.iterator].currentCosmeticItem;
					wardrobeInstance.wardrobeItemButtons[this.iterator].isOn = (!currentCosmeticItem.isNullItem && this.AnyMatch(this.currentWornSet, currentCosmeticItem));
					wardrobeInstance.wardrobeItemButtons[this.iterator].UpdateColor();
					this.iterator++;
				}
				wardrobeInstance.wardrobeItemButtons[0].controlledModel.SetCosmeticActive(wardrobeInstance.wardrobeItemButtons[0].currentCosmeticItem.displayName, false);
				wardrobeInstance.wardrobeItemButtons[1].controlledModel.SetCosmeticActive(wardrobeInstance.wardrobeItemButtons[1].currentCosmeticItem.displayName, false);
				wardrobeInstance.wardrobeItemButtons[2].controlledModel.SetCosmeticActive(wardrobeInstance.wardrobeItemButtons[2].currentCosmeticItem.displayName, false);
				wardrobeInstance.selfDoll.SetCosmeticActiveArray(this.currentWornSet.ToDisplayNameArray(), this.currentWornSet.ToOnRightSideArray());
			}
		}

		// Token: 0x0600545D RID: 21597 RVA: 0x001A1D08 File Offset: 0x0019FF08
		public int GetCategorySize(CosmeticsController.CosmeticCategory category)
		{
			int indexForCategory = this.GetIndexForCategory(category);
			if (indexForCategory != -1)
			{
				return this.itemLists[indexForCategory].Count;
			}
			return 0;
		}

		// Token: 0x0600545E RID: 21598 RVA: 0x001A1D30 File Offset: 0x0019FF30
		public CosmeticsController.CosmeticItem GetCosmetic(int category, int cosmeticIndex)
		{
			if (cosmeticIndex >= this.itemLists[category].Count || cosmeticIndex < 0)
			{
				return this.nullItem;
			}
			return this.itemLists[category][cosmeticIndex];
		}

		// Token: 0x0600545F RID: 21599 RVA: 0x001A1D5B File Offset: 0x0019FF5B
		public CosmeticsController.CosmeticItem GetCosmetic(CosmeticsController.CosmeticCategory category, int cosmeticIndex)
		{
			return this.GetCosmetic(this.GetIndexForCategory(category), cosmeticIndex);
		}

		// Token: 0x06005460 RID: 21600 RVA: 0x001A1D6C File Offset: 0x0019FF6C
		private int GetIndexForCategory(CosmeticsController.CosmeticCategory category)
		{
			switch (category)
			{
			case CosmeticsController.CosmeticCategory.Hat:
				return 0;
			case CosmeticsController.CosmeticCategory.Badge:
				return 2;
			case CosmeticsController.CosmeticCategory.Face:
				return 1;
			case CosmeticsController.CosmeticCategory.Paw:
				return 3;
			case CosmeticsController.CosmeticCategory.Chest:
				return 9;
			case CosmeticsController.CosmeticCategory.Fur:
				return 4;
			case CosmeticsController.CosmeticCategory.Shirt:
				return 5;
			case CosmeticsController.CosmeticCategory.Back:
				return 8;
			case CosmeticsController.CosmeticCategory.Arms:
				return 7;
			case CosmeticsController.CosmeticCategory.Pants:
				return 6;
			case CosmeticsController.CosmeticCategory.TagEffect:
				return 10;
			default:
				return 0;
			}
		}

		// Token: 0x06005461 RID: 21601 RVA: 0x001A1DC8 File Offset: 0x0019FFC8
		public bool IsCosmeticEquipped(CosmeticsController.CosmeticItem cosmetic)
		{
			return this.AnyMatch(this.currentWornSet, cosmetic);
		}

		// Token: 0x06005462 RID: 21602 RVA: 0x001A1DD8 File Offset: 0x0019FFD8
		public CosmeticsController.CosmeticItem GetSlotItem(CosmeticsController.CosmeticSlots slot, bool checkOpposite = true)
		{
			int num = (int)slot;
			if (checkOpposite)
			{
				num = (int)CosmeticsController.CosmeticSet.OppositeSlot(slot);
			}
			return this.currentWornSet.items[num];
		}

		// Token: 0x06005463 RID: 21603 RVA: 0x001A1E02 File Offset: 0x001A0002
		public string[] GetCurrentlyWornCosmetics()
		{
			return this.currentWornSet.ToDisplayNameArray();
		}

		// Token: 0x06005464 RID: 21604 RVA: 0x001A1E0F File Offset: 0x001A000F
		public bool[] GetCurrentRightEquippedSided()
		{
			return this.currentWornSet.ToOnRightSideArray();
		}

		// Token: 0x06005465 RID: 21605 RVA: 0x001A1E1C File Offset: 0x001A001C
		public void UpdateShoppingCart()
		{
			this.iterator = 0;
			while (this.iterator < this.itemCheckouts.Count)
			{
				if (!this.itemCheckouts[this.iterator].IsNull())
				{
					this.itemCheckouts[this.iterator].UpdateFromCart(this.currentCart, this.itemToBuy);
				}
				this.iterator++;
			}
			this.iterator = 0;
			while (this.iterator < this.fittingRooms.Count)
			{
				if (!this.fittingRooms[this.iterator].IsNull())
				{
					this.fittingRooms[this.iterator].UpdateFromCart(this.currentCart, this.tryOnSet);
				}
				this.iterator++;
			}
			if (CosmeticsV2Spawner_Dirty.allPartsInstantiated)
			{
				this.UpdateWardrobeModelsAndButtons();
			}
		}

		// Token: 0x06005466 RID: 21606 RVA: 0x001A1F00 File Offset: 0x001A0100
		public void UpdateWornCosmetics(bool sync = false)
		{
			GorillaTagger.Instance.offlineVRRig.LocalUpdateCosmeticsWithTryon(this.currentWornSet, this.tryOnSet);
			if (sync && GorillaTagger.Instance.myVRRig != null)
			{
				if (this.isHidingCosmeticsFromRemotePlayers)
				{
					GorillaTagger.Instance.myVRRig.SendRPC("RPC_HideAllCosmetics", RpcTarget.All, Array.Empty<object>());
					return;
				}
				int[] array = this.currentWornSet.ToPackedIDArray();
				int[] array2 = this.tryOnSet.ToPackedIDArray();
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.Others, new object[]
				{
					array,
					array2
				});
			}
		}

		// Token: 0x06005467 RID: 21607 RVA: 0x001A1F9B File Offset: 0x001A019B
		public CosmeticsController.CosmeticItem GetItemFromDict(string itemID)
		{
			if (!this.allCosmeticsDict.TryGetValue(itemID, out this.cosmeticItemVar))
			{
				return this.nullItem;
			}
			return this.cosmeticItemVar;
		}

		// Token: 0x06005468 RID: 21608 RVA: 0x001A1FBE File Offset: 0x001A01BE
		public string GetItemNameFromDisplayName(string displayName)
		{
			if (!this.allCosmeticsItemIDsfromDisplayNamesDict.TryGetValue(displayName, out this.returnString))
			{
				return "null";
			}
			return this.returnString;
		}

		// Token: 0x06005469 RID: 21609 RVA: 0x001A1FE0 File Offset: 0x001A01E0
		public CosmeticAnchorAntiIntersectOffsets GetClipOffsetsFromDisplayName(string displayName)
		{
			string itemNameFromDisplayName = this.GetItemNameFromDisplayName(displayName);
			if (itemNameFromDisplayName.Equals("null"))
			{
				return this.defaultClipOffsets;
			}
			AllCosmeticsArraySO allCosmeticsArraySO = this.v2_allCosmeticsInfoAssetRef.Asset as AllCosmeticsArraySO;
			if (allCosmeticsArraySO == null)
			{
				GTDev.LogWarning<string>("null AllCosmeticsArraySO", null);
				return this.defaultClipOffsets;
			}
			CosmeticSO cosmeticSO = allCosmeticsArraySO.SearchForCosmeticSO(itemNameFromDisplayName);
			if (cosmeticSO != null)
			{
				return cosmeticSO.info.anchorAntiIntersectOffsets;
			}
			GTDev.Log<string>("Could not find cosmetic info for " + itemNameFromDisplayName, null);
			return this.defaultClipOffsets;
		}

		// Token: 0x0600546A RID: 21610 RVA: 0x001A206C File Offset: 0x001A026C
		public bool AnyMatch(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem item)
		{
			if (item.itemCategory != CosmeticsController.CosmeticCategory.Set)
			{
				return set.IsActive(item.displayName);
			}
			if (item.bundledItems.Length == 1)
			{
				return this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[0]));
			}
			if (item.bundledItems.Length == 2)
			{
				return this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[0])) || this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[1]));
			}
			return item.bundledItems.Length >= 3 && (this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[0])) || this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[1])) || this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[2])));
		}

		// Token: 0x0600546B RID: 21611 RVA: 0x001A2140 File Offset: 0x001A0340
		public void Initialize()
		{
			if (!base.gameObject.activeSelf || this.v2_isCosmeticPlayFabCatalogDataLoaded || this.v2_isGetCosmeticsPlayCatalogDataWaitingForCallback)
			{
				return;
			}
			if (this.v2_allCosmeticsInfoAssetRef_isLoaded)
			{
				this.GetCosmeticsPlayFabCatalogData();
				return;
			}
			this.v2_isGetCosmeticsPlayCatalogDataWaitingForCallback = true;
			this.V2_allCosmeticsInfoAssetRef_OnPostLoad = (Action)Delegate.Combine(this.V2_allCosmeticsInfoAssetRef_OnPostLoad, new Action(this.GetCosmeticsPlayFabCatalogData));
		}

		// Token: 0x0600546C RID: 21612 RVA: 0x001A21A3 File Offset: 0x001A03A3
		public void GetLastDailyLogin()
		{
			PlayFabClientAPI.GetUserReadOnlyData(new PlayFab.ClientModels.GetUserDataRequest(), delegate(GetUserDataResult result)
			{
				if (result.Data.TryGetValue("DailyLogin", out this.userDataRecord))
				{
					this.lastDailyLogin = this.userDataRecord.Value;
					return;
				}
				this.lastDailyLogin = "NONE";
				base.StartCoroutine(this.GetMyDaily());
			}, delegate(PlayFabError error)
			{
				Debug.Log("Got error getting read-only user data:");
				Debug.Log(error.GenerateErrorReport());
				this.lastDailyLogin = "FAILED";
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					Application.Quit();
					NetworkSystem.Instance.ReturnToSinglePlayer();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GTPlayer.Instance);
					GameObject[] array = Object.FindObjectsOfType<GameObject>();
					for (int i = 0; i < array.Length; i++)
					{
						Object.Destroy(array[i]);
					}
				}
			}, null, null);
		}

		// Token: 0x0600546D RID: 21613 RVA: 0x001A21C9 File Offset: 0x001A03C9
		private IEnumerator CheckCanGetDaily()
		{
			while (!KIDManager.InitialisationComplete)
			{
				yield return new WaitForSeconds(1f);
			}
			for (;;)
			{
				if (GorillaComputer.instance != null && GorillaComputer.instance.startupMillis != 0L)
				{
					this.currentTime = new DateTime((GorillaComputer.instance.startupMillis + (long)(Time.realtimeSinceStartup * 1000f)) * 10000L);
					this.secondsUntilTomorrow = (int)(this.currentTime.AddDays(1.0).Date - this.currentTime).TotalSeconds;
					if (this.lastDailyLogin == null || this.lastDailyLogin == "")
					{
						this.GetLastDailyLogin();
					}
					else if (this.currentTime.ToString("o").Substring(0, 10) == this.lastDailyLogin)
					{
						this.checkedDaily = true;
						this.gotMyDaily = true;
					}
					else if (this.currentTime.ToString("o").Substring(0, 10) != this.lastDailyLogin)
					{
						this.checkedDaily = true;
						this.gotMyDaily = false;
						base.StartCoroutine(this.GetMyDaily());
					}
					else if (this.lastDailyLogin == "FAILED")
					{
						this.GetLastDailyLogin();
					}
					this.secondsToWaitToCheckDaily = (this.checkedDaily ? 60f : 10f);
					this.UpdateCurrencyBoards();
					yield return new WaitForSeconds(this.secondsToWaitToCheckDaily);
				}
				else
				{
					yield return new WaitForSeconds(1f);
				}
			}
			yield break;
		}

		// Token: 0x0600546E RID: 21614 RVA: 0x001A21D8 File Offset: 0x001A03D8
		private IEnumerator GetMyDaily()
		{
			yield return new WaitForSeconds(10f);
			GorillaServer.Instance.TryDistributeCurrency(delegate(ExecuteFunctionResult result)
			{
				this.GetCurrencyBalance();
				this.GetLastDailyLogin();
			}, delegate(PlayFabError error)
			{
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					Application.Quit();
					NetworkSystem.Instance.ReturnToSinglePlayer();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GTPlayer.Instance);
					GameObject[] array = Object.FindObjectsOfType<GameObject>();
					for (int i = 0; i < array.Length; i++)
					{
						Object.Destroy(array[i]);
					}
				}
			});
			yield break;
		}

		// Token: 0x0600546F RID: 21615 RVA: 0x001A21E7 File Offset: 0x001A03E7
		public void GetCosmeticsPlayFabCatalogData()
		{
			this.v2_isGetCosmeticsPlayCatalogDataWaitingForCallback = false;
			if (!this.v2_allCosmeticsInfoAssetRef_isLoaded)
			{
				throw new Exception("Method `GetCosmeticsPlayFabCatalogData` was called before `v2_allCosmeticsInfoAssetRef` was loaded. Listen to callback `V2_allCosmeticsInfoAssetRef_OnPostLoad` or check `v2_allCosmeticsInfoAssetRef_isLoaded` before trying to get PlayFab catalog data.");
			}
			PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), delegate(GetUserInventoryResult result)
			{
				PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest
				{
					CatalogVersion = this.catalog
				}, delegate(GetCatalogItemsResult result2)
				{
					this.unlockedCosmetics.Clear();
					this.unlockedHats.Clear();
					this.unlockedBadges.Clear();
					this.unlockedFaces.Clear();
					this.unlockedPaws.Clear();
					this.unlockedFurs.Clear();
					this.unlockedShirts.Clear();
					this.unlockedPants.Clear();
					this.unlockedArms.Clear();
					this.unlockedBacks.Clear();
					this.unlockedChests.Clear();
					this.unlockedTagFX.Clear();
					this.unlockedThrowables.Clear();
					this.catalogItems = result2.Catalog;
					using (List<CatalogItem>.Enumerator enumerator = this.catalogItems.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							CatalogItem catalogItem = enumerator.Current;
							if (!BuilderSetManager.IsItemIDBuilderItem(catalogItem.ItemId))
							{
								this.searchIndex = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => catalogItem.ItemId == x.itemName);
								if (this.searchIndex > -1)
								{
									this.tempStringArray = null;
									this.hasPrice = false;
									if (catalogItem.Bundle != null)
									{
										this.tempStringArray = catalogItem.Bundle.BundledItems.ToArray();
									}
									uint cost;
									if (catalogItem.VirtualCurrencyPrices.TryGetValue(this.currencyName, out cost))
									{
										this.hasPrice = true;
									}
									CosmeticsController.CosmeticItem cosmeticItem = this.allCosmetics[this.searchIndex];
									cosmeticItem.itemName = catalogItem.ItemId;
									cosmeticItem.displayName = catalogItem.DisplayName;
									cosmeticItem.cost = (int)cost;
									cosmeticItem.bundledItems = this.tempStringArray;
									cosmeticItem.canTryOn = this.hasPrice;
									if (cosmeticItem.itemCategory == CosmeticsController.CosmeticCategory.Paw)
									{
										CosmeticInfoV2 cosmeticInfoV = this.v2_allCosmetics[this.searchIndex];
										cosmeticItem.isThrowable = (cosmeticInfoV.isThrowable && !cosmeticInfoV.hasWardrobeParts);
									}
									if (cosmeticItem.displayName == null)
									{
										string text = "null";
										if (this.allCosmetics[this.searchIndex].itemPicture)
										{
											text = this.allCosmetics[this.searchIndex].itemPicture.name;
										}
										string debugCosmeticSOName = this.v2_allCosmetics[this.searchIndex].debugCosmeticSOName;
										Debug.LogError(string.Concat(new string[]
										{
											string.Format("Cosmetic encountered with a null displayName at index {0}! ", this.searchIndex),
											"Setting displayName to id: \"",
											this.allCosmetics[this.searchIndex].itemName,
											"\". iconName=\"",
											text,
											"\".cosmeticSOName=\"",
											debugCosmeticSOName,
											"\". "
										}));
										cosmeticItem.displayName = cosmeticItem.itemName;
									}
									this.V2_ConformCosmeticItemV1DisplayName(ref cosmeticItem);
									this._allCosmetics[this.searchIndex] = cosmeticItem;
									this._allCosmeticsDict[cosmeticItem.itemName] = cosmeticItem;
									this._allCosmeticsItemIDsfromDisplayNamesDict[cosmeticItem.displayName] = cosmeticItem.itemName;
									this._allCosmeticsItemIDsfromDisplayNamesDict[cosmeticItem.overrideDisplayName] = cosmeticItem.itemName;
								}
							}
						}
					}
					for (int i = this._allCosmetics.Count - 1; i > -1; i--)
					{
						this.tempItem = this._allCosmetics[i];
						if (this.tempItem.itemCategory == CosmeticsController.CosmeticCategory.Set && this.tempItem.canTryOn)
						{
							string[] bundledItems = this.tempItem.bundledItems;
							for (int j = 0; j < bundledItems.Length; j++)
							{
								string setItemName = bundledItems[j];
								this.searchIndex = this._allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => setItemName == x.itemName);
								if (this.searchIndex > -1)
								{
									this.tempItem = this._allCosmetics[this.searchIndex];
									this.tempItem.canTryOn = true;
									this._allCosmetics[this.searchIndex] = this.tempItem;
									this._allCosmeticsDict[this._allCosmetics[this.searchIndex].itemName] = this.tempItem;
									this._allCosmeticsItemIDsfromDisplayNamesDict[this._allCosmetics[this.searchIndex].displayName] = this.tempItem.itemName;
								}
							}
						}
					}
					foreach (KeyValuePair<string, StoreBundle> keyValuePair in BundleManager.instance.storeBundlesById)
					{
						string text2;
						StoreBundle bundleData2;
						keyValuePair.Deconstruct(out text2, out bundleData2);
						string key = text2;
						StoreBundle bundleData = bundleData2;
						int num = this._allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => bundleData.playfabBundleID == x.itemName);
						if (num > 0 && this._allCosmetics[num].bundledItems != null)
						{
							string[] bundledItems = this._allCosmetics[num].bundledItems;
							for (int j = 0; j < bundledItems.Length; j++)
							{
								string setItemName = bundledItems[j];
								this.searchIndex = this._allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => setItemName == x.itemName);
								if (this.searchIndex > -1)
								{
									this.tempItem = this._allCosmetics[this.searchIndex];
									this.tempItem.canTryOn = true;
									this._allCosmetics[this.searchIndex] = this.tempItem;
									this._allCosmeticsDict[this._allCosmetics[this.searchIndex].itemName] = this.tempItem;
									this._allCosmeticsItemIDsfromDisplayNamesDict[this._allCosmetics[this.searchIndex].displayName] = this.tempItem.itemName;
								}
							}
						}
						if (!bundleData.HasPrice)
						{
							num = this.catalogItems.FindIndex((CatalogItem ci) => ci.Bundle != null && ci.ItemId == bundleData.playfabBundleID);
							if (num > 0)
							{
								uint bundlePrice;
								if (this.catalogItems[num].VirtualCurrencyPrices.TryGetValue("RM", out bundlePrice))
								{
									BundleManager.instance.storeBundlesById[key].TryUpdatePrice(bundlePrice);
								}
								else
								{
									BundleManager.instance.storeBundlesById[key].TryUpdatePrice(null);
								}
							}
						}
					}
					this.searchIndex = this._allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => "Slingshot" == x.itemName);
					if (this.searchIndex < 0)
					{
						throw new MissingReferenceException("CosmeticsController: Cannot find default slingshot! it is required for players that do not have another slingshot equipped and are playing Paintbrawl.");
					}
					this._allCosmeticsDict["Slingshot"] = this._allCosmetics[this.searchIndex];
					this._allCosmeticsItemIDsfromDisplayNamesDict[this._allCosmetics[this.searchIndex].displayName] = this._allCosmetics[this.searchIndex].itemName;
					this.allCosmeticsDict_isInitialized = true;
					this.allCosmeticsItemIDsfromDisplayNamesDict_isInitialized = true;
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					using (List<ItemInstance>.Enumerator enumerator3 = result.Inventory.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							ItemInstance item = enumerator3.Current;
							if (!BuilderSetManager.IsItemIDBuilderItem(item.ItemId))
							{
								if (item.ItemId == this.m_earlyAccessSupporterPackCosmeticSO.info.playFabID)
								{
									foreach (CosmeticSO cosmeticSO in this.m_earlyAccessSupporterPackCosmeticSO.info.setCosmetics)
									{
										CosmeticsController.CosmeticItem item2;
										if (this.allCosmeticsDict.TryGetValue(cosmeticSO.info.playFabID, out item2))
										{
											this.unlockedCosmetics.Add(item2);
										}
									}
								}
								BundleManager.instance.MarkBundleOwnedByPlayFabID(item.ItemId);
								if (!dictionary.ContainsKey(item.ItemId))
								{
									this.searchIndex = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => item.ItemId == x.itemName);
									if (this.searchIndex > -1)
									{
										dictionary[item.ItemId] = item.ItemId;
										this.unlockedCosmetics.Add(this.allCosmetics[this.searchIndex]);
									}
								}
							}
						}
					}
					foreach (CosmeticsController.CosmeticItem cosmeticItem2 in this.unlockedCosmetics)
					{
						if (cosmeticItem2.itemCategory == CosmeticsController.CosmeticCategory.Hat && !this.unlockedHats.Contains(cosmeticItem2))
						{
							this.unlockedHats.Add(cosmeticItem2);
						}
						else if (cosmeticItem2.itemCategory == CosmeticsController.CosmeticCategory.Face && !this.unlockedFaces.Contains(cosmeticItem2))
						{
							this.unlockedFaces.Add(cosmeticItem2);
						}
						else if (cosmeticItem2.itemCategory == CosmeticsController.CosmeticCategory.Badge && !this.unlockedBadges.Contains(cosmeticItem2))
						{
							this.unlockedBadges.Add(cosmeticItem2);
						}
						else if (cosmeticItem2.itemCategory == CosmeticsController.CosmeticCategory.Paw)
						{
							if (!cosmeticItem2.isThrowable && !this.unlockedPaws.Contains(cosmeticItem2))
							{
								this.unlockedPaws.Add(cosmeticItem2);
							}
							else if (cosmeticItem2.isThrowable && !this.unlockedThrowables.Contains(cosmeticItem2))
							{
								this.unlockedThrowables.Add(cosmeticItem2);
							}
						}
						else if (cosmeticItem2.itemCategory == CosmeticsController.CosmeticCategory.Fur && !this.unlockedFurs.Contains(cosmeticItem2))
						{
							this.unlockedFurs.Add(cosmeticItem2);
						}
						else if (cosmeticItem2.itemCategory == CosmeticsController.CosmeticCategory.Shirt && !this.unlockedShirts.Contains(cosmeticItem2))
						{
							this.unlockedShirts.Add(cosmeticItem2);
						}
						else if (cosmeticItem2.itemCategory == CosmeticsController.CosmeticCategory.Arms && !this.unlockedArms.Contains(cosmeticItem2))
						{
							this.unlockedArms.Add(cosmeticItem2);
						}
						else if (cosmeticItem2.itemCategory == CosmeticsController.CosmeticCategory.Back && !this.unlockedBacks.Contains(cosmeticItem2))
						{
							this.unlockedBacks.Add(cosmeticItem2);
						}
						else if (cosmeticItem2.itemCategory == CosmeticsController.CosmeticCategory.Chest && !this.unlockedChests.Contains(cosmeticItem2))
						{
							this.unlockedChests.Add(cosmeticItem2);
						}
						else if (cosmeticItem2.itemCategory == CosmeticsController.CosmeticCategory.Pants && !this.unlockedPants.Contains(cosmeticItem2))
						{
							this.unlockedPants.Add(cosmeticItem2);
						}
						else if (cosmeticItem2.itemCategory == CosmeticsController.CosmeticCategory.TagEffect && !this.unlockedTagFX.Contains(cosmeticItem2))
						{
							this.unlockedTagFX.Add(cosmeticItem2);
						}
						this.concatStringCosmeticsAllowed += cosmeticItem2.itemName;
					}
					BuilderSetManager.instance.OnGotInventoryItems(result, result2);
					this.currencyBalance = result.VirtualCurrency[this.currencyName];
					int num2;
					this.playedInBeta = (result.VirtualCurrency.TryGetValue("TC", out num2) && num2 > 0);
					Action onGetCurrency = this.OnGetCurrency;
					if (onGetCurrency != null)
					{
						onGetCurrency();
					}
					BundleManager.instance.CheckIfBundlesOwned();
					StoreUpdater.instance.Initialize();
					this.currentWornSet.LoadFromPlayerPreferences(this);
					if (!ATM_Manager.instance.alreadyBegan)
					{
						ATM_Manager.instance.SwitchToStage(ATM_Manager.ATMStages.Begin);
						ATM_Manager.instance.alreadyBegan = true;
					}
					this.ProcessPurchaseItemState(null, false);
					this.UpdateShoppingCart();
					this.UpdateCurrencyBoards();
					if (this.UseNewCosmeticsPath())
					{
						this.ConfirmIndividualCosmeticsSharedGroup(result);
					}
					Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
					if (onCosmeticsUpdated != null)
					{
						onCosmeticsUpdated();
					}
					this.v2_isCosmeticPlayFabCatalogDataLoaded = true;
					Action v2_OnGetCosmeticsPlayFabCatalogData_PostSuccess = this.V2_OnGetCosmeticsPlayFabCatalogData_PostSuccess;
					if (v2_OnGetCosmeticsPlayFabCatalogData_PostSuccess != null)
					{
						v2_OnGetCosmeticsPlayFabCatalogData_PostSuccess();
					}
					if (!CosmeticsV2Spawner_Dirty.startedAllPartsInstantiated && !CosmeticsV2Spawner_Dirty.allPartsInstantiated)
					{
						CosmeticsV2Spawner_Dirty.StartInstantiatingPrefabs();
					}
				}, delegate(PlayFabError error)
				{
					if (error.Error == PlayFabErrorCode.NotAuthenticated)
					{
						PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					}
					else if (error.Error == PlayFabErrorCode.AccountBanned)
					{
						Application.Quit();
						NetworkSystem.Instance.ReturnToSinglePlayer();
						Object.DestroyImmediate(PhotonNetworkController.Instance);
						Object.DestroyImmediate(GTPlayer.Instance);
						GameObject[] array = Object.FindObjectsOfType<GameObject>();
						for (int i = 0; i < array.Length; i++)
						{
							Object.Destroy(array[i]);
						}
					}
					if (!this.tryTwice)
					{
						this.tryTwice = true;
						this.GetCosmeticsPlayFabCatalogData();
					}
				}, null, null);
			}, delegate(PlayFabError error)
			{
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
				}
				else if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					Application.Quit();
					NetworkSystem.Instance.ReturnToSinglePlayer();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GTPlayer.Instance);
					GameObject[] array = Object.FindObjectsOfType<GameObject>();
					for (int i = 0; i < array.Length; i++)
					{
						Object.Destroy(array[i]);
					}
				}
				if (!this.tryTwice)
				{
					this.tryTwice = true;
					this.GetCosmeticsPlayFabCatalogData();
				}
			}, null, null);
		}

		// Token: 0x06005470 RID: 21616 RVA: 0x001A2228 File Offset: 0x001A0428
		public void SteamPurchase()
		{
			if (string.IsNullOrEmpty(this.itemToPurchase))
			{
				Debug.Log("Unable to start steam purchase process. itemToPurchase is not set.");
				return;
			}
			Debug.Log(string.Format("attempting to purchase item through steam. Is this a bundle purchase: {0}", this.buyingBundle));
			PlayFabClientAPI.StartPurchase(this.GetStartPurchaseRequest(), new Action<StartPurchaseResult>(this.ProcessStartPurchaseResponse), new Action<PlayFabError>(this.ProcessSteamPurchaseError), null, null);
		}

		// Token: 0x06005471 RID: 21617 RVA: 0x001A228C File Offset: 0x001A048C
		private StartPurchaseRequest GetStartPurchaseRequest()
		{
			return new StartPurchaseRequest
			{
				CatalogVersion = this.catalog,
				Items = new List<ItemPurchaseRequest>
				{
					new ItemPurchaseRequest
					{
						ItemId = this.itemToPurchase,
						Quantity = 1U,
						Annotation = "Purchased via in-game store"
					}
				}
			};
		}

		// Token: 0x06005472 RID: 21618 RVA: 0x001A22E0 File Offset: 0x001A04E0
		private void ProcessStartPurchaseResponse(StartPurchaseResult result)
		{
			Debug.Log("successfully started purchase. attempted to pay for purchase through steam");
			this.currentPurchaseID = result.OrderId;
			PlayFabClientAPI.PayForPurchase(CosmeticsController.GetPayForPurchaseRequest(this.currentPurchaseID), new Action<PayForPurchaseResult>(CosmeticsController.ProcessPayForPurchaseResult), new Action<PlayFabError>(this.ProcessSteamPurchaseError), null, null);
		}

		// Token: 0x06005473 RID: 21619 RVA: 0x001A232D File Offset: 0x001A052D
		private static PayForPurchaseRequest GetPayForPurchaseRequest(string orderId)
		{
			return new PayForPurchaseRequest
			{
				OrderId = orderId,
				ProviderName = "Steam",
				Currency = "RM"
			};
		}

		// Token: 0x06005474 RID: 21620 RVA: 0x001A2351 File Offset: 0x001A0551
		private static void ProcessPayForPurchaseResult(PayForPurchaseResult result)
		{
			Debug.Log("succeeded on sending request for paying with steam! waiting for response");
		}

		// Token: 0x06005475 RID: 21621 RVA: 0x001A2360 File Offset: 0x001A0560
		private void ProcessSteamCallback(MicroTxnAuthorizationResponse_t callBackResponse)
		{
			Debug.Log("Steam has called back that the user has finished the payment interaction");
			if (callBackResponse.m_bAuthorized == 0)
			{
				Debug.Log("Steam has indicated that the payment was not authorised.");
			}
			if (this.buyingBundle)
			{
				PlayFabClientAPI.ConfirmPurchase(this.GetConfirmBundlePurchaseRequest(), delegate(ConfirmPurchaseResult _)
				{
					this.ProcessConfirmPurchaseSuccess();
				}, new Action<PlayFabError>(this.ProcessConfirmPurchaseError), null, null);
				return;
			}
			PlayFabClientAPI.ConfirmPurchase(this.GetConfirmATMPurchaseRequest(), delegate(ConfirmPurchaseResult _)
			{
				this.ProcessConfirmPurchaseSuccess();
			}, new Action<PlayFabError>(this.ProcessConfirmPurchaseError), null, null);
		}

		// Token: 0x06005476 RID: 21622 RVA: 0x001A23DC File Offset: 0x001A05DC
		private ConfirmPurchaseRequest GetConfirmBundlePurchaseRequest()
		{
			return new ConfirmPurchaseRequest
			{
				OrderId = this.currentPurchaseID
			};
		}

		// Token: 0x06005477 RID: 21623 RVA: 0x001A23F0 File Offset: 0x001A05F0
		private ConfirmPurchaseRequest GetConfirmATMPurchaseRequest()
		{
			return new ConfirmPurchaseRequest
			{
				OrderId = this.currentPurchaseID,
				CustomTags = new Dictionary<string, string>
				{
					{
						"NexusCreatorId",
						ATM_Manager.instance.ValidatedCreatorCode
					},
					{
						"PlayerName",
						GorillaComputer.instance.savedName
					}
				}
			};
		}

		// Token: 0x06005478 RID: 21624 RVA: 0x001A2448 File Offset: 0x001A0648
		private void ProcessConfirmPurchaseSuccess()
		{
			if (this.buyingBundle)
			{
				this.buyingBundle = false;
				if (PhotonNetwork.InRoom)
				{
					object[] data = new object[0];
					NetworkSystemRaiseEvent.RaiseEvent(9, data, NetworkSystemRaiseEvent.newWeb, true);
				}
				base.StartCoroutine(this.CheckIfMyCosmeticsUpdated(this.BundlePlayfabItemName));
			}
			else
			{
				ATM_Manager.instance.SwitchToStage(ATM_Manager.ATMStages.Success);
			}
			this.GetCurrencyBalance();
			this.UpdateCurrencyBoards();
			this.GetCosmeticsPlayFabCatalogData();
			GorillaTagger.Instance.offlineVRRig.GetCosmeticsPlayFabCatalogData();
		}

		// Token: 0x06005479 RID: 21625 RVA: 0x001A24C3 File Offset: 0x001A06C3
		private void ProcessConfirmPurchaseError(PlayFabError error)
		{
			this.ProcessSteamPurchaseError(error);
			ATM_Manager.instance.SwitchToStage(ATM_Manager.ATMStages.Failure);
			this.UpdateCurrencyBoards();
		}

		// Token: 0x0600547A RID: 21626 RVA: 0x001A24E0 File Offset: 0x001A06E0
		private void ProcessSteamPurchaseError(PlayFabError error)
		{
			PlayFabErrorCode error2 = error.Error;
			if (error2 <= PlayFabErrorCode.PurchaseInitializationFailure)
			{
				if (error2 <= PlayFabErrorCode.FailedByPaymentProvider)
				{
					if (error2 == PlayFabErrorCode.AccountBanned)
					{
						PhotonNetwork.Disconnect();
						Object.DestroyImmediate(PhotonNetworkController.Instance);
						Object.DestroyImmediate(GTPlayer.Instance);
						GameObject[] array = Object.FindObjectsOfType<GameObject>();
						for (int i = 0; i < array.Length; i++)
						{
							Object.Destroy(array[i]);
						}
						Application.Quit();
						goto IL_1A1;
					}
					if (error2 != PlayFabErrorCode.FailedByPaymentProvider)
					{
						goto IL_191;
					}
					Debug.Log(string.Format("Attempted to pay for order, but has been Failed by Steam with error: {0}", error));
					goto IL_1A1;
				}
				else
				{
					if (error2 == PlayFabErrorCode.InsufficientFunds)
					{
						Debug.Log(string.Format("Attempting to do purchase through steam, steam has returned insufficient funds: {0}", error));
						goto IL_1A1;
					}
					if (error2 == PlayFabErrorCode.InvalidPaymentProvider)
					{
						Debug.Log(string.Format("Attempted to connect to steam as payment provider, but received error: {0}", error));
						goto IL_1A1;
					}
					if (error2 != PlayFabErrorCode.PurchaseInitializationFailure)
					{
						goto IL_191;
					}
				}
			}
			else if (error2 <= PlayFabErrorCode.InvalidPurchaseTransactionStatus)
			{
				if (error2 == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					goto IL_1A1;
				}
				if (error2 == PlayFabErrorCode.PurchaseDoesNotExist)
				{
					Debug.Log(string.Format("Attempting to confirm purchase for order {0} but received error: {1}", this.currentPurchaseID, error));
					goto IL_1A1;
				}
				if (error2 != PlayFabErrorCode.InvalidPurchaseTransactionStatus)
				{
					goto IL_191;
				}
			}
			else
			{
				if (error2 == PlayFabErrorCode.InternalServerError)
				{
					Debug.Log(string.Format("PlayFab threw an internal server error: {0}", error));
					goto IL_1A1;
				}
				if (error2 == PlayFabErrorCode.StoreNotFound)
				{
					Debug.Log(string.Format("Attempted to load {0} from {1} but received an error: {2}", this.itemToPurchase, this.catalog, error));
					goto IL_1A1;
				}
				if (error2 != PlayFabErrorCode.DuplicatePurchaseTransactionId)
				{
					goto IL_191;
				}
			}
			Debug.Log(string.Format("Attempted to pay for order {0}, however received an error: {1}", this.currentPurchaseID, error));
			goto IL_1A1;
			IL_191:
			Debug.Log(string.Format("Steam purchase flow returned error: {0}", error));
			IL_1A1:
			ATM_Manager.instance.SwitchToStage(ATM_Manager.ATMStages.Failure);
		}

		// Token: 0x0600547B RID: 21627 RVA: 0x001A269C File Offset: 0x001A089C
		public void UpdateCurrencyBoards()
		{
			this.FormattedPurchaseText(this.finalLine, null, null, false, false);
			this.iterator = 0;
			while (this.iterator < this.currencyBoards.Count)
			{
				if (this.currencyBoards[this.iterator].IsNotNull())
				{
					this.currencyBoards[this.iterator].UpdateCurrencyBoard(this.checkedDaily, this.gotMyDaily, this.currencyBalance, this.secondsUntilTomorrow);
				}
				this.iterator++;
			}
		}

		// Token: 0x0600547C RID: 21628 RVA: 0x001A2729 File Offset: 0x001A0929
		public void AddCurrencyBoard(CurrencyBoard newCurrencyBoard)
		{
			if (this.currencyBoards.Contains(newCurrencyBoard))
			{
				return;
			}
			this.currencyBoards.Add(newCurrencyBoard);
			newCurrencyBoard.UpdateCurrencyBoard(this.checkedDaily, this.gotMyDaily, this.currencyBalance, this.secondsUntilTomorrow);
		}

		// Token: 0x0600547D RID: 21629 RVA: 0x001A2764 File Offset: 0x001A0964
		public void RemoveCurrencyBoard(CurrencyBoard currencyBoardToRemove)
		{
			this.currencyBoards.Remove(currencyBoardToRemove);
		}

		// Token: 0x0600547E RID: 21630 RVA: 0x001A2773 File Offset: 0x001A0973
		public void GetCurrencyBalance()
		{
			PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), delegate(GetUserInventoryResult result)
			{
				this.currencyBalance = result.VirtualCurrency[this.currencyName];
				this.UpdateCurrencyBoards();
				Action onGetCurrency = this.OnGetCurrency;
				if (onGetCurrency == null)
				{
					return;
				}
				onGetCurrency();
			}, delegate(PlayFabError error)
			{
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					Application.Quit();
					NetworkSystem.Instance.ReturnToSinglePlayer();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GTPlayer.Instance);
					GameObject[] array = Object.FindObjectsOfType<GameObject>();
					for (int i = 0; i < array.Length; i++)
					{
						Object.Destroy(array[i]);
					}
				}
			}, null, null);
		}

		// Token: 0x0600547F RID: 21631 RVA: 0x001A27AC File Offset: 0x001A09AC
		public string GetItemDisplayName(CosmeticsController.CosmeticItem item)
		{
			if (item.overrideDisplayName != null && item.overrideDisplayName != "")
			{
				return item.overrideDisplayName;
			}
			return item.displayName;
		}

		// Token: 0x06005480 RID: 21632 RVA: 0x001A27D8 File Offset: 0x001A09D8
		public void UpdateMyCosmetics()
		{
			if (NetworkSystem.Instance.InRoom)
			{
				if (GorillaServer.Instance != null && GorillaServer.Instance.NewCosmeticsPathShouldSetSharedGroupData())
				{
					this.UpdateMyCosmeticsForRoom(true);
				}
				if (GorillaServer.Instance != null && GorillaServer.Instance.NewCosmeticsPathShouldSetRoomData())
				{
					this.UpdateMyCosmeticsForRoom(false);
					return;
				}
			}
			else if (GorillaServer.Instance != null && GorillaServer.Instance.NewCosmeticsPathShouldSetSharedGroupData())
			{
				this.UpdateMyCosmeticsNotInRoom();
			}
		}

		// Token: 0x06005481 RID: 21633 RVA: 0x001A285D File Offset: 0x001A0A5D
		private void UpdateMyCosmeticsNotInRoom()
		{
			if (GorillaServer.Instance != null)
			{
				GorillaServer.Instance.UpdateUserCosmetics();
			}
		}

		// Token: 0x06005482 RID: 21634 RVA: 0x001A287C File Offset: 0x001A0A7C
		private void UpdateMyCosmeticsForRoom(bool shouldSetSharedGroupData)
		{
			byte eventCode = 9;
			if (shouldSetSharedGroupData)
			{
				eventCode = 10;
			}
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			WebFlags flags = new WebFlags(1);
			raiseEventOptions.Flags = flags;
			object[] eventContent = new object[0];
			PhotonNetwork.RaiseEvent(eventCode, eventContent, raiseEventOptions, SendOptions.SendReliable);
		}

		// Token: 0x06005483 RID: 21635 RVA: 0x001A28BC File Offset: 0x001A0ABC
		private void AlreadyOwnAllBundleButtons()
		{
			EarlyAccessButton[] array = this.earlyAccessButtons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].AlreadyOwn();
			}
		}

		// Token: 0x06005484 RID: 21636 RVA: 0x001A28E6 File Offset: 0x001A0AE6
		private bool UseNewCosmeticsPath()
		{
			return GorillaServer.Instance != null && GorillaServer.Instance.NewCosmeticsPathShouldReadSharedGroupData();
		}

		// Token: 0x06005485 RID: 21637 RVA: 0x001A2905 File Offset: 0x001A0B05
		public void CheckCosmeticsSharedGroup()
		{
			this.updateCosmeticsRetries++;
			if (this.updateCosmeticsRetries < this.maxUpdateCosmeticsRetries)
			{
				base.StartCoroutine(this.WaitForNextCosmeticsAttempt());
			}
		}

		// Token: 0x06005486 RID: 21638 RVA: 0x001A2930 File Offset: 0x001A0B30
		private IEnumerator WaitForNextCosmeticsAttempt()
		{
			int num = (int)Mathf.Pow(3f, (float)(this.updateCosmeticsRetries + 1));
			yield return new WaitForSeconds((float)num);
			this.ConfirmIndividualCosmeticsSharedGroup(this.latestInventory);
			yield break;
		}

		// Token: 0x06005487 RID: 21639 RVA: 0x001A2940 File Offset: 0x001A0B40
		private void ConfirmIndividualCosmeticsSharedGroup(GetUserInventoryResult inventory)
		{
			Debug.Log("confirming individual cosmetics with shared group");
			this.latestInventory = inventory;
			if (PhotonNetwork.LocalPlayer.UserId == null)
			{
				base.StartCoroutine(this.WaitForNextCosmeticsAttempt());
				return;
			}
			PlayFabClientAPI.GetSharedGroupData(new PlayFab.ClientModels.GetSharedGroupDataRequest
			{
				Keys = this.inventoryStringList,
				SharedGroupId = PhotonNetwork.LocalPlayer.UserId + "Inventory"
			}, delegate(GetSharedGroupDataResult result)
			{
				bool flag = true;
				foreach (KeyValuePair<string, PlayFab.ClientModels.SharedGroupDataRecord> keyValuePair in result.Data)
				{
					if (keyValuePair.Key != "Inventory")
					{
						break;
					}
					foreach (ItemInstance itemInstance in inventory.Inventory)
					{
						if (itemInstance.CatalogVersion == CosmeticsController.instance.catalog && !keyValuePair.Value.Value.Contains(itemInstance.ItemId))
						{
							flag = false;
							break;
						}
					}
				}
				if (!flag || result.Data.Count == 0)
				{
					this.UpdateMyCosmetics();
					return;
				}
				this.updateCosmeticsRetries = 0;
			}, delegate(PlayFabError error)
			{
				this.ReauthOrBan(error);
				this.CheckCosmeticsSharedGroup();
			}, null, null);
		}

		// Token: 0x06005488 RID: 21640 RVA: 0x001A29DC File Offset: 0x001A0BDC
		public void ReauthOrBan(PlayFabError error)
		{
			if (error.Error == PlayFabErrorCode.NotAuthenticated)
			{
				PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
				return;
			}
			if (error.Error == PlayFabErrorCode.AccountBanned)
			{
				Application.Quit();
				PhotonNetwork.Disconnect();
				Object.DestroyImmediate(PhotonNetworkController.Instance);
				Object.DestroyImmediate(GTPlayer.Instance);
				GameObject[] array = Object.FindObjectsOfType<GameObject>();
				for (int i = 0; i < array.Length; i++)
				{
					Object.Destroy(array[i]);
				}
			}
		}

		// Token: 0x06005489 RID: 21641 RVA: 0x001A2A4C File Offset: 0x001A0C4C
		public void ProcessExternalUnlock(string itemID, bool autoEquip, bool isLeftHand)
		{
			this.UnlockItem(itemID, false);
			VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			offlineVRRig.concatStringOfCosmeticsAllowed += itemID;
			this.UpdateMyCosmetics();
			if (autoEquip)
			{
				CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(itemID);
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.external_item_claim, itemFromDict);
				List<CosmeticsController.CosmeticSlots> list = CollectionPool<List<CosmeticsController.CosmeticSlots>, CosmeticsController.CosmeticSlots>.Get();
				if (list.Capacity < 16)
				{
					list.Capacity = 16;
				}
				this.ApplyCosmeticItemToSet(this.currentWornSet, itemFromDict, isLeftHand, true, list);
				foreach (CosmeticsController.CosmeticSlots cosmeticSlots in list)
				{
					this.tryOnSet.items[(int)cosmeticSlots] = this.nullItem;
				}
				CollectionPool<List<CosmeticsController.CosmeticSlots>, CosmeticsController.CosmeticSlots>.Release(list);
				this.UpdateShoppingCart();
				this.UpdateWornCosmetics(true);
				Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
				if (onCosmeticsUpdated == null)
				{
					return;
				}
				onCosmeticsUpdated();
			}
		}

		// Token: 0x0600548A RID: 21642 RVA: 0x001A2B40 File Offset: 0x001A0D40
		public void ClearTemporaryUnlocks(VRRig rig, string[] itemID)
		{
			if (this.tempUnlocks.Count == 0)
			{
				return;
			}
			string text = GorillaTagger.Instance.offlineVRRig.concatStringOfCosmeticsAllowed;
			for (int i = 0; i < itemID.Length; i++)
			{
				if (this.tempUnlocks.Contains(itemID[i]))
				{
					int num = text.IndexOf(itemID[i]);
					if (num >= 0)
					{
						text = text.Remove(num, itemID[i].Length);
						this.RemoveCosmeticItemFromSet(this.currentWornSet, itemID[i], false);
					}
					this.tempUnlocks.Remove(itemID[i]);
				}
			}
			GorillaTagger.Instance.offlineVRRig.concatStringOfCosmeticsAllowed = text;
			this.UpdateMyCosmetics();
			this.UpdateWornCosmetics(true);
			Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
			if (onCosmeticsUpdated == null)
			{
				return;
			}
			onCosmeticsUpdated();
		}

		// Token: 0x0600548B RID: 21643 RVA: 0x001A2BF4 File Offset: 0x001A0DF4
		public void ClearTemporaryUnlocks(VRRig rig, string itemID)
		{
			if (this.tempUnlocks.Count == 0)
			{
				return;
			}
			string text = GorillaTagger.Instance.offlineVRRig.concatStringOfCosmeticsAllowed;
			if (this.tempUnlocks.Contains(itemID))
			{
				int num = text.IndexOf(itemID);
				if (num >= 0)
				{
					text = text.Remove(num, itemID.Length);
					this.RemoveCosmeticItemFromSet(this.currentWornSet, itemID, false);
				}
				this.tempUnlocks.Remove(itemID);
			}
			GorillaTagger.Instance.offlineVRRig.concatStringOfCosmeticsAllowed = text;
			this.UpdateMyCosmetics();
			this.UpdateWornCosmetics(true);
			Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
			if (onCosmeticsUpdated == null)
			{
				return;
			}
			onCosmeticsUpdated();
		}

		// Token: 0x0600548C RID: 21644 RVA: 0x001A2C90 File Offset: 0x001A0E90
		public void TemporaryUnlock(VRRig rig, string[] itemID)
		{
			for (int i = 0; i < itemID.Length; i++)
			{
				if (!rig.concatStringOfCosmeticsAllowed.Contains(itemID[i]))
				{
					this.UnlockItem(itemID[i], false);
					rig.concatStringOfCosmeticsAllowed += itemID[i];
					this.tempUnlocks.Add(itemID[i]);
				}
			}
			this.UpdateMyCosmetics();
		}

		// Token: 0x0600548D RID: 21645 RVA: 0x001A2CED File Offset: 0x001A0EED
		public void TemporaryUnlock(VRRig rig, string itemID)
		{
			if (!rig.concatStringOfCosmeticsAllowed.Contains(itemID))
			{
				this.UnlockItem(itemID, false);
				rig.concatStringOfCosmeticsAllowed += itemID;
				this.tempUnlocks.Add(itemID);
				this.UpdateMyCosmetics();
			}
		}

		// Token: 0x0600548E RID: 21646 RVA: 0x001A2D29 File Offset: 0x001A0F29
		public bool BuildValidationCheck()
		{
			if (this.m_earlyAccessSupporterPackCosmeticSO == null)
			{
				Debug.LogError("m_earlyAccessSupporterPackCosmeticSO is empty, everything will break!");
				return false;
			}
			return true;
		}

		// Token: 0x0600548F RID: 21647 RVA: 0x001A2D46 File Offset: 0x001A0F46
		public void SetHideCosmeticsFromRemotePlayers(bool hideCosmetics)
		{
			if (hideCosmetics == this.isHidingCosmeticsFromRemotePlayers)
			{
				return;
			}
			this.isHidingCosmeticsFromRemotePlayers = hideCosmetics;
			GorillaTagger.Instance.offlineVRRig.reliableState.SetIsDirty();
			this.UpdateWornCosmetics(true);
		}

		// Token: 0x06005490 RID: 21648 RVA: 0x001A2D74 File Offset: 0x001A0F74
		public bool ValidatePackedItems(int[] packed)
		{
			if (packed.Length == 0)
			{
				return true;
			}
			int num = 0;
			int num2 = packed[0];
			for (int i = 0; i < 16; i++)
			{
				if ((num2 & 1 << i) != 0)
				{
					num++;
				}
			}
			return packed.Length == num + 1;
		}

		// Token: 0x04005DB8 RID: 23992
		[FormerlySerializedAs("v2AllCosmeticsInfoAssetRef")]
		[FormerlySerializedAs("newSysAllCosmeticsAssetRef")]
		[SerializeField]
		public GTAssetRef<AllCosmeticsArraySO> v2_allCosmeticsInfoAssetRef;

		// Token: 0x04005DBA RID: 23994
		private readonly Dictionary<string, CosmeticInfoV2> _allCosmeticsDictV2 = new Dictionary<string, CosmeticInfoV2>();

		// Token: 0x04005DBB RID: 23995
		public Action V2_allCosmeticsInfoAssetRef_OnPostLoad;

		// Token: 0x04005DBF RID: 23999
		public const int maximumTransferrableItems = 5;

		// Token: 0x04005DC0 RID: 24000
		[OnEnterPlay_SetNull]
		public static volatile CosmeticsController instance;

		// Token: 0x04005DC2 RID: 24002
		public Action V2_OnGetCosmeticsPlayFabCatalogData_PostSuccess;

		// Token: 0x04005DC3 RID: 24003
		public Action OnGetCurrency;

		// Token: 0x04005DC4 RID: 24004
		[FormerlySerializedAs("allCosmetics")]
		[SerializeField]
		private List<CosmeticsController.CosmeticItem> _allCosmetics;

		// Token: 0x04005DC6 RID: 24006
		public Dictionary<string, CosmeticsController.CosmeticItem> _allCosmeticsDict = new Dictionary<string, CosmeticsController.CosmeticItem>(2048);

		// Token: 0x04005DC8 RID: 24008
		public Dictionary<string, string> _allCosmeticsItemIDsfromDisplayNamesDict = new Dictionary<string, string>(2048);

		// Token: 0x04005DC9 RID: 24009
		private CosmeticAnchorAntiIntersectOffsets defaultClipOffsets;

		// Token: 0x04005DCA RID: 24010
		public CosmeticsController.CosmeticItem nullItem;

		// Token: 0x04005DCB RID: 24011
		public string catalog;

		// Token: 0x04005DCC RID: 24012
		private string[] tempStringArray;

		// Token: 0x04005DCD RID: 24013
		private CosmeticsController.CosmeticItem tempItem;

		// Token: 0x04005DCE RID: 24014
		private VRRigAnchorOverrides anchorOverrides;

		// Token: 0x04005DCF RID: 24015
		public List<CatalogItem> catalogItems;

		// Token: 0x04005DD0 RID: 24016
		public bool tryTwice;

		// Token: 0x04005DD1 RID: 24017
		public CustomMapCosmeticsData customMapCosmeticsData;

		// Token: 0x04005DD2 RID: 24018
		[NonSerialized]
		public CosmeticsController.CosmeticSet tryOnSet = new CosmeticsController.CosmeticSet();

		// Token: 0x04005DD3 RID: 24019
		public int numFittingRoomButtons = 12;

		// Token: 0x04005DD4 RID: 24020
		public List<FittingRoom> fittingRooms = new List<FittingRoom>();

		// Token: 0x04005DD5 RID: 24021
		public CosmeticStand[] cosmeticStands;

		// Token: 0x04005DD6 RID: 24022
		public List<CosmeticsController.CosmeticItem> currentCart = new List<CosmeticsController.CosmeticItem>();

		// Token: 0x04005DD7 RID: 24023
		public CosmeticsController.PurchaseItemStages currentPurchaseItemStage;

		// Token: 0x04005DD8 RID: 24024
		public List<ItemCheckout> itemCheckouts = new List<ItemCheckout>();

		// Token: 0x04005DD9 RID: 24025
		public CosmeticsController.CosmeticItem itemToBuy;

		// Token: 0x04005DDA RID: 24026
		private List<string> playerIDList = new List<string>();

		// Token: 0x04005DDB RID: 24027
		private List<string> inventoryStringList = new List<string>();

		// Token: 0x04005DDC RID: 24028
		private bool foundCosmetic;

		// Token: 0x04005DDD RID: 24029
		private int attempts;

		// Token: 0x04005DDE RID: 24030
		private string finalLine;

		// Token: 0x04005DDF RID: 24031
		private string leftCheckoutPurchaseButtonString;

		// Token: 0x04005DE0 RID: 24032
		private string rightCheckoutPurchaseButtonString;

		// Token: 0x04005DE1 RID: 24033
		private bool leftCheckoutPurchaseButtonOn;

		// Token: 0x04005DE2 RID: 24034
		private bool rightCheckoutPurchaseButtonOn;

		// Token: 0x04005DE3 RID: 24035
		private bool isLastHandTouchedLeft;

		// Token: 0x04005DE4 RID: 24036
		private CosmeticsController.CosmeticSet cachedSet = new CosmeticsController.CosmeticSet();

		// Token: 0x04005DE6 RID: 24038
		public readonly List<WardrobeInstance> wardrobes = new List<WardrobeInstance>();

		// Token: 0x04005DE7 RID: 24039
		public List<CosmeticsController.CosmeticItem> unlockedCosmetics = new List<CosmeticsController.CosmeticItem>(2048);

		// Token: 0x04005DE8 RID: 24040
		public List<CosmeticsController.CosmeticItem> unlockedHats = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04005DE9 RID: 24041
		public List<CosmeticsController.CosmeticItem> unlockedFaces = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04005DEA RID: 24042
		public List<CosmeticsController.CosmeticItem> unlockedBadges = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04005DEB RID: 24043
		public List<CosmeticsController.CosmeticItem> unlockedPaws = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04005DEC RID: 24044
		public List<CosmeticsController.CosmeticItem> unlockedChests = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04005DED RID: 24045
		public List<CosmeticsController.CosmeticItem> unlockedFurs = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04005DEE RID: 24046
		public List<CosmeticsController.CosmeticItem> unlockedShirts = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04005DEF RID: 24047
		public List<CosmeticsController.CosmeticItem> unlockedPants = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04005DF0 RID: 24048
		public List<CosmeticsController.CosmeticItem> unlockedBacks = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04005DF1 RID: 24049
		public List<CosmeticsController.CosmeticItem> unlockedArms = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04005DF2 RID: 24050
		public List<CosmeticsController.CosmeticItem> unlockedTagFX = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04005DF3 RID: 24051
		public List<CosmeticsController.CosmeticItem> unlockedThrowables = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04005DF4 RID: 24052
		public int[] cosmeticsPages = new int[11];

		// Token: 0x04005DF5 RID: 24053
		private List<CosmeticsController.CosmeticItem>[] itemLists = new List<CosmeticsController.CosmeticItem>[11];

		// Token: 0x04005DF6 RID: 24054
		private int wardrobeType;

		// Token: 0x04005DF7 RID: 24055
		[NonSerialized]
		public CosmeticsController.CosmeticSet currentWornSet = new CosmeticsController.CosmeticSet();

		// Token: 0x04005DF8 RID: 24056
		public string concatStringCosmeticsAllowed = "";

		// Token: 0x04005DF9 RID: 24057
		public Action OnCosmeticsUpdated;

		// Token: 0x04005DFA RID: 24058
		public int currencyBalance;

		// Token: 0x04005DFB RID: 24059
		public string currencyName;

		// Token: 0x04005DFC RID: 24060
		public List<CurrencyBoard> currencyBoards;

		// Token: 0x04005DFD RID: 24061
		public string itemToPurchase;

		// Token: 0x04005DFE RID: 24062
		public bool buyingBundle;

		// Token: 0x04005DFF RID: 24063
		public bool confirmedDidntPlayInBeta;

		// Token: 0x04005E00 RID: 24064
		public bool playedInBeta;

		// Token: 0x04005E01 RID: 24065
		public bool gotMyDaily;

		// Token: 0x04005E02 RID: 24066
		public bool checkedDaily;

		// Token: 0x04005E03 RID: 24067
		public string currentPurchaseID;

		// Token: 0x04005E04 RID: 24068
		public bool hasPrice;

		// Token: 0x04005E05 RID: 24069
		private int searchIndex;

		// Token: 0x04005E06 RID: 24070
		private int iterator;

		// Token: 0x04005E07 RID: 24071
		private CosmeticsController.CosmeticItem cosmeticItemVar;

		// Token: 0x04005E08 RID: 24072
		[SerializeField]
		private CosmeticSO m_earlyAccessSupporterPackCosmeticSO;

		// Token: 0x04005E09 RID: 24073
		public EarlyAccessButton[] earlyAccessButtons;

		// Token: 0x04005E0A RID: 24074
		private BundleList bundleList = new BundleList();

		// Token: 0x04005E0B RID: 24075
		public string BundleSkuName = "2024_i_lava_you_pack";

		// Token: 0x04005E0C RID: 24076
		public string BundlePlayfabItemName = "LSABG.";

		// Token: 0x04005E0D RID: 24077
		public int BundleShinyRocks = 10000;

		// Token: 0x04005E0E RID: 24078
		public DateTime currentTime;

		// Token: 0x04005E0F RID: 24079
		public string lastDailyLogin;

		// Token: 0x04005E10 RID: 24080
		public UserDataRecord userDataRecord;

		// Token: 0x04005E11 RID: 24081
		public int secondsUntilTomorrow;

		// Token: 0x04005E12 RID: 24082
		public float secondsToWaitToCheckDaily = 10f;

		// Token: 0x04005E13 RID: 24083
		private int updateCosmeticsRetries;

		// Token: 0x04005E14 RID: 24084
		private int maxUpdateCosmeticsRetries;

		// Token: 0x04005E15 RID: 24085
		private GetUserInventoryResult latestInventory;

		// Token: 0x04005E16 RID: 24086
		private string returnString;

		// Token: 0x04005E17 RID: 24087
		private bool checkoutCartButtonPressedWithLeft;

		// Token: 0x04005E18 RID: 24088
		private Callback<MicroTxnAuthorizationResponse_t> _steamMicroTransactionAuthorizationResponse;

		// Token: 0x04005E19 RID: 24089
		private static readonly List<CosmeticsController.CosmeticSlots> _g_default_outAppliedSlotsList_for_applyCosmeticItemToSet = new List<CosmeticsController.CosmeticSlots>(16);

		// Token: 0x04005E1A RID: 24090
		private List<string> tempUnlocks = new List<string>();

		// Token: 0x02000D48 RID: 3400
		public enum PurchaseItemStages
		{
			// Token: 0x04005E1C RID: 24092
			Start,
			// Token: 0x04005E1D RID: 24093
			CheckoutButtonPressed,
			// Token: 0x04005E1E RID: 24094
			ItemSelected,
			// Token: 0x04005E1F RID: 24095
			ItemOwned,
			// Token: 0x04005E20 RID: 24096
			FinalPurchaseAcknowledgement,
			// Token: 0x04005E21 RID: 24097
			Buying,
			// Token: 0x04005E22 RID: 24098
			Success,
			// Token: 0x04005E23 RID: 24099
			Failure
		}

		// Token: 0x02000D49 RID: 3401
		public enum CosmeticCategory
		{
			// Token: 0x04005E25 RID: 24101
			None,
			// Token: 0x04005E26 RID: 24102
			Hat,
			// Token: 0x04005E27 RID: 24103
			Badge,
			// Token: 0x04005E28 RID: 24104
			Face,
			// Token: 0x04005E29 RID: 24105
			Paw,
			// Token: 0x04005E2A RID: 24106
			Chest,
			// Token: 0x04005E2B RID: 24107
			Fur,
			// Token: 0x04005E2C RID: 24108
			Shirt,
			// Token: 0x04005E2D RID: 24109
			Back,
			// Token: 0x04005E2E RID: 24110
			Arms,
			// Token: 0x04005E2F RID: 24111
			Pants,
			// Token: 0x04005E30 RID: 24112
			TagEffect,
			// Token: 0x04005E31 RID: 24113
			Count,
			// Token: 0x04005E32 RID: 24114
			Set
		}

		// Token: 0x02000D4A RID: 3402
		public enum CosmeticSlots
		{
			// Token: 0x04005E34 RID: 24116
			Hat,
			// Token: 0x04005E35 RID: 24117
			Badge,
			// Token: 0x04005E36 RID: 24118
			Face,
			// Token: 0x04005E37 RID: 24119
			ArmLeft,
			// Token: 0x04005E38 RID: 24120
			ArmRight,
			// Token: 0x04005E39 RID: 24121
			BackLeft,
			// Token: 0x04005E3A RID: 24122
			BackRight,
			// Token: 0x04005E3B RID: 24123
			HandLeft,
			// Token: 0x04005E3C RID: 24124
			HandRight,
			// Token: 0x04005E3D RID: 24125
			Chest,
			// Token: 0x04005E3E RID: 24126
			Fur,
			// Token: 0x04005E3F RID: 24127
			Shirt,
			// Token: 0x04005E40 RID: 24128
			Pants,
			// Token: 0x04005E41 RID: 24129
			Back,
			// Token: 0x04005E42 RID: 24130
			Arms,
			// Token: 0x04005E43 RID: 24131
			TagEffect,
			// Token: 0x04005E44 RID: 24132
			Count
		}

		// Token: 0x02000D4B RID: 3403
		[Serializable]
		public class CosmeticSet
		{
			// Token: 0x1400009F RID: 159
			// (add) Token: 0x060054A0 RID: 21664 RVA: 0x001A339C File Offset: 0x001A159C
			// (remove) Token: 0x060054A1 RID: 21665 RVA: 0x001A33D4 File Offset: 0x001A15D4
			public event CosmeticsController.CosmeticSet.OnSetActivatedHandler onSetActivatedEvent;

			// Token: 0x060054A2 RID: 21666 RVA: 0x001A3409 File Offset: 0x001A1609
			protected void OnSetActivated(CosmeticsController.CosmeticSet prevSet, CosmeticsController.CosmeticSet currentSet, NetPlayer netPlayer)
			{
				if (this.onSetActivatedEvent != null)
				{
					this.onSetActivatedEvent(prevSet, currentSet, netPlayer);
				}
			}

			// Token: 0x17000821 RID: 2081
			// (get) Token: 0x060054A3 RID: 21667 RVA: 0x001A3424 File Offset: 0x001A1624
			public static CosmeticsController.CosmeticSet EmptySet
			{
				get
				{
					if (CosmeticsController.CosmeticSet._emptySet == null)
					{
						string[] array = new string[16];
						for (int i = 0; i < array.Length; i++)
						{
							array[i] = "NOTHING";
						}
						CosmeticsController.CosmeticSet._emptySet = new CosmeticsController.CosmeticSet(array, CosmeticsController.instance);
					}
					return CosmeticsController.CosmeticSet._emptySet;
				}
			}

			// Token: 0x060054A4 RID: 21668 RVA: 0x001A346D File Offset: 0x001A166D
			public CosmeticSet()
			{
				this.items = new CosmeticsController.CosmeticItem[16];
			}

			// Token: 0x060054A5 RID: 21669 RVA: 0x001A3490 File Offset: 0x001A1690
			public CosmeticSet(string[] itemNames, CosmeticsController controller)
			{
				this.items = new CosmeticsController.CosmeticItem[16];
				for (int i = 0; i < itemNames.Length; i++)
				{
					string displayName = itemNames[i];
					string itemNameFromDisplayName = controller.GetItemNameFromDisplayName(displayName);
					this.items[i] = controller.GetItemFromDict(itemNameFromDisplayName);
				}
			}

			// Token: 0x060054A6 RID: 21670 RVA: 0x001A34EC File Offset: 0x001A16EC
			public CosmeticSet(int[] itemNamesPacked, CosmeticsController controller)
			{
				this.items = new CosmeticsController.CosmeticItem[16];
				int num = (itemNamesPacked.Length != 0) ? itemNamesPacked[0] : 0;
				int num2 = 1;
				for (int i = 0; i < this.items.Length; i++)
				{
					if ((num & 1 << i) != 0)
					{
						int num3 = itemNamesPacked[num2];
						CosmeticsController.CosmeticSet.nameScratchSpace[0] = (char)(65 + num3 % 26);
						CosmeticsController.CosmeticSet.nameScratchSpace[1] = (char)(65 + num3 / 26 % 26);
						CosmeticsController.CosmeticSet.nameScratchSpace[2] = (char)(65 + num3 / 676 % 26);
						CosmeticsController.CosmeticSet.nameScratchSpace[3] = (char)(65 + num3 / 17576 % 26);
						CosmeticsController.CosmeticSet.nameScratchSpace[4] = (char)(65 + num3 / 456976 % 26);
						CosmeticsController.CosmeticSet.nameScratchSpace[5] = '.';
						this.items[i] = controller.GetItemFromDict(new string(CosmeticsController.CosmeticSet.nameScratchSpace));
						num2++;
					}
					else
					{
						this.items[i] = controller.GetItemFromDict("null");
					}
				}
			}

			// Token: 0x060054A7 RID: 21671 RVA: 0x001A35F4 File Offset: 0x001A17F4
			public void CopyItems(CosmeticsController.CosmeticSet other)
			{
				for (int i = 0; i < this.items.Length; i++)
				{
					this.items[i] = other.items[i];
				}
			}

			// Token: 0x060054A8 RID: 21672 RVA: 0x001A362C File Offset: 0x001A182C
			public void MergeSets(CosmeticsController.CosmeticSet tryOn, CosmeticsController.CosmeticSet current)
			{
				for (int i = 0; i < 16; i++)
				{
					if (tryOn == null)
					{
						this.items[i] = current.items[i];
					}
					else
					{
						this.items[i] = (tryOn.items[i].isNullItem ? current.items[i] : tryOn.items[i]);
					}
				}
			}

			// Token: 0x060054A9 RID: 21673 RVA: 0x001A369C File Offset: 0x001A189C
			public void ClearSet(CosmeticsController.CosmeticItem nullItem)
			{
				for (int i = 0; i < 16; i++)
				{
					this.items[i] = nullItem;
				}
			}

			// Token: 0x060054AA RID: 21674 RVA: 0x001A36C4 File Offset: 0x001A18C4
			public bool IsActive(string name)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					if (this.items[i].displayName == name)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x060054AB RID: 21675 RVA: 0x001A36FC File Offset: 0x001A18FC
			public bool HasItemOfCategory(CosmeticsController.CosmeticCategory category)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					if (!this.items[i].isNullItem && this.items[i].itemCategory == category)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x060054AC RID: 21676 RVA: 0x001A3744 File Offset: 0x001A1944
			public bool HasItem(string name)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					if (!this.items[i].isNullItem && this.items[i].displayName == name)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x060054AD RID: 21677 RVA: 0x001A378F File Offset: 0x001A198F
			public static bool IsSlotLeftHanded(CosmeticsController.CosmeticSlots slot)
			{
				return slot == CosmeticsController.CosmeticSlots.ArmLeft || slot == CosmeticsController.CosmeticSlots.BackLeft || slot == CosmeticsController.CosmeticSlots.HandLeft;
			}

			// Token: 0x060054AE RID: 21678 RVA: 0x001A379F File Offset: 0x001A199F
			public static bool IsSlotRightHanded(CosmeticsController.CosmeticSlots slot)
			{
				return slot == CosmeticsController.CosmeticSlots.ArmRight || slot == CosmeticsController.CosmeticSlots.BackRight || slot == CosmeticsController.CosmeticSlots.HandRight;
			}

			// Token: 0x060054AF RID: 21679 RVA: 0x001A37AF File Offset: 0x001A19AF
			public static bool IsHoldable(CosmeticsController.CosmeticItem item)
			{
				return item.isHoldable;
			}

			// Token: 0x060054B0 RID: 21680 RVA: 0x001A37B8 File Offset: 0x001A19B8
			public static CosmeticsController.CosmeticSlots OppositeSlot(CosmeticsController.CosmeticSlots slot)
			{
				switch (slot)
				{
				case CosmeticsController.CosmeticSlots.Hat:
					return CosmeticsController.CosmeticSlots.Hat;
				case CosmeticsController.CosmeticSlots.Badge:
					return CosmeticsController.CosmeticSlots.Badge;
				case CosmeticsController.CosmeticSlots.Face:
					return CosmeticsController.CosmeticSlots.Face;
				case CosmeticsController.CosmeticSlots.ArmLeft:
					return CosmeticsController.CosmeticSlots.ArmRight;
				case CosmeticsController.CosmeticSlots.ArmRight:
					return CosmeticsController.CosmeticSlots.ArmLeft;
				case CosmeticsController.CosmeticSlots.BackLeft:
					return CosmeticsController.CosmeticSlots.BackRight;
				case CosmeticsController.CosmeticSlots.BackRight:
					return CosmeticsController.CosmeticSlots.BackLeft;
				case CosmeticsController.CosmeticSlots.HandLeft:
					return CosmeticsController.CosmeticSlots.HandRight;
				case CosmeticsController.CosmeticSlots.HandRight:
					return CosmeticsController.CosmeticSlots.HandLeft;
				case CosmeticsController.CosmeticSlots.Chest:
					return CosmeticsController.CosmeticSlots.Chest;
				case CosmeticsController.CosmeticSlots.Fur:
					return CosmeticsController.CosmeticSlots.Fur;
				case CosmeticsController.CosmeticSlots.Shirt:
					return CosmeticsController.CosmeticSlots.Shirt;
				case CosmeticsController.CosmeticSlots.Pants:
					return CosmeticsController.CosmeticSlots.Pants;
				case CosmeticsController.CosmeticSlots.Back:
					return CosmeticsController.CosmeticSlots.Back;
				case CosmeticsController.CosmeticSlots.Arms:
					return CosmeticsController.CosmeticSlots.Arms;
				case CosmeticsController.CosmeticSlots.TagEffect:
					return CosmeticsController.CosmeticSlots.TagEffect;
				default:
					return CosmeticsController.CosmeticSlots.Count;
				}
			}

			// Token: 0x060054B1 RID: 21681 RVA: 0x001A3836 File Offset: 0x001A1A36
			public static string SlotPlayerPreferenceName(CosmeticsController.CosmeticSlots slot)
			{
				return "slot_" + slot.ToString();
			}

			// Token: 0x060054B2 RID: 21682 RVA: 0x001A3850 File Offset: 0x001A1A50
			private void ActivateCosmetic(CosmeticsController.CosmeticSet prevSet, VRRig rig, int slotIndex, CosmeticItemRegistry cosmeticsObjectRegistry, BodyDockPositions bDock)
			{
				CosmeticsController.CosmeticItem cosmeticItem = prevSet.items[slotIndex];
				string itemNameFromDisplayName = CosmeticsController.instance.GetItemNameFromDisplayName(cosmeticItem.displayName);
				CosmeticsController.CosmeticItem cosmeticItem2 = this.items[slotIndex];
				string itemNameFromDisplayName2 = CosmeticsController.instance.GetItemNameFromDisplayName(cosmeticItem2.displayName);
				BodyDockPositions.DropPositions dropPositions = CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)slotIndex);
				if (cosmeticItem2.itemCategory != CosmeticsController.CosmeticCategory.None && !CosmeticsController.CompareCategoryToSavedCosmeticSlots(cosmeticItem2.itemCategory, (CosmeticsController.CosmeticSlots)slotIndex))
				{
					return;
				}
				if (cosmeticItem2.isHoldable && dropPositions == BodyDockPositions.DropPositions.None)
				{
					return;
				}
				if (!(itemNameFromDisplayName == itemNameFromDisplayName2))
				{
					if (!cosmeticItem.isNullItem)
					{
						if (cosmeticItem.isHoldable)
						{
							bDock.TransferrableItemDisableAtPosition(dropPositions);
						}
						CosmeticItemInstance cosmeticItemInstance = cosmeticsObjectRegistry.Cosmetic(cosmeticItem.displayName);
						if (cosmeticItemInstance != null)
						{
							cosmeticItemInstance.DisableItem((CosmeticsController.CosmeticSlots)slotIndex);
						}
					}
					if (!cosmeticItem2.isNullItem)
					{
						if (cosmeticItem2.isHoldable)
						{
							bDock.TransferrableItemEnableAtPosition(cosmeticItem2.displayName, dropPositions);
						}
						CosmeticItemInstance cosmeticItemInstance2 = cosmeticsObjectRegistry.Cosmetic(cosmeticItem2.displayName);
						if (rig.IsItemAllowed(itemNameFromDisplayName2) && cosmeticItemInstance2 != null)
						{
							cosmeticItemInstance2.EnableItem((CosmeticsController.CosmeticSlots)slotIndex, rig);
						}
					}
					return;
				}
				if (cosmeticItem2.isNullItem)
				{
					return;
				}
				CosmeticItemInstance cosmeticItemInstance3 = cosmeticsObjectRegistry.Cosmetic(cosmeticItem2.displayName);
				if (cosmeticItemInstance3 != null)
				{
					if (!rig.IsItemAllowed(itemNameFromDisplayName2))
					{
						cosmeticItemInstance3.DisableItem((CosmeticsController.CosmeticSlots)slotIndex);
						return;
					}
					cosmeticItemInstance3.EnableItem((CosmeticsController.CosmeticSlots)slotIndex, rig);
				}
			}

			// Token: 0x060054B3 RID: 21683 RVA: 0x001A398C File Offset: 0x001A1B8C
			public void ActivateCosmetics(CosmeticsController.CosmeticSet prevSet, VRRig rig, BodyDockPositions bDock, CosmeticItemRegistry cosmeticsObjectRegistry)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					this.ActivateCosmetic(prevSet, rig, i, cosmeticsObjectRegistry, bDock);
				}
				this.OnSetActivated(prevSet, this, rig.creator);
			}

			// Token: 0x060054B4 RID: 21684 RVA: 0x001A39C4 File Offset: 0x001A1BC4
			public void DeactivateAllCosmetcs(BodyDockPositions bDock, CosmeticsController.CosmeticItem nullItem, CosmeticItemRegistry cosmeticObjectRegistry)
			{
				bDock.DisableAllTransferableItems();
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					CosmeticsController.CosmeticItem cosmeticItem = this.items[i];
					if (!cosmeticItem.isNullItem)
					{
						CosmeticsController.CosmeticSlots cosmeticSlot = (CosmeticsController.CosmeticSlots)i;
						CosmeticItemInstance cosmeticItemInstance = cosmeticObjectRegistry.Cosmetic(cosmeticItem.displayName);
						if (cosmeticItemInstance != null)
						{
							cosmeticItemInstance.DisableItem(cosmeticSlot);
						}
						this.items[i] = nullItem;
					}
				}
			}

			// Token: 0x060054B5 RID: 21685 RVA: 0x001A3A24 File Offset: 0x001A1C24
			public void LoadFromPlayerPreferences(CosmeticsController controller)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					CosmeticsController.CosmeticSlots slot = (CosmeticsController.CosmeticSlots)i;
					string @string = PlayerPrefs.GetString(CosmeticsController.CosmeticSet.SlotPlayerPreferenceName(slot), "NOTHING");
					if (@string == "null" || @string == "NOTHING")
					{
						this.items[i] = controller.nullItem;
					}
					else
					{
						CosmeticsController.CosmeticItem item = controller.GetItemFromDict(@string);
						if (item.isNullItem)
						{
							Debug.Log("LoadFromPlayerPreferences: Could not find item stored in player prefs: \"" + @string + "\"");
							this.items[i] = controller.nullItem;
						}
						else if (!CosmeticsController.CompareCategoryToSavedCosmeticSlots(item.itemCategory, slot))
						{
							this.items[i] = controller.nullItem;
						}
						else if (controller.unlockedCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => item.itemName == x.itemName) >= 0)
						{
							this.items[i] = item;
						}
						else
						{
							this.items[i] = controller.nullItem;
						}
					}
				}
			}

			// Token: 0x060054B6 RID: 21686 RVA: 0x001A3B40 File Offset: 0x001A1D40
			public string[] ToDisplayNameArray()
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					this.returnArray[i] = (string.IsNullOrEmpty(this.items[i].displayName) ? "null" : this.items[i].displayName);
				}
				return this.returnArray;
			}

			// Token: 0x060054B7 RID: 21687 RVA: 0x001A3B9C File Offset: 0x001A1D9C
			public int[] ToPackedIDArray()
			{
				int num = 0;
				int num2 = 0;
				int num3 = 16;
				for (int i = 0; i < num3; i++)
				{
					if (!this.items[i].isNullItem && this.items[i].itemName.Length == 6)
					{
						num |= 1 << i;
						num2++;
					}
				}
				if (num == 0)
				{
					return CosmeticsController.CosmeticSet.intArrays[0];
				}
				int[] array = CosmeticsController.CosmeticSet.intArrays[num2 + 1];
				array[0] = num;
				int num4 = 1;
				for (int j = 0; j < num3; j++)
				{
					if ((num & 1 << j) != 0)
					{
						string itemName = this.items[j].itemName;
						array[num4] = (int)(itemName[0] - 'A' + '\u001a' * (itemName[1] - 'A' + '\u001a' * (itemName[2] - 'A' + '\u001a' * (itemName[3] - 'A' + '\u001a' * (itemName[4] - 'A')))));
						num4++;
					}
				}
				return array;
			}

			// Token: 0x060054B8 RID: 21688 RVA: 0x001A3C9C File Offset: 0x001A1E9C
			public string[] HoldableDisplayNames(bool leftHoldables)
			{
				int num = 16;
				int num2 = 0;
				for (int i = 0; i < num; i++)
				{
					if (this.items[i].isHoldable && this.items[i].isHoldable && this.items[i].itemCategory != CosmeticsController.CosmeticCategory.Chest)
					{
						if (leftHoldables && BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)i)))
						{
							num2++;
						}
						else if (!leftHoldables && !BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)i)))
						{
							num2++;
						}
					}
				}
				if (num2 == 0)
				{
					return null;
				}
				int num3 = 0;
				string[] array = new string[num2];
				for (int j = 0; j < num; j++)
				{
					if (this.items[j].isHoldable)
					{
						if (leftHoldables && BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)j)))
						{
							array[num3] = this.items[j].displayName;
							num3++;
						}
						else if (!leftHoldables && !BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)j)))
						{
							array[num3] = this.items[j].displayName;
							num3++;
						}
					}
				}
				return array;
			}

			// Token: 0x060054B9 RID: 21689 RVA: 0x001A3DB0 File Offset: 0x001A1FB0
			public bool[] ToOnRightSideArray()
			{
				int num = 16;
				bool[] array = new bool[num];
				for (int i = 0; i < num; i++)
				{
					if (this.items[i].isHoldable && this.items[i].itemCategory != CosmeticsController.CosmeticCategory.Chest)
					{
						array[i] = !BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)i));
					}
					else
					{
						array[i] = false;
					}
				}
				return array;
			}

			// Token: 0x04005E45 RID: 24133
			public CosmeticsController.CosmeticItem[] items;

			// Token: 0x04005E47 RID: 24135
			public string[] returnArray = new string[16];

			// Token: 0x04005E48 RID: 24136
			private static int[][] intArrays = new int[][]
			{
				new int[0],
				new int[1],
				new int[2],
				new int[3],
				new int[4],
				new int[5],
				new int[6],
				new int[7],
				new int[8],
				new int[9],
				new int[10],
				new int[11],
				new int[12],
				new int[13],
				new int[14],
				new int[15],
				new int[16],
				new int[17],
				new int[18],
				new int[19],
				new int[20],
				new int[21]
			};

			// Token: 0x04005E49 RID: 24137
			private static CosmeticsController.CosmeticSet _emptySet;

			// Token: 0x04005E4A RID: 24138
			private static char[] nameScratchSpace = new char[6];

			// Token: 0x02000D4C RID: 3404
			// (Invoke) Token: 0x060054BC RID: 21692
			public delegate void OnSetActivatedHandler(CosmeticsController.CosmeticSet prevSet, CosmeticsController.CosmeticSet currentSet, NetPlayer netPlayer);
		}

		// Token: 0x02000D4E RID: 3406
		[Serializable]
		public struct CosmeticItem
		{
			// Token: 0x04005E4C RID: 24140
			[Tooltip("Should match the spreadsheet item name.")]
			public string itemName;

			// Token: 0x04005E4D RID: 24141
			[Tooltip("Determines what wardrobe section the item will show up in.")]
			public CosmeticsController.CosmeticCategory itemCategory;

			// Token: 0x04005E4E RID: 24142
			[Tooltip("If this is a holdable item.")]
			public bool isHoldable;

			// Token: 0x04005E4F RID: 24143
			[Tooltip("If this is a throwable item and hidden on the wardrobe.")]
			public bool isThrowable;

			// Token: 0x04005E50 RID: 24144
			[Tooltip("Icon shown in the store menus & hunt watch.")]
			public Sprite itemPicture;

			// Token: 0x04005E51 RID: 24145
			public string displayName;

			// Token: 0x04005E52 RID: 24146
			public string itemPictureResourceString;

			// Token: 0x04005E53 RID: 24147
			[Tooltip("The name shown on the store checkout screen.")]
			public string overrideDisplayName;

			// Token: 0x04005E54 RID: 24148
			[DebugReadout]
			[NonSerialized]
			public int cost;

			// Token: 0x04005E55 RID: 24149
			[DebugReadout]
			[NonSerialized]
			public string[] bundledItems;

			// Token: 0x04005E56 RID: 24150
			[DebugReadout]
			[NonSerialized]
			public bool canTryOn;

			// Token: 0x04005E57 RID: 24151
			[Tooltip("Set to true if the item takes up both left and right wearable hand slots at the same time. Used for things like mittens/gloves.")]
			public bool bothHandsHoldable;

			// Token: 0x04005E58 RID: 24152
			public bool bLoadsFromResources;

			// Token: 0x04005E59 RID: 24153
			public bool bUsesMeshAtlas;

			// Token: 0x04005E5A RID: 24154
			public Vector3 rotationOffset;

			// Token: 0x04005E5B RID: 24155
			public Vector3 positionOffset;

			// Token: 0x04005E5C RID: 24156
			public string meshAtlasResourceString;

			// Token: 0x04005E5D RID: 24157
			public string meshResourceString;

			// Token: 0x04005E5E RID: 24158
			public string materialResourceString;

			// Token: 0x04005E5F RID: 24159
			[HideInInspector]
			public bool isNullItem;
		}

		// Token: 0x02000D4F RID: 3407
		[Serializable]
		public class IAPRequestBody
		{
			// Token: 0x04005E60 RID: 24160
			public string userID;

			// Token: 0x04005E61 RID: 24161
			public string nonce;

			// Token: 0x04005E62 RID: 24162
			public string platform;

			// Token: 0x04005E63 RID: 24163
			public string sku;

			// Token: 0x04005E64 RID: 24164
			public Dictionary<string, string> customTags;
		}

		// Token: 0x02000D50 RID: 3408
		public enum EWearingCosmeticSet
		{
			// Token: 0x04005E66 RID: 24166
			NotASet,
			// Token: 0x04005E67 RID: 24167
			NotWearing,
			// Token: 0x04005E68 RID: 24168
			Partial,
			// Token: 0x04005E69 RID: 24169
			Complete
		}
	}
}
