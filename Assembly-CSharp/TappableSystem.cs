using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200077F RID: 1919
public class TappableSystem : GTSystem<Tappable>
{
	// Token: 0x0600302D RID: 12333 RVA: 0x000FD39C File Offset: 0x000FB59C
	[PunRPC]
	public void SendOnTapRPC(int key, float tapStrength, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "SendOnTapRPC");
		if (key < 0 || key >= this._instances.Count || !float.IsFinite(tapStrength))
		{
			return;
		}
		tapStrength = Mathf.Clamp(tapStrength, 0f, 1f);
		this._instances[key].OnTapLocal(tapStrength, Time.time, new PhotonMessageInfoWrapped(info));
	}
}
