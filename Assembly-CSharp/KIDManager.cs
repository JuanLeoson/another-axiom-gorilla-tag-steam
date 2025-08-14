using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GorillaNetworking;
using KID.Model;
using Newtonsoft.Json;
using UnityEngine;

// Token: 0x020008D7 RID: 2263
public class KIDManager : MonoBehaviour
{
	// Token: 0x17000573 RID: 1395
	// (get) Token: 0x0600383D RID: 14397 RVA: 0x00122370 File Offset: 0x00120570
	public static KIDManager Instance
	{
		get
		{
			return KIDManager._instance;
		}
	}

	// Token: 0x17000574 RID: 1396
	// (get) Token: 0x0600383E RID: 14398 RVA: 0x00122377 File Offset: 0x00120577
	// (set) Token: 0x0600383F RID: 14399 RVA: 0x0012237E File Offset: 0x0012057E
	public static bool InitialisationComplete { get; private set; } = false;

	// Token: 0x17000575 RID: 1397
	// (get) Token: 0x06003840 RID: 14400 RVA: 0x00122386 File Offset: 0x00120586
	// (set) Token: 0x06003841 RID: 14401 RVA: 0x0012238D File Offset: 0x0012058D
	public static bool InitialisationSuccessful { get; private set; } = false;

	// Token: 0x17000576 RID: 1398
	// (get) Token: 0x06003842 RID: 14402 RVA: 0x00122395 File Offset: 0x00120595
	// (set) Token: 0x06003843 RID: 14403 RVA: 0x0012239C File Offset: 0x0012059C
	public static TMPSession CurrentSession { get; private set; }

	// Token: 0x17000577 RID: 1399
	// (get) Token: 0x06003844 RID: 14404 RVA: 0x001223A4 File Offset: 0x001205A4
	// (set) Token: 0x06003845 RID: 14405 RVA: 0x001223AB File Offset: 0x001205AB
	public static SessionStatus PreviousStatus { get; private set; }

	// Token: 0x17000578 RID: 1400
	// (get) Token: 0x06003846 RID: 14406 RVA: 0x001223B3 File Offset: 0x001205B3
	// (set) Token: 0x06003847 RID: 14407 RVA: 0x001223BA File Offset: 0x001205BA
	public static GetRequirementsData _ageGateRequirements { get; private set; }

	// Token: 0x17000579 RID: 1401
	// (get) Token: 0x06003848 RID: 14408 RVA: 0x001223C2 File Offset: 0x001205C2
	public static bool KidTitleDataReady
	{
		get
		{
			return KIDManager._titleDataReady;
		}
	}

	// Token: 0x1700057A RID: 1402
	// (get) Token: 0x06003849 RID: 14409 RVA: 0x001223C9 File Offset: 0x001205C9
	public static bool KidEnabled
	{
		get
		{
			return KIDManager.KidTitleDataReady && KIDManager._useKid;
		}
	}

	// Token: 0x1700057B RID: 1403
	// (get) Token: 0x0600384A RID: 14410 RVA: 0x001223D9 File Offset: 0x001205D9
	public static bool KidEnabledAndReady
	{
		get
		{
			return KIDManager.KidEnabled && KIDManager.InitialisationSuccessful;
		}
	}

	// Token: 0x1700057C RID: 1404
	// (get) Token: 0x0600384B RID: 14411 RVA: 0x001223E9 File Offset: 0x001205E9
	public static bool HasSession
	{
		get
		{
			return KIDManager.CurrentSession != null && KIDManager.CurrentSession.SessionId != Guid.Empty;
		}
	}

	// Token: 0x1700057D RID: 1405
	// (get) Token: 0x0600384C RID: 14412 RVA: 0x00122408 File Offset: 0x00120608
	public static string PreviousStatusPlayerPrefRef
	{
		get
		{
			return "previous-status-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		}
	}

	// Token: 0x1700057E RID: 1406
	// (get) Token: 0x0600384D RID: 14413 RVA: 0x00122420 File Offset: 0x00120620
	// (set) Token: 0x0600384E RID: 14414 RVA: 0x00122427 File Offset: 0x00120627
	public static bool HasOptedInToKID { get; private set; }

	// Token: 0x1700057F RID: 1407
	// (get) Token: 0x0600384F RID: 14415 RVA: 0x0012242F File Offset: 0x0012062F
	private static string KIDSetupPlayerPref
	{
		get
		{
			return "KID-Setup-";
		}
	}

	// Token: 0x17000580 RID: 1408
	// (get) Token: 0x06003850 RID: 14416 RVA: 0x00122436 File Offset: 0x00120636
	// (set) Token: 0x06003851 RID: 14417 RVA: 0x0012243D File Offset: 0x0012063D
	public static string DbgLocale { get; set; }

	// Token: 0x17000581 RID: 1409
	// (get) Token: 0x06003852 RID: 14418 RVA: 0x00122445 File Offset: 0x00120645
	public static string DebugKIDLocalePlayerPrefRef
	{
		get
		{
			return KIDManager._debugKIDLocalePlayerPrefRef;
		}
	}

	// Token: 0x17000582 RID: 1410
	// (get) Token: 0x06003853 RID: 14419 RVA: 0x0012244C File Offset: 0x0012064C
	public static string GetEmailForUserPlayerPrefRef
	{
		get
		{
			if (string.IsNullOrEmpty(KIDManager.parentEmailForUserPlayerPrefRef))
			{
				KIDManager.parentEmailForUserPlayerPrefRef = "k-id_EmailAddress" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			}
			return KIDManager.parentEmailForUserPlayerPrefRef;
		}
	}

	// Token: 0x17000583 RID: 1411
	// (get) Token: 0x06003854 RID: 14420 RVA: 0x0012247A File Offset: 0x0012067A
	public static string GetChallengedBeforePlayerPrefRef
	{
		get
		{
			return "k-id_ChallengedBefore" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		}
	}

	// Token: 0x06003855 RID: 14421 RVA: 0x00122494 File Offset: 0x00120694
	private void Awake()
	{
		if (KIDManager._instance != null)
		{
			Debug.LogError("Trying to create new instance of [KIDManager], but one already exists. Destroying object [" + base.gameObject.name + "].");
			Object.Destroy(base.gameObject);
			return;
		}
		Debug.Log("[KID] INIT");
		KIDManager._instance = this;
		KIDManager.DbgLocale = PlayerPrefs.GetString(KIDManager._debugKIDLocalePlayerPrefRef, "");
	}

	// Token: 0x06003856 RID: 14422 RVA: 0x00122500 File Offset: 0x00120700
	private void Start()
	{
		KIDManager.<Start>d__70 <Start>d__;
		<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Start>d__.<>1__state = -1;
		<Start>d__.<>t__builder.Start<KIDManager.<Start>d__70>(ref <Start>d__);
	}

	// Token: 0x06003857 RID: 14423 RVA: 0x0012252F File Offset: 0x0012072F
	private void OnDestroy()
	{
		KIDManager._requestCancellationSource.Cancel();
	}

	// Token: 0x06003858 RID: 14424 RVA: 0x0012253C File Offset: 0x0012073C
	public static string GetActiveAccountStatusNiceString()
	{
		switch (KIDManager.GetActiveAccountStatus())
		{
		case AgeStatusType.DIGITALMINOR:
			return "Digital Minor";
		case AgeStatusType.DIGITALYOUTH:
			return "Digital Youth";
		case AgeStatusType.LEGALADULT:
			return "Legal Adult";
		default:
			return "UNKNOWN";
		}
	}

	// Token: 0x06003859 RID: 14425 RVA: 0x0012257C File Offset: 0x0012077C
	public static AgeStatusType GetActiveAccountStatus()
	{
		if (KIDManager.CurrentSession != null)
		{
			return KIDManager.CurrentSession.AgeStatus;
		}
		if (!PlayFabAuthenticator.instance.GetSafety())
		{
			return AgeStatusType.LEGALADULT;
		}
		return AgeStatusType.DIGITALMINOR;
	}

	// Token: 0x0600385A RID: 14426 RVA: 0x001225A1 File Offset: 0x001207A1
	public static List<Permission> GetAllPermissionsData()
	{
		if (KIDManager.CurrentSession == null)
		{
			Debug.LogError("[KID::MANAGER] There is no current session. Unless the age-gate has not yet finished there should always be a session even if it is the default session");
			return new List<Permission>();
		}
		return KIDManager.CurrentSession.GetAllPermissions();
	}

	// Token: 0x0600385B RID: 14427 RVA: 0x001225C4 File Offset: 0x001207C4
	public static bool TryGetAgeStatusTypeFromAge(int age, out AgeStatusType ageType)
	{
		if (KIDManager._ageGateRequirements == null)
		{
			Debug.LogError("[KID::MANAGER] [_ageGateRequirements] is not set - need to Get AgeGate Requirements first");
			ageType = AgeStatusType.DIGITALMINOR;
			return false;
		}
		if (age < KIDManager._ageGateRequirements.AgeGateRequirements.DigitalConsentAge)
		{
			ageType = AgeStatusType.DIGITALMINOR;
			return true;
		}
		if (age < KIDManager._ageGateRequirements.AgeGateRequirements.CivilAge)
		{
			ageType = AgeStatusType.DIGITALYOUTH;
			return true;
		}
		ageType = AgeStatusType.LEGALADULT;
		return true;
	}

	// Token: 0x0600385C RID: 14428 RVA: 0x0012261C File Offset: 0x0012081C
	[return: TupleElementNames(new string[]
	{
		"requiresOptIn",
		"hasOptedInPreviously"
	})]
	public static ValueTuple<bool, bool> CheckFeatureOptIn(EKIDFeatures feature, Permission permissionData = null)
	{
		if (permissionData == null)
		{
			permissionData = KIDManager.GetPermissionDataByFeature(feature);
			if (permissionData == null)
			{
				Debug.LogError("[KID::MANAGER] Unable to retrieve permission data for feature [" + feature.ToStandardisedString() + "]");
				return new ValueTuple<bool, bool>(false, false);
			}
		}
		if (permissionData.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
		{
			return new ValueTuple<bool, bool>(false, false);
		}
		bool item = KIDManager.CurrentSession.HasOptedInToPermission(feature);
		if (permissionData.ManagedBy == Permission.ManagedByEnum.GUARDIAN)
		{
			return new ValueTuple<bool, bool>(false, item);
		}
		if (permissionData.ManagedBy == Permission.ManagedByEnum.PLAYER && permissionData.Enabled)
		{
			Debug.Log("[KID::MANAGER] Feature [" + feature.ToStandardisedString() + "] is managed by Player AND permission data is enabled.");
			return new ValueTuple<bool, bool>(false, true);
		}
		return new ValueTuple<bool, bool>(true, item);
	}

	// Token: 0x0600385D RID: 14429 RVA: 0x001226C4 File Offset: 0x001208C4
	public static void SetFeatureOptIn(EKIDFeatures feature, bool optedIn)
	{
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(feature);
		if (permissionDataByFeature == null)
		{
			Debug.LogErrorFormat("[KID] Trying to set Feature Opt in for feature [" + feature.ToStandardisedString() + "] but permission data could not be found. Assumed is opt-in", Array.Empty<object>());
			return;
		}
		switch (permissionDataByFeature.ManagedBy)
		{
		case Permission.ManagedByEnum.PLAYER:
			KIDManager.CurrentSession.OptInToPermission(feature, optedIn);
			return;
		case Permission.ManagedByEnum.GUARDIAN:
			KIDManager.CurrentSession.OptInToPermission(feature, permissionDataByFeature.Enabled);
			return;
		case Permission.ManagedByEnum.PROHIBITED:
			KIDManager.CurrentSession.OptInToPermission(feature, false);
			return;
		default:
			return;
		}
	}

	// Token: 0x0600385E RID: 14430 RVA: 0x00122744 File Offset: 0x00120944
	public static bool CheckFeatureSettingEnabled(EKIDFeatures feature)
	{
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(feature);
		if (permissionDataByFeature == null)
		{
			Debug.LogError("[KID::MANAGER] Unable to permissions for feature [" + feature.ToStandardisedString() + "]");
			return false;
		}
		if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
		{
			return false;
		}
		bool item = KIDManager.CheckFeatureOptIn(feature, null).Item2;
		switch (feature)
		{
		case EKIDFeatures.Multiplayer:
		case EKIDFeatures.Mods:
			return item;
		case EKIDFeatures.Custom_Nametags:
			return item && GorillaComputer.instance.NametagsEnabled;
		case EKIDFeatures.Voice_Chat:
			return item && GorillaComputer.instance.CheckVoiceChatEnabled();
		case EKIDFeatures.Groups:
			return permissionDataByFeature.ManagedBy != Permission.ManagedByEnum.GUARDIAN || permissionDataByFeature.Enabled;
		default:
			Debug.LogError("[KID::MANAGER] Tried finding feature setting for [" + feature.ToStandardisedString() + "] but failed.");
			return false;
		}
	}

	// Token: 0x0600385F RID: 14431 RVA: 0x00122800 File Offset: 0x00120A00
	private static Task<GetPlayerData_Data> TryGetPlayerData(bool forceRefresh)
	{
		KIDManager.<TryGetPlayerData>d__81 <TryGetPlayerData>d__;
		<TryGetPlayerData>d__.<>t__builder = AsyncTaskMethodBuilder<GetPlayerData_Data>.Create();
		<TryGetPlayerData>d__.forceRefresh = forceRefresh;
		<TryGetPlayerData>d__.<>1__state = -1;
		<TryGetPlayerData>d__.<>t__builder.Start<KIDManager.<TryGetPlayerData>d__81>(ref <TryGetPlayerData>d__);
		return <TryGetPlayerData>d__.<>t__builder.Task;
	}

	// Token: 0x06003860 RID: 14432 RVA: 0x00122844 File Offset: 0x00120A44
	private static Task<GetRequirementsData> TryGetRequirements()
	{
		KIDManager.<TryGetRequirements>d__82 <TryGetRequirements>d__;
		<TryGetRequirements>d__.<>t__builder = AsyncTaskMethodBuilder<GetRequirementsData>.Create();
		<TryGetRequirements>d__.<>1__state = -1;
		<TryGetRequirements>d__.<>t__builder.Start<KIDManager.<TryGetRequirements>d__82>(ref <TryGetRequirements>d__);
		return <TryGetRequirements>d__.<>t__builder.Task;
	}

	// Token: 0x06003861 RID: 14433 RVA: 0x00122880 File Offset: 0x00120A80
	private static Task<VerifyAgeData> TryVerifyAgeResponse()
	{
		KIDManager.<TryVerifyAgeResponse>d__83 <TryVerifyAgeResponse>d__;
		<TryVerifyAgeResponse>d__.<>t__builder = AsyncTaskMethodBuilder<VerifyAgeData>.Create();
		<TryVerifyAgeResponse>d__.<>1__state = -1;
		<TryVerifyAgeResponse>d__.<>t__builder.Start<KIDManager.<TryVerifyAgeResponse>d__83>(ref <TryVerifyAgeResponse>d__);
		return <TryVerifyAgeResponse>d__.<>t__builder.Task;
	}

	// Token: 0x06003862 RID: 14434 RVA: 0x001228BC File Offset: 0x00120ABC
	[return: TupleElementNames(new string[]
	{
		"success",
		"exception"
	})]
	private static Task<ValueTuple<bool, string>> TrySendChallengeEmailRequest()
	{
		KIDManager.<TrySendChallengeEmailRequest>d__84 <TrySendChallengeEmailRequest>d__;
		<TrySendChallengeEmailRequest>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, string>>.Create();
		<TrySendChallengeEmailRequest>d__.<>1__state = -1;
		<TrySendChallengeEmailRequest>d__.<>t__builder.Start<KIDManager.<TrySendChallengeEmailRequest>d__84>(ref <TrySendChallengeEmailRequest>d__);
		return <TrySendChallengeEmailRequest>d__.<>t__builder.Task;
	}

	// Token: 0x06003863 RID: 14435 RVA: 0x001228F8 File Offset: 0x00120AF8
	private static Task<bool> TrySendOptInPermissions()
	{
		KIDManager.<TrySendOptInPermissions>d__85 <TrySendOptInPermissions>d__;
		<TrySendOptInPermissions>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<TrySendOptInPermissions>d__.<>1__state = -1;
		<TrySendOptInPermissions>d__.<>t__builder.Start<KIDManager.<TrySendOptInPermissions>d__85>(ref <TrySendOptInPermissions>d__);
		return <TrySendOptInPermissions>d__.<>t__builder.Task;
	}

	// Token: 0x06003864 RID: 14436 RVA: 0x00122934 File Offset: 0x00120B34
	public static Task<ValueTuple<bool, string>> TrySendUpgradeSessionChallengeEmail()
	{
		KIDManager.<TrySendUpgradeSessionChallengeEmail>d__86 <TrySendUpgradeSessionChallengeEmail>d__;
		<TrySendUpgradeSessionChallengeEmail>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, string>>.Create();
		<TrySendUpgradeSessionChallengeEmail>d__.<>1__state = -1;
		<TrySendUpgradeSessionChallengeEmail>d__.<>t__builder.Start<KIDManager.<TrySendUpgradeSessionChallengeEmail>d__86>(ref <TrySendUpgradeSessionChallengeEmail>d__);
		return <TrySendUpgradeSessionChallengeEmail>d__.<>t__builder.Task;
	}

	// Token: 0x06003865 RID: 14437 RVA: 0x00122970 File Offset: 0x00120B70
	public static Task<UpgradeSessionData> TryUpgradeSession(List<string> requestedPermissions)
	{
		KIDManager.<TryUpgradeSession>d__87 <TryUpgradeSession>d__;
		<TryUpgradeSession>d__.<>t__builder = AsyncTaskMethodBuilder<UpgradeSessionData>.Create();
		<TryUpgradeSession>d__.requestedPermissions = requestedPermissions;
		<TryUpgradeSession>d__.<>1__state = -1;
		<TryUpgradeSession>d__.<>t__builder.Start<KIDManager.<TryUpgradeSession>d__87>(ref <TryUpgradeSession>d__);
		return <TryUpgradeSession>d__.<>t__builder.Task;
	}

	// Token: 0x06003866 RID: 14438 RVA: 0x001229B4 File Offset: 0x00120BB4
	public static Task<AttemptAgeUpdateData> TryAttemptAgeUpdate(int age)
	{
		KIDManager.<TryAttemptAgeUpdate>d__88 <TryAttemptAgeUpdate>d__;
		<TryAttemptAgeUpdate>d__.<>t__builder = AsyncTaskMethodBuilder<AttemptAgeUpdateData>.Create();
		<TryAttemptAgeUpdate>d__.age = age;
		<TryAttemptAgeUpdate>d__.<>1__state = -1;
		<TryAttemptAgeUpdate>d__.<>t__builder.Start<KIDManager.<TryAttemptAgeUpdate>d__88>(ref <TryAttemptAgeUpdate>d__);
		return <TryAttemptAgeUpdate>d__.<>t__builder.Task;
	}

	// Token: 0x06003867 RID: 14439 RVA: 0x001229F8 File Offset: 0x00120BF8
	public static Task<bool> TryAppealAge(string email, int newAge)
	{
		KIDManager.<TryAppealAge>d__89 <TryAppealAge>d__;
		<TryAppealAge>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<TryAppealAge>d__.email = email;
		<TryAppealAge>d__.newAge = newAge;
		<TryAppealAge>d__.<>1__state = -1;
		<TryAppealAge>d__.<>t__builder.Start<KIDManager.<TryAppealAge>d__89>(ref <TryAppealAge>d__);
		return <TryAppealAge>d__.<>t__builder.Task;
	}

	// Token: 0x06003868 RID: 14440 RVA: 0x00122A44 File Offset: 0x00120C44
	public static Task UpdateSession(Action<bool> getDataCompleted = null)
	{
		KIDManager.<UpdateSession>d__90 <UpdateSession>d__;
		<UpdateSession>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<UpdateSession>d__.getDataCompleted = getDataCompleted;
		<UpdateSession>d__.<>1__state = -1;
		<UpdateSession>d__.<>t__builder.Start<KIDManager.<UpdateSession>d__90>(ref <UpdateSession>d__);
		return <UpdateSession>d__.<>t__builder.Task;
	}

	// Token: 0x06003869 RID: 14441 RVA: 0x00122A88 File Offset: 0x00120C88
	private static Task<bool> CheckWarningScreensOptedIn()
	{
		KIDManager.<CheckWarningScreensOptedIn>d__91 <CheckWarningScreensOptedIn>d__;
		<CheckWarningScreensOptedIn>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<CheckWarningScreensOptedIn>d__.<>1__state = -1;
		<CheckWarningScreensOptedIn>d__.<>t__builder.Start<KIDManager.<CheckWarningScreensOptedIn>d__91>(ref <CheckWarningScreensOptedIn>d__);
		return <CheckWarningScreensOptedIn>d__.<>t__builder.Task;
	}

	// Token: 0x0600386A RID: 14442 RVA: 0x00122AC3 File Offset: 0x00120CC3
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	public static void InitialiseBootFlow()
	{
		Debug.Log("[KID::MANAGER] PHASE ZERO -- START -- Checking K-ID Flag");
		if (PlayerPrefs.GetInt(KIDManager.KIDSetupPlayerPref, 0) != 0)
		{
			return;
		}
		Debug.Log("[KID::MANAGER] INITIALISE BOOT FLOW - Force Starting Overlay");
		PrivateUIRoom.ForceStartOverlay();
	}

	// Token: 0x0600386B RID: 14443 RVA: 0x00122AEC File Offset: 0x00120CEC
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	public static void InitialiseKID()
	{
		KIDManager.<InitialiseKID>d__93 <InitialiseKID>d__;
		<InitialiseKID>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<InitialiseKID>d__.<>1__state = -1;
		<InitialiseKID>d__.<>t__builder.Start<KIDManager.<InitialiseKID>d__93>(ref <InitialiseKID>d__);
	}

	// Token: 0x0600386C RID: 14444 RVA: 0x00122B1C File Offset: 0x00120D1C
	private static bool UpdatePermissions(TMPSession newSession)
	{
		Debug.Log("[KID::MANAGER] Updating Permissions to reflect session.");
		if (newSession == null || !newSession.IsValidSession)
		{
			Debug.LogError("[KID::MANAGER] A NULL or Invalid Session was received!");
			return false;
		}
		KIDManager.SaveStoredPermissions();
		KIDManager.CurrentSession = newSession;
		if (KIDUI_Controller.IsKIDUIActive)
		{
			KIDManager.PreviousStatus = KIDManager.CurrentSession.SessionStatus;
			PlayerPrefs.SetInt(KIDManager.PreviousStatusPlayerPrefRef, (int)KIDManager.PreviousStatus);
			PlayerPrefs.Save();
		}
		if (!KIDManager.CurrentSession.IsDefault)
		{
			PlayerPrefs.SetInt(KIDManager.KIDSetupPlayerPref, 1);
			PlayerPrefs.Save();
		}
		KIDManager.OnSessionUpdated();
		if (KIDUI_Controller.Instance)
		{
			KIDUI_Controller.Instance.UpdateScreenStatus();
		}
		return true;
	}

	// Token: 0x0600386D RID: 14445 RVA: 0x00122BB7 File Offset: 0x00120DB7
	private static void ClearSession()
	{
		KIDManager.CurrentSession = null;
		KIDManager.DeleteStoredPermissions();
	}

	// Token: 0x0600386E RID: 14446 RVA: 0x000023F5 File Offset: 0x000005F5
	private static void SaveStoredPermissions()
	{
	}

	// Token: 0x0600386F RID: 14447 RVA: 0x000023F5 File Offset: 0x000005F5
	private static void DeleteStoredPermissions()
	{
	}

	// Token: 0x06003870 RID: 14448 RVA: 0x00122BC4 File Offset: 0x00120DC4
	public static CancellationTokenSource ResetCancellationToken()
	{
		KIDManager._requestCancellationSource.Dispose();
		KIDManager._requestCancellationSource = new CancellationTokenSource();
		return KIDManager._requestCancellationSource;
	}

	// Token: 0x06003871 RID: 14449 RVA: 0x00122BE0 File Offset: 0x00120DE0
	public static Permission GetPermissionDataByFeature(EKIDFeatures feature)
	{
		if (KIDManager.CurrentSession == null)
		{
			if (!PlayFabAuthenticator.instance.GetSafety())
			{
				return new Permission(feature.ToStandardisedString(), true, Permission.ManagedByEnum.PLAYER);
			}
			return new Permission(feature.ToStandardisedString(), false, Permission.ManagedByEnum.GUARDIAN);
		}
		else
		{
			Permission result;
			if (!KIDManager.CurrentSession.TryGetPermission(feature, out result))
			{
				Debug.LogError("[KID::MANAGER] Failed to retreive permission from session for [" + feature.ToStandardisedString() + "]. Assuming disabled permission");
				return new Permission(feature.ToStandardisedString(), false, Permission.ManagedByEnum.GUARDIAN);
			}
			return result;
		}
	}

	// Token: 0x06003872 RID: 14450 RVA: 0x0012252F File Offset: 0x0012072F
	public static void CancelToken()
	{
		KIDManager._requestCancellationSource.Cancel();
	}

	// Token: 0x06003873 RID: 14451 RVA: 0x00122C5C File Offset: 0x00120E5C
	public static Task<bool> UseKID()
	{
		KIDManager.<UseKID>d__101 <UseKID>d__;
		<UseKID>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UseKID>d__.<>1__state = -1;
		<UseKID>d__.<>t__builder.Start<KIDManager.<UseKID>d__101>(ref <UseKID>d__);
		return <UseKID>d__.<>t__builder.Task;
	}

	// Token: 0x06003874 RID: 14452 RVA: 0x00122C98 File Offset: 0x00120E98
	public static Task<int> CheckKIDPhase()
	{
		KIDManager.<CheckKIDPhase>d__102 <CheckKIDPhase>d__;
		<CheckKIDPhase>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
		<CheckKIDPhase>d__.<>1__state = -1;
		<CheckKIDPhase>d__.<>t__builder.Start<KIDManager.<CheckKIDPhase>d__102>(ref <CheckKIDPhase>d__);
		return <CheckKIDPhase>d__.<>t__builder.Task;
	}

	// Token: 0x06003875 RID: 14453 RVA: 0x00122CD4 File Offset: 0x00120ED4
	public static Task<DateTime?> CheckKIDNewPlayerDateTime()
	{
		KIDManager.<CheckKIDNewPlayerDateTime>d__103 <CheckKIDNewPlayerDateTime>d__;
		<CheckKIDNewPlayerDateTime>d__.<>t__builder = AsyncTaskMethodBuilder<DateTime?>.Create();
		<CheckKIDNewPlayerDateTime>d__.<>1__state = -1;
		<CheckKIDNewPlayerDateTime>d__.<>t__builder.Start<KIDManager.<CheckKIDNewPlayerDateTime>d__103>(ref <CheckKIDNewPlayerDateTime>d__);
		return <CheckKIDNewPlayerDateTime>d__.<>t__builder.Task;
	}

	// Token: 0x06003876 RID: 14454 RVA: 0x00122D10 File Offset: 0x00120F10
	private static bool GetIsEnabled(string jsonTxt)
	{
		KIDTitleData kidtitleData = JsonConvert.DeserializeObject<KIDTitleData>(jsonTxt);
		if (kidtitleData == null)
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDTitleData]. Json: \n" + jsonTxt);
			return false;
		}
		bool result;
		if (!bool.TryParse(kidtitleData.KIDEnabled, out result))
		{
			Debug.LogError("[KID_MANAGER] Failed to parse 'KIDEnabled': [KIDEnabled] to bool.");
			return false;
		}
		return result;
	}

	// Token: 0x06003877 RID: 14455 RVA: 0x00122D58 File Offset: 0x00120F58
	private static int GetPhase(string jsonTxt)
	{
		KIDTitleData kidtitleData = JsonConvert.DeserializeObject<KIDTitleData>(jsonTxt);
		if (kidtitleData == null)
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDTitleData]. Json: \n" + jsonTxt);
			return 0;
		}
		return kidtitleData.KIDPhase;
	}

	// Token: 0x06003878 RID: 14456 RVA: 0x00122D88 File Offset: 0x00120F88
	private static DateTime? GetNewPlayerDateTime(string jsonTxt)
	{
		KIDTitleData kidtitleData = JsonConvert.DeserializeObject<KIDTitleData>(jsonTxt);
		if (kidtitleData == null)
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDTitleData]. Json: \n" + jsonTxt);
			return null;
		}
		DateTime value;
		if (!DateTime.TryParse(kidtitleData.KIDNewPlayerIsoTimestamp, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out value))
		{
			Debug.LogError("[KID_MANAGER] Failed to parse 'KIDNewPlayerIsoTimestamp': [KIDNewPlayerIsoTimestamp] to DateTime.");
			return null;
		}
		return new DateTime?(value);
	}

	// Token: 0x06003879 RID: 14457 RVA: 0x00122DEC File Offset: 0x00120FEC
	public static bool IsAdult()
	{
		return KIDManager.CurrentSession.IsValidSession && KIDManager.CurrentSession.AgeStatus == AgeStatusType.LEGALADULT;
	}

	// Token: 0x0600387A RID: 14458 RVA: 0x00122E0C File Offset: 0x0012100C
	public static bool HasAllPermissions()
	{
		List<Permission> allPermissions = KIDManager.CurrentSession.GetAllPermissions();
		for (int i = 0; i < allPermissions.Count; i++)
		{
			if (allPermissions[i].ManagedBy == Permission.ManagedByEnum.GUARDIAN || !allPermissions[i].Enabled)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600387B RID: 14459 RVA: 0x00122E58 File Offset: 0x00121058
	public static Task<bool> SetKIDOptIn()
	{
		KIDManager.<SetKIDOptIn>d__109 <SetKIDOptIn>d__;
		<SetKIDOptIn>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<SetKIDOptIn>d__.<>1__state = -1;
		<SetKIDOptIn>d__.<>t__builder.Start<KIDManager.<SetKIDOptIn>d__109>(ref <SetKIDOptIn>d__);
		return <SetKIDOptIn>d__.<>t__builder.Task;
	}

	// Token: 0x0600387C RID: 14460 RVA: 0x00122E94 File Offset: 0x00121094
	[return: TupleElementNames(new string[]
	{
		"success",
		"message"
	})]
	public static Task<ValueTuple<bool, string>> SetAndSendEmail(string email)
	{
		KIDManager.<SetAndSendEmail>d__110 <SetAndSendEmail>d__;
		<SetAndSendEmail>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, string>>.Create();
		<SetAndSendEmail>d__.email = email;
		<SetAndSendEmail>d__.<>1__state = -1;
		<SetAndSendEmail>d__.<>t__builder.Start<KIDManager.<SetAndSendEmail>d__110>(ref <SetAndSendEmail>d__);
		return <SetAndSendEmail>d__.<>t__builder.Task;
	}

	// Token: 0x0600387D RID: 14461 RVA: 0x00122ED8 File Offset: 0x001210D8
	public static Task<bool> SendOptInPermissions()
	{
		KIDManager.<SendOptInPermissions>d__111 <SendOptInPermissions>d__;
		<SendOptInPermissions>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<SendOptInPermissions>d__.<>1__state = -1;
		<SendOptInPermissions>d__.<>t__builder.Start<KIDManager.<SendOptInPermissions>d__111>(ref <SendOptInPermissions>d__);
		return <SendOptInPermissions>d__.<>t__builder.Task;
	}

	// Token: 0x0600387E RID: 14462 RVA: 0x00122F14 File Offset: 0x00121114
	public static bool HasPermissionToUseFeature(EKIDFeatures feature)
	{
		if (!KIDManager.KidEnabledAndReady)
		{
			return !PlayFabAuthenticator.instance.GetSafety();
		}
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(feature);
		return (permissionDataByFeature.Enabled || permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PLAYER) && permissionDataByFeature.ManagedBy != Permission.ManagedByEnum.PROHIBITED;
	}

	// Token: 0x0600387F RID: 14463 RVA: 0x00122F60 File Offset: 0x00121160
	private static Task<bool> WaitForAuthentication()
	{
		KIDManager.<WaitForAuthentication>d__113 <WaitForAuthentication>d__;
		<WaitForAuthentication>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<WaitForAuthentication>d__.<>1__state = -1;
		<WaitForAuthentication>d__.<>t__builder.Start<KIDManager.<WaitForAuthentication>d__113>(ref <WaitForAuthentication>d__);
		return <WaitForAuthentication>d__.<>t__builder.Task;
	}

	// Token: 0x06003880 RID: 14464 RVA: 0x00122F9C File Offset: 0x0012119C
	[return: TupleElementNames(new string[]
	{
		"ageStatus",
		"resp"
	})]
	private static Task<ValueTuple<AgeStatusType, TMPSession>> AgeGateFlow(GetPlayerData_Data newPlayerData)
	{
		KIDManager.<AgeGateFlow>d__114 <AgeGateFlow>d__;
		<AgeGateFlow>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<AgeStatusType, TMPSession>>.Create();
		<AgeGateFlow>d__.newPlayerData = newPlayerData;
		<AgeGateFlow>d__.<>1__state = -1;
		<AgeGateFlow>d__.<>t__builder.Start<KIDManager.<AgeGateFlow>d__114>(ref <AgeGateFlow>d__);
		return <AgeGateFlow>d__.<>t__builder.Task;
	}

	// Token: 0x06003881 RID: 14465 RVA: 0x00122FE0 File Offset: 0x001211E0
	private static Task<VerifyAgeData> ProcessAgeGate()
	{
		KIDManager.<ProcessAgeGate>d__115 <ProcessAgeGate>d__;
		<ProcessAgeGate>d__.<>t__builder = AsyncTaskMethodBuilder<VerifyAgeData>.Create();
		<ProcessAgeGate>d__.<>1__state = -1;
		<ProcessAgeGate>d__.<>t__builder.Start<KIDManager.<ProcessAgeGate>d__115>(ref <ProcessAgeGate>d__);
		return <ProcessAgeGate>d__.<>t__builder.Task;
	}

	// Token: 0x06003882 RID: 14466 RVA: 0x0012301B File Offset: 0x0012121B
	public static string GetOptInKey(EKIDFeatures feature)
	{
		return feature.ToStandardisedString() + "-opt-in-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
	}

	// Token: 0x06003883 RID: 14467 RVA: 0x00123039 File Offset: 0x00121239
	private static bool CanSendEmail()
	{
		Debug.LogError("[KID::MANAGER] TEMP: For now will do cooldown checks on the client. But eventually should move to server");
		return true;
	}

	// Token: 0x06003884 RID: 14468 RVA: 0x00123048 File Offset: 0x00121248
	private static Task<GetPlayerData_Data> Server_GetPlayerData(bool forceRefresh, Action failureCallback)
	{
		KIDManager.<Server_GetPlayerData>d__131 <Server_GetPlayerData>d__;
		<Server_GetPlayerData>d__.<>t__builder = AsyncTaskMethodBuilder<GetPlayerData_Data>.Create();
		<Server_GetPlayerData>d__.forceRefresh = forceRefresh;
		<Server_GetPlayerData>d__.failureCallback = failureCallback;
		<Server_GetPlayerData>d__.<>1__state = -1;
		<Server_GetPlayerData>d__.<>t__builder.Start<KIDManager.<Server_GetPlayerData>d__131>(ref <Server_GetPlayerData>d__);
		return <Server_GetPlayerData>d__.<>t__builder.Task;
	}

	// Token: 0x06003885 RID: 14469 RVA: 0x00123094 File Offset: 0x00121294
	private static Task<bool> Server_SetConfirmedStatus()
	{
		KIDManager.<Server_SetConfirmedStatus>d__132 <Server_SetConfirmedStatus>d__;
		<Server_SetConfirmedStatus>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<Server_SetConfirmedStatus>d__.<>1__state = -1;
		<Server_SetConfirmedStatus>d__.<>t__builder.Start<KIDManager.<Server_SetConfirmedStatus>d__132>(ref <Server_SetConfirmedStatus>d__);
		return <Server_SetConfirmedStatus>d__.<>t__builder.Task;
	}

	// Token: 0x06003886 RID: 14470 RVA: 0x001230D0 File Offset: 0x001212D0
	private static Task<UpgradeSessionData> Server_UpgradeSession(global::UpgradeSessionRequest request)
	{
		KIDManager.<Server_UpgradeSession>d__133 <Server_UpgradeSession>d__;
		<Server_UpgradeSession>d__.<>t__builder = AsyncTaskMethodBuilder<UpgradeSessionData>.Create();
		<Server_UpgradeSession>d__.request = request;
		<Server_UpgradeSession>d__.<>1__state = -1;
		<Server_UpgradeSession>d__.<>t__builder.Start<KIDManager.<Server_UpgradeSession>d__133>(ref <Server_UpgradeSession>d__);
		return <Server_UpgradeSession>d__.<>t__builder.Task;
	}

	// Token: 0x06003887 RID: 14471 RVA: 0x00123114 File Offset: 0x00121314
	private static Task<VerifyAgeData> Server_VerifyAge(VerifyAgeRequest request, Action failureCallback)
	{
		KIDManager.<Server_VerifyAge>d__134 <Server_VerifyAge>d__;
		<Server_VerifyAge>d__.<>t__builder = AsyncTaskMethodBuilder<VerifyAgeData>.Create();
		<Server_VerifyAge>d__.request = request;
		<Server_VerifyAge>d__.failureCallback = failureCallback;
		<Server_VerifyAge>d__.<>1__state = -1;
		<Server_VerifyAge>d__.<>t__builder.Start<KIDManager.<Server_VerifyAge>d__134>(ref <Server_VerifyAge>d__);
		return <Server_VerifyAge>d__.<>t__builder.Task;
	}

	// Token: 0x06003888 RID: 14472 RVA: 0x00123160 File Offset: 0x00121360
	private static Task<AttemptAgeUpdateData> Server_AttemptAgeUpdate(AttemptAgeUpdateRequest request, Action failureCallback)
	{
		KIDManager.<Server_AttemptAgeUpdate>d__135 <Server_AttemptAgeUpdate>d__;
		<Server_AttemptAgeUpdate>d__.<>t__builder = AsyncTaskMethodBuilder<AttemptAgeUpdateData>.Create();
		<Server_AttemptAgeUpdate>d__.request = request;
		<Server_AttemptAgeUpdate>d__.<>1__state = -1;
		<Server_AttemptAgeUpdate>d__.<>t__builder.Start<KIDManager.<Server_AttemptAgeUpdate>d__135>(ref <Server_AttemptAgeUpdate>d__);
		return <Server_AttemptAgeUpdate>d__.<>t__builder.Task;
	}

	// Token: 0x06003889 RID: 14473 RVA: 0x001231A4 File Offset: 0x001213A4
	private static Task<bool> Server_AppealAge(AppealAgeRequest request, Action failureCallback)
	{
		KIDManager.<Server_AppealAge>d__136 <Server_AppealAge>d__;
		<Server_AppealAge>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<Server_AppealAge>d__.request = request;
		<Server_AppealAge>d__.<>1__state = -1;
		<Server_AppealAge>d__.<>t__builder.Start<KIDManager.<Server_AppealAge>d__136>(ref <Server_AppealAge>d__);
		return <Server_AppealAge>d__.<>t__builder.Task;
	}

	// Token: 0x0600388A RID: 14474 RVA: 0x001231E8 File Offset: 0x001213E8
	private static Task<ValueTuple<bool, string>> Server_SendChallengeEmail(SendChallengeEmailRequest request)
	{
		KIDManager.<Server_SendChallengeEmail>d__137 <Server_SendChallengeEmail>d__;
		<Server_SendChallengeEmail>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, string>>.Create();
		<Server_SendChallengeEmail>d__.request = request;
		<Server_SendChallengeEmail>d__.<>1__state = -1;
		<Server_SendChallengeEmail>d__.<>t__builder.Start<KIDManager.<Server_SendChallengeEmail>d__137>(ref <Server_SendChallengeEmail>d__);
		return <Server_SendChallengeEmail>d__.<>t__builder.Task;
	}

	// Token: 0x0600388B RID: 14475 RVA: 0x0012322C File Offset: 0x0012142C
	private static Task<bool> Server_SetOptInPermissions(SetOptInPermissionsRequest request, Action failureCallback)
	{
		KIDManager.<Server_SetOptInPermissions>d__138 <Server_SetOptInPermissions>d__;
		<Server_SetOptInPermissions>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<Server_SetOptInPermissions>d__.request = request;
		<Server_SetOptInPermissions>d__.failureCallback = failureCallback;
		<Server_SetOptInPermissions>d__.<>1__state = -1;
		<Server_SetOptInPermissions>d__.<>t__builder.Start<KIDManager.<Server_SetOptInPermissions>d__138>(ref <Server_SetOptInPermissions>d__);
		return <Server_SetOptInPermissions>d__.<>t__builder.Task;
	}

	// Token: 0x0600388C RID: 14476 RVA: 0x00123278 File Offset: 0x00121478
	private static Task<bool> Server_OptIn()
	{
		KIDManager.<Server_OptIn>d__139 <Server_OptIn>d__;
		<Server_OptIn>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<Server_OptIn>d__.<>1__state = -1;
		<Server_OptIn>d__.<>t__builder.Start<KIDManager.<Server_OptIn>d__139>(ref <Server_OptIn>d__);
		return <Server_OptIn>d__.<>t__builder.Task;
	}

	// Token: 0x0600388D RID: 14477 RVA: 0x001232B4 File Offset: 0x001214B4
	private static Task<GetRequirementsData> Server_GetRequirements()
	{
		KIDManager.<Server_GetRequirements>d__140 <Server_GetRequirements>d__;
		<Server_GetRequirements>d__.<>t__builder = AsyncTaskMethodBuilder<GetRequirementsData>.Create();
		<Server_GetRequirements>d__.<>1__state = -1;
		<Server_GetRequirements>d__.<>t__builder.Start<KIDManager.<Server_GetRequirements>d__140>(ref <Server_GetRequirements>d__);
		return <Server_GetRequirements>d__.<>t__builder.Task;
	}

	// Token: 0x0600388E RID: 14478 RVA: 0x001232F0 File Offset: 0x001214F0
	[return: TupleElementNames(new string[]
	{
		"code",
		"responseModel",
		"errorMessage"
	})]
	private static Task<ValueTuple<long, T, string>> KIDServerWebRequest<T, Q>(string endpoint, string operationType, Q requestData, string queryParams = null, int maxRetries = 2, Func<long, bool> responseCodeIsRetryable = null) where T : class where Q : KIDRequestData
	{
		KIDManager.<KIDServerWebRequest>d__141<T, Q> <KIDServerWebRequest>d__;
		<KIDServerWebRequest>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<long, T, string>>.Create();
		<KIDServerWebRequest>d__.endpoint = endpoint;
		<KIDServerWebRequest>d__.operationType = operationType;
		<KIDServerWebRequest>d__.requestData = requestData;
		<KIDServerWebRequest>d__.queryParams = queryParams;
		<KIDServerWebRequest>d__.maxRetries = maxRetries;
		<KIDServerWebRequest>d__.responseCodeIsRetryable = responseCodeIsRetryable;
		<KIDServerWebRequest>d__.<>1__state = -1;
		<KIDServerWebRequest>d__.<>t__builder.Start<KIDManager.<KIDServerWebRequest>d__141<T, Q>>(ref <KIDServerWebRequest>d__);
		return <KIDServerWebRequest>d__.<>t__builder.Task;
	}

	// Token: 0x0600388F RID: 14479 RVA: 0x00123360 File Offset: 0x00121560
	private static Task<long> KIDServerWebRequestNoResponse<Q>(string endpoint, string operationType, Q requestData, int maxRetries = 2, Func<long, bool> responseCodeIsRetryable = null) where Q : KIDRequestData
	{
		KIDManager.<KIDServerWebRequestNoResponse>d__142<Q> <KIDServerWebRequestNoResponse>d__;
		<KIDServerWebRequestNoResponse>d__.<>t__builder = AsyncTaskMethodBuilder<long>.Create();
		<KIDServerWebRequestNoResponse>d__.endpoint = endpoint;
		<KIDServerWebRequestNoResponse>d__.operationType = operationType;
		<KIDServerWebRequestNoResponse>d__.requestData = requestData;
		<KIDServerWebRequestNoResponse>d__.maxRetries = maxRetries;
		<KIDServerWebRequestNoResponse>d__.responseCodeIsRetryable = responseCodeIsRetryable;
		<KIDServerWebRequestNoResponse>d__.<>1__state = -1;
		<KIDServerWebRequestNoResponse>d__.<>t__builder.Start<KIDManager.<KIDServerWebRequestNoResponse>d__142<Q>>(ref <KIDServerWebRequestNoResponse>d__);
		return <KIDServerWebRequestNoResponse>d__.<>t__builder.Task;
	}

	// Token: 0x06003890 RID: 14480 RVA: 0x001233C4 File Offset: 0x001215C4
	public static void RegisterSessionUpdateCallback_AnyPermission(Action callback)
	{
		Debug.Log("[KID] Successfully registered a new callback to SessionUpdate which monitors any permission change");
		KIDManager._onSessionUpdated_AnyPermission = (Action)Delegate.Combine(KIDManager._onSessionUpdated_AnyPermission, callback);
	}

	// Token: 0x06003891 RID: 14481 RVA: 0x001233E5 File Offset: 0x001215E5
	public static void UnregisterSessionUpdateCallback_AnyPermission(Action callback)
	{
		Debug.Log("[KID] Successfully unregistered a new callback to SessionUpdate which monitors any permission change");
		KIDManager._onSessionUpdated_AnyPermission = (Action)Delegate.Remove(KIDManager._onSessionUpdated_AnyPermission, callback);
	}

	// Token: 0x06003892 RID: 14482 RVA: 0x00123406 File Offset: 0x00121606
	public static void RegisterSessionUpdatedCallback_VoiceChat(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully registered a new callback to SessionUpdate which monitors the Voice Chat permission");
		KIDManager._onSessionUpdated_VoiceChat = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_VoiceChat, callback);
	}

	// Token: 0x06003893 RID: 14483 RVA: 0x00123427 File Offset: 0x00121627
	public static void UnregisterSessionUpdatedCallback_VoiceChat(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully unregistered a callback to SessionUpdate which monitors the Voice Chat permission");
		KIDManager._onSessionUpdated_VoiceChat = (Action<bool, Permission.ManagedByEnum>)Delegate.Remove(KIDManager._onSessionUpdated_VoiceChat, callback);
	}

	// Token: 0x06003894 RID: 14484 RVA: 0x00123448 File Offset: 0x00121648
	public static void RegisterSessionUpdatedCallback_CustomUsernames(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully registered a new callback to SessionUpdate which monitors the Custom Usernames permission");
		KIDManager._onSessionUpdated_CustomUsernames = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_CustomUsernames, callback);
	}

	// Token: 0x06003895 RID: 14485 RVA: 0x00123469 File Offset: 0x00121669
	public static void UnregisterSessionUpdatedCallback_CustomUsernames(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully unregistered a callback to SessionUpdate which monitors the Custom Usernames permission");
		KIDManager._onSessionUpdated_CustomUsernames = (Action<bool, Permission.ManagedByEnum>)Delegate.Remove(KIDManager._onSessionUpdated_CustomUsernames, callback);
	}

	// Token: 0x06003896 RID: 14486 RVA: 0x0012348A File Offset: 0x0012168A
	public static void RegisterSessionUpdatedCallback_PrivateRooms(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully registered a new callback to SessionUpdate which monitors the Private Rooms permission");
		KIDManager._onSessionUpdated_PrivateRooms = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_PrivateRooms, callback);
	}

	// Token: 0x06003897 RID: 14487 RVA: 0x001234AB File Offset: 0x001216AB
	public static void UnregisterSessionUpdatedCallback_PrivateRooms(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully unregistered a callback to SessionUpdate which monitors the Private Rooms permission");
		KIDManager._onSessionUpdated_PrivateRooms = (Action<bool, Permission.ManagedByEnum>)Delegate.Remove(KIDManager._onSessionUpdated_PrivateRooms, callback);
	}

	// Token: 0x06003898 RID: 14488 RVA: 0x001234CC File Offset: 0x001216CC
	public static void RegisterSessionUpdatedCallback_Multiplayer(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully registered a new callback to SessionUpdate which monitors the Multiplayer permission");
		KIDManager._onSessionUpdated_Multiplayer = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_Multiplayer, callback);
	}

	// Token: 0x06003899 RID: 14489 RVA: 0x001234ED File Offset: 0x001216ED
	public static void UnregisterSessionUpdatedCallback_Multiplayer(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully unregistered a callback to SessionUpdate which monitors the Multiplayer permission");
		KIDManager._onSessionUpdated_Multiplayer = (Action<bool, Permission.ManagedByEnum>)Delegate.Remove(KIDManager._onSessionUpdated_Multiplayer, callback);
	}

	// Token: 0x0600389A RID: 14490 RVA: 0x0012350E File Offset: 0x0012170E
	public static void RegisterSessionUpdatedCallback_UGC(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully registered a new callback to SessionUpdate which monitors the UGC permission");
		KIDManager._onSessionUpdated_UGC = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_UGC, callback);
	}

	// Token: 0x0600389B RID: 14491 RVA: 0x00123530 File Offset: 0x00121730
	public static Task<bool> WaitForAndUpdateNewSession(bool forceRefresh)
	{
		KIDManager.<WaitForAndUpdateNewSession>d__169 <WaitForAndUpdateNewSession>d__;
		<WaitForAndUpdateNewSession>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<WaitForAndUpdateNewSession>d__.forceRefresh = forceRefresh;
		<WaitForAndUpdateNewSession>d__.<>1__state = -1;
		<WaitForAndUpdateNewSession>d__.<>t__builder.Start<KIDManager.<WaitForAndUpdateNewSession>d__169>(ref <WaitForAndUpdateNewSession>d__);
		return <WaitForAndUpdateNewSession>d__.<>t__builder.Task;
	}

	// Token: 0x0600389C RID: 14492 RVA: 0x00123574 File Offset: 0x00121774
	private static bool HasSessionChanged(TMPSession newSession)
	{
		if (newSession == null)
		{
			return false;
		}
		if (KIDManager.CurrentSession == null)
		{
			return true;
		}
		if (!newSession.IsValidSession)
		{
			return false;
		}
		if (newSession.IsDefault)
		{
			Debug.LogError(string.Format("[KID::MANAGER] DEBUG - New Session Is Default! Age: [{0}]", newSession.Age));
			return false;
		}
		return KIDManager.CurrentSession.IsDefault || !newSession.Etag.Equals(KIDManager.CurrentSession.Etag);
	}

	// Token: 0x0600389D RID: 14493 RVA: 0x001235E8 File Offset: 0x001217E8
	private static void OnSessionUpdated()
	{
		Action onSessionUpdated_AnyPermission = KIDManager._onSessionUpdated_AnyPermission;
		if (onSessionUpdated_AnyPermission != null)
		{
			onSessionUpdated_AnyPermission();
		}
		bool voiceChatEnabled = false;
		bool joinGroupsEnabled = false;
		bool customUsernamesEnabled = false;
		List<Permission> allPermissionsData = KIDManager.GetAllPermissionsData();
		int count = allPermissionsData.Count;
		for (int i = 0; i < count; i++)
		{
			Permission permission = allPermissionsData[i];
			string name = permission.Name;
			if (!(name == "voice-chat"))
			{
				if (!(name == "custom-username"))
				{
					if (!(name == "join-groups"))
					{
						if (!(name == "multiplayer"))
						{
							if (!(name == "mods"))
							{
								Debug.Log("[KID] Tried updating permission with name [" + permission.Name + "] but did not match any of the set cases. Unable to process");
							}
							else if (KIDManager.HasPermissionChanged(permission))
							{
								Action<bool, Permission.ManagedByEnum> onSessionUpdated_UGC = KIDManager._onSessionUpdated_UGC;
								if (onSessionUpdated_UGC != null)
								{
									onSessionUpdated_UGC(permission.Enabled, permission.ManagedBy);
								}
								KIDManager._previousPermissionSettings[permission.Name] = permission;
							}
						}
						else
						{
							if (KIDManager.HasPermissionChanged(permission))
							{
								Action<bool, Permission.ManagedByEnum> onSessionUpdated_Multiplayer = KIDManager._onSessionUpdated_Multiplayer;
								if (onSessionUpdated_Multiplayer != null)
								{
									onSessionUpdated_Multiplayer(permission.Enabled, permission.ManagedBy);
								}
								KIDManager._previousPermissionSettings[permission.Name] = permission;
							}
							bool enabled = permission.Enabled;
						}
					}
					else
					{
						if (KIDManager.HasPermissionChanged(permission))
						{
							Action<bool, Permission.ManagedByEnum> onSessionUpdated_PrivateRooms = KIDManager._onSessionUpdated_PrivateRooms;
							if (onSessionUpdated_PrivateRooms != null)
							{
								onSessionUpdated_PrivateRooms(permission.Enabled, permission.ManagedBy);
							}
							KIDManager._previousPermissionSettings[permission.Name] = permission;
						}
						joinGroupsEnabled = permission.Enabled;
					}
				}
				else
				{
					if (KIDManager.HasPermissionChanged(permission))
					{
						Action<bool, Permission.ManagedByEnum> onSessionUpdated_CustomUsernames = KIDManager._onSessionUpdated_CustomUsernames;
						if (onSessionUpdated_CustomUsernames != null)
						{
							onSessionUpdated_CustomUsernames(permission.Enabled, permission.ManagedBy);
						}
						KIDManager._previousPermissionSettings[permission.Name] = permission;
					}
					customUsernamesEnabled = permission.Enabled;
				}
			}
			else
			{
				if (KIDManager.HasPermissionChanged(permission))
				{
					Action<bool, Permission.ManagedByEnum> onSessionUpdated_VoiceChat = KIDManager._onSessionUpdated_VoiceChat;
					if (onSessionUpdated_VoiceChat != null)
					{
						onSessionUpdated_VoiceChat(permission.Enabled, permission.ManagedBy);
					}
					KIDManager._previousPermissionSettings[permission.Name] = permission;
				}
				voiceChatEnabled = permission.Enabled;
			}
		}
		GorillaTelemetry.PostKidEvent(joinGroupsEnabled, voiceChatEnabled, customUsernamesEnabled, KIDManager.CurrentSession.AgeStatus, GTKidEventType.permission_update);
	}

	// Token: 0x0600389E RID: 14494 RVA: 0x0012381C File Offset: 0x00121A1C
	private static bool HasPermissionChanged(Permission newValue)
	{
		Permission permission;
		if (KIDManager._previousPermissionSettings.TryGetValue(newValue.Name, out permission))
		{
			return permission.Enabled != newValue.Enabled || permission.ManagedBy != newValue.ManagedBy;
		}
		KIDManager._previousPermissionSettings.Add(newValue.Name, newValue);
		return true;
	}

	// Token: 0x04004510 RID: 17680
	public const string MULTIPLAYER_PERMISSION_NAME = "multiplayer";

	// Token: 0x04004511 RID: 17681
	public const string UGC_PERMISSION_NAME = "mods";

	// Token: 0x04004512 RID: 17682
	public const string PRIVATE_ROOM_PERMISSION_NAME = "join-groups";

	// Token: 0x04004513 RID: 17683
	public const string VOICE_CHAT_PERMISSION_NAME = "voice-chat";

	// Token: 0x04004514 RID: 17684
	public const string CUSTOM_USERNAME_PERMISSION_NAME = "custom-username";

	// Token: 0x04004515 RID: 17685
	public const string PREVIOUS_STATUS_PREF_KEY_PREFIX = "previous-status-";

	// Token: 0x04004516 RID: 17686
	public const string KID_DATA_KEY = "KIDData";

	// Token: 0x04004517 RID: 17687
	private const string KID_EMAIL_KEY = "k-id_EmailAddress";

	// Token: 0x04004518 RID: 17688
	private const int SECONDS_BETWEEN_UPDATE_ATTEMPTS = 30;

	// Token: 0x04004519 RID: 17689
	private const string KID_SETUP_FLAG = "KID-Setup-";

	// Token: 0x0400451A RID: 17690
	[OnEnterPlay_SetNull]
	private static KIDManager _instance;

	// Token: 0x0400451F RID: 17695
	private static string _emailAddress;

	// Token: 0x04004520 RID: 17696
	private static CancellationTokenSource _requestCancellationSource = new CancellationTokenSource();

	// Token: 0x04004521 RID: 17697
	private static bool _titleDataReady = false;

	// Token: 0x04004522 RID: 17698
	private static bool _useKid = false;

	// Token: 0x04004523 RID: 17699
	private static int _kIDPhase = 0;

	// Token: 0x04004524 RID: 17700
	private static DateTime? _kIDNewPlayerDateTime = null;

	// Token: 0x04004528 RID: 17704
	private static string _debugKIDLocalePlayerPrefRef = "KID_SPOOF_LOCALE";

	// Token: 0x04004529 RID: 17705
	private static string parentEmailForUserPlayerPrefRef;

	// Token: 0x0400452A RID: 17706
	[OnEnterPlay_SetNull]
	private static Action _sessionUpdatedCallback = null;

	// Token: 0x0400452B RID: 17707
	[OnEnterPlay_SetNull]
	private static Action _onKIDInitialisationComplete = null;

	// Token: 0x0400452C RID: 17708
	public static KIDManager.OnEmailResultReceived onEmailResultReceived;

	// Token: 0x0400452D RID: 17709
	private const string KID_GET_SESSION = "GetPlayerData";

	// Token: 0x0400452E RID: 17710
	private const string KID_VERIFY_AGE = "VerifyAge";

	// Token: 0x0400452F RID: 17711
	private const string KID_UPGRADE_SESSION = "UpgradeSession";

	// Token: 0x04004530 RID: 17712
	private const string KID_SEND_CHALLENGE_EMAIL = "SendChallengeEmail";

	// Token: 0x04004531 RID: 17713
	private const string KID_ATTEMPT_AGE_UPDATE = "AttemptAgeUpdate";

	// Token: 0x04004532 RID: 17714
	private const string KID_APPEAL_AGE = "AppealAge";

	// Token: 0x04004533 RID: 17715
	private const string KID_OPT_IN = "OptIn";

	// Token: 0x04004534 RID: 17716
	private const string KID_GET_REQUIREMENTS = "GetRequirements";

	// Token: 0x04004535 RID: 17717
	private const string KID_SET_CONFIRMED_STATUS = "SetConfirmedStatus";

	// Token: 0x04004536 RID: 17718
	private const string KID_SET_OPT_IN_PERMISSIONS = "SetOptInPermissions";

	// Token: 0x04004537 RID: 17719
	private const string KID_FORCE_REFRESH = "sessionRefresh";

	// Token: 0x04004538 RID: 17720
	private const int MAX_RETRIES_FOR_CRITICAL_KID_SERVER_REQUESTS = 3;

	// Token: 0x04004539 RID: 17721
	private const int MAX_RETRIES_FOR_NORMAL_KID_SERVER_REQUESTS = 2;

	// Token: 0x0400453A RID: 17722
	public const string KID_PERMISSION__VOICE_CHAT = "voice-chat";

	// Token: 0x0400453B RID: 17723
	public const string KID_PERMISSION__CUSTOM_NAMES = "custom-username";

	// Token: 0x0400453C RID: 17724
	public const string KID_PERMISSION__PRIVATE_ROOMS = "join-groups";

	// Token: 0x0400453D RID: 17725
	public const string KID_PERMISSION__MULTIPLAYER = "multiplayer";

	// Token: 0x0400453E RID: 17726
	public const string KID_PERMISSION__UGC = "mods";

	// Token: 0x0400453F RID: 17727
	private const float MAX_SESSION_UPDATE_TIME = 600f;

	// Token: 0x04004540 RID: 17728
	private const int TIME_BETWEEN_SESSION_UPDATE_ATTEMPTS = 30;

	// Token: 0x04004541 RID: 17729
	[OnEnterPlay_SetNull]
	private static Action _onSessionUpdated_AnyPermission;

	// Token: 0x04004542 RID: 17730
	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_VoiceChat;

	// Token: 0x04004543 RID: 17731
	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_CustomUsernames;

	// Token: 0x04004544 RID: 17732
	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_PrivateRooms;

	// Token: 0x04004545 RID: 17733
	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_Multiplayer;

	// Token: 0x04004546 RID: 17734
	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_UGC;

	// Token: 0x04004547 RID: 17735
	private static bool _isUpdatingNewSession = false;

	// Token: 0x04004548 RID: 17736
	[OnEnterPlay_SetNull]
	private static Dictionary<string, Permission> _previousPermissionSettings = new Dictionary<string, Permission>();

	// Token: 0x020008D8 RID: 2264
	// (Invoke) Token: 0x060038A2 RID: 14498
	public delegate void OnEmailResultReceived(bool result);
}
