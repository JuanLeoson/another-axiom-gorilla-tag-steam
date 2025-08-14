using System;

// Token: 0x0200022F RID: 559
[Serializable]
public struct GTSerializableKeyValue<T1, T2>
{
	// Token: 0x06000D2B RID: 3371 RVA: 0x0004639D File Offset: 0x0004459D
	public GTSerializableKeyValue(T1 k, T2 v)
	{
		this.k = k;
		this.v = v;
	}

	// Token: 0x0400100E RID: 4110
	public T1 k;

	// Token: 0x0400100F RID: 4111
	public T2 v;
}
