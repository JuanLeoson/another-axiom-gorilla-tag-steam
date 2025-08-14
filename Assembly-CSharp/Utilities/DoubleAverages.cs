using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	// Token: 0x02000BD8 RID: 3032
	public class DoubleAverages : AverageCalculator<double>
	{
		// Token: 0x06004998 RID: 18840 RVA: 0x00166182 File Offset: 0x00164382
		public DoubleAverages(int sampleCount) : base(sampleCount)
		{
			this.Reset();
		}

		// Token: 0x06004999 RID: 18841 RVA: 0x00166191 File Offset: 0x00164391
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override double PlusEquals(double value, double sample)
		{
			return value + sample;
		}

		// Token: 0x0600499A RID: 18842 RVA: 0x00166196 File Offset: 0x00164396
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override double MinusEquals(double value, double sample)
		{
			return value - sample;
		}

		// Token: 0x0600499B RID: 18843 RVA: 0x0016619B File Offset: 0x0016439B
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override double Divide(double value, int sampleCount)
		{
			return value / (double)sampleCount;
		}

		// Token: 0x0600499C RID: 18844 RVA: 0x001661A1 File Offset: 0x001643A1
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override double Multiply(double value, int sampleCount)
		{
			return value * (double)sampleCount;
		}
	}
}
