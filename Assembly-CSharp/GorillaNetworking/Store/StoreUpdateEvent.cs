using System;
using System.Collections.Generic;
using LitJson;
using Newtonsoft.Json;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000DDA RID: 3546
	public class StoreUpdateEvent
	{
		// Token: 0x06005807 RID: 22535 RVA: 0x00002050 File Offset: 0x00000250
		public StoreUpdateEvent()
		{
		}

		// Token: 0x06005808 RID: 22536 RVA: 0x001B510C File Offset: 0x001B330C
		public StoreUpdateEvent(string pedestalID, string itemName, DateTime startTimeUTC, DateTime endTimeUTC)
		{
			this.PedestalID = pedestalID;
			this.ItemName = itemName;
			this.StartTimeUTC = startTimeUTC;
			this.EndTimeUTC = endTimeUTC;
		}

		// Token: 0x06005809 RID: 22537 RVA: 0x001B5131 File Offset: 0x001B3331
		public static string SerializeAsJSon(StoreUpdateEvent storeEvent)
		{
			return JsonUtility.ToJson(storeEvent);
		}

		// Token: 0x0600580A RID: 22538 RVA: 0x001B5139 File Offset: 0x001B3339
		public static string SerializeArrayAsJSon(StoreUpdateEvent[] storeEvents)
		{
			return JsonConvert.SerializeObject(storeEvents);
		}

		// Token: 0x0600580B RID: 22539 RVA: 0x001B5141 File Offset: 0x001B3341
		public static StoreUpdateEvent DeserializeFromJSon(string json)
		{
			return JsonUtility.FromJson<StoreUpdateEvent>(json);
		}

		// Token: 0x0600580C RID: 22540 RVA: 0x001B5149 File Offset: 0x001B3349
		public static StoreUpdateEvent[] DeserializeFromJSonArray(string json)
		{
			List<StoreUpdateEvent> list = JsonMapper.ToObject<List<StoreUpdateEvent>>(json);
			list.Sort((StoreUpdateEvent x, StoreUpdateEvent y) => x.StartTimeUTC.CompareTo(y.StartTimeUTC));
			return list.ToArray();
		}

		// Token: 0x0600580D RID: 22541 RVA: 0x001B517B File Offset: 0x001B337B
		public static List<StoreUpdateEvent> DeserializeFromJSonList(string json)
		{
			List<StoreUpdateEvent> list = JsonMapper.ToObject<List<StoreUpdateEvent>>(json);
			list.Sort((StoreUpdateEvent x, StoreUpdateEvent y) => x.StartTimeUTC.CompareTo(y.StartTimeUTC));
			return list;
		}

		// Token: 0x040061C7 RID: 25031
		public string PedestalID;

		// Token: 0x040061C8 RID: 25032
		public string ItemName;

		// Token: 0x040061C9 RID: 25033
		public DateTime StartTimeUTC;

		// Token: 0x040061CA RID: 25034
		public DateTime EndTimeUTC;
	}
}
