using System;
using Fusion;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Scripting;

// Token: 0x020000D4 RID: 212
[NetworkBehaviourWeaved(11)]
public class GhostLabReliableState : NetworkComponent
{
	// Token: 0x17000063 RID: 99
	// (get) Token: 0x06000525 RID: 1317 RVA: 0x0001D9BC File Offset: 0x0001BBBC
	// (set) Token: 0x06000526 RID: 1318 RVA: 0x0001D9E6 File Offset: 0x0001BBE6
	[Networked]
	[NetworkedWeaved(0, 11)]
	private unsafe GhostLabData NetData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GhostLabReliableState.NetData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(GhostLabData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GhostLabReliableState.NetData. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(GhostLabData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06000527 RID: 1319 RVA: 0x0001DA11 File Offset: 0x0001BC11
	protected override void Awake()
	{
		base.Awake();
		this.singleDoorOpen = new bool[this.singleDoorCount];
	}

	// Token: 0x06000528 RID: 1320 RVA: 0x0001DA2A File Offset: 0x0001BC2A
	public override void OnOwnerChange(Player newOwner, Player previousOwner)
	{
		base.OnOwnerChange(newOwner, previousOwner);
		Player localPlayer = PhotonNetwork.LocalPlayer;
	}

	// Token: 0x06000529 RID: 1321 RVA: 0x0001DA3C File Offset: 0x0001BC3C
	public override void WriteDataFusion()
	{
		this.NetData = new GhostLabData((int)this.doorState, this.singleDoorOpen);
	}

	// Token: 0x0600052A RID: 1322 RVA: 0x0001DA58 File Offset: 0x0001BC58
	public override void ReadDataFusion()
	{
		this.doorState = (GhostLab.EntranceDoorsState)this.NetData.DoorState;
		for (int i = 0; i < this.singleDoorCount; i++)
		{
			this.singleDoorOpen[i] = this.NetData.OpenDoors[i];
		}
	}

	// Token: 0x0600052B RID: 1323 RVA: 0x0001DAB0 File Offset: 0x0001BCB0
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!base.IsMine && !info.Sender.IsMasterClient)
		{
			return;
		}
		stream.SendNext(this.doorState);
		for (int i = 0; i < this.singleDoorOpen.Length; i++)
		{
			stream.SendNext(this.singleDoorOpen[i]);
		}
	}

	// Token: 0x0600052C RID: 1324 RVA: 0x0001DB0C File Offset: 0x0001BD0C
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!base.IsMine && !info.Sender.IsMasterClient)
		{
			return;
		}
		this.doorState = (GhostLab.EntranceDoorsState)stream.ReceiveNext();
		for (int i = 0; i < this.singleDoorOpen.Length; i++)
		{
			this.singleDoorOpen[i] = (bool)stream.ReceiveNext();
		}
	}

	// Token: 0x0600052D RID: 1325 RVA: 0x0001DB68 File Offset: 0x0001BD68
	public void UpdateEntranceDoorsState(GhostLab.EntranceDoorsState newState)
	{
		if (!NetworkSystem.Instance.InRoom || NetworkSystem.Instance.IsMasterClient)
		{
			this.doorState = newState;
			return;
		}
		if (NetworkSystem.Instance.InRoom && !NetworkSystem.Instance.IsMasterClient)
		{
			base.SendRPC("RemoteEntranceDoorState", RpcTarget.MasterClient, new object[]
			{
				newState
			});
		}
	}

	// Token: 0x0600052E RID: 1326 RVA: 0x0001DBC8 File Offset: 0x0001BDC8
	public void UpdateSingleDoorState(int singleDoorIndex)
	{
		if (!NetworkSystem.Instance.InRoom || NetworkSystem.Instance.IsMasterClient)
		{
			this.singleDoorOpen[singleDoorIndex] = !this.singleDoorOpen[singleDoorIndex];
			return;
		}
		if (NetworkSystem.Instance.InRoom && !NetworkSystem.Instance.IsMasterClient)
		{
			base.SendRPC("RemoteSingleDoorState", RpcTarget.MasterClient, new object[]
			{
				singleDoorIndex
			});
		}
	}

	// Token: 0x0600052F RID: 1327 RVA: 0x0001DC34 File Offset: 0x0001BE34
	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RPC_RemoteEntranceDoorState(GhostLab.EntranceDoorsState newState, RpcInfo info = default(RpcInfo))
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
							int num = 8;
							num += 4;
							SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
							byte* data = SimulationMessage.GetData(ptr);
							int num2 = RpcHeader.Write(RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1), data);
							*(GhostLab.EntranceDoorsState*)(data + num2) = newState;
							num2 += 4;
							ptr->Offset = num2 * 8;
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GhostLabReliableState::RPC_RemoteEntranceDoorState(GhostLab/EntranceDoorsState,Fusion.RpcInfo)", base.Object, 7);
			}
			return;
		}
		this.InvokeRpc = false;
		IL_12:
		GorillaNot.IncrementRPCCall(info, "RPC_RemoteEntranceDoorState");
		if (!base.IsMine)
		{
			return;
		}
		this.doorState = newState;
	}

	// Token: 0x06000530 RID: 1328 RVA: 0x0001DD98 File Offset: 0x0001BF98
	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RPC_RemoteSingleDoorState(int doorIndex, RpcInfo info = default(RpcInfo))
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
							int num = 8;
							num += 4;
							SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
							byte* data = SimulationMessage.GetData(ptr);
							int num2 = RpcHeader.Write(RpcHeader.Create(base.Object.Id, this.ObjectIndex, 2), data);
							*(int*)(data + num2) = doorIndex;
							num2 += 4;
							ptr->Offset = num2 * 8;
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GhostLabReliableState::RPC_RemoteSingleDoorState(System.Int32,Fusion.RpcInfo)", base.Object, 7);
			}
			return;
		}
		this.InvokeRpc = false;
		IL_12:
		GorillaNot.IncrementRPCCall(info, "RPC_RemoteSingleDoorState");
		if (!base.IsMine)
		{
			return;
		}
		if (doorIndex >= this.singleDoorCount)
		{
			return;
		}
		this.singleDoorOpen[doorIndex] = !this.singleDoorOpen[doorIndex];
	}

	// Token: 0x06000531 RID: 1329 RVA: 0x0001DF0D File Offset: 0x0001C10D
	[PunRPC]
	public void RemoteEntranceDoorState(GhostLab.EntranceDoorsState newState, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RemoteEntranceDoorState");
		if (!base.IsMine)
		{
			return;
		}
		this.doorState = newState;
	}

	// Token: 0x06000532 RID: 1330 RVA: 0x0001DF2A File Offset: 0x0001C12A
	[PunRPC]
	public void RemoteSingleDoorState(int doorIndex, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RemoteSingleDoorState");
		if (!base.IsMine)
		{
			return;
		}
		if (doorIndex >= this.singleDoorCount)
		{
			return;
		}
		this.singleDoorOpen[doorIndex] = !this.singleDoorOpen[doorIndex];
	}

	// Token: 0x06000534 RID: 1332 RVA: 0x0001DF5D File Offset: 0x0001C15D
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.NetData = this._NetData;
	}

	// Token: 0x06000535 RID: 1333 RVA: 0x0001DF75 File Offset: 0x0001C175
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._NetData = this.NetData;
	}

	// Token: 0x06000536 RID: 1334 RVA: 0x0001DF8C File Offset: 0x0001C18C
	[NetworkRpcWeavedInvoker(1, 7, 1)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_RemoteEntranceDoorState@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = RpcHeader.ReadSize(data) + 3 & -4;
		GhostLab.EntranceDoorsState entranceDoorsState = *(GhostLab.EntranceDoorsState*)(data + num);
		num += 4;
		GhostLab.EntranceDoorsState newState = entranceDoorsState;
		RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((GhostLabReliableState)behaviour).RPC_RemoteEntranceDoorState(newState, info);
	}

	// Token: 0x06000537 RID: 1335 RVA: 0x0001E000 File Offset: 0x0001C200
	[NetworkRpcWeavedInvoker(2, 7, 1)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_RemoteSingleDoorState@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = RpcHeader.ReadSize(data) + 3 & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int doorIndex = num2;
		RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((GhostLabReliableState)behaviour).RPC_RemoteSingleDoorState(doorIndex, info);
	}

	// Token: 0x04000622 RID: 1570
	public GhostLab.EntranceDoorsState doorState;

	// Token: 0x04000623 RID: 1571
	public int singleDoorCount;

	// Token: 0x04000624 RID: 1572
	public bool[] singleDoorOpen;

	// Token: 0x04000625 RID: 1573
	[WeaverGenerated]
	[DefaultForProperty("NetData", 0, 11)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private GhostLabData _NetData;
}
