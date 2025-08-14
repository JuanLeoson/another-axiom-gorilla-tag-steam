using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000DD1 RID: 3537
	public class StandImport
	{
		// Token: 0x060057DC RID: 22492 RVA: 0x001B4808 File Offset: 0x001B2A08
		public void DecomposeFromTitleDataString(string data)
		{
			string[] array = data.Split("\\n", StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				this.DecomposeStandDataTitleData(array[i]);
			}
		}

		// Token: 0x060057DD RID: 22493 RVA: 0x001B483C File Offset: 0x001B2A3C
		public void DecomposeStandDataTitleData(string dataString)
		{
			string[] array = dataString.Split("\\t", StringSplitOptions.None);
			if (array.Length == 5)
			{
				this.standData.Add(new StandTypeData(array));
				return;
			}
			if (array.Length == 4)
			{
				this.standData.Add(new StandTypeData(array));
				return;
			}
			string text = "";
			foreach (string str in array)
			{
				text = text + str + "|";
			}
			Debug.LogError("Store Importer Data String is not valid : " + text);
		}

		// Token: 0x060057DE RID: 22494 RVA: 0x001B48BF File Offset: 0x001B2ABF
		public void DeserializeFromJSON(string JSONString)
		{
			this.standData = JsonConvert.DeserializeObject<List<StandTypeData>>(JSONString);
		}

		// Token: 0x060057DF RID: 22495 RVA: 0x001B48D0 File Offset: 0x001B2AD0
		public void DecomposeStandData(string dataString)
		{
			string[] array = dataString.Split('\t', StringSplitOptions.None);
			if (array.Length == 5)
			{
				this.standData.Add(new StandTypeData(array));
				return;
			}
			if (array.Length == 4)
			{
				this.standData.Add(new StandTypeData(array));
				return;
			}
			string text = "";
			foreach (string str in array)
			{
				text = text + str + "|";
			}
			Debug.LogError("Store Importer Data String is not valid : " + text);
		}

		// Token: 0x04006198 RID: 24984
		public List<StandTypeData> standData = new List<StandTypeData>();
	}
}
