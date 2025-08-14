using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200025F RID: 607
public class WatchableGenericSO<T> : ScriptableObject
{
	// Token: 0x17000158 RID: 344
	// (get) Token: 0x06000E0F RID: 3599 RVA: 0x000567A3 File Offset: 0x000549A3
	// (set) Token: 0x06000E10 RID: 3600 RVA: 0x000567AB File Offset: 0x000549AB
	private T _value { get; set; }

	// Token: 0x17000159 RID: 345
	// (get) Token: 0x06000E11 RID: 3601 RVA: 0x000567B4 File Offset: 0x000549B4
	// (set) Token: 0x06000E12 RID: 3602 RVA: 0x000567C4 File Offset: 0x000549C4
	public T Value
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
			foreach (Action<T> action in this.callbacks)
			{
				action(value);
			}
		}
	}

	// Token: 0x06000E13 RID: 3603 RVA: 0x00056824 File Offset: 0x00054A24
	private void EnsureInitialized()
	{
		if (!this.enterPlayID.IsCurrent)
		{
			this._value = this.InitialValue;
			this.callbacks = new List<Action<T>>();
			this.enterPlayID = EnterPlayID.GetCurrent();
		}
	}

	// Token: 0x06000E14 RID: 3604 RVA: 0x00056858 File Offset: 0x00054A58
	public void AddCallback(Action<T> callback, bool shouldCallbackNow = false)
	{
		this.EnsureInitialized();
		this.callbacks.Add(callback);
		if (shouldCallbackNow)
		{
			T value = this._value;
			foreach (Action<T> action in this.callbacks)
			{
				action(value);
			}
		}
	}

	// Token: 0x06000E15 RID: 3605 RVA: 0x000568C8 File Offset: 0x00054AC8
	public void RemoveCallback(Action<T> callback)
	{
		this.EnsureInitialized();
		this.callbacks.Remove(callback);
	}

	// Token: 0x040016CA RID: 5834
	public T InitialValue;

	// Token: 0x040016CC RID: 5836
	private EnterPlayID enterPlayID;

	// Token: 0x040016CD RID: 5837
	private List<Action<T>> callbacks;
}
