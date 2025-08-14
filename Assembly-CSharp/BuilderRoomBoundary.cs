using System;
using System.Collections.Generic;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x02000550 RID: 1360
public class BuilderRoomBoundary : GorillaTriggerBox
{
	// Token: 0x06002127 RID: 8487 RVA: 0x000B3868 File Offset: 0x000B1A68
	private void Awake()
	{
		foreach (SizeChangerTrigger sizeChangerTrigger in this.enableOnEnterTrigger)
		{
			sizeChangerTrigger.OnEnter += this.OnEnteredBoundary;
		}
		this.disableOnExitTrigger.OnExit += this.OnExitedBoundary;
	}

	// Token: 0x06002128 RID: 8488 RVA: 0x000B38DC File Offset: 0x000B1ADC
	private void OnDestroy()
	{
		foreach (SizeChangerTrigger sizeChangerTrigger in this.enableOnEnterTrigger)
		{
			sizeChangerTrigger.OnEnter -= this.OnEnteredBoundary;
		}
		this.disableOnExitTrigger.OnExit -= this.OnExitedBoundary;
	}

	// Token: 0x06002129 RID: 8489 RVA: 0x000B3950 File Offset: 0x000B1B50
	public void OnEnteredBoundary(Collider other)
	{
		if (other.attachedRigidbody == null)
		{
			return;
		}
		this.rigRef = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (this.rigRef == null || !this.rigRef.isOfflineVRRig)
		{
			return;
		}
		BuilderTable builderTable;
		if (!BuilderTable.TryGetBuilderTableForZone(this.rigRef.zoneEntity.currentZone, out builderTable))
		{
			return;
		}
		if (builderTable.isTableMutable)
		{
			this.rigRef.EnableBuilderResizeWatch(true);
		}
	}

	// Token: 0x0600212A RID: 8490 RVA: 0x000B39CC File Offset: 0x000B1BCC
	public void OnExitedBoundary(Collider other)
	{
		if (other.attachedRigidbody == null)
		{
			return;
		}
		this.rigRef = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (this.rigRef == null || !this.rigRef.isOfflineVRRig)
		{
			return;
		}
		this.rigRef.EnableBuilderResizeWatch(false);
	}

	// Token: 0x04002A78 RID: 10872
	[SerializeField]
	private List<SizeChangerTrigger> enableOnEnterTrigger;

	// Token: 0x04002A79 RID: 10873
	[SerializeField]
	private SizeChangerTrigger disableOnExitTrigger;

	// Token: 0x04002A7A RID: 10874
	private VRRig rigRef;
}
