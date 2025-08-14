﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using PlayFab;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000D99 RID: 3481
	public class TitleDataFeatureFlags
	{
		// Token: 0x17000841 RID: 2113
		// (get) Token: 0x0600566E RID: 22126 RVA: 0x001AD230 File Offset: 0x001AB430
		// (set) Token: 0x0600566F RID: 22127 RVA: 0x001AD238 File Offset: 0x001AB438
		public bool ready { get; private set; }

		// Token: 0x06005670 RID: 22128 RVA: 0x001AD241 File Offset: 0x001AB441
		public void FetchFeatureFlags()
		{
			PlayFabTitleDataCache.Instance.GetTitleData(this.TitleDataKey, delegate(string json)
			{
				FeatureFlagListData featureFlagListData = JsonUtility.FromJson<FeatureFlagListData>(json);
				foreach (FeatureFlagData featureFlagData in featureFlagListData.flags)
				{
					if (featureFlagData.valueType == "percent")
					{
						this.flagValueByName.AddOrUpdate(featureFlagData.name, featureFlagData.value);
					}
					List<string> alwaysOnForUsers = featureFlagData.alwaysOnForUsers;
					if (alwaysOnForUsers != null && alwaysOnForUsers.Count > 0)
					{
						this.flagValueByUser.AddOrUpdate(featureFlagData.name, featureFlagData.alwaysOnForUsers);
					}
				}
				Debug.Log(string.Format("GorillaServer: Fetched flags ({0})", featureFlagListData));
				this.ready = true;
			}, delegate(PlayFabError e)
			{
				Debug.LogError("Error fetching rollout feature flags: " + e.ErrorMessage);
				this.ready = true;
			});
		}

		// Token: 0x06005671 RID: 22129 RVA: 0x001AD26C File Offset: 0x001AB46C
		public bool IsEnabledForUser(string flagName)
		{
			string playFabPlayerId = PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			Debug.Log(string.Concat(new string[]
			{
				"GorillaServer: Checking flag ",
				flagName,
				" for ",
				playFabPlayerId,
				"\nFlag values:\n",
				JsonConvert.SerializeObject(this.flagValueByName),
				"\n\nDefaults:\n",
				JsonConvert.SerializeObject(this.defaults)
			}));
			List<string> list;
			if (this.flagValueByUser.TryGetValue(flagName, out list) && list != null && list.Contains(playFabPlayerId))
			{
				return true;
			}
			int num;
			if (!this.flagValueByName.TryGetValue(flagName, out num))
			{
				Debug.Log("GorillaServer: Returning default");
				bool flag;
				return this.defaults.TryGetValue(flagName, out flag) && flag;
			}
			Debug.Log(string.Format("GorillaServer: Rollout % is {0}", num));
			if (num <= 0)
			{
				Debug.Log("GorillaServer: " + flagName + " is off (<=0%).");
				return false;
			}
			if (num >= 100)
			{
				Debug.Log("GorillaServer: " + flagName + " is on (>=100%).");
				return true;
			}
			uint num2 = XXHash32.Compute(Encoding.UTF8.GetBytes(playFabPlayerId), 0U) % 100U;
			Debug.Log(string.Format("GorillaServer: Partial rollout, seed = {0} flag value = {1}", num2, (ulong)num2 < (ulong)((long)num)));
			return (ulong)num2 < (ulong)((long)num);
		}

		// Token: 0x04006024 RID: 24612
		public string TitleDataKey = "DeployFeatureFlags";

		// Token: 0x04006026 RID: 24614
		public Dictionary<string, bool> defaults = new Dictionary<string, bool>
		{
			{
				"2024-05-ReturnCurrentVersionV2",
				true
			},
			{
				"2024-05-ReturnMyOculusHashV2",
				true
			},
			{
				"2024-05-TryDistributeCurrencyV2",
				true
			},
			{
				"2024-05-AddOrRemoveDLCOwnershipV2",
				true
			},
			{
				"2024-05-BroadcastMyRoomV2",
				true
			},
			{
				"2024-06-CosmeticsAuthenticationV2",
				true
			},
			{
				"2024-08-KIDIntegrationV1",
				true
			},
			{
				"2025-04-CosmeticsAuthenticationV2-SetData",
				false
			},
			{
				"2025-04-CosmeticsAuthenticationV2-ReadData",
				false
			},
			{
				"2025-04-CosmeticsAuthenticationV2-Compat",
				true
			}
		};

		// Token: 0x04006027 RID: 24615
		private Dictionary<string, int> flagValueByName = new Dictionary<string, int>();

		// Token: 0x04006028 RID: 24616
		private Dictionary<string, List<string>> flagValueByUser = new Dictionary<string, List<string>>();
	}
}
