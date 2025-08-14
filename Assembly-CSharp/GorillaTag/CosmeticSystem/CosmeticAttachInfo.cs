using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02000ED2 RID: 3794
	[Serializable]
	public struct CosmeticAttachInfo
	{
		// Token: 0x1700092A RID: 2346
		// (get) Token: 0x06005E70 RID: 24176 RVA: 0x001DC2A0 File Offset: 0x001DA4A0
		public static CosmeticAttachInfo Identity
		{
			get
			{
				return new CosmeticAttachInfo
				{
					selectSide = ECosmeticSelectSide.Both,
					parentBone = GTHardCodedBones.EBone.None,
					offset = XformOffset.Identity
				};
			}
		}

		// Token: 0x06005E71 RID: 24177 RVA: 0x001DC2DC File Offset: 0x001DA4DC
		public CosmeticAttachInfo(ECosmeticSelectSide selectSide, GTHardCodedBones.EBone parentBone, XformOffset offset)
		{
			this.selectSide = selectSide;
			this.parentBone = parentBone;
			this.offset = offset;
		}

		// Token: 0x0400685C RID: 26716
		[Tooltip("(Not used for holdables) Determines if the cosmetic part be shown depending on the hand that is used to press the in-game wardrobe \"EQUIP\" button.\n- Both: Show no matter what hand is used.\n- Left: Only show if the left hand selected.\n- Right: Only show if the right hand selected.\n")]
		public StringEnum<ECosmeticSelectSide> selectSide;

		// Token: 0x0400685D RID: 26717
		public GTHardCodedBones.SturdyEBone parentBone;

		// Token: 0x0400685E RID: 26718
		public XformOffset offset;
	}
}
