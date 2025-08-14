using System;
using UnityEngine;

// Token: 0x02000050 RID: 80
public class CrittersCageDepositShim : MonoBehaviour
{
	// Token: 0x0600018D RID: 397 RVA: 0x00009F4C File Offset: 0x0000814C
	[ContextMenu("Copy Deposit Data To Shim")]
	private CrittersCageDeposit CopySpawnerDataInPrefab()
	{
		CrittersCageDeposit component = base.gameObject.GetComponent<CrittersCageDeposit>();
		this.cageBoxCollider = (BoxCollider)component.gameObject.GetComponent<Collider>();
		this.type = component.actorType;
		this.disableGrabOnAttach = component.disableGrabOnAttach;
		this.allowMultiAttach = component.allowMultiAttach;
		this.snapOnAttach = component.snapOnAttach;
		this.startLocation = component.depositStartLocation;
		this.endLocation = component.depositEndLocation;
		this.submitDuration = component.submitDuration;
		this.returnDuration = component.returnDuration;
		this.depositAudio = component.depositAudio;
		this.depositStartSound = component.depositStartSound;
		this.depositEmptySound = component.depositEmptySound;
		this.depositCritterSound = component.depositCritterSound;
		this.attachPointTransform = component.GetComponentInChildren<CrittersActor>().transform;
		this.visiblePlatformTransform = this.attachPointTransform.transform.GetChild(0).transform;
		return component;
	}

	// Token: 0x0600018E RID: 398 RVA: 0x0000A03C File Offset: 0x0000823C
	[ContextMenu("Replace Deposit With Shim")]
	private void ReplaceSpawnerWithShim()
	{
		CrittersCageDeposit crittersCageDeposit = this.CopySpawnerDataInPrefab();
		if (crittersCageDeposit.attachPoint.GetComponent<Rigidbody>() != null)
		{
			Object.DestroyImmediate(crittersCageDeposit.attachPoint.GetComponent<Rigidbody>());
		}
		Object.DestroyImmediate(crittersCageDeposit.attachPoint);
		Object.DestroyImmediate(crittersCageDeposit);
	}

	// Token: 0x040001CF RID: 463
	public BoxCollider cageBoxCollider;

	// Token: 0x040001D0 RID: 464
	public CrittersActor.CrittersActorType type;

	// Token: 0x040001D1 RID: 465
	public bool disableGrabOnAttach;

	// Token: 0x040001D2 RID: 466
	public bool allowMultiAttach;

	// Token: 0x040001D3 RID: 467
	public bool snapOnAttach;

	// Token: 0x040001D4 RID: 468
	public Vector3 startLocation;

	// Token: 0x040001D5 RID: 469
	public Vector3 endLocation;

	// Token: 0x040001D6 RID: 470
	public float submitDuration;

	// Token: 0x040001D7 RID: 471
	public float returnDuration;

	// Token: 0x040001D8 RID: 472
	public AudioSource depositAudio;

	// Token: 0x040001D9 RID: 473
	public AudioClip depositStartSound;

	// Token: 0x040001DA RID: 474
	public AudioClip depositEmptySound;

	// Token: 0x040001DB RID: 475
	public AudioClip depositCritterSound;

	// Token: 0x040001DC RID: 476
	public Transform attachPointTransform;

	// Token: 0x040001DD RID: 477
	public Transform visiblePlatformTransform;
}
