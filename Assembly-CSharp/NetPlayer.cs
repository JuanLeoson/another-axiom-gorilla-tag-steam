using System;
using System.Collections.Generic;
using Fusion;
using GorillaTag;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020002D8 RID: 728
[Serializable]
public abstract class NetPlayer : ObjectPoolEvents
{
	// Token: 0x170001B4 RID: 436
	// (get) Token: 0x060010FB RID: 4347
	public abstract bool IsValid { get; }

	// Token: 0x170001B5 RID: 437
	// (get) Token: 0x060010FC RID: 4348
	public abstract int ActorNumber { get; }

	// Token: 0x170001B6 RID: 438
	// (get) Token: 0x060010FD RID: 4349
	public abstract string UserId { get; }

	// Token: 0x170001B7 RID: 439
	// (get) Token: 0x060010FE RID: 4350
	public abstract bool IsMasterClient { get; }

	// Token: 0x170001B8 RID: 440
	// (get) Token: 0x060010FF RID: 4351
	public abstract bool IsLocal { get; }

	// Token: 0x170001B9 RID: 441
	// (get) Token: 0x06001100 RID: 4352
	public abstract bool IsNull { get; }

	// Token: 0x170001BA RID: 442
	// (get) Token: 0x06001101 RID: 4353
	public abstract string NickName { get; }

	// Token: 0x170001BB RID: 443
	// (get) Token: 0x06001102 RID: 4354 RVA: 0x00061EEE File Offset: 0x000600EE
	// (set) Token: 0x06001103 RID: 4355 RVA: 0x00061EF6 File Offset: 0x000600F6
	public virtual string SanitizedNickName { get; set; } = string.Empty;

	// Token: 0x170001BC RID: 444
	// (get) Token: 0x06001104 RID: 4356
	public abstract string DefaultName { get; }

	// Token: 0x170001BD RID: 445
	// (get) Token: 0x06001105 RID: 4357
	public abstract bool InRoom { get; }

	// Token: 0x170001BE RID: 446
	// (get) Token: 0x06001106 RID: 4358 RVA: 0x00061EFF File Offset: 0x000600FF
	// (set) Token: 0x06001107 RID: 4359 RVA: 0x00061F07 File Offset: 0x00060107
	public virtual float JoinedTime { get; private set; }

	// Token: 0x170001BF RID: 447
	// (get) Token: 0x06001108 RID: 4360 RVA: 0x00061F10 File Offset: 0x00060110
	// (set) Token: 0x06001109 RID: 4361 RVA: 0x00061F18 File Offset: 0x00060118
	public virtual float LeftTime { get; private set; }

	// Token: 0x0600110A RID: 4362
	public abstract bool Equals(NetPlayer myPlayer, NetPlayer other);

	// Token: 0x0600110B RID: 4363 RVA: 0x00061F21 File Offset: 0x00060121
	public virtual void OnReturned()
	{
		this.LeftTime = Time.time;
		HashSet<int> singleCallRPCStatus = this.SingleCallRPCStatus;
		if (singleCallRPCStatus != null)
		{
			singleCallRPCStatus.Clear();
		}
		this.SanitizedNickName = string.Empty;
	}

	// Token: 0x0600110C RID: 4364 RVA: 0x00061F4A File Offset: 0x0006014A
	public virtual void OnTaken()
	{
		this.JoinedTime = Time.time;
		HashSet<int> singleCallRPCStatus = this.SingleCallRPCStatus;
		if (singleCallRPCStatus == null)
		{
			return;
		}
		singleCallRPCStatus.Clear();
	}

	// Token: 0x0600110D RID: 4365 RVA: 0x00061F67 File Offset: 0x00060167
	public virtual bool CheckSingleCallRPC(NetPlayer.SingleCallRPC RPCType)
	{
		return this.SingleCallRPCStatus.Contains((int)RPCType);
	}

	// Token: 0x0600110E RID: 4366 RVA: 0x00061F75 File Offset: 0x00060175
	public virtual void ReceivedSingleCallRPC(NetPlayer.SingleCallRPC RPCType)
	{
		this.SingleCallRPCStatus.Add((int)RPCType);
	}

	// Token: 0x0600110F RID: 4367 RVA: 0x00061F84 File Offset: 0x00060184
	public Player GetPlayerRef()
	{
		return (this as PunNetPlayer).PlayerRef;
	}

	// Token: 0x06001110 RID: 4368 RVA: 0x00061F91 File Offset: 0x00060191
	public string ToStringFull()
	{
		return string.Format("#{0: 0:00} '{1}', Not sure what to do with inactive yet, Or custom props?", this.ActorNumber, this.NickName);
	}

	// Token: 0x06001111 RID: 4369 RVA: 0x00061FAE File Offset: 0x000601AE
	public static implicit operator NetPlayer(Player player)
	{
		Utils.Log("Using an implicit cast from Player to NetPlayer. Please make sure this was intended as this has potential to cause errors when switching between network backends");
		NetworkSystem instance = NetworkSystem.Instance;
		return ((instance != null) ? instance.GetPlayer(player) : null) ?? null;
	}

	// Token: 0x06001112 RID: 4370 RVA: 0x00061FD1 File Offset: 0x000601D1
	public static implicit operator NetPlayer(PlayerRef player)
	{
		Utils.Log("Using an implicit cast from PlayerRef to NetPlayer. Please make sure this was intended as this has potential to cause errors when switching between network backends");
		NetworkSystem instance = NetworkSystem.Instance;
		return ((instance != null) ? instance.GetPlayer(player) : null) ?? null;
	}

	// Token: 0x06001113 RID: 4371 RVA: 0x00061FF4 File Offset: 0x000601F4
	public static NetPlayer Get(Player player)
	{
		NetworkSystem instance = NetworkSystem.Instance;
		return ((instance != null) ? instance.GetPlayer(player) : null) ?? null;
	}

	// Token: 0x06001114 RID: 4372 RVA: 0x0006200D File Offset: 0x0006020D
	public static NetPlayer Get(PlayerRef player)
	{
		NetworkSystem instance = NetworkSystem.Instance;
		return ((instance != null) ? instance.GetPlayer(player) : null) ?? null;
	}

	// Token: 0x06001115 RID: 4373 RVA: 0x00062026 File Offset: 0x00060226
	public static NetPlayer Get(int actorNr)
	{
		NetworkSystem instance = NetworkSystem.Instance;
		return ((instance != null) ? instance.GetPlayer(actorNr) : null) ?? null;
	}

	// Token: 0x0400194E RID: 6478
	private HashSet<int> SingleCallRPCStatus = new HashSet<int>(5);

	// Token: 0x020002D9 RID: 729
	public enum SingleCallRPC
	{
		// Token: 0x04001950 RID: 6480
		CMS_RequestRoomInitialization,
		// Token: 0x04001951 RID: 6481
		CMS_RequestTriggerHistory,
		// Token: 0x04001952 RID: 6482
		CMS_SyncTriggerHistory,
		// Token: 0x04001953 RID: 6483
		CMS_SyncTriggerCounts,
		// Token: 0x04001954 RID: 6484
		RankedSendScoreToLateJoiner,
		// Token: 0x04001955 RID: 6485
		Count
	}
}
