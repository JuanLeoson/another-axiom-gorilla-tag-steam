using System;
using System.Collections.Generic;
using System.Text;
using GorillaLocomotion;
using GorillaNetworking;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000AAD RID: 2733
public class DebugHudStats : MonoBehaviour
{
	// Token: 0x1700064C RID: 1612
	// (get) Token: 0x06004228 RID: 16936 RVA: 0x0014D0B2 File Offset: 0x0014B2B2
	public static DebugHudStats Instance
	{
		get
		{
			return DebugHudStats._instance;
		}
	}

	// Token: 0x06004229 RID: 16937 RVA: 0x0014D0B9 File Offset: 0x0014B2B9
	private void Awake()
	{
		if (DebugHudStats._instance != null && DebugHudStats._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			DebugHudStats._instance = this;
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600422A RID: 16938 RVA: 0x0014D0F4 File Offset: 0x0014B2F4
	private void OnDestroy()
	{
		if (DebugHudStats._instance == this)
		{
			DebugHudStats._instance = null;
		}
	}

	// Token: 0x0600422B RID: 16939 RVA: 0x0014D10C File Offset: 0x0014B30C
	private void Update()
	{
		bool flag = ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
		if (flag != this.buttonDown)
		{
			this.buttonDown = flag;
			if (!this.buttonDown)
			{
				switch (this.currentState)
				{
				case DebugHudStats.State.ShowStats:
					PlayerGameEvents.OnPlayerMoved += this.OnPlayerMoved;
					PlayerGameEvents.OnPlayerSwam += this.OnPlayerSwam;
					break;
				}
				switch (this.currentState)
				{
				case DebugHudStats.State.Inactive:
					this.currentState = DebugHudStats.State.Active;
					this.text.gameObject.SetActive(true);
					break;
				case DebugHudStats.State.Active:
					this.currentState = DebugHudStats.State.ShowLog;
					break;
				case DebugHudStats.State.ShowLog:
					this.currentState = DebugHudStats.State.ShowStats;
					this.distanceMoved = (this.distanceSwam = 0f);
					PlayerGameEvents.OnPlayerMoved += this.OnPlayerMoved;
					PlayerGameEvents.OnPlayerSwam += this.OnPlayerSwam;
					break;
				case DebugHudStats.State.ShowStats:
					this.currentState = DebugHudStats.State.Inactive;
					this.text.gameObject.SetActive(false);
					break;
				}
			}
		}
		if (this.firstAwake == 0f)
		{
			this.firstAwake = Time.time;
		}
		if (this.updateTimer < this.delayUpdateRate)
		{
			this.updateTimer += Time.deltaTime;
			return;
		}
		int num = Mathf.RoundToInt(1f / Time.smoothDeltaTime);
		if (num < 89)
		{
			this.lowFps++;
		}
		else
		{
			this.lowFps = 0;
		}
		this.fpsWarning.gameObject.SetActive(this.lowFps > 5 && this.currentState == DebugHudStats.State.Inactive);
		if (this.currentState != DebugHudStats.State.Inactive)
		{
			this.builder.Clear();
			this.builder.Append("v: ");
			this.builder.Append(GorillaComputer.instance.version);
			this.builder.Append(":");
			this.builder.Append(GorillaComputer.instance.buildCode);
			num = Mathf.Min(num, 90);
			this.builder.Append((num < 89) ? " - <color=\"red\">" : " - <color=\"white\">");
			this.builder.Append(num);
			this.builder.AppendLine(" fps</color>");
			if (GorillaComputer.instance != null)
			{
				this.builder.AppendLine(GorillaComputer.instance.GetServerTime().ToString());
			}
			else
			{
				this.builder.AppendLine("Server Time Unavailable");
			}
			GroupJoinZoneAB groupZone = GorillaTagger.Instance.offlineVRRig.zoneEntity.GroupZone;
			if (groupZone != this.lastGroupJoinZone)
			{
				this.zones = groupZone.ToString();
				this.lastGroupJoinZone = groupZone;
			}
			if (NetworkSystem.Instance.IsMasterClient)
			{
				this.builder.Append("H");
			}
			if (NetworkSystem.Instance.InRoom)
			{
				if (NetworkSystem.Instance.SessionIsPrivate)
				{
					this.builder.Append("Pri ");
				}
				else
				{
					this.builder.Append("Pub ");
				}
			}
			else
			{
				this.builder.Append("DC ");
			}
			this.builder.Append("Z: <color=\"orange\">");
			this.builder.Append(this.zones);
			this.builder.AppendLine("</color>");
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaGameManager instance = GorillaGameManager.instance;
				if (instance != null)
				{
					GorillaTagCompetitiveManager gorillaTagCompetitiveManager = instance as GorillaTagCompetitiveManager;
					if (gorillaTagCompetitiveManager != null)
					{
						this.builder.Append("Ranked Mode ELO: ");
						this.builder.Append(gorillaTagCompetitiveManager.GetScoring().Progression.GetEloScore().ToString());
						this.builder.Append("  Tier: ");
						this.builder.AppendLine(gorillaTagCompetitiveManager.GetScoring().Progression.GetRankedProgressionTierName());
						RankedMultiplayerScore.PlayerScoreInRound inGameScoreForSelf = gorillaTagCompetitiveManager.GetScoring().GetInGameScoreForSelf();
						this.builder.Append("Tags: ");
						this.builder.Append(inGameScoreForSelf.NumTags.ToString());
						this.builder.Append("  Defense: ");
						this.builder.Append(Mathf.RoundToInt(inGameScoreForSelf.PointsOnDefense).ToString());
						this.builder.Append("  Score: ");
						this.builder.AppendLine(Mathf.RoundToInt(gorillaTagCompetitiveManager.GetScoring().ComputeGameScore(inGameScoreForSelf.NumTags, inGameScoreForSelf.PointsOnDefense)).ToString());
						if (gorillaTagCompetitiveManager.ShowDebugPing)
						{
							this.builder.AppendLine("Server MatchID Ping!");
						}
					}
				}
			}
			if (this.currentState == DebugHudStats.State.ShowStats)
			{
				this.builder.AppendLine();
				Vector3 vector = GTPlayer.Instance.AveragedVelocity;
				Vector3 headCenterPosition = GTPlayer.Instance.HeadCenterPosition;
				float magnitude = vector.magnitude;
				this.groundVelocity = vector;
				this.groundVelocity.y = 0f;
				this.builder.AppendLine(string.Format("v: {0:F1} m/s", magnitude));
				this.builder.AppendLine(string.Format("ground: {0:F1} m/s", this.groundVelocity.magnitude));
				this.builder.AppendLine(string.Format("head: {0:F2}\n", headCenterPosition));
				this.builder.AppendLine(string.Format("odo: {0:F2}m", this.distanceMoved));
				this.builder.AppendLine(string.Format("swam: {0:F2}m", this.distanceSwam));
			}
			else if (this.currentState == DebugHudStats.State.ShowLog)
			{
				this.builder.AppendLine();
				for (int i = 0; i < this.logMessages.Count; i++)
				{
					this.builder.AppendLine(this.logMessages[i]);
				}
			}
			this.text.text = this.builder.ToString();
		}
		this.updateTimer = 0f;
	}

	// Token: 0x0600422C RID: 16940 RVA: 0x0014D736 File Offset: 0x0014B936
	private void OnPlayerSwam(float distance, float speed)
	{
		if (distance > 0.005f)
		{
			this.distanceSwam += distance;
		}
	}

	// Token: 0x0600422D RID: 16941 RVA: 0x0014D74E File Offset: 0x0014B94E
	private void OnPlayerMoved(float distance, float speed)
	{
		if (distance > 0.005f)
		{
			this.distanceMoved += distance;
		}
	}

	// Token: 0x0600422E RID: 16942 RVA: 0x0014D766 File Offset: 0x0014B966
	private void OnEnable()
	{
		Application.logMessageReceived += this.LogMessageReceived;
	}

	// Token: 0x0600422F RID: 16943 RVA: 0x0014D779 File Offset: 0x0014B979
	private void OnDisable()
	{
		Application.logMessageReceived -= this.LogMessageReceived;
	}

	// Token: 0x06004230 RID: 16944 RVA: 0x0014D78C File Offset: 0x0014B98C
	private void LogMessageReceived(string condition, string stackTrace, LogType type)
	{
		this.logMessages.Add(this.getColorStringFromLogType(type) + condition + "</color>");
		if (this.logMessages.Count > 6)
		{
			this.logMessages.RemoveAt(0);
		}
	}

	// Token: 0x06004231 RID: 16945 RVA: 0x0014D7C5 File Offset: 0x0014B9C5
	private string getColorStringFromLogType(LogType type)
	{
		switch (type)
		{
		case LogType.Error:
		case LogType.Assert:
		case LogType.Exception:
			return "<color=\"red\">";
		case LogType.Warning:
			return "<color=\"yellow\">";
		}
		return "<color=\"white\">";
	}

	// Token: 0x06004232 RID: 16946 RVA: 0x0014D7F4 File Offset: 0x0014B9F4
	private void OnZoneChanged(ZoneData[] zoneData)
	{
		this.zones = string.Empty;
		for (int i = 0; i < zoneData.Length; i++)
		{
			if (zoneData[i].active)
			{
				this.zones = this.zones + zoneData[i].zone.ToString().ToUpper() + "; ";
			}
		}
	}

	// Token: 0x04004D66 RID: 19814
	private const int FPS_THRESHOLD = 89;

	// Token: 0x04004D67 RID: 19815
	private static DebugHudStats _instance;

	// Token: 0x04004D68 RID: 19816
	[SerializeField]
	public TMP_Text text;

	// Token: 0x04004D69 RID: 19817
	[SerializeField]
	private TMP_Text fpsWarning;

	// Token: 0x04004D6A RID: 19818
	[SerializeField]
	private float delayUpdateRate = 0.25f;

	// Token: 0x04004D6B RID: 19819
	private float updateTimer;

	// Token: 0x04004D6C RID: 19820
	public float sessionAnytrackingLost;

	// Token: 0x04004D6D RID: 19821
	public float last30SecondsTrackingLost;

	// Token: 0x04004D6E RID: 19822
	private float firstAwake;

	// Token: 0x04004D6F RID: 19823
	private bool leftHandTracked;

	// Token: 0x04004D70 RID: 19824
	private bool rightHandTracked;

	// Token: 0x04004D71 RID: 19825
	private StringBuilder builder;

	// Token: 0x04004D72 RID: 19826
	private Vector3 averagedVelocity;

	// Token: 0x04004D73 RID: 19827
	private Vector3 groundVelocity;

	// Token: 0x04004D74 RID: 19828
	private Vector3 centerHeadPos;

	// Token: 0x04004D75 RID: 19829
	private float distanceMoved;

	// Token: 0x04004D76 RID: 19830
	private float distanceSwam;

	// Token: 0x04004D77 RID: 19831
	private List<string> logMessages = new List<string>();

	// Token: 0x04004D78 RID: 19832
	private bool buttonDown;

	// Token: 0x04004D79 RID: 19833
	private bool showLog;

	// Token: 0x04004D7A RID: 19834
	private int lowFps;

	// Token: 0x04004D7B RID: 19835
	private string zones;

	// Token: 0x04004D7C RID: 19836
	private GroupJoinZoneAB lastGroupJoinZone;

	// Token: 0x04004D7D RID: 19837
	private DebugHudStats.State currentState = DebugHudStats.State.Active;

	// Token: 0x02000AAE RID: 2734
	private enum State
	{
		// Token: 0x04004D7F RID: 19839
		Inactive,
		// Token: 0x04004D80 RID: 19840
		Active,
		// Token: 0x04004D81 RID: 19841
		ShowLog,
		// Token: 0x04004D82 RID: 19842
		ShowStats
	}
}
