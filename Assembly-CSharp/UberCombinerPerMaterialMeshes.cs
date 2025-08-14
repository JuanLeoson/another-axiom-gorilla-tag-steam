using System;
using UnityEngine;

// Token: 0x02000B42 RID: 2882
public class UberCombinerPerMaterialMeshes : MonoBehaviour
{
	// Token: 0x04004FA4 RID: 20388
	public GameObject rootObject;

	// Token: 0x04004FA5 RID: 20389
	public bool deleteSelfOnPrefabBake;

	// Token: 0x04004FA6 RID: 20390
	[Space]
	public GameObject[] objects = new GameObject[0];

	// Token: 0x04004FA7 RID: 20391
	public MeshRenderer[] renderers = new MeshRenderer[0];

	// Token: 0x04004FA8 RID: 20392
	public MeshFilter[] filters = new MeshFilter[0];

	// Token: 0x04004FA9 RID: 20393
	public Material[] materials = new Material[0];
}
