using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusion;
using GorillaExtensions;
using GorillaGameModes;
using GorillaNetworking;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;

namespace GorillaTagScripts
{
	// Token: 0x02000C30 RID: 3120
	public class FriendshipGroupDetection : NetworkSceneObject
	{
		// Token: 0x17000740 RID: 1856
		// (get) Token: 0x06004CCC RID: 19660 RVA: 0x0017CCD8 File Offset: 0x0017AED8
		// (set) Token: 0x06004CCD RID: 19661 RVA: 0x0017CCDF File Offset: 0x0017AEDF
		public static FriendshipGroupDetection Instance { get; private set; }

		// Token: 0x17000741 RID: 1857
		// (get) Token: 0x06004CCE RID: 19662 RVA: 0x0017CCE7 File Offset: 0x0017AEE7
		// (set) Token: 0x06004CCF RID: 19663 RVA: 0x0017CCEF File Offset: 0x0017AEEF
		public List<Color> myBeadColors { get; private set; } = new List<Color>();

		// Token: 0x17000742 RID: 1858
		// (get) Token: 0x06004CD0 RID: 19664 RVA: 0x0017CCF8 File Offset: 0x0017AEF8
		// (set) Token: 0x06004CD1 RID: 19665 RVA: 0x0017CD00 File Offset: 0x0017AF00
		public Color myBraceletColor { get; private set; }

		// Token: 0x17000743 RID: 1859
		// (get) Token: 0x06004CD2 RID: 19666 RVA: 0x0017CD09 File Offset: 0x0017AF09
		// (set) Token: 0x06004CD3 RID: 19667 RVA: 0x0017CD11 File Offset: 0x0017AF11
		public int MyBraceletSelfIndex { get; private set; }

		// Token: 0x17000744 RID: 1860
		// (get) Token: 0x06004CD4 RID: 19668 RVA: 0x0017CD1A File Offset: 0x0017AF1A
		public List<string> PartyMemberIDs
		{
			get
			{
				return this.myPartyMemberIDs;
			}
		}

		// Token: 0x17000745 RID: 1861
		// (get) Token: 0x06004CD5 RID: 19669 RVA: 0x0017CD22 File Offset: 0x0017AF22
		public bool IsInParty
		{
			get
			{
				return this.myPartyMemberIDs != null;
			}
		}

		// Token: 0x17000746 RID: 1862
		// (get) Token: 0x06004CD6 RID: 19670 RVA: 0x0017CD2D File Offset: 0x0017AF2D
		// (set) Token: 0x06004CD7 RID: 19671 RVA: 0x0017CD35 File Offset: 0x0017AF35
		public GroupJoinZoneAB partyZone { get; private set; }

		// Token: 0x06004CD8 RID: 19672 RVA: 0x0017CD40 File Offset: 0x0017AF40
		private void Awake()
		{
			FriendshipGroupDetection.Instance = this;
			if (this.friendshipBubble)
			{
				this.particleSystem = this.friendshipBubble.GetComponent<ParticleSystem>();
				this.audioSource = this.friendshipBubble.GetComponent<AudioSource>();
			}
			NetworkSystem.Instance.OnPlayerJoined += this.OnPlayerJoinedRoom;
		}

		// Token: 0x06004CD9 RID: 19673 RVA: 0x0017CDA4 File Offset: 0x0017AFA4
		private void OnPlayerJoinedRoom(NetPlayer joiningPlayer)
		{
			if (!this.IsInParty)
			{
				return;
			}
			bool flag = (int)RoomSystem.GetRoomSize("") == NetworkSystem.Instance.RoomPlayerCount;
			Debug.Log(string.Concat(new string[]
			{
				"[FriendshipGroupDetection::OnPlayerJoinedRoom] JoiningPlayer: ",
				joiningPlayer.NickName,
				", ",
				joiningPlayer.UserId,
				" ",
				string.Format("| IsLocal: {0} | Room Full: {1}", joiningPlayer.IsLocal, flag)
			}));
			if (joiningPlayer.IsLocal)
			{
				this.lastJoinedRoomTime = (double)Time.time;
				if (!flag)
				{
					Debug.Log("[FriendshipGroupDetection::OnPlayerJoinedRoom] Delaying PartyRefresh...");
					this.wantsPartyRefreshPostJoin = true;
					return;
				}
			}
			if (flag)
			{
				this.RefreshPartyMembers();
			}
		}

		// Token: 0x06004CDA RID: 19674 RVA: 0x0017CE5B File Offset: 0x0017B05B
		public void AddGroupZoneCallback(Action<GroupJoinZoneAB> callback)
		{
			this.groupZoneCallbacks.Add(callback);
		}

		// Token: 0x06004CDB RID: 19675 RVA: 0x0017CE69 File Offset: 0x0017B069
		public void RemoveGroupZoneCallback(Action<GroupJoinZoneAB> callback)
		{
			this.groupZoneCallbacks.Remove(callback);
		}

		// Token: 0x06004CDC RID: 19676 RVA: 0x0017CE78 File Offset: 0x0017B078
		public bool IsInMyGroup(string userID)
		{
			return this.myPartyMemberIDs != null && this.myPartyMemberIDs.Contains(userID);
		}

		// Token: 0x06004CDD RID: 19677 RVA: 0x0017CE90 File Offset: 0x0017B090
		public bool AnyPartyMembersOutsideFriendCollider()
		{
			if (!this.IsInParty)
			{
				return false;
			}
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (vrrig.IsLocalPartyMember && !GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(vrrig.creator.UserId))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x17000747 RID: 1863
		// (get) Token: 0x06004CDE RID: 19678 RVA: 0x0017CF20 File Offset: 0x0017B120
		// (set) Token: 0x06004CDF RID: 19679 RVA: 0x0017CF28 File Offset: 0x0017B128
		public bool DidJoinLeftHanded { get; private set; }

		// Token: 0x06004CE0 RID: 19680 RVA: 0x0017CF34 File Offset: 0x0017B134
		private void Update()
		{
			if (this.wantsPartyRefreshPostJoin && this.lastJoinedRoomTime + this.joinedRoomRefreshPartyDelay < (double)Time.time)
			{
				this.RefreshPartyMembers();
			}
			if (this.wantsPartyRefreshPostFollowFailed && this.lastFailedToFollowPartyTime + this.failedToFollowRefreshPartyDelay < (double)Time.time)
			{
				this.RefreshPartyMembers();
			}
			List<int> list = this.playersInProvisionalGroup;
			List<int> list2 = this.playersInProvisionalGroup;
			List<int> list3 = this.tempIntList;
			this.tempIntList = list2;
			this.playersInProvisionalGroup = list3;
			Vector3 position;
			this.UpdateProvisionalGroup(out position);
			if (this.playersInProvisionalGroup.Count > 0)
			{
				this.friendshipBubble.transform.position = position;
			}
			bool flag = false;
			if (list.Count == this.playersInProvisionalGroup.Count)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] != this.playersInProvisionalGroup[i])
					{
						flag = true;
						break;
					}
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				this.groupCreateAfterTimestamp = Time.time + this.groupTime;
				this.amFirstProvisionalPlayer = (this.playersInProvisionalGroup.Count > 0 && this.playersInProvisionalGroup[0] == NetworkSystem.Instance.LocalPlayer.ActorNumber);
				if (this.playersInProvisionalGroup.Count > 0 && !this.amFirstProvisionalPlayer)
				{
					List<int> list4 = this.tempIntList;
					list4.Clear();
					NetPlayer netPlayer = null;
					foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
					{
						if (vrrig.creator.ActorNumber == this.playersInProvisionalGroup[0])
						{
							netPlayer = vrrig.creator;
							if (vrrig.IsLocalPartyMember)
							{
								list4.Clear();
								break;
							}
						}
						else if (vrrig.IsLocalPartyMember)
						{
							list4.Add(vrrig.creator.ActorNumber);
						}
					}
					if (list4.Count > 0)
					{
						this.photonView.RPC("NotifyPartyMerging", netPlayer.GetPlayerRef(), new object[]
						{
							list4.ToArray()
						});
					}
					else
					{
						this.photonView.RPC("NotifyNoPartyToMerge", netPlayer.GetPlayerRef(), Array.Empty<object>());
					}
				}
				if (this.playersInProvisionalGroup.Count == 0)
				{
					if (Time.time > this.suppressPartyCreationUntilTimestamp && this.playEffectsAfterTimestamp == 0f)
					{
						this.audioSource.GTStop();
						this.audioSource.GTPlayOneShot(this.fistBumpInterruptedAudio, 1f);
					}
					this.particleSystem.Stop();
					this.playEffectsAfterTimestamp = 0f;
				}
				else
				{
					this.playEffectsAfterTimestamp = Time.time + this.playEffectsDelay;
				}
			}
			else if (this.playEffectsAfterTimestamp > 0f && Time.time > this.playEffectsAfterTimestamp)
			{
				this.audioSource.time = 0f;
				this.audioSource.GTPlay();
				this.particleSystem.Play();
				this.playEffectsAfterTimestamp = 0f;
			}
			else if (this.playersInProvisionalGroup.Count > 0 && Time.time > this.groupCreateAfterTimestamp && this.amFirstProvisionalPlayer)
			{
				List<int> list5 = this.tempIntList;
				list5.Clear();
				list5.AddRange(this.playersInProvisionalGroup);
				int num = 0;
				if (this.IsInParty)
				{
					foreach (VRRig vrrig2 in GorillaParent.instance.vrrigs)
					{
						if (vrrig2.IsLocalPartyMember)
						{
							list5.Add(vrrig2.creator.ActorNumber);
							num++;
						}
					}
				}
				int num2 = 0;
				foreach (int key in this.playersInProvisionalGroup)
				{
					int[] collection;
					if (this.partyMergeIDs.TryGetValue(key, out collection))
					{
						list5.AddRange(collection);
						num2++;
					}
				}
				list5.Sort();
				int[] memberIDs = list5.Distinct<int>().ToArray<int>();
				this.myBraceletColor = GTColor.RandomHSV(this.braceletRandomColorHSVRanges);
				this.SendPartyFormedRPC(FriendshipGroupDetection.PackColor(this.myBraceletColor), memberIDs, false);
				this.groupCreateAfterTimestamp = Time.time + this.cooldownAfterCreatingGroup;
			}
			if (this.myPartyMemberIDs != null)
			{
				this.UpdateWarningSigns();
			}
		}

		// Token: 0x06004CE1 RID: 19681 RVA: 0x0017D3BC File Offset: 0x0017B5BC
		private void UpdateProvisionalGroup(out Vector3 midpoint)
		{
			this.playersInProvisionalGroup.Clear();
			bool willJoinLeftHanded;
			VRMap makingFist = VRRig.LocalRig.GetMakingFist(this.debug, out willJoinLeftHanded);
			if (makingFist == null || !NetworkSystem.Instance.InRoom || VRRig.LocalRig.leftHandLink.IsLinkActive() || VRRig.LocalRig.rightHandLink.IsLinkActive() || GorillaParent.instance.vrrigs.Count == 0 || Time.time < this.suppressPartyCreationUntilTimestamp || (GorillaGameModes.GameMode.ActiveGameMode != null && !GorillaGameModes.GameMode.ActiveGameMode.CanJoinFrienship(NetworkSystem.Instance.LocalPlayer)))
			{
				midpoint = Vector3.zero;
				return;
			}
			this.WillJoinLeftHanded = willJoinLeftHanded;
			this.playersToPropagateFrom.Clear();
			this.provisionalGroupUsingLeftHands.Clear();
			this.playersMakingFists.Clear();
			int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			int num = -1;
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				bool isLeftHand;
				VRMap makingFist2 = vrrig.GetMakingFist(this.debug, out isLeftHand);
				if (makingFist2 != null && !vrrig.leftHandLink.IsLinkActive() && !vrrig.rightHandLink.IsLinkActive() && (!(GorillaGameModes.GameMode.ActiveGameMode != null) || GorillaGameModes.GameMode.ActiveGameMode.CanJoinFrienship(vrrig.OwningNetPlayer)))
				{
					FriendshipGroupDetection.PlayerFist item = new FriendshipGroupDetection.PlayerFist
					{
						actorNumber = vrrig.creator.ActorNumber,
						position = makingFist2.rigTarget.position,
						isLeftHand = isLeftHand
					};
					if (vrrig.isOfflineVRRig)
					{
						num = this.playersMakingFists.Count;
					}
					this.playersMakingFists.Add(item);
				}
			}
			if (this.playersMakingFists.Count <= 1 || num == -1)
			{
				midpoint = Vector3.zero;
				return;
			}
			this.playersToPropagateFrom.Enqueue(this.playersMakingFists[num]);
			this.playersInProvisionalGroup.Add(actorNumber);
			midpoint = makingFist.rigTarget.position;
			int num2 = 1 << num;
			FriendshipGroupDetection.PlayerFist playerFist;
			while (this.playersToPropagateFrom.TryDequeue(out playerFist))
			{
				for (int i = 0; i < this.playersMakingFists.Count; i++)
				{
					if ((num2 & 1 << i) == 0)
					{
						FriendshipGroupDetection.PlayerFist playerFist2 = this.playersMakingFists[i];
						if ((playerFist.position - playerFist2.position).IsShorterThan(this.detectionRadius))
						{
							int index = ~this.playersInProvisionalGroup.BinarySearch(playerFist2.actorNumber);
							num2 |= 1 << i;
							this.playersInProvisionalGroup.Insert(index, playerFist2.actorNumber);
							if (playerFist2.isLeftHand)
							{
								this.provisionalGroupUsingLeftHands.Add(playerFist2.actorNumber);
							}
							this.playersToPropagateFrom.Enqueue(playerFist2);
							midpoint += playerFist2.position;
						}
					}
				}
			}
			if (this.playersInProvisionalGroup.Count == 1)
			{
				this.playersInProvisionalGroup.Clear();
			}
			if (this.playersInProvisionalGroup.Count > 0)
			{
				midpoint /= (float)this.playersInProvisionalGroup.Count;
			}
		}

		// Token: 0x06004CE2 RID: 19682 RVA: 0x0017D72C File Offset: 0x0017B92C
		private void UpdateWarningSigns()
		{
			ZoneEntity zoneEntity = GorillaTagger.Instance.offlineVRRig.zoneEntity;
			GTZone currentRoomZone = PhotonNetworkController.Instance.CurrentRoomZone;
			GroupJoinZoneAB groupJoinZoneAB = 0;
			if (this.myPartyMemberIDs != null)
			{
				foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
				{
					if (vrrig.IsLocalPartyMember && !vrrig.isOfflineVRRig)
					{
						groupJoinZoneAB |= vrrig.zoneEntity.GroupZone;
					}
				}
			}
			if (groupJoinZoneAB != this.partyZone)
			{
				this.debugStr.Clear();
				foreach (VRRig vrrig2 in GorillaParent.instance.vrrigs)
				{
					if (vrrig2.IsLocalPartyMember && !vrrig2.isOfflineVRRig)
					{
						this.debugStr.Append(string.Format("{0} in {1};", vrrig2.playerNameVisible, vrrig2.zoneEntity.GroupZone));
					}
				}
				this.partyZone = groupJoinZoneAB;
				foreach (Action<GroupJoinZoneAB> action in this.groupZoneCallbacks)
				{
					action(this.partyZone);
				}
			}
		}

		// Token: 0x06004CE3 RID: 19683 RVA: 0x0017D8B4 File Offset: 0x0017BAB4
		[PunRPC]
		private void NotifyNoPartyToMerge(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "NotifyNoPartyToMerge");
			if (info.Sender == null || this.partyMergeIDs == null)
			{
				return;
			}
			this.partyMergeIDs.Remove(info.Sender.ActorNumber);
		}

		// Token: 0x06004CE4 RID: 19684 RVA: 0x0017D8EC File Offset: 0x0017BAEC
		[Rpc]
		private unsafe static void RPC_NotifyNoPartyToMerge(NetworkRunner runner, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage == SimulationStages.Resimulate)
				{
					return;
				}
				if (runner.HasAnyActiveConnections())
				{
					int capacityInBytes = 8;
					SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, capacityInBytes);
					byte* data = SimulationMessage.GetData(ptr);
					int num = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyNoPartyToMerge(Fusion.NetworkRunner,Fusion.RpcInfo)")), data);
					ptr->Offset = num * 8;
					ptr->SetStatic();
					runner.SendRpc(ptr);
				}
				info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
			}
			FriendshipGroupDetection.Instance.partyMergeIDs.Remove(info.Source.PlayerId);
		}

		// Token: 0x06004CE5 RID: 19685 RVA: 0x0017D9CC File Offset: 0x0017BBCC
		[PunRPC]
		private void NotifyPartyMerging(int[] memberIDs, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "NotifyPartyMerging");
			if (memberIDs.Length > 10)
			{
				return;
			}
			this.partyMergeIDs[info.Sender.ActorNumber] = memberIDs;
		}

		// Token: 0x06004CE6 RID: 19686 RVA: 0x0017D9F8 File Offset: 0x0017BBF8
		[Rpc]
		private unsafe static void RPC_NotifyPartyMerging(NetworkRunner runner, [RpcTarget] PlayerRef playerRef, int[] memberIDs, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != SimulationStages.Resimulate)
				{
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(playerRef);
					if (rpcTargetStatus == RpcTargetStatus.Unreachable)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(playerRef, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyMerging(Fusion.NetworkRunner,Fusion.PlayerRef,System.Int32[],Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == RpcTargetStatus.Self)
						{
							info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_10;
						}
						int num = 8;
						num += (memberIDs.Length * 4 + 4 + 3 & -4);
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* data = SimulationMessage.GetData(ptr);
						int num2 = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyMerging(Fusion.NetworkRunner,Fusion.PlayerRef,System.Int32[],Fusion.RpcInfo)")), data);
						*(int*)(data + num2) = memberIDs.Length;
						num2 += 4;
						num2 = (Native.CopyFromArray<int>((void*)(data + num2), memberIDs) + 3 & -4) + num2;
						ptr->Offset = num2 * 8;
						ptr->SetTarget(playerRef);
						ptr->SetStatic();
						runner.SendRpc(ptr);
					}
				}
				return;
			}
			IL_10:
			if (memberIDs.Length > 10)
			{
				return;
			}
			FriendshipGroupDetection.Instance.partyMergeIDs[info.Source.PlayerId] = memberIDs;
		}

		// Token: 0x06004CE7 RID: 19687 RVA: 0x0017DB64 File Offset: 0x0017BD64
		public void SendAboutToGroupJoin()
		{
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				Debug.Log(string.Concat(new string[]
				{
					"Sending group join to ",
					GorillaParent.instance.vrrigs.Count.ToString(),
					" players. Party member:",
					vrrig.OwningNetPlayer.NickName,
					"Is offline rig",
					vrrig.isOfflineVRRig.ToString()
				}));
				if (vrrig.IsLocalPartyMember && !vrrig.isOfflineVRRig)
				{
					this.photonView.RPC("PartyMemberIsAboutToGroupJoin", vrrig.Creator.GetPlayerRef(), Array.Empty<object>());
				}
			}
		}

		// Token: 0x06004CE8 RID: 19688 RVA: 0x0017DC4C File Offset: 0x0017BE4C
		[PunRPC]
		private void PartyMemberIsAboutToGroupJoin(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PartyMemberIsAboutToGroupJoin");
			this.PartMemberIsAboutToGroupJoinWrapped(new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06004CE9 RID: 19689 RVA: 0x0017DC68 File Offset: 0x0017BE68
		[Rpc]
		private unsafe static void RPC_PartyMemberIsAboutToGroupJoin(NetworkRunner runner, [RpcTarget] PlayerRef targetPlayer, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != SimulationStages.Resimulate)
				{
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(targetPlayer);
					if (rpcTargetStatus == RpcTargetStatus.Unreachable)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(targetPlayer, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyMemberIsAboutToGroupJoin(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == RpcTargetStatus.Self)
						{
							info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_10;
						}
						int capacityInBytes = 8;
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, capacityInBytes);
						byte* data = SimulationMessage.GetData(ptr);
						int num = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyMemberIsAboutToGroupJoin(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)")), data);
						ptr->Offset = num * 8;
						ptr->SetTarget(targetPlayer);
						ptr->SetStatic();
						runner.SendRpc(ptr);
					}
				}
				return;
			}
			IL_10:
			FriendshipGroupDetection.Instance.PartMemberIsAboutToGroupJoinWrapped(new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06004CEA RID: 19690 RVA: 0x0017DD68 File Offset: 0x0017BF68
		private void PartMemberIsAboutToGroupJoinWrapped(PhotonMessageInfoWrapped wrappedInfo)
		{
			float time = Time.time;
			float num = this.aboutToGroupJoin_CooldownUntilTimestamp;
			if (wrappedInfo.senderID < NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.aboutToGroupJoin_CooldownUntilTimestamp = Time.time + 5f;
				if (this.myPartyMembersHash.Contains(wrappedInfo.Sender.UserId))
				{
					PhotonNetworkController.Instance.DeferJoining(2f);
				}
			}
		}

		// Token: 0x06004CEB RID: 19691 RVA: 0x0017DDD4 File Offset: 0x0017BFD4
		private void SendPartyFormedRPC(short braceletColor, int[] memberIDs, bool forceDebug)
		{
			string text = Enum.Parse<GameModeType>(GorillaComputer.instance.currentGameMode.Value, true).ToString();
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (this.playersInProvisionalGroup.BinarySearch(vrrig.creator.ActorNumber) >= 0)
				{
					this.photonView.RPC("PartyFormedSuccessfully", vrrig.Creator.GetPlayerRef(), new object[]
					{
						text,
						braceletColor,
						memberIDs,
						forceDebug
					});
				}
			}
		}

		// Token: 0x06004CEC RID: 19692 RVA: 0x0017DEA0 File Offset: 0x0017C0A0
		[Rpc]
		private unsafe static void RPC_PartyFormedSuccessfully(NetworkRunner runner, [RpcTarget] PlayerRef targetPlayer, string partyGameMode, short braceletColor, int[] memberIDs, bool forceDebug, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != SimulationStages.Resimulate)
				{
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(targetPlayer);
					if (rpcTargetStatus == RpcTargetStatus.Unreachable)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(targetPlayer, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyFormedSuccessfully(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],System.Boolean,Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == RpcTargetStatus.Self)
						{
							info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_10;
						}
						int num = 8;
						num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(partyGameMode) + 3 & -4);
						num += 4;
						num += (memberIDs.Length * 4 + 4 + 3 & -4);
						num += 4;
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* data = SimulationMessage.GetData(ptr);
						int num2 = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyFormedSuccessfully(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],System.Boolean,Fusion.RpcInfo)")), data);
						num2 = (ReadWriteUtilsForWeaver.WriteStringUtf8NoHash((void*)(data + num2), partyGameMode) + 3 & -4) + num2;
						*(short*)(data + num2) = braceletColor;
						num2 += (2 + 3 & -4);
						*(int*)(data + num2) = memberIDs.Length;
						num2 += 4;
						num2 = (Native.CopyFromArray<int>((void*)(data + num2), memberIDs) + 3 & -4) + num2;
						ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), forceDebug);
						num2 += 4;
						ptr->Offset = num2 * 8;
						ptr->SetTarget(targetPlayer);
						ptr->SetStatic();
						runner.SendRpc(ptr);
					}
				}
				return;
			}
			IL_10:
			GorillaNot.IncrementRPCCall(info, "PartyFormedSuccessfully");
			FriendshipGroupDetection.Instance.PartyFormedSuccesfullyWrapped(partyGameMode, braceletColor, memberIDs, forceDebug, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06004CED RID: 19693 RVA: 0x0017E09C File Offset: 0x0017C29C
		[PunRPC]
		private void PartyFormedSuccessfully(string partyGameMode, short braceletColor, int[] memberIDs, bool forceDebug, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PartyFormedSuccessfully");
			this.PartyFormedSuccesfullyWrapped(partyGameMode, braceletColor, memberIDs, forceDebug, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06004CEE RID: 19694 RVA: 0x0017E0BC File Offset: 0x0017C2BC
		private void PartyFormedSuccesfullyWrapped(string partyGameMode, short braceletColor, int[] memberIDs, bool forceDebug, PhotonMessageInfoWrapped info)
		{
			if (memberIDs == null || memberIDs.Length > 10 || !memberIDs.Contains(info.Sender.ActorNumber) || this.playersInProvisionalGroup.IndexOf(info.Sender.ActorNumber) != 0 || Mathf.Abs(this.groupCreateAfterTimestamp - Time.time) > this.m_maxGroupJoinTimeDifference || !GorillaGameModes.GameMode.IsValidGameMode(partyGameMode))
			{
				return;
			}
			if (this.IsInParty)
			{
				string text = Enum.Parse<GameModeType>(GorillaComputer.instance.currentGameMode.Value, true).ToString();
				foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
				{
					if (vrrig.IsLocalPartyMember && !vrrig.isOfflineVRRig)
					{
						this.photonView.RPC("AddPartyMembers", vrrig.Creator.GetPlayerRef(), new object[]
						{
							text,
							braceletColor,
							memberIDs
						});
					}
				}
			}
			this.suppressPartyCreationUntilTimestamp = Time.time + this.cooldownAfterCreatingGroup;
			this.DidJoinLeftHanded = this.WillJoinLeftHanded;
			this.SetNewParty(partyGameMode, braceletColor, memberIDs);
		}

		// Token: 0x06004CEF RID: 19695 RVA: 0x0017E204 File Offset: 0x0017C404
		[PunRPC]
		private void AddPartyMembers(string partyGameMode, short braceletColor, int[] memberIDs, PhotonMessageInfo info)
		{
			this.AddPartyMembersWrapped(partyGameMode, braceletColor, memberIDs, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06004CF0 RID: 19696 RVA: 0x0017E218 File Offset: 0x0017C418
		[Rpc]
		private unsafe static void RPC_AddPartyMembers(NetworkRunner runner, [RpcTarget] PlayerRef rpcTarget, string partyGameMode, short braceletColor, int[] memberIDs, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != SimulationStages.Resimulate)
				{
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(rpcTarget);
					if (rpcTargetStatus == RpcTargetStatus.Unreachable)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(rpcTarget, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_AddPartyMembers(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == RpcTargetStatus.Self)
						{
							info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_10;
						}
						int num = 8;
						num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(partyGameMode) + 3 & -4);
						num += 4;
						num += (memberIDs.Length * 4 + 4 + 3 & -4);
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* data = SimulationMessage.GetData(ptr);
						int num2 = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_AddPartyMembers(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],Fusion.RpcInfo)")), data);
						num2 = (ReadWriteUtilsForWeaver.WriteStringUtf8NoHash((void*)(data + num2), partyGameMode) + 3 & -4) + num2;
						*(short*)(data + num2) = braceletColor;
						num2 += (2 + 3 & -4);
						*(int*)(data + num2) = memberIDs.Length;
						num2 += 4;
						num2 = (Native.CopyFromArray<int>((void*)(data + num2), memberIDs) + 3 & -4) + num2;
						ptr->Offset = num2 * 8;
						ptr->SetTarget(rpcTarget);
						ptr->SetStatic();
						runner.SendRpc(ptr);
					}
				}
				return;
			}
			IL_10:
			FriendshipGroupDetection.Instance.AddPartyMembersWrapped(partyGameMode, braceletColor, memberIDs, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06004CF1 RID: 19697 RVA: 0x0017E3DC File Offset: 0x0017C5DC
		private void AddPartyMembersWrapped(string partyGameMode, short braceletColor, int[] memberIDs, PhotonMessageInfoWrapped infoWrapped)
		{
			GorillaNot.IncrementRPCCall(infoWrapped, "AddPartyMembersWrapped");
			if (memberIDs.Length > 10 || !this.IsInParty || !this.myPartyMembersHash.Contains(NetworkSystem.Instance.GetUserID(infoWrapped.senderID)) || !GorillaGameModes.GameMode.IsValidGameMode(partyGameMode))
			{
				return;
			}
			Debug.Log("Adding party members: [" + string.Join<int>(",", memberIDs) + "]");
			this.SetNewParty(partyGameMode, braceletColor, memberIDs);
		}

		// Token: 0x06004CF2 RID: 19698 RVA: 0x0017E454 File Offset: 0x0017C654
		private void SetNewParty(string partyGameMode, short braceletColor, int[] memberIDs)
		{
			GorillaComputer.instance.SetGameModeWithoutButton(partyGameMode);
			this.myPartyMemberIDs = new List<string>();
			FriendshipGroupDetection.userIdLookup.Clear();
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				FriendshipGroupDetection.userIdLookup.Add(vrrig.creator.ActorNumber, vrrig.creator.UserId);
			}
			foreach (int key in memberIDs)
			{
				string item;
				if (FriendshipGroupDetection.userIdLookup.TryGetValue(key, out item))
				{
					this.myPartyMemberIDs.Add(item);
				}
			}
			this.myBraceletColor = FriendshipGroupDetection.UnpackColor(braceletColor);
			GorillaTagger.Instance.StartVibration(this.DidJoinLeftHanded, this.hapticStrength, this.hapticDuration);
			this.OnPartyMembershipChanged();
			PlayerGameEvents.MiscEvent("FriendshipGroupJoined", 1);
		}

		// Token: 0x06004CF3 RID: 19699 RVA: 0x0017E554 File Offset: 0x0017C754
		public void LeaveParty()
		{
			if (this.myPartyMemberIDs == null)
			{
				return;
			}
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (vrrig.IsLocalPartyMember && !vrrig.isOfflineVRRig)
				{
					this.photonView.RPC("PlayerLeftParty", vrrig.Creator.GetPlayerRef(), Array.Empty<object>());
				}
			}
			this.myPartyMemberIDs = null;
			this.OnPartyMembershipChanged();
			PhotonNetworkController.Instance.ClearDeferredJoin();
			GorillaTagger.Instance.StartVibration(false, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06004CF4 RID: 19700 RVA: 0x0017E610 File Offset: 0x0017C810
		public void OnFailedToFollowParty()
		{
			if (!this.IsInParty)
			{
				return;
			}
			this.lastFailedToFollowPartyTime = (double)Time.time;
			this.wantsPartyRefreshPostFollowFailed = true;
		}

		// Token: 0x06004CF5 RID: 19701 RVA: 0x0017E630 File Offset: 0x0017C830
		public void RefreshPartyMembers()
		{
			if (this.myPartyMemberIDs.IsNullOrEmpty<string>())
			{
				return;
			}
			Debug.Log("[FriendshipGroupDetection::RefreshPartyMembers] refreshing...");
			List<string> list = new List<string>(this.myPartyMemberIDs);
			Debug.Log("[FriendshipGroupDetection::RefreshPartyMembers] found " + string.Format("{0} current players in Room...", NetworkSystem.Instance.AllNetPlayers.Length));
			for (int i = 0; i < NetworkSystem.Instance.AllNetPlayers.Length; i++)
			{
				if (NetworkSystem.Instance.AllNetPlayers[i] != null)
				{
					list.Remove(NetworkSystem.Instance.AllNetPlayers[i].UserId);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				Debug.Log("[FriendshipGroupDetection::RefreshPartyMembers] removing missing player " + list[j] + " from party...");
				this.PlayerIDLeftParty(list[j]);
			}
			this.wantsPartyRefreshPostJoin = false;
			this.wantsPartyRefreshPostFollowFailed = false;
		}

		// Token: 0x06004CF6 RID: 19702 RVA: 0x0017E710 File Offset: 0x0017C910
		[Rpc]
		private unsafe static void RPC_PlayerLeftParty(NetworkRunner runner, [RpcTarget] PlayerRef player, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != SimulationStages.Resimulate)
				{
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(player);
					if (rpcTargetStatus == RpcTargetStatus.Unreachable)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(player, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PlayerLeftParty(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == RpcTargetStatus.Self)
						{
							info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_10;
						}
						int capacityInBytes = 8;
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, capacityInBytes);
						byte* data = SimulationMessage.GetData(ptr);
						int num = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PlayerLeftParty(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)")), data);
						ptr->Offset = num * 8;
						ptr->SetTarget(player);
						ptr->SetStatic();
						runner.SendRpc(ptr);
					}
				}
				return;
			}
			IL_10:
			GorillaNot.IncrementRPCCall(info, "PlayerLeftParty");
			FriendshipGroupDetection.Instance.PlayerLeftPartyWrapped(new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06004CF7 RID: 19703 RVA: 0x0017E81F File Offset: 0x0017CA1F
		[PunRPC]
		private void PlayerLeftParty(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerLeftParty");
			this.PlayerLeftPartyWrapped(new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06004CF8 RID: 19704 RVA: 0x0017E838 File Offset: 0x0017CA38
		private void PlayerLeftPartyWrapped(PhotonMessageInfoWrapped infoWrapped)
		{
			if (this.myPartyMemberIDs == null)
			{
				return;
			}
			if (!this.myPartyMemberIDs.Remove(infoWrapped.Sender.UserId))
			{
				return;
			}
			if (this.myPartyMemberIDs.Count <= 1)
			{
				this.myPartyMemberIDs = null;
			}
			this.OnPartyMembershipChanged();
			GorillaTagger.Instance.StartVibration(this.DidJoinLeftHanded, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06004CF9 RID: 19705 RVA: 0x0017E8A0 File Offset: 0x0017CAA0
		private void PlayerIDLeftParty(string userID)
		{
			if (this.myPartyMemberIDs == null)
			{
				return;
			}
			if (!this.myPartyMemberIDs.Remove(userID))
			{
				return;
			}
			if (this.myPartyMemberIDs.Count <= 1)
			{
				this.myPartyMemberIDs = null;
			}
			this.OnPartyMembershipChanged();
			GorillaTagger.Instance.StartVibration(this.DidJoinLeftHanded, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06004CFA RID: 19706 RVA: 0x0017E8FC File Offset: 0x0017CAFC
		public void SendVerifyPartyMember(NetPlayer player)
		{
			this.photonView.RPC("VerifyPartyMember", player.GetPlayerRef(), Array.Empty<object>());
		}

		// Token: 0x06004CFB RID: 19707 RVA: 0x0017E919 File Offset: 0x0017CB19
		[PunRPC]
		private void VerifyPartyMember(PhotonMessageInfo info)
		{
			this.VerifyPartyMemberWrapped(new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06004CFC RID: 19708 RVA: 0x0017E928 File Offset: 0x0017CB28
		[Rpc]
		private unsafe static void RPC_VerifyPartyMember(NetworkRunner runner, [RpcTarget] PlayerRef rpcTarget, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != SimulationStages.Resimulate)
				{
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(rpcTarget);
					if (rpcTargetStatus == RpcTargetStatus.Unreachable)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(rpcTarget, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_VerifyPartyMember(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == RpcTargetStatus.Self)
						{
							info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_10;
						}
						int capacityInBytes = 8;
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, capacityInBytes);
						byte* data = SimulationMessage.GetData(ptr);
						int num = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_VerifyPartyMember(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)")), data);
						ptr->Offset = num * 8;
						ptr->SetTarget(rpcTarget);
						ptr->SetStatic();
						runner.SendRpc(ptr);
					}
				}
				return;
			}
			IL_10:
			FriendshipGroupDetection.Instance.VerifyPartyMemberWrapped(new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06004CFD RID: 19709 RVA: 0x0017EA28 File Offset: 0x0017CC28
		private void VerifyPartyMemberWrapped(PhotonMessageInfoWrapped infoWrapped)
		{
			GorillaNot.IncrementRPCCall(infoWrapped, "VerifyPartyMemberWrapped");
			RigContainer rigContainer;
			if (!VRRigCache.Instance.TryGetVrrig(infoWrapped.Sender, out rigContainer) || !FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 15, infoWrapped.SentServerTime))
			{
				return;
			}
			if (this.myPartyMemberIDs == null || !this.myPartyMemberIDs.Contains(NetworkSystem.Instance.GetUserID(infoWrapped.senderID)))
			{
				this.photonView.RPC("PlayerLeftParty", infoWrapped.Sender.GetPlayerRef(), Array.Empty<object>());
			}
		}

		// Token: 0x06004CFE RID: 19710 RVA: 0x0017EAB8 File Offset: 0x0017CCB8
		public void SendRequestPartyGameMode(string gameMode)
		{
			int num = int.MaxValue;
			NetPlayer netPlayer = null;
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (vrrig.IsLocalPartyMember && vrrig.creator.ActorNumber < num)
				{
					netPlayer = vrrig.creator;
					num = vrrig.creator.ActorNumber;
				}
			}
			if (netPlayer != null)
			{
				this.photonView.RPC("RequestPartyGameMode", netPlayer.GetPlayerRef(), new object[]
				{
					gameMode
				});
			}
		}

		// Token: 0x06004CFF RID: 19711 RVA: 0x0017EB60 File Offset: 0x0017CD60
		[Rpc]
		private unsafe static void RPC_RequestPartyGameMode(NetworkRunner runner, [RpcTarget] PlayerRef targetPlayer, string gameMode, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != SimulationStages.Resimulate)
				{
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(targetPlayer);
					if (rpcTargetStatus == RpcTargetStatus.Unreachable)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(targetPlayer, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_RequestPartyGameMode(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == RpcTargetStatus.Self)
						{
							info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_10;
						}
						int num = 8;
						num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(gameMode) + 3 & -4);
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* data = SimulationMessage.GetData(ptr);
						int num2 = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_RequestPartyGameMode(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)")), data);
						num2 = (ReadWriteUtilsForWeaver.WriteStringUtf8NoHash((void*)(data + num2), gameMode) + 3 & -4) + num2;
						ptr->Offset = num2 * 8;
						ptr->SetTarget(targetPlayer);
						ptr->SetStatic();
						runner.SendRpc(ptr);
					}
				}
				return;
			}
			IL_10:
			FriendshipGroupDetection.Instance.RequestPartyGameModeWrapped(gameMode, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06004D00 RID: 19712 RVA: 0x0017EC9D File Offset: 0x0017CE9D
		[PunRPC]
		private void RequestPartyGameMode(string gameMode, PhotonMessageInfo info)
		{
			this.RequestPartyGameModeWrapped(gameMode, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06004D01 RID: 19713 RVA: 0x0017ECAC File Offset: 0x0017CEAC
		private void RequestPartyGameModeWrapped(string gameMode, PhotonMessageInfoWrapped info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestPartyGameModeWrapped");
			if (!this.IsInParty || !this.IsInMyGroup(info.Sender.UserId) || !GorillaGameModes.GameMode.IsValidGameMode(gameMode))
			{
				return;
			}
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (vrrig.IsLocalPartyMember)
				{
					this.photonView.RPC("NotifyPartyGameModeChanged", vrrig.creator.GetPlayerRef(), new object[]
					{
						gameMode
					});
				}
			}
		}

		// Token: 0x06004D02 RID: 19714 RVA: 0x0017ED5C File Offset: 0x0017CF5C
		[Rpc]
		private unsafe static void RPC_NotifyPartyGameModeChanged(NetworkRunner runner, [RpcTarget] PlayerRef targetPlayer, string gameMode, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != SimulationStages.Resimulate)
				{
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(targetPlayer);
					if (rpcTargetStatus == RpcTargetStatus.Unreachable)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(targetPlayer, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyGameModeChanged(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == RpcTargetStatus.Self)
						{
							info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_10;
						}
						int num = 8;
						num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(gameMode) + 3 & -4);
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* data = SimulationMessage.GetData(ptr);
						int num2 = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyGameModeChanged(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)")), data);
						num2 = (ReadWriteUtilsForWeaver.WriteStringUtf8NoHash((void*)(data + num2), gameMode) + 3 & -4) + num2;
						ptr->Offset = num2 * 8;
						ptr->SetTarget(targetPlayer);
						ptr->SetStatic();
						runner.SendRpc(ptr);
					}
				}
				return;
			}
			IL_10:
			FriendshipGroupDetection.Instance.NotifyPartyGameModeChangedWrapped(gameMode, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06004D03 RID: 19715 RVA: 0x0017EE99 File Offset: 0x0017D099
		[PunRPC]
		private void NotifyPartyGameModeChanged(string gameMode, PhotonMessageInfo info)
		{
			this.NotifyPartyGameModeChangedWrapped(gameMode, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06004D04 RID: 19716 RVA: 0x0017EEA8 File Offset: 0x0017D0A8
		private void NotifyPartyGameModeChangedWrapped(string gameMode, PhotonMessageInfoWrapped info)
		{
			GorillaNot.IncrementRPCCall(info, "NotifyPartyGameModeChangedWrapped");
			if (!this.IsInParty || !this.IsInMyGroup(info.Sender.UserId) || !GorillaGameModes.GameMode.IsValidGameMode(gameMode))
			{
				return;
			}
			GorillaComputer.instance.SetGameModeWithoutButton(gameMode);
		}

		// Token: 0x06004D05 RID: 19717 RVA: 0x0017EEE8 File Offset: 0x0017D0E8
		private void OnPartyMembershipChanged()
		{
			this.myPartyMembersHash.Clear();
			if (this.myPartyMemberIDs != null)
			{
				foreach (string item in this.myPartyMemberIDs)
				{
					this.myPartyMembersHash.Add(item);
				}
			}
			this.myBeadColors.Clear();
			FriendshipGroupDetection.tempColorLookup.Clear();
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				vrrig.ClearPartyMemberStatus();
				if (vrrig.IsLocalPartyMember)
				{
					FriendshipGroupDetection.tempColorLookup.Add(vrrig.Creator.UserId, vrrig.playerColor);
				}
			}
			this.MyBraceletSelfIndex = 0;
			if (this.myPartyMemberIDs != null)
			{
				using (List<string>.Enumerator enumerator = this.myPartyMemberIDs.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string text = enumerator.Current;
						Color item2;
						if (FriendshipGroupDetection.tempColorLookup.TryGetValue(text, out item2))
						{
							if (text == PhotonNetwork.LocalPlayer.UserId)
							{
								this.MyBraceletSelfIndex = this.myBeadColors.Count;
							}
							this.myBeadColors.Add(item2);
						}
					}
					goto IL_168;
				}
			}
			GorillaComputer.instance.SetGameModeWithoutButton(GorillaComputer.instance.lastPressedGameMode);
			this.wantsPartyRefreshPostJoin = false;
			this.wantsPartyRefreshPostFollowFailed = false;
			IL_168:
			this.myBeadColors.Add(this.myBraceletColor);
			GorillaTagger.Instance.offlineVRRig.UpdateFriendshipBracelet();
			this.UpdateWarningSigns();
		}

		// Token: 0x06004D06 RID: 19718 RVA: 0x0017F0AC File Offset: 0x0017D2AC
		public bool IsPartyWithinCollider(GorillaFriendCollider friendCollider)
		{
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (vrrig.IsLocalPartyMember && !vrrig.isOfflineVRRig && !friendCollider.playerIDsCurrentlyTouching.Contains(vrrig.Creator.UserId))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06004D07 RID: 19719 RVA: 0x0017F130 File Offset: 0x0017D330
		public static short PackColor(Color col)
		{
			return (short)(Mathf.RoundToInt(col.r * 9f) + Mathf.RoundToInt(col.g * 9f) * 10 + Mathf.RoundToInt(col.b * 9f) * 100);
		}

		// Token: 0x06004D08 RID: 19720 RVA: 0x0017F170 File Offset: 0x0017D370
		public static Color UnpackColor(short data)
		{
			return new Color
			{
				r = (float)(data % 10) / 9f,
				g = (float)(data / 10 % 10) / 9f,
				b = (float)(data / 100 % 10) / 9f
			};
		}

		// Token: 0x06004D0B RID: 19723 RVA: 0x0017F2CC File Offset: 0x0017D4CC
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyNoPartyToMerge(Fusion.NetworkRunner,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_NotifyNoPartyToMerge@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = RpcHeader.ReadSize(data) + 3 & -4;
			RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_NotifyNoPartyToMerge(runner, info);
		}

		// Token: 0x06004D0C RID: 19724 RVA: 0x0017F31C File Offset: 0x0017D51C
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyMerging(Fusion.NetworkRunner,Fusion.PlayerRef,System.Int32[],Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_NotifyPartyMerging@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = RpcHeader.ReadSize(data) + 3 & -4;
			PlayerRef target = message->Target;
			int[] array = new int[*(int*)(data + num)];
			num += 4;
			num = (Native.CopyToArray<int>(array, (void*)(data + num)) + 3 & -4) + num;
			RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_NotifyPartyMerging(runner, target, array, info);
		}

		// Token: 0x06004D0D RID: 19725 RVA: 0x0017F3C0 File Offset: 0x0017D5C0
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyMemberIsAboutToGroupJoin(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_PartyMemberIsAboutToGroupJoin@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = RpcHeader.ReadSize(data) + 3 & -4;
			PlayerRef target = message->Target;
			RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_PartyMemberIsAboutToGroupJoin(runner, target, info);
		}

		// Token: 0x06004D0E RID: 19726 RVA: 0x0017F420 File Offset: 0x0017D620
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyFormedSuccessfully(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],System.Boolean,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_PartyFormedSuccessfully@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = RpcHeader.ReadSize(data) + 3 & -4;
			PlayerRef target = message->Target;
			string partyGameMode;
			num = (ReadWriteUtilsForWeaver.ReadStringUtf8NoHash((void*)(data + num), out partyGameMode) + 3 & -4) + num;
			short num2 = *(short*)(data + num);
			num += (2 + 3 & -4);
			short braceletColor = num2;
			int[] array = new int[*(int*)(data + num)];
			num += 4;
			num = (Native.CopyToArray<int>(array, (void*)(data + num)) + 3 & -4) + num;
			bool flag = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
			num += 4;
			bool forceDebug = flag;
			RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_PartyFormedSuccessfully(runner, target, partyGameMode, braceletColor, array, forceDebug, info);
		}

		// Token: 0x06004D0F RID: 19727 RVA: 0x0017F530 File Offset: 0x0017D730
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_AddPartyMembers(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_AddPartyMembers@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = RpcHeader.ReadSize(data) + 3 & -4;
			PlayerRef target = message->Target;
			string partyGameMode;
			num = (ReadWriteUtilsForWeaver.ReadStringUtf8NoHash((void*)(data + num), out partyGameMode) + 3 & -4) + num;
			short num2 = *(short*)(data + num);
			num += (2 + 3 & -4);
			short braceletColor = num2;
			int[] array = new int[*(int*)(data + num)];
			num += 4;
			num = (Native.CopyToArray<int>(array, (void*)(data + num)) + 3 & -4) + num;
			RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_AddPartyMembers(runner, target, partyGameMode, braceletColor, array, info);
		}

		// Token: 0x06004D10 RID: 19728 RVA: 0x0017F620 File Offset: 0x0017D820
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PlayerLeftParty(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_PlayerLeftParty@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = RpcHeader.ReadSize(data) + 3 & -4;
			PlayerRef target = message->Target;
			RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_PlayerLeftParty(runner, target, info);
		}

		// Token: 0x06004D11 RID: 19729 RVA: 0x0017F680 File Offset: 0x0017D880
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_VerifyPartyMember(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_VerifyPartyMember@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = RpcHeader.ReadSize(data) + 3 & -4;
			PlayerRef target = message->Target;
			RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_VerifyPartyMember(runner, target, info);
		}

		// Token: 0x06004D12 RID: 19730 RVA: 0x0017F6E0 File Offset: 0x0017D8E0
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_RequestPartyGameMode(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_RequestPartyGameMode@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = RpcHeader.ReadSize(data) + 3 & -4;
			PlayerRef target = message->Target;
			string gameMode;
			num = (ReadWriteUtilsForWeaver.ReadStringUtf8NoHash((void*)(data + num), out gameMode) + 3 & -4) + num;
			RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_RequestPartyGameMode(runner, target, gameMode, info);
		}

		// Token: 0x06004D13 RID: 19731 RVA: 0x0017F768 File Offset: 0x0017D968
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyGameModeChanged(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_NotifyPartyGameModeChanged@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* data = SimulationMessage.GetData(message);
			int num = RpcHeader.ReadSize(data) + 3 & -4;
			PlayerRef target = message->Target;
			string gameMode;
			num = (ReadWriteUtilsForWeaver.ReadStringUtf8NoHash((void*)(data + num), out gameMode) + 3 & -4) + num;
			RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_NotifyPartyGameModeChanged(runner, target, gameMode, info);
		}

		// Token: 0x040055DF RID: 21983
		[SerializeField]
		private float detectionRadius = 0.5f;

		// Token: 0x040055E0 RID: 21984
		[SerializeField]
		private float groupTime = 5f;

		// Token: 0x040055E1 RID: 21985
		[SerializeField]
		private float cooldownAfterCreatingGroup = 5f;

		// Token: 0x040055E2 RID: 21986
		[SerializeField]
		private float hapticStrength = 1.5f;

		// Token: 0x040055E3 RID: 21987
		[SerializeField]
		private float hapticDuration = 2f;

		// Token: 0x040055E4 RID: 21988
		[SerializeField]
		private double joinedRoomRefreshPartyDelay = 30.0;

		// Token: 0x040055E5 RID: 21989
		[SerializeField]
		private double failedToFollowRefreshPartyDelay = 30.0;

		// Token: 0x040055E6 RID: 21990
		public bool debug;

		// Token: 0x040055E7 RID: 21991
		public double offset = 0.5;

		// Token: 0x040055E8 RID: 21992
		[SerializeField]
		private float m_maxGroupJoinTimeDifference = 1f;

		// Token: 0x040055E9 RID: 21993
		private List<string> myPartyMemberIDs;

		// Token: 0x040055EA RID: 21994
		private HashSet<string> myPartyMembersHash = new HashSet<string>();

		// Token: 0x040055EF RID: 21999
		private List<Action<GroupJoinZoneAB>> groupZoneCallbacks = new List<Action<GroupJoinZoneAB>>();

		// Token: 0x040055F0 RID: 22000
		[SerializeField]
		private GTColor.HSVRanges braceletRandomColorHSVRanges;

		// Token: 0x040055F1 RID: 22001
		public GameObject friendshipBubble;

		// Token: 0x040055F2 RID: 22002
		public AudioClip fistBumpInterruptedAudio;

		// Token: 0x040055F3 RID: 22003
		private ParticleSystem particleSystem;

		// Token: 0x040055F4 RID: 22004
		private AudioSource audioSource;

		// Token: 0x040055F5 RID: 22005
		private double lastJoinedRoomTime;

		// Token: 0x040055F6 RID: 22006
		private bool wantsPartyRefreshPostJoin;

		// Token: 0x040055F7 RID: 22007
		private double lastFailedToFollowPartyTime;

		// Token: 0x040055F8 RID: 22008
		private bool wantsPartyRefreshPostFollowFailed;

		// Token: 0x040055F9 RID: 22009
		private Queue<FriendshipGroupDetection.PlayerFist> playersToPropagateFrom = new Queue<FriendshipGroupDetection.PlayerFist>();

		// Token: 0x040055FA RID: 22010
		private List<int> playersInProvisionalGroup = new List<int>();

		// Token: 0x040055FB RID: 22011
		private List<int> provisionalGroupUsingLeftHands = new List<int>();

		// Token: 0x040055FC RID: 22012
		private List<int> tempIntList = new List<int>();

		// Token: 0x040055FD RID: 22013
		private bool amFirstProvisionalPlayer;

		// Token: 0x040055FE RID: 22014
		private Dictionary<int, int[]> partyMergeIDs = new Dictionary<int, int[]>();

		// Token: 0x040055FF RID: 22015
		private float groupCreateAfterTimestamp;

		// Token: 0x04005600 RID: 22016
		private float playEffectsAfterTimestamp;

		// Token: 0x04005601 RID: 22017
		[SerializeField]
		private float playEffectsDelay;

		// Token: 0x04005602 RID: 22018
		private float suppressPartyCreationUntilTimestamp;

		// Token: 0x04005604 RID: 22020
		private bool WillJoinLeftHanded;

		// Token: 0x04005605 RID: 22021
		private List<FriendshipGroupDetection.PlayerFist> playersMakingFists = new List<FriendshipGroupDetection.PlayerFist>();

		// Token: 0x04005606 RID: 22022
		private StringBuilder debugStr = new StringBuilder();

		// Token: 0x04005607 RID: 22023
		private float aboutToGroupJoin_CooldownUntilTimestamp;

		// Token: 0x04005608 RID: 22024
		private static Dictionary<int, string> userIdLookup = new Dictionary<int, string>();

		// Token: 0x04005609 RID: 22025
		private static Dictionary<string, Color> tempColorLookup = new Dictionary<string, Color>();

		// Token: 0x02000C31 RID: 3121
		private struct PlayerFist
		{
			// Token: 0x0400560A RID: 22026
			public int actorNumber;

			// Token: 0x0400560B RID: 22027
			public Vector3 position;

			// Token: 0x0400560C RID: 22028
			public bool isLeftHand;
		}
	}
}
