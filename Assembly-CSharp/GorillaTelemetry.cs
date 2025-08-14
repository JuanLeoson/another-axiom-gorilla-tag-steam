using System;
using System.Buffers;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using GorillaNetworking;
using KID.Model;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.EventsModels;
using UnityEngine;

// Token: 0x0200072A RID: 1834
public static class GorillaTelemetry
{
	// Token: 0x06002DF5 RID: 11765 RVA: 0x000F33A8 File Offset: 0x000F15A8
	static GorillaTelemetry()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["User"] = null;
		dictionary["EventType"] = null;
		dictionary["ZoneId"] = null;
		dictionary["SubZoneId"] = null;
		GorillaTelemetry.gZoneEventArgs = dictionary;
		Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
		dictionary2["User"] = null;
		dictionary2["EventType"] = null;
		GorillaTelemetry.gNotifEventArgs = dictionary2;
		GorillaTelemetry.CurrentZone = GTZone.none;
		GorillaTelemetry.CurrentSubZone = GTSubZone.none;
		Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
		dictionary3["User"] = null;
		dictionary3["EventType"] = null;
		dictionary3["Items"] = null;
		GorillaTelemetry.gShopEventArgs = dictionary3;
		GorillaTelemetry.gSingleItemParam = new CosmeticsController.CosmeticItem[1];
		GorillaTelemetry.gSingleItemBuilderParam = new BuilderSetManager.BuilderSetStoreItem[1];
		Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
		dictionary4["User"] = null;
		dictionary4["EventType"] = null;
		dictionary4["AgeCategory"] = null;
		dictionary4["VoiceChatEnabled"] = null;
		dictionary4["CustomUsernameEnabled"] = null;
		dictionary4["JoinGroups"] = null;
		GorillaTelemetry.gKidEventArgs = dictionary4;
		Dictionary<string, object> dictionary5 = new Dictionary<string, object>();
		dictionary5["User"] = null;
		dictionary5["WamGameId"] = null;
		dictionary5["WamMachineId"] = null;
		GorillaTelemetry.gWamGameStartArgs = dictionary5;
		Dictionary<string, object> dictionary6 = new Dictionary<string, object>();
		dictionary6["User"] = null;
		dictionary6["WamGameId"] = null;
		dictionary6["WamMachineId"] = null;
		dictionary6["WamMLevelNumber"] = null;
		dictionary6["WamGoodMolesShown"] = null;
		dictionary6["WamHazardMolesShown"] = null;
		dictionary6["WamLevelMinScore"] = null;
		dictionary6["WamLevelScore"] = null;
		dictionary6["WamHazardMolesHit"] = null;
		dictionary6["WamGameState"] = null;
		GorillaTelemetry.gWamLevelEndArgs = dictionary6;
		Dictionary<string, object> dictionary7 = new Dictionary<string, object>();
		dictionary7["CustomMapName"] = null;
		dictionary7["CustomMapModId"] = null;
		dictionary7["LowestFPS"] = null;
		dictionary7["LowestFPSDrawCalls"] = null;
		dictionary7["LowestFPSPlayerCount"] = null;
		dictionary7["AverageFPS"] = null;
		dictionary7["AverageDrawCalls"] = null;
		dictionary7["AveragePlayerCount"] = null;
		dictionary7["HighestFPS"] = null;
		dictionary7["HighestFPSDrawCalls"] = null;
		dictionary7["HighestFPSPlayerCount"] = null;
		dictionary7["PlaytimeInSeconds"] = null;
		GorillaTelemetry.gCustomMapPerfArgs = dictionary7;
		Dictionary<string, object> dictionary8 = new Dictionary<string, object>();
		dictionary8["User"] = null;
		dictionary8["CustomMapName"] = null;
		dictionary8["CustomMapModId"] = null;
		dictionary8["CustomMapCreator"] = null;
		dictionary8["MinPlayerCount"] = null;
		dictionary8["MaxPlayerCount"] = null;
		dictionary8["PlaytimeOnMap"] = null;
		dictionary8["PrivateRoom"] = null;
		GorillaTelemetry.gCustomMapTrackingMetrics = dictionary8;
		Dictionary<string, object> dictionary9 = new Dictionary<string, object>();
		dictionary9["User"] = null;
		dictionary9["CustomMapName"] = null;
		dictionary9["CustomMapModId"] = null;
		dictionary9["CustomMapCreator"] = null;
		GorillaTelemetry.gCustomMapDownloadMetrics = dictionary9;
		Dictionary<string, object> dictionary10 = new Dictionary<string, object>();
		dictionary10["User"] = null;
		dictionary10["ghost_game_id"] = null;
		dictionary10["event_timestamp"] = null;
		dictionary10["initial_cores_balance"] = null;
		dictionary10["number_of_players"] = null;
		dictionary10["start_at_beginning"] = null;
		dictionary10["seconds_into_shift_at_join"] = null;
		GorillaTelemetry.gGhostReactorShiftStartArgs = dictionary10;
		Dictionary<string, object> dictionary11 = new Dictionary<string, object>();
		dictionary11["User"] = null;
		dictionary11["ghost_game_id"] = null;
		dictionary11["event_timestamp"] = null;
		dictionary11["final_cores_balance"] = null;
		dictionary11["cores_spent_waiting_in_breakroom"] = null;
		dictionary11["cores_collected_from_ghosts"] = null;
		dictionary11["cores_collected_from_gathering"] = null;
		dictionary11["cores_spent_on_items"] = null;
		dictionary11["cores_spent_on_gates"] = null;
		dictionary11["cores_spent_on_levels"] = null;
		dictionary11["cores_given_to_others"] = null;
		dictionary11["cores_received_from_others"] = null;
		dictionary11["gates_unlocked"] = null;
		dictionary11["died"] = null;
		dictionary11["caught_in_anamole"] = null;
		dictionary11["items_purchased"] = null;
		dictionary11["levels_unlocked"] = null;
		dictionary11["shift_cut"] = null;
		dictionary11["play_duration"] = null;
		dictionary11["started_late"] = null;
		dictionary11["time_started"] = null;
		dictionary11["reason"] = null;
		dictionary11["cores_collected"] = null;
		dictionary11["max_number_in_game"] = null;
		dictionary11["end_number_in_game"] = null;
		dictionary11["items_picked_up"] = null;
		GorillaTelemetry.gGhostReactorShiftEndArgs = dictionary11;
		GameObject gameObject = new GameObject("GorillaTelemetryBatcher");
		Object.DontDestroyOnLoad(gameObject);
		gameObject.AddComponent<GorillaTelemetry.BatchRunner>();
	}

	// Token: 0x06002DF6 RID: 11766 RVA: 0x000F38AD File Offset: 0x000F1AAD
	private static void QueueTelemetryEventMothership(string eventName, object content)
	{
		GorillaTelemetry.telemetryEventsQueueMothership.Enqueue(new MothershipAnalyticsEvent
		{
			event_name = eventName,
			body = JsonConvert.SerializeObject(content)
		});
	}

	// Token: 0x06002DF7 RID: 11767 RVA: 0x000F38D1 File Offset: 0x000F1AD1
	private static void QueueTelemetryEventPlayFab(EventContents eventContent)
	{
		GorillaTelemetry.telemetryEventsQueuePlayFab.Enqueue(eventContent);
	}

	// Token: 0x06002DF8 RID: 11768 RVA: 0x000F38E0 File Offset: 0x000F1AE0
	private static void FlushPlayFabTelemetry()
	{
		int count = GorillaTelemetry.telemetryEventsQueuePlayFab.Count;
		if (count == 0)
		{
			return;
		}
		EventContents[] array = ArrayPool<EventContents>.Shared.Rent(count);
		try
		{
			int i;
			for (i = 0; i < count; i++)
			{
				EventContents eventContents;
				array[i] = (GorillaTelemetry.telemetryEventsQueuePlayFab.TryDequeue(out eventContents) ? eventContents : null);
			}
			if (i == 0)
			{
				ArrayPool<EventContents>.Shared.Return(array, false);
			}
			else
			{
				WriteEventsRequest writeEventsRequest = new WriteEventsRequest();
				writeEventsRequest.Events = GorillaTelemetry.GetEventListForArrayPlayFab(array, i);
				PlayFabEventsAPI.WriteTelemetryEvents(writeEventsRequest, delegate(WriteEventsResponse result)
				{
				}, delegate(PlayFabError error)
				{
				}, null, null);
			}
		}
		finally
		{
			ArrayPool<EventContents>.Shared.Return(array, false);
		}
	}

	// Token: 0x06002DF9 RID: 11769 RVA: 0x000F39B0 File Offset: 0x000F1BB0
	private static void FlushMothershipTelemetry()
	{
		int count = GorillaTelemetry.telemetryEventsQueueMothership.Count;
		if (count == 0)
		{
			return;
		}
		MothershipAnalyticsEvent[] array = ArrayPool<MothershipAnalyticsEvent>.Shared.Rent(count);
		try
		{
			int dequeuedCount2;
			int dequeuedCount;
			for (dequeuedCount = 0; dequeuedCount < count; dequeuedCount = dequeuedCount2)
			{
				MothershipAnalyticsEvent mothershipAnalyticsEvent;
				array[dequeuedCount] = (GorillaTelemetry.telemetryEventsQueueMothership.TryDequeue(out mothershipAnalyticsEvent) ? mothershipAnalyticsEvent : null);
				dequeuedCount2 = dequeuedCount + 1;
			}
			if (dequeuedCount == 0)
			{
				ArrayPool<MothershipAnalyticsEvent>.Shared.Return(array, false);
			}
			else
			{
				MothershipWriteEventsRequest req = new MothershipWriteEventsRequest
				{
					title_id = MothershipClientApiUnity.TitleId,
					deployment_id = MothershipClientApiUnity.DeploymentId,
					env_id = MothershipClientApiUnity.EnvironmentId,
					events = new AnalyticsRequestVector(GorillaTelemetry.GetEventListForArrayMothership(array, dequeuedCount))
				};
				MothershipClientApiUnity.WriteEvents(MothershipClientContext.MothershipId, req, delegate(MothershipWriteEventsResponse resp)
				{
					Debug.Log(string.Format("[Telemetry::MOTHERSHIP_ANALYTICS] Successfully submitted analytics batch: [{0} events]", dequeuedCount));
				}, delegate(MothershipError err, int i)
				{
					Debug.Log("[Telemetry::MOTHERSHIP_ANALYTICS] Failed to submit analytics batch with error:\n" + err.Message);
				});
			}
		}
		finally
		{
			ArrayPool<MothershipAnalyticsEvent>.Shared.Return(array, false);
		}
	}

	// Token: 0x06002DFA RID: 11770 RVA: 0x000F3ACC File Offset: 0x000F1CCC
	private static List<EventContents> GetEventListForArrayPlayFab(EventContents[] array, int count)
	{
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			if (array[i] != null)
			{
				num++;
			}
		}
		List<EventContents> list;
		if (!GorillaTelemetry.gListPoolPlayFab.TryGetValue(num, out list))
		{
			list = new List<EventContents>(num);
			GorillaTelemetry.gListPoolPlayFab.TryAdd(num, list);
		}
		else
		{
			list.Clear();
		}
		for (int j = 0; j < count; j++)
		{
			if (array[j] != null)
			{
				list.Add(array[j]);
			}
		}
		return list;
	}

	// Token: 0x06002DFB RID: 11771 RVA: 0x000F3B38 File Offset: 0x000F1D38
	private static List<MothershipAnalyticsEvent> GetEventListForArrayMothership(MothershipAnalyticsEvent[] array, int count)
	{
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			if (array[i] != null)
			{
				num++;
			}
		}
		List<MothershipAnalyticsEvent> list;
		if (!GorillaTelemetry.gListPoolMothership.TryGetValue(num, out list))
		{
			list = new List<MothershipAnalyticsEvent>(num);
			GorillaTelemetry.gListPoolMothership.TryAdd(num, list);
		}
		else
		{
			list.Clear();
		}
		for (int j = 0; j < count; j++)
		{
			if (array[j] != null)
			{
				list.Add(array[j]);
			}
		}
		return list;
	}

	// Token: 0x06002DFC RID: 11772 RVA: 0x000F3BA2 File Offset: 0x000F1DA2
	private static bool IsConnected()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return false;
		}
		if (GorillaTelemetry.gPlayFabAuth == null)
		{
			GorillaTelemetry.gPlayFabAuth = PlayFabAuthenticator.instance;
		}
		return !(GorillaTelemetry.gPlayFabAuth == null);
	}

	// Token: 0x06002DFD RID: 11773 RVA: 0x000F3BD5 File Offset: 0x000F1DD5
	private static string PlayFabUserId()
	{
		return GorillaTelemetry.gPlayFabAuth.GetPlayFabPlayerId();
	}

	// Token: 0x06002DFE RID: 11774 RVA: 0x000F3BE4 File Offset: 0x000F1DE4
	public static void PostZoneEvent(GTZone zone, GTSubZone subZone, GTZoneEventType zoneEvent)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		string value = GorillaTelemetry.PlayFabUserId();
		string name = zoneEvent.GetName<GTZoneEventType>();
		string name2 = zone.GetName<GTZone>();
		string name3 = subZone.GetName<GTSubZone>();
		bool sessionIsPrivate = NetworkSystem.Instance.SessionIsPrivate;
		Dictionary<string, object> dictionary = GorillaTelemetry.gZoneEventArgs;
		dictionary["User"] = value;
		dictionary["EventType"] = name;
		dictionary["ZoneId"] = name2;
		dictionary["SubZoneId"] = name3;
		dictionary["IsPrivateRoom"] = sessionIsPrivate;
		GorillaTelemetry.QueueTelemetryEventPlayFab(new EventContents
		{
			Name = "telemetry_zone_event",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = dictionary
		});
		GorillaTelemetry.QueueTelemetryEventMothership("telemetry_zone_event", dictionary);
	}

	// Token: 0x06002DFF RID: 11775 RVA: 0x000F3CA2 File Offset: 0x000F1EA2
	public static void PostShopEvent(VRRig playerRig, GTShopEventType shopEvent, CosmeticsController.CosmeticItem item)
	{
		GorillaTelemetry.gSingleItemParam[0] = item;
		GorillaTelemetry.PostShopEvent(playerRig, shopEvent, GorillaTelemetry.gSingleItemParam);
		GorillaTelemetry.gSingleItemParam[0] = default(CosmeticsController.CosmeticItem);
	}

	// Token: 0x06002E00 RID: 11776 RVA: 0x000F3CD0 File Offset: 0x000F1ED0
	private static string[] FetchItemArgs(IList<CosmeticsController.CosmeticItem> items)
	{
		int count = items.Count;
		if (count == 0)
		{
			return Array.Empty<string>();
		}
		HashSet<string> hashSet = new HashSet<string>(count);
		int num = 0;
		for (int i = 0; i < items.Count; i++)
		{
			CosmeticsController.CosmeticItem cosmeticItem = items[i];
			if (!cosmeticItem.isNullItem)
			{
				string itemName = cosmeticItem.itemName;
				if (!string.IsNullOrWhiteSpace(itemName) && !itemName.Contains("NOTHING", StringComparison.InvariantCultureIgnoreCase) && hashSet.Add(itemName))
				{
					num++;
				}
			}
		}
		string[] array = new string[num];
		hashSet.CopyTo(array);
		return array;
	}

	// Token: 0x06002E01 RID: 11777 RVA: 0x000F3D5C File Offset: 0x000F1F5C
	public static void PostShopEvent(VRRig playerRig, GTShopEventType shopEvent, IList<CosmeticsController.CosmeticItem> items)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		if (!playerRig.isLocal)
		{
			return;
		}
		string value = GorillaTelemetry.PlayFabUserId();
		string name = shopEvent.GetName<GTShopEventType>();
		string[] value2 = GorillaTelemetry.FetchItemArgs(items);
		Dictionary<string, object> dictionary = GorillaTelemetry.gShopEventArgs;
		dictionary["User"] = value;
		dictionary["EventType"] = name;
		dictionary["Items"] = value2;
		PlayFabClientAPI.WriteTitleEvent(new WriteTitleEventRequest
		{
			EventName = "telemetry_shop_event",
			Body = dictionary
		}, new Action<WriteEventResponse>(GorillaTelemetry.PostShopEvent_OnResult), new Action<PlayFabError>(GorillaTelemetry.PostShopEvent_OnError), null, null);
	}

	// Token: 0x06002E02 RID: 11778 RVA: 0x000023F5 File Offset: 0x000005F5
	private static void PostShopEvent_OnResult(WriteEventResponse result)
	{
	}

	// Token: 0x06002E03 RID: 11779 RVA: 0x000023F5 File Offset: 0x000005F5
	private static void PostShopEvent_OnError(PlayFabError error)
	{
	}

	// Token: 0x06002E04 RID: 11780 RVA: 0x000F3DEE File Offset: 0x000F1FEE
	public static void PostBuilderKioskEvent(VRRig playerRig, GTShopEventType shopEvent, BuilderSetManager.BuilderSetStoreItem item)
	{
		GorillaTelemetry.gSingleItemBuilderParam[0] = item;
		GorillaTelemetry.PostBuilderKioskEvent(playerRig, shopEvent, GorillaTelemetry.gSingleItemBuilderParam);
		GorillaTelemetry.gSingleItemBuilderParam[0] = default(BuilderSetManager.BuilderSetStoreItem);
	}

	// Token: 0x06002E05 RID: 11781 RVA: 0x000F3E1C File Offset: 0x000F201C
	private static string[] BuilderItemsToStrings(IList<BuilderSetManager.BuilderSetStoreItem> items)
	{
		int count = items.Count;
		if (count == 0)
		{
			return Array.Empty<string>();
		}
		HashSet<string> hashSet = new HashSet<string>(count);
		int num = 0;
		for (int i = 0; i < items.Count; i++)
		{
			BuilderSetManager.BuilderSetStoreItem builderSetStoreItem = items[i];
			if (!builderSetStoreItem.isNullItem)
			{
				string playfabID = builderSetStoreItem.playfabID;
				if (!string.IsNullOrWhiteSpace(playfabID) && !playfabID.Contains("NOTHING", StringComparison.InvariantCultureIgnoreCase) && hashSet.Add(playfabID))
				{
					num++;
				}
			}
		}
		string[] array = new string[num];
		hashSet.CopyTo(array);
		return array;
	}

	// Token: 0x06002E06 RID: 11782 RVA: 0x000F3EA8 File Offset: 0x000F20A8
	public static void PostBuilderKioskEvent(VRRig playerRig, GTShopEventType shopEvent, IList<BuilderSetManager.BuilderSetStoreItem> items)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		if (!playerRig.isLocal)
		{
			return;
		}
		string value = GorillaTelemetry.PlayFabUserId();
		string name = shopEvent.GetName<GTShopEventType>();
		string[] value2 = GorillaTelemetry.BuilderItemsToStrings(items);
		Dictionary<string, object> dictionary = GorillaTelemetry.gShopEventArgs;
		dictionary["User"] = value;
		dictionary["EventType"] = name;
		dictionary["Items"] = value2;
		PlayFabClientAPI.WriteTitleEvent(new WriteTitleEventRequest
		{
			EventName = "telemetry_shop_event",
			Body = dictionary
		}, new Action<WriteEventResponse>(GorillaTelemetry.PostShopEvent_OnResult), new Action<PlayFabError>(GorillaTelemetry.PostShopEvent_OnError), null, null);
	}

	// Token: 0x06002E07 RID: 11783 RVA: 0x000F3F3C File Offset: 0x000F213C
	public static void PostKidEvent(bool joinGroupsEnabled, bool voiceChatEnabled, bool customUsernamesEnabled, AgeStatusType ageCategory, GTKidEventType kidEvent)
	{
		if ((double)Random.value < 0.1)
		{
			return;
		}
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		string value = GorillaTelemetry.PlayFabUserId();
		string name = kidEvent.GetName<GTKidEventType>();
		string value2 = (ageCategory == AgeStatusType.LEGALADULT) ? "Not_Managed_Account" : "Managed_Account";
		string value3 = joinGroupsEnabled.ToString().ToUpper();
		string value4 = voiceChatEnabled.ToString().ToUpper();
		string value5 = customUsernamesEnabled.ToString().ToUpper();
		Dictionary<string, object> dictionary = GorillaTelemetry.gKidEventArgs;
		dictionary["User"] = value;
		dictionary["EventType"] = name;
		dictionary["AgeCategory"] = value2;
		dictionary["VoiceChatEnabled"] = value4;
		dictionary["CustomUsernameEnabled"] = value5;
		dictionary["JoinGroups"] = value3;
		GorillaTelemetry.QueueTelemetryEventPlayFab(new EventContents
		{
			Name = "telemetry_kid_event",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = dictionary
		});
	}

	// Token: 0x06002E08 RID: 11784 RVA: 0x000F402C File Offset: 0x000F222C
	public static void WamGameStart(string playerId, string gameId, string machineId)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GorillaTelemetry.gWamGameStartArgs["User"] = playerId;
		GorillaTelemetry.gWamGameStartArgs["WamGameId"] = gameId;
		GorillaTelemetry.gWamGameStartArgs["WamMachineId"] = machineId;
		GorillaTelemetry.QueueTelemetryEventPlayFab(new EventContents
		{
			Name = "telemetry_wam_gameStartEvent",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = GorillaTelemetry.gWamGameStartArgs
		});
	}

	// Token: 0x06002E09 RID: 11785 RVA: 0x000F409C File Offset: 0x000F229C
	public static void WamLevelEnd(string playerId, int gameId, string machineId, int currentLevelNumber, int levelGoodMolesShown, int levelHazardMolesShown, int levelMinScore, int currentScore, int levelHazardMolesHit, string currentGameResult)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GorillaTelemetry.gWamLevelEndArgs["User"] = playerId;
		GorillaTelemetry.gWamLevelEndArgs["WamGameId"] = gameId.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamMachineId"] = machineId;
		GorillaTelemetry.gWamLevelEndArgs["WamMLevelNumber"] = currentLevelNumber.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamGoodMolesShown"] = levelGoodMolesShown.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamHazardMolesShown"] = levelHazardMolesShown.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamLevelMinScore"] = levelMinScore.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamLevelScore"] = currentScore.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamHazardMolesHit"] = levelHazardMolesHit.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamGameState"] = currentGameResult;
		GorillaTelemetry.QueueTelemetryEventPlayFab(new EventContents
		{
			Name = "telemetry_wam_levelEndEvent",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = GorillaTelemetry.gWamLevelEndArgs
		});
	}

	// Token: 0x06002E0A RID: 11786 RVA: 0x000F41A8 File Offset: 0x000F23A8
	public static void PostCustomMapPerformance(string mapName, long mapModId, int lowestFPS, int lowestDC, int lowestPC, int avgFPS, int avgDC, int avgPC, int highestFPS, int highestDC, int highestPC, int playtime)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		Dictionary<string, object> dictionary = GorillaTelemetry.gCustomMapPerfArgs;
		dictionary["CustomMapName"] = mapName;
		dictionary["CustomMapModId"] = mapModId.ToString();
		dictionary["LowestFPS"] = lowestFPS.ToString();
		dictionary["LowestFPSDrawCalls"] = lowestDC.ToString();
		dictionary["LowestFPSPlayerCount"] = lowestPC.ToString();
		dictionary["AverageFPS"] = avgFPS.ToString();
		dictionary["AverageDrawCalls"] = avgDC.ToString();
		dictionary["AveragePlayerCount"] = avgPC.ToString();
		dictionary["HighestFPS"] = highestFPS.ToString();
		dictionary["HighestFPSDrawCalls"] = highestDC.ToString();
		dictionary["HighestFPSPlayerCount"] = highestPC.ToString();
		dictionary["PlaytimeInSeconds"] = playtime.ToString();
		GorillaTelemetry.QueueTelemetryEventPlayFab(new EventContents
		{
			Name = "CustomMapPerformance",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = dictionary
		});
	}

	// Token: 0x06002E0B RID: 11787 RVA: 0x000F42BC File Offset: 0x000F24BC
	public static void PostCustomMapTracking(string mapName, long mapModId, string mapCreatorUsername, int minPlayers, int maxPlayers, int playtime, bool privateRoom)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		int num = playtime % 60;
		int num2 = (playtime - num) / 60;
		int num3 = num2 % 60;
		int num4 = (num2 - num3) / 60;
		string value = string.Format("{0}.{1}.{2}", num4, num3, num);
		Dictionary<string, object> dictionary = GorillaTelemetry.gCustomMapTrackingMetrics;
		dictionary["User"] = GorillaTelemetry.PlayFabUserId();
		dictionary["CustomMapName"] = mapName;
		dictionary["CustomMapModId"] = mapModId.ToString();
		dictionary["CustomMapCreator"] = mapCreatorUsername;
		dictionary["MinPlayerCount"] = minPlayers.ToString();
		dictionary["MaxPlayerCount"] = maxPlayers.ToString();
		dictionary["PlaytimeInSeconds"] = playtime.ToString();
		dictionary["PrivateRoom"] = privateRoom.ToString();
		dictionary["PlaytimeOnMap"] = value;
		GorillaTelemetry.QueueTelemetryEventPlayFab(new EventContents
		{
			Name = "CustomMapTracking",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = dictionary
		});
	}

	// Token: 0x06002E0C RID: 11788 RVA: 0x000023F5 File Offset: 0x000005F5
	public static void PostCustomMapDownloadEvent(string mapName, long mapModId, string mapCreatorUsername)
	{
	}

	// Token: 0x06002E0D RID: 11789 RVA: 0x000F43CC File Offset: 0x000F25CC
	public static void GhostReactorShiftStart(string gameId, int initialCores, float timeIntoShift, bool wasPlayerInAtStart, int numPlayers)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GorillaTelemetry.gGhostReactorShiftStartArgs["User"] = GorillaTelemetry.PlayFabUserId();
		GorillaTelemetry.gGhostReactorShiftStartArgs["ghost_game_id"] = gameId;
		GorillaTelemetry.gGhostReactorShiftStartArgs["event_timestamp"] = DateTime.Now.ToString();
		GorillaTelemetry.gGhostReactorShiftStartArgs["initial_cores_balance"] = initialCores.ToString();
		GorillaTelemetry.gGhostReactorShiftStartArgs["number_of_players"] = numPlayers.ToString();
		GorillaTelemetry.gGhostReactorShiftStartArgs["start_at_beginning"] = wasPlayerInAtStart.ToString();
		GorillaTelemetry.gGhostReactorShiftStartArgs["seconds_into_shift_at_join"] = timeIntoShift.ToString();
		GorillaTelemetry.QueueTelemetryEventPlayFab(new EventContents
		{
			Name = "ghost_game_start",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = GorillaTelemetry.gGhostReactorShiftStartArgs
		});
		GorillaTelemetry.SendMothershipAnalytics(new GhostReactorTelemetryData
		{
			EventName = "ghost_game_start",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string>
			{
				{
					"ghost_game_id",
					gameId
				},
				{
					"event_timestamp",
					DateTime.Now.ToString()
				},
				{
					"initial_cores_balance",
					initialCores.ToString()
				},
				{
					"number_of_players",
					numPlayers.ToString()
				},
				{
					"start_at_beginning",
					wasPlayerInAtStart.ToString()
				},
				{
					"seconds_into_shift_at_join",
					timeIntoShift.ToString()
				}
			}
		});
	}

	// Token: 0x06002E0E RID: 11790 RVA: 0x000F4554 File Offset: 0x000F2754
	public static void GhostReactorShiftEnd(string gameId, int finalCores, int coresWhileWaiting, int coresFromGhosts, int coresFromGathering, int coresOnItems, int coresOnGates, int coresOnLevels, int coresGiven, int coresReceived, int gatesUnlocked, int deaths, bool caughtByAnomaly, List<string> itemsPurchased, List<string> levelsUnlocked, int shiftCut, bool isShiftActuallyEnding, float timeIntoShiftAtJoin, float playDuration, bool wasPlayerInAtStart, ZoneClearReason zoneClearReason, float coresCollected, int maxNumberOfPlayersInShift, int endNumberOfPlayers, Dictionary<string, int> itemTypesHeldThisShift)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GorillaTelemetry.gGhostReactorShiftEndArgs["User"] = GorillaTelemetry.PlayFabUserId();
		GorillaTelemetry.gGhostReactorShiftEndArgs["ghost_game_id"] = gameId;
		GorillaTelemetry.gGhostReactorShiftEndArgs["event_timestamp"] = DateTime.Now.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["final_cores_balance"] = finalCores.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["cores_spent_waiting_in_breakroom"] = coresWhileWaiting.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["cores_collected_from_ghosts"] = coresFromGhosts.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["cores_collected_from_gathering"] = coresFromGathering.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["cores_spent_on_items"] = coresOnItems.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["cores_spent_on_gates"] = coresOnGates.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["cores_spent_on_levels"] = coresOnLevels.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["cores_given_to_others"] = coresGiven.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["cores_received_from_others"] = coresReceived.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["gates_unlocked"] = gatesUnlocked.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["died"] = deaths.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["caught_in_anamole"] = caughtByAnomaly.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["items_purchased"] = itemsPurchased.ToJson(true);
		GorillaTelemetry.gGhostReactorShiftEndArgs["levels_unlocked"] = levelsUnlocked.ToJson(true);
		GorillaTelemetry.gGhostReactorShiftEndArgs["shift_cut"] = shiftCut.ToJson(true);
		GorillaTelemetry.gGhostReactorShiftEndArgs["play_duration"] = playDuration.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["started_late"] = (!wasPlayerInAtStart).ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["time_started"] = timeIntoShiftAtJoin.ToString();
		string value = "shift_ended";
		if (!isShiftActuallyEnding)
		{
			if (zoneClearReason == ZoneClearReason.LeaveZone)
			{
				value = "left_zone";
			}
			else
			{
				value = "disconnect";
			}
		}
		GorillaTelemetry.gGhostReactorShiftEndArgs["reason"] = value;
		GorillaTelemetry.gGhostReactorShiftEndArgs["cores_collected"] = coresCollected.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["max_number_in_game"] = maxNumberOfPlayersInShift.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["end_number_in_game"] = endNumberOfPlayers.ToString();
		GorillaTelemetry.gGhostReactorShiftEndArgs["items_picked_up"] = itemTypesHeldThisShift.ToJson(true);
		GorillaTelemetry.QueueTelemetryEventPlayFab(new EventContents
		{
			Name = "ghost_game_end",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = GorillaTelemetry.gGhostReactorShiftEndArgs
		});
		GorillaTelemetry.SendMothershipAnalytics(new GhostReactorTelemetryData
		{
			EventName = "ghost_game_end",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string>
			{
				{
					"ghost_game_id",
					gameId
				},
				{
					"event_timestamp",
					DateTime.Now.ToString()
				},
				{
					"final_cores_balance",
					finalCores.ToString()
				},
				{
					"cores_spent_waiting_in_breakroom",
					coresWhileWaiting.ToString()
				},
				{
					"cores_collected_from_ghosts",
					coresFromGhosts.ToString()
				},
				{
					"cores_collected_from_gathering",
					coresFromGathering.ToString()
				},
				{
					"cores_spent_on_items",
					coresOnItems.ToString()
				},
				{
					"cores_spent_on_gates",
					coresOnGates.ToString()
				},
				{
					"cores_spent_on_levels",
					coresOnLevels.ToString()
				},
				{
					"cores_given_to_others",
					coresGiven.ToString()
				},
				{
					"cores_received_from_others",
					coresReceived.ToString()
				},
				{
					"gates_unlocked",
					gatesUnlocked.ToString()
				},
				{
					"died",
					deaths.ToString()
				},
				{
					"caught_in_anamole",
					caughtByAnomaly.ToString()
				},
				{
					"items_purchased",
					itemsPurchased.ToJson(true)
				},
				{
					"levels_unlocked",
					levelsUnlocked.ToJson(true)
				},
				{
					"shift_cut_data",
					shiftCut.ToJson(true)
				},
				{
					"play_duration",
					playDuration.ToString()
				},
				{
					"started_late",
					(!wasPlayerInAtStart).ToString()
				},
				{
					"time_started",
					timeIntoShiftAtJoin.ToString()
				},
				{
					"reason",
					value
				},
				{
					"cores_collected",
					coresCollected.ToString()
				},
				{
					"max_number_in_game",
					maxNumberOfPlayersInShift.ToString()
				},
				{
					"end_number_in_game",
					endNumberOfPlayers.ToString()
				},
				{
					"items_picked_up",
					itemTypesHeldThisShift.ToJson(true)
				}
			}
		});
	}

	// Token: 0x06002E0F RID: 11791 RVA: 0x000F49F8 File Offset: 0x000F2BF8
	public static void SendMothershipAnalytics(KIDTelemetryData data)
	{
		if (string.IsNullOrEmpty(data.EventName))
		{
			Debug.LogError("[GORILLA_TELEMETRY::MOTHERSHIP_ANALYTICS] Event Name is null or empty");
			return;
		}
		if (data.BodyData == null || data.BodyData.Count == 0)
		{
			Debug.LogError("[GORILLA_TELEMETRY::MOTHERSHIP_ANALYTICS] Body Data KVPs are null or empty - must have at least 1");
			return;
		}
		string custom_tags = string.Empty;
		if (data.CustomTags != null && data.CustomTags.Length != 0)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			for (int j = 0; j < data.CustomTags.Length; j++)
			{
				dictionary.Add(string.Format("tag{0}", j + 1), data.CustomTags[j]);
			}
			custom_tags = JsonConvert.SerializeObject(dictionary);
		}
		string body = JsonConvert.SerializeObject(data.BodyData);
		MothershipWriteEventsRequest req = new MothershipWriteEventsRequest
		{
			title_id = MothershipClientApiUnity.TitleId,
			deployment_id = MothershipClientApiUnity.DeploymentId,
			env_id = MothershipClientApiUnity.EnvironmentId,
			events = new AnalyticsRequestVector(new List<MothershipAnalyticsEvent>
			{
				new MothershipAnalyticsEvent
				{
					event_timestamp = DateTime.UtcNow.ToString("O"),
					event_name = data.EventName,
					custom_tags = custom_tags,
					body = body
				}
			})
		};
		MothershipClientApiUnity.WriteEvents(MothershipClientContext.MothershipId, req, delegate(MothershipWriteEventsResponse resp)
		{
			Debug.Log("[GORILLA_TELEMETRY::MOTHERSHIP_ANALYTICS] Successfully submitted analytics for event: [" + data.EventName + "]");
		}, delegate(MothershipError err, int i)
		{
			Debug.Log("[GORILLA_TELEMETRY::MOTHERSHIP_ANALYTICS] Failed to submit analytics for event: [" + data.EventName + "], with error:\n" + err.Message);
		});
	}

	// Token: 0x06002E10 RID: 11792 RVA: 0x000F4B7C File Offset: 0x000F2D7C
	public static void SendMothershipAnalytics(GhostReactorTelemetryData data)
	{
		if (string.IsNullOrEmpty(data.EventName))
		{
			Debug.LogError("[GORILLA_TELEMETRY::MOTHERSHIP_ANALYTICS] Event Name is null or empty");
			return;
		}
		if (data.BodyData == null || data.BodyData.Count == 0)
		{
			Debug.LogError("[GORILLA_TELEMETRY::MOTHERSHIP_ANALYTICS] Body Data KVPs are null or empty - must have at least 1");
			return;
		}
		string custom_tags = string.Empty;
		if (data.CustomTags != null && data.CustomTags.Length != 0)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			for (int j = 0; j < data.CustomTags.Length; j++)
			{
				dictionary.Add(string.Format("tag{0}", j + 1), data.CustomTags[j]);
			}
			custom_tags = JsonConvert.SerializeObject(dictionary);
		}
		string body = JsonConvert.SerializeObject(data.BodyData);
		MothershipWriteEventsRequest req = new MothershipWriteEventsRequest
		{
			title_id = MothershipClientApiUnity.TitleId,
			deployment_id = MothershipClientApiUnity.DeploymentId,
			env_id = MothershipClientApiUnity.EnvironmentId,
			events = new AnalyticsRequestVector(new List<MothershipAnalyticsEvent>
			{
				new MothershipAnalyticsEvent
				{
					event_timestamp = DateTime.UtcNow.ToString("O"),
					event_name = data.EventName,
					custom_tags = custom_tags,
					body = body
				}
			})
		};
		MothershipClientApiUnity.WriteEvents(MothershipClientContext.MothershipId, req, delegate(MothershipWriteEventsResponse resp)
		{
			Debug.Log("[GORILLA_TELEMETRY::MOTHERSHIP_ANALYTICS] Successfully submitted analytics for event: [" + data.EventName + "]");
		}, delegate(MothershipError err, int i)
		{
			Debug.Log("[GORILLA_TELEMETRY::MOTHERSHIP_ANALYTICS] Failed to submit analytics for event: [" + data.EventName + "], with error:\n" + err.Message);
		});
	}

	// Token: 0x06002E11 RID: 11793 RVA: 0x000F4D00 File Offset: 0x000F2F00
	public static void PostNotificationEvent(string notificationType)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		string value = GorillaTelemetry.PlayFabUserId();
		Dictionary<string, object> dictionary = GorillaTelemetry.gNotifEventArgs;
		dictionary["User"] = value;
		dictionary["EventType"] = notificationType;
		GorillaTelemetry.QueueTelemetryEventPlayFab(new EventContents
		{
			Name = "telemetry_ggwp_event",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = dictionary
		});
	}

	// Token: 0x04003995 RID: 14741
	private static readonly float TELEMETRY_FLUSH_SEC = 10f;

	// Token: 0x04003996 RID: 14742
	private static readonly ConcurrentQueue<EventContents> telemetryEventsQueuePlayFab = new ConcurrentQueue<EventContents>();

	// Token: 0x04003997 RID: 14743
	private static readonly ConcurrentQueue<MothershipAnalyticsEvent> telemetryEventsQueueMothership = new ConcurrentQueue<MothershipAnalyticsEvent>();

	// Token: 0x04003998 RID: 14744
	private static readonly Dictionary<int, List<EventContents>> gListPoolPlayFab = new Dictionary<int, List<EventContents>>();

	// Token: 0x04003999 RID: 14745
	private static readonly Dictionary<int, List<MothershipAnalyticsEvent>> gListPoolMothership = new Dictionary<int, List<MothershipAnalyticsEvent>>();

	// Token: 0x0400399A RID: 14746
	private static readonly string namespacePrefix = "custom";

	// Token: 0x0400399B RID: 14747
	private static readonly string EVENT_NAMESPACE = GorillaTelemetry.namespacePrefix + "." + PlayFabAuthenticatorSettings.TitleId;

	// Token: 0x0400399C RID: 14748
	private static PlayFabAuthenticator gPlayFabAuth;

	// Token: 0x0400399D RID: 14749
	private static readonly Dictionary<string, object> gZoneEventArgs;

	// Token: 0x0400399E RID: 14750
	private static readonly Dictionary<string, object> gNotifEventArgs;

	// Token: 0x0400399F RID: 14751
	public static GTZone CurrentZone;

	// Token: 0x040039A0 RID: 14752
	public static GTSubZone CurrentSubZone;

	// Token: 0x040039A1 RID: 14753
	private static readonly Dictionary<string, object> gShopEventArgs;

	// Token: 0x040039A2 RID: 14754
	private static CosmeticsController.CosmeticItem[] gSingleItemParam;

	// Token: 0x040039A3 RID: 14755
	private static BuilderSetManager.BuilderSetStoreItem[] gSingleItemBuilderParam;

	// Token: 0x040039A4 RID: 14756
	private static Dictionary<string, object> gKidEventArgs;

	// Token: 0x040039A5 RID: 14757
	private static readonly Dictionary<string, object> gWamGameStartArgs;

	// Token: 0x040039A6 RID: 14758
	private static readonly Dictionary<string, object> gWamLevelEndArgs;

	// Token: 0x040039A7 RID: 14759
	private static Dictionary<string, object> gCustomMapPerfArgs;

	// Token: 0x040039A8 RID: 14760
	private static Dictionary<string, object> gCustomMapTrackingMetrics;

	// Token: 0x040039A9 RID: 14761
	private static Dictionary<string, object> gCustomMapDownloadMetrics;

	// Token: 0x040039AA RID: 14762
	private static readonly Dictionary<string, object> gGhostReactorShiftStartArgs;

	// Token: 0x040039AB RID: 14763
	private static readonly Dictionary<string, object> gGhostReactorShiftEndArgs;

	// Token: 0x0200072B RID: 1835
	public static class k
	{
		// Token: 0x040039AC RID: 14764
		public const string User = "User";

		// Token: 0x040039AD RID: 14765
		public const string ZoneId = "ZoneId";

		// Token: 0x040039AE RID: 14766
		public const string SubZoneId = "SubZoneId";

		// Token: 0x040039AF RID: 14767
		public const string EventType = "EventType";

		// Token: 0x040039B0 RID: 14768
		public const string IsPrivateRoom = "IsPrivateRoom";

		// Token: 0x040039B1 RID: 14769
		public const string Items = "Items";

		// Token: 0x040039B2 RID: 14770
		public const string VoiceChatEnabled = "VoiceChatEnabled";

		// Token: 0x040039B3 RID: 14771
		public const string JoinGroups = "JoinGroups";

		// Token: 0x040039B4 RID: 14772
		public const string CustomUsernameEnabled = "CustomUsernameEnabled";

		// Token: 0x040039B5 RID: 14773
		public const string AgeCategory = "AgeCategory";

		// Token: 0x040039B6 RID: 14774
		public const string telemetry_zone_event = "telemetry_zone_event";

		// Token: 0x040039B7 RID: 14775
		public const string telemetry_shop_event = "telemetry_shop_event";

		// Token: 0x040039B8 RID: 14776
		public const string telemetry_kid_event = "telemetry_kid_event";

		// Token: 0x040039B9 RID: 14777
		public const string telemetry_ggwp_event = "telemetry_ggwp_event";

		// Token: 0x040039BA RID: 14778
		public const string NOTHING = "NOTHING";

		// Token: 0x040039BB RID: 14779
		public const string telemetry_wam_gameStartEvent = "telemetry_wam_gameStartEvent";

		// Token: 0x040039BC RID: 14780
		public const string telemetry_wam_levelEndEvent = "telemetry_wam_levelEndEvent";

		// Token: 0x040039BD RID: 14781
		public const string WamMachineId = "WamMachineId";

		// Token: 0x040039BE RID: 14782
		public const string WamGameId = "WamGameId";

		// Token: 0x040039BF RID: 14783
		public const string WamMLevelNumber = "WamMLevelNumber";

		// Token: 0x040039C0 RID: 14784
		public const string WamGoodMolesShown = "WamGoodMolesShown";

		// Token: 0x040039C1 RID: 14785
		public const string WamHazardMolesShown = "WamHazardMolesShown";

		// Token: 0x040039C2 RID: 14786
		public const string WamLevelMinScore = "WamLevelMinScore";

		// Token: 0x040039C3 RID: 14787
		public const string WamLevelScore = "WamLevelScore";

		// Token: 0x040039C4 RID: 14788
		public const string WamHazardMolesHit = "WamHazardMolesHit";

		// Token: 0x040039C5 RID: 14789
		public const string WamGameState = "WamGameState";

		// Token: 0x040039C6 RID: 14790
		public const string CustomMapName = "CustomMapName";

		// Token: 0x040039C7 RID: 14791
		public const string LowestFPS = "LowestFPS";

		// Token: 0x040039C8 RID: 14792
		public const string LowestFPSDrawCalls = "LowestFPSDrawCalls";

		// Token: 0x040039C9 RID: 14793
		public const string LowestFPSPlayerCount = "LowestFPSPlayerCount";

		// Token: 0x040039CA RID: 14794
		public const string AverageFPS = "AverageFPS";

		// Token: 0x040039CB RID: 14795
		public const string AverageDrawCalls = "AverageDrawCalls";

		// Token: 0x040039CC RID: 14796
		public const string AveragePlayerCount = "AveragePlayerCount";

		// Token: 0x040039CD RID: 14797
		public const string HighestFPS = "HighestFPS";

		// Token: 0x040039CE RID: 14798
		public const string HighestFPSDrawCalls = "HighestFPSDrawCalls";

		// Token: 0x040039CF RID: 14799
		public const string HighestFPSPlayerCount = "HighestFPSPlayerCount";

		// Token: 0x040039D0 RID: 14800
		public const string CustomMapCreator = "CustomMapCreator";

		// Token: 0x040039D1 RID: 14801
		public const string CustomMapModId = "CustomMapModId";

		// Token: 0x040039D2 RID: 14802
		public const string MinPlayerCount = "MinPlayerCount";

		// Token: 0x040039D3 RID: 14803
		public const string MaxPlayerCount = "MaxPlayerCount";

		// Token: 0x040039D4 RID: 14804
		public const string PlaytimeOnMap = "PlaytimeOnMap";

		// Token: 0x040039D5 RID: 14805
		public const string PlaytimeInSeconds = "PlaytimeInSeconds";

		// Token: 0x040039D6 RID: 14806
		public const string PrivateRoom = "PrivateRoom";

		// Token: 0x040039D7 RID: 14807
		public const string ghost_game_start = "ghost_game_start";

		// Token: 0x040039D8 RID: 14808
		public const string ghost_game_end = "ghost_game_end";

		// Token: 0x040039D9 RID: 14809
		public const string ghost_game_id = "ghost_game_id";

		// Token: 0x040039DA RID: 14810
		public const string event_timestamp = "event_timestamp";

		// Token: 0x040039DB RID: 14811
		public const string initial_cores_balance = "initial_cores_balance";

		// Token: 0x040039DC RID: 14812
		public const string final_cores_balance = "final_cores_balance";

		// Token: 0x040039DD RID: 14813
		public const string cores_spent_waiting_in_breakroom = "cores_spent_waiting_in_breakroom";

		// Token: 0x040039DE RID: 14814
		public const string cores_collected = "cores_collected";

		// Token: 0x040039DF RID: 14815
		public const string cores_collected_from_ghosts = "cores_collected_from_ghosts";

		// Token: 0x040039E0 RID: 14816
		public const string cores_collected_from_gathering = "cores_collected_from_gathering";

		// Token: 0x040039E1 RID: 14817
		public const string cores_spent_on_items = "cores_spent_on_items";

		// Token: 0x040039E2 RID: 14818
		public const string cores_spent_on_gates = "cores_spent_on_gates";

		// Token: 0x040039E3 RID: 14819
		public const string cores_spent_on_levels = "cores_spent_on_levels";

		// Token: 0x040039E4 RID: 14820
		public const string cores_given_to_others = "cores_given_to_others";

		// Token: 0x040039E5 RID: 14821
		public const string cores_received_from_others = "cores_received_from_others";

		// Token: 0x040039E6 RID: 14822
		public const string gates_unlocked = "gates_unlocked";

		// Token: 0x040039E7 RID: 14823
		public const string died = "died";

		// Token: 0x040039E8 RID: 14824
		public const string caught_in_anamole = "caught_in_anamole";

		// Token: 0x040039E9 RID: 14825
		public const string items_purchased = "items_purchased";

		// Token: 0x040039EA RID: 14826
		public const string levels_unlocked = "levels_unlocked";

		// Token: 0x040039EB RID: 14827
		public const string shift_cut = "shift_cut";

		// Token: 0x040039EC RID: 14828
		public const string number_of_players = "number_of_players";

		// Token: 0x040039ED RID: 14829
		public const string start_at_beginning = "start_at_beginning";

		// Token: 0x040039EE RID: 14830
		public const string seconds_into_shift_at_join = "seconds_into_shift_at_join";

		// Token: 0x040039EF RID: 14831
		public const string reason = "reason";

		// Token: 0x040039F0 RID: 14832
		public const string play_duration = "play_duration";

		// Token: 0x040039F1 RID: 14833
		public const string started_late = "started_late";

		// Token: 0x040039F2 RID: 14834
		public const string time_started = "time_started";

		// Token: 0x040039F3 RID: 14835
		public const string max_number_in_game = "max_number_in_game";

		// Token: 0x040039F4 RID: 14836
		public const string end_number_in_game = "end_number_in_game";

		// Token: 0x040039F5 RID: 14837
		public const string items_picked_up = "items_picked_up";
	}

	// Token: 0x0200072C RID: 1836
	private class BatchRunner : MonoBehaviour
	{
		// Token: 0x06002E12 RID: 11794 RVA: 0x000F4D60 File Offset: 0x000F2F60
		private IEnumerator Start()
		{
			for (;;)
			{
				float start = Time.time;
				while (Time.time < start + GorillaTelemetry.TELEMETRY_FLUSH_SEC)
				{
					yield return null;
				}
				GorillaTelemetry.FlushPlayFabTelemetry();
				GorillaTelemetry.FlushMothershipTelemetry();
			}
			yield break;
		}
	}
}
