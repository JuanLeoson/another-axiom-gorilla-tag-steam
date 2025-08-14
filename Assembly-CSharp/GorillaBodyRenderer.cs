using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006B7 RID: 1719
public class GorillaBodyRenderer : MonoBehaviour
{
	// Token: 0x170003EC RID: 1004
	// (get) Token: 0x06002A95 RID: 10901 RVA: 0x000E2E67 File Offset: 0x000E1067
	// (set) Token: 0x06002A96 RID: 10902 RVA: 0x000E2E6F File Offset: 0x000E106F
	public GorillaBodyType bodyType
	{
		get
		{
			return this._bodyType;
		}
		set
		{
			this.SetBodyType(value);
		}
	}

	// Token: 0x170003ED RID: 1005
	// (get) Token: 0x06002A97 RID: 10903 RVA: 0x000E2E78 File Offset: 0x000E1078
	public bool renderFace
	{
		get
		{
			return this._renderFace;
		}
	}

	// Token: 0x170003EE RID: 1006
	// (get) Token: 0x06002A98 RID: 10904 RVA: 0x000E2E80 File Offset: 0x000E1080
	public static bool ForceSkeleton
	{
		get
		{
			return GorillaBodyRenderer.oopsAllSkeletons;
		}
	}

	// Token: 0x170003EF RID: 1007
	// (get) Token: 0x06002A99 RID: 10905 RVA: 0x000E2E87 File Offset: 0x000E1087
	// (set) Token: 0x06002A9A RID: 10906 RVA: 0x000E2E8F File Offset: 0x000E108F
	public GorillaBodyType gameModeBodyType { get; private set; }

	// Token: 0x06002A9B RID: 10907 RVA: 0x000E2E98 File Offset: 0x000E1098
	public SkinnedMeshRenderer GetBody(GorillaBodyType type)
	{
		if (type < GorillaBodyType.Default || type >= (GorillaBodyType)this._renderersCache.Length)
		{
			return null;
		}
		return this._renderersCache[(int)type];
	}

	// Token: 0x170003F0 RID: 1008
	// (get) Token: 0x06002A9C RID: 10908 RVA: 0x000E2EC0 File Offset: 0x000E10C0
	public SkinnedMeshRenderer ActiveBody
	{
		get
		{
			return this.GetBody(this._bodyType);
		}
	}

	// Token: 0x06002A9D RID: 10909 RVA: 0x000E2ED0 File Offset: 0x000E10D0
	public static void SetAllSkeletons(bool allSkeletons)
	{
		GorillaBodyRenderer.oopsAllSkeletons = allSkeletons;
		GorillaTagger.Instance.offlineVRRig.bodyRenderer.Refresh();
		foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
		{
			vrrig.bodyRenderer.Refresh();
		}
	}

	// Token: 0x06002A9E RID: 10910 RVA: 0x000E2F48 File Offset: 0x000E1148
	public void SetGameModeBodyType(GorillaBodyType bodyType)
	{
		if (this.gameModeBodyType == bodyType)
		{
			return;
		}
		this.gameModeBodyType = bodyType;
		this.Refresh();
	}

	// Token: 0x06002A9F RID: 10911 RVA: 0x000E2F61 File Offset: 0x000E1161
	public void SetCosmeticBodyType(GorillaBodyType bodyType)
	{
		if (this.cosmeticBodyType == bodyType)
		{
			return;
		}
		this.cosmeticBodyType = bodyType;
		this.Refresh();
	}

	// Token: 0x06002AA0 RID: 10912 RVA: 0x000E2F7A File Offset: 0x000E117A
	public void SetDefaults()
	{
		this.gameModeBodyType = GorillaBodyType.Default;
		this.cosmeticBodyType = GorillaBodyType.Default;
		this.Refresh();
	}

	// Token: 0x06002AA1 RID: 10913 RVA: 0x000E2F90 File Offset: 0x000E1190
	private void Refresh()
	{
		this.SetBodyType(this.GetActiveBodyType());
	}

	// Token: 0x06002AA2 RID: 10914 RVA: 0x000E2FA0 File Offset: 0x000E11A0
	public void SetMaterialIndex(int materialIndex)
	{
		this.bodyDefault.sharedMaterial = this.rig.materialsToChangeTo[materialIndex];
		this.bodyNoHead.sharedMaterial = this.bodyDefault.sharedMaterial;
		this.rig.skeleton.SetMaterialIndex(materialIndex);
	}

	// Token: 0x06002AA3 RID: 10915 RVA: 0x000E2FEC File Offset: 0x000E11EC
	public void SetSkinMaterials(Material bodyMat, Material chestMat)
	{
		Material[] sharedMaterials = this.bodyDefault.sharedMaterials;
		sharedMaterials[0] = bodyMat;
		sharedMaterials[1] = chestMat;
		this.bodyDefault.sharedMaterials = sharedMaterials;
		this.bodyNoHead.sharedMaterials = sharedMaterials;
	}

	// Token: 0x06002AA4 RID: 10916 RVA: 0x000E3025 File Offset: 0x000E1225
	public void SetupAsLocalPlayerBody()
	{
		this.faceRenderer.gameObject.layer = 22;
	}

	// Token: 0x06002AA5 RID: 10917 RVA: 0x000E3039 File Offset: 0x000E1239
	public GorillaBodyType GetActiveBodyType()
	{
		if (GorillaBodyRenderer.oopsAllSkeletons)
		{
			return GorillaBodyType.Skeleton;
		}
		if (this.gameModeBodyType == GorillaBodyType.Default)
		{
			return this.cosmeticBodyType;
		}
		return this.gameModeBodyType;
	}

	// Token: 0x06002AA6 RID: 10918 RVA: 0x000E305C File Offset: 0x000E125C
	private void SetBodyType(GorillaBodyType type)
	{
		if (this._bodyType == type)
		{
			return;
		}
		this.SetBodyEnabled(this._bodyType, false);
		this._bodyType = type;
		this.SetBodyEnabled(type, true);
		this._renderFace = (this._bodyType != GorillaBodyType.NoHead && this._bodyType != GorillaBodyType.Skeleton && this._bodyType != GorillaBodyType.Invisible);
		if (this.faceRenderer != null)
		{
			this.faceRenderer.enabled = this._renderFace;
		}
	}

	// Token: 0x06002AA7 RID: 10919 RVA: 0x000E30D8 File Offset: 0x000E12D8
	private void SetBodyEnabled(GorillaBodyType bodyType, bool enabled)
	{
		SkinnedMeshRenderer body = this.GetBody(bodyType);
		if (body == null)
		{
			return;
		}
		body.enabled = enabled;
		Transform[] bones = body.bones;
		for (int i = 0; i < bones.Length; i++)
		{
			bones[i].gameObject.SetActive(enabled);
		}
	}

	// Token: 0x06002AA8 RID: 10920 RVA: 0x000E3121 File Offset: 0x000E1321
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x06002AA9 RID: 10921 RVA: 0x000E312C File Offset: 0x000E132C
	private void Setup()
	{
		this.rig = base.GetComponentInParent<VRRig>();
		this._renderersCache = new SkinnedMeshRenderer[EnumData<GorillaBodyType>.Shared.Values.Length];
		this._renderersCache[0] = this.bodyDefault;
		this._renderersCache[1] = this.bodyNoHead;
		this._renderersCache[2] = this.bodySkeleton;
		this.SetBodyEnabled(GorillaBodyType.Default, true);
		this.SetBodyEnabled(GorillaBodyType.NoHead, false);
		this.SetBodyEnabled(GorillaBodyType.Skeleton, false);
		this._bodyType = GorillaBodyType.Default;
		this.bodyDefault.GetSharedMaterials(this._cachedDefaultMats);
		this.Refresh();
	}

	// Token: 0x04003603 RID: 13827
	[SerializeField]
	private GorillaBodyType _bodyType;

	// Token: 0x04003604 RID: 13828
	[SerializeField]
	private bool _renderFace = true;

	// Token: 0x04003605 RID: 13829
	public MeshRenderer faceRenderer;

	// Token: 0x04003606 RID: 13830
	public SkinnedMeshRenderer bodyDefault;

	// Token: 0x04003607 RID: 13831
	public SkinnedMeshRenderer bodyNoHead;

	// Token: 0x04003608 RID: 13832
	public SkinnedMeshRenderer bodySkeleton;

	// Token: 0x04003609 RID: 13833
	public SkinnedMeshRenderer bodyCosmetic;

	// Token: 0x0400360A RID: 13834
	private static bool oopsAllSkeletons;

	// Token: 0x0400360C RID: 13836
	private GorillaBodyType cosmeticBodyType;

	// Token: 0x0400360D RID: 13837
	[Space]
	[NonSerialized]
	private SkinnedMeshRenderer[] _renderersCache = new SkinnedMeshRenderer[0];

	// Token: 0x0400360E RID: 13838
	[NonSerialized]
	private List<Material> _cachedDefaultMats = new List<Material>(2);

	// Token: 0x0400360F RID: 13839
	private static readonly List<Material> gEmptyDefaultMats = new List<Material>();

	// Token: 0x04003610 RID: 13840
	[Space]
	public VRRig rig;
}
