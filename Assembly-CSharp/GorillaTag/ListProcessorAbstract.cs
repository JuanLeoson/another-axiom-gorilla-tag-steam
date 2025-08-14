using System;

namespace GorillaTag
{
	// Token: 0x02000E94 RID: 3732
	public abstract class ListProcessorAbstract<T> : ListProcessor<T>
	{
		// Token: 0x06005D75 RID: 23925 RVA: 0x001D8045 File Offset: 0x001D6245
		protected ListProcessorAbstract()
		{
			this.m_itemProcessorDelegate = new InAction<T>(this.ProcessItem);
		}

		// Token: 0x06005D76 RID: 23926 RVA: 0x001D8060 File Offset: 0x001D6260
		protected ListProcessorAbstract(int capacity) : base(capacity, null)
		{
			this.m_itemProcessorDelegate = new InAction<T>(this.ProcessItem);
		}

		// Token: 0x06005D77 RID: 23927
		protected abstract void ProcessItem(in T item);
	}
}
