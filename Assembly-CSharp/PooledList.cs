using System;
using System.Collections.Generic;
using GorillaTag;

// Token: 0x02000B04 RID: 2820
public class PooledList<T> : ObjectPoolEvents
{
	// Token: 0x060043DF RID: 17375 RVA: 0x000023F5 File Offset: 0x000005F5
	void ObjectPoolEvents.OnTaken()
	{
	}

	// Token: 0x060043E0 RID: 17376 RVA: 0x00154C67 File Offset: 0x00152E67
	void ObjectPoolEvents.OnReturned()
	{
		this.List.Clear();
	}

	// Token: 0x04004E56 RID: 20054
	public List<T> List = new List<T>();
}
