using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion.CodeGen
{
	// Token: 0x02001014 RID: 4116
	[WeaverGenerated]
	[NetworkStructWeaved(1)]
	[PreserveInPlugin]
	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	internal struct FixedStorage@1 : INetworkStruct
	{
		// Token: 0x0400715E RID: 29022
		[FixedBuffer(typeof(int), 1)]
		[WeaverGenerated]
		[FieldOffset(0)]
		public FixedStorage@1.<Data>e__FixedBuffer Data;

		// Token: 0x02001015 RID: 4117
		[CompilerGenerated]
		[UnsafeValueType]
		[PreserveInPlugin]
		[WeaverGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 4)]
		public struct <Data>e__FixedBuffer
		{
			// Token: 0x0400715F RID: 29023
			[WeaverGenerated]
			public int FixedElementField;
		}
	}
}
