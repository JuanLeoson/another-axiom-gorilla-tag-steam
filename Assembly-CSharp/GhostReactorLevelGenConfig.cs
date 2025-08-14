using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005CD RID: 1485
[CreateAssetMenu(fileName = "GhostReactorLevelGenConfig", menuName = "ScriptableObjects/GhostReactorLevelGenConfig")]
public class GhostReactorLevelGenConfig : ScriptableObject
{
	// Token: 0x04002DF4 RID: 11764
	public int shiftDuration;

	// Token: 0x04002DF5 RID: 11765
	public int coresRequired;

	// Token: 0x04002DF6 RID: 11766
	public int sentientCoresRequired;

	// Token: 0x04002DF7 RID: 11767
	public int maxPlayerDeaths = -1;

	// Token: 0x04002DF8 RID: 11768
	[ColorUsage(true, true)]
	public Color ambientLight = Color.black;

	// Token: 0x04002DF9 RID: 11769
	public List<GhostReactorLevelGeneratorV2.TreeLevelConfig> treeLevels = new List<GhostReactorLevelGeneratorV2.TreeLevelConfig>();

	// Token: 0x04002DFA RID: 11770
	public List<GRBonusEntry> enemyGlobalBonuses = new List<GRBonusEntry>();
}
