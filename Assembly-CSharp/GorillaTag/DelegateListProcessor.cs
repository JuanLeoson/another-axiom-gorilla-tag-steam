using System;

namespace GorillaTag
{
	// Token: 0x02000E90 RID: 3728
	public class DelegateListProcessor : DelegateListProcessorPlusMinus<DelegateListProcessor, Action>
	{
		// Token: 0x06005D5D RID: 23901 RVA: 0x001D7D7B File Offset: 0x001D5F7B
		public DelegateListProcessor()
		{
		}

		// Token: 0x06005D5E RID: 23902 RVA: 0x001D7D83 File Offset: 0x001D5F83
		public DelegateListProcessor(int capacity) : base(capacity)
		{
		}

		// Token: 0x06005D5F RID: 23903 RVA: 0x001D7D8C File Offset: 0x001D5F8C
		public void Invoke()
		{
			this.ProcessList();
		}

		// Token: 0x06005D60 RID: 23904 RVA: 0x001D7D94 File Offset: 0x001D5F94
		public void InvokeSafe()
		{
			this.ProcessListSafe();
		}

		// Token: 0x06005D61 RID: 23905 RVA: 0x001D7D9C File Offset: 0x001D5F9C
		protected override void ProcessItem(in Action del)
		{
			del();
		}
	}
}
