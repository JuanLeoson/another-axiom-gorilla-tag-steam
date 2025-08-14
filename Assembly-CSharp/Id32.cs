using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000878 RID: 2168
[Serializable]
public struct Id32
{
	// Token: 0x06003662 RID: 13922 RVA: 0x0011CE90 File Offset: 0x0011B090
	public Id32(string idString)
	{
		if (idString == null)
		{
			throw new ArgumentNullException("idString");
		}
		if (string.IsNullOrWhiteSpace(idString.Trim()))
		{
			throw new ArgumentNullException("idString");
		}
		this._id = XXHash32.Compute(idString, 0U);
	}

	// Token: 0x06003663 RID: 13923 RVA: 0x0011CEC5 File Offset: 0x0011B0C5
	public unsafe static implicit operator int(Id32 i32)
	{
		return *Unsafe.As<Id32, int>(ref i32);
	}

	// Token: 0x06003664 RID: 13924 RVA: 0x0011CECF File Offset: 0x0011B0CF
	public static implicit operator Id32(string s)
	{
		return Id32.ComputeID(s);
	}

	// Token: 0x06003665 RID: 13925 RVA: 0x0011CED8 File Offset: 0x0011B0D8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static Id32 ComputeID(string s)
	{
		int num = Id32.ComputeHash(s);
		return *Unsafe.As<int, Id32>(ref num);
	}

	// Token: 0x06003666 RID: 13926 RVA: 0x0011CEF8 File Offset: 0x0011B0F8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ComputeHash(string s)
	{
		if (s == null)
		{
			return 0;
		}
		s = s.Trim();
		if (string.IsNullOrWhiteSpace(s))
		{
			return 0;
		}
		return XXHash32.Compute(s, 0U);
	}

	// Token: 0x06003667 RID: 13927 RVA: 0x0011CF18 File Offset: 0x0011B118
	public override int GetHashCode()
	{
		return this._id;
	}

	// Token: 0x06003668 RID: 13928 RVA: 0x0011CF20 File Offset: 0x0011B120
	public override string ToString()
	{
		return string.Format("{{ {0} : {1} }}", "Id32", this._id);
	}

	// Token: 0x04004337 RID: 17207
	[SerializeField]
	private int _id;
}
