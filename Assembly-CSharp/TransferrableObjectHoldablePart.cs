using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020003F3 RID: 1011
public class TransferrableObjectHoldablePart : HoldableObject
{
	// Token: 0x06001793 RID: 6035 RVA: 0x0007F150 File Offset: 0x0007D350
	private void Update()
	{
		VRRig rig;
		if (!this.transferrableParentObject.IsLocalObject())
		{
			rig = this.transferrableParentObject.myOnlineRig;
			this.isHeld = ((this.transferrableParentObject.itemState & this.heldBit) > (TransferrableObject.ItemStates)0);
			TransferrableObject.PositionState currentState = this.transferrableParentObject.currentState;
			if (currentState == TransferrableObject.PositionState.OnRightArm || currentState == TransferrableObject.PositionState.InRightHand)
			{
				this.isHeldLeftHand = this.isHeld;
			}
			else
			{
				this.isHeldLeftHand = false;
			}
		}
		else
		{
			rig = VRRig.LocalRig;
		}
		if (this.isHeld)
		{
			if (this.transferrableParentObject.InHand())
			{
				this.UpdateHeld(rig, this.isHeldLeftHand);
				return;
			}
			if (this.transferrableParentObject.IsLocalObject())
			{
				this.OnRelease(null, this.isHeldLeftHand ? EquipmentInteractor.instance.leftHand : EquipmentInteractor.instance.rightHand);
			}
		}
	}

	// Token: 0x06001794 RID: 6036 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void UpdateHeld(VRRig rig, bool isHeldLeftHand)
	{
	}

	// Token: 0x06001795 RID: 6037 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06001796 RID: 6038 RVA: 0x0007F21C File Offset: 0x0007D41C
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (this.transferrableParentObject.ownerRig && !this.transferrableParentObject.ownerRig.isLocal)
		{
			return;
		}
		this.isHeld = true;
		this.isHeldLeftHand = (grabbingHand == EquipmentInteractor.instance.leftHand);
		this.transferrableParentObject.itemState |= this.heldBit;
		EquipmentInteractor.instance.UpdateHandEquipment(this, this.isHeldLeftHand);
		UnityEvent unityEvent = this.onGrab;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x06001797 RID: 6039 RVA: 0x0007F2A8 File Offset: 0x0007D4A8
	public override void DropItemCleanup()
	{
		this.isHeld = false;
		this.isHeldLeftHand = false;
		this.transferrableParentObject.itemState &= ~this.heldBit;
	}

	// Token: 0x06001798 RID: 6040 RVA: 0x0007F2D4 File Offset: 0x0007D4D4
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (EquipmentInteractor.instance.rightHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.rightHand)
		{
			return false;
		}
		if (EquipmentInteractor.instance.leftHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.leftHand)
		{
			return false;
		}
		EquipmentInteractor.instance.UpdateHandEquipment(null, this.isHeldLeftHand);
		this.isHeld = false;
		this.isHeldLeftHand = false;
		this.transferrableParentObject.itemState &= ~this.heldBit;
		UnityEvent unityEvent = this.onRelease;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		return true;
	}

	// Token: 0x04001F75 RID: 8053
	[SerializeField]
	protected TransferrableObject transferrableParentObject;

	// Token: 0x04001F76 RID: 8054
	[SerializeField]
	private TransferrableObject.ItemStates heldBit = TransferrableObject.ItemStates.Part0Held;

	// Token: 0x04001F77 RID: 8055
	private bool isHeld;

	// Token: 0x04001F78 RID: 8056
	protected bool isHeldLeftHand;

	// Token: 0x04001F79 RID: 8057
	public UnityEvent onGrab;

	// Token: 0x04001F7A RID: 8058
	public UnityEvent onRelease;

	// Token: 0x04001F7B RID: 8059
	public UnityEvent onDrop;
}
