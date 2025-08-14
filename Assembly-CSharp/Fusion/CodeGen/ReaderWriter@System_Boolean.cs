using System;
using System.Runtime.CompilerServices;

namespace Fusion.CodeGen
{
	// Token: 0x02001047 RID: 4167
	[WeaverGenerated]
	[PreserveInPlugin]
	internal struct ReaderWriter@System_Boolean : IElementReaderWriter<bool>
	{
		// Token: 0x06006711 RID: 26385 RVA: 0x0020A410 File Offset: 0x00208610
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe bool Read(byte* data, int index)
		{
			return ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + index * 4));
		}

		// Token: 0x06006712 RID: 26386 RVA: 0x0020A420 File Offset: 0x00208620
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe ref bool ReadRef(byte* data, int index)
		{
			throw new NotSupportedException("Only supported for trivially copyable types. System.Boolean is not trivially copyable.");
		}

		// Token: 0x06006713 RID: 26387 RVA: 0x0020A42C File Offset: 0x0020862C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, bool val)
		{
			ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + index * 4), val);
		}

		// Token: 0x06006714 RID: 26388 RVA: 0x0001D558 File Offset: 0x0001B758
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 1;
		}

		// Token: 0x06006715 RID: 26389 RVA: 0x0020A440 File Offset: 0x00208640
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementHashCode(bool val)
		{
			return val.GetHashCode();
		}

		// Token: 0x06006716 RID: 26390 RVA: 0x0020A454 File Offset: 0x00208654
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public static IElementReaderWriter<bool> GetInstance()
		{
			if (ReaderWriter@System_Boolean.Instance == null)
			{
				ReaderWriter@System_Boolean.Instance = default(ReaderWriter@System_Boolean);
			}
			return ReaderWriter@System_Boolean.Instance;
		}

		// Token: 0x040074A6 RID: 29862
		[WeaverGenerated]
		public static IElementReaderWriter<bool> Instance;
	}
}
