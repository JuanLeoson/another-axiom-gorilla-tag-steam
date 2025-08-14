using System;
using System.Runtime.CompilerServices;

namespace Fusion.CodeGen
{
	// Token: 0x0200102B RID: 4139
	[WeaverGenerated]
	[PreserveInPlugin]
	internal struct ReaderWriter@System_Int32 : IElementReaderWriter<int>
	{
		// Token: 0x060066D2 RID: 26322 RVA: 0x0020A095 File Offset: 0x00208295
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe int Read(byte* data, int index)
		{
			return *(int*)(data + index * 4);
		}

		// Token: 0x060066D3 RID: 26323 RVA: 0x00209D48 File Offset: 0x00207F48
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe ref int ReadRef(byte* data, int index)
		{
			return ref *(int*)(data + index * 4);
		}

		// Token: 0x060066D4 RID: 26324 RVA: 0x0020A0A1 File Offset: 0x002082A1
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, int val)
		{
			*(int*)(data + index * 4) = val;
		}

		// Token: 0x060066D5 RID: 26325 RVA: 0x0001D558 File Offset: 0x0001B758
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 1;
		}

		// Token: 0x060066D6 RID: 26326 RVA: 0x0020A0B0 File Offset: 0x002082B0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementHashCode(int val)
		{
			return val.GetHashCode();
		}

		// Token: 0x060066D7 RID: 26327 RVA: 0x0020A0C4 File Offset: 0x002082C4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public static IElementReaderWriter<int> GetInstance()
		{
			if (ReaderWriter@System_Int32.Instance == null)
			{
				ReaderWriter@System_Int32.Instance = default(ReaderWriter@System_Int32);
			}
			return ReaderWriter@System_Int32.Instance;
		}

		// Token: 0x040071A3 RID: 29091
		[WeaverGenerated]
		public static IElementReaderWriter<int> Instance;
	}
}
