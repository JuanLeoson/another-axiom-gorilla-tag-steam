using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000FF8 RID: 4088
	[Serializable]
	public struct Bits32
	{
		// Token: 0x170009A7 RID: 2471
		// (get) Token: 0x06006620 RID: 26144 RVA: 0x00207DAF File Offset: 0x00205FAF
		public int IntValue
		{
			get
			{
				return this.m_bits;
			}
		}

		// Token: 0x06006621 RID: 26145 RVA: 0x00207DB7 File Offset: 0x00205FB7
		public Bits32(int bits = 0)
		{
			this.m_bits = bits;
		}

		// Token: 0x06006622 RID: 26146 RVA: 0x00207DC0 File Offset: 0x00205FC0
		public void Clear()
		{
			this.m_bits = 0;
		}

		// Token: 0x06006623 RID: 26147 RVA: 0x00207DC9 File Offset: 0x00205FC9
		public void SetBit(int index, bool value)
		{
			if (value)
			{
				this.m_bits |= 1 << index;
				return;
			}
			this.m_bits &= ~(1 << index);
		}

		// Token: 0x06006624 RID: 26148 RVA: 0x00207DF6 File Offset: 0x00205FF6
		public bool IsBitSet(int index)
		{
			return (this.m_bits & 1 << index) != 0;
		}

		// Token: 0x04007110 RID: 28944
		[SerializeField]
		private int m_bits;
	}
}
