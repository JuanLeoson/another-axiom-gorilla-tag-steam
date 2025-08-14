using System;
using UnityEngine;

// Token: 0x02000A42 RID: 2626
public abstract class RandomContainer<T> : ScriptableObject
{
	// Token: 0x1700060F RID: 1551
	// (get) Token: 0x06004051 RID: 16465 RVA: 0x0014646C File Offset: 0x0014466C
	public T lastItem
	{
		get
		{
			return this._lastItem;
		}
	}

	// Token: 0x17000610 RID: 1552
	// (get) Token: 0x06004052 RID: 16466 RVA: 0x00146474 File Offset: 0x00144674
	public int lastItemIndex
	{
		get
		{
			return this._lastItemIndex;
		}
	}

	// Token: 0x06004053 RID: 16467 RVA: 0x0014647C File Offset: 0x0014467C
	public void ResetRandom(int? seedValue = null)
	{
		if (!this.staticSeed)
		{
			this._seed = (seedValue ?? StaticHash.Compute(DateTime.UtcNow.Ticks));
		}
		else
		{
			this._seed = this.seed;
		}
		this._rnd = new SRand(this._seed);
	}

	// Token: 0x06004054 RID: 16468 RVA: 0x001464DC File Offset: 0x001446DC
	public void Reset()
	{
		this.ResetRandom(null);
		this._lastItem = default(T);
		this._lastItemIndex = -1;
	}

	// Token: 0x06004055 RID: 16469 RVA: 0x0014650B File Offset: 0x0014470B
	private void Awake()
	{
		this.Reset();
	}

	// Token: 0x06004056 RID: 16470 RVA: 0x00146513 File Offset: 0x00144713
	public virtual T GetItem(int index)
	{
		return this.items[index];
	}

	// Token: 0x06004057 RID: 16471 RVA: 0x00146524 File Offset: 0x00144724
	public virtual T NextItem()
	{
		this._lastItemIndex = (this.distinct ? this._rnd.NextIntWithExclusion(0, this.items.Length, this._lastItemIndex) : this._rnd.NextInt(0, this.items.Length));
		T t = this.items[this._lastItemIndex];
		this._lastItem = t;
		return t;
	}

	// Token: 0x04004C03 RID: 19459
	public T[] items = new T[0];

	// Token: 0x04004C04 RID: 19460
	public int seed;

	// Token: 0x04004C05 RID: 19461
	public bool staticSeed;

	// Token: 0x04004C06 RID: 19462
	public bool distinct = true;

	// Token: 0x04004C07 RID: 19463
	[Space]
	[NonSerialized]
	private int _seed;

	// Token: 0x04004C08 RID: 19464
	[NonSerialized]
	private T _lastItem;

	// Token: 0x04004C09 RID: 19465
	[NonSerialized]
	private int _lastItemIndex = -1;

	// Token: 0x04004C0A RID: 19466
	[NonSerialized]
	private SRand _rnd;
}
