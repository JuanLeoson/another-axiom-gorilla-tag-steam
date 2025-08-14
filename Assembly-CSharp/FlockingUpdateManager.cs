using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200058C RID: 1420
public class FlockingUpdateManager : MonoBehaviour
{
	// Token: 0x060022A4 RID: 8868 RVA: 0x000BB668 File Offset: 0x000B9868
	protected void Awake()
	{
		if (FlockingUpdateManager.hasInstance && FlockingUpdateManager.instance != null && FlockingUpdateManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		FlockingUpdateManager.SetInstance(this);
	}

	// Token: 0x060022A5 RID: 8869 RVA: 0x000BB698 File Offset: 0x000B9898
	public static void CreateManager()
	{
		FlockingUpdateManager.SetInstance(new GameObject("FlockingUpdateManager").AddComponent<FlockingUpdateManager>());
	}

	// Token: 0x060022A6 RID: 8870 RVA: 0x000BB6AE File Offset: 0x000B98AE
	private static void SetInstance(FlockingUpdateManager manager)
	{
		FlockingUpdateManager.instance = manager;
		FlockingUpdateManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x060022A7 RID: 8871 RVA: 0x000BB6C9 File Offset: 0x000B98C9
	public static void RegisterFlocking(Flocking flocking)
	{
		if (!FlockingUpdateManager.hasInstance)
		{
			FlockingUpdateManager.CreateManager();
		}
		if (!FlockingUpdateManager.allFlockings.Contains(flocking))
		{
			FlockingUpdateManager.allFlockings.Add(flocking);
		}
	}

	// Token: 0x060022A8 RID: 8872 RVA: 0x000BB6EF File Offset: 0x000B98EF
	public static void UnregisterFlocking(Flocking flocking)
	{
		if (!FlockingUpdateManager.hasInstance)
		{
			FlockingUpdateManager.CreateManager();
		}
		if (FlockingUpdateManager.allFlockings.Contains(flocking))
		{
			FlockingUpdateManager.allFlockings.Remove(flocking);
		}
	}

	// Token: 0x060022A9 RID: 8873 RVA: 0x000BB718 File Offset: 0x000B9918
	public void Update()
	{
		for (int i = 0; i < FlockingUpdateManager.allFlockings.Count; i++)
		{
			FlockingUpdateManager.allFlockings[i].InvokeUpdate();
		}
	}

	// Token: 0x04002C52 RID: 11346
	public static FlockingUpdateManager instance;

	// Token: 0x04002C53 RID: 11347
	public static bool hasInstance = false;

	// Token: 0x04002C54 RID: 11348
	public static List<Flocking> allFlockings = new List<Flocking>();
}
