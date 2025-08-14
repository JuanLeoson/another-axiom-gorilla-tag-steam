using System;

// Token: 0x020008C3 RID: 2243
public class UpgradeSessionData
{
	// Token: 0x060037FD RID: 14333 RVA: 0x00121050 File Offset: 0x0011F250
	public UpgradeSessionData(UpgradeSessionResponse response)
	{
		this.status = response.status;
		this.session = new TMPSession(response.session, null, null, this.status);
	}

	// Token: 0x040044B6 RID: 17590
	public readonly SessionStatus status;

	// Token: 0x040044B7 RID: 17591
	public readonly TMPSession session;
}
