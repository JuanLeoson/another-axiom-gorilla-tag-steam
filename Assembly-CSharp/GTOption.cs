using System;
using UnityEngine;

// Token: 0x02000201 RID: 513
[Serializable]
public struct GTOption<T>
{
	// Token: 0x17000135 RID: 309
	// (get) Token: 0x06000C29 RID: 3113 RVA: 0x00041C7A File Offset: 0x0003FE7A
	public T ResolvedValue
	{
		get
		{
			if (!this.enabled)
			{
				return this.defaultValue;
			}
			return this.value;
		}
	}

	// Token: 0x06000C2A RID: 3114 RVA: 0x00041C91 File Offset: 0x0003FE91
	public GTOption(T defaultValue)
	{
		this.enabled = false;
		this.value = defaultValue;
		this.defaultValue = defaultValue;
	}

	// Token: 0x06000C2B RID: 3115 RVA: 0x00041CA8 File Offset: 0x0003FEA8
	public void ResetValue()
	{
		this.value = this.defaultValue;
	}

	// Token: 0x04000F02 RID: 3842
	[Tooltip("When checked, the filter is applied; when unchecked (default), it is ignored.")]
	[SerializeField]
	public bool enabled;

	// Token: 0x04000F03 RID: 3843
	[SerializeField]
	public T value;

	// Token: 0x04000F04 RID: 3844
	[NonSerialized]
	public readonly T defaultValue;
}
