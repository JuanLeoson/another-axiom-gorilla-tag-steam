using System;
using UnityEngine;

// Token: 0x0200043D RID: 1085
public abstract class HoldableObject : MonoBehaviour, IHoldableObject
{
	// Token: 0x170002EA RID: 746
	// (get) Token: 0x06001A70 RID: 6768 RVA: 0x00002076 File Offset: 0x00000276
	public virtual bool TwoHanded
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06001A71 RID: 6769 RVA: 0x0008D80F File Offset: 0x0008BA0F
	protected void OnDestroy()
	{
		if (EquipmentInteractor.hasInstance)
		{
			EquipmentInteractor.instance.ForceDropEquipment(this);
		}
	}

	// Token: 0x06001A72 RID: 6770
	public abstract void OnHover(InteractionPoint pointHovered, GameObject hoveringHand);

	// Token: 0x06001A73 RID: 6771
	public abstract void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand);

	// Token: 0x06001A74 RID: 6772
	public abstract void DropItemCleanup();

	// Token: 0x06001A75 RID: 6773 RVA: 0x0008D828 File Offset: 0x0008BA28
	public virtual bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		return (EquipmentInteractor.instance.rightHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.rightHand)) && (EquipmentInteractor.instance.leftHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.leftHand));
	}

	// Token: 0x06001A77 RID: 6775 RVA: 0x0001399F File Offset: 0x00011B9F
	GameObject IHoldableObject.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x06001A78 RID: 6776 RVA: 0x000139A7 File Offset: 0x00011BA7
	string IHoldableObject.get_name()
	{
		return base.name;
	}

	// Token: 0x06001A79 RID: 6777 RVA: 0x000139AF File Offset: 0x00011BAF
	void IHoldableObject.set_name(string value)
	{
		base.name = value;
	}
}
