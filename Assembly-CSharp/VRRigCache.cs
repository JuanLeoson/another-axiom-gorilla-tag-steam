using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTag;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020006B0 RID: 1712
internal class VRRigCache : MonoBehaviour
{
	// Token: 0x170003E1 RID: 993
	// (get) Token: 0x06002A25 RID: 10789 RVA: 0x000E1222 File Offset: 0x000DF422
	// (set) Token: 0x06002A26 RID: 10790 RVA: 0x000E1229 File Offset: 0x000DF429
	public static VRRigCache Instance { get; private set; }

	// Token: 0x170003E2 RID: 994
	// (get) Token: 0x06002A27 RID: 10791 RVA: 0x000E1231 File Offset: 0x000DF431
	public Transform NetworkParent
	{
		get
		{
			return this.networkParent;
		}
	}

	// Token: 0x170003E3 RID: 995
	// (get) Token: 0x06002A28 RID: 10792 RVA: 0x000E1239 File Offset: 0x000DF439
	// (set) Token: 0x06002A29 RID: 10793 RVA: 0x000E1240 File Offset: 0x000DF440
	[OnEnterPlay_Set(false)]
	public static bool isInitialized { get; private set; }

	// Token: 0x14000051 RID: 81
	// (add) Token: 0x06002A2A RID: 10794 RVA: 0x000E1248 File Offset: 0x000DF448
	// (remove) Token: 0x06002A2B RID: 10795 RVA: 0x000E127C File Offset: 0x000DF47C
	public static event Action OnActiveRigsChanged;

	// Token: 0x14000052 RID: 82
	// (add) Token: 0x06002A2C RID: 10796 RVA: 0x000E12B0 File Offset: 0x000DF4B0
	// (remove) Token: 0x06002A2D RID: 10797 RVA: 0x000E12E4 File Offset: 0x000DF4E4
	public static event Action OnPostInitialize;

	// Token: 0x14000053 RID: 83
	// (add) Token: 0x06002A2E RID: 10798 RVA: 0x000E1318 File Offset: 0x000DF518
	// (remove) Token: 0x06002A2F RID: 10799 RVA: 0x000E134C File Offset: 0x000DF54C
	public static event Action OnPostSpawnRig;

	// Token: 0x14000054 RID: 84
	// (add) Token: 0x06002A30 RID: 10800 RVA: 0x000E1380 File Offset: 0x000DF580
	// (remove) Token: 0x06002A31 RID: 10801 RVA: 0x000E13B4 File Offset: 0x000DF5B4
	public static event Action<RigContainer> OnRigActivated;

	// Token: 0x14000055 RID: 85
	// (add) Token: 0x06002A32 RID: 10802 RVA: 0x000E13E8 File Offset: 0x000DF5E8
	// (remove) Token: 0x06002A33 RID: 10803 RVA: 0x000E141C File Offset: 0x000DF61C
	public static event Action<RigContainer> OnRigDeactivated;

	// Token: 0x14000056 RID: 86
	// (add) Token: 0x06002A34 RID: 10804 RVA: 0x000E1450 File Offset: 0x000DF650
	// (remove) Token: 0x06002A35 RID: 10805 RVA: 0x000E1484 File Offset: 0x000DF684
	public static event Action<RigContainer> OnRigNameChanged;

	// Token: 0x06002A36 RID: 10806 RVA: 0x000E14B8 File Offset: 0x000DF6B8
	private void Awake()
	{
		this.InitializeVRRigCache();
		if (this.localRig != null && this.localRig.Rig != null)
		{
			VRRig rig = this.localRig.Rig;
			rig.OnNameChanged = (Action<RigContainer>)Delegate.Combine(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
			if (this.localRig.Rig.bodyRenderer != null)
			{
				this.localRig.Rig.bodyRenderer.SetupAsLocalPlayerBody();
			}
		}
		TickSystemTimer ensureNetworkObjectTimer = this.m_ensureNetworkObjectTimer;
		ensureNetworkObjectTimer.callback = (Action)Delegate.Combine(ensureNetworkObjectTimer.callback, new Action(this.InstantiateNetworkObject));
	}

	// Token: 0x06002A37 RID: 10807 RVA: 0x000E1568 File Offset: 0x000DF768
	private void OnDestroy()
	{
		if (VRRigCache.Instance == this)
		{
			VRRigCache.Instance = null;
		}
		VRRigCache.isInitialized = false;
		if (this.localRig != null && this.localRig.Rig != null)
		{
			VRRig rig = this.localRig.Rig;
			rig.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
		}
	}

	// Token: 0x06002A38 RID: 10808 RVA: 0x000E15D4 File Offset: 0x000DF7D4
	public void InitializeVRRigCache()
	{
		if (VRRigCache.isInitialized || ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (VRRigCache.Instance != null && VRRigCache.Instance != this)
		{
			Object.Destroy(this);
			return;
		}
		VRRigCache.Instance = this;
		if (this.rigParent == null)
		{
			this.rigParent = base.transform;
		}
		if (this.networkParent == null)
		{
			this.networkParent = base.transform;
		}
		for (int i = 0; i < this.rigAmount; i++)
		{
			RigContainer rigContainer = this.SpawnRig();
			VRRigCache.freeRigs.Enqueue(rigContainer);
			rigContainer.Rig.BuildInitialize();
			rigContainer.Rig.transform.parent = null;
		}
		VRRigCache.isInitialized = true;
		Action onPostInitialize = VRRigCache.OnPostInitialize;
		if (onPostInitialize == null)
		{
			return;
		}
		onPostInitialize();
	}

	// Token: 0x06002A39 RID: 10809 RVA: 0x000E16A0 File Offset: 0x000DF8A0
	private RigContainer SpawnRig()
	{
		if (this.rigTemplate.activeSelf)
		{
			this.rigTemplate.SetActive(false);
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this.rigTemplate, this.rigParent, false);
		Action onPostSpawnRig = VRRigCache.OnPostSpawnRig;
		if (onPostSpawnRig != null)
		{
			onPostSpawnRig();
		}
		if (gameObject == null)
		{
			return null;
		}
		return gameObject.GetComponent<RigContainer>();
	}

	// Token: 0x06002A3A RID: 10810 RVA: 0x000E16F3 File Offset: 0x000DF8F3
	internal bool TryGetVrrig(Player targetPlayer, out RigContainer playerRig)
	{
		return this.TryGetVrrig(NetworkSystem.Instance.GetPlayer(targetPlayer.ActorNumber), out playerRig);
	}

	// Token: 0x06002A3B RID: 10811 RVA: 0x000E170C File Offset: 0x000DF90C
	internal bool TryGetVrrig(int targetPlayerId, out RigContainer playerRig)
	{
		return this.TryGetVrrig(NetworkSystem.Instance.GetPlayer(targetPlayerId), out playerRig);
	}

	// Token: 0x06002A3C RID: 10812 RVA: 0x000E1720 File Offset: 0x000DF920
	internal bool TryGetVrrig(NetPlayer targetPlayer, out RigContainer playerRig)
	{
		playerRig = null;
		if (ApplicationQuittingState.IsQuitting)
		{
			return false;
		}
		if (targetPlayer == null || targetPlayer.IsNull)
		{
			GTDev.LogError<string>("[GT/VRRigCache]  ERROR!!!  TryGetVrrig: Supplied targetPlayer cannot be null!", null);
			return false;
		}
		if (targetPlayer.IsLocal)
		{
			playerRig = this.localRig;
			return true;
		}
		if (!targetPlayer.InRoom)
		{
			return false;
		}
		if (!VRRigCache.rigsInUse.TryGetValue(targetPlayer, out playerRig))
		{
			if (VRRigCache.freeRigs.Count <= 0)
			{
				return false;
			}
			playerRig = VRRigCache.freeRigs.Dequeue();
			playerRig.Creator = targetPlayer;
			VRRigCache.rigsInUse.Add(targetPlayer, playerRig);
			VRRig rig = playerRig.Rig;
			rig.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
			VRRig rig2 = playerRig.Rig;
			rig2.OnNameChanged = (Action<RigContainer>)Delegate.Combine(rig2.OnNameChanged, VRRigCache.OnRigNameChanged);
			playerRig.gameObject.SetActive(true);
			Action<RigContainer> onRigActivated = VRRigCache.OnRigActivated;
			if (onRigActivated != null)
			{
				onRigActivated(playerRig);
			}
			Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
			if (onActiveRigsChanged != null)
			{
				onActiveRigsChanged();
			}
		}
		return true;
	}

	// Token: 0x06002A3D RID: 10813 RVA: 0x000E1828 File Offset: 0x000DFA28
	private void AddRigToGorillaParent(NetPlayer player, VRRig vrrig)
	{
		GorillaParent instance = GorillaParent.instance;
		if (instance == null)
		{
			return;
		}
		if (!instance.vrrigs.Contains(vrrig))
		{
			instance.vrrigs.Add(vrrig);
		}
		if (!instance.vrrigDict.ContainsKey(player))
		{
			instance.vrrigDict.Add(player, vrrig);
			return;
		}
		instance.vrrigDict[player] = vrrig;
	}

	// Token: 0x06002A3E RID: 10814 RVA: 0x000E188C File Offset: 0x000DFA8C
	public void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		if (newPlayer.ActorNumber == -1)
		{
			Debug.LogError("LocalPlayer returned, vrrig no correctly initialised");
		}
		RigContainer rigContainer;
		if (this.TryGetVrrig(newPlayer, out rigContainer))
		{
			this.AddRigToGorillaParent(newPlayer, rigContainer.Rig);
			Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
			if (onActiveRigsChanged == null)
			{
				return;
			}
			onActiveRigsChanged();
		}
	}

	// Token: 0x06002A3F RID: 10815 RVA: 0x000E18D4 File Offset: 0x000DFAD4
	public void OnJoinedRoom()
	{
		foreach (NetPlayer netPlayer in NetworkSystem.Instance.AllNetPlayers)
		{
			RigContainer rigContainer;
			if (this.TryGetVrrig(netPlayer, out rigContainer))
			{
				this.AddRigToGorillaParent(netPlayer, rigContainer.Rig);
			}
		}
		this.m_ensureNetworkObjectTimer.Start();
		Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
		if (onActiveRigsChanged == null)
		{
			return;
		}
		onActiveRigsChanged();
	}

	// Token: 0x06002A40 RID: 10816 RVA: 0x000E1930 File Offset: 0x000DFB30
	private void RemoveRigFromGorillaParent(NetPlayer player, VRRig vrrig)
	{
		GorillaParent instance = GorillaParent.instance;
		if (instance == null)
		{
			return;
		}
		if (instance.vrrigs.Contains(vrrig))
		{
			instance.vrrigs.Remove(vrrig);
		}
		if (instance.vrrigDict.ContainsKey(player))
		{
			instance.vrrigDict.Remove(player);
		}
	}

	// Token: 0x06002A41 RID: 10817 RVA: 0x000E1988 File Offset: 0x000DFB88
	public void OnPlayerLeftRoom(NetPlayer leavingPlayer)
	{
		if (leavingPlayer.IsNull)
		{
			Debug.LogError("Leaving players NetPlayer is Null");
			this.CheckForMissingPlayer();
		}
		RigContainer rigContainer;
		if (!VRRigCache.rigsInUse.TryGetValue(leavingPlayer, out rigContainer))
		{
			this.LogError("failed to find player's vrrig who left " + leavingPlayer.UserId);
			return;
		}
		rigContainer.gameObject.Disable();
		VRRig rig = rigContainer.Rig;
		rig.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
		VRRigCache.freeRigs.Enqueue(rigContainer);
		VRRigCache.rigsInUse.Remove(leavingPlayer);
		this.RemoveRigFromGorillaParent(leavingPlayer, rigContainer.Rig);
		Action<RigContainer> onRigDeactivated = VRRigCache.OnRigDeactivated;
		if (onRigDeactivated != null)
		{
			onRigDeactivated(rigContainer);
		}
		Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
		if (onActiveRigsChanged == null)
		{
			return;
		}
		onActiveRigsChanged();
	}

	// Token: 0x06002A42 RID: 10818 RVA: 0x000E1A44 File Offset: 0x000DFC44
	private void CheckForMissingPlayer()
	{
		foreach (KeyValuePair<NetPlayer, RigContainer> keyValuePair in VRRigCache.rigsInUse)
		{
			if (keyValuePair.Key == null || keyValuePair.Value == null)
			{
				Debug.LogError("Somehow null reference in rigsInUse");
			}
			else if (!keyValuePair.Key.InRoom)
			{
				keyValuePair.Value.gameObject.Disable();
				VRRig rig = keyValuePair.Value.Rig;
				rig.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
				VRRigCache.freeRigs.Enqueue(keyValuePair.Value);
				VRRigCache.rigsInUse.Remove(keyValuePair.Key);
				this.RemoveRigFromGorillaParent(keyValuePair.Key, keyValuePair.Value.Rig);
				Action<RigContainer> onRigDeactivated = VRRigCache.OnRigDeactivated;
				if (onRigDeactivated != null)
				{
					onRigDeactivated(keyValuePair.Value);
				}
			}
		}
	}

	// Token: 0x06002A43 RID: 10819 RVA: 0x000E1B58 File Offset: 0x000DFD58
	public void OnLeftRoom()
	{
		this.m_ensureNetworkObjectTimer.Stop();
		foreach (NetPlayer netPlayer in VRRigCache.rigsInUse.Keys.ToArray<NetPlayer>())
		{
			RigContainer rigContainer = VRRigCache.rigsInUse[netPlayer];
			if (!(rigContainer == null))
			{
				VRRig rig = VRRigCache.rigsInUse[netPlayer].Rig;
				VRRig rig2 = rigContainer.Rig;
				rig2.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig2.OnNameChanged, VRRigCache.OnRigNameChanged);
				rigContainer.gameObject.Disable();
				VRRigCache.rigsInUse.Remove(netPlayer);
				this.RemoveRigFromGorillaParent(netPlayer, rig);
				VRRigCache.freeRigs.Enqueue(rigContainer);
				Action<RigContainer> onRigDeactivated = VRRigCache.OnRigDeactivated;
				if (onRigDeactivated != null)
				{
					onRigDeactivated(rigContainer);
				}
			}
		}
		Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
		if (onActiveRigsChanged == null)
		{
			return;
		}
		onActiveRigsChanged();
	}

	// Token: 0x06002A44 RID: 10820 RVA: 0x000E1C2C File Offset: 0x000DFE2C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal VRRig[] GetAllRigs()
	{
		VRRig[] array = new VRRig[VRRigCache.rigsInUse.Count + VRRigCache.freeRigs.Count];
		int num = 0;
		foreach (RigContainer rigContainer in VRRigCache.rigsInUse.Values)
		{
			array[num] = rigContainer.Rig;
			num++;
		}
		foreach (RigContainer rigContainer2 in VRRigCache.freeRigs)
		{
			array[num] = rigContainer2.Rig;
			num++;
		}
		return array;
	}

	// Token: 0x06002A45 RID: 10821 RVA: 0x000E1CF4 File Offset: 0x000DFEF4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void GetAllUsedRigs(List<VRRig> rigs)
	{
		if (rigs == null)
		{
			return;
		}
		foreach (RigContainer rigContainer in VRRigCache.rigsInUse.Values)
		{
			rigs.Add(rigContainer.Rig);
		}
	}

	// Token: 0x06002A46 RID: 10822 RVA: 0x000E1D54 File Offset: 0x000DFF54
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void GetActiveRigs(List<VRRig> rigsListToUpdate)
	{
		if (rigsListToUpdate == null)
		{
			return;
		}
		rigsListToUpdate.Clear();
		if (!VRRigCache.isInitialized)
		{
			return;
		}
		rigsListToUpdate.Add(VRRigCache.Instance.localRig.Rig);
		foreach (RigContainer rigContainer in VRRigCache.rigsInUse.Values)
		{
			rigsListToUpdate.Add(rigContainer.Rig);
		}
	}

	// Token: 0x06002A47 RID: 10823 RVA: 0x000E1DD8 File Offset: 0x000DFFD8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal int GetAllRigsHash()
	{
		int num = 0;
		foreach (RigContainer rigContainer in VRRigCache.rigsInUse.Values)
		{
			num += rigContainer.GetInstanceID();
		}
		foreach (RigContainer rigContainer2 in VRRigCache.freeRigs)
		{
			num += rigContainer2.GetInstanceID();
		}
		return num;
	}

	// Token: 0x06002A48 RID: 10824 RVA: 0x000E1E7C File Offset: 0x000E007C
	internal void InstantiateNetworkObject()
	{
		if (this.localRig.netView.IsNotNull() || !NetworkSystem.Instance.InRoom)
		{
			return;
		}
		PrefabType prefabType;
		if (!VRRigCache.Instance.GetComponent<PhotonPrefabPool>().networkPrefabs.TryGetValue("Player Network Controller", out prefabType) || prefabType.prefab == null)
		{
			Debug.LogError("OnJoinedRoom: Unable to find player prefab to spawn");
			return;
		}
		GameObject gameObject = GTPlayer.Instance.gameObject;
		Color playerColor = this.localRig.Rig.playerColor;
		VRRigCache.rigRGBData[0] = playerColor.r;
		VRRigCache.rigRGBData[1] = playerColor.g;
		VRRigCache.rigRGBData[2] = playerColor.b;
		NetworkSystem.Instance.NetInstantiate(prefabType.prefab, gameObject.transform.position, gameObject.transform.rotation, false, 0, VRRigCache.rigRGBData, null);
	}

	// Token: 0x06002A49 RID: 10825 RVA: 0x000023F5 File Offset: 0x000005F5
	private void LogInfo(string log)
	{
	}

	// Token: 0x06002A4A RID: 10826 RVA: 0x000023F5 File Offset: 0x000005F5
	private void LogWarning(string log)
	{
	}

	// Token: 0x06002A4B RID: 10827 RVA: 0x000023F5 File Offset: 0x000005F5
	private void LogError(string log)
	{
	}

	// Token: 0x040035D8 RID: 13784
	private const string preLog = "[GT/VRRigCache] ";

	// Token: 0x040035D9 RID: 13785
	private const string preErr = "[GT/VRRigCache]  ERROR!!!  ";

	// Token: 0x040035DA RID: 13786
	private const string preErrBeta = "[GT/VRRigCache]  ERROR!!!  (beta only log) ";

	// Token: 0x040035DB RID: 13787
	private const string preErrEd = "[GT/VRRigCache]  ERROR!!!  (editor only log) ";

	// Token: 0x040035DD RID: 13789
	public RigContainer localRig;

	// Token: 0x040035DE RID: 13790
	[SerializeField]
	private Transform rigParent;

	// Token: 0x040035DF RID: 13791
	[SerializeField]
	private Transform networkParent;

	// Token: 0x040035E0 RID: 13792
	[SerializeField]
	private GameObject rigTemplate;

	// Token: 0x040035E1 RID: 13793
	[SerializeField]
	private int rigAmount = 9;

	// Token: 0x040035E2 RID: 13794
	[SerializeField]
	private TickSystemTimer m_ensureNetworkObjectTimer = new TickSystemTimer(0.1f);

	// Token: 0x040035E3 RID: 13795
	[OnEnterPlay_Clear]
	private static Queue<RigContainer> freeRigs = new Queue<RigContainer>(10);

	// Token: 0x040035E4 RID: 13796
	[OnEnterPlay_Clear]
	private static Dictionary<NetPlayer, RigContainer> rigsInUse = new Dictionary<NetPlayer, RigContainer>(10);

	// Token: 0x040035EC RID: 13804
	private static object[] rigRGBData = new object[]
	{
		0f,
		0f,
		0f
	};
}
