using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020002B3 RID: 691
public class FusionCallbackHandler : SimulationBehaviour, INetworkRunnerCallbacks
{
	// Token: 0x06000FF2 RID: 4082 RVA: 0x0005C98C File Offset: 0x0005AB8C
	public void Setup(NetworkSystemFusion parentController)
	{
		this.parent = parentController;
		this.parent.runner.AddCallbacks(new INetworkRunnerCallbacks[]
		{
			this
		});
	}

	// Token: 0x06000FF3 RID: 4083 RVA: 0x0005C9AF File Offset: 0x0005ABAF
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		this.RemoveCallbacks();
	}

	// Token: 0x06000FF4 RID: 4084 RVA: 0x0005C9C0 File Offset: 0x0005ABC0
	private void RemoveCallbacks()
	{
		FusionCallbackHandler.<RemoveCallbacks>d__3 <RemoveCallbacks>d__;
		<RemoveCallbacks>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RemoveCallbacks>d__.<>4__this = this;
		<RemoveCallbacks>d__.<>1__state = -1;
		<RemoveCallbacks>d__.<>t__builder.Start<FusionCallbackHandler.<RemoveCallbacks>d__3>(ref <RemoveCallbacks>d__);
	}

	// Token: 0x06000FF5 RID: 4085 RVA: 0x0005C9F7 File Offset: 0x0005ABF7
	public void OnConnectedToServer(NetworkRunner runner)
	{
		this.parent.OnJoinedSession();
	}

	// Token: 0x06000FF6 RID: 4086 RVA: 0x0005CA04 File Offset: 0x0005AC04
	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
		this.parent.OnJoinFailed(reason);
	}

	// Token: 0x06000FF7 RID: 4087 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
	}

	// Token: 0x06000FF8 RID: 4088 RVA: 0x0005CA14 File Offset: 0x0005AC14
	public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
	{
		this.parent.CustomAuthenticationResponse(data);
		Debug.Log("Received custom auth response:");
		foreach (KeyValuePair<string, object> keyValuePair in data)
		{
			Debug.Log(keyValuePair.Key + ":" + (keyValuePair.Value as string));
		}
	}

	// Token: 0x06000FF9 RID: 4089 RVA: 0x0005CA94 File Offset: 0x0005AC94
	public void OnDisconnectedFromServer(NetworkRunner runner)
	{
		this.parent.OnDisconnectedFromSession();
	}

	// Token: 0x06000FFA RID: 4090 RVA: 0x0005CAA1 File Offset: 0x0005ACA1
	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
		this.parent.MigrateHost(runner, hostMigrationToken);
	}

	// Token: 0x06000FFB RID: 4091 RVA: 0x0005CAB0 File Offset: 0x0005ACB0
	public void OnInput(NetworkRunner runner, NetworkInput input)
	{
		NetworkedInput input2 = NetInput.GetInput();
		input.Set<NetworkedInput>(input2);
	}

	// Token: 0x06000FFC RID: 4092 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
	{
	}

	// Token: 0x06000FFD RID: 4093 RVA: 0x0005CACC File Offset: 0x0005ACCC
	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		this.parent.OnFusionPlayerJoined(player);
	}

	// Token: 0x06000FFE RID: 4094 RVA: 0x0005CADA File Offset: 0x0005ACDA
	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
		this.parent.OnFusionPlayerLeft(player);
	}

	// Token: 0x06000FFF RID: 4095 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
	{
	}

	// Token: 0x06001000 RID: 4096 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnSceneLoadDone(NetworkRunner runner)
	{
	}

	// Token: 0x06001001 RID: 4097 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnSceneLoadStart(NetworkRunner runner)
	{
	}

	// Token: 0x06001002 RID: 4098 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
	}

	// Token: 0x06001003 RID: 4099 RVA: 0x0005CAE8 File Offset: 0x0005ACE8
	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
		this.parent.OnRunnerShutDown();
	}

	// Token: 0x06001004 RID: 4100 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
	{
	}

	// Token: 0x06001005 RID: 4101 RVA: 0x0005CAF8 File Offset: 0x0005ACF8
	[Rpc(Channel = RpcChannel.Reliable)]
	public unsafe static void RPC_OnEventRaisedReliable(NetworkRunner runner, byte eventCode, byte[] byteData, bool hasOps, byte[] netOptsData, RpcInfo info = default(RpcInfo))
	{
		if (NetworkBehaviourUtils.InvokeRpc)
		{
			NetworkBehaviourUtils.InvokeRpc = false;
		}
		else
		{
			if (runner == null)
			{
				throw new ArgumentNullException("runner");
			}
			if (runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			if (runner.HasAnyActiveConnections())
			{
				int num = 8;
				num += 4;
				num += (byteData.Length * 1 + 4 + 3 & -4);
				num += 4;
				num += (netOptsData.Length * 1 + 4 + 3 & -4);
				SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void FusionCallbackHandler::RPC_OnEventRaisedReliable(Fusion.NetworkRunner,System.Byte,System.Byte[],System.Boolean,System.Byte[],Fusion.RpcInfo)")), data);
				data[num2] = eventCode;
				num2 += (1 + 3 & -4);
				*(int*)(data + num2) = byteData.Length;
				num2 += 4;
				num2 = (Native.CopyFromArray<byte>((void*)(data + num2), byteData) + 3 & -4) + num2;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), hasOps);
				num2 += 4;
				*(int*)(data + num2) = netOptsData.Length;
				num2 += 4;
				num2 = (Native.CopyFromArray<byte>((void*)(data + num2), netOptsData) + 3 & -4) + num2;
				ptr->Offset = num2 * 8;
				ptr->SetStatic();
				runner.SendRpc(ptr);
			}
			info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
		}
		object data2 = byteData.ByteDeserialize();
		NetEventOptions opts = null;
		if (hasOps)
		{
			opts = (NetEventOptions)netOptsData.ByteDeserialize();
		}
		if (!FusionCallbackHandler.CanRecieveEvent(runner, opts, info))
		{
			return;
		}
		NetworkSystem.Instance.RaiseEvent(eventCode, data2, info.Source.PlayerId);
	}

	// Token: 0x06001006 RID: 4102 RVA: 0x0005CCFC File Offset: 0x0005AEFC
	[Rpc(Channel = RpcChannel.Unreliable)]
	public unsafe static void RPC_OnEventRaisedUnreliable(NetworkRunner runner, byte eventCode, byte[] byteData, bool hasOps, byte[] netOptsData, RpcInfo info = default(RpcInfo))
	{
		if (NetworkBehaviourUtils.InvokeRpc)
		{
			NetworkBehaviourUtils.InvokeRpc = false;
		}
		else
		{
			if (runner == null)
			{
				throw new ArgumentNullException("runner");
			}
			if (runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			if (runner.HasAnyActiveConnections())
			{
				int num = 8;
				num += 4;
				num += (byteData.Length * 1 + 4 + 3 & -4);
				num += 4;
				num += (netOptsData.Length * 1 + 4 + 3 & -4);
				SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void FusionCallbackHandler::RPC_OnEventRaisedUnreliable(Fusion.NetworkRunner,System.Byte,System.Byte[],System.Boolean,System.Byte[],Fusion.RpcInfo)")), data);
				data[num2] = eventCode;
				num2 += (1 + 3 & -4);
				*(int*)(data + num2) = byteData.Length;
				num2 += 4;
				num2 = (Native.CopyFromArray<byte>((void*)(data + num2), byteData) + 3 & -4) + num2;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), hasOps);
				num2 += 4;
				*(int*)(data + num2) = netOptsData.Length;
				num2 += 4;
				num2 = (Native.CopyFromArray<byte>((void*)(data + num2), netOptsData) + 3 & -4) + num2;
				ptr->Offset = num2 * 8;
				ptr->SetUnreliable();
				ptr->SetStatic();
				runner.SendRpc(ptr);
			}
			info = RpcInfo.FromLocal(runner, RpcChannel.Unreliable, RpcHostMode.SourceIsServer);
		}
		object data2 = byteData.ByteDeserialize();
		NetEventOptions opts = null;
		if (hasOps)
		{
			opts = (NetEventOptions)netOptsData.ByteDeserialize();
		}
		if (!FusionCallbackHandler.CanRecieveEvent(runner, opts, info))
		{
			return;
		}
		NetworkSystem.Instance.RaiseEvent(eventCode, data2, info.Source.PlayerId);
	}

	// Token: 0x06001007 RID: 4103 RVA: 0x0005CF08 File Offset: 0x0005B108
	private static bool CanRecieveEvent(NetworkRunner runner, NetEventOptions opts, RpcInfo info)
	{
		if (opts != null)
		{
			if (opts.Reciever != NetEventOptions.RecieverTarget.all)
			{
				if (opts.Reciever == NetEventOptions.RecieverTarget.master && !NetworkSystem.Instance.IsMasterClient)
				{
					return false;
				}
				if (info.Source == runner.LocalPlayer)
				{
					return false;
				}
			}
			if (opts.TargetActors != null && !opts.TargetActors.Contains(runner.LocalPlayer.PlayerId))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001008 RID: 4104 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x06001009 RID: 4105 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x0600100A RID: 4106 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
	{
	}

	// Token: 0x0600100B RID: 4107 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
	{
	}

	// Token: 0x0600100C RID: 4108 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
	{
	}

	// Token: 0x0600100E RID: 4110 RVA: 0x0005CF7C File Offset: 0x0005B17C
	[NetworkRpcStaticWeavedInvoker("System.Void FusionCallbackHandler::RPC_OnEventRaisedReliable(Fusion.NetworkRunner,System.Byte,System.Byte[],System.Boolean,System.Byte[],Fusion.RpcInfo)")]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_OnEventRaisedReliable@Invoker(NetworkRunner runner, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = RpcHeader.ReadSize(data) + 3 & -4;
		byte b = data[num];
		num += (1 + 3 & -4);
		byte eventCode = b;
		byte[] array = new byte[*(int*)(data + num)];
		num += 4;
		num = (Native.CopyToArray<byte>(array, (void*)(data + num)) + 3 & -4) + num;
		bool flag = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool hasOps = flag;
		byte[] array2 = new byte[*(int*)(data + num)];
		num += 4;
		num = (Native.CopyToArray<byte>(array2, (void*)(data + num)) + 3 & -4) + num;
		RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
		NetworkBehaviourUtils.InvokeRpc = true;
		FusionCallbackHandler.RPC_OnEventRaisedReliable(runner, eventCode, array, hasOps, array2, info);
	}

	// Token: 0x0600100F RID: 4111 RVA: 0x0005D098 File Offset: 0x0005B298
	[NetworkRpcStaticWeavedInvoker("System.Void FusionCallbackHandler::RPC_OnEventRaisedUnreliable(Fusion.NetworkRunner,System.Byte,System.Byte[],System.Boolean,System.Byte[],Fusion.RpcInfo)")]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_OnEventRaisedUnreliable@Invoker(NetworkRunner runner, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = RpcHeader.ReadSize(data) + 3 & -4;
		byte b = data[num];
		num += (1 + 3 & -4);
		byte eventCode = b;
		byte[] array = new byte[*(int*)(data + num)];
		num += 4;
		num = (Native.CopyToArray<byte>(array, (void*)(data + num)) + 3 & -4) + num;
		bool flag = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool hasOps = flag;
		byte[] array2 = new byte[*(int*)(data + num)];
		num += 4;
		num = (Native.CopyToArray<byte>(array2, (void*)(data + num)) + 3 & -4) + num;
		RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
		NetworkBehaviourUtils.InvokeRpc = true;
		FusionCallbackHandler.RPC_OnEventRaisedUnreliable(runner, eventCode, array, hasOps, array2, info);
	}

	// Token: 0x04001874 RID: 6260
	private NetworkSystemFusion parent;
}
