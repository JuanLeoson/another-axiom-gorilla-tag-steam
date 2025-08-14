using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005CC RID: 1484
[CreateAssetMenu(fileName = "GhostReactorLevelDepthConfig", menuName = "ScriptableObjects/GhostReactorLevelDepthConfig")]
public class GhostReactorLevelDepthConfig : ScriptableObject
{
	// Token: 0x04002DF2 RID: 11762
	public string displayName;

	// Token: 0x04002DF3 RID: 11763
	public List<GhostReactorLevelGenConfig> configGenOptions = new List<GhostReactorLevelGenConfig>();
}
