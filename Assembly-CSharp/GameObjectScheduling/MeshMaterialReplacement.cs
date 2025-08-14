using System;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x02000F9D RID: 3997
	[CreateAssetMenu(fileName = "New Mesh Material Replacement", menuName = "Game Object Scheduling/New Mesh Material Replacement", order = 1)]
	public class MeshMaterialReplacement : ScriptableObject
	{
		// Token: 0x04006EBA RID: 28346
		public Mesh mesh;

		// Token: 0x04006EBB RID: 28347
		public Material[] materials;
	}
}
