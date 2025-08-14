using System;
using TMPro;
using UnityEngine;

// Token: 0x02000705 RID: 1797
public class GorillaTagCompetitiveRankDisplay : MonoBehaviour
{
	// Token: 0x06002D16 RID: 11542 RVA: 0x000EDA36 File Offset: 0x000EBC36
	private void OnEnable()
	{
		VRRig.LocalRig.OnRankedSubtierChanged += this.HandleRankedSubtierChanged;
		this.HandleRankedSubtierChanged(0, 0);
	}

	// Token: 0x06002D17 RID: 11543 RVA: 0x000EDA56 File Offset: 0x000EBC56
	private void OnDisable()
	{
		VRRig.LocalRig.OnRankedSubtierChanged -= this.HandleRankedSubtierChanged;
	}

	// Token: 0x06002D18 RID: 11544 RVA: 0x000EDA70 File Offset: 0x000EBC70
	public void HandleRankedSubtierChanged(int questSubTier, int pcSubTier)
	{
		float currentELO = RankedProgressionManager.Instance.GetCurrentELO();
		int progressionRankIndex = RankedProgressionManager.Instance.GetProgressionRankIndex(currentELO);
		this.UpdateRankIcons(progressionRankIndex);
		this.UpdateRankProgress(RankedProgressionManager.Instance.GetProgressionRankProgress());
	}

	// Token: 0x06002D19 RID: 11545 RVA: 0x000EDAAC File Offset: 0x000EBCAC
	private void UpdateRankIcons(int currentRank)
	{
		this.currentRankSprite.sprite = RankedProgressionManager.Instance.GetProgressionRankIcon(currentRank);
		this.currentRank_Name.text = RankedProgressionManager.Instance.GetProgressionRankName().ToUpper();
		bool flag = currentRank < RankedProgressionManager.Instance.MaxRank;
		bool flag2 = currentRank > 0;
		this.nextRankSprite.gameObject.SetActive(flag);
		this.nextText.gameObject.SetActive(flag);
		this.nextRank_Name.gameObject.SetActive(flag);
		if (flag)
		{
			this.nextRankSprite.sprite = RankedProgressionManager.Instance.GetNextProgressionRankIcon(currentRank);
			this.nextRank_Name.text = RankedProgressionManager.Instance.GetNextProgressionRankName(currentRank).ToUpper();
		}
		this.prevRankSprite.gameObject.SetActive(flag2);
		this.prevText.gameObject.SetActive(flag2);
		this.prevRank_Name.gameObject.SetActive(flag2);
		if (flag2)
		{
			this.prevRankSprite.sprite = RankedProgressionManager.Instance.GetPrevProgressionRankIcon(currentRank);
			this.prevRank_Name.text = RankedProgressionManager.Instance.GetPrevProgressionRankName(currentRank).ToUpper();
		}
	}

	// Token: 0x06002D1A RID: 11546 RVA: 0x000EDBCC File Offset: 0x000EBDCC
	private void UpdateRankProgress(float percent)
	{
		percent = Mathf.Clamp01(percent);
		Vector2 size = this.progressBar.size;
		size.x = this.progressBarSize * percent;
		this.progressBar.size = size;
	}

	// Token: 0x0400385A RID: 14426
	[SerializeField]
	private SpriteRenderer progressBar;

	// Token: 0x0400385B RID: 14427
	[SerializeField]
	private float progressBarSize = 100f;

	// Token: 0x0400385C RID: 14428
	[SerializeField]
	private SpriteRenderer currentRankSprite;

	// Token: 0x0400385D RID: 14429
	[SerializeField]
	private SpriteRenderer prevRankSprite;

	// Token: 0x0400385E RID: 14430
	[SerializeField]
	private SpriteRenderer nextRankSprite;

	// Token: 0x0400385F RID: 14431
	[SerializeField]
	private TextMeshPro currentRank_Name;

	// Token: 0x04003860 RID: 14432
	[SerializeField]
	private TextMeshPro prevText;

	// Token: 0x04003861 RID: 14433
	[SerializeField]
	private TextMeshPro nextText;

	// Token: 0x04003862 RID: 14434
	[SerializeField]
	private TextMeshPro prevRank_Name;

	// Token: 0x04003863 RID: 14435
	[SerializeField]
	private TextMeshPro nextRank_Name;
}
