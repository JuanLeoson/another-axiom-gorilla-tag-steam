using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x020003FC RID: 1020
public class WorldShareableItemManager : MonoBehaviour
{
	// Token: 0x060017D7 RID: 6103 RVA: 0x0007FED6 File Offset: 0x0007E0D6
	protected void Awake()
	{
		if (WorldShareableItemManager.hasInstance && WorldShareableItemManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		WorldShareableItemManager.SetInstance(this);
	}

	// Token: 0x060017D8 RID: 6104 RVA: 0x0007FEF9 File Offset: 0x0007E0F9
	protected void OnDestroy()
	{
		if (WorldShareableItemManager.instance == this)
		{
			WorldShareableItemManager.hasInstance = false;
			WorldShareableItemManager.instance = null;
		}
	}

	// Token: 0x060017D9 RID: 6105 RVA: 0x0007FF14 File Offset: 0x0007E114
	protected void Update()
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		for (int i = 0; i < WorldShareableItemManager.worldShareableItems.Count; i++)
		{
			if (WorldShareableItemManager.worldShareableItems[i] != null)
			{
				WorldShareableItemManager.worldShareableItems[i].TriggeredUpdate();
			}
		}
	}

	// Token: 0x060017DA RID: 6106 RVA: 0x0007FF61 File Offset: 0x0007E161
	public static void CreateManager()
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		WorldShareableItemManager.SetInstance(new GameObject("WorldShareableItemManager").AddComponent<WorldShareableItemManager>());
	}

	// Token: 0x060017DB RID: 6107 RVA: 0x0007FF7F File Offset: 0x0007E17F
	private static void SetInstance(WorldShareableItemManager manager)
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		WorldShareableItemManager.instance = manager;
		WorldShareableItemManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x060017DC RID: 6108 RVA: 0x0007FFA2 File Offset: 0x0007E1A2
	public static void Register(WorldShareableItem worldShareableItem)
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		if (!WorldShareableItemManager.hasInstance)
		{
			WorldShareableItemManager.CreateManager();
		}
		if (!WorldShareableItemManager.worldShareableItems.Contains(worldShareableItem))
		{
			WorldShareableItemManager.worldShareableItems.Add(worldShareableItem);
		}
	}

	// Token: 0x060017DD RID: 6109 RVA: 0x0007FFD0 File Offset: 0x0007E1D0
	public static void Unregister(WorldShareableItem worldShareableItem)
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		if (!WorldShareableItemManager.hasInstance)
		{
			WorldShareableItemManager.CreateManager();
		}
		if (WorldShareableItemManager.worldShareableItems.Contains(worldShareableItem))
		{
			WorldShareableItemManager.worldShareableItems.Remove(worldShareableItem);
		}
	}

	// Token: 0x04001FA2 RID: 8098
	public static WorldShareableItemManager instance;

	// Token: 0x04001FA3 RID: 8099
	private static bool hasInstance = false;

	// Token: 0x04001FA4 RID: 8100
	public static readonly List<WorldShareableItem> worldShareableItems = new List<WorldShareableItem>(1024);
}
