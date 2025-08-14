using System;
using UnityEngine;

// Token: 0x020000E3 RID: 227
public class SpiderDangler : MonoBehaviour
{
	// Token: 0x060005AD RID: 1453 RVA: 0x00020E50 File Offset: 0x0001F050
	protected void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
		Vector3 position = base.transform.position;
		float magnitude = (this.endTransform.position - position).magnitude;
		this.ropeSegLen = magnitude / 6f;
		this.ropeSegs = new SpiderDangler.RopeSegment[6];
		for (int i = 0; i < 6; i++)
		{
			this.ropeSegs[i] = new SpiderDangler.RopeSegment(position);
			position.y -= this.ropeSegLen;
		}
	}

	// Token: 0x060005AE RID: 1454 RVA: 0x00020ED7 File Offset: 0x0001F0D7
	protected void FixedUpdate()
	{
		this.Simulate();
	}

	// Token: 0x060005AF RID: 1455 RVA: 0x00020EE0 File Offset: 0x0001F0E0
	protected void LateUpdate()
	{
		this.DrawRope();
		Vector3 normalized = (this.ropeSegs[this.ropeSegs.Length - 2].pos - this.ropeSegs[this.ropeSegs.Length - 1].pos).normalized;
		this.endTransform.position = this.ropeSegs[this.ropeSegs.Length - 1].pos;
		this.endTransform.up = normalized;
		Vector4 vector = this.spinSpeeds * Time.time;
		vector = new Vector4(Mathf.Sin(vector.x), Mathf.Sin(vector.y), Mathf.Sin(vector.z), Mathf.Sin(vector.w));
		vector.Scale(this.spinScales);
		this.endTransform.Rotate(Vector3.up, vector.x + vector.y + vector.z + vector.w);
	}

	// Token: 0x060005B0 RID: 1456 RVA: 0x00020FE4 File Offset: 0x0001F1E4
	private void Simulate()
	{
		this.ropeSegLenScaled = this.ropeSegLen * base.transform.lossyScale.x;
		Vector3 b = new Vector3(0f, -0.5f, 0f) * Time.fixedDeltaTime;
		for (int i = 1; i < 6; i++)
		{
			Vector3 a = this.ropeSegs[i].pos - this.ropeSegs[i].posOld;
			this.ropeSegs[i].posOld = this.ropeSegs[i].pos;
			SpiderDangler.RopeSegment[] array = this.ropeSegs;
			int num = i;
			array[num].pos = array[num].pos + a * 0.95f;
			SpiderDangler.RopeSegment[] array2 = this.ropeSegs;
			int num2 = i;
			array2[num2].pos = array2[num2].pos + b;
		}
		for (int j = 0; j < 8; j++)
		{
			this.ApplyConstraint();
		}
	}

	// Token: 0x060005B1 RID: 1457 RVA: 0x000210EC File Offset: 0x0001F2EC
	private void ApplyConstraint()
	{
		this.ropeSegs[0].pos = base.transform.position;
		this.ApplyConstraintSegment(ref this.ropeSegs[0], ref this.ropeSegs[1], 0f, 1f);
		for (int i = 1; i < 5; i++)
		{
			this.ApplyConstraintSegment(ref this.ropeSegs[i], ref this.ropeSegs[i + 1], 0.5f, 0.5f);
		}
	}

	// Token: 0x060005B2 RID: 1458 RVA: 0x00021174 File Offset: 0x0001F374
	private void ApplyConstraintSegment(ref SpiderDangler.RopeSegment segA, ref SpiderDangler.RopeSegment segB, float dampenA, float dampenB)
	{
		float d = (segA.pos - segB.pos).magnitude - this.ropeSegLenScaled;
		Vector3 a = (segA.pos - segB.pos).normalized * d;
		segA.pos -= a * dampenA;
		segB.pos += a * dampenB;
	}

	// Token: 0x060005B3 RID: 1459 RVA: 0x00021200 File Offset: 0x0001F400
	private void DrawRope()
	{
		Vector3[] array = new Vector3[6];
		for (int i = 0; i < 6; i++)
		{
			array[i] = this.ropeSegs[i].pos;
		}
		this.lineRenderer.positionCount = array.Length;
		this.lineRenderer.SetPositions(array);
	}

	// Token: 0x040006CB RID: 1739
	public Transform endTransform;

	// Token: 0x040006CC RID: 1740
	public Vector4 spinSpeeds = new Vector4(0.1f, 0.2f, 0.3f, 0.4f);

	// Token: 0x040006CD RID: 1741
	public Vector4 spinScales = new Vector4(180f, 90f, 120f, 180f);

	// Token: 0x040006CE RID: 1742
	private LineRenderer lineRenderer;

	// Token: 0x040006CF RID: 1743
	private SpiderDangler.RopeSegment[] ropeSegs;

	// Token: 0x040006D0 RID: 1744
	private float ropeSegLen;

	// Token: 0x040006D1 RID: 1745
	private float ropeSegLenScaled;

	// Token: 0x040006D2 RID: 1746
	private const int kSegmentCount = 6;

	// Token: 0x040006D3 RID: 1747
	private const float kVelocityDamper = 0.95f;

	// Token: 0x040006D4 RID: 1748
	private const int kConstraintCalculationIterations = 8;

	// Token: 0x020000E4 RID: 228
	public struct RopeSegment
	{
		// Token: 0x060005B5 RID: 1461 RVA: 0x000212A5 File Offset: 0x0001F4A5
		public RopeSegment(Vector3 pos)
		{
			this.pos = pos;
			this.posOld = pos;
		}

		// Token: 0x040006D5 RID: 1749
		public Vector3 pos;

		// Token: 0x040006D6 RID: 1750
		public Vector3 posOld;
	}
}
