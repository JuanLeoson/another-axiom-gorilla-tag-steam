using System;
using TMPro;
using UnityEngine;

// Token: 0x02000709 RID: 1801
public class GorillaTagCompetitiveScoreboardLine : MonoBehaviour
{
	// Token: 0x06002D28 RID: 11560 RVA: 0x000EDFBC File Offset: 0x000EC1BC
	public void SetPlayer(string playerName, Sprite icon)
	{
		this.playerNameDisplay.text = playerName;
		this.rankSprite.sprite = icon;
	}

	// Token: 0x06002D29 RID: 11561 RVA: 0x000EDFD8 File Offset: 0x000EC1D8
	public void SetScore(float untaggedTime, int tagCount)
	{
		int num = Mathf.FloorToInt(untaggedTime);
		int num2 = num / 60;
		int num3 = num % 60;
		this.untaggedTimeDisplay.text = string.Format("{0}:{1:D2}", num2, num3);
		this.tagCountDisplay.text = tagCount.ToString();
	}

	// Token: 0x06002D2A RID: 11562 RVA: 0x000EE027 File Offset: 0x000EC227
	public void SetPredictedResult(GorillaTagCompetitiveScoreboard.PredictedResult result)
	{
		this.resultSprite.sprite = this.resultSprites[(int)result];
	}

	// Token: 0x06002D2B RID: 11563 RVA: 0x000EE03C File Offset: 0x000EC23C
	public void DisplayPredictedResults(bool bShow)
	{
		this.resultSprite.gameObject.SetActive(bShow);
	}

	// Token: 0x06002D2C RID: 11564 RVA: 0x000EE04F File Offset: 0x000EC24F
	public void SetInfected(bool infected)
	{
		this.playerNameDisplay.color = (infected ? Color.red : Color.white);
	}

	// Token: 0x04003877 RID: 14455
	public SpriteRenderer rankSprite;

	// Token: 0x04003878 RID: 14456
	public TMP_Text playerNameDisplay;

	// Token: 0x04003879 RID: 14457
	public TMP_Text untaggedTimeDisplay;

	// Token: 0x0400387A RID: 14458
	public TMP_Text tagCountDisplay;

	// Token: 0x0400387B RID: 14459
	public SpriteRenderer resultSprite;

	// Token: 0x0400387C RID: 14460
	public Sprite[] resultSprites;
}
