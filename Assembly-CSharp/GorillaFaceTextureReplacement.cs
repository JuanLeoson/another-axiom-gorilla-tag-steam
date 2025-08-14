using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000194 RID: 404
public class GorillaFaceTextureReplacement : MonoBehaviour, ISpawnable
{
	// Token: 0x170000F5 RID: 245
	// (get) Token: 0x06000A3C RID: 2620 RVA: 0x00037CBB File Offset: 0x00035EBB
	// (set) Token: 0x06000A3D RID: 2621 RVA: 0x00037CC3 File Offset: 0x00035EC3
	public bool IsSpawned { get; set; }

	// Token: 0x170000F6 RID: 246
	// (get) Token: 0x06000A3E RID: 2622 RVA: 0x00037CCC File Offset: 0x00035ECC
	// (set) Token: 0x06000A3F RID: 2623 RVA: 0x00037CD4 File Offset: 0x00035ED4
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06000A40 RID: 2624 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnDespawn()
	{
	}

	// Token: 0x06000A41 RID: 2625 RVA: 0x00037CDD File Offset: 0x00035EDD
	public void OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x06000A42 RID: 2626 RVA: 0x00037CE8 File Offset: 0x00035EE8
	private void OnEnable()
	{
		this.myRig.GetComponent<GorillaMouthFlap>().SetFaceMaterialReplacement(this.newFaceMaterial);
		Material sharedMaterial = this.myRig.GetComponent<GorillaMouthFlap>().SetFaceMaterialReplacement(this.newFaceMaterial);
		MeshRenderer[] array = this.alsoApplyFaceTo;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].sharedMaterial = sharedMaterial;
		}
	}

	// Token: 0x06000A43 RID: 2627 RVA: 0x00037D41 File Offset: 0x00035F41
	private void OnDisable()
	{
		this.myRig.GetComponent<GorillaMouthFlap>().ClearFaceMaterialReplacement();
	}

	// Token: 0x04000C64 RID: 3172
	[SerializeField]
	private Material newFaceMaterial;

	// Token: 0x04000C65 RID: 3173
	private VRRig myRig;

	// Token: 0x04000C66 RID: 3174
	[SerializeField]
	private MeshRenderer[] alsoApplyFaceTo;
}
