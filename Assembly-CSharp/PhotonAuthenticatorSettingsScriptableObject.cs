using System;
using UnityEngine;

// Token: 0x02000A11 RID: 2577
[CreateAssetMenu(fileName = "PhotonAuthenticatorSettings", menuName = "ScriptableObjects/PhotonAuthenticatorSettings")]
public class PhotonAuthenticatorSettingsScriptableObject : ScriptableObject
{
	// Token: 0x04004AE1 RID: 19169
	public string PunAppId;

	// Token: 0x04004AE2 RID: 19170
	public string FusionAppId;

	// Token: 0x04004AE3 RID: 19171
	public string VoiceAppId;
}
