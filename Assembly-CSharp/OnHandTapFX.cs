using System;
using UnityEngine;

// Token: 0x020006B4 RID: 1716
internal struct OnHandTapFX : IFXEffectContext<HandEffectContext>
{
	// Token: 0x170003E4 RID: 996
	// (get) Token: 0x06002A57 RID: 10839 RVA: 0x000E2050 File Offset: 0x000E0250
	public HandEffectContext effectContext
	{
		get
		{
			HandEffectContext handEffect = this.rig.GetHandEffect(this.isLeftHand);
			this.rig.SetHandEffectData(handEffect, this.surfaceIndex, this.isDownTap, this.isLeftHand, this.volume, this.speed, this.tapDir);
			return handEffect;
		}
	}

	// Token: 0x170003E5 RID: 997
	// (get) Token: 0x06002A58 RID: 10840 RVA: 0x000E20A0 File Offset: 0x000E02A0
	public FXSystemSettings settings
	{
		get
		{
			return this.rig.fxSettings;
		}
	}

	// Token: 0x040035F1 RID: 13809
	public VRRig rig;

	// Token: 0x040035F2 RID: 13810
	public Vector3 tapDir;

	// Token: 0x040035F3 RID: 13811
	public bool isDownTap;

	// Token: 0x040035F4 RID: 13812
	public bool isLeftHand;

	// Token: 0x040035F5 RID: 13813
	public int surfaceIndex;

	// Token: 0x040035F6 RID: 13814
	public float volume;

	// Token: 0x040035F7 RID: 13815
	public float speed;
}
