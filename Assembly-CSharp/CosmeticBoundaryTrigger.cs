using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200041B RID: 1051
public class CosmeticBoundaryTrigger : GorillaTriggerBox
{
	// Token: 0x0600198A RID: 6538 RVA: 0x0008982C File Offset: 0x00087A2C
	public void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody == null)
		{
			return;
		}
		this.rigRef = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (this.rigRef == null)
		{
			return;
		}
		if (CosmeticBoundaryTrigger.sinceLastTryOnEvent.HasElapsed(0.5f, true))
		{
			GorillaTelemetry.PostShopEvent(this.rigRef, GTShopEventType.item_try_on, this.rigRef.tryOnSet.items);
		}
		this.rigRef.inTryOnRoom = true;
		this.rigRef.LocalUpdateCosmeticsWithTryon(this.rigRef.cosmeticSet, this.rigRef.tryOnSet);
		this.rigRef.myBodyDockPositions.RefreshTransferrableItems();
	}

	// Token: 0x0600198B RID: 6539 RVA: 0x000898D8 File Offset: 0x00087AD8
	public void OnTriggerExit(Collider other)
	{
		if (other.attachedRigidbody == null)
		{
			return;
		}
		this.rigRef = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (this.rigRef == null)
		{
			return;
		}
		this.rigRef.inTryOnRoom = false;
		if (this.rigRef.isOfflineVRRig)
		{
			this.rigRef.tryOnSet.ClearSet(CosmeticsController.instance.nullItem);
			CosmeticsController.instance.ClearCheckout(false);
			CosmeticsController.instance.UpdateShoppingCart();
			CosmeticsController.instance.UpdateWornCosmetics(true);
		}
		this.rigRef.LocalUpdateCosmeticsWithTryon(this.rigRef.cosmeticSet, this.rigRef.tryOnSet);
		this.rigRef.myBodyDockPositions.RefreshTransferrableItems();
	}

	// Token: 0x040021E4 RID: 8676
	public VRRig rigRef;

	// Token: 0x040021E5 RID: 8677
	private static TimeSince sinceLastTryOnEvent = 0f;
}
