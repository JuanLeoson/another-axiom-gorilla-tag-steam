using System;
using System.IO;

// Token: 0x020005A8 RID: 1448
public interface IGameEntityZoneComponent
{
	// Token: 0x06002374 RID: 9076
	void OnZoneInit();

	// Token: 0x06002375 RID: 9077
	void OnZoneClear(ZoneClearReason reason);

	// Token: 0x06002376 RID: 9078
	void OnCreateGameEntity(GameEntity entity);

	// Token: 0x06002377 RID: 9079
	void SerializeZoneData(BinaryWriter writer);

	// Token: 0x06002378 RID: 9080
	void DeserializeZoneData(BinaryReader reader);

	// Token: 0x06002379 RID: 9081
	void SerializeZoneEntityData(BinaryWriter writer, GameEntity entity);

	// Token: 0x0600237A RID: 9082
	void DeserializeZoneEntityData(BinaryReader reader, GameEntity entity);

	// Token: 0x0600237B RID: 9083
	bool IsZoneReady();
}
