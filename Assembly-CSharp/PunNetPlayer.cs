using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000307 RID: 775
[Serializable]
public class PunNetPlayer : NetPlayer
{
	// Token: 0x17000208 RID: 520
	// (get) Token: 0x060012A6 RID: 4774 RVA: 0x00066CD7 File Offset: 0x00064ED7
	// (set) Token: 0x060012A7 RID: 4775 RVA: 0x00066CDF File Offset: 0x00064EDF
	public Player PlayerRef { get; private set; }

	// Token: 0x060012A9 RID: 4777 RVA: 0x00066CF0 File Offset: 0x00064EF0
	public void InitPlayer(Player playerRef)
	{
		this.PlayerRef = playerRef;
	}

	// Token: 0x17000209 RID: 521
	// (get) Token: 0x060012AA RID: 4778 RVA: 0x00066CF9 File Offset: 0x00064EF9
	public override bool IsValid
	{
		get
		{
			return !this.PlayerRef.IsInactive;
		}
	}

	// Token: 0x1700020A RID: 522
	// (get) Token: 0x060012AB RID: 4779 RVA: 0x00066D09 File Offset: 0x00064F09
	public override int ActorNumber
	{
		get
		{
			Player playerRef = this.PlayerRef;
			if (playerRef == null)
			{
				return -1;
			}
			return playerRef.ActorNumber;
		}
	}

	// Token: 0x1700020B RID: 523
	// (get) Token: 0x060012AC RID: 4780 RVA: 0x00066D1C File Offset: 0x00064F1C
	public override string UserId
	{
		get
		{
			return this.PlayerRef.UserId;
		}
	}

	// Token: 0x1700020C RID: 524
	// (get) Token: 0x060012AD RID: 4781 RVA: 0x00066D29 File Offset: 0x00064F29
	public override bool IsMasterClient
	{
		get
		{
			return this.PlayerRef.IsMasterClient;
		}
	}

	// Token: 0x1700020D RID: 525
	// (get) Token: 0x060012AE RID: 4782 RVA: 0x00066D36 File Offset: 0x00064F36
	public override bool IsLocal
	{
		get
		{
			return this.PlayerRef == PhotonNetwork.LocalPlayer;
		}
	}

	// Token: 0x1700020E RID: 526
	// (get) Token: 0x060012AF RID: 4783 RVA: 0x00066D45 File Offset: 0x00064F45
	public override bool IsNull
	{
		get
		{
			return this.PlayerRef == null;
		}
	}

	// Token: 0x1700020F RID: 527
	// (get) Token: 0x060012B0 RID: 4784 RVA: 0x00066D50 File Offset: 0x00064F50
	public override string NickName
	{
		get
		{
			return this.PlayerRef.NickName;
		}
	}

	// Token: 0x17000210 RID: 528
	// (get) Token: 0x060012B1 RID: 4785 RVA: 0x00066D5D File Offset: 0x00064F5D
	public override string DefaultName
	{
		get
		{
			return this.PlayerRef.DefaultName;
		}
	}

	// Token: 0x17000211 RID: 529
	// (get) Token: 0x060012B2 RID: 4786 RVA: 0x00066D6A File Offset: 0x00064F6A
	public override bool InRoom
	{
		get
		{
			Room currentRoom = PhotonNetwork.CurrentRoom;
			return currentRoom != null && currentRoom.Players.ContainsValue(this.PlayerRef);
		}
	}

	// Token: 0x060012B3 RID: 4787 RVA: 0x00066D87 File Offset: 0x00064F87
	public override bool Equals(NetPlayer myPlayer, NetPlayer other)
	{
		return myPlayer != null && other != null && ((PunNetPlayer)myPlayer).PlayerRef.Equals(((PunNetPlayer)other).PlayerRef);
	}

	// Token: 0x060012B4 RID: 4788 RVA: 0x00066DAC File Offset: 0x00064FAC
	public override void OnReturned()
	{
		base.OnReturned();
	}

	// Token: 0x060012B5 RID: 4789 RVA: 0x00066DB4 File Offset: 0x00064FB4
	public override void OnTaken()
	{
		base.OnTaken();
		this.PlayerRef = null;
	}
}
