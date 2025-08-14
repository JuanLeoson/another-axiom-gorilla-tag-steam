using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	// Token: 0x02000BD9 RID: 3033
	public class FloatAverages : AverageCalculator<float>
	{
		// Token: 0x0600499D RID: 18845 RVA: 0x001661A7 File Offset: 0x001643A7
		public FloatAverages(int sampleCount) : base(sampleCount)
		{
			this.Reset();
		}

		// Token: 0x0600499E RID: 18846 RVA: 0x00166191 File Offset: 0x00164391
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override float PlusEquals(float value, float sample)
		{
			return value + sample;
		}

		// Token: 0x0600499F RID: 18847 RVA: 0x00166196 File Offset: 0x00164396
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override float MinusEquals(float value, float sample)
		{
			return value - sample;
		}

		// Token: 0x060049A0 RID: 18848 RVA: 0x001661B6 File Offset: 0x001643B6
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override float Divide(float value, int sampleCount)
		{
			return value / (float)sampleCount;
		}

		// Token: 0x060049A1 RID: 18849 RVA: 0x001661BC File Offset: 0x001643BC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override float Multiply(float value, int sampleCount)
		{
			return value * (float)sampleCount;
		}
	}
}
