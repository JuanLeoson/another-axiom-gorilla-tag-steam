using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200042D RID: 1069
public class BodyDockPositions : MonoBehaviour
{
	// Token: 0x170002DC RID: 732
	// (get) Token: 0x060019D9 RID: 6617 RVA: 0x0008AB37 File Offset: 0x00088D37
	// (set) Token: 0x060019DA RID: 6618 RVA: 0x0008AB3F File Offset: 0x00088D3F
	public TransferrableObject[] allObjects
	{
		get
		{
			return this._allObjects;
		}
		set
		{
			this._allObjects = value;
		}
	}

	// Token: 0x060019DB RID: 6619 RVA: 0x0008AB48 File Offset: 0x00088D48
	public void Awake()
	{
		RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
		RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeftRoom);
	}

	// Token: 0x060019DC RID: 6620 RVA: 0x0008AB80 File Offset: 0x00088D80
	public void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		if (object.Equals(this.myRig.creator, otherPlayer))
		{
			this.DeallocateSharableInstances();
		}
	}

	// Token: 0x060019DD RID: 6621 RVA: 0x0008AB9B File Offset: 0x00088D9B
	public void OnLeftRoom()
	{
		this.DeallocateSharableInstances();
	}

	// Token: 0x060019DE RID: 6622 RVA: 0x0008ABA4 File Offset: 0x00088DA4
	public WorldShareableItem AllocateSharableInstance(BodyDockPositions.DropPositions position, NetPlayer owner)
	{
		switch (position)
		{
		case BodyDockPositions.DropPositions.None:
		case BodyDockPositions.DropPositions.LeftArm:
		case BodyDockPositions.DropPositions.RightArm:
		case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm:
		case BodyDockPositions.DropPositions.Chest:
		case BodyDockPositions.DropPositions.MaxDropPostions:
		case BodyDockPositions.DropPositions.RightArm | BodyDockPositions.DropPositions.Chest:
		case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm | BodyDockPositions.DropPositions.Chest:
			break;
		case BodyDockPositions.DropPositions.LeftBack:
			if (this.leftBackSharableItem == null)
			{
				this.leftBackSharableItem = ObjectPools.instance.Instantiate(this.SharableItemInstance, true).GetComponent<WorldShareableItem>();
				this.leftBackSharableItem.GetComponent<RequestableOwnershipGuard>().SetOwnership(owner, false, true);
				this.leftBackSharableItem.GetComponent<WorldShareableItem>().SetupSharableViewIDs(owner, 3);
			}
			return this.leftBackSharableItem;
		default:
			if (position == BodyDockPositions.DropPositions.RightBack)
			{
				if (this.rightBackShareableItem == null)
				{
					this.rightBackShareableItem = ObjectPools.instance.Instantiate(this.SharableItemInstance, true).GetComponent<WorldShareableItem>();
					this.rightBackShareableItem.GetComponent<RequestableOwnershipGuard>().SetOwnership(owner, false, true);
					this.rightBackShareableItem.GetComponent<WorldShareableItem>().SetupSharableViewIDs(owner, 4);
				}
				return this.rightBackShareableItem;
			}
			if (position != BodyDockPositions.DropPositions.All)
			{
			}
			break;
		}
		throw new ArgumentOutOfRangeException("position", position, null);
	}

	// Token: 0x060019DF RID: 6623 RVA: 0x0008ACA0 File Offset: 0x00088EA0
	public void DeallocateSharableInstance(WorldShareableItem worldShareable)
	{
		if (worldShareable == null)
		{
			return;
		}
		if (worldShareable == this.leftBackSharableItem)
		{
			if (this.leftBackSharableItem == null)
			{
				return;
			}
			this.leftBackSharableItem.ResetViews();
			ObjectPools.instance.Destroy(this.leftBackSharableItem.gameObject);
			this.leftBackSharableItem = null;
		}
		if (worldShareable == this.rightBackShareableItem)
		{
			if (this.rightBackShareableItem == null)
			{
				return;
			}
			this.rightBackShareableItem.ResetViews();
			ObjectPools.instance.Destroy(this.rightBackShareableItem.gameObject);
			this.rightBackShareableItem = null;
		}
	}

	// Token: 0x060019E0 RID: 6624 RVA: 0x0008AD34 File Offset: 0x00088F34
	public void DeallocateSharableInstances()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this.rightBackShareableItem != null)
		{
			this.rightBackShareableItem.ResetViews();
			ObjectPools.instance.Destroy(this.rightBackShareableItem.gameObject);
		}
		if (this.leftBackSharableItem != null)
		{
			this.leftBackSharableItem.ResetViews();
			ObjectPools.instance.Destroy(this.leftBackSharableItem.gameObject);
		}
		this.leftBackSharableItem = null;
		this.rightBackShareableItem = null;
	}

	// Token: 0x060019E1 RID: 6625 RVA: 0x0008ADA7 File Offset: 0x00088FA7
	public static bool IsPositionLeft(BodyDockPositions.DropPositions pos)
	{
		return pos == BodyDockPositions.DropPositions.LeftArm || pos == BodyDockPositions.DropPositions.LeftBack;
	}

	// Token: 0x060019E2 RID: 6626 RVA: 0x0008ADB4 File Offset: 0x00088FB4
	public int DropZoneStorageUsed(BodyDockPositions.DropPositions dropPosition)
	{
		if (this.myRig == null)
		{
			Debug.Log("BodyDockPositions lost reference to VR Rig, resetting it now", this);
			this.myRig = base.GetComponent<VRRig>();
		}
		if (this.myRig == null)
		{
			Debug.Log("Unable to reset reference");
			return -1;
		}
		for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			if (this.myRig.ActiveTransferrableObjectIndex(i) >= 0 && this.allObjects[this.myRig.ActiveTransferrableObjectIndex(i)].gameObject.activeInHierarchy && this.allObjects[this.myRig.ActiveTransferrableObjectIndex(i)].storedZone == dropPosition)
			{
				return this.myRig.ActiveTransferrableObjectIndex(i);
			}
		}
		return -1;
	}

	// Token: 0x060019E3 RID: 6627 RVA: 0x0008AE70 File Offset: 0x00089070
	public TransferrableObject ItemPositionInUse(BodyDockPositions.DropPositions dropPosition)
	{
		TransferrableObject.PositionState positionState = this.MapDropPositionToState(dropPosition);
		if (this.myRig == null)
		{
			Debug.Log("BodyDockPositions lost reference to VR Rig, resetting it now", this);
			this.myRig = base.GetComponent<VRRig>();
		}
		if (this.myRig == null)
		{
			Debug.Log("Unable to reset reference");
			return null;
		}
		for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			if (this.myRig.ActiveTransferrableObjectIndex(i) != -1 && this.allObjects[this.myRig.ActiveTransferrableObjectIndex(i)].gameObject.activeInHierarchy && this.allObjects[this.myRig.ActiveTransferrableObjectIndex(i)].currentState == positionState)
			{
				return this.allObjects[this.myRig.ActiveTransferrableObjectIndex(i)];
			}
		}
		return null;
	}

	// Token: 0x060019E4 RID: 6628 RVA: 0x0008AF38 File Offset: 0x00089138
	private int EnableTransferrableItem(int allItemsIndex, BodyDockPositions.DropPositions startingPosition, TransferrableObject.PositionState startingState)
	{
		if (allItemsIndex < 0 || allItemsIndex >= this.allObjects.Length)
		{
			return -1;
		}
		if (this.myRig != null && this.myRig.isOfflineVRRig)
		{
			for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
			{
				if (this.myRig.ActiveTransferrableObjectIndex(i) == allItemsIndex)
				{
					this.DisableTransferrableItem(allItemsIndex);
				}
			}
			for (int j = 0; j < this.myRig.ActiveTransferrableObjectIndexLength(); j++)
			{
				if (this.myRig.ActiveTransferrableObjectIndex(j) == -1)
				{
					string itemNameFromDisplayName = CosmeticsController.instance.GetItemNameFromDisplayName(this.allObjects[allItemsIndex].gameObject.name);
					if (this.myRig.IsItemAllowed(itemNameFromDisplayName))
					{
						this.myRig.SetActiveTransferrableObjectIndex(j, allItemsIndex);
						this.myRig.SetTransferrablePosStates(j, startingState);
						this.myRig.SetTransferrableItemStates(j, (TransferrableObject.ItemStates)0);
						this.myRig.SetTransferrableDockPosition(j, startingPosition);
						this.EnableTransferrableGameObject(allItemsIndex, startingPosition, startingState);
						return j;
					}
				}
			}
		}
		return -1;
	}

	// Token: 0x060019E5 RID: 6629 RVA: 0x0008B038 File Offset: 0x00089238
	public BodyDockPositions.DropPositions ItemActive(int allItemsIndex)
	{
		if (!this.allObjects[allItemsIndex].gameObject.activeSelf)
		{
			return BodyDockPositions.DropPositions.None;
		}
		return this.allObjects[allItemsIndex].storedZone;
	}

	// Token: 0x060019E6 RID: 6630 RVA: 0x0008B060 File Offset: 0x00089260
	public static BodyDockPositions.DropPositions OfflineItemActive(int allItemsIndex)
	{
		if (GorillaTagger.Instance == null || GorillaTagger.Instance.offlineVRRig == null)
		{
			return BodyDockPositions.DropPositions.None;
		}
		BodyDockPositions component = GorillaTagger.Instance.offlineVRRig.GetComponent<BodyDockPositions>();
		if (component == null)
		{
			return BodyDockPositions.DropPositions.None;
		}
		if (!component.allObjects[allItemsIndex].gameObject.activeSelf)
		{
			return BodyDockPositions.DropPositions.None;
		}
		return component.allObjects[allItemsIndex].storedZone;
	}

	// Token: 0x060019E7 RID: 6631 RVA: 0x0008B0D0 File Offset: 0x000892D0
	public void DisableTransferrableItem(int index)
	{
		TransferrableObject transferrableObject = this.allObjects[index];
		if (transferrableObject.gameObject.activeSelf)
		{
			transferrableObject.gameObject.Disable();
			transferrableObject.storedZone = BodyDockPositions.DropPositions.None;
		}
		if (this.myRig.isOfflineVRRig)
		{
			for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
			{
				if (this.myRig.ActiveTransferrableObjectIndex(i) == index)
				{
					this.myRig.SetActiveTransferrableObjectIndex(i, -1);
				}
			}
		}
	}

	// Token: 0x060019E8 RID: 6632 RVA: 0x0008B144 File Offset: 0x00089344
	public void DisableAllTransferableItems()
	{
		if (!CosmeticsV2Spawner_Dirty.allPartsInstantiated)
		{
			return;
		}
		for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			int num = this.myRig.ActiveTransferrableObjectIndex(i);
			if (num >= 0 && num < this.allObjects.Length)
			{
				TransferrableObject transferrableObject = this.allObjects[num];
				transferrableObject.gameObject.Disable();
				transferrableObject.storedZone = BodyDockPositions.DropPositions.None;
				this.myRig.SetActiveTransferrableObjectIndex(i, -1);
				this.myRig.SetTransferrableItemStates(i, (TransferrableObject.ItemStates)0);
				this.myRig.SetTransferrablePosStates(i, TransferrableObject.PositionState.None);
			}
		}
		this.DeallocateSharableInstances();
	}

	// Token: 0x060019E9 RID: 6633 RVA: 0x0008B1D1 File Offset: 0x000893D1
	private bool AllItemsIndexValid(int allItemsIndex)
	{
		return allItemsIndex != -1 && allItemsIndex < this.allObjects.Length;
	}

	// Token: 0x060019EA RID: 6634 RVA: 0x0008B1E4 File Offset: 0x000893E4
	public bool PositionAvailable(int allItemIndex, BodyDockPositions.DropPositions startPos)
	{
		return (this.allObjects[allItemIndex].dockPositions & startPos) > BodyDockPositions.DropPositions.None;
	}

	// Token: 0x060019EB RID: 6635 RVA: 0x0008B1F8 File Offset: 0x000893F8
	public BodyDockPositions.DropPositions FirstAvailablePosition(int allItemIndex)
	{
		for (int i = 0; i < 5; i++)
		{
			BodyDockPositions.DropPositions dropPositions = (BodyDockPositions.DropPositions)(1 << i);
			if ((this.allObjects[allItemIndex].dockPositions & dropPositions) != BodyDockPositions.DropPositions.None)
			{
				return dropPositions;
			}
		}
		return BodyDockPositions.DropPositions.None;
	}

	// Token: 0x060019EC RID: 6636 RVA: 0x0008B22C File Offset: 0x0008942C
	public int TransferrableItemDisable(int allItemsIndex)
	{
		if (BodyDockPositions.OfflineItemActive(allItemsIndex) != BodyDockPositions.DropPositions.None)
		{
			this.DisableTransferrableItem(allItemsIndex);
		}
		return 0;
	}

	// Token: 0x060019ED RID: 6637 RVA: 0x0008B240 File Offset: 0x00089440
	public void TransferrableItemDisableAtPosition(BodyDockPositions.DropPositions dropPositions)
	{
		int num = this.DropZoneStorageUsed(dropPositions);
		if (num >= 0)
		{
			this.TransferrableItemDisable(num);
		}
	}

	// Token: 0x060019EE RID: 6638 RVA: 0x0008B264 File Offset: 0x00089464
	public void TransferrableItemEnableAtPosition(string itemName, BodyDockPositions.DropPositions dropPosition)
	{
		if (this.DropZoneStorageUsed(dropPosition) >= 0)
		{
			return;
		}
		List<int> list = this.TransferrableObjectIndexFromName(itemName);
		if (list.Count == 0)
		{
			return;
		}
		TransferrableObject.PositionState startingState = this.MapDropPositionToState(dropPosition);
		if (list.Count == 1)
		{
			this.EnableTransferrableItem(list[0], dropPosition, startingState);
			return;
		}
		int allItemsIndex = BodyDockPositions.IsPositionLeft(dropPosition) ? list[0] : list[1];
		this.EnableTransferrableItem(allItemsIndex, dropPosition, startingState);
	}

	// Token: 0x060019EF RID: 6639 RVA: 0x0008B2D4 File Offset: 0x000894D4
	public bool TransferrableItemActive(string transferrableItemName)
	{
		List<int> list = this.TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return false;
		}
		foreach (int allItemsIndex in list)
		{
			if (this.TransferrableItemActive(allItemsIndex))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060019F0 RID: 6640 RVA: 0x0008B340 File Offset: 0x00089540
	public bool TransferrableItemActiveAtPos(string transferrableItemName, BodyDockPositions.DropPositions dropPosition)
	{
		List<int> list = this.TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return false;
		}
		foreach (int allItemsIndex in list)
		{
			BodyDockPositions.DropPositions dropPositions = this.TransferrableItemPosition(allItemsIndex);
			if (dropPositions != BodyDockPositions.DropPositions.None && dropPositions == dropPosition)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060019F1 RID: 6641 RVA: 0x0008B3B4 File Offset: 0x000895B4
	public bool TransferrableItemActive(int allItemsIndex)
	{
		return this.ItemActive(allItemsIndex) > BodyDockPositions.DropPositions.None;
	}

	// Token: 0x060019F2 RID: 6642 RVA: 0x0008B3C0 File Offset: 0x000895C0
	public TransferrableObject TransferrableItem(int allItemsIndex)
	{
		return this.allObjects[allItemsIndex];
	}

	// Token: 0x060019F3 RID: 6643 RVA: 0x0008B3CA File Offset: 0x000895CA
	public BodyDockPositions.DropPositions TransferrableItemPosition(int allItemsIndex)
	{
		return this.ItemActive(allItemsIndex);
	}

	// Token: 0x060019F4 RID: 6644 RVA: 0x0008B3D4 File Offset: 0x000895D4
	public bool DisableTransferrableItem(string transferrableItemName)
	{
		List<int> list = this.TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return false;
		}
		foreach (int index in list)
		{
			this.DisableTransferrableItem(index);
		}
		return true;
	}

	// Token: 0x060019F5 RID: 6645 RVA: 0x0008B438 File Offset: 0x00089638
	public BodyDockPositions.DropPositions OppositePosition(BodyDockPositions.DropPositions pos)
	{
		if (pos == BodyDockPositions.DropPositions.LeftArm)
		{
			return BodyDockPositions.DropPositions.RightArm;
		}
		if (pos == BodyDockPositions.DropPositions.RightArm)
		{
			return BodyDockPositions.DropPositions.LeftArm;
		}
		if (pos == BodyDockPositions.DropPositions.LeftBack)
		{
			return BodyDockPositions.DropPositions.RightBack;
		}
		if (pos == BodyDockPositions.DropPositions.RightBack)
		{
			return BodyDockPositions.DropPositions.LeftBack;
		}
		return pos;
	}

	// Token: 0x060019F6 RID: 6646 RVA: 0x0008B458 File Offset: 0x00089658
	public BodyDockPositions.DockingResult ToggleWithHandedness(string transferrableItemName, bool isLeftHand, bool bothHands)
	{
		List<int> list = this.TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return new BodyDockPositions.DockingResult();
		}
		if (!this.AllItemsIndexValid(list[0]))
		{
			return new BodyDockPositions.DockingResult();
		}
		BodyDockPositions.DropPositions startingPos;
		if (isLeftHand)
		{
			startingPos = (((this.allObjects[list[0]].dockPositions & BodyDockPositions.DropPositions.RightArm) != BodyDockPositions.DropPositions.None) ? BodyDockPositions.DropPositions.RightArm : BodyDockPositions.DropPositions.LeftBack);
		}
		else
		{
			startingPos = (((this.allObjects[list[0]].dockPositions & BodyDockPositions.DropPositions.LeftArm) != BodyDockPositions.DropPositions.None) ? BodyDockPositions.DropPositions.LeftArm : BodyDockPositions.DropPositions.RightBack);
		}
		return this.ToggleTransferrableItem(transferrableItemName, startingPos, bothHands);
	}

	// Token: 0x060019F7 RID: 6647 RVA: 0x0008B4D8 File Offset: 0x000896D8
	public BodyDockPositions.DockingResult ToggleTransferrableItem(string transferrableItemName, BodyDockPositions.DropPositions startingPos, bool bothHands)
	{
		BodyDockPositions.DockingResult dockingResult = new BodyDockPositions.DockingResult();
		List<int> list = this.TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return dockingResult;
		}
		if (bothHands && list.Count == 2)
		{
			for (int i = 0; i < list.Count; i++)
			{
				int allItemsIndex = list[i];
				BodyDockPositions.DropPositions dropPositions = BodyDockPositions.OfflineItemActive(allItemsIndex);
				if (dropPositions != BodyDockPositions.DropPositions.None)
				{
					this.TransferrableItemDisable(allItemsIndex);
					dockingResult.positionsDisabled.Add(dropPositions);
				}
			}
			if (dockingResult.positionsDisabled.Count >= 1)
			{
				return dockingResult;
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			int num = list[j];
			BodyDockPositions.DropPositions dropPositions2 = startingPos;
			if (bothHands && j != 0)
			{
				dropPositions2 = this.OppositePosition(dropPositions2);
			}
			if (!this.PositionAvailable(num, dropPositions2))
			{
				dropPositions2 = this.FirstAvailablePosition(num);
				if (dropPositions2 == BodyDockPositions.DropPositions.None)
				{
					return dockingResult;
				}
			}
			if (BodyDockPositions.OfflineItemActive(num) == dropPositions2)
			{
				this.TransferrableItemDisable(num);
				dockingResult.positionsDisabled.Add(dropPositions2);
			}
			else
			{
				this.TransferrableItemDisableAtPosition(dropPositions2);
				dockingResult.dockedPosition.Add(dropPositions2);
				TransferrableObject.PositionState positionState = this.MapDropPositionToState(dropPositions2);
				if (this.TransferrableItemActive(num))
				{
					BodyDockPositions.DropPositions item = this.TransferrableItemPosition(num);
					dockingResult.positionsDisabled.Add(item);
					this.MoveTransferableItem(num, dropPositions2, positionState);
				}
				else
				{
					this.EnableTransferrableItem(num, dropPositions2, positionState);
				}
			}
		}
		return dockingResult;
	}

	// Token: 0x060019F8 RID: 6648 RVA: 0x0008B61F File Offset: 0x0008981F
	private void MoveTransferableItem(int allItemsIndex, BodyDockPositions.DropPositions newPosition, TransferrableObject.PositionState newPositionState)
	{
		this.allObjects[allItemsIndex].storedZone = newPosition;
		this.allObjects[allItemsIndex].currentState = newPositionState;
		this.allObjects[allItemsIndex].ResetToDefaultState();
	}

	// Token: 0x060019F9 RID: 6649 RVA: 0x0008B64C File Offset: 0x0008984C
	public void EnableTransferrableGameObject(int allItemsIndex, BodyDockPositions.DropPositions dropZone, TransferrableObject.PositionState startingPosition)
	{
		GameObject gameObject = this.allObjects[allItemsIndex].gameObject;
		TransferrableObject component = gameObject.GetComponent<TransferrableObject>();
		if ((component.dockPositions & dropZone) == BodyDockPositions.DropPositions.None || !component.ValidateState(startingPosition))
		{
			gameObject.Disable();
			return;
		}
		this.MoveTransferableItem(allItemsIndex, dropZone, startingPosition);
		gameObject.SetActive(true);
		ProjectileWeapon component2;
		if ((component2 = gameObject.GetComponent<ProjectileWeapon>()) != null)
		{
			component2.enabled = true;
		}
	}

	// Token: 0x060019FA RID: 6650 RVA: 0x0008B6B0 File Offset: 0x000898B0
	public void RefreshTransferrableItems()
	{
		if (!this.myRig)
		{
			this.myRig = base.GetComponentInParent<VRRig>(true);
			if (!this.myRig)
			{
				Debug.LogError("BodyDockPositions.RefreshTransferrableItems: (should never happen) myRig is null and could not be found on same GameObject or parents. Path: " + base.transform.GetPathQ(), this);
			}
		}
		this.objectsToEnable.Clear();
		this.objectsToDisable.Clear();
		for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			bool flag = true;
			int num = this.myRig.ActiveTransferrableObjectIndex(i);
			if (num != -1)
			{
				if (num < 0 || num >= this.allObjects.Length)
				{
					Debug.LogError(string.Format("Transferrable object index {0} out of range, expected [0..{1})", num, this.allObjects.Length));
				}
				else if (this.myRig.IsItemAllowed(CosmeticsController.instance.GetItemNameFromDisplayName(this.allObjects[num].gameObject.name)))
				{
					for (int j = 0; j < this.allObjects.Length; j++)
					{
						if (j == this.myRig.ActiveTransferrableObjectIndex(i) && this.allObjects[j].gameObject.activeSelf)
						{
							this.allObjects[j].objectIndex = i;
							flag = false;
						}
					}
					if (flag)
					{
						this.objectsToEnable.Add(i);
					}
				}
			}
		}
		for (int k = 0; k < this.allObjects.Length; k++)
		{
			if (this.allObjects[k] != null && this.allObjects[k].gameObject.activeSelf)
			{
				bool flag2 = true;
				for (int l = 0; l < this.myRig.ActiveTransferrableObjectIndexLength(); l++)
				{
					if (this.myRig.ActiveTransferrableObjectIndex(l) == k && this.myRig.IsItemAllowed(CosmeticsController.instance.GetItemNameFromDisplayName(this.allObjects[this.myRig.ActiveTransferrableObjectIndex(l)].gameObject.name)))
					{
						flag2 = false;
					}
				}
				if (flag2)
				{
					this.objectsToDisable.Add(k);
				}
			}
		}
		foreach (int index in this.objectsToDisable)
		{
			this.DisableTransferrableItem(index);
		}
		foreach (int idx in this.objectsToEnable)
		{
			this.EnableTransferrableGameObject(this.myRig.ActiveTransferrableObjectIndex(idx), this.myRig.TransferrableDockPosition(idx), this.myRig.TransferrablePosStates(idx));
		}
		this.UpdateHandState();
	}

	// Token: 0x060019FB RID: 6651 RVA: 0x0008B97C File Offset: 0x00089B7C
	public int ReturnTransferrableItemIndex(int allItemsIndex)
	{
		for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			if (this.myRig.ActiveTransferrableObjectIndex(i) == allItemsIndex)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060019FC RID: 6652 RVA: 0x0008B9B4 File Offset: 0x00089BB4
	public List<int> TransferrableObjectIndexFromName(string transObjectName)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < this.allObjects.Length; i++)
		{
			if (!(this.allObjects[i] == null) && this.allObjects[i].gameObject.name == transObjectName)
			{
				list.Add(i);
			}
		}
		return list;
	}

	// Token: 0x060019FD RID: 6653 RVA: 0x0008BA0C File Offset: 0x00089C0C
	private TransferrableObject.PositionState MapDropPositionToState(BodyDockPositions.DropPositions pos)
	{
		if (pos == BodyDockPositions.DropPositions.RightArm)
		{
			return TransferrableObject.PositionState.OnRightArm;
		}
		if (pos == BodyDockPositions.DropPositions.LeftArm)
		{
			return TransferrableObject.PositionState.OnLeftArm;
		}
		if (pos == BodyDockPositions.DropPositions.LeftBack)
		{
			return TransferrableObject.PositionState.OnLeftShoulder;
		}
		if (pos == BodyDockPositions.DropPositions.RightBack)
		{
			return TransferrableObject.PositionState.OnRightShoulder;
		}
		return TransferrableObject.PositionState.OnChest;
	}

	// Token: 0x170002DD RID: 733
	// (get) Token: 0x060019FE RID: 6654 RVA: 0x0008BA2B File Offset: 0x00089C2B
	internal int PreviousLeftHandThrowableIndex
	{
		get
		{
			return this.throwableDisabledIndex[0];
		}
	}

	// Token: 0x170002DE RID: 734
	// (get) Token: 0x060019FF RID: 6655 RVA: 0x0008BA35 File Offset: 0x00089C35
	internal int PreviousRightHandThrowableIndex
	{
		get
		{
			return this.throwableDisabledIndex[1];
		}
	}

	// Token: 0x170002DF RID: 735
	// (get) Token: 0x06001A00 RID: 6656 RVA: 0x0008BA3F File Offset: 0x00089C3F
	internal float PreviousLeftHandThrowableDisabledTime
	{
		get
		{
			return this.throwableDisabledTime[0];
		}
	}

	// Token: 0x170002E0 RID: 736
	// (get) Token: 0x06001A01 RID: 6657 RVA: 0x0008BA49 File Offset: 0x00089C49
	internal float PreviousRightHandThrowableDisabledTime
	{
		get
		{
			return this.throwableDisabledTime[1];
		}
	}

	// Token: 0x06001A02 RID: 6658 RVA: 0x0008BA54 File Offset: 0x00089C54
	private void UpdateHandState()
	{
		for (int i = 0; i < 2; i++)
		{
			GameObject[] array = (i == 0) ? this.leftHandThrowables : this.rightHandThrowables;
			int num = (i == 0) ? this.myRig.LeftThrowableProjectileIndex : this.myRig.RightThrowableProjectileIndex;
			for (int j = 0; j < array.Length; j++)
			{
				bool activeSelf = array[j].activeSelf;
				bool flag = j == num;
				array[j].SetActive(flag);
				if (activeSelf && !flag)
				{
					this.throwableDisabledIndex[i] = j;
					this.throwableDisabledTime[i] = Time.time + 0.02f;
				}
			}
		}
	}

	// Token: 0x06001A03 RID: 6659 RVA: 0x0008BAE3 File Offset: 0x00089CE3
	internal GameObject GetLeftHandThrowable()
	{
		return this.GetLeftHandThrowable(this.myRig.LeftThrowableProjectileIndex);
	}

	// Token: 0x06001A04 RID: 6660 RVA: 0x0008BAF6 File Offset: 0x00089CF6
	internal GameObject GetLeftHandThrowable(int throwableIndex)
	{
		if (throwableIndex < 0 || throwableIndex >= this.leftHandThrowables.Length)
		{
			throwableIndex = this.PreviousLeftHandThrowableIndex;
			if (throwableIndex < 0 || throwableIndex >= this.leftHandThrowables.Length || this.PreviousLeftHandThrowableDisabledTime < Time.time)
			{
				return null;
			}
		}
		return this.leftHandThrowables[throwableIndex];
	}

	// Token: 0x06001A05 RID: 6661 RVA: 0x0008BB35 File Offset: 0x00089D35
	internal GameObject GetRightHandThrowable()
	{
		return this.GetRightHandThrowable(this.myRig.RightThrowableProjectileIndex);
	}

	// Token: 0x06001A06 RID: 6662 RVA: 0x0008BB48 File Offset: 0x00089D48
	internal GameObject GetRightHandThrowable(int throwableIndex)
	{
		if (throwableIndex < 0 || throwableIndex >= this.rightHandThrowables.Length)
		{
			throwableIndex = this.PreviousRightHandThrowableIndex;
			if (throwableIndex < 0 || throwableIndex >= this.rightHandThrowables.Length || this.PreviousRightHandThrowableDisabledTime < Time.time)
			{
				return null;
			}
		}
		return this.rightHandThrowables[throwableIndex];
	}

	// Token: 0x04002230 RID: 8752
	public VRRig myRig;

	// Token: 0x04002231 RID: 8753
	public GameObject[] leftHandThrowables;

	// Token: 0x04002232 RID: 8754
	public GameObject[] rightHandThrowables;

	// Token: 0x04002233 RID: 8755
	[FormerlySerializedAs("allObjects")]
	public TransferrableObject[] _allObjects;

	// Token: 0x04002234 RID: 8756
	private List<int> objectsToEnable = new List<int>();

	// Token: 0x04002235 RID: 8757
	private List<int> objectsToDisable = new List<int>();

	// Token: 0x04002236 RID: 8758
	public Transform leftHandTransform;

	// Token: 0x04002237 RID: 8759
	public Transform rightHandTransform;

	// Token: 0x04002238 RID: 8760
	public Transform chestTransform;

	// Token: 0x04002239 RID: 8761
	public Transform leftArmTransform;

	// Token: 0x0400223A RID: 8762
	public Transform rightArmTransform;

	// Token: 0x0400223B RID: 8763
	public Transform leftBackTransform;

	// Token: 0x0400223C RID: 8764
	public Transform rightBackTransform;

	// Token: 0x0400223D RID: 8765
	public WorldShareableItem leftBackSharableItem;

	// Token: 0x0400223E RID: 8766
	public WorldShareableItem rightBackShareableItem;

	// Token: 0x0400223F RID: 8767
	public GameObject SharableItemInstance;

	// Token: 0x04002240 RID: 8768
	private int[] throwableDisabledIndex = new int[]
	{
		-1,
		-1
	};

	// Token: 0x04002241 RID: 8769
	private float[] throwableDisabledTime = new float[2];

	// Token: 0x0200042E RID: 1070
	[Flags]
	public enum DropPositions
	{
		// Token: 0x04002243 RID: 8771
		LeftArm = 1,
		// Token: 0x04002244 RID: 8772
		RightArm = 2,
		// Token: 0x04002245 RID: 8773
		Chest = 4,
		// Token: 0x04002246 RID: 8774
		LeftBack = 8,
		// Token: 0x04002247 RID: 8775
		RightBack = 16,
		// Token: 0x04002248 RID: 8776
		MaxDropPostions = 5,
		// Token: 0x04002249 RID: 8777
		All = 31,
		// Token: 0x0400224A RID: 8778
		None = 0
	}

	// Token: 0x0200042F RID: 1071
	public class DockingResult
	{
		// Token: 0x06001A08 RID: 6664 RVA: 0x0008BBC5 File Offset: 0x00089DC5
		public DockingResult()
		{
			this.dockedPosition = new List<BodyDockPositions.DropPositions>(2);
			this.positionsDisabled = new List<BodyDockPositions.DropPositions>(2);
		}

		// Token: 0x0400224B RID: 8779
		public List<BodyDockPositions.DropPositions> positionsDisabled;

		// Token: 0x0400224C RID: 8780
		public List<BodyDockPositions.DropPositions> dockedPosition;
	}
}
