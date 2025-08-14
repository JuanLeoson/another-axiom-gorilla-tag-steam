using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion.CodeGen
{
	// Token: 0x02001024 RID: 4132
	[WeaverGenerated]
	[NetworkStructWeaved(10)]
	[PreserveInPlugin]
	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	internal struct FixedStorage@10 : INetworkStruct
	{
		// Token: 0x04007190 RID: 29072
		[FixedBuffer(typeof(int), 10)]
		[WeaverGenerated]
		[FieldOffset(0)]
		public FixedStorage@10.<Data>e__FixedBuffer Data;

		// Token: 0x04007191 RID: 29073
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(4)]
		private int _1;

		// Token: 0x04007192 RID: 29074
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(8)]
		private int _2;

		// Token: 0x04007193 RID: 29075
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(12)]
		private int _3;

		// Token: 0x04007194 RID: 29076
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(16)]
		private int _4;

		// Token: 0x04007195 RID: 29077
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(20)]
		private int _5;

		// Token: 0x04007196 RID: 29078
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(24)]
		private int _6;

		// Token: 0x04007197 RID: 29079
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(28)]
		private int _7;

		// Token: 0x04007198 RID: 29080
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(32)]
		private int _8;

		// Token: 0x04007199 RID: 29081
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(36)]
		private int _9;

		// Token: 0x02001025 RID: 4133
		[CompilerGenerated]
		[UnsafeValueType]
		[PreserveInPlugin]
		[WeaverGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 40)]
		public struct <Data>e__FixedBuffer
		{
			// Token: 0x0400719A RID: 29082
			[WeaverGenerated]
			public int FixedElementField;
		}
	}
}
