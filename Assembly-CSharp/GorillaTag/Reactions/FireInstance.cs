using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Reactions
{
	// Token: 0x02000EA2 RID: 3746
	public class FireInstance : MonoBehaviour
	{
		// Token: 0x06005DB0 RID: 23984 RVA: 0x001D8895 File Offset: 0x001D6A95
		protected void Awake()
		{
			FireManager.Register(this);
		}

		// Token: 0x06005DB1 RID: 23985 RVA: 0x001D889D File Offset: 0x001D6A9D
		protected void OnDestroy()
		{
			FireManager.Unregister(this);
		}

		// Token: 0x06005DB2 RID: 23986 RVA: 0x001D88A5 File Offset: 0x001D6AA5
		protected void OnEnable()
		{
			FireManager.OnEnable(this);
		}

		// Token: 0x06005DB3 RID: 23987 RVA: 0x001D88AD File Offset: 0x001D6AAD
		protected void OnDisable()
		{
			FireManager.OnDisable(this);
		}

		// Token: 0x06005DB4 RID: 23988 RVA: 0x001D88B5 File Offset: 0x001D6AB5
		protected void OnTriggerEnter(Collider other)
		{
			FireManager.OnTriggerEnter(this, other);
		}

		// Token: 0x04006779 RID: 26489
		[Header("Scene References")]
		[Tooltip("If not assigned it will try to auto assign to a component on the same GameObject.")]
		[SerializeField]
		internal Collider _collider;

		// Token: 0x0400677A RID: 26490
		[Tooltip("If not assigned it will try to auto assign to a component on the same GameObject.")]
		[FormerlySerializedAs("_thermalSourceVolume")]
		[SerializeField]
		internal ThermalSourceVolume _thermalVolume;

		// Token: 0x0400677B RID: 26491
		[SerializeField]
		internal ParticleSystem _particleSystem;

		// Token: 0x0400677C RID: 26492
		[FormerlySerializedAs("_audioSource")]
		[SerializeField]
		internal AudioSource _loopingAudioSource;

		// Token: 0x0400677D RID: 26493
		[Tooltip("The emissive color will be darkened on the materials of these renderers as the fire is extinguished.")]
		[SerializeField]
		internal Renderer[] _emissiveRenderers;

		// Token: 0x0400677E RID: 26494
		[Header("Asset References")]
		[SerializeField]
		internal GTDirectAssetRef<AudioClip> _extinguishSound;

		// Token: 0x0400677F RID: 26495
		[SerializeField]
		internal float _extinguishSoundVolume = 1f;

		// Token: 0x04006780 RID: 26496
		[SerializeField]
		internal GTDirectAssetRef<AudioClip> _igniteSound;

		// Token: 0x04006781 RID: 26497
		[SerializeField]
		internal float _igniteSoundVolume = 1f;

		// Token: 0x04006782 RID: 26498
		[Header("Values")]
		[SerializeField]
		internal bool _despawnOnExtinguish = true;

		// Token: 0x04006783 RID: 26499
		[SerializeField]
		internal float _maxLifetime = 10f;

		// Token: 0x04006784 RID: 26500
		[Tooltip("How long it should take to reheat to it's default temperature.")]
		[SerializeField]
		internal float _reheatSpeed = 1f;

		// Token: 0x04006785 RID: 26501
		[Tooltip("If you completely extinguish the object, how long should it stay extinguished?")]
		[SerializeField]
		internal float _stayExtinguishedDuration = 1f;

		// Token: 0x04006786 RID: 26502
		internal float _defaultTemperature;

		// Token: 0x04006787 RID: 26503
		internal float _timeSinceExtinguished;

		// Token: 0x04006788 RID: 26504
		internal float _timeSinceDyingStart;

		// Token: 0x04006789 RID: 26505
		internal float _timeAlive;

		// Token: 0x0400678A RID: 26506
		internal float _psDefaultEmissionRate;

		// Token: 0x0400678B RID: 26507
		internal ParticleSystem.EmissionModule _psEmissionModule;

		// Token: 0x0400678C RID: 26508
		internal Vector3Int _spatialGridPosition;

		// Token: 0x0400678D RID: 26509
		internal bool _isDespawning;

		// Token: 0x0400678E RID: 26510
		internal float _deathStateDuration;

		// Token: 0x0400678F RID: 26511
		internal MaterialPropertyBlock[] _emiRenderers_matPropBlocks;

		// Token: 0x04006790 RID: 26512
		internal Color[] _emiRenderers_defaultColors;
	}
}
