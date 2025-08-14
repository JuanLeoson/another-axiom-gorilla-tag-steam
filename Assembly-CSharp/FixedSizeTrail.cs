using System;
using UnityEngine;

// Token: 0x02000587 RID: 1415
[RequireComponent(typeof(LineRenderer))]
public class FixedSizeTrail : MonoBehaviour
{
	// Token: 0x17000372 RID: 882
	// (get) Token: 0x06002280 RID: 8832 RVA: 0x000BA612 File Offset: 0x000B8812
	public LineRenderer renderer
	{
		get
		{
			return this._lineRenderer;
		}
	}

	// Token: 0x17000373 RID: 883
	// (get) Token: 0x06002281 RID: 8833 RVA: 0x000BA61A File Offset: 0x000B881A
	// (set) Token: 0x06002282 RID: 8834 RVA: 0x000BA622 File Offset: 0x000B8822
	public float length
	{
		get
		{
			return this._length;
		}
		set
		{
			this._length = Math.Clamp(value, 0f, 128f);
		}
	}

	// Token: 0x17000374 RID: 884
	// (get) Token: 0x06002283 RID: 8835 RVA: 0x000BA63A File Offset: 0x000B883A
	public Vector3[] points
	{
		get
		{
			return this._points;
		}
	}

	// Token: 0x06002284 RID: 8836 RVA: 0x000BA642 File Offset: 0x000B8842
	private void Reset()
	{
		this.Setup();
	}

	// Token: 0x06002285 RID: 8837 RVA: 0x000BA642 File Offset: 0x000B8842
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x06002286 RID: 8838 RVA: 0x000BA64C File Offset: 0x000B884C
	private void Setup()
	{
		this._transform = base.transform;
		if (this._lineRenderer == null)
		{
			this._lineRenderer = base.GetComponent<LineRenderer>();
		}
		if (!this._lineRenderer)
		{
			return;
		}
		this._lineRenderer.useWorldSpace = true;
		Vector3 position = this._transform.position;
		Vector3 forward = this._transform.forward;
		int num = this._segments + 1;
		this._points = new Vector3[num];
		float d = this._length / (float)this._segments;
		for (int i = 0; i < num; i++)
		{
			this._points[i] = position - forward * d * (float)i;
		}
		this._lineRenderer.positionCount = num;
		this._lineRenderer.SetPositions(this._points);
		this.Update();
	}

	// Token: 0x06002287 RID: 8839 RVA: 0x000BA72A File Offset: 0x000B892A
	private void Update()
	{
		if (!this.manualUpdate)
		{
			this.Update(Time.deltaTime);
		}
	}

	// Token: 0x06002288 RID: 8840 RVA: 0x000BA740 File Offset: 0x000B8940
	private void FixedUpdate()
	{
		if (!this.applyPhysics)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		int num = this._points.Length - 1;
		float num2 = this._length / (float)num;
		for (int i = 1; i < num; i++)
		{
			float time = (float)(i - 1) / (float)num;
			float num3 = this.gravityCurve.Evaluate(time);
			Vector3 b = this.gravity * (num3 * deltaTime);
			this._points[i] += b;
			this._points[i + 1] += b;
		}
	}

	// Token: 0x06002289 RID: 8841 RVA: 0x000BA7E4 File Offset: 0x000B89E4
	public void Update(float dt)
	{
		float num = this._length / (float)(this._segments - 1);
		Vector3 position = this._transform.position;
		this._points[0] = position;
		float num2 = Vector3.Distance(this._points[0], this._points[1]);
		float num3 = num - num2;
		if (num2 > num)
		{
			Array.Copy(this._points, 0, this._points, 1, this._points.Length - 1);
		}
		for (int i = 0; i < this._points.Length - 1; i++)
		{
			Vector3 vector = this._points[i];
			Vector3 vector2 = this._points[i + 1] - vector;
			if (vector2.sqrMagnitude > num * num)
			{
				this._points[i + 1] = vector + vector2.normalized * num;
			}
		}
		if (num3 > 0f)
		{
			int num4 = this._points.Length - 1;
			int num5 = num4 - 1;
			Vector3 vector3 = this._points[num4] - this._points[num5];
			Vector3 a = vector3.normalized;
			if (this.applyPhysics)
			{
				Vector3 normalized = (this._points[num5] - this._points[num5 - 1]).normalized;
				a = Vector3.Lerp(a, normalized, 0.5f);
			}
			this._points[num4] = this._points[num5] + a * Math.Min(vector3.magnitude, num3);
		}
		this._lineRenderer.SetPositions(this._points);
	}

	// Token: 0x0600228A RID: 8842 RVA: 0x000BA99C File Offset: 0x000B8B9C
	private static float CalcLength(in Vector3[] positions)
	{
		float num = 0f;
		for (int i = 0; i < positions.Length - 1; i++)
		{
			num += Vector3.Distance(positions[i], positions[i + 1]);
		}
		return num;
	}

	// Token: 0x04002C14 RID: 11284
	[SerializeField]
	private Transform _transform;

	// Token: 0x04002C15 RID: 11285
	[SerializeField]
	private LineRenderer _lineRenderer;

	// Token: 0x04002C16 RID: 11286
	[SerializeField]
	[Range(1f, 128f)]
	private int _segments = 8;

	// Token: 0x04002C17 RID: 11287
	[SerializeField]
	private float _length = 8f;

	// Token: 0x04002C18 RID: 11288
	public bool manualUpdate;

	// Token: 0x04002C19 RID: 11289
	[Space]
	public bool applyPhysics;

	// Token: 0x04002C1A RID: 11290
	public Vector3 gravity = new Vector3(0f, -9.8f, 0f);

	// Token: 0x04002C1B RID: 11291
	public AnimationCurve gravityCurve = AnimationCurves.EaseInCubic;

	// Token: 0x04002C1C RID: 11292
	[Space]
	private Vector3[] _points = new Vector3[8];
}
