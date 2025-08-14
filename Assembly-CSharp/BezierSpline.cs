using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000AE6 RID: 2790
public class BezierSpline : MonoBehaviour
{
	// Token: 0x06004329 RID: 17193 RVA: 0x001517F0 File Offset: 0x0014F9F0
	private void Awake()
	{
		float num = 0f;
		for (int i = 1; i < this.points.Length; i++)
		{
			num += (this.points[i] - this.points[i - 1]).magnitude;
		}
		int subdivisions = Mathf.RoundToInt(num / 0.1f);
		this.buildTimesLenghtsTables(subdivisions);
	}

	// Token: 0x0600432A RID: 17194 RVA: 0x00151854 File Offset: 0x0014FA54
	private void buildTimesLenghtsTables(int subdivisions)
	{
		this._totalArcLength = 0f;
		float num = 1f / (float)subdivisions;
		this._timesTable = new float[subdivisions];
		this._lengthsTable = new float[subdivisions];
		Vector3 b = this.GetPoint(0f);
		for (int i = 1; i <= subdivisions; i++)
		{
			float num2 = num * (float)i;
			Vector3 point = this.GetPoint(num2);
			this._totalArcLength += Vector3.Distance(point, b);
			b = point;
			this._timesTable[i - 1] = num2;
			this._lengthsTable[i - 1] = this._totalArcLength;
		}
	}

	// Token: 0x0600432B RID: 17195 RVA: 0x001518EC File Offset: 0x0014FAEC
	private float getPathFromTime(float t)
	{
		if (float.IsNaN(this._totalArcLength) || this._totalArcLength == 0f)
		{
			return t;
		}
		if (t > 0f && t < 1f)
		{
			float num = this._totalArcLength * t;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			int num6 = this._lengthsTable.Length;
			int i = 0;
			while (i < num6)
			{
				if (this._lengthsTable[i] > num)
				{
					num4 = this._timesTable[i];
					num5 = this._lengthsTable[i];
					if (i > 0)
					{
						num3 = this._lengthsTable[i - 1];
						break;
					}
					break;
				}
				else
				{
					num2 = this._timesTable[i];
					i++;
				}
			}
			t = num2 + (num - num3) / (num5 - num3) * (num4 - num2);
		}
		if (t > 1f)
		{
			t = 1f;
		}
		else if (t < 0f)
		{
			t = 0f;
		}
		return t;
	}

	// Token: 0x0600432C RID: 17196 RVA: 0x001519D8 File Offset: 0x0014FBD8
	public void BuildSplineFromPoints(Vector3[] newPoints, BezierControlPointMode[] newModes, bool isLoop)
	{
		this.points = newPoints;
		this.modes = newModes;
		this.loop = isLoop;
		float num = 0f;
		for (int i = 1; i < this.points.Length; i++)
		{
			num += (this.points[i] - this.points[i - 1]).magnitude;
		}
		int subdivisions = Mathf.RoundToInt(num / 0.1f);
		this.buildTimesLenghtsTables(subdivisions);
	}

	// Token: 0x1700065E RID: 1630
	// (get) Token: 0x0600432D RID: 17197 RVA: 0x00151A51 File Offset: 0x0014FC51
	// (set) Token: 0x0600432E RID: 17198 RVA: 0x00151A59 File Offset: 0x0014FC59
	public bool Loop
	{
		get
		{
			return this.loop;
		}
		set
		{
			this.loop = value;
			if (value)
			{
				this.modes[this.modes.Length - 1] = this.modes[0];
				this.SetControlPoint(0, this.points[0]);
			}
		}
	}

	// Token: 0x1700065F RID: 1631
	// (get) Token: 0x0600432F RID: 17199 RVA: 0x00151A91 File Offset: 0x0014FC91
	public int ControlPointCount
	{
		get
		{
			return this.points.Length;
		}
	}

	// Token: 0x06004330 RID: 17200 RVA: 0x00151A9B File Offset: 0x0014FC9B
	public Vector3 GetControlPoint(int index)
	{
		return this.points[index];
	}

	// Token: 0x06004331 RID: 17201 RVA: 0x00151AAC File Offset: 0x0014FCAC
	public void SetControlPoint(int index, Vector3 point)
	{
		if (index % 3 == 0)
		{
			Vector3 b = point - this.points[index];
			if (this.loop)
			{
				if (index == 0)
				{
					this.points[1] += b;
					this.points[this.points.Length - 2] += b;
					this.points[this.points.Length - 1] = point;
				}
				else if (index == this.points.Length - 1)
				{
					this.points[0] = point;
					this.points[1] += b;
					this.points[index - 1] += b;
				}
				else
				{
					this.points[index - 1] += b;
					this.points[index + 1] += b;
				}
			}
			else
			{
				if (index > 0)
				{
					this.points[index - 1] += b;
				}
				if (index + 1 < this.points.Length)
				{
					this.points[index + 1] += b;
				}
			}
		}
		this.points[index] = point;
		this.EnforceMode(index);
	}

	// Token: 0x06004332 RID: 17202 RVA: 0x00151C3E File Offset: 0x0014FE3E
	public BezierControlPointMode GetControlPointMode(int index)
	{
		return this.modes[(index + 1) / 3];
	}

	// Token: 0x06004333 RID: 17203 RVA: 0x00151C4C File Offset: 0x0014FE4C
	public void SetControlPointMode(int index, BezierControlPointMode mode)
	{
		int num = (index + 1) / 3;
		this.modes[num] = mode;
		if (this.loop)
		{
			if (num == 0)
			{
				this.modes[this.modes.Length - 1] = mode;
			}
			else if (num == this.modes.Length - 1)
			{
				this.modes[0] = mode;
			}
		}
		this.EnforceMode(index);
	}

	// Token: 0x06004334 RID: 17204 RVA: 0x00151CA4 File Offset: 0x0014FEA4
	private void EnforceMode(int index)
	{
		int num = (index + 1) / 3;
		BezierControlPointMode bezierControlPointMode = this.modes[num];
		if (bezierControlPointMode == BezierControlPointMode.Free || (!this.loop && (num == 0 || num == this.modes.Length - 1)))
		{
			return;
		}
		int num2 = num * 3;
		int num3;
		int num4;
		if (index <= num2)
		{
			num3 = num2 - 1;
			if (num3 < 0)
			{
				num3 = this.points.Length - 2;
			}
			num4 = num2 + 1;
			if (num4 >= this.points.Length)
			{
				num4 = 1;
			}
		}
		else
		{
			num3 = num2 + 1;
			if (num3 >= this.points.Length)
			{
				num3 = 1;
			}
			num4 = num2 - 1;
			if (num4 < 0)
			{
				num4 = this.points.Length - 2;
			}
		}
		Vector3 a = this.points[num2];
		Vector3 b = a - this.points[num3];
		if (bezierControlPointMode == BezierControlPointMode.Aligned)
		{
			b = b.normalized * Vector3.Distance(a, this.points[num4]);
		}
		this.points[num4] = a + b;
	}

	// Token: 0x17000660 RID: 1632
	// (get) Token: 0x06004335 RID: 17205 RVA: 0x00151D93 File Offset: 0x0014FF93
	public int CurveCount
	{
		get
		{
			return (this.points.Length - 1) / 3;
		}
	}

	// Token: 0x06004336 RID: 17206 RVA: 0x00151DA1 File Offset: 0x0014FFA1
	public Vector3 GetPoint(float t, bool ConstantVelocity)
	{
		if (ConstantVelocity)
		{
			return this.GetPoint(this.getPathFromTime(t));
		}
		return this.GetPoint(t);
	}

	// Token: 0x06004337 RID: 17207 RVA: 0x00151DBC File Offset: 0x0014FFBC
	public Vector3 GetPoint(float t)
	{
		int num;
		if (t >= 1f)
		{
			t = 1f;
			num = this.points.Length - 4;
		}
		else
		{
			t = Mathf.Clamp01(t) * (float)this.CurveCount;
			num = (int)t;
			t -= (float)num;
			num *= 3;
		}
		return base.transform.TransformPoint(Bezier.GetPoint(this.points[num], this.points[num + 1], this.points[num + 2], this.points[num + 3], t));
	}

	// Token: 0x06004338 RID: 17208 RVA: 0x00151E4C File Offset: 0x0015004C
	public Vector3 GetPointLocal(float t)
	{
		int num;
		if (t >= 1f)
		{
			t = 1f;
			num = this.points.Length - 4;
		}
		else
		{
			t = Mathf.Clamp01(t) * (float)this.CurveCount;
			num = (int)t;
			t -= (float)num;
			num *= 3;
		}
		return Bezier.GetPoint(this.points[num], this.points[num + 1], this.points[num + 2], this.points[num + 3], t);
	}

	// Token: 0x06004339 RID: 17209 RVA: 0x00151ED0 File Offset: 0x001500D0
	public Vector3 GetVelocity(float t)
	{
		int num;
		if (t >= 1f)
		{
			t = 1f;
			num = this.points.Length - 4;
		}
		else
		{
			t = Mathf.Clamp01(t) * (float)this.CurveCount;
			num = (int)t;
			t -= (float)num;
			num *= 3;
		}
		return base.transform.TransformPoint(Bezier.GetFirstDerivative(this.points[num], this.points[num + 1], this.points[num + 2], this.points[num + 3], t)) - base.transform.position;
	}

	// Token: 0x0600433A RID: 17210 RVA: 0x00151F6D File Offset: 0x0015016D
	public Vector3 GetDirection(float t, bool ConstantVelocity)
	{
		if (ConstantVelocity)
		{
			return this.GetDirection(this.getPathFromTime(t));
		}
		return this.GetDirection(t);
	}

	// Token: 0x0600433B RID: 17211 RVA: 0x00151F88 File Offset: 0x00150188
	public Vector3 GetDirection(float t)
	{
		return this.GetVelocity(t).normalized;
	}

	// Token: 0x0600433C RID: 17212 RVA: 0x00151FA4 File Offset: 0x001501A4
	public void AddCurve()
	{
		Vector3 vector = this.points[this.points.Length - 1];
		Array.Resize<Vector3>(ref this.points, this.points.Length + 3);
		vector.x += 1f;
		this.points[this.points.Length - 3] = vector;
		vector.x += 1f;
		this.points[this.points.Length - 2] = vector;
		vector.x += 1f;
		this.points[this.points.Length - 1] = vector;
		Array.Resize<BezierControlPointMode>(ref this.modes, this.modes.Length + 1);
		this.modes[this.modes.Length - 1] = this.modes[this.modes.Length - 2];
		this.EnforceMode(this.points.Length - 4);
		if (this.loop)
		{
			this.points[this.points.Length - 1] = this.points[0];
			this.modes[this.modes.Length - 1] = this.modes[0];
			this.EnforceMode(0);
		}
	}

	// Token: 0x0600433D RID: 17213 RVA: 0x001520DE File Offset: 0x001502DE
	public void RemoveLastCurve()
	{
		if (this.points.Length <= 4)
		{
			return;
		}
		Array.Resize<Vector3>(ref this.points, this.points.Length - 3);
		Array.Resize<BezierControlPointMode>(ref this.modes, this.modes.Length - 1);
	}

	// Token: 0x0600433E RID: 17214 RVA: 0x00152118 File Offset: 0x00150318
	public void RemoveCurve(int index)
	{
		if (this.points.Length <= 4)
		{
			return;
		}
		List<Vector3> list = this.points.ToList<Vector3>();
		int num = 4;
		while (num < this.points.Length && index - 3 > num)
		{
			num += 3;
		}
		for (int i = 0; i < 3; i++)
		{
			list.RemoveAt(num);
		}
		this.points = list.ToArray();
		int index2 = (num - 4) / 3;
		List<BezierControlPointMode> list2 = this.modes.ToList<BezierControlPointMode>();
		list2.RemoveAt(index2);
		this.modes = list2.ToArray();
	}

	// Token: 0x0600433F RID: 17215 RVA: 0x001521A0 File Offset: 0x001503A0
	public void Reset()
	{
		this.points = new Vector3[]
		{
			new Vector3(0f, -1f, 0f),
			new Vector3(0f, -1f, 2f),
			new Vector3(0f, -1f, 4f),
			new Vector3(0f, -1f, 6f)
		};
		this.modes = new BezierControlPointMode[2];
	}

	// Token: 0x04004DF1 RID: 19953
	[SerializeField]
	private Vector3[] points;

	// Token: 0x04004DF2 RID: 19954
	[SerializeField]
	private BezierControlPointMode[] modes;

	// Token: 0x04004DF3 RID: 19955
	[SerializeField]
	private bool loop;

	// Token: 0x04004DF4 RID: 19956
	private float _totalArcLength;

	// Token: 0x04004DF5 RID: 19957
	private float[] _timesTable;

	// Token: 0x04004DF6 RID: 19958
	private float[] _lengthsTable;
}
