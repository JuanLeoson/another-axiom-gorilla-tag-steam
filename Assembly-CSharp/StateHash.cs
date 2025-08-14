using System;

// Token: 0x0200088E RID: 2190
[Serializable]
public struct StateHash
{
	// Token: 0x0600370D RID: 14093 RVA: 0x0011EB6D File Offset: 0x0011CD6D
	public override int GetHashCode()
	{
		return HashCode.Combine<int, int>(this.last, this.next);
	}

	// Token: 0x0600370E RID: 14094 RVA: 0x0011EB80 File Offset: 0x0011CD80
	public override string ToString()
	{
		return this.GetHashCode().ToString();
	}

	// Token: 0x0600370F RID: 14095 RVA: 0x0011EBA1 File Offset: 0x0011CDA1
	public bool Changed()
	{
		if (this.last == this.next)
		{
			return false;
		}
		this.last = this.next;
		return true;
	}

	// Token: 0x06003710 RID: 14096 RVA: 0x0011EBC0 File Offset: 0x0011CDC0
	public void Poll<T0>(T0 v0)
	{
		this.last = this.next;
		this.next = HashCode.Combine<T0>(v0);
	}

	// Token: 0x06003711 RID: 14097 RVA: 0x0011EBDA File Offset: 0x0011CDDA
	public void Poll<T1, T2>(T1 v1, T2 v2)
	{
		this.last = this.next;
		this.next = HashCode.Combine<T1, T2>(v1, v2);
	}

	// Token: 0x06003712 RID: 14098 RVA: 0x0011EBF5 File Offset: 0x0011CDF5
	public void Poll<T1, T2, T3>(T1 v1, T2 v2, T3 v3)
	{
		this.last = this.next;
		this.next = HashCode.Combine<T1, T2, T3>(v1, v2, v3);
	}

	// Token: 0x06003713 RID: 14099 RVA: 0x0011EC11 File Offset: 0x0011CE11
	public void Poll<T1, T2, T3, T4>(T1 v1, T2 v2, T3 v3, T4 v4)
	{
		this.last = this.next;
		this.next = HashCode.Combine<T1, T2, T3, T4>(v1, v2, v3, v4);
	}

	// Token: 0x06003714 RID: 14100 RVA: 0x0011EC2F File Offset: 0x0011CE2F
	public void Poll<T1, T2, T3, T4, T5>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5)
	{
		this.last = this.next;
		this.next = HashCode.Combine<T1, T2, T3, T4, T5>(v1, v2, v3, v4, v5);
	}

	// Token: 0x06003715 RID: 14101 RVA: 0x0011EC4F File Offset: 0x0011CE4F
	public void Poll<T1, T2, T3, T4, T5, T6>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6)
	{
		this.last = this.next;
		this.next = HashCode.Combine<T1, T2, T3, T4, T5, T6>(v1, v2, v3, v4, v5, v6);
	}

	// Token: 0x06003716 RID: 14102 RVA: 0x0011EC71 File Offset: 0x0011CE71
	public void Poll<T1, T2, T3, T4, T5, T6, T7>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7)
	{
		this.last = this.next;
		this.next = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7>(v1, v2, v3, v4, v5, v6, v7);
	}

	// Token: 0x06003717 RID: 14103 RVA: 0x0011EC98 File Offset: 0x0011CE98
	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8)
	{
		this.last = this.next;
		this.next = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
	}

	// Token: 0x06003718 RID: 14104 RVA: 0x0011ECCC File Offset: 0x0011CECC
	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9)
	{
		this.last = this.next;
		int value = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
		this.next = HashCode.Combine<int, T9>(value, v9);
	}

	// Token: 0x06003719 RID: 14105 RVA: 0x0011ED08 File Offset: 0x0011CF08
	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9, T10 v10)
	{
		this.last = this.next;
		int value = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
		this.next = HashCode.Combine<int, T9, T10>(value, v9, v10);
	}

	// Token: 0x0600371A RID: 14106 RVA: 0x0011ED44 File Offset: 0x0011CF44
	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9, T10 v10, T11 v11)
	{
		this.last = this.next;
		int value = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
		this.next = HashCode.Combine<int, T9, T10, T11>(value, v9, v10, v11);
	}

	// Token: 0x0600371B RID: 14107 RVA: 0x0011ED84 File Offset: 0x0011CF84
	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9, T10 v10, T11 v11, T12 v12)
	{
		this.last = this.next;
		int value = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
		this.next = HashCode.Combine<int, T9, T10, T11, T12>(value, v9, v10, v11, v12);
	}

	// Token: 0x0600371C RID: 14108 RVA: 0x0011EDC4 File Offset: 0x0011CFC4
	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9, T10 v10, T11 v11, T12 v12, T13 v13)
	{
		this.last = this.next;
		int value = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
		this.next = HashCode.Combine<int, T9, T10, T11, T12, T13>(value, v9, v10, v11, v12, v13);
	}

	// Token: 0x0600371D RID: 14109 RVA: 0x0011EE08 File Offset: 0x0011D008
	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9, T10 v10, T11 v11, T12 v12, T13 v13, T14 v14)
	{
		this.last = this.next;
		int value = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
		this.next = HashCode.Combine<int, T9, T10, T11, T12, T13, T14>(value, v9, v10, v11, v12, v13, v14);
	}

	// Token: 0x0600371E RID: 14110 RVA: 0x0011EE4C File Offset: 0x0011D04C
	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9, T10 v10, T11 v11, T12 v12, T13 v13, T14 v14, T15 v15)
	{
		this.last = this.next;
		int value = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
		this.next = HashCode.Combine<int, T9, T10, T11, T12, T13, T14, T15>(value, v9, v10, v11, v12, v13, v14, v15);
	}

	// Token: 0x0600371F RID: 14111 RVA: 0x0011EE94 File Offset: 0x0011D094
	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9, T10 v10, T11 v11, T12 v12, T13 v13, T14 v14, T15 v15, T16 v16)
	{
		this.last = this.next;
		int value = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
		int value2 = HashCode.Combine<int, T9, T10, T11, T12, T13, T14, T15>(value, v9, v10, v11, v12, v13, v14, v15);
		this.next = HashCode.Combine<int, int, T16>(value, value2, v16);
	}

	// Token: 0x040043C7 RID: 17351
	public int last;

	// Token: 0x040043C8 RID: 17352
	public int next;
}
