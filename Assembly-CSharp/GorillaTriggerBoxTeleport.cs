using System;
using UnityEngine;

// Token: 0x020004E1 RID: 1249
public class GorillaTriggerBoxTeleport : GorillaTriggerBox
{
	// Token: 0x06001E74 RID: 7796 RVA: 0x000A15B1 File Offset: 0x0009F7B1
	public override void OnBoxTriggered()
	{
		this.cameraOffest.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
		this.cameraOffest.transform.position = this.teleportLocation;
	}

	// Token: 0x04002722 RID: 10018
	public Vector3 teleportLocation;

	// Token: 0x04002723 RID: 10019
	public GameObject cameraOffest;
}
