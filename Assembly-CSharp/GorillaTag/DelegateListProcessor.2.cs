using System;

namespace GorillaTag
{
	// Token: 0x02000E91 RID: 3729
	public class DelegateListProcessor<T> : DelegateListProcessorPlusMinus<DelegateListProcessor<T>, Action<T>>
	{
		// Token: 0x06005D62 RID: 23906 RVA: 0x001D7DA5 File Offset: 0x001D5FA5
		public DelegateListProcessor()
		{
		}

		// Token: 0x06005D63 RID: 23907 RVA: 0x001D7DAD File Offset: 0x001D5FAD
		public DelegateListProcessor(int capacity) : base(capacity)
		{
		}

		// Token: 0x06005D64 RID: 23908 RVA: 0x001D7DB6 File Offset: 0x001D5FB6
		public void InvokeSafe(in T data)
		{
			this.m_data = data;
			this.ProcessListSafe();
			this.m_data = default(T);
		}

		// Token: 0x06005D65 RID: 23909 RVA: 0x001D7DD6 File Offset: 0x001D5FD6
		public void Invoke(in T data)
		{
			this.m_data = data;
			this.ProcessList();
			this.m_data = default(T);
		}

		// Token: 0x06005D66 RID: 23910 RVA: 0x001D7DF6 File Offset: 0x001D5FF6
		protected override void ProcessItem(in Action<T> item)
		{
			item(this.m_data);
		}

		// Token: 0x0400675C RID: 26460
		private T m_data;
	}
}
