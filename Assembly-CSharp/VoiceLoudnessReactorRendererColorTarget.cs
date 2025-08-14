using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000B52 RID: 2898
[Serializable]
public class VoiceLoudnessReactorRendererColorTarget
{
	// Token: 0x06004575 RID: 17781 RVA: 0x0015B268 File Offset: 0x00159468
	public void Inititialize()
	{
		if (this._materials == null)
		{
			this._materials = new List<Material>(this.renderer.materials);
			this._materials[this.materialIndex].EnableKeyword(this.colorProperty);
			this.renderer.SetMaterials(this._materials);
			this.UpdateMaterialColor(0f);
		}
	}

	// Token: 0x06004576 RID: 17782 RVA: 0x0015B2CC File Offset: 0x001594CC
	public void UpdateMaterialColor(float level)
	{
		Color color = this.gradient.Evaluate(level);
		if (this._lastColor == color)
		{
			return;
		}
		this._materials[this.materialIndex].SetColor(this.colorProperty, color);
		this._lastColor = color;
	}

	// Token: 0x0400505A RID: 20570
	[SerializeField]
	private string colorProperty = "_BaseColor";

	// Token: 0x0400505B RID: 20571
	public Renderer renderer;

	// Token: 0x0400505C RID: 20572
	public int materialIndex;

	// Token: 0x0400505D RID: 20573
	public Gradient gradient;

	// Token: 0x0400505E RID: 20574
	public bool useSmoothedLoudness;

	// Token: 0x0400505F RID: 20575
	public float scale = 1f;

	// Token: 0x04005060 RID: 20576
	private List<Material> _materials;

	// Token: 0x04005061 RID: 20577
	private Color _lastColor = Color.white;
}
