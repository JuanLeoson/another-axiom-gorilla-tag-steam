using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x020002EB RID: 747
public static class NetworkSystemRaiseEvent
{
	// Token: 0x060011E5 RID: 4581 RVA: 0x00062F0A File Offset: 0x0006110A
	public static void RaiseEvent(byte code, object data)
	{
		PhotonNetwork.RaiseEvent(code, data, RaiseEventOptions.Default, SendOptions.SendUnreliable);
	}

	// Token: 0x060011E6 RID: 4582 RVA: 0x00062F20 File Offset: 0x00061120
	public static void RaiseEvent(byte code, object data, NetEventOptions options, bool reliable)
	{
		PhotonNetwork.RaiseEvent(code, data, new RaiseEventOptions
		{
			TargetActors = options.TargetActors,
			Receivers = (ReceiverGroup)options.Reciever,
			Flags = options.Flags
		}, reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
	}

	// Token: 0x0400199C RID: 6556
	public static readonly NetEventOptions neoOthers = new NetEventOptions
	{
		Reciever = NetEventOptions.RecieverTarget.others
	};

	// Token: 0x0400199D RID: 6557
	public static readonly NetEventOptions neoMaster = new NetEventOptions
	{
		Reciever = NetEventOptions.RecieverTarget.master
	};

	// Token: 0x0400199E RID: 6558
	public static readonly NetEventOptions neoTarget = new NetEventOptions
	{
		TargetActors = new int[1]
	};

	// Token: 0x0400199F RID: 6559
	public static readonly NetEventOptions newWeb = new NetEventOptions
	{
		Flags = new WebFlags(1)
	};
}
