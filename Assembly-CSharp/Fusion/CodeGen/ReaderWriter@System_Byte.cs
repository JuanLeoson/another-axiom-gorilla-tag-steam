using System;
using System.Runtime.CompilerServices;

namespace Fusion.CodeGen
{
	// Token: 0x02001044 RID: 4164
	[WeaverGenerated]
	[PreserveInPlugin]
	internal struct ReaderWriter@System_Byte : IElementReaderWriter<byte>
	{
		// Token: 0x06006705 RID: 26373 RVA: 0x0020A36D File Offset: 0x0020856D
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe byte Read(byte* data, int index)
		{
			return data[index * 4];
		}

		// Token: 0x06006706 RID: 26374 RVA: 0x00209D48 File Offset: 0x00207F48
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe ref byte ReadRef(byte* data, int index)
		{
			return ref data[index * 4];
		}

		// Token: 0x06006707 RID: 26375 RVA: 0x0020A379 File Offset: 0x00208579
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, byte val)
		{
			data[index * 4] = val;
		}

		// Token: 0x06006708 RID: 26376 RVA: 0x0001D558 File Offset: 0x0001B758
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 1;
		}

		// Token: 0x06006709 RID: 26377 RVA: 0x0020A388 File Offset: 0x00208588
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementHashCode(byte val)
		{
			return val.GetHashCode();
		}

		// Token: 0x0600670A RID: 26378 RVA: 0x0020A39C File Offset: 0x0020859C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public static IElementReaderWriter<byte> GetInstance()
		{
			if (ReaderWriter@System_Byte.Instance == null)
			{
				ReaderWriter@System_Byte.Instance = default(ReaderWriter@System_Byte);
			}
			return ReaderWriter@System_Byte.Instance;
		}

		// Token: 0x040074A3 RID: 29859
		[WeaverGenerated]
		public static IElementReaderWriter<byte> Instance;
	}
}
