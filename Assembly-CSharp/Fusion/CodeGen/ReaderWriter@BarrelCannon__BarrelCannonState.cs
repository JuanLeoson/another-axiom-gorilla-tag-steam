using System;
using System.Runtime.CompilerServices;

namespace Fusion.CodeGen
{
	// Token: 0x02001016 RID: 4118
	[WeaverGenerated]
	[PreserveInPlugin]
	internal struct ReaderWriter@BarrelCannon__BarrelCannonState : IElementReaderWriter<BarrelCannon.BarrelCannonState>
	{
		// Token: 0x06006699 RID: 26265 RVA: 0x00209D38 File Offset: 0x00207F38
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe BarrelCannon.BarrelCannonState Read(byte* data, int index)
		{
			return *(BarrelCannon.BarrelCannonState*)(data + index * 4);
		}

		// Token: 0x0600669A RID: 26266 RVA: 0x00209D48 File Offset: 0x00207F48
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe ref BarrelCannon.BarrelCannonState ReadRef(byte* data, int index)
		{
			return ref *(BarrelCannon.BarrelCannonState*)(data + index * 4);
		}

		// Token: 0x0600669B RID: 26267 RVA: 0x00209D53 File Offset: 0x00207F53
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, BarrelCannon.BarrelCannonState val)
		{
			*(BarrelCannon.BarrelCannonState*)(data + index * 4) = val;
		}

		// Token: 0x0600669C RID: 26268 RVA: 0x0001D558 File Offset: 0x0001B758
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 1;
		}

		// Token: 0x0600669D RID: 26269 RVA: 0x00209D64 File Offset: 0x00207F64
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[PreserveInPlugin]
		[WeaverGenerated]
		public int GetElementHashCode(BarrelCannon.BarrelCannonState val)
		{
			return val.GetHashCode();
		}

		// Token: 0x0600669E RID: 26270 RVA: 0x00209D80 File Offset: 0x00207F80
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public static IElementReaderWriter<BarrelCannon.BarrelCannonState> GetInstance()
		{
			if (ReaderWriter@BarrelCannon__BarrelCannonState.Instance == null)
			{
				ReaderWriter@BarrelCannon__BarrelCannonState.Instance = default(ReaderWriter@BarrelCannon__BarrelCannonState);
			}
			return ReaderWriter@BarrelCannon__BarrelCannonState.Instance;
		}

		// Token: 0x04007160 RID: 29024
		[WeaverGenerated]
		public static IElementReaderWriter<BarrelCannon.BarrelCannonState> Instance;
	}
}
