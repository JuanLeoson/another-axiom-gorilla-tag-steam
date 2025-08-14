using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000FFC RID: 4092
	public class MathUtil
	{
		// Token: 0x06006639 RID: 26169 RVA: 0x001FD34C File Offset: 0x001FB54C
		public static float AsinSafe(float x)
		{
			return Mathf.Asin(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x0600663A RID: 26170 RVA: 0x001FD363 File Offset: 0x001FB563
		public static float AcosSafe(float x)
		{
			return Mathf.Acos(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x0600663B RID: 26171 RVA: 0x00208418 File Offset: 0x00206618
		public static float InvSafe(float x)
		{
			return 1f / Mathf.Max(MathUtil.Epsilon, x);
		}

		// Token: 0x0600663C RID: 26172 RVA: 0x0020842C File Offset: 0x0020662C
		public static float PointLineDist(Vector2 point, Vector2 linePos, Vector2 lineDir)
		{
			Vector2 vector = point - linePos;
			return (vector - Vector2.Dot(vector, lineDir) * lineDir).magnitude;
		}

		// Token: 0x0600663D RID: 26173 RVA: 0x0020845C File Offset: 0x0020665C
		public static float PointSegmentDist(Vector2 point, Vector2 segmentPosA, Vector2 segmentPosB)
		{
			Vector2 a = segmentPosB - segmentPosA;
			float num = 1f / a.magnitude;
			Vector2 rhs = a * num;
			float value = Vector2.Dot(point - segmentPosA, rhs) * num;
			return (segmentPosA + Mathf.Clamp(value, 0f, 1f) * a - point).magnitude;
		}

		// Token: 0x0600663E RID: 26174 RVA: 0x002084C4 File Offset: 0x002066C4
		public static float Seek(float current, float target, float maxDelta)
		{
			float num = target - current;
			num = Mathf.Sign(num) * Mathf.Min(maxDelta, Mathf.Abs(num));
			return current + num;
		}

		// Token: 0x0600663F RID: 26175 RVA: 0x002084EC File Offset: 0x002066EC
		public static Vector2 Seek(Vector2 current, Vector2 target, float maxDelta)
		{
			Vector2 b = target - current;
			float magnitude = b.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return target;
			}
			b = Mathf.Min(maxDelta, magnitude) * b.normalized;
			return current + b;
		}

		// Token: 0x06006640 RID: 26176 RVA: 0x0020852E File Offset: 0x0020672E
		public static float Remainder(float a, float b)
		{
			return a - a / b * b;
		}

		// Token: 0x06006641 RID: 26177 RVA: 0x0020852E File Offset: 0x0020672E
		public static int Remainder(int a, int b)
		{
			return a - a / b * b;
		}

		// Token: 0x06006642 RID: 26178 RVA: 0x00208537 File Offset: 0x00206737
		public static float Modulo(float a, float b)
		{
			return Mathf.Repeat(a, b);
		}

		// Token: 0x06006643 RID: 26179 RVA: 0x00208540 File Offset: 0x00206740
		public static int Modulo(int a, int b)
		{
			int num = a % b;
			if (num < 0)
			{
				return num + b;
			}
			return num;
		}

		// Token: 0x0400711D RID: 28957
		public static readonly float Pi = 3.1415927f;

		// Token: 0x0400711E RID: 28958
		public static readonly float TwoPi = 6.2831855f;

		// Token: 0x0400711F RID: 28959
		public static readonly float HalfPi = 1.5707964f;

		// Token: 0x04007120 RID: 28960
		public static readonly float QuaterPi = 0.7853982f;

		// Token: 0x04007121 RID: 28961
		public static readonly float SixthPi = 0.5235988f;

		// Token: 0x04007122 RID: 28962
		public static readonly float Sqrt2 = Mathf.Sqrt(2f);

		// Token: 0x04007123 RID: 28963
		public static readonly float Sqrt2Inv = 1f / Mathf.Sqrt(2f);

		// Token: 0x04007124 RID: 28964
		public static readonly float Sqrt3 = Mathf.Sqrt(3f);

		// Token: 0x04007125 RID: 28965
		public static readonly float Sqrt3Inv = 1f / Mathf.Sqrt(3f);

		// Token: 0x04007126 RID: 28966
		public static readonly float Epsilon = 1E-06f;

		// Token: 0x04007127 RID: 28967
		public static readonly float Rad2Deg = 57.295776f;

		// Token: 0x04007128 RID: 28968
		public static readonly float Deg2Rad = 0.017453292f;
	}
}
