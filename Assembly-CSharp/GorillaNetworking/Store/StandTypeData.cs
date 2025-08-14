using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000DD2 RID: 3538
	public class StandTypeData
	{
		// Token: 0x060057E1 RID: 22497 RVA: 0x001B4964 File Offset: 0x001B2B64
		public StandTypeData(string[] spawnData)
		{
			this.departmentID = spawnData[0];
			this.displayID = spawnData[1];
			this.standID = spawnData[2];
			this.bustType = spawnData[3];
			if (spawnData.Length == 5)
			{
				this.playFabID = spawnData[4];
			}
			Debug.Log(string.Concat(new string[]
			{
				"StoreStuff: StandTypeData: ",
				this.departmentID,
				"\n",
				this.displayID,
				"\n",
				this.standID,
				"\n",
				this.bustType,
				"\n",
				this.playFabID
			}));
		}

		// Token: 0x060057E2 RID: 22498 RVA: 0x001B4A48 File Offset: 0x001B2C48
		public StandTypeData(string departmentID, string displayID, string standID, HeadModel_CosmeticStand.BustType bustType, string playFabID)
		{
			this.departmentID = departmentID;
			this.displayID = displayID;
			this.standID = standID;
			this.bustType = bustType.ToString();
			this.playFabID = playFabID;
		}

		// Token: 0x04006199 RID: 24985
		public string departmentID = "";

		// Token: 0x0400619A RID: 24986
		public string displayID = "";

		// Token: 0x0400619B RID: 24987
		public string standID = "";

		// Token: 0x0400619C RID: 24988
		public string bustType = "";

		// Token: 0x0400619D RID: 24989
		public string playFabID = "";

		// Token: 0x02000DD3 RID: 3539
		public enum EStandDataID
		{
			// Token: 0x0400619F RID: 24991
			departmentID,
			// Token: 0x040061A0 RID: 24992
			displayID,
			// Token: 0x040061A1 RID: 24993
			standID,
			// Token: 0x040061A2 RID: 24994
			bustType,
			// Token: 0x040061A3 RID: 24995
			playFabID,
			// Token: 0x040061A4 RID: 24996
			Count
		}
	}
}
