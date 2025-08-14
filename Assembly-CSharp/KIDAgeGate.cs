using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// Token: 0x020008CA RID: 2250
public class KIDAgeGate : MonoBehaviour
{
	// Token: 0x1700056E RID: 1390
	// (get) Token: 0x06003807 RID: 14343 RVA: 0x00121436 File Offset: 0x0011F636
	public static int UserAge
	{
		get
		{
			return KIDAgeGate._ageValue;
		}
	}

	// Token: 0x1700056F RID: 1391
	// (get) Token: 0x06003808 RID: 14344 RVA: 0x0012143D File Offset: 0x0011F63D
	// (set) Token: 0x06003809 RID: 14345 RVA: 0x00121444 File Offset: 0x0011F644
	public static bool DisplayedScreen { get; private set; }

	// Token: 0x0600380A RID: 14346 RVA: 0x0012144C File Offset: 0x0011F64C
	private void Awake()
	{
		if (KIDAgeGate._activeReference != null)
		{
			Debug.LogError("[KID::Age_Gate] Age Gate already exists, this is a duplicate, deleting the new one");
			Object.DestroyImmediate(base.gameObject);
			return;
		}
		KIDAgeGate._activeReference = this;
	}

	// Token: 0x0600380B RID: 14347 RVA: 0x00121478 File Offset: 0x0011F678
	private void Start()
	{
		KIDAgeGate.<Start>d__29 <Start>d__;
		<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Start>d__.<>1__state = -1;
		<Start>d__.<>t__builder.Start<KIDAgeGate.<Start>d__29>(ref <Start>d__);
	}

	// Token: 0x0600380C RID: 14348 RVA: 0x001214A7 File Offset: 0x0011F6A7
	private void OnDestroy()
	{
		this.requestCancellationSource.Cancel();
	}

	// Token: 0x0600380D RID: 14349 RVA: 0x001214B4 File Offset: 0x0011F6B4
	public static Task BeginAgeGate()
	{
		KIDAgeGate.<BeginAgeGate>d__31 <BeginAgeGate>d__;
		<BeginAgeGate>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<BeginAgeGate>d__.<>1__state = -1;
		<BeginAgeGate>d__.<>t__builder.Start<KIDAgeGate.<BeginAgeGate>d__31>(ref <BeginAgeGate>d__);
		return <BeginAgeGate>d__.<>t__builder.Task;
	}

	// Token: 0x0600380E RID: 14350 RVA: 0x001214F0 File Offset: 0x0011F6F0
	private Task StartAgeGate()
	{
		KIDAgeGate.<StartAgeGate>d__32 <StartAgeGate>d__;
		<StartAgeGate>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartAgeGate>d__.<>4__this = this;
		<StartAgeGate>d__.<>1__state = -1;
		<StartAgeGate>d__.<>t__builder.Start<KIDAgeGate.<StartAgeGate>d__32>(ref <StartAgeGate>d__);
		return <StartAgeGate>d__.<>t__builder.Task;
	}

	// Token: 0x0600380F RID: 14351 RVA: 0x00121534 File Offset: 0x0011F734
	private Task InitialiseAgeGate()
	{
		KIDAgeGate.<InitialiseAgeGate>d__33 <InitialiseAgeGate>d__;
		<InitialiseAgeGate>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<InitialiseAgeGate>d__.<>4__this = this;
		<InitialiseAgeGate>d__.<>1__state = -1;
		<InitialiseAgeGate>d__.<>t__builder.Start<KIDAgeGate.<InitialiseAgeGate>d__33>(ref <InitialiseAgeGate>d__);
		return <InitialiseAgeGate>d__.<>t__builder.Task;
	}

	// Token: 0x06003810 RID: 14352 RVA: 0x00121578 File Offset: 0x0011F778
	private Task ProcessAgeGate()
	{
		KIDAgeGate.<ProcessAgeGate>d__34 <ProcessAgeGate>d__;
		<ProcessAgeGate>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<ProcessAgeGate>d__.<>4__this = this;
		<ProcessAgeGate>d__.<>1__state = -1;
		<ProcessAgeGate>d__.<>t__builder.Start<KIDAgeGate.<ProcessAgeGate>d__34>(ref <ProcessAgeGate>d__);
		return <ProcessAgeGate>d__.<>t__builder.Task;
	}

	// Token: 0x06003811 RID: 14353 RVA: 0x001215BC File Offset: 0x0011F7BC
	private Task<bool> ProcessAgeGateConfirmation()
	{
		KIDAgeGate.<ProcessAgeGateConfirmation>d__35 <ProcessAgeGateConfirmation>d__;
		<ProcessAgeGateConfirmation>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<ProcessAgeGateConfirmation>d__.<>4__this = this;
		<ProcessAgeGateConfirmation>d__.<>1__state = -1;
		<ProcessAgeGateConfirmation>d__.<>t__builder.Start<KIDAgeGate.<ProcessAgeGateConfirmation>d__35>(ref <ProcessAgeGateConfirmation>d__);
		return <ProcessAgeGateConfirmation>d__.<>t__builder.Task;
	}

	// Token: 0x06003812 RID: 14354 RVA: 0x00121600 File Offset: 0x0011F800
	private Task WaitForAgeChoice()
	{
		KIDAgeGate.<WaitForAgeChoice>d__36 <WaitForAgeChoice>d__;
		<WaitForAgeChoice>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForAgeChoice>d__.<>4__this = this;
		<WaitForAgeChoice>d__.<>1__state = -1;
		<WaitForAgeChoice>d__.<>t__builder.Start<KIDAgeGate.<WaitForAgeChoice>d__36>(ref <WaitForAgeChoice>d__);
		return <WaitForAgeChoice>d__.<>t__builder.Task;
	}

	// Token: 0x06003813 RID: 14355 RVA: 0x00121643 File Offset: 0x0011F843
	public static void OnConfirmAgePressed(int currentAge)
	{
		KIDAgeGate._hasChosenAge = true;
	}

	// Token: 0x06003814 RID: 14356 RVA: 0x0012164C File Offset: 0x0011F84C
	private Task OnAgeGateCompleted()
	{
		KIDAgeGate.<OnAgeGateCompleted>d__38 <OnAgeGateCompleted>d__;
		<OnAgeGateCompleted>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<OnAgeGateCompleted>d__.<>4__this = this;
		<OnAgeGateCompleted>d__.<>1__state = -1;
		<OnAgeGateCompleted>d__.<>t__builder.Start<KIDAgeGate.<OnAgeGateCompleted>d__38>(ref <OnAgeGateCompleted>d__);
		return <OnAgeGateCompleted>d__.<>t__builder.Task;
	}

	// Token: 0x06003815 RID: 14357 RVA: 0x0012168F File Offset: 0x0011F88F
	private void FinaliseAgeGateAndContinue()
	{
		if (this.requestCancellationSource.IsCancellationRequested)
		{
			return;
		}
		Debug.Log("[KID::AGE_GATE] Age gate completed");
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06003816 RID: 14358 RVA: 0x001216B4 File Offset: 0x0011F8B4
	private void QuitGame()
	{
		Debug.Log("[KID] QUIT PRESSED");
		Application.Quit();
	}

	// Token: 0x06003817 RID: 14359 RVA: 0x001216C8 File Offset: 0x0011F8C8
	private void AppealAge()
	{
		KIDAgeGate.<AppealAge>d__41 <AppealAge>d__;
		<AppealAge>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<AppealAge>d__.<>4__this = this;
		<AppealAge>d__.<>1__state = -1;
		<AppealAge>d__.<>t__builder.Start<KIDAgeGate.<AppealAge>d__41>(ref <AppealAge>d__);
	}

	// Token: 0x06003818 RID: 14360 RVA: 0x00121700 File Offset: 0x0011F900
	private void AppealRejected()
	{
		Debug.Log("[KID] APPEAL REJECTED");
		string messageTitle = "UNDER AGE";
		string messageBody = "Your VR platform requires a certain minimum age to play Gorilla Tag. Unfortunately, due to those age requirements, we cannot allow you to play Gorilla Tag at this time.\n\nIf you incorrectly submitted your age, please appeal.";
		string messageConfirmation = "Hold any face button to appeal";
		this._pregameMessageReference.ShowMessage(messageTitle, messageBody, messageConfirmation, new Action(this.AppealAge), 0.25f, 0f);
	}

	// Token: 0x06003819 RID: 14361 RVA: 0x000023F5 File Offset: 0x000005F5
	private void RefreshChallengeStatus()
	{
	}

	// Token: 0x0600381A RID: 14362 RVA: 0x0012174D File Offset: 0x0011F94D
	public static void SetAgeGateConfig(GetRequirementsData response)
	{
		KIDAgeGate._ageGateConfig = response;
	}

	// Token: 0x0600381B RID: 14363 RVA: 0x00121758 File Offset: 0x0011F958
	public void OnWhyAgeGateButtonPressed()
	{
		GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
		{
			EventName = "kid_screen_shown",
			CustomTags = new string[]
			{
				"kid_age_gate",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string>
			{
				{
					"screen",
					"why_age_gate"
				}
			}
		});
		this._uiParent.SetActive(false);
		PrivateUIRoom.AddUI(this._whyAgeGateScreen.transform);
		this._whyAgeGateScreen.SetActive(true);
	}

	// Token: 0x0600381C RID: 14364 RVA: 0x001217E8 File Offset: 0x0011F9E8
	public void OnWhyAgeGateButtonBackPressed()
	{
		this._uiParent.SetActive(true);
		PrivateUIRoom.RemoveUI(this._whyAgeGateScreen.transform);
		this._whyAgeGateScreen.SetActive(false);
	}

	// Token: 0x0600381D RID: 14365 RVA: 0x00121814 File Offset: 0x0011FA14
	public void OnLearnMoreAboutKIDPressed()
	{
		this._metrics_LearnMorePressed = true;
		GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
		{
			EventName = "kid_screen_shown",
			CustomTags = new string[]
			{
				"kid_age_gate",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string>
			{
				{
					"screen",
					"learn_more_url"
				}
			}
		});
		Application.OpenURL("https://whyagegate.com/");
	}

	// Token: 0x040044CD RID: 17613
	private const string LEARN_MORE_URL = "https://whyagegate.com/";

	// Token: 0x040044CE RID: 17614
	private const string DEFAULT_AGE_VALUE_STRING = "SET AGE";

	// Token: 0x040044CF RID: 17615
	private const int MINIMUM_PLATFORM_AGE = 13;

	// Token: 0x040044D0 RID: 17616
	[Header("Age Gate Settings")]
	[SerializeField]
	private PreGameMessage _pregameMessageReference;

	// Token: 0x040044D1 RID: 17617
	[SerializeField]
	private KIDUI_AgeDiscrepancyScreen _ageDiscrepancyScreen;

	// Token: 0x040044D2 RID: 17618
	[SerializeField]
	private GameObject _uiParent;

	// Token: 0x040044D3 RID: 17619
	[SerializeField]
	private AgeSliderWithProgressBar _ageSlider;

	// Token: 0x040044D4 RID: 17620
	[SerializeField]
	private GameObject _confirmationUI;

	// Token: 0x040044D5 RID: 17621
	[SerializeField]
	private KIDAgeGateConfirmation _confirmationUIManager;

	// Token: 0x040044D6 RID: 17622
	[SerializeField]
	private TMP_Text _confirmationAgeText;

	// Token: 0x040044D7 RID: 17623
	[SerializeField]
	private GameObject _whyAgeGateScreen;

	// Token: 0x040044D8 RID: 17624
	private const string strBlockAccessTitle = "UNDER AGE";

	// Token: 0x040044D9 RID: 17625
	private const string strBlockAccessMessage = "Your VR platform requires a certain minimum age to play Gorilla Tag. Unfortunately, due to those age requirements, we cannot allow you to play Gorilla Tag at this time.\n\nIf you incorrectly submitted your age, please appeal.";

	// Token: 0x040044DA RID: 17626
	private const string strBlockAccessConfirm = "Hold any face button to appeal";

	// Token: 0x040044DB RID: 17627
	private const string strVerifyAgeTitle = "VERIFY AGE";

	// Token: 0x040044DC RID: 17628
	private const string strVerifyAgeMessage = "GETTING ONE TIME PASSCODE. PLEASE WAIT.\n\nGIVE IT TO A PARENT/GUARDIAN TO ENTER IT AT: k-id.com/code";

	// Token: 0x040044DD RID: 17629
	private const string strDiscrepancyMessage = "You entered {0} for your age,\nbut your Meta account says you should be {1}. You could be logged into the wrong Meta account on this device.\n\nWe will use the lowest age ({2})\nif you Continue.";

	// Token: 0x040044DE RID: 17630
	private static KIDAgeGate _activeReference;

	// Token: 0x040044DF RID: 17631
	private static GetRequirementsData _ageGateConfig;

	// Token: 0x040044E0 RID: 17632
	private static int _ageValue;

	// Token: 0x040044E1 RID: 17633
	private CancellationTokenSource requestCancellationSource = new CancellationTokenSource();

	// Token: 0x040044E2 RID: 17634
	private static bool _hasChosenAge;

	// Token: 0x040044E4 RID: 17636
	private bool _metrics_LearnMorePressed;
}
