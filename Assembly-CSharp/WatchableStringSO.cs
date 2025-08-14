using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000260 RID: 608
[CreateAssetMenu(fileName = "WatchableStringSO", menuName = "ScriptableObjects/WatchableStringSO")]
public class WatchableStringSO : ScriptableObject
{
	// Token: 0x1700015A RID: 346
	// (get) Token: 0x06000E17 RID: 3607 RVA: 0x000568DD File Offset: 0x00054ADD
	// (set) Token: 0x06000E18 RID: 3608 RVA: 0x000568E5 File Offset: 0x00054AE5
	private string _value { get; set; }

	// Token: 0x1700015B RID: 347
	// (get) Token: 0x06000E19 RID: 3609 RVA: 0x000568EE File Offset: 0x00054AEE
	// (set) Token: 0x06000E1A RID: 3610 RVA: 0x000568FC File Offset: 0x00054AFC
	public string Value
	{
		get
		{
			this.EnsureInitialized();
			return this._value;
		}
		set
		{
			this.EnsureInitialized();
			this._value = value;
			foreach (Action<string> action in this.callbacks)
			{
				action(value);
			}
		}
	}

	// Token: 0x06000E1B RID: 3611 RVA: 0x0005695C File Offset: 0x00054B5C
	private void EnsureInitialized()
	{
		if (!this.enterPlayID.IsCurrent)
		{
			this._value = this.InitialValue;
			this.callbacks = new List<Action<string>>();
			this.enterPlayID = EnterPlayID.GetCurrent();
		}
	}

	// Token: 0x06000E1C RID: 3612 RVA: 0x00056990 File Offset: 0x00054B90
	public void AddCallback(Action<string> callback, bool shouldCallbackNow = false)
	{
		this.EnsureInitialized();
		this.callbacks.Add(callback);
		if (shouldCallbackNow)
		{
			string value = this._value;
			foreach (Action<string> action in this.callbacks)
			{
				action(value);
			}
		}
	}

	// Token: 0x06000E1D RID: 3613 RVA: 0x00056A00 File Offset: 0x00054C00
	public void RemoveCallback(Action<string> callback)
	{
		this.EnsureInitialized();
		this.callbacks.Remove(callback);
	}

	// Token: 0x06000E1E RID: 3614 RVA: 0x00056A15 File Offset: 0x00054C15
	public override string ToString()
	{
		return this.Value;
	}

	// Token: 0x040016CE RID: 5838
	[TextArea]
	public string InitialValue;

	// Token: 0x040016D0 RID: 5840
	private EnterPlayID enterPlayID;

	// Token: 0x040016D1 RID: 5841
	private List<Action<string>> callbacks;
}
