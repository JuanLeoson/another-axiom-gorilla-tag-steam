using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x0200043F RID: 1087
public class LegacyTransferrableObject : HoldableObject
{
	// Token: 0x06001A82 RID: 6786 RVA: 0x0008D880 File Offset: 0x0008BA80
	protected void Awake()
	{
		this.latched = false;
		this.initOffset = base.transform.localPosition;
		this.initRotation = base.transform.localRotation;
	}

	// Token: 0x06001A83 RID: 6787 RVA: 0x0008D8AC File Offset: 0x0008BAAC
	protected virtual void Start()
	{
		RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
		RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
		RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.OnPlayerLeftRoom);
	}

	// Token: 0x06001A84 RID: 6788 RVA: 0x0008D90C File Offset: 0x0008BB0C
	public void OnEnable()
	{
		if (this.myRig == null && this.myOnlineRig != null && this.myOnlineRig.netView != null && this.myOnlineRig.netView.IsMine)
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (this.myRig == null && this.myOnlineRig == null)
		{
			base.gameObject.SetActive(false);
			return;
		}
		this.objectIndex = this.targetDock.ReturnTransferrableItemIndex(this.myIndex);
		if (this.myRig != null && this.myRig.isOfflineVRRig)
		{
			if (this.currentState == TransferrableObject.PositionState.OnLeftArm)
			{
				this.storedZone = BodyDockPositions.DropPositions.LeftArm;
			}
			else if (this.currentState == TransferrableObject.PositionState.OnRightArm)
			{
				this.storedZone = BodyDockPositions.DropPositions.RightArm;
			}
			else if (this.currentState == TransferrableObject.PositionState.OnLeftShoulder)
			{
				this.storedZone = BodyDockPositions.DropPositions.LeftBack;
			}
			else if (this.currentState == TransferrableObject.PositionState.OnRightShoulder)
			{
				this.storedZone = BodyDockPositions.DropPositions.RightBack;
			}
			else
			{
				this.storedZone = BodyDockPositions.DropPositions.Chest;
			}
		}
		if (this.objectIndex == -1)
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (this.currentState == TransferrableObject.PositionState.OnLeftArm && this.flipOnXForLeftArm)
		{
			Transform transform = this.GetAnchor(this.currentState);
			transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}
		this.initState = this.currentState;
		this.enabledOnFrame = Time.frameCount;
		this.SpawnShareableObject();
	}

	// Token: 0x06001A85 RID: 6789 RVA: 0x0008DA92 File Offset: 0x0008BC92
	public void OnDisable()
	{
		this.enabledOnFrame = -1;
	}

	// Token: 0x06001A86 RID: 6790 RVA: 0x0008DA9C File Offset: 0x0008BC9C
	private void SpawnShareableObject()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (!this.canDrop && !this.shareable)
		{
			return;
		}
		if (this.worldShareableInstance != null)
		{
			return;
		}
		object[] data = new object[]
		{
			this.myIndex,
			PhotonNetwork.LocalPlayer
		};
		this.worldShareableInstance = PhotonNetwork.Instantiate("Objects/equipment/WorldShareableItem", base.transform.position, base.transform.rotation, 0, data);
		if (this.myRig != null && this.worldShareableInstance != null)
		{
			this.OnWorldShareableItemSpawn();
		}
	}

	// Token: 0x06001A87 RID: 6791 RVA: 0x0008DB38 File Offset: 0x0008BD38
	public void OnJoinedRoom()
	{
		Debug.Log("Here");
		this.SpawnShareableObject();
	}

	// Token: 0x06001A88 RID: 6792 RVA: 0x0008DB4A File Offset: 0x0008BD4A
	public void OnLeftRoom()
	{
		if (this.worldShareableInstance != null)
		{
			PhotonNetwork.Destroy(this.worldShareableInstance);
		}
		this.OnWorldShareableItemDeallocated(NetworkSystem.Instance.LocalPlayer);
	}

	// Token: 0x06001A89 RID: 6793 RVA: 0x0008DB75 File Offset: 0x0008BD75
	public void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		this.OnWorldShareableItemDeallocated(otherPlayer);
	}

	// Token: 0x06001A8A RID: 6794 RVA: 0x0008DB7E File Offset: 0x0008BD7E
	public void SetWorldShareableItem(GameObject item)
	{
		this.worldShareableInstance = item;
		this.OnWorldShareableItemSpawn();
	}

	// Token: 0x06001A8B RID: 6795 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void OnWorldShareableItemSpawn()
	{
	}

	// Token: 0x06001A8C RID: 6796 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void OnWorldShareableItemDeallocated(NetPlayer player)
	{
	}

	// Token: 0x06001A8D RID: 6797 RVA: 0x0008DB90 File Offset: 0x0008BD90
	public virtual void LateUpdate()
	{
		if (this.interactor == null)
		{
			this.interactor = EquipmentInteractor.instance;
		}
		if (this.IsMyItem())
		{
			this.LateUpdateLocal();
		}
		else
		{
			this.LateUpdateReplicated();
		}
		this.LateUpdateShared();
		this.previousState = this.currentState;
	}

	// Token: 0x06001A8E RID: 6798 RVA: 0x0008DBE0 File Offset: 0x0008BDE0
	protected Transform DefaultAnchor()
	{
		if (!(this.anchor == null))
		{
			return this.anchor;
		}
		return base.transform;
	}

	// Token: 0x06001A8F RID: 6799 RVA: 0x0008DBFD File Offset: 0x0008BDFD
	private Transform GetAnchor(TransferrableObject.PositionState pos)
	{
		if (this.grabAnchor == null)
		{
			return this.DefaultAnchor();
		}
		if (this.InHand())
		{
			return this.grabAnchor;
		}
		return this.DefaultAnchor();
	}

	// Token: 0x06001A90 RID: 6800 RVA: 0x0008DC2C File Offset: 0x0008BE2C
	protected bool Attached()
	{
		bool flag = this.InHand() && this.detatchOnGrab;
		return !this.Dropped() && !flag;
	}

	// Token: 0x06001A91 RID: 6801 RVA: 0x0008DC5C File Offset: 0x0008BE5C
	private void UpdateFollowXform()
	{
		if (this.targetRig == null)
		{
			return;
		}
		if (this.targetDock == null)
		{
			this.targetDock = this.targetRig.GetComponent<BodyDockPositions>();
		}
		if (this.anchorOverrides == null)
		{
			this.anchorOverrides = this.targetRig.GetComponent<VRRigAnchorOverrides>();
		}
		Transform transform = this.GetAnchor(this.currentState);
		Transform transform2 = transform;
		TransferrableObject.PositionState positionState = this.currentState;
		if (positionState <= TransferrableObject.PositionState.InRightHand)
		{
			switch (positionState)
			{
			case TransferrableObject.PositionState.OnLeftArm:
				transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.leftArmTransform);
				break;
			case TransferrableObject.PositionState.OnRightArm:
				transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.rightArmTransform);
				break;
			case TransferrableObject.PositionState.OnLeftArm | TransferrableObject.PositionState.OnRightArm:
				break;
			case TransferrableObject.PositionState.InLeftHand:
				transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.leftHandTransform);
				break;
			default:
				if (positionState == TransferrableObject.PositionState.InRightHand)
				{
					transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.rightHandTransform);
				}
				break;
			}
		}
		else if (positionState != TransferrableObject.PositionState.OnChest)
		{
			if (positionState != TransferrableObject.PositionState.OnLeftShoulder)
			{
				if (positionState == TransferrableObject.PositionState.OnRightShoulder)
				{
					transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.rightBackTransform);
				}
			}
			else
			{
				transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.leftBackTransform);
			}
		}
		else
		{
			transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.chestTransform);
		}
		LegacyTransferrableObject.InterpolateState interpolateState = this.interpState;
		if (interpolateState != LegacyTransferrableObject.InterpolateState.None)
		{
			if (interpolateState != LegacyTransferrableObject.InterpolateState.Interpolating)
			{
				return;
			}
			float t = Mathf.Clamp((this.interpTime - this.interpDt) / this.interpTime, 0f, 1f);
			transform.transform.position = Vector3.Lerp(this.interpStartPos, transform2.transform.position, t);
			transform.transform.rotation = Quaternion.Slerp(this.interpStartRot, transform2.transform.rotation, t);
			this.interpDt -= Time.deltaTime;
			if (this.interpDt <= 0f)
			{
				transform.parent = transform2;
				this.interpState = LegacyTransferrableObject.InterpolateState.None;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				transform.localScale = Vector3.one;
				if (this.flipOnXForLeftHand && this.currentState == TransferrableObject.PositionState.InLeftHand)
				{
					transform.localScale = new Vector3(-1f, 1f, 1f);
				}
				if (this.flipOnYForLeftHand && this.currentState == TransferrableObject.PositionState.InLeftHand)
				{
					transform.localScale = new Vector3(1f, -1f, 1f);
				}
			}
		}
		else if (transform2 != transform.parent)
		{
			if (Time.frameCount == this.enabledOnFrame)
			{
				transform.parent = transform2;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				return;
			}
			this.interpState = LegacyTransferrableObject.InterpolateState.Interpolating;
			this.interpDt = this.interpTime;
			this.interpStartPos = transform.transform.position;
			this.interpStartRot = transform.transform.rotation;
			return;
		}
	}

	// Token: 0x06001A92 RID: 6802 RVA: 0x0008DF81 File Offset: 0x0008C181
	public void DropItem()
	{
		base.transform.parent = null;
	}

	// Token: 0x06001A93 RID: 6803 RVA: 0x0008DF90 File Offset: 0x0008C190
	protected virtual void LateUpdateShared()
	{
		this.disableItem = true;
		for (int i = 0; i < this.targetRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			if (this.targetRig.ActiveTransferrableObjectIndex(i) == this.myIndex)
			{
				this.disableItem = false;
				break;
			}
		}
		if (this.disableItem)
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (this.previousState != this.currentState && this.detatchOnGrab && this.InHand())
		{
			base.transform.parent = null;
		}
		if (this.currentState != TransferrableObject.PositionState.Dropped)
		{
			this.UpdateFollowXform();
			return;
		}
		if (this.canDrop)
		{
			this.DropItem();
		}
	}

	// Token: 0x06001A94 RID: 6804 RVA: 0x0008E038 File Offset: 0x0008C238
	protected void ResetXf()
	{
		if (this.canDrop)
		{
			Transform transform = this.DefaultAnchor();
			if (base.transform != transform && base.transform.parent != transform)
			{
				base.transform.parent = transform;
			}
			base.transform.localPosition = this.initOffset;
			base.transform.localRotation = this.initRotation;
		}
	}

	// Token: 0x06001A95 RID: 6805 RVA: 0x0008E0A3 File Offset: 0x0008C2A3
	protected void ReDock()
	{
		if (this.IsMyItem())
		{
			this.currentState = this.initState;
		}
		this.ResetXf();
	}

	// Token: 0x06001A96 RID: 6806 RVA: 0x0008E0C0 File Offset: 0x0008C2C0
	private void HandleLocalInput()
	{
		GameObject[] array;
		if (!this.InHand())
		{
			array = this.gameObjectsActiveOnlyWhileHeld;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			return;
		}
		array = this.gameObjectsActiveOnlyWhileHeld;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(true);
		}
		XRNode node = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? XRNode.LeftHand : XRNode.RightHand;
		this.indexTrigger = ControllerInputPoller.TriggerFloat(node);
		bool flag = !this.latched && this.indexTrigger >= this.myThreshold;
		bool flag2 = this.latched && this.indexTrigger < this.myThreshold - this.hysterisis;
		if (flag || this.testActivate)
		{
			this.testActivate = false;
			if (this.CanActivate())
			{
				this.OnActivate();
				return;
			}
		}
		else if (flag2 || this.testDeactivate)
		{
			this.testDeactivate = false;
			if (this.CanDeactivate())
			{
				this.OnDeactivate();
			}
		}
	}

	// Token: 0x06001A97 RID: 6807 RVA: 0x0008E1AC File Offset: 0x0008C3AC
	protected virtual void LateUpdateLocal()
	{
		this.wasHover = this.isHover;
		this.isHover = false;
		if (PhotonNetwork.InRoom)
		{
			this.myRig.SetTransferrablePosStates(this.objectIndex, this.currentState);
			this.myRig.SetTransferrableItemStates(this.objectIndex, this.itemState);
		}
		this.targetRig = this.myRig;
		this.HandleLocalInput();
	}

	// Token: 0x06001A98 RID: 6808 RVA: 0x0008E214 File Offset: 0x0008C414
	protected virtual void LateUpdateReplicated()
	{
		this.currentState = this.myOnlineRig.TransferrablePosStates(this.objectIndex);
		if (this.currentState == TransferrableObject.PositionState.Dropped && !this.canDrop && !this.shareable)
		{
			if (this.previousState == TransferrableObject.PositionState.None)
			{
				base.gameObject.SetActive(false);
			}
			this.currentState = this.previousState;
		}
		this.itemState = this.myOnlineRig.TransferrableItemStates(this.objectIndex);
		this.targetRig = this.myOnlineRig;
		if (this.myOnlineRig != null)
		{
			bool flag = true;
			for (int i = 0; i < this.myOnlineRig.ActiveTransferrableObjectIndexLength(); i++)
			{
				if (this.myOnlineRig.ActiveTransferrableObjectIndex(i) == this.myIndex)
				{
					flag = false;
					GameObject[] array = this.gameObjectsActiveOnlyWhileHeld;
					for (int j = 0; j < array.Length; j++)
					{
						array[j].SetActive(this.InHand());
					}
				}
			}
			if (flag)
			{
				base.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06001A99 RID: 6809 RVA: 0x0008E306 File Offset: 0x0008C506
	public virtual void ResetToDefaultState()
	{
		this.canAutoGrabLeft = true;
		this.canAutoGrabRight = true;
		this.wasHover = false;
		this.isHover = false;
		this.ResetXf();
	}

	// Token: 0x06001A9A RID: 6810 RVA: 0x0008E32C File Offset: 0x0008C52C
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!this.IsMyItem())
		{
			return;
		}
		if (!(grabbingHand == this.interactor.leftHand) || this.currentState == TransferrableObject.PositionState.OnLeftArm)
		{
			if (grabbingHand == this.interactor.rightHand && this.currentState != TransferrableObject.PositionState.OnRightArm)
			{
				if (this.currentState == TransferrableObject.PositionState.InLeftHand && this.disableStealing)
				{
					return;
				}
				this.canAutoGrabRight = false;
				this.currentState = TransferrableObject.PositionState.InRightHand;
				EquipmentInteractor.instance.UpdateHandEquipment(this, false);
				GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			}
			return;
		}
		if (this.currentState == TransferrableObject.PositionState.InRightHand && this.disableStealing)
		{
			return;
		}
		this.canAutoGrabLeft = false;
		this.currentState = TransferrableObject.PositionState.InLeftHand;
		EquipmentInteractor.instance.UpdateHandEquipment(this, true);
		GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
	}

	// Token: 0x06001A9B RID: 6811 RVA: 0x0008E434 File Offset: 0x0008C634
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!this.IsMyItem())
		{
			return false;
		}
		if (!this.CanDeactivate())
		{
			return false;
		}
		if (this.IsHeld() && ((releasingHand == EquipmentInteractor.instance.rightHand && EquipmentInteractor.instance.rightHandHeldEquipment != null && this == (LegacyTransferrableObject)EquipmentInteractor.instance.rightHandHeldEquipment) || (releasingHand == EquipmentInteractor.instance.leftHand && EquipmentInteractor.instance.leftHandHeldEquipment != null && this == (LegacyTransferrableObject)EquipmentInteractor.instance.leftHandHeldEquipment)))
		{
			if (releasingHand == EquipmentInteractor.instance.leftHand)
			{
				this.canAutoGrabLeft = true;
			}
			else
			{
				this.canAutoGrabRight = true;
			}
			if (zoneReleased != null)
			{
				bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand && zoneReleased.dropPosition == BodyDockPositions.DropPositions.LeftArm;
				bool flag2 = this.currentState == TransferrableObject.PositionState.InRightHand && zoneReleased.dropPosition == BodyDockPositions.DropPositions.RightArm;
				if (flag || flag2)
				{
					return false;
				}
				if (this.targetDock.DropZoneStorageUsed(zoneReleased.dropPosition) == -1 && zoneReleased.forBodyDock == this.targetDock && (zoneReleased.dropPosition & this.dockPositions) != BodyDockPositions.DropPositions.None)
				{
					this.storedZone = zoneReleased.dropPosition;
				}
			}
			this.DropItemCleanup();
			EquipmentInteractor.instance.UpdateHandEquipment(null, releasingHand == EquipmentInteractor.instance.leftHand);
			return true;
		}
		return false;
	}

	// Token: 0x06001A9C RID: 6812 RVA: 0x0008E5A8 File Offset: 0x0008C7A8
	public override void DropItemCleanup()
	{
		if (this.canDrop)
		{
			this.currentState = TransferrableObject.PositionState.Dropped;
			return;
		}
		BodyDockPositions.DropPositions dropPositions = this.storedZone;
		switch (dropPositions)
		{
		case BodyDockPositions.DropPositions.LeftArm:
			this.currentState = TransferrableObject.PositionState.OnLeftArm;
			return;
		case BodyDockPositions.DropPositions.RightArm:
			this.currentState = TransferrableObject.PositionState.OnRightArm;
			return;
		case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm:
			break;
		case BodyDockPositions.DropPositions.Chest:
			this.currentState = TransferrableObject.PositionState.OnChest;
			return;
		default:
			if (dropPositions == BodyDockPositions.DropPositions.LeftBack)
			{
				this.currentState = TransferrableObject.PositionState.OnLeftShoulder;
				return;
			}
			if (dropPositions != BodyDockPositions.DropPositions.RightBack)
			{
				return;
			}
			this.currentState = TransferrableObject.PositionState.OnRightShoulder;
			break;
		}
	}

	// Token: 0x06001A9D RID: 6813 RVA: 0x0008E61C File Offset: 0x0008C81C
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
		if (!this.IsMyItem())
		{
			return;
		}
		if (!this.wasHover)
		{
			GorillaTagger.Instance.StartVibration(hoveringHand == EquipmentInteractor.instance.leftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		}
		this.isHover = true;
	}

	// Token: 0x06001A9E RID: 6814 RVA: 0x0008E680 File Offset: 0x0008C880
	protected void ActivateItemFX(float hapticStrength, float hapticDuration, int soundIndex, float soundVolume)
	{
		bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
		VRRig vrrig = this.targetRig;
		if ((vrrig != null) ? vrrig.netView : null)
		{
			this.targetRig.rigSerializer.RPC_PlayHandTap(soundIndex, flag, 0.1f, default(PhotonMessageInfo));
		}
		this.myRig.PlayHandTapLocal(soundIndex, flag, soundVolume);
		GorillaTagger.Instance.StartVibration(flag, hapticStrength, hapticDuration);
	}

	// Token: 0x06001A9F RID: 6815 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void PlayNote(int note, float volume)
	{
	}

	// Token: 0x06001AA0 RID: 6816 RVA: 0x0008E6EC File Offset: 0x0008C8EC
	public virtual bool AutoGrabTrue(bool leftGrabbingHand)
	{
		if (!leftGrabbingHand)
		{
			return this.canAutoGrabRight;
		}
		return this.canAutoGrabLeft;
	}

	// Token: 0x06001AA1 RID: 6817 RVA: 0x0001D558 File Offset: 0x0001B758
	public virtual bool CanActivate()
	{
		return true;
	}

	// Token: 0x06001AA2 RID: 6818 RVA: 0x0001D558 File Offset: 0x0001B758
	public virtual bool CanDeactivate()
	{
		return true;
	}

	// Token: 0x06001AA3 RID: 6819 RVA: 0x0008E6FE File Offset: 0x0008C8FE
	public virtual void OnActivate()
	{
		this.latched = true;
	}

	// Token: 0x06001AA4 RID: 6820 RVA: 0x0008E707 File Offset: 0x0008C907
	public virtual void OnDeactivate()
	{
		this.latched = false;
	}

	// Token: 0x06001AA5 RID: 6821 RVA: 0x0008E710 File Offset: 0x0008C910
	public virtual bool IsMyItem()
	{
		return this.myRig != null && this.myRig.isOfflineVRRig;
	}

	// Token: 0x06001AA6 RID: 6822 RVA: 0x0008E730 File Offset: 0x0008C930
	protected virtual bool IsHeld()
	{
		return (EquipmentInteractor.instance.leftHandHeldEquipment != null && (LegacyTransferrableObject)EquipmentInteractor.instance.leftHandHeldEquipment == this) || (EquipmentInteractor.instance.rightHandHeldEquipment != null && (LegacyTransferrableObject)EquipmentInteractor.instance.rightHandHeldEquipment == this);
	}

	// Token: 0x06001AA7 RID: 6823 RVA: 0x0008E78D File Offset: 0x0008C98D
	public bool InHand()
	{
		return this.currentState == TransferrableObject.PositionState.InLeftHand || this.currentState == TransferrableObject.PositionState.InRightHand;
	}

	// Token: 0x06001AA8 RID: 6824 RVA: 0x0008E7A3 File Offset: 0x0008C9A3
	public bool Dropped()
	{
		return this.currentState == TransferrableObject.PositionState.Dropped;
	}

	// Token: 0x06001AA9 RID: 6825 RVA: 0x0008E7B2 File Offset: 0x0008C9B2
	public bool InLeftHand()
	{
		return this.currentState == TransferrableObject.PositionState.InLeftHand;
	}

	// Token: 0x06001AAA RID: 6826 RVA: 0x0008E7BD File Offset: 0x0008C9BD
	public bool InRightHand()
	{
		return this.currentState == TransferrableObject.PositionState.InRightHand;
	}

	// Token: 0x06001AAB RID: 6827 RVA: 0x0008E7C8 File Offset: 0x0008C9C8
	public bool OnChest()
	{
		return this.currentState == TransferrableObject.PositionState.OnChest;
	}

	// Token: 0x06001AAC RID: 6828 RVA: 0x0008E7D4 File Offset: 0x0008C9D4
	public bool OnShoulder()
	{
		return this.currentState == TransferrableObject.PositionState.OnLeftShoulder || this.currentState == TransferrableObject.PositionState.OnRightShoulder;
	}

	// Token: 0x06001AAD RID: 6829 RVA: 0x0008E7EC File Offset: 0x0008C9EC
	protected NetPlayer OwningPlayer()
	{
		if (this.myRig == null)
		{
			return this.myOnlineRig.netView.Owner;
		}
		return NetworkSystem.Instance.LocalPlayer;
	}

	// Token: 0x040022C5 RID: 8901
	protected EquipmentInteractor interactor;

	// Token: 0x040022C6 RID: 8902
	public VRRig myRig;

	// Token: 0x040022C7 RID: 8903
	public VRRig myOnlineRig;

	// Token: 0x040022C8 RID: 8904
	public bool latched;

	// Token: 0x040022C9 RID: 8905
	private float indexTrigger;

	// Token: 0x040022CA RID: 8906
	public bool testActivate;

	// Token: 0x040022CB RID: 8907
	public bool testDeactivate;

	// Token: 0x040022CC RID: 8908
	public float myThreshold = 0.8f;

	// Token: 0x040022CD RID: 8909
	public float hysterisis = 0.05f;

	// Token: 0x040022CE RID: 8910
	public bool flipOnXForLeftHand;

	// Token: 0x040022CF RID: 8911
	public bool flipOnYForLeftHand;

	// Token: 0x040022D0 RID: 8912
	public bool flipOnXForLeftArm;

	// Token: 0x040022D1 RID: 8913
	public bool disableStealing;

	// Token: 0x040022D2 RID: 8914
	private TransferrableObject.PositionState initState;

	// Token: 0x040022D3 RID: 8915
	public TransferrableObject.ItemStates itemState;

	// Token: 0x040022D4 RID: 8916
	public BodyDockPositions.DropPositions storedZone;

	// Token: 0x040022D5 RID: 8917
	protected TransferrableObject.PositionState previousState;

	// Token: 0x040022D6 RID: 8918
	public TransferrableObject.PositionState currentState;

	// Token: 0x040022D7 RID: 8919
	public BodyDockPositions.DropPositions dockPositions;

	// Token: 0x040022D8 RID: 8920
	public VRRig targetRig;

	// Token: 0x040022D9 RID: 8921
	public BodyDockPositions targetDock;

	// Token: 0x040022DA RID: 8922
	private VRRigAnchorOverrides anchorOverrides;

	// Token: 0x040022DB RID: 8923
	public bool canAutoGrabLeft;

	// Token: 0x040022DC RID: 8924
	public bool canAutoGrabRight;

	// Token: 0x040022DD RID: 8925
	public int objectIndex;

	// Token: 0x040022DE RID: 8926
	[Tooltip("In Holdables.prefab, assign to the parent of this transform.\nExample: 'Holdables/YellowHandBootsRight' is the anchor of 'Holdables/YellowHandBootsRight/YELLOW HAND BOOTS'")]
	public Transform anchor;

	// Token: 0x040022DF RID: 8927
	[Tooltip("In Holdables.prefab, assign to the Collider to grab this object")]
	public InteractionPoint gripInteractor;

	// Token: 0x040022E0 RID: 8928
	[Tooltip("(Optional) Use this to override the transform used when the object is in the hand.\nExample: 'GHOST BALLOON' uses child 'grabPtAnchor' which is the end of the balloon's string.")]
	public Transform grabAnchor;

	// Token: 0x040022E1 RID: 8929
	public int myIndex;

	// Token: 0x040022E2 RID: 8930
	[Tooltip("(Optional)")]
	public GameObject[] gameObjectsActiveOnlyWhileHeld;

	// Token: 0x040022E3 RID: 8931
	protected GameObject worldShareableInstance;

	// Token: 0x040022E4 RID: 8932
	private float interpTime = 0.1f;

	// Token: 0x040022E5 RID: 8933
	private float interpDt;

	// Token: 0x040022E6 RID: 8934
	private Vector3 interpStartPos;

	// Token: 0x040022E7 RID: 8935
	private Quaternion interpStartRot;

	// Token: 0x040022E8 RID: 8936
	protected int enabledOnFrame = -1;

	// Token: 0x040022E9 RID: 8937
	private Vector3 initOffset;

	// Token: 0x040022EA RID: 8938
	private Quaternion initRotation;

	// Token: 0x040022EB RID: 8939
	public bool canDrop;

	// Token: 0x040022EC RID: 8940
	public bool shareable;

	// Token: 0x040022ED RID: 8941
	public bool detatchOnGrab;

	// Token: 0x040022EE RID: 8942
	private bool wasHover;

	// Token: 0x040022EF RID: 8943
	private bool isHover;

	// Token: 0x040022F0 RID: 8944
	private bool disableItem;

	// Token: 0x040022F1 RID: 8945
	public const int kPositionStateCount = 8;

	// Token: 0x040022F2 RID: 8946
	public LegacyTransferrableObject.InterpolateState interpState;

	// Token: 0x02000440 RID: 1088
	public enum InterpolateState
	{
		// Token: 0x040022F4 RID: 8948
		None,
		// Token: 0x040022F5 RID: 8949
		Interpolating
	}
}
