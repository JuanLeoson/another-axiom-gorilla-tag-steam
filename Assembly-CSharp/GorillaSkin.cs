using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020001A0 RID: 416
public class GorillaSkin : ScriptableObject
{
	// Token: 0x17000100 RID: 256
	// (get) Token: 0x06000A71 RID: 2673 RVA: 0x00038922 File Offset: 0x00036B22
	public Mesh bodyMesh
	{
		get
		{
			return this._bodyMesh;
		}
	}

	// Token: 0x06000A72 RID: 2674 RVA: 0x0003892C File Offset: 0x00036B2C
	public static GorillaSkin CopyWithInstancedMaterials(GorillaSkin basis)
	{
		GorillaSkin gorillaSkin = ScriptableObject.CreateInstance<GorillaSkin>();
		gorillaSkin._chestMaterial = new Material(basis._chestMaterial);
		gorillaSkin._bodyMaterial = new Material(basis._bodyMaterial);
		gorillaSkin._scoreboardMaterial = new Material(basis._scoreboardMaterial);
		gorillaSkin._bodyMesh = basis.bodyMesh;
		return gorillaSkin;
	}

	// Token: 0x17000101 RID: 257
	// (get) Token: 0x06000A73 RID: 2675 RVA: 0x0003897D File Offset: 0x00036B7D
	public Material bodyMaterial
	{
		get
		{
			return this._bodyMaterial;
		}
	}

	// Token: 0x17000102 RID: 258
	// (get) Token: 0x06000A74 RID: 2676 RVA: 0x00038985 File Offset: 0x00036B85
	public Material chestMaterial
	{
		get
		{
			return this._chestMaterial;
		}
	}

	// Token: 0x17000103 RID: 259
	// (get) Token: 0x06000A75 RID: 2677 RVA: 0x0003898D File Offset: 0x00036B8D
	public Material scoreboardMaterial
	{
		get
		{
			return this._scoreboardMaterial;
		}
	}

	// Token: 0x06000A76 RID: 2678 RVA: 0x00038998 File Offset: 0x00036B98
	public static void ShowActiveSkin(VRRig rig)
	{
		bool useDefaultBodySkin;
		GorillaSkin activeSkin = GorillaSkin.GetActiveSkin(rig, out useDefaultBodySkin);
		GorillaSkin.ShowSkin(rig, activeSkin, useDefaultBodySkin);
	}

	// Token: 0x06000A77 RID: 2679 RVA: 0x000389B8 File Offset: 0x00036BB8
	public void ApplySkinToMannequin(GameObject mannequin)
	{
		SkinnedMeshRenderer skinnedMeshRenderer;
		if (mannequin.TryGetComponent<SkinnedMeshRenderer>(out skinnedMeshRenderer))
		{
			Material[] sharedMaterials = skinnedMeshRenderer.sharedMaterials;
			sharedMaterials[0] = this.bodyMaterial;
			sharedMaterials[1] = this.chestMaterial;
			skinnedMeshRenderer.sharedMaterials = sharedMaterials;
			return;
		}
		MeshRenderer meshRenderer;
		if (mannequin.TryGetComponent<MeshRenderer>(out meshRenderer))
		{
			Material[] sharedMaterials2 = meshRenderer.sharedMaterials;
			sharedMaterials2[0] = this.bodyMaterial;
			sharedMaterials2[1] = this.chestMaterial;
			meshRenderer.sharedMaterials = sharedMaterials2;
		}
	}

	// Token: 0x06000A78 RID: 2680 RVA: 0x00038A1C File Offset: 0x00036C1C
	public static GorillaSkin GetActiveSkin(VRRig rig, out bool useDefaultBodySkin)
	{
		if (rig.CurrentModeSkin.IsNotNull())
		{
			useDefaultBodySkin = false;
			return rig.CurrentModeSkin;
		}
		if (rig.TemporaryEffectSkin.IsNotNull())
		{
			useDefaultBodySkin = false;
			return rig.TemporaryEffectSkin;
		}
		if (rig.CurrentCosmeticSkin.IsNotNull())
		{
			useDefaultBodySkin = false;
			return rig.CurrentCosmeticSkin;
		}
		useDefaultBodySkin = true;
		return rig.defaultSkin;
	}

	// Token: 0x06000A79 RID: 2681 RVA: 0x00038A78 File Offset: 0x00036C78
	public static void ShowSkin(VRRig rig, GorillaSkin skin, bool useDefaultBodySkin = false)
	{
		if (skin.bodyMesh != null)
		{
			rig.mainSkin.sharedMesh = skin.bodyMesh;
		}
		if (useDefaultBodySkin)
		{
			rig.materialsToChangeTo[0] = rig.myDefaultSkinMaterialInstance;
		}
		else
		{
			rig.materialsToChangeTo[0] = skin.bodyMaterial;
		}
		rig.bodyRenderer.SetSkinMaterials(rig.materialsToChangeTo[rig.setMatIndex], skin.chestMaterial);
		rig.scoreboardMaterial = skin.scoreboardMaterial;
	}

	// Token: 0x06000A7A RID: 2682 RVA: 0x00038AF0 File Offset: 0x00036CF0
	public static void ApplyToRig(VRRig rig, GorillaSkin skin, GorillaSkin.SkinType type)
	{
		bool flag;
		GorillaSkin activeSkin = GorillaSkin.GetActiveSkin(rig, out flag);
		switch (type)
		{
		case GorillaSkin.SkinType.cosmetic:
			rig.CurrentCosmeticSkin = skin;
			break;
		case GorillaSkin.SkinType.gameMode:
			rig.CurrentModeSkin = skin;
			break;
		case GorillaSkin.SkinType.temporaryEffect:
			rig.TemporaryEffectSkin = skin;
			break;
		default:
			Debug.LogError("Unknown skin slot");
			break;
		}
		bool useDefaultBodySkin;
		GorillaSkin activeSkin2 = GorillaSkin.GetActiveSkin(rig, out useDefaultBodySkin);
		if (activeSkin != activeSkin2)
		{
			GorillaSkin.ShowSkin(rig, activeSkin2, useDefaultBodySkin);
		}
	}

	// Token: 0x04000CBD RID: 3261
	[FormerlySerializedAs("chestMaterial")]
	[FormerlySerializedAs("chestEarsMaterial")]
	[SerializeField]
	private Material _chestMaterial;

	// Token: 0x04000CBE RID: 3262
	[FormerlySerializedAs("bodyMaterial")]
	[SerializeField]
	private Material _bodyMaterial;

	// Token: 0x04000CBF RID: 3263
	[SerializeField]
	private Material _scoreboardMaterial;

	// Token: 0x04000CC0 RID: 3264
	[Space]
	[SerializeField]
	private Mesh _bodyMesh;

	// Token: 0x04000CC1 RID: 3265
	[Space]
	[NonSerialized]
	private Material _bodyRuntime;

	// Token: 0x04000CC2 RID: 3266
	[NonSerialized]
	private Material _chestRuntime;

	// Token: 0x04000CC3 RID: 3267
	[NonSerialized]
	private Material _scoreRuntime;

	// Token: 0x020001A1 RID: 417
	public enum SkinType
	{
		// Token: 0x04000CC5 RID: 3269
		cosmetic,
		// Token: 0x04000CC6 RID: 3270
		gameMode,
		// Token: 0x04000CC7 RID: 3271
		temporaryEffect
	}
}
