using System;

// Token: 0x02000AA5 RID: 2725
internal class CircularBuffer<T>
{
	// Token: 0x17000648 RID: 1608
	// (get) Token: 0x060041E9 RID: 16873 RVA: 0x0014C345 File Offset: 0x0014A545
	// (set) Token: 0x060041EA RID: 16874 RVA: 0x0014C34D File Offset: 0x0014A54D
	public int Count { get; private set; }

	// Token: 0x17000649 RID: 1609
	// (get) Token: 0x060041EB RID: 16875 RVA: 0x0014C356 File Offset: 0x0014A556
	// (set) Token: 0x060041EC RID: 16876 RVA: 0x0014C35E File Offset: 0x0014A55E
	public int Capacity { get; private set; }

	// Token: 0x060041ED RID: 16877 RVA: 0x0014C367 File Offset: 0x0014A567
	public CircularBuffer(int capacity)
	{
		this.backingArray = new T[capacity];
		this.Capacity = capacity;
		this.Count = 0;
	}

	// Token: 0x060041EE RID: 16878 RVA: 0x0014C38C File Offset: 0x0014A58C
	public void Add(T value)
	{
		this.backingArray[this.nextWriteIdx] = value;
		this.lastWriteIdx = this.nextWriteIdx;
		this.nextWriteIdx = (this.nextWriteIdx + 1) % this.Capacity;
		if (this.Count < this.Capacity)
		{
			int count = this.Count;
			this.Count = count + 1;
		}
	}

	// Token: 0x060041EF RID: 16879 RVA: 0x0014C3EA File Offset: 0x0014A5EA
	public void Clear()
	{
		this.Count = 0;
	}

	// Token: 0x060041F0 RID: 16880 RVA: 0x0014C3F3 File Offset: 0x0014A5F3
	public T Last()
	{
		return this.backingArray[this.lastWriteIdx];
	}

	// Token: 0x1700064A RID: 1610
	public T this[int logicalIdx]
	{
		get
		{
			if (logicalIdx < 0 || logicalIdx >= this.Count)
			{
				throw new ArgumentOutOfRangeException("logicalIdx", logicalIdx, string.Format("Out of bounds index {0} into CircularBuffer with length {1}", logicalIdx, this.Count));
			}
			int num = (this.lastWriteIdx + this.Capacity - logicalIdx) % this.Capacity;
			return this.backingArray[num];
		}
	}

	// Token: 0x04004D56 RID: 19798
	private T[] backingArray;

	// Token: 0x04004D59 RID: 19801
	private int nextWriteIdx;

	// Token: 0x04004D5A RID: 19802
	private int lastWriteIdx;
}
