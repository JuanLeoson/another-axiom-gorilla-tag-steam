using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

// Token: 0x02000544 RID: 1348
public class BuilderTableDataRenderIndirectBatch
{
	// Token: 0x040029FF RID: 10751
	public int totalInstances;

	// Token: 0x04002A00 RID: 10752
	public TransformAccessArray instanceTransform;

	// Token: 0x04002A01 RID: 10753
	public NativeArray<int> instanceTransformIndexToDataIndex;

	// Token: 0x04002A02 RID: 10754
	public NativeArray<Matrix4x4> instanceObjectToWorld;

	// Token: 0x04002A03 RID: 10755
	public NativeArray<int> instanceTexIndex;

	// Token: 0x04002A04 RID: 10756
	public NativeArray<float> instanceTint;

	// Token: 0x04002A05 RID: 10757
	public NativeArray<int> instanceLodLevel;

	// Token: 0x04002A06 RID: 10758
	public NativeArray<int> instanceLodLevelDirty;

	// Token: 0x04002A07 RID: 10759
	public NativeList<BuilderTableMeshInstances> renderMeshes;

	// Token: 0x04002A08 RID: 10760
	public GraphicsBuffer commandBuf;

	// Token: 0x04002A09 RID: 10761
	public GraphicsBuffer matrixBuf;

	// Token: 0x04002A0A RID: 10762
	public GraphicsBuffer texIndexBuf;

	// Token: 0x04002A0B RID: 10763
	public GraphicsBuffer tintBuf;

	// Token: 0x04002A0C RID: 10764
	public NativeArray<GraphicsBuffer.IndirectDrawIndexedArgs> commandData;

	// Token: 0x04002A0D RID: 10765
	public int commandCount;

	// Token: 0x04002A0E RID: 10766
	public RenderParams rp;
}
