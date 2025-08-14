using System;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaNetworking
{
	// Token: 0x02000D83 RID: 3459
	public class GorillaNetworkJoinTrigger : GorillaTriggerBox
	{
		// Token: 0x1700083E RID: 2110
		// (get) Token: 0x0600561A RID: 22042 RVA: 0x001ABD08 File Offset: 0x001A9F08
		public GroupJoinZoneAB groupJoinRequiredZonesAB
		{
			get
			{
				return new GroupJoinZoneAB
				{
					a = this.groupJoinRequiredZones,
					b = this.groupJoinRequiredZonesB
				};
			}
		}

		// Token: 0x0600561B RID: 22043 RVA: 0x001ABD38 File Offset: 0x001A9F38
		private void Start()
		{
			if (this.primaryTriggerForMyZone == null)
			{
				this.primaryTriggerForMyZone = this;
			}
			if (this.primaryTriggerForMyZone == this)
			{
				GorillaComputer.instance.RegisterPrimaryJoinTrigger(this);
			}
			PhotonNetworkController.Instance.RegisterJoinTrigger(this);
			if (!this.didRegisterForCallbacks && this.ui != null)
			{
				this.didRegisterForCallbacks = true;
				FriendshipGroupDetection.Instance.AddGroupZoneCallback(new Action<GroupJoinZoneAB>(this.OnGroupPositionsChanged));
			}
		}

		// Token: 0x0600561C RID: 22044 RVA: 0x001ABDB8 File Offset: 0x001A9FB8
		public void RegisterUI(JoinTriggerUI ui)
		{
			this.ui = ui;
			if (!this.didRegisterForCallbacks && FriendshipGroupDetection.Instance != null)
			{
				this.didRegisterForCallbacks = true;
				FriendshipGroupDetection.Instance.AddGroupZoneCallback(new Action<GroupJoinZoneAB>(this.OnGroupPositionsChanged));
			}
			this.UpdateUI();
		}

		// Token: 0x0600561D RID: 22045 RVA: 0x001ABE04 File Offset: 0x001AA004
		public void UnregisterUI(JoinTriggerUI ui)
		{
			this.ui = null;
		}

		// Token: 0x0600561E RID: 22046 RVA: 0x001ABE0D File Offset: 0x001AA00D
		private void OnDestroy()
		{
			if (this.didRegisterForCallbacks)
			{
				FriendshipGroupDetection.Instance.RemoveGroupZoneCallback(new Action<GroupJoinZoneAB>(this.OnGroupPositionsChanged));
			}
		}

		// Token: 0x0600561F RID: 22047 RVA: 0x001ABE2D File Offset: 0x001AA02D
		private void OnGroupPositionsChanged(GroupJoinZoneAB groupZone)
		{
			this.UpdateUI();
		}

		// Token: 0x06005620 RID: 22048 RVA: 0x001ABE38 File Offset: 0x001AA038
		public void UpdateUI()
		{
			if (this.ui == null || NetworkSystem.Instance == null)
			{
				return;
			}
			if (GorillaScoreboardTotalUpdater.instance.offlineTextErrorString != null)
			{
				this.ui.SetState(JoinTriggerVisualState.ConnectionError, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameType));
				return;
			}
			if (NetworkSystem.Instance.SessionIsPrivate)
			{
				this.ui.SetState(JoinTriggerVisualState.InPrivateRoom, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameType));
				return;
			}
			if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.GameModeString == this.GetFullDesiredGameModeString())
			{
				this.ui.SetState(JoinTriggerVisualState.AlreadyInRoom, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameType));
				return;
			}
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				this.ui.SetState(this.CanPartyJoin() ? JoinTriggerVisualState.LeaveRoomAndPartyJoin : JoinTriggerVisualState.AbandonPartyAndSoloJoin, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameType));
				return;
			}
			if (!NetworkSystem.Instance.InRoom)
			{
				this.ui.SetState(JoinTriggerVisualState.NotConnectedSoloJoin, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameType));
				return;
			}
			if (PhotonNetworkController.Instance.currentJoinTrigger == this.primaryTriggerForMyZone)
			{
				this.ui.SetState(JoinTriggerVisualState.ChangingGameModeSoloJoin, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameType));
				return;
			}
			this.ui.SetState(JoinTriggerVisualState.LeaveRoomAndSoloJoin, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameType));
		}

		// Token: 0x06005621 RID: 22049 RVA: 0x001AC082 File Offset: 0x001AA282
		private string GetActiveNetworkZone()
		{
			return PhotonNetworkController.Instance.currentJoinTrigger.networkZone.ToUpper();
		}

		// Token: 0x06005622 RID: 22050 RVA: 0x001AC09A File Offset: 0x001AA29A
		private string GetDesiredNetworkZone()
		{
			return this.networkZone.ToUpper();
		}

		// Token: 0x06005623 RID: 22051 RVA: 0x001AC0A7 File Offset: 0x001AA2A7
		public static string GetActiveGameType()
		{
			GorillaGameManager activeGameMode = GameMode.ActiveGameMode;
			return ((activeGameMode != null) ? activeGameMode.GameModeName() : null) ?? "";
		}

		// Token: 0x06005624 RID: 22052 RVA: 0x001AC0C4 File Offset: 0x001AA2C4
		public string GetDesiredGameType()
		{
			return GameMode.GameModeZoneMapping.VerifyModeForZone(this.zone, Enum.Parse<GameModeType>(GorillaComputer.instance.currentGameMode.Value, true), NetworkSystem.Instance.SessionIsPrivate).ToString();
		}

		// Token: 0x06005625 RID: 22053 RVA: 0x001AC110 File Offset: 0x001AA310
		public virtual string GetFullDesiredGameModeString()
		{
			return this.networkZone + GorillaComputer.instance.currentQueue + this.GetDesiredGameType();
		}

		// Token: 0x06005626 RID: 22054 RVA: 0x001AC12F File Offset: 0x001AA32F
		public virtual byte GetRoomSize()
		{
			return RoomSystem.GetRoomSizeForCreate(this.networkZone);
		}

		// Token: 0x06005627 RID: 22055 RVA: 0x001AC13C File Offset: 0x001AA33C
		public bool CanPartyJoin()
		{
			return this.CanPartyJoin(FriendshipGroupDetection.Instance.partyZone);
		}

		// Token: 0x06005628 RID: 22056 RVA: 0x001AC14E File Offset: 0x001AA34E
		public bool CanPartyJoin(GroupJoinZoneAB zone)
		{
			return (this.groupJoinRequiredZonesAB & zone) == zone;
		}

		// Token: 0x06005629 RID: 22057 RVA: 0x001AC164 File Offset: 0x001AA364
		public override void OnBoxTriggered()
		{
			base.OnBoxTriggered();
			if (GorillaNetworkJoinTrigger.triggerJoinsDisabled)
			{
				Debug.Log("GorillaNetworkJoinTrigger::OnBoxTriggered - blocking join call");
				return;
			}
			GorillaComputer.instance.allowedMapsToJoin = this.myCollider.myAllowedMapsToJoin;
			if (NetworkSystem.Instance.groupJoinInProgress)
			{
				return;
			}
			List<ValueTuple<string, string>> list = new List<ValueTuple<string, string>>();
			foreach (AdditionalCustomProperty additionalCustomProperty in this.additionalJoinCustomProperties)
			{
				list.Add(new ValueTuple<string, string>(additionalCustomProperty.key, additionalCustomProperty.value));
			}
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				if (this.ignoredIfInParty)
				{
					return;
				}
				if (NetworkSystem.Instance.netState == NetSystemState.Connecting || NetworkSystem.Instance.netState == NetSystemState.Disconnecting || NetworkSystem.Instance.netState == NetSystemState.Initialization || NetworkSystem.Instance.netState == NetSystemState.PingRecon)
				{
					return;
				}
				if (NetworkSystem.Instance.InRoom)
				{
					if (NetworkSystem.Instance.GameModeString == this.GetFullDesiredGameModeString())
					{
						Debug.Log("JoinTrigger: Ignoring party join/leave because " + this.networkZone + " is already the game mode");
						return;
					}
					if (NetworkSystem.Instance.SessionIsPrivate)
					{
						Debug.Log("JoinTrigger: Ignoring party join/leave because we're in a private room");
						return;
					}
				}
				if (this.CanPartyJoin())
				{
					Debug.Log(string.Format("JoinTrigger: Attempting party join in 1 second! <{0}> accepts <{1}>", this.groupJoinRequiredZones, FriendshipGroupDetection.Instance.partyZone));
					PhotonNetworkController.Instance.DeferJoining(1f);
					FriendshipGroupDetection.Instance.SendAboutToGroupJoin();
					PhotonNetworkController.Instance.AttemptToJoinPublicRoom(this, JoinType.JoinWithParty, list);
					return;
				}
				Debug.Log(string.Format("JoinTrigger: LeaveGroup: Leaving party and will solo join, wanted <{0}> but got <{1}>", this.groupJoinRequiredZones, FriendshipGroupDetection.Instance.partyZone));
				FriendshipGroupDetection.Instance.LeaveParty();
				PhotonNetworkController.Instance.DeferJoining(1f);
			}
			else
			{
				Debug.Log("JoinTrigger: Solo join (not in a group)");
				PhotonNetworkController.Instance.ClearDeferredJoin();
			}
			PhotonNetworkController.Instance.AttemptToJoinPublicRoom(this, JoinType.Solo, list);
		}

		// Token: 0x0600562A RID: 22058 RVA: 0x001AC34F File Offset: 0x001AA54F
		public static void DisableTriggerJoins()
		{
			Debug.Log("[GorillaNetworkJoinTrigger::DisableTriggerJoins] Disabling Trigger-based Room Joins...");
			GorillaNetworkJoinTrigger.triggerJoinsDisabled = true;
		}

		// Token: 0x0600562B RID: 22059 RVA: 0x001AC361 File Offset: 0x001AA561
		public static void EnableTriggerJoins()
		{
			Debug.Log("[GorillaNetworkJoinTrigger::EnableTriggerJoins] Enabling Trigger-based Room Joins...");
			GorillaNetworkJoinTrigger.triggerJoinsDisabled = false;
		}

		// Token: 0x04005FDD RID: 24541
		public GameObject[] makeSureThisIsDisabled;

		// Token: 0x04005FDE RID: 24542
		public GameObject[] makeSureThisIsEnabled;

		// Token: 0x04005FDF RID: 24543
		public GTZone zone;

		// Token: 0x04005FE0 RID: 24544
		public GroupJoinZoneA groupJoinRequiredZones;

		// Token: 0x04005FE1 RID: 24545
		public GroupJoinZoneB groupJoinRequiredZonesB;

		// Token: 0x04005FE2 RID: 24546
		[FormerlySerializedAs("gameModeName")]
		public string networkZone;

		// Token: 0x04005FE3 RID: 24547
		public string componentTypeToAdd;

		// Token: 0x04005FE4 RID: 24548
		public GameObject componentTarget;

		// Token: 0x04005FE5 RID: 24549
		public GorillaFriendCollider myCollider;

		// Token: 0x04005FE6 RID: 24550
		public GorillaNetworkJoinTrigger primaryTriggerForMyZone;

		// Token: 0x04005FE7 RID: 24551
		public bool ignoredIfInParty;

		// Token: 0x04005FE8 RID: 24552
		private JoinTriggerUI ui;

		// Token: 0x04005FE9 RID: 24553
		private bool didRegisterForCallbacks;

		// Token: 0x04005FEA RID: 24554
		public AdditionalCustomProperty[] additionalJoinCustomProperties;

		// Token: 0x04005FEB RID: 24555
		private static bool triggerJoinsDisabled;
	}
}
