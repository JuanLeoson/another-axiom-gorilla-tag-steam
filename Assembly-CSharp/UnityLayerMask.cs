using System;

// Token: 0x0200025A RID: 602
[Flags]
public enum UnityLayerMask
{
	// Token: 0x04001604 RID: 5636
	Everything = -1,
	// Token: 0x04001605 RID: 5637
	Nothing = 0,
	// Token: 0x04001606 RID: 5638
	Default = 1,
	// Token: 0x04001607 RID: 5639
	TransparentFX = 2,
	// Token: 0x04001608 RID: 5640
	IgnoreRaycast = 4,
	// Token: 0x04001609 RID: 5641
	Zone = 8,
	// Token: 0x0400160A RID: 5642
	Water = 16,
	// Token: 0x0400160B RID: 5643
	UI = 32,
	// Token: 0x0400160C RID: 5644
	MeshBakerAtlas = 64,
	// Token: 0x0400160D RID: 5645
	GorillaEquipment = 128,
	// Token: 0x0400160E RID: 5646
	GorillaBodyCollider = 256,
	// Token: 0x0400160F RID: 5647
	GorillaObject = 512,
	// Token: 0x04001610 RID: 5648
	GorillaHand = 1024,
	// Token: 0x04001611 RID: 5649
	GorillaTrigger = 2048,
	// Token: 0x04001612 RID: 5650
	MetaReportScreen = 4096,
	// Token: 0x04001613 RID: 5651
	GorillaHead = 8192,
	// Token: 0x04001614 RID: 5652
	GorillaTagCollider = 16384,
	// Token: 0x04001615 RID: 5653
	GorillaBoundary = 32768,
	// Token: 0x04001616 RID: 5654
	GorillaEquipmentContainer = 65536,
	// Token: 0x04001617 RID: 5655
	LCKHide = 131072,
	// Token: 0x04001618 RID: 5656
	GorillaInteractable = 262144,
	// Token: 0x04001619 RID: 5657
	FirstPersonOnly = 524288,
	// Token: 0x0400161A RID: 5658
	GorillaParticle = 1048576,
	// Token: 0x0400161B RID: 5659
	GorillaCosmetics = 2097152,
	// Token: 0x0400161C RID: 5660
	MirrorOnly = 4194304,
	// Token: 0x0400161D RID: 5661
	GorillaThrowable = 8388608,
	// Token: 0x0400161E RID: 5662
	GorillaHandSocket = 16777216,
	// Token: 0x0400161F RID: 5663
	GorillaCosmeticParticle = 33554432,
	// Token: 0x04001620 RID: 5664
	BuilderProp = 67108864,
	// Token: 0x04001621 RID: 5665
	NoMirror = 134217728,
	// Token: 0x04001622 RID: 5666
	GorillaSlingshotCollider = 268435456,
	// Token: 0x04001623 RID: 5667
	RopeSwing = 536870912,
	// Token: 0x04001624 RID: 5668
	Prop = 1073741824,
	// Token: 0x04001625 RID: 5669
	Bake = -2147483648
}
