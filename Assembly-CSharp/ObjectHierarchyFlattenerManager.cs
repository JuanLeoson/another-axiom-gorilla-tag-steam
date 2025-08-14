using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000214 RID: 532
public class ObjectHierarchyFlattenerManager : MonoBehaviour
{
	// Token: 0x06000C77 RID: 3191 RVA: 0x00043696 File Offset: 0x00041896
	protected void Awake()
	{
		if (ObjectHierarchyFlattenerManager.hasInstance && ObjectHierarchyFlattenerManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		ObjectHierarchyFlattenerManager.SetInstance(this);
	}

	// Token: 0x06000C78 RID: 3192 RVA: 0x000436B9 File Offset: 0x000418B9
	public static void CreateManager()
	{
		ObjectHierarchyFlattenerManager.SetInstance(new GameObject("ObjectHierarchyFlattenerManager").AddComponent<ObjectHierarchyFlattenerManager>());
	}

	// Token: 0x06000C79 RID: 3193 RVA: 0x000436CF File Offset: 0x000418CF
	private static void SetInstance(ObjectHierarchyFlattenerManager manager)
	{
		ObjectHierarchyFlattenerManager.instance = manager;
		ObjectHierarchyFlattenerManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06000C7A RID: 3194 RVA: 0x000436EA File Offset: 0x000418EA
	public static void RegisterOHF(ObjectHierarchyFlattener rbWI)
	{
		if (!ObjectHierarchyFlattenerManager.hasInstance)
		{
			ObjectHierarchyFlattenerManager.CreateManager();
		}
		if (!ObjectHierarchyFlattenerManager.alloHF.Contains(rbWI))
		{
			ObjectHierarchyFlattenerManager.alloHF.Add(rbWI);
		}
	}

	// Token: 0x06000C7B RID: 3195 RVA: 0x00043710 File Offset: 0x00041910
	public static void UnregisterOHF(ObjectHierarchyFlattener rbWI)
	{
		if (!ObjectHierarchyFlattenerManager.hasInstance)
		{
			ObjectHierarchyFlattenerManager.CreateManager();
		}
		if (ObjectHierarchyFlattenerManager.alloHF.Contains(rbWI))
		{
			ObjectHierarchyFlattenerManager.alloHF.Remove(rbWI);
		}
	}

	// Token: 0x06000C7C RID: 3196 RVA: 0x00043738 File Offset: 0x00041938
	public void LateUpdate()
	{
		for (int i = 0; i < ObjectHierarchyFlattenerManager.alloHF.Count; i++)
		{
			ObjectHierarchyFlattenerManager.alloHF[i].InvokeLateUpdate();
		}
	}

	// Token: 0x04000F7A RID: 3962
	public static ObjectHierarchyFlattenerManager instance;

	// Token: 0x04000F7B RID: 3963
	[OnEnterPlay_Set(false)]
	public static bool hasInstance = false;

	// Token: 0x04000F7C RID: 3964
	public static List<ObjectHierarchyFlattener> alloHF = new List<ObjectHierarchyFlattener>();
}
