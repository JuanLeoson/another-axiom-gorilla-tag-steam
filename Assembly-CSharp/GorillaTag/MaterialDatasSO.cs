using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000E63 RID: 3683
	[CreateAssetMenu(fileName = "MaterialDatasSO", menuName = "Gorilla Tag/MaterialDatasSO")]
	public class MaterialDatasSO : ScriptableObject
	{
		// Token: 0x040065DE RID: 26078
		public List<GTPlayer.MaterialData> datas;

		// Token: 0x040065DF RID: 26079
		public List<HashWrapper> surfaceEffects;
	}
}
