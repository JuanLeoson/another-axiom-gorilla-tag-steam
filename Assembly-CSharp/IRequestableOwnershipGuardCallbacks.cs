using System;

// Token: 0x02000245 RID: 581
public interface IRequestableOwnershipGuardCallbacks
{
	// Token: 0x06000DA6 RID: 3494
	void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer);

	// Token: 0x06000DA7 RID: 3495
	bool OnOwnershipRequest(NetPlayer fromPlayer);

	// Token: 0x06000DA8 RID: 3496
	void OnMyOwnerLeft();

	// Token: 0x06000DA9 RID: 3497
	bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer);

	// Token: 0x06000DAA RID: 3498
	void OnMyCreatorLeft();
}
