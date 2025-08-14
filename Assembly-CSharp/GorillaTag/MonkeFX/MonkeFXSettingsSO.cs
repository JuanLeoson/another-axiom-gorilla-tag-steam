using System;
using UnityEngine;

namespace GorillaTag.MonkeFX
{
	// Token: 0x02000EB0 RID: 3760
	[CreateAssetMenu(fileName = "MeshGenerator", menuName = "ScriptableObjects/MeshGenerator", order = 1)]
	public class MonkeFXSettingsSO : ScriptableObject
	{
		// Token: 0x06005DF7 RID: 24055 RVA: 0x001DA4BE File Offset: 0x001D86BE
		protected void Awake()
		{
			MonkeFX.Register(this);
		}

		// Token: 0x040067F0 RID: 26608
		public GTDirectAssetRef<Mesh>[] sourceMeshes;

		// Token: 0x040067F1 RID: 26609
		[HideInInspector]
		public Mesh combinedMesh;
	}
}
