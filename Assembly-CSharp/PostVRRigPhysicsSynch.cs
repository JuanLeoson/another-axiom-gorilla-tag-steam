using System;
using UnityEngine;

// Token: 0x02000A15 RID: 2581
public class PostVRRigPhysicsSynch : MonoBehaviour
{
	// Token: 0x06003EF2 RID: 16114 RVA: 0x0013FD2B File Offset: 0x0013DF2B
	private void LateUpdate()
	{
		Physics.SyncTransforms();
	}
}
