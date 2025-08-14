using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200003E RID: 62
public class CrittersActorDeposit : MonoBehaviour
{
	// Token: 0x06000120 RID: 288 RVA: 0x00007D9C File Offset: 0x00005F9C
	public void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody.IsNotNull())
		{
			CrittersActor component = other.attachedRigidbody.GetComponent<CrittersActor>();
			if (CrittersManager.instance.LocalAuthority() && component.IsNotNull() && this.CanDeposit(component) && this.IsAttachAvailable())
			{
				this.HandleDeposit(component);
			}
		}
	}

	// Token: 0x06000121 RID: 289 RVA: 0x00007DF0 File Offset: 0x00005FF0
	protected virtual bool CanDeposit(CrittersActor depositActor)
	{
		if (depositActor.crittersActorType != this.actorType)
		{
			return false;
		}
		CrittersActor crittersActor;
		if (CrittersManager.instance.actorById.TryGetValue(depositActor.parentActorId, out crittersActor))
		{
			return crittersActor.crittersActorType == CrittersActor.CrittersActorType.Grabber;
		}
		return depositActor.parentActorId == -1;
	}

	// Token: 0x06000122 RID: 290 RVA: 0x00007E3C File Offset: 0x0000603C
	private bool IsAttachAvailable()
	{
		return this.allowMultiAttach || this.currentAttach == null;
	}

	// Token: 0x06000123 RID: 291 RVA: 0x00007E54 File Offset: 0x00006054
	protected virtual void HandleDeposit(CrittersActor depositedActor)
	{
		this.currentAttach = depositedActor;
		depositedActor.ReleasedEvent.AddListener(new UnityAction<CrittersActor>(this.HandleDetach));
		CrittersActor grabbingActor = this.attachPoint;
		bool positionOverride = this.snapOnAttach;
		bool disableGrabbing = this.disableGrabOnAttach;
		depositedActor.GrabbedBy(grabbingActor, positionOverride, default(Quaternion), default(Vector3), disableGrabbing);
	}

	// Token: 0x06000124 RID: 292 RVA: 0x00007EAC File Offset: 0x000060AC
	protected virtual void HandleDetach(CrittersActor detachingActor)
	{
		this.currentAttach = null;
	}

	// Token: 0x04000145 RID: 325
	public CrittersActor attachPoint;

	// Token: 0x04000146 RID: 326
	public CrittersActor.CrittersActorType actorType;

	// Token: 0x04000147 RID: 327
	public bool disableGrabOnAttach;

	// Token: 0x04000148 RID: 328
	public bool allowMultiAttach;

	// Token: 0x04000149 RID: 329
	public bool snapOnAttach;

	// Token: 0x0400014A RID: 330
	private CrittersActor currentAttach;
}
