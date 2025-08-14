using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion.CodeGen
{
	// Token: 0x02001042 RID: 4162
	[WeaverGenerated]
	[NetworkStructWeaved(6)]
	[PreserveInPlugin]
	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	internal struct FixedStorage@6 : INetworkStruct
	{
		// Token: 0x0400749C RID: 29852
		[FixedBuffer(typeof(int), 6)]
		[WeaverGenerated]
		[FieldOffset(0)]
		public FixedStorage@6.<Data>e__FixedBuffer Data;

		// Token: 0x0400749D RID: 29853
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(4)]
		private int _1;

		// Token: 0x0400749E RID: 29854
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(8)]
		private int _2;

		// Token: 0x0400749F RID: 29855
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(12)]
		private int _3;

		// Token: 0x040074A0 RID: 29856
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(16)]
		private int _4;

		// Token: 0x040074A1 RID: 29857
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(20)]
		private int _5;

		// Token: 0x02001043 RID: 4163
		[CompilerGenerated]
		[UnsafeValueType]
		[PreserveInPlugin]
		[WeaverGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 24)]
		public struct <Data>e__FixedBuffer
		{
			// Token: 0x040074A2 RID: 29858
			[WeaverGenerated]
			public int FixedElementField;
		}
	}
}
