using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005E4 RID: 1508
[CreateAssetMenu(fileName = "GhostReactorSpawnConfig", menuName = "ScriptableObjects/GhostReactorSpawnConfig")]
public class GhostReactorSpawnConfig : ScriptableObject
{
	// Token: 0x04002EDF RID: 11999
	public List<GhostReactorSpawnConfig.EntitySpawnGroup> entitySpawnGroups;

	// Token: 0x020005E5 RID: 1509
	public enum SpawnPointType
	{
		// Token: 0x04002EE1 RID: 12001
		Enemy,
		// Token: 0x04002EE2 RID: 12002
		Collectible,
		// Token: 0x04002EE3 RID: 12003
		Barrier,
		// Token: 0x04002EE4 RID: 12004
		HazardLiquid,
		// Token: 0x04002EE5 RID: 12005
		Phantom,
		// Token: 0x04002EE6 RID: 12006
		Pest,
		// Token: 0x04002EE7 RID: 12007
		Crate,
		// Token: 0x04002EE8 RID: 12008
		Tool,
		// Token: 0x04002EE9 RID: 12009
		SpawnPointTypeCount
	}

	// Token: 0x020005E6 RID: 1510
	[Serializable]
	public struct EntitySpawnGroup
	{
		// Token: 0x04002EEA RID: 12010
		public GhostReactorSpawnConfig.SpawnPointType spawnPointType;

		// Token: 0x04002EEB RID: 12011
		public GameEntity entity;

		// Token: 0x04002EEC RID: 12012
		public GRBreakableItemSpawnConfig randomEntity;

		// Token: 0x04002EED RID: 12013
		public int spawnCount;
	}
}
