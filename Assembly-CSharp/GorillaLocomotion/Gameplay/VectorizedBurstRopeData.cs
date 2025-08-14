using System;
using Unity.Collections;
using Unity.Mathematics;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000E33 RID: 3635
	public struct VectorizedBurstRopeData
	{
		// Token: 0x0400653A RID: 25914
		public NativeArray<float4> posX;

		// Token: 0x0400653B RID: 25915
		public NativeArray<float4> posY;

		// Token: 0x0400653C RID: 25916
		public NativeArray<float4> posZ;

		// Token: 0x0400653D RID: 25917
		public NativeArray<int4> validNodes;

		// Token: 0x0400653E RID: 25918
		public NativeArray<float4> lastPosX;

		// Token: 0x0400653F RID: 25919
		public NativeArray<float4> lastPosY;

		// Token: 0x04006540 RID: 25920
		public NativeArray<float4> lastPosZ;

		// Token: 0x04006541 RID: 25921
		public NativeArray<float3> ropeRoots;

		// Token: 0x04006542 RID: 25922
		public NativeArray<float4> nodeMass;
	}
}
