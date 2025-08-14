using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using GorillaGameModes;
using GorillaTag;
using GorillaTag.Audio;
using Photon.Realtime;
using Photon.Voice.Fusion;
using Photon.Voice.Unity;
using PlayFab;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020002C0 RID: 704
public class NetworkSystemFusion : NetworkSystem
{
	// Token: 0x170001A0 RID: 416
	// (get) Token: 0x06001068 RID: 4200 RVA: 0x0005E092 File Offset: 0x0005C292
	// (set) Token: 0x06001069 RID: 4201 RVA: 0x0005E09A File Offset: 0x0005C29A
	public NetworkRunner runner { get; private set; }

	// Token: 0x170001A1 RID: 417
	// (get) Token: 0x0600106A RID: 4202 RVA: 0x0005E0A3 File Offset: 0x0005C2A3
	public override bool IsOnline
	{
		get
		{
			return this.runner != null && !this.runner.IsSinglePlayer;
		}
	}

	// Token: 0x170001A2 RID: 418
	// (get) Token: 0x0600106B RID: 4203 RVA: 0x0005E0C3 File Offset: 0x0005C2C3
	public override bool InRoom
	{
		get
		{
			return this.runner != null && this.runner.State != NetworkRunner.States.Shutdown && !this.runner.IsSinglePlayer && this.runner.IsConnectedToServer;
		}
	}

	// Token: 0x170001A3 RID: 419
	// (get) Token: 0x0600106C RID: 4204 RVA: 0x0005E0FB File Offset: 0x0005C2FB
	public override string RoomName
	{
		get
		{
			SessionInfo sessionInfo = this.runner.SessionInfo;
			if (sessionInfo == null)
			{
				return null;
			}
			return sessionInfo.Name;
		}
	}

	// Token: 0x0600106D RID: 4205 RVA: 0x0005E114 File Offset: 0x0005C314
	public override string RoomStringStripped()
	{
		SessionInfo sessionInfo = this.runner.SessionInfo;
		NetworkSystem.reusableSB.Clear();
		NetworkSystem.reusableSB.AppendFormat("Room: '{0}' ", (sessionInfo.Name.Length < 20) ? sessionInfo.Name : sessionInfo.Name.Remove(20));
		NetworkSystem.reusableSB.AppendFormat("{0},{1} {3}/{2} players.", new object[]
		{
			sessionInfo.IsVisible ? "visible" : "hidden",
			sessionInfo.IsOpen ? "open" : "closed",
			sessionInfo.MaxPlayers,
			sessionInfo.PlayerCount
		});
		NetworkSystem.reusableSB.Append("\ncustomProps: {");
		NetworkSystem.reusableSB.AppendFormat("joinedGameMode={0}, ", (RoomSystem.RoomGameMode.Length < 50) ? RoomSystem.RoomGameMode : RoomSystem.RoomGameMode.Remove(50));
		IDictionary properties = sessionInfo.Properties;
		Debug.Log(RoomSystem.RoomGameMode.ToString());
		if (properties.Contains("gameMode"))
		{
			object obj = properties["gameMode"];
			if (obj == null)
			{
				NetworkSystem.reusableSB.AppendFormat("gameMode=null}", Array.Empty<object>());
			}
			else
			{
				string text = obj as string;
				if (text != null)
				{
					NetworkSystem.reusableSB.AppendFormat("gameMode={0}", (text.Length < 50) ? text : text.Remove(50));
				}
			}
		}
		NetworkSystem.reusableSB.Append("}");
		Debug.Log(NetworkSystem.reusableSB.ToString());
		return NetworkSystem.reusableSB.ToString();
	}

	// Token: 0x170001A4 RID: 420
	// (get) Token: 0x0600106E RID: 4206 RVA: 0x0005E2B0 File Offset: 0x0005C4B0
	public override string GameModeString
	{
		get
		{
			SessionProperty sessionProperty;
			this.runner.SessionInfo.Properties.TryGetValue("gameMode", out sessionProperty);
			if (sessionProperty != null)
			{
				return (string)sessionProperty.PropertyValue;
			}
			return null;
		}
	}

	// Token: 0x170001A5 RID: 421
	// (get) Token: 0x0600106F RID: 4207 RVA: 0x0005E2EA File Offset: 0x0005C4EA
	public override string CurrentRegion
	{
		get
		{
			SessionInfo sessionInfo = this.runner.SessionInfo;
			if (sessionInfo == null)
			{
				return null;
			}
			return sessionInfo.Region;
		}
	}

	// Token: 0x170001A6 RID: 422
	// (get) Token: 0x06001070 RID: 4208 RVA: 0x0005E304 File Offset: 0x0005C504
	public override bool SessionIsPrivate
	{
		get
		{
			NetworkRunner runner = this.runner;
			bool? flag;
			if (runner == null)
			{
				flag = null;
			}
			else
			{
				SessionInfo sessionInfo = runner.SessionInfo;
				flag = ((sessionInfo != null) ? new bool?(!sessionInfo.IsVisible) : null);
			}
			bool? flag2 = flag;
			return flag2.GetValueOrDefault();
		}
	}

	// Token: 0x170001A7 RID: 423
	// (get) Token: 0x06001071 RID: 4209 RVA: 0x0005E350 File Offset: 0x0005C550
	public override int LocalPlayerID
	{
		get
		{
			return this.runner.LocalPlayer.PlayerId;
		}
	}

	// Token: 0x170001A8 RID: 424
	// (get) Token: 0x06001072 RID: 4210 RVA: 0x0005E370 File Offset: 0x0005C570
	public override string CurrentPhotonBackend
	{
		get
		{
			return "Fusion";
		}
	}

	// Token: 0x170001A9 RID: 425
	// (get) Token: 0x06001073 RID: 4211 RVA: 0x0005E377 File Offset: 0x0005C577
	public override double SimTime
	{
		get
		{
			return (double)this.runner.SimulationTime;
		}
	}

	// Token: 0x170001AA RID: 426
	// (get) Token: 0x06001074 RID: 4212 RVA: 0x0005E385 File Offset: 0x0005C585
	public override float SimDeltaTime
	{
		get
		{
			return this.runner.DeltaTime;
		}
	}

	// Token: 0x170001AB RID: 427
	// (get) Token: 0x06001075 RID: 4213 RVA: 0x0005E392 File Offset: 0x0005C592
	public override int SimTick
	{
		get
		{
			return this.runner.Tick.Raw;
		}
	}

	// Token: 0x170001AC RID: 428
	// (get) Token: 0x06001076 RID: 4214 RVA: 0x0005E3A4 File Offset: 0x0005C5A4
	public override int TickRate
	{
		get
		{
			return this.runner.TickRate;
		}
	}

	// Token: 0x170001AD RID: 429
	// (get) Token: 0x06001077 RID: 4215 RVA: 0x0005E392 File Offset: 0x0005C592
	public override int ServerTimestamp
	{
		get
		{
			return this.runner.Tick.Raw;
		}
	}

	// Token: 0x170001AE RID: 430
	// (get) Token: 0x06001078 RID: 4216 RVA: 0x0005E3B1 File Offset: 0x0005C5B1
	public override int RoomPlayerCount
	{
		get
		{
			return this.runner.SessionInfo.PlayerCount;
		}
	}

	// Token: 0x170001AF RID: 431
	// (get) Token: 0x06001079 RID: 4217 RVA: 0x0005E3C3 File Offset: 0x0005C5C3
	public override VoiceConnection VoiceConnection
	{
		get
		{
			return this.FusionVoice;
		}
	}

	// Token: 0x170001B0 RID: 432
	// (get) Token: 0x0600107A RID: 4218 RVA: 0x0005E3CB File Offset: 0x0005C5CB
	public override bool IsMasterClient
	{
		get
		{
			NetworkRunner runner = this.runner;
			return runner == null || runner.IsSharedModeMasterClient;
		}
	}

	// Token: 0x170001B1 RID: 433
	// (get) Token: 0x0600107B RID: 4219 RVA: 0x0005E3E0 File Offset: 0x0005C5E0
	public override NetPlayer MasterClient
	{
		get
		{
			if (this.runner != null && this.runner.IsSharedModeMasterClient)
			{
				return base.GetPlayer(this.runner.LocalPlayer);
			}
			if (!(GorillaGameModes.GameMode.ActiveNetworkHandler != null))
			{
				return null;
			}
			return base.GetPlayer(GorillaGameModes.GameMode.ActiveNetworkHandler.Object.StateAuthority);
		}
	}

	// Token: 0x0600107C RID: 4220 RVA: 0x0005E440 File Offset: 0x0005C640
	public override void Initialise()
	{
		NetworkSystemFusion.<Initialise>d__54 <Initialise>d__;
		<Initialise>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Initialise>d__.<>4__this = this;
		<Initialise>d__.<>1__state = -1;
		<Initialise>d__.<>t__builder.Start<NetworkSystemFusion.<Initialise>d__54>(ref <Initialise>d__);
	}

	// Token: 0x0600107D RID: 4221 RVA: 0x0005E478 File Offset: 0x0005C678
	private void CreateRegionCrawler()
	{
		GameObject gameObject = new GameObject("[Network Crawler]");
		gameObject.transform.SetParent(base.transform);
		this.regionCrawler = gameObject.AddComponent<FusionRegionCrawler>();
	}

	// Token: 0x0600107E RID: 4222 RVA: 0x0005E4B0 File Offset: 0x0005C6B0
	private Task AwaitAuth()
	{
		NetworkSystemFusion.<AwaitAuth>d__56 <AwaitAuth>d__;
		<AwaitAuth>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<AwaitAuth>d__.<>4__this = this;
		<AwaitAuth>d__.<>1__state = -1;
		<AwaitAuth>d__.<>t__builder.Start<NetworkSystemFusion.<AwaitAuth>d__56>(ref <AwaitAuth>d__);
		return <AwaitAuth>d__.<>t__builder.Task;
	}

	// Token: 0x0600107F RID: 4223 RVA: 0x0005E4F3 File Offset: 0x0005C6F3
	public override void FinishAuthenticating()
	{
		if (this.cachedPlayfabAuth != null)
		{
			Debug.Log("AUTHED");
			return;
		}
		Debug.LogError("Authentication Failed");
	}

	// Token: 0x06001080 RID: 4224 RVA: 0x0005E514 File Offset: 0x0005C714
	public override Task<NetJoinResult> ConnectToRoom(string roomName, RoomConfig opts, int regionIndex = -1)
	{
		NetworkSystemFusion.<ConnectToRoom>d__58 <ConnectToRoom>d__;
		<ConnectToRoom>d__.<>t__builder = AsyncTaskMethodBuilder<NetJoinResult>.Create();
		<ConnectToRoom>d__.<>4__this = this;
		<ConnectToRoom>d__.roomName = roomName;
		<ConnectToRoom>d__.opts = opts;
		<ConnectToRoom>d__.<>1__state = -1;
		<ConnectToRoom>d__.<>t__builder.Start<NetworkSystemFusion.<ConnectToRoom>d__58>(ref <ConnectToRoom>d__);
		return <ConnectToRoom>d__.<>t__builder.Task;
	}

	// Token: 0x06001081 RID: 4225 RVA: 0x0005E568 File Offset: 0x0005C768
	private Task<bool> Connect(Fusion.GameMode mode, string targetSessionName, RoomConfig opts)
	{
		NetworkSystemFusion.<Connect>d__59 <Connect>d__;
		<Connect>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<Connect>d__.<>4__this = this;
		<Connect>d__.mode = mode;
		<Connect>d__.targetSessionName = targetSessionName;
		<Connect>d__.opts = opts;
		<Connect>d__.<>1__state = -1;
		<Connect>d__.<>t__builder.Start<NetworkSystemFusion.<Connect>d__59>(ref <Connect>d__);
		return <Connect>d__.<>t__builder.Task;
	}

	// Token: 0x06001082 RID: 4226 RVA: 0x0005E5C4 File Offset: 0x0005C7C4
	private Task<NetJoinResult> MakeOrJoinRoom(string roomName, RoomConfig opts)
	{
		NetworkSystemFusion.<MakeOrJoinRoom>d__60 <MakeOrJoinRoom>d__;
		<MakeOrJoinRoom>d__.<>t__builder = AsyncTaskMethodBuilder<NetJoinResult>.Create();
		<MakeOrJoinRoom>d__.<>4__this = this;
		<MakeOrJoinRoom>d__.roomName = roomName;
		<MakeOrJoinRoom>d__.opts = opts;
		<MakeOrJoinRoom>d__.<>1__state = -1;
		<MakeOrJoinRoom>d__.<>t__builder.Start<NetworkSystemFusion.<MakeOrJoinRoom>d__60>(ref <MakeOrJoinRoom>d__);
		return <MakeOrJoinRoom>d__.<>t__builder.Task;
	}

	// Token: 0x06001083 RID: 4227 RVA: 0x0005E618 File Offset: 0x0005C818
	private Task<NetJoinResult> JoinRandomPublicRoom(RoomConfig opts)
	{
		NetworkSystemFusion.<JoinRandomPublicRoom>d__61 <JoinRandomPublicRoom>d__;
		<JoinRandomPublicRoom>d__.<>t__builder = AsyncTaskMethodBuilder<NetJoinResult>.Create();
		<JoinRandomPublicRoom>d__.<>4__this = this;
		<JoinRandomPublicRoom>d__.opts = opts;
		<JoinRandomPublicRoom>d__.<>1__state = -1;
		<JoinRandomPublicRoom>d__.<>t__builder.Start<NetworkSystemFusion.<JoinRandomPublicRoom>d__61>(ref <JoinRandomPublicRoom>d__);
		return <JoinRandomPublicRoom>d__.<>t__builder.Task;
	}

	// Token: 0x06001084 RID: 4228 RVA: 0x0005E664 File Offset: 0x0005C864
	public override Task JoinFriendsRoom(string userID, int actorIDToFollow, string keyToFollow, string shufflerToFollow)
	{
		NetworkSystemFusion.<JoinFriendsRoom>d__62 <JoinFriendsRoom>d__;
		<JoinFriendsRoom>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<JoinFriendsRoom>d__.<>4__this = this;
		<JoinFriendsRoom>d__.userID = userID;
		<JoinFriendsRoom>d__.actorIDToFollow = actorIDToFollow;
		<JoinFriendsRoom>d__.keyToFollow = keyToFollow;
		<JoinFriendsRoom>d__.shufflerToFollow = shufflerToFollow;
		<JoinFriendsRoom>d__.<>1__state = -1;
		<JoinFriendsRoom>d__.<>t__builder.Start<NetworkSystemFusion.<JoinFriendsRoom>d__62>(ref <JoinFriendsRoom>d__);
		return <JoinFriendsRoom>d__.<>t__builder.Task;
	}

	// Token: 0x06001085 RID: 4229 RVA: 0x00002628 File Offset: 0x00000828
	public override void JoinPubWithFriends()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06001086 RID: 4230 RVA: 0x0005E6C8 File Offset: 0x0005C8C8
	public override Task ReturnToSinglePlayer()
	{
		NetworkSystemFusion.<ReturnToSinglePlayer>d__64 <ReturnToSinglePlayer>d__;
		<ReturnToSinglePlayer>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<ReturnToSinglePlayer>d__.<>4__this = this;
		<ReturnToSinglePlayer>d__.<>1__state = -1;
		<ReturnToSinglePlayer>d__.<>t__builder.Start<NetworkSystemFusion.<ReturnToSinglePlayer>d__64>(ref <ReturnToSinglePlayer>d__);
		return <ReturnToSinglePlayer>d__.<>t__builder.Task;
	}

	// Token: 0x06001087 RID: 4231 RVA: 0x0005E70C File Offset: 0x0005C90C
	private Task CloseRunner(ShutdownReason reason = ShutdownReason.Ok)
	{
		NetworkSystemFusion.<CloseRunner>d__65 <CloseRunner>d__;
		<CloseRunner>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<CloseRunner>d__.<>4__this = this;
		<CloseRunner>d__.reason = reason;
		<CloseRunner>d__.<>1__state = -1;
		<CloseRunner>d__.<>t__builder.Start<NetworkSystemFusion.<CloseRunner>d__65>(ref <CloseRunner>d__);
		return <CloseRunner>d__.<>t__builder.Task;
	}

	// Token: 0x06001088 RID: 4232 RVA: 0x0005E758 File Offset: 0x0005C958
	public void MigrateHost(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
		NetworkSystemFusion.<MigrateHost>d__66 <MigrateHost>d__;
		<MigrateHost>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<MigrateHost>d__.<>4__this = this;
		<MigrateHost>d__.<>1__state = -1;
		<MigrateHost>d__.<>t__builder.Start<NetworkSystemFusion.<MigrateHost>d__66>(ref <MigrateHost>d__);
	}

	// Token: 0x06001089 RID: 4233 RVA: 0x0005E790 File Offset: 0x0005C990
	public void ResetSystem()
	{
		NetworkSystemFusion.<ResetSystem>d__67 <ResetSystem>d__;
		<ResetSystem>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<ResetSystem>d__.<>4__this = this;
		<ResetSystem>d__.<>1__state = -1;
		<ResetSystem>d__.<>t__builder.Start<NetworkSystemFusion.<ResetSystem>d__67>(ref <ResetSystem>d__);
	}

	// Token: 0x0600108A RID: 4234 RVA: 0x0005E7C7 File Offset: 0x0005C9C7
	private void AddVoice()
	{
		this.SetupVoice();
		this.FusionVoiceBridge = this.volatileNetObj.AddComponent<FusionVoiceBridge>();
	}

	// Token: 0x0600108B RID: 4235 RVA: 0x0005E7E0 File Offset: 0x0005C9E0
	private void SetupVoice()
	{
		Utils.Log("<color=orange>Adding Voice Stuff</color>");
		this.FusionVoice = this.volatileNetObj.AddComponent<VoiceConnection>();
		this.FusionVoice.LogLevel = this.VoiceSettings.LogLevel;
		this.FusionVoice.GlobalRecordersLogLevel = this.VoiceSettings.GlobalRecordersLogLevel;
		this.FusionVoice.GlobalSpeakersLogLevel = this.VoiceSettings.GlobalSpeakersLogLevel;
		this.FusionVoice.AutoCreateSpeakerIfNotFound = this.VoiceSettings.CreateSpeakerIfNotFound;
		Photon.Realtime.AppSettings appSettings = new Photon.Realtime.AppSettings();
		appSettings.AppIdFusion = PhotonAppSettings.Global.AppSettings.AppIdFusion;
		appSettings.AppIdVoice = PhotonAppSettings.Global.AppSettings.AppIdVoice;
		this.FusionVoice.Settings = appSettings;
		this.remoteVoiceAddedCallbacks.ForEach(delegate(Action<RemoteVoiceLink> callback)
		{
			this.FusionVoice.RemoteVoiceAdded += callback;
		});
		this.localRecorder = this.volatileNetObj.AddComponent<Recorder>();
		this.localRecorder.LogLevel = this.VoiceSettings.LogLevel;
		this.localRecorder.RecordOnlyWhenEnabled = this.VoiceSettings.RecordOnlyWhenEnabled;
		this.localRecorder.RecordOnlyWhenJoined = this.VoiceSettings.RecordOnlyWhenJoined;
		this.localRecorder.StopRecordingWhenPaused = this.VoiceSettings.StopRecordingWhenPaused;
		this.localRecorder.TransmitEnabled = this.VoiceSettings.TransmitEnabled;
		this.localRecorder.AutoStart = this.VoiceSettings.AutoStart;
		this.localRecorder.Encrypt = this.VoiceSettings.Encrypt;
		this.localRecorder.FrameDuration = this.VoiceSettings.FrameDuration;
		this.localRecorder.SamplingRate = this.VoiceSettings.SamplingRate;
		this.localRecorder.InterestGroup = this.VoiceSettings.InterestGroup;
		this.localRecorder.SourceType = this.VoiceSettings.InputSourceType;
		this.localRecorder.MicrophoneType = this.VoiceSettings.MicrophoneType;
		this.localRecorder.UseMicrophoneTypeFallback = this.VoiceSettings.UseFallback;
		this.localRecorder.VoiceDetection = this.VoiceSettings.Detect;
		this.localRecorder.VoiceDetectionThreshold = this.VoiceSettings.Threshold;
		this.localRecorder.Bitrate = this.VoiceSettings.Bitrate;
		this.localRecorder.VoiceDetectionDelayMs = this.VoiceSettings.Delay;
		this.localRecorder.DebugEchoMode = this.VoiceSettings.DebugEcho;
		this.localRecorder.UserData = this.runner.UserId;
		this.FusionVoice.PrimaryRecorder = this.localRecorder;
		this.volatileNetObj.AddComponent<VoiceToLoudness>();
	}

	// Token: 0x0600108C RID: 4236 RVA: 0x0005EA83 File Offset: 0x0005CC83
	public override void AddRemoteVoiceAddedCallback(Action<RemoteVoiceLink> callback)
	{
		this.remoteVoiceAddedCallbacks.Add(callback);
	}

	// Token: 0x0600108D RID: 4237 RVA: 0x0005EA91 File Offset: 0x0005CC91
	private void AttachCallbackTargets()
	{
		this.runner.AddCallbacks(this.objectsThatNeedCallbacks.ToArray());
	}

	// Token: 0x0600108E RID: 4238 RVA: 0x0005EAA9 File Offset: 0x0005CCA9
	public void RegisterForNetworkCallbacks(INetworkRunnerCallbacks callbacks)
	{
		if (!this.objectsThatNeedCallbacks.Contains(callbacks))
		{
			this.objectsThatNeedCallbacks.Add(callbacks);
		}
		if (this.runner != null)
		{
			this.runner.AddCallbacks(new INetworkRunnerCallbacks[]
			{
				callbacks
			});
		}
	}

	// Token: 0x0600108F RID: 4239 RVA: 0x0005EAE8 File Offset: 0x0005CCE8
	private void AttachSceneObjects(bool onlyCached = false)
	{
		NetworkSystemFusion.<AttachSceneObjects>d__75 <AttachSceneObjects>d__;
		<AttachSceneObjects>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<AttachSceneObjects>d__.<>4__this = this;
		<AttachSceneObjects>d__.onlyCached = onlyCached;
		<AttachSceneObjects>d__.<>1__state = -1;
		<AttachSceneObjects>d__.<>t__builder.Start<NetworkSystemFusion.<AttachSceneObjects>d__75>(ref <AttachSceneObjects>d__);
	}

	// Token: 0x06001090 RID: 4240 RVA: 0x0005EB28 File Offset: 0x0005CD28
	public override void AttachObjectInGame(GameObject item)
	{
		base.AttachObjectInGame(item);
		NetworkObject component = item.GetComponent<NetworkObject>();
		if ((component != null && !this.cachedNetSceneObjects.Contains(component)) || !component.IsValid)
		{
			this.cachedNetSceneObjects.AddIfNew(component);
			this.registrationQueue.Enqueue(component);
			this.ProcessRegistrationQueue();
		}
	}

	// Token: 0x06001091 RID: 4241 RVA: 0x0005EB80 File Offset: 0x0005CD80
	private void ProcessRegistrationQueue()
	{
		if (this.isProcessingQueue)
		{
			Debug.LogError("Queue is still processing");
			return;
		}
		this.isProcessingQueue = true;
		List<NetworkObject> list = new List<NetworkObject>();
		SceneRef scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
		while (this.registrationQueue.Count > 0)
		{
			NetworkObject networkObject = this.registrationQueue.Dequeue();
			if (this.InRoom && !networkObject.IsValid && !networkObject.Id.IsValid && networkObject.Runner == null)
			{
				try
				{
					list.Add(networkObject);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
					this.isProcessingQueue = false;
					this.runner.RegisterSceneObjects(scene, list.ToArray(), default(NetworkSceneLoadId));
					this.ProcessRegistrationQueue();
					break;
				}
			}
		}
		this.runner.RegisterSceneObjects(scene, list.ToArray(), default(NetworkSceneLoadId));
		this.isProcessingQueue = false;
	}

	// Token: 0x06001092 RID: 4242 RVA: 0x0005EC7C File Offset: 0x0005CE7C
	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject = false)
	{
		Utils.Log("Net instantiate Fusion: " + prefab.name);
		try
		{
			return this.runner.Spawn(prefab, new Vector3?(position), new Quaternion?(rotation), new PlayerRef?(this.runner.LocalPlayer), null, (NetworkSpawnFlags)0).gameObject;
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
		return null;
	}

	// Token: 0x06001093 RID: 4243 RVA: 0x0005ECEC File Offset: 0x0005CEEC
	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, int playerAuthID, bool isRoomObject = false)
	{
		foreach (PlayerRef value in this.runner.ActivePlayers)
		{
			if (value.PlayerId == playerAuthID)
			{
				Utils.Log("Net instantiate Fusion: " + prefab.name);
				return this.runner.Spawn(prefab, new Vector3?(position), new Quaternion?(rotation), new PlayerRef?(value), null, (NetworkSpawnFlags)0).gameObject;
			}
		}
		Debug.LogError(string.Format("Couldn't find player with ID: {0}, cancelling requested spawn...", playerAuthID));
		return null;
	}

	// Token: 0x06001094 RID: 4244 RVA: 0x0005ED98 File Offset: 0x0005CF98
	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject, byte group = 0, object[] data = null, NetworkRunner.OnBeforeSpawned callback = null)
	{
		Utils.Log("Net instantiate Fusion: " + prefab.name);
		return this.runner.Spawn(prefab, new Vector3?(position), new Quaternion?(rotation), new PlayerRef?(this.runner.LocalPlayer), callback, (NetworkSpawnFlags)0).gameObject;
	}

	// Token: 0x06001095 RID: 4245 RVA: 0x0005EDEC File Offset: 0x0005CFEC
	public override void NetDestroy(GameObject instance)
	{
		NetworkObject networkObject;
		if (instance.TryGetComponent<NetworkObject>(out networkObject))
		{
			this.runner.Despawn(networkObject);
			return;
		}
		Object.Destroy(instance);
	}

	// Token: 0x06001096 RID: 4246 RVA: 0x0005EE18 File Offset: 0x0005D018
	public override bool ShouldSpawnLocally(int playerID)
	{
		if (this.runner.GameMode == Fusion.GameMode.Shared)
		{
			return this.runner.LocalPlayer.PlayerId == playerID || (playerID == -1 && this.runner.IsSharedModeMasterClient);
		}
		return this.runner.GameMode != Fusion.GameMode.Client;
	}

	// Token: 0x06001097 RID: 4247 RVA: 0x0005EE70 File Offset: 0x0005D070
	public override void CallRPC(MonoBehaviour component, NetworkSystem.RPC rpcMethod, bool sendToSelf = true)
	{
		Utils.Log(rpcMethod.GetDelegateName() + "RPC called!");
		foreach (PlayerRef a in this.runner.ActivePlayers)
		{
			if (!sendToSelf)
			{
				a != this.runner.LocalPlayer;
			}
		}
	}

	// Token: 0x06001098 RID: 4248 RVA: 0x0005EEE8 File Offset: 0x0005D0E8
	public override void CallRPC<T>(MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args, bool sendToSelf = true)
	{
		Utils.Log(rpcMethod.GetDelegateName() + "RPC called!");
		ref args.SerializeToRPCData<T>();
		foreach (PlayerRef a in this.runner.ActivePlayers)
		{
			if (!sendToSelf)
			{
				a != this.runner.LocalPlayer;
			}
		}
	}

	// Token: 0x06001099 RID: 4249 RVA: 0x0005EF68 File Offset: 0x0005D168
	public override void CallRPC(MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message, bool sendToSelf = true)
	{
		foreach (PlayerRef a in this.runner.ActivePlayers)
		{
			if (!sendToSelf)
			{
				a != this.runner.LocalPlayer;
			}
		}
	}

	// Token: 0x0600109A RID: 4250 RVA: 0x0005EFCC File Offset: 0x0005D1CC
	public override void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod)
	{
		this.GetPlayerRef(targetPlayerID);
		Utils.Log(rpcMethod.GetDelegateName() + "RPC called!");
	}

	// Token: 0x0600109B RID: 4251 RVA: 0x0005EFEB File Offset: 0x0005D1EB
	public override void CallRPC<T>(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args)
	{
		Utils.Log(rpcMethod.GetDelegateName() + "RPC called!");
		this.GetPlayerRef(targetPlayerID);
	}

	// Token: 0x0600109C RID: 4252 RVA: 0x0005F00A File Offset: 0x0005D20A
	public override void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message)
	{
		this.GetPlayerRef(targetPlayerID);
	}

	// Token: 0x0600109D RID: 4253 RVA: 0x0005F014 File Offset: 0x0005D214
	public override void NetRaiseEventReliable(byte eventCode, object data)
	{
		byte[] byteData = data.ByteSerialize();
		FusionCallbackHandler.RPC_OnEventRaisedReliable(this.runner, eventCode, byteData, false, null, default(RpcInfo));
	}

	// Token: 0x0600109E RID: 4254 RVA: 0x0005F040 File Offset: 0x0005D240
	public override void NetRaiseEventUnreliable(byte eventCode, object data)
	{
		byte[] byteData = data.ByteSerialize();
		FusionCallbackHandler.RPC_OnEventRaisedUnreliable(this.runner, eventCode, byteData, false, null, default(RpcInfo));
	}

	// Token: 0x0600109F RID: 4255 RVA: 0x0005F06C File Offset: 0x0005D26C
	public override void NetRaiseEventReliable(byte eventCode, object data, NetEventOptions opts)
	{
		byte[] byteData = data.ByteSerialize();
		byte[] netOptsData = opts.ByteSerialize();
		FusionCallbackHandler.RPC_OnEventRaisedReliable(this.runner, eventCode, byteData, true, netOptsData, default(RpcInfo));
	}

	// Token: 0x060010A0 RID: 4256 RVA: 0x0005F0A0 File Offset: 0x0005D2A0
	public override void NetRaiseEventUnreliable(byte eventCode, object data, NetEventOptions opts)
	{
		byte[] byteData = data.ByteSerialize();
		byte[] netOptsData = opts.ByteSerialize();
		FusionCallbackHandler.RPC_OnEventRaisedUnreliable(this.runner, eventCode, byteData, true, netOptsData, default(RpcInfo));
	}

	// Token: 0x060010A1 RID: 4257 RVA: 0x00002628 File Offset: 0x00000828
	public override string GetRandomWeightedRegion()
	{
		throw new NotImplementedException();
	}

	// Token: 0x060010A2 RID: 4258 RVA: 0x0005F0D4 File Offset: 0x0005D2D4
	public override Task AwaitSceneReady()
	{
		NetworkSystemFusion.<AwaitSceneReady>d__94 <AwaitSceneReady>d__;
		<AwaitSceneReady>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<AwaitSceneReady>d__.<>4__this = this;
		<AwaitSceneReady>d__.<>1__state = -1;
		<AwaitSceneReady>d__.<>t__builder.Start<NetworkSystemFusion.<AwaitSceneReady>d__94>(ref <AwaitSceneReady>d__);
		return <AwaitSceneReady>d__.<>t__builder.Task;
	}

	// Token: 0x060010A3 RID: 4259 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnJoinedSession()
	{
	}

	// Token: 0x060010A4 RID: 4260 RVA: 0x0005F117 File Offset: 0x0005D317
	public void OnJoinFailed(NetConnectFailedReason reason)
	{
		switch (reason)
		{
		case NetConnectFailedReason.Timeout:
		case NetConnectFailedReason.ServerRefused:
			break;
		case NetConnectFailedReason.ServerFull:
			this.lastConnectAttempt_WasFull = true;
			break;
		default:
			return;
		}
	}

	// Token: 0x060010A5 RID: 4261 RVA: 0x0005F135 File Offset: 0x0005D335
	public void OnDisconnectedFromSession()
	{
		Utils.Log("On Disconnected");
		this.internalState = NetworkSystemFusion.InternalState.Disconnected;
		base.UpdatePlayers();
	}

	// Token: 0x060010A6 RID: 4262 RVA: 0x0005F14F File Offset: 0x0005D34F
	public void OnRunnerShutDown()
	{
		Utils.Log("Runner shutdown callback");
		if (this.internalState == NetworkSystemFusion.InternalState.Disconnecting)
		{
			this.internalState = NetworkSystemFusion.InternalState.Disconnected;
		}
	}

	// Token: 0x060010A7 RID: 4263 RVA: 0x0005F16D File Offset: 0x0005D36D
	public void OnFusionPlayerJoined(PlayerRef player)
	{
		this.AwaitJoiningPlayerClientReady(player);
	}

	// Token: 0x060010A8 RID: 4264 RVA: 0x0005F178 File Offset: 0x0005D378
	private Task AwaitJoiningPlayerClientReady(PlayerRef player)
	{
		NetworkSystemFusion.<AwaitJoiningPlayerClientReady>d__100 <AwaitJoiningPlayerClientReady>d__;
		<AwaitJoiningPlayerClientReady>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<AwaitJoiningPlayerClientReady>d__.<>4__this = this;
		<AwaitJoiningPlayerClientReady>d__.player = player;
		<AwaitJoiningPlayerClientReady>d__.<>1__state = -1;
		<AwaitJoiningPlayerClientReady>d__.<>t__builder.Start<NetworkSystemFusion.<AwaitJoiningPlayerClientReady>d__100>(ref <AwaitJoiningPlayerClientReady>d__);
		return <AwaitJoiningPlayerClientReady>d__.<>t__builder.Task;
	}

	// Token: 0x060010A9 RID: 4265 RVA: 0x0005F1C4 File Offset: 0x0005D3C4
	public void OnFusionPlayerLeft(PlayerRef player)
	{
		if (this.IsTotalAuthority())
		{
			NetworkObject playerObject = this.runner.GetPlayerObject(player);
			if (playerObject != null)
			{
				Utils.Log("Destroying player object for leaving player!");
				this.NetDestroy(playerObject.gameObject);
			}
			else
			{
				Utils.Log("Player left without destroying an avatar for it somehow?");
			}
		}
		NetPlayer player2 = base.GetPlayer(player);
		if (player2 == null)
		{
			Debug.LogError("Joining player doesnt have a NetPlayer somehow, this shouldnt happen");
		}
		base.PlayerLeft(player2);
		base.UpdatePlayers();
	}

	// Token: 0x060010AA RID: 4266 RVA: 0x0005F234 File Offset: 0x0005D434
	protected override void UpdateNetPlayerList()
	{
		if (this.runner == null)
		{
			if (this.netPlayerCache.Count <= 1)
			{
				if (this.netPlayerCache.Exists((NetPlayer p) => p.IsLocal))
				{
					goto IL_84;
				}
			}
			this.netPlayerCache.ForEach(delegate(NetPlayer p)
			{
				this.playerPool.Return((FusionNetPlayer)p);
			});
			this.netPlayerCache.Clear();
			this.netPlayerCache.Add(new FusionNetPlayer(default(PlayerRef)));
			return;
		}
		IL_84:
		NetPlayer[] array;
		if (this.runner.IsSinglePlayer)
		{
			if (this.netPlayerCache.Count == 1 && this.netPlayerCache[0].IsLocal)
			{
				return;
			}
			bool flag = false;
			array = this.netPlayerCache.ToArray();
			if (this.netPlayerCache.Count > 0)
			{
				foreach (NetPlayer netPlayer in array)
				{
					if (((FusionNetPlayer)netPlayer).PlayerRef == this.runner.LocalPlayer)
					{
						flag = true;
					}
					else
					{
						this.playerPool.Return((FusionNetPlayer)netPlayer);
						this.netPlayerCache.Remove(netPlayer);
					}
				}
			}
			if (!flag)
			{
				FusionNetPlayer fusionNetPlayer = this.playerPool.Take();
				fusionNetPlayer.InitPlayer(this.runner.LocalPlayer);
				this.netPlayerCache.Add(fusionNetPlayer);
			}
		}
		foreach (PlayerRef playerRef in this.runner.ActivePlayers)
		{
			bool flag2 = false;
			for (int j = 0; j < this.netPlayerCache.Count; j++)
			{
				if (playerRef == ((FusionNetPlayer)this.netPlayerCache[j]).PlayerRef)
				{
					flag2 = true;
				}
			}
			if (!flag2)
			{
				FusionNetPlayer fusionNetPlayer2 = this.playerPool.Take();
				fusionNetPlayer2.InitPlayer(playerRef);
				this.netPlayerCache.Add(fusionNetPlayer2);
			}
		}
		array = this.netPlayerCache.ToArray();
		foreach (NetPlayer netPlayer2 in array)
		{
			bool flag3 = false;
			using (IEnumerator<PlayerRef> enumerator = this.runner.ActivePlayers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == ((FusionNetPlayer)netPlayer2).PlayerRef)
					{
						flag3 = true;
					}
				}
			}
			if (!flag3)
			{
				this.playerPool.Return((FusionNetPlayer)netPlayer2);
				this.netPlayerCache.Remove(netPlayer2);
			}
		}
	}

	// Token: 0x060010AB RID: 4267 RVA: 0x0005F4F0 File Offset: 0x0005D6F0
	public override void SetPlayerObject(GameObject playerInstance, int? owningPlayerID = null)
	{
		PlayerRef player = this.runner.LocalPlayer;
		if (owningPlayerID != null)
		{
			player = this.GetPlayerRef(owningPlayerID.Value);
		}
		this.runner.SetPlayerObject(player, playerInstance.GetComponent<NetworkObject>());
	}

	// Token: 0x060010AC RID: 4268 RVA: 0x0005F534 File Offset: 0x0005D734
	private PlayerRef GetPlayerRef(int playerID)
	{
		if (this.runner == null)
		{
			Debug.LogWarning("There is no runner yet - returning default player ref");
			return default(PlayerRef);
		}
		foreach (PlayerRef result in this.runner.ActivePlayers)
		{
			if (result.PlayerId == playerID)
			{
				return result;
			}
		}
		Debug.LogWarning(string.Format("GetPlayerRef - Couldn't find active player with ID #{0}", playerID));
		return default(PlayerRef);
	}

	// Token: 0x060010AD RID: 4269 RVA: 0x0005F5D0 File Offset: 0x0005D7D0
	public override NetPlayer GetLocalPlayer()
	{
		if (this.netPlayerCache.Count == 0 || this.netPlayerCache.Count != this.runner.SessionInfo.PlayerCount)
		{
			base.UpdatePlayers();
		}
		foreach (NetPlayer netPlayer in this.netPlayerCache)
		{
			if (netPlayer.IsLocal)
			{
				return netPlayer;
			}
		}
		Debug.LogError("Somehow there is no local NetPlayer. This shoulnd't happen.");
		return null;
	}

	// Token: 0x060010AE RID: 4270 RVA: 0x0005F668 File Offset: 0x0005D868
	public override NetPlayer GetPlayer(int PlayerID)
	{
		if (PlayerID == -1)
		{
			Debug.LogWarning("Attempting to get NetPlayer for local -1 ID.");
			return null;
		}
		foreach (NetPlayer netPlayer in this.netPlayerCache)
		{
			if (netPlayer.ActorNumber == PlayerID)
			{
				return netPlayer;
			}
		}
		if (this.netPlayerCache.Count == 0 || this.netPlayerCache.Count != this.runner.SessionInfo.PlayerCount)
		{
			base.UpdatePlayers();
			foreach (NetPlayer netPlayer2 in this.netPlayerCache)
			{
				if (netPlayer2.ActorNumber == PlayerID)
				{
					return netPlayer2;
				}
			}
		}
		Debug.LogError("Failed to find the player, before and after resyncing the player cache, this probably shoulnd't happen...");
		return null;
	}

	// Token: 0x060010AF RID: 4271 RVA: 0x0005F75C File Offset: 0x0005D95C
	public override void SetMyNickName(string name)
	{
		if (!KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags) && !name.StartsWith("gorilla"))
		{
			Debug.Log("[KID] Trying to set custom nickname but that permission has been disallowed");
			if (this.InRoom && GorillaTagger.Instance.rigSerializer != null)
			{
				GorillaTagger.Instance.rigSerializer.nickName = "gorilla";
			}
			return;
		}
		PlayerPrefs.SetString("playerName", name);
		if (this.InRoom && GorillaTagger.Instance.rigSerializer != null)
		{
			GorillaTagger.Instance.rigSerializer.nickName = name;
		}
	}

	// Token: 0x060010B0 RID: 4272 RVA: 0x0005F7F6 File Offset: 0x0005D9F6
	public override string GetMyNickName()
	{
		return PlayerPrefs.GetString("playerName");
	}

	// Token: 0x060010B1 RID: 4273 RVA: 0x0005F804 File Offset: 0x0005DA04
	public override string GetMyDefaultName()
	{
		return "gorilla" + Random.Range(0, 9999).ToString().PadLeft(4, '0');
	}

	// Token: 0x060010B2 RID: 4274 RVA: 0x0005F838 File Offset: 0x0005DA38
	public override string GetNickName(int playerID)
	{
		NetPlayer player = this.GetPlayer(playerID);
		return this.GetNickName(player);
	}

	// Token: 0x060010B3 RID: 4275 RVA: 0x0005F854 File Offset: 0x0005DA54
	public override string GetNickName(NetPlayer player)
	{
		if (player == null)
		{
			Debug.LogError("Cant get nick name as playerID doesnt have a NetPlayer...");
			return "";
		}
		RigContainer rigContainer;
		VRRigCache.Instance.TryGetVrrig(player, out rigContainer);
		if (!KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags))
		{
			return rigContainer.Rig.rigSerializer.defaultName.Value ?? "";
		}
		return rigContainer.Rig.rigSerializer.nickName.Value ?? "";
	}

	// Token: 0x060010B4 RID: 4276 RVA: 0x0005F8CD File Offset: 0x0005DACD
	public override string GetMyUserID()
	{
		return this.runner.GetPlayerUserId(this.runner.LocalPlayer);
	}

	// Token: 0x060010B5 RID: 4277 RVA: 0x0005F8E5 File Offset: 0x0005DAE5
	public override string GetUserID(int playerID)
	{
		if (this.runner == null)
		{
			return string.Empty;
		}
		return this.runner.GetPlayerUserId(this.GetPlayerRef(playerID));
	}

	// Token: 0x060010B6 RID: 4278 RVA: 0x0005F90D File Offset: 0x0005DB0D
	public override string GetUserID(NetPlayer player)
	{
		if (this.runner == null)
		{
			return string.Empty;
		}
		return this.runner.GetPlayerUserId(((FusionNetPlayer)player).PlayerRef);
	}

	// Token: 0x060010B7 RID: 4279 RVA: 0x0005F939 File Offset: 0x0005DB39
	public override void SetMyTutorialComplete()
	{
		if (!(PlayerPrefs.GetString("didTutorial", "nope") == "done"))
		{
			PlayerPrefs.SetString("didTutorial", "done");
			PlayerPrefs.Save();
		}
	}

	// Token: 0x060010B8 RID: 4280 RVA: 0x0005F96A File Offset: 0x0005DB6A
	public override bool GetMyTutorialCompletion()
	{
		return PlayerPrefs.GetString("didTutorial", "nope") == "done";
	}

	// Token: 0x060010B9 RID: 4281 RVA: 0x0005F988 File Offset: 0x0005DB88
	public override bool GetPlayerTutorialCompletion(int playerID)
	{
		NetPlayer player = this.GetPlayer(playerID);
		if (player == null)
		{
			Debug.LogError("Player not found");
			return false;
		}
		RigContainer rigContainer;
		VRRigCache.Instance.TryGetVrrig(player, out rigContainer);
		if (rigContainer == null)
		{
			Debug.LogError("VRRig not found for player");
			return false;
		}
		if (rigContainer.Rig.rigSerializer == null)
		{
			Debug.LogWarning("Vr rig serializer is not set up on the rig yet");
			return false;
		}
		return rigContainer.Rig.rigSerializer.tutorialComplete;
	}

	// Token: 0x060010BA RID: 4282 RVA: 0x0005F9FE File Offset: 0x0005DBFE
	public override int GlobalPlayerCount()
	{
		if (this.regionCrawler == null)
		{
			return 0;
		}
		return this.regionCrawler.PlayerCountGlobal;
	}

	// Token: 0x060010BB RID: 4283 RVA: 0x0005FA1C File Offset: 0x0005DC1C
	public override int GetOwningPlayerID(GameObject obj)
	{
		NetworkObject networkObject;
		if (!obj.TryGetComponent<NetworkObject>(out networkObject))
		{
			return -1;
		}
		if (this.runner.GameMode == Fusion.GameMode.Shared)
		{
			return networkObject.StateAuthority.PlayerId;
		}
		return networkObject.InputAuthority.PlayerId;
	}

	// Token: 0x060010BC RID: 4284 RVA: 0x0005FA60 File Offset: 0x0005DC60
	public override bool IsObjectLocallyOwned(GameObject obj)
	{
		NetworkObject networkObject;
		if (!obj.TryGetComponent<NetworkObject>(out networkObject))
		{
			return false;
		}
		if (this.runner.GameMode == Fusion.GameMode.Shared)
		{
			return networkObject.StateAuthority == this.runner.LocalPlayer;
		}
		return networkObject.InputAuthority == this.runner.LocalPlayer;
	}

	// Token: 0x060010BD RID: 4285 RVA: 0x0005FAB4 File Offset: 0x0005DCB4
	public override bool IsTotalAuthority()
	{
		return this.runner.Mode == SimulationModes.Server || this.runner.Mode == SimulationModes.Host || this.runner.GameMode == Fusion.GameMode.Single || this.runner.IsSharedModeMasterClient;
	}

	// Token: 0x060010BE RID: 4286 RVA: 0x0005FAF0 File Offset: 0x0005DCF0
	public override bool ShouldWriteObjectData(GameObject obj)
	{
		NetworkObject networkObject;
		return obj.TryGetComponent<NetworkObject>(out networkObject) && networkObject.HasStateAuthority;
	}

	// Token: 0x060010BF RID: 4287 RVA: 0x0005FB10 File Offset: 0x0005DD10
	public override bool ShouldUpdateObject(GameObject obj)
	{
		NetworkObject networkObject;
		if (!obj.TryGetComponent<NetworkObject>(out networkObject))
		{
			return true;
		}
		if (this.IsTotalAuthority())
		{
			return true;
		}
		if (networkObject.InputAuthority.IsRealPlayer && !networkObject.InputAuthority.IsRealPlayer)
		{
			return networkObject.InputAuthority == this.runner.LocalPlayer;
		}
		return this.runner.IsSharedModeMasterClient;
	}

	// Token: 0x060010C0 RID: 4288 RVA: 0x0005FB78 File Offset: 0x0005DD78
	public override bool IsObjectRoomObject(GameObject obj)
	{
		NetworkObject networkObject;
		if (obj.TryGetComponent<NetworkObject>(out networkObject))
		{
			Debug.LogWarning("Fusion currently automatically passes false for roomobject check.");
			return false;
		}
		return false;
	}

	// Token: 0x060010C1 RID: 4289 RVA: 0x0005FB9C File Offset: 0x0005DD9C
	private void OnMasterSwitch(NetPlayer player)
	{
		if (this.runner.IsSharedModeMasterClient)
		{
			Dictionary<string, SessionProperty> customProperties = new Dictionary<string, SessionProperty>
			{
				{
					"MasterClient",
					base.LocalPlayer.ActorNumber
				}
			};
			this.runner.SessionInfo.UpdateCustomProperties(customProperties);
		}
	}

	// Token: 0x040018A4 RID: 6308
	private NetworkSystemFusion.InternalState internalState;

	// Token: 0x040018A5 RID: 6309
	private FusionInternalRPCs internalRPCProvider;

	// Token: 0x040018A6 RID: 6310
	private FusionCallbackHandler callbackHandler;

	// Token: 0x040018A7 RID: 6311
	private FusionRegionCrawler regionCrawler;

	// Token: 0x040018A8 RID: 6312
	private GameObject volatileNetObj;

	// Token: 0x040018A9 RID: 6313
	private Fusion.Photon.Realtime.AuthenticationValues cachedPlayfabAuth;

	// Token: 0x040018AA RID: 6314
	private const string playerPropertiesPath = "P_FusionProperties";

	// Token: 0x040018AB RID: 6315
	private bool lastConnectAttempt_WasFull;

	// Token: 0x040018AC RID: 6316
	private FusionVoiceBridge FusionVoiceBridge;

	// Token: 0x040018AD RID: 6317
	private VoiceConnection FusionVoice;

	// Token: 0x040018AE RID: 6318
	private CustomObjectProvider myObjectProvider;

	// Token: 0x040018AF RID: 6319
	private ObjectPool<FusionNetPlayer> playerPool;

	// Token: 0x040018B0 RID: 6320
	public List<NetworkObject> cachedNetSceneObjects = new List<NetworkObject>();

	// Token: 0x040018B1 RID: 6321
	private List<INetworkRunnerCallbacks> objectsThatNeedCallbacks = new List<INetworkRunnerCallbacks>();

	// Token: 0x040018B2 RID: 6322
	private Queue<NetworkObject> registrationQueue = new Queue<NetworkObject>();

	// Token: 0x040018B3 RID: 6323
	private bool isProcessingQueue;

	// Token: 0x020002C1 RID: 705
	private enum InternalState
	{
		// Token: 0x040018B5 RID: 6325
		AwaitingAuth,
		// Token: 0x040018B6 RID: 6326
		Idle,
		// Token: 0x040018B7 RID: 6327
		Searching_Joining,
		// Token: 0x040018B8 RID: 6328
		Searching_Joined,
		// Token: 0x040018B9 RID: 6329
		Searching_JoinFailed,
		// Token: 0x040018BA RID: 6330
		Searching_Disconnecting,
		// Token: 0x040018BB RID: 6331
		Searching_Disconnected,
		// Token: 0x040018BC RID: 6332
		ConnectingToRoom,
		// Token: 0x040018BD RID: 6333
		ConnectedToRoom,
		// Token: 0x040018BE RID: 6334
		JoinRoomFailed,
		// Token: 0x040018BF RID: 6335
		Disconnecting,
		// Token: 0x040018C0 RID: 6336
		Disconnected,
		// Token: 0x040018C1 RID: 6337
		StateCheckFailed
	}
}
