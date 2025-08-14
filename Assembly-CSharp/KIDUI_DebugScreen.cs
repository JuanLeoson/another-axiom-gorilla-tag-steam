using System;
using UnityEngine;

// Token: 0x02000940 RID: 2368
public class KIDUI_DebugScreen : MonoBehaviour
{
	// Token: 0x06003A43 RID: 14915 RVA: 0x0012D69E File Offset: 0x0012B89E
	private void Awake()
	{
		Object.DestroyImmediate(base.gameObject);
	}

	// Token: 0x06003A44 RID: 14916 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnResetUserAndQuit()
	{
	}

	// Token: 0x06003A45 RID: 14917 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnClose()
	{
	}

	// Token: 0x06003A46 RID: 14918 RVA: 0x00058615 File Offset: 0x00056815
	public static string GetOrCreateUsername()
	{
		return null;
	}

	// Token: 0x06003A47 RID: 14919 RVA: 0x000023F5 File Offset: 0x000005F5
	public void ResetAll()
	{
	}
}
