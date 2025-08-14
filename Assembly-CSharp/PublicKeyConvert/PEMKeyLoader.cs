using System;
using System.IO;
using System.Security.Cryptography;

namespace PublicKeyConvert
{
	// Token: 0x02000B64 RID: 2916
	public class PEMKeyLoader
	{
		// Token: 0x060045DA RID: 17882 RVA: 0x0015C66C File Offset: 0x0015A86C
		private static bool CompareBytearrays(byte[] a, byte[] b)
		{
			if (a.Length != b.Length)
			{
				return false;
			}
			int num = 0;
			for (int i = 0; i < a.Length; i++)
			{
				if (a[i] != b[num])
				{
					return false;
				}
				num++;
			}
			return true;
		}

		// Token: 0x060045DB RID: 17883 RVA: 0x0015C6A4 File Offset: 0x0015A8A4
		public static RSACryptoServiceProvider CryptoServiceProviderFromPublicKeyInfo(byte[] x509key)
		{
			new byte[15];
			if (x509key == null || x509key.Length == 0)
			{
				return null;
			}
			BinaryReader binaryReader = new BinaryReader(new MemoryStream(x509key));
			RSACryptoServiceProvider result;
			try
			{
				ushort num = binaryReader.ReadUInt16();
				if (num == 33072)
				{
					binaryReader.ReadByte();
				}
				else
				{
					if (num != 33328)
					{
						return null;
					}
					binaryReader.ReadInt16();
				}
				if (!PEMKeyLoader.CompareBytearrays(binaryReader.ReadBytes(15), PEMKeyLoader.SeqOID))
				{
					result = null;
				}
				else
				{
					num = binaryReader.ReadUInt16();
					if (num == 33027)
					{
						binaryReader.ReadByte();
					}
					else
					{
						if (num != 33283)
						{
							return null;
						}
						binaryReader.ReadInt16();
					}
					if (binaryReader.ReadByte() != 0)
					{
						result = null;
					}
					else
					{
						num = binaryReader.ReadUInt16();
						if (num == 33072)
						{
							binaryReader.ReadByte();
						}
						else
						{
							if (num != 33328)
							{
								return null;
							}
							binaryReader.ReadInt16();
						}
						num = binaryReader.ReadUInt16();
						byte b = 0;
						byte b2;
						if (num == 33026)
						{
							b2 = binaryReader.ReadByte();
						}
						else
						{
							if (num != 33282)
							{
								return null;
							}
							b = binaryReader.ReadByte();
							b2 = binaryReader.ReadByte();
						}
						byte[] array = new byte[4];
						array[0] = b2;
						array[1] = b;
						int num2 = BitConverter.ToInt32(array, 0);
						if (binaryReader.PeekChar() == 0)
						{
							binaryReader.ReadByte();
							num2--;
						}
						byte[] modulus = binaryReader.ReadBytes(num2);
						if (binaryReader.ReadByte() != 2)
						{
							result = null;
						}
						else
						{
							int count = (int)binaryReader.ReadByte();
							byte[] exponent = binaryReader.ReadBytes(count);
							RSACryptoServiceProvider rsacryptoServiceProvider = new RSACryptoServiceProvider();
							rsacryptoServiceProvider.ImportParameters(new RSAParameters
							{
								Modulus = modulus,
								Exponent = exponent
							});
							result = rsacryptoServiceProvider;
						}
					}
				}
			}
			finally
			{
				binaryReader.Close();
			}
			return result;
		}

		// Token: 0x060045DC RID: 17884 RVA: 0x0015C870 File Offset: 0x0015AA70
		public static RSACryptoServiceProvider CryptoServiceProviderFromPublicKeyInfo(string base64EncodedKey)
		{
			try
			{
				return PEMKeyLoader.CryptoServiceProviderFromPublicKeyInfo(Convert.FromBase64String(base64EncodedKey));
			}
			catch (FormatException)
			{
			}
			return null;
		}

		// Token: 0x040050D7 RID: 20695
		private static byte[] SeqOID = new byte[]
		{
			48,
			13,
			6,
			9,
			42,
			134,
			72,
			134,
			247,
			13,
			1,
			1,
			1,
			5,
			0
		};
	}
}
