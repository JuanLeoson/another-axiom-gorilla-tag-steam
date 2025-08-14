using System;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000448 RID: 1096
[RequireComponent(typeof(BezierSpline))]
public class ManipulatableSpinner : ManipulatableObject
{
	// Token: 0x170002EE RID: 750
	// (get) Token: 0x06001AE1 RID: 6881 RVA: 0x0008F781 File Offset: 0x0008D981
	// (set) Token: 0x06001AE2 RID: 6882 RVA: 0x0008F789 File Offset: 0x0008D989
	public float angle { get; private set; }

	// Token: 0x06001AE3 RID: 6883 RVA: 0x0008F792 File Offset: 0x0008D992
	private void Awake()
	{
		this.spline = base.GetComponent<BezierSpline>();
	}

	// Token: 0x06001AE4 RID: 6884 RVA: 0x0008F7A0 File Offset: 0x0008D9A0
	protected override void OnStartManipulation(GameObject grabbingHand)
	{
		Vector3 position = grabbingHand.transform.position;
		float num = this.FindPositionOnSpline(position);
		this.previousHandT = num;
	}

	// Token: 0x06001AE5 RID: 6885 RVA: 0x000023F5 File Offset: 0x000005F5
	protected override void OnStopManipulation(GameObject releasingHand, Vector3 releaseVelocity)
	{
	}

	// Token: 0x06001AE6 RID: 6886 RVA: 0x0008F7C8 File Offset: 0x0008D9C8
	protected override bool ShouldHandDetach(GameObject hand)
	{
		if (!this.spline.Loop && (this.currentHandT >= 0.99f || this.currentHandT <= 0.01f))
		{
			return true;
		}
		Vector3 position = hand.transform.position;
		Vector3 point = this.spline.GetPoint(this.currentHandT);
		return Vector3.SqrMagnitude(position - point) > this.breakDistance * this.breakDistance;
	}

	// Token: 0x06001AE7 RID: 6887 RVA: 0x0008F838 File Offset: 0x0008DA38
	protected override void OnHeldUpdate(GameObject hand)
	{
		float angle = this.angle;
		Vector3 position = hand.transform.position;
		this.currentHandT = this.FindPositionOnSpline(position);
		float num = this.currentHandT - this.previousHandT;
		if (this.spline.Loop)
		{
			if (num > 0.5f)
			{
				num -= 1f;
			}
			else if (num < -0.5f)
			{
				num += 1f;
			}
		}
		this.angle += num;
		this.previousHandT = this.currentHandT;
		if (this.applyReleaseVelocity && this.currentHandT <= 0.99f && this.currentHandT >= 0.01f)
		{
			this.tVelocity = (this.angle - angle) / Time.deltaTime;
		}
	}

	// Token: 0x06001AE8 RID: 6888 RVA: 0x0008F8F4 File Offset: 0x0008DAF4
	protected override void OnReleasedUpdate()
	{
		if (this.tVelocity != 0f)
		{
			this.angle += this.tVelocity * Time.deltaTime;
			if (Mathf.Abs(this.tVelocity) < this.lowSpeedThreshold)
			{
				this.tVelocity *= 1f - this.lowSpeedDrag * Time.deltaTime;
				return;
			}
			this.tVelocity *= 1f - this.releaseDrag * Time.deltaTime;
		}
	}

	// Token: 0x06001AE9 RID: 6889 RVA: 0x0008F97C File Offset: 0x0008DB7C
	private float FindPositionOnSpline(Vector3 grabPoint)
	{
		int i = 0;
		int num = 200;
		float num2 = 0.001f;
		float num3 = 1f / (float)num;
		float3 y = base.transform.InverseTransformPoint(grabPoint);
		float result = 0f;
		float num4 = float.PositiveInfinity;
		while (i < num)
		{
			float num5 = math.distancesq(this.spline.GetPointLocal(num2), y);
			if (num5 < num4)
			{
				num4 = num5;
				result = num2;
			}
			num2 += num3;
			i++;
		}
		return result;
	}

	// Token: 0x06001AEA RID: 6890 RVA: 0x0008F9F8 File Offset: 0x0008DBF8
	public void SetAngle(float newAngle)
	{
		this.angle = newAngle;
	}

	// Token: 0x06001AEB RID: 6891 RVA: 0x0008FA01 File Offset: 0x0008DC01
	public void SetVelocity(float newVelocity)
	{
		this.tVelocity = newVelocity;
	}

	// Token: 0x04002327 RID: 8999
	public float breakDistance = 0.2f;

	// Token: 0x04002328 RID: 9000
	public bool applyReleaseVelocity;

	// Token: 0x04002329 RID: 9001
	public float releaseDrag = 1f;

	// Token: 0x0400232A RID: 9002
	public float lowSpeedThreshold = 0.12f;

	// Token: 0x0400232B RID: 9003
	public float lowSpeedDrag = 3f;

	// Token: 0x0400232C RID: 9004
	private BezierSpline spline;

	// Token: 0x0400232D RID: 9005
	private float previousHandT;

	// Token: 0x0400232E RID: 9006
	private float currentHandT;

	// Token: 0x0400232F RID: 9007
	private float tVelocity;
}
