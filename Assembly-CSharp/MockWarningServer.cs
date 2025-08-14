using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000904 RID: 2308
internal class MockWarningServer : WarningsServer
{
	// Token: 0x1700058E RID: 1422
	// (get) Token: 0x06003910 RID: 14608 RVA: 0x00127BB3 File Offset: 0x00125DB3
	public static string ShownScreenPlayerPref
	{
		get
		{
			return "screen-shown-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		}
	}

	// Token: 0x06003911 RID: 14609 RVA: 0x00127BCB File Offset: 0x00125DCB
	private void Awake()
	{
		if (WarningsServer.Instance == null)
		{
			WarningsServer.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06003912 RID: 14610 RVA: 0x00127BEC File Offset: 0x00125DEC
	private PlayerAgeGateWarningStatus CreateWarningStatus(string header, string body, MockWarningServer.ButtonSetup? leftButtonSetup, MockWarningServer.ButtonSetup? rightButtonSetup, EImageVisibility showImage, Action leftButtonCallback, Action rightButtonCallback)
	{
		PlayerAgeGateWarningStatus result;
		result.header = header;
		result.body = body;
		result.leftButtonText = string.Empty;
		result.rightButtonText = string.Empty;
		result.leftButtonResult = WarningButtonResult.None;
		result.rightButtonResult = WarningButtonResult.None;
		result.showImage = showImage;
		result.onLeftButtonPressedAction = leftButtonCallback;
		result.onRightButtonPressedAction = rightButtonCallback;
		if (leftButtonSetup != null)
		{
			result.leftButtonText = leftButtonSetup.Value.buttonText;
			result.leftButtonResult = leftButtonSetup.Value.buttonResult;
		}
		if (rightButtonSetup != null)
		{
			result.rightButtonText = rightButtonSetup.Value.buttonText;
			result.rightButtonResult = rightButtonSetup.Value.buttonResult;
		}
		return result;
	}

	// Token: 0x06003913 RID: 14611 RVA: 0x00127CAC File Offset: 0x00125EAC
	public override Task<PlayerAgeGateWarningStatus?> FetchPlayerData(CancellationToken token)
	{
		MockWarningServer.<FetchPlayerData>d__6 <FetchPlayerData>d__;
		<FetchPlayerData>d__.<>t__builder = AsyncTaskMethodBuilder<PlayerAgeGateWarningStatus?>.Create();
		<FetchPlayerData>d__.<>4__this = this;
		<FetchPlayerData>d__.token = token;
		<FetchPlayerData>d__.<>1__state = -1;
		<FetchPlayerData>d__.<>t__builder.Start<MockWarningServer.<FetchPlayerData>d__6>(ref <FetchPlayerData>d__);
		return <FetchPlayerData>d__.<>t__builder.Task;
	}

	// Token: 0x06003914 RID: 14612 RVA: 0x00127CF8 File Offset: 0x00125EF8
	public override Task<PlayerAgeGateWarningStatus?> GetOptInFollowUpMessage(CancellationToken token)
	{
		MockWarningServer.<GetOptInFollowUpMessage>d__7 <GetOptInFollowUpMessage>d__;
		<GetOptInFollowUpMessage>d__.<>t__builder = AsyncTaskMethodBuilder<PlayerAgeGateWarningStatus?>.Create();
		<GetOptInFollowUpMessage>d__.<>4__this = this;
		<GetOptInFollowUpMessage>d__.token = token;
		<GetOptInFollowUpMessage>d__.<>1__state = -1;
		<GetOptInFollowUpMessage>d__.<>t__builder.Start<MockWarningServer.<GetOptInFollowUpMessage>d__7>(ref <GetOptInFollowUpMessage>d__);
		return <GetOptInFollowUpMessage>d__.<>t__builder.Task;
	}

	// Token: 0x06003915 RID: 14613 RVA: 0x00127D43 File Offset: 0x00125F43
	private bool ShouldShowWarningScreen(int phase, bool inOptInCohort)
	{
		if (PlayerPrefs.GetInt(string.Format("phase-{0}-{1}", phase, MockWarningServer.ShownScreenPlayerPref), 0) == 0)
		{
			return true;
		}
		switch (phase)
		{
		default:
			return false;
		case 2:
			return inOptInCohort;
		case 3:
		case 4:
			return true;
		}
	}

	// Token: 0x04004631 RID: 17969
	private const string SHOWN_SCREEN_PREFIX = "screen-shown-";

	// Token: 0x02000905 RID: 2309
	public struct ButtonSetup
	{
		// Token: 0x06003917 RID: 14615 RVA: 0x00127D89 File Offset: 0x00125F89
		public ButtonSetup(string txt, WarningButtonResult result)
		{
			this.buttonText = txt;
			this.buttonResult = result;
		}

		// Token: 0x04004632 RID: 17970
		public string buttonText;

		// Token: 0x04004633 RID: 17971
		public WarningButtonResult buttonResult;
	}
}
