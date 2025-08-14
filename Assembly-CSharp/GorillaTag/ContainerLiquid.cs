using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000E4D RID: 3661
	[AddComponentMenu("GorillaTag/ContainerLiquid (GTag)")]
	[ExecuteInEditMode]
	public class ContainerLiquid : MonoBehaviour
	{
		// Token: 0x170008DE RID: 2270
		// (get) Token: 0x06005BE7 RID: 23527 RVA: 0x001CF408 File Offset: 0x001CD608
		[DebugReadout]
		public bool isEmpty
		{
			get
			{
				return this.fillAmount <= this.refillThreshold;
			}
		}

		// Token: 0x170008DF RID: 2271
		// (get) Token: 0x06005BE8 RID: 23528 RVA: 0x001CF41B File Offset: 0x001CD61B
		// (set) Token: 0x06005BE9 RID: 23529 RVA: 0x001CF423 File Offset: 0x001CD623
		public Vector3 cupTopWorldPos { get; private set; }

		// Token: 0x170008E0 RID: 2272
		// (get) Token: 0x06005BEA RID: 23530 RVA: 0x001CF42C File Offset: 0x001CD62C
		// (set) Token: 0x06005BEB RID: 23531 RVA: 0x001CF434 File Offset: 0x001CD634
		public Vector3 bottomLipWorldPos { get; private set; }

		// Token: 0x170008E1 RID: 2273
		// (get) Token: 0x06005BEC RID: 23532 RVA: 0x001CF43D File Offset: 0x001CD63D
		// (set) Token: 0x06005BED RID: 23533 RVA: 0x001CF445 File Offset: 0x001CD645
		public Vector3 liquidPlaneWorldPos { get; private set; }

		// Token: 0x170008E2 RID: 2274
		// (get) Token: 0x06005BEE RID: 23534 RVA: 0x001CF44E File Offset: 0x001CD64E
		// (set) Token: 0x06005BEF RID: 23535 RVA: 0x001CF456 File Offset: 0x001CD656
		public Vector3 liquidPlaneWorldNormal { get; private set; }

		// Token: 0x06005BF0 RID: 23536 RVA: 0x001CF460 File Offset: 0x001CD660
		protected bool IsValidLiquidSurfaceValues()
		{
			return this.meshRenderer != null && this.meshFilter != null && this.spillParticleSystem != null && !string.IsNullOrEmpty(this.liquidColorShaderPropertyName) && !string.IsNullOrEmpty(this.liquidPlaneNormalShaderPropertyName) && !string.IsNullOrEmpty(this.liquidPlanePositionShaderPropertyName);
		}

		// Token: 0x06005BF1 RID: 23537 RVA: 0x001CF4C4 File Offset: 0x001CD6C4
		protected void InitializeLiquidSurface()
		{
			this.liquidColorShaderProp = Shader.PropertyToID(this.liquidColorShaderPropertyName);
			this.liquidPlaneNormalShaderProp = Shader.PropertyToID(this.liquidPlaneNormalShaderPropertyName);
			this.liquidPlanePositionShaderProp = Shader.PropertyToID(this.liquidPlanePositionShaderPropertyName);
			this.localMeshBounds = this.meshFilter.sharedMesh.bounds;
		}

		// Token: 0x06005BF2 RID: 23538 RVA: 0x001CF51C File Offset: 0x001CD71C
		protected void InitializeParticleSystem()
		{
			this.spillParticleSystem.main.startColor = this.liquidColor;
		}

		// Token: 0x06005BF3 RID: 23539 RVA: 0x001CF547 File Offset: 0x001CD747
		protected void Awake()
		{
			this.matPropBlock = new MaterialPropertyBlock();
			this.topVerts = this.GetTopVerts();
		}

		// Token: 0x06005BF4 RID: 23540 RVA: 0x001CF560 File Offset: 0x001CD760
		protected void OnEnable()
		{
			if (Application.isPlaying)
			{
				base.enabled = (this.useLiquidShader && this.IsValidLiquidSurfaceValues());
				if (base.enabled)
				{
					this.InitializeLiquidSurface();
				}
				this.InitializeParticleSystem();
				this.useFloater = (this.floater != null);
			}
		}

		// Token: 0x06005BF5 RID: 23541 RVA: 0x001CF5B4 File Offset: 0x001CD7B4
		protected void LateUpdate()
		{
			this.UpdateRefillTimer();
			Transform transform = base.transform;
			Vector3 position = transform.position;
			Quaternion rotation = transform.rotation;
			Bounds bounds = this.meshRenderer.bounds;
			Vector3 a = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
			Vector3 b = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
			this.liquidPlaneWorldPos = Vector3.Lerp(a, b, this.fillAmount);
			Vector3 v = transform.InverseTransformPoint(this.liquidPlaneWorldPos);
			float deltaTime = Time.deltaTime;
			this.temporalWobbleAmp = Vector2.Lerp(this.temporalWobbleAmp, Vector2.zero, deltaTime * this.recovery);
			float num = 6.2831855f * this.wobbleFrequency;
			float d = Mathf.Lerp(this.lastSineWave, Mathf.Sin(num * Time.realtimeSinceStartup), deltaTime * Mathf.Clamp(this.lastVelocity.magnitude + this.lastAngularVelocity.magnitude, this.thickness, 10f));
			Vector2 vector = this.temporalWobbleAmp * d;
			this.liquidPlaneWorldNormal = new Vector3(vector.x, -1f, vector.y).normalized;
			Vector3 v2 = transform.InverseTransformDirection(this.liquidPlaneWorldNormal);
			if (this.useLiquidShader)
			{
				this.matPropBlock.SetVector(this.liquidPlaneNormalShaderProp, v2);
				this.matPropBlock.SetVector(this.liquidPlanePositionShaderProp, v);
				this.matPropBlock.SetVector(this.liquidColorShaderProp, this.liquidColor.linear);
				if (this.useLiquidVolume)
				{
					float value = MathUtils.Linear(this.fillAmount, 0f, 1f, this.liquidVolumeMinMax.x, this.liquidVolumeMinMax.y);
					this.matPropBlock.SetFloat(ShaderProps._LiquidFill, value);
				}
				this.meshRenderer.SetPropertyBlock(this.matPropBlock);
			}
			if (this.useFloater)
			{
				float y = Mathf.Lerp(this.localMeshBounds.min.y, this.localMeshBounds.max.y, this.fillAmount);
				this.floater.localPosition = this.floater.localPosition.WithY(y);
			}
			Vector3 vector2 = (this.lastPos - position) / deltaTime;
			Vector3 angularVelocity = GorillaMath.GetAngularVelocity(this.lastRot, rotation);
			this.temporalWobbleAmp.x = this.temporalWobbleAmp.x + Mathf.Clamp((vector2.x + vector2.y * 0.2f + angularVelocity.z + angularVelocity.y) * this.wobbleMax, -this.wobbleMax, this.wobbleMax);
			this.temporalWobbleAmp.y = this.temporalWobbleAmp.y + Mathf.Clamp((vector2.z + vector2.y * 0.2f + angularVelocity.x + angularVelocity.y) * this.wobbleMax, -this.wobbleMax, this.wobbleMax);
			this.lastPos = position;
			this.lastRot = rotation;
			this.lastSineWave = d;
			this.lastVelocity = vector2;
			this.lastAngularVelocity = angularVelocity;
			this.meshRenderer.enabled = (!this.keepMeshHidden && !this.isEmpty);
			float x = transform.lossyScale.x;
			float num2 = this.localMeshBounds.extents.x * x;
			float y2 = this.localMeshBounds.extents.y;
			Vector3 position2 = this.localMeshBounds.center + new Vector3(0f, y2, 0f);
			this.cupTopWorldPos = transform.TransformPoint(position2);
			Vector3 up = transform.up;
			Vector3 rhs = transform.InverseTransformDirection(Vector3.down);
			float num3 = float.MinValue;
			Vector3 position3 = Vector3.zero;
			for (int i = 0; i < this.topVerts.Length; i++)
			{
				float num4 = Vector3.Dot(this.topVerts[i], rhs);
				if (num4 > num3)
				{
					num3 = num4;
					position3 = this.topVerts[i];
				}
			}
			this.bottomLipWorldPos = transform.TransformPoint(position3);
			float num5 = Mathf.Clamp01((this.liquidPlaneWorldPos.y - this.bottomLipWorldPos.y) / (num2 * 2f));
			bool flag = num5 > 1E-05f;
			ParticleSystem.EmissionModule emission = this.spillParticleSystem.emission;
			emission.enabled = flag;
			if (flag)
			{
				if (!this.spillSoundBankPlayer.isPlaying)
				{
					this.spillSoundBankPlayer.Play();
				}
				this.spillParticleSystem.transform.position = Vector3.Lerp(this.bottomLipWorldPos, this.cupTopWorldPos, num5);
				this.spillParticleSystem.shape.radius = num2 * num5;
				ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
				float num6 = num5 * this.maxSpillRate;
				rateOverTime.constant = num6;
				emission.rateOverTime = rateOverTime;
				this.fillAmount -= num6 * deltaTime * 0.01f;
			}
			if (this.isEmpty && !this.wasEmptyLastFrame && !this.emptySoundBankPlayer.isPlaying)
			{
				this.emptySoundBankPlayer.Play();
			}
			else if (!this.isEmpty && this.wasEmptyLastFrame && !this.refillSoundBankPlayer.isPlaying)
			{
				this.refillSoundBankPlayer.Play();
			}
			this.wasEmptyLastFrame = this.isEmpty;
		}

		// Token: 0x06005BF6 RID: 23542 RVA: 0x001CFB40 File Offset: 0x001CDD40
		public void UpdateRefillTimer()
		{
			if (this.refillDelay < 0f || !this.isEmpty)
			{
				return;
			}
			if (this.refillTimer < 0f)
			{
				this.refillTimer = this.refillDelay;
				this.fillAmount = this.refillAmount;
				return;
			}
			this.refillTimer -= Time.deltaTime;
		}

		// Token: 0x06005BF7 RID: 23543 RVA: 0x001CFB9C File Offset: 0x001CDD9C
		private Vector3[] GetTopVerts()
		{
			Vector3[] vertices = this.meshFilter.sharedMesh.vertices;
			List<Vector3> list = new List<Vector3>(vertices.Length);
			float num = float.MinValue;
			foreach (Vector3 vector in vertices)
			{
				if (vector.y > num)
				{
					num = vector.y;
				}
			}
			foreach (Vector3 vector2 in vertices)
			{
				if (Mathf.Abs(vector2.y - num) < 0.001f)
				{
					list.Add(vector2);
				}
			}
			return list.ToArray();
		}

		// Token: 0x0400659A RID: 26010
		[Tooltip("Used to determine the world space bounds of the container.")]
		public MeshRenderer meshRenderer;

		// Token: 0x0400659B RID: 26011
		[Tooltip("Used to determine the local space bounds of the container.")]
		public MeshFilter meshFilter;

		// Token: 0x0400659C RID: 26012
		[Tooltip("If you are only using the liquid mesh to calculate the volume of the container and do not need visuals then set this to true.")]
		public bool keepMeshHidden;

		// Token: 0x0400659D RID: 26013
		[Tooltip("The object that will float on top of the liquid.")]
		public Transform floater;

		// Token: 0x0400659E RID: 26014
		public bool useLiquidShader = true;

		// Token: 0x0400659F RID: 26015
		public bool useLiquidVolume;

		// Token: 0x040065A0 RID: 26016
		public Vector2 liquidVolumeMinMax = Vector2.up;

		// Token: 0x040065A1 RID: 26017
		public string liquidColorShaderPropertyName = "_BaseColor";

		// Token: 0x040065A2 RID: 26018
		public string liquidPlaneNormalShaderPropertyName = "_LiquidPlaneNormal";

		// Token: 0x040065A3 RID: 26019
		public string liquidPlanePositionShaderPropertyName = "_LiquidPlanePosition";

		// Token: 0x040065A4 RID: 26020
		[Tooltip("Emits drips when pouring.")]
		public ParticleSystem spillParticleSystem;

		// Token: 0x040065A5 RID: 26021
		[SoundBankInfo]
		public SoundBankPlayer emptySoundBankPlayer;

		// Token: 0x040065A6 RID: 26022
		[SoundBankInfo]
		public SoundBankPlayer refillSoundBankPlayer;

		// Token: 0x040065A7 RID: 26023
		[SoundBankInfo]
		public SoundBankPlayer spillSoundBankPlayer;

		// Token: 0x040065A8 RID: 26024
		public Color liquidColor = new Color(0.33f, 0.25f, 0.21f, 1f);

		// Token: 0x040065A9 RID: 26025
		[Tooltip("The amount of liquid currently in the container. This value is passed to the shader.")]
		[Range(0f, 1f)]
		public float fillAmount = 0.85f;

		// Token: 0x040065AA RID: 26026
		[Tooltip("This is what fillAmount will be after automatic refilling.")]
		public float refillAmount = 0.85f;

		// Token: 0x040065AB RID: 26027
		[Tooltip("Set to a negative value to disable.")]
		public float refillDelay = 10f;

		// Token: 0x040065AC RID: 26028
		[Tooltip("The point that the liquid should be considered empty and should be auto refilled.")]
		public float refillThreshold = 0.1f;

		// Token: 0x040065AD RID: 26029
		public float wobbleMax = 0.2f;

		// Token: 0x040065AE RID: 26030
		public float wobbleFrequency = 1f;

		// Token: 0x040065AF RID: 26031
		public float recovery = 1f;

		// Token: 0x040065B0 RID: 26032
		public float thickness = 1f;

		// Token: 0x040065B1 RID: 26033
		public float maxSpillRate = 100f;

		// Token: 0x040065B6 RID: 26038
		[DebugReadout]
		private bool wasEmptyLastFrame;

		// Token: 0x040065B7 RID: 26039
		private int liquidColorShaderProp;

		// Token: 0x040065B8 RID: 26040
		private int liquidPlaneNormalShaderProp;

		// Token: 0x040065B9 RID: 26041
		private int liquidPlanePositionShaderProp;

		// Token: 0x040065BA RID: 26042
		private float refillTimer;

		// Token: 0x040065BB RID: 26043
		private float lastSineWave;

		// Token: 0x040065BC RID: 26044
		private float lastWobble;

		// Token: 0x040065BD RID: 26045
		private Vector2 temporalWobbleAmp;

		// Token: 0x040065BE RID: 26046
		private Vector3 lastPos;

		// Token: 0x040065BF RID: 26047
		private Vector3 lastVelocity;

		// Token: 0x040065C0 RID: 26048
		private Vector3 lastAngularVelocity;

		// Token: 0x040065C1 RID: 26049
		private Quaternion lastRot;

		// Token: 0x040065C2 RID: 26050
		private MaterialPropertyBlock matPropBlock;

		// Token: 0x040065C3 RID: 26051
		private Bounds localMeshBounds;

		// Token: 0x040065C4 RID: 26052
		private bool useFloater;

		// Token: 0x040065C5 RID: 26053
		private Vector3[] topVerts;
	}
}
