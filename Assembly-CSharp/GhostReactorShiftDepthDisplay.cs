using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x020005E2 RID: 1506
[Serializable]
public class GhostReactorShiftDepthDisplay
{
	// Token: 0x0600250D RID: 9485 RVA: 0x000C75E8 File Offset: 0x000C57E8
	public void RefreshDisplay()
	{
		int depthLevel = this.reactor.GetDepthLevel();
		GhostReactorLevelDepthConfig depthLevelConfig = this.reactor.GetDepthLevelConfig(depthLevel);
		GhostReactorLevelDepthConfig depthLevelConfig2 = this.reactor.GetDepthLevelConfig(depthLevel + 1);
		if (this.currShiftLevelName != null)
		{
			this.currShiftLevelName.text = depthLevelConfig.displayName;
		}
		if (this.nextShiftLevelName != null)
		{
			this.nextShiftLevelName.text = depthLevelConfig2.displayName;
		}
		int num = 0;
		if (num < this.requirementText.Length && this.shiftManager.coresRequiredToDelveDeeper > 0)
		{
			this.requirementText[num].text = string.Format("Collect {0} Cores", this.shiftManager.coresRequiredToDelveDeeper);
			num++;
		}
		if (num < this.requirementText.Length && this.shiftManager.sentientCoresRequiredToDelveDeeper > 0)
		{
			this.requirementText[num].text = string.Format("Collect {0} Sentient", this.shiftManager.sentientCoresRequiredToDelveDeeper);
			num++;
		}
		if (num < this.requirementText.Length && this.shiftManager.maxPlayerDeaths >= 0)
		{
			this.requirementText[num].text = string.Format("Limit Incidents to {0}", this.shiftManager.maxPlayerDeaths);
			num++;
		}
		for (int i = num; i < this.requirementText.Length; i++)
		{
			this.requirementText[i].text = null;
		}
		if (this.objectiveText != null)
		{
			for (int j = 0; j < this.objectiveText.Count; j++)
			{
				this.objectiveText[j].text = null;
			}
		}
		if (this.bonusObjectiveText != null)
		{
			for (int k = 0; k < this.bonusObjectiveText.Count; k++)
			{
				this.bonusObjectiveText[k].text = null;
			}
		}
		this.RefreshObjectives();
	}

	// Token: 0x0600250E RID: 9486 RVA: 0x000C77C0 File Offset: 0x000C59C0
	public void RefreshObjectives()
	{
		GRShiftStat shiftStats = this.shiftManager.shiftStats;
		bool flag = shiftStats.GetShiftStat(GRShiftStatType.CoresCollected) >= this.shiftManager.coresRequiredToDelveDeeper;
		bool flag2 = shiftStats.GetShiftStat(GRShiftStatType.SentientCoresCollected) >= this.shiftManager.sentientCoresRequiredToDelveDeeper;
		bool flag3 = this.shiftManager.maxPlayerDeaths < 0 || shiftStats.GetShiftStat(GRShiftStatType.PlayerDeaths) <= this.shiftManager.maxPlayerDeaths;
		if (this.shiftManager.ShiftActive && flag && flag2 && flag3)
		{
			this.shiftManager.authorizedToDelveDeeper = true;
		}
		bool authorizedToDelveDeeper = this.shiftManager.authorizedToDelveDeeper;
		if (authorizedToDelveDeeper)
		{
			this.delveDeeperStatusText.text = "<color=green>AUTHORIZED</color>";
		}
		else
		{
			this.delveDeeperStatusText.text = "<color=red>UNAUTHORIZED</color>";
		}
		this.delveDeeperButton.SetActive(authorizedToDelveDeeper && !this.shiftManager.ShiftActive);
	}

	// Token: 0x0600250F RID: 9487 RVA: 0x000C78A4 File Offset: 0x000C5AA4
	public void PlayDelveDeeperFX()
	{
		this.delveDeeperAudio.Play();
	}

	// Token: 0x04002EAE RID: 11950
	public GhostReactorShiftManager shiftManager;

	// Token: 0x04002EAF RID: 11951
	public GhostReactor reactor;

	// Token: 0x04002EB0 RID: 11952
	[SerializeField]
	private TMP_Text currShiftLevelName;

	// Token: 0x04002EB1 RID: 11953
	[SerializeField]
	private TMP_Text nextShiftLevelName;

	// Token: 0x04002EB2 RID: 11954
	[SerializeField]
	private TMP_Text[] requirementText;

	// Token: 0x04002EB3 RID: 11955
	[SerializeField]
	private TMP_Text delveDeeperStatusText;

	// Token: 0x04002EB4 RID: 11956
	[SerializeField]
	private List<TMP_Text> objectiveText;

	// Token: 0x04002EB5 RID: 11957
	[SerializeField]
	private List<TMP_Text> bonusObjectiveText;

	// Token: 0x04002EB6 RID: 11958
	[SerializeField]
	private GameObject delveDeeperButton;

	// Token: 0x04002EB7 RID: 11959
	[SerializeField]
	private AudioSource delveDeeperAudio;
}
