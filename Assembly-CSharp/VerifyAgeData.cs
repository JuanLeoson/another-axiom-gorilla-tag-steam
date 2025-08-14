using System;

// Token: 0x020008C4 RID: 2244
public class VerifyAgeData
{
	// Token: 0x060037FE RID: 14334 RVA: 0x00121090 File Offset: 0x0011F290
	public VerifyAgeData(VerifyAgeResponse response, int? age)
	{
		if (response == null)
		{
			return;
		}
		this.Status = response.Status;
		if (response.Session == null && response.DefaultSession == null)
		{
			return;
		}
		this.Session = new TMPSession(response.Session, response.DefaultSession, age, this.Status);
	}

	// Token: 0x040044B8 RID: 17592
	public readonly SessionStatus Status;

	// Token: 0x040044B9 RID: 17593
	public readonly TMPSession Session;
}
