using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000588 RID: 1416
public class FixedSizeTrailAdjustBySpeed : MonoBehaviour
{
	// Token: 0x0600228C RID: 8844 RVA: 0x000BAA32 File Offset: 0x000B8C32
	private void Start()
	{
		this.Setup();
	}

	// Token: 0x0600228D RID: 8845 RVA: 0x000BAA3C File Offset: 0x000B8C3C
	private void Setup()
	{
		this._lastPosition = base.transform.position;
		this._rawVelocity = Vector3.zero;
		this._rawSpeed = 0f;
		this._speed = 0f;
		if (this.trail)
		{
			this._initGravity = this.trail.gravity;
			this.trail.applyPhysics = this.adjustPhysics;
		}
		this.LerpTrailColors(0.5f);
	}

	// Token: 0x0600228E RID: 8846 RVA: 0x000BAAB8 File Offset: 0x000B8CB8
	private void LerpTrailColors(float t = 0.5f)
	{
		GradientColorKey[] colorKeys = this._mixGradient.colorKeys;
		int num = colorKeys.Length;
		for (int i = 0; i < num; i++)
		{
			float time = (float)i / (float)(num - 1);
			Color a = this.minColors.Evaluate(time);
			Color b = this.maxColors.Evaluate(time);
			Color color = Color.Lerp(a, b, t);
			colorKeys[i].color = color;
			colorKeys[i].time = time;
		}
		this._mixGradient.colorKeys = colorKeys;
		if (this.trail)
		{
			this.trail.renderer.colorGradient = this._mixGradient;
		}
	}

	// Token: 0x0600228F RID: 8847 RVA: 0x000BAB58 File Offset: 0x000B8D58
	private void Update()
	{
		float deltaTime = Time.deltaTime;
		Vector3 position = base.transform.position;
		this._rawVelocity = (position - this._lastPosition) / deltaTime;
		this._rawSpeed = this._rawVelocity.magnitude;
		if (this._rawSpeed > this.retractMin)
		{
			this._speed += this.expandSpeed * deltaTime;
		}
		if (this._rawSpeed <= this.retractMin)
		{
			this._speed -= this.retractSpeed * deltaTime;
		}
		if (this._speed > this.maxSpeed)
		{
			this._speed = this.maxSpeed;
		}
		this._speed = Mathf.Lerp(this._lastSpeed, this._speed, 0.5f);
		if (this._speed < 0.01f)
		{
			this._speed = 0f;
		}
		this.AdjustTrail();
		this._lastSpeed = this._speed;
		this._lastPosition = position;
	}

	// Token: 0x06002290 RID: 8848 RVA: 0x000BAC50 File Offset: 0x000B8E50
	private void AdjustTrail()
	{
		if (!this.trail)
		{
			return;
		}
		float num = MathUtils.Linear(this._speed, this.minSpeed, this.maxSpeed, 0f, 1f);
		float length = MathUtils.Linear(num, 0f, 1f, this.minLength, this.maxLength);
		this.trail.length = length;
		this.LerpTrailColors(num);
		if (this.adjustPhysics)
		{
			Transform transform = base.transform;
			Vector3 b = transform.forward * this.gravityOffset.z + transform.right * this.gravityOffset.x + transform.up * this.gravityOffset.y;
			Vector3 b2 = (this._initGravity + b) * (1f - num);
			this.trail.gravity = Vector3.Lerp(Vector3.zero, b2, 0.5f);
		}
	}

	// Token: 0x04002C1D RID: 11293
	public FixedSizeTrail trail;

	// Token: 0x04002C1E RID: 11294
	public bool adjustPhysics = true;

	// Token: 0x04002C1F RID: 11295
	private Vector3 _rawVelocity;

	// Token: 0x04002C20 RID: 11296
	private float _rawSpeed;

	// Token: 0x04002C21 RID: 11297
	private float _speed;

	// Token: 0x04002C22 RID: 11298
	private float _lastSpeed;

	// Token: 0x04002C23 RID: 11299
	private Vector3 _lastPosition;

	// Token: 0x04002C24 RID: 11300
	private Vector3 _initGravity;

	// Token: 0x04002C25 RID: 11301
	public Vector3 gravityOffset = Vector3.zero;

	// Token: 0x04002C26 RID: 11302
	[Space]
	public float retractMin = 0.5f;

	// Token: 0x04002C27 RID: 11303
	[Space]
	[FormerlySerializedAs("sizeIncreaseSpeed")]
	public float expandSpeed = 16f;

	// Token: 0x04002C28 RID: 11304
	[FormerlySerializedAs("sizeDecreaseSpeed")]
	public float retractSpeed = 4f;

	// Token: 0x04002C29 RID: 11305
	[Space]
	public float minSpeed;

	// Token: 0x04002C2A RID: 11306
	public float minLength = 1f;

	// Token: 0x04002C2B RID: 11307
	public Gradient minColors = GradientHelper.FromColor(new Color(0f, 1f, 1f, 1f));

	// Token: 0x04002C2C RID: 11308
	[Space]
	public float maxSpeed = 10f;

	// Token: 0x04002C2D RID: 11309
	public float maxLength = 8f;

	// Token: 0x04002C2E RID: 11310
	public Gradient maxColors = GradientHelper.FromColor(new Color(1f, 1f, 0f, 1f));

	// Token: 0x04002C2F RID: 11311
	[Space]
	[SerializeField]
	private Gradient _mixGradient = new Gradient
	{
		colorKeys = new GradientColorKey[8],
		alphaKeys = Array.Empty<GradientAlphaKey>()
	};

	// Token: 0x02000589 RID: 1417
	[Serializable]
	public struct GradientKey
	{
		// Token: 0x06002292 RID: 8850 RVA: 0x000BAE25 File Offset: 0x000B9025
		public GradientKey(Color color, float time)
		{
			this.color = color;
			this.time = time;
		}

		// Token: 0x04002C30 RID: 11312
		public Color color;

		// Token: 0x04002C31 RID: 11313
		public float time;
	}
}
