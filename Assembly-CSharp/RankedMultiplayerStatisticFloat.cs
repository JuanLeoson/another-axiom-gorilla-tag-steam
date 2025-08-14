using System;
using UnityEngine;

// Token: 0x0200078A RID: 1930
[Serializable]
public class RankedMultiplayerStatisticFloat : RankedMultiplayerStatistic
{
	// Token: 0x0600306E RID: 12398 RVA: 0x000FE40F File Offset: 0x000FC60F
	public RankedMultiplayerStatisticFloat(string n, float val, float min = 0f, float max = 3.4028235E+38f, RankedMultiplayerStatistic.SerializationType s = RankedMultiplayerStatistic.SerializationType.None) : base(n, s)
	{
		this.floatValue = val;
		this.minValue = min;
		this.maxValue = max;
	}

	// Token: 0x0600306F RID: 12399 RVA: 0x000FE430 File Offset: 0x000FC630
	public static implicit operator float(RankedMultiplayerStatisticFloat stat)
	{
		if (stat.IsValid)
		{
			return stat.floatValue;
		}
		Debug.LogError("Attempting to retrieve value for user data that does not yet have a valid key: " + stat.name);
		return 0f;
	}

	// Token: 0x06003070 RID: 12400 RVA: 0x000FE45B File Offset: 0x000FC65B
	public void Set(float val)
	{
		this.floatValue = Mathf.Clamp(val, this.minValue, this.maxValue);
		this.Save();
	}

	// Token: 0x06003071 RID: 12401 RVA: 0x000FE47B File Offset: 0x000FC67B
	public float Get()
	{
		return this.floatValue;
	}

	// Token: 0x06003072 RID: 12402 RVA: 0x000FE484 File Offset: 0x000FC684
	public override bool TrySetValue(string valAsString)
	{
		float value;
		bool flag = float.TryParse(valAsString, out value);
		if (flag)
		{
			this.floatValue = Mathf.Clamp(value, this.minValue, this.maxValue);
		}
		return flag;
	}

	// Token: 0x06003073 RID: 12403 RVA: 0x000FE4B4 File Offset: 0x000FC6B4
	public void Increment()
	{
		this.AddTo(1f);
	}

	// Token: 0x06003074 RID: 12404 RVA: 0x000FE4C1 File Offset: 0x000FC6C1
	public void AddTo(float amount)
	{
		this.floatValue += amount;
		this.floatValue = Mathf.Clamp(this.floatValue, this.minValue, this.maxValue);
		this.Save();
	}

	// Token: 0x06003075 RID: 12405 RVA: 0x000FE4F4 File Offset: 0x000FC6F4
	protected override void Save()
	{
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
		if (serializationType != RankedMultiplayerStatistic.SerializationType.Mothership && serializationType == RankedMultiplayerStatistic.SerializationType.PlayerPrefs)
		{
			PlayerPrefs.SetFloat(this.name, this.floatValue);
			PlayerPrefs.Save();
		}
	}

	// Token: 0x06003076 RID: 12406 RVA: 0x000FE528 File Offset: 0x000FC728
	public override void Load()
	{
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
		if (serializationType != RankedMultiplayerStatistic.SerializationType.Mothership)
		{
			if (serializationType == RankedMultiplayerStatistic.SerializationType.PlayerPrefs)
			{
				base.IsValid = true;
				this.floatValue = PlayerPrefs.GetFloat(this.name, this.floatValue);
				return;
			}
		}
		else
		{
			base.IsValid = false;
		}
	}

	// Token: 0x06003077 RID: 12407 RVA: 0x000FE56A File Offset: 0x000FC76A
	public override string ToString()
	{
		return this.floatValue.ToString();
	}

	// Token: 0x04003C5F RID: 15455
	private float floatValue;

	// Token: 0x04003C60 RID: 15456
	private float minValue;

	// Token: 0x04003C61 RID: 15457
	private float maxValue;
}
