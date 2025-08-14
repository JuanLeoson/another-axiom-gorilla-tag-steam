using System;
using System.Collections.Generic;
using GorillaTagScripts.ModIO;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000C54 RID: 3156
	internal class CMSSerializer : GorillaSerializer
	{
		// Token: 0x06004E0D RID: 19981 RVA: 0x00183EA1 File Offset: 0x001820A1
		public void Awake()
		{
			if (CMSSerializer.instance != null)
			{
				Object.Destroy(this);
			}
			CMSSerializer.instance = this;
			CMSSerializer.hasInstance = true;
		}

		// Token: 0x06004E0E RID: 19982 RVA: 0x00183EC6 File Offset: 0x001820C6
		public void OnEnable()
		{
			CustomMapManager.OnMapLoadComplete.RemoveListener(new UnityAction<bool>(this.OnCustomMapLoaded));
			CustomMapManager.OnMapLoadComplete.AddListener(new UnityAction<bool>(this.OnCustomMapLoaded));
		}

		// Token: 0x06004E0F RID: 19983 RVA: 0x00183EF4 File Offset: 0x001820F4
		public void OnDisable()
		{
			CustomMapManager.OnMapLoadComplete.RemoveListener(new UnityAction<bool>(this.OnCustomMapLoaded));
		}

		// Token: 0x06004E10 RID: 19984 RVA: 0x00183F0C File Offset: 0x0018210C
		private void OnCustomMapLoaded(bool success)
		{
			if (success)
			{
				CMSSerializer.RequestSyncTriggerHistory();
			}
		}

		// Token: 0x06004E11 RID: 19985 RVA: 0x00183F16 File Offset: 0x00182116
		public static void ResetSyncedMapObjects()
		{
			CMSSerializer.triggerHistory.Clear();
			CMSSerializer.triggerCounts.Clear();
			CMSSerializer.registeredTriggersPerScene.Clear();
			CMSSerializer.waitingForTriggerHistory = false;
			CMSSerializer.waitingForTriggerCounts = false;
		}

		// Token: 0x06004E12 RID: 19986 RVA: 0x00183F44 File Offset: 0x00182144
		public static void RegisterTrigger(string sceneName, CMSTrigger trigger)
		{
			Dictionary<byte, CMSTrigger> dictionary;
			if (CMSSerializer.registeredTriggersPerScene.TryGetValue(sceneName, out dictionary))
			{
				if (!dictionary.ContainsKey(trigger.GetID()))
				{
					dictionary.Add(trigger.GetID(), trigger);
					return;
				}
			}
			else
			{
				CMSSerializer.registeredTriggersPerScene.Add(sceneName, new Dictionary<byte, CMSTrigger>
				{
					{
						trigger.GetID(),
						trigger
					}
				});
			}
		}

		// Token: 0x06004E13 RID: 19987 RVA: 0x00183F9C File Offset: 0x0018219C
		private static bool TryGetRegisteredTrigger(byte triggerID, out CMSTrigger trigger)
		{
			trigger = null;
			foreach (KeyValuePair<string, Dictionary<byte, CMSTrigger>> keyValuePair in CMSSerializer.registeredTriggersPerScene)
			{
				if (keyValuePair.Value.TryGetValue(triggerID, out trigger))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004E14 RID: 19988 RVA: 0x00184004 File Offset: 0x00182204
		public static void UnregisterTriggers(string forScene)
		{
			CMSSerializer.registeredTriggersPerScene.Remove(forScene);
		}

		// Token: 0x06004E15 RID: 19989 RVA: 0x00184012 File Offset: 0x00182212
		public static void ResetTrigger(byte triggerID)
		{
			CMSSerializer.triggerCounts.Remove(triggerID);
		}

		// Token: 0x06004E16 RID: 19990 RVA: 0x00184020 File Offset: 0x00182220
		private static void RequestSyncTriggerHistory()
		{
			if (!CMSSerializer.hasInstance || !NetworkSystem.Instance.InRoom || NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			CMSSerializer.waitingForTriggerHistory = true;
			CMSSerializer.waitingForTriggerCounts = true;
			CMSSerializer.instance.SendRPC("RequestSyncTriggerHistory_RPC", false, Array.Empty<object>());
		}

		// Token: 0x06004E17 RID: 19991 RVA: 0x00184070 File Offset: 0x00182270
		[PunRPC]
		private void RequestSyncTriggerHistory_RPC(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestSyncTriggerHistory_RPC");
			if (!NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
			if (player.CheckSingleCallRPC(NetPlayer.SingleCallRPC.CMS_RequestTriggerHistory))
			{
				return;
			}
			player.ReceivedSingleCallRPC(NetPlayer.SingleCallRPC.CMS_RequestTriggerHistory);
			byte[] array = CMSSerializer.triggerHistory.ToArray();
			base.SendRPC("SyncTriggerHistory_RPC", info.Sender, new object[]
			{
				array
			});
			base.SendRPC("SyncTriggerCounts_RPC", info.Sender, new object[]
			{
				CMSSerializer.triggerCounts
			});
		}

		// Token: 0x06004E18 RID: 19992 RVA: 0x00184108 File Offset: 0x00182308
		[PunRPC]
		private void SyncTriggerHistory_RPC(byte[] syncedTriggerHistory, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "SyncTriggerHistory_RPC");
			if (!NetworkSystem.Instance.InRoom || !info.Sender.IsMasterClient)
			{
				return;
			}
			NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
			if (player.CheckSingleCallRPC(NetPlayer.SingleCallRPC.CMS_SyncTriggerHistory))
			{
				return;
			}
			player.ReceivedSingleCallRPC(NetPlayer.SingleCallRPC.CMS_SyncTriggerHistory);
			if (!CMSSerializer.waitingForTriggerHistory)
			{
				return;
			}
			CMSSerializer.triggerHistory.Clear();
			if (!syncedTriggerHistory.IsNullOrEmpty<byte>())
			{
				CMSSerializer.triggerHistory.AddRange(syncedTriggerHistory);
			}
			CMSSerializer.waitingForTriggerHistory = false;
			foreach (string forScene in CMSSerializer.scenesWaitingForTriggerHistory)
			{
				CMSSerializer.ProcessTriggerHistory(forScene);
			}
			CMSSerializer.scenesWaitingForTriggerHistory.Clear();
		}

		// Token: 0x06004E19 RID: 19993 RVA: 0x001841D4 File Offset: 0x001823D4
		[PunRPC]
		private void SyncTriggerCounts_RPC(Dictionary<byte, byte> syncedTriggerCounts, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "SyncTriggerCounts_RPC");
			if (!NetworkSystem.Instance.InRoom || !info.Sender.IsMasterClient)
			{
				return;
			}
			NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
			if (player.CheckSingleCallRPC(NetPlayer.SingleCallRPC.CMS_SyncTriggerCounts))
			{
				return;
			}
			player.ReceivedSingleCallRPC(NetPlayer.SingleCallRPC.CMS_SyncTriggerCounts);
			if (!CMSSerializer.waitingForTriggerCounts)
			{
				return;
			}
			CMSSerializer.triggerCounts.Clear();
			if (syncedTriggerCounts != null && syncedTriggerCounts.Count > 0)
			{
				CMSSerializer.triggerCounts = syncedTriggerCounts;
			}
			CMSSerializer.waitingForTriggerCounts = false;
			foreach (string forScene in CMSSerializer.scenesWaitingForTriggerCounts)
			{
				CMSSerializer.ProcessTriggerCounts(forScene);
			}
			CMSSerializer.scenesWaitingForTriggerCounts.Clear();
		}

		// Token: 0x06004E1A RID: 19994 RVA: 0x001842A0 File Offset: 0x001824A0
		public static void ProcessSceneLoad(string sceneName)
		{
			if (CMSSerializer.waitingForTriggerHistory)
			{
				CMSSerializer.scenesWaitingForTriggerHistory.Add(sceneName);
			}
			else
			{
				CMSSerializer.ProcessTriggerHistory(sceneName);
			}
			if (CMSSerializer.waitingForTriggerCounts)
			{
				CMSSerializer.scenesWaitingForTriggerCounts.Add(sceneName);
				return;
			}
			CMSSerializer.ProcessTriggerCounts(sceneName);
		}

		// Token: 0x06004E1B RID: 19995 RVA: 0x001842D8 File Offset: 0x001824D8
		private static void ProcessTriggerHistory(string forScene)
		{
			Dictionary<byte, CMSTrigger> dictionary;
			if (CMSSerializer.registeredTriggersPerScene.TryGetValue(forScene, out dictionary))
			{
				foreach (byte key in CMSSerializer.triggerHistory)
				{
					CMSTrigger cmstrigger;
					if (dictionary.TryGetValue(key, out cmstrigger))
					{
						cmstrigger.Trigger(1.0, false, true);
					}
				}
			}
			UnityEvent<string> onTriggerHistoryProcessedForScene = CMSSerializer.OnTriggerHistoryProcessedForScene;
			if (onTriggerHistoryProcessedForScene == null)
			{
				return;
			}
			onTriggerHistoryProcessedForScene.Invoke(forScene);
		}

		// Token: 0x06004E1C RID: 19996 RVA: 0x00184360 File Offset: 0x00182560
		private static void ProcessTriggerCounts(string forScene)
		{
			Dictionary<byte, CMSTrigger> dictionary;
			if (CMSSerializer.registeredTriggersPerScene.TryGetValue(forScene, out dictionary))
			{
				List<byte> list = new List<byte>();
				foreach (KeyValuePair<byte, byte> keyValuePair in CMSSerializer.triggerCounts)
				{
					CMSTrigger cmstrigger;
					if (dictionary.TryGetValue(keyValuePair.Key, out cmstrigger))
					{
						if (cmstrigger.numAllowedTriggers > 0)
						{
							cmstrigger.SetTriggerCount(keyValuePair.Value);
						}
						else
						{
							list.Add(keyValuePair.Key);
						}
					}
				}
				foreach (byte key in list)
				{
					CMSSerializer.triggerCounts.Remove(key);
				}
			}
		}

		// Token: 0x06004E1D RID: 19997 RVA: 0x00184440 File Offset: 0x00182640
		public static void RequestTrigger(byte triggerID)
		{
			if (!CMSSerializer.hasInstance)
			{
				return;
			}
			if (!NetworkSystem.Instance.InRoom || NetworkSystem.Instance.IsMasterClient)
			{
				double triggerTime = (double)Time.time;
				if (NetworkSystem.Instance.InRoom)
				{
					triggerTime = PhotonNetwork.Time;
					CMSSerializer.instance.SendRPC("ActivateTrigger_RPC", true, new object[]
					{
						triggerID,
						NetworkSystem.Instance.LocalPlayer.ActorNumber
					});
				}
				CMSSerializer.instance.ActivateTrigger(triggerID, triggerTime, true);
				return;
			}
			CMSSerializer.instance.SendRPC("RequestTrigger_RPC", false, new object[]
			{
				triggerID
			});
		}

		// Token: 0x06004E1E RID: 19998 RVA: 0x001844F0 File Offset: 0x001826F0
		[PunRPC]
		private void RequestTrigger_RPC(byte triggerID, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestTrigger_RPC");
			if (!NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
			RigContainer rigContainer;
			if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[11].CallLimitSettings.CheckCallTime(Time.unscaledTime))
			{
				return;
			}
			CMSTrigger cmstrigger;
			if (CMSSerializer.TryGetRegisteredTrigger(triggerID, out cmstrigger))
			{
				if (!cmstrigger.CanTrigger())
				{
					return;
				}
				Vector3 position = cmstrigger.gameObject.transform.position;
				RigContainer rigContainer2;
				if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer2))
				{
					return;
				}
				if ((rigContainer2.Rig.bodyTransform.position - position).sqrMagnitude > cmstrigger.validationDistanceSquared)
				{
					return;
				}
			}
			base.SendRPC("ActivateTrigger_RPC", true, new object[]
			{
				triggerID,
				info.Sender.ActorNumber
			});
			this.ActivateTrigger(triggerID, info.SentServerTime, false);
		}

		// Token: 0x06004E1F RID: 19999 RVA: 0x00184608 File Offset: 0x00182808
		[PunRPC]
		private void ActivateTrigger_RPC(byte triggerID, int originatingPlayer, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "ActivateTrigger_RPC");
			if (!NetworkSystem.Instance.InRoom || !info.Sender.IsMasterClient)
			{
				return;
			}
			if (info.SentServerTime < 0.0 || info.SentServerTime > 4294967.295)
			{
				return;
			}
			double num = (double)PhotonNetwork.GetPing() / 1000.0;
			if (!Utils.ValidateServerTime(info.SentServerTime, Math.Max(10.0, num * 2.0)))
			{
				return;
			}
			if (!CMSSerializer.ActivateTriggerCallLimiter.CheckCallTime(Time.unscaledTime))
			{
				return;
			}
			this.ActivateTrigger(triggerID, info.SentServerTime, NetworkSystem.Instance.LocalPlayer.ActorNumber == originatingPlayer);
		}

		// Token: 0x06004E20 RID: 20000 RVA: 0x001846CC File Offset: 0x001828CC
		private void ActivateTrigger(byte triggerID, double triggerTime = -1.0, bool originatedLocally = false)
		{
			CMSTrigger cmstrigger;
			bool flag = CMSSerializer.TryGetRegisteredTrigger(triggerID, out cmstrigger);
			if (!double.IsFinite(triggerTime))
			{
				triggerTime = -1.0;
			}
			byte b;
			bool flag2 = CMSSerializer.triggerCounts.TryGetValue(triggerID, out b);
			bool flag3 = !flag || cmstrigger.numAllowedTriggers > 0;
			if (flag2)
			{
				CMSSerializer.triggerCounts[triggerID] = ((b == byte.MaxValue) ? byte.MaxValue : (b += 1));
			}
			else if (flag3)
			{
				CMSSerializer.triggerCounts.Add(triggerID, 1);
			}
			CMSSerializer.triggerHistory.Remove(triggerID);
			CMSSerializer.triggerHistory.Add(triggerID);
			if (flag)
			{
				cmstrigger.Trigger(triggerTime, originatedLocally, false);
			}
		}

		// Token: 0x04005706 RID: 22278
		[OnEnterPlay_SetNull]
		private static volatile CMSSerializer instance;

		// Token: 0x04005707 RID: 22279
		[OnEnterPlay_Set(false)]
		private static bool hasInstance;

		// Token: 0x04005708 RID: 22280
		private static Dictionary<string, Dictionary<byte, CMSTrigger>> registeredTriggersPerScene = new Dictionary<string, Dictionary<byte, CMSTrigger>>();

		// Token: 0x04005709 RID: 22281
		private static List<byte> triggerHistory = new List<byte>();

		// Token: 0x0400570A RID: 22282
		private static Dictionary<byte, byte> triggerCounts = new Dictionary<byte, byte>();

		// Token: 0x0400570B RID: 22283
		private static bool waitingForTriggerHistory;

		// Token: 0x0400570C RID: 22284
		private static List<string> scenesWaitingForTriggerHistory = new List<string>();

		// Token: 0x0400570D RID: 22285
		private static bool waitingForTriggerCounts;

		// Token: 0x0400570E RID: 22286
		private static List<string> scenesWaitingForTriggerCounts = new List<string>();

		// Token: 0x0400570F RID: 22287
		private static CallLimiter ActivateTriggerCallLimiter = new CallLimiter(50, 1f, 0.5f);

		// Token: 0x04005710 RID: 22288
		public static UnityEvent<string> OnTriggerHistoryProcessedForScene = new UnityEvent<string>();
	}
}
