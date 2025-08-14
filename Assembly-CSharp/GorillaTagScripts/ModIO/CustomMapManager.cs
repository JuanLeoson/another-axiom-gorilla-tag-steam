using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using GorillaNetworking;
using GorillaTag.Rendering;
using GorillaTagScripts.CustomMapSupport;
using GorillaTagScripts.UI.ModIO;
using GT_CustomMapSupportRuntime;
using ModIO;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.ModIO
{
	// Token: 0x02000C5E RID: 3166
	public class CustomMapManager : MonoBehaviour, IBuildValidation
	{
		// Token: 0x1700075B RID: 1883
		// (get) Token: 0x06004E46 RID: 20038 RVA: 0x00185067 File Offset: 0x00183267
		public static bool WaitingForRoomJoin
		{
			get
			{
				return CustomMapManager.waitingForRoomJoin;
			}
		}

		// Token: 0x1700075C RID: 1884
		// (get) Token: 0x06004E47 RID: 20039 RVA: 0x0018506E File Offset: 0x0018326E
		public static bool WaitingForDisconnect
		{
			get
			{
				return CustomMapManager.waitingForDisconnect;
			}
		}

		// Token: 0x1700075D RID: 1885
		// (get) Token: 0x06004E48 RID: 20040 RVA: 0x00185075 File Offset: 0x00183275
		public static long LoadingMapId
		{
			get
			{
				return CustomMapManager.loadingMapId;
			}
		}

		// Token: 0x1700075E RID: 1886
		// (get) Token: 0x06004E49 RID: 20041 RVA: 0x00185081 File Offset: 0x00183281
		public static long UnloadingMapId
		{
			get
			{
				return CustomMapManager.unloadingMapId;
			}
		}

		// Token: 0x06004E4A RID: 20042 RVA: 0x00185090 File Offset: 0x00183290
		public bool BuildValidationCheck()
		{
			for (int i = 0; i < this.virtualStumpEjectLocations.Length; i++)
			{
				if (this.virtualStumpEjectLocations[i].ejectLocations.IsNullOrEmpty<GameObject>())
				{
					Debug.LogError("List of VirtualStumpEjectLocations is empty for zone " + this.virtualStumpEjectLocations[i].ejectZone.ToString(), base.gameObject);
					return false;
				}
			}
			return true;
		}

		// Token: 0x06004E4B RID: 20043 RVA: 0x001850F4 File Offset: 0x001832F4
		private void Awake()
		{
			if (CustomMapManager.instance == null)
			{
				CustomMapManager.instance = this;
				CustomMapManager.hasInstance = true;
				return;
			}
			if (CustomMapManager.instance != this)
			{
				Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06004E4C RID: 20044 RVA: 0x00185130 File Offset: 0x00183330
		public void OnEnable()
		{
			CustomMapManager.gamemodeButtonLayout = Object.FindObjectOfType<GameModeSelectorButtonLayout>();
			UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(this.OnUGCEnabled));
			UGCPermissionManager.SubscribeToUGCEnabled(new Action(this.OnUGCEnabled));
			UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(this.OnUGCDisabled));
			UGCPermissionManager.SubscribeToUGCDisabled(new Action(this.OnUGCDisabled));
			CMSSerializer.OnTriggerHistoryProcessedForScene.RemoveListener(new UnityAction<string>(CustomMapManager.OnSceneTriggerHistoryProcessed));
			CMSSerializer.OnTriggerHistoryProcessedForScene.AddListener(new UnityAction<string>(CustomMapManager.OnSceneTriggerHistoryProcessed));
			GameEvents.ModIOModManagementEvent.RemoveListener(new UnityAction<ModManagementEventType, ModId, Result>(this.HandleModManagementEvent));
			GameEvents.ModIOModManagementEvent.AddListener(new UnityAction<ModManagementEventType, ModId, Result>(this.HandleModManagementEvent));
			ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
			ModIOManager.OnModIOLoggedIn.AddListener(new UnityAction(this.OnModIOLoggedIn));
			ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
			ModIOManager.OnModIOLoggedOut.AddListener(new UnityAction(this.OnModIOLoggedOut));
			RoomSystem.JoinedRoomEvent -= new Action(this.OnJoinedRoom);
			RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnDisconnected;
			NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnDisconnected;
		}

		// Token: 0x06004E4D RID: 20045 RVA: 0x001852B4 File Offset: 0x001834B4
		public void OnDisable()
		{
			UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(this.OnUGCEnabled));
			UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(this.OnUGCDisabled));
			CMSSerializer.OnTriggerHistoryProcessedForScene.RemoveListener(new UnityAction<string>(CustomMapManager.OnSceneTriggerHistoryProcessed));
			GameEvents.ModIOModManagementEvent.RemoveListener(new UnityAction<ModManagementEventType, ModId, Result>(this.HandleModManagementEvent));
			ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
			ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
			NetworkSystem.Instance.OnMultiplayerStarted -= this.OnJoinedRoom;
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnDisconnected;
		}

		// Token: 0x06004E4E RID: 20046 RVA: 0x000023F5 File Offset: 0x000005F5
		private void OnUGCEnabled()
		{
		}

		// Token: 0x06004E4F RID: 20047 RVA: 0x000023F5 File Offset: 0x000005F5
		private void OnUGCDisabled()
		{
		}

		// Token: 0x06004E50 RID: 20048 RVA: 0x00185380 File Offset: 0x00183580
		private void Start()
		{
			for (int i = this.virtualStumpTeleportLocations.Count - 1; i >= 0; i--)
			{
				if (this.virtualStumpTeleportLocations[i] == null)
				{
					this.virtualStumpTeleportLocations.RemoveAt(i);
				}
			}
			for (int j = 0; j < this.virtualStumpEjectLocations.Length; j++)
			{
				if (this.virtualStumpEjectLocations[j].ejectLocations.IsNullOrEmpty<GameObject>())
				{
					GTDev.LogError<string>("List of VirtualStumpEjectLocations is empty for zone " + this.virtualStumpEjectLocations[j].ejectZone.ToString() + "!", null);
				}
				List<GameObject> list = new List<GameObject>(this.virtualStumpEjectLocations[j].ejectLocations);
				for (int k = list.Count - 1; k >= 0; k--)
				{
					if (list[k].IsNull())
					{
						list.RemoveAt(k);
					}
				}
				this.virtualStumpEjectLocations[j].ejectLocations = list.ToArray();
			}
			this.virtualStumpToggleableRoot.SetActive(false);
			base.gameObject.SetActive(false);
		}

		// Token: 0x06004E51 RID: 20049 RVA: 0x00185488 File Offset: 0x00183688
		private void OnDestroy()
		{
			if (CustomMapManager.instance == this)
			{
				CustomMapManager.instance = null;
				CustomMapManager.hasInstance = false;
			}
			UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(this.OnUGCEnabled));
			UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(this.OnUGCDisabled));
			CMSSerializer.OnTriggerHistoryProcessedForScene.RemoveListener(new UnityAction<string>(CustomMapManager.OnSceneTriggerHistoryProcessed));
			GameEvents.ModIOModManagementEvent.RemoveListener(new UnityAction<ModManagementEventType, ModId, Result>(this.HandleModManagementEvent));
			ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
			ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
			NetworkSystem.Instance.OnMultiplayerStarted -= this.OnJoinedRoom;
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnDisconnected;
		}

		// Token: 0x06004E52 RID: 20050 RVA: 0x00185570 File Offset: 0x00183770
		private void HandleModManagementEvent(ModManagementEventType eventType, ModId modId, Result result)
		{
			if (CustomMapManager.waitingForModInstall && CustomMapManager.waitingForModInstallId == modId)
			{
				if (CustomMapManager.abortModLoadIds.Contains(modId))
				{
					CustomMapManager.abortModLoadIds.Remove(modId);
					if (CustomMapManager.waitingForModInstallId.Equals(modId))
					{
						CustomMapManager.waitingForModInstall = false;
						CustomMapManager.waitingForModDownload = false;
						CustomMapManager.waitingForModInstallId = ModId.Null;
					}
					return;
				}
				switch (eventType)
				{
				case ModManagementEventType.Installed:
				case ModManagementEventType.Updated:
					CustomMapManager.waitingForModDownload = false;
					this.LoadInstalledMod(modId);
					break;
				case ModManagementEventType.InstallFailed:
					CustomMapManager.instance.HandleMapLoadFailed("FAILED TO INSTALL MAP: " + result.message);
					return;
				case ModManagementEventType.DownloadStarted:
				case ModManagementEventType.UpdateStarted:
					CustomMapManager.waitingForModDownload = true;
					return;
				case ModManagementEventType.Downloaded:
					CustomMapManager.waitingForModDownload = false;
					return;
				case ModManagementEventType.DownloadFailed:
					CustomMapManager.waitingForModDownload = false;
					return;
				case ModManagementEventType.UninstallStarted:
				case ModManagementEventType.Uninstalled:
				case ModManagementEventType.UninstallFailed:
					break;
				case ModManagementEventType.UpdateFailed:
					CustomMapManager.instance.HandleMapLoadFailed("FAILED TO DOWNLOAD MAP: " + result.message);
					return;
				default:
					return;
				}
			}
		}

		// Token: 0x06004E53 RID: 20051 RVA: 0x00185668 File Offset: 0x00183868
		private void OnModIOLoggedOut()
		{
			ModId @null = ModId.Null;
			if (CustomMapLoader.IsModLoaded(0L) && NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate)
			{
				@null = new ModId(CustomMapLoader.LoadedMapModId);
			}
			CustomMapManager.UnloadMod(true);
			CustomMapManager.mapIdToLoadOnLogin = @null;
		}

		// Token: 0x06004E54 RID: 20052 RVA: 0x001856B5 File Offset: 0x001838B5
		private void OnModIOLoggedIn()
		{
			if (CustomMapManager.mapIdToLoadOnLogin != ModId.Null && CustomMapManager.mapIdToLoadOnLogin.Equals(CustomMapManager.currentRoomMapModId) && CustomMapManager.currentRoomMapApproved)
			{
				CustomMapManager.LoadMod(CustomMapManager.mapIdToLoadOnLogin);
			}
		}

		// Token: 0x06004E55 RID: 20053 RVA: 0x001856EA File Offset: 0x001838EA
		internal static IEnumerator TeleportToVirtualStump(short teleporterIdx, Action<bool> callback, GTZone playerEntranceZone, VirtualStumpTeleporterSerializer teleporterSerializer)
		{
			if (UGCPermissionManager.IsUGCDisabled)
			{
				yield break;
			}
			if (!CustomMapManager.hasInstance)
			{
				if (callback != null)
				{
					callback(false);
				}
				yield break;
			}
			CustomMapManager.instance.gameObject.SetActive(true);
			CustomMapManager.entranceZone = playerEntranceZone;
			CustomMapManager.teleporterNetworkObj = teleporterSerializer;
			PrivateUIRoom.ForceStartOverlay();
			GorillaTagger.Instance.overrideNotInFocus = true;
			GreyZoneManager greyZoneManager = GreyZoneManager.Instance;
			if (greyZoneManager != null)
			{
				greyZoneManager.ForceStopGreyZone();
			}
			if (CustomMapManager.instance.virtualStumpTeleportLocations.Count > 0)
			{
				int index = Random.Range(0, CustomMapManager.instance.virtualStumpTeleportLocations.Count);
				Transform randTeleportTarget = CustomMapManager.instance.virtualStumpTeleportLocations[index];
				CustomMapManager.instance.EnableTeleportHUD(true);
				if (CustomMapManager.teleporterNetworkObj != null)
				{
					CustomMapManager.teleporterNetworkObj.NotifyPlayerTeleporting(teleporterIdx, CustomMapManager.instance.localTeleportSFXSource);
				}
				yield return new WaitForSeconds(0.75f);
				CosmeticsController.instance.ClearCheckoutAndCart(false);
				CustomMapManager.instance.virtualStumpToggleableRoot.SetActive(true);
				GTPlayer.Instance.TeleportTo(randTeleportTarget, true, false);
				GorillaComputer.instance.SetInVirtualStump(true);
				yield return null;
				if (VRRig.LocalRig.IsNotNull() && VRRig.LocalRig.zoneEntity.IsNotNull())
				{
					VRRig.LocalRig.zoneEntity.DisableZoneChanges();
				}
				ZoneManagement.SetActiveZone(GTZone.customMaps);
				foreach (GameObject gameObject in CustomMapManager.instance.rootObjectsToDeactivateAfterTeleport)
				{
					if (gameObject != null)
					{
						gameObject.gameObject.SetActive(false);
					}
				}
				if (CustomMapManager.hasInstance && CustomMapManager.instance.virtualStumpZoneShaderSettings.IsNotNull())
				{
					CustomMapManager.instance.virtualStumpZoneShaderSettings.BecomeActiveInstance(false);
				}
				else
				{
					ZoneShaderSettings.ActivateDefaultSettings();
				}
				CustomMapManager.currentTeleportCallback = callback;
				CustomMapManager.pendingNewPrivateRoomName = "";
				CustomMapManager.preTeleportInPrivateRoom = false;
				if (NetworkSystem.Instance.InRoom)
				{
					if (NetworkSystem.Instance.SessionIsPrivate)
					{
						CustomMapManager.preTeleportInPrivateRoom = true;
						CustomMapManager.waitingForRoomJoin = true;
						CustomMapManager.pendingNewPrivateRoomName = GorillaComputer.instance.VStumpRoomPrepend + NetworkSystem.Instance.RoomName;
					}
					CustomMapManager.waitingForLoginDisconnect = true;
					NetworkSystem.Instance.ReturnToSinglePlayer();
				}
				else
				{
					CustomMapManager.RequestPlatformLogin();
				}
				randTeleportTarget = null;
			}
			yield break;
		}

		// Token: 0x06004E56 RID: 20054 RVA: 0x00185710 File Offset: 0x00183910
		private static void OnPlatformLoginComplete(ModIORequestResult result)
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			if (CustomMapManager.preTeleportInPrivateRoom)
			{
				CustomMapManager.delayedEndTeleportCoroutine = CustomMapManager.instance.StartCoroutine(CustomMapManager.DelayedEndTeleport());
				if (NetworkSystem.Instance.netState != NetSystemState.Idle)
				{
					CustomMapManager.delayedJoinCoroutine = CustomMapManager.instance.StartCoroutine(CustomMapManager.DelayedJoinVStumpPrivateRoom());
				}
				else
				{
					PhotonNetworkController.Instance.AttemptToJoinSpecificRoomWithCallback(CustomMapManager.pendingNewPrivateRoomName, JoinType.Solo, new Action<NetJoinResult>(CustomMapManager.OnJoinSpecificRoomResult));
				}
			}
			if (!CustomMapManager.preTeleportInPrivateRoom && !CustomMapManager.waitingForDisconnect)
			{
				CustomMapManager.EndTeleport(true);
			}
			CustomMapManager.preTeleportInPrivateRoom = false;
		}

		// Token: 0x06004E57 RID: 20055 RVA: 0x0018579F File Offset: 0x0018399F
		private static IEnumerator DelayedJoinVStumpPrivateRoom()
		{
			while (NetworkSystem.Instance.netState != NetSystemState.Idle)
			{
				yield return null;
			}
			PhotonNetworkController.Instance.AttemptToJoinSpecificRoomWithCallback(CustomMapManager.pendingNewPrivateRoomName, JoinType.Solo, new Action<NetJoinResult>(CustomMapManager.OnJoinSpecificRoomResult));
			yield break;
		}

		// Token: 0x06004E58 RID: 20056 RVA: 0x001857A8 File Offset: 0x001839A8
		public static void ExitVirtualStump(Action<bool> callback)
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			for (int i = 0; i < CustomMapManager.instance.virtualStumpEjectLocations.Length; i++)
			{
				if (CustomMapManager.instance.virtualStumpEjectLocations[i].ejectZone == CustomMapManager.entranceZone && CustomMapManager.instance.virtualStumpEjectLocations[i].ejectLocations.IsNullOrEmpty<GameObject>())
				{
					if (callback != null)
					{
						callback(false);
					}
					return;
				}
			}
			CustomMapManager.instance.dayNightManager.RequestRepopulateLightmaps();
			PrivateUIRoom.ForceStartOverlay();
			GorillaTagger.Instance.overrideNotInFocus = true;
			CustomMapManager.instance.EnableTeleportHUD(false);
			CustomMapManager.currentTeleportCallback = callback;
			CustomMapManager.exitVirtualStumpPending = true;
			if (!CustomMapManager.UnloadMod(false))
			{
				CustomMapManager.FinalizeExitVirtualStump();
			}
		}

		// Token: 0x06004E59 RID: 20057 RVA: 0x0018585C File Offset: 0x00183A5C
		private static void FinalizeExitVirtualStump()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			GTPlayer.Instance.SetHoverActive(false);
			VRRig.LocalRig.hoverboardVisual.SetNotHeld();
			RoomSystem.ClearOverridenRoomSize();
			CosmeticsController.instance.ClearCheckoutAndCart(false);
			foreach (GameObject gameObject in CustomMapManager.instance.rootObjectsToDeactivateAfterTeleport)
			{
				if (gameObject != null)
				{
					gameObject.gameObject.SetActive(true);
				}
			}
			GTZone gtzone = CustomMapManager.entranceZone;
			if (gtzone != GTZone.forest)
			{
				if (gtzone == GTZone.arcade)
				{
					ZoneManagement.SetActiveZone(GTZone.arcade);
				}
			}
			else
			{
				ZoneManagement.SetActiveZone(GTZone.forest);
			}
			Transform destination = null;
			for (int j = 0; j < CustomMapManager.instance.virtualStumpEjectLocations.Length; j++)
			{
				if (CustomMapManager.instance.virtualStumpEjectLocations[j].ejectZone == CustomMapManager.entranceZone)
				{
					CustomMapManager.pendingTeleportVFXIdx = (short)Random.Range(0, CustomMapManager.instance.virtualStumpEjectLocations[j].ejectLocations.Length);
					destination = CustomMapManager.instance.virtualStumpEjectLocations[j].ejectLocations[(int)CustomMapManager.pendingTeleportVFXIdx].transform;
					break;
				}
			}
			if (VRRig.LocalRig.IsNotNull() && VRRig.LocalRig.zoneEntity.IsNotNull())
			{
				VRRig.LocalRig.zoneEntity.EnableZoneChanges();
			}
			GorillaComputer.instance.SetInVirtualStump(false);
			GTPlayer.Instance.TeleportTo(destination, true, false);
			CustomMapManager.instance.virtualStumpToggleableRoot.SetActive(false);
			ZoneShaderSettings.ActivateDefaultSettings();
			VRRig.LocalRig.EnableVStumpReturnWatch(false);
			GTPlayer.Instance.SetHoverAllowed(false, true);
			CustomMapManager.exitVirtualStumpPending = false;
			if (CustomMapManager.delayedEndTeleportCoroutine != null)
			{
				CustomMapManager.instance.StopCoroutine(CustomMapManager.delayedEndTeleportCoroutine);
			}
			CustomMapManager.delayedEndTeleportCoroutine = CustomMapManager.instance.StartCoroutine(CustomMapManager.DelayedEndTeleport());
			if (CustomMapManager.preTeleportInPrivateRoom)
			{
				CustomMapManager.waitingForRoomJoin = true;
				CustomMapManager.pendingNewPrivateRoomName = CustomMapManager.pendingNewPrivateRoomName.RemoveAll(GorillaComputer.instance.VStumpRoomPrepend, StringComparison.OrdinalIgnoreCase);
				PhotonNetworkController.Instance.AttemptToJoinSpecificRoomWithCallback(CustomMapManager.pendingNewPrivateRoomName, JoinType.Solo, new Action<NetJoinResult>(CustomMapManager.OnJoinSpecificRoomResult));
				return;
			}
			if (!NetworkSystem.Instance.InRoom)
			{
				GorillaComputer.instance.allowedMapsToJoin = CustomMapManager.instance.exitVirtualStumpJoinTrigger.myCollider.myAllowedMapsToJoin;
				CustomMapManager.waitingForRoomJoin = true;
				PhotonNetworkController.Instance.AttemptToJoinPublicRoom(CustomMapManager.instance.exitVirtualStumpJoinTrigger, JoinType.Solo, null);
				return;
			}
			if (NetworkSystem.Instance.SessionIsPrivate)
			{
				CustomMapManager.waitingForRoomJoin = true;
				CustomMapManager.pendingNewPrivateRoomName = NetworkSystem.Instance.RoomName.RemoveAll(GorillaComputer.instance.VStumpRoomPrepend, StringComparison.OrdinalIgnoreCase);
				PhotonNetworkController.Instance.AttemptToJoinSpecificRoomWithCallback(CustomMapManager.pendingNewPrivateRoomName, JoinType.Solo, new Action<NetJoinResult>(CustomMapManager.OnJoinSpecificRoomResult));
				return;
			}
			GorillaComputer.instance.allowedMapsToJoin = CustomMapManager.instance.exitVirtualStumpJoinTrigger.myCollider.myAllowedMapsToJoin;
			CustomMapManager.waitingForRoomJoin = true;
			PhotonNetworkController.Instance.AttemptToJoinPublicRoom(CustomMapManager.instance.exitVirtualStumpJoinTrigger, JoinType.Solo, null);
		}

		// Token: 0x06004E5A RID: 20058 RVA: 0x00185B45 File Offset: 0x00183D45
		private static void OnJoinSpecificRoomResult(NetJoinResult result)
		{
			switch (result)
			{
			case NetJoinResult.Failed_Full:
				CustomMapManager.instance.OnJoinRoomFailed();
				return;
			case NetJoinResult.AlreadyInRoom:
				CustomMapManager.instance.OnJoinedRoom();
				return;
			case NetJoinResult.Failed_Other:
				CustomMapManager.waitingForDisconnect = true;
				CustomMapManager.shouldRetryJoin = true;
				return;
			default:
				return;
			}
		}

		// Token: 0x06004E5B RID: 20059 RVA: 0x00185B82 File Offset: 0x00183D82
		private static void OnJoinSpecificRoomResultFailureAllowed(NetJoinResult result)
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			switch (result)
			{
			case NetJoinResult.Success:
			case NetJoinResult.FallbackCreated:
				return;
			case NetJoinResult.Failed_Full:
			case NetJoinResult.Failed_Other:
				CustomMapManager.instance.OnJoinRoomFailed();
				return;
			case NetJoinResult.AlreadyInRoom:
				CustomMapManager.instance.OnJoinedRoom();
				return;
			default:
				return;
			}
		}

		// Token: 0x06004E5C RID: 20060 RVA: 0x00185BC4 File Offset: 0x00183DC4
		public static bool AreAllPlayersInVirtualStump()
		{
			if (!CustomMapManager.hasInstance)
			{
				return false;
			}
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (!CustomMapManager.instance.virtualStumpPlayerDetector.playerIDsCurrentlyTouching.Contains(vrrig.creator.UserId))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06004E5D RID: 20061 RVA: 0x00185C4C File Offset: 0x00183E4C
		public static bool IsRemotePlayerInVirtualStump(string playerID)
		{
			return CustomMapManager.hasInstance && !CustomMapManager.instance.virtualStumpPlayerDetector.IsNull() && CustomMapManager.instance.virtualStumpPlayerDetector.playerIDsCurrentlyTouching.Contains(playerID);
		}

		// Token: 0x06004E5E RID: 20062 RVA: 0x00185C84 File Offset: 0x00183E84
		public static bool IsLocalPlayerInVirtualStump()
		{
			return CustomMapManager.hasInstance && !CustomMapManager.instance.virtualStumpPlayerDetector.IsNull() && !VRRig.LocalRig.IsNull() && CustomMapManager.instance.virtualStumpPlayerDetector.playerIDsCurrentlyTouching.Contains(VRRig.LocalRig.creator.UserId);
		}

		// Token: 0x06004E5F RID: 20063 RVA: 0x00185CE4 File Offset: 0x00183EE4
		private void OnDisconnected()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			CustomMapManager.ClearRoomMap();
			if (CustomMapManager.waitingForLoginDisconnect)
			{
				CustomMapManager.waitingForLoginDisconnect = false;
				CustomMapManager.RequestPlatformLogin();
				return;
			}
			if (CustomMapManager.waitingForDisconnect)
			{
				CustomMapManager.waitingForDisconnect = false;
				if (CustomMapManager.shouldRetryJoin)
				{
					CustomMapManager.shouldRetryJoin = false;
					PhotonNetworkController.Instance.AttemptToJoinSpecificRoomWithCallback(CustomMapManager.pendingNewPrivateRoomName, JoinType.Solo, new Action<NetJoinResult>(CustomMapManager.OnJoinSpecificRoomResultFailureAllowed));
					return;
				}
				CustomMapManager.EndTeleport(true);
			}
		}

		// Token: 0x06004E60 RID: 20064 RVA: 0x00185D50 File Offset: 0x00183F50
		private static void RequestPlatformLogin()
		{
			ModIOManager.Initialize(delegate(ModIORequestResult result)
			{
				if (result.success)
				{
					ModIOManager.RequestPlatformLogin(new Action<ModIORequestResult>(CustomMapManager.OnPlatformLoginComplete));
					return;
				}
				CustomMapManager.OnPlatformLoginComplete(result);
			});
		}

		// Token: 0x06004E61 RID: 20065 RVA: 0x00185D76 File Offset: 0x00183F76
		private void OnJoinRoomFailed()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			if (CustomMapManager.waitingForRoomJoin)
			{
				CustomMapManager.waitingForRoomJoin = false;
				CustomMapManager.EndTeleport(false);
			}
		}

		// Token: 0x06004E62 RID: 20066 RVA: 0x00185D94 File Offset: 0x00183F94
		private static void EndTeleport(bool teleportSuccessful)
		{
			if (CustomMapManager.hasInstance)
			{
				if (CustomMapManager.delayedEndTeleportCoroutine != null)
				{
					CustomMapManager.instance.StopCoroutine(CustomMapManager.delayedEndTeleportCoroutine);
					CustomMapManager.delayedEndTeleportCoroutine = null;
				}
				if (CustomMapManager.delayedJoinCoroutine != null)
				{
					CustomMapManager.instance.StopCoroutine(CustomMapManager.delayedJoinCoroutine);
					CustomMapManager.delayedJoinCoroutine = null;
				}
			}
			CustomMapManager.DisableTeleportHUD();
			GorillaTagger.Instance.overrideNotInFocus = false;
			PrivateUIRoom.StopForcedOverlay();
			Action<bool> action = CustomMapManager.currentTeleportCallback;
			if (action != null)
			{
				action(teleportSuccessful);
			}
			CustomMapManager.currentTeleportCallback = null;
			if (CustomMapManager.hasInstance && !GorillaComputer.instance.IsPlayerInVirtualStump())
			{
				CustomMapManager.instance.gameObject.SetActive(false);
			}
		}

		// Token: 0x06004E63 RID: 20067 RVA: 0x00185E37 File Offset: 0x00184037
		private static IEnumerator DelayedEndTeleport()
		{
			yield return new WaitForSecondsRealtime(CustomMapManager.instance.maxPostTeleportRoomProcessingTime);
			CustomMapManager.EndTeleport(false);
			yield break;
		}

		// Token: 0x06004E64 RID: 20068 RVA: 0x00185E40 File Offset: 0x00184040
		private void OnJoinedRoom()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			if (CustomMapManager.waitingForRoomJoin)
			{
				CustomMapManager.waitingForRoomJoin = false;
				CustomMapManager.EndTeleport(true);
				if (CustomMapManager.pendingTeleportVFXIdx > -1 && CustomMapManager.teleporterNetworkObj != null)
				{
					CustomMapManager.teleporterNetworkObj.NotifyPlayerReturning(CustomMapManager.pendingTeleportVFXIdx);
					CustomMapManager.pendingTeleportVFXIdx = -1;
				}
			}
		}

		// Token: 0x06004E65 RID: 20069 RVA: 0x00185E94 File Offset: 0x00184094
		public static bool UnloadMod(bool returnToSinglePlayerIfInPublic = true)
		{
			if (CustomMapManager.unloadInProgress)
			{
				return false;
			}
			if (!CustomMapLoader.IsModLoaded(0L) && !CustomMapLoader.IsLoading)
			{
				if (CustomMapManager.loadInProgress)
				{
					CustomMapManager.abortModLoadIds.AddIfNew(CustomMapManager.loadingMapId);
					if (CustomMapManager.waitingForModDownload)
					{
						ModIOManager.AbortModDownload(CustomMapManager.loadingMapId);
					}
					CustomMapManager.loadInProgress = false;
					CustomMapManager.loadingMapId = ModId.Null;
					CustomMapManager.mapIdToLoadOnLogin = ModId.Null;
					CustomMapManager.waitingForModDownload = false;
					CustomMapManager.waitingForModInstall = false;
					CustomMapManager.waitingForModInstallId = ModId.Null;
					CustomMapManager.ClearRoomMap();
				}
				else
				{
					CustomMapManager.ClearRoomMap();
				}
				return false;
			}
			CustomMapManager.unloadInProgress = true;
			CustomMapManager.unloadingMapId = new ModId(CustomMapLoader.IsModLoaded(0L) ? CustomMapLoader.LoadedMapModId : CustomMapLoader.LoadingMapModId);
			CustomMapManager.OnMapLoadStatusChanged.Invoke(MapLoadStatus.Unloading, 0, "");
			CustomMapManager.loadInProgress = false;
			CustomMapManager.loadingMapId = ModId.Null;
			CustomMapManager.mapIdToLoadOnLogin = ModId.Null;
			CustomMapManager.waitingForModDownload = false;
			CustomMapManager.waitingForModInstall = false;
			CustomMapManager.waitingForModInstallId = ModId.Null;
			CustomMapManager.ClearRoomMap();
			CustomGameMode.LuaScript = "";
			if (CustomGameMode.gameScriptRunner != null)
			{
				CustomGameMode.StopScript();
			}
			CustomMapManager.customMapDefaultZoneShaderSettingsInitialized = false;
			CustomMapManager.customMapDefaultZoneShaderProperties = default(CMSZoneShaderSettings.CMSZoneShaderProperties);
			CustomMapManager.loadedCustomMapDefaultZoneShaderSettings = null;
			if (CustomMapManager.hasInstance)
			{
				CustomMapManager.instance.customMapDefaultZoneShaderSettings.CopySettings(CustomMapManager.instance.virtualStumpZoneShaderSettings, false);
				CustomMapManager.instance.virtualStumpZoneShaderSettings.BecomeActiveInstance(false);
				CustomMapManager.allCustomMapZoneShaderSettings.Clear();
			}
			CustomMapLoader.CloseDoorAndUnloadMod(new Action(CustomMapManager.OnMapUnloadCompleted));
			if (returnToSinglePlayerIfInPublic && NetworkSystem.Instance.InRoom && !NetworkSystem.Instance.SessionIsPrivate)
			{
				NetworkSystem.Instance.ReturnToSinglePlayer();
			}
			return true;
		}

		// Token: 0x06004E66 RID: 20070 RVA: 0x00186030 File Offset: 0x00184230
		private static void OnMapUnloadCompleted()
		{
			CustomMapManager.unloadInProgress = false;
			CustomMapManager.OnMapUnloadComplete.Invoke();
			CustomMapManager.currentRoomMapModId = ModId.Null;
			CustomMapManager.currentRoomMapApproved = false;
			CustomMapManager.OnRoomMapChanged.Invoke(ModId.Null);
			if (CustomMapManager.exitVirtualStumpPending)
			{
				CustomMapManager.FinalizeExitVirtualStump();
			}
		}

		// Token: 0x06004E67 RID: 20071 RVA: 0x00186070 File Offset: 0x00184270
		public static void LoadMod(ModId modId)
		{
			if (!CustomMapManager.hasInstance || CustomMapManager.loadInProgress)
			{
				return;
			}
			if (CustomMapManager.abortModLoadIds.Contains(modId))
			{
				CustomMapManager.abortModLoadIds.Remove(modId);
			}
			if (!ModIOManager.IsLoggedIn())
			{
				CustomMapManager.SetMapToLoadOnLogin(modId);
				return;
			}
			if (CustomMapLoader.IsModLoaded(modId))
			{
				return;
			}
			CustomMapManager.loadInProgress = true;
			CustomMapManager.loadingMapId = modId;
			CustomMapManager.mapIdToLoadOnLogin = ModId.Null;
			CustomMapManager.waitingForModDownload = false;
			CustomMapManager.waitingForModInstall = false;
			CustomMapManager.waitingForModInstallId = ModId.Null;
			SubscribedMod subscribedMod;
			if (!ModIOManager.GetSubscribedModProfile(modId, out subscribedMod))
			{
				ModIOManager.SubscribeToMod(modId, delegate(Result result)
				{
					if (CustomMapManager.abortModLoadIds.Contains(modId))
					{
						CustomMapManager.abortModLoadIds.Remove(modId);
						return;
					}
					if (!result.Succeeded())
					{
						CustomMapManager.instance.HandleMapLoadFailed("FAILED TO SUBSCRIBE TO MAP: " + result.message);
						return;
					}
					if (ModIOManager.GetSubscribedModProfile(modId, out subscribedMod))
					{
						CustomMapManager.OnMapLoadStatusChanged.Invoke(MapLoadStatus.Downloading, 0, "");
						CustomMapManager.instance.LoadSubscribedMod(subscribedMod);
					}
				});
				return;
			}
			CustomMapManager.instance.LoadSubscribedMod(subscribedMod);
		}

		// Token: 0x06004E68 RID: 20072 RVA: 0x00186150 File Offset: 0x00184350
		private void LoadSubscribedMod(SubscribedMod subscribedMod)
		{
			if (subscribedMod.status != SubscribedModStatus.Installed)
			{
				CustomMapManager.waitingForModInstall = true;
				CustomMapManager.waitingForModInstallId = subscribedMod.modProfile.id;
				if (subscribedMod.status == SubscribedModStatus.WaitingToDownload || subscribedMod.status == SubscribedModStatus.WaitingToUpdate)
				{
					ModIOManager.DownloadMod(subscribedMod.modProfile.id, delegate(ModIORequestResult result)
					{
						if (CustomMapManager.abortModLoadIds.Contains(subscribedMod.modProfile.id))
						{
							CustomMapManager.abortModLoadIds.Remove(subscribedMod.modProfile.id);
							return;
						}
						if (!result.success)
						{
							CustomMapManager.instance.HandleMapLoadFailed("FAILED TO START MAP DOWNLOAD: " + result.message);
						}
						GorillaTelemetry.PostCustomMapDownloadEvent(subscribedMod.modProfile.name, subscribedMod.modProfile.id, subscribedMod.modProfile.creator.username);
					});
				}
				return;
			}
			this.LoadInstalledMod(subscribedMod.modProfile.id);
		}

		// Token: 0x06004E69 RID: 20073 RVA: 0x001861E8 File Offset: 0x001843E8
		private void LoadInstalledMod(ModId installedModId)
		{
			CustomMapManager.waitingForModInstall = false;
			CustomMapManager.waitingForModInstallId = ModId.Null;
			SubscribedMod subscribedMod;
			ModIOManager.GetSubscribedModProfile(installedModId, out subscribedMod);
			if (subscribedMod.status != SubscribedModStatus.Installed)
			{
				this.HandleMapLoadFailed("MAP IS NOT INSTALLED");
				return;
			}
			FileInfo[] files = new DirectoryInfo(subscribedMod.directory).GetFiles("package.json");
			if (files.Length == 0)
			{
				this.HandleMapLoadFailed("COULD NOT FIND PACKAGE.JSON IN MAP FILES");
				return;
			}
			CustomMapLoader.LoadMap(installedModId.id, files[0].FullName, new Action<bool>(this.OnMapLoadFinished), new Action<MapLoadStatus, int, string>(this.OnMapLoadProgress), new Action<string>(CustomMapManager.OnSceneLoaded));
		}

		// Token: 0x06004E6A RID: 20074 RVA: 0x0018627F File Offset: 0x0018447F
		private void OnMapLoadProgress(MapLoadStatus loadStatus, int progress, string message)
		{
			CustomMapManager.OnMapLoadStatusChanged.Invoke(loadStatus, progress, message);
		}

		// Token: 0x06004E6B RID: 20075 RVA: 0x00186290 File Offset: 0x00184490
		private void OnMapLoadFinished(bool success)
		{
			CustomMapManager.loadInProgress = false;
			CustomMapManager.loadingMapId = ModId.Null;
			CustomMapManager.waitingForModDownload = false;
			CustomMapManager.waitingForModInstall = false;
			CustomMapManager.waitingForModInstallId = ModId.Null;
			if (success)
			{
				CustomMapLoader.OpenDoorToMap();
				if (CustomMapLoader.LoadedMapDescriptor.CustomGamemode != null)
				{
					CustomGameMode.LuaScript = CustomMapLoader.LoadedMapDescriptor.CustomGamemode.text;
					if (CustomGameMode.LuaScript != "" && CustomGameMode.GameModeInitialized && CustomGameMode.gameScriptRunner == null)
					{
						CustomGameMode.LuaStart();
					}
				}
			}
			CustomMapManager.OnMapLoadComplete.Invoke(success);
		}

		// Token: 0x06004E6C RID: 20076 RVA: 0x00186324 File Offset: 0x00184524
		private void HandleMapLoadFailed(string message = null)
		{
			CustomMapManager.loadInProgress = false;
			CustomMapManager.loadingMapId = ModId.Null;
			CustomMapManager.waitingForModInstall = false;
			CustomMapManager.waitingForModInstallId = ModId.Null;
			CustomMapManager.OnMapLoadStatusChanged.Invoke(MapLoadStatus.Error, 0, message ?? "UNKNOWN ERROR");
			CustomMapManager.OnMapLoadComplete.Invoke(false);
		}

		// Token: 0x06004E6D RID: 20077 RVA: 0x00186372 File Offset: 0x00184572
		public static bool IsUnloading()
		{
			return CustomMapManager.unloadInProgress;
		}

		// Token: 0x06004E6E RID: 20078 RVA: 0x00186379 File Offset: 0x00184579
		public static bool IsLoading(long mapId = 0L)
		{
			if (mapId == 0L)
			{
				return CustomMapManager.loadInProgress || CustomMapLoader.IsLoading;
			}
			return CustomMapManager.loadInProgress && CustomMapManager.loadingMapId.id == mapId;
		}

		// Token: 0x06004E6F RID: 20079 RVA: 0x001863A3 File Offset: 0x001845A3
		public static void SetMapToLoadOnLogin(ModId modId)
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			CustomMapManager.mapIdToLoadOnLogin = modId;
		}

		// Token: 0x06004E70 RID: 20080 RVA: 0x001863B3 File Offset: 0x001845B3
		public static void ClearMapToLoadOnLogin()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			CustomMapManager.mapIdToLoadOnLogin = ModId.Null;
		}

		// Token: 0x06004E71 RID: 20081 RVA: 0x001863C8 File Offset: 0x001845C8
		public static ModId GetRoomMapId()
		{
			if (NetworkSystem.Instance.InRoom)
			{
				if (CustomMapManager.currentRoomMapModId == ModId.Null && NetworkSystem.Instance.IsMasterClient && CustomMapLoader.IsModLoaded(0L))
				{
					CustomMapManager.currentRoomMapModId = new ModId(CustomMapLoader.LoadedMapModId);
				}
				return CustomMapManager.currentRoomMapModId;
			}
			if (CustomMapManager.IsLoading(0L))
			{
				return CustomMapManager.loadingMapId;
			}
			if (CustomMapLoader.IsModLoaded(0L))
			{
				return new ModId(CustomMapLoader.LoadedMapModId);
			}
			return ModId.Null;
		}

		// Token: 0x06004E72 RID: 20082 RVA: 0x00186444 File Offset: 0x00184644
		public static void SetRoomMod(long modId)
		{
			if (!CustomMapManager.hasInstance || modId == CustomMapManager.currentRoomMapModId.id)
			{
				return;
			}
			if (CustomMapManager.mapIdToLoadOnLogin.id != modId)
			{
				CustomMapManager.mapIdToLoadOnLogin = ModId.Null;
			}
			CustomMapManager.currentRoomMapModId = new ModId(modId);
			CustomMapManager.currentRoomMapApproved = false;
			CustomMapManager.OnRoomMapChanged.Invoke(CustomMapManager.currentRoomMapModId);
		}

		// Token: 0x06004E73 RID: 20083 RVA: 0x001864A0 File Offset: 0x001846A0
		public static void ClearRoomMap()
		{
			if (!CustomMapManager.hasInstance || CustomMapManager.currentRoomMapModId.Equals(ModId.Null))
			{
				return;
			}
			CustomMapManager.mapIdToLoadOnLogin = ModId.Null;
			CustomMapManager.currentRoomMapModId = ModId.Null;
			CustomMapManager.currentRoomMapApproved = false;
			CustomMapManager.OnRoomMapChanged.Invoke(ModId.Null);
		}

		// Token: 0x06004E74 RID: 20084 RVA: 0x001864EF File Offset: 0x001846EF
		public static bool CanLoadRoomMap()
		{
			return CustomMapManager.currentRoomMapModId != ModId.Null;
		}

		// Token: 0x06004E75 RID: 20085 RVA: 0x00186505 File Offset: 0x00184705
		public static void ApproveAndLoadRoomMap()
		{
			CustomMapManager.currentRoomMapApproved = true;
			CMSSerializer.ResetSyncedMapObjects();
			CustomMapManager.LoadMod(CustomMapManager.currentRoomMapModId);
		}

		// Token: 0x06004E76 RID: 20086 RVA: 0x0018651C File Offset: 0x0018471C
		public static void RequestEnableTeleportHUD(bool enteringVirtualStump)
		{
			if (CustomMapManager.hasInstance)
			{
				CustomMapManager.instance.EnableTeleportHUD(enteringVirtualStump);
			}
		}

		// Token: 0x06004E77 RID: 20087 RVA: 0x00186534 File Offset: 0x00184734
		private void EnableTeleportHUD(bool enteringVirtualStump)
		{
			if (CustomMapManager.teleportingHUD != null)
			{
				CustomMapManager.teleportingHUD.gameObject.SetActive(true);
				CustomMapManager.teleportingHUD.Initialize(enteringVirtualStump);
				return;
			}
			if (this.teleportingHUDPrefab != null)
			{
				Camera main = Camera.main;
				if (main != null)
				{
					GameObject gameObject = Object.Instantiate<GameObject>(this.teleportingHUDPrefab, main.transform);
					if (gameObject != null)
					{
						CustomMapManager.teleportingHUD = gameObject.GetComponent<VirtualStumpTeleportingHUD>();
						if (CustomMapManager.teleportingHUD != null)
						{
							CustomMapManager.teleportingHUD.Initialize(enteringVirtualStump);
						}
					}
				}
			}
		}

		// Token: 0x06004E78 RID: 20088 RVA: 0x001865C5 File Offset: 0x001847C5
		public static void DisableTeleportHUD()
		{
			if (CustomMapManager.teleportingHUD != null)
			{
				CustomMapManager.teleportingHUD.gameObject.SetActive(false);
			}
		}

		// Token: 0x06004E79 RID: 20089 RVA: 0x001865E4 File Offset: 0x001847E4
		public static void LoadZoneTriggered(int[] scenesToLoad, int[] scenesToUnload)
		{
			CustomMapLoader.LoadZoneTriggered(scenesToLoad, scenesToUnload, new Action<string>(CustomMapManager.OnSceneLoaded), new Action<string>(CustomMapManager.OnSceneUnloaded));
		}

		// Token: 0x06004E7A RID: 20090 RVA: 0x00186605 File Offset: 0x00184805
		private static void OnSceneLoaded(string sceneName)
		{
			CMSSerializer.ProcessSceneLoad(sceneName);
			CustomMapManager.ProcessZoneShaderSettings(sceneName);
		}

		// Token: 0x06004E7B RID: 20091 RVA: 0x00186614 File Offset: 0x00184814
		private static void OnSceneUnloaded(string sceneName)
		{
			CMSSerializer.UnregisterTriggers(sceneName);
			for (int i = CustomMapManager.allCustomMapZoneShaderSettings.Count - 1; i >= 0; i--)
			{
				if (CustomMapManager.allCustomMapZoneShaderSettings[i].IsNull())
				{
					CustomMapManager.allCustomMapZoneShaderSettings.RemoveAt(i);
				}
			}
		}

		// Token: 0x06004E7C RID: 20092 RVA: 0x0018665C File Offset: 0x0018485C
		private static void OnSceneTriggerHistoryProcessed(string sceneName)
		{
			CapsuleCollider bodyCollider = GTPlayer.Instance.bodyCollider;
			SphereCollider headCollider = GTPlayer.Instance.headCollider;
			Vector3 position = bodyCollider.transform.TransformPoint(bodyCollider.center);
			float radius = Mathf.Max(bodyCollider.height, bodyCollider.radius) * GTPlayer.Instance.scale;
			Collider[] array = new Collider[100];
			Physics.OverlapSphereNonAlloc(position, radius, array);
			foreach (Collider collider in array)
			{
				if (collider != null && collider.gameObject.scene.name.Equals(sceneName))
				{
					CMSTrigger[] components = collider.gameObject.GetComponents<CMSTrigger>();
					for (int j = 0; j < components.Length; j++)
					{
						if (components[j] != null)
						{
							components[j].OnTriggerEnter(bodyCollider);
							components[j].OnTriggerEnter(headCollider);
						}
					}
					CMSLoadingZone[] components2 = collider.gameObject.GetComponents<CMSLoadingZone>();
					for (int k = 0; k < components2.Length; k++)
					{
						if (components2[k] != null)
						{
							components2[k].OnTriggerEnter(bodyCollider);
						}
					}
					CMSZoneShaderSettingsTrigger[] components3 = collider.gameObject.GetComponents<CMSZoneShaderSettingsTrigger>();
					for (int l = 0; l < components3.Length; l++)
					{
						if (components3[l] != null)
						{
							components3[l].OnTriggerEnter(bodyCollider);
						}
					}
					HoverboardAreaTrigger[] components4 = collider.gameObject.GetComponents<HoverboardAreaTrigger>();
					for (int m = 0; m < components4.Length; m++)
					{
						if (components4[m] != null)
						{
							components4[m].OnTriggerEnter(headCollider);
						}
					}
					WaterVolume[] components5 = collider.gameObject.GetComponents<WaterVolume>();
					for (int n = 0; n < components5.Length; n++)
					{
						if (components5[n] != null)
						{
							components5[n].OnTriggerEnter(bodyCollider);
							components5[n].OnTriggerEnter(headCollider);
						}
					}
				}
			}
		}

		// Token: 0x06004E7D RID: 20093 RVA: 0x0018683B File Offset: 0x00184A3B
		public static void SetDefaultZoneShaderSettings(ZoneShaderSettings defaultCustomMapShaderSettings, CMSZoneShaderSettings.CMSZoneShaderProperties defaultZoneShaderProperties)
		{
			if (CustomMapManager.hasInstance)
			{
				CustomMapManager.instance.customMapDefaultZoneShaderSettings.CopySettings(defaultCustomMapShaderSettings, true);
				CustomMapManager.loadedCustomMapDefaultZoneShaderSettings = defaultCustomMapShaderSettings;
				CustomMapManager.customMapDefaultZoneShaderProperties = defaultZoneShaderProperties;
				CustomMapManager.customMapDefaultZoneShaderSettingsInitialized = true;
			}
		}

		// Token: 0x06004E7E RID: 20094 RVA: 0x0018686C File Offset: 0x00184A6C
		private static void ProcessZoneShaderSettings(string loadedSceneName)
		{
			if (CustomMapManager.hasInstance && CustomMapManager.customMapDefaultZoneShaderSettingsInitialized && CustomMapManager.customMapDefaultZoneShaderProperties.isInitialized)
			{
				for (int i = 0; i < CustomMapManager.allCustomMapZoneShaderSettings.Count; i++)
				{
					if (CustomMapManager.allCustomMapZoneShaderSettings[i].IsNotNull() && CustomMapManager.allCustomMapZoneShaderSettings[i] != CustomMapManager.loadedCustomMapDefaultZoneShaderSettings && CustomMapManager.allCustomMapZoneShaderSettings[i].gameObject.scene.name.Equals(loadedSceneName))
					{
						CustomMapManager.allCustomMapZoneShaderSettings[i].ReplaceDefaultValues(CustomMapManager.customMapDefaultZoneShaderProperties, true);
					}
				}
				return;
			}
			if (CustomMapManager.hasInstance && CustomMapManager.instance.virtualStumpZoneShaderSettings.IsNotNull())
			{
				for (int j = 0; j < CustomMapManager.allCustomMapZoneShaderSettings.Count; j++)
				{
					if (CustomMapManager.allCustomMapZoneShaderSettings[j].IsNotNull() && CustomMapManager.allCustomMapZoneShaderSettings[j].gameObject.scene.name.Equals(loadedSceneName))
					{
						CustomMapManager.allCustomMapZoneShaderSettings[j].ReplaceDefaultValues(CustomMapManager.instance.virtualStumpZoneShaderSettings, true);
					}
				}
			}
		}

		// Token: 0x06004E7F RID: 20095 RVA: 0x00186996 File Offset: 0x00184B96
		public static void AddZoneShaderSettings(ZoneShaderSettings zoneShaderSettings)
		{
			CustomMapManager.allCustomMapZoneShaderSettings.AddIfNew(zoneShaderSettings);
		}

		// Token: 0x06004E80 RID: 20096 RVA: 0x001869A3 File Offset: 0x00184BA3
		public static void ActivateDefaultZoneShaderSettings()
		{
			if (CustomMapManager.hasInstance && CustomMapManager.customMapDefaultZoneShaderSettingsInitialized)
			{
				CustomMapManager.instance.customMapDefaultZoneShaderSettings.BecomeActiveInstance(true);
				return;
			}
			if (CustomMapManager.hasInstance)
			{
				CustomMapManager.instance.virtualStumpZoneShaderSettings.BecomeActiveInstance(true);
			}
		}

		// Token: 0x06004E81 RID: 20097 RVA: 0x001869E0 File Offset: 0x00184BE0
		public static void ReturnToVirtualStump()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			if (!GorillaComputer.instance.IsPlayerInVirtualStump())
			{
				return;
			}
			if (CustomMapManager.instance.returnToVirtualStumpTeleportLocation.IsNotNull())
			{
				GTPlayer gtplayer = GTPlayer.Instance;
				if (gtplayer != null)
				{
					CustomMapLoader.ResetToInitialZone(new Action<string>(CustomMapManager.OnSceneLoaded), new Action<string>(CustomMapManager.OnSceneUnloaded));
					gtplayer.TeleportTo(CustomMapManager.instance.returnToVirtualStumpTeleportLocation, true, false);
				}
			}
		}

		// Token: 0x04005735 RID: 22325
		[OnEnterPlay_SetNull]
		private static volatile CustomMapManager instance;

		// Token: 0x04005736 RID: 22326
		[OnEnterPlay_Set(false)]
		private static bool hasInstance = false;

		// Token: 0x04005737 RID: 22327
		[SerializeField]
		private GameObject virtualStumpToggleableRoot;

		// Token: 0x04005738 RID: 22328
		[SerializeField]
		private GorillaNetworkJoinTrigger exitVirtualStumpJoinTrigger;

		// Token: 0x04005739 RID: 22329
		[SerializeField]
		private Transform returnToVirtualStumpTeleportLocation;

		// Token: 0x0400573A RID: 22330
		[SerializeField]
		private List<Transform> virtualStumpTeleportLocations;

		// Token: 0x0400573B RID: 22331
		[SerializeField]
		private ZoneEjectLocations[] virtualStumpEjectLocations;

		// Token: 0x0400573C RID: 22332
		[SerializeField]
		private GameObject[] rootObjectsToDeactivateAfterTeleport;

		// Token: 0x0400573D RID: 22333
		[SerializeField]
		private GorillaFriendCollider virtualStumpPlayerDetector;

		// Token: 0x0400573E RID: 22334
		[SerializeField]
		private ZoneShaderSettings virtualStumpZoneShaderSettings;

		// Token: 0x0400573F RID: 22335
		[SerializeField]
		private BetterDayNightManager dayNightManager;

		// Token: 0x04005740 RID: 22336
		[SerializeField]
		private ZoneShaderSettings customMapDefaultZoneShaderSettings;

		// Token: 0x04005741 RID: 22337
		[SerializeField]
		private GameObject teleportingHUDPrefab;

		// Token: 0x04005742 RID: 22338
		[SerializeField]
		private AudioSource localTeleportSFXSource;

		// Token: 0x04005743 RID: 22339
		private static GTZone entranceZone;

		// Token: 0x04005744 RID: 22340
		private static VirtualStumpTeleporterSerializer teleporterNetworkObj = null;

		// Token: 0x04005745 RID: 22341
		[SerializeField]
		private float maxPostTeleportRoomProcessingTime = 15f;

		// Token: 0x04005746 RID: 22342
		private static bool customMapDefaultZoneShaderSettingsInitialized;

		// Token: 0x04005747 RID: 22343
		private static ZoneShaderSettings loadedCustomMapDefaultZoneShaderSettings;

		// Token: 0x04005748 RID: 22344
		private static CMSZoneShaderSettings.CMSZoneShaderProperties customMapDefaultZoneShaderProperties;

		// Token: 0x04005749 RID: 22345
		private static readonly List<ZoneShaderSettings> allCustomMapZoneShaderSettings = new List<ZoneShaderSettings>();

		// Token: 0x0400574A RID: 22346
		private static GameModeSelectorButtonLayout gamemodeButtonLayout;

		// Token: 0x0400574B RID: 22347
		private static bool loadInProgress = false;

		// Token: 0x0400574C RID: 22348
		private static ModId loadingMapId = ModId.Null;

		// Token: 0x0400574D RID: 22349
		private static bool unloadInProgress = false;

		// Token: 0x0400574E RID: 22350
		private static ModId unloadingMapId = ModId.Null;

		// Token: 0x0400574F RID: 22351
		private static List<ModId> abortModLoadIds = new List<ModId>();

		// Token: 0x04005750 RID: 22352
		private static bool waitingForModDownload = false;

		// Token: 0x04005751 RID: 22353
		private static bool waitingForModInstall = false;

		// Token: 0x04005752 RID: 22354
		private static ModId waitingForModInstallId = ModId.Null;

		// Token: 0x04005753 RID: 22355
		private static bool preTeleportInPrivateRoom = false;

		// Token: 0x04005754 RID: 22356
		private static string pendingNewPrivateRoomName = "";

		// Token: 0x04005755 RID: 22357
		private static ModId mapIdToLoadOnLogin = ModId.Null;

		// Token: 0x04005756 RID: 22358
		private static Action<bool> currentTeleportCallback;

		// Token: 0x04005757 RID: 22359
		private static bool waitingForLoginDisconnect = false;

		// Token: 0x04005758 RID: 22360
		private static bool waitingForDisconnect = false;

		// Token: 0x04005759 RID: 22361
		private static bool waitingForRoomJoin = false;

		// Token: 0x0400575A RID: 22362
		private static bool shouldRetryJoin = false;

		// Token: 0x0400575B RID: 22363
		private static short pendingTeleportVFXIdx = -1;

		// Token: 0x0400575C RID: 22364
		private static bool exitVirtualStumpPending = false;

		// Token: 0x0400575D RID: 22365
		private static ModId currentRoomMapModId = ModId.Null;

		// Token: 0x0400575E RID: 22366
		private static bool currentRoomMapApproved = false;

		// Token: 0x0400575F RID: 22367
		private static VirtualStumpTeleportingHUD teleportingHUD;

		// Token: 0x04005760 RID: 22368
		private static Coroutine delayedEndTeleportCoroutine;

		// Token: 0x04005761 RID: 22369
		private static Coroutine delayedJoinCoroutine;

		// Token: 0x04005762 RID: 22370
		public static UnityEvent<ModId> OnRoomMapChanged = new UnityEvent<ModId>();

		// Token: 0x04005763 RID: 22371
		public static UnityEvent<MapLoadStatus, int, string> OnMapLoadStatusChanged = new UnityEvent<MapLoadStatus, int, string>();

		// Token: 0x04005764 RID: 22372
		public static UnityEvent<bool> OnMapLoadComplete = new UnityEvent<bool>();

		// Token: 0x04005765 RID: 22373
		public static UnityEvent OnMapUnloadComplete = new UnityEvent();
	}
}
