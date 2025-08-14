using System;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000C9D RID: 3229
	public class BuilderScaleParticles : MonoBehaviour
	{
		// Token: 0x0600501D RID: 20509 RVA: 0x0018F420 File Offset: 0x0018D620
		private void OnEnable()
		{
			if (this.useLossyScale)
			{
				this.setScaleNextFrame = true;
				this.enableFrame = Time.frameCount;
			}
		}

		// Token: 0x0600501E RID: 20510 RVA: 0x0018F43C File Offset: 0x0018D63C
		private void LateUpdate()
		{
			if (this.setScaleNextFrame && Time.frameCount > this.enableFrame)
			{
				if (this.useLossyScale)
				{
					this.SetScale(base.transform.lossyScale.x);
				}
				this.setScaleNextFrame = false;
			}
		}

		// Token: 0x0600501F RID: 20511 RVA: 0x0018F478 File Offset: 0x0018D678
		private void OnDisable()
		{
			if (this.useLossyScale)
			{
				this.RevertScale();
			}
		}

		// Token: 0x06005020 RID: 20512 RVA: 0x0018F488 File Offset: 0x0018D688
		public void SetScale(float inScale)
		{
			bool isPlaying = this.system.isPlaying;
			if (isPlaying)
			{
				this.system.Stop();
				this.system.Clear();
			}
			if (Mathf.Approximately(inScale, this.scale))
			{
				if (this.autoPlay || isPlaying)
				{
					this.system.Play(true);
				}
				return;
			}
			this.scale = inScale;
			this.RevertScale();
			if (Mathf.Approximately(this.scale, 1f))
			{
				if (this.autoPlay || isPlaying)
				{
					this.system.Play(true);
				}
				return;
			}
			ParticleSystem.MainModule main = this.system.main;
			this.gravityMod = main.gravityModifierMultiplier;
			main.gravityModifierMultiplier = this.gravityMod * this.scale;
			if (main.startSize3D)
			{
				ParticleSystem.MinMaxCurve startSizeX = main.startSizeX;
				this.sizeCurveXCache = main.startSizeX;
				this.ScaleCurve(ref startSizeX, this.scale);
				main.startSizeX = startSizeX;
				ParticleSystem.MinMaxCurve startSizeY = main.startSizeY;
				this.sizeCurveYCache = main.startSizeY;
				this.ScaleCurve(ref startSizeY, this.scale);
				main.startSizeY = startSizeY;
				ParticleSystem.MinMaxCurve startSizeZ = main.startSizeZ;
				this.sizeCurveZCache = main.startSizeZ;
				this.ScaleCurve(ref startSizeZ, this.scale);
				main.startSizeZ = startSizeZ;
			}
			else
			{
				ParticleSystem.MinMaxCurve startSize = main.startSize;
				this.sizeCurveCache = main.startSize;
				this.ScaleCurve(ref startSize, this.scale);
				main.startSize = startSize;
			}
			ParticleSystem.MinMaxCurve startSpeed = main.startSpeed;
			this.speedCurveCache = main.startSpeed;
			this.ScaleCurve(ref startSpeed, this.scale);
			main.startSpeed = startSpeed;
			if (this.scaleShape)
			{
				ParticleSystem.ShapeModule shape = this.system.shape;
				this.shapeScale = shape.scale;
				shape.scale = this.shapeScale * this.scale;
			}
			if (this.scaleVelocityLifetime)
			{
				ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = this.system.velocityOverLifetime;
				this.lifetimeVelocityX = velocityOverLifetime.x;
				this.lifetimeVelocityY = velocityOverLifetime.y;
				this.lifetimeVelocityZ = velocityOverLifetime.z;
				ParticleSystem.MinMaxCurve minMaxCurve = velocityOverLifetime.x;
				this.ScaleCurve(ref minMaxCurve, this.scale);
				velocityOverLifetime.x = minMaxCurve;
				minMaxCurve = velocityOverLifetime.y;
				this.ScaleCurve(ref minMaxCurve, this.scale);
				velocityOverLifetime.y = minMaxCurve;
				minMaxCurve = velocityOverLifetime.z;
				this.ScaleCurve(ref minMaxCurve, this.scale);
				velocityOverLifetime.z = minMaxCurve;
			}
			if (this.scaleVelocityLimitLifetime)
			{
				ParticleSystem.LimitVelocityOverLifetimeModule limitVelocityOverLifetime = this.system.limitVelocityOverLifetime;
				this.limitMultiplier = limitVelocityOverLifetime.limitMultiplier;
				limitVelocityOverLifetime.limitMultiplier = this.limitMultiplier * this.scale;
			}
			if (this.scaleForceOverLife)
			{
				ParticleSystem.ForceOverLifetimeModule forceOverLifetime = this.system.forceOverLifetime;
				this.forceX = forceOverLifetime.x;
				this.forceY = forceOverLifetime.y;
				this.forceZ = forceOverLifetime.z;
				ParticleSystem.MinMaxCurve minMaxCurve2 = forceOverLifetime.x;
				this.ScaleCurve(ref minMaxCurve2, this.scale);
				forceOverLifetime.x = minMaxCurve2;
				minMaxCurve2 = forceOverLifetime.y;
				this.ScaleCurve(ref minMaxCurve2, this.scale);
				forceOverLifetime.y = minMaxCurve2;
				minMaxCurve2 = forceOverLifetime.z;
				this.ScaleCurve(ref minMaxCurve2, this.scale);
				forceOverLifetime.z = minMaxCurve2;
			}
			if (this.autoPlay || isPlaying)
			{
				this.system.Play(true);
			}
			this.shouldRevert = true;
		}

		// Token: 0x06005021 RID: 20513 RVA: 0x0018F7F8 File Offset: 0x0018D9F8
		private void ScaleCurve(ref ParticleSystem.MinMaxCurve curve, float scale)
		{
			switch (curve.mode)
			{
			case ParticleSystemCurveMode.Constant:
				curve.constant *= scale;
				return;
			case ParticleSystemCurveMode.Curve:
			case ParticleSystemCurveMode.TwoCurves:
				curve.curveMultiplier *= scale;
				return;
			case ParticleSystemCurveMode.TwoConstants:
				curve.constantMin *= scale;
				curve.constantMax *= scale;
				return;
			default:
				return;
			}
		}

		// Token: 0x06005022 RID: 20514 RVA: 0x0018F860 File Offset: 0x0018DA60
		public void RevertScale()
		{
			if (!this.shouldRevert)
			{
				return;
			}
			ParticleSystem.MainModule main = this.system.main;
			main.gravityModifierMultiplier = this.gravityMod;
			main.startSpeed = this.speedCurveCache;
			if (main.startSize3D)
			{
				main.startSizeX = this.sizeCurveXCache;
				main.startSizeY = this.sizeCurveYCache;
				main.startSizeZ = this.sizeCurveZCache;
			}
			else
			{
				main.startSize = this.sizeCurveCache;
			}
			if (this.scaleShape)
			{
				this.system.shape.scale = this.shapeScale;
			}
			if (this.scaleVelocityLifetime)
			{
				ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = this.system.velocityOverLifetime;
				velocityOverLifetime.x = this.lifetimeVelocityX;
				velocityOverLifetime.y = this.lifetimeVelocityY;
				velocityOverLifetime.z = this.lifetimeVelocityZ;
			}
			if (this.scaleVelocityLimitLifetime)
			{
				this.system.limitVelocityOverLifetime.limitMultiplier = this.limitMultiplier;
			}
			if (this.scaleForceOverLife)
			{
				ParticleSystem.ForceOverLifetimeModule forceOverLifetime = this.system.forceOverLifetime;
				forceOverLifetime.x = this.forceX;
				forceOverLifetime.y = this.forceY;
				forceOverLifetime.z = this.forceZ;
			}
			this.scale = 1f;
			this.shouldRevert = false;
		}

		// Token: 0x04005936 RID: 22838
		private float scale = 1f;

		// Token: 0x04005937 RID: 22839
		[Tooltip("Scale particles on enable using lossy scale")]
		[SerializeField]
		private bool useLossyScale;

		// Token: 0x04005938 RID: 22840
		[Tooltip("Play particles after scaling")]
		[SerializeField]
		private bool autoPlay;

		// Token: 0x04005939 RID: 22841
		[SerializeField]
		private ParticleSystem system;

		// Token: 0x0400593A RID: 22842
		[SerializeField]
		private bool scaleShape;

		// Token: 0x0400593B RID: 22843
		[SerializeField]
		private bool scaleVelocityLifetime;

		// Token: 0x0400593C RID: 22844
		[SerializeField]
		private bool scaleVelocityLimitLifetime;

		// Token: 0x0400593D RID: 22845
		[SerializeField]
		private bool scaleForceOverLife;

		// Token: 0x0400593E RID: 22846
		private float gravityMod = 1f;

		// Token: 0x0400593F RID: 22847
		private ParticleSystem.MinMaxCurve speedCurveCache;

		// Token: 0x04005940 RID: 22848
		private ParticleSystem.MinMaxCurve sizeCurveCache;

		// Token: 0x04005941 RID: 22849
		private ParticleSystem.MinMaxCurve sizeCurveXCache;

		// Token: 0x04005942 RID: 22850
		private ParticleSystem.MinMaxCurve sizeCurveYCache;

		// Token: 0x04005943 RID: 22851
		private ParticleSystem.MinMaxCurve sizeCurveZCache;

		// Token: 0x04005944 RID: 22852
		private ParticleSystem.MinMaxCurve forceX;

		// Token: 0x04005945 RID: 22853
		private ParticleSystem.MinMaxCurve forceY;

		// Token: 0x04005946 RID: 22854
		private ParticleSystem.MinMaxCurve forceZ;

		// Token: 0x04005947 RID: 22855
		private Vector3 shapeScale = Vector3.one;

		// Token: 0x04005948 RID: 22856
		private ParticleSystem.MinMaxCurve lifetimeVelocityX;

		// Token: 0x04005949 RID: 22857
		private ParticleSystem.MinMaxCurve lifetimeVelocityY;

		// Token: 0x0400594A RID: 22858
		private ParticleSystem.MinMaxCurve lifetimeVelocityZ;

		// Token: 0x0400594B RID: 22859
		private float limitMultiplier = 1f;

		// Token: 0x0400594C RID: 22860
		private bool shouldRevert;

		// Token: 0x0400594D RID: 22861
		private bool setScaleNextFrame;

		// Token: 0x0400594E RID: 22862
		private int enableFrame;
	}
}
