using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200093C RID: 2364
public class KIDUI_Controller : MonoBehaviour
{
	// Token: 0x1700059F RID: 1439
	// (get) Token: 0x06003A30 RID: 14896 RVA: 0x0012CF8E File Offset: 0x0012B18E
	public static KIDUI_Controller Instance
	{
		get
		{
			return KIDUI_Controller._instance;
		}
	}

	// Token: 0x170005A0 RID: 1440
	// (get) Token: 0x06003A31 RID: 14897 RVA: 0x0012CF95 File Offset: 0x0012B195
	public static bool IsKIDUIActive
	{
		get
		{
			return !(KIDUI_Controller.Instance == null) && KIDUI_Controller.Instance._isKidUIActive;
		}
	}

	// Token: 0x170005A1 RID: 1441
	// (get) Token: 0x06003A32 RID: 14898 RVA: 0x0012CFB0 File Offset: 0x0012B1B0
	private static string EtagOnCloseBlackScreenPlayerPrefRef
	{
		get
		{
			if (string.IsNullOrEmpty(KIDUI_Controller.etagOnCloseBlackScreenPlayerPrefStr))
			{
				KIDUI_Controller.etagOnCloseBlackScreenPlayerPrefStr = "closeBlackScreen-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			}
			return KIDUI_Controller.etagOnCloseBlackScreenPlayerPrefStr;
		}
	}

	// Token: 0x06003A33 RID: 14899 RVA: 0x0012CFDE File Offset: 0x0012B1DE
	private void Awake()
	{
		KIDUI_Controller._instance = this;
		Debug.LogFormat("[KID::UI::CONTROLLER] Controller Initialised", Array.Empty<object>());
	}

	// Token: 0x06003A34 RID: 14900 RVA: 0x0012CFF5 File Offset: 0x0012B1F5
	private void OnDestroy()
	{
		KIDManager.onEmailResultReceived = (KIDManager.OnEmailResultReceived)Delegate.Remove(KIDManager.onEmailResultReceived, new KIDManager.OnEmailResultReceived(this.NotifyOfEmailResult));
	}

	// Token: 0x06003A35 RID: 14901 RVA: 0x0012D018 File Offset: 0x0012B218
	public Task StartKIDScreens(CancellationToken cancellationToken)
	{
		KIDUI_Controller.<StartKIDScreens>d__20 <StartKIDScreens>d__;
		<StartKIDScreens>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartKIDScreens>d__.<>4__this = this;
		<StartKIDScreens>d__.cancellationToken = cancellationToken;
		<StartKIDScreens>d__.<>1__state = -1;
		<StartKIDScreens>d__.<>t__builder.Start<KIDUI_Controller.<StartKIDScreens>d__20>(ref <StartKIDScreens>d__);
		return <StartKIDScreens>d__.<>t__builder.Task;
	}

	// Token: 0x06003A36 RID: 14902 RVA: 0x0012D064 File Offset: 0x0012B264
	public void CloseKIDScreens()
	{
		this.SaveEtagOnCloseScreen();
		this._isKidUIActive = false;
		this._mainKIDScreen.HideMainScreen();
		PrivateUIRoom.RemoveUI(base.transform);
		HandRayController.Instance.DisableHandRays();
		Object.DestroyImmediate(base.gameObject);
		KIDManager.onEmailResultReceived = (KIDManager.OnEmailResultReceived)Delegate.Remove(KIDManager.onEmailResultReceived, new KIDManager.OnEmailResultReceived(this.NotifyOfEmailResult));
	}

	// Token: 0x06003A37 RID: 14903 RVA: 0x0012D0CC File Offset: 0x0012B2CC
	public void UpdateScreenStatus()
	{
		EMainScreenStatus screenStatusFromSession = this.GetScreenStatusFromSession();
		KIDUI_MainScreen mainKIDScreen = this._mainKIDScreen;
		if (mainKIDScreen == null)
		{
			return;
		}
		mainKIDScreen.UpdateScreenStatus(screenStatusFromSession, true);
	}

	// Token: 0x06003A38 RID: 14904 RVA: 0x0012D0F4 File Offset: 0x0012B2F4
	public void NotifyOfEmailResult(bool success)
	{
		if (this._confirmScreen == null)
		{
			Debug.LogError("[KID::UI_CONTROLLER] _confirmScreen has not been set yet and is NULL. Cannot inform of result");
			return;
		}
		if (success)
		{
			PlayerPrefs.SetInt(KIDManager.GetChallengedBeforePlayerPrefRef, 1);
			PlayerPrefs.Save();
		}
		Debug.Log("[KID::UI_CONTROLLER] Notifying user about email result. Showing confirm screen.");
		this._confirmScreen.NotifyOfResult(success);
	}

	// Token: 0x06003A39 RID: 14905 RVA: 0x0012D144 File Offset: 0x0012B344
	private EMainScreenStatus GetScreenStatusFromSession()
	{
		EMainScreenStatus result;
		switch (KIDManager.CurrentSession.SessionStatus)
		{
		case SessionStatus.PASS:
			if (this.ShouldShowScreenOnPermissionChange())
			{
				result = EMainScreenStatus.Updated;
			}
			else if (KIDManager.PreviousStatus == SessionStatus.CHALLENGE_SESSION_UPGRADE)
			{
				result = EMainScreenStatus.Declined;
			}
			else
			{
				result = EMainScreenStatus.Missing;
			}
			break;
		case SessionStatus.PROHIBITED:
			Debug.LogError("[KID::KIDUI_CONTROLLER] Status is PROHIBITED but is trying to show k-ID screens");
			result = EMainScreenStatus.Declined;
			break;
		case SessionStatus.CHALLENGE:
		case SessionStatus.CHALLENGE_SESSION_UPGRADE:
		case SessionStatus.PENDING_AGE_APPEAL:
			if (string.IsNullOrEmpty(PlayerPrefs.GetString(KIDManager.GetEmailForUserPlayerPrefRef, "")))
			{
				result = EMainScreenStatus.Setup;
			}
			else
			{
				result = EMainScreenStatus.Pending;
			}
			break;
		default:
			Debug.LogError("[KID::KIDUI_CONTROLLER] Unknown status");
			result = EMainScreenStatus.None;
			break;
		}
		return result;
	}

	// Token: 0x06003A3A RID: 14906 RVA: 0x0012D1D0 File Offset: 0x0012B3D0
	private Task<bool> ShouldShowKIDScreen(CancellationToken cancellationToken)
	{
		KIDUI_Controller.<ShouldShowKIDScreen>d__25 <ShouldShowKIDScreen>d__;
		<ShouldShowKIDScreen>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<ShouldShowKIDScreen>d__.<>4__this = this;
		<ShouldShowKIDScreen>d__.cancellationToken = cancellationToken;
		<ShouldShowKIDScreen>d__.<>1__state = -1;
		<ShouldShowKIDScreen>d__.<>t__builder.Start<KIDUI_Controller.<ShouldShowKIDScreen>d__25>(ref <ShouldShowKIDScreen>d__);
		return <ShouldShowKIDScreen>d__.<>t__builder.Task;
	}

	// Token: 0x06003A3B RID: 14907 RVA: 0x0012D21B File Offset: 0x0012B41B
	private bool ShouldShowScreenOnPermissionChange()
	{
		this._lastEtagOnClose = this.GetLastBlackScreenEtag();
		string lastEtagOnClose = this._lastEtagOnClose;
		TMPSession currentSession = KIDManager.CurrentSession;
		return lastEtagOnClose != (((currentSession != null) ? currentSession.Etag : null) ?? string.Empty);
	}

	// Token: 0x06003A3C RID: 14908 RVA: 0x0012D24E File Offset: 0x0012B44E
	private string GetLastBlackScreenEtag()
	{
		return PlayerPrefs.GetString(KIDUI_Controller.EtagOnCloseBlackScreenPlayerPrefRef, "");
	}

	// Token: 0x06003A3D RID: 14909 RVA: 0x0012D25F File Offset: 0x0012B45F
	private void SaveEtagOnCloseScreen()
	{
		if (KIDManager.CurrentSession == null)
		{
			Debug.Log("[KID::MANAGER] Trying to save Pre-Game Screen ETAG, but [CurrentSession] is null");
			return;
		}
		PlayerPrefs.SetString(KIDUI_Controller.EtagOnCloseBlackScreenPlayerPrefRef, KIDManager.CurrentSession.Etag);
		PlayerPrefs.Save();
	}

	// Token: 0x0400476F RID: 18287
	private const string CLOSE_BLACK_SCREEN_ETAG_PLAYER_PREF_PREFIX = "closeBlackScreen-";

	// Token: 0x04004770 RID: 18288
	private const string FIRST_TIME_POST_CHANGE_PLAYER_PREF = "hasShownFirstTimePostChange-";

	// Token: 0x04004771 RID: 18289
	private static KIDUI_Controller _instance;

	// Token: 0x04004772 RID: 18290
	[SerializeField]
	private KIDUI_MainScreen _mainKIDScreen;

	// Token: 0x04004773 RID: 18291
	[SerializeField]
	private KIDUI_ConfirmScreen _confirmScreen;

	// Token: 0x04004774 RID: 18292
	[SerializeField]
	private List<string> _PermissionsWithToggles = new List<string>();

	// Token: 0x04004775 RID: 18293
	[SerializeField]
	private List<EKIDFeatures> _inaccessibleSettings = new List<EKIDFeatures>
	{
		EKIDFeatures.Multiplayer,
		EKIDFeatures.Mods
	};

	// Token: 0x04004776 RID: 18294
	private KIDUI_Controller.Metrics_ShowReason _showReason;

	// Token: 0x04004777 RID: 18295
	private bool _isKidUIActive;

	// Token: 0x04004778 RID: 18296
	private static string etagOnCloseBlackScreenPlayerPrefStr;

	// Token: 0x04004779 RID: 18297
	private string _lastEtagOnClose;

	// Token: 0x0200093D RID: 2365
	public enum Metrics_ShowReason
	{
		// Token: 0x0400477B RID: 18299
		None,
		// Token: 0x0400477C RID: 18300
		Inaccessible,
		// Token: 0x0400477D RID: 18301
		Guardian_Disabled,
		// Token: 0x0400477E RID: 18302
		Permissions_Changed,
		// Token: 0x0400477F RID: 18303
		Default_Session,
		// Token: 0x04004780 RID: 18304
		No_Session
	}
}
