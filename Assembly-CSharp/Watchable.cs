using System;
using System.Collections.Generic;

// Token: 0x0200025E RID: 606
public class Watchable<T>
{
	// Token: 0x17000157 RID: 343
	// (get) Token: 0x06000E09 RID: 3593 RVA: 0x00056694 File Offset: 0x00054894
	// (set) Token: 0x06000E0A RID: 3594 RVA: 0x0005669C File Offset: 0x0005489C
	public T value
	{
		get
		{
			return this._value;
		}
		set
		{
			T value2 = this._value;
			this._value = value;
			foreach (Action<T> action in this.callbacks)
			{
				action(value);
			}
		}
	}

	// Token: 0x06000E0B RID: 3595 RVA: 0x000566FC File Offset: 0x000548FC
	public Watchable()
	{
	}

	// Token: 0x06000E0C RID: 3596 RVA: 0x0005670F File Offset: 0x0005490F
	public Watchable(T initial)
	{
		this._value = initial;
	}

	// Token: 0x06000E0D RID: 3597 RVA: 0x0005672C File Offset: 0x0005492C
	public void AddCallback(Action<T> callback, bool shouldCallbackNow = false)
	{
		this.callbacks.Add(callback);
		if (shouldCallbackNow)
		{
			foreach (Action<T> action in this.callbacks)
			{
				action(this._value);
			}
		}
	}

	// Token: 0x06000E0E RID: 3598 RVA: 0x00056794 File Offset: 0x00054994
	public void RemoveCallback(Action<T> callback)
	{
		this.callbacks.Remove(callback);
	}

	// Token: 0x040016C8 RID: 5832
	private T _value;

	// Token: 0x040016C9 RID: 5833
	private List<Action<T>> callbacks = new List<Action<T>>();
}
