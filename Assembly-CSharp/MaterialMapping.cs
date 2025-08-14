using System;
using UnityEngine;

// Token: 0x02000B4E RID: 2894
public class MaterialMapping : ScriptableObject
{
	// Token: 0x06004568 RID: 17768 RVA: 0x000023F5 File Offset: 0x000005F5
	public void CleanUpData()
	{
	}

	// Token: 0x0400503D RID: 20541
	private static string path = "Assets/UberShaderConversion/MaterialMap.asset";

	// Token: 0x0400503E RID: 20542
	public static string materialDirectory = "Assets/UberShaderConversion/Materials/";

	// Token: 0x0400503F RID: 20543
	private static MaterialMapping instance;

	// Token: 0x04005040 RID: 20544
	public ShaderGroup[] map;

	// Token: 0x04005041 RID: 20545
	public Material mirrorMat;

	// Token: 0x04005042 RID: 20546
	public RenderTexture mirrorTexture;
}
