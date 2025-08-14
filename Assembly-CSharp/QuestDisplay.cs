using System;
using TMPro;
using UnityEngine;

// Token: 0x02000146 RID: 326
public class QuestDisplay : MonoBehaviour
{
	// Token: 0x170000CD RID: 205
	// (get) Token: 0x06000889 RID: 2185 RVA: 0x0002EB5E File Offset: 0x0002CD5E
	public bool IsChanged
	{
		get
		{
			return this.quest.lastChange > this._lastUpdate;
		}
	}

	// Token: 0x0600088A RID: 2186 RVA: 0x0002EB74 File Offset: 0x0002CD74
	public void UpdateDisplay()
	{
		this.text.text = this.quest.GetTextDescription();
		if (this.quest.isQuestComplete)
		{
			this.progressDisplay.SetVisible(false);
		}
		else if (this.quest.requiredOccurenceCount > 1)
		{
			this.progressDisplay.SetProgress(this.quest.occurenceCount, this.quest.requiredOccurenceCount);
			this.progressDisplay.SetVisible(true);
		}
		else
		{
			this.progressDisplay.SetVisible(false);
		}
		this.UpdateCompletionIndicator();
		this._lastUpdate = Time.frameCount;
	}

	// Token: 0x0600088B RID: 2187 RVA: 0x0002EC0C File Offset: 0x0002CE0C
	private void UpdateCompletionIndicator()
	{
		bool isQuestComplete = this.quest.isQuestComplete;
		bool flag = !isQuestComplete && this.quest.requiredOccurenceCount == 1;
		this.dailyIncompleteIndicator.SetActive(this.quest.isDailyQuest && flag);
		this.dailyCompleteIndicator.SetActive(this.quest.isDailyQuest && isQuestComplete);
		this.weeklyIncompleteIndicator.SetActive(!this.quest.isDailyQuest && flag);
		this.weeklyCompleteIndicator.SetActive(!this.quest.isDailyQuest && isQuestComplete);
	}

	// Token: 0x04000A20 RID: 2592
	[SerializeField]
	private ProgressDisplay progressDisplay;

	// Token: 0x04000A21 RID: 2593
	[SerializeField]
	private TMP_Text text;

	// Token: 0x04000A22 RID: 2594
	[SerializeField]
	private TMP_Text statusText;

	// Token: 0x04000A23 RID: 2595
	[SerializeField]
	private GameObject dailyIncompleteIndicator;

	// Token: 0x04000A24 RID: 2596
	[SerializeField]
	private GameObject dailyCompleteIndicator;

	// Token: 0x04000A25 RID: 2597
	[SerializeField]
	private GameObject weeklyIncompleteIndicator;

	// Token: 0x04000A26 RID: 2598
	[SerializeField]
	private GameObject weeklyCompleteIndicator;

	// Token: 0x04000A27 RID: 2599
	[NonSerialized]
	public RotatingQuestsManager.RotatingQuest quest;

	// Token: 0x04000A28 RID: 2600
	private int _lastUpdate = -1;
}
