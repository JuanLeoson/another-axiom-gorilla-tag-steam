using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GorillaNetworking;
using JetBrains.Annotations;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Networking;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000CA9 RID: 3241
	public class SharedBlocksManager : MonoBehaviour
	{
		// Token: 0x1400008D RID: 141
		// (add) Token: 0x0600506D RID: 20589 RVA: 0x0019178C File Offset: 0x0018F98C
		// (remove) Token: 0x0600506E RID: 20590 RVA: 0x001917C4 File Offset: 0x0018F9C4
		public event Action<string> OnGetTableConfiguration;

		// Token: 0x1400008E RID: 142
		// (add) Token: 0x0600506F RID: 20591 RVA: 0x001917FC File Offset: 0x0018F9FC
		// (remove) Token: 0x06005070 RID: 20592 RVA: 0x00191834 File Offset: 0x0018FA34
		public event Action<string> OnGetTitleDataBuildComplete;

		// Token: 0x1400008F RID: 143
		// (add) Token: 0x06005071 RID: 20593 RVA: 0x0019186C File Offset: 0x0018FA6C
		// (remove) Token: 0x06005072 RID: 20594 RVA: 0x001918A4 File Offset: 0x0018FAA4
		public event Action<int> OnSavePrivateScanSuccess;

		// Token: 0x14000090 RID: 144
		// (add) Token: 0x06005073 RID: 20595 RVA: 0x001918DC File Offset: 0x0018FADC
		// (remove) Token: 0x06005074 RID: 20596 RVA: 0x00191914 File Offset: 0x0018FB14
		public event Action<int, string> OnSavePrivateScanFailed;

		// Token: 0x14000091 RID: 145
		// (add) Token: 0x06005075 RID: 20597 RVA: 0x0019194C File Offset: 0x0018FB4C
		// (remove) Token: 0x06005076 RID: 20598 RVA: 0x00191984 File Offset: 0x0018FB84
		public event Action<int, bool> OnFetchPrivateScanComplete;

		// Token: 0x14000092 RID: 146
		// (add) Token: 0x06005077 RID: 20599 RVA: 0x001919BC File Offset: 0x0018FBBC
		// (remove) Token: 0x06005078 RID: 20600 RVA: 0x001919F4 File Offset: 0x0018FBF4
		public event Action<bool, SharedBlocksManager.SharedBlocksMap> OnFoundDefaultSharedBlocksMap;

		// Token: 0x14000093 RID: 147
		// (add) Token: 0x06005079 RID: 20601 RVA: 0x00191A2C File Offset: 0x0018FC2C
		// (remove) Token: 0x0600507A RID: 20602 RVA: 0x00191A64 File Offset: 0x0018FC64
		public event Action<bool> OnGetPopularMapsComplete;

		// Token: 0x14000094 RID: 148
		// (add) Token: 0x0600507B RID: 20603 RVA: 0x00191A9C File Offset: 0x0018FC9C
		// (remove) Token: 0x0600507C RID: 20604 RVA: 0x00191AD0 File Offset: 0x0018FCD0
		public static event Action OnRecentMapIdsUpdated;

		// Token: 0x14000095 RID: 149
		// (add) Token: 0x0600507D RID: 20605 RVA: 0x00191B04 File Offset: 0x0018FD04
		// (remove) Token: 0x0600507E RID: 20606 RVA: 0x00191B38 File Offset: 0x0018FD38
		public static event Action OnSaveTimeUpdated;

		// Token: 0x17000778 RID: 1912
		// (get) Token: 0x0600507F RID: 20607 RVA: 0x00191B6B File Offset: 0x0018FD6B
		public List<SharedBlocksManager.SharedBlocksMap> LatestPopularMaps
		{
			get
			{
				return this.latestPopularMaps;
			}
		}

		// Token: 0x17000779 RID: 1913
		// (get) Token: 0x06005080 RID: 20608 RVA: 0x00191B73 File Offset: 0x0018FD73
		public string[] BuildData
		{
			get
			{
				return this.privateScanDataCache;
			}
		}

		// Token: 0x06005081 RID: 20609 RVA: 0x00191B7B File Offset: 0x0018FD7B
		public bool IsWaitingOnRequest()
		{
			return this.saveScanInProgress || this.getScanInProgress;
		}

		// Token: 0x06005082 RID: 20610 RVA: 0x00191B90 File Offset: 0x0018FD90
		private void Awake()
		{
			if (SharedBlocksManager.instance == null)
			{
				SharedBlocksManager.instance = this;
				for (int i = 0; i < BuilderScanKiosk.NUM_SAVE_SLOTS; i++)
				{
					this.privateScanDataCache[i] = string.Empty;
					this.hasPulledPrivateScanMothership[i] = false;
				}
				return;
			}
			Object.Destroy(this);
		}

		// Token: 0x06005083 RID: 20611 RVA: 0x00191BE0 File Offset: 0x0018FDE0
		public void Start()
		{
			SharedBlocksManager.<Start>d__99 <Start>d__;
			<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Start>d__.<>4__this = this;
			<Start>d__.<>1__state = -1;
			<Start>d__.<>t__builder.Start<SharedBlocksManager.<Start>d__99>(ref <Start>d__);
		}

		// Token: 0x06005084 RID: 20612 RVA: 0x00191C18 File Offset: 0x0018FE18
		private bool TryGetCachedSharedBlocksMapByMapID(string mapID, out SharedBlocksManager.SharedBlocksMap result)
		{
			foreach (SharedBlocksManager.SharedBlocksMap sharedBlocksMap in this.mapResponseCache)
			{
				if (sharedBlocksMap.MapID.Equals(mapID))
				{
					result = sharedBlocksMap;
					return true;
				}
			}
			result = null;
			return false;
		}

		// Token: 0x06005085 RID: 20613 RVA: 0x00191C80 File Offset: 0x0018FE80
		private void AddMapToResponseCache(SharedBlocksManager.SharedBlocksMap map)
		{
			if (map == null)
			{
				return;
			}
			try
			{
				int num = this.mapResponseCache.FindIndex((SharedBlocksManager.SharedBlocksMap x) => x.MapID.Equals(map.MapID));
				if (num < 0)
				{
					this.mapResponseCache.Add(map);
				}
				else
				{
					this.mapResponseCache[num] = map;
				}
			}
			catch (Exception ex)
			{
				GTDev.LogError<string>("SharedBlocksManager AddMapToResponseCache Exception " + ex.ToString(), null);
			}
			if (this.mapResponseCache.Count >= 5)
			{
				this.mapResponseCache.RemoveAt(0);
			}
		}

		// Token: 0x06005086 RID: 20614 RVA: 0x00191D2C File Offset: 0x0018FF2C
		public static bool IsMapIDValid(string mapID)
		{
			if (mapID.IsNullOrEmpty())
			{
				return false;
			}
			if (mapID.Length != 8)
			{
				return false;
			}
			if (!Regex.IsMatch(mapID, "^[CFGHKMNPRTWXZ256789]+$"))
			{
				GTDev.LogError<string>("Invalid Characters in SharedBlocksManager IsMapIDValid map " + mapID, null);
				return false;
			}
			return true;
		}

		// Token: 0x06005087 RID: 20615 RVA: 0x00191D64 File Offset: 0x0018FF64
		public static LinkedList<string> GetRecentUpVotes()
		{
			return SharedBlocksManager.recentUpVotes;
		}

		// Token: 0x06005088 RID: 20616 RVA: 0x00191D6B File Offset: 0x0018FF6B
		public static List<string> GetLocalMapIDs()
		{
			return SharedBlocksManager.localMapIds;
		}

		// Token: 0x06005089 RID: 20617 RVA: 0x00191D74 File Offset: 0x0018FF74
		private static void SetPublishTimeForSlot(int slotID, DateTime time)
		{
			SharedBlocksManager.LocalPublishInfo value;
			if (SharedBlocksManager.localPublishData.TryGetValue(slotID, out value))
			{
				value.publishTime = time.ToBinary();
				SharedBlocksManager.localPublishData[slotID] = value;
				return;
			}
			SharedBlocksManager.LocalPublishInfo value2 = new SharedBlocksManager.LocalPublishInfo
			{
				mapID = null,
				publishTime = time.ToBinary()
			};
			SharedBlocksManager.localPublishData.Add(slotID, value2);
		}

		// Token: 0x0600508A RID: 20618 RVA: 0x00191DD8 File Offset: 0x0018FFD8
		private static void SetMapIDAndPublishTimeForSlot(int slotID, string mapID, DateTime time)
		{
			SharedBlocksManager.LocalPublishInfo value = new SharedBlocksManager.LocalPublishInfo
			{
				mapID = mapID,
				publishTime = time.ToBinary()
			};
			SharedBlocksManager.localPublishData.AddOrUpdate(slotID, value);
		}

		// Token: 0x0600508B RID: 20619 RVA: 0x00191E14 File Offset: 0x00190014
		public static SharedBlocksManager.LocalPublishInfo GetPublishInfoForSlot(int slot)
		{
			SharedBlocksManager.LocalPublishInfo result;
			if (SharedBlocksManager.localPublishData.TryGetValue(slot, out result))
			{
				return result;
			}
			return new SharedBlocksManager.LocalPublishInfo
			{
				mapID = null,
				publishTime = DateTime.MinValue.ToBinary()
			};
		}

		// Token: 0x0600508C RID: 20620 RVA: 0x00191E54 File Offset: 0x00190054
		private void LoadPlayerPrefs()
		{
			string recentVotesPrefsKey = this.serializationConfig.recentVotesPrefsKey;
			string localMapsPrefsKey = this.serializationConfig.localMapsPrefsKey;
			string @string = PlayerPrefs.GetString(recentVotesPrefsKey, null);
			string string2 = PlayerPrefs.GetString(localMapsPrefsKey, null);
			if (!@string.IsNullOrEmpty())
			{
				try
				{
					SharedBlocksManager.recentUpVotes = JsonConvert.DeserializeObject<LinkedList<string>>(@string);
					while (SharedBlocksManager.recentUpVotes.Count > 10)
					{
						SharedBlocksManager.recentUpVotes.RemoveLast();
					}
					goto IL_82;
				}
				catch (Exception ex)
				{
					GTDev.LogWarning<string>("SharedBlocksManager failed to deserialize Recent Up Votes " + ex.Message, null);
					SharedBlocksManager.recentUpVotes.Clear();
					goto IL_82;
				}
			}
			SharedBlocksManager.recentUpVotes.Clear();
			IL_82:
			if (!string2.IsNullOrEmpty())
			{
				SharedBlocksManager.localPublishData.Clear();
				SharedBlocksManager.localMapIds.Clear();
				try
				{
					SharedBlocksManager.localPublishData = JsonConvert.DeserializeObject<Dictionary<int, SharedBlocksManager.LocalPublishInfo>>(string2);
				}
				catch (Exception ex2)
				{
					GTDev.LogWarning<string>("SharedBlocksManager failed to deserialize localMapIDs " + ex2.Message, null);
					this.GetPlayfabLastSaveTime();
				}
				foreach (KeyValuePair<int, SharedBlocksManager.LocalPublishInfo> keyValuePair in SharedBlocksManager.localPublishData)
				{
					if (!keyValuePair.Value.mapID.IsNullOrEmpty() && SharedBlocksManager.IsMapIDValid(keyValuePair.Value.mapID))
					{
						SharedBlocksManager.localMapIds.Add(keyValuePair.Value.mapID);
					}
				}
				Action onSaveTimeUpdated = SharedBlocksManager.OnSaveTimeUpdated;
				if (onSaveTimeUpdated != null)
				{
					onSaveTimeUpdated();
				}
			}
			else
			{
				SharedBlocksManager.localMapIds.Clear();
				this.GetPlayfabLastSaveTime();
			}
			Action onRecentMapIdsUpdated = SharedBlocksManager.OnRecentMapIdsUpdated;
			if (onRecentMapIdsUpdated == null)
			{
				return;
			}
			onRecentMapIdsUpdated();
		}

		// Token: 0x0600508D RID: 20621 RVA: 0x00191FF8 File Offset: 0x001901F8
		private void SaveRecentVotesToPlayerPrefs()
		{
			PlayerPrefs.SetString(this.serializationConfig.recentVotesPrefsKey, JsonConvert.SerializeObject(SharedBlocksManager.recentUpVotes));
			PlayerPrefs.Save();
		}

		// Token: 0x0600508E RID: 20622 RVA: 0x00192019 File Offset: 0x00190219
		private void SaveLocalMapIdsToPlayerPrefs()
		{
			PlayerPrefs.SetString(this.serializationConfig.localMapsPrefsKey, JsonConvert.SerializeObject(SharedBlocksManager.localPublishData));
			PlayerPrefs.Save();
		}

		// Token: 0x0600508F RID: 20623 RVA: 0x0019203C File Offset: 0x0019023C
		public void RequestVote(string mapID, bool up, Action<bool, string> callback)
		{
			if (!MothershipClientContext.IsClientLoggedIn())
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestVote Client Not Logged into Mothership", null);
				if (callback != null)
				{
					callback(false, "NOT LOGGED IN");
				}
				return;
			}
			if (this.voteInProgress)
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestVote already in progress", null);
				return;
			}
			this.voteInProgress = true;
			base.StartCoroutine(this.PostVote(new SharedBlocksManager.VoteRequest
			{
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				mapId = mapID,
				vote = (up ? 1 : -1)
			}, callback));
		}

		// Token: 0x06005090 RID: 20624 RVA: 0x001920C3 File Offset: 0x001902C3
		private IEnumerator PostVote(SharedBlocksManager.VoteRequest data, Action<bool, string> callback)
		{
			UnityWebRequest request = new UnityWebRequest(this.serializationConfig.sharedBlocksApiBaseURL + "/api/MapVote", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if (request.result == UnityWebRequest.Result.Success)
			{
				string mapId = data.mapId;
				if (data.vote == -1)
				{
					if (SharedBlocksManager.recentUpVotes.Remove(mapId))
					{
						this.SaveRecentVotesToPlayerPrefs();
						Action onRecentMapIdsUpdated = SharedBlocksManager.OnRecentMapIdsUpdated;
						if (onRecentMapIdsUpdated != null)
						{
							onRecentMapIdsUpdated();
						}
					}
				}
				else if (!SharedBlocksManager.recentUpVotes.Contains(mapId))
				{
					if (SharedBlocksManager.recentUpVotes.Count >= 10)
					{
						SharedBlocksManager.recentUpVotes.RemoveLast();
					}
					SharedBlocksManager.recentUpVotes.AddFirst(mapId);
					this.SaveRecentVotesToPlayerPrefs();
					Action onRecentMapIdsUpdated2 = SharedBlocksManager.OnRecentMapIdsUpdated;
					if (onRecentMapIdsUpdated2 != null)
					{
						onRecentMapIdsUpdated2();
					}
				}
				this.voteInProgress = false;
				if (callback != null)
				{
					callback(true, "");
				}
			}
			else
			{
				GTDev.LogError<string>(string.Format("PostVote Error: {0} -- raw response: ", request.responseCode) + request.downloadHandler.text, null);
				long responseCode = request.responseCode;
				if (responseCode > 500L && responseCode < 600L)
				{
					retry = true;
				}
				else if (request.result == UnityWebRequest.Result.ConnectionError)
				{
					retry = true;
				}
				else
				{
					this.voteInProgress = false;
					if (callback != null)
					{
						callback(false, "REQUEST ERROR");
					}
				}
			}
			if (retry)
			{
				if (this.voteRetryCount < this.maxRetriesOnFail)
				{
					int num = (int)Mathf.Pow(2f, (float)(this.voteRetryCount + 1));
					this.voteRetryCount++;
					yield return new WaitForSeconds((float)num);
					this.voteInProgress = false;
					this.RequestVote(data.mapId, data.vote == 1, callback);
				}
				else
				{
					this.voteRetryCount = 0;
					this.voteInProgress = false;
					if (callback != null)
					{
						callback(false, "CONNECTION ERROR");
					}
				}
			}
			yield break;
		}

		// Token: 0x06005091 RID: 20625 RVA: 0x001920E0 File Offset: 0x001902E0
		private void RequestPublishMap(string userMetadataKey)
		{
			if (!MothershipClientContext.IsClientLoggedIn())
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestPublishMap Client Not Logged into Mothership", null);
				this.PublishMapComplete(false, userMetadataKey, string.Empty, 0L);
				return;
			}
			if (this.publishRequestInProgress)
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestPublishMap Publish Request in progress", null);
				return;
			}
			this.publishRequestInProgress = true;
			base.StartCoroutine(this.PostPublishMapRequest(new SharedBlocksManager.PublishMapRequestData
			{
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				userdataMetadataKey = userMetadataKey,
				playerNickname = GorillaTagger.Instance.offlineVRRig.playerNameVisible
			}, new SharedBlocksManager.PublishMapRequestCallback(this.PublishMapComplete)));
		}

		// Token: 0x06005092 RID: 20626 RVA: 0x0019217C File Offset: 0x0019037C
		private void PublishMapComplete(bool success, string key, [CanBeNull] string mapID, long response)
		{
			this.publishRequestInProgress = false;
			if (success)
			{
				int num = this.serializationConfig.scanSlotMothershipKeys.IndexOf(key);
				if (num >= 0)
				{
					SharedBlocksManager.LocalPublishInfo localPublishInfo;
					if (SharedBlocksManager.localPublishData.TryGetValue(num, out localPublishInfo))
					{
						SharedBlocksManager.localMapIds.Remove(localPublishInfo.mapID);
					}
					SharedBlocksManager.SetMapIDAndPublishTimeForSlot(num, mapID, DateTime.Now);
					this.SaveLocalMapIdsToPlayerPrefs();
				}
				if (!SharedBlocksManager.localMapIds.Contains(mapID))
				{
					SharedBlocksManager.localMapIds.Add(mapID);
					Action onRecentMapIdsUpdated = SharedBlocksManager.OnRecentMapIdsUpdated;
					if (onRecentMapIdsUpdated != null)
					{
						onRecentMapIdsUpdated();
					}
				}
				SharedBlocksManager.SharedBlocksMap map = new SharedBlocksManager.SharedBlocksMap
				{
					MapID = mapID,
					MapData = this.privateScanDataCache[num],
					CreatorNickName = GorillaTagger.Instance.offlineVRRig.playerNameVisible,
					UpdateTime = DateTime.Now
				};
				this.AddMapToResponseCache(map);
				Action<int> onSavePrivateScanSuccess = this.OnSavePrivateScanSuccess;
				if (onSavePrivateScanSuccess != null)
				{
					onSavePrivateScanSuccess(this.currentSaveScanIndex);
				}
			}
			else
			{
				Action<int, string> onSavePrivateScanFailed = this.OnSavePrivateScanFailed;
				if (onSavePrivateScanFailed != null)
				{
					onSavePrivateScanFailed(this.currentSaveScanIndex, "ERROR PUBLISHING: " + response.ToString());
				}
			}
			this.currentSaveScanIndex = -1;
			this.currentSaveScanData = string.Empty;
		}

		// Token: 0x06005093 RID: 20627 RVA: 0x0019229B File Offset: 0x0019049B
		private IEnumerator PostPublishMapRequest(SharedBlocksManager.PublishMapRequestData data, SharedBlocksManager.PublishMapRequestCallback callback)
		{
			UnityWebRequest request = new UnityWebRequest(this.serializationConfig.sharedBlocksApiBaseURL + "/api/Publish", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if (request.result == UnityWebRequest.Result.Success)
			{
				GTDev.Log<string>("PostPublishMapRequest Success: raw response: " + request.downloadHandler.text, null);
				try
				{
					string text = request.downloadHandler.text;
					bool success = !text.IsNullOrEmpty() && SharedBlocksManager.IsMapIDValid(text);
					if (callback != null)
					{
						callback(success, data.userdataMetadataKey, text, request.responseCode);
					}
					goto IL_1F8;
				}
				catch (Exception ex)
				{
					GTDev.LogError<string>("SharedBlocksManager PostPublishMapRequest " + ex.Message, null);
					if (callback != null)
					{
						callback(false, data.userdataMetadataKey, null, request.responseCode);
					}
					goto IL_1F8;
				}
			}
			long responseCode = request.responseCode;
			if (responseCode > 500L && responseCode < 600L)
			{
				retry = true;
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				retry = true;
			}
			else if (callback != null)
			{
				callback(false, data.userdataMetadataKey, string.Empty, request.responseCode);
			}
			IL_1F8:
			if (retry)
			{
				if (this.postPublishMapRetryCount < this.maxRetriesOnFail)
				{
					int num = (int)Mathf.Pow(2f, (float)(this.postPublishMapRetryCount + 1));
					this.postPublishMapRetryCount++;
					yield return new WaitForSeconds((float)num);
					this.publishRequestInProgress = false;
					this.RequestPublishMap(data.userdataMetadataKey);
				}
				else
				{
					this.postPublishMapRetryCount = 0;
					if (callback != null)
					{
						callback(false, data.userdataMetadataKey, string.Empty, request.responseCode);
					}
				}
			}
			yield break;
		}

		// Token: 0x06005094 RID: 20628 RVA: 0x001922B8 File Offset: 0x001904B8
		public void RequestMapDataFromID(string mapID, SharedBlocksManager.BlocksMapRequestCallback callback)
		{
			if (!MothershipClientContext.IsClientLoggedIn())
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestMapDataFromID Client Not Logged into Mothership", null);
				if (callback != null)
				{
					callback(null);
				}
				return;
			}
			SharedBlocksManager.SharedBlocksMap response;
			if (this.TryGetCachedSharedBlocksMapByMapID(mapID, out response))
			{
				if (callback != null)
				{
					callback(response);
				}
				return;
			}
			if (this.getMapDataFromIDInProgress)
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestMapDataFromID Fetch already in progress", null);
				return;
			}
			this.getMapDataFromIDInProgress = true;
			base.StartCoroutine(this.GetMapDataFromID(new SharedBlocksManager.GetMapDataFromIDRequest
			{
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				mapId = mapID
			}, callback));
		}

		// Token: 0x06005095 RID: 20629 RVA: 0x00192343 File Offset: 0x00190543
		private IEnumerator GetMapDataFromID(SharedBlocksManager.GetMapDataFromIDRequest data, SharedBlocksManager.BlocksMapRequestCallback callback)
		{
			UnityWebRequest request = new UnityWebRequest(this.serializationConfig.sharedBlocksApiBaseURL + "/api/GetMapData", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if (request.result == UnityWebRequest.Result.Success)
			{
				string text = request.downloadHandler.text;
				this.GetMapDataFromIDComplete(data.mapId, text, callback);
			}
			else
			{
				long responseCode = request.responseCode;
				if (responseCode > 500L && responseCode < 600L)
				{
					retry = true;
				}
				else if (request.result == UnityWebRequest.Result.ConnectionError)
				{
					retry = true;
				}
				else
				{
					this.GetMapDataFromIDComplete(data.mapId, null, callback);
				}
			}
			if (retry)
			{
				if (this.getMapDataFromIDRetryCount < this.maxRetriesOnFail)
				{
					int num = (int)Mathf.Pow(2f, (float)(this.getMapDataFromIDRetryCount + 1));
					this.getMapDataFromIDRetryCount++;
					yield return new WaitForSeconds((float)num);
					this.getMapDataFromIDInProgress = false;
					this.RequestMapDataFromID(data.mapId, callback);
				}
				else
				{
					this.getMapDataFromIDRetryCount = 0;
					this.GetMapDataFromIDComplete(data.mapId, null, callback);
				}
			}
			yield break;
		}

		// Token: 0x06005096 RID: 20630 RVA: 0x00192360 File Offset: 0x00190560
		private void GetMapDataFromIDComplete(string mapID, [CanBeNull] string response, SharedBlocksManager.BlocksMapRequestCallback callback)
		{
			this.getMapDataFromIDInProgress = false;
			if (response == null)
			{
				if (callback != null)
				{
					callback(null);
					return;
				}
			}
			else
			{
				SharedBlocksManager.SharedBlocksMap sharedBlocksMap = new SharedBlocksManager.SharedBlocksMap
				{
					MapID = mapID,
					MapData = response
				};
				this.AddMapToResponseCache(sharedBlocksMap);
				if (callback != null)
				{
					callback(sharedBlocksMap);
				}
			}
		}

		// Token: 0x06005097 RID: 20631 RVA: 0x001923A8 File Offset: 0x001905A8
		public bool RequestGetTopMaps(int pageNum, int pageSize, string sort)
		{
			if (!MothershipClientContext.IsClientLoggedIn())
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestFetchPopularBlocksMaps Client Not Logged into Mothership", null);
				return false;
			}
			if (this.getTopMapsInProgress)
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestFetchPopularBlocksMaps already in progress", null);
				return false;
			}
			this.getTopMapsInProgress = true;
			this.lastGetTopMapsTime = Time.timeAsDouble;
			base.StartCoroutine(this.GetTopMaps(new SharedBlocksManager.GetMapsRequest
			{
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				page = pageNum,
				pageSize = pageSize,
				sort = sort,
				ShowInactive = false
			}, new Action<List<SharedBlocksManager.SharedBlocksMapMetaData>>(this.GetTopMapsComplete)));
			return true;
		}

		// Token: 0x06005098 RID: 20632 RVA: 0x00192441 File Offset: 0x00190641
		private IEnumerator GetTopMaps(SharedBlocksManager.GetMapsRequest data, Action<List<SharedBlocksManager.SharedBlocksMapMetaData>> callback)
		{
			UnityWebRequest request = new UnityWebRequest(this.serializationConfig.sharedBlocksApiBaseURL + "/api/GetMaps", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if (request.result == UnityWebRequest.Result.Success)
			{
				try
				{
					List<SharedBlocksManager.SharedBlocksMapMetaData> obj = JsonConvert.DeserializeObject<List<SharedBlocksManager.SharedBlocksMapMetaData>>(request.downloadHandler.text);
					if (callback != null)
					{
						callback(obj);
					}
					goto IL_162;
				}
				catch (Exception)
				{
					if (callback != null)
					{
						callback(null);
					}
					goto IL_162;
				}
			}
			long responseCode = request.responseCode;
			if (responseCode > 500L && responseCode < 600L)
			{
				retry = true;
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				retry = true;
			}
			else if (callback != null)
			{
				callback(null);
			}
			IL_162:
			if (retry)
			{
				if (this.getTopMapsRetryCount < this.maxRetriesOnFail)
				{
					int num = (int)Mathf.Pow(2f, (float)(this.getTopMapsRetryCount + 1));
					this.getTopMapsRetryCount++;
					yield return new WaitForSeconds((float)num);
					this.getTopMapsInProgress = false;
					this.RequestGetTopMaps(data.page, data.pageSize, data.sort);
				}
				else
				{
					this.getTopMapsRetryCount = 0;
					if (callback != null)
					{
						callback(null);
					}
				}
			}
			yield break;
		}

		// Token: 0x06005099 RID: 20633 RVA: 0x00192460 File Offset: 0x00190660
		private void GetTopMapsComplete([CanBeNull] List<SharedBlocksManager.SharedBlocksMapMetaData> maps)
		{
			this.getTopMapsInProgress = false;
			if (maps != null)
			{
				this.latestPopularMaps.Clear();
				foreach (SharedBlocksManager.SharedBlocksMapMetaData sharedBlocksMapMetaData in maps)
				{
					if (sharedBlocksMapMetaData != null && SharedBlocksManager.IsMapIDValid(sharedBlocksMapMetaData.mapId))
					{
						DateTime createTime = DateTime.MinValue;
						DateTime updateTime = DateTime.MinValue;
						try
						{
							createTime = DateTime.Parse(sharedBlocksMapMetaData.createdTime);
							updateTime = DateTime.Parse(sharedBlocksMapMetaData.updatedTime);
						}
						catch (Exception ex)
						{
							GTDev.LogWarning<string>("SharedBlocksManager GetTopMaps bad update or create time" + ex.Message, null);
						}
						SharedBlocksManager.SharedBlocksMap item = new SharedBlocksManager.SharedBlocksMap
						{
							MapID = sharedBlocksMapMetaData.mapId,
							CreatorID = null,
							CreatorNickName = sharedBlocksMapMetaData.nickname,
							CreateTime = createTime,
							UpdateTime = updateTime,
							MapData = null
						};
						this.latestPopularMaps.Add(item);
					}
				}
				this.hasCachedTopMaps = true;
				Action<bool> onGetPopularMapsComplete = this.OnGetPopularMapsComplete;
				if (onGetPopularMapsComplete == null)
				{
					return;
				}
				onGetPopularMapsComplete(true);
				return;
			}
			else
			{
				Action<bool> onGetPopularMapsComplete2 = this.OnGetPopularMapsComplete;
				if (onGetPopularMapsComplete2 == null)
				{
					return;
				}
				onGetPopularMapsComplete2(false);
				return;
			}
		}

		// Token: 0x0600509A RID: 20634 RVA: 0x0019259C File Offset: 0x0019079C
		private void RequestUpdateMapActive(string userMetadataKey, bool active)
		{
			if (!MothershipClientContext.IsClientLoggedIn())
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestUpdateMapActive Client Not Logged into Mothership", null);
				return;
			}
			if (this.updateMapActiveInProgress)
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestUpdateMapActive already in progress", null);
				return;
			}
			this.updateMapActiveInProgress = true;
			base.StartCoroutine(this.PostUpdateMapActive(new SharedBlocksManager.UpdateMapActiveRequest
			{
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				userdataMetadataKey = userMetadataKey,
				setActive = active
			}, new Action<bool>(this.OnUpdatedMapActiveComplete)));
		}

		// Token: 0x0600509B RID: 20635 RVA: 0x00192619 File Offset: 0x00190819
		private IEnumerator PostUpdateMapActive(SharedBlocksManager.UpdateMapActiveRequest data, Action<bool> callback)
		{
			UnityWebRequest request = new UnityWebRequest(this.serializationConfig.sharedBlocksApiBaseURL + "/api/UpdateMapActive", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if (request.result == UnityWebRequest.Result.Success)
			{
				if (callback != null)
				{
					callback(true);
				}
			}
			else
			{
				long responseCode = request.responseCode;
				if (responseCode > 500L && responseCode < 600L)
				{
					retry = true;
				}
				else if (request.result == UnityWebRequest.Result.ConnectionError)
				{
					retry = true;
				}
				else if (callback != null)
				{
					callback(false);
				}
			}
			if (retry)
			{
				if (this.updateMapActiveRetryCount < this.maxRetriesOnFail)
				{
					int num = (int)Mathf.Pow(2f, (float)(this.updateMapActiveRetryCount + 1));
					this.updateMapActiveRetryCount++;
					yield return new WaitForSeconds((float)num);
					this.updateMapActiveInProgress = false;
					this.RequestUpdateMapActive(data.userdataMetadataKey, data.setActive);
				}
				else
				{
					this.updateMapActiveRetryCount = 0;
					if (callback != null)
					{
						callback(false);
					}
				}
			}
			yield break;
		}

		// Token: 0x0600509C RID: 20636 RVA: 0x00192636 File Offset: 0x00190836
		private void OnUpdatedMapActiveComplete(bool success)
		{
			this.updateMapActiveInProgress = false;
		}

		// Token: 0x0600509D RID: 20637 RVA: 0x00192640 File Offset: 0x00190840
		private Task WaitForPlayfabSessionToken()
		{
			SharedBlocksManager.<WaitForPlayfabSessionToken>d__125 <WaitForPlayfabSessionToken>d__;
			<WaitForPlayfabSessionToken>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitForPlayfabSessionToken>d__.<>1__state = -1;
			<WaitForPlayfabSessionToken>d__.<>t__builder.Start<SharedBlocksManager.<WaitForPlayfabSessionToken>d__125>(ref <WaitForPlayfabSessionToken>d__);
			return <WaitForPlayfabSessionToken>d__.<>t__builder.Task;
		}

		// Token: 0x0600509E RID: 20638 RVA: 0x0019267B File Offset: 0x0019087B
		public void RequestTableConfiguration()
		{
			if (this.fetchedTableConfig)
			{
				Action<string> onGetTableConfiguration = this.OnGetTableConfiguration;
				if (onGetTableConfiguration == null)
				{
					return;
				}
				onGetTableConfiguration(this.tableConfigResponse);
			}
		}

		// Token: 0x0600509F RID: 20639 RVA: 0x0019269C File Offset: 0x0019089C
		private void FetchConfigurationFromTitleData()
		{
			PlayFabClientAPI.GetTitleData(new GetTitleDataRequest
			{
				Keys = new List<string>
				{
					this.serializationConfig.tableConfigurationKey
				}
			}, new Action<GetTitleDataResult>(this.OnGetConfigurationSuccess), new Action<PlayFabError>(this.OnGetConfigurationFail), null, null);
		}

		// Token: 0x060050A0 RID: 20640 RVA: 0x001926EC File Offset: 0x001908EC
		private void OnGetConfigurationSuccess(GetTitleDataResult result)
		{
			GTDev.Log<string>("SharedBlocksManager OnGetConfigurationSuccess", null);
			string text;
			if (result.Data.TryGetValue(this.serializationConfig.tableConfigurationKey, out text))
			{
				this.tableConfigResponse = text;
				this.fetchedTableConfig = true;
				Action<string> onGetTableConfiguration = this.OnGetTableConfiguration;
				if (onGetTableConfiguration == null)
				{
					return;
				}
				onGetTableConfiguration(this.tableConfigResponse);
			}
		}

		// Token: 0x060050A1 RID: 20641 RVA: 0x00192744 File Offset: 0x00190944
		private void OnGetConfigurationFail(PlayFabError error)
		{
			GTDev.LogWarning<string>("SharedBlocksManager OnGetConfigurationFail " + error.Error.ToString(), null);
			if (error.Error == PlayFabErrorCode.ConnectionError && this.fetchTableConfigRetryCount < this.maxRetriesOnFail)
			{
				int waitTime = (int)Mathf.Pow(2f, (float)(this.fetchTableConfigRetryCount + 1));
				this.fetchTableConfigRetryCount++;
				base.StartCoroutine(this.RetryAfterWaitTime(waitTime, new Action(this.FetchConfigurationFromTitleData)));
				return;
			}
			this.tableConfigResponse = string.Empty;
			this.fetchedTableConfig = true;
			Action<string> onGetTableConfiguration = this.OnGetTableConfiguration;
			if (onGetTableConfiguration == null)
			{
				return;
			}
			onGetTableConfiguration(this.tableConfigResponse);
		}

		// Token: 0x060050A2 RID: 20642 RVA: 0x001927EF File Offset: 0x001909EF
		private IEnumerator RetryAfterWaitTime(int waitTime, Action function)
		{
			yield return new WaitForSeconds((float)waitTime);
			if (function != null)
			{
				function();
			}
			yield break;
		}

		// Token: 0x060050A3 RID: 20643 RVA: 0x00192808 File Offset: 0x00190A08
		public void FetchTitleDataBuild()
		{
			if (!this.fetchTitleDataBuildComplete)
			{
				if (!this.fetchTitleDataBuildInProgress)
				{
					this.fetchTitleDataBuildInProgress = true;
					base.StartCoroutine(this.SendTitleDataRequest(new GetTitleDataRequest
					{
						Keys = new List<string>
						{
							this.serializationConfig.titleDataKey
						}
					}, new Action<GetTitleDataResult>(this.OnGetTitleDataBuildSuccess), new Action<PlayFabError>(this.OnGetTitleDataBuildFail)));
				}
				return;
			}
			Action<string> onGetTitleDataBuildComplete = this.OnGetTitleDataBuildComplete;
			if (onGetTitleDataBuildComplete == null)
			{
				return;
			}
			onGetTitleDataBuildComplete(this.titleDataBuildCache);
		}

		// Token: 0x060050A4 RID: 20644 RVA: 0x00192889 File Offset: 0x00190A89
		private IEnumerator SendTitleDataRequest(GetTitleDataRequest request, Action<GetTitleDataResult> successCallback, Action<PlayFabError> failCallback)
		{
			while (!PlayFabSettings.staticPlayer.IsClientLoggedIn())
			{
				yield return new WaitForSeconds(5f);
			}
			PlayFabClientAPI.GetTitleData(request, successCallback, failCallback, null, null);
			yield break;
		}

		// Token: 0x060050A5 RID: 20645 RVA: 0x001928A8 File Offset: 0x00190AA8
		private void OnGetTitleDataBuildSuccess(GetTitleDataResult result)
		{
			this.fetchTitleDataBuildInProgress = false;
			GTDev.Log<string>("SharedBlocksManager OnGetTitleDataBuildSuccess", null);
			string s;
			if (result.Data.TryGetValue(this.serializationConfig.titleDataKey, out s) && !s.IsNullOrEmpty())
			{
				this.titleDataBuildCache = s;
				this.fetchTitleDataBuildComplete = true;
				Action<string> onGetTitleDataBuildComplete = this.OnGetTitleDataBuildComplete;
				if (onGetTitleDataBuildComplete == null)
				{
					return;
				}
				onGetTitleDataBuildComplete(this.titleDataBuildCache);
				return;
			}
			else
			{
				this.titleDataBuildCache = string.Empty;
				this.fetchTitleDataBuildComplete = true;
				Action<string> onGetTitleDataBuildComplete2 = this.OnGetTitleDataBuildComplete;
				if (onGetTitleDataBuildComplete2 == null)
				{
					return;
				}
				onGetTitleDataBuildComplete2(this.titleDataBuildCache);
				return;
			}
		}

		// Token: 0x060050A6 RID: 20646 RVA: 0x00192938 File Offset: 0x00190B38
		private void OnGetTitleDataBuildFail(PlayFabError error)
		{
			this.fetchTitleDataBuildInProgress = false;
			GTDev.LogWarning<string>("SharedBlocksManager FetchTitleDataBuildFail " + error.Error.ToString(), null);
			if (error.Error == PlayFabErrorCode.ConnectionError && this.fetchTitleDataRetryCount < this.maxRetriesOnFail)
			{
				int waitTime = (int)Mathf.Pow(2f, (float)(this.fetchTitleDataRetryCount + 1));
				this.fetchTitleDataRetryCount++;
				base.StartCoroutine(this.RetryAfterWaitTime(waitTime, new Action(this.FetchTitleDataBuild)));
				return;
			}
			this.titleDataBuildCache = string.Empty;
			this.fetchTitleDataBuildComplete = true;
			Action<string> onGetTitleDataBuildComplete = this.OnGetTitleDataBuildComplete;
			if (onGetTitleDataBuildComplete == null)
			{
				return;
			}
			onGetTitleDataBuildComplete(this.titleDataBuildCache);
		}

		// Token: 0x060050A7 RID: 20647 RVA: 0x001929EA File Offset: 0x00190BEA
		private string GetPlayfabKeyForSlot(int slot)
		{
			return this.serializationConfig.playfabScanKey + slot.ToString("D2");
		}

		// Token: 0x060050A8 RID: 20648 RVA: 0x00192A08 File Offset: 0x00190C08
		private string GetPlayfabSlotTimeKey(int slot)
		{
			return this.serializationConfig.playfabScanKey + slot.ToString("D2") + this.serializationConfig.timeAppend;
		}

		// Token: 0x060050A9 RID: 20649 RVA: 0x00192A34 File Offset: 0x00190C34
		private void GetPlayfabLastSaveTime()
		{
			if (!this.hasQueriedSaveTime)
			{
				PlayFab.ClientModels.GetUserDataRequest request = new PlayFab.ClientModels.GetUserDataRequest
				{
					PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
					Keys = SharedBlocksManager.saveDateKeys
				};
				try
				{
					PlayFabClientAPI.GetUserData(request, new Action<GetUserDataResult>(this.OnGetLastSaveTimeSuccess), new Action<PlayFabError>(this.OnGetLastSaveTimeFailure), null, null);
				}
				catch (PlayFabException ex)
				{
					this.OnGetLastSaveTimeFailure(new PlayFabError
					{
						Error = PlayFabErrorCode.Unknown,
						ErrorMessage = ex.Message
					});
				}
				this.hasQueriedSaveTime = true;
				return;
			}
			Action onSaveTimeUpdated = SharedBlocksManager.OnSaveTimeUpdated;
			if (onSaveTimeUpdated == null)
			{
				return;
			}
			onSaveTimeUpdated();
		}

		// Token: 0x060050AA RID: 20650 RVA: 0x00192AD8 File Offset: 0x00190CD8
		private void OnGetLastSaveTimeSuccess(GetUserDataResult result)
		{
			bool flag = false;
			for (int i = 0; i < BuilderScanKiosk.NUM_SAVE_SLOTS; i++)
			{
				UserDataRecord userDataRecord;
				if (result.Data.TryGetValue(this.GetPlayfabSlotTimeKey(i), out userDataRecord))
				{
					flag = true;
					DateTime lastUpdated = userDataRecord.LastUpdated;
					SharedBlocksManager.SetPublishTimeForSlot(i, lastUpdated + DateTimeOffset.Now.Offset);
				}
			}
			if (flag)
			{
				this.SaveLocalMapIdsToPlayerPrefs();
			}
			Action onSaveTimeUpdated = SharedBlocksManager.OnSaveTimeUpdated;
			if (onSaveTimeUpdated == null)
			{
				return;
			}
			onSaveTimeUpdated();
		}

		// Token: 0x060050AB RID: 20651 RVA: 0x00192B48 File Offset: 0x00190D48
		private void OnGetLastSaveTimeFailure(PlayFabError error)
		{
			string str = ((error != null) ? error.ErrorMessage : null) ?? "Null";
			GTDev.LogError<string>("SharedBlocksManager GetLastSaveTimeFailure " + str, null);
		}

		// Token: 0x060050AC RID: 20652 RVA: 0x00192B7C File Offset: 0x00190D7C
		private void FetchBuildFromPlayfab()
		{
			if (this.hasPulledPrivateScanPlayfab[this.currentGetScanIndex])
			{
				Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
				if (onFetchPrivateScanComplete != null)
				{
					onFetchPrivateScanComplete(this.currentGetScanIndex, true);
				}
				this.currentGetScanIndex = -1;
				this.getScanInProgress = false;
				return;
			}
			PlayFab.ClientModels.GetUserDataRequest request = new PlayFab.ClientModels.GetUserDataRequest
			{
				PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
				Keys = new List<string>
				{
					this.GetPlayfabKeyForSlot(this.currentGetScanIndex)
				}
			};
			base.StartCoroutine(this.SendPlayfabUserDataRequest(request, new Action<GetUserDataResult>(this.OnFetchBuildFromPlayfabSuccess), new Action<PlayFabError>(this.OnFetchBuildFromPlayfabFail)));
		}

		// Token: 0x060050AD RID: 20653 RVA: 0x00192C1A File Offset: 0x00190E1A
		private IEnumerator SendPlayfabUserDataRequest(PlayFab.ClientModels.GetUserDataRequest request, Action<GetUserDataResult> resultCallback, Action<PlayFabError> errorCallback)
		{
			while (!PlayFabSettings.staticPlayer.IsClientLoggedIn())
			{
				yield return new WaitForSeconds(5f);
			}
			try
			{
				PlayFabClientAPI.GetUserData(request, resultCallback, errorCallback, null, null);
				yield break;
			}
			catch (PlayFabException ex)
			{
				if (errorCallback != null)
				{
					errorCallback(new PlayFabError
					{
						Error = PlayFabErrorCode.Unknown,
						ErrorMessage = ex.Message
					});
				}
				yield break;
			}
			yield break;
		}

		// Token: 0x060050AE RID: 20654 RVA: 0x00192C38 File Offset: 0x00190E38
		private void OnFetchBuildFromPlayfabSuccess(GetUserDataResult result)
		{
			this.getScanInProgress = false;
			GTDev.Log<string>("SharedBlocksManager OnFetchBuildsFromPlayfabSuccess", null);
			UserDataRecord userDataRecord;
			if (result != null && result.Data != null && result.Data.TryGetValue(this.GetPlayfabKeyForSlot(this.currentGetScanIndex), out userDataRecord))
			{
				this.privateScanDataCache[this.currentGetScanIndex] = userDataRecord.Value;
				this.hasPulledPrivateScanPlayfab[this.currentGetScanIndex] = true;
				if (!userDataRecord.Value.IsNullOrEmpty())
				{
					this.RequestSavePrivateScan(this.currentGetScanIndex, userDataRecord.Value);
				}
			}
			else
			{
				this.privateScanDataCache[this.currentGetScanIndex] = string.Empty;
				this.hasPulledPrivateScanPlayfab[this.currentGetScanIndex] = true;
			}
			Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
			if (onFetchPrivateScanComplete != null)
			{
				onFetchPrivateScanComplete(this.currentGetScanIndex, true);
			}
			this.currentGetScanIndex = -1;
		}

		// Token: 0x060050AF RID: 20655 RVA: 0x00192D00 File Offset: 0x00190F00
		private void OnFetchBuildFromPlayfabFail(PlayFabError error)
		{
			GTDev.LogWarning<string>("SharedBlocksManager OnFetchBuildsFromPlayfabFail " + (((error != null) ? error.ErrorMessage : null) ?? "Null"), null);
			if (error != null && error.Error == PlayFabErrorCode.ConnectionError && this.fetchPlayfabBuildsRetryCount < this.maxRetriesOnFail)
			{
				int waitTime = (int)Mathf.Pow(2f, (float)(this.fetchPlayfabBuildsRetryCount + 1));
				this.fetchPlayfabBuildsRetryCount++;
				base.StartCoroutine(this.RetryAfterWaitTime(waitTime, new Action(this.FetchBuildFromPlayfab)));
				return;
			}
			this.privateScanDataCache[this.currentGetScanIndex] = string.Empty;
			this.hasPulledPrivateScanPlayfab[this.currentGetScanIndex] = true;
			this.getScanInProgress = false;
			Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
			if (onFetchPrivateScanComplete != null)
			{
				onFetchPrivateScanComplete(this.currentGetScanIndex, false);
			}
			this.currentGetScanIndex = -1;
		}

		// Token: 0x060050B0 RID: 20656 RVA: 0x00192DD0 File Offset: 0x00190FD0
		private Task WaitForMothership()
		{
			SharedBlocksManager.<WaitForMothership>d__144 <WaitForMothership>d__;
			<WaitForMothership>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitForMothership>d__.<>1__state = -1;
			<WaitForMothership>d__.<>t__builder.Start<SharedBlocksManager.<WaitForMothership>d__144>(ref <WaitForMothership>d__);
			return <WaitForMothership>d__.<>t__builder.Task;
		}

		// Token: 0x060050B1 RID: 20657 RVA: 0x00192E0C File Offset: 0x0019100C
		public void RequestSavePrivateScan(int scanIndex, string scanData)
		{
			if (scanIndex < 0 || scanIndex >= this.serializationConfig.scanSlotMothershipKeys.Count)
			{
				GTDev.LogError<string>(string.Format("SharedBlocksManager RequestSaveScanToMothership: scan index {0} out of bounds", scanIndex), null);
				return;
			}
			this.currentSaveScanIndex = scanIndex;
			this.currentSaveScanData = scanData;
			if (!this.hasPulledPrivateScanMothership[scanIndex])
			{
				this.PullMothershipPrivateScanThenPush(scanIndex);
				return;
			}
			this.privateScanDataCache[scanIndex] = scanData;
			this.RequestSetMothershipUserData(this.serializationConfig.scanSlotMothershipKeys[scanIndex], scanData);
		}

		// Token: 0x060050B2 RID: 20658 RVA: 0x00192E88 File Offset: 0x00191088
		private void PullMothershipPrivateScanThenPush(int scanIndex)
		{
			if (this.getScanInProgress && this.currentGetScanIndex != scanIndex)
			{
				GTDev.LogWarning<string>("SharedBLocksManager PullMothershipPrivateScanThenPush GetScan in progress", null);
				Action<int, string> onSavePrivateScanFailed = this.OnSavePrivateScanFailed;
				if (onSavePrivateScanFailed != null)
				{
					onSavePrivateScanFailed(scanIndex, "ERROR SAVING: BUSY");
				}
				this.currentSaveScanIndex = -1;
				this.currentSaveScanData = string.Empty;
				return;
			}
			this.OnFetchPrivateScanComplete += this.PushMothershipPrivateScan;
			this.RequestFetchPrivateScan(scanIndex);
		}

		// Token: 0x060050B3 RID: 20659 RVA: 0x00192EF4 File Offset: 0x001910F4
		private void PushMothershipPrivateScan(int scan, bool success)
		{
			if (scan == this.currentSaveScanIndex)
			{
				this.OnFetchPrivateScanComplete -= this.PushMothershipPrivateScan;
				this.privateScanDataCache[this.currentSaveScanIndex] = this.currentSaveScanData;
				this.RequestSetMothershipUserData(this.serializationConfig.scanSlotMothershipKeys[this.currentSaveScanIndex], this.currentSaveScanData);
			}
		}

		// Token: 0x060050B4 RID: 20660 RVA: 0x00192F54 File Offset: 0x00191154
		private void RequestSetMothershipUserData(string keyName, string value)
		{
			if (this.saveScanInProgress)
			{
				Debug.LogError("SharedBlocksManager RequestSetMothershipUserData: request already in progress");
				return;
			}
			this.saveScanInProgress = true;
			try
			{
				if (!MothershipClientApiUnity.SetUserDataValue(keyName, value, new Action<SetUserDataResponse>(this.OnSetMothershipUserDataSuccess), new Action<MothershipError, int>(this.OnSetMothershipUserDataFail), ""))
				{
					Debug.LogError("SharedBlocksManager RequestSetMothershipUserData: SetUserDataValue Fail");
					this.OnSetMothershipDataComplete(false);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("SharedBlocksManager RequestSetMothershipUserData: exception " + ex.Message);
				this.OnSetMothershipDataComplete(false);
			}
		}

		// Token: 0x060050B5 RID: 20661 RVA: 0x00192FE4 File Offset: 0x001911E4
		private void OnSetMothershipUserDataSuccess(SetUserDataResponse response)
		{
			GTDev.Log<string>("SharedBlocksManager OnSetMothershipUserDataSuccess", null);
			this.OnSetMothershipDataComplete(true);
			response.Dispose();
		}

		// Token: 0x060050B6 RID: 20662 RVA: 0x00193000 File Offset: 0x00191200
		private void OnSetMothershipUserDataFail(MothershipError error, int status)
		{
			string str = (error == null) ? status.ToString() : error.Message;
			GTDev.LogError<string>("SharedBlocksManager OnSetMothershipUserDataFail: " + str, null);
			this.OnSetMothershipDataComplete(false);
			if (error != null)
			{
				error.Dispose();
			}
		}

		// Token: 0x060050B7 RID: 20663 RVA: 0x00193044 File Offset: 0x00191244
		private void OnSetMothershipDataComplete(bool success)
		{
			this.saveScanInProgress = false;
			if (!BuilderScanKiosk.IsSaveSlotValid(this.currentSaveScanIndex))
			{
				this.currentSaveScanIndex = -1;
				this.currentSaveScanData = string.Empty;
				return;
			}
			if (success)
			{
				this.RequestPublishMap(this.serializationConfig.scanSlotMothershipKeys[this.currentSaveScanIndex]);
				return;
			}
			Action<int, string> onSavePrivateScanFailed = this.OnSavePrivateScanFailed;
			if (onSavePrivateScanFailed != null)
			{
				onSavePrivateScanFailed(this.currentSaveScanIndex, "ERROR SAVING");
			}
			this.currentSaveScanIndex = -1;
			this.currentSaveScanData = string.Empty;
		}

		// Token: 0x060050B8 RID: 20664 RVA: 0x001930C6 File Offset: 0x001912C6
		public bool TryGetPrivateScanResponse(int scanSlot, out string scanData)
		{
			if (scanSlot < 0 || scanSlot >= this.privateScanDataCache.Length || !this.hasPulledPrivateScanMothership[scanSlot])
			{
				scanData = string.Empty;
				return false;
			}
			scanData = this.privateScanDataCache[scanSlot];
			return true;
		}

		// Token: 0x060050B9 RID: 20665 RVA: 0x001930F8 File Offset: 0x001912F8
		public void RequestFetchPrivateScan(int slot)
		{
			if (!BuilderScanKiosk.IsSaveSlotValid(slot))
			{
				GTDev.LogError<string>(string.Format("SharedBlocksManager RequestSaveScan: slot {0} OOB", slot), null);
				slot = Mathf.Clamp(slot, 0, BuilderScanKiosk.NUM_SAVE_SLOTS - 1);
			}
			if (this.hasPulledPrivateScanMothership[slot])
			{
				bool arg = this.privateScanDataCache[slot].Length > 0;
				Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
				if (onFetchPrivateScanComplete == null)
				{
					return;
				}
				onFetchPrivateScanComplete(slot, arg);
				return;
			}
			else
			{
				if (this.getScanInProgress)
				{
					Debug.LogError("SharedBlocksManager RequestFetchPrivateScan: request already in progress");
					if (slot != this.currentGetScanIndex)
					{
						Action<int, bool> onFetchPrivateScanComplete2 = this.OnFetchPrivateScanComplete;
						if (onFetchPrivateScanComplete2 == null)
						{
							return;
						}
						onFetchPrivateScanComplete2(slot, false);
					}
					return;
				}
				this.currentGetScanIndex = slot;
				this.getScanInProgress = true;
				try
				{
					if (!MothershipClientApiUnity.GetUserDataValue(this.serializationConfig.scanSlotMothershipKeys[slot], new Action<MothershipUserData>(this.OnGetMothershipPrivateScanSuccess), new Action<MothershipError, int>(this.OnGetMothershipPrivateScanFail), ""))
					{
						Debug.LogError("SharedBlocksManager RequestFetchPrivateScan failed ");
						this.currentGetScanIndex = -1;
						this.getScanInProgress = false;
						Action<int, bool> onFetchPrivateScanComplete3 = this.OnFetchPrivateScanComplete;
						if (onFetchPrivateScanComplete3 != null)
						{
							onFetchPrivateScanComplete3(slot, false);
						}
					}
				}
				catch (Exception ex)
				{
					Debug.LogError("SharedBlocksManager RequestFetchPrivateScan exception " + ex.Message);
					this.currentGetScanIndex = -1;
					this.getScanInProgress = false;
					Action<int, bool> onFetchPrivateScanComplete4 = this.OnFetchPrivateScanComplete;
					if (onFetchPrivateScanComplete4 != null)
					{
						onFetchPrivateScanComplete4(slot, false);
					}
				}
				return;
			}
		}

		// Token: 0x060050BA RID: 20666 RVA: 0x00193248 File Offset: 0x00191448
		private void OnGetMothershipPrivateScanSuccess(MothershipUserData response)
		{
			GTDev.Log<string>("SharedBlocksManager OnGetMothershipPrivateScanSuccess", null);
			bool flag = response != null && response.value != null && response.value.Length > 0;
			int arg = this.currentGetScanIndex;
			if (response != null)
			{
				this.privateScanDataCache[this.currentGetScanIndex] = response.value;
				this.hasPulledPrivateScanMothership[this.currentGetScanIndex] = true;
				if (flag)
				{
					SharedBlocksManager.LocalPublishInfo publishInfoForSlot = SharedBlocksManager.GetPublishInfoForSlot(this.currentGetScanIndex);
					if (publishInfoForSlot.mapID != null)
					{
						SharedBlocksManager.SharedBlocksMap map = new SharedBlocksManager.SharedBlocksMap
						{
							MapID = publishInfoForSlot.mapID,
							MapData = this.privateScanDataCache[this.currentGetScanIndex],
							CreatorNickName = GorillaTagger.Instance.offlineVRRig.playerNameVisible,
							UpdateTime = DateTime.Now
						};
						this.AddMapToResponseCache(map);
					}
					this.currentGetScanIndex = -1;
					this.getScanInProgress = false;
					Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
					if (onFetchPrivateScanComplete != null)
					{
						onFetchPrivateScanComplete(arg, true);
					}
				}
				else
				{
					this.FetchBuildFromPlayfab();
				}
			}
			else
			{
				this.currentGetScanIndex = -1;
				this.getScanInProgress = false;
				Action<int, bool> onFetchPrivateScanComplete2 = this.OnFetchPrivateScanComplete;
				if (onFetchPrivateScanComplete2 != null)
				{
					onFetchPrivateScanComplete2(arg, false);
				}
			}
			if (response != null)
			{
				response.Dispose();
			}
		}

		// Token: 0x060050BB RID: 20667 RVA: 0x00193368 File Offset: 0x00191568
		private void OnGetMothershipPrivateScanFail(MothershipError error, int status)
		{
			string str = (error == null) ? status.ToString() : error.Message;
			GTDev.LogError<string>("SharedBlocksManager OnGetMothershipPrivateScanFail: " + str, null);
			int arg = this.currentGetScanIndex;
			if (BuilderScanKiosk.IsSaveSlotValid(this.currentGetScanIndex))
			{
				this.privateScanDataCache[this.currentGetScanIndex] = string.Empty;
				this.hasPulledPrivateScanMothership[this.currentGetScanIndex] = true;
			}
			this.getScanInProgress = false;
			this.currentGetScanIndex = -1;
			Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
			if (onFetchPrivateScanComplete != null)
			{
				onFetchPrivateScanComplete(arg, false);
			}
			if (error != null)
			{
				error.Dispose();
			}
		}

		// Token: 0x040059F3 RID: 23027
		public static SharedBlocksManager instance;

		// Token: 0x040059FD RID: 23037
		[SerializeField]
		private BuilderTableSerializationConfig serializationConfig;

		// Token: 0x040059FE RID: 23038
		private int maxRetriesOnFail = 3;

		// Token: 0x040059FF RID: 23039
		public const int MAP_ID_LENGTH = 8;

		// Token: 0x04005A00 RID: 23040
		private const string MAP_ID_PATTERN = "^[CFGHKMNPRTWXZ256789]+$";

		// Token: 0x04005A01 RID: 23041
		public const float MINIMUM_REFRESH_DELAY = 60f;

		// Token: 0x04005A02 RID: 23042
		public const int VOTE_HISTORY_LENGTH = 10;

		// Token: 0x04005A03 RID: 23043
		private const int NUM_CACHED_MAP_RESULTS = 5;

		// Token: 0x04005A04 RID: 23044
		private SharedBlocksManager.StartingMapConfig startingMapConfig = new SharedBlocksManager.StartingMapConfig
		{
			pageNumber = 0,
			pageSize = 10,
			sortMethod = SharedBlocksManager.MapSortMethod.Top.ToString(),
			useMapID = false,
			mapID = null
		};

		// Token: 0x04005A05 RID: 23045
		private bool hasQueriedSaveTime;

		// Token: 0x04005A06 RID: 23046
		private static List<string> saveDateKeys = new List<string>(BuilderScanKiosk.NUM_SAVE_SLOTS);

		// Token: 0x04005A07 RID: 23047
		private bool fetchedTableConfig;

		// Token: 0x04005A08 RID: 23048
		private int fetchTableConfigRetryCount;

		// Token: 0x04005A09 RID: 23049
		private string tableConfigResponse;

		// Token: 0x04005A0A RID: 23050
		private bool fetchTitleDataBuildInProgress;

		// Token: 0x04005A0B RID: 23051
		private bool fetchTitleDataBuildComplete;

		// Token: 0x04005A0C RID: 23052
		private int fetchTitleDataRetryCount;

		// Token: 0x04005A0D RID: 23053
		private string titleDataBuildCache = string.Empty;

		// Token: 0x04005A0E RID: 23054
		private bool[] hasPulledPrivateScanPlayfab = new bool[BuilderScanKiosk.NUM_SAVE_SLOTS];

		// Token: 0x04005A0F RID: 23055
		private int fetchPlayfabBuildsRetryCount;

		// Token: 0x04005A10 RID: 23056
		private readonly int publicSlotIndex = BuilderScanKiosk.NUM_SAVE_SLOTS;

		// Token: 0x04005A11 RID: 23057
		private string[] privateScanDataCache = new string[BuilderScanKiosk.NUM_SAVE_SLOTS];

		// Token: 0x04005A12 RID: 23058
		private bool[] hasPulledPrivateScanMothership = new bool[BuilderScanKiosk.NUM_SAVE_SLOTS];

		// Token: 0x04005A13 RID: 23059
		private bool hasPulledDevScan;

		// Token: 0x04005A14 RID: 23060
		private string devScanDataCache;

		// Token: 0x04005A15 RID: 23061
		private bool saveScanInProgress;

		// Token: 0x04005A16 RID: 23062
		private int currentSaveScanIndex = -1;

		// Token: 0x04005A17 RID: 23063
		private string currentSaveScanData = string.Empty;

		// Token: 0x04005A18 RID: 23064
		private bool getScanInProgress;

		// Token: 0x04005A19 RID: 23065
		private int currentGetScanIndex = -1;

		// Token: 0x04005A1A RID: 23066
		private int voteRetryCount;

		// Token: 0x04005A1B RID: 23067
		private bool voteInProgress;

		// Token: 0x04005A1C RID: 23068
		private bool publishRequestInProgress;

		// Token: 0x04005A1D RID: 23069
		private int postPublishMapRetryCount;

		// Token: 0x04005A1E RID: 23070
		private bool getMapDataFromIDInProgress;

		// Token: 0x04005A1F RID: 23071
		private int getMapDataFromIDRetryCount;

		// Token: 0x04005A20 RID: 23072
		private bool getTopMapsInProgress;

		// Token: 0x04005A21 RID: 23073
		private int getTopMapsRetryCount;

		// Token: 0x04005A22 RID: 23074
		private bool hasCachedTopMaps;

		// Token: 0x04005A23 RID: 23075
		private double lastGetTopMapsTime = double.MinValue;

		// Token: 0x04005A24 RID: 23076
		private bool updateMapActiveInProgress;

		// Token: 0x04005A25 RID: 23077
		private int updateMapActiveRetryCount;

		// Token: 0x04005A26 RID: 23078
		private List<SharedBlocksManager.SharedBlocksMap> latestPopularMaps = new List<SharedBlocksManager.SharedBlocksMap>();

		// Token: 0x04005A27 RID: 23079
		private static LinkedList<string> recentUpVotes = new LinkedList<string>();

		// Token: 0x04005A28 RID: 23080
		private static Dictionary<int, SharedBlocksManager.LocalPublishInfo> localPublishData = new Dictionary<int, SharedBlocksManager.LocalPublishInfo>(BuilderScanKiosk.NUM_SAVE_SLOTS);

		// Token: 0x04005A29 RID: 23081
		private static List<string> localMapIds = new List<string>(BuilderScanKiosk.NUM_SAVE_SLOTS);

		// Token: 0x04005A2A RID: 23082
		private List<SharedBlocksManager.SharedBlocksMap> mapResponseCache = new List<SharedBlocksManager.SharedBlocksMap>(5);

		// Token: 0x04005A2B RID: 23083
		private SharedBlocksManager.SharedBlocksMap defaultMap;

		// Token: 0x04005A2C RID: 23084
		private bool hasDefaultMap;

		// Token: 0x04005A2D RID: 23085
		private double defaultMapCacheTime = double.MinValue;

		// Token: 0x04005A2E RID: 23086
		private bool getDefaultMapInProgress;

		// Token: 0x02000CAA RID: 3242
		[Serializable]
		public class SharedBlocksMap
		{
			// Token: 0x1700077A RID: 1914
			// (get) Token: 0x060050BE RID: 20670 RVA: 0x00193525 File Offset: 0x00191725
			// (set) Token: 0x060050BF RID: 20671 RVA: 0x0019352D File Offset: 0x0019172D
			public string MapID { get; set; }

			// Token: 0x1700077B RID: 1915
			// (get) Token: 0x060050C0 RID: 20672 RVA: 0x00193536 File Offset: 0x00191736
			// (set) Token: 0x060050C1 RID: 20673 RVA: 0x0019353E File Offset: 0x0019173E
			public string CreatorID { get; set; }

			// Token: 0x1700077C RID: 1916
			// (get) Token: 0x060050C2 RID: 20674 RVA: 0x00193547 File Offset: 0x00191747
			// (set) Token: 0x060050C3 RID: 20675 RVA: 0x0019354F File Offset: 0x0019174F
			public string CreatorNickName { get; set; }

			// Token: 0x1700077D RID: 1917
			// (get) Token: 0x060050C4 RID: 20676 RVA: 0x00193558 File Offset: 0x00191758
			// (set) Token: 0x060050C5 RID: 20677 RVA: 0x00193560 File Offset: 0x00191760
			public DateTime CreateTime { get; set; }

			// Token: 0x1700077E RID: 1918
			// (get) Token: 0x060050C6 RID: 20678 RVA: 0x00193569 File Offset: 0x00191769
			// (set) Token: 0x060050C7 RID: 20679 RVA: 0x00193571 File Offset: 0x00191771
			public DateTime UpdateTime { get; set; }

			// Token: 0x1700077F RID: 1919
			// (get) Token: 0x060050C8 RID: 20680 RVA: 0x0019357A File Offset: 0x0019177A
			// (set) Token: 0x060050C9 RID: 20681 RVA: 0x00193582 File Offset: 0x00191782
			public string MapData { get; set; }
		}

		// Token: 0x02000CAB RID: 3243
		[Serializable]
		public struct LocalPublishInfo
		{
			// Token: 0x04005A35 RID: 23093
			public string mapID;

			// Token: 0x04005A36 RID: 23094
			public long publishTime;
		}

		// Token: 0x02000CAC RID: 3244
		[Serializable]
		private class VoteRequest
		{
			// Token: 0x04005A37 RID: 23095
			public string mothershipId;

			// Token: 0x04005A38 RID: 23096
			public string mothershipToken;

			// Token: 0x04005A39 RID: 23097
			public string mapId;

			// Token: 0x04005A3A RID: 23098
			public int vote;
		}

		// Token: 0x02000CAD RID: 3245
		[Serializable]
		private class PublishMapRequestData
		{
			// Token: 0x04005A3B RID: 23099
			public string mothershipId;

			// Token: 0x04005A3C RID: 23100
			public string mothershipToken;

			// Token: 0x04005A3D RID: 23101
			public string userdataMetadataKey;

			// Token: 0x04005A3E RID: 23102
			public string playerNickname;
		}

		// Token: 0x02000CAE RID: 3246
		public enum MapSortMethod
		{
			// Token: 0x04005A40 RID: 23104
			Top,
			// Token: 0x04005A41 RID: 23105
			NewlyCreated,
			// Token: 0x04005A42 RID: 23106
			RecentlyUpdated
		}

		// Token: 0x02000CAF RID: 3247
		public struct StartingMapConfig
		{
			// Token: 0x04005A43 RID: 23107
			public int pageNumber;

			// Token: 0x04005A44 RID: 23108
			public int pageSize;

			// Token: 0x04005A45 RID: 23109
			public string sortMethod;

			// Token: 0x04005A46 RID: 23110
			public bool useMapID;

			// Token: 0x04005A47 RID: 23111
			public string mapID;
		}

		// Token: 0x02000CB0 RID: 3248
		[Serializable]
		private class GetMapsRequest
		{
			// Token: 0x04005A48 RID: 23112
			public string mothershipId;

			// Token: 0x04005A49 RID: 23113
			public string mothershipToken;

			// Token: 0x04005A4A RID: 23114
			public int page;

			// Token: 0x04005A4B RID: 23115
			public int pageSize;

			// Token: 0x04005A4C RID: 23116
			public string sort;

			// Token: 0x04005A4D RID: 23117
			public bool ShowInactive;
		}

		// Token: 0x02000CB1 RID: 3249
		[Serializable]
		private class GetMapDataFromIDRequest
		{
			// Token: 0x04005A4E RID: 23118
			public string mothershipId;

			// Token: 0x04005A4F RID: 23119
			public string mothershipToken;

			// Token: 0x04005A50 RID: 23120
			public string mapId;
		}

		// Token: 0x02000CB2 RID: 3250
		[Serializable]
		private class GetMapIDFromPlayerRequest
		{
			// Token: 0x04005A51 RID: 23121
			public string mothershipId;

			// Token: 0x04005A52 RID: 23122
			public string mothershipToken;

			// Token: 0x04005A53 RID: 23123
			public string requestId;

			// Token: 0x04005A54 RID: 23124
			public string requestUserDataMetaKey;
		}

		// Token: 0x02000CB3 RID: 3251
		[Serializable]
		private class GetMapIDFromPlayerResponse
		{
			// Token: 0x04005A55 RID: 23125
			public SharedBlocksManager.SharedBlocksMapMetaData result;

			// Token: 0x04005A56 RID: 23126
			public int statusCode;

			// Token: 0x04005A57 RID: 23127
			public string error;
		}

		// Token: 0x02000CB4 RID: 3252
		[Serializable]
		private class SharedBlocksMapMetaData
		{
			// Token: 0x04005A58 RID: 23128
			public string mapId;

			// Token: 0x04005A59 RID: 23129
			public string mothershipId;

			// Token: 0x04005A5A RID: 23130
			public string userDataMetadataKey;

			// Token: 0x04005A5B RID: 23131
			public string nickname;

			// Token: 0x04005A5C RID: 23132
			public string createdTime;

			// Token: 0x04005A5D RID: 23133
			public string updatedTime;

			// Token: 0x04005A5E RID: 23134
			public int voteCount;

			// Token: 0x04005A5F RID: 23135
			public bool isActive;
		}

		// Token: 0x02000CB5 RID: 3253
		[Serializable]
		private struct GetMapDataFromPlayerRequestData
		{
			// Token: 0x04005A60 RID: 23136
			public string CreatorID;

			// Token: 0x04005A61 RID: 23137
			public string MapScan;

			// Token: 0x04005A62 RID: 23138
			public SharedBlocksManager.BlocksMapRequestCallback Callback;
		}

		// Token: 0x02000CB6 RID: 3254
		[Serializable]
		private class UpdateMapActiveRequest
		{
			// Token: 0x04005A63 RID: 23139
			public string mothershipId;

			// Token: 0x04005A64 RID: 23140
			public string mothershipToken;

			// Token: 0x04005A65 RID: 23141
			public string userdataMetadataKey;

			// Token: 0x04005A66 RID: 23142
			public bool setActive;
		}

		// Token: 0x02000CB7 RID: 3255
		// (Invoke) Token: 0x060050D4 RID: 20692
		public delegate void PublishMapRequestCallback(bool success, string key, string mapID, long responseCode);

		// Token: 0x02000CB8 RID: 3256
		// (Invoke) Token: 0x060050D8 RID: 20696
		public delegate void BlocksMapRequestCallback(SharedBlocksManager.SharedBlocksMap response);
	}
}
