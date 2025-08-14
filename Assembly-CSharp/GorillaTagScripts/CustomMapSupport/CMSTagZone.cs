using System;
using GorillaGameModes;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000C55 RID: 3157
	public class CMSTagZone : CMSTrigger
	{
		// Token: 0x06004E23 RID: 20003 RVA: 0x001847C7 File Offset: 0x001829C7
		public override void Trigger(double triggerTime = -1.0, bool originatedLocally = false, bool ignoreTriggerCount = false)
		{
			base.Trigger(triggerTime, originatedLocally, ignoreTriggerCount);
			if (originatedLocally)
			{
				GameMode.ReportHit();
			}
		}
	}
}
