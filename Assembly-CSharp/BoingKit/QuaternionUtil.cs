using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000FFD RID: 4093
	public class QuaternionUtil
	{
		// Token: 0x06006646 RID: 26182 RVA: 0x001FDE50 File Offset: 0x001FC050
		public static float Magnitude(Quaternion q)
		{
			return Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
		}

		// Token: 0x06006647 RID: 26183 RVA: 0x001FDE8E File Offset: 0x001FC08E
		public static float MagnitudeSqr(Quaternion q)
		{
			return q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w;
		}

		// Token: 0x06006648 RID: 26184 RVA: 0x00208604 File Offset: 0x00206804
		public static Quaternion Normalize(Quaternion q)
		{
			float num = 1f / QuaternionUtil.Magnitude(q);
			return new Quaternion(num * q.x, num * q.y, num * q.z, num * q.w);
		}

		// Token: 0x06006649 RID: 26185 RVA: 0x00208644 File Offset: 0x00206844
		public static Quaternion AxisAngle(Vector3 axis, float angle)
		{
			float f = 0.5f * angle;
			float num = Mathf.Sin(f);
			float w = Mathf.Cos(f);
			return new Quaternion(num * axis.x, num * axis.y, num * axis.z, w);
		}

		// Token: 0x0600664A RID: 26186 RVA: 0x00208684 File Offset: 0x00206884
		public static Vector3 GetAxis(Quaternion q)
		{
			Vector3 a = new Vector3(q.x, q.y, q.z);
			float magnitude = a.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return Vector3.left;
			}
			return a / magnitude;
		}

		// Token: 0x0600664B RID: 26187 RVA: 0x001FDFEB File Offset: 0x001FC1EB
		public static float GetAngle(Quaternion q)
		{
			return 2f * Mathf.Acos(Mathf.Clamp(q.w, -1f, 1f));
		}

		// Token: 0x0600664C RID: 26188 RVA: 0x002086C8 File Offset: 0x002068C8
		public static Quaternion FromAngularVector(Vector3 v)
		{
			float magnitude = v.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return Quaternion.identity;
			}
			v /= magnitude;
			float f = 0.5f * magnitude;
			float num = Mathf.Sin(f);
			float w = Mathf.Cos(f);
			return new Quaternion(num * v.x, num * v.y, num * v.z, w);
		}

		// Token: 0x0600664D RID: 26189 RVA: 0x00208728 File Offset: 0x00206928
		public static Vector3 ToAngularVector(Quaternion q)
		{
			Vector3 axis = QuaternionUtil.GetAxis(q);
			return QuaternionUtil.GetAngle(q) * axis;
		}

		// Token: 0x0600664E RID: 26190 RVA: 0x00208748 File Offset: 0x00206948
		public static Quaternion Pow(Quaternion q, float exp)
		{
			Vector3 axis = QuaternionUtil.GetAxis(q);
			float angle = QuaternionUtil.GetAngle(q) * exp;
			return QuaternionUtil.AxisAngle(axis, angle);
		}

		// Token: 0x0600664F RID: 26191 RVA: 0x0020876A File Offset: 0x0020696A
		public static Quaternion Integrate(Quaternion q, Quaternion v, float dt)
		{
			return QuaternionUtil.Pow(v, dt) * q;
		}

		// Token: 0x06006650 RID: 26192 RVA: 0x0020877C File Offset: 0x0020697C
		public static Quaternion Integrate(Quaternion q, Vector3 omega, float dt)
		{
			omega *= 0.5f;
			Quaternion quaternion = new Quaternion(omega.x, omega.y, omega.z, 0f) * q;
			return QuaternionUtil.Normalize(new Quaternion(q.x + quaternion.x * dt, q.y + quaternion.y * dt, q.z + quaternion.z * dt, q.w + quaternion.w * dt));
		}

		// Token: 0x06006651 RID: 26193 RVA: 0x001FE079 File Offset: 0x001FC279
		public static Vector4 ToVector4(Quaternion q)
		{
			return new Vector4(q.x, q.y, q.z, q.w);
		}

		// Token: 0x06006652 RID: 26194 RVA: 0x00208800 File Offset: 0x00206A00
		public static Quaternion FromVector4(Vector4 v, bool normalize = true)
		{
			if (normalize)
			{
				float sqrMagnitude = v.sqrMagnitude;
				if (sqrMagnitude < MathUtil.Epsilon)
				{
					return Quaternion.identity;
				}
				v /= Mathf.Sqrt(sqrMagnitude);
			}
			return new Quaternion(v.x, v.y, v.z, v.w);
		}

		// Token: 0x06006653 RID: 26195 RVA: 0x00208854 File Offset: 0x00206A54
		public static void DecomposeSwingTwist(Quaternion q, Vector3 twistAxis, out Quaternion swing, out Quaternion twist)
		{
			Vector3 vector = new Vector3(q.x, q.y, q.z);
			if (vector.sqrMagnitude < MathUtil.Epsilon)
			{
				Vector3 vector2 = q * twistAxis;
				Vector3 axis = Vector3.Cross(twistAxis, vector2);
				if (axis.sqrMagnitude > MathUtil.Epsilon)
				{
					float angle = Vector3.Angle(twistAxis, vector2);
					swing = Quaternion.AngleAxis(angle, axis);
				}
				else
				{
					swing = Quaternion.identity;
				}
				twist = Quaternion.AngleAxis(180f, twistAxis);
				return;
			}
			Vector3 vector3 = Vector3.Project(vector, twistAxis);
			twist = new Quaternion(vector3.x, vector3.y, vector3.z, q.w);
			twist = QuaternionUtil.Normalize(twist);
			swing = q * Quaternion.Inverse(twist);
		}

		// Token: 0x06006654 RID: 26196 RVA: 0x00208930 File Offset: 0x00206B30
		public static Quaternion Sterp(Quaternion a, Quaternion b, Vector3 twistAxis, float t, QuaternionUtil.SterpMode mode = QuaternionUtil.SterpMode.Slerp)
		{
			Quaternion quaternion;
			Quaternion quaternion2;
			return QuaternionUtil.Sterp(a, b, twistAxis, t, out quaternion, out quaternion2, mode);
		}

		// Token: 0x06006655 RID: 26197 RVA: 0x0020894C File Offset: 0x00206B4C
		public static Quaternion Sterp(Quaternion a, Quaternion b, Vector3 twistAxis, float t, out Quaternion swing, out Quaternion twist, QuaternionUtil.SterpMode mode = QuaternionUtil.SterpMode.Slerp)
		{
			return QuaternionUtil.Sterp(a, b, twistAxis, t, t, out swing, out twist, mode);
		}

		// Token: 0x06006656 RID: 26198 RVA: 0x00208960 File Offset: 0x00206B60
		public static Quaternion Sterp(Quaternion a, Quaternion b, Vector3 twistAxis, float tSwing, float tTwist, QuaternionUtil.SterpMode mode = QuaternionUtil.SterpMode.Slerp)
		{
			Quaternion quaternion;
			Quaternion quaternion2;
			return QuaternionUtil.Sterp(a, b, twistAxis, tSwing, tTwist, out quaternion, out quaternion2, mode);
		}

		// Token: 0x06006657 RID: 26199 RVA: 0x00208980 File Offset: 0x00206B80
		public static Quaternion Sterp(Quaternion a, Quaternion b, Vector3 twistAxis, float tSwing, float tTwist, out Quaternion swing, out Quaternion twist, QuaternionUtil.SterpMode mode)
		{
			Quaternion b2;
			Quaternion b3;
			QuaternionUtil.DecomposeSwingTwist(b * Quaternion.Inverse(a), twistAxis, out b2, out b3);
			if (mode == QuaternionUtil.SterpMode.Nlerp || mode != QuaternionUtil.SterpMode.Slerp)
			{
				swing = Quaternion.Lerp(Quaternion.identity, b2, tSwing);
				twist = Quaternion.Lerp(Quaternion.identity, b3, tTwist);
			}
			else
			{
				swing = Quaternion.Slerp(Quaternion.identity, b2, tSwing);
				twist = Quaternion.Slerp(Quaternion.identity, b3, tTwist);
			}
			return twist * swing;
		}

		// Token: 0x02000FFE RID: 4094
		public enum SterpMode
		{
			// Token: 0x0400712A RID: 28970
			Nlerp,
			// Token: 0x0400712B RID: 28971
			Slerp
		}
	}
}
