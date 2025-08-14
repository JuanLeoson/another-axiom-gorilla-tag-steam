using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Fusion;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000D9B RID: 3483
	public class PhotonNetworkController : MonoBehaviour
	{
		// Token: 0x17000842 RID: 2114
		// (get) Token: 0x06005675 RID: 22133 RVA: 0x001AD521 File Offset: 0x001AB721
		// (set) Token: 0x06005676 RID: 22134 RVA: 0x001AD529 File Offset: 0x001AB729
		public List<string> FriendIDList
		{
			get
			{
				return this.friendIDList;
			}
			set
			{
				this.friendIDList = value;
			}
		}

		// Token: 0x17000843 RID: 2115
		// (get) Token: 0x06005677 RID: 22135 RVA: 0x001AD532 File Offset: 0x001AB732
		// (set) Token: 0x06005678 RID: 22136 RVA: 0x001AD53A File Offset: 0x001AB73A
		public string StartLevel
		{
			get
			{
				return this.startLevel;
			}
			set
			{
				this.startLevel = value;
			}
		}

		// Token: 0x17000844 RID: 2116
		// (get) Token: 0x06005679 RID: 22137 RVA: 0x001AD543 File Offset: 0x001AB743
		// (set) Token: 0x0600567A RID: 22138 RVA: 0x001AD54B File Offset: 0x001AB74B
		public GTZone StartZone
		{
			get
			{
				return this.startZone;
			}
			set
			{
				this.startZone = value;
			}
		}

		// Token: 0x17000845 RID: 2117
		// (get) Token: 0x0600567B RID: 22139 RVA: 0x001AD554 File Offset: 0x001AB754
		public GTZone CurrentRoomZone
		{
			get
			{
				if (!(this.currentJoinTrigger != null))
				{
					return GTZone.none;
				}
				return this.currentJoinTrigger.zone;
			}
		}

		// Token: 0x17000846 RID: 2118
		// (get) Token: 0x0600567C RID: 22140 RVA: 0x001AD572 File Offset: 0x001AB772
		// (set) Token: 0x0600567D RID: 22141 RVA: 0x001AD57A File Offset: 0x001AB77A
		public GorillaGeoHideShowTrigger StartGeoTrigger
		{
			get
			{
				return this.startGeoTrigger;
			}
			set
			{
				this.startGeoTrigger = value;
			}
		}

		// Token: 0x0600567E RID: 22142 RVA: 0x001AD584 File Offset: 0x001AB784
		public void Awake()
		{
			if (PhotonNetworkController.Instance == null)
			{
				PhotonNetworkController.Instance = this;
			}
			else if (PhotonNetworkController.Instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			this.updatedName = false;
			this.playersInRegion = new int[this.serverRegions.Length];
			this.pingInRegion = new int[this.serverRegions.Length];
		}

		// Token: 0x0600567F RID: 22143 RVA: 0x001AD5F4 File Offset: 0x001AB7F4
		public void Start()
		{
			base.StartCoroutine(this.DisableOnStart());
			NetworkSystem.Instance.OnJoinedRoomEvent += this.OnJoinedRoom;
			NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnDisconnected;
			PhotonNetwork.NetworkingClient.LoadBalancingPeer.ReuseEventInstance = true;
		}

		// Token: 0x06005680 RID: 22144 RVA: 0x001AD660 File Offset: 0x001AB860
		private IEnumerator DisableOnStart()
		{
			ZoneManagement.SetActiveZone(this.StartZone);
			yield break;
		}

		// Token: 0x06005681 RID: 22145 RVA: 0x001AD670 File Offset: 0x001AB870
		public void FixedUpdate()
		{
			this.headRightHandDistance = (GTPlayer.Instance.headCollider.transform.position - GTPlayer.Instance.rightControllerTransform.position).magnitude;
			this.headLeftHandDistance = (GTPlayer.Instance.headCollider.transform.position - GTPlayer.Instance.leftControllerTransform.position).magnitude;
			this.headQuat = GTPlayer.Instance.headCollider.transform.rotation;
			if (!this.disableAFKKick && Quaternion.Angle(this.headQuat, this.lastHeadQuat) <= 0.01f && Mathf.Abs(this.headRightHandDistance - this.lastHeadRightHandDistance) < 0.001f && Mathf.Abs(this.headLeftHandDistance - this.lastHeadLeftHandDistance) < 0.001f && this.pauseTime + this.disconnectTime < Time.realtimeSinceStartup)
			{
				this.pauseTime = Time.realtimeSinceStartup;
				NetworkSystem.Instance.ReturnToSinglePlayer();
			}
			else if (Quaternion.Angle(this.headQuat, this.lastHeadQuat) > 0.01f || Mathf.Abs(this.headRightHandDistance - this.lastHeadRightHandDistance) >= 0.001f || Mathf.Abs(this.headLeftHandDistance - this.lastHeadLeftHandDistance) >= 0.001f)
			{
				this.pauseTime = Time.realtimeSinceStartup;
			}
			this.lastHeadRightHandDistance = this.headRightHandDistance;
			this.lastHeadLeftHandDistance = this.headLeftHandDistance;
			this.lastHeadQuat = this.headQuat;
			if (this.deferredJoin && Time.time >= this.partyJoinDeferredUntilTimestamp)
			{
				if ((this.partyJoinDeferredUntilTimestamp != 0f || NetworkSystem.Instance.netState == NetSystemState.Idle) && this.currentJoinTrigger != null)
				{
					this.deferredJoin = false;
					this.partyJoinDeferredUntilTimestamp = 0f;
					if (this.currentJoinTrigger == this.privateTrigger)
					{
						this.AttemptToJoinSpecificRoom(this.customRoomID, FriendshipGroupDetection.Instance.IsInParty ? JoinType.JoinWithParty : JoinType.Solo);
						return;
					}
					this.AttemptToJoinPublicRoom(this.currentJoinTrigger, this.currentJoinType, null);
					return;
				}
				else if (NetworkSystem.Instance.netState != NetSystemState.PingRecon && NetworkSystem.Instance.netState != NetSystemState.Initialization)
				{
					this.deferredJoin = false;
					this.partyJoinDeferredUntilTimestamp = 0f;
				}
			}
		}

		// Token: 0x06005682 RID: 22146 RVA: 0x001AD8BE File Offset: 0x001ABABE
		public void DeferJoining(float duration)
		{
			this.partyJoinDeferredUntilTimestamp = Mathf.Max(this.partyJoinDeferredUntilTimestamp, Time.time + duration);
		}

		// Token: 0x06005683 RID: 22147 RVA: 0x001AD8D8 File Offset: 0x001ABAD8
		public void ClearDeferredJoin()
		{
			this.partyJoinDeferredUntilTimestamp = 0f;
			this.deferredJoin = false;
		}

		// Token: 0x06005684 RID: 22148 RVA: 0x001AD8EC File Offset: 0x001ABAEC
		public void AttemptToJoinPublicRoom(GorillaNetworkJoinTrigger triggeredTrigger, JoinType roomJoinType = JoinType.Solo, List<ValueTuple<string, string>> additionalCustomProperties = null)
		{
			this.AttemptToJoinPublicRoomAsync(triggeredTrigger, roomJoinType, additionalCustomProperties);
		}

		// Token: 0x06005685 RID: 22149 RVA: 0x001AD8F8 File Offset: 0x001ABAF8
		private void AttemptToJoinPublicRoomAsync(GorillaNetworkJoinTrigger triggeredTrigger, JoinType roomJoinType, List<ValueTuple<string, string>> additionalCustomProperties)
		{
			PhotonNetworkController.<AttemptToJoinPublicRoomAsync>d__67 <AttemptToJoinPublicRoomAsync>d__;
			<AttemptToJoinPublicRoomAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AttemptToJoinPublicRoomAsync>d__.<>4__this = this;
			<AttemptToJoinPublicRoomAsync>d__.triggeredTrigger = triggeredTrigger;
			<AttemptToJoinPublicRoomAsync>d__.roomJoinType = roomJoinType;
			<AttemptToJoinPublicRoomAsync>d__.additionalCustomProperties = additionalCustomProperties;
			<AttemptToJoinPublicRoomAsync>d__.<>1__state = -1;
			<AttemptToJoinPublicRoomAsync>d__.<>t__builder.Start<PhotonNetworkController.<AttemptToJoinPublicRoomAsync>d__67>(ref <AttemptToJoinPublicRoomAsync>d__);
		}

		// Token: 0x06005686 RID: 22150 RVA: 0x001AD948 File Offset: 0x001ABB48
		public void AttemptToJoinRankedPublicRoom(GorillaNetworkJoinTrigger triggeredTrigger, JoinType roomJoinType = JoinType.Solo)
		{
			string mmrTier = RankedProgressionManager.Instance.GetRankedMatchmakingTier().ToString();
			string platform = "PC";
			this.AttemptToJoinRankedPublicRoomAsync(triggeredTrigger, mmrTier, platform, roomJoinType);
		}

		// Token: 0x06005687 RID: 22151 RVA: 0x001AD984 File Offset: 0x001ABB84
		private void AttemptToJoinRankedPublicRoomAsync(GorillaNetworkJoinTrigger triggeredTrigger, string mmrTier, string platform, JoinType roomJoinType)
		{
			PhotonNetworkController.<AttemptToJoinRankedPublicRoomAsync>d__69 <AttemptToJoinRankedPublicRoomAsync>d__;
			<AttemptToJoinRankedPublicRoomAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AttemptToJoinRankedPublicRoomAsync>d__.<>4__this = this;
			<AttemptToJoinRankedPublicRoomAsync>d__.triggeredTrigger = triggeredTrigger;
			<AttemptToJoinRankedPublicRoomAsync>d__.mmrTier = mmrTier;
			<AttemptToJoinRankedPublicRoomAsync>d__.platform = platform;
			<AttemptToJoinRankedPublicRoomAsync>d__.roomJoinType = roomJoinType;
			<AttemptToJoinRankedPublicRoomAsync>d__.<>1__state = -1;
			<AttemptToJoinRankedPublicRoomAsync>d__.<>t__builder.Start<PhotonNetworkController.<AttemptToJoinRankedPublicRoomAsync>d__69>(ref <AttemptToJoinRankedPublicRoomAsync>d__);
		}

		// Token: 0x06005688 RID: 22152 RVA: 0x001AD9DC File Offset: 0x001ABBDC
		private Task SendPartyFollowCommands()
		{
			PhotonNetworkController.<SendPartyFollowCommands>d__70 <SendPartyFollowCommands>d__;
			<SendPartyFollowCommands>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SendPartyFollowCommands>d__.<>1__state = -1;
			<SendPartyFollowCommands>d__.<>t__builder.Start<PhotonNetworkController.<SendPartyFollowCommands>d__70>(ref <SendPartyFollowCommands>d__);
			return <SendPartyFollowCommands>d__.<>t__builder.Task;
		}

		// Token: 0x06005689 RID: 22153 RVA: 0x001ADA17 File Offset: 0x001ABC17
		public void AttemptToJoinSpecificRoom(string roomID, JoinType roomJoinType)
		{
			this.AttemptToJoinSpecificRoomAsync(roomID, roomJoinType, null);
		}

		// Token: 0x0600568A RID: 22154 RVA: 0x001ADA23 File Offset: 0x001ABC23
		public void AttemptToJoinSpecificRoomWithCallback(string roomID, JoinType roomJoinType, Action<NetJoinResult> callback)
		{
			this.AttemptToJoinSpecificRoomAsync(roomID, roomJoinType, callback);
		}

		// Token: 0x0600568B RID: 22155 RVA: 0x001ADA30 File Offset: 0x001ABC30
		public Task AttemptToJoinSpecificRoomAsync(string roomID, JoinType roomJoinType, Action<NetJoinResult> callback)
		{
			PhotonNetworkController.<AttemptToJoinSpecificRoomAsync>d__73 <AttemptToJoinSpecificRoomAsync>d__;
			<AttemptToJoinSpecificRoomAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AttemptToJoinSpecificRoomAsync>d__.<>4__this = this;
			<AttemptToJoinSpecificRoomAsync>d__.roomID = roomID;
			<AttemptToJoinSpecificRoomAsync>d__.roomJoinType = roomJoinType;
			<AttemptToJoinSpecificRoomAsync>d__.callback = callback;
			<AttemptToJoinSpecificRoomAsync>d__.<>1__state = -1;
			<AttemptToJoinSpecificRoomAsync>d__.<>t__builder.Start<PhotonNetworkController.<AttemptToJoinSpecificRoomAsync>d__73>(ref <AttemptToJoinSpecificRoomAsync>d__);
			return <AttemptToJoinSpecificRoomAsync>d__.<>t__builder.Task;
		}

		// Token: 0x0600568C RID: 22156 RVA: 0x001ADA8C File Offset: 0x001ABC8C
		private void DisconnectCleanup()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			if (GorillaParent.instance != null)
			{
				GorillaScoreboardSpawner[] componentsInChildren = GorillaParent.instance.GetComponentsInChildren<GorillaScoreboardSpawner>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].OnLeftRoom();
				}
			}
			this.attemptingToConnect = true;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in this.offlineVRRig)
			{
				if (skinnedMeshRenderer != null)
				{
					skinnedMeshRenderer.enabled = true;
				}
			}
			if (GorillaComputer.instance != null && !ApplicationQuittingState.IsQuitting)
			{
				this.UpdateTriggerScreens();
			}
			GTPlayer.Instance.maxJumpSpeed = 6.5f;
			GTPlayer.Instance.jumpMultiplier = 1.1f;
			GorillaNot.instance.currentMasterClient = null;
			GorillaTagger.Instance.offlineVRRig.huntComputer.SetActive(false);
			this.initialGameMode = "";
		}

		// Token: 0x0600568D RID: 22157 RVA: 0x001ADB6C File Offset: 0x001ABD6C
		public void OnJoinedRoom()
		{
			if (NetworkSystem.Instance.GameModeString.IsNullOrEmpty())
			{
				NetworkSystem.Instance.ReturnToSinglePlayer();
			}
			this.initialGameMode = NetworkSystem.Instance.GameModeString;
			if (NetworkSystem.Instance.SessionIsPrivate)
			{
				this.currentJoinTrigger = this.privateTrigger;
				PhotonNetworkController.Instance.UpdateTriggerScreens();
			}
			else if (this.currentJoinType != JoinType.FollowingParty)
			{
				bool flag = false;
				for (int i = 0; i < GorillaComputer.instance.allowedMapsToJoin.Length; i++)
				{
					if (NetworkSystem.Instance.GameModeString.StartsWith(GorillaComputer.instance.allowedMapsToJoin[i]))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					GorillaComputer.instance.roomNotAllowed = true;
					NetworkSystem.Instance.ReturnToSinglePlayer();
					return;
				}
			}
			NetworkSystem.Instance.SetMyTutorialComplete();
			VRRigCache.Instance.InstantiateNetworkObject();
			if (NetworkSystem.Instance.IsMasterClient)
			{
				GorillaGameModes.GameMode.LoadGameModeFromProperty(this.initialGameMode);
			}
			GorillaComputer.instance.roomFull = false;
			GorillaComputer.instance.roomNotAllowed = false;
			if (this.currentJoinType == JoinType.JoinWithParty || this.currentJoinType == JoinType.JoinWithNearby || this.currentJoinType == JoinType.ForceJoinWithParty || this.currentJoinType == JoinType.JoinWithElevator)
			{
				this.keyToFollow = NetworkSystem.Instance.LocalPlayer.UserId + this.keyStr;
				NetworkSystem.Instance.BroadcastMyRoom(true, this.keyToFollow, this.shuffler);
			}
			GorillaNot.instance.currentMasterClient = null;
			this.UpdateCurrentJoinTrigger();
			this.UpdateTriggerScreens();
			NetworkSystem.Instance.MultiplayerStarted();
		}

		// Token: 0x0600568E RID: 22158 RVA: 0x001ADCF2 File Offset: 0x001ABEF2
		public void RegisterJoinTrigger(GorillaNetworkJoinTrigger trigger)
		{
			this.allJoinTriggers.Add(trigger);
		}

		// Token: 0x0600568F RID: 22159 RVA: 0x001ADD00 File Offset: 0x001ABF00
		private void UpdateCurrentJoinTrigger()
		{
			GorillaNetworkJoinTrigger joinTriggerFromFullGameModeString = GorillaComputer.instance.GetJoinTriggerFromFullGameModeString(NetworkSystem.Instance.GameModeString);
			if (joinTriggerFromFullGameModeString != null)
			{
				this.currentJoinTrigger = joinTriggerFromFullGameModeString;
				return;
			}
			if (NetworkSystem.Instance.SessionIsPrivate)
			{
				if (this.currentJoinTrigger != this.privateTrigger)
				{
					Debug.LogError("IN a private game but private trigger isnt current");
					return;
				}
			}
			else
			{
				Debug.LogError("Not in private room and unabel tp update jointrigger.");
			}
		}

		// Token: 0x06005690 RID: 22160 RVA: 0x001ADD6C File Offset: 0x001ABF6C
		public void UpdateTriggerScreens()
		{
			foreach (GorillaNetworkJoinTrigger gorillaNetworkJoinTrigger in this.allJoinTriggers)
			{
				gorillaNetworkJoinTrigger.UpdateUI();
			}
		}

		// Token: 0x06005691 RID: 22161 RVA: 0x001ADDBC File Offset: 0x001ABFBC
		public void AttemptToFollowIntoPub(string userIDToFollow, int actorNumberToFollow, string newKeyStr, string shufflerStr, JoinType joinType)
		{
			this.friendToFollow = userIDToFollow;
			this.keyToFollow = userIDToFollow + newKeyStr;
			this.shuffler = shufflerStr;
			this.currentJoinType = joinType;
			this.ClearDeferredJoin();
			if (NetworkSystem.Instance.InRoom)
			{
				NetworkSystem.Instance.JoinFriendsRoom(this.friendToFollow, actorNumberToFollow, this.keyToFollow, this.shuffler);
			}
		}

		// Token: 0x06005692 RID: 22162 RVA: 0x001ADE1D File Offset: 0x001AC01D
		public void OnDisconnected()
		{
			this.DisconnectCleanup();
		}

		// Token: 0x06005693 RID: 22163 RVA: 0x001ADE25 File Offset: 0x001AC025
		public void OnApplicationQuit()
		{
			if (PhotonNetwork.IsConnected)
			{
				PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion != "dev";
			}
		}

		// Token: 0x06005694 RID: 22164 RVA: 0x001ADE48 File Offset: 0x001AC048
		private string ReturnRoomName()
		{
			if (this.isPrivate)
			{
				return this.customRoomID;
			}
			return this.RandomRoomName();
		}

		// Token: 0x06005695 RID: 22165 RVA: 0x001ADE60 File Offset: 0x001AC060
		private string RandomRoomName()
		{
			string text = "";
			for (int i = 0; i < 4; i++)
			{
				text += "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Substring(Random.Range(0, "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Length), 1);
			}
			if (GorillaComputer.instance.CheckAutoBanListForName(text))
			{
				return text;
			}
			return this.RandomRoomName();
		}

		// Token: 0x06005696 RID: 22166 RVA: 0x001ADEB8 File Offset: 0x001AC0B8
		private string GetRegionWithLowestPing()
		{
			int num = 10000;
			int num2 = 0;
			for (int i = 0; i < this.serverRegions.Length; i++)
			{
				Debug.Log("ping in region " + this.serverRegions[i] + " is " + this.pingInRegion[i].ToString());
				if (this.pingInRegion[i] < num && this.pingInRegion[i] > 0)
				{
					num = this.pingInRegion[i];
					num2 = i;
				}
			}
			return this.serverRegions[num2];
		}

		// Token: 0x06005697 RID: 22167 RVA: 0x001ADF38 File Offset: 0x001AC138
		public int TotalUsers()
		{
			int num = 0;
			foreach (int num2 in this.playersInRegion)
			{
				num += num2;
			}
			return num;
		}

		// Token: 0x06005698 RID: 22168 RVA: 0x001ADF68 File Offset: 0x001AC168
		public string CurrentState()
		{
			if (NetworkSystem.Instance == null)
			{
				Debug.Log("Null netsys!!!");
			}
			return NetworkSystem.Instance.netState.ToString();
		}

		// Token: 0x06005699 RID: 22169 RVA: 0x001ADFA4 File Offset: 0x001AC1A4
		private void OnApplicationPause(bool pause)
		{
			if (pause)
			{
				this.timeWhenApplicationPaused = new DateTime?(DateTime.Now);
				return;
			}
			if ((DateTime.Now - (this.timeWhenApplicationPaused ?? DateTime.Now)).TotalSeconds > (double)this.disconnectTime)
			{
				this.timeWhenApplicationPaused = null;
				NetworkSystem instance = NetworkSystem.Instance;
				if (instance != null)
				{
					instance.ReturnToSinglePlayer();
				}
			}
			if (NetworkSystem.Instance != null && !NetworkSystem.Instance.InRoom && NetworkSystem.Instance.netState == NetSystemState.InGame)
			{
				NetworkSystem instance2 = NetworkSystem.Instance;
				if (instance2 == null)
				{
					return;
				}
				instance2.ReturnToSinglePlayer();
			}
		}

		// Token: 0x0600569A RID: 22170 RVA: 0x001AE051 File Offset: 0x001AC251
		private void OnApplicationFocus(bool focus)
		{
			if (!focus && NetworkSystem.Instance != null && !NetworkSystem.Instance.InRoom && NetworkSystem.Instance.netState == NetSystemState.InGame)
			{
				NetworkSystem instance = NetworkSystem.Instance;
				if (instance == null)
				{
					return;
				}
				instance.ReturnToSinglePlayer();
			}
		}

		// Token: 0x04006033 RID: 24627
		public static volatile PhotonNetworkController Instance;

		// Token: 0x04006034 RID: 24628
		public int incrementCounter;

		// Token: 0x04006035 RID: 24629
		public PlayFabAuthenticator playFabAuthenticator;

		// Token: 0x04006036 RID: 24630
		public string[] serverRegions;

		// Token: 0x04006037 RID: 24631
		public bool isPrivate;

		// Token: 0x04006038 RID: 24632
		public string customRoomID;

		// Token: 0x04006039 RID: 24633
		public GameObject playerOffset;

		// Token: 0x0400603A RID: 24634
		public SkinnedMeshRenderer[] offlineVRRig;

		// Token: 0x0400603B RID: 24635
		public bool attemptingToConnect;

		// Token: 0x0400603C RID: 24636
		private int currentRegionIndex;

		// Token: 0x0400603D RID: 24637
		public string currentGameType;

		// Token: 0x0400603E RID: 24638
		public bool roomCosmeticsInitialized;

		// Token: 0x0400603F RID: 24639
		public GameObject photonVoiceObjectPrefab;

		// Token: 0x04006040 RID: 24640
		public Dictionary<string, bool> playerCosmeticsLookup = new Dictionary<string, bool>();

		// Token: 0x04006041 RID: 24641
		private float lastHeadRightHandDistance;

		// Token: 0x04006042 RID: 24642
		private float lastHeadLeftHandDistance;

		// Token: 0x04006043 RID: 24643
		private float pauseTime;

		// Token: 0x04006044 RID: 24644
		private float disconnectTime = 120f;

		// Token: 0x04006045 RID: 24645
		public bool disableAFKKick;

		// Token: 0x04006046 RID: 24646
		private float headRightHandDistance;

		// Token: 0x04006047 RID: 24647
		private float headLeftHandDistance;

		// Token: 0x04006048 RID: 24648
		private Quaternion headQuat;

		// Token: 0x04006049 RID: 24649
		private Quaternion lastHeadQuat;

		// Token: 0x0400604A RID: 24650
		public GameObject[] disableOnStartup;

		// Token: 0x0400604B RID: 24651
		public GameObject[] enableOnStartup;

		// Token: 0x0400604C RID: 24652
		public bool updatedName;

		// Token: 0x0400604D RID: 24653
		private int[] playersInRegion;

		// Token: 0x0400604E RID: 24654
		private int[] pingInRegion;

		// Token: 0x0400604F RID: 24655
		private List<string> friendIDList = new List<string>();

		// Token: 0x04006050 RID: 24656
		private JoinType currentJoinType;

		// Token: 0x04006051 RID: 24657
		private string friendToFollow;

		// Token: 0x04006052 RID: 24658
		private string keyToFollow;

		// Token: 0x04006053 RID: 24659
		public string shuffler;

		// Token: 0x04006054 RID: 24660
		public string keyStr;

		// Token: 0x04006055 RID: 24661
		private string platformTag = "OTHER";

		// Token: 0x04006056 RID: 24662
		private string startLevel;

		// Token: 0x04006057 RID: 24663
		[SerializeField]
		private GTZone startZone;

		// Token: 0x04006058 RID: 24664
		private GorillaGeoHideShowTrigger startGeoTrigger;

		// Token: 0x04006059 RID: 24665
		public GorillaNetworkJoinTrigger privateTrigger;

		// Token: 0x0400605A RID: 24666
		internal string initialGameMode = "";

		// Token: 0x0400605B RID: 24667
		public GorillaNetworkJoinTrigger currentJoinTrigger;

		// Token: 0x0400605C RID: 24668
		public string autoJoinRoom;

		// Token: 0x0400605D RID: 24669
		private bool deferredJoin;

		// Token: 0x0400605E RID: 24670
		private float partyJoinDeferredUntilTimestamp;

		// Token: 0x0400605F RID: 24671
		private DateTime? timeWhenApplicationPaused;

		// Token: 0x04006060 RID: 24672
		[NetworkPrefab]
		[SerializeField]
		private NetworkObject testPlayerPrefab;

		// Token: 0x04006061 RID: 24673
		private List<GorillaNetworkJoinTrigger> allJoinTriggers = new List<GorillaNetworkJoinTrigger>();
	}
}
