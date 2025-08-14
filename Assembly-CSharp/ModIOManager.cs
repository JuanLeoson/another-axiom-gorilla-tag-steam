using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using GorillaTagScripts.ModIO;
using GorillaTagScripts.VirtualStumpCustomMaps.ModIO;
using GT_CustomMapSupportRuntime;
using ModIO;
using Steamworks;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000826 RID: 2086
public class ModIOManager : MonoBehaviour
{
	// Token: 0x14000064 RID: 100
	// (add) Token: 0x0600343F RID: 13375 RVA: 0x00110198 File Offset: 0x0010E398
	// (remove) Token: 0x06003440 RID: 13376 RVA: 0x001101CC File Offset: 0x0010E3CC
	public static event ModIOManager.ModIOEnabled ModIOEnabledEvent;

	// Token: 0x14000065 RID: 101
	// (add) Token: 0x06003441 RID: 13377 RVA: 0x00110200 File Offset: 0x0010E400
	// (remove) Token: 0x06003442 RID: 13378 RVA: 0x00110234 File Offset: 0x0010E434
	public static event ModIOManager.ModIODisabled ModIODisabledEvent;

	// Token: 0x14000066 RID: 102
	// (add) Token: 0x06003443 RID: 13379 RVA: 0x00110268 File Offset: 0x0010E468
	// (remove) Token: 0x06003444 RID: 13380 RVA: 0x0011029C File Offset: 0x0010E49C
	public static event ModIOManager.InitializationFinished InitializationFinishedEvent;

	// Token: 0x06003445 RID: 13381 RVA: 0x001102D0 File Offset: 0x0010E4D0
	private void Awake()
	{
		if (ModIOManager.instance == null)
		{
			ModIOManager.instance = this;
			ModIOManager.hasInstance = true;
			UGCPermissionManager.SubscribeToUGCEnabled(new Action(ModIOManager.OnUGCEnabled));
			UGCPermissionManager.SubscribeToUGCDisabled(new Action(ModIOManager.OnUGCDisabled));
			return;
		}
		if (ModIOManager.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06003446 RID: 13382 RVA: 0x00110337 File Offset: 0x0010E537
	private void Start()
	{
		NetworkSystem.Instance.OnMultiplayerStarted += this.OnJoinedRoom;
	}

	// Token: 0x06003447 RID: 13383 RVA: 0x0011035C File Offset: 0x0010E55C
	private void OnDestroy()
	{
		if (ModIOManager.instance == this)
		{
			ModIOManager.instance = null;
			ModIOManager.hasInstance = false;
			UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(ModIOManager.OnUGCEnabled));
			UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(ModIOManager.OnUGCDisabled));
		}
		NetworkSystem.Instance.OnMultiplayerStarted -= this.OnJoinedRoom;
	}

	// Token: 0x06003448 RID: 13384 RVA: 0x001103CC File Offset: 0x0010E5CC
	private void Update()
	{
		if (!ModIOManager.hasInstance)
		{
			return;
		}
		if (ModIOManager.modDownloadQueue.IsNullOrEmpty<ModId>())
		{
			return;
		}
		if (ModIOUnity.IsModManagementBusy())
		{
			return;
		}
		ModId modId = ModIOManager.modDownloadQueue[0];
		ModIOManager.DownloadMod(modId, null);
		ModIOManager.modDownloadQueue.Remove(modId);
	}

	// Token: 0x06003449 RID: 13385 RVA: 0x000023F5 File Offset: 0x000005F5
	private static void OnUGCEnabled()
	{
	}

	// Token: 0x0600344A RID: 13386 RVA: 0x000023F5 File Offset: 0x000005F5
	private static void OnUGCDisabled()
	{
	}

	// Token: 0x0600344B RID: 13387 RVA: 0x00110415 File Offset: 0x0010E615
	public static void Initialize(Action<ModIORequestResult> callback)
	{
		if (UGCPermissionManager.IsUGCDisabled)
		{
			if (callback != null)
			{
				callback(ModIORequestResult.CreateFailureResult("MOD.IO FUNCTIONALITY IS CURRENTLY DISABLED."));
			}
			return;
		}
		if (ModIOManager.initialized && callback != null)
		{
			callback(ModIORequestResult.CreateSuccessResult());
		}
		ModIOManager.InitInternal(callback);
	}

	// Token: 0x0600344C RID: 13388 RVA: 0x00110450 File Offset: 0x0010E650
	private static void InitInternal(Action<ModIORequestResult> callback)
	{
		if (UGCPermissionManager.IsUGCDisabled)
		{
			return;
		}
		string userProfileIdentifier = "default";
		if (SteamManager.Initialized && SteamUser.BLoggedOn())
		{
			userProfileIdentifier = SteamUser.GetSteamID().ToString();
		}
		Result result = ModIOUnity.InitializeForUser(userProfileIdentifier);
		if (result.Succeeded())
		{
			ModIOManager.initialized = true;
			if (callback != null)
			{
				callback(ModIORequestResult.CreateSuccessResult());
				return;
			}
		}
		else if (callback != null)
		{
			callback(ModIORequestResult.CreateFailureResult(result.message));
		}
	}

	// Token: 0x0600344D RID: 13389 RVA: 0x001104C8 File Offset: 0x0010E6C8
	private void HasAcceptedLatestTerms(Action<bool> callback)
	{
		if (ModIOManager.initialized)
		{
			ModIOUnity.GetTermsOfUse(delegate(ResultAnd<TermsOfUse> result)
			{
				if (result.result.Succeeded())
				{
					this.OnReceivedTermsOfUse(result.value, callback);
					return;
				}
				Action<bool> callback3 = callback;
				if (callback3 == null)
				{
					return;
				}
				callback3(false);
			});
			return;
		}
		Action<bool> callback2 = callback;
		if (callback2 == null)
		{
			return;
		}
		callback2(false);
	}

	// Token: 0x0600344E RID: 13390 RVA: 0x00110514 File Offset: 0x0010E714
	private void OnReceivedTermsOfUse(TermsOfUse terms, Action<bool> callback)
	{
		ModIOManager.cachedModIOTerms = terms;
		ref TermsHash hash = ModIOManager.cachedModIOTerms.hash;
		string @string = PlayerPrefs.GetString("modIOAcceptedTermsHash");
		bool obj = hash.md5hash.Equals(@string);
		if (callback != null)
		{
			callback(obj);
		}
	}

	// Token: 0x0600344F RID: 13391 RVA: 0x00110554 File Offset: 0x0010E754
	private void ShowModIOTermsOfUse(Action<ModIORequestResultAnd<bool>> callback)
	{
		if (!ModIOManager.initialized)
		{
			if (callback != null)
			{
				callback(ModIORequestResultAnd<bool>.CreateFailureResult("CANNOT SHOW TERMS OF USE, MOD.IO HAS NOT BEEN INITIALIZED."));
			}
			return;
		}
		if (this.modIOTermsOfUsePrefab != null)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.modIOTermsOfUsePrefab, base.transform);
			if (gameObject != null)
			{
				ModIOTermsOfUse_v2 component = gameObject.GetComponent<ModIOTermsOfUse_v2>();
				if (component != null)
				{
					CustomMapManager.DisableTeleportHUD();
					ModIOManager.modIOTermsAcknowledgedCallback = callback;
					gameObject.SetActive(true);
					component.Initialize(ModIOManager.cachedModIOTerms, new Action<bool>(this.OnModIOTermsOfUseAcknowledged));
					return;
				}
				if (callback != null)
				{
					callback(ModIORequestResultAnd<bool>.CreateFailureResult("FAILED TO DISPLAY MOD.IO TERMS OF USE: \nTHE 'ModIOTermsOfUse' PREFAB DOES NOT CONTAIN A 'ModIOTermsOfUse' SCRIPT COMPONENT."));
					return;
				}
			}
			else if (callback != null)
			{
				callback(ModIORequestResultAnd<bool>.CreateFailureResult("FAILED TO DISPLAY MOD.IO TERMS OF USE: \nFAILED TO INSTANTIATE TERMS OF USE GAME OBJECT FROM 'ModIOTermsOfUse' PREFAB"));
				return;
			}
		}
		else if (this.modIOTermsOfUsePrefab == null && callback != null)
		{
			callback(ModIORequestResultAnd<bool>.CreateFailureResult("FAILED TO DISPLAY MOD.IO TERMS OF USE: \n`ModIOTermsOfUse` PREFAB IS SET TO NULL."));
		}
	}

	// Token: 0x06003450 RID: 13392 RVA: 0x00110628 File Offset: 0x0010E828
	private void OnModIOTermsOfUseAcknowledged(bool accepted)
	{
		if (accepted)
		{
			PlayerPrefs.SetString("modIOAcceptedTermsHash", ModIOManager.cachedModIOTerms.hash.md5hash);
			CustomMapManager.RequestEnableTeleportHUD(true);
			Action<ModIORequestResultAnd<bool>> action = ModIOManager.modIOTermsAcknowledgedCallback;
			if (action != null)
			{
				action(ModIORequestResultAnd<bool>.CreateSuccessResult(true));
			}
		}
		else
		{
			Action<ModIORequestResultAnd<bool>> action2 = ModIOManager.modIOTermsAcknowledgedCallback;
			if (action2 != null)
			{
				action2(ModIORequestResultAnd<bool>.CreateFailureResult("MOD.IO TERMS OF USE HAVE NOT BEEN ACCEPTED. YOU MUST ACCEPT THE MOD.IO TERMS OF USE TO LOGIN WITH YOUR PLATFORM CREDENTIALS OR YOU CAN LOGIN WITH AN EXISTING MOD.IO ACCOUNT BY PRESSING THE 'LINK MOD.IO ACCOUNT' BUTTON AND FOLLOWING THE INSTRUCTIONS."));
			}
		}
		ModIOManager.modIOTermsAcknowledgedCallback = null;
	}

	// Token: 0x06003451 RID: 13393 RVA: 0x0011068F File Offset: 0x0010E88F
	private void EnableModManagement()
	{
		if (!ModIOManager.modManagementEnabled)
		{
			ModIOManager.Refresh(delegate(bool result)
			{
				if (ModIOUnity.EnableModManagement(new ModManagementEventDelegate(this.HandleModManagementEvent), ModIOManager.allowedAutomaticModOperations).Succeeded())
				{
					ModIOManager.modManagementEnabled = true;
				}
			}, false);
		}
	}

	// Token: 0x06003452 RID: 13394 RVA: 0x001106AC File Offset: 0x0010E8AC
	private void HandleModManagementEvent(ModManagementEventType eventType, ModId modId, Result result)
	{
		switch (eventType)
		{
		case ModManagementEventType.InstallStarted:
		case ModManagementEventType.InstallFailed:
		case ModManagementEventType.DownloadStarted:
		case ModManagementEventType.DownloadFailed:
		case ModManagementEventType.UninstallStarted:
		case ModManagementEventType.UninstallFailed:
		case ModManagementEventType.UpdateStarted:
		case ModManagementEventType.UpdateFailed:
			break;
		case ModManagementEventType.Installed:
		{
			ModIOManager.outdatedModCMSVersions.Remove(modId.id);
			int num;
			ModIOManager.IsModOutdated(modId, out num);
			break;
		}
		case ModManagementEventType.Downloaded:
		case ModManagementEventType.Uninstalled:
		case ModManagementEventType.Updated:
			ModIOManager.outdatedModCMSVersions.Remove(modId.id);
			break;
		default:
			return;
		}
		GameEvents.ModIOModManagementEvent.Invoke(eventType, modId, result);
	}

	// Token: 0x06003453 RID: 13395 RVA: 0x0011072C File Offset: 0x0010E92C
	public static SubscribedModStatus ConvertModManagementEventToSubscribedModStatus(ModManagementEventType eventType)
	{
		switch (eventType)
		{
		case ModManagementEventType.InstallStarted:
			return SubscribedModStatus.Installing;
		case ModManagementEventType.Installed:
			return SubscribedModStatus.Installed;
		case ModManagementEventType.InstallFailed:
			return SubscribedModStatus.ProblemOccurred;
		case ModManagementEventType.DownloadStarted:
			return SubscribedModStatus.Downloading;
		case ModManagementEventType.Downloaded:
			return SubscribedModStatus.Downloading;
		case ModManagementEventType.DownloadFailed:
			return SubscribedModStatus.ProblemOccurred;
		case ModManagementEventType.UninstallStarted:
			return SubscribedModStatus.Uninstalling;
		case ModManagementEventType.Uninstalled:
			return SubscribedModStatus.None;
		case ModManagementEventType.UninstallFailed:
			return SubscribedModStatus.ProblemOccurred;
		case ModManagementEventType.UpdateStarted:
			return SubscribedModStatus.Updating;
		case ModManagementEventType.Updated:
			return SubscribedModStatus.Installed;
		case ModManagementEventType.UpdateFailed:
			return SubscribedModStatus.ProblemOccurred;
		default:
			return SubscribedModStatus.None;
		}
	}

	// Token: 0x06003454 RID: 13396 RVA: 0x00110790 File Offset: 0x0010E990
	public static bool IsModOutdated(ModId modId, out int exportedVersion)
	{
		exportedVersion = -1;
		SubscribedMod subscribedMod;
		return ModIOManager.hasInstance && (ModIOManager.outdatedModCMSVersions.TryGetValue(modId.id, out exportedVersion) || (ModIOManager.GetSubscribedModProfile(modId, out subscribedMod) && ModIOManager.IsInstalledModOutdated(subscribedMod)));
	}

	// Token: 0x06003455 RID: 13397 RVA: 0x001107D0 File Offset: 0x0010E9D0
	private static bool IsInstalledModOutdated(SubscribedMod subscribedMod)
	{
		if (!ModIOManager.hasInstance)
		{
			return false;
		}
		if (subscribedMod.status != SubscribedModStatus.Installed)
		{
			return false;
		}
		FileInfo[] files = new DirectoryInfo(subscribedMod.directory).GetFiles("package.json");
		try
		{
			MapPackageInfo packageInfo = CustomMapLoader.GetPackageInfo(files[0].FullName);
			if (packageInfo.customMapSupportVersion != GT_CustomMapSupportRuntime.Constants.customMapSupportVersion)
			{
				ModIOManager.outdatedModCMSVersions.Add(subscribedMod.modProfile.id.id, packageInfo.customMapSupportVersion);
				return true;
			}
		}
		catch (Exception)
		{
		}
		return false;
	}

	// Token: 0x06003456 RID: 13398 RVA: 0x00110860 File Offset: 0x0010EA60
	public static void Refresh(Action<bool> callback = null, bool force = false)
	{
		if (!ModIOManager.hasInstance)
		{
			if (callback != null)
			{
				callback(false);
			}
			return;
		}
		if (ModIOManager.refreshing)
		{
			ModIOManager.currentRefreshCallbacks.Add(callback);
			return;
		}
		if (force || Mathf.Approximately(0f, ModIOManager.lastRefreshTime) || Time.realtimeSinceStartup - ModIOManager.lastRefreshTime >= 5f)
		{
			ModIOManager.currentRefreshCallbacks.Add(callback);
			ModIOManager.lastRefreshTime = Time.realtimeSinceStartup;
			ModIOManager.refreshing = true;
			ModIOUnity.FetchUpdates(delegate(Result result)
			{
				ModIOManager.refreshing = false;
				foreach (Action<bool> action in ModIOManager.currentRefreshCallbacks)
				{
					if (action != null)
					{
						action(true);
					}
				}
				ModIOManager.currentRefreshCallbacks.Clear();
			});
			return;
		}
		if (callback != null)
		{
			callback(false);
		}
	}

	// Token: 0x06003457 RID: 13399 RVA: 0x00110904 File Offset: 0x0010EB04
	public static void GetModProfile(ModId modId, Action<ModIORequestResultAnd<ModProfile>> callback)
	{
		if (ModIOManager.hasInstance)
		{
			ModIOUnity.GetMod(modId, delegate(ResultAnd<ModProfile> result)
			{
				if (!result.result.Succeeded())
				{
					Action<ModIORequestResultAnd<ModProfile>> callback3 = callback;
					if (callback3 == null)
					{
						return;
					}
					callback3(ModIORequestResultAnd<ModProfile>.CreateFailureResult(result.result.message));
					return;
				}
				else
				{
					Action<ModIORequestResultAnd<ModProfile>> callback4 = callback;
					if (callback4 == null)
					{
						return;
					}
					callback4(ModIORequestResultAnd<ModProfile>.CreateSuccessResult(result.value));
					return;
				}
			});
			return;
		}
		Action<ModIORequestResultAnd<ModProfile>> callback2 = callback;
		if (callback2 == null)
		{
			return;
		}
		callback2(ModIORequestResultAnd<ModProfile>.CreateFailureResult("MOD.IO MANAGER INSTANCE DOES NOT EXIST!"));
	}

	// Token: 0x06003458 RID: 13400 RVA: 0x00110952 File Offset: 0x0010EB52
	public static void RefreshUserProfile()
	{
		ModIOUnity.GetCurrentUser(delegate(ResultAnd<UserProfile> result)
		{
			if (result.result.Succeeded())
			{
				ModIOManager.cachedUserProfile = result.value;
			}
			else
			{
				ModIOManager.cachedUserProfile = ModIOManager.NULL_USER_PROFILE;
			}
			UnityEvent<UserProfile> onModIOUserProfileUpdated = ModIOManager.OnModIOUserProfileUpdated;
			if (onModIOUserProfileUpdated == null)
			{
				return;
			}
			onModIOUserProfileUpdated.Invoke(ModIOManager.cachedUserProfile);
		}, false);
	}

	// Token: 0x06003459 RID: 13401 RVA: 0x00110979 File Offset: 0x0010EB79
	public static bool IsLoggedIn()
	{
		return ModIOManager.loggedIn;
	}

	// Token: 0x0600345A RID: 13402 RVA: 0x00110980 File Offset: 0x0010EB80
	public static bool IsLoggingIn()
	{
		return ModIOManager.loggingIn;
	}

	// Token: 0x0600345B RID: 13403 RVA: 0x00110987 File Offset: 0x0010EB87
	public static string GetCurrentUsername()
	{
		if (!ModIOManager.IsLoggedIn())
		{
			return "";
		}
		if (ModIOManager.cachedUserProfile.userId == -1L)
		{
			return "";
		}
		return ModIOManager.cachedUserProfile.username;
	}

	// Token: 0x0600345C RID: 13404 RVA: 0x001109B4 File Offset: 0x0010EBB4
	public static void IsAuthenticated(Action<Result> callback)
	{
		if (!ModIOManager.hasInstance)
		{
			return;
		}
		ModIOUnity.IsAuthenticated(delegate(Result result)
		{
			if (result.Succeeded())
			{
				ModIOManager.loggedIn = true;
				ModIOManager.instance.EnableModManagement();
				UnityEvent onModIOLoggedIn = ModIOManager.OnModIOLoggedIn;
				if (onModIOLoggedIn != null)
				{
					onModIOLoggedIn.Invoke();
				}
			}
			else
			{
				ModIOManager.loggedIn = false;
				ModIOManager.modManagementEnabled = false;
				UnityEvent onModIOLoggedOut = ModIOManager.OnModIOLoggedOut;
				if (onModIOLoggedOut != null)
				{
					onModIOLoggedOut.Invoke();
				}
			}
			ModIOManager.RefreshUserProfile();
			Action<Result> callback2 = callback;
			if (callback2 == null)
			{
				return;
			}
			callback2(result);
		});
	}

	// Token: 0x0600345D RID: 13405 RVA: 0x001109E8 File Offset: 0x0010EBE8
	public static void LogoutFromModIO()
	{
		if (!ModIOManager.hasInstance || ModIOManager.loggingIn)
		{
			return;
		}
		ModIOManager.CancelExternalAuthentication();
		ModIOManager.loggingIn = false;
		ModIOManager.loggedIn = false;
		ModIOUnity.LogOutCurrentUser();
		ModIOManager.cachedUserProfile = ModIOManager.NULL_USER_PROFILE;
		ModIOUnity.DisableModManagement();
		ModIOManager.modManagementEnabled = false;
		ModIOManager.OnModIOLoggedOut.Invoke();
		UnityEvent<UserProfile> onModIOUserProfileUpdated = ModIOManager.OnModIOUserProfileUpdated;
		if (onModIOUserProfileUpdated != null)
		{
			onModIOUserProfileUpdated.Invoke(ModIOManager.cachedUserProfile);
		}
		PlayerPrefs.SetInt("modIOLassSuccessfulAuthMethod", ModIOManager.ModIOAuthMethod.Invalid.GetIndex<ModIOManager.ModIOAuthMethod>());
	}

	// Token: 0x0600345E RID: 13406 RVA: 0x00110A60 File Offset: 0x0010EC60
	public static void RequestAccountLinkCode(Action<ModIORequestResult, string, string> onGetCodeCallback, Action<ModIORequestResult> onAuthCompleteCallback)
	{
		if (!ModIOManager.hasInstance)
		{
			if (onGetCodeCallback != null)
			{
				onGetCodeCallback(ModIORequestResult.CreateFailureResult("MOD.IO MANAGER INSTANCE DOES NOT EXIST!"), null, null);
			}
			if (onAuthCompleteCallback != null)
			{
				onAuthCompleteCallback(ModIORequestResult.CreateFailureResult("MOD.IO MANAGER INSTANCE DOES NOT EXIST!"));
			}
			return;
		}
		if (ModIOManager.loggingIn || ModIOManager.loggedIn)
		{
			if (onGetCodeCallback != null)
			{
				onGetCodeCallback(ModIORequestResult.CreateFailureResult("YOU MUST BE LOGGED OUT OF MOD.IO TO LINK AN EXISTING ACCOUNT."), null, null);
			}
			if (onAuthCompleteCallback != null)
			{
				onAuthCompleteCallback(ModIORequestResult.CreateFailureResult("YOU MUST BE LOGGED OUT OF MOD.IO TO LINK AN EXISTING ACCOUNT."));
			}
			return;
		}
		ModIOManager.loggingIn = true;
		ModIOManager.externalAuthGetCodeCallback = onGetCodeCallback;
		ModIOManager.externalAuthCallback = onAuthCompleteCallback;
		ModIOUnity.RequestExternalAuthentication(new Action<ResultAnd<ExternalAuthenticationToken>>(ModIOManager.instance.OnRequestExternalAuthentication));
	}

	// Token: 0x0600345F RID: 13407 RVA: 0x00110B00 File Offset: 0x0010ED00
	private void OnRequestExternalAuthentication(ResultAnd<ExternalAuthenticationToken> resultAndToken)
	{
		ModIOManager.<OnRequestExternalAuthentication>d__73 <OnRequestExternalAuthentication>d__;
		<OnRequestExternalAuthentication>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnRequestExternalAuthentication>d__.<>4__this = this;
		<OnRequestExternalAuthentication>d__.resultAndToken = resultAndToken;
		<OnRequestExternalAuthentication>d__.<>1__state = -1;
		<OnRequestExternalAuthentication>d__.<>t__builder.Start<ModIOManager.<OnRequestExternalAuthentication>d__73>(ref <OnRequestExternalAuthentication>d__);
	}

	// Token: 0x06003460 RID: 13408 RVA: 0x00110B40 File Offset: 0x0010ED40
	public static void CancelExternalAuthentication()
	{
		if (!ModIOManager.hasInstance)
		{
			return;
		}
		if (ModIOManager.externalAuthenticationToken.task != null)
		{
			ModIOManager.externalAuthenticationToken.Cancel();
			ModIOManager.externalAuthenticationToken = default(ExternalAuthenticationToken);
			Action<ModIORequestResult> action = ModIOManager.externalAuthCallback;
			if (action != null)
			{
				action(ModIORequestResult.CreateFailureResult("AUTHENTICATION CANCELED"));
			}
			ModIOManager.externalAuthCallback = null;
			ModIOManager.loggedIn = false;
			ModIOManager.loggingIn = false;
		}
	}

	// Token: 0x06003461 RID: 13409 RVA: 0x00110BA4 File Offset: 0x0010EDA4
	public static void RequestPlatformLogin(Action<ModIORequestResult> callback)
	{
		if (!ModIOManager.hasInstance)
		{
			Action<ModIORequestResult> callback2 = callback;
			if (callback2 == null)
			{
				return;
			}
			callback2(ModIORequestResult.CreateFailureResult("MOD.IO MANAGER INSTANCE DOES NOT EXIST!"));
			return;
		}
		else
		{
			if (!ModIOManager.loggingIn)
			{
				ModIOManager.loggingIn = true;
				ModIOManager.IsAuthenticated(delegate(Result result)
				{
					if (!result.Succeeded())
					{
						ModIOManager.instance.InitiatePlatformLogin(callback);
						return;
					}
					ModIOManager.loggingIn = false;
					Action<ModIORequestResult> callback4 = callback;
					if (callback4 == null)
					{
						return;
					}
					callback4(ModIORequestResult.CreateSuccessResult());
				});
				return;
			}
			Action<ModIORequestResult> callback3 = callback;
			if (callback3 == null)
			{
				return;
			}
			callback3(ModIORequestResult.CreateFailureResult("LOGIN ALREADY IN PROGRESS"));
			return;
		}
	}

	// Token: 0x06003462 RID: 13410 RVA: 0x00110C1C File Offset: 0x0010EE1C
	private void InitiatePlatformLogin(Action<ModIORequestResult> callback)
	{
		UnityEvent onModIOLoginStarted = ModIOManager.OnModIOLoginStarted;
		if (onModIOLoginStarted != null)
		{
			onModIOLoginStarted.Invoke();
		}
		Action<ModIORequestResultAnd<bool>> <>9__1;
		this.HasAcceptedLatestTerms(delegate(bool termsAlreadyAccepted)
		{
			if (!termsAlreadyAccepted)
			{
				ModIOManager <>4__this = this;
				Action<ModIORequestResultAnd<bool>> callback2;
				if ((callback2 = <>9__1) == null)
				{
					callback2 = (<>9__1 = delegate(ModIORequestResultAnd<bool> showTermsResult)
					{
						if (!showTermsResult.result.success)
						{
							this.OnAuthenticationComplete(ModIORequestResult.CreateFailureResult(showTermsResult.result.message));
							return;
						}
						if (!showTermsResult.data)
						{
							ModIORequestResult obj = ModIORequestResult.CreateFailureResult("MOD.IO TERMS OF USE HAVE NOT BEEN ACCEPTED, CANNOT PERFORM PLATFORM LOGIN.");
							Action<ModIORequestResult> callback3 = callback;
							if (callback3 != null)
							{
								callback3(obj);
							}
							this.OnAuthenticationComplete(ModIORequestResult.CreateFailureResult("MOD.IO TERMS OF USE HAVE NOT BEEN ACCEPTED, CANNOT PERFORM PLATFORM LOGIN."));
							return;
						}
						this.ContinuePlatformLogin(callback);
					});
				}
				<>4__this.ShowModIOTermsOfUse(callback2);
				return;
			}
			this.ContinuePlatformLogin(callback);
		});
	}

	// Token: 0x06003463 RID: 13411 RVA: 0x00110C60 File Offset: 0x0010EE60
	private void ContinuePlatformLogin(Action<ModIORequestResult> callback)
	{
		if (SteamManager.Initialized)
		{
			if (ModIOManager.RequestEncryptedAppTicketResponse == null)
			{
				ModIOManager.RequestEncryptedAppTicketResponse = CallResult<EncryptedAppTicketResponse_t>.Create(new CallResult<EncryptedAppTicketResponse_t>.APIDispatchDelegate(this.OnRequestEncryptedAppTicketFinished));
			}
			SteamAPICall_t hAPICall = SteamUser.RequestEncryptedAppTicket(null, 0);
			ModIOManager.RequestEncryptedAppTicketResponse.Set(hAPICall, null);
			ModIOManager.externalAuthCallback = callback;
			return;
		}
		UnityEvent<string> onModIOLoginFailed = ModIOManager.OnModIOLoginFailed;
		if (onModIOLoginFailed != null)
		{
			onModIOLoginFailed.Invoke("FAILED TO LOGIN TO MOD.IO VIA STEAM:\nSTEAM IS ENABLED BUT NOT INITIALIZED.");
		}
		if (callback != null)
		{
			callback(ModIORequestResult.CreateFailureResult("FAILED TO LOGIN TO MOD.IO VIA STEAM:\nSTEAM IS ENABLED BUT NOT INITIALIZED"));
		}
	}

	// Token: 0x06003464 RID: 13412 RVA: 0x00110CD4 File Offset: 0x0010EED4
	private string GetSteamEncryptedAppTicket()
	{
		Array.Resize<byte>(ref ModIOManager.ticketBlob, (int)ModIOManager.ticketSize);
		return Convert.ToBase64String(ModIOManager.ticketBlob);
	}

	// Token: 0x06003465 RID: 13413 RVA: 0x00110CF0 File Offset: 0x0010EEF0
	private void OnRequestEncryptedAppTicketFinished(EncryptedAppTicketResponse_t response, bool bIOFailure)
	{
		if (bIOFailure)
		{
			this.OnAuthenticationComplete(ModIORequestResult.CreateFailureResult("FAILED TO LOGIN TO MOD.IO VIA STEAM:\nFAILED TO RETRIEVE 'EncryptedAppTicket' DUE TO A STEAM API IO FAILURE."));
			return;
		}
		EResult eResult = response.m_eResult;
		if (eResult <= EResult.k_EResultNoConnection)
		{
			if (eResult != EResult.k_EResultOK)
			{
				if (eResult == EResult.k_EResultNoConnection)
				{
					this.OnAuthenticationComplete(ModIORequestResult.CreateFailureResult("FAILED TO LOGIN TO MOD.IO VIA STEAM:\nNOT CONNECTED TO STEAM."));
					return;
				}
			}
			else
			{
				if (!SteamUser.GetEncryptedAppTicket(ModIOManager.ticketBlob, ModIOManager.ticketBlob.Length, out ModIOManager.ticketSize))
				{
					this.OnAuthenticationComplete(ModIORequestResult.CreateFailureResult("FAILED TO LOGIN TO MOD.IO VIA STEAM:\nFAILED TO RETRIEVE 'EncryptedAppTicket'."));
					return;
				}
				ModIOUnity.AuthenticateUserViaSteam(this.GetSteamEncryptedAppTicket(), null, new TermsHash?(ModIOManager.cachedModIOTerms.hash), new Action<Result>(this.OnAuthWithSteam));
				return;
			}
		}
		else
		{
			if (eResult == EResult.k_EResultLimitExceeded)
			{
				this.OnAuthenticationComplete(ModIORequestResult.CreateFailureResult("FAILED TO LOGIN TO MOD.IO VIA STEAM:\nRATE LIMIT EXCEEDED, CAN ONLY REQUEST ONE 'EncryptedAppTicket' PER MINUTE."));
				return;
			}
			if (eResult == EResult.k_EResultDuplicateRequest)
			{
				this.OnAuthenticationComplete(ModIORequestResult.CreateFailureResult("FAILED TO LOGIN TO MOD.IO VIA STEAM:\nTHERE IS ALREADY AN 'EncryptedAppTicket' REQUEST IN PROGRESS."));
				return;
			}
		}
		this.OnAuthenticationComplete(ModIORequestResult.CreateFailureResult("FAILED TO LOGIN TO MOD.IO VIA STEAM:\n" + string.Format("{0}", response.m_eResult)));
	}

	// Token: 0x06003466 RID: 13414 RVA: 0x00110DE8 File Offset: 0x0010EFE8
	private void OnAuthWithSteam(Result result)
	{
		if (result.Succeeded())
		{
			PlayerPrefs.SetInt("modIOLassSuccessfulAuthMethod", ModIOManager.ModIOAuthMethod.Steam.GetIndex<ModIOManager.ModIOAuthMethod>());
			this.OnAuthenticationComplete(ModIORequestResult.CreateSuccessResult());
			return;
		}
		this.OnAuthenticationComplete(ModIORequestResult.CreateFailureResult("FAILED TO LOGIN TO MOD.IO VIA STEAM:\n" + result.message));
	}

	// Token: 0x06003467 RID: 13415 RVA: 0x00110E38 File Offset: 0x0010F038
	private void OnAuthenticationComplete(ModIORequestResult result)
	{
		if (result.success)
		{
			ModIOManager.loggedIn = true;
			ModIOManager.Refresh(null, true);
			this.EnableModManagement();
			ModIOManager.RefreshUserProfile();
			UnityEvent onModIOLoggedIn = ModIOManager.OnModIOLoggedIn;
			if (onModIOLoggedIn != null)
			{
				onModIOLoggedIn.Invoke();
			}
		}
		else
		{
			ModIOManager.loggedIn = false;
			UnityEvent<string> onModIOLoginFailed = ModIOManager.OnModIOLoginFailed;
			if (onModIOLoginFailed != null)
			{
				onModIOLoginFailed.Invoke(result.message);
			}
		}
		ModIOManager.loggingIn = false;
		Action<ModIORequestResult> action = ModIOManager.externalAuthCallback;
		if (action != null)
		{
			action(result);
		}
		ModIOManager.externalAuthCallback = null;
	}

	// Token: 0x06003468 RID: 13416 RVA: 0x00110EB0 File Offset: 0x0010F0B0
	public static ModIOManager.ModIOAuthMethod GetLastAuthMethod()
	{
		int @int = PlayerPrefs.GetInt("modIOLassSuccessfulAuthMethod", -1);
		if (@int == -1)
		{
			return ModIOManager.ModIOAuthMethod.Invalid;
		}
		return (ModIOManager.ModIOAuthMethod)@int;
	}

	// Token: 0x06003469 RID: 13417 RVA: 0x00110ED0 File Offset: 0x0010F0D0
	public static SubscribedMod[] GetSubscribedMods()
	{
		Result result;
		SubscribedMod[] subscribedMods = ModIOUnity.GetSubscribedMods(out result);
		if (result.Succeeded())
		{
			return subscribedMods;
		}
		return null;
	}

	// Token: 0x0600346A RID: 13418 RVA: 0x00110EF4 File Offset: 0x0010F0F4
	public static void SubscribeToMod(ModId modId, Action<Result> callback)
	{
		ModIOUnity.SubscribeToMod(modId, delegate(Result result)
		{
			Action<Result> callback2 = callback;
			if (callback2 == null)
			{
				return;
			}
			callback2(result);
		});
	}

	// Token: 0x0600346B RID: 13419 RVA: 0x00110F20 File Offset: 0x0010F120
	public static void UnsubscribeFromMod(ModId modId, Action<Result> callback)
	{
		ModIOUnity.UnsubscribeFromMod(modId, delegate(Result result)
		{
			Action<Result> callback2 = callback;
			if (callback2 == null)
			{
				return;
			}
			callback2(result);
		});
	}

	// Token: 0x0600346C RID: 13420 RVA: 0x00110F4C File Offset: 0x0010F14C
	public static bool GetSubscribedModStatus(ModId modId, out SubscribedModStatus modStatus)
	{
		modStatus = SubscribedModStatus.None;
		if (!ModIOManager.hasInstance)
		{
			return false;
		}
		Result result;
		SubscribedMod[] subscribedMods = ModIOUnity.GetSubscribedMods(out result);
		if (!result.Succeeded())
		{
			return false;
		}
		foreach (SubscribedMod subscribedMod in subscribedMods)
		{
			if (subscribedMod.modProfile.id.Equals(modId))
			{
				modStatus = subscribedMod.status;
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600346D RID: 13421 RVA: 0x00110FB4 File Offset: 0x0010F1B4
	public static bool GetSubscribedModProfile(ModId modId, out SubscribedMod subscribedMod)
	{
		subscribedMod = default(SubscribedMod);
		if (!ModIOManager.hasInstance)
		{
			return false;
		}
		Result result;
		SubscribedMod[] subscribedMods = ModIOUnity.GetSubscribedMods(out result);
		if (!result.Succeeded())
		{
			return false;
		}
		foreach (SubscribedMod subscribedMod2 in subscribedMods)
		{
			if (subscribedMod2.modProfile.id.Equals(modId))
			{
				subscribedMod = subscribedMod2;
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600346E RID: 13422 RVA: 0x0011101C File Offset: 0x0010F21C
	public static void DownloadMod(ModId modId, Action<ModIORequestResult> callback)
	{
		if (!ModIOManager.hasInstance)
		{
			return;
		}
		if (!ModIOUnity.IsModManagementBusy())
		{
			ModIOUnity.DownloadNow(modId, delegate(Result result)
			{
				if (result.Succeeded())
				{
					Action<ModIORequestResult> callback3 = callback;
					if (callback3 == null)
					{
						return;
					}
					callback3(ModIORequestResult.CreateSuccessResult());
					return;
				}
				else
				{
					Action<ModIORequestResult> callback4 = callback;
					if (callback4 == null)
					{
						return;
					}
					callback4(ModIORequestResult.CreateFailureResult(result.message));
					return;
				}
			});
			return;
		}
		if (!ModIOManager.modDownloadQueue.Contains(modId))
		{
			ModIOManager.modDownloadQueue.Add(modId);
		}
		Action<ModIORequestResult> callback2 = callback;
		if (callback2 == null)
		{
			return;
		}
		callback2(ModIORequestResult.CreateSuccessResult());
	}

	// Token: 0x0600346F RID: 13423 RVA: 0x00111085 File Offset: 0x0010F285
	public static bool IsModInDownloadQueue(ModId modId)
	{
		return ModIOManager.hasInstance && ModIOManager.modDownloadQueue.Contains(modId);
	}

	// Token: 0x06003470 RID: 13424 RVA: 0x0011109B File Offset: 0x0010F29B
	public static void AbortModDownload(ModId modId)
	{
		if (!ModIOManager.hasInstance)
		{
			return;
		}
		if (ModIOManager.modDownloadQueue.Contains(modId))
		{
			ModIOManager.modDownloadQueue.Remove(modId);
			return;
		}
		if (ModIOUnity.IsModManagementBusy())
		{
			ModIOUnity.AbortDownload(modId);
		}
	}

	// Token: 0x06003471 RID: 13425 RVA: 0x001110CC File Offset: 0x0010F2CC
	private void OnJoinedRoom()
	{
		if (NetworkSystem.Instance.RoomName.Contains(GorillaComputer.instance.VStumpRoomPrepend) && !GorillaComputer.instance.IsPlayerInVirtualStump() && !CustomMapManager.IsLocalPlayerInVirtualStump())
		{
			Debug.LogError("[ModIOManager::OnJoinedRoom] Player joined @ room while not in the VStump! Leaving the room...");
			NetworkSystem.Instance.ReturnToSinglePlayer();
		}
	}

	// Token: 0x06003472 RID: 13426 RVA: 0x00111120 File Offset: 0x0010F320
	public static ModId GetNewMapsModId()
	{
		if (!ModIOManager.hasInstance)
		{
			return ModId.Null;
		}
		return new ModId(ModIOManager.instance.newMapsModId);
	}

	// Token: 0x04004122 RID: 16674
	private const string MODIO_ACCEPTED_TERMS_KEY = "modIOAcceptedTermsHash";

	// Token: 0x04004123 RID: 16675
	private const string MODIO_LAST_AUTH_METHOD_KEY = "modIOLassSuccessfulAuthMethod";

	// Token: 0x04004124 RID: 16676
	private static readonly UserProfile NULL_USER_PROFILE = new UserProfile
	{
		userId = -1L
	};

	// Token: 0x04004125 RID: 16677
	private const float REFRESH_RATE_LIMIT = 5f;

	// Token: 0x04004126 RID: 16678
	[OnEnterPlay_SetNull]
	private static volatile ModIOManager instance;

	// Token: 0x04004127 RID: 16679
	[OnEnterPlay_Set(false)]
	private static bool hasInstance;

	// Token: 0x04004128 RID: 16680
	private static readonly List<ModManagementOperationType> allowedAutomaticModOperations = new List<ModManagementOperationType>
	{
		ModManagementOperationType.Install,
		ModManagementOperationType.Uninstall
	};

	// Token: 0x04004129 RID: 16681
	private static bool initialized;

	// Token: 0x0400412A RID: 16682
	private static bool refreshing;

	// Token: 0x0400412B RID: 16683
	private static bool modManagementEnabled;

	// Token: 0x0400412C RID: 16684
	private static bool loggingIn;

	// Token: 0x0400412D RID: 16685
	private static bool loggedIn;

	// Token: 0x0400412E RID: 16686
	private static Coroutine preInitCoroutine;

	// Token: 0x0400412F RID: 16687
	private static Coroutine refreshDisabledCoroutine;

	// Token: 0x04004130 RID: 16688
	private static UserProfile cachedUserProfile = ModIOManager.NULL_USER_PROFILE;

	// Token: 0x04004131 RID: 16689
	private static float lastRefreshTime;

	// Token: 0x04004132 RID: 16690
	private static List<Action<bool>> currentRefreshCallbacks = new List<Action<bool>>();

	// Token: 0x04004133 RID: 16691
	private static TermsOfUse cachedModIOTerms;

	// Token: 0x04004134 RID: 16692
	private static Action<ModIORequestResultAnd<bool>> modIOTermsAcknowledgedCallback;

	// Token: 0x04004135 RID: 16693
	private static List<ModId> modDownloadQueue = new List<ModId>();

	// Token: 0x04004136 RID: 16694
	private static Dictionary<long, int> outdatedModCMSVersions = new Dictionary<long, int>();

	// Token: 0x04004137 RID: 16695
	private static Action<ModIORequestResult> externalAuthCallback;

	// Token: 0x04004138 RID: 16696
	private static ExternalAuthenticationToken externalAuthenticationToken;

	// Token: 0x04004139 RID: 16697
	private static Action<ModIORequestResult, string, string> externalAuthGetCodeCallback;

	// Token: 0x0400413A RID: 16698
	private static byte[] ticketBlob = new byte[1024];

	// Token: 0x0400413B RID: 16699
	private static uint ticketSize;

	// Token: 0x0400413C RID: 16700
	protected static CallResult<EncryptedAppTicketResponse_t> RequestEncryptedAppTicketResponse = null;

	// Token: 0x0400413D RID: 16701
	[SerializeField]
	private GameObject modIOTermsOfUsePrefab;

	// Token: 0x0400413E RID: 16702
	[SerializeField]
	private long newMapsModId;

	// Token: 0x04004142 RID: 16706
	public static UnityEvent OnModIOLoginStarted = new UnityEvent();

	// Token: 0x04004143 RID: 16707
	public static UnityEvent OnModIOLoggedIn = new UnityEvent();

	// Token: 0x04004144 RID: 16708
	public static UnityEvent<string> OnModIOLoginFailed = new UnityEvent<string>();

	// Token: 0x04004145 RID: 16709
	public static UnityEvent OnModIOLoggedOut = new UnityEvent();

	// Token: 0x04004146 RID: 16710
	public static UnityEvent<UserProfile> OnModIOUserProfileUpdated = new UnityEvent<UserProfile>();

	// Token: 0x02000827 RID: 2087
	public enum ModIOAuthMethod
	{
		// Token: 0x04004148 RID: 16712
		Invalid,
		// Token: 0x04004149 RID: 16713
		LinkedAccount,
		// Token: 0x0400414A RID: 16714
		Steam,
		// Token: 0x0400414B RID: 16715
		Oculus
	}

	// Token: 0x02000828 RID: 2088
	// (Invoke) Token: 0x06003477 RID: 13431
	public delegate void ModIOEnabled();

	// Token: 0x02000829 RID: 2089
	// (Invoke) Token: 0x0600347B RID: 13435
	public delegate void ModIODisabled();

	// Token: 0x0200082A RID: 2090
	// (Invoke) Token: 0x0600347F RID: 13439
	public delegate void InitializationFinished();
}
