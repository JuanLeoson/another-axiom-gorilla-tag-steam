using System;
using UnityEngine;

// Token: 0x020007A7 RID: 1959
[Serializable]
public class Ref<T> where T : class
{
	// Token: 0x1700049B RID: 1179
	// (get) Token: 0x06003140 RID: 12608 RVA: 0x001009E3 File Offset: 0x000FEBE3
	// (set) Token: 0x06003141 RID: 12609 RVA: 0x001009EB File Offset: 0x000FEBEB
	public T AsT
	{
		get
		{
			return this;
		}
		set
		{
			this._target = (value as Object);
		}
	}

	// Token: 0x06003142 RID: 12610 RVA: 0x00100A00 File Offset: 0x000FEC00
	public static implicit operator bool(Ref<T> r)
	{
		Object @object = (r != null) ? r._target : null;
		return @object != null && @object != null;
	}

	// Token: 0x06003143 RID: 12611 RVA: 0x00100A28 File Offset: 0x000FEC28
	public static implicit operator T(Ref<T> r)
	{
		Object @object = (r != null) ? r._target : null;
		if (@object == null)
		{
			return default(T);
		}
		if (@object == null)
		{
			return default(T);
		}
		return @object as T;
	}

	// Token: 0x06003144 RID: 12612 RVA: 0x00100A70 File Offset: 0x000FEC70
	public static implicit operator Object(Ref<T> r)
	{
		Object @object = (r != null) ? r._target : null;
		if (@object == null)
		{
			return null;
		}
		if (@object == null)
		{
			return null;
		}
		return @object;
	}

	// Token: 0x04003CDF RID: 15583
	[SerializeField]
	private Object _target;
}
