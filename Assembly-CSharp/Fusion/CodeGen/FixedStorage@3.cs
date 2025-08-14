using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion.CodeGen
{
	// Token: 0x0200101E RID: 4126
	[WeaverGenerated]
	[NetworkStructWeaved(3)]
	[PreserveInPlugin]
	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	internal struct FixedStorage@3 : INetworkStruct
	{
		// Token: 0x04007188 RID: 29064
		[FixedBuffer(typeof(int), 3)]
		[WeaverGenerated]
		[FieldOffset(0)]
		public FixedStorage@3.<Data>e__FixedBuffer Data;

		// Token: 0x04007189 RID: 29065
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(4)]
		private int _1;

		// Token: 0x0400718A RID: 29066
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(8)]
		private int _2;

		// Token: 0x0200101F RID: 4127
		[CompilerGenerated]
		[UnsafeValueType]
		[PreserveInPlugin]
		[WeaverGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 12)]
		public struct <Data>e__FixedBuffer
		{
			// Token: 0x0400718B RID: 29067
			[WeaverGenerated]
			public int FixedElementField;
		}
	}
}
