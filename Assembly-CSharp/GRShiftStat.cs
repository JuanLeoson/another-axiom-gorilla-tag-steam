using System;
using System.Collections.Generic;
using System.IO;

// Token: 0x0200066F RID: 1647
public class GRShiftStat
{
	// Token: 0x06002846 RID: 10310 RVA: 0x000D9056 File Offset: 0x000D7256
	public void Serialize(BinaryWriter writer)
	{
		writer.Write(this.GetShiftStat(GRShiftStatType.EnemyDeaths));
		writer.Write(this.GetShiftStat(GRShiftStatType.PlayerDeaths));
		writer.Write(this.GetShiftStat(GRShiftStatType.CoresCollected));
		writer.Write(this.GetShiftStat(GRShiftStatType.SentientCoresCollected));
	}

	// Token: 0x06002847 RID: 10311 RVA: 0x000D908C File Offset: 0x000D728C
	public void Deserialize(BinaryReader reader)
	{
		this.shiftStats[GRShiftStatType.EnemyDeaths] = reader.ReadInt32();
		this.shiftStats[GRShiftStatType.PlayerDeaths] = reader.ReadInt32();
		this.shiftStats[GRShiftStatType.CoresCollected] = reader.ReadInt32();
		this.shiftStats[GRShiftStatType.SentientCoresCollected] = reader.ReadInt32();
	}

	// Token: 0x06002848 RID: 10312 RVA: 0x000D90E1 File Offset: 0x000D72E1
	public void SetShiftStat(GRShiftStatType stat, int newValue)
	{
		this.shiftStats[stat] = newValue;
	}

	// Token: 0x06002849 RID: 10313 RVA: 0x000D90F0 File Offset: 0x000D72F0
	public void IncrementShiftStat(GRShiftStatType stat)
	{
		if (this.shiftStats.ContainsKey(stat))
		{
			Dictionary<GRShiftStatType, int> dictionary = this.shiftStats;
			int num = dictionary[stat];
			dictionary[stat] = num + 1;
			return;
		}
		this.shiftStats[stat] = 1;
	}

	// Token: 0x0600284A RID: 10314 RVA: 0x000D9132 File Offset: 0x000D7332
	public void ResetShiftStats()
	{
		this.shiftStats[GRShiftStatType.EnemyDeaths] = 0;
		this.shiftStats[GRShiftStatType.PlayerDeaths] = 0;
		this.shiftStats[GRShiftStatType.CoresCollected] = 0;
		this.shiftStats[GRShiftStatType.SentientCoresCollected] = 0;
	}

	// Token: 0x0600284B RID: 10315 RVA: 0x000D9168 File Offset: 0x000D7368
	public int GetShiftStat(GRShiftStatType stat)
	{
		if (this.shiftStats.ContainsKey(stat))
		{
			return this.shiftStats[stat];
		}
		return 0;
	}

	// Token: 0x040033C9 RID: 13257
	public Dictionary<GRShiftStatType, int> shiftStats = new Dictionary<GRShiftStatType, int>();
}
