using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000239 RID: 569
public static class GTVertexDataStreams_Descriptors
{
	// Token: 0x06000D45 RID: 3397 RVA: 0x0004EF08 File Offset: 0x0004D108
	public static void DoSetVertexBufferParams(ref Mesh.MeshData writeData, int totalVertexCount)
	{
		NativeArray<VertexAttributeDescriptor> attributes = new NativeArray<VertexAttributeDescriptor>(6, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
		int num = 0;
		attributes[num++] = GTVertexDataStreams_Descriptors.position;
		attributes[num++] = GTVertexDataStreams_Descriptors.color;
		attributes[num++] = GTVertexDataStreams_Descriptors.uv1;
		attributes[num++] = GTVertexDataStreams_Descriptors.lightmapUv;
		attributes[num++] = GTVertexDataStreams_Descriptors.normal;
		attributes[num++] = GTVertexDataStreams_Descriptors.tangent;
		writeData.SetVertexBufferParams(totalVertexCount, attributes);
		attributes.Dispose();
	}

	// Token: 0x040011C0 RID: 4544
	public static readonly VertexAttributeDescriptor position = new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, 0);

	// Token: 0x040011C1 RID: 4545
	public static readonly VertexAttributeDescriptor color = new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.UNorm8, 4, 0);

	// Token: 0x040011C2 RID: 4546
	public static readonly VertexAttributeDescriptor uv1 = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float16, 4, 0);

	// Token: 0x040011C3 RID: 4547
	public static readonly VertexAttributeDescriptor lightmapUv = new VertexAttributeDescriptor(VertexAttribute.TexCoord1, VertexAttributeFormat.Float16, 2, 0);

	// Token: 0x040011C4 RID: 4548
	public static readonly VertexAttributeDescriptor normal = new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3, 1);

	// Token: 0x040011C5 RID: 4549
	public static readonly VertexAttributeDescriptor tangent = new VertexAttributeDescriptor(VertexAttribute.Tangent, VertexAttributeFormat.SNorm8, 4, 1);
}
