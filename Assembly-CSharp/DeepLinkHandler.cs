using System;
using System.Collections;
using GorillaNetworking;
using Oculus.Platform;
using Oculus.Platform.Models;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x020004BC RID: 1212
public class DeepLinkHandler : MonoBehaviour
{
	// Token: 0x06001DE4 RID: 7652 RVA: 0x0009FFE0 File Offset: 0x0009E1E0
	public void Awake()
	{
		if (DeepLinkHandler.instance == null)
		{
			DeepLinkHandler.instance = this;
			return;
		}
		if (DeepLinkHandler.instance != this)
		{
			Object.Destroy(this);
		}
	}

	// Token: 0x06001DE5 RID: 7653 RVA: 0x000A0010 File Offset: 0x0009E210
	public static void Initialize(GameObject parent)
	{
		if (DeepLinkHandler.instance == null && parent != null)
		{
			parent.AddComponent<DeepLinkHandler>();
		}
		if (DeepLinkHandler.instance == null)
		{
			return;
		}
		DeepLinkHandler.instance.RefreshLaunchDetails();
		if (DeepLinkHandler.instance.cachedLaunchDetails != null && DeepLinkHandler.instance.cachedLaunchDetails.LaunchType == LaunchType.Deeplink)
		{
			DeepLinkHandler.instance.HandleDeepLink();
			return;
		}
		Object.Destroy(DeepLinkHandler.instance);
	}

	// Token: 0x06001DE6 RID: 7654 RVA: 0x000A0094 File Offset: 0x0009E294
	private void RefreshLaunchDetails()
	{
		if (UnityEngine.Application.platform != RuntimePlatform.Android)
		{
			GTDev.Log<string>("[DeepLinkHandler::RefreshLaunchDetails] Not on Android Platform!", null);
			return;
		}
		this.cachedLaunchDetails = ApplicationLifecycle.GetLaunchDetails();
		GTDev.Log<string>(string.Concat(new string[]
		{
			"[DeepLinkHandler::RefreshLaunchDetails] LaunchType: ",
			this.cachedLaunchDetails.LaunchType.ToString(),
			"\n[DeepLinkHandler::RefreshLaunchDetails] LaunchSource: ",
			this.cachedLaunchDetails.LaunchSource,
			"\n[DeepLinkHandler::RefreshLaunchDetails] DeepLinkMessage: ",
			this.cachedLaunchDetails.DeeplinkMessage
		}), null);
	}

	// Token: 0x06001DE7 RID: 7655 RVA: 0x000A011F File Offset: 0x0009E31F
	private static IEnumerator ProcessWebRequest(string url, string data, string contentType, Action<UnityWebRequest> callback)
	{
		UnityWebRequest request = UnityWebRequest.Post(url, data, contentType);
		yield return request.SendWebRequest();
		callback(request);
		yield break;
	}

	// Token: 0x06001DE8 RID: 7656 RVA: 0x000A0144 File Offset: 0x0009E344
	private void HandleDeepLink()
	{
		GTDev.Log<string>("[DeepLinkHandler::HandleDeepLink] Handling deep link...", null);
		if (this.cachedLaunchDetails.LaunchSource.Contains("7221491444554579"))
		{
			GTDev.Log<string>("[DeepLinkHandler::HandleDeepLink] DeepLink received from Witchblood, processing...", null);
			string deeplinkMessage = this.cachedLaunchDetails.DeeplinkMessage;
			string launchSource = this.cachedLaunchDetails.LaunchSource;
			string userID = PlayFabAuthenticator.instance.userID;
			string playFabPlayerId = PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			string playFabSessionTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket();
			string text = string.Concat(new string[]
			{
				"{ \"itemGUID\": \"",
				deeplinkMessage,
				"\", \"launchSource\": \"",
				launchSource,
				"\", \"oculusUserID\": \"",
				userID,
				"\", \"playFabID\": \"",
				playFabPlayerId,
				"\", \"playFabSessionTicket\": \"",
				playFabSessionTicket,
				"\" }"
			});
			GTDev.Log<string>("[DeepLinkHandler::HandleDeepLink] Web Request body: \n" + text, null);
			base.StartCoroutine(DeepLinkHandler.ProcessWebRequest(PlayFabAuthenticatorSettings.HpPromoApiBaseUrl + "/api/ConsumeItem", text, "application/json", new Action<UnityWebRequest>(this.OnWitchbloodCollabResponse)));
			return;
		}
		if (this.cachedLaunchDetails.LaunchSource.Contains("1903584373052985"))
		{
			GTDev.Log<string>("[DeepLinkHandler::HandleDeepLink] DeepLink received from Racoon Lagoon, processing...", null);
			string deeplinkMessage2 = this.cachedLaunchDetails.DeeplinkMessage;
			string launchSource2 = this.cachedLaunchDetails.LaunchSource;
			string userID2 = PlayFabAuthenticator.instance.userID;
			string playFabPlayerId2 = PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			string playFabSessionTicket2 = PlayFabAuthenticator.instance.GetPlayFabSessionTicket();
			string text2 = string.Concat(new string[]
			{
				"{ \"itemGUID\": \"",
				deeplinkMessage2,
				"\", \"launchSource\": \"",
				launchSource2,
				"\", \"oculusUserID\": \"",
				userID2,
				"\", \"playFabID\": \"",
				playFabPlayerId2,
				"\", \"playFabSessionTicket\": \"",
				playFabSessionTicket2,
				"\" }"
			});
			GTDev.Log<string>("[DeepLinkHandler::HandleDeepLink] Web Request body: \n" + text2, null);
			base.StartCoroutine(DeepLinkHandler.ProcessWebRequest(PlayFabAuthenticatorSettings.HpPromoApiBaseUrl + "/api/ConsumeItem", text2, "application/json", new Action<UnityWebRequest>(this.OnRaccoonLagoonCollabResponse)));
			return;
		}
		GTDev.LogError<string>("[DeepLinkHandler::HandleDeepLink] App launched via DeepLink, but from an unknown app. App ID: " + this.cachedLaunchDetails.LaunchSource, null);
		Object.Destroy(this);
	}

	// Token: 0x06001DE9 RID: 7657 RVA: 0x000A037C File Offset: 0x0009E57C
	private void OnWitchbloodCollabResponse(UnityWebRequest completedRequest)
	{
		if (completedRequest.result != UnityWebRequest.Result.Success)
		{
			GTDev.LogError<string>("[DeepLinkHandler::OnWitchbloodCollabResponse] Web Request failed: " + completedRequest.error + "\nDetails: " + completedRequest.downloadHandler.text, null);
			Object.Destroy(this);
			return;
		}
		if (completedRequest.downloadHandler.text.Contains("AlreadyRedeemed", StringComparison.OrdinalIgnoreCase))
		{
			GTDev.Log<string>("[DeepLinkHandler::OnWitchbloodCollabResponse] Item has already been redeemed!", null);
			Object.Destroy(this);
			return;
		}
		GTDev.Log<string>("[DeepLinkHandler::OnWitchbloodCollabResponse] Item successfully granted, processing external unlock...", null);
		base.StartCoroutine(this.CheckProcessExternalUnlock(this.WitchbloodCollabCosmeticID, true, true, true));
	}

	// Token: 0x06001DEA RID: 7658 RVA: 0x000A040C File Offset: 0x0009E60C
	private void OnRaccoonLagoonCollabResponse(UnityWebRequest completedRequest)
	{
		if (completedRequest.result != UnityWebRequest.Result.Success)
		{
			GTDev.LogError<string>("[DeepLinkHandler::OnRaccoonLagoonCollabResponse] Web Request failed: " + completedRequest.error + "\nDetails: " + completedRequest.downloadHandler.text, null);
			Object.Destroy(this);
			return;
		}
		if (completedRequest.downloadHandler.text.Contains("AlreadyRedeemed", StringComparison.OrdinalIgnoreCase))
		{
			GTDev.Log<string>("[DeepLinkHandler::OnRaccoonLagoonCollabResponse] Item has already been redeemed!", null);
			Object.Destroy(this);
			return;
		}
		GTDev.Log<string>("[DeepLinkHandler::OnRaccoonLagoonCollabResponse] Item successfully granted, processing external unlock...", null);
		base.StartCoroutine(this.CheckProcessExternalUnlock(this.RaccoonLagoonCosmeticIDs, true, true, true));
	}

	// Token: 0x06001DEB RID: 7659 RVA: 0x000A049A File Offset: 0x0009E69A
	private IEnumerator CheckProcessExternalUnlock(string[] itemIDs, bool autoEquip, bool isLeftHand, bool destroyOnFinish)
	{
		GTDev.Log<string>("[DeepLinkHandler::CheckProcessExternalUnlock] Checking if we can process external cosmetic unlock...", null);
		while (!CosmeticsController.instance.allCosmeticsDict_isInitialized || !CosmeticsV2Spawner_Dirty.allPartsInstantiated)
		{
			yield return null;
		}
		GTDev.Log<string>("[DeepLinkHandler::CheckProcessExternalUnlock] Cosmetics initialized, proceeding to process external unlock...", null);
		foreach (string itemID in itemIDs)
		{
			CosmeticsController.instance.ProcessExternalUnlock(itemID, autoEquip, isLeftHand);
		}
		if (destroyOnFinish)
		{
			Object.Destroy(this);
		}
		yield break;
	}

	// Token: 0x04002687 RID: 9863
	public static volatile DeepLinkHandler instance;

	// Token: 0x04002688 RID: 9864
	private LaunchDetails cachedLaunchDetails;

	// Token: 0x04002689 RID: 9865
	private const string WitchbloodAppID = "7221491444554579";

	// Token: 0x0400268A RID: 9866
	private readonly string[] WitchbloodCollabCosmeticID = new string[]
	{
		"LMAKT."
	};

	// Token: 0x0400268B RID: 9867
	private const string RaccoonLagoonAppID = "1903584373052985";

	// Token: 0x0400268C RID: 9868
	private readonly string[] RaccoonLagoonCosmeticIDs = new string[]
	{
		"LMALI.",
		"LHAGS."
	};

	// Token: 0x0400268D RID: 9869
	private const string HiddenPathCollabEndpoint = "/api/ConsumeItem";
}
