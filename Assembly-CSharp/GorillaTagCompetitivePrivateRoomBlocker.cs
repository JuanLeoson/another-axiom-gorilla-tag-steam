using System;
using UnityEngine;

// Token: 0x02000702 RID: 1794
public class GorillaTagCompetitivePrivateRoomBlocker : MonoBehaviour
{
	// Token: 0x06002D01 RID: 11521 RVA: 0x000ED7EF File Offset: 0x000EB9EF
	private void Update()
	{
		this.blocker.SetActive(NetworkSystem.Instance.SessionIsPrivate);
	}

	// Token: 0x0400384F RID: 14415
	[SerializeField]
	private GameObject blocker;
}
