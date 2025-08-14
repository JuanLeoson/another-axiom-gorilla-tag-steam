using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameObjectScheduling;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000127 RID: 295
public class MonkeVoteMachine : MonoBehaviour
{
	// Token: 0x06000798 RID: 1944 RVA: 0x0002ADAA File Offset: 0x00028FAA
	private void Reset()
	{
		this.Configure();
	}

	// Token: 0x06000799 RID: 1945 RVA: 0x0002ADB2 File Offset: 0x00028FB2
	private void Awake()
	{
		this._proximityTrigger.OnEnter += this.OnPlayerEnteredVoteProximity;
	}

	// Token: 0x0600079A RID: 1946 RVA: 0x0002ADCC File Offset: 0x00028FCC
	private void Start()
	{
		MonkeVoteController.instance.OnPollsUpdated += this.HandleOnPollsUpdated;
		MonkeVoteController.instance.OnVoteAccepted += this.HandleOnVoteAccepted;
		MonkeVoteController.instance.OnVoteFailed += this.HandleOnVoteFailed;
		MonkeVoteController.instance.OnCurrentPollEnded += this.HandleCurrentPollEnded;
		this.Init();
	}

	// Token: 0x0600079B RID: 1947 RVA: 0x0002AE38 File Offset: 0x00029038
	private void OnDestroy()
	{
		this._proximityTrigger.OnEnter -= this.OnPlayerEnteredVoteProximity;
		MonkeVoteController.instance.OnPollsUpdated -= this.HandleOnPollsUpdated;
		MonkeVoteController.instance.OnVoteAccepted -= this.HandleOnVoteAccepted;
		MonkeVoteController.instance.OnVoteFailed -= this.HandleOnVoteFailed;
		MonkeVoteController.instance.OnCurrentPollEnded -= this.HandleCurrentPollEnded;
	}

	// Token: 0x0600079C RID: 1948 RVA: 0x0002AEB4 File Offset: 0x000290B4
	public void Init()
	{
		this._isTestingPoll = false;
		this._previousPoll = (this._currentPoll = null);
		this._waitingOnVote = false;
		foreach (MonkeVoteOption monkeVoteOption in this._votingOptions)
		{
			monkeVoteOption.ResetState();
			monkeVoteOption.OnVote += this.OnVoteEntered;
		}
		this.UpdatePollDisplays();
	}

	// Token: 0x0600079D RID: 1949 RVA: 0x0002AF14 File Offset: 0x00029114
	private void OnPlayerEnteredVoteProximity()
	{
		MonkeVoteController.instance.RequestPolls();
	}

	// Token: 0x0600079E RID: 1950 RVA: 0x0002AF20 File Offset: 0x00029120
	private void HandleOnPollsUpdated()
	{
		this.UpdatePollDisplays();
	}

	// Token: 0x0600079F RID: 1951 RVA: 0x0002AF28 File Offset: 0x00029128
	private void UpdatePollDisplays()
	{
		if (MonkeVoteController.instance == null)
		{
			this.SetState(MonkeVoteMachine.VotingState.None, true);
			this.ShowResults(null);
			return;
		}
		MonkeVoteController.FetchPollsResponse lastPollData = MonkeVoteController.instance.GetLastPollData();
		if (lastPollData != null)
		{
			this._previousPoll = new MonkeVoteMachine.PollEntry(lastPollData);
			this.ShowResults(this._previousPoll);
		}
		else
		{
			this.ShowResults(null);
		}
		MonkeVoteController.FetchPollsResponse currentPollData = MonkeVoteController.instance.GetCurrentPollData();
		if (currentPollData == null)
		{
			this.SetState(MonkeVoteMachine.VotingState.None, true);
			return;
		}
		this._nextPollUpdate = MonkeVoteController.instance.GetCurrentPollCompletionTime();
		this._currentPoll = new MonkeVoteMachine.PollEntry(currentPollData);
		MonkeVoteMachine.PollEntry currentPoll = this._currentPoll;
		if (currentPoll != null && currentPoll.IsValid)
		{
			ValueTuple<int, int> vote = this.GetVote(this._currentPoll.PollId);
			int item = vote.Item1;
			int item2 = vote.Item2;
			MonkeVoteMachine.VotingState newState = (item < 0) ? MonkeVoteMachine.VotingState.Voting : ((item2 < 0) ? MonkeVoteMachine.VotingState.Predicting : MonkeVoteMachine.VotingState.Complete);
			this.SetState(newState, true);
			return;
		}
		this.SetState(MonkeVoteMachine.VotingState.None, true);
	}

	// Token: 0x060007A0 RID: 1952 RVA: 0x0002B00C File Offset: 0x0002920C
	private void HandleOnVoteAccepted()
	{
		int lastVotePollId = MonkeVoteController.instance.GetLastVotePollId();
		int lastVoteSelectedOption = MonkeVoteController.instance.GetLastVoteSelectedOption();
		bool lastVoteWasPrediction = MonkeVoteController.instance.GetLastVoteWasPrediction();
		this.OnVoteResponseReceived(lastVotePollId, lastVoteSelectedOption, lastVoteWasPrediction, true);
	}

	// Token: 0x060007A1 RID: 1953 RVA: 0x0002B044 File Offset: 0x00029244
	private void HandleOnVoteFailed()
	{
		this._waitingOnVote = false;
		int lastVotePollId = MonkeVoteController.instance.GetLastVotePollId();
		int lastVoteSelectedOption = MonkeVoteController.instance.GetLastVoteSelectedOption();
		bool lastVoteWasPrediction = MonkeVoteController.instance.GetLastVoteWasPrediction();
		this.OnVoteResponseReceived(lastVotePollId, lastVoteSelectedOption, lastVoteWasPrediction, false);
	}

	// Token: 0x060007A2 RID: 1954 RVA: 0x0002B083 File Offset: 0x00029283
	private void HandleCurrentPollEnded()
	{
		if (this._proximityTrigger.isPlayerNearby)
		{
			MonkeVoteController.instance.RequestPolls();
		}
	}

	// Token: 0x060007A3 RID: 1955 RVA: 0x0002B09C File Offset: 0x0002929C
	[Tooltip("Hide dynamic child meshes to avoid them getting combined into the parent mesh on awake")]
	private void HideDynamicMeshes()
	{
		this.SetDynamicMeshesVisible(false);
	}

	// Token: 0x060007A4 RID: 1956 RVA: 0x0002B0A5 File Offset: 0x000292A5
	[Tooltip("Show dynamic child meshes to allow easy visualization")]
	private void ShowDynamicMeshes()
	{
		this.SetDynamicMeshesVisible(true);
	}

	// Token: 0x060007A5 RID: 1957 RVA: 0x0002B0B0 File Offset: 0x000292B0
	private void SetDynamicMeshesVisible(bool enabled)
	{
		MonkeVoteOption[] votingOptions = this._votingOptions;
		for (int i = 0; i < votingOptions.Length; i++)
		{
			votingOptions[i].SetDynamicMeshesVisible(enabled);
		}
		MonkeVoteResult[] results = this._results;
		for (int i = 0; i < results.Length; i++)
		{
			results[i].SetDynamicMeshesVisible(enabled);
		}
	}

	// Token: 0x060007A6 RID: 1958 RVA: 0x0002B0F9 File Offset: 0x000292F9
	private void Configure()
	{
		this._audio = base.GetComponentInChildren<AudioSource>();
		this._audio.spatialBlend = 1f;
		this._votingOptions = base.GetComponentsInChildren<MonkeVoteOption>();
		this._results = base.GetComponentsInChildren<MonkeVoteResult>();
	}

	// Token: 0x060007A7 RID: 1959 RVA: 0x0002B130 File Offset: 0x00029330
	public void CreateNextDummyPoll()
	{
		this._isTestingPoll = true;
		if (this._currentPoll != null)
		{
			this._previousPoll = this._currentPoll;
		}
		else
		{
			this._previousPoll = null;
		}
		this.ShowResults(this._previousPoll);
		int pollId = 0;
		if (this._previousPoll != null)
		{
			pollId = this._previousPoll.PollId + 1;
		}
		string question = "Test Question Number: " + Random.Range(1, 101).ToString();
		string text = "Answer " + Random.Range(1, 101).ToString();
		string text2 = "Answer " + Random.Range(1, 101).ToString();
		string[] voteOptions = new string[]
		{
			text,
			text2
		};
		this._currentPoll = new MonkeVoteMachine.PollEntry(pollId, question, voteOptions);
		MonkeVoteMachine.PollEntry currentPoll = this._currentPoll;
		if (currentPoll != null && currentPoll.IsValid)
		{
			ValueTuple<int, int> vote = this.GetVote(this._currentPoll.PollId);
			int item = vote.Item1;
			int item2 = vote.Item2;
			MonkeVoteMachine.VotingState newState = (item < 0) ? MonkeVoteMachine.VotingState.Voting : ((item2 < 0) ? MonkeVoteMachine.VotingState.Predicting : MonkeVoteMachine.VotingState.Complete);
			this.SetState(newState, true);
			return;
		}
		this.SetState(MonkeVoteMachine.VotingState.None, true);
	}

	// Token: 0x060007A8 RID: 1960 RVA: 0x0002B252 File Offset: 0x00029452
	private void VoteLeft()
	{
		this.OnVoteEntered(this._votingOptions[0], null);
	}

	// Token: 0x060007A9 RID: 1961 RVA: 0x0002B263 File Offset: 0x00029463
	private void VoteRight()
	{
		this.OnVoteEntered(this._votingOptions[1], null);
	}

	// Token: 0x060007AA RID: 1962 RVA: 0x0002B274 File Offset: 0x00029474
	private void VoteWinner()
	{
		if (this._currentPoll != null)
		{
			if (this._currentPoll.VoteCount[0] > this._currentPoll.VoteCount[1])
			{
				this.OnVoteEntered(this._votingOptions[0], null);
				return;
			}
			this.OnVoteEntered(this._votingOptions[1], null);
		}
	}

	// Token: 0x060007AB RID: 1963 RVA: 0x0002B2C4 File Offset: 0x000294C4
	private void ClearLocalData()
	{
		this.ClearLocalVoteAndPredictionData();
		this.UpdatePollDisplays();
	}

	// Token: 0x060007AC RID: 1964 RVA: 0x0002B2D4 File Offset: 0x000294D4
	private void SetState(MonkeVoteMachine.VotingState newState, bool instant = true)
	{
		this._state = newState;
		MonkeVoteMachine.PollEntry currentPoll = this._currentPoll;
		bool flag = currentPoll != null && currentPoll.IsValid;
		if (this._state < MonkeVoteMachine.VotingState.None || this._state > MonkeVoteMachine.VotingState.Complete || (this._state != MonkeVoteMachine.VotingState.None && !flag))
		{
			this._state = MonkeVoteMachine.VotingState.None;
		}
		if (flag)
		{
			int item = this.GetVote(this._currentPoll.PollId).Item2;
			if (this._state < MonkeVoteMachine.VotingState.Predicting)
			{
				this.SaveVote(this._currentPoll.PollId, -1, item);
			}
			int item2 = this.GetVote(this._currentPoll.PollId).Item1;
			if (this._state < MonkeVoteMachine.VotingState.Complete)
			{
				this.SaveVote(this._currentPoll.PollId, item2, -1);
			}
		}
		bool flag2 = true;
		switch (this._state)
		{
		case MonkeVoteMachine.VotingState.None:
			this._timerText.SetFixedText(this._pollsClosedText);
			this._titleText.text = this._defaultTitle;
			this._questionText.text = this._defaultQuestion;
			flag2 = false;
			break;
		case MonkeVoteMachine.VotingState.Voting:
			this._timerText.SetCountdownTime(this._nextPollUpdate);
			this._titleText.text = this._voteTitle;
			this._questionText.text = this._currentPoll.Question;
			break;
		case MonkeVoteMachine.VotingState.Predicting:
			this._timerText.SetCountdownTime(this._nextPollUpdate);
			this._titleText.text = this._predictTitle;
			this._questionText.text = this._predictQuestion;
			break;
		case MonkeVoteMachine.VotingState.Complete:
			this._timerText.SetCountdownTime(this._nextPollUpdate);
			this._titleText.text = this._completeTitle;
			this._questionText.text = this._currentPoll.Question;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		int num;
		int num2;
		if (!flag)
		{
			num = -1;
			num2 = -1;
		}
		else
		{
			ValueTuple<int, int> vote = this.GetVote(this._currentPoll.PollId);
			num = vote.Item1;
			num2 = vote.Item2;
		}
		if (flag2)
		{
			for (int i = 0; i < this._votingOptions.Length; i++)
			{
				this._votingOptions[i].Text = this._currentPoll.VoteOptions[i];
				this._votingOptions[i].ShowIndicators(num == i, num2 == i, instant);
			}
			return;
		}
		foreach (MonkeVoteOption monkeVoteOption in this._votingOptions)
		{
			monkeVoteOption.Text = string.Empty;
			monkeVoteOption.ShowIndicators(false, false, true);
		}
	}

	// Token: 0x060007AD RID: 1965 RVA: 0x0002B550 File Offset: 0x00029750
	private void ShowResults(MonkeVoteMachine.PollEntry entry)
	{
		if (entry != null && entry.IsValid)
		{
			ValueTuple<int, int> vote = this.GetVote(entry.PollId);
			int item = vote.Item1;
			int item2 = vote.Item2;
			GTDev.Log<string>(string.Format("Showing {0} V:{1} P:{2}", entry.Question, item, item2), null);
			List<int> list = this.ConvertToPercentages(entry.VoteCount);
			int num = 0;
			int num2 = -1;
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] > num)
				{
					num = list[i];
					num2 = i;
				}
			}
			this._resultsTitleText.text = this._defaultResultsTitle;
			this._resultsQuestionText.text = entry.Question;
			for (int j = 0; j < entry.VoteOptions.Length; j++)
			{
				this._results[j].ShowResult(entry.VoteOptions[j], list[j], item == j, item2 == j, num2 == j);
			}
			int prePollStreak = this.GetPrePollStreak(entry.PollId);
			int postPollStreak = this.GetPostPollStreak(entry);
			this._resultsStreakText.text = ((postPollStreak >= prePollStreak) ? string.Format(this._streakBlurb, postPollStreak) : string.Format(this._streakLostBlurb, prePollStreak, postPollStreak));
			return;
		}
		this._resultsTitleText.text = this._defaultResultsTitle;
		this._resultsQuestionText.text = this._defaultQuestion;
		this._resultsStreakText.text = string.Empty;
		MonkeVoteResult[] results = this._results;
		for (int k = 0; k < results.Length; k++)
		{
			results[k].HideResult();
		}
	}

	// Token: 0x060007AE RID: 1966 RVA: 0x0002B700 File Offset: 0x00029900
	private List<int> ConvertToPercentages(int[] votes)
	{
		List<int> list = new List<int>();
		List<float> list2 = new List<float>();
		if (votes == null || votes.Length == 0)
		{
			list.Add(-1);
			list.Add(-1);
			return list;
		}
		if (votes.Length == 1)
		{
			list.Add(100);
			list.Add(0);
			return list;
		}
		int num = MonkeVoteMachine.<ConvertToPercentages>g__Sum|64_0(votes);
		if (num == 0)
		{
			list.Add(-1);
			list.Add(-1);
			return list;
		}
		int num2 = -1;
		int num3 = 0;
		for (int i = 0; i < votes.Length; i++)
		{
			if (votes[i] > num2)
			{
				num2 = votes[i];
				num3 = i;
			}
			float num4 = (float)votes[i] / (float)num * 100f;
			list.Add((int)num4);
			list2.Add(num4 - (float)((int)num4));
		}
		int num5 = MonkeVoteMachine.<ConvertToPercentages>g__Sum|64_0(list);
		int num6 = 100 - num5;
		for (int j = 0; j < num6; j++)
		{
			int num7 = MonkeVoteMachine.<ConvertToPercentages>g__LargestFractionIndex|64_1(list2);
			List<int> list3 = list;
			int index = num7;
			int num8 = list3[index];
			list3[index] = num8 + 1;
			list2[num7] = 0f;
		}
		if (list.Count == 2 && list[num3] == 50)
		{
			List<int> list4 = list;
			int num8 = num3;
			list4[num8]++;
			list4 = list;
			num8 = 1 - num3;
			list4[num8]--;
		}
		return list;
	}

	// Token: 0x060007AF RID: 1967 RVA: 0x0002B84C File Offset: 0x00029A4C
	private void OnVoteEntered(MonkeVoteOption option, Collider votingCollider)
	{
		if (this._waitingOnVote || (Time.time < this._voteCooldownEnd && !this._isTestingPoll))
		{
			this.PlayVoteFailEffects();
			return;
		}
		int num = Array.IndexOf<MonkeVoteOption>(this._votingOptions, option);
		if (num < 0)
		{
			return;
		}
		switch (this._state)
		{
		case MonkeVoteMachine.VotingState.Voting:
			this.Vote(this._currentPoll.PollId, num, false);
			return;
		case MonkeVoteMachine.VotingState.Predicting:
			this.Vote(this._currentPoll.PollId, num, true);
			return;
		}
		this.PlayVoteFailEffects();
	}

	// Token: 0x060007B0 RID: 1968 RVA: 0x0002B8DC File Offset: 0x00029ADC
	private void Vote(int id, int option, bool isPrediction)
	{
		if (option < 0 || this._waitingOnVote)
		{
			return;
		}
		this._waitingOnVote = true;
		if (this._isTestingPoll)
		{
			this.OnVoteResponseReceived(id, option, isPrediction, true);
			return;
		}
		MonkeVoteController.instance.Vote(id, option, isPrediction);
	}

	// Token: 0x060007B1 RID: 1969 RVA: 0x0002B914 File Offset: 0x00029B14
	private void OnVoteResponseReceived(int id, int option, bool isPrediction, bool success)
	{
		this._waitingOnVote = false;
		if (success)
		{
			this.PlayVoteSuccessEffects();
			this._voteCooldownEnd = Time.time + this._voteCooldown;
			ValueTuple<int, int> vote = this.GetVote(id);
			int num = vote.Item1;
			int num2 = vote.Item2;
			if (!isPrediction)
			{
				int num3 = num2;
				num = option;
				num2 = num3;
			}
			else
			{
				num = num;
				num2 = option;
			}
			this.SaveVote(id, num, num2);
			MonkeVoteMachine.VotingState state = this._state;
			if (state != MonkeVoteMachine.VotingState.Voting)
			{
				if (state == MonkeVoteMachine.VotingState.Predicting)
				{
					this.SetState(MonkeVoteMachine.VotingState.Complete, false);
				}
			}
			else
			{
				this.SetState(MonkeVoteMachine.VotingState.Predicting, false);
			}
			if (isPrediction && id == this._currentPoll.PollId)
			{
				this.SavePrePollStreak(id, this.GetPostPollStreak(this._previousPoll));
				return;
			}
		}
		else
		{
			this.PlayVoteFailEffects();
		}
	}

	// Token: 0x060007B2 RID: 1970 RVA: 0x0002B9C8 File Offset: 0x00029BC8
	private void PlayVoteSuccessEffects()
	{
		MonkeVoteMachine.<PlayVoteSuccessEffects>d__68 <PlayVoteSuccessEffects>d__;
		<PlayVoteSuccessEffects>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<PlayVoteSuccessEffects>d__.<>4__this = this;
		<PlayVoteSuccessEffects>d__.<>1__state = -1;
		<PlayVoteSuccessEffects>d__.<>t__builder.Start<MonkeVoteMachine.<PlayVoteSuccessEffects>d__68>(ref <PlayVoteSuccessEffects>d__);
	}

	// Token: 0x060007B3 RID: 1971 RVA: 0x0002B9FF File Offset: 0x00029BFF
	private void PlayVoteFailEffects()
	{
		this._audio.GTPlayOneShot(this._voteFailSound, this._audio.volume);
	}

	// Token: 0x060007B4 RID: 1972 RVA: 0x0002BA20 File Offset: 0x00029C20
	private void SaveVote(int id, int voteOption, int predictionOption)
	{
		int @int = PlayerPrefs.GetInt("Vote_Current_Id", -1);
		if (@int == -1 || @int == id)
		{
			PlayerPrefs.SetInt("Vote_Current_Id", id);
			PlayerPrefs.SetInt("Vote_Current_Option", voteOption);
			PlayerPrefs.SetInt("Vote_Current_Prediction", predictionOption);
		}
		else
		{
			PlayerPrefs.SetInt("Vote_Previous_Id", @int);
			PlayerPrefs.SetInt("Vote_Previous_Option", PlayerPrefs.GetInt("Vote_Current_Option"));
			PlayerPrefs.SetInt("Vote_Previous_Prediction", PlayerPrefs.GetInt("Vote_Current_Prediction"));
			PlayerPrefs.SetInt("Vote_Previous_Streak", PlayerPrefs.GetInt("Vote_Current_Streak"));
			PlayerPrefs.SetInt("Vote_Current_Id", id);
			PlayerPrefs.SetInt("Vote_Current_Option", voteOption);
			PlayerPrefs.SetInt("Vote_Current_Prediction", predictionOption);
			PlayerPrefs.SetInt("Vote_Current_Streak", 0);
		}
		PlayerPrefs.Save();
	}

	// Token: 0x060007B5 RID: 1973 RVA: 0x0002BADC File Offset: 0x00029CDC
	[return: TupleElementNames(new string[]
	{
		"voteOption",
		"predictionOption"
	})]
	private ValueTuple<int, int> GetVote(int voteId)
	{
		if (PlayerPrefs.GetInt("Vote_Current_Id", -1) == voteId)
		{
			int @int = PlayerPrefs.GetInt("Vote_Current_Option", -1);
			int int2 = PlayerPrefs.GetInt("Vote_Current_Prediction", -1);
			return new ValueTuple<int, int>(@int, int2);
		}
		if (PlayerPrefs.GetInt("Vote_Previous_Id", -1) == voteId)
		{
			int int3 = PlayerPrefs.GetInt("Vote_Previous_Option", -1);
			int int4 = PlayerPrefs.GetInt("Vote_Previous_Prediction", -1);
			return new ValueTuple<int, int>(int3, int4);
		}
		return new ValueTuple<int, int>(-1, -1);
	}

	// Token: 0x060007B6 RID: 1974 RVA: 0x0002BB48 File Offset: 0x00029D48
	private void SavePrePollStreak(int id, int streak)
	{
		if (id < 0)
		{
			return;
		}
		if (PlayerPrefs.GetInt("Vote_Current_Id", -1) == id)
		{
			PlayerPrefs.SetInt("Vote_Current_Streak", streak);
			return;
		}
		if (PlayerPrefs.GetInt("Vote_Previous_Id", -1) == id)
		{
			PlayerPrefs.SetInt("Vote_Previous_Streak", streak);
		}
	}

	// Token: 0x060007B7 RID: 1975 RVA: 0x0002BB82 File Offset: 0x00029D82
	private int GetPrePollStreak(int id)
	{
		if (id < 0)
		{
			return 0;
		}
		if (PlayerPrefs.GetInt("Vote_Current_Id", -1) == id)
		{
			return PlayerPrefs.GetInt("Vote_Current_Streak", 0);
		}
		if (PlayerPrefs.GetInt("Vote_Previous_Id", -1) == id)
		{
			return PlayerPrefs.GetInt("Vote_Previous_Streak", 0);
		}
		return 0;
	}

	// Token: 0x060007B8 RID: 1976 RVA: 0x0002BBC0 File Offset: 0x00029DC0
	private int GetPostPollStreak(MonkeVoteMachine.PollEntry entry)
	{
		if (entry == null || !entry.IsValid)
		{
			return 0;
		}
		int item = this.GetVote(entry.PollId).Item2;
		if (item < 0)
		{
			return 0;
		}
		int prePollStreak = this.GetPrePollStreak(entry.PollId);
		if (item != entry.GetWinner())
		{
			return 0;
		}
		return prePollStreak + 1;
	}

	// Token: 0x060007B9 RID: 1977 RVA: 0x0002BC10 File Offset: 0x00029E10
	private void ClearLocalVoteAndPredictionData()
	{
		PlayerPrefs.DeleteKey("Vote_Current_Id");
		PlayerPrefs.DeleteKey("Vote_Current_Option");
		PlayerPrefs.DeleteKey("Vote_Current_Prediction");
		PlayerPrefs.DeleteKey("Vote_Current_Streak");
		PlayerPrefs.DeleteKey("Vote_Previous_Id");
		PlayerPrefs.DeleteKey("Vote_Previous_Option");
		PlayerPrefs.DeleteKey("Vote_Previous_Prediction");
		PlayerPrefs.DeleteKey("Vote_Previous_Streak");
	}

	// Token: 0x060007BB RID: 1979 RVA: 0x0002BCFC File Offset: 0x00029EFC
	[CompilerGenerated]
	internal static int <ConvertToPercentages>g__Sum|64_0(IList<int> items)
	{
		int num = 0;
		foreach (int num2 in items)
		{
			num += num2;
		}
		return num;
	}

	// Token: 0x060007BC RID: 1980 RVA: 0x0002BD44 File Offset: 0x00029F44
	[CompilerGenerated]
	internal static int <ConvertToPercentages>g__LargestFractionIndex|64_1(IList<float> fractions)
	{
		float num = float.NegativeInfinity;
		int result = -1;
		for (int i = 0; i < fractions.Count; i++)
		{
			if (fractions[i] > num)
			{
				num = fractions[i];
				result = i;
			}
		}
		return result;
	}

	// Token: 0x04000931 RID: 2353
	private const string kVoteCurrentIdKey = "Vote_Current_Id";

	// Token: 0x04000932 RID: 2354
	private const string kVoteCurrentOptionKey = "Vote_Current_Option";

	// Token: 0x04000933 RID: 2355
	private const string kVoteCurrentPredictionKey = "Vote_Current_Prediction";

	// Token: 0x04000934 RID: 2356
	private const string kVoteCurrentStreak = "Vote_Current_Streak";

	// Token: 0x04000935 RID: 2357
	private const string kVotePreviousIdKey = "Vote_Previous_Id";

	// Token: 0x04000936 RID: 2358
	private const string kVotePreviousOptionKey = "Vote_Previous_Option";

	// Token: 0x04000937 RID: 2359
	private const string kVotePreviousPredictionKey = "Vote_Previous_Prediction";

	// Token: 0x04000938 RID: 2360
	private const string kVotePreviousStreak = "Vote_Previous_Streak";

	// Token: 0x04000939 RID: 2361
	[SerializeField]
	private MonkeVoteProximityTrigger _proximityTrigger;

	// Token: 0x0400093A RID: 2362
	[Header("VOTING")]
	[SerializeField]
	private string _pollsClosedText = "POLLS CLOSED";

	// Token: 0x0400093B RID: 2363
	[SerializeField]
	private string _defaultTitle = "MONKE VOTE";

	// Token: 0x0400093C RID: 2364
	[SerializeField]
	private string _voteTitle = "VOTE";

	// Token: 0x0400093D RID: 2365
	[SerializeField]
	private string _predictTitle = "GUESS";

	// Token: 0x0400093E RID: 2366
	[SerializeField]
	private string _completeTitle = "VOTING COMPLETE";

	// Token: 0x0400093F RID: 2367
	[SerializeField]
	private string _defaultQuestion = "COME BACK LATER";

	// Token: 0x04000940 RID: 2368
	[SerializeField]
	private string _predictQuestion = "WHICH WILL BE MORE POPULAR?";

	// Token: 0x04000941 RID: 2369
	[Tooltip("Must be in the format \"STREAK: {0}\"")]
	[SerializeField]
	private string _streakBlurb = "PREDICTION STREAK: {0}";

	// Token: 0x04000942 RID: 2370
	[Tooltip("Must be in the format \"LOST {0} PREDICTION STREAK! STREAK: {1}\"")]
	[SerializeField]
	private string _streakLostBlurb = "<color=red>{0} POLL STREAK LOST!</color>  STREAK: {1}";

	// Token: 0x04000943 RID: 2371
	[SerializeField]
	private float _voteCooldown = 1f;

	// Token: 0x04000944 RID: 2372
	[SerializeField]
	private MonkeVoteOption[] _votingOptions;

	// Token: 0x04000945 RID: 2373
	[SerializeField]
	private CountdownText _timerText;

	// Token: 0x04000946 RID: 2374
	[SerializeField]
	private TMP_Text _titleText;

	// Token: 0x04000947 RID: 2375
	[SerializeField]
	private TMP_Text _questionText;

	// Token: 0x04000948 RID: 2376
	[Header("RESULTS")]
	[SerializeField]
	private string _defaultResultsTitle = "PREVIOUS QUESTION";

	// Token: 0x04000949 RID: 2377
	[SerializeField]
	private TMP_Text _resultsTitleText;

	// Token: 0x0400094A RID: 2378
	[SerializeField]
	private TMP_Text _resultsQuestionText;

	// Token: 0x0400094B RID: 2379
	[SerializeField]
	private TMP_Text _resultsStreakText;

	// Token: 0x0400094C RID: 2380
	[SerializeField]
	private MonkeVoteResult[] _results;

	// Token: 0x0400094D RID: 2381
	[FormerlySerializedAs("_sound")]
	[Header("FX")]
	[SerializeField]
	private AudioSource _audio;

	// Token: 0x0400094E RID: 2382
	[FormerlySerializedAs("_voteProcessingAudio")]
	[SerializeField]
	private AudioSource _voteTubeAudio;

	// Token: 0x0400094F RID: 2383
	[SerializeField]
	private AudioClip[] _voteFailSound;

	// Token: 0x04000950 RID: 2384
	[SerializeField]
	private AudioClip[] _voteSuccessDing;

	// Token: 0x04000951 RID: 2385
	[FormerlySerializedAs("_voteSuccessSound")]
	[SerializeField]
	private AudioClip[] _voteProcessingSound;

	// Token: 0x04000952 RID: 2386
	private MonkeVoteMachine.VotingState _state;

	// Token: 0x04000953 RID: 2387
	private float _voteCooldownEnd;

	// Token: 0x04000954 RID: 2388
	private bool _waitingOnVote;

	// Token: 0x04000955 RID: 2389
	private MonkeVoteMachine.PollEntry _currentPoll;

	// Token: 0x04000956 RID: 2390
	private MonkeVoteMachine.PollEntry _previousPoll;

	// Token: 0x04000957 RID: 2391
	private DateTime _nextPollUpdate;

	// Token: 0x04000958 RID: 2392
	private bool _isTestingPoll;

	// Token: 0x02000128 RID: 296
	public enum VotingState
	{
		// Token: 0x0400095A RID: 2394
		None,
		// Token: 0x0400095B RID: 2395
		Voting,
		// Token: 0x0400095C RID: 2396
		Predicting,
		// Token: 0x0400095D RID: 2397
		Complete
	}

	// Token: 0x02000129 RID: 297
	public class PollEntry
	{
		// Token: 0x060007BD RID: 1981 RVA: 0x0002BD80 File Offset: 0x00029F80
		public PollEntry(int pollId, string question, string[] voteOptions)
		{
			this.PollId = pollId;
			this.Question = question;
			this.VoteOptions = voteOptions;
			this.VoteCount = new int[2];
			this.VoteCount[0] = Random.Range(0, 50000);
			this.VoteCount[1] = Random.Range(0, 50000);
			this.PredictionCount = new int[2];
			this.PredictionCount[0] = Random.Range(0, 50000);
			this.PredictionCount[1] = Random.Range(0, 50000);
			this.StartTime = DateTime.Now;
			this.EndTime = DateTime.Now + TimeSpan.FromSeconds(20.0);
		}

		// Token: 0x060007BE RID: 1982 RVA: 0x0002BE38 File Offset: 0x0002A038
		public PollEntry(MonkeVoteController.FetchPollsResponse poll)
		{
			this.PollId = poll.PollId;
			this.Question = poll.Question;
			this.VoteOptions = poll.VoteOptions.ToArray();
			this.VoteCount = poll.VoteCount.ToArray();
			this.PredictionCount = poll.PredictionCount.ToArray();
			this.StartTime = poll.StartTime;
			this.EndTime = poll.EndTime;
		}

		// Token: 0x060007BF RID: 1983 RVA: 0x0002BEB0 File Offset: 0x0002A0B0
		public int GetWinner()
		{
			if (this.VoteCount == null || this.VoteCount.Length == 0)
			{
				return -1;
			}
			int num = int.MinValue;
			int result = -1;
			for (int i = 0; i < this.VoteCount.Length; i++)
			{
				if (this.VoteCount[i] > num)
				{
					num = this.VoteCount[i];
					result = i;
				}
			}
			return result;
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x060007C0 RID: 1984 RVA: 0x0002BF04 File Offset: 0x0002A104
		public bool IsValid
		{
			get
			{
				string[] voteOptions = this.VoteOptions;
				return voteOptions != null && voteOptions.Length == 2;
			}
		}

		// Token: 0x0400095E RID: 2398
		public int PollId;

		// Token: 0x0400095F RID: 2399
		public string Question;

		// Token: 0x04000960 RID: 2400
		public string[] VoteOptions;

		// Token: 0x04000961 RID: 2401
		public int[] VoteCount;

		// Token: 0x04000962 RID: 2402
		public int[] PredictionCount;

		// Token: 0x04000963 RID: 2403
		public DateTime StartTime;

		// Token: 0x04000964 RID: 2404
		public DateTime EndTime;
	}
}
