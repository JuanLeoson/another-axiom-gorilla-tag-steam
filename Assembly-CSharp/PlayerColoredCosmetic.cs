using System;
using UnityEngine;

// Token: 0x020004A0 RID: 1184
public class PlayerColoredCosmetic : MonoBehaviour
{
	// Token: 0x06001D55 RID: 7509 RVA: 0x0009D514 File Offset: 0x0009B714
	public void Awake()
	{
		for (int i = 0; i < this.coloringRules.Length; i++)
		{
			this.coloringRules[i].Init();
		}
	}

	// Token: 0x06001D56 RID: 7510 RVA: 0x0009D548 File Offset: 0x0009B748
	private void OnEnable()
	{
		if (!this.didInit)
		{
			this.didInit = true;
			this.rig = base.GetComponentInParent<VRRig>();
			if (this.rig == null && GorillaTagger.Instance != null)
			{
				this.rig = GorillaTagger.Instance.offlineVRRig;
			}
		}
		if (this.rig != null)
		{
			this.rig.OnColorChanged += this.UpdateColor;
			this.UpdateColor(this.rig.playerColor);
		}
	}

	// Token: 0x06001D57 RID: 7511 RVA: 0x0009D5D1 File Offset: 0x0009B7D1
	private void OnDisable()
	{
		if (this.rig != null)
		{
			this.rig.OnColorChanged -= this.UpdateColor;
		}
	}

	// Token: 0x06001D58 RID: 7512 RVA: 0x0009D5F8 File Offset: 0x0009B7F8
	private void UpdateColor(Color color)
	{
		Color color2 = Color.Lerp(color, this.lerpToColor, this.lerpStrength);
		foreach (PlayerColoredCosmetic.ColoringRule coloringRule in this.coloringRules)
		{
			coloringRule.Apply(color2);
		}
	}

	// Token: 0x040025C9 RID: 9673
	private bool didInit;

	// Token: 0x040025CA RID: 9674
	private VRRig rig;

	// Token: 0x040025CB RID: 9675
	[SerializeField]
	private Color lerpToColor = Color.white;

	// Token: 0x040025CC RID: 9676
	[SerializeField]
	[Range(0f, 1f)]
	private float lerpStrength;

	// Token: 0x040025CD RID: 9677
	[SerializeField]
	private PlayerColoredCosmetic.ColoringRule[] coloringRules;

	// Token: 0x020004A1 RID: 1185
	[Serializable]
	private struct ColoringRule
	{
		// Token: 0x06001D5A RID: 7514 RVA: 0x0009D650 File Offset: 0x0009B850
		public void Init()
		{
			this.hashId = Shader.PropertyToID(this.shaderColorProperty);
			Material[] sharedMaterials = this.meshRenderer.sharedMaterials;
			this.instancedMaterial = new Material(sharedMaterials[this.materialIndex]);
			sharedMaterials[this.materialIndex] = this.instancedMaterial;
			this.meshRenderer.sharedMaterials = sharedMaterials;
		}

		// Token: 0x06001D5B RID: 7515 RVA: 0x0009D6A7 File Offset: 0x0009B8A7
		public void Apply(Color color)
		{
			this.instancedMaterial.SetColor(this.hashId, color);
		}

		// Token: 0x040025CE RID: 9678
		[SerializeField]
		private string shaderColorProperty;

		// Token: 0x040025CF RID: 9679
		private int hashId;

		// Token: 0x040025D0 RID: 9680
		[SerializeField]
		private Renderer meshRenderer;

		// Token: 0x040025D1 RID: 9681
		[SerializeField]
		private int materialIndex;

		// Token: 0x040025D2 RID: 9682
		private Material instancedMaterial;
	}
}
