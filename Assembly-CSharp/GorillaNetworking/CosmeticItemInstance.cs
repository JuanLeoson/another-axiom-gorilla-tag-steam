using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000D46 RID: 3398
	public class CosmeticItemInstance
	{
		// Token: 0x06005412 RID: 21522 RVA: 0x0019F964 File Offset: 0x0019DB64
		private void EnableItem(GameObject obj, bool enable)
		{
			CosmeticAnchors component = obj.GetComponent<CosmeticAnchors>();
			try
			{
				if (component && !enable)
				{
					component.EnableAnchor(false);
				}
				obj.SetActive(enable);
				if (component && enable)
				{
					component.EnableAnchor(true);
				}
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Exception while enabling cosmetic: {0}", arg));
			}
		}

		// Token: 0x06005413 RID: 21523 RVA: 0x0019F9C8 File Offset: 0x0019DBC8
		private void ApplyClippingOffsets(bool itemEnabled)
		{
			if (this._bodyDockPositions == null)
			{
				return;
			}
			if (this._anchorOverrides != null)
			{
				if (this.clippingOffsets.nameTag.enabled)
				{
					this._anchorOverrides.UpdateNameTagOffset(itemEnabled ? this.clippingOffsets.nameTag.offset : XformOffset.Identity, itemEnabled, this.activeSlot);
				}
				if (this.clippingOffsets.leftArm.enabled)
				{
					this._anchorOverrides.ApplyAntiClippingOffsets(TransferrableObject.PositionState.OnLeftArm, this.clippingOffsets.leftArm.offset, itemEnabled, this._bodyDockPositions.leftArmTransform);
				}
				if (this.clippingOffsets.rightArm.enabled)
				{
					this._anchorOverrides.ApplyAntiClippingOffsets(TransferrableObject.PositionState.OnRightArm, this.clippingOffsets.rightArm.offset, itemEnabled, this._bodyDockPositions.rightArmTransform);
				}
				if (this.clippingOffsets.chest.enabled)
				{
					this._anchorOverrides.ApplyAntiClippingOffsets(TransferrableObject.PositionState.OnChest, this.clippingOffsets.chest.offset, itemEnabled, this._anchorOverrides.chestDefaultTransform);
				}
				if (this.clippingOffsets.huntComputer.enabled)
				{
					this._anchorOverrides.UpdateHuntWatchOffset(this.clippingOffsets.huntComputer.offset, itemEnabled);
				}
				if (this.clippingOffsets.badge.enabled)
				{
					this._anchorOverrides.UpdateBadgeOffset(itemEnabled ? this.clippingOffsets.badge.offset : XformOffset.Identity, itemEnabled, this.activeSlot);
				}
				if (this.clippingOffsets.builderWatch.enabled)
				{
					this._anchorOverrides.UpdateBuilderWatchOffset(this.clippingOffsets.builderWatch.offset, itemEnabled);
				}
			}
		}

		// Token: 0x06005414 RID: 21524 RVA: 0x0019FB7C File Offset: 0x0019DD7C
		public void DisableItem(CosmeticsController.CosmeticSlots cosmeticSlot)
		{
			bool flag = CosmeticsController.CosmeticSet.IsSlotLeftHanded(cosmeticSlot);
			bool flag2 = CosmeticsController.CosmeticSet.IsSlotRightHanded(cosmeticSlot);
			foreach (GameObject obj in this.objects)
			{
				this.EnableItem(obj, false);
			}
			if (flag)
			{
				foreach (GameObject obj2 in this.leftObjects)
				{
					this.EnableItem(obj2, false);
				}
			}
			if (flag2)
			{
				foreach (GameObject obj3 in this.rightObjects)
				{
					this.EnableItem(obj3, false);
				}
			}
			this.ApplyClippingOffsets(false);
		}

		// Token: 0x06005415 RID: 21525 RVA: 0x0019FC78 File Offset: 0x0019DE78
		public void EnableItem(CosmeticsController.CosmeticSlots cosmeticSlot, VRRig rig)
		{
			bool flag = CosmeticsController.CosmeticSet.IsSlotLeftHanded(cosmeticSlot);
			bool flag2 = CosmeticsController.CosmeticSet.IsSlotRightHanded(cosmeticSlot);
			this.activeSlot = cosmeticSlot;
			if (rig != null && this._anchorOverrides == null)
			{
				this._anchorOverrides = rig.gameObject.GetComponent<VRRigAnchorOverrides>();
				this._bodyDockPositions = rig.GetComponent<BodyDockPositions>();
			}
			foreach (GameObject gameObject in this.objects)
			{
				this.EnableItem(gameObject, true);
				if (cosmeticSlot == CosmeticsController.CosmeticSlots.Badge)
				{
					if (this.objects.Count > 1)
					{
						GTHardCodedBones.EBone ebone;
						Transform transform;
						if (GTHardCodedBones.TryGetFirstBoneInParents(gameObject.transform, out ebone, out transform) && ebone == GTHardCodedBones.EBone.body)
						{
							this._anchorOverrides.CurrentBadgeTransform = gameObject.transform;
						}
					}
					else
					{
						this._anchorOverrides.CurrentBadgeTransform = gameObject.transform;
					}
				}
			}
			if (flag)
			{
				foreach (GameObject obj in this.leftObjects)
				{
					this.EnableItem(obj, true);
				}
			}
			if (flag2)
			{
				foreach (GameObject obj2 in this.rightObjects)
				{
					this.EnableItem(obj2, true);
				}
			}
			this.ApplyClippingOffsets(true);
		}

		// Token: 0x04005DAF RID: 23983
		public List<GameObject> leftObjects = new List<GameObject>();

		// Token: 0x04005DB0 RID: 23984
		public List<GameObject> rightObjects = new List<GameObject>();

		// Token: 0x04005DB1 RID: 23985
		public List<GameObject> objects = new List<GameObject>();

		// Token: 0x04005DB2 RID: 23986
		public List<GameObject> holdableObjects = new List<GameObject>();

		// Token: 0x04005DB3 RID: 23987
		public CosmeticAnchorAntiIntersectOffsets clippingOffsets;

		// Token: 0x04005DB4 RID: 23988
		public string dbgname;

		// Token: 0x04005DB5 RID: 23989
		private BodyDockPositions _bodyDockPositions;

		// Token: 0x04005DB6 RID: 23990
		private VRRigAnchorOverrides _anchorOverrides;

		// Token: 0x04005DB7 RID: 23991
		private CosmeticsController.CosmeticSlots activeSlot;
	}
}
