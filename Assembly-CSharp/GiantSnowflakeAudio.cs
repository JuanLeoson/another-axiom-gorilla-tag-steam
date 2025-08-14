using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200069B RID: 1691
public class GiantSnowflakeAudio : MonoBehaviour
{
	// Token: 0x06002969 RID: 10601 RVA: 0x000DEC5C File Offset: 0x000DCE5C
	private void Start()
	{
		foreach (GiantSnowflakeAudio.SnowflakeScaleOverride snowflakeScaleOverride in this.audioOverrides)
		{
			if (base.transform.lossyScale.x < snowflakeScaleOverride.scaleMax)
			{
				base.GetComponent<GorillaSurfaceOverride>().overrideIndex = snowflakeScaleOverride.newOverrideIndex;
			}
		}
	}

	// Token: 0x04003574 RID: 13684
	public List<GiantSnowflakeAudio.SnowflakeScaleOverride> audioOverrides;

	// Token: 0x0200069C RID: 1692
	[Serializable]
	public struct SnowflakeScaleOverride
	{
		// Token: 0x04003575 RID: 13685
		public float scaleMax;

		// Token: 0x04003576 RID: 13686
		[GorillaSoundLookup]
		public int newOverrideIndex;
	}
}
