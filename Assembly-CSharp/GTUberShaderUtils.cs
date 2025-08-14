using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000B4C RID: 2892
public static class GTUberShaderUtils
{
	// Token: 0x0600455E RID: 17758 RVA: 0x00159C92 File Offset: 0x00157E92
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SetStencilComparison(this Material m, GTShaderStencilCompare cmp)
	{
		m.SetFloat(GTUberShaderUtils._StencilComparison, (float)cmp);
	}

	// Token: 0x0600455F RID: 17759 RVA: 0x00159CA6 File Offset: 0x00157EA6
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SetStencilPassFrontOp(this Material m, GTShaderStencilOp op)
	{
		m.SetFloat(GTUberShaderUtils._StencilPassFront, (float)op);
	}

	// Token: 0x06004560 RID: 17760 RVA: 0x00159CBA File Offset: 0x00157EBA
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SetStencilReferenceValue(this Material m, int value)
	{
		m.SetFloat(GTUberShaderUtils._StencilReference, (float)value);
	}

	// Token: 0x06004561 RID: 17761 RVA: 0x00159CD0 File Offset: 0x00157ED0
	public static void SetVisibleToXRay(this Material m, bool visible, bool saveToDisk = false)
	{
		GTShaderStencilCompare cmp = visible ? GTShaderStencilCompare.Equal : GTShaderStencilCompare.NotEqual;
		GTShaderStencilOp op = visible ? GTShaderStencilOp.Replace : GTShaderStencilOp.Keep;
		m.SetStencilComparison(cmp);
		m.SetStencilPassFrontOp(op);
		m.SetStencilReferenceValue(7);
	}

	// Token: 0x06004562 RID: 17762 RVA: 0x00159D04 File Offset: 0x00157F04
	public static void SetRevealsXRay(this Material m, bool reveals, bool changeQueue = true, bool saveToDisk = false)
	{
		m.SetFloat(GTUberShaderUtils._ZWrite, (float)(reveals ? 0 : 1));
		m.SetFloat(GTUberShaderUtils._ColorMask_, (float)(reveals ? 0 : 14));
		m.SetStencilComparison(GTShaderStencilCompare.Disabled);
		m.SetStencilPassFrontOp(reveals ? GTShaderStencilOp.Replace : GTShaderStencilOp.Keep);
		m.SetStencilReferenceValue(reveals ? 7 : 0);
		if (changeQueue)
		{
			int renderQueue = m.renderQueue;
			m.renderQueue = renderQueue + (reveals ? -1 : 1);
		}
	}

	// Token: 0x06004563 RID: 17763 RVA: 0x00159D7C File Offset: 0x00157F7C
	public static int GetNearestRenderQueue(this Material m, out RenderQueue queue)
	{
		int renderQueue = m.renderQueue;
		int num = -1;
		int num2 = int.MaxValue;
		for (int i = 0; i < GTUberShaderUtils.kRenderQueueInts.Length; i++)
		{
			int num3 = GTUberShaderUtils.kRenderQueueInts[i];
			int num4 = Math.Abs(num3 - renderQueue);
			if (num2 > num4)
			{
				num = num3;
				num2 = num4;
			}
		}
		queue = (RenderQueue)num;
		return num;
	}

	// Token: 0x06004564 RID: 17764 RVA: 0x00159DCD File Offset: 0x00157FCD
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitOnLoad()
	{
		GTUberShaderUtils.kUberShader = Shader.Find("GorillaTag/UberShader");
	}

	// Token: 0x04004FEA RID: 20458
	private static Shader kUberShader;

	// Token: 0x04004FEB RID: 20459
	private static readonly ShaderHashId _StencilComparison = "_StencilComparison";

	// Token: 0x04004FEC RID: 20460
	private static readonly ShaderHashId _StencilPassFront = "_StencilPassFront";

	// Token: 0x04004FED RID: 20461
	private static readonly ShaderHashId _StencilReference = "_StencilReference";

	// Token: 0x04004FEE RID: 20462
	private static readonly ShaderHashId _ColorMask_ = "_ColorMask_";

	// Token: 0x04004FEF RID: 20463
	private static readonly ShaderHashId _ManualZWrite = "_ManualZWrite";

	// Token: 0x04004FF0 RID: 20464
	private static readonly ShaderHashId _ZWrite = "_ZWrite";

	// Token: 0x04004FF1 RID: 20465
	private static readonly int[] kRenderQueueInts = new int[]
	{
		1000,
		2000,
		2450,
		2500,
		3000,
		4000
	};
}
