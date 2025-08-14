using System;
using System.Diagnostics;
using UnityEngine;

namespace BuildSafe
{
	// Token: 0x02000CE1 RID: 3297
	public static class AssetDatabase
	{
		// Token: 0x060051F0 RID: 20976 RVA: 0x00198414 File Offset: 0x00196614
		public static T LoadAssetAtPath<T>(string assetPath) where T : Object
		{
			return default(T);
		}

		// Token: 0x060051F1 RID: 20977 RVA: 0x0019842A File Offset: 0x0019662A
		public static T[] LoadAssetsOfType<T>() where T : Object
		{
			return Array.Empty<T>();
		}

		// Token: 0x060051F2 RID: 20978 RVA: 0x00198431 File Offset: 0x00196631
		public static string[] FindAssetsOfType<T>() where T : Object
		{
			return Array.Empty<string>();
		}

		// Token: 0x060051F3 RID: 20979 RVA: 0x00198438 File Offset: 0x00196638
		[Conditional("UNITY_EDITOR")]
		public static void SaveToDisk(params Object[] assetsToSave)
		{
			AssetDatabase.SaveAssetsToDisk(assetsToSave, true);
		}

		// Token: 0x060051F4 RID: 20980 RVA: 0x000023F5 File Offset: 0x000005F5
		public static void SaveAssetsToDisk(Object[] assetsToSave, bool saveProject = true)
		{
		}
	}
}
