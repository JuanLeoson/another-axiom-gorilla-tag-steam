using System;

// Token: 0x020008C7 RID: 2247
public static class KIDFeaturesExtensions
{
	// Token: 0x060037FF RID: 14335 RVA: 0x001210E4 File Offset: 0x0011F2E4
	public static string ToStandardisedString(this EKIDFeatures feature)
	{
		switch (feature)
		{
		case EKIDFeatures.Multiplayer:
			return "multiplayer";
		case EKIDFeatures.Custom_Nametags:
			return "custom-username";
		case EKIDFeatures.Voice_Chat:
			return "voice-chat";
		case EKIDFeatures.Mods:
			return "mods";
		case EKIDFeatures.Groups:
			return "join-groups";
		default:
			return feature.ToString();
		}
	}

	// Token: 0x06003800 RID: 14336 RVA: 0x00121138 File Offset: 0x0011F338
	public static EKIDFeatures? FromString(string name)
	{
		string a = name.ToLower();
		if (a == "voice-chat")
		{
			return new EKIDFeatures?(EKIDFeatures.Voice_Chat);
		}
		if (a == "custom-username")
		{
			return new EKIDFeatures?(EKIDFeatures.Custom_Nametags);
		}
		if (a == "multiplayer")
		{
			return new EKIDFeatures?(EKIDFeatures.Multiplayer);
		}
		if (a == "mods")
		{
			return new EKIDFeatures?(EKIDFeatures.Mods);
		}
		if (!(a == "join-groups"))
		{
			return null;
		}
		return new EKIDFeatures?(EKIDFeatures.Groups);
	}

	// Token: 0x06003801 RID: 14337 RVA: 0x001211BC File Offset: 0x0011F3BC
	public static bool TryGetFromString(string name, out EKIDFeatures result)
	{
		EKIDFeatures? ekidfeatures = KIDFeaturesExtensions.FromString(name);
		if (ekidfeatures != null)
		{
			result = ekidfeatures.Value;
			return true;
		}
		result = EKIDFeatures.Voice_Chat;
		return false;
	}
}
