using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000D8E RID: 3470
	public class GorillaServer : MonoBehaviour, ISerializationCallbackReceiver
	{
		// Token: 0x1700083F RID: 2111
		// (get) Token: 0x0600563C RID: 22076 RVA: 0x001AC580 File Offset: 0x001AA780
		public bool FeatureFlagsReady
		{
			get
			{
				return this.featureFlags.ready;
			}
		}

		// Token: 0x17000840 RID: 2112
		// (get) Token: 0x0600563D RID: 22077 RVA: 0x001AC58D File Offset: 0x001AA78D
		private PlayFab.CloudScriptModels.EntityKey playerEntity
		{
			get
			{
				return new PlayFab.CloudScriptModels.EntityKey
				{
					Id = PlayFabSettings.staticPlayer.EntityId,
					Type = PlayFabSettings.staticPlayer.EntityType
				};
			}
		}

		// Token: 0x0600563E RID: 22078 RVA: 0x001AC5B4 File Offset: 0x001AA7B4
		public void Start()
		{
			this.featureFlags.FetchFeatureFlags();
		}

		// Token: 0x0600563F RID: 22079 RVA: 0x001AC5C1 File Offset: 0x001AA7C1
		private void Awake()
		{
			if (GorillaServer.Instance == null)
			{
				GorillaServer.Instance = this;
				return;
			}
			Object.Destroy(this);
		}

		// Token: 0x06005640 RID: 22080 RVA: 0x001AC5E4 File Offset: 0x001AA7E4
		public void ReturnCurrentVersion(ReturnCurrentVersionRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "ReturnCurrentVersion result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "ReturnCurrentVersion error");
			if (this.featureFlags.IsEnabledForUser("2024-05-ReturnCurrentVersionV2"))
			{
				Debug.Log("GorillaServer: ReturnCurrentVersion V2 call");
				PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
				{
					Entity = this.playerEntity,
					FunctionName = "ReturnCurrentVersionV2",
					FunctionParameter = request
				}, successCallback, errorCallback, null, null);
				return;
			}
			Debug.Log("GorillaServer: ReturnCurrentVersion LEGACY call");
			PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
			{
				FunctionName = "ReturnCurrentVersionNew",
				FunctionParameter = request
			}, delegate(PlayFab.ClientModels.ExecuteCloudScriptResult result)
			{
				successCallback(this.toFunctionResult(result));
			}, errorCallback, null, null);
		}

		// Token: 0x06005641 RID: 22081 RVA: 0x001AC6B0 File Offset: 0x001AA8B0
		public void ReturnMyOculusHash(Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "ReturnMyOculusHash result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "ReturnMyOculusHash error");
			if (this.featureFlags.IsEnabledForUser("2024-05-ReturnMyOculusHashV2"))
			{
				Debug.Log("GorillaServer: ReturnMyOculusHash V2 call");
				PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
				{
					Entity = this.playerEntity,
					FunctionName = "ReturnMyOculusHashV2",
					FunctionParameter = new
					{

					}
				}, successCallback, errorCallback, null, null);
				return;
			}
			Debug.Log("GorillaServer: ReturnMyOculusHash LEGACY call");
			PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
			{
				FunctionName = "ReturnMyOculusHash"
			}, delegate(PlayFab.ClientModels.ExecuteCloudScriptResult result)
			{
				successCallback(this.toFunctionResult(result));
			}, errorCallback, null, null);
		}

		// Token: 0x06005642 RID: 22082 RVA: 0x001AC778 File Offset: 0x001AA978
		public void TryDistributeCurrency(Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "TryDistributeCurrency result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "TryDistributeCurrency error");
			if (this.featureFlags.IsEnabledForUser("2024-05-TryDistributeCurrencyV2"))
			{
				Debug.Log("GorillaServer: TryDistributeCurrency V2 call");
				PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
				{
					Entity = this.playerEntity,
					FunctionName = "TryDistributeCurrencyV2",
					FunctionParameter = new
					{

					}
				}, successCallback, errorCallback, null, null);
				return;
			}
			Debug.Log("GorillaServer: TryDistributeCurrency LEGACY call");
			PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
			{
				FunctionName = "TryDistributeCurrency",
				FunctionParameter = new
				{

				}
			}, delegate(PlayFab.ClientModels.ExecuteCloudScriptResult result)
			{
				successCallback(this.toFunctionResult(result));
			}, errorCallback, null, null);
		}

		// Token: 0x06005643 RID: 22083 RVA: 0x001AC84C File Offset: 0x001AAA4C
		public void AddOrRemoveDLCOwnership(Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "AddOrRemoveDLCOwnership result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "AddOrRemoveDLCOwnership error");
			if (this.featureFlags.IsEnabledForUser("2024-05-AddOrRemoveDLCOwnershipV2"))
			{
				Debug.Log("GorillaServer: AddOrRemoveDLCOwnership V2 call");
				PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
				{
					Entity = this.playerEntity,
					FunctionName = "AddOrRemoveDLCOwnershipV2",
					FunctionParameter = new
					{

					}
				}, successCallback, errorCallback, null, null);
				return;
			}
			Debug.Log("GorillaServer: AddOrRemoveDLCOwnership LEGACY call");
			PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
			{
				FunctionName = "AddOrRemoveDLCOwnership",
				FunctionParameter = new
				{

				}
			}, delegate(PlayFab.ClientModels.ExecuteCloudScriptResult result)
			{
				successCallback(this.toFunctionResult(result));
			}, errorCallback, null, null);
		}

		// Token: 0x06005644 RID: 22084 RVA: 0x001AC920 File Offset: 0x001AAB20
		public void BroadcastMyRoom(BroadcastMyRoomRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "BroadcastMyRoom result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "BroadcastMyRoom error");
			if (this.featureFlags.IsEnabledForUser("2024-05-BroadcastMyRoomV2"))
			{
				Debug.Log(string.Format("GorillaServer: BroadcastMyRoom V2 call ({0})", request));
				PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
				{
					Entity = this.playerEntity,
					FunctionName = "BroadcastMyRoomV2",
					FunctionParameter = request
				}, successCallback, errorCallback, null, null);
				return;
			}
			Debug.Log(string.Format("GorillaServer: BroadcastMyRoom LEGACY call ({0})", request));
			PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
			{
				FunctionName = "BroadcastMyRoom",
				FunctionParameter = request
			}, delegate(PlayFab.ClientModels.ExecuteCloudScriptResult result)
			{
				successCallback(this.toFunctionResult(result));
			}, errorCallback, null, null);
		}

		// Token: 0x06005645 RID: 22085 RVA: 0x001AC9F5 File Offset: 0x001AABF5
		public bool NewCosmeticsPath()
		{
			return this.featureFlags.IsEnabledForUser("2024-06-CosmeticsAuthenticationV2");
		}

		// Token: 0x06005646 RID: 22086 RVA: 0x001ACA07 File Offset: 0x001AAC07
		public bool NewCosmeticsPathShouldSetSharedGroupData()
		{
			return this.featureFlags.IsEnabledForUser("2025-04-CosmeticsAuthenticationV2-SetData");
		}

		// Token: 0x06005647 RID: 22087 RVA: 0x001ACA19 File Offset: 0x001AAC19
		public bool NewCosmeticsPathShouldReadSharedGroupData()
		{
			return this.featureFlags.IsEnabledForUser("2025-04-CosmeticsAuthenticationV2-ReadData");
		}

		// Token: 0x06005648 RID: 22088 RVA: 0x001ACA2B File Offset: 0x001AAC2B
		public bool NewCosmeticsPathShouldSetRoomData()
		{
			return this.featureFlags.IsEnabledForUser("2025-04-CosmeticsAuthenticationV2-Compat");
		}

		// Token: 0x06005649 RID: 22089 RVA: 0x001ACA40 File Offset: 0x001AAC40
		public void UpdateUserCosmetics()
		{
			ExecuteFunctionRequest executeFunctionRequest = new ExecuteFunctionRequest();
			executeFunctionRequest.Entity = this.playerEntity;
			executeFunctionRequest.FunctionName = "UpdatePersonalCosmeticsList";
			executeFunctionRequest.FunctionParameter = new
			{

			};
			executeFunctionRequest.GeneratePlayStreamEvent = new bool?(false);
			PlayFabCloudScriptAPI.ExecuteFunction(executeFunctionRequest, delegate(ExecuteFunctionResult result)
			{
				if (CosmeticsController.instance != null)
				{
					CosmeticsController.instance.CheckCosmeticsSharedGroup();
				}
			}, delegate(PlayFabError error)
			{
			}, null, null);
		}

		// Token: 0x0600564A RID: 22090 RVA: 0x001ACAC8 File Offset: 0x001AACC8
		public void GetAcceptedAgreements(GetAcceptedAgreementsRequest request, Action<Dictionary<string, string>> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<Dictionary<string, string>>(successCallback, "GetAcceptedAgreements result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "GetAcceptedAgreements json error");
			Debug.Log(string.Format("GorillaServer: GetAcceptedAgreements call ({0})", request));
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "GetAcceptedAgreements",
				FunctionParameter = string.Join(",", request.AgreementKeys),
				GeneratePlayStreamEvent = new bool?(false)
			}, delegate(ExecuteFunctionResult result)
			{
				try
				{
					string value = Convert.ToString(result.FunctionResult);
					successCallback(JsonConvert.DeserializeObject<Dictionary<string, string>>(value));
				}
				catch (Exception arg)
				{
					errorCallback(new PlayFabError
					{
						ErrorMessage = string.Format("Invalid format for GetAcceptedAgreements ({0})", arg),
						Error = PlayFabErrorCode.JsonParseError
					});
				}
			}, errorCallback, null, null);
		}

		// Token: 0x0600564B RID: 22091 RVA: 0x001ACB80 File Offset: 0x001AAD80
		public void SubmitAcceptedAgreements(SubmitAcceptedAgreementsRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "SubmitAcceptedAgreements result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "SubmitAcceptedAgreements error");
			Debug.Log(string.Format("GorillaServer: SubmitAcceptedAgreements call ({0})", request));
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "SubmitAcceptedAgreements",
				FunctionParameter = request.Agreements,
				GeneratePlayStreamEvent = new bool?(false)
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x0600564C RID: 22092 RVA: 0x001ACBF8 File Offset: 0x001AADF8
		public void UploadGorillanalytics(object uploadData)
		{
			Debug.Log(string.Format("GorillaServer: UploadGorillanalytics call ({0})", uploadData));
			ExecuteFunctionRequest executeFunctionRequest = new ExecuteFunctionRequest();
			executeFunctionRequest.Entity = this.playerEntity;
			executeFunctionRequest.FunctionName = "Gorillanalytics";
			executeFunctionRequest.FunctionParameter = uploadData;
			executeFunctionRequest.GeneratePlayStreamEvent = new bool?(false);
			PlayFabCloudScriptAPI.ExecuteFunction(executeFunctionRequest, delegate(ExecuteFunctionResult result)
			{
				Debug.Log(string.Format("The {0} function took {1} to complete", result.FunctionName, result.ExecutionTimeMilliseconds));
			}, delegate(PlayFabError error)
			{
				Debug.Log("Error uploading Gorillanalytics: " + error.GenerateErrorReport());
			}, null, null);
		}

		// Token: 0x0600564D RID: 22093 RVA: 0x001ACC8C File Offset: 0x001AAE8C
		public void CheckForBadName(CheckForBadNameRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "CheckForBadName result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "CheckForBadName error");
			Debug.Log(string.Format("GorillaServer: CheckForBadName call ({0})", request));
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "CheckForBadName",
				FunctionParameter = new
				{
					name = request.name,
					forRoom = request.forRoom.ToString()
				},
				GeneratePlayStreamEvent = new bool?(false)
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x0600564E RID: 22094 RVA: 0x001ACD14 File Offset: 0x001AAF14
		public void GetRandomName(Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "GetRandomName result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "GetRandomName error");
			Debug.Log("GorillaServer: GetRandomName call");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "GetRandomName",
				GeneratePlayStreamEvent = new bool?(false)
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x0600564F RID: 22095 RVA: 0x001ACD78 File Offset: 0x001AAF78
		public void ReturnQueueStats(ReturnQueueStatsRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "ReturnQueueStats result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "ReturnQueueStats error");
			Debug.Log("GorillaServer: ReturnQueueStats call");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "ReturnQueueStats",
				FunctionParameter = new
				{
					QueueName = request.queueName
				},
				GeneratePlayStreamEvent = new bool?(false)
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x06005650 RID: 22096 RVA: 0x001ACDED File Offset: 0x001AAFED
		private Action<T> DebugWrapCb<T>(Action<T> cb, string label)
		{
			return delegate(T arg)
			{
				if (this.debug)
				{
					try
					{
						Debug.Log(string.Concat(new string[]
						{
							"GorillaServer: ",
							label,
							" (",
							JsonConvert.SerializeObject(arg, this.serializationSettings),
							")"
						}));
					}
					catch (Exception arg2)
					{
						Debug.LogError(string.Format("GorillaServer: {0} Error printing failure log: {1}", label, arg2));
					}
				}
				cb(arg);
			};
		}

		// Token: 0x06005651 RID: 22097 RVA: 0x001ACE14 File Offset: 0x001AB014
		private ExecuteFunctionResult toFunctionResult(PlayFab.ClientModels.ExecuteCloudScriptResult csResult)
		{
			FunctionExecutionError error = null;
			if (csResult.Error != null)
			{
				error = new FunctionExecutionError
				{
					Error = csResult.Error.Error,
					Message = csResult.Error.Message,
					StackTrace = csResult.Error.StackTrace
				};
			}
			return new ExecuteFunctionResult
			{
				CustomData = csResult.CustomData,
				Error = error,
				ExecutionTimeMilliseconds = Convert.ToInt32(Math.Round(csResult.ExecutionTimeSeconds * 1000.0)),
				FunctionName = csResult.FunctionName,
				FunctionResult = csResult.FunctionResult,
				FunctionResultTooLarge = csResult.FunctionResultTooLarge
			};
		}

		// Token: 0x06005652 RID: 22098 RVA: 0x001ACEC0 File Offset: 0x001AB0C0
		public void OnBeforeSerialize()
		{
			this.FeatureFlagsTitleDataKey = this.featureFlags.TitleDataKey;
			this.DefaultDeployFeatureFlagsEnabled.Clear();
			foreach (KeyValuePair<string, bool> keyValuePair in this.featureFlags.defaults)
			{
				if (keyValuePair.Value)
				{
					this.DefaultDeployFeatureFlagsEnabled.Add(keyValuePair.Key);
				}
			}
		}

		// Token: 0x06005653 RID: 22099 RVA: 0x001ACF48 File Offset: 0x001AB148
		public void OnAfterDeserialize()
		{
			this.featureFlags.TitleDataKey = this.FeatureFlagsTitleDataKey;
			foreach (string key in this.DefaultDeployFeatureFlagsEnabled)
			{
				this.featureFlags.defaults.AddOrUpdate(key, true);
			}
		}

		// Token: 0x06005654 RID: 22100 RVA: 0x001ACFB8 File Offset: 0x001AB1B8
		public bool CheckIsInKIDOptInCohort()
		{
			return this.featureFlags.IsEnabledForUser("2025-04-KIDOptIn");
		}

		// Token: 0x06005655 RID: 22101 RVA: 0x001ACFCA File Offset: 0x001AB1CA
		public bool CheckIsInKIDRequiredCohort()
		{
			return this.featureFlags.IsEnabledForUser("2025-04-KIDRequired");
		}

		// Token: 0x06005656 RID: 22102 RVA: 0x001ACFDC File Offset: 0x001AB1DC
		public bool CheckOptedInKID()
		{
			return KIDManager.HasOptedInToKID;
		}

		// Token: 0x04006005 RID: 24581
		public static volatile GorillaServer Instance;

		// Token: 0x04006006 RID: 24582
		public string FeatureFlagsTitleDataKey = "DeployFeatureFlags";

		// Token: 0x04006007 RID: 24583
		public List<string> DefaultDeployFeatureFlagsEnabled = new List<string>();

		// Token: 0x04006008 RID: 24584
		private TitleDataFeatureFlags featureFlags = new TitleDataFeatureFlags();

		// Token: 0x04006009 RID: 24585
		private bool debug;

		// Token: 0x0400600A RID: 24586
		private JsonSerializerSettings serializationSettings = new JsonSerializerSettings
		{
			NullValueHandling = NullValueHandling.Ignore,
			DefaultValueHandling = DefaultValueHandling.Ignore,
			MissingMemberHandling = MissingMemberHandling.Ignore,
			ObjectCreationHandling = ObjectCreationHandling.Replace,
			ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
			TypeNameHandling = TypeNameHandling.Auto
		};
	}
}
