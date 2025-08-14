using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTagScripts.ModIO;
using GorillaTagScripts.UI;
using ModIO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x02000840 RID: 2112
public class CustomMapsDetailsScreen : CustomMapsTerminalScreen
{
	// Token: 0x170004FB RID: 1275
	// (get) Token: 0x060034CA RID: 13514 RVA: 0x00113082 File Offset: 0x00111282
	// (set) Token: 0x060034C9 RID: 13513 RVA: 0x00113079 File Offset: 0x00111279
	public ModProfile currentModProfile { get; private set; }

	// Token: 0x060034CB RID: 13515 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void Initialize()
	{
	}

	// Token: 0x060034CC RID: 13516 RVA: 0x0011308C File Offset: 0x0011128C
	public override void Show()
	{
		base.Show();
		GameEvents.ModIOModManagementEvent.RemoveListener(new UnityAction<ModManagementEventType, ModId, Result>(this.HandleModManagementEvent));
		CustomMapManager.OnMapLoadStatusChanged.RemoveListener(new UnityAction<MapLoadStatus, int, string>(this.OnMapLoadProgress));
		CustomMapManager.OnMapLoadComplete.RemoveListener(new UnityAction<bool>(this.OnMapLoadComplete));
		CustomMapManager.OnRoomMapChanged.RemoveListener(new UnityAction<ModId>(this.OnRoomMapChanged));
		CustomMapManager.OnMapUnloadComplete.RemoveListener(new UnityAction(this.OnMapUnloaded));
		GameEvents.ModIOModManagementEvent.AddListener(new UnityAction<ModManagementEventType, ModId, Result>(this.HandleModManagementEvent));
		CustomMapManager.OnMapLoadStatusChanged.AddListener(new UnityAction<MapLoadStatus, int, string>(this.OnMapLoadProgress));
		CustomMapManager.OnMapLoadComplete.AddListener(new UnityAction<bool>(this.OnMapLoadComplete));
		CustomMapManager.OnRoomMapChanged.AddListener(new UnityAction<ModId>(this.OnRoomMapChanged));
		CustomMapManager.OnMapUnloadComplete.AddListener(new UnityAction(this.OnMapUnloaded));
		for (int i = 0; i < this.buttonsToHide.Length; i++)
		{
			this.buttonsToHide[i].SetActive(false);
		}
		for (int j = 0; j < this.buttonsToShow.Length; j++)
		{
			this.buttonsToShow[j].SetActive(true);
		}
		this.ResetToDefaultView();
	}

	// Token: 0x060034CD RID: 13517 RVA: 0x001131C4 File Offset: 0x001113C4
	public override void Hide()
	{
		base.Hide();
		GameEvents.ModIOModManagementEvent.RemoveListener(new UnityAction<ModManagementEventType, ModId, Result>(this.HandleModManagementEvent));
		CustomMapManager.OnMapLoadStatusChanged.RemoveListener(new UnityAction<MapLoadStatus, int, string>(this.OnMapLoadProgress));
		CustomMapManager.OnMapLoadComplete.RemoveListener(new UnityAction<bool>(this.OnMapLoadComplete));
		CustomMapManager.OnRoomMapChanged.RemoveListener(new UnityAction<ModId>(this.OnRoomMapChanged));
		CustomMapManager.OnMapUnloadComplete.RemoveListener(new UnityAction(this.OnMapUnloaded));
		this.lastCompletedOperation = ModManagementOperationType.None_ErrorOcurred;
	}

	// Token: 0x060034CE RID: 13518 RVA: 0x0011324C File Offset: 0x0011144C
	private void HandleModManagementEvent(ModManagementEventType eventType, ModId modId, Result result)
	{
		if (base.isActiveAndEnabled && this.hasModProfile && this.currentModProfile.id.id == modId.id)
		{
			this.UpdateSubscriptionStatus(eventType == ModManagementEventType.InstallFailed || eventType == ModManagementEventType.UninstallFailed || eventType == ModManagementEventType.DownloadFailed || eventType == ModManagementEventType.UpdateFailed);
			if (result.errorCode == 20460U)
			{
				this.modDescriptionText.gameObject.SetActive(false);
				this.loadingMapLabelText.text = this.mapLoadingErrorString;
				this.loadingMapLabelText.gameObject.SetActive(true);
				this.loadingMapMessageText.text = this.mapLoadingErrorInvalidModFile;
				this.loadingMapMessageText.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x060034CF RID: 13519 RVA: 0x00113304 File Offset: 0x00111504
	private void Update()
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (this.currentProgressHandle != null && this.currentProgressHandle.modId != this.currentModProfile.id)
		{
			this.currentProgressHandle = null;
		}
		if (this.currentProgressHandle == null)
		{
			if (!ModIOUnity.IsModManagementBusy())
			{
				return;
			}
			ProgressHandle currentModManagementOperation = ModIOUnity.GetCurrentModManagementOperation();
			if (currentModManagementOperation == null || currentModManagementOperation.Completed || !(currentModManagementOperation.modId == this.currentModProfile.id))
			{
				return;
			}
			this.currentProgressHandle = currentModManagementOperation;
		}
		string str;
		if (this.modOperationTypeStrings.TryGetValue(this.currentProgressHandle.OperationType, out str))
		{
			float f = this.currentProgressHandle.Progress * 100f;
			this.modStatusText.text = str + string.Format(" {0}%", Mathf.RoundToInt(f));
		}
		if (this.currentProgressHandle.Completed)
		{
			this.lastCompletedOperation = this.currentProgressHandle.OperationType;
			this.currentProgressHandle = null;
			this.UpdateSubscriptionStatus(false);
		}
	}

	// Token: 0x060034D0 RID: 13520 RVA: 0x00113408 File Offset: 0x00111608
	public void RetrieveProfileFromModIO(long id, Action<ModIORequestResultAnd<ModProfile>> callback = null)
	{
		if (this.hasModProfile && this.currentModProfile.id == id)
		{
			this.UpdateModDetails(true);
			return;
		}
		this.pendingModId = id;
		ModIOManager.GetModProfile(new ModId(id), (callback != null) ? callback : new Action<ModIORequestResultAnd<ModProfile>>(this.OnProfileReceived));
	}

	// Token: 0x060034D1 RID: 13521 RVA: 0x0011345C File Offset: 0x0011165C
	public void SetModProfile(ModProfile modProfile)
	{
		if (modProfile.id != ModId.Null)
		{
			this.pendingModId = 0L;
			this.currentModProfile = modProfile;
			this.hasModProfile = true;
			this.UpdateModDetails(true);
		}
	}

	// Token: 0x060034D2 RID: 13522 RVA: 0x00113490 File Offset: 0x00111690
	protected override void PressButton(CustomMapKeyboardBinding buttonPressed)
	{
		if (!base.isActiveAndEnabled || !CustomMapsTerminal.IsDriver)
		{
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.goback)
		{
			if (CustomMapManager.IsLoading(0L))
			{
				return;
			}
			if (CustomMapManager.IsUnloading())
			{
				return;
			}
			if (this.mapLoadError)
			{
				this.mapLoadError = false;
				CustomMapManager.ClearRoomMap();
				this.ResetToDefaultView();
				return;
			}
			if (!CustomMapLoader.IsModLoaded(0L) && !(CustomMapManager.GetRoomMapId() != ModId.Null))
			{
				CustomMapsTerminal.ReturnFromDetailsScreen();
				this.hasModProfile = false;
				this.currentModProfile = default(ModProfile);
				return;
			}
			string text;
			if (!this.CanChangeMapState(false, out text))
			{
				this.modDescriptionText.gameObject.SetActive(false);
				this.errorText.text = text;
				this.errorText.gameObject.SetActive(true);
				return;
			}
			this.UnloadMod();
			return;
		}
		else
		{
			if (!this.hasModProfile)
			{
				return;
			}
			if (buttonPressed != CustomMapKeyboardBinding.option3)
			{
				if (buttonPressed == CustomMapKeyboardBinding.map)
				{
					if (CustomMapLoader.IsModLoaded(0L) || CustomMapManager.IsLoading(0L) || CustomMapManager.IsUnloading())
					{
						return;
					}
					this.errorText.gameObject.SetActive(false);
					this.errorText.text = "";
					this.loadingMapLabelText.gameObject.SetActive(false);
					this.loadingMapMessageText.gameObject.SetActive(false);
					this.modDescriptionText.gameObject.SetActive(true);
					ModIOManager.Refresh(delegate(bool result)
					{
						SubscribedModStatus subscribedModStatus2;
						if (ModIOManager.GetSubscribedModStatus(this.currentModProfile.id, out subscribedModStatus2))
						{
							ModIOManager.UnsubscribeFromMod(this.currentModProfile.id, delegate(Result result)
							{
								if (result.Succeeded())
								{
									this.UpdateModDetails(false);
								}
							});
							return;
						}
						ModIOManager.SubscribeToMod(this.currentModProfile.id, delegate(Result result)
						{
							if (result.Succeeded())
							{
								this.UpdateModDetails(false);
								ModIOManager.DownloadMod(this.currentModProfile.id, delegate(ModIORequestResult result)
								{
									GorillaTelemetry.PostCustomMapDownloadEvent(this.currentModProfile.name, this.currentModProfile.id, this.currentModProfile.creator.username);
									this.UpdateModDetails(false);
								});
							}
						});
					}, false);
				}
				SubscribedModStatus subscribedModStatus;
				if (buttonPressed == CustomMapKeyboardBinding.enter && !CustomMapManager.IsLoading(0L) && !CustomMapManager.IsUnloading() && !CustomMapLoader.IsModLoaded(0L) && this.lastCompletedOperation != ModManagementOperationType.Update && ModIOManager.GetSubscribedModStatus(this.currentModProfile.id, out subscribedModStatus))
				{
					if (subscribedModStatus == SubscribedModStatus.Installed)
					{
						string text2;
						if (!this.CanChangeMapState(true, out text2))
						{
							this.modDescriptionText.gameObject.SetActive(false);
							this.errorText.text = text2;
							this.errorText.gameObject.SetActive(true);
							return;
						}
						this.LoadMap();
						return;
					}
					else if (subscribedModStatus == SubscribedModStatus.WaitingToDownload || (subscribedModStatus == SubscribedModStatus.WaitingToUpdate && this.lastCompletedOperation != ModManagementOperationType.Update))
					{
						ModIOManager.DownloadMod(this.currentModProfile.id, delegate(ModIORequestResult result)
						{
							this.UpdateSubscriptionStatus(!result.success);
							GorillaTelemetry.PostCustomMapDownloadEvent(this.currentModProfile.name, this.currentModProfile.id, this.currentModProfile.creator.username);
						});
					}
				}
				return;
			}
			if (CustomMapLoader.IsModLoaded(0L) || CustomMapManager.IsLoading(0L) || CustomMapManager.IsUnloading() || CustomMapManager.GetRoomMapId() != ModId.Null)
			{
				return;
			}
			if (this.hasModProfile)
			{
				long currentModId = this.currentModProfile.id.id;
				this.hasModProfile = false;
				this.currentModProfile = default(ModProfile);
				ModIOManager.Refresh(delegate(bool result)
				{
					this.RetrieveProfileFromModIO(currentModId, null);
				}, false);
			}
			return;
		}
	}

	// Token: 0x060034D3 RID: 13523 RVA: 0x00113728 File Offset: 0x00111928
	private void OnProfileReceived(ModIORequestResultAnd<ModProfile> profile)
	{
		if (profile.result.success)
		{
			this.SetModProfile(profile.data);
			return;
		}
		this.modDescriptionText.gameObject.SetActive(false);
		this.errorText.text = "Failed to retrieve mod details";
		this.errorText.gameObject.SetActive(true);
	}

	// Token: 0x060034D4 RID: 13524 RVA: 0x00113784 File Offset: 0x00111984
	private void ResetToDefaultView()
	{
		this.loadingMapLabelText.gameObject.SetActive(false);
		this.loadingMapMessageText.gameObject.SetActive(false);
		this.mapReadyText.gameObject.SetActive(false);
		this.errorText.gameObject.SetActive(false);
		this.modNameText.gameObject.SetActive(false);
		this.modCreatorLabelText.gameObject.SetActive(false);
		this.modCreatorText.gameObject.SetActive(false);
		this.modDescriptionText.gameObject.SetActive(false);
		this.modStatusText.gameObject.SetActive(false);
		this.modSubscriptionStatusText.gameObject.SetActive(false);
		this.OptionButtonTooltipText.gameObject.SetActive(false);
		this.mapScreenshotImage.gameObject.SetActive(false);
		this.loadRoomMapPromptText.gameObject.SetActive(false);
		this.outdatedText.gameObject.SetActive(false);
		this.loadingText.gameObject.SetActive(true);
		if (CustomMapLoader.IsModLoaded(0L) || CustomMapManager.IsLoading(0L) || CustomMapManager.IsUnloading())
		{
			ModId modId = new ModId(CustomMapLoader.IsModLoaded(0L) ? CustomMapLoader.LoadedMapModId : (CustomMapManager.IsLoading(0L) ? CustomMapManager.LoadingMapId : CustomMapManager.UnloadingMapId));
			if (this.hasModProfile && this.currentModProfile.id == modId)
			{
				this.UpdateModDetails(true);
				return;
			}
			this.RetrieveProfileFromModIO(modId, delegate(ModIORequestResultAnd<ModProfile> result)
			{
				this.OnProfileReceived(result);
			});
			return;
		}
		else
		{
			if (CustomMapManager.GetRoomMapId() != ModId.Null)
			{
				this.OnRoomMapChanged(CustomMapManager.GetRoomMapId());
				return;
			}
			if (this.hasModProfile)
			{
				this.UpdateModDetails(true);
			}
			return;
		}
	}

	// Token: 0x060034D5 RID: 13525 RVA: 0x00113940 File Offset: 0x00111B40
	private void UpdateModDetails(bool refreshScreenState = true)
	{
		if (!this.hasModProfile)
		{
			return;
		}
		this.modNameText.text = this.currentModProfile.name;
		this.modCreatorText.text = this.currentModProfile.creator.username;
		this.modDescriptionText.text = this.currentModProfile.description;
		this.UpdateSubscriptionStatus(false);
		ModIOUnity.DownloadTexture(this.currentModProfile.logoImage320x180, new Action<ResultAnd<Texture2D>>(this.OnTextureDownloaded));
		if (refreshScreenState)
		{
			this.loadingText.gameObject.SetActive(false);
			this.loadingMapLabelText.gameObject.SetActive(false);
			this.loadingMapMessageText.gameObject.SetActive(false);
			this.loadRoomMapPromptText.gameObject.SetActive(false);
			this.mapReadyText.gameObject.SetActive(false);
			this.unloadPromptText.gameObject.SetActive(false);
			this.errorText.gameObject.SetActive(false);
			this.modNameText.gameObject.SetActive(true);
			this.modCreatorLabelText.gameObject.SetActive(true);
			this.modCreatorText.gameObject.SetActive(true);
			this.modDescriptionText.gameObject.SetActive(true);
			if (CustomMapLoader.IsModLoaded(0L))
			{
				ModId modId = new ModId(CustomMapLoader.LoadedMapModId);
				if (this.currentModProfile.id == modId)
				{
					this.OnMapLoadComplete_UIUpdate();
					return;
				}
				this.RetrieveProfileFromModIO(modId, delegate(ModIORequestResultAnd<ModProfile> result)
				{
					this.OnProfileReceived(result);
				});
				return;
			}
			else
			{
				if (CustomMapManager.IsLoading(0L))
				{
					this.modDescriptionText.gameObject.SetActive(false);
					this.loadingMapLabelText.text = this.mapLoadingString + " 0%";
					this.loadingMapLabelText.gameObject.SetActive(true);
					return;
				}
				if (CustomMapManager.IsUnloading())
				{
					this.modDescriptionText.gameObject.SetActive(false);
					this.loadingMapLabelText.text = this.mapUnloadingString;
					this.loadingMapLabelText.gameObject.SetActive(true);
					return;
				}
				if (CustomMapManager.GetRoomMapId() != ModId.Null)
				{
					this.modDescriptionText.gameObject.SetActive(false);
					this.loadRoomMapPromptText.gameObject.SetActive(true);
					return;
				}
				if (this.mapLoadError)
				{
					this.modDescriptionText.gameObject.SetActive(false);
					this.loadingMapLabelText.gameObject.SetActive(true);
					this.loadingMapMessageText.gameObject.SetActive(true);
				}
			}
		}
	}

	// Token: 0x060034D6 RID: 13526 RVA: 0x00113BBC File Offset: 0x00111DBC
	private void OnTextureDownloaded(ResultAnd<Texture2D> resultAnd)
	{
		if (!resultAnd.result.Succeeded())
		{
			return;
		}
		Texture2D value = resultAnd.value;
		this.mapScreenshotImage.sprite = Sprite.Create(value, new Rect(0f, 0f, (float)value.width, (float)value.height), new Vector2(0.5f, 0.5f));
		this.mapScreenshotImage.gameObject.SetActive(true);
	}

	// Token: 0x060034D7 RID: 13527 RVA: 0x00113C2C File Offset: 0x00111E2C
	private void UpdateSubscriptionStatus(bool errorEncountered = false)
	{
		if (base.isActiveAndEnabled)
		{
			bool flag = this.mapLoadError || CustomMapManager.IsUnloading() || CustomMapManager.IsLoading(0L) || CustomMapLoader.IsModLoaded(0L) || CustomMapManager.GetRoomMapId() != ModId.Null;
			this.outdatedText.gameObject.SetActive(false);
			if (!flag)
			{
				SubscribedModStatus subscribedModStatus2;
				bool subscribedModStatus = ModIOManager.GetSubscribedModStatus(this.currentModProfile.id, out subscribedModStatus2);
				if (errorEncountered)
				{
					this.lastCompletedOperation = ModManagementOperationType.None_ErrorOcurred;
					subscribedModStatus2 = SubscribedModStatus.ProblemOccurred;
				}
				this.modSubscriptionStatusText.text = (subscribedModStatus ? this.subscribedStatusString : this.unsubscribedStatusString);
				if (this.subscriptionToggleButton.IsNotNull())
				{
					this.subscriptionToggleButton.SetButtonStatus(subscribedModStatus);
				}
				if (subscribedModStatus)
				{
					switch (subscribedModStatus2)
					{
					case SubscribedModStatus.Installed:
					{
						this.selectButtonTooltipText.text = this.loadMapTooltipString;
						this.selectButtonTooltipText.gameObject.SetActive(true);
						this.lastCompletedOperation = ModManagementOperationType.None_ErrorOcurred;
						int num;
						if (ModIOManager.IsModOutdated(this.currentModProfile.id, out num))
						{
							this.outdatedText.gameObject.SetActive(true);
							goto IL_186;
						}
						this.outdatedText.gameObject.SetActive(false);
						goto IL_186;
					}
					case SubscribedModStatus.WaitingToDownload:
						this.selectButtonTooltipText.text = this.downloadMapTooltipString;
						this.selectButtonTooltipText.gameObject.SetActive(true);
						goto IL_186;
					case SubscribedModStatus.WaitingToUpdate:
						this.selectButtonTooltipText.text = this.updateMapTooltipString;
						this.selectButtonTooltipText.gameObject.SetActive(true);
						goto IL_186;
					}
					this.selectButtonTooltipText.gameObject.SetActive(false);
					IL_186:
					string text;
					if (subscribedModStatus2 == SubscribedModStatus.WaitingToUpdate && this.lastCompletedOperation == ModManagementOperationType.Update)
					{
						this.modStatusText.text = "INSTALLING";
					}
					else if (this.modStatusStrings.TryGetValue(subscribedModStatus2, out text))
					{
						this.modStatusText.text = text;
					}
					else
					{
						this.modStatusText.text = "STATUS STRING MISSING!";
					}
					if (ModIOManager.IsModInDownloadQueue(this.currentModProfile.id))
					{
						this.selectButtonTooltipText.gameObject.SetActive(false);
						this.modStatusText.text = this.mapDownloadQueuedString;
					}
					this.OptionButtonTooltipText.text = this.unsubscribeTooltipString;
				}
				else
				{
					this.selectButtonTooltipText.gameObject.SetActive(false);
					this.modStatusText.text = this.modAvailableString;
					this.OptionButtonTooltipText.text = this.subscribeTooltipString;
				}
				this.modStatusText.gameObject.SetActive(true);
				this.OptionButtonTooltipText.gameObject.SetActive(true);
				this.modSubscriptionStatusText.gameObject.SetActive(true);
				return;
			}
			this.lastCompletedOperation = ModManagementOperationType.None_ErrorOcurred;
			this.selectButtonTooltipText.gameObject.SetActive(false);
			this.modStatusText.gameObject.SetActive(false);
			this.OptionButtonTooltipText.gameObject.SetActive(false);
			this.modSubscriptionStatusText.gameObject.SetActive(false);
		}
	}

	// Token: 0x060034D8 RID: 13528 RVA: 0x00113F08 File Offset: 0x00112108
	private bool CanChangeMapState(bool load, out string disallowedReason)
	{
		disallowedReason = "";
		if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate)
		{
			if (!CustomMapManager.AreAllPlayersInVirtualStump())
			{
				disallowedReason = "ALL PLAYERS IN THE ROOM MUST BE INSIDE THE VIRTUAL STUMP BEFORE " + (load ? "" : "UN") + "LOADING A MAP.";
				return false;
			}
			return true;
		}
		else
		{
			if (!CustomMapManager.IsLocalPlayerInVirtualStump())
			{
				disallowedReason = "YOU MUST BE INSIDE THE VIRTUAL STUMP TO " + (load ? "" : "UN") + "LOAD A MAP.";
				return false;
			}
			return true;
		}
	}

	// Token: 0x060034D9 RID: 13529 RVA: 0x00113F8C File Offset: 0x0011218C
	private void LoadMap()
	{
		this.modDescriptionText.gameObject.SetActive(false);
		this.OptionButtonTooltipText.gameObject.SetActive(false);
		this.selectButtonTooltipText.gameObject.SetActive(false);
		this.modStatusText.gameObject.SetActive(false);
		this.modSubscriptionStatusText.gameObject.SetActive(false);
		this.outdatedText.gameObject.SetActive(false);
		this.loadingMapLabelText.gameObject.SetActive(true);
		if (NetworkSystem.Instance.InRoom && !NetworkSystem.Instance.SessionIsPrivate)
		{
			NetworkSystem.Instance.ReturnToSinglePlayer();
		}
		this.networkObject.LoadModSynced(this.currentModProfile.id);
	}

	// Token: 0x060034DA RID: 13530 RVA: 0x0011404E File Offset: 0x0011224E
	private void UnloadMod()
	{
		this.networkObject.UnloadModSynced();
	}

	// Token: 0x060034DB RID: 13531 RVA: 0x0011405B File Offset: 0x0011225B
	public void OnMapLoadComplete(bool success)
	{
		if (success)
		{
			this.OnMapLoadComplete_UIUpdate();
			return;
		}
		this.mapLoadError = true;
	}

	// Token: 0x060034DC RID: 13532 RVA: 0x00114070 File Offset: 0x00112270
	private void OnMapLoadComplete_UIUpdate()
	{
		this.modDescriptionText.gameObject.SetActive(false);
		this.loadingMapLabelText.gameObject.SetActive(false);
		this.loadingMapMessageText.gameObject.SetActive(false);
		this.loadRoomMapPromptText.gameObject.SetActive(false);
		this.errorText.gameObject.SetActive(false);
		this.mapReadyText.gameObject.SetActive(true);
		this.unloadPromptText.gameObject.SetActive(true);
	}

	// Token: 0x060034DD RID: 13533 RVA: 0x001140F4 File Offset: 0x001122F4
	private void OnMapUnloaded()
	{
		this.mapLoadError = false;
		this.UpdateModDetails(true);
	}

	// Token: 0x060034DE RID: 13534 RVA: 0x00114104 File Offset: 0x00112304
	private void OnRoomMapChanged(ModId roomMapID)
	{
		if (roomMapID == ModId.Null)
		{
			this.UpdateModDetails(true);
			return;
		}
		if (this.currentModProfile.id != roomMapID)
		{
			this.RetrieveProfileFromModIO(roomMapID.id, new Action<ModIORequestResultAnd<ModProfile>>(this.OnRoomMapProfileReceived));
			return;
		}
		this.ShowLoadRoomMapPrompt();
	}

	// Token: 0x060034DF RID: 13535 RVA: 0x00114158 File Offset: 0x00112358
	private void OnRoomMapProfileReceived(ModIORequestResultAnd<ModProfile> result)
	{
		this.OnProfileReceived(result);
		if (result.result.success)
		{
			this.ShowLoadRoomMapPrompt();
		}
	}

	// Token: 0x060034E0 RID: 13536 RVA: 0x00114174 File Offset: 0x00112374
	private void ShowLoadRoomMapPrompt()
	{
		if (CustomMapManager.IsUnloading() || CustomMapManager.IsLoading(0L) || CustomMapLoader.IsModLoaded(this.currentModProfile.id))
		{
			return;
		}
		this.modDescriptionText.gameObject.SetActive(false);
		this.loadingText.gameObject.SetActive(false);
		this.loadingMapLabelText.gameObject.SetActive(false);
		this.mapReadyText.gameObject.SetActive(false);
		this.unloadPromptText.gameObject.SetActive(false);
		this.loadRoomMapPromptText.gameObject.SetActive(true);
	}

	// Token: 0x060034E1 RID: 13537 RVA: 0x00114210 File Offset: 0x00112410
	public void OnMapLoadProgress(MapLoadStatus loadStatus, int progress, string message)
	{
		if (loadStatus != MapLoadStatus.None)
		{
			this.mapLoadError = false;
			this.loadRoomMapPromptText.gameObject.SetActive(false);
			this.modDescriptionText.gameObject.SetActive(false);
		}
		switch (loadStatus)
		{
		case MapLoadStatus.Downloading:
			this.loadingMapLabelText.text = this.mapAutoDownloadingString;
			this.loadingMapLabelText.gameObject.SetActive(true);
			this.loadingMapMessageText.gameObject.SetActive(false);
			this.loadingMapMessageText.text = "";
			return;
		case MapLoadStatus.Loading:
			this.loadingMapLabelText.text = this.mapLoadingString + " " + progress.ToString() + "%";
			this.loadingMapLabelText.gameObject.SetActive(true);
			this.loadingMapMessageText.text = message;
			this.loadingMapMessageText.gameObject.SetActive(true);
			return;
		case MapLoadStatus.Unloading:
			this.mapReadyText.gameObject.SetActive(false);
			this.unloadPromptText.gameObject.SetActive(false);
			this.loadingMapLabelText.text = this.mapUnloadingString;
			this.loadingMapLabelText.gameObject.SetActive(true);
			this.loadingMapMessageText.gameObject.SetActive(false);
			this.loadingMapMessageText.text = "";
			return;
		case MapLoadStatus.Error:
			this.loadingMapLabelText.text = this.mapLoadingErrorString;
			this.loadingMapLabelText.gameObject.SetActive(true);
			if (CustomMapsTerminal.IsDriver)
			{
				this.loadingMapMessageText.text = message + "\n" + this.mapLoadingErrorDriverString;
			}
			else
			{
				this.loadingMapMessageText.text = message + "\n" + this.mapLoadingErrorNonDriverString;
			}
			this.loadingMapMessageText.gameObject.SetActive(true);
			return;
		default:
			return;
		}
	}

	// Token: 0x060034E2 RID: 13538 RVA: 0x001143D6 File Offset: 0x001125D6
	public long GetModId()
	{
		return this.currentModProfile.id.id;
	}

	// Token: 0x040041A6 RID: 16806
	[SerializeField]
	private SpriteRenderer mapScreenshotImage;

	// Token: 0x040041A7 RID: 16807
	[SerializeField]
	private TMP_Text loadingText;

	// Token: 0x040041A8 RID: 16808
	[SerializeField]
	private TMP_Text modNameText;

	// Token: 0x040041A9 RID: 16809
	[SerializeField]
	private TMP_Text modCreatorLabelText;

	// Token: 0x040041AA RID: 16810
	[SerializeField]
	private TMP_Text modCreatorText;

	// Token: 0x040041AB RID: 16811
	[SerializeField]
	private TMP_Text modDescriptionText;

	// Token: 0x040041AC RID: 16812
	[SerializeField]
	private TMP_Text selectButtonTooltipText;

	// Token: 0x040041AD RID: 16813
	[SerializeField]
	private TMP_Text OptionButtonTooltipText;

	// Token: 0x040041AE RID: 16814
	[SerializeField]
	private TMP_Text modStatusText;

	// Token: 0x040041AF RID: 16815
	[SerializeField]
	private TMP_Text modSubscriptionStatusText;

	// Token: 0x040041B0 RID: 16816
	[SerializeField]
	private TMP_Text loadingMapLabelText;

	// Token: 0x040041B1 RID: 16817
	[SerializeField]
	private TMP_Text loadingMapMessageText;

	// Token: 0x040041B2 RID: 16818
	[SerializeField]
	private TMP_Text loadRoomMapPromptText;

	// Token: 0x040041B3 RID: 16819
	[SerializeField]
	private TMP_Text mapReadyText;

	// Token: 0x040041B4 RID: 16820
	[SerializeField]
	private TMP_Text unloadPromptText;

	// Token: 0x040041B5 RID: 16821
	[SerializeField]
	private TMP_Text errorText;

	// Token: 0x040041B6 RID: 16822
	[SerializeField]
	private TMP_Text outdatedText;

	// Token: 0x040041B7 RID: 16823
	[SerializeField]
	private GameObject[] buttonsToHide;

	// Token: 0x040041B8 RID: 16824
	[SerializeField]
	private GameObject[] buttonsToShow;

	// Token: 0x040041B9 RID: 16825
	[SerializeField]
	private CustomMapsKeyToggleButton subscriptionToggleButton;

	// Token: 0x040041BA RID: 16826
	[SerializeField]
	private string modAvailableString = "AVAILABLE";

	// Token: 0x040041BB RID: 16827
	[SerializeField]
	private string mapAutoDownloadingString = "DOWNLOADING...";

	// Token: 0x040041BC RID: 16828
	[SerializeField]
	private string mapDownloadQueuedString = "DOWNLOAD QUEUED";

	// Token: 0x040041BD RID: 16829
	[SerializeField]
	private string mapLoadingString = "LOADING:";

	// Token: 0x040041BE RID: 16830
	[SerializeField]
	private string mapUnloadingString = "UNLOADING...";

	// Token: 0x040041BF RID: 16831
	[SerializeField]
	private string mapLoadingErrorString = "ERROR:";

	// Token: 0x040041C0 RID: 16832
	[SerializeField]
	private string mapLoadingErrorDriverString = "PRESS THE 'BACK' BUTTON TO TRY AGAIN";

	// Token: 0x040041C1 RID: 16833
	[SerializeField]
	private string mapLoadingErrorNonDriverString = "LEAVE AND REJOIN THE VIRTUAL STUMP TO TRY AGAIN";

	// Token: 0x040041C2 RID: 16834
	[SerializeField]
	private string mapLoadingErrorInvalidModFile = "INSTALL FAILED DUE TO INVALID MAP FILE";

	// Token: 0x040041C3 RID: 16835
	[SerializeField]
	private VirtualStumpSerializer networkObject;

	// Token: 0x040041C4 RID: 16836
	private Dictionary<SubscribedModStatus, string> modStatusStrings = new Dictionary<SubscribedModStatus, string>
	{
		{
			SubscribedModStatus.Installed,
			"READY"
		},
		{
			SubscribedModStatus.WaitingToDownload,
			"NOT DOWNLOADED"
		},
		{
			SubscribedModStatus.WaitingToInstall,
			"INSTALL QUEUED"
		},
		{
			SubscribedModStatus.WaitingToUpdate,
			"NEEDS UPDATE"
		},
		{
			SubscribedModStatus.WaitingToUninstall,
			"UNINSTALL QUEUED"
		},
		{
			SubscribedModStatus.Downloading,
			"DOWNLOADING"
		},
		{
			SubscribedModStatus.Installing,
			"INSTALLING"
		},
		{
			SubscribedModStatus.Uninstalling,
			"UNINSTALLING"
		},
		{
			SubscribedModStatus.Updating,
			"UPDATING"
		},
		{
			SubscribedModStatus.ProblemOccurred,
			"ERROR"
		},
		{
			SubscribedModStatus.None,
			""
		}
	};

	// Token: 0x040041C5 RID: 16837
	private Dictionary<ModManagementOperationType, string> modOperationTypeStrings = new Dictionary<ModManagementOperationType, string>
	{
		{
			ModManagementOperationType.Install,
			"INSTALLING"
		},
		{
			ModManagementOperationType.Download,
			"DOWNLOADING"
		},
		{
			ModManagementOperationType.Update,
			"UPDATING"
		},
		{
			ModManagementOperationType.Uninstall,
			"UNINSTALLING"
		},
		{
			ModManagementOperationType.None_ErrorOcurred,
			"ERROR"
		},
		{
			ModManagementOperationType.None_AlreadyInstalled,
			"READY"
		}
	};

	// Token: 0x040041C6 RID: 16838
	[FormerlySerializedAs("unsubscribedTooltipString")]
	[SerializeField]
	private string subscribeTooltipString = "MAP: SUBSCRIBE TO MAP";

	// Token: 0x040041C7 RID: 16839
	[FormerlySerializedAs("subscribedTooltipString")]
	[SerializeField]
	private string unsubscribeTooltipString = "MAP: UNSUBSCRIBE FROM MAP";

	// Token: 0x040041C8 RID: 16840
	[SerializeField]
	private string subscribedStatusString = "SUBSCRIBED";

	// Token: 0x040041C9 RID: 16841
	[SerializeField]
	private string unsubscribedStatusString = "NOT SUBSCRIBED";

	// Token: 0x040041CA RID: 16842
	[SerializeField]
	private string loadMapTooltipString = "SELECT: LOAD MAP";

	// Token: 0x040041CB RID: 16843
	[SerializeField]
	private string downloadMapTooltipString = "SELECT: DOWNLOAD MAP";

	// Token: 0x040041CC RID: 16844
	[SerializeField]
	private string updateMapTooltipString = "SELECT: UPDATE MAP";

	// Token: 0x040041CD RID: 16845
	public long pendingModId;

	// Token: 0x040041CF RID: 16847
	private bool hasModProfile;

	// Token: 0x040041D0 RID: 16848
	private bool mapLoadError;

	// Token: 0x040041D1 RID: 16849
	private const uint IO_ModFileInvalid_ErrorCode = 20460U;

	// Token: 0x040041D2 RID: 16850
	private ProgressHandle currentProgressHandle;

	// Token: 0x040041D3 RID: 16851
	private ModManagementOperationType lastCompletedOperation = ModManagementOperationType.None_ErrorOcurred;
}
