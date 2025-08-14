using System;
using System.Collections.Generic;
using System.Timers;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.Cosmetics;
using GorillaTagScripts;
using Photon.Pun;
using TagEffects;
using UnityEngine;

// Token: 0x02000A63 RID: 2659
internal class RoomSystem : MonoBehaviour
{
	// Token: 0x060040D3 RID: 16595 RVA: 0x00147F20 File Offset: 0x00146120
	internal static void DeserializeLaunchProjectile(object[] projectileData, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		GorillaNot.IncrementRPCCall(info, "LaunchSlingshotProjectile");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			return;
		}
		byte b = Convert.ToByte(projectileData[5]);
		byte b2 = Convert.ToByte(projectileData[6]);
		byte b3 = Convert.ToByte(projectileData[7]);
		byte b4 = Convert.ToByte(projectileData[8]);
		Color32 c = new Color32(b, b2, b3, b4);
		Vector3 position = (Vector3)projectileData[0];
		Vector3 velocity = (Vector3)projectileData[1];
		float num = 10000f;
		if (position.IsValid(num))
		{
			float num2 = 10000f;
			if (velocity.IsValid(num2) && float.IsFinite((float)b) && float.IsFinite((float)b2) && float.IsFinite((float)b3) && float.IsFinite((float)b4))
			{
				RoomSystem.ProjectileSource projectileSource = (RoomSystem.ProjectileSource)Convert.ToInt32(projectileData[2]);
				int projectileIndex = Convert.ToInt32(projectileData[3]);
				bool overridecolour = Convert.ToBoolean(projectileData[4]);
				VRRig rig = rigContainer.Rig;
				if (rig.isOfflineVRRig || rig.IsPositionInRange(position, 4f))
				{
					RoomSystem.launchProjectile.targetRig = rig;
					RoomSystem.launchProjectile.position = position;
					RoomSystem.launchProjectile.velocity = velocity;
					RoomSystem.launchProjectile.overridecolour = overridecolour;
					RoomSystem.launchProjectile.colour = c;
					RoomSystem.launchProjectile.projectileIndex = projectileIndex;
					RoomSystem.launchProjectile.projectileSource = projectileSource;
					RoomSystem.launchProjectile.messageInfo = info;
					FXSystem.PlayFXForRig(FXType.Projectile, RoomSystem.launchProjectile, info);
				}
				return;
			}
		}
		GorillaNot.instance.SendReport("invalid projectile state", player.UserId, player.NickName);
	}

	// Token: 0x060040D4 RID: 16596 RVA: 0x001480B8 File Offset: 0x001462B8
	internal static void SendLaunchProjectile(Vector3 position, Vector3 velocity, RoomSystem.ProjectileSource projectileSource, int projectileCount, bool randomColour, byte r, byte g, byte b, byte a)
	{
		if (!RoomSystem.JoinedRoom)
		{
			return;
		}
		RoomSystem.projectileSendData[0] = position;
		RoomSystem.projectileSendData[1] = velocity;
		RoomSystem.projectileSendData[2] = projectileSource;
		RoomSystem.projectileSendData[3] = projectileCount;
		RoomSystem.projectileSendData[4] = randomColour;
		RoomSystem.projectileSendData[5] = r;
		RoomSystem.projectileSendData[6] = g;
		RoomSystem.projectileSendData[7] = b;
		RoomSystem.projectileSendData[8] = a;
		byte b2 = 0;
		object obj = RoomSystem.projectileSendData;
		RoomSystem.SendEvent(b2, obj, NetworkSystemRaiseEvent.neoOthers, false);
	}

	// Token: 0x060040D5 RID: 16597 RVA: 0x00148160 File Offset: 0x00146360
	internal static void ImpactEffect(VRRig targetRig, Vector3 position, float r, float g, float b, float a, int projectileCount, PhotonMessageInfoWrapped info = default(PhotonMessageInfoWrapped))
	{
		RoomSystem.impactEffect.targetRig = targetRig;
		RoomSystem.impactEffect.position = position;
		RoomSystem.impactEffect.colour = new Color(r, g, b, a);
		RoomSystem.impactEffect.projectileIndex = projectileCount;
		FXSystem.PlayFXForRig(FXType.Impact, RoomSystem.impactEffect, info);
	}

	// Token: 0x060040D6 RID: 16598 RVA: 0x001481B4 File Offset: 0x001463B4
	internal static void DeserializeImpactEffect(object[] impactData, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		GorillaNot.IncrementRPCCall(info, "SpawnSlingshotPlayerImpactEffect");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || rigContainer.Rig.projectileWeapon.IsNull())
		{
			return;
		}
		float num = Convert.ToSingle(impactData[1]);
		float num2 = Convert.ToSingle(impactData[2]);
		float num3 = Convert.ToSingle(impactData[3]);
		float num4 = Convert.ToSingle(impactData[4]);
		Vector3 position = (Vector3)impactData[0];
		float num5 = 10000f;
		if (!position.IsValid(num5) || !float.IsFinite(num) || !float.IsFinite(num2) || !float.IsFinite(num3) || !float.IsFinite(num4))
		{
			GorillaNot.instance.SendReport("invalid impact state", player.UserId, player.NickName);
			return;
		}
		int projectileCount = Convert.ToInt32(impactData[5]);
		RoomSystem.ImpactEffect(rigContainer.Rig, position, num, num2, num3, num4, projectileCount, info);
	}

	// Token: 0x060040D7 RID: 16599 RVA: 0x001482A4 File Offset: 0x001464A4
	internal static void SendImpactEffect(Vector3 position, float r, float g, float b, float a, int projectileCount)
	{
		RoomSystem.ImpactEffect(VRRigCache.Instance.localRig.Rig, position, r, g, b, a, projectileCount, default(PhotonMessageInfoWrapped));
		if (RoomSystem.joinedRoom)
		{
			RoomSystem.impactSendData[0] = position;
			RoomSystem.impactSendData[1] = r;
			RoomSystem.impactSendData[2] = g;
			RoomSystem.impactSendData[3] = b;
			RoomSystem.impactSendData[4] = a;
			RoomSystem.impactSendData[5] = projectileCount;
			byte b2 = 1;
			object obj = RoomSystem.impactSendData;
			RoomSystem.SendEvent(b2, obj, NetworkSystemRaiseEvent.neoOthers, false);
		}
	}

	// Token: 0x060040D8 RID: 16600 RVA: 0x00148344 File Offset: 0x00146544
	private void Awake()
	{
		base.transform.SetParent(null, true);
		Object.DontDestroyOnLoad(this);
		RoomSystem.playerImpactEffectPrefab = this.roomSettings.PlayerImpactEffect;
		RoomSystem.callbackInstance = this;
		RoomSystem.disconnectTimer.Interval = (double)(this.roomSettings.PausedDCTimer * 1000);
		RoomSystem.playerEffectDictionary.Clear();
		foreach (RoomSystem.PlayerEffectConfig playerEffectConfig in this.roomSettings.PlayerEffects)
		{
			RoomSystem.playerEffectDictionary.Add(playerEffectConfig.type, playerEffectConfig);
		}
	}

	// Token: 0x060040D9 RID: 16601 RVA: 0x001483F8 File Offset: 0x001465F8
	private void Start()
	{
		List<PhotonView> list = new List<PhotonView>(20);
		foreach (PhotonView photonView in PhotonNetwork.PhotonViewCollection)
		{
			if (photonView.IsRoomView)
			{
				list.Add(photonView);
			}
		}
		RoomSystem.sceneViews = list.ToArray();
		NetworkSystem.Instance.OnRaiseEvent += RoomSystem.OnEvent;
		NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerLeftRoom;
		NetworkSystem.Instance.OnPlayerJoined += this.OnPlayerEnteredRoom;
		NetworkSystem.Instance.OnMultiplayerStarted += this.OnJoinedRoom;
		NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnLeftRoom;
	}

	// Token: 0x060040DA RID: 16602 RVA: 0x00148504 File Offset: 0x00146704
	private void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			RoomSystem.disconnectTimer.Stop();
			return;
		}
		if (RoomSystem.JoinedRoom)
		{
			RoomSystem.disconnectTimer.Start();
		}
	}

	// Token: 0x060040DB RID: 16603 RVA: 0x00148528 File Offset: 0x00146728
	private void OnJoinedRoom()
	{
		RoomSystem.joinedRoom = true;
		foreach (NetPlayer item in NetworkSystem.Instance.AllNetPlayers)
		{
			RoomSystem.netPlayersInRoom.Add(item);
		}
		PlayerCosmeticsSystem.UpdatePlayerCosmetics(RoomSystem.netPlayersInRoom);
		RoomSystem.roomGameMode = NetworkSystem.Instance.GameModeString;
		if (NetworkSystem.Instance.IsMasterClient)
		{
			for (int j = 0; j < this.prefabsToInstantiateByPath.Length; j++)
			{
				this.prefabsInstantiated.Add(NetworkSystem.Instance.NetInstantiate(this.prefabsToInstantiate[j], Vector3.zero, Quaternion.identity, true));
			}
		}
		try
		{
			RoomSystem.m_roomSizeOnJoin = PhotonNetwork.CurrentRoom.MaxPlayers;
			this.roomSettings.ExpectedUsersTimer.Start();
			DelegateListProcessor joinedRoomEvent = RoomSystem.JoinedRoomEvent;
			if (joinedRoomEvent != null)
			{
				joinedRoomEvent.InvokeSafe();
			}
		}
		catch (Exception)
		{
			Debug.LogError("RoomSystem failed invoking event");
		}
	}

	// Token: 0x060040DC RID: 16604 RVA: 0x00148614 File Offset: 0x00146814
	private void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		if (newPlayer.IsLocal)
		{
			return;
		}
		if (!RoomSystem.netPlayersInRoom.Contains(newPlayer))
		{
			RoomSystem.netPlayersInRoom.Add(newPlayer);
		}
		PlayerCosmeticsSystem.UpdatePlayerCosmetics(newPlayer);
		try
		{
			DelegateListProcessor<NetPlayer> playerJoinedEvent = RoomSystem.PlayerJoinedEvent;
			if (playerJoinedEvent != null)
			{
				playerJoinedEvent.InvokeSafe(newPlayer);
			}
			DelegateListProcessor playersChangedEvent = RoomSystem.PlayersChangedEvent;
			if (playersChangedEvent != null)
			{
				playersChangedEvent.InvokeSafe();
			}
		}
		catch (Exception)
		{
			Debug.LogError("RoomSystem failed invoking event");
		}
	}

	// Token: 0x060040DD RID: 16605 RVA: 0x0014868C File Offset: 0x0014688C
	private void OnLeftRoom()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		RoomSystem.joinedRoom = false;
		RoomSystem.netPlayersInRoom.Clear();
		RoomSystem.roomGameMode = "";
		PlayerCosmeticsSystem.StaticReset();
		int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
		for (int i = 0; i < RoomSystem.sceneViews.Length; i++)
		{
			RoomSystem.sceneViews[i].ControllerActorNr = actorNumber;
			RoomSystem.sceneViews[i].OwnerActorNr = actorNumber;
		}
		this.roomSettings.StatusEffectLimiter.Reset();
		this.roomSettings.SoundEffectLimiter.Reset();
		this.roomSettings.SoundEffectOtherLimiter.Reset();
		this.roomSettings.PlayerEffectLimiter.Reset();
		try
		{
			RoomSystem.m_roomSizeOnJoin = 0;
			this.roomSettings.ExpectedUsersTimer.Stop();
			DelegateListProcessor leftRoomEvent = RoomSystem.LeftRoomEvent;
			if (leftRoomEvent != null)
			{
				leftRoomEvent.InvokeSafe();
			}
		}
		catch (Exception)
		{
			Debug.LogError("RoomSystem failed invoking event");
		}
		GC.Collect(0);
	}

	// Token: 0x060040DE RID: 16606 RVA: 0x00148788 File Offset: 0x00146988
	private void OnPlayerLeftRoom(NetPlayer netPlayer)
	{
		if (netPlayer == null)
		{
			Debug.LogError("Player how left doesnt have a reference somehow");
		}
		foreach (NetPlayer netPlayer2 in RoomSystem.netPlayersInRoom)
		{
			if (netPlayer2 == netPlayer)
			{
				RoomSystem.netPlayersInRoom.Remove(netPlayer2);
				break;
			}
		}
		RoomSystem.netPlayersInRoom.Remove(netPlayer);
		try
		{
			DelegateListProcessor<NetPlayer> playerLeftEvent = RoomSystem.PlayerLeftEvent;
			if (playerLeftEvent != null)
			{
				playerLeftEvent.InvokeSafe(netPlayer);
			}
			DelegateListProcessor playersChangedEvent = RoomSystem.PlayersChangedEvent;
			if (playersChangedEvent != null)
			{
				playersChangedEvent.InvokeSafe();
			}
		}
		catch (Exception)
		{
			Debug.LogError("RoomSystem failed invoking event");
		}
	}

	// Token: 0x17000626 RID: 1574
	// (get) Token: 0x060040DF RID: 16607 RVA: 0x0014883C File Offset: 0x00146A3C
	// (set) Token: 0x060040E0 RID: 16608 RVA: 0x00148843 File Offset: 0x00146A43
	private static bool UseRoomSizeOverride { get; set; }

	// Token: 0x17000627 RID: 1575
	// (get) Token: 0x060040E1 RID: 16609 RVA: 0x0014884B File Offset: 0x00146A4B
	// (set) Token: 0x060040E2 RID: 16610 RVA: 0x00148852 File Offset: 0x00146A52
	public static byte RoomSizeOverride { get; set; }

	// Token: 0x17000628 RID: 1576
	// (get) Token: 0x060040E3 RID: 16611 RVA: 0x0014885A File Offset: 0x00146A5A
	public static List<NetPlayer> PlayersInRoom
	{
		get
		{
			return RoomSystem.netPlayersInRoom;
		}
	}

	// Token: 0x17000629 RID: 1577
	// (get) Token: 0x060040E4 RID: 16612 RVA: 0x00148861 File Offset: 0x00146A61
	public static string RoomGameMode
	{
		get
		{
			return RoomSystem.roomGameMode;
		}
	}

	// Token: 0x1700062A RID: 1578
	// (get) Token: 0x060040E5 RID: 16613 RVA: 0x00148868 File Offset: 0x00146A68
	public static bool JoinedRoom
	{
		get
		{
			return NetworkSystem.Instance.InRoom && RoomSystem.joinedRoom;
		}
	}

	// Token: 0x1700062B RID: 1579
	// (get) Token: 0x060040E6 RID: 16614 RVA: 0x0014887D File Offset: 0x00146A7D
	public static bool AmITheHost
	{
		get
		{
			return NetworkSystem.Instance.IsMasterClient || !NetworkSystem.Instance.InRoom;
		}
	}

	// Token: 0x060040E7 RID: 16615 RVA: 0x0014889C File Offset: 0x00146A9C
	static RoomSystem()
	{
		RoomSystem.disconnectTimer.Elapsed += RoomSystem.TimerDC;
		RoomSystem.disconnectTimer.AutoReset = false;
		RoomSystem.StaticLoad();
	}

	// Token: 0x060040E8 RID: 16616 RVA: 0x001489FC File Offset: 0x00146BFC
	[OnEnterPlay_Run]
	private static void StaticLoad()
	{
		RoomSystem.netEventCallbacks[0] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializeLaunchProjectile);
		RoomSystem.netEventCallbacks[1] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializeImpactEffect);
		RoomSystem.netEventCallbacks[4] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.SearchForNearby);
		RoomSystem.netEventCallbacks[7] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.SearchForParty);
		RoomSystem.netEventCallbacks[10] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.SearchForElevator);
		RoomSystem.netEventCallbacks[2] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializeStatusEffect);
		RoomSystem.netEventCallbacks[3] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializeSoundEffect);
		RoomSystem.netEventCallbacks[5] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializeReportTouch);
		RoomSystem.netEventCallbacks[8] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializePlayerLaunched);
		RoomSystem.netEventCallbacks[6] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializePlayerEffect);
		RoomSystem.netEventCallbacks[9] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializePlayerHit);
		RoomSystem.soundEffectCallback = new Action<RoomSystem.SoundEffect, NetPlayer>(RoomSystem.OnPlaySoundEffect);
		RoomSystem.statusEffectCallback = new Action<RoomSystem.StatusEffects>(RoomSystem.OnStatusEffect);
	}

	// Token: 0x060040E9 RID: 16617 RVA: 0x00148B2A File Offset: 0x00146D2A
	private static void TimerDC(object sender, ElapsedEventArgs args)
	{
		RoomSystem.disconnectTimer.Stop();
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		PhotonNetwork.Disconnect();
		PhotonNetwork.SendAllOutgoingCommands();
	}

	// Token: 0x060040EA RID: 16618 RVA: 0x00148B48 File Offset: 0x00146D48
	public static byte GetRoomSize(string gameMode = "")
	{
		if (RoomSystem.joinedRoom)
		{
			if (RoomSystem.m_roomSizeOnJoin > 10)
			{
				return 10;
			}
			return RoomSystem.m_roomSizeOnJoin;
		}
		else
		{
			if (RoomSystem.UseRoomSizeOverride)
			{
				return RoomSystem.RoomSizeOverride;
			}
			return 10;
		}
	}

	// Token: 0x060040EB RID: 16619 RVA: 0x00148B72 File Offset: 0x00146D72
	public static byte GetRoomSizeForCreate(string gameMode = "")
	{
		if (RoomSystem.UseRoomSizeOverride)
		{
			return RoomSystem.RoomSizeOverride;
		}
		return 10;
	}

	// Token: 0x060040EC RID: 16620 RVA: 0x00148B83 File Offset: 0x00146D83
	public static void OverrideRoomSize(byte roomSize)
	{
		if (roomSize < 1)
		{
			roomSize = 1;
		}
		if (roomSize > 10)
		{
			roomSize = 10;
		}
		if (roomSize == 10)
		{
			RoomSystem.UseRoomSizeOverride = false;
			RoomSystem.RoomSizeOverride = 10;
			return;
		}
		RoomSystem.UseRoomSizeOverride = true;
		RoomSystem.RoomSizeOverride = roomSize;
	}

	// Token: 0x060040ED RID: 16621 RVA: 0x00148B72 File Offset: 0x00146D72
	public static byte GetOverridenRoomSize()
	{
		if (RoomSystem.UseRoomSizeOverride)
		{
			return RoomSystem.RoomSizeOverride;
		}
		return 10;
	}

	// Token: 0x060040EE RID: 16622 RVA: 0x00148BB4 File Offset: 0x00146DB4
	public static void ClearOverridenRoomSize()
	{
		RoomSystem.UseRoomSizeOverride = false;
		RoomSystem.RoomSizeOverride = 10;
	}

	// Token: 0x060040EF RID: 16623 RVA: 0x00148BC3 File Offset: 0x00146DC3
	public static void MakeRoomMultiplayer(byte roomSize)
	{
		if (!RoomSystem.joinedRoom || RoomSystem.m_roomSizeOnJoin > 1)
		{
			return;
		}
		if (roomSize > 10)
		{
			roomSize = 10;
		}
		RoomSystem.m_roomSizeOnJoin = roomSize;
		PhotonNetwork.CurrentRoom.MaxPlayers = roomSize;
	}

	// Token: 0x060040F0 RID: 16624 RVA: 0x00148BEF File Offset: 0x00146DEF
	internal static void SendEvent(in byte code, in object evData, in NetPlayer target, bool reliable)
	{
		NetworkSystemRaiseEvent.neoTarget.TargetActors[0] = target.ActorNumber;
		RoomSystem.SendEvent(code, evData, NetworkSystemRaiseEvent.neoTarget, reliable);
	}

	// Token: 0x060040F1 RID: 16625 RVA: 0x00148C11 File Offset: 0x00146E11
	internal static void SendEvent(in byte code, in object evData, in NetEventOptions neo, bool reliable)
	{
		RoomSystem.sendEventData[0] = NetworkSystem.Instance.ServerTimestamp;
		RoomSystem.sendEventData[1] = code;
		RoomSystem.sendEventData[2] = evData;
		NetworkSystemRaiseEvent.RaiseEvent(3, RoomSystem.sendEventData, neo, reliable);
	}

	// Token: 0x060040F2 RID: 16626 RVA: 0x00148C4E File Offset: 0x00146E4E
	private static void OnEvent(EventData data)
	{
		RoomSystem.OnEvent(data.Code, data.CustomData, data.Sender);
	}

	// Token: 0x060040F3 RID: 16627 RVA: 0x00148C68 File Offset: 0x00146E68
	private static void OnEvent(byte code, object data, int source)
	{
		NetPlayer netPlayer;
		if (code != 3 || !Utils.PlayerInRoom(source, out netPlayer))
		{
			return;
		}
		try
		{
			object[] array = (object[])data;
			int tick = Convert.ToInt32(array[0]);
			byte key = Convert.ToByte(array[1]);
			object[] arg = null;
			if (array.Length > 2)
			{
				object obj = array[2];
				arg = ((obj == null) ? null : ((object[])obj));
			}
			PhotonMessageInfoWrapped arg2 = new PhotonMessageInfoWrapped(netPlayer.ActorNumber, tick);
			Action<object[], PhotonMessageInfoWrapped> action;
			if (RoomSystem.netEventCallbacks.TryGetValue(key, out action))
			{
				action(arg, arg2);
			}
		}
		catch
		{
		}
	}

	// Token: 0x060040F4 RID: 16628 RVA: 0x00148CFC File Offset: 0x00146EFC
	internal static void SearchForNearby(object[] shuffleData, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		GorillaNot.IncrementRPCCall(info, "JoinPubWithNearby");
		string shufflerStr = (string)shuffleData[0];
		string newKeyStr = (string)shuffleData[1];
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups);
		if (!GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(NetworkSystem.Instance.LocalPlayer.UserId))
		{
			GorillaNot.instance.SendReport("possible kick attempt", player.UserId, player.NickName);
			return;
		}
		if (!flag || !NetworkSystem.Instance.SessionIsPrivate)
		{
			return;
		}
		PhotonNetworkController.Instance.AttemptToFollowIntoPub(player.UserId, player.ActorNumber, newKeyStr, shufflerStr, JoinType.FollowingNearby);
	}

	// Token: 0x060040F5 RID: 16629 RVA: 0x00148DB0 File Offset: 0x00146FB0
	internal static void SearchForParty(object[] shuffleData, PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "PARTY_JOIN");
		string shufflerStr = (string)shuffleData[0];
		string newKeyStr = (string)shuffleData[1];
		if (!FriendshipGroupDetection.Instance.IsInMyGroup(info.Sender.UserId))
		{
			GorillaNot.instance.SendReport("possible kick attempt", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		if (PlayFabAuthenticator.instance.GetSafety())
		{
			return;
		}
		PhotonNetworkController.Instance.AttemptToFollowIntoPub(info.Sender.UserId, info.Sender.ActorNumber, newKeyStr, shufflerStr, JoinType.FollowingParty);
	}

	// Token: 0x060040F6 RID: 16630 RVA: 0x00148E54 File Offset: 0x00147054
	internal static void SearchForElevator(object[] shuffleData, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		GorillaNot.IncrementRPCCall(info, "JoinPubWithElevator");
		string shufflerStr = (string)shuffleData[0];
		string newKeyStr = (string)shuffleData[1];
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups);
		if (GRElevatorManager.ValidElevatorNetworking(info.Sender.ActorNumber) && GRElevatorManager.ValidElevatorNetworking(NetworkSystem.Instance.LocalPlayer.ActorNumber))
		{
			if (!flag)
			{
				GRElevatorManager.JoinPublicRoom();
				return;
			}
			PhotonNetworkController.Instance.AttemptToFollowIntoPub(player.UserId, player.ActorNumber, newKeyStr, shufflerStr, JoinType.JoinWithElevator);
		}
	}

	// Token: 0x060040F7 RID: 16631 RVA: 0x00148EE4 File Offset: 0x001470E4
	internal static void SendNearbyFollowCommand(GorillaFriendCollider friendCollider, string shuffler, string keyStr)
	{
		RoomSystem.groupJoinSendData[0] = shuffler;
		RoomSystem.groupJoinSendData[1] = keyStr;
		NetEventOptions netEventOptions = new NetEventOptions
		{
			TargetActors = new int[1]
		};
		foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
		{
			if (friendCollider.playerIDsCurrentlyTouching.Contains(netPlayer.UserId) && netPlayer != NetworkSystem.Instance.LocalPlayer)
			{
				netEventOptions.TargetActors[0] = netPlayer.ActorNumber;
				byte b = 4;
				object obj = RoomSystem.groupJoinSendData;
				RoomSystem.SendEvent(b, obj, netEventOptions, false);
			}
		}
	}

	// Token: 0x060040F8 RID: 16632 RVA: 0x00148F94 File Offset: 0x00147194
	internal static void SendPartyFollowCommand(string shuffler, string keyStr)
	{
		RoomSystem.groupJoinSendData[0] = shuffler;
		RoomSystem.groupJoinSendData[1] = keyStr;
		NetEventOptions netEventOptions = new NetEventOptions
		{
			TargetActors = new int[1]
		};
		foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
		{
			if (vrrig.IsLocalPartyMember && vrrig.creator != NetworkSystem.Instance.LocalPlayer)
			{
				netEventOptions.TargetActors[0] = vrrig.creator.ActorNumber;
				Debug.Log(string.Format("SendGroupFollowCommand - sendEvent to {0} from {1}, shuffler {2} key {3}", new object[]
				{
					vrrig.creator.NickName,
					NetworkSystem.Instance.LocalPlayer.UserId,
					RoomSystem.groupJoinSendData[0],
					RoomSystem.groupJoinSendData[1]
				}));
				byte b = 7;
				object obj = RoomSystem.groupJoinSendData;
				RoomSystem.SendEvent(b, obj, netEventOptions, false);
			}
		}
	}

	// Token: 0x060040F9 RID: 16633 RVA: 0x0014909C File Offset: 0x0014729C
	internal static void SendElevatorFollowCommand(string shuffler, string keyStr, GorillaFriendCollider sourceFriendCollider, GorillaFriendCollider targetFriendCollider)
	{
		RoomSystem.groupJoinSendData[0] = shuffler;
		RoomSystem.groupJoinSendData[1] = keyStr;
		NetEventOptions netEventOptions = new NetEventOptions
		{
			TargetActors = new int[1]
		};
		foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
		{
			if (sourceFriendCollider.playerIDsCurrentlyTouching.Contains(netPlayer.UserId) || (targetFriendCollider.playerIDsCurrentlyTouching.Contains(netPlayer.UserId) && netPlayer != NetworkSystem.Instance.LocalPlayer))
			{
				netEventOptions.TargetActors[0] = netPlayer.ActorNumber;
				Debug.Log(string.Format("SendElevatorFollowCommand - sendEvent to {0} from {1}, shuffler {2} key {3}", new object[]
				{
					netPlayer.NickName,
					NetworkSystem.Instance.LocalPlayer.UserId,
					RoomSystem.groupJoinSendData[0],
					RoomSystem.groupJoinSendData[1]
				}));
				byte b = 10;
				object obj = RoomSystem.groupJoinSendData;
				RoomSystem.SendEvent(b, obj, netEventOptions, false);
			}
		}
	}

	// Token: 0x060040FA RID: 16634 RVA: 0x001491AC File Offset: 0x001473AC
	private static void DeserializeReportTouch(object[] data, PhotonMessageInfoWrapped info)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		NetPlayer arg = (NetPlayer)data[0];
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		Action<NetPlayer, NetPlayer> action = RoomSystem.playerTouchedCallback;
		if (action == null)
		{
			return;
		}
		action(arg, player);
	}

	// Token: 0x060040FB RID: 16635 RVA: 0x001491F4 File Offset: 0x001473F4
	internal static void SendReportTouch(NetPlayer touchedNetPlayer)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			RoomSystem.reportTouchSendData[0] = touchedNetPlayer;
			byte b = 5;
			object obj = RoomSystem.reportTouchSendData;
			RoomSystem.SendEvent(b, obj, NetworkSystemRaiseEvent.neoMaster, false);
			return;
		}
		Action<NetPlayer, NetPlayer> action = RoomSystem.playerTouchedCallback;
		if (action == null)
		{
			return;
		}
		action(touchedNetPlayer, NetworkSystem.Instance.LocalPlayer);
	}

	// Token: 0x060040FC RID: 16636 RVA: 0x00149248 File Offset: 0x00147448
	internal static void LaunchPlayer(NetPlayer player, Vector3 velocity)
	{
		RoomSystem.reportTouchSendData[0] = velocity;
		byte b = 8;
		object obj = RoomSystem.reportTouchSendData;
		RoomSystem.SendEvent(b, obj, player, false);
	}

	// Token: 0x060040FD RID: 16637 RVA: 0x00149278 File Offset: 0x00147478
	private static void DeserializePlayerLaunched(object[] data, PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "DeserializePlayerLaunched");
		GorillaGameManager activeGameMode = GameMode.ActiveGameMode;
		if (activeGameMode != null && activeGameMode.GameType() == GameModeType.Guardian && info.Sender == NetworkSystem.Instance.MasterClient)
		{
			object obj = data[0];
			if (obj is Vector3)
			{
				Vector3 velocity = (Vector3)obj;
				float num = 10000f;
				if (velocity.IsValid(num) && velocity.magnitude <= 20f && RoomSystem.playerLaunchedCallLimiter.CheckCallTime(Time.time))
				{
					GTPlayer.Instance.DoLaunch(velocity);
					return;
				}
			}
		}
	}

	// Token: 0x060040FE RID: 16638 RVA: 0x0014930C File Offset: 0x0014750C
	internal static void HitPlayer(NetPlayer player, Vector3 direction, float strength)
	{
		RoomSystem.reportHitSendData[0] = direction;
		RoomSystem.reportHitSendData[1] = strength;
		RoomSystem.reportHitSendData[2] = player.ActorNumber;
		byte b = 9;
		object obj = RoomSystem.reportHitSendData;
		RoomSystem.SendEvent(b, obj, NetworkSystemRaiseEvent.neoOthers, false);
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			rigContainer.Rig.DisableHitWithKnockBack();
		}
	}

	// Token: 0x060040FF RID: 16639 RVA: 0x00149378 File Offset: 0x00147578
	private static void DeserializePlayerHit(object[] data, PhotonMessageInfoWrapped info)
	{
		object obj = data[0];
		if (obj is Vector3)
		{
			Vector3 vector = (Vector3)obj;
			obj = data[1];
			if (obj is float)
			{
				float value = (float)obj;
				obj = data[2];
				if (obj is int)
				{
					int num = (int)obj;
					float num2 = 10000f;
					RigContainer rigContainer;
					if (vector.IsValid(num2) && VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer) && FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 20, info.SentServerTime))
					{
						float num3 = value.ClampSafe(0f, 10f);
						GorillaNot.IncrementRPCCall(info, "DeserializePlayerHit");
						if (num == NetworkSystem.Instance.LocalPlayer.ActorNumber)
						{
							CosmeticEffectsOnPlayers.CosmeticEffect cosmeticEffect;
							CosmeticEffectsOnPlayers.CosmeticEffect cosmeticEffect2;
							if (GorillaTagger.Instance.offlineVRRig.TemporaryCosmeticEffects.TryGetValue(CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback, out cosmeticEffect))
							{
								if (!cosmeticEffect.IsGameModeAllowed())
								{
									return;
								}
								float speed = (num3 * cosmeticEffect.knockbackStrength * cosmeticEffect.knockbackStrengthMultiplier).ClampSafe(cosmeticEffect.minKnockbackStrength, cosmeticEffect.maxKnockbackStrength);
								GTPlayer.Instance.ApplyKnockback(vector.normalized, speed, cosmeticEffect.forceOffTheGround);
							}
							else if (GorillaTagger.Instance.offlineVRRig.TemporaryCosmeticEffects.TryGetValue(CosmeticEffectsOnPlayers.EFFECTTYPE.InstantKnockback, out cosmeticEffect2))
							{
								if (!cosmeticEffect2.IsGameModeAllowed())
								{
									return;
								}
								float speed2 = (num3 * cosmeticEffect2.knockbackStrength * cosmeticEffect2.knockbackStrengthMultiplier).ClampSafe(cosmeticEffect2.minKnockbackStrength, cosmeticEffect2.maxKnockbackStrength);
								GTPlayer.Instance.ApplyKnockback(vector.normalized, speed2, cosmeticEffect2.forceOffTheGround);
							}
						}
						NetPlayer player = NetworkSystem.Instance.GetPlayer(num);
						RigContainer rigContainer2;
						if (player != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer2))
						{
							rigContainer2.Rig.DisableHitWithKnockBack();
						}
						return;
					}
				}
			}
		}
	}

	// Token: 0x06004100 RID: 16640 RVA: 0x00149534 File Offset: 0x00147734
	private static void SetSlowedTime()
	{
		if (GorillaTagger.Instance.currentStatus != GorillaTagger.StatusEffect.Slowed)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
		}
		GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Slowed, GorillaTagger.Instance.slowCooldown);
		GorillaTagger.Instance.offlineVRRig.PlayTaggedEffect();
	}

	// Token: 0x06004101 RID: 16641 RVA: 0x001495B0 File Offset: 0x001477B0
	private static void SetTaggedTime()
	{
		GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, GorillaTagger.Instance.tagCooldown);
		GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
		GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
		GorillaTagger.Instance.offlineVRRig.PlayTaggedEffect();
	}

	// Token: 0x06004102 RID: 16642 RVA: 0x00149620 File Offset: 0x00147820
	private static void SetFrozenTime()
	{
		GorillaFreezeTagManager gorillaFreezeTagManager = GameMode.ActiveGameMode as GorillaFreezeTagManager;
		if (gorillaFreezeTagManager != null)
		{
			GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Slowed, gorillaFreezeTagManager.freezeDuration);
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
			GorillaTagger.Instance.offlineVRRig.PlayTaggedEffect();
		}
	}

	// Token: 0x06004103 RID: 16643 RVA: 0x00149699 File Offset: 0x00147899
	private static void SetJoinedTaggedTime()
	{
		GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
		GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
	}

	// Token: 0x06004104 RID: 16644 RVA: 0x001496DC File Offset: 0x001478DC
	private static void SetUntaggedTime()
	{
		GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.None, 0f);
		GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
		GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
	}

	// Token: 0x06004105 RID: 16645 RVA: 0x00149737 File Offset: 0x00147937
	private static void OnStatusEffect(RoomSystem.StatusEffects status)
	{
		switch (status)
		{
		case RoomSystem.StatusEffects.TaggedTime:
			RoomSystem.SetTaggedTime();
			return;
		case RoomSystem.StatusEffects.JoinedTaggedTime:
			RoomSystem.SetJoinedTaggedTime();
			return;
		case RoomSystem.StatusEffects.SetSlowedTime:
			RoomSystem.SetSlowedTime();
			return;
		case RoomSystem.StatusEffects.UnTagged:
			RoomSystem.SetUntaggedTime();
			return;
		case RoomSystem.StatusEffects.FrozenTime:
			RoomSystem.SetFrozenTime();
			return;
		default:
			return;
		}
	}

	// Token: 0x06004106 RID: 16646 RVA: 0x00149774 File Offset: 0x00147974
	private static void DeserializeStatusEffect(object[] data, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		GorillaNot.IncrementRPCCall(info, "DeserializeStatusEffect");
		if (!player.IsMasterClient)
		{
			GorillaNot.instance.SendReport("invalid status", player.UserId, player.NickName);
			return;
		}
		if (!RoomSystem.callbackInstance.roomSettings.StatusEffectLimiter.CheckCallServerTime(info.SentServerTime))
		{
			return;
		}
		RoomSystem.StatusEffects obj = (RoomSystem.StatusEffects)Convert.ToInt32(data[0]);
		Action<RoomSystem.StatusEffects> action = RoomSystem.statusEffectCallback;
		if (action == null)
		{
			return;
		}
		action(obj);
	}

	// Token: 0x06004107 RID: 16647 RVA: 0x001497FC File Offset: 0x001479FC
	internal static void SendStatusEffectAll(RoomSystem.StatusEffects status)
	{
		Action<RoomSystem.StatusEffects> action = RoomSystem.statusEffectCallback;
		if (action != null)
		{
			action(status);
		}
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		RoomSystem.statusSendData[0] = (int)status;
		byte b = 2;
		object obj = RoomSystem.statusSendData;
		RoomSystem.SendEvent(b, obj, NetworkSystemRaiseEvent.neoOthers, false);
	}

	// Token: 0x06004108 RID: 16648 RVA: 0x00149848 File Offset: 0x00147A48
	internal static void SendStatusEffectToPlayer(RoomSystem.StatusEffects status, NetPlayer target)
	{
		if (!target.IsLocal)
		{
			RoomSystem.statusSendData[0] = (int)status;
			byte b = 2;
			object obj = RoomSystem.statusSendData;
			RoomSystem.SendEvent(b, obj, target, false);
			return;
		}
		Action<RoomSystem.StatusEffects> action = RoomSystem.statusEffectCallback;
		if (action == null)
		{
			return;
		}
		action(status);
	}

	// Token: 0x06004109 RID: 16649 RVA: 0x0014988F File Offset: 0x00147A8F
	internal static void PlaySoundEffect(int soundIndex, float soundVolume, bool stopCurrentAudio)
	{
		VRRigCache.Instance.localRig.Rig.PlayTagSoundLocal(soundIndex, soundVolume, stopCurrentAudio);
	}

	// Token: 0x0600410A RID: 16650 RVA: 0x001498A8 File Offset: 0x00147AA8
	internal static void PlaySoundEffect(int soundIndex, float soundVolume, bool stopCurrentAudio, NetPlayer target)
	{
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(target, out rigContainer))
		{
			rigContainer.Rig.PlayTagSoundLocal(soundIndex, soundVolume, stopCurrentAudio);
		}
	}

	// Token: 0x0600410B RID: 16651 RVA: 0x001498D2 File Offset: 0x00147AD2
	private static void OnPlaySoundEffect(RoomSystem.SoundEffect sound, NetPlayer target)
	{
		if (target.IsLocal)
		{
			RoomSystem.PlaySoundEffect(sound.id, sound.volume, sound.stopCurrentAudio);
			return;
		}
		RoomSystem.PlaySoundEffect(sound.id, sound.volume, sound.stopCurrentAudio, target);
	}

	// Token: 0x0600410C RID: 16652 RVA: 0x0014990C File Offset: 0x00147B0C
	private static void DeserializeSoundEffect(object[] data, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		GorillaNot.IncrementRPCCall(info, "DeserializeSoundEffect");
		if (!player.Equals(NetworkSystem.Instance.MasterClient))
		{
			GorillaNot.instance.SendReport("invalid sound effect", player.UserId, player.NickName);
			return;
		}
		RoomSystem.SoundEffect soundEffect;
		soundEffect.id = Convert.ToInt32(data[0]);
		soundEffect.volume = Convert.ToSingle(data[1]);
		soundEffect.stopCurrentAudio = Convert.ToBoolean(data[2]);
		if (!float.IsFinite(soundEffect.volume))
		{
			return;
		}
		NetPlayer arg;
		if (data.Length > 3)
		{
			if (!RoomSystem.callbackInstance.roomSettings.SoundEffectOtherLimiter.CheckCallServerTime(info.SentServerTime))
			{
				return;
			}
			int playerID = Convert.ToInt32(data[3]);
			arg = NetworkSystem.Instance.GetPlayer(playerID);
		}
		else
		{
			if (!RoomSystem.callbackInstance.roomSettings.SoundEffectLimiter.CheckCallServerTime(info.SentServerTime))
			{
				return;
			}
			arg = NetworkSystem.Instance.LocalPlayer;
		}
		RoomSystem.soundEffectCallback(soundEffect, arg);
	}

	// Token: 0x0600410D RID: 16653 RVA: 0x00149A12 File Offset: 0x00147C12
	internal static void SendSoundEffectAll(int soundIndex, float soundVolume, bool stopCurrentAudio = false)
	{
		RoomSystem.SendSoundEffectAll(new RoomSystem.SoundEffect(soundIndex, soundVolume, stopCurrentAudio));
	}

	// Token: 0x0600410E RID: 16654 RVA: 0x00149A24 File Offset: 0x00147C24
	internal static void SendSoundEffectAll(RoomSystem.SoundEffect sound)
	{
		Action<RoomSystem.SoundEffect, NetPlayer> action = RoomSystem.soundEffectCallback;
		if (action != null)
		{
			action(sound, NetworkSystem.Instance.LocalPlayer);
		}
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		RoomSystem.soundSendData[0] = sound.id;
		RoomSystem.soundSendData[1] = sound.volume;
		RoomSystem.soundSendData[2] = sound.stopCurrentAudio;
		byte b = 3;
		object obj = RoomSystem.soundSendData;
		RoomSystem.SendEvent(b, obj, NetworkSystemRaiseEvent.neoOthers, false);
	}

	// Token: 0x0600410F RID: 16655 RVA: 0x00149AA1 File Offset: 0x00147CA1
	internal static void SendSoundEffectToPlayer(int soundIndex, float soundVolume, NetPlayer player, bool stopCurrentAudio = false)
	{
		RoomSystem.SendSoundEffectToPlayer(new RoomSystem.SoundEffect(soundIndex, soundVolume, stopCurrentAudio), player);
	}

	// Token: 0x06004110 RID: 16656 RVA: 0x00149AB4 File Offset: 0x00147CB4
	internal static void SendSoundEffectToPlayer(RoomSystem.SoundEffect sound, NetPlayer player)
	{
		if (player.IsLocal)
		{
			Action<RoomSystem.SoundEffect, NetPlayer> action = RoomSystem.soundEffectCallback;
			if (action == null)
			{
				return;
			}
			action(sound, player);
			return;
		}
		else
		{
			if (!RoomSystem.joinedRoom)
			{
				return;
			}
			RoomSystem.soundSendData[0] = sound.id;
			RoomSystem.soundSendData[1] = sound.volume;
			RoomSystem.soundSendData[2] = sound.stopCurrentAudio;
			byte b = 3;
			object obj = RoomSystem.soundSendData;
			RoomSystem.SendEvent(b, obj, player, false);
			return;
		}
	}

	// Token: 0x06004111 RID: 16657 RVA: 0x00149B2D File Offset: 0x00147D2D
	internal static void SendSoundEffectOnOther(int soundIndex, float soundvolume, NetPlayer target, bool stopCurrentAudio = false)
	{
		RoomSystem.SendSoundEffectOnOther(new RoomSystem.SoundEffect(soundIndex, soundvolume, stopCurrentAudio), target);
	}

	// Token: 0x06004112 RID: 16658 RVA: 0x00149B40 File Offset: 0x00147D40
	internal static void SendSoundEffectOnOther(RoomSystem.SoundEffect sound, NetPlayer target)
	{
		Action<RoomSystem.SoundEffect, NetPlayer> action = RoomSystem.soundEffectCallback;
		if (action != null)
		{
			action(sound, target);
		}
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		RoomSystem.sendSoundDataOther[0] = sound.id;
		RoomSystem.sendSoundDataOther[1] = sound.volume;
		RoomSystem.sendSoundDataOther[2] = sound.stopCurrentAudio;
		RoomSystem.sendSoundDataOther[3] = target.ActorNumber;
		byte b = 3;
		object obj = RoomSystem.sendSoundDataOther;
		RoomSystem.SendEvent(b, obj, NetworkSystemRaiseEvent.neoOthers, false);
	}

	// Token: 0x06004113 RID: 16659 RVA: 0x00149BC8 File Offset: 0x00147DC8
	internal static void OnPlayerEffect(PlayerEffect effect, NetPlayer target)
	{
		if (target == null)
		{
			return;
		}
		RoomSystem.PlayerEffectConfig playerEffectConfig;
		RigContainer rigContainer;
		if (RoomSystem.playerEffectDictionary.TryGetValue(effect, out playerEffectConfig) && VRRigCache.Instance.TryGetVrrig(target, out rigContainer) && rigContainer != null && rigContainer.Rig != null && playerEffectConfig.tagEffectPack != null)
		{
			TagEffectsLibrary.PlayEffect(rigContainer.Rig.transform, false, rigContainer.Rig.scaleFactor, target.IsLocal ? TagEffectsLibrary.EffectType.FIRST_PERSON : TagEffectsLibrary.EffectType.THIRD_PERSON, playerEffectConfig.tagEffectPack, playerEffectConfig.tagEffectPack, rigContainer.Rig.transform.rotation);
		}
	}

	// Token: 0x06004114 RID: 16660 RVA: 0x00149C60 File Offset: 0x00147E60
	private static void DeserializePlayerEffect(object[] data, PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "DeserializePlayerEffect");
		if (!RoomSystem.callbackInstance.roomSettings.PlayerEffectLimiter.CheckCallServerTime(info.SentServerTime))
		{
			return;
		}
		int playerID = Convert.ToInt32(data[0]);
		PlayerEffect effect = (PlayerEffect)Convert.ToInt32(data[1]);
		NetPlayer player = NetworkSystem.Instance.GetPlayer(playerID);
		RoomSystem.OnPlayerEffect(effect, player);
	}

	// Token: 0x06004115 RID: 16661 RVA: 0x00149CBC File Offset: 0x00147EBC
	internal static void SendPlayerEffect(PlayerEffect effect, NetPlayer target)
	{
		RoomSystem.OnPlayerEffect(effect, target);
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		RoomSystem.playerEffectData[0] = target.ActorNumber;
		RoomSystem.playerEffectData[1] = effect;
		byte b = 6;
		object obj = RoomSystem.playerEffectData;
		RoomSystem.SendEvent(b, obj, NetworkSystemRaiseEvent.neoOthers, false);
	}

	// Token: 0x04004C6D RID: 19565
	private static RoomSystem.ImpactFxContainer impactEffect = new RoomSystem.ImpactFxContainer();

	// Token: 0x04004C6E RID: 19566
	private static RoomSystem.LaunchProjectileContainer launchProjectile = new RoomSystem.LaunchProjectileContainer();

	// Token: 0x04004C6F RID: 19567
	public static GameObject playerImpactEffectPrefab = null;

	// Token: 0x04004C70 RID: 19568
	private static readonly object[] projectileSendData = new object[9];

	// Token: 0x04004C71 RID: 19569
	private static readonly object[] impactSendData = new object[6];

	// Token: 0x04004C72 RID: 19570
	private static readonly List<int> hashValues = new List<int>(2);

	// Token: 0x04004C73 RID: 19571
	[SerializeField]
	private RoomSystemSettings roomSettings;

	// Token: 0x04004C74 RID: 19572
	[SerializeField]
	private string[] prefabsToInstantiateByPath;

	// Token: 0x04004C75 RID: 19573
	[SerializeField]
	private GameObject[] prefabsToInstantiate;

	// Token: 0x04004C76 RID: 19574
	private List<GameObject> prefabsInstantiated = new List<GameObject>();

	// Token: 0x04004C77 RID: 19575
	public static Dictionary<PlayerEffect, RoomSystem.PlayerEffectConfig> playerEffectDictionary = new Dictionary<PlayerEffect, RoomSystem.PlayerEffectConfig>();

	// Token: 0x04004C78 RID: 19576
	[OnEnterPlay_SetNull]
	private static RoomSystem callbackInstance;

	// Token: 0x04004C7B RID: 19579
	private static byte m_roomSizeOnJoin;

	// Token: 0x04004C7C RID: 19580
	[OnEnterPlay_Clear]
	private static List<NetPlayer> netPlayersInRoom = new List<NetPlayer>(10);

	// Token: 0x04004C7D RID: 19581
	[OnEnterPlay_Set("")]
	private static string roomGameMode = "";

	// Token: 0x04004C7E RID: 19582
	[OnEnterPlay_Set(false)]
	private static bool joinedRoom = false;

	// Token: 0x04004C7F RID: 19583
	[OnEnterPlay_SetNull]
	private static PhotonView[] sceneViews;

	// Token: 0x04004C80 RID: 19584
	public static DelegateListProcessor LeftRoomEvent = new DelegateListProcessor();

	// Token: 0x04004C81 RID: 19585
	public static DelegateListProcessor JoinedRoomEvent = new DelegateListProcessor();

	// Token: 0x04004C82 RID: 19586
	public static DelegateListProcessor<NetPlayer> PlayerJoinedEvent = new DelegateListProcessor<NetPlayer>();

	// Token: 0x04004C83 RID: 19587
	public static DelegateListProcessor<NetPlayer> PlayerLeftEvent = new DelegateListProcessor<NetPlayer>();

	// Token: 0x04004C84 RID: 19588
	public static DelegateListProcessor PlayersChangedEvent = new DelegateListProcessor();

	// Token: 0x04004C85 RID: 19589
	private static Timer disconnectTimer = new Timer();

	// Token: 0x04004C86 RID: 19590
	[OnExitPlay_Clear]
	internal static readonly Dictionary<byte, Action<object[], PhotonMessageInfoWrapped>> netEventCallbacks = new Dictionary<byte, Action<object[], PhotonMessageInfoWrapped>>(10);

	// Token: 0x04004C87 RID: 19591
	private static readonly object[] sendEventData = new object[3];

	// Token: 0x04004C88 RID: 19592
	private static readonly object[] groupJoinSendData = new object[2];

	// Token: 0x04004C89 RID: 19593
	private static readonly object[] reportTouchSendData = new object[1];

	// Token: 0x04004C8A RID: 19594
	private static readonly object[] reportHitSendData = new object[3];

	// Token: 0x04004C8B RID: 19595
	[OnExitPlay_SetNull]
	public static Action<NetPlayer, NetPlayer> playerTouchedCallback;

	// Token: 0x04004C8C RID: 19596
	private static CallLimiter playerLaunchedCallLimiter = new CallLimiter(3, 15f, 0.5f);

	// Token: 0x04004C8D RID: 19597
	private static CallLimiter hitPlayerCallLimiter = new CallLimiter(10, 2f, 0.5f);

	// Token: 0x04004C8E RID: 19598
	private static object[] statusSendData = new object[1];

	// Token: 0x04004C8F RID: 19599
	public static Action<RoomSystem.StatusEffects> statusEffectCallback;

	// Token: 0x04004C90 RID: 19600
	private static object[] soundSendData = new object[3];

	// Token: 0x04004C91 RID: 19601
	private static object[] sendSoundDataOther = new object[4];

	// Token: 0x04004C92 RID: 19602
	public static Action<RoomSystem.SoundEffect, NetPlayer> soundEffectCallback;

	// Token: 0x04004C93 RID: 19603
	private static object[] playerEffectData = new object[2];

	// Token: 0x02000A64 RID: 2660
	private class ImpactFxContainer : IFXContext
	{
		// Token: 0x1700062C RID: 1580
		// (get) Token: 0x06004117 RID: 16663 RVA: 0x00149D21 File Offset: 0x00147F21
		public FXSystemSettings settings
		{
			get
			{
				return this.targetRig.fxSettings;
			}
		}

		// Token: 0x06004118 RID: 16664 RVA: 0x00149D30 File Offset: 0x00147F30
		public virtual void OnPlayFX()
		{
			NetPlayer creator = this.targetRig.creator;
			ProjectileTracker.ProjectileInfo projectileInfo;
			if (this.targetRig.isOfflineVRRig)
			{
				projectileInfo = ProjectileTracker.GetLocalProjectile(this.projectileIndex);
			}
			else
			{
				ValueTuple<bool, ProjectileTracker.ProjectileInfo> andRemoveRemotePlayerProjectile = ProjectileTracker.GetAndRemoveRemotePlayerProjectile(creator, this.projectileIndex);
				if (!andRemoveRemotePlayerProjectile.Item1)
				{
					return;
				}
				projectileInfo = andRemoveRemotePlayerProjectile.Item2;
			}
			SlingshotProjectile projectileInstance = projectileInfo.projectileInstance;
			GameObject obj = projectileInfo.hasImpactOverride ? projectileInstance.playerImpactEffectPrefab : RoomSystem.playerImpactEffectPrefab;
			GameObject gameObject = ObjectPools.instance.Instantiate(obj, this.position, true);
			gameObject.transform.localScale = Vector3.one * this.targetRig.scaleFactor;
			GorillaColorizableBase gorillaColorizableBase;
			if (gameObject.TryGetComponent<GorillaColorizableBase>(out gorillaColorizableBase))
			{
				gorillaColorizableBase.SetColor(this.colour);
			}
			SurfaceImpactFX component = gameObject.GetComponent<SurfaceImpactFX>();
			if (component != null)
			{
				component.SetScale(projectileInstance.transform.localScale.x * projectileInstance.impactEffectScaleMultiplier);
			}
			SoundBankPlayer component2 = gameObject.GetComponent<SoundBankPlayer>();
			if (component2 != null && !component2.playOnEnable)
			{
				component2.Play(projectileInstance.impactSoundVolumeOverride, projectileInstance.impactSoundPitchOverride);
			}
			if (projectileInstance.gameObject.activeSelf && projectileInstance.projectileOwner == creator)
			{
				projectileInstance.Deactivate();
			}
		}

		// Token: 0x04004C94 RID: 19604
		public VRRig targetRig;

		// Token: 0x04004C95 RID: 19605
		public Vector3 position;

		// Token: 0x04004C96 RID: 19606
		public Color colour;

		// Token: 0x04004C97 RID: 19607
		public int projectileIndex;
	}

	// Token: 0x02000A65 RID: 2661
	private class LaunchProjectileContainer : RoomSystem.ImpactFxContainer
	{
		// Token: 0x0600411A RID: 16666 RVA: 0x00149E64 File Offset: 0x00148064
		public override void OnPlayFX()
		{
			GameObject gameObject = null;
			SlingshotProjectile slingshotProjectile = null;
			try
			{
				switch (this.projectileSource)
				{
				case RoomSystem.ProjectileSource.ProjectileWeapon:
					if (this.targetRig.projectileWeapon.IsNotNull() && this.targetRig.projectileWeapon.IsNotNull())
					{
						this.velocity = this.targetRig.ClampVelocityRelativeToPlayerSafe(this.velocity, 70f);
						SlingshotProjectile slingshotProjectile2 = this.targetRig.projectileWeapon.LaunchNetworkedProjectile(this.position, this.velocity, this.projectileSource, this.projectileIndex, this.targetRig.scaleFactor, this.overridecolour, this.colour, this.messageInfo);
						if (slingshotProjectile2.IsNotNull())
						{
							ProjectileTracker.AddRemotePlayerProjectile(this.messageInfo.Sender, slingshotProjectile2, this.projectileIndex, this.messageInfo.SentServerTime, this.velocity, this.position, this.targetRig.scaleFactor);
						}
					}
					return;
				case RoomSystem.ProjectileSource.LeftHand:
					this.tempThrowableGO = this.targetRig.myBodyDockPositions.GetLeftHandThrowable();
					break;
				case RoomSystem.ProjectileSource.RightHand:
					this.tempThrowableGO = this.targetRig.myBodyDockPositions.GetRightHandThrowable();
					break;
				default:
					return;
				}
				if (!this.tempThrowableGO.IsNull() && this.tempThrowableGO.TryGetComponent<SnowballThrowable>(out this.tempThrowableRef) && !(this.tempThrowableRef is GrowingSnowballThrowable))
				{
					this.velocity = this.targetRig.ClampVelocityRelativeToPlayerSafe(this.velocity, 50f);
					int projectileHash = this.tempThrowableRef.ProjectileHash;
					gameObject = ObjectPools.instance.Instantiate(projectileHash, true);
					slingshotProjectile = gameObject.GetComponent<SlingshotProjectile>();
					ProjectileTracker.AddRemotePlayerProjectile(this.targetRig.creator, slingshotProjectile, this.projectileIndex, this.messageInfo.SentServerTime, this.velocity, this.position, this.targetRig.scaleFactor);
					slingshotProjectile.Launch(this.position, this.velocity, this.messageInfo.Sender, false, false, this.projectileIndex, this.targetRig.scaleFactor, this.overridecolour, this.colour);
				}
			}
			catch
			{
				GorillaNot.instance.SendReport("throwable error", this.messageInfo.Sender.UserId, this.messageInfo.Sender.NickName);
				if (slingshotProjectile != null && slingshotProjectile)
				{
					slingshotProjectile.transform.position = Vector3.zero;
					slingshotProjectile.Deactivate();
				}
				else if (gameObject.IsNotNull())
				{
					ObjectPools.instance.Destroy(gameObject);
				}
			}
		}

		// Token: 0x04004C98 RID: 19608
		public Vector3 velocity;

		// Token: 0x04004C99 RID: 19609
		public RoomSystem.ProjectileSource projectileSource;

		// Token: 0x04004C9A RID: 19610
		public bool overridecolour;

		// Token: 0x04004C9B RID: 19611
		public PhotonMessageInfoWrapped messageInfo;

		// Token: 0x04004C9C RID: 19612
		private GameObject tempThrowableGO;

		// Token: 0x04004C9D RID: 19613
		private SnowballThrowable tempThrowableRef;
	}

	// Token: 0x02000A66 RID: 2662
	internal enum ProjectileSource
	{
		// Token: 0x04004C9F RID: 19615
		ProjectileWeapon,
		// Token: 0x04004CA0 RID: 19616
		LeftHand,
		// Token: 0x04004CA1 RID: 19617
		RightHand
	}

	// Token: 0x02000A67 RID: 2663
	private struct Events
	{
		// Token: 0x04004CA2 RID: 19618
		public const byte PROJECTILE = 0;

		// Token: 0x04004CA3 RID: 19619
		public const byte IMPACT = 1;

		// Token: 0x04004CA4 RID: 19620
		public const byte STATUS_EFFECT = 2;

		// Token: 0x04004CA5 RID: 19621
		public const byte SOUND_EFFECT = 3;

		// Token: 0x04004CA6 RID: 19622
		public const byte NEARBY_JOIN = 4;

		// Token: 0x04004CA7 RID: 19623
		public const byte PLAYER_TOUCHED = 5;

		// Token: 0x04004CA8 RID: 19624
		public const byte PLAYER_EFFECT = 6;

		// Token: 0x04004CA9 RID: 19625
		public const byte PARTY_JOIN = 7;

		// Token: 0x04004CAA RID: 19626
		public const byte PLAYER_LAUNCHED = 8;

		// Token: 0x04004CAB RID: 19627
		public const byte PLAYER_HIT = 9;

		// Token: 0x04004CAC RID: 19628
		public const byte ELEVATOR_JOIN = 10;
	}

	// Token: 0x02000A68 RID: 2664
	public enum StatusEffects
	{
		// Token: 0x04004CAE RID: 19630
		TaggedTime,
		// Token: 0x04004CAF RID: 19631
		JoinedTaggedTime,
		// Token: 0x04004CB0 RID: 19632
		SetSlowedTime,
		// Token: 0x04004CB1 RID: 19633
		UnTagged,
		// Token: 0x04004CB2 RID: 19634
		FrozenTime
	}

	// Token: 0x02000A69 RID: 2665
	public struct SoundEffect
	{
		// Token: 0x0600411C RID: 16668 RVA: 0x0014A114 File Offset: 0x00148314
		public SoundEffect(int soundID, float soundVolume, bool _stopCurrentAudio)
		{
			this.id = soundID;
			this.volume = soundVolume;
			this.volume = soundVolume;
			this.stopCurrentAudio = _stopCurrentAudio;
		}

		// Token: 0x04004CB3 RID: 19635
		public int id;

		// Token: 0x04004CB4 RID: 19636
		public float volume;

		// Token: 0x04004CB5 RID: 19637
		public bool stopCurrentAudio;
	}

	// Token: 0x02000A6A RID: 2666
	[Serializable]
	public struct PlayerEffectConfig
	{
		// Token: 0x04004CB6 RID: 19638
		public PlayerEffect type;

		// Token: 0x04004CB7 RID: 19639
		public TagEffectPack tagEffectPack;
	}
}
