using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Oculus.Platform;
using Oculus.Platform.Models;
using UnityEngine;
using UnityEngine.Networking;

namespace GorillaNetworking
{
	// Token: 0x02000DB6 RID: 3510
	public class TitleVotingExample : MonoBehaviour
	{
		// Token: 0x06005726 RID: 22310 RVA: 0x001B0904 File Offset: 0x001AEB04
		public void Start()
		{
			TitleVotingExample.<Start>d__8 <Start>d__;
			<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Start>d__.<>4__this = this;
			<Start>d__.<>1__state = -1;
			<Start>d__.<>t__builder.Start<TitleVotingExample.<Start>d__8>(ref <Start>d__);
		}

		// Token: 0x06005727 RID: 22311 RVA: 0x000023F5 File Offset: 0x000005F5
		public void Update()
		{
		}

		// Token: 0x06005728 RID: 22312 RVA: 0x001B093C File Offset: 0x001AEB3C
		private Task WaitForSessionToken()
		{
			TitleVotingExample.<WaitForSessionToken>d__10 <WaitForSessionToken>d__;
			<WaitForSessionToken>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitForSessionToken>d__.<>1__state = -1;
			<WaitForSessionToken>d__.<>t__builder.Start<TitleVotingExample.<WaitForSessionToken>d__10>(ref <WaitForSessionToken>d__);
			return <WaitForSessionToken>d__.<>t__builder.Task;
		}

		// Token: 0x06005729 RID: 22313 RVA: 0x001B0978 File Offset: 0x001AEB78
		public void FetchPollsAndVote()
		{
			base.StartCoroutine(this.DoFetchPolls(new TitleVotingExample.FetchPollsRequest
			{
				TitleId = PlayFabAuthenticatorSettings.TitleId,
				PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
				PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
				IncludeInactive = this.includeInactive
			}, new Action<List<TitleVotingExample.FetchPollsResponse>>(this.OnFetchPollsResponse)));
		}

		// Token: 0x0600572A RID: 22314 RVA: 0x001B09E0 File Offset: 0x001AEBE0
		private void GetNonceForVotingCallback([CanBeNull] Message<UserProof> message)
		{
			if (message != null)
			{
				UserProof data = message.Data;
				this.Nonce = ((data != null) ? data.ToString() : null);
			}
			base.StartCoroutine(this.DoVote(new TitleVotingExample.VoteRequest
			{
				PollId = this.PollId,
				TitleId = PlayFabAuthenticatorSettings.TitleId,
				PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
				OculusId = PlayFabAuthenticator.instance.userID,
				UserPlatform = PlayFabAuthenticator.instance.platform.ToString(),
				UserNonce = this.Nonce,
				PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
				OptionIndex = this.Option,
				IsPrediction = this.isPrediction
			}, new Action<TitleVotingExample.VoteResponse>(this.OnVoteSuccess)));
		}

		// Token: 0x0600572B RID: 22315 RVA: 0x001B0AAE File Offset: 0x001AECAE
		public void Vote()
		{
			this.GetNonceForVotingCallback(null);
		}

		// Token: 0x0600572C RID: 22316 RVA: 0x001B0AB7 File Offset: 0x001AECB7
		private IEnumerator DoFetchPolls(TitleVotingExample.FetchPollsRequest data, Action<List<TitleVotingExample.FetchPollsResponse>> callback)
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
				List<TitleVotingExample.FetchPollsResponse> obj = JsonConvert.DeserializeObject<List<TitleVotingExample.FetchPollsResponse>>(request.downloadHandler.text);
				callback(obj);
			}
			else
			{
				Debug.LogError(string.Format("FetchPolls Error: {0} -- raw response: ", request.responseCode) + request.downloadHandler.text);
				long responseCode = request.responseCode;
				if (responseCode >= 500L && responseCode < 600L)
				{
					retry = true;
					Debug.LogError(string.Format("HTTP {0} error: {1}", request.responseCode, request.error));
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
					Debug.LogWarning(string.Format("Retrying Title Voting FetchPolls... Retry attempt #{0}, waiting for {1} seconds", this.fetchPollsRetryCount + 1, num));
					this.fetchPollsRetryCount++;
					yield return new WaitForSeconds((float)num);
					this.FetchPollsAndVote();
				}
				else
				{
					Debug.LogError("Maximum FetchPolls retries attempted. Please check your network connection.");
					this.fetchPollsRetryCount = 0;
					callback(null);
				}
			}
			yield break;
		}

		// Token: 0x0600572D RID: 22317 RVA: 0x001B0AD4 File Offset: 0x001AECD4
		private IEnumerator DoVote(TitleVotingExample.VoteRequest data, Action<TitleVotingExample.VoteResponse> callback)
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
				TitleVotingExample.VoteResponse obj = JsonConvert.DeserializeObject<TitleVotingExample.VoteResponse>(request.downloadHandler.text);
				callback(obj);
			}
			else
			{
				Debug.LogError(string.Format("Vote Error: {0} -- raw response: ", request.responseCode) + request.downloadHandler.text);
				long responseCode = request.responseCode;
				if (responseCode >= 500L && responseCode < 600L)
				{
					retry = true;
					Debug.LogError(string.Format("HTTP {0} error: {1}", request.responseCode, request.error));
				}
				else if (request.responseCode == 409L)
				{
					Debug.LogWarning("User already voted on this poll!");
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
					Debug.LogWarning(string.Format("Retrying Voting... Retry attempt #{0}, waiting for {1} seconds", this.voteRetryCount + 1, num));
					this.voteRetryCount++;
					yield return new WaitForSeconds((float)num);
					this.Vote();
				}
				else
				{
					Debug.LogError("Maximum Vote retries attempted. Please check your network connection.");
					this.voteRetryCount = 0;
					callback(null);
				}
			}
			yield break;
		}

		// Token: 0x0600572E RID: 22318 RVA: 0x001B0AF1 File Offset: 0x001AECF1
		private void OnFetchPollsResponse([CanBeNull] List<TitleVotingExample.FetchPollsResponse> response)
		{
			if (response != null)
			{
				Debug.Log("Got polls: " + JsonConvert.SerializeObject(response));
				this.Vote();
				return;
			}
			Debug.LogError("Error: Could not fetch polls!");
		}

		// Token: 0x0600572F RID: 22319 RVA: 0x001B0B1C File Offset: 0x001AED1C
		private void OnVoteSuccess([CanBeNull] TitleVotingExample.VoteResponse response)
		{
			if (response != null)
			{
				Debug.Log("Voted! " + JsonConvert.SerializeObject(response));
				return;
			}
			Debug.LogError("Error: Could not vote!");
		}

		// Token: 0x040060F6 RID: 24822
		private string Nonce = "";

		// Token: 0x040060F7 RID: 24823
		private int PollId = 5;

		// Token: 0x040060F8 RID: 24824
		private bool includeInactive = true;

		// Token: 0x040060F9 RID: 24825
		private int Option;

		// Token: 0x040060FA RID: 24826
		private bool isPrediction;

		// Token: 0x040060FB RID: 24827
		private int fetchPollsRetryCount;

		// Token: 0x040060FC RID: 24828
		private int voteRetryCount;

		// Token: 0x040060FD RID: 24829
		private int maxRetriesOnFail = 3;

		// Token: 0x02000DB7 RID: 3511
		[Serializable]
		private class FetchPollsRequest
		{
			// Token: 0x040060FE RID: 24830
			public string TitleId;

			// Token: 0x040060FF RID: 24831
			public string PlayFabId;

			// Token: 0x04006100 RID: 24832
			public string PlayFabTicket;

			// Token: 0x04006101 RID: 24833
			public bool IncludeInactive;
		}

		// Token: 0x02000DB8 RID: 3512
		[Serializable]
		private class FetchPollsResponse
		{
			// Token: 0x04006102 RID: 24834
			public int PollId;

			// Token: 0x04006103 RID: 24835
			public string Question;

			// Token: 0x04006104 RID: 24836
			public List<string> VoteOptions;

			// Token: 0x04006105 RID: 24837
			public List<int> VoteCount;

			// Token: 0x04006106 RID: 24838
			public List<int> PredictionCount;

			// Token: 0x04006107 RID: 24839
			public DateTime StartTime;

			// Token: 0x04006108 RID: 24840
			public DateTime EndTime;
		}

		// Token: 0x02000DB9 RID: 3513
		[Serializable]
		private class VoteRequest
		{
			// Token: 0x04006109 RID: 24841
			public int PollId;

			// Token: 0x0400610A RID: 24842
			public string TitleId;

			// Token: 0x0400610B RID: 24843
			public string PlayFabId;

			// Token: 0x0400610C RID: 24844
			public string OculusId;

			// Token: 0x0400610D RID: 24845
			public string UserNonce;

			// Token: 0x0400610E RID: 24846
			public string UserPlatform;

			// Token: 0x0400610F RID: 24847
			public int OptionIndex;

			// Token: 0x04006110 RID: 24848
			public bool IsPrediction;

			// Token: 0x04006111 RID: 24849
			public string PlayFabTicket;
		}

		// Token: 0x02000DBA RID: 3514
		[Serializable]
		private class VoteResponse
		{
			// Token: 0x1700085D RID: 2141
			// (get) Token: 0x06005734 RID: 22324 RVA: 0x001B0B69 File Offset: 0x001AED69
			// (set) Token: 0x06005735 RID: 22325 RVA: 0x001B0B71 File Offset: 0x001AED71
			public int PollId { get; set; }

			// Token: 0x1700085E RID: 2142
			// (get) Token: 0x06005736 RID: 22326 RVA: 0x001B0B7A File Offset: 0x001AED7A
			// (set) Token: 0x06005737 RID: 22327 RVA: 0x001B0B82 File Offset: 0x001AED82
			public string TitleId { get; set; }

			// Token: 0x1700085F RID: 2143
			// (get) Token: 0x06005738 RID: 22328 RVA: 0x001B0B8B File Offset: 0x001AED8B
			// (set) Token: 0x06005739 RID: 22329 RVA: 0x001B0B93 File Offset: 0x001AED93
			public List<string> VoteOptions { get; set; }

			// Token: 0x17000860 RID: 2144
			// (get) Token: 0x0600573A RID: 22330 RVA: 0x001B0B9C File Offset: 0x001AED9C
			// (set) Token: 0x0600573B RID: 22331 RVA: 0x001B0BA4 File Offset: 0x001AEDA4
			public List<int> VoteCount { get; set; }

			// Token: 0x17000861 RID: 2145
			// (get) Token: 0x0600573C RID: 22332 RVA: 0x001B0BAD File Offset: 0x001AEDAD
			// (set) Token: 0x0600573D RID: 22333 RVA: 0x001B0BB5 File Offset: 0x001AEDB5
			public List<int> PredictionCount { get; set; }
		}
	}
}
