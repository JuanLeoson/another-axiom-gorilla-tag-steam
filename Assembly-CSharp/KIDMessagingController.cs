using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GorillaNetworking;
using Newtonsoft.Json;
using UnityEngine;

// Token: 0x02000915 RID: 2325
public class KIDMessagingController : MonoBehaviour
{
	// Token: 0x17000591 RID: 1425
	// (get) Token: 0x06003962 RID: 14690 RVA: 0x00129723 File Offset: 0x00127923
	private static string HasShownConfirmationScreenPlayerPref
	{
		get
		{
			return "hasShownKIDConfirmationScreen-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		}
	}

	// Token: 0x06003963 RID: 14691 RVA: 0x0012973B File Offset: 0x0012793B
	public void OnConfirmPressed()
	{
		this._closeMessageBox = true;
	}

	// Token: 0x06003964 RID: 14692 RVA: 0x00129744 File Offset: 0x00127944
	private void Awake()
	{
		if (KIDMessagingController.instance != null)
		{
			Debug.LogError("[KID::MESSAGING_CONTROLLER] Trying to start a new [KIDMessagingController] but one already exists");
			Object.Destroy(this);
			return;
		}
		KIDMessagingController.instance = this;
	}

	// Token: 0x06003965 RID: 14693 RVA: 0x0012976A File Offset: 0x0012796A
	private bool ShouldShowConfirmationScreen()
	{
		return !KIDManager.CurrentSession.IsDefault && PlayerPrefs.GetInt(KIDMessagingController.HasShownConfirmationScreenPlayerPref, 0) != 1;
	}

	// Token: 0x06003966 RID: 14694 RVA: 0x0012978C File Offset: 0x0012798C
	private Task StartKIDConfirmationScreenInternal(CancellationToken token)
	{
		KIDMessagingController.<StartKIDConfirmationScreenInternal>d__12 <StartKIDConfirmationScreenInternal>d__;
		<StartKIDConfirmationScreenInternal>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartKIDConfirmationScreenInternal>d__.<>4__this = this;
		<StartKIDConfirmationScreenInternal>d__.token = token;
		<StartKIDConfirmationScreenInternal>d__.<>1__state = -1;
		<StartKIDConfirmationScreenInternal>d__.<>t__builder.Start<KIDMessagingController.<StartKIDConfirmationScreenInternal>d__12>(ref <StartKIDConfirmationScreenInternal>d__);
		return <StartKIDConfirmationScreenInternal>d__.<>t__builder.Task;
	}

	// Token: 0x06003967 RID: 14695 RVA: 0x001297D8 File Offset: 0x001279D8
	public static Task StartKIDConfirmationScreen(CancellationToken token)
	{
		KIDMessagingController.<StartKIDConfirmationScreen>d__13 <StartKIDConfirmationScreen>d__;
		<StartKIDConfirmationScreen>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartKIDConfirmationScreen>d__.token = token;
		<StartKIDConfirmationScreen>d__.<>1__state = -1;
		<StartKIDConfirmationScreen>d__.<>t__builder.Start<KIDMessagingController.<StartKIDConfirmationScreen>d__13>(ref <StartKIDConfirmationScreen>d__);
		return <StartKIDConfirmationScreen>d__.<>t__builder.Task;
	}

	// Token: 0x06003968 RID: 14696 RVA: 0x0012981C File Offset: 0x00127A1C
	private static Task<string> GetSetupConfirmationMessage()
	{
		KIDMessagingController.<GetSetupConfirmationMessage>d__14 <GetSetupConfirmationMessage>d__;
		<GetSetupConfirmationMessage>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
		<GetSetupConfirmationMessage>d__.<>1__state = -1;
		<GetSetupConfirmationMessage>d__.<>t__builder.Start<KIDMessagingController.<GetSetupConfirmationMessage>d__14>(ref <GetSetupConfirmationMessage>d__);
		return <GetSetupConfirmationMessage>d__.<>t__builder.Task;
	}

	// Token: 0x06003969 RID: 14697 RVA: 0x00129858 File Offset: 0x00127A58
	private static string GetConfirmMessageFromTitleDataJson(string jsonTxt)
	{
		if (string.IsNullOrEmpty(jsonTxt))
		{
			Debug.LogError("[KID_MANAGER] Cannot get Confirmation Message. JSON is null or empty!");
			return null;
		}
		KIDMessagingTitleData kidmessagingTitleData = JsonConvert.DeserializeObject<KIDMessagingTitleData>(jsonTxt);
		if (kidmessagingTitleData == null)
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDMessagingTitleData]. Json: \n" + jsonTxt);
			return null;
		}
		if (string.IsNullOrEmpty(kidmessagingTitleData.KIDSetupConfirmation))
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDMessagingTitleData] - [KIDSetupConfirmation] is null or empty. Json: \n" + jsonTxt);
			return null;
		}
		return kidmessagingTitleData.KIDSetupConfirmation;
	}

	// Token: 0x04004681 RID: 18049
	private const string SHOWN_CONFIRMATION_SCREEN_PREFIX = "hasShownKIDConfirmationScreen-";

	// Token: 0x04004682 RID: 18050
	private const string CONFIRMATION_HEADER = "Thank you";

	// Token: 0x04004683 RID: 18051
	private const string CONFIRMATION_BODY = "k-ID setup is now complete. Thanks and have fun in Gorilla World!";

	// Token: 0x04004684 RID: 18052
	private const string CONFIRMATION_BUTTON = "Continue";

	// Token: 0x04004685 RID: 18053
	private static KIDMessagingController instance;

	// Token: 0x04004686 RID: 18054
	[SerializeField]
	private MessageBox _confirmationMessageBox;

	// Token: 0x04004687 RID: 18055
	private bool _closeMessageBox;
}
