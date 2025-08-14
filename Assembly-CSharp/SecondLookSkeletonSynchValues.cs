using System;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020000DB RID: 219
[NetworkBehaviourWeaved(11)]
public class SecondLookSkeletonSynchValues : NetworkComponent
{
	// Token: 0x1700006A RID: 106
	// (get) Token: 0x0600056C RID: 1388 RVA: 0x0001F84A File Offset: 0x0001DA4A
	// (set) Token: 0x0600056D RID: 1389 RVA: 0x0001F874 File Offset: 0x0001DA74
	[Networked]
	[NetworkedWeaved(0, 11)]
	public unsafe SkeletonNetData NetData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing SecondLookSkeletonSynchValues.NetData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(SkeletonNetData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing SecondLookSkeletonSynchValues.NetData. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(SkeletonNetData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x0600056E RID: 1390 RVA: 0x0001F89F File Offset: 0x0001DA9F
	protected override void OnOwnerSwitched(NetPlayer newOwningPlayer)
	{
		base.OnOwnerSwitched(newOwningPlayer);
		if (newOwningPlayer.IsLocal)
		{
			this.mySkeleton.SetNodes();
			if (this.mySkeleton.currentState != this.currentState)
			{
				this.mySkeleton.ChangeState(this.currentState);
			}
		}
	}

	// Token: 0x0600056F RID: 1391 RVA: 0x0001F8DF File Offset: 0x0001DADF
	public override void WriteDataFusion()
	{
		this.NetData = new SkeletonNetData((int)this.currentState, this.position, this.rotation, this.currentNode, this.nextNode, this.angerPoint);
	}

	// Token: 0x06000570 RID: 1392 RVA: 0x0001F910 File Offset: 0x0001DB10
	public override void ReadDataFusion()
	{
		this.currentState = (SecondLookSkeleton.GhostState)this.NetData.CurrentState;
		Vector3 vector = this.NetData.Position;
		ref this.position.SetValueSafe(vector);
		Quaternion quaternion = this.NetData.Rotation;
		ref this.rotation.SetValueSafe(quaternion);
		this.currentNode = this.NetData.CurrentNode;
		this.nextNode = this.NetData.NextNode;
		this.angerPoint = this.NetData.AngerPoint;
		if (this.mySkeleton.tapped && this.currentState != this.mySkeleton.currentState)
		{
			this.mySkeleton.ChangeState(this.currentState);
		}
	}

	// Token: 0x06000571 RID: 1393 RVA: 0x0001F9D8 File Offset: 0x0001DBD8
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!base.IsMine && !info.Sender.IsMasterClient)
		{
			return;
		}
		stream.SendNext(this.mySkeleton.currentState);
		stream.SendNext(this.mySkeleton.spookyGhost.transform.position);
		stream.SendNext(this.mySkeleton.spookyGhost.transform.rotation);
		stream.SendNext(this.currentNode);
		stream.SendNext(this.nextNode);
		stream.SendNext(this.angerPoint);
	}

	// Token: 0x06000572 RID: 1394 RVA: 0x0001FA84 File Offset: 0x0001DC84
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!base.IsMine && !info.Sender.IsMasterClient)
		{
			return;
		}
		this.currentState = (SecondLookSkeleton.GhostState)stream.ReceiveNext();
		Vector3 vector = (Vector3)stream.ReceiveNext();
		ref this.position.SetValueSafe(vector);
		Quaternion quaternion = (Quaternion)stream.ReceiveNext();
		ref this.rotation.SetValueSafe(quaternion);
		this.currentNode = (int)stream.ReceiveNext();
		this.nextNode = (int)stream.ReceiveNext();
		this.angerPoint = (int)stream.ReceiveNext();
		if (this.mySkeleton.tapped && this.currentState != this.mySkeleton.currentState)
		{
			this.mySkeleton.ChangeState(this.currentState);
		}
	}

	// Token: 0x06000573 RID: 1395 RVA: 0x0001FB50 File Offset: 0x0001DD50
	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RPC_RemoteActiveGhost(RpcInfo info = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != SimulationStages.Resimulate)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 7) != 0)
				{
					if ((localAuthorityMask & 1) != 1)
					{
						if (base.Runner.HasAnyActiveConnections())
						{
							int capacityInBytes = 8;
							SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, capacityInBytes);
							byte* data = SimulationMessage.GetData(ptr);
							int num = RpcHeader.Write(RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1), data);
							ptr->Offset = num * 8;
							base.Runner.SendRpc(ptr);
						}
						if ((localAuthorityMask & 1) == 0)
						{
							return;
						}
					}
					info = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
					goto IL_12;
				}
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void SecondLookSkeletonSynchValues::RPC_RemoteActiveGhost(Fusion.RpcInfo)", base.Object, 7);
			}
			return;
		}
		this.InvokeRpc = false;
		IL_12:
		GorillaNot.IncrementRPCCall(info, "RPC_RemoteActiveGhost");
		if (!base.IsMine)
		{
			return;
		}
		this.mySkeleton.RemoteActivateGhost();
	}

	// Token: 0x06000574 RID: 1396 RVA: 0x0001FC94 File Offset: 0x0001DE94
	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RPC_RemotePlayerSeen(RpcInfo info = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != SimulationStages.Resimulate)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 7) == 0)
				{
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void SecondLookSkeletonSynchValues::RPC_RemotePlayerSeen(Fusion.RpcInfo)", base.Object, 7);
				}
				else
				{
					if (base.Runner.HasAnyActiveConnections())
					{
						int capacityInBytes = 8;
						SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, capacityInBytes);
						byte* data = SimulationMessage.GetData(ptr);
						int num = RpcHeader.Write(RpcHeader.Create(base.Object.Id, this.ObjectIndex, 2), data);
						ptr->Offset = num * 8;
						base.Runner.SendRpc(ptr);
					}
					if ((localAuthorityMask & 7) != 0)
					{
						info = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
						goto IL_12;
					}
				}
			}
			return;
		}
		this.InvokeRpc = false;
		IL_12:
		GorillaNot.IncrementRPCCall(info, "RPC_RemotePlayerSeen");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Source);
		if (!this.mySkeleton.playersSeen.Contains(player))
		{
			this.mySkeleton.RemotePlayerSeen(player);
		}
	}

	// Token: 0x06000575 RID: 1397 RVA: 0x0001FDE8 File Offset: 0x0001DFE8
	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RPC_RemotePlayerCaught(RpcInfo info = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != SimulationStages.Resimulate)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 7) != 0)
				{
					if ((localAuthorityMask & 1) != 1)
					{
						if (base.Runner.HasAnyActiveConnections())
						{
							int capacityInBytes = 8;
							SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, capacityInBytes);
							byte* data = SimulationMessage.GetData(ptr);
							int num = RpcHeader.Write(RpcHeader.Create(base.Object.Id, this.ObjectIndex, 3), data);
							ptr->Offset = num * 8;
							base.Runner.SendRpc(ptr);
						}
						if ((localAuthorityMask & 1) == 0)
						{
							return;
						}
					}
					info = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
					goto IL_12;
				}
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void SecondLookSkeletonSynchValues::RPC_RemotePlayerCaught(Fusion.RpcInfo)", base.Object, 7);
			}
			return;
		}
		this.InvokeRpc = false;
		IL_12:
		GorillaNot.IncrementRPCCall(info, "RPC_RemotePlayerCaught");
		if (!base.IsMine)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Source);
		if (this.mySkeleton.currentState == SecondLookSkeleton.GhostState.Chasing)
		{
			this.mySkeleton.RemotePlayerCaught(player);
		}
	}

	// Token: 0x06000576 RID: 1398 RVA: 0x0001FF49 File Offset: 0x0001E149
	[PunRPC]
	public void RemoteActivateGhost(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RemoteActivateGhost");
		if (!base.IsMine)
		{
			return;
		}
		this.mySkeleton.RemoteActivateGhost();
	}

	// Token: 0x06000577 RID: 1399 RVA: 0x0001FF6C File Offset: 0x0001E16C
	[PunRPC]
	public void RemotePlayerSeen(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RemotePlayerSeen");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (!this.mySkeleton.playersSeen.Contains(player))
		{
			this.mySkeleton.RemotePlayerSeen(player);
		}
	}

	// Token: 0x06000578 RID: 1400 RVA: 0x0001FFB4 File Offset: 0x0001E1B4
	[PunRPC]
	public void RemotePlayerCaught(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RemotePlayerCaught");
		if (!base.IsMine)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (this.mySkeleton.currentState == SecondLookSkeleton.GhostState.Chasing)
		{
			this.mySkeleton.RemotePlayerCaught(player);
		}
	}

	// Token: 0x0600057A RID: 1402 RVA: 0x00020000 File Offset: 0x0001E200
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.NetData = this._NetData;
	}

	// Token: 0x0600057B RID: 1403 RVA: 0x00020018 File Offset: 0x0001E218
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._NetData = this.NetData;
	}

	// Token: 0x0600057C RID: 1404 RVA: 0x0002002C File Offset: 0x0001E22C
	[NetworkRpcWeavedInvoker(1, 7, 1)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_RemoteActiveGhost@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = RpcHeader.ReadSize(data) + 3 & -4;
		RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((SecondLookSkeletonSynchValues)behaviour).RPC_RemoteActiveGhost(info);
	}

	// Token: 0x0600057D RID: 1405 RVA: 0x00020080 File Offset: 0x0001E280
	[NetworkRpcWeavedInvoker(2, 7, 7)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_RemotePlayerSeen@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = RpcHeader.ReadSize(data) + 3 & -4;
		RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((SecondLookSkeletonSynchValues)behaviour).RPC_RemotePlayerSeen(info);
	}

	// Token: 0x0600057E RID: 1406 RVA: 0x000200D4 File Offset: 0x0001E2D4
	[NetworkRpcWeavedInvoker(3, 7, 1)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_RemotePlayerCaught@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = RpcHeader.ReadSize(data) + 3 & -4;
		RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((SecondLookSkeletonSynchValues)behaviour).RPC_RemotePlayerCaught(info);
	}

	// Token: 0x04000675 RID: 1653
	public SecondLookSkeleton.GhostState currentState;

	// Token: 0x04000676 RID: 1654
	public Vector3 position;

	// Token: 0x04000677 RID: 1655
	public Quaternion rotation;

	// Token: 0x04000678 RID: 1656
	public SecondLookSkeleton mySkeleton;

	// Token: 0x04000679 RID: 1657
	public int currentNode;

	// Token: 0x0400067A RID: 1658
	public int nextNode;

	// Token: 0x0400067B RID: 1659
	public int angerPoint;

	// Token: 0x0400067C RID: 1660
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("NetData", 0, 11)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private SkeletonNetData _NetData;
}
