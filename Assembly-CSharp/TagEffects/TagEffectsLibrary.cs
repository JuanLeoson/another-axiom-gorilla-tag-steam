using System;
using System.Collections;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02000DFD RID: 3581
	public class TagEffectsLibrary : MonoBehaviour
	{
		// Token: 0x1700088F RID: 2191
		// (get) Token: 0x060058B3 RID: 22707 RVA: 0x001B92BB File Offset: 0x001B74BB
		public static float FistBumpSpeedThreshold
		{
			get
			{
				return TagEffectsLibrary._instance.fistBumpSpeedThreshold;
			}
		}

		// Token: 0x17000890 RID: 2192
		// (get) Token: 0x060058B4 RID: 22708 RVA: 0x001B92C7 File Offset: 0x001B74C7
		public static float HighFiveSpeedThreshold
		{
			get
			{
				return TagEffectsLibrary._instance.highFiveSpeedThreshold;
			}
		}

		// Token: 0x17000891 RID: 2193
		// (get) Token: 0x060058B5 RID: 22709 RVA: 0x001B92D3 File Offset: 0x001B74D3
		public static bool DebugMode
		{
			get
			{
				return TagEffectsLibrary._instance.debugMode;
			}
		}

		// Token: 0x060058B6 RID: 22710 RVA: 0x001B92DF File Offset: 0x001B74DF
		private void Awake()
		{
			if (TagEffectsLibrary._instance != null)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			TagEffectsLibrary._instance = this;
			this.tagEffectsPool = new Dictionary<string, Queue<GameObjectOnDisableDispatcher>>();
			this.tagEffectsComboLookUp = new Dictionary<TagEffectsCombo, TagEffectPack[]>();
		}

		// Token: 0x060058B7 RID: 22711 RVA: 0x001B9318 File Offset: 0x001B7518
		public static void PlayEffect(Transform target, bool isLeftHand, float rigScale, TagEffectsLibrary.EffectType effectType, TagEffectPack playerCosmeticTagEffectPack, TagEffectPack otherPlayerCosmeticTagEffectPack, Quaternion rotation)
		{
			if (TagEffectsLibrary._instance == null)
			{
				return;
			}
			ModeTagEffect modeTagEffect = null;
			TagEffectPack tagEffectPack = null;
			GameModeType item = (GameMode.ActiveGameMode != null) ? GameMode.ActiveGameMode.GameType() : GameModeType.Casual;
			for (int i = 0; i < TagEffectsLibrary._instance.defaultTagEffects.Length; i++)
			{
				if (TagEffectsLibrary._instance.defaultTagEffects[i] != null && TagEffectsLibrary._instance.defaultTagEffects[i].Modes.Contains(item))
				{
					modeTagEffect = TagEffectsLibrary._instance.defaultTagEffects[i];
					tagEffectPack = modeTagEffect.tagEffect;
					break;
				}
			}
			if (tagEffectPack == null)
			{
				return;
			}
			GameObject firstPerson = tagEffectPack.firstPerson;
			GameObject thirdPerson = tagEffectPack.thirdPerson;
			GameObject fistBump = tagEffectPack.fistBump;
			GameObject highFive = tagEffectPack.highFive;
			bool firstPersonParentEffect = tagEffectPack.firstPersonParentEffect;
			bool thirdPersonParentEffect = tagEffectPack.thirdPersonParentEffect;
			bool flag = tagEffectPack.fistBumpParentEffect;
			bool highFiveParentEffect = tagEffectPack.highFiveParentEffect;
			if (playerCosmeticTagEffectPack != null)
			{
				TagEffectPack tagEffectPack2 = TagEffectsLibrary.comboLookup(playerCosmeticTagEffectPack, otherPlayerCosmeticTagEffectPack);
				if (!modeTagEffect.blockFistBumpOverride && playerCosmeticTagEffectPack.fistBump != null)
				{
					fistBump = tagEffectPack2.fistBump;
					flag = tagEffectPack2.firstPersonParentEffect;
				}
				if (!modeTagEffect.blockHiveFiveOverride && playerCosmeticTagEffectPack.highFive != null)
				{
					highFive = tagEffectPack2.highFive;
					highFiveParentEffect = tagEffectPack2.highFiveParentEffect;
				}
			}
			if (otherPlayerCosmeticTagEffectPack != null)
			{
				if (!modeTagEffect.blockTagOverride && otherPlayerCosmeticTagEffectPack.firstPerson != null)
				{
					firstPerson = otherPlayerCosmeticTagEffectPack.firstPerson;
					firstPersonParentEffect = otherPlayerCosmeticTagEffectPack.firstPersonParentEffect;
				}
				if (!modeTagEffect.blockTagOverride && otherPlayerCosmeticTagEffectPack.thirdPerson != null)
				{
					thirdPerson = otherPlayerCosmeticTagEffectPack.thirdPerson;
					thirdPersonParentEffect = otherPlayerCosmeticTagEffectPack.thirdPersonParentEffect;
				}
			}
			switch (effectType)
			{
			case TagEffectsLibrary.EffectType.FIRST_PERSON:
				TagEffectsLibrary.placeEffects(firstPerson, target, firstPersonParentEffect ? 1f : rigScale, false, firstPersonParentEffect, rotation);
				return;
			case TagEffectsLibrary.EffectType.THIRD_PERSON:
				TagEffectsLibrary.placeEffects(thirdPerson, target, thirdPersonParentEffect ? 1f : rigScale, false, thirdPersonParentEffect, rotation);
				return;
			case TagEffectsLibrary.EffectType.HIGH_FIVE:
				TagEffectsLibrary.placeEffects(highFive, target, highFiveParentEffect ? 1f : rigScale, isLeftHand, highFiveParentEffect, rotation);
				return;
			case TagEffectsLibrary.EffectType.FIST_BUMP:
				TagEffectsLibrary.placeEffects(fistBump, target, flag ? 1f : rigScale, isLeftHand, flag, rotation);
				return;
			default:
				return;
			}
		}

		// Token: 0x060058B8 RID: 22712 RVA: 0x001B9538 File Offset: 0x001B7738
		private static TagEffectPack comboLookup(TagEffectPack playerCosmeticTagEffectPack, TagEffectPack otherPlayerCosmeticTagEffectPack)
		{
			if (otherPlayerCosmeticTagEffectPack == null)
			{
				return playerCosmeticTagEffectPack;
			}
			TagEffectsCombo tagEffectsCombo = new TagEffectsCombo();
			tagEffectsCombo.inputA = playerCosmeticTagEffectPack;
			tagEffectsCombo.inputB = otherPlayerCosmeticTagEffectPack;
			TagEffectPack[] array;
			if (!TagEffectsLibrary._instance.tagEffectsComboLookUp.TryGetValue(tagEffectsCombo, out array))
			{
				return playerCosmeticTagEffectPack;
			}
			int num = 0;
			if (GorillaComputer.instance != null)
			{
				num = GorillaComputer.instance.GetServerTime().Second;
			}
			return array[num % array.Length];
		}

		// Token: 0x060058B9 RID: 22713 RVA: 0x001B95A8 File Offset: 0x001B77A8
		public static void placeEffects(GameObject prefab, Transform target, float scale, bool flipZAxis, bool parentEffect, Quaternion rotation)
		{
			if (prefab == null)
			{
				return;
			}
			Queue<GameObjectOnDisableDispatcher> queue;
			if (!TagEffectsLibrary._instance.tagEffectsPool.TryGetValue(prefab.name, out queue))
			{
				queue = new Queue<GameObjectOnDisableDispatcher>();
				TagEffectsLibrary._instance.tagEffectsPool.Add(prefab.name, queue);
			}
			if (queue.Count == 0 || (queue.Peek().gameObject.activeInHierarchy && queue.Count < 12))
			{
				GameObject gameObject = Object.Instantiate<GameObject>(prefab, target.transform.position, rotation, parentEffect ? target : TagEffectsLibrary._instance.transform);
				gameObject.name = prefab.name;
				gameObject.transform.localScale = (flipZAxis ? new Vector3(scale, scale, -scale) : (Vector3.one * scale));
				GameObjectOnDisableDispatcher gameObjectOnDisableDispatcher;
				if (!gameObject.TryGetComponent<GameObjectOnDisableDispatcher>(out gameObjectOnDisableDispatcher))
				{
					gameObjectOnDisableDispatcher = gameObject.AddComponent<GameObjectOnDisableDispatcher>();
				}
				gameObjectOnDisableDispatcher.OnDisabled += TagEffectsLibrary.NewGameObjectOnDisableDispatcher_OnDisabled;
				gameObject.SetActive(true);
				queue.Enqueue(gameObjectOnDisableDispatcher);
				return;
			}
			GameObjectOnDisableDispatcher recycledGameObject = queue.Dequeue();
			TagEffectsLibrary._instance.StartCoroutine(TagEffectsLibrary._instance.RecycleGameObject(recycledGameObject, target, scale, flipZAxis, parentEffect));
		}

		// Token: 0x060058BA RID: 22714 RVA: 0x001B96C7 File Offset: 0x001B78C7
		private static void NewGameObjectOnDisableDispatcher_OnDisabled(GameObjectOnDisableDispatcher goodd)
		{
			TagEffectsLibrary._instance.StartCoroutine(TagEffectsLibrary._instance.ReclaimDisabled(goodd.transform));
		}

		// Token: 0x060058BB RID: 22715 RVA: 0x001B96E4 File Offset: 0x001B78E4
		private IEnumerator RecycleGameObject(GameObjectOnDisableDispatcher recycledGameObject, Transform target, float scale, bool flipZAxis, bool parentEffect)
		{
			if (recycledGameObject.gameObject.activeInHierarchy)
			{
				recycledGameObject.gameObject.SetActive(false);
				recycledGameObject.OnDisabled -= TagEffectsLibrary.NewGameObjectOnDisableDispatcher_OnDisabled;
				yield return null;
			}
			recycledGameObject.transform.position = target.transform.position;
			recycledGameObject.transform.rotation = target.transform.rotation;
			recycledGameObject.transform.localScale = (flipZAxis ? new Vector3(scale, scale, -scale) : (Vector3.one * scale));
			recycledGameObject.transform.parent = (parentEffect ? target : TagEffectsLibrary._instance.transform);
			Queue<GameObjectOnDisableDispatcher> queue;
			if (TagEffectsLibrary._instance.tagEffectsPool.TryGetValue(recycledGameObject.gameObject.name, out queue))
			{
				recycledGameObject.gameObject.SetActive(true);
				queue.Enqueue(recycledGameObject);
			}
			yield break;
		}

		// Token: 0x060058BC RID: 22716 RVA: 0x001B9711 File Offset: 0x001B7911
		private IEnumerator ReclaimDisabled(Transform transform)
		{
			yield return null;
			transform.parent = TagEffectsLibrary._instance.transform;
			yield break;
		}

		// Token: 0x04006281 RID: 25217
		private const int OBJECT_QUEUE_LIMIT = 12;

		// Token: 0x04006282 RID: 25218
		[OnEnterPlay_SetNull]
		private static TagEffectsLibrary _instance;

		// Token: 0x04006283 RID: 25219
		[SerializeField]
		private float fistBumpSpeedThreshold = 1f;

		// Token: 0x04006284 RID: 25220
		[SerializeField]
		private float highFiveSpeedThreshold = 1f;

		// Token: 0x04006285 RID: 25221
		[SerializeField]
		private ModeTagEffect[] defaultTagEffects;

		// Token: 0x04006286 RID: 25222
		[SerializeField]
		private TagEffectsComboResult[] tagEffectsCombos;

		// Token: 0x04006287 RID: 25223
		[SerializeField]
		private bool debugMode;

		// Token: 0x04006288 RID: 25224
		private Dictionary<string, Queue<GameObjectOnDisableDispatcher>> tagEffectsPool;

		// Token: 0x04006289 RID: 25225
		private Dictionary<TagEffectsCombo, TagEffectPack[]> tagEffectsComboLookUp;

		// Token: 0x02000DFE RID: 3582
		public enum EffectType
		{
			// Token: 0x0400628B RID: 25227
			FIRST_PERSON,
			// Token: 0x0400628C RID: 25228
			THIRD_PERSON,
			// Token: 0x0400628D RID: 25229
			HIGH_FIVE,
			// Token: 0x0400628E RID: 25230
			FIST_BUMP
		}
	}
}
