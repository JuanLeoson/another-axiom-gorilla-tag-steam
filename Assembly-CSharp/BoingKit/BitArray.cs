using System;

namespace BoingKit
{
	// Token: 0x02000FF9 RID: 4089
	public struct BitArray
	{
		// Token: 0x170009A8 RID: 2472
		// (get) Token: 0x06006625 RID: 26149 RVA: 0x00207E08 File Offset: 0x00206008
		public int[] Blocks
		{
			get
			{
				return this.m_aBlock;
			}
		}

		// Token: 0x06006626 RID: 26150 RVA: 0x00207E10 File Offset: 0x00206010
		private static int GetBlockIndex(int index)
		{
			return index / 4;
		}

		// Token: 0x06006627 RID: 26151 RVA: 0x00207E15 File Offset: 0x00206015
		private static int GetSubIndex(int index)
		{
			return index % 4;
		}

		// Token: 0x06006628 RID: 26152 RVA: 0x00207E1C File Offset: 0x0020601C
		private static void SetBit(int index, bool value, int[] blocks)
		{
			int blockIndex = BitArray.GetBlockIndex(index);
			int subIndex = BitArray.GetSubIndex(index);
			if (value)
			{
				blocks[blockIndex] |= 1 << subIndex;
				return;
			}
			blocks[blockIndex] &= ~(1 << subIndex);
		}

		// Token: 0x06006629 RID: 26153 RVA: 0x00207E5E File Offset: 0x0020605E
		private static bool IsBitSet(int index, int[] blocks)
		{
			return (blocks[BitArray.GetBlockIndex(index)] & 1 << BitArray.GetSubIndex(index)) != 0;
		}

		// Token: 0x0600662A RID: 26154 RVA: 0x00207E78 File Offset: 0x00206078
		public BitArray(int capacity)
		{
			int num = (capacity + 4 - 1) / 4;
			this.m_aBlock = new int[num];
			this.Clear();
		}

		// Token: 0x0600662B RID: 26155 RVA: 0x00207EA0 File Offset: 0x002060A0
		public void Resize(int capacity)
		{
			int num = (capacity + 4 - 1) / 4;
			if (num <= this.m_aBlock.Length)
			{
				return;
			}
			int[] array = new int[num];
			int i = 0;
			int num2 = this.m_aBlock.Length;
			while (i < num2)
			{
				array[i] = this.m_aBlock[i];
				i++;
			}
			this.m_aBlock = array;
		}

		// Token: 0x0600662C RID: 26156 RVA: 0x00207EEF File Offset: 0x002060EF
		public void Clear()
		{
			this.SetAllBits(false);
		}

		// Token: 0x0600662D RID: 26157 RVA: 0x00207EF8 File Offset: 0x002060F8
		public void SetAllBits(bool value)
		{
			int num = value ? -1 : 1;
			int i = 0;
			int num2 = this.m_aBlock.Length;
			while (i < num2)
			{
				this.m_aBlock[i] = num;
				i++;
			}
		}

		// Token: 0x0600662E RID: 26158 RVA: 0x00207F2B File Offset: 0x0020612B
		public void SetBit(int index, bool value)
		{
			BitArray.SetBit(index, value, this.m_aBlock);
		}

		// Token: 0x0600662F RID: 26159 RVA: 0x00207F3A File Offset: 0x0020613A
		public bool IsBitSet(int index)
		{
			return BitArray.IsBitSet(index, this.m_aBlock);
		}

		// Token: 0x04007111 RID: 28945
		private int[] m_aBlock;
	}
}
