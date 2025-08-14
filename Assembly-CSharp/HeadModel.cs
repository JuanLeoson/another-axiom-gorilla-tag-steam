using System;
using System.Collections.Generic;
using Cysharp.Text;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

// Token: 0x0200042B RID: 1067
public class HeadModel : MonoBehaviour, IDelayedExecListener
{
	// Token: 0x060019CE RID: 6606 RVA: 0x0008A6B8 File Offset: 0x000888B8
	protected void Awake()
	{
		this.RefreshRenderer();
	}

	// Token: 0x060019CF RID: 6607 RVA: 0x0008A6C0 File Offset: 0x000888C0
	protected void RefreshRenderer()
	{
		this._mannequinRenderer = base.GetComponentInChildren<Renderer>(true);
	}

	// Token: 0x060019D0 RID: 6608 RVA: 0x0008A6CF File Offset: 0x000888CF
	public void SetCosmeticActive(string playFabId, bool forRightSide = false)
	{
		this._ClearCurrent();
		this._AddPreviewCosmetic(playFabId, forRightSide);
	}

	// Token: 0x060019D1 RID: 6609 RVA: 0x0008A6E0 File Offset: 0x000888E0
	public void SetCosmeticActiveArray(string[] playFabIds, bool[] forRightSideArray)
	{
		this._ClearCurrent();
		for (int i = 0; i < playFabIds.Length; i++)
		{
			this._AddPreviewCosmetic(playFabIds[i], forRightSideArray[i]);
		}
	}

	// Token: 0x060019D2 RID: 6610 RVA: 0x0008A710 File Offset: 0x00088910
	private void _AddPreviewCosmetic(string playFabId, bool forRightSide)
	{
		CosmeticInfoV2 cosmeticInfoV;
		if (!CosmeticsController.instance.TryGetCosmeticInfoV2(playFabId, out cosmeticInfoV))
		{
			if (!(playFabId == "null") && !(playFabId == "NOTHING") && !(playFabId == "Slingshot"))
			{
				Debug.LogError(ZString.Concat<string, string, string>("HeadModel._AddPreviewCosmetic: Cosmetic id \"", playFabId, "\" not found in `CosmeticsController`."), this);
			}
			return;
		}
		if (cosmeticInfoV.hideWardrobeMannequin)
		{
			if (this._mannequinRenderer.IsNull())
			{
				this.RefreshRenderer();
			}
			if (this._mannequinRenderer.IsNotNull())
			{
				this._mannequinRenderer.enabled = false;
			}
		}
		foreach (CosmeticPart cosmeticPart in cosmeticInfoV.wardrobeParts)
		{
			if (!cosmeticPart.prefabAssetRef.RuntimeKeyIsValid())
			{
				GTDev.LogError<string>("Cosmetic " + cosmeticInfoV.displayName + " has missing object reference in wardrobe parts, skipping load", null);
			}
			else
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
						cosmeticPartLoadInfo.loadOp.Completed += this._HandleLoadOpOnCompleted;
						this._loadOp_to_partInfoIndex[cosmeticPartLoadInfo.loadOp] = this._currentPartLoadInfos.Count;
						this._currentPartLoadInfos.Add(cosmeticPartLoadInfo);
					}
				}
			}
		}
	}

	// Token: 0x060019D3 RID: 6611 RVA: 0x0008A8E4 File Offset: 0x00088AE4
	private void _HandleLoadOpOnCompleted(AsyncOperationHandle<GameObject> loadOp)
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
		cosmeticPartLoadInfo.xform.localPosition = cosmeticPartLoadInfo.attachInfo.offset.pos;
		cosmeticPartLoadInfo.xform.localRotation = cosmeticPartLoadInfo.attachInfo.offset.rot;
		cosmeticPartLoadInfo.xform.localScale = cosmeticPartLoadInfo.attachInfo.offset.scale;
		cosmeticPartLoadInfo.xform.gameObject.SetActive(true);
	}

	// Token: 0x060019D4 RID: 6612 RVA: 0x0008A9E0 File Offset: 0x00088BE0
	void IDelayedExecListener.OnDelayedAction(int partLoadInfosIndex)
	{
		if (partLoadInfosIndex < 0 || partLoadInfosIndex >= this._currentPartLoadInfos.Count)
		{
			return;
		}
		HeadModel._CosmeticPartLoadInfo cosmeticPartLoadInfo = this._currentPartLoadInfos[partLoadInfosIndex];
		if (cosmeticPartLoadInfo.loadOp.Status != AsyncOperationStatus.Failed)
		{
			return;
		}
		cosmeticPartLoadInfo.loadOp.Completed += this._HandleLoadOpOnCompleted;
		cosmeticPartLoadInfo.loadOp = cosmeticPartLoadInfo.prefabAssetRef.InstantiateAsync(base.transform, false);
		this._loadOp_to_partInfoIndex[cosmeticPartLoadInfo.loadOp] = partLoadInfosIndex;
	}

	// Token: 0x060019D5 RID: 6613 RVA: 0x0008AA68 File Offset: 0x00088C68
	protected void _ClearCurrent()
	{
		for (int i = 0; i < this._currentPartLoadInfos.Count; i++)
		{
			Object.Destroy(this._currentPartLoadInfos[i].loadOp.Result);
		}
		this._EnsureCapacityAndClear<AsyncOperationHandle, int>(this._loadOp_to_partInfoIndex);
		this._EnsureCapacityAndClear<HeadModel._CosmeticPartLoadInfo>(this._currentPartLoadInfos);
		if (this._mannequinRenderer.IsNull())
		{
			this.RefreshRenderer();
		}
		this._mannequinRenderer.enabled = true;
	}

	// Token: 0x060019D6 RID: 6614 RVA: 0x0008AAE0 File Offset: 0x00088CE0
	private void _EnsureCapacityAndClear<T>(List<T> list)
	{
		if (list.Count > list.Capacity)
		{
			list.Capacity = list.Count;
		}
		list.Clear();
	}

	// Token: 0x060019D7 RID: 6615 RVA: 0x0008AB02 File Offset: 0x00088D02
	private void _EnsureCapacityAndClear<T1, T2>(Dictionary<T1, T2> dict)
	{
		dict.EnsureCapacity(dict.Count);
		dict.Clear();
	}

	// Token: 0x04002227 RID: 8743
	[DebugReadout]
	protected readonly List<HeadModel._CosmeticPartLoadInfo> _currentPartLoadInfos = new List<HeadModel._CosmeticPartLoadInfo>(1);

	// Token: 0x04002228 RID: 8744
	[DebugReadout]
	private readonly Dictionary<AsyncOperationHandle, int> _loadOp_to_partInfoIndex = new Dictionary<AsyncOperationHandle, int>(1);

	// Token: 0x04002229 RID: 8745
	private Renderer _mannequinRenderer;

	// Token: 0x0400222A RID: 8746
	public GameObject[] cosmetics;

	// Token: 0x0200042C RID: 1068
	protected struct _CosmeticPartLoadInfo
	{
		// Token: 0x0400222B RID: 8747
		public string playFabId;

		// Token: 0x0400222C RID: 8748
		public GTAssetRef<GameObject> prefabAssetRef;

		// Token: 0x0400222D RID: 8749
		public CosmeticAttachInfo attachInfo;

		// Token: 0x0400222E RID: 8750
		public AsyncOperationHandle<GameObject> loadOp;

		// Token: 0x0400222F RID: 8751
		public Transform xform;
	}
}
