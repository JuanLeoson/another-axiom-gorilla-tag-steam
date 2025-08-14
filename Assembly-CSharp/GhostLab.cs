using System;
using UnityEngine;

// Token: 0x020000D0 RID: 208
public class GhostLab : MonoBehaviour, IBuildValidation
{
	// Token: 0x06000515 RID: 1301 RVA: 0x0001D52E File Offset: 0x0001B72E
	private void Awake()
	{
		this.relState = Object.FindFirstObjectByType<GhostLabReliableState>();
		this.doorState = GhostLab.EntranceDoorsState.BothClosed;
		this.doorOpen = new bool[this.relState.singleDoorCount];
	}

	// Token: 0x06000516 RID: 1302 RVA: 0x0001D558 File Offset: 0x0001B758
	public bool BuildValidationCheck()
	{
		return true;
	}

	// Token: 0x06000517 RID: 1303 RVA: 0x0001D55B File Offset: 0x0001B75B
	public void DoorButtonPress(int buttonIndex, bool forSingleDoor)
	{
		if (!forSingleDoor)
		{
			this.UpdateEntranceDoorsState(buttonIndex);
			return;
		}
		this.UpdateDoorState(buttonIndex);
		this.relState.UpdateSingleDoorState(buttonIndex);
	}

	// Token: 0x06000518 RID: 1304 RVA: 0x0001D57C File Offset: 0x0001B77C
	public void UpdateDoorState(int buttonIndex)
	{
		if ((this.doorOpen[buttonIndex] && this.slidingDoor[buttonIndex].localPosition == this.singleDoorTravelDistance) || (!this.doorOpen[buttonIndex] && this.slidingDoor[buttonIndex].localPosition == Vector3.zero))
		{
			this.doorOpen[buttonIndex] = !this.doorOpen[buttonIndex];
		}
	}

	// Token: 0x06000519 RID: 1305 RVA: 0x0001D5E4 File Offset: 0x0001B7E4
	public void UpdateEntranceDoorsState(int buttonIndex)
	{
		if (this.outerDoor == null || this.innerDoor == null)
		{
			return;
		}
		if (this.doorState == GhostLab.EntranceDoorsState.BothClosed)
		{
			if (!(this.innerDoor.localPosition != Vector3.zero) && !(this.outerDoor.localPosition != Vector3.zero))
			{
				if (buttonIndex == 0 || buttonIndex == 1)
				{
					this.doorState = GhostLab.EntranceDoorsState.OuterDoorOpen;
				}
				if (buttonIndex == 2 || buttonIndex == 3)
				{
					this.doorState = GhostLab.EntranceDoorsState.InnerDoorOpen;
				}
			}
		}
		else if (this.innerDoor.localPosition == this.doorTravelDistance || this.outerDoor.localPosition == this.doorTravelDistance)
		{
			this.doorState = GhostLab.EntranceDoorsState.BothClosed;
		}
		this.relState.UpdateEntranceDoorsState(this.doorState);
	}

	// Token: 0x0600051A RID: 1306 RVA: 0x0001D6AC File Offset: 0x0001B8AC
	public void Update()
	{
		this.SynchStates();
		if (this.innerDoor != null && this.outerDoor != null)
		{
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			switch (this.doorState)
			{
			case GhostLab.EntranceDoorsState.InnerDoorOpen:
				zero2 = this.doorTravelDistance;
				break;
			case GhostLab.EntranceDoorsState.OuterDoorOpen:
				zero = this.doorTravelDistance;
				break;
			}
			this.outerDoor.localPosition = Vector3.MoveTowards(this.outerDoor.localPosition, zero, this.doorMoveSpeed * Time.deltaTime);
			this.innerDoor.localPosition = Vector3.MoveTowards(this.innerDoor.localPosition, zero2, this.doorMoveSpeed * Time.deltaTime);
		}
		Vector3 zero3 = Vector3.zero;
		for (int i = 0; i < this.slidingDoor.Length; i++)
		{
			if (this.doorOpen[i])
			{
				zero3 = this.singleDoorTravelDistance;
			}
			else
			{
				zero3 = Vector3.zero;
			}
			this.slidingDoor[i].localPosition = Vector3.MoveTowards(this.slidingDoor[i].localPosition, zero3, this.singleDoorMoveSpeed * Time.deltaTime);
		}
	}

	// Token: 0x0600051B RID: 1307 RVA: 0x0001D7D0 File Offset: 0x0001B9D0
	private void SynchStates()
	{
		this.doorState = this.relState.doorState;
		for (int i = 0; i < this.doorOpen.Length; i++)
		{
			this.doorOpen[i] = this.relState.singleDoorOpen[i];
		}
	}

	// Token: 0x0600051C RID: 1308 RVA: 0x0001D818 File Offset: 0x0001BA18
	public bool IsDoorMoving(bool singleDoor, int index)
	{
		if (singleDoor)
		{
			return (this.doorOpen[index] && this.slidingDoor[index].localPosition != this.singleDoorTravelDistance) || (!this.doorOpen[index] && this.slidingDoor[index].localPosition != Vector3.zero);
		}
		if (index == 0 || index == 1)
		{
			return (this.doorState == GhostLab.EntranceDoorsState.OuterDoorOpen && this.outerDoor.localPosition != this.doorTravelDistance) || (this.doorState != GhostLab.EntranceDoorsState.OuterDoorOpen && this.outerDoor.localPosition != Vector3.zero);
		}
		return (this.doorState == GhostLab.EntranceDoorsState.InnerDoorOpen && this.innerDoor.localPosition != this.doorTravelDistance) || (this.doorState != GhostLab.EntranceDoorsState.InnerDoorOpen && this.innerDoor.localPosition != Vector3.zero);
	}

	// Token: 0x0400060F RID: 1551
	public Transform outerDoor;

	// Token: 0x04000610 RID: 1552
	public Transform innerDoor;

	// Token: 0x04000611 RID: 1553
	public Vector3 doorTravelDistance;

	// Token: 0x04000612 RID: 1554
	public float doorMoveSpeed;

	// Token: 0x04000613 RID: 1555
	public float singleDoorMoveSpeed;

	// Token: 0x04000614 RID: 1556
	public GhostLab.EntranceDoorsState doorState;

	// Token: 0x04000615 RID: 1557
	public GhostLabReliableState relState;

	// Token: 0x04000616 RID: 1558
	public Transform[] slidingDoor;

	// Token: 0x04000617 RID: 1559
	public Vector3 singleDoorTravelDistance;

	// Token: 0x04000618 RID: 1560
	private bool[] doorOpen;

	// Token: 0x020000D1 RID: 209
	public enum EntranceDoorsState
	{
		// Token: 0x0400061A RID: 1562
		BothClosed,
		// Token: 0x0400061B RID: 1563
		InnerDoorOpen,
		// Token: 0x0400061C RID: 1564
		OuterDoorOpen
	}
}
