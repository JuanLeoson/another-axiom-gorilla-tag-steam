using System;
using System.Runtime.CompilerServices;

namespace Fusion.CodeGen
{
	// Token: 0x02001018 RID: 4120
	[WeaverGenerated]
	[PreserveInPlugin]
	internal struct ReaderWriter@Fusion_NetworkBool : IElementReaderWriter<NetworkBool>
	{
		// Token: 0x060066A2 RID: 26274 RVA: 0x00209DC5 File Offset: 0x00207FC5
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe NetworkBool Read(byte* data, int index)
		{
			return *(NetworkBool*)(data + index * 4);
		}

		// Token: 0x060066A3 RID: 26275 RVA: 0x00209D48 File Offset: 0x00207F48
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe ref NetworkBool ReadRef(byte* data, int index)
		{
			return ref *(NetworkBool*)(data + index * 4);
		}

		// Token: 0x060066A4 RID: 26276 RVA: 0x00209DD5 File Offset: 0x00207FD5
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, NetworkBool val)
		{
			*(NetworkBool*)(data + index * 4) = val;
		}

		// Token: 0x060066A5 RID: 26277 RVA: 0x0001D558 File Offset: 0x0001B758
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 1;
		}

		// Token: 0x060066A6 RID: 26278 RVA: 0x00209DE8 File Offset: 0x00207FE8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementHashCode(NetworkBool val)
		{
			return val.GetHashCode();
		}

		// Token: 0x060066A7 RID: 26279 RVA: 0x00209E04 File Offset: 0x00208004
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public static IElementReaderWriter<NetworkBool> GetInstance()
		{
			if (ReaderWriter@Fusion_NetworkBool.Instance == null)
			{
				ReaderWriter@Fusion_NetworkBool.Instance = default(ReaderWriter@Fusion_NetworkBool);
			}
			return ReaderWriter@Fusion_NetworkBool.Instance;
		}

		// Token: 0x04007162 RID: 29026
		[WeaverGenerated]
		public static IElementReaderWriter<NetworkBool> Instance;
	}
}
