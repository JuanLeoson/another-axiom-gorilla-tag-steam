using System;

// Token: 0x02000AF1 RID: 2801
public static class StaticHashExt
{
	// Token: 0x06004383 RID: 17283 RVA: 0x00153CD1 File Offset: 0x00151ED1
	public static int GetStaticHash(this int i)
	{
		return StaticHash.Compute(i);
	}

	// Token: 0x06004384 RID: 17284 RVA: 0x00153CD9 File Offset: 0x00151ED9
	public static int GetStaticHash(this uint u)
	{
		return StaticHash.Compute(u);
	}

	// Token: 0x06004385 RID: 17285 RVA: 0x00153CE1 File Offset: 0x00151EE1
	public static int GetStaticHash(this float f)
	{
		return StaticHash.Compute(f);
	}

	// Token: 0x06004386 RID: 17286 RVA: 0x00153CE9 File Offset: 0x00151EE9
	public static int GetStaticHash(this long l)
	{
		return StaticHash.Compute(l);
	}

	// Token: 0x06004387 RID: 17287 RVA: 0x00153CF1 File Offset: 0x00151EF1
	public static int GetStaticHash(this double d)
	{
		return StaticHash.Compute(d);
	}

	// Token: 0x06004388 RID: 17288 RVA: 0x00153CF9 File Offset: 0x00151EF9
	public static int GetStaticHash(this bool b)
	{
		return StaticHash.Compute(b);
	}

	// Token: 0x06004389 RID: 17289 RVA: 0x00153D01 File Offset: 0x00151F01
	public static int GetStaticHash(this DateTime dt)
	{
		return StaticHash.Compute(dt);
	}

	// Token: 0x0600438A RID: 17290 RVA: 0x00153D09 File Offset: 0x00151F09
	public static int GetStaticHash(this string s)
	{
		return StaticHash.Compute(s);
	}

	// Token: 0x0600438B RID: 17291 RVA: 0x00153D11 File Offset: 0x00151F11
	public static int GetStaticHash(this byte[] bytes)
	{
		return StaticHash.Compute(bytes);
	}
}
