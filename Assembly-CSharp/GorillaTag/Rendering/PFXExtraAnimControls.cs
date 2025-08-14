using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x02000EF8 RID: 3832
	public class PFXExtraAnimControls : MonoBehaviour
	{
		// Token: 0x06005F01 RID: 24321 RVA: 0x001DF034 File Offset: 0x001DD234
		protected void Awake()
		{
			this.emissionModules = new ParticleSystem.EmissionModule[this.particleSystems.Length];
			this.cachedEmitBursts = new ParticleSystem.Burst[this.particleSystems.Length][];
			this.adjustedEmitBursts = new ParticleSystem.Burst[this.particleSystems.Length][];
			for (int i = 0; i < this.particleSystems.Length; i++)
			{
				ParticleSystem.EmissionModule emission = this.particleSystems[i].emission;
				this.cachedEmitBursts[i] = new ParticleSystem.Burst[emission.burstCount];
				this.adjustedEmitBursts[i] = new ParticleSystem.Burst[emission.burstCount];
				for (int j = 0; j < emission.burstCount; j++)
				{
					this.cachedEmitBursts[i][j] = emission.GetBurst(j);
					this.adjustedEmitBursts[i][j] = emission.GetBurst(j);
				}
				this.emissionModules[i] = emission;
			}
		}

		// Token: 0x06005F02 RID: 24322 RVA: 0x001DF114 File Offset: 0x001DD314
		protected void LateUpdate()
		{
			for (int i = 0; i < this.emissionModules.Length; i++)
			{
				this.emissionModules[i].rateOverTimeMultiplier = this.emitRateMult;
				Mathf.Min(this.emissionModules[i].burstCount, this.cachedEmitBursts[i].Length);
				for (int j = 0; j < this.cachedEmitBursts[i].Length; j++)
				{
					this.adjustedEmitBursts[i][j].probability = this.cachedEmitBursts[i][j].probability * this.emitBurstProbabilityMult;
				}
				this.emissionModules[i].SetBursts(this.adjustedEmitBursts[i]);
			}
		}

		// Token: 0x04006963 RID: 26979
		public float emitRateMult = 1f;

		// Token: 0x04006964 RID: 26980
		public float emitBurstProbabilityMult = 1f;

		// Token: 0x04006965 RID: 26981
		[SerializeField]
		private ParticleSystem[] particleSystems;

		// Token: 0x04006966 RID: 26982
		private ParticleSystem.EmissionModule[] emissionModules;

		// Token: 0x04006967 RID: 26983
		private ParticleSystem.Burst[][] cachedEmitBursts;

		// Token: 0x04006968 RID: 26984
		private ParticleSystem.Burst[][] adjustedEmitBursts;
	}
}
