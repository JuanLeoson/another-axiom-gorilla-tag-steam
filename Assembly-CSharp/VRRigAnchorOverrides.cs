using System;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000469 RID: 1129
public class VRRigAnchorOverrides : MonoBehaviour
{
	// Token: 0x17000300 RID: 768
	// (get) Token: 0x06001C07 RID: 7175 RVA: 0x000969CD File Offset: 0x00094BCD
	// (set) Token: 0x06001C08 RID: 7176 RVA: 0x000969D8 File Offset: 0x00094BD8
	public Transform CurrentBadgeTransform
	{
		get
		{
			return this.currentBadgeTransform;
		}
		set
		{
			if (value != this.currentBadgeTransform)
			{
				this.ResetBadge();
				this.currentBadgeTransform = value;
				this.badgeDefaultRot = this.currentBadgeTransform.localRotation;
				this.badgeDefaultPos = this.currentBadgeTransform.localPosition;
				this.UpdateBadge();
			}
		}
	}

	// Token: 0x17000301 RID: 769
	// (get) Token: 0x06001C09 RID: 7177 RVA: 0x00096A28 File Offset: 0x00094C28
	public Transform HuntDefaultAnchor
	{
		get
		{
			return this.huntComputerDefaultAnchor;
		}
	}

	// Token: 0x17000302 RID: 770
	// (get) Token: 0x06001C0A RID: 7178 RVA: 0x00096A30 File Offset: 0x00094C30
	public Transform HuntComputer
	{
		get
		{
			return this.huntComputer;
		}
	}

	// Token: 0x17000303 RID: 771
	// (get) Token: 0x06001C0B RID: 7179 RVA: 0x00096A38 File Offset: 0x00094C38
	public Transform BuilderWatchAnchor
	{
		get
		{
			return this.builderResizeButtonDefaultAnchor;
		}
	}

	// Token: 0x17000304 RID: 772
	// (get) Token: 0x06001C0C RID: 7180 RVA: 0x00096A40 File Offset: 0x00094C40
	public Transform BuilderWatch
	{
		get
		{
			return this.builderResizeButton;
		}
	}

	// Token: 0x06001C0D RID: 7181 RVA: 0x00096A48 File Offset: 0x00094C48
	private void Awake()
	{
		for (int i = 0; i < 8; i++)
		{
			this.overrideAnchors[i] = null;
		}
		int num = this.MapPositionToIndex(TransferrableObject.PositionState.OnChest);
		this.overrideAnchors[num] = this.chestDefaultTransform;
		this.huntDefaultTransform = this.huntComputer;
		this.builderResizeButtonDefaultTransform = this.builderResizeButton;
		this.activeAntiClippingOffsets = default(CosmeticAnchorAntiIntersectOffsets);
	}

	// Token: 0x06001C0E RID: 7182 RVA: 0x00096AA8 File Offset: 0x00094CA8
	private void OnEnable()
	{
		if (this.nameDefaultAnchor && this.nameDefaultAnchor.parent)
		{
			this.nameTransform.parent = this.nameDefaultAnchor.parent;
		}
		else
		{
			Debug.LogError("VRRigAnchorOverrides: could not set parent `nameTransform` because `nameDefaultAnchor` or its parent was null!" + base.transform.GetPathQ(), this);
		}
		this.huntComputer = this.huntDefaultTransform;
		if (this.huntComputerDefaultAnchor && this.huntComputerDefaultAnchor.parent)
		{
			this.huntComputer.parent = this.huntComputerDefaultAnchor.parent;
		}
		else
		{
			Debug.LogError("VRRigAnchorOverrides: could not set parent `huntComputer` because `huntComputerDefaultAnchor` or its parent was null!" + base.transform.GetPathQ(), this);
		}
		this.builderResizeButton = this.builderResizeButtonDefaultTransform;
		if (this.builderResizeButtonDefaultAnchor && this.builderResizeButtonDefaultAnchor.parent)
		{
			this.builderResizeButton.parent = this.builderResizeButtonDefaultAnchor.parent;
			return;
		}
		Debug.LogError("VRRigAnchorOverrides: could not set parent `builderResizeButton` because `builderResizeButtonDefaultAnchor` or its parent was null! Path: " + base.transform.GetPathQ(), this);
	}

	// Token: 0x06001C0F RID: 7183 RVA: 0x00096BC4 File Offset: 0x00094DC4
	private int MapPositionToIndex(TransferrableObject.PositionState pos)
	{
		int num = (int)pos;
		int num2 = 0;
		while ((num >>= 1) != 0)
		{
			num2++;
		}
		return num2;
	}

	// Token: 0x06001C10 RID: 7184 RVA: 0x00096BE4 File Offset: 0x00094DE4
	public void ApplyAntiClippingOffsets(TransferrableObject.PositionState pos, XformOffset offset, bool enable, Transform defaultAnchor)
	{
		int num = this.MapPositionToIndex(pos);
		if (pos != TransferrableObject.PositionState.OnLeftArm)
		{
			if (pos != TransferrableObject.PositionState.OnRightArm)
			{
				if (pos != TransferrableObject.PositionState.OnChest)
				{
					GTDev.LogWarning<string>(string.Format("Anti Clipping offset for position {0} is not implemented", pos), null);
					return;
				}
				this.activeAntiClippingOffsets.chest.enabled = enable;
				this.activeAntiClippingOffsets.chest.offset = (enable ? offset : XformOffset.Identity);
			}
			else
			{
				this.activeAntiClippingOffsets.rightArm.enabled = enable;
				this.activeAntiClippingOffsets.rightArm.offset = (enable ? offset : XformOffset.Identity);
			}
		}
		else
		{
			this.activeAntiClippingOffsets.leftArm.enabled = enable;
			this.activeAntiClippingOffsets.leftArm.offset = (enable ? offset : XformOffset.Identity);
		}
		if (enable && (this.overrideAnchors[num] == null || (pos == TransferrableObject.PositionState.OnChest && this.overrideAnchors[num] == this.chestDefaultTransform)))
		{
			if (this.clippingOffsetTransforms[num] == null)
			{
				GameObject gameObject = new GameObject("Anti Clipping Offset");
				gameObject.transform.SetParent(defaultAnchor);
				this.clippingOffsetTransforms[num] = gameObject.transform;
			}
			Transform transform = this.clippingOffsetTransforms[num];
			transform.SetParent(defaultAnchor);
			transform.localPosition = offset.pos;
			transform.localRotation = offset.rot;
			transform.localScale = Vector3.one;
			this.OverrideAnchor(pos, transform);
			return;
		}
		if (!enable && this.overrideAnchors[num] == this.clippingOffsetTransforms[num])
		{
			if (pos == TransferrableObject.PositionState.OnChest)
			{
				this.OverrideAnchor(pos, this.chestDefaultTransform);
				return;
			}
			this.OverrideAnchor(pos, null);
		}
	}

	// Token: 0x06001C11 RID: 7185 RVA: 0x00096D88 File Offset: 0x00094F88
	public void OverrideAnchor(TransferrableObject.PositionState pos, Transform anchor)
	{
		int num = this.MapPositionToIndex(pos);
		if (this.overrideAnchors[num] == this.chestDefaultTransform)
		{
			foreach (object obj in this.overrideAnchors[num])
			{
				Transform transform = (Transform)obj;
				if (!transform.name.Equals("DropZoneChest") && transform != anchor)
				{
					transform.parent = null;
				}
			}
			this.overrideAnchors[num] = anchor;
			return;
		}
		if (this.overrideAnchors[num])
		{
			foreach (object obj2 in this.overrideAnchors[num])
			{
				Transform transform2 = (Transform)obj2;
				if (transform2 != anchor)
				{
					transform2.parent = null;
				}
			}
		}
		this.overrideAnchors[num] = anchor;
	}

	// Token: 0x06001C12 RID: 7186 RVA: 0x00096E94 File Offset: 0x00095094
	public Transform AnchorOverride(TransferrableObject.PositionState pos, Transform fallback)
	{
		int num = this.MapPositionToIndex(pos);
		Transform transform = this.overrideAnchors[num];
		if (transform != null)
		{
			return transform;
		}
		return fallback;
	}

	// Token: 0x06001C13 RID: 7187 RVA: 0x00096EB8 File Offset: 0x000950B8
	public void UpdateHuntWatchOffset(XformOffset offset, bool enable)
	{
		this.activeAntiClippingOffsets.huntComputer.enabled = enable;
		this.activeAntiClippingOffsets.huntComputer.offset = (enable ? offset : XformOffset.Identity);
		this.huntComputer.parent = this.HuntDefaultAnchor;
		this.huntComputer.localPosition = this.activeAntiClippingOffsets.huntComputer.offset.pos;
		this.huntComputer.localRotation = this.activeAntiClippingOffsets.huntComputer.offset.rot;
	}

	// Token: 0x06001C14 RID: 7188 RVA: 0x00096F44 File Offset: 0x00095144
	public void UpdateBuilderWatchOffset(XformOffset offset, bool enable)
	{
		this.activeAntiClippingOffsets.builderWatch.enabled = enable;
		this.activeAntiClippingOffsets.builderWatch.offset = (enable ? offset : XformOffset.Identity);
		this.BuilderWatch.parent = this.BuilderWatchAnchor;
		this.BuilderWatch.localPosition = this.activeAntiClippingOffsets.builderWatch.offset.pos;
		this.BuilderWatch.localRotation = this.activeAntiClippingOffsets.builderWatch.offset.rot;
	}

	// Token: 0x06001C15 RID: 7189 RVA: 0x00096FD0 File Offset: 0x000951D0
	public void UpdateNameTagOffset(XformOffset offset, bool enable, CosmeticsController.CosmeticSlots slot)
	{
		if (slot != CosmeticsController.CosmeticSlots.Badge)
		{
			if (slot != CosmeticsController.CosmeticSlots.Face)
			{
				switch (slot)
				{
				case CosmeticsController.CosmeticSlots.Shirt:
					this.nameOffsets[0].enabled = enable;
					this.nameOffsets[0].offset = offset;
					break;
				case CosmeticsController.CosmeticSlots.Pants:
					this.nameOffsets[1].enabled = enable;
					this.nameOffsets[1].offset = offset;
					break;
				case CosmeticsController.CosmeticSlots.Back:
					this.nameOffsets[2].enabled = enable;
					this.nameOffsets[2].offset = offset;
					break;
				}
			}
			else
			{
				this.nameOffsets[3].enabled = enable;
				this.nameOffsets[3].offset = offset;
			}
		}
		else
		{
			this.nameOffsets[4].enabled = enable;
			this.nameOffsets[4].offset = offset;
		}
		this.UpdateName();
	}

	// Token: 0x06001C16 RID: 7190 RVA: 0x000970CC File Offset: 0x000952CC
	public void UpdateNameAnchor(GameObject nameAnchor, CosmeticsController.CosmeticSlots slot)
	{
		if (slot != CosmeticsController.CosmeticSlots.Badge)
		{
			if (slot != CosmeticsController.CosmeticSlots.Face)
			{
				switch (slot)
				{
				case CosmeticsController.CosmeticSlots.Shirt:
					this.nameAnchors[0] = nameAnchor;
					break;
				case CosmeticsController.CosmeticSlots.Pants:
					this.nameAnchors[1] = nameAnchor;
					break;
				case CosmeticsController.CosmeticSlots.Back:
					this.nameAnchors[2] = nameAnchor;
					break;
				}
			}
			else
			{
				this.nameAnchors[3] = nameAnchor;
			}
		}
		else
		{
			this.nameAnchors[4] = nameAnchor;
		}
		this.UpdateName();
	}

	// Token: 0x06001C17 RID: 7191 RVA: 0x00097134 File Offset: 0x00095334
	private void UpdateName()
	{
		for (int i = 0; i < this.nameAnchors.Length; i++)
		{
			if (this.nameAnchors[i] != null)
			{
				this.nameTransform.parent = this.nameAnchors[i].transform;
				this.nameTransform.localRotation = Quaternion.identity;
				this.nameTransform.localPosition = Vector3.zero;
				return;
			}
			if (this.nameOffsets[i].enabled)
			{
				this.nameTransform.parent = this.nameDefaultAnchor;
				this.nameTransform.localRotation = this.nameOffsets[i].offset.rot;
				this.nameTransform.localPosition = this.nameOffsets[i].offset.pos;
				return;
			}
		}
		if (this.nameDefaultAnchor)
		{
			this.nameTransform.parent = this.nameDefaultAnchor;
			this.nameTransform.localRotation = Quaternion.identity;
			this.nameTransform.localPosition = Vector3.zero;
			return;
		}
		Debug.LogError("VRRigAnchorOverrides: could not set parent for `nameTransform` because `nameDefaultAnchor` or its parent was null! Path: " + base.transform.GetPathQ(), this);
	}

	// Token: 0x06001C18 RID: 7192 RVA: 0x00097264 File Offset: 0x00095464
	public void UpdateBadgeOffset(XformOffset offset, bool enable, CosmeticsController.CosmeticSlots slot)
	{
		switch (slot)
		{
		case CosmeticsController.CosmeticSlots.Shirt:
			this.badgeOffsets[0].enabled = enable;
			this.badgeOffsets[0].offset = offset;
			break;
		case CosmeticsController.CosmeticSlots.Pants:
			this.badgeOffsets[1].enabled = enable;
			this.badgeOffsets[1].offset = offset;
			break;
		case CosmeticsController.CosmeticSlots.Back:
			this.badgeOffsets[2].enabled = enable;
			this.badgeOffsets[2].offset = offset;
			break;
		}
		this.UpdateBadge();
	}

	// Token: 0x06001C19 RID: 7193 RVA: 0x000972FE File Offset: 0x000954FE
	public void UpdateBadgeAnchor(GameObject badgeAnchor, CosmeticsController.CosmeticSlots slot)
	{
		switch (slot)
		{
		case CosmeticsController.CosmeticSlots.Shirt:
			this.badgeAnchors[0] = badgeAnchor;
			break;
		case CosmeticsController.CosmeticSlots.Pants:
			this.badgeAnchors[1] = badgeAnchor;
			break;
		case CosmeticsController.CosmeticSlots.Back:
			this.badgeAnchors[2] = badgeAnchor;
			break;
		}
		this.UpdateBadge();
	}

	// Token: 0x06001C1A RID: 7194 RVA: 0x0009733C File Offset: 0x0009553C
	private void UpdateBadge()
	{
		if (!this.currentBadgeTransform)
		{
			return;
		}
		for (int i = 0; i < this.badgeAnchors.Length; i++)
		{
			if (this.badgeAnchors[i] != null)
			{
				this.currentBadgeTransform.localRotation = this.badgeAnchors[i].transform.localRotation;
				this.currentBadgeTransform.localPosition = this.badgeAnchors[i].transform.localPosition;
				return;
			}
			if (this.badgeOffsets[i].enabled)
			{
				Matrix4x4 matrix = Matrix4x4.TRS(this.badgeDefaultPos, this.badgeDefaultRot, Vector3.one) * Matrix4x4.TRS(this.badgeOffsets[i].offset.pos, this.badgeOffsets[i].offset.rot, Vector3.one);
				this.currentBadgeTransform.localRotation = matrix.rotation;
				this.currentBadgeTransform.localPosition = matrix.Position();
				return;
			}
		}
		foreach (GameObject gameObject in this.badgeAnchors)
		{
			if (gameObject)
			{
				this.currentBadgeTransform.localRotation = gameObject.transform.localRotation;
				this.currentBadgeTransform.localPosition = gameObject.transform.localPosition;
				return;
			}
		}
		this.ResetBadge();
	}

	// Token: 0x06001C1B RID: 7195 RVA: 0x0009749C File Offset: 0x0009569C
	private void ResetBadge()
	{
		if (!this.currentBadgeTransform)
		{
			return;
		}
		this.currentBadgeTransform.localRotation = this.badgeDefaultRot;
		this.currentBadgeTransform.localPosition = this.badgeDefaultPos;
	}

	// Token: 0x06001C1C RID: 7196 RVA: 0x000974D0 File Offset: 0x000956D0
	private void OnDestroy()
	{
		for (int i = 0; i < this.clippingOffsetTransforms.Length; i++)
		{
			if (this.clippingOffsetTransforms[i] != null)
			{
				foreach (object obj in this.clippingOffsetTransforms[i])
				{
					((Transform)obj).parent = null;
				}
				Object.Destroy(this.clippingOffsetTransforms[i].gameObject);
			}
		}
	}

	// Token: 0x0400248B RID: 9355
	[SerializeField]
	internal Transform nameDefaultAnchor;

	// Token: 0x0400248C RID: 9356
	[SerializeField]
	internal Transform nameTransform;

	// Token: 0x0400248D RID: 9357
	[SerializeField]
	internal Transform chestDefaultTransform;

	// Token: 0x0400248E RID: 9358
	[SerializeField]
	internal Transform huntComputer;

	// Token: 0x0400248F RID: 9359
	[SerializeField]
	internal Transform huntComputerDefaultAnchor;

	// Token: 0x04002490 RID: 9360
	private Transform huntDefaultTransform;

	// Token: 0x04002491 RID: 9361
	[SerializeField]
	protected Transform builderResizeButton;

	// Token: 0x04002492 RID: 9362
	[SerializeField]
	protected Transform builderResizeButtonDefaultAnchor;

	// Token: 0x04002493 RID: 9363
	private Transform builderResizeButtonDefaultTransform;

	// Token: 0x04002494 RID: 9364
	private readonly Transform[] overrideAnchors = new Transform[8];

	// Token: 0x04002495 RID: 9365
	private CosmeticAnchorAntiIntersectOffsets activeAntiClippingOffsets;

	// Token: 0x04002496 RID: 9366
	private Transform[] clippingOffsetTransforms = new Transform[8];

	// Token: 0x04002497 RID: 9367
	private GameObject nameLastObjectToAttach;

	// Token: 0x04002498 RID: 9368
	private Transform currentBadgeTransform;

	// Token: 0x04002499 RID: 9369
	private Vector3 badgeDefaultPos;

	// Token: 0x0400249A RID: 9370
	private Quaternion badgeDefaultRot;

	// Token: 0x0400249B RID: 9371
	private GameObject[] badgeAnchors = new GameObject[3];

	// Token: 0x0400249C RID: 9372
	private GameObject[] nameAnchors = new GameObject[5];

	// Token: 0x0400249D RID: 9373
	private CosmeticAnchorAntiClipEntry[] badgeOffsets = new CosmeticAnchorAntiClipEntry[3];

	// Token: 0x0400249E RID: 9374
	private CosmeticAnchorAntiClipEntry[] nameOffsets = new CosmeticAnchorAntiClipEntry[5];

	// Token: 0x0400249F RID: 9375
	[SerializeField]
	public Transform friendshipBraceletLeftDefaultAnchor;

	// Token: 0x040024A0 RID: 9376
	public Transform friendshipBraceletLeftAnchor;

	// Token: 0x040024A1 RID: 9377
	[SerializeField]
	public Transform friendshipBraceletRightDefaultAnchor;

	// Token: 0x040024A2 RID: 9378
	public Transform friendshipBraceletRightAnchor;
}
