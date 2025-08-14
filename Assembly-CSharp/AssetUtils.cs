using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x02000A91 RID: 2705
public static class AssetUtils
{
	// Token: 0x060041A6 RID: 16806 RVA: 0x000023F5 File Offset: 0x000005F5
	[Conditional("UNITY_EDITOR")]
	public static void ExecAndUnloadUnused(Action action)
	{
	}

	// Token: 0x060041A7 RID: 16807 RVA: 0x0014B2BE File Offset: 0x001494BE
	[Conditional("UNITY_EDITOR")]
	public static void LoadAssetOfType<T>(ref T result, ref string resultPath) where T : Object
	{
		result = default(T);
		resultPath = null;
	}

	// Token: 0x060041A8 RID: 16808 RVA: 0x0014B2CA File Offset: 0x001494CA
	[Conditional("UNITY_EDITOR")]
	public static void FindAllAssetsOfType<T>(ref T[] results, ref string[] assetPaths) where T : Object
	{
		results = Array.Empty<T>();
	}

	// Token: 0x060041A9 RID: 16809 RVA: 0x000023F5 File Offset: 0x000005F5
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void ForceSave<T>(this IList<T> assets, Action<T> onPreSave = null, bool unloadUnusedAfter = false) where T : Object
	{
	}

	// Token: 0x060041AA RID: 16810 RVA: 0x000023F5 File Offset: 0x000005F5
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void ForceSave(this Object asset)
	{
	}

	// Token: 0x060041AB RID: 16811 RVA: 0x0014B2D3 File Offset: 0x001494D3
	public static long ComputeAssetId(this Object asset, bool unsigned = false)
	{
		return 0L;
	}
}
