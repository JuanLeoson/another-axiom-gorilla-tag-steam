using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion.CodeGen
{
	// Token: 0x02001027 RID: 4135
	[WeaverGenerated]
	[NetworkStructWeaved(4)]
	[PreserveInPlugin]
	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	internal struct FixedStorage@4 : INetworkStruct
	{
		// Token: 0x0400719C RID: 29084
		[FixedBuffer(typeof(int), 4)]
		[WeaverGenerated]
		[FieldOffset(0)]
		public FixedStorage@4.<Data>e__FixedBuffer Data;

		// Token: 0x0400719D RID: 29085
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(4)]
		private int _1;

		// Token: 0x0400719E RID: 29086
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(8)]
		private int _2;

		// Token: 0x0400719F RID: 29087
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(12)]
		private int _3;

		// Token: 0x02001028 RID: 4136
		[CompilerGenerated]
		[UnsafeValueType]
		[PreserveInPlugin]
		[WeaverGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 16)]
		public struct <Data>e__FixedBuffer
		{
			// Token: 0x040071A0 RID: 29088
			[WeaverGenerated]
			public int FixedElementField;
		}
	}
}
