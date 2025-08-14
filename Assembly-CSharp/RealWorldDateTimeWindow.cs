using System;
using UniLabs.Time;
using UnityEngine;

// Token: 0x02000083 RID: 131
public class RealWorldDateTimeWindow : ScriptableObject
{
	// Token: 0x06000347 RID: 839 RVA: 0x00013BBE File Offset: 0x00011DBE
	public bool MatchesDate(DateTime utcDate)
	{
		return this.startTime <= utcDate && this.endTime >= utcDate;
	}

	// Token: 0x040003DB RID: 987
	[SerializeField]
	private UDateTime startTime;

	// Token: 0x040003DC RID: 988
	[SerializeField]
	private UDateTime endTime;
}
