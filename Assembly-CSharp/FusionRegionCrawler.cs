using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using UnityEngine;

// Token: 0x020002BA RID: 698
public class FusionRegionCrawler : MonoBehaviour, INetworkRunnerCallbacks
{
	// Token: 0x1700019A RID: 410
	// (get) Token: 0x0600103D RID: 4157 RVA: 0x0005DC14 File Offset: 0x0005BE14
	public int PlayerCountGlobal
	{
		get
		{
			return this.globalPlayerCount;
		}
	}

	// Token: 0x0600103E RID: 4158 RVA: 0x0005DC1C File Offset: 0x0005BE1C
	public void Start()
	{
		this.regionRunner = base.gameObject.AddComponent<NetworkRunner>();
		this.regionRunner.AddCallbacks(new INetworkRunnerCallbacks[]
		{
			this
		});
		base.StartCoroutine(this.OccasionalUpdate());
	}

	// Token: 0x0600103F RID: 4159 RVA: 0x0005DC51 File Offset: 0x0005BE51
	public IEnumerator OccasionalUpdate()
	{
		while (this.refreshPlayerCountAutomatically)
		{
			yield return this.UpdatePlayerCount();
			yield return new WaitForSeconds(this.UpdateFrequency);
		}
		yield break;
	}

	// Token: 0x06001040 RID: 4160 RVA: 0x0005DC60 File Offset: 0x0005BE60
	public IEnumerator UpdatePlayerCount()
	{
		int tempGlobalPlayerCount = 0;
		StartGameArgs startGameArgs = default(StartGameArgs);
		foreach (string fixedRegion in NetworkSystem.Instance.regionNames)
		{
			startGameArgs.CustomPhotonAppSettings = new FusionAppSettings();
			startGameArgs.CustomPhotonAppSettings.FixedRegion = fixedRegion;
			this.waitingForSessionListUpdate = true;
			this.regionRunner.JoinSessionLobby(SessionLobby.ClientServer, startGameArgs.CustomPhotonAppSettings.FixedRegion, null, null, new bool?(false), default(CancellationToken), false);
			while (this.waitingForSessionListUpdate)
			{
				yield return new WaitForEndOfFrame();
			}
			foreach (SessionInfo sessionInfo in this.sessionInfoCache)
			{
				tempGlobalPlayerCount += sessionInfo.PlayerCount;
			}
			tempGlobalPlayerCount += this.tempSessionPlayerCount;
		}
		string[] array = null;
		this.globalPlayerCount = tempGlobalPlayerCount;
		FusionRegionCrawler.PlayerCountUpdated onPlayerCountUpdated = this.OnPlayerCountUpdated;
		if (onPlayerCountUpdated != null)
		{
			onPlayerCountUpdated(this.globalPlayerCount);
		}
		yield break;
	}

	// Token: 0x06001041 RID: 4161 RVA: 0x0005DC6F File Offset: 0x0005BE6F
	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
		if (this.waitingForSessionListUpdate)
		{
			this.sessionInfoCache = sessionList;
			this.waitingForSessionListUpdate = false;
		}
	}

	// Token: 0x06001042 RID: 4162 RVA: 0x000023F5 File Offset: 0x000005F5
	void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
	}

	// Token: 0x06001043 RID: 4163 RVA: 0x000023F5 File Offset: 0x000005F5
	void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
	}

	// Token: 0x06001044 RID: 4164 RVA: 0x000023F5 File Offset: 0x000005F5
	void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
	{
	}

	// Token: 0x06001045 RID: 4165 RVA: 0x000023F5 File Offset: 0x000005F5
	void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
	{
	}

	// Token: 0x06001046 RID: 4166 RVA: 0x000023F5 File Offset: 0x000005F5
	void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
	}

	// Token: 0x06001047 RID: 4167 RVA: 0x000023F5 File Offset: 0x000005F5
	void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
	{
	}

	// Token: 0x06001048 RID: 4168 RVA: 0x000023F5 File Offset: 0x000005F5
	void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
	}

	// Token: 0x06001049 RID: 4169 RVA: 0x000023F5 File Offset: 0x000005F5
	void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
	}

	// Token: 0x0600104A RID: 4170 RVA: 0x000023F5 File Offset: 0x000005F5
	void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
	{
	}

	// Token: 0x0600104B RID: 4171 RVA: 0x000023F5 File Offset: 0x000005F5
	void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
	}

	// Token: 0x0600104C RID: 4172 RVA: 0x000023F5 File Offset: 0x000005F5
	void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
	{
	}

	// Token: 0x0600104D RID: 4173 RVA: 0x000023F5 File Offset: 0x000005F5
	void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
	}

	// Token: 0x0600104E RID: 4174 RVA: 0x000023F5 File Offset: 0x000005F5
	void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner)
	{
	}

	// Token: 0x0600104F RID: 4175 RVA: 0x000023F5 File Offset: 0x000005F5
	void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner)
	{
	}

	// Token: 0x06001050 RID: 4176 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x06001051 RID: 4177 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x06001052 RID: 4178 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
	{
	}

	// Token: 0x06001053 RID: 4179 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
	{
	}

	// Token: 0x06001054 RID: 4180 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
	{
	}

	// Token: 0x04001880 RID: 6272
	public FusionRegionCrawler.PlayerCountUpdated OnPlayerCountUpdated;

	// Token: 0x04001881 RID: 6273
	private NetworkRunner regionRunner;

	// Token: 0x04001882 RID: 6274
	private List<SessionInfo> sessionInfoCache;

	// Token: 0x04001883 RID: 6275
	private bool waitingForSessionListUpdate;

	// Token: 0x04001884 RID: 6276
	private int globalPlayerCount;

	// Token: 0x04001885 RID: 6277
	private float UpdateFrequency = 10f;

	// Token: 0x04001886 RID: 6278
	private bool refreshPlayerCountAutomatically = true;

	// Token: 0x04001887 RID: 6279
	private int tempSessionPlayerCount;

	// Token: 0x020002BB RID: 699
	// (Invoke) Token: 0x06001057 RID: 4183
	public delegate void PlayerCountUpdated(int playerCount);
}
