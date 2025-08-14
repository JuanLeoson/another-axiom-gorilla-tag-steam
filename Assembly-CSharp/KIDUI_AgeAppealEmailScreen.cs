using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

// Token: 0x0200092A RID: 2346
public class KIDUI_AgeAppealEmailScreen : MonoBehaviour
{
	// Token: 0x060039EA RID: 14826 RVA: 0x0012B6D0 File Offset: 0x001298D0
	public void ShowAgeAppealEmailScreen(bool receivedChallenge, int newAge)
	{
		this.newAgeToAppeal = newAge;
		base.gameObject.SetActive(true);
		this.hasChallenge = receivedChallenge;
		this._enterEmailText.text = (this.hasChallenge ? this.PARENT_EMAIL_DESCRIPTION : this.VERIFY_AGE_EMAIL_DESCRIPTION);
		if (this._parentPermissionNotice)
		{
			this._parentPermissionNotice.SetActive(this.hasChallenge);
		}
		this.OnInputChanged(this._emailText.text);
		GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
		{
			EventName = "kid_age_appeal_enter_email",
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
				}
			}
		});
	}

	// Token: 0x060039EB RID: 14827 RVA: 0x0012B7B4 File Offset: 0x001299B4
	public void OnInputChanged(string newVal)
	{
		bool flag = !string.IsNullOrEmpty(newVal);
		if (flag)
		{
			flag = Regex.IsMatch(newVal, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$");
		}
		this._confirmButton.interactable = flag;
	}

	// Token: 0x060039EC RID: 14828 RVA: 0x0012B7E8 File Offset: 0x001299E8
	public void OnConfirmPressed()
	{
		if (string.IsNullOrEmpty(this._emailText.text))
		{
			Debug.LogError("[KID::UI::APPEAL_AGE_EMAIL] Age Appeal Email Text is empty");
			return;
		}
		this._confirmationScreen.ShowAgeAppealConfirmationScreen(this.hasChallenge, this.newAgeToAppeal, this._emailText.text);
		base.gameObject.SetActive(false);
	}

	// Token: 0x04004701 RID: 18177
	[SerializeField]
	private KIDUIButton _confirmButton;

	// Token: 0x04004702 RID: 18178
	[SerializeField]
	private KIDUI_AgeAppealEmailConfirmation _confirmationScreen;

	// Token: 0x04004703 RID: 18179
	[SerializeField]
	private TMP_Text _enterEmailText;

	// Token: 0x04004704 RID: 18180
	[SerializeField]
	private TMP_InputField _emailText;

	// Token: 0x04004705 RID: 18181
	[SerializeField]
	private GameObject _parentPermissionNotice;

	// Token: 0x04004706 RID: 18182
	private string PARENT_EMAIL_DESCRIPTION = "Enter your parent or guardian's email address below.";

	// Token: 0x04004707 RID: 18183
	private string VERIFY_AGE_EMAIL_DESCRIPTION = "Enter your email address below";

	// Token: 0x04004708 RID: 18184
	private bool hasChallenge = true;

	// Token: 0x04004709 RID: 18185
	private int newAgeToAppeal;
}
