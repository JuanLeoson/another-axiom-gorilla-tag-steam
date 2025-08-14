using System;
using UnityEngine;

// Token: 0x020000E0 RID: 224
public class BakeBlendShape : MonoBehaviour
{
	// Token: 0x0600059A RID: 1434 RVA: 0x00020998 File Offset: 0x0001EB98
	private void Update()
	{
		Mesh mesh = new Mesh();
		MeshCollider component = base.GetComponent<MeshCollider>();
		base.GetComponent<SkinnedMeshRenderer>().BakeMesh(mesh);
		component.sharedMesh = null;
		component.sharedMesh = mesh;
	}
}
