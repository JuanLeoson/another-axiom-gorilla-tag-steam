using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GorillaTag
{
	// Token: 0x02000E96 RID: 3734
	public class ObjectPool<T> where T : ObjectPoolEvents, new()
	{
		// Token: 0x06005D7A RID: 23930 RVA: 0x001D807D File Offset: 0x001D627D
		protected ObjectPool()
		{
		}

		// Token: 0x06005D7B RID: 23931 RVA: 0x001D8090 File Offset: 0x001D6290
		public ObjectPool(int amount) : this(amount, amount)
		{
		}

		// Token: 0x06005D7C RID: 23932 RVA: 0x001D809A File Offset: 0x001D629A
		public ObjectPool(int initialAmount, int maxAmount)
		{
			this.InitializePool(initialAmount, maxAmount);
		}

		// Token: 0x06005D7D RID: 23933 RVA: 0x001D80B8 File Offset: 0x001D62B8
		protected void InitializePool(int initialAmount, int maxAmount)
		{
			this.maxInstances = maxAmount;
			this.pool = new Stack<T>(initialAmount);
			for (int i = 0; i < initialAmount; i++)
			{
				this.pool.Push(this.CreateInstance());
			}
		}

		// Token: 0x06005D7E RID: 23934 RVA: 0x001D80F8 File Offset: 0x001D62F8
		public T Take()
		{
			T result;
			if (this.pool.Count < 1)
			{
				result = this.CreateInstance();
			}
			else
			{
				result = this.pool.Pop();
			}
			result.OnTaken();
			return result;
		}

		// Token: 0x06005D7F RID: 23935 RVA: 0x001D8136 File Offset: 0x001D6336
		public void Return(T instance)
		{
			instance.OnReturned();
			if (this.pool.Count == this.maxInstances)
			{
				return;
			}
			this.pool.Push(instance);
		}

		// Token: 0x06005D80 RID: 23936 RVA: 0x001D8165 File Offset: 0x001D6365
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual T CreateInstance()
		{
			return Activator.CreateInstance<T>();
		}

		// Token: 0x04006762 RID: 26466
		private Stack<T> pool;

		// Token: 0x04006763 RID: 26467
		public int maxInstances = 500;
	}
}
