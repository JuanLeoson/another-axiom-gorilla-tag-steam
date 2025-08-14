using System;
using UnityEngine;

// Token: 0x02000872 RID: 2162
public static class GradientHelper
{
	// Token: 0x06003631 RID: 13873 RVA: 0x0011C63C File Offset: 0x0011A83C
	public static Gradient FromColor(Color color)
	{
		float a = color.a;
		Color col = color;
		col.a = 1f;
		return new Gradient
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(col, 1f)
			},
			alphaKeys = new GradientAlphaKey[]
			{
				new GradientAlphaKey(a, 1f)
			}
		};
	}
}
