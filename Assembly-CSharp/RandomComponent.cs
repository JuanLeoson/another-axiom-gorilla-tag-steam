using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000A41 RID: 2625
public abstract class RandomComponent<T> : MonoBehaviour
{
	// Token: 0x1700060D RID: 1549
	// (get) Token: 0x06004048 RID: 16456 RVA: 0x00146311 File Offset: 0x00144511
	public T lastItem
	{
		get
		{
			return this._lastItem;
		}
	}

	// Token: 0x1700060E RID: 1550
	// (get) Token: 0x06004049 RID: 16457 RVA: 0x00146319 File Offset: 0x00144519
	public int lastItemIndex
	{
		get
		{
			return this._lastItemIndex;
		}
	}

	// Token: 0x0600404A RID: 16458 RVA: 0x00146324 File Offset: 0x00144524
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

	// Token: 0x0600404B RID: 16459 RVA: 0x00146384 File Offset: 0x00144584
	public void Reset()
	{
		this.ResetRandom(null);
		this._lastItem = default(T);
		this._lastItemIndex = -1;
	}

	// Token: 0x0600404C RID: 16460 RVA: 0x001463B3 File Offset: 0x001445B3
	private void Awake()
	{
		this.Reset();
	}

	// Token: 0x0600404D RID: 16461 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void OnNextItem(T item)
	{
	}

	// Token: 0x0600404E RID: 16462 RVA: 0x001463BB File Offset: 0x001445BB
	public virtual T GetItem(int index)
	{
		return this.items[index];
	}

	// Token: 0x0600404F RID: 16463 RVA: 0x001463CC File Offset: 0x001445CC
	public virtual T NextItem()
	{
		this._lastItemIndex = (this.distinct ? this._rnd.NextIntWithExclusion(0, this.items.Length, this._lastItemIndex) : this._rnd.NextInt(0, this.items.Length));
		T t = this.items[this._lastItemIndex];
		this._lastItem = t;
		this.OnNextItem(t);
		UnityEvent<T> unityEvent = this.onNextItem;
		if (unityEvent != null)
		{
			unityEvent.Invoke(t);
		}
		return t;
	}

	// Token: 0x04004BFA RID: 19450
	public T[] items = new T[0];

	// Token: 0x04004BFB RID: 19451
	public int seed;

	// Token: 0x04004BFC RID: 19452
	public bool staticSeed;

	// Token: 0x04004BFD RID: 19453
	public bool distinct = true;

	// Token: 0x04004BFE RID: 19454
	[Space]
	[NonSerialized]
	private int _seed;

	// Token: 0x04004BFF RID: 19455
	[NonSerialized]
	private T _lastItem;

	// Token: 0x04004C00 RID: 19456
	[NonSerialized]
	private int _lastItemIndex = -1;

	// Token: 0x04004C01 RID: 19457
	[NonSerialized]
	private SRand _rnd;

	// Token: 0x04004C02 RID: 19458
	public UnityEvent<T> onNextItem;
}
