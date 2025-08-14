using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200004D RID: 77
public class CrittersCage : CrittersActor
{
	// Token: 0x17000019 RID: 25
	// (get) Token: 0x0600016E RID: 366 RVA: 0x000098B0 File Offset: 0x00007AB0
	public Vector3 critterScale
	{
		get
		{
			if (this.subObjectIndex < this.critterScales.Length && this.subObjectIndex >= 0)
			{
				return this.critterScales[this.subObjectIndex];
			}
			return Vector3.one;
		}
	}

	// Token: 0x1700001A RID: 26
	// (get) Token: 0x0600016F RID: 367 RVA: 0x000098E2 File Offset: 0x00007AE2
	public bool CanCatch
	{
		get
		{
			return this.heldByPlayer && !this.hasCritter && !this.inReleasingPosition && this._releaseCooldownEnd <= Time.time;
		}
	}

	// Token: 0x06000170 RID: 368 RVA: 0x0000990E File Offset: 0x00007B0E
	public void SetHasCritter(bool value)
	{
		if (this.hasCritter != value && !value)
		{
			this._releaseCooldownEnd = Time.time + this.releaseCooldown;
		}
		this.hasCritter = value;
		this.UpdateCageVisuals();
	}

	// Token: 0x06000171 RID: 369 RVA: 0x0000993B File Offset: 0x00007B3B
	public override void Initialize()
	{
		base.Initialize();
		this.hasCritter = false;
		this.heldByPlayer = false;
		this.inReleasingPosition = false;
		this.SetLidActive(true, false);
	}

	// Token: 0x06000172 RID: 370 RVA: 0x00009960 File Offset: 0x00007B60
	private void UpdateCageVisuals()
	{
		this.SetLidActive(!this.heldByPlayer || this.hasCritter, true);
	}

	// Token: 0x06000173 RID: 371 RVA: 0x0000997C File Offset: 0x00007B7C
	private void SetLidActive(bool active, bool playAudio = true)
	{
		if (active != this._lidActive && playAudio)
		{
			this.sound.GTPlayOneShot(active ? this.openSound : this.closeSound, 1f);
		}
		this.lid.SetActive(active);
		this._lidActive = active;
	}

	// Token: 0x06000174 RID: 372 RVA: 0x000099CD File Offset: 0x00007BCD
	protected override void RemoteGrabbedBy(CrittersActor grabbingActor)
	{
		base.RemoteGrabbedBy(grabbingActor);
		this.heldByPlayer = grabbingActor.isOnPlayer;
		this.UpdateCageVisuals();
	}

	// Token: 0x06000175 RID: 373 RVA: 0x000099E8 File Offset: 0x00007BE8
	public override void GrabbedBy(CrittersActor grabbingActor, bool positionOverride = false, Quaternion localRotation = default(Quaternion), Vector3 localOffset = default(Vector3), bool disableGrabbing = false)
	{
		base.GrabbedBy(grabbingActor, positionOverride, localRotation, localOffset, disableGrabbing);
		this.heldByPlayer = grabbingActor.isOnPlayer;
		this.UpdateCageVisuals();
	}

	// Token: 0x06000176 RID: 374 RVA: 0x00009A09 File Offset: 0x00007C09
	public override void Released(bool keepWorldPosition, Quaternion rotation = default(Quaternion), Vector3 position = default(Vector3), Vector3 impulseVelocity = default(Vector3), Vector3 impulseAngularVelocity = default(Vector3))
	{
		base.Released(keepWorldPosition, rotation, position, impulseVelocity, impulseAngularVelocity);
		this.heldByPlayer = false;
		this.UpdateCageVisuals();
	}

	// Token: 0x06000177 RID: 375 RVA: 0x00009A25 File Offset: 0x00007C25
	protected override void HandleRemoteReleased()
	{
		base.HandleRemoteReleased();
		this.heldByPlayer = false;
		this.UpdateCageVisuals();
	}

	// Token: 0x06000178 RID: 376 RVA: 0x00009A3A File Offset: 0x00007C3A
	public override bool ShouldDespawn()
	{
		return base.ShouldDespawn() && !this.hasCritter;
	}

	// Token: 0x06000179 RID: 377 RVA: 0x00009A4F File Offset: 0x00007C4F
	public override void SendDataByCrittersActorType(PhotonStream stream)
	{
		base.SendDataByCrittersActorType(stream);
		stream.SendNext(this.hasCritter);
	}

	// Token: 0x0600017A RID: 378 RVA: 0x00009A6C File Offset: 0x00007C6C
	public override bool UpdateSpecificActor(PhotonStream stream)
	{
		if (!base.UpdateSpecificActor(stream))
		{
			return false;
		}
		bool flag;
		if (!CrittersManager.ValidateDataType<bool>(stream.ReceiveNext(), out flag))
		{
			return false;
		}
		this.SetHasCritter(flag);
		return true;
	}

	// Token: 0x0600017B RID: 379 RVA: 0x00009A9D File Offset: 0x00007C9D
	public override int AddActorDataToList(ref List<object> objList)
	{
		base.AddActorDataToList(ref objList);
		objList.Add(this.hasCritter);
		return this.TotalActorDataLength();
	}

	// Token: 0x0600017C RID: 380 RVA: 0x000090A0 File Offset: 0x000072A0
	public override int TotalActorDataLength()
	{
		return base.BaseActorDataLength() + 1;
	}

	// Token: 0x0600017D RID: 381 RVA: 0x00009AC0 File Offset: 0x00007CC0
	public override int UpdateFromRPC(object[] data, int startingIndex)
	{
		startingIndex += base.UpdateFromRPC(data, startingIndex);
		bool flag;
		if (!CrittersManager.ValidateDataType<bool>(data[startingIndex], out flag))
		{
			return this.TotalActorDataLength();
		}
		this.SetHasCritter(flag);
		return this.TotalActorDataLength();
	}

	// Token: 0x040001AE RID: 430
	public Transform grabPosition;

	// Token: 0x040001AF RID: 431
	public Transform cagePosition;

	// Token: 0x040001B0 RID: 432
	public float grabDistance;

	// Token: 0x040001B1 RID: 433
	[SerializeField]
	private Vector3[] critterScales = new Vector3[]
	{
		Vector3.one
	};

	// Token: 0x040001B2 RID: 434
	[SerializeField]
	private float releaseCooldown = 0.25f;

	// Token: 0x040001B3 RID: 435
	[SerializeField]
	private AudioSource sound;

	// Token: 0x040001B4 RID: 436
	[SerializeField]
	private AudioClip openSound;

	// Token: 0x040001B5 RID: 437
	[SerializeField]
	private AudioClip closeSound;

	// Token: 0x040001B6 RID: 438
	public GameObject lid;

	// Token: 0x040001B7 RID: 439
	[NonSerialized]
	public bool heldByPlayer;

	// Token: 0x040001B8 RID: 440
	[NonSerialized]
	private bool hasCritter;

	// Token: 0x040001B9 RID: 441
	[NonSerialized]
	public bool inReleasingPosition;

	// Token: 0x040001BA RID: 442
	private float _releaseCooldownEnd;

	// Token: 0x040001BB RID: 443
	private bool _lidActive;
}
