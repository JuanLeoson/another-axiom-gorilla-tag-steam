using System;
using UnityEngine;

namespace GorillaTagScripts.ModIO
{
	// Token: 0x02000C5D RID: 3165
	[Serializable]
	public class ZoneEjectLocations
	{
		// Token: 0x04005733 RID: 22323
		public GTZone ejectZone = GTZone.none;

		// Token: 0x04005734 RID: 22324
		public GameObject[] ejectLocations;
	}
}
