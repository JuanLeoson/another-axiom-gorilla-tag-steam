using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ExitGames.Client.Photon;
using Fusion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020006EF RID: 1775
public class GorillaNot : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x17000406 RID: 1030
	// (get) Token: 0x06002C0F RID: 11279 RVA: 0x0005D541 File Offset: 0x0005B741
	private NetworkRunner runner
	{
		get
		{
			return ((NetworkSystemFusion)NetworkSystem.Instance).runner;
		}
	}

	// Token: 0x17000407 RID: 1031
	// (get) Token: 0x06002C10 RID: 11280 RVA: 0x000E95A6 File Offset: 0x000E77A6
	// (set) Token: 0x06002C11 RID: 11281 RVA: 0x000E95AE File Offset: 0x000E77AE
	private bool sendReport
	{
		get
		{
			return this._sendReport;
		}
		set
		{
			if (!this._sendReport)
			{
				this._sendReport = true;
			}
		}
	}

	// Token: 0x17000408 RID: 1032
	// (get) Token: 0x06002C12 RID: 11282 RVA: 0x000E95BF File Offset: 0x000E77BF
	// (set) Token: 0x06002C13 RID: 11283 RVA: 0x000E95C7 File Offset: 0x000E77C7
	private string suspiciousPlayerId
	{
		get
		{
			return this._suspiciousPlayerId;
		}
		set
		{
			if (this._suspiciousPlayerId == "")
			{
				this._suspiciousPlayerId = value;
			}
		}
	}

	// Token: 0x17000409 RID: 1033
	// (get) Token: 0x06002C14 RID: 11284 RVA: 0x000E95E2 File Offset: 0x000E77E2
	// (set) Token: 0x06002C15 RID: 11285 RVA: 0x000E95EA File Offset: 0x000E77EA
	private string suspiciousPlayerName
	{
		get
		{
			return this._suspiciousPlayerName;
		}
		set
		{
			if (this._suspiciousPlayerName == "")
			{
				this._suspiciousPlayerName = value;
			}
		}
	}

	// Token: 0x1700040A RID: 1034
	// (get) Token: 0x06002C16 RID: 11286 RVA: 0x000E9605 File Offset: 0x000E7805
	// (set) Token: 0x06002C17 RID: 11287 RVA: 0x000E960D File Offset: 0x000E780D
	private string suspiciousReason
	{
		get
		{
			return this._suspiciousReason;
		}
		set
		{
			if (this._suspiciousReason == "")
			{
				this._suspiciousReason = value;
			}
		}
	}

	// Token: 0x06002C18 RID: 11288 RVA: 0x000172AD File Offset: 0x000154AD
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06002C19 RID: 11289 RVA: 0x000172B6 File Offset: 0x000154B6
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06002C1A RID: 11290 RVA: 0x000E9628 File Offset: 0x000E7828
	public void SliceUpdate()
	{
		this.CheckReports();
	}

	// Token: 0x06002C1B RID: 11291 RVA: 0x000E9630 File Offset: 0x000E7830
	private void Start()
	{
		if (GorillaNot.instance == null)
		{
			GorillaNot.instance = this;
		}
		else if (GorillaNot.instance != this)
		{
			Object.Destroy(this);
		}
		RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.OnPlayerEnteredRoom);
		RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeftRoom);
		RoomSystem.JoinedRoomEvent += delegate
		{
			this.cachedPlayerList = (NetworkSystem.Instance.AllNetPlayers ?? new NetPlayer[0]);
		};
		this.logErrorCount = 0;
		Application.logMessageReceived += this.LogErrorCount;
	}

	// Token: 0x06002C1C RID: 11292 RVA: 0x000E96D4 File Offset: 0x000E78D4
	private void OnApplicationPause(bool paused)
	{
		if (paused || !RoomSystem.JoinedRoom)
		{
			return;
		}
		this.lastServerTimestamp = NetworkSystem.Instance.SimTick;
		this.RefreshRPCs();
	}

	// Token: 0x06002C1D RID: 11293 RVA: 0x000E96F8 File Offset: 0x000E78F8
	public void LogErrorCount(string logString, string stackTrace, LogType type)
	{
		if (type == LogType.Error)
		{
			this.logErrorCount++;
			this.stringIndex = logString.LastIndexOf("Sender is ");
			if (logString.Contains("RPC") && this.stringIndex >= 0)
			{
				this.playerID = logString.Substring(this.stringIndex + 10);
				this.tempPlayer = null;
				for (int i = 0; i < this.cachedPlayerList.Length; i++)
				{
					if (this.cachedPlayerList[i].UserId == this.playerID)
					{
						this.tempPlayer = this.cachedPlayerList[i];
						break;
					}
				}
				string text = "invalid RPC stuff";
				if (!this.IncrementRPCTracker(this.tempPlayer, text, this.rpcErrorMax))
				{
					this.SendReport("invalid RPC stuff", this.tempPlayer.UserId, this.tempPlayer.NickName);
				}
				this.tempPlayer = null;
			}
			if (this.logErrorCount > this.logErrorMax)
			{
				Debug.unityLogger.logEnabled = false;
			}
		}
	}

	// Token: 0x06002C1E RID: 11294 RVA: 0x000E97FC File Offset: 0x000E79FC
	public void SendReport(string susReason, string susId, string susNick)
	{
		this.suspiciousReason = susReason;
		this.suspiciousPlayerId = susId;
		this.suspiciousPlayerName = susNick;
		this.sendReport = true;
	}

	// Token: 0x06002C1F RID: 11295 RVA: 0x000E981C File Offset: 0x000E7A1C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void DispatchReport()
	{
		if ((this.sendReport || this.testAssault) && this.suspiciousPlayerId != "" && this.reportedPlayers.IndexOf(this.suspiciousPlayerId) == -1)
		{
			if (this._suspiciousPlayerName.Length > 12)
			{
				this._suspiciousPlayerName = this._suspiciousPlayerName.Remove(12);
			}
			this.reportedPlayers.Add(this.suspiciousPlayerId);
			this.testAssault = false;
			WebFlags flags = new WebFlags(1);
			NetEventOptions options = new NetEventOptions
			{
				TargetActors = GorillaNot.targetActors,
				Reciever = NetEventOptions.RecieverTarget.master,
				Flags = flags
			};
			string[] array = new string[this.cachedPlayerList.Length];
			int num = 0;
			foreach (NetPlayer netPlayer in this.cachedPlayerList)
			{
				array[num] = netPlayer.UserId;
				num++;
			}
			object[] data = new object[]
			{
				NetworkSystem.Instance.RoomStringStripped(),
				array,
				NetworkSystem.Instance.MasterClient.UserId,
				this.suspiciousPlayerId,
				this.suspiciousPlayerName,
				this.suspiciousReason,
				NetworkSystemConfig.AppVersion
			};
			NetworkSystemRaiseEvent.RaiseEvent(8, data, options, true);
			if (this.ShouldDisconnectFromRoom())
			{
				base.StartCoroutine(this.QuitDelay());
			}
		}
		this._sendReport = false;
		this._suspiciousPlayerId = "";
		this._suspiciousPlayerName = "";
		this._suspiciousReason = "";
	}

	// Token: 0x06002C20 RID: 11296 RVA: 0x000E99A0 File Offset: 0x000E7BA0
	private void CheckReports()
	{
		if (Time.time < this.lastCheck + this.reportCheckCooldown)
		{
			return;
		}
		this.lastCheck = Time.time;
		try
		{
			this.logErrorCount = 0;
			if (NetworkSystem.Instance.InRoom)
			{
				this.lastCheck = Time.time;
				this.lastServerTimestamp = NetworkSystem.Instance.SimTick;
				if (!PhotonNetwork.CurrentRoom.PublishUserId)
				{
					this.sendReport = true;
					this.suspiciousReason = "missing player ids";
					this.SetToRoomCreatorIfHere();
					this.CloseInvalidRoom();
				}
				if (this.cachedPlayerList.Length > (int)RoomSystem.GetRoomSize(PhotonNetworkController.Instance.currentGameType))
				{
					this.sendReport = true;
					this.suspiciousReason = "too many players";
					this.SetToRoomCreatorIfHere();
					this.CloseInvalidRoom();
				}
				if (this.currentMasterClient != NetworkSystem.Instance.MasterClient || this.LowestActorNumber() != NetworkSystem.Instance.MasterClient.ActorNumber)
				{
					foreach (NetPlayer netPlayer in this.cachedPlayerList)
					{
						if (this.currentMasterClient == netPlayer)
						{
							this.sendReport = true;
							this.suspiciousReason = "room host force changed";
							this.suspiciousPlayerId = NetworkSystem.Instance.MasterClient.UserId;
							this.suspiciousPlayerName = NetworkSystem.Instance.MasterClient.NickName;
						}
					}
					this.currentMasterClient = NetworkSystem.Instance.MasterClient;
				}
				this.RefreshRPCs();
				this.DispatchReport();
			}
		}
		catch
		{
		}
	}

	// Token: 0x06002C21 RID: 11297 RVA: 0x000E9B28 File Offset: 0x000E7D28
	private void RefreshRPCs()
	{
		foreach (Dictionary<string, GorillaNot.RPCCallTracker> dictionary in this.userRPCCalls.Values)
		{
			foreach (GorillaNot.RPCCallTracker rpccallTracker in dictionary.Values)
			{
				rpccallTracker.RPCCalls = 0;
			}
		}
	}

	// Token: 0x06002C22 RID: 11298 RVA: 0x000E9BB8 File Offset: 0x000E7DB8
	private int LowestActorNumber()
	{
		this.lowestActorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
		foreach (NetPlayer netPlayer in this.cachedPlayerList)
		{
			if (netPlayer.ActorNumber < this.lowestActorNumber)
			{
				this.lowestActorNumber = netPlayer.ActorNumber;
			}
		}
		return this.lowestActorNumber;
	}

	// Token: 0x06002C23 RID: 11299 RVA: 0x000E9C13 File Offset: 0x000E7E13
	public void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		this.cachedPlayerList = (NetworkSystem.Instance.AllNetPlayers ?? new NetPlayer[0]);
	}

	// Token: 0x06002C24 RID: 11300 RVA: 0x000E9C30 File Offset: 0x000E7E30
	public void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		this.cachedPlayerList = (NetworkSystem.Instance.AllNetPlayers ?? new NetPlayer[0]);
		Dictionary<string, GorillaNot.RPCCallTracker> dictionary;
		if (this.userRPCCalls.TryGetValue(otherPlayer.UserId, out dictionary))
		{
			this.userRPCCalls.Remove(otherPlayer.UserId);
		}
	}

	// Token: 0x06002C25 RID: 11301 RVA: 0x000E9C7E File Offset: 0x000E7E7E
	public static void IncrementRPCCall(PhotonMessageInfo info, [CallerMemberName] string callingMethod = "")
	{
		GorillaNot.IncrementRPCCall(new PhotonMessageInfoWrapped(info), callingMethod);
	}

	// Token: 0x06002C26 RID: 11302 RVA: 0x000E9C8C File Offset: 0x000E7E8C
	public static void IncrementRPCCall(PhotonMessageInfoWrapped infoWrapped, [CallerMemberName] string callingMethod = "")
	{
		GorillaNot.instance.IncrementRPCCallLocal(infoWrapped, callingMethod);
	}

	// Token: 0x06002C27 RID: 11303 RVA: 0x000E9C9C File Offset: 0x000E7E9C
	private void IncrementRPCCallLocal(PhotonMessageInfoWrapped infoWrapped, string rpcFunction)
	{
		if (infoWrapped.sentTick < this.lastServerTimestamp)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(infoWrapped.senderID);
		if (player == null)
		{
			return;
		}
		string userId = player.UserId;
		if (!this.IncrementRPCTracker(userId, rpcFunction, this.rpcCallLimit))
		{
			this.SendReport("too many rpc calls! " + rpcFunction, player.UserId, player.NickName);
			return;
		}
	}

	// Token: 0x06002C28 RID: 11304 RVA: 0x000E9D04 File Offset: 0x000E7F04
	private bool IncrementRPCTracker(in NetPlayer sender, in string rpcFunction, in int callLimit)
	{
		string userId = sender.UserId;
		return this.IncrementRPCTracker(userId, rpcFunction, callLimit);
	}

	// Token: 0x06002C29 RID: 11305 RVA: 0x000E9D24 File Offset: 0x000E7F24
	private bool IncrementRPCTracker(in Player sender, in string rpcFunction, in int callLimit)
	{
		string userId = sender.UserId;
		return this.IncrementRPCTracker(userId, rpcFunction, callLimit);
	}

	// Token: 0x06002C2A RID: 11306 RVA: 0x000E9D44 File Offset: 0x000E7F44
	private bool IncrementRPCTracker(in string userId, in string rpcFunction, in int callLimit)
	{
		GorillaNot.RPCCallTracker rpccallTracker = this.GetRPCCallTracker(userId, rpcFunction);
		if (rpccallTracker == null)
		{
			return true;
		}
		rpccallTracker.RPCCalls++;
		if (rpccallTracker.RPCCalls > rpccallTracker.RPCCallsMax)
		{
			rpccallTracker.RPCCallsMax = rpccallTracker.RPCCalls;
		}
		return rpccallTracker.RPCCalls <= callLimit;
	}

	// Token: 0x06002C2B RID: 11307 RVA: 0x000E9D98 File Offset: 0x000E7F98
	private GorillaNot.RPCCallTracker GetRPCCallTracker(string userID, string rpcFunction)
	{
		if (userID == null)
		{
			return null;
		}
		GorillaNot.RPCCallTracker rpccallTracker = null;
		Dictionary<string, GorillaNot.RPCCallTracker> dictionary;
		if (!this.userRPCCalls.TryGetValue(userID, out dictionary))
		{
			rpccallTracker = new GorillaNot.RPCCallTracker
			{
				RPCCalls = 0,
				RPCCallsMax = 0
			};
			Dictionary<string, GorillaNot.RPCCallTracker> dictionary2 = new Dictionary<string, GorillaNot.RPCCallTracker>();
			dictionary2.Add(rpcFunction, rpccallTracker);
			this.userRPCCalls.Add(userID, dictionary2);
		}
		else if (!dictionary.TryGetValue(rpcFunction, out rpccallTracker))
		{
			rpccallTracker = new GorillaNot.RPCCallTracker
			{
				RPCCalls = 0,
				RPCCallsMax = 0
			};
			dictionary.Add(rpcFunction, rpccallTracker);
		}
		return rpccallTracker;
	}

	// Token: 0x06002C2C RID: 11308 RVA: 0x000E9E15 File Offset: 0x000E8015
	private IEnumerator QuitDelay()
	{
		yield return new WaitForSeconds(1f);
		NetworkSystem.Instance.ReturnToSinglePlayer();
		yield break;
	}

	// Token: 0x06002C2D RID: 11309 RVA: 0x000E9E20 File Offset: 0x000E8020
	private void SetToRoomCreatorIfHere()
	{
		this.tempPlayer = PhotonNetwork.CurrentRoom.GetPlayer(1, false);
		if (this.tempPlayer != null)
		{
			this.suspiciousPlayerId = this.tempPlayer.UserId;
			this.suspiciousPlayerName = this.tempPlayer.NickName;
			return;
		}
		this.suspiciousPlayerId = "n/a";
		this.suspiciousPlayerName = "n/a";
	}

	// Token: 0x06002C2E RID: 11310 RVA: 0x000E9E88 File Offset: 0x000E8088
	private bool ShouldDisconnectFromRoom()
	{
		return this._suspiciousReason.Contains("too many players") || this._suspiciousReason.Contains("invalid room name") || this._suspiciousReason.Contains("invalid game mode") || this._suspiciousReason.Contains("missing player ids");
	}

	// Token: 0x06002C2F RID: 11311 RVA: 0x000E9EDD File Offset: 0x000E80DD
	private void CloseInvalidRoom()
	{
		PhotonNetwork.CurrentRoom.IsOpen = false;
		PhotonNetwork.CurrentRoom.IsVisible = false;
		PhotonNetwork.CurrentRoom.MaxPlayers = RoomSystem.GetRoomSize(PhotonNetworkController.Instance.currentGameType);
	}

	// Token: 0x0400378D RID: 14221
	[OnEnterPlay_SetNull]
	public static volatile GorillaNot instance;

	// Token: 0x0400378E RID: 14222
	private bool _sendReport;

	// Token: 0x0400378F RID: 14223
	private string _suspiciousPlayerId = "";

	// Token: 0x04003790 RID: 14224
	private string _suspiciousPlayerName = "";

	// Token: 0x04003791 RID: 14225
	private string _suspiciousReason = "";

	// Token: 0x04003792 RID: 14226
	internal List<string> reportedPlayers = new List<string>();

	// Token: 0x04003793 RID: 14227
	public byte roomSize;

	// Token: 0x04003794 RID: 14228
	public float lastCheck;

	// Token: 0x04003795 RID: 14229
	public float userDecayTime = 15f;

	// Token: 0x04003796 RID: 14230
	public NetPlayer currentMasterClient;

	// Token: 0x04003797 RID: 14231
	public bool testAssault;

	// Token: 0x04003798 RID: 14232
	private const byte ReportAssault = 8;

	// Token: 0x04003799 RID: 14233
	private int lowestActorNumber;

	// Token: 0x0400379A RID: 14234
	private int calls;

	// Token: 0x0400379B RID: 14235
	public int rpcCallLimit = 50;

	// Token: 0x0400379C RID: 14236
	public int logErrorMax = 50;

	// Token: 0x0400379D RID: 14237
	public int rpcErrorMax = 10;

	// Token: 0x0400379E RID: 14238
	private object outObj;

	// Token: 0x0400379F RID: 14239
	private NetPlayer tempPlayer;

	// Token: 0x040037A0 RID: 14240
	private int logErrorCount;

	// Token: 0x040037A1 RID: 14241
	private int stringIndex;

	// Token: 0x040037A2 RID: 14242
	private string playerID;

	// Token: 0x040037A3 RID: 14243
	private string playerNick;

	// Token: 0x040037A4 RID: 14244
	private int lastServerTimestamp;

	// Token: 0x040037A5 RID: 14245
	private const string InvalidRPC = "invalid RPC stuff";

	// Token: 0x040037A6 RID: 14246
	public NetPlayer[] cachedPlayerList;

	// Token: 0x040037A7 RID: 14247
	private float lastReportChecked;

	// Token: 0x040037A8 RID: 14248
	private float reportCheckCooldown = 1f;

	// Token: 0x040037A9 RID: 14249
	private static int[] targetActors = new int[]
	{
		-1
	};

	// Token: 0x040037AA RID: 14250
	private Dictionary<string, Dictionary<string, GorillaNot.RPCCallTracker>> userRPCCalls = new Dictionary<string, Dictionary<string, GorillaNot.RPCCallTracker>>();

	// Token: 0x040037AB RID: 14251
	private Hashtable hashTable;

	// Token: 0x020006F0 RID: 1776
	private class RPCCallTracker
	{
		// Token: 0x040037AC RID: 14252
		public int RPCCalls;

		// Token: 0x040037AD RID: 14253
		public int RPCCallsMax;
	}
}
