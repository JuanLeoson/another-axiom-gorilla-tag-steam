using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameObjectScheduling;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000132 RID: 306
public class MonkeBusinessStation : MonoBehaviourPunCallbacks
{
	// Token: 0x060007EF RID: 2031 RVA: 0x0002C724 File Offset: 0x0002A924
	public override void OnEnable()
	{
		base.OnEnable();
		this.FindQuestManager();
		ProgressionController.OnQuestSelectionChanged += this.OnQuestSelectionChanged;
		ProgressionController.OnProgressEvent += this.OnProgress;
		ProgressionController.RequestProgressUpdate();
		this.UpdateCountdownTimers();
	}

	// Token: 0x060007F0 RID: 2032 RVA: 0x0002C75F File Offset: 0x0002A95F
	public override void OnDisable()
	{
		base.OnDisable();
		ProgressionController.OnQuestSelectionChanged -= this.OnQuestSelectionChanged;
		ProgressionController.OnProgressEvent -= this.OnProgress;
	}

	// Token: 0x060007F1 RID: 2033 RVA: 0x0002C789 File Offset: 0x0002A989
	private void FindQuestManager()
	{
		if (!this._questManager)
		{
			this._questManager = Object.FindObjectOfType<RotatingQuestsManager>();
		}
	}

	// Token: 0x060007F2 RID: 2034 RVA: 0x0002C7A3 File Offset: 0x0002A9A3
	private void UpdateCountdownTimers()
	{
		this._dailyCountdown.SetCountdownTime(this._questManager.DailyQuestCountdown);
		this._weeklyCountdown.SetCountdownTime(this._questManager.WeeklyQuestCountdown);
	}

	// Token: 0x060007F3 RID: 2035 RVA: 0x0002C7D1 File Offset: 0x0002A9D1
	private void OnQuestSelectionChanged()
	{
		this.UpdateCountdownTimers();
	}

	// Token: 0x060007F4 RID: 2036 RVA: 0x0002C7D9 File Offset: 0x0002A9D9
	private void OnProgress()
	{
		this.UpdateQuestStatus();
		this.UpdateProgressDisplays();
	}

	// Token: 0x060007F5 RID: 2037 RVA: 0x0002C7E8 File Offset: 0x0002A9E8
	private void UpdateProgressDisplays()
	{
		ValueTuple<int, int, int> progressionData = ProgressionController.GetProgressionData();
		int item = progressionData.Item1;
		int item2 = progressionData.Item2;
		this._weeklyProgress.SetProgress(item, ProgressionController.WeeklyCap);
		if (!this._isUpdatingPointCount)
		{
			this._unclaimedPoints.text = item2.ToString();
			this._claimButton.isOn = (item2 > 0);
		}
		bool flag = item2 > 0;
		this._claimablePointsObject.SetActive(flag);
		this._noClaimablePointsObject.SetActive(!flag);
		this._badgeMount.position = (flag ? this._claimablePointsBadgePosition.position : this._noClaimablePointsBadgePosition.position);
		this._claimButton.gameObject.SetActive(flag);
	}

	// Token: 0x060007F6 RID: 2038 RVA: 0x0002C89C File Offset: 0x0002AA9C
	private void UpdateQuestStatus()
	{
		if (this._lastQuestChange >= RotatingQuestsManager.LastQuestChange)
		{
			return;
		}
		this.FindQuestManager();
		if (this._quests.Count == 0 || this._lastQuestDailyID != RotatingQuestsManager.LastQuestDailyID)
		{
			this.BuildQuestList();
		}
		foreach (QuestDisplay questDisplay in this._quests)
		{
			if (questDisplay.IsChanged)
			{
				questDisplay.UpdateDisplay();
			}
		}
		this._lastQuestChange = Time.frameCount;
		this._lastQuestDailyID = RotatingQuestsManager.LastQuestDailyID;
	}

	// Token: 0x060007F7 RID: 2039 RVA: 0x0002C940 File Offset: 0x0002AB40
	public void RedeemProgress()
	{
		if (this._claimButton.isOn)
		{
			this._isUpdatingPointCount = true;
			ValueTuple<int, int, int> progressionData = ProgressionController.GetProgressionData();
			int item = progressionData.Item2;
			int item2 = progressionData.Item3;
			this._tempUnclaimedPoints = item;
			this._tempTotalPoints = item2;
			this._claimButton.isOn = false;
			ProgressionController.RedeemProgress();
			if (PhotonNetwork.InRoom)
			{
				base.photonView.RPC("BroadcastRedeemQuestPoints", RpcTarget.Others, new object[]
				{
					this._tempUnclaimedPoints
				});
			}
			base.StartCoroutine(this.PerformPointRedemptionSequence());
		}
	}

	// Token: 0x060007F8 RID: 2040 RVA: 0x0002C9CB File Offset: 0x0002ABCB
	private IEnumerator PerformPointRedemptionSequence()
	{
		while (this._tempUnclaimedPoints > 0)
		{
			this._tempUnclaimedPoints--;
			this._tempTotalPoints++;
			this._unclaimedPoints.text = this._tempUnclaimedPoints.ToString();
			if (this._tempUnclaimedPoints == 0)
			{
				this._audioSource.PlayOneShot(this._claimPointFinalSFX);
			}
			else
			{
				this._audioSource.PlayOneShot(this._claimPointDefaultSFX);
			}
			yield return new WaitForSeconds(this._claimDelayPerPoint);
		}
		this._isUpdatingPointCount = false;
		this.UpdateProgressDisplays();
		yield break;
	}

	// Token: 0x060007F9 RID: 2041 RVA: 0x0002C9DC File Offset: 0x0002ABDC
	[PunRPC]
	private void BroadcastRedeemQuestPoints(int redeemedPointCount, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "BroadcastRedeemQuestPoints");
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		RigContainer rigContainer;
		if (photonMessageInfoWrapped.Sender != null && VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			if (!FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 10, (double)Time.unscaledTime))
			{
				return;
			}
			redeemedPointCount = Mathf.Min(redeemedPointCount, 50);
			Coroutine coroutine;
			if (this.perPlayerRedemptionSequence.TryGetValue(info.Sender, out coroutine))
			{
				if (coroutine != null)
				{
					base.StopCoroutine(coroutine);
				}
				this.perPlayerRedemptionSequence.Remove(info.Sender);
			}
			if (base.gameObject.activeInHierarchy)
			{
				Coroutine value = base.StartCoroutine(this.PerformRemotePointRedemptionSequence(info.Sender, redeemedPointCount));
				this.perPlayerRedemptionSequence.Add(info.Sender, value);
			}
		}
	}

	// Token: 0x060007FA RID: 2042 RVA: 0x0002CABC File Offset: 0x0002ACBC
	private IEnumerator PerformRemotePointRedemptionSequence(NetPlayer player, int redeemedPointCount)
	{
		while (redeemedPointCount > 0)
		{
			int num = redeemedPointCount;
			redeemedPointCount = num - 1;
			if (redeemedPointCount == 0)
			{
				this._audioSource.PlayOneShot(this._claimPointFinalSFX);
			}
			else
			{
				this._audioSource.PlayOneShot(this._claimPointDefaultSFX);
			}
			yield return new WaitForSeconds(this._claimDelayPerPoint);
		}
		this.perPlayerRedemptionSequence.Remove(player);
		yield break;
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x0002CADC File Offset: 0x0002ACDC
	private void BuildQuestList()
	{
		this.DestroyQuestList();
		RotatingQuestsManager.RotatingQuestList quests = this._questManager.quests;
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup in quests.DailyQuests)
		{
			foreach (RotatingQuestsManager.RotatingQuest rotatingQuest in rotatingQuestGroup.quests)
			{
				if (rotatingQuest.isQuestActive)
				{
					QuestDisplay questDisplay = Object.Instantiate<QuestDisplay>(this._questDisplayPrefab, this._dailyQuestContainer);
					questDisplay.quest = rotatingQuest;
					this._quests.Add(questDisplay);
				}
			}
		}
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup2 in quests.WeeklyQuests)
		{
			foreach (RotatingQuestsManager.RotatingQuest rotatingQuest2 in rotatingQuestGroup2.quests)
			{
				if (rotatingQuest2.isQuestActive)
				{
					QuestDisplay questDisplay2 = Object.Instantiate<QuestDisplay>(this._questDisplayPrefab, this._weeklyQuestContainer);
					questDisplay2.quest = rotatingQuest2;
					this._quests.Add(questDisplay2);
				}
			}
		}
		foreach (QuestDisplay questDisplay3 in this._quests)
		{
			questDisplay3.UpdateDisplay();
		}
		if (!this._hasBuiltQuestList)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(this._questContainerParent);
			this._hasBuiltQuestList = true;
			return;
		}
		LayoutRebuilder.MarkLayoutForRebuild(this._questContainerParent);
	}

	// Token: 0x060007FC RID: 2044 RVA: 0x0002CCB0 File Offset: 0x0002AEB0
	private void DestroyQuestList()
	{
		MonkeBusinessStation.<DestroyQuestList>g__DestroyChildren|40_0(this._dailyQuestContainer);
		MonkeBusinessStation.<DestroyQuestList>g__DestroyChildren|40_0(this._weeklyQuestContainer);
		this._quests.Clear();
	}

	// Token: 0x060007FE RID: 2046 RVA: 0x0002CD00 File Offset: 0x0002AF00
	[CompilerGenerated]
	internal static void <DestroyQuestList>g__DestroyChildren|40_0(Transform parent)
	{
		for (int i = parent.childCount - 1; i >= 0; i--)
		{
			Object.Destroy(parent.GetChild(i).gameObject);
		}
	}

	// Token: 0x04000990 RID: 2448
	[SerializeField]
	private RectTransform _questContainerParent;

	// Token: 0x04000991 RID: 2449
	[SerializeField]
	private RectTransform _dailyQuestContainer;

	// Token: 0x04000992 RID: 2450
	[SerializeField]
	private RectTransform _weeklyQuestContainer;

	// Token: 0x04000993 RID: 2451
	[SerializeField]
	private QuestDisplay _questDisplayPrefab;

	// Token: 0x04000994 RID: 2452
	[SerializeField]
	private List<QuestDisplay> _quests;

	// Token: 0x04000995 RID: 2453
	[SerializeField]
	private ProgressDisplay _weeklyProgress;

	// Token: 0x04000996 RID: 2454
	[SerializeField]
	private TMP_Text _unclaimedPoints;

	// Token: 0x04000997 RID: 2455
	[SerializeField]
	private GorillaPressableButton _claimButton;

	// Token: 0x04000998 RID: 2456
	[SerializeField]
	private AudioSource _audioSource;

	// Token: 0x04000999 RID: 2457
	[SerializeField]
	private GameObject _claimablePointsObject;

	// Token: 0x0400099A RID: 2458
	[SerializeField]
	private GameObject _noClaimablePointsObject;

	// Token: 0x0400099B RID: 2459
	[SerializeField]
	private Transform _claimablePointsBadgePosition;

	// Token: 0x0400099C RID: 2460
	[SerializeField]
	private Transform _noClaimablePointsBadgePosition;

	// Token: 0x0400099D RID: 2461
	[SerializeField]
	private Transform _badgeMount;

	// Token: 0x0400099E RID: 2462
	[Space]
	[SerializeField]
	private float _claimDelayPerPoint = 0.12f;

	// Token: 0x0400099F RID: 2463
	[SerializeField]
	private AudioClip _claimPointDefaultSFX;

	// Token: 0x040009A0 RID: 2464
	[SerializeField]
	private AudioClip _claimPointFinalSFX;

	// Token: 0x040009A1 RID: 2465
	[Header("Quest Timers")]
	[SerializeField]
	private CountdownText _dailyCountdown;

	// Token: 0x040009A2 RID: 2466
	[SerializeField]
	private CountdownText _weeklyCountdown;

	// Token: 0x040009A3 RID: 2467
	private RotatingQuestsManager _questManager;

	// Token: 0x040009A4 RID: 2468
	private int _lastQuestChange = -1;

	// Token: 0x040009A5 RID: 2469
	private int _lastQuestDailyID = -1;

	// Token: 0x040009A6 RID: 2470
	private bool _isUpdatingPointCount;

	// Token: 0x040009A7 RID: 2471
	private int _tempUnclaimedPoints;

	// Token: 0x040009A8 RID: 2472
	private int _tempTotalPoints;

	// Token: 0x040009A9 RID: 2473
	private bool _hasBuiltQuestList;

	// Token: 0x040009AA RID: 2474
	private Dictionary<NetPlayer, Coroutine> perPlayerRedemptionSequence = new Dictionary<NetPlayer, Coroutine>();
}
