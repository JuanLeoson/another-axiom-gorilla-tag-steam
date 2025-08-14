using System;

// Token: 0x02000749 RID: 1865
public struct HandLinkAuthorityStatus
{
	// Token: 0x06002EA6 RID: 11942 RVA: 0x000F6FA8 File Offset: 0x000F51A8
	public HandLinkAuthorityStatus(HandLinkAuthorityType authority)
	{
		this.type = authority;
		this.timestamp = -1f;
		this.tiebreak = -1;
	}

	// Token: 0x06002EA7 RID: 11943 RVA: 0x000F6FC3 File Offset: 0x000F51C3
	public HandLinkAuthorityStatus(HandLinkAuthorityType authority, float timestamp, int tiebreak)
	{
		this.type = authority;
		this.timestamp = timestamp;
		this.tiebreak = tiebreak;
	}

	// Token: 0x06002EA8 RID: 11944 RVA: 0x000F6FDC File Offset: 0x000F51DC
	public static bool operator >(HandLinkAuthorityStatus a, HandLinkAuthorityStatus b)
	{
		return a.type > b.type || (b.type <= a.type && (a.timestamp > b.timestamp || (b.timestamp <= a.timestamp && a.tiebreak > b.tiebreak)));
	}

	// Token: 0x06002EA9 RID: 11945 RVA: 0x000F7038 File Offset: 0x000F5238
	public static bool operator <(HandLinkAuthorityStatus a, HandLinkAuthorityStatus b)
	{
		return a.type < b.type || (b.type >= a.type && (a.timestamp < b.timestamp || (b.timestamp >= a.timestamp && a.tiebreak < b.tiebreak)));
	}

	// Token: 0x06002EAA RID: 11946 RVA: 0x000F7094 File Offset: 0x000F5294
	public int CompareTo(HandLinkAuthorityStatus b)
	{
		int num = this.type.CompareTo(b.type);
		if (num != 0)
		{
			return num;
		}
		int num2 = this.timestamp.CompareTo(b.timestamp);
		if (num2 != 0)
		{
			return num2;
		}
		return this.tiebreak.CompareTo(b.tiebreak);
	}

	// Token: 0x06002EAB RID: 11947 RVA: 0x000F70EB File Offset: 0x000F52EB
	public static bool operator ==(HandLinkAuthorityStatus a, HandLinkAuthorityStatus b)
	{
		return a.type == b.type && a.timestamp == b.timestamp && a.tiebreak == b.tiebreak;
	}

	// Token: 0x06002EAC RID: 11948 RVA: 0x000F7119 File Offset: 0x000F5319
	public static bool operator !=(HandLinkAuthorityStatus a, HandLinkAuthorityStatus b)
	{
		return a.timestamp != b.timestamp || a.tiebreak != b.tiebreak;
	}

	// Token: 0x06002EAD RID: 11949 RVA: 0x000F713C File Offset: 0x000F533C
	public override string ToString()
	{
		return string.Format("{0}/{1}", this.timestamp.ToString("0.0000"), this.tiebreak);
	}

	// Token: 0x04003AA1 RID: 15009
	public HandLinkAuthorityType type;

	// Token: 0x04003AA2 RID: 15010
	public float timestamp;

	// Token: 0x04003AA3 RID: 15011
	public int tiebreak;
}
