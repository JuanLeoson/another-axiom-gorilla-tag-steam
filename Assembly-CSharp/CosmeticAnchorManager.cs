using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000433 RID: 1075
public class CosmeticAnchorManager : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06001A24 RID: 6692 RVA: 0x0008C004 File Offset: 0x0008A204
	protected void Awake()
	{
		if (CosmeticAnchorManager.hasInstance && CosmeticAnchorManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		CosmeticAnchorManager.SetInstance(this);
	}

	// Token: 0x06001A25 RID: 6693 RVA: 0x0008C027 File Offset: 0x0008A227
	public static void CreateManager()
	{
		CosmeticAnchorManager.SetInstance(new GameObject("CosmeticAnchorManager").AddComponent<CosmeticAnchorManager>());
	}

	// Token: 0x06001A26 RID: 6694 RVA: 0x0008C03D File Offset: 0x0008A23D
	private static void SetInstance(CosmeticAnchorManager manager)
	{
		CosmeticAnchorManager.instance = manager;
		CosmeticAnchorManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06001A27 RID: 6695 RVA: 0x0008C058 File Offset: 0x0008A258
	public static void RegisterCosmeticAnchor(CosmeticAnchors cA)
	{
		if (!CosmeticAnchorManager.hasInstance)
		{
			CosmeticAnchorManager.CreateManager();
		}
		if ((cA.AffectedByHunt() || cA.AffectedByBuilder()) && !CosmeticAnchorManager.allAnchors.Contains(cA))
		{
			CosmeticAnchorManager.allAnchors.Add(cA);
		}
	}

	// Token: 0x06001A28 RID: 6696 RVA: 0x0008C08E File Offset: 0x0008A28E
	public static void UnregisterCosmeticAnchor(CosmeticAnchors cA)
	{
		if (!CosmeticAnchorManager.hasInstance)
		{
			CosmeticAnchorManager.CreateManager();
		}
		if ((cA.AffectedByHunt() || cA.AffectedByBuilder()) && CosmeticAnchorManager.allAnchors.Contains(cA))
		{
			CosmeticAnchorManager.allAnchors.Remove(cA);
		}
	}

	// Token: 0x06001A29 RID: 6697 RVA: 0x000172AD File Offset: 0x000154AD
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001A2A RID: 6698 RVA: 0x000172B6 File Offset: 0x000154B6
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001A2B RID: 6699 RVA: 0x0008C0C8 File Offset: 0x0008A2C8
	public void SliceUpdate()
	{
		for (int i = 0; i < CosmeticAnchorManager.allAnchors.Count; i++)
		{
			CosmeticAnchorManager.allAnchors[i].TryUpdate();
		}
	}

	// Token: 0x04002260 RID: 8800
	public static CosmeticAnchorManager instance;

	// Token: 0x04002261 RID: 8801
	public static bool hasInstance = false;

	// Token: 0x04002262 RID: 8802
	public static List<CosmeticAnchors> allAnchors = new List<CosmeticAnchors>();
}
