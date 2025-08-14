using System;
using System.Runtime.CompilerServices;

namespace Fusion.CodeGen
{
	// Token: 0x02001030 RID: 4144
	[WeaverGenerated]
	[PreserveInPlugin]
	internal struct ReaderWriter@System_Int64 : IElementReaderWriter<long>
	{
		// Token: 0x060066DE RID: 26334 RVA: 0x0020A12D File Offset: 0x0020832D
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe long Read(byte* data, int index)
		{
			return *(long*)(data + index * 8);
		}

		// Token: 0x060066DF RID: 26335 RVA: 0x0020A139 File Offset: 0x00208339
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe ref long ReadRef(byte* data, int index)
		{
			return ref *(long*)(data + index * 8);
		}

		// Token: 0x060066E0 RID: 26336 RVA: 0x0020A144 File Offset: 0x00208344
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, long val)
		{
			*(long*)(data + index * 8) = val;
		}

		// Token: 0x060066E1 RID: 26337 RVA: 0x00012237 File Offset: 0x00010437
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 2;
		}

		// Token: 0x060066E2 RID: 26338 RVA: 0x0020A154 File Offset: 0x00208354
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementHashCode(long val)
		{
			return val.GetHashCode();
		}

		// Token: 0x060066E3 RID: 26339 RVA: 0x0020A168 File Offset: 0x00208368
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public static IElementReaderWriter<long> GetInstance()
		{
			if (ReaderWriter@System_Int64.Instance == null)
			{
				ReaderWriter@System_Int64.Instance = default(ReaderWriter@System_Int64);
			}
			return ReaderWriter@System_Int64.Instance;
		}

		// Token: 0x04007276 RID: 29302
		[WeaverGenerated]
		public static IElementReaderWriter<long> Instance;
	}
}
