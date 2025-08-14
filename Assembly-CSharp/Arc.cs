using System;
using System.Diagnostics;
using UnityEngine;

// Token: 0x0200085F RID: 2143
[Serializable]
public struct Arc
{
	// Token: 0x060035EE RID: 13806 RVA: 0x0011B8BB File Offset: 0x00119ABB
	public Vector3[] GetArcPoints(int count = 12)
	{
		return Arc.ComputeArcPoints(this.start, this.end, new Vector3?(this.control), count);
	}

	// Token: 0x060035EF RID: 13807 RVA: 0x000023F5 File Offset: 0x000005F5
	[Conditional("UNITY_EDITOR")]
	public void DrawGizmo()
	{
	}

	// Token: 0x060035F0 RID: 13808 RVA: 0x0011B8DC File Offset: 0x00119ADC
	public static Arc From(Vector3 start, Vector3 end)
	{
		Vector3 vector = Arc.DeriveArcControlPoint(start, end, null, null);
		return new Arc
		{
			start = start,
			end = end,
			control = vector
		};
	}

	// Token: 0x060035F1 RID: 13809 RVA: 0x0011B924 File Offset: 0x00119B24
	public static Vector3[] ComputeArcPoints(Vector3 a, Vector3 b, Vector3? c = null, int count = 12)
	{
		Vector3[] array = new Vector3[count];
		float num = 1f / (float)count;
		Vector3 value = c.GetValueOrDefault();
		if (c == null)
		{
			value = Arc.DeriveArcControlPoint(a, b, null, null);
			c = new Vector3?(value);
		}
		for (int i = 0; i < count; i++)
		{
			float t;
			if (i == 0)
			{
				t = 0f;
			}
			else if (i == count - 1)
			{
				t = 1f;
			}
			else
			{
				t = num * (float)i;
			}
			array[i] = Arc.BezierLerp(a, b, c.Value, t);
		}
		return array;
	}

	// Token: 0x060035F2 RID: 13810 RVA: 0x0011B9C4 File Offset: 0x00119BC4
	public static Vector3 BezierLerp(Vector3 a, Vector3 b, Vector3 c, float t)
	{
		Vector3 a2 = Vector3.Lerp(a, c, t);
		Vector3 b2 = Vector3.Lerp(c, b, t);
		return Vector3.Lerp(a2, b2, t);
	}

	// Token: 0x060035F3 RID: 13811 RVA: 0x0011B9EC File Offset: 0x00119BEC
	public static Vector3 DeriveArcControlPoint(Vector3 a, Vector3 b, Vector3? dir = null, float? height = null)
	{
		Vector3 b2 = (b - a) * 0.5f;
		Vector3 normalized = b2.normalized;
		float value = height.GetValueOrDefault();
		if (height == null)
		{
			value = b2.magnitude;
			height = new float?(value);
		}
		if (dir == null)
		{
			Vector3 rhs = Vector3.Cross(normalized, Vector3.up);
			dir = new Vector3?(Vector3.Cross(normalized, rhs));
		}
		Vector3 b3 = dir.Value * -height.Value;
		return a + b2 + b3;
	}

	// Token: 0x040042DC RID: 17116
	public Vector3 start;

	// Token: 0x040042DD RID: 17117
	public Vector3 end;

	// Token: 0x040042DE RID: 17118
	public Vector3 control;
}
