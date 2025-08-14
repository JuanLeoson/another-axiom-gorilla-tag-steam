using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000431 RID: 1073
[DefaultExecutionOrder(2000)]
public class ChestObjectHysteresisManager : MonoBehaviour
{
	// Token: 0x06001A14 RID: 6676 RVA: 0x0008BD5A File Offset: 0x00089F5A
	protected void Awake()
	{
		if (ChestObjectHysteresisManager.hasInstance && ChestObjectHysteresisManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		ChestObjectHysteresisManager.SetInstance(this);
	}

	// Token: 0x06001A15 RID: 6677 RVA: 0x0008BD7D File Offset: 0x00089F7D
	public static void CreateManager()
	{
		ChestObjectHysteresisManager.SetInstance(new GameObject("ChestObjectHysteresisManager").AddComponent<ChestObjectHysteresisManager>());
	}

	// Token: 0x06001A16 RID: 6678 RVA: 0x0008BD93 File Offset: 0x00089F93
	private static void SetInstance(ChestObjectHysteresisManager manager)
	{
		ChestObjectHysteresisManager.instance = manager;
		ChestObjectHysteresisManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06001A17 RID: 6679 RVA: 0x0008BDAE File Offset: 0x00089FAE
	public static void RegisterCH(ChestObjectHysteresis cOH)
	{
		if (!ChestObjectHysteresisManager.hasInstance)
		{
			ChestObjectHysteresisManager.CreateManager();
		}
		if (!ChestObjectHysteresisManager.allChests.Contains(cOH))
		{
			ChestObjectHysteresisManager.allChests.Add(cOH);
		}
	}

	// Token: 0x06001A18 RID: 6680 RVA: 0x0008BDD4 File Offset: 0x00089FD4
	public static void UnregisterCH(ChestObjectHysteresis cOH)
	{
		if (!ChestObjectHysteresisManager.hasInstance)
		{
			ChestObjectHysteresisManager.CreateManager();
		}
		if (ChestObjectHysteresisManager.allChests.Contains(cOH))
		{
			ChestObjectHysteresisManager.allChests.Remove(cOH);
		}
	}

	// Token: 0x06001A19 RID: 6681 RVA: 0x0008BDFC File Offset: 0x00089FFC
	public void Update()
	{
		for (int i = 0; i < ChestObjectHysteresisManager.allChests.Count; i++)
		{
			ChestObjectHysteresisManager.allChests[i].InvokeUpdate();
		}
	}

	// Token: 0x04002255 RID: 8789
	public static ChestObjectHysteresisManager instance;

	// Token: 0x04002256 RID: 8790
	public static bool hasInstance = false;

	// Token: 0x04002257 RID: 8791
	public static List<ChestObjectHysteresis> allChests = new List<ChestObjectHysteresis>();
}
