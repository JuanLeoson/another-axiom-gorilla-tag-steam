using System;
using UnityEngine;

// Token: 0x02000AE3 RID: 2787
public static class Bezier
{
	// Token: 0x06004320 RID: 17184 RVA: 0x00151548 File Offset: 0x0014F748
	public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
	{
		t = Mathf.Clamp01(t);
		float num = 1f - t;
		return num * num * p0 + 2f * num * t * p1 + t * t * p2;
	}

	// Token: 0x06004321 RID: 17185 RVA: 0x00151590 File Offset: 0x0014F790
	public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t)
	{
		return 2f * (1f - t) * (p1 - p0) + 2f * t * (p2 - p1);
	}

	// Token: 0x06004322 RID: 17186 RVA: 0x001515C4 File Offset: 0x0014F7C4
	public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		t = Mathf.Clamp01(t);
		float num = 1f - t;
		return num * num * num * p0 + 3f * num * num * t * p1 + 3f * num * t * t * p2 + t * t * t * p3;
	}

	// Token: 0x06004323 RID: 17187 RVA: 0x00151630 File Offset: 0x0014F830
	public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		t = Mathf.Clamp01(t);
		float num = 1f - t;
		return 3f * num * num * (p1 - p0) + 6f * num * t * (p2 - p1) + 3f * t * t * (p3 - p2);
	}
}
