using System;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000434 RID: 1076
public class CosmeticAnchors : MonoBehaviour, ISpawnable
{
	// Token: 0x170002E3 RID: 739
	// (get) Token: 0x06001A2E RID: 6702 RVA: 0x0008C10C File Offset: 0x0008A30C
	// (set) Token: 0x06001A2F RID: 6703 RVA: 0x0008C114 File Offset: 0x0008A314
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170002E4 RID: 740
	// (get) Token: 0x06001A30 RID: 6704 RVA: 0x0008C11D File Offset: 0x0008A31D
	// (set) Token: 0x06001A31 RID: 6705 RVA: 0x0008C125 File Offset: 0x0008A325
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06001A32 RID: 6706 RVA: 0x0008C130 File Offset: 0x0008A330
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.anchorEnabled = false;
		this.vrRig = rig;
		if (!this.vrRig)
		{
			Debug.LogError("CosmeticAnchors.OnSpawn: Disabling! Could not find VRRig in parent! Path: " + base.transform.GetPathQ(), this);
			base.enabled = false;
			return;
		}
		this.anchorOverrides = this.vrRig.gameObject.GetComponent<VRRigAnchorOverrides>();
		this.AssignAnchorToPath(ref this.nameAnchor, this.nameAnchor_path);
		this.AssignAnchorToPath(ref this.leftArmAnchor, this.leftArmAnchor_path);
		this.AssignAnchorToPath(ref this.rightArmAnchor, this.rightArmAnchor_path);
		this.AssignAnchorToPath(ref this.chestAnchor, this.chestAnchor_path);
		this.AssignAnchorToPath(ref this.huntComputerAnchor, this.huntComputerAnchor_path);
		this.AssignAnchorToPath(ref this.badgeAnchor, this.badgeAnchor_path);
		this.AssignAnchorToPath(ref this.builderWatchAnchor, this.builderWatchAnchor_path);
		this.AssignAnchorToPath(ref this.friendshipBraceletLeftOverride, this.friendshipBraceletLeftOverride_path);
		this.AssignAnchorToPath(ref this.friendshipBraceletRightOverride, this.friendshipBraceletRightOverride_path);
	}

	// Token: 0x06001A33 RID: 6707 RVA: 0x000023F5 File Offset: 0x000005F5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06001A34 RID: 6708 RVA: 0x0008C234 File Offset: 0x0008A434
	private void AssignAnchorToPath(ref GameObject anchorGObjRef, string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return;
		}
		Transform transform;
		if (!base.transform.TryFindByPath(path, out transform, false))
		{
			this.vrRig = base.GetComponentInParent<VRRig>(true);
			if (this.vrRig && this.vrRig.isOfflineVRRig)
			{
				Debug.LogError("CosmeticAnchors: Could not find path: \"" + path + "\".\nPath to this component: " + base.transform.GetPathQ(), this);
			}
			return;
		}
		anchorGObjRef = transform.gameObject;
	}

	// Token: 0x06001A35 RID: 6709 RVA: 0x0008C2AC File Offset: 0x0008A4AC
	private void OnEnable()
	{
		if (this.huntComputerAnchor || this.builderWatchAnchor)
		{
			CosmeticAnchorManager.RegisterCosmeticAnchor(this);
		}
	}

	// Token: 0x06001A36 RID: 6710 RVA: 0x0008C2CE File Offset: 0x0008A4CE
	private void OnDisable()
	{
		if (this.huntComputerAnchor || this.builderWatchAnchor)
		{
			CosmeticAnchorManager.UnregisterCosmeticAnchor(this);
		}
	}

	// Token: 0x06001A37 RID: 6711 RVA: 0x0008C2F0 File Offset: 0x0008A4F0
	public void TryUpdate()
	{
		if (this.anchorEnabled && this.huntComputerAnchor && !GorillaTagger.Instance.offlineVRRig.huntComputer.activeSelf && this.anchorOverrides.HuntComputer.parent != this.anchorOverrides.HuntDefaultAnchor)
		{
			this.anchorOverrides.HuntComputer.parent = this.anchorOverrides.HuntDefaultAnchor;
			return;
		}
		if (this.anchorEnabled && this.huntComputerAnchor && GorillaTagger.Instance.offlineVRRig.huntComputer.activeSelf && this.anchorOverrides.HuntComputer.parent == this.anchorOverrides.HuntDefaultAnchor)
		{
			this.SetHuntComputerAnchor(this.anchorEnabled);
		}
		if (this.anchorEnabled && this.builderWatchAnchor && !GorillaTagger.Instance.offlineVRRig.builderResizeWatch.activeSelf && this.anchorOverrides.BuilderWatch.parent != this.anchorOverrides.BuilderWatchAnchor)
		{
			this.anchorOverrides.BuilderWatch.parent = this.anchorOverrides.BuilderWatchAnchor;
			return;
		}
		if (this.anchorEnabled && this.builderWatchAnchor && GorillaTagger.Instance.offlineVRRig.builderResizeWatch.activeSelf && this.anchorOverrides.BuilderWatch.parent == this.anchorOverrides.BuilderWatchAnchor)
		{
			this.SetBuilderWatchAnchor(this.anchorEnabled);
		}
	}

	// Token: 0x06001A38 RID: 6712 RVA: 0x0008C484 File Offset: 0x0008A684
	public void EnableAnchor(bool enable)
	{
		this.anchorEnabled = enable;
		if (this.anchorOverrides == null)
		{
			return;
		}
		if (this.leftArmAnchor)
		{
			this.anchorOverrides.OverrideAnchor(TransferrableObject.PositionState.OnLeftArm, enable ? this.leftArmAnchor.transform : null);
		}
		if (this.rightArmAnchor)
		{
			this.anchorOverrides.OverrideAnchor(TransferrableObject.PositionState.OnRightArm, enable ? this.rightArmAnchor.transform : null);
		}
		if (this.chestAnchor)
		{
			this.anchorOverrides.OverrideAnchor(TransferrableObject.PositionState.OnChest, enable ? this.chestAnchor.transform : this.anchorOverrides.chestDefaultTransform);
		}
		this.anchorOverrides.UpdateNameAnchor(enable ? this.nameAnchor : null, this.slot);
		this.anchorOverrides.UpdateBadgeAnchor(enable ? this.badgeAnchor : null, this.slot);
		if (this.huntComputerAnchor)
		{
			this.SetHuntComputerAnchor(enable);
		}
		if (this.builderWatchAnchor)
		{
			this.SetBuilderWatchAnchor(enable);
		}
		this.SetCustomAnchor(this.anchorOverrides.friendshipBraceletLeftAnchor, enable, this.friendshipBraceletLeftOverride, this.anchorOverrides.friendshipBraceletLeftDefaultAnchor);
		this.SetCustomAnchor(this.anchorOverrides.friendshipBraceletRightAnchor, enable, this.friendshipBraceletRightOverride, this.anchorOverrides.friendshipBraceletRightDefaultAnchor);
	}

	// Token: 0x06001A39 RID: 6713 RVA: 0x0008C5D8 File Offset: 0x0008A7D8
	private void SetHuntComputerAnchor(bool enable)
	{
		Transform huntComputer = this.anchorOverrides.HuntComputer;
		if (!GorillaTagger.Instance.offlineVRRig.huntComputer.activeSelf || !enable)
		{
			huntComputer.parent = this.anchorOverrides.HuntDefaultAnchor;
		}
		else
		{
			huntComputer.parent = this.huntComputerAnchor.transform;
		}
		huntComputer.transform.localPosition = Vector3.zero;
		huntComputer.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x06001A3A RID: 6714 RVA: 0x0008C650 File Offset: 0x0008A850
	private void SetBuilderWatchAnchor(bool enable)
	{
		Transform builderWatch = this.anchorOverrides.BuilderWatch;
		if (!GorillaTagger.Instance.offlineVRRig.builderResizeWatch.activeSelf || !enable)
		{
			builderWatch.parent = this.anchorOverrides.BuilderWatchAnchor;
		}
		else
		{
			builderWatch.parent = this.builderWatchAnchor.transform;
		}
		builderWatch.transform.localPosition = Vector3.zero;
		builderWatch.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x06001A3B RID: 6715 RVA: 0x0008C6C8 File Offset: 0x0008A8C8
	private void SetCustomAnchor(Transform target, bool enable, GameObject overrideAnchor, Transform defaultAnchor)
	{
		Transform transform = (enable && overrideAnchor != null) ? overrideAnchor.transform : defaultAnchor;
		if (target != null && target.parent != transform)
		{
			target.parent = transform;
			target.transform.localPosition = Vector3.zero;
			target.transform.localRotation = Quaternion.identity;
			target.transform.localScale = Vector3.one;
		}
	}

	// Token: 0x06001A3C RID: 6716 RVA: 0x0008C73C File Offset: 0x0008A93C
	public Transform GetPositionAnchor(TransferrableObject.PositionState pos)
	{
		if (pos != TransferrableObject.PositionState.OnLeftArm)
		{
			if (pos != TransferrableObject.PositionState.OnRightArm)
			{
				if (pos != TransferrableObject.PositionState.OnChest)
				{
					return null;
				}
				if (!this.chestAnchor)
				{
					return null;
				}
				return this.chestAnchor.transform;
			}
			else
			{
				if (!this.rightArmAnchor)
				{
					return null;
				}
				return this.rightArmAnchor.transform;
			}
		}
		else
		{
			if (!this.leftArmAnchor)
			{
				return null;
			}
			return this.leftArmAnchor.transform;
		}
	}

	// Token: 0x06001A3D RID: 6717 RVA: 0x0008C7AA File Offset: 0x0008A9AA
	public Transform GetNameAnchor()
	{
		if (!this.nameAnchor)
		{
			return null;
		}
		return this.nameAnchor.transform;
	}

	// Token: 0x06001A3E RID: 6718 RVA: 0x0008C7C6 File Offset: 0x0008A9C6
	public bool AffectedByHunt()
	{
		return this.huntComputerAnchor != null;
	}

	// Token: 0x06001A3F RID: 6719 RVA: 0x0008C7D4 File Offset: 0x0008A9D4
	public bool AffectedByBuilder()
	{
		return this.builderWatchAnchor != null;
	}

	// Token: 0x04002263 RID: 8803
	[SerializeField]
	protected GameObject nameAnchor;

	// Token: 0x04002264 RID: 8804
	[Delayed]
	[SerializeField]
	protected string nameAnchor_path;

	// Token: 0x04002265 RID: 8805
	[SerializeField]
	protected GameObject leftArmAnchor;

	// Token: 0x04002266 RID: 8806
	[Delayed]
	[SerializeField]
	protected string leftArmAnchor_path;

	// Token: 0x04002267 RID: 8807
	[SerializeField]
	protected GameObject rightArmAnchor;

	// Token: 0x04002268 RID: 8808
	[Delayed]
	[SerializeField]
	protected string rightArmAnchor_path;

	// Token: 0x04002269 RID: 8809
	[SerializeField]
	protected GameObject chestAnchor;

	// Token: 0x0400226A RID: 8810
	[Delayed]
	[SerializeField]
	protected string chestAnchor_path;

	// Token: 0x0400226B RID: 8811
	[SerializeField]
	protected GameObject huntComputerAnchor;

	// Token: 0x0400226C RID: 8812
	[Delayed]
	[SerializeField]
	protected string huntComputerAnchor_path;

	// Token: 0x0400226D RID: 8813
	[SerializeField]
	protected GameObject builderWatchAnchor;

	// Token: 0x0400226E RID: 8814
	[Delayed]
	[SerializeField]
	protected string builderWatchAnchor_path;

	// Token: 0x0400226F RID: 8815
	[SerializeField]
	protected GameObject friendshipBraceletLeftOverride;

	// Token: 0x04002270 RID: 8816
	[Delayed]
	[SerializeField]
	protected string friendshipBraceletLeftOverride_path;

	// Token: 0x04002271 RID: 8817
	[SerializeField]
	protected GameObject friendshipBraceletRightOverride;

	// Token: 0x04002272 RID: 8818
	[Delayed]
	[SerializeField]
	protected string friendshipBraceletRightOverride_path;

	// Token: 0x04002273 RID: 8819
	[SerializeField]
	protected GameObject badgeAnchor;

	// Token: 0x04002274 RID: 8820
	[Delayed]
	[SerializeField]
	protected string badgeAnchor_path;

	// Token: 0x04002275 RID: 8821
	[SerializeField]
	public CosmeticsController.CosmeticSlots slot;

	// Token: 0x04002276 RID: 8822
	private VRRig vrRig;

	// Token: 0x04002277 RID: 8823
	private VRRigAnchorOverrides anchorOverrides;

	// Token: 0x04002278 RID: 8824
	private bool anchorEnabled;

	// Token: 0x04002279 RID: 8825
	private static GTLogErrorLimiter k_debugLogError_anchorOverridesNull = new GTLogErrorLimiter("The array `anchorOverrides` was null. Is the cosmetic getting initialized properly? ", 10, "\n- ");
}
