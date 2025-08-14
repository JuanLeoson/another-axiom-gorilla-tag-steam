using System;
using System.Collections.Generic;

// Token: 0x0200088C RID: 2188
public class RingBuffer<T>
{
	// Token: 0x1700053B RID: 1339
	// (get) Token: 0x060036D2 RID: 14034 RVA: 0x0011E3C6 File Offset: 0x0011C5C6
	public int Size
	{
		get
		{
			return this._size;
		}
	}

	// Token: 0x1700053C RID: 1340
	// (get) Token: 0x060036D3 RID: 14035 RVA: 0x0011E3CE File Offset: 0x0011C5CE
	public int Capacity
	{
		get
		{
			return this._capacity;
		}
	}

	// Token: 0x1700053D RID: 1341
	// (get) Token: 0x060036D4 RID: 14036 RVA: 0x0011E3D6 File Offset: 0x0011C5D6
	public bool IsFull
	{
		get
		{
			return this._size == this._capacity;
		}
	}

	// Token: 0x1700053E RID: 1342
	// (get) Token: 0x060036D5 RID: 14037 RVA: 0x0011E3E6 File Offset: 0x0011C5E6
	public bool IsEmpty
	{
		get
		{
			return this._size == 0;
		}
	}

	// Token: 0x060036D6 RID: 14038 RVA: 0x0011E3F1 File Offset: 0x0011C5F1
	public RingBuffer(int capacity)
	{
		if (capacity < 1)
		{
			throw new ArgumentException("Can't be zero or negative", "capacity");
		}
		this._size = 0;
		this._capacity = capacity;
		this._items = new T[capacity];
	}

	// Token: 0x060036D7 RID: 14039 RVA: 0x0011E427 File Offset: 0x0011C627
	public RingBuffer(IList<T> list) : this(list.Count)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		list.CopyTo(this._items, 0);
	}

	// Token: 0x060036D8 RID: 14040 RVA: 0x0011E450 File Offset: 0x0011C650
	public ref T PeekFirst()
	{
		return ref this._items[this._head];
	}

	// Token: 0x060036D9 RID: 14041 RVA: 0x0011E463 File Offset: 0x0011C663
	public ref T PeekLast()
	{
		return ref this._items[this._tail];
	}

	// Token: 0x060036DA RID: 14042 RVA: 0x0011E478 File Offset: 0x0011C678
	public bool Push(T item)
	{
		if (this._size == this._capacity)
		{
			return false;
		}
		this._items[this._tail] = item;
		this._tail = (this._tail + 1) % this._capacity;
		this._size++;
		return true;
	}

	// Token: 0x060036DB RID: 14043 RVA: 0x0011E4CC File Offset: 0x0011C6CC
	public T Pop()
	{
		if (this._size == 0)
		{
			return default(T);
		}
		T result = this._items[this._head];
		this._head = (this._head + 1) % this._capacity;
		this._size--;
		return result;
	}

	// Token: 0x060036DC RID: 14044 RVA: 0x0011E520 File Offset: 0x0011C720
	public bool TryPop(out T item)
	{
		if (this._size == 0)
		{
			item = default(T);
			return false;
		}
		item = this._items[this._head];
		this._head = (this._head + 1) % this._capacity;
		this._size--;
		return true;
	}

	// Token: 0x060036DD RID: 14045 RVA: 0x0011E579 File Offset: 0x0011C779
	public void Clear()
	{
		this._head = 0;
		this._tail = 0;
		this._size = 0;
		Array.Clear(this._items, 0, this._capacity);
	}

	// Token: 0x060036DE RID: 14046 RVA: 0x0011E5A2 File Offset: 0x0011C7A2
	public bool TryGet(int i, out T item)
	{
		if (this._size == 0)
		{
			item = default(T);
			return false;
		}
		item = this._items[this._head + i % this._size];
		return true;
	}

	// Token: 0x060036DF RID: 14047 RVA: 0x0011E5D6 File Offset: 0x0011C7D6
	public ArraySegment<T> AsSegment()
	{
		return new ArraySegment<T>(this._items);
	}

	// Token: 0x040043BC RID: 17340
	private T[] _items;

	// Token: 0x040043BD RID: 17341
	private int _head;

	// Token: 0x040043BE RID: 17342
	private int _tail;

	// Token: 0x040043BF RID: 17343
	private int _size;

	// Token: 0x040043C0 RID: 17344
	private readonly int _capacity;
}
