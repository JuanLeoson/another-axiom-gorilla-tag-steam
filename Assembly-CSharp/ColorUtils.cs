using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000AA8 RID: 2728
public static class ColorUtils
{
	// Token: 0x060041FD RID: 16893 RVA: 0x0014C579 File Offset: 0x0014A779
	public static Color WithAlpha(this Color c, float alpha)
	{
		c.a = Math.Clamp(alpha, 0f, 1f);
		return c;
	}

	// Token: 0x060041FE RID: 16894 RVA: 0x0014C593 File Offset: 0x0014A793
	public static Color32 WithAlpha(this Color32 c, byte alpha)
	{
		c.a = alpha;
		return c;
	}

	// Token: 0x060041FF RID: 16895 RVA: 0x0014C5A0 File Offset: 0x0014A7A0
	public static Color ComposeHDR(Color baseColor, float intensity)
	{
		intensity = Mathf.Clamp(intensity, -10f, 10f);
		Color color = baseColor;
		if (baseColor.maxColorComponent > 1f)
		{
			color = ColorUtils.DecomposeHDR(baseColor).Item1;
		}
		float b = Mathf.Pow(2f, intensity);
		if (QualitySettings.activeColorSpace == ColorSpace.Linear)
		{
			b = Mathf.GammaToLinearSpace(intensity);
		}
		color *= b;
		color.a = baseColor.a;
		return color;
	}

	// Token: 0x06004200 RID: 16896 RVA: 0x0014C60C File Offset: 0x0014A80C
	[return: TupleElementNames(new string[]
	{
		"baseColor",
		"intensity"
	})]
	public static ValueTuple<Color, float> DecomposeHDR(Color hdrColor)
	{
		Color32 c = default(Color32);
		float item = 0f;
		float maxColorComponent = hdrColor.maxColorComponent;
		if (maxColorComponent == 0f || (maxColorComponent <= 1f && maxColorComponent >= 0.003921569f))
		{
			c.r = (byte)Mathf.RoundToInt(hdrColor.r * 255f);
			c.g = (byte)Mathf.RoundToInt(hdrColor.g * 255f);
			c.b = (byte)Mathf.RoundToInt(hdrColor.b * 255f);
		}
		else
		{
			float num = 191f / maxColorComponent;
			item = Mathf.Log(255f / num) / Mathf.Log(2f);
			c.r = Math.Min(191, (byte)Mathf.CeilToInt(num * hdrColor.r));
			c.g = Math.Min(191, (byte)Mathf.CeilToInt(num * hdrColor.g));
			c.b = Math.Min(191, (byte)Mathf.CeilToInt(num * hdrColor.b));
		}
		return new ValueTuple<Color, float>(c, item);
	}

	// Token: 0x04004D5D RID: 19805
	private const byte kMaxByteForOverexposedColor = 191;
}
