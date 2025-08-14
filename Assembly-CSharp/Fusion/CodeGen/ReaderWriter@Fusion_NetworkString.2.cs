using System;
using System.Runtime.CompilerServices;

namespace Fusion.CodeGen
{
	// Token: 0x0200103D RID: 4157
	[WeaverGenerated]
	[PreserveInPlugin]
	internal struct ReaderWriter@Fusion_NetworkString : IElementReaderWriter<NetworkString<_128>>
	{
		// Token: 0x060066F9 RID: 26361 RVA: 0x0020A2B4 File Offset: 0x002084B4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe NetworkString<_128> Read(byte* data, int index)
		{
			return *(NetworkString<_128>*)(data + index * 516);
		}

		// Token: 0x060066FA RID: 26362 RVA: 0x0020A2C4 File Offset: 0x002084C4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe ref NetworkString<_128> ReadRef(byte* data, int index)
		{
			return ref *(NetworkString<_128>*)(data + index * 516);
		}

		// Token: 0x060066FB RID: 26363 RVA: 0x0020A2CF File Offset: 0x002084CF
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, NetworkString<_128> val)
		{
			*(NetworkString<_128>*)(data + index * 516) = val;
		}

		// Token: 0x060066FC RID: 26364 RVA: 0x0020A2E0 File Offset: 0x002084E0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 129;
		}

		// Token: 0x060066FD RID: 26365 RVA: 0x0020A2E8 File Offset: 0x002084E8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementHashCode(NetworkString<_128> val)
		{
			return val.GetHashCode();
		}

		// Token: 0x060066FE RID: 26366 RVA: 0x0020A304 File Offset: 0x00208504
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public static IElementReaderWriter<NetworkString<_128>> GetInstance()
		{
			if (ReaderWriter@Fusion_NetworkString.Instance == null)
			{
				ReaderWriter@Fusion_NetworkString.Instance = default(ReaderWriter@Fusion_NetworkString);
			}
			return ReaderWriter@Fusion_NetworkString.Instance;
		}

		// Token: 0x04007451 RID: 29777
		[WeaverGenerated]
		public static IElementReaderWriter<NetworkString<_128>> Instance;
	}
}
