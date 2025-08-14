using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000DCC RID: 3532
	public class DynamicCosmeticStand_Link : MonoBehaviour
	{
		// Token: 0x060057AD RID: 22445 RVA: 0x001B334A File Offset: 0x001B154A
		public void SetStandType(HeadModel_CosmeticStand.BustType type)
		{
			this.stand.SetStandType(type);
		}

		// Token: 0x060057AE RID: 22446 RVA: 0x001B3358 File Offset: 0x001B1558
		public void SpawnItemOntoStand(string PlayFabID)
		{
			this.stand.SpawnItemOntoStand(PlayFabID);
		}

		// Token: 0x060057AF RID: 22447 RVA: 0x001B3366 File Offset: 0x001B1566
		public void SaveCosmeticMountPosition()
		{
			this.stand.UpdateCosmeticsMountPositions();
		}

		// Token: 0x060057B0 RID: 22448 RVA: 0x001B3373 File Offset: 0x001B1573
		public void ClearCosmeticItems()
		{
			this.stand.ClearCosmetics();
		}

		// Token: 0x0400617B RID: 24955
		public DynamicCosmeticStand stand;
	}
}
