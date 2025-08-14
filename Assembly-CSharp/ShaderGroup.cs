using System;
using UnityEngine;

// Token: 0x02000B4F RID: 2895
[Serializable]
public struct ShaderGroup
{
	// Token: 0x0600456B RID: 17771 RVA: 0x0015A874 File Offset: 0x00158A74
	public ShaderGroup(Material material, Shader original, Shader gameplay, Shader baking)
	{
		this.material = material;
		this.originalShader = original;
		this.gameplayShader = gameplay;
		this.bakingShader = baking;
	}

	// Token: 0x04005043 RID: 20547
	public Material material;

	// Token: 0x04005044 RID: 20548
	public Shader originalShader;

	// Token: 0x04005045 RID: 20549
	public Shader gameplayShader;

	// Token: 0x04005046 RID: 20550
	public Shader bakingShader;
}
