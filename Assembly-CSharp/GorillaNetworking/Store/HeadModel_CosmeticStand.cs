using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GorillaNetworking.Store
{
	// Token: 0x02000DCD RID: 3533
	public class HeadModel_CosmeticStand : HeadModel
	{
		// Token: 0x17000871 RID: 2161
		// (get) Token: 0x060057B2 RID: 22450 RVA: 0x001B3380 File Offset: 0x001B1580
		private string mountID
		{
			get
			{
				return "Mount_" + this.bustType.ToString();
			}
		}

		// Token: 0x060057B3 RID: 22451 RVA: 0x001B33A0 File Offset: 0x001B15A0
		public void LoadCosmeticParts(CosmeticSO cosmeticInfo, bool forRightSide = false)
		{
			this.ClearManuallySpawnedCosmeticParts();
			this.ClearCosmetics();
			if (cosmeticInfo == null)
			{
				Debug.LogWarning("Dynamic Cosmetics - LoadWardRobeParts -  No Cosmetic Info");
				return;
			}
			Debug.Log("Dynamic Cosmetics - Loading Wardrobe Parts for " + cosmeticInfo.info.playFabID);
			this.HandleLoadCosmeticParts(cosmeticInfo, forRightSide);
		}

		// Token: 0x060057B4 RID: 22452 RVA: 0x001B33F0 File Offset: 0x001B15F0
		private void ResetMannequinSkin()
		{
			this.mannequin.GetComponent<SkinnedMeshRenderer>();
			SkinnedMeshRenderer skinnedMeshRenderer;
			if (this.mannequin.TryGetComponent<SkinnedMeshRenderer>(out skinnedMeshRenderer))
			{
				Material[] sharedMaterials = new Material[]
				{
					this.defaultMannequinBody,
					this.defaultMannequinChest,
					this.defaultMannequinFace
				};
				skinnedMeshRenderer.sharedMaterials = sharedMaterials;
				return;
			}
			MeshRenderer meshRenderer;
			if (this.mannequin.TryGetComponent<MeshRenderer>(out meshRenderer))
			{
				Material[] sharedMaterials2 = new Material[]
				{
					this.defaultMannequinBody,
					this.defaultMannequinChest,
					this.defaultMannequinFace
				};
				meshRenderer.sharedMaterials = sharedMaterials2;
			}
		}

		// Token: 0x060057B5 RID: 22453 RVA: 0x001B347C File Offset: 0x001B167C
		private void HandleLoadCosmeticParts(CosmeticSO cosmeticInfo, bool forRightSide)
		{
			if (cosmeticInfo.info.category == CosmeticsController.CosmeticCategory.Set && !cosmeticInfo.info.hasStoreParts)
			{
				foreach (CosmeticSO cosmeticInfo2 in cosmeticInfo.info.setCosmetics)
				{
					this.HandleLoadCosmeticParts(cosmeticInfo2, forRightSide);
				}
				return;
			}
			CosmeticPart[] array;
			if (cosmeticInfo.info.storeParts.Length != 0)
			{
				array = cosmeticInfo.info.storeParts;
			}
			else
			{
				if (cosmeticInfo.info.category == CosmeticsController.CosmeticCategory.Fur)
				{
					CosmeticPart[] array2 = cosmeticInfo.info.functionalParts;
					int i = 0;
					if (i < array2.Length)
					{
						CosmeticPart cosmeticPart = array2[i];
						GameObject gameObject = this.LoadAndInstantiatePrefab(cosmeticPart.prefabAssetRef, base.transform);
						gameObject.GetComponent<GorillaSkinToggle>().ApplyToMannequin(this.mannequin);
						Object.DestroyImmediate(gameObject);
						return;
					}
				}
				array = cosmeticInfo.info.wardrobeParts;
			}
			foreach (CosmeticPart cosmeticPart2 in array)
			{
				foreach (CosmeticAttachInfo cosmeticAttachInfo in cosmeticPart2.attachAnchors)
				{
					if ((!forRightSide || !(cosmeticAttachInfo.selectSide == ECosmeticSelectSide.Left)) && (forRightSide || !(cosmeticAttachInfo.selectSide == ECosmeticSelectSide.Right)))
					{
						HeadModel._CosmeticPartLoadInfo partLoadInfo = new HeadModel._CosmeticPartLoadInfo
						{
							playFabId = cosmeticInfo.info.playFabID,
							prefabAssetRef = cosmeticPart2.prefabAssetRef,
							attachInfo = cosmeticAttachInfo,
							xform = null
						};
						GameObject gameObject2 = this.LoadAndInstantiatePrefab(cosmeticPart2.prefabAssetRef, base.transform);
						partLoadInfo.xform = gameObject2.transform;
						this._manuallySpawnedCosmeticParts.Add(gameObject2);
						gameObject2.SetActive(true);
						switch (this.bustType)
						{
						case HeadModel_CosmeticStand.BustType.Disabled:
							this.PositionWithWardRobeOffsets(partLoadInfo);
							break;
						case HeadModel_CosmeticStand.BustType.GorillaHead:
							this.PositionWithWardRobeOffsets(partLoadInfo);
							break;
						case HeadModel_CosmeticStand.BustType.GorillaTorso:
							this.PositionWithWardRobeOffsets(partLoadInfo);
							break;
						case HeadModel_CosmeticStand.BustType.GorillaTorsoPost:
							this.PositionWithWardRobeOffsets(partLoadInfo);
							break;
						case HeadModel_CosmeticStand.BustType.GorillaMannequin:
							this._manuallySpawnedCosmeticParts.Remove(gameObject2);
							Object.DestroyImmediate(gameObject2);
							break;
						case HeadModel_CosmeticStand.BustType.GuitarStand:
							this.PositionWardRobeItems(gameObject2, partLoadInfo);
							break;
						case HeadModel_CosmeticStand.BustType.JewelryBox:
							this.PositionWardRobeItems(gameObject2, partLoadInfo);
							break;
						case HeadModel_CosmeticStand.BustType.Table:
							this.PositionWardRobeItems(gameObject2, partLoadInfo);
							break;
						case HeadModel_CosmeticStand.BustType.PinDisplay:
							this.PositionWardRobeItems(gameObject2, partLoadInfo);
							break;
						case HeadModel_CosmeticStand.BustType.TagEffectDisplay:
							this.PositionWardRobeItems(gameObject2, partLoadInfo);
							break;
						default:
							this.PositionWithWardRobeOffsets(partLoadInfo);
							break;
						}
					}
				}
			}
		}

		// Token: 0x060057B6 RID: 22454 RVA: 0x001B3718 File Offset: 0x001B1918
		public void LoadCosmeticPartsV2(string playFabId, bool forRightSide = false)
		{
			this.ClearManuallySpawnedCosmeticParts();
			this.ClearCosmetics();
			CosmeticInfoV2 cosmeticInfo;
			if (!CosmeticsController.instance.TryGetCosmeticInfoV2(playFabId, out cosmeticInfo))
			{
				if (!(playFabId == "null") && !(playFabId == "NOTHING") && !(playFabId == "Slingshot"))
				{
					Debug.LogError("HeadModel.playFabId: Cosmetic id \"" + playFabId + "\" not found in `CosmeticsController`.", this);
				}
				return;
			}
			this.HandleLoadingAllPieces(playFabId, forRightSide, cosmeticInfo);
		}

		// Token: 0x060057B7 RID: 22455 RVA: 0x001B378C File Offset: 0x001B198C
		private void HandleLoadingAllPieces(string playFabId, bool forRightSide, CosmeticInfoV2 cosmeticInfo)
		{
			CosmeticPart[] array;
			if (cosmeticInfo.storeParts.Length != 0)
			{
				array = cosmeticInfo.storeParts;
			}
			else
			{
				if (cosmeticInfo.category == CosmeticsController.CosmeticCategory.Fur)
				{
					this.HandleLoadingFur(playFabId, forRightSide, cosmeticInfo);
					return;
				}
				if (cosmeticInfo.category == CosmeticsController.CosmeticCategory.Set)
				{
					foreach (CosmeticSO cosmeticSO in cosmeticInfo.setCosmetics)
					{
						this.HandleLoadingAllPieces(playFabId, forRightSide, cosmeticSO.info);
					}
					return;
				}
				array = cosmeticInfo.wardrobeParts;
			}
			foreach (CosmeticPart cosmeticPart in array)
			{
				foreach (CosmeticAttachInfo cosmeticAttachInfo in cosmeticPart.attachAnchors)
				{
					if ((!forRightSide || !(cosmeticAttachInfo.selectSide == ECosmeticSelectSide.Left)) && (forRightSide || !(cosmeticAttachInfo.selectSide == ECosmeticSelectSide.Right)))
					{
						HeadModel._CosmeticPartLoadInfo cosmeticPartLoadInfo = new HeadModel._CosmeticPartLoadInfo
						{
							playFabId = playFabId,
							prefabAssetRef = cosmeticPart.prefabAssetRef,
							attachInfo = cosmeticAttachInfo,
							loadOp = cosmeticPart.prefabAssetRef.InstantiateAsync(base.transform, false),
							xform = null
						};
						cosmeticPartLoadInfo.loadOp.Completed += this._HandleLoadCosmeticPartsV2;
						this._loadOp_to_partInfoIndex[cosmeticPartLoadInfo.loadOp] = this._currentPartLoadInfos.Count;
						this._currentPartLoadInfos.Add(cosmeticPartLoadInfo);
					}
				}
			}
		}

		// Token: 0x060057B8 RID: 22456 RVA: 0x001B3924 File Offset: 0x001B1B24
		private void _HandleLoadCosmeticPartsV2(AsyncOperationHandle<GameObject> loadOp)
		{
			int num;
			if (!this._loadOp_to_partInfoIndex.TryGetValue(loadOp, out num))
			{
				if (loadOp.Status == AsyncOperationStatus.Succeeded && loadOp.Result)
				{
					Object.Destroy(loadOp.Result);
				}
				return;
			}
			HeadModel._CosmeticPartLoadInfo cosmeticPartLoadInfo = this._currentPartLoadInfos[num];
			if (loadOp.Status == AsyncOperationStatus.Failed)
			{
				Debug.Log("HeadModel: Failed to load a part for cosmetic \"" + cosmeticPartLoadInfo.playFabId + "\"! Waiting for 10 seconds before trying again.", this);
				GTDelayedExec.Add(this, 10f, num);
				return;
			}
			cosmeticPartLoadInfo.xform = loadOp.Result.transform;
			this._manuallySpawnedCosmeticParts.Add(cosmeticPartLoadInfo.xform.gameObject);
			switch (this.bustType)
			{
			case HeadModel_CosmeticStand.BustType.Disabled:
				this.PositionWithWardRobeOffsets(cosmeticPartLoadInfo);
				break;
			case HeadModel_CosmeticStand.BustType.GorillaHead:
				this.PositionWithWardRobeOffsets(cosmeticPartLoadInfo);
				break;
			case HeadModel_CosmeticStand.BustType.GorillaTorso:
				this.PositionWithWardRobeOffsets(cosmeticPartLoadInfo);
				break;
			case HeadModel_CosmeticStand.BustType.GorillaTorsoPost:
				this.PositionWithWardRobeOffsets(cosmeticPartLoadInfo);
				break;
			case HeadModel_CosmeticStand.BustType.GorillaMannequin:
				this._manuallySpawnedCosmeticParts.Remove(cosmeticPartLoadInfo.xform.gameObject);
				Object.DestroyImmediate(cosmeticPartLoadInfo.xform.gameObject);
				break;
			case HeadModel_CosmeticStand.BustType.GuitarStand:
				this.PositionWardRobeItems(cosmeticPartLoadInfo);
				break;
			case HeadModel_CosmeticStand.BustType.JewelryBox:
				this.PositionWardRobeItems(cosmeticPartLoadInfo);
				break;
			case HeadModel_CosmeticStand.BustType.Table:
				this.PositionWardRobeItems(cosmeticPartLoadInfo);
				break;
			case HeadModel_CosmeticStand.BustType.PinDisplay:
				this.PositionWardRobeItems(cosmeticPartLoadInfo);
				break;
			case HeadModel_CosmeticStand.BustType.TagEffectDisplay:
				this.PositionWardRobeItems(cosmeticPartLoadInfo);
				break;
			default:
				this.PositionWithWardRobeOffsets(cosmeticPartLoadInfo);
				break;
			}
			cosmeticPartLoadInfo.xform.gameObject.SetActive(true);
		}

		// Token: 0x060057B9 RID: 22457 RVA: 0x001B3A9C File Offset: 0x001B1C9C
		private void HandleLoadingFur(string playFabId, bool forRightSide, CosmeticInfoV2 cosmeticInfo)
		{
			foreach (CosmeticPart cosmeticPart in cosmeticInfo.functionalParts)
			{
				foreach (CosmeticAttachInfo cosmeticAttachInfo in cosmeticPart.attachAnchors)
				{
					if ((!forRightSide || !(cosmeticAttachInfo.selectSide == ECosmeticSelectSide.Left)) && (forRightSide || !(cosmeticAttachInfo.selectSide == ECosmeticSelectSide.Right)))
					{
						HeadModel._CosmeticPartLoadInfo cosmeticPartLoadInfo = new HeadModel._CosmeticPartLoadInfo
						{
							playFabId = playFabId,
							prefabAssetRef = cosmeticPart.prefabAssetRef,
							attachInfo = cosmeticAttachInfo,
							loadOp = cosmeticPart.prefabAssetRef.InstantiateAsync(base.transform, false),
							xform = null
						};
						cosmeticPartLoadInfo.loadOp.Completed += this._HandleLoadCosmeticPartsV2Fur;
						this._loadOp_to_partInfoIndex[cosmeticPartLoadInfo.loadOp] = this._currentPartLoadInfos.Count;
						this._currentPartLoadInfos.Add(cosmeticPartLoadInfo);
					}
				}
			}
		}

		// Token: 0x060057BA RID: 22458 RVA: 0x001B3BBC File Offset: 0x001B1DBC
		private void _HandleLoadCosmeticPartsV2Fur(AsyncOperationHandle<GameObject> loadOp)
		{
			int num;
			if (!this._loadOp_to_partInfoIndex.TryGetValue(loadOp, out num))
			{
				if (loadOp.Status == AsyncOperationStatus.Succeeded && loadOp.Result)
				{
					Object.Destroy(loadOp.Result);
				}
				return;
			}
			HeadModel._CosmeticPartLoadInfo cosmeticPartLoadInfo = this._currentPartLoadInfos[num];
			if (loadOp.Status == AsyncOperationStatus.Failed)
			{
				Debug.Log("HeadModel: Failed to load a part for cosmetic \"" + cosmeticPartLoadInfo.playFabId + "\"! Waiting for 10 seconds before trying again.", this);
				GTDelayedExec.Add(this, 10f, num);
				return;
			}
			cosmeticPartLoadInfo.xform = loadOp.Result.transform;
			cosmeticPartLoadInfo.xform.GetComponent<GorillaSkinToggle>().ApplyToMannequin(this.mannequin);
			Object.DestroyImmediate(cosmeticPartLoadInfo.xform.gameObject);
		}

		// Token: 0x060057BB RID: 22459 RVA: 0x001B3C7B File Offset: 0x001B1E7B
		public void SetStandType(HeadModel_CosmeticStand.BustType newBustType)
		{
			this.bustType = newBustType;
		}

		// Token: 0x060057BC RID: 22460 RVA: 0x001B3C84 File Offset: 0x001B1E84
		private void PositionWardRobeItems(GameObject instantiateEdObject, HeadModel._CosmeticPartLoadInfo partLoadInfo)
		{
			Transform transform = instantiateEdObject.transform.FindChildRecursive(this.mountID);
			if (transform != null)
			{
				Debug.Log("Dynamic Cosmetics - Mount Found: " + this.mountID);
				instantiateEdObject.transform.position = base.transform.position;
				instantiateEdObject.transform.rotation = base.transform.rotation;
				instantiateEdObject.transform.localPosition = transform.localPosition;
				instantiateEdObject.transform.localRotation = transform.localRotation;
				return;
			}
			HeadModel_CosmeticStand.BustType bustType = this.bustType;
			if (bustType - HeadModel_CosmeticStand.BustType.GuitarStand <= 2 || bustType == HeadModel_CosmeticStand.BustType.TagEffectDisplay)
			{
				instantiateEdObject.transform.position = base.transform.position;
				instantiateEdObject.transform.rotation = base.transform.rotation;
				return;
			}
			this.PositionWithWardRobeOffsets(partLoadInfo);
		}

		// Token: 0x060057BD RID: 22461 RVA: 0x001B3D58 File Offset: 0x001B1F58
		private void PositionWardRobeItems(HeadModel._CosmeticPartLoadInfo partLoadInfo)
		{
			Transform transform = partLoadInfo.xform.FindChildRecursive(this.mountID);
			if (transform != null)
			{
				Debug.Log("Dynamic Cosmetics - Mount Found: " + this.mountID);
				partLoadInfo.xform.position = base.transform.position;
				partLoadInfo.xform.rotation = base.transform.rotation;
				partLoadInfo.xform.localPosition = transform.localPosition;
				partLoadInfo.xform.localRotation = transform.localRotation;
				return;
			}
			HeadModel_CosmeticStand.BustType bustType = this.bustType;
			if (bustType - HeadModel_CosmeticStand.BustType.GuitarStand <= 2 || bustType == HeadModel_CosmeticStand.BustType.TagEffectDisplay)
			{
				partLoadInfo.xform.position = base.transform.position;
				partLoadInfo.xform.rotation = base.transform.rotation;
				return;
			}
			this.PositionWithWardRobeOffsets(partLoadInfo);
		}

		// Token: 0x060057BE RID: 22462 RVA: 0x001B3E2C File Offset: 0x001B202C
		private void PositionWithWardRobeOffsets(HeadModel._CosmeticPartLoadInfo partLoadInfo)
		{
			Debug.Log("Dynamic Cosmetics - Mount Not Found: " + this.mountID);
			partLoadInfo.xform.localPosition = partLoadInfo.attachInfo.offset.pos;
			partLoadInfo.xform.localRotation = partLoadInfo.attachInfo.offset.rot;
			partLoadInfo.xform.localScale = partLoadInfo.attachInfo.offset.scale;
		}

		// Token: 0x060057BF RID: 22463 RVA: 0x001B3EA0 File Offset: 0x001B20A0
		public void ClearManuallySpawnedCosmeticParts()
		{
			foreach (GameObject obj in this._manuallySpawnedCosmeticParts)
			{
				Object.DestroyImmediate(obj);
			}
			this._manuallySpawnedCosmeticParts.Clear();
		}

		// Token: 0x060057C0 RID: 22464 RVA: 0x001B3EFC File Offset: 0x001B20FC
		public void ClearCosmetics()
		{
			this.ResetMannequinSkin();
			for (int i = base.transform.childCount - 1; i >= 0; i--)
			{
				Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
			}
		}

		// Token: 0x060057C1 RID: 22465 RVA: 0x00058615 File Offset: 0x00056815
		private GameObject LoadAndInstantiatePrefab(GTAssetRef<GameObject> prefabAssetRef, Transform parent)
		{
			return null;
		}

		// Token: 0x060057C2 RID: 22466 RVA: 0x000023F5 File Offset: 0x000005F5
		public void UpdateCosmeticsMountPositions(CosmeticSO findCosmeticInAllCosmeticsArraySO)
		{
		}

		// Token: 0x0400617C RID: 24956
		public HeadModel_CosmeticStand.BustType bustType = HeadModel_CosmeticStand.BustType.JewelryBox;

		// Token: 0x0400617D RID: 24957
		[SerializeField]
		private List<GameObject> _manuallySpawnedCosmeticParts = new List<GameObject>();

		// Token: 0x0400617E RID: 24958
		public GameObject mannequin;

		// Token: 0x0400617F RID: 24959
		public Material defaultMannequinFace;

		// Token: 0x04006180 RID: 24960
		public Material defaultMannequinChest;

		// Token: 0x04006181 RID: 24961
		public Material defaultMannequinBody;

		// Token: 0x04006182 RID: 24962
		[DebugReadout]
		protected new readonly List<HeadModel._CosmeticPartLoadInfo> _currentPartLoadInfos = new List<HeadModel._CosmeticPartLoadInfo>(1);

		// Token: 0x04006183 RID: 24963
		[DebugReadout]
		private readonly Dictionary<AsyncOperationHandle, int> _loadOp_to_partInfoIndex = new Dictionary<AsyncOperationHandle, int>(1);

		// Token: 0x02000DCE RID: 3534
		public enum BustType
		{
			// Token: 0x04006185 RID: 24965
			Disabled,
			// Token: 0x04006186 RID: 24966
			GorillaHead,
			// Token: 0x04006187 RID: 24967
			GorillaTorso,
			// Token: 0x04006188 RID: 24968
			GorillaTorsoPost,
			// Token: 0x04006189 RID: 24969
			GorillaMannequin,
			// Token: 0x0400618A RID: 24970
			GuitarStand,
			// Token: 0x0400618B RID: 24971
			JewelryBox,
			// Token: 0x0400618C RID: 24972
			Table,
			// Token: 0x0400618D RID: 24973
			PinDisplay,
			// Token: 0x0400618E RID: 24974
			TagEffectDisplay
		}
	}
}
