using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020006A9 RID: 1705
public class PhotonViewCache : MonoBehaviour, IPunInstantiateMagicCallback
{
	// Token: 0x170003CD RID: 973
	// (get) Token: 0x060029DD RID: 10717 RVA: 0x000E06F4 File Offset: 0x000DE8F4
	// (set) Token: 0x060029DE RID: 10718 RVA: 0x000E06FC File Offset: 0x000DE8FC
	public bool Initialized { get; private set; }

	// Token: 0x060029DF RID: 10719 RVA: 0x000023F5 File Offset: 0x000005F5
	void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
	{
	}

	// Token: 0x040035B5 RID: 13749
	private PhotonView[] m_photonViews;

	// Token: 0x040035B6 RID: 13750
	[SerializeField]
	private bool m_isRoomObject;
}
