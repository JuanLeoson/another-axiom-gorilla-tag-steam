using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion.CodeGen
{
	// Token: 0x02001029 RID: 4137
	[WeaverGenerated]
	[PreserveInPlugin]
	internal struct ReaderWriter@UnityEngine_Quaternion : IElementReaderWriter<Quaternion>
	{
		// Token: 0x060066C9 RID: 26313 RVA: 0x0020A005 File Offset: 0x00208205
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe Quaternion Read(byte* data, int index)
		{
			return *(Quaternion*)(data + index * 16);
		}

		// Token: 0x060066CA RID: 26314 RVA: 0x0020A015 File Offset: 0x00208215
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe ref Quaternion ReadRef(byte* data, int index)
		{
			return ref *(Quaternion*)(data + index * 16);
		}

		// Token: 0x060066CB RID: 26315 RVA: 0x0020A020 File Offset: 0x00208220
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, Quaternion val)
		{
			*(Quaternion*)(data + index * 16) = val;
		}

		// Token: 0x060066CC RID: 26316 RVA: 0x00144C03 File Offset: 0x00142E03
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 4;
		}

		// Token: 0x060066CD RID: 26317 RVA: 0x0020A034 File Offset: 0x00208234
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementHashCode(Quaternion val)
		{
			return val.GetHashCode();
		}

		// Token: 0x060066CE RID: 26318 RVA: 0x0020A050 File Offset: 0x00208250
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public static IElementReaderWriter<Quaternion> GetInstance()
		{
			if (ReaderWriter@UnityEngine_Quaternion.Instance == null)
			{
				ReaderWriter@UnityEngine_Quaternion.Instance = default(ReaderWriter@UnityEngine_Quaternion);
			}
			return ReaderWriter@UnityEngine_Quaternion.Instance;
		}

		// Token: 0x040071A1 RID: 29089
		[WeaverGenerated]
		public static IElementReaderWriter<Quaternion> Instance;
	}
}
