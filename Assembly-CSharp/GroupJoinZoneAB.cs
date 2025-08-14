using System;

// Token: 0x0200059C RID: 1436
[Serializable]
public struct GroupJoinZoneAB
{
	// Token: 0x060022F4 RID: 8948 RVA: 0x000BC9A0 File Offset: 0x000BABA0
	public static GroupJoinZoneAB operator &(GroupJoinZoneAB one, GroupJoinZoneAB two)
	{
		return new GroupJoinZoneAB
		{
			a = (one.a & two.a),
			b = (one.b & two.b)
		};
	}

	// Token: 0x060022F5 RID: 8949 RVA: 0x000BC9E0 File Offset: 0x000BABE0
	public static GroupJoinZoneAB operator |(GroupJoinZoneAB one, GroupJoinZoneAB two)
	{
		return new GroupJoinZoneAB
		{
			a = (one.a | two.a),
			b = (one.b | two.b)
		};
	}

	// Token: 0x060022F6 RID: 8950 RVA: 0x000BCA20 File Offset: 0x000BAC20
	public static GroupJoinZoneAB operator ~(GroupJoinZoneAB z)
	{
		return new GroupJoinZoneAB
		{
			a = ~z.a,
			b = ~z.b
		};
	}

	// Token: 0x060022F7 RID: 8951 RVA: 0x000BCA52 File Offset: 0x000BAC52
	public static bool operator ==(GroupJoinZoneAB one, GroupJoinZoneAB two)
	{
		return one.a == two.a && one.b == two.b;
	}

	// Token: 0x060022F8 RID: 8952 RVA: 0x000BCA72 File Offset: 0x000BAC72
	public static bool operator !=(GroupJoinZoneAB one, GroupJoinZoneAB two)
	{
		return one.a != two.a || one.b != two.b;
	}

	// Token: 0x060022F9 RID: 8953 RVA: 0x000BCA95 File Offset: 0x000BAC95
	public override bool Equals(object other)
	{
		return this == (GroupJoinZoneAB)other;
	}

	// Token: 0x060022FA RID: 8954 RVA: 0x000BCAA8 File Offset: 0x000BACA8
	public override int GetHashCode()
	{
		return this.a.GetHashCode() ^ this.b.GetHashCode();
	}

	// Token: 0x060022FB RID: 8955 RVA: 0x000BCAD0 File Offset: 0x000BACD0
	public static implicit operator GroupJoinZoneAB(int d)
	{
		return new GroupJoinZoneAB
		{
			a = (GroupJoinZoneA)d
		};
	}

	// Token: 0x060022FC RID: 8956 RVA: 0x000BCAF0 File Offset: 0x000BACF0
	public override string ToString()
	{
		if (this.b == (GroupJoinZoneB)0)
		{
			return this.a.ToString();
		}
		if (this.a != (GroupJoinZoneA)0)
		{
			return this.a.ToString() + "," + this.b.ToString();
		}
		return this.b.ToString();
	}

	// Token: 0x04002CC7 RID: 11463
	public GroupJoinZoneA a;

	// Token: 0x04002CC8 RID: 11464
	public GroupJoinZoneB b;
}
