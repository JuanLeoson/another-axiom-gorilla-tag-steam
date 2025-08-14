using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Fusion;
using Fusion.Sockets;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x0200023E RID: 574
internal class RequestableOwnershipGaurdHandler : IPunOwnershipCallbacks, IInRoomCallbacks, INetworkRunnerCallbacks
{
	// Token: 0x06000D4F RID: 3407 RVA: 0x00052700 File Offset: 0x00050900
	static RequestableOwnershipGaurdHandler()
	{
		PhotonNetwork.AddCallbackTarget(RequestableOwnershipGaurdHandler.callbackInstance);
	}

	// Token: 0x06000D50 RID: 3408 RVA: 0x0005272A File Offset: 0x0005092A
	internal static void RegisterView(NetworkView view, RequestableOwnershipGuard guard)
	{
		if (view == null || RequestableOwnershipGaurdHandler.gaurdedViews.Contains(view))
		{
			return;
		}
		RequestableOwnershipGaurdHandler.gaurdedViews.Add(view);
		RequestableOwnershipGaurdHandler.guardingLookup.Add(view, guard);
	}

	// Token: 0x06000D51 RID: 3409 RVA: 0x0005275B File Offset: 0x0005095B
	internal static void RemoveView(NetworkView view)
	{
		if (view == null)
		{
			return;
		}
		RequestableOwnershipGaurdHandler.gaurdedViews.Remove(view);
		RequestableOwnershipGaurdHandler.guardingLookup.Remove(view);
	}

	// Token: 0x06000D52 RID: 3410 RVA: 0x00052780 File Offset: 0x00050980
	internal static void RegisterViews(NetworkView[] views, RequestableOwnershipGuard guard)
	{
		for (int i = 0; i < views.Length; i++)
		{
			RequestableOwnershipGaurdHandler.RegisterView(views[i], guard);
		}
	}

	// Token: 0x06000D53 RID: 3411 RVA: 0x000527A8 File Offset: 0x000509A8
	public static void RemoveViews(NetworkView[] views, RequestableOwnershipGuard guard)
	{
		for (int i = 0; i < views.Length; i++)
		{
			RequestableOwnershipGaurdHandler.RemoveView(views[i]);
		}
	}

	// Token: 0x06000D54 RID: 3412 RVA: 0x000527D0 File Offset: 0x000509D0
	void IPunOwnershipCallbacks.OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
	{
		NetworkView networkView = RequestableOwnershipGaurdHandler.gaurdedViews.FirstOrDefault((NetworkView p) => p.GetView == targetView);
		RequestableOwnershipGuard requestableOwnershipGuard;
		if (networkView.IsNull() || !RequestableOwnershipGaurdHandler.guardingLookup.TryGetValue(networkView, out requestableOwnershipGuard) || requestableOwnershipGuard.IsNull())
		{
			return;
		}
		NetPlayer currentOwner = requestableOwnershipGuard.currentOwner;
		Player player = (currentOwner != null) ? currentOwner.GetPlayerRef() : null;
		int num = (player != null) ? player.ActorNumber : 0;
		if (num == 0 || previousOwner != player)
		{
			GTDev.LogWarning<string>("Ownership transferred but the previous owner didn't initiate the request, Switching back", null);
			targetView.OwnerActorNr = num;
			targetView.ControllerActorNr = num;
		}
	}

	// Token: 0x06000D55 RID: 3413 RVA: 0x0005286F File Offset: 0x00050A6F
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
		this.OnHostChangedShared();
	}

	// Token: 0x06000D56 RID: 3414 RVA: 0x0005286F File Offset: 0x00050A6F
	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
		this.OnHostChangedShared();
	}

	// Token: 0x06000D57 RID: 3415 RVA: 0x00052878 File Offset: 0x00050A78
	private void OnHostChangedShared()
	{
		foreach (NetworkView networkView in RequestableOwnershipGaurdHandler.gaurdedViews)
		{
			RequestableOwnershipGuard requestableOwnershipGuard;
			if (!RequestableOwnershipGaurdHandler.guardingLookup.TryGetValue(networkView, out requestableOwnershipGuard))
			{
				break;
			}
			if (networkView.Owner != null && requestableOwnershipGuard.currentOwner != null && !object.Equals(networkView.Owner, requestableOwnershipGuard.currentOwner))
			{
				networkView.OwnerActorNr = requestableOwnershipGuard.currentOwner.ActorNumber;
				networkView.ControllerActorNr = requestableOwnershipGuard.currentOwner.ActorNumber;
			}
		}
	}

	// Token: 0x06000D58 RID: 3416 RVA: 0x000023F5 File Offset: 0x000005F5
	void IPunOwnershipCallbacks.OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
	{
	}

	// Token: 0x06000D59 RID: 3417 RVA: 0x000023F5 File Offset: 0x000005F5
	void IPunOwnershipCallbacks.OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
	{
	}

	// Token: 0x06000D5A RID: 3418 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x06000D5B RID: 3419 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnPlayerLeftRoom(Player otherPlayer)
	{
	}

	// Token: 0x06000D5C RID: 3420 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x06000D5D RID: 3421 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x06000D5E RID: 3422 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x06000D5F RID: 3423 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x06000D60 RID: 3424 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
	}

	// Token: 0x06000D61 RID: 3425 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
	}

	// Token: 0x06000D62 RID: 3426 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnInput(NetworkRunner runner, NetworkInput input)
	{
	}

	// Token: 0x06000D63 RID: 3427 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
	{
	}

	// Token: 0x06000D64 RID: 3428 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
	}

	// Token: 0x06000D65 RID: 3429 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnConnectedToServer(NetworkRunner runner)
	{
	}

	// Token: 0x06000D66 RID: 3430 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
	{
	}

	// Token: 0x06000D67 RID: 3431 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
	}

	// Token: 0x06000D68 RID: 3432 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
	}

	// Token: 0x06000D69 RID: 3433 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
	{
	}

	// Token: 0x06000D6A RID: 3434 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
	}

	// Token: 0x06000D6B RID: 3435 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
	{
	}

	// Token: 0x06000D6C RID: 3436 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
	{
	}

	// Token: 0x06000D6D RID: 3437 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
	{
	}

	// Token: 0x06000D6E RID: 3438 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnSceneLoadDone(NetworkRunner runner)
	{
	}

	// Token: 0x06000D6F RID: 3439 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnSceneLoadStart(NetworkRunner runner)
	{
	}

	// Token: 0x04001558 RID: 5464
	private static HashSet<NetworkView> gaurdedViews = new HashSet<NetworkView>();

	// Token: 0x04001559 RID: 5465
	private static readonly RequestableOwnershipGaurdHandler callbackInstance = new RequestableOwnershipGaurdHandler();

	// Token: 0x0400155A RID: 5466
	private static Dictionary<NetworkView, RequestableOwnershipGuard> guardingLookup = new Dictionary<NetworkView, RequestableOwnershipGuard>();
}
