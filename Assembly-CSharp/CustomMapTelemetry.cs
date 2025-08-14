using System;
using System.Collections;
using ModIO;
using Unity.Profiling;
using UnityEngine;

// Token: 0x0200081D RID: 2077
public class CustomMapTelemetry : MonoBehaviour
{
	// Token: 0x170004F6 RID: 1270
	// (get) Token: 0x0600341C RID: 13340 RVA: 0x0010FA2B File Offset: 0x0010DC2B
	public static bool IsActive
	{
		get
		{
			return CustomMapTelemetry.metricsCaptureStarted || CustomMapTelemetry.perfCaptureStarted;
		}
	}

	// Token: 0x0600341D RID: 13341 RVA: 0x0010FA3B File Offset: 0x0010DC3B
	private void Awake()
	{
		if (CustomMapTelemetry.instance == null)
		{
			CustomMapTelemetry.instance = this;
			return;
		}
		if (CustomMapTelemetry.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x0600341E RID: 13342 RVA: 0x0010FA6F File Offset: 0x0010DC6F
	private static void OnPlayerJoinedRoom(NetPlayer obj)
	{
		CustomMapTelemetry.runningPlayerCount++;
		CustomMapTelemetry.maxPlayersInMap = Math.Max(CustomMapTelemetry.runningPlayerCount, CustomMapTelemetry.maxPlayersInMap);
	}

	// Token: 0x0600341F RID: 13343 RVA: 0x0010FA91 File Offset: 0x0010DC91
	private static void OnPlayerLeftRoom(NetPlayer obj)
	{
		CustomMapTelemetry.runningPlayerCount--;
		CustomMapTelemetry.minPlayersInMap = Math.Min(CustomMapTelemetry.runningPlayerCount, CustomMapTelemetry.minPlayersInMap);
	}

	// Token: 0x06003420 RID: 13344 RVA: 0x0010FAB4 File Offset: 0x0010DCB4
	public static void StartMapTracking()
	{
		if (CustomMapTelemetry.metricsCaptureStarted || CustomMapTelemetry.perfCaptureStarted)
		{
			return;
		}
		CustomMapTelemetry.mapEnterTime = Time.unscaledTime;
		float value = Random.value;
		if (value <= 0.01f)
		{
			CustomMapTelemetry.StartMetricsCapture();
		}
		else if (value >= 0.99f)
		{
			CustomMapTelemetry.StartPerfCapture();
		}
		if (CustomMapTelemetry.metricsCaptureStarted || CustomMapTelemetry.perfCaptureStarted)
		{
			ModIOManager.GetModProfile(new ModId(CustomMapLoader.LoadedMapModId), delegate(ModIORequestResultAnd<ModProfile> resultAndProfile)
			{
				if (resultAndProfile.result.success)
				{
					CustomMapTelemetry.mapName = resultAndProfile.data.name;
					CustomMapTelemetry.mapModId = resultAndProfile.data.id.id;
					CustomMapTelemetry.mapCreatorUsername = resultAndProfile.data.creator.username;
				}
			});
		}
	}

	// Token: 0x06003421 RID: 13345 RVA: 0x0010FB38 File Offset: 0x0010DD38
	public static void EndMapTracking()
	{
		CustomMapTelemetry.EndMetricsCapture();
		CustomMapTelemetry.EndPerfCapture();
		CustomMapTelemetry.mapName = "NULL";
		CustomMapTelemetry.mapCreatorUsername = "NULL";
		CustomMapTelemetry.mapEnterTime = -1f;
		CustomMapTelemetry.mapModId = 0L;
	}

	// Token: 0x06003422 RID: 13346 RVA: 0x0010FB6C File Offset: 0x0010DD6C
	private static void StartMetricsCapture()
	{
		if (CustomMapTelemetry.metricsCaptureStarted)
		{
			return;
		}
		CustomMapTelemetry.metricsCaptureStarted = true;
		NetworkSystem.Instance.OnPlayerJoined -= CustomMapTelemetry.OnPlayerJoinedRoom;
		NetworkSystem.Instance.OnPlayerJoined += CustomMapTelemetry.OnPlayerJoinedRoom;
		NetworkSystem.Instance.OnPlayerLeft -= CustomMapTelemetry.OnPlayerLeftRoom;
		NetworkSystem.Instance.OnPlayerLeft += CustomMapTelemetry.OnPlayerLeftRoom;
		CustomMapTelemetry.runningPlayerCount = NetworkSystem.Instance.RoomPlayerCount;
		CustomMapTelemetry.minPlayersInMap = CustomMapTelemetry.runningPlayerCount;
		CustomMapTelemetry.maxPlayersInMap = CustomMapTelemetry.runningPlayerCount;
	}

	// Token: 0x06003423 RID: 13347 RVA: 0x0010FC30 File Offset: 0x0010DE30
	private static void EndMetricsCapture()
	{
		if (!CustomMapTelemetry.metricsCaptureStarted)
		{
			return;
		}
		CustomMapTelemetry.metricsCaptureStarted = false;
		NetworkSystem.Instance.OnPlayerJoined -= CustomMapTelemetry.OnPlayerJoinedRoom;
		NetworkSystem.Instance.OnPlayerLeft -= CustomMapTelemetry.OnPlayerLeftRoom;
		CustomMapTelemetry.inPrivateRoom = (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate);
		int num = Mathf.RoundToInt(Time.unscaledTime - CustomMapTelemetry.mapEnterTime);
		if (num < 30)
		{
			return;
		}
		if (CustomMapTelemetry.mapName.Equals("NULL") || CustomMapTelemetry.mapModId == 0L)
		{
			Debug.LogError("[CustomMapTelemetry::EndMetricsCapture] mapName or mapModID is invalid, throwing out this capture data...");
			return;
		}
		GorillaTelemetry.PostCustomMapTracking(CustomMapTelemetry.mapName, CustomMapTelemetry.mapModId, CustomMapTelemetry.mapCreatorUsername, CustomMapTelemetry.minPlayersInMap, CustomMapTelemetry.maxPlayersInMap, num, CustomMapTelemetry.inPrivateRoom);
	}

	// Token: 0x06003424 RID: 13348 RVA: 0x0010FD0C File Offset: 0x0010DF0C
	private static void StartPerfCapture()
	{
		if (CustomMapTelemetry.perfCaptureStarted)
		{
			return;
		}
		CustomMapTelemetry.perfCaptureStarted = true;
		if (CustomMapTelemetry.instance.perfCaptureCoroutine != null)
		{
			CustomMapTelemetry.EndPerfCapture();
		}
		CustomMapTelemetry.drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count", 1, ProfilerRecorderOptions.Default);
		CustomMapTelemetry.LowestFPS = int.MaxValue;
		CustomMapTelemetry.HighestFPS = int.MinValue;
		CustomMapTelemetry.totalFPS = 0;
		CustomMapTelemetry.totalDrawCalls = 0;
		CustomMapTelemetry.totalPlayerCount = 0;
		CustomMapTelemetry.frameCounter = 0;
		CustomMapTelemetry.instance.perfCaptureCoroutine = CustomMapTelemetry.instance.StartCoroutine(CustomMapTelemetry.instance.CaptureMapPerformance());
	}

	// Token: 0x06003425 RID: 13349 RVA: 0x0010FDA4 File Offset: 0x0010DFA4
	private static void EndPerfCapture()
	{
		if (!CustomMapTelemetry.perfCaptureStarted)
		{
			return;
		}
		CustomMapTelemetry.perfCaptureStarted = false;
		if (CustomMapTelemetry.instance.perfCaptureCoroutine != null)
		{
			CustomMapTelemetry.instance.StopAllCoroutines();
			CustomMapTelemetry.instance.perfCaptureCoroutine = null;
		}
		CustomMapTelemetry.drawCallsRecorder.Dispose();
		if (CustomMapTelemetry.frameCounter == 0)
		{
			return;
		}
		int num = Mathf.RoundToInt(Time.unscaledTime - CustomMapTelemetry.mapEnterTime);
		CustomMapTelemetry.AverageFPS = CustomMapTelemetry.totalFPS / CustomMapTelemetry.frameCounter;
		CustomMapTelemetry.AverageDrawCalls = CustomMapTelemetry.totalDrawCalls / CustomMapTelemetry.frameCounter;
		CustomMapTelemetry.AveragePlayerCount = CustomMapTelemetry.totalPlayerCount / CustomMapTelemetry.frameCounter;
		if (num < 30)
		{
			return;
		}
		if (CustomMapTelemetry.mapName.Equals("NULL") || CustomMapTelemetry.mapModId == 0L)
		{
			Debug.LogError("[CustomMapTelemetry::EndPerfCapture] mapName or mapModID is invalid, throwing out this capture data...");
			return;
		}
		GorillaTelemetry.PostCustomMapPerformance(CustomMapTelemetry.mapName, CustomMapTelemetry.mapModId, CustomMapTelemetry.LowestFPS, CustomMapTelemetry.LowestFPSDrawCalls, CustomMapTelemetry.LowestFPSPlayerCount, CustomMapTelemetry.AverageFPS, CustomMapTelemetry.AverageDrawCalls, CustomMapTelemetry.AveragePlayerCount, CustomMapTelemetry.HighestFPS, CustomMapTelemetry.HighestFPSDrawCalls, CustomMapTelemetry.HighestFPSPlayerCount, num);
	}

	// Token: 0x06003426 RID: 13350 RVA: 0x0010FE9F File Offset: 0x0010E09F
	private IEnumerator CaptureMapPerformance()
	{
		for (;;)
		{
			int num = Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
			int num2 = Mathf.RoundToInt((float)CustomMapTelemetry.drawCallsRecorder.LastValue);
			int roomPlayerCount = NetworkSystem.Instance.RoomPlayerCount;
			CustomMapTelemetry.totalFPS += num;
			CustomMapTelemetry.totalDrawCalls += num2;
			CustomMapTelemetry.totalPlayerCount += roomPlayerCount;
			if (num > CustomMapTelemetry.HighestFPS)
			{
				CustomMapTelemetry.HighestFPS = num;
				CustomMapTelemetry.HighestFPSDrawCalls = num2;
				CustomMapTelemetry.HighestFPSPlayerCount = roomPlayerCount;
			}
			if (num < CustomMapTelemetry.LowestFPS)
			{
				CustomMapTelemetry.LowestFPS = num;
				CustomMapTelemetry.LowestFPSDrawCalls = num2;
				CustomMapTelemetry.LowestFPSPlayerCount = roomPlayerCount;
			}
			CustomMapTelemetry.frameCounter++;
			yield return null;
		}
		yield break;
	}

	// Token: 0x06003427 RID: 13351 RVA: 0x0010FEA7 File Offset: 0x0010E0A7
	private void OnDestroy()
	{
		if (this.perfCaptureCoroutine != null)
		{
			CustomMapTelemetry.EndMapTracking();
		}
	}

	// Token: 0x040040FC RID: 16636
	[OnEnterPlay_SetNull]
	private static volatile CustomMapTelemetry instance;

	// Token: 0x040040FD RID: 16637
	private static string mapName;

	// Token: 0x040040FE RID: 16638
	private static long mapModId;

	// Token: 0x040040FF RID: 16639
	private static string mapCreatorUsername;

	// Token: 0x04004100 RID: 16640
	private static bool metricsCaptureStarted;

	// Token: 0x04004101 RID: 16641
	private static float mapEnterTime;

	// Token: 0x04004102 RID: 16642
	private static int runningPlayerCount;

	// Token: 0x04004103 RID: 16643
	private static int minPlayersInMap;

	// Token: 0x04004104 RID: 16644
	private static int maxPlayersInMap;

	// Token: 0x04004105 RID: 16645
	private static bool inPrivateRoom;

	// Token: 0x04004106 RID: 16646
	private const int minimumPlaytimeForTracking = 30;

	// Token: 0x04004107 RID: 16647
	private static int LowestFPS = int.MaxValue;

	// Token: 0x04004108 RID: 16648
	private static int LowestFPSDrawCalls;

	// Token: 0x04004109 RID: 16649
	private static int LowestFPSPlayerCount;

	// Token: 0x0400410A RID: 16650
	private static int AverageFPS;

	// Token: 0x0400410B RID: 16651
	private static int AverageDrawCalls;

	// Token: 0x0400410C RID: 16652
	private static int AveragePlayerCount;

	// Token: 0x0400410D RID: 16653
	private static int HighestFPS = int.MinValue;

	// Token: 0x0400410E RID: 16654
	private static int HighestFPSDrawCalls;

	// Token: 0x0400410F RID: 16655
	private static int HighestFPSPlayerCount;

	// Token: 0x04004110 RID: 16656
	private static int totalFPS;

	// Token: 0x04004111 RID: 16657
	private static int totalDrawCalls;

	// Token: 0x04004112 RID: 16658
	private static int totalPlayerCount;

	// Token: 0x04004113 RID: 16659
	private static int frameCounter;

	// Token: 0x04004114 RID: 16660
	private Coroutine perfCaptureCoroutine;

	// Token: 0x04004115 RID: 16661
	private static ProfilerRecorder drawCallsRecorder;

	// Token: 0x04004116 RID: 16662
	private static bool perfCaptureStarted;
}
