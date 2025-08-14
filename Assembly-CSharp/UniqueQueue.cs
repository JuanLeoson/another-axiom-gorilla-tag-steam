using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x02000AFC RID: 2812
public class UniqueQueue<T> : IEnumerable<!0>, IEnumerable
{
	// Token: 0x17000661 RID: 1633
	// (get) Token: 0x060043BC RID: 17340 RVA: 0x0015479F File Offset: 0x0015299F
	public int Count
	{
		get
		{
			return this.queue.Count;
		}
	}

	// Token: 0x060043BD RID: 17341 RVA: 0x001547AC File Offset: 0x001529AC
	public UniqueQueue()
	{
		this.queuedItems = new HashSet<T>();
		this.queue = new Queue<T>();
	}

	// Token: 0x060043BE RID: 17342 RVA: 0x001547CA File Offset: 0x001529CA
	public UniqueQueue(int capacity)
	{
		this.queuedItems = new HashSet<T>(capacity);
		this.queue = new Queue<T>(capacity);
	}

	// Token: 0x060043BF RID: 17343 RVA: 0x001547EA File Offset: 0x001529EA
	public void Clear()
	{
		this.queuedItems.Clear();
		this.queue.Clear();
	}

	// Token: 0x060043C0 RID: 17344 RVA: 0x00154802 File Offset: 0x00152A02
	public bool Enqueue(T item)
	{
		if (!this.queuedItems.Add(item))
		{
			return false;
		}
		this.queue.Enqueue(item);
		return true;
	}

	// Token: 0x060043C1 RID: 17345 RVA: 0x00154824 File Offset: 0x00152A24
	public T Dequeue()
	{
		T t = this.queue.Dequeue();
		this.queuedItems.Remove(t);
		return t;
	}

	// Token: 0x060043C2 RID: 17346 RVA: 0x0015484B File Offset: 0x00152A4B
	public bool TryDequeue(out T item)
	{
		if (this.queue.Count < 1)
		{
			item = default(T);
			return false;
		}
		item = this.Dequeue();
		return true;
	}

	// Token: 0x060043C3 RID: 17347 RVA: 0x00154871 File Offset: 0x00152A71
	public T Peek()
	{
		return this.queue.Peek();
	}

	// Token: 0x060043C4 RID: 17348 RVA: 0x0015487E File Offset: 0x00152A7E
	public bool Contains(T item)
	{
		return this.queuedItems.Contains(item);
	}

	// Token: 0x060043C5 RID: 17349 RVA: 0x0015488C File Offset: 0x00152A8C
	IEnumerator<T> IEnumerable<!0>.GetEnumerator()
	{
		return this.queue.GetEnumerator();
	}

	// Token: 0x060043C6 RID: 17350 RVA: 0x0015488C File Offset: 0x00152A8C
	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.queue.GetEnumerator();
	}

	// Token: 0x04004E43 RID: 20035
	private HashSet<T> queuedItems;

	// Token: 0x04004E44 RID: 20036
	private Queue<T> queue;
}
