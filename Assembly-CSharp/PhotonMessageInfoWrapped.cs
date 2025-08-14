using System;
using Fusion;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x020002D7 RID: 727
public struct PhotonMessageInfoWrapped
{
	// Token: 0x170001B2 RID: 434
	// (get) Token: 0x060010F4 RID: 4340 RVA: 0x00061E3D File Offset: 0x0006003D
	public NetPlayer Sender
	{
		get
		{
			return NetworkSystem.Instance.GetPlayer(this.senderID);
		}
	}

	// Token: 0x170001B3 RID: 435
	// (get) Token: 0x060010F5 RID: 4341 RVA: 0x00061E4F File Offset: 0x0006004F
	public double SentServerTime
	{
		get
		{
			return this.sentTick / 1000.0;
		}
	}

	// Token: 0x060010F6 RID: 4342 RVA: 0x00061E63 File Offset: 0x00060063
	public PhotonMessageInfoWrapped(PhotonMessageInfo info)
	{
		Player sender = info.Sender;
		this.senderID = ((sender != null) ? sender.ActorNumber : -1);
		this.sentTick = info.SentServerTimestamp;
		this.punInfo = info;
	}

	// Token: 0x060010F7 RID: 4343 RVA: 0x00061E91 File Offset: 0x00060091
	public PhotonMessageInfoWrapped(RpcInfo info)
	{
		this.senderID = info.Source.PlayerId;
		this.sentTick = info.Tick.Raw;
		this.punInfo = default(PhotonMessageInfo);
	}

	// Token: 0x060010F8 RID: 4344 RVA: 0x00061EC2 File Offset: 0x000600C2
	public PhotonMessageInfoWrapped(int playerID, int tick)
	{
		this.senderID = playerID;
		this.sentTick = tick;
		this.punInfo = default(PhotonMessageInfo);
	}

	// Token: 0x060010F9 RID: 4345 RVA: 0x00061EDE File Offset: 0x000600DE
	public static implicit operator PhotonMessageInfoWrapped(PhotonMessageInfo info)
	{
		return new PhotonMessageInfoWrapped(info);
	}

	// Token: 0x060010FA RID: 4346 RVA: 0x00061EE6 File Offset: 0x000600E6
	public static implicit operator PhotonMessageInfoWrapped(RpcInfo info)
	{
		return new PhotonMessageInfoWrapped(info);
	}

	// Token: 0x04001948 RID: 6472
	public int senderID;

	// Token: 0x04001949 RID: 6473
	public int sentTick;

	// Token: 0x0400194A RID: 6474
	public PhotonMessageInfo punInfo;
}
