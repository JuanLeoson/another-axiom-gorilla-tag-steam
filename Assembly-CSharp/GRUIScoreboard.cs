using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000694 RID: 1684
public class GRUIScoreboard : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06002940 RID: 10560 RVA: 0x000DDE88 File Offset: 0x000DC088
	public void SliceUpdate()
	{
		if (this.currentScreen == GRUIScoreboard.ScoreboardScreen.ShiftCutCalculation)
		{
			this.Refresh(GhostReactor.instance.vrRigs);
		}
	}

	// Token: 0x06002941 RID: 10561 RVA: 0x000172AD File Offset: 0x000154AD
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06002942 RID: 10562 RVA: 0x000172B6 File Offset: 0x000154B6
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06002943 RID: 10563 RVA: 0x000DDEA4 File Offset: 0x000DC0A4
	public void Refresh(List<VRRig> vrRigs)
	{
		if (this.currentScreen == GRUIScoreboard.ScoreboardScreen.ShiftCutCalculation)
		{
			GhostReactor.instance.shiftManager.CalculatePlayerPercentages();
		}
		for (int i = 0; i < this.entries.Count; i++)
		{
			if (!(this.entries[i] == null))
			{
				if (i < vrRigs.Count && vrRigs[i] != null && vrRigs[i].OwningNetPlayer != null)
				{
					this.entries[i].gameObject.SetActive(true);
					this.entries[i].Setup(vrRigs[i], vrRigs[i].OwningNetPlayer.ActorNumber, this.currentScreen);
				}
				else
				{
					this.entries[i].gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x06002944 RID: 10564 RVA: 0x000DDF84 File Offset: 0x000DC184
	public void SwitchToScreen(GRUIScoreboard.ScoreboardScreen screenType)
	{
		this.currentScreen = screenType;
		GRUIScoreboard.ScoreboardScreen scoreboardScreen = this.currentScreen;
		if (scoreboardScreen == GRUIScoreboard.ScoreboardScreen.DefaultInfo)
		{
			this.infoTextParent.SetActive(true);
			this.calcTextParent.SetActive(false);
			this.buttonText.text = "SHOW CUT CALC";
			return;
		}
		if (scoreboardScreen != GRUIScoreboard.ScoreboardScreen.ShiftCutCalculation)
		{
			return;
		}
		this.infoTextParent.SetActive(false);
		this.calcTextParent.SetActive(true);
		this.buttonText.text = "SHOW INFO";
	}

	// Token: 0x06002945 RID: 10565 RVA: 0x000DDFF8 File Offset: 0x000DC1F8
	public void SwitchState()
	{
		if (this.currentScreen == GRUIScoreboard.ScoreboardScreen.DefaultInfo)
		{
			this.SwitchToScreen(GRUIScoreboard.ScoreboardScreen.ShiftCutCalculation);
		}
		else
		{
			this.SwitchToScreen(GRUIScoreboard.ScoreboardScreen.DefaultInfo);
		}
		this.Refresh(GhostReactor.instance.vrRigs);
		GhostReactor.UpdateRemoteScoreboardScreen(this.currentScreen);
	}

	// Token: 0x06002946 RID: 10566 RVA: 0x000DE02D File Offset: 0x000DC22D
	public static bool ValidPage(GRUIScoreboard.ScoreboardScreen screen)
	{
		return screen == GRUIScoreboard.ScoreboardScreen.DefaultInfo || screen == GRUIScoreboard.ScoreboardScreen.ShiftCutCalculation;
	}

	// Token: 0x0400353C RID: 13628
	public List<GRUIScoreboardEntry> entries;

	// Token: 0x0400353D RID: 13629
	public TMP_Text total;

	// Token: 0x0400353E RID: 13630
	public TMP_Text buttonText;

	// Token: 0x0400353F RID: 13631
	public GRUIScoreboard.ScoreboardScreen currentScreen;

	// Token: 0x04003540 RID: 13632
	public GameObject infoTextParent;

	// Token: 0x04003541 RID: 13633
	public GameObject calcTextParent;

	// Token: 0x02000695 RID: 1685
	public enum ScoreboardScreen
	{
		// Token: 0x04003543 RID: 13635
		DefaultInfo,
		// Token: 0x04003544 RID: 13636
		ShiftCutCalculation
	}
}
