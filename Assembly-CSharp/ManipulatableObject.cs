using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000443 RID: 1091
public class ManipulatableObject : HoldableObject
{
	// Token: 0x06001AB8 RID: 6840 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void OnStartManipulation(GameObject grabbingHand)
	{
	}

	// Token: 0x06001AB9 RID: 6841 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void OnStopManipulation(GameObject releasingHand, Vector3 releaseVelocity)
	{
	}

	// Token: 0x06001ABA RID: 6842 RVA: 0x00002076 File Offset: 0x00000276
	protected virtual bool ShouldHandDetach(GameObject hand)
	{
		return false;
	}

	// Token: 0x06001ABB RID: 6843 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void OnHeldUpdate(GameObject hand)
	{
	}

	// Token: 0x06001ABC RID: 6844 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void OnReleasedUpdate()
	{
	}

	// Token: 0x06001ABD RID: 6845 RVA: 0x0008EAD4 File Offset: 0x0008CCD4
	public virtual void LateUpdate()
	{
		if (this.isHeld)
		{
			if (this.holdingHand == null)
			{
				EquipmentInteractor.instance.ForceDropManipulatableObject(this);
				return;
			}
			this.OnHeldUpdate(this.holdingHand);
			if (this.ShouldHandDetach(this.holdingHand))
			{
				EquipmentInteractor.instance.ForceDropManipulatableObject(this);
				return;
			}
		}
		else
		{
			this.OnReleasedUpdate();
		}
	}

	// Token: 0x06001ABE RID: 6846 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06001ABF RID: 6847 RVA: 0x0008EB34 File Offset: 0x0008CD34
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		bool forLeftHand = grabbingHand == EquipmentInteractor.instance.leftHand;
		EquipmentInteractor.instance.UpdateHandEquipment(this, forLeftHand);
		this.isHeld = true;
		this.holdingHand = grabbingHand;
		this.OnStartManipulation(this.holdingHand);
	}

	// Token: 0x06001AC0 RID: 6848 RVA: 0x0008EB7C File Offset: 0x0008CD7C
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		bool flag = releasingHand == EquipmentInteractor.instance.leftHand;
		Vector3 releaseVelocity = Vector3.zero;
		if (flag)
		{
			releaseVelocity = GTPlayer.Instance.leftHandCenterVelocityTracker.GetAverageVelocity(true, 0.15f, false);
			EquipmentInteractor.instance.leftHandHeldEquipment = null;
		}
		else
		{
			releaseVelocity = GTPlayer.Instance.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0.15f, false);
			EquipmentInteractor.instance.rightHandHeldEquipment = null;
		}
		this.isHeld = false;
		this.holdingHand = null;
		this.OnStopManipulation(releasingHand, releaseVelocity);
		return true;
	}

	// Token: 0x06001AC1 RID: 6849 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void DropItemCleanup()
	{
	}

	// Token: 0x040022FF RID: 8959
	protected bool isHeld;

	// Token: 0x04002300 RID: 8960
	protected GameObject holdingHand;
}
