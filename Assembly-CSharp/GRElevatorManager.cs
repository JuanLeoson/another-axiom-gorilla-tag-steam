using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000620 RID: 1568
[NetworkBehaviourWeaved(0)]
public class GRElevatorManager : NetworkComponent
{
	// Token: 0x170003A1 RID: 929
	// (get) Token: 0x0600266A RID: 9834 RVA: 0x000CD16E File Offset: 0x000CB36E
	public bool InPrivateRoom
	{
		get
		{
			return NetworkSystem.Instance.SessionIsPrivate;
		}
	}

	// Token: 0x0600266B RID: 9835 RVA: 0x000CD17C File Offset: 0x000CB37C
	protected override void Awake()
	{
		base.Awake();
		if (GRElevatorManager._instance != null)
		{
			Debug.LogError("Multiple elevator managers! This should never happen!");
			return;
		}
		GRElevatorManager._instance = this;
		this.currentState = GRElevatorManager.ElevatorSystemState.InLocation;
		this.currentLocation = GRElevatorManager.ElevatorLocation.Stump;
		this.destination = GRElevatorManager.ElevatorLocation.Stump;
		this.elevatorByLocation = new Dictionary<GRElevatorManager.ElevatorLocation, GRElevator>();
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			this.elevatorByLocation[this.allElevators[i].location] = this.allElevators[i];
		}
		this.actorIds = new List<int>();
	}

	// Token: 0x0600266C RID: 9836 RVA: 0x000CD218 File Offset: 0x000CB418
	public void Update()
	{
		if (!this.cosmeticsInitialized)
		{
			this.CheckInitializationState();
			return;
		}
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			this.allElevators[i].PhysicalElevatorUpdate();
		}
		this.ProcessElevatorSystemState();
		if (this.justTeleported)
		{
			this.justTeleported = false;
			GTPlayer.Instance.disableMovement = false;
		}
	}

	// Token: 0x0600266D RID: 9837 RVA: 0x000CD27B File Offset: 0x000CB47B
	private void CheckInitializationState()
	{
		if (!VRRig.LocalRig.RigBuildFullyInitialized)
		{
			return;
		}
		this.cosmeticsInitialized = true;
		if (GRElevatorManager.InControlOfElevator())
		{
			this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.InLocation, GRElevatorManager.ElevatorLocation.Stump);
		}
	}

	// Token: 0x0600266E RID: 9838 RVA: 0x000CD2A0 File Offset: 0x000CB4A0
	public void ProcessElevatorSystemState()
	{
		switch (this.currentState)
		{
		case GRElevatorManager.ElevatorSystemState.Dormant:
		case GRElevatorManager.ElevatorSystemState.InLocation:
			break;
		case GRElevatorManager.ElevatorSystemState.DestinationPressed:
		{
			if (!GRElevatorManager.InControlOfElevator())
			{
				return;
			}
			double time = this.GetTime();
			if (this.elevatorByLocation[this.currentLocation].DoorsFullyClosed() && time >= this.doorsFullyClosedTime + (double)this.doorsFullyClosedDelay)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.WaitingToTeleport, GRElevatorManager.ElevatorLocation.None);
				return;
			}
			if (time >= this.destinationButtonLastPressedTime + (double)this.destinationButtonlastPressedDelay && !this.elevatorByLocation[this.currentLocation].DoorIsClosing())
			{
				this.destinationButtonLastPressedTime = time;
				this.CloseAllElevators();
				return;
			}
			break;
		}
		case GRElevatorManager.ElevatorSystemState.WaitingToTeleport:
			if (!GRElevatorManager.InControlOfElevator())
			{
				return;
			}
			if (this.GetTime() >= this.doorsFullyClosedTime + (double)this.doorsFullyClosedDelay && !this.waitingForRemoteTeleport)
			{
				this.ActivateElevating();
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x0600266F RID: 9839 RVA: 0x000CD370 File Offset: 0x000CB570
	public void ActivateElevating()
	{
		if (PhotonNetwork.InRoom)
		{
			this.photonView.RPC("RemoteActivateTeleport", RpcTarget.All, new object[]
			{
				(int)this.currentLocation,
				(int)this.destination,
				GRElevatorManager.LowestActorNumberInElevator()
			});
			return;
		}
		this.ActivateTeleport(this.currentLocation, this.destination, -1, this.GetTime());
	}

	// Token: 0x06002670 RID: 9840 RVA: 0x000CD3E0 File Offset: 0x000CB5E0
	public void LeadElevatorJoin()
	{
		if (NetworkSystem.Instance.InRoom)
		{
			GorillaFriendCollider friendCollider = this.elevatorByLocation[this.currentLocation].friendCollider;
			GorillaFriendCollider friendCollider2 = this.elevatorByLocation[this.destination].friendCollider;
			friendCollider.RefreshPlayersInSphere();
			friendCollider2.RefreshPlayersInSphere();
			PhotonNetworkController.Instance.FriendIDList = new List<string>(friendCollider.playerIDsCurrentlyTouching);
			PhotonNetworkController.Instance.FriendIDList.AddRange(friendCollider2.playerIDsCurrentlyTouching);
			foreach (string text in PhotonNetworkController.Instance.FriendIDList)
			{
			}
			PhotonNetworkController.Instance.shuffler = Random.Range(0, 99).ToString().PadLeft(2, '0') + Random.Range(0, 99999999).ToString().PadLeft(8, '0');
			PhotonNetworkController.Instance.keyStr = Random.Range(0, 99999999).ToString().PadLeft(8, '0');
			RoomSystem.SendElevatorFollowCommand(PhotonNetworkController.Instance.shuffler, PhotonNetworkController.Instance.keyStr, friendCollider, friendCollider2);
			PhotonNetwork.SendAllOutgoingCommands();
			PhotonNetworkController.Instance.AttemptToJoinPublicRoom(this.elevatorByLocation[this.destination].joinTrigger, JoinType.JoinWithElevator, null);
		}
		GRElevatorManager.JoinPublicRoom();
	}

	// Token: 0x06002671 RID: 9841 RVA: 0x000CD564 File Offset: 0x000CB764
	public void UpdateElevatorState(GRElevatorManager.ElevatorSystemState newState, GRElevatorManager.ElevatorLocation location = GRElevatorManager.ElevatorLocation.None)
	{
		switch (this.currentState)
		{
		case GRElevatorManager.ElevatorSystemState.Dormant:
			switch (newState)
			{
			case GRElevatorManager.ElevatorSystemState.InLocation:
				this.elevatorByLocation[this.currentLocation].PlayDing();
				this.OpenElevator(this.destination);
				break;
			case GRElevatorManager.ElevatorSystemState.DestinationPressed:
			case GRElevatorManager.ElevatorSystemState.WaitingToTeleport:
				this.maxDoorClosingTime = this.GetTime();
				this.destinationButtonLastPressedTime = this.GetTime();
				this.doorsFullyClosedTime = this.GetTime();
				if (this.destination != this.currentLocation)
				{
					this.destination = location;
				}
				this.elevatorByLocation[this.currentLocation].PlayElevatorMoving();
				this.elevatorByLocation[this.destination].PlayElevatorMoving();
				break;
			}
			break;
		case GRElevatorManager.ElevatorSystemState.InLocation:
			switch (newState)
			{
			case GRElevatorManager.ElevatorSystemState.Dormant:
				this.CloseAllElevators();
				break;
			case GRElevatorManager.ElevatorSystemState.InLocation:
				if (location == this.currentLocation)
				{
					this.OpenElevator(this.currentLocation);
				}
				else
				{
					this.CloseAllElevators();
				}
				break;
			case GRElevatorManager.ElevatorSystemState.DestinationPressed:
				if (location != this.currentLocation)
				{
					this.destination = location;
					this.destinationButtonLastPressedTime = this.GetTime();
					this.maxDoorClosingTime = this.GetTime();
				}
				else
				{
					if (this.elevatorByLocation[this.destination].DoorIsClosing())
					{
						this.OpenElevator(this.currentLocation);
					}
					newState = this.currentState;
				}
				if (this.currentLocation != GRElevatorManager.ElevatorLocation.None)
				{
					this.elevatorByLocation[this.currentLocation].PlayElevatorMoving();
				}
				this.elevatorByLocation[this.destination].PlayElevatorMoving();
				break;
			case GRElevatorManager.ElevatorSystemState.WaitingToTeleport:
				if (this.currentLocation != GRElevatorManager.ElevatorLocation.None)
				{
					this.elevatorByLocation[this.currentLocation].PlayElevatorMoving();
				}
				this.elevatorByLocation[this.destination].PlayElevatorMoving();
				break;
			}
			break;
		case GRElevatorManager.ElevatorSystemState.DestinationPressed:
			switch (newState)
			{
			case GRElevatorManager.ElevatorSystemState.Dormant:
				this.CloseAllElevators();
				break;
			case GRElevatorManager.ElevatorSystemState.InLocation:
				this.OpenElevator(location);
				this.elevatorByLocation[this.currentLocation].PlayDing();
				break;
			case GRElevatorManager.ElevatorSystemState.DestinationPressed:
				if (location != this.currentLocation)
				{
					this.destination = location;
				}
				break;
			case GRElevatorManager.ElevatorSystemState.WaitingToTeleport:
				this.doorsFullyClosedTime = this.GetTime();
				if (this.currentLocation != GRElevatorManager.ElevatorLocation.None)
				{
					this.elevatorByLocation[this.currentLocation].PlayElevatorMoving();
					this.elevatorByLocation[this.currentLocation].PlayElevatorMusic(0f);
				}
				this.elevatorByLocation[this.destination].PlayElevatorMoving();
				break;
			}
			break;
		case GRElevatorManager.ElevatorSystemState.WaitingToTeleport:
			switch (newState)
			{
			case GRElevatorManager.ElevatorSystemState.Dormant:
				this.CloseAllElevators();
				this.elevatorByLocation[this.currentLocation].PlayElevatorStopped();
				this.elevatorByLocation[this.destination].PlayElevatorStopped();
				break;
			case GRElevatorManager.ElevatorSystemState.InLocation:
				this.elevatorByLocation[this.currentLocation].PlayElevatorStopped();
				this.elevatorByLocation[this.destination].PlayElevatorStopped();
				this.currentLocation = location;
				if (location == this.destination)
				{
					this.OpenElevator(this.currentLocation);
					this.elevatorByLocation[this.currentLocation].PlayDing();
				}
				break;
			case GRElevatorManager.ElevatorSystemState.DestinationPressed:
			case GRElevatorManager.ElevatorSystemState.WaitingToTeleport:
				if (location != this.currentLocation)
				{
					this.destination = location;
				}
				else
				{
					this.OpenElevator(location);
					newState = GRElevatorManager.ElevatorSystemState.InLocation;
				}
				break;
			}
			break;
		}
		this.currentState = newState;
		this.UpdateUI();
	}

	// Token: 0x06002672 RID: 9842 RVA: 0x000CD8F0 File Offset: 0x000CBAF0
	public void UpdateUI()
	{
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			this.allElevators[i].outerText.text = "ELEVATOR LOCATION:\n" + this.currentLocation.ToString().ToUpper();
			GRElevatorManager.ElevatorSystemState elevatorSystemState = this.currentState;
			if (elevatorSystemState > GRElevatorManager.ElevatorSystemState.InLocation)
			{
				if (elevatorSystemState - GRElevatorManager.ElevatorSystemState.DestinationPressed <= 1)
				{
					if (this.destination != this.currentLocation)
					{
						this.allElevators[i].innerText.text = "NEXT STOP:\n" + this.destination.ToString().ToUpper();
					}
					else
					{
						this.allElevators[i].innerText.text = "CHOOSE DESTINATION";
					}
				}
			}
			else
			{
				this.allElevators[i].innerText.text = "CHOOSE DESTINATION";
			}
		}
	}

	// Token: 0x06002673 RID: 9843 RVA: 0x000CD9E0 File Offset: 0x000CBBE0
	public static void RegisterElevator(GRElevator elevator)
	{
		if (GRElevatorManager._instance == null)
		{
			return;
		}
		GRElevatorManager._instance.elevatorByLocation[elevator.location] = elevator;
	}

	// Token: 0x06002674 RID: 9844 RVA: 0x000CDA06 File Offset: 0x000CBC06
	public static void DeregisterElevator(GRElevator elevator)
	{
		if (GRElevatorManager._instance == null)
		{
			return;
		}
		GRElevatorManager._instance.elevatorByLocation[elevator.location] = null;
	}

	// Token: 0x06002675 RID: 9845 RVA: 0x000CDA2C File Offset: 0x000CBC2C
	public static void ElevatorButtonPressed(GRElevator.ButtonType type, GRElevatorManager.ElevatorLocation location)
	{
		if (GRElevatorManager._instance != null)
		{
			GRElevatorManager._instance.ElevatorButtonPressedInternal(type, location);
			if (!GRElevatorManager._instance.IsMine && NetworkSystem.Instance.InRoom)
			{
				GRElevatorManager._instance.photonView.RPC("RemoteElevatorButtonPress", RpcTarget.MasterClient, new object[]
				{
					(int)type,
					(int)location
				});
			}
		}
	}

	// Token: 0x06002676 RID: 9846 RVA: 0x000CDA97 File Offset: 0x000CBC97
	private void ElevatorButtonPressedInternal(GRElevator.ButtonType type, GRElevatorManager.ElevatorLocation location)
	{
		this.elevatorByLocation[location].PressButtonVisuals(type);
		this.elevatorByLocation[location].PlayButtonPress();
		if (base.IsMine)
		{
			this.ProcessElevatorButtonPress(type, location);
		}
	}

	// Token: 0x06002677 RID: 9847 RVA: 0x000CDACC File Offset: 0x000CBCCC
	public void ProcessElevatorButtonPress(GRElevator.ButtonType type, GRElevatorManager.ElevatorLocation location)
	{
		switch (type)
		{
		case GRElevator.ButtonType.Stump:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.DestinationPressed, GRElevatorManager.ElevatorLocation.Stump);
				return;
			}
			break;
		case GRElevator.ButtonType.City:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.DestinationPressed, GRElevatorManager.ElevatorLocation.City);
				return;
			}
			break;
		case GRElevator.ButtonType.GhostReactor:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.DestinationPressed, GRElevatorManager.ElevatorLocation.GhostReactor);
				return;
			}
			break;
		case GRElevator.ButtonType.Open:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport)
			{
				if (this.currentState == GRElevatorManager.ElevatorSystemState.DestinationPressed)
				{
					if (this.GetTime() >= this.maxDoorClosingTime + (double)this.doorMaxClosingDelay)
					{
						break;
					}
					this.destinationButtonLastPressedTime = this.GetTime();
					this.doorsFullyClosedTime = this.GetTime();
				}
				this.OpenElevator(location);
				return;
			}
			break;
		case GRElevator.ButtonType.Close:
			this.CloseAllElevators();
			break;
		case GRElevator.ButtonType.Summon:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport && this.currentState != GRElevatorManager.ElevatorSystemState.DestinationPressed)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.DestinationPressed, location);
				return;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002678 RID: 9848 RVA: 0x000CDBA0 File Offset: 0x000CBDA0
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.doorsFullyClosedTime);
		stream.SendNext(this.destinationButtonLastPressedTime);
		stream.SendNext(this.maxDoorClosingTime);
		stream.SendNext((int)this.currentLocation);
		stream.SendNext((int)this.destination);
		stream.SendNext((int)this.currentState);
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			stream.SendNext((int)this.allElevators[i].state);
		}
	}

	// Token: 0x06002679 RID: 9849 RVA: 0x000CDC48 File Offset: 0x000CBE48
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		double d = (double)stream.ReceiveNext();
		if (!double.IsNaN(d) && !double.IsInfinity(d))
		{
			this.doorsFullyClosedTime = d;
		}
		d = (double)stream.ReceiveNext();
		if (!double.IsNaN(d) && !double.IsInfinity(d))
		{
			this.destinationButtonLastPressedTime = d;
		}
		d = (double)stream.ReceiveNext();
		if (!double.IsNaN(d) && !double.IsInfinity(d))
		{
			this.maxDoorClosingTime = d;
		}
		GRElevatorManager.ElevatorLocation elevatorLocation = this.currentLocation;
		int num = (int)stream.ReceiveNext();
		if (num >= 0 && num <= 3)
		{
			this.currentLocation = (GRElevatorManager.ElevatorLocation)num;
		}
		GRElevatorManager.ElevatorLocation elevatorLocation2 = this.destination;
		num = (int)stream.ReceiveNext();
		if (num >= 0 && num <= 3)
		{
			this.destination = (GRElevatorManager.ElevatorLocation)num;
		}
		num = (int)stream.ReceiveNext();
		if (num >= 0 && num < 5)
		{
			this.currentState = (GRElevatorManager.ElevatorSystemState)num;
		}
		this.UpdateUI();
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			num = (int)stream.ReceiveNext();
			if (num >= 0 && num < 8)
			{
				this.allElevators[i].UpdateRemoteState((GRElevator.ElevatorState)num);
			}
		}
	}

	// Token: 0x0600267A RID: 9850 RVA: 0x000CDD6D File Offset: 0x000CBF6D
	[PunRPC]
	public void RemoteElevatorButtonPress(int elevatorButtonPressed, int elevatorLocation, PhotonMessageInfo info)
	{
		if (!base.IsMine || this.m_RpcSpamChecks.IsSpamming(GRElevatorManager.RPC.RemoteElevatorButtonPress))
		{
			return;
		}
		if (elevatorLocation < 0 || elevatorLocation >= 3)
		{
			return;
		}
		if (elevatorButtonPressed < 0 || elevatorButtonPressed >= 7)
		{
			return;
		}
		this.ElevatorButtonPressedInternal((GRElevator.ButtonType)elevatorButtonPressed, (GRElevatorManager.ElevatorLocation)elevatorLocation);
	}

	// Token: 0x0600267B RID: 9851 RVA: 0x000CDDA0 File Offset: 0x000CBFA0
	[PunRPC]
	public void RemoteActivateTeleport(int elevatorStartLocation, int elevatorDestinationLocation, int lowestActorNumber, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient || this.m_RpcSpamChecks.IsSpamming(GRElevatorManager.RPC.RemoteActivateTeleport))
		{
			return;
		}
		if (elevatorStartLocation < 0 || elevatorStartLocation >= 3 || elevatorDestinationLocation < 0 || elevatorDestinationLocation >= 3)
		{
			return;
		}
		if (!this.waitingForRemoteTeleport)
		{
			base.StartCoroutine(this.TeleportDelay((GRElevatorManager.ElevatorLocation)elevatorStartLocation, (GRElevatorManager.ElevatorLocation)elevatorDestinationLocation, lowestActorNumber, info.SentServerTime));
		}
	}

	// Token: 0x0600267C RID: 9852 RVA: 0x000CDDFA File Offset: 0x000CBFFA
	private IEnumerator TeleportDelay(GRElevatorManager.ElevatorLocation start, GRElevatorManager.ElevatorLocation destination, int lowestActorNumber, double sentServerTime)
	{
		this.timeLastTeleported = (double)Time.time;
		this.waitingForRemoteTeleport = true;
		this.lastTeleportSource = start;
		yield return new WaitForSeconds((float)(PhotonNetwork.Time - (sentServerTime + 0.75)));
		this.RefreshTeleportingPlayersJoinTime();
		yield return new WaitForSeconds(0.25f);
		this.waitingForRemoteTeleport = false;
		this.ActivateTeleport(start, destination, lowestActorNumber, sentServerTime);
		yield break;
	}

	// Token: 0x0600267D RID: 9853 RVA: 0x000CDE28 File Offset: 0x000CC028
	public void ActivateTeleport(GRElevatorManager.ElevatorLocation start, GRElevatorManager.ElevatorLocation destination, int lowestActorNumber, double photonServerTime)
	{
		GRElevator grelevator = this.elevatorByLocation[start];
		GRElevator grelevator2 = this.elevatorByLocation[destination];
		if (grelevator == null || grelevator2 == null)
		{
			return;
		}
		grelevator.friendCollider.RefreshPlayersInSphere();
		if (!PhotonNetwork.InRoom)
		{
			this.RefreshTeleportingPlayersJoinTime();
		}
		if (!grelevator.friendCollider.playerIDsCurrentlyTouching.Contains(NetworkSystem.Instance.LocalPlayer.UserId))
		{
			this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.InLocation, destination);
			return;
		}
		this.elevatorByLocation[destination].collidersAndVisuals.SetActive(true);
		float num = grelevator2.transform.rotation.eulerAngles.y - grelevator.transform.rotation.eulerAngles.y;
		GTPlayer instance = GTPlayer.Instance;
		VRRig localRig = VRRig.LocalRig;
		Vector3 b = localRig.transform.position - instance.transform.position;
		Vector3 b2 = instance.headCollider.transform.position - instance.transform.position;
		Vector3 a = grelevator2.transform.TransformPoint(grelevator.transform.InverseTransformPoint(instance.transform.position));
		Vector3 point = localRig.transform.position - grelevator.transform.position;
		point.x *= 0.8f;
		point.z *= 0.8f;
		a = grelevator2.transform.position + (Quaternion.Euler(0f, num, 0f) * point - b) + localRig.headConstraint.rotation * localRig.head.trackingPositionOffset;
		Vector3 b3 = Vector3.zero;
		Vector3 vector = grelevator2.transform.position + (Quaternion.Euler(0f, num, 0f) * point - b) + b2 - grelevator2.transform.position;
		float magnitude = vector.magnitude;
		vector = vector.normalized;
		if (Physics.SphereCastNonAlloc(grelevator2.transform.position, instance.headCollider.radius * 1.5f, vector, this.correctionRaycastHit, magnitude * 1.05f, this.correctionRaycastMask) > 0)
		{
			b3 = vector * instance.headCollider.radius * -1.5f;
		}
		instance.TeleportTo(a + b3, instance.transform.rotation, false);
		instance.turnParent.transform.RotateAround(instance.headCollider.transform.position, base.transform.up, num);
		localRig.transform.position = instance.transform.position + b;
		instance.InitializeValues();
		this.justTeleported = true;
		instance.disableMovement = true;
		GorillaComputer.instance.allowedMapsToJoin = this.elevatorByLocation[destination].joinTrigger.myCollider.myAllowedMapsToJoin;
		this.lastTeleportSource = start;
		this.lastLowestActorNr = lowestActorNumber;
		if (!this.InPrivateRoom && lowestActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.LeadElevatorJoin();
		}
		this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.InLocation, destination);
		grelevator2.PlayElevatorMusic(grelevator.musicAudio.time);
	}

	// Token: 0x0600267E RID: 9854 RVA: 0x000CE194 File Offset: 0x000CC394
	public void CloseAllElevators()
	{
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			if (!this.allElevators[i].DoorIsClosing())
			{
				this.allElevators[i].UpdateLocalState(GRElevator.ElevatorState.DoorBeginClosing);
			}
		}
	}

	// Token: 0x0600267F RID: 9855 RVA: 0x000CE1DC File Offset: 0x000CC3DC
	public void OpenElevator(GRElevatorManager.ElevatorLocation location)
	{
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			this.allElevators[i].UpdateLocalState((this.allElevators[i].location == location) ? GRElevator.ElevatorState.DoorBeginOpening : GRElevator.ElevatorState.DoorBeginClosing);
		}
	}

	// Token: 0x06002680 RID: 9856 RVA: 0x000CE228 File Offset: 0x000CC428
	public double GetTime()
	{
		double num = PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time);
		if (this.doorsFullyClosedTime > num || this.destinationButtonLastPressedTime > num || this.maxDoorClosingTime > num || num - this.doorsFullyClosedTime > 10.0 || num - this.destinationButtonLastPressedTime > 10.0 || num - this.maxDoorClosingTime > 20.0)
		{
			this.doorsFullyClosedTime = num;
			this.destinationButtonLastPressedTime = num;
			this.maxDoorClosingTime = num;
		}
		return num;
	}

	// Token: 0x06002681 RID: 9857 RVA: 0x000CE2B4 File Offset: 0x000CC4B4
	public static bool ValidElevatorNetworking(int actorNr)
	{
		if (GRElevatorManager._instance == null)
		{
			return false;
		}
		if (NetworkSystem.Instance.SessionIsPrivate)
		{
			return false;
		}
		if (actorNr == GRElevatorManager._instance.lastLowestActorNr)
		{
			return true;
		}
		if (GRElevatorManager._instance.lastTeleportSource == GRElevatorManager.ElevatorLocation.None)
		{
			return false;
		}
		GorillaFriendCollider friendCollider = GRElevatorManager._instance.elevatorByLocation[GRElevatorManager._instance.destination].friendCollider;
		GorillaFriendCollider friendCollider2 = GRElevatorManager._instance.elevatorByLocation[GRElevatorManager._instance.lastTeleportSource].friendCollider;
		if ((double)Time.time < GRElevatorManager._instance.timeLastTeleported + 3.0)
		{
			friendCollider.RefreshPlayersInSphere();
			friendCollider2.RefreshPlayersInSphere();
		}
		NetPlayer netPlayer = NetPlayer.Get(actorNr);
		return netPlayer != null && (friendCollider.playerIDsCurrentlyTouching.Contains(netPlayer.UserId) || friendCollider2.playerIDsCurrentlyTouching.Contains(netPlayer.UserId));
	}

	// Token: 0x06002682 RID: 9858 RVA: 0x000CE394 File Offset: 0x000CC594
	public static int LowestActorNumberInElevator()
	{
		GorillaFriendCollider friendCollider = GRElevatorManager._instance.elevatorByLocation[GRElevatorManager._instance.currentLocation].friendCollider;
		GorillaFriendCollider friendCollider2 = GRElevatorManager._instance.elevatorByLocation[GRElevatorManager._instance.destination].friendCollider;
		friendCollider.RefreshPlayersInSphere();
		friendCollider2.RefreshPlayersInSphere();
		int num = int.MaxValue;
		NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
		for (int i = 0; i < allNetPlayers.Length; i++)
		{
			if (num > allNetPlayers[i].ActorNumber && (friendCollider.playerIDsCurrentlyTouching.Contains(allNetPlayers[i].UserId) || friendCollider2.playerIDsCurrentlyTouching.Contains(allNetPlayers[i].UserId)))
			{
				num = allNetPlayers[i].ActorNumber;
			}
		}
		return num;
	}

	// Token: 0x06002683 RID: 9859 RVA: 0x000CE454 File Offset: 0x000CC654
	private void RefreshTeleportingPlayersJoinTime()
	{
		GorillaFriendCollider friendCollider = GRElevatorManager._instance.elevatorByLocation[GRElevatorManager._instance.currentLocation].friendCollider;
		this.actorIds.Clear();
		NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
		for (int i = 0; i < allNetPlayers.Length; i++)
		{
			RigContainer rigContainer;
			if (friendCollider.playerIDsCurrentlyTouching.Contains(allNetPlayers[i].UserId) && VRRigCache.Instance.TryGetVrrig(allNetPlayers[i], out rigContainer))
			{
				rigContainer.Rig.ResetTimeSpawned();
			}
		}
	}

	// Token: 0x06002684 RID: 9860 RVA: 0x000CE4D5 File Offset: 0x000CC6D5
	public static bool InControlOfElevator()
	{
		return !NetworkSystem.Instance.InRoom || GRElevatorManager._instance.IsMine;
	}

	// Token: 0x06002685 RID: 9861 RVA: 0x000CE4EF File Offset: 0x000CC6EF
	public static void JoinPublicRoom()
	{
		PhotonNetworkController.Instance.AttemptToJoinPublicRoom(GRElevatorManager._instance.elevatorByLocation[GRElevatorManager._instance.destination].joinTrigger, JoinType.Solo, null);
	}

	// Token: 0x06002686 RID: 9862 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void WriteDataFusion()
	{
	}

	// Token: 0x06002687 RID: 9863 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void ReadDataFusion()
	{
	}

	// Token: 0x06002689 RID: 9865 RVA: 0x00002637 File Offset: 0x00000837
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x0600268A RID: 9866 RVA: 0x00002643 File Offset: 0x00000843
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x040030CF RID: 12495
	public PhotonView photonView;

	// Token: 0x040030D0 RID: 12496
	public static GRElevatorManager _instance;

	// Token: 0x040030D1 RID: 12497
	public Dictionary<GRElevatorManager.ElevatorLocation, GRElevator> elevatorByLocation;

	// Token: 0x040030D2 RID: 12498
	public List<GRElevator> allElevators;

	// Token: 0x040030D3 RID: 12499
	[SerializeField]
	private GRElevatorManager.ElevatorLocation destination;

	// Token: 0x040030D4 RID: 12500
	[SerializeField]
	private GRElevatorManager.ElevatorLocation currentLocation;

	// Token: 0x040030D5 RID: 12501
	private GRElevatorManager.ElevatorLocation lastTeleportSource = GRElevatorManager.ElevatorLocation.None;

	// Token: 0x040030D6 RID: 12502
	public GRElevatorManager.ElevatorSystemState currentState;

	// Token: 0x040030D7 RID: 12503
	private double timeLastTeleported;

	// Token: 0x040030D8 RID: 12504
	private bool cosmeticsInitialized;

	// Token: 0x040030D9 RID: 12505
	public float destinationButtonlastPressedDelay = 3f;

	// Token: 0x040030DA RID: 12506
	public float doorsFullyClosedDelay = 3f;

	// Token: 0x040030DB RID: 12507
	public float doorMaxClosingDelay = 12f;

	// Token: 0x040030DC RID: 12508
	public double destinationButtonLastPressedTime;

	// Token: 0x040030DD RID: 12509
	public double doorsFullyClosedTime;

	// Token: 0x040030DE RID: 12510
	public double maxDoorClosingTime;

	// Token: 0x040030DF RID: 12511
	private List<int> actorIds;

	// Token: 0x040030E0 RID: 12512
	public CallLimitersList<CallLimiter, GRElevatorManager.RPC> m_RpcSpamChecks = new CallLimitersList<CallLimiter, GRElevatorManager.RPC>();

	// Token: 0x040030E1 RID: 12513
	private bool justTeleported;

	// Token: 0x040030E2 RID: 12514
	private bool waitingForRemoteTeleport;

	// Token: 0x040030E3 RID: 12515
	private int lastLowestActorNr;

	// Token: 0x040030E4 RID: 12516
	private RaycastHit[] correctionRaycastHit = new RaycastHit[1];

	// Token: 0x040030E5 RID: 12517
	public LayerMask correctionRaycastMask;

	// Token: 0x02000621 RID: 1569
	public enum ElevatorSystemState
	{
		// Token: 0x040030E7 RID: 12519
		Dormant,
		// Token: 0x040030E8 RID: 12520
		InLocation,
		// Token: 0x040030E9 RID: 12521
		DestinationPressed,
		// Token: 0x040030EA RID: 12522
		WaitingToTeleport,
		// Token: 0x040030EB RID: 12523
		Teleporting,
		// Token: 0x040030EC RID: 12524
		None
	}

	// Token: 0x02000622 RID: 1570
	public enum RPC
	{
		// Token: 0x040030EE RID: 12526
		RemoteElevatorButtonPress,
		// Token: 0x040030EF RID: 12527
		RemoteActivateTeleport
	}

	// Token: 0x02000623 RID: 1571
	public enum ElevatorLocation
	{
		// Token: 0x040030F1 RID: 12529
		Stump,
		// Token: 0x040030F2 RID: 12530
		City,
		// Token: 0x040030F3 RID: 12531
		GhostReactor,
		// Token: 0x040030F4 RID: 12532
		None
	}
}
