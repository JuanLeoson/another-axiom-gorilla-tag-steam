using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x0200027F RID: 639
public class HandEffectsOverrideCosmetic : MonoBehaviour, ISpawnable
{
	// Token: 0x1700015E RID: 350
	// (get) Token: 0x06000E92 RID: 3730 RVA: 0x00058560 File Offset: 0x00056760
	// (set) Token: 0x06000E93 RID: 3731 RVA: 0x00058568 File Offset: 0x00056768
	public bool IsSpawned { get; set; }

	// Token: 0x1700015F RID: 351
	// (get) Token: 0x06000E94 RID: 3732 RVA: 0x00058571 File Offset: 0x00056771
	// (set) Token: 0x06000E95 RID: 3733 RVA: 0x00058579 File Offset: 0x00056779
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06000E96 RID: 3734 RVA: 0x00058582 File Offset: 0x00056782
	public void OnSpawn(VRRig rig)
	{
		this._rig = rig;
	}

	// Token: 0x06000E97 RID: 3735 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnDespawn()
	{
	}

	// Token: 0x06000E98 RID: 3736 RVA: 0x0005858B File Offset: 0x0005678B
	public void OnEnable()
	{
		if (!this.isLeftHand)
		{
			this._rig.CosmeticHandEffectsOverride_Right.Add(this);
			return;
		}
		this._rig.CosmeticHandEffectsOverride_Left.Add(this);
	}

	// Token: 0x06000E99 RID: 3737 RVA: 0x000585B8 File Offset: 0x000567B8
	public void OnDisable()
	{
		if (!this.isLeftHand)
		{
			this._rig.CosmeticHandEffectsOverride_Right.Remove(this);
			return;
		}
		this._rig.CosmeticHandEffectsOverride_Left.Remove(this);
	}

	// Token: 0x0400177C RID: 6012
	public HandEffectsOverrideCosmetic.HandEffectType handEffectType;

	// Token: 0x0400177D RID: 6013
	public bool isLeftHand;

	// Token: 0x0400177E RID: 6014
	public HandEffectsOverrideCosmetic.EffectsOverride firstPerson;

	// Token: 0x0400177F RID: 6015
	public HandEffectsOverrideCosmetic.EffectsOverride thirdPerson;

	// Token: 0x04001780 RID: 6016
	private VRRig _rig;

	// Token: 0x02000280 RID: 640
	[Serializable]
	public class EffectsOverride
	{
		// Token: 0x04001783 RID: 6019
		public GameObject effectVFX;

		// Token: 0x04001784 RID: 6020
		public bool playHaptics;

		// Token: 0x04001785 RID: 6021
		public float hapticStrength = 0.5f;

		// Token: 0x04001786 RID: 6022
		public float hapticDuration = 0.5f;

		// Token: 0x04001787 RID: 6023
		public bool parentEffect;
	}

	// Token: 0x02000281 RID: 641
	public enum HandEffectType
	{
		// Token: 0x04001789 RID: 6025
		None,
		// Token: 0x0400178A RID: 6026
		FistBump,
		// Token: 0x0400178B RID: 6027
		HighFive
	}
}
