using System;

// Token: 0x02000046 RID: 70
public class CrittersAttachPoint : CrittersActor
{
	// Token: 0x0600015C RID: 348 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void ProcessRemote()
	{
	}

	// Token: 0x04000185 RID: 389
	public bool fixedOrientation = true;

	// Token: 0x04000186 RID: 390
	public CrittersAttachPoint.AnchoredLocationTypes anchorLocation;

	// Token: 0x04000187 RID: 391
	public bool isLeft;

	// Token: 0x02000047 RID: 71
	public enum AnchoredLocationTypes
	{
		// Token: 0x04000189 RID: 393
		Arm,
		// Token: 0x0400018A RID: 394
		Chest,
		// Token: 0x0400018B RID: 395
		Back
	}
}
