using System;
using UnityEngine;

// Token: 0x02000A10 RID: 2576
public class PhotonAuthenticatorSettings
{
	// Token: 0x06003EDC RID: 16092 RVA: 0x0013FC60 File Offset: 0x0013DE60
	static PhotonAuthenticatorSettings()
	{
		PhotonAuthenticatorSettings.Load("PhotonAuthenticatorSettings");
	}

	// Token: 0x06003EDD RID: 16093 RVA: 0x0013FC6C File Offset: 0x0013DE6C
	public static void Load(string path)
	{
		PhotonAuthenticatorSettingsScriptableObject photonAuthenticatorSettingsScriptableObject = Resources.Load<PhotonAuthenticatorSettingsScriptableObject>(path);
		PhotonAuthenticatorSettings.PunAppId = photonAuthenticatorSettingsScriptableObject.PunAppId;
		PhotonAuthenticatorSettings.FusionAppId = photonAuthenticatorSettingsScriptableObject.FusionAppId;
		PhotonAuthenticatorSettings.VoiceAppId = photonAuthenticatorSettingsScriptableObject.VoiceAppId;
	}

	// Token: 0x04004ADE RID: 19166
	public static string PunAppId;

	// Token: 0x04004ADF RID: 19167
	public static string FusionAppId;

	// Token: 0x04004AE0 RID: 19168
	public static string VoiceAppId;
}
