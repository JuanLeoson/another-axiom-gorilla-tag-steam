using System;
using GorillaGameModes;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000750 RID: 1872
public class HoldableHand : HoldableObject, IGorillaSliceableSimple
{
	// Token: 0x1700044C RID: 1100
	// (get) Token: 0x06002EE3 RID: 12003 RVA: 0x000F873C File Offset: 0x000F693C
	public VRRig Rig
	{
		get
		{
			return this.myPlayer;
		}
	}

	// Token: 0x06002EE4 RID: 12004 RVA: 0x000F8744 File Offset: 0x000F6944
	private void Start()
	{
		if (this.myPlayer.isOfflineVRRig)
		{
			base.gameObject.SetActive(false);
		}
		if (this.interactionPoint == null)
		{
			this.interactionPoint = base.GetComponent<InteractionPoint>();
		}
	}

	// Token: 0x06002EE5 RID: 12005 RVA: 0x00010F6F File Offset: 0x0000F16F
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06002EE6 RID: 12006 RVA: 0x00010F78 File Offset: 0x0000F178
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06002EE7 RID: 12007 RVA: 0x000F8779 File Offset: 0x000F6979
	public void SliceUpdate()
	{
		this.interactionPoint.enabled = (GameMode.ActiveGameMode is GorillaGuardianManager);
	}

	// Token: 0x06002EE8 RID: 12008 RVA: 0x000F8794 File Offset: 0x000F6994
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
		if (gorillaGuardianManager != null && !this.myPlayer.creator.IsLocal && gorillaGuardianManager.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
		{
			bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
			this.myPlayer.netView.SendRPC("GrabbedByPlayer", this.myPlayer.Creator, new object[]
			{
				this.isBody,
				this.isLeftHand,
				flag
			});
			this.myPlayer.ApplyLocalGrabOverride(this.isBody, this.isLeftHand, grabbingHand.transform);
			EquipmentInteractor.instance.UpdateHandEquipment(this, flag);
			this.ClearOtherGrabs(flag);
		}
	}

	// Token: 0x06002EE9 RID: 12009 RVA: 0x000F886C File Offset: 0x000F6A6C
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
		if (gorillaGuardianManager != null && !this.myPlayer.creator.IsLocal)
		{
			bool flag = releasingHand == EquipmentInteractor.instance.leftHand;
			Vector3 vector = Vector3.zero;
			if (gorillaGuardianManager.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
			{
				vector = (flag ? GTPlayer.Instance.leftHandCenterVelocityTracker : GTPlayer.Instance.rightHandCenterVelocityTracker).GetAverageVelocity(true, 0.15f, false);
			}
			vector = Vector3.ClampMagnitude(vector, 20f);
			this.myPlayer.netView.SendRPC("DroppedByPlayer", this.myPlayer.Creator, new object[]
			{
				vector
			});
			this.myPlayer.ClearLocalGrabOverride();
			this.myPlayer.ApplyLocalTrajectoryOverride(vector);
			EquipmentInteractor.instance.UpdateHandEquipment(null, flag);
		}
		return true;
	}

	// Token: 0x06002EEA RID: 12010 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06002EEB RID: 12011 RVA: 0x000F895D File Offset: 0x000F6B5D
	public override void DropItemCleanup()
	{
		this.myPlayer.ClearLocalGrabOverride();
	}

	// Token: 0x06002EEC RID: 12012 RVA: 0x000F896C File Offset: 0x000F6B6C
	private void ClearOtherGrabs(bool grabbedLeft)
	{
		IHoldableObject holdableObject = grabbedLeft ? EquipmentInteractor.instance.rightHandHeldEquipment : EquipmentInteractor.instance.leftHandHeldEquipment;
		if (this.isBody)
		{
			if (holdableObject == this.myPlayer.leftHolds || holdableObject == this.myPlayer.rightHolds)
			{
				EquipmentInteractor.instance.UpdateHandEquipment(null, !grabbedLeft);
				return;
			}
		}
		else if (this.isLeftHand)
		{
			if (holdableObject == this.myPlayer.rightHolds || holdableObject == this.myPlayer.bodyHolds)
			{
				EquipmentInteractor.instance.UpdateHandEquipment(null, !grabbedLeft);
				return;
			}
		}
		else if (holdableObject == this.myPlayer.leftHolds || holdableObject == this.myPlayer.bodyHolds)
		{
			EquipmentInteractor.instance.UpdateHandEquipment(null, !grabbedLeft);
		}
	}

	// Token: 0x04003AEC RID: 15084
	[SerializeField]
	private VRRig myPlayer;

	// Token: 0x04003AED RID: 15085
	[SerializeField]
	private bool isBody;

	// Token: 0x04003AEE RID: 15086
	[SerializeField]
	private bool isLeftHand;

	// Token: 0x04003AEF RID: 15087
	public InteractionPoint interactionPoint;
}
