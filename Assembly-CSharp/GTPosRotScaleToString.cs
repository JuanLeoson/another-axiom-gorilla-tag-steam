using System;
using System.Text.RegularExpressions;
using UnityEngine;

// Token: 0x0200024F RID: 591
public static class GTPosRotScaleToString
{
	// Token: 0x06000DCB RID: 3531 RVA: 0x000546E4 File Offset: 0x000528E4
	public static string ToString(Vector3 pos, Vector3 rot, Vector3 scale, bool isWorldSpace, string parentPath = null)
	{
		string text = isWorldSpace ? "WorldPRS" : "LocalPRS";
		string str = string.Concat(new string[]
		{
			text,
			" { p=",
			GTPosRotScaleToString.ValToStr(pos),
			", r=",
			GTPosRotScaleToString.ValToStr(rot),
			", s=",
			GTPosRotScaleToString.ValToStr(scale)
		});
		if (!string.IsNullOrEmpty(parentPath))
		{
			str = str + " parent=\"" + parentPath + "\"";
		}
		return str + " }";
	}

	// Token: 0x06000DCC RID: 3532 RVA: 0x0005476D File Offset: 0x0005296D
	private static string ValToStr(Vector3 v)
	{
		return string.Format("({0:R}, {1:R}, {2:R})", v.x, v.y, v.z);
	}

	// Token: 0x06000DCD RID: 3533 RVA: 0x0005479A File Offset: 0x0005299A
	public static bool ParseIsWorldSpace(string input)
	{
		return input.Contains("WorldPRS");
	}

	// Token: 0x06000DCE RID: 3534 RVA: 0x000547A8 File Offset: 0x000529A8
	public static string ParseParentPath(string input)
	{
		MatchCollection matchCollection = Regex.Matches(input, "parent\\s*=\\s*\"(?<parent>.*?)\"");
		if (matchCollection.Count <= 0)
		{
			return null;
		}
		return matchCollection[0].Groups["parent"].Value;
	}

	// Token: 0x06000DCF RID: 3535 RVA: 0x000547E7 File Offset: 0x000529E7
	public static bool TryParsePos(string input, out Vector3 v)
	{
		return GTPosRotScaleToString.TryParseVec3_internal(GTRegex.k_Pos, input, out v);
	}

	// Token: 0x06000DD0 RID: 3536 RVA: 0x000547F5 File Offset: 0x000529F5
	public static bool TryParseRot(string input, out Vector3 v)
	{
		return GTPosRotScaleToString.TryParseVec3_internal(GTRegex.k_Rot, input, out v);
	}

	// Token: 0x06000DD1 RID: 3537 RVA: 0x00054803 File Offset: 0x00052A03
	public static bool TryParseScale(string input, out Vector3 v)
	{
		return GTPosRotScaleToString.TryParseVec3_internal(GTRegex.k_Scale, input, out v) || GTPosRotScaleToString.TryParseVec3_internal(GTRegex.k_Vec3, input, out v);
	}

	// Token: 0x06000DD2 RID: 3538 RVA: 0x00054821 File Offset: 0x00052A21
	public static bool TryParseVec3(string input, out Vector3 v)
	{
		return GTPosRotScaleToString.TryParseVec3_internal(GTRegex.k_Vec3, input, out v);
	}

	// Token: 0x06000DD3 RID: 3539 RVA: 0x00054830 File Offset: 0x00052A30
	private static bool TryParseVec3_internal(Regex regex, string input, out Vector3 v)
	{
		v = Vector3.zero;
		MatchCollection matchCollection = regex.Matches(input);
		if (matchCollection.Count <= 0)
		{
			return false;
		}
		v = GTPosRotScaleToString.StringToVector3(matchCollection[0]);
		return true;
	}

	// Token: 0x06000DD4 RID: 3540 RVA: 0x00054870 File Offset: 0x00052A70
	private static Vector3 StringToVector3(Match match)
	{
		float x = float.Parse(match.Groups["x"].Value);
		float y = float.Parse(match.Groups["y"].Value);
		float z = float.Parse(match.Groups["z"].Value);
		return new Vector3(x, y, z);
	}

	// Token: 0x04001598 RID: 5528
	public const string k_LocalPRSLabel = "LocalPRS";

	// Token: 0x04001599 RID: 5529
	public const string k_WorldPRSLabel = "WorldPRS";
}
