using System;
using UnityEngine;

// Token: 0x020001E7 RID: 487
public class DevInspectorManager : MonoBehaviour
{
	// Token: 0x1700012E RID: 302
	// (get) Token: 0x06000BAC RID: 2988 RVA: 0x0004063B File Offset: 0x0003E83B
	public static DevInspectorManager instance
	{
		get
		{
			if (DevInspectorManager._instance == null)
			{
				DevInspectorManager._instance = Object.FindObjectOfType<DevInspectorManager>();
			}
			return DevInspectorManager._instance;
		}
	}

	// Token: 0x04000E9C RID: 3740
	private static DevInspectorManager _instance;
}
