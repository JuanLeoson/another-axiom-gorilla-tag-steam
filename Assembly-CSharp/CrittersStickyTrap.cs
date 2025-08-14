using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200006E RID: 110
public class CrittersStickyTrap : CrittersToolThrowable
{
	// Token: 0x060002AD RID: 685 RVA: 0x000109BF File Offset: 0x0000EBBF
	public override void Initialize()
	{
		base.Initialize();
		this.TogglePhysics(!this.isStuck);
	}

	// Token: 0x060002AE RID: 686 RVA: 0x000109D6 File Offset: 0x0000EBD6
	public override void OnDisable()
	{
		base.OnDisable();
		this.isStuck = false;
	}

	// Token: 0x060002AF RID: 687 RVA: 0x000109E8 File Offset: 0x0000EBE8
	public override void SetImpulse()
	{
		if (this.isOnPlayer || this.isSceneActor)
		{
			return;
		}
		this.localLastImpulse = this.lastImpulseTime;
		base.MoveActor(this.lastImpulsePosition, this.lastImpulseQuaternion, this.parentActorId >= 0, false, true);
		this.TogglePhysics(this.usesRB && this.parentActorId == -1 && !this.isStuck);
		if (!this.rb.isKinematic)
		{
			this.rb.velocity = this.lastImpulseVelocity;
			this.rb.angularVelocity = this.lastImpulseAngularVelocity;
		}
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x00010A84 File Offset: 0x0000EC84
	protected override void OnImpact(Vector3 hitPosition, Vector3 hitNormal)
	{
		if (CrittersManager.instance.LocalAuthority())
		{
			if (this.stickOnImpact)
			{
				this.rb.isKinematic = true;
				this.isStuck = true;
				this.updatedSinceLastFrame = true;
				base.UpdateImpulses(false, true);
			}
			CrittersStickyGoo crittersStickyGoo = (CrittersStickyGoo)CrittersManager.instance.SpawnActor(CrittersActor.CrittersActorType.StickyGoo, this.subStickyGooIndex);
			if (crittersStickyGoo == null)
			{
				return;
			}
			CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.StickyDeployed, this.actorId, base.transform.position, Quaternion.LookRotation(hitNormal));
			Vector3 vector = base.transform.forward;
			vector -= hitNormal * Vector3.Dot(hitNormal, vector);
			crittersStickyGoo.MoveActor(hitPosition, Quaternion.LookRotation(vector, hitNormal), false, true, true);
			crittersStickyGoo.SetImpulseVelocity(Vector3.zero, Vector3.zero);
			base.UpdateImpulses(true, false);
		}
	}

	// Token: 0x060002B1 RID: 689 RVA: 0x0000D89F File Offset: 0x0000BA9F
	protected override void OnImpactCritter(CrittersPawn impactedCritter)
	{
		this.OnImpact(impactedCritter.transform.position, impactedCritter.transform.up);
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x00010B5D File Offset: 0x0000ED5D
	protected override void OnPickedUp()
	{
		if (this.isStuck)
		{
			this.isStuck = false;
			this.updatedSinceLastFrame = true;
		}
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x00010B75 File Offset: 0x0000ED75
	public override void SendDataByCrittersActorType(PhotonStream stream)
	{
		base.SendDataByCrittersActorType(stream);
		stream.SendNext(this.isStuck);
	}

	// Token: 0x060002B4 RID: 692 RVA: 0x00010B90 File Offset: 0x0000ED90
	public override bool UpdateSpecificActor(PhotonStream stream)
	{
		bool flag;
		if (!(base.UpdateSpecificActor(stream) & CrittersManager.ValidateDataType<bool>(stream.ReceiveNext(), out flag)))
		{
			return false;
		}
		this.isStuck = flag;
		this.TogglePhysics(!this.isStuck);
		return true;
	}

	// Token: 0x060002B5 RID: 693 RVA: 0x00010BCD File Offset: 0x0000EDCD
	public override int AddActorDataToList(ref List<object> objList)
	{
		base.AddActorDataToList(ref objList);
		objList.Add(this.isStuck);
		return this.TotalActorDataLength();
	}

	// Token: 0x060002B6 RID: 694 RVA: 0x000090A0 File Offset: 0x000072A0
	public override int TotalActorDataLength()
	{
		return base.BaseActorDataLength() + 1;
	}

	// Token: 0x060002B7 RID: 695 RVA: 0x00010BF0 File Offset: 0x0000EDF0
	public override int UpdateFromRPC(object[] data, int startingIndex)
	{
		startingIndex += base.UpdateFromRPC(data, startingIndex);
		bool flag;
		if (!CrittersManager.ValidateDataType<bool>(data[startingIndex], out flag))
		{
			return this.TotalActorDataLength();
		}
		this.isStuck = flag;
		this.TogglePhysics(!this.isStuck);
		return this.TotalActorDataLength();
	}

	// Token: 0x04000326 RID: 806
	[Header("Sticky Trap")]
	public bool stickOnImpact = true;

	// Token: 0x04000327 RID: 807
	public int subStickyGooIndex = -1;

	// Token: 0x04000328 RID: 808
	private bool isStuck;
}
