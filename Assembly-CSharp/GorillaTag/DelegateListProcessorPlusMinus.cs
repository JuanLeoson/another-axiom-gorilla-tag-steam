using System;

namespace GorillaTag
{
	// Token: 0x02000E8F RID: 3727
	public abstract class DelegateListProcessorPlusMinus<T1, T2> : ListProcessorAbstract<T2> where T1 : DelegateListProcessorPlusMinus<T1, T2>, new() where T2 : Delegate
	{
		// Token: 0x06005D59 RID: 23897 RVA: 0x001D7D02 File Offset: 0x001D5F02
		protected DelegateListProcessorPlusMinus()
		{
		}

		// Token: 0x06005D5A RID: 23898 RVA: 0x001D7D0A File Offset: 0x001D5F0A
		protected DelegateListProcessorPlusMinus(int capacity) : base(capacity)
		{
		}

		// Token: 0x06005D5B RID: 23899 RVA: 0x001D7D13 File Offset: 0x001D5F13
		public static T1 operator +(DelegateListProcessorPlusMinus<T1, T2> left, T2 right)
		{
			if (left == null)
			{
				left = Activator.CreateInstance<T1>();
			}
			if (right == null)
			{
				return (T1)((object)left);
			}
			left.Add(right);
			return (T1)((object)left);
		}

		// Token: 0x06005D5C RID: 23900 RVA: 0x001D7D44 File Offset: 0x001D5F44
		public static T1 operator -(DelegateListProcessorPlusMinus<T1, T2> left, T2 right)
		{
			if (left == null)
			{
				return default(T1);
			}
			if (right == null)
			{
				return (T1)((object)left);
			}
			left.Remove(right);
			return (T1)((object)left);
		}
	}
}
