using System;
using System.Runtime.CompilerServices;

namespace Fusion.CodeGen
{
	// Token: 0x0200101C RID: 4124
	[WeaverGenerated]
	[PreserveInPlugin]
	internal struct ReaderWriter@Fusion_NetworkString : IElementReaderWriter<NetworkString<_32>>
	{
		// Token: 0x060066AB RID: 26283 RVA: 0x00209E49 File Offset: 0x00208049
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe NetworkString<_32> Read(byte* data, int index)
		{
			return *(NetworkString<_32>*)(data + index * 132);
		}

		// Token: 0x060066AC RID: 26284 RVA: 0x00209E59 File Offset: 0x00208059
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe ref NetworkString<_32> ReadRef(byte* data, int index)
		{
			return ref *(NetworkString<_32>*)(data + index * 132);
		}

		// Token: 0x060066AD RID: 26285 RVA: 0x00209E64 File Offset: 0x00208064
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, NetworkString<_32> val)
		{
			*(NetworkString<_32>*)(data + index * 132) = val;
		}

		// Token: 0x060066AE RID: 26286 RVA: 0x00209E75 File Offset: 0x00208075
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 33;
		}

		// Token: 0x060066AF RID: 26287 RVA: 0x00209E7C File Offset: 0x0020807C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementHashCode(NetworkString<_32> val)
		{
			return val.GetHashCode();
		}

		// Token: 0x060066B0 RID: 26288 RVA: 0x00209E98 File Offset: 0x00208098
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public static IElementReaderWriter<NetworkString<_32>> GetInstance()
		{
			if (ReaderWriter@Fusion_NetworkString.Instance == null)
			{
				ReaderWriter@Fusion_NetworkString.Instance = default(ReaderWriter@Fusion_NetworkString);
			}
			return ReaderWriter@Fusion_NetworkString.Instance;
		}

		// Token: 0x04007186 RID: 29062
		[WeaverGenerated]
		public static IElementReaderWriter<NetworkString<_32>> Instance;
	}
}
