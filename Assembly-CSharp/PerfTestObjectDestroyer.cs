using System;
using GorillaTag;
using UnityEngine;

// Token: 0x0200027B RID: 635
[GTStripGameObjectFromBuild("!GT_AUTOMATED_PERF_TEST")]
public class PerfTestObjectDestroyer : MonoBehaviour
{
	// Token: 0x06000E89 RID: 3721 RVA: 0x000583CA File Offset: 0x000565CA
	private void Start()
	{
		Object.DestroyImmediate(base.gameObject, true);
	}
}
