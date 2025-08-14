using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag
{
	// Token: 0x02000E49 RID: 3657
	[Serializable]
	public struct GTDirectAssetRef<T> : IEquatable<T> where T : Object
	{
		// Token: 0x170008DC RID: 2268
		// (get) Token: 0x06005BD8 RID: 23512 RVA: 0x001CF327 File Offset: 0x001CD527
		// (set) Token: 0x06005BD9 RID: 23513 RVA: 0x001CF32F File Offset: 0x001CD52F
		public T obj
		{
			get
			{
				return this._obj;
			}
			set
			{
				this._obj = value;
				this.edAssetPath = null;
			}
		}

		// Token: 0x06005BDA RID: 23514 RVA: 0x001CF32F File Offset: 0x001CD52F
		public GTDirectAssetRef(T theObj)
		{
			this._obj = theObj;
			this.edAssetPath = null;
		}

		// Token: 0x06005BDB RID: 23515 RVA: 0x001CF33F File Offset: 0x001CD53F
		public static implicit operator T(GTDirectAssetRef<T> refObject)
		{
			return refObject.obj;
		}

		// Token: 0x06005BDC RID: 23516 RVA: 0x001CF348 File Offset: 0x001CD548
		public static implicit operator GTDirectAssetRef<T>(T other)
		{
			return new GTDirectAssetRef<T>
			{
				obj = other
			};
		}

		// Token: 0x06005BDD RID: 23517 RVA: 0x001CF366 File Offset: 0x001CD566
		public bool Equals(T other)
		{
			return this.obj == other;
		}

		// Token: 0x06005BDE RID: 23518 RVA: 0x001CF380 File Offset: 0x001CD580
		public override bool Equals(object other)
		{
			T t = other as T;
			return t != null && this.Equals(t);
		}

		// Token: 0x06005BDF RID: 23519 RVA: 0x001CF3AA File Offset: 0x001CD5AA
		public override int GetHashCode()
		{
			if (!(this.obj != null))
			{
				return 0;
			}
			return this.obj.GetHashCode();
		}

		// Token: 0x06005BE0 RID: 23520 RVA: 0x001CF3D1 File Offset: 0x001CD5D1
		public static bool operator ==(GTDirectAssetRef<T> left, T right)
		{
			return left.Equals(right);
		}

		// Token: 0x06005BE1 RID: 23521 RVA: 0x001CF3DB File Offset: 0x001CD5DB
		public static bool operator !=(GTDirectAssetRef<T> left, T right)
		{
			return !(left == right);
		}

		// Token: 0x04006597 RID: 26007
		[SerializeField]
		[HideInInspector]
		internal T _obj;

		// Token: 0x04006598 RID: 26008
		[FormerlySerializedAs("assetPath")]
		public string edAssetPath;
	}
}
