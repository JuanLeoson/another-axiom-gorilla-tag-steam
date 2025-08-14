using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// Token: 0x0200092B RID: 2347
public class KIDUI_AgeAppealEmailConfirmation : MonoBehaviour
{
	// Token: 0x060039EE RID: 14830 RVA: 0x0012B865 File Offset: 0x00129A65
	private void OnEnable()
	{
		KIDManager.onEmailResultReceived = (KIDManager.OnEmailResultReceived)Delegate.Combine(KIDManager.onEmailResultReceived, new KIDManager.OnEmailResultReceived(this.NotifyOfEmailResult));
	}

	// Token: 0x060039EF RID: 14831 RVA: 0x0012B887 File Offset: 0x00129A87
	private void OnDisable()
	{
		KIDManager.onEmailResultReceived = (KIDManager.OnEmailResultReceived)Delegate.Remove(KIDManager.onEmailResultReceived, new KIDManager.OnEmailResultReceived(this.NotifyOfEmailResult));
	}

	// Token: 0x060039F0 RID: 14832 RVA: 0x0012B8AC File Offset: 0x00129AAC
	public void ShowAgeAppealConfirmationScreen(bool hasChallenge, int newAge, string emailToConfirm)
	{
		this.hasChallenge = hasChallenge;
		this.newAgeToAppeal = newAge;
		this._confirmText.text = (this.hasChallenge ? this.CONFIRM_PARENT_EMAIL : this.CONFIRM_YOUR_EMAIL);
		this._emailText.text = emailToConfirm;
		base.gameObject.SetActive(true);
	}

	// Token: 0x060039F1 RID: 14833 RVA: 0x0012B900 File Offset: 0x00129B00
	public void OnConfirmPressed()
	{
		GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
		{
			EventName = "kid_age_appeal_confirm_email",
			CustomTags = new string[]
			{
				"kid_age_appeal",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string>
			{
				{
					"email_type",
					this.hasChallenge ? "under_dac" : "over_dac"
				},
				{
					"button_pressed",
					"confirm"
				}
			}
		});
		if (this.hasChallenge)
		{
			this.StartAgeAppealChallengeEmail();
			return;
		}
		this.StartAgeAppealEmail();
	}

	// Token: 0x060039F2 RID: 14834 RVA: 0x0012B9A0 File Offset: 0x00129BA0
	public void OnBackPressed()
	{
		GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
		{
			EventName = "kid_age_appeal_confirm_email",
			CustomTags = new string[]
			{
				"kid_age_appeal",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string>
			{
				{
					"email_type",
					this.hasChallenge ? "under_dac" : "over_dac"
				},
				{
					"button_pressed",
					"go_back"
				}
			}
		});
		base.gameObject.SetActive(false);
		this._ageAppealEmailScreen.ShowAgeAppealEmailScreen(this.hasChallenge, this.newAgeToAppeal);
	}

	// Token: 0x060039F3 RID: 14835 RVA: 0x0012BA4C File Offset: 0x00129C4C
	private void StartAgeAppealChallengeEmail()
	{
		KIDUI_AgeAppealEmailConfirmation.<StartAgeAppealChallengeEmail>d__16 <StartAgeAppealChallengeEmail>d__;
		<StartAgeAppealChallengeEmail>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<StartAgeAppealChallengeEmail>d__.<>4__this = this;
		<StartAgeAppealChallengeEmail>d__.<>1__state = -1;
		<StartAgeAppealChallengeEmail>d__.<>t__builder.Start<KIDUI_AgeAppealEmailConfirmation.<StartAgeAppealChallengeEmail>d__16>(ref <StartAgeAppealChallengeEmail>d__);
	}

	// Token: 0x060039F4 RID: 14836 RVA: 0x0012BA84 File Offset: 0x00129C84
	private Task StartAgeAppealEmail()
	{
		KIDUI_AgeAppealEmailConfirmation.<StartAgeAppealEmail>d__17 <StartAgeAppealEmail>d__;
		<StartAgeAppealEmail>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartAgeAppealEmail>d__.<>4__this = this;
		<StartAgeAppealEmail>d__.<>1__state = -1;
		<StartAgeAppealEmail>d__.<>t__builder.Start<KIDUI_AgeAppealEmailConfirmation.<StartAgeAppealEmail>d__17>(ref <StartAgeAppealEmail>d__);
		return <StartAgeAppealEmail>d__.<>t__builder.Task;
	}

	// Token: 0x060039F5 RID: 14837 RVA: 0x0012BAC8 File Offset: 0x00129CC8
	private void NotifyOfEmailResult(bool success)
	{
		if (this._successScreen == null)
		{
			Debug.LogError("[KID::AGE_APPEAL_EMAIL] _successScreen has not been set yet and is NULL. Cannot inform of result");
			return;
		}
		this._hasCompletedSendEmailRequest = true;
		if (success)
		{
			base.gameObject.SetActive(false);
			this._successScreen.ShowSuccessScreenAppeal(this._emailText.text);
			return;
		}
	}

	// Token: 0x060039F6 RID: 14838 RVA: 0x0012BB1B File Offset: 0x00129D1B
	private void ShowErrorScreen()
	{
		Debug.LogErrorFormat("[KID::UI::Setup] K-ID Confirmation Failed - Failed to send email", Array.Empty<object>());
		base.gameObject.SetActive(false);
		this._errorScreen.ShowAgeAppealEmailErrorScreen(this.hasChallenge, this.newAgeToAppeal, this._emailText.text);
	}

	// Token: 0x0400470A RID: 18186
	[SerializeField]
	private TMP_Text _confirmText;

	// Token: 0x0400470B RID: 18187
	[SerializeField]
	private TMP_Text _emailText;

	// Token: 0x0400470C RID: 18188
	private string CONFIRM_PARENT_EMAIL = "Please confirm your parent or guardian's email address.";

	// Token: 0x0400470D RID: 18189
	private string CONFIRM_YOUR_EMAIL = "Please confirm your email address.";

	// Token: 0x0400470E RID: 18190
	private bool hasChallenge = true;

	// Token: 0x0400470F RID: 18191
	private int newAgeToAppeal;

	// Token: 0x04004710 RID: 18192
	private bool _hasCompletedSendEmailRequest;

	// Token: 0x04004711 RID: 18193
	[SerializeField]
	private KIDUI_EmailSuccess _successScreen;

	// Token: 0x04004712 RID: 18194
	[SerializeField]
	private KIDUI_AgeAppealEmailError _errorScreen;

	// Token: 0x04004713 RID: 18195
	[SerializeField]
	private KIDUI_AgeAppealEmailScreen _ageAppealEmailScreen;

	// Token: 0x04004714 RID: 18196
	[SerializeField]
	private int _minimumDelay = 1000;
}
