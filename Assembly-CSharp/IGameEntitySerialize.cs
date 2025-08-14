using System;
using System.IO;

// Token: 0x020005A3 RID: 1443
public interface IGameEntitySerialize
{
	// Token: 0x06002348 RID: 9032
	void OnGameEntitySerialize(BinaryWriter writer);

	// Token: 0x06002349 RID: 9033
	void OnGameEntityDeserialize(BinaryReader reader);
}
