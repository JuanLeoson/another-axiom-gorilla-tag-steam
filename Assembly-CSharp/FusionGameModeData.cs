using System;
using Fusion;

// Token: 0x020004C8 RID: 1224
[NetworkBehaviourWeaved(0)]
public abstract class FusionGameModeData : NetworkBehaviour
{
	// Token: 0x1700033C RID: 828
	// (get) Token: 0x06001E1F RID: 7711
	// (set) Token: 0x06001E20 RID: 7712
	public abstract object Data { get; set; }

	// Token: 0x06001E22 RID: 7714 RVA: 0x000023F5 File Offset: 0x000005F5
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
	}

	// Token: 0x06001E23 RID: 7715 RVA: 0x000023F5 File Offset: 0x000005F5
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
	}

	// Token: 0x040026B0 RID: 9904
	protected INetworkStruct data;
}
