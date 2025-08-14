using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaNetworking;
using GorillaNetworking.Store;
using GT_CustomMapSupportRuntime;
using PlayFab;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000C66 RID: 3174
	[CreateAssetMenu(menuName = "ScriptableObjects/CustomMapCosmeticDataSO", order = 0)]
	[Serializable]
	public class CustomMapCosmeticsData : ScriptableObject
	{
		// Token: 0x06004E9D RID: 20125 RVA: 0x00187069 File Offset: 0x00185269
		public void OnEnable()
		{
			this.initializedFromTitleData = false;
		}

		// Token: 0x06004E9E RID: 20126 RVA: 0x00187072 File Offset: 0x00185272
		public void OnDestroy()
		{
			if (PlayFabTitleDataCache.Instance.IsNotNull())
			{
				PlayFabTitleDataCache.Instance.OnTitleDataUpdate.RemoveListener(new UnityAction<string>(this.OnTitleDataUpdated));
			}
		}

		// Token: 0x06004E9F RID: 20127 RVA: 0x0018709C File Offset: 0x0018529C
		public bool TryGetItem(GTObjectPlaceholder.ECustomMapCosmeticItem customMapItemSlot, out CustomMapCosmeticItem foundItem)
		{
			if (!this.initializedFromTitleData)
			{
				this.UpdateFromTitleData();
			}
			foundItem = new CustomMapCosmeticItem
			{
				bustType = HeadModel_CosmeticStand.BustType.Disabled,
				playFabID = "INVALID"
			};
			for (int i = 0; i < this.customMapCosmeticItemList.Count; i++)
			{
				if (this.customMapCosmeticItemList[i].customMapItemSlot == customMapItemSlot)
				{
					foundItem = this.customMapCosmeticItemList[i];
					return true;
				}
			}
			for (int j = 0; j < this.fallbackItems.Count; j++)
			{
				if (this.fallbackItems[j].customMapItemSlot == customMapItemSlot)
				{
					foundItem = this.fallbackItems[j];
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004EA0 RID: 20128 RVA: 0x00187158 File Offset: 0x00185358
		private void UpdateFromTitleData()
		{
			if (this.initializedFromTitleData)
			{
				return;
			}
			if (PlayFabTitleDataCache.Instance.IsNull())
			{
				return;
			}
			PlayFabTitleDataCache.Instance.OnTitleDataUpdate.RemoveListener(new UnityAction<string>(this.OnTitleDataUpdated));
			PlayFabTitleDataCache.Instance.OnTitleDataUpdate.AddListener(new UnityAction<string>(this.OnTitleDataUpdated));
			if (PlayFabTitleDataCache.Instance == null)
			{
				Debug.LogError("[CustomMapCosmeticsData::UpdateFromTitleData] TitleData not available, using fallback item data.");
				this.initializedFromTitleData = true;
				return;
			}
			PlayFabTitleDataCache.Instance.GetTitleData(this.titleDataKey, new Action<string>(this.OnGetCosmeticsDataFromTitleData), new Action<PlayFabError>(this.OnPlayFabError));
			this.initializedFromTitleData = true;
		}

		// Token: 0x06004EA1 RID: 20129 RVA: 0x001871FF File Offset: 0x001853FF
		private void OnTitleDataUpdated(string updatedKey)
		{
			if (updatedKey == this.titleDataKey)
			{
				this.initializedFromTitleData = false;
				this.UpdateFromTitleData();
			}
		}

		// Token: 0x06004EA2 RID: 20130 RVA: 0x0018721C File Offset: 0x0018541C
		private void OnGetCosmeticsDataFromTitleData(string cosmeticsData)
		{
			string[] array = cosmeticsData.Split("|", StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				string text2 = text;
				text2 = text2.RemoveAll('\\', StringComparison.OrdinalIgnoreCase);
				text2 = text2.Trim('"');
				CustomMapCosmeticItem itemFromJson = JsonUtility.FromJson<CustomMapCosmeticItem>(text2);
				this.customMapCosmeticItemList.RemoveAll((CustomMapCosmeticItem item) => item.customMapItemSlot == itemFromJson.customMapItemSlot);
				this.customMapCosmeticItemList.Add(itemFromJson);
			}
		}

		// Token: 0x06004EA3 RID: 20131 RVA: 0x00187296 File Offset: 0x00185496
		private void OnPlayFabError(PlayFabError error)
		{
			Debug.LogError("[CustomMapCosmeticsData::OnPlayFabError] failed to retrieve CosmeticsData from PlayFab: " + error.ErrorMessage);
		}

		// Token: 0x04005779 RID: 22393
		[SerializeField]
		private List<CustomMapCosmeticItem> fallbackItems;

		// Token: 0x0400577A RID: 22394
		[SerializeField]
		private List<CustomMapCosmeticItem> customMapCosmeticItemList;

		// Token: 0x0400577B RID: 22395
		public string titleDataKey = "CustomMapCosmeticData";

		// Token: 0x0400577C RID: 22396
		[OnEnterPlay_Set(false)]
		private bool initializedFromTitleData;
	}
}
