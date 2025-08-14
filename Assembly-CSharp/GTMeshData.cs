using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000B3D RID: 2877
public class GTMeshData
{
	// Token: 0x06004536 RID: 17718 RVA: 0x00158FB4 File Offset: 0x001571B4
	public GTMeshData(Mesh m)
	{
		this.mesh = m;
		this.subMeshCount = m.subMeshCount;
		this.vertices = m.vertices;
		this.triangles = m.triangles;
		this.normals = m.normals;
		this.tangents = m.tangents;
		this.colors32 = m.colors32;
		this.boneWeights = m.boneWeights;
		this.uv = m.uv;
		this.uv2 = m.uv2;
		this.uv3 = m.uv3;
		this.uv4 = m.uv4;
		this.uv5 = m.uv5;
		this.uv6 = m.uv6;
		this.uv7 = m.uv7;
		this.uv8 = m.uv8;
	}

	// Token: 0x06004537 RID: 17719 RVA: 0x00159084 File Offset: 0x00157284
	public Mesh ExtractSubmesh(int subMeshIndex, bool optimize = false)
	{
		if (subMeshIndex < 0 || subMeshIndex >= this.subMeshCount)
		{
			throw new IndexOutOfRangeException("subMeshIndex");
		}
		SubMeshDescriptor subMesh = this.mesh.GetSubMesh(subMeshIndex);
		int firstVertex = subMesh.firstVertex;
		int vertexCount = subMesh.vertexCount;
		MeshTopology topology = subMesh.topology;
		int[] indices = this.mesh.GetIndices(subMeshIndex, false);
		for (int i = 0; i < indices.Length; i++)
		{
			indices[i] -= firstVertex;
		}
		Mesh mesh = new Mesh();
		mesh.indexFormat = ((vertexCount > 65535) ? IndexFormat.UInt32 : IndexFormat.UInt16);
		mesh.SetVertices(this.vertices, firstVertex, vertexCount);
		mesh.SetIndices(indices, topology, 0);
		mesh.SetNormals(this.normals, firstVertex, vertexCount);
		mesh.SetTangents(this.tangents, firstVertex, vertexCount);
		if (!this.uv.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(0, this.uv, firstVertex, vertexCount);
		}
		if (!this.uv2.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(1, this.uv2, firstVertex, vertexCount);
		}
		if (!this.uv3.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(2, this.uv3, firstVertex, vertexCount);
		}
		if (!this.uv4.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(3, this.uv4, firstVertex, vertexCount);
		}
		if (!this.uv5.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(4, this.uv5, firstVertex, vertexCount);
		}
		if (!this.uv6.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(5, this.uv6, firstVertex, vertexCount);
		}
		if (!this.uv7.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(6, this.uv7, firstVertex, vertexCount);
		}
		if (!this.uv8.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(7, this.uv8, firstVertex, vertexCount);
		}
		if (optimize)
		{
			mesh.Optimize();
			mesh.OptimizeIndexBuffers();
		}
		mesh.RecalculateBounds();
		return mesh;
	}

	// Token: 0x06004538 RID: 17720 RVA: 0x00159252 File Offset: 0x00157452
	public static GTMeshData Parse(Mesh mesh)
	{
		if (mesh == null)
		{
			throw new ArgumentNullException("mesh");
		}
		return new GTMeshData(mesh);
	}

	// Token: 0x04004F6F RID: 20335
	public Mesh mesh;

	// Token: 0x04004F70 RID: 20336
	public Vector3[] vertices;

	// Token: 0x04004F71 RID: 20337
	public Vector3[] normals;

	// Token: 0x04004F72 RID: 20338
	public Vector4[] tangents;

	// Token: 0x04004F73 RID: 20339
	public Color32[] colors32;

	// Token: 0x04004F74 RID: 20340
	public int[] triangles;

	// Token: 0x04004F75 RID: 20341
	public BoneWeight[] boneWeights;

	// Token: 0x04004F76 RID: 20342
	public Vector2[] uv;

	// Token: 0x04004F77 RID: 20343
	public Vector2[] uv2;

	// Token: 0x04004F78 RID: 20344
	public Vector2[] uv3;

	// Token: 0x04004F79 RID: 20345
	public Vector2[] uv4;

	// Token: 0x04004F7A RID: 20346
	public Vector2[] uv5;

	// Token: 0x04004F7B RID: 20347
	public Vector2[] uv6;

	// Token: 0x04004F7C RID: 20348
	public Vector2[] uv7;

	// Token: 0x04004F7D RID: 20349
	public Vector2[] uv8;

	// Token: 0x04004F7E RID: 20350
	public int subMeshCount;
}
