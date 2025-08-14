using System;
using UnityEngine;

// Token: 0x02000A9F RID: 2719
public class BezierUtils
{
	// Token: 0x060041D2 RID: 16850 RVA: 0x0014B76C File Offset: 0x0014996C
	public static Vector3 BezierSolve(float t, Vector3 startPos, Vector3 ctrl1, Vector3 ctrl2, Vector3 endPos)
	{
		float num = 1f - t;
		float d = num * num * num;
		float d2 = 3f * num * num * t;
		float d3 = 3f * num * t * t;
		float d4 = t * t * t;
		return startPos * d + ctrl1 * d2 + ctrl2 * d3 + endPos * d4;
	}
}
