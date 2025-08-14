using System;

// Token: 0x02000048 RID: 72
public class CrittersAttachPointSettings : CrittersActorSettings
{
	// Token: 0x0600015E RID: 350 RVA: 0x00009269 File Offset: 0x00007469
	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersAttachPoint crittersAttachPoint = (CrittersAttachPoint)this.parentActor;
		crittersAttachPoint.anchorLocation = this.anchoredLocation;
		crittersAttachPoint.rb.isKinematic = true;
		crittersAttachPoint.isLeft = this.isLeft;
	}

	// Token: 0x0400018C RID: 396
	public bool isLeft;

	// Token: 0x0400018D RID: 397
	public CrittersAttachPoint.AnchoredLocationTypes anchoredLocation;
}
