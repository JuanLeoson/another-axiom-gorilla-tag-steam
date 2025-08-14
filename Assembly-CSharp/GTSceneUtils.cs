using System;
using System.Diagnostics;
using UnityEngine.SceneManagement;

// Token: 0x02000B0F RID: 2831
public static class GTSceneUtils
{
	// Token: 0x0600442C RID: 17452 RVA: 0x000023F5 File Offset: 0x000005F5
	[Conditional("UNITY_EDITOR")]
	public static void AddToBuild(GTScene scene)
	{
	}

	// Token: 0x0600442D RID: 17453 RVA: 0x00155AF1 File Offset: 0x00153CF1
	public static bool Equals(GTScene x, Scene y)
	{
		return !(x == null) && y.IsValid() && x.Equals(y);
	}

	// Token: 0x0600442E RID: 17454 RVA: 0x00155B15 File Offset: 0x00153D15
	public static GTScene[] ScenesInBuild()
	{
		return Array.Empty<GTScene>();
	}

	// Token: 0x0600442F RID: 17455 RVA: 0x000023F5 File Offset: 0x000005F5
	[Conditional("UNITY_EDITOR")]
	public static void SyncBuildScenes()
	{
	}
}
