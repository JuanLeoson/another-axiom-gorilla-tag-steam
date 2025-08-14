using System;
using Fusion;
using GorillaGameModes;
using Photon.Pun;

// Token: 0x020004C4 RID: 1220
public sealed class CasualGameMode : GorillaGameManager
{
	// Token: 0x06001E0B RID: 7691 RVA: 0x000A0AE3 File Offset: 0x0009ECE3
	public override int MyMatIndex(NetPlayer player)
	{
		if (this.GetMyMaterial == null)
		{
			return 0;
		}
		return this.GetMyMaterial(player);
	}

	// Token: 0x06001E0C RID: 7692 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void OnSerializeRead(object newData)
	{
	}

	// Token: 0x06001E0D RID: 7693 RVA: 0x00058615 File Offset: 0x00056815
	public override object OnSerializeWrite()
	{
		return null;
	}

	// Token: 0x06001E0E RID: 7694 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06001E0F RID: 7695 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06001E10 RID: 7696 RVA: 0x00002076 File Offset: 0x00000276
	public override GameModeType GameType()
	{
		return GameModeType.Casual;
	}

	// Token: 0x06001E11 RID: 7697 RVA: 0x000A0AFB File Offset: 0x0009ECFB
	public override void AddFusionDataBehaviour(NetworkObject behaviour)
	{
		behaviour.AddBehaviour<CasualGameModeData>();
	}

	// Token: 0x06001E12 RID: 7698 RVA: 0x000A0B04 File Offset: 0x0009ED04
	public override string GameModeName()
	{
		return "CASUAL";
	}

	// Token: 0x040026AE RID: 9902
	public CasualGameMode.MyMatDelegate GetMyMaterial;

	// Token: 0x020004C5 RID: 1221
	// (Invoke) Token: 0x06001E15 RID: 7701
	public delegate int MyMatDelegate(NetPlayer player);
}
