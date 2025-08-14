using System;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000C6A RID: 3178
	public class VirtualStumpReturnWatchTrigger : MonoBehaviour
	{
		// Token: 0x06004EB8 RID: 20152 RVA: 0x001877AF File Offset: 0x001859AF
		public void OnTriggerEnter(Collider other)
		{
			if (other == GTPlayer.Instance.headCollider)
			{
				VRRig.LocalRig.EnableVStumpReturnWatch(false);
			}
		}

		// Token: 0x06004EB9 RID: 20153 RVA: 0x001877CE File Offset: 0x001859CE
		public void OnTriggerExit(Collider other)
		{
			if (other == GTPlayer.Instance.headCollider && GorillaComputer.instance.IsPlayerInVirtualStump())
			{
				VRRig.LocalRig.EnableVStumpReturnWatch(true);
			}
		}
	}
}
