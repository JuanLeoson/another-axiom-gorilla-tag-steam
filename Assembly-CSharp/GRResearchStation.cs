using System;
using GorillaExtensions;
using GorillaNetworking;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000661 RID: 1633
public class GRResearchStation : MonoBehaviour
{
	// Token: 0x060027F2 RID: 10226 RVA: 0x000D705C File Offset: 0x000D525C
	public void Init(GRResearchManager researchManager)
	{
		this._researchManager = researchManager;
		this.upgradeIndexOffset = 0;
		this.totalTools = this._researchManager.researchTree.RootNodes.Count;
		this.selectedToolIndex = 0;
		this.currentPage = 0;
		this._levelString = this.LevelText.text;
		this._costString = this.CostText.text;
		this._researchPointsString = this.ResearchPointsTex.text;
		this._requiredLevelString = this.RequiredLevelText.text;
		this._bonusString = this.BonusText.text;
		this.UpdateUI();
		this.SelectTool(0);
	}

	// Token: 0x060027F3 RID: 10227 RVA: 0x000D7104 File Offset: 0x000D5304
	private void SelectTool(int index)
	{
		if (this._researchManager.IsNull())
		{
			return;
		}
		if (index < this._researchManager.researchTree.RootNodes.Count && index > -1)
		{
			this.upgradeIndexOffset = 0;
			this.currentPage = 0;
			this.selectedToolIndex = index;
			this.SelectUpgrade(0);
			this.UpdateUI();
		}
	}

	// Token: 0x060027F4 RID: 10228 RVA: 0x000D715D File Offset: 0x000D535D
	public void ResearchTreeUpdated()
	{
		this.UpdateUI();
	}

	// Token: 0x060027F5 RID: 10229 RVA: 0x000D7165 File Offset: 0x000D5365
	public void UpdateUI()
	{
		this.UpdateToolName();
		this.UpdateUpgradeTitles();
		this.UpdateLocked();
		this.UpdateRequiredLevel();
		this.UpdateCost();
		this.UpdateResearchPoints(this._researchManager.playerResearchPoints);
	}

	// Token: 0x060027F6 RID: 10230 RVA: 0x000D7198 File Offset: 0x000D5398
	public void SelectUpgrade(int UpgradeIndex)
	{
		if (this._researchManager == null)
		{
			return;
		}
		if (this._researchManager.researchTree.RootNodes[this.selectedToolIndex].modNodes.Count > UpgradeIndex + this.upgradeIndexOffset && UpgradeIndex > -1)
		{
			this.selectedUpgradeIndex = UpgradeIndex;
			this.SetUpgradeTextColors(UpgradeIndex);
			this.UpdateDescriptionText(this._researchManager.researchTree.RootNodes[this.selectedToolIndex].modNodes[UpgradeIndex + this.upgradeIndexOffset].description);
			this.UpdateUI();
		}
	}

	// Token: 0x060027F7 RID: 10231 RVA: 0x000D7234 File Offset: 0x000D5434
	private void SetUpgradeTextColors(int index)
	{
		for (int i = 0; i < this.UpgradeTitlesText.Length; i++)
		{
			this.UpgradeButton[i].isOn = false;
			this.UpgradeButton[i].UpdateColor();
		}
		this.UpgradeButton[index].isOn = true;
		this.UpgradeButton[index].UpdateColor();
	}

	// Token: 0x060027F8 RID: 10232 RVA: 0x000D728C File Offset: 0x000D548C
	private void UpdateUpgradeTitles()
	{
		for (int i = 0; i < this.UpgradeTitlesText.Length; i++)
		{
			if (this._researchManager.researchTree.RootNodes.Count >= this.selectedToolIndex && this._researchManager.researchTree.RootNodes[this.selectedToolIndex].modNodes.Count > i + this.upgradeIndexOffset)
			{
				this.UpgradeTitlesText[i].text = this._researchManager.researchTree.RootNodes[this.selectedToolIndex].modNodes[i + this.upgradeIndexOffset].name;
			}
			else
			{
				this.UpgradeTitlesText[i].text = null;
			}
		}
	}

	// Token: 0x060027F9 RID: 10233 RVA: 0x000D7350 File Offset: 0x000D5550
	public void UpdateLocked()
	{
		if (this._researchManager.IsResearchUnlocked(this._researchManager.researchTree.RootNodes[this.selectedToolIndex].modNodes[this.selectedUpgradeIndex + this.upgradeIndexOffset].id))
		{
			this.UnlockedText.color = this.unlockedToolColor;
			this.UnlockedText.text = "UNLOCKED";
		}
		else
		{
			this.UnlockedText.color = this.lockedToolColor;
			this.UnlockedText.text = "LOCKED";
		}
		for (int i = 0; i < this.UpgradeTitlesText.Length; i++)
		{
			if (this._researchManager.researchTree.RootNodes.Count >= this.selectedToolIndex && this._researchManager.researchTree.RootNodes[this.selectedToolIndex].modNodes.Count > i + this.upgradeIndexOffset)
			{
				bool unlocked = this._researchManager.researchTree.RootNodes[this.selectedToolIndex].modNodes[i + this.upgradeIndexOffset].unlocked;
				this.UpgradeTitlesText[i].color = (unlocked ? this.unlockedToolColor : this.lockedToolColor);
				this.LockedImage[i].gameObject.SetActive(!unlocked);
			}
			else
			{
				this.UpgradeTitlesText[i].color = Color.black;
				this.LockedImage[i].gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x060027FA RID: 10234 RVA: 0x000D74DC File Offset: 0x000D56DC
	public void UpdateRequiredLevel()
	{
		int requiredEmployeeLevel = this._researchManager.researchTree.RootNodes[this.selectedToolIndex].modNodes[this.selectedUpgradeIndex + this.upgradeIndexOffset].requiredEmployeeLevel;
		string titleNameFromLevel = GhostReactorProgression.GetTitleNameFromLevel(requiredEmployeeLevel);
		int num = 0;
		GRPlayer grplayer = GRPlayer.Get(PhotonNetwork.LocalPlayer.ActorNumber);
		if (grplayer != null)
		{
			num = GhostReactorProgression.GetTitleLevel(grplayer.CurrentProgression.redeemedPoints);
		}
		string titleNameFromLevel2 = GhostReactorProgression.GetTitleNameFromLevel(num);
		this.RequiredLevelText.text = string.Format(this._requiredLevelString, titleNameFromLevel);
		this.LevelText.text = string.Format(this._levelString, titleNameFromLevel2);
		this.RequiredLevelText.color = ((num >= requiredEmployeeLevel) ? this.unlockedToolColor : this.lockedToolColor);
	}

	// Token: 0x060027FB RID: 10235 RVA: 0x000D75A8 File Offset: 0x000D57A8
	public void UpdateDescriptionText(string description)
	{
		this.DescriptionText.text = description;
	}

	// Token: 0x060027FC RID: 10236 RVA: 0x000D75B8 File Offset: 0x000D57B8
	public void UpdateCost()
	{
		int playerResearchPoints = this._researchManager.playerResearchPoints;
		int researchCost = this._researchManager.researchTree.RootNodes[this.selectedToolIndex].modNodes[this.selectedUpgradeIndex + this.upgradeIndexOffset].researchCost;
		this.CostText.text = string.Format(this._costString, researchCost);
		this.CostText.color = ((playerResearchPoints >= researchCost) ? this.unlockedToolColor : this.lockedToolColor);
	}

	// Token: 0x060027FD RID: 10237 RVA: 0x000D7642 File Offset: 0x000D5842
	public void UpdateToolName()
	{
		this.ToolNameText.text = this._researchManager.researchTree.RootNodes[this.selectedToolIndex].name;
	}

	// Token: 0x060027FE RID: 10238 RVA: 0x000D766F File Offset: 0x000D586F
	public void UpdateResearchPoints(int ResearchPoints)
	{
		this.ResearchPointsTex.text = string.Format(this._researchPointsString, ResearchPoints);
	}

	// Token: 0x060027FF RID: 10239 RVA: 0x000D768D File Offset: 0x000D588D
	public void MFDButton0Pressed()
	{
		this.SelectUpgrade(0);
	}

	// Token: 0x06002800 RID: 10240 RVA: 0x000D7696 File Offset: 0x000D5896
	public void MFDButton1Pressed()
	{
		this.SelectUpgrade(1);
	}

	// Token: 0x06002801 RID: 10241 RVA: 0x000D769F File Offset: 0x000D589F
	public void MFDButton2Pressed()
	{
		this.SelectUpgrade(2);
	}

	// Token: 0x06002802 RID: 10242 RVA: 0x000D76A8 File Offset: 0x000D58A8
	public void MFDButton3Pressed()
	{
		this.SelectUpgrade(3);
	}

	// Token: 0x06002803 RID: 10243 RVA: 0x000D76B1 File Offset: 0x000D58B1
	public void MFDButton4Pressed()
	{
		this.SelectUpgrade(4);
	}

	// Token: 0x06002804 RID: 10244 RVA: 0x000D76BC File Offset: 0x000D58BC
	public void ScrollUpgradesUp()
	{
		if (this._researchManager.researchTree.RootNodes[this.selectedToolIndex].modNodes.Count > this.UpgradeTitlesText.Length)
		{
			int num = this._researchManager.researchTree.RootNodes[this.selectedToolIndex].modNodes.Count / this.UpgradeTitlesText.Length;
			this.currentPage = ((this.currentPage == num) ? (this.currentPage = 0) : (this.currentPage + 1));
			this.upgradeIndexOffset = this.UpgradeTitlesText.Length * this.currentPage;
			this.SelectUpgrade(0);
			this.UpdateUI();
		}
	}

	// Token: 0x06002805 RID: 10245 RVA: 0x000D776C File Offset: 0x000D596C
	public void ScrolLUpgradesDown()
	{
		if (this._researchManager.researchTree.RootNodes[this.selectedToolIndex].modNodes.Count > this.UpgradeTitlesText.Length)
		{
			int num = this._researchManager.researchTree.RootNodes[this.selectedToolIndex].modNodes.Count / this.UpgradeTitlesText.Length;
			this.currentPage = ((this.currentPage <= 0) ? num : (this.currentPage - 1));
			this.upgradeIndexOffset = this.UpgradeTitlesText.Length * this.currentPage;
			this.SelectUpgrade(0);
			this.UpdateUI();
		}
	}

	// Token: 0x06002806 RID: 10246 RVA: 0x000D7813 File Offset: 0x000D5A13
	public void NextToolButtonPressed()
	{
		this.selectedToolIndex = (this.selectedToolIndex + 1) % this.totalTools;
		this.SelectTool(this.selectedToolIndex);
	}

	// Token: 0x06002807 RID: 10247 RVA: 0x000D7836 File Offset: 0x000D5A36
	public void PreviousToolButtonPressed()
	{
		this.selectedToolIndex = (this.selectedToolIndex - 1).PositiveModulo(this.totalTools);
		this.SelectTool(this.selectedToolIndex);
	}

	// Token: 0x06002808 RID: 10248 RVA: 0x000D7860 File Offset: 0x000D5A60
	public void UpgradeButtonPressed()
	{
		UnityEvent onSucceeded = this.scanner.onSucceeded;
		if (onSucceeded != null)
		{
			onSucceeded.Invoke();
		}
		this._researchManager.RequestUnlockUpgrade(this._researchManager.researchTree.RootNodes[this.selectedToolIndex].modNodes[this.selectedUpgradeIndex + this.upgradeIndexOffset].id);
	}

	// Token: 0x06002809 RID: 10249 RVA: 0x000D78C5 File Offset: 0x000D5AC5
	public void ResearchCompleted(bool success, string researchID)
	{
		this.UpdateUI();
	}

	// Token: 0x04003362 RID: 13154
	public Color selectedUpgradeColor = Color.yellow;

	// Token: 0x04003363 RID: 13155
	public Color unselectedUpgradeColor = Color.black;

	// Token: 0x04003364 RID: 13156
	public Color lockedToolColor = Color.red;

	// Token: 0x04003365 RID: 13157
	public Color unlockedToolColor = Color.green;

	// Token: 0x04003366 RID: 13158
	private int selectedUpgradeIndex;

	// Token: 0x04003367 RID: 13159
	[SerializeField]
	private IDCardScanner scanner;

	// Token: 0x04003368 RID: 13160
	[SerializeField]
	private TMP_Text BonusText;

	// Token: 0x04003369 RID: 13161
	[SerializeField]
	private TMP_Text CostText;

	// Token: 0x0400336A RID: 13162
	[SerializeField]
	private TMP_Text DescriptionText;

	// Token: 0x0400336B RID: 13163
	[SerializeField]
	private TMP_Text LevelText;

	// Token: 0x0400336C RID: 13164
	[SerializeField]
	private TMP_Text ResearchPointsTex;

	// Token: 0x0400336D RID: 13165
	[SerializeField]
	private TMP_Text RequiredLevelText;

	// Token: 0x0400336E RID: 13166
	[SerializeField]
	private TMP_Text ToolNameText;

	// Token: 0x0400336F RID: 13167
	[SerializeField]
	private TMP_Text UnlockedText;

	// Token: 0x04003370 RID: 13168
	[SerializeField]
	private TMP_Text[] UpgradePointerText;

	// Token: 0x04003371 RID: 13169
	[SerializeField]
	private TMP_Text[] UpgradeTitlesText;

	// Token: 0x04003372 RID: 13170
	[SerializeField]
	private Image[] LockedImage;

	// Token: 0x04003373 RID: 13171
	[SerializeField]
	private GorillaPressableButton[] UpgradeButton;

	// Token: 0x04003374 RID: 13172
	private string _bonusString;

	// Token: 0x04003375 RID: 13173
	private string _costString;

	// Token: 0x04003376 RID: 13174
	private string _levelString;

	// Token: 0x04003377 RID: 13175
	private string _researchPointsString;

	// Token: 0x04003378 RID: 13176
	private string _requiredLevelString;

	// Token: 0x04003379 RID: 13177
	private GRResearchManager _researchManager;

	// Token: 0x0400337A RID: 13178
	private int upgradeIndexOffset;

	// Token: 0x0400337B RID: 13179
	private int selectedToolIndex;

	// Token: 0x0400337C RID: 13180
	private int totalTools;

	// Token: 0x0400337D RID: 13181
	private int currentPage;
}
