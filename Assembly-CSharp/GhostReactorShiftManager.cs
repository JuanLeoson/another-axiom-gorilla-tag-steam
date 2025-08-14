using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x020005E3 RID: 1507
public class GhostReactorShiftManager : MonoBehaviour
{
	// Token: 0x17000391 RID: 913
	// (get) Token: 0x06002511 RID: 9489 RVA: 0x000C78B1 File Offset: 0x000C5AB1
	public int ShiftTotalEarned
	{
		get
		{
			return this.shiftTotalEarned;
		}
	}

	// Token: 0x17000392 RID: 914
	// (get) Token: 0x06002512 RID: 9490 RVA: 0x000C78B9 File Offset: 0x000C5AB9
	public bool ShiftActive
	{
		get
		{
			return this.shiftStarted;
		}
	}

	// Token: 0x17000393 RID: 915
	// (get) Token: 0x06002513 RID: 9491 RVA: 0x000C78C1 File Offset: 0x000C5AC1
	public double ShiftStartNetworkTime
	{
		get
		{
			return this.shiftStartNetworkTime;
		}
	}

	// Token: 0x17000394 RID: 916
	// (get) Token: 0x06002514 RID: 9492 RVA: 0x000C78C9 File Offset: 0x000C5AC9
	public bool LocalPlayerInside
	{
		get
		{
			return this.localPlayerInside;
		}
	}

	// Token: 0x17000395 RID: 917
	// (get) Token: 0x06002515 RID: 9493 RVA: 0x000C78D1 File Offset: 0x000C5AD1
	public float TotalPlayTime
	{
		get
		{
			return this.totalPlayTime;
		}
	}

	// Token: 0x17000396 RID: 918
	// (get) Token: 0x06002516 RID: 9494 RVA: 0x000C78D9 File Offset: 0x000C5AD9
	public string ShiftId
	{
		get
		{
			return this.gameIdGuid;
		}
	}

	// Token: 0x06002517 RID: 9495 RVA: 0x000C78E1 File Offset: 0x000C5AE1
	public void Init(GhostReactorManager grManager)
	{
		this.grManager = grManager;
	}

	// Token: 0x06002518 RID: 9496 RVA: 0x000C78EC File Offset: 0x000C5AEC
	public void RefreshShiftStatsDisplay()
	{
		this.shiftStatsText.text = string.Concat(new string[]
		{
			"\n\n",
			this.shiftStats.GetShiftStat(GRShiftStatType.EnemyDeaths).ToString("D2"),
			"\n",
			this.shiftStats.GetShiftStat(GRShiftStatType.CoresCollected).ToString("D2"),
			"\n",
			this.shiftStats.GetShiftStat(GRShiftStatType.PlayerDeaths).ToString("D2")
		});
		this.depthDisplay.RefreshObjectives();
	}

	// Token: 0x06002519 RID: 9497 RVA: 0x000C7986 File Offset: 0x000C5B86
	public void StartShiftButtonPressed()
	{
		this.RequestShiftStart();
	}

	// Token: 0x0600251A RID: 9498 RVA: 0x000C798E File Offset: 0x000C5B8E
	public void RequestShiftStart()
	{
		this.grManager.RequestShiftStart();
	}

	// Token: 0x0600251B RID: 9499 RVA: 0x000C799B File Offset: 0x000C5B9B
	public void EndShift()
	{
		this.grManager.RequestShiftEnd();
	}

	// Token: 0x0600251C RID: 9500 RVA: 0x000C79A8 File Offset: 0x000C5BA8
	public void ClearEntities()
	{
		Debug.LogError("Need to re-implement whatever this was doing");
	}

	// Token: 0x0600251D RID: 9501 RVA: 0x000C79B4 File Offset: 0x000C5BB4
	public void RefreshShiftTimer()
	{
		if (this.shiftTimerText != null)
		{
			this.shiftTimerText.text = Mathf.FloorToInt(this.shiftDurationMinutes).ToString("D2") + ":00";
		}
	}

	// Token: 0x0600251E RID: 9502 RVA: 0x000C79FC File Offset: 0x000C5BFC
	public void OnShiftStarted(string gameId, double shiftStartTime, bool wasPlayerInAtStart)
	{
		this.gameIdGuid = gameId;
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		if (!this.shiftStarted && grplayer != null)
		{
			float num = (float)(PhotonNetwork.Time - shiftStartTime);
			grplayer.ResetTelemetryTracking(this.gameIdGuid, num);
			grplayer.SendShiftStartedTelemetry(num, wasPlayerInAtStart);
		}
		this.shiftStarted = true;
		this.shiftStartNetworkTime = shiftStartTime;
		this.frontGate.OpenGate();
		this.ringTransform.gameObject.SetActive(false);
		this.gateBlockerTransform.gameObject.SetActive(false);
		this.prevCountDownTotal = this.shiftDurationMinutes * 60f;
		this.shiftTotalEarned = -1;
		this.authorizedToDelveDeeper = false;
		this.ResetJoinTimes();
		this.reactor.RefreshScoreboards();
		this.reactor.RefreshDepth();
		this.isRoomClosed = false;
		if (grplayer != null)
		{
			grplayer.RefreshPlayerVisuals();
		}
	}

	// Token: 0x0600251F RID: 9503 RVA: 0x000C7AD8 File Offset: 0x000C5CD8
	public void OnShiftEnded(double shiftEndTime, bool isShiftActuallyEnding, ZoneClearReason zoneClearReason = ZoneClearReason.JoinZone)
	{
		if (this.shiftStarted)
		{
			GRPlayer component = VRRig.LocalRig.GetComponent<GRPlayer>();
			if (component != null)
			{
				component.SendShiftEndedTelemetry(isShiftActuallyEnding, (float)this.shiftStartNetworkTime, zoneClearReason);
			}
		}
		this.shiftStarted = false;
		this.shiftEndNetworkTime = shiftEndTime;
		this.RefreshShiftTimer();
		this.frontGate.CloseGate();
		this.ringTransform.gameObject.SetActive(false);
		this.TeleportLocalPlayerIfOutOfBounds();
		if (this.shiftEndNetworkTime > 0.0 && this.shiftStats.GetShiftStat(GRShiftStatType.EnemyDeaths) > this.shiftStats.GetShiftStat(GRShiftStatType.PlayerDeaths))
		{
			PlayerGameEvents.MiscEvent("GRShiftGoodKD", 1);
		}
		if (PhotonNetwork.InRoom && !NetworkSystem.Instance.SessionIsPrivate && this.grManager.IsAuthority())
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("ghostReactorShiftStarted", "false");
			PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
			this.isRoomClosed = false;
		}
	}

	// Token: 0x06002520 RID: 9504 RVA: 0x000C7BC8 File Offset: 0x000C5DC8
	private void Update()
	{
		double num = PhotonNetwork.Time - this.shiftStartNetworkTime;
		float num2 = 60f * this.shiftDurationMinutes - (float)num;
		if (this.grManager.IsAuthority())
		{
			this.AuthorityUpdate(num2);
		}
		num2 = Mathf.Clamp(num2, 0f, 60f * this.shiftDurationMinutes);
		this.SharedUpdate(num2);
		this.prevCountDownTotal = num2;
	}

	// Token: 0x06002521 RID: 9505 RVA: 0x000C7C2C File Offset: 0x000C5E2C
	private void AuthorityUpdate(float countDownTotal)
	{
		if (PhotonNetwork.InRoom && this.grManager.IsAuthority())
		{
			if (this.shiftStarted && !NetworkSystem.Instance.SessionIsPrivate && !this.isRoomClosed && 60f * this.shiftDurationMinutes - countDownTotal >= this.roomCloseTimeSeconds)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add("ghostReactorShiftStarted", "true");
				PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
				this.isRoomClosed = true;
			}
			if (this.shiftStarted && countDownTotal <= 0f)
			{
				this.grManager.RequestShiftEnd();
			}
		}
	}

	// Token: 0x06002522 RID: 9506 RVA: 0x000C7CC8 File Offset: 0x000C5EC8
	private void SharedUpdate(float countDownTotal)
	{
		if (this.shiftStarted)
		{
			if (this.debugFastForwarding)
			{
				float num = this.debugFastForwardRate * Time.deltaTime;
				this.shiftStartNetworkTime -= (double)num;
			}
			int num2 = Mathf.FloorToInt(countDownTotal / 60f);
			int num3 = Mathf.FloorToInt(countDownTotal % 60f);
			this.shiftTimerText.text = num2.ToString("D2") + ":" + num3.ToString("D2");
			for (int i = 0; i < this.warningClipPlayTimes.Count; i++)
			{
				if (countDownTotal < (float)this.warningClipPlayTimes[i] && this.prevCountDownTotal >= (float)this.warningClipPlayTimes[i])
				{
					this.warningAudioSource.PlayOneShot(this.warningAudio);
				}
			}
			if (this.localPlayerInside)
			{
				if (countDownTotal >= 0f && countDownTotal < this.ringClosingDuration * 60f)
				{
					this.ringTransform.gameObject.SetActive(true);
					float num4 = Mathf.Lerp(this.ringClosingMinRadius, this.ringClosingMaxRadius, countDownTotal / (this.ringClosingDuration * 60f));
					this.ringTransform.localScale = new Vector3(num4, 1f, num4);
					Vector3 vector = VRRig.LocalRig.bodyTransform.position - this.ringTransform.position;
					vector.y = 0f;
					if (vector.sqrMagnitude > num4 * num4)
					{
						this.TeleportLocalPlayerIfOutOfBounds();
						return;
					}
				}
			}
			else if (this.ringTransform.gameObject.activeSelf)
			{
				this.ringTransform.gameObject.SetActive(false);
				return;
			}
		}
		else if (!this.shiftStarted)
		{
			this.TeleportLocalPlayerIfOutOfBounds();
		}
	}

	// Token: 0x06002523 RID: 9507 RVA: 0x000C7E80 File Offset: 0x000C6080
	private void TeleportLocalPlayerIfOutOfBounds()
	{
		if (this.localPlayerInside || (this.localPlayerOverlapping && Vector3.Dot(GTPlayer.Instance.headCollider.transform.position - this.gatePlaneTransform.position, this.gatePlaneTransform.forward) < 0f))
		{
			this.grManager.ReportLocalPlayerHit();
			GRPlayer component = VRRig.LocalRig.GetComponent<GRPlayer>();
			component.ChangePlayerState(GRPlayer.GRPlayerState.Ghost, this.grManager);
			GTPlayer.Instance.TeleportTo(this.playerTeleportTransform, true, true);
			this.localPlayerInside = false;
			this.localPlayerOverlapping = false;
			component.caughtByAnomaly = true;
		}
	}

	// Token: 0x06002524 RID: 9508 RVA: 0x000C7F24 File Offset: 0x000C6124
	public void RevealJudgment(int evaluation)
	{
		if (evaluation <= 0)
		{
			this.shiftJugmentText.text = "DON'T QUIT YOUR DAY JOB.";
			return;
		}
		switch (evaluation)
		{
		case 1:
			this.shiftJugmentText.text = "YOU'RE LEARNING. GOOD.";
			return;
		case 2:
			this.shiftJugmentText.text = "YOU MIGHT EARN A PROMOTION.";
			return;
		case 3:
			this.shiftJugmentText.text = "YOU DID A MANAGER-TIER JOB.";
			return;
		case 4:
			this.shiftJugmentText.text = "NICE. YOU GET EXTRA SHIFTS.";
			return;
		default:
			this.shiftJugmentText.text = "YOU WORK FOR US NOW.";
			this.wrongStumpGoo.SetActive(true);
			return;
		}
	}

	// Token: 0x06002525 RID: 9509 RVA: 0x000C7FC0 File Offset: 0x000C61C0
	public void ResetJudgment()
	{
		this.shiftJugmentText.text = "";
		this.wrongStumpGoo.SetActive(false);
	}

	// Token: 0x06002526 RID: 9510 RVA: 0x000C7FE0 File Offset: 0x000C61E0
	public void ResetJoinTimes()
	{
		int count = this.reactor.vrRigs.Count;
		this.totalPlayTime = 0f;
		for (int i = 0; i < count; i++)
		{
			GRPlayer.Get(this.reactor.vrRigs[i]).shiftJoinTime = this.shiftStartNetworkTime;
		}
	}

	// Token: 0x06002527 RID: 9511 RVA: 0x000C8038 File Offset: 0x000C6238
	public void CalculatePlayerPercentages()
	{
		int count = this.reactor.vrRigs.Count;
		this.totalPlayTime = 0f;
		for (int i = 0; i < count; i++)
		{
			GRPlayer grplayer = GRPlayer.Get(this.reactor.vrRigs[i]);
			if (this.reactor.vrRigs[i] != null && grplayer != null)
			{
				if (this.reactor.vrRigs[i].OwningNetPlayer == null)
				{
					grplayer.ShiftPlayTime = 0.1f;
				}
				else if (this.shiftStarted)
				{
					grplayer.ShiftPlayTime = Mathf.Min(this.shiftDurationMinutes * 60f, (float)(PhotonNetwork.Time - grplayer.shiftJoinTime));
				}
				else
				{
					grplayer.ShiftPlayTime = Mathf.Min(this.shiftDurationMinutes * 60f, (float)(this.shiftEndNetworkTime - grplayer.shiftJoinTime));
				}
				this.totalPlayTime += grplayer.ShiftPlayTime;
			}
		}
	}

	// Token: 0x06002528 RID: 9512 RVA: 0x000C8140 File Offset: 0x000C6340
	public void CalculateShiftTotal()
	{
		this.shiftTotalEarned = 0;
		int count = this.reactor.vrRigs.Count;
		double num = 0.0;
		for (int i = 0; i < count; i++)
		{
			GRPlayer grplayer = GRPlayer.Get(this.reactor.vrRigs[i]);
			if (this.reactor.vrRigs[i] != null && grplayer != null)
			{
				this.shiftTotalEarned += grplayer.currency;
				grplayer.currency = 0;
				if (this.reactor.vrRigs[i].OwningNetPlayer == null)
				{
					grplayer.ShiftPlayTime = 0.1f;
				}
				else
				{
					grplayer.ShiftPlayTime = Mathf.Min(this.shiftDurationMinutes * 60f, (float)(PhotonNetwork.Time - grplayer.shiftJoinTime));
				}
				num += (double)grplayer.ShiftPlayTime;
			}
		}
		this.shiftTotalEarned = Mathf.Clamp(this.shiftTotalEarned, 0, this.shiftSanityMaximumEarned);
		num = (double)Mathf.Clamp((float)num, 0.1f, this.shiftDurationMinutes * 10f * 60f);
		for (int j = 0; j < count; j++)
		{
			GRPlayer grplayer2 = GRPlayer.Get(this.reactor.vrRigs[j]);
			if (this.reactor.vrRigs[j] != null && grplayer2 != null)
			{
				float num2 = 1f;
				if (grplayer2.ShiftPlayTime >= this.shiftDurationMinutes * 60f)
				{
					num2 = 1.2f;
				}
				grplayer2.LastShiftCut = Mathf.CeilToInt((float)((double)grplayer2.ShiftPlayTime / num * (double)this.shiftTotalEarned * (double)num2));
				grplayer2.CollectShiftCut();
			}
		}
		this.reactor.RefreshScoreboards();
		this.reactor.promotionBot.Refresh();
		this.reactor.RefreshDepth();
	}

	// Token: 0x06002529 RID: 9513 RVA: 0x000C8325 File Offset: 0x000C6525
	private void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			this.localPlayerOverlapping = true;
		}
	}

	// Token: 0x0600252A RID: 9514 RVA: 0x000C8340 File Offset: 0x000C6540
	private void OnTriggerExit(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			bool flag = Vector3.Dot(other.transform.position - this.gatePlaneTransform.position, this.gatePlaneTransform.forward) < 0f;
			this.localPlayerInside = flag;
			this.localPlayerOverlapping = false;
		}
	}

	// Token: 0x0600252B RID: 9515 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnButtonDelveDeeper()
	{
	}

	// Token: 0x0600252C RID: 9516 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnButtonDEBUGResetDepth()
	{
	}

	// Token: 0x0600252D RID: 9517 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnButtonDEBUGDelveDeeper()
	{
	}

	// Token: 0x0600252E RID: 9518 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnButtonDEBUGDelveShallower()
	{
	}

	// Token: 0x0600252F RID: 9519 RVA: 0x000023F5 File Offset: 0x000005F5
	public void RefreshDepthDisplay()
	{
	}

	// Token: 0x04002EB8 RID: 11960
	private const string EVENT_GOOD_KD = "GRShiftGoodKD";

	// Token: 0x04002EB9 RID: 11961
	[SerializeField]
	private GhostReactor reactor;

	// Token: 0x04002EBA RID: 11962
	[SerializeField]
	private GRMetalEnergyGate frontGate;

	// Token: 0x04002EBB RID: 11963
	[SerializeField]
	private TMP_Text shiftTimerText;

	// Token: 0x04002EBC RID: 11964
	[SerializeField]
	private TMP_Text shiftStatsText;

	// Token: 0x04002EBD RID: 11965
	[SerializeField]
	private TMP_Text shiftJugmentText;

	// Token: 0x04002EBE RID: 11966
	[SerializeField]
	private GameObject wrongStumpGoo;

	// Token: 0x04002EBF RID: 11967
	[SerializeField]
	private float shiftDurationMinutes = 20f;

	// Token: 0x04002EC0 RID: 11968
	[SerializeField]
	private Transform playerTeleportTransform;

	// Token: 0x04002EC1 RID: 11969
	[SerializeField]
	private Transform gatePlaneTransform;

	// Token: 0x04002EC2 RID: 11970
	[SerializeField]
	private Transform gateBlockerTransform;

	// Token: 0x04002EC3 RID: 11971
	[SerializeField]
	private float roomCloseTimeSeconds = 300f;

	// Token: 0x04002EC4 RID: 11972
	private bool isRoomClosed;

	// Token: 0x04002EC5 RID: 11973
	[Header("Audio")]
	[SerializeField]
	private AudioSource warningAudioSource;

	// Token: 0x04002EC6 RID: 11974
	[SerializeField]
	private AudioClip warningAudio;

	// Token: 0x04002EC7 RID: 11975
	[SerializeField]
	[Tooltip("Must be ordered from largest time (first played) to smallest time (last played)")]
	private List<int> warningClipPlayTimes = new List<int>();

	// Token: 0x04002EC8 RID: 11976
	[Header("Ring")]
	[SerializeField]
	private Transform ringTransform;

	// Token: 0x04002EC9 RID: 11977
	[SerializeField]
	private float ringClosingDuration = 3f;

	// Token: 0x04002ECA RID: 11978
	[SerializeField]
	private float ringClosingMaxRadius = 100f;

	// Token: 0x04002ECB RID: 11979
	[SerializeField]
	private float ringClosingMinRadius = 7f;

	// Token: 0x04002ECC RID: 11980
	[Header("Debug")]
	[SerializeField]
	private float debugFastForwardRate = 30f;

	// Token: 0x04002ECD RID: 11981
	[SerializeField]
	private bool debugFastForwarding;

	// Token: 0x04002ECE RID: 11982
	private bool shiftStarted;

	// Token: 0x04002ECF RID: 11983
	private double shiftStartNetworkTime;

	// Token: 0x04002ED0 RID: 11984
	private double shiftEndNetworkTime;

	// Token: 0x04002ED1 RID: 11985
	private float prevCountDownTotal;

	// Token: 0x04002ED2 RID: 11986
	[SerializeField]
	private int shiftTotalEarned = -1;

	// Token: 0x04002ED3 RID: 11987
	[SerializeField]
	private int shiftSanityMaximumEarned = 10000;

	// Token: 0x04002ED4 RID: 11988
	public GhostReactorShiftDepthDisplay depthDisplay;

	// Token: 0x04002ED5 RID: 11989
	public bool authorizedToDelveDeeper;

	// Token: 0x04002ED6 RID: 11990
	public int coresRequiredToDelveDeeper;

	// Token: 0x04002ED7 RID: 11991
	public int sentientCoresRequiredToDelveDeeper;

	// Token: 0x04002ED8 RID: 11992
	public int maxPlayerDeaths;

	// Token: 0x04002ED9 RID: 11993
	private bool localPlayerInside;

	// Token: 0x04002EDA RID: 11994
	private bool localPlayerOverlapping;

	// Token: 0x04002EDB RID: 11995
	private float totalPlayTime;

	// Token: 0x04002EDC RID: 11996
	private string gameIdGuid = "";

	// Token: 0x04002EDD RID: 11997
	public GRShiftStat shiftStats = new GRShiftStat();

	// Token: 0x04002EDE RID: 11998
	[NonSerialized]
	private GhostReactorManager grManager;
}
