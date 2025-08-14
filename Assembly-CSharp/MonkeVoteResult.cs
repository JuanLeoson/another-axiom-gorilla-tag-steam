using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200012D RID: 301
public class MonkeVoteResult : MonoBehaviour
{
	// Token: 0x170000C0 RID: 192
	// (get) Token: 0x060007DA RID: 2010 RVA: 0x0002C2ED File Offset: 0x0002A4ED
	// (set) Token: 0x060007DB RID: 2011 RVA: 0x0002C2F8 File Offset: 0x0002A4F8
	public string Text
	{
		get
		{
			return this._text;
		}
		set
		{
			TMP_Text optionText = this._optionText;
			this._text = value;
			optionText.text = value;
		}
	}

	// Token: 0x060007DC RID: 2012 RVA: 0x0002C31C File Offset: 0x0002A51C
	public void ShowResult(string questionOption, int percentage, bool showVote, bool showPrediction, bool isWinner)
	{
		this._optionText.text = questionOption;
		this._optionIndicator.SetActive(true);
		this._scoreText.text = ((percentage >= 0) ? string.Format("{0}%", percentage) : "--");
		this._voteIndicator.SetActive(showVote);
		this._guessWinIndicator.SetActive(showPrediction && isWinner);
		this._guessLoseIndicator.SetActive(showPrediction && !isWinner);
		this._youWinIndicator.SetActive(isWinner && showPrediction);
		this._mostPopularIndicator.SetActive(isWinner);
		this.ShowRockPile(percentage);
	}

	// Token: 0x060007DD RID: 2013 RVA: 0x0002C3C0 File Offset: 0x0002A5C0
	public void HideResult()
	{
		this._optionIndicator.SetActive(false);
		this._voteIndicator.SetActive(false);
		this._guessWinIndicator.SetActive(false);
		this._guessLoseIndicator.SetActive(false);
		this._youWinIndicator.SetActive(false);
		this._mostPopularIndicator.SetActive(false);
		this.ShowRockPile(0);
	}

	// Token: 0x060007DE RID: 2014 RVA: 0x0002C41C File Offset: 0x0002A61C
	private void ShowRockPile(int percentage)
	{
		this._rockPiles.Show(percentage);
	}

	// Token: 0x060007DF RID: 2015 RVA: 0x0002C42C File Offset: 0x0002A62C
	public void SetDynamicMeshesVisible(bool visible)
	{
		this._mostPopularIndicator.SetActive(visible);
		this._voteIndicator.SetActive(visible);
		this._guessWinIndicator.SetActive(visible);
		this._guessLoseIndicator.SetActive(visible);
		this._rockPiles.Show(visible ? 100 : -1);
	}

	// Token: 0x04000974 RID: 2420
	[SerializeField]
	private GameObject _optionIndicator;

	// Token: 0x04000975 RID: 2421
	[SerializeField]
	private TMP_Text _optionText;

	// Token: 0x04000976 RID: 2422
	[FormerlySerializedAs("_scoreLabelPost")]
	[SerializeField]
	private GameObject _scoreIndicator;

	// Token: 0x04000977 RID: 2423
	[SerializeField]
	private TMP_Text _scoreText;

	// Token: 0x04000978 RID: 2424
	[SerializeField]
	private GameObject _voteIndicator;

	// Token: 0x04000979 RID: 2425
	[SerializeField]
	private GameObject _guessWinIndicator;

	// Token: 0x0400097A RID: 2426
	[SerializeField]
	private GameObject _guessLoseIndicator;

	// Token: 0x0400097B RID: 2427
	[SerializeField]
	private GameObject _mostPopularIndicator;

	// Token: 0x0400097C RID: 2428
	[SerializeField]
	private GameObject _youWinIndicator;

	// Token: 0x0400097D RID: 2429
	[SerializeField]
	private RockPiles _rockPiles;

	// Token: 0x0400097E RID: 2430
	private MonkeVoteMachine _machine;

	// Token: 0x0400097F RID: 2431
	private string _text = string.Empty;

	// Token: 0x04000980 RID: 2432
	private bool _canVote;

	// Token: 0x04000981 RID: 2433
	private float _rockPileHeight;
}
