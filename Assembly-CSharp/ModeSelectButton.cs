using System;
using GameObjectScheduling;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x02000796 RID: 1942
public class ModeSelectButton : GorillaPressableButton
{
	// Token: 0x17000485 RID: 1157
	// (get) Token: 0x060030D9 RID: 12505 RVA: 0x000FF7AD File Offset: 0x000FD9AD
	// (set) Token: 0x060030DA RID: 12506 RVA: 0x000FF7B5 File Offset: 0x000FD9B5
	public PartyGameModeWarning WarningScreen
	{
		get
		{
			return this.warningScreen;
		}
		set
		{
			this.warningScreen = value;
		}
	}

	// Token: 0x060030DB RID: 12507 RVA: 0x000FF7BE File Offset: 0x000FD9BE
	public override void Start()
	{
		base.Start();
		GorillaComputer.instance.currentGameMode.AddCallback(new Action<string>(this.OnGameModeChanged), true);
	}

	// Token: 0x060030DC RID: 12508 RVA: 0x000FF7E4 File Offset: 0x000FD9E4
	private void OnDestroy()
	{
		if (!ApplicationQuittingState.IsQuitting)
		{
			GorillaComputer.instance.currentGameMode.RemoveCallback(new Action<string>(this.OnGameModeChanged));
		}
	}

	// Token: 0x060030DD RID: 12509 RVA: 0x000FF80A File Offset: 0x000FDA0A
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivationWithHand(isLeftHand);
		if (this.warningScreen.ShouldShowWarning)
		{
			this.warningScreen.Show();
			return;
		}
		GorillaComputer.instance.OnModeSelectButtonPress(this.gameMode, isLeftHand);
	}

	// Token: 0x060030DE RID: 12510 RVA: 0x000FF83F File Offset: 0x000FDA3F
	public void OnGameModeChanged(string newGameMode)
	{
		this.buttonRenderer.material = ((newGameMode.ToLower() == this.gameMode.ToLower()) ? this.pressedMaterial : this.unpressedMaterial);
	}

	// Token: 0x060030DF RID: 12511 RVA: 0x000FF872 File Offset: 0x000FDA72
	public void SetInfo(ModeSelectButtonInfoData info)
	{
		this.SetInfo(info.Mode, info.ModeTitle, info.NewMode, info.CountdownTo);
	}

	// Token: 0x060030E0 RID: 12512 RVA: 0x000FF894 File Offset: 0x000FDA94
	public void SetInfo(string Mode, string ModeTitle, bool NewMode, CountdownTextDate CountdownTo)
	{
		this.gameModeTitle.text = ModeTitle;
		this.gameMode = Mode;
		this.newModeSplash.SetActive(NewMode);
		this.limitedCountdown.gameObject.SetActive(false);
		if (CountdownTo == null)
		{
			return;
		}
		this.limitedCountdown.Countdown = CountdownTo;
		this.limitedCountdown.gameObject.SetActive(true);
	}

	// Token: 0x060030E1 RID: 12513 RVA: 0x000FF8FA File Offset: 0x000FDAFA
	public void HideNewAndLimitedTimeInfo()
	{
		this.limitedCountdown.gameObject.SetActive(false);
		this.newModeSplash.SetActive(false);
	}

	// Token: 0x04003C99 RID: 15513
	[SerializeField]
	public string gameMode;

	// Token: 0x04003C9A RID: 15514
	[SerializeField]
	private PartyGameModeWarning warningScreen;

	// Token: 0x04003C9B RID: 15515
	[SerializeField]
	private TMP_Text gameModeTitle;

	// Token: 0x04003C9C RID: 15516
	[SerializeField]
	private GameObject newModeSplash;

	// Token: 0x04003C9D RID: 15517
	[SerializeField]
	private CountdownText limitedCountdown;
}
