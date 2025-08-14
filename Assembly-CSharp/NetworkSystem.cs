using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Fusion;
using GorillaNetworking;
using GorillaTag;
using Photon.Realtime;
using Photon.Voice.Unity;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using Steamworks;
using UnityEngine;

// Token: 0x020002E1 RID: 737
public abstract class NetworkSystem : MonoBehaviour
{
	// Token: 0x170001CC RID: 460
	// (get) Token: 0x0600114E RID: 4430 RVA: 0x000623DC File Offset: 0x000605DC
	// (set) Token: 0x0600114F RID: 4431 RVA: 0x000623E4 File Offset: 0x000605E4
	public bool groupJoinInProgress { get; protected set; }

	// Token: 0x170001CD RID: 461
	// (get) Token: 0x06001150 RID: 4432 RVA: 0x000623ED File Offset: 0x000605ED
	// (set) Token: 0x06001151 RID: 4433 RVA: 0x000623F5 File Offset: 0x000605F5
	public NetSystemState netState
	{
		get
		{
			return this.testState;
		}
		protected set
		{
			Debug.Log("netstate set to:" + value.ToString());
			this.testState = value;
		}
	}

	// Token: 0x170001CE RID: 462
	// (get) Token: 0x06001152 RID: 4434 RVA: 0x0006241A File Offset: 0x0006061A
	public NetPlayer LocalPlayer
	{
		get
		{
			return this.netPlayerCache.Find((NetPlayer p) => p.IsLocal);
		}
	}

	// Token: 0x170001CF RID: 463
	// (get) Token: 0x06001153 RID: 4435 RVA: 0x00062446 File Offset: 0x00060646
	public virtual bool IsMasterClient { get; }

	// Token: 0x170001D0 RID: 464
	// (get) Token: 0x06001154 RID: 4436 RVA: 0x0006244E File Offset: 0x0006064E
	public virtual NetPlayer MasterClient
	{
		get
		{
			return this.netPlayerCache.Find((NetPlayer p) => p.IsMasterClient);
		}
	}

	// Token: 0x170001D1 RID: 465
	// (get) Token: 0x06001155 RID: 4437 RVA: 0x0006247A File Offset: 0x0006067A
	public Recorder LocalRecorder
	{
		get
		{
			return this.localRecorder;
		}
	}

	// Token: 0x170001D2 RID: 466
	// (get) Token: 0x06001156 RID: 4438 RVA: 0x00062482 File Offset: 0x00060682
	public Speaker LocalSpeaker
	{
		get
		{
			return this.localSpeaker;
		}
	}

	// Token: 0x06001157 RID: 4439 RVA: 0x0006248A File Offset: 0x0006068A
	protected void JoinedNetworkRoom()
	{
		VRRigCache.Instance.OnJoinedRoom();
		DelegateListProcessor onJoinedRoomEvent = this.OnJoinedRoomEvent;
		if (onJoinedRoomEvent == null)
		{
			return;
		}
		onJoinedRoomEvent.InvokeSafe();
	}

	// Token: 0x06001158 RID: 4440 RVA: 0x000624A6 File Offset: 0x000606A6
	internal void MultiplayerStarted()
	{
		DelegateListProcessor onMultiplayerStarted = this.OnMultiplayerStarted;
		if (onMultiplayerStarted == null)
		{
			return;
		}
		onMultiplayerStarted.InvokeSafe();
	}

	// Token: 0x06001159 RID: 4441 RVA: 0x000624B8 File Offset: 0x000606B8
	protected void SinglePlayerStarted()
	{
		try
		{
			DelegateListProcessor onReturnedToSinglePlayer = this.OnReturnedToSinglePlayer;
			if (onReturnedToSinglePlayer != null)
			{
				onReturnedToSinglePlayer.InvokeSafe();
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		VRRigCache.Instance.OnLeftRoom();
	}

	// Token: 0x0600115A RID: 4442 RVA: 0x000624FC File Offset: 0x000606FC
	protected void PlayerJoined(NetPlayer netPlayer)
	{
		if (this.IsOnline)
		{
			VRRigCache.Instance.OnPlayerEnteredRoom(netPlayer);
			DelegateListProcessor<NetPlayer> onPlayerJoined = this.OnPlayerJoined;
			if (onPlayerJoined == null)
			{
				return;
			}
			onPlayerJoined.InvokeSafe(netPlayer);
		}
	}

	// Token: 0x0600115B RID: 4443 RVA: 0x00062524 File Offset: 0x00060724
	protected void PlayerLeft(NetPlayer netPlayer)
	{
		try
		{
			DelegateListProcessor<NetPlayer> onPlayerLeft = this.OnPlayerLeft;
			if (onPlayerLeft != null)
			{
				onPlayerLeft.InvokeSafe(netPlayer);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		VRRigCache.Instance.OnPlayerLeftRoom(netPlayer);
	}

	// Token: 0x0600115C RID: 4444 RVA: 0x00062568 File Offset: 0x00060768
	protected void OnMasterClientSwitchedCallback(NetPlayer nMaster)
	{
		DelegateListProcessor<NetPlayer> onMasterClientSwitchedEvent = this.OnMasterClientSwitchedEvent;
		if (onMasterClientSwitchedEvent == null)
		{
			return;
		}
		onMasterClientSwitchedEvent.InvokeSafe(nMaster);
	}

	// Token: 0x1400002A RID: 42
	// (add) Token: 0x0600115D RID: 4445 RVA: 0x0006257C File Offset: 0x0006077C
	// (remove) Token: 0x0600115E RID: 4446 RVA: 0x000625B4 File Offset: 0x000607B4
	public event Action<byte, object, int> OnRaiseEvent;

	// Token: 0x0600115F RID: 4447 RVA: 0x000625E9 File Offset: 0x000607E9
	internal void RaiseEvent(byte eventCode, object data, int source)
	{
		Action<byte, object, int> onRaiseEvent = this.OnRaiseEvent;
		if (onRaiseEvent == null)
		{
			return;
		}
		onRaiseEvent(eventCode, data, source);
	}

	// Token: 0x1400002B RID: 43
	// (add) Token: 0x06001160 RID: 4448 RVA: 0x00062600 File Offset: 0x00060800
	// (remove) Token: 0x06001161 RID: 4449 RVA: 0x00062638 File Offset: 0x00060838
	public event Action<Dictionary<string, object>> OnCustomAuthenticationResponse;

	// Token: 0x06001162 RID: 4450 RVA: 0x0006266D File Offset: 0x0006086D
	internal void CustomAuthenticationResponse(Dictionary<string, object> response)
	{
		Action<Dictionary<string, object>> onCustomAuthenticationResponse = this.OnCustomAuthenticationResponse;
		if (onCustomAuthenticationResponse == null)
		{
			return;
		}
		onCustomAuthenticationResponse(response);
	}

	// Token: 0x06001163 RID: 4451 RVA: 0x00062680 File Offset: 0x00060880
	public virtual void Initialise()
	{
		Debug.Log("INITIALISING NETWORKSYSTEMS");
		if (NetworkSystem.Instance)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		NetworkSystem.Instance = this;
		NetCrossoverUtils.Prewarm();
	}

	// Token: 0x06001164 RID: 4452 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void Update()
	{
	}

	// Token: 0x06001165 RID: 4453 RVA: 0x000626AF File Offset: 0x000608AF
	public void RegisterSceneNetworkItem(GameObject item)
	{
		if (!this.SceneObjectsToAttach.Contains(item))
		{
			this.SceneObjectsToAttach.Add(item);
		}
	}

	// Token: 0x06001166 RID: 4454 RVA: 0x000626CB File Offset: 0x000608CB
	public virtual void AttachObjectInGame(GameObject item)
	{
		this.RegisterSceneNetworkItem(item);
	}

	// Token: 0x06001167 RID: 4455 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void DetatchSceneObjectInGame(GameObject item)
	{
	}

	// Token: 0x06001168 RID: 4456 RVA: 0x000626D4 File Offset: 0x000608D4
	public virtual AuthenticationValues GetAuthenticationValues()
	{
		Debug.LogWarning("NetworkSystem.GetAuthenticationValues should be overridden");
		return new AuthenticationValues();
	}

	// Token: 0x06001169 RID: 4457 RVA: 0x000626E5 File Offset: 0x000608E5
	public virtual void SetAuthenticationValues(AuthenticationValues authValues)
	{
		Debug.LogWarning("NetworkSystem.SetAuthenticationValues should be overridden");
	}

	// Token: 0x0600116A RID: 4458
	public abstract void FinishAuthenticating();

	// Token: 0x0600116B RID: 4459
	public abstract Task<NetJoinResult> ConnectToRoom(string roomName, RoomConfig opts, int regionIndex = -1);

	// Token: 0x0600116C RID: 4460
	public abstract Task JoinFriendsRoom(string userID, int actorID, string keyToFollow, string shufflerToFollow);

	// Token: 0x0600116D RID: 4461
	public abstract Task ReturnToSinglePlayer();

	// Token: 0x0600116E RID: 4462
	public abstract void JoinPubWithFriends();

	// Token: 0x170001D3 RID: 467
	// (get) Token: 0x0600116F RID: 4463 RVA: 0x000626F1 File Offset: 0x000608F1
	public bool WrongVersion
	{
		get
		{
			return this.isWrongVersion;
		}
	}

	// Token: 0x06001170 RID: 4464 RVA: 0x000626F9 File Offset: 0x000608F9
	public void SetWrongVersion()
	{
		this.isWrongVersion = true;
	}

	// Token: 0x06001171 RID: 4465 RVA: 0x00062702 File Offset: 0x00060902
	public GameObject NetInstantiate(GameObject prefab, bool isRoomObject = false)
	{
		return this.NetInstantiate(prefab, Vector3.zero, Quaternion.identity, false);
	}

	// Token: 0x06001172 RID: 4466 RVA: 0x00062716 File Offset: 0x00060916
	public GameObject NetInstantiate(GameObject prefab, Vector3 position, bool isRoomObject = false)
	{
		return this.NetInstantiate(prefab, position, Quaternion.identity, false);
	}

	// Token: 0x06001173 RID: 4467
	public abstract GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject = false);

	// Token: 0x06001174 RID: 4468
	public abstract GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, int playerAuthID, bool isRoomObject = false);

	// Token: 0x06001175 RID: 4469
	public abstract GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject, byte group = 0, object[] data = null, NetworkRunner.OnBeforeSpawned callback = null);

	// Token: 0x06001176 RID: 4470
	public abstract void SetPlayerObject(GameObject playerInstance, int? owningPlayerID = null);

	// Token: 0x06001177 RID: 4471
	public abstract void NetDestroy(GameObject instance);

	// Token: 0x06001178 RID: 4472
	public abstract void CallRPC(MonoBehaviour component, NetworkSystem.RPC rpcMethod, bool sendToSelf = true);

	// Token: 0x06001179 RID: 4473
	public abstract void CallRPC<T>(MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args, bool sendToSelf = true) where T : struct;

	// Token: 0x0600117A RID: 4474
	public abstract void CallRPC(MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message, bool sendToSelf = true);

	// Token: 0x0600117B RID: 4475
	public abstract void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod);

	// Token: 0x0600117C RID: 4476
	public abstract void CallRPC<T>(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args) where T : struct;

	// Token: 0x0600117D RID: 4477
	public abstract void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message);

	// Token: 0x0600117E RID: 4478 RVA: 0x00062728 File Offset: 0x00060928
	public static string GetRandomRoomName()
	{
		string text = "";
		for (int i = 0; i < 4; i++)
		{
			text += "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Substring(Random.Range(0, "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Length), 1);
		}
		if (GorillaComputer.instance.IsPlayerInVirtualStump())
		{
			text = GorillaComputer.instance.VStumpRoomPrepend + text;
		}
		if (GorillaComputer.instance.CheckAutoBanListForName(text))
		{
			return text;
		}
		return NetworkSystem.GetRandomRoomName();
	}

	// Token: 0x0600117F RID: 4479
	public abstract string GetRandomWeightedRegion();

	// Token: 0x06001180 RID: 4480 RVA: 0x000627A0 File Offset: 0x000609A0
	protected Task RefreshNonce()
	{
		NetworkSystem.<RefreshNonce>d__89 <RefreshNonce>d__;
		<RefreshNonce>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<RefreshNonce>d__.<>4__this = this;
		<RefreshNonce>d__.<>1__state = -1;
		<RefreshNonce>d__.<>t__builder.Start<NetworkSystem.<RefreshNonce>d__89>(ref <RefreshNonce>d__);
		return <RefreshNonce>d__.<>t__builder.Task;
	}

	// Token: 0x06001181 RID: 4481 RVA: 0x000627E4 File Offset: 0x000609E4
	private void GetSteamAuthTicketSuccessCallback(string ticket)
	{
		AuthenticationValues authenticationValues = this.GetAuthenticationValues();
		Dictionary<string, object> dictionary = ((authenticationValues != null) ? authenticationValues.AuthPostData : null) as Dictionary<string, object>;
		if (dictionary != null)
		{
			dictionary["Nonce"] = ticket;
			authenticationValues.SetAuthPostData(dictionary);
			this.SetAuthenticationValues(authenticationValues);
			this.nonceRefreshed = true;
		}
	}

	// Token: 0x06001182 RID: 4482 RVA: 0x0006282E File Offset: 0x00060A2E
	private void GetSteamAuthTicketFailureCallback(EResult result)
	{
		base.StartCoroutine(this.ReGetNonce());
	}

	// Token: 0x06001183 RID: 4483 RVA: 0x0006283D File Offset: 0x00060A3D
	private IEnumerator ReGetNonce()
	{
		yield return new WaitForSeconds(3f);
		PlayFabAuthenticator.instance.RefreshSteamAuthTicketForPhoton(new Action<string>(this.GetSteamAuthTicketSuccessCallback), new Action<EResult>(this.GetSteamAuthTicketFailureCallback));
		yield return null;
		yield break;
	}

	// Token: 0x06001184 RID: 4484 RVA: 0x0006284C File Offset: 0x00060A4C
	public void BroadcastMyRoom(bool create, string key, string shuffler)
	{
		string text = NetworkSystem.ShuffleRoomName(NetworkSystem.Instance.RoomName, shuffler.Substring(2, 8), true) + "|" + NetworkSystem.ShuffleRoomName("ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Substring(NetworkSystem.Instance.currentRegionIndex, 1), shuffler.Substring(0, 2), true);
		Debug.Log(string.Format("Broadcasting room {0} region {1}({2}). Create: {3} key: {4} (shuffler {5}) shuffled: {6}", new object[]
		{
			NetworkSystem.Instance.RoomName,
			NetworkSystem.Instance.currentRegionIndex,
			NetworkSystem.Instance.regionNames[NetworkSystem.Instance.currentRegionIndex],
			create,
			key,
			shuffler,
			text
		}));
		GorillaServer instance = GorillaServer.Instance;
		BroadcastMyRoomRequest broadcastMyRoomRequest = new BroadcastMyRoomRequest();
		broadcastMyRoomRequest.KeyToFollow = key;
		broadcastMyRoomRequest.RoomToJoin = text;
		broadcastMyRoomRequest.Set = create;
		instance.BroadcastMyRoom(broadcastMyRoomRequest, delegate(ExecuteFunctionResult result)
		{
		}, delegate(PlayFabError error)
		{
		});
	}

	// Token: 0x06001185 RID: 4485 RVA: 0x00062964 File Offset: 0x00060B64
	public bool InstantCheckGroupData(string userID, string keyToFollow)
	{
		bool success = false;
		PlayFab.ClientModels.GetSharedGroupDataRequest getSharedGroupDataRequest = new PlayFab.ClientModels.GetSharedGroupDataRequest();
		getSharedGroupDataRequest.Keys = new List<string>
		{
			keyToFollow
		};
		getSharedGroupDataRequest.SharedGroupId = userID;
		PlayFabClientAPI.GetSharedGroupData(getSharedGroupDataRequest, delegate(GetSharedGroupDataResult result)
		{
			Debug.Log("Get Shared Group Data returned a success");
			Debug.Log(result.Data.ToStringFull());
			if (result.Data.Count > 0)
			{
				success = true;
				return;
			}
			Debug.Log("RESULT returned but no DATA");
		}, delegate(PlayFabError error)
		{
			Debug.Log("ERROR - no group data found");
		}, null, null);
		return success;
	}

	// Token: 0x06001186 RID: 4486 RVA: 0x000629D4 File Offset: 0x00060BD4
	public NetPlayer GetNetPlayerByID(int playerActorNumber)
	{
		return this.netPlayerCache.Find((NetPlayer a) => a.ActorNumber == playerActorNumber);
	}

	// Token: 0x06001187 RID: 4487 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void NetRaiseEventReliable(byte eventCode, object data)
	{
	}

	// Token: 0x06001188 RID: 4488 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void NetRaiseEventUnreliable(byte eventCode, object data)
	{
	}

	// Token: 0x06001189 RID: 4489 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void NetRaiseEventReliable(byte eventCode, object data, NetEventOptions options)
	{
	}

	// Token: 0x0600118A RID: 4490 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void NetRaiseEventUnreliable(byte eventCode, object data, NetEventOptions options)
	{
	}

	// Token: 0x0600118B RID: 4491 RVA: 0x00062A08 File Offset: 0x00060C08
	public static string ShuffleRoomName(string room, string shuffle, bool encode)
	{
		NetworkSystem.shuffleStringBuilder.Clear();
		int num;
		if (!int.TryParse(shuffle, out num))
		{
			Debug.Log("Shuffle room failed");
			return "";
		}
		for (int i = 0; i < room.Length; i++)
		{
			int num2 = int.Parse(shuffle.Substring(i * 2 % (shuffle.Length - 1), 2));
			int index = NetworkSystem.mod("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".IndexOf(room[i]) + (encode ? num2 : (-num2)), "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".Length);
			NetworkSystem.shuffleStringBuilder.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"[index]);
		}
		return NetworkSystem.shuffleStringBuilder.ToString();
	}

	// Token: 0x0600118C RID: 4492 RVA: 0x00023F71 File Offset: 0x00022171
	public static int mod(int x, int m)
	{
		return (x % m + m) % m;
	}

	// Token: 0x0600118D RID: 4493
	public abstract Task AwaitSceneReady();

	// Token: 0x170001D4 RID: 468
	// (get) Token: 0x0600118E RID: 4494
	public abstract string CurrentPhotonBackend { get; }

	// Token: 0x0600118F RID: 4495
	public abstract NetPlayer GetLocalPlayer();

	// Token: 0x06001190 RID: 4496
	public abstract NetPlayer GetPlayer(int PlayerID);

	// Token: 0x06001191 RID: 4497 RVA: 0x00062AB0 File Offset: 0x00060CB0
	public NetPlayer GetPlayer(Player punPlayer)
	{
		if (punPlayer == null)
		{
			return null;
		}
		NetPlayer netPlayer = this.FindPlayer(punPlayer);
		if (netPlayer == null)
		{
			this.UpdatePlayers();
			netPlayer = this.FindPlayer(punPlayer);
			if (netPlayer == null)
			{
				Debug.LogError(string.Format("There is no NetPlayer with this ID currently in game. Passed ID: {0} nickname {1}", punPlayer.ActorNumber, punPlayer.NickName));
				return null;
			}
		}
		return netPlayer;
	}

	// Token: 0x06001192 RID: 4498 RVA: 0x00062B04 File Offset: 0x00060D04
	private NetPlayer FindPlayer(Player punPlayer)
	{
		for (int i = 0; i < this.netPlayerCache.Count; i++)
		{
			if (this.netPlayerCache[i].GetPlayerRef() == punPlayer)
			{
				return this.netPlayerCache[i];
			}
		}
		return null;
	}

	// Token: 0x06001193 RID: 4499 RVA: 0x00058615 File Offset: 0x00056815
	public NetPlayer GetPlayer(PlayerRef playerRef)
	{
		return null;
	}

	// Token: 0x06001194 RID: 4500
	public abstract void SetMyNickName(string name);

	// Token: 0x06001195 RID: 4501
	public abstract string GetMyNickName();

	// Token: 0x06001196 RID: 4502
	public abstract string GetMyDefaultName();

	// Token: 0x06001197 RID: 4503
	public abstract string GetNickName(int playerID);

	// Token: 0x06001198 RID: 4504
	public abstract string GetNickName(NetPlayer player);

	// Token: 0x06001199 RID: 4505
	public abstract string GetMyUserID();

	// Token: 0x0600119A RID: 4506
	public abstract string GetUserID(int playerID);

	// Token: 0x0600119B RID: 4507
	public abstract string GetUserID(NetPlayer player);

	// Token: 0x0600119C RID: 4508
	public abstract void SetMyTutorialComplete();

	// Token: 0x0600119D RID: 4509
	public abstract bool GetMyTutorialCompletion();

	// Token: 0x0600119E RID: 4510
	public abstract bool GetPlayerTutorialCompletion(int playerID);

	// Token: 0x0600119F RID: 4511 RVA: 0x00062B49 File Offset: 0x00060D49
	public void AddVoiceSettings(SO_NetworkVoiceSettings settings)
	{
		this.VoiceSettings = settings;
	}

	// Token: 0x060011A0 RID: 4512
	public abstract void AddRemoteVoiceAddedCallback(Action<RemoteVoiceLink> callback);

	// Token: 0x170001D5 RID: 469
	// (get) Token: 0x060011A1 RID: 4513
	public abstract VoiceConnection VoiceConnection { get; }

	// Token: 0x170001D6 RID: 470
	// (get) Token: 0x060011A2 RID: 4514
	public abstract bool IsOnline { get; }

	// Token: 0x170001D7 RID: 471
	// (get) Token: 0x060011A3 RID: 4515
	public abstract bool InRoom { get; }

	// Token: 0x170001D8 RID: 472
	// (get) Token: 0x060011A4 RID: 4516
	public abstract string RoomName { get; }

	// Token: 0x060011A5 RID: 4517
	public abstract string RoomStringStripped();

	// Token: 0x060011A6 RID: 4518 RVA: 0x00062B54 File Offset: 0x00060D54
	public string RoomString()
	{
		return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", new object[]
		{
			this.RoomName,
			this.CurrentRoom.isPublic ? "visible" : "hidden",
			this.CurrentRoom.isJoinable ? "open" : "closed",
			this.CurrentRoom.MaxPlayers,
			this.RoomPlayerCount,
			this.CurrentRoom.CustomProps.ToStringFull()
		});
	}

	// Token: 0x170001D9 RID: 473
	// (get) Token: 0x060011A7 RID: 4519
	public abstract string GameModeString { get; }

	// Token: 0x170001DA RID: 474
	// (get) Token: 0x060011A8 RID: 4520
	public abstract string CurrentRegion { get; }

	// Token: 0x170001DB RID: 475
	// (get) Token: 0x060011A9 RID: 4521
	public abstract bool SessionIsPrivate { get; }

	// Token: 0x170001DC RID: 476
	// (get) Token: 0x060011AA RID: 4522
	public abstract int LocalPlayerID { get; }

	// Token: 0x170001DD RID: 477
	// (get) Token: 0x060011AB RID: 4523 RVA: 0x00062BE6 File Offset: 0x00060DE6
	public virtual NetPlayer[] AllNetPlayers
	{
		get
		{
			return this.netPlayerCache.ToArray();
		}
	}

	// Token: 0x170001DE RID: 478
	// (get) Token: 0x060011AC RID: 4524 RVA: 0x00062BF3 File Offset: 0x00060DF3
	public virtual NetPlayer[] PlayerListOthers
	{
		get
		{
			return this.netPlayerCache.FindAll((NetPlayer p) => !p.IsLocal).ToArray();
		}
	}

	// Token: 0x060011AD RID: 4525
	protected abstract void UpdateNetPlayerList();

	// Token: 0x060011AE RID: 4526 RVA: 0x00062C24 File Offset: 0x00060E24
	public void UpdatePlayers()
	{
		this.UpdateNetPlayerList();
	}

	// Token: 0x170001DF RID: 479
	// (get) Token: 0x060011AF RID: 4527
	public abstract double SimTime { get; }

	// Token: 0x170001E0 RID: 480
	// (get) Token: 0x060011B0 RID: 4528
	public abstract float SimDeltaTime { get; }

	// Token: 0x170001E1 RID: 481
	// (get) Token: 0x060011B1 RID: 4529
	public abstract int SimTick { get; }

	// Token: 0x170001E2 RID: 482
	// (get) Token: 0x060011B2 RID: 4530
	public abstract int TickRate { get; }

	// Token: 0x170001E3 RID: 483
	// (get) Token: 0x060011B3 RID: 4531
	public abstract int ServerTimestamp { get; }

	// Token: 0x170001E4 RID: 484
	// (get) Token: 0x060011B4 RID: 4532
	public abstract int RoomPlayerCount { get; }

	// Token: 0x060011B5 RID: 4533
	public abstract int GlobalPlayerCount();

	// Token: 0x170001E5 RID: 485
	// (get) Token: 0x060011B6 RID: 4534 RVA: 0x00062C2C File Offset: 0x00060E2C
	// (set) Token: 0x060011B7 RID: 4535 RVA: 0x00062C34 File Offset: 0x00060E34
	public RoomConfig CurrentRoom { get; protected set; }

	// Token: 0x060011B8 RID: 4536
	public abstract bool IsObjectLocallyOwned(GameObject obj);

	// Token: 0x060011B9 RID: 4537
	public abstract bool IsObjectRoomObject(GameObject obj);

	// Token: 0x060011BA RID: 4538
	public abstract bool ShouldUpdateObject(GameObject obj);

	// Token: 0x060011BB RID: 4539
	public abstract bool ShouldWriteObjectData(GameObject obj);

	// Token: 0x060011BC RID: 4540
	public abstract int GetOwningPlayerID(GameObject obj);

	// Token: 0x060011BD RID: 4541
	public abstract bool ShouldSpawnLocally(int playerID);

	// Token: 0x060011BE RID: 4542
	public abstract bool IsTotalAuthority();

	// Token: 0x0400196E RID: 6510
	public static NetworkSystem Instance;

	// Token: 0x0400196F RID: 6511
	public NetworkSystemConfig config;

	// Token: 0x04001970 RID: 6512
	public bool changingSceneManually;

	// Token: 0x04001971 RID: 6513
	public string[] regionNames;

	// Token: 0x04001972 RID: 6514
	public int currentRegionIndex;

	// Token: 0x04001974 RID: 6516
	private bool nonceRefreshed;

	// Token: 0x04001975 RID: 6517
	protected bool isWrongVersion;

	// Token: 0x04001976 RID: 6518
	private NetSystemState testState;

	// Token: 0x04001977 RID: 6519
	protected List<NetPlayer> netPlayerCache = new List<NetPlayer>();

	// Token: 0x04001978 RID: 6520
	protected Recorder localRecorder;

	// Token: 0x04001979 RID: 6521
	protected Speaker localSpeaker;

	// Token: 0x0400197B RID: 6523
	public List<GameObject> SceneObjectsToAttach = new List<GameObject>();

	// Token: 0x0400197C RID: 6524
	protected SO_NetworkVoiceSettings VoiceSettings;

	// Token: 0x0400197D RID: 6525
	protected List<Action<RemoteVoiceLink>> remoteVoiceAddedCallbacks = new List<Action<RemoteVoiceLink>>();

	// Token: 0x0400197E RID: 6526
	public DelegateListProcessor OnJoinedRoomEvent = new DelegateListProcessor();

	// Token: 0x0400197F RID: 6527
	public DelegateListProcessor OnMultiplayerStarted = new DelegateListProcessor();

	// Token: 0x04001980 RID: 6528
	public DelegateListProcessor OnReturnedToSinglePlayer = new DelegateListProcessor();

	// Token: 0x04001981 RID: 6529
	public DelegateListProcessor<NetPlayer> OnPlayerJoined = new DelegateListProcessor<NetPlayer>();

	// Token: 0x04001982 RID: 6530
	public DelegateListProcessor<NetPlayer> OnPlayerLeft = new DelegateListProcessor<NetPlayer>();

	// Token: 0x04001983 RID: 6531
	internal DelegateListProcessor<NetPlayer> OnMasterClientSwitchedEvent = new DelegateListProcessor<NetPlayer>();

	// Token: 0x04001986 RID: 6534
	protected static readonly byte[] EmptyArgs = new byte[0];

	// Token: 0x04001987 RID: 6535
	public const string roomCharacters = "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789";

	// Token: 0x04001988 RID: 6536
	public const string shuffleCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

	// Token: 0x04001989 RID: 6537
	private static StringBuilder shuffleStringBuilder = new StringBuilder(4);

	// Token: 0x0400198A RID: 6538
	protected static StringBuilder reusableSB = new StringBuilder();

	// Token: 0x020002E2 RID: 738
	// (Invoke) Token: 0x060011C2 RID: 4546
	public delegate void RPC(byte[] data);

	// Token: 0x020002E3 RID: 739
	// (Invoke) Token: 0x060011C6 RID: 4550
	public delegate void StringRPC(string message);

	// Token: 0x020002E4 RID: 740
	// (Invoke) Token: 0x060011CA RID: 4554
	public delegate void StaticRPC(byte[] data);

	// Token: 0x020002E5 RID: 741
	// (Invoke) Token: 0x060011CE RID: 4558
	public delegate void StaticRPCPlaceholder(byte[] args);
}
