using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PlayFab;
using UnityEngine;

// Token: 0x0200014B RID: 331
public class RotatingQuestsManager : MonoBehaviour, ITickSystemTick
{
	// Token: 0x170000D2 RID: 210
	// (get) Token: 0x060008A1 RID: 2209 RVA: 0x0002EF4E File Offset: 0x0002D14E
	// (set) Token: 0x060008A2 RID: 2210 RVA: 0x0002EF56 File Offset: 0x0002D156
	public bool TickRunning { get; set; }

	// Token: 0x170000D3 RID: 211
	// (get) Token: 0x060008A3 RID: 2211 RVA: 0x0002EF5F File Offset: 0x0002D15F
	// (set) Token: 0x060008A4 RID: 2212 RVA: 0x0002EF67 File Offset: 0x0002D167
	public DateTime DailyQuestCountdown { get; private set; }

	// Token: 0x170000D4 RID: 212
	// (get) Token: 0x060008A5 RID: 2213 RVA: 0x0002EF70 File Offset: 0x0002D170
	// (set) Token: 0x060008A6 RID: 2214 RVA: 0x0002EF78 File Offset: 0x0002D178
	public DateTime WeeklyQuestCountdown { get; private set; }

	// Token: 0x060008A7 RID: 2215 RVA: 0x0002EF81 File Offset: 0x0002D181
	private void Start()
	{
		this._questAudio = base.GetComponent<AudioSource>();
		this.RequestQuestsFromTitleData();
	}

	// Token: 0x060008A8 RID: 2216 RVA: 0x0002EF95 File Offset: 0x0002D195
	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x060008A9 RID: 2217 RVA: 0x0002EF9D File Offset: 0x0002D19D
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x060008AA RID: 2218 RVA: 0x0002EFA5 File Offset: 0x0002D1A5
	public void Tick()
	{
		if (this.hasQuest && this.nextQuestUpdateTime < DateTime.UtcNow)
		{
			this.SetupQuests();
		}
	}

	// Token: 0x060008AB RID: 2219 RVA: 0x0002EFC8 File Offset: 0x0002D1C8
	private void ProcessAllQuests(Action<RotatingQuestsManager.RotatingQuest> action)
	{
		RotatingQuestsManager.<>c__DisplayClass30_0 CS$<>8__locals1;
		CS$<>8__locals1.action = action;
		RotatingQuestsManager.<ProcessAllQuests>g__ProcessAllQuestsInList|30_0(this.quests.DailyQuests, ref CS$<>8__locals1);
		RotatingQuestsManager.<ProcessAllQuests>g__ProcessAllQuestsInList|30_0(this.quests.WeeklyQuests, ref CS$<>8__locals1);
	}

	// Token: 0x060008AC RID: 2220 RVA: 0x0002F001 File Offset: 0x0002D201
	private void QuestLoadPostProcess(RotatingQuestsManager.RotatingQuest quest)
	{
		if (quest.requiredZones.Count == 1 && quest.requiredZones[0] == GTZone.none)
		{
			quest.requiredZones.Clear();
		}
	}

	// Token: 0x060008AD RID: 2221 RVA: 0x0002F02C File Offset: 0x0002D22C
	private void QuestSavePreProcess(RotatingQuestsManager.RotatingQuest quest)
	{
		if (quest.requiredZones.Count == 0)
		{
			quest.requiredZones.Add(GTZone.none);
		}
	}

	// Token: 0x060008AE RID: 2222 RVA: 0x0002F048 File Offset: 0x0002D248
	public void LoadTestQuestsFromFile()
	{
		TextAsset textAsset = Resources.Load<TextAsset>(this.localQuestPath);
		this.LoadQuestsFromJson(textAsset.text);
	}

	// Token: 0x060008AF RID: 2223 RVA: 0x0002F06D File Offset: 0x0002D26D
	public void RequestQuestsFromTitleData()
	{
		PlayFabTitleDataCache.Instance.GetTitleData("AllActiveQuests", delegate(string data)
		{
			this.LoadQuestsFromJson(data);
		}, delegate(PlayFabError e)
		{
			Debug.LogError(string.Format("Error getting AllActiveQuests data: {0}", e));
		});
	}

	// Token: 0x060008B0 RID: 2224 RVA: 0x0002F0AC File Offset: 0x0002D2AC
	private void LoadQuestsFromJson(string jsonString)
	{
		this.quests = JsonConvert.DeserializeObject<RotatingQuestsManager.RotatingQuestList>(jsonString);
		this.ProcessAllQuests(new Action<RotatingQuestsManager.RotatingQuest>(this.QuestLoadPostProcess));
		if (this.quests == null)
		{
			Debug.LogError("Error: Quests failed to parse!");
			return;
		}
		this.hasQuest = true;
		this.quests.Init();
		if (Application.isPlaying)
		{
			this.SetupQuests();
		}
	}

	// Token: 0x060008B1 RID: 2225 RVA: 0x0002F10C File Offset: 0x0002D30C
	private void SetupQuests()
	{
		this.ClearAllQuestEventListeners();
		this.SelectActiveQuests();
		this.LoadQuestProgress();
		this.HandleQuestProgressChanged(true);
		this.SetupAllQuestEventListeners();
		this.nextQuestUpdateTime = this.DailyQuestCountdown;
		this.nextQuestUpdateTime = this.nextQuestUpdateTime.AddMinutes(1.0);
	}

	// Token: 0x060008B2 RID: 2226 RVA: 0x0002F160 File Offset: 0x0002D360
	private void SelectActiveQuests()
	{
		DateTime dateTime = new DateTime(2025, 1, 10, 18, 0, 0, DateTimeKind.Utc);
		TimeSpan timeSpan = TimeSpan.FromHours(-8.0);
		DateTime dateStart = new DateTime(1, 1, 1, 0, 0, 0);
		DateTime dateEnd = new DateTime(2006, 12, 31, 0, 0, 0);
		TimeSpan daylightDelta = TimeSpan.FromHours(1.0);
		TimeZoneInfo.TransitionTime daylightTransitionStart = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 4, 1, DayOfWeek.Sunday);
		TimeZoneInfo.TransitionTime daylightTransitionEnd = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 10, 5, DayOfWeek.Sunday);
		DateTime dateStart2 = new DateTime(2007, 1, 1, 0, 0, 0);
		DateTime dateEnd2 = new DateTime(9999, 12, 31, 0, 0, 0);
		TimeSpan daylightDelta2 = TimeSpan.FromHours(1.0);
		TimeZoneInfo.TransitionTime daylightTransitionStart2 = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 3, 2, DayOfWeek.Sunday);
		TimeZoneInfo.TransitionTime daylightTransitionEnd2 = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 11, 1, DayOfWeek.Sunday);
		TimeZoneInfo timeZoneInfo = TimeZoneInfo.CreateCustomTimeZone("Pacific Standard Time", timeSpan, "Pacific Standard Time", "Pacific Standard Time", "Pacific Standard Time", new TimeZoneInfo.AdjustmentRule[]
		{
			TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(dateStart, dateEnd, daylightDelta, daylightTransitionStart, daylightTransitionEnd),
			TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(dateStart2, dateEnd2, daylightDelta2, daylightTransitionStart2, daylightTransitionEnd2)
		});
		if (timeZoneInfo != null && timeZoneInfo.IsDaylightSavingTime(DateTime.UtcNow - timeSpan))
		{
			dateTime -= TimeSpan.FromHours(1.0);
		}
		TimeSpan timeSpan2 = DateTime.UtcNow - dateTime;
		this.RemoveDisabledQuests();
		int days = timeSpan2.Days;
		this.dailyQuestSetID = days;
		this.weeklyQuestSetID = days / 7;
		RotatingQuestsManager.LastQuestDailyID = this.dailyQuestSetID;
		this.DailyQuestCountdown = dateTime + TimeSpan.FromDays((double)(this.dailyQuestSetID + 1));
		this.WeeklyQuestCountdown = dateTime + TimeSpan.FromDays((double)((this.weeklyQuestSetID + 1) * 7));
		Random.InitState(this.dailyQuestSetID);
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup in this.quests.DailyQuests)
		{
			int num = Math.Min(rotatingQuestGroup.selectCount, rotatingQuestGroup.quests.Count);
			float num2 = 0f;
			List<ValueTuple<int, float>> list = new List<ValueTuple<int, float>>(rotatingQuestGroup.quests.Count);
			for (int i = 0; i < rotatingQuestGroup.quests.Count; i++)
			{
				rotatingQuestGroup.quests[i].isQuestActive = false;
				num2 += rotatingQuestGroup.quests[i].weight;
				list.Add(new ValueTuple<int, float>(i, rotatingQuestGroup.quests[i].weight));
			}
			for (int j = 0; j < num; j++)
			{
				float num3 = Random.Range(0f, num2);
				for (int k = 0; k < list.Count; k++)
				{
					float item = list[k].Item2;
					if (num3 <= item || k == list.Count - 1)
					{
						num2 -= item;
						int item2 = list[k].Item1;
						list.RemoveAt(k);
						rotatingQuestGroup.quests[item2].isQuestActive = true;
						rotatingQuestGroup.quests[item2].SetRequiredZone();
						break;
					}
					num3 -= item;
				}
			}
		}
		Random.InitState(this.weeklyQuestSetID);
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup2 in this.quests.WeeklyQuests)
		{
			int num4 = Math.Min(rotatingQuestGroup2.selectCount, rotatingQuestGroup2.quests.Count);
			float num5 = 0f;
			List<ValueTuple<int, float>> list2 = new List<ValueTuple<int, float>>(rotatingQuestGroup2.quests.Count);
			for (int l = 0; l < rotatingQuestGroup2.quests.Count; l++)
			{
				rotatingQuestGroup2.quests[l].isQuestActive = false;
				num5 += rotatingQuestGroup2.quests[l].weight;
				list2.Add(new ValueTuple<int, float>(l, rotatingQuestGroup2.quests[l].weight));
			}
			for (int m = 0; m < num4; m++)
			{
				float num6 = Random.Range(0f, num5);
				for (int n = 0; n < list2.Count; n++)
				{
					float item3 = list2[n].Item2;
					if (num6 <= item3 || n == list2.Count - 1)
					{
						num5 -= item3;
						int item4 = list2[n].Item1;
						list2.RemoveAt(n);
						rotatingQuestGroup2.quests[item4].isQuestActive = true;
						rotatingQuestGroup2.quests[item4].SetRequiredZone();
						break;
					}
					num6 -= item3;
				}
			}
		}
		ProgressionController.ReportQuestSelectionChanged();
	}

	// Token: 0x060008B3 RID: 2227 RVA: 0x0002F694 File Offset: 0x0002D894
	private void RemoveDisabledQuests()
	{
		RotatingQuestsManager.<RemoveDisabledQuests>g__RemoveDisabledQuestsFromGroupList|38_0(this.quests.DailyQuests);
		RotatingQuestsManager.<RemoveDisabledQuests>g__RemoveDisabledQuestsFromGroupList|38_0(this.quests.WeeklyQuests);
	}

	// Token: 0x060008B4 RID: 2228 RVA: 0x0002F6B8 File Offset: 0x0002D8B8
	private void LoadQuestProgress()
	{
		int @int = PlayerPrefs.GetInt("Rotating_Quest_Daily_SetID_Key", -1);
		int int2 = PlayerPrefs.GetInt("Rotating_Quest_Daily_SaveCount_Key", -1);
		if (@int == this.dailyQuestSetID)
		{
			for (int i = 0; i < int2; i++)
			{
				int int3 = PlayerPrefs.GetInt(string.Format("{0}{1}", "Rotating_Quest_Daily_ID_Key", i), -1);
				int int4 = PlayerPrefs.GetInt(string.Format("{0}{1}", "Rotating_Quest_Daily_Progress_Key", i), -1);
				if (int3 != -1)
				{
					for (int j = 0; j < this.quests.DailyQuests.Count; j++)
					{
						for (int k = 0; k < this.quests.DailyQuests[j].quests.Count; k++)
						{
							RotatingQuestsManager.RotatingQuest rotatingQuest = this.quests.DailyQuests[j].quests[k];
							if (rotatingQuest.questID == int3)
							{
								rotatingQuest.ApplySavedProgress(int4);
								break;
							}
						}
					}
				}
			}
		}
		int int5 = PlayerPrefs.GetInt("Rotating_Quest_Weekly_SetID_Key", -1);
		int int6 = PlayerPrefs.GetInt("Rotating_Quest_Weekly_SaveCount_Key", -1);
		if (int5 == this.weeklyQuestSetID)
		{
			for (int l = 0; l < int6; l++)
			{
				int int7 = PlayerPrefs.GetInt(string.Format("{0}{1}", "Rotating_Quest_Weekly_ID_Key", l), -1);
				int int8 = PlayerPrefs.GetInt(string.Format("{0}{1}", "Rotating_Quest_Weekly_Progress_Key", l), -1);
				if (int7 != -1)
				{
					for (int m = 0; m < this.quests.WeeklyQuests.Count; m++)
					{
						for (int n = 0; n < this.quests.WeeklyQuests[m].quests.Count; n++)
						{
							RotatingQuestsManager.RotatingQuest rotatingQuest2 = this.quests.WeeklyQuests[m].quests[n];
							if (rotatingQuest2.questID == int7)
							{
								rotatingQuest2.ApplySavedProgress(int8);
								break;
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060008B5 RID: 2229 RVA: 0x0002F8B4 File Offset: 0x0002DAB4
	private void SaveQuestProgress()
	{
		int num = 0;
		for (int i = 0; i < this.quests.DailyQuests.Count; i++)
		{
			for (int j = 0; j < this.quests.DailyQuests[i].quests.Count; j++)
			{
				RotatingQuestsManager.RotatingQuest rotatingQuest = this.quests.DailyQuests[i].quests[j];
				int progress = rotatingQuest.GetProgress();
				if (progress > 0)
				{
					PlayerPrefs.SetInt(string.Format("{0}{1}", "Rotating_Quest_Daily_ID_Key", num), rotatingQuest.questID);
					PlayerPrefs.SetInt(string.Format("{0}{1}", "Rotating_Quest_Daily_Progress_Key", num), progress);
					num++;
				}
			}
		}
		if (num > 0)
		{
			PlayerPrefs.SetInt("Rotating_Quest_Daily_SetID_Key", this.dailyQuestSetID);
			PlayerPrefs.SetInt("Rotating_Quest_Daily_SaveCount_Key", num);
		}
		int num2 = 0;
		for (int k = 0; k < this.quests.WeeklyQuests.Count; k++)
		{
			for (int l = 0; l < this.quests.WeeklyQuests[k].quests.Count; l++)
			{
				RotatingQuestsManager.RotatingQuest rotatingQuest2 = this.quests.WeeklyQuests[k].quests[l];
				int progress2 = rotatingQuest2.GetProgress();
				if (progress2 > 0)
				{
					PlayerPrefs.SetInt(string.Format("{0}{1}", "Rotating_Quest_Weekly_ID_Key", num2), rotatingQuest2.questID);
					PlayerPrefs.SetInt(string.Format("{0}{1}", "Rotating_Quest_Weekly_Progress_Key", num2), progress2);
					num2++;
				}
			}
		}
		if (num2 > 0)
		{
			PlayerPrefs.SetInt("Rotating_Quest_Weekly_SetID_Key", this.weeklyQuestSetID);
			PlayerPrefs.SetInt("Rotating_Quest_Weekly_SaveCount_Key", num2);
		}
		PlayerPrefs.Save();
	}

	// Token: 0x060008B6 RID: 2230 RVA: 0x0002FA84 File Offset: 0x0002DC84
	private void SetupAllQuestEventListeners()
	{
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup in this.quests.DailyQuests)
		{
			foreach (RotatingQuestsManager.RotatingQuest rotatingQuest in rotatingQuestGroup.quests)
			{
				rotatingQuest.questManager = this;
				if (rotatingQuest.isQuestActive && !rotatingQuest.isQuestComplete)
				{
					rotatingQuest.AddEventListener();
				}
			}
		}
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup2 in this.quests.WeeklyQuests)
		{
			foreach (RotatingQuestsManager.RotatingQuest rotatingQuest2 in rotatingQuestGroup2.quests)
			{
				rotatingQuest2.questManager = this;
				if (rotatingQuest2.isQuestActive && !rotatingQuest2.isQuestComplete)
				{
					rotatingQuest2.AddEventListener();
				}
			}
		}
	}

	// Token: 0x060008B7 RID: 2231 RVA: 0x0002FBC4 File Offset: 0x0002DDC4
	private void ClearAllQuestEventListeners()
	{
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup in this.quests.DailyQuests)
		{
			foreach (RotatingQuestsManager.RotatingQuest rotatingQuest in rotatingQuestGroup.quests)
			{
				rotatingQuest.RemoveEventListener();
			}
		}
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup2 in this.quests.WeeklyQuests)
		{
			foreach (RotatingQuestsManager.RotatingQuest rotatingQuest2 in rotatingQuestGroup2.quests)
			{
				rotatingQuest2.RemoveEventListener();
			}
		}
	}

	// Token: 0x060008B8 RID: 2232 RVA: 0x0002FCD0 File Offset: 0x0002DED0
	private void HandleQuestCompleted(int questID)
	{
		RotatingQuestsManager.RotatingQuest quest = this.quests.GetQuest(questID);
		if (quest == null)
		{
			return;
		}
		ProgressionController.ReportQuestComplete(questID, quest.isDailyQuest);
		if (this._playQuestSounds)
		{
			AudioSource questAudio = this._questAudio;
			if (questAudio == null)
			{
				return;
			}
			questAudio.GTPlay();
		}
	}

	// Token: 0x060008B9 RID: 2233 RVA: 0x0002FD12 File Offset: 0x0002DF12
	private void HandleQuestProgressChanged(bool initialLoad)
	{
		if (!initialLoad)
		{
			this.SaveQuestProgress();
		}
		RotatingQuestsManager.LastQuestChange = Time.frameCount;
		ProgressionController.ReportQuestChanged(initialLoad);
	}

	// Token: 0x060008BB RID: 2235 RVA: 0x0002FD40 File Offset: 0x0002DF40
	[CompilerGenerated]
	internal static void <ProcessAllQuests>g__ProcessAllQuestsInList|30_0(List<RotatingQuestsManager.RotatingQuestGroup> questGroups, ref RotatingQuestsManager.<>c__DisplayClass30_0 A_1)
	{
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup in questGroups)
		{
			foreach (RotatingQuestsManager.RotatingQuest obj in rotatingQuestGroup.quests)
			{
				A_1.action(obj);
			}
		}
	}

	// Token: 0x060008BD RID: 2237 RVA: 0x0002FDD8 File Offset: 0x0002DFD8
	[CompilerGenerated]
	internal static void <RemoveDisabledQuests>g__RemoveDisabledQuestsFromGroupList|38_0(List<RotatingQuestsManager.RotatingQuestGroup> questList)
	{
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup in questList)
		{
			for (int i = rotatingQuestGroup.quests.Count - 1; i >= 0; i--)
			{
				if (rotatingQuestGroup.quests[i].disable)
				{
					rotatingQuestGroup.quests.RemoveAt(i);
				}
			}
		}
	}

	// Token: 0x04000A45 RID: 2629
	private bool hasQuest;

	// Token: 0x04000A46 RID: 2630
	[SerializeField]
	private bool useTestLocalQuests;

	// Token: 0x04000A47 RID: 2631
	[SerializeField]
	private string localQuestPath = "TestingRotatingQuests";

	// Token: 0x04000A48 RID: 2632
	public static int LastQuestChange;

	// Token: 0x04000A49 RID: 2633
	public static int LastQuestDailyID;

	// Token: 0x04000A4A RID: 2634
	public RotatingQuestsManager.RotatingQuestList quests;

	// Token: 0x04000A4B RID: 2635
	public int dailyQuestSetID;

	// Token: 0x04000A4C RID: 2636
	public int weeklyQuestSetID;

	// Token: 0x04000A4D RID: 2637
	[SerializeField]
	private bool _playQuestSounds;

	// Token: 0x04000A4E RID: 2638
	private AudioSource _questAudio;

	// Token: 0x04000A51 RID: 2641
	private DateTime nextQuestUpdateTime;

	// Token: 0x04000A52 RID: 2642
	private const string kDailyQuestSetIDKey = "Rotating_Quest_Daily_SetID_Key";

	// Token: 0x04000A53 RID: 2643
	private const string kDailyQuestSaveCountKey = "Rotating_Quest_Daily_SaveCount_Key";

	// Token: 0x04000A54 RID: 2644
	private const string kDailyQuestIDKey = "Rotating_Quest_Daily_ID_Key";

	// Token: 0x04000A55 RID: 2645
	private const string kDailyQuestProgressKey = "Rotating_Quest_Daily_Progress_Key";

	// Token: 0x04000A56 RID: 2646
	private const string kWeeklyQuestSetIDKey = "Rotating_Quest_Weekly_SetID_Key";

	// Token: 0x04000A57 RID: 2647
	private const string kWeeklyQuestSaveCountKey = "Rotating_Quest_Weekly_SaveCount_Key";

	// Token: 0x04000A58 RID: 2648
	private const string kWeeklyQuestIDKey = "Rotating_Quest_Weekly_ID_Key";

	// Token: 0x04000A59 RID: 2649
	private const string kWeeklyQuestProgressKey = "Rotating_Quest_Weekly_Progress_Key";

	// Token: 0x0200014C RID: 332
	[Serializable]
	public class RotatingQuest
	{
		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x060008BE RID: 2238 RVA: 0x0002FE58 File Offset: 0x0002E058
		[JsonIgnore]
		public bool IsMovementQuest
		{
			get
			{
				return this.questType == QuestType.moveDistance || this.questType == QuestType.swimDistance;
			}
		}

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x060008BF RID: 2239 RVA: 0x0002FE6F File Offset: 0x0002E06F
		// (set) Token: 0x060008C0 RID: 2240 RVA: 0x0002FE77 File Offset: 0x0002E077
		[JsonIgnore]
		public GTZone RequiredZone { get; private set; } = GTZone.none;

		// Token: 0x060008C1 RID: 2241 RVA: 0x0002FE80 File Offset: 0x0002E080
		public void SetRequiredZone()
		{
			this.RequiredZone = ((this.requiredZones.Count > 0) ? this.requiredZones[Random.Range(0, this.requiredZones.Count)] : GTZone.none);
		}

		// Token: 0x060008C2 RID: 2242 RVA: 0x0002FEB8 File Offset: 0x0002E0B8
		public void AddEventListener()
		{
			if (this.isQuestComplete)
			{
				return;
			}
			switch (this.questType)
			{
			case QuestType.gameModeObjective:
				PlayerGameEvents.OnGameModeObjectiveTrigger += this.OnGameEventOccurence;
				return;
			case QuestType.gameModeRound:
				PlayerGameEvents.OnGameModeCompleteRound += this.OnGameEventOccurence;
				return;
			case QuestType.grabObject:
				PlayerGameEvents.OnGrabbedObject += this.OnGameEventOccurence;
				return;
			case QuestType.dropObject:
				PlayerGameEvents.OnDroppedObject += this.OnGameEventOccurence;
				return;
			case QuestType.eatObject:
				PlayerGameEvents.OnEatObject += this.OnGameEventOccurence;
				return;
			case QuestType.tapObject:
				PlayerGameEvents.OnTapObject += this.OnGameEventOccurence;
				return;
			case QuestType.launchedProjectile:
				PlayerGameEvents.OnLaunchedProjectile += this.OnGameEventOccurence;
				return;
			case QuestType.moveDistance:
				PlayerGameEvents.OnPlayerMoved += this.OnGameMoveEvent;
				return;
			case QuestType.swimDistance:
				PlayerGameEvents.OnPlayerSwam += this.OnGameMoveEvent;
				return;
			case QuestType.triggerHandEffect:
				PlayerGameEvents.OnTriggerHandEffect += this.OnGameEventOccurence;
				return;
			case QuestType.enterLocation:
				PlayerGameEvents.OnEnterLocation += this.OnGameEventOccurence;
				return;
			case QuestType.misc:
				PlayerGameEvents.OnMiscEvent += this.OnGameEventOccurence;
				return;
			case QuestType.critter:
				PlayerGameEvents.OnCritterEvent += this.OnGameEventOccurence;
				return;
			default:
				return;
			}
		}

		// Token: 0x060008C3 RID: 2243 RVA: 0x0002FFFC File Offset: 0x0002E1FC
		public void RemoveEventListener()
		{
			switch (this.questType)
			{
			case QuestType.gameModeObjective:
				PlayerGameEvents.OnGameModeObjectiveTrigger -= this.OnGameEventOccurence;
				return;
			case QuestType.gameModeRound:
				PlayerGameEvents.OnGameModeCompleteRound -= this.OnGameEventOccurence;
				return;
			case QuestType.grabObject:
				PlayerGameEvents.OnGrabbedObject -= this.OnGameEventOccurence;
				return;
			case QuestType.dropObject:
				PlayerGameEvents.OnDroppedObject -= this.OnGameEventOccurence;
				return;
			case QuestType.eatObject:
				PlayerGameEvents.OnEatObject -= this.OnGameEventOccurence;
				return;
			case QuestType.tapObject:
				PlayerGameEvents.OnTapObject -= this.OnGameEventOccurence;
				return;
			case QuestType.launchedProjectile:
				PlayerGameEvents.OnLaunchedProjectile -= this.OnGameEventOccurence;
				return;
			case QuestType.moveDistance:
				PlayerGameEvents.OnPlayerMoved -= this.OnGameMoveEvent;
				return;
			case QuestType.swimDistance:
				PlayerGameEvents.OnPlayerSwam -= this.OnGameMoveEvent;
				return;
			case QuestType.triggerHandEffect:
				PlayerGameEvents.OnTriggerHandEffect -= this.OnGameEventOccurence;
				return;
			case QuestType.enterLocation:
				PlayerGameEvents.OnEnterLocation -= this.OnGameEventOccurence;
				return;
			case QuestType.misc:
				PlayerGameEvents.OnMiscEvent -= this.OnGameEventOccurence;
				return;
			case QuestType.critter:
				PlayerGameEvents.OnCritterEvent -= this.OnGameEventOccurence;
				return;
			default:
				return;
			}
		}

		// Token: 0x060008C4 RID: 2244 RVA: 0x00030138 File Offset: 0x0002E338
		public void ApplySavedProgress(int progress)
		{
			if (this.questType == QuestType.moveDistance || this.questType == QuestType.swimDistance)
			{
				this.moveDistance = (float)progress;
				this.occurenceCount = Mathf.FloorToInt(this.moveDistance);
				this.isQuestComplete = (this.occurenceCount >= this.requiredOccurenceCount);
				return;
			}
			this.occurenceCount = progress;
			this.isQuestComplete = (this.occurenceCount >= this.requiredOccurenceCount);
		}

		// Token: 0x060008C5 RID: 2245 RVA: 0x000301A7 File Offset: 0x0002E3A7
		public int GetProgress()
		{
			if (this.questType == QuestType.moveDistance || this.questType == QuestType.swimDistance)
			{
				return Mathf.FloorToInt(this.moveDistance);
			}
			return this.occurenceCount;
		}

		// Token: 0x060008C6 RID: 2246 RVA: 0x000301CE File Offset: 0x0002E3CE
		private void OnGameEventOccurence(string eventName)
		{
			this.OnGameEventOccurence(eventName, 1);
		}

		// Token: 0x060008C7 RID: 2247 RVA: 0x000301D8 File Offset: 0x0002E3D8
		private void OnGameEventOccurence(string eventName, int count)
		{
			if (this.RequiredZone != GTZone.none && !ZoneManagement.IsInZone(this.RequiredZone))
			{
				return;
			}
			string.IsNullOrEmpty(this.questOccurenceFilter);
			if (eventName.StartsWith(this.questOccurenceFilter))
			{
				this.SetProgress(this.occurenceCount + count);
			}
		}

		// Token: 0x060008C8 RID: 2248 RVA: 0x00030225 File Offset: 0x0002E425
		private void OnGameMoveEvent(float distance, float speed)
		{
			if (this.RequiredZone != GTZone.none && !ZoneManagement.IsInZone(this.RequiredZone))
			{
				return;
			}
			this.moveDistance += distance;
			this.SetProgress(Mathf.FloorToInt(this.moveDistance));
		}

		// Token: 0x060008C9 RID: 2249 RVA: 0x00030260 File Offset: 0x0002E460
		private void SetProgress(int progress)
		{
			if (this.isQuestComplete)
			{
				return;
			}
			if (this.occurenceCount == progress)
			{
				return;
			}
			this.lastChange = Time.frameCount;
			this.occurenceCount = progress;
			if (this.occurenceCount >= this.requiredOccurenceCount)
			{
				this.Complete();
			}
			this.questManager.HandleQuestProgressChanged(false);
		}

		// Token: 0x060008CA RID: 2250 RVA: 0x000302B2 File Offset: 0x0002E4B2
		private void Complete()
		{
			if (this.isQuestComplete)
			{
				return;
			}
			this.isQuestComplete = true;
			this.RemoveEventListener();
			this.questManager.HandleQuestCompleted(this.questID);
		}

		// Token: 0x060008CB RID: 2251 RVA: 0x000302DB File Offset: 0x0002E4DB
		public string GetTextDescription()
		{
			return this.<GetTextDescription>g__GetActionName|31_0().ToUpper() + this.<GetTextDescription>g__GetLocationText|31_1().ToUpper();
		}

		// Token: 0x060008CC RID: 2252 RVA: 0x000302F8 File Offset: 0x0002E4F8
		public string GetProgressText()
		{
			if (!this.isQuestComplete)
			{
				return string.Format("{0}/{1}", this.occurenceCount, this.requiredOccurenceCount);
			}
			return "[DONE]";
		}

		// Token: 0x060008CE RID: 2254 RVA: 0x00030358 File Offset: 0x0002E558
		[CompilerGenerated]
		private string <GetTextDescription>g__GetActionName|31_0()
		{
			switch (this.questType)
			{
			case QuestType.none:
				return "[UNDEFINED]";
			case QuestType.gameModeObjective:
				return this.questName;
			case QuestType.gameModeRound:
				return this.questName;
			case QuestType.grabObject:
				return this.questName;
			case QuestType.dropObject:
				return this.questName;
			case QuestType.eatObject:
				return this.questName;
			case QuestType.launchedProjectile:
				return this.questName;
			case QuestType.moveDistance:
				return this.questName;
			case QuestType.swimDistance:
				return this.questName;
			case QuestType.triggerHandEffect:
				return this.questName;
			case QuestType.enterLocation:
				return this.questName;
			case QuestType.misc:
				return this.questName;
			}
			return this.questName;
		}

		// Token: 0x060008CF RID: 2255 RVA: 0x0003041B File Offset: 0x0002E61B
		[CompilerGenerated]
		private string <GetTextDescription>g__GetLocationText|31_1()
		{
			if (this.RequiredZone == GTZone.none)
			{
				return "";
			}
			return string.Format(" IN {0}", this.RequiredZone);
		}

		// Token: 0x04000A5A RID: 2650
		public bool disable;

		// Token: 0x04000A5B RID: 2651
		public int questID;

		// Token: 0x04000A5C RID: 2652
		public float weight = 1f;

		// Token: 0x04000A5D RID: 2653
		public string questName = "UNNAMED QUEST";

		// Token: 0x04000A5E RID: 2654
		public QuestType questType;

		// Token: 0x04000A5F RID: 2655
		public string questOccurenceFilter;

		// Token: 0x04000A60 RID: 2656
		public int requiredOccurenceCount = 1;

		// Token: 0x04000A61 RID: 2657
		[JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
		public List<GTZone> requiredZones;

		// Token: 0x04000A62 RID: 2658
		[Space]
		[NonSerialized]
		public bool isQuestActive;

		// Token: 0x04000A63 RID: 2659
		[NonSerialized]
		public bool isQuestComplete;

		// Token: 0x04000A64 RID: 2660
		[NonSerialized]
		public bool isDailyQuest;

		// Token: 0x04000A65 RID: 2661
		[NonSerialized]
		public int lastChange;

		// Token: 0x04000A67 RID: 2663
		[NonSerialized]
		public int occurenceCount;

		// Token: 0x04000A68 RID: 2664
		private float moveDistance;

		// Token: 0x04000A69 RID: 2665
		[NonSerialized]
		public RotatingQuestsManager questManager;
	}

	// Token: 0x0200014D RID: 333
	[Serializable]
	public class RotatingQuestGroup
	{
		// Token: 0x04000A6A RID: 2666
		public int selectCount;

		// Token: 0x04000A6B RID: 2667
		public string name;

		// Token: 0x04000A6C RID: 2668
		public List<RotatingQuestsManager.RotatingQuest> quests;
	}

	// Token: 0x0200014E RID: 334
	[Serializable]
	public class RotatingQuestList
	{
		// Token: 0x060008D1 RID: 2257 RVA: 0x00030442 File Offset: 0x0002E642
		public void Init()
		{
			RotatingQuestsManager.RotatingQuestList.<Init>g__SetIsDaily|2_0(this.DailyQuests, true);
			RotatingQuestsManager.RotatingQuestList.<Init>g__SetIsDaily|2_0(this.WeeklyQuests, false);
		}

		// Token: 0x060008D2 RID: 2258 RVA: 0x0003045C File Offset: 0x0002E65C
		public RotatingQuestsManager.RotatingQuest GetQuest(int questID)
		{
			RotatingQuestsManager.RotatingQuestList.<>c__DisplayClass3_0 CS$<>8__locals1;
			CS$<>8__locals1.questID = questID;
			RotatingQuestsManager.RotatingQuest rotatingQuest = RotatingQuestsManager.RotatingQuestList.<GetQuest>g__GetQuestFrom|3_0(this.DailyQuests, ref CS$<>8__locals1);
			if (rotatingQuest == null)
			{
				rotatingQuest = RotatingQuestsManager.RotatingQuestList.<GetQuest>g__GetQuestFrom|3_0(this.WeeklyQuests, ref CS$<>8__locals1);
			}
			return rotatingQuest;
		}

		// Token: 0x060008D4 RID: 2260 RVA: 0x00030494 File Offset: 0x0002E694
		[CompilerGenerated]
		internal static void <Init>g__SetIsDaily|2_0(List<RotatingQuestsManager.RotatingQuestGroup> questList, bool isDaily)
		{
			foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup in questList)
			{
				foreach (RotatingQuestsManager.RotatingQuest rotatingQuest in rotatingQuestGroup.quests)
				{
					rotatingQuest.isDailyQuest = isDaily;
				}
			}
		}

		// Token: 0x060008D5 RID: 2261 RVA: 0x0003051C File Offset: 0x0002E71C
		[CompilerGenerated]
		internal static RotatingQuestsManager.RotatingQuest <GetQuest>g__GetQuestFrom|3_0(List<RotatingQuestsManager.RotatingQuestGroup> list, ref RotatingQuestsManager.RotatingQuestList.<>c__DisplayClass3_0 A_1)
		{
			foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup in list)
			{
				foreach (RotatingQuestsManager.RotatingQuest rotatingQuest in rotatingQuestGroup.quests)
				{
					if (rotatingQuest.questID == A_1.questID)
					{
						return rotatingQuest;
					}
				}
			}
			return null;
		}

		// Token: 0x04000A6D RID: 2669
		public List<RotatingQuestsManager.RotatingQuestGroup> DailyQuests;

		// Token: 0x04000A6E RID: 2670
		public List<RotatingQuestsManager.RotatingQuestGroup> WeeklyQuests;
	}
}
