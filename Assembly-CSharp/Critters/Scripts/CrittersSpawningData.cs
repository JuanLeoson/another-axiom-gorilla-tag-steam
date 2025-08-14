using System;
using System.Collections.Generic;
using UnityEngine;

namespace Critters.Scripts
{
	// Token: 0x02000F8E RID: 3982
	public class CrittersSpawningData : MonoBehaviour
	{
		// Token: 0x06006397 RID: 25495 RVA: 0x001F5EA0 File Offset: 0x001F40A0
		public void InitializeSpawnCollection()
		{
			for (int i = 0; i < this.SpawnParametersList.Count; i++)
			{
				for (int j = 0; j < this.SpawnParametersList[i].ChancesToSpawn; j++)
				{
					this.templateCollection.Add(i);
				}
			}
		}

		// Token: 0x06006398 RID: 25496 RVA: 0x001F5EEC File Offset: 0x001F40EC
		public int GetRandomTemplate()
		{
			int index = Random.Range(0, this.templateCollection.Count - 1);
			return this.templateCollection[index];
		}

		// Token: 0x04006E85 RID: 28293
		public List<CrittersSpawningData.CreatureSpawnParameters> SpawnParametersList;

		// Token: 0x04006E86 RID: 28294
		private List<int> templateCollection = new List<int>();

		// Token: 0x02000F8F RID: 3983
		[Serializable]
		public class CreatureSpawnParameters
		{
			// Token: 0x04006E87 RID: 28295
			public CritterTemplate Template;

			// Token: 0x04006E88 RID: 28296
			public int ChancesToSpawn;

			// Token: 0x04006E89 RID: 28297
			[HideInInspector]
			[NonSerialized]
			public int StartingIndex;
		}
	}
}
