using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200026D RID: 621
public static class SceneIndexExtensions
{
	// Token: 0x06000E54 RID: 3668 RVA: 0x0005790F File Offset: 0x00055B0F
	public static SceneIndex GetSceneIndex(this Scene scene)
	{
		return (SceneIndex)scene.buildIndex;
	}

	// Token: 0x06000E55 RID: 3669 RVA: 0x00057918 File Offset: 0x00055B18
	public static SceneIndex GetSceneIndex(this GameObject obj)
	{
		return (SceneIndex)obj.scene.buildIndex;
	}

	// Token: 0x06000E56 RID: 3670 RVA: 0x00057934 File Offset: 0x00055B34
	public static SceneIndex GetSceneIndex(this Component cmp)
	{
		return (SceneIndex)cmp.gameObject.scene.buildIndex;
	}

	// Token: 0x06000E57 RID: 3671 RVA: 0x00057954 File Offset: 0x00055B54
	public static void AddCallbackOnSceneLoad(this SceneIndex scene, Action callback)
	{
		if (SceneIndexExtensions.onSceneLoadCallbacks == null)
		{
			SceneIndexExtensions.onSceneLoadCallbacks = new List<Action>[18];
			for (int i = 0; i < SceneIndexExtensions.onSceneLoadCallbacks.Length; i++)
			{
				SceneIndexExtensions.onSceneLoadCallbacks[i] = new List<Action>();
			}
			SceneManager.sceneLoaded += SceneIndexExtensions.OnSceneLoad;
		}
		SceneIndexExtensions.onSceneLoadCallbacks[(int)scene].Add(callback);
	}

	// Token: 0x06000E58 RID: 3672 RVA: 0x000579B0 File Offset: 0x00055BB0
	public static void RemoveCallbackOnSceneLoad(this SceneIndex scene, Action callback)
	{
		SceneIndexExtensions.onSceneLoadCallbacks[(int)scene].Remove(callback);
	}

	// Token: 0x06000E59 RID: 3673 RVA: 0x000579C0 File Offset: 0x00055BC0
	public static void OnSceneLoad(Scene scene, LoadSceneMode mode)
	{
		if (scene.buildIndex != -1)
		{
			foreach (Action action in SceneIndexExtensions.onSceneLoadCallbacks[scene.buildIndex])
			{
				action();
			}
		}
	}

	// Token: 0x06000E5A RID: 3674 RVA: 0x00057A24 File Offset: 0x00055C24
	public static void AddCallbackOnSceneUnload(this SceneIndex scene, Action callback)
	{
		if (SceneIndexExtensions.onSceneUnloadCallbacks == null)
		{
			SceneIndexExtensions.onSceneUnloadCallbacks = new List<Action>[18];
			for (int i = 0; i < SceneIndexExtensions.onSceneUnloadCallbacks.Length; i++)
			{
				SceneIndexExtensions.onSceneUnloadCallbacks[i] = new List<Action>();
			}
			SceneManager.sceneUnloaded += SceneIndexExtensions.OnSceneUnload;
		}
		SceneIndexExtensions.onSceneUnloadCallbacks[(int)scene].Add(callback);
	}

	// Token: 0x06000E5B RID: 3675 RVA: 0x00057A80 File Offset: 0x00055C80
	public static void RemoveCallbackOnSceneUnload(this SceneIndex scene, Action callback)
	{
		SceneIndexExtensions.onSceneUnloadCallbacks[(int)scene].Remove(callback);
	}

	// Token: 0x06000E5C RID: 3676 RVA: 0x00057A90 File Offset: 0x00055C90
	public static void OnSceneUnload(Scene scene)
	{
		if (scene.buildIndex != -1)
		{
			foreach (Action action in SceneIndexExtensions.onSceneUnloadCallbacks[scene.buildIndex])
			{
				action();
			}
		}
	}

	// Token: 0x06000E5D RID: 3677 RVA: 0x00057AF4 File Offset: 0x00055CF4
	[OnEnterPlay_Run]
	private static void Reset()
	{
		if (SceneIndexExtensions.onSceneLoadCallbacks != null)
		{
			SceneIndexExtensions.onSceneLoadCallbacks = null;
			SceneManager.sceneLoaded -= SceneIndexExtensions.OnSceneLoad;
		}
		if (SceneIndexExtensions.onSceneUnloadCallbacks != null)
		{
			SceneIndexExtensions.onSceneUnloadCallbacks = null;
			SceneManager.sceneUnloaded -= SceneIndexExtensions.OnSceneUnload;
		}
	}

	// Token: 0x04001743 RID: 5955
	private const int SceneIndex_COUNT = 18;

	// Token: 0x04001744 RID: 5956
	[OnEnterPlay_SetNull]
	private static List<Action>[] onSceneLoadCallbacks;

	// Token: 0x04001745 RID: 5957
	[OnEnterPlay_SetNull]
	private static List<Action>[] onSceneUnloadCallbacks;
}
