using System;
using Fusion;
using Photon.Pun;

// Token: 0x020006A1 RID: 1697
[NetworkBehaviourWeaved(0)]
internal abstract class GorillaSerializerMasterOnly : GorillaWrappedSerializer
{
	// Token: 0x0600299E RID: 10654 RVA: 0x000DFD8B File Offset: 0x000DDF8B
	protected override bool ValidOnSerialize(PhotonStream stream, in PhotonMessageInfo info)
	{
		return info.Sender == PhotonNetwork.MasterClient;
	}

	// Token: 0x060029A0 RID: 10656 RVA: 0x000DFDA5 File Offset: 0x000DDFA5
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x060029A1 RID: 10657 RVA: 0x000DFDB1 File Offset: 0x000DDFB1
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}
}
