using System;
using Fusion;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000449 RID: 1097
[NetworkBehaviourWeaved(0)]
public abstract class NetworkHoldableObject : NetworkComponent, IHoldableObject
{
	// Token: 0x170002EF RID: 751
	// (get) Token: 0x06001AED RID: 6893 RVA: 0x00002076 File Offset: 0x00000276
	public virtual bool TwoHanded
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06001AEE RID: 6894
	public abstract void OnHover(InteractionPoint pointHovered, GameObject hoveringHand);

	// Token: 0x06001AEF RID: 6895
	public abstract void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand);

	// Token: 0x06001AF0 RID: 6896
	public abstract void DropItemCleanup();

	// Token: 0x06001AF1 RID: 6897 RVA: 0x0008FA40 File Offset: 0x0008DC40
	public virtual bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		return (EquipmentInteractor.instance.rightHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.rightHand)) && (EquipmentInteractor.instance.leftHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.leftHand));
	}

	// Token: 0x06001AF2 RID: 6898 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void ReadDataFusion()
	{
	}

	// Token: 0x06001AF3 RID: 6899 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void WriteDataFusion()
	{
	}

	// Token: 0x06001AF4 RID: 6900 RVA: 0x000023F5 File Offset: 0x000005F5
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06001AF5 RID: 6901 RVA: 0x000023F5 File Offset: 0x000005F5
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06001AF7 RID: 6903 RVA: 0x0001399F File Offset: 0x00011B9F
	GameObject IHoldableObject.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x06001AF8 RID: 6904 RVA: 0x000139A7 File Offset: 0x00011BA7
	string IHoldableObject.get_name()
	{
		return base.name;
	}

	// Token: 0x06001AF9 RID: 6905 RVA: 0x000139AF File Offset: 0x00011BAF
	void IHoldableObject.set_name(string value)
	{
		base.name = value;
	}

	// Token: 0x06001AFA RID: 6906 RVA: 0x00002637 File Offset: 0x00000837
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06001AFB RID: 6907 RVA: 0x00002643 File Offset: 0x00000843
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}
}
