using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02000ED3 RID: 3795
	[Serializable]
	public struct CosmeticHoldableSlotAttachInfo
	{
		// Token: 0x0400685F RID: 26719
		[Tooltip("The anchor that this holdable cosmetic can attach to.")]
		public GTSturdyEnum<GTHardCodedBones.EHandAndStowSlots> stowSlot;

		// Token: 0x04006860 RID: 26720
		public XformOffset offset;
	}
}
