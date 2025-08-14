using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000655 RID: 1621
public class GRProgressionScriptableObject : ScriptableObject
{
	// Token: 0x04003325 RID: 13093
	[SerializeField]
	[Header("Progression Tiers")]
	public List<GRPlayer.ProgressionLevels> progressionData;
}
