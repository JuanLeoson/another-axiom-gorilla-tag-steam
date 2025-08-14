using System;
using GorillaTag;

// Token: 0x02000AC4 RID: 2756
public class LoopingArray<T> : ObjectPoolEvents
{
	// Token: 0x17000653 RID: 1619
	// (get) Token: 0x06004277 RID: 17015 RVA: 0x0014E26C File Offset: 0x0014C46C
	public int Length
	{
		get
		{
			return this.m_length;
		}
	}

	// Token: 0x17000654 RID: 1620
	// (get) Token: 0x06004278 RID: 17016 RVA: 0x0014E274 File Offset: 0x0014C474
	public int CurrentIndex
	{
		get
		{
			return this.m_currentIndex;
		}
	}

	// Token: 0x17000655 RID: 1621
	public T this[int index]
	{
		get
		{
			return this.m_array[index];
		}
		set
		{
			this.m_array[index] = value;
		}
	}

	// Token: 0x0600427B RID: 17019 RVA: 0x0014E299 File Offset: 0x0014C499
	public LoopingArray() : this(0)
	{
	}

	// Token: 0x0600427C RID: 17020 RVA: 0x0014E2A2 File Offset: 0x0014C4A2
	public LoopingArray(int capicity)
	{
		this.m_length = capicity;
		this.m_array = new T[capicity];
		this.Clear();
	}

	// Token: 0x0600427D RID: 17021 RVA: 0x0014E2C3 File Offset: 0x0014C4C3
	public int AddAndIncrement(in T value)
	{
		int currentIndex = this.m_currentIndex;
		this.m_array[this.m_currentIndex] = value;
		this.m_currentIndex = (this.m_currentIndex + 1) % this.m_length;
		return currentIndex;
	}

	// Token: 0x0600427E RID: 17022 RVA: 0x0014E2F7 File Offset: 0x0014C4F7
	public int IncrementAndAdd(in T value)
	{
		this.m_currentIndex = (this.m_currentIndex + 1) % this.m_length;
		this.m_array[this.m_currentIndex] = value;
		return this.m_currentIndex;
	}

	// Token: 0x0600427F RID: 17023 RVA: 0x0014E32C File Offset: 0x0014C52C
	public void Clear()
	{
		this.m_currentIndex = 0;
		for (int i = 0; i < this.m_array.Length; i++)
		{
			this.m_array[i] = default(T);
		}
	}

	// Token: 0x06004280 RID: 17024 RVA: 0x0014E368 File Offset: 0x0014C568
	void ObjectPoolEvents.OnTaken()
	{
		this.Clear();
	}

	// Token: 0x06004281 RID: 17025 RVA: 0x000023F5 File Offset: 0x000005F5
	void ObjectPoolEvents.OnReturned()
	{
	}

	// Token: 0x04004DAD RID: 19885
	private int m_length;

	// Token: 0x04004DAE RID: 19886
	private int m_currentIndex;

	// Token: 0x04004DAF RID: 19887
	private T[] m_array;

	// Token: 0x02000AC5 RID: 2757
	public class Pool : ObjectPool<LoopingArray<T>>
	{
		// Token: 0x06004282 RID: 17026 RVA: 0x0014E370 File Offset: 0x0014C570
		private Pool(int amount) : base(amount)
		{
		}

		// Token: 0x06004283 RID: 17027 RVA: 0x0014E379 File Offset: 0x0014C579
		public Pool(int size, int amount) : this(size, amount, amount)
		{
		}

		// Token: 0x06004284 RID: 17028 RVA: 0x0014E384 File Offset: 0x0014C584
		public Pool(int size, int initialAmount, int maxAmount)
		{
			this.m_size = size;
			base.InitializePool(initialAmount, maxAmount);
		}

		// Token: 0x06004285 RID: 17029 RVA: 0x0014E39B File Offset: 0x0014C59B
		public override LoopingArray<T> CreateInstance()
		{
			return new LoopingArray<T>(this.m_size);
		}

		// Token: 0x04004DB0 RID: 19888
		private readonly int m_size;
	}
}
