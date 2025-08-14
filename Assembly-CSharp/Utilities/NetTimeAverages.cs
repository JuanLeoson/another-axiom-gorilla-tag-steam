using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	// Token: 0x02000BDA RID: 3034
	public class NetTimeAverages : DoubleAverages
	{
		// Token: 0x060049A2 RID: 18850 RVA: 0x001661C2 File Offset: 0x001643C2
		public NetTimeAverages(int sampleCount) : base(sampleCount)
		{
		}

		// Token: 0x060049A3 RID: 18851 RVA: 0x001661CB File Offset: 0x001643CB
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override double DefaultTypeValue()
		{
			return 1.0;
		}
	}
}
