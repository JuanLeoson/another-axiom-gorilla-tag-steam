using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FXP;
using PlayFab;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000DDC RID: 3548
	public class StoreUpdater : MonoBehaviour
	{
		// Token: 0x17000874 RID: 2164
		// (get) Token: 0x06005812 RID: 22546 RVA: 0x001B51C7 File Offset: 0x001B33C7
		public DateTime DateTimeNowServerAdjusted
		{
			get
			{
				return GorillaComputer.instance.GetServerTime();
			}
		}

		// Token: 0x06005813 RID: 22547 RVA: 0x001B51D5 File Offset: 0x001B33D5
		public void Awake()
		{
			if (StoreUpdater.instance == null)
			{
				StoreUpdater.instance = this;
				return;
			}
			if (StoreUpdater.instance != this)
			{
				Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06005814 RID: 22548 RVA: 0x001B5209 File Offset: 0x001B3409
		private void OnApplicationFocus(bool hasFocus)
		{
			if (hasFocus)
			{
				this.HandleHMDMounted();
				return;
			}
			this.HandleHMDUnmounted();
		}

		// Token: 0x06005815 RID: 22549 RVA: 0x001B521C File Offset: 0x001B341C
		public void Initialize()
		{
			this.FindAllCosmeticItemPrefabs();
			OVRManager.HMDMounted += this.HandleHMDMounted;
			OVRManager.HMDUnmounted += this.HandleHMDUnmounted;
			OVRManager.HMDLost += this.HandleHMDUnmounted;
			OVRManager.HMDAcquired += this.HandleHMDMounted;
			Debug.Log("StoreUpdater - Starting");
			if (this.bLoadFromJSON)
			{
				base.StartCoroutine(this.InitializeTitleData());
			}
		}

		// Token: 0x06005816 RID: 22550 RVA: 0x001B5292 File Offset: 0x001B3492
		private void ServerTimeUpdater()
		{
			base.StartCoroutine(this.InitializeTitleData());
		}

		// Token: 0x06005817 RID: 22551 RVA: 0x001B52A4 File Offset: 0x001B34A4
		public void OnDestroy()
		{
			OVRManager.HMDMounted -= this.HandleHMDMounted;
			OVRManager.HMDUnmounted -= this.HandleHMDUnmounted;
			OVRManager.HMDLost -= this.HandleHMDUnmounted;
			OVRManager.HMDAcquired -= this.HandleHMDMounted;
		}

		// Token: 0x06005818 RID: 22552 RVA: 0x001B52F8 File Offset: 0x001B34F8
		private void HandleHMDUnmounted()
		{
			foreach (string key in this.pedestalUpdateCoroutines.Keys)
			{
				if (this.pedestalUpdateCoroutines[key] != null)
				{
					base.StopCoroutine(this.pedestalUpdateCoroutines[key]);
				}
			}
			foreach (string key2 in this.cosmeticItemPrefabsDictionary.Keys)
			{
				if (this.cosmeticItemPrefabsDictionary[key2] != null)
				{
					this.cosmeticItemPrefabsDictionary[key2].StopCountdownCoroutine();
				}
			}
		}

		// Token: 0x06005819 RID: 22553 RVA: 0x001B53D0 File Offset: 0x001B35D0
		private void HandleHMDMounted()
		{
			foreach (string text in this.cosmeticItemPrefabsDictionary.Keys)
			{
				if (this.cosmeticItemPrefabsDictionary[text] != null && this.pedestalUpdateEvents.ContainsKey(text) && this.cosmeticItemPrefabsDictionary[text].gameObject.activeInHierarchy)
				{
					this.CheckEventsOnResume(this.pedestalUpdateEvents[text]);
					this.StartNextEvent(text, false);
				}
			}
		}

		// Token: 0x0600581A RID: 22554 RVA: 0x001B5478 File Offset: 0x001B3678
		private void FindAllCosmeticItemPrefabs()
		{
			foreach (CosmeticItemPrefab cosmeticItemPrefab in Object.FindObjectsOfType<CosmeticItemPrefab>())
			{
				if (this.cosmeticItemPrefabsDictionary.ContainsKey(cosmeticItemPrefab.PedestalID))
				{
					Debug.LogWarning("StoreUpdater - Duplicate Pedestal ID " + cosmeticItemPrefab.PedestalID);
				}
				else
				{
					Debug.Log("StoreUpdater - Adding Pedestal " + cosmeticItemPrefab.PedestalID);
					this.cosmeticItemPrefabsDictionary.Add(cosmeticItemPrefab.PedestalID, cosmeticItemPrefab);
				}
			}
		}

		// Token: 0x0600581B RID: 22555 RVA: 0x001B54EE File Offset: 0x001B36EE
		private IEnumerator HandlePedestalUpdate(StoreUpdateEvent updateEvent, bool playFX)
		{
			this.cosmeticItemPrefabsDictionary[updateEvent.PedestalID].SetStoreUpdateEvent(updateEvent, playFX);
			yield return new WaitForSeconds((float)(updateEvent.EndTimeUTC.ToUniversalTime() - this.DateTimeNowServerAdjusted).TotalSeconds);
			if (this.pedestalClearCartCoroutines.ContainsKey(updateEvent.PedestalID))
			{
				if (this.pedestalClearCartCoroutines[updateEvent.PedestalID] != null)
				{
					base.StopCoroutine(this.pedestalClearCartCoroutines[updateEvent.PedestalID]);
				}
				this.pedestalClearCartCoroutines[updateEvent.PedestalID] = base.StartCoroutine(this.HandleClearCart(updateEvent));
			}
			else
			{
				this.pedestalClearCartCoroutines.Add(updateEvent.PedestalID, base.StartCoroutine(this.HandleClearCart(updateEvent)));
			}
			if (this.cosmeticItemPrefabsDictionary[updateEvent.PedestalID].gameObject.activeInHierarchy)
			{
				this.pedestalUpdateEvents[updateEvent.PedestalID].RemoveAt(0);
				this.StartNextEvent(updateEvent.PedestalID, true);
			}
			yield break;
		}

		// Token: 0x0600581C RID: 22556 RVA: 0x001B550B File Offset: 0x001B370B
		private IEnumerator HandleClearCart(StoreUpdateEvent updateEvent)
		{
			float seconds = Math.Clamp((float)(updateEvent.EndTimeUTC.ToUniversalTime() - this.DateTimeNowServerAdjusted).TotalSeconds + 60f, 0f, 60f);
			yield return new WaitForSeconds(seconds);
			if (CosmeticsController.instance.RemoveItemFromCart(CosmeticsController.instance.GetItemFromDict(updateEvent.ItemName)))
			{
				CosmeticsController.instance.ClearCheckout(true);
				CosmeticsController.instance.UpdateShoppingCart();
				CosmeticsController.instance.UpdateWornCosmetics(true);
			}
			yield break;
		}

		// Token: 0x0600581D RID: 22557 RVA: 0x001B5524 File Offset: 0x001B3724
		private void StartNextEvent(string pedestalID, bool playFX)
		{
			if (this.pedestalUpdateEvents[pedestalID].Count > 0)
			{
				Coroutine value = base.StartCoroutine(this.HandlePedestalUpdate(this.pedestalUpdateEvents[pedestalID].First<StoreUpdateEvent>(), playFX));
				if (this.pedestalUpdateCoroutines.ContainsKey(pedestalID))
				{
					if (this.pedestalUpdateCoroutines[pedestalID] != null && this.pedestalUpdateCoroutines[pedestalID] != null)
					{
						base.StopCoroutine(this.pedestalUpdateCoroutines[pedestalID]);
					}
					this.pedestalUpdateCoroutines[pedestalID] = value;
				}
				else
				{
					this.pedestalUpdateCoroutines.Add(pedestalID, value);
				}
				if (this.pedestalUpdateEvents[pedestalID].Count == 0 && !this.bLoadFromJSON)
				{
					this.GetStoreUpdateEventsPlaceHolder(pedestalID);
					return;
				}
			}
			else if (!this.bLoadFromJSON)
			{
				this.GetStoreUpdateEventsPlaceHolder(pedestalID);
				this.StartNextEvent(pedestalID, true);
			}
		}

		// Token: 0x0600581E RID: 22558 RVA: 0x001B55FC File Offset: 0x001B37FC
		private void GetStoreUpdateEventsPlaceHolder(string PedestalID)
		{
			List<StoreUpdateEvent> list = new List<StoreUpdateEvent>();
			list = this.CreateTempEvents(PedestalID, 1, 15);
			this.CheckEvents(list);
			if (this.pedestalUpdateEvents.ContainsKey(PedestalID))
			{
				this.pedestalUpdateEvents[PedestalID].AddRange(list);
				return;
			}
			this.pedestalUpdateEvents.Add(PedestalID, list);
		}

		// Token: 0x0600581F RID: 22559 RVA: 0x001B5650 File Offset: 0x001B3850
		private void CheckEvents(List<StoreUpdateEvent> updateEvents)
		{
			for (int i = 0; i < updateEvents.Count; i++)
			{
				if (updateEvents[i].EndTimeUTC.ToUniversalTime() < this.DateTimeNowServerAdjusted)
				{
					updateEvents.RemoveAt(i);
					i--;
				}
			}
		}

		// Token: 0x06005820 RID: 22560 RVA: 0x001B5698 File Offset: 0x001B3898
		private void CheckEventsOnResume(List<StoreUpdateEvent> updateEvents)
		{
			bool flag = false;
			for (int i = 0; i < updateEvents.Count; i++)
			{
				if (updateEvents[i].EndTimeUTC.ToUniversalTime() < this.DateTimeNowServerAdjusted)
				{
					if (Math.Clamp((float)(updateEvents[i].EndTimeUTC.ToUniversalTime() - this.DateTimeNowServerAdjusted).TotalSeconds + 60f, 0f, 60f) <= 0f)
					{
						flag ^= CosmeticsController.instance.RemoveItemFromCart(CosmeticsController.instance.GetItemFromDict(updateEvents[i].ItemName));
					}
					else if (this.pedestalClearCartCoroutines.ContainsKey(updateEvents[i].PedestalID))
					{
						if (this.pedestalClearCartCoroutines[updateEvents[i].PedestalID] != null)
						{
							base.StopCoroutine(this.pedestalClearCartCoroutines[updateEvents[i].PedestalID]);
						}
						this.pedestalClearCartCoroutines[updateEvents[i].PedestalID] = base.StartCoroutine(this.HandleClearCart(updateEvents[i]));
					}
					else
					{
						this.pedestalClearCartCoroutines.Add(updateEvents[i].PedestalID, base.StartCoroutine(this.HandleClearCart(updateEvents[i])));
					}
					updateEvents.RemoveAt(i);
					i--;
				}
			}
			if (flag)
			{
				CosmeticsController.instance.ClearCheckout(true);
				CosmeticsController.instance.UpdateShoppingCart();
				CosmeticsController.instance.UpdateWornCosmetics(true);
			}
		}

		// Token: 0x06005821 RID: 22561 RVA: 0x001B5825 File Offset: 0x001B3A25
		private IEnumerator InitializeTitleData()
		{
			yield return new WaitForSeconds(1f);
			PlayFabTitleDataCache.Instance.UpdateData();
			yield return new WaitForSeconds(1f);
			this.GetEventsFromTitleData();
			yield break;
		}

		// Token: 0x06005822 RID: 22562 RVA: 0x001B5834 File Offset: 0x001B3A34
		private void GetEventsFromTitleData()
		{
			Debug.Log("StoreUpdater - GetEventsFromTitleData");
			if (this.bUsePlaceHolderJSON)
			{
				DateTime startTime = new DateTime(2024, 2, 13, 16, 0, 0, DateTimeKind.Utc);
				List<StoreUpdateEvent> updateEvents = StoreUpdateEvent.DeserializeFromJSonList(StoreUpdateEvent.SerializeArrayAsJSon(this.CreateTempEvents("Pedestal1", 2, 120, startTime).ToArray()));
				this.HandleRecievingEventsFromTitleData(updateEvents);
				return;
			}
			PlayFabTitleDataCache.Instance.GetTitleData("TOTD", delegate(string result)
			{
				Debug.Log("StoreUpdater - Recieved TitleData : " + result);
				List<StoreUpdateEvent> updateEvents2 = StoreUpdateEvent.DeserializeFromJSonList(result);
				this.HandleRecievingEventsFromTitleData(updateEvents2);
			}, delegate(PlayFabError error)
			{
				Debug.Log("StoreUpdater - Error Title Data : " + error.ErrorMessage);
			});
		}

		// Token: 0x06005823 RID: 22563 RVA: 0x001B58C8 File Offset: 0x001B3AC8
		private void HandleRecievingEventsFromTitleData(List<StoreUpdateEvent> updateEvents)
		{
			Debug.Log("StoreUpdater - HandleRecievingEventsFromTitleData");
			this.CheckEvents(updateEvents);
			if (CosmeticsController.instance.GetItemFromDict("LBAEY.").isNullItem)
			{
				Debug.LogWarning("StoreUpdater - CosmeticsController is not initialized.  Reinitializing TitleData");
				base.StartCoroutine(this.InitializeTitleData());
				return;
			}
			foreach (StoreUpdateEvent storeUpdateEvent in updateEvents)
			{
				if (this.pedestalUpdateEvents.ContainsKey(storeUpdateEvent.PedestalID))
				{
					this.pedestalUpdateEvents[storeUpdateEvent.PedestalID].Add(storeUpdateEvent);
				}
				else
				{
					this.pedestalUpdateEvents.Add(storeUpdateEvent.PedestalID, new List<StoreUpdateEvent>());
					this.pedestalUpdateEvents[storeUpdateEvent.PedestalID].Add(storeUpdateEvent);
				}
			}
			Debug.Log("StoreUpdater - Starting Events");
			foreach (string text in this.pedestalUpdateEvents.Keys)
			{
				if (this.cosmeticItemPrefabsDictionary.ContainsKey(text))
				{
					Debug.Log("StoreUpdater - Starting Event " + text);
					this.StartNextEvent(text, false);
				}
			}
			foreach (string text2 in this.cosmeticItemPrefabsDictionary.Keys)
			{
				if (!this.pedestalUpdateEvents.ContainsKey(text2))
				{
					Debug.Log("StoreUpdater - Adding PlaceHolder Events " + text2);
					this.GetStoreUpdateEventsPlaceHolder(text2);
					this.StartNextEvent(text2, false);
				}
			}
		}

		// Token: 0x06005824 RID: 22564 RVA: 0x001B5A8C File Offset: 0x001B3C8C
		private void PrintJSONEvents()
		{
			string text = StoreUpdateEvent.SerializeArrayAsJSon(this.CreateTempEvents("Pedestal1", 5, 28).ToArray());
			foreach (StoreUpdateEvent storeUpdateEvent in StoreUpdateEvent.DeserializeFromJSonList(text))
			{
				Debug.Log(string.Concat(new string[]
				{
					"Event : ",
					storeUpdateEvent.ItemName,
					" : ",
					storeUpdateEvent.StartTimeUTC.ToString(),
					" : ",
					storeUpdateEvent.EndTimeUTC.ToString()
				}));
			}
			Debug.Log("NewEvents :\n" + text);
			this.tempJson = text;
		}

		// Token: 0x06005825 RID: 22565 RVA: 0x001B5B58 File Offset: 0x001B3D58
		private List<StoreUpdateEvent> CreateTempEvents(string PedestalID, int minuteDelay, int totalEvents)
		{
			string[] array = new string[]
			{
				"LBAEY.",
				"LBAEZ.",
				"LBAFA.",
				"LBAFB.",
				"LBAFC.",
				"LBAFD.",
				"LBAFE.",
				"LBAFF.",
				"LBAFG.",
				"LBAFH.",
				"LBAFO.",
				"LBAFP.",
				"LBAFQ.",
				"LBAFR."
			};
			List<StoreUpdateEvent> list = new List<StoreUpdateEvent>();
			for (int i = 0; i < totalEvents; i++)
			{
				StoreUpdateEvent item = new StoreUpdateEvent(PedestalID, array[i % 14], DateTime.UtcNow + TimeSpan.FromMinutes((double)(minuteDelay * i)), DateTime.UtcNow + TimeSpan.FromMinutes((double)(minuteDelay * (i + 1))));
				list.Add(item);
			}
			return list;
		}

		// Token: 0x06005826 RID: 22566 RVA: 0x001B5C34 File Offset: 0x001B3E34
		private List<StoreUpdateEvent> CreateTempEvents(string PedestalID, int minuteDelay, int totalEvents, DateTime startTime)
		{
			string[] array = new string[]
			{
				"LBAEY.",
				"LBAEZ.",
				"LBAFA.",
				"LBAFB.",
				"LBAFC.",
				"LBAFD.",
				"LBAFE.",
				"LBAFF.",
				"LBAFG.",
				"LBAFH.",
				"LBAFO.",
				"LBAFP.",
				"LBAFQ.",
				"LBAFR."
			};
			List<StoreUpdateEvent> list = new List<StoreUpdateEvent>();
			for (int i = 0; i < totalEvents; i++)
			{
				StoreUpdateEvent item = new StoreUpdateEvent(PedestalID, array[i % 14], startTime + TimeSpan.FromMinutes((double)(minuteDelay * i)), startTime + TimeSpan.FromMinutes((double)(minuteDelay * (i + 1))));
				list.Add(item);
			}
			return list;
		}

		// Token: 0x06005827 RID: 22567 RVA: 0x001B5D07 File Offset: 0x001B3F07
		public void PedestalAsleep(CosmeticItemPrefab pedestal)
		{
			if (this.pedestalUpdateCoroutines.ContainsKey(pedestal.PedestalID) && this.pedestalUpdateCoroutines[pedestal.PedestalID] != null)
			{
				base.StopCoroutine(this.pedestalUpdateCoroutines[pedestal.PedestalID]);
			}
		}

		// Token: 0x06005828 RID: 22568 RVA: 0x001B5D48 File Offset: 0x001B3F48
		public void PedestalAwakened(CosmeticItemPrefab pedestal)
		{
			if (!this.cosmeticItemPrefabsDictionary.ContainsKey(pedestal.PedestalID))
			{
				this.cosmeticItemPrefabsDictionary.Add(pedestal.PedestalID, pedestal);
			}
			if (this.pedestalUpdateEvents.ContainsKey(pedestal.PedestalID))
			{
				this.CheckEventsOnResume(this.pedestalUpdateEvents[pedestal.PedestalID]);
				this.StartNextEvent(pedestal.PedestalID, false);
			}
		}

		// Token: 0x040061CE RID: 25038
		public static volatile StoreUpdater instance;

		// Token: 0x040061CF RID: 25039
		private DateTime StoreItemsChangeTimeUTC;

		// Token: 0x040061D0 RID: 25040
		private Dictionary<string, CosmeticItemPrefab> cosmeticItemPrefabsDictionary = new Dictionary<string, CosmeticItemPrefab>();

		// Token: 0x040061D1 RID: 25041
		private Dictionary<string, List<StoreUpdateEvent>> pedestalUpdateEvents = new Dictionary<string, List<StoreUpdateEvent>>();

		// Token: 0x040061D2 RID: 25042
		private Dictionary<string, Coroutine> pedestalUpdateCoroutines = new Dictionary<string, Coroutine>();

		// Token: 0x040061D3 RID: 25043
		private Dictionary<string, Coroutine> pedestalClearCartCoroutines = new Dictionary<string, Coroutine>();

		// Token: 0x040061D4 RID: 25044
		private string tempJson;

		// Token: 0x040061D5 RID: 25045
		private bool bLoadFromJSON = true;

		// Token: 0x040061D6 RID: 25046
		private bool bUsePlaceHolderJSON;
	}
}
