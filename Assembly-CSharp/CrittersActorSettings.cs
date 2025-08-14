using System;
using UnityEngine;

// Token: 0x02000042 RID: 66
public class CrittersActorSettings : MonoBehaviour
{
	// Token: 0x06000147 RID: 327 RVA: 0x00008E42 File Offset: 0x00007042
	public virtual void OnEnable()
	{
		this.UpdateActorSettings();
	}

	// Token: 0x06000148 RID: 328 RVA: 0x00008E4C File Offset: 0x0000704C
	public virtual void UpdateActorSettings()
	{
		this.parentActor.usesRB = this.usesRB;
		this.parentActor.rb.isKinematic = !this.usesRB;
		this.parentActor.equipmentStorable = this.canBeStored;
		this.parentActor.storeCollider = this.storeCollider;
		this.parentActor.equipmentStoreTriggerCollider = this.equipmentStoreTriggerCollider;
	}

	// Token: 0x04000170 RID: 368
	public CrittersActor parentActor;

	// Token: 0x04000171 RID: 369
	public bool usesRB;

	// Token: 0x04000172 RID: 370
	public bool canBeStored;

	// Token: 0x04000173 RID: 371
	public CapsuleCollider storeCollider;

	// Token: 0x04000174 RID: 372
	public CapsuleCollider equipmentStoreTriggerCollider;
}
