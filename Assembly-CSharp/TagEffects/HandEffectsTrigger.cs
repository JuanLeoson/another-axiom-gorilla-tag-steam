using System;
using GorillaExtensions;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02000DF9 RID: 3577
	public class HandEffectsTrigger : MonoBehaviour, IHandEffectsTrigger
	{
		// Token: 0x1700087D RID: 2173
		// (get) Token: 0x06005895 RID: 22677 RVA: 0x001B8C1F File Offset: 0x001B6E1F
		public bool Static
		{
			get
			{
				return this.isStatic;
			}
		}

		// Token: 0x1700087E RID: 2174
		// (get) Token: 0x06005896 RID: 22678 RVA: 0x001B8C28 File Offset: 0x001B6E28
		public bool FingersDown
		{
			get
			{
				return !(this.rig == null) && ((this.rightHand && this.rig.IsMakingFistRight()) || (!this.rightHand && this.rig.IsMakingFistLeft()));
			}
		}

		// Token: 0x1700087F RID: 2175
		// (get) Token: 0x06005897 RID: 22679 RVA: 0x001B8C74 File Offset: 0x001B6E74
		public bool FingersUp
		{
			get
			{
				return !(this.rig == null) && ((this.rightHand && this.rig.IsMakingFiveRight()) || (!this.rightHand && this.rig.IsMakingFiveLeft()));
			}
		}

		// Token: 0x17000880 RID: 2176
		// (get) Token: 0x06005898 RID: 22680 RVA: 0x001B8CC0 File Offset: 0x001B6EC0
		public Vector3 Velocity
		{
			get
			{
				if (this.velocityEstimator != null && this.rig != null && this.rig.scaleFactor > 0.001f)
				{
					return this.velocityEstimator.linearVelocity / this.rig.scaleFactor;
				}
				return Vector3.zero;
			}
		}

		// Token: 0x17000881 RID: 2177
		// (get) Token: 0x06005899 RID: 22681 RVA: 0x001B8D1C File Offset: 0x001B6F1C
		bool IHandEffectsTrigger.RightHand
		{
			get
			{
				return this.rightHand;
			}
		}

		// Token: 0x17000882 RID: 2178
		// (get) Token: 0x0600589A RID: 22682 RVA: 0x001B8D24 File Offset: 0x001B6F24
		public IHandEffectsTrigger.Mode EffectMode { get; }

		// Token: 0x17000883 RID: 2179
		// (get) Token: 0x0600589B RID: 22683 RVA: 0x0005860D File Offset: 0x0005680D
		public Transform Transform
		{
			get
			{
				return base.transform;
			}
		}

		// Token: 0x17000884 RID: 2180
		// (get) Token: 0x0600589C RID: 22684 RVA: 0x001B8D2C File Offset: 0x001B6F2C
		public VRRig Rig
		{
			get
			{
				return this.rig;
			}
		}

		// Token: 0x17000885 RID: 2181
		// (get) Token: 0x0600589D RID: 22685 RVA: 0x001B8D34 File Offset: 0x001B6F34
		public TagEffectPack CosmeticEffectPack
		{
			get
			{
				if (this.rig == null)
				{
					return null;
				}
				return this.rig.CosmeticEffectPack;
			}
		}

		// Token: 0x0600589E RID: 22686 RVA: 0x001B8D54 File Offset: 0x001B6F54
		private void Awake()
		{
			this.rig = base.GetComponentInParent<VRRig>();
			if (this.velocityEstimator == null)
			{
				this.velocityEstimator = base.GetComponentInParent<GorillaVelocityEstimator>();
			}
			for (int i = 0; i < this.debugVisuals.Length; i++)
			{
				this.debugVisuals[i].SetActive(TagEffectsLibrary.DebugMode);
			}
		}

		// Token: 0x0600589F RID: 22687 RVA: 0x00058663 File Offset: 0x00056863
		private void OnEnable()
		{
			if (!HandEffectsTriggerRegistry.HasInstance)
			{
				HandEffectsTriggerRegistry.FindInstance();
			}
			HandEffectsTriggerRegistry.Instance.Register(this);
		}

		// Token: 0x060058A0 RID: 22688 RVA: 0x0005867C File Offset: 0x0005687C
		private void OnDisable()
		{
			HandEffectsTriggerRegistry.Instance.Unregister(this);
		}

		// Token: 0x060058A1 RID: 22689 RVA: 0x001B8DAC File Offset: 0x001B6FAC
		public void OnTriggerEntered(IHandEffectsTrigger other)
		{
			if (this.rig == other.Rig)
			{
				return;
			}
			if (this.FingersDown && other.FingersDown && (other.Static || (Vector3.Dot(Vector3.Dot(this.Velocity, base.transform.up) * base.transform.up - Vector3.Dot(other.Velocity, other.Transform.up) * other.Transform.up, -other.Transform.up) > TagEffectsLibrary.FistBumpSpeedThreshold && Vector3.Dot(base.transform.up, other.Transform.up) < -0.01f)))
			{
				this.PlayHandEffects(TagEffectsLibrary.EffectType.FIST_BUMP, other);
			}
			if (this.FingersUp && other.FingersUp && (other.Static || Mathf.Abs(Vector3.Dot(Vector3.Dot(this.Velocity, base.transform.right) * base.transform.right - Vector3.Dot(other.Velocity, other.Transform.right) * other.Transform.right, other.Transform.right)) > TagEffectsLibrary.HighFiveSpeedThreshold))
			{
				this.PlayHandEffects(TagEffectsLibrary.EffectType.HIGH_FIVE, other);
			}
		}

		// Token: 0x060058A2 RID: 22690 RVA: 0x001B8F18 File Offset: 0x001B7118
		private void PlayHandEffects(TagEffectsLibrary.EffectType effectType, IHandEffectsTrigger other)
		{
			if (this.rig.IsNull())
			{
				return;
			}
			bool flag = false;
			if (this.rig.isOfflineVRRig)
			{
				PlayerGameEvents.TriggerHandEffect(effectType.ToString());
			}
			HandEffectsOverrideCosmetic handEffectsOverrideCosmetic = null;
			HandEffectsOverrideCosmetic handEffectsOverrideCosmetic2 = null;
			foreach (HandEffectsOverrideCosmetic handEffectsOverrideCosmetic3 in (this.rightHand ? this.rig.CosmeticHandEffectsOverride_Right : this.rig.CosmeticHandEffectsOverride_Left))
			{
				if (handEffectsOverrideCosmetic3.handEffectType == this.MapEnum(effectType))
				{
					handEffectsOverrideCosmetic2 = handEffectsOverrideCosmetic3;
					break;
				}
			}
			if (this.rig.isOfflineVRRig && GorillaTagger.Instance != null)
			{
				if (other.Rig)
				{
					foreach (HandEffectsOverrideCosmetic handEffectsOverrideCosmetic4 in ((other.Rig.CosmeticHandEffectsOverride_Right != null) ? other.Rig.CosmeticHandEffectsOverride_Right : other.Rig.CosmeticHandEffectsOverride_Left))
					{
						if (handEffectsOverrideCosmetic4.handEffectType == this.MapEnum(effectType))
						{
							handEffectsOverrideCosmetic = handEffectsOverrideCosmetic4;
							break;
						}
					}
					if (handEffectsOverrideCosmetic && handEffectsOverrideCosmetic.handEffectType == this.MapEnum(effectType) && ((!handEffectsOverrideCosmetic.isLeftHand && other.RightHand) || (handEffectsOverrideCosmetic.isLeftHand && !other.RightHand)))
					{
						if (handEffectsOverrideCosmetic.thirdPerson.playHaptics)
						{
							GorillaTagger.Instance.StartVibration(!this.rightHand, handEffectsOverrideCosmetic.thirdPerson.hapticStrength, handEffectsOverrideCosmetic.thirdPerson.hapticDuration);
						}
						TagEffectsLibrary.placeEffects(handEffectsOverrideCosmetic.thirdPerson.effectVFX, base.transform, this.rig.scaleFactor, false, handEffectsOverrideCosmetic.thirdPerson.parentEffect, base.transform.rotation);
						flag = true;
					}
				}
				if (handEffectsOverrideCosmetic2 && handEffectsOverrideCosmetic2.handEffectType == this.MapEnum(effectType) && ((handEffectsOverrideCosmetic2.isLeftHand && !this.rightHand) || (!handEffectsOverrideCosmetic2.isLeftHand && this.rightHand)))
				{
					if (handEffectsOverrideCosmetic2.firstPerson.playHaptics)
					{
						GorillaTagger.Instance.StartVibration(!this.rightHand, handEffectsOverrideCosmetic2.firstPerson.hapticStrength, handEffectsOverrideCosmetic2.firstPerson.hapticDuration);
					}
					TagEffectsLibrary.placeEffects(handEffectsOverrideCosmetic2.firstPerson.effectVFX, other.Transform, this.rig.scaleFactor, false, handEffectsOverrideCosmetic2.firstPerson.parentEffect, other.Transform.rotation);
					flag = true;
				}
			}
			if (!flag)
			{
				if (this.rig.isOfflineVRRig)
				{
					GorillaTagger.Instance.StartVibration(!this.rightHand, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
				}
				TagEffectsLibrary.PlayEffect(base.transform, !this.rightHand, this.rig.scaleFactor, effectType, this.CosmeticEffectPack, other.CosmeticEffectPack, base.transform.rotation);
			}
		}

		// Token: 0x060058A3 RID: 22691 RVA: 0x001B9240 File Offset: 0x001B7440
		public bool InTriggerZone(IHandEffectsTrigger t)
		{
			return (base.transform.position - t.Transform.position).IsShorterThan(this.triggerRadius * this.rig.scaleFactor);
		}

		// Token: 0x060058A4 RID: 22692 RVA: 0x001B9274 File Offset: 0x001B7474
		private HandEffectsOverrideCosmetic.HandEffectType MapEnum(TagEffectsLibrary.EffectType oldEnum)
		{
			return HandEffectsTrigger.mappingArray[(int)oldEnum];
		}

		// Token: 0x0400626A RID: 25194
		[SerializeField]
		private float triggerRadius = 0.07f;

		// Token: 0x0400626B RID: 25195
		[SerializeField]
		private bool rightHand;

		// Token: 0x0400626C RID: 25196
		[SerializeField]
		private bool isStatic;

		// Token: 0x0400626D RID: 25197
		private VRRig rig;

		// Token: 0x0400626E RID: 25198
		public GorillaVelocityEstimator velocityEstimator;

		// Token: 0x0400626F RID: 25199
		[SerializeField]
		private GameObject[] debugVisuals;

		// Token: 0x04006271 RID: 25201
		private static HandEffectsOverrideCosmetic.HandEffectType[] mappingArray = new HandEffectsOverrideCosmetic.HandEffectType[]
		{
			HandEffectsOverrideCosmetic.HandEffectType.None,
			HandEffectsOverrideCosmetic.HandEffectType.None,
			HandEffectsOverrideCosmetic.HandEffectType.HighFive,
			HandEffectsOverrideCosmetic.HandEffectType.FistBump
		};
	}
}
