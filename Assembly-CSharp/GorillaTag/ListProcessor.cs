using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000E93 RID: 3731
	public class ListProcessor<T>
	{
		// Token: 0x1700090D RID: 2317
		// (get) Token: 0x06005D6C RID: 23916 RVA: 0x001D7E65 File Offset: 0x001D6065
		// (set) Token: 0x06005D6D RID: 23917 RVA: 0x001D7E6D File Offset: 0x001D606D
		public InAction<T> ItemProcessor
		{
			get
			{
				return this.m_itemProcessorDelegate;
			}
			set
			{
				this.m_itemProcessorDelegate = value;
			}
		}

		// Token: 0x06005D6E RID: 23918 RVA: 0x001D7E76 File Offset: 0x001D6076
		public ListProcessor() : this(10, null)
		{
		}

		// Token: 0x06005D6F RID: 23919 RVA: 0x001D7E81 File Offset: 0x001D6081
		public ListProcessor(int capacity, InAction<T> itemProcessorDelegate = null)
		{
			this.m_list = new List<T>(capacity);
			this.m_currentIndex = -1;
			this.m_listCount = -1;
			this.m_itemProcessorDelegate = itemProcessorDelegate;
		}

		// Token: 0x06005D70 RID: 23920 RVA: 0x001D7EAA File Offset: 0x001D60AA
		public void Add(in T item)
		{
			this.m_listCount++;
			this.m_list.Add(item);
		}

		// Token: 0x06005D71 RID: 23921 RVA: 0x001D7ECC File Offset: 0x001D60CC
		public void Remove(in T item)
		{
			int num = this.m_list.IndexOf(item);
			if (num < 0)
			{
				return;
			}
			if (num < this.m_currentIndex)
			{
				this.m_currentIndex--;
			}
			this.m_listCount--;
			this.m_list.RemoveAt(num);
		}

		// Token: 0x06005D72 RID: 23922 RVA: 0x001D7F21 File Offset: 0x001D6121
		public void Clear()
		{
			this.m_list.Clear();
			this.m_currentIndex = -1;
			this.m_listCount = -1;
		}

		// Token: 0x06005D73 RID: 23923 RVA: 0x001D7F3C File Offset: 0x001D613C
		public virtual void ProcessListSafe()
		{
			if (this.m_itemProcessorDelegate == null)
			{
				Debug.LogError("ListProcessor: ItemProcessor is null");
				return;
			}
			this.m_listCount = this.m_list.Count;
			this.m_currentIndex = 0;
			while (this.m_currentIndex < this.m_listCount)
			{
				try
				{
					InAction<T> itemProcessorDelegate = this.m_itemProcessorDelegate;
					T t = this.m_list[this.m_currentIndex];
					itemProcessorDelegate(t);
				}
				catch (Exception ex)
				{
					Debug.LogError(ex.ToString());
				}
				this.m_currentIndex++;
			}
		}

		// Token: 0x06005D74 RID: 23924 RVA: 0x001D7FD0 File Offset: 0x001D61D0
		public virtual void ProcessList()
		{
			if (this.m_itemProcessorDelegate == null)
			{
				Debug.LogError("ListProcessor: ItemProcessor is null");
				return;
			}
			this.m_listCount = this.m_list.Count;
			this.m_currentIndex = 0;
			while (this.m_currentIndex < this.m_listCount)
			{
				InAction<T> itemProcessorDelegate = this.m_itemProcessorDelegate;
				T t = this.m_list[this.m_currentIndex];
				itemProcessorDelegate(t);
				this.m_currentIndex++;
			}
		}

		// Token: 0x0400675E RID: 26462
		protected readonly List<T> m_list;

		// Token: 0x0400675F RID: 26463
		protected int m_currentIndex;

		// Token: 0x04006760 RID: 26464
		protected int m_listCount;

		// Token: 0x04006761 RID: 26465
		protected InAction<T> m_itemProcessorDelegate;
	}
}
