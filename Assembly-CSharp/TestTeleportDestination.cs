using System;
using GorillaTag;
using UnityEngine;

// Token: 0x0200027C RID: 636
[GTStripGameObjectFromBuild("!GT_AUTOMATED_PERF_TEST")]
public class TestTeleportDestination : MonoBehaviour
{
	// Token: 0x06000E8B RID: 3723 RVA: 0x000583D8 File Offset: 0x000565D8
	private void OnDrawGizmosSelected()
	{
		Debug.DrawRay(base.transform.position, base.transform.forward * 2f, Color.magenta);
	}

	// Token: 0x04001770 RID: 6000
	public GTZone[] zones;

	// Token: 0x04001771 RID: 6001
	public GameObject teleportTransform;
}
