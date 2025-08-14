using System;
using System.Collections.Generic;
using GorillaNetworking;
using GorillaTag;
using UnityEngine;

// Token: 0x020007E9 RID: 2025
public class GorillaScoreboardTotalUpdater : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060032AD RID: 12973 RVA: 0x00107E84 File Offset: 0x00106084
	public void UpdateLineState(GorillaPlayerScoreboardLine line)
	{
		if (line.playerActorNumber == -1)
		{
			return;
		}
		if (this.reportDict.ContainsKey(line.playerActorNumber))
		{
			this.reportDict[line.playerActorNumber] = new GorillaScoreboardTotalUpdater.PlayerReports(this.reportDict[line.playerActorNumber], line);
			return;
		}
		this.reportDict.Add(line.playerActorNumber, new GorillaScoreboardTotalUpdater.PlayerReports(line));
	}

	// Token: 0x060032AE RID: 12974 RVA: 0x00107EEE File Offset: 0x001060EE
	protected void Awake()
	{
		if (GorillaScoreboardTotalUpdater.hasInstance && GorillaScoreboardTotalUpdater.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		GorillaScoreboardTotalUpdater.SetInstance(this);
	}

	// Token: 0x060032AF RID: 12975 RVA: 0x00107F14 File Offset: 0x00106114
	private void Start()
	{
		RoomSystem.JoinedRoomEvent += new Action(this.JoinedRoom);
		RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
		RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.OnPlayerEnteredRoom);
		RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeftRoom);
	}

	// Token: 0x060032B0 RID: 12976 RVA: 0x00107F8D File Offset: 0x0010618D
	public static void CreateManager()
	{
		GorillaScoreboardTotalUpdater.SetInstance(new GameObject("GorillaScoreboardTotalUpdater").AddComponent<GorillaScoreboardTotalUpdater>());
	}

	// Token: 0x060032B1 RID: 12977 RVA: 0x00107FA3 File Offset: 0x001061A3
	private static void SetInstance(GorillaScoreboardTotalUpdater manager)
	{
		GorillaScoreboardTotalUpdater.instance = manager;
		GorillaScoreboardTotalUpdater.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x060032B2 RID: 12978 RVA: 0x00107FBE File Offset: 0x001061BE
	public static void RegisterSL(GorillaPlayerScoreboardLine sL)
	{
		if (!GorillaScoreboardTotalUpdater.hasInstance)
		{
			GorillaScoreboardTotalUpdater.CreateManager();
		}
		if (!GorillaScoreboardTotalUpdater.allScoreboardLines.Contains(sL))
		{
			GorillaScoreboardTotalUpdater.allScoreboardLines.Add(sL);
		}
	}

	// Token: 0x060032B3 RID: 12979 RVA: 0x00107FE4 File Offset: 0x001061E4
	public static void UnregisterSL(GorillaPlayerScoreboardLine sL)
	{
		if (!GorillaScoreboardTotalUpdater.hasInstance)
		{
			GorillaScoreboardTotalUpdater.CreateManager();
		}
		if (GorillaScoreboardTotalUpdater.allScoreboardLines.Contains(sL))
		{
			GorillaScoreboardTotalUpdater.allScoreboardLines.Remove(sL);
		}
	}

	// Token: 0x060032B4 RID: 12980 RVA: 0x0010800B File Offset: 0x0010620B
	public static void RegisterScoreboard(GorillaScoreBoard sB)
	{
		if (!GorillaScoreboardTotalUpdater.hasInstance)
		{
			GorillaScoreboardTotalUpdater.CreateManager();
		}
		if (!GorillaScoreboardTotalUpdater.allScoreboards.Contains(sB))
		{
			GorillaScoreboardTotalUpdater.allScoreboards.Add(sB);
			GorillaScoreboardTotalUpdater.instance.UpdateScoreboard(sB);
		}
	}

	// Token: 0x060032B5 RID: 12981 RVA: 0x0010803C File Offset: 0x0010623C
	public static void UnregisterScoreboard(GorillaScoreBoard sB)
	{
		if (!GorillaScoreboardTotalUpdater.hasInstance)
		{
			GorillaScoreboardTotalUpdater.CreateManager();
		}
		if (GorillaScoreboardTotalUpdater.allScoreboards.Contains(sB))
		{
			GorillaScoreboardTotalUpdater.allScoreboards.Remove(sB);
		}
	}

	// Token: 0x060032B6 RID: 12982 RVA: 0x00108064 File Offset: 0x00106264
	public void UpdateActiveScoreboards()
	{
		for (int i = 0; i < GorillaScoreboardTotalUpdater.allScoreboards.Count; i++)
		{
			this.UpdateScoreboard(GorillaScoreboardTotalUpdater.allScoreboards[i]);
		}
	}

	// Token: 0x060032B7 RID: 12983 RVA: 0x00108097 File Offset: 0x00106297
	public void SetOfflineFailureText(string failureText)
	{
		this.offlineTextErrorString = failureText;
		this.UpdateActiveScoreboards();
	}

	// Token: 0x060032B8 RID: 12984 RVA: 0x001080A6 File Offset: 0x001062A6
	public void ClearOfflineFailureText()
	{
		this.offlineTextErrorString = null;
		this.UpdateActiveScoreboards();
	}

	// Token: 0x060032B9 RID: 12985 RVA: 0x001080B8 File Offset: 0x001062B8
	public void UpdateScoreboard(GorillaScoreBoard sB)
	{
		sB.SetSleepState(this.joinedRoom);
		if (GorillaComputer.instance == null)
		{
			return;
		}
		if (!this.joinedRoom)
		{
			if (sB.notInRoomText != null)
			{
				sB.notInRoomText.gameObject.SetActive(true);
				sB.notInRoomText.text = ((this.offlineTextErrorString != null) ? this.offlineTextErrorString : GorillaComputer.instance.offlineTextInitialString);
			}
			for (int i = 0; i < sB.lines.Count; i++)
			{
				sB.lines[i].ResetData();
			}
			return;
		}
		if (sB.notInRoomText != null)
		{
			sB.notInRoomText.gameObject.SetActive(false);
		}
		for (int j = 0; j < sB.lines.Count; j++)
		{
			GorillaPlayerScoreboardLine gorillaPlayerScoreboardLine = sB.lines[j];
			if (j < this.playersInRoom.Count)
			{
				gorillaPlayerScoreboardLine.gameObject.SetActive(true);
				gorillaPlayerScoreboardLine.SetLineData(this.playersInRoom[j]);
			}
			else
			{
				gorillaPlayerScoreboardLine.ResetData();
				gorillaPlayerScoreboardLine.gameObject.SetActive(false);
			}
		}
		sB.RedrawPlayerLines();
	}

	// Token: 0x060032BA RID: 12986 RVA: 0x000172AD File Offset: 0x000154AD
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060032BB RID: 12987 RVA: 0x000172B6 File Offset: 0x000154B6
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060032BC RID: 12988 RVA: 0x001081E0 File Offset: 0x001063E0
	public void SliceUpdate()
	{
		if (GorillaScoreboardTotalUpdater.allScoreboardLines.Count == 0)
		{
			return;
		}
		for (int i = 0; i < GorillaScoreboardTotalUpdater.linesPerFrame; i++)
		{
			if (GorillaScoreboardTotalUpdater.lineIndex >= GorillaScoreboardTotalUpdater.allScoreboardLines.Count)
			{
				GorillaScoreboardTotalUpdater.lineIndex = 0;
			}
			GorillaScoreboardTotalUpdater.allScoreboardLines[GorillaScoreboardTotalUpdater.lineIndex].UpdateLine();
			GorillaScoreboardTotalUpdater.lineIndex++;
		}
		for (int j = 0; j < GorillaScoreboardTotalUpdater.allScoreboards.Count; j++)
		{
			if (GorillaScoreboardTotalUpdater.allScoreboards[j].IsDirty)
			{
				this.UpdateScoreboard(GorillaScoreboardTotalUpdater.allScoreboards[j]);
			}
		}
	}

	// Token: 0x060032BD RID: 12989 RVA: 0x00108279 File Offset: 0x00106479
	private void OnPlayerEnteredRoom(NetPlayer netPlayer)
	{
		if (netPlayer == null)
		{
			Debug.LogError("Null netplayer");
		}
		if (!this.playersInRoom.Contains(netPlayer))
		{
			this.playersInRoom.Add(netPlayer);
		}
		this.UpdateActiveScoreboards();
	}

	// Token: 0x060032BE RID: 12990 RVA: 0x001082A8 File Offset: 0x001064A8
	private void OnPlayerLeftRoom(NetPlayer netPlayer)
	{
		if (netPlayer == null)
		{
			Debug.LogError("Null netplayer");
		}
		this.playersInRoom.Remove(netPlayer);
		this.UpdateActiveScoreboards();
		ReportMuteTimer reportMuteTimer;
		if (GorillaScoreboardTotalUpdater.m_reportMuteTimerDict.TryGetValue(netPlayer.ActorNumber, out reportMuteTimer))
		{
			GorillaScoreboardTotalUpdater.m_reportMuteTimerDict.Remove(netPlayer.ActorNumber);
			GorillaScoreboardTotalUpdater.m_reportMuteTimerPool.Return(reportMuteTimer);
		}
	}

	// Token: 0x060032BF RID: 12991 RVA: 0x00108308 File Offset: 0x00106508
	internal void JoinedRoom()
	{
		this.joinedRoom = true;
		foreach (NetPlayer item in NetworkSystem.Instance.AllNetPlayers)
		{
			this.playersInRoom.Add(item);
		}
		this.playersInRoom.Sort((NetPlayer x, NetPlayer y) => x.ActorNumber.CompareTo(y.ActorNumber));
		foreach (GorillaScoreBoard sB in GorillaScoreboardTotalUpdater.allScoreboards)
		{
			this.UpdateScoreboard(sB);
		}
	}

	// Token: 0x060032C0 RID: 12992 RVA: 0x001083B8 File Offset: 0x001065B8
	private void OnLeftRoom()
	{
		this.joinedRoom = false;
		this.playersInRoom.Clear();
		this.reportDict.Clear();
		foreach (GorillaScoreBoard sB in GorillaScoreboardTotalUpdater.allScoreboards)
		{
			this.UpdateScoreboard(sB);
		}
		foreach (KeyValuePair<int, ReportMuteTimer> keyValuePair in GorillaScoreboardTotalUpdater.m_reportMuteTimerDict)
		{
			GorillaScoreboardTotalUpdater.m_reportMuteTimerPool.Return(keyValuePair.Value);
		}
		GorillaScoreboardTotalUpdater.m_reportMuteTimerDict.Clear();
	}

	// Token: 0x060032C1 RID: 12993 RVA: 0x0010847C File Offset: 0x0010667C
	public static void ReportMute(NetPlayer player, int muted)
	{
		ReportMuteTimer reportMuteTimer;
		if (GorillaScoreboardTotalUpdater.m_reportMuteTimerDict.TryGetValue(player.ActorNumber, out reportMuteTimer))
		{
			reportMuteTimer.Muted = muted;
			if (!reportMuteTimer.Running)
			{
				reportMuteTimer.Start();
			}
			return;
		}
		reportMuteTimer = GorillaScoreboardTotalUpdater.m_reportMuteTimerPool.Take();
		reportMuteTimer.SetReportData(player.UserId, player.NickName, muted);
		reportMuteTimer.coolDown = 5f;
		reportMuteTimer.Start();
		GorillaScoreboardTotalUpdater.m_reportMuteTimerDict[player.ActorNumber] = reportMuteTimer;
	}

	// Token: 0x04003F94 RID: 16276
	public static GorillaScoreboardTotalUpdater instance;

	// Token: 0x04003F95 RID: 16277
	[OnEnterPlay_Set(false)]
	public static bool hasInstance = false;

	// Token: 0x04003F96 RID: 16278
	public static List<GorillaPlayerScoreboardLine> allScoreboardLines = new List<GorillaPlayerScoreboardLine>();

	// Token: 0x04003F97 RID: 16279
	public static int lineIndex = 0;

	// Token: 0x04003F98 RID: 16280
	private static int linesPerFrame = 2;

	// Token: 0x04003F99 RID: 16281
	public static List<GorillaScoreBoard> allScoreboards = new List<GorillaScoreBoard>();

	// Token: 0x04003F9A RID: 16282
	public static int boardIndex = 0;

	// Token: 0x04003F9B RID: 16283
	private List<NetPlayer> playersInRoom = new List<NetPlayer>();

	// Token: 0x04003F9C RID: 16284
	private bool joinedRoom;

	// Token: 0x04003F9D RID: 16285
	private bool wasGameManagerNull;

	// Token: 0x04003F9E RID: 16286
	public bool forOverlay;

	// Token: 0x04003F9F RID: 16287
	public string offlineTextErrorString;

	// Token: 0x04003FA0 RID: 16288
	public Dictionary<int, GorillaScoreboardTotalUpdater.PlayerReports> reportDict = new Dictionary<int, GorillaScoreboardTotalUpdater.PlayerReports>();

	// Token: 0x04003FA1 RID: 16289
	private static readonly Dictionary<int, ReportMuteTimer> m_reportMuteTimerDict = new Dictionary<int, ReportMuteTimer>(10);

	// Token: 0x04003FA2 RID: 16290
	private static readonly ObjectPool<ReportMuteTimer> m_reportMuteTimerPool = new ObjectPool<ReportMuteTimer>(10);

	// Token: 0x020007EA RID: 2026
	public struct PlayerReports
	{
		// Token: 0x060032C4 RID: 12996 RVA: 0x00108568 File Offset: 0x00106768
		public PlayerReports(GorillaScoreboardTotalUpdater.PlayerReports reportToUpdate, GorillaPlayerScoreboardLine lineToUpdate)
		{
			this.cheating = (reportToUpdate.cheating || lineToUpdate.reportedCheating);
			this.toxicity = (reportToUpdate.toxicity || lineToUpdate.reportedToxicity);
			this.hateSpeech = (reportToUpdate.hateSpeech || lineToUpdate.reportedHateSpeech);
			this.pressedReport = lineToUpdate.reportInProgress;
		}

		// Token: 0x060032C5 RID: 12997 RVA: 0x001085C6 File Offset: 0x001067C6
		public PlayerReports(GorillaPlayerScoreboardLine lineToUpdate)
		{
			this.cheating = lineToUpdate.reportedCheating;
			this.toxicity = lineToUpdate.reportedToxicity;
			this.hateSpeech = lineToUpdate.reportedHateSpeech;
			this.pressedReport = lineToUpdate.reportInProgress;
		}

		// Token: 0x04003FA3 RID: 16291
		public bool cheating;

		// Token: 0x04003FA4 RID: 16292
		public bool toxicity;

		// Token: 0x04003FA5 RID: 16293
		public bool hateSpeech;

		// Token: 0x04003FA6 RID: 16294
		public bool pressedReport;
	}
}
