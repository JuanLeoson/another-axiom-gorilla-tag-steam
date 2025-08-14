using System;

// Token: 0x020006A5 RID: 1701
internal interface IGorillaSerializeableScene : IGorillaSerializeable
{
	// Token: 0x060029CA RID: 10698
	void OnSceneLinking(GorillaSerializerScene serializer);

	// Token: 0x060029CB RID: 10699
	void OnNetworkObjectDisable();

	// Token: 0x060029CC RID: 10700
	void OnNetworkObjectEnable();
}
