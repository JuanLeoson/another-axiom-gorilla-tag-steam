using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

// Token: 0x0200094E RID: 2382
public class KIDUI_SetupScreen : MonoBehaviour
{
	// Token: 0x06003A99 RID: 15001 RVA: 0x0012F5A4 File Offset: 0x0012D7A4
	private void Awake()
	{
		if (this._emailInputField == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Email Input Field is NULL", Array.Empty<object>());
			return;
		}
		if (this._confirmScreen == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Confirm Screen is NULL", Array.Empty<object>());
			return;
		}
		if (this._mainScreen == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Main Screen is NULL", Array.Empty<object>());
			return;
		}
	}

	// Token: 0x06003A9A RID: 15002 RVA: 0x0012F60C File Offset: 0x0012D80C
	private void OnEnable()
	{
		string @string = PlayerPrefs.GetString(KIDManager.GetEmailForUserPlayerPrefRef, "");
		this._emailInputField.text = @string;
		this._confirmButton.ResetButton();
		this.OnInputChanged(@string);
	}

	// Token: 0x06003A9B RID: 15003 RVA: 0x0012F647 File Offset: 0x0012D847
	private void OnDisable()
	{
		if (this._keyboard == null)
		{
			return;
		}
		this._keyboard.active = false;
	}

	// Token: 0x06003A9C RID: 15004 RVA: 0x0012F660 File Offset: 0x0012D860
	public void OnStartSetup()
	{
		base.gameObject.SetActive(true);
		GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
		{
			EventName = "kid_screen_shown",
			CustomTags = new string[]
			{
				"kid_setup",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string>
			{
				{
					"screen",
					"enter_email"
				}
			}
		});
	}

	// Token: 0x06003A9D RID: 15005 RVA: 0x0012F6D4 File Offset: 0x0012D8D4
	public void OnInputSelected()
	{
		Debug.LogFormat("[KID::UI::SETUP] Email Input Selected!", Array.Empty<object>());
	}

	// Token: 0x06003A9E RID: 15006 RVA: 0x0012F6E8 File Offset: 0x0012D8E8
	public void OnInputChanged(string newVal)
	{
		bool flag = !string.IsNullOrEmpty(newVal);
		if (flag)
		{
			flag = Regex.IsMatch(newVal, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$");
		}
		this._confirmButton.interactable = flag;
	}

	// Token: 0x06003A9F RID: 15007 RVA: 0x0012F71A File Offset: 0x0012D91A
	public void OnSubmitEmailPressed()
	{
		PlayerPrefs.SetString(KIDManager.GetEmailForUserPlayerPrefRef, this._emailInputField.text);
		PlayerPrefs.Save();
		base.gameObject.SetActive(false);
		this._confirmScreen.OnEmailSubmitted(this._emailInputField.text);
	}

	// Token: 0x06003AA0 RID: 15008 RVA: 0x0012F758 File Offset: 0x0012D958
	public void OnBackPressed()
	{
		PlayerPrefs.SetString(KIDManager.GetEmailForUserPlayerPrefRef, this._emailInputField.text);
		PlayerPrefs.Save();
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.Previous);
	}

	// Token: 0x040047EB RID: 18411
	[SerializeField]
	private TMP_InputField _emailInputField;

	// Token: 0x040047EC RID: 18412
	[SerializeField]
	private KIDUIButton _confirmButton;

	// Token: 0x040047ED RID: 18413
	[SerializeField]
	private KIDUI_ConfirmScreen _confirmScreen;

	// Token: 0x040047EE RID: 18414
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;

	// Token: 0x040047EF RID: 18415
	[SerializeField]
	private TMP_Text _riftKeyboardMessage;

	// Token: 0x040047F0 RID: 18416
	private string _emailStr = string.Empty;

	// Token: 0x040047F1 RID: 18417
	private TouchScreenKeyboard _keyboard;
}
