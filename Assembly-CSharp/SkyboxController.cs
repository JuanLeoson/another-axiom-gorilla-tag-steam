using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020000C5 RID: 197
public class SkyboxController : MonoBehaviour
{
	// Token: 0x060004E7 RID: 1255 RVA: 0x0001C934 File Offset: 0x0001AB34
	private void Start()
	{
		if (this._dayNightManager.AsNull<BetterDayNightManager>() == null)
		{
			this._dayNightManager = BetterDayNightManager.instance;
		}
		if (this._dayNightManager.AsNull<BetterDayNightManager>() == null)
		{
			return;
		}
		for (int i = 0; i < this._dayNightManager.timeOfDayRange.Length; i++)
		{
			this._totalSecondsInRange += this._dayNightManager.timeOfDayRange[i] * 3600.0;
		}
		this._totalSecondsInRange = Math.Floor(this._totalSecondsInRange);
	}

	// Token: 0x060004E8 RID: 1256 RVA: 0x0001C9C2 File Offset: 0x0001ABC2
	private void Update()
	{
		if (!this.lastUpdate.HasElapsed(1f, true))
		{
			return;
		}
		this.UpdateTime();
		this.UpdateSky();
	}

	// Token: 0x060004E9 RID: 1257 RVA: 0x0001C9E4 File Offset: 0x0001ABE4
	private void OnValidate()
	{
		this.UpdateSky();
	}

	// Token: 0x060004EA RID: 1258 RVA: 0x0001C9EC File Offset: 0x0001ABEC
	private void UpdateTime()
	{
		this._currentSeconds = ((ITimeOfDaySystem)this._dayNightManager).currentTimeInSeconds;
		this._currentSeconds = Math.Floor(this._currentSeconds);
		this._currentTime = (float)(this._currentSeconds / this._totalSecondsInRange);
	}

	// Token: 0x060004EB RID: 1259 RVA: 0x0001CA24 File Offset: 0x0001AC24
	private void UpdateSky()
	{
		if (this.skyMaterials == null || this.skyMaterials.Length == 0)
		{
			return;
		}
		int num = this.skyMaterials.Length;
		float num2 = Mathf.Clamp(this._currentTime, 0f, 1f);
		float num3 = 1f / (float)num;
		int num4 = (int)(num2 / num3);
		float num5 = (num2 - (float)num4 * num3) / num3;
		this._currentSky = this.skyMaterials[num4];
		this._nextSky = this.skyMaterials[(num4 + 1) % num];
		this.skyFront.sharedMaterial = this._currentSky;
		this.skyBack.sharedMaterial = this._nextSky;
		if (this._currentSky.renderQueue != 3000)
		{
			this.SetFrontToTransparent();
		}
		if (this._nextSky.renderQueue == 3000)
		{
			this.SetBackToOpaque();
		}
		this._currentSky.SetFloat(ShaderProps._SkyAlpha, 1f - num5);
	}

	// Token: 0x060004EC RID: 1260 RVA: 0x0001CB00 File Offset: 0x0001AD00
	private void SetFrontToTransparent()
	{
		bool flag = false;
		bool flag2 = false;
		string val = "Transparent";
		int renderQueue = 3000;
		BlendMode blendMode = BlendMode.SrcAlpha;
		BlendMode blendMode2 = BlendMode.OneMinusSrcAlpha;
		BlendMode blendMode3 = BlendMode.One;
		BlendMode blendMode4 = BlendMode.OneMinusSrcAlpha;
		Material sharedMaterial = this.skyFront.sharedMaterial;
		sharedMaterial.SetFloat(ShaderProps._ZWrite, flag ? 1f : 0f);
		sharedMaterial.SetShaderPassEnabled("DepthOnly", flag);
		sharedMaterial.SetFloat(ShaderProps._AlphaToMask, flag2 ? 1f : 0f);
		sharedMaterial.SetOverrideTag("RenderType", val);
		sharedMaterial.renderQueue = renderQueue;
		sharedMaterial.SetFloat(ShaderProps._SrcBlend, (float)blendMode);
		sharedMaterial.SetFloat(ShaderProps._DstBlend, (float)blendMode2);
		sharedMaterial.SetFloat(ShaderProps._SrcBlendAlpha, (float)blendMode3);
		sharedMaterial.SetFloat(ShaderProps._DstBlendAlpha, (float)blendMode4);
	}

	// Token: 0x060004ED RID: 1261 RVA: 0x0001CBC0 File Offset: 0x0001ADC0
	private void SetFrontToOpaque()
	{
		bool flag = false;
		bool flag2 = true;
		string val = "Opaque";
		int renderQueue = 2000;
		BlendMode blendMode = BlendMode.One;
		BlendMode blendMode2 = BlendMode.Zero;
		BlendMode blendMode3 = BlendMode.One;
		BlendMode blendMode4 = BlendMode.Zero;
		Material sharedMaterial = this.skyFront.sharedMaterial;
		sharedMaterial.SetFloat(ShaderProps._ZWrite, flag2 ? 1f : 0f);
		sharedMaterial.SetShaderPassEnabled("DepthOnly", flag2);
		sharedMaterial.SetFloat(ShaderProps._AlphaToMask, flag ? 1f : 0f);
		sharedMaterial.SetOverrideTag("RenderType", val);
		sharedMaterial.renderQueue = renderQueue;
		sharedMaterial.SetFloat(ShaderProps._SrcBlend, (float)blendMode);
		sharedMaterial.SetFloat(ShaderProps._DstBlend, (float)blendMode2);
		sharedMaterial.SetFloat(ShaderProps._SrcBlendAlpha, (float)blendMode3);
		sharedMaterial.SetFloat(ShaderProps._DstBlendAlpha, (float)blendMode4);
	}

	// Token: 0x060004EE RID: 1262 RVA: 0x0001CC80 File Offset: 0x0001AE80
	private void SetBackToOpaque()
	{
		bool flag = false;
		bool flag2 = true;
		string val = "Opaque";
		int renderQueue = 2000;
		BlendMode blendMode = BlendMode.One;
		BlendMode blendMode2 = BlendMode.Zero;
		BlendMode blendMode3 = BlendMode.One;
		BlendMode blendMode4 = BlendMode.Zero;
		Material sharedMaterial = this.skyBack.sharedMaterial;
		sharedMaterial.SetFloat(ShaderProps._ZWrite, flag2 ? 1f : 0f);
		sharedMaterial.SetShaderPassEnabled("DepthOnly", flag2);
		sharedMaterial.SetFloat(ShaderProps._AlphaToMask, flag ? 1f : 0f);
		sharedMaterial.SetOverrideTag("RenderType", val);
		sharedMaterial.renderQueue = renderQueue;
		sharedMaterial.SetFloat(ShaderProps._SrcBlend, (float)blendMode);
		sharedMaterial.SetFloat(ShaderProps._DstBlend, (float)blendMode2);
		sharedMaterial.SetFloat(ShaderProps._SrcBlendAlpha, (float)blendMode3);
		sharedMaterial.SetFloat(ShaderProps._DstBlendAlpha, (float)blendMode4);
	}

	// Token: 0x040005D2 RID: 1490
	public MeshRenderer skyFront;

	// Token: 0x040005D3 RID: 1491
	public MeshRenderer skyBack;

	// Token: 0x040005D4 RID: 1492
	public Material[] skyMaterials = new Material[0];

	// Token: 0x040005D5 RID: 1493
	[Range(0f, 1f)]
	public float lerpValue;

	// Token: 0x040005D6 RID: 1494
	[NonSerialized]
	private Material _currentSky;

	// Token: 0x040005D7 RID: 1495
	[NonSerialized]
	private Material _nextSky;

	// Token: 0x040005D8 RID: 1496
	private TimeSince lastUpdate = TimeSince.Now();

	// Token: 0x040005D9 RID: 1497
	[Space]
	private BetterDayNightManager _dayNightManager;

	// Token: 0x040005DA RID: 1498
	private double _currentSeconds = -1.0;

	// Token: 0x040005DB RID: 1499
	private double _totalSecondsInRange = -1.0;

	// Token: 0x040005DC RID: 1500
	private float _currentTime = -1f;
}
