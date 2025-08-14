using System;
using GorillaTag;

// Token: 0x02000AA4 RID: 2724
internal class CallbackContainer<T> : ListProcessorAbstract<T> where T : ICallBack
{
	// Token: 0x060041E5 RID: 16869 RVA: 0x0014C309 File Offset: 0x0014A509
	public CallbackContainer() : base(100)
	{
	}

	// Token: 0x060041E6 RID: 16870 RVA: 0x0014C313 File Offset: 0x0014A513
	public CallbackContainer(int capacity) : base(capacity)
	{
	}

	// Token: 0x060041E7 RID: 16871 RVA: 0x0014C31C File Offset: 0x0014A51C
	public void TryRunCallbacks()
	{
		this.ProcessListSafe();
	}

	// Token: 0x060041E8 RID: 16872 RVA: 0x0014C324 File Offset: 0x0014A524
	protected override void ProcessItem(in T item)
	{
		T t = item;
		t.CallBack();
	}
}
