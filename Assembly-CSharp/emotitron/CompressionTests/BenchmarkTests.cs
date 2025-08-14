using System;
using System.Diagnostics;
using emotitron.Compression;
using UnityEngine;

namespace emotitron.CompressionTests
{
	// Token: 0x02000F85 RID: 3973
	public class BenchmarkTests : MonoBehaviour
	{
		// Token: 0x0600636B RID: 25451 RVA: 0x001F524D File Offset: 0x001F344D
		private void Start()
		{
			BenchmarkTests.TestWriterIntegrity();
			BenchmarkTests.ArrayCopy();
			BenchmarkTests.ArrayCopySafe();
		}

		// Token: 0x0600636C RID: 25452 RVA: 0x001F5260 File Offset: 0x001F3460
		public static void TestWriterIntegrity()
		{
			int num = 1;
			int num2 = 1;
			BenchmarkTests.ubuffer.Write(ulong.MaxValue, ref num, 64);
			if (BenchmarkTests.ubuffer.Read(ref num2, 64) != 18446744073709551615UL)
			{
				Debug.Log("Error writing with maxulong");
			}
			for (int i = 0; i < 3000; i++)
			{
				num = Random.Range(0, 200);
				num2 = num;
				int num3 = num;
				int num4 = Random.Range(1, 64);
				int num5 = Random.Range(-(1 << num4 - 1), (1 << num4 - 1) - 1);
				BenchmarkTests.ubuffer.WriteSigned(num5, ref num, num4);
				BenchmarkTests.ubuffer.WriteSigned(num5, ref num, num4);
				BenchmarkTests.ubuffer.WriteSigned(num5, ref num, num4);
				if (BenchmarkTests.ubuffer.ReadSigned(ref num2, num4) != num5)
				{
					Debug.Log(string.Concat(new string[]
					{
						"Error writing ",
						num5.ToString(),
						" to pos ",
						num3.ToString(),
						" with size ",
						num4.ToString()
					}));
				}
				if (BenchmarkTests.ubuffer.ReadSigned(ref num2, num4) != num5)
				{
					Debug.Log(string.Concat(new string[]
					{
						"Error writing ",
						num5.ToString(),
						" to pos ",
						num3.ToString(),
						" with size ",
						num4.ToString()
					}));
				}
				if (BenchmarkTests.ubuffer.ReadSigned(ref num2, num4) != num5)
				{
					Debug.Log(string.Concat(new string[]
					{
						"Error writing ",
						num5.ToString(),
						" to pos ",
						num3.ToString(),
						" with size ",
						num4.ToString()
					}));
				}
				ulong num6 = (ulong)Random.Range(0f, (1L << num4) - 1L);
				BenchmarkTests.ubuffer.Write(num6, ref num, num4);
				BenchmarkTests.ubuffer.Write(num6, ref num, num4);
				BenchmarkTests.ubuffer.Write(num6, ref num, num4);
				if (BenchmarkTests.ubuffer.Read(ref num2, num4) != num6)
				{
					Debug.Log(string.Concat(new string[]
					{
						"Error writing ",
						num6.ToString(),
						" to pos ",
						num3.ToString(),
						" with size ",
						num4.ToString()
					}));
				}
				if (BenchmarkTests.ubuffer.Read(ref num2, num4) != num6)
				{
					Debug.Log(string.Concat(new string[]
					{
						"Error writing ",
						num6.ToString(),
						" to pos ",
						num3.ToString(),
						" with size ",
						num4.ToString()
					}));
				}
				if (BenchmarkTests.ubuffer.Read(ref num2, num4) != num6)
				{
					Debug.Log(string.Concat(new string[]
					{
						"Error writing ",
						num6.ToString(),
						" to pos ",
						num3.ToString(),
						" with size ",
						num4.ToString()
					}));
				}
			}
			Debug.Log("Integrity check complete.");
		}

		// Token: 0x0600636D RID: 25453 RVA: 0x001F5580 File Offset: 0x001F3780
		private static void TestLog2()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (uint num = 0U; num <= 4294967295U; num += 3000U)
			{
				num.UsedBitCount();
				num.UsedBitCount();
				num.UsedBitCount();
				num.UsedBitCount();
				num.UsedBitCount();
				if (4294967295U - num < 4000U)
				{
					break;
				}
			}
			stopwatch.Stop();
			Debug.Log("Log2 nifty: time=" + stopwatch.ElapsedMilliseconds.ToString() + " ms");
		}

		// Token: 0x0600636E RID: 25454 RVA: 0x001F55F8 File Offset: 0x001F37F8
		private static void ArrayCopy()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < 1000000; i++)
			{
				int num = 0;
				BenchmarkTests.ubuffer.ReadOutUnsafe(0, BenchmarkTests.buffer, ref num, 960);
			}
			stopwatch.Stop();
			Debug.Log("Array Copy Unsafe: time=" + stopwatch.ElapsedMilliseconds.ToString() + " ms");
		}

		// Token: 0x0600636F RID: 25455 RVA: 0x001F565C File Offset: 0x001F385C
		private static void ArrayCopySafe()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < 1000000; i++)
			{
				int num = 0;
				BenchmarkTests.ubuffer.ReadOutSafe(0, BenchmarkTests.buffer, ref num, 960);
			}
			stopwatch.Stop();
			Debug.Log("Array Copy Safe: time=" + stopwatch.ElapsedMilliseconds.ToString() + " ms");
		}

		// Token: 0x06006370 RID: 25456 RVA: 0x001F56C0 File Offset: 0x001F38C0
		public static void ByteForByteWrite()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < 1000000; i++)
			{
				BasicWriter.Reset();
				for (int j = 0; j < 128; j++)
				{
					BasicWriter.BasicWrite(BenchmarkTests.buffer, byte.MaxValue);
				}
				BasicWriter.Reset();
				for (int k = 0; k < 128; k++)
				{
					BasicWriter.BasicRead(BenchmarkTests.buffer);
				}
			}
			stopwatch.Stop();
			Debug.Log("Byte For Byte: time=" + stopwatch.ElapsedMilliseconds.ToString() + " ms");
		}

		// Token: 0x06006371 RID: 25457 RVA: 0x001F5754 File Offset: 0x001F3954
		public static void BitpackBytesEven()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < 1000000; i++)
			{
				int num = 0;
				for (int j = 0; j < 128; j++)
				{
					BenchmarkTests.buffer.Write(255UL, ref num, 8);
				}
				num = 0;
				for (int k = 0; k < 127; k++)
				{
					BenchmarkTests.buffer.Read(ref num, 8);
				}
			}
			stopwatch.Stop();
			Debug.Log("Even Bitpack byte: time=" + stopwatch.ElapsedMilliseconds.ToString() + " ms");
		}

		// Token: 0x06006372 RID: 25458 RVA: 0x001F57E8 File Offset: 0x001F39E8
		public static void BitpackBytesToULongUneven()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < 1000000; i++)
			{
				int num = 0;
				BenchmarkTests.ubuffer.Write(1UL, ref num, 1);
				for (int j = 0; j < 127; j++)
				{
					BenchmarkTests.ubuffer.Write(255UL, ref num, 33);
				}
				num = 0;
				BenchmarkTests.ubuffer.Read(ref num, 1);
				for (int k = 0; k < 127; k++)
				{
					BenchmarkTests.ubuffer.Read(ref num, 33);
				}
			}
			stopwatch.Stop();
			Debug.Log("Uneven Bitpack ulong[]: time=" + stopwatch.ElapsedMilliseconds.ToString() + " ms");
		}

		// Token: 0x06006373 RID: 25459 RVA: 0x001F5898 File Offset: 0x001F3A98
		public static void BitpackBytesUnEven()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < 1000000; i++)
			{
				int num = 0;
				BenchmarkTests.buffer.Write(1UL, ref num, 1);
				for (int j = 0; j < 127; j++)
				{
					BenchmarkTests.buffer.Write(255UL, ref num, 8);
				}
				num = 0;
				BenchmarkTests.buffer.Read(ref num, 1);
				for (int k = 0; k < 127; k++)
				{
					BenchmarkTests.buffer.Read(ref num, 8);
				}
			}
			stopwatch.Stop();
			Debug.Log("Uneven Bitpack byte: time=" + stopwatch.ElapsedMilliseconds.ToString() + " ms");
		}

		// Token: 0x04006E71 RID: 28273
		public const int BYTE_CNT = 128;

		// Token: 0x04006E72 RID: 28274
		public const int LOOP = 1000000;

		// Token: 0x04006E73 RID: 28275
		public static byte[] buffer = new byte[4800];

		// Token: 0x04006E74 RID: 28276
		public static uint[] ibuffer = new uint[128];

		// Token: 0x04006E75 RID: 28277
		public static ulong[] ubuffer = new ulong[128];

		// Token: 0x04006E76 RID: 28278
		public static ulong[] ubuffer2 = new ulong[128];
	}
}
