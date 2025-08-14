using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;

// Token: 0x02000A12 RID: 2578
public class PhotonUserCache : IInRoomCallbacks, IMatchmakingCallbacks
{
	// Token: 0x06003EE0 RID: 16096 RVA: 0x000023F5 File Offset: 0x000005F5
	void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x06003EE1 RID: 16097 RVA: 0x000023F5 File Offset: 0x000005F5
	void IMatchmakingCallbacks.OnJoinedRoom()
	{
	}

	// Token: 0x06003EE2 RID: 16098 RVA: 0x000023F5 File Offset: 0x000005F5
	void IMatchmakingCallbacks.OnLeftRoom()
	{
	}

	// Token: 0x06003EE3 RID: 16099 RVA: 0x000023F5 File Offset: 0x000005F5
	void IInRoomCallbacks.OnPlayerLeftRoom(Player player)
	{
	}

	// Token: 0x06003EE4 RID: 16100 RVA: 0x000023F5 File Offset: 0x000005F5
	void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
	{
	}

	// Token: 0x06003EE5 RID: 16101 RVA: 0x000023F5 File Offset: 0x000005F5
	void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
	{
	}

	// Token: 0x06003EE6 RID: 16102 RVA: 0x000023F5 File Offset: 0x000005F5
	void IMatchmakingCallbacks.OnCreatedRoom()
	{
	}

	// Token: 0x06003EE7 RID: 16103 RVA: 0x000023F5 File Offset: 0x000005F5
	void IMatchmakingCallbacks.OnPreLeavingRoom()
	{
	}

	// Token: 0x06003EE8 RID: 16104 RVA: 0x000023F5 File Offset: 0x000005F5
	void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
	{
	}

	// Token: 0x06003EE9 RID: 16105 RVA: 0x000023F5 File Offset: 0x000005F5
	void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
	{
	}

	// Token: 0x06003EEA RID: 16106 RVA: 0x000023F5 File Offset: 0x000005F5
	void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable changedProperties)
	{
	}

	// Token: 0x06003EEB RID: 16107 RVA: 0x000023F5 File Offset: 0x000005F5
	void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player player, Hashtable changedProperties)
	{
	}

	// Token: 0x06003EEC RID: 16108 RVA: 0x000023F5 File Offset: 0x000005F5
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
	}
}
