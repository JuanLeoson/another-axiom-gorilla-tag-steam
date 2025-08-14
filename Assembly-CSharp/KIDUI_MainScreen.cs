using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GorillaNetworking;
using KID.Model;
using UnityEngine;

// Token: 0x02000944 RID: 2372
public class KIDUI_MainScreen : MonoBehaviour
{
	// Token: 0x06003A5F RID: 14943 RVA: 0x0012DCE1 File Offset: 0x0012BEE1
	private void Awake()
	{
		KIDUI_MainScreen._featuresList.Clear();
		if (this._setupKidScreen == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Setup K-ID Screen is NULL", Array.Empty<object>());
			return;
		}
		if (this._initialised)
		{
			return;
		}
		this.InitialiseMainScreen();
	}

	// Token: 0x06003A60 RID: 14944 RVA: 0x0012DD1A File Offset: 0x0012BF1A
	private void OnEnable()
	{
		KIDManager.RegisterSessionUpdateCallback_AnyPermission(new Action(this.UpdatePermissionsAndFeaturesScreen));
		this.UpdatePermissionsAndFeaturesScreen();
	}

	// Token: 0x06003A61 RID: 14945 RVA: 0x0012DD33 File Offset: 0x0012BF33
	private void OnDisable()
	{
		KIDManager.UnregisterSessionUpdateCallback_AnyPermission(new Action(this.UpdatePermissionsAndFeaturesScreen));
	}

	// Token: 0x06003A62 RID: 14946 RVA: 0x000023F5 File Offset: 0x000005F5
	private void OnDestroy()
	{
	}

	// Token: 0x06003A63 RID: 14947 RVA: 0x0012DD48 File Offset: 0x0012BF48
	private void ConstructFeatureSettings()
	{
		for (int i = 0; i < this._displayOrder.Length; i++)
		{
			for (int j = 0; j < this._featureSetups.Count; j++)
			{
				if (this._featureSetups[j].linkedFeature == this._displayOrder[i])
				{
					this.CreateNewFeatureDisplay(this._featureSetups[j]);
					break;
				}
			}
		}
		this.UpdatePermissionsAndFeaturesScreen();
	}

	// Token: 0x06003A64 RID: 14948 RVA: 0x0012DDB4 File Offset: 0x0012BFB4
	private void CreateNewFeatureDisplay(KIDUI_MainScreen.FeatureToggleSetup setup)
	{
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(setup.linkedFeature);
		if (permissionDataByFeature == null)
		{
			Debug.LogErrorFormat("[KID::UI::MAIN] Failed to retrieve permission data for feature; [" + setup.linkedFeature.ToString() + "]", Array.Empty<object>());
			return;
		}
		if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
		{
			return;
		}
		if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PLAYER)
		{
			if (permissionDataByFeature.Enabled)
			{
				return;
			}
			if (KIDManager.CheckFeatureOptIn(setup.linkedFeature, null).Item2)
			{
				return;
			}
		}
		if (setup.alwaysCheckFeatureSetting && KIDManager.CheckFeatureSettingEnabled(setup.linkedFeature))
		{
			return;
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this._featurePrefab, this._featureRootTransform);
		KIDUIFeatureSetting component = gameObject.GetComponent<KIDUIFeatureSetting>();
		if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.GUARDIAN)
		{
			Debug.LogFormat(string.Format("[KID::UI::MAIN_SCREEN] Adding new Locked Feature:  {0} Is enabled: {1}", setup.linkedFeature.ToString(), permissionDataByFeature.Enabled), Array.Empty<object>());
			component.CreateNewFeatureSettingGuardianManaged(setup, permissionDataByFeature.Enabled);
			if (!KIDUI_MainScreen._featuresList.ContainsKey(setup.linkedFeature))
			{
				KIDUI_MainScreen._featuresList.Add(setup.linkedFeature, new List<KIDUIFeatureSetting>());
			}
			KIDUI_MainScreen._featuresList[setup.linkedFeature].Add(component);
			return;
		}
		if (setup.requiresToggle)
		{
			component.CreateNewFeatureSettingWithToggle(setup, false, setup.alwaysCheckFeatureSetting);
		}
		else
		{
			component.CreateNewFeatureSettingWithoutToggle(setup, setup.alwaysCheckFeatureSetting);
		}
		if (!KIDUI_MainScreen._featuresList.ContainsKey(setup.linkedFeature))
		{
			KIDUI_MainScreen._featuresList.Add(setup.linkedFeature, new List<KIDUIFeatureSetting>());
		}
		KIDUI_MainScreen._featuresList[setup.linkedFeature].Add(component);
		this.ConstructAdditionalSetup(setup.linkedFeature, gameObject);
	}

	// Token: 0x06003A65 RID: 14949 RVA: 0x0012DF50 File Offset: 0x0012C150
	private void ConstructAdditionalSetup(EKIDFeatures feature, GameObject featureObject)
	{
	}

	// Token: 0x06003A66 RID: 14950 RVA: 0x0012DF58 File Offset: 0x0012C158
	private void UpdatePermissionsAndFeaturesScreen()
	{
		int num = 0;
		Debug.LogFormat(string.Format("[KID::UI::MAIN] Updated Feature listings. To Update: [{0}]", KIDUI_MainScreen._featuresList.Count), Array.Empty<object>());
		foreach (KeyValuePair<EKIDFeatures, List<KIDUIFeatureSetting>> keyValuePair in KIDUI_MainScreen._featuresList)
		{
			for (int i = 0; i < keyValuePair.Value.Count; i++)
			{
				Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(keyValuePair.Key);
				if (permissionDataByFeature == null)
				{
					Debug.LogErrorFormat("[KID::UI::MAIN] Failed to find permission data for feature: [" + keyValuePair.Key.ToString() + "]", Array.Empty<object>());
				}
				else if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.GUARDIAN)
				{
					keyValuePair.Value[i].SetGuardianManagedState(permissionDataByFeature.Enabled);
				}
				else
				{
					bool isOptedIn = KIDManager.CheckFeatureOptIn(keyValuePair.Key, permissionDataByFeature).Item2;
					if (keyValuePair.Value[i].AlwaysCheckFeatureSetting)
					{
						isOptedIn = KIDManager.CheckFeatureSettingEnabled(keyValuePair.Key);
					}
					if (!keyValuePair.Value[i].GetHasToggle())
					{
						keyValuePair.Value[i].SetPlayerManagedState(permissionDataByFeature.Enabled, isOptedIn);
					}
				}
			}
		}
		int num2 = 0;
		foreach (KeyValuePair<EKIDFeatures, List<KIDUIFeatureSetting>> keyValuePair2 in KIDUI_MainScreen._featuresList)
		{
			for (int j = 0; j < keyValuePair2.Value.Count; j++)
			{
				num2++;
				Permission permissionDataByFeature2 = KIDManager.GetPermissionDataByFeature(keyValuePair2.Key);
				if (keyValuePair2.Value[j].GetFeatureToggleState() || permissionDataByFeature2.ManagedBy == Permission.ManagedByEnum.PLAYER)
				{
					num++;
				}
			}
		}
		if (num >= num2)
		{
			this._hasAllPermissions = true;
			this._getPermissionsButton.gameObject.SetActive(false);
			this._gettingPermissionsButton.gameObject.SetActive(false);
			this._requestPermissionsButton.gameObject.SetActive(false);
			this._permissionsTip.SetActive(false);
			this.SetButtonContainersVisibility(EGetPermissionsStatus.RequestedPermission);
		}
	}

	// Token: 0x06003A67 RID: 14951 RVA: 0x0012E1C0 File Offset: 0x0012C3C0
	private bool IsFeatureToggledOn(EKIDFeatures permissionFeature)
	{
		List<KIDUIFeatureSetting> source;
		if (!KIDUI_MainScreen._featuresList.TryGetValue(permissionFeature, out source))
		{
			return true;
		}
		KIDUIFeatureSetting kiduifeatureSetting = source.FirstOrDefault<KIDUIFeatureSetting>();
		if (kiduifeatureSetting == null)
		{
			Debug.LogErrorFormat(string.Format("[KID::UI::MAIN] Empty list for permission Name [{0}]", permissionFeature), Array.Empty<object>());
			return false;
		}
		return kiduifeatureSetting.GetFeatureToggleState();
	}

	// Token: 0x06003A68 RID: 14952 RVA: 0x0012E210 File Offset: 0x0012C410
	public void InitialiseMainScreen()
	{
		if (this._initialised)
		{
			Debug.Log("[KID::MAIN_SCREEN] Already Initialised");
			return;
		}
		this.ConstructFeatureSettings();
		this._declinedStatus.SetActive(false);
		this._timeoutStatus.SetActive(false);
		this._pendingStatus.SetActive(false);
		this._updatedStatus.SetActive(false);
		this._setupRequiredStatus.SetActive(false);
		this._missingStatus.SetActive(false);
		this._fullPlayerControlStatus.SetActive(false);
		this._initialised = true;
	}

	// Token: 0x06003A69 RID: 14953 RVA: 0x0012E294 File Offset: 0x0012C494
	public void ShowMainScreen(EMainScreenStatus showStatus, KIDUI_Controller.Metrics_ShowReason reason)
	{
		this.ShowMainScreen(showStatus);
		this._mainScreenOpenedReason = reason;
		string value = reason.ToString().Replace("_", "-").ToLower();
		KIDTelemetryData kidtelemetryData = new KIDTelemetryData
		{
			EventName = "kid_game_settings",
			CustomTags = new string[]
			{
				"kid_setup",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment,
				KIDTelemetry.Open_MetricActionCustomTag
			},
			BodyData = new Dictionary<string, string>
			{
				{
					"screen_shown_reason",
					value
				}
			}
		};
		foreach (Permission permission in KIDManager.GetAllPermissionsData())
		{
			kidtelemetryData.BodyData.Add(KIDTelemetry.GetPermissionManagedByBodyData(permission.Name), permission.ManagedBy.ToString().ToLower());
			kidtelemetryData.BodyData.Add(KIDTelemetry.GetPermissionEnabledBodyData(permission.Name), permission.Enabled.ToString().ToLower());
		}
		GorillaTelemetry.SendMothershipAnalytics(kidtelemetryData);
	}

	// Token: 0x06003A6A RID: 14954 RVA: 0x0012E3D0 File Offset: 0x0012C5D0
	public void ShowMainScreen(EMainScreenStatus showStatus)
	{
		KIDUI_MainScreen.ShownSettingsScreen = true;
		base.gameObject.SetActive(true);
		this.ConfigurePermissionsButtons();
		this.UpdateScreenStatus(showStatus, false);
	}

	// Token: 0x06003A6B RID: 14955 RVA: 0x0012E3F4 File Offset: 0x0012C5F4
	public void UpdateScreenStatus(EMainScreenStatus showStatus, bool sendMetrics = false)
	{
		if (sendMetrics && showStatus == EMainScreenStatus.Updated)
		{
			string value = this._mainScreenOpenedReason.ToString().Replace("_", "-").ToLower();
			KIDTelemetryData kidtelemetryData = new KIDTelemetryData
			{
				EventName = "kid_game_settings",
				CustomTags = new string[]
				{
					"kid_setup",
					KIDTelemetry.GameVersionCustomTag,
					KIDTelemetry.GameEnvironment,
					KIDTelemetry.Updated_MetricActionCustomTag
				},
				BodyData = new Dictionary<string, string>
				{
					{
						"screen_shown_reason",
						value
					}
				}
			};
			foreach (Permission permission in KIDManager.GetAllPermissionsData())
			{
				kidtelemetryData.BodyData.Add(KIDTelemetry.GetPermissionManagedByBodyData(permission.Name), permission.ManagedBy.ToString().ToLower());
				kidtelemetryData.BodyData.Add(KIDTelemetry.GetPermissionEnabledBodyData(permission.Name), permission.Enabled.ToString().ToLower());
			}
			GorillaTelemetry.SendMothershipAnalytics(kidtelemetryData);
		}
		GameObject activeStatusObject = this.GetActiveStatusObject();
		this._declinedStatus.SetActive(false);
		this._timeoutStatus.SetActive(false);
		this._pendingStatus.SetActive(false);
		this._updatedStatus.SetActive(false);
		this._setupRequiredStatus.SetActive(false);
		this._missingStatus.SetActive(false);
		this._fullPlayerControlStatus.SetActive(false);
		switch (showStatus)
		{
		default:
			if (!this._hasAllPermissions)
			{
				this._missingStatus.SetActive(true);
			}
			else if (this._hasAllPermissions)
			{
				this._fullPlayerControlStatus.SetActive(true);
			}
			else
			{
				this._screenStatus = showStatus;
			}
			break;
		case EMainScreenStatus.Declined:
			this._declinedStatus.SetActive(true);
			this._screenStatus = showStatus;
			break;
		case EMainScreenStatus.Pending:
			this._pendingStatus.SetActive(true);
			this._screenStatus = showStatus;
			break;
		case EMainScreenStatus.Timedout:
			this._timeoutStatus.SetActive(true);
			this._screenStatus = showStatus;
			break;
		case EMainScreenStatus.Setup:
			this._setupRequiredStatus.SetActive(true);
			this._screenStatus = showStatus;
			break;
		case EMainScreenStatus.Previous:
			if (activeStatusObject != null)
			{
				activeStatusObject.SetActive(true);
			}
			else
			{
				this._updatedStatus.SetActive(true);
			}
			break;
		case EMainScreenStatus.FullControl:
			this._fullPlayerControlStatus.SetActive(true);
			break;
		}
		this.SetButtonContainersVisibility(KIDUI_MainScreen.GetPermissionState());
	}

	// Token: 0x06003A6C RID: 14956 RVA: 0x00020127 File Offset: 0x0001E327
	public void HideMainScreen()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06003A6D RID: 14957 RVA: 0x0012E680 File Offset: 0x0012C880
	public void OnAskForPermission()
	{
		KIDUI_MainScreen.<OnAskForPermission>d__50 <OnAskForPermission>d__;
		<OnAskForPermission>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnAskForPermission>d__.<>4__this = this;
		<OnAskForPermission>d__.<>1__state = -1;
		<OnAskForPermission>d__.<>t__builder.Start<KIDUI_MainScreen.<OnAskForPermission>d__50>(ref <OnAskForPermission>d__);
	}

	// Token: 0x06003A6E RID: 14958 RVA: 0x0012E6B8 File Offset: 0x0012C8B8
	public void OnSaveAndExit()
	{
		if (KIDManager.CurrentSession == null)
		{
			Debug.LogError("[KID::KID_UI_MAINSCREEN] There is no session as such cannot opt into anything");
			KIDUI_Controller.Instance.CloseKIDScreens();
			return;
		}
		List<Permission> allPermissionsData = KIDManager.GetAllPermissionsData();
		for (int i = 0; i < allPermissionsData.Count; i++)
		{
			string name = allPermissionsData[i].Name;
			if (!(name == "multiplayer"))
			{
				if (!(name == "mods"))
				{
					if (!(name == "join-groups"))
					{
						if (!(name == "voice-chat"))
						{
							if (!(name == "custom-username"))
							{
								Debug.LogError("[KID::UI::MainScreen] Unhandled permission when saving and exiting: [" + allPermissionsData[i].Name + "]");
							}
							else
							{
								this.UpdateOptInSetting(allPermissionsData[i], EKIDFeatures.Custom_Nametags, delegate(bool b, Permission p, bool hasOptedInPreviously)
								{
									GorillaComputer.instance.SetNametagSetting(b, p.ManagedBy, hasOptedInPreviously);
								});
							}
						}
						else
						{
							this.UpdateOptInSetting(allPermissionsData[i], EKIDFeatures.Voice_Chat, delegate(bool b, Permission p, bool hasOptedInPreviously)
							{
								GorillaComputer.instance.KID_SetVoiceChatSettingOnStart(b, p.ManagedBy, hasOptedInPreviously);
							});
						}
					}
				}
				else
				{
					this.UpdateOptInSetting(allPermissionsData[i], EKIDFeatures.Mods, null);
				}
			}
			else
			{
				this.UpdateOptInSetting(allPermissionsData[i], EKIDFeatures.Multiplayer, null);
			}
		}
		KIDManager.SendOptInPermissions();
		if (this._screenStatus != EMainScreenStatus.None)
		{
			string value = this._mainScreenOpenedReason.ToString().Replace("_", "-").ToLower();
			GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
			{
				EventName = "kid_game_settings",
				CustomTags = new string[]
				{
					"kid_setup",
					KIDTelemetry.GameVersionCustomTag,
					KIDTelemetry.GameEnvironment
				},
				BodyData = new Dictionary<string, string>
				{
					{
						"screen_shown_reason",
						value
					},
					{
						"kid_status",
						this._screenStatus.ToString().ToLower()
					},
					{
						"button_pressed",
						"save_and_continue"
					}
				}
			});
		}
		else
		{
			Debug.LogError("[KID::UI::MAIN_SCREEN] Trying to close k-ID Main Screen, but screen status is set to [None] - Invalid status, will not submit analytics");
		}
		KIDUI_Controller.Instance.CloseKIDScreens();
	}

	// Token: 0x06003A6F RID: 14959 RVA: 0x0012E8D0 File Offset: 0x0012CAD0
	public int GetFeatureListingCount()
	{
		int num = 0;
		foreach (List<KIDUIFeatureSetting> list in KIDUI_MainScreen._featuresList.Values)
		{
			num += list.Count;
		}
		return num;
	}

	// Token: 0x06003A70 RID: 14960 RVA: 0x0012E92C File Offset: 0x0012CB2C
	private Task<bool> UpdateAndCheckForMissingPermissions()
	{
		KIDUI_MainScreen.<UpdateAndCheckForMissingPermissions>d__53 <UpdateAndCheckForMissingPermissions>d__;
		<UpdateAndCheckForMissingPermissions>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UpdateAndCheckForMissingPermissions>d__.<>4__this = this;
		<UpdateAndCheckForMissingPermissions>d__.<>1__state = -1;
		<UpdateAndCheckForMissingPermissions>d__.<>t__builder.Start<KIDUI_MainScreen.<UpdateAndCheckForMissingPermissions>d__53>(ref <UpdateAndCheckForMissingPermissions>d__);
		return <UpdateAndCheckForMissingPermissions>d__.<>t__builder.Task;
	}

	// Token: 0x06003A71 RID: 14961 RVA: 0x0012E970 File Offset: 0x0012CB70
	private void UpdateOptInSetting(Permission permissionData, EKIDFeatures feature, Action<bool, Permission, bool> onOptedIn)
	{
		bool item = KIDManager.CheckFeatureOptIn(feature, permissionData).Item2;
		bool flag = this.IsFeatureToggledOn(feature);
		Debug.Log(string.Format("[KID::UI::MainScreen] Update opt in for {0}. Has opted in: {1}. Toggled on: {2}", feature.ToString(), item, flag));
		KIDManager.SetFeatureOptIn(feature, flag);
		if (onOptedIn != null)
		{
			onOptedIn(flag, permissionData, item);
		}
	}

	// Token: 0x06003A72 RID: 14962 RVA: 0x0012E9CD File Offset: 0x0012CBCD
	public void OnConfirmedEmailAddress(string emailAddress)
	{
		this._emailAddress = emailAddress;
		Debug.LogFormat("[KID::UI::Main] Email has been confirmed: " + this._emailAddress, Array.Empty<object>());
	}

	// Token: 0x06003A73 RID: 14963 RVA: 0x0012E9F0 File Offset: 0x0012CBF0
	private IEnumerable<string> CollectPermissionsToUpgrade()
	{
		return from permission in KIDManager.GetAllPermissionsData()
		where permission.ManagedBy == Permission.ManagedByEnum.GUARDIAN && !permission.Enabled
		select permission.Name;
	}

	// Token: 0x06003A74 RID: 14964 RVA: 0x0012EA4C File Offset: 0x0012CC4C
	private void ConfigurePermissionsButtons()
	{
		Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS");
		if (!this._getPermissionsButton.gameObject.activeSelf && !this._gettingPermissionsButton.gameObject.activeSelf)
		{
			Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - GET PERMISSIONS IS DISABLED");
			return;
		}
		Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - CHECK SESSION STATUS: Is Default: [" + KIDManager.CurrentSession.IsDefault.ToString() + "]");
		this.SetButtonContainersVisibility(KIDUI_MainScreen.GetPermissionState());
	}

	// Token: 0x06003A75 RID: 14965 RVA: 0x0012EAC0 File Offset: 0x0012CCC0
	private void SetButtonContainersVisibility(EGetPermissionsStatus permissionStatus)
	{
		Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - PERMISSION STATE: [" + permissionStatus.ToString() + "]");
		this._defaultButtonsContainer.SetActive(permissionStatus == EGetPermissionsStatus.GetPermission);
		this._permissionsRequestingButtonContainer.SetActive(permissionStatus == EGetPermissionsStatus.RequestingPermission);
		this._permissionsRequestedButtonContainer.SetActive(permissionStatus == EGetPermissionsStatus.RequestedPermission);
	}

	// Token: 0x06003A76 RID: 14966 RVA: 0x0012EB1C File Offset: 0x0012CD1C
	private GameObject GetActiveStatusObject()
	{
		foreach (GameObject gameObject in new List<GameObject>
		{
			this._declinedStatus,
			this._timeoutStatus,
			this._pendingStatus,
			this._updatedStatus,
			this._setupRequiredStatus,
			this._fullPlayerControlStatus
		})
		{
			if (gameObject.activeInHierarchy)
			{
				return gameObject;
			}
		}
		return null;
	}

	// Token: 0x06003A77 RID: 14967 RVA: 0x0012EBC0 File Offset: 0x0012CDC0
	private static EGetPermissionsStatus GetPermissionState()
	{
		if (!KIDManager.CurrentSession.IsDefault)
		{
			Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - SHOW REQUESTED");
			return EGetPermissionsStatus.RequestedPermission;
		}
		if (PlayerPrefs.GetInt(KIDManager.GetChallengedBeforePlayerPrefRef, 0) == 0)
		{
			Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - SHOW DEFAULT");
			return EGetPermissionsStatus.GetPermission;
		}
		Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - SHOW SWAPPED DEFAULT");
		return EGetPermissionsStatus.RequestingPermission;
	}

	// Token: 0x06003A78 RID: 14968 RVA: 0x0012EC00 File Offset: 0x0012CE00
	private void OnFeatureToggleChanged(EKIDFeatures feature)
	{
		switch (feature)
		{
		case EKIDFeatures.Multiplayer:
			this.OnMultiplayerToggled();
			return;
		case EKIDFeatures.Custom_Nametags:
			this.OnCustomNametagsToggled();
			return;
		case EKIDFeatures.Voice_Chat:
			this.OnVoiceChatToggled();
			return;
		case EKIDFeatures.Mods:
			this.OnModToggleChanged();
			return;
		case EKIDFeatures.Groups:
			this.OnGroupToggleChanged();
			return;
		default:
			Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] Toggle NOT YET IMPLEMENTED for Feature: " + feature.ToString() + ".", Array.Empty<object>());
			return;
		}
	}

	// Token: 0x06003A79 RID: 14969 RVA: 0x0012EC72 File Offset: 0x0012CE72
	private void OnMultiplayerToggled()
	{
		Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] MULTIPLAYER Toggle NOT YET IMPLEMENTED.", Array.Empty<object>());
	}

	// Token: 0x06003A7A RID: 14970 RVA: 0x0012EC83 File Offset: 0x0012CE83
	private void OnVoiceChatToggled()
	{
		Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] VOICE CHAT Toggle NOT YET IMPLEMENTED.", Array.Empty<object>());
	}

	// Token: 0x06003A7B RID: 14971 RVA: 0x0012EC94 File Offset: 0x0012CE94
	private void OnGroupToggleChanged()
	{
		Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] GROUPS Toggle NOT YET IMPLEMENTED.", Array.Empty<object>());
	}

	// Token: 0x06003A7C RID: 14972 RVA: 0x0012ECA5 File Offset: 0x0012CEA5
	private void OnModToggleChanged()
	{
		Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] MODS Toggle NOT YET IMPLEMENTED.", Array.Empty<object>());
	}

	// Token: 0x06003A7D RID: 14973 RVA: 0x0012ECB6 File Offset: 0x0012CEB6
	private void OnCustomNametagsToggled()
	{
		Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] CUSTOM USERNAMES Toggle NOT YET IMPLEMENTED.", Array.Empty<object>());
	}

	// Token: 0x0400479F RID: 18335
	public const string OPT_IN_SUFFIX = "-opt-in";

	// Token: 0x040047A0 RID: 18336
	public static bool ShownSettingsScreen = false;

	// Token: 0x040047A1 RID: 18337
	[SerializeField]
	private GameObject _kidScreensGroup;

	// Token: 0x040047A2 RID: 18338
	[SerializeField]
	private KIDUI_SetupScreen _setupKidScreen;

	// Token: 0x040047A3 RID: 18339
	[SerializeField]
	private KIDUI_SendUpgradeEmailScreen _sendUpgradeEmailScreen;

	// Token: 0x040047A4 RID: 18340
	[SerializeField]
	private KIDUI_AnimatedEllipsis _animatedEllipsis;

	// Token: 0x040047A5 RID: 18341
	[Header("Permission Request Buttons")]
	[SerializeField]
	private KIDUIButton _getPermissionsButton;

	// Token: 0x040047A6 RID: 18342
	[SerializeField]
	private KIDUIButton _gettingPermissionsButton;

	// Token: 0x040047A7 RID: 18343
	[SerializeField]
	private KIDUIButton _requestPermissionsButton;

	// Token: 0x040047A8 RID: 18344
	[SerializeField]
	private GameObject _defaultButtonsContainer;

	// Token: 0x040047A9 RID: 18345
	[SerializeField]
	private GameObject _permissionsRequestingButtonContainer;

	// Token: 0x040047AA RID: 18346
	[SerializeField]
	private GameObject _permissionsRequestedButtonContainer;

	// Token: 0x040047AB RID: 18347
	private bool _hasAllPermissions;

	// Token: 0x040047AC RID: 18348
	[Header("Dynamic Feature Settings Setup")]
	[SerializeField]
	private GameObject _featurePrefab;

	// Token: 0x040047AD RID: 18349
	[SerializeField]
	private Transform _featureRootTransform;

	// Token: 0x040047AE RID: 18350
	[SerializeField]
	private EKIDFeatures[] _displayOrder = new EKIDFeatures[4];

	// Token: 0x040047AF RID: 18351
	[SerializeField]
	private List<KIDUI_MainScreen.FeatureToggleSetup> _featureSetups = new List<KIDUI_MainScreen.FeatureToggleSetup>();

	// Token: 0x040047B0 RID: 18352
	[Header("Additional Feature-Specific Setup")]
	[SerializeField]
	private GameObject _voiceChatLabel;

	// Token: 0x040047B1 RID: 18353
	[Header("Hide Permissions Tip")]
	[SerializeField]
	private GameObject _permissionsTip;

	// Token: 0x040047B2 RID: 18354
	[Header("Game Status Setup")]
	[SerializeField]
	private GameObject _missingStatus;

	// Token: 0x040047B3 RID: 18355
	[SerializeField]
	private GameObject _updatedStatus;

	// Token: 0x040047B4 RID: 18356
	[SerializeField]
	private GameObject _declinedStatus;

	// Token: 0x040047B5 RID: 18357
	[SerializeField]
	private GameObject _pendingStatus;

	// Token: 0x040047B6 RID: 18358
	[SerializeField]
	private GameObject _timeoutStatus;

	// Token: 0x040047B7 RID: 18359
	[SerializeField]
	private GameObject _setupRequiredStatus;

	// Token: 0x040047B8 RID: 18360
	[SerializeField]
	private GameObject _fullPlayerControlStatus;

	// Token: 0x040047B9 RID: 18361
	private string _emailAddress;

	// Token: 0x040047BA RID: 18362
	private bool _multiplayerEnabled;

	// Token: 0x040047BB RID: 18363
	private bool _customNameEnabled;

	// Token: 0x040047BC RID: 18364
	private bool _voiceChatEnabled;

	// Token: 0x040047BD RID: 18365
	private bool _initialised;

	// Token: 0x040047BE RID: 18366
	private KIDUI_Controller.Metrics_ShowReason _mainScreenOpenedReason;

	// Token: 0x040047BF RID: 18367
	private EMainScreenStatus _screenStatus;

	// Token: 0x040047C0 RID: 18368
	private GameObject _eventSystemObj;

	// Token: 0x040047C1 RID: 18369
	private static Dictionary<EKIDFeatures, List<KIDUIFeatureSetting>> _featuresList = new Dictionary<EKIDFeatures, List<KIDUIFeatureSetting>>();

	// Token: 0x02000945 RID: 2373
	[Serializable]
	public struct FeatureToggleSetup
	{
		// Token: 0x040047C2 RID: 18370
		public EKIDFeatures linkedFeature;

		// Token: 0x040047C3 RID: 18371
		public string permissionName;

		// Token: 0x040047C4 RID: 18372
		public string featureName;

		// Token: 0x040047C5 RID: 18373
		public bool requiresToggle;

		// Token: 0x040047C6 RID: 18374
		public bool alwaysCheckFeatureSetting;

		// Token: 0x040047C7 RID: 18375
		public string enabledText;

		// Token: 0x040047C8 RID: 18376
		public string disabledText;
	}
}
