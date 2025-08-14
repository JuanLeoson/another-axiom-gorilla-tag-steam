using System;
using UnityEngine;

// Token: 0x0200043E RID: 1086
public interface IHoldableObject
{
	// Token: 0x170002EB RID: 747
	// (get) Token: 0x06001A7A RID: 6778
	GameObject gameObject { get; }

	// Token: 0x170002EC RID: 748
	// (get) Token: 0x06001A7B RID: 6779
	// (set) Token: 0x06001A7C RID: 6780
	string name { get; set; }

	// Token: 0x170002ED RID: 749
	// (get) Token: 0x06001A7D RID: 6781
	bool TwoHanded { get; }

	// Token: 0x06001A7E RID: 6782
	void OnHover(InteractionPoint pointHovered, GameObject hoveringHand);

	// Token: 0x06001A7F RID: 6783
	void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand);

	// Token: 0x06001A80 RID: 6784
	bool OnRelease(DropZone zoneReleased, GameObject releasingHand);

	// Token: 0x06001A81 RID: 6785
	void DropItemCleanup();
}
