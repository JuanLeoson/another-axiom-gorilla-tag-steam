using System;

// Token: 0x0200025C RID: 604
public enum UnityTag
{
	// Token: 0x04001627 RID: 5671
	Invalid = -1,
	// Token: 0x04001628 RID: 5672
	Untagged,
	// Token: 0x04001629 RID: 5673
	Respawn,
	// Token: 0x0400162A RID: 5674
	Finish,
	// Token: 0x0400162B RID: 5675
	EditorOnly,
	// Token: 0x0400162C RID: 5676
	MainCamera,
	// Token: 0x0400162D RID: 5677
	Player,
	// Token: 0x0400162E RID: 5678
	GameController,
	// Token: 0x0400162F RID: 5679
	SceneChanger,
	// Token: 0x04001630 RID: 5680
	PlayerOffset,
	// Token: 0x04001631 RID: 5681
	GorillaTagManager,
	// Token: 0x04001632 RID: 5682
	GorillaTagCollider,
	// Token: 0x04001633 RID: 5683
	GorillaPlayer,
	// Token: 0x04001634 RID: 5684
	GorillaObject,
	// Token: 0x04001635 RID: 5685
	GorillaGameManager,
	// Token: 0x04001636 RID: 5686
	GorillaCosmetic,
	// Token: 0x04001637 RID: 5687
	projectile,
	// Token: 0x04001638 RID: 5688
	FxTemporaire,
	// Token: 0x04001639 RID: 5689
	SlingshotProjectile,
	// Token: 0x0400163A RID: 5690
	SlingshotProjectileTrail,
	// Token: 0x0400163B RID: 5691
	SlingshotProjectilePlayerImpactFX,
	// Token: 0x0400163C RID: 5692
	SlingshotProjectileSurfaceImpactFX,
	// Token: 0x0400163D RID: 5693
	BalloonPopFX,
	// Token: 0x0400163E RID: 5694
	WorldShareableItem,
	// Token: 0x0400163F RID: 5695
	HornsSlingshotProjectile,
	// Token: 0x04001640 RID: 5696
	HornsSlingshotProjectileTrail,
	// Token: 0x04001641 RID: 5697
	HornsSlingshotProjectilePlayerImpactFX,
	// Token: 0x04001642 RID: 5698
	HornsSlingshotProjectileSurfaceImpactFX,
	// Token: 0x04001643 RID: 5699
	FryingPan,
	// Token: 0x04001644 RID: 5700
	LeafPileImpactFX,
	// Token: 0x04001645 RID: 5701
	BalloonPopFx,
	// Token: 0x04001646 RID: 5702
	CloudSlingshotProjectile,
	// Token: 0x04001647 RID: 5703
	CloudSlingshotProjectileTrail,
	// Token: 0x04001648 RID: 5704
	CloudSlingshotProjectilePlayerImpactFX,
	// Token: 0x04001649 RID: 5705
	CloudSlingshotProjectileSurfaceImpactFX,
	// Token: 0x0400164A RID: 5706
	SnowballProjectile,
	// Token: 0x0400164B RID: 5707
	SnowballProjectileImpactFX,
	// Token: 0x0400164C RID: 5708
	CupidBowProjectile,
	// Token: 0x0400164D RID: 5709
	CupidBowProjectileTrail,
	// Token: 0x0400164E RID: 5710
	CupidBowProjectileSurfaceImpactFX,
	// Token: 0x0400164F RID: 5711
	NoCrazyCheck,
	// Token: 0x04001650 RID: 5712
	IceSlingshotProjectile,
	// Token: 0x04001651 RID: 5713
	IceSlingshotProjectileSurfaceImpactFX,
	// Token: 0x04001652 RID: 5714
	IceSlingshotProjectileTrail,
	// Token: 0x04001653 RID: 5715
	ElfBowProjectile,
	// Token: 0x04001654 RID: 5716
	ElfBowProjectileSurfaceImpactFX,
	// Token: 0x04001655 RID: 5717
	ElfBowProjectileTrail,
	// Token: 0x04001656 RID: 5718
	RenderIfSmall,
	// Token: 0x04001657 RID: 5719
	DeleteOnNonBetaBuild,
	// Token: 0x04001658 RID: 5720
	DeleteOnNonDebugBuild,
	// Token: 0x04001659 RID: 5721
	FlagColoringCauldon,
	// Token: 0x0400165A RID: 5722
	WaterRippleEffect,
	// Token: 0x0400165B RID: 5723
	WaterSplashEffect,
	// Token: 0x0400165C RID: 5724
	FireworkMortarProjectile,
	// Token: 0x0400165D RID: 5725
	FireworkMortarProjectileImpactFX,
	// Token: 0x0400165E RID: 5726
	WaterBalloonProjectile,
	// Token: 0x0400165F RID: 5727
	WaterBalloonProjectileImpactFX,
	// Token: 0x04001660 RID: 5728
	PlayerHeadTrigger,
	// Token: 0x04001661 RID: 5729
	WizardStaff,
	// Token: 0x04001662 RID: 5730
	LurkerGhost,
	// Token: 0x04001663 RID: 5731
	HauntedObject,
	// Token: 0x04001664 RID: 5732
	WanderingGhost,
	// Token: 0x04001665 RID: 5733
	LavaSurfaceRock,
	// Token: 0x04001666 RID: 5734
	LavaRockProjectile,
	// Token: 0x04001667 RID: 5735
	LavaRockProjectileImpactFX,
	// Token: 0x04001668 RID: 5736
	MoltenSlingshotProjectile,
	// Token: 0x04001669 RID: 5737
	MoltenSlingshotProjectileTrail,
	// Token: 0x0400166A RID: 5738
	MoltenSlingshotProjectileSurfaceImpactFX,
	// Token: 0x0400166B RID: 5739
	MoltenSlingshotProjectilePlayerImpactFX,
	// Token: 0x0400166C RID: 5740
	SpiderBowProjectile,
	// Token: 0x0400166D RID: 5741
	SpiderBowProjectileTrail,
	// Token: 0x0400166E RID: 5742
	SpiderBowProjectileSurfaceImpactFX,
	// Token: 0x0400166F RID: 5743
	SpiderBowProjectilePlayerImpactFX,
	// Token: 0x04001670 RID: 5744
	ZoneRoot,
	// Token: 0x04001671 RID: 5745
	DontProcessMaterials,
	// Token: 0x04001672 RID: 5746
	OrnamentProjectileSurfaceImpactFX,
	// Token: 0x04001673 RID: 5747
	BucketGiftCane,
	// Token: 0x04001674 RID: 5748
	BucketGiftCoal,
	// Token: 0x04001675 RID: 5749
	BucketGiftRoll,
	// Token: 0x04001676 RID: 5750
	BucketGiftRound,
	// Token: 0x04001677 RID: 5751
	BucketGiftSquare,
	// Token: 0x04001678 RID: 5752
	OrnamentProjectile,
	// Token: 0x04001679 RID: 5753
	OrnamentShatterFX,
	// Token: 0x0400167A RID: 5754
	ScienceCandyProjectile,
	// Token: 0x0400167B RID: 5755
	ScienceCandyImpactFX,
	// Token: 0x0400167C RID: 5756
	PaperAirplaneProjectile,
	// Token: 0x0400167D RID: 5757
	DevilBowProjectile,
	// Token: 0x0400167E RID: 5758
	DevilBowProjectileTrail,
	// Token: 0x0400167F RID: 5759
	DevilBowProjectileSurfaceImpactFX,
	// Token: 0x04001680 RID: 5760
	DevilBowProjectilePlayerImpactFX,
	// Token: 0x04001681 RID: 5761
	FireFX,
	// Token: 0x04001682 RID: 5762
	FishFood,
	// Token: 0x04001683 RID: 5763
	FishFoodImpactFX,
	// Token: 0x04001684 RID: 5764
	LeafNinjaStarProjectile,
	// Token: 0x04001685 RID: 5765
	LeafNinjaStarProjectileC1,
	// Token: 0x04001686 RID: 5766
	LeafNinjaStarProjectileC2,
	// Token: 0x04001687 RID: 5767
	SamuraiBowProjectile,
	// Token: 0x04001688 RID: 5768
	SamuraiBowProjectileTrail,
	// Token: 0x04001689 RID: 5769
	SamuraiBowProjectileSurfaceImpactFX,
	// Token: 0x0400168A RID: 5770
	SamuraiBowProjectilePlayerImpactFX,
	// Token: 0x0400168B RID: 5771
	DragonSlingProjectile,
	// Token: 0x0400168C RID: 5772
	DragonSlingProjectileTrail,
	// Token: 0x0400168D RID: 5773
	DragonSlingProjectileSurfaceImpactFX,
	// Token: 0x0400168E RID: 5774
	DragonSlingProjectilePlayerImpactFX,
	// Token: 0x0400168F RID: 5775
	FireballProjectile,
	// Token: 0x04001690 RID: 5776
	StealthHandTapFX,
	// Token: 0x04001691 RID: 5777
	EnvPieceTree01,
	// Token: 0x04001692 RID: 5778
	FxSnapPiecePlaced,
	// Token: 0x04001693 RID: 5779
	FxSnapPieceDisconnected,
	// Token: 0x04001694 RID: 5780
	FxSnapPieceGrabbed,
	// Token: 0x04001695 RID: 5781
	FxSnapPieceLocationLock,
	// Token: 0x04001696 RID: 5782
	CyberNinjaStarProjectile,
	// Token: 0x04001697 RID: 5783
	RoomLight,
	// Token: 0x04001698 RID: 5784
	SamplesInfoPanel,
	// Token: 0x04001699 RID: 5785
	GorillaHandLeft,
	// Token: 0x0400169A RID: 5786
	GorillaHandRight,
	// Token: 0x0400169B RID: 5787
	GorillaHandSocket,
	// Token: 0x0400169C RID: 5788
	PlayingCardProjectile,
	// Token: 0x0400169D RID: 5789
	RottenPumpkinProjectile,
	// Token: 0x0400169E RID: 5790
	FxSnapPieceRecycle,
	// Token: 0x0400169F RID: 5791
	FxSnapPieceDispenser,
	// Token: 0x040016A0 RID: 5792
	AppleProjectile,
	// Token: 0x040016A1 RID: 5793
	AppleProjectileSurfaceImpactFX,
	// Token: 0x040016A2 RID: 5794
	RecyclerForceVolumeFX,
	// Token: 0x040016A3 RID: 5795
	FxSnapPieceTooHeavy,
	// Token: 0x040016A4 RID: 5796
	FxBuilderPrivatePlotClaimed,
	// Token: 0x040016A5 RID: 5797
	TrickTreatCandy,
	// Token: 0x040016A6 RID: 5798
	TrickTreatEyeball,
	// Token: 0x040016A7 RID: 5799
	TrickTreatBat,
	// Token: 0x040016A8 RID: 5800
	TrickTreatBomb,
	// Token: 0x040016A9 RID: 5801
	TrickTreatSurfaceImpact,
	// Token: 0x040016AA RID: 5802
	TrickTreatBatImpact,
	// Token: 0x040016AB RID: 5803
	TrickTreatBombImpact,
	// Token: 0x040016AC RID: 5804
	GuardianSlapFX,
	// Token: 0x040016AD RID: 5805
	GuardianSlamFX,
	// Token: 0x040016AE RID: 5806
	GuardianIdolLandedFX,
	// Token: 0x040016AF RID: 5807
	GuardianIdolFallFX,
	// Token: 0x040016B0 RID: 5808
	GuardianIdolTappedFX,
	// Token: 0x040016B1 RID: 5809
	VotingRockProjectile,
	// Token: 0x040016B2 RID: 5810
	LeafPileImpactFXMedium,
	// Token: 0x040016B3 RID: 5811
	LeafPileImpactFXSmall,
	// Token: 0x040016B4 RID: 5812
	WoodenSword,
	// Token: 0x040016B5 RID: 5813
	WoodenShield,
	// Token: 0x040016B6 RID: 5814
	FxBuilderShrink,
	// Token: 0x040016B7 RID: 5815
	FxBuilderGrow,
	// Token: 0x040016B8 RID: 5816
	FxSnapPieceWreathJump,
	// Token: 0x040016B9 RID: 5817
	ElfLauncherElf,
	// Token: 0x040016BA RID: 5818
	RubberBandCar,
	// Token: 0x040016BB RID: 5819
	SnowPileImpactFX,
	// Token: 0x040016BC RID: 5820
	FirecrackersProjectile,
	// Token: 0x040016BD RID: 5821
	PaperAirplaneSquareProjectile,
	// Token: 0x040016BE RID: 5822
	SmokeBombProjectile,
	// Token: 0x040016BF RID: 5823
	ThrowableHeartProjectile,
	// Token: 0x040016C0 RID: 5824
	SunFlowers,
	// Token: 0x040016C1 RID: 5825
	RobotCannonProjectile,
	// Token: 0x040016C2 RID: 5826
	RobotCannonProjectileImpact,
	// Token: 0x040016C3 RID: 5827
	SmokeBombExplosionEffect,
	// Token: 0x040016C4 RID: 5828
	FireCrackerExplosionEffect,
	// Token: 0x040016C5 RID: 5829
	GorillaMouth
}
