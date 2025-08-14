using System;
using System.ComponentModel;
using UnityEngine;

// Token: 0x0200025B RID: 603
public static class UnityTagsExt
{
	// Token: 0x06000E01 RID: 3585 RVA: 0x00055728 File Offset: 0x00053928
	public static UnityTag ToTag(this string s)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return UnityTag.Invalid;
		}
		UnityTag result;
		if (!UnityTags.StringToTag.TryGetValue(s, out result))
		{
			return UnityTag.Invalid;
		}
		return result;
	}

	// Token: 0x06000E02 RID: 3586 RVA: 0x00055751 File Offset: 0x00053951
	public static void SetTag(this Component c, UnityTag tag)
	{
		if (c == null)
		{
			return;
		}
		if (tag == UnityTag.Invalid)
		{
			throw new InvalidEnumArgumentException("tag");
		}
		c.tag = UnityTags.StringValues[(int)tag];
	}

	// Token: 0x06000E03 RID: 3587 RVA: 0x00055779 File Offset: 0x00053979
	public static void SetTag(this GameObject g, UnityTag tag)
	{
		if (g == null)
		{
			return;
		}
		if (tag == UnityTag.Invalid)
		{
			throw new InvalidEnumArgumentException("tag");
		}
		g.tag = UnityTags.StringValues[(int)tag];
	}

	// Token: 0x06000E04 RID: 3588 RVA: 0x000557A1 File Offset: 0x000539A1
	public static bool TryGetTag(this GameObject g, out UnityTag tag)
	{
		tag = UnityTag.Invalid;
		return !(g == null) && UnityTags.StringToTag.TryGetValue(g.tag, out tag);
	}

	// Token: 0x06000E05 RID: 3589 RVA: 0x000557C2 File Offset: 0x000539C2
	public static bool TryGetTag(this Component c, out UnityTag tag)
	{
		tag = UnityTag.Invalid;
		return !(c == null) && UnityTags.StringToTag.TryGetValue(c.tag, out tag);
	}

	// Token: 0x06000E06 RID: 3590 RVA: 0x000557E3 File Offset: 0x000539E3
	public static bool CompareTag(this GameObject g, UnityTag tag)
	{
		if (g == null)
		{
			return false;
		}
		if (tag == UnityTag.Invalid)
		{
			throw new InvalidEnumArgumentException("tag");
		}
		return g.CompareTag(UnityTags.StringValues[(int)tag]);
	}

	// Token: 0x06000E07 RID: 3591 RVA: 0x0005580C File Offset: 0x00053A0C
	public static bool CompareTag(this Component c, UnityTag tag)
	{
		if (c == null)
		{
			return false;
		}
		if (tag == UnityTag.Invalid)
		{
			throw new InvalidEnumArgumentException("tag");
		}
		return c.CompareTag(UnityTags.StringValues[(int)tag]);
	}
}
