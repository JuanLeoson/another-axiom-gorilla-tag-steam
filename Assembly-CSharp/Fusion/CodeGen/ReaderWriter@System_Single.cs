using System;
using System.Runtime.CompilerServices;

namespace Fusion.CodeGen
{
	// Token: 0x02001022 RID: 4130
	[WeaverGenerated]
	[PreserveInPlugin]
	internal struct ReaderWriter@System_Single : IElementReaderWriter<float>
	{
		// Token: 0x060066BD RID: 26301 RVA: 0x00209F6D File Offset: 0x0020816D
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe float Read(byte* data, int index)
		{
			return *(float*)(data + index * 4);
		}

		// Token: 0x060066BE RID: 26302 RVA: 0x00209D48 File Offset: 0x00207F48
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe ref float ReadRef(byte* data, int index)
		{
			return ref *(float*)(data + index * 4);
		}

		// Token: 0x060066BF RID: 26303 RVA: 0x00209F79 File Offset: 0x00208179
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, float val)
		{
			*(float*)(data + index * 4) = val;
		}

		// Token: 0x060066C0 RID: 26304 RVA: 0x0001D558 File Offset: 0x0001B758
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 1;
		}

		// Token: 0x060066C1 RID: 26305 RVA: 0x00209F88 File Offset: 0x00208188
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementHashCode(float val)
		{
			return val.GetHashCode();
		}

		// Token: 0x060066C2 RID: 26306 RVA: 0x00209F9C File Offset: 0x0020819C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public static IElementReaderWriter<float> GetInstance()
		{
			if (ReaderWriter@System_Single.Instance == null)
			{
				ReaderWriter@System_Single.Instance = default(ReaderWriter@System_Single);
			}
			return ReaderWriter@System_Single.Instance;
		}

		// Token: 0x0400718E RID: 29070
		[WeaverGenerated]
		public static IElementReaderWriter<float> Instance;
	}
}
