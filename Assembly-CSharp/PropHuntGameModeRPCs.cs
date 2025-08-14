using System;

// Token: 0x0200015B RID: 347
internal class PropHuntGameModeRPCs : RPCNetworkBase
{
	// Token: 0x06000941 RID: 2369 RVA: 0x00032731 File Offset: 0x00030931
	public override void SetClassTarget(IWrappedSerializable target, GorillaWrappedSerializer netHandler)
	{
		this.propHuntManager = (GorillaPropHuntGameManager)target;
		this.serializer = (GameModeSerializer)netHandler;
	}

	// Token: 0x04000AE3 RID: 2787
	private GameModeSerializer serializer;

	// Token: 0x04000AE4 RID: 2788
	private GorillaPropHuntGameManager propHuntManager;
}
