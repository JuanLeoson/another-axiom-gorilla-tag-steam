using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using JetBrains.Annotations;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

// Token: 0x0200045D RID: 1117
public class TransferrableObject : HoldableObject, ISelfValidator, IRequestableOwnershipGuardCallbacks, IPreDisable, ISpawnable, IBuildValidation
{
	// Token: 0x06001B7C RID: 7036 RVA: 0x000932D4 File Offset: 0x000914D4
	public void FixTransformOverride()
	{
		this.transferrableItemSlotTransformOverride = base.GetComponent<TransferrableItemSlotTransformOverride>();
	}

	// Token: 0x06001B7D RID: 7037 RVA: 0x000023F5 File Offset: 0x000005F5
	public void Validate(SelfValidationResult result)
	{
	}

	// Token: 0x170002F7 RID: 759
	// (get) Token: 0x06001B7E RID: 7038 RVA: 0x000932E2 File Offset: 0x000914E2
	// (set) Token: 0x06001B7F RID: 7039 RVA: 0x000932EA File Offset: 0x000914EA
	public VRRig myRig
	{
		get
		{
			return this._myRig;
		}
		private set
		{
			this._myRig = value;
		}
	}

	// Token: 0x170002F8 RID: 760
	// (get) Token: 0x06001B80 RID: 7040 RVA: 0x000932F3 File Offset: 0x000914F3
	// (set) Token: 0x06001B81 RID: 7041 RVA: 0x000932FB File Offset: 0x000914FB
	public bool isMyRigValid { get; private set; }

	// Token: 0x170002F9 RID: 761
	// (get) Token: 0x06001B82 RID: 7042 RVA: 0x00093304 File Offset: 0x00091504
	// (set) Token: 0x06001B83 RID: 7043 RVA: 0x0009330C File Offset: 0x0009150C
	public VRRig myOnlineRig
	{
		get
		{
			return this._myOnlineRig;
		}
		private set
		{
			this._myOnlineRig = value;
			this.isMyOnlineRigValid = true;
		}
	}

	// Token: 0x170002FA RID: 762
	// (get) Token: 0x06001B84 RID: 7044 RVA: 0x0009331C File Offset: 0x0009151C
	// (set) Token: 0x06001B85 RID: 7045 RVA: 0x00093324 File Offset: 0x00091524
	public bool isMyOnlineRigValid { get; private set; }

	// Token: 0x06001B86 RID: 7046 RVA: 0x00093330 File Offset: 0x00091530
	public void SetTargetRig(VRRig rig)
	{
		if (rig == null)
		{
			this.targetRigSet = false;
			if (this.isSceneObject)
			{
				this.targetRig = rig;
				this.targetDockPositions = null;
				this.anchorOverrides = null;
				return;
			}
			if (this.myRig)
			{
				this.SetTargetRig(this.myRig);
			}
			if (this.myOnlineRig)
			{
				this.SetTargetRig(this.myOnlineRig);
			}
			return;
		}
		else
		{
			this.targetRigSet = true;
			this.targetRig = rig;
			BodyDockPositions component = rig.GetComponent<BodyDockPositions>();
			VRRigAnchorOverrides component2 = rig.GetComponent<VRRigAnchorOverrides>();
			if (!component)
			{
				Debug.LogError("There is no dock attached to this rig", this);
				return;
			}
			if (!component2)
			{
				Debug.LogError("There is no overrides attached to this rig", this);
				return;
			}
			this.anchorOverrides = component2;
			this.targetDockPositions = component;
			if (this.interpState == TransferrableObject.InterpolateState.Interpolating)
			{
				this.interpState = TransferrableObject.InterpolateState.None;
			}
			return;
		}
	}

	// Token: 0x170002FB RID: 763
	// (get) Token: 0x06001B87 RID: 7047 RVA: 0x00093400 File Offset: 0x00091600
	public bool IsLocalOwnedWorldShareable
	{
		get
		{
			return this.worldShareableInstance && this.worldShareableInstance.guard.isTrulyMine;
		}
	}

	// Token: 0x06001B88 RID: 7048 RVA: 0x00093424 File Offset: 0x00091624
	public void WorldShareableRequestOwnership()
	{
		if (this.worldShareableInstance != null && !this.worldShareableInstance.guard.isMine)
		{
			this.worldShareableInstance.guard.RequestOwnershipImmediately(delegate
			{
			});
		}
	}

	// Token: 0x170002FC RID: 764
	// (get) Token: 0x06001B89 RID: 7049 RVA: 0x00093480 File Offset: 0x00091680
	// (set) Token: 0x06001B8A RID: 7050 RVA: 0x00093488 File Offset: 0x00091688
	public bool isRigidbodySet { get; private set; }

	// Token: 0x170002FD RID: 765
	// (get) Token: 0x06001B8B RID: 7051 RVA: 0x00093491 File Offset: 0x00091691
	// (set) Token: 0x06001B8C RID: 7052 RVA: 0x00093499 File Offset: 0x00091699
	public bool shouldUseGravity { get; private set; }

	// Token: 0x06001B8D RID: 7053 RVA: 0x000934A2 File Offset: 0x000916A2
	protected virtual void Awake()
	{
		if (this.isSceneObject)
		{
			this.IsSpawned = true;
			this.OnSpawn(null);
		}
	}

	// Token: 0x170002FE RID: 766
	// (get) Token: 0x06001B8E RID: 7054 RVA: 0x000934BA File Offset: 0x000916BA
	// (set) Token: 0x06001B8F RID: 7055 RVA: 0x000934C2 File Offset: 0x000916C2
	public bool IsSpawned { get; set; }

	// Token: 0x170002FF RID: 767
	// (get) Token: 0x06001B90 RID: 7056 RVA: 0x000934CB File Offset: 0x000916CB
	// (set) Token: 0x06001B91 RID: 7057 RVA: 0x000934D3 File Offset: 0x000916D3
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06001B92 RID: 7058 RVA: 0x000934DC File Offset: 0x000916DC
	public virtual void OnSpawn(VRRig rig)
	{
		try
		{
			if (!this.isSceneObject)
			{
				if (!rig)
				{
					Debug.LogError("Disabling TransferrableObject because could not find VRRig! \"" + base.transform.GetPath() + "\"", this);
					base.enabled = false;
					this.isMyRigValid = false;
					this.isMyOnlineRigValid = false;
					return;
				}
				this.myRig = (rig.isOfflineVRRig ? rig : null);
				this.myOnlineRig = (rig.isOfflineVRRig ? null : rig);
			}
			else
			{
				this.myRig = null;
				this.myOnlineRig = null;
			}
			this.isMyRigValid = true;
			this.isMyOnlineRigValid = true;
			this.targetDockPositions = base.GetComponentInParent<BodyDockPositions>();
			this.anchor = base.transform.parent;
			if (this.rigidbodyInstance == null)
			{
				this.rigidbodyInstance = base.GetComponent<Rigidbody>();
			}
			if (this.rigidbodyInstance != null)
			{
				this.isRigidbodySet = true;
				this.shouldUseGravity = this.rigidbodyInstance.useGravity;
			}
			this.audioSrc = base.GetComponent<AudioSource>();
			this.latched = false;
			if (!this.positionInitialized)
			{
				this.SetInitMatrix();
				this.positionInitialized = true;
			}
			if (this.anchor == null)
			{
				this.InitialDockObject = base.transform.parent;
			}
			else
			{
				this.InitialDockObject = this.anchor.parent;
			}
			this.isGrabAnchorSet = (this.grabAnchor != null);
			if (this.isSceneObject)
			{
				foreach (ISpawnable spawnable in base.GetComponentsInChildren<ISpawnable>(true))
				{
					if (spawnable != this)
					{
						spawnable.IsSpawned = true;
						spawnable.CosmeticSelectedSide = this.CosmeticSelectedSide;
						spawnable.OnSpawn(this.myRig);
					}
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception, this);
			base.enabled = false;
			base.gameObject.SetActive(false);
			Debug.LogError("TransferrableObject: Disabled & deactivated self because of the exception logged above. Path: " + base.transform.GetPathQ(), this);
		}
	}

	// Token: 0x06001B93 RID: 7059 RVA: 0x000936D8 File Offset: 0x000918D8
	public virtual void OnDespawn()
	{
		try
		{
			if (!this.isSceneObject)
			{
				foreach (ISpawnable spawnable in base.GetComponentsInChildren<ISpawnable>(true))
				{
					if (spawnable != this)
					{
						spawnable.IsSpawned = false;
						spawnable.OnDespawn();
					}
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception, this);
			base.enabled = false;
			base.gameObject.SetActive(false);
			Debug.LogError("TransferrableObject: Disabled & deactivated self because of the exception logged above. Path: " + base.transform.GetPathQ(), this);
		}
	}

	// Token: 0x06001B94 RID: 7060 RVA: 0x00093760 File Offset: 0x00091960
	private void SetInitMatrix()
	{
		this.initMatrix = base.transform.LocalMatrixRelativeToParentWithScale();
		if (this.handPoseLeft != null)
		{
			base.transform.localRotation = TransferrableObject.handPoseLeftReferenceRotation * Quaternion.Inverse(this.handPoseLeft.localRotation);
			base.transform.position += base.transform.parent.TransformPoint(TransferrableObject.handPoseLeftReferencePoint) - this.handPoseLeft.transform.position;
			this.leftHandMatrix = base.transform.LocalMatrixRelativeToParentWithScale();
		}
		else
		{
			this.leftHandMatrix = this.initMatrix;
		}
		if (this.handPoseRight != null)
		{
			base.transform.localRotation = TransferrableObject.handPoseRightReferenceRotation * Quaternion.Inverse(this.handPoseRight.localRotation);
			base.transform.position += base.transform.parent.TransformPoint(TransferrableObject.handPoseRightReferencePoint) - this.handPoseRight.transform.position;
			this.rightHandMatrix = base.transform.LocalMatrixRelativeToParentWithScale();
		}
		else
		{
			this.rightHandMatrix = this.initMatrix;
		}
		base.transform.localPosition = this.initMatrix.Position();
		base.transform.localRotation = this.initMatrix.Rotation();
		this.positionInitialized = true;
	}

	// Token: 0x06001B95 RID: 7061 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void Start()
	{
	}

	// Token: 0x06001B96 RID: 7062 RVA: 0x000938D8 File Offset: 0x00091AD8
	internal virtual void OnEnable()
	{
		try
		{
			if (!ApplicationQuittingState.IsQuitting)
			{
				RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
				RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
				if (!this.isSceneObject && !CosmeticsV2Spawner_Dirty.allPartsInstantiated)
				{
					Debug.LogError("`TransferrableObject.OnEnable()` was called before allPartsInstantiated was true. Path: " + base.transform.GetPathQ(), this);
					if (!this._isListeningFor_OnPostInstantiateAllPrefabs2)
					{
						this._isListeningFor_OnPostInstantiateAllPrefabs2 = true;
						CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs2 = (Action)Delegate.Combine(CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs2, new Action(this.OnEnable_AfterAllCosmeticsSpawnedOrIsSceneObject));
					}
				}
				else
				{
					this.OnEnable_AfterAllCosmeticsSpawnedOrIsSceneObject();
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception, this);
			base.enabled = false;
			base.gameObject.SetActive(false);
			Debug.LogError("TransferrableObject: Disabled & deactivated self because of the exception logged above. Path: " + base.transform.GetPathQ(), this);
		}
	}

	// Token: 0x06001B97 RID: 7063 RVA: 0x000939D4 File Offset: 0x00091BD4
	public virtual void OnEnable_AfterAllCosmeticsSpawnedOrIsSceneObject()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (!base.enabled)
		{
			base.gameObject.SetActive(false);
			return;
		}
		this._isListeningFor_OnPostInstantiateAllPrefabs2 = false;
		CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs2 = (Action)Delegate.Remove(CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs2, new Action(this.OnEnable_AfterAllCosmeticsSpawnedOrIsSceneObject));
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		try
		{
			TransferrableObjectManager.Register(this);
			this.transferrableItemSlotTransformOverride = base.GetComponent<TransferrableItemSlotTransformOverride>();
			if (!this.positionInitialized)
			{
				this.SetInitMatrix();
				this.positionInitialized = true;
			}
			if (this.isSceneObject)
			{
				if (!this.worldShareableInstance)
				{
					Debug.LogError("Missing Sharable Instance on Scene enabled object: " + base.gameObject.name);
				}
				else
				{
					this.worldShareableInstance.SyncToSceneObject(this);
					this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().AddCallbackTarget(this);
				}
			}
			else
			{
				if (!this.isSceneObject && !this.myRig && !this.myOnlineRig && !this.ownerRig)
				{
					this.ownerRig = base.GetComponentInParent<VRRig>(true);
					if (this.ownerRig.isOfflineVRRig)
					{
						this.myRig = this.ownerRig;
					}
					else
					{
						this.myOnlineRig = this.ownerRig;
					}
				}
				if (!this.myRig && this.myOnlineRig)
				{
					this.ownerRig = this.myOnlineRig;
					this.SetTargetRig(this.myOnlineRig);
				}
				if (this.myRig == null && this.myOnlineRig == null)
				{
					if (!this.isSceneObject)
					{
						base.gameObject.SetActive(false);
					}
				}
				else
				{
					this.objectIndex = this.targetDockPositions.ReturnTransferrableItemIndex(this.myIndex);
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
					else if (this.currentState == TransferrableObject.PositionState.OnChest)
					{
						this.storedZone = BodyDockPositions.DropPositions.Chest;
					}
					if (this.IsLocalObject())
					{
						this.ownerRig = GorillaTagger.Instance.offlineVRRig;
						this.SetTargetRig(GorillaTagger.Instance.offlineVRRig);
					}
					if (this.objectIndex == -1)
					{
						base.gameObject.SetActive(false);
					}
					else
					{
						if (this.currentState == TransferrableObject.PositionState.OnLeftArm && this.flipOnXForLeftArm)
						{
							Transform transform = this.GetAnchor(this.currentState);
							transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
						}
						this.initState = this.currentState;
						this.enabledOnFrame = Time.frameCount;
						this.startInterpolation = true;
						if (NetworkSystem.Instance.InRoom)
						{
							if (this.canDrop || this.shareable)
							{
								this.SpawnTransferableObjectViews();
								if (this.myRig)
								{
									if (this.myRig != null && this.worldShareableInstance != null)
									{
										this.OnWorldShareableItemSpawn();
									}
								}
							}
						}
					}
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception, this);
			base.enabled = false;
			base.gameObject.SetActive(false);
			Debug.LogError("TransferrableObject: Disabled & deactivated self because of the exception logged above. Path: " + base.transform.GetPathQ(), this);
		}
	}

	// Token: 0x06001B98 RID: 7064 RVA: 0x00093D44 File Offset: 0x00091F44
	internal virtual void OnDisable()
	{
		TransferrableObjectManager.Unregister(this);
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		RoomSystem.JoinedRoomEvent -= new Action(this.OnJoinedRoom);
		RoomSystem.LeftRoomEvent -= new Action(this.OnLeftRoom);
		this._isListeningFor_OnPostInstantiateAllPrefabs2 = false;
		CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs2 = (Action)Delegate.Remove(CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs2, new Action(this.OnEnable_AfterAllCosmeticsSpawnedOrIsSceneObject));
		this.enabledOnFrame = -1;
		base.transform.localScale = Vector3.one;
		try
		{
			if (!this.isSceneObject && this.IsLocalObject() && this.worldShareableInstance && !this.IsMyItem())
			{
				this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RequestOwnershipImmediately(delegate
				{
				});
			}
			if (this.worldShareableInstance)
			{
				this.worldShareableInstance.Invalidate();
				this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RemoveCallbackTarget(this);
				if (this.targetDockPositions)
				{
					this.targetDockPositions.DeallocateSharableInstance(this.worldShareableInstance);
				}
				if (!this.isSceneObject)
				{
					this.worldShareableInstance = null;
				}
			}
			this.PlayDestroyedOrDisabledEffect();
			if (this.isSceneObject)
			{
				this.IsSpawned = false;
				this.OnDespawn();
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception, this);
			base.enabled = false;
			base.gameObject.SetActive(false);
			Debug.LogError("TransferrableObject: Disabled & deactivated self because of the exception logged above. Path: " + base.transform.GetPathQ(), this);
		}
	}

	// Token: 0x06001B99 RID: 7065 RVA: 0x00093EE4 File Offset: 0x000920E4
	protected new virtual void OnDestroy()
	{
		TransferrableObjectManager.Unregister(this);
	}

	// Token: 0x06001B9A RID: 7066 RVA: 0x00093EEC File Offset: 0x000920EC
	public void CleanupDisable()
	{
		this.currentState = TransferrableObject.PositionState.None;
		this.enabledOnFrame = -1;
		if (this.anchor)
		{
			this.anchor.parent = this.InitialDockObject;
			if (this.anchor != base.transform)
			{
				base.transform.parent = this.anchor;
			}
		}
		else
		{
			base.transform.parent = this.anchor;
		}
		this.interpState = TransferrableObject.InterpolateState.None;
		Transform transform = base.transform;
		Matrix4x4 defaultTransformationMatrix = this.GetDefaultTransformationMatrix();
		transform.SetLocalMatrixRelativeToParentWithXParity(defaultTransformationMatrix);
	}

	// Token: 0x06001B9B RID: 7067 RVA: 0x00093F77 File Offset: 0x00092177
	public virtual void PreDisable()
	{
		this.itemState = TransferrableObject.ItemStates.State0;
		this.currentState = TransferrableObject.PositionState.None;
		this.interpState = TransferrableObject.InterpolateState.None;
		this.ResetToDefaultState();
	}

	// Token: 0x06001B9C RID: 7068 RVA: 0x00093F94 File Offset: 0x00092194
	public virtual Matrix4x4 GetDefaultTransformationMatrix()
	{
		TransferrableObject.PositionState positionState = this.currentState;
		if (positionState == TransferrableObject.PositionState.InLeftHand)
		{
			return this.leftHandMatrix;
		}
		if (positionState != TransferrableObject.PositionState.InRightHand)
		{
			return this.initMatrix;
		}
		return this.rightHandMatrix;
	}

	// Token: 0x06001B9D RID: 7069 RVA: 0x00093FC6 File Offset: 0x000921C6
	public virtual bool ShouldBeKinematic()
	{
		if (this.detatchOnGrab)
		{
			return this.currentState != TransferrableObject.PositionState.Dropped && this.currentState != TransferrableObject.PositionState.InLeftHand && this.currentState != TransferrableObject.PositionState.InRightHand;
		}
		return this.currentState != TransferrableObject.PositionState.Dropped;
	}

	// Token: 0x06001B9E RID: 7070 RVA: 0x00094004 File Offset: 0x00092204
	private void SpawnShareableObject()
	{
		if (this.isSceneObject)
		{
			if (this.worldShareableInstance == null)
			{
				return;
			}
			this.worldShareableInstance.GetComponent<WorldShareableItem>().SetupSceneObjectOnNetwork(NetworkSystem.Instance.MasterClient);
			return;
		}
		else
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			this.SpawnTransferableObjectViews();
			if (!this.myRig)
			{
				return;
			}
			if (!this.canDrop && !this.shareable)
			{
				return;
			}
			if (this.myRig != null && this.worldShareableInstance != null)
			{
				this.OnWorldShareableItemSpawn();
			}
			return;
		}
	}

	// Token: 0x06001B9F RID: 7071 RVA: 0x00094098 File Offset: 0x00092298
	public void SpawnTransferableObjectViews()
	{
		NetPlayer owner = NetworkSystem.Instance.LocalPlayer;
		if (!this.ownerRig.isOfflineVRRig)
		{
			owner = this.ownerRig.creator;
		}
		if (this.worldShareableInstance == null)
		{
			this.worldShareableInstance = this.targetDockPositions.AllocateSharableInstance(this.storedZone, owner);
		}
		GorillaTagger.OnPlayerSpawned(delegate
		{
			this.worldShareableInstance.SetupSharableObject(this.myIndex, owner, this.transform);
		});
	}

	// Token: 0x06001BA0 RID: 7072 RVA: 0x0009411C File Offset: 0x0009231C
	public virtual void OnJoinedRoom()
	{
		if (this.isSceneObject)
		{
			this.worldShareableInstance == null;
			return;
		}
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		if (!this.canDrop && !this.shareable)
		{
			return;
		}
		this.SpawnTransferableObjectViews();
		if (!this.myRig)
		{
			return;
		}
		if (this.myRig != null && this.worldShareableInstance != null)
		{
			this.OnWorldShareableItemSpawn();
		}
	}

	// Token: 0x06001BA1 RID: 7073 RVA: 0x00094194 File Offset: 0x00092394
	public virtual void OnLeftRoom()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this.isSceneObject)
		{
			return;
		}
		if (!this.shareable && !this.allowWorldSharableInstance && !this.canDrop)
		{
			return;
		}
		if (base.gameObject.activeSelf && this.worldShareableInstance)
		{
			this.worldShareableInstance.Invalidate();
			this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RemoveCallbackTarget(this);
			if (this.targetDockPositions)
			{
				this.targetDockPositions.DeallocateSharableInstance(this.worldShareableInstance);
			}
			else
			{
				this.worldShareableInstance.ResetViews();
				ObjectPools.instance.Destroy(this.worldShareableInstance.gameObject);
			}
			this.worldShareableInstance = null;
		}
		if (!this.IsLocalObject())
		{
			this.OnItemDestroyedOrDisabled();
			base.gameObject.Disable();
			return;
		}
	}

	// Token: 0x06001BA2 RID: 7074 RVA: 0x00094262 File Offset: 0x00092462
	public bool IsLocalObject()
	{
		return this.myRig != null && this.myRig.isOfflineVRRig;
	}

	// Token: 0x06001BA3 RID: 7075 RVA: 0x00094279 File Offset: 0x00092479
	public void SetWorldShareableItem(WorldShareableItem item)
	{
		this.worldShareableInstance = item;
		this.OnWorldShareableItemSpawn();
	}

	// Token: 0x06001BA4 RID: 7076 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void OnWorldShareableItemSpawn()
	{
	}

	// Token: 0x06001BA5 RID: 7077 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void PlayDestroyedOrDisabledEffect()
	{
	}

	// Token: 0x06001BA6 RID: 7078 RVA: 0x00094288 File Offset: 0x00092488
	protected virtual void OnItemDestroyedOrDisabled()
	{
		if (this.worldShareableInstance)
		{
			this.worldShareableInstance.Invalidate();
			this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RemoveCallbackTarget(this);
			if (this.targetDockPositions)
			{
				this.targetDockPositions.DeallocateSharableInstance(this.worldShareableInstance);
			}
			Debug.LogError("Setting WSI to null in OnItemDestroyedOrDisabled", this);
			this.worldShareableInstance = null;
		}
		this.PlayDestroyedOrDisabledEffect();
		this.enabledOnFrame = -1;
		this.currentState = TransferrableObject.PositionState.None;
	}

	// Token: 0x06001BA7 RID: 7079 RVA: 0x00094302 File Offset: 0x00092502
	public virtual void TriggeredLateUpdate()
	{
		if (this.IsLocalObject() && this.canDrop)
		{
			this.LocalMyObjectValidation();
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
	}

	// Token: 0x06001BA8 RID: 7080 RVA: 0x00094336 File Offset: 0x00092536
	protected Transform DefaultAnchor()
	{
		if (this._isDefaultAnchorSet)
		{
			return this._defaultAnchor;
		}
		this._isDefaultAnchorSet = true;
		this._defaultAnchor = ((this.anchor == null) ? base.transform : this.anchor);
		return this._defaultAnchor;
	}

	// Token: 0x06001BA9 RID: 7081 RVA: 0x00094376 File Offset: 0x00092576
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

	// Token: 0x06001BAA RID: 7082 RVA: 0x000943A4 File Offset: 0x000925A4
	protected bool Attached()
	{
		bool flag = this.InHand() && this.detatchOnGrab;
		return !this.Dropped() && !flag;
	}

	// Token: 0x06001BAB RID: 7083 RVA: 0x000943D4 File Offset: 0x000925D4
	private Transform GetTargetStorageZone(BodyDockPositions.DropPositions state)
	{
		switch (state)
		{
		case BodyDockPositions.DropPositions.None:
			return null;
		case BodyDockPositions.DropPositions.LeftArm:
			return this.targetDockPositions.leftArmTransform;
		case BodyDockPositions.DropPositions.RightArm:
			return this.targetDockPositions.rightArmTransform;
		case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm:
		case BodyDockPositions.DropPositions.MaxDropPostions:
		case BodyDockPositions.DropPositions.RightArm | BodyDockPositions.DropPositions.Chest:
		case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm | BodyDockPositions.DropPositions.Chest:
			break;
		case BodyDockPositions.DropPositions.Chest:
			return this.targetDockPositions.chestTransform;
		case BodyDockPositions.DropPositions.LeftBack:
			return this.targetDockPositions.leftBackTransform;
		default:
			if (state == BodyDockPositions.DropPositions.RightBack)
			{
				return this.targetDockPositions.rightBackTransform;
			}
			break;
		}
		throw new ArgumentOutOfRangeException();
	}

	// Token: 0x06001BAC RID: 7084 RVA: 0x00094455 File Offset: 0x00092655
	public static Transform GetTargetDock(TransferrableObject.PositionState state, VRRig rig)
	{
		return TransferrableObject.GetTargetDock(state, rig.myBodyDockPositions, rig.GetComponent<VRRigAnchorOverrides>());
	}

	// Token: 0x06001BAD RID: 7085 RVA: 0x0009446C File Offset: 0x0009266C
	public static Transform GetTargetDock(TransferrableObject.PositionState state, BodyDockPositions dockPositions, VRRigAnchorOverrides anchorOverrides)
	{
		if (state <= TransferrableObject.PositionState.InRightHand)
		{
			switch (state)
			{
			case TransferrableObject.PositionState.OnLeftArm:
				return anchorOverrides.AnchorOverride(state, dockPositions.leftArmTransform);
			case TransferrableObject.PositionState.OnRightArm:
				return anchorOverrides.AnchorOverride(state, dockPositions.rightArmTransform);
			case TransferrableObject.PositionState.OnLeftArm | TransferrableObject.PositionState.OnRightArm:
				break;
			case TransferrableObject.PositionState.InLeftHand:
				return anchorOverrides.AnchorOverride(state, dockPositions.leftHandTransform);
			default:
				if (state == TransferrableObject.PositionState.InRightHand)
				{
					return anchorOverrides.AnchorOverride(state, dockPositions.rightHandTransform);
				}
				break;
			}
		}
		else
		{
			if (state == TransferrableObject.PositionState.OnChest)
			{
				return anchorOverrides.AnchorOverride(state, dockPositions.chestTransform);
			}
			if (state == TransferrableObject.PositionState.OnLeftShoulder)
			{
				return anchorOverrides.AnchorOverride(state, dockPositions.leftBackTransform);
			}
			if (state == TransferrableObject.PositionState.OnRightShoulder)
			{
				return anchorOverrides.AnchorOverride(state, dockPositions.rightBackTransform);
			}
		}
		return null;
	}

	// Token: 0x06001BAE RID: 7086 RVA: 0x00094510 File Offset: 0x00092710
	private void UpdateFollowXform()
	{
		if (!this.targetRigSet)
		{
			return;
		}
		Transform transform = this.GetAnchor(this.currentState);
		Transform transform2 = transform;
		try
		{
			transform2 = TransferrableObject.GetTargetDock(this.currentState, this.targetDockPositions, this.anchorOverrides);
		}
		catch
		{
			Debug.LogError("anchorOverrides or targetDock has been destroyed", this);
			this.SetTargetRig(null);
		}
		if (this.currentState != TransferrableObject.PositionState.Dropped && this.rigidbodyInstance && this.ShouldBeKinematic() && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
		if (this.detatchOnGrab && (this.currentState == TransferrableObject.PositionState.InLeftHand || this.currentState == TransferrableObject.PositionState.InRightHand))
		{
			base.transform.parent = null;
		}
		if (this.interpState == TransferrableObject.InterpolateState.None)
		{
			try
			{
				if (transform == null)
				{
					return;
				}
				this.startInterpolation |= (transform2 != transform.parent);
			}
			catch
			{
			}
			if (!this.startInterpolation && !this.isGrabAnchorSet && base.transform.parent != transform && transform != base.transform)
			{
				this.startInterpolation = true;
			}
			if (this.startInterpolation)
			{
				Vector3 position = base.transform.position;
				Quaternion rotation = base.transform.rotation;
				if (base.transform.parent != transform && transform != base.transform)
				{
					base.transform.parent = transform;
				}
				transform.parent = transform2;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				if (this.currentState == TransferrableObject.PositionState.InLeftHand)
				{
					if (this.flipOnXForLeftHand)
					{
						transform.localScale = new Vector3(-1f, 1f, 1f);
					}
					else if (this.flipOnYForLeftHand)
					{
						transform.localScale = new Vector3(1f, -1f, 1f);
					}
					else
					{
						transform.localScale = Vector3.one;
					}
				}
				else
				{
					transform.localScale = Vector3.one;
				}
				if (Time.frameCount == this.enabledOnFrame || Time.frameCount == this.enabledOnFrame + 1)
				{
					Matrix4x4 rhs = this.GetDefaultTransformationMatrix();
					if ((this.currentState != TransferrableObject.PositionState.InLeftHand || !(this.handPoseLeft != null)) && this.currentState == TransferrableObject.PositionState.InRightHand)
					{
						this.handPoseRight != null;
					}
					Matrix4x4 matrix4x;
					if (this.transferrableItemSlotTransformOverride && this.transferrableItemSlotTransformOverride.GetTransformFromPositionState(this.currentState, this.advancedGrabState, transform2, out matrix4x))
					{
						rhs = matrix4x;
					}
					Matrix4x4 matrix = transform.localToWorldMatrix * rhs;
					base.transform.SetLocalToWorldMatrixNoScale(matrix);
					base.transform.localScale = matrix.lossyScale;
				}
				else
				{
					this.interpState = TransferrableObject.InterpolateState.Interpolating;
					if (this.IsMyItem() && this.useGrabType == TransferrableObject.GrabType.Free)
					{
						bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
						if (!flag)
						{
							GameObject rightHand = EquipmentInteractor.instance.rightHand;
						}
						else
						{
							GameObject leftHand = EquipmentInteractor.instance.leftHand;
						}
						Transform targetDock = TransferrableObject.GetTargetDock(this.currentState, GorillaTagger.Instance.offlineVRRig);
						this.SetupMatrixForFreeGrab(position, rotation, targetDock, flag);
					}
					this.interpDt = this.interpTime;
					this.interpStartRot = rotation;
					this.interpStartPos = position;
					base.transform.position = position;
					base.transform.rotation = rotation;
				}
				this.startInterpolation = false;
			}
		}
		if (this.interpState == TransferrableObject.InterpolateState.Interpolating)
		{
			Matrix4x4 rhs2 = this.GetDefaultTransformationMatrix();
			if (this.transferrableItemSlotTransformOverride != null)
			{
				if (this.transferrableItemSlotTransformOverrideCachedMatrix == null)
				{
					Matrix4x4 value;
					this.transferrableItemSlotTransformOverrideApplicable = this.transferrableItemSlotTransformOverride.GetTransformFromPositionState(this.currentState, this.advancedGrabState, transform2, out value);
					this.transferrableItemSlotTransformOverrideCachedMatrix = new Matrix4x4?(value);
				}
				if (this.transferrableItemSlotTransformOverrideApplicable)
				{
					rhs2 = this.transferrableItemSlotTransformOverrideCachedMatrix.Value;
				}
			}
			float t = Mathf.Clamp((this.interpTime - this.interpDt) / this.interpTime, 0f, 1f);
			Mathf.SmoothStep(0f, 1f, t);
			Matrix4x4 matrix2 = transform.localToWorldMatrix * rhs2;
			Transform transform3 = base.transform;
			Vector3 vector = matrix2.Position();
			transform3.position = this.interpStartPos.LerpToUnclamped(vector, t);
			base.transform.rotation = Quaternion.Slerp(this.interpStartRot, matrix2.Rotation(), t);
			base.transform.localScale = rhs2.lossyScale;
			this.interpDt -= Time.deltaTime;
			if (this.interpDt <= 0f)
			{
				transform.parent = transform2;
				this.interpState = TransferrableObject.InterpolateState.None;
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
				matrix2 = transform.localToWorldMatrix * rhs2;
				base.transform.SetLocalToWorldMatrixNoScale(matrix2);
				base.transform.localScale = rhs2.lossyScale;
			}
		}
	}

	// Token: 0x06001BAF RID: 7087 RVA: 0x00094A50 File Offset: 0x00092C50
	public virtual void DropItem()
	{
		this.OnDocked.Invoke();
		if (EquipmentInteractor.instance.leftHandHeldEquipment == this)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			EquipmentInteractor.instance.UpdateHandEquipment(null, true);
		}
		if (EquipmentInteractor.instance.rightHandHeldEquipment == this)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			EquipmentInteractor.instance.UpdateHandEquipment(null, false);
		}
		this.currentState = TransferrableObject.PositionState.Dropped;
		if (this.worldShareableInstance)
		{
			this.worldShareableInstance.transferableObjectState = this.currentState;
		}
		if (this.canDrop)
		{
			base.transform.parent = null;
			if (this.anchor)
			{
				this.anchor.parent = this.InitialDockObject;
			}
			if (this.rigidbodyInstance && this.ShouldBeKinematic() && !this.rigidbodyInstance.isKinematic)
			{
				this.rigidbodyInstance.isKinematic = true;
			}
		}
	}

	// Token: 0x06001BB0 RID: 7088 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void OnStateChanged()
	{
	}

	// Token: 0x06001BB1 RID: 7089 RVA: 0x00094B84 File Offset: 0x00092D84
	protected virtual void LateUpdateShared()
	{
		this.disableItem = true;
		if (this.isSceneObject)
		{
			this.disableItem = false;
		}
		else
		{
			for (int i = 0; i < this.ownerRig.ActiveTransferrableObjectIndexLength(); i++)
			{
				if (this.ownerRig.ActiveTransferrableObjectIndex(i) == this.myIndex)
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
		}
		if (this.previousState != this.currentState)
		{
			this.previousState = this.currentState;
			if (!this.Attached())
			{
				base.transform.parent = null;
				if (!this.ShouldBeKinematic() && this.rigidbodyInstance.isKinematic)
				{
					this.rigidbodyInstance.isKinematic = false;
				}
			}
			if (this.currentState == TransferrableObject.PositionState.None)
			{
				this.ResetToHome();
			}
			this.transferrableItemSlotTransformOverrideCachedMatrix = null;
			if (this.interpState == TransferrableObject.InterpolateState.Interpolating)
			{
				this.interpState = TransferrableObject.InterpolateState.None;
			}
			this.OnStateChanged();
		}
		if (this.currentState == TransferrableObject.PositionState.Dropped)
		{
			if (!this.canDrop || this.allowReparenting)
			{
				goto IL_15A;
			}
			if (base.transform.parent != null)
			{
				base.transform.parent = null;
			}
			try
			{
				if (this.anchor != null && this.anchor.parent != this.InitialDockObject)
				{
					this.anchor.parent = this.InitialDockObject;
				}
				goto IL_15A;
			}
			catch
			{
				goto IL_15A;
			}
		}
		if (this.currentState != TransferrableObject.PositionState.None)
		{
			this.UpdateFollowXform();
		}
		IL_15A:
		if (!this.isRigidbodySet)
		{
			return;
		}
		if (this.rigidbodyInstance.isKinematic != this.ShouldBeKinematic())
		{
			this.rigidbodyInstance.isKinematic = this.ShouldBeKinematic();
			if (this.worldShareableInstance)
			{
				if (this.currentState == TransferrableObject.PositionState.Dropped)
				{
					this.worldShareableInstance.EnableRemoteSync = true;
					return;
				}
				this.worldShareableInstance.EnableRemoteSync = !this.ShouldBeKinematic();
			}
		}
	}

	// Token: 0x06001BB2 RID: 7090 RVA: 0x00094D64 File Offset: 0x00092F64
	public virtual void ResetToHome()
	{
		if (this.isSceneObject)
		{
			this.currentState = TransferrableObject.PositionState.None;
		}
		this.ResetXf();
		if (!this.isRigidbodySet)
		{
			return;
		}
		if (this.ShouldBeKinematic() && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
	}

	// Token: 0x06001BB3 RID: 7091 RVA: 0x00094DB0 File Offset: 0x00092FB0
	protected void ResetXf()
	{
		if (!this.positionInitialized)
		{
			this.initOffset = base.transform.localPosition;
			this.initRotation = base.transform.localRotation;
		}
		if (this.canDrop || this.allowWorldSharableInstance)
		{
			Transform transform = this.DefaultAnchor();
			if (base.transform != transform && base.transform.parent != transform)
			{
				base.transform.parent = transform;
			}
			if (this.ClearLocalPositionOnReset)
			{
				base.transform.localPosition = Vector3.zero;
				base.transform.localRotation = Quaternion.identity;
				base.transform.localScale = Vector3.one;
			}
			if (this.InitialDockObject)
			{
				this.anchor.localPosition = Vector3.zero;
				this.anchor.localRotation = Quaternion.identity;
				this.anchor.localScale = Vector3.one;
			}
			if (this.grabAnchor)
			{
				if (this.grabAnchor.parent != base.transform)
				{
					this.grabAnchor.parent = base.transform;
				}
				this.grabAnchor.localPosition = Vector3.zero;
				this.grabAnchor.localRotation = Quaternion.identity;
				this.grabAnchor.localScale = Vector3.one;
			}
			if (this.transferrableItemSlotTransformOverride)
			{
				Transform transformFromPositionState = this.transferrableItemSlotTransformOverride.GetTransformFromPositionState(this.currentState);
				if (transformFromPositionState)
				{
					base.transform.position = transformFromPositionState.position;
					base.transform.rotation = transformFromPositionState.rotation;
					return;
				}
				if (this.anchorOverrides != null)
				{
					Transform transform2 = this.GetAnchor(this.currentState);
					Transform targetDock = TransferrableObject.GetTargetDock(this.currentState, this.targetDockPositions, this.anchorOverrides);
					Matrix4x4 rhs = this.GetDefaultTransformationMatrix();
					Matrix4x4 matrix4x;
					if (this.transferrableItemSlotTransformOverride.GetTransformFromPositionState(this.currentState, this.advancedGrabState, targetDock, out matrix4x))
					{
						rhs = matrix4x;
					}
					Matrix4x4 matrix = transform2.localToWorldMatrix * rhs;
					base.transform.SetLocalToWorldMatrixNoScale(matrix);
					base.transform.localScale = matrix.lossyScale;
					return;
				}
			}
			else
			{
				base.transform.SetLocalMatrixRelativeToParent(this.GetDefaultTransformationMatrix());
			}
		}
	}

	// Token: 0x06001BB4 RID: 7092 RVA: 0x00094FF0 File Offset: 0x000931F0
	protected void ReDock()
	{
		if (this.IsMyItem())
		{
			this.currentState = this.initState;
		}
		if (this.rigidbodyInstance && this.ShouldBeKinematic() && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
		this.ResetXf();
	}

	// Token: 0x06001BB5 RID: 7093 RVA: 0x00095048 File Offset: 0x00093248
	private void HandleLocalInput()
	{
		Behaviour[] array2;
		if (this.Dropped())
		{
			foreach (GameObject gameObject in this.gameObjectsActiveOnlyWhileHeld)
			{
				if (gameObject.activeSelf)
				{
					gameObject.SetActive(false);
				}
			}
			array2 = this.behavioursEnabledOnlyWhileHeld;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].enabled = false;
			}
			foreach (GameObject gameObject2 in this.gameObjectsActiveOnlyWhileDocked)
			{
				if (gameObject2.activeSelf)
				{
					gameObject2.SetActive(false);
				}
			}
			array2 = this.behavioursEnabledOnlyWhileDocked;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].enabled = false;
			}
			return;
		}
		if (!this.InHand())
		{
			foreach (GameObject gameObject3 in this.gameObjectsActiveOnlyWhileHeld)
			{
				if (gameObject3.activeSelf)
				{
					gameObject3.SetActive(false);
				}
			}
			array2 = this.behavioursEnabledOnlyWhileHeld;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].enabled = false;
			}
			foreach (GameObject gameObject4 in this.gameObjectsActiveOnlyWhileDocked)
			{
				if (!gameObject4.activeSelf)
				{
					gameObject4.SetActive(true);
				}
			}
			array2 = this.behavioursEnabledOnlyWhileDocked;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].enabled = true;
			}
			return;
		}
		foreach (GameObject gameObject5 in this.gameObjectsActiveOnlyWhileHeld)
		{
			if (!gameObject5.activeSelf)
			{
				gameObject5.SetActive(true);
			}
		}
		array2 = this.behavioursEnabledOnlyWhileHeld;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].enabled = true;
		}
		foreach (GameObject gameObject6 in this.gameObjectsActiveOnlyWhileDocked)
		{
			if (gameObject6.activeSelf)
			{
				gameObject6.SetActive(false);
			}
		}
		array2 = this.behavioursEnabledOnlyWhileDocked;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].enabled = false;
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

	// Token: 0x06001BB6 RID: 7094 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void LocalMyObjectValidation()
	{
	}

	// Token: 0x06001BB7 RID: 7095 RVA: 0x000952D0 File Offset: 0x000934D0
	protected virtual void LocalPersistanceValidation()
	{
		if (this.maxDistanceFromOriginBeforeRespawn != 0f && Vector3.Distance(base.transform.position, this.originPoint.position) > this.maxDistanceFromOriginBeforeRespawn)
		{
			if (this.audioSrc != null && this.resetPositionAudioClip != null)
			{
				this.audioSrc.GTPlayOneShot(this.resetPositionAudioClip, 1f);
			}
			if (this.currentState != TransferrableObject.PositionState.Dropped)
			{
				this.DropItem();
				this.currentState = TransferrableObject.PositionState.Dropped;
			}
			base.transform.position = this.originPoint.position;
			if (!this.rigidbodyInstance.isKinematic)
			{
				this.rigidbodyInstance.velocity = Vector3.zero;
			}
		}
		if (this.rigidbodyInstance && this.rigidbodyInstance.velocity.sqrMagnitude > 10000f)
		{
			Debug.Log("Moving too fast, Assuming ive fallen out of the map. Ressetting position", this);
			this.ResetToHome();
		}
	}

	// Token: 0x06001BB8 RID: 7096 RVA: 0x000953D0 File Offset: 0x000935D0
	public void ObjectBeingTaken()
	{
		if (EquipmentInteractor.instance.leftHandHeldEquipment == this)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			EquipmentInteractor.instance.UpdateHandEquipment(null, true);
		}
		if (EquipmentInteractor.instance.rightHandHeldEquipment == this)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			EquipmentInteractor.instance.UpdateHandEquipment(null, false);
		}
	}

	// Token: 0x06001BB9 RID: 7097 RVA: 0x00095470 File Offset: 0x00093670
	protected virtual void LateUpdateLocal()
	{
		this.wasHover = this.isHover;
		this.isHover = false;
		this.LocalPersistanceValidation();
		if (NetworkSystem.Instance.InRoom)
		{
			if (!this.isSceneObject && this.IsLocalObject())
			{
				this.myRig.SetTransferrablePosStates(this.objectIndex, this.currentState);
				this.myRig.SetTransferrableItemStates(this.objectIndex, this.itemState);
				this.myRig.SetTransferrableDockPosition(this.objectIndex, this.storedZone);
			}
			if (this.worldShareableInstance)
			{
				this.worldShareableInstance.transferableObjectState = this.currentState;
				this.worldShareableInstance.transferableObjectItemState = this.itemState;
			}
		}
		this.HandleLocalInput();
	}

	// Token: 0x06001BBA RID: 7098 RVA: 0x00095530 File Offset: 0x00093730
	protected void LateUpdateReplicatedSceneObject()
	{
		if (this.myOnlineRig != null)
		{
			this.storedZone = this.myOnlineRig.TransferrableDockPosition(this.objectIndex);
		}
		if (this.worldShareableInstance != null)
		{
			this.currentState = this.worldShareableInstance.transferableObjectState;
			this.itemState = this.worldShareableInstance.transferableObjectItemState;
			this.worldShareableInstance.EnableRemoteSync = (!this.ShouldBeKinematic() || this.currentState == TransferrableObject.PositionState.Dropped);
		}
		if (this.isRigidbodySet && this.ShouldBeKinematic() && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
	}

	// Token: 0x06001BBB RID: 7099 RVA: 0x000955D4 File Offset: 0x000937D4
	protected virtual void LateUpdateReplicated()
	{
		if (this.isSceneObject || this.shareable)
		{
			this.LateUpdateReplicatedSceneObject();
			return;
		}
		if (this.myOnlineRig == null)
		{
			return;
		}
		this.currentState = this.myOnlineRig.TransferrablePosStates(this.objectIndex);
		if (!this.ValidateState(this.currentState))
		{
			if (this.previousState == TransferrableObject.PositionState.None)
			{
				base.gameObject.Disable();
			}
			this.currentState = this.previousState;
		}
		if (this.isRigidbodySet)
		{
			this.rigidbodyInstance.isKinematic = this.ShouldBeKinematic();
		}
		bool flag = true;
		this.itemState = this.myOnlineRig.TransferrableItemStates(this.objectIndex);
		this.storedZone = this.myOnlineRig.TransferrableDockPosition(this.objectIndex);
		int num = this.myOnlineRig.ActiveTransferrableObjectIndexLength();
		for (int i = 0; i < num; i++)
		{
			if (this.myOnlineRig.ActiveTransferrableObjectIndex(i) == this.myIndex)
			{
				flag = false;
				foreach (GameObject gameObject in this.gameObjectsActiveOnlyWhileHeld)
				{
					bool flag2 = this.InHand();
					if (gameObject.activeSelf != flag2)
					{
						gameObject.SetActive(flag2);
					}
				}
				Behaviour[] array2 = this.behavioursEnabledOnlyWhileHeld;
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j].enabled = this.InHand();
				}
				foreach (GameObject gameObject2 in this.gameObjectsActiveOnlyWhileDocked)
				{
					bool flag3 = this.InHand();
					if (gameObject2.activeSelf == flag3)
					{
						gameObject2.SetActive(!flag3);
					}
				}
				array2 = this.behavioursEnabledOnlyWhileDocked;
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j].enabled = !this.InHand();
				}
			}
		}
		if (flag)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001BBC RID: 7100 RVA: 0x000957A8 File Offset: 0x000939A8
	public virtual void ResetToDefaultState()
	{
		this.canAutoGrabLeft = true;
		this.canAutoGrabRight = true;
		this.wasHover = false;
		this.isHover = false;
		if (!this.IsLocalObject() && this.worldShareableInstance && !this.isSceneObject)
		{
			if (this.IsMyItem())
			{
				return;
			}
			this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RequestOwnershipImmediately(delegate
			{
			});
		}
		this.ResetXf();
	}

	// Token: 0x06001BBD RID: 7101 RVA: 0x0009582C File Offset: 0x00093A2C
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!(this.worldShareableInstance == null) && !this.worldShareableInstance.guard.isTrulyMine)
		{
			if (!this.IsGrabbable())
			{
				return;
			}
			this.worldShareableInstance.guard.RequestOwnershipImmediately(delegate
			{
			});
		}
		if (grabbingHand == EquipmentInteractor.instance.leftHand && this.currentState != TransferrableObject.PositionState.OnLeftArm)
		{
			if (this.currentState == TransferrableObject.PositionState.InRightHand && this.disableStealing)
			{
				return;
			}
			this.canAutoGrabLeft = false;
			this.interpState = TransferrableObject.InterpolateState.None;
			Debug.Log("<color=red>Setting current State</color>");
			this.currentState = TransferrableObject.PositionState.InLeftHand;
			if (this.transferrableItemSlotTransformOverride)
			{
				this.advancedGrabState = this.transferrableItemSlotTransformOverride.GetAdvancedItemStateFromHand(TransferrableObject.PositionState.InLeftHand, EquipmentInteractor.instance.leftHand.transform, TransferrableObject.GetTargetDock(this.currentState, GorillaTagger.Instance.offlineVRRig));
			}
			EquipmentInteractor.instance.UpdateHandEquipment(this, true);
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			this.OnUndocked.Invoke();
		}
		else if (grabbingHand == EquipmentInteractor.instance.rightHand && this.currentState != TransferrableObject.PositionState.OnRightArm)
		{
			if (this.currentState == TransferrableObject.PositionState.InLeftHand && this.disableStealing)
			{
				return;
			}
			this.canAutoGrabRight = false;
			this.interpState = TransferrableObject.InterpolateState.None;
			this.currentState = TransferrableObject.PositionState.InRightHand;
			if (this.transferrableItemSlotTransformOverride)
			{
				this.advancedGrabState = this.transferrableItemSlotTransformOverride.GetAdvancedItemStateFromHand(TransferrableObject.PositionState.InRightHand, EquipmentInteractor.instance.rightHand.transform, TransferrableObject.GetTargetDock(this.currentState, GorillaTagger.Instance.offlineVRRig));
			}
			EquipmentInteractor.instance.UpdateHandEquipment(this, false);
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			this.OnUndocked.Invoke();
		}
		if (this.rigidbodyInstance && !this.rigidbodyInstance.isKinematic && this.ShouldBeKinematic())
		{
			this.rigidbodyInstance.isKinematic = true;
		}
		PlayerGameEvents.GrabbedObject(this.interactEventName);
	}

	// Token: 0x06001BBE RID: 7102 RVA: 0x00095A84 File Offset: 0x00093C84
	private void SetupMatrixForFreeGrab(Vector3 worldPosition, Quaternion worldRotation, Transform attachPoint, bool leftHand)
	{
		Quaternion rotation = attachPoint.transform.rotation;
		Vector3 position = attachPoint.transform.position;
		Quaternion localRotation = Quaternion.Inverse(rotation) * worldRotation;
		Vector3 localPosition = Quaternion.Inverse(rotation) * (worldPosition - position);
		this.OnHandMatrixUpdate(localPosition, localRotation, leftHand);
	}

	// Token: 0x06001BBF RID: 7103 RVA: 0x00095AD7 File Offset: 0x00093CD7
	protected void SetupHandMatrix(Vector3 leftHandPos, Quaternion leftHandRot, Vector3 rightHandPos, Quaternion rightHandRot)
	{
		this.leftHandMatrix = Matrix4x4.TRS(leftHandPos, leftHandRot, Vector3.one);
		this.rightHandMatrix = Matrix4x4.TRS(rightHandPos, rightHandRot, Vector3.one);
	}

	// Token: 0x06001BC0 RID: 7104 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void OnHandMatrixUpdate(Vector3 localPosition, Quaternion localRotation, bool leftHand)
	{
	}

	// Token: 0x06001BC1 RID: 7105 RVA: 0x00095B00 File Offset: 0x00093D00
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (!this.IsMyItem())
		{
			return false;
		}
		if (!this.CanDeactivate())
		{
			return false;
		}
		if (!this.IsHeld())
		{
			return false;
		}
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
			if (this.targetDockPositions.DropZoneStorageUsed(zoneReleased.dropPosition) == -1 && zoneReleased.forBodyDock == this.targetDockPositions && (zoneReleased.dropPosition & this.dockPositions) != BodyDockPositions.DropPositions.None)
			{
				this.storedZone = zoneReleased.dropPosition;
			}
		}
		bool flag3 = false;
		this.interpState = TransferrableObject.InterpolateState.None;
		if (this.isSceneObject || this.canDrop || this.allowWorldSharableInstance)
		{
			if (!this.rigidbodyInstance)
			{
				return false;
			}
			if (this.worldShareableInstance)
			{
				this.worldShareableInstance.EnableRemoteSync = true;
			}
			if (!flag3)
			{
				this.currentState = TransferrableObject.PositionState.Dropped;
			}
			if (this.rigidbodyInstance.isKinematic && !this.ShouldBeKinematic())
			{
				this.rigidbodyInstance.isKinematic = false;
			}
			GorillaVelocityEstimator component = base.GetComponent<GorillaVelocityEstimator>();
			if (component != null && this.rigidbodyInstance != null)
			{
				this.rigidbodyInstance.velocity = component.linearVelocity;
				this.rigidbodyInstance.angularVelocity = component.angularVelocity;
			}
		}
		else
		{
			bool flag4 = this.allowWorldSharableInstance;
		}
		this.DropItemCleanup();
		EquipmentInteractor.instance.ForceDropEquipment(this);
		PlayerGameEvents.DroppedObject(this.interactEventName);
		return true;
	}

	// Token: 0x06001BC2 RID: 7106 RVA: 0x00095CB4 File Offset: 0x00093EB4
	public override void DropItemCleanup()
	{
		if (this.currentState == TransferrableObject.PositionState.Dropped)
		{
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

	// Token: 0x06001BC3 RID: 7107 RVA: 0x00095D24 File Offset: 0x00093F24
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
		if (!this.IsGrabbable())
		{
			return;
		}
		if (!this.wasHover)
		{
			GorillaTagger.Instance.StartVibration(hoveringHand == EquipmentInteractor.instance.leftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		}
		this.isHover = true;
	}

	// Token: 0x06001BC4 RID: 7108 RVA: 0x00095D88 File Offset: 0x00093F88
	protected void ActivateItemFX(float hapticStrength, float hapticDuration, int soundIndex, float soundVolume)
	{
		bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
		if (this.myRig.netView != null)
		{
			this.myRig.netView.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[]
			{
				soundIndex,
				flag,
				0.1f
			});
		}
		this.myRig.PlayHandTapLocal(soundIndex, flag, soundVolume);
		GorillaTagger.Instance.StartVibration(flag, hapticStrength, hapticDuration);
	}

	// Token: 0x06001BC5 RID: 7109 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void PlayNote(int note, float volume)
	{
	}

	// Token: 0x06001BC6 RID: 7110 RVA: 0x00095E09 File Offset: 0x00094009
	public virtual bool AutoGrabTrue(bool leftGrabbingHand)
	{
		if (!leftGrabbingHand)
		{
			return this.canAutoGrabRight;
		}
		return this.canAutoGrabLeft;
	}

	// Token: 0x06001BC7 RID: 7111 RVA: 0x0001D558 File Offset: 0x0001B758
	public virtual bool CanActivate()
	{
		return true;
	}

	// Token: 0x06001BC8 RID: 7112 RVA: 0x0001D558 File Offset: 0x0001B758
	public virtual bool CanDeactivate()
	{
		return true;
	}

	// Token: 0x06001BC9 RID: 7113 RVA: 0x00095E1B File Offset: 0x0009401B
	public virtual void OnActivate()
	{
		this.latched = true;
	}

	// Token: 0x06001BCA RID: 7114 RVA: 0x00095E24 File Offset: 0x00094024
	public virtual void OnDeactivate()
	{
		this.latched = false;
	}

	// Token: 0x06001BCB RID: 7115 RVA: 0x00095E2D File Offset: 0x0009402D
	public virtual bool IsMyItem()
	{
		return GorillaTagger.Instance == null || (this.targetRig != null && this.targetRig == GorillaTagger.Instance.offlineVRRig);
	}

	// Token: 0x06001BCC RID: 7116 RVA: 0x00095E57 File Offset: 0x00094057
	protected virtual bool IsHeld()
	{
		return EquipmentInteractor.instance != null && (EquipmentInteractor.instance.leftHandHeldEquipment == this || EquipmentInteractor.instance.rightHandHeldEquipment == this);
	}

	// Token: 0x06001BCD RID: 7117 RVA: 0x00095E84 File Offset: 0x00094084
	public virtual bool IsGrabbable()
	{
		return this.IsMyItem() || ((this.isSceneObject || this.shareable) && (this.isSceneObject || this.shareable) && (this.allowPlayerStealing || this.currentState == TransferrableObject.PositionState.Dropped || this.currentState == TransferrableObject.PositionState.None));
	}

	// Token: 0x06001BCE RID: 7118 RVA: 0x00095EE3 File Offset: 0x000940E3
	public bool InHand()
	{
		return this.currentState == TransferrableObject.PositionState.InLeftHand || this.currentState == TransferrableObject.PositionState.InRightHand;
	}

	// Token: 0x06001BCF RID: 7119 RVA: 0x00095EF9 File Offset: 0x000940F9
	public bool Dropped()
	{
		return this.currentState == TransferrableObject.PositionState.Dropped;
	}

	// Token: 0x06001BD0 RID: 7120 RVA: 0x00095F08 File Offset: 0x00094108
	public bool InLeftHand()
	{
		return this.currentState == TransferrableObject.PositionState.InLeftHand;
	}

	// Token: 0x06001BD1 RID: 7121 RVA: 0x00095F13 File Offset: 0x00094113
	public bool InRightHand()
	{
		return this.currentState == TransferrableObject.PositionState.InRightHand;
	}

	// Token: 0x06001BD2 RID: 7122 RVA: 0x00095F1E File Offset: 0x0009411E
	public bool OnChest()
	{
		return this.currentState == TransferrableObject.PositionState.OnChest;
	}

	// Token: 0x06001BD3 RID: 7123 RVA: 0x00095F2A File Offset: 0x0009412A
	public bool OnShoulder()
	{
		return this.currentState == TransferrableObject.PositionState.OnLeftShoulder || this.currentState == TransferrableObject.PositionState.OnRightShoulder;
	}

	// Token: 0x06001BD4 RID: 7124 RVA: 0x00095F42 File Offset: 0x00094142
	protected NetPlayer OwningPlayer()
	{
		if (this.myRig == null)
		{
			return this.myOnlineRig.netView.Owner;
		}
		return NetworkSystem.Instance.LocalPlayer;
	}

	// Token: 0x06001BD5 RID: 7125 RVA: 0x00095F70 File Offset: 0x00094170
	public bool ValidateState(TransferrableObject.PositionState state)
	{
		if (state <= TransferrableObject.PositionState.OnChest)
		{
			switch (state)
			{
			case TransferrableObject.PositionState.OnLeftArm:
				if ((this.dockPositions & BodyDockPositions.DropPositions.LeftArm) != BodyDockPositions.DropPositions.None)
				{
					return true;
				}
				return false;
			case TransferrableObject.PositionState.OnRightArm:
				if ((this.dockPositions & BodyDockPositions.DropPositions.RightArm) != BodyDockPositions.DropPositions.None)
				{
					return true;
				}
				return false;
			case TransferrableObject.PositionState.OnLeftArm | TransferrableObject.PositionState.OnRightArm:
				return false;
			case TransferrableObject.PositionState.InLeftHand:
				break;
			default:
				if (state != TransferrableObject.PositionState.InRightHand)
				{
					if (state != TransferrableObject.PositionState.OnChest)
					{
						return false;
					}
					if ((this.dockPositions & BodyDockPositions.DropPositions.Chest) != BodyDockPositions.DropPositions.None)
					{
						return true;
					}
					return false;
				}
				break;
			}
			return true;
		}
		if (state != TransferrableObject.PositionState.OnLeftShoulder)
		{
			if (state != TransferrableObject.PositionState.OnRightShoulder)
			{
				if (state == TransferrableObject.PositionState.Dropped)
				{
					return this.canDrop || this.shareable;
				}
			}
			else if ((this.dockPositions & BodyDockPositions.DropPositions.RightBack) != BodyDockPositions.DropPositions.None)
			{
				return true;
			}
		}
		else if ((this.dockPositions & BodyDockPositions.DropPositions.LeftBack) != BodyDockPositions.DropPositions.None)
		{
			return true;
		}
		return false;
	}

	// Token: 0x06001BD6 RID: 7126 RVA: 0x00096010 File Offset: 0x00094210
	public virtual void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		if (toPlayer != null && toPlayer.Equals(fromPlayer))
		{
			return;
		}
		if (object.Equals(fromPlayer, NetworkSystem.Instance.LocalPlayer) && this.IsHeld())
		{
			this.DropItem();
		}
		if (toPlayer == null)
		{
			this.SetTargetRig(null);
			return;
		}
		this.rigidbodyInstance.useGravity = (this.shouldUseGravity && object.Equals(toPlayer, NetworkSystem.Instance.LocalPlayer));
		if (!this.shareable && !this.isSceneObject)
		{
			return;
		}
		if (object.Equals(toPlayer, NetworkSystem.Instance.LocalPlayer))
		{
			if (GorillaTagger.Instance == null)
			{
				Debug.LogError("OnOwnershipTransferred has been initiated too quickly, The local player is not ready");
				return;
			}
			this.SetTargetRig(GorillaTagger.Instance.offlineVRRig);
			return;
		}
		else
		{
			VRRig exists = GorillaGameManager.StaticFindRigForPlayer(toPlayer);
			if (!exists)
			{
				Debug.LogError("failed to find target rig for ownershiptransfer");
				return;
			}
			this.SetTargetRig(exists);
			return;
		}
	}

	// Token: 0x06001BD7 RID: 7127 RVA: 0x000960E8 File Offset: 0x000942E8
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(fromPlayer, out rigContainer))
		{
			return false;
		}
		if (Vector3.SqrMagnitude(base.transform.position - rigContainer.transform.position) > 16f)
		{
			Debug.Log("Player whos trying to get is too far, Denying takeover");
			return false;
		}
		if (this.allowPlayerStealing || this.currentState == TransferrableObject.PositionState.Dropped || this.currentState == TransferrableObject.PositionState.None)
		{
			return true;
		}
		if (this.isSceneObject)
		{
			return false;
		}
		if (this.canDrop)
		{
			if (this.ownerRig == null || this.ownerRig.creator == null)
			{
				return true;
			}
			if (this.ownerRig.creator.Equals(fromPlayer))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001BD8 RID: 7128 RVA: 0x000961A0 File Offset: 0x000943A0
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(fromPlayer, out rigContainer))
		{
			return true;
		}
		if (Vector3.SqrMagnitude(base.transform.position - rigContainer.transform.position) > 16f)
		{
			Debug.Log("Player whos trying to get is too far, Denying takeover");
			return false;
		}
		if (this.currentState == TransferrableObject.PositionState.Dropped || this.currentState == TransferrableObject.PositionState.None)
		{
			return true;
		}
		if (this.canDrop)
		{
			if (this.ownerRig == null || this.ownerRig.creator == null)
			{
				return true;
			}
			if (this.ownerRig.creator.Equals(fromPlayer))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001BD9 RID: 7129 RVA: 0x00096244 File Offset: 0x00094444
	public void OnMyOwnerLeft()
	{
		if (this.currentState == TransferrableObject.PositionState.None || this.currentState == TransferrableObject.PositionState.Dropped)
		{
			return;
		}
		this.DropItem();
		if (this.anchor)
		{
			this.anchor.parent = this.InitialDockObject;
			this.anchor.localPosition = Vector3.zero;
			this.anchor.localRotation = Quaternion.identity;
		}
	}

	// Token: 0x06001BDA RID: 7130 RVA: 0x000962AB File Offset: 0x000944AB
	public void OnMyCreatorLeft()
	{
		this.OnItemDestroyedOrDisabled();
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06001BDB RID: 7131 RVA: 0x000962C0 File Offset: 0x000944C0
	public bool BuildValidationCheck()
	{
		int num = 0;
		if (this.storedZone.HasFlag(BodyDockPositions.DropPositions.LeftArm))
		{
			num++;
		}
		if (this.storedZone.HasFlag(BodyDockPositions.DropPositions.RightArm))
		{
			num++;
		}
		if (this.storedZone.HasFlag(BodyDockPositions.DropPositions.Chest))
		{
			num++;
		}
		if (this.storedZone.HasFlag(BodyDockPositions.DropPositions.LeftBack))
		{
			num++;
		}
		if (this.storedZone.HasFlag(BodyDockPositions.DropPositions.RightBack))
		{
			num++;
		}
		if (num > 1)
		{
			Debug.LogError("transferrableitem is starting with multiple storedzones: " + base.transform.parent.name, base.gameObject);
			return false;
		}
		return true;
	}

	// Token: 0x040023FA RID: 9210
	private VRRig _myRig;

	// Token: 0x040023FC RID: 9212
	private VRRig _myOnlineRig;

	// Token: 0x040023FE RID: 9214
	public bool latched;

	// Token: 0x040023FF RID: 9215
	private float indexTrigger;

	// Token: 0x04002400 RID: 9216
	public bool testActivate;

	// Token: 0x04002401 RID: 9217
	public bool testDeactivate;

	// Token: 0x04002402 RID: 9218
	public float myThreshold = 0.8f;

	// Token: 0x04002403 RID: 9219
	public float hysterisis = 0.05f;

	// Token: 0x04002404 RID: 9220
	public bool flipOnXForLeftHand;

	// Token: 0x04002405 RID: 9221
	public bool flipOnYForLeftHand;

	// Token: 0x04002406 RID: 9222
	public bool flipOnXForLeftArm;

	// Token: 0x04002407 RID: 9223
	public bool disableStealing;

	// Token: 0x04002408 RID: 9224
	public bool allowPlayerStealing;

	// Token: 0x04002409 RID: 9225
	private TransferrableObject.PositionState initState;

	// Token: 0x0400240A RID: 9226
	public TransferrableObject.ItemStates itemState;

	// Token: 0x0400240B RID: 9227
	[DevInspectorShow]
	public BodyDockPositions.DropPositions storedZone;

	// Token: 0x0400240C RID: 9228
	protected TransferrableObject.PositionState previousState;

	// Token: 0x0400240D RID: 9229
	[DevInspectorYellow]
	[DevInspectorShow]
	public TransferrableObject.PositionState currentState;

	// Token: 0x0400240E RID: 9230
	public BodyDockPositions.DropPositions dockPositions;

	// Token: 0x0400240F RID: 9231
	[DevInspectorCyan]
	[DevInspectorShow]
	public AdvancedItemState advancedGrabState;

	// Token: 0x04002410 RID: 9232
	[DevInspectorShow]
	[DevInspectorCyan]
	public VRRig targetRig;

	// Token: 0x04002411 RID: 9233
	public bool targetRigSet;

	// Token: 0x04002412 RID: 9234
	public TransferrableObject.GrabType useGrabType;

	// Token: 0x04002413 RID: 9235
	[DevInspectorShow]
	[DevInspectorCyan]
	public VRRig ownerRig;

	// Token: 0x04002414 RID: 9236
	[DebugReadout]
	[NonSerialized]
	public BodyDockPositions targetDockPositions;

	// Token: 0x04002415 RID: 9237
	private VRRigAnchorOverrides anchorOverrides;

	// Token: 0x04002416 RID: 9238
	public bool canAutoGrabLeft;

	// Token: 0x04002417 RID: 9239
	public bool canAutoGrabRight;

	// Token: 0x04002418 RID: 9240
	[DevInspectorShow]
	public int objectIndex;

	// Token: 0x04002419 RID: 9241
	[NonSerialized]
	public Transform anchor;

	// Token: 0x0400241A RID: 9242
	[Tooltip("In core prefab, assign to the Collider to grab this object")]
	public InteractionPoint gripInteractor;

	// Token: 0x0400241B RID: 9243
	[Tooltip("(Optional) Use this to override the transform used when the object is in the hand.\nExample: 'GHOST BALLOON' uses child 'grabPtAnchor' which is the end of the balloon's string.")]
	public Transform grabAnchor;

	// Token: 0x0400241C RID: 9244
	[Tooltip("(Optional) Use this (with the GorillaHandClosed_Left mesh) to intuitively define how\nthe player holds this object, by placing a representation of their hand gripping it.")]
	public Transform handPoseLeft;

	// Token: 0x0400241D RID: 9245
	[Tooltip("(Optional) Use this (with the GorillaHandClosed_Right mesh) to intuitively define how\nthe player holds this object, by placing a representation of their hand gripping it.")]
	public Transform handPoseRight;

	// Token: 0x0400241E RID: 9246
	public bool isGrabAnchorSet;

	// Token: 0x0400241F RID: 9247
	private static Vector3 handPoseRightReferencePoint = new Vector3(-0.0141f, 0.0065f, -0.278f);

	// Token: 0x04002420 RID: 9248
	private static Quaternion handPoseRightReferenceRotation = Quaternion.Euler(-2.058f, -17.2f, 65.05f);

	// Token: 0x04002421 RID: 9249
	private static Vector3 handPoseLeftReferencePoint = new Vector3(0.0136f, 0.0045f, -0.2809f);

	// Token: 0x04002422 RID: 9250
	private static Quaternion handPoseLeftReferenceRotation = Quaternion.Euler(-0.58f, 21.356f, -63.965f);

	// Token: 0x04002423 RID: 9251
	public TransferrableItemSlotTransformOverride transferrableItemSlotTransformOverride;

	// Token: 0x04002424 RID: 9252
	public int myIndex;

	// Token: 0x04002425 RID: 9253
	[Tooltip("(Optional)")]
	public GameObject[] gameObjectsActiveOnlyWhileHeld;

	// Token: 0x04002426 RID: 9254
	[Tooltip("(Optional)")]
	public GameObject[] gameObjectsActiveOnlyWhileDocked;

	// Token: 0x04002427 RID: 9255
	[Tooltip("(Optional)")]
	public Behaviour[] behavioursEnabledOnlyWhileHeld;

	// Token: 0x04002428 RID: 9256
	[Tooltip("(Optional)")]
	public Behaviour[] behavioursEnabledOnlyWhileDocked;

	// Token: 0x04002429 RID: 9257
	[SerializeField]
	protected internal WorldShareableItem worldShareableInstance;

	// Token: 0x0400242A RID: 9258
	private float interpTime = 0.2f;

	// Token: 0x0400242B RID: 9259
	private float interpDt;

	// Token: 0x0400242C RID: 9260
	private Vector3 interpStartPos;

	// Token: 0x0400242D RID: 9261
	private Quaternion interpStartRot;

	// Token: 0x0400242E RID: 9262
	protected int enabledOnFrame = -1;

	// Token: 0x0400242F RID: 9263
	protected Vector3 initOffset;

	// Token: 0x04002430 RID: 9264
	protected Quaternion initRotation;

	// Token: 0x04002431 RID: 9265
	private Matrix4x4 initMatrix = Matrix4x4.identity;

	// Token: 0x04002432 RID: 9266
	private Matrix4x4 leftHandMatrix = Matrix4x4.identity;

	// Token: 0x04002433 RID: 9267
	private Matrix4x4 rightHandMatrix = Matrix4x4.identity;

	// Token: 0x04002434 RID: 9268
	private bool positionInitialized;

	// Token: 0x04002435 RID: 9269
	public bool isSceneObject;

	// Token: 0x04002436 RID: 9270
	public Rigidbody rigidbodyInstance;

	// Token: 0x04002439 RID: 9273
	public bool canDrop;

	// Token: 0x0400243A RID: 9274
	public bool allowReparenting;

	// Token: 0x0400243B RID: 9275
	public bool shareable;

	// Token: 0x0400243C RID: 9276
	public bool detatchOnGrab;

	// Token: 0x0400243D RID: 9277
	public bool allowWorldSharableInstance;

	// Token: 0x0400243E RID: 9278
	[ItemCanBeNull]
	public Transform originPoint;

	// Token: 0x0400243F RID: 9279
	[ItemCanBeNull]
	public float maxDistanceFromOriginBeforeRespawn;

	// Token: 0x04002440 RID: 9280
	public AudioClip resetPositionAudioClip;

	// Token: 0x04002441 RID: 9281
	public float maxDistanceFromTargetPlayerBeforeRespawn;

	// Token: 0x04002442 RID: 9282
	private bool wasHover;

	// Token: 0x04002443 RID: 9283
	private bool isHover;

	// Token: 0x04002444 RID: 9284
	private bool disableItem;

	// Token: 0x04002445 RID: 9285
	protected bool loaded;

	// Token: 0x04002446 RID: 9286
	public bool ClearLocalPositionOnReset;

	// Token: 0x04002447 RID: 9287
	[SerializeField]
	private UnityEvent OnUndocked;

	// Token: 0x04002448 RID: 9288
	[SerializeField]
	private UnityEvent OnDocked;

	// Token: 0x04002449 RID: 9289
	public string interactEventName;

	// Token: 0x0400244A RID: 9290
	public const int kPositionStateCount = 8;

	// Token: 0x0400244B RID: 9291
	[DevInspectorShow]
	public TransferrableObject.InterpolateState interpState;

	// Token: 0x0400244C RID: 9292
	public bool startInterpolation;

	// Token: 0x0400244D RID: 9293
	public Transform InitialDockObject;

	// Token: 0x0400244E RID: 9294
	private AudioSource audioSrc;

	// Token: 0x0400244F RID: 9295
	private bool _isListeningFor_OnPostInstantiateAllPrefabs2;

	// Token: 0x04002452 RID: 9298
	protected Transform _defaultAnchor;

	// Token: 0x04002453 RID: 9299
	protected bool _isDefaultAnchorSet;

	// Token: 0x04002454 RID: 9300
	private Matrix4x4? transferrableItemSlotTransformOverrideCachedMatrix;

	// Token: 0x04002455 RID: 9301
	private bool transferrableItemSlotTransformOverrideApplicable;

	// Token: 0x0200045E RID: 1118
	public enum ItemStates
	{
		// Token: 0x04002457 RID: 9303
		State0 = 1,
		// Token: 0x04002458 RID: 9304
		State1,
		// Token: 0x04002459 RID: 9305
		State2 = 4,
		// Token: 0x0400245A RID: 9306
		State3 = 8,
		// Token: 0x0400245B RID: 9307
		State4 = 16,
		// Token: 0x0400245C RID: 9308
		State5 = 32,
		// Token: 0x0400245D RID: 9309
		Part0Held = 64,
		// Token: 0x0400245E RID: 9310
		Part1Held = 128
	}

	// Token: 0x0200045F RID: 1119
	public enum GrabType
	{
		// Token: 0x04002460 RID: 9312
		Default,
		// Token: 0x04002461 RID: 9313
		Free
	}

	// Token: 0x02000460 RID: 1120
	[Flags]
	public enum PositionState
	{
		// Token: 0x04002463 RID: 9315
		OnLeftArm = 1,
		// Token: 0x04002464 RID: 9316
		OnRightArm = 2,
		// Token: 0x04002465 RID: 9317
		InLeftHand = 4,
		// Token: 0x04002466 RID: 9318
		InRightHand = 8,
		// Token: 0x04002467 RID: 9319
		OnChest = 16,
		// Token: 0x04002468 RID: 9320
		OnLeftShoulder = 32,
		// Token: 0x04002469 RID: 9321
		OnRightShoulder = 64,
		// Token: 0x0400246A RID: 9322
		Dropped = 128,
		// Token: 0x0400246B RID: 9323
		None = 0
	}

	// Token: 0x02000461 RID: 1121
	public enum InterpolateState
	{
		// Token: 0x0400246D RID: 9325
		None,
		// Token: 0x0400246E RID: 9326
		Interpolating
	}
}
