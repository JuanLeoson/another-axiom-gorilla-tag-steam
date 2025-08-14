using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000FAE RID: 4014
	public class MathUtil
	{
		// Token: 0x06006456 RID: 25686 RVA: 0x001FD34C File Offset: 0x001FB54C
		public static float AsinSafe(float x)
		{
			return Mathf.Asin(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x06006457 RID: 25687 RVA: 0x001FD363 File Offset: 0x001FB563
		public static float AcosSafe(float x)
		{
			return Mathf.Acos(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x06006458 RID: 25688 RVA: 0x001FD37C File Offset: 0x001FB57C
		public static float CatmullRom(float p0, float p1, float p2, float p3, float t)
		{
			float num = t * t;
			return 0.5f * (2f * p1 + (-p0 + p2) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * num + (-p0 + 3f * p1 - 3f * p2 + p3) * num * t);
		}

		// Token: 0x04006F11 RID: 28433
		public static readonly float Pi = 3.1415927f;

		// Token: 0x04006F12 RID: 28434
		public static readonly float TwoPi = 6.2831855f;

		// Token: 0x04006F13 RID: 28435
		public static readonly float HalfPi = 1.5707964f;

		// Token: 0x04006F14 RID: 28436
		public static readonly float ThirdPi = 1.0471976f;

		// Token: 0x04006F15 RID: 28437
		public static readonly float QuarterPi = 0.7853982f;

		// Token: 0x04006F16 RID: 28438
		public static readonly float FifthPi = 0.62831855f;

		// Token: 0x04006F17 RID: 28439
		public static readonly float SixthPi = 0.5235988f;

		// Token: 0x04006F18 RID: 28440
		public static readonly float Sqrt2 = Mathf.Sqrt(2f);

		// Token: 0x04006F19 RID: 28441
		public static readonly float Sqrt2Inv = 1f / Mathf.Sqrt(2f);

		// Token: 0x04006F1A RID: 28442
		public static readonly float Sqrt3 = Mathf.Sqrt(3f);

		// Token: 0x04006F1B RID: 28443
		public static readonly float Sqrt3Inv = 1f / Mathf.Sqrt(3f);

		// Token: 0x04006F1C RID: 28444
		public static readonly float Epsilon = 1E-09f;

		// Token: 0x04006F1D RID: 28445
		public static readonly float EpsilonComp = 1f - MathUtil.Epsilon;

		// Token: 0x04006F1E RID: 28446
		public static readonly float Rad2Deg = 57.295776f;

		// Token: 0x04006F1F RID: 28447
		public static readonly float Deg2Rad = 0.017453292f;
	}
}
