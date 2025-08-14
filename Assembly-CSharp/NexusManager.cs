using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using NexusSDK;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x0200046B RID: 1131
public class NexusManager : MonoBehaviour
{
	// Token: 0x06001C21 RID: 7201 RVA: 0x0009788B File Offset: 0x00095A8B
	private void Awake()
	{
		if (NexusManager.instance == null)
		{
			NexusManager.instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06001C22 RID: 7202 RVA: 0x000978A7 File Offset: 0x00095AA7
	private void Start()
	{
		SDKInitializer.Init(this.publicApiKey, this.environment);
	}

	// Token: 0x06001C23 RID: 7203 RVA: 0x000978BA File Offset: 0x00095ABA
	public static IEnumerator GetMembers(NexusManager.GetMembersRequest RequestParams, Action<AttributionAPI.GetMembers200Response> onSuccess, Action<string> onFailure)
	{
		string text = SDKInitializer.ApiBaseUrl + "/manage/members";
		List<string> list = new List<string>();
		if (RequestParams.page != 0)
		{
			list.Add("page=" + RequestParams.page.ToString());
		}
		if (RequestParams.pageSize != 0)
		{
			list.Add("pageSize=" + RequestParams.pageSize.ToString());
		}
		text += "?";
		text += string.Join("&", list);
		using (UnityWebRequest webRequest = UnityWebRequest.Get(text))
		{
			webRequest.SetRequestHeader("x-shared-secret", SDKInitializer.ApiKey);
			yield return webRequest.SendWebRequest();
			if (webRequest.responseCode == 200L)
			{
				AttributionAPI.GetMembers200Response obj = JsonConvert.DeserializeObject<AttributionAPI.GetMembers200Response>(webRequest.downloadHandler.text, new JsonSerializerSettings
				{
					NullValueHandling = NullValueHandling.Ignore
				});
				if (onSuccess != null)
				{
					onSuccess(obj);
				}
			}
			else if (onFailure != null)
			{
				onFailure(webRequest.error);
			}
		}
		UnityWebRequest webRequest = null;
		yield break;
		yield break;
	}

	// Token: 0x06001C24 RID: 7204 RVA: 0x000978D8 File Offset: 0x00095AD8
	public void VerifyCreatorCode(string code, Action<Member> onSuccess, Action onFailure)
	{
		NexusManager.GetMemberByCodeRequest requestParams = new NexusManager.GetMemberByCodeRequest
		{
			memberCode = code
		};
		base.StartCoroutine(NexusManager.GetMemberByCode(requestParams, onSuccess, onFailure));
	}

	// Token: 0x06001C25 RID: 7205 RVA: 0x00097906 File Offset: 0x00095B06
	public static IEnumerator GetMemberByCode(NexusManager.GetMemberByCodeRequest RequestParams, Action<Member> onSuccess, Action onFailure)
	{
		string text = SDKInitializer.ApiBaseUrl + "/manage/members/{memberCode}";
		text = text.Replace("{memberCode}", RequestParams.memberCode);
		List<string> values = new List<string>();
		text += "?";
		text += string.Join("&", values);
		using (UnityWebRequest webRequest = UnityWebRequest.Get(text))
		{
			webRequest.SetRequestHeader("x-shared-secret", SDKInitializer.ApiKey);
			yield return webRequest.SendWebRequest();
			if (webRequest.responseCode == 200L)
			{
				Member obj = JsonConvert.DeserializeObject<Member>(webRequest.downloadHandler.text, new JsonSerializerSettings
				{
					NullValueHandling = NullValueHandling.Ignore
				});
				if (onSuccess != null)
				{
					onSuccess(obj);
				}
			}
			else if (onFailure != null)
			{
				onFailure();
			}
		}
		UnityWebRequest webRequest = null;
		yield break;
		yield break;
	}

	// Token: 0x040024AB RID: 9387
	private string publicApiKey = "nexus_pk_4c18dcb1531846c7abad4cb00c5242bb";

	// Token: 0x040024AC RID: 9388
	private string environment = "production";

	// Token: 0x040024AD RID: 9389
	public static NexusManager instance;

	// Token: 0x040024AE RID: 9390
	private Member[] validatedMembers;

	// Token: 0x0200046C RID: 1132
	[Serializable]
	public struct GetMemberByCodeRequest
	{
		// Token: 0x17000305 RID: 773
		// (get) Token: 0x06001C27 RID: 7207 RVA: 0x00097941 File Offset: 0x00095B41
		// (set) Token: 0x06001C28 RID: 7208 RVA: 0x00097949 File Offset: 0x00095B49
		public string memberCode { readonly get; set; }

		// Token: 0x17000306 RID: 774
		// (get) Token: 0x06001C29 RID: 7209 RVA: 0x00097952 File Offset: 0x00095B52
		// (set) Token: 0x06001C2A RID: 7210 RVA: 0x0009795A File Offset: 0x00095B5A
		public string groupId { readonly get; set; }
	}

	// Token: 0x0200046D RID: 1133
	[Serializable]
	public struct GetMembersRequest
	{
		// Token: 0x17000307 RID: 775
		// (get) Token: 0x06001C2B RID: 7211 RVA: 0x00097963 File Offset: 0x00095B63
		// (set) Token: 0x06001C2C RID: 7212 RVA: 0x0009796B File Offset: 0x00095B6B
		public int page { readonly get; set; }

		// Token: 0x17000308 RID: 776
		// (get) Token: 0x06001C2D RID: 7213 RVA: 0x00097974 File Offset: 0x00095B74
		// (set) Token: 0x06001C2E RID: 7214 RVA: 0x0009797C File Offset: 0x00095B7C
		public int pageSize { readonly get; set; }
	}
}
