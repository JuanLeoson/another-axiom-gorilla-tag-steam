using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaNetworking;
using GorillaTag.Audio;
using Newtonsoft.Json;
using Photon.Voice.PUN;
using PlayFab;
using PlayFab.CloudScriptModels;
using UnityEngine;

// Token: 0x020006AA RID: 1706
[RequireComponent(typeof(VRRig), typeof(VRRigReliableState))]
public class RigContainer : MonoBehaviour
{
	// Token: 0x170003CE RID: 974
	// (get) Token: 0x060029E1 RID: 10721 RVA: 0x000E0705 File Offset: 0x000DE905
	// (set) Token: 0x060029E2 RID: 10722 RVA: 0x000E070D File Offset: 0x000DE90D
	public bool Initialized { get; private set; }

	// Token: 0x170003CF RID: 975
	// (get) Token: 0x060029E3 RID: 10723 RVA: 0x000E0716 File Offset: 0x000DE916
	public VRRig Rig
	{
		get
		{
			return this.vrrig;
		}
	}

	// Token: 0x170003D0 RID: 976
	// (get) Token: 0x060029E4 RID: 10724 RVA: 0x000E071E File Offset: 0x000DE91E
	public VRRigReliableState ReliableState
	{
		get
		{
			return this.reliableState;
		}
	}

	// Token: 0x170003D1 RID: 977
	// (get) Token: 0x060029E5 RID: 10725 RVA: 0x000E0726 File Offset: 0x000DE926
	public Transform SpeakerHead
	{
		get
		{
			return this.speakerHead;
		}
	}

	// Token: 0x170003D2 RID: 978
	// (get) Token: 0x060029E6 RID: 10726 RVA: 0x000E072E File Offset: 0x000DE92E
	public AudioSource ReplacementVoiceSource
	{
		get
		{
			return this.replacementVoiceSource;
		}
	}

	// Token: 0x170003D3 RID: 979
	// (get) Token: 0x060029E7 RID: 10727 RVA: 0x000E0736 File Offset: 0x000DE936
	public List<LoudSpeakerNetwork> LoudSpeakerNetworks
	{
		get
		{
			return this.loudSpeakerNetworks;
		}
	}

	// Token: 0x170003D4 RID: 980
	// (get) Token: 0x060029E8 RID: 10728 RVA: 0x000E073E File Offset: 0x000DE93E
	// (set) Token: 0x060029E9 RID: 10729 RVA: 0x000E0746 File Offset: 0x000DE946
	public PhotonVoiceView Voice
	{
		get
		{
			return this.voiceView;
		}
		set
		{
			if (value == this.voiceView)
			{
				return;
			}
			if (this.voiceView != null)
			{
				this.voiceView.SpeakerInUse.enabled = false;
			}
			this.voiceView = value;
			this.RefreshVoiceChat();
		}
	}

	// Token: 0x170003D5 RID: 981
	// (get) Token: 0x060029EA RID: 10730 RVA: 0x000E0783 File Offset: 0x000DE983
	public NetworkView netView
	{
		get
		{
			return this.vrrig.netView;
		}
	}

	// Token: 0x170003D6 RID: 982
	// (get) Token: 0x060029EB RID: 10731 RVA: 0x000E0790 File Offset: 0x000DE990
	public int CachedNetViewID
	{
		get
		{
			return this.m_cachedNetViewID;
		}
	}

	// Token: 0x170003D7 RID: 983
	// (get) Token: 0x060029EC RID: 10732 RVA: 0x000E0798 File Offset: 0x000DE998
	// (set) Token: 0x060029ED RID: 10733 RVA: 0x000E07A3 File Offset: 0x000DE9A3
	public bool Muted
	{
		get
		{
			return !this.enableVoice;
		}
		set
		{
			this.enableVoice = !value;
			this.RefreshVoiceChat();
		}
	}

	// Token: 0x170003D8 RID: 984
	// (get) Token: 0x060029EE RID: 10734 RVA: 0x000E07B5 File Offset: 0x000DE9B5
	// (set) Token: 0x060029EF RID: 10735 RVA: 0x000E07C4 File Offset: 0x000DE9C4
	public NetPlayer Creator
	{
		get
		{
			return this.vrrig.creator;
		}
		set
		{
			if (this.vrrig.isOfflineVRRig || (this.vrrig.creator != null && this.vrrig.creator.InRoom))
			{
				return;
			}
			this.vrrig.creator = value;
			this.vrrig.UpdateName();
		}
	}

	// Token: 0x170003D9 RID: 985
	// (get) Token: 0x060029F0 RID: 10736 RVA: 0x000E0815 File Offset: 0x000DEA15
	// (set) Token: 0x060029F1 RID: 10737 RVA: 0x000E081D File Offset: 0x000DEA1D
	public bool ForceMute
	{
		get
		{
			return this.forceMute;
		}
		set
		{
			this.forceMute = value;
			this.RefreshVoiceChat();
		}
	}

	// Token: 0x170003DA RID: 986
	// (get) Token: 0x060029F2 RID: 10738 RVA: 0x000E082C File Offset: 0x000DEA2C
	public SphereCollider HeadCollider
	{
		get
		{
			return this.headCollider;
		}
	}

	// Token: 0x170003DB RID: 987
	// (get) Token: 0x060029F3 RID: 10739 RVA: 0x000E0834 File Offset: 0x000DEA34
	public CapsuleCollider BodyCollider
	{
		get
		{
			return this.bodyCollider;
		}
	}

	// Token: 0x170003DC RID: 988
	// (get) Token: 0x060029F4 RID: 10740 RVA: 0x000E083C File Offset: 0x000DEA3C
	public VRRigEvents RigEvents
	{
		get
		{
			return this.rigEvents;
		}
	}

	// Token: 0x060029F5 RID: 10741 RVA: 0x000E0844 File Offset: 0x000DEA44
	public bool GetIsPlayerAutoMuted()
	{
		return this.bPlayerAutoMuted;
	}

	// Token: 0x060029F6 RID: 10742 RVA: 0x000E084C File Offset: 0x000DEA4C
	public void UpdateAutomuteLevel(string autoMuteLevel)
	{
		if (autoMuteLevel.Equals("LOW", StringComparison.OrdinalIgnoreCase))
		{
			this.playerChatQuality = 1;
		}
		else if (autoMuteLevel.Equals("HIGH", StringComparison.OrdinalIgnoreCase))
		{
			this.playerChatQuality = 0;
		}
		else if (autoMuteLevel.Equals("ERROR", StringComparison.OrdinalIgnoreCase))
		{
			this.playerChatQuality = 2;
		}
		else
		{
			this.playerChatQuality = 2;
		}
		this.RefreshVoiceChat();
	}

	// Token: 0x060029F7 RID: 10743 RVA: 0x000E08AB File Offset: 0x000DEAAB
	private void Awake()
	{
		this.loudSpeakerNetworks = new List<LoudSpeakerNetwork>();
	}

	// Token: 0x060029F8 RID: 10744 RVA: 0x000E08B8 File Offset: 0x000DEAB8
	private void Start()
	{
		if (this.Rig.isOfflineVRRig)
		{
			this.vrrig.creator = NetworkSystem.Instance.LocalPlayer;
			RoomSystem.JoinedRoomEvent += new Action(this.OnMultiPlayerStarted);
			RoomSystem.LeftRoomEvent += new Action(this.OnReturnedToSinglePlayer);
		}
		this.Rig.rigContainer = this;
	}

	// Token: 0x060029F9 RID: 10745 RVA: 0x000E0929 File Offset: 0x000DEB29
	private void OnMultiPlayerStarted()
	{
		if (this.Rig.isOfflineVRRig)
		{
			this.vrrig.creator = NetworkSystem.Instance.GetLocalPlayer();
		}
	}

	// Token: 0x060029FA RID: 10746 RVA: 0x000E094D File Offset: 0x000DEB4D
	private void OnReturnedToSinglePlayer()
	{
		if (this.Rig.isOfflineVRRig)
		{
			RigContainer.CancelAutomuteRequest();
		}
	}

	// Token: 0x060029FB RID: 10747 RVA: 0x000E0964 File Offset: 0x000DEB64
	private void OnDisable()
	{
		this.Initialized = false;
		this.enableVoice = true;
		this.voiceView = null;
		base.gameObject.transform.localPosition = Vector3.zero;
		base.gameObject.transform.localRotation = Quaternion.identity;
		this.vrrig.syncPos = base.gameObject.transform.position;
		this.vrrig.syncRotation = base.gameObject.transform.rotation;
		this.forceMute = false;
	}

	// Token: 0x060029FC RID: 10748 RVA: 0x000E09ED File Offset: 0x000DEBED
	internal void InitializeNetwork(NetworkView netView, PhotonVoiceView voiceView, VRRigSerializer vrRigSerializer)
	{
		if (!netView || !voiceView)
		{
			return;
		}
		this.InitializeNetwork_Shared(netView, vrRigSerializer);
		this.Voice = voiceView;
		this.vrrig.voiceAudio = voiceView.SpeakerInUse.GetComponent<AudioSource>();
	}

	// Token: 0x060029FD RID: 10749 RVA: 0x000E0A28 File Offset: 0x000DEC28
	private void InitializeNetwork_Shared(NetworkView netView, VRRigSerializer vrRigSerializer)
	{
		if (this.vrrig.netView)
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent creating multiple vrrigs", this.Creator.UserId, this.Creator.NickName);
			if (this.vrrig.netView.IsMine)
			{
				NetworkSystem.Instance.NetDestroy(this.vrrig.gameObject);
			}
			else
			{
				this.vrrig.netView.gameObject.SetActive(false);
			}
		}
		this.vrrig.netView = netView;
		this.vrrig.rigSerializer = vrRigSerializer;
		this.vrrig.OwningNetPlayer = NetworkSystem.Instance.GetPlayer(NetworkSystem.Instance.GetOwningPlayerID(vrRigSerializer.gameObject));
		this.m_cachedNetViewID = netView.ViewID;
		if (!this.Initialized)
		{
			this.vrrig.NetInitialize();
			if (GorillaGameManager.instance != null && NetworkSystem.Instance.IsMasterClient)
			{
				int owningPlayerID = NetworkSystem.Instance.GetOwningPlayerID(vrRigSerializer.gameObject);
				bool playerTutorialCompletion = NetworkSystem.Instance.GetPlayerTutorialCompletion(owningPlayerID);
				GorillaGameManager.instance.NewVRRig(netView.Owner, netView.ViewID, playerTutorialCompletion);
			}
			bool isLocal = this.vrrig.OwningNetPlayer.IsLocal;
			if (this.vrrig.InitializedCosmetics)
			{
				netView.SendRPC("RPC_RequestCosmetics", netView.Owner, Array.Empty<object>());
			}
		}
		this.Initialized = true;
		if (!this.vrrig.isOfflineVRRig)
		{
			base.StartCoroutine(RigContainer.QueueAutomute(this.Creator));
		}
	}

	// Token: 0x060029FE RID: 10750 RVA: 0x000E0BB3 File Offset: 0x000DEDB3
	private static IEnumerator QueueAutomute(NetPlayer player)
	{
		RigContainer.playersToCheckAutomute.Add(player);
		if (!RigContainer.automuteQueued)
		{
			RigContainer.automuteQueued = true;
			yield return new WaitForSecondsRealtime(1f);
			while (RigContainer.waitingForAutomuteCallback)
			{
				yield return null;
			}
			RigContainer.automuteQueued = false;
			RigContainer.RequestAutomuteSettings();
		}
		yield break;
	}

	// Token: 0x060029FF RID: 10751 RVA: 0x000E0BC4 File Offset: 0x000DEDC4
	private static void RequestAutomuteSettings()
	{
		if (RigContainer.playersToCheckAutomute.Count == 0)
		{
			return;
		}
		RigContainer.waitingForAutomuteCallback = true;
		RigContainer.playersToCheckAutomute.RemoveAll((NetPlayer player) => player == null);
		RigContainer.requestedAutomutePlayers = new List<NetPlayer>(RigContainer.playersToCheckAutomute);
		RigContainer.playersToCheckAutomute.Clear();
		string[] value = (from x in RigContainer.requestedAutomutePlayers
		select x.UserId).ToArray<string>();
		foreach (NetPlayer netPlayer in RigContainer.requestedAutomutePlayers)
		{
		}
		ExecuteFunctionRequest executeFunctionRequest = new ExecuteFunctionRequest();
		executeFunctionRequest.Entity = new EntityKey
		{
			Id = PlayFabSettings.staticPlayer.EntityId,
			Type = PlayFabSettings.staticPlayer.EntityType
		};
		executeFunctionRequest.FunctionName = "ShouldUserAutomutePlayer";
		executeFunctionRequest.FunctionParameter = string.Join(",", value);
		PlayFabCloudScriptAPI.ExecuteFunction(executeFunctionRequest, delegate(ExecuteFunctionResult result)
		{
			Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.FunctionResult.ToString());
			if (dictionary == null)
			{
				using (List<NetPlayer>.Enumerator enumerator2 = RigContainer.requestedAutomutePlayers.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						NetPlayer netPlayer2 = enumerator2.Current;
						if (netPlayer2 != null)
						{
							RigContainer.ReceiveAutomuteSettings(netPlayer2, "none");
						}
					}
					goto IL_A6;
				}
			}
			foreach (NetPlayer netPlayer3 in RigContainer.requestedAutomutePlayers)
			{
				if (netPlayer3 != null)
				{
					string score;
					if (dictionary.TryGetValue(netPlayer3.UserId, out score))
					{
						RigContainer.ReceiveAutomuteSettings(netPlayer3, score);
					}
					else
					{
						RigContainer.ReceiveAutomuteSettings(netPlayer3, "none");
					}
				}
			}
			IL_A6:
			RigContainer.requestedAutomutePlayers.Clear();
			RigContainer.waitingForAutomuteCallback = false;
		}, delegate(PlayFabError error)
		{
			foreach (NetPlayer player in RigContainer.requestedAutomutePlayers)
			{
				RigContainer.ReceiveAutomuteSettings(player, "ERROR");
			}
			RigContainer.requestedAutomutePlayers.Clear();
			RigContainer.waitingForAutomuteCallback = false;
		}, null, null);
	}

	// Token: 0x06002A00 RID: 10752 RVA: 0x000E0D28 File Offset: 0x000DEF28
	private static void CancelAutomuteRequest()
	{
		RigContainer.playersToCheckAutomute.Clear();
		RigContainer.automuteQueued = false;
		if (RigContainer.requestedAutomutePlayers != null)
		{
			RigContainer.requestedAutomutePlayers.Clear();
		}
		RigContainer.waitingForAutomuteCallback = false;
	}

	// Token: 0x06002A01 RID: 10753 RVA: 0x000E0D54 File Offset: 0x000DEF54
	private static void ReceiveAutomuteSettings(NetPlayer player, string score)
	{
		RigContainer rigContainer;
		VRRigCache.Instance.TryGetVrrig(player, out rigContainer);
		if (rigContainer != null)
		{
			rigContainer.UpdateAutomuteLevel(score);
		}
	}

	// Token: 0x06002A02 RID: 10754 RVA: 0x000E0D80 File Offset: 0x000DEF80
	private void ProcessAutomute()
	{
		int @int = PlayerPrefs.GetInt("autoMute", 1);
		this.bPlayerAutoMuted = (!this.hasManualMute && this.playerChatQuality < @int);
	}

	// Token: 0x06002A03 RID: 10755 RVA: 0x000E0DB4 File Offset: 0x000DEFB4
	public void RefreshVoiceChat()
	{
		if (this.Voice == null)
		{
			return;
		}
		this.ProcessAutomute();
		this.Voice.SpeakerInUse.enabled = (!this.forceMute && this.enableVoice && !this.bPlayerAutoMuted && GorillaComputer.instance.voiceChatOn == "TRUE");
		this.replacementVoiceSource.mute = (this.forceMute || !this.enableVoice || this.bPlayerAutoMuted || GorillaComputer.instance.voiceChatOn == "OFF");
	}

	// Token: 0x06002A04 RID: 10756 RVA: 0x000E0E53 File Offset: 0x000DF053
	public void AddLoudSpeakerNetwork(LoudSpeakerNetwork network)
	{
		if (this.loudSpeakerNetworks.Contains(network))
		{
			return;
		}
		this.loudSpeakerNetworks.Add(network);
	}

	// Token: 0x06002A05 RID: 10757 RVA: 0x000E0E70 File Offset: 0x000DF070
	public void RemoveLoudSpeakerNetwork(LoudSpeakerNetwork network)
	{
		this.loudSpeakerNetworks.Remove(network);
	}

	// Token: 0x06002A06 RID: 10758 RVA: 0x000E0E80 File Offset: 0x000DF080
	public static void RefreshAllRigVoices()
	{
		RigContainer.staticTempRC = null;
		if (!NetworkSystem.Instance.InRoom || VRRigCache.Instance == null)
		{
			return;
		}
		foreach (NetPlayer targetPlayer in NetworkSystem.Instance.AllNetPlayers)
		{
			if (VRRigCache.Instance.TryGetVrrig(targetPlayer, out RigContainer.staticTempRC))
			{
				RigContainer.staticTempRC.RefreshVoiceChat();
			}
		}
	}

	// Token: 0x040035B8 RID: 13752
	[SerializeField]
	private VRRig vrrig;

	// Token: 0x040035B9 RID: 13753
	[SerializeField]
	private VRRigReliableState reliableState;

	// Token: 0x040035BA RID: 13754
	[SerializeField]
	private Transform speakerHead;

	// Token: 0x040035BB RID: 13755
	[SerializeField]
	private AudioSource replacementVoiceSource;

	// Token: 0x040035BC RID: 13756
	private List<LoudSpeakerNetwork> loudSpeakerNetworks;

	// Token: 0x040035BD RID: 13757
	private PhotonVoiceView voiceView;

	// Token: 0x040035BE RID: 13758
	private int m_cachedNetViewID;

	// Token: 0x040035BF RID: 13759
	private bool enableVoice = true;

	// Token: 0x040035C0 RID: 13760
	private bool forceMute;

	// Token: 0x040035C1 RID: 13761
	[SerializeField]
	private SphereCollider headCollider;

	// Token: 0x040035C2 RID: 13762
	[SerializeField]
	private CapsuleCollider bodyCollider;

	// Token: 0x040035C3 RID: 13763
	[SerializeField]
	private VRRigEvents rigEvents;

	// Token: 0x040035C4 RID: 13764
	public bool hasManualMute;

	// Token: 0x040035C5 RID: 13765
	private bool bPlayerAutoMuted;

	// Token: 0x040035C6 RID: 13766
	public int playerChatQuality = 2;

	// Token: 0x040035C7 RID: 13767
	private static List<NetPlayer> playersToCheckAutomute = new List<NetPlayer>();

	// Token: 0x040035C8 RID: 13768
	private static bool automuteQueued = false;

	// Token: 0x040035C9 RID: 13769
	private static List<NetPlayer> requestedAutomutePlayers;

	// Token: 0x040035CA RID: 13770
	private static bool waitingForAutomuteCallback = false;

	// Token: 0x040035CB RID: 13771
	private static RigContainer staticTempRC;
}
