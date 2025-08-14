using System;
using System.Collections.Generic;
using KID.Model;
using UnityEngine;

// Token: 0x02000929 RID: 2345
public class KIDUI_AgeAppealController : MonoBehaviour
{
	// Token: 0x1700059A RID: 1434
	// (get) Token: 0x060039E3 RID: 14819 RVA: 0x0012B534 File Offset: 0x00129734
	public static KIDUI_AgeAppealController Instance
	{
		get
		{
			return KIDUI_AgeAppealController._instance;
		}
	}

	// Token: 0x060039E4 RID: 14820 RVA: 0x0012B53B File Offset: 0x0012973B
	private void Awake()
	{
		KIDUI_AgeAppealController._instance = this;
		Debug.LogFormat("[KID::UI::AGEAPPEALCONTROLLER] Controller Initialised", Array.Empty<object>());
	}

	// Token: 0x060039E5 RID: 14821 RVA: 0x0012B554 File Offset: 0x00129754
	public void StartAgeAppealScreens(SessionStatus? sessionStatus)
	{
		Debug.LogFormat("[KID::UI::AGEAPPEALCONTROLLER] Showing k-ID Age Appeal Screens", Array.Empty<object>());
		HandRayController.Instance.EnableHandRays();
		PrivateUIRoom.AddUI(base.transform);
		this._firstAgeAppealScreen.ShowRestrictedAccessScreen(sessionStatus);
		AgeStatusType ageStatusType;
		if (KIDManager.TryGetAgeStatusTypeFromAge(KIDAgeGate.UserAge, out ageStatusType))
		{
			GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
			{
				EventName = "kid_age_appeal",
				CustomTags = new string[]
				{
					"kid_age_appeal",
					KIDTelemetry.GameVersionCustomTag,
					KIDTelemetry.GameEnvironment
				},
				BodyData = new Dictionary<string, string>
				{
					{
						"submitted_age",
						ageStatusType.ToString()
					}
				}
			});
		}
	}

	// Token: 0x060039E6 RID: 14822 RVA: 0x0012B602 File Offset: 0x00129802
	public void CloseKIDScreens()
	{
		PrivateUIRoom.RemoveUI(base.transform);
		HandRayController.Instance.DisableHandRays();
		this._firstAgeAppealScreen.gameObject.SetActive(false);
		Object.DestroyImmediate(base.gameObject);
	}

	// Token: 0x060039E7 RID: 14823 RVA: 0x0012B638 File Offset: 0x00129838
	public void StartTooYoungToPlayScreen()
	{
		Debug.LogFormat("[KID::UI::AGEAPPEALCONTROLLER] Showing k-ID Too Young to Play Screen", Array.Empty<object>());
		HandRayController.Instance.EnableHandRays();
		PrivateUIRoom.AddUI(base.transform);
		this._tooYoungToPlayScreen.ShowTooYoungToPlayScreen();
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
					"blocked"
				}
			}
		});
	}

	// Token: 0x060039E8 RID: 14824 RVA: 0x000A1481 File Offset: 0x0009F681
	public void OnQuitGamePressed()
	{
		Application.Quit();
	}

	// Token: 0x040046FE RID: 18174
	private static KIDUI_AgeAppealController _instance;

	// Token: 0x040046FF RID: 18175
	[SerializeField]
	private KIDUI_RestrictedAccessScreen _firstAgeAppealScreen;

	// Token: 0x04004700 RID: 18176
	[SerializeField]
	private KIDUI_TooYoungToPlay _tooYoungToPlayScreen;
}
