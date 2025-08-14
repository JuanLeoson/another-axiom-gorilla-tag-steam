using System;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F1C RID: 3868
	public class CosmeticEffectsOnPlayers : MonoBehaviour, ISpawnable
	{
		// Token: 0x06005FC9 RID: 24521 RVA: 0x001E68A4 File Offset: 0x001E4AA4
		private bool ShouldAffectRig(VRRig rig, CosmeticEffectsOnPlayers.TargetType target)
		{
			bool flag = rig == this.myRig;
			bool result;
			switch (target)
			{
			case CosmeticEffectsOnPlayers.TargetType.Owner:
				result = flag;
				break;
			case CosmeticEffectsOnPlayers.TargetType.Others:
				result = !flag;
				break;
			case CosmeticEffectsOnPlayers.TargetType.All:
				result = true;
				break;
			default:
				result = false;
				break;
			}
			return result;
		}

		// Token: 0x06005FCA RID: 24522 RVA: 0x001E68E4 File Offset: 0x001E4AE4
		private void Awake()
		{
			foreach (CosmeticEffectsOnPlayers.CosmeticEffect cosmeticEffect in this.allEffects)
			{
				this.allEffectsDict.TryAdd(cosmeticEffect.effectType, cosmeticEffect);
			}
			this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
		}

		// Token: 0x06005FCB RID: 24523 RVA: 0x001E692C File Offset: 0x001E4B2C
		public void SetKnockbackStrengthMultiplier(float value)
		{
			foreach (KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> keyValuePair in this.allEffectsDict)
			{
				keyValuePair.Value.knockbackStrengthMultiplier = value;
			}
		}

		// Token: 0x06005FCC RID: 24524 RVA: 0x001E6988 File Offset: 0x001E4B88
		public void ApplyAllEffects()
		{
			this.ApplyAllEffectsByDistance(base.transform.position);
		}

		// Token: 0x06005FCD RID: 24525 RVA: 0x001E699B File Offset: 0x001E4B9B
		public void ApplyAllEffectsByDistance(Transform _transform)
		{
			this.ApplyAllEffectsByDistance(_transform.position);
		}

		// Token: 0x06005FCE RID: 24526 RVA: 0x001E69AC File Offset: 0x001E4BAC
		public void ApplyAllEffectsByDistance(Vector3 position)
		{
			foreach (KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect in this.allEffectsDict)
			{
				switch (effect.Key)
				{
				case CosmeticEffectsOnPlayers.EFFECTTYPE.Skin:
					this.ApplySkinByDistance(effect, position);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.FPV:
					this.ApplyFPVEffectsByDistance(effect, position);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback:
					this.ApplyTagWithKnockbackByDistance(effect, position);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.InstantKnockback:
					this.ApplyInstantKnockbackByDistance(effect, position);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.SFX:
					this.PlaySfxByDistance(effect, position);
					break;
				}
			}
		}

		// Token: 0x06005FCF RID: 24527 RVA: 0x001E6A50 File Offset: 0x001E4C50
		public void ApplyAllEffectsForRig(VRRig rig)
		{
			foreach (KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect in this.allEffectsDict)
			{
				switch (effect.Key)
				{
				case CosmeticEffectsOnPlayers.EFFECTTYPE.Skin:
					this.ApplySkinForRig(effect, rig);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.FPV:
					this.ApplyFPVEffectsForRig(effect, rig);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback:
					this.ApplyTagWithKnockbackForRig(effect, rig);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.InstantKnockback:
					this.ApplyInstantKnockbackForRig(effect, rig);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.VoiceOverride:
					this.ApplyVOForRig(effect, rig);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.SFX:
					this.PlaySfxForRig(effect, rig);
					break;
				}
			}
		}

		// Token: 0x06005FD0 RID: 24528 RVA: 0x001E6B00 File Offset: 0x001E4D00
		private void ApplyFPVEffectsByDistance(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, Vector3 position)
		{
			if (!effect.Value.IsGameModeAllowed())
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			IEnumerable<VRRig> enumerable2;
			if (!PhotonNetwork.InRoom)
			{
				IEnumerable<VRRig> enumerable = new VRRig[]
				{
					GorillaTagger.Instance.offlineVRRig
				};
				enumerable2 = enumerable;
			}
			else
			{
				IEnumerable<VRRig> enumerable = GorillaParent.instance.vrrigs;
				enumerable2 = enumerable;
			}
			foreach (VRRig vrrig in enumerable2)
			{
				if (this.ShouldAffectRig(vrrig, effect.Value.target) && (vrrig.transform.position - position).IsShorterThan(effect.Value.effectDistanceRadius))
				{
					if (vrrig == this.myRig)
					{
						effect.Value.EffectDuration = effect.Value.effectDurationOwner;
					}
					vrrig.SpawnFPVEffects(effect, true);
				}
			}
		}

		// Token: 0x06005FD1 RID: 24529 RVA: 0x001E6BF4 File Offset: 0x001E4DF4
		private void ApplyFPVEffectsForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig rig)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(rig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (rig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			rig.SpawnFPVEffects(effect, true);
		}

		// Token: 0x06005FD2 RID: 24530 RVA: 0x001E6C64 File Offset: 0x001E4E64
		private void ApplySkinByDistance(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, Vector3 position)
		{
			if (!effect.Value.IsGameModeAllowed())
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			IEnumerable<VRRig> enumerable2;
			if (!PhotonNetwork.InRoom)
			{
				IEnumerable<VRRig> enumerable = new VRRig[]
				{
					GorillaTagger.Instance.offlineVRRig
				};
				enumerable2 = enumerable;
			}
			else
			{
				IEnumerable<VRRig> enumerable = GorillaParent.instance.vrrigs;
				enumerable2 = enumerable;
			}
			foreach (VRRig vrrig in enumerable2)
			{
				if (this.ShouldAffectRig(vrrig, effect.Value.target) && (vrrig.transform.position - position).IsShorterThan(effect.Value.effectDistanceRadius))
				{
					if (vrrig == this.myRig)
					{
						effect.Value.EffectDuration = effect.Value.effectDurationOwner;
					}
					vrrig.SpawnSkinEffects(effect);
				}
			}
		}

		// Token: 0x06005FD3 RID: 24531 RVA: 0x001E6D58 File Offset: 0x001E4F58
		private void ApplySkinForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig vrRig)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(vrRig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (vrRig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			vrRig.SpawnSkinEffects(effect);
		}

		// Token: 0x06005FD4 RID: 24532 RVA: 0x001E6DC8 File Offset: 0x001E4FC8
		private void ApplyTagWithKnockbackForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig vrRig)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(vrRig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (vrRig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			vrRig.EnableHitWithKnockBack(effect);
		}

		// Token: 0x06005FD5 RID: 24533 RVA: 0x001E6E38 File Offset: 0x001E5038
		private void ApplyTagWithKnockbackByDistance(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, Vector3 position)
		{
			if (!effect.Value.IsGameModeAllowed())
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			IEnumerable<VRRig> enumerable2;
			if (!PhotonNetwork.InRoom)
			{
				IEnumerable<VRRig> enumerable = new VRRig[]
				{
					GorillaTagger.Instance.offlineVRRig
				};
				enumerable2 = enumerable;
			}
			else
			{
				IEnumerable<VRRig> enumerable = GorillaParent.instance.vrrigs;
				enumerable2 = enumerable;
			}
			foreach (VRRig vrrig in enumerable2)
			{
				if (this.ShouldAffectRig(vrrig, effect.Value.target) && (vrrig.transform.position - position).IsShorterThan(effect.Value.effectDistanceRadius))
				{
					if (vrrig == this.myRig)
					{
						effect.Value.EffectDuration = effect.Value.effectDurationOwner;
					}
					vrrig.EnableHitWithKnockBack(effect);
				}
			}
		}

		// Token: 0x06005FD6 RID: 24534 RVA: 0x001E6F2C File Offset: 0x001E512C
		private void ApplyInstantKnockbackForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig vrRig)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(vrRig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (vrRig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			Vector3 vector = vrRig.transform.position - base.transform.position;
			float strength = (1f / vector.magnitude * effect.Value.knockbackStrength * effect.Value.knockbackStrengthMultiplier).ClampSafe(effect.Value.minKnockbackStrength, effect.Value.maxKnockbackStrength);
			RoomSystem.HitPlayer(vrRig.creator, vector.normalized, strength);
			vrRig.ApplyInstanceKnockBack(effect);
		}

		// Token: 0x06005FD7 RID: 24535 RVA: 0x001E7010 File Offset: 0x001E5210
		private void ApplyInstantKnockbackByDistance(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, Vector3 position)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(GorillaTagger.Instance.offlineVRRig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (GorillaTagger.Instance.offlineVRRig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			Vector3 vector = GorillaTagger.Instance.offlineVRRig.transform.position - position;
			if (vector.IsShorterThan(effect.Value.effectDistanceRadius))
			{
				float magnitude = vector.magnitude;
				GTPlayer instance = GTPlayer.Instance;
				if (effect.Value.specialVerticalForce && (instance.wasLeftHandColliding || instance.wasRightHandColliding || instance.BodyOnGround))
				{
					Vector3 vector2 = -Physics.gravity.normalized;
					Vector3 vector3 = Vector3.ProjectOnPlane(vector, vector2);
					vector = ((Vector3.Dot(vector / magnitude, vector2) > 0f) ? vector : vector3) + vector3.magnitude * vector2;
				}
				float speed = (effect.Value.knockbackStrength * effect.Value.knockbackStrengthMultiplier / magnitude).ClampSafe(effect.Value.minKnockbackStrength, effect.Value.maxKnockbackStrength);
				instance.ApplyKnockback(vector.normalized, speed, effect.Value.forceOffTheGround);
				GorillaTagger.Instance.offlineVRRig.ApplyInstanceKnockBack(effect);
			}
		}

		// Token: 0x06005FD8 RID: 24536 RVA: 0x001E71A0 File Offset: 0x001E53A0
		private void ApplyVOForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig rig)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(rig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (rig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			rig.ActivateVOEffect(effect);
		}

		// Token: 0x06005FD9 RID: 24537 RVA: 0x001E7210 File Offset: 0x001E5410
		private void PlaySfxForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig vrRig)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(vrRig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (vrRig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			vrRig.PlayCosmeticEffectSFX(effect);
		}

		// Token: 0x06005FDA RID: 24538 RVA: 0x001E7280 File Offset: 0x001E5480
		private void PlaySfxByDistance(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, Vector3 position)
		{
			if (!effect.Value.IsGameModeAllowed())
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			IEnumerable<VRRig> enumerable2;
			if (!PhotonNetwork.InRoom)
			{
				IEnumerable<VRRig> enumerable = new VRRig[]
				{
					GorillaTagger.Instance.offlineVRRig
				};
				enumerable2 = enumerable;
			}
			else
			{
				IEnumerable<VRRig> enumerable = GorillaParent.instance.vrrigs;
				enumerable2 = enumerable;
			}
			foreach (VRRig vrrig in enumerable2)
			{
				if (this.ShouldAffectRig(vrrig, effect.Value.target) && (vrrig.transform.position - position).IsShorterThan(effect.Value.effectDistanceRadius))
				{
					if (vrrig == this.myRig)
					{
						effect.Value.EffectDuration = effect.Value.effectDurationOwner;
					}
					vrrig.PlayCosmeticEffectSFX(effect);
				}
			}
		}

		// Token: 0x17000946 RID: 2374
		// (get) Token: 0x06005FDB RID: 24539 RVA: 0x001E7374 File Offset: 0x001E5574
		// (set) Token: 0x06005FDC RID: 24540 RVA: 0x001E737C File Offset: 0x001E557C
		public bool IsSpawned { get; set; }

		// Token: 0x17000947 RID: 2375
		// (get) Token: 0x06005FDD RID: 24541 RVA: 0x001E7385 File Offset: 0x001E5585
		// (set) Token: 0x06005FDE RID: 24542 RVA: 0x001E738D File Offset: 0x001E558D
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x06005FDF RID: 24543 RVA: 0x001E7396 File Offset: 0x001E5596
		public void OnSpawn(VRRig rig)
		{
			this.myRig = rig;
		}

		// Token: 0x06005FE0 RID: 24544 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnDespawn()
		{
		}

		// Token: 0x04006B10 RID: 27408
		public CosmeticEffectsOnPlayers.CosmeticEffect[] allEffects = new CosmeticEffectsOnPlayers.CosmeticEffect[0];

		// Token: 0x04006B11 RID: 27409
		private VRRig myRig;

		// Token: 0x04006B12 RID: 27410
		private TransferrableObject parentTransferable;

		// Token: 0x04006B13 RID: 27411
		private Dictionary<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> allEffectsDict = new Dictionary<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect>();

		// Token: 0x02000F1D RID: 3869
		[Serializable]
		public enum TargetType
		{
			// Token: 0x04006B17 RID: 27415
			Owner,
			// Token: 0x04006B18 RID: 27416
			Others,
			// Token: 0x04006B19 RID: 27417
			All
		}

		// Token: 0x02000F1E RID: 3870
		[Serializable]
		public class CosmeticEffect
		{
			// Token: 0x17000948 RID: 2376
			// (get) Token: 0x06005FE2 RID: 24546 RVA: 0x001E73BE File Offset: 0x001E55BE
			// (set) Token: 0x06005FE3 RID: 24547 RVA: 0x001E73C6 File Offset: 0x001E55C6
			public float knockbackStrengthMultiplier { get; set; }

			// Token: 0x06005FE4 RID: 24548 RVA: 0x001E73D0 File Offset: 0x001E55D0
			public bool IsGameModeAllowed()
			{
				GameModeType value = (GameMode.ActiveGameMode != null) ? GameMode.ActiveGameMode.GameType() : GameModeType.Casual;
				return !this.excludeForGameModes.Contains(value);
			}

			// Token: 0x17000949 RID: 2377
			// (get) Token: 0x06005FE5 RID: 24549 RVA: 0x001E7409 File Offset: 0x001E5609
			// (set) Token: 0x06005FE6 RID: 24550 RVA: 0x001E7411 File Offset: 0x001E5611
			public float EffectDuration
			{
				get
				{
					return this.effectDurationOthers;
				}
				set
				{
					this.effectDurationOthers = value;
				}
			}

			// Token: 0x1700094A RID: 2378
			// (get) Token: 0x06005FE7 RID: 24551 RVA: 0x001E741A File Offset: 0x001E561A
			// (set) Token: 0x06005FE8 RID: 24552 RVA: 0x001E7422 File Offset: 0x001E5622
			public float EffectStartedTime { get; set; }

			// Token: 0x06005FE9 RID: 24553 RVA: 0x001E742B File Offset: 0x001E562B
			private bool IsSkin()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.Skin;
			}

			// Token: 0x06005FEA RID: 24554 RVA: 0x001E7436 File Offset: 0x001E5636
			private bool IsFPV()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.FPV;
			}

			// Token: 0x06005FEB RID: 24555 RVA: 0x001E7441 File Offset: 0x001E5641
			private bool IsTagKnockback()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback;
			}

			// Token: 0x06005FEC RID: 24556 RVA: 0x001E744C File Offset: 0x001E564C
			private bool IsInstantKnockback()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.InstantKnockback;
			}

			// Token: 0x06005FED RID: 24557 RVA: 0x001E7458 File Offset: 0x001E5658
			private bool HasKnockback()
			{
				CosmeticEffectsOnPlayers.EFFECTTYPE effecttype = this.effectType;
				return effecttype == CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback || effecttype == CosmeticEffectsOnPlayers.EFFECTTYPE.InstantKnockback;
			}

			// Token: 0x06005FEE RID: 24558 RVA: 0x001E747D File Offset: 0x001E567D
			private bool IsVO()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.VoiceOverride;
			}

			// Token: 0x06005FEF RID: 24559 RVA: 0x001E7488 File Offset: 0x001E5688
			private bool IsSFX()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.SFX;
			}

			// Token: 0x1700094B RID: 2379
			// (get) Token: 0x06005FF0 RID: 24560 RVA: 0x001E7493 File Offset: 0x001E5693
			private HashSet<GameModeType> Modes
			{
				get
				{
					if (this.modesHash == null)
					{
						this.modesHash = new HashSet<GameModeType>(this.excludeForGameModes);
					}
					return this.modesHash;
				}
			}

			// Token: 0x04006B1A RID: 27418
			public GameModeType[] excludeForGameModes;

			// Token: 0x04006B1B RID: 27419
			public CosmeticEffectsOnPlayers.EFFECTTYPE effectType;

			// Token: 0x04006B1C RID: 27420
			public float effectDistanceRadius;

			// Token: 0x04006B1D RID: 27421
			public CosmeticEffectsOnPlayers.TargetType target = CosmeticEffectsOnPlayers.TargetType.All;

			// Token: 0x04006B1E RID: 27422
			public float effectDurationOthers;

			// Token: 0x04006B1F RID: 27423
			public float effectDurationOwner;

			// Token: 0x04006B20 RID: 27424
			public GorillaSkin newSkin;

			// Token: 0x04006B21 RID: 27425
			[Tooltip("Spawn effects for the FPV - Use object pools")]
			public GameObject FPVEffect;

			// Token: 0x04006B22 RID: 27426
			[Tooltip("Use object pools")]
			public GameObject knockbackVFX;

			// Token: 0x04006B23 RID: 27427
			[FormerlySerializedAs("knockbackStrengthMultiplier")]
			public float knockbackStrength;

			// Token: 0x04006B24 RID: 27428
			[Tooltip("force pushing players with hands on the ground")]
			public bool forceOffTheGround;

			// Token: 0x04006B25 RID: 27429
			[Tooltip("Take the horizontal magnitude of the knockback, and add it opposite gravity. For example, being hit sideways will also impart a large upwards force. Breaks conservation of energy, but feels better to the player.")]
			public bool specialVerticalForce;

			// Token: 0x04006B26 RID: 27430
			[FormerlySerializedAs("minStrengthClamp")]
			public float minKnockbackStrength = 0.5f;

			// Token: 0x04006B27 RID: 27431
			[FormerlySerializedAs("maxStrengthClamp")]
			public float maxKnockbackStrength = 6f;

			// Token: 0x04006B29 RID: 27433
			public AudioClip[] voiceOverrideNormalClips;

			// Token: 0x04006B2A RID: 27434
			public AudioClip[] voiceOverrideLoudClips;

			// Token: 0x04006B2B RID: 27435
			public float voiceOverrideNormalVolume = 0.5f;

			// Token: 0x04006B2C RID: 27436
			public float voiceOverrideLoudVolume = 0.8f;

			// Token: 0x04006B2D RID: 27437
			public float voiceOverrideLoudThreshold = 0.175f;

			// Token: 0x04006B2E RID: 27438
			[Tooltip("plays sfx on player")]
			public List<AudioClip> sfxAudioClip;

			// Token: 0x04006B2F RID: 27439
			private HashSet<GameModeType> modesHash;
		}

		// Token: 0x02000F1F RID: 3871
		public enum EFFECTTYPE
		{
			// Token: 0x04006B32 RID: 27442
			Skin,
			// Token: 0x04006B33 RID: 27443
			FPV,
			// Token: 0x04006B34 RID: 27444
			TagWithKnockback,
			// Token: 0x04006B35 RID: 27445
			InstantKnockback,
			// Token: 0x04006B36 RID: 27446
			VoiceOverride,
			// Token: 0x04006B37 RID: 27447
			SFX
		}
	}
}
