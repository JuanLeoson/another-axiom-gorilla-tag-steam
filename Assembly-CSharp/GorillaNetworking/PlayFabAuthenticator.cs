using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using GorillaExtensions;
using JetBrains.Annotations;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using Steamworks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GorillaNetworking
{
	// Token: 0x02000DA1 RID: 3489
	public class PlayFabAuthenticator : MonoBehaviour
	{
		// Token: 0x17000849 RID: 2121
		// (get) Token: 0x060056AA RID: 22186 RVA: 0x001AEBEA File Offset: 0x001ACDEA
		public GorillaComputer gorillaComputer
		{
			get
			{
				return GorillaComputer.instance;
			}
		}

		// Token: 0x1700084A RID: 2122
		// (get) Token: 0x060056AB RID: 22187 RVA: 0x001AEBF3 File Offset: 0x001ACDF3
		// (set) Token: 0x060056AC RID: 22188 RVA: 0x001AEBFB File Offset: 0x001ACDFB
		public bool IsReturningPlayer { get; private set; }

		// Token: 0x1700084B RID: 2123
		// (get) Token: 0x060056AD RID: 22189 RVA: 0x001AEC04 File Offset: 0x001ACE04
		// (set) Token: 0x060056AE RID: 22190 RVA: 0x001AEC0C File Offset: 0x001ACE0C
		public bool postAuthSetSafety { get; private set; }

		// Token: 0x060056AF RID: 22191 RVA: 0x001AEC18 File Offset: 0x001ACE18
		private void Awake()
		{
			if (PlayFabAuthenticator.instance == null)
			{
				PlayFabAuthenticator.instance = this;
			}
			else if (PlayFabAuthenticator.instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			if (PlayFabAuthenticator.instance.photonAuthenticator == null)
			{
				PlayFabAuthenticator.instance.photonAuthenticator = PlayFabAuthenticator.instance.gameObject.GetOrAddComponent<PhotonAuthenticator>();
			}
			this.platform = ScriptableObject.CreateInstance<PlatformTagJoin>();
			PlayFabSettings.CompressApiData = false;
			new byte[1];
			if (this.screenDebugMode)
			{
				this.debugText.text = "";
			}
			Debug.Log("doing steam thing");
			if (PlayFabAuthenticator.instance.steamAuthenticator == null)
			{
				PlayFabAuthenticator.instance.steamAuthenticator = PlayFabAuthenticator.instance.gameObject.GetOrAddComponent<SteamAuthenticator>();
			}
			this.platform.PlatformTag = "Steam";
			PlayFabSettings.TitleId = PlayFabAuthenticatorSettings.TitleId;
			PlayFabSettings.DisableFocusTimeCollection = true;
			this.BeginLoginFlow();
		}

		// Token: 0x060056B0 RID: 22192 RVA: 0x001AED1C File Offset: 0x001ACF1C
		public void BeginLoginFlow()
		{
			if (!MothershipClientApiUnity.IsEnabled())
			{
				this.AuthenticateWithPlayFab();
				return;
			}
			if (PlayFabAuthenticator.instance.mothershipAuthenticator == null)
			{
				PlayFabAuthenticator.instance.mothershipAuthenticator = (MothershipAuthenticator.Instance ?? PlayFabAuthenticator.instance.gameObject.GetOrAddComponent<MothershipAuthenticator>());
				MothershipAuthenticator mothershipAuthenticator = PlayFabAuthenticator.instance.mothershipAuthenticator;
				mothershipAuthenticator.OnLoginSuccess = (Action)Delegate.Combine(mothershipAuthenticator.OnLoginSuccess, new Action(delegate()
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
				}));
				MothershipAuthenticator mothershipAuthenticator2 = PlayFabAuthenticator.instance.mothershipAuthenticator;
				mothershipAuthenticator2.OnLoginAttemptFailure = (Action<int>)Delegate.Combine(mothershipAuthenticator2.OnLoginAttemptFailure, new Action<int>(delegate(int attempts)
				{
					if (attempts == 1)
					{
						PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					}
				}));
				PlayFabAuthenticator.instance.mothershipAuthenticator.BeginLoginFlow();
			}
		}

		// Token: 0x060056B1 RID: 22193 RVA: 0x000023F5 File Offset: 0x000005F5
		private void Start()
		{
		}

		// Token: 0x060056B2 RID: 22194 RVA: 0x001AEE07 File Offset: 0x001AD007
		private void OnEnable()
		{
			NetworkSystem.Instance.OnCustomAuthenticationResponse += this.OnCustomAuthenticationResponse;
		}

		// Token: 0x060056B3 RID: 22195 RVA: 0x001AEE1F File Offset: 0x001AD01F
		private void OnDisable()
		{
			NetworkSystem.Instance.OnCustomAuthenticationResponse -= this.OnCustomAuthenticationResponse;
			SteamAuthTicket steamAuthTicket = this.steamAuthTicketForPhoton;
			if (steamAuthTicket != null)
			{
				steamAuthTicket.Dispose();
			}
			SteamAuthTicket steamAuthTicket2 = this.steamAuthTicketForPlayFab;
			if (steamAuthTicket2 == null)
			{
				return;
			}
			steamAuthTicket2.Dispose();
		}

		// Token: 0x060056B4 RID: 22196 RVA: 0x001AEE58 File Offset: 0x001AD058
		public void RefreshSteamAuthTicketForPhoton(Action<string> successCallback, Action<EResult> failureCallback)
		{
			SteamAuthTicket steamAuthTicket = this.steamAuthTicketForPhoton;
			if (steamAuthTicket != null)
			{
				steamAuthTicket.Dispose();
			}
			this.steamAuthTicketForPhoton = this.steamAuthenticator.GetAuthTicketForWebApi(this.steamAuthIdForPhoton, successCallback, failureCallback);
		}

		// Token: 0x060056B5 RID: 22197 RVA: 0x001AEE8C File Offset: 0x001AD08C
		private void OnCustomAuthenticationResponse(Dictionary<string, object> response)
		{
			SteamAuthTicket steamAuthTicket = this.steamAuthTicketForPhoton;
			if (steamAuthTicket != null)
			{
				steamAuthTicket.Dispose();
			}
			object obj;
			if (response.TryGetValue("SteamAuthIdForPhoton", out obj))
			{
				string text = obj as string;
				if (text != null)
				{
					this.steamAuthIdForPhoton = text;
					return;
				}
			}
			this.steamAuthIdForPhoton = null;
		}

		// Token: 0x060056B6 RID: 22198 RVA: 0x001AEED4 File Offset: 0x001AD0D4
		public void AuthenticateWithPlayFab()
		{
			Debug.Log("authenticating with playFab!");
			GorillaServer gorillaServer = GorillaServer.Instance;
			if (gorillaServer != null && gorillaServer.FeatureFlagsReady)
			{
				if (KIDManager.KidEnabled)
				{
					Debug.Log("[KID] Is Enabled - Enabling safeties by platform and age category");
					this.DefaultSafetiesByAgeCategory();
				}
			}
			else
			{
				this.postAuthSetSafety = true;
			}
			if (SteamManager.Initialized)
			{
				this.userID = SteamUser.GetSteamID().ToString();
				Debug.Log("trying to auth with steam");
				this.steamAuthTicketForPlayFab = this.steamAuthenticator.GetAuthTicket(delegate(string ticket)
				{
					Debug.Log("Got steam auth session ticket!");
					PlayFabClientAPI.LoginWithSteam(new LoginWithSteamRequest
					{
						CreateAccount = new bool?(true),
						SteamTicket = ticket
					}, new Action<LoginResult>(this.OnLoginWithSteamResponse), new Action<PlayFabError>(this.OnPlayFabError), null, null);
				}, delegate(EResult result)
				{
					base.StartCoroutine(this.DisplayGeneralFailureMessageOnGorillaComputerAfter1Frame());
				});
				return;
			}
			base.StartCoroutine(this.DisplayGeneralFailureMessageOnGorillaComputerAfter1Frame());
		}

		// Token: 0x060056B7 RID: 22199 RVA: 0x001AEF86 File Offset: 0x001AD186
		private IEnumerator VerifyKidAuthenticated(DateTime accountCreationDateTime)
		{
			Task<DateTime?> getNewPlayerDateTimeTask = KIDManager.CheckKIDNewPlayerDateTime();
			yield return new WaitUntil(() => getNewPlayerDateTimeTask.IsCompleted);
			DateTime? result = getNewPlayerDateTimeTask.Result;
			if (result != null && KIDManager.KidEnabled)
			{
				this.IsReturningPlayer = (accountCreationDateTime < result);
			}
			yield break;
		}

		// Token: 0x060056B8 RID: 22200 RVA: 0x001AEF9C File Offset: 0x001AD19C
		private IEnumerator DisplayGeneralFailureMessageOnGorillaComputerAfter1Frame()
		{
			yield return null;
			if (this.gorillaComputer != null)
			{
				this.gorillaComputer.GeneralFailureMessage("UNABLE TO AUTHENTICATE YOUR STEAM ACCOUNT! PLEASE MAKE SURE STEAM IS RUNNING AND YOU ARE LAUNCHING THE GAME DIRECTLY FROM STEAM.");
				this.gorillaComputer.screenText.Text = "UNABLE TO AUTHENTICATE YOUR STEAM ACCOUNT! PLEASE MAKE SURE STEAM IS RUNNING AND YOU ARE LAUNCHING THE GAME DIRECTLY FROM STEAM.";
				Debug.Log("Couldn't authenticate steam account");
			}
			else
			{
				Debug.LogError("PlayFabAuthenticator: gorillaComputer is null, so could not set GeneralFailureMessage notifying user that the steam account could not be authenticated.", this);
			}
			yield break;
		}

		// Token: 0x060056B9 RID: 22201 RVA: 0x001AEFAC File Offset: 0x001AD1AC
		private void OnLoginWithSteamResponse(LoginResult obj)
		{
			this._playFabPlayerIdCache = obj.PlayFabId;
			this._sessionTicket = obj.SessionTicket;
			base.StartCoroutine(this.CachePlayFabId(new PlayFabAuthenticator.CachePlayFabIdRequest
			{
				Platform = this.platform.ToString(),
				SessionTicket = this._sessionTicket,
				PlayFabId = this._playFabPlayerIdCache,
				TitleId = PlayFabSettings.TitleId,
				MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
				MothershipToken = MothershipClientContext.Token,
				MothershipId = MothershipClientContext.MothershipId
			}, new Action<PlayFabAuthenticator.CachePlayFabIdResponse>(this.OnCachePlayFabIdRequest)));
		}

		// Token: 0x060056BA RID: 22202 RVA: 0x001AF044 File Offset: 0x001AD244
		private void OnCachePlayFabIdRequest([CanBeNull] PlayFabAuthenticator.CachePlayFabIdResponse response)
		{
			if (response != null)
			{
				this.steamAuthIdForPhoton = response.SteamAuthIdForPhoton;
				DateTime accountCreationDateTime;
				if (DateTime.TryParse(response.AccountCreationIsoTimestamp, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out accountCreationDateTime))
				{
					base.StartCoroutine(this.VerifyKidAuthenticated(accountCreationDateTime));
				}
				Debug.Log("Successfully cached PlayFab Id.  Continuing!");
				this.AdvanceLogin();
				return;
			}
			Debug.LogError("Could not cache PlayFab Id.  Cannot continue.");
		}

		// Token: 0x060056BB RID: 22203 RVA: 0x001AF0A2 File Offset: 0x001AD2A2
		private void AdvanceLogin()
		{
			this.LogMessage("PlayFab authenticated ... Getting Nonce");
			this.RefreshSteamAuthTicketForPhoton(delegate(string ticket)
			{
				this._nonce = ticket;
				Debug.Log("Got nonce!  Authenticating...");
				this.AuthenticateWithPhoton();
			}, delegate(EResult result)
			{
				Debug.LogWarning("Failed to get nonce!");
				this.AuthenticateWithPhoton();
			});
		}

		// Token: 0x060056BC RID: 22204 RVA: 0x001AF0D0 File Offset: 0x001AD2D0
		private void AuthenticateWithPhoton()
		{
			this.photonAuthenticator.SetCustomAuthenticationParameters(new Dictionary<string, object>
			{
				{
					"AppId",
					PlayFabSettings.TitleId
				},
				{
					"AppVersion",
					NetworkSystemConfig.AppVersion ?? "-1"
				},
				{
					"Ticket",
					this._sessionTicket
				},
				{
					"Nonce",
					this._nonce
				},
				{
					"MothershipEnvId",
					MothershipClientApiUnity.EnvironmentId
				},
				{
					"MothershipToken",
					MothershipClientContext.Token
				}
			});
			this.GetPlayerDisplayName(this._playFabPlayerIdCache);
			GorillaServer.Instance.AddOrRemoveDLCOwnership(delegate(ExecuteFunctionResult result)
			{
				Debug.Log("got results! updating!");
				if (GorillaTagger.Instance != null)
				{
					GorillaTagger.Instance.offlineVRRig.GetCosmeticsPlayFabCatalogData();
				}
			}, delegate(PlayFabError error)
			{
				Debug.Log("Got error retrieving user data:");
				Debug.Log(error.GenerateErrorReport());
				if (GorillaTagger.Instance != null)
				{
					GorillaTagger.Instance.offlineVRRig.GetCosmeticsPlayFabCatalogData();
				}
			});
			if (CosmeticsController.instance != null)
			{
				Debug.Log("initializing cosmetics");
				CosmeticsController.instance.Initialize();
			}
			if (this.gorillaComputer != null)
			{
				this.gorillaComputer.OnConnectedToMasterStuff();
			}
			else
			{
				base.StartCoroutine(this.ComputerOnConnectedToMaster());
			}
			if (RankedProgressionManager.Instance != null)
			{
				RankedProgressionManager.Instance.LoadStats();
			}
			if (PhotonNetworkController.Instance != null)
			{
				Debug.Log("Finish authenticating");
				NetworkSystem.Instance.FinishAuthenticating();
			}
		}

		// Token: 0x060056BD RID: 22205 RVA: 0x001AF235 File Offset: 0x001AD435
		private IEnumerator ComputerOnConnectedToMaster()
		{
			WaitForEndOfFrame frameYield = new WaitForEndOfFrame();
			while (this.gorillaComputer == null)
			{
				yield return frameYield;
			}
			this.gorillaComputer.OnConnectedToMasterStuff();
			yield break;
		}

		// Token: 0x060056BE RID: 22206 RVA: 0x001AF244 File Offset: 0x001AD444
		private void OnPlayFabError(PlayFabError obj)
		{
			this.LogMessage(obj.ErrorMessage);
			Debug.Log("OnPlayFabError(): " + obj.ErrorMessage);
			this.loginFailed = true;
			if (obj.ErrorMessage == "The account making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator = obj.ErrorDetails.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						return;
					}
					KeyValuePair<string, List<string>> keyValuePair = enumerator.Current;
					if (keyValuePair.Value[0] != "Indefinite")
					{
						this.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + keyValuePair.Key + "\nHOURS LEFT: " + ((int)((DateTime.Parse(keyValuePair.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString());
						return;
					}
					this.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED INDEFINITELY.\nREASON: " + keyValuePair.Key);
					return;
				}
			}
			if (obj.ErrorMessage == "The IP making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator = obj.ErrorDetails.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						return;
					}
					KeyValuePair<string, List<string>> keyValuePair2 = enumerator.Current;
					if (keyValuePair2.Value[0] != "Indefinite")
					{
						this.gorillaComputer.GeneralFailureMessage("THIS IP HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + keyValuePair2.Key + "\nHOURS LEFT: " + ((int)((DateTime.Parse(keyValuePair2.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString());
						return;
					}
					this.gorillaComputer.GeneralFailureMessage("THIS IP HAS BEEN BANNED INDEFINITELY.\nREASON: " + keyValuePair2.Key);
					return;
				}
			}
			if (this.gorillaComputer != null)
			{
				this.gorillaComputer.GeneralFailureMessage(this.gorillaComputer.unableToConnect);
			}
		}

		// Token: 0x060056BF RID: 22207 RVA: 0x000023F5 File Offset: 0x000005F5
		private void LogMessage(string message)
		{
		}

		// Token: 0x060056C0 RID: 22208 RVA: 0x001AF47C File Offset: 0x001AD67C
		private void GetPlayerDisplayName(string playFabId)
		{
			GetPlayerProfileRequest getPlayerProfileRequest = new GetPlayerProfileRequest();
			getPlayerProfileRequest.PlayFabId = playFabId;
			getPlayerProfileRequest.ProfileConstraints = new PlayerProfileViewConstraints
			{
				ShowDisplayName = true
			};
			PlayFabClientAPI.GetPlayerProfile(getPlayerProfileRequest, delegate(GetPlayerProfileResult result)
			{
				this._displayName = result.PlayerProfile.DisplayName;
			}, delegate(PlayFabError error)
			{
				Debug.LogError(error.GenerateErrorReport());
			}, null, null);
		}

		// Token: 0x060056C1 RID: 22209 RVA: 0x001AF4DC File Offset: 0x001AD6DC
		public void SetDisplayName(string playerName)
		{
			if (this._displayName == null || (this._displayName.Length > 4 && this._displayName.Substring(0, this._displayName.Length - 4) != playerName))
			{
				UpdateUserTitleDisplayNameRequest updateUserTitleDisplayNameRequest = new UpdateUserTitleDisplayNameRequest();
				updateUserTitleDisplayNameRequest.DisplayName = playerName;
				PlayFabClientAPI.UpdateUserTitleDisplayName(updateUserTitleDisplayNameRequest, delegate(UpdateUserTitleDisplayNameResult result)
				{
					this._displayName = playerName;
				}, delegate(PlayFabError error)
				{
					Debug.LogError(error.GenerateErrorReport());
				}, null, null);
			}
		}

		// Token: 0x060056C2 RID: 22210 RVA: 0x001AF57C File Offset: 0x001AD77C
		public void ScreenDebug(string debugString)
		{
			Debug.Log(debugString);
			if (this.screenDebugMode)
			{
				Text text = this.debugText;
				text.text = text.text + debugString + "\n";
			}
		}

		// Token: 0x060056C3 RID: 22211 RVA: 0x001AF5A8 File Offset: 0x001AD7A8
		public void ScreenDebugClear()
		{
			this.debugText.text = "";
		}

		// Token: 0x060056C4 RID: 22212 RVA: 0x001AF5BA File Offset: 0x001AD7BA
		public IEnumerator PlayfabAuthenticate(PlayFabAuthenticator.PlayfabAuthRequestData data, Action<PlayFabAuthenticator.PlayfabAuthResponseData> callback)
		{
			UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.AuthApiBaseUrl + "/api/PlayFabAuthentication", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			request.timeout = 15;
			yield return request.SendWebRequest();
			if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError)
			{
				PlayFabAuthenticator.PlayfabAuthResponseData obj = JsonUtility.FromJson<PlayFabAuthenticator.PlayfabAuthResponseData>(request.downloadHandler.text);
				callback(obj);
			}
			else
			{
				if (request.responseCode == 403L)
				{
					Debug.LogError(string.Format("HTTP {0}: {1}, with body: {2}", request.responseCode, request.error, request.downloadHandler.text));
					PlayFabAuthenticator.BanInfo banInfo = JsonUtility.FromJson<PlayFabAuthenticator.BanInfo>(request.downloadHandler.text);
					this.ShowBanMessage(banInfo);
					callback(null);
				}
				if (request.result == UnityWebRequest.Result.ProtocolError && request.responseCode != 400L)
				{
					retry = true;
					Debug.LogError(string.Format("HTTP {0} error: {1} message:{2}", request.responseCode, request.error, request.downloadHandler.text));
				}
				else if (request.result == UnityWebRequest.Result.ConnectionError)
				{
					retry = true;
					Debug.LogError("NETWORK ERROR: " + request.error + "\nMessage: " + request.downloadHandler.text);
				}
				else
				{
					Debug.LogError("HTTP ERROR: " + request.error + "\nMessage: " + request.downloadHandler.text);
					retry = true;
				}
			}
			if (retry)
			{
				if (this.playFabAuthRetryCount < this.playFabMaxRetries)
				{
					int num = (int)Mathf.Pow(2f, (float)(this.playFabAuthRetryCount + 1));
					Debug.LogWarning(string.Format("Retrying PlayFab auth... Retry attempt #{0}, waiting for {1} seconds", this.playFabAuthRetryCount + 1, num));
					this.playFabAuthRetryCount++;
					yield return new WaitForSeconds((float)num);
				}
				else
				{
					Debug.LogError("Maximum retries attempted. Please check your network connection.");
					callback(null);
					this.ShowPlayFabAuthErrorMessage(request.downloadHandler.text);
				}
			}
			yield break;
		}

		// Token: 0x060056C5 RID: 22213 RVA: 0x001AF5D8 File Offset: 0x001AD7D8
		private void ShowPlayFabAuthErrorMessage(string errorJson)
		{
			try
			{
				PlayFabAuthenticator.ErrorInfo errorInfo = JsonUtility.FromJson<PlayFabAuthenticator.ErrorInfo>(errorJson);
				this.gorillaComputer.GeneralFailureMessage("UNABLE TO AUTHENTICATE WITH PLAYFAB.\nREASON: " + errorInfo.Message);
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Failed to show PlayFab auth error message: {0}", arg));
			}
		}

		// Token: 0x060056C6 RID: 22214 RVA: 0x001AF62C File Offset: 0x001AD82C
		private void ShowBanMessage(PlayFabAuthenticator.BanInfo banInfo)
		{
			try
			{
				if (banInfo.BanExpirationTime != null && banInfo.BanMessage != null)
				{
					if (banInfo.BanExpirationTime != "Indefinite")
					{
						this.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + banInfo.BanMessage + "\nHOURS LEFT: " + ((int)((DateTime.Parse(banInfo.BanExpirationTime) - DateTime.UtcNow).TotalHours + 1.0)).ToString());
					}
					else
					{
						this.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED INDEFINITELY.\nREASON: " + banInfo.BanMessage);
					}
				}
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Failed to show ban message: {0}", arg));
			}
		}

		// Token: 0x060056C7 RID: 22215 RVA: 0x001AF6F0 File Offset: 0x001AD8F0
		public IEnumerator CachePlayFabId(PlayFabAuthenticator.CachePlayFabIdRequest data, Action<PlayFabAuthenticator.CachePlayFabIdResponse> callback)
		{
			Debug.Log("Trying to cache playfab Id");
			UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.AuthApiBaseUrl + "/api/CachePlayFabId", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			request.timeout = 15;
			yield return request.SendWebRequest();
			if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError)
			{
				if (request.responseCode == 200L)
				{
					PlayFabAuthenticator.CachePlayFabIdResponse obj = JsonUtility.FromJson<PlayFabAuthenticator.CachePlayFabIdResponse>(request.downloadHandler.text);
					callback(obj);
				}
			}
			else if (request.result == UnityWebRequest.Result.ProtocolError && request.responseCode != 400L)
			{
				retry = true;
				Debug.LogError(string.Format("HTTP {0} error: {1}", request.responseCode, request.error));
			}
			else
			{
				retry = (request.result != UnityWebRequest.Result.ConnectionError || true);
			}
			if (retry)
			{
				if (this.playFabCacheRetryCount < this.playFabCacheMaxRetries)
				{
					int num = (int)Mathf.Pow(2f, (float)(this.playFabCacheRetryCount + 1));
					Debug.LogWarning(string.Format("Retrying PlayFab auth... Retry attempt #{0}, waiting for {1} seconds", this.playFabCacheRetryCount + 1, num));
					this.playFabCacheRetryCount++;
					yield return new WaitForSeconds((float)num);
					base.StartCoroutine(this.CachePlayFabId(new PlayFabAuthenticator.CachePlayFabIdRequest
					{
						Platform = this.platform.ToString(),
						SessionTicket = this._sessionTicket,
						PlayFabId = this._playFabPlayerIdCache,
						TitleId = PlayFabSettings.TitleId,
						MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
						MothershipToken = MothershipClientContext.Token,
						MothershipId = MothershipClientContext.MothershipId
					}, new Action<PlayFabAuthenticator.CachePlayFabIdResponse>(this.OnCachePlayFabIdRequest)));
				}
				else
				{
					Debug.LogError("Maximum retries attempted. Please check your network connection.");
					callback(null);
					this.ShowPlayFabAuthErrorMessage(request.downloadHandler.text);
				}
			}
			yield break;
		}

		// Token: 0x060056C8 RID: 22216 RVA: 0x001AF70D File Offset: 0x001AD90D
		public void DefaultSafetiesByAgeCategory()
		{
			Debug.Log("[KID::PLAYFAB_AUTHENTICATOR] Defaulting Safety Settings to Disabled because age category data unavailable on this platform");
			this.SetSafety(false, true, false);
		}

		// Token: 0x060056C9 RID: 22217 RVA: 0x001AF724 File Offset: 0x001AD924
		public void SetSafety(bool isSafety, bool isAutoSet, bool setPlayfab = false)
		{
			this.postAuthSetSafety = false;
			Action<bool> onSafetyUpdate = this.OnSafetyUpdate;
			if (onSafetyUpdate != null)
			{
				onSafetyUpdate(isSafety);
			}
			Debug.Log("[KID] Setting safety to: [" + isSafety.ToString() + "]");
			this.isSafeAccount = isSafety;
			this.safetyType = PlayFabAuthenticator.SafetyType.None;
			if (!isSafety)
			{
				if (isAutoSet)
				{
					PlayerPrefs.SetInt("autoSafety", 0);
				}
				else
				{
					PlayerPrefs.SetInt("optSafety", 0);
				}
				PlayerPrefs.Save();
				return;
			}
			if (isAutoSet)
			{
				PlayerPrefs.SetInt("autoSafety", 1);
				this.safetyType = PlayFabAuthenticator.SafetyType.Auto;
				return;
			}
			PlayerPrefs.SetInt("optSafety", 1);
			this.safetyType = PlayFabAuthenticator.SafetyType.OptIn;
		}

		// Token: 0x060056CA RID: 22218 RVA: 0x001AF7BF File Offset: 0x001AD9BF
		public string GetPlayFabSessionTicket()
		{
			return this._sessionTicket;
		}

		// Token: 0x060056CB RID: 22219 RVA: 0x001AF7C7 File Offset: 0x001AD9C7
		public string GetPlayFabPlayerId()
		{
			return this._playFabPlayerIdCache;
		}

		// Token: 0x060056CC RID: 22220 RVA: 0x001AF7CF File Offset: 0x001AD9CF
		public bool GetSafety()
		{
			return this.isSafeAccount;
		}

		// Token: 0x060056CD RID: 22221 RVA: 0x001AF7D7 File Offset: 0x001AD9D7
		public PlayFabAuthenticator.SafetyType GetSafetyType()
		{
			return this.safetyType;
		}

		// Token: 0x060056CE RID: 22222 RVA: 0x001AF7DF File Offset: 0x001AD9DF
		public string GetUserID()
		{
			return this.userID;
		}

		// Token: 0x04006083 RID: 24707
		public static volatile PlayFabAuthenticator instance;

		// Token: 0x04006084 RID: 24708
		private string _playFabPlayerIdCache;

		// Token: 0x04006085 RID: 24709
		private string _sessionTicket;

		// Token: 0x04006086 RID: 24710
		private string _displayName;

		// Token: 0x04006087 RID: 24711
		private string _nonce;

		// Token: 0x04006088 RID: 24712
		public string userID;

		// Token: 0x04006089 RID: 24713
		private string userToken;

		// Token: 0x0400608A RID: 24714
		public PlatformTagJoin platform;

		// Token: 0x0400608B RID: 24715
		private bool isSafeAccount;

		// Token: 0x0400608C RID: 24716
		public Action<bool> OnSafetyUpdate;

		// Token: 0x0400608D RID: 24717
		private PlayFabAuthenticator.SafetyType safetyType;

		// Token: 0x0400608E RID: 24718
		private byte[] m_Ticket;

		// Token: 0x0400608F RID: 24719
		private uint m_pcbTicket;

		// Token: 0x04006090 RID: 24720
		public Text debugText;

		// Token: 0x04006091 RID: 24721
		public bool screenDebugMode;

		// Token: 0x04006092 RID: 24722
		public bool loginFailed;

		// Token: 0x04006093 RID: 24723
		[FormerlySerializedAs("loginDisplayID")]
		public GameObject emptyObject;

		// Token: 0x04006094 RID: 24724
		private int playFabAuthRetryCount;

		// Token: 0x04006095 RID: 24725
		private int playFabMaxRetries = 5;

		// Token: 0x04006096 RID: 24726
		private int playFabCacheRetryCount;

		// Token: 0x04006097 RID: 24727
		private int playFabCacheMaxRetries = 5;

		// Token: 0x04006098 RID: 24728
		public MetaAuthenticator metaAuthenticator;

		// Token: 0x04006099 RID: 24729
		public SteamAuthenticator steamAuthenticator;

		// Token: 0x0400609A RID: 24730
		public MothershipAuthenticator mothershipAuthenticator;

		// Token: 0x0400609B RID: 24731
		public PhotonAuthenticator photonAuthenticator;

		// Token: 0x0400609C RID: 24732
		[SerializeField]
		private bool dbg_isReturningPlayer;

		// Token: 0x0400609E RID: 24734
		private SteamAuthTicket steamAuthTicketForPlayFab;

		// Token: 0x0400609F RID: 24735
		private SteamAuthTicket steamAuthTicketForPhoton;

		// Token: 0x040060A0 RID: 24736
		private string steamAuthIdForPhoton;

		// Token: 0x02000DA2 RID: 3490
		public enum SafetyType
		{
			// Token: 0x040060A3 RID: 24739
			None,
			// Token: 0x040060A4 RID: 24740
			Auto,
			// Token: 0x040060A5 RID: 24741
			OptIn
		}

		// Token: 0x02000DA3 RID: 3491
		[Serializable]
		public class CachePlayFabIdRequest
		{
			// Token: 0x040060A6 RID: 24742
			public string Platform;

			// Token: 0x040060A7 RID: 24743
			public string SessionTicket;

			// Token: 0x040060A8 RID: 24744
			public string PlayFabId;

			// Token: 0x040060A9 RID: 24745
			public string TitleId;

			// Token: 0x040060AA RID: 24746
			public string MothershipEnvId;

			// Token: 0x040060AB RID: 24747
			public string MothershipToken;

			// Token: 0x040060AC RID: 24748
			public string MothershipId;
		}

		// Token: 0x02000DA4 RID: 3492
		[Serializable]
		public class PlayfabAuthRequestData
		{
			// Token: 0x040060AD RID: 24749
			public string AppId;

			// Token: 0x040060AE RID: 24750
			public string Nonce;

			// Token: 0x040060AF RID: 24751
			public string OculusId;

			// Token: 0x040060B0 RID: 24752
			public string Platform;

			// Token: 0x040060B1 RID: 24753
			public string AgeCategory;

			// Token: 0x040060B2 RID: 24754
			public string MothershipEnvId;

			// Token: 0x040060B3 RID: 24755
			public string MothershipToken;

			// Token: 0x040060B4 RID: 24756
			public string MothershipId;
		}

		// Token: 0x02000DA5 RID: 3493
		[Serializable]
		public class PlayfabAuthResponseData
		{
			// Token: 0x040060B5 RID: 24757
			public string SessionTicket;

			// Token: 0x040060B6 RID: 24758
			public string EntityToken;

			// Token: 0x040060B7 RID: 24759
			public string PlayFabId;

			// Token: 0x040060B8 RID: 24760
			public string EntityId;

			// Token: 0x040060B9 RID: 24761
			public string EntityType;

			// Token: 0x040060BA RID: 24762
			public string AccountCreationIsoTimestamp;
		}

		// Token: 0x02000DA6 RID: 3494
		[Serializable]
		public class CachePlayFabIdResponse
		{
			// Token: 0x040060BB RID: 24763
			public string PlayFabId;

			// Token: 0x040060BC RID: 24764
			public string SteamAuthIdForPhoton;

			// Token: 0x040060BD RID: 24765
			public string AccountCreationIsoTimestamp;
		}

		// Token: 0x02000DA7 RID: 3495
		private class ErrorInfo
		{
			// Token: 0x040060BE RID: 24766
			public string Message;

			// Token: 0x040060BF RID: 24767
			public string Error;
		}

		// Token: 0x02000DA8 RID: 3496
		private class BanInfo
		{
			// Token: 0x040060C0 RID: 24768
			public string BanMessage;

			// Token: 0x040060C1 RID: 24769
			public string BanExpirationTime;
		}
	}
}
