using System;
using Fusion;
using GorillaExtensions;
using GorillaGameModes;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200069D RID: 1693
[NetworkBehaviourWeaved(1)]
internal class GameModeSerializer : GorillaSerializerMasterOnly, IStateAuthorityChanged
{
	// Token: 0x170003C3 RID: 963
	// (get) Token: 0x0600296B RID: 10603 RVA: 0x000DECD4 File Offset: 0x000DCED4
	// (set) Token: 0x0600296C RID: 10604 RVA: 0x000DECFA File Offset: 0x000DCEFA
	[Networked]
	[NetworkedWeaved(0, 1)]
	private unsafe int gameModeKeyInt
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameModeSerializer.gameModeKeyInt. Networked properties can only be accessed when Spawned() has been called.");
			}
			return this.Ptr[0];
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameModeSerializer.gameModeKeyInt. Networked properties can only be accessed when Spawned() has been called.");
			}
			this.Ptr[0] = value;
		}
	}

	// Token: 0x170003C4 RID: 964
	// (get) Token: 0x0600296D RID: 10605 RVA: 0x000DED21 File Offset: 0x000DCF21
	public GorillaGameManager GameModeInstance
	{
		get
		{
			return this.gameModeInstance;
		}
	}

	// Token: 0x0600296E RID: 10606 RVA: 0x000DED2C File Offset: 0x000DCF2C
	protected override bool OnSpawnSetupCheck(PhotonMessageInfoWrapped wrappedInfo, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetObject = null;
		outTargetType = null;
		NetPlayer player = NetworkSystem.Instance.GetPlayer(wrappedInfo.senderID);
		if (player != null)
		{
			GorillaNot.IncrementRPCCall(wrappedInfo, "OnSpawnSetupCheck");
		}
		GameModeSerializer activeNetworkHandler = GorillaGameModes.GameMode.ActiveNetworkHandler;
		if (player != null && player.InRoom)
		{
			if (!player.IsMasterClient)
			{
				GTDev.LogError<string>("SPAWN FAIL NOT MASTER :" + player.UserId + player.NickName, null);
				GorillaNot.instance.SendReport("trying to inappropriately create game managers", player.UserId, player.NickName);
				return false;
			}
			if (!this.netView.IsRoomView)
			{
				GTDev.LogError<string>("SPAWN FAIL ROOM VIEW" + player.UserId + player.NickName, null);
				GorillaNot.instance.SendReport("creating game manager as player object", player.UserId, player.NickName);
				return false;
			}
			if (activeNetworkHandler.IsNotNull() && activeNetworkHandler != this)
			{
				GTDev.LogError<string>("DUPLICATE CHECK" + player.UserId + player.NickName, null);
				GorillaNot.instance.SendReport("trying to create multiple game managers", player.UserId, player.NickName);
				return false;
			}
		}
		else if ((activeNetworkHandler.IsNotNull() && activeNetworkHandler != this) || !this.netView.IsRoomView)
		{
			GTDev.LogError<string>("ACTIVE HANDLER CHECK FAIL" + ((player != null) ? player.UserId : null) + ((player != null) ? player.NickName : null), null);
			GTDev.LogError<string>("existing game manager! destroying newly created manager", null);
			return false;
		}
		object[] instantiationData = wrappedInfo.punInfo.photonView.InstantiationData;
		if (instantiationData != null && instantiationData.Length >= 1)
		{
			object obj = instantiationData[0];
			if (obj is int)
			{
				int num = (int)obj;
				this.gameModeKey = (GameModeType)num;
				this.gameModeInstance = GorillaGameModes.GameMode.GetGameModeInstance(this.gameModeKey);
				if (this.gameModeInstance.IsNull() || !this.gameModeInstance.ValidGameMode())
				{
					return false;
				}
				this.serializeTarget = this.gameModeInstance;
				base.transform.parent = VRRigCache.Instance.NetworkParent;
				return true;
			}
		}
		GTDev.LogError<string>("missing instantiation data", null);
		return false;
	}

	// Token: 0x0600296F RID: 10607 RVA: 0x000DEF3A File Offset: 0x000DD13A
	internal void Init(int gameModeType)
	{
		Debug.Log("<color=red>Init called</color>");
		this.gameModeKeyInt = gameModeType;
	}

	// Token: 0x06002970 RID: 10608 RVA: 0x000DEF4D File Offset: 0x000DD14D
	protected override void OnSuccesfullySpawned(PhotonMessageInfoWrapped info)
	{
		this.netView.GetView.AddCallbackTarget(this);
		GorillaGameModes.GameMode.SetupGameModeRemote(this);
	}

	// Token: 0x06002971 RID: 10609 RVA: 0x000DEF66 File Offset: 0x000DD166
	protected override void OnBeforeDespawn()
	{
		GorillaGameModes.GameMode.RemoveNetworkLink(this);
	}

	// Token: 0x06002972 RID: 10610 RVA: 0x000023F5 File Offset: 0x000005F5
	protected override void OnFailedSpawn()
	{
	}

	// Token: 0x06002973 RID: 10611 RVA: 0x000DEF6E File Offset: 0x000DD16E
	[PunRPC]
	internal void RPC_ReportTag(int taggedPlayer, PhotonMessageInfo info)
	{
		this.ReportTag(NetworkSystem.Instance.GetPlayer(taggedPlayer), new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x06002974 RID: 10612 RVA: 0x000DEF87 File Offset: 0x000DD187
	[PunRPC]
	internal void RPC_ReportHit(PhotonMessageInfo info)
	{
		this.ReportHit(new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x06002975 RID: 10613 RVA: 0x000DEF98 File Offset: 0x000DD198
	[Rpc(RpcSources.All, RpcTargets.All)]
	internal unsafe void RPC_ReportTag(int taggedPlayer, RpcInfo info = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != SimulationStages.Resimulate)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 7) == 0)
				{
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameModeSerializer::RPC_ReportTag(System.Int32,Fusion.RpcInfo)", base.Object, 7);
				}
				else
				{
					if (base.Runner.HasAnyActiveConnections())
					{
						int num = 8;
						num += 4;
						SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
						byte* data = SimulationMessage.GetData(ptr);
						int num2 = RpcHeader.Write(RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1), data);
						*(int*)(data + num2) = taggedPlayer;
						num2 += 4;
						ptr->Offset = num2 * 8;
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
		this.ReportTag(NetworkSystem.Instance.GetPlayer(taggedPlayer), new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x06002976 RID: 10614 RVA: 0x000DF0E4 File Offset: 0x000DD2E4
	[Rpc(RpcSources.All, RpcTargets.All)]
	internal unsafe void RPC_ReportHit(RpcInfo info = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != SimulationStages.Resimulate)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 7) == 0)
				{
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameModeSerializer::RPC_ReportHit(Fusion.RpcInfo)", base.Object, 7);
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
		this.ReportHit(new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x06002977 RID: 10615 RVA: 0x000DF204 File Offset: 0x000DD404
	private void ReportTag(NetPlayer taggedPlayer, PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "ReportTag");
		NetPlayer sender = info.Sender;
		this.gameModeInstance.ReportTag(taggedPlayer, sender);
	}

	// Token: 0x06002978 RID: 10616 RVA: 0x000DF234 File Offset: 0x000DD434
	private void ReportHit(PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "ReportContactWithLavaRPC");
		NetPlayer sender = info.Sender;
		bool flag = ZoneManagement.instance.IsZoneActive(GTZone.customMaps);
		bool flag2 = false;
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			InfectionLavaController instance = InfectionLavaController.Instance;
			flag2 = (instance != null && instance.LavaCurrentlyActivated && (instance.SurfaceCenter - rigContainer.Rig.syncPos).sqrMagnitude < 2500f && instance.LavaPlane.GetDistanceToPoint(rigContainer.Rig.syncPos) < 5f);
		}
		if (flag || flag2)
		{
			this.GameModeInstance.HitPlayer(info.Sender);
		}
	}

	// Token: 0x06002979 RID: 10617 RVA: 0x000DF2EE File Offset: 0x000DD4EE
	[PunRPC]
	internal void RPC_BroadcastRoundComplete(PhotonMessageInfo info)
	{
		this.BroadcastRoundComplete(info);
	}

	// Token: 0x0600297A RID: 10618 RVA: 0x000DF2FC File Offset: 0x000DD4FC
	private void BroadcastRoundComplete(PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "BroadcastRoundComplete");
		if (info.Sender.IsMasterClient)
		{
			this.gameModeInstance.HandleRoundComplete();
		}
	}

	// Token: 0x0600297B RID: 10619 RVA: 0x000DF322 File Offset: 0x000DD522
	[PunRPC]
	internal void RPC_BroadcastTag(int taggedPlayer, int taggingPlayer, PhotonMessageInfo info)
	{
		this.BroadcastTag(NetworkSystem.Instance.GetPlayer(taggedPlayer), NetworkSystem.Instance.GetPlayer(taggingPlayer), info);
	}

	// Token: 0x0600297C RID: 10620 RVA: 0x000DF344 File Offset: 0x000DD544
	private void BroadcastTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "BroadcastTag");
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		if (taggedPlayer == null || taggingPlayer == null)
		{
			return;
		}
		if (!this.broadcastTagCallLimit.CheckCallTime(Time.time))
		{
			return;
		}
		this.gameModeInstance.HandleTagBroadcast(taggedPlayer, taggingPlayer);
	}

	// Token: 0x0600297D RID: 10621 RVA: 0x000DF391 File Offset: 0x000DD591
	protected override void FusionDataRPC(string method, NetPlayer targetPlayer, params object[] parameters)
	{
		Debug.Log(this.gameModeData.GetType().Name);
	}

	// Token: 0x0600297E RID: 10622 RVA: 0x000DF3A8 File Offset: 0x000DD5A8
	protected override void FusionDataRPC(string method, RpcTarget target, params object[] parameters)
	{
		base.FusionDataRPC(method, target, parameters);
	}

	// Token: 0x0600297F RID: 10623 RVA: 0x000DF3B3 File Offset: 0x000DD5B3
	void IStateAuthorityChanged.StateAuthorityChanged()
	{
		GameModeSerializer.FusionGameModeOwnerChanged(NetworkSystem.Instance.GetPlayer(base.Object.StateAuthority));
	}

	// Token: 0x06002981 RID: 10625 RVA: 0x000DF3F3 File Offset: 0x000DD5F3
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.gameModeKeyInt = this._gameModeKeyInt;
	}

	// Token: 0x06002982 RID: 10626 RVA: 0x000DF40B File Offset: 0x000DD60B
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._gameModeKeyInt = this.gameModeKeyInt;
	}

	// Token: 0x06002983 RID: 10627 RVA: 0x000DF420 File Offset: 0x000DD620
	[NetworkRpcWeavedInvoker(1, 7, 7)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_ReportTag@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = RpcHeader.ReadSize(data) + 3 & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int taggedPlayer = num2;
		RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((GameModeSerializer)behaviour).RPC_ReportTag(taggedPlayer, info);
	}

	// Token: 0x06002984 RID: 10628 RVA: 0x000DF490 File Offset: 0x000DD690
	[NetworkRpcWeavedInvoker(2, 7, 7)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_ReportHit@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = RpcHeader.ReadSize(data) + 3 & -4;
		RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((GameModeSerializer)behaviour).RPC_ReportHit(info);
	}

	// Token: 0x04003577 RID: 13687
	[WeaverGenerated]
	[DefaultForProperty("gameModeKeyInt", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private int _gameModeKeyInt;

	// Token: 0x04003578 RID: 13688
	private GameModeType gameModeKey;

	// Token: 0x04003579 RID: 13689
	private GorillaGameManager gameModeInstance;

	// Token: 0x0400357A RID: 13690
	private FusionGameModeData gameModeData;

	// Token: 0x0400357B RID: 13691
	private Type currentGameDataType;

	// Token: 0x0400357C RID: 13692
	private CallLimiter broadcastTagCallLimit = new CallLimiter(12, 5f, 0.5f);

	// Token: 0x0400357D RID: 13693
	public static Action<NetPlayer> FusionGameModeOwnerChanged;
}
