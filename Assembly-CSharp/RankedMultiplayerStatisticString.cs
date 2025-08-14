using System;
using UnityEngine;

// Token: 0x0200078B RID: 1931
[Serializable]
public class RankedMultiplayerStatisticString : RankedMultiplayerStatistic
{
	// Token: 0x06003078 RID: 12408 RVA: 0x000FE577 File Offset: 0x000FC777
	public RankedMultiplayerStatisticString(string n, string val, RankedMultiplayerStatistic.SerializationType s = RankedMultiplayerStatistic.SerializationType.None) : base(n, s)
	{
		this.stringValue = val;
	}

	// Token: 0x06003079 RID: 12409 RVA: 0x000FE588 File Offset: 0x000FC788
	public static implicit operator string(RankedMultiplayerStatisticString stat)
	{
		if (stat.IsValid)
		{
			return stat.stringValue;
		}
		Debug.LogError("Attempting to retrieve value for user data that does not yet have a valid key: " + stat.name);
		return string.Empty;
	}

	// Token: 0x0600307A RID: 12410 RVA: 0x000FE5B3 File Offset: 0x000FC7B3
	public void Set(string val)
	{
		this.stringValue = val;
		this.Save();
	}

	// Token: 0x0600307B RID: 12411 RVA: 0x000FE5C2 File Offset: 0x000FC7C2
	public string Get()
	{
		return this.stringValue;
	}

	// Token: 0x0600307C RID: 12412 RVA: 0x000FE5CA File Offset: 0x000FC7CA
	public override bool TrySetValue(string valAsString)
	{
		this.stringValue = valAsString;
		return true;
	}

	// Token: 0x0600307D RID: 12413 RVA: 0x000FE5D4 File Offset: 0x000FC7D4
	protected override void Save()
	{
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
		if (serializationType != RankedMultiplayerStatistic.SerializationType.Mothership && serializationType == RankedMultiplayerStatistic.SerializationType.PlayerPrefs)
		{
			PlayerPrefs.SetString(this.name, this.stringValue);
			PlayerPrefs.Save();
		}
	}

	// Token: 0x0600307E RID: 12414 RVA: 0x000FE608 File Offset: 0x000FC808
	public override void Load()
	{
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
		if (serializationType != RankedMultiplayerStatistic.SerializationType.Mothership)
		{
			if (serializationType == RankedMultiplayerStatistic.SerializationType.PlayerPrefs)
			{
				base.IsValid = true;
				this.stringValue = PlayerPrefs.GetString(this.name, this.stringValue);
				return;
			}
		}
		else
		{
			base.IsValid = false;
		}
	}

	// Token: 0x0600307F RID: 12415 RVA: 0x000FE5C2 File Offset: 0x000FC7C2
	public override string ToString()
	{
		return this.stringValue;
	}

	// Token: 0x04003C62 RID: 15458
	private string stringValue;
}
