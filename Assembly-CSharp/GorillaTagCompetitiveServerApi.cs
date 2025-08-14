using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GorillaNetworking;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x0200070A RID: 1802
public class GorillaTagCompetitiveServerApi : MonoBehaviour
{
	// Token: 0x06002D2E RID: 11566 RVA: 0x000EE06B File Offset: 0x000EC26B
	private void Awake()
	{
		if (GorillaTagCompetitiveServerApi.Instance)
		{
			GTDev.LogError<string>("Duplicate GorillaTagCompetitiveServerApi detected. Destroying self.", base.gameObject, null);
			Object.Destroy(this);
			return;
		}
		GorillaTagCompetitiveServerApi.Instance = this;
	}

	// Token: 0x06002D2F RID: 11567 RVA: 0x000EE098 File Offset: 0x000EC298
	public void RequestGetRankInformation(List<string> playfabs, Action<GorillaTagCompetitiveServerApi.RankedModeProgressionData> callback)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestGetRankInformation Client Not Logged into Mothership", null);
			return;
		}
		if (this.GetRankInformationInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestGetRankInformation already in progress", null);
			return;
		}
		this.GetRankInformationInProgress = true;
		string platform = "PC";
		base.StartCoroutine(this.GetRankInformation(new GorillaTagCompetitiveServerApi.RankedModeProgressionRequestData
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			platform = platform,
			playfabIds = playfabs
		}, callback));
	}

	// Token: 0x06002D30 RID: 11568 RVA: 0x000EE116 File Offset: 0x000EC316
	private IEnumerator GetRankInformation(GorillaTagCompetitiveServerApi.RankedModeProgressionRequestData data, Action<GorillaTagCompetitiveServerApi.RankedModeProgressionData> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.MmrApiBaseUrl + "/api/GetTier", "GET");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			GTDev.Log<string>("GetRankInformation Success: raw response: " + request.downloadHandler.text, null);
			this.OnCompleteGetRankInformation(request.downloadHandler.text, callback);
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
			else if (request.responseCode != 400L || !(request.downloadHandler.text == "Token validation failed: Client Authentication Failed"))
			{
				this.OnCompleteGetRankInformation(null, callback);
			}
		}
		if (retry)
		{
			if (this.GetRankInformationRetryCount < this.MAX_SERVER_RETRIES)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.GetRankInformationRetryCount + 1));
				this.GetRankInformationRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.GetRankInformationInProgress = false;
				this.RequestGetRankInformation(data.playfabIds, callback);
			}
			else
			{
				this.GetRankInformationRetryCount = 0;
				this.OnCompleteGetRankInformation(null, callback);
			}
		}
		yield break;
	}

	// Token: 0x06002D31 RID: 11569 RVA: 0x000EE134 File Offset: 0x000EC334
	private void OnCompleteGetRankInformation([CanBeNull] string response, Action<GorillaTagCompetitiveServerApi.RankedModeProgressionData> callback)
	{
		this.GetRankInformationInProgress = false;
		this.GetRankInformationRetryCount = 0;
		if (response.IsNullOrEmpty())
		{
			return;
		}
		string text = "{ \"playerData\": " + response + " }";
		GorillaTagCompetitiveServerApi.RankedModeProgressionData obj;
		try
		{
			obj = JsonUtility.FromJson<GorillaTagCompetitiveServerApi.RankedModeProgressionData>(text);
		}
		catch (ArgumentException exception)
		{
			Debug.LogException(exception);
			Debug.LogError("[GT/GorillaTagCompetitiveServerApi]  ERROR!!!  OnCompleteGetRankInformation: Encountered ArgumentException above while trying to parse json string:\n" + text);
			return;
		}
		catch (Exception exception2)
		{
			Debug.LogException(exception2);
			Debug.LogError("[GT/GorillaTagCompetitiveServerApi]  ERROR!!!  OnCompleteGetRankInformation: Encountered exception above while trying to parse json string:\n" + text);
			return;
		}
		if (callback != null)
		{
			callback(obj);
		}
	}

	// Token: 0x06002D32 RID: 11570 RVA: 0x000EE1C8 File Offset: 0x000EC3C8
	public void RequestCreateMatchId(Action<string> callback)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestCreateMatchId Client Not Logged into Mothership", null);
			return;
		}
		if (this.CreateMatchIdInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestCreateMatchId already in progress", null);
			return;
		}
		string platform = "PC";
		this.CreateMatchIdInProgress = true;
		base.StartCoroutine(this.CreateMatchId(new GorillaTagCompetitiveServerApi.RankedModeRequestDataPlatformed
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			platform = platform
		}, callback));
	}

	// Token: 0x06002D33 RID: 11571 RVA: 0x000EE23F File Offset: 0x000EC43F
	private IEnumerator CreateMatchId(GorillaTagCompetitiveServerApi.RankedModeRequestDataPlatformed data, Action<string> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.MmrApiBaseUrl + "/api/CreateMatchId", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			GTDev.Log<string>("CreateMatchId Success: raw response: " + request.downloadHandler.text, null);
			this.OnCompleteCreateMatchId(request.downloadHandler.text, callback);
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
				this.OnCompleteCreateMatchId(request.downloadHandler.text, callback);
			}
		}
		if (retry)
		{
			if (this.CreateMatchIdRetryCount < this.MAX_SERVER_RETRIES)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.CreateMatchIdRetryCount + 1));
				this.CreateMatchIdRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.CreateMatchIdInProgress = false;
				this.RequestCreateMatchId(callback);
			}
			else
			{
				this.CreateMatchIdRetryCount = 0;
				this.OnCompleteCreateMatchId(null, callback);
			}
		}
		yield break;
	}

	// Token: 0x06002D34 RID: 11572 RVA: 0x000EE25C File Offset: 0x000EC45C
	private void OnCompleteCreateMatchId([CanBeNull] string response, Action<string> callback)
	{
		this.CreateMatchIdInProgress = false;
		this.CreateMatchIdRetryCount = 0;
		if (response.IsNullOrEmpty())
		{
			return;
		}
		if (callback != null)
		{
			callback(response);
		}
	}

	// Token: 0x06002D35 RID: 11573 RVA: 0x000EE280 File Offset: 0x000EC480
	public void RequestValidateMatchJoin(string matchId, Action<bool> callback)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestValidateMatchJoin Client Not Logged into Mothership", null);
			return;
		}
		if (this.ValidateMatchJoinInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestValidateMatchJoin already in progress", null);
			return;
		}
		string platform = "PC";
		this.ValidateMatchJoinInProgress = true;
		base.StartCoroutine(this.ValidateMatchJoin(new GorillaTagCompetitiveServerApi.RankedModeRequestDataWithMatchId
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			platform = platform,
			matchId = matchId
		}, callback));
	}

	// Token: 0x06002D36 RID: 11574 RVA: 0x000EE2FE File Offset: 0x000EC4FE
	private IEnumerator ValidateMatchJoin(GorillaTagCompetitiveServerApi.RankedModeRequestDataWithMatchId data, Action<bool> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.MmrApiBaseUrl + "/api/ValidateMatchJoin", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			GTDev.Log<string>("ValidateMatchJoin Success: raw response: " + request.downloadHandler.text, null);
			this.OnCompleteValidateMatchJoin(request.downloadHandler.text, callback);
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
				this.OnCompleteValidateMatchJoin(request.downloadHandler.text, callback);
			}
		}
		if (retry)
		{
			if (this.ValidateMatchJoinRetryCount < this.MAX_SERVER_RETRIES)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.ValidateMatchJoinRetryCount + 1));
				this.ValidateMatchJoinRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.ValidateMatchJoinInProgress = false;
				this.RequestValidateMatchJoin(data.matchId, callback);
			}
			else
			{
				this.ValidateMatchJoinRetryCount = 0;
				this.OnCompleteValidateMatchJoin(null, callback);
			}
		}
		yield break;
	}

	// Token: 0x06002D37 RID: 11575 RVA: 0x000EE31C File Offset: 0x000EC51C
	private void OnCompleteValidateMatchJoin([CanBeNull] string response, Action<bool> callback)
	{
		this.ValidateMatchJoinInProgress = false;
		this.ValidateMatchJoinRetryCount = 0;
		if (response.IsNullOrEmpty())
		{
			return;
		}
		GorillaTagCompetitiveServerApi.RankedModeValidateMatchJoinResponseData rankedModeValidateMatchJoinResponseData = JsonUtility.FromJson<GorillaTagCompetitiveServerApi.RankedModeValidateMatchJoinResponseData>(response);
		if (callback != null)
		{
			callback(rankedModeValidateMatchJoinResponseData.validJoin);
		}
	}

	// Token: 0x06002D38 RID: 11576 RVA: 0x000EE358 File Offset: 0x000EC558
	public void RequestSubmitMatchScores(string matchId, List<RankedMultiplayerScore.PlayerScore> finalScores)
	{
		List<GorillaTagCompetitiveServerApi.RankedModePlayerScore> list = new List<GorillaTagCompetitiveServerApi.RankedModePlayerScore>();
		foreach (RankedMultiplayerScore.PlayerScore playerScore in finalScores)
		{
			NetPlayer player = NetworkSystem.Instance.GetPlayer(playerScore.PlayerId);
			list.Add(new GorillaTagCompetitiveServerApi.RankedModePlayerScore
			{
				playfabId = player.UserId,
				gameScore = playerScore.GameScore
			});
		}
		this.RequestSubmitMatchScores(matchId, list);
	}

	// Token: 0x06002D39 RID: 11577 RVA: 0x000EE3E4 File Offset: 0x000EC5E4
	private void RequestSubmitMatchScores(string matchId, List<GorillaTagCompetitiveServerApi.RankedModePlayerScore> playerScores)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestSubmitMatchScores Client Not Logged into Mothership", null);
			return;
		}
		if (this.SubmitMatchScoresInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestSubmitMatchScores already in progress", null);
			return;
		}
		this.SubmitMatchScoresInProgress = true;
		base.StartCoroutine(this.SubmitMatchScores(new GorillaTagCompetitiveServerApi.RankedModeSubmitMatchScoresRequestData
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			matchId = matchId,
			playfabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			playerScores = playerScores
		}));
	}

	// Token: 0x06002D3A RID: 11578 RVA: 0x000EE467 File Offset: 0x000EC667
	private IEnumerator SubmitMatchScores(GorillaTagCompetitiveServerApi.RankedModeSubmitMatchScoresRequestData data)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.MmrApiBaseUrl + "/api/SubmitMatchScores", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			GTDev.Log<string>("SubmitMatchScores Success: raw response: " + request.downloadHandler.text, null);
			this.OnCompleteSubmitMatchScores(request.downloadHandler.text);
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
				this.OnCompleteSubmitMatchScores(request.downloadHandler.text);
			}
		}
		if (retry)
		{
			if (this.SubmitMatchScoresRetryCount < this.MAX_SERVER_RETRIES)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.SubmitMatchScoresRetryCount + 1));
				this.SubmitMatchScoresRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.SubmitMatchScoresInProgress = false;
				this.RequestSubmitMatchScores(data.matchId, data.playerScores);
			}
			else
			{
				this.SubmitMatchScoresRetryCount = 0;
				this.OnCompleteSubmitMatchScores(null);
			}
		}
		yield break;
	}

	// Token: 0x06002D3B RID: 11579 RVA: 0x000EE47D File Offset: 0x000EC67D
	private void OnCompleteSubmitMatchScores([CanBeNull] string response)
	{
		this.SubmitMatchScoresInProgress = false;
		this.SubmitMatchScoresRetryCount = 0;
	}

	// Token: 0x06002D3C RID: 11580 RVA: 0x000EE490 File Offset: 0x000EC690
	public void RequestSetEloValue(float desiredElo, Action callback)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestSetEloValue Client Not Logged into Mothership", null);
			return;
		}
		if (this.SetEloValueInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestSetEloValue already in progress", null);
			return;
		}
		string platform = "PC";
		this.SetEloValueInProgress = true;
		base.StartCoroutine(this.SetEloValue(new GorillaTagCompetitiveServerApi.RankedModeSetEloValueRequestData
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			platform = platform,
			elo = desiredElo
		}, callback));
	}

	// Token: 0x06002D3D RID: 11581 RVA: 0x000EE50E File Offset: 0x000EC70E
	private IEnumerator SetEloValue(GorillaTagCompetitiveServerApi.RankedModeSetEloValueRequestData data, Action callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.MmrApiBaseUrl + "/api/UpdateELOInternal", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		request.SetRequestHeader("x-functions-key", GorillaTagCompetitiveServerApi.FUNCTION_KEY);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			GTDev.Log<string>("SetEloValue Success: raw response: " + request.downloadHandler.text, null);
			this.OnCompleteSetEloValue(request.downloadHandler.text, callback);
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
				this.OnCompleteSetEloValue(request.downloadHandler.text, callback);
			}
		}
		if (retry)
		{
			if (this.SetEloValueRetryCount < this.MAX_SERVER_RETRIES)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.SetEloValueRetryCount + 1));
				this.SetEloValueRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.SetEloValueInProgress = false;
				this.RequestSetEloValue(data.elo, callback);
			}
			else
			{
				this.SetEloValueRetryCount = 0;
				this.OnCompleteSetEloValue(null, callback);
			}
		}
		yield break;
	}

	// Token: 0x06002D3E RID: 11582 RVA: 0x000EE52B File Offset: 0x000EC72B
	private void OnCompleteSetEloValue([CanBeNull] string response, Action callback)
	{
		this.SetEloValueInProgress = false;
		this.SetEloValueRetryCount = 0;
		if (response != null && callback != null)
		{
			callback();
		}
	}

	// Token: 0x06002D3F RID: 11583 RVA: 0x000EE548 File Offset: 0x000EC748
	public void RequestPingRoom(string matchId, Action callback)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestPingRoom Client Not Logged into Mothership", null);
			return;
		}
		if (this.SetEloValueInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestPingRoom already in progress", null);
			return;
		}
		string platform = "PC";
		this.PingMatchInProgress = true;
		base.StartCoroutine(this.PingRoom(new GorillaTagCompetitiveServerApi.RankedModeRequestDataWithMatchId
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			platform = platform,
			matchId = matchId
		}, callback));
	}

	// Token: 0x06002D40 RID: 11584 RVA: 0x000EE5C6 File Offset: 0x000EC7C6
	private IEnumerator PingRoom(GorillaTagCompetitiveServerApi.RankedModeRequestDataWithMatchId data, Action callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.MmrApiBaseUrl + "/api/PingRoom", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			GTDev.Log<string>("PingRoom Success: raw response: " + request.downloadHandler.text, null);
			this.OnCompletePingRoom(request.downloadHandler.text, callback);
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
				this.OnCompletePingRoom(request.downloadHandler.text, callback);
			}
		}
		if (retry)
		{
			if (this.PingMatchRetryCount < this.MAX_SERVER_RETRIES)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.PingMatchRetryCount + 1));
				this.ValidateMatchJoinRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.PingMatchInProgress = false;
				this.RequestPingRoom(data.matchId, callback);
			}
			else
			{
				this.PingMatchRetryCount = 0;
				this.OnCompletePingRoom(null, callback);
			}
		}
		yield break;
	}

	// Token: 0x06002D41 RID: 11585 RVA: 0x000EE5E3 File Offset: 0x000EC7E3
	private void OnCompletePingRoom([CanBeNull] string response, Action callback)
	{
		GTDev.Log<string>("PingRoom complete", null);
		this.PingMatchInProgress = false;
		this.PingMatchRetryCount = 0;
		if (response != null && callback != null)
		{
			callback();
		}
	}

	// Token: 0x06002D42 RID: 11586 RVA: 0x000EE60C File Offset: 0x000EC80C
	public void RequestUnlockCompetitiveQueue(bool unlocked, Action callback)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestUnlockCompetitiveQueue Client Not Logged into Mothership", null);
			return;
		}
		if (this.UnlockCompetitiveQueueInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestUnlockCompetitiveQueue already in progress", null);
			return;
		}
		string platform = "PC";
		this.UnlockCompetitiveQueueInProgress = true;
		base.StartCoroutine(this.UnlockCompetitiveQueue(new GorillaTagCompetitiveServerApi.RankedModeUnlockCompetitiveQueueRequestData
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			platform = platform,
			unlocked = unlocked
		}, callback));
	}

	// Token: 0x06002D43 RID: 11587 RVA: 0x000EE68A File Offset: 0x000EC88A
	private IEnumerator UnlockCompetitiveQueue(GorillaTagCompetitiveServerApi.RankedModeUnlockCompetitiveQueueRequestData data, Action callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.MmrApiBaseUrl + "/api/UnlockCompetitiveQueue", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			GTDev.Log<string>("UnlockCompetitiveQueue Success: raw response: " + request.downloadHandler.text, null);
			this.OnCompleteUnlockCompetitiveQueue(request.downloadHandler.text, callback);
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
				this.OnCompleteUnlockCompetitiveQueue(request.downloadHandler.text, callback);
			}
		}
		if (retry)
		{
			if (this.UnlockCompetitiveQueueRetryCount < this.MAX_SERVER_RETRIES)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.UnlockCompetitiveQueueRetryCount + 1));
				this.ValidateMatchJoinRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.UnlockCompetitiveQueueInProgress = false;
				this.RequestUnlockCompetitiveQueue(data.unlocked, callback);
			}
			else
			{
				this.UnlockCompetitiveQueueRetryCount = 0;
				this.OnCompleteUnlockCompetitiveQueue(null, callback);
			}
		}
		yield break;
	}

	// Token: 0x06002D44 RID: 11588 RVA: 0x000EE6A7 File Offset: 0x000EC8A7
	private void OnCompleteUnlockCompetitiveQueue([CanBeNull] string response, Action callback)
	{
		GTDev.Log<string>("UnlockCompetitiveQueue complete", null);
		this.UnlockCompetitiveQueueInProgress = false;
		this.UnlockCompetitiveQueueRetryCount = 0;
		if (response != null && callback != null)
		{
			callback();
		}
	}

	// Token: 0x0400387D RID: 14461
	public static GorillaTagCompetitiveServerApi Instance;

	// Token: 0x0400387E RID: 14462
	public static string FUNCTION_KEY = "";

	// Token: 0x0400387F RID: 14463
	public int MAX_SERVER_RETRIES = 3;

	// Token: 0x04003880 RID: 14464
	private bool GetRankInformationInProgress;

	// Token: 0x04003881 RID: 14465
	private int GetRankInformationRetryCount;

	// Token: 0x04003882 RID: 14466
	private bool CreateMatchIdInProgress;

	// Token: 0x04003883 RID: 14467
	private int CreateMatchIdRetryCount;

	// Token: 0x04003884 RID: 14468
	private bool ValidateMatchJoinInProgress;

	// Token: 0x04003885 RID: 14469
	private int ValidateMatchJoinRetryCount;

	// Token: 0x04003886 RID: 14470
	private bool SubmitMatchScoresInProgress;

	// Token: 0x04003887 RID: 14471
	private int SubmitMatchScoresRetryCount;

	// Token: 0x04003888 RID: 14472
	private bool SetEloValueInProgress;

	// Token: 0x04003889 RID: 14473
	private int SetEloValueRetryCount;

	// Token: 0x0400388A RID: 14474
	private bool PingMatchInProgress;

	// Token: 0x0400388B RID: 14475
	private int PingMatchRetryCount;

	// Token: 0x0400388C RID: 14476
	private bool UnlockCompetitiveQueueInProgress;

	// Token: 0x0400388D RID: 14477
	private int UnlockCompetitiveQueueRetryCount;

	// Token: 0x0200070B RID: 1803
	public enum EPlatformType
	{
		// Token: 0x0400388F RID: 14479
		PC,
		// Token: 0x04003890 RID: 14480
		Quest,
		// Token: 0x04003891 RID: 14481
		NumPlatforms
	}

	// Token: 0x0200070C RID: 1804
	[Serializable]
	public class RankedModeRequestDataBase
	{
		// Token: 0x04003892 RID: 14482
		public string mothershipId;

		// Token: 0x04003893 RID: 14483
		public string mothershipToken;
	}

	// Token: 0x0200070D RID: 1805
	[Serializable]
	public class RankedModeRequestDataPlatformed : GorillaTagCompetitiveServerApi.RankedModeRequestDataBase
	{
		// Token: 0x04003894 RID: 14484
		public string platform;
	}

	// Token: 0x0200070E RID: 1806
	[Serializable]
	public class RankedModeProgressionRequestData : GorillaTagCompetitiveServerApi.RankedModeRequestDataPlatformed
	{
		// Token: 0x04003895 RID: 14485
		public List<string> playfabIds;
	}

	// Token: 0x0200070F RID: 1807
	[Serializable]
	public class RankedModeProgressionPlatformData
	{
		// Token: 0x04003896 RID: 14486
		public string platform;

		// Token: 0x04003897 RID: 14487
		public float elo;

		// Token: 0x04003898 RID: 14488
		public int majorTier;

		// Token: 0x04003899 RID: 14489
		public int minorTier;

		// Token: 0x0400389A RID: 14490
		public float rankProgress;
	}

	// Token: 0x02000710 RID: 1808
	[Serializable]
	public class RankedModePlayerProgressionData
	{
		// Token: 0x0400389B RID: 14491
		public string playfabID;

		// Token: 0x0400389C RID: 14492
		public GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData[] platformData = new GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData[2];
	}

	// Token: 0x02000711 RID: 1809
	[Serializable]
	public class RankedModeProgressionData
	{
		// Token: 0x0400389D RID: 14493
		public List<GorillaTagCompetitiveServerApi.RankedModePlayerProgressionData> playerData;
	}

	// Token: 0x02000712 RID: 1810
	[Serializable]
	public class RankedModeRequestDataWithMatchId : GorillaTagCompetitiveServerApi.RankedModeRequestDataPlatformed
	{
		// Token: 0x0400389E RID: 14494
		public string matchId;
	}

	// Token: 0x02000713 RID: 1811
	[Serializable]
	public class RankedModeValidateMatchJoinResponseData
	{
		// Token: 0x0400389F RID: 14495
		public bool validJoin;
	}

	// Token: 0x02000714 RID: 1812
	[Serializable]
	public class RankedModePlayerScore
	{
		// Token: 0x040038A0 RID: 14496
		public string playfabId;

		// Token: 0x040038A1 RID: 14497
		public float gameScore;
	}

	// Token: 0x02000715 RID: 1813
	[Serializable]
	public class RankedModeSubmitMatchScoresRequestData : GorillaTagCompetitiveServerApi.RankedModeRequestDataBase
	{
		// Token: 0x040038A2 RID: 14498
		public string matchId;

		// Token: 0x040038A3 RID: 14499
		public string playfabId;

		// Token: 0x040038A4 RID: 14500
		public List<GorillaTagCompetitiveServerApi.RankedModePlayerScore> playerScores;
	}

	// Token: 0x02000716 RID: 1814
	[Serializable]
	public class RankedModeSetEloValueRequestData : GorillaTagCompetitiveServerApi.RankedModeRequestDataPlatformed
	{
		// Token: 0x040038A5 RID: 14501
		public float elo;
	}

	// Token: 0x02000717 RID: 1815
	[Serializable]
	public class RankedModeUnlockCompetitiveQueueRequestData : GorillaTagCompetitiveServerApi.RankedModeRequestDataPlatformed
	{
		// Token: 0x040038A6 RID: 14502
		public bool unlocked;
	}
}
