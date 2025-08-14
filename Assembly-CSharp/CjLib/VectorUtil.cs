using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000FB6 RID: 4022
	public class VectorUtil
	{
		// Token: 0x06006496 RID: 25750 RVA: 0x001FE2A8 File Offset: 0x001FC4A8
		public static Vector3 Rotate2D(Vector3 v, float deg)
		{
			Vector3 result = v;
			float num = Mathf.Cos(MathUtil.Deg2Rad * deg);
			float num2 = Mathf.Sin(MathUtil.Deg2Rad * deg);
			result.x = num * v.x - num2 * v.y;
			result.y = num2 * v.x + num * v.y;
			return result;
		}

		// Token: 0x06006497 RID: 25751 RVA: 0x001FE302 File Offset: 0x001FC502
		public static Vector3 NormalizeSafe(Vector3 v, Vector3 fallback)
		{
			if (v.sqrMagnitude <= MathUtil.Epsilon)
			{
				return fallback;
			}
			return v.normalized;
		}

		// Token: 0x06006498 RID: 25752 RVA: 0x001FE31C File Offset: 0x001FC51C
		public static Vector3 FindOrthogonal(Vector3 v)
		{
			if (Mathf.Abs(v.x) >= MathUtil.Sqrt3Inv)
			{
				return Vector3.Normalize(new Vector3(v.y, -v.x, 0f));
			}
			return Vector3.Normalize(new Vector3(0f, v.z, -v.y));
		}

		// Token: 0x06006499 RID: 25753 RVA: 0x001FE374 File Offset: 0x001FC574
		public static void FormOrthogonalBasis(Vector3 v, out Vector3 a, out Vector3 b)
		{
			a = VectorUtil.FindOrthogonal(v);
			b = Vector3.Cross(a, v);
		}

		// Token: 0x0600649A RID: 25754 RVA: 0x001FE394 File Offset: 0x001FC594
		public static Vector3 Integrate(Vector3 x, Vector3 v, float dt)
		{
			return x + v * dt;
		}

		// Token: 0x0600649B RID: 25755 RVA: 0x001FE3A4 File Offset: 0x001FC5A4
		public static Vector3 Slerp(Vector3 a, Vector3 b, float t)
		{
			float num = Vector3.Dot(a, b);
			if (num > 0.99999f)
			{
				return Vector3.Lerp(a, b, t);
			}
			if (num < -0.99999f)
			{
				Vector3 axis = VectorUtil.FindOrthogonal(a);
				return Quaternion.AngleAxis(180f * t, axis) * a;
			}
			float num2 = MathUtil.AcosSafe(num);
			return (Mathf.Sin((1f - t) * num2) * a + Mathf.Sin(t * num2) * b) / Mathf.Sin(num2);
		}

		// Token: 0x0600649C RID: 25756 RVA: 0x001FE428 File Offset: 0x001FC628
		public static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			float d = t * t;
			return 0.5f * (2f * p1 + (-p0 + p2) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * d + (-p0 + 3f * p1 - 3f * p2 + p3) * d * t);
		}
	}
}
