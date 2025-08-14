using System;
using GorillaNetworking.Store;
using GT_CustomMapSupportRuntime;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000C65 RID: 3173
	[Serializable]
	public struct CustomMapCosmeticItem
	{
		// Token: 0x04005776 RID: 22390
		public GTObjectPlaceholder.ECustomMapCosmeticItem customMapItemSlot;

		// Token: 0x04005777 RID: 22391
		public HeadModel_CosmeticStand.BustType bustType;

		// Token: 0x04005778 RID: 22392
		public string playFabID;
	}
}
