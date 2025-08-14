using System;
using System.Diagnostics;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x02000070 RID: 112
public class CrittersToolThrowable : CrittersActor
{
	// Token: 0x060002BC RID: 700 RVA: 0x00010D1E File Offset: 0x0000EF1E
	public override void Initialize()
	{
		base.Initialize();
		this.hasBeenGrabbedByPlayer = false;
		this.shouldDisable = false;
		this.hasTriggeredSinceLastGrab = false;
		this._sqrActivationSpeed = this.requiredActivationSpeed * this.requiredActivationSpeed;
	}

	// Token: 0x060002BD RID: 701 RVA: 0x00010D4E File Offset: 0x0000EF4E
	public override void GrabbedBy(CrittersActor grabbingActor, bool positionOverride = false, Quaternion localRotation = default(Quaternion), Vector3 localOffset = default(Vector3), bool disableGrabbing = false)
	{
		base.GrabbedBy(grabbingActor, positionOverride, localRotation, localOffset, disableGrabbing);
		this.hasBeenGrabbedByPlayer = true;
		this.hasTriggeredSinceLastGrab = false;
		this.OnPickedUp();
	}

	// Token: 0x060002BE RID: 702 RVA: 0x00010D74 File Offset: 0x0000EF74
	public void OnCollisionEnter(Collision collision)
	{
		if (CrittersManager.instance.containerLayer.Contains(collision.gameObject.layer))
		{
			return;
		}
		if (this.requiresPlayerGrabBeforeActivate && !this.hasBeenGrabbedByPlayer)
		{
			return;
		}
		if (this._sqrActivationSpeed > 0f && collision.relativeVelocity.sqrMagnitude < this._sqrActivationSpeed)
		{
			return;
		}
		if (this.onlyTriggerOncePerGrab && this.hasTriggeredSinceLastGrab)
		{
			return;
		}
		if (this.onlyTriggerOnDirectCritterHit)
		{
			CrittersPawn component = collision.gameObject.GetComponent<CrittersPawn>();
			if (component != null && component.isActiveAndEnabled)
			{
				this.hasTriggeredSinceLastGrab = true;
				this.OnImpactCritter(component);
			}
		}
		else
		{
			Vector3 point = collision.contacts[0].point;
			Vector3 normal = collision.contacts[0].normal;
			this.hasTriggeredSinceLastGrab = true;
			this.OnImpact(point, normal);
		}
		if (this.destroyOnImpact)
		{
			this.shouldDisable = true;
		}
	}

	// Token: 0x060002BF RID: 703 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void OnImpact(Vector3 hitPosition, Vector3 hitNormal)
	{
	}

	// Token: 0x060002C0 RID: 704 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void OnImpactCritter(CrittersPawn impactedCritter)
	{
	}

	// Token: 0x060002C1 RID: 705 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void OnPickedUp()
	{
	}

	// Token: 0x060002C2 RID: 706 RVA: 0x00010E60 File Offset: 0x0000F060
	[Conditional("DRAW_DEBUG")]
	protected void ShowDebugVisualization(Vector3 position, float scale, float duration = 0f)
	{
		if (!this.debugImpactPrefab)
		{
			return;
		}
		DelayedDestroyObject delayedDestroyObject = Object.Instantiate<DelayedDestroyObject>(this.debugImpactPrefab, position, Quaternion.identity);
		delayedDestroyObject.transform.localScale *= scale;
		if (duration != 0f)
		{
			delayedDestroyObject.lifetime = duration;
		}
	}

	// Token: 0x060002C3 RID: 707 RVA: 0x00010EB4 File Offset: 0x0000F0B4
	public override bool ProcessLocal()
	{
		bool result = base.ProcessLocal();
		if (this.shouldDisable)
		{
			base.gameObject.SetActive(false);
			return true;
		}
		return result;
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x00010EE0 File Offset: 0x0000F0E0
	public override void TogglePhysics(bool enable)
	{
		if (enable)
		{
			this.rb.isKinematic = false;
			this.rb.interpolation = RigidbodyInterpolation.Interpolate;
			this.rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
			return;
		}
		this.rb.isKinematic = true;
		this.rb.interpolation = RigidbodyInterpolation.None;
		this.rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
	}

	// Token: 0x0400032B RID: 811
	[Header("Throwable")]
	public bool requiresPlayerGrabBeforeActivate = true;

	// Token: 0x0400032C RID: 812
	public float requiredActivationSpeed = 2f;

	// Token: 0x0400032D RID: 813
	public bool onlyTriggerOnDirectCritterHit;

	// Token: 0x0400032E RID: 814
	public bool destroyOnImpact = true;

	// Token: 0x0400032F RID: 815
	public bool onlyTriggerOncePerGrab = true;

	// Token: 0x04000330 RID: 816
	[Header("Debug")]
	[SerializeField]
	private DelayedDestroyObject debugImpactPrefab;

	// Token: 0x04000331 RID: 817
	private bool hasBeenGrabbedByPlayer;

	// Token: 0x04000332 RID: 818
	protected bool shouldDisable;

	// Token: 0x04000333 RID: 819
	private bool hasTriggeredSinceLastGrab;

	// Token: 0x04000334 RID: 820
	private float _sqrActivationSpeed;
}
