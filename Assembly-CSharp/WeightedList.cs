using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000087 RID: 135
public class WeightedList<T>
{
	// Token: 0x17000038 RID: 56
	// (get) Token: 0x06000351 RID: 849 RVA: 0x00013E16 File Offset: 0x00012016
	public int Count
	{
		get
		{
			return this.items.Count;
		}
	}

	// Token: 0x17000039 RID: 57
	// (get) Token: 0x06000352 RID: 850 RVA: 0x00013E23 File Offset: 0x00012023
	public List<T> Items
	{
		get
		{
			return this.items;
		}
	}

	// Token: 0x06000353 RID: 851 RVA: 0x00013E2C File Offset: 0x0001202C
	public void Add(T item, float weight)
	{
		if (weight <= 0f)
		{
			throw new ArgumentException("Weight must be greater than zero.");
		}
		this.totalWeight += weight;
		this.items.Add(item);
		this.weights.Add(weight);
		this.cumulativeWeights.Add(this.totalWeight);
	}

	// Token: 0x1700003A RID: 58
	[TupleElementNames(new string[]
	{
		"Item",
		"Weight"
	})]
	public ValueTuple<T, float> this[int index]
	{
		[return: TupleElementNames(new string[]
		{
			"Item",
			"Weight"
		})]
		get
		{
			if (index < 0 || index >= this.items.Count)
			{
				throw new IndexOutOfRangeException();
			}
			return new ValueTuple<T, float>(this.items[index], this.weights[index]);
		}
	}

	// Token: 0x06000355 RID: 853 RVA: 0x00013EBA File Offset: 0x000120BA
	public T GetRandomItem()
	{
		return this.items[this.GetRandomIndex()];
	}

	// Token: 0x06000356 RID: 854 RVA: 0x00013ED0 File Offset: 0x000120D0
	public int GetRandomIndex()
	{
		if (this.items.Count == 0)
		{
			throw new InvalidOperationException("The list is empty.");
		}
		float item = Random.value * this.totalWeight;
		int num = this.cumulativeWeights.BinarySearch(item);
		if (num < 0)
		{
			num = ~num;
		}
		return num;
	}

	// Token: 0x06000357 RID: 855 RVA: 0x00013F18 File Offset: 0x00012118
	public bool Remove(T item)
	{
		int num = this.items.IndexOf(item);
		if (num == -1)
		{
			return false;
		}
		this.RemoveAt(num);
		return true;
	}

	// Token: 0x06000358 RID: 856 RVA: 0x00013F40 File Offset: 0x00012140
	public void RemoveAt(int index)
	{
		if (index < 0 || index >= this.items.Count)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		this.totalWeight -= this.weights[index];
		this.items.RemoveAt(index);
		this.weights.RemoveAt(index);
		this.RecalculateCumulativeWeights();
	}

	// Token: 0x06000359 RID: 857 RVA: 0x00013FA4 File Offset: 0x000121A4
	private void RecalculateCumulativeWeights()
	{
		this.cumulativeWeights.Clear();
		float num = 0f;
		foreach (float num2 in this.weights)
		{
			num += num2;
			this.cumulativeWeights.Add(num);
		}
		this.totalWeight = num;
	}

	// Token: 0x0600035A RID: 858 RVA: 0x00014018 File Offset: 0x00012218
	public void Clear()
	{
		this.items.Clear();
		this.weights.Clear();
		this.cumulativeWeights.Clear();
		this.totalWeight = 0f;
	}

	// Token: 0x040003EC RID: 1004
	private List<T> items = new List<T>();

	// Token: 0x040003ED RID: 1005
	private List<float> weights = new List<float>();

	// Token: 0x040003EE RID: 1006
	private List<float> cumulativeWeights = new List<float>();

	// Token: 0x040003EF RID: 1007
	private float totalWeight;
}
