using System;
using UnityEngine;

// Token: 0x02000768 RID: 1896
public class MagicCauldronLiquid : MonoBehaviour
{
	// Token: 0x06002F7B RID: 12155 RVA: 0x000FAB6B File Offset: 0x000F8D6B
	private void Test()
	{
		this._animProgress = 0f;
		this._animating = true;
		base.enabled = true;
	}

	// Token: 0x06002F7C RID: 12156 RVA: 0x000FAB86 File Offset: 0x000F8D86
	public void AnimateColorFromTo(Color a, Color b, float length = 1f)
	{
		this._colorStart = a;
		this._colorEnd = b;
		this._animProgress = 0f;
		this._animating = true;
		this.animLength = length;
		base.enabled = true;
	}

	// Token: 0x06002F7D RID: 12157 RVA: 0x000FABB6 File Offset: 0x000F8DB6
	private void ApplyColor(Color color)
	{
		if (!this._applyMaterial)
		{
			return;
		}
		this._applyMaterial.SetColor(ShaderProps._BaseColor, color);
		this._applyMaterial.Apply();
	}

	// Token: 0x06002F7E RID: 12158 RVA: 0x000FABE4 File Offset: 0x000F8DE4
	private void ApplyWaveParams(float amplitude, float frequency, float scale, float rotation)
	{
		if (!this._applyMaterial)
		{
			return;
		}
		this._applyMaterial.SetFloat(ShaderProps._WaveAmplitude, amplitude);
		this._applyMaterial.SetFloat(ShaderProps._WaveFrequency, frequency);
		this._applyMaterial.SetFloat(ShaderProps._WaveScale, scale);
		this._applyMaterial.Apply();
	}

	// Token: 0x06002F7F RID: 12159 RVA: 0x000FAC3D File Offset: 0x000F8E3D
	private void OnEnable()
	{
		if (this._applyMaterial)
		{
			this._applyMaterial.mode = ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock;
		}
	}

	// Token: 0x06002F80 RID: 12160 RVA: 0x000FAC58 File Offset: 0x000F8E58
	private void OnDisable()
	{
		this._animating = false;
		this._animProgress = 0f;
	}

	// Token: 0x06002F81 RID: 12161 RVA: 0x000FAC6C File Offset: 0x000F8E6C
	private void Update()
	{
		if (!this._animating)
		{
			return;
		}
		float num = this._animationCurve.Evaluate(this._animProgress / this.animLength);
		float t = this._waveCurve.Evaluate(this._animProgress / this.animLength);
		if (num >= 1f)
		{
			this.ApplyColor(this._colorEnd);
			this._animating = false;
			base.enabled = false;
			return;
		}
		Color color = Color.Lerp(this._colorStart, this._colorEnd, num);
		Mathf.Lerp(this.waveNormal.frequency, this.waveAnimating.frequency, t);
		Mathf.Lerp(this.waveNormal.amplitude, this.waveAnimating.amplitude, t);
		Mathf.Lerp(this.waveNormal.scale, this.waveAnimating.scale, t);
		Mathf.Lerp(this.waveNormal.rotation, this.waveAnimating.rotation, t);
		this.ApplyColor(color);
		this._animProgress += Time.deltaTime;
	}

	// Token: 0x04003B7B RID: 15227
	[SerializeField]
	private ApplyMaterialProperty _applyMaterial;

	// Token: 0x04003B7C RID: 15228
	[SerializeField]
	private Color _colorStart;

	// Token: 0x04003B7D RID: 15229
	[SerializeField]
	private Color _colorEnd;

	// Token: 0x04003B7E RID: 15230
	[SerializeField]
	private bool _animating;

	// Token: 0x04003B7F RID: 15231
	[SerializeField]
	private float _animProgress;

	// Token: 0x04003B80 RID: 15232
	[SerializeField]
	private AnimationCurve _animationCurve = AnimationCurves.EaseOutCubic;

	// Token: 0x04003B81 RID: 15233
	[SerializeField]
	private AnimationCurve _waveCurve = AnimationCurves.EaseInElastic;

	// Token: 0x04003B82 RID: 15234
	public float animLength = 1f;

	// Token: 0x04003B83 RID: 15235
	public MagicCauldronLiquid.WaveParams waveNormal;

	// Token: 0x04003B84 RID: 15236
	public MagicCauldronLiquid.WaveParams waveAnimating;

	// Token: 0x02000769 RID: 1897
	[Serializable]
	public struct WaveParams
	{
		// Token: 0x04003B85 RID: 15237
		public float amplitude;

		// Token: 0x04003B86 RID: 15238
		public float frequency;

		// Token: 0x04003B87 RID: 15239
		public float scale;

		// Token: 0x04003B88 RID: 15240
		public float rotation;
	}
}
