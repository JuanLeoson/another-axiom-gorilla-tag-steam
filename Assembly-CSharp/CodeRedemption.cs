using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000427 RID: 1063
public class CodeRedemption : MonoBehaviour
{
	// Token: 0x060019BB RID: 6587 RVA: 0x0008A356 File Offset: 0x00088556
	public void Awake()
	{
		if (CodeRedemption.Instance == null)
		{
			CodeRedemption.Instance = this;
			return;
		}
		if (CodeRedemption.Instance != this)
		{
			Object.Destroy(this);
		}
	}

	// Token: 0x060019BC RID: 6588 RVA: 0x0008A388 File Offset: 0x00088588
	public void HandleCodeRedemption(string code)
	{
		string playFabPlayerId = PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		string playFabSessionTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket();
		string text = string.Concat(new string[]
		{
			"{ \"itemGUID\": \"",
			code,
			"\", \"playFabID\": \"",
			playFabPlayerId,
			"\", \"playFabSessionTicket\": \"",
			playFabSessionTicket,
			"\" }"
		});
		Debug.Log("[CodeRedemption] Web Request body: \n" + text);
		base.StartCoroutine(CodeRedemption.ProcessWebRequest(PlayFabAuthenticatorSettings.HpPromoApiBaseUrl + "/api/ConsumeCodeItem", text, "application/json", new Action<UnityWebRequest>(this.OnCodeRedemptionResponse)));
	}

	// Token: 0x060019BD RID: 6589 RVA: 0x0008A428 File Offset: 0x00088628
	private void OnCodeRedemptionResponse(UnityWebRequest completedRequest)
	{
		if (completedRequest.result != UnityWebRequest.Result.Success)
		{
			Debug.LogError("[CodeRedemption] Web Request failed: " + completedRequest.error + "\nDetails: " + completedRequest.downloadHandler.text);
			GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.Invalid;
			return;
		}
		string text = string.Empty;
		try
		{
			CodeRedemption.CodeRedemptionRequest codeRedemptionRequest = JsonUtility.FromJson<CodeRedemption.CodeRedemptionRequest>(completedRequest.downloadHandler.text);
			if (codeRedemptionRequest.result.Contains("AlreadyRedeemed", StringComparison.OrdinalIgnoreCase))
			{
				Debug.Log("[CodeRedemption] Item has already been redeemed!");
				GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.AlreadyUsed;
				return;
			}
			text = codeRedemptionRequest.playFabItemName;
		}
		catch (Exception ex)
		{
			string str = "[CodeRedemption] Error parsing JSON response: ";
			Exception ex2 = ex;
			Debug.LogError(str + ((ex2 != null) ? ex2.ToString() : null));
			GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.Invalid;
			return;
		}
		Debug.Log("[CodeRedemption] Item successfully granted, processing external unlock...");
		GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.Success;
		GorillaComputer.instance.RedemptionCode = "";
		base.StartCoroutine(this.CheckProcessExternalUnlock(new string[]
		{
			text
		}, true, true, true));
	}

	// Token: 0x060019BE RID: 6590 RVA: 0x0008A53C File Offset: 0x0008873C
	private IEnumerator CheckProcessExternalUnlock(string[] itemIDs, bool autoEquip, bool isLeftHand, bool destroyOnFinish)
	{
		Debug.Log("[CodeRedemption] Checking if we can process external cosmetic unlock...");
		while (!CosmeticsController.instance.allCosmeticsDict_isInitialized || !CosmeticsV2Spawner_Dirty.allPartsInstantiated)
		{
			yield return null;
		}
		Debug.Log("[CodeRedemption] Cosmetics initialized, proceeding to process external unlock...");
		foreach (string itemID in itemIDs)
		{
			CosmeticsController.instance.ProcessExternalUnlock(itemID, autoEquip, isLeftHand);
		}
		yield break;
	}

	// Token: 0x060019BF RID: 6591 RVA: 0x0008A559 File Offset: 0x00088759
	private static IEnumerator ProcessWebRequest(string url, string data, string contentType, Action<UnityWebRequest> callback)
	{
		UnityWebRequest request = UnityWebRequest.Post(url, data, contentType);
		yield return request.SendWebRequest();
		callback(request);
		yield break;
	}

	// Token: 0x04002216 RID: 8726
	public static volatile CodeRedemption Instance;

	// Token: 0x04002217 RID: 8727
	private const string HiddenPathCollabEndpoint = "/api/ConsumeCodeItem";

	// Token: 0x02000428 RID: 1064
	[Serializable]
	public class CodeRedemptionRequest
	{
		// Token: 0x04002218 RID: 8728
		public string result;

		// Token: 0x04002219 RID: 8729
		public string itemID;

		// Token: 0x0400221A RID: 8730
		public string playFabItemName;
	}
}
