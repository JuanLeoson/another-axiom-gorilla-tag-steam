using System;
using GorillaTagScripts.ModIO;
using ModIO;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200084E RID: 2126
internal class VirtualStumpSerializer : GorillaSerializer
{
	// Token: 0x17000502 RID: 1282
	// (get) Token: 0x06003565 RID: 13669 RVA: 0x000DFDB9 File Offset: 0x000DDFB9
	internal bool HasAuthority
	{
		get
		{
			return this.photonView.IsMine;
		}
	}

	// Token: 0x06003566 RID: 13670 RVA: 0x00117FE0 File Offset: 0x001161E0
	protected void Start()
	{
		NetworkSystem.Instance.OnMultiplayerStarted += this.OnJoinedRoom;
		NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerLeftRoom;
	}

	// Token: 0x06003567 RID: 13671 RVA: 0x00118030 File Offset: 0x00116230
	private void OnPlayerLeftRoom(NetPlayer leavingPlayer)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		int driverID = CustomMapsTerminal.GetDriverID();
		if (leavingPlayer.ActorNumber == driverID)
		{
			CustomMapsTerminal.SetTerminalControlStatus(false, -2, true);
		}
	}

	// Token: 0x06003568 RID: 13672 RVA: 0x00118062 File Offset: 0x00116262
	private void OnJoinedRoom()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		this.waitingForRoomInitialization = true;
		base.SendRPC("RequestRoomInitialization_RPC", false, Array.Empty<object>());
	}

	// Token: 0x06003569 RID: 13673 RVA: 0x0011808C File Offset: 0x0011628C
	[PunRPC]
	private void RequestRoomInitialization_RPC(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RequestRoomInitialization_RPC");
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (player.CheckSingleCallRPC(NetPlayer.SingleCallRPC.CMS_RequestRoomInitialization))
		{
			return;
		}
		player.ReceivedSingleCallRPC(NetPlayer.SingleCallRPC.CMS_RequestRoomInitialization);
		CustomMapManager.GetRoomMapId().Equals(ModId.Null);
		long id = CustomMapManager.GetRoomMapId().id;
		bool flag = CustomMapsTerminal.GetDriverID() != -2;
		base.SendRPC("InitializeRoom_RPC", info.Sender, new object[]
		{
			flag,
			CustomMapsTerminal.GetDriverID(),
			CustomMapsTerminal.UpdateAndRetrieveLocalStatus().PackData(true),
			id
		});
	}

	// Token: 0x0600356A RID: 13674 RVA: 0x00118144 File Offset: 0x00116344
	[PunRPC]
	private void InitializeRoom_RPC(bool locked, int driverID, long[] statusData, long loadedMapModID, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "InitializeRoom_RPC");
		if (!info.Sender.IsMasterClient || !this.waitingForRoomInitialization)
		{
			return;
		}
		if (statusData.IsNullOrEmpty<long>())
		{
			return;
		}
		if (driverID != -2 && NetworkSystem.Instance.GetPlayer(driverID) == null)
		{
			return;
		}
		CustomMapsTerminal.ClearTags();
		CustomMapsTerminal.UpdateStatusFromDriver(statusData, driverID, false);
		if (loadedMapModID > 0L)
		{
			CustomMapManager.SetRoomMod(loadedMapModID);
		}
		this.waitingForRoomInitialization = false;
	}

	// Token: 0x0600356B RID: 13675 RVA: 0x001181B4 File Offset: 0x001163B4
	public void LoadModSynced(long modId)
	{
		CustomMapManager.SetRoomMod(modId);
		CustomMapManager.LoadMod(new ModId(modId));
		if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate)
		{
			base.SendRPC("SetRoomMap_RPC", true, new object[]
			{
				modId
			});
		}
	}

	// Token: 0x0600356C RID: 13676 RVA: 0x00118205 File Offset: 0x00116405
	public void UnloadModSynced()
	{
		CustomMapManager.UnloadMod(true);
		if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate)
		{
			base.SendRPC("UnloadMod_RPC", true, Array.Empty<object>());
		}
	}

	// Token: 0x0600356D RID: 13677 RVA: 0x00118238 File Offset: 0x00116438
	[PunRPC]
	private void SetRoomMap_RPC(long modId, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "SetRoomMap_RPC");
		if (modId <= 0L)
		{
			return;
		}
		if (info.Sender.ActorNumber != this.photonView.OwnerActorNr && info.Sender.ActorNumber != CustomMapsTerminal.GetDriverID())
		{
			return;
		}
		if (modId != this.detailsScreen.currentModProfile.id.id)
		{
			return;
		}
		CustomMapManager.SetRoomMod(modId);
	}

	// Token: 0x0600356E RID: 13678 RVA: 0x001182A0 File Offset: 0x001164A0
	[PunRPC]
	private void UnloadMod_RPC(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "UnloadMod_RPC");
		if (info.Sender.ActorNumber != CustomMapsTerminal.GetDriverID())
		{
			return;
		}
		if (!CustomMapManager.AreAllPlayersInVirtualStump())
		{
			return;
		}
		CustomMapManager.UnloadMod(true);
	}

	// Token: 0x0600356F RID: 13679 RVA: 0x001182CF File Offset: 0x001164CF
	public void RequestTerminalControlStatusChange(bool lockedStatus)
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		base.SendRPC("RequestTerminalControlStatusChange_RPC", false, new object[]
		{
			lockedStatus
		});
	}

	// Token: 0x06003570 RID: 13680 RVA: 0x001182FC File Offset: 0x001164FC
	[PunRPC]
	private void RequestTerminalControlStatusChange_RPC(bool lockedStatus, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RequestTerminalControlStatusChange_RPC");
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[19].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		if (!player.IsNull && CustomMapManager.IsRemotePlayerInVirtualStump(info.Sender.UserId))
		{
			CustomMapsTerminal.HandleTerminalControlStatusChangeRequest(lockedStatus, info.Sender.ActorNumber);
		}
	}

	// Token: 0x06003571 RID: 13681 RVA: 0x00118391 File Offset: 0x00116591
	public void SetTerminalControlStatus(bool locked, int playerID)
	{
		if (!NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		base.SendRPC("SetTerminalControlStatus_RPC", true, new object[]
		{
			locked,
			playerID
		});
	}

	// Token: 0x06003572 RID: 13682 RVA: 0x001183D0 File Offset: 0x001165D0
	[PunRPC]
	private void SetTerminalControlStatus_RPC(bool locked, int driverID, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "SetTerminalControlStatus_RPC");
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		if (driverID != -2 && NetworkSystem.Instance.GetPlayer(driverID) == null)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[16].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		CustomMapsTerminal.SetTerminalControlStatus(locked, driverID, false);
	}

	// Token: 0x06003573 RID: 13683 RVA: 0x00118458 File Offset: 0x00116658
	public void SendTerminalStatus(bool sendFullModList = false, bool forceSearch = false)
	{
		if (!NetworkSystem.Instance.InRoom || !CustomMapsTerminal.IsDriver)
		{
			return;
		}
		base.SendRPC("UpdateScreen_RPC", true, new object[]
		{
			CustomMapsTerminal.UpdateAndRetrieveLocalStatus().PackData(sendFullModList),
			forceSearch,
			CustomMapsTerminal.GetDriverID()
		});
	}

	// Token: 0x06003574 RID: 13684 RVA: 0x001184B0 File Offset: 0x001166B0
	[PunRPC]
	private void UpdateScreen_RPC(long[] statusData, bool forceNewSearch, int driverID, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "UpdateScreen_RPC");
		if (info.Sender.ActorNumber != CustomMapsTerminal.GetDriverID() || !CustomMapManager.IsRemotePlayerInVirtualStump(info.Sender.UserId))
		{
			return;
		}
		if (statusData.IsNullOrEmpty<long>())
		{
			return;
		}
		if (NetworkSystem.Instance.GetPlayer(driverID) == null)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[17].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		CustomMapsTerminal.UpdateStatusFromDriver(statusData, driverID, forceNewSearch);
	}

	// Token: 0x06003575 RID: 13685 RVA: 0x00118555 File Offset: 0x00116755
	public void RefreshDriverNickName()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		base.SendRPC("RefreshDriverNickName_RPC", true, Array.Empty<object>());
	}

	// Token: 0x06003576 RID: 13686 RVA: 0x00118578 File Offset: 0x00116778
	[PunRPC]
	private void RefreshDriverNickName_RPC(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RefreshDriverNickName_RPC");
		if (info.Sender.ActorNumber != CustomMapsTerminal.GetDriverID())
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[18].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		CustomMapsTerminal.RefreshDriverNickName();
	}

	// Token: 0x04004260 RID: 16992
	[SerializeField]
	private VirtualStumpBarrierSFX barrierSFX;

	// Token: 0x04004261 RID: 16993
	[SerializeField]
	private CustomMapsDetailsScreen detailsScreen;

	// Token: 0x04004262 RID: 16994
	private bool waitingForRoomInitialization;
}
