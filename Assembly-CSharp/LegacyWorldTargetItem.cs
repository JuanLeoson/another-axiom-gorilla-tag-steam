using System;
using Photon.Realtime;

// Token: 0x020003DA RID: 986
public class LegacyWorldTargetItem
{
	// Token: 0x0600171B RID: 5915 RVA: 0x0007D2E4 File Offset: 0x0007B4E4
	public bool IsValid()
	{
		return this.itemIdx != -1 && this.owner != null;
	}

	// Token: 0x0600171C RID: 5916 RVA: 0x0007D2FA File Offset: 0x0007B4FA
	public void Invalidate()
	{
		this.itemIdx = -1;
		this.owner = null;
	}

	// Token: 0x04001EEE RID: 7918
	public Player owner;

	// Token: 0x04001EEF RID: 7919
	public int itemIdx;
}
