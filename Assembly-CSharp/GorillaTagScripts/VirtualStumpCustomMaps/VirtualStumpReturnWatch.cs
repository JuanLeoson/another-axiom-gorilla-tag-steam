using System;
using System.Collections;
using GorillaExtensions;
using GorillaGameModes;
using GorillaTagScripts.ModIO;
using GT_CustomMapSupportRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000C68 RID: 3176
	public class VirtualStumpReturnWatch : MonoBehaviour
	{
		// Token: 0x06004EA7 RID: 20135 RVA: 0x001872D8 File Offset: 0x001854D8
		private void Start()
		{
			if (this.returnButton != null)
			{
				this.returnButton.onStartPressingButton.AddListener(new UnityAction(this.OnStartedPressingButton));
				this.returnButton.onStopPressingButton.AddListener(new UnityAction(this.OnStoppedPressingButton));
				this.returnButton.onPressButton.AddListener(new UnityAction(this.OnButtonPressed));
			}
		}

		// Token: 0x06004EA8 RID: 20136 RVA: 0x00187348 File Offset: 0x00185548
		private void OnDestroy()
		{
			if (this.returnButton != null)
			{
				this.returnButton.onStartPressingButton.RemoveListener(new UnityAction(this.OnStartedPressingButton));
				this.returnButton.onStopPressingButton.RemoveListener(new UnityAction(this.OnStoppedPressingButton));
				this.returnButton.onPressButton.RemoveListener(new UnityAction(this.OnButtonPressed));
			}
		}

		// Token: 0x06004EA9 RID: 20137 RVA: 0x001873B8 File Offset: 0x001855B8
		public static void SetWatchProperties(VirtualStumpReturnWatchProps props)
		{
			VirtualStumpReturnWatch.currentCustomMapProps = props;
			VirtualStumpReturnWatch.currentCustomMapProps.holdDuration = Mathf.Clamp(VirtualStumpReturnWatch.currentCustomMapProps.holdDuration, 0.5f, 5f);
			VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Infection = Mathf.Clamp(VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Infection, 0.5f, 5f);
			VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Custom = Mathf.Clamp(VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Custom, 0.5f, 5f);
		}

		// Token: 0x06004EAA RID: 20138 RVA: 0x00187434 File Offset: 0x00185634
		private float GetCurrentHoldDuration()
		{
			switch (GorillaGameManager.instance.GameType())
			{
			case GameModeType.Infection:
				if (VirtualStumpReturnWatch.currentCustomMapProps.infectionOverride)
				{
					return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Infection;
				}
				return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration;
			case GameModeType.Custom:
				if (VirtualStumpReturnWatch.currentCustomMapProps.customModeOverride)
				{
					return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Custom;
				}
				return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration;
			}
			return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration;
		}

		// Token: 0x06004EAB RID: 20139 RVA: 0x001874C6 File Offset: 0x001856C6
		private void OnStartedPressingButton()
		{
			this.startPressingButtonTime = Time.time;
			this.currentlyBeingPressed = true;
			this.returnButton.pressDuration = this.GetCurrentHoldDuration();
			this.ShowCountdownText();
			this.updateCountdownCoroutine = base.StartCoroutine(this.UpdateCountdownText());
		}

		// Token: 0x06004EAC RID: 20140 RVA: 0x00187503 File Offset: 0x00185703
		private void OnStoppedPressingButton()
		{
			this.currentlyBeingPressed = false;
			this.HideCountdownText();
			if (this.updateCountdownCoroutine != null)
			{
				base.StopCoroutine(this.updateCountdownCoroutine);
				this.updateCountdownCoroutine = null;
			}
		}

		// Token: 0x06004EAD RID: 20141 RVA: 0x00187530 File Offset: 0x00185730
		private void OnButtonPressed()
		{
			this.currentlyBeingPressed = false;
			if (ZoneManagement.IsInZone(GTZone.customMaps) && !CustomMapManager.IsLocalPlayerInVirtualStump())
			{
				bool flag = VirtualStumpReturnWatch.currentCustomMapProps.shouldTagPlayer;
				bool flag2 = VirtualStumpReturnWatch.currentCustomMapProps.shouldKickPlayer;
				switch (GorillaGameManager.instance.GameType())
				{
				case GameModeType.Infection:
					if (VirtualStumpReturnWatch.currentCustomMapProps.infectionOverride)
					{
						flag = VirtualStumpReturnWatch.currentCustomMapProps.shouldTagPlayer_Infection;
						flag2 = VirtualStumpReturnWatch.currentCustomMapProps.shouldKickPlayer_Infection;
					}
					break;
				case GameModeType.Custom:
					if (VirtualStumpReturnWatch.currentCustomMapProps.customModeOverride)
					{
						flag = VirtualStumpReturnWatch.currentCustomMapProps.shouldTagPlayer_CustomMode;
						flag2 = VirtualStumpReturnWatch.currentCustomMapProps.shouldKickPlayer_CustomMode;
					}
					break;
				}
				if (flag2 && NetworkSystem.Instance.InRoom && !NetworkSystem.Instance.SessionIsPrivate)
				{
					NetworkSystem.Instance.ReturnToSinglePlayer();
				}
				else if (flag)
				{
					GameMode.ReportHit();
				}
				CustomMapManager.ReturnToVirtualStump();
			}
		}

		// Token: 0x06004EAE RID: 20142 RVA: 0x00187624 File Offset: 0x00185824
		private void ShowCountdownText()
		{
			if (this.countdownText.IsNull())
			{
				return;
			}
			int num = 1 + Mathf.FloorToInt(this.GetCurrentHoldDuration());
			this.countdownText.text = num.ToString();
			this.countdownText.gameObject.SetActive(true);
			if (this.buttonText.IsNotNull())
			{
				this.buttonText.gameObject.SetActive(false);
			}
		}

		// Token: 0x06004EAF RID: 20143 RVA: 0x00187690 File Offset: 0x00185890
		private void HideCountdownText()
		{
			if (this.countdownText.IsNull())
			{
				return;
			}
			this.countdownText.text = "";
			this.countdownText.gameObject.SetActive(false);
			if (this.buttonText.IsNotNull())
			{
				this.buttonText.gameObject.SetActive(true);
			}
		}

		// Token: 0x06004EB0 RID: 20144 RVA: 0x001876EA File Offset: 0x001858EA
		private IEnumerator UpdateCountdownText()
		{
			while (this.currentlyBeingPressed)
			{
				if (this.countdownText.IsNull())
				{
					yield break;
				}
				float f = this.GetCurrentHoldDuration() - (Time.time - this.startPressingButtonTime);
				int num = 1 + Mathf.FloorToInt(f);
				this.countdownText.text = num.ToString();
				yield return null;
			}
			yield break;
		}

		// Token: 0x0400577E RID: 22398
		[SerializeField]
		private HeldButton returnButton;

		// Token: 0x0400577F RID: 22399
		[SerializeField]
		private TMP_Text buttonText;

		// Token: 0x04005780 RID: 22400
		[SerializeField]
		private TMP_Text countdownText;

		// Token: 0x04005781 RID: 22401
		private static VirtualStumpReturnWatchProps currentCustomMapProps;

		// Token: 0x04005782 RID: 22402
		private float startPressingButtonTime = -1f;

		// Token: 0x04005783 RID: 22403
		private bool currentlyBeingPressed;

		// Token: 0x04005784 RID: 22404
		private Coroutine updateCountdownCoroutine;
	}
}
