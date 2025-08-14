using System;
using emotitron.Compression.HalfFloat;
using UnityEngine;

namespace emotitron.Compression
{
	// Token: 0x02000F7D RID: 3965
	[Serializable]
	public class LiteFloatCrusher : LiteCrusher<float>
	{
		// Token: 0x0600630A RID: 25354 RVA: 0x001F4038 File Offset: 0x001F2238
		public LiteFloatCrusher()
		{
			this.compressType = LiteFloatCompressType.Half16;
			this.min = 0f;
			this.max = 1f;
			this.accurateCenter = true;
			LiteFloatCrusher.Recalculate(this.compressType, this.min, this.max, this.accurateCenter, ref this.bits, ref this.encoder, ref this.decoder, ref this.maxCVal);
		}

		// Token: 0x0600630B RID: 25355 RVA: 0x001F40B4 File Offset: 0x001F22B4
		public LiteFloatCrusher(LiteFloatCompressType compressType, float min, float max, bool accurateCenter)
		{
			this.compressType = compressType;
			this.min = min;
			this.max = max;
			this.accurateCenter = accurateCenter;
			LiteFloatCrusher.Recalculate(compressType, min, max, accurateCenter, ref this.bits, ref this.encoder, ref this.decoder, ref this.maxCVal);
		}

		// Token: 0x0600630C RID: 25356 RVA: 0x001F4118 File Offset: 0x001F2318
		public static void Recalculate(LiteFloatCompressType compressType, float min, float max, bool accurateCenter, ref int bits, ref float encoder, ref float decoder, ref ulong maxCVal)
		{
			bits = (int)compressType;
			float num = max - min;
			ulong num2 = (bits == 64) ? ulong.MaxValue : ((1UL << bits) - 1UL);
			if (accurateCenter && num2 != 0UL)
			{
				num2 -= 1UL;
			}
			encoder = ((num == 0f) ? 0f : (num2 / num));
			decoder = ((num2 == 0UL) ? 0f : (num / num2));
			maxCVal = num2;
		}

		// Token: 0x0600630D RID: 25357 RVA: 0x001F4180 File Offset: 0x001F2380
		public override ulong Encode(float val)
		{
			if (this.compressType == LiteFloatCompressType.Half16)
			{
				return (ulong)HalfUtilities.Pack(val);
			}
			if (this.compressType == LiteFloatCompressType.Full32)
			{
				return (ulong)val.uint32;
			}
			float num = (val - this.min) * this.encoder + 0.5f;
			if (num < 0f)
			{
				return 0UL;
			}
			ulong num2 = (ulong)num;
			if (num2 <= this.maxCVal)
			{
				return num2;
			}
			return this.maxCVal;
		}

		// Token: 0x0600630E RID: 25358 RVA: 0x001F41EC File Offset: 0x001F23EC
		public override float Decode(uint cval)
		{
			if (this.compressType == LiteFloatCompressType.Half16)
			{
				return HalfUtilities.Unpack((ushort)cval);
			}
			if (this.compressType == LiteFloatCompressType.Full32)
			{
				return cval.float32;
			}
			if (cval == 0U)
			{
				return this.min;
			}
			if ((ulong)cval == this.maxCVal)
			{
				return this.max;
			}
			return cval * this.decoder + this.min;
		}

		// Token: 0x0600630F RID: 25359 RVA: 0x001F4250 File Offset: 0x001F2450
		public override ulong WriteValue(float val, byte[] buffer, ref int bitposition)
		{
			if (this.compressType == LiteFloatCompressType.Half16)
			{
				ulong num = (ulong)HalfUtilities.Pack(val);
				buffer.Write(num, ref bitposition, 16);
				return num;
			}
			if (this.compressType == LiteFloatCompressType.Full32)
			{
				ulong num2 = (ulong)val.uint32;
				buffer.Write(num2, ref bitposition, 32);
				return num2;
			}
			ulong num3 = this.Encode(val);
			buffer.Write(num3, ref bitposition, (int)this.compressType);
			return num3;
		}

		// Token: 0x06006310 RID: 25360 RVA: 0x001F42B5 File Offset: 0x001F24B5
		public override void WriteCValue(uint cval, byte[] buffer, ref int bitposition)
		{
			if (this.compressType == LiteFloatCompressType.Half16)
			{
				buffer.Write((ulong)cval, ref bitposition, 16);
				return;
			}
			if (this.compressType == LiteFloatCompressType.Full32)
			{
				buffer.Write((ulong)cval, ref bitposition, 32);
				return;
			}
			buffer.Write((ulong)cval, ref bitposition, (int)this.compressType);
		}

		// Token: 0x06006311 RID: 25361 RVA: 0x001F42F4 File Offset: 0x001F24F4
		public override float ReadValue(byte[] buffer, ref int bitposition)
		{
			if (this.compressType == LiteFloatCompressType.Half16)
			{
				return HalfUtilities.Unpack((ushort)buffer.Read(ref bitposition, 16));
			}
			if (this.compressType == LiteFloatCompressType.Full32)
			{
				return ((uint)buffer.Read(ref bitposition, 32)).float32;
			}
			uint val = (uint)buffer.Read(ref bitposition, (int)this.compressType);
			return this.Decode(val);
		}

		// Token: 0x06006312 RID: 25362 RVA: 0x001F4350 File Offset: 0x001F2550
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
				" e: ",
				this.encoder.ToString(),
				" d: ",
				this.decoder.ToString()
			});
		}

		// Token: 0x04006E38 RID: 28216
		[SerializeField]
		protected float min;

		// Token: 0x04006E39 RID: 28217
		[SerializeField]
		protected float max;

		// Token: 0x04006E3A RID: 28218
		[SerializeField]
		public LiteFloatCompressType compressType = LiteFloatCompressType.Half16;

		// Token: 0x04006E3B RID: 28219
		[SerializeField]
		private bool accurateCenter = true;

		// Token: 0x04006E3C RID: 28220
		[SerializeField]
		private float encoder;

		// Token: 0x04006E3D RID: 28221
		[SerializeField]
		private float decoder;

		// Token: 0x04006E3E RID: 28222
		[SerializeField]
		private ulong maxCVal;
	}
}
