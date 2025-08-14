using System;

namespace GorillaTag.Shared.Scripts.Utilities
{
	// Token: 0x02000EC8 RID: 3784
	public sealed class GTBitArray
	{
		// Token: 0x17000929 RID: 2345
		public bool this[int idx]
		{
			get
			{
				if (idx < 0 || idx >= this.Length)
				{
					throw new ArgumentOutOfRangeException();
				}
				int num = idx / 32;
				int num2 = idx % 32;
				return ((ulong)this._data[num] & (ulong)(1L << (num2 & 31))) > 0UL;
			}
			set
			{
				if (idx < 0 || idx >= this.Length)
				{
					throw new ArgumentOutOfRangeException();
				}
				int num = idx / 32;
				int num2 = idx % 32;
				if (value)
				{
					this._data[num] |= 1U << num2;
					return;
				}
				this._data[num] &= ~(1U << num2);
			}
		}

		// Token: 0x06005E61 RID: 24161 RVA: 0x001DBBE0 File Offset: 0x001D9DE0
		public GTBitArray(int length)
		{
			this.Length = length;
			this._data = ((length % 32 == 0) ? new uint[length / 32] : new uint[length / 32 + 1]);
			for (int i = 0; i < this._data.Length; i++)
			{
				this._data[i] = 0U;
			}
		}

		// Token: 0x06005E62 RID: 24162 RVA: 0x001DBC38 File Offset: 0x001D9E38
		public void Clear()
		{
			for (int i = 0; i < this._data.Length; i++)
			{
				this._data[i] = 0U;
			}
		}

		// Token: 0x04006834 RID: 26676
		public readonly int Length;

		// Token: 0x04006835 RID: 26677
		private readonly uint[] _data;
	}
}
