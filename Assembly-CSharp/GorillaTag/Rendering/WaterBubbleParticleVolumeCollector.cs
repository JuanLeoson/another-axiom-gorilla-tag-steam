using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GorillaTag.Rendering
{
	// Token: 0x02000EF3 RID: 3827
	public class WaterBubbleParticleVolumeCollector : MonoBehaviour
	{
		// Token: 0x06005EF1 RID: 24305 RVA: 0x001DEB6C File Offset: 0x001DCD6C
		protected void Awake()
		{
			List<WaterVolume> componentsInHierarchy = SceneManager.GetActiveScene().GetComponentsInHierarchy(true, 64);
			List<Collider> list = new List<Collider>(componentsInHierarchy.Count * 4);
			foreach (WaterVolume waterVolume in componentsInHierarchy)
			{
				if (!(waterVolume.Parameters != null) || waterVolume.Parameters.allowBubblesInVolume)
				{
					foreach (Collider collider in waterVolume.volumeColliders)
					{
						if (!(collider == null))
						{
							list.Add(collider);
						}
					}
				}
			}
			this.bubbleableVolumeColliders = list.ToArray();
			this.particleTriggerModules = new ParticleSystem.TriggerModule[this.particleSystems.Length];
			this.particleEmissionModules = new ParticleSystem.EmissionModule[this.particleSystems.Length];
			for (int i = 0; i < this.particleSystems.Length; i++)
			{
				this.particleTriggerModules[i] = this.particleSystems[i].trigger;
				this.particleEmissionModules[i] = this.particleSystems[i].emission;
			}
			for (int j = 0; j < this.particleSystems.Length; j++)
			{
				ParticleSystem.TriggerModule triggerModule = this.particleTriggerModules[j];
				for (int k = 0; k < list.Count; k++)
				{
					triggerModule.SetCollider(k, this.bubbleableVolumeColliders[k]);
				}
			}
			this.SetEmissionState(false);
		}

		// Token: 0x06005EF2 RID: 24306 RVA: 0x001DED0C File Offset: 0x001DCF0C
		protected void LateUpdate()
		{
			bool headInWater = GTPlayer.Instance.HeadInWater;
			if (headInWater && !this.emissionEnabled)
			{
				this.SetEmissionState(true);
				return;
			}
			if (!headInWater && this.emissionEnabled)
			{
				this.SetEmissionState(false);
			}
		}

		// Token: 0x06005EF3 RID: 24307 RVA: 0x001DED4C File Offset: 0x001DCF4C
		private void SetEmissionState(bool setEnabled)
		{
			float rateOverTimeMultiplier = setEnabled ? 1f : 0f;
			for (int i = 0; i < this.particleEmissionModules.Length; i++)
			{
				this.particleEmissionModules[i].rateOverTimeMultiplier = rateOverTimeMultiplier;
			}
			this.emissionEnabled = setEnabled;
		}

		// Token: 0x04006957 RID: 26967
		public ParticleSystem[] particleSystems;

		// Token: 0x04006958 RID: 26968
		private ParticleSystem.TriggerModule[] particleTriggerModules;

		// Token: 0x04006959 RID: 26969
		private ParticleSystem.EmissionModule[] particleEmissionModules;

		// Token: 0x0400695A RID: 26970
		private Collider[] bubbleableVolumeColliders;

		// Token: 0x0400695B RID: 26971
		private bool emissionEnabled;
	}
}
