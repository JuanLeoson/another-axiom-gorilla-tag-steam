using System;
using Cinemachine.Utility;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020001B1 RID: 433
public class LookDirectionStabilizer : MonoBehaviour, ISpawnable
{
	// Token: 0x1700010C RID: 268
	// (get) Token: 0x06000ABA RID: 2746 RVA: 0x00039CAA File Offset: 0x00037EAA
	// (set) Token: 0x06000ABB RID: 2747 RVA: 0x00039CB2 File Offset: 0x00037EB2
	public bool IsSpawned { get; set; }

	// Token: 0x1700010D RID: 269
	// (get) Token: 0x06000ABC RID: 2748 RVA: 0x00039CBB File Offset: 0x00037EBB
	// (set) Token: 0x06000ABD RID: 2749 RVA: 0x00039CC3 File Offset: 0x00037EC3
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06000ABE RID: 2750 RVA: 0x00039CCC File Offset: 0x00037ECC
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x06000ABF RID: 2751 RVA: 0x000023F5 File Offset: 0x000005F5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06000AC0 RID: 2752 RVA: 0x00039CD8 File Offset: 0x00037ED8
	private void Update()
	{
		Transform rigTarget = this.myRig.head.rigTarget;
		if (rigTarget.forward.y < 0f)
		{
			Quaternion b = Quaternion.LookRotation(rigTarget.up.ProjectOntoPlane(Vector3.up));
			Quaternion rotation = base.transform.parent.rotation;
			float value = Vector3.Dot(rigTarget.up, Vector3.up);
			base.transform.rotation = Quaternion.Lerp(rotation, b, Mathf.InverseLerp(1f, 0.7f, value));
			return;
		}
		base.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x04000D2B RID: 3371
	private VRRig myRig;
}
