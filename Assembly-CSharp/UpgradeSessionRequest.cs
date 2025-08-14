using System;
using System.Collections.Generic;
using KID.Model;

// Token: 0x020008BD RID: 2237
[Serializable]
public class UpgradeSessionRequest : KIDRequestData
{
	// Token: 0x0400449F RID: 17567
	public List<RequestedPermission> Permissions;
}
