using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion.CodeGen
{
	// Token: 0x02001020 RID: 4128
	[WeaverGenerated]
	[PreserveInPlugin]
	internal struct ReaderWriter@UnityEngine_Vector3 : IElementReaderWriter<Vector3>
	{
		// Token: 0x060066B4 RID: 26292 RVA: 0x00209EDD File Offset: 0x002080DD
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe Vector3 Read(byte* data, int index)
		{
			return *(Vector3*)(data + index * 12);
		}

		// Token: 0x060066B5 RID: 26293 RVA: 0x00209EED File Offset: 0x002080ED
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe ref Vector3 ReadRef(byte* data, int index)
		{
			return ref *(Vector3*)(data + index * 12);
		}

		// Token: 0x060066B6 RID: 26294 RVA: 0x00209EF8 File Offset: 0x002080F8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, Vector3 val)
		{
			*(Vector3*)(data + index * 12) = val;
		}

		// Token: 0x060066B7 RID: 26295 RVA: 0x000EA036 File Offset: 0x000E8236
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 3;
		}

		// Token: 0x060066B8 RID: 26296 RVA: 0x00209F0C File Offset: 0x0020810C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementHashCode(Vector3 val)
		{
			return val.GetHashCode();
		}

		// Token: 0x060066B9 RID: 26297 RVA: 0x00209F28 File Offset: 0x00208128
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public static IElementReaderWriter<Vector3> GetInstance()
		{
			if (ReaderWriter@UnityEngine_Vector3.Instance == null)
			{
				ReaderWriter@UnityEngine_Vector3.Instance = default(ReaderWriter@UnityEngine_Vector3);
			}
			return ReaderWriter@UnityEngine_Vector3.Instance;
		}

		// Token: 0x0400718C RID: 29068
		[WeaverGenerated]
		public static IElementReaderWriter<Vector3> Instance;
	}
}
