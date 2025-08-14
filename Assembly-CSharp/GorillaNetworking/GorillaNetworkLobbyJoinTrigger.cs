using System;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000D86 RID: 3462
	public class GorillaNetworkLobbyJoinTrigger : GorillaTriggerBox
	{
		// Token: 0x04005FF1 RID: 24561
		public GameObject[] makeSureThisIsDisabled;

		// Token: 0x04005FF2 RID: 24562
		public GameObject[] makeSureThisIsEnabled;

		// Token: 0x04005FF3 RID: 24563
		public string gameModeName;

		// Token: 0x04005FF4 RID: 24564
		public PhotonNetworkController photonNetworkController;

		// Token: 0x04005FF5 RID: 24565
		public string componentTypeToRemove;

		// Token: 0x04005FF6 RID: 24566
		public GameObject componentRemoveTarget;

		// Token: 0x04005FF7 RID: 24567
		public string componentTypeToAdd;

		// Token: 0x04005FF8 RID: 24568
		public GameObject componentAddTarget;

		// Token: 0x04005FF9 RID: 24569
		public GameObject gorillaParent;

		// Token: 0x04005FFA RID: 24570
		public GameObject joinFailedBlock;
	}
}
