using System;
using UnityEngine;

// Token: 0x02000789 RID: 1929
[Serializable]
public class RankedMultiplayerStatisticInt : RankedMultiplayerStatistic
{
	// Token: 0x06003064 RID: 12388 RVA: 0x000FE2AD File Offset: 0x000FC4AD
	public RankedMultiplayerStatisticInt(string n, int val, int min = 0, int max = 2147483647, RankedMultiplayerStatistic.SerializationType s = RankedMultiplayerStatistic.SerializationType.None) : base(n, s)
	{
		this.intValue = val;
		this.minValue = min;
		this.maxValue = max;
	}

	// Token: 0x06003065 RID: 12389 RVA: 0x000FE2CE File Offset: 0x000FC4CE
	public static implicit operator int(RankedMultiplayerStatisticInt stat)
	{
		if (stat.IsValid)
		{
			return stat.intValue;
		}
		Debug.LogError("Attempting to retrieve value for user data that does not yet have a valid key: " + stat.name);
		return 0;
	}

	// Token: 0x06003066 RID: 12390 RVA: 0x000FE2F5 File Offset: 0x000FC4F5
	public void Set(int val)
	{
		this.intValue = Mathf.Clamp(val, this.minValue, this.maxValue);
		this.Save();
	}

	// Token: 0x06003067 RID: 12391 RVA: 0x000FE315 File Offset: 0x000FC515
	public int Get()
	{
		return this.intValue;
	}

	// Token: 0x06003068 RID: 12392 RVA: 0x000FE320 File Offset: 0x000FC520
	public override bool TrySetValue(string valAsString)
	{
		int value;
		bool flag = int.TryParse(valAsString, out value);
		if (flag)
		{
			this.intValue = Mathf.Clamp(value, this.minValue, this.maxValue);
		}
		return flag;
	}

	// Token: 0x06003069 RID: 12393 RVA: 0x000FE350 File Offset: 0x000FC550
	public void Increment()
	{
		this.AddTo(1);
	}

	// Token: 0x0600306A RID: 12394 RVA: 0x000FE359 File Offset: 0x000FC559
	public void AddTo(int amount)
	{
		this.intValue += amount;
		this.intValue = Mathf.Clamp(this.intValue, this.minValue, this.maxValue);
		this.Save();
	}

	// Token: 0x0600306B RID: 12395 RVA: 0x000FE38C File Offset: 0x000FC58C
	protected override void Save()
	{
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
		if (serializationType != RankedMultiplayerStatistic.SerializationType.Mothership && serializationType == RankedMultiplayerStatistic.SerializationType.PlayerPrefs)
		{
			PlayerPrefs.SetInt(this.name, this.intValue);
			PlayerPrefs.Save();
		}
	}

	// Token: 0x0600306C RID: 12396 RVA: 0x000FE3C0 File Offset: 0x000FC5C0
	public override void Load()
	{
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
		if (serializationType != RankedMultiplayerStatistic.SerializationType.Mothership)
		{
			if (serializationType == RankedMultiplayerStatistic.SerializationType.PlayerPrefs)
			{
				base.IsValid = true;
				this.intValue = PlayerPrefs.GetInt(this.name, this.intValue);
				return;
			}
		}
		else
		{
			base.IsValid = false;
		}
	}

	// Token: 0x0600306D RID: 12397 RVA: 0x000FE402 File Offset: 0x000FC602
	public override string ToString()
	{
		return this.intValue.ToString();
	}

	// Token: 0x04003C5C RID: 15452
	private int intValue;

	// Token: 0x04003C5D RID: 15453
	private int minValue;

	// Token: 0x04003C5E RID: 15454
	private int maxValue;
}
