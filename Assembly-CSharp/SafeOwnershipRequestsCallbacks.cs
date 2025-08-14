using System;
using UnityEngine;

// Token: 0x020006AF RID: 1711
public class SafeOwnershipRequestsCallbacks : MonoBehaviour, IRequestableOwnershipGuardCallbacks
{
	// Token: 0x06002A1E RID: 10782 RVA: 0x000E1214 File Offset: 0x000DF414
	private void Awake()
	{
		this._requestableOwnershipGuard.AddCallbackTarget(this);
	}

	// Token: 0x06002A1F RID: 10783 RVA: 0x000023F5 File Offset: 0x000005F5
	void IRequestableOwnershipGuardCallbacks.OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
	}

	// Token: 0x06002A20 RID: 10784 RVA: 0x00002076 File Offset: 0x00000276
	bool IRequestableOwnershipGuardCallbacks.OnOwnershipRequest(NetPlayer fromPlayer)
	{
		return false;
	}

	// Token: 0x06002A21 RID: 10785 RVA: 0x000023F5 File Offset: 0x000005F5
	void IRequestableOwnershipGuardCallbacks.OnMyOwnerLeft()
	{
	}

	// Token: 0x06002A22 RID: 10786 RVA: 0x00002076 File Offset: 0x00000276
	bool IRequestableOwnershipGuardCallbacks.OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		return false;
	}

	// Token: 0x06002A23 RID: 10787 RVA: 0x000023F5 File Offset: 0x000005F5
	void IRequestableOwnershipGuardCallbacks.OnMyCreatorLeft()
	{
	}

	// Token: 0x040035D7 RID: 13783
	[SerializeField]
	private RequestableOwnershipGuard _requestableOwnershipGuard;
}
