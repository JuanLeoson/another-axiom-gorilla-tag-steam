using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GorillaNetworking;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x0200013A RID: 314
public class ProgressionController : MonoBehaviour
{
	// Token: 0x14000020 RID: 32
	// (add) Token: 0x06000843 RID: 2115 RVA: 0x0002D944 File Offset: 0x0002BB44
	// (remove) Token: 0x06000844 RID: 2116 RVA: 0x0002D978 File Offset: 0x0002BB78
	public static event Action OnQuestSelectionChanged;

	// Token: 0x14000021 RID: 33
	// (add) Token: 0x06000845 RID: 2117 RVA: 0x0002D9AC File Offset: 0x0002BBAC
	// (remove) Token: 0x06000846 RID: 2118 RVA: 0x0002D9E0 File Offset: 0x0002BBE0
	public static event Action OnProgressEvent;

	// Token: 0x170000C7 RID: 199
	// (get) Token: 0x06000847 RID: 2119 RVA: 0x0002DA13 File Offset: 0x0002BC13
	// (set) Token: 0x06000848 RID: 2120 RVA: 0x0002DA1A File Offset: 0x0002BC1A
	public static int WeeklyCap { get; private set; } = 25;

	// Token: 0x170000C8 RID: 200
	// (get) Token: 0x06000849 RID: 2121 RVA: 0x0002DA22 File Offset: 0x0002BC22
	public static int TotalPoints
	{
		get
		{
			return ProgressionController._gInstance.totalPointsRaw - ProgressionController._gInstance.unclaimedPoints;
		}
	}

	// Token: 0x0600084A RID: 2122 RVA: 0x0002DA39 File Offset: 0x0002BC39
	public static void ReportQuestChanged(bool initialLoad)
	{
		ProgressionController._gInstance.OnQuestProgressChanged(initialLoad);
	}

	// Token: 0x0600084B RID: 2123 RVA: 0x0002DA46 File Offset: 0x0002BC46
	public static void ReportQuestSelectionChanged()
	{
		ProgressionController._gInstance.LoadCompletedQuestQueue();
		Action onQuestSelectionChanged = ProgressionController.OnQuestSelectionChanged;
		if (onQuestSelectionChanged == null)
		{
			return;
		}
		onQuestSelectionChanged();
	}

	// Token: 0x0600084C RID: 2124 RVA: 0x0002DA61 File Offset: 0x0002BC61
	public static void ReportQuestComplete(int questId, bool isDaily)
	{
		ProgressionController._gInstance.OnQuestComplete(questId, isDaily);
	}

	// Token: 0x0600084D RID: 2125 RVA: 0x0002DA6F File Offset: 0x0002BC6F
	public static void RedeemProgress()
	{
		ProgressionController._gInstance.RequestProgressRedemption(new Action(ProgressionController._gInstance.OnProgressRedeemed));
	}

	// Token: 0x0600084E RID: 2126 RVA: 0x0002DA8B File Offset: 0x0002BC8B
	[return: TupleElementNames(new string[]
	{
		"weekly",
		"unclaimed",
		"total"
	})]
	public static ValueTuple<int, int, int> GetProgressionData()
	{
		return ProgressionController._gInstance.GetProgress();
	}

	// Token: 0x0600084F RID: 2127 RVA: 0x0002DA97 File Offset: 0x0002BC97
	public static void RequestProgressUpdate()
	{
		ProgressionController gInstance = ProgressionController._gInstance;
		if (gInstance == null)
		{
			return;
		}
		gInstance.ReportProgress();
	}

	// Token: 0x06000850 RID: 2128 RVA: 0x0002DAA8 File Offset: 0x0002BCA8
	private void Awake()
	{
		if (ProgressionController._gInstance)
		{
			Debug.LogError("Duplicate ProgressionController detected. Destroying self.", base.gameObject);
			Object.Destroy(this);
			return;
		}
		ProgressionController._gInstance = this;
		this.unclaimedPoints = PlayerPrefs.GetInt("Claimed_Points_Key", 0);
		this.RequestStatus();
		this.LoadCompletedQuestQueue();
	}

	// Token: 0x06000851 RID: 2129 RVA: 0x0002DAFC File Offset: 0x0002BCFC
	private void RequestStatus()
	{
		ProgressionController.<RequestStatus>d__36 <RequestStatus>d__;
		<RequestStatus>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RequestStatus>d__.<>4__this = this;
		<RequestStatus>d__.<>1__state = -1;
		<RequestStatus>d__.<>t__builder.Start<ProgressionController.<RequestStatus>d__36>(ref <RequestStatus>d__);
	}

	// Token: 0x06000852 RID: 2130 RVA: 0x0002DB34 File Offset: 0x0002BD34
	private Task WaitForSessionToken()
	{
		ProgressionController.<WaitForSessionToken>d__37 <WaitForSessionToken>d__;
		<WaitForSessionToken>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForSessionToken>d__.<>1__state = -1;
		<WaitForSessionToken>d__.<>t__builder.Start<ProgressionController.<WaitForSessionToken>d__37>(ref <WaitForSessionToken>d__);
		return <WaitForSessionToken>d__.<>t__builder.Task;
	}

	// Token: 0x06000853 RID: 2131 RVA: 0x0002DB70 File Offset: 0x0002BD70
	private void FetchStatus()
	{
		base.StartCoroutine(this.DoFetchStatus(new ProgressionController.GetQuestsStatusRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			MothershipId = "",
			MothershipToken = ""
		}, new Action<ProgressionController.GetQuestStatusResponse>(this.OnFetchStatusResponse)));
	}

	// Token: 0x06000854 RID: 2132 RVA: 0x0002DBD5 File Offset: 0x0002BDD5
	private IEnumerator DoFetchStatus(ProgressionController.GetQuestsStatusRequest data, Action<ProgressionController.GetQuestStatusResponse> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl + "/api/GetQuestStatus", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionController.GetQuestStatusResponse obj = JsonConvert.DeserializeObject<ProgressionController.GetQuestStatusResponse>(request.downloadHandler.text);
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
			if (this._fetchStatusRetryCount < this._maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this._fetchStatusRetryCount + 1));
				this._fetchStatusRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.FetchStatus();
			}
			else
			{
				GTDev.LogError<string>("Maximum FetchStatus retries attempted. Please check your network connection.", null);
				this._fetchStatusRetryCount = 0;
				callback(null);
			}
		}
		yield break;
	}

	// Token: 0x06000855 RID: 2133 RVA: 0x0002DBF4 File Offset: 0x0002BDF4
	private void OnFetchStatusResponse([CanBeNull] ProgressionController.GetQuestStatusResponse response)
	{
		this._isFetchingStatus = false;
		this._statusReceived = false;
		if (response != null)
		{
			this.SetProgressionValues(response.result.GetWeeklyPoints(), this.unclaimedPoints, response.result.userPointsTotal);
			this.ReportProgress();
			return;
		}
		GTDev.LogError<string>("Error: Could not fetch status!", null);
	}

	// Token: 0x06000856 RID: 2134 RVA: 0x0002DC46 File Offset: 0x0002BE46
	private void SendQuestCompleted(int questId)
	{
		if (this._isSendingQuestComplete)
		{
			return;
		}
		this._isSendingQuestComplete = true;
		this.StartSendQuestComplete(questId);
	}

	// Token: 0x06000857 RID: 2135 RVA: 0x0002DC60 File Offset: 0x0002BE60
	private void StartSendQuestComplete(int questId)
	{
		base.StartCoroutine(this.DoSendQuestComplete(new ProgressionController.SetQuestCompleteRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			MothershipId = "",
			MothershipToken = "",
			QuestId = questId,
			ClientVersion = Application.version
		}, new Action<ProgressionController.SetQuestCompleteResponse>(this.OnSendQuestCompleteSuccess)));
	}

	// Token: 0x06000858 RID: 2136 RVA: 0x0002DCD7 File Offset: 0x0002BED7
	private IEnumerator DoSendQuestComplete(ProgressionController.SetQuestCompleteRequest data, Action<ProgressionController.SetQuestCompleteResponse> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl + "/api/SetQuestComplete", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionController.SetQuestCompleteResponse obj = JsonConvert.DeserializeObject<ProgressionController.SetQuestCompleteResponse>(request.downloadHandler.text);
			callback(obj);
			this.ProcessQuestSubmittedSuccess();
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				retry = true;
			}
			else if (request.responseCode == 403L)
			{
				GTDev.LogWarning<string>("User already reached the max number of completion points for this time period!", null);
				callback(null);
				this.ClearQuestQueue();
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				retry = true;
			}
		}
		if (retry)
		{
			if (this._sendQuestCompleteRetryCount < this._maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this._sendQuestCompleteRetryCount + 1));
				this._sendQuestCompleteRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.StartSendQuestComplete(data.QuestId);
			}
			else
			{
				GTDev.LogError<string>("Maximum SendQuestComplete retries attempted. Please check your network connection.", null);
				this._sendQuestCompleteRetryCount = 0;
				callback(null);
				this.ProcessQuestSubmittedFail();
			}
		}
		else
		{
			this._isSendingQuestComplete = false;
		}
		yield break;
	}

	// Token: 0x06000859 RID: 2137 RVA: 0x0002DCF4 File Offset: 0x0002BEF4
	private void OnSendQuestCompleteSuccess([CanBeNull] ProgressionController.SetQuestCompleteResponse response)
	{
		this._isSendingQuestComplete = false;
		if (response != null)
		{
			this.UpdateProgressionValues(response.result.GetWeeklyPoints(), response.result.userPointsTotal);
			this.ReportProgress();
		}
	}

	// Token: 0x0600085A RID: 2138 RVA: 0x0002DD22 File Offset: 0x0002BF22
	private void OnQuestProgressChanged(bool initialLoad)
	{
		this.ReportProgress();
	}

	// Token: 0x0600085B RID: 2139 RVA: 0x0002DD2A File Offset: 0x0002BF2A
	private void OnQuestComplete(int questId, bool isDaily)
	{
		this.QueueQuestCompletion(questId, isDaily);
	}

	// Token: 0x0600085C RID: 2140 RVA: 0x0002DD34 File Offset: 0x0002BF34
	private void QueueQuestCompletion(int questId, bool isDaily)
	{
		if (isDaily)
		{
			this._queuedDailyCompletedQuests.Add(questId);
		}
		else
		{
			this._queuedWeeklyCompletedQuests.Add(questId);
		}
		this.SaveCompletedQuestQueue();
		this.SubmitNextQuestInQueue();
	}

	// Token: 0x0600085D RID: 2141 RVA: 0x0002DD60 File Offset: 0x0002BF60
	private void SubmitNextQuestInQueue()
	{
		if (this._currentlyProcessingQuest == -1 && this.AreCompletedQuestsQueued())
		{
			int num = -1;
			if (this._queuedWeeklyCompletedQuests.Count > 0)
			{
				num = this._queuedWeeklyCompletedQuests[0];
			}
			else if (this._queuedDailyCompletedQuests.Count > 0)
			{
				num = this._queuedDailyCompletedQuests[0];
			}
			this._currentlyProcessingQuest = num;
			this.SendQuestCompleted(num);
		}
	}

	// Token: 0x0600085E RID: 2142 RVA: 0x0002DDC6 File Offset: 0x0002BFC6
	private void ClearQuestQueue()
	{
		this._currentlyProcessingQuest = -1;
		this._queuedDailyCompletedQuests.Clear();
		this._queuedWeeklyCompletedQuests.Clear();
		this.SaveCompletedQuestQueue();
	}

	// Token: 0x0600085F RID: 2143 RVA: 0x0002DDEC File Offset: 0x0002BFEC
	private void ProcessQuestSubmittedSuccess()
	{
		if (this._currentlyProcessingQuest != -1)
		{
			if (this.AreCompletedQuestsQueued())
			{
				if (this._queuedWeeklyCompletedQuests.Remove(this._currentlyProcessingQuest))
				{
					this.SaveCompletedQuestQueue();
				}
				else if (this._queuedDailyCompletedQuests.Remove(this._currentlyProcessingQuest))
				{
					this.SaveCompletedQuestQueue();
				}
			}
			this._currentlyProcessingQuest = -1;
			this.SubmitNextQuestInQueue();
		}
	}

	// Token: 0x06000860 RID: 2144 RVA: 0x0002DE4B File Offset: 0x0002C04B
	private void ProcessQuestSubmittedFail()
	{
		this._currentlyProcessingQuest = -1;
	}

	// Token: 0x06000861 RID: 2145 RVA: 0x0002DE54 File Offset: 0x0002C054
	private bool AreCompletedQuestsQueued()
	{
		return this._queuedDailyCompletedQuests.Count > 0 || this._queuedWeeklyCompletedQuests.Count > 0;
	}

	// Token: 0x06000862 RID: 2146 RVA: 0x0002DE74 File Offset: 0x0002C074
	private void SaveCompletedQuestQueue()
	{
		int num = 0;
		for (int i = 0; i < this._queuedDailyCompletedQuests.Count; i++)
		{
			PlayerPrefs.SetInt(string.Format("{0}{1}", "Queued_Quest_Daily_ID_Key", num), this._queuedDailyCompletedQuests[i]);
			num++;
		}
		int dailyQuestSetID = this._questManager.dailyQuestSetID;
		PlayerPrefs.SetInt("Queued_Quest_Daily_SetID_Key", dailyQuestSetID);
		PlayerPrefs.SetInt("Queued_Quest_Daily_SaveCount_Key", num);
		int num2 = 0;
		for (int j = 0; j < this._queuedWeeklyCompletedQuests.Count; j++)
		{
			PlayerPrefs.SetInt(string.Format("{0}{1}", "Queued_Quest_Weekly_ID_Key", num2), this._queuedWeeklyCompletedQuests[j]);
			num2++;
		}
		int weeklyQuestSetID = this._questManager.weeklyQuestSetID;
		PlayerPrefs.SetInt("Queued_Quest_Weekly_SetID_Key", weeklyQuestSetID);
		PlayerPrefs.SetInt("Queued_Quest_Weekly_SaveCount_Key", num2);
	}

	// Token: 0x06000863 RID: 2147 RVA: 0x0002DF54 File Offset: 0x0002C154
	private void LoadCompletedQuestQueue()
	{
		this._queuedDailyCompletedQuests.Clear();
		int @int = PlayerPrefs.GetInt("Queued_Quest_Daily_SetID_Key", -1);
		int int2 = PlayerPrefs.GetInt("Queued_Quest_Daily_SaveCount_Key", -1);
		int dailyQuestSetID = this._questManager.dailyQuestSetID;
		if (@int == dailyQuestSetID)
		{
			for (int i = 0; i < int2; i++)
			{
				int int3 = PlayerPrefs.GetInt(string.Format("{0}{1}", "Queued_Quest_Daily_ID_Key", i), -1);
				if (int3 != -1)
				{
					this._queuedDailyCompletedQuests.Add(int3);
				}
			}
		}
		this._queuedWeeklyCompletedQuests.Clear();
		int int4 = PlayerPrefs.GetInt("Queued_Quest_Weekly_SetID_Key", -1);
		int int5 = PlayerPrefs.GetInt("Queued_Quest_Weekly_SaveCount_Key", -1);
		int weeklyQuestSetID = this._questManager.weeklyQuestSetID;
		if (int4 == weeklyQuestSetID)
		{
			for (int j = 0; j < int5; j++)
			{
				int int6 = PlayerPrefs.GetInt(string.Format("{0}{1}", "Queued_Quest_Weekly_ID_Key", j), -1);
				if (int6 != -1)
				{
					this._queuedWeeklyCompletedQuests.Add(int6);
				}
			}
		}
		this.SubmitNextQuestInQueue();
	}

	// Token: 0x06000864 RID: 2148 RVA: 0x0002E04C File Offset: 0x0002C24C
	private void RequestProgressRedemption(Action onComplete)
	{
		ProgressionController.<RequestProgressRedemption>d__66 <RequestProgressRedemption>d__;
		<RequestProgressRedemption>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RequestProgressRedemption>d__.onComplete = onComplete;
		<RequestProgressRedemption>d__.<>1__state = -1;
		<RequestProgressRedemption>d__.<>t__builder.Start<ProgressionController.<RequestProgressRedemption>d__66>(ref <RequestProgressRedemption>d__);
	}

	// Token: 0x06000865 RID: 2149 RVA: 0x0002E083 File Offset: 0x0002C283
	private void OnProgressRedeemed()
	{
		this.unclaimedPoints = 0;
		PlayerPrefs.SetInt("Claimed_Points_Key", this.unclaimedPoints);
		this.ReportProgress();
	}

	// Token: 0x06000866 RID: 2150 RVA: 0x0002E0A4 File Offset: 0x0002C2A4
	private void AddPoints(int points)
	{
		if (this.weeklyPoints >= ProgressionController.WeeklyCap)
		{
			return;
		}
		int num = Mathf.Clamp(points, 0, ProgressionController.WeeklyCap - this.weeklyPoints);
		this.SetProgressionValues(this.weeklyPoints + num, this.unclaimedPoints + num, this.totalPointsRaw + num);
	}

	// Token: 0x06000867 RID: 2151 RVA: 0x0002E0F4 File Offset: 0x0002C2F4
	private void UpdateProgressionValues(int weekly, int totalRaw)
	{
		int num = totalRaw - this.totalPointsRaw;
		this.unclaimedPoints += num;
		this.SetProgressionValues(weekly, this.unclaimedPoints, totalRaw);
	}

	// Token: 0x06000868 RID: 2152 RVA: 0x0002E126 File Offset: 0x0002C326
	private void SetProgressionValues(int weekly, int unclaimed, int totalRaw)
	{
		this.weeklyPoints = weekly;
		this.unclaimedPoints = unclaimed;
		this.totalPointsRaw = totalRaw;
		this.ReportScoreChange();
		PlayerPrefs.SetInt("Claimed_Points_Key", unclaimed);
	}

	// Token: 0x06000869 RID: 2153 RVA: 0x0002E150 File Offset: 0x0002C350
	private void ReportProgress()
	{
		ProgressionController.<ReportProgress>d__71 <ReportProgress>d__;
		<ReportProgress>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<ReportProgress>d__.<>4__this = this;
		<ReportProgress>d__.<>1__state = -1;
		<ReportProgress>d__.<>t__builder.Start<ProgressionController.<ReportProgress>d__71>(ref <ReportProgress>d__);
	}

	// Token: 0x0600086A RID: 2154 RVA: 0x0002E188 File Offset: 0x0002C388
	private void ReportScoreChange()
	{
		ValueTuple<int, int, int> valueTuple = new ValueTuple<int, int, int>(this.weeklyPoints, this.unclaimedPoints, this.totalPointsRaw);
		ValueTuple<int, int, int> lastProgressReport = this._lastProgressReport;
		ValueTuple<int, int, int> valueTuple2 = valueTuple;
		if (lastProgressReport.Item1 == valueTuple2.Item1 && lastProgressReport.Item2 == valueTuple2.Item2 && lastProgressReport.Item3 == valueTuple2.Item3)
		{
			return;
		}
		if (VRRig.LocalRig)
		{
			VRRig.LocalRig.SetQuestScore(ProgressionController.TotalPoints);
		}
		this._lastProgressReport = valueTuple;
	}

	// Token: 0x0600086B RID: 2155 RVA: 0x0002E204 File Offset: 0x0002C404
	[return: TupleElementNames(new string[]
	{
		"weekly",
		"unclaimed",
		"total"
	})]
	private ValueTuple<int, int, int> GetProgress()
	{
		return new ValueTuple<int, int, int>(this.weeklyPoints, this.unclaimedPoints, this.totalPointsRaw - this.unclaimedPoints);
	}

	// Token: 0x0600086E RID: 2158 RVA: 0x0002E259 File Offset: 0x0002C459
	[CompilerGenerated]
	private bool <RequestStatus>g__ShouldFetchStatus|36_0()
	{
		return !this._isFetchingStatus && !this._statusReceived;
	}

	// Token: 0x040009D9 RID: 2521
	private static ProgressionController _gInstance;

	// Token: 0x040009DC RID: 2524
	[SerializeField]
	private RotatingQuestsManager _questManager;

	// Token: 0x040009DD RID: 2525
	private int weeklyPoints;

	// Token: 0x040009DE RID: 2526
	private int totalPointsRaw;

	// Token: 0x040009DF RID: 2527
	private int unclaimedPoints;

	// Token: 0x040009E0 RID: 2528
	private bool _progressReportPending;

	// Token: 0x040009E1 RID: 2529
	[TupleElementNames(new string[]
	{
		"weeklyPoints",
		"unclaimedPoints",
		"totalPointsRaw"
	})]
	private ValueTuple<int, int, int> _lastProgressReport;

	// Token: 0x040009E2 RID: 2530
	private bool _isFetchingStatus;

	// Token: 0x040009E3 RID: 2531
	private bool _statusReceived;

	// Token: 0x040009E4 RID: 2532
	private bool _isSendingQuestComplete;

	// Token: 0x040009E5 RID: 2533
	private int _fetchStatusRetryCount;

	// Token: 0x040009E6 RID: 2534
	private int _sendQuestCompleteRetryCount;

	// Token: 0x040009E7 RID: 2535
	private int _maxRetriesOnFail = 3;

	// Token: 0x040009E8 RID: 2536
	private List<int> _queuedDailyCompletedQuests = new List<int>();

	// Token: 0x040009E9 RID: 2537
	private List<int> _queuedWeeklyCompletedQuests = new List<int>();

	// Token: 0x040009EA RID: 2538
	private int _currentlyProcessingQuest = -1;

	// Token: 0x040009EB RID: 2539
	private const string kUnclaimedPointKey = "Claimed_Points_Key";

	// Token: 0x040009ED RID: 2541
	private const string kQueuedDailyQuestSetIDKey = "Queued_Quest_Daily_SetID_Key";

	// Token: 0x040009EE RID: 2542
	private const string kQueuedDailyQuestSaveCountKey = "Queued_Quest_Daily_SaveCount_Key";

	// Token: 0x040009EF RID: 2543
	private const string kQueuedDailyQuestIDKey = "Queued_Quest_Daily_ID_Key";

	// Token: 0x040009F0 RID: 2544
	private const string kQueuedWeeklyQuestSetIDKey = "Queued_Quest_Weekly_SetID_Key";

	// Token: 0x040009F1 RID: 2545
	private const string kQueuedWeeklyQuestSaveCountKey = "Queued_Quest_Weekly_SaveCount_Key";

	// Token: 0x040009F2 RID: 2546
	private const string kQueuedWeeklyQuestIDKey = "Queued_Quest_Weekly_ID_Key";

	// Token: 0x0200013B RID: 315
	[Serializable]
	private class GetQuestsStatusRequest
	{
		// Token: 0x040009F3 RID: 2547
		public string PlayFabId;

		// Token: 0x040009F4 RID: 2548
		public string PlayFabTicket;

		// Token: 0x040009F5 RID: 2549
		public string MothershipId;

		// Token: 0x040009F6 RID: 2550
		public string MothershipToken;
	}

	// Token: 0x0200013C RID: 316
	[Serializable]
	public class GetQuestStatusResponse
	{
		// Token: 0x040009F7 RID: 2551
		public ProgressionController.UserQuestsStatus result;
	}

	// Token: 0x0200013D RID: 317
	public class UserQuestsStatus
	{
		// Token: 0x06000871 RID: 2161 RVA: 0x0002E270 File Offset: 0x0002C470
		public int GetWeeklyPoints()
		{
			int num = 0;
			if (this.dailyPoints != null)
			{
				foreach (KeyValuePair<string, int> keyValuePair in this.dailyPoints)
				{
					num += keyValuePair.Value;
				}
			}
			if (this.weeklyPoints != null)
			{
				foreach (KeyValuePair<int, int> keyValuePair2 in this.weeklyPoints)
				{
					num += keyValuePair2.Value;
				}
			}
			return Mathf.Min(num, ProgressionController.WeeklyCap);
		}

		// Token: 0x040009F8 RID: 2552
		public Dictionary<string, int> dailyPoints;

		// Token: 0x040009F9 RID: 2553
		public Dictionary<int, int> weeklyPoints;

		// Token: 0x040009FA RID: 2554
		public int userPointsTotal;
	}

	// Token: 0x0200013E RID: 318
	[Serializable]
	private class SetQuestCompleteRequest
	{
		// Token: 0x040009FB RID: 2555
		public string PlayFabId;

		// Token: 0x040009FC RID: 2556
		public string PlayFabTicket;

		// Token: 0x040009FD RID: 2557
		public string MothershipId;

		// Token: 0x040009FE RID: 2558
		public string MothershipToken;

		// Token: 0x040009FF RID: 2559
		public int QuestId;

		// Token: 0x04000A00 RID: 2560
		public string ClientVersion;
	}

	// Token: 0x0200013F RID: 319
	[Serializable]
	public class SetQuestCompleteResponse
	{
		// Token: 0x04000A01 RID: 2561
		public ProgressionController.UserQuestsStatus result;
	}
}
