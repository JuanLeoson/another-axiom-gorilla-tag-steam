using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Fusion;
using GorillaTag;
using GorillaTag.Audio;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x020002F3 RID: 755
[RequireComponent(typeof(PUNCallbackNotifier))]
public class NetworkSystemPUN : NetworkSystem
{
	// Token: 0x170001F4 RID: 500
	// (get) Token: 0x06001218 RID: 4632 RVA: 0x00063646 File Offset: 0x00061846
	public override NetPlayer[] AllNetPlayers
	{
		get
		{
			return this.m_allNetPlayers;
		}
	}

	// Token: 0x170001F5 RID: 501
	// (get) Token: 0x06001219 RID: 4633 RVA: 0x0006364E File Offset: 0x0006184E
	public override NetPlayer[] PlayerListOthers
	{
		get
		{
			return this.m_otherNetPlayers;
		}
	}

	// Token: 0x170001F6 RID: 502
	// (get) Token: 0x0600121A RID: 4634 RVA: 0x00063656 File Offset: 0x00061856
	public override VoiceConnection VoiceConnection
	{
		get
		{
			return this.punVoice;
		}
	}

	// Token: 0x170001F7 RID: 503
	// (get) Token: 0x0600121B RID: 4635 RVA: 0x00063660 File Offset: 0x00061860
	private int lowestPingRegionIndex
	{
		get
		{
			int num = 9999;
			int result = -1;
			for (int i = 0; i < this.regionData.Length; i++)
			{
				if (this.regionData[i].pingToRegion < num)
				{
					num = this.regionData[i].pingToRegion;
					result = i;
				}
			}
			return result;
		}
	}

	// Token: 0x170001F8 RID: 504
	// (get) Token: 0x0600121C RID: 4636 RVA: 0x000636A9 File Offset: 0x000618A9
	// (set) Token: 0x0600121D RID: 4637 RVA: 0x000636B1 File Offset: 0x000618B1
	private NetworkSystemPUN.InternalState internalState
	{
		get
		{
			return this.currentState;
		}
		set
		{
			this.currentState = value;
		}
	}

	// Token: 0x170001F9 RID: 505
	// (get) Token: 0x0600121E RID: 4638 RVA: 0x000636BA File Offset: 0x000618BA
	public override string CurrentPhotonBackend
	{
		get
		{
			return "PUN";
		}
	}

	// Token: 0x170001FA RID: 506
	// (get) Token: 0x0600121F RID: 4639 RVA: 0x000636C1 File Offset: 0x000618C1
	public override bool IsOnline
	{
		get
		{
			return this.InRoom;
		}
	}

	// Token: 0x170001FB RID: 507
	// (get) Token: 0x06001220 RID: 4640 RVA: 0x000636C9 File Offset: 0x000618C9
	public override bool InRoom
	{
		get
		{
			return PhotonNetwork.InRoom;
		}
	}

	// Token: 0x170001FC RID: 508
	// (get) Token: 0x06001221 RID: 4641 RVA: 0x000636D0 File Offset: 0x000618D0
	public override string RoomName
	{
		get
		{
			Room currentRoom = PhotonNetwork.CurrentRoom;
			return ((currentRoom != null) ? currentRoom.Name : null) ?? string.Empty;
		}
	}

	// Token: 0x06001222 RID: 4642 RVA: 0x000636EC File Offset: 0x000618EC
	public override string RoomStringStripped()
	{
		Room currentRoom = PhotonNetwork.CurrentRoom;
		NetworkSystem.reusableSB.Clear();
		NetworkSystem.reusableSB.AppendFormat("Room: '{0}' ", (currentRoom.Name.Length < 20) ? currentRoom.Name : currentRoom.Name.Remove(20));
		NetworkSystem.reusableSB.AppendFormat("{0},{1} {3}/{2} players.", new object[]
		{
			currentRoom.IsVisible ? "visible" : "hidden",
			currentRoom.IsOpen ? "open" : "closed",
			currentRoom.MaxPlayers,
			currentRoom.PlayerCount
		});
		NetworkSystem.reusableSB.Append("\ncustomProps: {");
		NetworkSystem.reusableSB.AppendFormat("joinedGameMode={0}, ", (RoomSystem.RoomGameMode.Length < 50) ? RoomSystem.RoomGameMode : RoomSystem.RoomGameMode.Remove(50));
		IDictionary customProperties = currentRoom.CustomProperties;
		if (customProperties.Contains("gameMode"))
		{
			object obj = customProperties["gameMode"];
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

	// Token: 0x170001FD RID: 509
	// (get) Token: 0x06001223 RID: 4643 RVA: 0x00063874 File Offset: 0x00061A74
	public override string GameModeString
	{
		get
		{
			object obj;
			PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out obj);
			if (obj != null)
			{
				return obj.ToString();
			}
			return null;
		}
	}

	// Token: 0x170001FE RID: 510
	// (get) Token: 0x06001224 RID: 4644 RVA: 0x000638A3 File Offset: 0x00061AA3
	public override string CurrentRegion
	{
		get
		{
			return PhotonNetwork.CloudRegion;
		}
	}

	// Token: 0x170001FF RID: 511
	// (get) Token: 0x06001225 RID: 4645 RVA: 0x000638AA File Offset: 0x00061AAA
	public override bool SessionIsPrivate
	{
		get
		{
			Room currentRoom = PhotonNetwork.CurrentRoom;
			return currentRoom != null && !currentRoom.IsVisible;
		}
	}

	// Token: 0x17000200 RID: 512
	// (get) Token: 0x06001226 RID: 4646 RVA: 0x000638BF File Offset: 0x00061ABF
	public override int LocalPlayerID
	{
		get
		{
			return PhotonNetwork.LocalPlayer.ActorNumber;
		}
	}

	// Token: 0x17000201 RID: 513
	// (get) Token: 0x06001227 RID: 4647 RVA: 0x000638CB File Offset: 0x00061ACB
	public override int ServerTimestamp
	{
		get
		{
			return PhotonNetwork.ServerTimestamp;
		}
	}

	// Token: 0x17000202 RID: 514
	// (get) Token: 0x06001228 RID: 4648 RVA: 0x000638D2 File Offset: 0x00061AD2
	public override double SimTime
	{
		get
		{
			return PhotonNetwork.Time;
		}
	}

	// Token: 0x17000203 RID: 515
	// (get) Token: 0x06001229 RID: 4649 RVA: 0x000638D9 File Offset: 0x00061AD9
	public override float SimDeltaTime
	{
		get
		{
			return Time.deltaTime;
		}
	}

	// Token: 0x17000204 RID: 516
	// (get) Token: 0x0600122A RID: 4650 RVA: 0x000638CB File Offset: 0x00061ACB
	public override int SimTick
	{
		get
		{
			return PhotonNetwork.ServerTimestamp;
		}
	}

	// Token: 0x17000205 RID: 517
	// (get) Token: 0x0600122B RID: 4651 RVA: 0x000638E0 File Offset: 0x00061AE0
	public override int TickRate
	{
		get
		{
			return PhotonNetwork.SerializationRate;
		}
	}

	// Token: 0x17000206 RID: 518
	// (get) Token: 0x0600122C RID: 4652 RVA: 0x000638E7 File Offset: 0x00061AE7
	public override int RoomPlayerCount
	{
		get
		{
			return (int)PhotonNetwork.CurrentRoom.PlayerCount;
		}
	}

	// Token: 0x17000207 RID: 519
	// (get) Token: 0x0600122D RID: 4653 RVA: 0x000638F3 File Offset: 0x00061AF3
	public override bool IsMasterClient
	{
		get
		{
			return PhotonNetwork.IsMasterClient;
		}
	}

	// Token: 0x0600122E RID: 4654 RVA: 0x000638FC File Offset: 0x00061AFC
	public override void Initialise()
	{
		NetworkSystemPUN.<Initialise>d__53 <Initialise>d__;
		<Initialise>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Initialise>d__.<>4__this = this;
		<Initialise>d__.<>1__state = -1;
		<Initialise>d__.<>t__builder.Start<NetworkSystemPUN.<Initialise>d__53>(ref <Initialise>d__);
	}

	// Token: 0x0600122F RID: 4655 RVA: 0x00063934 File Offset: 0x00061B34
	private Task CacheRegionInfo()
	{
		NetworkSystemPUN.<CacheRegionInfo>d__54 <CacheRegionInfo>d__;
		<CacheRegionInfo>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<CacheRegionInfo>d__.<>4__this = this;
		<CacheRegionInfo>d__.<>1__state = -1;
		<CacheRegionInfo>d__.<>t__builder.Start<NetworkSystemPUN.<CacheRegionInfo>d__54>(ref <CacheRegionInfo>d__);
		return <CacheRegionInfo>d__.<>t__builder.Task;
	}

	// Token: 0x06001230 RID: 4656 RVA: 0x00063977 File Offset: 0x00061B77
	public override AuthenticationValues GetAuthenticationValues()
	{
		return PhotonNetwork.AuthValues;
	}

	// Token: 0x06001231 RID: 4657 RVA: 0x0006397E File Offset: 0x00061B7E
	public override void SetAuthenticationValues(AuthenticationValues authValues)
	{
		PhotonNetwork.AuthValues = authValues;
	}

	// Token: 0x06001232 RID: 4658 RVA: 0x00063986 File Offset: 0x00061B86
	public override void FinishAuthenticating()
	{
		this.internalState = NetworkSystemPUN.InternalState.Authenticated;
	}

	// Token: 0x06001233 RID: 4659 RVA: 0x00063990 File Offset: 0x00061B90
	private Task WaitForState(CancellationToken ct, params NetworkSystemPUN.InternalState[] desiredStates)
	{
		NetworkSystemPUN.<WaitForState>d__58 <WaitForState>d__;
		<WaitForState>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForState>d__.<>4__this = this;
		<WaitForState>d__.ct = ct;
		<WaitForState>d__.desiredStates = desiredStates;
		<WaitForState>d__.<>1__state = -1;
		<WaitForState>d__.<>t__builder.Start<NetworkSystemPUN.<WaitForState>d__58>(ref <WaitForState>d__);
		return <WaitForState>d__.<>t__builder.Task;
	}

	// Token: 0x06001234 RID: 4660 RVA: 0x000639E4 File Offset: 0x00061BE4
	private Task<bool> WaitForStateCheck(params NetworkSystemPUN.InternalState[] desiredStates)
	{
		NetworkSystemPUN.<WaitForStateCheck>d__59 <WaitForStateCheck>d__;
		<WaitForStateCheck>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<WaitForStateCheck>d__.<>4__this = this;
		<WaitForStateCheck>d__.desiredStates = desiredStates;
		<WaitForStateCheck>d__.<>1__state = -1;
		<WaitForStateCheck>d__.<>t__builder.Start<NetworkSystemPUN.<WaitForStateCheck>d__59>(ref <WaitForStateCheck>d__);
		return <WaitForStateCheck>d__.<>t__builder.Task;
	}

	// Token: 0x06001235 RID: 4661 RVA: 0x00063A30 File Offset: 0x00061C30
	private Task<NetJoinResult> MakeOrFindRoom(string roomName, RoomConfig opts, int regionIndex = -1)
	{
		NetworkSystemPUN.<MakeOrFindRoom>d__60 <MakeOrFindRoom>d__;
		<MakeOrFindRoom>d__.<>t__builder = AsyncTaskMethodBuilder<NetJoinResult>.Create();
		<MakeOrFindRoom>d__.<>4__this = this;
		<MakeOrFindRoom>d__.roomName = roomName;
		<MakeOrFindRoom>d__.opts = opts;
		<MakeOrFindRoom>d__.regionIndex = regionIndex;
		<MakeOrFindRoom>d__.<>1__state = -1;
		<MakeOrFindRoom>d__.<>t__builder.Start<NetworkSystemPUN.<MakeOrFindRoom>d__60>(ref <MakeOrFindRoom>d__);
		return <MakeOrFindRoom>d__.<>t__builder.Task;
	}

	// Token: 0x06001236 RID: 4662 RVA: 0x00063A8C File Offset: 0x00061C8C
	private Task<bool> TryJoinRoom(string roomName, RoomConfig opts)
	{
		NetworkSystemPUN.<TryJoinRoom>d__61 <TryJoinRoom>d__;
		<TryJoinRoom>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<TryJoinRoom>d__.<>4__this = this;
		<TryJoinRoom>d__.roomName = roomName;
		<TryJoinRoom>d__.opts = opts;
		<TryJoinRoom>d__.<>1__state = -1;
		<TryJoinRoom>d__.<>t__builder.Start<NetworkSystemPUN.<TryJoinRoom>d__61>(ref <TryJoinRoom>d__);
		return <TryJoinRoom>d__.<>t__builder.Task;
	}

	// Token: 0x06001237 RID: 4663 RVA: 0x00063AE0 File Offset: 0x00061CE0
	private Task<bool> TryJoinRoomInRegion(string roomName, RoomConfig opts, int regionIndex)
	{
		NetworkSystemPUN.<TryJoinRoomInRegion>d__62 <TryJoinRoomInRegion>d__;
		<TryJoinRoomInRegion>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<TryJoinRoomInRegion>d__.<>4__this = this;
		<TryJoinRoomInRegion>d__.roomName = roomName;
		<TryJoinRoomInRegion>d__.opts = opts;
		<TryJoinRoomInRegion>d__.regionIndex = regionIndex;
		<TryJoinRoomInRegion>d__.<>1__state = -1;
		<TryJoinRoomInRegion>d__.<>t__builder.Start<NetworkSystemPUN.<TryJoinRoomInRegion>d__62>(ref <TryJoinRoomInRegion>d__);
		return <TryJoinRoomInRegion>d__.<>t__builder.Task;
	}

	// Token: 0x06001238 RID: 4664 RVA: 0x00063B3C File Offset: 0x00061D3C
	private Task<NetJoinResult> TryCreateRoom(string roomName, RoomConfig opts)
	{
		NetworkSystemPUN.<TryCreateRoom>d__63 <TryCreateRoom>d__;
		<TryCreateRoom>d__.<>t__builder = AsyncTaskMethodBuilder<NetJoinResult>.Create();
		<TryCreateRoom>d__.<>4__this = this;
		<TryCreateRoom>d__.roomName = roomName;
		<TryCreateRoom>d__.opts = opts;
		<TryCreateRoom>d__.<>1__state = -1;
		<TryCreateRoom>d__.<>t__builder.Start<NetworkSystemPUN.<TryCreateRoom>d__63>(ref <TryCreateRoom>d__);
		return <TryCreateRoom>d__.<>t__builder.Task;
	}

	// Token: 0x06001239 RID: 4665 RVA: 0x00063B90 File Offset: 0x00061D90
	private Task<NetJoinResult> JoinRandomPublicRoom(RoomConfig opts)
	{
		NetworkSystemPUN.<JoinRandomPublicRoom>d__64 <JoinRandomPublicRoom>d__;
		<JoinRandomPublicRoom>d__.<>t__builder = AsyncTaskMethodBuilder<NetJoinResult>.Create();
		<JoinRandomPublicRoom>d__.<>4__this = this;
		<JoinRandomPublicRoom>d__.opts = opts;
		<JoinRandomPublicRoom>d__.<>1__state = -1;
		<JoinRandomPublicRoom>d__.<>t__builder.Start<NetworkSystemPUN.<JoinRandomPublicRoom>d__64>(ref <JoinRandomPublicRoom>d__);
		return <JoinRandomPublicRoom>d__.<>t__builder.Task;
	}

	// Token: 0x0600123A RID: 4666 RVA: 0x00063BDC File Offset: 0x00061DDC
	public override Task<NetJoinResult> ConnectToRoom(string roomName, RoomConfig opts, int regionIndex = -1)
	{
		NetworkSystemPUN.<ConnectToRoom>d__65 <ConnectToRoom>d__;
		<ConnectToRoom>d__.<>t__builder = AsyncTaskMethodBuilder<NetJoinResult>.Create();
		<ConnectToRoom>d__.<>4__this = this;
		<ConnectToRoom>d__.roomName = roomName;
		<ConnectToRoom>d__.opts = opts;
		<ConnectToRoom>d__.regionIndex = regionIndex;
		<ConnectToRoom>d__.<>1__state = -1;
		<ConnectToRoom>d__.<>t__builder.Start<NetworkSystemPUN.<ConnectToRoom>d__65>(ref <ConnectToRoom>d__);
		return <ConnectToRoom>d__.<>t__builder.Task;
	}

	// Token: 0x0600123B RID: 4667 RVA: 0x00063C38 File Offset: 0x00061E38
	public override Task JoinFriendsRoom(string userID, int actorIDToFollow, string keyToFollow, string shufflerToFollow)
	{
		NetworkSystemPUN.<JoinFriendsRoom>d__66 <JoinFriendsRoom>d__;
		<JoinFriendsRoom>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<JoinFriendsRoom>d__.<>4__this = this;
		<JoinFriendsRoom>d__.userID = userID;
		<JoinFriendsRoom>d__.actorIDToFollow = actorIDToFollow;
		<JoinFriendsRoom>d__.keyToFollow = keyToFollow;
		<JoinFriendsRoom>d__.shufflerToFollow = shufflerToFollow;
		<JoinFriendsRoom>d__.<>1__state = -1;
		<JoinFriendsRoom>d__.<>t__builder.Start<NetworkSystemPUN.<JoinFriendsRoom>d__66>(ref <JoinFriendsRoom>d__);
		return <JoinFriendsRoom>d__.<>t__builder.Task;
	}

	// Token: 0x0600123C RID: 4668 RVA: 0x00002628 File Offset: 0x00000828
	public override void JoinPubWithFriends()
	{
		throw new NotImplementedException();
	}

	// Token: 0x0600123D RID: 4669 RVA: 0x00063C9C File Offset: 0x00061E9C
	public override string GetRandomWeightedRegion()
	{
		float value = Random.value;
		int num = 0;
		for (int i = 0; i < this.regionData.Length; i++)
		{
			num += this.regionData[i].playersInRegion;
		}
		float num2 = 0f;
		int num3 = -1;
		while (num2 < value && num3 < this.regionData.Length - 1)
		{
			num3++;
			num2 += (float)this.regionData[num3].playersInRegion / (float)num;
		}
		return this.regionNames[num3];
	}

	// Token: 0x0600123E RID: 4670 RVA: 0x00063D14 File Offset: 0x00061F14
	public override Task ReturnToSinglePlayer()
	{
		NetworkSystemPUN.<ReturnToSinglePlayer>d__69 <ReturnToSinglePlayer>d__;
		<ReturnToSinglePlayer>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<ReturnToSinglePlayer>d__.<>4__this = this;
		<ReturnToSinglePlayer>d__.<>1__state = -1;
		<ReturnToSinglePlayer>d__.<>t__builder.Start<NetworkSystemPUN.<ReturnToSinglePlayer>d__69>(ref <ReturnToSinglePlayer>d__);
		return <ReturnToSinglePlayer>d__.<>t__builder.Task;
	}

	// Token: 0x0600123F RID: 4671 RVA: 0x00063D58 File Offset: 0x00061F58
	private Task InternalDisconnect()
	{
		NetworkSystemPUN.<InternalDisconnect>d__70 <InternalDisconnect>d__;
		<InternalDisconnect>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<InternalDisconnect>d__.<>4__this = this;
		<InternalDisconnect>d__.<>1__state = -1;
		<InternalDisconnect>d__.<>t__builder.Start<NetworkSystemPUN.<InternalDisconnect>d__70>(ref <InternalDisconnect>d__);
		return <InternalDisconnect>d__.<>t__builder.Task;
	}

	// Token: 0x06001240 RID: 4672 RVA: 0x00063D9B File Offset: 0x00061F9B
	private void AddVoice()
	{
		this.SetupVoice();
	}

	// Token: 0x06001241 RID: 4673 RVA: 0x00063DA4 File Offset: 0x00061FA4
	private void SetupVoice()
	{
		try
		{
			this.punVoice = PhotonVoiceNetwork.Instance;
			this.VoiceNetworkObject = this.punVoice.gameObject;
			this.VoiceNetworkObject.name = "VoiceNetworkObject";
			this.VoiceNetworkObject.transform.parent = base.transform;
			this.VoiceNetworkObject.transform.localPosition = Vector3.zero;
			this.punVoice.LogLevel = this.VoiceSettings.LogLevel;
			this.punVoice.GlobalRecordersLogLevel = this.VoiceSettings.GlobalRecordersLogLevel;
			this.punVoice.GlobalSpeakersLogLevel = this.VoiceSettings.GlobalSpeakersLogLevel;
			this.punVoice.AutoConnectAndJoin = this.VoiceSettings.AutoConnectAndJoin;
			this.punVoice.AutoLeaveAndDisconnect = this.VoiceSettings.AutoLeaveAndDisconnect;
			this.punVoice.WorkInOfflineMode = this.VoiceSettings.WorkInOfflineMode;
			this.punVoice.AutoCreateSpeakerIfNotFound = this.VoiceSettings.CreateSpeakerIfNotFound;
			AppSettings appSettings = new AppSettings();
			appSettings.AppIdRealtime = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime;
			appSettings.AppIdVoice = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdVoice;
			this.punVoice.Settings = appSettings;
			this.remoteVoiceAddedCallbacks.ForEach(delegate(Action<RemoteVoiceLink> callback)
			{
				this.punVoice.RemoteVoiceAdded += callback;
			});
			this.localRecorder = this.VoiceNetworkObject.GetComponent<GTRecorder>();
			if (this.localRecorder == null)
			{
				this.localRecorder = this.VoiceNetworkObject.AddComponent<GTRecorder>();
				if (VRRigCache.Instance != null && VRRigCache.Instance.localRig != null)
				{
					LoudSpeakerActivator componentInChildren = VRRigCache.Instance.localRig.GetComponentInChildren<LoudSpeakerActivator>();
					if (componentInChildren != null)
					{
						componentInChildren.SetRecorder((GTRecorder)this.localRecorder);
					}
				}
			}
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
			this.punVoice.PrimaryRecorder = this.localRecorder;
			this.VoiceNetworkObject.AddComponent<VoiceToLoudness>();
		}
		catch (Exception ex)
		{
			Debug.LogError("An exception was thrown when trying to setup photon voice, please check microphone permissions.:/n" + ex.ToString());
		}
	}

	// Token: 0x06001242 RID: 4674 RVA: 0x0005EA83 File Offset: 0x0005CC83
	public override void AddRemoteVoiceAddedCallback(Action<RemoteVoiceLink> callback)
	{
		this.remoteVoiceAddedCallbacks.Add(callback);
	}

	// Token: 0x06001243 RID: 4675 RVA: 0x00064150 File Offset: 0x00062350
	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject = false)
	{
		if (PhotonNetwork.CurrentRoom == null)
		{
			return null;
		}
		if (isRoomObject)
		{
			return PhotonNetwork.InstantiateRoomObject(prefab.name, position, rotation, 0, null);
		}
		return PhotonNetwork.Instantiate(prefab.name, position, rotation, 0, null);
	}

	// Token: 0x06001244 RID: 4676 RVA: 0x0006417E File Offset: 0x0006237E
	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, int playerAuthID, bool isRoomObject = false)
	{
		return this.NetInstantiate(prefab, position, rotation, isRoomObject);
	}

	// Token: 0x06001245 RID: 4677 RVA: 0x0006418B File Offset: 0x0006238B
	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject, byte group = 0, object[] data = null, NetworkRunner.OnBeforeSpawned callback = null)
	{
		if (PhotonNetwork.CurrentRoom == null)
		{
			return null;
		}
		if (isRoomObject)
		{
			return PhotonNetwork.InstantiateRoomObject(prefab.name, position, rotation, group, data);
		}
		return PhotonNetwork.Instantiate(prefab.name, position, rotation, group, data);
	}

	// Token: 0x06001246 RID: 4678 RVA: 0x000641C0 File Offset: 0x000623C0
	public override void NetDestroy(GameObject instance)
	{
		PhotonView photonView;
		if (instance.TryGetComponent<PhotonView>(out photonView) && photonView.AmOwner)
		{
			PhotonNetwork.Destroy(instance);
			return;
		}
		Object.Destroy(instance);
	}

	// Token: 0x06001247 RID: 4679 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void SetPlayerObject(GameObject playerInstance, int? owningPlayerID = null)
	{
	}

	// Token: 0x06001248 RID: 4680 RVA: 0x000641EC File Offset: 0x000623EC
	public override void CallRPC(MonoBehaviour component, NetworkSystem.RPC rpcMethod, bool sendToSelf = true)
	{
		RpcTarget target = sendToSelf ? RpcTarget.All : RpcTarget.Others;
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, target, new object[]
		{
			NetworkSystem.EmptyArgs
		});
	}

	// Token: 0x06001249 RID: 4681 RVA: 0x00064228 File Offset: 0x00062428
	public override void CallRPC<T>(MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args, bool sendToSelf = true)
	{
		RpcTarget target = sendToSelf ? RpcTarget.All : RpcTarget.Others;
		ref args.SerializeToRPCData<T>();
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, target, new object[]
		{
			args.Data
		});
	}

	// Token: 0x0600124A RID: 4682 RVA: 0x0006426C File Offset: 0x0006246C
	public override void CallRPC(MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message, bool sendToSelf = true)
	{
		RpcTarget target = sendToSelf ? RpcTarget.All : RpcTarget.Others;
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, target, new object[]
		{
			message
		});
	}

	// Token: 0x0600124B RID: 4683 RVA: 0x000642A4 File Offset: 0x000624A4
	public override void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod)
	{
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(targetPlayerID, false);
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, player, new object[]
		{
			NetworkSystem.EmptyArgs
		});
	}

	// Token: 0x0600124C RID: 4684 RVA: 0x000642E4 File Offset: 0x000624E4
	public override void CallRPC<T>(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args)
	{
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(targetPlayerID, false);
		ref args.SerializeToRPCData<T>();
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, player, new object[]
		{
			args.Data
		});
	}

	// Token: 0x0600124D RID: 4685 RVA: 0x0006432C File Offset: 0x0006252C
	public override void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message)
	{
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(targetPlayerID, false);
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, player, new object[]
		{
			message
		});
	}

	// Token: 0x0600124E RID: 4686 RVA: 0x00064368 File Offset: 0x00062568
	public override Task AwaitSceneReady()
	{
		NetworkSystemPUN.<AwaitSceneReady>d__85 <AwaitSceneReady>d__;
		<AwaitSceneReady>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<AwaitSceneReady>d__.<>1__state = -1;
		<AwaitSceneReady>d__.<>t__builder.Start<NetworkSystemPUN.<AwaitSceneReady>d__85>(ref <AwaitSceneReady>d__);
		return <AwaitSceneReady>d__.<>t__builder.Task;
	}

	// Token: 0x0600124F RID: 4687 RVA: 0x000643A4 File Offset: 0x000625A4
	public override NetPlayer GetLocalPlayer()
	{
		if (this.netPlayerCache.Count == 0)
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
		Debug.LogError("Somehow no local net players found. This shouldn't happen");
		return null;
	}

	// Token: 0x06001250 RID: 4688 RVA: 0x0006441C File Offset: 0x0006261C
	public override NetPlayer GetPlayer(int PlayerID)
	{
		if (this.InRoom && !PhotonNetwork.CurrentRoom.Players.ContainsKey(PlayerID))
		{
			return null;
		}
		foreach (NetPlayer netPlayer in this.netPlayerCache)
		{
			if (netPlayer.ActorNumber == PlayerID)
			{
				return netPlayer;
			}
		}
		base.UpdatePlayers();
		foreach (NetPlayer netPlayer2 in this.netPlayerCache)
		{
			if (netPlayer2.ActorNumber == PlayerID)
			{
				return netPlayer2;
			}
		}
		GTDev.LogWarning<string>("There is no NetPlayer with this ID currently in game. Passed ID: " + PlayerID.ToString(), null);
		return null;
	}

	// Token: 0x06001251 RID: 4689 RVA: 0x000644FC File Offset: 0x000626FC
	public override void SetMyNickName(string id)
	{
		if (!KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags) && !id.StartsWith("gorilla"))
		{
			Debug.Log("[KID] Trying to set custom nickname but that permission has been disallowed");
			PhotonNetwork.LocalPlayer.NickName = "gorilla";
			return;
		}
		PlayerPrefs.SetString("playerName", id);
		PhotonNetwork.LocalPlayer.NickName = id;
	}

	// Token: 0x06001252 RID: 4690 RVA: 0x0006454E File Offset: 0x0006274E
	public override string GetMyNickName()
	{
		return PhotonNetwork.LocalPlayer.NickName;
	}

	// Token: 0x06001253 RID: 4691 RVA: 0x0006455A File Offset: 0x0006275A
	public override string GetMyDefaultName()
	{
		return PhotonNetwork.LocalPlayer.DefaultName;
	}

	// Token: 0x06001254 RID: 4692 RVA: 0x00064568 File Offset: 0x00062768
	public override string GetNickName(int playerID)
	{
		NetPlayer player = this.GetPlayer(playerID);
		if (player != null)
		{
			return player.NickName;
		}
		return null;
	}

	// Token: 0x06001255 RID: 4693 RVA: 0x00064588 File Offset: 0x00062788
	public override string GetNickName(NetPlayer player)
	{
		return player.NickName;
	}

	// Token: 0x06001256 RID: 4694 RVA: 0x00064590 File Offset: 0x00062790
	public override void SetMyTutorialComplete()
	{
		bool flag = PlayerPrefs.GetString("didTutorial", "nope") == "done";
		if (!flag)
		{
			PlayerPrefs.SetString("didTutorial", "done");
			PlayerPrefs.Save();
		}
		Hashtable hashtable = new Hashtable();
		hashtable.Add("didTutorial", flag);
		PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable, null, null);
	}

	// Token: 0x06001257 RID: 4695 RVA: 0x0005F96A File Offset: 0x0005DB6A
	public override bool GetMyTutorialCompletion()
	{
		return PlayerPrefs.GetString("didTutorial", "nope") == "done";
	}

	// Token: 0x06001258 RID: 4696 RVA: 0x000645F4 File Offset: 0x000627F4
	public override bool GetPlayerTutorialCompletion(int playerID)
	{
		NetPlayer player = this.GetPlayer(playerID);
		if (player == null)
		{
			return false;
		}
		Player player2 = PhotonNetwork.CurrentRoom.GetPlayer(player.ActorNumber, false);
		if (player2 == null)
		{
			return false;
		}
		object obj;
		if (player2.CustomProperties.TryGetValue("didTutorial", out obj))
		{
			bool flag;
			bool flag2;
			if (obj is bool)
			{
				flag = (bool)obj;
				flag2 = (1 == 0);
			}
			else
			{
				flag2 = true;
			}
			return flag2 || flag;
		}
		return false;
	}

	// Token: 0x06001259 RID: 4697 RVA: 0x00064653 File Offset: 0x00062853
	public override string GetMyUserID()
	{
		return PhotonNetwork.LocalPlayer.UserId;
	}

	// Token: 0x0600125A RID: 4698 RVA: 0x00064660 File Offset: 0x00062860
	public override string GetUserID(int playerID)
	{
		NetPlayer player = this.GetPlayer(playerID);
		if (player != null)
		{
			return player.UserId;
		}
		return null;
	}

	// Token: 0x0600125B RID: 4699 RVA: 0x00064680 File Offset: 0x00062880
	public override string GetUserID(NetPlayer netPlayer)
	{
		Player playerRef = ((PunNetPlayer)netPlayer).PlayerRef;
		if (playerRef != null)
		{
			return playerRef.UserId;
		}
		return null;
	}

	// Token: 0x0600125C RID: 4700 RVA: 0x000646A4 File Offset: 0x000628A4
	public override int GlobalPlayerCount()
	{
		int num = 0;
		foreach (NetworkRegionInfo networkRegionInfo in this.regionData)
		{
			num += networkRegionInfo.playersInRegion;
		}
		return num;
	}

	// Token: 0x0600125D RID: 4701 RVA: 0x000646D8 File Offset: 0x000628D8
	public override bool IsObjectLocallyOwned(GameObject obj)
	{
		PhotonView photonView;
		return !this.IsOnline || !obj.TryGetComponent<PhotonView>(out photonView) || photonView.IsMine;
	}

	// Token: 0x0600125E RID: 4702 RVA: 0x00064704 File Offset: 0x00062904
	protected override void UpdateNetPlayerList()
	{
		if (!this.IsOnline)
		{
			bool flag = false;
			PunNetPlayer punNetPlayer = null;
			if (this.netPlayerCache.Count > 0)
			{
				for (int i = 0; i < this.netPlayerCache.Count; i++)
				{
					NetPlayer netPlayer = this.netPlayerCache[i];
					if (netPlayer.IsLocal)
					{
						punNetPlayer = (PunNetPlayer)netPlayer;
						flag = true;
					}
					else
					{
						this.playerPool.Return((PunNetPlayer)netPlayer);
					}
				}
				this.netPlayerCache.Clear();
			}
			if (!flag)
			{
				punNetPlayer = this.playerPool.Take();
				punNetPlayer.InitPlayer(PhotonNetwork.LocalPlayer);
			}
			this.netPlayerCache.Add(punNetPlayer);
		}
		else
		{
			Dictionary<int, Player>.ValueCollection values = PhotonNetwork.CurrentRoom.Players.Values;
			foreach (Player player in values)
			{
				bool flag2 = false;
				for (int j = 0; j < this.netPlayerCache.Count; j++)
				{
					if (player == ((PunNetPlayer)this.netPlayerCache[j]).PlayerRef)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					PunNetPlayer punNetPlayer2 = this.playerPool.Take();
					punNetPlayer2.InitPlayer(player);
					this.netPlayerCache.Add(punNetPlayer2);
				}
			}
			for (int k = 0; k < this.netPlayerCache.Count; k++)
			{
				PunNetPlayer punNetPlayer3 = (PunNetPlayer)this.netPlayerCache[k];
				bool flag3 = false;
				using (Dictionary<int, Player>.ValueCollection.Enumerator enumerator = values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current == punNetPlayer3.PlayerRef)
						{
							flag3 = true;
							break;
						}
					}
				}
				if (!flag3)
				{
					this.playerPool.Return(punNetPlayer3);
					this.netPlayerCache.Remove(punNetPlayer3);
				}
			}
		}
		this.m_allNetPlayers = this.netPlayerCache.ToArray();
		this.m_otherNetPlayers = new NetPlayer[this.m_allNetPlayers.Length - 1];
		int num = 0;
		for (int l = 0; l < this.m_allNetPlayers.Length; l++)
		{
			NetPlayer netPlayer2 = this.m_allNetPlayers[l];
			if (netPlayer2.IsLocal)
			{
				num++;
			}
			else
			{
				int num2 = l - num;
				if (num2 == this.m_otherNetPlayers.Length)
				{
					break;
				}
				this.m_otherNetPlayers[num2] = netPlayer2;
			}
		}
	}

	// Token: 0x0600125F RID: 4703 RVA: 0x00064970 File Offset: 0x00062B70
	public override bool IsObjectRoomObject(GameObject obj)
	{
		PhotonView component = obj.GetComponent<PhotonView>();
		if (component == null)
		{
			Debug.LogError("No photonview found on this Object, this shouldn't happen");
			return false;
		}
		return component.IsRoomView;
	}

	// Token: 0x06001260 RID: 4704 RVA: 0x0006499F File Offset: 0x00062B9F
	public override bool ShouldUpdateObject(GameObject obj)
	{
		return this.IsObjectLocallyOwned(obj);
	}

	// Token: 0x06001261 RID: 4705 RVA: 0x0006499F File Offset: 0x00062B9F
	public override bool ShouldWriteObjectData(GameObject obj)
	{
		return this.IsObjectLocallyOwned(obj);
	}

	// Token: 0x06001262 RID: 4706 RVA: 0x000649A8 File Offset: 0x00062BA8
	public override int GetOwningPlayerID(GameObject obj)
	{
		PhotonView photonView;
		if (obj.TryGetComponent<PhotonView>(out photonView) && photonView.Owner != null)
		{
			return photonView.Owner.ActorNumber;
		}
		return -1;
	}

	// Token: 0x06001263 RID: 4707 RVA: 0x000649D4 File Offset: 0x00062BD4
	public override bool ShouldSpawnLocally(int playerID)
	{
		return this.LocalPlayerID == playerID || (playerID == -1 && PhotonNetwork.MasterClient.IsLocal);
	}

	// Token: 0x06001264 RID: 4708 RVA: 0x00002076 File Offset: 0x00000276
	public override bool IsTotalAuthority()
	{
		return false;
	}

	// Token: 0x06001265 RID: 4709 RVA: 0x000649F1 File Offset: 0x00062BF1
	public void OnConnectedtoMaster()
	{
		if (this.internalState == NetworkSystemPUN.InternalState.ConnectingToMaster)
		{
			this.internalState = NetworkSystemPUN.InternalState.ConnectedToMaster;
		}
		base.UpdatePlayers();
	}

	// Token: 0x06001266 RID: 4710 RVA: 0x00064A09 File Offset: 0x00062C09
	public void OnJoinedRoom()
	{
		if (this.internalState == NetworkSystemPUN.InternalState.Searching_Joining)
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_Joined;
		}
		else if (this.internalState == NetworkSystemPUN.InternalState.Searching_Creating)
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_Created;
		}
		this.AddVoice();
		base.UpdatePlayers();
		base.JoinedNetworkRoom();
	}

	// Token: 0x06001267 RID: 4711 RVA: 0x00064A43 File Offset: 0x00062C43
	public void OnJoinRoomFailed(short returnCode, string message)
	{
		Debug.Log("onJoinRoomFailed " + returnCode.ToString() + message);
		if (this.internalState == NetworkSystemPUN.InternalState.Searching_Joining)
		{
			if (returnCode == 32765)
			{
				this.internalState = NetworkSystemPUN.InternalState.Searching_JoinFailed_Full;
				return;
			}
			this.internalState = NetworkSystemPUN.InternalState.Searching_JoinFailed;
		}
	}

	// Token: 0x06001268 RID: 4712 RVA: 0x00064A7F File Offset: 0x00062C7F
	public void OnCreateRoomFailed(short returnCode, string message)
	{
		if (this.internalState == NetworkSystemPUN.InternalState.Searching_Creating)
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_CreateFailed;
		}
	}

	// Token: 0x06001269 RID: 4713 RVA: 0x00064A94 File Offset: 0x00062C94
	public void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.UpdatePlayers();
		NetPlayer player = base.GetPlayer(newPlayer);
		base.PlayerJoined(player);
	}

	// Token: 0x0600126A RID: 4714 RVA: 0x00064AB8 File Offset: 0x00062CB8
	public void OnPlayerLeftRoom(Player otherPlayer)
	{
		NetPlayer player = base.GetPlayer(otherPlayer);
		base.UpdatePlayers();
		base.PlayerLeft(player);
	}

	// Token: 0x0600126B RID: 4715 RVA: 0x00064ADC File Offset: 0x00062CDC
	public void OnDisconnected(DisconnectCause cause)
	{
		NetworkSystemPUN.<OnDisconnected>d__114 <OnDisconnected>d__;
		<OnDisconnected>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnDisconnected>d__.<>4__this = this;
		<OnDisconnected>d__.<>1__state = -1;
		<OnDisconnected>d__.<>t__builder.Start<NetworkSystemPUN.<OnDisconnected>d__114>(ref <OnDisconnected>d__);
	}

	// Token: 0x0600126C RID: 4716 RVA: 0x00064B13 File Offset: 0x00062D13
	public void OnMasterClientSwitched(Player newMasterClient)
	{
		base.OnMasterClientSwitchedCallback(newMasterClient);
	}

	// Token: 0x0600126D RID: 4717 RVA: 0x00064B24 File Offset: 0x00062D24
	private ValueTuple<CancellationTokenSource, CancellationToken> GetCancellationToken()
	{
		CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		CancellationToken token = cancellationTokenSource.Token;
		this._taskCancelTokens.Add(cancellationTokenSource);
		return new ValueTuple<CancellationTokenSource, CancellationToken>(cancellationTokenSource, token);
	}

	// Token: 0x0600126E RID: 4718 RVA: 0x00064B54 File Offset: 0x00062D54
	public void ResetSystem()
	{
		if (this.VoiceNetworkObject)
		{
			Object.Destroy(this.VoiceNetworkObject);
		}
		PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = this.regionNames[this.lowestPingRegionIndex];
		this.currentRegionIndex = this.lowestPingRegionIndex;
		PhotonNetwork.Disconnect();
		this._taskCancelTokens.ForEach(delegate(CancellationTokenSource token)
		{
			token.Cancel();
			token.Dispose();
		});
		this._taskCancelTokens.Clear();
		this.internalState = NetworkSystemPUN.InternalState.Idle;
		base.netState = NetSystemState.Idle;
	}

	// Token: 0x0600126F RID: 4719 RVA: 0x00064BEC File Offset: 0x00062DEC
	private void UpdateZoneInfo(bool roomIsPublic, string zoneName = null)
	{
		AuthenticationValues authenticationValues = this.GetAuthenticationValues();
		Dictionary<string, object> dictionary = ((authenticationValues != null) ? authenticationValues.AuthPostData : null) as Dictionary<string, object>;
		if (dictionary != null)
		{
			dictionary["Zone"] = ((zoneName != null) ? zoneName : ((ZoneManagement.instance.activeZones.Count > 0) ? ZoneManagement.instance.activeZones.First<GTZone>().GetName<GTZone>() : ""));
			dictionary["SubZone"] = GTSubZone.none.GetName<GTSubZone>();
			dictionary["IsPublic"] = roomIsPublic;
			authenticationValues.SetAuthPostData(dictionary);
			this.SetAuthenticationValues(authenticationValues);
		}
	}

	// Token: 0x040019C7 RID: 6599
	private NetworkRegionInfo[] regionData;

	// Token: 0x040019C8 RID: 6600
	private Task<NetJoinResult> roomTask;

	// Token: 0x040019C9 RID: 6601
	private ObjectPool<PunNetPlayer> playerPool;

	// Token: 0x040019CA RID: 6602
	private NetPlayer[] m_allNetPlayers = new NetPlayer[0];

	// Token: 0x040019CB RID: 6603
	private NetPlayer[] m_otherNetPlayers = new NetPlayer[0];

	// Token: 0x040019CC RID: 6604
	private List<CancellationTokenSource> _taskCancelTokens = new List<CancellationTokenSource>();

	// Token: 0x040019CD RID: 6605
	private PhotonVoiceNetwork punVoice;

	// Token: 0x040019CE RID: 6606
	private GameObject VoiceNetworkObject;

	// Token: 0x040019CF RID: 6607
	private NetworkSystemPUN.InternalState currentState;

	// Token: 0x040019D0 RID: 6608
	private bool firstRoomJoin;

	// Token: 0x020002F4 RID: 756
	private enum InternalState
	{
		// Token: 0x040019D2 RID: 6610
		AwaitingAuth,
		// Token: 0x040019D3 RID: 6611
		Authenticated,
		// Token: 0x040019D4 RID: 6612
		PingGathering,
		// Token: 0x040019D5 RID: 6613
		StateCheckFailed,
		// Token: 0x040019D6 RID: 6614
		ConnectingToMaster,
		// Token: 0x040019D7 RID: 6615
		ConnectedToMaster,
		// Token: 0x040019D8 RID: 6616
		Idle,
		// Token: 0x040019D9 RID: 6617
		Internal_Disconnecting,
		// Token: 0x040019DA RID: 6618
		Internal_Disconnected,
		// Token: 0x040019DB RID: 6619
		Searching_Connecting,
		// Token: 0x040019DC RID: 6620
		Searching_Connected,
		// Token: 0x040019DD RID: 6621
		Searching_Joining,
		// Token: 0x040019DE RID: 6622
		Searching_Joined,
		// Token: 0x040019DF RID: 6623
		Searching_JoinFailed,
		// Token: 0x040019E0 RID: 6624
		Searching_JoinFailed_Full,
		// Token: 0x040019E1 RID: 6625
		Searching_Creating,
		// Token: 0x040019E2 RID: 6626
		Searching_Created,
		// Token: 0x040019E3 RID: 6627
		Searching_CreateFailed,
		// Token: 0x040019E4 RID: 6628
		Searching_Disconnecting,
		// Token: 0x040019E5 RID: 6629
		Searching_Disconnected
	}
}
