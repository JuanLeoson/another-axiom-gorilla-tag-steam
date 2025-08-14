using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02000ED6 RID: 3798
	[Serializable]
	public struct CosmeticPartMirrorOption
	{
		// Token: 0x0400687E RID: 26750
		public ECosmeticPartMirrorAxis axis;

		// Token: 0x0400687F RID: 26751
		[Tooltip("This will multiply the local scale for the selected axis by -1.")]
		public bool negativeScale;
	}
}
