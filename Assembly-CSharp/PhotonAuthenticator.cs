using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000A0F RID: 2575
public class PhotonAuthenticator : MonoBehaviour
{
	// Token: 0x06003ED9 RID: 16089 RVA: 0x0013FBEA File Offset: 0x0013DDEA
	private void Awake()
	{
		Debug.Log("Environment is *************** PRODUCTION PUN *******************");
		PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = PhotonAuthenticatorSettings.PunAppId;
		PhotonNetwork.PhotonServerSettings.AppSettings.AppIdVoice = PhotonAuthenticatorSettings.VoiceAppId;
	}

	// Token: 0x06003EDA RID: 16090 RVA: 0x0013FC20 File Offset: 0x0013DE20
	public void SetCustomAuthenticationParameters(Dictionary<string, object> customAuthData)
	{
		AuthenticationValues authenticationValues = new AuthenticationValues();
		authenticationValues.AuthType = CustomAuthenticationType.Custom;
		authenticationValues.SetAuthPostData(customAuthData);
		NetworkSystem.Instance.SetAuthenticationValues(authenticationValues);
		Debug.Log("Set Photon auth data. AppVersion is: " + NetworkSystemConfig.AppVersion);
	}
}
