using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020007B1 RID: 1969
public class SnapXformToLine : MonoBehaviour
{
	// Token: 0x170004A1 RID: 1185
	// (get) Token: 0x06003167 RID: 12647 RVA: 0x00100F1F File Offset: 0x000FF11F
	public Vector3 linePoint
	{
		get
		{
			return this._closest;
		}
	}

	// Token: 0x170004A2 RID: 1186
	// (get) Token: 0x06003168 RID: 12648 RVA: 0x00100F27 File Offset: 0x000FF127
	public float linearDistance
	{
		get
		{
			return this._linear;
		}
	}

	// Token: 0x06003169 RID: 12649 RVA: 0x00100F2F File Offset: 0x000FF12F
	public void SnapTarget(bool applyToXform = true)
	{
		this.Snap(this.target, true);
	}

	// Token: 0x0600316A RID: 12650 RVA: 0x00100F3E File Offset: 0x000FF13E
	public void SnapTarget(Vector3 point)
	{
		if (this.target)
		{
			this.target.position = this.GetSnappedPoint(this.target.position);
		}
	}

	// Token: 0x0600316B RID: 12651 RVA: 0x00100F6C File Offset: 0x000FF16C
	public void SnapTargetLinear(float t)
	{
		if (this.target && this.from && this.to)
		{
			this.target.position = Vector3.Lerp(this.from.position, this.to.position, t);
		}
	}

	// Token: 0x0600316C RID: 12652 RVA: 0x00100FC7 File Offset: 0x000FF1C7
	public Vector3 GetSnappedPoint(Transform t)
	{
		return this.GetSnappedPoint(t.position);
	}

	// Token: 0x0600316D RID: 12653 RVA: 0x00100FD8 File Offset: 0x000FF1D8
	public Vector3 GetSnappedPoint(Vector3 point)
	{
		if (!this.apply)
		{
			return point;
		}
		if (!this.from || !this.to)
		{
			return point;
		}
		return SnapXformToLine.GetClosestPointOnLine(point, this.from.position, this.to.position);
	}

	// Token: 0x0600316E RID: 12654 RVA: 0x00101028 File Offset: 0x000FF228
	public void Snap(Transform xform, bool applyToXform = true)
	{
		if (!this.apply || !xform || !this.from || !this.to)
		{
			return;
		}
		Vector3 position = xform.position;
		Vector3 position2 = this.from.position;
		Vector3 position3 = this.to.position;
		Vector3 closestPointOnLine = SnapXformToLine.GetClosestPointOnLine(position, position2, position3);
		float num = Vector3.Distance(position2, position3);
		float num2 = Vector3.Distance(closestPointOnLine, position2);
		Vector3 closest = this._closest;
		Vector3 vector = closestPointOnLine;
		float linear = this._linear;
		float num3 = Mathf.Approximately(num, 0f) ? 0f : (num2 / (num + Mathf.Epsilon));
		this._closest = vector;
		this._linear = num3;
		if (this.output)
		{
			IRangedVariable<float> asT = this.output.AsT;
			asT.Set(asT.Min + this._linear * asT.Range);
		}
		if (applyToXform)
		{
			xform.position = this._closest;
			if (!Mathf.Approximately(closest.x, vector.x) || !Mathf.Approximately(closest.y, vector.y) || !Mathf.Approximately(closest.z, vector.z))
			{
				UnityEvent<Vector3> unityEvent = this.onPositionChanged;
				if (unityEvent != null)
				{
					unityEvent.Invoke(this._closest);
				}
			}
			if (!Mathf.Approximately(linear, num3))
			{
				UnityEvent<float> unityEvent2 = this.onLinearDistanceChanged;
				if (unityEvent2 != null)
				{
					unityEvent2.Invoke(this._linear);
				}
			}
			if (this.snapOrientation)
			{
				xform.forward = (position3 - position2).normalized;
				xform.up = Vector3.Lerp(this.from.up.normalized, this.to.up.normalized, this._linear);
			}
		}
	}

	// Token: 0x0600316F RID: 12655 RVA: 0x001011EE File Offset: 0x000FF3EE
	private void OnDisable()
	{
		if (this.resetOnDisable)
		{
			this.SnapTargetLinear(0f);
		}
	}

	// Token: 0x06003170 RID: 12656 RVA: 0x00101203 File Offset: 0x000FF403
	private void LateUpdate()
	{
		this.SnapTarget(true);
	}

	// Token: 0x06003171 RID: 12657 RVA: 0x0010120C File Offset: 0x000FF40C
	private static Vector3 GetClosestPointOnLine(Vector3 p, Vector3 a, Vector3 b)
	{
		Vector3 lhs = p - a;
		Vector3 vector = b - a;
		float sqrMagnitude = vector.sqrMagnitude;
		float d = Mathf.Clamp(Vector3.Dot(lhs, vector) / sqrMagnitude, 0f, 1f);
		return a + vector * d;
	}

	// Token: 0x04003D06 RID: 15622
	public bool apply = true;

	// Token: 0x04003D07 RID: 15623
	public bool snapOrientation = true;

	// Token: 0x04003D08 RID: 15624
	public bool resetOnDisable = true;

	// Token: 0x04003D09 RID: 15625
	[Space]
	public Transform target;

	// Token: 0x04003D0A RID: 15626
	[Space]
	public Transform from;

	// Token: 0x04003D0B RID: 15627
	public Transform to;

	// Token: 0x04003D0C RID: 15628
	private Vector3 _closest;

	// Token: 0x04003D0D RID: 15629
	private float _linear;

	// Token: 0x04003D0E RID: 15630
	public Ref<IRangedVariable<float>> output;

	// Token: 0x04003D0F RID: 15631
	public UnityEvent<float> onLinearDistanceChanged;

	// Token: 0x04003D10 RID: 15632
	public UnityEvent<Vector3> onPositionChanged;
}
