using System;
using GameObjectScheduling;
using UnityEngine;

// Token: 0x020002A3 RID: 675
public class MeshMaterialReplacer : MonoBehaviour
{
	// Token: 0x06000F92 RID: 3986 RVA: 0x0005B9CC File Offset: 0x00059BCC
	private void Start()
	{
		MeshRenderer meshRenderer;
		if (base.TryGetComponent<MeshRenderer>(out meshRenderer))
		{
			base.GetComponent<MeshFilter>().mesh = this.meshMaterialReplacement.mesh;
			meshRenderer.materials = this.meshMaterialReplacement.materials;
			return;
		}
		SkinnedMeshRenderer skinnedMeshRenderer;
		if (base.TryGetComponent<SkinnedMeshRenderer>(out skinnedMeshRenderer))
		{
			skinnedMeshRenderer.sharedMesh = this.meshMaterialReplacement.mesh;
			skinnedMeshRenderer.materials = this.meshMaterialReplacement.materials;
		}
	}

	// Token: 0x0400183A RID: 6202
	[SerializeField]
	private MeshMaterialReplacement meshMaterialReplacement;
}
