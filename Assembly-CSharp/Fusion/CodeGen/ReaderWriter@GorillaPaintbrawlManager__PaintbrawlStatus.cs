using System;
using System.Runtime.CompilerServices;

namespace Fusion.CodeGen
{
	// Token: 0x02001033 RID: 4147
	[WeaverGenerated]
	[PreserveInPlugin]
	internal struct ReaderWriter@GorillaPaintbrawlManager__PaintbrawlStatus : IElementReaderWriter<GorillaPaintbrawlManager.PaintbrawlStatus>
	{
		// Token: 0x060066EA RID: 26346 RVA: 0x0020A1DC File Offset: 0x002083DC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe GorillaPaintbrawlManager.PaintbrawlStatus Read(byte* data, int index)
		{
			return *(GorillaPaintbrawlManager.PaintbrawlStatus*)(data + index * 4);
		}

		// Token: 0x060066EB RID: 26347 RVA: 0x00209D48 File Offset: 0x00207F48
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe ref GorillaPaintbrawlManager.PaintbrawlStatus ReadRef(byte* data, int index)
		{
			return ref *(GorillaPaintbrawlManager.PaintbrawlStatus*)(data + index * 4);
		}

		// Token: 0x060066EC RID: 26348 RVA: 0x0020A1EC File Offset: 0x002083EC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, GorillaPaintbrawlManager.PaintbrawlStatus val)
		{
			*(GorillaPaintbrawlManager.PaintbrawlStatus*)(data + index * 4) = val;
		}

		// Token: 0x060066ED RID: 26349 RVA: 0x0001D558 File Offset: 0x0001B758
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 1;
		}

		// Token: 0x060066EE RID: 26350 RVA: 0x0020A200 File Offset: 0x00208400
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementHashCode(GorillaPaintbrawlManager.PaintbrawlStatus val)
		{
			return val.GetHashCode();
		}

		// Token: 0x060066EF RID: 26351 RVA: 0x0020A21C File Offset: 0x0020841C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public static IElementReaderWriter<GorillaPaintbrawlManager.PaintbrawlStatus> GetInstance()
		{
			if (ReaderWriter@GorillaPaintbrawlManager__PaintbrawlStatus.Instance == null)
			{
				ReaderWriter@GorillaPaintbrawlManager__PaintbrawlStatus.Instance = default(ReaderWriter@GorillaPaintbrawlManager__PaintbrawlStatus);
			}
			return ReaderWriter@GorillaPaintbrawlManager__PaintbrawlStatus.Instance;
		}

		// Token: 0x04007279 RID: 29305
		[WeaverGenerated]
		public static IElementReaderWriter<GorillaPaintbrawlManager.PaintbrawlStatus> Instance;
	}
}
