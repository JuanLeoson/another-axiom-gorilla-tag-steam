using System;
using UnityEngine;

// Token: 0x02000AE5 RID: 2789
public class BezierCurve : MonoBehaviour
{
	// Token: 0x06004324 RID: 17188 RVA: 0x0015169C File Offset: 0x0014F89C
	public Vector3 GetPoint(float t)
	{
		return base.transform.TransformPoint(Bezier.GetPoint(this.points[0], this.points[1], this.points[2], this.points[3], t));
	}

	// Token: 0x06004325 RID: 17189 RVA: 0x001516EC File Offset: 0x0014F8EC
	public Vector3 GetVelocity(float t)
	{
		return base.transform.TransformPoint(Bezier.GetFirstDerivative(this.points[0], this.points[1], this.points[2], this.points[3], t)) - base.transform.position;
	}

	// Token: 0x06004326 RID: 17190 RVA: 0x0015174C File Offset: 0x0014F94C
	public Vector3 GetDirection(float t)
	{
		return this.GetVelocity(t).normalized;
	}

	// Token: 0x06004327 RID: 17191 RVA: 0x00151768 File Offset: 0x0014F968
	public void Reset()
	{
		this.points = new Vector3[]
		{
			new Vector3(1f, 0f, 0f),
			new Vector3(2f, 0f, 0f),
			new Vector3(3f, 0f, 0f),
			new Vector3(4f, 0f, 0f)
		};
	}

	// Token: 0x04004DF0 RID: 19952
	public Vector3[] points;
}
