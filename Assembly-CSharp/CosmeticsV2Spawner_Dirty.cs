using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cysharp.Text;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaNetworking.Store;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

// Token: 0x020001CA RID: 458
public class CosmeticsV2Spawner_Dirty : IDelayedExecListener, ITickSystemTick
{
	// Token: 0x1700011E RID: 286
	// (get) Token: 0x06000B5E RID: 2910 RVA: 0x0003E99F File Offset: 0x0003CB9F
	// (set) Token: 0x06000B5F RID: 2911 RVA: 0x0003E9A6 File Offset: 0x0003CBA6
	[OnEnterPlay_Set(false)]
	public static bool startedAllPartsInstantiated { get; private set; }

	// Token: 0x1700011F RID: 287
	// (get) Token: 0x06000B60 RID: 2912 RVA: 0x0003E9AE File Offset: 0x0003CBAE
	// (set) Token: 0x06000B61 RID: 2913 RVA: 0x0003E9B5 File Offset: 0x0003CBB5
	[OnEnterPlay_Set(false)]
	public static bool allPartsInstantiated { get; private set; }

	// Token: 0x17000120 RID: 288
	// (get) Token: 0x06000B62 RID: 2914 RVA: 0x0003E9BD File Offset: 0x0003CBBD
	// (set) Token: 0x06000B63 RID: 2915 RVA: 0x0003E9C4 File Offset: 0x0003CBC4
	[OnEnterPlay_Set(false)]
	public static bool completed { get; private set; }

	// Token: 0x17000121 RID: 289
	// (get) Token: 0x06000B64 RID: 2916 RVA: 0x0003E9CC File Offset: 0x0003CBCC
	// (set) Token: 0x06000B65 RID: 2917 RVA: 0x0003E9D4 File Offset: 0x0003CBD4
	public bool TickRunning { get; set; }

	// Token: 0x06000B66 RID: 2918 RVA: 0x0003E9DD File Offset: 0x0003CBDD
	void ITickSystemTick.Tick()
	{
		this._shouldTick = false;
		if (CosmeticsV2Spawner_Dirty._g_loadOp_to_index.Count < CosmeticsV2Spawner_Dirty._g_loadOpInfos.Count)
		{
			this._shouldTick = true;
			CosmeticsV2Spawner_Dirty._Step2_UpdateLoadOpStarting();
		}
		if (!this._shouldTick)
		{
			TickSystem<object>.RemoveTickCallback(this);
		}
	}

	// Token: 0x06000B67 RID: 2919 RVA: 0x0003EA16 File Offset: 0x0003CC16
	void IDelayedExecListener.OnDelayedAction(int contextId)
	{
		if (contextId >= 0 && contextId < 1000000)
		{
			CosmeticsV2Spawner_Dirty._RetryDownload(contextId);
			return;
		}
		if (contextId == -100)
		{
			this._DelayedStatusCheck();
			return;
		}
		if (contextId == -Mathf.Abs("_Step5_InitializeVRRigsAndCosmeticsControllerFinalize".GetHashCode()))
		{
			CosmeticsV2Spawner_Dirty._Step5_InitializeVRRigsAndCosmeticsControllerFinalize();
		}
	}

	// Token: 0x06000B68 RID: 2920 RVA: 0x0003EA50 File Offset: 0x0003CC50
	public static void StartInstantiatingPrefabs()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (CosmeticsV2Spawner_Dirty.startedAllPartsInstantiated || CosmeticsV2Spawner_Dirty.allPartsInstantiated)
		{
			Debug.LogError("CosmeticsV2Spawner_Dirty.StartInstantiatingPrefabs: All parts already started instantiated. Check `startedAllPartsInstantiated` before calling this.");
			return;
		}
		if (CosmeticsV2Spawner_Dirty._instance == null)
		{
			CosmeticsV2Spawner_Dirty._instance = new CosmeticsV2Spawner_Dirty();
		}
		CosmeticsV2Spawner_Dirty.k_stopwatch.Restart();
		CosmeticsV2Spawner_Dirty.g_gorillaPlayer = Object.FindObjectOfType<GTPlayer>();
		foreach (SnowballMaker snowballMaker in CosmeticsV2Spawner_Dirty.g_gorillaPlayer.GetComponentsInChildren<SnowballMaker>(true))
		{
			if (snowballMaker.isLeftHand)
			{
				CosmeticsV2Spawner_Dirty._gSnowballMakerLeft = snowballMaker;
			}
			else
			{
				CosmeticsV2Spawner_Dirty._gSnowballMakerRight = snowballMaker;
			}
		}
		if (!CosmeticsController.hasInstance)
		{
			Debug.LogError("(should never happen) cannot instantiate prefabs before cosmetics controller instance is available.");
			return;
		}
		if (!CosmeticsController.instance.v2_allCosmeticsInfoAssetRef.IsValid())
		{
			Debug.LogError("(should never happen) cannot load prefabs before v2_allCosmeticsInfoAssetRef is loaded.");
			return;
		}
		AllCosmeticsArraySO allCosmeticsArraySO = CosmeticsController.instance.v2_allCosmeticsInfoAssetRef.Asset as AllCosmeticsArraySO;
		if (allCosmeticsArraySO == null)
		{
			Debug.LogError("(should never happen) v2_allCosmeticsInfoAssetRef is valid but null.");
			return;
		}
		Transform[] boneXforms;
		string str;
		if (!GTHardCodedBones.TryGetBoneXforms(VRRig.LocalRig, out boneXforms, out str))
		{
			Debug.LogError("CosmeticsV2Spawner_Dirty: Error getting bone Transforms from local VRRig: " + str, VRRig.LocalRig);
			return;
		}
		CosmeticsV2Spawner_Dirty._gVRRigDatas.Add(new CosmeticsV2Spawner_Dirty.VRRigData(VRRig.LocalRig, boneXforms));
		int vrRigIndex = 0;
		foreach (VRRig vrRig in VRRigCache.Instance.GetAllRigs())
		{
			Transform[] boneXforms2;
			if (!GTHardCodedBones.TryGetBoneXforms(vrRig, out boneXforms2, out str))
			{
				Debug.LogError("CosmeticsV2Spawner_Dirty: Error getting bone Transforms from cached VRRig: " + str, VRRig.LocalRig);
				return;
			}
			CosmeticsV2Spawner_Dirty._gVRRigDatas.Add(new CosmeticsV2Spawner_Dirty.VRRigData(vrRig, boneXforms2));
		}
		CosmeticsV2Spawner_Dirty._gDeactivatedSpawnParent = GlobalDeactivatedSpawnRoot.GetOrCreate();
		GTDelayedExec.Add(CosmeticsV2Spawner_Dirty._instance, 2f, -100);
		int num = 0;
		int num2 = 0;
		foreach (GTDirectAssetRef<CosmeticSO> gtdirectAssetRef in allCosmeticsArraySO.sturdyAssetRefs)
		{
			CosmeticInfoV2 info = gtdirectAssetRef.obj.info;
			if (info.hasHoldableParts)
			{
				for (int j = 0; j < CosmeticsV2Spawner_Dirty._gVRRigDatas.Count; j++)
				{
					for (int k = 0; k < info.holdableParts.Length; k++)
					{
						CosmeticPart cosmeticPart = info.holdableParts[k];
						if (!cosmeticPart.prefabAssetRef.RuntimeKeyIsValid())
						{
							if (j == 0)
							{
								GTDev.LogError<string>("Cosmetic " + info.displayName + " has missing object reference in wearable parts, skipping load", null);
							}
						}
						else
						{
							CosmeticsV2Spawner_Dirty.AddEachAttachInfoToLoadOpInfosList(cosmeticPart, k, info, j, ref num);
						}
					}
				}
			}
			if (info.hasFunctionalParts)
			{
				for (int l = 0; l < CosmeticsV2Spawner_Dirty._gVRRigDatas.Count; l++)
				{
					for (int m = 0; m < info.functionalParts.Length; m++)
					{
						CosmeticPart cosmeticPart2 = info.functionalParts[m];
						if (!cosmeticPart2.prefabAssetRef.RuntimeKeyIsValid())
						{
							if (l == 0)
							{
								GTDev.LogError<string>("Cosmetic " + info.displayName + " has missing object reference in functional parts, skipping load", null);
							}
						}
						else
						{
							CosmeticsV2Spawner_Dirty.AddEachAttachInfoToLoadOpInfosList(cosmeticPart2, m, info, l, ref num);
						}
					}
				}
			}
			if (info.hasFirstPersonViewParts)
			{
				for (int n = 0; n < info.firstPersonViewParts.Length; n++)
				{
					CosmeticPart cosmeticPart3 = info.firstPersonViewParts[n];
					if (!cosmeticPart3.prefabAssetRef.RuntimeKeyIsValid())
					{
						GTDev.LogError<string>("Cosmetic " + info.displayName + " has missing object reference in first person parts, skipping load", null);
					}
					else
					{
						CosmeticsV2Spawner_Dirty.AddEachAttachInfoToLoadOpInfosList(cosmeticPart3, n, info, vrRigIndex, ref num2);
					}
				}
			}
		}
		TickSystem<object>.AddTickCallback(CosmeticsV2Spawner_Dirty._instance);
	}

	// Token: 0x06000B69 RID: 2921 RVA: 0x0003EDBC File Offset: 0x0003CFBC
	private static void AddEachAttachInfoToLoadOpInfosList(CosmeticPart part, int partIndex, CosmeticInfoV2 cosmeticInfo, int vrRigIndex, ref int partCount)
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		for (int i = 0; i < part.attachAnchors.Length; i++)
		{
			CosmeticsV2Spawner_Dirty.LoadOpInfo item = new CosmeticsV2Spawner_Dirty.LoadOpInfo(part.attachAnchors[i], part, partIndex, cosmeticInfo, vrRigIndex);
			CosmeticsV2Spawner_Dirty._g_loadOpInfos.Add(item);
			partCount++;
			if (part.partType == ECosmeticPartType.Holdable && i == 0)
			{
				break;
			}
		}
	}

	// Token: 0x06000B6A RID: 2922 RVA: 0x0003EE1C File Offset: 0x0003D01C
	private static void _Step2_UpdateLoadOpStarting()
	{
		int num = CosmeticsV2Spawner_Dirty._g_loadOp_to_index.Count - CosmeticsV2Spawner_Dirty._g_loadOpsCountCompleted;
		while (CosmeticsV2Spawner_Dirty._g_loadOp_to_index.Count < CosmeticsV2Spawner_Dirty._g_loadOpInfos.Count && num < 1000000)
		{
			num++;
			int count = CosmeticsV2Spawner_Dirty._g_loadOp_to_index.Count;
			CosmeticsV2Spawner_Dirty.LoadOpInfo loadOpInfo = CosmeticsV2Spawner_Dirty._g_loadOpInfos[count];
			loadOpInfo.loadOp = loadOpInfo.part.prefabAssetRef.InstantiateAsync(CosmeticsV2Spawner_Dirty._gDeactivatedSpawnParent, false);
			loadOpInfo.isStarted = true;
			CosmeticsV2Spawner_Dirty._g_loadOp_to_index.Add(loadOpInfo.loadOp, count);
			loadOpInfo.loadOp.Completed += CosmeticsV2Spawner_Dirty._Step3_HandleLoadOpCompleted;
			CosmeticsV2Spawner_Dirty._g_loadOpInfos[count] = loadOpInfo;
		}
	}

	// Token: 0x06000B6B RID: 2923 RVA: 0x0003EED4 File Offset: 0x0003D0D4
	private static void _Step3_HandleLoadOpCompleted(AsyncOperationHandle<GameObject> loadOp)
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		int num;
		if (!CosmeticsV2Spawner_Dirty._g_loadOp_to_index.TryGetValue(loadOp, out num))
		{
			throw new Exception("(this should never happen) could not find LoadOpInfo in `_g_loadOpInfos`.");
		}
		CosmeticsV2Spawner_Dirty.LoadOpInfo loadOpInfo = CosmeticsV2Spawner_Dirty._g_loadOpInfos[num];
		if (loadOp.Status == AsyncOperationStatus.Failed)
		{
			Debug.Log("CosmeticsV2Spawner_Dirty: Failed to load a part for cosmetic \"" + loadOpInfo.cosmeticInfoV2.displayName + "\"! waiting for 10 seconds before trying again.");
			GTDelayedExec.Add(CosmeticsV2Spawner_Dirty._instance, 10f, num);
			return;
		}
		CosmeticsV2Spawner_Dirty._g_loadOpsCountCompleted++;
		ECosmeticSelectSide ecosmeticSelectSide = loadOpInfo.attachInfo.selectSide;
		string name = loadOpInfo.cosmeticInfoV2.playFabID;
		if (ecosmeticSelectSide != ECosmeticSelectSide.Both)
		{
			string playFabID = loadOpInfo.cosmeticInfoV2.playFabID;
			string arg;
			if (ecosmeticSelectSide != ECosmeticSelectSide.Left)
			{
				if (ecosmeticSelectSide != ECosmeticSelectSide.Right)
				{
					arg = "";
				}
				else
				{
					arg = " RIGHT.";
				}
			}
			else
			{
				arg = " LEFT.";
			}
			name = ZString.Concat<string, string>(playFabID, arg);
		}
		loadOpInfo.resultGObj = loadOp.Result;
		loadOpInfo.resultGObj.SetActive(false);
		Transform transform = loadOpInfo.resultGObj.transform;
		Transform transform2 = transform;
		CosmeticPart[] holdableParts = loadOpInfo.cosmeticInfoV2.holdableParts;
		if (holdableParts != null && holdableParts.Length > 0)
		{
			TransferrableObject componentInChildren = loadOpInfo.resultGObj.GetComponentInChildren<TransferrableObject>(true);
			if (componentInChildren && componentInChildren.gameObject != loadOpInfo.resultGObj)
			{
				transform2 = componentInChildren.transform;
				transform2.gameObject.SetActive(false);
				loadOpInfo.resultGObj.SetActive(true);
			}
		}
		if (loadOpInfo.cosmeticInfoV2.isThrowable)
		{
			SnowballThrowable componentInChildren2 = loadOpInfo.resultGObj.GetComponentInChildren<SnowballThrowable>(true);
			if (componentInChildren2 && componentInChildren2.gameObject != loadOpInfo.resultGObj)
			{
				transform2 = componentInChildren2.transform;
				transform2.gameObject.SetActive(false);
				loadOpInfo.resultGObj.SetActive(true);
			}
		}
		transform2.name = name;
		CosmeticsV2Spawner_Dirty.VRRigData vrrigData = (loadOpInfo.vrRigIndex != -1) ? CosmeticsV2Spawner_Dirty._gVRRigDatas[loadOpInfo.vrRigIndex] : default(CosmeticsV2Spawner_Dirty.VRRigData);
		Transform transform3;
		switch (loadOpInfo.part.partType)
		{
		case ECosmeticPartType.Holdable:
			transform3 = ((loadOpInfo.attachInfo.parentBone != GTHardCodedBones.EBone.body_AnchorFront_StowSlot) ? vrrigData.parentOfDeactivatedHoldables : vrrigData.boneXforms[(int)loadOpInfo.attachInfo.parentBone]);
			goto IL_284;
		case ECosmeticPartType.Functional:
			transform3 = vrrigData.boneXforms[(int)loadOpInfo.attachInfo.parentBone];
			goto IL_284;
		case ECosmeticPartType.FirstPerson:
			transform3 = CosmeticsV2Spawner_Dirty.g_gorillaPlayer.CosmeticsHeadTarget;
			goto IL_284;
		}
		throw new ArgumentOutOfRangeException("partType", "unhandled part type.");
		IL_284:
		Transform transform4 = transform3;
		if (transform4)
		{
			transform.SetParent(transform4, false);
			transform.localPosition = loadOpInfo.attachInfo.offset.pos;
			Transform transform5 = transform;
			XformOffset offset = loadOpInfo.attachInfo.offset;
			transform5.localRotation = offset.rot;
			transform.localScale = loadOpInfo.attachInfo.offset.scale;
		}
		else
		{
			Debug.LogError(string.Concat(new string[]
			{
				string.Format("Bone transform not found for cosmetic part type {0}. Cosmetic: ", loadOpInfo.part.partType),
				"\"",
				loadOpInfo.cosmeticInfoV2.displayName,
				"\",",
				string.Format("part: \"{0}\"", loadOpInfo.part.prefabAssetRef.RuntimeKey)
			}));
		}
		switch (loadOpInfo.part.partType)
		{
		case ECosmeticPartType.Holdable:
		{
			vrrigData.vrRig_cosmetics.Add(transform2.gameObject);
			HoldableObject componentInChildren3 = loadOpInfo.resultGObj.GetComponentInChildren<HoldableObject>(true);
			SnowballThrowable snowballThrowable = componentInChildren3 as SnowballThrowable;
			if (snowballThrowable != null)
			{
				CosmeticsV2Spawner_Dirty.AddPartToThrowableLists(loadOpInfo, snowballThrowable);
				goto IL_5D6;
			}
			TransferrableObject transferrableObject = componentInChildren3 as TransferrableObject;
			if (transferrableObject == null)
			{
				if (componentInChildren3 != null)
				{
					throw new Exception("Encountered unexpected HoldableObject derived type on cosmetic part: \"" + loadOpInfo.cosmeticInfoV2.displayName + "\"");
				}
				goto IL_5D6;
			}
			else
			{
				vrrigData.bdPositions_allObjects.Add(transferrableObject);
				string text = loadOpInfo.cosmeticInfoV2.playFabID;
				int[] array;
				if (CosmeticsLegacyV1Info.TryGetBodyDockAllObjectsIndexes(text, out array))
				{
					if (loadOpInfo.partIndex < array.Length && loadOpInfo.partIndex >= 0)
					{
						transferrableObject.myIndex = array[loadOpInfo.partIndex];
					}
				}
				else if (text.Length >= 5 && text[0] == 'L')
				{
					if (text[1] != 'M')
					{
						throw new Exception("(this should never happen) A TransferrableObject cosmetic added sometime after 2024-06 does not use the expected PlayFabID format where the string starts with \"LM\" and ends with \".\". Path: " + transform2.GetPathQ());
					}
					string text2 = text;
					text = ((text2[text2.Length - 1] == '.') ? text : (text + "."));
					int num2 = 224;
					transferrableObject.myIndex = num2 + CosmeticIDUtils.PlayFabIdToIndexInCategory(text);
				}
				else
				{
					transferrableObject.myIndex = -2;
					if (!(text == "STICKABLE TARGET"))
					{
						Debug.LogError(string.Concat(new string[]
						{
							"Cosmetic \"",
							loadOpInfo.cosmeticInfoV2.displayName,
							"\" cannot derive `TransferrableObject.myIndex` from playFabId \"",
							text,
							"\" and so will not be included in `BodyDockPositions.allObjects` array."
						}));
					}
				}
				vrrigData.bdPositions_allObjects_length = math.max(transferrableObject.myIndex + 1, vrrigData.bdPositions_allObjects_length);
				ProjectileWeapon projectileWeapon = transferrableObject as ProjectileWeapon;
				if (projectileWeapon != null && loadOpInfo.cosmeticInfoV2.playFabID == "Slingshot")
				{
					vrrigData.vrRig.projectileWeapon = projectileWeapon;
					goto IL_5D6;
				}
				goto IL_5D6;
			}
			break;
		}
		case ECosmeticPartType.Functional:
			vrrigData.vrRig_cosmetics.Add(transform2.gameObject);
			goto IL_5D6;
		case ECosmeticPartType.FirstPerson:
			vrrigData.vrRig_override.Add(transform2.gameObject);
			goto IL_5D6;
		}
		throw new ArgumentOutOfRangeException("Unexpected ECosmeticPartType value encountered: " + string.Format("{0}, ", loadOpInfo.part.partType) + string.Format("int: {0}.", (int)loadOpInfo.part.partType));
		IL_5D6:
		if (loadOpInfo.vrRigIndex > -1)
		{
			CosmeticsV2Spawner_Dirty._gVRRigDatas[loadOpInfo.vrRigIndex] = vrrigData;
		}
		CosmeticRefRegistry cosmeticReferences = CosmeticsV2Spawner_Dirty._gVRRigDatas[loadOpInfo.vrRigIndex].vrRig.cosmeticReferences;
		CosmeticRefTarget[] componentsInChildren = loadOpInfo.resultGObj.GetComponentsInChildren<CosmeticRefTarget>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			cosmeticReferences.Register(componentsInChildren[i].id, componentsInChildren[i].gameObject);
		}
		CosmeticsV2Spawner_Dirty._g_loadOpInfos[num] = loadOpInfo;
		if (CosmeticsV2Spawner_Dirty._g_loadOpsCountCompleted < CosmeticsV2Spawner_Dirty._g_loadOpInfos.Count)
		{
			return;
		}
		CosmeticsV2Spawner_Dirty._Step4_PopulateAllArrays();
	}

	// Token: 0x06000B6C RID: 2924 RVA: 0x0003F550 File Offset: 0x0003D750
	private static void _RetryDownload(int loadOpIndex)
	{
		if (loadOpIndex < 0 || loadOpIndex >= CosmeticsV2Spawner_Dirty._g_loadOpInfos.Count)
		{
			Debug.LogError("(should never happen) Unexpected! While trying to recover from a failed download, the value " + string.Format("{0}={1} was out of range of ", "loadOpIndex", loadOpIndex) + string.Format("{0}.Count={1}.", "_g_loadOpInfos", CosmeticsV2Spawner_Dirty._g_loadOpInfos.Count));
			return;
		}
		CosmeticsV2Spawner_Dirty.LoadOpInfo loadOpInfo = CosmeticsV2Spawner_Dirty._g_loadOpInfos[loadOpIndex];
		if (!CosmeticsV2Spawner_Dirty._g_loadOp_to_index.Remove(loadOpInfo.loadOp))
		{
			Debug.LogWarning(string.Concat(new string[]
			{
				"(should never happen) Unexpected! Could not find the loadOp to remove it in the _g_loadOp_to_index. If you see this message then comparison does not work the way I thought and we need a different way to store/retrieve loadOpInfos. Happened while trying to retry failed download prefab part of cosmetic \"",
				loadOpInfo.cosmeticInfoV2.displayName,
				"\" with guid \"",
				loadOpInfo.part.prefabAssetRef.AssetGUID,
				"\"."
			}));
		}
		Debug.Log(string.Concat(new string[]
		{
			"Retrying prefab part of cosmetic \"",
			loadOpInfo.cosmeticInfoV2.displayName,
			"\" with guid \"",
			loadOpInfo.part.prefabAssetRef.AssetGUID,
			"\"."
		}));
		loadOpInfo.loadOp = loadOpInfo.part.prefabAssetRef.InstantiateAsync(CosmeticsV2Spawner_Dirty._gDeactivatedSpawnParent, false);
		CosmeticsV2Spawner_Dirty._g_loadOpInfos[loadOpIndex] = loadOpInfo;
		CosmeticsV2Spawner_Dirty._g_loadOp_to_index[loadOpInfo.loadOp] = loadOpIndex;
		loadOpInfo.loadOp.Completed += CosmeticsV2Spawner_Dirty._Step3_HandleLoadOpCompleted;
	}

	// Token: 0x06000B6D RID: 2925 RVA: 0x0003F6B4 File Offset: 0x0003D8B4
	private static void AddPartToThrowableLists(CosmeticsV2Spawner_Dirty.LoadOpInfo loadOpInfo, SnowballThrowable throwable)
	{
		CosmeticsV2Spawner_Dirty.VRRigData vrrigData = CosmeticsV2Spawner_Dirty._gVRRigDatas[loadOpInfo.vrRigIndex];
		EHandedness handednessFromBone = GTHardCodedBones.GetHandednessFromBone(loadOpInfo.attachInfo.parentBone);
		bool flag = vrrigData.vrRig == CosmeticsV2Spawner_Dirty._gVRRigDatas[0].vrRig;
		switch (handednessFromBone)
		{
		case EHandedness.None:
			throw new ArgumentException(string.Concat(new string[]
			{
				"Encountered throwable cosmetic \"",
				loadOpInfo.cosmeticInfoV2.displayName,
				"\" where handedness ",
				string.Format("could not be determined from bone `{0}`. ", loadOpInfo.attachInfo.parentBone),
				"Path: \"",
				throwable.transform.GetPath(),
				"\""
			}));
		case EHandedness.Left:
			CosmeticsV2Spawner_Dirty.ResizeAndSetAtIndex<GameObject>(vrrigData.bdPositions_leftHandThrowables, throwable.gameObject, throwable.throwableMakerIndex);
			if (flag)
			{
				CosmeticsV2Spawner_Dirty.ResizeAndSetAtIndex<SnowballThrowable>(CosmeticsV2Spawner_Dirty._gSnowballMakerLeft_throwables, throwable, throwable.throwableMakerIndex);
				return;
			}
			break;
		case EHandedness.Right:
			CosmeticsV2Spawner_Dirty.ResizeAndSetAtIndex<GameObject>(vrrigData.bdPositions_rightHandThrowables, throwable.gameObject, throwable.throwableMakerIndex);
			if (flag)
			{
				CosmeticsV2Spawner_Dirty.ResizeAndSetAtIndex<SnowballThrowable>(CosmeticsV2Spawner_Dirty._gSnowballMakerRight_throwables, throwable, throwable.throwableMakerIndex);
				return;
			}
			break;
		default:
			throw new ArgumentOutOfRangeException("Unexpected ECosmeticSelectSide value encountered: " + string.Format("{0}, ", handednessFromBone) + string.Format("int: {0}.", (int)handednessFromBone));
		}
	}

	// Token: 0x06000B6E RID: 2926 RVA: 0x0003F814 File Offset: 0x0003DA14
	private static void ResizeAndSetAtIndex<T>(List<T> list, T item, int index)
	{
		if (index >= list.Count)
		{
			int num = index - list.Count + 1;
			for (int i = 0; i < num; i++)
			{
				list.Add(default(T));
			}
		}
		list[index] = item;
	}

	// Token: 0x06000B6F RID: 2927 RVA: 0x0003F858 File Offset: 0x0003DA58
	private static void _Step4_PopulateAllArrays()
	{
		if (CosmeticsV2Spawner_Dirty.allPartsInstantiated)
		{
			Debug.LogError("_Step4_PopulateAllArrays: (should never happen) CALLED MORE THAN ONCE!");
			return;
		}
		foreach (CosmeticsV2Spawner_Dirty.LoadOpInfo loadOpInfo in CosmeticsV2Spawner_Dirty._g_loadOpInfos)
		{
			ISpawnable[] componentsInChildren = loadOpInfo.resultGObj.GetComponentsInChildren<ISpawnable>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				try
				{
					componentsInChildren[i].IsSpawned = true;
					componentsInChildren[i].CosmeticSelectedSide = loadOpInfo.attachInfo.selectSide;
					componentsInChildren[i].OnSpawn(CosmeticsV2Spawner_Dirty._gVRRigDatas[loadOpInfo.vrRigIndex].vrRig);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
		}
		CosmeticsV2Spawner_Dirty._gSnowballMakerLeft.SetupThrowables(CosmeticsV2Spawner_Dirty._gSnowballMakerLeft_throwables.ToArray());
		CosmeticsV2Spawner_Dirty._gSnowballMakerRight.SetupThrowables(CosmeticsV2Spawner_Dirty._gSnowballMakerRight_throwables.ToArray());
		foreach (CosmeticsV2Spawner_Dirty.VRRigData vrrigData in CosmeticsV2Spawner_Dirty._gVRRigDatas)
		{
			vrrigData.vrRig.cosmetics = vrrigData.vrRig_cosmetics.ToArray();
			vrrigData.vrRig.overrideCosmetics = vrrigData.vrRig_override.ToArray();
			vrrigData.bdPositionsComp.leftHandThrowables = vrrigData.bdPositions_leftHandThrowables.ToArray();
			vrrigData.bdPositionsComp.rightHandThrowables = vrrigData.bdPositions_rightHandThrowables.ToArray();
			vrrigData.bdPositionsComp._allObjects = new TransferrableObject[vrrigData.bdPositions_allObjects_length];
			foreach (TransferrableObject transferrableObject in vrrigData.bdPositions_allObjects)
			{
				if (transferrableObject.myIndex >= 0 && transferrableObject.myIndex < vrrigData.bdPositions_allObjects_length)
				{
					vrrigData.bdPositionsComp._allObjects[transferrableObject.myIndex] = transferrableObject;
				}
			}
		}
		CosmeticsV2Spawner_Dirty.allPartsInstantiated = true;
		GTDelayedExec.Add(CosmeticsV2Spawner_Dirty._instance, 1f, -Mathf.Abs("_Step5_InitializeVRRigsAndCosmeticsControllerFinalize".GetHashCode()));
	}

	// Token: 0x06000B70 RID: 2928 RVA: 0x0003FA9C File Offset: 0x0003DC9C
	private static void _Step5_InitializeVRRigsAndCosmeticsControllerFinalize()
	{
		CosmeticsController.instance.UpdateWardrobeModelsAndButtons();
		try
		{
			Action onPostInstantiateAllPrefabs = CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs;
			if (onPostInstantiateAllPrefabs != null)
			{
				onPostInstantiateAllPrefabs();
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		try
		{
			CosmeticsController.instance.InitializeCosmeticStands();
		}
		catch (Exception exception2)
		{
			Debug.LogException(exception2);
		}
		try
		{
			Action onPostInstantiateAllPrefabs2 = CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs2;
			if (onPostInstantiateAllPrefabs2 != null)
			{
				onPostInstantiateAllPrefabs2();
			}
		}
		catch (Exception exception3)
		{
			Debug.LogException(exception3);
		}
		try
		{
			CosmeticsController.instance.UpdateWornCosmetics(false);
		}
		catch (Exception exception4)
		{
			Debug.LogException(exception4);
		}
		foreach (CosmeticsV2Spawner_Dirty.VRRigData vrrigData in CosmeticsV2Spawner_Dirty._gVRRigDatas)
		{
			try
			{
				if (vrrigData.bdPositionsComp.isActiveAndEnabled)
				{
					vrrigData.bdPositionsComp.RefreshTransferrableItems();
				}
			}
			catch (Exception exception5)
			{
				Debug.LogException(exception5, vrrigData.vrRig);
			}
		}
		try
		{
			StoreController.instance.InitalizeCosmeticStands();
		}
		catch (Exception exception6)
		{
			Debug.LogException(exception6);
		}
		CosmeticsV2Spawner_Dirty.completed = true;
		CosmeticsV2Spawner_Dirty.k_stopwatch.Stop();
		Debug.Log("_Step5_InitializeVRRigsAndCosmeticsControllerFinalize" + string.Format(": Done instantiating cosmetics in {0:0.0000} seconds.", (double)CosmeticsV2Spawner_Dirty.k_stopwatch.ElapsedMilliseconds / 1000.0));
	}

	// Token: 0x06000B71 RID: 2929 RVA: 0x0003FC1C File Offset: 0x0003DE1C
	private void _DelayedStatusCheck()
	{
		int count = CosmeticsV2Spawner_Dirty._g_loadOpInfos.Count;
		Debug.Log(ZString.Concat<string, string, string, string, double, string, int, string, int, string>("CosmeticsV2Spawner_Dirty", ".", "_DelayedStatusCheck", ": Load progress ", (double)CosmeticsV2Spawner_Dirty._g_loadOpsCountCompleted / (double)count * 100.0, "% (", CosmeticsV2Spawner_Dirty._g_loadOpsCountCompleted, "/", count, ")."));
		if (CosmeticsV2Spawner_Dirty._g_loadOpsCountCompleted < count)
		{
			GTDelayedExec.Add(this, 2f, -100);
		}
	}

	// Token: 0x04000DF7 RID: 3575
	private static CosmeticsV2Spawner_Dirty _instance;

	// Token: 0x04000DF8 RID: 3576
	public static Action OnPostInstantiateAllPrefabs;

	// Token: 0x04000DF9 RID: 3577
	public static Action OnPostInstantiateAllPrefabs2;

	// Token: 0x04000DFD RID: 3581
	[OnEnterPlay_SetNull]
	private static Transform _gDeactivatedSpawnParent;

	// Token: 0x04000DFE RID: 3582
	[OnEnterPlay_Set(0)]
	private static int _g_loadOpsCountCompleted = 0;

	// Token: 0x04000DFF RID: 3583
	private const int _k_maxActiveLoadOps = 1000000;

	// Token: 0x04000E00 RID: 3584
	private const int _k_maxTotalLoadOps = 1000000;

	// Token: 0x04000E01 RID: 3585
	private const int _k_delayedStatusCheckContextId = -100;

	// Token: 0x04000E02 RID: 3586
	[OnEnterPlay_Clear]
	private static readonly List<CosmeticsV2Spawner_Dirty.LoadOpInfo> _g_loadOpInfos = new List<CosmeticsV2Spawner_Dirty.LoadOpInfo>(100000);

	// Token: 0x04000E03 RID: 3587
	[OnEnterPlay_Clear]
	private static readonly Dictionary<AsyncOperationHandle<GameObject>, int> _g_loadOp_to_index = new Dictionary<AsyncOperationHandle<GameObject>, int>(100000);

	// Token: 0x04000E04 RID: 3588
	[OnEnterPlay_SetNull]
	private static SnowballMaker _gSnowballMakerLeft;

	// Token: 0x04000E05 RID: 3589
	[OnEnterPlay_Clear]
	private static readonly List<SnowballThrowable> _gSnowballMakerLeft_throwables = new List<SnowballThrowable>(20);

	// Token: 0x04000E06 RID: 3590
	[OnEnterPlay_SetNull]
	private static SnowballMaker _gSnowballMakerRight;

	// Token: 0x04000E07 RID: 3591
	[OnEnterPlay_Clear]
	private static readonly List<SnowballThrowable> _gSnowballMakerRight_throwables = new List<SnowballThrowable>(20);

	// Token: 0x04000E08 RID: 3592
	[OnEnterPlay_SetNull]
	private static GTPlayer g_gorillaPlayer;

	// Token: 0x04000E09 RID: 3593
	[OnEnterPlay_SetNull]
	private static Transform[] g_allInstantiatedParts;

	// Token: 0x04000E0A RID: 3594
	private static Stopwatch k_stopwatch = new Stopwatch();

	// Token: 0x04000E0B RID: 3595
	[OnEnterPlay_Clear]
	private static readonly List<CosmeticsV2Spawner_Dirty.VRRigData> _gVRRigDatas = new List<CosmeticsV2Spawner_Dirty.VRRigData>(11);

	// Token: 0x04000E0C RID: 3596
	private bool _shouldTick;

	// Token: 0x020001CB RID: 459
	private struct LoadOpInfo
	{
		// Token: 0x06000B74 RID: 2932 RVA: 0x0003FCF0 File Offset: 0x0003DEF0
		public LoadOpInfo(CosmeticAttachInfo attachInfo, CosmeticPart part, int partIndex, CosmeticInfoV2 cosmeticInfoV2, int vrRigIndex)
		{
			this.isStarted = false;
			this.loadOp = default(AsyncOperationHandle<GameObject>);
			this.resultGObj = null;
			this.attachInfo = attachInfo;
			this.part = part;
			this.partIndex = partIndex;
			this.cosmeticInfoV2 = cosmeticInfoV2;
			this.vrRigIndex = vrRigIndex;
		}

		// Token: 0x04000E0E RID: 3598
		public bool isStarted;

		// Token: 0x04000E0F RID: 3599
		public AsyncOperationHandle<GameObject> loadOp;

		// Token: 0x04000E10 RID: 3600
		public GameObject resultGObj;

		// Token: 0x04000E11 RID: 3601
		public readonly CosmeticAttachInfo attachInfo;

		// Token: 0x04000E12 RID: 3602
		public readonly CosmeticPart part;

		// Token: 0x04000E13 RID: 3603
		public readonly int partIndex;

		// Token: 0x04000E14 RID: 3604
		public readonly CosmeticInfoV2 cosmeticInfoV2;

		// Token: 0x04000E15 RID: 3605
		public readonly int vrRigIndex;
	}

	// Token: 0x020001CC RID: 460
	private struct VRRigData
	{
		// Token: 0x06000B75 RID: 2933 RVA: 0x0003FD3C File Offset: 0x0003DF3C
		public VRRigData(VRRig vrRig, Transform[] boneXforms)
		{
			this.vrRig = vrRig;
			this.boneXforms = boneXforms;
			if (!vrRig.transform.TryFindByPath("./**/Holdables", out this.parentOfDeactivatedHoldables, false))
			{
				Debug.LogError("Could not find parent for deactivated holdables. Falling back to VRRig transform: \"" + vrRig.transform.GetPath() + "\"");
			}
			this.bdPositionsComp = vrRig.GetComponentInChildren<BodyDockPositions>(true);
			this.vrRig_cosmetics = new List<GameObject>(500);
			this.vrRig_override = new List<GameObject>(500);
			this.bdPositions_leftHandThrowables = new List<GameObject>(20);
			this.bdPositions_rightHandThrowables = new List<GameObject>(20);
			this.bdPositions_allObjects = new List<TransferrableObject>(20);
			this.bdPositions_allObjects_length = 0;
		}

		// Token: 0x04000E16 RID: 3606
		public readonly VRRig vrRig;

		// Token: 0x04000E17 RID: 3607
		public readonly Transform[] boneXforms;

		// Token: 0x04000E18 RID: 3608
		public readonly BodyDockPositions bdPositionsComp;

		// Token: 0x04000E19 RID: 3609
		public readonly List<GameObject> vrRig_cosmetics;

		// Token: 0x04000E1A RID: 3610
		public readonly List<GameObject> vrRig_override;

		// Token: 0x04000E1B RID: 3611
		public readonly Transform parentOfDeactivatedHoldables;

		// Token: 0x04000E1C RID: 3612
		public readonly List<TransferrableObject> bdPositions_allObjects;

		// Token: 0x04000E1D RID: 3613
		public int bdPositions_allObjects_length;

		// Token: 0x04000E1E RID: 3614
		public readonly List<GameObject> bdPositions_leftHandThrowables;

		// Token: 0x04000E1F RID: 3615
		public readonly List<GameObject> bdPositions_rightHandThrowables;
	}
}
