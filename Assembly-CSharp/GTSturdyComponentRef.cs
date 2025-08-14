using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020001D0 RID: 464
[Serializable]
public struct GTSturdyComponentRef<T> where T : Component
{
	// Token: 0x17000123 RID: 291
	// (get) Token: 0x06000B7E RID: 2942 RVA: 0x0003FF69 File Offset: 0x0003E169
	// (set) Token: 0x06000B7F RID: 2943 RVA: 0x0003FF71 File Offset: 0x0003E171
	public Transform BaseXform
	{
		get
		{
			return this._baseXform;
		}
		set
		{
			this._baseXform = value;
		}
	}

	// Token: 0x17000124 RID: 292
	// (get) Token: 0x06000B80 RID: 2944 RVA: 0x0003FF7C File Offset: 0x0003E17C
	// (set) Token: 0x06000B81 RID: 2945 RVA: 0x0003FFEB File Offset: 0x0003E1EB
	public T Value
	{
		get
		{
			if (!this._value)
			{
				return this._value;
			}
			if (string.IsNullOrEmpty(this._relativePath))
			{
				return default(T);
			}
			Transform transform;
			if (!this._baseXform.TryFindByPath(this._relativePath, out transform, false))
			{
				return default(T);
			}
			this._value = transform.GetComponent<T>();
			return this._value;
		}
		set
		{
			this._value = value;
			this._relativePath = ((!value) ? this._baseXform.GetRelativePath(value.transform) : string.Empty);
		}
	}

	// Token: 0x06000B82 RID: 2946 RVA: 0x00040024 File Offset: 0x0003E224
	public static implicit operator T(GTSturdyComponentRef<T> sturdyRef)
	{
		return sturdyRef.Value;
	}

	// Token: 0x06000B83 RID: 2947 RVA: 0x00040030 File Offset: 0x0003E230
	public static implicit operator GTSturdyComponentRef<T>(T component)
	{
		return new GTSturdyComponentRef<T>
		{
			Value = component
		};
	}

	// Token: 0x04000E29 RID: 3625
	[SerializeField]
	private T _value;

	// Token: 0x04000E2A RID: 3626
	[SerializeField]
	private string _relativePath;

	// Token: 0x04000E2B RID: 3627
	[SerializeField]
	private Transform _baseXform;
}
