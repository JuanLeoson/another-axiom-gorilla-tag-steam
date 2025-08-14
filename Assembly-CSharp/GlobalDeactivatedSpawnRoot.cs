using System;
using UnityEngine;

// Token: 0x020001CF RID: 463
public static class GlobalDeactivatedSpawnRoot
{
	// Token: 0x06000B7D RID: 2941 RVA: 0x0003FF08 File Offset: 0x0003E108
	public static Transform GetOrCreate()
	{
		if (!GlobalDeactivatedSpawnRoot._xform)
		{
			GlobalDeactivatedSpawnRoot._xform = new GameObject("GlobalDeactivatedSpawnRoot").transform;
			GlobalDeactivatedSpawnRoot._xform.gameObject.SetActive(false);
			Object.DontDestroyOnLoad(GlobalDeactivatedSpawnRoot._xform.gameObject);
		}
		GlobalDeactivatedSpawnRoot._xform.gameObject.SetActive(false);
		return GlobalDeactivatedSpawnRoot._xform;
	}

	// Token: 0x04000E28 RID: 3624
	private static Transform _xform;
}
