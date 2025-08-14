using System;
using Photon.Pun;

// Token: 0x020007F9 RID: 2041
public class GorillaThrowingRock : GorillaThrowable, IPunInstantiateMagicCallback
{
	// Token: 0x06003318 RID: 13080 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
	}

	// Token: 0x0400401C RID: 16412
	public float bonkSpeedMin = 1f;

	// Token: 0x0400401D RID: 16413
	public float bonkSpeedMax = 5f;

	// Token: 0x0400401E RID: 16414
	public VRRig hitRig;
}
