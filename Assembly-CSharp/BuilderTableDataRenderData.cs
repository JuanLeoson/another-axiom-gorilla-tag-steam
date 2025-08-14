using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

// Token: 0x02000545 RID: 1349
public class BuilderTableDataRenderData
{
	// Token: 0x04002A0F RID: 10767
	public const int NUM_SPLIT_MESH_INSTANCE_GROUPS = 1;

	// Token: 0x04002A10 RID: 10768
	public int texWidth;

	// Token: 0x04002A11 RID: 10769
	public int texHeight;

	// Token: 0x04002A12 RID: 10770
	public TextureFormat textureFormat;

	// Token: 0x04002A13 RID: 10771
	public Dictionary<Material, int> materialToIndex;

	// Token: 0x04002A14 RID: 10772
	public List<Material> materials;

	// Token: 0x04002A15 RID: 10773
	public Material sharedMaterial;

	// Token: 0x04002A16 RID: 10774
	public Material sharedMaterialIndirect;

	// Token: 0x04002A17 RID: 10775
	public Dictionary<Texture2D, int> textureToIndex;

	// Token: 0x04002A18 RID: 10776
	public List<Texture2D> textures;

	// Token: 0x04002A19 RID: 10777
	public List<Material> perTextureMaterial;

	// Token: 0x04002A1A RID: 10778
	public List<MaterialPropertyBlock> perTexturePropertyBlock;

	// Token: 0x04002A1B RID: 10779
	public Texture2DArray sharedTexArray;

	// Token: 0x04002A1C RID: 10780
	public Dictionary<Mesh, int> meshToIndex;

	// Token: 0x04002A1D RID: 10781
	public List<Mesh> meshes;

	// Token: 0x04002A1E RID: 10782
	public List<int> meshInstanceCount;

	// Token: 0x04002A1F RID: 10783
	public NativeList<BuilderTableSubMesh> subMeshes;

	// Token: 0x04002A20 RID: 10784
	public Mesh sharedMesh;

	// Token: 0x04002A21 RID: 10785
	public BuilderTableDataRenderIndirectBatch dynamicBatch;

	// Token: 0x04002A22 RID: 10786
	public BuilderTableDataRenderIndirectBatch staticBatch;

	// Token: 0x04002A23 RID: 10787
	public JobHandle setupInstancesJobs;
}
