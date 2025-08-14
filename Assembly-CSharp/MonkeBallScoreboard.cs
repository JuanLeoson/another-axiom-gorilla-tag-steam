using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x02000508 RID: 1288
public class MonkeBallScoreboard : MonoBehaviour
{
	// Token: 0x06001F70 RID: 8048 RVA: 0x000A664E File Offset: 0x000A484E
	public void Setup(MonkeBallGame game)
	{
		this.game = game;
	}

	// Token: 0x06001F71 RID: 8049 RVA: 0x000A6658 File Offset: 0x000A4858
	public void RefreshScore()
	{
		for (int i = 0; i < this.game.team.Count; i++)
		{
			this.teamDisplays[i].scoreLabel.text = this.game.team[i].score.ToString();
		}
	}

	// Token: 0x06001F72 RID: 8050 RVA: 0x000A66AD File Offset: 0x000A48AD
	public void RefreshTeamPlayers(int teamId, int numPlayers)
	{
		this.teamDisplays[teamId].playersLabel.text = string.Format("PLAYERS: {0}", Mathf.Clamp(numPlayers, 0, 99));
	}

	// Token: 0x06001F73 RID: 8051 RVA: 0x000A66D9 File Offset: 0x000A48D9
	public void PlayScoreFx()
	{
		this.PlayFX(this.scoreSound, this.scoreSoundVolume);
	}

	// Token: 0x06001F74 RID: 8052 RVA: 0x000A66ED File Offset: 0x000A48ED
	public void PlayPlayerJoinFx()
	{
		this.PlayFX(this.playerJoinSound, 0.5f);
	}

	// Token: 0x06001F75 RID: 8053 RVA: 0x000A6700 File Offset: 0x000A4900
	public void PlayPlayerLeaveFx()
	{
		this.PlayFX(this.playerLeaveSound, 0.5f);
	}

	// Token: 0x06001F76 RID: 8054 RVA: 0x000A6713 File Offset: 0x000A4913
	public void PlayGameStartFx()
	{
		this.PlayFX(this.gameStartSound, this.gameStartVolume);
	}

	// Token: 0x06001F77 RID: 8055 RVA: 0x000A6727 File Offset: 0x000A4927
	public void PlayGameEndFx()
	{
		this.PlayFX(this.gameEndSound, this.gameEndVolume);
	}

	// Token: 0x06001F78 RID: 8056 RVA: 0x000A673B File Offset: 0x000A493B
	private void PlayFX(AudioClip clip, float volume)
	{
		if (this.audioSource != null)
		{
			this.audioSource.clip = clip;
			this.audioSource.volume = volume;
			this.audioSource.Play();
		}
	}

	// Token: 0x06001F79 RID: 8057 RVA: 0x000A6770 File Offset: 0x000A4970
	public void RefreshTime()
	{
		float a = (float)(this.game.gameEndTime - PhotonNetwork.Time);
		if (this.game.gameEndTime < 0.0)
		{
			a = 0f;
		}
		a = Mathf.Max(a, 0f);
		if (this._frameIndex == 0)
		{
			this.timeRemainingLabel.text = a.ToString("#00.00");
		}
		this._frameIndex++;
		if (this._frameIndex > 2)
		{
			this._frameIndex = 0;
		}
	}

	// Token: 0x04002802 RID: 10242
	private MonkeBallGame game;

	// Token: 0x04002803 RID: 10243
	public MonkeBallScoreboard.TeamDisplay[] teamDisplays;

	// Token: 0x04002804 RID: 10244
	public TextMeshPro timeRemainingLabel;

	// Token: 0x04002805 RID: 10245
	public AudioSource audioSource;

	// Token: 0x04002806 RID: 10246
	public AudioClip scoreSound;

	// Token: 0x04002807 RID: 10247
	public float scoreSoundVolume;

	// Token: 0x04002808 RID: 10248
	public AudioClip playerJoinSound;

	// Token: 0x04002809 RID: 10249
	public AudioClip playerLeaveSound;

	// Token: 0x0400280A RID: 10250
	public AudioClip gameStartSound;

	// Token: 0x0400280B RID: 10251
	public float gameStartVolume;

	// Token: 0x0400280C RID: 10252
	public AudioClip gameEndSound;

	// Token: 0x0400280D RID: 10253
	public float gameEndVolume;

	// Token: 0x0400280E RID: 10254
	private int _frameIndex;

	// Token: 0x02000509 RID: 1289
	[Serializable]
	public class TeamDisplay
	{
		// Token: 0x0400280F RID: 10255
		public TextMeshPro nameLabel;

		// Token: 0x04002810 RID: 10256
		public TextMeshPro scoreLabel;

		// Token: 0x04002811 RID: 10257
		public TextMeshPro playersLabel;
	}
}
