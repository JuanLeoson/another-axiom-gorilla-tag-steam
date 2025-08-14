using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000FF2 RID: 4082
	public class Codec
	{
		// Token: 0x060065DD RID: 26077 RVA: 0x0020735C File Offset: 0x0020555C
		public static float PackSaturated(float a, float b)
		{
			a = Mathf.Floor(a * 4095f);
			b = Mathf.Floor(b * 4095f);
			return a * 4096f + b;
		}

		// Token: 0x060065DE RID: 26078 RVA: 0x00207383 File Offset: 0x00205583
		public static float PackSaturated(Vector2 v)
		{
			return Codec.PackSaturated(v.x, v.y);
		}

		// Token: 0x060065DF RID: 26079 RVA: 0x00207396 File Offset: 0x00205596
		public static Vector2 UnpackSaturated(float f)
		{
			return new Vector2(Mathf.Floor(f / 4096f), Mathf.Repeat(f, 4096f)) / 4095f;
		}

		// Token: 0x060065E0 RID: 26080 RVA: 0x002073C0 File Offset: 0x002055C0
		public static Vector2 OctWrap(Vector2 v)
		{
			return (Vector2.one - new Vector2(Mathf.Abs(v.y), Mathf.Abs(v.x))) * new Vector2(Mathf.Sign(v.x), Mathf.Sign(v.y));
		}

		// Token: 0x060065E1 RID: 26081 RVA: 0x00207414 File Offset: 0x00205614
		public static float PackNormal(Vector3 n)
		{
			n /= Mathf.Abs(n.x) + Mathf.Abs(n.y) + Mathf.Abs(n.z);
			return Codec.PackSaturated(((n.z >= 0f) ? new Vector2(n.x, n.y) : Codec.OctWrap(new Vector2(n.x, n.y))) * 0.5f + 0.5f * Vector2.one);
		}

		// Token: 0x060065E2 RID: 26082 RVA: 0x002074A8 File Offset: 0x002056A8
		public static Vector3 UnpackNormal(float f)
		{
			Vector2 vector = Codec.UnpackSaturated(f);
			vector = vector * 2f - Vector2.one;
			Vector3 vector2 = new Vector3(vector.x, vector.y, 1f - Mathf.Abs(vector.x) - Mathf.Abs(vector.y));
			float num = Mathf.Clamp01(-vector2.z);
			vector2.x += ((vector2.x >= 0f) ? (-num) : num);
			vector2.y += ((vector2.y >= 0f) ? (-num) : num);
			return vector2.normalized;
		}

		// Token: 0x060065E3 RID: 26083 RVA: 0x00207550 File Offset: 0x00205750
		public static uint PackRgb(Color color)
		{
			return (uint)(color.b * 255f) << 16 | (uint)(color.g * 255f) << 8 | (uint)(color.r * 255f);
		}

		// Token: 0x060065E4 RID: 26084 RVA: 0x00207580 File Offset: 0x00205780
		public static Color UnpackRgb(uint i)
		{
			return new Color((i & 255U) / 255f, ((i & 65280U) >> 8) / 255f, ((i & 16711680U) >> 16) / 255f);
		}

		// Token: 0x060065E5 RID: 26085 RVA: 0x002075BC File Offset: 0x002057BC
		public static uint PackRgba(Color color)
		{
			return (uint)(color.a * 255f) << 24 | (uint)(color.b * 255f) << 16 | (uint)(color.g * 255f) << 8 | (uint)(color.r * 255f);
		}

		// Token: 0x060065E6 RID: 26086 RVA: 0x00207608 File Offset: 0x00205808
		public static Color UnpackRgba(uint i)
		{
			return new Color((i & 255U) / 255f, ((i & 65280U) >> 8) / 255f, ((i & 16711680U) >> 16) / 255f, ((i & 4278190080U) >> 24) / 255f);
		}

		// Token: 0x060065E7 RID: 26087 RVA: 0x0020765E File Offset: 0x0020585E
		public static uint Pack8888(uint x, uint y, uint z, uint w)
		{
			return (x & 255U) << 24 | (y & 255U) << 16 | (z & 255U) << 8 | (w & 255U);
		}

		// Token: 0x060065E8 RID: 26088 RVA: 0x00207687 File Offset: 0x00205887
		public static void Unpack8888(uint i, out uint x, out uint y, out uint z, out uint w)
		{
			x = (i >> 24 & 255U);
			y = (i >> 16 & 255U);
			z = (i >> 8 & 255U);
			w = (i & 255U);
		}

		// Token: 0x060065E9 RID: 26089 RVA: 0x002076B8 File Offset: 0x002058B8
		private static int IntReinterpret(float f)
		{
			return new Codec.IntFloat
			{
				FloatValue = f
			}.IntValue;
		}

		// Token: 0x060065EA RID: 26090 RVA: 0x002076DB File Offset: 0x002058DB
		public static int HashConcat(int hash, int i)
		{
			return (hash ^ i) * Codec.FnvPrime;
		}

		// Token: 0x060065EB RID: 26091 RVA: 0x002076E6 File Offset: 0x002058E6
		public static int HashConcat(int hash, long i)
		{
			hash = Codec.HashConcat(hash, (int)(i & (long)((ulong)-1)));
			hash = Codec.HashConcat(hash, (int)(i >> 32));
			return hash;
		}

		// Token: 0x060065EC RID: 26092 RVA: 0x00207703 File Offset: 0x00205903
		public static int HashConcat(int hash, float f)
		{
			return Codec.HashConcat(hash, Codec.IntReinterpret(f));
		}

		// Token: 0x060065ED RID: 26093 RVA: 0x00207711 File Offset: 0x00205911
		public static int HashConcat(int hash, bool b)
		{
			return Codec.HashConcat(hash, b ? 1 : 0);
		}

		// Token: 0x060065EE RID: 26094 RVA: 0x00207720 File Offset: 0x00205920
		public static int HashConcat(int hash, params int[] ints)
		{
			foreach (int i2 in ints)
			{
				hash = Codec.HashConcat(hash, i2);
			}
			return hash;
		}

		// Token: 0x060065EF RID: 26095 RVA: 0x0020774C File Offset: 0x0020594C
		public static int HashConcat(int hash, params float[] floats)
		{
			foreach (float f in floats)
			{
				hash = Codec.HashConcat(hash, f);
			}
			return hash;
		}

		// Token: 0x060065F0 RID: 26096 RVA: 0x00207777 File Offset: 0x00205977
		public static int HashConcat(int hash, Vector2 v)
		{
			return Codec.HashConcat(hash, new float[]
			{
				v.x,
				v.y
			});
		}

		// Token: 0x060065F1 RID: 26097 RVA: 0x00207797 File Offset: 0x00205997
		public static int HashConcat(int hash, Vector3 v)
		{
			return Codec.HashConcat(hash, new float[]
			{
				v.x,
				v.y,
				v.z
			});
		}

		// Token: 0x060065F2 RID: 26098 RVA: 0x002077C0 File Offset: 0x002059C0
		public static int HashConcat(int hash, Vector4 v)
		{
			return Codec.HashConcat(hash, new float[]
			{
				v.x,
				v.y,
				v.z,
				v.w
			});
		}

		// Token: 0x060065F3 RID: 26099 RVA: 0x002077F2 File Offset: 0x002059F2
		public static int HashConcat(int hash, Quaternion q)
		{
			return Codec.HashConcat(hash, new float[]
			{
				q.x,
				q.y,
				q.z,
				q.w
			});
		}

		// Token: 0x060065F4 RID: 26100 RVA: 0x00207824 File Offset: 0x00205A24
		public static int HashConcat(int hash, Color c)
		{
			return Codec.HashConcat(hash, new float[]
			{
				c.r,
				c.g,
				c.b,
				c.a
			});
		}

		// Token: 0x060065F5 RID: 26101 RVA: 0x00207856 File Offset: 0x00205A56
		public static int HashConcat(int hash, Transform t)
		{
			return Codec.HashConcat(hash, t.GetHashCode());
		}

		// Token: 0x060065F6 RID: 26102 RVA: 0x00207864 File Offset: 0x00205A64
		public static int Hash(int i)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, i);
		}

		// Token: 0x060065F7 RID: 26103 RVA: 0x00207871 File Offset: 0x00205A71
		public static int Hash(long i)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, i);
		}

		// Token: 0x060065F8 RID: 26104 RVA: 0x0020787E File Offset: 0x00205A7E
		public static int Hash(float f)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, f);
		}

		// Token: 0x060065F9 RID: 26105 RVA: 0x0020788B File Offset: 0x00205A8B
		public static int Hash(bool b)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, b);
		}

		// Token: 0x060065FA RID: 26106 RVA: 0x00207898 File Offset: 0x00205A98
		public static int Hash(params int[] ints)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, ints);
		}

		// Token: 0x060065FB RID: 26107 RVA: 0x002078A5 File Offset: 0x00205AA5
		public static int Hash(params float[] floats)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, floats);
		}

		// Token: 0x060065FC RID: 26108 RVA: 0x002078B2 File Offset: 0x00205AB2
		public static int Hash(Vector2 v)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, v);
		}

		// Token: 0x060065FD RID: 26109 RVA: 0x002078BF File Offset: 0x00205ABF
		public static int Hash(Vector3 v)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, v);
		}

		// Token: 0x060065FE RID: 26110 RVA: 0x002078CC File Offset: 0x00205ACC
		public static int Hash(Vector4 v)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, v);
		}

		// Token: 0x060065FF RID: 26111 RVA: 0x002078D9 File Offset: 0x00205AD9
		public static int Hash(Quaternion q)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, q);
		}

		// Token: 0x06006600 RID: 26112 RVA: 0x002078E6 File Offset: 0x00205AE6
		public static int Hash(Color c)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, c);
		}

		// Token: 0x06006601 RID: 26113 RVA: 0x002078F4 File Offset: 0x00205AF4
		private static int HashTransformHierarchyRecurvsive(int hash, Transform t)
		{
			hash = Codec.HashConcat(hash, t);
			hash = Codec.HashConcat(hash, t.childCount);
			for (int i = 0; i < t.childCount; i++)
			{
				hash = Codec.HashTransformHierarchyRecurvsive(hash, t.GetChild(i));
			}
			return hash;
		}

		// Token: 0x06006602 RID: 26114 RVA: 0x00207939 File Offset: 0x00205B39
		public static int HashTransformHierarchy(Transform t)
		{
			return Codec.HashTransformHierarchyRecurvsive(Codec.FnvDefaultBasis, t);
		}

		// Token: 0x04007101 RID: 28929
		public static readonly int FnvDefaultBasis = -2128831035;

		// Token: 0x04007102 RID: 28930
		public static readonly int FnvPrime = 16777619;

		// Token: 0x02000FF3 RID: 4083
		[StructLayout(LayoutKind.Explicit)]
		private struct IntFloat
		{
			// Token: 0x04007103 RID: 28931
			[FieldOffset(0)]
			public int IntValue;

			// Token: 0x04007104 RID: 28932
			[FieldOffset(0)]
			public float FloatValue;
		}
	}
}
