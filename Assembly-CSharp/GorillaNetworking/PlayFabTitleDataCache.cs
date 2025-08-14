using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using LitJson;
using PlayFab;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace GorillaNetworking
{
	// Token: 0x02000DB2 RID: 3506
	public class PlayFabTitleDataCache : MonoBehaviour
	{
		// Token: 0x17000856 RID: 2134
		// (get) Token: 0x06005707 RID: 22279 RVA: 0x001B01C8 File Offset: 0x001AE3C8
		// (set) Token: 0x06005708 RID: 22280 RVA: 0x001B01CF File Offset: 0x001AE3CF
		public static PlayFabTitleDataCache Instance { get; private set; }

		// Token: 0x17000857 RID: 2135
		// (get) Token: 0x06005709 RID: 22281 RVA: 0x001B01D7 File Offset: 0x001AE3D7
		private static string FilePath
		{
			get
			{
				return Path.Combine(Application.persistentDataPath, "TitleDataCache.json");
			}
		}

		// Token: 0x0600570A RID: 22282 RVA: 0x001B01E8 File Offset: 0x001AE3E8
		public void GetTitleData(string name, Action<string> callback, Action<PlayFabError> errorCallback)
		{
			if (this.isDataUpToDate && this.titleData.ContainsKey(name))
			{
				callback.SafeInvoke(this.titleData[name]);
				return;
			}
			PlayFabTitleDataCache.DataRequest item = new PlayFabTitleDataCache.DataRequest
			{
				Name = name,
				Callback = callback,
				ErrorCallback = errorCallback
			};
			this.requests.Add(item);
			if (this.isDataUpToDate && this.updateDataCoroutine == null)
			{
				this.UpdateData();
			}
		}

		// Token: 0x0600570B RID: 22283 RVA: 0x001B025B File Offset: 0x001AE45B
		private void Awake()
		{
			if (PlayFabTitleDataCache.Instance != null)
			{
				Object.Destroy(this);
				return;
			}
			PlayFabTitleDataCache.Instance = this;
		}

		// Token: 0x0600570C RID: 22284 RVA: 0x001B0277 File Offset: 0x001AE477
		private void Start()
		{
			this.UpdateData();
		}

		// Token: 0x0600570D RID: 22285 RVA: 0x001B0280 File Offset: 0x001AE480
		public void LoadDataFromFile()
		{
			try
			{
				if (!File.Exists(PlayFabTitleDataCache.FilePath))
				{
					Debug.LogWarning("Title data file " + PlayFabTitleDataCache.FilePath + " does not exist!");
				}
				else
				{
					string json = File.ReadAllText(PlayFabTitleDataCache.FilePath);
					this.titleData = JsonMapper.ToObject<Dictionary<string, string>>(json);
				}
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Error reading PlayFab title data from file: {0}", arg));
			}
		}

		// Token: 0x0600570E RID: 22286 RVA: 0x001B02F0 File Offset: 0x001AE4F0
		private void SaveDataToFile(string filepath)
		{
			try
			{
				string contents = JsonMapper.ToJson(this.titleData);
				File.WriteAllText(filepath, contents);
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Error writing PlayFab title data to file: {0}", arg));
			}
		}

		// Token: 0x0600570F RID: 22287 RVA: 0x001B0338 File Offset: 0x001AE538
		public void UpdateData()
		{
			this.updateDataCoroutine = base.StartCoroutine(this.UpdateDataCo());
		}

		// Token: 0x06005710 RID: 22288 RVA: 0x001B034C File Offset: 0x001AE54C
		private IEnumerator UpdateDataCo()
		{
			try
			{
				this.LoadDataFromFile();
				this.LoadKey();
				Dictionary<string, string> dictionary = this.titleData;
				Dictionary<string, string> dictionary2 = new Dictionary<string, string>((dictionary != null) ? dictionary.Count : 0);
				if (this.titleData != null)
				{
					foreach (KeyValuePair<string, string> keyValuePair in this.titleData)
					{
						string text;
						string text2;
						keyValuePair.Deconstruct(out text, out text2);
						string text3 = text;
						string text4 = text2;
						if (text3 != null)
						{
							dictionary2[text3] = ((text4 != null) ? PlayFabTitleDataCache.MD5(text4) : null);
						}
					}
				}
				string s = JsonMapper.ToJson(new Dictionary<string, object>
				{
					{
						"version",
						Application.version
					},
					{
						"key",
						this.titleDataKey
					},
					{
						"data",
						dictionary2
					}
				});
				Stopwatch sw = Stopwatch.StartNew();
				Dictionary<string, JsonData> dictionary3;
				using (UnityWebRequest www = new UnityWebRequest(PlayFabAuthenticatorSettings.TitleDataApiBaseUrl, "POST"))
				{
					byte[] bytes = new UTF8Encoding(true).GetBytes(s);
					www.uploadHandler = new UploadHandlerRaw(bytes);
					www.downloadHandler = new DownloadHandlerBuffer();
					www.SetRequestHeader("Content-Type", "application/json");
					www.timeout = 15;
					yield return www.SendWebRequest();
					if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
					{
						Debug.LogError("Failed to get TitleData from the server.\n" + www.error);
						this.ClearRequestWithError(null);
						yield break;
					}
					dictionary3 = JsonMapper.ToObject<Dictionary<string, JsonData>>(www.downloadHandler.text);
				}
				UnityWebRequest www = null;
				Debug.Log(string.Format("TitleData fetched: {0:N5}", sw.Elapsed.TotalSeconds));
				if (dictionary3 != null)
				{
					foreach (KeyValuePair<string, JsonData> keyValuePair2 in dictionary3)
					{
						PlayFabTitleDataCache.DataUpdate onTitleDataUpdate = this.OnTitleDataUpdate;
						if (onTitleDataUpdate != null)
						{
							onTitleDataUpdate.Invoke(keyValuePair2.Key);
						}
						if (keyValuePair2.Value == null)
						{
							Dictionary<string, string> dictionary4 = this.titleData;
							if (dictionary4 != null)
							{
								dictionary4.Remove(keyValuePair2.Key);
							}
						}
						else
						{
							if (this.titleData == null)
							{
								this.titleData = new Dictionary<string, string>();
							}
							this.titleData.AddOrUpdate(keyValuePair2.Key, JsonMapper.ToJson(keyValuePair2.Value));
						}
					}
					if (dictionary3.Keys.Count > 0)
					{
						this.SaveDataToFile(PlayFabTitleDataCache.FilePath);
					}
				}
				this.requests.RemoveAll(delegate(PlayFabTitleDataCache.DataRequest request)
				{
					Dictionary<string, string> dictionary5 = this.titleData;
					string data;
					if (dictionary5 != null && dictionary5.TryGetValue(request.Name, out data))
					{
						request.Callback.SafeInvoke(data);
						return true;
					}
					return false;
				});
				sw = null;
			}
			finally
			{
				this.ClearRequestWithError(null);
				this.isDataUpToDate = true;
				this.updateDataCoroutine = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x06005711 RID: 22289 RVA: 0x001B035C File Offset: 0x001AE55C
		private void LoadKey()
		{
			TextAsset textAsset = Resources.Load<TextAsset>("title_data_key");
			this.titleDataKey = textAsset.text;
		}

		// Token: 0x06005712 RID: 22290 RVA: 0x001B0380 File Offset: 0x001AE580
		private static string MD5(string value)
		{
			HashAlgorithm hashAlgorithm = new MD5CryptoServiceProvider();
			byte[] bytes = Encoding.Default.GetBytes(value);
			byte[] array = hashAlgorithm.ComputeHash(bytes);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in array)
			{
				stringBuilder.Append(b.ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06005713 RID: 22291 RVA: 0x001B03D8 File Offset: 0x001AE5D8
		private void ClearRequestWithError(PlayFabError e = null)
		{
			if (e == null)
			{
				e = new PlayFabError();
			}
			foreach (PlayFabTitleDataCache.DataRequest dataRequest in this.requests)
			{
				dataRequest.ErrorCallback.SafeInvoke(e);
			}
			this.requests.Clear();
		}

		// Token: 0x040060E7 RID: 24807
		public PlayFabTitleDataCache.DataUpdate OnTitleDataUpdate;

		// Token: 0x040060E8 RID: 24808
		private const string FileName = "TitleDataCache.json";

		// Token: 0x040060E9 RID: 24809
		private readonly List<PlayFabTitleDataCache.DataRequest> requests = new List<PlayFabTitleDataCache.DataRequest>();

		// Token: 0x040060EA RID: 24810
		private Dictionary<string, string> titleData = new Dictionary<string, string>();

		// Token: 0x040060EB RID: 24811
		private string titleDataKey;

		// Token: 0x040060EC RID: 24812
		private bool isDataUpToDate;

		// Token: 0x040060ED RID: 24813
		private Coroutine updateDataCoroutine;

		// Token: 0x02000DB3 RID: 3507
		[Serializable]
		public sealed class DataUpdate : UnityEvent<string>
		{
		}

		// Token: 0x02000DB4 RID: 3508
		private class DataRequest
		{
			// Token: 0x17000858 RID: 2136
			// (get) Token: 0x06005717 RID: 22295 RVA: 0x001B04A4 File Offset: 0x001AE6A4
			// (set) Token: 0x06005718 RID: 22296 RVA: 0x001B04AC File Offset: 0x001AE6AC
			public string Name { get; set; }

			// Token: 0x17000859 RID: 2137
			// (get) Token: 0x06005719 RID: 22297 RVA: 0x001B04B5 File Offset: 0x001AE6B5
			// (set) Token: 0x0600571A RID: 22298 RVA: 0x001B04BD File Offset: 0x001AE6BD
			public Action<string> Callback { get; set; }

			// Token: 0x1700085A RID: 2138
			// (get) Token: 0x0600571B RID: 22299 RVA: 0x001B04C6 File Offset: 0x001AE6C6
			// (set) Token: 0x0600571C RID: 22300 RVA: 0x001B04CE File Offset: 0x001AE6CE
			public Action<PlayFabError> ErrorCallback { get; set; }
		}
	}
}
