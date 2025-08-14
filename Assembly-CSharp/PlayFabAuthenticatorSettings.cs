using System;
using UnityEngine;

// Token: 0x02000A13 RID: 2579
public class PlayFabAuthenticatorSettings
{
	// Token: 0x06003EEE RID: 16110 RVA: 0x0013FC94 File Offset: 0x0013DE94
	static PlayFabAuthenticatorSettings()
	{
		PlayFabAuthenticatorSettings.Load("PlayFabAuthenticatorSettings");
	}

	// Token: 0x06003EEF RID: 16111 RVA: 0x0013FCA0 File Offset: 0x0013DEA0
	public static void Load(string path)
	{
		PlayFabAuthenticatorSettingsScriptableObject playFabAuthenticatorSettingsScriptableObject = Resources.Load<PlayFabAuthenticatorSettingsScriptableObject>(path);
		PlayFabAuthenticatorSettings.TitleId = playFabAuthenticatorSettingsScriptableObject.TitleId;
		PlayFabAuthenticatorSettings.AuthApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.AuthApiBaseUrl;
		PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.DailyQuestsApiBaseUrl;
		PlayFabAuthenticatorSettings.FriendApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.FriendApiBaseUrl;
		PlayFabAuthenticatorSettings.HpPromoApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.HpPromoApiBaseUrl;
		PlayFabAuthenticatorSettings.IapApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.IapApiBaseUrl;
		PlayFabAuthenticatorSettings.KidApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.KidApiBaseUrl;
		PlayFabAuthenticatorSettings.MmrApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.MmrApiBaseUrl;
		PlayFabAuthenticatorSettings.ProgressionApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.ProgressionApiBaseUrl;
		PlayFabAuthenticatorSettings.TitleDataApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.TitleDataApiBaseUrl;
		PlayFabAuthenticatorSettings.VotingApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.VotingApiBaseUrl;
	}

	// Token: 0x04004AE4 RID: 19172
	public static string TitleId;

	// Token: 0x04004AE5 RID: 19173
	public static string AuthApiBaseUrl;

	// Token: 0x04004AE6 RID: 19174
	public static string DailyQuestsApiBaseUrl;

	// Token: 0x04004AE7 RID: 19175
	public static string FriendApiBaseUrl;

	// Token: 0x04004AE8 RID: 19176
	public static string HpPromoApiBaseUrl;

	// Token: 0x04004AE9 RID: 19177
	public static string IapApiBaseUrl;

	// Token: 0x04004AEA RID: 19178
	public static string KidApiBaseUrl;

	// Token: 0x04004AEB RID: 19179
	public static string MmrApiBaseUrl;

	// Token: 0x04004AEC RID: 19180
	public static string ProgressionApiBaseUrl;

	// Token: 0x04004AED RID: 19181
	public static string TitleDataApiBaseUrl;

	// Token: 0x04004AEE RID: 19182
	public static string VotingApiBaseUrl;
}
