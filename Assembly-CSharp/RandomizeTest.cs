using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007A5 RID: 1957
public class RandomizeTest : MonoBehaviour
{
	// Token: 0x06003131 RID: 12593 RVA: 0x00100768 File Offset: 0x000FE968
	private void Start()
	{
		for (int i = 0; i < 10; i++)
		{
			this.testList.Add(i);
		}
		for (int j = 0; j < 10; j++)
		{
			this.testListArray[j] = 0;
		}
		for (int k = 0; k < this.testList.Count; k++)
		{
			this.testListArray[k] = this.testList[k];
		}
		this.RandomizeList(ref this.testList);
		for (int l = 0; l < 10; l++)
		{
			this.testListArray[l] = 0;
		}
		for (int m = 0; m < this.testList.Count; m++)
		{
			this.testListArray[m] = this.testList[m];
		}
	}

	// Token: 0x06003132 RID: 12594 RVA: 0x00100820 File Offset: 0x000FEA20
	public void RandomizeList(ref List<int> listToRandomize)
	{
		this.randomIterator = 0;
		while (this.randomIterator < listToRandomize.Count)
		{
			this.tempRandIndex = Random.Range(this.randomIterator, listToRandomize.Count);
			this.tempRandValue = listToRandomize[this.randomIterator];
			listToRandomize[this.randomIterator] = listToRandomize[this.tempRandIndex];
			listToRandomize[this.tempRandIndex] = this.tempRandValue;
			this.randomIterator++;
		}
	}

	// Token: 0x04003CD6 RID: 15574
	public List<int> testList = new List<int>();

	// Token: 0x04003CD7 RID: 15575
	public int[] testListArray = new int[10];

	// Token: 0x04003CD8 RID: 15576
	public int randomIterator;

	// Token: 0x04003CD9 RID: 15577
	public int tempRandIndex;

	// Token: 0x04003CDA RID: 15578
	public int tempRandValue;
}
