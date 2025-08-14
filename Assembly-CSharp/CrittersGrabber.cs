using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000056 RID: 86
public class CrittersGrabber : CrittersActor
{
	// Token: 0x060001A4 RID: 420 RVA: 0x0000A59D File Offset: 0x0000879D
	public override void ProcessRemote()
	{
		if (this.rigPlayerId == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			this.UpdateAverageSpeed();
		}
	}

	// Token: 0x060001A5 RID: 421 RVA: 0x0000A5B7 File Offset: 0x000087B7
	public override bool ProcessLocal()
	{
		if (this.rigPlayerId == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			this.UpdateAverageSpeed();
		}
		return base.ProcessLocal();
	}

	// Token: 0x040001F2 RID: 498
	public Transform grabPosition;

	// Token: 0x040001F3 RID: 499
	public bool grabbing;

	// Token: 0x040001F4 RID: 500
	public float grabDistance;

	// Token: 0x040001F5 RID: 501
	public List<CrittersActor> grabbedActors = new List<CrittersActor>();

	// Token: 0x040001F6 RID: 502
	public bool isLeft;
}
