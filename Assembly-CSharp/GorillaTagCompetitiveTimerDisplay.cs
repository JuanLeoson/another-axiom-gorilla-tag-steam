using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x0200071F RID: 1823
public class GorillaTagCompetitiveTimerDisplay : MonoBehaviour
{
	// Token: 0x06002D7D RID: 11645 RVA: 0x000EF5CC File Offset: 0x000ED7CC
	private void Awake()
	{
		this.prevTime = -1;
		if (this.waitingForPlayersBackground)
		{
			this.waitingForPlayersBackground.SetActive(true);
			this.currentBackground = this.waitingForPlayersBackground;
		}
		if (this.startCountdownBackground)
		{
			this.startCountdownBackground.SetActive(false);
		}
		if (this.playingBackground)
		{
			this.playingBackground.SetActive(false);
		}
		if (this.postRoundBackground)
		{
			this.postRoundBackground.SetActive(false);
		}
		this.timerDisplay.gameObject.SetActive(false);
		if (this.timerDisplay2)
		{
			this.timerDisplay2.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002D7E RID: 11646 RVA: 0x000EF680 File Offset: 0x000ED880
	private void OnEnable()
	{
		GorillaTagCompetitiveManager.onStateChanged += this.HandleOnGameStateChanged;
		GorillaTagCompetitiveManager.onUpdateRemainingTime += this.HandleOnTimeChanged;
		GorillaTagCompetitiveManager gorillaTagCompetitiveManager = GorillaGameManager.instance as GorillaTagCompetitiveManager;
		if (gorillaTagCompetitiveManager != null)
		{
			this.HandleOnGameStateChanged(gorillaTagCompetitiveManager.GetCurrentGameState());
		}
		this.myRig = base.GetComponentInParent<VRRig>();
		this.DisplayStandardTimer(false);
	}

	// Token: 0x06002D7F RID: 11647 RVA: 0x000EF6E2 File Offset: 0x000ED8E2
	private void OnDisable()
	{
		GorillaTagCompetitiveManager.onStateChanged -= this.HandleOnGameStateChanged;
		GorillaTagCompetitiveManager.onUpdateRemainingTime -= this.HandleOnTimeChanged;
	}

	// Token: 0x06002D80 RID: 11648 RVA: 0x000EF708 File Offset: 0x000ED908
	private void HandleOnGameStateChanged(GorillaTagCompetitiveManager.GameState newState)
	{
		this.SetNewBackground(newState);
		switch (newState)
		{
		case GorillaTagCompetitiveManager.GameState.WaitingForPlayers:
			this.DisplayStandardTimer(false);
			this.resultsDisplay.gameObject.SetActive(false);
			return;
		case GorillaTagCompetitiveManager.GameState.StartingCountdown:
		case GorillaTagCompetitiveManager.GameState.Playing:
			this.DisplayStandardTimer(true);
			return;
		case GorillaTagCompetitiveManager.GameState.PostRound:
			this.DoPostRoundShow();
			return;
		default:
			return;
		}
	}

	// Token: 0x06002D81 RID: 11649 RVA: 0x000EF75C File Offset: 0x000ED95C
	private void DisplayStandardTimer(bool bShow)
	{
		if (bShow)
		{
			this.resultsDisplay.gameObject.SetActive(false);
		}
		this.timerDisplay.gameObject.SetActive(bShow);
		if (this.timerDisplay2 != null)
		{
			this.timerDisplay2.gameObject.SetActive(bShow);
		}
	}

	// Token: 0x06002D82 RID: 11650 RVA: 0x000EF7B0 File Offset: 0x000ED9B0
	private void DoPostRoundShow()
	{
		GorillaTagCompetitiveManager gorillaTagCompetitiveManager = GorillaGameManager.instance as GorillaTagCompetitiveManager;
		if (gorillaTagCompetitiveManager == null)
		{
			return;
		}
		this.DisplayStandardTimer(false);
		this.resultsDisplay.gameObject.SetActive(true);
		List<VRRig> list = new List<VRRig>();
		List<RankedMultiplayerScore.PlayerScoreInRound> sortedScores = gorillaTagCompetitiveManager.GetScoring().GetSortedScores();
		float b = gorillaTagCompetitiveManager.GetScoring().ComputeGameScore(sortedScores[0].NumTags, sortedScores[0].PointsOnDefense);
		int num = 0;
		while (num < sortedScores.Count && num < 3)
		{
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(sortedScores[num].PlayerId, out rigContainer))
			{
				float a = gorillaTagCompetitiveManager.GetScoring().ComputeGameScore(sortedScores[num].NumTags, sortedScores[num].PointsOnDefense);
				if (num == 0 || a.Approx(b, 0.01f))
				{
					list.Add(rigContainer.Rig);
				}
				switch (num)
				{
				case 0:
					if (this.tintableCelebration != null)
					{
						Color playerColor = rigContainer.Rig.playerColor;
						float h;
						float s;
						float num2;
						Color.RGBToHSV(playerColor, out h, out s, out num2);
						Color max = Color.HSVToRGB(h, s, (num2 < 0.5f) ? (num2 + 0.5f) : (num2 - 0.5f));
						this.tintableCelebration.main.startColor = new ParticleSystem.MinMaxGradient(playerColor, max);
						this.tintableCelebration.gameObject.SetActive(true);
					}
					if (this.goldCelebration != null && rigContainer.Rig == this.myRig)
					{
						this.goldCelebration.gameObject.SetActive(true);
					}
					if (this.celebrationAudio != null)
					{
						this.celebrationAudio.Play();
					}
					break;
				case 1:
					if (this.silverCelebration != null && rigContainer.Rig == this.myRig)
					{
						this.silverCelebration.gameObject.SetActive(true);
					}
					if (this.celebrationAudio != null)
					{
						this.celebrationAudio.Play();
					}
					break;
				case 2:
					if (this.bronzeCelebration != null && rigContainer.Rig == this.myRig)
					{
						this.bronzeCelebration.gameObject.SetActive(true);
					}
					if (this.celebrationAudio != null)
					{
						this.celebrationAudio.Play();
					}
					break;
				}
			}
			num++;
		}
		for (int i = 0; i < this.postRoundTimerText.Length; i++)
		{
			this.postRoundTimerText[i].text = ((list.Count > 1) ? "SHARED WIN" : "WINNER");
		}
		string text = string.Empty;
		for (int j = 0; j < list.Count; j++)
		{
			text = text + list[j].playerText1.text.ToUpper() + "\n";
		}
		this.resultsDisplay.text = text.Trim();
		if (this.timerDisplay2 != null)
		{
			this.timerDisplay2.text = this.resultsDisplay.text;
		}
	}

	// Token: 0x06002D83 RID: 11651 RVA: 0x000EFAD8 File Offset: 0x000EDCD8
	private void HandleOnTimeChanged(float time)
	{
		int num = Mathf.CeilToInt(time);
		num = Mathf.Max(num, 1);
		if (this.prevTime != num)
		{
			this.prevTime = num;
			if (this.currentState == GorillaTagCompetitiveManager.GameState.Playing)
			{
				int num2 = this.prevTime / 60;
				int num3 = this.prevTime % 60;
				this.timerDisplay.text = string.Format("{0}:{1:D2}", num2, num3);
				if (this.timerDisplay2)
				{
					this.timerDisplay2.text = string.Format("{0}:{1:D2}", num2, num3);
					return;
				}
			}
			else if (this.currentState != GorillaTagCompetitiveManager.GameState.PostRound)
			{
				this.timerDisplay.text = this.prevTime.ToString("#00");
				if (this.timerDisplay2)
				{
					this.timerDisplay2.text = this.prevTime.ToString("#00");
				}
			}
		}
	}

	// Token: 0x06002D84 RID: 11652 RVA: 0x000EFBC0 File Offset: 0x000EDDC0
	private void SetNewBackground(GorillaTagCompetitiveManager.GameState newState)
	{
		if (this.currentBackground != null)
		{
			this.currentBackground.SetActive(false);
		}
		this.currentState = newState;
		GameObject x = this.SelectBackground(newState);
		this.GetTextColor(newState);
		this.currentBackground = null;
		if (x != null)
		{
			this.currentBackground = x;
			this.currentBackground.SetActive(true);
		}
	}

	// Token: 0x06002D85 RID: 11653 RVA: 0x000EFC21 File Offset: 0x000EDE21
	private GameObject SelectBackground(GorillaTagCompetitiveManager.GameState newState)
	{
		switch (newState)
		{
		case GorillaTagCompetitiveManager.GameState.WaitingForPlayers:
			return this.waitingForPlayersBackground;
		case GorillaTagCompetitiveManager.GameState.StartingCountdown:
			return this.startCountdownBackground;
		case GorillaTagCompetitiveManager.GameState.Playing:
			return this.playingBackground;
		case GorillaTagCompetitiveManager.GameState.PostRound:
			return this.postRoundBackground;
		default:
			return null;
		}
	}

	// Token: 0x06002D86 RID: 11654 RVA: 0x000EFC5A File Offset: 0x000EDE5A
	private Color GetTextColor(GorillaTagCompetitiveManager.GameState newState)
	{
		switch (newState)
		{
		case GorillaTagCompetitiveManager.GameState.StartingCountdown:
			return this.timerColorStart;
		case GorillaTagCompetitiveManager.GameState.Playing:
			return this.timerColorPlaying;
		case GorillaTagCompetitiveManager.GameState.PostRound:
			return this.timerColorPostRound;
		default:
			return Color.white;
		}
	}

	// Token: 0x040038D7 RID: 14551
	public TextMeshPro timerDisplay;

	// Token: 0x040038D8 RID: 14552
	public TextMeshPro timerDisplay2;

	// Token: 0x040038D9 RID: 14553
	public TextMeshPro resultsDisplay;

	// Token: 0x040038DA RID: 14554
	public GameObject waitingForPlayersBackground;

	// Token: 0x040038DB RID: 14555
	public GameObject startCountdownBackground;

	// Token: 0x040038DC RID: 14556
	public Color timerColorStart = Color.white;

	// Token: 0x040038DD RID: 14557
	public GameObject playingBackground;

	// Token: 0x040038DE RID: 14558
	public Color timerColorPlaying = Color.white;

	// Token: 0x040038DF RID: 14559
	public GameObject postRoundBackground;

	// Token: 0x040038E0 RID: 14560
	public Color timerColorPostRound = Color.white;

	// Token: 0x040038E1 RID: 14561
	public TextMeshPro[] postRoundTimerText;

	// Token: 0x040038E2 RID: 14562
	private GorillaTagCompetitiveManager.GameState currentState;

	// Token: 0x040038E3 RID: 14563
	private GameObject currentBackground;

	// Token: 0x040038E4 RID: 14564
	private int prevTime = -1;

	// Token: 0x040038E5 RID: 14565
	[SerializeField]
	private ParticleSystem tintableCelebration;

	// Token: 0x040038E6 RID: 14566
	[SerializeField]
	private ParticleSystem goldCelebration;

	// Token: 0x040038E7 RID: 14567
	[SerializeField]
	private ParticleSystem silverCelebration;

	// Token: 0x040038E8 RID: 14568
	[SerializeField]
	private ParticleSystem bronzeCelebration;

	// Token: 0x040038E9 RID: 14569
	private VRRig myRig;

	// Token: 0x040038EA RID: 14570
	[SerializeField]
	private AudioSource celebrationAudio;
}
