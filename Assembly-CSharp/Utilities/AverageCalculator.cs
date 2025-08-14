using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	// Token: 0x02000BD7 RID: 3031
	public abstract class AverageCalculator<T> where T : struct
	{
		// Token: 0x17000718 RID: 1816
		// (get) Token: 0x0600498F RID: 18831 RVA: 0x00166062 File Offset: 0x00164262
		public T Average
		{
			get
			{
				return this.m_average;
			}
		}

		// Token: 0x06004990 RID: 18832 RVA: 0x0016606A File Offset: 0x0016426A
		public AverageCalculator(int sampleCount)
		{
			this.m_samples = new T[sampleCount];
		}

		// Token: 0x06004991 RID: 18833 RVA: 0x00166080 File Offset: 0x00164280
		public virtual void AddSample(T sample)
		{
			T sample2 = this.m_samples[this.m_index];
			this.m_total = this.MinusEquals(this.m_total, sample2);
			this.m_total = this.PlusEquals(this.m_total, sample);
			this.m_average = this.Divide(this.m_total, this.m_samples.Length);
			this.m_samples[this.m_index] = sample;
			int num = this.m_index + 1;
			this.m_index = num;
			this.m_index = num % this.m_samples.Length;
		}

		// Token: 0x06004992 RID: 18834 RVA: 0x00166114 File Offset: 0x00164314
		public virtual void Reset()
		{
			T t = this.DefaultTypeValue();
			for (int i = 0; i < this.m_samples.Length; i++)
			{
				this.m_samples[i] = t;
			}
			this.m_index = 0;
			this.m_average = t;
			this.m_total = this.Multiply(t, this.m_samples.Length);
		}

		// Token: 0x06004993 RID: 18835 RVA: 0x0016616C File Offset: 0x0016436C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual T DefaultTypeValue()
		{
			return default(T);
		}

		// Token: 0x06004994 RID: 18836
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract T PlusEquals(T value, T sample);

		// Token: 0x06004995 RID: 18837
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract T MinusEquals(T value, T sample);

		// Token: 0x06004996 RID: 18838
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract T Divide(T value, int sampleCount);

		// Token: 0x06004997 RID: 18839
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract T Multiply(T value, int sampleCount);

		// Token: 0x0400527B RID: 21115
		private T[] m_samples;

		// Token: 0x0400527C RID: 21116
		private T m_average;

		// Token: 0x0400527D RID: 21117
		private T m_total;

		// Token: 0x0400527E RID: 21118
		private int m_index;
	}
}
