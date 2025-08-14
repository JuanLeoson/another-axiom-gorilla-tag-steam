using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Cysharp.Text;
using UnityEngine;

// Token: 0x02000AF2 RID: 2802
public static class StringUtils
{
	// Token: 0x0600438C RID: 17292 RVA: 0x00153D19 File Offset: 0x00151F19
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNullOrEmpty(this string s)
	{
		return string.IsNullOrEmpty(s);
	}

	// Token: 0x0600438D RID: 17293 RVA: 0x00153D21 File Offset: 0x00151F21
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNullOrWhiteSpace(this string s)
	{
		return string.IsNullOrWhiteSpace(s);
	}

	// Token: 0x0600438E RID: 17294 RVA: 0x00153D2C File Offset: 0x00151F2C
	public static string ToAlphaNumeric(this string s)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return string.Empty;
		}
		string result;
		using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
		{
			foreach (char c in s)
			{
				if (char.IsLetterOrDigit(c))
				{
					utf16ValueStringBuilder.Append(c);
				}
			}
			result = utf16ValueStringBuilder.ToString();
		}
		return result;
	}

	// Token: 0x0600438F RID: 17295 RVA: 0x00153DA8 File Offset: 0x00151FA8
	public static string Capitalize(this string s)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return s;
		}
		char[] array = s.ToCharArray();
		array[0] = char.ToUpperInvariant(array[0]);
		return new string(array);
	}

	// Token: 0x06004390 RID: 17296 RVA: 0x00153DD7 File Offset: 0x00151FD7
	public static string Concat(this IEnumerable<string> source)
	{
		return string.Concat(source);
	}

	// Token: 0x06004391 RID: 17297 RVA: 0x00153DDF File Offset: 0x00151FDF
	public static string Join(this IEnumerable<string> source, string separator)
	{
		return string.Join(separator, source);
	}

	// Token: 0x06004392 RID: 17298 RVA: 0x00153DE8 File Offset: 0x00151FE8
	public static string Join(this IEnumerable<string> source, char separator)
	{
		return string.Join<string>(separator, source);
	}

	// Token: 0x06004393 RID: 17299 RVA: 0x00153DF1 File Offset: 0x00151FF1
	public static string RemoveAll(this string s, string value, StringComparison mode = StringComparison.OrdinalIgnoreCase)
	{
		if (string.IsNullOrEmpty(s))
		{
			return s;
		}
		return s.Replace(value, string.Empty, mode);
	}

	// Token: 0x06004394 RID: 17300 RVA: 0x00153E0A File Offset: 0x0015200A
	public static string RemoveAll(this string s, char value, StringComparison mode = StringComparison.OrdinalIgnoreCase)
	{
		return s.RemoveAll(value.ToString(), mode);
	}

	// Token: 0x06004395 RID: 17301 RVA: 0x00153E1A File Offset: 0x0015201A
	public static byte[] ToBytesASCII(this string s)
	{
		return Encoding.ASCII.GetBytes(s);
	}

	// Token: 0x06004396 RID: 17302 RVA: 0x00153E27 File Offset: 0x00152027
	public static byte[] ToBytesUTF8(this string s)
	{
		return Encoding.UTF8.GetBytes(s);
	}

	// Token: 0x06004397 RID: 17303 RVA: 0x00153E34 File Offset: 0x00152034
	public static byte[] ToBytesUnicode(this string s)
	{
		return Encoding.Unicode.GetBytes(s);
	}

	// Token: 0x06004398 RID: 17304 RVA: 0x00153E44 File Offset: 0x00152044
	public static string ComputeSHV2(this string s)
	{
		return Hash128.Compute(s).ToString();
	}

	// Token: 0x06004399 RID: 17305 RVA: 0x00153E65 File Offset: 0x00152065
	public static string ToQueryString(this Dictionary<string, string> d)
	{
		if (d == null)
		{
			return null;
		}
		return "?" + string.Join("&", from x in d
		select x.Key + "=" + x.Value);
	}

	// Token: 0x0600439A RID: 17306 RVA: 0x00153EA8 File Offset: 0x001520A8
	public static string Combine(string separator, params string[] values)
	{
		if (values == null || values.Length == 0)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = !string.IsNullOrEmpty(separator);
		for (int i = 0; i < values.Length; i++)
		{
			if (flag)
			{
				stringBuilder.Append(separator);
			}
			stringBuilder.Append(values);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0600439B RID: 17307 RVA: 0x00153EF8 File Offset: 0x001520F8
	public static string ToUpperCamelCase(this string input)
	{
		if (string.IsNullOrWhiteSpace(input))
		{
			return string.Empty;
		}
		string[] array = Regex.Split(input, "[^A-Za-z0-9]+");
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Length > 0)
			{
				string[] array2 = array;
				int num = i;
				string str = char.ToUpper(array[i][0]).ToString();
				string str2;
				if (array[i].Length <= 1)
				{
					str2 = "";
				}
				else
				{
					string text = array[i];
					str2 = text.Substring(1, text.Length - 1).ToLower();
				}
				array2[num] = str + str2;
			}
		}
		return string.Join("", array);
	}

	// Token: 0x0600439C RID: 17308 RVA: 0x00153F8C File Offset: 0x0015218C
	public static string ToUpperCaseFromCamelCase(this string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return input;
		}
		input = input.Trim();
		string result;
		using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
		{
			bool flag = true;
			foreach (char c in input)
			{
				if (char.IsUpper(c) && !flag)
				{
					utf16ValueStringBuilder.Append(' ');
				}
				utf16ValueStringBuilder.Append(char.ToUpper(c));
				flag = char.IsUpper(c);
			}
			result = utf16ValueStringBuilder.ToString().Trim();
		}
		return result;
	}

	// Token: 0x0600439D RID: 17309 RVA: 0x0015402C File Offset: 0x0015222C
	public static string RemoveStart(this string s, string value, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase)
	{
		if (string.IsNullOrEmpty(s) || !s.StartsWith(value, comparison))
		{
			return s;
		}
		return s.Substring(value.Length);
	}

	// Token: 0x0600439E RID: 17310 RVA: 0x0015404E File Offset: 0x0015224E
	public static string RemoveEnd(this string s, string value, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase)
	{
		if (string.IsNullOrEmpty(s) || !s.EndsWith(value, comparison))
		{
			return s;
		}
		return s.Substring(0, s.Length - value.Length);
	}

	// Token: 0x0600439F RID: 17311 RVA: 0x00154078 File Offset: 0x00152278
	public static string RemoveBothEnds(this string s, string value, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase)
	{
		return s.RemoveEnd(value, comparison).RemoveStart(value, comparison);
	}

	// Token: 0x04004E22 RID: 20002
	public const string kForwardSlash = "/";

	// Token: 0x04004E23 RID: 20003
	public const string kBackSlash = "/";

	// Token: 0x04004E24 RID: 20004
	public const string kBackTick = "`";

	// Token: 0x04004E25 RID: 20005
	public const string kMinusDash = "-";

	// Token: 0x04004E26 RID: 20006
	public const string kPeriod = ".";

	// Token: 0x04004E27 RID: 20007
	public const string kUnderScore = "_";

	// Token: 0x04004E28 RID: 20008
	public const string kColon = ":";
}
