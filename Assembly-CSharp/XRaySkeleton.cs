using System;
using UnityEngine;

// Token: 0x020004B5 RID: 1205
public class XRaySkeleton : SyncToPlayerColor
{
	// Token: 0x06001DD3 RID: 7635 RVA: 0x0009FB10 File Offset: 0x0009DD10
	protected override void Awake()
	{
		base.Awake();
		this.target = this.renderer.material;
		Material[] materialsToChangeTo = this.rig.materialsToChangeTo;
		this.tagMaterials = new Material[materialsToChangeTo.Length];
		this.tagMaterials[0] = new Material(this.target);
		for (int i = 1; i < materialsToChangeTo.Length; i++)
		{
			Material material = new Material(materialsToChangeTo[i]);
			this.tagMaterials[i] = material;
		}
	}

	// Token: 0x06001DD4 RID: 7636 RVA: 0x0009FB81 File Offset: 0x0009DD81
	public void SetMaterialIndex(int index)
	{
		this.renderer.sharedMaterial = this.tagMaterials[index];
		this._lastMatIndex = index;
	}

	// Token: 0x06001DD5 RID: 7637 RVA: 0x0009FB9D File Offset: 0x0009DD9D
	private void Setup()
	{
		this.colorPropertiesToSync = new ShaderHashId[]
		{
			XRaySkeleton._BaseColor,
			XRaySkeleton._EmissionColor
		};
	}

	// Token: 0x06001DD6 RID: 7638 RVA: 0x0009FBC4 File Offset: 0x0009DDC4
	public override void UpdateColor(Color color)
	{
		if (this._lastMatIndex != 0)
		{
			return;
		}
		Material material = this.tagMaterials[0];
		float h;
		float s;
		float value;
		Color.RGBToHSV(color, out h, out s, out value);
		Color value2 = Color.HSVToRGB(h, s, Mathf.Clamp(value, this.baseValueMinMax.x, this.baseValueMinMax.y));
		material.SetColor(XRaySkeleton._BaseColor, value2);
		float h2;
		float num;
		float num2;
		Color.RGBToHSV(color, out h2, out num, out num2);
		Color color2 = Color.HSVToRGB(h2, 0.82f, 0.9f, true);
		color2 = new Color(color2.r * 1.4f, color2.g * 1.4f, color2.b * 1.4f);
		material.SetColor(XRaySkeleton._EmissionColor, ColorUtils.ComposeHDR(new Color32(36, 191, 136, byte.MaxValue), 2f));
		this.renderer.sharedMaterial = material;
	}

	// Token: 0x04002666 RID: 9830
	public SkinnedMeshRenderer renderer;

	// Token: 0x04002667 RID: 9831
	public Vector2 baseValueMinMax = new Vector2(0.69f, 1f);

	// Token: 0x04002668 RID: 9832
	public Material[] tagMaterials = new Material[0];

	// Token: 0x04002669 RID: 9833
	private int _lastMatIndex;

	// Token: 0x0400266A RID: 9834
	private static readonly ShaderHashId _BaseColor = "_BaseColor";

	// Token: 0x0400266B RID: 9835
	private static readonly ShaderHashId _EmissionColor = "_EmissionColor";
}
