using System;
using UnityEngine;

namespace emotitron.Compression
{
	// Token: 0x02000F7F RID: 3967
	[Serializable]
	public class LiteIntCrusher : LiteCrusher<int>
	{
		// Token: 0x06006313 RID: 25363 RVA: 0x001F43F0 File Offset: 0x001F25F0
		public LiteIntCrusher()
		{
			this.compressType = LiteIntCompressType.PackSigned;
			this.min = -128;
			this.max = 127;
			if (this.compressType == LiteIntCompressType.Range)
			{
				LiteIntCrusher.Recalculate(this.min, this.max, ref this.smallest, ref this.biggest, ref this.bits);
			}
		}

		// Token: 0x06006314 RID: 25364 RVA: 0x001F4446 File Offset: 0x001F2646
		public LiteIntCrusher(LiteIntCompressType comType = LiteIntCompressType.PackSigned, int min = -128, int max = 127)
		{
			this.compressType = comType;
			this.min = min;
			this.max = max;
			if (this.compressType == LiteIntCompressType.Range)
			{
				LiteIntCrusher.Recalculate(min, max, ref this.smallest, ref this.biggest, ref this.bits);
			}
		}

		// Token: 0x06006315 RID: 25365 RVA: 0x001F4488 File Offset: 0x001F2688
		public override ulong WriteValue(int val, byte[] buffer, ref int bitposition)
		{
			switch (this.compressType)
			{
			case LiteIntCompressType.PackSigned:
			{
				uint num = (uint)(val << 1 ^ val >> 31);
				buffer.WritePackedBytes((ulong)num, ref bitposition, 32);
				return (ulong)num;
			}
			case LiteIntCompressType.PackUnsigned:
				buffer.WritePackedBytes((ulong)val, ref bitposition, 32);
				return (ulong)val;
			case LiteIntCompressType.Range:
			{
				ulong num2 = this.Encode(val);
				buffer.Write(num2, ref bitposition, this.bits);
				return num2;
			}
			default:
				return 0UL;
			}
		}

		// Token: 0x06006316 RID: 25366 RVA: 0x001F44F4 File Offset: 0x001F26F4
		public override void WriteCValue(uint cval, byte[] buffer, ref int bitposition)
		{
			switch (this.compressType)
			{
			case LiteIntCompressType.PackSigned:
				buffer.WritePackedBytes((ulong)cval, ref bitposition, 32);
				return;
			case LiteIntCompressType.PackUnsigned:
				buffer.WritePackedBytes((ulong)cval, ref bitposition, 32);
				return;
			case LiteIntCompressType.Range:
				buffer.Write((ulong)cval, ref bitposition, this.bits);
				return;
			default:
				return;
			}
		}

		// Token: 0x06006317 RID: 25367 RVA: 0x001F4544 File Offset: 0x001F2744
		public override int ReadValue(byte[] buffer, ref int bitposition)
		{
			switch (this.compressType)
			{
			case LiteIntCompressType.PackSigned:
				return buffer.ReadSignedPackedBytes(ref bitposition, 32);
			case LiteIntCompressType.PackUnsigned:
				return (int)buffer.ReadPackedBytes(ref bitposition, 32);
			case LiteIntCompressType.Range:
			{
				uint val = (uint)buffer.Read(ref bitposition, this.bits);
				return this.Decode(val);
			}
			default:
				return 0;
			}
		}

		// Token: 0x06006318 RID: 25368 RVA: 0x001F4599 File Offset: 0x001F2799
		public override ulong Encode(int value)
		{
			value = ((value > this.biggest) ? this.biggest : ((value < this.smallest) ? this.smallest : value));
			return (ulong)((long)(value - this.smallest));
		}

		// Token: 0x06006319 RID: 25369 RVA: 0x001F45C9 File Offset: 0x001F27C9
		public override int Decode(uint cvalue)
		{
			return (int)((ulong)cvalue + (ulong)((long)this.smallest));
		}

		// Token: 0x0600631A RID: 25370 RVA: 0x001F45D8 File Offset: 0x001F27D8
		public static void Recalculate(int min, int max, ref int smallest, ref int biggest, ref int bits)
		{
			if (min < max)
			{
				smallest = min;
				biggest = max;
			}
			else
			{
				smallest = max;
				biggest = min;
			}
			int maxvalue = biggest - smallest;
			bits = LiteCrusher.GetBitsForMaxValue((uint)maxvalue);
		}

		// Token: 0x0600631B RID: 25371 RVA: 0x001F4608 File Offset: 0x001F2808
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				base.GetType().Name,
				" ",
				this.compressType.ToString(),
				" mn: ",
				this.min.ToString(),
				" mx: ",
				this.max.ToString(),
				" sm: ",
				this.smallest.ToString()
			});
		}

		// Token: 0x04006E43 RID: 28227
		[SerializeField]
		public LiteIntCompressType compressType;

		// Token: 0x04006E44 RID: 28228
		[SerializeField]
		protected int min;

		// Token: 0x04006E45 RID: 28229
		[SerializeField]
		protected int max;

		// Token: 0x04006E46 RID: 28230
		[SerializeField]
		private int smallest;

		// Token: 0x04006E47 RID: 28231
		[SerializeField]
		private int biggest;
	}
}
