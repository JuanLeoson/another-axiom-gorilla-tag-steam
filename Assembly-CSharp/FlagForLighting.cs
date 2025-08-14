using System;
using UnityEngine;

// Token: 0x02000AB7 RID: 2743
public class FlagForLighting : MonoBehaviour
{
	// Token: 0x04004D88 RID: 19848
	public FlagForLighting.TimeOfDay myTimeOfDay;

	// Token: 0x02000AB8 RID: 2744
	public enum TimeOfDay
	{
		// Token: 0x04004D8A RID: 19850
		Sunrise,
		// Token: 0x04004D8B RID: 19851
		TenAM,
		// Token: 0x04004D8C RID: 19852
		Noon,
		// Token: 0x04004D8D RID: 19853
		ThreePM,
		// Token: 0x04004D8E RID: 19854
		Sunset,
		// Token: 0x04004D8F RID: 19855
		Night,
		// Token: 0x04004D90 RID: 19856
		RainingDay,
		// Token: 0x04004D91 RID: 19857
		RainingNight,
		// Token: 0x04004D92 RID: 19858
		None
	}
}
