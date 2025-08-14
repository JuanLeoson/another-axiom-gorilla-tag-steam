using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000C2A RID: 3114
	public class EnvItem : MonoBehaviour, IPunInstantiateMagicCallback
	{
		// Token: 0x06004CA0 RID: 19616 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnEnable()
		{
		}

		// Token: 0x06004CA1 RID: 19617 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnDisable()
		{
		}

		// Token: 0x06004CA2 RID: 19618 RVA: 0x0017C14C File Offset: 0x0017A34C
		public void OnPhotonInstantiate(PhotonMessageInfo info)
		{
			object[] instantiationData = info.photonView.InstantiationData;
			this.spawnedByPhotonViewId = (int)instantiationData[0];
		}

		// Token: 0x040055B8 RID: 21944
		public int spawnedByPhotonViewId;
	}
}
