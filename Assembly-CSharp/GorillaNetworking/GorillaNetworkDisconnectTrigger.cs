using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000D81 RID: 3457
	public class GorillaNetworkDisconnectTrigger : GorillaTriggerBox
	{
		// Token: 0x06005618 RID: 22040 RVA: 0x001ABC44 File Offset: 0x001A9E44
		public override void OnBoxTriggered()
		{
			base.OnBoxTriggered();
			if (this.makeSureThisIsEnabled != null)
			{
				this.makeSureThisIsEnabled.SetActive(true);
			}
			GameObject[] array = this.makeSureTheseAreEnabled;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(true);
			}
			if (PhotonNetwork.InRoom)
			{
				if (this.componentTypeToRemove != "" && this.componentTarget.GetComponent(this.componentTypeToRemove) != null)
				{
					Object.Destroy(this.componentTarget.GetComponent(this.componentTypeToRemove));
				}
				PhotonNetwork.Disconnect();
				SkinnedMeshRenderer[] array2 = this.photonNetworkController.offlineVRRig;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].enabled = true;
				}
				PhotonNetwork.ConnectUsingSettings();
			}
		}

		// Token: 0x04005FD5 RID: 24533
		public PhotonNetworkController photonNetworkController;

		// Token: 0x04005FD6 RID: 24534
		public GameObject offlineVRRig;

		// Token: 0x04005FD7 RID: 24535
		public GameObject makeSureThisIsEnabled;

		// Token: 0x04005FD8 RID: 24536
		public GameObject[] makeSureTheseAreEnabled;

		// Token: 0x04005FD9 RID: 24537
		public string componentTypeToRemove;

		// Token: 0x04005FDA RID: 24538
		public GameObject componentTarget;
	}
}
