using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000EB2 RID: 3762
	public static class GRef
	{
		// Token: 0x06005E02 RID: 24066 RVA: 0x001DA4F4 File Offset: 0x001D86F4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ShouldResolveNow(GRef.EResolveModes mode)
		{
			return Application.isPlaying && (mode & GRef.EResolveModes.Runtime) == GRef.EResolveModes.Runtime;
		}

		// Token: 0x06005E03 RID: 24067 RVA: 0x001DA505 File Offset: 0x001D8705
		public static bool IsAnyResolveModeOn(GRef.EResolveModes mode)
		{
			return mode > GRef.EResolveModes.None;
		}

		// Token: 0x02000EB3 RID: 3763
		[Flags]
		public enum EResolveModes
		{
			// Token: 0x040067F4 RID: 26612
			None = 0,
			// Token: 0x040067F5 RID: 26613
			Runtime = 1,
			// Token: 0x040067F6 RID: 26614
			SceneProcessing = 2
		}
	}
}
