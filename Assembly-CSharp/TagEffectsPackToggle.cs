using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using TagEffects;
using UnityEngine;

// Token: 0x020001C4 RID: 452
public class TagEffectsPackToggle : MonoBehaviour, ISpawnable
{
	// Token: 0x1700011A RID: 282
	// (get) Token: 0x06000B3B RID: 2875 RVA: 0x0003BCE1 File Offset: 0x00039EE1
	// (set) Token: 0x06000B3C RID: 2876 RVA: 0x0003BCE9 File Offset: 0x00039EE9
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x1700011B RID: 283
	// (get) Token: 0x06000B3D RID: 2877 RVA: 0x0003BCF2 File Offset: 0x00039EF2
	// (set) Token: 0x06000B3E RID: 2878 RVA: 0x0003BCFA File Offset: 0x00039EFA
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06000B3F RID: 2879 RVA: 0x0003BD03 File Offset: 0x00039F03
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this._rig = rig;
	}

	// Token: 0x06000B40 RID: 2880 RVA: 0x000023F5 File Offset: 0x000005F5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06000B41 RID: 2881 RVA: 0x0003BD0C File Offset: 0x00039F0C
	private void OnEnable()
	{
		this.Apply();
	}

	// Token: 0x06000B42 RID: 2882 RVA: 0x0003BD14 File Offset: 0x00039F14
	private void OnDisable()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.Remove();
	}

	// Token: 0x06000B43 RID: 2883 RVA: 0x0003BD24 File Offset: 0x00039F24
	public void Apply()
	{
		this._rig.CosmeticEffectPack = this.tagEffectPack;
	}

	// Token: 0x06000B44 RID: 2884 RVA: 0x0003BD37 File Offset: 0x00039F37
	public void Remove()
	{
		this._rig.CosmeticEffectPack = null;
	}

	// Token: 0x04000DD1 RID: 3537
	private VRRig _rig;

	// Token: 0x04000DD2 RID: 3538
	[SerializeField]
	private TagEffectPack tagEffectPack;
}
