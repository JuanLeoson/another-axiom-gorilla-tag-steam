using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020004DF RID: 1247
public class GorillaTriggerBoxGameFlag : GorillaTriggerBox
{
	// Token: 0x06001E6F RID: 7791 RVA: 0x000A14F8 File Offset: 0x0009F6F8
	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		PhotonView.Get(Object.FindObjectOfType<GorillaGameManager>()).RPC(this.functionName, RpcTarget.MasterClient, null);
	}

	// Token: 0x0400271E RID: 10014
	public string functionName;
}
