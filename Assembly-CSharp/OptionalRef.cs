using System;
using UnityEngine;

// Token: 0x02000887 RID: 2183
[Serializable]
public class OptionalRef<T> where T : Object
{
	// Token: 0x17000534 RID: 1332
	// (get) Token: 0x060036B6 RID: 14006 RVA: 0x0011DFA5 File Offset: 0x0011C1A5
	// (set) Token: 0x060036B7 RID: 14007 RVA: 0x0011DFAD File Offset: 0x0011C1AD
	public bool enabled
	{
		get
		{
			return this._enabled;
		}
		set
		{
			this._enabled = value;
		}
	}

	// Token: 0x17000535 RID: 1333
	// (get) Token: 0x060036B8 RID: 14008 RVA: 0x0011DFB8 File Offset: 0x0011C1B8
	// (set) Token: 0x060036B9 RID: 14009 RVA: 0x0011DFE0 File Offset: 0x0011C1E0
	public T Value
	{
		get
		{
			if (this)
			{
				return this._target;
			}
			return default(T);
		}
		set
		{
			this._target = (value ? value : default(T));
		}
	}

	// Token: 0x060036BA RID: 14010 RVA: 0x0011E00C File Offset: 0x0011C20C
	public static implicit operator bool(OptionalRef<T> r)
	{
		if (r == null)
		{
			return false;
		}
		if (!r._enabled)
		{
			return false;
		}
		Object @object = r._target;
		return @object != null && @object;
	}

	// Token: 0x060036BB RID: 14011 RVA: 0x0011E040 File Offset: 0x0011C240
	public static implicit operator T(OptionalRef<T> r)
	{
		if (r == null)
		{
			return default(T);
		}
		if (!r._enabled)
		{
			return default(T);
		}
		Object @object = r._target;
		if (@object == null)
		{
			return default(T);
		}
		if (!@object)
		{
			return default(T);
		}
		return @object as T;
	}

	// Token: 0x060036BC RID: 14012 RVA: 0x0011E0A4 File Offset: 0x0011C2A4
	public static implicit operator Object(OptionalRef<T> r)
	{
		if (r == null)
		{
			return null;
		}
		if (!r._enabled)
		{
			return null;
		}
		Object @object = r._target;
		if (@object == null)
		{
			return null;
		}
		if (!@object)
		{
			return null;
		}
		return @object;
	}

	// Token: 0x040043A9 RID: 17321
	[SerializeField]
	private bool _enabled;

	// Token: 0x040043AA RID: 17322
	[SerializeField]
	private T _target;
}
