using System;

// Token: 0x02000B60 RID: 2912
[Serializable]
public struct ZoneKey : IEquatable<ZoneKey>, IComparable<ZoneKey>, IComparable
{
	// Token: 0x1700069A RID: 1690
	// (get) Token: 0x060045B9 RID: 17849 RVA: 0x0015C25A File Offset: 0x0015A45A
	public int intValue
	{
		get
		{
			return ZoneKey.ToIntValue(this.zoneId, this.subZoneId);
		}
	}

	// Token: 0x1700069B RID: 1691
	// (get) Token: 0x060045BA RID: 17850 RVA: 0x0015C26D File Offset: 0x0015A46D
	public string zoneName
	{
		get
		{
			return this.zoneId.GetName<GTZone>();
		}
	}

	// Token: 0x1700069C RID: 1692
	// (get) Token: 0x060045BB RID: 17851 RVA: 0x0015C27A File Offset: 0x0015A47A
	public string subZoneName
	{
		get
		{
			return this.subZoneId.GetName<GTSubZone>();
		}
	}

	// Token: 0x060045BC RID: 17852 RVA: 0x0015C287 File Offset: 0x0015A487
	public ZoneKey(GTZone zone, GTSubZone subZone)
	{
		this.zoneId = zone;
		this.subZoneId = subZone;
	}

	// Token: 0x060045BD RID: 17853 RVA: 0x0015C297 File Offset: 0x0015A497
	public override int GetHashCode()
	{
		return this.intValue;
	}

	// Token: 0x060045BE RID: 17854 RVA: 0x0015C29F File Offset: 0x0015A49F
	public override string ToString()
	{
		return string.Concat(new string[]
		{
			"ZoneKey { ",
			this.zoneName,
			" : ",
			this.subZoneName,
			" }"
		});
	}

	// Token: 0x060045BF RID: 17855 RVA: 0x0015C2D6 File Offset: 0x0015A4D6
	public static ZoneKey GetKey(GTZone zone, GTSubZone subZone)
	{
		return new ZoneKey(zone, subZone);
	}

	// Token: 0x060045C0 RID: 17856 RVA: 0x0015C2DF File Offset: 0x0015A4DF
	public static int ToIntValue(GTZone zone, GTSubZone subZone)
	{
		if (zone == GTZone.none && subZone == GTSubZone.none)
		{
			return 0;
		}
		return StaticHash.Compute(zone.GetLongValue<GTZone>(), subZone.GetLongValue<GTSubZone>());
	}

	// Token: 0x060045C1 RID: 17857 RVA: 0x0015C2FC File Offset: 0x0015A4FC
	public bool Equals(ZoneKey other)
	{
		return this.intValue == other.intValue && this.zoneId == other.zoneId && this.subZoneId == other.subZoneId;
	}

	// Token: 0x060045C2 RID: 17858 RVA: 0x0015C32C File Offset: 0x0015A52C
	public override bool Equals(object obj)
	{
		if (obj is ZoneKey)
		{
			ZoneKey other = (ZoneKey)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x060045C3 RID: 17859 RVA: 0x0015C351 File Offset: 0x0015A551
	public static bool operator ==(ZoneKey x, ZoneKey y)
	{
		return x.Equals(y);
	}

	// Token: 0x060045C4 RID: 17860 RVA: 0x0015C35B File Offset: 0x0015A55B
	public static bool operator !=(ZoneKey x, ZoneKey y)
	{
		return !x.Equals(y);
	}

	// Token: 0x060045C5 RID: 17861 RVA: 0x0015C368 File Offset: 0x0015A568
	public int CompareTo(ZoneKey other)
	{
		int num = this.intValue.CompareTo(other.intValue);
		if (num == 0)
		{
			num = string.CompareOrdinal(this.zoneName, other.zoneName);
		}
		if (num == 0)
		{
			num = string.CompareOrdinal(this.subZoneName, other.subZoneName);
		}
		return num;
	}

	// Token: 0x060045C6 RID: 17862 RVA: 0x0015C3B8 File Offset: 0x0015A5B8
	public int CompareTo(object obj)
	{
		if (obj is ZoneKey)
		{
			ZoneKey other = (ZoneKey)obj;
			return this.CompareTo(other);
		}
		return 1;
	}

	// Token: 0x060045C7 RID: 17863 RVA: 0x0015C3DD File Offset: 0x0015A5DD
	public static bool operator <(ZoneKey x, ZoneKey y)
	{
		return x.CompareTo(y) < 0;
	}

	// Token: 0x060045C8 RID: 17864 RVA: 0x0015C3EA File Offset: 0x0015A5EA
	public static bool operator >(ZoneKey x, ZoneKey y)
	{
		return x.CompareTo(y) > 0;
	}

	// Token: 0x060045C9 RID: 17865 RVA: 0x0015C3F7 File Offset: 0x0015A5F7
	public static bool operator <=(ZoneKey x, ZoneKey y)
	{
		return x.CompareTo(y) <= 0;
	}

	// Token: 0x060045CA RID: 17866 RVA: 0x0015C407 File Offset: 0x0015A607
	public static bool operator >=(ZoneKey x, ZoneKey y)
	{
		return x.CompareTo(y) >= 0;
	}

	// Token: 0x060045CB RID: 17867 RVA: 0x0015C417 File Offset: 0x0015A617
	public static explicit operator int(ZoneKey key)
	{
		return key.intValue;
	}

	// Token: 0x040050C7 RID: 20679
	public GTZone zoneId;

	// Token: 0x040050C8 RID: 20680
	public GTSubZone subZoneId;

	// Token: 0x040050C9 RID: 20681
	public static readonly ZoneKey Null = new ZoneKey(GTZone.none, GTSubZone.none);
}
