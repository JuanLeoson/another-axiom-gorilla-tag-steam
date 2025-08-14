using System;
using UnityEngine;

// Token: 0x0200077C RID: 1916
public class TapInnerGlow : MonoBehaviour
{
	// Token: 0x17000478 RID: 1144
	// (get) Token: 0x06003008 RID: 12296 RVA: 0x000FC958 File Offset: 0x000FAB58
	private Material targetMaterial
	{
		get
		{
			if (this._instance.AsNull<Material>() == null)
			{
				return this._instance = this._renderer.material;
			}
			return this._instance;
		}
	}

	// Token: 0x06003009 RID: 12297 RVA: 0x000FC994 File Offset: 0x000FAB94
	public void Tap()
	{
		if (!this._renderer)
		{
			return;
		}
		Material targetMaterial = this.targetMaterial;
		float value = this.tapLength;
		float time = GTShaderGlobals.Time;
		UberShader.InnerGlowSinePeriod.SetValue<float>(targetMaterial, value);
		UberShader.InnerGlowSinePhaseShift.SetValue<float>(targetMaterial, time);
	}

	// Token: 0x04003C1C RID: 15388
	public Renderer _renderer;

	// Token: 0x04003C1D RID: 15389
	public float tapLength = 1f;

	// Token: 0x04003C1E RID: 15390
	[Space]
	private Material _instance;
}
