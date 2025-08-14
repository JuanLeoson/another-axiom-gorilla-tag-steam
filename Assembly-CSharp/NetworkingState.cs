using System;

// Token: 0x02000240 RID: 576
public enum NetworkingState
{
	// Token: 0x0400155D RID: 5469
	IsOwner,
	// Token: 0x0400155E RID: 5470
	IsBlindClient,
	// Token: 0x0400155F RID: 5471
	IsClient,
	// Token: 0x04001560 RID: 5472
	ForcefullyTakingOver,
	// Token: 0x04001561 RID: 5473
	RequestingOwnership,
	// Token: 0x04001562 RID: 5474
	RequestingOwnershipWaitingForSight,
	// Token: 0x04001563 RID: 5475
	ForcefullyTakingOverWaitingForSight
}
