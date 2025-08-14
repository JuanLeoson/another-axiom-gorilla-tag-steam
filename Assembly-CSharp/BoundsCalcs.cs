using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000B5A RID: 2906
public class BoundsCalcs : MonoBehaviour
{
	// Token: 0x0600458C RID: 17804 RVA: 0x0015B4B4 File Offset: 0x001596B4
	public void Compute()
	{
		MeshFilter[] array;
		if (this.useRootMeshOnly)
		{
			BoundsCalcs.singleMesh[0] = base.GetComponent<MeshFilter>();
			array = BoundsCalcs.singleMesh;
		}
		else if (this.optionalTargets != null && this.optionalTargets.Length != 0)
		{
			array = base.GetComponentsInChildren<MeshFilter>().Concat(this.optionalTargets).ToArray<MeshFilter>();
		}
		else
		{
			array = base.GetComponentsInChildren<MeshFilter>();
		}
		List<Mesh> list = new List<Mesh>((array.Length + 1) / 2);
		List<Vector3> list2 = new List<Vector3>(array.Length * 512);
		this.elements.Clear();
		for (int i = 0; i < array.Length; i++)
		{
			Matrix4x4 localToWorldMatrix = array[i].transform.localToWorldMatrix;
			Mesh mesh = array[i].sharedMesh;
			if (!mesh.isReadable)
			{
				Mesh mesh2 = mesh.CreateReadableMeshCopy();
				list.Add(mesh2);
				mesh = mesh2;
			}
			Vector3[] vertices = mesh.vertices;
			for (int j = 0; j < vertices.Length; j++)
			{
				vertices[j] = localToWorldMatrix.MultiplyPoint3x4(vertices[j]);
			}
			BoundsInfo item = BoundsInfo.ComputeBounds(vertices);
			this.elements.Add(item);
			list2.AddRange(vertices);
		}
		this.composite = BoundsInfo.ComputeBounds(list2.ToArray());
		list.ForEach(new Action<Mesh>(Object.DestroyImmediate));
	}

	// Token: 0x04005086 RID: 20614
	public MeshFilter[] optionalTargets = new MeshFilter[0];

	// Token: 0x04005087 RID: 20615
	public bool useRootMeshOnly;

	// Token: 0x04005088 RID: 20616
	[Space]
	public List<BoundsInfo> elements = new List<BoundsInfo>();

	// Token: 0x04005089 RID: 20617
	[Space]
	public BoundsInfo composite;

	// Token: 0x0400508A RID: 20618
	[Space]
	private StateHash _state;

	// Token: 0x0400508B RID: 20619
	private static MeshFilter[] singleMesh = new MeshFilter[1];
}
