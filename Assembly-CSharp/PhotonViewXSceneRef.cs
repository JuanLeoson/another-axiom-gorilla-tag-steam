using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200026B RID: 619
public class PhotonViewXSceneRef : MonoBehaviour
{
	// Token: 0x1700015D RID: 349
	// (get) Token: 0x06000E52 RID: 3666 RVA: 0x000578F0 File Offset: 0x00055AF0
	public PhotonView photonView
	{
		get
		{
			PhotonView result;
			if (this.reference.TryResolve<PhotonView>(out result))
			{
				return result;
			}
			return null;
		}
	}

	// Token: 0x0400172F RID: 5935
	[SerializeField]
	private XSceneRef reference;
}
