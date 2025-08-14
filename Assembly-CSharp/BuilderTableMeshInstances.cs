using System;
using Unity.Collections;
using UnityEngine.Jobs;

// Token: 0x02000542 RID: 1346
public struct BuilderTableMeshInstances
{
	// Token: 0x040029F9 RID: 10745
	public TransformAccessArray transforms;

	// Token: 0x040029FA RID: 10746
	public NativeList<int> texIndex;

	// Token: 0x040029FB RID: 10747
	public NativeList<float> tint;
}
