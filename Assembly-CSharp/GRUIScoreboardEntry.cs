using System;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x02000696 RID: 1686
public class GRUIScoreboardEntry : MonoBehaviour
{
	// Token: 0x06002948 RID: 10568 RVA: 0x000DE038 File Offset: 0x000DC238
	public void Setup(VRRig vrRig, int playerActorId, GRUIScoreboard.ScoreboardScreen screenType)
	{
		this.playerActorId = playerActorId;
		this.Refresh(vrRig, screenType);
	}

	// Token: 0x06002949 RID: 10569 RVA: 0x000DE04C File Offset: 0x000DC24C
	private void Refresh(VRRig vrRig, GRUIScoreboard.ScoreboardScreen screenType)
	{
		GRPlayer grplayer = GRPlayer.Get(vrRig);
		if (!(vrRig != null) || !(grplayer != null))
		{
			this.playerNameLabel.text = "";
			this.playerCurrencyLabel.text = "";
			this.playerTitleLabel.text = "";
			this.playerCutLabel.text = "";
			this.currencySet = 0;
			return;
		}
		if (!this.playerNameLabel.text.Equals(vrRig.playerNameVisible))
		{
			this.playerNameLabel.text = vrRig.playerNameVisible;
		}
		if (screenType != GRUIScoreboard.ScoreboardScreen.DefaultInfo)
		{
			if (screenType == GRUIScoreboard.ScoreboardScreen.ShiftCutCalculation)
			{
				this.defaultUIParent.SetActive(false);
				this.shiftCutParent.SetActive(true);
				if (GhostReactor.instance.shiftManager.ShiftActive || GhostReactor.instance.shiftManager.ShiftTotalEarned >= 0)
				{
					int num = Mathf.FloorToInt(grplayer.ShiftPlayTime / 60f);
					int num2 = Mathf.FloorToInt(grplayer.ShiftPlayTime - (float)(num * 60));
					this.playerTimeLabel.text = string.Format("{0:00}:{1:00}", num, num2);
					this.playerPercentageLabel.text = "%" + Mathf.Floor(grplayer.ShiftPlayTime / GhostReactor.instance.shiftManager.TotalPlayTime * 100f).ToString();
				}
				else
				{
					this.playerTimeLabel.text = "n/a";
					this.playerPercentageLabel.text = "n/a";
				}
				this.playerTitleLabel.text = this.titleSet;
			}
		}
		else
		{
			this.defaultUIParent.SetActive(true);
			this.shiftCutParent.SetActive(false);
			if (grplayer.currency != this.currencySet)
			{
				this.currencySet = grplayer.currency;
				this.playerCurrencyLabel.text = this.currencySet.ToString();
			}
			string titleNameAndGrade = GhostReactorProgression.GetTitleNameAndGrade(grplayer.CurrentProgression.redeemedPoints);
			if (titleNameAndGrade != this.titleSet)
			{
				this.titleSet = titleNameAndGrade;
				this.playerTitleLabel.text = this.titleSet;
			}
		}
		if (GhostReactor.instance.shiftManager.ShiftActive)
		{
			this.playerCutLabel.text = "-";
			return;
		}
		this.playerCutLabel.text = grplayer.LastShiftCut.ToString();
	}

	// Token: 0x04003545 RID: 13637
	[SerializeField]
	private TMP_Text playerNameLabel;

	// Token: 0x04003546 RID: 13638
	[SerializeField]
	private TMP_Text playerCutLabel;

	// Token: 0x04003547 RID: 13639
	public GameObject defaultUIParent;

	// Token: 0x04003548 RID: 13640
	[SerializeField]
	private TMP_Text playerTitleLabel;

	// Token: 0x04003549 RID: 13641
	[SerializeField]
	private TMP_Text playerCurrencyLabel;

	// Token: 0x0400354A RID: 13642
	public GameObject shiftCutParent;

	// Token: 0x0400354B RID: 13643
	[SerializeField]
	private TMP_Text playerTimeLabel;

	// Token: 0x0400354C RID: 13644
	[SerializeField]
	private TMP_Text playerPercentageLabel;

	// Token: 0x0400354D RID: 13645
	private int playerActorId = -1;

	// Token: 0x0400354E RID: 13646
	private int currencySet = -1;

	// Token: 0x0400354F RID: 13647
	private string titleSet = "";
}
