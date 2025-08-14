using System;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000049 RID: 73
public class CrittersBag : CrittersActor
{
	// Token: 0x06000160 RID: 352 RVA: 0x000092A7 File Offset: 0x000074A7
	protected override void Awake()
	{
		base.Awake();
		this.overlapColliders = new Collider[20];
		this.attachedColliders = new Dictionary<int, GameObject>();
		this.isAttachedToPlayer = false;
	}

	// Token: 0x06000161 RID: 353 RVA: 0x000092CE File Offset: 0x000074CE
	public override void OnHover(bool isLeft)
	{
		if (this.isAttachedToPlayer)
		{
			GorillaTagger.Instance.StartVibration(isLeft, GorillaTagger.Instance.tapHapticStrength / 4f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			return;
		}
		base.OnHover(isLeft);
	}

	// Token: 0x06000162 RID: 354 RVA: 0x0000930C File Offset: 0x0000750C
	protected override void CleanupActor()
	{
		base.CleanupActor();
		for (int i = this.attachedColliders.Count - 1; i >= 0; i--)
		{
			this.attachedColliders[this.attachedColliders.ElementAt(i).Key].gameObject.Destroy();
		}
		this.attachedColliders.Clear();
	}

	// Token: 0x06000163 RID: 355 RVA: 0x0000936C File Offset: 0x0000756C
	protected override void GlobalGrabbedBy(CrittersActor grabbedBy)
	{
		base.GlobalGrabbedBy(grabbedBy);
		bool flag = this.attachedToLocalPlayer;
		if (grabbedBy.IsNotNull())
		{
			CrittersAttachPoint crittersAttachPoint = grabbedBy as CrittersAttachPoint;
			if (crittersAttachPoint != null)
			{
				this.isAttachedToPlayer = true;
				this.attachedToLocalPlayer = (crittersAttachPoint.rigPlayerId == PhotonNetwork.LocalPlayer.ActorNumber);
				goto IL_4F;
			}
		}
		this.isAttachedToPlayer = false;
		this.attachedToLocalPlayer = false;
		IL_4F:
		if (this.attachedToLocalPlayer != flag)
		{
			bool flag2 = this.attachedToLocalPlayer || flag;
			this.audioSrc.transform.localPosition = Vector3.zero;
			this.audioSrc.GTPlayOneShot(this.attachedToLocalPlayer ? this.equipSound : this.unequipSound, flag2 ? 1f : 0.5f);
		}
	}

	// Token: 0x06000164 RID: 356 RVA: 0x0000941F File Offset: 0x0000761F
	public override void GrabbedBy(CrittersActor grabbedBy, bool positionOverride = false, Quaternion localRotation = default(Quaternion), Vector3 localOffset = default(Vector3), bool disableGrabbing = false)
	{
		base.GrabbedBy(grabbedBy, positionOverride, localRotation, localOffset, disableGrabbing);
	}

	// Token: 0x06000165 RID: 357 RVA: 0x00009430 File Offset: 0x00007630
	public override void Released(bool keepWorldPosition, Quaternion rotation = default(Quaternion), Vector3 position = default(Vector3), Vector3 impulse = default(Vector3), Vector3 impulseRotation = default(Vector3))
	{
		if (this.parentActorId >= 0)
		{
			base.AttemptRemoveStoredObjectCollider(this.parentActorId, true);
		}
		int num = Physics.OverlapBoxNonAlloc(this.dropCube.transform.position, this.dropCube.size / 2f, this.overlapColliders, this.dropCube.transform.rotation, CrittersManager.instance.objectLayers, QueryTriggerInteraction.Collide);
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				Rigidbody attachedRigidbody = this.overlapColliders[i].attachedRigidbody;
				if (!(attachedRigidbody == null))
				{
					CrittersAttachPoint component = attachedRigidbody.GetComponent<CrittersAttachPoint>();
					if (!(component == null) && component.anchorLocation == this.anchorLocation && !(component.GetComponentInChildren<CrittersBag>() != null))
					{
						CrittersActor crittersActor;
						if (this.lastGrabbedPlayer == PhotonNetwork.LocalPlayer.ActorNumber && CrittersManager.instance.actorById.TryGetValue(this.parentActorId, out crittersActor))
						{
							CrittersGrabber crittersGrabber = crittersActor as CrittersGrabber;
							if (crittersGrabber != null)
							{
								GorillaTagger.Instance.StartVibration(crittersGrabber.isLeft, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
							}
						}
						this.GrabbedBy(component, true, default(Quaternion), default(Vector3), false);
						return;
					}
				}
			}
		}
		base.Released(keepWorldPosition, rotation, position, impulse, impulseRotation);
	}

	// Token: 0x06000166 RID: 358 RVA: 0x000095A0 File Offset: 0x000077A0
	public void AddStoredObjectCollider(CrittersActor actor)
	{
		if (this.attachedColliders.ContainsKey(actor.actorId))
		{
			if (this.attachedColliders[actor.actorId].IsNull())
			{
				this.attachedColliders[actor.actorId] = CrittersManager.DuplicateCapsuleCollider(base.transform, actor.storeCollider).gameObject;
			}
		}
		else
		{
			this.attachedColliders.Add(actor.actorId, CrittersManager.DuplicateCapsuleCollider(base.transform, actor.storeCollider).gameObject);
		}
		this.audioSrc.transform.position = actor.transform.position;
		this.audioSrc.GTPlayOneShot(this.attachSound, 1f);
	}

	// Token: 0x06000167 RID: 359 RVA: 0x0000965C File Offset: 0x0000785C
	public void RemoveStoredObjectCollider(CrittersActor actor, bool playSound = true)
	{
		GameObject obj;
		if (this.attachedColliders.TryGetValue(actor.actorId, out obj))
		{
			Object.Destroy(obj);
			this.attachedColliders.Remove(actor.actorId);
		}
		if (playSound)
		{
			this.audioSrc.transform.position = actor.transform.position;
			this.audioSrc.GTPlayOneShot(this.detachSound, 1f);
		}
	}

	// Token: 0x06000168 RID: 360 RVA: 0x000096CA File Offset: 0x000078CA
	public bool IsActorValidStore(CrittersActor actor)
	{
		return this.blockAttachTypes == null || !this.blockAttachTypes.Contains(actor.crittersActorType);
	}

	// Token: 0x0400018E RID: 398
	public AudioSource audioSrc;

	// Token: 0x0400018F RID: 399
	public CrittersAttachPoint.AnchoredLocationTypes anchorLocation;

	// Token: 0x04000190 RID: 400
	public Collider attachableCollider;

	// Token: 0x04000191 RID: 401
	public BoxCollider dropCube;

	// Token: 0x04000192 RID: 402
	private Collider[] overlapColliders;

	// Token: 0x04000193 RID: 403
	public List<Collider> attachDisableColliders;

	// Token: 0x04000194 RID: 404
	public Dictionary<int, GameObject> attachedColliders;

	// Token: 0x04000195 RID: 405
	[Header("Child object attachment sounds")]
	public AudioClip attachSound;

	// Token: 0x04000196 RID: 406
	public AudioClip detachSound;

	// Token: 0x04000197 RID: 407
	[Header("Monke equip sounds")]
	public AudioClip equipSound;

	// Token: 0x04000198 RID: 408
	public AudioClip unequipSound;

	// Token: 0x04000199 RID: 409
	[Header("Attachment Blocking")]
	public List<CrittersActor.CrittersActorType> blockAttachTypes;

	// Token: 0x0400019A RID: 410
	private bool isAttachedToPlayer;

	// Token: 0x0400019B RID: 411
	private bool attachedToLocalPlayer;
}
