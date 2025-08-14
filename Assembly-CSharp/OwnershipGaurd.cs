using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000A0E RID: 2574
internal class OwnershipGaurd : MonoBehaviour
{
	// Token: 0x06003ED6 RID: 16086 RVA: 0x0013FB9B File Offset: 0x0013DD9B
	private void Start()
	{
		if (this.autoRegisterAll)
		{
			this.NetViews = base.GetComponents<PhotonView>();
		}
		if (this.NetViews == null)
		{
			return;
		}
		OwnershipGaurdHandler.RegisterViews(this.NetViews);
	}

	// Token: 0x06003ED7 RID: 16087 RVA: 0x0013FBC5 File Offset: 0x0013DDC5
	private void OnDestroy()
	{
		if (this.NetViews == null)
		{
			return;
		}
		OwnershipGaurdHandler.RemoveViews(this.NetViews);
	}

	// Token: 0x04004ADC RID: 19164
	[SerializeField]
	private PhotonView[] NetViews;

	// Token: 0x04004ADD RID: 19165
	[SerializeField]
	private bool autoRegisterAll = true;
}
