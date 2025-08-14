using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200077B RID: 1915
public class SizeManagerManager : MonoBehaviour
{
	// Token: 0x06003000 RID: 12288 RVA: 0x000FC870 File Offset: 0x000FAA70
	protected void Awake()
	{
		if (SizeManagerManager.hasInstance && SizeManagerManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		SizeManagerManager.SetInstance(this);
	}

	// Token: 0x06003001 RID: 12289 RVA: 0x000FC893 File Offset: 0x000FAA93
	public static void CreateManager()
	{
		SizeManagerManager.SetInstance(new GameObject("SizeManagerManager").AddComponent<SizeManagerManager>());
	}

	// Token: 0x06003002 RID: 12290 RVA: 0x000FC8A9 File Offset: 0x000FAAA9
	private static void SetInstance(SizeManagerManager manager)
	{
		SizeManagerManager.instance = manager;
		SizeManagerManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06003003 RID: 12291 RVA: 0x000FC8C4 File Offset: 0x000FAAC4
	public static void RegisterSM(SizeManager sM)
	{
		if (!SizeManagerManager.hasInstance)
		{
			SizeManagerManager.CreateManager();
		}
		if (!SizeManagerManager.allSM.Contains(sM))
		{
			SizeManagerManager.allSM.Add(sM);
		}
	}

	// Token: 0x06003004 RID: 12292 RVA: 0x000FC8EA File Offset: 0x000FAAEA
	public static void UnregisterSM(SizeManager sM)
	{
		if (!SizeManagerManager.hasInstance)
		{
			SizeManagerManager.CreateManager();
		}
		if (SizeManagerManager.allSM.Contains(sM))
		{
			SizeManagerManager.allSM.Remove(sM);
		}
	}

	// Token: 0x06003005 RID: 12293 RVA: 0x000FC914 File Offset: 0x000FAB14
	public void FixedUpdate()
	{
		for (int i = 0; i < SizeManagerManager.allSM.Count; i++)
		{
			SizeManagerManager.allSM[i].InvokeFixedUpdate();
		}
	}

	// Token: 0x04003C19 RID: 15385
	[OnEnterPlay_SetNull]
	public static SizeManagerManager instance;

	// Token: 0x04003C1A RID: 15386
	[OnEnterPlay_Set(false)]
	public static bool hasInstance = false;

	// Token: 0x04003C1B RID: 15387
	[OnEnterPlay_Clear]
	public static List<SizeManager> allSM = new List<SizeManager>();
}
