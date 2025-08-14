using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000553 RID: 1363
public class BuilderSetManager : MonoBehaviour
{
	// Token: 0x17000356 RID: 854
	// (get) Token: 0x06002141 RID: 8513 RVA: 0x000B4434 File Offset: 0x000B2634
	// (set) Token: 0x06002142 RID: 8514 RVA: 0x000B443B File Offset: 0x000B263B
	public static bool hasInstance { get; private set; }

	// Token: 0x06002143 RID: 8515 RVA: 0x000B4444 File Offset: 0x000B2644
	public string GetStarterSetsConcat()
	{
		if (BuilderSetManager.concatStarterSets.Length > 0)
		{
			return BuilderSetManager.concatStarterSets;
		}
		BuilderSetManager.concatStarterSets = string.Empty;
		foreach (BuilderPieceSet builderPieceSet in this._starterPieceSets)
		{
			BuilderSetManager.concatStarterSets += builderPieceSet.playfabID;
		}
		return BuilderSetManager.concatStarterSets;
	}

	// Token: 0x06002144 RID: 8516 RVA: 0x000B44C8 File Offset: 0x000B26C8
	public string GetAllSetsConcat()
	{
		if (BuilderSetManager.concatAllSets.Length > 0)
		{
			return BuilderSetManager.concatAllSets;
		}
		BuilderSetManager.concatAllSets = string.Empty;
		foreach (BuilderPieceSet builderPieceSet in this._allPieceSets)
		{
			BuilderSetManager.concatAllSets += builderPieceSet.playfabID;
		}
		return BuilderSetManager.concatAllSets;
	}

	// Token: 0x06002145 RID: 8517 RVA: 0x000B454C File Offset: 0x000B274C
	public void Awake()
	{
		if (BuilderSetManager.instance == null)
		{
			BuilderSetManager.instance = this;
			BuilderSetManager.hasInstance = true;
		}
		else if (BuilderSetManager.instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		this.Init();
		if (this.monitor == null)
		{
			this.monitor = base.StartCoroutine(this.MonitorTime());
		}
	}

	// Token: 0x06002146 RID: 8518 RVA: 0x000B45B4 File Offset: 0x000B27B4
	private void Init()
	{
		this.InitPieceDictionary();
		this.catalog = "DLC";
		this.currencyName = "SR";
		this.pulledStoreItems = false;
		BuilderSetManager._setIdToStoreItem = new Dictionary<int, BuilderSetManager.BuilderSetStoreItem>(this._allPieceSets.Count);
		BuilderSetManager._setIdToStoreItem.Clear();
		BuilderSetManager.pieceSetInfos = new List<BuilderSetManager.BuilderPieceSetInfo>(this._allPieceSets.Count * 45);
		BuilderSetManager.pieceSetInfoMap = new Dictionary<int, int>(this._allPieceSets.Count * 45);
		this.livePieceSets = new List<BuilderPieceSet>(this._allPieceSets.Count);
		this.scheduledPieceSets = new List<BuilderPieceSet>(this._allPieceSets.Count);
		foreach (BuilderPieceSet builderPieceSet in this._allPieceSets)
		{
			BuilderSetManager.BuilderSetStoreItem value = new BuilderSetManager.BuilderSetStoreItem
			{
				displayName = builderPieceSet.setName,
				playfabID = builderPieceSet.playfabID,
				setID = builderPieceSet.GetIntIdentifier(),
				cost = 0U,
				setRef = builderPieceSet,
				displayModel = builderPieceSet.displayModel,
				isNullItem = false
			};
			BuilderSetManager._setIdToStoreItem.TryAdd(builderPieceSet.GetIntIdentifier(), value);
			int num = -1;
			if (!string.IsNullOrEmpty(builderPieceSet.materialId))
			{
				num = builderPieceSet.materialId.GetHashCode();
			}
			for (int i = 0; i < builderPieceSet.subsets.Count; i++)
			{
				BuilderPieceSet.BuilderPieceSubset builderPieceSubset = builderPieceSet.subsets[i];
				for (int j = 0; j < builderPieceSubset.pieceInfos.Count; j++)
				{
					BuilderPiece piecePrefab = builderPieceSubset.pieceInfos[j].piecePrefab;
					int staticHash = piecePrefab.name.GetStaticHash();
					int pieceMaterial = num;
					if (piecePrefab.materialOptions == null)
					{
						pieceMaterial = -1;
						this.AddPieceToInfoMap(staticHash, pieceMaterial, builderPieceSet.GetIntIdentifier());
					}
					else if (builderPieceSubset.pieceInfos[j].overrideSetMaterial)
					{
						if (builderPieceSubset.pieceInfos[j].pieceMaterialTypes.Length == 0)
						{
							Debug.LogErrorFormat("Material List for piece {0} in set {1} is empty", new object[]
							{
								piecePrefab.name,
								builderPieceSet.setName
							});
						}
						foreach (string text in builderPieceSubset.pieceInfos[j].pieceMaterialTypes)
						{
							if (string.IsNullOrEmpty(text))
							{
								Debug.LogErrorFormat("Material List Entry for piece {0} in set {1} is empty", new object[]
								{
									piecePrefab.name,
									builderPieceSet.setName
								});
							}
							else
							{
								pieceMaterial = text.GetHashCode();
								this.AddPieceToInfoMap(staticHash, pieceMaterial, builderPieceSet.GetIntIdentifier());
							}
						}
					}
					else
					{
						Material x;
						int num2;
						piecePrefab.materialOptions.GetMaterialFromType(num, out x, out num2);
						if (x == null)
						{
							pieceMaterial = -1;
						}
						this.AddPieceToInfoMap(staticHash, pieceMaterial, builderPieceSet.GetIntIdentifier());
					}
				}
			}
			if (!builderPieceSet.isScheduled)
			{
				this.livePieceSets.Add(builderPieceSet);
			}
			else
			{
				this.scheduledPieceSets.Add(builderPieceSet);
			}
		}
		this._unlockedPieceSets = new List<BuilderPieceSet>(this._allPieceSets.Count);
		this._unlockedPieceSets.AddRange(this._starterPieceSets);
	}

	// Token: 0x06002147 RID: 8519 RVA: 0x000B4914 File Offset: 0x000B2B14
	public void InitPieceDictionary()
	{
		if (this.hasPieceDictionary)
		{
			return;
		}
		BuilderSetManager.pieceTypes = new List<int>(256);
		BuilderSetManager.pieceList = new List<BuilderPiece>(256);
		BuilderSetManager.pieceTypeToIndex = new Dictionary<int, int>(256);
		int num = 0;
		foreach (BuilderPieceSet builderPieceSet in this._allPieceSets)
		{
			foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in builderPieceSet.subsets)
			{
				foreach (BuilderPieceSet.PieceInfo pieceInfo in builderPieceSubset.pieceInfos)
				{
					int staticHash = pieceInfo.piecePrefab.name.GetStaticHash();
					if (!BuilderSetManager.pieceTypeToIndex.ContainsKey(staticHash))
					{
						BuilderSetManager.pieceList.Add(pieceInfo.piecePrefab);
						BuilderSetManager.pieceTypes.Add(staticHash);
						BuilderSetManager.pieceTypeToIndex.Add(staticHash, num);
						num++;
					}
				}
			}
		}
		this.hasPieceDictionary = true;
	}

	// Token: 0x06002148 RID: 8520 RVA: 0x000B4A70 File Offset: 0x000B2C70
	public BuilderPiece GetPiecePrefab(int pieceType)
	{
		int index;
		if (BuilderSetManager.pieceTypeToIndex.TryGetValue(pieceType, out index))
		{
			return BuilderSetManager.pieceList[index];
		}
		Debug.LogErrorFormat("No Prefab found for type {0}", new object[]
		{
			pieceType
		});
		return null;
	}

	// Token: 0x06002149 RID: 8521 RVA: 0x000B4AB2 File Offset: 0x000B2CB2
	private void OnEnable()
	{
		if (this.monitor == null && this.scheduledPieceSets.Count > 0)
		{
			this.monitor = base.StartCoroutine(this.MonitorTime());
		}
	}

	// Token: 0x0600214A RID: 8522 RVA: 0x000B4ADC File Offset: 0x000B2CDC
	private void OnDisable()
	{
		if (this.monitor != null)
		{
			base.StopCoroutine(this.monitor);
		}
		this.monitor = null;
	}

	// Token: 0x0600214B RID: 8523 RVA: 0x000B4AF9 File Offset: 0x000B2CF9
	private IEnumerator MonitorTime()
	{
		while (GorillaComputer.instance == null || GorillaComputer.instance.startupMillis == 0L)
		{
			yield return null;
		}
		while (this.scheduledPieceSets.Count > 0)
		{
			bool flag = false;
			for (int i = this.scheduledPieceSets.Count - 1; i >= 0; i--)
			{
				BuilderPieceSet builderPieceSet = this.scheduledPieceSets[i];
				if (GorillaComputer.instance.GetServerTime() > builderPieceSet.GetScheduleDateTime())
				{
					flag = true;
					this.livePieceSets.Add(builderPieceSet);
					this.scheduledPieceSets.RemoveAt(i);
				}
			}
			if (flag)
			{
				this.OnLiveSetsUpdated.Invoke();
			}
			yield return new WaitForSeconds(60f);
		}
		this.monitor = null;
		yield break;
	}

	// Token: 0x0600214C RID: 8524 RVA: 0x000B4B08 File Offset: 0x000B2D08
	private void AddPieceToInfoMap(int pieceType, int pieceMaterial, int setID)
	{
		int index;
		if (BuilderSetManager.pieceSetInfoMap.TryGetValue(HashCode.Combine<int, int>(pieceType, pieceMaterial), out index))
		{
			BuilderSetManager.BuilderPieceSetInfo builderPieceSetInfo = BuilderSetManager.pieceSetInfos[index];
			if (!builderPieceSetInfo.setIds.Contains(setID))
			{
				builderPieceSetInfo.setIds.Add(setID);
			}
			BuilderSetManager.pieceSetInfos[index] = builderPieceSetInfo;
			return;
		}
		BuilderSetManager.BuilderPieceSetInfo item = new BuilderSetManager.BuilderPieceSetInfo
		{
			pieceType = pieceType,
			materialType = pieceMaterial,
			setIds = new List<int>
			{
				setID
			}
		};
		BuilderSetManager.pieceSetInfoMap.Add(HashCode.Combine<int, int>(pieceType, pieceMaterial), BuilderSetManager.pieceSetInfos.Count);
		BuilderSetManager.pieceSetInfos.Add(item);
	}

	// Token: 0x0600214D RID: 8525 RVA: 0x000B4BB0 File Offset: 0x000B2DB0
	public static bool IsItemIDBuilderItem(string playfabID)
	{
		return BuilderSetManager.instance.GetAllSetsConcat().Contains(playfabID);
	}

	// Token: 0x0600214E RID: 8526 RVA: 0x000B4BC4 File Offset: 0x000B2DC4
	public void OnGotInventoryItems(GetUserInventoryResult inventoryResult, GetCatalogItemsResult catalogResult)
	{
		CosmeticsController cosmeticsController = CosmeticsController.instance;
		cosmeticsController.concatStringCosmeticsAllowed += this.GetStarterSetsConcat();
		this._unlockedPieceSets.Clear();
		this._unlockedPieceSets.AddRange(this._starterPieceSets);
		foreach (CatalogItem catalogItem in catalogResult.Catalog)
		{
			BuilderSetManager.BuilderSetStoreItem builderSetStoreItem;
			if (BuilderSetManager.IsItemIDBuilderItem(catalogItem.ItemId) && BuilderSetManager._setIdToStoreItem.TryGetValue(catalogItem.ItemId.GetStaticHash(), out builderSetStoreItem))
			{
				bool hasPrice = false;
				uint cost = 0U;
				if (catalogItem.VirtualCurrencyPrices.TryGetValue(this.currencyName, out cost))
				{
					hasPrice = true;
				}
				builderSetStoreItem.playfabID = catalogItem.ItemId;
				builderSetStoreItem.cost = cost;
				builderSetStoreItem.hasPrice = hasPrice;
				BuilderSetManager._setIdToStoreItem[builderSetStoreItem.setRef.GetIntIdentifier()] = builderSetStoreItem;
			}
		}
		foreach (ItemInstance itemInstance in inventoryResult.Inventory)
		{
			if (BuilderSetManager.IsItemIDBuilderItem(itemInstance.ItemId))
			{
				BuilderSetManager.BuilderSetStoreItem builderSetStoreItem2;
				if (BuilderSetManager._setIdToStoreItem.TryGetValue(itemInstance.ItemId.GetStaticHash(), out builderSetStoreItem2))
				{
					Debug.LogFormat("BuilderSetManager: Unlocking Inventory Item {0}", new object[]
					{
						itemInstance.ItemId
					});
					this._unlockedPieceSets.Add(builderSetStoreItem2.setRef);
					CosmeticsController cosmeticsController2 = CosmeticsController.instance;
					cosmeticsController2.concatStringCosmeticsAllowed += itemInstance.ItemId;
				}
				else
				{
					Debug.Log("BuilderSetManager: No store item found with id" + itemInstance.ItemId);
				}
			}
		}
		this.pulledStoreItems = true;
		UnityEvent onOwnedSetsUpdated = this.OnOwnedSetsUpdated;
		if (onOwnedSetsUpdated == null)
		{
			return;
		}
		onOwnedSetsUpdated.Invoke();
	}

	// Token: 0x0600214F RID: 8527 RVA: 0x000B4DA8 File Offset: 0x000B2FA8
	public BuilderSetManager.BuilderSetStoreItem GetStoreItemFromSetID(int setID)
	{
		return BuilderSetManager._setIdToStoreItem.GetValueOrDefault(setID, BuilderKiosk.nullItem);
	}

	// Token: 0x06002150 RID: 8528 RVA: 0x000B4DBC File Offset: 0x000B2FBC
	public BuilderPieceSet GetPieceSetFromID(int setID)
	{
		BuilderSetManager.BuilderSetStoreItem builderSetStoreItem;
		if (BuilderSetManager._setIdToStoreItem.TryGetValue(setID, out builderSetStoreItem))
		{
			return builderSetStoreItem.setRef;
		}
		return null;
	}

	// Token: 0x06002151 RID: 8529 RVA: 0x000B4DE0 File Offset: 0x000B2FE0
	public List<BuilderPieceSet> GetAllPieceSets()
	{
		return this._allPieceSets;
	}

	// Token: 0x06002152 RID: 8530 RVA: 0x000B4DE8 File Offset: 0x000B2FE8
	public List<BuilderPieceSet> GetLivePieceSets()
	{
		return this.livePieceSets;
	}

	// Token: 0x06002153 RID: 8531 RVA: 0x000B4DF0 File Offset: 0x000B2FF0
	public List<BuilderPieceSet> GetUnlockedPieceSets()
	{
		return this._unlockedPieceSets;
	}

	// Token: 0x06002154 RID: 8532 RVA: 0x000B4DF8 File Offset: 0x000B2FF8
	public List<BuilderPieceSet> GetPermanentSetsForSale()
	{
		return this._setsAlwaysForSale;
	}

	// Token: 0x06002155 RID: 8533 RVA: 0x000B4E00 File Offset: 0x000B3000
	public List<BuilderPieceSet> GetSeasonalSetsForSale()
	{
		return this._seasonalSetsForSale;
	}

	// Token: 0x06002156 RID: 8534 RVA: 0x000B4E08 File Offset: 0x000B3008
	public bool IsSetSeasonal(string playfabID)
	{
		return !this._seasonalSetsForSale.IsNullOrEmpty<BuilderPieceSet>() && this._seasonalSetsForSale.FindIndex((BuilderPieceSet x) => x.playfabID.Equals(playfabID)) >= 0;
	}

	// Token: 0x06002157 RID: 8535 RVA: 0x000B4E50 File Offset: 0x000B3050
	public bool DoesPlayerOwnPieceSet(Player player, int setID)
	{
		BuilderPieceSet pieceSetFromID = this.GetPieceSetFromID(setID);
		if (pieceSetFromID == null)
		{
			return false;
		}
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			bool flag = rigContainer.Rig.IsItemAllowed(pieceSetFromID.playfabID);
			Debug.LogFormat("BuilderSetManager: does player {0} own set {1} {2}", new object[]
			{
				player.ActorNumber,
				pieceSetFromID.setName,
				flag
			});
			return flag;
		}
		Debug.LogFormat("BuilderSetManager: could not get rig for player {0}", new object[]
		{
			player.ActorNumber
		});
		return false;
	}

	// Token: 0x06002158 RID: 8536 RVA: 0x000B4EE4 File Offset: 0x000B30E4
	public bool DoesAnyPlayerInRoomOwnPieceSet(int setID)
	{
		BuilderPieceSet pieceSetFromID = this.GetPieceSetFromID(setID);
		if (pieceSetFromID == null)
		{
			return false;
		}
		if (this.GetStarterSetsConcat().Contains(pieceSetFromID.setName))
		{
			return true;
		}
		foreach (NetPlayer targetPlayer in RoomSystem.PlayersInRoom)
		{
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(targetPlayer, out rigContainer) && rigContainer.Rig.IsItemAllowed(pieceSetFromID.playfabID))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002159 RID: 8537 RVA: 0x000B4F84 File Offset: 0x000B3184
	public bool IsPieceOwnedByRoom(int pieceType, int materialType)
	{
		int index;
		if (BuilderSetManager.pieceSetInfoMap.TryGetValue(HashCode.Combine<int, int>(pieceType, materialType), out index))
		{
			foreach (int setID in BuilderSetManager.pieceSetInfos[index].setIds)
			{
				if (this.DoesAnyPlayerInRoomOwnPieceSet(setID))
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x0600215A RID: 8538 RVA: 0x000B5004 File Offset: 0x000B3204
	public bool IsPieceOwnedLocally(int pieceType, int materialType)
	{
		int index;
		if (BuilderSetManager.pieceSetInfoMap.TryGetValue(HashCode.Combine<int, int>(pieceType, materialType), out index))
		{
			foreach (int setID in BuilderSetManager.pieceSetInfos[index].setIds)
			{
				if (this.IsPieceSetOwnedLocally(setID))
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x0600215B RID: 8539 RVA: 0x000B5084 File Offset: 0x000B3284
	public bool IsPieceSetOwnedLocally(int setID)
	{
		return this._unlockedPieceSets.FindIndex((BuilderPieceSet x) => setID == x.GetIntIdentifier()) >= 0;
	}

	// Token: 0x0600215C RID: 8540 RVA: 0x000B50BC File Offset: 0x000B32BC
	public void UnlockSet(int setID)
	{
		int num = this._allPieceSets.FindIndex((BuilderPieceSet x) => setID == x.GetIntIdentifier());
		if (num >= 0 && !this._unlockedPieceSets.Contains(this._allPieceSets[num]))
		{
			Debug.Log("BuilderSetManager: unlocking set " + this._allPieceSets[num].setName);
			this._unlockedPieceSets.Add(this._allPieceSets[num]);
		}
		UnityEvent onOwnedSetsUpdated = this.OnOwnedSetsUpdated;
		if (onOwnedSetsUpdated == null)
		{
			return;
		}
		onOwnedSetsUpdated.Invoke();
	}

	// Token: 0x0600215D RID: 8541 RVA: 0x000B5154 File Offset: 0x000B3354
	public void TryPurchaseItem(int setID, Action<bool> resultCallback)
	{
		BuilderSetManager.BuilderSetStoreItem storeItem;
		if (!BuilderSetManager._setIdToStoreItem.TryGetValue(setID, out storeItem))
		{
			Debug.Log("BuilderSetManager: no store Item for set " + setID.ToString());
			Action<bool> resultCallback2 = resultCallback;
			if (resultCallback2 == null)
			{
				return;
			}
			resultCallback2(false);
			return;
		}
		else
		{
			if (!this.IsPieceSetOwnedLocally(setID))
			{
				PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
				{
					ItemId = storeItem.playfabID,
					Price = (int)storeItem.cost,
					VirtualCurrency = this.currencyName,
					CatalogVersion = this.catalog
				}, delegate(PurchaseItemResult result)
				{
					if (result.Items.Count > 0)
					{
						foreach (ItemInstance itemInstance in result.Items)
						{
							Debug.Log("BuilderSetManager: unlocking set " + itemInstance.ItemId);
							this.UnlockSet(itemInstance.ItemId.GetStaticHash());
						}
						CosmeticsController.instance.UpdateMyCosmetics();
						if (PhotonNetwork.InRoom)
						{
							this.StartCoroutine(this.CheckIfMyCosmeticsUpdated(storeItem.playfabID));
						}
						Action<bool> resultCallback4 = resultCallback;
						if (resultCallback4 == null)
						{
							return;
						}
						resultCallback4(true);
						return;
					}
					else
					{
						Debug.Log("BuilderSetManager: no items purchased ");
						Action<bool> resultCallback5 = resultCallback;
						if (resultCallback5 == null)
						{
							return;
						}
						resultCallback5(false);
						return;
					}
				}, delegate(PlayFabError error)
				{
					Debug.LogErrorFormat("BuilderSetManager: purchase {0} Error {1}", new object[]
					{
						setID,
						error.ErrorMessage
					});
					Action<bool> resultCallback4 = resultCallback;
					if (resultCallback4 == null)
					{
						return;
					}
					resultCallback4(false);
				}, null, null);
				return;
			}
			Debug.Log("BuilderSetManager: set already owned " + setID.ToString());
			Action<bool> resultCallback3 = resultCallback;
			if (resultCallback3 == null)
			{
				return;
			}
			resultCallback3(false);
			return;
		}
	}

	// Token: 0x0600215E RID: 8542 RVA: 0x000B5258 File Offset: 0x000B3458
	private IEnumerator CheckIfMyCosmeticsUpdated(string itemToBuyID)
	{
		yield return new WaitForSeconds(1f);
		this.foundCosmetic = false;
		this.attempts = 0;
		while (!this.foundCosmetic && this.attempts < 10 && PhotonNetwork.InRoom)
		{
			this.playerIDList.Clear();
			if (GorillaServer.Instance != null && GorillaServer.Instance.NewCosmeticsPath())
			{
				this.playerIDList.Add("Inventory");
				PlayFabClientAPI.GetSharedGroupData(new PlayFab.ClientModels.GetSharedGroupDataRequest
				{
					Keys = this.playerIDList,
					SharedGroupId = PhotonNetwork.LocalPlayer.UserId + "Inventory"
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
					bool flag = this.foundCosmetic;
				}, delegate(PlayFabError error)
				{
					this.attempts++;
					CosmeticsController.instance.ReauthOrBan(error);
				}, null, null);
				yield return new WaitForSeconds(1f);
			}
			else
			{
				this.playerIDList.Add(PhotonNetwork.LocalPlayer.ActorNumber.ToString());
				PlayFabClientAPI.GetSharedGroupData(new PlayFab.ClientModels.GetSharedGroupDataRequest
				{
					Keys = this.playerIDList,
					SharedGroupId = PhotonNetwork.CurrentRoom.Name + Regex.Replace(PhotonNetwork.CloudRegion, "[^a-zA-Z0-9]", "").ToUpper()
				}, delegate(GetSharedGroupDataResult result)
				{
					this.attempts++;
					foreach (KeyValuePair<string, PlayFab.ClientModels.SharedGroupDataRecord> keyValuePair in result.Data)
					{
						if (keyValuePair.Value.Value.Contains(itemToBuyID))
						{
							Debug.Log("BuilderSetManager: found it! updating others cosmetic!");
							PhotonNetwork.RaiseEvent(199, null, new RaiseEventOptions
							{
								Receivers = ReceiverGroup.Others
							}, SendOptions.SendReliable);
							this.foundCosmetic = true;
						}
						else
						{
							Debug.Log("BuilderSetManager: didnt find it, updating attempts and trying again in a bit. current attempt is " + this.attempts.ToString());
						}
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
					Debug.Log("BuilderSetManager: Got error retrieving user data, on attempt " + this.attempts.ToString());
					Debug.Log(error.GenerateErrorReport());
				}, null, null);
				yield return new WaitForSeconds(1f);
			}
		}
		Debug.Log("BuilderSetManager: done!");
		yield break;
	}

	// Token: 0x04002A96 RID: 10902
	[SerializeField]
	private List<BuilderPieceSet> _allPieceSets;

	// Token: 0x04002A97 RID: 10903
	[SerializeField]
	private List<BuilderPieceSet> _starterPieceSets;

	// Token: 0x04002A98 RID: 10904
	[SerializeField]
	private List<BuilderPieceSet> _setsAlwaysForSale;

	// Token: 0x04002A99 RID: 10905
	[SerializeField]
	private List<BuilderPieceSet> _seasonalSetsForSale;

	// Token: 0x04002A9A RID: 10906
	private List<BuilderPieceSet> livePieceSets;

	// Token: 0x04002A9B RID: 10907
	private List<BuilderPieceSet> scheduledPieceSets;

	// Token: 0x04002A9C RID: 10908
	private Coroutine monitor;

	// Token: 0x04002A9D RID: 10909
	private List<BuilderSetManager.BuilderSetStoreItem> _allStoreItems;

	// Token: 0x04002A9E RID: 10910
	private List<BuilderPieceSet> _unlockedPieceSets;

	// Token: 0x04002A9F RID: 10911
	private static Dictionary<int, BuilderSetManager.BuilderSetStoreItem> _setIdToStoreItem;

	// Token: 0x04002AA0 RID: 10912
	private static List<BuilderSetManager.BuilderPieceSetInfo> pieceSetInfos;

	// Token: 0x04002AA1 RID: 10913
	private static Dictionary<int, int> pieceSetInfoMap;

	// Token: 0x04002AA2 RID: 10914
	[OnEnterPlay_SetNull]
	public static volatile BuilderSetManager instance;

	// Token: 0x04002AA4 RID: 10916
	[HideInInspector]
	public string catalog;

	// Token: 0x04002AA5 RID: 10917
	[HideInInspector]
	public string currencyName;

	// Token: 0x04002AA6 RID: 10918
	private string[] tempStringArray;

	// Token: 0x04002AA7 RID: 10919
	[HideInInspector]
	public UnityEvent OnLiveSetsUpdated;

	// Token: 0x04002AA8 RID: 10920
	[HideInInspector]
	public UnityEvent OnOwnedSetsUpdated;

	// Token: 0x04002AA9 RID: 10921
	[HideInInspector]
	public bool pulledStoreItems;

	// Token: 0x04002AAA RID: 10922
	private static string concatStarterSets = string.Empty;

	// Token: 0x04002AAB RID: 10923
	private static string concatAllSets = string.Empty;

	// Token: 0x04002AAC RID: 10924
	private bool foundCosmetic;

	// Token: 0x04002AAD RID: 10925
	private int attempts;

	// Token: 0x04002AAE RID: 10926
	private List<string> playerIDList = new List<string>();

	// Token: 0x04002AAF RID: 10927
	private static List<int> pieceTypes;

	// Token: 0x04002AB0 RID: 10928
	[HideInInspector]
	public static List<BuilderPiece> pieceList;

	// Token: 0x04002AB1 RID: 10929
	private static Dictionary<int, int> pieceTypeToIndex;

	// Token: 0x04002AB2 RID: 10930
	private bool hasPieceDictionary;

	// Token: 0x02000554 RID: 1364
	[Serializable]
	public struct BuilderSetStoreItem
	{
		// Token: 0x04002AB3 RID: 10931
		public string displayName;

		// Token: 0x04002AB4 RID: 10932
		public string playfabID;

		// Token: 0x04002AB5 RID: 10933
		public int setID;

		// Token: 0x04002AB6 RID: 10934
		public uint cost;

		// Token: 0x04002AB7 RID: 10935
		public bool hasPrice;

		// Token: 0x04002AB8 RID: 10936
		public BuilderPieceSet setRef;

		// Token: 0x04002AB9 RID: 10937
		public GameObject displayModel;

		// Token: 0x04002ABA RID: 10938
		[NonSerialized]
		public bool isNullItem;
	}

	// Token: 0x02000555 RID: 1365
	[Serializable]
	public struct BuilderPieceSetInfo
	{
		// Token: 0x04002ABB RID: 10939
		public int pieceType;

		// Token: 0x04002ABC RID: 10940
		public int materialType;

		// Token: 0x04002ABD RID: 10941
		public List<int> setIds;
	}
}
