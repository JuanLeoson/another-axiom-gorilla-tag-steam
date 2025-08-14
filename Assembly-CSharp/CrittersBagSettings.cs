using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200004A RID: 74
public class CrittersBagSettings : CrittersActorSettings
{
	// Token: 0x0600016A RID: 362 RVA: 0x000096F4 File Offset: 0x000078F4
	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersBag crittersBag = (CrittersBag)this.parentActor;
		crittersBag.attachableCollider = this.attachableCollider;
		crittersBag.dropCube = this.dropCube;
		crittersBag.anchorLocation = this.anchorLocation;
		crittersBag.attachDisableColliders = this.attachDisableColliders;
		crittersBag.attachSound = this.attachSound;
		crittersBag.detachSound = this.detachSound;
		crittersBag.blockAttachTypes = this.blockAttachTypes;
	}

	// Token: 0x0400019C RID: 412
	public Collider attachableCollider;

	// Token: 0x0400019D RID: 413
	public BoxCollider dropCube;

	// Token: 0x0400019E RID: 414
	public CrittersAttachPoint.AnchoredLocationTypes anchorLocation;

	// Token: 0x0400019F RID: 415
	public List<Collider> attachDisableColliders;

	// Token: 0x040001A0 RID: 416
	public AudioClip attachSound;

	// Token: 0x040001A1 RID: 417
	public AudioClip detachSound;

	// Token: 0x040001A2 RID: 418
	public List<CrittersActor.CrittersActorType> blockAttachTypes;
}
