using System;
using Photon.Pun;

// Token: 0x020006A4 RID: 1700
public interface IGorillaSerializeable
{
	// Token: 0x060029C8 RID: 10696
	void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x060029C9 RID: 10697
	void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info);
}
