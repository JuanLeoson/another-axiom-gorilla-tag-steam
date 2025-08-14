using System;
using System.Collections.Generic;
using GameObjectScheduling;
using GorillaNetworking;
using UnityEngine;

namespace GorillaGameModes
{
	// Token: 0x02000BE2 RID: 3042
	[CreateAssetMenu(fileName = "New Game Mode Zone Map", menuName = "Game Settings/Game Mode Zone Map", order = 2)]
	public class GameModeZoneMapping : ScriptableObject
	{
		// Token: 0x1700071D RID: 1821
		// (get) Token: 0x060049E1 RID: 18913 RVA: 0x00166DF5 File Offset: 0x00164FF5
		public HashSet<GameModeType> AllModes
		{
			get
			{
				this.init();
				return this.allModes;
			}
		}

		// Token: 0x060049E2 RID: 18914 RVA: 0x00166E04 File Offset: 0x00165004
		private void init()
		{
			if (this.allModes != null)
			{
				return;
			}
			this.allModes = new HashSet<GameModeType>();
			for (int i = 0; i < this.defaultGameModes.Length; i++)
			{
				if (!this.allModes.Contains(this.defaultGameModes[i]))
				{
					this.allModes.Add(this.defaultGameModes[i]);
				}
			}
			this.publicZoneGameModesLookup = new Dictionary<GTZone, HashSet<GameModeType>>();
			this.privateZoneGameModesLookup = new Dictionary<GTZone, HashSet<GameModeType>>();
			for (int j = 0; j < this.zoneGameModes.Length; j++)
			{
				for (int k = 0; k < this.zoneGameModes[j].zone.Length; k++)
				{
					this.publicZoneGameModesLookup.Add(this.zoneGameModes[j].zone[k], new HashSet<GameModeType>(this.zoneGameModes[j].modes));
					for (int l = 0; l < this.zoneGameModes[j].modes.Length; l++)
					{
						if (!this.allModes.Contains(this.zoneGameModes[j].modes[l]))
						{
							this.allModes.Add(this.zoneGameModes[j].modes[l]);
						}
					}
					if (this.zoneGameModes[j].privateModes.Length != 0)
					{
						this.privateZoneGameModesLookup.Add(this.zoneGameModes[j].zone[k], new HashSet<GameModeType>(this.zoneGameModes[j].privateModes));
						for (int m = 0; m < this.zoneGameModes[j].privateModes.Length; m++)
						{
							if (!this.allModes.Contains(this.zoneGameModes[j].privateModes[m]))
							{
								this.allModes.Add(this.zoneGameModes[j].privateModes[m]);
							}
						}
					}
					else
					{
						this.privateZoneGameModesLookup.Add(this.zoneGameModes[j].zone[k], new HashSet<GameModeType>(this.zoneGameModes[j].modes));
					}
				}
			}
			this.modeNameLookup = new Dictionary<GameModeType, string>();
			for (int n = 0; n < this.gameModeNameOverrides.Length; n++)
			{
				this.modeNameLookup.Add(this.gameModeNameOverrides[n].mode, this.gameModeNameOverrides[n].displayName);
			}
			this.isNewLookup = new HashSet<GameModeType>(this.newThisUpdate);
			this.gameModeTypeCountdownsLookup = new Dictionary<GameModeType, CountdownTextDate>();
			for (int num = 0; num < this.gameModeTypeCountdowns.Length; num++)
			{
				this.gameModeTypeCountdownsLookup.Add(this.gameModeTypeCountdowns[num].mode, this.gameModeTypeCountdowns[num].countdownTextDate);
			}
		}

		// Token: 0x060049E3 RID: 18915 RVA: 0x001670DC File Offset: 0x001652DC
		public HashSet<GameModeType> GetModesForZone(GTZone zone, bool isPrivate)
		{
			this.init();
			if (isPrivate && this.privateZoneGameModesLookup.ContainsKey(zone))
			{
				return this.privateZoneGameModesLookup[zone];
			}
			if (this.publicZoneGameModesLookup.ContainsKey(zone))
			{
				return this.publicZoneGameModesLookup[zone];
			}
			return new HashSet<GameModeType>(this.defaultGameModes);
		}

		// Token: 0x060049E4 RID: 18916 RVA: 0x00167133 File Offset: 0x00165333
		internal string GetModeName(GameModeType mode)
		{
			this.init();
			if (this.modeNameLookup.ContainsKey(mode))
			{
				return this.modeNameLookup[mode];
			}
			return mode.ToString().ToUpper();
		}

		// Token: 0x060049E5 RID: 18917 RVA: 0x00167168 File Offset: 0x00165368
		internal bool IsNew(GameModeType mode)
		{
			this.init();
			return this.isNewLookup.Contains(mode);
		}

		// Token: 0x060049E6 RID: 18918 RVA: 0x0016717C File Offset: 0x0016537C
		internal CountdownTextDate GetCountdown(GameModeType mode)
		{
			this.init();
			if (this.gameModeTypeCountdownsLookup.ContainsKey(mode))
			{
				return this.gameModeTypeCountdownsLookup[mode];
			}
			return null;
		}

		// Token: 0x060049E7 RID: 18919 RVA: 0x001671A0 File Offset: 0x001653A0
		internal GameModeType VerifyModeForZone(GTZone zone, GameModeType mode, bool isPrivate)
		{
			if (GorillaComputer.instance.IsPlayerInVirtualStump())
			{
				zone = GTZone.customMaps;
			}
			if (zone == GTZone.none)
			{
				return mode;
			}
			HashSet<GameModeType> hashSet;
			if (isPrivate && this.privateZoneGameModesLookup.ContainsKey(zone))
			{
				hashSet = this.privateZoneGameModesLookup[zone];
			}
			else if (this.publicZoneGameModesLookup.ContainsKey(zone))
			{
				hashSet = this.publicZoneGameModesLookup[zone];
			}
			else
			{
				hashSet = new HashSet<GameModeType>(this.defaultGameModes);
			}
			if (hashSet.Contains(mode))
			{
				return mode;
			}
			using (HashSet<GameModeType>.Enumerator enumerator = hashSet.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
			}
			return GameModeType.Casual;
		}

		// Token: 0x040052A1 RID: 21153
		[SerializeField]
		private GameModeNameOverrides[] gameModeNameOverrides;

		// Token: 0x040052A2 RID: 21154
		[SerializeField]
		private GameModeType[] defaultGameModes;

		// Token: 0x040052A3 RID: 21155
		[SerializeField]
		private ZoneGameModes[] zoneGameModes;

		// Token: 0x040052A4 RID: 21156
		[SerializeField]
		private GameModeTypeCountdown[] gameModeTypeCountdowns;

		// Token: 0x040052A5 RID: 21157
		[SerializeField]
		private GameModeType[] newThisUpdate;

		// Token: 0x040052A6 RID: 21158
		private Dictionary<GTZone, HashSet<GameModeType>> publicZoneGameModesLookup;

		// Token: 0x040052A7 RID: 21159
		private Dictionary<GTZone, HashSet<GameModeType>> privateZoneGameModesLookup;

		// Token: 0x040052A8 RID: 21160
		private Dictionary<GameModeType, string> modeNameLookup;

		// Token: 0x040052A9 RID: 21161
		private HashSet<GameModeType> isNewLookup;

		// Token: 0x040052AA RID: 21162
		private Dictionary<GameModeType, CountdownTextDate> gameModeTypeCountdownsLookup;

		// Token: 0x040052AB RID: 21163
		private HashSet<GameModeType> allModes;
	}
}
