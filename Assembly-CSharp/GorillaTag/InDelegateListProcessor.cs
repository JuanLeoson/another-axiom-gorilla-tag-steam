using System;

namespace GorillaTag
{
	// Token: 0x02000E92 RID: 3730
	public class InDelegateListProcessor<T> : DelegateListProcessorPlusMinus<InDelegateListProcessor<T>, InAction<T>>
	{
		// Token: 0x06005D67 RID: 23911 RVA: 0x001D7E05 File Offset: 0x001D6005
		public InDelegateListProcessor()
		{
		}

		// Token: 0x06005D68 RID: 23912 RVA: 0x001D7E0D File Offset: 0x001D600D
		public InDelegateListProcessor(int capacity) : base(capacity)
		{
		}

		// Token: 0x06005D69 RID: 23913 RVA: 0x001D7E16 File Offset: 0x001D6016
		public void InvokeSafe(in T data)
		{
			this.m_data = data;
			this.ProcessListSafe();
			this.m_data = default(T);
		}

		// Token: 0x06005D6A RID: 23914 RVA: 0x001D7E36 File Offset: 0x001D6036
		public void Invoke(in T data)
		{
			this.m_data = data;
			this.ProcessList();
			this.m_data = default(T);
		}

		// Token: 0x06005D6B RID: 23915 RVA: 0x001D7E56 File Offset: 0x001D6056
		protected override void ProcessItem(in InAction<T> item)
		{
			item(this.m_data);
		}

		// Token: 0x0400675D RID: 26461
		private T m_data;
	}
}
