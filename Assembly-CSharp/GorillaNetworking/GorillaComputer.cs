using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using GorillaGameModes;
using GorillaTagScripts;
using GorillaTagScripts.ModIO;
using KID.Model;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using PlayFab.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace GorillaNetworking
{
	// Token: 0x02000D67 RID: 3431
	public class GorillaComputer : MonoBehaviour, IMatchmakingCallbacks, IGorillaSliceableSimple
	{
		// Token: 0x0600551D RID: 21789 RVA: 0x001A6243 File Offset: 0x001A4443
		public DateTime GetServerTime()
		{
			return this.startupTime + TimeSpan.FromSeconds((double)Time.realtimeSinceStartup);
		}

		// Token: 0x1700082F RID: 2095
		// (get) Token: 0x0600551E RID: 21790 RVA: 0x001A625B File Offset: 0x001A445B
		public string VStumpRoomPrepend
		{
			get
			{
				return this.virtualStumpRoomPrepend;
			}
		}

		// Token: 0x17000830 RID: 2096
		// (get) Token: 0x0600551F RID: 21791 RVA: 0x001A6264 File Offset: 0x001A4464
		public GorillaComputer.ComputerState currentState
		{
			get
			{
				GorillaComputer.ComputerState result;
				this.stateStack.TryPeek(out result);
				return result;
			}
		}

		// Token: 0x17000831 RID: 2097
		// (get) Token: 0x06005520 RID: 21792 RVA: 0x001A6280 File Offset: 0x001A4480
		public string NameTagPlayerPref
		{
			get
			{
				if (PlayFabAuthenticator.instance == null)
				{
					Debug.LogError("Trying to access PlayFab Authenticator Instance, but it is null. Will use a shared key for the nametag instead");
					return "nameTagsOn";
				}
				return "nameTagsOn-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			}
		}

		// Token: 0x17000832 RID: 2098
		// (get) Token: 0x06005521 RID: 21793 RVA: 0x001A62B7 File Offset: 0x001A44B7
		// (set) Token: 0x06005522 RID: 21794 RVA: 0x001A62BF File Offset: 0x001A44BF
		public bool NametagsEnabled { get; private set; }

		// Token: 0x17000833 RID: 2099
		// (get) Token: 0x06005523 RID: 21795 RVA: 0x001A62C8 File Offset: 0x001A44C8
		// (set) Token: 0x06005524 RID: 21796 RVA: 0x001A62D0 File Offset: 0x001A44D0
		public GorillaComputer.RedemptionResult RedemptionStatus
		{
			get
			{
				return this.redemptionResult;
			}
			set
			{
				this.redemptionResult = value;
				this.UpdateScreen();
			}
		}

		// Token: 0x17000834 RID: 2100
		// (get) Token: 0x06005525 RID: 21797 RVA: 0x001A62DF File Offset: 0x001A44DF
		// (set) Token: 0x06005526 RID: 21798 RVA: 0x001A62E7 File Offset: 0x001A44E7
		public string RedemptionCode
		{
			get
			{
				return this.redemptionCode;
			}
			set
			{
				this.redemptionCode = value;
			}
		}

		// Token: 0x06005527 RID: 21799 RVA: 0x001A62F0 File Offset: 0x001A44F0
		private void Awake()
		{
			if (GorillaComputer.instance == null)
			{
				GorillaComputer.instance = this;
				GorillaComputer.hasInstance = true;
			}
			else if (GorillaComputer.instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			this._activeOrderList = this.OrderList;
		}

		// Token: 0x06005528 RID: 21800 RVA: 0x001A6342 File Offset: 0x001A4542
		private void Start()
		{
			Debug.Log("Computer Init");
			this.Initialise();
		}

		// Token: 0x06005529 RID: 21801 RVA: 0x001A6354 File Offset: 0x001A4554
		public void OnEnable()
		{
			KIDManager.RegisterSessionUpdatedCallback_VoiceChat(new Action<bool, Permission.ManagedByEnum>(this.SetVoiceChatBySafety));
			KIDManager.RegisterSessionUpdatedCallback_CustomUsernames(new Action<bool, Permission.ManagedByEnum>(this.OnKIDSessionUpdated_CustomNicknames));
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x0600552A RID: 21802 RVA: 0x001A637F File Offset: 0x001A457F
		public void OnDisable()
		{
			KIDManager.UnregisterSessionUpdatedCallback_VoiceChat(new Action<bool, Permission.ManagedByEnum>(this.SetVoiceChatBySafety));
			KIDManager.UnregisterSessionUpdatedCallback_CustomUsernames(new Action<bool, Permission.ManagedByEnum>(this.OnKIDSessionUpdated_CustomNicknames));
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x0600552B RID: 21803 RVA: 0x001A63AA File Offset: 0x001A45AA
		protected void OnDestroy()
		{
			if (GorillaComputer.instance == this)
			{
				GorillaComputer.hasInstance = false;
				GorillaComputer.instance = null;
			}
			KIDManager.UnregisterSessionUpdateCallback_AnyPermission(new Action(this.OnSessionUpdate_GorillaComputer));
		}

		// Token: 0x0600552C RID: 21804 RVA: 0x001A63DC File Offset: 0x001A45DC
		public void SliceUpdate()
		{
			if ((this.internetFailure && Time.time < this.lastCheckedWifi + this.checkIfConnectedSeconds) || (!this.internetFailure && Time.time < this.lastCheckedWifi + this.checkIfDisconnectedSeconds))
			{
				if (!this.internetFailure && this.isConnectedToMaster && Time.time > this.lastUpdateTime + this.updateCooldown)
				{
					this.lastUpdateTime = Time.time;
					this.UpdateScreen();
				}
				return;
			}
			this.lastCheckedWifi = Time.time;
			this.stateUpdated = false;
			if (!this.CheckInternetConnection())
			{
				this.UpdateFailureText("NO WIFI OR LAN CONNECTION DETECTED.");
				this.internetFailure = true;
				return;
			}
			if (this.internetFailure)
			{
				if (this.CheckInternetConnection())
				{
					this.internetFailure = false;
				}
				this.RestoreFromFailureState();
				this.UpdateScreen();
				return;
			}
			if (this.isConnectedToMaster && Time.time > this.lastUpdateTime + this.updateCooldown)
			{
				this.lastUpdateTime = Time.time;
				this.UpdateScreen();
			}
		}

		// Token: 0x0600552D RID: 21805 RVA: 0x001A64D8 File Offset: 0x001A46D8
		private void Initialise()
		{
			GameEvents.OnGorrillaKeyboardButtonPressedEvent.AddListener(new UnityAction<GorillaKeyboardBindings>(this.PressButton));
			RoomSystem.JoinedRoomEvent += new Action(this.UpdateScreen);
			RoomSystem.LeftRoomEvent += new Action(this.UpdateScreen);
			RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.PlayerCountChangedCallback);
			RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.PlayerCountChangedCallback);
			this.InitialiseRoomScreens();
			this.InitialiseStrings();
			this.InitialiseAllRoomStates();
			this.UpdateScreen();
			byte[] bytes = new byte[]
			{
				Convert.ToByte(64)
			};
			this.virtualStumpRoomPrepend = Encoding.ASCII.GetString(bytes);
			this.initialized = true;
		}

		// Token: 0x0600552E RID: 21806 RVA: 0x001A65A8 File Offset: 0x001A47A8
		private void InitialiseRoomScreens()
		{
			this.screenText.Initialize(this.computerScreenRenderer.materials, this.wrongVersionMaterial, GameEvents.ScreenTextChangedEvent, GameEvents.ScreenTextMaterialsEvent);
			this.functionSelectText.Initialize(this.computerScreenRenderer.materials, this.wrongVersionMaterial, GameEvents.FunctionSelectTextChangedEvent, null);
		}

		// Token: 0x0600552F RID: 21807 RVA: 0x001A6600 File Offset: 0x001A4800
		private void InitialiseStrings()
		{
			this.roomToJoin = "";
			this.redText = "";
			this.blueText = "";
			this.greenText = "";
			this.currentName = "";
			this.savedName = "";
		}

		// Token: 0x06005530 RID: 21808 RVA: 0x001A6650 File Offset: 0x001A4850
		private void InitialiseAllRoomStates()
		{
			this.SwitchState(GorillaComputer.ComputerState.Startup, true);
			this.InitializeColorState();
			this.InitializeNameState();
			this.InitializeRoomState();
			this.InitializeTurnState();
			this.InitializeStartupState();
			this.InitializeQueueState();
			this.InitializeMicState();
			this.InitializeGroupState();
			this.InitializeVoiceState();
			this.InitializeAutoMuteState();
			this.InitializeGameMode();
			this.InitializeVisualsState();
			this.InitializeCreditsState();
			this.InitializeTimeState();
			this.InitializeSupportState();
			this.InitializeTroopState();
			this.InitializeKIdState();
			this.InitializeRedeemState();
		}

		// Token: 0x06005531 RID: 21809 RVA: 0x000023F5 File Offset: 0x000005F5
		private void InitializeStartupState()
		{
		}

		// Token: 0x06005532 RID: 21810 RVA: 0x000023F5 File Offset: 0x000005F5
		private void InitializeRoomState()
		{
		}

		// Token: 0x06005533 RID: 21811 RVA: 0x001A66D4 File Offset: 0x001A48D4
		private void InitializeColorState()
		{
			this.redValue = PlayerPrefs.GetFloat("redValue", 0f);
			this.greenValue = PlayerPrefs.GetFloat("greenValue", 0f);
			this.blueValue = PlayerPrefs.GetFloat("blueValue", 0f);
			this.blueText = Mathf.Floor(this.blueValue * 9f).ToString();
			this.redText = Mathf.Floor(this.redValue * 9f).ToString();
			this.greenText = Mathf.Floor(this.greenValue * 9f).ToString();
			this.colorCursorLine = 0;
			GorillaTagger.Instance.UpdateColor(this.redValue, this.greenValue, this.blueValue);
		}

		// Token: 0x06005534 RID: 21812 RVA: 0x001A67A0 File Offset: 0x001A49A0
		private void InitializeNameState()
		{
			int @int = PlayerPrefs.GetInt("nameTagsOn", -1);
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Custom_Nametags);
			switch (permissionDataByFeature.ManagedBy)
			{
			case Permission.ManagedByEnum.PLAYER:
				if (@int == -1)
				{
					this.NametagsEnabled = permissionDataByFeature.Enabled;
				}
				else
				{
					this.NametagsEnabled = (@int > 0);
				}
				break;
			case Permission.ManagedByEnum.GUARDIAN:
				this.NametagsEnabled = (permissionDataByFeature.Enabled && @int > 0);
				break;
			case Permission.ManagedByEnum.PROHIBITED:
				this.NametagsEnabled = false;
				break;
			}
			this.savedName = PlayerPrefs.GetString("playerName", "gorilla");
			NetworkSystem.Instance.SetMyNickName(this.savedName);
			this.currentName = this.savedName;
			VRRigCache.Instance.localRig.Rig.UpdateName();
			this.exactOneWeek = this.exactOneWeekFile.text.Split('\n', StringSplitOptions.None);
			this.anywhereOneWeek = this.anywhereOneWeekFile.text.Split('\n', StringSplitOptions.None);
			this.anywhereTwoWeek = this.anywhereTwoWeekFile.text.Split('\n', StringSplitOptions.None);
			for (int i = 0; i < this.exactOneWeek.Length; i++)
			{
				this.exactOneWeek[i] = this.exactOneWeek[i].ToLower().TrimEnd(new char[]
				{
					'\r',
					'\n'
				});
			}
			for (int j = 0; j < this.anywhereOneWeek.Length; j++)
			{
				this.anywhereOneWeek[j] = this.anywhereOneWeek[j].ToLower().TrimEnd(new char[]
				{
					'\r',
					'\n'
				});
			}
			for (int k = 0; k < this.anywhereTwoWeek.Length; k++)
			{
				this.anywhereTwoWeek[k] = this.anywhereTwoWeek[k].ToLower().TrimEnd(new char[]
				{
					'\r',
					'\n'
				});
			}
		}

		// Token: 0x06005535 RID: 21813 RVA: 0x001A696C File Offset: 0x001A4B6C
		private void InitializeTurnState()
		{
			GorillaSnapTurn.LoadSettingsFromPlayerPrefs();
		}

		// Token: 0x06005536 RID: 21814 RVA: 0x001A6973 File Offset: 0x001A4B73
		private void InitializeMicState()
		{
			this.pttType = PlayerPrefs.GetString("pttType", "ALL CHAT");
		}

		// Token: 0x06005537 RID: 21815 RVA: 0x001A698C File Offset: 0x001A4B8C
		private void InitializeAutoMuteState()
		{
			int @int = PlayerPrefs.GetInt("autoMute", 1);
			if (@int == 0)
			{
				this.autoMuteType = "OFF";
				return;
			}
			if (@int == 1)
			{
				this.autoMuteType = "MODERATE";
				return;
			}
			if (@int == 2)
			{
				this.autoMuteType = "AGGRESSIVE";
			}
		}

		// Token: 0x06005538 RID: 21816 RVA: 0x001A69D4 File Offset: 0x001A4BD4
		private void InitializeQueueState()
		{
			this.currentQueue = PlayerPrefs.GetString("currentQueue", "DEFAULT");
			this.allowedInCompetitive = (PlayerPrefs.GetInt("allowedInCompetitive", 0) == 1);
			if (!this.allowedInCompetitive && this.currentQueue == "COMPETITIVE")
			{
				PlayerPrefs.SetString("currentQueue", "DEFAULT");
				PlayerPrefs.Save();
				this.currentQueue = "DEFAULT";
			}
		}

		// Token: 0x06005539 RID: 21817 RVA: 0x001A6A43 File Offset: 0x001A4C43
		private void InitializeGroupState()
		{
			this.groupMapJoin = PlayerPrefs.GetString("groupMapJoin", "FOREST");
			this.groupMapJoinIndex = PlayerPrefs.GetInt("groupMapJoinIndex", 0);
			this.allowedMapsToJoin = this.friendJoinCollider.myAllowedMapsToJoin;
		}

		// Token: 0x0600553A RID: 21818 RVA: 0x001A6A7C File Offset: 0x001A4C7C
		private void InitializeTroopState()
		{
			bool flag = false;
			this.troopToJoin = (this.troopName = PlayerPrefs.GetString("troopName", string.Empty));
			if (!this.rememberTroopQueueState)
			{
				bool flag2 = PlayerPrefs.GetInt("troopQueueActive", 0) == 1;
				bool flag3 = this.currentQueue != "DEFAULT" && this.currentQueue != "COMPETITIVE" && this.currentQueue != "MINIGAMES";
				if (flag2 || flag3)
				{
					this.currentQueue = "DEFAULT";
					PlayerPrefs.SetInt("troopQueueActive", 0);
					PlayerPrefs.SetString("currentQueue", this.currentQueue);
					PlayerPrefs.Save();
				}
			}
			this.troopQueueActive = (PlayerPrefs.GetInt("troopQueueActive", 0) == 1);
			if (this.troopQueueActive && !this.IsValidTroopName(this.troopName))
			{
				this.troopQueueActive = false;
				PlayerPrefs.SetInt("troopQueueActive", this.troopQueueActive ? 1 : 0);
				this.currentQueue = "DEFAULT";
				PlayerPrefs.SetString("currentQueue", this.currentQueue);
				flag = true;
			}
			if (this.troopQueueActive)
			{
				base.StartCoroutine(this.HandleInitialTroopQueueState());
			}
			if (flag)
			{
				PlayerPrefs.Save();
			}
		}

		// Token: 0x0600553B RID: 21819 RVA: 0x001A6BA7 File Offset: 0x001A4DA7
		private IEnumerator HandleInitialTroopQueueState()
		{
			Debug.Log("HandleInitialTroopQueueState()");
			while (!PlayFabCloudScriptAPI.IsEntityLoggedIn())
			{
				yield return null;
			}
			this.RequestTroopPopulation(false);
			while (this.currentTroopPopulation < 0)
			{
				yield return null;
			}
			if (this.currentTroopPopulation < 2)
			{
				Debug.Log("Low population - starting in DEFAULT queue");
				this.JoinDefaultQueue();
			}
			yield break;
		}

		// Token: 0x0600553C RID: 21820 RVA: 0x001A6BB8 File Offset: 0x001A4DB8
		private void InitializeVoiceState()
		{
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat);
			string text = PlayerPrefs.GetString("voiceChatOn", "");
			string defaultValue = "FALSE";
			switch (permissionDataByFeature.ManagedBy)
			{
			case Permission.ManagedByEnum.PLAYER:
				if (string.IsNullOrEmpty(text))
				{
					defaultValue = (permissionDataByFeature.Enabled ? "TRUE" : "FALSE");
				}
				else
				{
					defaultValue = text;
				}
				break;
			case Permission.ManagedByEnum.GUARDIAN:
				if (permissionDataByFeature.Enabled)
				{
					text = (string.IsNullOrEmpty(text) ? "FALSE" : text);
					defaultValue = text;
				}
				else
				{
					defaultValue = "FALSE";
				}
				break;
			case Permission.ManagedByEnum.PROHIBITED:
				defaultValue = "FALSE";
				break;
			}
			this.voiceChatOn = PlayerPrefs.GetString("voiceChatOn", defaultValue);
		}

		// Token: 0x0600553D RID: 21821 RVA: 0x001A6C60 File Offset: 0x001A4E60
		private void InitializeGameMode()
		{
			string text = PlayerPrefs.GetString("currentGameMode", GameModeType.Infection.ToString());
			GameModeType gameModeType;
			try
			{
				gameModeType = Enum.Parse<GameModeType>(text, true);
			}
			catch
			{
				gameModeType = GameModeType.Infection;
				text = GameModeType.Infection.ToString();
			}
			if (gameModeType != GameModeType.Casual && gameModeType != GameModeType.Infection && gameModeType != GameModeType.HuntDown && gameModeType != GameModeType.Paintbrawl && gameModeType != GameModeType.Ambush && gameModeType != GameModeType.PropHunt)
			{
				PlayerPrefs.SetString("currentGameMode", GameModeType.Infection.ToString());
				PlayerPrefs.Save();
				text = GameModeType.Infection.ToString();
			}
			this.leftHanded = (PlayerPrefs.GetInt("leftHanded", 0) == 1);
			this.OnModeSelectButtonPress(text, this.leftHanded);
			GameModePages.SetSelectedGameModeShared(text);
		}

		// Token: 0x0600553E RID: 21822 RVA: 0x000023F5 File Offset: 0x000005F5
		private void InitializeCreditsState()
		{
		}

		// Token: 0x0600553F RID: 21823 RVA: 0x001A6D24 File Offset: 0x001A4F24
		private void InitializeTimeState()
		{
			BetterDayNightManager.instance.currentSetting = TimeSettings.Normal;
		}

		// Token: 0x06005540 RID: 21824 RVA: 0x001A6D33 File Offset: 0x001A4F33
		private void InitializeSupportState()
		{
			this.displaySupport = false;
		}

		// Token: 0x06005541 RID: 21825 RVA: 0x001A6D3C File Offset: 0x001A4F3C
		private void InitializeVisualsState()
		{
			this.disableParticles = (PlayerPrefs.GetString("disableParticles", "FALSE") == "TRUE");
			GorillaTagger.Instance.ShowCosmeticParticles(!this.disableParticles);
			this.instrumentVolume = PlayerPrefs.GetFloat("instrumentVolume", 0.1f);
		}

		// Token: 0x06005542 RID: 21826 RVA: 0x001A6D90 File Offset: 0x001A4F90
		private void InitializeRedeemState()
		{
			this.RedemptionStatus = GorillaComputer.RedemptionResult.Empty;
		}

		// Token: 0x06005543 RID: 21827 RVA: 0x001A6D99 File Offset: 0x001A4F99
		private bool CheckInternetConnection()
		{
			return Application.internetReachability > NetworkReachability.NotReachable;
		}

		// Token: 0x06005544 RID: 21828 RVA: 0x001A6DA4 File Offset: 0x001A4FA4
		public void OnConnectedToMasterStuff()
		{
			if (!this.isConnectedToMaster)
			{
				this.isConnectedToMaster = true;
				GorillaServer.Instance.ReturnCurrentVersion(new ReturnCurrentVersionRequest
				{
					CurrentVersion = NetworkSystemConfig.AppVersionStripped,
					UpdatedSynchTest = new int?(this.includeUpdatedServerSynchTest)
				}, new Action<ExecuteFunctionResult>(this.OnReturnCurrentVersion), new Action<PlayFabError>(GorillaComputer.OnErrorShared));
				if (this.startupMillis == 0L && !this.tryGetTimeAgain)
				{
					this.GetCurrentTime();
				}
				RuntimePlatform platform = Application.platform;
				this.SaveModAccountData();
				bool safety = PlayFabAuthenticator.instance.GetSafety();
				if (!KIDManager.KidEnabledAndReady && !KIDManager.HasSession)
				{
					this.SetComputerSettingsBySafety(safety, new GorillaComputer.ComputerState[]
					{
						GorillaComputer.ComputerState.Voice,
						GorillaComputer.ComputerState.AutoMute,
						GorillaComputer.ComputerState.Name,
						GorillaComputer.ComputerState.Group
					}, false);
				}
			}
		}

		// Token: 0x06005545 RID: 21829 RVA: 0x001A6E64 File Offset: 0x001A5064
		private void OnReturnCurrentVersion(ExecuteFunctionResult result)
		{
			JsonObject jsonObject = (JsonObject)result.FunctionResult;
			if (jsonObject == null)
			{
				this.GeneralFailureMessage(this.versionMismatch);
				return;
			}
			object obj;
			if (jsonObject.TryGetValue("SynchTime", out obj))
			{
				Debug.Log("message value is: " + (string)obj);
			}
			if (jsonObject.TryGetValue("Fail", out obj) && (bool)obj)
			{
				this.GeneralFailureMessage(this.versionMismatch);
				return;
			}
			if (jsonObject.TryGetValue("ResultCode", out obj) && (ulong)obj != 0UL)
			{
				this.GeneralFailureMessage(this.versionMismatch);
				return;
			}
			if (jsonObject.TryGetValue("QueueStats", out obj) && ((JsonObject)obj).TryGetValue("TopTroops", out obj))
			{
				this.topTroops.Clear();
				foreach (object obj2 in ((JsonArray)obj))
				{
					this.topTroops.Add(obj2.ToString());
				}
			}
			if (jsonObject.TryGetValue("BannedUsers", out obj))
			{
				this.usersBanned = int.Parse((string)obj);
			}
			this.UpdateScreen();
		}

		// Token: 0x06005546 RID: 21830 RVA: 0x001A6FA0 File Offset: 0x001A51A0
		public void SaveModAccountData()
		{
			string path = Application.persistentDataPath + "/DoNotShareWithAnyoneEVERNoMatterWhatTheySay.txt";
			if (File.Exists(path))
			{
				return;
			}
			GorillaServer.Instance.ReturnMyOculusHash(delegate(ExecuteFunctionResult result)
			{
				object obj;
				if (((JsonObject)result.FunctionResult).TryGetValue("oculusHash", out obj))
				{
					StreamWriter streamWriter = new StreamWriter(path);
					streamWriter.Write(PlayFabAuthenticator.instance.GetPlayFabPlayerId() + "." + (string)obj);
					streamWriter.Close();
				}
			}, delegate(PlayFabError error)
			{
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
				}
			});
		}

		// Token: 0x06005547 RID: 21831 RVA: 0x001A7010 File Offset: 0x001A5210
		public void PressButton(GorillaKeyboardBindings buttonPressed)
		{
			if (this.currentState == GorillaComputer.ComputerState.Startup)
			{
				this.ProcessStartupState(buttonPressed);
				this.UpdateScreen();
				return;
			}
			this.RequestTroopPopulation(false);
			bool flag = true;
			if (buttonPressed == GorillaKeyboardBindings.up)
			{
				flag = false;
				this.DecreaseState();
			}
			else if (buttonPressed == GorillaKeyboardBindings.down)
			{
				flag = false;
				this.IncreaseState();
			}
			if (flag)
			{
				switch (this.currentState)
				{
				case GorillaComputer.ComputerState.Color:
					this.ProcessColorState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Name:
					this.ProcessNameState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Turn:
					this.ProcessTurnState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Mic:
					this.ProcessMicState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Room:
					this.ProcessRoomState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Queue:
					this.ProcessQueueState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Group:
					this.ProcessGroupState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Voice:
					this.ProcessVoiceState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.AutoMute:
					this.ProcessAutoMuteState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Credits:
					this.ProcessCreditsState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Visuals:
					this.ProcessVisualsState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.NameWarning:
					this.ProcessNameWarningState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Support:
					this.ProcessSupportState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Troop:
					this.ProcessTroopState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.KID:
					this.ProcessKIdState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Redemption:
					this.ProcessRedemptionState(buttonPressed);
					break;
				}
			}
			this.UpdateScreen();
		}

		// Token: 0x06005548 RID: 21832 RVA: 0x001A7154 File Offset: 0x001A5354
		public void OnModeSelectButtonPress(string gameMode, bool leftHand)
		{
			this.lastPressedGameMode = gameMode;
			PlayerPrefs.SetString("currentGameMode", gameMode);
			if (leftHand != this.leftHanded)
			{
				PlayerPrefs.SetInt("leftHanded", leftHand ? 1 : 0);
				this.leftHanded = leftHand;
			}
			PlayerPrefs.Save();
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				FriendshipGroupDetection.Instance.SendRequestPartyGameMode(gameMode);
				return;
			}
			this.SetGameModeWithoutButton(gameMode);
		}

		// Token: 0x06005549 RID: 21833 RVA: 0x001A71B8 File Offset: 0x001A53B8
		public void SetGameModeWithoutButton(string gameMode)
		{
			this.currentGameMode.Value = gameMode;
			this.UpdateGameModeText();
			PhotonNetworkController.Instance.UpdateTriggerScreens();
		}

		// Token: 0x0600554A RID: 21834 RVA: 0x001A71D8 File Offset: 0x001A53D8
		public void RegisterPrimaryJoinTrigger(GorillaNetworkJoinTrigger trigger)
		{
			this.primaryTriggersByZone[trigger.networkZone] = trigger;
		}

		// Token: 0x0600554B RID: 21835 RVA: 0x001A71EC File Offset: 0x001A53EC
		private GorillaNetworkJoinTrigger GetSelectedMapJoinTrigger()
		{
			GorillaNetworkJoinTrigger result;
			this.primaryTriggersByZone.TryGetValue(this.allowedMapsToJoin[Mathf.Min(this.allowedMapsToJoin.Length - 1, this.groupMapJoinIndex)], out result);
			return result;
		}

		// Token: 0x0600554C RID: 21836 RVA: 0x001A7224 File Offset: 0x001A5424
		public GorillaNetworkJoinTrigger GetJoinTriggerForZone(string zone)
		{
			GorillaNetworkJoinTrigger result;
			this.primaryTriggersByZone.TryGetValue(zone, out result);
			return result;
		}

		// Token: 0x0600554D RID: 21837 RVA: 0x001A7244 File Offset: 0x001A5444
		public GorillaNetworkJoinTrigger GetJoinTriggerFromFullGameModeString(string gameModeString)
		{
			foreach (KeyValuePair<string, GorillaNetworkJoinTrigger> keyValuePair in this.primaryTriggersByZone)
			{
				if (gameModeString.StartsWith(keyValuePair.Key))
				{
					return keyValuePair.Value;
				}
			}
			return null;
		}

		// Token: 0x0600554E RID: 21838 RVA: 0x001A72AC File Offset: 0x001A54AC
		public void OnGroupJoinButtonPress(int mapJoinIndex, GorillaFriendCollider chosenFriendJoinCollider)
		{
			Debug.Log("On Group button press. Map:" + mapJoinIndex.ToString() + " - collider: " + chosenFriendJoinCollider.name);
			if (mapJoinIndex >= this.allowedMapsToJoin.Length)
			{
				this.roomNotAllowed = true;
				this.currentStateIndex = 0;
				this.SwitchState(this.GetState(this.currentStateIndex), true);
				return;
			}
			GorillaNetworkJoinTrigger selectedMapJoinTrigger = this.GetSelectedMapJoinTrigger();
			if (!FriendshipGroupDetection.Instance.IsInParty)
			{
				if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate)
				{
					PhotonNetworkController.Instance.FriendIDList = new List<string>(chosenFriendJoinCollider.playerIDsCurrentlyTouching);
					foreach (string str in this.networkController.FriendIDList)
					{
						Debug.Log("Friend ID:" + str);
					}
					PhotonNetworkController.Instance.shuffler = Random.Range(0, 99).ToString().PadLeft(2, '0') + Random.Range(0, 99999999).ToString().PadLeft(8, '0');
					PhotonNetworkController.Instance.keyStr = Random.Range(0, 99999999).ToString().PadLeft(8, '0');
					RoomSystem.SendNearbyFollowCommand(chosenFriendJoinCollider, PhotonNetworkController.Instance.shuffler, PhotonNetworkController.Instance.keyStr);
					PhotonNetwork.SendAllOutgoingCommands();
					PhotonNetworkController.Instance.AttemptToJoinPublicRoom(selectedMapJoinTrigger, JoinType.JoinWithNearby, null);
					this.currentStateIndex = 0;
					this.SwitchState(this.GetState(this.currentStateIndex), true);
				}
				return;
			}
			if (selectedMapJoinTrigger != null && selectedMapJoinTrigger.CanPartyJoin())
			{
				PhotonNetworkController.Instance.AttemptToJoinPublicRoom(selectedMapJoinTrigger, JoinType.ForceJoinWithParty, null);
				this.currentStateIndex = 0;
				this.SwitchState(this.GetState(this.currentStateIndex), true);
				return;
			}
			this.UpdateScreen();
		}

		// Token: 0x0600554F RID: 21839 RVA: 0x001A749C File Offset: 0x001A569C
		public void CompQueueUnlockButtonPress()
		{
			this.allowedInCompetitive = true;
			PlayerPrefs.SetInt("allowedInCompetitive", 1);
			PlayerPrefs.Save();
			if (RankedProgressionManager.Instance != null)
			{
				RankedProgressionManager.Instance.RequestUnlockCompetitiveQueue(true);
			}
		}

		// Token: 0x06005550 RID: 21840 RVA: 0x001A74D0 File Offset: 0x001A56D0
		private void SwitchState(GorillaComputer.ComputerState newState, bool clearStack = true)
		{
			if (this.previousComputerState != this.currentComputerState)
			{
				this.previousComputerState = this.currentComputerState;
			}
			this.currentComputerState = newState;
			if (this.LoadingRoutine != null)
			{
				base.StopCoroutine(this.LoadingRoutine);
			}
			if (clearStack)
			{
				this.stateStack.Clear();
			}
			this.stateStack.Push(newState);
		}

		// Token: 0x06005551 RID: 21841 RVA: 0x001A752C File Offset: 0x001A572C
		private void PopState()
		{
			this.currentComputerState = this.previousComputerState;
			if (this.stateStack.Count <= 1)
			{
				Debug.LogError("Can't pop into an empty stack");
				return;
			}
			this.stateStack.Pop();
			this.UpdateScreen();
		}

		// Token: 0x06005552 RID: 21842 RVA: 0x001A7565 File Offset: 0x001A5765
		private void SwitchToWarningState()
		{
			this.warningConfirmationInputString = string.Empty;
			this.SwitchState(GorillaComputer.ComputerState.NameWarning, false);
		}

		// Token: 0x06005553 RID: 21843 RVA: 0x001A757B File Offset: 0x001A577B
		private void SwitchToLoadingState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Loading, false);
		}

		// Token: 0x06005554 RID: 21844 RVA: 0x001A7586 File Offset: 0x001A5786
		private void ProcessStartupState(GorillaKeyboardBindings buttonPressed)
		{
			this.SwitchState(this.GetState(this.currentStateIndex), true);
		}

		// Token: 0x06005555 RID: 21845 RVA: 0x001A759C File Offset: 0x001A579C
		private void ProcessColorState(GorillaKeyboardBindings buttonPressed)
		{
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.enter:
				return;
			case GorillaKeyboardBindings.option1:
				this.colorCursorLine = 0;
				return;
			case GorillaKeyboardBindings.option2:
				this.colorCursorLine = 1;
				return;
			case GorillaKeyboardBindings.option3:
				this.colorCursorLine = 2;
				return;
			default:
			{
				int num = (int)buttonPressed;
				if (num < 10)
				{
					switch (this.colorCursorLine)
					{
					case 0:
						this.redText = num.ToString();
						this.redValue = (float)num / 9f;
						PlayerPrefs.SetFloat("redValue", this.redValue);
						break;
					case 1:
						this.greenText = num.ToString();
						this.greenValue = (float)num / 9f;
						PlayerPrefs.SetFloat("greenValue", this.greenValue);
						break;
					case 2:
						this.blueText = num.ToString();
						this.blueValue = (float)num / 9f;
						PlayerPrefs.SetFloat("blueValue", this.blueValue);
						break;
					}
					GorillaTagger.Instance.UpdateColor(this.redValue, this.greenValue, this.blueValue);
					PlayerPrefs.Save();
					if (NetworkSystem.Instance.InRoom)
					{
						GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[]
						{
							this.redValue,
							this.greenValue,
							this.blueValue
						});
					}
				}
				return;
			}
			}
		}

		// Token: 0x06005556 RID: 21846 RVA: 0x001A76FC File Offset: 0x001A58FC
		public void ProcessNameState(GorillaKeyboardBindings buttonPressed)
		{
			if (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags))
			{
				switch (buttonPressed)
				{
				case GorillaKeyboardBindings.delete:
					if (this.currentName.Length > 0 && this.NametagsEnabled)
					{
						this.currentName = this.currentName.Substring(0, this.currentName.Length - 1);
						return;
					}
					break;
				case GorillaKeyboardBindings.enter:
					if (this.currentName != this.savedName && this.currentName != "" && this.NametagsEnabled)
					{
						this.CheckAutoBanListForPlayerName(this.currentName);
						return;
					}
					break;
				case GorillaKeyboardBindings.option1:
					this.UpdateNametagSetting(!this.NametagsEnabled, true);
					return;
				default:
					if (this.NametagsEnabled && this.currentName.Length < 12 && (buttonPressed < GorillaKeyboardBindings.up || buttonPressed > GorillaKeyboardBindings.option3))
					{
						string str = this.currentName;
						string str2;
						if (buttonPressed >= GorillaKeyboardBindings.up)
						{
							str2 = buttonPressed.ToString();
						}
						else
						{
							int num = (int)buttonPressed;
							str2 = num.ToString();
						}
						this.currentName = str + str2;
						return;
					}
					break;
				}
			}
			else if (buttonPressed != GorillaKeyboardBindings.option2)
			{
				if (buttonPressed != GorillaKeyboardBindings.option3)
				{
					return;
				}
				if (this._currentScreentState != GorillaComputer.EKidScreenState.Show_OTP)
				{
					return;
				}
				this.ProcessScreen_SetupKID();
			}
			else
			{
				if (this._currentScreentState != GorillaComputer.EKidScreenState.Ready)
				{
					this.ProcessScreen_SetupKID();
					return;
				}
				this.RequestUpdatedPermissions();
				return;
			}
		}

		// Token: 0x06005557 RID: 21847 RVA: 0x001A7840 File Offset: 0x001A5A40
		private void ProcessRoomState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.limitOnlineScreens)
			{
				return;
			}
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups);
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.delete:
				if (flag && ((this.playerInVirtualStump && this.roomToJoin.Length > 1) || (!this.playerInVirtualStump && this.roomToJoin.Length > 0)))
				{
					this.roomToJoin = this.roomToJoin.Substring(0, this.roomToJoin.Length - 1);
					return;
				}
				break;
			case GorillaKeyboardBindings.enter:
				if (flag && ((!this.playerInVirtualStump && this.roomToJoin != "") || (this.playerInVirtualStump && this.roomToJoin.Length > 1)))
				{
					this.CheckAutoBanListForRoomName(this.roomToJoin);
					return;
				}
				break;
			case GorillaKeyboardBindings.option1:
				if (FriendshipGroupDetection.Instance.IsInParty)
				{
					FriendshipGroupDetection.Instance.LeaveParty();
					this.DisconnectAfterDelay(1f);
					return;
				}
				NetworkSystem.Instance.ReturnToSinglePlayer();
				return;
			case GorillaKeyboardBindings.option2:
				if (this._currentScreentState != GorillaComputer.EKidScreenState.Ready)
				{
					this.ProcessScreen_SetupKID();
					return;
				}
				this.RequestUpdatedPermissions();
				return;
			case GorillaKeyboardBindings.option3:
				if (this._currentScreentState != GorillaComputer.EKidScreenState.Show_OTP)
				{
					return;
				}
				this.ProcessScreen_SetupKID();
				return;
			default:
				if (flag && this.roomToJoin.Length < 10)
				{
					string str = this.roomToJoin;
					string str2;
					if (buttonPressed >= GorillaKeyboardBindings.up)
					{
						str2 = buttonPressed.ToString();
					}
					else
					{
						int num = (int)buttonPressed;
						str2 = num.ToString();
					}
					this.roomToJoin = str + str2;
				}
				break;
			}
		}

		// Token: 0x06005558 RID: 21848 RVA: 0x001A79B0 File Offset: 0x001A5BB0
		private void DisconnectAfterDelay(float seconds)
		{
			GorillaComputer.<DisconnectAfterDelay>d__169 <DisconnectAfterDelay>d__;
			<DisconnectAfterDelay>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<DisconnectAfterDelay>d__.seconds = seconds;
			<DisconnectAfterDelay>d__.<>1__state = -1;
			<DisconnectAfterDelay>d__.<>t__builder.Start<GorillaComputer.<DisconnectAfterDelay>d__169>(ref <DisconnectAfterDelay>d__);
		}

		// Token: 0x06005559 RID: 21849 RVA: 0x001A79E8 File Offset: 0x001A5BE8
		private void ProcessTurnState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed < GorillaKeyboardBindings.up)
			{
				GorillaSnapTurn.UpdateAndSaveTurnFactor((int)buttonPressed);
				return;
			}
			string text = string.Empty;
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.option1:
				text = "SNAP";
				break;
			case GorillaKeyboardBindings.option2:
				text = "SMOOTH";
				break;
			case GorillaKeyboardBindings.option3:
				text = "NONE";
				break;
			}
			if (text.Length > 0)
			{
				GorillaSnapTurn.UpdateAndSaveTurnType(text);
			}
		}

		// Token: 0x0600555A RID: 21850 RVA: 0x001A7A48 File Offset: 0x001A5C48
		private void ProcessMicState(GorillaKeyboardBindings buttonPressed)
		{
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.option1:
				this.pttType = "ALL CHAT";
				PlayerPrefs.SetString("pttType", this.pttType);
				PlayerPrefs.Save();
				return;
			case GorillaKeyboardBindings.option2:
				this.pttType = "PUSH TO TALK";
				PlayerPrefs.SetString("pttType", this.pttType);
				PlayerPrefs.Save();
				return;
			case GorillaKeyboardBindings.option3:
				this.pttType = "PUSH TO MUTE";
				PlayerPrefs.SetString("pttType", this.pttType);
				PlayerPrefs.Save();
				return;
			default:
				return;
			}
		}

		// Token: 0x0600555B RID: 21851 RVA: 0x001A7AD0 File Offset: 0x001A5CD0
		private void ProcessQueueState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.limitOnlineScreens)
			{
				return;
			}
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.option1:
				this.JoinQueue("DEFAULT", false);
				return;
			case GorillaKeyboardBindings.option2:
				this.JoinQueue("MINIGAMES", false);
				return;
			case GorillaKeyboardBindings.option3:
				if (this.allowedInCompetitive)
				{
					this.JoinQueue("COMPETITIVE", false);
				}
				return;
			default:
				return;
			}
		}

		// Token: 0x0600555C RID: 21852 RVA: 0x001A7B2C File Offset: 0x001A5D2C
		public void JoinTroop(string newTroopName)
		{
			if (this.IsValidTroopName(newTroopName))
			{
				this.currentTroopPopulation = -1;
				this.troopName = newTroopName;
				PlayerPrefs.SetString("troopName", this.troopName);
				if (this.troopQueueActive)
				{
					this.currentQueue = this.GetQueueNameForTroop(this.troopName);
					PlayerPrefs.SetString("currentQueue", this.currentQueue);
				}
				PlayerPrefs.Save();
				this.JoinTroopQueue();
			}
		}

		// Token: 0x0600555D RID: 21853 RVA: 0x001A7B95 File Offset: 0x001A5D95
		public void JoinTroopQueue()
		{
			if (this.IsValidTroopName(this.troopName))
			{
				this.currentTroopPopulation = -1;
				this.JoinQueue(this.GetQueueNameForTroop(this.troopName), true);
				this.RequestTroopPopulation(true);
			}
		}

		// Token: 0x0600555E RID: 21854 RVA: 0x001A7BC8 File Offset: 0x001A5DC8
		private void RequestTroopPopulation(bool forceUpdate = false)
		{
			if (!PlayFabCloudScriptAPI.IsEntityLoggedIn())
			{
				return;
			}
			if (!this.hasRequestedInitialTroopPopulation || forceUpdate)
			{
				if (this.nextPopulationCheckTime > Time.time)
				{
					return;
				}
				this.nextPopulationCheckTime = Time.time + this.troopPopulationCheckCooldown;
				this.hasRequestedInitialTroopPopulation = true;
				GorillaServer.Instance.ReturnQueueStats(new ReturnQueueStatsRequest
				{
					queueName = this.troopName
				}, delegate(ExecuteFunctionResult result)
				{
					Debug.Log("Troop pop received");
					object obj;
					if (((JsonObject)result.FunctionResult).TryGetValue("PlayerCount", out obj))
					{
						this.currentTroopPopulation = int.Parse(obj.ToString());
						if (this.currentComputerState == GorillaComputer.ComputerState.Queue)
						{
							this.UpdateScreen();
							return;
						}
					}
					else
					{
						this.currentTroopPopulation = 0;
					}
				}, delegate(PlayFabError error)
				{
					Debug.LogError(string.Format("Error requesting troop population: {0}", error));
					this.currentTroopPopulation = -1;
				});
			}
		}

		// Token: 0x0600555F RID: 21855 RVA: 0x001A7C46 File Offset: 0x001A5E46
		public void JoinDefaultQueue()
		{
			this.JoinQueue("DEFAULT", false);
		}

		// Token: 0x06005560 RID: 21856 RVA: 0x001A7C54 File Offset: 0x001A5E54
		public void LeaveTroop()
		{
			if (this.IsValidTroopName(this.troopName))
			{
				this.troopToJoin = this.troopName;
			}
			this.currentTroopPopulation = -1;
			this.troopName = string.Empty;
			PlayerPrefs.SetString("troopName", this.troopName);
			if (this.troopQueueActive)
			{
				this.JoinDefaultQueue();
			}
			PlayerPrefs.Save();
		}

		// Token: 0x06005561 RID: 21857 RVA: 0x001A7CB0 File Offset: 0x001A5EB0
		public string GetCurrentTroop()
		{
			if (this.troopQueueActive)
			{
				return this.troopName;
			}
			return this.currentQueue;
		}

		// Token: 0x06005562 RID: 21858 RVA: 0x001A7CC7 File Offset: 0x001A5EC7
		public int GetCurrentTroopPopulation()
		{
			if (this.troopQueueActive)
			{
				return this.currentTroopPopulation;
			}
			return -1;
		}

		// Token: 0x06005563 RID: 21859 RVA: 0x001A7CDC File Offset: 0x001A5EDC
		private void JoinQueue(string queueName, bool isTroopQueue = false)
		{
			this.currentQueue = queueName;
			this.troopQueueActive = isTroopQueue;
			this.currentTroopPopulation = -1;
			PlayerPrefs.SetString("currentQueue", this.currentQueue);
			PlayerPrefs.SetInt("troopQueueActive", this.troopQueueActive ? 1 : 0);
			PlayerPrefs.Save();
		}

		// Token: 0x06005564 RID: 21860 RVA: 0x001A7D2C File Offset: 0x001A5F2C
		private void ProcessGroupState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.limitOnlineScreens)
			{
				return;
			}
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.one:
				this.groupMapJoin = "FOREST";
				this.groupMapJoinIndex = 0;
				PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
				PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
				PlayerPrefs.Save();
				break;
			case GorillaKeyboardBindings.two:
				this.groupMapJoin = "CAVE";
				this.groupMapJoinIndex = 1;
				PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
				PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
				PlayerPrefs.Save();
				break;
			case GorillaKeyboardBindings.three:
				this.groupMapJoin = "CANYON";
				this.groupMapJoinIndex = 2;
				PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
				PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
				PlayerPrefs.Save();
				break;
			case GorillaKeyboardBindings.four:
				this.groupMapJoin = "CITY";
				this.groupMapJoinIndex = 3;
				PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
				PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
				PlayerPrefs.Save();
				break;
			case GorillaKeyboardBindings.five:
				this.groupMapJoin = "CLOUDS";
				this.groupMapJoinIndex = 4;
				PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
				PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
				PlayerPrefs.Save();
				break;
			default:
				if (buttonPressed == GorillaKeyboardBindings.enter)
				{
					this.OnGroupJoinButtonPress(Mathf.Min(this.allowedMapsToJoin.Length - 1, this.groupMapJoinIndex), this.friendJoinCollider);
				}
				break;
			}
			this.roomFull = false;
		}

		// Token: 0x06005565 RID: 21861 RVA: 0x001A7EBC File Offset: 0x001A60BC
		private void ProcessTroopState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.limitOnlineScreens)
			{
				return;
			}
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups);
			bool flag2 = this.IsValidTroopName(this.troopName);
			if (flag)
			{
				switch (buttonPressed)
				{
				case GorillaKeyboardBindings.delete:
					if (!flag2 && this.troopToJoin.Length > 0)
					{
						this.troopToJoin = this.troopToJoin.Substring(0, this.troopToJoin.Length - 1);
						return;
					}
					break;
				case GorillaKeyboardBindings.enter:
					if (!flag2)
					{
						this.CheckAutoBanListForTroopName(this.troopToJoin);
						return;
					}
					break;
				case GorillaKeyboardBindings.option1:
					this.JoinTroopQueue();
					return;
				case GorillaKeyboardBindings.option2:
					this.JoinDefaultQueue();
					return;
				case GorillaKeyboardBindings.option3:
					this.LeaveTroop();
					return;
				default:
					if (!flag2 && this.troopToJoin.Length < 12)
					{
						string str = this.troopToJoin;
						string str2;
						if (buttonPressed >= GorillaKeyboardBindings.up)
						{
							str2 = buttonPressed.ToString();
						}
						else
						{
							int num = (int)buttonPressed;
							str2 = num.ToString();
						}
						this.troopToJoin = str + str2;
						return;
					}
					break;
				}
			}
			else
			{
				switch (buttonPressed)
				{
				case GorillaKeyboardBindings.option1:
					break;
				case GorillaKeyboardBindings.option2:
					if (this._currentScreentState != GorillaComputer.EKidScreenState.Ready)
					{
						this.ProcessScreen_SetupKID();
						return;
					}
					this.RequestUpdatedPermissions();
					return;
				case GorillaKeyboardBindings.option3:
					if (this._currentScreentState != GorillaComputer.EKidScreenState.Show_OTP)
					{
						return;
					}
					this.ProcessScreen_SetupKID();
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06005566 RID: 21862 RVA: 0x001A7FE5 File Offset: 0x001A61E5
		private bool IsValidTroopName(string troop)
		{
			return !string.IsNullOrEmpty(troop) && troop.Length <= 12 && (this.allowedInCompetitive || troop != "COMPETITIVE");
		}

		// Token: 0x06005567 RID: 21863 RVA: 0x0008089B File Offset: 0x0007EA9B
		private string GetQueueNameForTroop(string troop)
		{
			return troop;
		}

		// Token: 0x06005568 RID: 21864 RVA: 0x001A8010 File Offset: 0x001A6210
		private void ProcessVoiceState(GorillaKeyboardBindings buttonPressed)
		{
			if (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Voice_Chat))
			{
				if (buttonPressed != GorillaKeyboardBindings.option1)
				{
					if (buttonPressed == GorillaKeyboardBindings.option2)
					{
						this.SetVoice(false, true);
					}
				}
				else
				{
					this.SetVoice(true, true);
				}
			}
			else if (buttonPressed != GorillaKeyboardBindings.option2)
			{
				if (buttonPressed == GorillaKeyboardBindings.option3)
				{
					if (this._currentScreentState != GorillaComputer.EKidScreenState.Show_OTP)
					{
						return;
					}
					this.ProcessScreen_SetupKID();
				}
			}
			else if (this._currentScreentState != GorillaComputer.EKidScreenState.Ready)
			{
				this.ProcessScreen_SetupKID();
			}
			else
			{
				this.RequestUpdatedPermissions();
			}
			RigContainer.RefreshAllRigVoices();
		}

		// Token: 0x06005569 RID: 21865 RVA: 0x001A8080 File Offset: 0x001A6280
		private void ProcessAutoMuteState(GorillaKeyboardBindings buttonPressed)
		{
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.option1:
				this.autoMuteType = "AGGRESSIVE";
				PlayerPrefs.SetInt("autoMute", 2);
				PlayerPrefs.Save();
				RigContainer.RefreshAllRigVoices();
				break;
			case GorillaKeyboardBindings.option2:
				this.autoMuteType = "MODERATE";
				PlayerPrefs.SetInt("autoMute", 1);
				PlayerPrefs.Save();
				RigContainer.RefreshAllRigVoices();
				break;
			case GorillaKeyboardBindings.option3:
				this.autoMuteType = "OFF";
				PlayerPrefs.SetInt("autoMute", 0);
				PlayerPrefs.Save();
				RigContainer.RefreshAllRigVoices();
				break;
			}
			this.UpdateScreen();
		}

		// Token: 0x0600556A RID: 21866 RVA: 0x001A8110 File Offset: 0x001A6310
		private void ProcessVisualsState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed < GorillaKeyboardBindings.up)
			{
				this.instrumentVolume = (float)buttonPressed / 50f;
				PlayerPrefs.SetFloat("instrumentVolume", this.instrumentVolume);
				PlayerPrefs.Save();
				return;
			}
			if (buttonPressed == GorillaKeyboardBindings.option1)
			{
				this.disableParticles = false;
				PlayerPrefs.SetString("disableParticles", "FALSE");
				PlayerPrefs.Save();
				GorillaTagger.Instance.ShowCosmeticParticles(!this.disableParticles);
				return;
			}
			if (buttonPressed != GorillaKeyboardBindings.option2)
			{
				return;
			}
			this.disableParticles = true;
			PlayerPrefs.SetString("disableParticles", "TRUE");
			PlayerPrefs.Save();
			GorillaTagger.Instance.ShowCosmeticParticles(!this.disableParticles);
		}

		// Token: 0x0600556B RID: 21867 RVA: 0x001A81B0 File Offset: 0x001A63B0
		private void ProcessCreditsState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed == GorillaKeyboardBindings.enter)
			{
				this.creditsView.ProcessButtonPress(buttonPressed);
			}
		}

		// Token: 0x0600556C RID: 21868 RVA: 0x001A81C3 File Offset: 0x001A63C3
		private void ProcessSupportState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed == GorillaKeyboardBindings.enter)
			{
				this.displaySupport = true;
			}
		}

		// Token: 0x0600556D RID: 21869 RVA: 0x001A81D4 File Offset: 0x001A63D4
		private void ProcessRedemptionState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.RedemptionStatus == GorillaComputer.RedemptionResult.Checking)
			{
				return;
			}
			if (buttonPressed != GorillaKeyboardBindings.delete)
			{
				if (buttonPressed == GorillaKeyboardBindings.enter)
				{
					if (this.redemptionCode != "")
					{
						if (this.redemptionCode.Length < 8)
						{
							this.RedemptionStatus = GorillaComputer.RedemptionResult.Invalid;
							return;
						}
						CodeRedemption.Instance.HandleCodeRedemption(this.redemptionCode);
						this.RedemptionStatus = GorillaComputer.RedemptionResult.Checking;
						return;
					}
					else if (this.RedemptionStatus != GorillaComputer.RedemptionResult.Success)
					{
						this.RedemptionStatus = GorillaComputer.RedemptionResult.Empty;
						return;
					}
				}
				else if (this.redemptionCode.Length < 8 && (buttonPressed < GorillaKeyboardBindings.up || buttonPressed > GorillaKeyboardBindings.option3))
				{
					string str = this.redemptionCode;
					string str2;
					if (buttonPressed >= GorillaKeyboardBindings.up)
					{
						str2 = buttonPressed.ToString();
					}
					else
					{
						int num = (int)buttonPressed;
						str2 = num.ToString();
					}
					this.redemptionCode = str + str2;
				}
			}
			else if (this.redemptionCode.Length > 0)
			{
				this.redemptionCode = this.redemptionCode.Substring(0, this.redemptionCode.Length - 1);
				return;
			}
		}

		// Token: 0x0600556E RID: 21870 RVA: 0x001A82C0 File Offset: 0x001A64C0
		private void ProcessNameWarningState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.warningConfirmationInputString.ToLower() == "yes")
			{
				this.PopState();
				return;
			}
			if (buttonPressed == GorillaKeyboardBindings.delete)
			{
				if (this.warningConfirmationInputString.Length > 0)
				{
					this.warningConfirmationInputString = this.warningConfirmationInputString.Substring(0, this.warningConfirmationInputString.Length - 1);
					return;
				}
			}
			else if (this.warningConfirmationInputString.Length < 3)
			{
				this.warningConfirmationInputString += buttonPressed.ToString();
			}
		}

		// Token: 0x0600556F RID: 21871 RVA: 0x001A834C File Offset: 0x001A654C
		public void UpdateScreen()
		{
			if (NetworkSystem.Instance != null && !NetworkSystem.Instance.WrongVersion)
			{
				this.UpdateFunctionScreen();
				switch (this.currentState)
				{
				case GorillaComputer.ComputerState.Startup:
					this.StartupScreen();
					break;
				case GorillaComputer.ComputerState.Color:
					this.ColourScreen();
					break;
				case GorillaComputer.ComputerState.Name:
					this.NameScreen();
					break;
				case GorillaComputer.ComputerState.Turn:
					this.TurnScreen();
					break;
				case GorillaComputer.ComputerState.Mic:
					this.MicScreen();
					break;
				case GorillaComputer.ComputerState.Room:
					this.RoomScreen();
					break;
				case GorillaComputer.ComputerState.Queue:
					this.QueueScreen();
					break;
				case GorillaComputer.ComputerState.Group:
					this.GroupScreen();
					break;
				case GorillaComputer.ComputerState.Voice:
					this.VoiceScreen();
					break;
				case GorillaComputer.ComputerState.AutoMute:
					this.AutomuteScreen();
					break;
				case GorillaComputer.ComputerState.Credits:
					this.CreditsScreen();
					break;
				case GorillaComputer.ComputerState.Visuals:
					this.VisualsScreen();
					break;
				case GorillaComputer.ComputerState.Time:
					this.TimeScreen();
					break;
				case GorillaComputer.ComputerState.NameWarning:
					this.NameWarningScreen();
					break;
				case GorillaComputer.ComputerState.Loading:
					this.LoadingScreen();
					break;
				case GorillaComputer.ComputerState.Support:
					this.SupportScreen();
					break;
				case GorillaComputer.ComputerState.Troop:
					this.TroopScreen();
					break;
				case GorillaComputer.ComputerState.KID:
					this.KIdScreen();
					break;
				case GorillaComputer.ComputerState.Redemption:
					this.RedemptionScreen();
					break;
				}
			}
			this.UpdateGameModeText();
		}

		// Token: 0x06005570 RID: 21872 RVA: 0x001A847E File Offset: 0x001A667E
		private void LoadingScreen()
		{
			this.screenText.Text = "LOADING";
			this.LoadingRoutine = base.StartCoroutine(this.<LoadingScreen>g__LoadingScreenLocal|195_0());
		}

		// Token: 0x06005571 RID: 21873 RVA: 0x001A84A4 File Offset: 0x001A66A4
		private void NameWarningScreen()
		{
			this.screenText.Text = "<color=red>WARNING: PLEASE CHOOSE A BETTER NAME\n\nENTERING ANOTHER BAD NAME WILL RESULT IN A BAN</color>";
			if (this.warningConfirmationInputString.ToLower() == "yes")
			{
				GorillaText gorillaText = this.screenText;
				gorillaText.Text += "\n\nPRESS ANY KEY TO CONTINUE";
				return;
			}
			GorillaText gorillaText2 = this.screenText;
			gorillaText2.Text = gorillaText2.Text + "\n\nTYPE 'YES' TO CONFIRM: " + this.warningConfirmationInputString;
		}

		// Token: 0x06005572 RID: 21874 RVA: 0x001A8518 File Offset: 0x001A6718
		private void SupportScreen()
		{
			if (this.displaySupport)
			{
				string text = PlayFabAuthenticator.instance.platform.ToString().ToUpper();
				string text2;
				if (text == "PC")
				{
					text2 = "OCULUS PC";
				}
				else
				{
					text2 = text;
				}
				text = text2;
				this.screenText.Text = string.Concat(new string[]
				{
					"SUPPORT\n\nPLAYERID   ",
					PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
					"\nVERSION    ",
					this.version.ToUpper(),
					"\nPLATFORM   ",
					text,
					"\nBUILD DATE ",
					this.buildDate,
					"\n"
				});
				if (KIDManager.KidEnabled)
				{
					GorillaText gorillaText = this.screenText;
					gorillaText.Text = gorillaText.Text + "\nk-ID ACCOUNT TYPE: " + KIDManager.GetActiveAccountStatusNiceString().ToUpper();
					return;
				}
			}
			else
			{
				this.screenText.Text = "SUPPORT\n\n";
				GorillaText gorillaText2 = this.screenText;
				gorillaText2.Text += "PRESS ENTER TO DISPLAY SUPPORT AND ACCOUNT INFORMATION\n\n\n\n";
				GorillaText gorillaText3 = this.screenText;
				gorillaText3.Text += "<color=red>DO NOT SHARE ACCOUNT INFORMATION WITH ANYONE OTHER ";
				GorillaText gorillaText4 = this.screenText;
				gorillaText4.Text += "THAN ANOTHER AXIOM SUPPORT</color>";
			}
		}

		// Token: 0x06005573 RID: 21875 RVA: 0x001A865C File Offset: 0x001A685C
		private void TimeScreen()
		{
			this.screenText.Text = string.Concat(new string[]
			{
				"UPDATE TIME SETTINGS. (LOCALLY ONLY). \nPRESS OPTION 1 FOR NORMAL MODE. \nPRESS OPTION 2 FOR STATIC MODE. \nPRESS 1-10 TO CHANGE TIME OF DAY. \nCURRENT MODE: ",
				BetterDayNightManager.instance.currentSetting.ToString().ToUpper(),
				". \nTIME OF DAY: ",
				BetterDayNightManager.instance.currentTimeOfDay.ToUpper(),
				". \n"
			});
		}

		// Token: 0x06005574 RID: 21876 RVA: 0x001A86CA File Offset: 0x001A68CA
		private void CreditsScreen()
		{
			this.screenText.Text = this.creditsView.GetScreenText();
		}

		// Token: 0x06005575 RID: 21877 RVA: 0x001A86E4 File Offset: 0x001A68E4
		private void VisualsScreen()
		{
			this.screenText.Text = "UPDATE ITEMS SETTINGS. PRESS OPTION 1 TO ENABLE ITEM PARTICLES. PRESS OPTION 2 TO DISABLE ITEM PARTICLES. PRESS 1-10 TO CHANGE INSTRUMENT VOLUME FOR OTHER PLAYERS.\n\nITEM PARTICLES ON: " + (this.disableParticles ? "FALSE" : "TRUE") + "\nINSTRUMENT VOLUME: " + Mathf.CeilToInt(this.instrumentVolume * 50f).ToString();
		}

		// Token: 0x06005576 RID: 21878 RVA: 0x001A8738 File Offset: 0x001A6938
		private void VoiceScreen()
		{
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat);
			if (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Voice_Chat))
			{
				this.screenText.Text = "CHOOSE WHICH TYPE OF VOICE YOU WANT TO HEAR AND SPEAK. \nPRESS OPTION 1 = HUMAN VOICES. \nPRESS OPTION 2 = MONKE VOICES. \n\nVOICE TYPE: " + ((this.voiceChatOn == "TRUE") ? "HUMAN" : ((this.voiceChatOn == "FALSE") ? "MONKE" : "OFF"));
				return;
			}
			if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				this.VoiceScreen_KIdProhibited();
				return;
			}
			this.VoiceScreen_Permission();
		}

		// Token: 0x06005577 RID: 21879 RVA: 0x001A87B7 File Offset: 0x001A69B7
		private void AutomuteScreen()
		{
			this.screenText.Text = "AUTOMOD AUTOMATICALLY MUTES PLAYERS WHEN THEY JOIN YOUR ROOM IF A LOT OF OTHER PLAYERS HAVE MUTED THEM\nPRESS OPTION 1 FOR AGGRESSIVE MUTING\nPRESS OPTION 2 FOR MODERATE MUTING\nPRESS OPTION 3 TO TURN AUTOMOD OFF\n\nCURRENT AUTOMOD LEVEL: " + this.autoMuteType;
		}

		// Token: 0x06005578 RID: 21880 RVA: 0x001A87D4 File Offset: 0x001A69D4
		private void GroupScreen()
		{
			if (this.limitOnlineScreens)
			{
				this.LimitedOnlineFunctionalityScreen();
				return;
			}
			string str = (this.allowedMapsToJoin.Length > 1) ? this.groupMapJoin : this.allowedMapsToJoin[0].ToUpper();
			string str2 = (this.allowedMapsToJoin.Length > 1) ? "\n\nUSE NUMBER KEYS TO SELECT DESTINATION\n1: FOREST, 2: CAVE, 3: CANYON, 4: CITY, 5: CLOUDS." : "";
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				string str3 = this.GetSelectedMapJoinTrigger().CanPartyJoin() ? "" : "\n\n<color=red>CANNOT JOIN BECAUSE YOUR GROUP IS NOT HERE</color>";
				this.screenText.Text = "PRESS ENTER TO JOIN A PUBLIC GAME WITH YOUR FRIENDSHIP GROUP.\n\nACTIVE ZONE WILL BE: " + str + str2 + str3;
				return;
			}
			this.screenText.Text = "PRESS ENTER TO JOIN A PUBLIC GAME AND BRING EVERYONE IN THIS ROOM WITH YOU.\n\nACTIVE ZONE WILL BE: " + str + str2;
		}

		// Token: 0x06005579 RID: 21881 RVA: 0x001A887F File Offset: 0x001A6A7F
		private void MicScreen()
		{
			if (KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat).ManagedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				this.MicScreen_KIdProhibited();
				return;
			}
			this.screenText.Text = "PRESS OPTION 1 = ALL CHAT.\nPRESS OPTION 2 = PUSH TO TALK.\nPRESS OPTION 3 = PUSH TO MUTE.\n\nCURRENT MIC SETTING: " + this.pttType + "\n\nPUSH TO TALK AND PUSH TO MUTE WORK WITH ANY FACE BUTTON";
		}

		// Token: 0x0600557A RID: 21882 RVA: 0x001A88B8 File Offset: 0x001A6AB8
		private void QueueScreen()
		{
			if (this.limitOnlineScreens)
			{
				this.LimitedOnlineFunctionalityScreen();
				return;
			}
			if (this.allowedInCompetitive)
			{
				this.screenText.Text = "THIS OPTION AFFECTS WHO YOU PLAY WITH. DEFAULT IS FOR ANYONE TO PLAY NORMALLY. MINIGAMES IS FOR PEOPLE LOOKING TO PLAY WITH THEIR OWN MADE UP RULES. COMPETITIVE IS FOR PLAYERS WHO WANT TO PLAY THE GAME AND TRY AS HARD AS THEY CAN. PRESS OPTION 1 FOR DEFAULT, OPTION 2 FOR MINIGAMES, OR OPTION 3 FOR COMPETITIVE.";
			}
			else
			{
				this.screenText.Text = "THIS OPTION AFFECTS WHO YOU PLAY WITH. DEFAULT IS FOR ANYONE TO PLAY NORMALLY. MINIGAMES IS FOR PEOPLE LOOKING TO PLAY WITH THEIR OWN MADE UP RULES. BEAT THE OBSTACLE COURSE IN CITY TO ALLOW COMPETITIVE PLAY. PRESS OPTION 1 FOR DEFAULT, OR OPTION 2 FOR MINIGAMES.";
			}
			GorillaText gorillaText = this.screenText;
			gorillaText.Text = gorillaText.Text + "\n\nCURRENT QUEUE: " + this.currentQueue;
		}

		// Token: 0x0600557B RID: 21883 RVA: 0x001A8920 File Offset: 0x001A6B20
		private void TroopScreen()
		{
			if (this.limitOnlineScreens)
			{
				this.LimitedOnlineFunctionalityScreen();
				return;
			}
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Groups);
			Permission permissionDataByFeature2 = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Multiplayer);
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups) && KIDManager.HasPermissionToUseFeature(EKIDFeatures.Multiplayer);
			bool flag2 = this.IsValidTroopName(this.troopName);
			this.screenText.Text = string.Empty;
			if (flag)
			{
				this.screenText.Text = "PLAY WITH A PERSISTENT GROUP ACROSS MULTIPLE ROOMS.";
				if (!flag2)
				{
					GorillaText gorillaText = this.screenText;
					gorillaText.Text += " PRESS ENTER TO JOIN OR CREATE A TROOP.";
				}
			}
			GorillaText gorillaText2 = this.screenText;
			gorillaText2.Text += "\n\nCURRENT TROOP: ";
			if (flag2)
			{
				GorillaText gorillaText3 = this.screenText;
				gorillaText3.Text = gorillaText3.Text + this.troopName + "\n";
				if (flag)
				{
					bool flag3 = this.currentTroopPopulation > -1;
					if (this.troopQueueActive)
					{
						GorillaText gorillaText4 = this.screenText;
						gorillaText4.Text += "  -IN TROOP QUEUE-\n";
						if (flag3)
						{
							GorillaText gorillaText5 = this.screenText;
							gorillaText5.Text += string.Format("\nPLAYERS IN TROOP: {0}\n", Mathf.Max(1, this.currentTroopPopulation));
						}
						GorillaText gorillaText6 = this.screenText;
						gorillaText6.Text += "\nPRESS OPTION 2 FOR DEFAULT QUEUE.\n";
					}
					else
					{
						GorillaText gorillaText7 = this.screenText;
						gorillaText7.Text = gorillaText7.Text + "  -IN " + this.currentQueue + " QUEUE-\n";
						if (flag3)
						{
							GorillaText gorillaText8 = this.screenText;
							gorillaText8.Text += string.Format("\nPLAYERS IN TROOP: {0}\n", Mathf.Max(1, this.currentTroopPopulation));
						}
						GorillaText gorillaText9 = this.screenText;
						gorillaText9.Text += "\nPRESS OPTION 1 FOR TROOP QUEUE.\n";
					}
					GorillaText gorillaText10 = this.screenText;
					gorillaText10.Text += "PRESS OPTION 3 TO LEAVE YOUR TROOP.";
				}
			}
			else
			{
				GorillaText gorillaText11 = this.screenText;
				gorillaText11.Text += "-NOT IN TROOP-";
			}
			if (flag)
			{
				if (!flag2)
				{
					GorillaText gorillaText12 = this.screenText;
					gorillaText12.Text = gorillaText12.Text + "\n\nTROOP TO JOIN: " + this.troopToJoin;
					return;
				}
			}
			else
			{
				if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED || permissionDataByFeature2.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
				{
					this.TroopScreen_KIdProhibited();
					return;
				}
				this.TroopScreen_Permission();
			}
		}

		// Token: 0x0600557C RID: 21884 RVA: 0x001A8B68 File Offset: 0x001A6D68
		private void TurnScreen()
		{
			this.screenText.Text = "PRESS OPTION 1 TO USE SNAP TURN. PRESS OPTION 2 TO USE SMOOTH TURN. PRESS OPTION 3 TO USE NO ARTIFICIAL TURNING. PRESS THE NUMBER KEYS TO CHOOSE A TURNING SPEED.\n CURRENT TURN TYPE: " + GorillaSnapTurn.CachedSnapTurnRef.turnType + "\nCURRENT TURN SPEED: " + GorillaSnapTurn.CachedSnapTurnRef.turnFactor.ToString();
		}

		// Token: 0x0600557D RID: 21885 RVA: 0x001A8BAC File Offset: 0x001A6DAC
		private void NameScreen()
		{
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Custom_Nametags);
			if (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags))
			{
				this.screenText.Text = "PRESS ENTER TO CHANGE YOUR NAME TO THE ENTERED NEW NAME.\n\nCURRENT NAME: " + this.savedName;
				if (this.NametagsEnabled)
				{
					GorillaText gorillaText = this.screenText;
					gorillaText.Text = gorillaText.Text + "\n\n    NEW NAME: " + this.currentName;
				}
				GorillaText gorillaText2 = this.screenText;
				gorillaText2.Text = gorillaText2.Text + "\n\nPRESS OPTION 1 TO TOGGLE NAMETAGS.\nCURRENTLY NAMETAGS ARE: " + (this.NametagsEnabled ? "ON" : "OFF");
				return;
			}
			if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				this.NameScreen_KIdProhibited();
				return;
			}
			this.NameScreen_Permission();
		}

		// Token: 0x0600557E RID: 21886 RVA: 0x001A8C54 File Offset: 0x001A6E54
		private void StartupScreen()
		{
			string text = string.Empty;
			if (KIDManager.GetActiveAccountStatus() == AgeStatusType.DIGITALMINOR)
			{
				text = "YOU ARE PLAYING ON A MANAGED ACCOUNT. SOME SETTINGS MAY BE DISABLED WITHOUT PARENT OR GUARDIAN APPROVAL\n\n";
			}
			string empty = string.Empty;
			this.screenText.Text = string.Concat(new string[]
			{
				"GORILLA OS\n\n",
				text,
				NetworkSystem.Instance.GlobalPlayerCount().ToString(),
				" PLAYERS ONLINE\n\n",
				this.usersBanned.ToString(),
				" USERS BANNED YESTERDAY\n\n",
				empty,
				"PRESS ANY KEY TO BEGIN"
			});
		}

		// Token: 0x0600557F RID: 21887 RVA: 0x001A8CDC File Offset: 0x001A6EDC
		private void ColourScreen()
		{
			this.screenText.Text = "USE THE OPTIONS BUTTONS TO SELECT THE COLOR TO UPDATE, THEN PRESS 0-9 TO SET A NEW VALUE.";
			GorillaText gorillaText = this.screenText;
			gorillaText.Text = gorillaText.Text + "\n\n  RED: " + Mathf.FloorToInt(this.redValue * 9f).ToString() + ((this.colorCursorLine == 0) ? "<--" : "");
			GorillaText gorillaText2 = this.screenText;
			gorillaText2.Text = gorillaText2.Text + "\n\nGREEN: " + Mathf.FloorToInt(this.greenValue * 9f).ToString() + ((this.colorCursorLine == 1) ? "<--" : "");
			GorillaText gorillaText3 = this.screenText;
			gorillaText3.Text = gorillaText3.Text + "\n\n BLUE: " + Mathf.FloorToInt(this.blueValue * 9f).ToString() + ((this.colorCursorLine == 2) ? "<--" : "");
		}

		// Token: 0x06005580 RID: 21888 RVA: 0x001A8DD4 File Offset: 0x001A6FD4
		private void RoomScreen()
		{
			if (this.limitOnlineScreens)
			{
				this.LimitedOnlineFunctionalityScreen();
				return;
			}
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Groups);
			Permission permissionDataByFeature2 = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Multiplayer);
			object obj = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups) && KIDManager.HasPermissionToUseFeature(EKIDFeatures.Multiplayer);
			this.screenText.Text = "";
			object obj2 = obj;
			if (obj2 != null)
			{
				GorillaText gorillaText = this.screenText;
				gorillaText.Text += "PRESS ENTER TO JOIN OR CREATE A CUSTOM ROOM WITH THE ENTERED CODE. ";
			}
			GorillaText gorillaText2 = this.screenText;
			gorillaText2.Text += "PRESS OPTION 1 TO DISCONNECT FROM THE CURRENT ROOM. ";
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				if (FriendshipGroupDetection.Instance.IsPartyWithinCollider(this.friendJoinCollider))
				{
					GorillaText gorillaText3 = this.screenText;
					gorillaText3.Text += "YOUR GROUP WILL TRAVEL WITH YOU. ";
				}
				else
				{
					GorillaText gorillaText4 = this.screenText;
					gorillaText4.Text += "<color=red>YOU WILL LEAVE YOUR PARTY UNLESS YOU GATHER THEM HERE FIRST!</color> ";
				}
			}
			GorillaText gorillaText5 = this.screenText;
			gorillaText5.Text += "\n\nCURRENT ROOM: ";
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaText gorillaText6 = this.screenText;
				gorillaText6.Text += NetworkSystem.Instance.RoomName;
				if (NetworkSystem.Instance.SessionIsPrivate)
				{
					GorillaGameManager activeGameMode = GameMode.ActiveGameMode;
					string text = (activeGameMode != null) ? activeGameMode.GameModeName() : null;
					if (text != null && text != this.currentGameMode.Value)
					{
						GorillaText gorillaText7 = this.screenText;
						gorillaText7.Text = gorillaText7.Text + " (" + text + " GAME)";
					}
				}
				GorillaText gorillaText8 = this.screenText;
				gorillaText8.Text = gorillaText8.Text + "\n\nPLAYERS IN ROOM: " + NetworkSystem.Instance.RoomPlayerCount.ToString();
			}
			else
			{
				GorillaText gorillaText9 = this.screenText;
				gorillaText9.Text += "-NOT IN ROOM-";
				GorillaText gorillaText10 = this.screenText;
				gorillaText10.Text = gorillaText10.Text + "\n\nPLAYERS ONLINE: " + NetworkSystem.Instance.GlobalPlayerCount().ToString();
			}
			if (obj2 != null)
			{
				GorillaText gorillaText11 = this.screenText;
				gorillaText11.Text = gorillaText11.Text + "\n\nROOM TO JOIN: " + this.roomToJoin;
				if (this.roomFull)
				{
					GorillaText gorillaText12 = this.screenText;
					gorillaText12.Text += "\n\nROOM FULL. JOIN ROOM FAILED.";
					return;
				}
				if (this.roomNotAllowed)
				{
					GorillaText gorillaText13 = this.screenText;
					gorillaText13.Text += "\n\nCANNOT JOIN ROOM TYPE FROM HERE.";
					return;
				}
			}
			else
			{
				if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED || permissionDataByFeature2.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
				{
					this.RoomScreen_KIdProhibited();
					return;
				}
				this.RoomScreen_Permission();
			}
		}

		// Token: 0x06005581 RID: 21889 RVA: 0x001A9050 File Offset: 0x001A7250
		private void RedemptionScreen()
		{
			this.screenText.Text = "TYPE REDEMPTION CODE AND PRESS ENTER";
			GorillaText gorillaText = this.screenText;
			gorillaText.Text = gorillaText.Text + "\n\nCODE: " + this.redemptionCode;
			switch (this.RedemptionStatus)
			{
			case GorillaComputer.RedemptionResult.Empty:
				break;
			case GorillaComputer.RedemptionResult.Invalid:
			{
				GorillaText gorillaText2 = this.screenText;
				gorillaText2.Text += "\n\nINVALID CODE";
				return;
			}
			case GorillaComputer.RedemptionResult.Checking:
			{
				GorillaText gorillaText3 = this.screenText;
				gorillaText3.Text += "\n\nVALIDATING...";
				return;
			}
			case GorillaComputer.RedemptionResult.AlreadyUsed:
			{
				GorillaText gorillaText4 = this.screenText;
				gorillaText4.Text += "\n\nCODE ALREADY CLAIMED";
				return;
			}
			case GorillaComputer.RedemptionResult.Success:
			{
				GorillaText gorillaText5 = this.screenText;
				gorillaText5.Text += "\n\nSUCCESSFULLY CLAIMED!";
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x06005582 RID: 21890 RVA: 0x001A911F File Offset: 0x001A731F
		private void LimitedOnlineFunctionalityScreen()
		{
			this.screenText.Text = "NOT AVAILABLE IN RANKED PLAY";
		}

		// Token: 0x06005583 RID: 21891 RVA: 0x001A9134 File Offset: 0x001A7334
		private void UpdateGameModeText()
		{
			if (NetworkSystem.Instance.InRoom)
			{
				if (GorillaGameManager.instance != null)
				{
					this.currentGameModeText.Value = "CURRENT MODE\n" + GorillaGameManager.instance.GameModeName();
					return;
				}
				this.currentGameModeText.Value = "CURRENT MODE\n-NOT IN ROOM-";
			}
		}

		// Token: 0x06005584 RID: 21892 RVA: 0x001A918A File Offset: 0x001A738A
		private void UpdateFunctionScreen()
		{
			this.functionSelectText.Text = this.GetOrderListForScreen(this.currentState);
		}

		// Token: 0x06005585 RID: 21893 RVA: 0x001A91A3 File Offset: 0x001A73A3
		private void CheckAutoBanListForRoomName(string nameToCheck)
		{
			this.SwitchToLoadingState();
			this.AutoBanPlayfabFunction(nameToCheck, true, new Action<ExecuteFunctionResult>(this.OnRoomNameChecked));
		}

		// Token: 0x06005586 RID: 21894 RVA: 0x001A91BF File Offset: 0x001A73BF
		private void CheckAutoBanListForPlayerName(string nameToCheck)
		{
			this.SwitchToLoadingState();
			this.AutoBanPlayfabFunction(nameToCheck, false, new Action<ExecuteFunctionResult>(this.OnPlayerNameChecked));
		}

		// Token: 0x06005587 RID: 21895 RVA: 0x001A91DB File Offset: 0x001A73DB
		private void CheckAutoBanListForTroopName(string nameToCheck)
		{
			if (this.IsValidTroopName(this.troopToJoin))
			{
				this.SwitchToLoadingState();
				this.AutoBanPlayfabFunction(nameToCheck, false, new Action<ExecuteFunctionResult>(this.OnTroopNameChecked));
			}
		}

		// Token: 0x06005588 RID: 21896 RVA: 0x001A9205 File Offset: 0x001A7405
		private void AutoBanPlayfabFunction(string nameToCheck, bool forRoom, Action<ExecuteFunctionResult> resultCallback)
		{
			GorillaServer.Instance.CheckForBadName(new CheckForBadNameRequest
			{
				name = nameToCheck,
				forRoom = forRoom
			}, resultCallback, new Action<PlayFabError>(this.OnErrorNameCheck));
		}

		// Token: 0x06005589 RID: 21897 RVA: 0x001A9234 File Offset: 0x001A7434
		private void OnRoomNameChecked(ExecuteFunctionResult result)
		{
			object obj;
			if (((JsonObject)result.FunctionResult).TryGetValue("result", out obj))
			{
				switch (int.Parse(obj.ToString()))
				{
				case 0:
					if (FriendshipGroupDetection.Instance.IsInParty && !FriendshipGroupDetection.Instance.IsPartyWithinCollider(this.friendJoinCollider))
					{
						FriendshipGroupDetection.Instance.LeaveParty();
					}
					if (this.playerInVirtualStump)
					{
						CustomMapManager.UnloadMod(false);
					}
					this.networkController.AttemptToJoinSpecificRoom(this.roomToJoin, FriendshipGroupDetection.Instance.IsInParty ? JoinType.JoinWithParty : JoinType.Solo);
					break;
				case 1:
					this.roomToJoin = "";
					this.roomToJoin += (this.playerInVirtualStump ? this.virtualStumpRoomPrepend : "");
					this.SwitchToWarningState();
					break;
				case 2:
					this.roomToJoin = "";
					this.roomToJoin += (this.playerInVirtualStump ? this.virtualStumpRoomPrepend : "");
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
					break;
				}
			}
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
		}

		// Token: 0x0600558A RID: 21898 RVA: 0x001A935C File Offset: 0x001A755C
		private void OnPlayerNameChecked(ExecuteFunctionResult result)
		{
			object obj;
			if (((JsonObject)result.FunctionResult).TryGetValue("result", out obj))
			{
				switch (int.Parse(obj.ToString()))
				{
				case 0:
					NetworkSystem.Instance.SetMyNickName(this.currentName);
					CustomMapsTerminal.RequestDriverNickNameRefresh();
					break;
				case 1:
					NetworkSystem.Instance.SetMyNickName("gorilla");
					CustomMapsTerminal.RequestDriverNickNameRefresh();
					this.currentName = "gorilla";
					this.SwitchToWarningState();
					break;
				case 2:
					NetworkSystem.Instance.SetMyNickName("gorilla");
					CustomMapsTerminal.RequestDriverNickNameRefresh();
					this.currentName = "gorilla";
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
					break;
				}
			}
			this.SetLocalNameTagText(this.currentName);
			this.savedName = this.currentName;
			PlayerPrefs.SetString("playerName", this.currentName);
			PlayerPrefs.Save();
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[]
				{
					this.redValue,
					this.greenValue,
					this.blueValue
				});
			}
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
		}

		// Token: 0x0600558B RID: 21899 RVA: 0x001A9498 File Offset: 0x001A7698
		private void OnTroopNameChecked(ExecuteFunctionResult result)
		{
			object obj;
			if (((JsonObject)result.FunctionResult).TryGetValue("result", out obj))
			{
				switch (int.Parse(obj.ToString()))
				{
				case 0:
					this.JoinTroop(this.troopToJoin);
					break;
				case 1:
					this.troopToJoin = string.Empty;
					this.SwitchToWarningState();
					break;
				case 2:
					this.troopToJoin = string.Empty;
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
					break;
				}
			}
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
		}

		// Token: 0x0600558C RID: 21900 RVA: 0x001A951F File Offset: 0x001A771F
		private void OnErrorNameCheck(PlayFabError error)
		{
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
			GorillaComputer.OnErrorShared(error);
		}

		// Token: 0x0600558D RID: 21901 RVA: 0x001A9538 File Offset: 0x001A7738
		public bool CheckAutoBanListForName(string nameToCheck)
		{
			nameToCheck = nameToCheck.ToLower();
			nameToCheck = new string(Array.FindAll<char>(nameToCheck.ToCharArray(), (char c) => char.IsLetterOrDigit(c)));
			foreach (string value in this.anywhereTwoWeek)
			{
				if (nameToCheck.IndexOf(value) >= 0)
				{
					return false;
				}
			}
			foreach (string value2 in this.anywhereOneWeek)
			{
				if (nameToCheck.IndexOf(value2) >= 0 && !nameToCheck.Contains("fagol"))
				{
					return false;
				}
			}
			string[] array = this.exactOneWeek;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == nameToCheck)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600558E RID: 21902 RVA: 0x001A95F8 File Offset: 0x001A77F8
		public void UpdateColor(float red, float green, float blue)
		{
			this.redValue = Mathf.Clamp(red, 0f, 1f);
			this.greenValue = Mathf.Clamp(green, 0f, 1f);
			this.blueValue = Mathf.Clamp(blue, 0f, 1f);
		}

		// Token: 0x0600558F RID: 21903 RVA: 0x001A9647 File Offset: 0x001A7847
		public void UpdateFailureText(string failMessage)
		{
			GorillaScoreboardTotalUpdater.instance.SetOfflineFailureText(failMessage);
			PhotonNetworkController.Instance.UpdateTriggerScreens();
			this.screenText.EnableFailedState(failMessage);
			this.functionSelectText.EnableFailedState(failMessage);
		}

		// Token: 0x06005590 RID: 21904 RVA: 0x001A9678 File Offset: 0x001A7878
		private void RestoreFromFailureState()
		{
			GorillaScoreboardTotalUpdater.instance.ClearOfflineFailureText();
			PhotonNetworkController.Instance.UpdateTriggerScreens();
			this.screenText.DisableFailedState();
			this.functionSelectText.DisableFailedState();
		}

		// Token: 0x06005591 RID: 21905 RVA: 0x001A96A6 File Offset: 0x001A78A6
		public void GeneralFailureMessage(string failMessage)
		{
			this.isConnectedToMaster = false;
			NetworkSystem.Instance.SetWrongVersion();
			this.UpdateFailureText(failMessage);
			this.UpdateScreen();
		}

		// Token: 0x06005592 RID: 21906 RVA: 0x001A96C8 File Offset: 0x001A78C8
		private static void OnErrorShared(PlayFabError error)
		{
			if (error.Error == PlayFabErrorCode.NotAuthenticated)
			{
				PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
			}
			else if (error.Error == PlayFabErrorCode.AccountBanned)
			{
				GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
			}
			if (error.ErrorMessage == "The account making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator = error.ErrorDetails.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						return;
					}
					KeyValuePair<string, List<string>> keyValuePair = enumerator.Current;
					if (keyValuePair.Value[0] != "Indefinite")
					{
						GorillaComputer.instance.GeneralFailureMessage(string.Concat(new string[]
						{
							"YOUR ACCOUNT ",
							PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
							" HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: ",
							keyValuePair.Key,
							"\nHOURS LEFT: ",
							((int)((DateTime.Parse(keyValuePair.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString()
						}));
						return;
					}
					GorillaComputer.instance.GeneralFailureMessage("YOUR ACCOUNT " + PlayFabAuthenticator.instance.GetPlayFabPlayerId() + " HAS BEEN BANNED INDEFINITELY.\nREASON: " + keyValuePair.Key);
					return;
				}
			}
			if (error.ErrorMessage == "The IP making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator = error.ErrorDetails.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						KeyValuePair<string, List<string>> keyValuePair2 = enumerator.Current;
						if (keyValuePair2.Value[0] != "Indefinite")
						{
							GorillaComputer.instance.GeneralFailureMessage("THIS IP HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + keyValuePair2.Key + "\nHOURS LEFT: " + ((int)((DateTime.Parse(keyValuePair2.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString());
						}
						else
						{
							GorillaComputer.instance.GeneralFailureMessage("THIS IP HAS BEEN BANNED INDEFINITELY.\nREASON: " + keyValuePair2.Key);
						}
					}
				}
			}
		}

		// Token: 0x06005593 RID: 21907 RVA: 0x001A9920 File Offset: 0x001A7B20
		private void DecreaseState()
		{
			this.currentStateIndex--;
			if (this.GetState(this.currentStateIndex) == GorillaComputer.ComputerState.Time)
			{
				this.currentStateIndex--;
			}
			if (this.currentStateIndex < 0)
			{
				this.currentStateIndex = this.FunctionsCount - 1;
			}
			this.SwitchState(this.GetState(this.currentStateIndex), true);
		}

		// Token: 0x06005594 RID: 21908 RVA: 0x001A9984 File Offset: 0x001A7B84
		private void IncreaseState()
		{
			this.currentStateIndex++;
			if (this.GetState(this.currentStateIndex) == GorillaComputer.ComputerState.Time)
			{
				this.currentStateIndex++;
			}
			if (this.currentStateIndex >= this.FunctionsCount)
			{
				this.currentStateIndex = 0;
			}
			this.SwitchState(this.GetState(this.currentStateIndex), true);
		}

		// Token: 0x06005595 RID: 21909 RVA: 0x001A99E8 File Offset: 0x001A7BE8
		public GorillaComputer.ComputerState GetState(int index)
		{
			GorillaComputer.ComputerState state;
			try
			{
				state = this._activeOrderList[index].State;
			}
			catch
			{
				state = this._activeOrderList[0].State;
			}
			return state;
		}

		// Token: 0x06005596 RID: 21910 RVA: 0x001A9A30 File Offset: 0x001A7C30
		public int GetStateIndex(GorillaComputer.ComputerState state)
		{
			return this._activeOrderList.FindIndex((GorillaComputer.StateOrderItem s) => s.State == state);
		}

		// Token: 0x06005597 RID: 21911 RVA: 0x001A9A64 File Offset: 0x001A7C64
		public string GetOrderListForScreen(GorillaComputer.ComputerState currentState)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int stateIndex = this.GetStateIndex(currentState);
			for (int i = 0; i < this.FunctionsCount; i++)
			{
				stringBuilder.Append(this.FunctionNames[i]);
				if (i == stateIndex)
				{
					stringBuilder.Append(this.Pointer);
				}
				if (i < this.FunctionsCount - 1)
				{
					stringBuilder.Append("\n");
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06005598 RID: 21912 RVA: 0x001A9AD1 File Offset: 0x001A7CD1
		private void GetCurrentTime()
		{
			this.tryGetTimeAgain = true;
			PlayFabClientAPI.GetTime(new GetTimeRequest(), new Action<GetTimeResult>(this.OnGetTimeSuccess), new Action<PlayFabError>(this.OnGetTimeFailure), null, null);
		}

		// Token: 0x06005599 RID: 21913 RVA: 0x001A9B00 File Offset: 0x001A7D00
		private void OnGetTimeSuccess(GetTimeResult result)
		{
			this.startupMillis = (long)(TimeSpan.FromTicks(result.Time.Ticks).TotalMilliseconds - (double)(Time.realtimeSinceStartup * 1000f));
			this.startupTime = result.Time - TimeSpan.FromSeconds((double)Time.realtimeSinceStartup);
			Action onServerTimeUpdated = this.OnServerTimeUpdated;
			if (onServerTimeUpdated == null)
			{
				return;
			}
			onServerTimeUpdated();
		}

		// Token: 0x0600559A RID: 21914 RVA: 0x001A9B68 File Offset: 0x001A7D68
		private void OnGetTimeFailure(PlayFabError error)
		{
			this.startupMillis = (long)(TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalMilliseconds - (double)(Time.realtimeSinceStartup * 1000f));
			this.startupTime = DateTime.UtcNow - TimeSpan.FromSeconds((double)Time.realtimeSinceStartup);
			Action onServerTimeUpdated = this.OnServerTimeUpdated;
			if (onServerTimeUpdated != null)
			{
				onServerTimeUpdated();
			}
			if (error.Error == PlayFabErrorCode.NotAuthenticated)
			{
				PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
				return;
			}
			if (error.Error == PlayFabErrorCode.AccountBanned)
			{
				GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
			}
		}

		// Token: 0x0600559B RID: 21915 RVA: 0x001A9BFB File Offset: 0x001A7DFB
		private void PlayerCountChangedCallback(NetPlayer player)
		{
			this.UpdateScreen();
		}

		// Token: 0x0600559C RID: 21916 RVA: 0x001A9C04 File Offset: 0x001A7E04
		public void SetNameBySafety(bool isSafety)
		{
			if (!isSafety)
			{
				return;
			}
			PlayerPrefs.SetString("playerNameBackup", this.currentName);
			this.currentName = "gorilla" + Random.Range(0, 9999).ToString().PadLeft(4, '0');
			this.savedName = this.currentName;
			NetworkSystem.Instance.SetMyNickName(this.currentName);
			this.SetLocalNameTagText(this.currentName);
			PlayerPrefs.SetString("playerName", this.currentName);
			PlayerPrefs.Save();
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[]
				{
					this.redValue,
					this.greenValue,
					this.blueValue
				});
			}
		}

		// Token: 0x0600559D RID: 21917 RVA: 0x001A9CDE File Offset: 0x001A7EDE
		public void SetLocalNameTagText(string newName)
		{
			VRRig.LocalRig.SetNameTagText(newName);
		}

		// Token: 0x0600559E RID: 21918 RVA: 0x001A9CEC File Offset: 0x001A7EEC
		public void SetComputerSettingsBySafety(bool isSafety, GorillaComputer.ComputerState[] toFilterOut, bool shouldHide)
		{
			this._activeOrderList = this.OrderList;
			if (!isSafety)
			{
				this._activeOrderList = this.OrderList;
				if (this._filteredStates.Count > 0 && toFilterOut.Length != 0)
				{
					for (int i = 0; i < toFilterOut.Length; i++)
					{
						if (this._filteredStates.Contains(toFilterOut[i]))
						{
							this._filteredStates.Remove(toFilterOut[i]);
						}
					}
				}
			}
			else if (shouldHide)
			{
				for (int j = 0; j < toFilterOut.Length; j++)
				{
					if (!this._filteredStates.Contains(toFilterOut[j]))
					{
						this._filteredStates.Add(toFilterOut[j]);
					}
				}
			}
			if (this._filteredStates.Count > 0)
			{
				int k = 0;
				int num = this._activeOrderList.Count;
				while (k < num)
				{
					if (this._filteredStates.Contains(this._activeOrderList[k].State))
					{
						this._activeOrderList.RemoveAt(k);
						k--;
						num--;
					}
					k++;
				}
			}
			this.FunctionsCount = this._activeOrderList.Count;
			this.FunctionNames.Clear();
			this._activeOrderList.ForEach(delegate(GorillaComputer.StateOrderItem s)
			{
				string name = s.GetName();
				if (name.Length > this.highestCharacterCount)
				{
					this.highestCharacterCount = name.Length;
				}
				this.FunctionNames.Add(name);
			});
			for (int l = 0; l < this.FunctionsCount; l++)
			{
				int num2 = this.highestCharacterCount - this.FunctionNames[l].Length;
				for (int m = 0; m < num2; m++)
				{
					List<string> functionNames = this.FunctionNames;
					int index = l;
					functionNames[index] += " ";
				}
			}
			this.UpdateScreen();
		}

		// Token: 0x0600559F RID: 21919 RVA: 0x001A9E80 File Offset: 0x001A8080
		public void KID_SetVoiceChatSettingOnStart(bool voiceChatEnabled, Permission.ManagedByEnum managedBy, bool hasOptedInPreviously)
		{
			if (managedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				return;
			}
			if (managedBy == Permission.ManagedByEnum.GUARDIAN)
			{
				string @string = PlayerPrefs.GetString("voiceChatOn", "");
				voiceChatEnabled = (string.IsNullOrEmpty(@string) ? voiceChatEnabled : (@string == "TRUE"));
				this.SetVoice(voiceChatEnabled, false);
				return;
			}
			voiceChatEnabled = (PlayerPrefs.GetString("voiceChatOn", voiceChatEnabled ? "TRUE" : "") == "TRUE");
			this.SetVoice(voiceChatEnabled, !hasOptedInPreviously && voiceChatEnabled);
		}

		// Token: 0x060055A0 RID: 21920 RVA: 0x001A9EFC File Offset: 0x001A80FC
		private void SetVoice(bool setting, bool saveSetting = true)
		{
			this.voiceChatOn = (setting ? "TRUE" : "FALSE");
			if (setting && !KIDManager.CheckFeatureOptIn(EKIDFeatures.Voice_Chat, null).Item2)
			{
				KIDManager.SetFeatureOptIn(EKIDFeatures.Voice_Chat, true);
				KIDManager.SendOptInPermissions();
			}
			if (!saveSetting)
			{
				return;
			}
			PlayerPrefs.SetString("voiceChatOn", this.voiceChatOn);
			PlayerPrefs.Save();
		}

		// Token: 0x060055A1 RID: 21921 RVA: 0x001A9F55 File Offset: 0x001A8155
		public bool CheckVoiceChatEnabled()
		{
			return this.voiceChatOn == "TRUE";
		}

		// Token: 0x060055A2 RID: 21922 RVA: 0x001A9F68 File Offset: 0x001A8168
		private void SetVoiceChatBySafety(bool voiceChatEnabled, Permission.ManagedByEnum managedBy)
		{
			bool isSafety = !voiceChatEnabled;
			this.SetComputerSettingsBySafety(isSafety, new GorillaComputer.ComputerState[]
			{
				GorillaComputer.ComputerState.Voice,
				GorillaComputer.ComputerState.AutoMute,
				GorillaComputer.ComputerState.Mic
			}, false);
			string value = PlayerPrefs.GetString("voiceChatOn", "");
			if (KIDManager.KidEnabledAndReady)
			{
				Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat);
				if (permissionDataByFeature != null)
				{
					ValueTuple<bool, bool> valueTuple = KIDManager.CheckFeatureOptIn(EKIDFeatures.Voice_Chat, permissionDataByFeature);
					if (valueTuple.Item1 && !valueTuple.Item2)
					{
						value = "FALSE";
					}
				}
				else
				{
					Debug.LogErrorFormat("[KID] Could not find permission data for [" + EKIDFeatures.Voice_Chat.ToStandardisedString() + "]", Array.Empty<object>());
				}
			}
			switch (managedBy)
			{
			case Permission.ManagedByEnum.PLAYER:
				if (string.IsNullOrEmpty(value))
				{
					this.voiceChatOn = (voiceChatEnabled ? "TRUE" : "FALSE");
				}
				else
				{
					this.voiceChatOn = value;
				}
				break;
			case Permission.ManagedByEnum.GUARDIAN:
				if (KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat).Enabled)
				{
					if (string.IsNullOrEmpty(value))
					{
						this.voiceChatOn = "TRUE";
					}
					else
					{
						this.voiceChatOn = value;
					}
				}
				else
				{
					this.voiceChatOn = "FALSE";
				}
				break;
			case Permission.ManagedByEnum.PROHIBITED:
				this.voiceChatOn = "FALSE";
				break;
			}
			RigContainer.RefreshAllRigVoices();
			Debug.Log("[KID] On Session Update - Voice Chat Permission changed - Has enabled voiceChat? [" + voiceChatEnabled.ToString() + "]");
		}

		// Token: 0x060055A3 RID: 21923 RVA: 0x001AA094 File Offset: 0x001A8294
		public void SetNametagSetting(bool setting, Permission.ManagedByEnum managedBy, bool hasOptedInPreviously)
		{
			if (managedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				return;
			}
			if (managedBy == Permission.ManagedByEnum.GUARDIAN)
			{
				int @int = PlayerPrefs.GetInt(this.NameTagPlayerPref, 1);
				setting = (setting && @int == 1);
				this.UpdateNametagSetting(setting, false);
				return;
			}
			setting = (PlayerPrefs.GetInt(this.NameTagPlayerPref, setting ? 1 : 0) == 1);
			this.UpdateNametagSetting(setting, !hasOptedInPreviously && setting);
		}

		// Token: 0x060055A4 RID: 21924 RVA: 0x001AA0F0 File Offset: 0x001A82F0
		public static void RegisterOnNametagSettingChanged(Action<bool> callback)
		{
			GorillaComputer.onNametagSettingChangedAction = (Action<bool>)Delegate.Combine(GorillaComputer.onNametagSettingChangedAction, callback);
		}

		// Token: 0x060055A5 RID: 21925 RVA: 0x001AA107 File Offset: 0x001A8307
		public static void UnregisterOnNametagSettingChanged(Action<bool> callback)
		{
			GorillaComputer.onNametagSettingChangedAction = (Action<bool>)Delegate.Remove(GorillaComputer.onNametagSettingChangedAction, callback);
		}

		// Token: 0x060055A6 RID: 21926 RVA: 0x001AA120 File Offset: 0x001A8320
		private void UpdateNametagSetting(bool newSettingValue, bool saveSetting = true)
		{
			if (newSettingValue)
			{
				KIDManager.SetFeatureOptIn(EKIDFeatures.Custom_Nametags, true);
			}
			this.NametagsEnabled = newSettingValue;
			NetworkSystem.Instance.SetMyNickName(this.NametagsEnabled ? this.savedName : NetworkSystem.Instance.GetMyDefaultName());
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[]
				{
					this.redValue,
					this.greenValue,
					this.blueValue
				});
			}
			Action<bool> action = GorillaComputer.onNametagSettingChangedAction;
			if (action != null)
			{
				action(this.NametagsEnabled);
			}
			if (!saveSetting)
			{
				return;
			}
			int value = this.NametagsEnabled ? 1 : 0;
			PlayerPrefs.SetInt(this.NameTagPlayerPref, value);
			PlayerPrefs.Save();
		}

		// Token: 0x060055A7 RID: 21927 RVA: 0x000023F5 File Offset: 0x000005F5
		void IMatchmakingCallbacks.OnFriendListUpdate(List<Photon.Realtime.FriendInfo> friendList)
		{
		}

		// Token: 0x060055A8 RID: 21928 RVA: 0x000023F5 File Offset: 0x000005F5
		void IMatchmakingCallbacks.OnCreatedRoom()
		{
		}

		// Token: 0x060055A9 RID: 21929 RVA: 0x000023F5 File Offset: 0x000005F5
		void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x060055AA RID: 21930 RVA: 0x000023F5 File Offset: 0x000005F5
		void IMatchmakingCallbacks.OnJoinedRoom()
		{
		}

		// Token: 0x060055AB RID: 21931 RVA: 0x000023F5 File Offset: 0x000005F5
		void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
		{
		}

		// Token: 0x060055AC RID: 21932 RVA: 0x000023F5 File Offset: 0x000005F5
		void IMatchmakingCallbacks.OnLeftRoom()
		{
		}

		// Token: 0x060055AD RID: 21933 RVA: 0x000023F5 File Offset: 0x000005F5
		void IMatchmakingCallbacks.OnPreLeavingRoom()
		{
		}

		// Token: 0x060055AE RID: 21934 RVA: 0x001AA1EB File Offset: 0x001A83EB
		void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
		{
			if (returnCode == 32765)
			{
				this.roomFull = true;
			}
		}

		// Token: 0x060055AF RID: 21935 RVA: 0x001AA1FC File Offset: 0x001A83FC
		public void SetInVirtualStump(bool inVirtualStump)
		{
			this.playerInVirtualStump = inVirtualStump;
			this.roomToJoin = (this.playerInVirtualStump ? (this.virtualStumpRoomPrepend + this.roomToJoin) : this.roomToJoin.RemoveAll(this.virtualStumpRoomPrepend, StringComparison.OrdinalIgnoreCase));
		}

		// Token: 0x060055B0 RID: 21936 RVA: 0x001AA238 File Offset: 0x001A8438
		public bool IsPlayerInVirtualStump()
		{
			return this.playerInVirtualStump;
		}

		// Token: 0x060055B1 RID: 21937 RVA: 0x001AA240 File Offset: 0x001A8440
		public void SetLimitOnlineScreens(bool isLimited)
		{
			this.limitOnlineScreens = isLimited;
			this.UpdateScreen();
		}

		// Token: 0x060055B2 RID: 21938 RVA: 0x001AA24F File Offset: 0x001A844F
		private void InitializeKIdState()
		{
			KIDManager.RegisterSessionUpdateCallback_AnyPermission(new Action(this.OnSessionUpdate_GorillaComputer));
		}

		// Token: 0x060055B3 RID: 21939 RVA: 0x001AA262 File Offset: 0x001A8462
		private void UpdateKidState()
		{
			this._currentScreentState = GorillaComputer.EKidScreenState.Ready;
		}

		// Token: 0x060055B4 RID: 21940 RVA: 0x001AA26B File Offset: 0x001A846B
		private void RequestUpdatedPermissions()
		{
			if (!KIDManager.KidEnabledAndReady)
			{
				return;
			}
			if (this._waitingForUpdatedSession)
			{
				return;
			}
			if (Time.time < this._nextUpdateAttemptTime)
			{
				return;
			}
			this._waitingForUpdatedSession = true;
			this.UpdateSession();
		}

		// Token: 0x060055B5 RID: 21941 RVA: 0x001AA29C File Offset: 0x001A849C
		private void UpdateSession()
		{
			GorillaComputer.<UpdateSession>d__275 <UpdateSession>d__;
			<UpdateSession>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<UpdateSession>d__.<>4__this = this;
			<UpdateSession>d__.<>1__state = -1;
			<UpdateSession>d__.<>t__builder.Start<GorillaComputer.<UpdateSession>d__275>(ref <UpdateSession>d__);
		}

		// Token: 0x060055B6 RID: 21942 RVA: 0x001AA2D3 File Offset: 0x001A84D3
		private void OnSessionUpdate_GorillaComputer()
		{
			this.UpdateKidState();
			this.UpdateScreen();
		}

		// Token: 0x060055B7 RID: 21943 RVA: 0x001AA2E1 File Offset: 0x001A84E1
		private void ProcessScreen_SetupKID()
		{
			if (!KIDManager.KidEnabledAndReady)
			{
				Debug.LogError("[KID] Unable to start k-ID Flow. Kid is disabled");
				return;
			}
		}

		// Token: 0x060055B8 RID: 21944 RVA: 0x001AA2F8 File Offset: 0x001A84F8
		private bool GuardianConsentMessage(string setupKIDButtonName, string featureDescription)
		{
			GorillaText gorillaText = this.screenText;
			gorillaText.Text = gorillaText.Text + "PARENT/GUARDIAN PERMISSION REQUIRED TO " + featureDescription.ToUpper() + "!";
			if (this._waitingForUpdatedSession)
			{
				GorillaText gorillaText2 = this.screenText;
				gorillaText2.Text += "\n\nWAITING FOR PARENT/GUARDIAN CONSENT!";
				return true;
			}
			if (Time.time >= this._nextUpdateAttemptTime)
			{
				GorillaText gorillaText3 = this.screenText;
				gorillaText3.Text += "\n\nPRESS OPTION 2 TO REFRESH PERMISSIONS!";
			}
			else
			{
				GorillaText gorillaText4 = this.screenText;
				gorillaText4.Text = gorillaText4.Text + "\n\nCHECK AGAIN IN " + ((int)(this._nextUpdateAttemptTime - Time.time)).ToString() + " SECONDS!";
			}
			return false;
		}

		// Token: 0x060055B9 RID: 21945 RVA: 0x001AA3B0 File Offset: 0x001A85B0
		private void ProhibitedMessage(string verb)
		{
			this.screenText.Text = "";
			GorillaText gorillaText = this.screenText;
			gorillaText.Text = gorillaText.Text + "\n\nYOU ARE NOT ALLOWED TO " + verb + " IN YOUR JURISDICTION.";
		}

		// Token: 0x060055BA RID: 21946 RVA: 0x001AA3E3 File Offset: 0x001A85E3
		private void RoomScreen_Permission()
		{
			if (!KIDManager.KidEnabled)
			{
				this.screenText.Text = "YOU CANNOT USE THE PRIVATE ROOM FEATURE RIGHT NOW";
				return;
			}
			this.screenText.Text = "";
			this.GuardianConsentMessage("OPTION 3", "JOIN PRIVATE ROOMS");
		}

		// Token: 0x060055BB RID: 21947 RVA: 0x001AA41E File Offset: 0x001A861E
		private void RoomScreen_KIdProhibited()
		{
			this.ProhibitedMessage("CREATE OR JOIN PRIVATE ROOMS");
		}

		// Token: 0x060055BC RID: 21948 RVA: 0x001AA42C File Offset: 0x001A862C
		private void VoiceScreen_Permission()
		{
			this.screenText.Text = "VOICE TYPE: \"MONKE\"\n\n";
			if (!KIDManager.KidEnabled)
			{
				GorillaText gorillaText = this.screenText;
				gorillaText.Text += "YOU CANNOT USE THE HUMAN VOICE TYPE FEATURE RIGHT NOW";
				return;
			}
			this.GuardianConsentMessage("OPTION 3", "ENABLE HUMAN VOICE CHAT");
		}

		// Token: 0x060055BD RID: 21949 RVA: 0x001AA47D File Offset: 0x001A867D
		private void VoiceScreen_KIdProhibited()
		{
			this.ProhibitedMessage("USE THE VOICE CHAT");
		}

		// Token: 0x060055BE RID: 21950 RVA: 0x001AA48A File Offset: 0x001A868A
		private void MicScreen_Permission()
		{
			this.screenText.Text = "";
			this.GuardianConsentMessage("OPTION 3", "ENABLE HUMAN VOICE CHAT");
		}

		// Token: 0x060055BF RID: 21951 RVA: 0x001AA4AD File Offset: 0x001A86AD
		private void MicScreen_KIdProhibited()
		{
			this.VoiceScreen_KIdProhibited();
		}

		// Token: 0x060055C0 RID: 21952 RVA: 0x001AA4B5 File Offset: 0x001A86B5
		private void NameScreen_Permission()
		{
			if (!KIDManager.KidEnabled)
			{
				this.screenText.Text = "YOU CANNOT USE THE CUSTOM NICKNAME FEATURE RIGHT NOW";
				return;
			}
			this.screenText.Text = "";
			this.GuardianConsentMessage("OPTION 3", "SET CUSTOM NICKNAMES");
		}

		// Token: 0x060055C1 RID: 21953 RVA: 0x001AA4F0 File Offset: 0x001A86F0
		private void NameScreen_KIdProhibited()
		{
			this.ProhibitedMessage("SET CUSTOM NICKNAMES");
		}

		// Token: 0x060055C2 RID: 21954 RVA: 0x001AA500 File Offset: 0x001A8700
		private void OnKIDSessionUpdated_CustomNicknames(bool showCustomNames, Permission.ManagedByEnum managedBy)
		{
			bool flag = (showCustomNames || managedBy == Permission.ManagedByEnum.PLAYER) && managedBy != Permission.ManagedByEnum.PROHIBITED;
			this.SetComputerSettingsBySafety(!flag, new GorillaComputer.ComputerState[]
			{
				GorillaComputer.ComputerState.Name
			}, false);
			int @int = PlayerPrefs.GetInt(this.NameTagPlayerPref, -1);
			bool flag2 = @int > 0;
			switch (managedBy)
			{
			case Permission.ManagedByEnum.PLAYER:
				if (showCustomNames)
				{
					this.NametagsEnabled = (@int == -1 || flag2);
				}
				else
				{
					this.NametagsEnabled = (@int != -1 && flag2);
				}
				break;
			case Permission.ManagedByEnum.GUARDIAN:
				this.NametagsEnabled = (showCustomNames && (flag2 || @int == -1));
				break;
			case Permission.ManagedByEnum.PROHIBITED:
				this.NametagsEnabled = false;
				break;
			}
			if (this.NametagsEnabled)
			{
				NetworkSystem.Instance.SetMyNickName(this.savedName);
			}
			Action<bool> action = GorillaComputer.onNametagSettingChangedAction;
			if (action == null)
			{
				return;
			}
			action(this.NametagsEnabled);
		}

		// Token: 0x060055C3 RID: 21955 RVA: 0x001AA5CC File Offset: 0x001A87CC
		private void TroopScreen_Permission()
		{
			this.screenText.Text = "";
			if (!KIDManager.KidEnabled)
			{
				GorillaText gorillaText = this.screenText;
				gorillaText.Text += "YOU CANNOT USE THE TROOPS FEATURE RIGHT NOW";
				return;
			}
			this.GuardianConsentMessage("OPTION 3", "JOIN TROOPS");
		}

		// Token: 0x060055C4 RID: 21956 RVA: 0x001AA61D File Offset: 0x001A881D
		private void TroopScreen_KIdProhibited()
		{
			this.ProhibitedMessage("CREATE OR JOIN TROOPS");
		}

		// Token: 0x060055C5 RID: 21957 RVA: 0x001AA62A File Offset: 0x001A882A
		private void ProcessKIdState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed == GorillaKeyboardBindings.option1 && this._currentScreentState == GorillaComputer.EKidScreenState.Ready)
			{
				this.RequestUpdatedPermissions();
			}
		}

		// Token: 0x060055C6 RID: 21958 RVA: 0x001AA63F File Offset: 0x001A883F
		private void KIdScreen()
		{
			if (!KIDManager.KidEnabledAndReady)
			{
				return;
			}
			if (!KIDManager.HasSession)
			{
				this.GuardianConsentMessage("OPTION 3", "");
				return;
			}
			this.KIdScreen_DisplayPermissions();
		}

		// Token: 0x060055C7 RID: 21959 RVA: 0x001AA668 File Offset: 0x001A8868
		private void KIdScreen_DisplayPermissions()
		{
			AgeStatusType activeAccountStatus = KIDManager.GetActiveAccountStatus();
			string str = (!KIDManager.InitialisationSuccessful) ? "NOT READY" : activeAccountStatus.ToString();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("k-ID Account Status:\t" + str);
			if (activeAccountStatus == (AgeStatusType)0)
			{
				stringBuilder.AppendLine("\nPress 'OPTION 1' to get permissions!");
				this.screenText.Text = stringBuilder.ToString();
				return;
			}
			if (this._waitingForUpdatedSession)
			{
				stringBuilder.AppendLine("\nWAITING FOR PARENT/GUARDIAN CONSENT!");
				this.screenText.Text = stringBuilder.ToString();
				return;
			}
			stringBuilder.AppendLine("\nPermissions:");
			List<Permission> allPermissionsData = KIDManager.GetAllPermissionsData();
			int count = allPermissionsData.Count;
			int num = 1;
			for (int i = 0; i < count; i++)
			{
				if (this._interestedPermissionNames.Contains(allPermissionsData[i].Name))
				{
					string text = allPermissionsData[i].Enabled ? "<color=#85ffa5>" : "<color=\"RED\">";
					stringBuilder.AppendLine(string.Concat(new string[]
					{
						"[",
						num.ToString(),
						"] ",
						text,
						allPermissionsData[i].Name,
						"</color>"
					}));
					num++;
				}
			}
			stringBuilder.AppendLine("\nTO REFRESH PERMISSIONS PRESS OPTION 1!");
			this.screenText.Text = stringBuilder.ToString();
		}

		// Token: 0x060055CB RID: 21963 RVA: 0x001AAA09 File Offset: 0x001A8C09
		[CompilerGenerated]
		private IEnumerator <LoadingScreen>g__LoadingScreenLocal|195_0()
		{
			int dotsCount = 0;
			while (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				int num = dotsCount;
				dotsCount = num + 1;
				if (dotsCount == 3)
				{
					dotsCount = 0;
				}
				this.screenText.Text = "LOADING";
				for (int i = 0; i < dotsCount; i++)
				{
					GorillaText gorillaText = this.screenText;
					gorillaText.Text += ". ";
				}
				yield return this.waitOneSecond;
			}
			yield break;
		}

		// Token: 0x04005ECA RID: 24266
		private const bool HIDE_SCREENS = false;

		// Token: 0x04005ECB RID: 24267
		public const string NAMETAG_PLAYER_PREF_KEY = "nameTagsOn";

		// Token: 0x04005ECC RID: 24268
		[OnEnterPlay_SetNull]
		public static volatile GorillaComputer instance;

		// Token: 0x04005ECD RID: 24269
		[OnEnterPlay_Set(false)]
		public static bool hasInstance;

		// Token: 0x04005ECE RID: 24270
		[OnEnterPlay_SetNull]
		private static Action<bool> onNametagSettingChangedAction;

		// Token: 0x04005ECF RID: 24271
		public bool tryGetTimeAgain;

		// Token: 0x04005ED0 RID: 24272
		public Material unpressedMaterial;

		// Token: 0x04005ED1 RID: 24273
		public Material pressedMaterial;

		// Token: 0x04005ED2 RID: 24274
		public string currentTextField;

		// Token: 0x04005ED3 RID: 24275
		public float buttonFadeTime;

		// Token: 0x04005ED4 RID: 24276
		public string offlineTextInitialString;

		// Token: 0x04005ED5 RID: 24277
		public GorillaText screenText;

		// Token: 0x04005ED6 RID: 24278
		public GorillaText functionSelectText;

		// Token: 0x04005ED7 RID: 24279
		public GorillaText wallScreenText;

		// Token: 0x04005ED8 RID: 24280
		public string versionMismatch = "PLEASE UPDATE TO THE LATEST VERSION OF GORILLA TAG. YOU'RE ON AN OLD VERSION. FEEL FREE TO RUN AROUND, BUT YOU WON'T BE ABLE TO PLAY WITH ANYONE ELSE.";

		// Token: 0x04005ED9 RID: 24281
		public string unableToConnect = "UNABLE TO CONNECT TO THE INTERNET. PLEASE CHECK YOUR CONNECTION AND RESTART THE GAME.";

		// Token: 0x04005EDA RID: 24282
		public Material wrongVersionMaterial;

		// Token: 0x04005EDB RID: 24283
		public MeshRenderer wallScreenRenderer;

		// Token: 0x04005EDC RID: 24284
		public MeshRenderer computerScreenRenderer;

		// Token: 0x04005EDD RID: 24285
		public long startupMillis;

		// Token: 0x04005EDE RID: 24286
		public DateTime startupTime;

		// Token: 0x04005EDF RID: 24287
		public string lastPressedGameMode;

		// Token: 0x04005EE0 RID: 24288
		public WatchableStringSO currentGameMode;

		// Token: 0x04005EE1 RID: 24289
		public WatchableStringSO currentGameModeText;

		// Token: 0x04005EE2 RID: 24290
		public int includeUpdatedServerSynchTest;

		// Token: 0x04005EE3 RID: 24291
		public PhotonNetworkController networkController;

		// Token: 0x04005EE4 RID: 24292
		public float updateCooldown = 1f;

		// Token: 0x04005EE5 RID: 24293
		public float lastUpdateTime;

		// Token: 0x04005EE6 RID: 24294
		public bool isConnectedToMaster;

		// Token: 0x04005EE7 RID: 24295
		public bool internetFailure;

		// Token: 0x04005EE8 RID: 24296
		public string[] allowedMapsToJoin;

		// Token: 0x04005EE9 RID: 24297
		public bool limitOnlineScreens;

		// Token: 0x04005EEA RID: 24298
		[Header("State vars")]
		public bool stateUpdated;

		// Token: 0x04005EEB RID: 24299
		public bool screenChanged;

		// Token: 0x04005EEC RID: 24300
		public bool initialized;

		// Token: 0x04005EED RID: 24301
		public List<GorillaComputer.StateOrderItem> OrderList = new List<GorillaComputer.StateOrderItem>
		{
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Room),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Name),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Color),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Turn),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Mic),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Queue),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Troop),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Group),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Voice),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.AutoMute, "Automod"),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Visuals, "Items"),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Credits),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Support)
		};

		// Token: 0x04005EEE RID: 24302
		public string Pointer = "<-";

		// Token: 0x04005EEF RID: 24303
		public int highestCharacterCount;

		// Token: 0x04005EF0 RID: 24304
		public List<string> FunctionNames = new List<string>();

		// Token: 0x04005EF1 RID: 24305
		public int FunctionsCount;

		// Token: 0x04005EF2 RID: 24306
		[Header("Room vars")]
		public string roomToJoin;

		// Token: 0x04005EF3 RID: 24307
		public bool roomFull;

		// Token: 0x04005EF4 RID: 24308
		public bool roomNotAllowed;

		// Token: 0x04005EF5 RID: 24309
		[Header("Mic vars")]
		public string pttType;

		// Token: 0x04005EF6 RID: 24310
		[Header("Automute vars")]
		public string autoMuteType;

		// Token: 0x04005EF7 RID: 24311
		[Header("Queue vars")]
		public string currentQueue;

		// Token: 0x04005EF8 RID: 24312
		public bool allowedInCompetitive;

		// Token: 0x04005EF9 RID: 24313
		[Header("Group Vars")]
		public string groupMapJoin;

		// Token: 0x04005EFA RID: 24314
		public int groupMapJoinIndex;

		// Token: 0x04005EFB RID: 24315
		public GorillaFriendCollider friendJoinCollider;

		// Token: 0x04005EFC RID: 24316
		[Header("Troop vars")]
		public string troopName;

		// Token: 0x04005EFD RID: 24317
		public bool troopQueueActive;

		// Token: 0x04005EFE RID: 24318
		public string troopToJoin;

		// Token: 0x04005EFF RID: 24319
		private bool rememberTroopQueueState;

		// Token: 0x04005F00 RID: 24320
		[Header("Join Triggers")]
		public Dictionary<string, GorillaNetworkJoinTrigger> primaryTriggersByZone = new Dictionary<string, GorillaNetworkJoinTrigger>();

		// Token: 0x04005F01 RID: 24321
		public string voiceChatOn;

		// Token: 0x04005F02 RID: 24322
		[Header("Mode select vars")]
		public ModeSelectButton[] modeSelectButtons;

		// Token: 0x04005F03 RID: 24323
		public string version;

		// Token: 0x04005F04 RID: 24324
		public string buildDate;

		// Token: 0x04005F05 RID: 24325
		public string buildCode;

		// Token: 0x04005F06 RID: 24326
		[Header("Cosmetics")]
		public bool disableParticles;

		// Token: 0x04005F07 RID: 24327
		public float instrumentVolume;

		// Token: 0x04005F08 RID: 24328
		[Header("Credits")]
		public CreditsView creditsView;

		// Token: 0x04005F09 RID: 24329
		[Header("Handedness")]
		public bool leftHanded;

		// Token: 0x04005F0A RID: 24330
		[Header("Name state vars")]
		public string savedName;

		// Token: 0x04005F0B RID: 24331
		public string currentName;

		// Token: 0x04005F0C RID: 24332
		public TextAsset exactOneWeekFile;

		// Token: 0x04005F0D RID: 24333
		public TextAsset anywhereOneWeekFile;

		// Token: 0x04005F0E RID: 24334
		public TextAsset anywhereTwoWeekFile;

		// Token: 0x04005F0F RID: 24335
		private List<GorillaComputer.ComputerState> _filteredStates = new List<GorillaComputer.ComputerState>();

		// Token: 0x04005F10 RID: 24336
		private List<GorillaComputer.StateOrderItem> _activeOrderList = new List<GorillaComputer.StateOrderItem>();

		// Token: 0x04005F11 RID: 24337
		private Stack<GorillaComputer.ComputerState> stateStack = new Stack<GorillaComputer.ComputerState>();

		// Token: 0x04005F12 RID: 24338
		private GorillaComputer.ComputerState currentComputerState;

		// Token: 0x04005F13 RID: 24339
		private GorillaComputer.ComputerState previousComputerState;

		// Token: 0x04005F14 RID: 24340
		private int currentStateIndex;

		// Token: 0x04005F15 RID: 24341
		private int usersBanned;

		// Token: 0x04005F16 RID: 24342
		private float redValue;

		// Token: 0x04005F17 RID: 24343
		private string redText;

		// Token: 0x04005F18 RID: 24344
		private float blueValue;

		// Token: 0x04005F19 RID: 24345
		private string blueText;

		// Token: 0x04005F1A RID: 24346
		private float greenValue;

		// Token: 0x04005F1B RID: 24347
		private string greenText;

		// Token: 0x04005F1C RID: 24348
		private int colorCursorLine;

		// Token: 0x04005F1D RID: 24349
		private string warningConfirmationInputString = string.Empty;

		// Token: 0x04005F1E RID: 24350
		private bool displaySupport;

		// Token: 0x04005F1F RID: 24351
		private string[] exactOneWeek;

		// Token: 0x04005F20 RID: 24352
		private string[] anywhereOneWeek;

		// Token: 0x04005F21 RID: 24353
		private string[] anywhereTwoWeek;

		// Token: 0x04005F22 RID: 24354
		private GorillaComputer.RedemptionResult redemptionResult;

		// Token: 0x04005F23 RID: 24355
		private string redemptionCode = "";

		// Token: 0x04005F24 RID: 24356
		private bool playerInVirtualStump;

		// Token: 0x04005F25 RID: 24357
		private string virtualStumpRoomPrepend = "";

		// Token: 0x04005F26 RID: 24358
		private WaitForSeconds waitOneSecond = new WaitForSeconds(1f);

		// Token: 0x04005F27 RID: 24359
		private Coroutine LoadingRoutine;

		// Token: 0x04005F28 RID: 24360
		private List<string> topTroops = new List<string>();

		// Token: 0x04005F29 RID: 24361
		private bool hasRequestedInitialTroopPopulation;

		// Token: 0x04005F2A RID: 24362
		private int currentTroopPopulation = -1;

		// Token: 0x04005F2C RID: 24364
		private float lastCheckedWifi;

		// Token: 0x04005F2D RID: 24365
		private float checkIfDisconnectedSeconds = 10f;

		// Token: 0x04005F2E RID: 24366
		private float checkIfConnectedSeconds = 1f;

		// Token: 0x04005F2F RID: 24367
		private float troopPopulationCheckCooldown = 3f;

		// Token: 0x04005F30 RID: 24368
		private float nextPopulationCheckTime;

		// Token: 0x04005F31 RID: 24369
		public Action OnServerTimeUpdated;

		// Token: 0x04005F32 RID: 24370
		private const string ENABLED_COLOUR = "#85ffa5";

		// Token: 0x04005F33 RID: 24371
		private const string DISABLED_COLOUR = "\"RED\"";

		// Token: 0x04005F34 RID: 24372
		private const string FAMILY_PORTAL_URL = "k-id.com/code";

		// Token: 0x04005F35 RID: 24373
		private float _updateAttemptCooldown = 15f;

		// Token: 0x04005F36 RID: 24374
		private float _nextUpdateAttemptTime;

		// Token: 0x04005F37 RID: 24375
		private bool _waitingForUpdatedSession;

		// Token: 0x04005F38 RID: 24376
		private GorillaComputer.EKidScreenState _currentScreentState = GorillaComputer.EKidScreenState.Show_OTP;

		// Token: 0x04005F39 RID: 24377
		private string[] _interestedPermissionNames = new string[]
		{
			"custom-username",
			"voice-chat",
			"join-groups"
		};

		// Token: 0x02000D68 RID: 3432
		public enum ComputerState
		{
			// Token: 0x04005F3B RID: 24379
			Startup,
			// Token: 0x04005F3C RID: 24380
			Color,
			// Token: 0x04005F3D RID: 24381
			Name,
			// Token: 0x04005F3E RID: 24382
			Turn,
			// Token: 0x04005F3F RID: 24383
			Mic,
			// Token: 0x04005F40 RID: 24384
			Room,
			// Token: 0x04005F41 RID: 24385
			Queue,
			// Token: 0x04005F42 RID: 24386
			Group,
			// Token: 0x04005F43 RID: 24387
			Voice,
			// Token: 0x04005F44 RID: 24388
			AutoMute,
			// Token: 0x04005F45 RID: 24389
			Credits,
			// Token: 0x04005F46 RID: 24390
			Visuals,
			// Token: 0x04005F47 RID: 24391
			Time,
			// Token: 0x04005F48 RID: 24392
			NameWarning,
			// Token: 0x04005F49 RID: 24393
			Loading,
			// Token: 0x04005F4A RID: 24394
			Support,
			// Token: 0x04005F4B RID: 24395
			Troop,
			// Token: 0x04005F4C RID: 24396
			KID,
			// Token: 0x04005F4D RID: 24397
			Redemption
		}

		// Token: 0x02000D69 RID: 3433
		private enum NameCheckResult
		{
			// Token: 0x04005F4F RID: 24399
			Success,
			// Token: 0x04005F50 RID: 24400
			Warning,
			// Token: 0x04005F51 RID: 24401
			Ban
		}

		// Token: 0x02000D6A RID: 3434
		public enum RedemptionResult
		{
			// Token: 0x04005F53 RID: 24403
			Empty,
			// Token: 0x04005F54 RID: 24404
			Invalid,
			// Token: 0x04005F55 RID: 24405
			Checking,
			// Token: 0x04005F56 RID: 24406
			AlreadyUsed,
			// Token: 0x04005F57 RID: 24407
			Success
		}

		// Token: 0x02000D6B RID: 3435
		[Serializable]
		public class StateOrderItem
		{
			// Token: 0x060055CD RID: 21965 RVA: 0x001AAA52 File Offset: 0x001A8C52
			public StateOrderItem()
			{
			}

			// Token: 0x060055CE RID: 21966 RVA: 0x001AAA65 File Offset: 0x001A8C65
			public StateOrderItem(GorillaComputer.ComputerState state)
			{
				this.State = state;
			}

			// Token: 0x060055CF RID: 21967 RVA: 0x001AAA7F File Offset: 0x001A8C7F
			public StateOrderItem(GorillaComputer.ComputerState state, string overrideName)
			{
				this.State = state;
				this.OverrideName = overrideName;
			}

			// Token: 0x060055D0 RID: 21968 RVA: 0x001AAAA0 File Offset: 0x001A8CA0
			public string GetName()
			{
				if (!string.IsNullOrEmpty(this.OverrideName))
				{
					return this.OverrideName.ToUpper();
				}
				return this.State.ToString().ToUpper();
			}

			// Token: 0x04005F58 RID: 24408
			public GorillaComputer.ComputerState State;

			// Token: 0x04005F59 RID: 24409
			[Tooltip("Case not important - ToUpper applied at runtime")]
			public string OverrideName = "";
		}

		// Token: 0x02000D6C RID: 3436
		private enum EKidScreenState
		{
			// Token: 0x04005F5B RID: 24411
			Ready,
			// Token: 0x04005F5C RID: 24412
			Show_OTP,
			// Token: 0x04005F5D RID: 24413
			Show_Setup_Screen
		}
	}
}
