using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000465 RID: 1125
[DefaultExecutionOrder(1549)]
public class TransferrableObjectManager : MonoBehaviour
{
	// Token: 0x06001BE9 RID: 7145 RVA: 0x000964CE File Offset: 0x000946CE
	protected void Awake()
	{
		if (TransferrableObjectManager.hasInstance && TransferrableObjectManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		TransferrableObjectManager.SetInstance(this);
	}

	// Token: 0x06001BEA RID: 7146 RVA: 0x000964F1 File Offset: 0x000946F1
	protected void OnDestroy()
	{
		if (TransferrableObjectManager.instance == this)
		{
			TransferrableObjectManager.hasInstance = false;
			TransferrableObjectManager.instance = null;
		}
	}

	// Token: 0x06001BEB RID: 7147 RVA: 0x0009650C File Offset: 0x0009470C
	protected void LateUpdate()
	{
		for (int i = 0; i < TransferrableObjectManager.transObs.Count; i++)
		{
			TransferrableObjectManager.transObs[i].TriggeredLateUpdate();
		}
	}

	// Token: 0x06001BEC RID: 7148 RVA: 0x0009653E File Offset: 0x0009473E
	public static void CreateManager()
	{
		TransferrableObjectManager.SetInstance(new GameObject("TransferrableObjectManager").AddComponent<TransferrableObjectManager>());
	}

	// Token: 0x06001BED RID: 7149 RVA: 0x00096554 File Offset: 0x00094754
	private static void SetInstance(TransferrableObjectManager manager)
	{
		TransferrableObjectManager.instance = manager;
		TransferrableObjectManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06001BEE RID: 7150 RVA: 0x0009656F File Offset: 0x0009476F
	public static void Register(TransferrableObject transOb)
	{
		if (!TransferrableObjectManager.hasInstance)
		{
			TransferrableObjectManager.CreateManager();
		}
		if (!TransferrableObjectManager.transObs.Contains(transOb))
		{
			TransferrableObjectManager.transObs.Add(transOb);
		}
	}

	// Token: 0x06001BEF RID: 7151 RVA: 0x00096595 File Offset: 0x00094795
	public static void Unregister(TransferrableObject transOb)
	{
		if (!TransferrableObjectManager.hasInstance)
		{
			TransferrableObjectManager.CreateManager();
		}
		if (TransferrableObjectManager.transObs.Contains(transOb))
		{
			TransferrableObjectManager.transObs.Remove(transOb);
		}
	}

	// Token: 0x04002478 RID: 9336
	public static TransferrableObjectManager instance;

	// Token: 0x04002479 RID: 9337
	public static bool hasInstance = false;

	// Token: 0x0400247A RID: 9338
	public static readonly List<TransferrableObject> transObs = new List<TransferrableObject>(1024);
}
