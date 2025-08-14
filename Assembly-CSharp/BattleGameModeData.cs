using System;
using Fusion;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020004C3 RID: 1219
[NetworkBehaviourWeaved(31)]
public class BattleGameModeData : FusionGameModeData
{
	// Token: 0x17000338 RID: 824
	// (get) Token: 0x06001E01 RID: 7681 RVA: 0x000A07AB File Offset: 0x0009E9AB
	// (set) Token: 0x06001E02 RID: 7682 RVA: 0x000A07D5 File Offset: 0x0009E9D5
	[Networked]
	[NetworkedWeaved(0, 31)]
	private unsafe PaintbrawlData PaintbrawlData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing BattleGameModeData.PaintbrawlData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(PaintbrawlData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing BattleGameModeData.PaintbrawlData. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(PaintbrawlData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x17000339 RID: 825
	// (get) Token: 0x06001E03 RID: 7683 RVA: 0x000A0800 File Offset: 0x0009EA00
	// (set) Token: 0x06001E04 RID: 7684 RVA: 0x000A080D File Offset: 0x0009EA0D
	public override object Data
	{
		get
		{
			return this.PaintbrawlData;
		}
		set
		{
			this.PaintbrawlData = (PaintbrawlData)value;
		}
	}

	// Token: 0x06001E05 RID: 7685 RVA: 0x000A081B File Offset: 0x0009EA1B
	public override void Spawned()
	{
		this.serializer = base.GetComponent<GameModeSerializer>();
		this.battleTarget = (GorillaPaintbrawlManager)this.serializer.GameModeInstance;
	}

	// Token: 0x06001E06 RID: 7686 RVA: 0x000A0840 File Offset: 0x0009EA40
	[Rpc]
	public unsafe void RPC_ReportSlinshotHit(int taggedPlayerID, Vector3 hitLocation, int projectileCount, RpcInfo rpcInfo = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != SimulationStages.Resimulate)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 7) == 0)
				{
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void BattleGameModeData::RPC_ReportSlinshotHit(System.Int32,UnityEngine.Vector3,System.Int32,Fusion.RpcInfo)", base.Object, 7);
				}
				else
				{
					if (base.Runner.HasAnyActiveConnections())
					{
						int num = 8;
						num += 4;
						num += 12;
						num += 4;
						SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
						byte* data = SimulationMessage.GetData(ptr);
						int num2 = RpcHeader.Write(RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1), data);
						*(int*)(data + num2) = taggedPlayerID;
						num2 += 4;
						*(Vector3*)(data + num2) = hitLocation;
						num2 += 12;
						*(int*)(data + num2) = projectileCount;
						num2 += 4;
						ptr->Offset = num2 * 8;
						base.Runner.SendRpc(ptr);
					}
					if ((localAuthorityMask & 7) != 0)
					{
						rpcInfo = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
						goto IL_12;
					}
				}
			}
			return;
		}
		this.InvokeRpc = false;
		IL_12:
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(rpcInfo);
		GorillaNot.IncrementRPCCall(photonMessageInfoWrapped, "RPC_ReportSlinshotHit");
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(taggedPlayerID);
		this.battleTarget.ReportSlingshotHit(player, hitLocation, projectileCount, photonMessageInfoWrapped);
	}

	// Token: 0x06001E08 RID: 7688 RVA: 0x000A0A07 File Offset: 0x0009EC07
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.PaintbrawlData = this._PaintbrawlData;
	}

	// Token: 0x06001E09 RID: 7689 RVA: 0x000A0A1F File Offset: 0x0009EC1F
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._PaintbrawlData = this.PaintbrawlData;
	}

	// Token: 0x06001E0A RID: 7690 RVA: 0x000A0A34 File Offset: 0x0009EC34
	[NetworkRpcWeavedInvoker(1, 7, 7)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_ReportSlinshotHit@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = RpcHeader.ReadSize(data) + 3 & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int taggedPlayerID = num2;
		Vector3 vector = *(Vector3*)(data + num);
		num += 12;
		Vector3 hitLocation = vector;
		int num3 = *(int*)(data + num);
		num += 4;
		int projectileCount = num3;
		RpcInfo rpcInfo = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((BattleGameModeData)behaviour).RPC_ReportSlinshotHit(taggedPlayerID, hitLocation, projectileCount, rpcInfo);
	}

	// Token: 0x040026AB RID: 9899
	[WeaverGenerated]
	[DefaultForProperty("PaintbrawlData", 0, 31)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private PaintbrawlData _PaintbrawlData;

	// Token: 0x040026AC RID: 9900
	private GorillaPaintbrawlManager battleTarget;

	// Token: 0x040026AD RID: 9901
	private GameModeSerializer serializer;
}
