using System;
using GorillaNetworking;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000692 RID: 1682
public class GRUIPromotionBot : MonoBehaviour
{
	// Token: 0x06002935 RID: 10549 RVA: 0x000DD954 File Offset: 0x000DBB54
	public string FormattedUserInfo(GRPlayer player)
	{
		int num = GhostReactorProgression.TotalPointsForNextGrade(player.CurrentProgression.redeemedPoints);
		return string.Concat(new string[]
		{
			player.gamePlayer.rig.playerNameVisible,
			"\n",
			GhostReactorProgression.GetTitleName(player.CurrentProgression.redeemedPoints),
			"\n",
			GhostReactorProgression.GetGrade(player.CurrentProgression.redeemedPoints).ToString(),
			"\n",
			player.CurrentProgression.points.ToString(),
			"\n",
			num.ToString(),
			"\n",
			Mathf.Max(0, num - player.CurrentProgression.points).ToString(),
			"\n\n",
			this.EligibleForPromotion(player).ToString()
		});
	}

	// Token: 0x06002936 RID: 10550 RVA: 0x000DDA43 File Offset: 0x000DBC43
	public bool EligibleForPromotion(GRPlayer player)
	{
		return player.CurrentProgression.points - GhostReactorProgression.TotalPointsForNextGrade(player.CurrentProgression.redeemedPoints) >= 0;
	}

	// Token: 0x06002937 RID: 10551 RVA: 0x000DDA68 File Offset: 0x000DBC68
	public void PlayerInteraction(GRPlayer player, GRUIPromotionBot.BotActions action, bool initialize = false)
	{
		if (player == null || !player.gameObject.activeInHierarchy)
		{
			action = GRUIPromotionBot.BotActions.Inert;
		}
		this.SwitchScreenToAction(action);
		switch (action)
		{
		case GRUIPromotionBot.BotActions.Inert:
			this.buttonText.text = this.inertButtonText;
			break;
		case GRUIPromotionBot.BotActions.PlayerInfo:
			this.currentPlayer = player;
			this.buttonText.text = this.requestPromotionText;
			if (this.currentPlayer == null || this.currentPlayer.gamePlayer.rig.OwningNetPlayer == null || !this.currentPlayer.gamePlayer.rig.OwningNetPlayer.InRoom)
			{
				this.PlayerInteraction(null, GRUIPromotionBot.BotActions.Inert, false);
			}
			else
			{
				this.userInfo.text = this.FormattedUserInfo(player);
			}
			break;
		case GRUIPromotionBot.BotActions.PromotionSuccess:
			if (player == null || player != this.currentPlayer || player.gamePlayer.rig.OwningNetPlayer == null || !player.gamePlayer.rig.OwningNetPlayer.InRoom || !this.EligibleForPromotion(this.currentPlayer))
			{
				this.PlayerInteraction(null, GRUIPromotionBot.BotActions.Inert, false);
				return;
			}
			if (this.currentAction != action && !initialize)
			{
				player.AttemptPromotion();
				this.particlesGO.SetActive(false);
				this.particlesGO.SetActive(true);
				this.levelUpSound.Play();
				this.popSound.Play();
			}
			this.promotionText.text = string.Concat(new string[]
			{
				this.promotionTextStr1,
				player.gamePlayer.rig.playerNameVisible,
				this.promotionTextStr2,
				GhostReactorProgression.GetGrade(player.CurrentProgression.redeemedPoints).ToString(),
				" ",
				GhostReactorProgression.GetTitleName(player.CurrentProgression.redeemedPoints),
				this.promotionTextStr3
			});
			this.buttonText.text = this.buttonReturnText;
			GhostReactor.instance.RefreshScoreboards();
			PlayerGameEvents.MiscEvent(GRUIPromotionBot.EVENT_PROMOTED, 1);
			break;
		}
		this.currentAction = action;
	}

	// Token: 0x06002938 RID: 10552 RVA: 0x000DDC7F File Offset: 0x000DBE7F
	public void Refresh()
	{
		this.PlayerInteraction(this.currentPlayer, this.currentAction, false);
	}

	// Token: 0x06002939 RID: 10553 RVA: 0x000DDC94 File Offset: 0x000DBE94
	public void RequestPromotionButton()
	{
		GRUIPromotionBot.BotActions botActions = this.currentAction;
		if (botActions != GRUIPromotionBot.BotActions.PlayerInfo)
		{
			if (botActions != GRUIPromotionBot.BotActions.PromotionSuccess)
			{
				return;
			}
			if (this.currentPlayer != null && this.currentPlayer.gamePlayer.IsLocal())
			{
				GhostReactorManager.instance.PromotionBotActivePlayerRequest(1);
			}
		}
		else if (this.currentPlayer != null && this.currentPlayer.gamePlayer.IsLocal() && this.EligibleForPromotion(this.currentPlayer))
		{
			GhostReactorManager.instance.PromotionBotActivePlayerRequest(2);
			return;
		}
	}

	// Token: 0x0600293A RID: 10554 RVA: 0x000DDD16 File Offset: 0x000DBF16
	public void PlayerSwipedID(int actorNumber)
	{
		if (actorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			UnityEvent onSucceeded = this.scanner.onSucceeded;
			if (onSucceeded != null)
			{
				onSucceeded.Invoke();
			}
			GhostReactorManager.instance.PromotionBotActivePlayerRequest(1);
		}
	}

	// Token: 0x0600293B RID: 10555 RVA: 0x000DDD4C File Offset: 0x000DBF4C
	public void SwitchScreenToAction(GRUIPromotionBot.BotActions action)
	{
		if (this.inertText != null)
		{
			this.inertText.gameObject.SetActive(action == GRUIPromotionBot.BotActions.Inert);
		}
		if (this.userInfo != null)
		{
			this.userInfo.gameObject.SetActive(action == GRUIPromotionBot.BotActions.PlayerInfo);
		}
		if (this.header != null)
		{
			this.header.gameObject.SetActive(action == GRUIPromotionBot.BotActions.PlayerInfo);
		}
		if (this.promotionText != null)
		{
			this.promotionText.gameObject.SetActive(action == GRUIPromotionBot.BotActions.PromotionSuccess);
		}
	}

	// Token: 0x0600293C RID: 10556 RVA: 0x000DDDE1 File Offset: 0x000DBFE1
	public int GetCurrentPlayerActorNumber()
	{
		if (this.currentPlayer == null)
		{
			return -1;
		}
		return this.currentPlayer.gamePlayer.rig.OwningNetPlayer.ActorNumber;
	}

	// Token: 0x0600293D RID: 10557 RVA: 0x000DDE0D File Offset: 0x000DC00D
	public static bool ValidBotAction(GRUIPromotionBot.BotActions action)
	{
		return action == GRUIPromotionBot.BotActions.Inert || action == GRUIPromotionBot.BotActions.PlayerInfo || action == GRUIPromotionBot.BotActions.PromotionSuccess;
	}

	// Token: 0x04003524 RID: 13604
	private static string EVENT_PROMOTED = "GRPromoted";

	// Token: 0x04003525 RID: 13605
	public TMP_Text inertText;

	// Token: 0x04003526 RID: 13606
	public TMP_Text userInfo;

	// Token: 0x04003527 RID: 13607
	public TMP_Text header;

	// Token: 0x04003528 RID: 13608
	public TMP_Text promotionText;

	// Token: 0x04003529 RID: 13609
	public TMP_Text buttonText;

	// Token: 0x0400352A RID: 13610
	public IDCardScanner scanner;

	// Token: 0x0400352B RID: 13611
	public GameObject particlesGO;

	// Token: 0x0400352C RID: 13612
	public AudioSource levelUpSound;

	// Token: 0x0400352D RID: 13613
	public AudioSource popSound;

	// Token: 0x0400352E RID: 13614
	private string defaultText = "-N/A-\n-N/A-\n-N/A-\n-N/A-\n-N/A-\n\n-N/A-";

	// Token: 0x0400352F RID: 13615
	private string promotionTextStr1 = "CONGRATULATIONS\n ";

	// Token: 0x04003530 RID: 13616
	private string promotionTextStr2 = ".\n\nYOU ARE NOW A GRADE ";

	// Token: 0x04003531 RID: 13617
	private string promotionTextStr3 = ".\n\nYOU MAY TAKE TWO UNPAID MINUTES TO CELEBRATE, THEN RETURN TO WORK.";

	// Token: 0x04003532 RID: 13618
	private string inertButtonText = "-";

	// Token: 0x04003533 RID: 13619
	private string buttonReturnText = "-RETURN-";

	// Token: 0x04003534 RID: 13620
	private string requestPromotionText = "REQUEST PROMOTION";

	// Token: 0x04003535 RID: 13621
	public const string newLine = "\n";

	// Token: 0x04003536 RID: 13622
	public GRPlayer currentPlayer;

	// Token: 0x04003537 RID: 13623
	public GRUIPromotionBot.BotActions currentAction;

	// Token: 0x02000693 RID: 1683
	public enum BotActions
	{
		// Token: 0x04003539 RID: 13625
		Inert,
		// Token: 0x0400353A RID: 13626
		PlayerInfo,
		// Token: 0x0400353B RID: 13627
		PromotionSuccess
	}
}
