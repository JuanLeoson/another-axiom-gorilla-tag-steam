using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GorillaExtensions;
using GorillaNetworking;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

// Token: 0x02000A5E RID: 2654
internal class PlayerCosmeticsSystem : MonoBehaviour, ITickSystemPre
{
	// Token: 0x17000622 RID: 1570
	// (get) Token: 0x060040B1 RID: 16561 RVA: 0x0014743C File Offset: 0x0014563C
	// (set) Token: 0x060040B2 RID: 16562 RVA: 0x00147444 File Offset: 0x00145644
	bool ITickSystemPre.PreTickRunning { get; set; }

	// Token: 0x060040B3 RID: 16563 RVA: 0x00147450 File Offset: 0x00145650
	private void Awake()
	{
		if (PlayerCosmeticsSystem.instance == null)
		{
			PlayerCosmeticsSystem.instance = this;
			base.transform.SetParent(null, true);
			Object.DontDestroyOnLoad(this);
			this.inventory = new List<string>();
			this.inventory.Add("Inventory");
			NetworkSystem.Instance.OnRaiseEvent += this.OnNetEvent;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x060040B4 RID: 16564 RVA: 0x001474BD File Offset: 0x001456BD
	private void Start()
	{
		this.playerLookUpCooldown = Mathf.Max(this.playerLookUpCooldown, 3f);
	}

	// Token: 0x060040B5 RID: 16565 RVA: 0x001474D5 File Offset: 0x001456D5
	private void OnDestroy()
	{
		if (PlayerCosmeticsSystem.instance == this)
		{
			PlayerCosmeticsSystem.instance = null;
		}
	}

	// Token: 0x060040B6 RID: 16566 RVA: 0x001474EA File Offset: 0x001456EA
	private void LookUpPlayerCosmetics(bool wait = false)
	{
		if (!this.isLookingUp)
		{
			TickSystem<object>.AddPreTickCallback(this);
			if (wait)
			{
				this.startSearchingTime = Time.time;
				return;
			}
			this.startSearchingTime = float.MinValue;
		}
	}

	// Token: 0x060040B7 RID: 16567 RVA: 0x00147514 File Offset: 0x00145714
	public void PreTick()
	{
		if (PlayerCosmeticsSystem.playersToLookUp.Count < 1)
		{
			TickSystem<object>.RemovePreTickCallback(this);
			this.startSearchingTime = float.MinValue;
			this.isLookingUp = false;
			return;
		}
		this.isLookingUp = true;
		if (this.startSearchingTime + this.playerLookUpCooldown > Time.time)
		{
			return;
		}
		if (GorillaServer.Instance.NewCosmeticsPathShouldReadSharedGroupData())
		{
			this.NewCosmeticsPath();
			return;
		}
		PlayerCosmeticsSystem.playerIDsList.Clear();
		while (PlayerCosmeticsSystem.playersToLookUp.Count > 0)
		{
			NetPlayer netPlayer = PlayerCosmeticsSystem.playersToLookUp.Dequeue();
			string item = netPlayer.ActorNumber.ToString();
			if (netPlayer.InRoom() && !PlayerCosmeticsSystem.playerIDsList.Contains(item))
			{
				if (PlayerCosmeticsSystem.playerIDsList.Count == 0)
				{
					int actorNumber = netPlayer.ActorNumber;
				}
				PlayerCosmeticsSystem.playerIDsList.Add(item);
				PlayerCosmeticsSystem.playersWaiting.AddSortedUnique(netPlayer.ActorNumber);
			}
		}
		if (PlayerCosmeticsSystem.playerIDsList.Count > 0)
		{
			PlayFab.ClientModels.GetSharedGroupDataRequest getSharedGroupDataRequest = new PlayFab.ClientModels.GetSharedGroupDataRequest();
			getSharedGroupDataRequest.Keys = PlayerCosmeticsSystem.playerIDsList;
			getSharedGroupDataRequest.SharedGroupId = NetworkSystem.Instance.RoomName + Regex.Replace(NetworkSystem.Instance.CurrentRegion, "[^a-zA-Z0-9]", "").ToUpper();
			PlayFabClientAPI.GetSharedGroupData(getSharedGroupDataRequest, new Action<GetSharedGroupDataResult>(this.OnGetsharedGroupData), delegate(PlayFabError error)
			{
				Debug.Log(error.GenerateErrorReport());
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
				}
			}, null, null);
		}
		this.isLookingUp = false;
	}

	// Token: 0x060040B8 RID: 16568 RVA: 0x00147679 File Offset: 0x00145879
	private void NewCosmeticsPath()
	{
		if (this.isLookingUpNew)
		{
			return;
		}
		base.StartCoroutine(this.NewCosmeticsPathCoroutine());
	}

	// Token: 0x060040B9 RID: 16569 RVA: 0x00147691 File Offset: 0x00145891
	private IEnumerator NewCosmeticsPathCoroutine()
	{
		this.isLookingUpNew = true;
		NetPlayer player = null;
		PlayerCosmeticsSystem.playerIDsList.Clear();
		PlayerCosmeticsSystem.playerActorNumberList.Clear();
		while (PlayerCosmeticsSystem.playersToLookUp.Count > 0)
		{
			player = PlayerCosmeticsSystem.playersToLookUp.Dequeue();
			string item = player.ActorNumber.ToString();
			if (player.InRoom() && !PlayerCosmeticsSystem.playerIDsList.Contains(item))
			{
				PlayerCosmeticsSystem.playerIDsList.Add(player.UserId);
				PlayerCosmeticsSystem.playerActorNumberList.Add(player.ActorNumber);
			}
		}
		int num;
		for (int i = 0; i < PlayerCosmeticsSystem.playerIDsList.Count; i = num + 1)
		{
			int j = i;
			PlayFab.ClientModels.GetSharedGroupDataRequest getSharedGroupDataRequest = new PlayFab.ClientModels.GetSharedGroupDataRequest();
			getSharedGroupDataRequest.Keys = this.inventory;
			getSharedGroupDataRequest.SharedGroupId = PlayerCosmeticsSystem.playerIDsList[j] + "Inventory";
			PlayFabClientAPI.GetSharedGroupData(getSharedGroupDataRequest, delegate(GetSharedGroupDataResult result)
			{
				if (!NetworkSystem.Instance.InRoom)
				{
					PlayerCosmeticsSystem.playersWaiting.Clear();
					return;
				}
				foreach (KeyValuePair<string, PlayFab.ClientModels.SharedGroupDataRecord> keyValuePair in result.Data)
				{
					if (!(keyValuePair.Key != "Inventory") && Utils.PlayerInRoom(PlayerCosmeticsSystem.playerActorNumberList[j]))
					{
						this.tempCosmetics = keyValuePair.Value.Value;
						IUserCosmeticsCallback userCosmeticsCallback;
						if (!PlayerCosmeticsSystem.userCosmeticCallback.TryGetValue(PlayerCosmeticsSystem.playerActorNumberList[j], out userCosmeticsCallback))
						{
							PlayerCosmeticsSystem.userCosmeticsWaiting[PlayerCosmeticsSystem.playerActorNumberList[j]] = this.tempCosmetics;
						}
						else
						{
							userCosmeticsCallback.PendingUpdate = false;
							if (!userCosmeticsCallback.OnGetUserCosmetics(this.tempCosmetics))
							{
								PlayerCosmeticsSystem.playersToLookUp.Enqueue(player);
								userCosmeticsCallback.PendingUpdate = true;
							}
						}
					}
				}
			}, delegate(PlayFabError error)
			{
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
				}
			}, null, null);
			yield return new WaitForSeconds(this.getSharedGroupDataCooldown);
			num = i;
		}
		this.isLookingUpNew = false;
		yield break;
	}

	// Token: 0x060040BA RID: 16570 RVA: 0x001476A0 File Offset: 0x001458A0
	private void UpdatePlayersWaitingAndDoLookup(bool retrying)
	{
		if (PlayerCosmeticsSystem.playersWaiting.Count > 0)
		{
			for (int i = PlayerCosmeticsSystem.playersWaiting.Count - 1; i >= 0; i--)
			{
				int num = PlayerCosmeticsSystem.playersWaiting[i];
				if (!Utils.PlayerInRoom(num))
				{
					PlayerCosmeticsSystem.playersWaiting.RemoveAt(i);
				}
				else
				{
					PlayerCosmeticsSystem.playersToLookUp.Enqueue(NetworkSystem.Instance.GetPlayer(num));
					retrying = true;
				}
			}
		}
		if (retrying)
		{
			this.LookUpPlayerCosmetics(true);
		}
	}

	// Token: 0x060040BB RID: 16571 RVA: 0x00147714 File Offset: 0x00145914
	private void OnGetsharedGroupData(GetSharedGroupDataResult result)
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			PlayerCosmeticsSystem.playersWaiting.Clear();
			return;
		}
		bool retrying = false;
		foreach (KeyValuePair<string, PlayFab.ClientModels.SharedGroupDataRecord> keyValuePair in result.Data)
		{
			this.playerTemp = null;
			int num;
			if (int.TryParse(keyValuePair.Key, out num))
			{
				if (!Utils.PlayerInRoom(num))
				{
					PlayerCosmeticsSystem.playersWaiting.Remove(num);
				}
				else
				{
					PlayerCosmeticsSystem.playersWaiting.Remove(num);
					this.playerTemp = NetworkSystem.Instance.GetPlayer(num);
					this.tempCosmetics = keyValuePair.Value.Value;
					IUserCosmeticsCallback userCosmeticsCallback;
					if (!PlayerCosmeticsSystem.userCosmeticCallback.TryGetValue(num, out userCosmeticsCallback))
					{
						PlayerCosmeticsSystem.userCosmeticsWaiting[num] = this.tempCosmetics;
					}
					else
					{
						userCosmeticsCallback.PendingUpdate = false;
						if (!userCosmeticsCallback.OnGetUserCosmetics(this.tempCosmetics))
						{
							Debug.Log("retrying cosmetics for " + this.playerTemp.ToStringFull());
							PlayerCosmeticsSystem.playersToLookUp.Enqueue(this.playerTemp);
							retrying = true;
							userCosmeticsCallback.PendingUpdate = true;
						}
					}
				}
			}
		}
		this.UpdatePlayersWaitingAndDoLookup(retrying);
	}

	// Token: 0x060040BC RID: 16572 RVA: 0x00147858 File Offset: 0x00145A58
	private void OnNetEvent(byte code, object data, int source)
	{
		if (code != 199 || source < 0)
		{
			return;
		}
		Debug.Log("OnNetEvent in Cosmetics called!!");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(source);
		GorillaNot.IncrementRPCCall(new PhotonMessageInfoWrapped(source, NetworkSystem.Instance.ServerTimestamp), "UpdatePlayerCosmetics");
		PlayerCosmeticsSystem.UpdatePlayerCosmetics(player);
	}

	// Token: 0x17000623 RID: 1571
	// (get) Token: 0x060040BD RID: 16573 RVA: 0x001478A6 File Offset: 0x00145AA6
	private static bool nullInstance
	{
		get
		{
			return PlayerCosmeticsSystem.instance == null || !PlayerCosmeticsSystem.instance;
		}
	}

	// Token: 0x060040BE RID: 16574 RVA: 0x001478C0 File Offset: 0x00145AC0
	public static void RegisterCosmeticCallback(int playerID, IUserCosmeticsCallback callback)
	{
		PlayerCosmeticsSystem.userCosmeticCallback[playerID] = callback;
		string cosmetics;
		if (PlayerCosmeticsSystem.userCosmeticsWaiting.TryGetValue(playerID, out cosmetics))
		{
			callback.PendingUpdate = false;
			callback.OnGetUserCosmetics(cosmetics);
			PlayerCosmeticsSystem.userCosmeticsWaiting.Remove(playerID);
		}
	}

	// Token: 0x060040BF RID: 16575 RVA: 0x00147903 File Offset: 0x00145B03
	public static void RemoveCosmeticCallback(int playerID)
	{
		if (PlayerCosmeticsSystem.userCosmeticCallback.ContainsKey(playerID))
		{
			PlayerCosmeticsSystem.userCosmeticCallback.Remove(playerID);
		}
	}

	// Token: 0x060040C0 RID: 16576 RVA: 0x00147920 File Offset: 0x00145B20
	public static void UpdatePlayerCosmetics(NetPlayer player)
	{
		if (player == null || player.IsLocal)
		{
			return;
		}
		PlayerCosmeticsSystem.playersToLookUp.Enqueue(player);
		IUserCosmeticsCallback userCosmeticsCallback;
		if (PlayerCosmeticsSystem.userCosmeticCallback.TryGetValue(player.ActorNumber, out userCosmeticsCallback))
		{
			userCosmeticsCallback.PendingUpdate = true;
		}
		if (!PlayerCosmeticsSystem.nullInstance)
		{
			PlayerCosmeticsSystem.instance.LookUpPlayerCosmetics(true);
		}
	}

	// Token: 0x060040C1 RID: 16577 RVA: 0x00147974 File Offset: 0x00145B74
	public static void UpdatePlayerCosmetics(List<NetPlayer> players)
	{
		foreach (NetPlayer netPlayer in players)
		{
			if (netPlayer != null && !netPlayer.IsLocal)
			{
				PlayerCosmeticsSystem.playersToLookUp.Enqueue(netPlayer);
				IUserCosmeticsCallback userCosmeticsCallback;
				if (PlayerCosmeticsSystem.userCosmeticCallback.TryGetValue(netPlayer.ActorNumber, out userCosmeticsCallback))
				{
					userCosmeticsCallback.PendingUpdate = true;
				}
			}
		}
		if (!PlayerCosmeticsSystem.nullInstance)
		{
			PlayerCosmeticsSystem.instance.LookUpPlayerCosmetics(false);
		}
	}

	// Token: 0x060040C2 RID: 16578 RVA: 0x00147A00 File Offset: 0x00145C00
	public static void SetRigTryOn(bool inTryon, RigContainer rigRefg)
	{
		VRRig rig = rigRefg.Rig;
		rig.inTryOnRoom = inTryon;
		if (inTryon)
		{
			if (PlayerCosmeticsSystem.sinceLastTryOnEvent.HasElapsed(0.5f, true))
			{
				GorillaTelemetry.PostShopEvent(rig, GTShopEventType.item_try_on, rig.tryOnSet.items);
			}
		}
		else if (rig.isOfflineVRRig)
		{
			rig.tryOnSet.ClearSet(CosmeticsController.instance.nullItem);
			CosmeticsController.instance.ClearCheckout(false);
			CosmeticsController.instance.UpdateShoppingCart();
			CosmeticsController.instance.UpdateWornCosmetics(true);
			rig.myBodyDockPositions.RefreshTransferrableItems();
			return;
		}
		rig.LocalUpdateCosmeticsWithTryon(rig.cosmeticSet, rig.tryOnSet);
		rig.myBodyDockPositions.RefreshTransferrableItems();
	}

	// Token: 0x060040C3 RID: 16579 RVA: 0x00147AB2 File Offset: 0x00145CB2
	public static void StaticReset()
	{
		PlayerCosmeticsSystem.playersToLookUp.Clear();
		PlayerCosmeticsSystem.userCosmeticCallback.Clear();
		PlayerCosmeticsSystem.userCosmeticsWaiting.Clear();
		PlayerCosmeticsSystem.playerIDsList.Clear();
		PlayerCosmeticsSystem.playersWaiting.Clear();
	}

	// Token: 0x04004C50 RID: 19536
	public float playerLookUpCooldown = 3f;

	// Token: 0x04004C51 RID: 19537
	public float getSharedGroupDataCooldown = 0.1f;

	// Token: 0x04004C52 RID: 19538
	private float startSearchingTime = float.MinValue;

	// Token: 0x04004C53 RID: 19539
	private bool isLookingUp;

	// Token: 0x04004C54 RID: 19540
	private bool isLookingUpNew;

	// Token: 0x04004C55 RID: 19541
	private string tempCosmetics;

	// Token: 0x04004C56 RID: 19542
	private NetPlayer playerTemp;

	// Token: 0x04004C57 RID: 19543
	private RigContainer tempRC;

	// Token: 0x04004C58 RID: 19544
	private List<string> inventory;

	// Token: 0x04004C59 RID: 19545
	private static PlayerCosmeticsSystem instance;

	// Token: 0x04004C5A RID: 19546
	private static Queue<NetPlayer> playersToLookUp = new Queue<NetPlayer>(10);

	// Token: 0x04004C5B RID: 19547
	private static Dictionary<int, IUserCosmeticsCallback> userCosmeticCallback = new Dictionary<int, IUserCosmeticsCallback>(10);

	// Token: 0x04004C5C RID: 19548
	private static Dictionary<int, string> userCosmeticsWaiting = new Dictionary<int, string>(5);

	// Token: 0x04004C5D RID: 19549
	private static List<string> playerIDsList = new List<string>(10);

	// Token: 0x04004C5E RID: 19550
	private static List<int> playerActorNumberList = new List<int>(10);

	// Token: 0x04004C5F RID: 19551
	private static List<int> playersWaiting = new List<int>();

	// Token: 0x04004C60 RID: 19552
	private static TimeSince sinceLastTryOnEvent = 0f;
}
