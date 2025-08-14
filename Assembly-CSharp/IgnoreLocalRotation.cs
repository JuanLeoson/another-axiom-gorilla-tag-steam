using System;
using UnityEngine;

// Token: 0x020001AE RID: 430
public class IgnoreLocalRotation : MonoBehaviour
{
	// Token: 0x06000AA7 RID: 2727 RVA: 0x000396D7 File Offset: 0x000378D7
	private void LateUpdate()
	{
		base.transform.rotation = Quaternion.identity;
	}
}
