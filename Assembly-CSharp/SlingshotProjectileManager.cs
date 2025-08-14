using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003E9 RID: 1001
public class SlingshotProjectileManager : MonoBehaviour
{
	// Token: 0x0600177E RID: 6014 RVA: 0x0007EE3B File Offset: 0x0007D03B
	protected void Awake()
	{
		if (SlingshotProjectileManager.hasInstance && SlingshotProjectileManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		SlingshotProjectileManager.SetInstance(this);
	}

	// Token: 0x0600177F RID: 6015 RVA: 0x0007EE5E File Offset: 0x0007D05E
	public static void CreateManager()
	{
		SlingshotProjectileManager.SetInstance(new GameObject("SlingshotProjectileManager").AddComponent<SlingshotProjectileManager>());
	}

	// Token: 0x06001780 RID: 6016 RVA: 0x0007EE74 File Offset: 0x0007D074
	private static void SetInstance(SlingshotProjectileManager manager)
	{
		SlingshotProjectileManager.instance = manager;
		SlingshotProjectileManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06001781 RID: 6017 RVA: 0x0007EE8F File Offset: 0x0007D08F
	public static void RegisterSP(SlingshotProjectile sP)
	{
		if (!SlingshotProjectileManager.hasInstance)
		{
			SlingshotProjectileManager.CreateManager();
		}
		if (!SlingshotProjectileManager.allsP.Contains(sP))
		{
			SlingshotProjectileManager.allsP.Add(sP);
		}
	}

	// Token: 0x06001782 RID: 6018 RVA: 0x0007EEB5 File Offset: 0x0007D0B5
	public static void UnregisterSP(SlingshotProjectile sP)
	{
		if (!SlingshotProjectileManager.hasInstance)
		{
			SlingshotProjectileManager.CreateManager();
		}
		if (SlingshotProjectileManager.allsP.Contains(sP))
		{
			SlingshotProjectileManager.allsP.Remove(sP);
		}
	}

	// Token: 0x06001783 RID: 6019 RVA: 0x0007EEDC File Offset: 0x0007D0DC
	public void Update()
	{
		for (int i = 0; i < SlingshotProjectileManager.allsP.Count; i++)
		{
			SlingshotProjectileManager.allsP[i].InvokeUpdate();
		}
	}

	// Token: 0x04001F5E RID: 8030
	public static SlingshotProjectileManager instance;

	// Token: 0x04001F5F RID: 8031
	public static bool hasInstance = false;

	// Token: 0x04001F60 RID: 8032
	public static List<SlingshotProjectile> allsP = new List<SlingshotProjectile>();
}
