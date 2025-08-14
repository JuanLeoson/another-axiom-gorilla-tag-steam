using System;
using System.Collections.Generic;
using GorillaTag.Rendering.Shaders;
using UnityEngine;

// Token: 0x02000A9A RID: 2714
public class BetterBakerBakeMe : FlagForBaking
{
	// Token: 0x04004D37 RID: 19767
	public GameObject[] stuffIncludingParentsToBake;

	// Token: 0x04004D38 RID: 19768
	public GameObject getMatStuffFromHere;

	// Token: 0x04004D39 RID: 19769
	public List<ShaderConfigData.ShaderConfig> allConfigs = new List<ShaderConfigData.ShaderConfig>();
}
