using System;
using System.Runtime.InteropServices;

namespace emotitron.Compression.Utilities
{
	// Token: 0x02000F81 RID: 3969
	[StructLayout(LayoutKind.Explicit)]
	public struct ByteConverter
	{
		// Token: 0x17000966 RID: 2406
		public byte this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return this.byte0;
				case 1:
					return this.byte1;
				case 2:
					return this.byte2;
				case 3:
					return this.byte3;
				case 4:
					return this.byte4;
				case 5:
					return this.byte5;
				case 6:
					return this.byte6;
				case 7:
					return this.byte7;
				default:
					return 0;
				}
			}
		}

		// Token: 0x06006325 RID: 25381 RVA: 0x001F4758 File Offset: 0x001F2958
		public static implicit operator ByteConverter(byte[] bytes)
		{
			ByteConverter result = default(ByteConverter);
			int num = bytes.Length;
			result.byte0 = bytes[0];
			if (num > 0)
			{
				result.byte1 = bytes[1];
			}
			if (num > 1)
			{
				result.byte2 = bytes[2];
			}
			if (num > 2)
			{
				result.byte3 = bytes[3];
			}
			if (num > 3)
			{
				result.byte4 = bytes[4];
			}
			if (num > 4)
			{
				result.byte5 = bytes[5];
			}
			if (num > 5)
			{
				result.byte6 = bytes[3];
			}
			if (num > 6)
			{
				result.byte7 = bytes[7];
			}
			return result;
		}

		// Token: 0x06006326 RID: 25382 RVA: 0x001F47DC File Offset: 0x001F29DC
		public static implicit operator ByteConverter(byte val)
		{
			return new ByteConverter
			{
				byte0 = val
			};
		}

		// Token: 0x06006327 RID: 25383 RVA: 0x001F47FC File Offset: 0x001F29FC
		public static implicit operator ByteConverter(sbyte val)
		{
			return new ByteConverter
			{
				int8 = val
			};
		}

		// Token: 0x06006328 RID: 25384 RVA: 0x001F481C File Offset: 0x001F2A1C
		public static implicit operator ByteConverter(char val)
		{
			return new ByteConverter
			{
				character = val
			};
		}

		// Token: 0x06006329 RID: 25385 RVA: 0x001F483C File Offset: 0x001F2A3C
		public static implicit operator ByteConverter(uint val)
		{
			return new ByteConverter
			{
				uint32 = val
			};
		}

		// Token: 0x0600632A RID: 25386 RVA: 0x001F485C File Offset: 0x001F2A5C
		public static implicit operator ByteConverter(int val)
		{
			return new ByteConverter
			{
				int32 = val
			};
		}

		// Token: 0x0600632B RID: 25387 RVA: 0x001F487C File Offset: 0x001F2A7C
		public static implicit operator ByteConverter(ulong val)
		{
			return new ByteConverter
			{
				uint64 = val
			};
		}

		// Token: 0x0600632C RID: 25388 RVA: 0x001F489C File Offset: 0x001F2A9C
		public static implicit operator ByteConverter(long val)
		{
			return new ByteConverter
			{
				int64 = val
			};
		}

		// Token: 0x0600632D RID: 25389 RVA: 0x001F48BC File Offset: 0x001F2ABC
		public static implicit operator ByteConverter(float val)
		{
			return new ByteConverter
			{
				float32 = val
			};
		}

		// Token: 0x0600632E RID: 25390 RVA: 0x001F48DC File Offset: 0x001F2ADC
		public static implicit operator ByteConverter(double val)
		{
			return new ByteConverter
			{
				float64 = val
			};
		}

		// Token: 0x0600632F RID: 25391 RVA: 0x001F48FC File Offset: 0x001F2AFC
		public static implicit operator ByteConverter(bool val)
		{
			return new ByteConverter
			{
				int32 = (val ? 1 : 0)
			};
		}

		// Token: 0x06006330 RID: 25392 RVA: 0x001F4920 File Offset: 0x001F2B20
		public void ExtractByteArray(byte[] targetArray)
		{
			int num = targetArray.Length;
			targetArray[0] = this.byte0;
			if (num > 0)
			{
				targetArray[1] = this.byte1;
			}
			if (num > 1)
			{
				targetArray[2] = this.byte2;
			}
			if (num > 2)
			{
				targetArray[3] = this.byte3;
			}
			if (num > 3)
			{
				targetArray[4] = this.byte4;
			}
			if (num > 4)
			{
				targetArray[5] = this.byte5;
			}
			if (num > 5)
			{
				targetArray[6] = this.byte6;
			}
			if (num > 6)
			{
				targetArray[7] = this.byte7;
			}
		}

		// Token: 0x06006331 RID: 25393 RVA: 0x001F4993 File Offset: 0x001F2B93
		public static implicit operator byte(ByteConverter bc)
		{
			return bc.byte0;
		}

		// Token: 0x06006332 RID: 25394 RVA: 0x001F499B File Offset: 0x001F2B9B
		public static implicit operator sbyte(ByteConverter bc)
		{
			return bc.int8;
		}

		// Token: 0x06006333 RID: 25395 RVA: 0x001F49A3 File Offset: 0x001F2BA3
		public static implicit operator char(ByteConverter bc)
		{
			return bc.character;
		}

		// Token: 0x06006334 RID: 25396 RVA: 0x001F49AB File Offset: 0x001F2BAB
		public static implicit operator ushort(ByteConverter bc)
		{
			return bc.uint16;
		}

		// Token: 0x06006335 RID: 25397 RVA: 0x001F49B3 File Offset: 0x001F2BB3
		public static implicit operator short(ByteConverter bc)
		{
			return bc.int16;
		}

		// Token: 0x06006336 RID: 25398 RVA: 0x001F49BB File Offset: 0x001F2BBB
		public static implicit operator uint(ByteConverter bc)
		{
			return bc.uint32;
		}

		// Token: 0x06006337 RID: 25399 RVA: 0x001F49C3 File Offset: 0x001F2BC3
		public static implicit operator int(ByteConverter bc)
		{
			return bc.int32;
		}

		// Token: 0x06006338 RID: 25400 RVA: 0x001F49CB File Offset: 0x001F2BCB
		public static implicit operator ulong(ByteConverter bc)
		{
			return bc.uint64;
		}

		// Token: 0x06006339 RID: 25401 RVA: 0x001F49D3 File Offset: 0x001F2BD3
		public static implicit operator long(ByteConverter bc)
		{
			return bc.int64;
		}

		// Token: 0x0600633A RID: 25402 RVA: 0x001F49DB File Offset: 0x001F2BDB
		public static implicit operator float(ByteConverter bc)
		{
			return bc.float32;
		}

		// Token: 0x0600633B RID: 25403 RVA: 0x001F49E3 File Offset: 0x001F2BE3
		public static implicit operator double(ByteConverter bc)
		{
			return bc.float64;
		}

		// Token: 0x0600633C RID: 25404 RVA: 0x001F49EB File Offset: 0x001F2BEB
		public static implicit operator bool(ByteConverter bc)
		{
			return bc.int32 != 0;
		}

		// Token: 0x04006E48 RID: 28232
		[FieldOffset(0)]
		public float float32;

		// Token: 0x04006E49 RID: 28233
		[FieldOffset(0)]
		public double float64;

		// Token: 0x04006E4A RID: 28234
		[FieldOffset(0)]
		public sbyte int8;

		// Token: 0x04006E4B RID: 28235
		[FieldOffset(0)]
		public short int16;

		// Token: 0x04006E4C RID: 28236
		[FieldOffset(0)]
		public ushort uint16;

		// Token: 0x04006E4D RID: 28237
		[FieldOffset(0)]
		public char character;

		// Token: 0x04006E4E RID: 28238
		[FieldOffset(0)]
		public int int32;

		// Token: 0x04006E4F RID: 28239
		[FieldOffset(0)]
		public uint uint32;

		// Token: 0x04006E50 RID: 28240
		[FieldOffset(0)]
		public long int64;

		// Token: 0x04006E51 RID: 28241
		[FieldOffset(0)]
		public ulong uint64;

		// Token: 0x04006E52 RID: 28242
		[FieldOffset(0)]
		public byte byte0;

		// Token: 0x04006E53 RID: 28243
		[FieldOffset(1)]
		public byte byte1;

		// Token: 0x04006E54 RID: 28244
		[FieldOffset(2)]
		public byte byte2;

		// Token: 0x04006E55 RID: 28245
		[FieldOffset(3)]
		public byte byte3;

		// Token: 0x04006E56 RID: 28246
		[FieldOffset(4)]
		public byte byte4;

		// Token: 0x04006E57 RID: 28247
		[FieldOffset(5)]
		public byte byte5;

		// Token: 0x04006E58 RID: 28248
		[FieldOffset(6)]
		public byte byte6;

		// Token: 0x04006E59 RID: 28249
		[FieldOffset(7)]
		public byte byte7;

		// Token: 0x04006E5A RID: 28250
		[FieldOffset(4)]
		public uint uint16_B;
	}
}
