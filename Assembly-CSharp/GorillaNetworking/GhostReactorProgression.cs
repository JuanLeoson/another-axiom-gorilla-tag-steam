using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace GorillaNetworking
{
	// Token: 0x02000D78 RID: 3448
	public class GhostReactorProgression : MonoBehaviour
	{
		// Token: 0x060055F4 RID: 22004 RVA: 0x001AB0B4 File Offset: 0x001A92B4
		public void Awake()
		{
			GhostReactorProgression.instance = this;
		}

		// Token: 0x060055F5 RID: 22005 RVA: 0x001AB0BC File Offset: 0x001A92BC
		private Task WaitForSessionToken()
		{
			GhostReactorProgression.<WaitForSessionToken>d__6 <WaitForSessionToken>d__;
			<WaitForSessionToken>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitForSessionToken>d__.<>1__state = -1;
			<WaitForSessionToken>d__.<>t__builder.Start<GhostReactorProgression.<WaitForSessionToken>d__6>(ref <WaitForSessionToken>d__);
			return <WaitForSessionToken>d__.<>t__builder.Task;
		}

		// Token: 0x060055F6 RID: 22006 RVA: 0x001AB0F8 File Offset: 0x001A92F8
		public void GetStartingProgression(GRPlayer grPlayer)
		{
			GhostReactorProgression.<GetStartingProgression>d__7 <GetStartingProgression>d__;
			<GetStartingProgression>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<GetStartingProgression>d__.<>4__this = this;
			<GetStartingProgression>d__.grPlayer = grPlayer;
			<GetStartingProgression>d__.<>1__state = -1;
			<GetStartingProgression>d__.<>t__builder.Start<GhostReactorProgression.<GetStartingProgression>d__7>(ref <GetStartingProgression>d__);
		}

		// Token: 0x060055F7 RID: 22007 RVA: 0x001AB137 File Offset: 0x001A9337
		private IEnumerator DoGetStartingProgression(GhostReactorProgression.GetProgressionRequest data, GRPlayer grPlayer)
		{
			UnityWebRequest request = this.FormatWebRequest(data, null);
			bool retry = false;
			yield return request.SendWebRequest();
			if (request.result == UnityWebRequest.Result.Success)
			{
				int num = int.Parse(request.downloadHandler.text);
				grPlayer.SetProgressionData(num, num, false);
				yield break;
			}
			Debug.LogError(string.Format("GRP: GetProgression Error: {0} -- raw response: ", request.responseCode) + request.downloadHandler.text);
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				retry = true;
				Debug.LogError(string.Format("GRP: HTTP {0} error: {1}", request.responseCode, request.error));
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				retry = true;
			}
			if (!retry)
			{
				yield break;
			}
			if (this.getProgressionRetryCount < this.maxRetriesOnFail)
			{
				int num2 = (int)Mathf.Pow(2f, (float)(this.getProgressionRetryCount + 1));
				Debug.LogWarning(string.Format("GRP: Retrying Get Progression call... Retry attempt #{0}, waiting for {1} seconds", this.getProgressionRetryCount + 1, num2));
				this.getProgressionRetryCount++;
				yield return new WaitForSeconds((float)num2);
				this.GetStartingProgression(grPlayer);
			}
			else
			{
				Debug.LogError("GRP: Maximum Get Progression retries attempted. Please check your network connection.");
				this.getProgressionRetryCount = 0;
			}
			yield break;
		}

		// Token: 0x060055F8 RID: 22008 RVA: 0x001AB154 File Offset: 0x001A9354
		public void SetProgression(int progressionAmountToAdd, GRPlayer grPlayer)
		{
			base.StartCoroutine(this.DoSetProgression(new GhostReactorProgression.SetProgressionRequest
			{
				MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
				MothershipId = MothershipClientContext.MothershipId,
				MothershipToken = MothershipClientContext.Token,
				TrackId = this.progressionTrackId,
				Progress = progressionAmountToAdd
			}, grPlayer));
		}

		// Token: 0x060055F9 RID: 22009 RVA: 0x001AB1A8 File Offset: 0x001A93A8
		private IEnumerator DoSetProgression(GhostReactorProgression.SetProgressionRequest data, GRPlayer grPlayer)
		{
			UnityWebRequest request = this.FormatWebRequest(null, data);
			bool retry = false;
			yield return request.SendWebRequest();
			if (request.result == UnityWebRequest.Result.Success)
			{
				GhostReactorProgression.GetProgressionResponse getProgressionResponse = JsonConvert.DeserializeObject<GhostReactorProgression.GetProgressionResponse>(request.downloadHandler.text);
				grPlayer.SetProgressionData(getProgressionResponse.Progress, grPlayer.CurrentProgression.redeemedPoints, false);
				yield break;
			}
			Debug.LogError(string.Format("GRP: SetProgression Error: {0} -- raw response: ", request.responseCode) + request.downloadHandler.text);
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				retry = true;
				Debug.LogError(string.Format("GRP: HTTP {0} error: {1}", request.responseCode, request.error));
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				retry = true;
			}
			if (!retry)
			{
				yield break;
			}
			if (this.setProgressionRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.setProgressionRetryCount + 1));
				Debug.LogWarning(string.Format("GRP: Retrying Set Progression call... Retry attempt #{0}, waiting for {1} seconds", this.setProgressionRetryCount + 1, num));
				this.setProgressionRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.SetProgression(data.Progress, grPlayer);
			}
			else
			{
				Debug.LogError("GRP: Maximum Set Progression retries attempted. Please check your network connection.");
				this.setProgressionRetryCount = 0;
			}
			yield break;
		}

		// Token: 0x060055FA RID: 22010 RVA: 0x001AB1C8 File Offset: 0x001A93C8
		private UnityWebRequest FormatWebRequest(GhostReactorProgression.GetProgressionRequest getRequest = null, GhostReactorProgression.SetProgressionRequest setRequest = null)
		{
			string str;
			byte[] bytes;
			if (getRequest != null)
			{
				str = "/api/GetProgression";
				bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(getRequest));
			}
			else
			{
				str = "/api/SetProgression";
				bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(setRequest));
			}
			UnityWebRequest unityWebRequest = new UnityWebRequest(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl + str, "POST");
			unityWebRequest.uploadHandler = new UploadHandlerRaw(bytes);
			unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
			unityWebRequest.SetRequestHeader("Content-Type", "application/json");
			return unityWebRequest;
		}

		// Token: 0x060055FB RID: 22011 RVA: 0x001AB244 File Offset: 0x001A9444
		public static int PointsToNextGrade(int points)
		{
			GhostReactorProgression.LoadGRPSO();
			int num = 0;
			int num2 = 0;
			int i;
			for (i = 0; i < GhostReactorProgression.grPSO.progressionData.Count; i++)
			{
				num2 = num;
				num += GhostReactorProgression.grPSO.progressionData[i].grades * GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade;
				if (points < num)
				{
					break;
				}
			}
			if (points > num)
			{
				return -1;
			}
			return GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade - (points - num2) % GhostReactorProgression.grPSO.progressionData[GhostReactorProgression.GetTitleLevel(points)].pointsPerGrade;
		}

		// Token: 0x060055FC RID: 22012 RVA: 0x001AB2E4 File Offset: 0x001A94E4
		public static int TotalPointsForNextGrade(int points)
		{
			GhostReactorProgression.LoadGRPSO();
			return GhostReactorProgression.PointsToNextGrade(points) + points;
		}

		// Token: 0x060055FD RID: 22013 RVA: 0x001AB2F4 File Offset: 0x001A94F4
		public static string GetTitleNameAndGrade(int points)
		{
			GhostReactorProgression.LoadGRPSO();
			int num = 0;
			for (int i = 0; i < GhostReactorProgression.grPSO.progressionData.Count; i++)
			{
				num += GhostReactorProgression.grPSO.progressionData[i].grades * GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade;
				if (points < num)
				{
					return GhostReactorProgression.grPSO.progressionData[i].tierName + " " + (GhostReactorProgression.grPSO.progressionData[i].grades - Mathf.FloorToInt((float)((num - points) / GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade)) + 1).ToString();
				}
			}
			return "null";
		}

		// Token: 0x060055FE RID: 22014 RVA: 0x001AB3C0 File Offset: 0x001A95C0
		public static string GetTitleName(int points)
		{
			GhostReactorProgression.LoadGRPSO();
			int num = 0;
			for (int i = 0; i < GhostReactorProgression.grPSO.progressionData.Count; i++)
			{
				num += GhostReactorProgression.grPSO.progressionData[i].grades * GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade;
				if (points < num)
				{
					return GhostReactorProgression.grPSO.progressionData[i].tierName;
				}
			}
			return "null";
		}

		// Token: 0x060055FF RID: 22015 RVA: 0x001AB43C File Offset: 0x001A963C
		public static string GetTitleNameFromLevel(int level)
		{
			GhostReactorProgression.LoadGRPSO();
			for (int i = 0; i < GhostReactorProgression.grPSO.progressionData.Count; i++)
			{
				if (GhostReactorProgression.grPSO.progressionData[i].tierId >= level)
				{
					return GhostReactorProgression.grPSO.progressionData[i].tierName;
				}
			}
			return "null";
		}

		// Token: 0x06005600 RID: 22016 RVA: 0x001AB49C File Offset: 0x001A969C
		public static int GetGrade(int points)
		{
			GhostReactorProgression.LoadGRPSO();
			int num = 0;
			for (int i = 0; i < GhostReactorProgression.grPSO.progressionData.Count; i++)
			{
				num += GhostReactorProgression.grPSO.progressionData[i].grades * GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade;
				if (points < num)
				{
					return GhostReactorProgression.grPSO.progressionData[i].grades - Mathf.FloorToInt((float)((num - points) / GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade)) + 1;
				}
			}
			return -1;
		}

		// Token: 0x06005601 RID: 22017 RVA: 0x001AB538 File Offset: 0x001A9738
		public static int GetTitleLevel(int points)
		{
			GhostReactorProgression.LoadGRPSO();
			int num = 0;
			for (int i = 0; i < GhostReactorProgression.grPSO.progressionData.Count; i++)
			{
				num += GhostReactorProgression.grPSO.progressionData[i].grades * GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade;
				if (points < num)
				{
					return GhostReactorProgression.grPSO.progressionData[i].tierId;
				}
			}
			return -1;
		}

		// Token: 0x06005602 RID: 22018 RVA: 0x001AB5AF File Offset: 0x001A97AF
		public static void LoadGRPSO()
		{
			if (GhostReactorProgression.grPSO == null)
			{
				GhostReactorProgression.grPSO = Resources.Load<GRProgressionScriptableObject>("ProgressionTiersData");
			}
		}

		// Token: 0x04005FA6 RID: 24486
		public static GhostReactorProgression instance;

		// Token: 0x04005FA7 RID: 24487
		private string progressionTrackId = "a0208736-e696-489b-81cd-c0c772489cc5";

		// Token: 0x04005FA8 RID: 24488
		private int setProgressionRetryCount;

		// Token: 0x04005FA9 RID: 24489
		private int getProgressionRetryCount;

		// Token: 0x04005FAA RID: 24490
		private int maxRetriesOnFail = 3;

		// Token: 0x04005FAB RID: 24491
		public static GRProgressionScriptableObject grPSO;

		// Token: 0x04005FAC RID: 24492
		public const string grPSODirectory = "ProgressionTiersData";

		// Token: 0x02000D79 RID: 3449
		[Serializable]
		private class GetProgressionRequest
		{
			// Token: 0x04005FAD RID: 24493
			public string MothershipEnvId;

			// Token: 0x04005FAE RID: 24494
			public string MothershipId;

			// Token: 0x04005FAF RID: 24495
			public string MothershipToken;

			// Token: 0x04005FB0 RID: 24496
			public string TrackId;
		}

		// Token: 0x02000D7A RID: 3450
		[Serializable]
		private class GetProgressionResponse
		{
			// Token: 0x04005FB1 RID: 24497
			public string Track;

			// Token: 0x04005FB2 RID: 24498
			public int Progress;

			// Token: 0x04005FB3 RID: 24499
			public int StatusCode;

			// Token: 0x04005FB4 RID: 24500
			public string Error;
		}

		// Token: 0x02000D7B RID: 3451
		[Serializable]
		private class SetProgressionRequest
		{
			// Token: 0x04005FB5 RID: 24501
			public string MothershipEnvId;

			// Token: 0x04005FB6 RID: 24502
			public string MothershipId;

			// Token: 0x04005FB7 RID: 24503
			public string MothershipToken;

			// Token: 0x04005FB8 RID: 24504
			public string TrackId;

			// Token: 0x04005FB9 RID: 24505
			public int Progress;
		}

		// Token: 0x02000D7C RID: 3452
		[Serializable]
		private class SetProgressionResponse
		{
			// Token: 0x04005FBA RID: 24506
			public string Track;

			// Token: 0x04005FBB RID: 24507
			public int Progress;

			// Token: 0x04005FBC RID: 24508
			public int StatusCode;

			// Token: 0x04005FBD RID: 24509
			public string Error;
		}
	}
}
