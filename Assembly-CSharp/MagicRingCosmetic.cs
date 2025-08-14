using System;
using UnityEngine;

// Token: 0x020000F6 RID: 246
public class MagicRingCosmetic : MonoBehaviour
{
	// Token: 0x0600062D RID: 1581 RVA: 0x00023BDF File Offset: 0x00021DDF
	protected void Awake()
	{
		this.materialPropertyBlock = new MaterialPropertyBlock();
		this.defaultEmissiveColor = this.ringRenderer.sharedMaterial.GetColor(ShaderProps._EmissionColor);
	}

	// Token: 0x0600062E RID: 1582 RVA: 0x00023C08 File Offset: 0x00021E08
	protected void LateUpdate()
	{
		float celsius = this.thermalReceiver.celsius;
		if (celsius >= this.fadeInTemperatureThreshold && this.fadeState != MagicRingCosmetic.FadeState.FadedIn)
		{
			this.fadeInSounds.Play();
			this.fadeState = MagicRingCosmetic.FadeState.FadedIn;
		}
		else if (celsius <= this.fadeOutTemperatureThreshold && this.fadeState != MagicRingCosmetic.FadeState.FadedOut)
		{
			this.fadeOutSounds.Play();
			this.fadeState = MagicRingCosmetic.FadeState.FadedOut;
		}
		this.emissiveAmount = Mathf.MoveTowards(this.emissiveAmount, (this.fadeState == MagicRingCosmetic.FadeState.FadedIn) ? 1f : 0f, Time.deltaTime / this.fadeTime);
		this.ringRenderer.GetPropertyBlock(this.materialPropertyBlock);
		this.materialPropertyBlock.SetColor(ShaderProps._EmissionColor, new Color(this.defaultEmissiveColor.r, this.defaultEmissiveColor.g, this.defaultEmissiveColor.b, this.emissiveAmount));
		this.ringRenderer.SetPropertyBlock(this.materialPropertyBlock);
	}

	// Token: 0x04000753 RID: 1875
	[Tooltip("The ring will fade in the emissive texture based on temperature from this ThermalReceiver.")]
	public ThermalReceiver thermalReceiver;

	// Token: 0x04000754 RID: 1876
	public Renderer ringRenderer;

	// Token: 0x04000755 RID: 1877
	public float fadeInTemperatureThreshold = 200f;

	// Token: 0x04000756 RID: 1878
	public float fadeOutTemperatureThreshold = 190f;

	// Token: 0x04000757 RID: 1879
	public float fadeTime = 1.5f;

	// Token: 0x04000758 RID: 1880
	public SoundBankPlayer fadeInSounds;

	// Token: 0x04000759 RID: 1881
	public SoundBankPlayer fadeOutSounds;

	// Token: 0x0400075A RID: 1882
	private MagicRingCosmetic.FadeState fadeState;

	// Token: 0x0400075B RID: 1883
	private Color defaultEmissiveColor;

	// Token: 0x0400075C RID: 1884
	private float emissiveAmount;

	// Token: 0x0400075D RID: 1885
	private MaterialPropertyBlock materialPropertyBlock;

	// Token: 0x020000F7 RID: 247
	private enum FadeState
	{
		// Token: 0x0400075F RID: 1887
		FadedOut,
		// Token: 0x04000760 RID: 1888
		FadedIn
	}
}
