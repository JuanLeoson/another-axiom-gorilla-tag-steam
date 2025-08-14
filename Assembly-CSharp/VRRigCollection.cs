using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000B07 RID: 2823
[RequireComponent(typeof(CompositeTriggerEvents))]
public class VRRigCollection : MonoBehaviour
{
	// Token: 0x17000662 RID: 1634
	// (get) Token: 0x06004402 RID: 17410 RVA: 0x001553E2 File Offset: 0x001535E2
	public List<RigContainer> Rigs
	{
		get
		{
			return this.containedRigs;
		}
	}

	// Token: 0x06004403 RID: 17411 RVA: 0x001553EA File Offset: 0x001535EA
	private void OnEnable()
	{
		this.collisionTriggerEvents.CompositeTriggerEnter += this.OnRigTriggerEnter;
		this.collisionTriggerEvents.CompositeTriggerExit += this.OnRigTriggerExit;
	}

	// Token: 0x06004404 RID: 17412 RVA: 0x0015541C File Offset: 0x0015361C
	private void OnDisable()
	{
		for (int i = this.containedRigs.Count - 1; i >= 0; i--)
		{
			this.RigDisabled(this.containedRigs[i]);
		}
		this.collisionTriggerEvents.CompositeTriggerEnter -= this.OnRigTriggerEnter;
		this.collisionTriggerEvents.CompositeTriggerExit -= this.OnRigTriggerExit;
	}

	// Token: 0x06004405 RID: 17413 RVA: 0x00155484 File Offset: 0x00153684
	private void OnRigTriggerEnter(Collider other)
	{
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		RigContainer rigContainer;
		if (attachedRigidbody == null || !attachedRigidbody.TryGetComponent<RigContainer>(out rigContainer) || other != rigContainer.HeadCollider || this.containedRigs.Contains(rigContainer))
		{
			return;
		}
		VRRigEvents rigEvents = rigContainer.RigEvents;
		rigEvents.disableEvent = (Action<RigContainer>)Delegate.Combine(rigEvents.disableEvent, new Action<RigContainer>(this.RigDisabled));
		this.containedRigs.Add(rigContainer);
		Action<RigContainer> action = this.playerEnteredCollection;
		if (action == null)
		{
			return;
		}
		action(rigContainer);
	}

	// Token: 0x06004406 RID: 17414 RVA: 0x0015550C File Offset: 0x0015370C
	private void OnRigTriggerExit(Collider other)
	{
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		RigContainer rigContainer;
		if (attachedRigidbody == null || !attachedRigidbody.TryGetComponent<RigContainer>(out rigContainer) || other != rigContainer.HeadCollider || !this.containedRigs.Contains(rigContainer))
		{
			return;
		}
		VRRigEvents rigEvents = rigContainer.RigEvents;
		rigEvents.disableEvent = (Action<RigContainer>)Delegate.Remove(rigEvents.disableEvent, new Action<RigContainer>(this.RigDisabled));
		this.containedRigs.Remove(rigContainer);
		Action<RigContainer> action = this.playerLeftCollection;
		if (action == null)
		{
			return;
		}
		action(rigContainer);
	}

	// Token: 0x06004407 RID: 17415 RVA: 0x00155595 File Offset: 0x00153795
	private void RigDisabled(RigContainer rig)
	{
		this.collisionTriggerEvents.ResetColliderMask(rig.HeadCollider);
		this.collisionTriggerEvents.ResetColliderMask(rig.BodyCollider);
	}

	// Token: 0x06004408 RID: 17416 RVA: 0x001555BC File Offset: 0x001537BC
	private bool HasRig(VRRig rig)
	{
		for (int i = 0; i < this.containedRigs.Count; i++)
		{
			if (this.containedRigs[i].Rig == rig)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06004409 RID: 17417 RVA: 0x001555FC File Offset: 0x001537FC
	private bool HasRig(NetPlayer player)
	{
		for (int i = 0; i < this.containedRigs.Count; i++)
		{
			if (this.containedRigs[i].Creator == player)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04004E59 RID: 20057
	public readonly List<RigContainer> containedRigs = new List<RigContainer>(10);

	// Token: 0x04004E5A RID: 20058
	[SerializeField]
	private CompositeTriggerEvents collisionTriggerEvents;

	// Token: 0x04004E5B RID: 20059
	public Action<RigContainer> playerEnteredCollection;

	// Token: 0x04004E5C RID: 20060
	public Action<RigContainer> playerLeftCollection;
}
