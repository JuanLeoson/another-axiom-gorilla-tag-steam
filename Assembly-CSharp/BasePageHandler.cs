using System;
using UnityEngine;

// Token: 0x02000A97 RID: 2711
public abstract class BasePageHandler : MonoBehaviour
{
	// Token: 0x17000642 RID: 1602
	// (get) Token: 0x060041BC RID: 16828 RVA: 0x0014B598 File Offset: 0x00149798
	// (set) Token: 0x060041BD RID: 16829 RVA: 0x0014B5A0 File Offset: 0x001497A0
	private protected int selectedIndex { protected get; private set; }

	// Token: 0x17000643 RID: 1603
	// (get) Token: 0x060041BE RID: 16830 RVA: 0x0014B5A9 File Offset: 0x001497A9
	// (set) Token: 0x060041BF RID: 16831 RVA: 0x0014B5B1 File Offset: 0x001497B1
	private protected int currentPage { protected get; private set; }

	// Token: 0x17000644 RID: 1604
	// (get) Token: 0x060041C0 RID: 16832 RVA: 0x0014B5BA File Offset: 0x001497BA
	// (set) Token: 0x060041C1 RID: 16833 RVA: 0x0014B5C2 File Offset: 0x001497C2
	private protected int pages { protected get; private set; }

	// Token: 0x17000645 RID: 1605
	// (get) Token: 0x060041C2 RID: 16834 RVA: 0x0014B5CB File Offset: 0x001497CB
	// (set) Token: 0x060041C3 RID: 16835 RVA: 0x0014B5D3 File Offset: 0x001497D3
	private protected int maxEntires { protected get; private set; }

	// Token: 0x17000646 RID: 1606
	// (get) Token: 0x060041C4 RID: 16836
	protected abstract int pageSize { get; }

	// Token: 0x17000647 RID: 1607
	// (get) Token: 0x060041C5 RID: 16837
	protected abstract int entriesCount { get; }

	// Token: 0x060041C6 RID: 16838 RVA: 0x0014B5DC File Offset: 0x001497DC
	protected virtual void Start()
	{
		Debug.Log("base page handler " + this.entriesCount.ToString() + " " + this.pageSize.ToString());
		this.pages = this.entriesCount / this.pageSize + 1;
		this.maxEntires = this.pages * this.pageSize;
	}

	// Token: 0x060041C7 RID: 16839 RVA: 0x0014B644 File Offset: 0x00149844
	public void SelectEntryOnPage(int entryIndex)
	{
		int num = entryIndex + this.pageSize * this.currentPage;
		if (num > this.entriesCount)
		{
			return;
		}
		this.selectedIndex = num;
		this.PageEntrySelected(entryIndex, this.selectedIndex);
	}

	// Token: 0x060041C8 RID: 16840 RVA: 0x0014B680 File Offset: 0x00149880
	public void SelectEntryFromIndex(int index)
	{
		this.selectedIndex = index;
		this.currentPage = this.selectedIndex / this.pageSize;
		int pageEntry = index - this.pageSize * this.currentPage;
		this.PageEntrySelected(pageEntry, index);
		this.SetPage(this.currentPage);
	}

	// Token: 0x060041C9 RID: 16841 RVA: 0x0014B6CC File Offset: 0x001498CC
	public void ChangePage(bool left)
	{
		int num = left ? -1 : 1;
		this.SetPage(Mathf.Abs((this.currentPage + num) % this.pages));
	}

	// Token: 0x060041CA RID: 16842 RVA: 0x0014B6FC File Offset: 0x001498FC
	public void SetPage(int page)
	{
		if (page > this.pages)
		{
			return;
		}
		this.currentPage = page;
		int num = this.pageSize * page;
		this.ShowPage(this.currentPage, num, Mathf.Min(num + this.pageSize, this.entriesCount));
	}

	// Token: 0x060041CB RID: 16843
	protected abstract void ShowPage(int selectedPage, int startIndex, int endIndex);

	// Token: 0x060041CC RID: 16844
	protected abstract void PageEntrySelected(int pageEntry, int selectionIndex);
}
