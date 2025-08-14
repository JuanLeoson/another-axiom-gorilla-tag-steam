using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200088D RID: 2189
[Serializable]
public struct SRand
{
	// Token: 0x060036E0 RID: 14048 RVA: 0x0011E5E3 File Offset: 0x0011C7E3
	public SRand(int seed)
	{
		this._seed = (uint)seed;
		this._state = this._seed;
	}

	// Token: 0x060036E1 RID: 14049 RVA: 0x0011E5E3 File Offset: 0x0011C7E3
	public SRand(uint seed)
	{
		this._seed = seed;
		this._state = this._seed;
	}

	// Token: 0x060036E2 RID: 14050 RVA: 0x0011E5F8 File Offset: 0x0011C7F8
	public SRand(long seed)
	{
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x060036E3 RID: 14051 RVA: 0x0011E612 File Offset: 0x0011C812
	public SRand(DateTime seed)
	{
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x060036E4 RID: 14052 RVA: 0x0011E62C File Offset: 0x0011C82C
	public SRand(string seed)
	{
		if (string.IsNullOrEmpty(seed))
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x060036E5 RID: 14053 RVA: 0x0011E65E File Offset: 0x0011C85E
	public SRand(byte[] seed)
	{
		if (seed == null || seed.Length == 0)
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x060036E6 RID: 14054 RVA: 0x0011E68F File Offset: 0x0011C88F
	public double NextDouble()
	{
		return this.NextState() % 268435457U * 3.725290298461914E-09;
	}

	// Token: 0x060036E7 RID: 14055 RVA: 0x0011E6A9 File Offset: 0x0011C8A9
	public double NextDouble(double max)
	{
		if (max < 0.0)
		{
			return 0.0;
		}
		return this.NextDouble() * max;
	}

	// Token: 0x060036E8 RID: 14056 RVA: 0x0011E6CC File Offset: 0x0011C8CC
	public double NextDouble(double min, double max)
	{
		double num = max - min;
		if (num <= 0.0)
		{
			return min;
		}
		double num2 = this.NextDouble() * num;
		return min + num2;
	}

	// Token: 0x060036E9 RID: 14057 RVA: 0x0011E6F7 File Offset: 0x0011C8F7
	public float NextFloat()
	{
		return (float)this.NextDouble();
	}

	// Token: 0x060036EA RID: 14058 RVA: 0x0011E700 File Offset: 0x0011C900
	public float NextFloat(float max)
	{
		return (float)this.NextDouble((double)max);
	}

	// Token: 0x060036EB RID: 14059 RVA: 0x0011E70B File Offset: 0x0011C90B
	public float NextFloat(float min, float max)
	{
		return (float)this.NextDouble((double)min, (double)max);
	}

	// Token: 0x060036EC RID: 14060 RVA: 0x0011E718 File Offset: 0x0011C918
	public bool NextBool()
	{
		return this.NextState() % 2U == 1U;
	}

	// Token: 0x060036ED RID: 14061 RVA: 0x0011E725 File Offset: 0x0011C925
	public uint NextUInt()
	{
		return this.NextState();
	}

	// Token: 0x060036EE RID: 14062 RVA: 0x0011E725 File Offset: 0x0011C925
	public int NextInt()
	{
		return (int)this.NextState();
	}

	// Token: 0x060036EF RID: 14063 RVA: 0x0011E72D File Offset: 0x0011C92D
	public int NextInt(int max)
	{
		if (max <= 0)
		{
			return 0;
		}
		return (int)((ulong)this.NextState() % (ulong)((long)max));
	}

	// Token: 0x060036F0 RID: 14064 RVA: 0x0011E740 File Offset: 0x0011C940
	public int NextInt(int min, int max)
	{
		int num = max - min;
		if (num <= 0)
		{
			return min;
		}
		return min + this.NextInt(num);
	}

	// Token: 0x060036F1 RID: 14065 RVA: 0x0011E760 File Offset: 0x0011C960
	public int NextIntWithExclusion(int min, int max, int exclude)
	{
		int num = max - min - 1;
		if (num <= 0)
		{
			return min;
		}
		int num2 = min + 1 + this.NextInt(num);
		if (num2 > exclude)
		{
			return num2;
		}
		return num2 - 1;
	}

	// Token: 0x060036F2 RID: 14066 RVA: 0x0011E790 File Offset: 0x0011C990
	public int NextIntWithExclusion2(int min, int max, int exclude, int exclude2)
	{
		if (exclude == exclude2)
		{
			return this.NextIntWithExclusion(min, max, exclude);
		}
		int num = max - min - 2;
		if (num <= 0)
		{
			return min;
		}
		int num2 = min + 2 + this.NextInt(num);
		int num3;
		int num4;
		if (exclude >= exclude2)
		{
			num3 = exclude2 + 1;
			num4 = exclude;
		}
		else
		{
			num3 = exclude + 1;
			num4 = exclude2;
		}
		if (num2 <= num3)
		{
			return num2 - 2;
		}
		if (num2 <= num4)
		{
			return num2 - 1;
		}
		return num2;
	}

	// Token: 0x060036F3 RID: 14067 RVA: 0x0011E7F6 File Offset: 0x0011C9F6
	public byte NextByte()
	{
		return (byte)(this.NextState() & 255U);
	}

	// Token: 0x060036F4 RID: 14068 RVA: 0x0011E808 File Offset: 0x0011CA08
	public Color32 NextColor32()
	{
		byte r = this.NextByte();
		byte g = this.NextByte();
		byte b = this.NextByte();
		return new Color32(r, g, b, byte.MaxValue);
	}

	// Token: 0x060036F5 RID: 14069 RVA: 0x0011E838 File Offset: 0x0011CA38
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector3 NextPointInsideSphere(float radius)
	{
		float num = this.NextFloat() * 2f - 1f;
		float num2 = this.NextFloat() * 2f - 1f;
		float num3 = this.NextFloat() * 2f - 1f;
		float num4 = MathF.Pow(this.NextFloat(), 0.33333334f);
		float num5 = 1f / MathF.Sqrt(num * num + num2 * num2 + num3 * num3);
		return new Vector3(num * num5 * num4 * radius, num2 * num5 * num4 * radius, num3 * num5 * num4 * radius);
	}

	// Token: 0x060036F6 RID: 14070 RVA: 0x0011E8C4 File Offset: 0x0011CAC4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector3 NextPointOnSphere(float radius)
	{
		float num = this.NextFloat() * 2f - 1f;
		float num2 = this.NextFloat() * 2f - 1f;
		float num3 = this.NextFloat() * 2f - 1f;
		float num4 = 1f / MathF.Sqrt(num * num + num2 * num2 + num3 * num3);
		return new Vector3(num * num4 * radius, num2 * num4 * radius, num3 * num4 * radius);
	}

	// Token: 0x060036F7 RID: 14071 RVA: 0x0011E938 File Offset: 0x0011CB38
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector3 NextPointInsideBox(Vector3 extents)
	{
		float num = this.NextFloat() - 0.5f;
		float num2 = this.NextFloat() - 0.5f;
		float num3 = this.NextFloat() - 0.5f;
		return new Vector3(num * extents.x, num2 * extents.y, num3 * extents.z);
	}

	// Token: 0x060036F8 RID: 14072 RVA: 0x0011E988 File Offset: 0x0011CB88
	public Color NextColor()
	{
		float r = this.NextFloat();
		float g = this.NextFloat();
		float b = this.NextFloat();
		return new Color(r, g, b, 1f);
	}

	// Token: 0x060036F9 RID: 14073 RVA: 0x0011E9B8 File Offset: 0x0011CBB8
	public void Shuffle<T>(T[] array)
	{
		int i = array.Length;
		while (i > 1)
		{
			int num = this.NextInt(i--);
			int num2 = i;
			int num3 = num;
			T t = array[num];
			T t2 = array[i];
			array[num2] = t;
			array[num3] = t2;
		}
	}

	// Token: 0x060036FA RID: 14074 RVA: 0x0011EA08 File Offset: 0x0011CC08
	public void Shuffle<T>(List<T> list)
	{
		int i = list.Count;
		while (i > 1)
		{
			int num = this.NextInt(i--);
			int index = i;
			int index2 = num;
			T value = list[num];
			T value2 = list[i];
			list[index] = value;
			list[index2] = value2;
		}
	}

	// Token: 0x060036FB RID: 14075 RVA: 0x0011EA60 File Offset: 0x0011CC60
	public void Reset()
	{
		this._state = this._seed;
	}

	// Token: 0x060036FC RID: 14076 RVA: 0x0011E5E3 File Offset: 0x0011C7E3
	public void Reset(int seed)
	{
		this._seed = (uint)seed;
		this._state = this._seed;
	}

	// Token: 0x060036FD RID: 14077 RVA: 0x0011E5E3 File Offset: 0x0011C7E3
	public void Reset(uint seed)
	{
		this._seed = seed;
		this._state = this._seed;
	}

	// Token: 0x060036FE RID: 14078 RVA: 0x0011E5F8 File Offset: 0x0011C7F8
	public void Reset(long seed)
	{
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x060036FF RID: 14079 RVA: 0x0011E612 File Offset: 0x0011C812
	public void Reset(DateTime seed)
	{
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x06003700 RID: 14080 RVA: 0x0011E62C File Offset: 0x0011C82C
	public void Reset(string seed)
	{
		if (string.IsNullOrEmpty(seed))
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x06003701 RID: 14081 RVA: 0x0011E65E File Offset: 0x0011C85E
	public void Reset(byte[] seed)
	{
		if (seed == null || seed.Length == 0)
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x06003702 RID: 14082 RVA: 0x0011EA70 File Offset: 0x0011CC70
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private uint NextState()
	{
		return this._state = this.Mix(this._state + 184402071U);
	}

	// Token: 0x06003703 RID: 14083 RVA: 0x0011EA98 File Offset: 0x0011CC98
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private uint Mix(uint x)
	{
		x = (x >> 17 ^ x) * 3982152891U;
		x = (x >> 11 ^ x) * 2890668881U;
		x = (x >> 15 ^ x) * 830770091U;
		x = (x >> 14 ^ x);
		return x;
	}

	// Token: 0x06003704 RID: 14084 RVA: 0x0011EACD File Offset: 0x0011CCCD
	public override int GetHashCode()
	{
		return StaticHash.Compute((int)this._seed, (int)this._state);
	}

	// Token: 0x06003705 RID: 14085 RVA: 0x0011EAE0 File Offset: 0x0011CCE0
	public override string ToString()
	{
		return string.Format("{0} {{ {1}: {2:X8} {3}: {4:X8} }}", new object[]
		{
			"SRand",
			"_seed",
			this._seed,
			"_state",
			this._state
		});
	}

	// Token: 0x06003706 RID: 14086 RVA: 0x0011EB31 File Offset: 0x0011CD31
	public static SRand New()
	{
		return new SRand(DateTime.UtcNow);
	}

	// Token: 0x06003707 RID: 14087 RVA: 0x0011EB3D File Offset: 0x0011CD3D
	public static explicit operator SRand(int seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06003708 RID: 14088 RVA: 0x0011EB45 File Offset: 0x0011CD45
	public static explicit operator SRand(uint seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06003709 RID: 14089 RVA: 0x0011EB4D File Offset: 0x0011CD4D
	public static explicit operator SRand(long seed)
	{
		return new SRand(seed);
	}

	// Token: 0x0600370A RID: 14090 RVA: 0x0011EB55 File Offset: 0x0011CD55
	public static explicit operator SRand(string seed)
	{
		return new SRand(seed);
	}

	// Token: 0x0600370B RID: 14091 RVA: 0x0011EB5D File Offset: 0x0011CD5D
	public static explicit operator SRand(byte[] seed)
	{
		return new SRand(seed);
	}

	// Token: 0x0600370C RID: 14092 RVA: 0x0011EB65 File Offset: 0x0011CD65
	public static explicit operator SRand(DateTime seed)
	{
		return new SRand(seed);
	}

	// Token: 0x040043C1 RID: 17345
	[SerializeField]
	private uint _seed;

	// Token: 0x040043C2 RID: 17346
	[SerializeField]
	private uint _state;

	// Token: 0x040043C3 RID: 17347
	private const double MAX_AS_DOUBLE = 268435456.0;

	// Token: 0x040043C4 RID: 17348
	private const uint MAX_PLUS_ONE = 268435457U;

	// Token: 0x040043C5 RID: 17349
	private const double STEP_SIZE = 3.725290298461914E-09;

	// Token: 0x040043C6 RID: 17350
	private const float ONE_THIRD = 0.33333334f;
}
