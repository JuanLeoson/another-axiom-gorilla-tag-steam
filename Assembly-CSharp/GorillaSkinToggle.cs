using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020001A4 RID: 420
public class GorillaSkinToggle : MonoBehaviour, ISpawnable
{
	// Token: 0x17000104 RID: 260
	// (get) Token: 0x06000A7D RID: 2685 RVA: 0x00038B6D File Offset: 0x00036D6D
	public bool applied
	{
		get
		{
			return this._applied;
		}
	}

	// Token: 0x17000105 RID: 261
	// (get) Token: 0x06000A7E RID: 2686 RVA: 0x00038B75 File Offset: 0x00036D75
	// (set) Token: 0x06000A7F RID: 2687 RVA: 0x00038B7D File Offset: 0x00036D7D
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000106 RID: 262
	// (get) Token: 0x06000A80 RID: 2688 RVA: 0x00038B86 File Offset: 0x00036D86
	// (set) Token: 0x06000A81 RID: 2689 RVA: 0x00038B8E File Offset: 0x00036D8E
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06000A82 RID: 2690 RVA: 0x00038B98 File Offset: 0x00036D98
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this._rig = base.GetComponentInParent<VRRig>(true);
		if (this.coloringRules.Length != 0)
		{
			this._activeSkin = GorillaSkin.CopyWithInstancedMaterials(this._skin);
			for (int i = 0; i < this.coloringRules.Length; i++)
			{
				this.coloringRules[i].Init();
			}
			return;
		}
		this._activeSkin = this._skin;
	}

	// Token: 0x06000A83 RID: 2691 RVA: 0x000023F5 File Offset: 0x000005F5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06000A84 RID: 2692 RVA: 0x00038C00 File Offset: 0x00036E00
	private void OnPlayerColorChanged(Color playerColor)
	{
		foreach (GorillaSkinToggle.ColoringRule coloringRule in this.coloringRules)
		{
			coloringRule.Apply(this._activeSkin, playerColor);
		}
	}

	// Token: 0x06000A85 RID: 2693 RVA: 0x00038C38 File Offset: 0x00036E38
	private void OnEnable()
	{
		if (this.coloringRules.Length != 0)
		{
			this._rig.OnColorChanged += this.OnPlayerColorChanged;
			this.OnPlayerColorChanged(this._rig.playerColor);
		}
		this.Apply();
	}

	// Token: 0x06000A86 RID: 2694 RVA: 0x00038C71 File Offset: 0x00036E71
	private void OnDisable()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.Remove();
		if (this.coloringRules.Length != 0)
		{
			this._rig.OnColorChanged -= this.OnPlayerColorChanged;
		}
	}

	// Token: 0x06000A87 RID: 2695 RVA: 0x00038CA1 File Offset: 0x00036EA1
	public void Apply()
	{
		GorillaSkin.ApplyToRig(this._rig, this._activeSkin, GorillaSkin.SkinType.cosmetic);
		this._applied = true;
	}

	// Token: 0x06000A88 RID: 2696 RVA: 0x00038CBC File Offset: 0x00036EBC
	public void ApplyToMannequin(GameObject mannequin)
	{
		if (this._skin.IsNull())
		{
			Debug.LogError("No skin set on GorillaSkinToggle");
			return;
		}
		if (mannequin.IsNull())
		{
			Debug.LogError("No mannequin set on GorillaSkinToggle");
			return;
		}
		this._skin.ApplySkinToMannequin(mannequin);
	}

	// Token: 0x06000A89 RID: 2697 RVA: 0x00038CF8 File Offset: 0x00036EF8
	public void Remove()
	{
		GorillaSkin.ApplyToRig(this._rig, null, GorillaSkin.SkinType.cosmetic);
		float @float = PlayerPrefs.GetFloat("redValue", 0f);
		float float2 = PlayerPrefs.GetFloat("greenValue", 0f);
		float float3 = PlayerPrefs.GetFloat("blueValue", 0f);
		GorillaTagger.Instance.UpdateColor(@float, float2, float3);
		this._applied = false;
	}

	// Token: 0x04000CCE RID: 3278
	private VRRig _rig;

	// Token: 0x04000CCF RID: 3279
	[SerializeField]
	private GorillaSkin _skin;

	// Token: 0x04000CD0 RID: 3280
	private GorillaSkin _activeSkin;

	// Token: 0x04000CD1 RID: 3281
	[SerializeField]
	private GorillaSkinToggle.ColoringRule[] coloringRules;

	// Token: 0x04000CD2 RID: 3282
	[Space]
	[SerializeField]
	private bool _applied;

	// Token: 0x020001A5 RID: 421
	[Serializable]
	private struct ColoringRule
	{
		// Token: 0x06000A8B RID: 2699 RVA: 0x00038D56 File Offset: 0x00036F56
		public void Init()
		{
			if (this.shaderColorProperty == "")
			{
				this.shaderColorProperty = "_BaseColor";
			}
			this.shaderHashId = new ShaderHashId(this.shaderColorProperty);
		}

		// Token: 0x06000A8C RID: 2700 RVA: 0x00038D88 File Offset: 0x00036F88
		public void Apply(GorillaSkin skin, Color color)
		{
			if (this.colorMaterials.HasFlag(GorillaSkinMaterials.Body))
			{
				skin.bodyMaterial.SetColor(this.shaderHashId, color);
			}
			if (this.colorMaterials.HasFlag(GorillaSkinMaterials.Chest))
			{
				skin.chestMaterial.SetColor(this.shaderHashId, color);
			}
			if (this.colorMaterials.HasFlag(GorillaSkinMaterials.Scoreboard))
			{
				skin.scoreboardMaterial.SetColor(this.shaderHashId, color);
			}
		}

		// Token: 0x04000CD5 RID: 3285
		public GorillaSkinMaterials colorMaterials;

		// Token: 0x04000CD6 RID: 3286
		public string shaderColorProperty;

		// Token: 0x04000CD7 RID: 3287
		private ShaderHashId shaderHashId;
	}
}
