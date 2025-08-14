using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using UnityEngine;

namespace GorillaGameModes
{
	// Token: 0x02000BDF RID: 3039
	public class GameMode : MonoBehaviour
	{
		// Token: 0x060049AA RID: 18858 RVA: 0x00166234 File Offset: 0x00164434
		private void Awake()
		{
			if (GameMode.instance.IsNull())
			{
				GameMode.instance = this;
				foreach (GorillaGameManager gorillaGameManager in base.gameObject.GetComponentsInChildren<GorillaGameManager>(true))
				{
					int num = (int)gorillaGameManager.GameType();
					string text = gorillaGameManager.GameTypeName();
					if (GameMode.gameModeTable.ContainsKey(num))
					{
						Debug.LogWarning("Duplicate gamemode type, skipping this instance", gorillaGameManager);
					}
					else
					{
						GameMode.gameModeTable.Add((int)gorillaGameManager.GameType(), gorillaGameManager);
						GameMode.gameModeKeyByName.Add(text, num);
						GameMode.gameModes.Add(gorillaGameManager);
						GameMode.gameModeNames.Add(text);
					}
				}
				return;
			}
			Object.Destroy(this);
		}

		// Token: 0x060049AB RID: 18859 RVA: 0x001662D9 File Offset: 0x001644D9
		private void OnDestroy()
		{
			if (GameMode.instance == this)
			{
				GameMode.instance = null;
			}
		}

		// Token: 0x14000082 RID: 130
		// (add) Token: 0x060049AC RID: 18860 RVA: 0x001662F0 File Offset: 0x001644F0
		// (remove) Token: 0x060049AD RID: 18861 RVA: 0x00166324 File Offset: 0x00164524
		public static event GameMode.OnStartGameModeAction OnStartGameMode;

		// Token: 0x17000719 RID: 1817
		// (get) Token: 0x060049AE RID: 18862 RVA: 0x00166357 File Offset: 0x00164557
		public static GorillaGameManager ActiveGameMode
		{
			get
			{
				return GameMode.activeGameMode;
			}
		}

		// Token: 0x1700071A RID: 1818
		// (get) Token: 0x060049AF RID: 18863 RVA: 0x0016635E File Offset: 0x0016455E
		internal static GameModeSerializer ActiveNetworkHandler
		{
			get
			{
				return GameMode.activeNetworkHandler;
			}
		}

		// Token: 0x1700071B RID: 1819
		// (get) Token: 0x060049B0 RID: 18864 RVA: 0x00166365 File Offset: 0x00164565
		public static GameModeZoneMapping GameModeZoneMapping
		{
			get
			{
				return GameMode.instance.gameModeZoneMapping;
			}
		}

		// Token: 0x14000083 RID: 131
		// (add) Token: 0x060049B1 RID: 18865 RVA: 0x00166374 File Offset: 0x00164574
		// (remove) Token: 0x060049B2 RID: 18866 RVA: 0x001663A8 File Offset: 0x001645A8
		public static event Action<List<NetPlayer>, List<NetPlayer>> ParticipatingPlayersChanged;

		// Token: 0x060049B3 RID: 18867 RVA: 0x001663DC File Offset: 0x001645DC
		static GameMode()
		{
			GameMode.StaticLoad();
		}

		// Token: 0x060049B4 RID: 18868 RVA: 0x00166478 File Offset: 0x00164678
		[OnEnterPlay_Run]
		private static void StaticLoad()
		{
			RoomSystem.LeftRoomEvent += new Action(GameMode.ResetGameModes);
			RoomSystem.JoinedRoomEvent += new Action(GameMode.RefreshPlayers);
			RoomSystem.PlayersChangedEvent += new Action(GameMode.RefreshPlayers);
		}

		// Token: 0x060049B5 RID: 18869 RVA: 0x001664D6 File Offset: 0x001646D6
		internal static bool LoadGameModeFromProperty()
		{
			return GameMode.LoadGameMode(GameMode.FindGameModeFromRoomProperty());
		}

		// Token: 0x060049B6 RID: 18870 RVA: 0x001664E2 File Offset: 0x001646E2
		internal static bool ChangeGameFromProperty()
		{
			return GameMode.ChangeGameMode(GameMode.FindGameModeFromRoomProperty());
		}

		// Token: 0x060049B7 RID: 18871 RVA: 0x001664EE File Offset: 0x001646EE
		internal static bool LoadGameModeFromProperty(string prop)
		{
			return GameMode.LoadGameMode(GameMode.FindGameModeInString(prop));
		}

		// Token: 0x060049B8 RID: 18872 RVA: 0x001664FB File Offset: 0x001646FB
		internal static bool ChangeGameFromProperty(string prop)
		{
			return GameMode.ChangeGameMode(GameMode.FindGameModeInString(prop));
		}

		// Token: 0x060049B9 RID: 18873 RVA: 0x00166508 File Offset: 0x00164708
		public static int GetGameModeKeyFromRoomProp()
		{
			string text = GameMode.FindGameModeFromRoomProperty();
			int result;
			if (string.IsNullOrEmpty(text) || !GameMode.gameModeKeyByName.TryGetValue(text, out result))
			{
				GTDev.LogWarning<string>("Unable to find game mode key for " + text, null);
				return -1;
			}
			return result;
		}

		// Token: 0x060049BA RID: 18874 RVA: 0x00166546 File Offset: 0x00164746
		private static string FindGameModeFromRoomProperty()
		{
			if (!NetworkSystem.Instance.InRoom || string.IsNullOrEmpty(NetworkSystem.Instance.GameModeString))
			{
				return null;
			}
			return GameMode.FindGameModeInString(NetworkSystem.Instance.GameModeString);
		}

		// Token: 0x060049BB RID: 18875 RVA: 0x00166576 File Offset: 0x00164776
		public static bool IsValidGameMode(string gameMode)
		{
			return !string.IsNullOrEmpty(gameMode) && GameMode.gameModeKeyByName.ContainsKey(gameMode);
		}

		// Token: 0x060049BC RID: 18876 RVA: 0x00166590 File Offset: 0x00164790
		private static string FindGameModeInString(string gmString)
		{
			for (int i = 0; i < GameMode.gameModes.Count; i++)
			{
				string text = GameMode.gameModes[i].GameTypeName();
				if (gmString.EndsWith(text))
				{
					return text;
				}
			}
			return null;
		}

		// Token: 0x060049BD RID: 18877 RVA: 0x001665D0 File Offset: 0x001647D0
		public static bool LoadGameMode(string gameMode)
		{
			if (gameMode == null)
			{
				Debug.LogError("GAME MODE NULL");
				return false;
			}
			int key;
			if (!GameMode.gameModeKeyByName.TryGetValue(gameMode, out key))
			{
				Debug.LogWarning("Unable to find game mode key for " + gameMode);
				return false;
			}
			return GameMode.LoadGameMode(key);
		}

		// Token: 0x060049BE RID: 18878 RVA: 0x00166614 File Offset: 0x00164814
		public static bool LoadGameMode(int key)
		{
			foreach (KeyValuePair<int, GorillaGameManager> keyValuePair in GameMode.gameModeTable)
			{
			}
			if (!GameMode.gameModeTable.ContainsKey(key))
			{
				Debug.LogWarning("Missing game mode for key " + key.ToString());
				return false;
			}
			PrefabType prefabType;
			VRRigCache.Instance.GetComponent<PhotonPrefabPool>().networkPrefabs.TryGetValue("GameMode", out prefabType);
			GameObject prefab = prefabType.prefab;
			if (prefab == null)
			{
				GTDev.LogError<string>("Unable to find game mode prefab to spawn", null);
				return false;
			}
			if (NetworkSystem.Instance.NetInstantiate(prefab, Vector3.zero, Quaternion.identity, true, 0, new object[]
			{
				key
			}, delegate(NetworkRunner runner, NetworkObject no)
			{
				no.GetComponent<GameModeSerializer>().Init(key);
			}).IsNull())
			{
				GTDev.LogWarning<string>("Unable to create GameManager with key " + key.ToString(), null);
				return false;
			}
			return true;
		}

		// Token: 0x060049BF RID: 18879 RVA: 0x00166730 File Offset: 0x00164930
		internal static bool ChangeGameMode(string gameMode)
		{
			if (gameMode == null)
			{
				return false;
			}
			int key;
			if (!GameMode.gameModeKeyByName.TryGetValue(gameMode, out key))
			{
				Debug.LogWarning("Unable to find game mode key for " + gameMode);
				return false;
			}
			return GameMode.ChangeGameMode(key);
		}

		// Token: 0x060049C0 RID: 18880 RVA: 0x0016676C File Offset: 0x0016496C
		internal static bool ChangeGameMode(int key)
		{
			GorillaGameManager x;
			if (!NetworkSystem.Instance.IsMasterClient || !GameMode.gameModeTable.TryGetValue(key, out x) || x == GameMode.activeGameMode)
			{
				return false;
			}
			if (GameMode.activeNetworkHandler.IsNotNull())
			{
				NetworkSystem.Instance.NetDestroy(GameMode.activeNetworkHandler.gameObject);
			}
			GameMode.StopGameModeSafe(GameMode.activeGameMode);
			GameMode.activeGameMode = null;
			GameMode.activeNetworkHandler = null;
			return GameMode.LoadGameMode(key);
		}

		// Token: 0x060049C1 RID: 18881 RVA: 0x001667E0 File Offset: 0x001649E0
		internal static void SetupGameModeRemote(GameModeSerializer networkSerializer)
		{
			GorillaGameManager gameModeInstance = networkSerializer.GameModeInstance;
			bool flag = gameModeInstance != GameMode.activeGameMode;
			if (GameMode.activeGameMode.IsNotNull() && gameModeInstance.IsNotNull() && flag)
			{
				GameMode.StopGameModeSafe(GameMode.activeGameMode);
			}
			GameMode.activeNetworkHandler = networkSerializer;
			GameMode.activeGameMode = gameModeInstance;
			GameMode.activeGameMode.NetworkLinkSetup(networkSerializer);
			if (!GameMode.activatedGameModes.Contains(GameMode.activeGameMode))
			{
				GameMode.activatedGameModes.Add(GameMode.activeGameMode);
			}
			if (flag)
			{
				GameMode.StartGameModeSafe(GameMode.activeGameMode);
				if (GameMode.OnStartGameMode != null)
				{
					GameMode.OnStartGameMode(GameMode.activeGameMode.GameType());
				}
			}
		}

		// Token: 0x060049C2 RID: 18882 RVA: 0x00166882 File Offset: 0x00164A82
		internal static void RemoveNetworkLink(GameModeSerializer networkSerializer)
		{
			if (GameMode.activeGameMode.IsNotNull() && networkSerializer == GameMode.activeNetworkHandler)
			{
				GameMode.activeGameMode.NetworkLinkDestroyed(networkSerializer);
				GameMode.activeNetworkHandler = null;
				return;
			}
		}

		// Token: 0x060049C3 RID: 18883 RVA: 0x001668AF File Offset: 0x00164AAF
		public static GorillaGameManager GetGameModeInstance(GameModeType type)
		{
			return GameMode.GetGameModeInstance((int)type);
		}

		// Token: 0x060049C4 RID: 18884 RVA: 0x001668B8 File Offset: 0x00164AB8
		public static GorillaGameManager GetGameModeInstance(int type)
		{
			GorillaGameManager gorillaGameManager;
			if (GameMode.gameModeTable.TryGetValue(type, out gorillaGameManager))
			{
				if (gorillaGameManager == null)
				{
					Debug.LogError("Couldnt get mode from table");
					foreach (KeyValuePair<int, GorillaGameManager> keyValuePair in GameMode.gameModeTable)
					{
					}
				}
				return gorillaGameManager;
			}
			return null;
		}

		// Token: 0x060049C5 RID: 18885 RVA: 0x00166928 File Offset: 0x00164B28
		public static T GetGameModeInstance<T>(GameModeType type) where T : GorillaGameManager
		{
			return GameMode.GetGameModeInstance<T>((int)type);
		}

		// Token: 0x060049C6 RID: 18886 RVA: 0x00166930 File Offset: 0x00164B30
		public static T GetGameModeInstance<T>(int type) where T : GorillaGameManager
		{
			T t = GameMode.GetGameModeInstance(type) as T;
			if (t != null)
			{
				return t;
			}
			return default(T);
		}

		// Token: 0x060049C7 RID: 18887 RVA: 0x00166964 File Offset: 0x00164B64
		public static void ResetGameModes()
		{
			GameMode.activeGameMode = null;
			GameMode.activeNetworkHandler = null;
			GameMode.optOutPlayers.Clear();
			GameMode.ParticipatingPlayers.Clear();
			for (int i = 0; i < GameMode.activatedGameModes.Count; i++)
			{
				GorillaGameManager gameMode = GameMode.activatedGameModes[i];
				GameMode.StopGameModeSafe(gameMode);
				GameMode.ResetGameModeSafe(gameMode);
			}
			GameMode.activatedGameModes.Clear();
		}

		// Token: 0x060049C8 RID: 18888 RVA: 0x001669C8 File Offset: 0x00164BC8
		private static void StartGameModeSafe(GorillaGameManager gameMode)
		{
			try
			{
				gameMode.StartPlaying();
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x060049C9 RID: 18889 RVA: 0x001669F0 File Offset: 0x00164BF0
		private static void StopGameModeSafe(GorillaGameManager gameMode)
		{
			try
			{
				gameMode.StopPlaying();
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x060049CA RID: 18890 RVA: 0x00166A18 File Offset: 0x00164C18
		private static void ResetGameModeSafe(GorillaGameManager gameMode)
		{
			try
			{
				gameMode.Reset();
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x060049CB RID: 18891 RVA: 0x00166A40 File Offset: 0x00164C40
		public static void ReportTag(NetPlayer player)
		{
			if (NetworkSystem.Instance.InRoom && GameMode.activeNetworkHandler.IsNotNull())
			{
				GameMode.activeNetworkHandler.SendRPC("RPC_ReportTag", false, new object[]
				{
					player.ActorNumber
				});
			}
		}

		// Token: 0x060049CC RID: 18892 RVA: 0x00166A80 File Offset: 0x00164C80
		public static void ReportHit()
		{
			if (GorillaGameManager.instance.GameType() == GameModeType.Custom)
			{
				CustomGameMode.TaggedByEnvironment();
			}
			if (NetworkSystem.Instance.InRoom && GameMode.activeNetworkHandler.IsNotNull())
			{
				GameMode.activeNetworkHandler.SendRPC("RPC_ReportHit", false, Array.Empty<object>());
			}
		}

		// Token: 0x060049CD RID: 18893 RVA: 0x00166ACC File Offset: 0x00164CCC
		public static void BroadcastRoundComplete()
		{
			if (NetworkSystem.Instance.IsMasterClient && NetworkSystem.Instance.InRoom && GameMode.activeNetworkHandler.IsNotNull())
			{
				GameMode.activeNetworkHandler.SendRPC("RPC_BroadcastRoundComplete", true, Array.Empty<object>());
			}
		}

		// Token: 0x060049CE RID: 18894 RVA: 0x00166B08 File Offset: 0x00164D08
		public static void BroadcastTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
		{
			if (NetworkSystem.Instance.IsMasterClient && NetworkSystem.Instance.InRoom && GameMode.activeNetworkHandler.IsNotNull())
			{
				GameMode.activeNetworkHandler.SendRPC("RPC_BroadcastTag", true, new object[]
				{
					taggedPlayer.ActorNumber,
					taggingPlayer.ActorNumber
				});
			}
		}

		// Token: 0x1700071C RID: 1820
		// (get) Token: 0x060049CF RID: 18895 RVA: 0x00166B6B File Offset: 0x00164D6B
		public static List<NetPlayer> ParticipatingPlayers
		{
			get
			{
				return GameMode._participatingPlayers;
			}
		}

		// Token: 0x060049D0 RID: 18896 RVA: 0x00166B74 File Offset: 0x00164D74
		public static void RefreshPlayers()
		{
			GameMode._oldPlayersCount = GameMode._participatingPlayers.Count;
			for (int i = 0; i < GameMode._oldPlayersCount; i++)
			{
				GameMode._oldPlayersBuffer[i] = GameMode._participatingPlayers[i];
			}
			GameMode._participatingPlayers.Clear();
			List<NetPlayer> playersInRoom = RoomSystem.PlayersInRoom;
			int num = Mathf.Min(playersInRoom.Count, 10);
			for (int j = 0; j < num; j++)
			{
				if (GameMode.CanParticipate(playersInRoom[j]))
				{
					GameMode.ParticipatingPlayers.Add(playersInRoom[j]);
				}
			}
			GameMode._tempRemovedPlayers.Clear();
			for (int k = 0; k < GameMode._oldPlayersCount; k++)
			{
				NetPlayer netPlayer = GameMode._oldPlayersBuffer[k];
				if (!GameMode.ContainsNetPlayer(GameMode._participatingPlayers, netPlayer))
				{
					GameMode._tempRemovedPlayers.Add(netPlayer);
				}
			}
			GameMode._tempAddedPlayers.Clear();
			int count = GameMode._participatingPlayers.Count;
			for (int l = 0; l < count; l++)
			{
				NetPlayer netPlayer2 = GameMode._participatingPlayers[l];
				if (!GameMode.ContainsNetPlayer(GameMode._oldPlayersBuffer, netPlayer2, GameMode._oldPlayersCount))
				{
					GameMode._tempAddedPlayers.Add(netPlayer2);
				}
			}
			if ((GameMode._tempAddedPlayers.Count > 0 || GameMode._tempRemovedPlayers.Count > 0) && GameMode.ParticipatingPlayersChanged != null)
			{
				GameMode.ParticipatingPlayersChanged(GameMode._tempAddedPlayers, GameMode._tempRemovedPlayers);
			}
		}

		// Token: 0x060049D1 RID: 18897 RVA: 0x00166CCC File Offset: 0x00164ECC
		private static bool ContainsNetPlayer(List<NetPlayer> list, NetPlayer candidate)
		{
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				if (list[i] == candidate)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060049D2 RID: 18898 RVA: 0x00166CFC File Offset: 0x00164EFC
		private static bool ContainsNetPlayer(NetPlayer[] array, NetPlayer candidate, int length)
		{
			for (int i = 0; i < length; i++)
			{
				if (array[i] == candidate)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060049D3 RID: 18899 RVA: 0x00166D1E File Offset: 0x00164F1E
		public static void OptOut(VRRig rig)
		{
			GameMode.OptOut(rig.creator.ActorNumber);
		}

		// Token: 0x060049D4 RID: 18900 RVA: 0x00166D30 File Offset: 0x00164F30
		public static void OptOut(NetPlayer player)
		{
			GameMode.OptOut(player.ActorNumber);
		}

		// Token: 0x060049D5 RID: 18901 RVA: 0x00166D3D File Offset: 0x00164F3D
		public static void OptOut(int playerActorNumber)
		{
			if (GameMode.optOutPlayers.Add(playerActorNumber))
			{
				GameMode.RefreshPlayers();
			}
		}

		// Token: 0x060049D6 RID: 18902 RVA: 0x00166D51 File Offset: 0x00164F51
		public static void OptIn(VRRig rig)
		{
			GameMode.OptIn(rig.creator.ActorNumber);
		}

		// Token: 0x060049D7 RID: 18903 RVA: 0x00166D63 File Offset: 0x00164F63
		public static void OptIn(NetPlayer player)
		{
			GameMode.OptIn(player.ActorNumber);
		}

		// Token: 0x060049D8 RID: 18904 RVA: 0x00166D70 File Offset: 0x00164F70
		public static void OptIn(int playerActorNumber)
		{
			if (GameMode.optOutPlayers.Remove(playerActorNumber))
			{
				GameMode.RefreshPlayers();
			}
		}

		// Token: 0x060049D9 RID: 18905 RVA: 0x00166D84 File Offset: 0x00164F84
		private static bool CanParticipate(NetPlayer player)
		{
			return player.InRoom() && !GameMode.optOutPlayers.Contains(player.ActorNumber) && NetworkSystem.Instance.GetPlayerTutorialCompletion(player.ActorNumber) && (!(GorillaGameManager.instance != null) || GorillaGameManager.instance.CanPlayerParticipate(player));
		}

		// Token: 0x0400528E RID: 21134
		[SerializeField]
		private GameModeZoneMapping gameModeZoneMapping;

		// Token: 0x04005290 RID: 21136
		[OnEnterPlay_SetNull]
		private static GameMode instance;

		// Token: 0x04005291 RID: 21137
		[OnEnterPlay_Clear]
		private static Dictionary<int, GorillaGameManager> gameModeTable = new Dictionary<int, GorillaGameManager>();

		// Token: 0x04005292 RID: 21138
		[OnEnterPlay_Clear]
		private static Dictionary<string, int> gameModeKeyByName = new Dictionary<string, int>();

		// Token: 0x04005293 RID: 21139
		[OnEnterPlay_Clear]
		private static Dictionary<int, FusionGameModeData> fusionTypeTable = new Dictionary<int, FusionGameModeData>();

		// Token: 0x04005294 RID: 21140
		[OnEnterPlay_Clear]
		private static List<GorillaGameManager> gameModes = new List<GorillaGameManager>(10);

		// Token: 0x04005295 RID: 21141
		[OnEnterPlay_Clear]
		public static readonly List<string> gameModeNames = new List<string>(10);

		// Token: 0x04005296 RID: 21142
		[OnEnterPlay_Clear]
		private static readonly List<GorillaGameManager> activatedGameModes = new List<GorillaGameManager>(11);

		// Token: 0x04005297 RID: 21143
		[OnEnterPlay_SetNull]
		private static GorillaGameManager activeGameMode = null;

		// Token: 0x04005298 RID: 21144
		[OnEnterPlay_SetNull]
		private static GameModeSerializer activeNetworkHandler = null;

		// Token: 0x0400529A RID: 21146
		[OnEnterPlay_Clear]
		private static readonly HashSet<int> optOutPlayers = new HashSet<int>(10);

		// Token: 0x0400529B RID: 21147
		[OnEnterPlay_Clear]
		private static readonly List<NetPlayer> _participatingPlayers = new List<NetPlayer>(10);

		// Token: 0x0400529C RID: 21148
		private static readonly NetPlayer[] _oldPlayersBuffer = new NetPlayer[10];

		// Token: 0x0400529D RID: 21149
		private static int _oldPlayersCount;

		// Token: 0x0400529E RID: 21150
		private static readonly List<NetPlayer> _tempAddedPlayers = new List<NetPlayer>(10);

		// Token: 0x0400529F RID: 21151
		private static readonly List<NetPlayer> _tempRemovedPlayers = new List<NetPlayer>(10);

		// Token: 0x02000BE0 RID: 3040
		// (Invoke) Token: 0x060049DC RID: 18908
		public delegate void OnStartGameModeAction(GameModeType newGameModeType);
	}
}
