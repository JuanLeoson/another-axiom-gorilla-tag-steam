using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000A58 RID: 2648
internal class PaintbrawlRPCs : RPCNetworkBase
{
	// Token: 0x0600409C RID: 16540 RVA: 0x0014723D File Offset: 0x0014543D
	public override void SetClassTarget(IWrappedSerializable target, GorillaWrappedSerializer netHandler)
	{
		this.paintbrawlManager = (GorillaPaintbrawlManager)target;
		this.serializer = (GameModeSerializer)netHandler;
	}

	// Token: 0x0600409D RID: 16541 RVA: 0x00147258 File Offset: 0x00145458
	[PunRPC]
	public void RPC_ReportSlingshotHit(Player taggedPlayer, Vector3 hitLocation, int projectileCount, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RPC_ReportSlingshotHit");
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(taggedPlayer);
		PhotonMessageInfoWrapped info2 = new PhotonMessageInfoWrapped(info);
		this.paintbrawlManager.ReportSlingshotHit(player, hitLocation, projectileCount, info2);
	}

	// Token: 0x04004C45 RID: 19525
	private GameModeSerializer serializer;

	// Token: 0x04004C46 RID: 19526
	private GorillaPaintbrawlManager paintbrawlManager;
}
