using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000E5C RID: 3676
	public static class GTColor
	{
		// Token: 0x06005C31 RID: 23601 RVA: 0x001D03CC File Offset: 0x001CE5CC
		public static Color RandomHSV(GTColor.HSVRanges ranges)
		{
			return Color.HSVToRGB(Random.Range(ranges.h.x, ranges.h.y), Random.Range(ranges.s.x, ranges.s.y), Random.Range(ranges.v.x, ranges.v.y));
		}

		// Token: 0x02000E5D RID: 3677
		[Serializable]
		public struct HSVRanges
		{
			// Token: 0x06005C32 RID: 23602 RVA: 0x001D042F File Offset: 0x001CE62F
			public HSVRanges(float hMin = 0f, float hMax = 1f, float sMin = 0f, float sMax = 1f, float vMin = 0f, float vMax = 1f)
			{
				this.h = new Vector2(hMin, hMax);
				this.s = new Vector2(sMin, sMax);
				this.v = new Vector2(vMin, vMax);
			}

			// Token: 0x040065DA RID: 26074
			public Vector2 h;

			// Token: 0x040065DB RID: 26075
			public Vector2 s;

			// Token: 0x040065DC RID: 26076
			public Vector2 v;
		}
	}
}
