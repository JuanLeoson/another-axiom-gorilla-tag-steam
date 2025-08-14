using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

// Token: 0x02000876 RID: 2166
[Serializable]
[StructLayout(LayoutKind.Explicit)]
public struct Id128 : IEquatable<Id128>, IComparable<Id128>, IEquatable<Guid>, IEquatable<Hash128>
{
	// Token: 0x06003639 RID: 13881 RVA: 0x0011C948 File Offset: 0x0011AB48
	public Id128(int a, int b, int c, int d)
	{
		this.guid = Guid.Empty;
		this.h128 = default(Hash128);
		this.x = (this.y = 0L);
		this.a = a;
		this.b = b;
		this.c = c;
		this.d = d;
	}

	// Token: 0x0600363A RID: 13882 RVA: 0x0011C99C File Offset: 0x0011AB9C
	public Id128(long x, long y)
	{
		this.a = (this.b = (this.c = (this.d = 0)));
		this.guid = Guid.Empty;
		this.h128 = default(Hash128);
		this.x = x;
		this.y = y;
	}

	// Token: 0x0600363B RID: 13883 RVA: 0x0011C9F0 File Offset: 0x0011ABF0
	public Id128(Hash128 hash)
	{
		this.x = (this.y = 0L);
		this.a = (this.b = (this.c = (this.d = 0)));
		this.guid = Guid.Empty;
		this.h128 = hash;
	}

	// Token: 0x0600363C RID: 13884 RVA: 0x0011CA44 File Offset: 0x0011AC44
	public Id128(Guid guid)
	{
		this.a = (this.b = (this.c = (this.d = 0)));
		this.x = (this.y = 0L);
		this.h128 = default(Hash128);
		this.guid = guid;
	}

	// Token: 0x0600363D RID: 13885 RVA: 0x0011CA98 File Offset: 0x0011AC98
	public Id128(string guid)
	{
		if (string.IsNullOrWhiteSpace(guid))
		{
			throw new ArgumentNullException("guid");
		}
		this.a = (this.b = (this.c = (this.d = 0)));
		this.x = (this.y = 0L);
		this.h128 = default(Hash128);
		this.guid = Guid.Parse(guid);
	}

	// Token: 0x0600363E RID: 13886 RVA: 0x0011CB04 File Offset: 0x0011AD04
	public Id128(byte[] bytes)
	{
		if (bytes == null)
		{
			throw new ArgumentNullException("bytes");
		}
		if (bytes.Length != 16)
		{
			throw new ArgumentException("Input buffer must be exactly 16 bytes", "bytes");
		}
		this.a = (this.b = (this.c = (this.d = 0)));
		this.x = (this.y = 0L);
		this.h128 = default(Hash128);
		this.guid = new Guid(bytes);
	}

	// Token: 0x0600363F RID: 13887 RVA: 0x0011CB81 File Offset: 0x0011AD81
	[return: TupleElementNames(new string[]
	{
		"l1",
		"l2"
	})]
	public ValueTuple<long, long> ToLongs()
	{
		return new ValueTuple<long, long>(this.x, this.y);
	}

	// Token: 0x06003640 RID: 13888 RVA: 0x0011CB94 File Offset: 0x0011AD94
	[return: TupleElementNames(new string[]
	{
		"i1",
		"i2",
		"i3",
		"i4"
	})]
	public ValueTuple<int, int, int, int> ToInts()
	{
		return new ValueTuple<int, int, int, int>(this.a, this.b, this.c, this.d);
	}

	// Token: 0x06003641 RID: 13889 RVA: 0x0011CBB3 File Offset: 0x0011ADB3
	public byte[] ToByteArray()
	{
		return this.guid.ToByteArray();
	}

	// Token: 0x06003642 RID: 13890 RVA: 0x0011CBC0 File Offset: 0x0011ADC0
	public bool Equals(Id128 id)
	{
		return this.x == id.x && this.y == id.y;
	}

	// Token: 0x06003643 RID: 13891 RVA: 0x0011CBE0 File Offset: 0x0011ADE0
	public bool Equals(Guid g)
	{
		return this.guid == g;
	}

	// Token: 0x06003644 RID: 13892 RVA: 0x0011CBEE File Offset: 0x0011ADEE
	public bool Equals(Hash128 h)
	{
		return this.h128 == h;
	}

	// Token: 0x06003645 RID: 13893 RVA: 0x0011CBFC File Offset: 0x0011ADFC
	public override bool Equals(object obj)
	{
		if (obj is Id128)
		{
			Id128 id = (Id128)obj;
			return this.Equals(id);
		}
		if (obj is Guid)
		{
			Guid g = (Guid)obj;
			return this.Equals(g);
		}
		if (obj is Hash128)
		{
			Hash128 h = (Hash128)obj;
			return this.Equals(h);
		}
		return false;
	}

	// Token: 0x06003646 RID: 13894 RVA: 0x0011CC4F File Offset: 0x0011AE4F
	public override string ToString()
	{
		return this.guid.ToString();
	}

	// Token: 0x06003647 RID: 13895 RVA: 0x0011CC62 File Offset: 0x0011AE62
	public override int GetHashCode()
	{
		return StaticHash.Compute(this.a, this.b, this.c, this.d);
	}

	// Token: 0x06003648 RID: 13896 RVA: 0x0011CC84 File Offset: 0x0011AE84
	public int CompareTo(Id128 id)
	{
		int num = this.x.CompareTo(id.x);
		if (num == 0)
		{
			num = this.y.CompareTo(id.y);
		}
		return num;
	}

	// Token: 0x06003649 RID: 13897 RVA: 0x0011CCBC File Offset: 0x0011AEBC
	public int CompareTo(object obj)
	{
		if (obj is Id128)
		{
			Id128 id = (Id128)obj;
			return this.CompareTo(id);
		}
		if (obj is Guid)
		{
			Guid value = (Guid)obj;
			return this.guid.CompareTo(value);
		}
		if (obj is Hash128)
		{
			Hash128 rhs = (Hash128)obj;
			return this.h128.CompareTo(rhs);
		}
		throw new ArgumentException("Object must be of type Id128 or Guid");
	}

	// Token: 0x0600364A RID: 13898 RVA: 0x0011CD22 File Offset: 0x0011AF22
	public static Id128 NewId()
	{
		return new Id128(Guid.NewGuid());
	}

	// Token: 0x0600364B RID: 13899 RVA: 0x0011CD30 File Offset: 0x0011AF30
	public static Id128 ComputeMD5(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return Id128.Empty;
		}
		Id128 result;
		using (MD5 md = MD5.Create())
		{
			result = new Guid(md.ComputeHash(Encoding.UTF8.GetBytes(s)));
		}
		return result;
	}

	// Token: 0x0600364C RID: 13900 RVA: 0x0011CD8C File Offset: 0x0011AF8C
	public static Id128 ComputeSHV2(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return Id128.Empty;
		}
		return Hash128.Compute(s);
	}

	// Token: 0x0600364D RID: 13901 RVA: 0x0011CDA7 File Offset: 0x0011AFA7
	public static bool operator ==(Id128 j, Id128 k)
	{
		return j.Equals(k);
	}

	// Token: 0x0600364E RID: 13902 RVA: 0x0011CDB1 File Offset: 0x0011AFB1
	public static bool operator !=(Id128 j, Id128 k)
	{
		return !j.Equals(k);
	}

	// Token: 0x0600364F RID: 13903 RVA: 0x0011CDBE File Offset: 0x0011AFBE
	public static bool operator ==(Id128 j, Guid k)
	{
		return j.Equals(k);
	}

	// Token: 0x06003650 RID: 13904 RVA: 0x0011CDC8 File Offset: 0x0011AFC8
	public static bool operator !=(Id128 j, Guid k)
	{
		return !j.Equals(k);
	}

	// Token: 0x06003651 RID: 13905 RVA: 0x0011CDD5 File Offset: 0x0011AFD5
	public static bool operator ==(Guid j, Id128 k)
	{
		return j.Equals(k.guid);
	}

	// Token: 0x06003652 RID: 13906 RVA: 0x0011CDE4 File Offset: 0x0011AFE4
	public static bool operator !=(Guid j, Id128 k)
	{
		return !j.Equals(k.guid);
	}

	// Token: 0x06003653 RID: 13907 RVA: 0x0011CDF6 File Offset: 0x0011AFF6
	public static bool operator ==(Id128 j, Hash128 k)
	{
		return j.Equals(k);
	}

	// Token: 0x06003654 RID: 13908 RVA: 0x0011CE00 File Offset: 0x0011B000
	public static bool operator !=(Id128 j, Hash128 k)
	{
		return !j.Equals(k);
	}

	// Token: 0x06003655 RID: 13909 RVA: 0x0011CE0D File Offset: 0x0011B00D
	public static bool operator ==(Hash128 j, Id128 k)
	{
		return j.Equals(k.h128);
	}

	// Token: 0x06003656 RID: 13910 RVA: 0x0011CE1C File Offset: 0x0011B01C
	public static bool operator !=(Hash128 j, Id128 k)
	{
		return !j.Equals(k.h128);
	}

	// Token: 0x06003657 RID: 13911 RVA: 0x0011CE2E File Offset: 0x0011B02E
	public static bool operator <(Id128 j, Id128 k)
	{
		return j.CompareTo(k) < 0;
	}

	// Token: 0x06003658 RID: 13912 RVA: 0x0011CE3B File Offset: 0x0011B03B
	public static bool operator >(Id128 j, Id128 k)
	{
		return j.CompareTo(k) > 0;
	}

	// Token: 0x06003659 RID: 13913 RVA: 0x0011CE48 File Offset: 0x0011B048
	public static bool operator <=(Id128 j, Id128 k)
	{
		return j.CompareTo(k) <= 0;
	}

	// Token: 0x0600365A RID: 13914 RVA: 0x0011CE58 File Offset: 0x0011B058
	public static bool operator >=(Id128 j, Id128 k)
	{
		return j.CompareTo(k) >= 0;
	}

	// Token: 0x0600365B RID: 13915 RVA: 0x0011CE68 File Offset: 0x0011B068
	public static implicit operator Guid(Id128 id)
	{
		return id.guid;
	}

	// Token: 0x0600365C RID: 13916 RVA: 0x0011CE70 File Offset: 0x0011B070
	public static implicit operator Id128(Guid guid)
	{
		return new Id128(guid);
	}

	// Token: 0x0600365D RID: 13917 RVA: 0x0011CE78 File Offset: 0x0011B078
	public static implicit operator Id128(Hash128 h)
	{
		return new Id128(h);
	}

	// Token: 0x0600365E RID: 13918 RVA: 0x0011CE80 File Offset: 0x0011B080
	public static implicit operator Hash128(Id128 id)
	{
		return id.h128;
	}

	// Token: 0x0600365F RID: 13919 RVA: 0x0011CE88 File Offset: 0x0011B088
	public static explicit operator Id128(string s)
	{
		return Id128.ComputeMD5(s);
	}

	// Token: 0x0400432E RID: 17198
	[SerializeField]
	[FieldOffset(0)]
	public long x;

	// Token: 0x0400432F RID: 17199
	[SerializeField]
	[FieldOffset(8)]
	public long y;

	// Token: 0x04004330 RID: 17200
	[NonSerialized]
	[FieldOffset(0)]
	public int a;

	// Token: 0x04004331 RID: 17201
	[NonSerialized]
	[FieldOffset(4)]
	public int b;

	// Token: 0x04004332 RID: 17202
	[NonSerialized]
	[FieldOffset(8)]
	public int c;

	// Token: 0x04004333 RID: 17203
	[NonSerialized]
	[FieldOffset(12)]
	public int d;

	// Token: 0x04004334 RID: 17204
	[NonSerialized]
	[FieldOffset(0)]
	public Guid guid;

	// Token: 0x04004335 RID: 17205
	[NonSerialized]
	[FieldOffset(0)]
	public Hash128 h128;

	// Token: 0x04004336 RID: 17206
	public static readonly Id128 Empty;
}
