using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Fusion;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020006C5 RID: 1733
public abstract class GorillaGameManager : MonoBehaviourPunCallbacks, ITickSystemTick, IWrappedSerializable, INetworkStruct
{
	// Token: 0x14000057 RID: 87
	// (add) Token: 0x06002AD9 RID: 10969 RVA: 0x000E3DC8 File Offset: 0x000E1FC8
	// (remove) Token: 0x06002ADA RID: 10970 RVA: 0x000E3DFC File Offset: 0x000E1FFC
	public static event GorillaGameManager.OnTouchDelegate OnTouch;

	// Token: 0x170003F4 RID: 1012
	// (get) Token: 0x06002ADB RID: 10971 RVA: 0x000E3E2F File Offset: 0x000E202F
	public static GorillaGameManager instance
	{
		get
		{
			return GorillaGameModes.GameMode.ActiveGameMode;
		}
	}

	// Token: 0x170003F5 RID: 1013
	// (get) Token: 0x06002ADC RID: 10972 RVA: 0x000E3E36 File Offset: 0x000E2036
	// (set) Token: 0x06002ADD RID: 10973 RVA: 0x000E3E3E File Offset: 0x000E203E
	bool ITickSystemTick.TickRunning { get; set; }

	// Token: 0x06002ADE RID: 10974 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void Awake()
	{
	}

	// Token: 0x06002ADF RID: 10975 RVA: 0x000023F5 File Offset: 0x000005F5
	private new void OnEnable()
	{
	}

	// Token: 0x06002AE0 RID: 10976 RVA: 0x000023F5 File Offset: 0x000005F5
	private new void OnDisable()
	{
	}

	// Token: 0x06002AE1 RID: 10977 RVA: 0x000E3E48 File Offset: 0x000E2048
	public virtual void Tick()
	{
		if (this.lastCheck + this.checkCooldown < Time.time)
		{
			this.lastCheck = Time.time;
			if (NetworkSystem.Instance.IsMasterClient && !this.ValidGameMode())
			{
				GorillaGameModes.GameMode.ChangeGameFromProperty();
				return;
			}
			this.InfrequentUpdate();
		}
	}

	// Token: 0x06002AE2 RID: 10978 RVA: 0x000E3E95 File Offset: 0x000E2095
	public virtual void InfrequentUpdate()
	{
		GorillaGameModes.GameMode.RefreshPlayers();
		this.currentNetPlayerArray = NetworkSystem.Instance.AllNetPlayers;
	}

	// Token: 0x06002AE3 RID: 10979 RVA: 0x000E3EAC File Offset: 0x000E20AC
	public virtual string GameModeName()
	{
		return "NONE";
	}

	// Token: 0x06002AE4 RID: 10980 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void LocalTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer, bool bodyHit, bool leftHand)
	{
	}

	// Token: 0x06002AE5 RID: 10981 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void ReportTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
	}

	// Token: 0x06002AE6 RID: 10982 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void HitPlayer(NetPlayer player)
	{
	}

	// Token: 0x06002AE7 RID: 10983 RVA: 0x00002076 File Offset: 0x00000276
	public virtual bool CanAffectPlayer(NetPlayer player, bool thisFrame)
	{
		return false;
	}

	// Token: 0x06002AE8 RID: 10984 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void HandleHandTap(NetPlayer tappingPlayer, Tappable hitTappable, bool leftHand, Vector3 handVelocity, Vector3 tapSurfaceNormal)
	{
	}

	// Token: 0x06002AE9 RID: 10985 RVA: 0x0001D558 File Offset: 0x0001B758
	public virtual bool CanJoinFrienship(NetPlayer player)
	{
		return true;
	}

	// Token: 0x06002AEA RID: 10986 RVA: 0x0001D558 File Offset: 0x0001B758
	public virtual bool CanPlayerParticipate(NetPlayer player)
	{
		return true;
	}

	// Token: 0x06002AEB RID: 10987 RVA: 0x000E3EB3 File Offset: 0x000E20B3
	public virtual void HandleRoundComplete()
	{
		PlayerGameEvents.GameModeCompleteRound();
	}

	// Token: 0x06002AEC RID: 10988 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void HandleTagBroadcast(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
	}

	// Token: 0x06002AED RID: 10989 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void HandleTagBroadcast(NetPlayer taggedPlayer, NetPlayer taggingPlayer, double tagTime)
	{
	}

	// Token: 0x06002AEE RID: 10990 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void NewVRRig(NetPlayer player, int vrrigPhotonViewID, bool didTutorial)
	{
	}

	// Token: 0x06002AEF RID: 10991 RVA: 0x00002076 File Offset: 0x00000276
	public virtual bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		return false;
	}

	// Token: 0x06002AF0 RID: 10992 RVA: 0x000E3EBA File Offset: 0x000E20BA
	public virtual VRRig FindPlayerVRRig(NetPlayer player)
	{
		this.returnRig = null;
		this.outContainer = null;
		if (player == null)
		{
			return null;
		}
		if (VRRigCache.Instance.TryGetVrrig(player, out this.outContainer))
		{
			this.returnRig = this.outContainer.Rig;
		}
		return this.returnRig;
	}

	// Token: 0x06002AF1 RID: 10993 RVA: 0x000E3EFC File Offset: 0x000E20FC
	public static VRRig StaticFindRigForPlayer(NetPlayer player)
	{
		VRRig result = null;
		RigContainer rigContainer;
		if (GorillaGameManager.instance != null)
		{
			result = GorillaGameManager.instance.FindPlayerVRRig(player);
		}
		else if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			result = rigContainer.Rig;
		}
		return result;
	}

	// Token: 0x06002AF2 RID: 10994 RVA: 0x000E3F3D File Offset: 0x000E213D
	public virtual float[] LocalPlayerSpeed()
	{
		this.playerSpeed[0] = this.slowJumpLimit;
		this.playerSpeed[1] = this.slowJumpMultiplier;
		return this.playerSpeed;
	}

	// Token: 0x06002AF3 RID: 10995 RVA: 0x000E3F64 File Offset: 0x000E2164
	public virtual void UpdatePlayerAppearance(VRRig rig)
	{
		ScienceExperimentManager instance = ScienceExperimentManager.instance;
		int materialIndex;
		if (instance != null && instance.GetMaterialIfPlayerInGame(rig.creator.ActorNumber, out materialIndex))
		{
			rig.ChangeMaterialLocal(materialIndex);
			return;
		}
		int materialIndex2 = this.MyMatIndex(rig.creator);
		rig.ChangeMaterialLocal(materialIndex2);
	}

	// Token: 0x06002AF4 RID: 10996 RVA: 0x00002076 File Offset: 0x00000276
	public virtual int MyMatIndex(NetPlayer forPlayer)
	{
		return 0;
	}

	// Token: 0x06002AF5 RID: 10997 RVA: 0x000E3FB3 File Offset: 0x000E21B3
	public virtual int SpecialHandFX(NetPlayer player, RigContainer rigContainer)
	{
		return -1;
	}

	// Token: 0x06002AF6 RID: 10998 RVA: 0x000E3FB6 File Offset: 0x000E21B6
	public virtual bool ValidGameMode()
	{
		return NetworkSystem.Instance.InRoom && NetworkSystem.Instance.GameModeString.Contains(this.GameTypeName());
	}

	// Token: 0x06002AF7 RID: 10999 RVA: 0x000E3FDE File Offset: 0x000E21DE
	public static void OnInstanceReady(Action action)
	{
		GorillaParent.OnReplicatedClientReady(delegate
		{
			if (GorillaGameManager.instance)
			{
				action();
				return;
			}
			GorillaGameManager.onInstanceReady = (Action)Delegate.Combine(GorillaGameManager.onInstanceReady, action);
		});
	}

	// Token: 0x06002AF8 RID: 11000 RVA: 0x000E3FFC File Offset: 0x000E21FC
	public static void ReplicatedClientReady()
	{
		GorillaGameManager.replicatedClientReady = true;
	}

	// Token: 0x06002AF9 RID: 11001 RVA: 0x000E4004 File Offset: 0x000E2204
	public static void OnReplicatedClientReady(Action action)
	{
		if (GorillaGameManager.replicatedClientReady)
		{
			action();
			return;
		}
		GorillaGameManager.onReplicatedClientReady = (Action)Delegate.Combine(GorillaGameManager.onReplicatedClientReady, action);
	}

	// Token: 0x170003F6 RID: 1014
	// (get) Token: 0x06002AFA RID: 11002 RVA: 0x000E4029 File Offset: 0x000E2229
	internal GameModeSerializer Serializer
	{
		get
		{
			return this.serializer;
		}
	}

	// Token: 0x06002AFB RID: 11003 RVA: 0x000E4031 File Offset: 0x000E2231
	internal virtual void NetworkLinkSetup(GameModeSerializer netSerializer)
	{
		this.serializer = netSerializer;
	}

	// Token: 0x06002AFC RID: 11004 RVA: 0x000E403A File Offset: 0x000E223A
	internal virtual void NetworkLinkDestroyed(GameModeSerializer netSerializer)
	{
		if (this.serializer == netSerializer)
		{
			this.serializer = null;
		}
	}

	// Token: 0x06002AFD RID: 11005
	public abstract GameModeType GameType();

	// Token: 0x06002AFE RID: 11006 RVA: 0x000E4054 File Offset: 0x000E2254
	public string GameTypeName()
	{
		return this.GameType().ToString();
	}

	// Token: 0x06002AFF RID: 11007
	public abstract void AddFusionDataBehaviour(NetworkObject behaviour);

	// Token: 0x06002B00 RID: 11008
	public abstract void OnSerializeRead(object newData);

	// Token: 0x06002B01 RID: 11009
	public abstract object OnSerializeWrite();

	// Token: 0x06002B02 RID: 11010
	public abstract void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x06002B03 RID: 11011
	public abstract void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x06002B04 RID: 11012 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void Reset()
	{
	}

	// Token: 0x06002B05 RID: 11013 RVA: 0x000E4078 File Offset: 0x000E2278
	public virtual void StartPlaying()
	{
		TickSystem<object>.AddTickCallback(this);
		NetworkSystem.Instance.OnPlayerJoined += this.OnPlayerEnteredRoom;
		NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerLeftRoom;
		NetworkSystem.Instance.OnMasterClientSwitchedEvent += this.OnMasterClientSwitched;
		this.currentNetPlayerArray = NetworkSystem.Instance.AllNetPlayers;
	}

	// Token: 0x06002B06 RID: 11014 RVA: 0x000E4104 File Offset: 0x000E2304
	public virtual void StopPlaying()
	{
		TickSystem<object>.RemoveTickCallback(this);
		NetworkSystem.Instance.OnPlayerJoined -= this.OnPlayerEnteredRoom;
		NetworkSystem.Instance.OnPlayerLeft -= this.OnPlayerLeftRoom;
		NetworkSystem.Instance.OnMasterClientSwitchedEvent -= this.OnMasterClientSwitched;
		this.lastCheck = 0f;
	}

	// Token: 0x06002B07 RID: 11015 RVA: 0x000023F5 File Offset: 0x000005F5
	public new virtual void OnMasterClientSwitched(Player newMaster)
	{
	}

	// Token: 0x06002B08 RID: 11016 RVA: 0x000023F5 File Offset: 0x000005F5
	public new virtual void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x06002B09 RID: 11017 RVA: 0x000023F5 File Offset: 0x000005F5
	public new virtual void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x06002B0A RID: 11018 RVA: 0x000E4188 File Offset: 0x000E2388
	public virtual void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		this.currentNetPlayerArray = NetworkSystem.Instance.AllNetPlayers;
		if (this.lastTaggedActorNr.ContainsKey(otherPlayer.ActorNumber))
		{
			this.lastTaggedActorNr.Remove(otherPlayer.ActorNumber);
		}
	}

	// Token: 0x06002B0B RID: 11019 RVA: 0x000E41BF File Offset: 0x000E23BF
	public virtual void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		this.currentNetPlayerArray = NetworkSystem.Instance.AllNetPlayers;
	}

	// Token: 0x06002B0C RID: 11020 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void OnMasterClientSwitched(NetPlayer newMaster)
	{
	}

	// Token: 0x06002B0D RID: 11021 RVA: 0x000E41D4 File Offset: 0x000E23D4
	internal static void ForceStopGame_DisconnectAndDestroy()
	{
		Application.Quit();
		NetworkSystem instance = NetworkSystem.Instance;
		if (instance != null)
		{
			instance.ReturnToSinglePlayer();
		}
		Object.DestroyImmediate(PhotonNetworkController.Instance);
		Object.DestroyImmediate(GTPlayer.Instance);
		GameObject[] array = Object.FindObjectsOfType<GameObject>();
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy(array[i]);
		}
	}

	// Token: 0x06002B0E RID: 11022 RVA: 0x000E422C File Offset: 0x000E242C
	public void AddLastTagged(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		if (this.lastTaggedActorNr.ContainsKey(taggedPlayer.ActorNumber))
		{
			this.lastTaggedActorNr[taggedPlayer.ActorNumber] = taggingPlayer.ActorNumber;
			return;
		}
		this.lastTaggedActorNr.Add(taggedPlayer.ActorNumber, taggingPlayer.ActorNumber);
	}

	// Token: 0x06002B0F RID: 11023 RVA: 0x000E427C File Offset: 0x000E247C
	public void WriteLastTagged(PhotonStream stream)
	{
		stream.SendNext(this.lastTaggedActorNr.Count);
		foreach (KeyValuePair<int, int> keyValuePair in this.lastTaggedActorNr)
		{
			stream.SendNext(keyValuePair.Key);
			stream.SendNext(keyValuePair.Value);
		}
	}

	// Token: 0x06002B10 RID: 11024 RVA: 0x000E4304 File Offset: 0x000E2504
	public void ReadLastTagged(PhotonStream stream)
	{
		this.lastTaggedActorNr.Clear();
		int num = Mathf.Min((int)stream.ReceiveNext(), 10);
		for (int i = 0; i < num; i++)
		{
			this.lastTaggedActorNr.Add((int)stream.ReceiveNext(), (int)stream.ReceiveNext());
		}
	}

	// Token: 0x04003669 RID: 13929
	public object obj;

	// Token: 0x0400366A RID: 13930
	public float fastJumpLimit;

	// Token: 0x0400366B RID: 13931
	public float fastJumpMultiplier;

	// Token: 0x0400366C RID: 13932
	public float slowJumpLimit;

	// Token: 0x0400366D RID: 13933
	public float slowJumpMultiplier;

	// Token: 0x0400366E RID: 13934
	public float lastCheck;

	// Token: 0x0400366F RID: 13935
	public float checkCooldown = 3f;

	// Token: 0x04003670 RID: 13936
	public float startingToLookForFriend;

	// Token: 0x04003671 RID: 13937
	public float timeToSpendLookingForFriend = 10f;

	// Token: 0x04003672 RID: 13938
	public bool successfullyFoundFriend;

	// Token: 0x04003673 RID: 13939
	public float tagDistanceThreshold = 4f;

	// Token: 0x04003674 RID: 13940
	public bool testAssault;

	// Token: 0x04003675 RID: 13941
	public bool endGameManually;

	// Token: 0x04003676 RID: 13942
	public NetPlayer currentMasterClient;

	// Token: 0x04003677 RID: 13943
	public VRRig returnRig;

	// Token: 0x04003678 RID: 13944
	private NetPlayer outPlayer;

	// Token: 0x04003679 RID: 13945
	private int outInt;

	// Token: 0x0400367A RID: 13946
	private VRRig tempRig;

	// Token: 0x0400367B RID: 13947
	public NetPlayer[] currentNetPlayerArray;

	// Token: 0x0400367C RID: 13948
	public float[] playerSpeed = new float[2];

	// Token: 0x0400367D RID: 13949
	private RigContainer outContainer;

	// Token: 0x0400367E RID: 13950
	public Dictionary<int, int> lastTaggedActorNr = new Dictionary<int, int>();

	// Token: 0x04003680 RID: 13952
	private static Action onInstanceReady;

	// Token: 0x04003681 RID: 13953
	private static bool replicatedClientReady;

	// Token: 0x04003682 RID: 13954
	private static Action onReplicatedClientReady;

	// Token: 0x04003683 RID: 13955
	private GameModeSerializer serializer;

	// Token: 0x020006C6 RID: 1734
	// (Invoke) Token: 0x06002B13 RID: 11027
	public delegate void OnTouchDelegate(NetPlayer taggedPlayer, NetPlayer taggingPlayer);
}
