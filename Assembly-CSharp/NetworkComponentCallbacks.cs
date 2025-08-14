using System;
using Fusion;
using Photon.Pun;

// Token: 0x020002DB RID: 731
[NetworkBehaviourWeaved(0)]
public class NetworkComponentCallbacks : NetworkComponent
{
	// Token: 0x06001135 RID: 4405 RVA: 0x000621D4 File Offset: 0x000603D4
	public override void ReadDataFusion()
	{
		this.ReadData();
	}

	// Token: 0x06001136 RID: 4406 RVA: 0x000621E1 File Offset: 0x000603E1
	public override void WriteDataFusion()
	{
		this.WriteData();
	}

	// Token: 0x06001137 RID: 4407 RVA: 0x000621EE File Offset: 0x000603EE
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.ReadPunData(stream, info);
	}

	// Token: 0x06001138 RID: 4408 RVA: 0x000621FD File Offset: 0x000603FD
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.WritePunData(stream, info);
	}

	// Token: 0x0600113A RID: 4410 RVA: 0x00002637 File Offset: 0x00000837
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x0600113B RID: 4411 RVA: 0x00002643 File Offset: 0x00000843
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x04001956 RID: 6486
	public Action ReadData;

	// Token: 0x04001957 RID: 6487
	public Action WriteData;

	// Token: 0x04001958 RID: 6488
	public Action<PhotonStream, PhotonMessageInfo> ReadPunData;

	// Token: 0x04001959 RID: 6489
	public Action<PhotonStream, PhotonMessageInfo> WritePunData;
}
