using System;
using System.Runtime.CompilerServices;
using KID.Model;

// Token: 0x020008B9 RID: 2233
[Serializable]
public class GetPlayerDataResponse
{
	// Token: 0x0400448E RID: 17550
	public SessionStatus? Status;

	// Token: 0x0400448F RID: 17551
	public Session Session;

	// Token: 0x04004490 RID: 17552
	public int? Age;

	// Token: 0x04004491 RID: 17553
	public AgeStatusType? AgeStatus;

	// Token: 0x04004492 RID: 17554
	public KIDDefaultSession DefaultSession;

	// Token: 0x04004493 RID: 17555
	[Nullable(new byte[]
	{
		2,
		0
	})]
	public string[] Permissions;

	// Token: 0x04004494 RID: 17556
	public bool HasConfirmedSetup;
}
