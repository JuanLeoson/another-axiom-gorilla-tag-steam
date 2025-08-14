using System;
using System.Collections.Generic;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x0200061C RID: 1564
public class GRElevator : MonoBehaviour
{
	// Token: 0x0600264C RID: 9804 RVA: 0x000CC998 File Offset: 0x000CAB98
	private void OnEnable()
	{
		GRElevatorManager.RegisterElevator(this);
		this.ambientAudio.clip = this.ambientLoopClip;
		this.ambientAudio.Play();
	}

	// Token: 0x0600264D RID: 9805 RVA: 0x000CC9BC File Offset: 0x000CABBC
	private void OnDisable()
	{
		GRElevatorManager.DeregisterElevator(this);
	}

	// Token: 0x0600264E RID: 9806 RVA: 0x000CC9C4 File Offset: 0x000CABC4
	private void Awake()
	{
		this.typeButtonDict = new Dictionary<GRElevator.ButtonType, GRElevatorButton>();
		for (int i = 0; i < this.elevatorButtons.Count; i++)
		{
			if (!this.typeButtonDict.TryAdd(this.elevatorButtons[i].buttonType, this.elevatorButtons[i]))
			{
				Debug.LogError("elevator already has that type of buttan: " + this.elevatorButtons[i].buttonType.ToString(), this);
			}
		}
		this.travelDistance = (this.openTargetTop.position - this.closedTargetTop.position).magnitude;
		this.doorOpenSpeed = this.travelDistance / this.openTravelDuration;
		this.doorCloseSpeed = this.travelDistance / this.closeTravelDuration;
		this.state = GRElevator.ElevatorState.DoorClosed;
		this.UpdateLocalState(this.state);
	}

	// Token: 0x0600264F RID: 9807 RVA: 0x000CCAAA File Offset: 0x000CACAA
	public void PressButton(int type)
	{
		GRElevatorManager.ElevatorButtonPressed((GRElevator.ButtonType)type, this.location);
	}

	// Token: 0x06002650 RID: 9808 RVA: 0x000CCAB8 File Offset: 0x000CACB8
	public void PressButtonVisuals(GRElevator.ButtonType type)
	{
		this.typeButtonDict[type].Pressed();
	}

	// Token: 0x06002651 RID: 9809 RVA: 0x000CCACB File Offset: 0x000CACCB
	public void PlayDing()
	{
		this.ambientAudio.PlayOneShot(this.dingClip);
	}

	// Token: 0x06002652 RID: 9810 RVA: 0x000CCADE File Offset: 0x000CACDE
	public void PlayButtonPress()
	{
		this.buttonBank.Play();
	}

	// Token: 0x06002653 RID: 9811 RVA: 0x000CCAEC File Offset: 0x000CACEC
	public void PlayElevatorMoving()
	{
		if (this.ambientAudio.isPlaying && this.ambientAudio.clip == this.travellingLoopClip)
		{
			return;
		}
		this.ambientAudio.clip = this.travellingLoopClip;
		this.ambientAudio.loop = true;
		this.ambientAudio.time = 0f;
		this.ambientAudio.Play();
	}

	// Token: 0x06002654 RID: 9812 RVA: 0x000CCB58 File Offset: 0x000CAD58
	public void PlayElevatorStopped()
	{
		if (this.ambientAudio.isPlaying && this.ambientAudio.clip == this.ambientLoopClip)
		{
			return;
		}
		this.ambientAudio.clip = this.ambientLoopClip;
		this.ambientAudio.loop = true;
		this.ambientAudio.time = 0f;
		this.ambientAudio.Play();
	}

	// Token: 0x06002655 RID: 9813 RVA: 0x000CCBC3 File Offset: 0x000CADC3
	public void PlayElevatorMusic(float time = 0f)
	{
		if (this.musicAudio.isPlaying)
		{
			return;
		}
		this.musicAudio.time = time;
		this.musicAudio.Play();
	}

	// Token: 0x06002656 RID: 9814 RVA: 0x000CCBEA File Offset: 0x000CADEA
	public void PlayDoorOpenBegin()
	{
		this.doorAudio.clip = this.doorOpenClip;
		this.doorAudio.time = 0f;
		this.doorAudio.Play();
	}

	// Token: 0x06002657 RID: 9815 RVA: 0x000CCC18 File Offset: 0x000CAE18
	public void PlayDoorCloseBegin()
	{
		this.doorAudio.clip = this.doorCloseClip;
		this.doorAudio.time = 0f;
		this.doorAudio.Play();
	}

	// Token: 0x06002658 RID: 9816 RVA: 0x000CCC46 File Offset: 0x000CAE46
	public void PlayDoorOpenTravel()
	{
		this.doorAudio.time = this.adjustedOffsetTime + this.openBeginDuration;
	}

	// Token: 0x06002659 RID: 9817 RVA: 0x000CCC60 File Offset: 0x000CAE60
	public void PlayDoorCloseTravel()
	{
		this.doorAudio.time = this.adjustedOffsetTime + this.closeBeginDuration;
	}

	// Token: 0x0600265A RID: 9818 RVA: 0x000CCC7C File Offset: 0x000CAE7C
	public bool DoorsFullyClosed()
	{
		return (this.upperDoor.position - this.closedTargetTop.position).sqrMagnitude < 0.0001f;
	}

	// Token: 0x0600265B RID: 9819 RVA: 0x000CCCB4 File Offset: 0x000CAEB4
	public bool DoorsFullyOpen()
	{
		return (this.upperDoor.position - this.openTargetTop.position).sqrMagnitude < 0.0001f;
	}

	// Token: 0x0600265C RID: 9820 RVA: 0x000CCCEC File Offset: 0x000CAEEC
	public void UpdateLocalState(GRElevator.ElevatorState newState)
	{
		if (newState == this.state)
		{
			return;
		}
		this.state = newState;
		switch (newState)
		{
		case GRElevator.ElevatorState.DoorBeginClosing:
			if (this.DoorsFullyClosed())
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorClosed);
				return;
			}
			this.doorMoveBeginTime = Time.time;
			this.SetDoorClosedBeginTime();
			this.PlayDoorCloseBegin();
			return;
		case GRElevator.ElevatorState.DoorMovingClosing:
			this.PlayDoorCloseTravel();
			return;
		case GRElevator.ElevatorState.DoorEndClosing:
		case GRElevator.ElevatorState.DoorEndOpening:
			break;
		case GRElevator.ElevatorState.DoorClosed:
			this.upperDoor.position = this.closedTargetTop.position;
			this.lowerDoor.position = this.closedTargetBottom.position;
			return;
		case GRElevator.ElevatorState.DoorBeginOpening:
			if (this.DoorsFullyOpen())
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorOpen);
				return;
			}
			this.doorMoveBeginTime = Time.time;
			this.SetDoorOpenBeginTime();
			this.PlayDoorOpenBegin();
			return;
		case GRElevator.ElevatorState.DoorMovingOpening:
			this.PlayDoorOpenTravel();
			return;
		case GRElevator.ElevatorState.DoorOpen:
			this.upperDoor.position = this.openTargetTop.position;
			this.lowerDoor.position = this.openTargetBottom.position;
			break;
		default:
			return;
		}
	}

	// Token: 0x0600265D RID: 9821 RVA: 0x000CCDE8 File Offset: 0x000CAFE8
	public void UpdateRemoteState(GRElevator.ElevatorState remoteNewState)
	{
		if (GRElevator.StateIsOpeningState(remoteNewState) && GRElevator.StateIsClosingState(this.state))
		{
			this.UpdateLocalState(GRElevator.ElevatorState.DoorBeginOpening);
			return;
		}
		if (GRElevator.StateIsClosingState(remoteNewState) && GRElevator.StateIsOpeningState(this.state))
		{
			this.UpdateLocalState(GRElevator.ElevatorState.DoorBeginClosing);
		}
	}

	// Token: 0x0600265E RID: 9822 RVA: 0x000CCE24 File Offset: 0x000CB024
	public void SetDoorOpenBeginTime()
	{
		float num = (this.travelDistance - (this.upperDoor.position - this.openTargetTop.position).magnitude) / this.travelDistance;
		this.adjustedOffsetTime = num * this.openTravelDuration;
	}

	// Token: 0x0600265F RID: 9823 RVA: 0x000CCE74 File Offset: 0x000CB074
	public void SetDoorClosedBeginTime()
	{
		float num = (this.travelDistance - (this.upperDoor.position - this.closedTargetTop.position).magnitude) / this.travelDistance;
		this.adjustedOffsetTime = num * this.closeTravelDuration;
	}

	// Token: 0x06002660 RID: 9824 RVA: 0x000CCEC1 File Offset: 0x000CB0C1
	public static bool StateIsOpeningState(GRElevator.ElevatorState checkState)
	{
		return checkState == GRElevator.ElevatorState.DoorMovingOpening || checkState == GRElevator.ElevatorState.DoorBeginOpening || checkState == GRElevator.ElevatorState.DoorEndOpening || checkState == GRElevator.ElevatorState.DoorOpen;
	}

	// Token: 0x06002661 RID: 9825 RVA: 0x000CCED5 File Offset: 0x000CB0D5
	public static bool StateIsClosingState(GRElevator.ElevatorState checkState)
	{
		return checkState == GRElevator.ElevatorState.DoorMovingClosing || checkState == GRElevator.ElevatorState.DoorBeginClosing || checkState == GRElevator.ElevatorState.DoorEndClosing || checkState == GRElevator.ElevatorState.DoorClosed;
	}

	// Token: 0x06002662 RID: 9826 RVA: 0x000CCEE8 File Offset: 0x000CB0E8
	public bool DoorIsOpening()
	{
		return GRElevator.StateIsOpeningState(this.state);
	}

	// Token: 0x06002663 RID: 9827 RVA: 0x000CCEF5 File Offset: 0x000CB0F5
	public bool DoorIsClosing()
	{
		return GRElevator.StateIsClosingState(this.state);
	}

	// Token: 0x06002664 RID: 9828 RVA: 0x000CCF04 File Offset: 0x000CB104
	public void PhysicalElevatorUpdate()
	{
		switch (this.state)
		{
		case GRElevator.ElevatorState.DoorBeginClosing:
			if (Time.time > this.doorMoveBeginTime + this.closeBeginDuration)
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorMovingClosing);
			}
			break;
		case GRElevator.ElevatorState.DoorMovingClosing:
			if (Time.time > this.doorMoveBeginTime - this.adjustedOffsetTime + this.closeBeginDuration + this.closeTravelDuration)
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorEndClosing);
			}
			break;
		case GRElevator.ElevatorState.DoorEndClosing:
			if (Time.time > this.doorMoveBeginTime - this.adjustedOffsetTime + this.closeBeginDuration + this.closeTravelDuration + this.closeEndDuration)
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorClosed);
			}
			break;
		case GRElevator.ElevatorState.DoorBeginOpening:
			if (Time.time > this.doorMoveBeginTime + this.openBeginDuration)
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorMovingOpening);
			}
			break;
		case GRElevator.ElevatorState.DoorMovingOpening:
			if (Time.time > this.doorMoveBeginTime - this.adjustedOffsetTime + this.openBeginDuration + this.openTravelDuration)
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorEndOpening);
			}
			break;
		case GRElevator.ElevatorState.DoorEndOpening:
			if (Time.time > this.doorMoveBeginTime - this.adjustedOffsetTime + this.openBeginDuration + this.openTravelDuration + this.openEndDuration)
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorOpen);
			}
			break;
		}
		GRElevator.ElevatorState elevatorState = this.state;
		Transform transform;
		Transform transform2;
		float num;
		if (elevatorState != GRElevator.ElevatorState.DoorMovingClosing)
		{
			if (elevatorState == GRElevator.ElevatorState.DoorMovingOpening)
			{
				transform = this.openTargetTop;
				transform2 = this.openTargetBottom;
				num = this.doorOpenSpeed;
			}
			else
			{
				transform = this.upperDoor;
				transform2 = this.lowerDoor;
				num = 1f;
			}
		}
		else
		{
			transform = this.closedTargetTop;
			transform2 = this.closedTargetBottom;
			num = this.doorCloseSpeed;
		}
		this.upperDoor.position = Vector3.MoveTowards(this.upperDoor.position, transform.position, Time.deltaTime * num);
		this.lowerDoor.position = Vector3.MoveTowards(this.lowerDoor.position, transform2.position, Time.deltaTime * num);
	}

	// Token: 0x04003095 RID: 12437
	public GRElevatorManager.ElevatorLocation location;

	// Token: 0x04003096 RID: 12438
	public Transform upperDoor;

	// Token: 0x04003097 RID: 12439
	public Transform lowerDoor;

	// Token: 0x04003098 RID: 12440
	public Transform closedTargetTop;

	// Token: 0x04003099 RID: 12441
	public Transform closedTargetBottom;

	// Token: 0x0400309A RID: 12442
	public Transform openTargetTop;

	// Token: 0x0400309B RID: 12443
	public Transform openTargetBottom;

	// Token: 0x0400309C RID: 12444
	public TextMeshPro outerText;

	// Token: 0x0400309D RID: 12445
	public TextMeshPro innerText;

	// Token: 0x0400309E RID: 12446
	public List<GRElevatorButton> elevatorButtons;

	// Token: 0x0400309F RID: 12447
	private Dictionary<GRElevator.ButtonType, GRElevatorButton> typeButtonDict;

	// Token: 0x040030A0 RID: 12448
	public GorillaFriendCollider friendCollider;

	// Token: 0x040030A1 RID: 12449
	public GorillaNetworkJoinTrigger joinTrigger;

	// Token: 0x040030A2 RID: 12450
	public SoundBankPlayer buttonBank;

	// Token: 0x040030A3 RID: 12451
	public AudioSource doorAudio;

	// Token: 0x040030A4 RID: 12452
	public AudioSource ambientAudio;

	// Token: 0x040030A5 RID: 12453
	public AudioSource musicAudio;

	// Token: 0x040030A6 RID: 12454
	public AudioClip travellingLoopClip;

	// Token: 0x040030A7 RID: 12455
	public AudioClip ambientLoopClip;

	// Token: 0x040030A8 RID: 12456
	public AudioClip dingClip;

	// Token: 0x040030A9 RID: 12457
	public AudioClip doorOpenClip;

	// Token: 0x040030AA RID: 12458
	public AudioClip doorCloseClip;

	// Token: 0x040030AB RID: 12459
	public float adjustedOffsetTime;

	// Token: 0x040030AC RID: 12460
	public float doorMoveBeginTime;

	// Token: 0x040030AD RID: 12461
	public float doorOpenSpeed = 0.5f;

	// Token: 0x040030AE RID: 12462
	public float doorCloseSpeed = 0.5f;

	// Token: 0x040030AF RID: 12463
	public float closeBeginDuration;

	// Token: 0x040030B0 RID: 12464
	public float closeTravelDuration;

	// Token: 0x040030B1 RID: 12465
	public float closeEndDuration;

	// Token: 0x040030B2 RID: 12466
	public float openBeginDuration;

	// Token: 0x040030B3 RID: 12467
	public float openTravelDuration;

	// Token: 0x040030B4 RID: 12468
	public float openEndDuration;

	// Token: 0x040030B5 RID: 12469
	public float travelDistance;

	// Token: 0x040030B6 RID: 12470
	public GRElevator.ElevatorState state;

	// Token: 0x040030B7 RID: 12471
	public GameObject collidersAndVisuals;

	// Token: 0x0200061D RID: 1565
	public enum ElevatorState
	{
		// Token: 0x040030B9 RID: 12473
		DoorBeginClosing,
		// Token: 0x040030BA RID: 12474
		DoorMovingClosing,
		// Token: 0x040030BB RID: 12475
		DoorEndClosing,
		// Token: 0x040030BC RID: 12476
		DoorClosed,
		// Token: 0x040030BD RID: 12477
		DoorBeginOpening,
		// Token: 0x040030BE RID: 12478
		DoorMovingOpening,
		// Token: 0x040030BF RID: 12479
		DoorEndOpening,
		// Token: 0x040030C0 RID: 12480
		DoorOpen,
		// Token: 0x040030C1 RID: 12481
		None
	}

	// Token: 0x0200061E RID: 1566
	[Serializable]
	public enum ButtonType
	{
		// Token: 0x040030C3 RID: 12483
		Stump = 1,
		// Token: 0x040030C4 RID: 12484
		City,
		// Token: 0x040030C5 RID: 12485
		GhostReactor,
		// Token: 0x040030C6 RID: 12486
		Open,
		// Token: 0x040030C7 RID: 12487
		Close,
		// Token: 0x040030C8 RID: 12488
		Summon,
		// Token: 0x040030C9 RID: 12489
		Count
	}
}
