using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000941 RID: 2369
public class KIDUI_EmailSuccess : MonoBehaviour
{
	// Token: 0x06003A49 RID: 14921 RVA: 0x0012D6AC File Offset: 0x0012B8AC
	public void ShowSuccessScreen(string email)
	{
		this._emailTxt.text = email;
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
					"email_sent"
				}
			}
		});
	}

	// Token: 0x06003A4A RID: 14922 RVA: 0x0012D72C File Offset: 0x0012B92C
	public void ShowSuccessScreenAppeal(string email)
	{
		this._emailTxt.text = email;
		base.gameObject.SetActive(true);
		GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
		{
			EventName = "kid_screen_shown",
			CustomTags = new string[]
			{
				"kid_age_appeal",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string>
			{
				{
					"screen",
					"age_appeal_email_sent"
				}
			}
		});
	}

	// Token: 0x06003A4B RID: 14923 RVA: 0x0012D7AC File Offset: 0x0012B9AC
	public void OnClose()
	{
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.Pending);
	}

	// Token: 0x06003A4C RID: 14924 RVA: 0x000A1481 File Offset: 0x0009F681
	public void OnCloseGame()
	{
		Application.Quit();
	}

	// Token: 0x0400478B RID: 18315
	[SerializeField]
	private TMP_Text _emailTxt;

	// Token: 0x0400478C RID: 18316
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;
}
