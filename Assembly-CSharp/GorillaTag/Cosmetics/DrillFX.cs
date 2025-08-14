using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F2C RID: 3884
	public class DrillFX : MonoBehaviour
	{
		// Token: 0x0600604C RID: 24652 RVA: 0x001E989C File Offset: 0x001E7A9C
		protected void Awake()
		{
			if (!DrillFX.appIsQuittingHandlerIsSubscribed)
			{
				DrillFX.appIsQuittingHandlerIsSubscribed = true;
				Application.quitting += DrillFX.HandleApplicationQuitting;
			}
			this.hasFX = (this.fx != null);
			if (this.hasFX)
			{
				this.fxEmissionModule = this.fx.emission;
				this.fxEmissionMaxRate = this.fxEmissionModule.rateOverTimeMultiplier;
				this.fxShapeModule = this.fx.shape;
				this.fxShapeMaxRadius = this.fxShapeModule.radius;
			}
			this.hasAudio = (this.loopAudio != null);
			if (this.hasAudio)
			{
				this.audioMaxVolume = this.loopAudio.volume;
				this.loopAudio.volume = 0f;
				this.loopAudio.loop = true;
				this.loopAudio.GTPlay();
			}
		}

		// Token: 0x0600604D RID: 24653 RVA: 0x001E9978 File Offset: 0x001E7B78
		protected void OnEnable()
		{
			if (DrillFX.appIsQuitting)
			{
				return;
			}
			if (this.hasFX)
			{
				this.fxEmissionModule.rateOverTimeMultiplier = 0f;
			}
			if (this.hasAudio)
			{
				this.loopAudio.volume = 0f;
				this.loopAudio.loop = true;
				this.loopAudio.GTPlay();
			}
			this.ValidateLineCastPositions();
		}

		// Token: 0x0600604E RID: 24654 RVA: 0x001E99DC File Offset: 0x001E7BDC
		protected void OnDisable()
		{
			if (DrillFX.appIsQuitting)
			{
				return;
			}
			if (this.hasFX)
			{
				this.fxEmissionModule.rateOverTimeMultiplier = 0f;
			}
			if (this.hasAudio)
			{
				this.loopAudio.volume = 0f;
				this.loopAudio.GTStop();
			}
		}

		// Token: 0x0600604F RID: 24655 RVA: 0x001E9A2C File Offset: 0x001E7C2C
		protected void LateUpdate()
		{
			if (DrillFX.appIsQuitting)
			{
				return;
			}
			Transform transform = base.transform;
			RaycastHit raycastHit;
			Vector3 position = Physics.Linecast(transform.TransformPoint(this.lineCastStart), transform.TransformPoint(this.lineCastEnd), out raycastHit, this.lineCastLayerMask, QueryTriggerInteraction.Ignore) ? raycastHit.point : this.lineCastEnd;
			Vector3 vector = transform.InverseTransformPoint(position);
			float num = Mathf.Clamp01(Vector3.Distance(this.lineCastStart, vector) / this.maxDepth);
			if (this.hasFX)
			{
				this.fxEmissionModule.rateOverTimeMultiplier = this.fxEmissionMaxRate * this.fxEmissionCurve.Evaluate(num);
				this.fxShapeModule.position = vector;
				this.fxShapeModule.radius = Mathf.Lerp(this.fxShapeMaxRadius, this.fxMinRadiusScale * this.fxShapeMaxRadius, num);
			}
			if (this.hasAudio)
			{
				this.loopAudio.volume = Mathf.MoveTowards(this.loopAudio.volume, this.audioMaxVolume * this.loopAudioVolumeCurve.Evaluate(num), this.loopAudioVolumeTransitionSpeed * Time.deltaTime);
			}
		}

		// Token: 0x06006050 RID: 24656 RVA: 0x001E9B42 File Offset: 0x001E7D42
		private static void HandleApplicationQuitting()
		{
			DrillFX.appIsQuitting = true;
		}

		// Token: 0x06006051 RID: 24657 RVA: 0x001E9B4C File Offset: 0x001E7D4C
		private bool ValidateLineCastPositions()
		{
			this.maxDepth = Vector3.Distance(this.lineCastStart, this.lineCastEnd);
			if (this.maxDepth > 1E-45f)
			{
				return true;
			}
			if (Application.isPlaying)
			{
				Debug.Log("DrillFX: lineCastStart and End are too close together. Disabling component.", this);
				base.enabled = false;
			}
			return false;
		}

		// Token: 0x04006BA8 RID: 27560
		[SerializeField]
		private ParticleSystem fx;

		// Token: 0x04006BA9 RID: 27561
		[SerializeField]
		private AnimationCurve fxEmissionCurve;

		// Token: 0x04006BAA RID: 27562
		[SerializeField]
		private float fxMinRadiusScale = 0.01f;

		// Token: 0x04006BAB RID: 27563
		[Tooltip("Right click menu has custom menu items. Anything starting with \"- \" is custom.")]
		[SerializeField]
		private AudioSource loopAudio;

		// Token: 0x04006BAC RID: 27564
		[SerializeField]
		private AnimationCurve loopAudioVolumeCurve;

		// Token: 0x04006BAD RID: 27565
		[Tooltip("Higher value makes it reach the target volume faster.")]
		[SerializeField]
		private float loopAudioVolumeTransitionSpeed = 3f;

		// Token: 0x04006BAE RID: 27566
		[FormerlySerializedAs("layerMask")]
		[Tooltip("The collision layers the line cast should intersect with")]
		[SerializeField]
		private LayerMask lineCastLayerMask;

		// Token: 0x04006BAF RID: 27567
		[Tooltip("The position in local space that the line cast starts.")]
		[SerializeField]
		private Vector3 lineCastStart = Vector3.zero;

		// Token: 0x04006BB0 RID: 27568
		[Tooltip("The position in local space that the line cast ends.")]
		[SerializeField]
		private Vector3 lineCastEnd = Vector3.forward;

		// Token: 0x04006BB1 RID: 27569
		private static bool appIsQuitting;

		// Token: 0x04006BB2 RID: 27570
		private static bool appIsQuittingHandlerIsSubscribed;

		// Token: 0x04006BB3 RID: 27571
		private float maxDepth;

		// Token: 0x04006BB4 RID: 27572
		private bool hasFX;

		// Token: 0x04006BB5 RID: 27573
		private ParticleSystem.EmissionModule fxEmissionModule;

		// Token: 0x04006BB6 RID: 27574
		private float fxEmissionMaxRate;

		// Token: 0x04006BB7 RID: 27575
		private ParticleSystem.ShapeModule fxShapeModule;

		// Token: 0x04006BB8 RID: 27576
		private float fxShapeMaxRadius;

		// Token: 0x04006BB9 RID: 27577
		private bool hasAudio;

		// Token: 0x04006BBA RID: 27578
		private float audioMaxVolume;
	}
}
