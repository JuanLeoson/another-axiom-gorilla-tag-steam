using System;
using Fusion;
using Photon.Pun;

// Token: 0x020006A6 RID: 1702
internal interface IWrappedSerializable : INetworkStruct
{
	// Token: 0x060029CD RID: 10701
	void OnSerializeRead(object newData);

	// Token: 0x060029CE RID: 10702
	void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x060029CF RID: 10703
	object OnSerializeWrite();

	// Token: 0x060029D0 RID: 10704
	void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info);
}
