using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GorillaNetworking;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Oculus.Platform;
using Oculus.Platform.Models;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x0200011E RID: 286
public class MonkeVoteController : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x170000B2 RID: 178
	// (get) Token: 0x0600075A RID: 1882 RVA: 0x0002A0C2 File Offset: 0x000282C2
	// (set) Token: 0x0600075B RID: 1883 RVA: 0x0002A0C9 File Offset: 0x000282C9
	public static MonkeVoteController instance { get; private set; }

	// Token: 0x1400000D RID: 13
	// (add) Token: 0x0600075C RID: 1884 RVA: 0x0002A0D4 File Offset: 0x000282D4
	// (remove) Token: 0x0600075D RID: 1885 RVA: 0x0002A10C File Offset: 0x0002830C
	public event Action OnPollsUpdated;

	// Token: 0x1400000E RID: 14
	// (add) Token: 0x0600075E RID: 1886 RVA: 0x0002A144 File Offset: 0x00028344
	// (remove) Token: 0x0600075F RID: 1887 RVA: 0x0002A17C File Offset: 0x0002837C
	public event Action OnVoteAccepted;

	// Token: 0x1400000F RID: 15
	// (add) Token: 0x06000760 RID: 1888 RVA: 0x0002A1B4 File Offset: 0x000283B4
	// (remove) Token: 0x06000761 RID: 1889 RVA: 0x0002A1EC File Offset: 0x000283EC
	public event Action OnVoteFailed;

	// Token: 0x14000010 RID: 16
	// (add) Token: 0x06000762 RID: 1890 RVA: 0x0002A224 File Offset: 0x00028424
	// (remove) Token: 0x06000763 RID: 1891 RVA: 0x0002A25C File Offset: 0x0002845C
	public event Action OnCurrentPollEnded;

	// Token: 0x06000764 RID: 1892 RVA: 0x0002A291 File Offset: 0x00028491
	public void Awake()
	{
		if (MonkeVoteController.instance == null)
		{
			MonkeVoteController.instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06000765 RID: 1893 RVA: 0x0002A2B0 File Offset: 0x000284B0
	public void SliceUpdate()
	{
		if (this.isCurrentPollActive && !this.hasCurrentPollCompleted && this.currentPollCompletionTime < DateTime.UtcNow)
		{
			GTDev.Log<string>("Active vote poll completed.", null);
			this.hasCurrentPollCompleted = true;
			Action onCurrentPollEnded = this.OnCurrentPollEnded;
			if (onCurrentPollEnded == null)
			{
				return;
			}
			onCurrentPollEnded();
		}
	}

	// Token: 0x06000766 RID: 1894 RVA: 0x000172AD File Offset: 0x000154AD
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000767 RID: 1895 RVA: 0x000172B6 File Offset: 0x000154B6
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000768 RID: 1896 RVA: 0x0002A304 File Offset: 0x00028504
	public void RequestPolls()
	{
		MonkeVoteController.<RequestPolls>d__34 <RequestPolls>d__;
		<RequestPolls>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RequestPolls>d__.<>4__this = this;
		<RequestPolls>d__.<>1__state = -1;
		<RequestPolls>d__.<>t__builder.Start<MonkeVoteController.<RequestPolls>d__34>(ref <RequestPolls>d__);
	}

	// Token: 0x06000769 RID: 1897 RVA: 0x0002A33C File Offset: 0x0002853C
	private Task WaitForSessionToken()
	{
		MonkeVoteController.<WaitForSessionToken>d__35 <WaitForSessionToken>d__;
		<WaitForSessionToken>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForSessionToken>d__.<>1__state = -1;
		<WaitForSessionToken>d__.<>t__builder.Start<MonkeVoteController.<WaitForSessionToken>d__35>(ref <WaitForSessionToken>d__);
		return <WaitForSessionToken>d__.<>t__builder.Task;
	}

	// Token: 0x0600076A RID: 1898 RVA: 0x0002A378 File Offset: 0x00028578
	private void FetchPolls()
	{
		base.StartCoroutine(this.DoFetchPolls(new MonkeVoteController.FetchPollsRequest
		{
			TitleId = PlayFabAuthenticatorSettings.TitleId,
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			IncludeInactive = this.includeInactive
		}, new Action<List<MonkeVoteController.FetchPollsResponse>>(this.OnFetchPollsResponse)));
	}

	// Token: 0x0600076B RID: 1899 RVA: 0x0002A3DE File Offset: 0x000285DE
	private IEnumerator DoFetchPolls(MonkeVoteController.FetchPollsRequest data, Action<List<MonkeVoteController.FetchPollsResponse>> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.VotingApiBaseUrl + "/api/FetchPoll", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			List<MonkeVoteController.FetchPollsResponse> obj = JsonConvert.DeserializeObject<List<MonkeVoteController.FetchPollsResponse>>(request.downloadHandler.text);
			callback(obj);
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				retry = true;
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				retry = true;
			}
		}
		if (retry)
		{
			if (this.fetchPollsRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.fetchPollsRetryCount + 1));
				this.fetchPollsRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.FetchPolls();
			}
			else
			{
				GTDev.LogError<string>("Maximum FetchPolls retries attempted. Please check your network connection.", null);
				this.fetchPollsRetryCount = 0;
				callback(null);
			}
		}
		yield break;
	}

	// Token: 0x0600076C RID: 1900 RVA: 0x0002A3FC File Offset: 0x000285FC
	private void OnFetchPollsResponse([CanBeNull] List<MonkeVoteController.FetchPollsResponse> response)
	{
		this.isFetchingPoll = false;
		this.hasPoll = false;
		this.lastPollData = null;
		this.currentPollData = null;
		this.isCurrentPollActive = false;
		this.hasCurrentPollCompleted = false;
		if (response != null)
		{
			DateTime minValue = DateTime.MinValue;
			using (List<MonkeVoteController.FetchPollsResponse>.Enumerator enumerator = response.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MonkeVoteController.FetchPollsResponse fetchPollsResponse = enumerator.Current;
					if (fetchPollsResponse.isActive)
					{
						this.hasPoll = true;
						this.currentPollData = fetchPollsResponse;
						if (this.currentPollData.EndTime > DateTime.UtcNow)
						{
							this.isCurrentPollActive = true;
							this.hasCurrentPollCompleted = false;
							this.currentPollCompletionTime = this.currentPollData.EndTime;
							this.currentPollCompletionTime = this.currentPollCompletionTime.AddMinutes(1.0);
						}
					}
					if (!fetchPollsResponse.isActive && fetchPollsResponse.EndTime > minValue && fetchPollsResponse.EndTime < DateTime.UtcNow)
					{
						this.lastPollData = fetchPollsResponse;
					}
				}
				goto IL_106;
			}
		}
		GTDev.LogError<string>("Error: Could not fetch polls!", null);
		IL_106:
		Action onPollsUpdated = this.OnPollsUpdated;
		if (onPollsUpdated == null)
		{
			return;
		}
		onPollsUpdated();
	}

	// Token: 0x0600076D RID: 1901 RVA: 0x0002A530 File Offset: 0x00028730
	public void Vote(int pollId, int option, bool isPrediction)
	{
		if (!this.hasPoll)
		{
			return;
		}
		if (this.isSendingVote)
		{
			return;
		}
		this.isSendingVote = true;
		this.pollId = pollId;
		this.option = option;
		this.isPrediction = isPrediction;
		this.SendVote();
	}

	// Token: 0x0600076E RID: 1902 RVA: 0x0002A566 File Offset: 0x00028766
	private void SendVote()
	{
		this.GetNonceForVotingCallback(null);
	}

	// Token: 0x0600076F RID: 1903 RVA: 0x0002A570 File Offset: 0x00028770
	private void GetNonceForVotingCallback([CanBeNull] Message<UserProof> message)
	{
		if (message != null)
		{
			UserProof data = message.Data;
			this.Nonce = ((data != null) ? data.Value : null);
		}
		base.StartCoroutine(this.DoVote(new MonkeVoteController.VoteRequest
		{
			PollId = this.pollId,
			TitleId = PlayFabAuthenticatorSettings.TitleId,
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			OculusId = PlayFabAuthenticator.instance.userID,
			UserPlatform = PlayFabAuthenticator.instance.platform.ToString(),
			UserNonce = this.Nonce,
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			OptionIndex = this.option,
			IsPrediction = this.isPrediction
		}, new Action<MonkeVoteController.VoteResponse>(this.OnVoteSuccess)));
	}

	// Token: 0x06000770 RID: 1904 RVA: 0x0002A63E File Offset: 0x0002883E
	private IEnumerator DoVote(MonkeVoteController.VoteRequest data, Action<MonkeVoteController.VoteResponse> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.VotingApiBaseUrl + "/api/Vote", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			MonkeVoteController.VoteResponse obj = JsonConvert.DeserializeObject<MonkeVoteController.VoteResponse>(request.downloadHandler.text);
			callback(obj);
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				retry = true;
			}
			else if (request.responseCode == 429L)
			{
				GTDev.LogWarning<string>("User already voted on this poll!", null);
				callback(null);
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				retry = true;
			}
		}
		if (retry)
		{
			if (this.voteRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.voteRetryCount + 1));
				this.voteRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.SendVote();
			}
			else
			{
				GTDev.LogError<string>("Maximum Vote retries attempted. Please check your network connection.", null);
				this.voteRetryCount = 0;
				callback(null);
			}
		}
		else
		{
			this.isSendingVote = false;
		}
		yield break;
	}

	// Token: 0x06000771 RID: 1905 RVA: 0x0002A65B File Offset: 0x0002885B
	private void OnVoteSuccess([CanBeNull] MonkeVoteController.VoteResponse response)
	{
		this.isSendingVote = false;
		if (response != null)
		{
			this.lastVoteData = response;
			Action onVoteAccepted = this.OnVoteAccepted;
			if (onVoteAccepted == null)
			{
				return;
			}
			onVoteAccepted();
			return;
		}
		else
		{
			Action onVoteFailed = this.OnVoteFailed;
			if (onVoteFailed == null)
			{
				return;
			}
			onVoteFailed();
			return;
		}
	}

	// Token: 0x06000772 RID: 1906 RVA: 0x0002A68F File Offset: 0x0002888F
	public MonkeVoteController.FetchPollsResponse GetLastPollData()
	{
		return this.lastPollData;
	}

	// Token: 0x06000773 RID: 1907 RVA: 0x0002A697 File Offset: 0x00028897
	public MonkeVoteController.FetchPollsResponse GetCurrentPollData()
	{
		return this.currentPollData;
	}

	// Token: 0x06000774 RID: 1908 RVA: 0x0002A69F File Offset: 0x0002889F
	public MonkeVoteController.VoteResponse GetVoteData()
	{
		return this.lastVoteData;
	}

	// Token: 0x06000775 RID: 1909 RVA: 0x0002A6A7 File Offset: 0x000288A7
	public int GetLastVotePollId()
	{
		return this.pollId;
	}

	// Token: 0x06000776 RID: 1910 RVA: 0x0002A6AF File Offset: 0x000288AF
	public int GetLastVoteSelectedOption()
	{
		return this.option;
	}

	// Token: 0x06000777 RID: 1911 RVA: 0x0002A6B7 File Offset: 0x000288B7
	public bool GetLastVoteWasPrediction()
	{
		return this.isPrediction;
	}

	// Token: 0x06000778 RID: 1912 RVA: 0x0002A6BF File Offset: 0x000288BF
	public DateTime GetCurrentPollCompletionTime()
	{
		return this.currentPollCompletionTime;
	}

	// Token: 0x040008EC RID: 2284
	private string Nonce = "";

	// Token: 0x040008ED RID: 2285
	private bool includeInactive = true;

	// Token: 0x040008EE RID: 2286
	private int fetchPollsRetryCount;

	// Token: 0x040008EF RID: 2287
	private int maxRetriesOnFail = 3;

	// Token: 0x040008F0 RID: 2288
	private int voteRetryCount;

	// Token: 0x040008F5 RID: 2293
	private MonkeVoteController.FetchPollsResponse lastPollData;

	// Token: 0x040008F6 RID: 2294
	private MonkeVoteController.FetchPollsResponse currentPollData;

	// Token: 0x040008F7 RID: 2295
	private MonkeVoteController.VoteResponse lastVoteData;

	// Token: 0x040008F8 RID: 2296
	private bool isFetchingPoll;

	// Token: 0x040008F9 RID: 2297
	private bool hasPoll;

	// Token: 0x040008FA RID: 2298
	private bool isCurrentPollActive;

	// Token: 0x040008FB RID: 2299
	private bool hasCurrentPollCompleted;

	// Token: 0x040008FC RID: 2300
	private DateTime currentPollCompletionTime;

	// Token: 0x040008FD RID: 2301
	private bool isSendingVote;

	// Token: 0x040008FE RID: 2302
	private int pollId = -1;

	// Token: 0x040008FF RID: 2303
	private int option;

	// Token: 0x04000900 RID: 2304
	private bool isPrediction;

	// Token: 0x0200011F RID: 287
	[Serializable]
	private class FetchPollsRequest
	{
		// Token: 0x04000901 RID: 2305
		public string TitleId;

		// Token: 0x04000902 RID: 2306
		public string PlayFabId;

		// Token: 0x04000903 RID: 2307
		public string PlayFabTicket;

		// Token: 0x04000904 RID: 2308
		public bool IncludeInactive;
	}

	// Token: 0x02000120 RID: 288
	[Serializable]
	public class FetchPollsResponse
	{
		// Token: 0x04000905 RID: 2309
		public int PollId;

		// Token: 0x04000906 RID: 2310
		public string Question;

		// Token: 0x04000907 RID: 2311
		public List<string> VoteOptions;

		// Token: 0x04000908 RID: 2312
		public List<int> VoteCount;

		// Token: 0x04000909 RID: 2313
		public List<int> PredictionCount;

		// Token: 0x0400090A RID: 2314
		public DateTime StartTime;

		// Token: 0x0400090B RID: 2315
		public DateTime EndTime;

		// Token: 0x0400090C RID: 2316
		public bool isActive;
	}

	// Token: 0x02000121 RID: 289
	[Serializable]
	private class VoteRequest
	{
		// Token: 0x0400090D RID: 2317
		public int PollId;

		// Token: 0x0400090E RID: 2318
		public string TitleId;

		// Token: 0x0400090F RID: 2319
		public string PlayFabId;

		// Token: 0x04000910 RID: 2320
		public string OculusId;

		// Token: 0x04000911 RID: 2321
		public string UserNonce;

		// Token: 0x04000912 RID: 2322
		public string UserPlatform;

		// Token: 0x04000913 RID: 2323
		public int OptionIndex;

		// Token: 0x04000914 RID: 2324
		public bool IsPrediction;

		// Token: 0x04000915 RID: 2325
		public string PlayFabTicket;
	}

	// Token: 0x02000122 RID: 290
	[Serializable]
	public class VoteResponse
	{
		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x0600077D RID: 1917 RVA: 0x0002A6EF File Offset: 0x000288EF
		// (set) Token: 0x0600077E RID: 1918 RVA: 0x0002A6F7 File Offset: 0x000288F7
		public int PollId { get; set; }

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x0600077F RID: 1919 RVA: 0x0002A700 File Offset: 0x00028900
		// (set) Token: 0x06000780 RID: 1920 RVA: 0x0002A708 File Offset: 0x00028908
		public string TitleId { get; set; }

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x06000781 RID: 1921 RVA: 0x0002A711 File Offset: 0x00028911
		// (set) Token: 0x06000782 RID: 1922 RVA: 0x0002A719 File Offset: 0x00028919
		public List<string> VoteOptions { get; set; }

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x06000783 RID: 1923 RVA: 0x0002A722 File Offset: 0x00028922
		// (set) Token: 0x06000784 RID: 1924 RVA: 0x0002A72A File Offset: 0x0002892A
		public List<int> VoteCount { get; set; }

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000785 RID: 1925 RVA: 0x0002A733 File Offset: 0x00028933
		// (set) Token: 0x06000786 RID: 1926 RVA: 0x0002A73B File Offset: 0x0002893B
		public List<int> PredictionCount { get; set; }
	}
}
