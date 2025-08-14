using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020009F0 RID: 2544
public class FriendingManager : MonoBehaviourPun, IPunObservable, IGorillaSliceableSimple
{
	// Token: 0x06003E30 RID: 15920 RVA: 0x0013BB85 File Offset: 0x00139D85
	private void Awake()
	{
		if (FriendingManager.Instance == null)
		{
			FriendingManager.Instance = this;
			PhotonNetwork.AddCallbackTarget(this);
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06003E31 RID: 15921 RVA: 0x0013BBB0 File Offset: 0x00139DB0
	private void Start()
	{
		NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerLeftRoom;
		NetworkSystem.Instance.OnMultiplayerStarted += this.ValidateState;
		NetworkSystem.Instance.OnReturnedToSinglePlayer += this.ValidateState;
	}

	// Token: 0x06003E32 RID: 15922 RVA: 0x0013BC20 File Offset: 0x00139E20
	private void OnDestroy()
	{
		if (NetworkSystem.Instance != null)
		{
			NetworkSystem.Instance.OnPlayerLeft -= this.OnPlayerLeftRoom;
			NetworkSystem.Instance.OnMultiplayerStarted -= this.ValidateState;
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.ValidateState;
		}
	}

	// Token: 0x06003E33 RID: 15923 RVA: 0x000172AD File Offset: 0x000154AD
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06003E34 RID: 15924 RVA: 0x000172B6 File Offset: 0x000154B6
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06003E35 RID: 15925 RVA: 0x0013BC9D File Offset: 0x00139E9D
	public void SliceUpdate()
	{
		this.AuthorityUpdate();
	}

	// Token: 0x06003E36 RID: 15926 RVA: 0x0013BCA8 File Offset: 0x00139EA8
	private void AuthorityUpdate()
	{
		if (PhotonNetwork.InRoom && base.photonView.IsMine)
		{
			for (int i = 0; i < this.activeFriendStationData.Count; i++)
			{
				if (this.activeFriendStationData[i].state >= FriendingManager.FriendStationState.ButtonConfirmationTimer0 && this.activeFriendStationData[i].state <= FriendingManager.FriendStationState.ButtonConfirmationTimer4)
				{
					FriendingManager.FriendStationData friendStationData = this.activeFriendStationData[i];
					int num = 4;
					float num2 = (Time.time - friendStationData.progressBarStartTime) / this.progressBarDuration;
					if (num2 < 1f)
					{
						int num3 = Mathf.RoundToInt(num2 * (float)num);
						friendStationData.state = num3 + FriendingManager.FriendStationState.ButtonConfirmationTimer0;
					}
					else
					{
						base.photonView.RPC("NotifyClientsFriendRequestReadyRPC", RpcTarget.All, new object[]
						{
							friendStationData.zone
						});
						friendStationData.state = FriendingManager.FriendStationState.WaitingOnRequestBoth;
					}
					this.activeFriendStationData[i] = friendStationData;
				}
			}
		}
	}

	// Token: 0x06003E37 RID: 15927 RVA: 0x0013BD95 File Offset: 0x00139F95
	private void OnPlayerLeftRoom(NetPlayer player)
	{
		this.ValidateState();
	}

	// Token: 0x06003E38 RID: 15928 RVA: 0x0013BDA0 File Offset: 0x00139FA0
	private void ValidateState()
	{
		for (int i = 0; i < this.activeFriendStationData.Count; i++)
		{
			FriendingManager.FriendStationData friendStationData = this.activeFriendStationData[i];
			if (friendStationData.actorNumberA != -1 && NetworkSystem.Instance.GetNetPlayerByID(friendStationData.actorNumberA) == null)
			{
				friendStationData.actorNumberA = -1;
			}
			if (friendStationData.actorNumberB != -1 && NetworkSystem.Instance.GetNetPlayerByID(friendStationData.actorNumberB) == null)
			{
				friendStationData.actorNumberB = -1;
			}
			if (friendStationData.actorNumberA == -1 || friendStationData.actorNumberB == -1)
			{
				friendStationData.state = FriendingManager.FriendStationState.WaitingForPlayers;
			}
			this.activeFriendStationData[i] = friendStationData;
		}
		this.UpdateFriendingStations();
	}

	// Token: 0x06003E39 RID: 15929 RVA: 0x0013BE48 File Offset: 0x0013A048
	private void UpdateFriendingStations()
	{
		for (int i = 0; i < this.activeFriendStationData.Count; i++)
		{
			FriendingStation friendingStation;
			if (this.friendingStations.TryGetValue(this.activeFriendStationData[i].zone, out friendingStation))
			{
				friendingStation.UpdateState(this.activeFriendStationData[i]);
			}
		}
	}

	// Token: 0x06003E3A RID: 15930 RVA: 0x0013BE9D File Offset: 0x0013A09D
	public void RegisterFriendingStation(FriendingStation friendingStation)
	{
		if (!this.friendingStations.ContainsKey(friendingStation.Zone))
		{
			this.friendingStations.Add(friendingStation.Zone, friendingStation);
		}
	}

	// Token: 0x06003E3B RID: 15931 RVA: 0x0013BEC4 File Offset: 0x0013A0C4
	public void UnregisterFriendingStation(FriendingStation friendingStation)
	{
		this.friendingStations.Remove(friendingStation.Zone);
	}

	// Token: 0x06003E3C RID: 15932 RVA: 0x0013BED8 File Offset: 0x0013A0D8
	private void DebugLogFriendingStations()
	{
		string text = string.Format("Friending Stations: Count: {0} ", this.friendingStations.Count);
		foreach (KeyValuePair<GTZone, FriendingStation> keyValuePair in this.friendingStations)
		{
			text += string.Format("Station Zone {0}", keyValuePair.Key);
		}
		Debug.Log(text);
	}

	// Token: 0x06003E3D RID: 15933 RVA: 0x0013BF64 File Offset: 0x0013A164
	public void PlayerEnteredStation(GTZone zone, NetPlayer netPlayer)
	{
		if (netPlayer != null && netPlayer.ActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.localPlayerZone = zone;
		}
		if (PhotonNetwork.InRoom && base.photonView.IsMine)
		{
			int num = -1;
			int i = 0;
			while (i < this.activeFriendStationData.Count)
			{
				if (this.activeFriendStationData[i].zone == zone)
				{
					num = i;
					if (this.activeFriendStationData[i].actorNumberA == -1 && this.activeFriendStationData[i].actorNumberB != netPlayer.ActorNumber)
					{
						FriendingManager.FriendStationData friendStationData = this.activeFriendStationData[i];
						friendStationData.actorNumberA = netPlayer.ActorNumber;
						if (friendStationData.actorNumberA != -1 && friendStationData.actorNumberB != -1)
						{
							friendStationData.state = FriendingManager.FriendStationState.WaitingOnFriendStatusBoth;
						}
						else
						{
							friendStationData.state = FriendingManager.FriendStationState.WaitingForPlayers;
						}
						this.activeFriendStationData[i] = friendStationData;
					}
					else if (this.activeFriendStationData[i].actorNumberA != -1 && this.activeFriendStationData[i].actorNumberA != netPlayer.ActorNumber && this.activeFriendStationData[i].actorNumberB == -1)
					{
						FriendingManager.FriendStationData friendStationData2 = this.activeFriendStationData[i];
						friendStationData2.actorNumberB = netPlayer.ActorNumber;
						if (friendStationData2.actorNumberA != -1 && friendStationData2.actorNumberB != -1)
						{
							friendStationData2.state = FriendingManager.FriendStationState.WaitingOnFriendStatusBoth;
						}
						else
						{
							friendStationData2.state = FriendingManager.FriendStationState.WaitingForPlayers;
						}
						this.activeFriendStationData[i] = friendStationData2;
					}
					if (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnFriendStatusBoth)
					{
						base.photonView.RPC("CheckFriendStatusRequestRPC", RpcTarget.All, new object[]
						{
							this.activeFriendStationData[i].zone,
							this.activeFriendStationData[i].actorNumberA,
							this.activeFriendStationData[i].actorNumberB
						});
						break;
					}
					break;
				}
				else
				{
					i++;
				}
			}
			if (num < 0)
			{
				this.activeFriendStationData.Add(new FriendingManager.FriendStationData
				{
					zone = zone,
					actorNumberA = netPlayer.ActorNumber,
					actorNumberB = -1,
					state = FriendingManager.FriendStationState.WaitingForPlayers
				});
			}
			this.UpdateFriendingStations();
		}
	}

	// Token: 0x06003E3E RID: 15934 RVA: 0x0013C1A8 File Offset: 0x0013A3A8
	public void PlayerExitedStation(GTZone zone, NetPlayer netPlayer)
	{
		if (netPlayer != null && netPlayer.ActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.localPlayerZone = GTZone.none;
		}
		if (PhotonNetwork.InRoom && base.photonView.IsMine)
		{
			int num = -1;
			int i = 0;
			while (i < this.activeFriendStationData.Count)
			{
				if (this.activeFriendStationData[i].zone == zone)
				{
					if ((this.activeFriendStationData[i].actorNumberA == netPlayer.ActorNumber && this.activeFriendStationData[i].actorNumberB == -1) || (this.activeFriendStationData[i].actorNumberA == -1 && this.activeFriendStationData[i].actorNumberB == netPlayer.ActorNumber))
					{
						FriendingManager.FriendStationData value = this.activeFriendStationData[i];
						value.actorNumberA = -1;
						value.actorNumberB = -1;
						value.state = FriendingManager.FriendStationState.WaitingForPlayers;
						this.activeFriendStationData[i] = value;
						num = i;
						break;
					}
					if (this.activeFriendStationData[i].actorNumberA != -1 && this.activeFriendStationData[i].actorNumberA != netPlayer.ActorNumber && this.activeFriendStationData[i].actorNumberB == netPlayer.ActorNumber)
					{
						FriendingManager.FriendStationData value2 = this.activeFriendStationData[i];
						value2.actorNumberB = -1;
						value2.state = FriendingManager.FriendStationState.WaitingForPlayers;
						this.activeFriendStationData[i] = value2;
						break;
					}
					if (this.activeFriendStationData[i].actorNumberB != -1 && this.activeFriendStationData[i].actorNumberB != netPlayer.ActorNumber && this.activeFriendStationData[i].actorNumberA == netPlayer.ActorNumber)
					{
						FriendingManager.FriendStationData value3 = this.activeFriendStationData[i];
						value3.actorNumberA = -1;
						value3.state = FriendingManager.FriendStationState.WaitingForPlayers;
						this.activeFriendStationData[i] = value3;
						break;
					}
					break;
				}
				else
				{
					i++;
				}
			}
			this.UpdateFriendingStations();
			if (num >= 0)
			{
				base.photonView.RPC("StationNoLongerActiveRPC", RpcTarget.Others, new object[]
				{
					this.activeFriendStationData[num].zone
				});
				this.activeFriendStationData.RemoveAt(num);
			}
		}
	}

	// Token: 0x06003E3F RID: 15935 RVA: 0x0013C3E8 File Offset: 0x0013A5E8
	private void PlayerPressedButton(GTZone zone, int playerId)
	{
		if (PhotonNetwork.InRoom && base.photonView.IsMine)
		{
			int i = 0;
			while (i < this.activeFriendStationData.Count)
			{
				if (this.activeFriendStationData[i].zone == zone)
				{
					if (this.activeFriendStationData[i].actorNumberA == -1 || this.activeFriendStationData[i].actorNumberB == -1)
					{
						break;
					}
					if ((this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnButtonPlayerA && this.activeFriendStationData[i].actorNumberA == playerId) || (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnButtonPlayerB && this.activeFriendStationData[i].actorNumberB == playerId))
					{
						FriendingManager.FriendStationData value = this.activeFriendStationData[i];
						value.state = FriendingManager.FriendStationState.ButtonConfirmationTimer0;
						value.progressBarStartTime = Time.time;
						this.activeFriendStationData[i] = value;
						return;
					}
					if (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnButtonBoth && this.activeFriendStationData[i].actorNumberA == playerId)
					{
						FriendingManager.FriendStationData value2 = this.activeFriendStationData[i];
						value2.state = FriendingManager.FriendStationState.WaitingOnButtonPlayerB;
						this.activeFriendStationData[i] = value2;
						return;
					}
					if (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnButtonBoth && this.activeFriendStationData[i].actorNumberB == playerId)
					{
						FriendingManager.FriendStationData value3 = this.activeFriendStationData[i];
						value3.state = FriendingManager.FriendStationState.WaitingOnButtonPlayerA;
						this.activeFriendStationData[i] = value3;
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
	}

	// Token: 0x06003E40 RID: 15936 RVA: 0x0013C588 File Offset: 0x0013A788
	private void PlayerUnpressedButton(GTZone zone, int playerId)
	{
		if (PhotonNetwork.InRoom && base.photonView.IsMine)
		{
			int i = 0;
			while (i < this.activeFriendStationData.Count)
			{
				if (this.activeFriendStationData[i].zone == zone)
				{
					if (this.activeFriendStationData[i].actorNumberA == -1 || this.activeFriendStationData[i].actorNumberB == -1)
					{
						break;
					}
					bool flag = this.activeFriendStationData[i].state >= FriendingManager.FriendStationState.ButtonConfirmationTimer0 && this.activeFriendStationData[i].state <= FriendingManager.FriendStationState.ButtonConfirmationTimer4;
					if (flag && this.activeFriendStationData[i].actorNumberA == playerId)
					{
						FriendingManager.FriendStationData value = this.activeFriendStationData[i];
						value.state = FriendingManager.FriendStationState.WaitingOnButtonPlayerA;
						this.activeFriendStationData[i] = value;
						return;
					}
					if (flag && this.activeFriendStationData[i].actorNumberB == playerId)
					{
						FriendingManager.FriendStationData value2 = this.activeFriendStationData[i];
						value2.state = FriendingManager.FriendStationState.WaitingOnButtonPlayerB;
						this.activeFriendStationData[i] = value2;
						return;
					}
					if ((this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnButtonPlayerA && this.activeFriendStationData[i].actorNumberB == playerId) || (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnButtonPlayerB && this.activeFriendStationData[i].actorNumberA == playerId))
					{
						FriendingManager.FriendStationData value3 = this.activeFriendStationData[i];
						value3.state = FriendingManager.FriendStationState.WaitingOnButtonBoth;
						this.activeFriendStationData[i] = value3;
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
	}

	// Token: 0x06003E41 RID: 15937 RVA: 0x0013C729 File Offset: 0x0013A929
	private void CheckFriendStatusRequest(GTZone zone, int actorNumberA, int actorNumberB)
	{
		FriendSystem.Instance.OnFriendListRefresh -= this.CheckFriendStatusOnFriendListRefresh;
		FriendSystem.Instance.OnFriendListRefresh += this.CheckFriendStatusOnFriendListRefresh;
		FriendSystem.Instance.RefreshFriendsList();
	}

	// Token: 0x06003E42 RID: 15938 RVA: 0x0013C768 File Offset: 0x0013A968
	private void CheckFriendStatusOnFriendListRefresh(List<FriendBackendController.Friend> friendList)
	{
		FriendSystem.Instance.OnFriendListRefresh -= this.CheckFriendStatusOnFriendListRefresh;
		int i = 0;
		while (i < this.activeFriendStationData.Count)
		{
			if (this.activeFriendStationData[i].zone == this.localPlayerZone)
			{
				int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
				int num = -1;
				if (this.activeFriendStationData[i].actorNumberA == actorNumber)
				{
					num = this.activeFriendStationData[i].actorNumberB;
				}
				else if (this.activeFriendStationData[i].actorNumberB == actorNumber)
				{
					num = this.activeFriendStationData[i].actorNumberA;
				}
				if (num != -1 && FriendSystem.Instance.CheckFriendshipWithPlayer(num))
				{
					base.photonView.RPC("CheckFriendStatusResponseRPC", RpcTarget.MasterClient, new object[]
					{
						this.localPlayerZone,
						num,
						true
					});
					return;
				}
				base.photonView.RPC("CheckFriendStatusResponseRPC", RpcTarget.MasterClient, new object[]
				{
					this.localPlayerZone,
					num,
					false
				});
				return;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x06003E43 RID: 15939 RVA: 0x0013C8A8 File Offset: 0x0013AAA8
	private void CheckFriendStatusResponse(GTZone zone, int responderActorNumber, int friendTargetActorNumber, bool friends)
	{
		if (PhotonNetwork.InRoom && base.photonView.IsMine)
		{
			int i = 0;
			while (i < this.activeFriendStationData.Count)
			{
				if (this.activeFriendStationData[i].zone == zone)
				{
					if (this.activeFriendStationData[i].actorNumberA == -1 || this.activeFriendStationData[i].actorNumberB == -1)
					{
						break;
					}
					if ((this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnFriendStatusPlayerA && this.activeFriendStationData[i].actorNumberA == responderActorNumber) || (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnFriendStatusPlayerB && this.activeFriendStationData[i].actorNumberB == responderActorNumber))
					{
						FriendingManager.FriendStationData value = this.activeFriendStationData[i];
						if (friends)
						{
							value.state = FriendingManager.FriendStationState.AlreadyFriends;
						}
						else
						{
							value.state = FriendingManager.FriendStationState.WaitingOnButtonBoth;
						}
						this.activeFriendStationData[i] = value;
						return;
					}
					if (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnFriendStatusBoth && this.activeFriendStationData[i].actorNumberA == responderActorNumber)
					{
						FriendingManager.FriendStationData value2 = this.activeFriendStationData[i];
						if (friends)
						{
							value2.state = FriendingManager.FriendStationState.WaitingOnFriendStatusPlayerB;
						}
						else
						{
							value2.state = FriendingManager.FriendStationState.WaitingOnButtonBoth;
						}
						this.activeFriendStationData[i] = value2;
						return;
					}
					if (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnFriendStatusBoth && this.activeFriendStationData[i].actorNumberB == responderActorNumber)
					{
						FriendingManager.FriendStationData value3 = this.activeFriendStationData[i];
						if (friends)
						{
							value3.state = FriendingManager.FriendStationState.WaitingOnFriendStatusPlayerA;
						}
						else
						{
							value3.state = FriendingManager.FriendStationState.WaitingOnButtonBoth;
						}
						this.activeFriendStationData[i] = value3;
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
	}

	// Token: 0x06003E44 RID: 15940 RVA: 0x0013CA64 File Offset: 0x0013AC64
	private void SendFriendRequestIfApplicable(GTZone zone)
	{
		int i = 0;
		while (i < this.activeFriendStationData.Count)
		{
			if (this.activeFriendStationData[i].zone == zone)
			{
				FriendingManager.FriendStationData friendStationData = this.activeFriendStationData[i];
				int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
				NetPlayer netPlayer = null;
				if (friendStationData.actorNumberA == actorNumber)
				{
					netPlayer = NetworkSystem.Instance.GetNetPlayerByID(friendStationData.actorNumberB);
				}
				else if (friendStationData.actorNumberB == actorNumber)
				{
					netPlayer = NetworkSystem.Instance.GetNetPlayerByID(friendStationData.actorNumberA);
				}
				if (netPlayer == null)
				{
					return;
				}
				FriendingStation friendingStation;
				if (this.friendingStations.TryGetValue(friendStationData.zone, out friendingStation) && (GTPlayer.Instance.HeadCenterPosition - friendingStation.transform.position).sqrMagnitude < this.requiredProximityToStation * this.requiredProximityToStation)
				{
					FriendSystem.Instance.SendFriendRequest(netPlayer, friendStationData.zone, new FriendSystem.FriendRequestCallback(this.FriendRequestCallback));
				}
				return;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x06003E45 RID: 15941 RVA: 0x0013CB60 File Offset: 0x0013AD60
	private void FriendRequestCompletedAuthority(GTZone zone, int playerId, bool succeeded)
	{
		if (PhotonNetwork.InRoom && base.photonView.IsMine)
		{
			int i = 0;
			while (i < this.activeFriendStationData.Count)
			{
				if (this.activeFriendStationData[i].zone == zone)
				{
					if (!succeeded)
					{
						FriendingManager.FriendStationData value = this.activeFriendStationData[i];
						value.state = FriendingManager.FriendStationState.RequestFailed;
						this.activeFriendStationData[i] = value;
						return;
					}
					if ((this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnRequestPlayerA && this.activeFriendStationData[i].actorNumberA == playerId) || (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnRequestPlayerB && this.activeFriendStationData[i].actorNumberB == playerId))
					{
						FriendingManager.FriendStationData value2 = this.activeFriendStationData[i];
						value2.state = FriendingManager.FriendStationState.Friends;
						this.activeFriendStationData[i] = value2;
						return;
					}
					if (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnRequestBoth && this.activeFriendStationData[i].actorNumberA == playerId)
					{
						FriendingManager.FriendStationData value3 = this.activeFriendStationData[i];
						value3.state = FriendingManager.FriendStationState.WaitingOnRequestPlayerB;
						this.activeFriendStationData[i] = value3;
						return;
					}
					if (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnRequestBoth && this.activeFriendStationData[i].actorNumberB == playerId)
					{
						FriendingManager.FriendStationData value4 = this.activeFriendStationData[i];
						value4.state = FriendingManager.FriendStationState.WaitingOnRequestPlayerA;
						this.activeFriendStationData[i] = value4;
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
	}

	// Token: 0x06003E46 RID: 15942 RVA: 0x0013CCF8 File Offset: 0x0013AEF8
	private void FriendRequestCallback(GTZone zone, int localId, int friendId, bool success)
	{
		if (base.photonView.IsMine)
		{
			this.FriendRequestCompletedAuthority(zone, PhotonNetwork.LocalPlayer.ActorNumber, success);
			return;
		}
		base.photonView.RPC("FriendRequestCompletedRPC", RpcTarget.MasterClient, new object[]
		{
			zone,
			success
		});
	}

	// Token: 0x06003E47 RID: 15943 RVA: 0x0013CD50 File Offset: 0x0013AF50
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.activeFriendStationData.Count);
			for (int i = 0; i < this.activeFriendStationData.Count; i++)
			{
				FriendingManager.<OnPhotonSerializeView>g__SendFriendStationData|31_0(stream, this.activeFriendStationData[i]);
			}
		}
		else if (stream.IsReading && info.Sender.IsMasterClient)
		{
			int num = (int)stream.ReceiveNext();
			if (num >= 0 && num <= 10)
			{
				this.activeFriendStationData.Clear();
				for (int j = 0; j < num; j++)
				{
					this.activeFriendStationData.Add(FriendingManager.<OnPhotonSerializeView>g__ReceiveFriendStationData|31_1(stream));
				}
			}
		}
		this.UpdateFriendingStations();
	}

	// Token: 0x06003E48 RID: 15944 RVA: 0x0013CE00 File Offset: 0x0013B000
	[PunRPC]
	public void CheckFriendStatusRequestRPC(GTZone zone, int actorNumberA, int actorNumberB, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "CheckFriendStatusRequestRPC");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender), out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[12].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.CheckFriendStatusRequest(zone, actorNumberA, actorNumberB);
	}

	// Token: 0x06003E49 RID: 15945 RVA: 0x0013CE68 File Offset: 0x0013B068
	[PunRPC]
	public void CheckFriendStatusResponseRPC(GTZone zone, int friendTargetActorNumber, bool friends, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "CheckFriendStatusRequestRPC");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender), out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[12].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.CheckFriendStatusResponse(zone, info.Sender.ActorNumber, friendTargetActorNumber, friends);
	}

	// Token: 0x06003E4A RID: 15946 RVA: 0x0013CEDC File Offset: 0x0013B0DC
	[PunRPC]
	public void FriendButtonPressedRPC(GTZone zone, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "FriendButtonPressedRPC");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender), out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[12].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.PlayerPressedButton(zone, info.Sender.ActorNumber);
	}

	// Token: 0x06003E4B RID: 15947 RVA: 0x0013CF4C File Offset: 0x0013B14C
	[PunRPC]
	public void FriendButtonUnpressedRPC(GTZone zone, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "FriendButtonUnpressedRPC");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender), out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[12].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.PlayerUnpressedButton(zone, info.Sender.ActorNumber);
	}

	// Token: 0x06003E4C RID: 15948 RVA: 0x0013CFBC File Offset: 0x0013B1BC
	[PunRPC]
	public void StationNoLongerActiveRPC(GTZone zone, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "StationNoLongerActiveRPC");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender), out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[12].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		FriendingStation friendingStation;
		if (info.Sender.IsMasterClient && this.friendingStations.TryGetValue(zone, out friendingStation))
		{
			friendingStation.UpdateState(new FriendingManager.FriendStationData
			{
				zone = zone,
				actorNumberA = -1,
				actorNumberB = -1,
				state = FriendingManager.FriendStationState.WaitingForPlayers
			});
		}
	}

	// Token: 0x06003E4D RID: 15949 RVA: 0x0013D064 File Offset: 0x0013B264
	[PunRPC]
	public void NotifyClientsFriendRequestReadyRPC(GTZone zone, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "NotifyClientsFriendRequestReadyRPC");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender), out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[12].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.SendFriendRequestIfApplicable(zone);
	}

	// Token: 0x06003E4E RID: 15950 RVA: 0x0013D0C8 File Offset: 0x0013B2C8
	[PunRPC]
	public void FriendRequestCompletedRPC(GTZone zone, bool succeeded, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "FriendRequestCompletedRPC");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender), out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[12].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.FriendRequestCompletedAuthority(zone, info.Sender.ActorNumber, succeeded);
	}

	// Token: 0x06003E50 RID: 15952 RVA: 0x0013D174 File Offset: 0x0013B374
	[CompilerGenerated]
	internal static void <OnPhotonSerializeView>g__SendFriendStationData|31_0(PhotonStream stream, FriendingManager.FriendStationData data)
	{
		stream.SendNext((int)data.zone);
		stream.SendNext(data.actorNumberA);
		stream.SendNext(data.actorNumberB);
		stream.SendNext((int)data.state);
	}

	// Token: 0x06003E51 RID: 15953 RVA: 0x0013D1C8 File Offset: 0x0013B3C8
	[CompilerGenerated]
	internal static FriendingManager.FriendStationData <OnPhotonSerializeView>g__ReceiveFriendStationData|31_1(PhotonStream stream)
	{
		return new FriendingManager.FriendStationData
		{
			zone = (GTZone)((int)stream.ReceiveNext()),
			actorNumberA = (int)stream.ReceiveNext(),
			actorNumberB = (int)stream.ReceiveNext(),
			state = (FriendingManager.FriendStationState)((int)stream.ReceiveNext())
		};
	}

	// Token: 0x04004A36 RID: 18998
	[OnEnterPlay_SetNull]
	public static volatile FriendingManager Instance;

	// Token: 0x04004A37 RID: 18999
	[SerializeField]
	private float progressBarDuration = 3f;

	// Token: 0x04004A38 RID: 19000
	[SerializeField]
	private float requiredProximityToStation = 3f;

	// Token: 0x04004A39 RID: 19001
	private List<FriendingManager.FriendStationData> activeFriendStationData = new List<FriendingManager.FriendStationData>(10);

	// Token: 0x04004A3A RID: 19002
	private Dictionary<GTZone, FriendingStation> friendingStations = new Dictionary<GTZone, FriendingStation>();

	// Token: 0x04004A3B RID: 19003
	private GTZone localPlayerZone = GTZone.none;

	// Token: 0x020009F1 RID: 2545
	public enum FriendStationState
	{
		// Token: 0x04004A3D RID: 19005
		NotInRoom,
		// Token: 0x04004A3E RID: 19006
		WaitingForPlayers,
		// Token: 0x04004A3F RID: 19007
		WaitingOnFriendStatusBoth,
		// Token: 0x04004A40 RID: 19008
		WaitingOnFriendStatusPlayerA,
		// Token: 0x04004A41 RID: 19009
		WaitingOnFriendStatusPlayerB,
		// Token: 0x04004A42 RID: 19010
		WaitingOnButtonBoth,
		// Token: 0x04004A43 RID: 19011
		WaitingOnButtonPlayerA,
		// Token: 0x04004A44 RID: 19012
		WaitingOnButtonPlayerB,
		// Token: 0x04004A45 RID: 19013
		ButtonConfirmationTimer0,
		// Token: 0x04004A46 RID: 19014
		ButtonConfirmationTimer1,
		// Token: 0x04004A47 RID: 19015
		ButtonConfirmationTimer2,
		// Token: 0x04004A48 RID: 19016
		ButtonConfirmationTimer3,
		// Token: 0x04004A49 RID: 19017
		ButtonConfirmationTimer4,
		// Token: 0x04004A4A RID: 19018
		WaitingOnRequestBoth,
		// Token: 0x04004A4B RID: 19019
		WaitingOnRequestPlayerA,
		// Token: 0x04004A4C RID: 19020
		WaitingOnRequestPlayerB,
		// Token: 0x04004A4D RID: 19021
		RequestFailed,
		// Token: 0x04004A4E RID: 19022
		Friends,
		// Token: 0x04004A4F RID: 19023
		AlreadyFriends
	}

	// Token: 0x020009F2 RID: 2546
	public struct FriendStationData
	{
		// Token: 0x04004A50 RID: 19024
		public GTZone zone;

		// Token: 0x04004A51 RID: 19025
		public int actorNumberA;

		// Token: 0x04004A52 RID: 19026
		public int actorNumberB;

		// Token: 0x04004A53 RID: 19027
		public FriendingManager.FriendStationState state;

		// Token: 0x04004A54 RID: 19028
		public float progressBarStartTime;
	}
}
