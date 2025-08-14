using System;
using TMPro;
using UnityEngine;

// Token: 0x02000222 RID: 546
public class RaceVisual : MonoBehaviour
{
	// Token: 0x17000141 RID: 321
	// (get) Token: 0x06000CC6 RID: 3270 RVA: 0x0004490F File Offset: 0x00042B0F
	// (set) Token: 0x06000CC7 RID: 3271 RVA: 0x00044917 File Offset: 0x00042B17
	public int raceId { get; private set; }

	// Token: 0x17000142 RID: 322
	// (get) Token: 0x06000CC8 RID: 3272 RVA: 0x00044920 File Offset: 0x00042B20
	// (set) Token: 0x06000CC9 RID: 3273 RVA: 0x00044928 File Offset: 0x00042B28
	public bool TickRunning { get; set; }

	// Token: 0x06000CCA RID: 3274 RVA: 0x00044931 File Offset: 0x00042B31
	private void Awake()
	{
		this.checkpoints = base.GetComponent<RaceCheckpointManager>();
		this.finishLineText.text = "";
		this.SetScoreboardText("", "");
		this.SetRaceStartScoreboardText("", "");
	}

	// Token: 0x06000CCB RID: 3275 RVA: 0x0004496F File Offset: 0x00042B6F
	private void OnEnable()
	{
		RacingManager.instance.RegisterVisual(this);
	}

	// Token: 0x06000CCC RID: 3276 RVA: 0x0004497C File Offset: 0x00042B7C
	public void Button_StartRace(int laps)
	{
		RacingManager.instance.Button_StartRace(this.raceId, laps);
	}

	// Token: 0x06000CCD RID: 3277 RVA: 0x0004498F File Offset: 0x00042B8F
	public void ShowFinishLineText(string text)
	{
		this.finishLineText.text = text;
	}

	// Token: 0x06000CCE RID: 3278 RVA: 0x0004499D File Offset: 0x00042B9D
	public void UpdateCountdown(int timeRemaining)
	{
		if (timeRemaining != this.lastDisplayedCountdown)
		{
			this.countdownText.text = timeRemaining.ToString();
			this.finishLineText.text = "";
			this.lastDisplayedCountdown = timeRemaining;
		}
	}

	// Token: 0x06000CCF RID: 3279 RVA: 0x000449D4 File Offset: 0x00042BD4
	public void SetScoreboardText(string mainText, string timesText)
	{
		foreach (RacingScoreboard racingScoreboard in this.raceScoreboards)
		{
			racingScoreboard.mainDisplay.text = mainText;
			racingScoreboard.timesDisplay.text = timesText;
		}
	}

	// Token: 0x06000CD0 RID: 3280 RVA: 0x00044A10 File Offset: 0x00042C10
	public void SetRaceStartScoreboardText(string mainText, string timesText)
	{
		this.raceStartScoreboard.mainDisplay.text = mainText;
		this.raceStartScoreboard.timesDisplay.text = timesText;
	}

	// Token: 0x06000CD1 RID: 3281 RVA: 0x00044A34 File Offset: 0x00042C34
	public void ActivateStartingWall(bool enable)
	{
		this.startingWall.SetActive(enable);
	}

	// Token: 0x06000CD2 RID: 3282 RVA: 0x00044A42 File Offset: 0x00042C42
	public bool IsPlayerNearCheckpoint(VRRig player, int checkpoint)
	{
		return this.checkpoints.IsPlayerNearCheckpoint(player, checkpoint);
	}

	// Token: 0x06000CD3 RID: 3283 RVA: 0x00044A51 File Offset: 0x00042C51
	public void OnCountdownStart(int laps, float goAfterInterval)
	{
		this.raceConsoleVisual.ShowRaceInProgress(laps);
		this.countdownSoundPlayer.Play();
		this.countdownSoundPlayer.time = this.countdownSoundGoTime - goAfterInterval;
	}

	// Token: 0x06000CD4 RID: 3284 RVA: 0x00044A7D File Offset: 0x00042C7D
	public void OnRaceStart()
	{
		this.finishLineText.text = "GO!";
		this.checkpoints.OnRaceStart();
		this.lastDisplayedCountdown = 0;
		this.startingWall.SetActive(false);
		this.isRaceEndSoundEnabled = false;
	}

	// Token: 0x06000CD5 RID: 3285 RVA: 0x00044AB4 File Offset: 0x00042CB4
	public void OnRaceEnded()
	{
		this.finishLineText.text = "";
		this.lastDisplayedCountdown = 0;
		this.checkpoints.OnRaceEnd();
	}

	// Token: 0x06000CD6 RID: 3286 RVA: 0x00044AD8 File Offset: 0x00042CD8
	public void OnRaceReset()
	{
		this.raceConsoleVisual.ShowCanStartRace();
	}

	// Token: 0x06000CD7 RID: 3287 RVA: 0x00044AE5 File Offset: 0x00042CE5
	public void EnableRaceEndSound()
	{
		this.isRaceEndSoundEnabled = true;
	}

	// Token: 0x06000CD8 RID: 3288 RVA: 0x00044AEE File Offset: 0x00042CEE
	public void OnCheckpointPassed(int index, SoundBankPlayer checkpointSound)
	{
		if (index == 0 && this.isRaceEndSoundEnabled)
		{
			this.countdownSoundPlayer.PlayOneShot(this.raceEndSound);
		}
		else
		{
			checkpointSound.Play();
		}
		RacingManager.instance.OnCheckpointPassed(this.raceId, index);
	}

	// Token: 0x04000FBC RID: 4028
	[SerializeField]
	private TextMeshPro finishLineText;

	// Token: 0x04000FBD RID: 4029
	[SerializeField]
	private TextMeshPro countdownText;

	// Token: 0x04000FBE RID: 4030
	[SerializeField]
	private RacingScoreboard[] raceScoreboards;

	// Token: 0x04000FBF RID: 4031
	[SerializeField]
	private RacingScoreboard raceStartScoreboard;

	// Token: 0x04000FC0 RID: 4032
	[SerializeField]
	private RaceConsoleVisual raceConsoleVisual;

	// Token: 0x04000FC1 RID: 4033
	private float nextVisualRefreshTimestamp;

	// Token: 0x04000FC2 RID: 4034
	private RaceCheckpointManager checkpoints;

	// Token: 0x04000FC3 RID: 4035
	[SerializeField]
	private AudioClip raceEndSound;

	// Token: 0x04000FC4 RID: 4036
	[SerializeField]
	private float countdownSoundGoTime;

	// Token: 0x04000FC5 RID: 4037
	[SerializeField]
	private AudioSource countdownSoundPlayer;

	// Token: 0x04000FC6 RID: 4038
	[SerializeField]
	private GameObject startingWall;

	// Token: 0x04000FC7 RID: 4039
	private int lastDisplayedCountdown;

	// Token: 0x04000FC8 RID: 4040
	private bool isRaceEndSoundEnabled;
}
