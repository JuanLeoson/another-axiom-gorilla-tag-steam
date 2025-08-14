using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x0200019F RID: 415
public class GorillaMouthTextureReplacement : MonoBehaviour, ISpawnable
{
	// Token: 0x170000FE RID: 254
	// (get) Token: 0x06000A68 RID: 2664 RVA: 0x000388CD File Offset: 0x00036ACD
	// (set) Token: 0x06000A69 RID: 2665 RVA: 0x000388D5 File Offset: 0x00036AD5
	public bool IsSpawned { get; set; }

	// Token: 0x170000FF RID: 255
	// (get) Token: 0x06000A6A RID: 2666 RVA: 0x000388DE File Offset: 0x00036ADE
	// (set) Token: 0x06000A6B RID: 2667 RVA: 0x000388E6 File Offset: 0x00036AE6
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06000A6C RID: 2668 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnDespawn()
	{
	}

	// Token: 0x06000A6D RID: 2669 RVA: 0x000388EF File Offset: 0x00036AEF
	public void OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x06000A6E RID: 2670 RVA: 0x000388F8 File Offset: 0x00036AF8
	private void OnEnable()
	{
		this.myRig.GetComponent<GorillaMouthFlap>().SetMouthTextureReplacement(this.newMouthAtlas);
	}

	// Token: 0x06000A6F RID: 2671 RVA: 0x00038910 File Offset: 0x00036B10
	private void OnDisable()
	{
		this.myRig.GetComponent<GorillaMouthFlap>().ClearMouthTextureReplacement();
	}

	// Token: 0x04000CB9 RID: 3257
	[SerializeField]
	private Texture2D newMouthAtlas;

	// Token: 0x04000CBA RID: 3258
	private VRRig myRig;
}
