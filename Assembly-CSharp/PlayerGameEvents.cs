using System;

// Token: 0x02000137 RID: 311
public class PlayerGameEvents
{
	// Token: 0x14000013 RID: 19
	// (add) Token: 0x06000815 RID: 2069 RVA: 0x0002D204 File Offset: 0x0002B404
	// (remove) Token: 0x06000816 RID: 2070 RVA: 0x0002D238 File Offset: 0x0002B438
	public static event Action<string> OnGameModeObjectiveTrigger;

	// Token: 0x14000014 RID: 20
	// (add) Token: 0x06000817 RID: 2071 RVA: 0x0002D26C File Offset: 0x0002B46C
	// (remove) Token: 0x06000818 RID: 2072 RVA: 0x0002D2A0 File Offset: 0x0002B4A0
	public static event Action<string> OnGameModeCompleteRound;

	// Token: 0x14000015 RID: 21
	// (add) Token: 0x06000819 RID: 2073 RVA: 0x0002D2D4 File Offset: 0x0002B4D4
	// (remove) Token: 0x0600081A RID: 2074 RVA: 0x0002D308 File Offset: 0x0002B508
	public static event Action<string> OnGrabbedObject;

	// Token: 0x14000016 RID: 22
	// (add) Token: 0x0600081B RID: 2075 RVA: 0x0002D33C File Offset: 0x0002B53C
	// (remove) Token: 0x0600081C RID: 2076 RVA: 0x0002D370 File Offset: 0x0002B570
	public static event Action<string> OnDroppedObject;

	// Token: 0x14000017 RID: 23
	// (add) Token: 0x0600081D RID: 2077 RVA: 0x0002D3A4 File Offset: 0x0002B5A4
	// (remove) Token: 0x0600081E RID: 2078 RVA: 0x0002D3D8 File Offset: 0x0002B5D8
	public static event Action<string> OnEatObject;

	// Token: 0x14000018 RID: 24
	// (add) Token: 0x0600081F RID: 2079 RVA: 0x0002D40C File Offset: 0x0002B60C
	// (remove) Token: 0x06000820 RID: 2080 RVA: 0x0002D440 File Offset: 0x0002B640
	public static event Action<string> OnTapObject;

	// Token: 0x14000019 RID: 25
	// (add) Token: 0x06000821 RID: 2081 RVA: 0x0002D474 File Offset: 0x0002B674
	// (remove) Token: 0x06000822 RID: 2082 RVA: 0x0002D4A8 File Offset: 0x0002B6A8
	public static event Action<string> OnLaunchedProjectile;

	// Token: 0x1400001A RID: 26
	// (add) Token: 0x06000823 RID: 2083 RVA: 0x0002D4DC File Offset: 0x0002B6DC
	// (remove) Token: 0x06000824 RID: 2084 RVA: 0x0002D510 File Offset: 0x0002B710
	public static event Action<float, float> OnPlayerMoved;

	// Token: 0x1400001B RID: 27
	// (add) Token: 0x06000825 RID: 2085 RVA: 0x0002D544 File Offset: 0x0002B744
	// (remove) Token: 0x06000826 RID: 2086 RVA: 0x0002D578 File Offset: 0x0002B778
	public static event Action<float, float> OnPlayerSwam;

	// Token: 0x1400001C RID: 28
	// (add) Token: 0x06000827 RID: 2087 RVA: 0x0002D5AC File Offset: 0x0002B7AC
	// (remove) Token: 0x06000828 RID: 2088 RVA: 0x0002D5E0 File Offset: 0x0002B7E0
	public static event Action<string> OnTriggerHandEffect;

	// Token: 0x1400001D RID: 29
	// (add) Token: 0x06000829 RID: 2089 RVA: 0x0002D614 File Offset: 0x0002B814
	// (remove) Token: 0x0600082A RID: 2090 RVA: 0x0002D648 File Offset: 0x0002B848
	public static event Action<string> OnEnterLocation;

	// Token: 0x1400001E RID: 30
	// (add) Token: 0x0600082B RID: 2091 RVA: 0x0002D67C File Offset: 0x0002B87C
	// (remove) Token: 0x0600082C RID: 2092 RVA: 0x0002D6B0 File Offset: 0x0002B8B0
	public static event Action<string, int> OnMiscEvent;

	// Token: 0x1400001F RID: 31
	// (add) Token: 0x0600082D RID: 2093 RVA: 0x0002D6E4 File Offset: 0x0002B8E4
	// (remove) Token: 0x0600082E RID: 2094 RVA: 0x0002D718 File Offset: 0x0002B918
	public static event Action<string> OnCritterEvent;

	// Token: 0x0600082F RID: 2095 RVA: 0x0002D74C File Offset: 0x0002B94C
	public static void GameModeObjectiveTriggered()
	{
		string obj = GorillaGameManager.instance.GameModeName();
		Action<string> onGameModeObjectiveTrigger = PlayerGameEvents.OnGameModeObjectiveTrigger;
		if (onGameModeObjectiveTrigger == null)
		{
			return;
		}
		onGameModeObjectiveTrigger(obj);
	}

	// Token: 0x06000830 RID: 2096 RVA: 0x0002D774 File Offset: 0x0002B974
	public static void GameModeCompleteRound()
	{
		string obj = GorillaGameManager.instance.GameModeName();
		Action<string> onGameModeCompleteRound = PlayerGameEvents.OnGameModeCompleteRound;
		if (onGameModeCompleteRound == null)
		{
			return;
		}
		onGameModeCompleteRound(obj);
	}

	// Token: 0x06000831 RID: 2097 RVA: 0x0002D79C File Offset: 0x0002B99C
	public static void GrabbedObject(string objectName)
	{
		Action<string> onGrabbedObject = PlayerGameEvents.OnGrabbedObject;
		if (onGrabbedObject == null)
		{
			return;
		}
		onGrabbedObject(objectName);
	}

	// Token: 0x06000832 RID: 2098 RVA: 0x0002D7AE File Offset: 0x0002B9AE
	public static void DroppedObject(string objectName)
	{
		Action<string> onDroppedObject = PlayerGameEvents.OnDroppedObject;
		if (onDroppedObject == null)
		{
			return;
		}
		onDroppedObject(objectName);
	}

	// Token: 0x06000833 RID: 2099 RVA: 0x0002D7C0 File Offset: 0x0002B9C0
	public static void EatObject(string objectName)
	{
		Action<string> onEatObject = PlayerGameEvents.OnEatObject;
		if (onEatObject == null)
		{
			return;
		}
		onEatObject(objectName);
	}

	// Token: 0x06000834 RID: 2100 RVA: 0x0002D7D2 File Offset: 0x0002B9D2
	public static void TapObject(string objectName)
	{
		Action<string> onTapObject = PlayerGameEvents.OnTapObject;
		if (onTapObject == null)
		{
			return;
		}
		onTapObject(objectName);
	}

	// Token: 0x06000835 RID: 2101 RVA: 0x0002D7E4 File Offset: 0x0002B9E4
	public static void LaunchedProjectile(string objectName)
	{
		Action<string> onLaunchedProjectile = PlayerGameEvents.OnLaunchedProjectile;
		if (onLaunchedProjectile == null)
		{
			return;
		}
		onLaunchedProjectile(objectName);
	}

	// Token: 0x06000836 RID: 2102 RVA: 0x0002D7F6 File Offset: 0x0002B9F6
	public static void PlayerMoved(float distance, float speed)
	{
		Action<float, float> onPlayerMoved = PlayerGameEvents.OnPlayerMoved;
		if (onPlayerMoved == null)
		{
			return;
		}
		onPlayerMoved(distance, speed);
	}

	// Token: 0x06000837 RID: 2103 RVA: 0x0002D809 File Offset: 0x0002BA09
	public static void PlayerSwam(float distance, float speed)
	{
		Action<float, float> onPlayerSwam = PlayerGameEvents.OnPlayerSwam;
		if (onPlayerSwam == null)
		{
			return;
		}
		onPlayerSwam(distance, speed);
	}

	// Token: 0x06000838 RID: 2104 RVA: 0x0002D81C File Offset: 0x0002BA1C
	public static void TriggerHandEffect(string effectName)
	{
		Action<string> onTriggerHandEffect = PlayerGameEvents.OnTriggerHandEffect;
		if (onTriggerHandEffect == null)
		{
			return;
		}
		onTriggerHandEffect(effectName);
	}

	// Token: 0x06000839 RID: 2105 RVA: 0x0002D82E File Offset: 0x0002BA2E
	public static void TriggerEnterLocation(string locationName)
	{
		Action<string> onEnterLocation = PlayerGameEvents.OnEnterLocation;
		if (onEnterLocation == null)
		{
			return;
		}
		onEnterLocation(locationName);
	}

	// Token: 0x0600083A RID: 2106 RVA: 0x0002D840 File Offset: 0x0002BA40
	public static void MiscEvent(string eventName, int count = 1)
	{
		Action<string, int> onMiscEvent = PlayerGameEvents.OnMiscEvent;
		if (onMiscEvent == null)
		{
			return;
		}
		onMiscEvent(eventName, count);
	}

	// Token: 0x0600083B RID: 2107 RVA: 0x0002D853 File Offset: 0x0002BA53
	public static void CritterEvent(string eventName)
	{
		Action<string> onCritterEvent = PlayerGameEvents.OnCritterEvent;
		if (onCritterEvent == null)
		{
			return;
		}
		onCritterEvent(eventName);
	}

	// Token: 0x02000138 RID: 312
	public enum EventType
	{
		// Token: 0x040009C8 RID: 2504
		NONE,
		// Token: 0x040009C9 RID: 2505
		GameModeObjective,
		// Token: 0x040009CA RID: 2506
		GameModeCompleteRound,
		// Token: 0x040009CB RID: 2507
		GrabbedObject,
		// Token: 0x040009CC RID: 2508
		DroppedObject,
		// Token: 0x040009CD RID: 2509
		EatObject,
		// Token: 0x040009CE RID: 2510
		TapObject,
		// Token: 0x040009CF RID: 2511
		LaunchedProjectile,
		// Token: 0x040009D0 RID: 2512
		PlayerMoved,
		// Token: 0x040009D1 RID: 2513
		PlayerSwam,
		// Token: 0x040009D2 RID: 2514
		TriggerHandEfffect,
		// Token: 0x040009D3 RID: 2515
		EnterLocation,
		// Token: 0x040009D4 RID: 2516
		MiscEvent
	}
}
