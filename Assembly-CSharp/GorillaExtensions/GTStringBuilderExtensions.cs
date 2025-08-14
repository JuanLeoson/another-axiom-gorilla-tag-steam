using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Text;
using UnityEngine;

namespace GorillaExtensions
{
	// Token: 0x02000E3D RID: 3645
	public static class GTStringBuilderExtensions
	{
		// Token: 0x06005A95 RID: 23189 RVA: 0x001C9DF4 File Offset: 0x001C7FF4
		public unsafe static IEnumerable<ReadOnlyMemory<char>> GetSegmentsOfMem(this Utf16ValueStringBuilder sb, int maxCharsPerSegment = 16300)
		{
			int i = 0;
			List<ReadOnlyMemory<char>> list = new List<ReadOnlyMemory<char>>(64);
			ReadOnlyMemory<char> readOnlyMemory = sb.AsMemory();
			while (i < readOnlyMemory.Length)
			{
				int num = Mathf.Min(i + maxCharsPerSegment, readOnlyMemory.Length);
				if (num < readOnlyMemory.Length)
				{
					int num2 = -1;
					for (int j = num - 1; j >= i; j--)
					{
						if (*readOnlyMemory.Span[j] == 10)
						{
							num2 = j;
							break;
						}
					}
					if (num2 != -1)
					{
						num = num2;
					}
				}
				list.Add(readOnlyMemory.Slice(i, num - i));
				i = num + 1;
			}
			return list;
		}

		// Token: 0x06005A96 RID: 23190 RVA: 0x001C9E89 File Offset: 0x001C8089
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTAddPath(this Utf16ValueStringBuilder stringBuilderToAddTo, GameObject gameObject)
		{
			gameObject.transform.GetPathQ(ref stringBuilderToAddTo);
		}

		// Token: 0x06005A97 RID: 23191 RVA: 0x001C9E98 File Offset: 0x001C8098
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTAddPath(this Utf16ValueStringBuilder stringBuilderToAddTo, Transform transform)
		{
			transform.GetPathQ(ref stringBuilderToAddTo);
		}

		// Token: 0x06005A98 RID: 23192 RVA: 0x001C9EA2 File Offset: 0x001C80A2
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Q(this Utf16ValueStringBuilder sb, string value)
		{
			sb.Append('"');
			sb.Append(value);
			sb.Append('"');
		}

		// Token: 0x06005A99 RID: 23193 RVA: 0x001C9EBE File Offset: 0x001C80BE
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b)
		{
			sb.Append(a);
			sb.Append(b);
		}

		// Token: 0x06005A9A RID: 23194 RVA: 0x001C9ED0 File Offset: 0x001C80D0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
		}

		// Token: 0x06005A9B RID: 23195 RVA: 0x001C9EEA File Offset: 0x001C80EA
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
		}

		// Token: 0x06005A9C RID: 23196 RVA: 0x001C9F0D File Offset: 0x001C810D
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d, string e)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
			sb.Append(e);
		}

		// Token: 0x06005A9D RID: 23197 RVA: 0x001C9F39 File Offset: 0x001C8139
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d, string e, string f)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
			sb.Append(e);
			sb.Append(f);
		}

		// Token: 0x06005A9E RID: 23198 RVA: 0x001C9F6E File Offset: 0x001C816E
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d, string e, string f, string g)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
			sb.Append(e);
			sb.Append(f);
			sb.Append(g);
		}

		// Token: 0x06005A9F RID: 23199 RVA: 0x001C9FAC File Offset: 0x001C81AC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d, string e, string f, string g, string h)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
			sb.Append(e);
			sb.Append(f);
			sb.Append(g);
			sb.Append(h);
		}

		// Token: 0x06005AA0 RID: 23200 RVA: 0x001CA000 File Offset: 0x001C8200
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d, string e, string f, string g, string h, string i)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
			sb.Append(e);
			sb.Append(f);
			sb.Append(g);
			sb.Append(h);
			sb.Append(i);
		}

		// Token: 0x06005AA1 RID: 23201 RVA: 0x001CA05C File Offset: 0x001C825C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d, string e, string f, string g, string h, string i, string j)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
			sb.Append(e);
			sb.Append(f);
			sb.Append(g);
			sb.Append(h);
			sb.Append(i);
			sb.Append(j);
		}
	}
}
